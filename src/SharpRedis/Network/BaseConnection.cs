using SharpRedis.Entities;
using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using SharpRedis.Network.Pool;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Network;

internal abstract class BaseConnection : IDisposable
{
    protected private volatile bool _available = true;
    protected private volatile bool _disposedValue;
    private int _activeOperations;
    private int _cleanupReady;
    private int _cleanupStarted;
    private int _disposeStarted;
    private bool _receiveAgain;
#if MODERN_NET
    private object? _receiveState;
#else
    private object _receiveState;
#endif
    private readonly CommandDispatcher _commandDispatcher;
    private readonly AsyncCallback _receiveCallback;
    private readonly RedisSocket _socket;
    protected private SharpConnectionOptions _connectionOptions;
    protected private ConcurrentQueue<WaitResultQueueItem> _waitQueue;
#if MODERN_NET
    private MemoryStream? _dataPacket;
    protected private DataPacketExtensions.PartialAggregate? _partialReply;
#else
    private MemoryStream _dataPacket;
    protected private DataPacketExtensions.PartialAggregate _partialReply;
#endif
    protected private byte[] _receiveBuffer;
#if MODERN_NET
    private TaskCompletionSource<RedisResult>? _pendingCommand;
#else
    private TaskCompletionSource<RedisResult> _pendingCommand;
#endif

    internal string Host => this._connectionOptions.Host;

    internal int Port => this._connectionOptions.Port;

    internal Encoding Encoding => this._connectionOptions.Encoding;

    internal bool Available => this._available && !this._disposedValue && Volatile.Read(ref this._disposeStarted) == 0;

    internal DateTimeOffset LastActiveTime { get; protected private set; }

    internal string ConnectionName { get; private protected set; }

    internal long ClientId { get; set; } = -1;

    internal long RedirectConnectionId { get; set; }

    protected private BaseConnection(SharpConnectionOptions connectionOptions, CommandDispatcher commandDispatcher, int connectTimeout = 0)
    {
        this._connectionOptions = connectionOptions;
        this._commandDispatcher = commandDispatcher;
        this._receiveCallback = this.ReceiveCompleted;
        this._socket = RedisSocket.Open(connectionOptions, connectTimeout);
        if (!string.IsNullOrEmpty(connectionOptions.ConnectName))
        {
            this.ConnectionName = $"SharpRedisId:{Guid.NewGuid():N}_{connectionOptions.ConnectName}";
        }
        else
        {
            this.ConnectionName = $"SharpRedisId:{Guid.NewGuid():N}";
        }
        this._receiveBuffer = new byte[connectionOptions.BuffSize];
        this._waitQueue = new();
    }

    internal abstract BaseConnection Copy(IConnectionPool pool);

    internal void Invalidate()
    {
        this._available = false;
        // Preserve the subscription state until the pool has copied it.
        this._socket.CloseTransport();
    }

    internal void CommandEnqueue(ICommand command, TaskCompletionSource<RedisResult> resultTask)
    {
        this._commandDispatcher.Enqueue(command, this, resultTask);
    }

    internal virtual void SendCommand(ReadOnlySpan<(ICommand command, TaskCompletionSource<RedisResult> resultTask)> commands, ref byte[] buffer)
    {
        if (!this.TryEnterOperation())
        {
            SetError(ref commands, new NotSupportedException("The current connection is unavailable."), this.Encoding);
            return;
        }
        try
        {
            int length = 0;
            if (!this.Available)
            {
                SetError(ref commands, new NotSupportedException("The current connection is unavailable."), this._connectionOptions.Encoding);
                return;
            }

            bool updatesActivity = false;
            foreach (var (command, resultTask) in commands)
            {
                command.WriteToResp(this._connectionOptions.Encoding, ref buffer, ref length, this._connectionOptions.KeyPrefix, this._connectionOptions.KeySuffix);
                this._waitQueue.Enqueue(new(resultTask, command));
                updatesActivity |= (command.Mode & CommandMode.WithoutActiveTime) == 0;
            }

            if (updatesActivity) this.LastActiveTime = DateTimeOffset.UtcNow;
            this.SendBuffer(buffer, length);
        }
        catch (Exception ex)
        {
            this._available = false;
            SetError(ref commands, ex, this._connectionOptions.Encoding);
            return;
        }

        finally
        {
            this.ExitOperation();
        }

        static void SetError(ref readonly ReadOnlySpan<(ICommand command, TaskCompletionSource<RedisResult> resultTask)> commands, Exception ex, Encoding encoding)
        {
            foreach (var (_, resultTask) in commands)
            {
                _ = resultTask.TrySetResult(new RedisResult(RespType.Exception, ex, encoding));
            }
        }
    }

    protected private void SendBuffer(byte[] buffer, int length)
        => this._socket.SendBuffer(buffer, length);

    protected private int EndReceive(IAsyncResult result) => this._socket.FinishRead(result);

    protected private virtual void FailPending(Exception ex)
    {
        this._available = false;
        _ = this._pendingCommand?.TrySetResult(new RedisResult(RespType.Exception, ex, this.Encoding));
        this._pendingCommand = null;
        while (this._waitQueue != null && this._waitQueue.TryDequeue(out var command))
        {
            command.SetException(ex, this.Encoding);
        }
    }

    // The socket buffer is valid until the next StartRead. Only incomplete scalar frames
    // need their bytes copied across callbacks; aggregate progress lives in _partialReply.
    protected private byte[] ReadPacket(IAsyncResult ar, out int end)
    {
        end = this.EndReceive(ar);
        if (end == 0) throw new EndOfStreamException("The remote endpoint closed the connection.");
        if (this._dataPacket is null || this._dataPacket.Length == 0) return this._receiveBuffer;
        this._dataPacket.Write(this._receiveBuffer, 0, end);
        end = (int)this._dataPacket.Length;
        return this._dataPacket.GetBuffer();
    }

