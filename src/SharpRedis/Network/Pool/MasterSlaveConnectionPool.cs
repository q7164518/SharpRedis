using SharpRedis.Entities;
using SharpRedis.Entities.Command;
using SharpRedis.Exceptions;
using SharpRedis.Extensions;
using SharpRedis.Pipelines.Middlewares.ClientCaching;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SharpRedis.Network.Pool;

internal sealed class MasterSlaveConnectionPool : IReadWriteConnectionPool
{
    private const int _masterIndex = 0;
    private const int _subIndex = 1;
    private const int _slaveStart = 2;
#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif
    // Master, subscription connection, then one persistent connection per slave.
    private readonly object[] _connections;
    private readonly SharpConnectionOptions[] _nodeOptions;
    private readonly CommandDispatcher _commandDispatcher;
    private readonly ConcurrentDictionary<TempConnection, byte> _temporaryConnections = new();
    private readonly ManualResetEventSlim _stop = new(false);
    private readonly Thread _heartbeatThread;
    private readonly int _heartbeatInterval;
    private readonly int _reconnectInterval;
    private readonly SlaveSelectionMode _selectionMode;
    private int _nextSlave = -1;
    private readonly bool _heartbeatStarted;
    private readonly bool _initialRetry;
    private volatile bool _disposedValue;
    private volatile bool _suspended;
    private readonly bool _ownsCache;
    private readonly bool _verifyRoles;
    private readonly ConcurrentDictionary<BaseConnection, byte> _dedicatedConnections = new();
#if MODERN_NET
#if !NET6_0_OR_GREATER
    [ThreadStatic] private static Random? _random;
#endif
#else
    [ThreadStatic] private static Random _random;
#endif

#if MODERN_NET
    private readonly ClientCachingBase? _clientCaching;
    private readonly CacheKeyInvoker? _cacheKeyInvoker;
    public ClientCachingBase? ClientCaching => this._clientCaching;
#else
    private readonly ClientCachingBase _clientCaching;
    private readonly CacheKeyInvoker _cacheKeyInvoker;
    public ClientCachingBase ClientCaching => this._clientCaching;
#endif

#if MODERN_NET
    internal MasterSlaveConnectionPool(MasterSlaveConnectionOptions options, ClientCachingBase? clientCaching, bool ownsCache = true, bool verifyRoles = false)
#else
    internal MasterSlaveConnectionPool(MasterSlaveConnectionOptions options, ClientCachingBase clientCaching, bool ownsCache = true, bool verifyRoles = false)
#endif
    {
        this._ownsCache = ownsCache;
        this._verifyRoles = verifyRoles;
        this._clientCaching = clientCaching;
        if (clientCaching is not null)
            this._cacheKeyInvoker = new(options.Encoding, options.Prefix ?? string.Empty, options.Suffix ?? string.Empty);
        if (options.Slaves.Count == 0 && !verifyRoles) throw new ArgumentException("At least one slave endpoint is required.");
        this._heartbeatInterval = options.HeartbeatInterval;
        this._reconnectInterval = options.ReconnectInterval;
        this._selectionMode = options.SlaveSelectionMode;
        this._nodeOptions = new SharpConnectionOptions[options.Slaves.Count + 1];
        this._connections = new object[options.Slaves.Count + MasterSlaveConnectionPool._slaveStart];
        this._nodeOptions[0] = CreateOptions(options, options.Host, options.Port, options.User, options.Password, options.AddressFamily);
        for (int i = 0; i < options.Slaves.Count; i++)
        {
            var slave = options.Slaves[i] ?? throw new ArgumentException("Slave endpoints cannot be null.");
            this._nodeOptions[i + 1] = CreateOptions(options, slave.Host, slave.Port, slave.User, slave.Password, slave.AddressFamily, slave);
        }
        this._commandDispatcher = new(options.BatchCount, options.Encoding);
        this._heartbeatThread = new Thread(this.Heartbeat)
        {
            IsBackground = true,
            Name = "SharpRedis_MasterSlave_Heartbeat",
        };
        try
        {
            if (this._clientCaching is not null)
            {
                var subscriber = this.CreateSubConnection();
                this._connections[MasterSlaveConnectionPool._subIndex] = subscriber;
                subscriber.SubscribeClientCacheInvalidate(this._clientCaching, options.CommandTimeout);
                this._clientCaching.RedirectConnectionId = subscriber.ClientId;
            }
            this._connections[MasterSlaveConnectionPool._masterIndex] = this.CreateMasterConnection();
            for (int i = MasterSlaveConnectionPool._slaveStart; i < this._connections.Length; i++)
            {
                try
                {
                    this._connections[i] = this.CreateNodeConnection(i - 1, enableTracking: false);
                }
                catch (Exception ex)
                {
                    this._initialRetry = true;
                    SharpConsole.WriteWarning($"Slave connection failed: {ex.Message}");
                }
            }
            this._heartbeatThread.Start();
            this._heartbeatStarted = true;
        }
        catch
        {
            this.Dispose();
            throw;
        }
    }

#if MODERN_NET
    private static SharpConnectionOptions CreateOptions(MasterSlaveConnectionOptions options, string host, int port, string? user, string? password, System.Net.Sockets.AddressFamily family, StandaloneConnectionOptions? endpoint = null)
#else
    private static SharpConnectionOptions CreateOptions(MasterSlaveConnectionOptions options, string host, int port, string user, string password, System.Net.Sockets.AddressFamily family, StandaloneConnectionOptions endpoint = null)
#endif
        => new(host, port, options.RespVersion, user, password, options.ConnectName, options.Prefix, options.Suffix,
            family, options.BatchCount, options.BuffSize, options.Database, options.CommandTimeout, options.Encoding, SharpConnectionOptions.TlsOptions.Create(options, endpoint));

