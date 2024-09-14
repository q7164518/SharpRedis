#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable IDE0063
#endif
#if !LOW_NET
using System.Threading.Tasks;
#endif
using SharpRedis.Network.Standard;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SharpRedis.Network.Pool
{
    internal sealed class MasterSlavePool : BaseConnectionPool
    {
        private readonly int _slaveCount;
        private Random _slaveRandom;
        private ConnectionOptions[] _slaveConnectionOptions;
        private ConcurrentStack<DefaultConnection>[] _slaveConnections;
        private int[] _slaveConnectionCount;
        private ConcurrentQueue<SyncGetConnectionManual<DefaultConnection>> _syncAwaitSlaveConnection;
#if !LOW_NET
#if NET5_0_OR_GREATER
        private System.Threading.Channels.Channel<TaskCompletionSource<DefaultConnection>> _asyncAwaitSlaveConnection;
#else
        private ConcurrentQueue<TaskCompletionSource<DefaultConnection>> _asyncAwaitSlaveConnection;
#endif
#endif

        internal MasterSlavePool(ConnectionOptions masterConnectionOptions, ConnectionOptions[] slaveConnectionOptions)
            : base(masterConnectionOptions)
        {
            this._slaveConnectionOptions = slaveConnectionOptions;
            this._slaveCount = this._slaveConnectionOptions.Length;
            this._slaveConnections = new ConcurrentStack<DefaultConnection>[this._slaveCount];
            this._slaveConnectionCount = new int[this._slaveCount];

            bool isEmpty = true;
            for (int i = 0; i < this._slaveCount; i++)
            {
                if (this._slaveConnections[i] is null) this._slaveConnections[i] = new ConcurrentStack<DefaultConnection>();
                for (int c = 0; c < this._slaveConnectionOptions[i].MinPoolSize; c++)
                {
                    var connection = this.CreateSlaveConnection(i);
                    if (connection != null)
                    {
                        this._slaveConnections[i].Push(connection);
                        isEmpty = false;
                    }
                }
            }

            if (isEmpty)
            {
                this.Dispose();
                throw new RedisConnectionException("Failed to connect to Slave Redis, please check the network and Redis configuration.");
            }

#if !LOW_NET
#if NET5_0_OR_GREATER
            this._asyncAwaitSlaveConnection = System.Threading.Channels.Channel.CreateUnbounded<TaskCompletionSource<DefaultConnection>>();
#else
            this._asyncAwaitSlaveConnection = new ConcurrentQueue<TaskCompletionSource<DefaultConnection>>();
#endif
#endif
            this._syncAwaitSlaveConnection = new ConcurrentQueue<SyncGetConnectionManual<DefaultConnection>>();

            this._slaveRandom = new Random();
            base._idleThread.Start();
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private DefaultConnection? CreateSlaveConnection(int index)
#else
        private DefaultConnection CreateSlaveConnection(int index)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
            if (Interlocked.Increment(ref this._slaveConnectionCount[index]) > this._slaveConnectionOptions[index].MaxPoolSize)
            {
                _ = Interlocked.Decrement(ref this._slaveConnectionCount[index]);
                return null;
            }

            var connection = new DefaultConnection(base._encoding, this._slaveConnectionOptions[index], base._prefix, index);
            if (!connection.Connect())
            {
                goto RetunNull;
            }

            if (base._allConnections.TryAdd(connection.ConnectionName, connection))
            {
                return connection;
            }

            RetunNull:
            _ = Interlocked.Decrement(ref this._slaveConnectionCount[index]);
            connection.Dispose();
            connection = null;
            return connection;
        }

        public sealed override void ReturnSlaveConnection(DefaultConnection connection)
        {
            if (base._disposedValue) throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
            if (connection is null) return;
            if (!connection.Connected)
            {
                connection.Dispose();
                if (this._allConnections.TryRemove(connection.ConnectionName, out _))
                {
                    _ = Interlocked.Decrement(ref this._slaveConnectionCount[connection.SlaveIndex]);
                }
                return;
            }

            if (this._syncAwaitSlaveConnection.TryDequeue(out var syncAwait))
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
            if (this._asyncAwaitSlaveConnection.Reader.TryRead(out var awaitTcs) && awaitTcs.Task.Status == TaskStatus.WaitingForActivation)
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
            if (this._asyncAwaitSlaveConnection.TryDequeue(out var awaitTcs) && awaitTcs.Task.Status == TaskStatus.WaitingForActivation)
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
            this._slaveConnections?[connection.SlaveIndex].Push(connection);
        }

        public sealed override DefaultConnection GetSlaveConnection(CancellationToken cancellationToken)
        {
            if (base._disposedValue) throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
            var slaveIndex = this._slaveRandom.Next(0, this._slaveCount);
            var timeout = DateTime.UtcNow.AddMilliseconds(this._slaveConnectionOptions[slaveIndex].CommandTimeout);
            var timespan = timeout - DateTime.UtcNow;
            var connection = this.TryGetSlaveConnection(slaveIndex, timespan, cancellationToken);
            if (connection != null) return connection;

            using (var syncWait = new SyncGetConnectionManual<DefaultConnection>())
            {
                this._syncAwaitSlaveConnection.Enqueue(syncWait);
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
                    throw new TimeoutException("Get slave connection timeout");
                }
                if (syncWait.Connection is null) throw new TimeoutException("Get slave connection timeout");
                return syncWait.Connection;
            }
        }

#if !LOW_NET
        async public sealed override Task<DefaultConnection> GetSlaveConnectionAsync(CancellationToken cancellationToken)
        {
            if (this._disposedValue) throw new ObjectDisposedException("Redis", "The current Redis connection instance has been released");
            var slaveIndex = this._slaveRandom.Next(0, this._slaveCount);
            var timeout = DateTime.UtcNow.AddMilliseconds(this._slaveConnectionOptions[slaveIndex].CommandTimeout);
            var timespan = timeout - DateTime.UtcNow;
            var connection = this.TryGetSlaveConnection(slaveIndex, timespan, cancellationToken);
            if (connection != null) return connection;

            var awaitTcs = new TaskCompletionSource<DefaultConnection>();
#if NET5_0_OR_GREATER
            await this._asyncAwaitSlaveConnection.Writer.WriteAsync(awaitTcs, cancellationToken).ConfigureAwait(false);
#else
            this._asyncAwaitSlaveConnection.Enqueue(awaitTcs);
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
#endif

        public sealed override void IdleSlaveConnections(int delayMilliseconds)
        {
        }

        #region Private methods
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private DefaultConnection? TryGetSlaveConnection(int index, TimeSpan timeout, CancellationToken cancellationToken)
#else
        private DefaultConnection TryGetSlaveConnection(int index, TimeSpan timeout, CancellationToken cancellationToken)
#endif
        {
            if (this._slaveConnections[index].TryPop(out var connection))
            {
                if (connection.Connected) return connection;
                connection.Dispose();
                if (base._allConnections.TryRemove(connection.ConnectionName, out _))
                {
                    _ = Interlocked.Decrement(ref this._slaveConnectionCount[index]);
                }
            }

            //create
            if (this._slaveConnectionCount[index] < this._slaveConnectionOptions[index].MaxPoolSize)
            {
                connection = this.CreateSlaveConnection(index);
                if (connection != null) return connection;
            }

            if (cancellationToken != default)
            {
                if (cancellationToken.IsCancellationRequested) throw new TimeoutException("Get slave connection timeout");
            }
            else
            {
                if (timeout.TotalMilliseconds <= 0) throw new TimeoutException("Get slave connection timeout");
            }
            return null;
        }
        #endregion

        #region Dispose
        private protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
            if (disposing && this._slaveConnections != null)
            {
                for (int i = 0; i < this._slaveConnections.Length; i++)
                {
                    this._slaveConnections[i]?.Clear();
                    this._slaveConnections[i] = null;
                }

#if LOW_NET || NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                this._syncAwaitSlaveConnection.Clear();
#endif
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#if !NET5_0_OR_GREATER
                this._asyncAwaitSlaveConnection.Clear();
#else
                this._asyncAwaitSlaveConnection.Writer.Complete();
#endif
#endif
            }

            this._slaveConnectionOptions = null;
            this._slaveConnections = null;
            this._slaveConnectionCount = null;
            this._syncAwaitSlaveConnection = null;
            this._slaveRandom = null;
#if !LOW_NET
            this._asyncAwaitSlaveConnection = null;
#endif
        }

        ~MasterSlavePool()
        {
            base.Dispose(true);
        }
        #endregion
    }
}
