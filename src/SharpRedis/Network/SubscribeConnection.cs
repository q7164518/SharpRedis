using SharpRedis.Commands;
using SharpRedis.Entities;
using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Exceptions;
using SharpRedis.Extensions;
using SharpRedis.Network.Pool;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Network;

internal sealed class SubscribeConnection : BaseConnection
{
    private Dictionary<string, ReceiveItem<OnReceive>> _onReceives;
    private Dictionary<string, ReceiveItem<POnReceive>> _ponReceives;
    private readonly ConcurrentQueue<(TaskCompletionSource<RedisResult> Result, PendingSubscription Subscription)> _pendingReplies = new();
    private SubscriptionConfirmation _restoreConfirmation = null!;

    internal SubscribeConnection(SharpConnectionOptions connectionOptions, CommandDispatcher commandDispatcher, int connectTimeout = 0)
        : base(connectionOptions, commandDispatcher, connectTimeout)
    {
        this._onReceives = [];
        this._ponReceives = [];
        base.ConnectionName += "_Subscribe";
        base.BeginReceive(null);
    }

    internal override void SendCommand(ReadOnlySpan<(ICommand command, TaskCompletionSource<RedisResult> resultTask)> commands, ref byte[] buffer)
    {
        if (!base.TryEnterOperation())
        {
            SetError(ref commands, "The current connection is unavailable.", this.Encoding);
            return;
        }
        try
        {
            int length = 0;
            if (!this.Available)
            {
                SetError(ref commands, "The current connection is unavailable.", base._connectionOptions.Encoding);
                return;
            }

            foreach (var (command, resultTask) in commands)
            {
                command.WriteToResp(this._connectionOptions.Encoding, ref buffer, ref length, this._connectionOptions.KeyPrefix, this._connectionOptions.KeySuffix);
                if (command is CommandSubscribe subscription)
                {
                    this._pendingReplies.Enqueue((resultTask, new PendingSubscription(subscription, this.Encoding)));
                }
                else if ((command.Mode & CommandMode.WithoutResult) != CommandMode.WithoutResult)
                {
                    this._pendingReplies.Enqueue((resultTask, null!));
                }
                if ((command.Mode & CommandMode.WithoutActiveTime) != CommandMode.WithoutActiveTime)
                {
                    base.LastActiveTime = DateTimeOffset.UtcNow;
                }
            }

            if (!this.Available) throw new IOException("The subscription connection is unavailable.");

            base.SendBuffer(buffer, length);
            foreach (var (command, resultTask) in commands)
            {
                if (command is not CommandSubscribe
                    && (command.Mode & CommandMode.WithoutResult) == CommandMode.WithoutResult)
                {
                    _ = resultTask.TrySetResult(RedisResult.Empty);
                }
            }
        }
        catch (Exception ex)
        {
            base._available = false;
            SetError(ref commands, ex.Message, this._connectionOptions.Encoding);
            return;
        }

        finally
        {
            this.ExitOperation();
        }

        static void SetError(ref readonly ReadOnlySpan<(ICommand command, TaskCompletionSource<RedisResult> resultTask)> commands, string errorMessage, Encoding encoding)
        {
            foreach (var (_, resultTask) in commands)
            {
                _ = resultTask.TrySetResult(new RedisResult(RespType.Exception, errorMessage, encoding));
            }
        }
    }

