using SharpRedis.Entities;
using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Exceptions;
using SharpRedis.Extensions;
using SharpRedis.Pipelines;
using SharpRedis.Pipelines.Middlewares.ClientCaching;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SharpRedis.Network.Pool;

internal sealed class ClusterConnectionPool : IConnectionPool
{
    private readonly ConcurrentDictionary<ClusterConnection, byte> _redirects = new();
#if NET9_0_OR_GREATER
    private readonly Lock _gate = new();
#else
    private readonly object _gate = new();
#endif
    private readonly ManualResetEventSlim _stop = new(false);
    private readonly AutoResetEvent _refresh = new(false);
    private readonly Thread _monitor;
    private readonly bool _started;
    private BaseConnection _subscriber = null!;
    private volatile bool _disposed;
    private volatile bool _cacheValid = true;
    private Snapshot _snapshot = new(new ClusterNodePool[16384], [], string.Empty);
#nullable enable
    private readonly Dictionary<(string Host, int Port), SharpConnectionOptions.TlsOptions>? _seedTls;
#if MODERN_NET
    private readonly Dictionary<(string Host, int Port), string>? _unixSockets;
#endif
#nullable restore
#if MODERN_NET
    private readonly SharpConnectionOptions.TlsOptions? _tls;
#else
    private readonly SharpConnectionOptions.TlsOptions _tls;
#endif
    private readonly byte[] _prefix;
    private readonly byte[] _suffix;
    private readonly CacheKeyInvoker _keys;
#if MODERN_NET
#if !NET6_0_OR_GREATER
    [ThreadStatic] private static Random? _random;
#endif
#else
    [ThreadStatic] private static Random _random;
#endif

    internal ClusterConnectionOptions Options { get; }

    internal CommandDispatcher Dispatcher { get; }

    internal RedisDelegate TransactionExecutor { get; set; } = null!;

    internal AsyncRedisDelegate AsyncTransactionExecutor { get; set; } = null!;

#if MODERN_NET
    public ClientCachingBase? ClientCaching { get; }

    internal ClusterConnectionPool(ClusterConnectionOptions options, ClientCachingBase? cache)
#else
    public ClientCachingBase ClientCaching { get; }

    internal ClusterConnectionPool(ClusterConnectionOptions options, ClientCachingBase cache)
#endif
    {
        this.Options = options;
#if MODERN_NET
        if (options.ConfiguredUnixSockets is { Count: > 0 } mappings)
        {
            this._unixSockets = new(mappings, EndpointComparer._instance);
        }
        foreach (var seed in options.Nodes)
        {
            if (seed.AddressFamily == System.Net.Sockets.AddressFamily.Unix)
            {
                (this._unixSockets ??= [with(EndpointComparer._instance)]).TryAdd(("127.0.0.1", seed.Port), seed.Host);
            }
        }
#endif
        bool tlsEnabled = options.Ssl || options.Nodes.Exists(static node => node.Ssl);
        this._tls = SharpConnectionOptions.TlsOptions.Create(options, enable: tlsEnabled);
        if (tlsEnabled)
        {
            foreach (var seed in options.Nodes)
            {
                if (seed.SslHost is null
                    && seed.CertificateValidation is null
                    && seed.CertificateSelection is null
                    && !seed.CheckCertificateRevocation
                    && seed.ConfiguredCertificates is not { Count: > 0 })
                {
                    continue;
                }
                (this._seedTls ??= [with(EndpointComparer._instance)])[(seed.Host, seed.Port)] = SharpConnectionOptions.TlsOptions.Create(options, seed, enable: true)!;
            }
        }
        this.ClientCaching = cache;
        this._prefix = options.Encoding.GetBytes(options.Prefix ?? string.Empty);
        this._suffix = options.Encoding.GetBytes(options.Suffix ?? string.Empty);
        this._keys = new(options.Encoding, options.Prefix ?? string.Empty, options.Suffix ?? string.Empty);
        this.Dispatcher = new(options.BatchCount, options.Encoding);
        this._monitor = new Thread(this.Monitor) { IsBackground = true, Name = "SharpRedis_Cluster_Heartbeat" };
        try
        {
            this.Refresh();
            this._monitor.Start();
            this._started = true;
        }
        catch
        {
            this.Dispose();
            throw;
        }
    }

#if NET6_0_OR_GREATER
    internal static int NextRandom(int count)
        => count == 0 ? 0 : Random.Shared.Next(count);
#else
    internal static int NextRandom(int count)
        => count == 0 ? 0 : (ClusterConnectionPool._random ??= new Random(Guid.NewGuid().GetHashCode())).Next(count);
#endif

