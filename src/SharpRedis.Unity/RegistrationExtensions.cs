#nullable enable
using SharpRedis.Provider.Standard;
using System;
using Unity;
using Unity.Lifetime;

namespace SharpRedis.Unity;

/// <summary>
/// Registers SharpRedis clients and their command groups with a Unity container.
/// <para>将 SharpRedis 客户端及其命令组注册到 Unity 容器。</para>
/// </summary>
/// <remarks>
/// The registering container owns each Redis client. Command groups use the same registration name as their Redis client.
/// <para>Redis 客户端由注册它的容器释放。命令组与所属 Redis 客户端使用相同的注册名称。</para>
/// </remarks>
public static class RegistrationExtensions
{
    #region Standalone
    /// <summary>
    /// Connects to Redis in standalone mode and registers the client as a singleton.
    /// <para>连接单机模式的 Redis，并将客户端注册为单例。</para>
    /// </summary>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="connectionString">The Redis connection string.
    /// <para>Redis 连接字符串。</para>
    /// </param>
    /// <param name="optionsAction">An optional callback to configure the connection.
    /// <para>用于配置连接的可选回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisStandalone(
        this IUnityContainer container,
        string connectionString,
        Action<StandaloneConnectionOptions>? optionsAction = null,
        string? serviceName = null)
    {
        return RegisterClient(container, serviceName, () => Redis.UseStandalone(connectionString, optionsAction));
    }

    /// <summary>
    /// Registers a standalone Redis singleton with a client-side cache created on first resolution.
    /// <para>注册单机 Redis 单例，首次解析时创建客户端和本地缓存。需要 Redis 6.0 或更新版本。</para>
    /// </summary>
    /// <remarks>Client-side caching requires Redis 6.0 or later.</remarks>
    /// <typeparam name="TCache">The cache implementation constructed by Unity.
    /// <para>由 Unity 创建的本地缓存实现类型。</para>
    /// </typeparam>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="connectionString">The Redis connection string.
    /// <para>Redis 连接字符串。</para>
    /// </param>
    /// <param name="optionsAction">An optional callback to configure the connection.
    /// <para>用于配置连接的可选回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisStandalone<TCache>(
        this IUnityContainer container,
        string connectionString,
        Action<StandaloneConnectionOptions>? optionsAction = null,
        string? serviceName = null)
        where TCache : ClientCachingBase
    {
        return RegisterCachedClient<TCache, StandaloneConnectionOptions>(
            container,
            serviceName,
            configure => Redis.UseStandalone(connectionString, configure),
            optionsAction);
    }

    /// <summary>
    /// Connects to Redis in standalone mode and registers the client as a singleton.
    /// <para>连接单机模式的 Redis，并将客户端注册为单例。</para>
    /// </summary>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="optionsAction">The callback that configures the connection.
    /// <para>用于配置连接的回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisStandalone(
        this IUnityContainer container,
        Action<StandaloneConnectionOptions> optionsAction,
        string? serviceName = null)
    {
        return RegisterClient(container, serviceName, () => Redis.UseStandalone(optionsAction));
    }

    /// <summary>
    /// Registers a standalone Redis singleton with a client-side cache created on first resolution.
    /// <para>注册单机 Redis 单例，首次解析时创建客户端和本地缓存。需要 Redis 6.0 或更新版本。</para>
    /// </summary>
    /// <remarks>Client-side caching requires Redis 6.0 or later.</remarks>
    /// <typeparam name="TCache">The cache implementation constructed by Unity.
    /// <para>由 Unity 创建的本地缓存实现类型。</para>
    /// </typeparam>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="optionsAction">The callback that configures the connection.
    /// <para>用于配置连接的回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisStandalone<TCache>(
        this IUnityContainer container,
        Action<StandaloneConnectionOptions> optionsAction,
        string? serviceName = null)
        where TCache : ClientCachingBase
    {
        return RegisterCachedClient<TCache, StandaloneConnectionOptions>(
            container,
            serviceName,
            configure => Redis.UseStandalone(configure),
            optionsAction);
    }
    #endregion

    #region MasterSlave
    /// <summary>
    /// Connects to Redis in primary and replica mode and registers the client as a singleton.
    /// <para>连接主从读写分离模式的 Redis，并将客户端注册为单例。</para>
    /// </summary>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="masterConnectionString">The Redis primary connection string.
    /// <para>Redis 主节点的连接字符串。</para>
    /// </param>
    /// <param name="slaveConnectionStrings">The connection strings for the Redis replicas.
    /// <para>Redis 从节点的连接字符串数组。</para>
    /// </param>
    /// <param name="optionsAction">An optional callback to configure the connection.
    /// <para>用于配置连接的可选回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisMasterSlave(
        this IUnityContainer container,
        string masterConnectionString,
        string[] slaveConnectionStrings,
        Action<MasterSlaveConnectionOptions>? optionsAction = null,
        string? serviceName = null)
    {
        return RegisterClient(container, serviceName, () => Redis.UseMasterSlave(masterConnectionString, slaveConnectionStrings, optionsAction));
    }

    /// <summary>
    /// Registers a primary and replica Redis singleton with a client-side cache created on first resolution.
    /// <para>注册主从读写分离 Redis 单例，首次解析时创建客户端和本地缓存。需要 Redis 6.0 或更新版本。</para>
    /// </summary>
    /// <remarks>Client-side caching requires Redis 6.0 or later.</remarks>
    /// <typeparam name="TCache">The cache implementation constructed by Unity.
    /// <para>由 Unity 创建的本地缓存实现类型。</para>
    /// </typeparam>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="masterConnectionString">The Redis primary connection string.
    /// <para>Redis 主节点的连接字符串。</para>
    /// </param>
    /// <param name="slaveConnectionStrings">The connection strings for the Redis replicas.
    /// <para>Redis 从节点的连接字符串数组。</para>
    /// </param>
    /// <param name="optionsAction">An optional callback to configure the connection.
    /// <para>用于配置连接的可选回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisMasterSlave<TCache>(
        this IUnityContainer container,
        string masterConnectionString,
        string[] slaveConnectionStrings,
        Action<MasterSlaveConnectionOptions>? optionsAction = null,
        string? serviceName = null)
        where TCache : ClientCachingBase
    {
        return RegisterCachedClient<TCache, MasterSlaveConnectionOptions>(
            container,
            serviceName,
            configure => Redis.UseMasterSlave(masterConnectionString, slaveConnectionStrings, configure),
            optionsAction);
    }

    /// <summary>
    /// Connects to Redis in primary and replica mode and registers the client as a singleton.
    /// <para>连接主从读写分离模式的 Redis，并将客户端注册为单例。</para>
    /// </summary>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="optionsAction">The callback that configures the connection.
    /// <para>用于配置连接的回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisMasterSlave(
        this IUnityContainer container,
        Action<MasterSlaveConnectionOptions> optionsAction,
        string? serviceName = null)
    {
        return RegisterClient(container, serviceName, () => Redis.UseMasterSlave(optionsAction));
    }

    /// <summary>
    /// Registers a primary and replica Redis singleton with a client-side cache created on first resolution.
    /// <para>注册主从读写分离 Redis 单例，首次解析时创建客户端和本地缓存。需要 Redis 6.0 或更新版本。</para>
    /// </summary>
    /// <remarks>Client-side caching requires Redis 6.0 or later.</remarks>
    /// <typeparam name="TCache">The cache implementation constructed by Unity.
    /// <para>由 Unity 创建的本地缓存实现类型。</para>
    /// </typeparam>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="optionsAction">The callback that configures the connection.
    /// <para>用于配置连接的回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisMasterSlave<TCache>(
        this IUnityContainer container,
        Action<MasterSlaveConnectionOptions> optionsAction,
        string? serviceName = null)
        where TCache : ClientCachingBase
    {
        return RegisterCachedClient<TCache, MasterSlaveConnectionOptions>(
            container,
            serviceName,
            configure => Redis.UseMasterSlave(configure),
            optionsAction);
    }
    #endregion

    #region Sentinel
    /// <summary>
    /// Connects to Redis in Sentinel mode and registers the client as a singleton.
    /// <para>连接哨兵模式的 Redis，并将客户端注册为单例。</para>
    /// </summary>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="enableReadWriteSeparation">Whether to route eligible reads to replicas.
    /// <para>是否将符合条件的读命令路由到从节点。</para>
    /// </param>
    /// <param name="masterConnectionString">The monitored primary name and Redis data node connection settings.
    /// <para>哨兵监控的主节点名称和 Redis 数据节点连接配置。</para>
    /// </param>
    /// <param name="sentinelConnectionStrings">The connection strings for the Sentinel endpoints.
    /// <para>哨兵节点的连接字符串数组。</para>
    /// </param>
    /// <param name="optionsAction">An optional callback to configure the connection.
    /// <para>用于配置连接的可选回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisSentinel(
        this IUnityContainer container,
        bool enableReadWriteSeparation,
        string masterConnectionString,
        string[] sentinelConnectionStrings,
        Action<SentinelConnectionOptions>? optionsAction = null,
        string? serviceName = null)
    {
        return RegisterClient(container, serviceName, () => Redis.UseSentinel(enableReadWriteSeparation, masterConnectionString, sentinelConnectionStrings, optionsAction));
    }

    /// <summary>
    /// Registers a Sentinel Redis singleton with a client-side cache created on first resolution.
    /// <para>注册哨兵 Redis 单例，首次解析时创建客户端和本地缓存。需要 Redis 6.0 或更新版本。</para>
    /// </summary>
    /// <remarks>Client-side caching requires Redis 6.0 or later.</remarks>
    /// <typeparam name="TCache">The cache implementation constructed by Unity.
    /// <para>由 Unity 创建的本地缓存实现类型。</para>
    /// </typeparam>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="enableReadWriteSeparation">Whether to route eligible reads to replicas.
    /// <para>是否将符合条件的读命令路由到从节点。</para>
    /// </param>
    /// <param name="masterConnectionString">The monitored primary name and Redis data node connection settings.
    /// <para>哨兵监控的主节点名称和 Redis 数据节点连接配置。</para>
    /// </param>
    /// <param name="sentinelConnectionStrings">The connection strings for the Sentinel endpoints.
    /// <para>哨兵节点的连接字符串数组。</para>
    /// </param>
    /// <param name="optionsAction">An optional callback to configure the connection.
    /// <para>用于配置连接的可选回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisSentinel<TCache>(
        this IUnityContainer container,
        bool enableReadWriteSeparation,
        string masterConnectionString,
        string[] sentinelConnectionStrings,
        Action<SentinelConnectionOptions>? optionsAction = null,
        string? serviceName = null)
        where TCache : ClientCachingBase
    {
        return RegisterCachedClient<TCache, SentinelConnectionOptions>(
            container,
            serviceName,
            configure => Redis.UseSentinel(enableReadWriteSeparation, masterConnectionString, sentinelConnectionStrings, configure),
            optionsAction);
    }

    /// <summary>
    /// Connects to Redis in Sentinel mode and registers the client as a singleton.
    /// <para>连接哨兵模式的 Redis，并将客户端注册为单例。</para>
    /// </summary>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="enableReadWriteSeparation">Whether to route eligible reads to replicas.
    /// <para>是否将符合条件的读命令路由到从节点。</para>
    /// </param>
    /// <param name="optionsAction">The callback that configures the connection.
    /// <para>用于配置连接的回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisSentinel(
        this IUnityContainer container,
        bool enableReadWriteSeparation,
        Action<SentinelConnectionOptions> optionsAction,
        string? serviceName = null)
    {
        return RegisterClient(container, serviceName, () => Redis.UseSentinel(enableReadWriteSeparation, optionsAction));
    }

    /// <summary>
    /// Registers a Sentinel Redis singleton with a client-side cache created on first resolution.
    /// <para>注册哨兵 Redis 单例，首次解析时创建客户端和本地缓存。需要 Redis 6.0 或更新版本。</para>
    /// </summary>
    /// <remarks>Client-side caching requires Redis 6.0 or later.</remarks>
    /// <typeparam name="TCache">The cache implementation constructed by Unity.
    /// <para>由 Unity 创建的本地缓存实现类型。</para>
    /// </typeparam>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="enableReadWriteSeparation">Whether to route eligible reads to replicas.
    /// <para>是否将符合条件的读命令路由到从节点。</para>
    /// </param>
    /// <param name="optionsAction">The callback that configures the connection.
    /// <para>用于配置连接的回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisSentinel<TCache>(
        this IUnityContainer container,
        bool enableReadWriteSeparation,
        Action<SentinelConnectionOptions> optionsAction,
        string? serviceName = null)
        where TCache : ClientCachingBase
    {
        return RegisterCachedClient<TCache, SentinelConnectionOptions>(
            container,
            serviceName,
            configure => Redis.UseSentinel(enableReadWriteSeparation, configure),
            optionsAction);
    }
    #endregion

    #region Cluster
    /// <summary>
    /// Connects to Redis in cluster mode and registers the client as a singleton.
    /// <para>连接集群模式的 Redis，并将客户端注册为单例。</para>
    /// </summary>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="enableReadWriteSeparation">Whether to route eligible reads to replicas.
    /// <para>是否将符合条件的读命令路由到从节点。</para>
    /// </param>
    /// <param name="nodeConnectionString">The connection string for a seed node used to discover the cluster.
    /// <para>用于发现集群拓扑的入口节点连接字符串。</para>
    /// </param>
    /// <param name="optionsAction">An optional callback to configure the connection.
    /// <para>用于配置连接的可选回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisCluster(
        this IUnityContainer container,
        bool enableReadWriteSeparation,
        string nodeConnectionString,
        Action<ClusterConnectionOptions>? optionsAction = null,
        string? serviceName = null)
    {
        return RegisterClient(container, serviceName, () => Redis.UseCluster(enableReadWriteSeparation, nodeConnectionString, optionsAction));
    }

    /// <summary>
    /// Registers a cluster Redis singleton with a client-side cache created on first resolution.
    /// <para>注册集群 Redis 单例，首次解析时创建客户端和本地缓存。需要 Redis 6.0 或更新版本。</para>
    /// </summary>
    /// <remarks>Client-side caching requires Redis 6.0 or later.</remarks>
    /// <typeparam name="TCache">The cache implementation constructed by Unity.
    /// <para>由 Unity 创建的本地缓存实现类型。</para>
    /// </typeparam>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="enableReadWriteSeparation">Whether to route eligible reads to replicas.
    /// <para>是否将符合条件的读命令路由到从节点。</para>
    /// </param>
    /// <param name="nodeConnectionString">The connection string for a seed node used to discover the cluster.
    /// <para>用于发现集群拓扑的入口节点连接字符串。</para>
    /// </param>
    /// <param name="optionsAction">An optional callback to configure the connection.
    /// <para>用于配置连接的可选回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisCluster<TCache>(
        this IUnityContainer container,
        bool enableReadWriteSeparation,
        string nodeConnectionString,
        Action<ClusterConnectionOptions>? optionsAction = null,
        string? serviceName = null)
        where TCache : ClientCachingBase
    {
        return RegisterCachedClient<TCache, ClusterConnectionOptions>(
            container,
            serviceName,
            configure => Redis.UseCluster(enableReadWriteSeparation, nodeConnectionString, configure),
            optionsAction);
    }

    /// <summary>
    /// Connects to Redis in cluster mode and registers the client as a singleton.
    /// <para>连接集群模式的 Redis，并将客户端注册为单例。</para>
    /// </summary>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="enableReadWriteSeparation">Whether to route eligible reads to replicas.
    /// <para>是否将符合条件的读命令路由到从节点。</para>
    /// </param>
    /// <param name="optionsAction">The callback that configures the connection.
    /// <para>用于配置连接的回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisCluster(
        this IUnityContainer container,
        bool enableReadWriteSeparation,
        Action<ClusterConnectionOptions> optionsAction,
        string? serviceName = null)
    {
        return RegisterClient(container, serviceName, () => Redis.UseCluster(enableReadWriteSeparation, optionsAction));
    }

    /// <summary>
    /// Registers a cluster Redis singleton with a client-side cache created on first resolution.
    /// <para>注册集群 Redis 单例，首次解析时创建客户端和本地缓存。需要 Redis 6.0 或更新版本。</para>
    /// </summary>
    /// <remarks>Client-side caching requires Redis 6.0 or later.</remarks>
    /// <typeparam name="TCache">The cache implementation constructed by Unity.
    /// <para>由 Unity 创建的本地缓存实现类型。</para>
    /// </typeparam>
    /// <param name="container">The Unity container that owns the registration.
    /// <para>持有此注册的 Unity 容器。</para>
    /// </param>
    /// <param name="enableReadWriteSeparation">Whether to route eligible reads to replicas.
    /// <para>是否将符合条件的读命令路由到从节点。</para>
    /// </param>
    /// <param name="optionsAction">The callback that configures the connection.
    /// <para>用于配置连接的回调。</para>
    /// </param>
    /// <param name="serviceName">The Unity registration name. Pass <see langword="null"/> for the default registration.
    /// <para>Unity 注册名称。传入 <see langword="null"/> 使用默认注册。</para>
    /// </param>
    /// <returns>The container, so that registrations can be chained.</returns>
    public static IUnityContainer AddSharpRedisCluster<TCache>(
        this IUnityContainer container,
        bool enableReadWriteSeparation,
        Action<ClusterConnectionOptions> optionsAction,
        string? serviceName = null)
        where TCache : ClientCachingBase
    {
        return RegisterCachedClient<TCache, ClusterConnectionOptions>(
            container,
            serviceName,
            configure => Redis.UseCluster(enableReadWriteSeparation, configure),
            optionsAction);
    }
    #endregion

    private static IUnityContainer RegisterClient(IUnityContainer container, string? serviceName, Func<Redis> connect)
    {
        if (container is null) throw new ArgumentNullException(nameof(container));

        var redis = connect();
        try
        {
            container.RegisterInstance<Redis>(serviceName, redis, new ContainerControlledLifetimeManager());
        }
        catch
        {
            redis.Dispose();
            throw;
        }

        return RegisterCommandGroups(container, serviceName);
    }

    private static IUnityContainer RegisterCachedClient<TCache, TOptions>(
        IUnityContainer container,
        string? serviceName,
        Func<Action<TOptions>, Redis> connect,
        Action<TOptions>? optionsAction)
        where TCache : ClientCachingBase
        where TOptions : BasalConnectionOptions
    {
        if (container is null) throw new ArgumentNullException(nameof(container));

        // Each Redis client owns its cache. A private transient Unity registration
        // gives each creation attempt a fresh cache and still allows dependency injection.
        var cacheName = Guid.NewGuid().ToString("N");
        container.RegisterType<TCache>(cacheName);
        container.RegisterFactory(
            typeof(Redis),
            serviceName,
            (resolver, _, _) =>
            {
                TCache? cache = null;
                try
                {
                    return connect(options =>
                    {
                        optionsAction?.Invoke(options);
                        cache = resolver.Resolve<TCache>(cacheName);
                        options.UseClientCaching(cache);
                    });
                }
                catch
                {
                    cache?.Dispose();
                    throw;
                }
            },
            new ContainerControlledLifetimeManager());

        return RegisterCommandGroups(container, serviceName);
    }

    private static IUnityContainer RegisterCommandGroups(IUnityContainer container, string? serviceName)
    {
        RegisterCommandGroup(container, serviceName, static redis => redis.String);
        RegisterCommandGroup(container, serviceName, static redis => redis.Hash);
        RegisterCommandGroup(container, serviceName, static redis => redis.List);
        RegisterCommandGroup(container, serviceName, static redis => redis.Set);
        RegisterCommandGroup(container, serviceName, static redis => redis.SortedSet);
        RegisterCommandGroup(container, serviceName, static redis => redis.Stream);
        RegisterCommandGroup(container, serviceName, static redis => redis.Bitmap);
        RegisterCommandGroup(container, serviceName, static redis => redis.HyperLogLog);
        RegisterCommandGroup(container, serviceName, static redis => redis.Geo);
        RegisterCommandGroup(container, serviceName, static redis => redis.Key);
        RegisterCommandGroup(container, serviceName, static redis => redis.Connection);
        RegisterCommandGroup(container, serviceName, static redis => redis.Server);
        RegisterCommandGroup(container, serviceName, static redis => redis.Script);
        RegisterCommandGroup(container, serviceName, static redis => redis.Func);
        RegisterCommandGroup(container, serviceName, static redis => redis.PubSub);
        RegisterCommandGroup(container, serviceName, static redis => redis.Array);
        RegisterCommandGroup(container, serviceName, static redis => redis.Cluster);
        RegisterCommandGroup(container, serviceName, static redis => redis.Modules);
        RegisterCommandGroup(container, serviceName, static redis => redis.Modules.Bloom);
        RegisterCommandGroup(container, serviceName, static redis => redis.Modules.Cuckoo);
        RegisterCommandGroup(container, serviceName, static redis => redis.Modules.Sketch);
        RegisterCommandGroup(container, serviceName, static redis => redis.Modules.Search);
        return container;
    }

    private static void RegisterCommandGroup<TCommands>(
        IUnityContainer container,
        string? serviceName,
        Func<Redis, TCommands> select)
        where TCommands : class
    {
        // Redis already exposes a shared command group. Only the client owns it.
        container.RegisterFactory(
            typeof(TCommands),
            serviceName,
            (resolver, _, name) => select(resolver.Resolve<Redis>(name)),
            null);
    }
}
