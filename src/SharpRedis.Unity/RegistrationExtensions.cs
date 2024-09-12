using System;
using Unity;
using Unity.Lifetime;

namespace SharpRedis.Unity
{
    /// <summary>
    /// SharpRedis Unity Extensions
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Connect singleton Redis and register with UnityContainer
        /// <para>连接单例Redis, 并注册到UnityContainer</para>
        /// </summary>
        /// <param name="builder">UnityContainer</param>
        /// <param name="connectionString">connection string
        /// <para>连接字符串</para>
        /// </param>
        /// <param name="serviceName">Named service to associate with the component
        /// <para>UnityContainer命名注册, 不命名注册传null</para>
        /// </param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static UnityContainer AddSharpRedisStandalone(this UnityContainer builder, string connectionString, Action<ConnectionOptions>? optionsAction = null, string? serviceName = null)
#else
        public static UnityContainer AddSharpRedisStandalone(this UnityContainer builder, string connectionString, Action<ConnectionOptions> optionsAction = null, string serviceName = null)
#endif
        {
            var redis = Redis.UseStandalone(connectionString, optionsAction);
            
            if (serviceName != null)
            {
                _ = builder.RegisterInstance(serviceName, redis);
            }
            else
            {
                _ = builder.RegisterInstance(redis);
            }
            return builder.RegisterTypes(serviceName);
        }

        /// <summary>
        /// Connect singleton Redis and register with UnityContainer
        /// <para>连接单例Redis, 并注册到UnityContainer</para>
        /// </summary>
        /// <param name="builder">UnityContainer</param>
        /// <param name="serviceName">Named service to associate with the component
        /// <para>UnityContainer命名注册, 不命名注册传null</para>
        /// </param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static UnityContainer AddSharpRedisStandalone(this UnityContainer builder, Action<ConnectionOptions>? optionsAction = null, string? serviceName = null)
#else
        public static UnityContainer AddSharpRedisStandalone(this UnityContainer builder, Action<ConnectionOptions> optionsAction = null, string serviceName = null)
#endif
        {
            return builder.AddSharpRedisStandalone(string.Empty, optionsAction, serviceName);
        }

        /// <summary>
        /// Connect singleton Redis and register with UnityContainer
        /// <para>连接单例Redis, 并注册到UnityContainer</para>
        /// </summary>
        /// <typeparam name="TCache">The local cache implements generics
        /// <para>本地缓存实现泛型</para>
        /// </typeparam>
        /// <param name="builder">UnityContainer</param>
        /// <param name="connectionString">connection string
        /// <para>连接字符串</para>
        /// </param>
        /// <param name="serviceName">Named service to associate with the component
        /// <para>UnityContainer命名注册, 不命名注册传null</para>
        /// </param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static UnityContainer AddSharpRedisStandalone<TCache>(this UnityContainer builder, string connectionString, Action<ConnectionOptions>? optionsAction = null, string? serviceName = null)
#else
        public static UnityContainer AddSharpRedisStandalone<TCache>(this UnityContainer builder, string connectionString, Action<ConnectionOptions> optionsAction = null, string serviceName = null)
#endif
            where TCache : ClientSideCachingStandard
        {
            var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
            _ = builder.RegisterSingleton<ClientSideCachingStandard, TCache>(cacheServiceName);

            if (serviceName == null)
            {
                _ = builder.RegisterFactory<Redis>(c =>
                {
                    var cache = c.Resolve<ClientSideCachingStandard>(cacheServiceName);
                    var redis = Redis.UseStandalone(connectionString, options =>
                    {
                        optionsAction?.Invoke(options);
                        options.SetClientSideCaching(cache);
                    });
                    return redis;
                }, new ContainerControlledLifetimeManager());
            }
            else
            {
                _ = builder.RegisterFactory<Redis>(serviceName, c =>
                {
                    var cache = c.Resolve<ClientSideCachingStandard>(cacheServiceName);
                    var redis = Redis.UseStandalone(connectionString, options =>
                    {
                        optionsAction?.Invoke(options);
                        options.SetClientSideCaching(cache);
                    });
                    return redis;
                }, new ContainerControlledLifetimeManager());
            }

            return builder.RegisterTypes(serviceName);
        }

