using SharpRedis.Pipelines;
using System;

namespace SharpRedis;

public sealed partial class Redis
{
    /// <summary>
    /// Connect through Sentinel discovery and follow master failover.
    /// <para>连接到哨兵模式的Redis</para>
    /// </summary>
    /// <param name="enableReadWriteSeparation">Whether to read from replicas. When false, all operations use the master.
    /// <para>是否开启读写分离；false 时不连接从节点，所有操作使用主节点，并自动跟随主节点切换。</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <returns></returns>
    public static Redis UseSentinel(bool enableReadWriteSeparation, Action<SentinelConnectionOptions> optionsAction)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(optionsAction, nameof(optionsAction));
#else
        if (optionsAction is null) throw new ArgumentNullException(nameof(optionsAction));
#endif
        var options = new SentinelConnectionOptions { EnableReadWriteSeparation = enableReadWriteSeparation };
        optionsAction.Invoke(options);
        return Redis.BuildSentinel(options);
    }

    /// <summary>
    /// Connect using a monitored master name and separate Sentinel connection strings.
    /// <para>连接到哨兵模式的Redis</para>
    /// </summary>
    /// <param name="enableReadWriteSeparation">Whether to read from replicas. When false, all operations use the master.
    /// <para>是否开启读写分离；false 时不连接从节点，所有操作使用主节点，并自动跟随主节点切换。</para>
    /// </param>
    /// <param name="masterConnectionString">Master node connection string, for example: mastername=mymaster, password=123456
    /// <para>主节点连接字符串, 如: mastername=mymaster,password=123456</para>
    /// </param>
    /// <param name="sentinelConnectionStrings">Sentinel endpoints. Only Host, Port, User and Password are used from each entry.
    /// <para>哨兵列表连接字符串；各节点分别使用地址、端口和认证配置。</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
#if MODERN_NET
    public static Redis UseSentinel(bool enableReadWriteSeparation, string masterConnectionString, string[] sentinelConnectionStrings, Action<SentinelConnectionOptions>? optionsAction = null)
#else
    public static Redis UseSentinel(bool enableReadWriteSeparation, string masterConnectionString, string[] sentinelConnectionStrings, Action<SentinelConnectionOptions> optionsAction = null)
#endif
    {
        var options = new SentinelConnectionOptions(masterConnectionString, sentinelConnectionStrings)
        {
            EnableReadWriteSeparation = enableReadWriteSeparation,
        };
        optionsAction?.Invoke(options);
        return Redis.BuildSentinel(options);
    }

    private static Redis BuildSentinel(SentinelConnectionOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.MasterName)) throw new ArgumentException("MasterName is required.");
        if (options.Sentinels.Count == 0) throw new ArgumentException("At least one Sentinel endpoint is required.");
        if (options.SentinelRefreshInterval <= 0 || options.SentinelCommandTimeout <= 0)
            throw new ArgumentOutOfRangeException(nameof(options.SentinelRefreshInterval));
        if (options.EnableReadWriteSeparation && options.SlaveSelectionMode is not (SlaveSelectionMode.RoundRobin or SlaveSelectionMode.Random))
            throw new ArgumentOutOfRangeException(nameof(options.SlaveSelectionMode));
        var context = new RedisContext(RedisServerMode.Sentinel);
        try
        {
            options.Build(context);
            return new Redis(context);
        }
        catch
        {
            context.Dispose(); throw;
        }
    }
}
