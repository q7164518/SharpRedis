using SharpRedis.Exceptions;
using SharpRedis.Extensions;
using SharpRedis.Pipelines;
using SharpRedis.Pipelines.Middlewares;
using SharpRedis.Pipelines.Middlewares.ClientCaching;
using SharpRedis.Pipelines.Middlewares.SelectConnection;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SharpRedis;

public abstract class BasalConnectionOptions
{
    /// <summary>
    /// IP AddressFamily
    /// </summary>
    internal AddressFamily AddressFamily
    {
        get;
        private set;
    } = AddressFamily.InterNetwork;

    /// <summary>
    /// Redis server host or an absolute Unix socket path. Defaults to 127.0.0.1.
    /// <para>Redis主机地址, 默认为127.0.0.1</para>
    /// </summary>
    public string Host
    {
        get;
        set
        {
            var host = value?.Trim();
            if (string.IsNullOrEmpty(host)) return;

            this.AddressFamily = ResolveAddressFamily(host);
            field = host;
        }
    } = Config.DEFAULT_HOST;

    internal static AddressFamily ResolveAddressFamily(string host)
    {
        if (IsUnixSocketHost(host))
        {
#if MODERN_NET
            return AddressFamily.Unix;
#else
            throw new PlatformNotSupportedException("Unix domain sockets require the .NET Standard 2.1 or newer SharpRedis build.");
#endif
        }
        if (!IPAddress.TryParse(host, out var ip))
            ip = Dns.GetHostEntry(host).AddressList[0];
        if (ip.AddressFamily != AddressFamily.InterNetwork && ip.AddressFamily != AddressFamily.InterNetworkV6)
            throw new RedisInitializationException($"Redis host: [{host}] is not the correct ipv4 or ipv6 address");
        return ip.AddressFamily;
    }

    /// <summary>
    /// Redis server port, default is 6379
    /// <para>Redis主机端口号, 默认为6379</para>
    /// </summary>
    public int Port
    {
        get;
        set
        {
            if (value > 0 && value < 65535)
                field = value;
        }
    } = Config.DEFAULT_PORT;

    /// <summary>
    /// Set or get the RESP version. Default to using RESP2
    /// <para>设置或获取RESP版本, 默认为RESP2</para>
    /// </summary>
    public Resp RespVersion
    {
        get;
        set
        {
            if (value != Resp.RESP2 && value != Resp.RESP3) throw new RedisInitializationException("The RESP version can only be 2 or 3");
            field = value;
        }
    } = Resp.RESP2;

    /// <summary>
    /// Timeout period of executing the command, default is 60000 milliseconds
    /// <para>When the execution times out, the system does not wait for the result, and a timeout exception is reported</para>
    /// <para>执行命令的超时时间, 单位: 毫秒. 默认为60000毫秒, 对应60秒</para>
    /// <para>当执行超时, 不再等待该结果, 会报超时异常</para>
    /// </summary>
    public int CommandTimeout
    {
        get;

        set
        {
            if (value > 0) field = value;
        }
    } = Config.DEFAULT_COMMAND_TIMEOUT;

    /// <summary>
    /// Default encoding format. The default is utf-8
    /// <para>编码, 默认为utf-8</para>
    /// </summary>
    public Encoding Encoding
    {
        get;
        set;
    } = Encoding.UTF8;

