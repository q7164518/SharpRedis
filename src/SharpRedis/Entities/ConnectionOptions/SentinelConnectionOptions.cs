using SharpRedis.Extensions;
using System;
using System.Collections.Generic;

namespace SharpRedis;

/// <summary>
/// Redis Sentinel configuration. Redis data-node credentials are separate from Sentinel credentials.
/// <para>哨兵配置。User、Password 用于 Redis 数据节点；Sentinels 中分别配置哨兵认证。</para>
/// </summary>
public sealed class SentinelConnectionOptions : BasalConnectionOptions
{
    /// <summary>
    /// Whether replica reads are enabled. Configured by Redis.UseSentinel.
    /// <para>是否开启读写分离，由 Redis.UseSentinel 的第一个参数指定。</para>
    /// </summary>
    public bool EnableReadWriteSeparation { get; internal set; }

    /// <summary>
    /// Sentinel endpoints. Only Host, Port, User and Password are used from each entry.
    /// <para>哨兵列表；各节点分别使用地址、端口和认证配置。</para>
    /// </summary>
    public List<StandaloneConnectionOptions> Sentinels { get; }

#if MODERN_NET
    /// <summary>Local socket paths keyed by the data node address and port reported by Sentinel.</summary>
    public Dictionary<(string Host, int Port), string> DataNodeUnixSockets => this.ConfiguredUnixSockets ??= [with(EndpointComparer._instance)];

    internal Dictionary<(string Host, int Port), string>? ConfiguredUnixSockets { get; private set; }
#endif

    /// <summary>
    /// Sentinel monitored master name.
    /// <para>哨兵监控的主节点组名称</para>
    /// </summary>
    public string MasterName { get; set; } = "mymaster";

    /// <summary>
    /// Topology refresh and Sentinel heartbeat interval, milliseconds.
    /// <para>拓扑刷新和 Sentinel 心跳间隔，单位为毫秒。</para>
    /// </summary>
    public int SentinelRefreshInterval { get; set; } = 1000;

    /// <summary>
    /// Timeout for Sentinel queries, milliseconds.
    /// <para>哨兵查询的超时时间，单位: 毫秒</para>
    /// </summary>
    public int SentinelCommandTimeout { get; set; } = 1000;

    /// <summary>
    /// Slave selection strategy, round-robin by default.
    /// <para>从节点选择策略，默认轮询。</para>
    /// </summary>
    public SlaveSelectionMode SlaveSelectionMode { get; set; } = SlaveSelectionMode.RoundRobin;

#if MODERN_NET
    /// <summary>
    /// Redis server password, No password requires no assignment
    /// <para>Redis主机密码, 没有密码不需要设置</para>
    /// </summary>
    public string? Password
    {
        get;

        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }

    /// <summary>
    /// <para>Redis server user, default is null. Available since: 6.0.0+</para>
    /// <para>Redis主机用户名, Redis 6+以上才支持, 默认为空</para>
    /// </summary>
    public string? User
    {
        get;

        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }
#else
    /// <summary>
    /// Redis server password, No password requires no assignment
    /// <para>Redis主机密码, 没有密码不需要设置</para>
    /// </summary>
    public string Password
    {
        get;

        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }

    /// <summary>
    /// <para>Redis server user, default is null. Available since: 6.0.0+</para>
    /// <para>Redis主机用户名, Redis 6+以上才支持, 默认为空</para>
    /// </summary>
    public string User
    {
        get;

        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }
#endif

    public SentinelConnectionOptions()
    {
        this.Sentinels = [];
    }

    /// <summary>
    /// Create sentinel mode configuration based on the connection string
    /// <para>根据连接字符串创建哨兵模式配置</para>
    /// </summary>
    /// <param name="masterConnectionString">Master node connection string, for example: mastername=mymaster, password=123456
    /// <para>主节点连接字符串, 如: mastername=mymaster,password=123456</para>
    /// </param>
    /// <param name="sentinelConnectionStrings">Sentinel endpoints. Only Host, Port, User and Password are used from each entry.
    /// <para>哨兵列表连接字符串；各节点分别使用地址、端口和认证配置。</para>
    /// </param>
    public SentinelConnectionOptions(string masterConnectionString, string[] sentinelConnectionStrings)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(masterConnectionString, nameof(masterConnectionString));
        ArgumentNullException.ThrowIfNull(sentinelConnectionStrings, nameof(sentinelConnectionStrings));
#else
        if (masterConnectionString is null) throw new ArgumentNullException(nameof(masterConnectionString));
        if (sentinelConnectionStrings is null) throw new ArgumentNullException(nameof(sentinelConnectionStrings));
#endif

        var masterOptions = Utilities.ConnectionToDictionary(masterConnectionString);
        base.Init(masterOptions);
        if (masterOptions.TryGetValue("password", out var password)) this.Password = password;
        if (masterOptions.TryGetValue("user", out var user)) this.User = user;
        if (masterOptions.TryGetValue("mastername", out var masterName)) this.MasterName = masterName;

        this.Sentinels = [with(sentinelConnectionStrings.Length)];
        foreach (var sentinelConnectionString in sentinelConnectionStrings)
        {
            this.Sentinels.Add(new(sentinelConnectionString));
        }
    }
}
