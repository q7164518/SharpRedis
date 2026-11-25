using SharpRedis.Entities;
using SharpRedis.Exceptions;
using SharpRedis.Extensions;
using SharpRedis.Provider.Standard;
using System;
using System.Diagnostics;
using System.Threading;

namespace SharpRedis.Network.Pool;

internal sealed class StandaloneConnectionPool : IConnectionPool
{
    private const int _masterIndex = 0;
    private const int _subIndex = 1;
    private const int _tempIndex = 2;
    private static readonly object _closedSlot = new();
#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif

    private readonly object[] _connections = [null!, null!, Array.Empty<TempConnection>()];
    private volatile bool _disposedValue;
    private readonly StandaloneConnectionOptions _connectionFullOptions;
    private readonly SharpConnectionOptions _connectionOptions;
    private readonly CommandDispatcher _commandDispatcher;

#if MODERN_NET
    private readonly ClientCachingBase? _clientCaching;
    public ClientCachingBase? ClientCaching => this._clientCaching;
#else
    private readonly ClientCachingBase _clientCaching;
    public ClientCachingBase ClientCaching => this._clientCaching;
#endif

#if MODERN_NET
    public StandaloneConnectionPool(StandaloneConnectionOptions connectionOptions, ClientCachingBase? clientCaching)
#else
    public StandaloneConnectionPool(StandaloneConnectionOptions connectionOptions, ClientCachingBase clientCaching)
#endif
    {
        this._clientCaching = clientCaching;
        this._connectionFullOptions = connectionOptions;
        this._connectionOptions = new(
            host: connectionOptions.Host,
            port: connectionOptions.Port,
            resp: connectionOptions.RespVersion,
            user: connectionOptions.User,
            password: connectionOptions.Password,
            connectName: connectionOptions.ConnectName,
            prefix: connectionOptions.Prefix,
            suffix: connectionOptions.Suffix,
            addressFamily: connectionOptions.AddressFamily,
            batchCount: connectionOptions.BatchCount,
            buffSize: connectionOptions.BuffSize,
            database: connectionOptions.Database,
            commandTimeout: connectionOptions.CommandTimeout,
            encoding: connectionOptions.Encoding,
            tls: SharpConnectionOptions.TlsOptions.Create(connectionOptions)
        );
        this._commandDispatcher = new(this._connectionOptions.BatchCount, this._connectionOptions.Encoding);

        try
        {
            if (this._clientCaching is not null)
            {
                var subConnection = this.CreateSubConnection();
                this._connections[StandaloneConnectionPool._subIndex] = subConnection;
                subConnection.SubscribeClientCacheInvalidate(this._clientCaching, connectionOptions.CommandTimeout);
                this._clientCaching.RedirectConnectionId = subConnection.ClientId;
            }
            this._connections[StandaloneConnectionPool._masterIndex] = this.CreateMasterConnection();
            new Thread(this.Heartbeat)
            {
                IsBackground = true,
                Name = "SharpRedis_Standalone_Heartbeat",
            }.Start();
        }
        catch
        {
            foreach (var connection in this._connections)
            {
                (connection as BaseConnection)?.Dispose();
            }
            this._commandDispatcher.Dispose();
            throw;
        }
    }

    public BaseConnection GetMasterConnection()
    {
        this.ThrowIfDisposed();
        var slot = this._connections[StandaloneConnectionPool._masterIndex];
        if (slot is BaseConnection connection)
        {
            return this.CheckConnection(connection);
        }
        throw new ObjectDisposedException(nameof(StandaloneConnectionPool));
    }

    public BaseConnection GetSlaveConnection()
        => this.GetMasterConnection();

    public BaseConnection GetSubConnection()
    {
        this.ThrowIfDisposed();
        ref object slot = ref this._connections[StandaloneConnectionPool._subIndex];
        var current = Volatile.Read(ref slot);
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(ReferenceEquals(current, StandaloneConnectionPool._closedSlot), this);
#else
        if (ReferenceEquals(current, StandaloneConnectionPool._closedSlot))
        {
            throw new ObjectDisposedException(nameof(StandaloneConnectionPool));
        }
#endif
        if (current is BaseConnection existing) return existing;

        var candidate = this.CreateSubConnection();
        current = Interlocked.CompareExchange(ref slot, candidate, null);
        if (current is null)
        {
            // Once published, the pool owns disposal of this connection.
            this.ThrowIfDisposed();
            return candidate;
        }

        candidate.Dispose();
        if (current is BaseConnection winner)
        {
            this.ThrowIfDisposed();
            return winner;
        }
        throw new ObjectDisposedException(nameof(StandaloneConnectionPool));
    }

