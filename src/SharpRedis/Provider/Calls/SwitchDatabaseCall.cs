#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
#if !LOW_NET
using System.Threading.Tasks;
#endif
#if NET40
using Task = System.Threading.Tasks.TaskEx;
#endif
using System.Threading;
using SharpRedis.Models;
using SharpRedis.Network.Standard;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Concurrent;
using SharpRedis.Network;

namespace SharpRedis.Provider.Calls
{
    internal sealed class SwitchDatabaseCall : BaseCall
    {
        private readonly ushort _databaseIndex;
        private ConcurrentStack<DatabaseConnection> _connections;
        private ConcurrentQueue<SyncGetConnectionManual> _syncQueue;
#if !LOW_NET
#if NET5_0_OR_GREATER
        private System.Threading.Channels.Channel<TaskCompletionSource<DatabaseConnection>> _asyncQueue;
#else
        private ConcurrentQueue<TaskCompletionSource<DatabaseConnection>> _asyncQueue;
#endif
#endif

        internal override bool SubUsable => true;

        internal override string CallMode => "Switch database mode";

        internal SwitchDatabaseCall(ushort databaseIndex, IConnectionPool connectionPool)
            : base(connectionPool)
        {
            this._databaseIndex = databaseIndex;
            this._connections = new ConcurrentStack<DatabaseConnection>();
            this._syncQueue = new ConcurrentQueue<SyncGetConnectionManual>();
#if !LOW_NET
#if NET5_0_OR_GREATER
            this._asyncQueue = System.Threading.Channels.Channel.CreateUnbounded<TaskCompletionSource<DatabaseConnection>>();
#else
            this._asyncQueue = new ConcurrentQueue<TaskCompletionSource<DatabaseConnection>>();
#endif
#endif
        }

        #region Sync
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        sealed internal override object? Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        sealed internal override object Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisSwitchDatabase), "The current connection has been released and cannot continue");
            var connection = this.GetMasterConnection(cancellationToken);
            try
            {
                return connection.Connection.ExecuteCommand(command, cancellationToken);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.ReturnDatabaseConnection(connection);
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override object?[]? Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        internal sealed override object[] Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisSwitchDatabase), "The current connection has been released and cannot continue");
            var connection = this.GetMasterConnection(cancellationToken);
            try
            {
                var result = connection.Connection.ExecuteCommands(commands, cancellationToken);
                if (result is Exception ex) throw ex;
                if (result is object[] array) return array;
                throw new RedisException("Not a valid pipe return value");
            }
            catch
            {
                throw;
            }
            finally
            {
                this.ReturnDatabaseConnection(connection);
            }
        }

        #endregion

        #region Async
#if !LOW_NET
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        sealed async internal override Task<object?> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        sealed async internal override Task<object> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisSwitchDatabase), "The current connection has been released and cannot continue");
            var connection = await this.GetMasterConnectionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await connection.Connection.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.ReturnDatabaseConnection(connection);
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal sealed override Task<object?[]?> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        async internal sealed override Task<object[]> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisSwitchDatabase), "The current connection has been released and cannot continue");
            var connection = await this.GetMasterConnectionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var result = await connection.Connection.ExecuteCommandsAsync(commands, cancellationToken).ConfigureAwait(false);
                if (result is Exception ex) throw ex;
                if (result is object[] array) return array;
                throw new RedisException("Not a valid pipe return value");
            }
            catch
            {
                throw;
            }
            finally
            {
                this.ReturnDatabaseConnection(connection);
            }
        }
#endif
        #endregion

        #region Get database connection
        internal DatabaseConnection GetMasterConnection(CancellationToken cancellationToken)
        {
            {
                if (this._connections.TryPop(out var itemConnection))
                {
                    return itemConnection;
                }
                var connection = base.ConnectionPool.GetMasterConnection(cancellationToken);
                var result = new DatabaseConnection(connection);
                if (connection.SwitchDatabaseIndex(this._databaseIndex))
                {
                    return result;
                }
                base.ConnectionPool.ReturnMasterConnection(connection);
            }

            var timeout = DateTime.UtcNow.AddMilliseconds(base.ConnectionPool.MasterConnectionOptions.CommandTimeout);
            var timespan = timeout - DateTime.UtcNow;
            if (cancellationToken != default)
            {
                if (cancellationToken.IsCancellationRequested) throw new TimeoutException("Get connection timeout");
            }
            else
            {
                if (timespan.TotalMilliseconds <= 0) throw new TimeoutException("Get connection timeout");
            }
            var syncWait = new SyncGetConnectionManual();
            this._syncQueue.Enqueue(syncWait);
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
                    if (syncWait.Connection.HasValue) return syncWait.Connection.Value;
                }
                throw new TimeoutException("Get connection timeout");
            }
            if (!syncWait.Connection.HasValue) throw new TimeoutException("Get connection timeout");
            return syncWait.Connection.Value;
        }

