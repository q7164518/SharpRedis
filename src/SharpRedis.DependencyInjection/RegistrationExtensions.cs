using Microsoft.Extensions.DependencyInjection;
using System;

namespace SharpRedis.DependencyInjection
{
    /// <summary>
    /// SharpRedis Microsoft.Extensions.DependencyInjection Extensions
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Connect singleton Redis and register with Microsoft.Extensions.DependencyInjection ServiceCollection
        /// <para>连接单例Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="connectionString">connection string
        /// <para>连接字符串</para>
        /// </param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static IServiceCollection AddSharpRedisStandalone(this IServiceCollection services, string connectionString, Action<ConnectionOptions>? optionsAction = null)
#else
        public static IServiceCollection AddSharpRedisStandalone(this IServiceCollection services, string connectionString, Action<ConnectionOptions> optionsAction = null)
#endif
        {
            var redis = Redis.UseStandalone(connectionString, optionsAction);

            return services.AddSingleton(redis)
                .RegisterTypes();
        }

        /// <summary>
        /// Connect singleton Redis and register with Microsoft.Extensions.DependencyInjection ServiceCollection
        /// <para>连接单例Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
        public static IServiceCollection AddSharpRedisStandalone(this IServiceCollection services, Action<ConnectionOptions> optionsAction)
        {
            return services.AddSharpRedisStandalone(string.Empty, optionsAction);
        }

        /// <summary>
        /// Connect singleton Redis and register with Microsoft.Extensions.DependencyInjection ServiceCollection
        /// <para>连接单例Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
        /// </summary>
        /// <typeparam name="TCache">The local cache implements generics
        /// <para>本地缓存实现泛型</para>
        /// </typeparam>
        /// <param name="services">IServiceCollection</param>
        /// <param name="connectionString">connection string
        /// <para>连接字符串</para>
        /// </param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static IServiceCollection AddSharpRedisStandalone<TCache>(this IServiceCollection services, string connectionString, Action<ConnectionOptions>? optionsAction = null)
#else
        public static IServiceCollection AddSharpRedisStandalone<TCache>(this IServiceCollection services, string connectionString, Action<ConnectionOptions> optionsAction = null)
#endif
            where TCache : ClientSideCachingStandard
        {
            return services.AddSingleton<ClientSideCachingStandard, TCache>()
                .AddSingleton(c =>
                {
                    var cache = c.GetService<ClientSideCachingStandard>();
                    var redis = Redis.UseStandalone(connectionString, options =>
                    {
                        optionsAction?.Invoke(options);
                        options.SetClientSideCaching(cache);
                    });
                    return redis;
                })
                .RegisterTypes();
        }

        /// <summary>
        /// Connect singleton Redis and register with Microsoft.Extensions.DependencyInjection ServiceCollection
        /// <para>连接单例Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
        /// </summary>
        /// <typeparam name="TCache">The local cache implements generics
        /// <para>本地缓存实现泛型</para>
        /// </typeparam>
        /// <param name="services">IServiceCollection</param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
        public static IServiceCollection AddSharpRedisStandalone<TCache>(this IServiceCollection services, Action<ConnectionOptions> optionsAction)
            where TCache : ClientSideCachingStandard
        {
            return services.AddSharpRedisStandalone<TCache>(string.Empty, optionsAction);
        }

        private static IServiceCollection RegisterTypes(this IServiceCollection services)
        {
            return services.AddSingleton(c => c.GetService<Redis>().String)
                .AddSingleton(c => c.GetService<Redis>().Hash)
                .AddSingleton(c => c.GetService<Redis>().List)
                .AddSingleton(c => c.GetService<Redis>().Set)
                .AddSingleton(c => c.GetService<Redis>().SortedSet)
                .AddSingleton(c => c.GetService<Redis>().Bitmap)
                .AddSingleton(c => c.GetService<Redis>().HyperLogLog)
                .AddSingleton(c => c.GetService<Redis>().Geospatial)
                .AddSingleton(c => c.GetService<Redis>().Script)
                .AddSingleton(c => c.GetService<Redis>().Connection)
                .AddSingleton(c => c.GetService<Redis>().Key)
                .AddSingleton(c => c.GetService<Redis>().Server)
                .AddSingleton(c => c.GetService<Redis>().PubSub)
                .AddSingleton(c => c.GetService<Redis>().Stream);
        }
    }
}