    public BaseConnection GetMasterConnection()
    {
        this.ThrowIfDisposed();
        if (this._connections[MasterSlaveConnectionPool._masterIndex] is BaseConnection { Available: true } connection) return connection;
        return this.GetTempConnection(this._nodeOptions[0].Database);
    }

    public BaseConnection GetSlaveConnection()
        => this.GetSlaveConnection(requireSlave: false);

    public BaseConnection GetSlaveConnection(bool requireSlave)
    {
        this.ThrowIfDisposed();
        int index = this.SelectSlave();
        if (index >= 0) return (BaseConnection)Volatile.Read(ref this._connections[index]);
        if (requireSlave) throw new RedisInitializationException("No available slave connection.");
        return this.GetMasterConnection();
    }

    private int SelectSlave()
    {
        int count = this._connections.Length - MasterSlaveConnectionPool._slaveStart;
        if (count == 0) return -1;
        if (count == 1)
        {
            return Volatile.Read(ref this._connections[MasterSlaveConnectionPool._slaveStart]) is BaseConnection { Available: true }
                ? MasterSlaveConnectionPool._slaveStart
                : -1;
        }

        if (this._selectionMode == SlaveSelectionMode.Random)
        {
            // Reservoir sampling selects uniformly among currently available slaves, without allocations or locks.
            int selected = -1, available = 0;
            for (int i = MasterSlaveConnectionPool._slaveStart; i < this._connections.Length; i++)
            {
                if (Volatile.Read(ref this._connections[i]) is BaseConnection { Available: true }
#if NET6_0_OR_GREATER
                    && Random.Shared.Next(++available) == 0
#else
                    && (_random ??= new Random(Guid.NewGuid().GetHashCode())).Next(++available) == 0
#endif
                )
                {
                    selected = i;
                }
            }
            return selected;
        }

        int start = (int)(unchecked((uint)Interlocked.Increment(ref this._nextSlave)) % (uint)count);
        for (int i = 0; i < count; i++)
        {
            int index = MasterSlaveConnectionPool._slaveStart + (start + i) % count;
            if (Volatile.Read(ref this._connections[index]) is BaseConnection { Available: true }) return index;
        }
        return -1;
    }

    public BaseConnection GetSubConnection()
    {
        this.ThrowIfDisposed();
        if (Volatile.Read(ref this._connections[MasterSlaveConnectionPool._subIndex]) is BaseConnection existing) return existing;
        var candidate = this.CreateSubConnection();
        lock (this._lock)
        {
            if (this._disposedValue || this._suspended)
            {
                candidate.Dispose();
                throw new ObjectDisposedException(nameof(MasterSlaveConnectionPool));
            }
            if (this._connections[MasterSlaveConnectionPool._subIndex] is BaseConnection winner)
            {
                candidate.Dispose();
                return winner;
            }
            Volatile.Write(ref this._connections[MasterSlaveConnectionPool._subIndex], candidate);
            return candidate;
        }
    }

