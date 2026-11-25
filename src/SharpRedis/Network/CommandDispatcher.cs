using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Network;

internal sealed class CommandDispatcher : IDisposable
{
    private readonly Thread _thread;
    private readonly ConcurrentQueue<QueueItem> _sendQueue;
    private readonly int _batchCount;
    private readonly Encoding _encoding;
    private readonly SemaphoreSlim _sendSignal;
    private CommandLifetime _submissions;
    private int _disposeStarted;
    private int _waitingForCommands;
#if MODERN_NET
    private BaseConnection? _sendingConnection;
#else
    private BaseConnection _sendingConnection;
#endif

    internal CommandDispatcher(int batchCount, Encoding encoding)
    {
        this._batchCount = batchCount;
        this._encoding = encoding;
        this._sendSignal = new(0, 1);
        this._sendQueue = new();
        this._thread = new Thread(CommandDispatcher.Send)
        {
            IsBackground = true,
            Name = "SharpRedis_CommandDispatcher",
        };
        this._thread.Start(this);
    }

    internal void Enqueue(ICommand command, BaseConnection connection, TaskCompletionSource<RedisResult> resultTask)
    {
        if (!this._submissions.TryRetain())
        {
            this.Reject(resultTask);
            return;
        }

        try
        {
            if (!command.TryRetainForSend())
            {
                _ = resultTask.TrySetResult(new(RespType.Exception, new ObjectDisposedException(nameof(ICommand)), this._encoding));
                return;
            }
            this._sendQueue.Enqueue(new(command, connection, resultTask));
            this.WakeSender();
        }
        finally
        {
            _ = this._submissions.Release();
        }
    }

