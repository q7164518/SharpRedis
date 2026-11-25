namespace SharpRedis;

public sealed partial class RedisConnection
{
    /// <summary>
    /// Client-side caching tracking settings for the selected connection.
    /// <para>当前所选连接的客户端缓存追踪设置。</para>
    /// </summary>
    public readonly struct TrackingInfo
    {
        /// <summary>
        /// Tracking flags reported by Redis, including on or off.
        /// <para>Redis 返回的追踪标记，包括表示启停状态的 on 或 off。</para>
        /// </summary>
        public readonly string[] Flags { get; }

        /// <summary>
        /// Client ID for invalidation notifications. Zero denotes this connection and -1 denotes no redirection.
        /// <para>接收失效通知的客户端 ID，0 表示当前连接，-1 表示未设置重定向。</para>
        /// </summary>
        public readonly long Redirect { get; }

        /// <summary>
        /// Key prefixes tracked in broadcast mode. ReturnBytes preserves their original bytes.
        /// <para>广播模式下追踪的 key 前缀，使用 ReturnBytes 可保留前缀的原始字节。</para>
        /// </summary>
        public readonly RedisResult[] Prefixes { get; }

        internal TrackingInfo(RedisResult value)
        {
            string[] flags = [];
            long redirect = -1;
            RedisResult[] prefixes = [];
            RedisServer.AclReplyReader.ReadFields(value, (name, field) =>
            {
                switch (name)
                {
                    case "flags":
                        flags = RedisServer.AclReplyReader.ReadStrings(field);
                        break;
                    case "redirect":
                        redirect = field;
                        break;
                    case "prefixes":
                        prefixes = field.RawValue as RedisResult[] ?? [];
                        break;
                }
            });
            this.Flags = flags;
            this.Redirect = redirect;
            this.Prefixes = prefixes;
        }
    }

    /// <summary>
    /// Server and connection information returned by HELLO.
    /// <para>HELLO 返回的服务端及连接信息。</para>
    /// </summary>
    public readonly struct HelloResult
    {
        /// <summary>
        /// Server name.
        /// <para>服务端名称。</para>
        /// </summary>
        public readonly string Server { get; }

        /// <summary>
        /// Server version.
        /// <para>服务端版本。</para>
        /// </summary>
        public readonly string Version { get; }

        /// <summary>
        /// Negotiated protocol version.
        /// <para>协商后的 RESP 协议版本。</para>
        /// </summary>
        public readonly Resp Protocol { get; }

        /// <summary>
        /// Client connection ID.
        /// <para>客户端连接 ID。</para>
        /// </summary>
        public readonly long Id { get; }

        /// <summary>
        /// Server mode.
        /// <para>服务端运行模式。</para>
        /// </summary>
        public readonly string Mode { get; }

        /// <summary>
        /// Replication role, or an empty string when omitted.
        /// <para>复制角色，服务端省略此字段时为空字符串。</para>
        /// </summary>
        public readonly string Role { get; }

        /// <summary>
        /// Loaded modules.
        /// <para>已加载的模块。</para>
        /// </summary>
        public readonly RedisServer.ModuleInfo[] Modules { get; }

        internal HelloResult(RedisResult value)
        {
            string server = string.Empty, version = string.Empty, mode = string.Empty, role = string.Empty;
            Resp protocol = default;
            long id = 0;
            RedisServer.ModuleInfo[] modules = [];
            RedisServer.AclReplyReader.ReadFields(value, (name, field) =>
            {
                switch (name)
                {
                    case "server": server = (string)field!; break;
                    case "version": version = (string)field!; break;
                    case "proto": protocol = (Resp)(int)field; break;
                    case "id": id = field; break;
                    case "mode": mode = (string)field!; break;
                    case "role": role = (string)field!; break;
                    case "modules": modules = field; break;
                }
            });
            this.Server = server;
            this.Version = version;
            this.Protocol = protocol;
            this.Id = id;
            this.Mode = mode;
            this.Role = role;
            this.Modules = modules;
        }
    }
}
