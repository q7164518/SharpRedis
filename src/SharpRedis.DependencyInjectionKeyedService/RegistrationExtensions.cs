using Microsoft.Extensions.DependencyInjection;
using System;

namespace SharpRedis.DependencyInjectionKeyedService
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
        /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
        /// <para>命名注册, 不命名注册传null</para>
        /// </param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static IServiceCollection AddSharpRedisStandalone(this IServiceCollection services, string connectionString, Action<ConnectionOptions>? optionsAction = null, string? serviceName = null)
#else
        public static IServiceCollection AddSharpRedisStandalone(this IServiceCollection services, string connectionString, Action<ConnectionOptions> optionsAction = null, string serviceName = null)
#endif
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
        /// Connect singleton Redis and register with Microsoft.Extensions.DependencyInjection ServiceCollection
        /// <para>连接单例Redis, 并注册到Microsoft.Extensions.DependencyInjection ServiceCollection</para>
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
        /// <para>命名注册, 不命名注册传null</para>
        /// </param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static IServiceCollection AddSharpRedisStandalone(this IServiceCollection services, Action<ConnectionOptions> optionsAction, string? serviceName = null)
#else
        public static IServiceCollection AddSharpRedisStandalone(this IServiceCollection services, Action<ConnectionOptions> optionsAction, string serviceName = null)
#endif
        {
            return services.AddSharpRedisStandalone(string.Empty, optionsAction, serviceName);
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
        /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
        /// <para>命名注册, 不命名注册传null</para>
        /// </param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static IServiceCollection AddSharpRedisStandalone<TCache>(this IServiceCollection services, string connectionString, Action<ConnectionOptions>? optionsAction = null, string? serviceName = null)
#else
        public static IServiceCollection AddSharpRedisStandalone<TCache>(this IServiceCollection services, string connectionString, Action<ConnectionOptions> optionsAction = null, string serviceName = null)
#endif
            where TCache : ClientSideCachingStandard
        {
            var cacheServiceName = serviceName ?? Guid.NewGuid().ToString();
            services.AddKeyedSingleton<ClientSideCachingStandard, TCache>(cacheServiceName);

            if (serviceName != null)
            {
                services.AddKeyedSingleton(serviceName, (c, _) =>
                {
                    var cache = c.GetKeyedService<ClientSideCachingStandard>(cacheServiceName)
                        ?? throw new NullReferenceException("Unable to get ClientSideCachingStandard implementation");
                    var redis = Redis.UseStandalone(connectionString, options =>
                    {
                        optionsAction?.Invoke(options);
                        options.SetClientSideCaching(cache);
                    });
                    return redis;
                });
            }
            else
            {
                services.AddSingleton(c =>
                {
                    var cache = c.GetKeyedService<ClientSideCachingStandard>(cacheServiceName)
                        ?? throw new NullReferenceException("Unable to get ClientSideCachingStandard implementation");
                    var redis = Redis.UseStandalone(connectionString, options =>
                    {
                        optionsAction?.Invoke(options);
                        options.SetClientSideCaching(cache);
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
        /// <typeparam name="TCache">The local cache implements generics
        /// <para>本地缓存实现泛型</para>
        /// </typeparam>
        /// <param name="services">IServiceCollection</param>
        /// <param name="serviceName">The <see cref="ServiceDescriptor.ServiceKey"/> of the service
        /// <para>命名注册, 不命名注册传null</para>
        /// </param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static IServiceCollection AddSharpRedisStandalone<TCache>(this IServiceCollection services, Action<ConnectionOptions>? optionsAction = null, string? serviceName = null)
#else
        public static IServiceCollection AddSharpRedisStandalone<TCache>(this IServiceCollection services, Action<ConnectionOptions> optionsAction = null, string serviceName = null)
#endif
            where TCache : ClientSideCachingStandard
        {
            return services.AddSharpRedisStandalone<TCache>(string.Empty, optionsAction, serviceName);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static IServiceCollection RegisterTypes(this IServiceCollection services, string? serviceName)
#else
        private static IServiceCollection RegisterTypes(this IServiceCollection services, string serviceName)
#endif
        {
            if (serviceName != null)
            {
                return services.AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.String ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.Hash ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.List ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.Set ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.SortedSet ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.Stream ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.Bitmap ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.HyperLogLog ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.Script ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.Connection ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.Server ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.Key ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.PubSub ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddKeyedSingleton(serviceName, (c, _) => c.GetKeyedService<Redis>(serviceName)?.Geospatial ?? throw new NullReferenceException("Unable to obtain Redis"));
            }
            else
            {
                return services.AddSingleton(c => c.GetService<Redis>()?.String ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.Hash ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.List ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.Set ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.SortedSet ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.Bitmap ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.HyperLogLog ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.Geospatial ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.Script ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.Connection ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.Key ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.Server ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.PubSub ?? throw new NullReferenceException("Unable to obtain Redis"))
                    .AddSingleton(c => c.GetService<Redis>()?.Stream ?? throw new NullReferenceException("Unable to obtain Redis"));
            }
        }
    }
}