    private void WakeSender()
    {
        if (Volatile.Read(ref this._waitingForCommands) != 0
            && Interlocked.Exchange(ref this._waitingForCommands, 0) != 0)
        {
            _ = this._sendSignal.Release();
        }
    }

#if MODERN_NET
    private static void Send(object? obj)
#else
    private static void Send(object obj)
#endif
    {
        if (obj is not CommandDispatcher dispatcher) return;

        var buffer = SharedPool.BytesShard.Rent(Config.DEFAULT_BUFFER_SIZE);
        var commands = new (ICommand command, TaskCompletionSource<RedisResult> resultTask)[dispatcher._batchCount];
        // Bounded look-ahead, with all storage owned and reused by this send thread.
        int windowSize = dispatcher._batchCount * 4;
        var window = new QueueItem[windowSize];
        var next = new int[windowSize];
        var heads = new int[windowSize];
        var tails = new int[windowSize];
        var counts = new int[windowSize];
        var groups = new Dictionary<BaseConnection, int>();
        try
        {
            while (Volatile.Read(ref dispatcher._disposeStarted) == 0)
            {
                if (!dispatcher._sendQueue.TryDequeue(out var first))
                {
                    _ = Interlocked.Exchange(ref dispatcher._waitingForCommands, 1);
                    bool hasCommand = dispatcher._sendQueue.TryDequeue(out first);
                    if (!hasCommand && Volatile.Read(ref dispatcher._disposeStarted) == 0)
                    {
                        dispatcher._sendSignal.Wait();
                        continue;
                    }
                    if (Interlocked.Exchange(ref dispatcher._waitingForCommands, 0) == 0)
                        dispatcher._sendSignal.Wait();
                    if (!hasCommand) break;
                }

                int windowCount = 1, groupCount = 1, lastGroup = 0;
                window[0] = first;
                var lastConnection = first.Connection;
                try
                {
                    // A contiguous batch needs no connection groups or linked indices.
                    while (windowCount < dispatcher._batchCount && dispatcher._sendQueue.TryDequeue(out var entry))
                    {
                        window[windowCount++] = entry;
                        if (!ReferenceEquals(entry.Connection, first.Connection))
                        {
                            int split = windowCount - 1;
                            heads[0] = 0;
                            tails[0] = split - 1;
                            counts[0] = split;
                            for (int i = 0; i < split - 1; i++) next[i] = i + 1;
                            next[split - 1] = next[split] = -1;
                            heads[1] = tails[1] = split;
                            counts[1] = 1;
                            groups.Add(first.Connection, 0);
                            groups.Add(entry.Connection, 1);
                            lastConnection = entry.Connection;
                            lastGroup = 1;
                            groupCount = 2;
                            break;
                        }
                    }
                    while (groupCount > 1 && windowCount < windowSize && counts[lastGroup] < dispatcher._batchCount)
                    {
                        if (!dispatcher._sendQueue.TryDequeue(out var item)) break;
                        int index = windowCount++;
                        window[index] = item;
                        next[index] = -1;
                        if (!ReferenceEquals(item.Connection, lastConnection))
                        {
                            if (!groups.TryGetValue(item.Connection, out lastGroup))
                            {
                                lastGroup = groupCount++;
                                groups.Add(item.Connection, lastGroup);
                                heads[lastGroup] = tails[lastGroup] = index;
                                counts[lastGroup] = 1;
                                lastConnection = item.Connection;
                                continue;
                            }
                            lastConnection = item.Connection;
                        }
                        next[tails[lastGroup]] = index;
                        tails[lastGroup] = index;
                        counts[lastGroup]++;
                    }

                    // First-seen group order; FIFO is preserved within each connection.
                    for (int group = 0; group < groupCount; group++)
                    {
                        int head = groupCount == 1 ? 0 : heads[group];
                        int count = groupCount == 1 ? windowCount : counts[group];
                        var connection = window[head].Connection;
                        int commandCount = 0;
                        Interlocked.Exchange(ref dispatcher._sendingConnection, connection);
                        try
                        {
                            for (int i = 0, index = head; i < count; i++, index = groupCount == 1 ? index + 1 : next[index])
                            {
                                var item = window[index];
                                if (Volatile.Read(ref dispatcher._disposeStarted) != 0)
                                    dispatcher.Reject(item.ResultTask);
                                else if (item.Command.CancellationToken.IsCancellationRequested)
                                    _ = item.ResultTask.TrySetResult(new(RespType.Exception,
                                        new OperationCanceledException(item.Command.CancellationToken), dispatcher._encoding));
                                else if (item.Command.TimeoutAt != DateTime.MaxValue && DateTime.UtcNow >= item.Command.TimeoutAt)
                                    _ = item.ResultTask.TrySetResult(new(RespType.Exception, new TimeoutException(), dispatcher._encoding));
                                else
                                    commands[commandCount++] = (item.Command, item.ResultTask);
                            }
                            if (commandCount != 0)
                            {
                                if (Volatile.Read(ref dispatcher._disposeStarted) != 0)
                                {
                                    for (int i = 0; i < commandCount; i++) dispatcher.Reject(commands[i].resultTask);
                                }
                                else
                                {
                                    connection.SendCommand(new(commands, 0, commandCount), ref buffer);
                                    for (int i = 0; i < commandCount; i++)
                                        if (commands[i].resultTask is HeartbeatExecution heartbeat)
                                            _ = heartbeat.Sent.TrySetResult(true);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            connection.Invalidate();
                            for (int i = 0; i < commandCount; i++)
                                _ = commands[i].resultTask.TrySetResult(new(RespType.Exception, ex, dispatcher._encoding));
                        }
                        finally
                        {
                            Volatile.Write(ref dispatcher._sendingConnection, null);
                            Array.Clear(commands, 0, commandCount);
                        }
                    }
                }
                finally
                {
                    for (int i = 0; i < windowCount; i++) window[i].Command.ReleaseAfterSend();
                    Array.Clear(window, 0, windowCount);
                    if (groupCount > 1) groups.Clear();
                }
            }
        }
        finally
        {
            SharedPool.BytesShard.Return(buffer, true);
        }
    }

    private void Reject(TaskCompletionSource<RedisResult> resultTask)
    {
        _ = resultTask.TrySetResult(new(RespType.Exception, new ObjectDisposedException(nameof(CommandDispatcher)), this._encoding));
    }

    public void Dispose()
    {
        if (Thread.CurrentThread == this._thread)
        {
            throw new InvalidOperationException("Cannot dispose the dispatcher from its send thread.");
        }
        if (Interlocked.Exchange(ref this._disposeStarted, 1) != 0) return;

        _ = this._submissions.DisposeOwner();
        SpinWait wait = new();
        while (this._submissions.HasLeases) wait.SpinOnce();

        // Closing the active socket also releases a worker blocked in Socket.Send.
        Volatile.Read(ref this._sendingConnection)?.Dispose();
        this.WakeSender();
        this._thread.Join();
        while (this._sendQueue.TryDequeue(out var pending))
        {
            pending.Command.ReleaseAfterSend();
            this.Reject(pending.ResultTask);
        }
        this._sendSignal.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly struct QueueItem
    {
        private readonly ICommand _command;
        private readonly BaseConnection _connection;
        private readonly TaskCompletionSource<RedisResult> _resultTask;

        internal readonly ICommand Command => this._command;

        internal readonly BaseConnection Connection => this._connection;

        internal readonly TaskCompletionSource<RedisResult> ResultTask => this._resultTask;

        internal QueueItem(ICommand command, BaseConnection connection, TaskCompletionSource<RedisResult> resultTask)
        {
            this._command = command;
            this._connection = connection;
            this._resultTask = resultTask;
        }
    }
}
