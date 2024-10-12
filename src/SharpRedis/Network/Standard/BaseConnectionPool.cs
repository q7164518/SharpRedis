#pragma warning disable IDE0059
#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable IDE0074
#pragma warning disable IDE0063
#endif
#if NET8_0_OR_GREATER
#pragma warning disable IDE0305
#pragma warning disable IDE0300
#endif
#if !LOW_NET
using System.Threading.Tasks;
#endif
#if !NET30
using System.Linq;
#endif
using SharpRedis.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SharpRedis.Commands;

namespace SharpRedis.Network.Standard
{
    internal abstract class BaseConnectionPool : IConnectionPool
    {
        private protected bool _disposedValue;
        private protected Encoding _encoding;
        private protected volatile int _masterConnectionCount = 0;
        private protected ConcurrentDictionary<string, IConnection> _allConnections;
        private protected ConcurrentStack<DefaultConnection> _masterConnections;
        private protected ConnectionOptions _masterConnectionOptions;
        private protected Thread _idleThread;
        private protected AutoResetEvent _idleAutoResetEvent;
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private protected ClientSideCachingStandard? _clientSideCaching;
        private protected byte[]? _prefix;
        private ConcurrentStack<SubConnection>? _subConnections;
#else
        private protected ClientSideCachingStandard _clientSideCaching;
        private protected byte[] _prefix;
        private ConcurrentStack<SubConnection> _subConnections;
#endif
        private protected ConcurrentQueue<SyncGetConnectionManual<DefaultConnection>> _syncAwaitMasterConnection;
#if !LOW_NET
#if NET5_0_OR_GREATER
        private protected System.Threading.Channels.Channel<TaskCompletionSource<DefaultConnection>> _asyncAwaitMasterConnection;
#else
        private protected ConcurrentQueue<TaskCompletionSource<DefaultConnection>> _asyncAwaitMasterConnection;
#endif
#endif

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public byte[]? KeyPrefix => this._prefix;
#else
        public byte[] KeyPrefix => this._prefix;
#endif

        public ConnectionOptions MasterConnectionOptions => this._masterConnectionOptions;

        public Encoding Encoding => this._encoding;

        #region Constructor
        private protected BaseConnectionPool(ConnectionOptions masterConnectionOptions)
        {
            this._masterConnectionOptions = masterConnectionOptions;
            this._encoding = Encoding.GetEncoding(this._masterConnectionOptions.Encoding);
            this._masterConnections = new ConcurrentStack<DefaultConnection>();
            this._allConnections = new ConcurrentDictionary<string, IConnection>();
            if (!string.IsNullOrEmpty(this._masterConnectionOptions.Prefix))
            {
                this._prefix = this._encoding.GetBytes(this._masterConnectionOptions.Prefix.Trim());
            }

            for (int i = 0; i < this._masterConnectionOptions.MinPoolSize; i++)
            {
                var connection = this.CreateMasterConnection();
                if (connection != null) this._masterConnections.Push(connection);
            }
            if (this._masterConnections.IsEmpty)
            {
                this.Dispose();
                throw new RedisConnectionException("Failed to connect to Redis, please check the network and Redis configuration.");
            }
#if !LOW_NET
#if NET5_0_OR_GREATER
            this._asyncAwaitMasterConnection = System.Threading.Channels.Channel.CreateUnbounded<TaskCompletionSource<DefaultConnection>>();
#else
            this._asyncAwaitMasterConnection = new ConcurrentQueue<TaskCompletionSource<DefaultConnection>>();
#endif
#endif
            this._syncAwaitMasterConnection = new ConcurrentQueue<SyncGetConnectionManual<DefaultConnection>>();
            this._idleAutoResetEvent = new AutoResetEvent(false);
            this._idleThread = new Thread(this.IdleWithHeartbeat)
            {
                IsBackground = true
            };
        }
        #endregion

        public void SetSetClientSideCaching(ClientSideCachingStandard clientSideCaching)
        {
            this._clientSideCaching = clientSideCaching;
        }

