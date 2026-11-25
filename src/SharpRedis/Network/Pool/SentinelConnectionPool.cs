using SharpRedis.Commands;
using SharpRedis.Entities;
using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Exceptions;
using SharpRedis.Extensions;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Network.Pool;

internal sealed class SentinelConnectionPool : IReadWriteConnectionPool
{
    private readonly SentinelConnectionOptions _options;
    private readonly CommandDispatcher _sentinelDispatcher;
    private readonly BaseConnection[] _sentinels;
    private readonly ManualResetEventSlim _stop = new(false);
    private readonly Thread _monitor;
    private readonly bool _started;
    private int _disposed;
    private volatile bool _switching;
    private MasterSlaveConnectionPool _current = null!;
    private string _signature = string.Empty;
    private string _masterAddress = string.Empty;
    private long _epoch = -1;
#if MODERN_NET
    public ClientCachingBase? ClientCaching { get; }
    internal SentinelConnectionPool(SentinelConnectionOptions options, ClientCachingBase? clientCaching)
#else
    public ClientCachingBase ClientCaching { get; }
    internal SentinelConnectionPool(SentinelConnectionOptions options, ClientCachingBase clientCaching)
#endif
    {
        this._options = options;
        this.ClientCaching = clientCaching;
        this._sentinels = new BaseConnection[options.Sentinels.Count];
        this._sentinelDispatcher = new(options.BatchCount, options.Encoding);
        this._monitor = new Thread(this.Monitor) { IsBackground = true, Name = "SharpRedis_Sentinel_Heartbeat" };
        try
        {
            this.Refresh();
            if (this._current is null) throw new RedisInitializationException("No reachable Sentinel supplied a verified master for " + options.MasterName);
            this._monitor.Start();
            this._started = true;
        }
        catch { this.Dispose(); throw; }
    }

    private MasterSlaveConnectionPool Current
    {
        get
        {
#if NET7_0_OR_GREATER
            ObjectDisposedException.ThrowIf(Volatile.Read(ref this._disposed) != 0, this);
#else
            if (Volatile.Read(ref this._disposed) != 0) throw new ObjectDisposedException(nameof(SentinelConnectionPool));
#endif
            if (this._switching) throw new RedisInitializationException("Sentinel topology is changing; retry after failover completes.");
            return Volatile.Read(ref this._current);
        }
    }

    public BaseConnection GetMasterConnection()
        => this.Current.GetMasterConnection();

    public BaseConnection GetSlaveConnection()
        => this.GetSlaveConnection(requireSlave: false);

    public BaseConnection GetSlaveConnection(bool requireSlave)
        => this._options.EnableReadWriteSeparation ? this.Current.GetSlaveConnection(requireSlave) : this.Current.GetMasterConnection();

    public BaseConnection GetSubConnection()
        => this.Current.GetSubConnection();

    public BaseConnection CreateMasterConnection()
        => this.Current.CreateMasterConnection();

    public BaseConnection CreateSlaveConnection()
        => this._options.EnableReadWriteSeparation ? this.Current.CreateSlaveConnection() : this.Current.CreateMasterConnection();

    public BaseConnection CreateSubConnection()
        => this.Current.CreateSubConnection();

    public BaseConnection GetTempConnection(int database)
        => this.Current.GetTempConnection(database);

    public void ReleaseTempConnection(TempConnection connection)
        => Volatile.Read(ref this._current).ReleaseTempConnection(connection);

    public bool ShouldCache(ICommand command)
        => this.Current.ShouldCache(command);

    public void RetryTracking()
        => this.Current.RetryTracking();

    public bool IsCacheReady(BaseConnection connection, long redirectId)
        => Volatile.Read(ref this._disposed) == 0 && !this._switching
            && Volatile.Read(ref this._current) is { } pool && pool.IsCacheReady(connection, redirectId);

    private void Monitor()
    {
        while (!this._stop.Wait(this._options.SentinelRefreshInterval))
        {
            try { this.Refresh(); }
            catch (Exception ex) { SharpConsole.WriteWarning("Sentinel discovery: " + ex.Message); }
        }
    }

