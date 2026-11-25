using System;
using System.Collections.Generic;

namespace SharpRedis;

/// <summary>
/// Redis Cluster configuration.
/// <para>Redis集群配置</para>
/// </summary>
public sealed class ClusterConnectionOptions : BasalConnectionOptions
{
    /// <summary>
    /// Discovery seeds; the client discovers the remaining nodes.
    /// <para>集群发现入口节点。显式认证信息必须一致；未填写的认证信息继承统一配置。</para>
    /// </summary>
    public List<StandaloneConnectionOptions> Nodes { get; }

#if MODERN_NET
    /// <summary>Local socket paths keyed by the address and port announced by Cluster nodes.</summary>
    public Dictionary<(string Host, int Port), string> NodeUnixSockets => this.ConfiguredUnixSockets ??= [with(SharpRedis.Extensions.EndpointComparer._instance)];

    internal Dictionary<(string Host, int Port), string>? ConfiguredUnixSockets { get; private set; }
#endif

    /// <summary>
    /// Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </summary>
    public bool EnableReadWriteSeparation { get; internal set; }

    /// <summary>
    /// Slave selection strategy, round-robin by default.
    /// <para>从节点(读节点)选择策略，默认轮询。</para>
    /// </summary>
    public SlaveSelectionMode SlaveSelectionMode { get; set; } = SlaveSelectionMode.RoundRobin;

    /// <summary>
    /// Topology refresh interval in milliseconds.
    /// <para>拓扑刷新周期（毫秒）</para>
    /// </summary>
    public int ClusterRefreshInterval { get; set; } = 1000;

    /// <summary>
    /// Maximum MOVED/ASK redirects per command.
    /// <para>单条命令的重定向次数上限</para>
    /// </summary>
    public int MaxRedirects { get; set; } = 5;

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

    // Validate only at registration, after the callback has finished configuring all seeds.
    // Discovered nodes and redirected connections also use this single credential pair.
    internal void ValidateCredentials()
    {
        var user = this.User;
        var password = this.Password;
        for (int i = 0; i < this.Nodes.Count; i++)
        {
            var node = this.Nodes[i] ?? throw new ArgumentException($"Cluster seed Nodes[{i}] cannot be null.", nameof(this.Nodes));
            if (!string.IsNullOrEmpty(node.User))
            {
                if (string.IsNullOrEmpty(user)) user = node.User;
                else if (!string.Equals(user, node.User, StringComparison.Ordinal))
                    throw new ArgumentException($"Nodes[{i}].User conflicts with the shared Cluster credentials. All explicitly configured usernames must match.", nameof(this.Nodes));
            }
            if (!string.IsNullOrEmpty(node.Password))
            {
                if (string.IsNullOrEmpty(password)) password = node.Password;
                else if (!string.Equals(password, node.Password, StringComparison.Ordinal))
                    throw new ArgumentException($"Nodes[{i}].Password conflicts with the shared Cluster credentials. All explicitly configured passwords must match.", nameof(this.Nodes));
            }
        }
        this.User = user;
        this.Password = password;
    }

    public ClusterConnectionOptions()
    {
        this.Nodes = [];
    }

    public ClusterConnectionOptions(string[] nodeConnectionStrings)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(nodeConnectionStrings, nameof(nodeConnectionStrings));
#else
        if (nodeConnectionStrings is null) throw new ArgumentNullException(nameof(nodeConnectionStrings));
#endif

        this.Nodes = [with(nodeConnectionStrings.Length)];
        foreach (var nodeConnectionString in nodeConnectionStrings)
        {
            this.Nodes.Add(new(nodeConnectionString));
        }
        if (this.Nodes.Count > 0)
        {
            this.User = this.Nodes[0].User;
            this.Password = this.Nodes[0].Password;
        }
    }
}
