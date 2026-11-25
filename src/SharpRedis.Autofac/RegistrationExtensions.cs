using Autofac;
using Autofac.Features.AttributeFilters;
using SharpRedis.Modules;
using SharpRedis.Provider.Standard;
using System;

namespace SharpRedis.Autofac;

/// <summary>
/// SharpRedis Autofac Extensions
/// </summary>
public static class RegistrationExtensions
{
    #region Standalone
    /// <summary>
    /// Connect singleton Redis and register with Autofac
    /// <para>连接单例Redis, 并注册到Autofac</para>
    /// </summary>
    /// <param name="builder">Autofac</param>
    /// <param name="connectionString">connection string
    /// <para>连接字符串</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <param name="optionsAction">optionsAction</param>
    /// <returns></returns>
#if MODERN_NET
    public static ContainerBuilder AddSharpRedisStandalone(this ContainerBuilder builder, string connectionString, Action<StandaloneConnectionOptions>? optionsAction = null, string? serviceName = null)
#else
    public static ContainerBuilder AddSharpRedisStandalone(this ContainerBuilder builder, string connectionString, Action<StandaloneConnectionOptions> optionsAction = null, string serviceName = null)
#endif
    {
        var redis = Redis.UseStandalone(connectionString, optionsAction);
        var redisBuilder = builder.RegisterInstance(redis).SingleInstance();

        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect singleton Redis and register with Autofac
    /// <para>连接单例Redis, 并注册到Autofac</para>
    /// </summary>
    /// <param name="builder">Autofac</param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <param name="optionsAction">optionsAction</param>
    /// <returns></returns>
#if MODERN_NET
    public static ContainerBuilder AddSharpRedisStandalone(this ContainerBuilder builder, Action<StandaloneConnectionOptions> optionsAction, string? serviceName = null)
#else
    public static ContainerBuilder AddSharpRedisStandalone(this ContainerBuilder builder, Action<StandaloneConnectionOptions> optionsAction, string serviceName = null)
#endif
    {
        return builder.AddSharpRedisStandalone(string.Empty, optionsAction, serviceName);
    }

    /// <summary>
    /// Connect to singleton Redis, enable local cache support, and register with Autofac
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接单例Redis, 开启本地缓存支持, 并注册到Autofac</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="builder">Autofac</param>
    /// <param name="connectionString">connection string
    /// <para>连接字符串</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <param name="optionsAction">optionsAction</param>
    /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public static ContainerBuilder AddSharpRedisStandalone<TCache>(this ContainerBuilder builder, string connectionString, Action<StandaloneConnectionOptions>? optionsAction = null, string? serviceName = null)
#else
    public static ContainerBuilder AddSharpRedisStandalone<TCache>(this ContainerBuilder builder, string connectionString, Action<StandaloneConnectionOptions> optionsAction = null, string serviceName = null)
#endif
        where TCache : ClientCachingBase
    {
        var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
        var cacheBuilder = builder.RegisterType<TCache>()
            .WithAttributeFiltering()
            .SingleInstance()
            .Named<ClientCachingBase>(cacheServiceName);

        var redisBuilder = builder.Register(c =>
        {
            var cache = c.ResolveNamed<ClientCachingBase>(cacheServiceName);
            var redis = Redis.UseStandalone(connectionString, options =>
            {
                optionsAction?.Invoke(options);
                options.UseClientCaching(cache);
            });
            return redis;
        }).SingleInstance();

        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to singleton Redis, enable local cache support, and register with Autofac
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接单例Redis, 开启本地缓存支持, 并注册到Autofac</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="builder">Autofac</param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <param name="optionsAction">optionsAction</param>
    /// <returns></returns>
#if MODERN_NET
    public static ContainerBuilder AddSharpRedisStandalone<TCache>(this ContainerBuilder builder, Action<StandaloneConnectionOptions> optionsAction, string? serviceName = null)
#else
    public static ContainerBuilder AddSharpRedisStandalone<TCache>(this ContainerBuilder builder, Action<StandaloneConnectionOptions> optionsAction, string serviceName = null)
#endif
        where TCache : ClientCachingBase
    {
        return builder.AddSharpRedisStandalone<TCache>(string.Empty, optionsAction, serviceName);
    }
    #endregion

    #region MasterSlave
    /// <summary>
    /// Connect to the Redis in the read-write separation mode and register it with Autofac
    /// <para>连接到读写分离模式的Redis, 并注册到Autofac</para>
    /// </summary>
    /// <param name="builder">Autofac</param>
    /// <param name="masterConnectionString">Master node connection string, for example: host=127.0.0.1,port=10086,password=123456
    /// <para>主节点连接字符串, 如: host=127.0.0.1,port=10086,password=123456</para>
    /// </param>
    /// <param name="slaveConnectionStrings">Array of connection strings for Redis slave nodes
    /// <para>从节点连接字符串数组</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisMasterSlave(
        this ContainerBuilder builder,
        string masterConnectionString,
        string[] slaveConnectionStrings,
#if MODERN_NET
        Action<MasterSlaveConnectionOptions>? optionsAction = null,
        string? serviceName = null
#else
        Action<MasterSlaveConnectionOptions> optionsAction = null,
        string serviceName = null
#endif
    )
    {
        var redis = Redis.UseMasterSlave(masterConnectionString, slaveConnectionStrings, optionsAction);
        var redisBuilder = builder.RegisterInstance(redis).SingleInstance();
        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Redis in the read-write separation mode, enable local cache support, and register with Autofac
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接到读写分离模式的Redis, 开启本地缓存支持, 并注册到Autofac</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="builder">Autofac</param>
    /// <param name="masterConnectionString">Master node connection string, for example: host=127.0.0.1,port=10086,password=123456
    /// <para>主节点连接字符串, 如: host=127.0.0.1,port=10086,password=123456</para>
    /// </param>
    /// <param name="slaveConnectionStrings">Array of connection strings for Redis slave nodes
    /// <para>从节点连接字符串数组</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisMasterSlave<TCache>(
        this ContainerBuilder builder,
        string masterConnectionString,
        string[] slaveConnectionStrings,
#if MODERN_NET
        Action<MasterSlaveConnectionOptions>? optionsAction = null,
        string? serviceName = null
#else
        Action<MasterSlaveConnectionOptions> optionsAction = null,
        string serviceName = null
#endif
    ) where TCache : ClientCachingBase
    {
        var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
        var cacheBuilder = builder.RegisterType<TCache>()
            .WithAttributeFiltering()
            .SingleInstance()
            .Named<ClientCachingBase>(cacheServiceName);

        var redisBuilder = builder.Register(c =>
        {
            var cache = c.ResolveNamed<ClientCachingBase>(cacheServiceName);
            var redis = Redis.UseMasterSlave(masterConnectionString, slaveConnectionStrings, options =>
            {
                optionsAction?.Invoke(options);
                options.UseClientCaching(cache);
            });
            return redis;
        }).SingleInstance();

        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Redis in the read-write separation mode, enable local cache support, and register with Autofac
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接到读写分离模式的Redis, 开启本地缓存支持, 并注册到Autofac</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="builder">Autofac</param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisMasterSlave<TCache>(
        this ContainerBuilder builder,
        Action<MasterSlaveConnectionOptions> optionsAction,
#if MODERN_NET
        string? serviceName = null
#else
        string serviceName = null
#endif
    ) where TCache : ClientCachingBase
    {
        var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
        var cacheBuilder = builder.RegisterType<TCache>()
            .WithAttributeFiltering()
            .SingleInstance()
            .Named<ClientCachingBase>(cacheServiceName);

        var redisBuilder = builder.Register(c =>
        {
            var cache = c.ResolveNamed<ClientCachingBase>(cacheServiceName);
            var redis = Redis.UseMasterSlave(options =>
            {
                optionsAction.Invoke(options);
                options.UseClientCaching(cache);
            });
            return redis;
        }).SingleInstance();

        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Redis in the read-write separation mode and register it with Autofac
    /// <para>连接到读写分离模式的Redis, 并注册到Autofac</para>
    /// </summary>
    /// <param name="builder">Autofac</param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisMasterSlave(
        this ContainerBuilder builder,
        Action<MasterSlaveConnectionOptions> optionsAction,
#if MODERN_NET
        string? serviceName = null
#else
        string serviceName = null
#endif
    )
    {
        var redis = Redis.UseMasterSlave(optionsAction);
        var redisBuilder = builder.RegisterInstance(redis).SingleInstance();
        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }
    #endregion

    #region Sentinel
    /// <summary>
    /// Connect to the Sentinel mode Redis and register it with Autofac
    /// <para>连接到哨兵模式的Redis, 并注册到Autofac</para>
    /// </summary>
    /// <param name="builder">Autofac</param>
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
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisSentinel(
        this ContainerBuilder builder,
        bool enableReadWriteSeparation,
        string masterConnectionString,
        string[] sentinelConnectionStrings,
#if MODERN_NET
        Action<SentinelConnectionOptions>? optionsAction = null,
        string? serviceName = null
#else
        Action<SentinelConnectionOptions> optionsAction = null,
        string serviceName = null
#endif
    )
    {
        var redis = Redis.UseSentinel(enableReadWriteSeparation, masterConnectionString, sentinelConnectionStrings, optionsAction);
        var redisBuilder = builder.RegisterInstance(redis).SingleInstance();
        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Sentinel mode Redis, enable local cache support, and register with Autofac
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接到哨兵模式的Redis, 开启本地缓存支持, 并注册到Autofac</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="builder">Autofac</param>
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
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisSentinel<TCache>(
        this ContainerBuilder builder,
        bool enableReadWriteSeparation,
        string masterConnectionString,
        string[] sentinelConnectionStrings,
#if MODERN_NET
        Action<SentinelConnectionOptions>? optionsAction = null,
        string? serviceName = null
#else
        Action<SentinelConnectionOptions> optionsAction = null,
        string serviceName = null
#endif
    ) where TCache : ClientCachingBase
    {
        var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
        var cacheBuilder = builder.RegisterType<TCache>()
            .WithAttributeFiltering()
            .SingleInstance()
            .Named<ClientCachingBase>(cacheServiceName);

        var redisBuilder = builder.Register(c =>
        {
            var cache = c.ResolveNamed<ClientCachingBase>(cacheServiceName);
            var redis = Redis.UseSentinel(enableReadWriteSeparation, masterConnectionString, sentinelConnectionStrings, options =>
            {
                optionsAction?.Invoke(options);
                options.UseClientCaching(cache);
            });
            return redis;
        }).SingleInstance();

        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Sentinel mode Redis and register it with Autofac
    /// <para>连接到哨兵模式的Redis, 并注册到Autofac</para>
    /// </summary>
    /// <param name="builder">Autofac</param>
    /// <param name="enableReadWriteSeparation">Whether to read from replicas. When false, all operations use the master.
    /// <para>是否开启读写分离；false 时不连接从节点，所有操作使用主节点，并自动跟随主节点切换。</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisSentinel(
        this ContainerBuilder builder,
        bool enableReadWriteSeparation,
        Action<SentinelConnectionOptions> optionsAction,
#if MODERN_NET
        string? serviceName = null
#else
        string serviceName = null
#endif
    )
    {
        var redis = Redis.UseSentinel(enableReadWriteSeparation, optionsAction);
        var redisBuilder = builder.RegisterInstance(redis).SingleInstance();
        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Sentinel mode Redis, enable local cache support, and register with Autofac
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接到哨兵模式的Redis, 开启本地缓存支持, 并注册到Autofac</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="builder">Autofac</param>
    /// <param name="enableReadWriteSeparation">Whether to read from replicas. When false, all operations use the master.
    /// <para>是否开启读写分离；false 时不连接从节点，所有操作使用主节点，并自动跟随主节点切换。</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisSentinel<TCache>(
        this ContainerBuilder builder,
        bool enableReadWriteSeparation,
        Action<SentinelConnectionOptions> optionsAction,
#if MODERN_NET
        string? serviceName = null
#else
        string serviceName = null
#endif
    ) where TCache : ClientCachingBase
    {
        var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
        var cacheBuilder = builder.RegisterType<TCache>()
            .WithAttributeFiltering()
            .SingleInstance()
            .Named<ClientCachingBase>(cacheServiceName);

        var redisBuilder = builder.Register(c =>
        {
            var cache = c.ResolveNamed<ClientCachingBase>(cacheServiceName);
            var redis = Redis.UseSentinel(enableReadWriteSeparation, options =>
            {
                optionsAction.Invoke(options);
                options.UseClientCaching(cache);
            });
            return redis;
        }).SingleInstance();

        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }
    #endregion

    #region Cluster
    /// <summary>
    /// Connect to the cluster mode of Redis and register it with Autofac
    /// <para>连接到集群模式的Redis, 并注册到Autofac</para>
    /// </summary>
    /// <param name="builder">Autofac</param>
    /// <param name="enableReadWriteSeparation">Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </param>
    /// <param name="nodeConnectionString">Connection string for any master node in the Redis Cluster. SharpRedis will automatically discover the other cluster nodes.
    /// <para>任意一个插槽主节点的连接字符串, ShapRedis会自动发现其它插槽节点</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisCluster(
        this ContainerBuilder builder,
        bool enableReadWriteSeparation,
        string nodeConnectionString,
#if MODERN_NET
        Action<ClusterConnectionOptions>? optionsAction = null,
        string? serviceName = null
#else
        Action<ClusterConnectionOptions> optionsAction = null,
        string serviceName = null
#endif
    )
    {
        var redis = Redis.UseCluster(enableReadWriteSeparation, nodeConnectionString, optionsAction);
        var redisBuilder = builder.RegisterInstance(redis).SingleInstance();
        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the cluster mode of Redis, enable local cache support, and register with Autofac
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接到集群模式的Redis, 开启本地缓存支持, 并注册到Autofac</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="builder">Autofac</param>
    /// <param name="enableReadWriteSeparation">Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </param>
    /// <param name="nodeConnectionString">Connection string for any master node in the Redis Cluster. SharpRedis will automatically discover the other cluster nodes.
    /// <para>任意一个插槽主节点的连接字符串, ShapRedis会自动发现其它插槽节点</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisCluster<TCache>(
        this ContainerBuilder builder,
        bool enableReadWriteSeparation,
        string nodeConnectionString,
#if MODERN_NET
        Action<ClusterConnectionOptions>? optionsAction = null,
        string? serviceName = null
#else
        Action<ClusterConnectionOptions> optionsAction = null,
        string serviceName = null
#endif
    ) where TCache : ClientCachingBase
    {
        var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
        var cacheBuilder = builder.RegisterType<TCache>()
            .WithAttributeFiltering()
            .SingleInstance()
            .Named<ClientCachingBase>(cacheServiceName);

        var redisBuilder = builder.Register(c =>
        {
            var cache = c.ResolveNamed<ClientCachingBase>(cacheServiceName);
            var redis = Redis.UseCluster(enableReadWriteSeparation, nodeConnectionString, options =>
            {
                optionsAction?.Invoke(options);
                options.UseClientCaching(cache);
            });
            return redis;
        }).SingleInstance();

        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the cluster mode of Redis and register it with Autofac
    /// <para>连接到集群模式的Redis, 并注册到Autofac</para>
    /// </summary>
    /// <param name="builder">Autofac</param>
    /// <param name="enableReadWriteSeparation">Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisCluster(
        this ContainerBuilder builder,
        bool enableReadWriteSeparation,
        Action<ClusterConnectionOptions> optionsAction,
#if MODERN_NET
        string? serviceName = null
#else
        string serviceName = null
#endif
    )
    {
        var redis = Redis.UseCluster(enableReadWriteSeparation, optionsAction);
        var redisBuilder = builder.RegisterInstance(redis).SingleInstance();
        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the cluster mode of Redis, enable local cache support, and register with Autofac
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接到集群模式的Redis, 开启本地缓存支持, 并注册到Autofac</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="builder">Autofac</param>
    /// <param name="enableReadWriteSeparation">Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">Named service to associate with the component
    /// <para>Autofac命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static ContainerBuilder AddSharpRedisCluster<TCache>(
        this ContainerBuilder builder,
        bool enableReadWriteSeparation,
        Action<ClusterConnectionOptions> optionsAction,
#if MODERN_NET
        string? serviceName = null
#else
        string serviceName = null
#endif
    ) where TCache : ClientCachingBase
    {
        var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
        var cacheBuilder = builder.RegisterType<TCache>()
            .WithAttributeFiltering()
            .SingleInstance()
            .Named<ClientCachingBase>(cacheServiceName);

        var redisBuilder = builder.Register(c =>
        {
            var cache = c.ResolveNamed<ClientCachingBase>(cacheServiceName);
            var redis = Redis.UseCluster(enableReadWriteSeparation, options =>
            {
                optionsAction.Invoke(options);
                options.UseClientCaching(cache);
            });
            return redis;
        }).SingleInstance();

        if (serviceName != null)
        {
            redisBuilder.Named<Redis>(serviceName);
        }

        return builder.RegisterTypes(serviceName);
    }
    #endregion

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    private static ContainerBuilder RegisterTypes(this ContainerBuilder builder, string? serviceName)
#else
    private static ContainerBuilder RegisterTypes(this ContainerBuilder builder, string serviceName)
#endif
    {
        var stringBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).String;
            return c.Resolve<Redis>().String;
        }).SingleInstance();