#if !LOW_NET
        internal async Task<DatabaseConnection> GetMasterConnectionAsync(CancellationToken cancellationToken)
        {
            {
                if (this._connections.TryPop(out var itemConnection))
                {
                    return itemConnection;
                }
                var connection = await base.ConnectionPool.GetMasterConnectionAsync(CancellationToken.None).ConfigureAwait(false);
                var result = new DatabaseConnection(connection);
                if (connection.SwitchDatabaseIndex(this._databaseIndex))
                {
                    return result;
                }
                base.ConnectionPool.ReturnMasterConnection(connection);
            }

            var timeout = DateTime.UtcNow.AddMilliseconds(base.ConnectionPool.MasterConnectionOptions.CommandTimeout);
            var timespan = timeout - DateTime.UtcNow;
            if (cancellationToken != default)
            {
                if (cancellationToken.IsCancellationRequested) throw new TimeoutException("Get connection timeout");
            }
            else
            {
                if (timespan.TotalMilliseconds <= 0) throw new TimeoutException("Get connection timeout");
            }
            var awaitTcs = new TaskCompletionSource<DatabaseConnection>();
#if NET5_0_OR_GREATER
            await this._asyncQueue.Writer.WriteAsync(awaitTcs, cancellationToken).ConfigureAwait(false);
#else
            this._asyncQueue.Enqueue(awaitTcs);
#endif
            try
            {
                if (cancellationToken == default)
                {
#if NET6_0_OR_GREATER
                    return await awaitTcs.Task.WaitAsync(timespan, CancellationToken.None).ConfigureAwait(false);
#else
                    var task = await Task.WhenAny(awaitTcs.Task, Task.Delay(timespan, CancellationToken.None)).ConfigureAwait(false);
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
        #endregion

        #region Return database connection
        internal void ReturnDatabaseConnection(DatabaseConnection connection)
        {
            if (!connection.Connection.Connected)
            {
                base.ConnectionPool.ReturnMasterConnection(connection.Connection);
                return;
            }

            if (this._syncQueue.TryDequeue(out var syncAwait))
            {
                if (syncAwait.SetConnection(connection))
                {
                    syncAwait.Dispose();
                    return;
                }
                else
                {
                    syncAwait.Dispose();
                    goto ReturnConnection;
                }
            }

#if !LOW_NET
#if NET5_0_OR_GREATER
            if (this._asyncQueue.Reader.TryRead(out var awaitTcs) && awaitTcs.Task.Status == TaskStatus.WaitingForActivation)
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
            if (this._asyncQueue.TryDequeue(out var awaitTcs) && awaitTcs.Task.Status == TaskStatus.WaitingForActivation)
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
            ReturnConnection:
            this._connections?.Push(connection);
        }
        #endregion

        #region Dispose
        sealed protected override void Dispose(bool disposing)
        {
            if (base._disposedValue) return;
            base._disposedValue = true;

            if (disposing)
            {
                var connections = this._connections?.ToArray();
                this._connections?.Clear();
                if (connections?.Length > 0)
                {
                    foreach (var connection in connections)
                    {
                        try
                        {
                            if (!connection.Connection.SwitchDatabaseIndex(connection.DefaultDatabaseIndex))
                            {
                                connection.Connection.Dispose();
                            }
                        }
                        catch
                        {
                            connection.Connection.Dispose();
                        }
                        base.ConnectionPool.ReturnMasterConnection(connection.Connection);
                    }
                }
#if LOW_NET || NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                this._syncQueue.Clear();
#endif
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#if !NET5_0_OR_GREATER
                this._asyncQueue.Clear();
#else
                this._asyncQueue.Writer.Complete();
#endif
#endif
            }
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
            this._connections = null;
            this._syncQueue = null;
#if !LOW_NET
            this._asyncQueue = null;
#endif
            base.Dispose(disposing);
        }

        ~SwitchDatabaseCall()
        {
            this.Dispose(true);
        }
#endregion

        internal readonly struct DatabaseConnection
        {
            internal ushort DefaultDatabaseIndex { get; }

            internal DefaultConnection Connection { get; }

            internal DatabaseConnection(DefaultConnection connection)
            {
                this.Connection = connection;
                this.DefaultDatabaseIndex = connection.CurrentDataBaseIndex;
            }
        }

        private class SyncGetConnectionManual : IDisposable
        {
            private bool _disposedValue;
            private volatile bool _timeout;
            private DatabaseConnection? _connection;

            internal DatabaseConnection? Connection => this._connection;

#if !LOW_NET
            private ManualResetEventSlim _waitHandle = new ManualResetEventSlim(false);

            internal ManualResetEventSlim WaitHandle => this._waitHandle;
#else
            private ManualResetEvent _waitHandle = new ManualResetEvent(false);

            internal ManualResetEvent WaitHandle => this._waitHandle;
#endif

            internal bool SetConnection(DatabaseConnection connection)
            {
                if (this._timeout) return false;
                lock (this)
                {
                    if (this._timeout) return false;
                    this._connection = connection;
                    this._waitHandle.Set();
                    return true;
                }
            }

            internal bool SetTimeout()
            {
                if (this._connection != null) return false;
                lock (this)
                {
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