    public BaseConnection GetTempConnection(int database)
    {
        this.ThrowIfDisposed();
        ref object slot = ref this._connections[StandaloneConnectionPool._tempIndex];
        if (Volatile.Read(ref slot) is TempConnection[] connections)
        {
            foreach (var connection in connections)
            {
                if (connection.Database != database || !connection.TryBorrow()) continue;
                try
                {
                    this.ThrowIfDisposed();
                    if (this._clientCaching is { } cache && connection.RedirectConnectionId != cache.RedirectConnectionId)
                    {
                        connection.ResetClientCaching(cache, this._connectionOptions.CommandTimeout);
                    }
                    this.ThrowIfDisposed();
                    return connection;
                }
                catch
                {
                    connection.Dispose();
                    throw;
                }
            }

            var candidate = new TempConnection(this._connectionOptions, this._commandDispatcher, database);
            try
            {
                this.InitializeConnection(candidate, enableCaching: true);
                if (database != this._connectionOptions.Database
                    && !candidate.SelectDatabase(database, this._connectionOptions.CommandTimeout))
                {
                    throw new RedisInitializationException("Failed to switch the temporary connection database.");
                }
                while (Volatile.Read(ref slot) is TempConnection[] current)
                {
                    this.ThrowIfDisposed();
                    var updated = new TempConnection[current.Length + 1];
                    Array.Copy(current, updated, current.Length);
                    updated[current.Length] = candidate;
                    if (ReferenceEquals(Interlocked.CompareExchange(ref slot, updated, current), current))
                    {
                        this.ThrowIfDisposed();
                        return candidate;
                    }
                }
                throw new ObjectDisposedException(nameof(StandaloneConnectionPool));
            }
            catch
            {
                candidate.Dispose();
                this.ThrowIfDisposed();
                throw;
            }
        }
        throw new ObjectDisposedException(nameof(StandaloneConnectionPool));
    }

    public BaseConnection CreateMasterConnection()
    {
        this.ThrowIfDisposed();
        return this.InitializeConnection(new StandaloneConnection(this._connectionOptions, this._commandDispatcher), enableCaching: true);
    }

    public BaseConnection CreateSlaveConnection()
    {
        return this.CreateMasterConnection();
    }

    public BaseConnection CreateSubConnection()
    {
        this.ThrowIfDisposed();
        return this.InitializeConnection(new SubscribeConnection(this._connectionOptions, this._commandDispatcher), enableCaching: false);
    }

    public bool IsCacheReady(BaseConnection connection, long redirectId)
        => redirectId != 0 && !this._disposedValue && connection.Available
            && connection.RedirectConnectionId == redirectId
            && this._clientCaching is { } cache && cache.RedirectConnectionId == redirectId
            && Volatile.Read(ref this._connections[StandaloneConnectionPool._subIndex]) is BaseConnection { Available: true } subscriber
            && subscriber.ClientId == redirectId;

    public void RetryTracking()
    {
        if (this._clientCaching is not null)
        {
            lock (this._lock)
            {
                if (this._disposedValue) return;
                if (Volatile.Read(ref this._connections[StandaloneConnectionPool._masterIndex]) is BaseConnection { Available: true } master)
                {
                    if (master.RedirectConnectionId != this._clientCaching.RedirectConnectionId)
                        master.ResetClientCaching(this._clientCaching, this._connectionOptions.CommandTimeout);
                }
            }
        }
    }

    private BaseConnection CheckConnection(BaseConnection connection)
    {
        this.ThrowIfDisposed();
        if (connection.Available) return connection;

        return this.GetTempConnection(this._connectionOptions.Database);
    }