        /// <summary>
        /// Connect singleton Redis and register with UnityContainer
        /// <para>连接单例Redis, 并注册到UnityContainer</para>
        /// </summary>
        /// <typeparam name="TCache">The local cache implements generics
        /// <para>本地缓存实现泛型</para>
        /// </typeparam>
        /// <param name="builder">UnityContainer</param>
        /// <param name="serviceName">Named service to associate with the component
        /// <para>UnityContainer命名注册, 不命名注册传null</para>
        /// </param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static UnityContainer AddSharpRedisStandalone<TCache>(this UnityContainer builder, Action<ConnectionOptions>? optionsAction = null, string? serviceName = null)
#else
        public static UnityContainer AddSharpRedisStandalone<TCache>(this UnityContainer builder, Action<ConnectionOptions> optionsAction = null, string serviceName = null)
#endif
            where TCache : ClientSideCachingStandard
        {
            return builder.AddSharpRedisStandalone<TCache>(string.Empty, optionsAction, serviceName);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static UnityContainer RegisterTypes(this UnityContainer builder, string? serviceName)
#else
        private static UnityContainer RegisterTypes(this UnityContainer builder, string serviceName)
#endif
        {
            if (serviceName != null)
            {
                _ = builder.RegisterFactory<RedisString>(serviceName, c => c.Resolve<Redis>(serviceName).String, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisHash>(serviceName, c => c.Resolve<Redis>(serviceName).Hash, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisList>(serviceName, c => c.Resolve<Redis>(serviceName).List, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisSet>(serviceName, c => c.Resolve<Redis>(serviceName).Set, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisSortedSet>(serviceName, c => c.Resolve<Redis>(serviceName).SortedSet, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisStream>(serviceName, c => c.Resolve<Redis>(serviceName).Stream, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisBitmap>(serviceName, c => c.Resolve<Redis>(serviceName).Bitmap, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisHyperLogLog>(serviceName, c => c.Resolve<Redis>(serviceName).HyperLogLog, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisGeospatialIndices>(serviceName, c => c.Resolve<Redis>(serviceName).Geospatial, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisKey>(serviceName, c => c.Resolve<Redis>(serviceName).Key, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisConnection>(serviceName, c => c.Resolve<Redis>(serviceName).Connection, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisServer>(serviceName, c => c.Resolve<Redis>(serviceName).Server, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisScript>(serviceName, c => c.Resolve<Redis>(serviceName).Script, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisPubSub>(serviceName, c => c.Resolve<Redis>(serviceName).PubSub, new ContainerControlledLifetimeManager());
            }
            else
            {
                _ = builder.RegisterFactory<RedisString>(c => c.Resolve<Redis>().String, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisHash>(c => c.Resolve<Redis>().Hash, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisList>(c => c.Resolve<Redis>().List, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisSet>(c => c.Resolve<Redis>().Set, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisSortedSet>(c => c.Resolve<Redis>().SortedSet, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisStream>(c => c.Resolve<Redis>().Stream, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisBitmap>(c => c.Resolve<Redis>().Bitmap, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisHyperLogLog>(c => c.Resolve<Redis>().HyperLogLog, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisGeospatialIndices>(c => c.Resolve<Redis>().Geospatial, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisKey>(c => c.Resolve<Redis>().Key, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisConnection>(c => c.Resolve<Redis>().Connection, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisServer>(c => c.Resolve<Redis>().Server, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisScript>(c => c.Resolve<Redis>().Script, new ContainerControlledLifetimeManager());
                _ = builder.RegisterFactory<RedisPubSub>(c => c.Resolve<Redis>().PubSub, new ContainerControlledLifetimeManager());
            }

            return builder;
        }
    }
}