    // Validate before any network action; return as soon as the first differing slot is found.
    internal int GetSlot(ICommand command)
    {
        if (command.DataBaseIndex != 0) throw new NotSupportedException("Redis Cluster only supports database 0.");
        int slot = -1;
        if (command.IsBatch)
        {
            foreach (var child in command.BatchCommands) this.CheckKeys(child, ref slot);
        }
        else this.CheckKeys(command, ref slot);
        return slot;
    }

    internal void CheckKeys(ICommand command, ref int slot)
    {
        foreach (var item in command.Items)
        {
            if (item.Category != CommandItemCategory.Key) continue;
            int current = CRC16.GetClusterSlot(in item, this.Options.Encoding, this._prefix, this._suffix);
            if (slot < 0) slot = current;
            else if (current != slot)
                throw new ArgumentException($"CROSSSLOT: expected slot {slot}, but encountered slot {current}. No command was sent.");
        }
    }

    internal bool ShouldCache(ICommand command)
        => this.ClientCaching is not null && !command.IsBatch
            && (command.Mode & Config.WITH_CLIENT_CACHING_MODE) == Config.WITH_CLIENT_CACHING_MODE
            && this.ClientCaching.MatchesKeys(command, this._keys);

    internal ClusterNodePool Node(int slot)
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(this._disposed, this);
#else
        if (this._disposed) throw new ObjectDisposedException(nameof(ClusterConnectionPool));
#endif
        var snapshot = Volatile.Read(ref this._snapshot);
        if (slot >= 0)
        {
            var node = snapshot._slots[slot];
            if (node is not null) return node;
            this._refresh.Set();
            throw new RedisInitializationException("No cluster node currently owns slot " + slot);
        }
        if (snapshot._nodes.Length == 0) throw new RedisInitializationException("No available Cluster topology.");
        return snapshot._nodes[0];
    }

    public BaseConnection GetMasterConnection()
        => this.Node(-1).GetMasterConnection();

    public BaseConnection GetSlaveConnection()
        => this.Options.EnableReadWriteSeparation ? this.Node(-1).GetSlaveConnection() : this.GetMasterConnection();

    public BaseConnection CreateMasterConnection()
        => this.Node(-1).CreateMasterConnection();

    public BaseConnection CreateSlaveConnection()
        => this.Options.EnableReadWriteSeparation ? this.Node(-1).CreateSlaveConnection() : this.CreateMasterConnection();

    public BaseConnection CreateSubConnection()
        => this.Node(-1).CreateSubConnection();

    public BaseConnection GetTempConnection(int database)
        => this.Node(-1).GetTempConnection(database);

    public bool IsCacheReady(BaseConnection connection, long redirectId)
        => !this._disposed && this._cacheValid && connection is ClusterConnection { Node: { } node }
            && node.IsCacheReady(connection, redirectId);

    public BaseConnection GetSubConnection()
    {
        if (this._subscriber is { Available: true } subscriber) return subscriber;
        lock (this._gate)
        {
#if NET7_0_OR_GREATER
            ObjectDisposedException.ThrowIf(this._disposed, this);
#else
            if (this._disposed) throw new ObjectDisposedException(nameof(ClusterConnectionPool));
#endif
            var previous = this._subscriber;
            if (previous is { Available: true }) return previous;
            this._subscriber = previous is null ? this.CreateSubConnection() : previous.Copy(new SubscriptionFactory(this.Node(-1)));
            previous?.Dispose();
            return this._subscriber;
        }
    }

    public void RetryTracking()
    {
        foreach (var node in Volatile.Read(ref this._snapshot)._nodes) node.RetryTracking();
    }

    internal SharpConnectionOptions Settings(string host, int port)
    {
#if MODERN_NET
        string? path = null;
        this._unixSockets?.TryGetValue((host, port), out path);
        var family = path is null ? BasalConnectionOptions.ResolveAddressFamily(host)
            : System.Net.Sockets.AddressFamily.Unix;
#else
        var family = BasalConnectionOptions.ResolveAddressFamily(host);
#endif
        var options = this.Options;
        return new(
            host,
            port,
            options.RespVersion,
            options.User,
            options.Password,
            options.ConnectName,
            options.Prefix,
            options.Suffix,
            family,
            options.BatchCount,
            options.BuffSize,
            0,
            options.CommandTimeout,
            options.Encoding,
            this._seedTls is not null && this._seedTls.TryGetValue((host, port), out var tls) ? tls : this._tls
#if MODERN_NET
            , path
#endif
        );
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Initialize(BaseConnection connection, SharpConnectionOptions options)
    {
        if (!connection.Authentication(options.User, options.Password, options.RespVersion, 0, options.CommandTimeout))
            throw new RedisInitializationException("Cluster node authentication failed.");
    }

    internal ClusterConnection Redirect(string host, int port, bool moved)
    {
        if (BasalConnectionOptions.IsUnixSocketHost(host.Trim()))
            throw new RedisInitializationException("Invalid Cluster redirect host.");

        // Redirected requests bypass caching until discovery confirms the new topology.
        if (moved)
        {
            this._cacheValid = false;
            this.ClientCaching?.ClearCache();
            this._refresh.Set();
        }
        var options = this.Settings(host, port);
        var connection = new ClusterConnection(options, this.Dispatcher, null!, dedicated: true);
        try
        {
            this._redirects.TryAdd(connection, 0);
            ClusterConnectionPool.Initialize(connection, options);
#if NET7_0_OR_GREATER
            ObjectDisposedException.ThrowIf(this._disposed, this);
#else
            if (this._disposed) throw new ObjectDisposedException(nameof(ClusterConnectionPool));
#endif
            return connection;
        }
        catch
        {
            this.Release(connection);
            throw;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Release(ClusterConnection connection)
    {
        if (connection.Node is not null)
        {
            ClusterNodePool.Release(connection);
        }
        else
        {
            connection.Dispose();
            this._redirects.TryRemove(connection, out _);
        }
    }

    private void Monitor()
    {
        var handles = new[] { this._stop.WaitHandle, this._refresh };
        var heartbeatAt = DateTime.UtcNow;
        while (WaitHandle.WaitAny(handles, this.Options.ClusterRefreshInterval) != 0)
        {
            try
            {
                this.Refresh();
                bool heartbeat = (DateTime.UtcNow - heartbeatAt).TotalMilliseconds >= this.Options.HeartbeatInterval;
                foreach (var node in Volatile.Read(ref this._snapshot)._nodes)
                {
                    if (heartbeat) node.Heartbeat();
                    else node.Repair();
                }
                if (this._subscriber is { } subscriber)
                {
                    if (heartbeat && subscriber.Available)
                    {
                        try
                        {
                            if (subscriber.Ping(this.Options.CommandTimeout) == HeartbeatResult.Failed)
                                subscriber.Invalidate();
                        }
                        catch
                        {
                            subscriber.Invalidate();
                        }
                    }
                    if (!subscriber.Available) this.GetSubConnection();
                }
                if (heartbeat) heartbeatAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                SharpConsole.WriteWarning("Cluster topology refresh: " + ex.Message);
            }
        }
    }

    private void Refresh()
    {
        lock (this._gate)
        {
            if (this._disposed) return;
            var old = Volatile.Read(ref this._snapshot);
            var seeds = old._nodes.Select(static n => (n.Host, n.Port)).Concat(this.Options.Nodes.Select(static n => (n.Host, n.Port)))
                .Distinct()
                .ToArray();
            RedisCluster.SlotInfo[] ranges = null!;
            string sourceHost = string.Empty;
            Exception failure = null!;
            foreach (var (Host, Port) in seeds)
            {
                try
                {
                    var settings = this.Settings(Host, Port);
                    using var discovery = new ClusterConnection(settings, this.Dispatcher, null!);
                    ClusterConnectionPool.Initialize(discovery, settings);
                    ranges = discovery.ClusterSlots(this.Options.CommandTimeout);
                    sourceHost = BasalConnectionOptions.IsUnixSocketHost(Host) ? "127.0.0.1" : Host;
                    break;
                }
                catch (Exception ex)
                {
                    failure = ex;
                }
            }
            if (ranges is null) throw new RedisInitializationException("Cluster discovery failed: " + failure?.Message);
            var definitions = new Dictionary<string, Definition>(StringComparer.Ordinal);
            var owners = new string[16384];
            foreach (var range in ranges)
            {
                int first = range.StartSlot, last = range.EndSlot;
                var (host, port) = ClusterConnectionPool.Endpoint(range.Master, sourceHost);
                string id = host + ":" + port;
                if (!definitions.ContainsKey(id))
                {
                    var replicas = new List<SharpConnectionOptions>();
                    if (this.Options.EnableReadWriteSeparation)
                    {
                        for (int i = 0; i < range.Replicas.Length; i++)
                        {
                            var replica = ClusterConnectionPool.Endpoint(range.Replicas[i], sourceHost);
                            replicas.Add(this.Settings(replica.host, replica.port));
                        }
                    }
                    replicas.Sort((a, b) => string.CompareOrdinal(a.Host + ":" + a.Port, b.Host + ":" + b.Port));
                    definitions.Add(id, new(this.Settings(host, port), [.. replicas]));
                }
                for (int slot = first; slot <= last; slot++)
                {
                    if (owners[slot] is not null) throw new RedisInitializationException("Overlapping cluster slot ranges.");
                    owners[slot] = id;
                }
            }
            if (definitions.Count == 0) throw new RedisInitializationException("Cluster has no assigned slots.");
            var signature = new StringBuilder();
            string previousOwner = null!;
            for (int slot = 0; slot < owners.Length; slot++)
            {
                if (slot == 0 || owners[slot] != previousOwner)
                {
                    _ = signature
                        .Append(slot)
                        .Append(':')
                        .Append(owners[slot])
                        .Append('|');
                    previousOwner = owners[slot];
                }
            }
            foreach (var pair in definitions.OrderBy(pair => pair.Key, StringComparer.Ordinal))
            {
                _ = signature
                    .Append(pair.Key)
                    .Append('=');
                foreach (var replica in pair.Value._replicas)
                {
                    _ = signature
                        .Append(replica.Host)
                        .Append(':')
                        .Append(replica.Port)
                        .Append(',');
                }
            }
            string version = signature.ToString();
            if (version == old._signature && old._nodes.All(static n => n._active)) return;

            var created = new Dictionary<string, ClusterNodePool>(StringComparer.Ordinal);
            try
            {
                foreach (var pair in definitions)
                {
                    created.Add(pair.Key, new(this, pair.Key, pair.Value._master, pair.Value._replicas));
                }
#if NET7_0_OR_GREATER
                ObjectDisposedException.ThrowIf(this._disposed, this);
#else
                if (this._disposed) throw new ObjectDisposedException(nameof(ClusterConnectionPool));
#endif
                this._cacheValid = false;
                this.ClientCaching?.ClearCache();
                foreach (var node in old._nodes) node.Retire();

                foreach (var node in old._nodes)
                {
                    if (created.TryGetValue(node.Identity, out var replacement))
                    {
                        replacement.CopySubscriptionsFrom(node);
                    }
                }
                var nodes = created.Values.OrderBy(static n => n.Identity, StringComparer.Ordinal).ToArray();

                // Ordinary Pub/Sub is independent of each master's cache invalidation subscriber.
                if (this._subscriber is { } subscriber)
                {
                    subscriber.Invalidate();
                    this._subscriber = subscriber.Copy(new SubscriptionFactory(nodes[0]));
                    subscriber.Dispose();
                }
                var slots = new ClusterNodePool[16384];
                for (int i = 0; i < slots.Length; i++)
                {
                    if (owners[i] is { } id)
                    {
                        slots[i] = created[id];
                    }
                }
                foreach (var node in nodes)
                {
                    node._active = true;
                }
                Volatile.Write(ref this._snapshot, new Snapshot(slots, nodes, version));
                this.ClientCaching?.ClearCache();
                this._cacheValid = true;
            }
            catch
            {
                foreach (var node in created.Values)
                {
                    node.Dispose();
                }
                throw;
            }
            foreach (var node in old._nodes)
            {
                node.Dispose();
            }
        }
    }

    private static (string host, int port) Endpoint(RedisCluster.SlotNode node, string fallbackHost)
    {
        if (node.HasUnknownEndpoint) throw new RedisInitializationException("Unknown cluster endpoint.");
        string host = node.RequiresEndpointFallback ? fallbackHost : node.Endpoint!;
        if (BasalConnectionOptions.IsUnixSocketHost(host.Trim()))
            throw new RedisInitializationException("Invalid Cluster endpoint host.");
        int port = node.Port;
        if (port <= 0 || port > 65535) throw new RedisInitializationException("Invalid cluster endpoint port.");
        return (host, port);
    }

    public void Dispose()
    {
        lock (this._gate)
        {
            if (this._disposed) return;
            this._disposed = true; this._stop.Set();
            this._subscriber?.Dispose();
            foreach (var node in this._snapshot._nodes) node.Dispose();
            foreach (var item in this._redirects) item.Key.Dispose();
            this._redirects.Clear();
        }
        this.Dispatcher.Dispose();
        if (this._started) this._monitor.Join();
        this._stop.Dispose();
        this._refresh.Dispose();
        this._keys.Dispose();
        this.ClientCaching?.Dispose();
    }

    private sealed class SubscriptionFactory(ClusterNodePool node) : IConnectionPool
    {
#if MODERN_NET
        public ClientCachingBase? ClientCaching => null;
#else
        public ClientCachingBase ClientCaching => null!;
#endif
        public BaseConnection CreateSubConnection() => node.CreateSubConnection();

        public BaseConnection GetSubConnection() => this.CreateSubConnection();

        public BaseConnection CreateMasterConnection() => throw new NotSupportedException();

        public BaseConnection CreateSlaveConnection() => throw new NotSupportedException();

        public BaseConnection GetMasterConnection() => throw new NotSupportedException();

        public BaseConnection GetSlaveConnection() => throw new NotSupportedException();

        public BaseConnection GetTempConnection(int database) => throw new NotSupportedException();

        public bool IsCacheReady(BaseConnection connection, long redirectId) => false;

        public void RetryTracking() { }

        public void Dispose() { }
    }

    private sealed class Snapshot(ClusterNodePool[] slots, ClusterNodePool[] nodes, string signature)
    {
        internal readonly ClusterNodePool[] _slots = slots;
        internal readonly ClusterNodePool[] _nodes = nodes;
        internal readonly string _signature = signature;
    }

    private sealed class Definition(SharpConnectionOptions master, SharpConnectionOptions[] replicas)
    {
        internal readonly SharpConnectionOptions _master = master;
        internal readonly SharpConnectionOptions[] _replicas = replicas;
    }
}
