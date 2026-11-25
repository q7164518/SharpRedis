using SharpRedis.Pipelines;
using System;

namespace SharpRedis;

public sealed partial class Redis
{
    /// <summary>
    /// Connect to the Redis cluster
    /// <para>连接到Redis集群</para>
    /// </summary>
    /// <param name="enableReadWriteSeparation">Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <returns></returns>
    public static Redis UseCluster(bool enableReadWriteSeparation, Action<ClusterConnectionOptions> optionsAction)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(optionsAction, nameof(optionsAction));
#else
        if (optionsAction is null) throw new ArgumentNullException(nameof(optionsAction));
#endif
        var options = new ClusterConnectionOptions
        {
            EnableReadWriteSeparation = enableReadWriteSeparation,
        };
        optionsAction(options);
        return Redis.BuildCluster(options);
    }

    /// <summary>
    /// Connect to the Redis cluster
    /// <para>连接到Redis集群</para>
    /// </summary>
    /// <param name="enableReadWriteSeparation">Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </param>
    /// <param name="nodeConnectionStrings">Array of connection strings for the slot master node
    /// <para>插槽主节点连接字符串数组</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <returns></returns>
#if MODERN_NET
    public static Redis UseCluster(bool enableReadWriteSeparation, string[] nodeConnectionStrings, Action<ClusterConnectionOptions>? optionsAction = null)
#else
    public static Redis UseCluster(bool enableReadWriteSeparation, string[] nodeConnectionStrings, Action<ClusterConnectionOptions> optionsAction = null)
#endif
    {
        var options = new ClusterConnectionOptions(nodeConnectionStrings)
        {
            EnableReadWriteSeparation = enableReadWriteSeparation,
        };
        optionsAction?.Invoke(options);
        return Redis.BuildCluster(options);
    }

    /// <summary>
    /// Connect to the Redis cluster
    /// <para>连接到Redis集群</para>
    /// </summary>
    /// <param name="enableReadWriteSeparation">Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </param>
    /// <param name="nodeConnectionString">Connection string for any master node in the Redis Cluster. SharpRedis will automatically discover the other cluster nodes.
    /// <para>任意一个插槽主节点的连接字符串, ShapRedis会自动发现其它插槽节点</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <returns></returns>
#if MODERN_NET
    public static Redis UseCluster(bool enableReadWriteSeparation, string nodeConnectionString, Action<ClusterConnectionOptions>? optionsAction = null)
#else
    public static Redis UseCluster(bool enableReadWriteSeparation, string nodeConnectionString, Action<ClusterConnectionOptions> optionsAction = null)
#endif
    {
        var options = new ClusterConnectionOptions([nodeConnectionString])
        {
            EnableReadWriteSeparation = enableReadWriteSeparation,
        };
        optionsAction?.Invoke(options);
        return Redis.BuildCluster(options);
    }

    private static Redis BuildCluster(ClusterConnectionOptions options)
    {
#if MODERN_NET
        if (options.AddressFamily == System.Net.Sockets.AddressFamily.Unix
            && !options.Nodes.Exists(node => node?.Host == options.Host && node.Port == options.Port))
            options.Nodes.Insert(0, new() { Host = options.Host, Port = options.Port });
#endif
        if (options.Nodes.Count == 0) throw new ArgumentException("At least one Cluster seed is required.");
        if (options.Database != 0) throw new NotSupportedException("Redis Cluster only supports database 0.");
        if (options.ClusterRefreshInterval <= 0 || options.CommandTimeout <= 0 || options.MaxRedirects < 0)
            throw new ArgumentOutOfRangeException(nameof(options.ClusterRefreshInterval));
        if (options.EnableReadWriteSeparation && options.SlaveSelectionMode is not (SlaveSelectionMode.RoundRobin or SlaveSelectionMode.Random))
            throw new ArgumentOutOfRangeException(nameof(options.SlaveSelectionMode));
        options.ValidateCredentials();
        var context = new RedisContext(RedisServerMode.Cluster);
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
