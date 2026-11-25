using SharpRedis.Pipelines;
using System;

namespace SharpRedis;

public sealed partial class Redis
{
    /// <summary>
    /// Connect to Redis using the single-node mode
    /// <para>Host: 127.0.0.1, Port: 6379, no password</para>
    /// <para>连接到单节点Redis</para>
    /// <para>使用默认127.0.0.1:6379且无密码连接</para>
    /// </summary>
    /// <returns>Redis client</returns>
    public static Redis UseStandalone()
    {
        return Redis.UseStandalone(string.Empty);
    }

    /// <summary>
    /// Connect to Redis using the single-node mode
    /// <para>连接到单节点Redis</para>
    /// </summary>
    /// <param name="connectionString">connection string
    /// <para>连接字符串</para>
    /// </param>
    /// <param name="optionsAction">Configuration method
    /// <para>配置</para>
    /// </param>
    /// <returns>Redis client</returns>
#if MODERN_NET
    public static Redis UseStandalone(string connectionString, Action<StandaloneConnectionOptions>? optionsAction = null)
#else
    public static Redis UseStandalone(string connectionString, Action<StandaloneConnectionOptions> optionsAction = null)
#endif
    {
        StandaloneConnectionOptions connectionOptions;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionOptions = new();
        }
        else
        {
            connectionOptions = new(connectionString);
        }
        optionsAction?.Invoke(connectionOptions);

        RedisContext context = new(RedisServerMode.Standalone);
        connectionOptions.Build(context);
        var redis = new Redis(context);
        return redis;
    }

    /// <summary>
    /// Connect to Redis using the single-node mode
    /// <para>连接到单节点Redis</para>
    /// </summary>
    /// <param name="optionsAction">Configuration method
    /// <para>配置</para>
    /// </param>
    /// <returns>Redis client</returns>
    public static Redis UseStandalone(Action<StandaloneConnectionOptions> optionsAction)
    {
        return Redis.UseStandalone(string.Empty, optionsAction);
    }
}
