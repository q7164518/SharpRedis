using SharpRedis.Extensions;
using System;
using System.Collections.Generic;

namespace SharpRedis;

/// <summary>
/// Master/slave Redis connection configuration. Host and Port identify the master.
/// <para>读写分离配置。Host、Port、User、Password 为主节点配置。</para>
/// </summary>
public sealed class MasterSlaveConnectionOptions : BasalConnectionOptions
{
    /// <summary>
    /// Slave endpoints. Only Host, Port, User and Password are used from each entry.
    /// <para>从节点列表；各节点仅使用地址、端口和认证配置，其余参数统一使用当前配置。</para>
    /// </summary>
    public List<StandaloneConnectionOptions> Slaves { get; }

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

    public MasterSlaveConnectionOptions()
    {
        this.Slaves = [];
    }

    /// <summary>
    /// Create a master-slave read-write separation configuration based on the connection string.
    /// <para>根据连接字符串创建主从读写分离配置</para>
    /// </summary>
    /// <param name="masterConnectionString">Master node connection string, for example: host=127.0.0.1,port=10086,password=123456
    /// <para>主节点连接字符串, 如: host=127.0.0.1,port=10086,password=123456</para>
    /// </param>
    /// <param name="slaveConnectionStrings">Array of connection strings for Redis slave nodes
    /// <para>从节点连接字符串数组</para>
    /// </param>
    public MasterSlaveConnectionOptions(string masterConnectionString, string[] slaveConnectionStrings)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(masterConnectionString, nameof(masterConnectionString));
        ArgumentNullException.ThrowIfNull(slaveConnectionStrings, nameof(slaveConnectionStrings));
#else
        if (masterConnectionString is null) throw new ArgumentNullException(nameof(masterConnectionString));
        if (slaveConnectionStrings is null) throw new ArgumentNullException(nameof(slaveConnectionStrings));
#endif

        var masterOptions = Utilities.ConnectionToDictionary(masterConnectionString);
        base.Init(masterOptions);
        if (masterOptions.TryGetValue("password", out var password)) this.Password = password;
        if (masterOptions.TryGetValue("user", out var user)) this.User = user;

        this.Slaves = [with(slaveConnectionStrings.Length)];
        foreach (var slaveConnectionString in slaveConnectionStrings)
        {
            this.Slaves.Add(new(slaveConnectionString));
        }
    }
}
