using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis key
/// <para>Key操作</para>
/// </summary>
public sealed partial class RedisKeyspace : RedisTypeBase
{
    #region enums
    /// <summary>
    /// Redis <b>EXPIRE</b> Command Conditional Options
    /// <para>Redis <b>EXPIRE</b> 命令条件选项</para>
    /// </summary>
    public enum ExpireCondition : byte
    {
        /// <summary>
        /// Default mode
        /// <para>默认方式</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Only when the key has not been set with an expiration time will it be set.
        /// <para>Available since: 7.0.0</para>
        /// <para>仅当 key 没有设置过期时间时才设置</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        NX = 1,

        /// <summary>
        /// Only set it when the key already has an expiration time.
        /// <para>Available since: 7.0.0</para>
        /// <para>仅当 key 已经存在过期时间时才设置</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        XX = 2,

        /// <summary>
        /// Only set it when the new expiration time is greater than the current remaining TTL.
        /// <para>Available since: 7.0.0</para>
        /// <para>仅当新的过期时间大于当前剩余 TTL 时才设置</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        GT = 3,

        /// <summary>
        /// Only set it when the new expiration time is less than the current remaining TTL.
        /// <para>Available since: 7.0.0</para>
        /// <para>仅当新的过期时间小于当前剩余 TTL 时才设置</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        LT = 4,
    }

    /// <summary>
    /// Sort direction.
    /// <para>Redis SORT / SORT_RO 命令的排序方向。</para>
    /// </summary>
    public enum SortDirection : byte
    {
        /// <summary>
        /// Default mode
        /// <para>默认方式</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Ascending order (default).
        /// <para>升序，Redis 默认行为。</para>
        /// </summary>
        Asc = 1,

        /// <summary>
        /// Descending order.
        /// <para>降序，对应 DESC。</para>
        /// </summary>
        Desc = 2,
    }
    #endregion

    internal RedisKeyspace(BaseRedis redis) : base(redis) { }

    /// <summary>
    /// Returns if key exists.
    /// <para>Available since: 1.0.0</para>
    /// <para>判断指定的Key是否存在</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Returns true if it exists, otherwise returns false.
    /// <para>存在返回true, 否则返回false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Exists">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Exists">命令文档</see></para>
    /// </remarks>
    public bool Exists(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.Exists(key) | flags, cancellationToken);

    /// <summary>
    /// Returns if key exists.
    /// <para>Available since: 3.0.3</para>
    /// <para>判断指定的Key是否存在</para>
    /// <para>支持此命令的Redis版本, 3.0.3+</para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of keys that existed.
    /// <para>Key存在的数量</para>
    /// </returns>
    public int Exists(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.Exists(keys) | flags, cancellationToken);

    /// <summary>
    /// Returns the remaining time to live of a key that has a timeout in seconds.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得指定Key剩余的有效期 (秒为单位)</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// In Redis 2.6 or older the command returns -1 if the key does not exist or if the key exists but has no associated expire.
    /// <para>TTL in seconds. -1 if the key exists but has no associated expiration. -2 if the key does not exist.</para>
    /// <para>在Redis 2.6及以前的老版本中, Key不存在和没有设置过期时间, 都返回-1</para>
    /// <para>有效期剩余秒数, -1表示Key永不过期, -2表示Key不存在</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Ttl">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Ttl">命令文档</see></para>
    /// </remarks>
    public long Ttl(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.Ttl(key) | flags, cancellationToken);

    /// <summary>
    /// Returns the remaining time to live of a key that has a timeout in milliseconds.
    /// <para>Available since: 2.6.0</para>
    /// <para>获得指定Key剩余的有效期 (毫秒为单位)</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// In Redis 2.6 or older the command returns -1 if the key does not exist or if the key exists but has no associated expire.
    /// <para>TTL in milliseconds. -1 if the key exists but has no associated expiration. -2 if the key does not exist.</para>
    /// <para>在Redis 2.6及以前的老版本中, Key不存在和没有设置过期时间, 都返回-1</para>
    /// <para>有效期剩余毫秒数, -1表示Key永不过期, -2表示Key不存在</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Ttl">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Ttl">命令文档</see></para>
    /// </remarks>
    public long PTtl(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.PTtl(key) | flags, cancellationToken);

