#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
#if NET8_0_OR_GREATER
#pragma warning disable IDE0028
#endif
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using AliasTaskCompletionSource = System.Threading.Tasks.TaskCompletionSource<object?>;
#else
#if !LOW_NET
using AliasTaskCompletionSource = System.Threading.Tasks.TaskCompletionSource<object>;
#endif
#endif
#if !NET30
using System.Linq;
#endif
#if !LOW_NET
using System.Threading.Tasks;
#endif
using SharpRedis.Models;
using SharpRedis.Network.Standard;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using SharpRedis.Extensions;
using System;

namespace SharpRedis.Network
{
    internal sealed class SubConnection : BaseConnection
    {
        private volatile int _subCount = 0;
        private Dictionary<ChannelMode, OnReceiveModel<OnReceive>> _onReceives = new Dictionary<ChannelMode, OnReceiveModel<OnReceive>>();
        private Dictionary<ChannelMode, OnReceiveModel<POnReceive>> _pOnReceives = new Dictionary<ChannelMode, OnReceiveModel<POnReceive>>();

        internal Dictionary<ChannelMode, OnReceiveModel<OnReceive>> OnReceives => this._onReceives;

        internal Dictionary<ChannelMode, OnReceiveModel<POnReceive>> POnReceives => this._pOnReceives;

        internal ref readonly int SubCount => ref this._subCount;

        internal SubConnection(Encoding encoding, ConnectionOptions connectionOptions)
            : base(encoding, connectionOptions, ConnectionType.PubSub, null)
        {
        }

