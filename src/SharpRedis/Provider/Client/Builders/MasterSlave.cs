using SharpRedis.Pipelines;
using System;

namespace SharpRedis;

public sealed partial class Redis
{
    /// <summary>
    /// Connect to a fixed master and one or more slaves. This does not configure Redis replication or promote slaves.
    /// <para>连接已配置复制关系的一主多从节点，不负责配置复制或自动提升从节点。</para>
    /// </summary>
    public static Redis UseMasterSlave(Action<MasterSlaveConnectionOptions> optionsAction)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(optionsAction, nameof(optionsAction));
#else
        if (optionsAction is null) throw new ArgumentNullException(nameof(optionsAction));
#endif
        var options = new MasterSlaveConnectionOptions();
        optionsAction.Invoke(options);
        return Redis.BuildMasterSlave(options);
    }

    /// <summary>
    /// Connect using separate master and slave connection strings.
    /// <para>使用连接字符串连接到主从读写分离模式的Redis</para>
    /// </summary>
    /// <param name="masterConnectionString">Master node connection string, for example: host=127.0.0.1,port=10086,password=123456
    /// <para>主节点连接字符串, 如: host=127.0.0.1,port=10086,password=123456</para>
    /// </param>
    /// <param name="slaveConnectionStrings">Array of connection strings for Redis slave nodes
    /// <para>从节点连接字符串数组</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
#if MODERN_NET
    public static Redis UseMasterSlave(string masterConnectionString, string[] slaveConnectionStrings, Action<MasterSlaveConnectionOptions>? optionsAction = null)
#else
    public static Redis UseMasterSlave(string masterConnectionString, string[] slaveConnectionStrings, Action<MasterSlaveConnectionOptions> optionsAction = null)
#endif
    {
        var options = new MasterSlaveConnectionOptions(masterConnectionString, slaveConnectionStrings);
        optionsAction?.Invoke(options);
        return Redis.BuildMasterSlave(options);
    }

    private static Redis BuildMasterSlave(MasterSlaveConnectionOptions options)
    {
        if (options.Slaves.Count == 0) throw new ArgumentException("At least one slave endpoint is required.");
        if (options.SlaveSelectionMode is not (SlaveSelectionMode.RoundRobin or SlaveSelectionMode.Random))
            throw new ArgumentOutOfRangeException(nameof(options.SlaveSelectionMode));
        var context = new RedisContext(RedisServerMode.MasterSlave);
        try
        {
            options.Build(context);
            return new Redis(context);
        }
        catch
        {
            context.Dispose();
            throw;
        }
    }
}
