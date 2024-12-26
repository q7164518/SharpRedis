#pragma warning disable IDE0130
using SharpRedis.Extensions;
using System.Net;
using System.Net.Sockets;
using System;

namespace SharpRedis
{
    public sealed class ConnectionOptions
    {
        private int _respVersion = 2;
        /// <summary>
        /// Redis RESP version. 2 or 3, Redis 6.0 and later supports RESP 3
        /// <para>RESP协议, 2或3. Redis 6.0及以上版本才支持RESP 3</para>
        /// </summary>
        public int RespVersion
        {
            get => this._respVersion;

            set
            {
                if (value != 2 && value != 3) throw new ArgumentException("The RESP version can only be 2 or 3", nameof(RespVersion));
                this._respVersion = value;
            }
        }

        private AddressFamily _addressFamily = AddressFamily.InterNetwork;
        /// <summary>
        /// IP AddressFamily
        /// </summary>
        public AddressFamily AddressFamily
        {
            get => this._addressFamily;
        }

        private string _host = "127.0.0.1";
        /// <summary>
        /// Redis server host
        /// <para>Redis主机地址</para>
        /// </summary>
        public string Host
        {
            get => this._host;

            set
            {
                var host = value?.Trim();
                if (string.IsNullOrEmpty(host)) return;

                if (!IPAddress.TryParse(host, out var ip))
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(host);
                    ip = hostEntry.AddressList[0];
                }
                if (ip.AddressFamily != AddressFamily.InterNetwork && ip.AddressFamily != AddressFamily.InterNetworkV6)
                {
                    throw new RedisInitializationException($"Redis host: [{host}] is not the correct ipv4 or ipv6 address");
                }
                this._host = host;
                this._addressFamily = ip.AddressFamily;
            }
        }

