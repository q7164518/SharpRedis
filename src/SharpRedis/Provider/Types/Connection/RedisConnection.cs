using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis connection operation class
/// <para>Redis连接操作类</para>
/// </summary>
public sealed partial class RedisConnection : RedisTypeBase
{
    /// <summary>
    /// Specifies the reply sent to the blocked client.
    /// <para>指定被解除阻塞的客户端收到的响应。</para>
    /// </summary>
    public enum ClientUnblockMode : byte
    {
        /// <summary>Omit the option and use the Redis default (TIMEOUT).
        /// <para>省略该参数，使用 Redis 默认的 TIMEOUT 行为。</para>
        /// </summary>
        None,

        /// <summary>Complete the blocked command as if it had timed out.
        /// <para>让被阻塞的命令按超时完成。</para>
        /// </summary>
        Timeout,

        /// <summary>Return an UNBLOCKED error to the blocked client.
        /// <para>向被阻塞的客户端返回 UNBLOCKED 错误。</para>
        /// </summary>
        Error,
    }

    #region Enums
    /// <summary>
    /// Selects which client commands Redis pauses. Available since Redis 6.2.0.
    /// <para>指定暂停执行的客户端命令范围，Redis 6.2.0 起支持。</para>
    /// </summary>
    public enum ClientPauseMode : byte
    {
        /// <summary>
        /// Pause write commands and allow reads.
        /// <para>暂停写命令，允许读命令继续执行。</para>
        /// </summary>
        Write,

        /// <summary>
        /// Pause both read and write commands.
        /// <para>暂停读命令和写命令。</para>
        /// </summary>
        All,
    }

    public enum SetInfoCategory : byte
    {
        /// <summary>
        /// Meant to hold the name of the client library that's in use.
        /// <para>设置当前客户端库的名称</para>
        /// </summary>
        LibName,

        /// <summary>
        /// Meant to hold the client library's version.
        /// <para>设置当前客户端库的版本</para>
        /// </summary>
        LibVersion,
    }
    #endregion

    /// <summary>
    /// Client connection type for the TYPE filter.
    /// <para>TYPE 过滤条件使用的客户端连接类型。</para>
    /// </summary>
    public enum ClientType : byte
    {
        /// <summary>
        /// Normal client.
        /// <para>普通连接。</para>
        /// </summary>
        Normal,

        /// <summary>
        /// Upstream primary (Redis 3.2+).
        /// <para>上游主节点连接（Redis 3.2+）。</para>
        /// </summary>
        Master,

        /// <summary>
        /// Replica using the legacy SLAVE spelling.
        /// <para>从节点连接，使用兼容旧版本的 SLAVE。</para>
        /// </summary>
        Slave,

        /// <summary>
        /// Replica (Redis 5.0+).
        /// <para>从节点连接，使用 REPLICA（Redis 5.0+）。</para>
        /// </summary>
        Replica,

        /// <summary>
        /// Pub/Sub client.
        /// <para>发布订阅连接。</para>
        /// </summary>
        PubSub,
    }

    /// <summary>
    /// Filters for CLIENT KILL. Specify at least one. A client must match every supplied filter.
    /// <para>CLIENT KILL 的过滤条件。至少设置一项，只有同时满足所有条件的连接才会被关闭。</para>
    /// </summary>
    public struct KillFilter
    {
        /// <summary>
        /// Match the client ID.
        /// <para>匹配客户端 ID。</para>
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Match the client connection type.
        /// <para>匹配客户端连接类型。</para>
        /// </summary>
        public ClientType? Type { get; set; }

        /// <summary>
        /// Match the authenticated ACL username (Redis 6.0+).
        /// <para>匹配客户端认证的 ACL 用户名（Redis 6.0+）。</para>
        /// </summary>
#if MODERN_NET
        public string? User { get; set; }
#else
        public string User { get; set; }
#endif

        /// <summary>
        /// Match the remote client address (ADDR), including its port.
        /// <para>匹配客户端远端地址和端口，对应 ADDR。</para>
        /// </summary>
#if MODERN_NET
        public string? Address { get; set; }
#else
        public string Address { get; set; }
#endif