    /// <summary>
    /// This command copies the value stored at the source key to the destination key.
    /// <para>Available since: 6.2.0</para>
    /// <para>复制一个Key到另外一个key</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="source">The key to copy from.<para>要复制的Key</para></param>
    /// <param name="destination">The key to copy to.<para>新的Key</para></param>
    /// <param name="dbIndex">Copy the key to the given database index instead of the current one.
    /// <para>复制到指定的数据库, 不指定为复制到当前数据库</para>
    /// </param>
    /// <param name="replace">Replace the destination key if it already exists. Default: No Replace
    /// <para>如果目标Key存在, 是否进行覆盖. 默认不覆盖</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns><b>True</b> if source was copied. <b>False</b> if source was not copied.
    /// <para>True复制成功, False复制失败</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Copy">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Copy">命令文档</see></para>
    /// </remarks>
    public bool Copy(SharpValue source, SharpValue destination, int? dbIndex = null, bool replace = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Copy(source, destination, dbIndex, replace) | flags, cancellationToken);
    }

    /// <summary>
    /// Removes the specified key. A key is ignored if it does not exist.
    /// <para>Available since: 1.0.0</para>
    /// <para>删除一个Key, 如果Key不存在, 则不进行任何操作</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Del">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Del">命令文档</see></para>
    /// </remarks>
    public bool Del(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Del(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Removes the specified keys. A key is ignored if it does not exist.
    /// <para>Available since: 1.0.0</para>
    /// <para>删除一个Key或多个Key, 如果Key不存在, 则忽略</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="keys">keys
    /// <para>要删除的Key集合</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of keys that were removed.
    /// <para>删除的Key数量, 不存在的Key不计入</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Del">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Del">命令文档</see></para>
    /// </remarks>
    public int Del(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Del(keys) | flags, cancellationToken);
    }

    /// <summary>
    /// Serialize the value stored at key in a Redis-specific format and return it to the user. The returned value can be synthesized back into a Redis key using the <b>RESTORE</b> command.
    /// <para>Available since: 2.6.0</para>
    /// <para>序列化 (导出) 一个Key中的值, 序列化后的值可以使用<b>RESTORE</b>命令反序列化到Redis</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The serialized value of the key.
    /// <para>序列化后的字节数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Dump">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Dump">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    byte[]?
#else
    byte[]
#endif
    Dump(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Dump(key) | flags | CommandFlags.ReturnBytes, cancellationToken);
    }

    /// <summary>
    /// Create a key associated with a value that is obtained by deserializing the provided serialized value (obtained via <b>DUMP</b>).
    /// <para>Available since: 2.6.0</para>
    /// <para>反序列化 (恢复) 一个值到Redis. 该值来源于<b>DUMP</b>命令, 序列化得到的值</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="datas">The serialized value, as produced by <b>DUMP</b>.
    /// <para><b>DUMP</b>命令得到的数据</para>
    /// </param>
    /// <param name="ttl">The time to live for the key, in milliseconds. 0 means no expiration. Default is 0
    /// <para>If an absolute time such as DateTime is passed in, it will be parsed as a millisecond-level timestamp. <b>Available since: 5.0.0</b></para>
    /// <para>有效期, 单位: 毫秒. 0表示永不过期. 默认就是0, 永不过期</para>
    /// <para>如果传入DateTime等绝对时间, 会解析为毫秒级的时间戳. <b>支持时间戳的Redis版本: 5.0.0+</b></para>
    /// </param>
    /// <param name="replace">Replace the key if it already exists. <b>Available since: 3.0.0</b>
    /// <para>如果Key存在, 是否覆盖它. <b>支持覆盖的Redis版本: 3.0.0+</b></para>
    /// </param>
    /// <param name="idletime">Set the key's idle time (for LRU eviction) to this many seconds. <b>Available since: 5.0.0</b>
    /// <para>设置Key的空闲时间, 单位: 秒. 这个和 LRU 淘汰策略相关. <b>支持此参数的Redis版本: 5.0.0+</b></para>
    /// </param>
    /// <param name="frequency">Set the key's access-frequency counter (for LFU eviction) to this value. <b>Available since: 5.0.0</b>
    /// <para>设置Key的访问次数. 这个和 LFU 淘汰策略相关. <b>支持此参数的Redis版本: 5.0.0+</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Restore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Restore">命令文档</see></para>
    /// </remarks>
    public bool Restore(SharpValue key, byte[] datas, SharpExpiry ttl = default, bool replace = false, ulong? idletime = null, ulong? frequency = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Restore(key, ttl, datas, replace, idletime, frequency) | flags, cancellationToken);
    }

    /// <summary>
    /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted.
    /// <para>Available since: 1.0.0</para>
    /// <para>给指定的Key设置有效期, 到期后, 该key将自动删除</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="seconds">The time to live, in seconds. The key is deleted after this many seconds.
    /// <para>Key的有效时间, 单位: 秒. Key在这个秒数之后将会被删除</para>
    /// </param>
    /// <param name="condition">Conditional options
    /// <para>Available since: 7.0.0</para>
    /// <para>条件选项</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if the timeout was not set; for example, the key doesn't exist, or the operation was skipped because of the provided arguments.
    /// <para>True if the timeout was set.</para>
    /// <para>如果过期时间未设置或Key不存在返回False. 设置成功返回True</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Expire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Expire">命令文档</see></para>
    /// </remarks>
    public bool Expire(SharpValue key, long seconds, ExpireCondition condition = ExpireCondition.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Expire(key, seconds, condition) | flags, cancellationToken);
    }

    /// <summary>
    /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted.
    /// <para>Available since: 1.0.0</para>
    /// <para>给指定的Key设置有效期, 到期后, 该key将自动删除</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="timeout">The time to live, in seconds. The key is deleted after this many seconds.
    /// <para>Key的有效时间, 单位: 秒. Key在这个秒数之后将会被删除</para>
    /// </param>
    /// <param name="condition">Conditional options
    /// <para>Available since: 7.0.0</para>
    /// <para>条件选项</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if the timeout was not set; for example, the key doesn't exist, or the operation was skipped because of the provided arguments.
    /// <para>True if the timeout was set.</para>
    /// <para>如果过期时间未设置或Key不存在返回False. 设置成功返回True</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Expire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Expire">命令文档</see></para>
    /// </remarks>
    public bool Expire(SharpValue key, TimeSpan timeout, ExpireCondition condition = ExpireCondition.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Expire(key, (long)timeout.TotalSeconds, condition) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works exactly like <b>EXPIRE</b> but the time to live of the key is specified in milliseconds instead of seconds.
    /// <para>Available since: 2.6.0</para>
    /// <para>给指定的Key设置有效期, 到期后, 该key将自动删除. 此命令跟<b>EXPIRE</b>区别为, 接受的有效时间为毫秒</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="milliseconds">The timeout, in milliseconds.
    /// <para>Key的有效时间, 单位: 毫秒. Key在这个毫秒数之后将会被删除</para>
    /// </param>
    /// <param name="condition">Conditional options
    /// <para>Available since: 7.0.0</para>
    /// <para>条件选项</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if the timeout was not set; for example, the key doesn't exist, or the operation was skipped because of the provided arguments.
    /// <para>True if the timeout was set.</para>
    /// <para>如果过期时间未设置或Key不存在返回False. 设置成功返回True</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PExpire">命令文档</see></para>
    /// </remarks>
    public bool PExpire(SharpValue key, long milliseconds, ExpireCondition condition = ExpireCondition.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.PExpire(key, milliseconds, condition) | flags, cancellationToken);

    /// <summary>
    /// This command works exactly like <b>EXPIRE</b> but the time to live of the key is specified in milliseconds instead of seconds.
    /// <para>Available since: 2.6.0</para>
    /// <para>给指定的Key设置有效期, 到期后, 该key将自动删除. 此命令跟<b>EXPIRE</b>区别为, 接受的有效时间为毫秒</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="timeout">The timeout, in milliseconds.
    /// <para>Key的有效时间, 单位: 毫秒. Key在这个毫秒数之后将会被删除</para>
    /// </param>
    /// <param name="condition">Conditional options
    /// <para>Available since: 7.0.0</para>
    /// <para>条件选项</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if the timeout was not set; for example, the key doesn't exist, or the operation was skipped because of the provided arguments.
    /// <para>True if the timeout was set.</para>
    /// <para>如果过期时间未设置或Key不存在返回False. 设置成功返回True</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PExpire">命令文档</see></para>
    /// </remarks>
    public bool PExpire(SharpValue key, TimeSpan timeout, ExpireCondition condition = ExpireCondition.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.PExpire(key, (long)timeout.TotalMilliseconds, condition) | flags, cancellationToken);

    /// <summary>
    /// EXPIREAT has the same effect and semantic as <b>EXPIRE</b>, but instead of specifying the number of seconds representing the TTL (time to live), it takes an absolute Unix timestamp (seconds since January 1, 1970). A timestamp in the past will delete the key immediately.
    /// <para>Available since: 1.2.0</para>
    /// <para>根据到期时间戳<b>(秒级时间戳)</b>, 给Key设置到期时间. 如果到时间小于当前时间, 则Key会立即删除</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="unixTimeSeconds">The absolute expiration time as a Unix timestamp in seconds. A timestamp in the past deletes the key immediately.
    /// <para>到期时间戳<b>(秒级时间戳)</b></para>
    /// </param>
    /// <param name="condition">Conditional options
    /// <para>Available since: 7.0.0</para>
    /// <para>条件选项</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if the timeout was not set; for example, the key doesn't exist, or the operation was skipped because of the provided arguments.
    /// <para>True if the timeout was set.</para>
    /// <para>如果过期时间未设置或Key不存在返回False. 设置成功返回True</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ExpireAt">命令文档</see></para>
    /// </remarks>
    public bool ExpireAt(SharpValue key, long unixTimeSeconds, ExpireCondition condition = ExpireCondition.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.ExpireAt(key, unixTimeSeconds, condition) | flags, cancellationToken);
    }

    /// <summary>
    /// EXPIREAT has the same effect and semantic as <b>EXPIRE</b>, but instead of specifying the number of seconds representing the TTL (time to live), it takes an absolute Unix timestamp (seconds since January 1, 1970). A timestamp in the past will delete the key immediately.
    /// <para>Available since: 1.2.0</para>
    /// <para>根据到期时间戳<b>(秒级时间戳)</b>, 给Key设置到期时间. 如果到时间小于当前时间, 则Key会立即删除</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="timeout">The absolute expiration time as a Unix timestamp in seconds. A timestamp in the past deletes the key immediately.
    /// <para>到期时间戳<b>(秒级时间戳)</b></para>
    /// </param>
    /// <param name="condition">Conditional options
    /// <para>Available since: 7.0.0</para>
    /// <para>条件选项</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if the timeout was not set; for example, the key doesn't exist, or the operation was skipped because of the provided arguments.
    /// <para>True if the timeout was set.</para>
    /// <para>如果过期时间未设置或Key不存在返回False. 设置成功返回True</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ExpireAt">命令文档</see></para>
    /// </remarks>
    public bool ExpireAt(SharpValue key, DateTimeOffset timeout, ExpireCondition condition = ExpireCondition.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.ExpireAt(key, timeout.ToUnixTimeSeconds(), condition) | flags, cancellationToken);
    }

    /// <summary>
    /// PEXPIREAT has the same effect and semantic as <b>EXPIREAT</b>, but the Unix time at which the key will expire is specified in milliseconds instead of seconds.
    /// <para>Available since: 2.6.0</para>
    /// <para>根据到期时间戳<b>(毫秒级时间戳)</b>, 给Key设置到期时间. 如果到时间小于当前时间, 则Key会立即删除</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="unixTimeMilliseconds">The absolute Unix expiration timestamp, in milliseconds since Unix epoch. A timestamp in the past deletes the key immediately.
    /// <para>到期时间戳<b>(毫秒级时间戳)</b></para>
    /// </param>
    /// <param name="condition">Conditional options
    /// <para>Available since: 7.0.0</para>
    /// <para>条件选项</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if the timeout was not set; for example, the key doesn't exist, or the operation was skipped because of the provided arguments.
    /// <para>True if the timeout was set.</para>
    /// <para>如果过期时间未设置或Key不存在返回False. 设置成功返回True</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PExpireAt">命令文档</see></para>
    /// </remarks>
    public bool PExpireAt(SharpValue key, long unixTimeMilliseconds, ExpireCondition condition = ExpireCondition.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.PExpireAt(key, unixTimeMilliseconds, condition) | flags, cancellationToken);

    /// <summary>
    /// PEXPIREAT has the same effect and semantic as <b>EXPIREAT</b>, but the Unix time at which the key will expire is specified in milliseconds instead of seconds.
    /// <para>Available since: 2.6.0</para>
    /// <para>根据到期时间戳<b>(毫秒级时间戳)</b>, 给Key设置到期时间. 如果到时间小于当前时间, 则Key会立即删除</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="timeout">The absolute Unix expiration timestamp, in milliseconds since Unix epoch. A timestamp in the past deletes the key immediately.
    /// <para>到期时间戳<b>(毫秒级时间戳)</b></para>
    /// </param>
    /// <param name="condition">Conditional options
    /// <para>Available since: 7.0.0</para>
    /// <para>条件选项</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if the timeout was not set; for example, the key doesn't exist, or the operation was skipped because of the provided arguments.
    /// <para>True if the timeout was set.</para>
    /// <para>如果过期时间未设置或Key不存在返回False. 设置成功返回True</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PExpireAt">命令文档</see></para>
    /// </remarks>
    public bool PExpireAt(SharpValue key, DateTimeOffset timeout, ExpireCondition condition = ExpireCondition.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.PExpireAt(key, timeout.ToUnixTimeMilliseconds(), condition) | flags, cancellationToken);

    /// <summary>
    /// Returns the absolute Unix timestamp (since January 1, 1970) in seconds at which the given key will expire.
    /// <para>Available since: 7.0.0</para>
    /// <para>获得指定Key的到期时间戳(秒级时间戳)</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The expiration Unix timestamp in seconds.
    /// <para><b>-1</b> if the key exists but has no associated expiration time. <b>-2</b> if the key does not exist.</para>
    /// <para>到期时间, 秒级时间戳形式. 如果Key没有过期时间返回<b>-1</b>, 如果Key不存在返回<b>-2</b></para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ExpireTime">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ExpireTime">命令文档</see></para>
    /// </remarks>
    public long ExpireTime(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.ExpireTime(key) | flags, cancellationToken);

    /// <summary>
    /// PEXPIRETIME has the same semantic as <b>EXPIRETIME</b>, but returns the absolute Unix expiration timestamp in milliseconds instead of seconds.
    /// <para>Available since: 7.0.0</para>
    /// <para>获得指定Key的到期时间戳(毫米级时间戳), 跟<b>EXPIRETIME</b>命令类似, 不过返回毫米时间戳</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Expiration Unix timestamp in milliseconds.
    /// <para><b>-1</b> if the key exists but has no associated expiration time. <b>-2</b> if the key does not exist.</para>
    /// <para>到期时间, 毫秒级时间戳形式. 如果Key没有过期时间返回<b>-1</b>, 如果Key不存在返回<b>-2</b></para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PExpireTime">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PExpireTime">命令文档</see></para>
    /// </remarks>
    public long PExpireTime(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.PExpireTime(key) | flags, cancellationToken);

    /// <summary>
    /// Returns all keys matching pattern.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得所有 pattern 匹配的Key</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="pattern">The glob-style pattern to match key names against.
    /// <para>匹配的模式</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A Array of keys matching pattern.
    /// <para>匹配到的所有Key数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Keys">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Keys">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    Keys(SharpValue pattern, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.Keys(pattern) | flags, cancellationToken);

    /// <summary>
    /// Atomically transfer a key from a source Redis instance to a destination Redis instance. On success the key is deleted from the original instance and is guaranteed to exist in the target instance.
    /// <para>Available since: 2.6.0</para>
    /// <para>将一个Key从现有实例移动到另一个目标实例. 移动成功后, Key将从现有实例删除</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="host">The hostname or IP address of the destination Redis instance.
    /// <para>目标实例的IP或主机地址</para>
    /// </param>
    /// <param name="port">The port of the destination Redis instance.
    /// <para>目标实例的端口号</para>
    /// </param>
    /// <param name="key">The key to migrate
    /// <para>要移动的Key</para>
    /// </param>
    /// <param name="destinationDb">The database index to migrate the key into on the destination instance.
    /// <para>目标实例的数据库下标</para>
    /// </param>
    /// <param name="timeout">The maximum idle time for the transfer, in milliseconds.
    /// <para>移动超时时间, 单位: 毫秒</para>
    /// </param>
    /// <param name="copy">Do not remove the key from the local instance.
    /// <para>Available since: 3.0.0</para>
    /// <para>是否要从现有实例删除Key, 默认为删除, 如果不要删除传true</para>
    /// <para>支持此参数为True的Redis版本, 3.0.0+</para>
    /// </param>
    /// <param name="replace">Replace an existing key on the destination instance.
    /// <para>Available since: 3.0.0</para>
    /// <para>如果目标实例存在Key, 是否要进行覆盖, 默认不覆盖. 要覆盖传true</para>
    /// <para>支持此参数为True的Redis版本, 3.0.0+</para>
    /// </param>
    /// <param name="password">Authenticate to the destination instance with the given password.
    /// <para>Available since: 4.0.7</para>
    /// <para>目标实例的密码</para>
    /// <para>支持此参数的Redis版本, 4.0.7+</para>
    /// </param>
    /// <param name="username">Authenticate to the destination instance with the given username and password (ACL-style auth, Redis 6 or later).
    /// <para>Available since: 6.0.0</para>
    /// <para>目标实例的认证用户名</para>
    /// <para>支持此参数的Redis版本, 6.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>OK on success
    /// <para>是否迁移成功</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Migrate">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Migrate">命令文档</see></para>
    /// </remarks>
    public bool Migrate(
        string host,
        ushort port,
        SharpValue key,
        int destinationDb,
        long timeout,
        bool copy = false,
        bool replace = false,
#if MODERN_NET
        string? password = null,
        string? username = null,
#else
        string password = null,
        string username = null,
#endif
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Migrate(
            host: host,
            port: port,
            key: key,
            destinationDb: destinationDb,
            timeout: timeout,
            copy: copy,
            replace: replace,
            password: password,
            username: username,
            keys: default) | flags, cancellationToken);
    }

    /// <summary>
    /// Atomically transfer a list keys from a source Redis instance to a destination Redis instance. On success the key is deleted from the original instance and is guaranteed to exist in the target instance.
    /// <para>Available since: 3.0.6</para>
    /// <para>将多个Key从现有实例移动到另一个目标实例. 移动成功后, Key将从现有实例删除</para>
    /// <para>支持此命令的Redis版本, 3.0.6+</para>
    /// </summary>
    /// <param name="host">The hostname or IP address of the destination Redis instance.
    /// <para>目标实例的IP或主机地址</para>
    /// </param>
    /// <param name="port">The port of the destination Redis instance.
    /// <para>目标实例的端口号</para>
    /// </param>
    /// <param name="keys">The list keys to migrate
    /// <para>要移动的Key集合</para>
    /// </param>
    /// <param name="destinationDb">The database index to migrate the key into on the destination instance.
    /// <para>目标实例的数据库下标</para>
    /// </param>
    /// <param name="timeout">The maximum idle time for the transfer, in milliseconds.
    /// <para>移动超时时间, 单位: 毫秒</para>
    /// </param>
    /// <param name="copy">Do not remove the key from the local instance.
    /// <para>是否要从现有实例删除Key, 默认为删除, 如果不要删除传true</para>
    /// </param>
    /// <param name="replace">Replace an existing key on the destination instance.
    /// <para>如果目标实例存在Key, 是否要进行覆盖, 默认不覆盖. 要覆盖传true</para>
    /// </param>
    /// <param name="password">Authenticate to the destination instance with the given password.
    /// <para>Available since: 4.0.7</para>
    /// <para>目标实例的密码</para>
    /// <para>支持此参数的Redis版本, 4.0.7+</para>
    /// </param>
    /// <param name="username">Authenticate to the destination instance with the given username and password (ACL-style auth, Redis 6 or later).
    /// <para>Available since: 6.0.0</para>
    /// <para>目标实例的认证用户名</para>
    /// <para>支持此参数的Redis版本, 6.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>OK on success
    /// <para>是否迁移成功</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Migrate">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Migrate">命令文档</see></para>
    /// </remarks>
    public bool Migrate(
        string host,
        ushort port,
        SharpValueSpan keys,
        int destinationDb,
        long timeout,
        bool copy = false,
        bool replace = false,
#if MODERN_NET
        string? password = null,
        string? username = null,
#else
        string password = null,
        string username = null,
#endif
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Migrate(
            host: host,
            port: port,
            key: default,
            destinationDb: destinationDb,
            timeout: timeout,
            copy: copy,
            replace: replace,
            password: password,
            username: username,
            keys: keys) | flags, cancellationToken);
    }

    /// <summary>
    /// Move key from the currently selected database (see SELECT) to the specified destination database. When key already exists in the destination database, or it does not exist in the source database, it does nothing
    /// <para>Available since: 1.0.0</para>
    /// <para>将一个Key从当前数据库移动到指定的目标数据库中 (下标)</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">The name of the key.
    /// <para>要移动的Key</para>
    /// </param>
    /// <param name="db">The destination database index.
    /// <para>目标数据库下标</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if key was moved. 0 if key wasn't moved.
    /// <para>移动成功返回True, 否则返回False</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Move">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Move">命令文档</see></para>
    /// </remarks>
    public bool Move(SharpValue key, int db, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Move(key, db) | flags, cancellationToken);
    }

    /// <summary>
    /// Remove the existing timeout on key, turning the key from volatile (a key with an expire set) to persistent (a key that will never expire as no timeout is associated).
    /// <para>Available since: 2.2.0</para>
    /// <para>移除一个Key的有效时间, 移除后Key将会被设为永不过期</para>
    /// <para>支持此命令的Redis版本, 2.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key.
    /// <para>要移除有效期的key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if key does not exist or does not have an associated timeout.
    /// <para>True if the timeout has been removed.</para>
    /// <para>移除成功返回True, 如果Key不存在或者Key本来就没有有效期则返回False</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Persist">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Persist">命令文档</see></para>
    /// </remarks>
    public bool Persist(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Persist(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Return a random key from the currently selected database.
    /// <para>Available since: 1.0.0</para>
    /// <para>从当前数据库中, 随机返回一个Key</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A random key in database.
    /// <para>随机得到的Key, 如果数据库中没有Key, 返回null</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/RandomKey">Command documentation</see>
    /// <para><see href="https://redis.io/commands/RandomKey">命令文档</see></para>
    /// </remarks>
    public RedisResult RandomKey(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.RandomKey() | flags, cancellationToken);

    /// <summary>
    /// Renames key to newkey. It returns an error when key does not exist. If newkey already exists it is overwritten.
    /// <para>Available since: 1.0.0</para>
    /// <para>重命名一个Key. 如果新Key存在, 会被覆盖</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">The key to rename.<para>要重命名的Key</para></param>
    /// <param name="newKey">The new key name. An existing key with this name is overwritten.
    /// <para>Key的新名称. 如果存在会被覆盖</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Rename">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Rename">命令文档</see></para>
    /// </remarks>
    public bool Rename(SharpValue key, SharpValue newKey, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.Rename(key, newKey) | flags, cancellationToken);

    /// <summary>
    /// Renames key to newkey if newkey does not yet exist. It returns an error when key does not exist.
    /// <para>Available since: 1.0.0</para>
    /// <para>重命名一个Key. 仅在新名称不存在的时候才重命名</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">The key to rename.<para>要重命名的Key</para></param>
    /// <param name="newKey">The new key name. The command fails if a key with this name already exists.
    /// <para>Key的新名称, 如果已存在, 则会重命名失败</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if key was renamed to newkey. False if newkey already exists.
    /// <para>重命名成功返回True. 如果新名称已经存在, 返回False</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/RenameNX">Command documentation</see>
    /// <para><see href="https://redis.io/commands/RenameNX">命令文档</see></para>
    /// </remarks>
    public bool RenameNX(SharpValue key, SharpValue newKey, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(KeyspaceCommands.RenameNX(key, newKey) | flags, cancellationToken);

    /// <summary>
    /// Returns the internal encoding for the Redis object stored at key.
    /// <para>Available since: 2.2.3</para>
    /// <para>获得指定key的编码</para>
    /// <para>支持此命令的Redis版本, 2.2.3+</para>
    /// </summary>
    /// <param name="key">The name of the key.
    /// <para>要获取编码的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The encoding of the object
    /// <para>指定Key对象的编码</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/object-encoding">Command documentation</see>
    /// <para><see href="https://redis.io/commands/object-encoding">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    ObjectEncoding(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.ObjectEncoding(key) | flags, cancellationToken);
    }

    /// <summary>
    /// This command returns the logarithmic access frequency counter of a Redis object stored at key.
    /// <para>Available since: 4.0.0</para>
    /// <para>返回指定 key 在 LFU（Least Frequently Used）淘汰策略下的访问频率计数</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="key">The name of the key.
    /// <para>要获得频率计数的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/object-freq">Command documentation</see>
    /// <para><see href="https://redis.io/commands/object-freq">命令文档</see></para>
    /// </remarks>
    public long? ObjectFreq(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.ObjectFreq(key) | flags, cancellationToken);
    }

    /// <summary>
    /// This command returns the time in seconds since the last access to the value stored at key.
    /// <para>Available since: 2.2.3</para>
    /// <para>返回指定 key 自上次被访问以来的空闲时间（秒）</para>
    /// <para>支持此命令的Redis版本, 2.2.3+</para>
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the idle time in seconds.
    /// <para>空闲时间, 单位: 秒</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/object-idletime">Command documentation</see>
    /// <para><see href="https://redis.io/commands/object-idletime">命令文档</see></para>
    /// </remarks>
    public long? ObjectIdleTime(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.ObjectIdleTime(key) | flags, cancellationToken);
    }

    /// <summary>
    /// This command returns the reference count of the stored at key.
    /// <para>Available since: 2.2.3</para>
    /// <para>返回指定 key 对应值对象的引用计数</para>
    /// <para>支持此命令的Redis版本, 2.2.3+</para>
    /// </summary>
    /// <param name="key">The name of the key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of references.
    /// <para>引用计数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/object-refcount">Command documentation</see>
    /// <para><see href="https://redis.io/commands/object-refcount">命令文档</see></para>
    /// </remarks>
    public long? ObjectRefCount(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.ObjectRefCount(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the string representation of the type of the value stored at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得 key 的数据类型</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">The name of the key.
    /// <para>要获得类型的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The type of key, or none when key doesn't exist.
    /// <para>Key的类型, 如果Key不存在, 返回none</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/type">Command documentation</see>
    /// <para><see href="https://redis.io/commands/type">命令文档</see></para>
    /// </remarks>
    public string Type(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Type(key) | flags, cancellationToken)!;
    }

    /// <summary>
    /// The command just unlinks the keys from the keyspace. The actual removal will happen later asynchronously.
    /// <para>Available since: 4.0.0</para>
    /// <para>异步删除一个或多个 key。与 DEL 不同，UNLINK 将删除操作放入后台线程，不阻塞主线程。</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="key">One key to delete asynchronously.
    /// <para>要异步删除的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Unlink">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Unlink">命令文档</see></para>
    /// </remarks>
    public bool Unlink(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Unlink(key) | flags, cancellationToken);
    }

    /// <summary>
    /// The command just unlinks the keys from the keyspace. The actual removal will happen later asynchronously.
    /// <para>Available since: 4.0.0</para>
    /// <para>异步删除一个或多个 key。与 DEL 不同，UNLINK 将删除操作放入后台线程，不阻塞主线程。</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="keys">One or more keys to delete asynchronously.
    /// <para>要异步删除的Key, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of keys that were unlinked.
    /// <para>成功删除的Key数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Unlink">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Unlink">命令文档</see></para>
    /// </remarks>
    public int Unlink(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Unlink(keys) | flags, cancellationToken);
    }

    /// <summary>
    /// This command blocks the current client until all the previous write commands are successfully transferred and acknowledged by at least the number of replicas you specify in the numreplicas argument.
    /// <para>Available since: 3.0.0</para>
    /// <para>阻塞等待当前连接的写命令同步到指定数量的副本（从节点）。</para>
    /// <para>支持此命令的Redis版本, 3.0.0+</para>
    /// </summary>
    /// <param name="numreplicas">The number of replicas to wait for.
    /// <para>需要确认的副本数量</para>
    /// </param>
    /// <param name="timeout">The maximum time to wait, in milliseconds. 0 means wait forever.
    /// <para>最大等待时间，0 表示永久阻塞. 单位: 毫秒</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the command returns the number of replicas reached by all the writes performed in the context of the current connection.
    /// <para>实际确认的副本数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Wait">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Wait">命令文档</see></para>
    /// </remarks>
    public int Wait(int numreplicas, long timeout, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Wait(numreplicas, timeout) | flags, cancellationToken);
    }

    /// <summary>
    /// This command blocks the current client until all the previous write commands are successfully transferred and acknowledged by at least the number of replicas you specify in the numreplicas argument.
    /// <para>Available since: 3.0.0</para>
    /// <para>阻塞等待当前连接的写命令同步到指定数量的副本（从节点）。</para>
    /// <para>支持此命令的Redis版本, 3.0.0+</para>
    /// </summary>
    /// <param name="numreplicas">The number of replicas to wait for.
    /// <para>需要确认的副本数量</para>
    /// </param>
    /// <param name="timeout">The maximum time to wait, in milliseconds. 0 means wait forever.
    /// <para>最大等待时间，0 表示永久阻塞. 单位: 毫秒</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the command returns the number of replicas reached by all the writes performed in the context of the current connection.
    /// <para>实际确认的副本数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Wait">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Wait">命令文档</see></para>
    /// </remarks>
    public int Wait(int numreplicas, TimeSpan timeout, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return this.Wait(numreplicas, (long)timeout.TotalMilliseconds, flags, cancellationToken);
    }

    /// <summary>
    /// This command blocks the current client until all previous write commands by that client are acknowledged as having been fsynced to the AOF of the local Redis and/or at least the specified number of replicas.
    /// <para>Available since: 7.2.0</para>
    /// <para>阻塞当前客户端，直到该客户端之前的所有写命令被确认已 fsync 到本地 AOF 和/或指定数量的副本的 AOF 中</para>
    /// <para>支持此命令的Redis版本, 7.2.0+</para>
    /// </summary>
    /// <param name="numlocal">The number of local Redis instances to wait for an AOF fsync on (0: false or 1: true).
    /// <para>需要确认的本地 fsync 数量。true 表示等待本地 AOF 持久化，false 表示禁用本地检查</para>
    /// </param>
    /// <param name="numreplicas">The number of replicas to wait for an AOF fsync on.
    /// <para>需要确认的副本 fsync 数量。0 表示不等待副本</para>
    /// </param>
    /// <param name="timeout">The maximum time to wait, in milliseconds. 0 means wait forever.
    /// <para>超时时间（毫秒）。0 表示永久阻塞</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/WaitAof">Command documentation</see>
    /// <para><see href="https://redis.io/commands/WaitAof">命令文档</see></para>
    /// </remarks>
    public WaitAofResult WaitAof(bool numlocal, int numreplicas, long timeout, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.WaitAof(numlocal, numreplicas, timeout) | flags, cancellationToken);
    }

    /// <summary>
    /// This command blocks the current client until all previous write commands by that client are acknowledged as having been fsynced to the AOF of the local Redis and/or at least the specified number of replicas.
    /// <para>Available since: 7.2.0</para>
    /// <para>阻塞当前客户端，直到该客户端之前的所有写命令被确认已 fsync 到本地 AOF 和/或指定数量的副本的 AOF 中</para>
    /// <para>支持此命令的Redis版本, 7.2.0+</para>
    /// </summary>
    /// <param name="numlocal">The number of local Redis instances to wait for an AOF fsync on (0: false or 1: true).
    /// <para>需要确认的本地 fsync 数量。true 表示等待本地 AOF 持久化，false 表示禁用本地检查</para>
    /// </param>
    /// <param name="numreplicas">The number of replicas to wait for an AOF fsync on.
    /// <para>需要确认的副本 fsync 数量。0 表示不等待副本</para>
    /// </param>
    /// <param name="timeout">The maximum time to wait, in milliseconds. 0 means wait forever.
    /// <para>超时时间（毫秒）。0 表示永久阻塞</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/WaitAof">Command documentation</see>
    /// <para><see href="https://redis.io/commands/WaitAof">命令文档</see></para>
    /// </remarks>
    public WaitAofResult WaitAof(bool numlocal, int numreplicas, TimeSpan timeout, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return this.WaitAof(numlocal, numreplicas, (long)timeout.TotalMilliseconds, flags, cancellationToken);
    }

    /// <summary>
    /// Alters the last access time of one key. A key is ignored if it does not exist.
    /// <para>Available since: 3.2.1</para>
    /// <para>更新指定Key的最后访问时间，而不改变键的值</para>
    /// <para>支持此命令的Redis版本, 3.2.1+</para>
    /// </summary>
    /// <param name="key">One key whose last-access time to update.
    /// <para>要更新的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Touch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Touch">命令文档</see></para>
    /// </remarks>
    public bool Touch(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Touch(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Alters the last access time of one key. A key is ignored if it does not exist.
    /// <para>Available since: 3.2.1</para>
    /// <para>更新指定Key的最后访问时间，而不改变键的值</para>
    /// <para>支持此命令的Redis版本, 3.2.1+</para>
    /// </summary>
    /// <param name="keys">One or more keys whose last-access time to update.
    /// <para>要更新的Key, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of touched keys.
    /// <para>成功更新访问时间的key的数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Touch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Touch">命令文档</see></para>
    /// </remarks>
    public int Touch(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Touch(keys) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns or stores the elements contained in the list, set or sorted set at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>对 List、Set 或 Sorted Set 中的元素进行排序，并支持多种高级选项（按外部 key 排序、限制返回数量、存储结果等）</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">The name of the key to sort. It may hold a list, set, or sorted set.
    /// <para>要排序的Key（List/Set/Sorted Set）</para>
    /// </param>
    /// <param name="offset">Skip offset elements
    /// <para>要跳过的元素数量</para>
    /// </param>
    /// <param name="count">return up to count.
    /// <para>最多返回元素数量</para>
    /// </param>
    /// <param name="sort">Sort in ascending (ASC, the default) or descending (DESC) order.
    /// <para>排序, ASC 或 DESC. 默认为ASC</para>
    /// </param>
    /// <param name="pattern">Sort by the values of the external keys matched by pattern (each * is replaced by an element) instead of by the elements themselves. A pattern with no *, or a hash-field pattern that resolves the same for all elements, skips sorting.
    /// <para>按外部 key 的值排序，pattern 可用 * 通配符替代元素值</para>
    /// </param>
    /// <param name="get">Return the values of the external keys matched by pattern for each sorted element. Use GET # to also return the element itself. May be given multiple times.
    /// <para>返回外部 key 的值替代元素本身，可多次指定</para>
    /// </param>
    /// <param name="alpha">Sort the elements lexicographically instead of numerically.
    /// <para>按字典序排序（默认按数字排序）</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>a list of sorted elements
    /// <para>一个已排序的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Sort">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Sort">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    Sort(
        SharpValue key,
        long? offset = null,
        long? count = null,
        SortDirection sort = SortDirection.None,
        SharpValue pattern = default,
        SharpValueSpan get = default,
        bool alpha = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Sort(
            key: key,
            pattern: pattern,
            offset: offset,
            count: count,
            get: get,
            sort: sort,
            alpha: alpha,
            store: default) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns or stores the elements contained in the list, set or sorted set at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>对 List、Set 或 Sorted Set 中的元素进行排序，并支持多种高级选项（按外部 key 排序、限制返回数量、存储结果等）</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">The name of the key to sort. It may hold a list, set, or sorted set.
    /// <para>要排序的Key（List/Set/Sorted Set）</para>
    /// </param>
    /// <param name="store">Store the resulting list in destination instead of returning it.
    /// <para>保存排序结果的Key</para>
    /// </param>
    /// <param name="offset">Skip offset elements
    /// <para>要跳过的元素数量</para>
    /// </param>
    /// <param name="count">return up to count.
    /// <para>最多返回元素数量</para>
    /// </param>
    /// <param name="sort">Sort in ascending (ASC, the default) or descending (DESC) order.
    /// <para>排序, ASC 或 DESC. 默认为ASC</para>
    /// </param>
    /// <param name="pattern">Sort by the values of the external keys matched by pattern (each * is replaced by an element) instead of by the elements themselves. A pattern with no *, or a hash-field pattern that resolves the same for all elements, skips sorting.
    /// <para>按外部 key 的值排序，pattern 可用 * 通配符替代元素值</para>
    /// </param>
    /// <param name="get">Return the values of the external keys matched by pattern for each sorted element. Use GET # to also return the element itself. May be given multiple times.
    /// <para>返回外部 key 的值替代元素本身，可多次指定</para>
    /// </param>
    /// <param name="alpha">Sort the elements lexicographically instead of numerically.
    /// <para>按字典序排序（默认按数字排序）</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the number of sorted elements in the destination list.
    /// <para>存入目标结果Key中的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Sort">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Sort">命令文档</see></para>
    /// </remarks>
    public long SortStore(
        SharpValue key,
        SharpValue store,
        long? offset = null,
        long? count = null,
        SortDirection sort = SortDirection.None,
        SharpValue pattern = default,
        SharpValueSpan get = default,
        bool alpha = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Sort(
            key: key,
            pattern: pattern,
            offset: offset,
            count: count,
            get: get,
            sort: sort,
            alpha: alpha,
            store: store) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Read-only variant of the SORT command. It is exactly like the original SORT but refuses the STORE option and can safely be used in read-only replicas.
    /// <para>Available since: 7.0.0</para>
    /// <para>只读版本的 SORT 命令。不修改任何 key，不支持 STORE 选项</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="key">The name of the key to sort. It may hold a list, set, or sorted set.
    /// <para>要排序的Key（List/Set/Sorted Set）</para>
    /// </param>
    /// <param name="offset">Skip offset elements
    /// <para>要跳过的元素数量</para>
    /// </param>
    /// <param name="count">return up to count.
    /// <para>最多返回元素数量</para>
    /// </param>
    /// <param name="sort">Sort in ascending (ASC, the default) or descending (DESC) order.
    /// <para>排序, ASC 或 DESC. 默认为ASC</para>
    /// </param>
    /// <param name="pattern">Sort by the values of the external keys matched by pattern (each * is replaced by an element) instead of by the elements themselves. A pattern with no *, or a hash-field pattern that resolves the same for all elements, skips sorting.
    /// <para>按外部 key 的值排序，pattern 可用 * 通配符替代元素值</para>
    /// </param>
    /// <param name="get">Return the values of the external keys matched by pattern for each sorted element. Use GET # to also return the element itself. May be given multiple times.
    /// <para>返回外部 key 的值替代元素本身，可多次指定</para>
    /// </param>
    /// <param name="alpha">Sort the elements lexicographically instead of numerically.
    /// <para>按字典序排序（默认按数字排序）</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>a list of sorted elements
    /// <para>一个已排序的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/sort_ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/sort_ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    Sort_Ro(
        SharpValue key,
        long? offset = null,
        long? count = null,
        SortDirection sort = SortDirection.None,
        SharpValue pattern = default,
        SharpValueSpan get = default,
        bool alpha = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Sort_Ro(
            key: key,
            pattern: pattern,
            offset: offset,
            count: count,
            get: get,
            sort: sort,
            alpha: alpha) | flags, cancellationToken);
    }

    /// <summary>
    /// SCAN iterates the set of keys in the currently selected Redis database.
    /// <para>Available since: 2.8.0</para>
    /// <para>以游标方式分批遍历 key，不阻塞 Redis 主线程</para>
    /// <para>支持此命令的Redis版本, 2.8.0+</para>
    /// </summary>
    /// <param name="cursor">The cursor value. Start an iteration with 0, then use the cursor returned by each call as the argument to the next call.
    /// <para>游标，第一次为 0，后续用上一次的返回值</para>
    /// </param>
    /// <param name="pattern">Only return keys matching the given glob-style pattern.
    /// <para>匹配模式，glob 风格</para>
    /// </param>
    /// <param name="count">A hint for the number of keys to return per iteration. The default is 10.
    /// <para>每次扫描的槽位数量，默认 10，建议 100-1000</para>
    /// </param>
    /// <param name="type"><b>Added in Redis 6.0.0.</b> Only return keys of the given type, such as string, list, or set.
    /// <para><b>Redis 6.0.0+ 才支持此参数</b> 只返回指定类型的 key. 比如string, list, set等</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/scan">Command documentation</see>
    /// <para><see href="https://redis.io/commands/scan">命令文档</see></para>
    /// </remarks>
    public ScanResult Scan(
        long cursor,
        SharpValue pattern = default,
        long? count = null,
#if MODERN_NET
        string? type = null,
#else
        string type = null,
#endif
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.Scan(
            cursor: cursor,
            pattern: pattern,
            count: count,
            type: type) | flags, cancellationToken);
    }

    /// <summary>
    /// BLESS SET marks a key so that Redis skips it when choosing candidates for eviction under memory pressure. The key must already exist.
    /// <para>Available since: 8.12.0</para>
    /// <para>为 Key 添加内存淘汰保护, 就算内存不足, 也不会淘汰</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key to protect. The key must already exist, or the command returns an error.
    /// <para>要保护的Key, Key必须存在, 否则引发异常</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the flag was added, False if the key already had it.
    /// <para>True: 添加保护标记成功. False: 表示已添加过保护标记</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bless-set">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bless-set">命令文档</see></para>
    /// </remarks>
    public bool BlessSet(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.BlessSet(key) | flags, cancellationToken);
    }

    /// <summary>
    /// BLESS GET returns the eviction-protection flags currently set on a key. The key must already exist.
    /// <para>Available since: 8.12.0</para>
    /// <para>查询 Key 当前具有的保护标记</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key to inspect. The key must already exist, or the command returns an error.
    /// <para>要查询的Key, Key必须存在, 否则引发异常</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>of the key's active protection flags, empty if none are set.
    /// <para>Key已设置的保护标记数组, 没有就为null</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bless-get">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bless-get">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string[]?