    public BaseConnection CreateMasterConnection()
    {
        var connection = this.CreateNodeConnection(0, true);
        if (this._verifyRoles)
        {
            this._dedicatedConnections.TryAdd(connection, 0);
            if (this._disposedValue || this._suspended) { connection.Dispose(); this.ThrowIfDisposed(); }
        }
        return connection;
    }

    public BaseConnection CreateSlaveConnection()
    {
        this.ThrowIfDisposed();
        int slot = this.SelectSlave();
        if (slot < 0) throw new RedisInitializationException("No available slave connection.");
        return this.CreateNodeConnection(slot - 1, false);
    }

    public BaseConnection CreateSubConnection()
    {
        this.ThrowIfDisposed();
        return this.Initialize(new SubscribeConnection(this._nodeOptions[0], this._commandDispatcher, this._verifyRoles ? this._nodeOptions[0].CommandTimeout : 0), this._nodeOptions[0], false);
    }

    public BaseConnection GetTempConnection(int database)
    {
        this.ThrowIfDisposed();
        var options = this._nodeOptions[0];
        var connection = new TempConnection(options, this._commandDispatcher, database, this._verifyRoles ? options.CommandTimeout : 0);
        this._temporaryConnections.TryAdd(connection, 0);
        try
        {
            // Register before initialization, then recheck shutdown so a concurrent
            // Dispose cannot leave a connection outside the shutdown snapshot.
            this.ThrowIfDisposed();
            this.Initialize(connection, options, true);
            if (this._verifyRoles) SentinelConnectionPool.VerifyRole(connection, true, options.CommandTimeout);
            if (database != options.Database
                && !connection.SelectDatabase(database, options.CommandTimeout))
            {
                throw new RedisInitializationException("Failed to switch the temporary connection database.");
            }
            this.ThrowIfDisposed();
            return connection;
        }
        catch
        {
            this.ReleaseTempConnection(connection);
            throw;
        }
    }

    public void ReleaseTempConnection(TempConnection connection)
    {
        connection.Dispose();
        this._temporaryConnections.TryRemove(connection, out _);
    }

    private BaseConnection CreateNodeConnection(int node, bool enableTracking)
    {
        var options = this._nodeOptions[node];
        var connection = this.Initialize(new StandaloneConnection(options, this._commandDispatcher, this._verifyRoles ? options.CommandTimeout : 0), options, enableTracking);
        try
        {
            if (this._verifyRoles) SentinelConnectionPool.VerifyRole(connection, node == 0, options.CommandTimeout);
            return connection;
        }
        catch { connection.Dispose(); throw; }
    }