        /// <summary>
        /// Match the local server address (LADDR), including its port (Redis 6.2+).
        /// <para>匹配服务端本地地址和端口，对应 LADDR（Redis 6.2+）。</para>
        /// </summary>
#if MODERN_NET
        public string? LocalAddress { get; set; }
#else
        public string LocalAddress { get; set; }
#endif

        /// <summary>
        /// Skip the connection issuing the command. Redis defaults to YES when omitted.
        /// <para>是否跳过执行命令的连接，省略时 Redis 默认为 YES。</para>
        /// </summary>
        public bool? SkipMe { get; set; }

        /// <summary>
        /// Filter by connection age in seconds (Redis 7.4+).
        /// <para>按连接存续时间过滤，单位为秒，对应 MAXAGE（Redis 7.4+）。</para>
        /// </summary>
        public long? MaxAge { get; set; }
    }

    internal RedisConnection(BaseRedis redis)
        : base(redis)
    { }

    /// <summary>
    /// Use password verification to authenticate Redis connection identity
    /// <para>Available since: 1.0.0</para>
    /// <para>使用密码验证 Redis 连接身份</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="password">The password to authenticate with. With requirepass, this is the server password; with ACLs, it is the user's password.
    /// <para>认证密码</para>
    /// </param>
    /// <param name="username">The ACL username to authenticate as. If omitted, the default user is used. <b>Starting with Redis version 6.0.0</b>
    /// <para>ACL 用户名（Redis 6.0+）</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/auth">Command documentation</see>
    /// <para><see href="https://redis.io/commands/auth">命令文档</see></para>
    /// </remarks>
    public bool Auth(
#if MODERN_NET
        string? password,
        string? username,
#else
        string password,
        string username,
#endif
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.Auth(username, password) | flags, cancellationToken);

    /// <summary>
    /// This command returns the client ID we are redirecting our tracking notifications to
    /// <para>Available since: 6.0.0</para>
    /// <para>返回当前客户端的重定向客户端 ID</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>0 when not redirecting notifications to any client.
    /// <para>-1 if client tracking is not enabled.</para>
    /// <para>the ID of the client to which notification are being redirected.</para>
    /// <para>-1：未启用客户端缓存重定向. 0：重定向到当前连接自身（默认）. 否则: 重定向到指定客户端 ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-getredir">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-getredir">命令文档</see></para>
    /// </remarks>
    public long ClientGetRedir(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.ClientGetRedir() | flags, cancellationToken);