        #region Sync methods
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public DefaultConnection? CreateMasterConnection()
#else
        public DefaultConnection CreateMasterConnection()
#endif
        {
            if (this._disposedValue) throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
            if (Interlocked.Increment(ref this._masterConnectionCount) > this._masterConnectionOptions.MaxPoolSize)
            {
                _ = Interlocked.Decrement(ref this._masterConnectionCount);
                return null;
            }
            var connection = new DefaultConnection(this._encoding, this._masterConnectionOptions, this._prefix);
            if (!connection.Connect())
            {
                goto RetunNull;
            }

            //tracking
            if (this._clientSideCaching?.RedirectConnectionId > 0)
            {
                var trackingCommand = ClientSideCachingExtensions.GetTrackingCommand(this._clientSideCaching);
                try
                {
                    if (ConvertExtensions.To<string>(connection.ExecuteCommand(trackingCommand, CancellationToken.None), ResultType.String, this._encoding) != "OK")
                    {
                        _ = Interlocked.Decrement(ref this._masterConnectionCount);
                        connection.Dispose();
                        connection = null;
                        return null;
                    }
                }
                catch
                {
                    _ = Interlocked.Decrement(ref this._masterConnectionCount);
                    if (connection != null)
                    {
                        connection.Dispose();
                        connection = null;
                    }
                    return null;
                }
                connection.Tracking = true;
                connection.RedirectConnectionId = this._clientSideCaching.RedirectConnectionId;
            }

            if (this._allConnections.TryAdd(connection.ConnectionName, connection))
            {
                return connection;
            }

            RetunNull:
            _ = Interlocked.Decrement(ref this._masterConnectionCount);
            connection.Dispose();
            connection = null;
            return connection;
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public SubConnection? CreateSubConnection()
#else
        public SubConnection CreateSubConnection()
#endif
        {
            if (this._disposedValue) throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
            var connection = new SubConnection(this._encoding, this._masterConnectionOptions);
            if (!connection.Connect())
            {
                connection?.Dispose();
                connection = null;
                return connection;
            }
            if (this._allConnections.TryAdd(connection.ConnectionName, connection))
            {
                return connection;
            }
            connection.Dispose();
            connection = null;
            return connection;
        }

        public DefaultConnection GetMasterConnection(CancellationToken cancellationToken)
        {
            if (this._disposedValue) throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
            var timeout = DateTime.UtcNow.AddMilliseconds(this._masterConnectionOptions.CommandTimeout);
            var timespan = timeout - DateTime.UtcNow;
            var connection = this.TryGetMasterConnection(timespan, cancellationToken);
            if (connection != null) return connection;

            using (var syncWait = new SyncGetConnectionManual<DefaultConnection>())
            {
                this._syncAwaitMasterConnection.Enqueue(syncWait);
#if NET8_0_OR_GREATER
                int index = WaitHandle.WaitAny([syncWait.WaitHandle.WaitHandle, cancellationToken.WaitHandle], timespan);
#else
#if !LOW_NET
                int index = WaitHandle.WaitAny(new WaitHandle[] { syncWait.WaitHandle.WaitHandle, cancellationToken.WaitHandle }, timespan);
#else
                int index = WaitHandle.WaitAny(new WaitHandle[] { syncWait.WaitHandle, cancellationToken.WaitHandle }, timespan);
#endif
#endif
                if (index != 0)
                {
                    if (!syncWait.SetTimeout())
                    {
                        if (syncWait.Connection != null) return syncWait.Connection;
                    }
                    throw new TimeoutException("Get connection timeout");
                }
                if (syncWait.Connection is null) throw new TimeoutException("Get connection timeout");
                return syncWait.Connection;
            }
        }

        public abstract DefaultConnection GetSlaveConnection(CancellationToken cancellationToken);

        public SubConnection GetSubConnection()
        {
            if (this._disposedValue) throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
            lock (this)
            {
                if (this._subConnections is null) this._subConnections = new ConcurrentStack<SubConnection>();
            }
            if (this._subConnections.TryPeek(out var connection) && connection.SubCount < this._masterConnectionOptions.SubConcurrency)
            {
                return connection;
            }
            //create
            connection = this.CreateSubConnection();
            if (connection is null) throw new RedisConnectionException("Failed to create a subscription connection");
            this._subConnections.Push(connection);
            return connection;
        }

        public IConnection[] GetAllMasterConnections()
        {
            if (this._disposedValue || this._allConnections is null || this._allConnections.IsEmpty)
                throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
#if !NET30
            return this._allConnections.Values.Where(f => f.Type == ConnectionType.Master && f.Connected).ToArray();
#else
            var result = new List<IConnection>();
            var connections = this._allConnections.Values;
            foreach (var connection in connections)
            {
                if (connection.Type == ConnectionType.Master && connection.Connected) result.Add(connection);
            }
            return result.ToArray();
#endif
        }

        public IConnection[] GetAllConnections()
        {
            if (this._disposedValue || this._allConnections.IsEmpty)
                throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
#if !NET30
            return this._allConnections.Values.Where(f => f.Connected).ToArray();
#else
            var result = new List<IConnection>();
            var connections = this._allConnections.Values;
            foreach (var connection in connections)
            {
                if (connection.Connected) result.Add(connection);
            }
            return result.ToArray();
#endif
        }

        public SubConnection[] GetAllSubConnections()
        {
            if (this._disposedValue || this._subConnections is null || this._subConnections.IsEmpty)
                throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
            return this._subConnections.ToArray();
        }

        public void ReturnMasterConnection(DefaultConnection connection)
        {
            if (this._disposedValue) throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
            if (connection is null) return;
            if (!connection.Connected)
            {
                connection.Dispose();
                if (this._allConnections.TryRemove(connection.ConnectionName, out _))
                {
                    _ = Interlocked.Decrement(ref this._masterConnectionCount);
                }
                return;
            }

            if (this._syncAwaitMasterConnection.TryDequeue(out var syncAwait))
            {
                if (syncAwait.SetConnection(connection))
                {
                    return;
                }
                else
                {
                    goto ReturnConnection;
                }
            }

            #region Async await
#if !LOW_NET
#if NET5_0_OR_GREATER
            if (this._asyncAwaitMasterConnection.Reader.TryRead(out var awaitTcs) && awaitTcs.Task.Status == TaskStatus.WaitingForActivation)
            {
                lock (awaitTcs)
                {
                    try
                    {
                        awaitTcs.SetResult(connection);
                    }
                    catch
                    {
                        goto ReturnConnection;
                    }
                }
                return;
            }
#else
            if (this._asyncAwaitMasterConnection.TryDequeue(out var awaitTcs) && awaitTcs.Task.Status == TaskStatus.WaitingForActivation)
            {
                lock (awaitTcs)
                {
                    try
                    {
                        awaitTcs.SetResult(connection);
                    }
                    catch
                    {
                        goto ReturnConnection;
                    }
                }
                return;
            }
#endif
#endif
            #endregion

            ReturnConnection:
            this._masterConnections?.Push(connection);
        }

        public abstract void ReturnSlaveConnection(DefaultConnection connection);
        #endregion

        #region Async Methods
#if !LOW_NET
        public async Task<DefaultConnection> GetMasterConnectionAsync(CancellationToken cancellationToken)
        {
            if (this._disposedValue) throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
            var timeout = DateTime.UtcNow.AddMilliseconds(this._masterConnectionOptions.CommandTimeout);
            var timespan = timeout - DateTime.UtcNow;
            var connection = this.TryGetMasterConnection(timespan, cancellationToken);
            if (connection != null) return connection;

            var awaitTcs = new TaskCompletionSource<DefaultConnection>();
#if NET5_0_OR_GREATER
            await this._asyncAwaitMasterConnection.Writer.WriteAsync(awaitTcs, cancellationToken).ConfigureAwait(false);
#else
            this._asyncAwaitMasterConnection.Enqueue(awaitTcs);
#endif
            try
            {
                if (cancellationToken == default)
                {
#if NET6_0_OR_GREATER
                    return await awaitTcs.Task.WaitAsync(timespan, CancellationToken.None).ConfigureAwait(false);
#else
#if NET40
                    var task = await TaskEx.WhenAny(awaitTcs.Task, TaskEx.Delay(timespan, CancellationToken.None)).ConfigureAwait(false);
#else
                    var task = await Task.WhenAny(awaitTcs.Task, Task.Delay(timespan, CancellationToken.None)).ConfigureAwait(false);
#endif
                    if (object.ReferenceEquals(task, awaitTcs.Task))
                    {
                        return await awaitTcs.Task.ConfigureAwait(false);
                    }
                    else
                    {
                        throw new OperationCanceledException();
                    }
#endif
                }
                else
                {
#if NET6_0_OR_GREATER
                    return await awaitTcs.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
#else
                    return await awaitTcs.Task.ContinueWith(res => res.Result, cancellationToken).ConfigureAwait(false);
#endif
                }
            }
            catch (OperationCanceledException)
            {
                if (awaitTcs.Task.Status == TaskStatus.RanToCompletion)
                {
                    return await awaitTcs.Task.ConfigureAwait(false);
                }
                else
                {
#if NET5_0_OR_GREATER
                    lock (awaitTcs) awaitTcs.SetCanceled(CancellationToken.None);
#else
                    lock (awaitTcs) awaitTcs.SetCanceled();
#endif
                    throw new TimeoutException("Get connection timeout");
                }
            }
            catch
            {
                throw;
            }
        }

        public abstract Task<DefaultConnection> GetSlaveConnectionAsync(CancellationToken cancellationToken);
#endif
        #endregion

        #region Idle methods
        public void IdleWithHeartbeat()
        {
            int delayMilliseconds = 1000 * 30;
            if (this._masterConnectionOptions.IdleTimeout <= delayMilliseconds)
            {
                delayMilliseconds = this._masterConnectionOptions.IdleTimeout;
            }
            if (delayMilliseconds < 1000 * 30) delayMilliseconds = 1000 * 30;
            if (delayMilliseconds > 1000 * 120) delayMilliseconds = 1000 * 120;

            while (true)
            {
                if (this._idleAutoResetEvent.WaitOne(TimeSpan.FromSeconds(60))) return;
                if (this._disposedValue) return;
                try
                {
                    this.IdleMasterConnections(delayMilliseconds);
                }
                catch (Exception ex)
                {
                    SharpConsole.WriteError($"Idle master exception, message: {ex.Message}");
                }

                try
                {
                    this.IdleSlaveConnections(delayMilliseconds);
                }
                catch (Exception ex)
                {
                    SharpConsole.WriteError($"Idle slave exception, message: {ex.Message}");
                }

                try
                {
                    this.IdleSubConnections();
                }
                catch (Exception ex)
                {
                    SharpConsole.WriteError($"Idle PubSub exception, message: {ex.Message}");
                }
            }
        }

        public void IdleMasterConnections(int delayMilliseconds)
        {
            if (this._masterConnections is null || this._allConnections.IsEmpty) return;

            var masterConnections = new List<DefaultConnection>();
            if (this._masterConnectionCount <= this._masterConnectionOptions.MinPoolSize)
            {
                while (true)
                {
                    if (this._masterConnections.TryPop(out var connection))
                    {
                        if (!connection.Connected)
                        {
                            if (this._allConnections.TryRemove(connection.ConnectionName, out _))
                            {
                                _ = Interlocked.Decrement(ref this._masterConnectionCount);
                            }
                            connection.Dispose();
                            connection = null;
                            continue;
                        }
                        connection.ResetBuffer();
                        if (ConnectionExtensions.Ping(connection))
                        {
                            masterConnections.Add(connection);
                            continue;
                        }

                        if (this._allConnections.TryRemove(connection.ConnectionName, out _))
                        {
                            _ = Interlocked.Decrement(ref this._masterConnectionCount);
                        }
                        connection.Dispose();
                        connection = null;
                    }
                    break;
                }

                if (masterConnections.Count > 0)
                {
                    foreach (var item in masterConnections)
                    {
                        this._masterConnections.Push(item);
                    }
                }
                masterConnections.Clear();
                RetryConnection();
                return;
            }

            while (true)
            {
                if (!this._masterConnections.TryPop(out var connection)) break;

                if (!connection.Connected)
                {
                    if (this._allConnections.TryRemove(connection.ConnectionName, out _))
                    {
                        _ = Interlocked.Decrement(ref this._masterConnectionCount);
                    }
                    connection.Dispose();
                    connection = null;
                    continue;
                }

                if (this._masterConnectionCount > this._masterConnectionOptions.MinPoolSize
                    && (DateTime.UtcNow - connection.LastActiveTime).TotalMilliseconds >= delayMilliseconds) // discard
                {
                    if (this._allConnections.TryRemove(connection.ConnectionName, out _))
                    {
                        _ = Interlocked.Decrement(ref this._masterConnectionCount);
                    }
                    connection.Dispose();
                    connection = null;
                    continue;
                }
                connection.ResetBuffer();
                if (ConnectionExtensions.Ping(connection))
                {
                    masterConnections.Add(connection);
                }
                else
                {
                    if (this._allConnections.TryRemove(connection.ConnectionName, out _))
                    {
                        _ = Interlocked.Decrement(ref this._masterConnectionCount);
                    }
                    connection.Dispose();
                    connection = null;
                    continue;
                }
            }

            if (masterConnections.Count > 0)
            {
                foreach (var item in masterConnections)
                {
                    this._masterConnections.Push(item);
                }
            }
            masterConnections.Clear();
            RetryConnection();

            void RetryConnection()
            {
                if (this._masterConnectionCount <= 0)
                {
                    var connection = this.CreateMasterConnection();
                    if (connection != null) this._masterConnections.Push(connection);
                    if (this._masterConnectionCount <= 0)
                    {
                        SharpConsole.WriteWarning("Unable to maintain connection with Redis, retry will try again after one minute");
                    }
                }
            }
        }

        public virtual void IdleSlaveConnections(int delayMilliseconds) { }

        public void IdleSubConnections()
        {
            if (this._subConnections is null || this._subConnections.IsEmpty) return;

            var subConnections = new List<SubConnection>();
            while (true)
            {
                if (!this._subConnections.TryPop(out var connection)) break;
                if (connection.Connected && ConnectionExtensions.Ping(connection))
                {
                    subConnections.Add(connection);
                    continue;
                }
                else
                {
                    //create
                    var retryConnection = this.CreateSubConnection();
                    if (retryConnection is null)
                    {
                        SharpConsole.WriteError("Failed to recreate the subscription connection. Try again in one minute");
                        subConnections.Add(connection);
                        continue;
                    }

                    foreach (var item in connection.OnReceives)
                    {
                        if (item.Key.Mode == ChannelModeEnum.Default)
                        {
                            try
                            {
                                retryConnection.Subscribe(PubSubCommands.Subscribe().WriteValue(item.Key.Channel), new Models.ChannelMode[] { item.Key }, item.Value, CancellationToken.None);
                                if (item.Key.Channel == ClientSideCachingExtensions._invalidate_channel_name && this._clientSideCaching != null)
                                {
                                    this._clientSideCaching.WholeClear();
                                    this._clientSideCaching.RedirectConnectionId = retryConnection.ConnectionId;
                                }
                            }
                            catch (Exception ex)
                            {
                                SharpConsole.WriteError($"Sub channel {item.Key.Channel} error, Msg: {ex.Message}");
                                retryConnection.Dispose();
                                retryConnection = null;
                                break;
                            }
                        }
                        else if (item.Key.Mode == ChannelModeEnum.Shard)
                        {
                            try
                            {
                                retryConnection.Subscribe(PubSubCommands.SSubscribe().WriteValue(item.Key.Channel), new Models.ChannelMode[] { item.Key }, item.Value, CancellationToken.None);
                            }
                            catch (Exception ex)
                            {
                                SharpConsole.WriteError($"Sub shard channel {item.Key.Channel} error, Msg: {ex.Message}");
                                retryConnection.Dispose();
                                retryConnection = null;
                                break;
                            }
                        }
                    }

                    if (retryConnection != null)
                    {
                        foreach (var item in connection.POnReceives)
                        {
                            if (item.Key.Mode == ChannelModeEnum.Default)
                            {
                                try
                                {
                                    retryConnection.PSubscribe(PubSubCommands.PSubscribe().WriteValue(item.Key.Channel), new Models.ChannelMode[] { item.Key }, item.Value, CancellationToken.None);
                                }
                                catch (Exception ex)
                                {
                                    SharpConsole.WriteError($"Sub pattern channel {item.Key.Channel} error, Msg: {ex.Message}");
                                    retryConnection.Dispose();
                                    retryConnection = null;
                                    break;
                                }
                            }
                        }
                    }

                    if (retryConnection != null)
                    {
                        subConnections.Add(retryConnection);
                        _ = this._allConnections.TryRemove(connection.ConnectionName, out _);
                        connection.Dispose();
                        connection = null;
                    }
                    else
                    {
                        subConnections.Add(connection);
                    }
                }
            }

            if (subConnections.Count > 0)
            {
                foreach (var item in subConnections)
                {
                    this._subConnections.Push(item);
                }
            }
            subConnections.Clear();
        }
        #endregion

        #region Private methods
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private DefaultConnection? TryGetMasterConnection(TimeSpan timeout, CancellationToken cancellationToken)
#else
        private DefaultConnection TryGetMasterConnection(TimeSpan timeout, CancellationToken cancellationToken)
#endif
        {
            if (this._masterConnections.TryPop(out var connection))
            {
                if (connection.Connected) return connection;
                connection.Dispose();
                if (this._allConnections.TryRemove(connection.ConnectionName, out _))
                {
                    _ = Interlocked.Decrement(ref this._masterConnectionCount);
                }
            }
            //create
            if (this._masterConnectionCount < this._masterConnectionOptions.MaxPoolSize)
            {
                connection = this.CreateMasterConnection();
                if (connection != null) return connection;
            }

            if (cancellationToken != default)
            {
                if (cancellationToken.IsCancellationRequested) throw new TimeoutException("Get connection timeout");
            }
            else
            {
                if (timeout.TotalMilliseconds <= 0) throw new TimeoutException("Get connection timeout");
            }
            return null;
        }
        #endregion

        #region Dispose
        private protected virtual void Dispose(bool disposing)
        {
            if (this._disposedValue) return;
            this._disposedValue = true;
            if (disposing)
            {
                this._idleAutoResetEvent.Set();
                this._masterConnections?.Clear();
                this._subConnections?.Clear();
#if LOW_NET || NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                this._syncAwaitMasterConnection.Clear();
#endif
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#if !NET5_0_OR_GREATER
                this._asyncAwaitMasterConnection.Clear();
#else
                this._asyncAwaitMasterConnection.Writer.Complete();
#endif
#endif
                foreach (var connection in this._allConnections)
                {
                    connection.Value.Dispose();
                }
                this._allConnections.Clear();
#if !LOW_NET
                this._idleAutoResetEvent.Dispose();
#else
                (this._idleAutoResetEvent as IDisposable).Dispose();
#endif
            }
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
            this._masterConnectionOptions = null;
            this._masterConnections = null;
            this._subConnections = null;
            this._allConnections = null;
            this._encoding = null;
            this._syncAwaitMasterConnection = null;
            this._clientSideCaching = null;
            this._idleThread = null;
            this._idleAutoResetEvent = null;
            this._prefix = null;
#if !LOW_NET
            this._asyncAwaitMasterConnection = null;
#endif
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~BaseConnectionPool()
        {
            this.Dispose(disposing: true);
        }
        #endregion

        private protected class SyncGetConnectionManual<TConnection> : IDisposable where TConnection : class, IConnection
        {
            private volatile bool _disposedValue;
            private volatile bool _timeout;
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            private TConnection? _connection;

            internal TConnection? Connection => this._connection;
#else
            private TConnection _connection;

            internal TConnection Connection => this._connection;
#endif

#if !LOW_NET
            private ManualResetEventSlim _waitHandle = new ManualResetEventSlim(false);

            internal ManualResetEventSlim WaitHandle => this._waitHandle;
#else
            private ManualResetEvent _waitHandle = new ManualResetEvent(false);

            internal ManualResetEvent WaitHandle => this._waitHandle;
#endif

            internal bool SetConnection(TConnection connection)
            {
                lock (this)
                {
                    if (this._disposedValue) return false;
                    if (this._timeout) return false;
                    this._connection = connection;
                    this._waitHandle.Set();
                    return true;
                }
            }

            internal bool SetTimeout()
            {
                lock (this)
                {
                    if (this._disposedValue) return false;
                    if (this._connection != null) return false;
                    this._timeout = true;
                    return true;
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                lock (this)
                {
                    if (!this._disposedValue)
                    {
                        this._disposedValue = true;
                        if (disposing)
                        {
#if !LOW_NET
                            this._waitHandle?.Dispose();
#else
                            (this._waitHandle as IDisposable)?.Dispose();
#endif
                        }
                    }
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
                    this._waitHandle = null;
                    this._connection = null;
                }
            }

            public void Dispose()
            {
                this.Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            ~SyncGetConnectionManual()
            {
                this.Dispose(disposing: true);
            }
        }
    }
}