    /// <summary>
    /// Connection name prefix, defalut is empty
    /// <para>连接名称前缀, 默认为空</para>
    /// </summary>
    public string ConnectName
    {
        get;

        set => field = value?.Trim() ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Redis server database, defalut is 0
    /// <para>Redis默认使用的数据库, 默认为0号数据库</para>
    /// </summary>
    public int Database
    {
        get;

        set;
    } = Config.DEFAULT_DATA_BASE;

    /// <summary>
    /// The maximum number of commands that can be sent in each batch. The default value is 80, and the maximum is 300.
    /// <para>每批最多发送的命令数量, 默认为80, 最多为300</para>
    /// </summary>
    public int BatchCount
    {
        get;
        set
        {
            if (value is <= 0 or > 300) field = Config.DEFAULT_BATCH_COUNT;
            else field = value;
        }
    } = Config.DEFAULT_BATCH_COUNT;

    /// <summary>
    /// Data buffer size, default value is 4096kb
    /// <para>数据包缓冲大小, 默认为4096KB</para>
    /// </summary>
    public int BuffSize
    {
        get;
        set
        {
            if (value <= 0) field = Config.DEFAULT_BUFFER_SIZE;
            else field = value;
        }
    } = Config.DEFAULT_BUFFER_SIZE;

#if MODERN_NET
    /// <summary>
    /// Redis Key prefix. All operations involving keys are concatenated with this prefix. Default empty
    /// <para>Redis Key前缀, 所有涉及到Key的操作都会拼接上此前缀. 默认没有</para>
    /// </summary>
    public string? Prefix
    {
        get;
        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }

    /// <summary>
    /// Redis Key suffix. All operations involving keys are concatenated with this suffix. Default empty
    /// <para>Redis Key后缀, 所有涉及到Key的操作都会拼接上此后缀. 默认没有</para>
    /// </summary>
    public string? Suffix
    {
        get;
        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }

    /// <summary>
    /// TLS server name used for certificate verification. Defaults to Host.
    /// <para>用于证书验证的 TLS 服务器名称。默认为 Host</para>
    /// </summary>
    public string? SslHost { get; set; }

    /// <summary>
    /// Optional server certificate validation callback. The default uses system trust and hostname validation.
    /// <para>可选的服务器证书验证回调。默认使用系统信任库和主机名验证。</para>
    /// </summary>
    public RemoteCertificateValidationCallback? CertificateValidation { get; set; }

    /// <summary>
    /// Optional client certificate selection callback for mutual TLS.
    /// <para>用于双向认证 TLS 的可选客户端证书选择回调</para>
    /// </summary>
    public LocalCertificateSelectionCallback? CertificateSelection { get; set; }
#else

    /// <summary>
    /// Redis Key prefix. All operations involving keys are concatenated with this prefix. Default empty
    /// <para>Redis Key前缀, 所有涉及到Key的操作都会拼接上此前缀. 默认没有</para>
    /// </summary>
    public string Prefix
    {
        get;
        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }

    /// <summary>
    /// Redis Key suffix. All operations involving keys are concatenated with this suffix. Default empty
    /// <para>Redis Key后缀, 所有涉及到Key的操作都会拼接上此后缀. 默认没有</para>
    /// </summary>
    public string Suffix
    {
        get;
        set
        {
            if (!string.IsNullOrEmpty(value)) field = value;
        }
    }

    /// <summary>
    /// TLS server name used for certificate verification. Defaults to Host.
    /// <para>用于证书验证的 TLS 服务器名称。默认为 Host</para>
    /// </summary>
    public string SslHost { get; set; }

    /// <summary>
    /// Optional server certificate validation callback. The default uses system trust and hostname validation.
    /// <para>可选的服务器证书验证回调。默认使用系统信任库和主机名验证。</para>
    /// </summary>
    public RemoteCertificateValidationCallback CertificateValidation { get; set; }

    /// <summary>
    /// Optional client certificate selection callback for mutual TLS.
    /// <para>用于双向认证 TLS 的可选客户端证书选择回调</para>
    /// </summary>
    public LocalCertificateSelectionCallback CertificateSelection { get; set; }
#endif

    /// <summary>
    /// The interval time of heartbeat packets, measured in milliseconds. The default value is 60000 milliseconds.
    /// <para>心跳包检测间隔时间, 单位: 毫秒. 默认为60000毫秒</para>
    /// </summary>
    public int HeartbeatInterval
    {
        get;
        set => field = value > 0 ? value : Config.DEFAULT_HEARTBEAT_INTERVAL;
    } = Config.DEFAULT_HEARTBEAT_INTERVAL;

    /// <summary>
    /// The number of milliseconds for the interval of reconnection in case of a connection interruption. The default value is 5000 milliseconds.
    /// <para>如果连接断开, 重新连接的间隔, 单位: 毫秒. 默认为5000</para>
    /// </summary>
    public int ReconnectInterval
    {
        get;
        set => field = value > 0 ? value : Config.DEFAULT_RECONNECTION_INTERVAL;
    } = Config.DEFAULT_RECONNECTION_INTERVAL;

    /// <summary>
    /// Use TLS for the Redis connection. Defaults to false.
    /// <para>是否启用TLS连接, 默认不启用</para>
    /// </summary>
    public bool Ssl { get; set; }

    /// <summary>Check the server certificate for revocation during TLS authentication.</summary>
    public bool CheckCertificateRevocation { get; set; }

    /// <summary>
    /// Client certificates for mutual TLS. Keep their private keys available for the lifetime of the Redis client.
    /// <para>用于双向TLS的客户端证书。在整个Redis客户端生命周期内，保持其私钥可用。</para>
    /// </summary>
    public X509CertificateCollection ClientCertificates => this.ConfiguredCertificates ??= [];

#nullable enable
    internal X509CertificateCollection? ConfiguredCertificates { get; private set; }
#nullable restore

    internal static bool IsUnixSocketHost(string host)
        => host.Length > 0 && (host[0] == '/' ||
            (host.Length >= 3 && char.IsLetter(host[0]) && host[1] == ':' && (host[2] == '\\' || host[2] == '/')));


    public void Init(Dictionary<string, string> options)
    {
        if (options.TryGetValue("connectname", out var connectname)) this.ConnectName = connectname;
        if (options.TryGetValue("prefix", out var prefix)) this.Prefix = prefix;
        if (options.TryGetValue("suffix", out var suffix)) this.Suffix = suffix;
        if (options.TryGetValue("unixsocket", out var unixSocket))
        {
            if (!IsUnixSocketHost(unixSocket.Trim()))
                throw new ArgumentException("The unixsocket option requires an absolute path.");
            this.Host = unixSocket;
        }
        else if (options.TryGetValue("host", out var host)) this.Host = host;
        if (options.TryGetValue("port", out var portString) && int.TryParse(portString.Trim(), out var port)) this.Port = port;
        if (options.TryGetValue("heartbeatinterval", out var heartbeatIntervalString) && int.TryParse(heartbeatIntervalString.Trim(), out var heartbeatInterval)) this.HeartbeatInterval = heartbeatInterval;
        if (options.TryGetValue("reconnectinterval", out var reconnectIntervalString) && int.TryParse(reconnectIntervalString.Trim(), out var reconnectInterval)) this.ReconnectInterval = reconnectInterval;

        if (options.TryGetValue("resp", out var respString) && int.TryParse(respString, out var resp))
        {
            this.RespVersion = (Resp)resp;
        }

        if (options.TryGetValue("encoding", out var encoding))
        {
            encoding = encoding.Trim();
            if (!"utf-8".Equals(encoding, StringComparison.OrdinalIgnoreCase)
                && !"utf8".Equals(encoding, StringComparison.OrdinalIgnoreCase))
            {
                this.Encoding = Encoding.GetEncoding(encoding);
            }
        }

        if (options.TryGetValue("database", out var defaultdatabaseString) && int.TryParse(defaultdatabaseString, out var defaultDatabase))
        {
            this.Database = defaultDatabase;
        }

        if (options.TryGetValue("commandtimeout", out var commandTimeoutString) && int.TryParse(commandTimeoutString, out var commandTimeout))
        {
            this.CommandTimeout = commandTimeout;
        }

        if (options.TryGetValue("ssl", out var ssl))
        {
            if (!bool.TryParse(ssl, out var enabled))
                throw new ArgumentException("The ssl option must be true or false.");
            this.Ssl = enabled;
        }
        if (options.TryGetValue("checkcertificaterevocation", out var revocation))
            this.CheckCertificateRevocation = bool.Parse(revocation);
        if (options.TryGetValue("sslhost", out var sslHost)) this.SslHost = sslHost.Trim();
    }

    private Dictionary<string, IMiddleware> _middlewares = [with(8)];

    internal void Build(RedisContext redisContext)
    {
        redisContext.RedisOptions = this;

        #region Initialize the middleware
        this.RemoveAndAddMiddleware(ExceptionMiddleware.NAME, new ExceptionMiddleware());
        this.RemoveAndAddMiddleware(ExecuteCommandMiddleware.NAME, new ExecuteCommandMiddleware(redisContext));

        if (this._clientCaching is not null)
        {
            switch (this._clientCaching.Mode)
            {
                case ClientCachingBase.ClientCachingMode.Default:
                    this.RemoveAndAddMiddleware(
                        ClientCachingDefaultMiddleware.NAME,
                        new ClientCachingDefaultMiddleware(
                            redisContext,
                            this._clientCaching
                        )
                    );
                    break;
                case ClientCachingBase.ClientCachingMode.Broadcast:
                    this.RemoveAndAddMiddleware(
                        ClientCachingBroadcastMiddleware.NAME,
                        new ClientCachingBroadcastMiddleware(
                            redisContext,
                            this._clientCaching
                        )
                    );
                    break;
                default:
                    break;
            }
        }

        switch (redisContext.RedisMode)
        {
            case RedisServerMode.Standalone:
                this.RemoveAndAddMiddleware(StandaloneSelectConnectionMiddleware.NAME, new StandaloneSelectConnectionMiddleware(redisContext));
                break;
            case RedisServerMode.MasterSlave:
                this.RemoveAndAddMiddleware(MasterSlaveSelectConnectionMiddleware.NAME, new MasterSlaveSelectConnectionMiddleware(redisContext));
                break;
            case RedisServerMode.Sentinel:
                this.RemoveAndAddMiddleware(MasterSlaveSelectConnectionMiddleware.NAME, new MasterSlaveSelectConnectionMiddleware(redisContext));
                break;
            case RedisServerMode.Cluster:
                this.RemoveAndAddMiddleware(ClusterSelectConnectionMiddleware.NAME, new ClusterSelectConnectionMiddleware(redisContext));
                break;
            default:
                break;
        }
        #endregion

        redisContext.Build(this._middlewares, this._clientCaching);
        this._middlewares = null!;
        this._clientCaching = null!;
    }

    private void RemoveAndAddMiddleware(string middlewareName, IMiddleware middleware)
    {
#if NETSTANDARD2_0
        if (this._middlewares.TryGetValue(middlewareName, out var old))
        {
            old.Dispose();
            _ = this._middlewares.Remove(middlewareName);
        }
#else
        if (this._middlewares.Remove(middlewareName, out var old))
        {
            old.Dispose();
        }
#endif
        this._middlewares[middlewareName] = middleware;
    }

    #region ClientCaching
#if MODERN_NET
    private ClientCachingBase? _clientCaching;
#else
    private ClientCachingBase _clientCaching;
#endif

    /// <summary>
    /// Enable client caching in the broadcast mode and specify the Key prefix to be cached.
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>在广播模式下启用客户端缓存，并指定要缓存的Key前缀。</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <param name="keyPrefixes">Key prefixes to cache.
    /// <para>要缓存的 Key 前缀。</para>
    /// </param>
    /// <param name="expiration">Positive cache lifetime measured from the last cache write. Cache hits do not extend it.
    /// <para>本地缓存有效期，须为正数。从最近一次写入本地缓存开始计时，命中时不续期。</para>
    /// </param>
    public BasalConnectionOptions UseClientCaching(string[] keyPrefixes, TimeSpan expiration)
        => this.UseClientCaching(DefaultClientCaching.Create(ClientCachingBase.ClientCachingMode.Broadcast, keyPrefixes, null, expiration));

    /// <summary>
    /// Enable client caching in the default mode
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>在默认模式下启用客户端缓存功能</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <param name="matching">Predicate that selects keys to cache.
    /// <para>要缓存的 Key 匹配规则。</para>
    /// </param>
    /// <param name="expiration">Positive cache lifetime measured from the last cache write. Cache hits do not extend it.
    /// <para>本地缓存有效期，须为正数。从最近一次写入本地缓存开始计时，命中时不续期。</para>
    /// </param>
    public BasalConnectionOptions UseClientCaching(Func<string, bool> matching, TimeSpan expiration)
        => this.UseClientCaching(DefaultClientCaching.Create(ClientCachingBase.ClientCachingMode.Default, null, matching, expiration));

    /// <summary>
    /// Enable custom client cache
    /// <para>Redis 6.0 and above versions are required to support client-side caching.</para>
    /// <para>启用自定义客户端缓存</para>
    /// <para>需要使用 Redis 6.0 及更高版本，以支持客户端缓存功能。</para>
    /// </summary>
    /// <param name="clientCaching"></param>
    /// <returns></returns>
    public BasalConnectionOptions UseClientCaching(ClientCachingBase clientCaching)
    {
        this._clientCaching?.Dispose();
        this._clientCaching = clientCaching;
        return this;
    }
    #endregion
}