    /// <summary>
    /// The CLIENT GETNAME returns the name of the current connection as set by CLIENT SETNAME. Since every new connection starts without an associated name, if no name was assigned nil is returned.
    /// <para>Available since: 2.6.9</para>
    /// <para>返回当前客户端连接的名称</para>
    /// <para>支持此命令的Redis版本, 2.6.9+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the connection name of the current connection.
    /// <para>客户端名称, 未设置名称返回null</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-getname">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-getname">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    ClientGetName(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.ClientGetName() | flags, cancellationToken);
    }

    /// <summary>
    /// This command controls the tracking of the keys in the next command executed by the connection, when tracking is enabled in OPTIN or OPTOUT mode.
    /// <para>Available since: 6.0.0</para>
    /// <para>在 CLIENT TRACKING OPTIN/OPTOUT 模式下，标记下一个命令是否读取数据用于缓存</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="enabled">Whether to cache keys for the next command (YES) or not (NO) when tracking is in OPTIN/OPTOUT mode.
    /// <para>是否缓存下一个命令的读取结果</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-caching">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-caching">命令文档</see></para>
    /// </remarks>
    [Obsolete("This command is not recommended to be called manually. When local caching support is enabled, SharpRedis will handle it automatically.")]
    public bool ClientCaching(bool enabled, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.ClientCaching(enabled) | flags, cancellationToken);
    }

    /// <summary>
    /// The command just returns the ID of the current connection.
    /// <para>Available since: 5.0.0</para>
    /// <para>返回当前客户端连接的唯一标识符（ID）</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the ID of the client.
    /// <para>当前客户端的唯一 ID（64 位整数）</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-id">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-id">命令文档</see></para>
    /// </remarks>
    public long ClientId(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.ClientId() | flags, cancellationToken);
    }

    /// <summary>
    /// The command returns information and statistics about the current client connection in a mostly human readable format.
    /// <para>Available since: 6.2.0</para>
    /// <para>返回当前客户端连接的详细信息</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>a unique string for the current client, as described at the CLIENT LIST page.
    /// <para>当前客户端的详细信息，包含多个键值对字段</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-info">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-info">命令文档</see></para>
    /// </remarks>
    public string ClientInfo(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.ClientInfo() | flags, cancellationToken)!;
    }

    /// <summary>
    /// Closes the client connection at the specified address using the legacy CLIENT KILL syntax.
    /// <para>Available since: 2.4.0</para>
    /// <para>使用旧版 CLIENT KILL 语法关闭指定地址的客户端连接。</para>
    /// <para>支持此命令的Redis版本, 2.4.0+</para>
    /// </summary>
    /// <param name="address">The remote client address, including the port, as shown by CLIENT LIST.
    /// <para>CLIENT LIST 中显示的客户端远端地址，包含端口。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-kill">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-kill">命令文档</see></para>
    /// </remarks>
    public bool ClientKill(string address, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.ClientKill(address) | flags, cancellationToken);

    /// <summary>
    /// Closes client connections matching all the supplied filters.
    /// <para>Available since: 2.8.12</para>
    /// <para>关闭同时满足所有过滤条件的客户端连接。</para>
    /// <para>支持此命令的Redis版本, 2.8.12+</para>
    /// </summary>
    /// <param name="filter">One or more client filters. Redis skips the calling connection by default.
    /// <para>至少指定一个过滤条件。默认跳过当前执行命令的连接。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of client connections closed.
    /// <para>已关闭的客户端连接数，无匹配时返回 0。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-kill">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-kill">命令文档</see></para>
    /// </remarks>
    public long ClientKill(KillFilter filter, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.ClientKill(filter) | flags, cancellationToken);

    /// <summary>
    /// Lists all clients connected to the server.
    /// <para>Available since: 2.4.0</para>
    /// <para>列出服务端的所有客户端连接。</para>
    /// <para>支持此命令的Redis版本, 2.4.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The original CLIENT LIST text returned by Redis, including its line breaks.
    /// <para>Redis 返回的 CLIENT LIST 原始文本，保留换行。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-list">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-list">命令文档</see></para>
    /// </remarks>
    public string ClientList(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.ClientList() | flags, cancellationToken)!;

    /// <summary>
    /// Lists connected clients of the specified type.
    /// <para>Available since: 5.0.0</para>
    /// <para>列出指定类型的客户端连接。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="type">Client connection type.
    /// <para>客户端连接类型。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The original CLIENT LIST text returned by Redis, including its line breaks.
    /// <para>Redis 返回的 CLIENT LIST 原始文本，保留换行。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-list">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-list">命令文档</see></para>
    /// </remarks>
    public string ClientList(ClientType type, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.ClientList(type) | flags, cancellationToken)!;

    /// <summary>
    /// Lists the clients matching the specified IDs.
    /// <para>Available since: 6.2.0</para>
    /// <para>按一个或多个客户端 ID 列出匹配的连接。</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="ids">One or more client IDs.
    /// <para>一个或多个客户端 ID。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The original CLIENT LIST text returned by Redis, including its line breaks.
    /// <para>Redis 返回的 CLIENT LIST 原始文本，保留换行。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-list">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-list">命令文档</see></para>
    /// </remarks>
    public string ClientList(ReadOnlySpan<long> ids, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.ClientList(ids) | flags, cancellationToken)!;

    /// <summary>
    /// The CLIENT NO-EVICT command sets the client eviction mode for the current connection.
    /// <para>Available since: 7.0.0</para>
    /// <para>设置当前客户端连接的驱逐保护状态，防止其数据被内存驱逐策略删除</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="on">Turn eviction protection on (ON) or off (OFF) for the current connection.
    /// <para>启用或禁用驱逐保护</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-no-evict">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-no-evict">命令文档</see></para>
    /// </remarks>
    public bool ClientNoEvict(bool on, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.ClientNoEvict(on) | flags, cancellationToken);
    }

    /// <summary>
    /// The CLIENT NO-TOUCH command controls whether commands sent by the client will alter the LRU/LFU of the keys they access.
    /// <para>Available since: 7.2.0</para>
    /// <para>设置当前客户端连接是否更新所访问键的 LRU/LFU 时间戳</para>
    /// <para>支持此命令的Redis版本, 7.2.0+</para>
    /// </summary>
    /// <param name="on">Turn off (ON) or on (OFF) updates to keys' last-access time and LFU counter for commands issued by this connection.
    /// <para>ON=不更新 LRU/LFU，OFF=正常更新</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-no-touch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-no-touch">命令文档</see></para>
    /// </remarks>
    public bool ClientNoTouch(bool on, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.ClientNoTouch(on) | flags, cancellationToken);
    }

    /// <summary>
    /// Pauses client command processing for the specified duration.
    /// <para>Available since: 3.0.0</para>
    /// <para>在指定时间内暂停服务端处理客户端命令。</para>
    /// <para>支持此命令的Redis版本, 3.0.0+</para>
    /// </summary>
    /// <param name="timeout">Pause duration in milliseconds.
    /// <para>暂停时长，单位为毫秒。</para>
    /// </param>
    /// <param name="mode">Optional WRITE or ALL modifier, supported since Redis 6.2.0. Omit to pause both reads and writes.
    /// <para>可选模式，Redis 6.2.0 起支持。Write 只暂停写命令，All 暂停读写命令。省略时不发送模式参数，默认暂停读写命令。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-pause">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-pause">命令文档</see></para>
    /// </remarks>
    public bool ClientPause(long timeout, ClientPauseMode? mode = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.ClientPause(timeout, mode) | flags, cancellationToken);
    }

    /// <summary>
    /// Enables command replies on the selected connection.
    /// <para>Available since: 3.2.0</para>
    /// <para>执行 CLIENT REPLY ON，开启当前所选连接的命令回复。</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-reply">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-reply">命令文档</see></para>
    /// </remarks>
    public bool ClientReplyOn(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.ClientReplyOn() | flags, cancellationToken);

    /// <summary>
    /// The CLIENT SETINFO command assigns various info attributes to the current connection which are displayed in the output of CLIENT LIST and CLIENT INFO.
    /// <para>Available since: 7.2.0</para>
    /// <para>设置当前连接使用的客户端库名称和版本。用于服务器端识别客户端来源和版本</para>
    /// <para>支持此命令的Redis版本, 7.2.0+</para>
    /// </summary>
    /// <param name="category">The type of attribute to be set
    /// <para>要设置的属性类型</para>
    /// </param>
    /// <param name="value">The value of the attribute
    /// <para>属性的值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-setinfo">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-setinfo">命令文档</see></para>
    /// </remarks>
    public bool ClientSetInfo(SetInfoCategory category, string value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.ClientSetInfo(category, value) | flags, cancellationToken);
    }

    /// <summary>
    /// The CLIENT SETNAME command assigns a name to the current connection.
    /// <para>Available since: 2.6.9</para>
    /// <para>设置当前连接的客户端名称</para>
    /// <para>支持此命令的Redis版本, 2.6.9+</para>
    /// </summary>
    /// <param name="connectionName">The name to assign to the current connection.
    /// <para>客户端名称，空字符串表示清除名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-setname">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-setname">命令文档</see></para>
    /// </remarks>
    public bool ClientSetName(string connectionName, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.ClientSetName(connectionName) | flags, cancellationToken);
    }

    /// <summary>
    /// This command enables the tracking feature of the Redis server, which is used for server assisted client side caching.
    /// <para>Available since: 6.0.0</para>
    /// <para>开启或关闭客户端缓存（Client-side caching）</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="onOff">Turn key-invalidation tracking on or off for the connection.
    /// <para>开启或关闭客户端缓存</para>
    /// </param>
    /// <param name="redirect">Send invalidation messages to the connection with the specified ID. The connection must exist. You can get the ID of a connection using CLIENT ID. If the redirected connection terminates while in RESP3 mode, the connection with tracking enabled receives tracking-redir-broken push messages to signal the disconnection.
    /// <para>将失效通知重定向到指定客户端</para>
    /// </param>
    /// <param name="keyPrefixes">Track only keys that start with one of the specified prefixes. Use with the BCAST argument. You can specify this option multiple times to track multiple prefixes. If you enable broadcasting without this option, Redis sends invalidation messages for every key. You can’t remove a single prefix; to remove all prefixes, disable and re-enable tracking. This option adds O(N^2) time complexity, where N is the total number of tracked prefixes.
    /// <para>只追踪匹配前缀的 key</para>
    /// </param>
    /// <param name="bcast">Enable broadcasting mode. In this mode, Redis sends invalidation messages for all keys, or for keys that match a specified PREFIX, instead of only the keys that the connection reads. 
    /// <para>广播模式，服务器主动推送所有匹配前缀的失效</para>
    /// </param>
    /// <param name="optin">Track keys only for read commands that immediately follow a CLIENT CACHING YES.
    /// <para>选择性加入，只缓存 CLIENT CACHING YES 后的命令</para>
    /// </param>
    /// <param name="optout">Track all read keys except those for commands that immediately follow a CLIENT CACHING NO.
    /// <para>选择性退出，默认缓存，CLIENT CACHING NO 后跳过</para>
    /// </param>
    /// <param name="noloop">Do not send invalidation messages for keys modified by this connection.
    /// <para>不接收自己修改的 key 的失效通知</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    [Obsolete("It is strongly advised against invoking this command, as it will interfere with the functionality of the local cache.")]
    public bool ClientTracking(
        bool onOff,
        long? redirect,
#if MODERN_NET
        string[]? keyPrefixes,
#else
        string[] keyPrefixes,
#endif
        bool bcast,
        bool optin,
        bool optout,
        bool noloop,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.ClientTracking(
            onOff: onOff,
            redirect: redirect,
            keyPrefixes: keyPrefixes,
            bcast: bcast,
            optin: optin,
            optout: optout,
            noloop: noloop) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the current client-side caching tracking settings of the selected connection.
    /// <para>Available since: 6.2.0</para>
    /// <para>获取当前所选连接的客户端缓存追踪设置。</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The tracking flags, redirection client ID and tracked key prefixes.
    /// <para>追踪标记、接收失效通知的客户端 ID 及追踪的 key 前缀。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-trackinginfo">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-trackinginfo">命令文档</see></para>
    /// </remarks>
    public TrackingInfo ClientTrackingInfo(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.ClientTrackingInfo() | flags, cancellationToken);

    /// <summary>
    /// Unblocks a client on the selected Redis server.
    /// <para>Available since: 5.0.0</para>
    /// <para>解除所选 Redis 节点上指定客户端的阻塞。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="clientId">The ID of the blocked client.
    /// <para>被阻塞客户端的 ID。</para>
    /// </param>
    /// <param name="mode">The reply sent to the blocked client. None uses the server default (TIMEOUT).
    /// <para>被阻塞客户端收到的响应。None 表示省略该参数，默认按超时处理。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the client was unblocked, otherwise false.
    /// <para>成功解除阻塞返回 true，否则返回 false。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-unblock">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-unblock">命令文档</see></para>
    /// </remarks>
    public bool ClientUnblock(long clientId, ClientUnblockMode mode = ClientUnblockMode.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.ClientUnblock(clientId, mode) | flags, cancellationToken);

    /// <summary>
    /// CLIENT UNPAUSE is used to resume command processing for all clients that were paused by CLIENT PAUSE.
    /// <para>Available since: 6.2.0</para>
    /// <para>恢复处理客户端请求</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/client-unpause">Command documentation</see>
    /// <para><see href="https://redis.io/commands/client-unpause">命令文档</see></para>
    /// </remarks>
    public bool ClientUnpause(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.ClientUnpause() | flags, cancellationToken);
    }

    /// <summary>
    /// Returns message.
    /// <para>Available since: 1.0.0</para>
    /// <para>返回客户端传入的字符串</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="message">The message to echo back.
    /// <para>要回显的消息</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the given string.
    /// <para>返回传入的 message</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/echo">Command documentation</see>
    /// <para><see href="https://redis.io/commands/echo">命令文档</see></para>
    /// </remarks>
    public string Echo(string message, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.Echo(message) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the current connection information.
    /// <para>Available since: 6.2.0</para>
    /// <para>查询当前连接信息。</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Server and connection information.
    /// <para>服务端及连接信息。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/hello">Command documentation</see>
    /// <para><see href="https://redis.io/commands/hello">命令文档</see></para>
    /// </remarks>
    public HelloResult Hello(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.Hello() | flags, cancellationToken);

    /// <summary>
    /// Negotiate the RESP protocol and return connection information.
    /// <para>Available since: 6.0.0</para>
    /// <para>协商 RESP 协议版本并返回连接信息。</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="protover">RESP protocol version.
    /// <para>要协商的 RESP 协议版本。</para>
    /// </param>
    /// <param name="user">Username for AUTH; defaults to the default user.
    /// <para>认证用户名，省略时使用默认用户。</para>
    /// </param>
    /// <param name="password">Password for AUTH.
    /// <para>用于认证的密码。</para>
    /// </param>
    /// <param name="clientname">Name to set on the connection.
    /// <para>要设置的连接名称。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Server and connection information.
    /// <para>服务端及连接信息。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/hello">Command documentation</see>
    /// <para><see href="https://redis.io/commands/hello">命令文档</see></para>
    /// </remarks>
    public HelloResult Hello(Resp protover,
#if MODERN_NET
        string? user = null, string? password = null, string? clientname = null,
#else
        string user = null, string password = null, string clientname = null,
#endif
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ConnectionCommands.Hello(protover, user, password, clientname) | flags, cancellationToken);

    /// <summary>
    /// This command returns the simple string PONG if no argument is provided, otherwise it returns a copy of the provided message as a bulk string.
    /// <para>Available since: 1.0.0</para>
    /// <para>测试与 Redis 服务器的连接是否存活，常用于心跳检测和延迟测量</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="message">A message to return instead of PONG.
    /// <para>自定义消息内容</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Ping">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Ping">命令文档</see></para>
    /// </remarks>
    public string Ping(
#if MODERN_NET
        string? message = null,
#else
        string message = null,
#endif
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.Ping(message) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Ask the server to close the connection. The connection is closed as soon as all pending replies have been written to the client.
    /// <para>Available since: 1.0.0</para>
    /// <para>关闭当前客户端连接</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Quit">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Quit">命令文档</see></para>
    /// </remarks>
    [Obsolete("Use with caution, as this command will close the current connection, which may affect subsequent operations.")]
    public bool Quit(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.Quit() | flags, cancellationToken);
    }

    /// <summary>
    /// This command performs a full reset of the connection's server-side context, mimicking the effect of disconnecting and reconnecting again.
    /// <para>Available since: 6.2.0</para>
    /// <para>重置当前连接状态。清除所有未完成的操作、事务、监视等</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    [Obsolete("Use with caution, as this command will close the current connection, which may affect subsequent operations.")]
    public string Reset(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ConnectionCommands.Reset() | flags, cancellationToken)!;
    }

    [Obsolete("Do not use the SELECT command directly. If you need to switch databases, please use the Redis.Select method instead.", true)]
    public bool Select()
    {
        return false;
    }
}