        #region ReceiveCompleted
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        sealed private protected override void ReceiveCompleted(object? sender, SocketAsyncEventArgs e)
#else
        sealed private protected override void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
#endif
        {
            if (e.SocketError != SocketError.Success || e.BytesTransferred is 0)
            {
                this._connected = false;
                if (base._currentIsSync)
                {
                    e.UserToken = new RedisConnectionException("Unexpected connection closure");
                    return;
                }
#if !LOW_NET
                else
                {
                    if (e.UserToken is AliasTaskCompletionSource lastTcs)
                    {
                        lock (lastTcs)
                        {
                            if (lastTcs.Task.Status == TaskStatus.WaitingForActivation)
                                lastTcs.SetResult(new RedisConnectionException("Unexpected connection closure"));
                        }
                        return;
                    }
                }
#endif
                e.UserToken = null;
                return;
            }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            base._dataPacket.Write(e.MemoryBuffer.Slice(e.Offset, e.BytesTransferred).Span);
#else
            base._dataPacket.Write(e.Buffer, e.Offset, e.BytesTransferred);
#endif
            _ = base._dataPacket.Seek(0, SeekOrigin.Begin);

            while (true)
            {
                if (!DataPacketExtensions.GetNextValue(this._dataPacket, base._encoding, ResultDataType.Bytes, out var data))
                {
                    _ = this._dataPacket.Seek(this._dataPacket.Length, SeekOrigin.Begin);
                    break;
                }
                if (base._dataPacket.Position == base._dataPacket.Length) base._dataPacket.SetLength(0);

                if (data is object[] array)
                {
                    var first = ConvertExtensions.To<string>(array[0], ResultType.String, base.Encoding);
                    if (!string.IsNullOrEmpty(first))
                    {
                        if (!"pong".Equals(first, StringComparison.OrdinalIgnoreCase))
                        {
                            switch (first.ToLower())
                            {
                                case "subscribe":
                                case "psubscribe":
                                    continue;
                                case "message":
                                    {
                                        if (this._onReceives is null || this._onReceives.Count <= 0) continue;
                                        if (array.Length is 3)
                                        {
                                            var channel = ConvertExtensions.To<string>(array[1], ResultType.String, base.Encoding);
                                            if (!string.IsNullOrEmpty(channel)
                                                && this._onReceives.TryGetValue(new ChannelMode(channel, ChannelModeEnum.Default), out var receive))
                                            {
                                                _ = ThreadPool.QueueUserWorkItem(SubConnection.OnReceiveInvoke, new OnReceiveInvokeItem(receive, channel, array[2], base._encoding));
                                            }
                                        }
                                        continue;
                                    }
                                case "pmessage":
                                    {
                                        if (this._pOnReceives is null || this._pOnReceives.Count <= 0) continue;
                                        if (array.Length is 4)
                                        {
                                            var pattern = ConvertExtensions.To<string>(array[1], ResultType.String, base.Encoding);
                                            var channel = ConvertExtensions.To<string>(array[2], ResultType.String, base.Encoding);
                                            if (!string.IsNullOrEmpty(pattern)
                                                && !string.IsNullOrEmpty(channel)
                                                && this._pOnReceives.TryGetValue(new ChannelMode(pattern, ChannelModeEnum.Default), out var preceive))
                                            {
                                                _ = ThreadPool.QueueUserWorkItem(SubConnection.POnReceiveInvoke, new POnReceiveInvokeItem(preceive, pattern, channel, array[3], base._encoding));
                                            }
                                        }
                                        continue;
                                    }
                                case "smessage":
                                    {
                                        if (this._onReceives is null || this._onReceives.Count <= 0) continue;
                                        if (array.Length is 3)
                                        {
                                            var channel = ConvertExtensions.To<string>(array[1], ResultType.String, base.Encoding);
                                            if (!string.IsNullOrEmpty(channel)
                                                && this._onReceives.TryGetValue(new ChannelMode(channel, ChannelModeEnum.Shard), out var receive))
                                            {
                                                _ = ThreadPool.QueueUserWorkItem(SubConnection.OnReceiveInvoke, new OnReceiveInvokeItem(receive, channel, array[2], base._encoding));
                                            }
                                        }
                                        continue;
                                    }
                                case "unsubscribe":
                                    {
                                        if (this._onReceives is null || this._onReceives.Count <= 0) continue;
                                        if (array.Length is 3)
                                        {
                                            var channel = ConvertExtensions.To<string>(array[1], ResultType.String, base.Encoding);
                                            if (!string.IsNullOrEmpty(channel))
                                            {
                                                if (this._onReceives.Remove(new ChannelMode(channel, ChannelModeEnum.Default)))
                                                {
                                                    _ = Interlocked.Decrement(ref this._subCount);
                                                }
                                            }
                                        }
                                        continue;
                                    }
                                case "punsubscribe":
                                    {
                                        if (this._pOnReceives is null) continue;
                                        if (array.Length is 3)
                                        {
                                            var pattern = ConvertExtensions.To<string>(array[1], ResultType.String, base.Encoding);
                                            if (!string.IsNullOrEmpty(pattern))
                                            {
                                                if (this._pOnReceives.Remove(new ChannelMode(pattern, ChannelModeEnum.Default)))
                                                {
                                                    _ = Interlocked.Decrement(ref this._subCount);
                                                }
                                            }
                                        }
                                        continue;
                                    }
                                case "sunsubscribe":
                                    {
                                        if (this._onReceives is null || this._onReceives.Count <= 0) continue;
                                        if (array.Length is 3)
                                        {
                                            var channel = ConvertExtensions.To<string>(array[1], ResultType.String, base.Encoding);
                                            if (!string.IsNullOrEmpty(channel))
                                            {
                                                if (this._onReceives.Remove(new ChannelMode(channel, ChannelModeEnum.Shard)))
                                                {
                                                    _ = Interlocked.Decrement(ref this._subCount);
                                                }
                                            }
                                        }
                                        continue;
                                    }
                                case "invalidate":
                                    {
                                        if (this._onReceives is null || this._onReceives.Count <= 0) continue;
                                        if (array.Length is 2
                                            && this._onReceives.TryGetValue(new ChannelMode(ClientSideCachingExtensions._invalidate_channel_name, ChannelModeEnum.Default), out var receive))
                                        {
                                            _ = ThreadPool.QueueUserWorkItem(SubConnection.OnReceiveInvoke, new OnReceiveInvokeItem(receive, ClientSideCachingExtensions._invalidate_channel_name, array[1], base._encoding));
                                        }
                                        continue;
                                    }
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            if (base._currentIsSync)
                            {
                                e.UserToken = array[1];
                                _ = base._autoResetEvent.Set();
                                continue;
                            }
#if !LOW_NET
                            else
                            {
                                if (e.UserToken is AliasTaskCompletionSource tcs)
                                {
                                    lock (tcs)
                                    {
                                        if (tcs.Task.Status == TaskStatus.WaitingForActivation)
                                        {
                                            tcs.SetResult(array[1]);
                                        }
                                    }
                                }
                            }
#endif
                        }
                    }
                }
                else
                {
                    if (base._currentIsSync)
                    {
                        e.UserToken = data;
                        _ = this._autoResetEvent.Set();
                    }
#if !LOW_NET
                    else
                    {
                        if (e.UserToken is AliasTaskCompletionSource tcs)
                        {
                            lock (tcs)
                            {
                                if (tcs.Task.Status == TaskStatus.WaitingForActivation)
                                {
                                    tcs.SetResult(data);
                                }
                            }
                        }
                    }
#endif
                }
            }

            if (!base._socketClient.ReceiveAsync(e))
            {
                this.ReceiveCompleted(sender, e);
            }
        }
        #endregion

        #region Sync
        #region Subscribe
        internal void Subscribe(CommandPacket command, ChannelMode[] channels, OnReceiveModel<OnReceive> onReceive, CancellationToken cancellationToken)
        {
#if NET30
            var channelNames = new List<string>();
            for (uint i = 0; i < channels.Length; i++)
            {
                channelNames.Add(channels[i].Channel);
            }
            _ = base.ExecuteCommand(command.WriteValues(channelNames.ToArray()), cancellationToken);
#else
            _ = base.ExecuteCommand(command.WriteValues(channels.Select(f => f.Channel).ToArray()), cancellationToken);
#endif
            this.BindOnReceive(channels, onReceive);
        }

        internal void PSubscribe(CommandPacket command, ChannelMode[] patterns, OnReceiveModel<POnReceive> onReceive, CancellationToken cancellationToken)
        {
#if NET30
            var patternNames = new List<string>();
            for (uint i = 0; i < patterns.Length; i++)
            {
                patternNames.Add(patterns[i].Channel);
            }
            _ = base.ExecuteCommand(command.WriteValues(patternNames.ToArray()), cancellationToken);
#else
            _ = base.ExecuteCommand(command.WriteValues(patterns.Select(f => f.Channel).ToArray()), cancellationToken);
#endif
            this.BindPOnReceive(patterns, onReceive);
        }
        #endregion

        #region UnSubscribe
        public void UnSubscribe(CommandPacket command, ChannelMode[] channels, CancellationToken cancellationToken)
        {
            if (channels != null && channels.Length > 0)
            {
#if NET30
                var channelNames = new List<string>();
                for (uint i = 0; i < channels.Length; i++)
                {
                    channelNames.Add(channels[i].Channel);
                }
                command.WriteValues(channelNames.ToArray());
#else
                command.WriteValues(channels.Select(f => f.Channel).ToArray());
#endif
            }
            _ = base.ExecuteCommand(command, cancellationToken);
        }
        #endregion
        #endregion

        #region Async
#if !LOW_NET
        #region Subscribe
        async internal Task SubscribeAsync(CommandPacket command, ChannelMode[] channels, OnReceiveModel<OnReceive> onReceive, CancellationToken cancellationToken)
        {
            _ = await base.ExecuteCommandAsync(command.WriteValues(channels.Select(f => f.Channel).ToArray()), cancellationToken).ConfigureAwait(false);
            this.BindOnReceive(channels, onReceive);
        }

        internal async Task PSubscribeAsync(CommandPacket command, ChannelMode[] patterns, OnReceiveModel<POnReceive> onReceive, CancellationToken cancellationToken)
        {
            _ = await base.ExecuteCommandAsync(command.WriteValues(patterns.Select(f => f.Channel).ToArray()), cancellationToken).ConfigureAwait(false);
            this.BindPOnReceive(patterns, onReceive);
        }
        #endregion

        #region UnSubscribe
        public Task UnSubscribeAsync(CommandPacket command, ChannelMode[] channels, CancellationToken cancellationToken)
        {
            if (channels != null && channels.Length > 0)
            {
                command.WriteValues(channels.Select(f => f.Channel).ToArray());
            }
            return base.ExecuteCommandAsync(command, cancellationToken);
        }
        #endregion
#endif
        #endregion

        #region Private methods
        private void BindOnReceive(ChannelMode[] channels, OnReceiveModel<OnReceive> onReceive)
        {
            if (this._onReceives is null) return;
            foreach (var channel in channels)
            {
                if (!this._onReceives.ContainsKey(channel))
                {
                    _ = Interlocked.Increment(ref this._subCount);
                }
                this._onReceives[channel] = onReceive;
            }
        }

        private void BindPOnReceive(ChannelMode[] patterns, OnReceiveModel<POnReceive> onReceive)
        {
            if (this._pOnReceives is null) return;
            foreach (var pattern in patterns)
            {
                if (!this._pOnReceives.ContainsKey(pattern))
                {
                    _ = Interlocked.Increment(ref this._subCount);
                }
                this._pOnReceives[pattern] = onReceive;
            }
        }
        #endregion

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static void OnReceiveInvoke(object? state)
#else
        private static void OnReceiveInvoke(object state)
#endif
        {
            if (state is OnReceiveInvokeItem item)
            {
                item.Invoke();
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static void POnReceiveInvoke(object? state)
#else
        private static void POnReceiveInvoke(object state)
#endif
        {
            if (state is POnReceiveInvokeItem item)
            {
                item.Invoke();
            }
        }

        sealed protected private override void Dispose(bool disposing)
        {
            if (base._disposedValue) return;
            this._onReceives?.Clear();
            this._pOnReceives?.Clear();
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
            this._onReceives = null;
            this._pOnReceives = null;
            base.Dispose(disposing);
        }

        ~SubConnection()
        {
            this.Dispose(true);
        }

        private sealed class OnReceiveInvokeItem : IDisposable
        {
            private OnReceiveModel<OnReceive> _onReceive;
            private string _channel;
            private object _data;
            private Encoding _encoding;
            private bool _disposedValue;

            internal OnReceiveInvokeItem(OnReceiveModel<OnReceive> onReceive, string channel, object data, Encoding encoding)
            {
                this._onReceive = onReceive;
                this._channel = channel;
                this._data = data;
                this._encoding = encoding;
            }

            internal void Invoke()
            {
                if (this._onReceive.DataType is ResultDataType.Default)
                {
                    var str = ConvertExtensions.To<string>(this._data, ResultType.String, this._encoding);
                    if (str != null) this._onReceive.OnReceive.Invoke(this._channel, str);
                }
                else if (this._onReceive.DataType is ResultDataType.Bytes)
                {
                    if (this._data is byte[] bytes)
                    {
                        this._onReceive.OnReceive.Invoke(this._channel, bytes);
                    }
                }
            }

            private void Dispose(bool disposing)
            {
                if (!this._disposedValue)
                {
                    if (disposing)
                    {
                        this._disposedValue = true;
                        this._onReceive = null;
                        this._channel = null;
                        this._data = null;
                        this._encoding = null;
                    }
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            ~OnReceiveInvokeItem()
            {
                Dispose(disposing: true);
            }
        }

        private sealed class POnReceiveInvokeItem : IDisposable
        {
            private OnReceiveModel<POnReceive> _onReceive;
            private string _channel;
            private string _pattern;
            private object _data;
            private Encoding _encoding;
            private bool _disposedValue;

            internal POnReceiveInvokeItem(OnReceiveModel<POnReceive> onReceive, string pattern, string channel, object data, Encoding encoding)
            {
                this._onReceive = onReceive;
                this._channel = channel;
                this._pattern = pattern;
                this._data = data;
                this._encoding = encoding;
            }

            internal void Invoke()
            {
                if (this._onReceive.DataType is ResultDataType.Default)
                {
                    var str = ConvertExtensions.To<string>(this._data, ResultType.String, this._encoding);
                    if (str != null) this._onReceive.OnReceive.Invoke(this._pattern, this._channel, str);
                }
                else if (this._onReceive.DataType is ResultDataType.Bytes)
                {
                    if (this._data is byte[] bytes)
                    {
                        this._onReceive.OnReceive.Invoke(this._pattern, this._channel, bytes);
                    }
                }
            }

            private void Dispose(bool disposing)
            {
                if (!this._disposedValue)
                {
                    if (disposing)
                    {
                        this._disposedValue = true;
                        this._onReceive = null;
                        this._channel = null;
                        this._data = null;
                        this._pattern = null;
                        this._encoding = null;
                    }
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            ~POnReceiveInvokeItem()
            {
                Dispose(disposing: true);
            }
        }
    }
}