#else
    string[]
#endif
    BlessGet(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.BlessGet(key) | flags, cancellationToken);
    }

    /// <summary>
    /// BLESS CLEAR removes a protection flag from a key, making it eligible again for eviction. The key must already exist.
    /// <para>Available since: 8.12.0</para>
    /// <para>移除 Key 的内存淘汰保护</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key to unprotect. The key must already exist, or the command returns an error.
    /// <para>要移除淘汰保护的Key, Key必须存在, 否则引发异常</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the flag was removed, False if the key didn't have it.
    /// <para>成功移除淘汰保护标记返回True, 如果Key没有淘汰保护标记返回False</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bless-clear">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bless-clear">命令文档</see></para>
    /// </remarks>
    public bool BlessClear(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.BlessClear(key) | flags, cancellationToken);
    }

    /// <summary>
    /// BLESS SCAN incrementally iterates the blessed keys of the current database that carry the given protection flag, using the same cursor-based protocol as SCAN.
    /// <para>Available since: 8.12.0</para>
    /// <para>分批遍历已设置保护标记的 Key</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="cursor">The cursor value. Start an iteration with 0, then use the cursor returned by each call as the argument to the next call.
    /// <para>游标, 第一次使用0.</para>
    /// </param>
    /// <param name="count">A hint for the number of keys to visit per iteration. The default is 1024.
    /// <para>每次返回的最多数量, 默认为1024</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bless-scan">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bless-scan">命令文档</see></para>
    /// </remarks>
    public ScanResult BlessScan(long cursor, long? count = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(KeyspaceCommands.BlessScan(cursor, count) | flags, cancellationToken);
    }
}