    private BaseConnection Initialize(BaseConnection connection, SharpConnectionOptions options, bool enableCaching)
    {
        try
        {
            this.ThrowIfDisposed();
            if (!connection.Authentication(
                username: options.User,
                password: options.Password,
                respVer: options.RespVersion,
                dbIndex: options.Database,
                millisecondsTimeout: options.CommandTimeout))
            {
                throw new RedisInitializationException("Authentication failed.");
            }
            if (enableCaching && this._clientCaching is not null)
            {
                connection.UseClientCaching(this._clientCaching, options.CommandTimeout);
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

    public bool ShouldCache(ICommand command)
        => this._clientCaching is not null
            && (command.Mode & Config.WITH_CLIENT_CACHING_MODE) == Config.WITH_CLIENT_CACHING_MODE
            && this._clientCaching.MatchesKeys(command, this._cacheKeyInvoker!);

    public bool IsCacheReady(BaseConnection connection, long redirectId)
        => redirectId != 0 && !this._disposedValue && connection.Available
            && connection.RedirectConnectionId == redirectId
            && this._clientCaching is { } cache && cache.RedirectConnectionId == redirectId
            && Volatile.Read(ref this._connections[_subIndex]) is BaseConnection { Available: true } subscriber
            && subscriber.ClientId == redirectId;

    public void RetryTracking()
    {
        if (this._clientCaching is null) return;
        lock (this._lock)
        {
            if (this._disposedValue) return;
            if (this._connections[MasterSlaveConnectionPool._masterIndex] is BaseConnection { Available: true } master
                && master.RedirectConnectionId != this._clientCaching.RedirectConnectionId)
            {
                // Redis rejects overlapping BCAST prefixes when tracking is enabled twice.
                // Clear the old registration before binding the replacement subscriber.
                master.DisableClientCaching(this._nodeOptions[0].CommandTimeout);
                master.UseClientCaching(this._clientCaching, this._nodeOptions[0].CommandTimeout);
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (this._suspended) throw new RedisInitializationException("The Sentinel topology has been replaced.");
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(this._disposedValue, this);
#else
        if (this._disposedValue) throw new ObjectDisposedException(nameof(MasterSlaveConnectionPool));
#endif
    }

    private void Heartbeat()
    {
        int interval = this._initialRetry ? this._reconnectInterval : this._heartbeatInterval;
        while (!this._stop.Wait(interval))
        {
            lock (this._lock)
            {
                if (this._disposedValue) return;
                bool retry = false;
                for (int i = 0; i < this._connections.Length; i++)
                {
                    var connection = this._connections[i] as BaseConnection;
                    if (i == MasterSlaveConnectionPool._subIndex && connection is null) continue;
                    try
                    {
                        if (connection is { Available: true })
                        {
                            if (!this._verifyRoles && (DateTimeOffset.UtcNow - connection.LastActiveTime).TotalMilliseconds < this._heartbeatInterval) continue;
                            if (this._verifyRoles && i != _subIndex)
                                SentinelConnectionPool.VerifyRole(connection, i == _masterIndex, this._nodeOptions[0].CommandTimeout);
                            var heartbeat = connection.Ping(this._nodeOptions[0].CommandTimeout);
                            if (connection.Available && heartbeat is HeartbeatResult.Succeeded or HeartbeatResult.Deferred)
                            {
                                if (i == MasterSlaveConnectionPool._masterIndex) this.RetryTracking();
                                continue;
                            }
                        }
                        if (i is MasterSlaveConnectionPool._masterIndex or MasterSlaveConnectionPool._subIndex) this._clientCaching?.ClearCache();
                        connection?.Invalidate();
                        var replacement = i == MasterSlaveConnectionPool._subIndex
                            ? connection!.Copy(this)
                            : this.CreateNodeConnection(i == MasterSlaveConnectionPool._masterIndex ? 0 : i - 1, enableTracking: i == MasterSlaveConnectionPool._masterIndex);
                        Volatile.Write(ref this._connections[i], replacement);
                        connection?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        connection?.Invalidate();
                        if (i is _masterIndex or _subIndex) this._clientCaching?.ClearCache();
                        retry = true;
                        SharpConsole.WriteError($"Master/slave reconnection failed, slot: {i}, Msg: {ex.Message}");
                    }
                }
                foreach (var entry in this._dedicatedConnections)
                    if (!entry.Key.Available)
                    {
                        entry.Key.Dispose();
                        this._dedicatedConnections.TryRemove(entry.Key, out _);
                    }
                interval = retry ? this._reconnectInterval : this._heartbeatInterval;
            }
        }
    }

    // Sentinel changes topology only on its monitor thread. Stop reconnection to the old
    // endpoints, but retain subscription callbacks until they have been copied successfully.
    internal void SuspendForReplacement()
    {
        this._suspended = true;
        this._stop.Set();
        if (this._heartbeatStarted) this._heartbeatThread.Join();
        lock (this._lock)
        {
            foreach (var item in this._connections)
                if (item is BaseConnection connection) connection.Invalidate();
            foreach (var entry in this._temporaryConnections) this.ReleaseTempConnection(entry.Key);
            foreach (var entry in this._dedicatedConnections) entry.Key.Dispose();
        }
        this._commandDispatcher.Dispose();
    }

    internal void RestoreSubscriptionsFrom(MasterSlaveConnectionPool previous)
    {
        if (previous._connections[_subIndex] is not BaseConnection source) return;
        var replacement = source.Copy(this);
        var old = Interlocked.Exchange(ref this._connections[_subIndex], replacement);
        (old as BaseConnection)?.Dispose();
    }

    public void Dispose()
    {
        lock (this._lock)
        {
            if (this._disposedValue) return;
            this._disposedValue = true;
            this._stop.Set();
            foreach (var connection in this._connections) (connection as BaseConnection)?.Dispose();
            foreach (var entry in this._temporaryConnections)
                this.ReleaseTempConnection(entry.Key);
            foreach (var entry in this._dedicatedConnections) entry.Key.Dispose();
            this._dedicatedConnections.Clear();
        }
        this._commandDispatcher.Dispose();
        if (this._heartbeatStarted) this._heartbeatThread.Join();
        this._stop.Dispose();
        if (this._ownsCache) this._clientCaching?.Dispose();
        this._cacheKeyInvoker?.Dispose();
        GC.SuppressFinalize(this);
    }
}
