using SharpRedis.Extensions;

namespace SharpRedis;

/// <summary>
/// Single-instance Redis connection configuration class
/// <para>单机Redis, 连接配置类</para>
/// </summary>
public sealed class StandaloneConnectionOptions : BasalConnectionOptions
{
#if MODERN_NET
    /// <summary>
    /// Redis server password, No password requires no assignment
    /// <para>Redis主机密码, 没有密码不需要设置</para>
    /// </summary>
    public string? Password
    {
        get;

        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }

    /// <summary>
    /// <para>Redis server user, default is null. Available since: 6.0.0+</para>
    /// <para>Redis主机用户名, Redis 6+以上才支持, 默认为空</para>
    /// </summary>
    public string? User
    {
        get;

        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }
#else
    /// <summary>
    /// Redis server password, No password requires no assignment
    /// <para>Redis主机密码, 没有密码不需要设置</para>
    /// </summary>
    public string Password
    {
        get;

        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }

    /// <summary>
    /// <para>Redis server user, default is null. Available since: 6.0.0+</para>
    /// <para>Redis主机用户名, Redis 6+以上才支持, 默认为空</para>
    /// </summary>
    public string User
    {
        get;

        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }
#endif

    public StandaloneConnectionOptions()
    {
    }

    /// <summary>
    /// Create configuration based on the connection string
    /// <para>根据连接字符串创建配置</para>
    /// </summary>
    /// <param name="connectionString">Connection string, such as host=127.0.0.1, port=10086, password=123456
    /// <para>连接字符串, 如host=127.0.0.1,port=10086,password=123456</para>
    /// </param>
    public StandaloneConnectionOptions(string connectionString)
    {
        var options = Utilities.ConnectionToDictionary(connectionString);
        base.Init(options);

        if (options.TryGetValue("password", out var password)) this.Password = password;
        if (options.TryGetValue("user", out var user)) this.User = user;
    }
}