    protected private void KeepRemaining(byte[] buffer, int position, int end)
    {
        int remaining = end - position;
        if (this._dataPacket is null && remaining == 0) return;
        // Release oversized scratch storage even if a subsequent reply has already started.
        if (this._dataPacket != null && remaining <= this._receiveBuffer.Length
            && this._dataPacket.Capacity > Math.Max(this._receiveBuffer.Length, 1024 * 1024))
            this._dataPacket = null;
        if (remaining == 0)
        {
            this._dataPacket?.SetLength(0);
            return;
        }

        this._dataPacket ??= new MemoryStream(remaining);
        if (buffer == this._dataPacket.GetBuffer() && position == 0) return;
        this._dataPacket.SetLength(0);
        this._dataPacket.Write(buffer, position, remaining);
    }

    protected private virtual void Receive(IAsyncResult ar)
    {
        var buffer = this.ReadPacket(ar, out int end);
        int position = 0;
        WaitResultQueueItem command = ar.AsyncState is WaitResultQueueItem pending ? pending : default;
        bool hasCommand = command.ResultHandler != null;

        while (position < end && !this._disposedValue)
        {
            if (!hasCommand)
            {
                hasCommand = this._waitQueue.TryDequeue(out command);
                if (hasCommand) this._pendingCommand = command.ResultHandler;
                else if (this._partialReply is null && buffer[position] != Config.PUSHES)
                    throw new FormatException("Received a Redis reply without a pending command.");
            }
            bool bytes = hasCommand && command.RetutnBytes(command.CurrentResultIndex);
            if (!DataPacketExtensions.GetRawValue(buffer, end, ref position, this.Encoding, bytes, ref this._partialReply, out var result))
                break;
            if (result.Type == RespType.Push) continue;
            if (command.SetResult(result, this.Encoding)) hasCommand = false;
        }

        this.KeepRemaining(buffer, position, end);
        this.ContinueReceive(hasCommand ? command : null);
    }

#if MODERN_NET
    protected private void ContinueReceive(object? state)
#else
    protected private void ContinueReceive(object state)
#endif
    {
        this._receiveState = state;
        this._receiveAgain = true;
        this._pendingCommand = state is WaitResultQueueItem command ? command.ResultHandler : null;
    }

#if MODERN_NET
    protected private void BeginReceive(object? state)
#else
    protected private void BeginReceive(object state)
#endif
        => this.RunReceive(state, null);

    private void ReceiveCompleted(IAsyncResult ar)
    {
        // Synchronous completion belongs to the initiating loop, never the callback.
        if (!ar.CompletedSynchronously) this.RunReceive(null, ar);
    }

#if MODERN_NET
    private void RunReceive(object? state, IAsyncResult? completed)
#else
    private void RunReceive(object state, IAsyncResult completed)
#endif
    {
        if (!this.TryEnterOperation())
        {
            if (completed != null)
            {
                try { _ = this.EndReceive(completed); }
                catch (ObjectDisposedException) { }
                catch (SocketException) { }
                catch (IOException) { }
            }
            return;
        }
        try
        {
            while (true)
            {
                if (completed != null)
                {
                    this._receiveAgain = false;
                    this.Receive(completed);
                    if (!this._receiveAgain) return;
                    state = this._receiveState;
                }
                if (!this.Available) return;
                completed = this._socket.StartRead(this._receiveBuffer, this._receiveCallback, state);
                // An asynchronous callback may already be running; do not touch shared receive state here.
                if (!completed.CompletedSynchronously) return;
            }
        }
        catch (Exception ex)
        {
            this.FailPending(ex);
        }
        finally
        {
            this.ExitOperation();
        }
    }

    internal virtual void RetryTracking(ClientCachingBase clientCaching)
    {
        if (clientCaching.RedirectConnectionId == this.RedirectConnectionId) return;
        try
        {
            this.ResetClientCaching(clientCaching, this._connectionOptions.CommandTimeout);
        }
        catch (Exception ex)
        {
            SharpConsole.WriteError($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Exception occurred when re-enabling client-side caching, Msg: [{ex.Message}]");
        }
    }

    #region Dispose
    protected private virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.FailPending(new ObjectDisposedException(nameof(BaseConnection), "The current connection is unavailable."));
            this._dataPacket?.Dispose();
            this._dataPacket = null;
            this._partialReply = null;
        }
    }

    protected private bool TryEnterOperation()
    {
        if (Volatile.Read(ref this._disposeStarted) != 0) return false;
        Interlocked.Increment(ref this._activeOperations);
        if (Volatile.Read(ref this._disposeStarted) == 0) return true;
        this.ExitOperation();
        return false;
    }

    protected private void ExitOperation()
    {
        if (Interlocked.Decrement(ref this._activeOperations) == 0)
            this.TryCleanup();
    }

    private void TryCleanup()
    {
        if (Volatile.Read(ref this._cleanupReady) == 0
            || Volatile.Read(ref this._activeOperations) != 0
            || Interlocked.CompareExchange(ref this._cleanupStarted, 1, 0) != 0) return;
        this.Dispose(disposing: true);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref this._disposeStarted, 1) != 0) return;
        this._available = false;
        try
        {
            this._socket.CloseTransport();
        }
        finally
        {
            Volatile.Write(ref this._cleanupReady, 1);
            this.TryCleanup();
            GC.SuppressFinalize(this);
        }
    }
    #endregion
}
