using SharpRedis.Entities;
using SharpRedis.Exceptions;
using SharpRedis.Extensions;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SharpRedis.Network.Pool;

/// <summary>
/// A master and its replicas. All groups share the cluster dispatcher and monitor.
/// </summary>
internal sealed class ClusterNodePool : IConnectionPool
{
    private readonly ClusterConnectionPool _pool;
#if NET9_0_OR_GREATER
    private readonly Lock _gate = new();
#else
    private readonly object _gate = new();
#endif
    private readonly SharpConnectionOptions _masterOptions;
    private readonly SharpConnectionOptions[] _replicaOptions;
    private readonly ClusterConnection[] _replicas;
    private readonly ConcurrentDictionary<ClusterConnection, byte> _dedicated = new();
    private ClusterConnection _master = null!;
    private BaseConnection _subscriber = null!;
    private int _next = -1;
    private volatile bool _disposed;
    private volatile bool _retired;
    internal volatile bool _active;

    internal string Identity { get; }

    internal string Host => this._masterOptions.Host;

    internal int Port => this._masterOptions.Port;

#if MODERN_NET
    public ClientCachingBase? ClientCaching { get; }
#else
    public ClientCachingBase ClientCaching { get; }
#endif

    internal ClusterNodePool(ClusterConnectionPool pool, string identity, SharpConnectionOptions master, SharpConnectionOptions[] replicas)
    {
        this._pool = pool;
        this.Identity = identity;
        this._masterOptions = master;
        this._replicaOptions = replicas;
        this._replicas = new ClusterConnection[replicas.Length];
        if (pool.ClientCaching is { } cache) this.ClientCaching = new ClusterNodeCache(cache);
        this.Repair();
    }

    internal void Repair()
    {
        lock (this._gate)
        {
            if (this._disposed || this._retired) return;
            try
            {
                if (this.ClientCaching is not null || this._subscriber is not null)
                {
                    if (this._subscriber is not { Available: true })
                    {
                        this.ClientCaching?.ClearCache();
                        var old = this._subscriber;
                        var replacement = old is null ? this.CreateSubConnection() : old.Copy(this);
                        if (old is null && this._pool.ClientCaching is { } cache)
                        {
                            replacement.SubscribeClientCacheInvalidate(cache, this._masterOptions.CommandTimeout);
                        }
                        if (this.ClientCaching is { } nodeCache)
                        {
                            nodeCache.RedirectConnectionId = replacement.ClientId;
                        }
                        Volatile.Write(ref this._subscriber, replacement);
                        old?.Dispose();
                    }
                }
                if (this._master is not { Available: true })
                {
                    var replacement = this.Open(this._masterOptions, false, false);
                    Interlocked.Exchange(ref this._master, replacement)?.Dispose();
                }
                this.RetryTracking();
            }
            catch (Exception ex)
            {
                SharpConsole.WriteWarning("Cluster master recovery: " + ex.Message);
            }
            for (int i = 0; i < this._replicas.Length; i++)
            {
                if (this._replicas[i] is { Available: true }) continue;
                try
                {
                    Interlocked.Exchange(ref this._replicas[i], this.Open(this._replicaOptions[i], true, false))?.Dispose();
                }
                catch (Exception ex)
                {
                    SharpConsole.WriteWarning("Cluster replica recovery: " + ex.Message);
                }
            }
            foreach (var item in this._dedicated)
            {
                if (!item.Key.Available)
                {
                    item.Key.Dispose();
                    _ = this._dedicated.TryRemove(item.Key, out _);
                }
            }
        }
    }

    internal void Heartbeat()
    {
        lock (this._gate)
        {
            if (this._disposed || this._retired) return;
            this.Check(this._master, true);
            foreach (var replica in this._replicas)
            {
                this.Check(replica, false);
            }
            if (this._subscriber is { Available: true } subscriber)
            {
                try
                {
                    if (subscriber.Ping(this._masterOptions.CommandTimeout) == HeartbeatResult.Failed)
                    {
                        subscriber.Invalidate();
                        this.ClientCaching?.ClearCache();
                    }
                }
                catch
                {
                    subscriber.Invalidate();
                    this.ClientCaching?.ClearCache();
                }
            }
        }
        this.Repair();
    }

    private void Check(ClusterConnection connection, bool master)
    {
        if (connection is not { Available: true }) return;
        try
        {
            SentinelConnectionPool.VerifyRole(connection, master, this._masterOptions.CommandTimeout);
        }
        catch
        {
            connection.Invalidate();
            if (master)
            {
                this.ClientCaching?.ClearCache();
            }
        }
    }

