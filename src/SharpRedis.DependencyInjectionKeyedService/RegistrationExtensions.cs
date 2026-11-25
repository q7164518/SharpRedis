using Microsoft.Extensions.DependencyInjection;
using SharpRedis.Provider.Standard;
using System;

namespace SharpRedis.DependencyInjectionKeyedService;

/// <summary>
/// SharpRedis Microsoft.Extensions.DependencyInjection Extensions
/// </summary>
public static class RegistrationExtensions
{
    #region Standalone
    /// <summary>
    /// Connect singleton Redis and register with Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>连接单例Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="connectionString">connection string
    /// <para>连接字符串</para>
    /// </param>
    /// <param name="optionsAction">optionsAction</param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisStandalone(
        this IServiceCollection services,
        string connectionString,
#if MODERN_NET
        Action<StandaloneConnectionOptions>? optionsAction = null,
        string? serviceName = null
#else
        Action<StandaloneConnectionOptions> optionsAction = null,
        string serviceName = null
#endif
    )
    {
        var redis = Redis.UseStandalone(connectionString, optionsAction);
        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, redis);
        }
        else
        {
            services.AddSingleton(redis);
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the singleton Redis, enable local cache support, and register it to the Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接单例Redis, 开启本地缓存支持, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="services">IServiceCollection</param>
    /// <param name="connectionString">connection string
    /// <para>连接字符串</para>
    /// </param>
    /// <param name="optionsAction">optionsAction</param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisStandalone<TCache>(
        this IServiceCollection services,
        string connectionString,
#if MODERN_NET
        Action<StandaloneConnectionOptions>? optionsAction = null,
        string? serviceName = null
#else
        Action<StandaloneConnectionOptions> optionsAction = null,
        string serviceName = null
#endif
    ) where TCache : ClientCachingBase
    {
        var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
        services.AddKeyedSingleton<ClientCachingBase, TCache>(cacheServiceName);

        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, (c, _) =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseStandalone(connectionString, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        else
        {
            services.AddSingleton(c =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseStandalone(connectionString, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect singleton Redis and register with Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>连接单例Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="optionsAction">optionsAction</param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisStandalone(
        this IServiceCollection services,
        Action<StandaloneConnectionOptions> optionsAction,
#if MODERN_NET
        string? serviceName = null
#else
        string serviceName = null
#endif
    )
    {
        var redis = Redis.UseStandalone(optionsAction);
        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, redis);
        }
        else
        {
            services.AddSingleton(redis);
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the singleton Redis, enable local cache support, and register it to the Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接单例Redis, 开启本地缓存支持, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="services">IServiceCollection</param>
    /// <param name="optionsAction">optionsAction</param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisStandalone<TCache>(
        this IServiceCollection services,
        Action<StandaloneConnectionOptions> optionsAction,
#if MODERN_NET
        string? serviceName = null
#else
        string serviceName = null
#endif
    ) where TCache : ClientCachingBase
    {
        var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
        services.AddKeyedSingleton<ClientCachingBase, TCache>(cacheServiceName);

        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, (c, _) =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseStandalone(options =>
                {
                    optionsAction.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        else
        {
            services.AddSingleton(c =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseStandalone(options =>
                {
                    optionsAction.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        return services.RegisterTypes(serviceName);
    }
    #endregion

    #region MasterSlave
    /// <summary>
    /// Connect to the Redis in the read-write separation mode and register it with Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>连接到读写分离模式的Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="masterConnectionString">Master node connection string, for example: host=127.0.0.1,port=10086,password=123456
    /// <para>主节点连接字符串, 如: host=127.0.0.1,port=10086,password=123456</para>
    /// </param>
    /// <param name="slaveConnectionStrings">Array of connection strings for Redis slave nodes
    /// <para>从节点连接字符串数组</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisMasterSlave(
        this IServiceCollection services,
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
        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, redis);
        }
        else
        {
            services.AddSingleton(redis);
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Redis in the read-write separation mode, enable local cache support, and register it to the Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接到读写分离模式的Redis, 开启本地缓存支持, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="services">IServiceCollection</param>
    /// <param name="masterConnectionString">Master node connection string, for example: host=127.0.0.1,port=10086,password=123456
    /// <para>主节点连接字符串, 如: host=127.0.0.1,port=10086,password=123456</para>
    /// </param>
    /// <param name="slaveConnectionStrings">Array of connection strings for Redis slave nodes
    /// <para>从节点连接字符串数组</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisMasterSlave<TCache>(
        this IServiceCollection services,
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
        services.AddKeyedSingleton<ClientCachingBase, TCache>(cacheServiceName);

        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, (c, _) =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseMasterSlave(masterConnectionString, slaveConnectionStrings, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        else
        {
            services.AddSingleton(c =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseMasterSlave(masterConnectionString, slaveConnectionStrings, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Redis in the read-write separation mode and register it with Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>连接到读写分离模式的Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisMasterSlave(
        this IServiceCollection services,
        Action<MasterSlaveConnectionOptions> optionsAction,
#if MODERN_NET
        string? serviceName = null
#else
        string serviceName = null
#endif
    )
    {
        var redis = Redis.UseMasterSlave(optionsAction);
        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, redis);
        }
        else
        {
            services.AddSingleton(redis);
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Redis in the read-write separation mode, enable local cache support, and register it to the Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接到读写分离模式的Redis, 开启本地缓存支持, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="services">IServiceCollection</param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisMasterSlave<TCache>(
        this IServiceCollection services,
        Action<MasterSlaveConnectionOptions> optionsAction,
#if MODERN_NET
        string? serviceName = null
#else
        string serviceName = null
#endif
    ) where TCache : ClientCachingBase
    {
        var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
        services.AddKeyedSingleton<ClientCachingBase, TCache>(cacheServiceName);

        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, (c, _) =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseMasterSlave(options =>
                {
                    optionsAction.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        else
        {
            services.AddSingleton(c =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseMasterSlave(options =>
                {
                    optionsAction.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        return services.RegisterTypes(serviceName);
    }
    #endregion

    #region Sentinel
    /// <summary>
    /// Connect to the Sentinel mode Redis and register it with Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>连接到哨兵模式的Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// </summary>
    /// <param name="services">IServiceCollection</param>
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
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisSentinel(
        this IServiceCollection services,
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
        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, redis);
        }
        else
        {
            services.AddSingleton(redis);
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Sentinel mode Redis, enable local cache support, and register it to Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>连接到哨兵模式的Redis, 开启本地缓存支持, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <param name="services">IServiceCollection</param>
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
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisSentinel<TCache>(
        this IServiceCollection services,
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
        services.AddKeyedSingleton<ClientCachingBase, TCache>(cacheServiceName);

        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, (c, _) =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseSentinel(enableReadWriteSeparation, masterConnectionString, sentinelConnectionStrings, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        else
        {
            services.AddSingleton(c =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseSentinel(enableReadWriteSeparation, masterConnectionString, sentinelConnectionStrings, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Sentinel mode Redis and register it with Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>连接到哨兵模式的Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="enableReadWriteSeparation">Whether to read from replicas. When false, all operations use the master.
    /// <para>是否开启读写分离；false 时不连接从节点，所有操作使用主节点，并自动跟随主节点切换。</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisSentinel(
        this IServiceCollection services,
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
        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, redis);
        }
        else
        {
            services.AddSingleton(redis);
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the Sentinel mode Redis, enable local cache support, and register it to Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>连接到哨兵模式的Redis, 开启本地缓存支持, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="enableReadWriteSeparation">Whether to read from replicas. When false, all operations use the master.
    /// <para>是否开启读写分离；false 时不连接从节点，所有操作使用主节点，并自动跟随主节点切换。</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisSentinel<TCache>(
        this IServiceCollection services,
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
        services.AddKeyedSingleton<ClientCachingBase, TCache>(cacheServiceName);

        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, (c, _) =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseSentinel(enableReadWriteSeparation, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        else
        {
            services.AddSingleton(c =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseSentinel(enableReadWriteSeparation, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        return services.RegisterTypes(serviceName);
    }
    #endregion

    #region Cluster
    /// <summary>
    /// Connect to the cluster mode of Redis and register it with Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>连接到集群模式的Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="enableReadWriteSeparation">Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </param>
    /// <param name="nodeConnectionString">Connection string for any master node in the Redis Cluster. SharpRedis will automatically discover the other cluster nodes.
    /// <para>任意一个插槽主节点的连接字符串, ShapRedis会自动发现其它插槽节点</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisCluster(
        this IServiceCollection services,
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
        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, redis);
        }
        else
        {
            services.AddSingleton(redis);
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the cluster mode of Redis, enable local cache support, and register it to Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接到集群模式的Redis, 开启本地缓存支持, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="services">IServiceCollection</param>
    /// <param name="enableReadWriteSeparation">Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </param>
    /// <param name="nodeConnectionString">Connection string for any master node in the Redis Cluster. SharpRedis will automatically discover the other cluster nodes.
    /// <para>任意一个插槽主节点的连接字符串, ShapRedis会自动发现其它插槽节点</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisCluster<TCache>(
        this IServiceCollection services,
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
        services.AddKeyedSingleton<ClientCachingBase, TCache>(cacheServiceName);

        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, (c, _) =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseCluster(enableReadWriteSeparation, nodeConnectionString, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        else
        {
            services.AddSingleton(c =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseCluster(enableReadWriteSeparation, nodeConnectionString, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the cluster mode of Redis and register it with Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>连接到集群模式的Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="enableReadWriteSeparation">Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisCluster(
        this IServiceCollection services,
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
        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, redis);
        }
        else
        {
            services.AddSingleton(redis);
        }
        return services.RegisterTypes(serviceName);
    }

    /// <summary>
    /// Connect to the cluster mode of Redis, enable local cache support, and register it to Microsoft.Extensions.DependencyInjection ServiceCollection
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>连接到集群模式的Redis, 开启本地缓存支持, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <typeparam name="TCache">The local cache implements generics
    /// <para>本地缓存实现泛型</para>
    /// </typeparam>
    /// <param name="services">IServiceCollection</param>
    /// <param name="enableReadWriteSeparation">Whether to enable read-write separation
    /// <para>是否开启读写分离</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置方法</para>
    /// </param>
    /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
    /// <para>命名注册, 不命名注册传null</para>
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddSharpRedisCluster<TCache>(
        this IServiceCollection services,
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
        services.AddKeyedSingleton<ClientCachingBase, TCache>(cacheServiceName);

        if (serviceName != null)
        {
            services.AddKeyedSingleton(serviceName, (c, _) =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseCluster(enableReadWriteSeparation, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        else
        {
            services.AddSingleton(c =>
            {
                var cache = c.GetRequiredKeyedService<ClientCachingBase>(cacheServiceName);
                var redis = Redis.UseCluster(enableReadWriteSeparation, options =>
                {
                    optionsAction?.Invoke(options);
                    options.UseClientCaching(cache);
                });
                return redis;
            });
        }
        return services.RegisterTypes(serviceName);
    }
    #endregion

#if MODERN_NET
    private static IServiceCollection RegisterTypes(this IServiceCollection services, string? serviceName)
#else
    private static IServiceCollection RegisterTypes(this IServiceCollection services, string serviceName)
#endif
    {
        if (serviceName != null)
        {
            return services.AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().String)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Hash)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().List)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Set)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().SortedSet)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Bitmap)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().HyperLogLog)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Geo)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Script)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Func)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Connection)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Key)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Server)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().PubSub)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Stream)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Cluster)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Array)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Modules)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Modules.Bloom)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Modules.Cuckoo)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Modules.Search)
                .AddKeyedSingleton(serviceName, (c, _) => c.GetRequiredService<Redis>().Modules.Sketch)
            ;
        }
        else
        {
            return services.AddSingleton(c => c.GetRequiredService<Redis>().String)
                .AddSingleton(c => c.GetRequiredService<Redis>().Hash)
                .AddSingleton(c => c.GetRequiredService<Redis>().List)
                .AddSingleton(c => c.GetRequiredService<Redis>().Set)
                .AddSingleton(c => c.GetRequiredService<Redis>().SortedSet)
                .AddSingleton(c => c.GetRequiredService<Redis>().Bitmap)
                .AddSingleton(c => c.GetRequiredService<Redis>().HyperLogLog)
                .AddSingleton(c => c.GetRequiredService<Redis>().Geo)
                .AddSingleton(c => c.GetRequiredService<Redis>().Script)
                .AddSingleton(c => c.GetRequiredService<Redis>().Func)
                .AddSingleton(c => c.GetRequiredService<Redis>().Connection)
                .AddSingleton(c => c.GetRequiredService<Redis>().Key)
                .AddSingleton(c => c.GetRequiredService<Redis>().Server)
                .AddSingleton(c => c.GetRequiredService<Redis>().PubSub)
                .AddSingleton(c => c.GetRequiredService<Redis>().Stream)
                .AddSingleton(c => c.GetRequiredService<Redis>().Cluster)
                .AddSingleton(c => c.GetRequiredService<Redis>().Array)
                .AddSingleton(c => c.GetRequiredService<Redis>().Modules)
                .AddSingleton(c => c.GetRequiredService<Redis>().Modules.Bloom)
                .AddSingleton(c => c.GetRequiredService<Redis>().Modules.Cuckoo)
                .AddSingleton(c => c.GetRequiredService<Redis>().Modules.Search)
                .AddSingleton(c => c.GetRequiredService<Redis>().Modules.Sketch)
            ;
        }
    }
}