        private int _port = 6379;
        /// <summary>
        /// Redis server port, default is 6379
        /// <para>Redis主机端口号, 默认为6379</para>
        /// </summary>
        public int Port
        {
            get => this._port;

            set
            {
                if (value > 0 && value < 65536) this._port = value;
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private string? _password;
        /// <summary>
        /// Redis server password, No password requires no assignment
        /// <para>Redis主机密码, 没有密码不需要设置</para>
        /// </summary>
        public string? Password
        {
            get => this._password;

            set => this._password = value?.Trim();
        }

        private string? _user;
        /// <summary>
        /// <para>Redis server user, default is null. Available since: 6.0.0</para>
        /// <para>Redis主机用户名, Redis 6+以上才支持, 默认为空</para>
        /// </summary>
        public string? User
        {
            get => this._user;

            set => this._user = value?.Trim();
        }

        private string? _prefix;
        /// <summary>
        /// Redis Key prefix. All operations involving keys are concatenated with this prefix. Default none
        /// <para>Redis Key前缀, 所有涉及到Key的操作都会拼接上此前缀. 默认没有</para>
        /// </summary>
        public string? Prefix
        {
            set => this._prefix = value?.Trim();

            get => this._prefix;
        }
#else
        private string _password;
        /// <summary>
        /// Redis server password, No password requires no assignment
        /// <para>Redis主机密码, 没有密码不需要设置</para>
        /// </summary>
        public string Password
        {
            get => this._password;

            set => this._password = value?.Trim();
        }

        private string _user;
        /// <summary>
        /// <para>Redis server user, default is null. Available since: 6.0.0</para>
        /// <para>Redis主机用户名, Redis 6+以上才支持, 默认为空</para>
        /// </summary>
        public string User
        {
            get => this._user;

            set => this._user = value?.Trim();
        }

        private string _prefix;
        /// <summary>
        /// Redis Key prefix. All operations involving keys are concatenated with this prefix. Default none
        /// <para>Redis Key前缀, 所有涉及到Key的操作都会拼接上此前缀. 默认没有</para>
        /// </summary>
        public string Prefix
        {
            set => this._prefix = value?.Trim();

            get => this._prefix;
        }
#endif

        private string _connectName = string.Empty;
        /// <summary>
        /// Connection name prefix, defalut is empty
        /// <para>连接名称前缀, 默认为空</para>
        /// </summary>
        public string ConnectName
        {
            get => this._connectName;

            set
            {
                var connectName = value?.Trim();
                if (!string.IsNullOrEmpty(connectName)) this._connectName = connectName;
            }
        }

        private ushort _defaultDatabase = 0;
        /// <summary>
        /// Redis server database, defalut is 0
        /// <para>Redis默认使用的数据库, 默认为0号数据库</para>
        /// </summary>
        public ushort DefaultDatabase
        {
            get => this._defaultDatabase;

            set
            {
                if (value > 15 || value < 0) throw new RedisInitializationException("The database supports only 0 to 15");
                this._defaultDatabase = value;
            }
        }

        private ushort _minPoolSize = 3;
        /// <summary>
        /// Connection min pool size, default is 3
        /// <para>连接池最小活跃连接, 默认为3, 不建议设置太大或太小, 根据项目自身情况合理设置</para>
        /// </summary>
        public ushort MinPoolSize
        {
            get => this._minPoolSize;

            set
            {
                if (value > 0) this._minPoolSize = value;
            }
        }

        private ushort _maxPoolSize = 100;
        /// <summary>
        /// Connection max pool size, default is 100
        /// <para>连接池最大活跃连接, 默认为100, 不建议设置太大或太小, 根据项目自身情况合理设置</para>
        /// </summary>
        public ushort MaxPoolSize
        {
            get => this._maxPoolSize;

            set
            {
                if (value > 0) this._maxPoolSize = value;
            }
        }

        private int _commandTimeout = 1000 * 60;
        /// <summary>
        /// Timeout period of executing the command, default is 60000 milliseconds
        /// <para>When the execution times out, the system does not wait for the result, and a timeout exception is reported</para>
        /// <para>执行命令的超时时间, 单位: 毫秒. 默认为60000毫秒, 对应60秒</para>
        /// <para>当执行超时, 不再等待该结果, 会报超时异常</para>
        /// </summary>
        public int CommandTimeout
        {
            get => this._commandTimeout;

            set
            {
                if (value > 0) this._commandTimeout = value;
            }
        }

        private int _subConcurrency = 5;
        /// <summary>
        /// Maximum number of subscriptions per connection, default is 5
        /// <para>If you exceed this number of subscriptions, a new connection is created</para>
        /// <para>单个连接最大订阅数, 默认为5个</para>
        /// <para>如果超过这个数量的订阅, 会新建连接</para>
        /// </summary>
        public int SubConcurrency
        {
            get => this._subConcurrency;

            set
            {
                if (value > 0) this._subConcurrency = value;
            }
        }

        private int _idleTimeout = 1000 * 30;
        /// <summary>
        /// Idle time of elements in the connection pool, default is 30000 milliseconds
        /// <para>If the connection is inactive after this time, the connection is closed</para>
        /// <para>空闲释放连接的时间, 单位: 毫秒, 默认为30000, 30秒</para>
        /// <para>超过这个时间不活跃的连接将会被关闭</para>
        /// </summary>
        public int IdleTimeout
        {
            get => this._idleTimeout;

            set
            {
                if (value > 0) this._idleTimeout = value;
            }
        }

        private string _encoding = "utf-8";
        /// <summary>
        /// String character set. The default is utf-8
        /// <para>字符串字符集, 默认为utf-8</para>
        /// </summary>
        public string Encoding
        {
            get => this._encoding;

            set
            {
                var encoding = value?.Trim();
                if (string.IsNullOrEmpty(encoding)) return;
                _ = System.Text.Encoding.GetEncoding(encoding);
                this._encoding = encoding;
            }
        }

        private int _buffer = 4096;
        /// <summary>
        /// Memory stream buffer size, default 4096(4kb)
        /// <para>If you read Redis values that are frequently greater than 4kb, it is recommended to adjust this value. Otherwise MemoryStream may resize buffers frequently, affecting performance</para>
        /// <para>内存流缓冲区大小, 默认为4096(4kb). 如果你读取的Redis值经常大于4kb, 建议调整此值, 否则MemoryStream可能会频繁调整缓冲区大小，影响性能</para>
        /// </summary>
        public int Buffer
        {
            get => this._buffer;

            set => this._buffer = value;
        }

        internal ConnectionOptions()
        {
        }

        internal ConnectionOptions(string connectionString)
        {
            var options = Extend.ConnectionToDictionary(connectionString);

            if (options.TryGetValue("host", out var host)) this.Host = host;
            if (options.TryGetValue("port", out var portString) && int.TryParse(portString.Trim(), out var port)) this.Port = port;
            if (options.TryGetValue("password", out var password)) this.Password = password;
            if (options.TryGetValue("user", out var user)) this.User = user;
            if (options.TryGetValue("encoding", out var encoding)) this.Encoding = encoding;
            if (options.TryGetValue("connectname", out var connectname)) this.ConnectName = connectname;
            if (options.TryGetValue("prefix", out var prefix)) this._prefix = prefix;

            if (options.TryGetValue("defaultdatabase", out var defaultdatabaseString) && ushort.TryParse(defaultdatabaseString, out var defaultDatabase))
            {
                this.DefaultDatabase = defaultDatabase;
            }
            if (options.TryGetValue("maxpoolsize", out var maxPoolsizeString) && ushort.TryParse(maxPoolsizeString, out var maxPoolsize))
            {
                this.MaxPoolSize = maxPoolsize;
            }
            if (options.TryGetValue("minpoolsize", out var minPoolsizeString) && ushort.TryParse(minPoolsizeString, out var minPoolsize))
            {
                this.MinPoolSize = minPoolsize;
            }
            if (options.TryGetValue("commandtimeout", out var commandTimeoutString) && int.TryParse(commandTimeoutString, out var commandTimeout))
            {
                this.CommandTimeout = commandTimeout;
            }
            if (options.TryGetValue("idletimeout", out var idletimeoutString) && int.TryParse(idletimeoutString, out var idletimeout))
            {
                this.IdleTimeout = idletimeout;
            }
            if (options.TryGetValue("subconcurrency", out var subConcurrencyString) && int.TryParse(subConcurrencyString, out var subConcurrency))
            {
                this.SubConcurrency = subConcurrency;
            }
            if (options.TryGetValue("resp", out var respString) && int.TryParse(respString, out var resp))
            {
                this.RespVersion = resp;
            }
            if (options.TryGetValue("buffer", out var bufferString) && int.TryParse(bufferString, out var buffer))
            {
                if (buffer > 0) this._buffer = buffer;
            }
            if (this.MaxPoolSize < this.MinPoolSize) this.MaxPoolSize = this.MinPoolSize;
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private ClientSideCachingStandard? _clientSideCaching;
#else
        private ClientSideCachingStandard _clientSideCaching;
#endif
        /// <summary>
        /// Enable local cache support
        /// <para>开启本地缓存支持</para>
        /// </summary>
        /// <param name="clientSideCaching">ClientSideCachingStandard</param>
        /// <returns></returns>
        public ConnectionOptions SetClientSideCaching(ClientSideCachingStandard clientSideCaching)
        {
            this._clientSideCaching = clientSideCaching;
            return this;
        }

        internal void UseClientSideCaching(Redis redis)
        {
            if (this._clientSideCaching != null)
            {
                redis.UseClientSideCaching(this._clientSideCaching);
            }
            this._clientSideCaching = null;
        }
    }
}