    private protected sealed override void Receive(IAsyncResult ar)
    {
        var buffer = this.ReadPacket(ar, out int end);
        int position = 0;
        while (position < end && DataPacketExtensions.GetRawValue(buffer, end, ref position, this.Encoding, true, ref this._partialReply, out var redisResult))
        {
            if (redisResult.Type is RespType.Array or RespType.Push
                && redisResult.RawValue is RedisResult[] resultArray)
            {
#if MODERN_NET
                string? first
#else
                string first
#endif
                    = resultArray[0];
                if (!string.IsNullOrEmpty(first))
                {
                    if ("invalidate".Equals(first, StringComparison.OrdinalIgnoreCase))
                    {
                        //resp3 key invalidate
                        if (!this._onReceives.TryGetValue(Config.CLIENT_CACHING_SUB_CHANNEL, out var receive)
                            || !receive.SubSucceed)
                        {
                            goto End;
                        }

                        _ = ThreadPool.QueueUserWorkItem(static async state =>
                        {
                            if (state is not SubInvoke invocation) return;
                            try
                            {
                                await invocation.Invoke().ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                SharpConsole.WriteError($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}. An unhandled exception occurred during the execution of the subscription event. Msg:{ex.Message}");
                            }
                            finally
                            {
                                invocation.Dispose();
                            }
                        }, new SubInvoke(receive.Func, Config.CLIENT_CACHING_SUB_CHANNEL, resultArray[1]));
                        goto End;
                    }
                    else if (!"pong".Equals(first, StringComparison.OrdinalIgnoreCase))
                    {
#if MODERN_NET
                        string? channel;
#else
                        string channel;
#endif
                        if (resultArray.Length is 4)
                        {
                            channel = resultArray[2];
                        }
                        else
                        {
                            channel = resultArray[1];
                        }
                        if (channel is null) goto End;

                        if ("message".Equals(first, StringComparison.OrdinalIgnoreCase))
                        {
                            if (resultArray.Length != 3
                                || !this._onReceives.TryGetValue(channel, out var receive)
                                || !receive.SubSucceed)
                            {
                                goto End;
                            }

                            _ = ThreadPool.QueueUserWorkItem(static async state =>
                            {
                                if (state is not SubInvoke invocation) return;
                                try
                                {
                                    await invocation.Invoke().ConfigureAwait(false);
                                }
                                catch (Exception ex)
                                {
                                    SharpConsole.WriteError($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}. An unhandled exception occurred during the execution of the subscription event. Msg:{ex.Message}");
                                }
                                finally
                                {
                                    invocation.Dispose();
                                }
                            }, new SubInvoke(receive.Func, channel, resultArray[2]));
                        }
                        else if ("pmessage".Equals(first, StringComparison.OrdinalIgnoreCase))
                        {
#if MODERN_NET
                            string? pattern
#else
                            string pattern
#endif
                                = resultArray[1];
                            if (pattern is null
                                || resultArray.Length != 4
                                || !this._ponReceives.TryGetValue(pattern, out var receive)
                                || !receive.SubSucceed)
                            {
                                goto End;
                            }

                            _ = ThreadPool.QueueUserWorkItem(static async state =>
                            {
                                if (state is not PSubInvoke invocation) return;
                                try
                                {
                                    await invocation.Invoke().ConfigureAwait(false);
                                }
                                catch (Exception ex)
                                {
                                    SharpConsole.WriteError($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}. An unhandled exception occurred during the execution of the subscription event. Msg:{ex.Message}");
                                }
                                finally
                                {
                                    invocation.Dispose();
                                }
                            }, new PSubInvoke(receive.Func, pattern, channel, resultArray[3]));
                        }
                        else if ("unsubscribe".Equals(first, StringComparison.OrdinalIgnoreCase))
                        {
                            while (this._onReceives.TryGetValue(channel, out var item))
                            {
                                item.Dispose();
                                _ = this._onReceives.Remove(channel);
                            }
                        }
                        else if ("punsubscribe".Equals(first, StringComparison.OrdinalIgnoreCase))
                        {
                            while (this._ponReceives.TryGetValue(channel, out var item))
                            {
                                item.Dispose();
                                _ = this._ponReceives.Remove(channel);
                            }
                        }
                        else if ("subscribe".Equals(first, StringComparison.OrdinalIgnoreCase))
                        {
                            this.ConfirmSubscription(channel, isPattern: false);
                        }
                        else if ("psubscribe".Equals(first, StringComparison.OrdinalIgnoreCase))
                        {
                            this.ConfirmSubscription(channel, isPattern: true);
                        }
                        goto End;
                    }
                    else
                    {
                        //resp2 ping result
                        redisResult = resultArray[1];
                    }
                }
            }

            if (redisResult.Type is RespType.SimpleErrors or RespType.BulkErrors
                && Volatile.Read(ref this._restoreConfirmation) is { } confirmation)
            {
                _ = confirmation.Completion.TrySetResult(redisResult);
            }
            else if (this._pendingReplies.TryDequeue(out var pending))
            {
                var result = redisResult;
                if (pending.Subscription != null
                    && result.Type is not (RespType.SimpleErrors or RespType.BulkErrors or RespType.Exception))
                {
                    result = new(RespType.Exception, new FormatException("Expected a subscription confirmation."), this.Encoding);
                }
                _ = pending.Result.TrySetResult(result);
            }

            End:;
        }

        if (base._disposedValue || !base.Available) return;
        this.KeepRemaining(buffer, position, end);
        this.ContinueReceive(null);
    }