    private void Refresh()
    {
        MasterSlaveConnectionOptions best = null!;
        string bestIdentity = string.Empty;
        long bestEpoch = -1;
        for (int i = 0; i < this._sentinels.Length && Volatile.Read(ref this._disposed) == 0; i++)
        {
            try
            {
                var sentinel = this.GetSentinel(i);
                Query(sentinel, this._options.SentinelCommandTimeout, "PING");
                var master = SentinelConnectionPool.Fields(Query(sentinel, this._options.SentinelCommandTimeout, "SENTINEL", "MASTER", this._options.MasterName));
                if (master["flags"].Contains("failover-in-progress", StringComparison.OrdinalIgnoreCase)) continue;

                long epoch = long.Parse(master["config-epoch"], CultureInfo.InvariantCulture);
                if (epoch < this._epoch || epoch < bestEpoch) continue;
                if (SentinelConnectionPool.Query(
                    sentinel,
                    this._options.SentinelCommandTimeout,
                    "SENTINEL",
                    "get-master-addr-by-name",
                    this._options.MasterName).RawValue is not RedisResult[] { Length: 2 } address)
                {
                    continue;
                }

                string host = address[0]!;
                int port = address[1];
                if (master["ip"] != host || master["port"] != port.ToString(CultureInfo.InvariantCulture)) continue;

                string identity = host + ":" + port.ToString(CultureInfo.InvariantCulture);
                if (epoch == this._epoch && this._masterAddress != identity) continue;
                var candidate = this.CreateOptions(host, port);
                if (this._options.EnableReadWriteSeparation)
                {
                    var replicas = SentinelConnectionPool.Query(
                        sentinel,
                        this._options.SentinelCommandTimeout,
                        "SENTINEL",
                        "REPLICAS",
                        this._options.MasterName).RawValue as RedisResult[];
                    if (replicas is { Length: > 0 })
                    {
                        foreach (var replica in replicas)
                        {
                            var fields = Fields(replica);
                            if (!fields.TryGetValue("flags", out var flags)
                                || flags.Contains("s_down", StringComparison.OrdinalIgnoreCase)
                                || flags.Contains("o_down", StringComparison.OrdinalIgnoreCase)
                                || flags.Contains("disconnected", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                            if (fields["ip"] == host && fields["port"] == port.ToString(CultureInfo.InvariantCulture)) continue;
                            var replicaPort = int.Parse(fields["port"], CultureInfo.InvariantCulture);
                            candidate.Slaves.Add(new StandaloneConnectionOptions
                            {
#if MODERN_NET
                                Host = this.GetDataHost(fields["ip"], replicaPort),
#else
                                Host = fields["ip"],
#endif
                                Port = replicaPort,
                                User = this._options.User,
                                Password = this._options.Password,
                            });
                        }
                    }
                    candidate.Slaves.Sort(static (a, b) =>
                    {
                        int order = string.CompareOrdinal(a.Host, b.Host);
                        return order != 0 ? order : a.Port.CompareTo(b.Port);
                    });
                }
                if (best is null || epoch > bestEpoch)
                {
                    best = candidate;
                    bestIdentity = identity;
                    bestEpoch = epoch;
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref this._sentinels[i], null!)?.Dispose();
                SharpConsole.WriteWarning("Sentinel endpoint unavailable: " + ex.Message);
            }
        }
        if (Volatile.Read(ref this._disposed) != 0) return;
        if (best is null)
        {
            if (this._current is null) throw new RedisInitializationException("Sentinel is unreachable or the master name is unknown.");
            return; // Keep the known topology when discovery alone is unavailable.
        }
        string signature = bestIdentity + "|" + best.Host;
        foreach (var slave in best.Slaves) signature += "|" + slave.Host + ":" + slave.Port;
        if (!this._switching && signature == this._signature) { return; }

        // Validate before retiring working connections; Sentinel may still report stale roles.
        using (var probe = this.CreateProbe(best))
        {
            SentinelConnectionPool.VerifyRole(probe, true, best.CommandTimeout);
        }
        this._switching = true;
        this.ClientCaching?.ClearCache();
        var previous = this._current;
        previous?.SuspendForReplacement();
        MasterSlaveConnectionPool replacement = null!;
        try
        {
            replacement = new(best, this.ClientCaching, ownsCache: false, verifyRoles: true);
            if (previous is not null) replacement.RestoreSubscriptionsFrom(previous);
#if NET7_0_OR_GREATER
            ObjectDisposedException.ThrowIf(Volatile.Read(ref this._disposed) != 0, this);
#else
            if (Volatile.Read(ref this._disposed) != 0) throw new ObjectDisposedException(nameof(SentinelConnectionPool));
#endif
            this.ClientCaching?.ClearCache();
            Volatile.Write(ref this._current, replacement);
            this._signature = signature;
            this._masterAddress = bestIdentity;
            this._epoch = bestEpoch;
            this._switching = false;
        }
        catch
        {
            replacement?.Dispose();
            throw;
        }
        previous?.Dispose();
    }

    private BaseConnection GetSentinel(int index)
    {
        if (this._sentinels[index] is { Available: true } existing) return existing;

        Interlocked.Exchange(ref this._sentinels[index], null!)?.Dispose();
        var endpoint = this._options.Sentinels[index] ?? throw new ArgumentException("Sentinel endpoint cannot be null.");

        var options = new SharpConnectionOptions(
            endpoint.Host,
            endpoint.Port,
            Resp.RESP2,
            endpoint.User,
            endpoint.Password,
            this._options.ConnectName,
            null,
            null,
            endpoint.AddressFamily,
            this._options.BatchCount,
            this._options.BuffSize,
            0,
            this._options.SentinelCommandTimeout,
            this._options.Encoding,
            SharpConnectionOptions.TlsOptions.Create(endpoint));
        var connection = new StandaloneConnection(options, this._sentinelDispatcher, this._options.SentinelCommandTimeout);
        this._sentinels[index] = connection;
        try
        {
#if NET7_0_OR_GREATER
            ObjectDisposedException.ThrowIf(Volatile.Read(ref this._disposed) != 0, this);
#else
            if (Volatile.Read(ref this._disposed) != 0) throw new ObjectDisposedException(nameof(SentinelConnectionPool));
#endif
            if (!string.IsNullOrEmpty(endpoint.User) || !string.IsNullOrEmpty(endpoint.Password))
            {
                using var auth = ConnectionCommands.Auth(endpoint.User, endpoint.Password);
                _ = SentinelConnectionPool.Execute(connection, auth, this._options.SentinelCommandTimeout);
            }
            // Sentinel has no SELECT command. Do not use Redis data-node initialization here.
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MasterSlaveConnectionOptions CreateOptions(string host, int port)
    {
        var options = new MasterSlaveConnectionOptions
        {
#if MODERN_NET
            Host = this.GetDataHost(host, port),
#else
            Host = host,
#endif
            Port = port,
            User = this._options.User,
            Password = this._options.Password,
            Database = this._options.Database,
            RespVersion = this._options.RespVersion,
            Encoding = this._options.Encoding,
            Prefix = this._options.Prefix,
            Suffix = this._options.Suffix,
            ConnectName = this._options.ConnectName,
            BatchCount = this._options.BatchCount,
            BuffSize = this._options.BuffSize,
            CommandTimeout = this._options.CommandTimeout,
            HeartbeatInterval = this._options.HeartbeatInterval,
            ReconnectInterval = this._options.ReconnectInterval,
            SlaveSelectionMode = this._options.SlaveSelectionMode,
            Ssl = this._options.Ssl,
            CheckCertificateRevocation = this._options.CheckCertificateRevocation,
            SslHost = this._options.SslHost,
            CertificateValidation = this._options.CertificateValidation,
            CertificateSelection = this._options.CertificateSelection,
        };
        if (options.Ssl && this._options.ConfiguredCertificates is { Count: > 0 } certificates)
            options.ClientCertificates.AddRange(certificates);
        return options;
    }

#if MODERN_NET
    private string GetDataHost(string host, int port)
    {
        if (BasalConnectionOptions.IsUnixSocketHost(host.Trim()))
            throw new RedisInitializationException("Invalid Sentinel data-node host.");
        if (this._options.ConfiguredUnixSockets is { } mappings && mappings.TryGetValue((host, port), out var path))
        {
            if (!BasalConnectionOptions.IsUnixSocketHost(path))
                throw new ArgumentException("A Sentinel data-node socket mapping requires an absolute path.");
            return path;
        }
        return port == this._options.Port && this._options.AddressFamily == System.Net.Sockets.AddressFamily.Unix
            && host == "127.0.0.1" ? this._options.Host : host;
    }
#endif

    private StandaloneConnection CreateProbe(MasterSlaveConnectionOptions options)
    {
        var settings = new SharpConnectionOptions(
            host: options.Host,
            port: options.Port,
            resp: options.RespVersion,
            user: options.User,
            password: options.Password,
            connectName: options.ConnectName,
            prefix: null,
            suffix: null,
            addressFamily: options.AddressFamily,
            batchCount: options.BatchCount,
            buffSize: options.BuffSize,
            database: options.Database,
            commandTimeout: options.CommandTimeout,
            encoding: options.Encoding,
            tls: SharpConnectionOptions.TlsOptions.Create(options));
        var connection = new StandaloneConnection(settings, this._sentinelDispatcher, options.CommandTimeout);
        try
        {
            if (!connection.Authentication(options.User, options.Password, options.RespVersion, options.Database, options.CommandTimeout))
                throw new RedisInitializationException("Redis node authentication failed.");
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void VerifyRole(BaseConnection connection, bool master, int timeout)
    {
        if (SentinelConnectionPool.Query(
            connection, timeout, "ROLE").RawValue is not RedisResult[] values || values.Length == 0 || values[0]! != (master ? "master" : "slave"))
        {
            throw new RedisInitializationException("Redis endpoint role does not match the discovered topology.");
        }
    }

    private static RedisResult Query(BaseConnection connection, int timeout, params string[] arguments)
    {
        using var command = new CommandPacket(new SharpValue(arguments[0], CommandItemCategory.Command), CommandMode.ConnectionManagement);
        for (int i = 1; i < arguments.Length; i++)
            command.WriteArguments(arguments[i]);
        return SentinelConnectionPool.Execute(connection, command, timeout);
    }

    private static RedisResult Execute(BaseConnection connection, ICommand command, int timeout)
    {
        var completion = new TaskCompletionSource<RedisResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, completion, timeout);
        if (!completion.WaitForCompletion(timeout)) throw new RedisInitializationException("Sentinel discovery command timed out.");
        var result = completion.Task.GetAwaiter().GetResult();
        if (result.Type is RespType.Exception or RespType.SimpleErrors or RespType.BulkErrors)
            throw new RedisInitializationException("Sentinel discovery failed: " + result.RawValue);
        return result;
    }

    private static Dictionary<string, string> Fields(RedisResult result)
    {
        var fields = new Dictionary<string, string>(StringComparer.Ordinal);
        if (result.RawValue is not RedisResult[] values) throw new RedisInitializationException("Invalid Sentinel response.");
        for (int i = 0; i + 1 < values.Length; i += 2) fields[(string)values[i]!] = (string)values[i + 1]!;
        return fields;
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref this._disposed, 1) != 0) return;
        this._stop.Set();
        foreach (var sentinel in this._sentinels) sentinel?.Dispose();
        this._sentinelDispatcher.Dispose();
        if (this._started) this._monitor.Join();
        foreach (var sentinel in this._sentinels) sentinel?.Dispose();
        this._current?.Dispose();
        this.ClientCaching?.Dispose();
        this._stop.Dispose();
    }
}