        var hashBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Hash;
            return c.Resolve<Redis>().Hash;
        }).SingleInstance();

        var listBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).List;
            return c.Resolve<Redis>().List;
        }).SingleInstance();

        var setBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Set;
            return c.Resolve<Redis>().Set;
        }).SingleInstance();

        var sortedSetBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).SortedSet;
            return c.Resolve<Redis>().SortedSet;
        }).SingleInstance();

        var streamBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Stream;
            return c.Resolve<Redis>().Stream;
        }).SingleInstance();

        var bitmapBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Bitmap;
            return c.Resolve<Redis>().Bitmap;
        }).SingleInstance();

        var hyperLogLogBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).HyperLogLog;
            return c.Resolve<Redis>().HyperLogLog;
        }).SingleInstance();

        var geoBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Geo;
            return c.Resolve<Redis>().Geo;
        }).SingleInstance();

        var keyBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Key;
            return c.Resolve<Redis>().Key;
        }).SingleInstance();

        var connectionBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Connection;
            return c.Resolve<Redis>().Connection;
        }).SingleInstance();

        var serverBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Server;
            return c.Resolve<Redis>().Server;
        }).SingleInstance();

        var scriptBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Script;
            return c.Resolve<Redis>().Script;
        }).SingleInstance();

        var funcBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Func;
            return c.Resolve<Redis>().Func;
        }).SingleInstance();

        var pubsubBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).PubSub;
            return c.Resolve<Redis>().PubSub;
        }).SingleInstance();

        var arrayBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Array;
            return c.Resolve<Redis>().Array;
        }).SingleInstance();

        var clusterBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Cluster;
            return c.Resolve<Redis>().Cluster;
        }).SingleInstance();

        var modulesBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Modules;
            return c.Resolve<Redis>().Modules;
        }).SingleInstance();

        var modulesBloomBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Modules.Bloom;
            return c.Resolve<Redis>().Modules.Bloom;
        }).SingleInstance();

        var modulesCuckooBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Modules.Cuckoo;
            return c.Resolve<Redis>().Modules.Cuckoo;
        }).SingleInstance();

        var modulesSketchBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Modules.Sketch;
            return c.Resolve<Redis>().Modules.Sketch;
        }).SingleInstance();

        var modulesSearchBuilder = builder.Register(c =>
        {
            if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Modules.Search;
            return c.Resolve<Redis>().Modules.Search;
        }).SingleInstance();

        if (serviceName != null)
        {
            stringBuilder.Named<RedisString>(serviceName);
            hashBuilder.Named<RedisHash>(serviceName);
            listBuilder.Named<RedisList>(serviceName);
            setBuilder.Named<RedisSet>(serviceName);
            sortedSetBuilder.Named<RedisSortedSet>(serviceName);
            streamBuilder.Named<RedisStream>(serviceName);
            bitmapBuilder.Named<RedisBitmap>(serviceName);
            hyperLogLogBuilder.Named<RedisHyperLogLog>(serviceName);
            geoBuilder.Named<RedisGeospatial>(serviceName);
            keyBuilder.Named<RedisKeyspace>(serviceName);
            connectionBuilder.Named<RedisConnection>(serviceName);
            serverBuilder.Named<RedisServer>(serviceName);
            scriptBuilder.Named<RedisScript>(serviceName);
            pubsubBuilder.Named<RedisPubSub>(serviceName);
            arrayBuilder.Named<RedisArray>(serviceName);
            funcBuilder.Named<RedisFunction>(serviceName);
            clusterBuilder.Named<RedisCluster>(serviceName);
            modulesBuilder.Named<RedisModules>(serviceName);
            modulesBloomBuilder.Named<RedisBloom>(serviceName);
            modulesCuckooBuilder.Named<RedisCuckoo>(serviceName);
            modulesSketchBuilder.Named<RedisCountMinSketch>(serviceName);
            modulesSearchBuilder.Named<RedisSearch>(serviceName);
        }
        return builder;
    }
}