    private void ConfirmSubscription(string channel, bool isPattern)
    {
        if (this._pendingReplies.TryPeek(out var pending) && pending.Subscription is { } subscription
            && subscription.TryConfirm(channel, isPattern))
        {
            if (isPattern)
            {
                var receive = new ReceiveItem<POnReceive>(subscription.PatternCallback);
                receive.Confirm();
                this._ponReceives[channel] = receive;
            }
            else
            {
                var receive = new ReceiveItem<OnReceive>(subscription.ChannelCallback);
                receive.Confirm();
                this._onReceives[channel] = receive;
            }

            if (subscription.IsComplete && this._pendingReplies.TryDequeue(out _))
            {
                _ = pending.Result.TrySetResult(RedisResult.Empty);
            }
        }
        else if (isPattern)
        {
            if (this._ponReceives.TryGetValue(channel, out var receive)) receive.Confirm();
        }
        else
        {
            if (this._onReceives.TryGetValue(channel, out var receive)) receive.Confirm();
        }
    }

    protected private override void FailPending(Exception ex)
    {
        base.FailPending(ex);
        var result = new RedisResult(RespType.Exception, ex, this.Encoding);
        while (this._pendingReplies.TryDequeue(out var pending))
        {
            _ = pending.Result.TrySetResult(result);
        }
        _ = Volatile.Read(ref this._restoreConfirmation)?.Completion.TrySetResult(result);
    }

