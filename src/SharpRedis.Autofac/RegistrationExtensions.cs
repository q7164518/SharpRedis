using Autofac;
using Autofac.Features.AttributeFilters;
using System;

namespace SharpRedis.Autofac
{
    /// <summary>
    /// SharpRedis Autofac Extensions
    /// </summary>
    public static class RegistrationExtensions
    {
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static ContainerBuilder AddSharpRedisStandalone(this ContainerBuilder builder, string connectionString, Action<ConnectionOptions>? optionsAction = null, string? serviceName = null)
#else
        public static ContainerBuilder AddSharpRedisStandalone(this ContainerBuilder builder, string connectionString, Action<ConnectionOptions> optionsAction = null, string serviceName = null)
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static ContainerBuilder AddSharpRedisStandalone(this ContainerBuilder builder, Action<ConnectionOptions> optionsAction, string? serviceName = null)
#else
        public static ContainerBuilder AddSharpRedisStandalone(this ContainerBuilder builder, Action<ConnectionOptions> optionsAction, string serviceName = null)
#endif
        {
            return builder.AddSharpRedisStandalone(string.Empty, optionsAction, serviceName);
        }

        /// <summary>
        /// Connect to singleton Redis, enable local cache support, and register with Autofac
        /// <para>连接单例Redis, 开启本地缓存支持, 并注册到Autofac</para>
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
        public static ContainerBuilder AddSharpRedisStandalone<TCache>(this ContainerBuilder builder, string connectionString, Action<ConnectionOptions>? optionsAction = null, string? serviceName = null)
#else
        public static ContainerBuilder AddSharpRedisStandalone<TCache>(this ContainerBuilder builder, string connectionString, Action<ConnectionOptions> optionsAction = null, string serviceName = null)
#endif
            where TCache : ClientSideCachingStandard
        {
            var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
            var cacheBuilder = builder.RegisterType<TCache>()
                .WithAttributeFiltering()
                .SingleInstance()
                .Named<ClientSideCachingStandard>(cacheServiceName);

            var redisBuilder = builder.Register(c =>
            {
                var cache = c.ResolveNamed<ClientSideCachingStandard>(cacheServiceName);
                var redis = Redis.UseStandalone(connectionString, options =>
                {
                    optionsAction?.Invoke(options);
                    options.SetClientSideCaching(cache);
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
        /// <para>连接单例Redis, 开启本地缓存支持, 并注册到Autofac</para>
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static ContainerBuilder AddSharpRedisStandalone<TCache>(this ContainerBuilder builder, Action<ConnectionOptions> optionsAction, string? serviceName = null)
#else
        public static ContainerBuilder AddSharpRedisStandalone<TCache>(this ContainerBuilder builder, Action<ConnectionOptions> optionsAction, string serviceName = null)
#endif
            where TCache : ClientSideCachingStandard
        {
            return builder.AddSharpRedisStandalone<TCache>(string.Empty, optionsAction, serviceName);
        }

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
                if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).Geospatial;
                return c.Resolve<Redis>().Geospatial;
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

            var pubsubBuilder = builder.Register(c =>
            {
                if (serviceName != null) return c.ResolveNamed<Redis>(serviceName).PubSub;
                return c.Resolve<Redis>().PubSub;
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
                geoBuilder.Named<RedisGeospatialIndices>(serviceName);
                keyBuilder.Named<RedisKey>(serviceName);
                connectionBuilder.Named<RedisConnection>(serviceName);
                serverBuilder.Named<RedisServer>(serviceName);
                scriptBuilder.Named<RedisScript>(serviceName);
                pubsubBuilder.Named<RedisPubSub>(serviceName);
            }
            return builder;
        }
    }
}