    private ClusterConnection Open(SharpConnectionOptions options, bool replica, bool dedicated)
    {
        var connection = new ClusterConnection(options, this._pool.Dispatcher, this, replica, dedicated);
        try
        {
            if (dedicated) _ = this._dedicated.TryAdd(connection, 0);
            this.ThrowIfDisposed();
            ClusterConnectionPool.Initialize(connection, options);
            SentinelConnectionPool.VerifyRole(connection, !replica, options.CommandTimeout);
            if (replica)
            {
                connection.Readonly(options.CommandTimeout);
            }
            else if (!dedicated && this.ClientCaching is { } cache && this._subscriber is { Available: true })
            {
                connection.UseClientCaching(cache, options.CommandTimeout);
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

    private void ThrowIfDisposed()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(this._disposed || this._retired, this);
#else
        if (this._disposed || this._retired) throw new ObjectDisposedException(nameof(ClusterNodePool));
#endif
    }

    public BaseConnection GetMasterConnection()
    {
        this.ThrowIfDisposed();
        return this._master is { Available: true } master ? master : this.CreateMasterConnection();
    }

    public BaseConnection GetSlaveConnection() => this.GetSlaveConnection(false);

    internal BaseConnection GetSlaveConnection(bool required)
    {
        this.ThrowIfDisposed();
        int count = this._replicas.Length;
        int start = this._pool.Options.SlaveSelectionMode == SlaveSelectionMode.Random
            ? ClusterConnectionPool.NextRandom(count)
            : (int)((uint)Interlocked.Increment(ref this._next) % (uint)Math.Max(1, count));

        for (int n = 0; n < count; n++)
        {
            if (Volatile.Read(ref this._replicas[(start + n) % count]) is { Available: true } connection)
            {
                return connection;
            }
        }
        if (required) throw new RedisInitializationException("No available replica for the requested slot.");
        return this.GetMasterConnection();
    }

    public BaseConnection CreateMasterConnection()
    {
        this.ThrowIfDisposed();
        var connection = this.Open(this._masterOptions, false, true);
        return connection;
    }

    public BaseConnection CreateSlaveConnection()
        => throw new NotSupportedException("Dedicated replica connections are not supported.");

    public BaseConnection GetTempConnection(int database)
    {
        if (database != 0) throw new NotSupportedException("Redis Cluster only supports database 0.");
        return this.CreateMasterConnection();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Release(ClusterConnection connection)
        => connection.Dispose();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Forget(ClusterConnection connection)
        => this._dedicated.TryRemove(connection, out _);

    public BaseConnection CreateSubConnection()
    {
        this.ThrowIfDisposed();
        var connection = new SubscribeConnection(this._masterOptions, this._pool.Dispatcher, this._masterOptions.CommandTimeout);
        try
        {
            ClusterConnectionPool.Initialize(connection, this._masterOptions);
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    public BaseConnection GetSubConnection()
    {
        this.ThrowIfDisposed();
        if (this._subscriber is { Available: true } subscriber) return subscriber;
        lock (this._gate)
        {
            this.ThrowIfDisposed();
            if (this._subscriber is null) this._subscriber = this.CreateSubConnection();
            else if (!this._subscriber.Available) this.Repair();
            return this._subscriber;
        }
    }

    public bool IsCacheReady(BaseConnection connection, long redirectId)
        => this._active && !this._disposed && ReferenceEquals(connection, this._master) && connection.Available
            && redirectId > 0 && connection.RedirectConnectionId == redirectId
            && this.ClientCaching?.RedirectConnectionId == redirectId
            && this._subscriber is { Available: true } subscriber && subscriber.ClientId == redirectId;

    public void RetryTracking()
    {
        if (this.ClientCaching is { } cache && this._master is { Available: true } master
            && master.RedirectConnectionId != cache.RedirectConnectionId)
            master.ResetClientCaching(cache, this._masterOptions.CommandTimeout);
    }

    internal void Retire()
    {
        lock (this._gate)
        {
            this._retired = true;
            this._active = false;
            this._master?.Invalidate();
            foreach (var replica in this._replicas) replica?.Invalidate();
            this._subscriber?.Invalidate();
            foreach (var item in this._dedicated) item.Key.Dispose();
        }
    }

    internal void CopySubscriptionsFrom(ClusterNodePool previous)
    {
        if (previous._subscriber is not { } subscriber) return;
        lock (this._gate)
        {
            var replacement = subscriber.Copy(this);
            Interlocked.Exchange(ref this._subscriber, replacement)?.Dispose();
        }
    }

    public void Dispose()
    {
        lock (this._gate)
        {
            if (this._disposed) return;
            this._disposed = true; this._active = false;
            this._master?.Dispose(); this._subscriber?.Dispose();
            foreach (var replica in this._replicas) replica?.Dispose();
            foreach (var item in this._dedicated) item.Key.Dispose();
            this._dedicated.Clear();
        }
    }
}