    internal sealed override BaseConnection Copy(IConnectionPool pool)
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(base._disposedValue || base._connectionOptions is null, this);
#else
        if (base._disposedValue || base._connectionOptions is null)
        {
            throw new ObjectDisposedException(nameof(SubscribeConnection), "The current connection object has been released and cannot be copied.");
        }
#endif
        var connection = (SubscribeConnection)pool.CreateSubConnection();
        try
        {
            connection.RestoreSubscriptions(
                this._onReceives,
                connection._onReceives,
                static channels => PubSubCommands.Subscribe(new SharpValueSpan(channels)));

            connection.RestoreSubscriptions(
                this._ponReceives,
                connection._ponReceives,
                static patterns => PubSubCommands.PSubscribe(new SharpValueSpan(patterns)));

            if (!connection.Available)
            {
                throw new RedisInitializationException("The subscription connection closed during restoration.");
            }

            if (pool.ClientCaching is { } clientCaching)
            {
                clientCaching.ClearCache();
                clientCaching.RedirectConnectionId = connection.ClientId;
                pool.RetryTracking();
            }

            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    private void RestoreSubscriptions<TReceive>(
        Dictionary<string, ReceiveItem<TReceive>> source,
        Dictionary<string, ReceiveItem<TReceive>> destination,
        Func<ICollection<string>, CommandPacket> createCommand)
        where TReceive : Delegate
    {
        if (source.Count == 0) return;

        var subscriptions = new Dictionary<string, ReceiveItem<TReceive>>(source);
        var confirmation = new SubscriptionConfirmation(subscriptions.Count);
        foreach (var subscription in subscriptions)
        {
            destination.Add(subscription.Key, new ReceiveItem<TReceive>(subscription.Value.Func, confirmation));
        }

        using var command = createCommand(subscriptions.Keys);
        TaskCompletionSource<RedisResult> sent = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Volatile.Write(ref this._restoreConfirmation, confirmation);
        try
        {
            this.CommandEnqueue(command, sent, this._connectionOptions.CommandTimeout);
            if (!sent.WaitForCompletion(this._connectionOptions.CommandTimeout))
            {
                throw new RedisInitializationException("Timed out while sending the subscription command.");
            }
            this.WaitForRestoration(sent.Task, "sending the subscription command");
            this.WaitForRestoration(confirmation.Completion.Task, "waiting for subscription confirmations");
        }
        finally
        {
            Volatile.Write(ref this._restoreConfirmation, null!);
        }
    }

    private void WaitForRestoration(Task<RedisResult> task, string operation)
    {
        if (!task.Wait(this._connectionOptions.CommandTimeout))
        {
            throw new RedisInitializationException($"Timed out while {operation}.");
        }

        var result = task.GetAwaiter().GetResult();
        if (result.Type != RespType.Empty)
        {
            throw new RedisInitializationException($"Failed while {operation}: {result.RawValue}");
        }
    }

    internal override sealed void RetryTracking(ClientCachingBase clientCaching)
    {
    }

    protected private sealed override void Dispose(bool disposing)
    {
        if (!base._disposedValue)
        {
            base._disposedValue = true;
            base.Dispose(disposing);
            if (disposing)
            {
                if (this._onReceives is { Count: > 0 })
                {
                    foreach (var item in this._onReceives)
                    {
                        item.Value.Dispose();
                    }
                }
                this._onReceives?.Clear();

                if (this._ponReceives is { Count: > 0 })
                {
                    foreach (var item in this._ponReceives)
                    {
                        item.Value.Dispose();
                    }
                }
                this._ponReceives?.Clear();
            }
            this._onReceives = null!;
            this._ponReceives = null!;
        }
    }

    private sealed class PendingSubscription
    {
        private readonly Queue<string> _channels = new();
        private readonly bool _isPattern;

        internal OnReceive ChannelCallback { get; }
        internal POnReceive PatternCallback { get; }
        internal bool IsComplete => this._channels.Count == 0;

        internal PendingSubscription(CommandSubscribe command, Encoding encoding)
        {
            this._isPattern = (command.Mode & CommandMode.PSub) == CommandMode.PSub;
            this.ChannelCallback = command.OnReceive!;
            this.PatternCallback = command.POnReceive!;
            foreach (var item in command.Items)
            {
                if (item.Category == CommandItemCategory.Argument) this._channels.Enqueue(item.ToString(encoding));
            }
        }

        internal bool TryConfirm(string channel, bool isPattern)
        {
            if (this._isPattern != isPattern || this._channels.Count == 0
                || !string.Equals(this._channels.Peek(), channel, StringComparison.Ordinal)) return false;

            _ = this._channels.Dequeue();
            return true;
        }
    }

    private sealed class SubscriptionConfirmation(int count)
    {
        private int _remaining = count;

        internal TaskCompletionSource<RedisResult> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal void Confirm()
        {
            if (--this._remaining == 0) _ = this.Completion.TrySetResult(RedisResult.Empty);
        }
    }

    private sealed class ReceiveItem<TReceive> : IDisposable
        where TReceive : Delegate
    {
        private bool _disposedValue;
        private TReceive _func;

        private SubscriptionConfirmation _confirmation;

        internal bool SubSucceed { get; private set; }

        internal TReceive Func => this._func;

        internal ReceiveItem(TReceive func, SubscriptionConfirmation confirmation = null!)
        {
            this._func = func;
            this._confirmation = confirmation;
        }

        internal void Confirm()
        {
            this.SubSucceed = true;
            var confirmation = this._confirmation;
            this._confirmation = null!;
            confirmation?.Confirm();
        }

        #region Dispose
        private void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                this._disposedValue = true;
                if (disposing)
                {
                }

                this._func = null!;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    private sealed class SubInvoke : IDisposable
    {
        private bool _disposedValue;
        private OnReceive _func;
        private string _channel;
        private readonly RedisResult _data;

        internal SubInvoke(OnReceive func, string channel, RedisResult data)
        {
            this._func = func;
            this._channel = channel;
            this._data = data;
        }

        internal async Task Invoke()
        {
            if (this._func != null)
                await this._func.Invoke(this._channel, this._data).ConfigureAwait(false);
        }

        #region Dispose
        private void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                this._disposedValue = true;
                if (disposing)
                {
                }
                this._func = null!;
                this._channel = null!;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    private sealed class PSubInvoke : IDisposable
    {
        private bool _disposedValue;
        private POnReceive _func;
        private string _pattern;
        private string _channel;
        private readonly RedisResult _data;

        internal PSubInvoke(POnReceive func, string pattern, string channel, RedisResult data)
        {
            this._func = func;
            this._pattern = pattern;
            this._channel = channel;
            this._data = data;
        }

        internal async Task Invoke()
        {
            if (this._func != null)
                await this._func.Invoke(this._pattern, this._channel, this._data).ConfigureAwait(false);
        }

        #region Dispose
        private void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                this._disposedValue = true;
                if (disposing)
                {
                }
                this._func = null!;
                this._channel = null!;
                this._pattern = null!;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