    private void ThrowIfDisposed()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(this._disposedValue, this);
#else
        if (this._disposedValue) throw new ObjectDisposedException(nameof(StandaloneConnectionPool));
#endif
    }

    private BaseConnection InitializeConnection(BaseConnection connection, bool enableCaching)
    {
        try
        {
            this.ThrowIfDisposed();
            if (!connection.Authentication(
                username: this._connectionOptions.User,
                password: this._connectionOptions.Password,
                respVer: this._connectionOptions.RespVersion,
                dbIndex: this._connectionOptions.Database,
                millisecondsTimeout: this._connectionOptions.CommandTimeout))
            {
                throw new RedisInitializationException("Authentication failed.");
            }
            if (enableCaching && this._clientCaching is not null)
            {
                connection.UseClientCaching(this._clientCaching, this._connectionOptions.CommandTimeout);
            }
            this.ThrowIfDisposed();
            return connection;
        }
        catch
        {
            connection.Dispose();
            this.ThrowIfDisposed();
            throw;
        }
    }

    private void RetireIdleTempConnections()
    {
        ref object slot = ref this._connections[StandaloneConnectionPool._tempIndex];
        long now = Stopwatch.GetTimestamp();
        double idleTicks = this._connectionFullOptions.HeartbeatInterval * (Stopwatch.Frequency / 1000.0);
        while (Volatile.Read(ref slot) is TempConnection[] connections)
        {
            bool hasRetired = false;
            foreach (var connection in connections)
            {
                connection.RetireIfIdle(now, idleTicks);
                hasRetired |= connection.IsRetired;
            }
            if (!hasRetired) return;

            var remaining = Array.FindAll(connections, static connection => !connection.IsRetired);
            if (ReferenceEquals(Interlocked.CompareExchange(ref slot, remaining, connections), connections)) return;
        }
    }

    private void Heartbeat()
    {
        int sleepMilliseconds = this._connectionFullOptions.HeartbeatInterval;
        while (!this._disposedValue)
        {
            Thread.Sleep(sleepMilliseconds);
            lock (this._lock)
            {
                if (this._disposedValue) break;
                this.RetireIdleTempConnections();

                bool hasReconnect = false;
                for (int i = 0; i < _tempIndex; i++)
                {
                    if (Volatile.Read(ref this._connections[i]) is not BaseConnection connection) continue;
                    if (connection.Available)
                    {
                        var idleMilliseconds = (DateTimeOffset.UtcNow - connection.LastActiveTime).TotalMilliseconds;
                        if (idleMilliseconds < this._connectionFullOptions.HeartbeatInterval) continue;

                        var heartbeat = connection.Ping(this._connectionFullOptions.CommandTimeout);
                        if (connection.Available && heartbeat == HeartbeatResult.Succeeded)
                        {
                            if (this._clientCaching is not null) connection.RetryTracking(this._clientCaching);
                            continue;
                        }
                        if (connection.Available && heartbeat == HeartbeatResult.Deferred) continue;

                        SharpConsole.WriteWarning($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Connection to health monitoring failed, ConnectName: {connection.ConnectionName}");
                    }

                    this._clientCaching?.ClearCache();
                    connection.Invalidate();
                    try
                    {
                        BaseConnection newConnection = connection.Copy(this);
                        Volatile.Write(ref this._connections[i], newConnection);
                        SharpConsole.WriteSucceed($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Reconnection successful. ConnectName: {newConnection.ConnectionName}");
                        connection.Dispose();
                    }
                    catch (Exception ex)
                    {
                        var now = DateTime.Now;
                        SharpConsole.WriteError($"Time: {now:yyyy-MM-dd HH:mm:ss}, reconnection error, Msg: [{ex.Message}] The next reconnection time: {now.AddMilliseconds(this._connectionFullOptions.ReconnectInterval):yyyy-MM-dd HH:mm:ss}");
                        hasReconnect = true;
                    }
                }

                if (hasReconnect)
                {
                    sleepMilliseconds = this._connectionFullOptions.ReconnectInterval;
                }
                else
                {
                    sleepMilliseconds = this._connectionFullOptions.HeartbeatInterval;
                }
            }
        }
    }

    public void Dispose()
    {
        lock (this._lock)
        {
            if (this._disposedValue) return;
            this._disposedValue = true;
            if (Interlocked.Exchange(ref this._connections[StandaloneConnectionPool._tempIndex], StandaloneConnectionPool._closedSlot) is TempConnection[] temporary)
            {
                foreach (var connection in temporary) connection.Dispose();
            }
            for (var i = 0; i < StandaloneConnectionPool._tempIndex; i++)
            {
                var connection = Interlocked.Exchange(ref this._connections[i], StandaloneConnectionPool._closedSlot);
                (connection as BaseConnection)?.Dispose();
            }
            this._clientCaching?.Dispose();
            this._commandDispatcher.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
