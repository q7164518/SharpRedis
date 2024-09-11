#if !LOW_NET
using System.Threading;
#else
#pragma warning disable IDE0130
#endif
using SharpRedis.Extensions;
using SharpRedis.Provider.Standard;
using System;
using SharpRedis.Commands;

namespace SharpRedis
{
    /// <summary>
    /// Key
    /// <para>Key操作</para>
    /// </summary>
    public sealed partial class RedisKey : BaseType
    {
        internal RedisKey(BaseCall call) : base(call)
        {
        }

        /// <summary>
        /// This command copies the value stored at the source key to the destination key.
        /// <para>Available since: 6.2.0</para>
        /// <para>将一个Key复制到另一个Key</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="sourceKey">source key
        /// <para>被复制的Key</para>
        /// </param>
        /// <param name="destinationKey">destination key
        /// <para>目标Key</para>
        /// </param>
        /// <param name="replace">Removes the destination key before copying the value to it.
        /// <para>如果目标Key存在, 是否强行覆盖. 不管目标Key是什么类型, 都会强行覆盖</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Copy(string sourceKey, string destinationKey, bool replace = false, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Copy(sourceKey, destinationKey, null, replace), "1", cancellationToken);
        }

        /// <summary>
        /// This command copies the value stored at the source key to the destination key.
        /// <para>Available since: 6.2.0</para>
        /// <para>将一个Key复制到另一个Key</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="sourceKey">source key
        /// <para>被复制的Key</para>
        /// </param>
        /// <param name="destinationKey">destination key
        /// <para>目标Key</para>
        /// </param>
        /// <param name="db">The DB option allows specifying an alternative logical database index for the destination key.
        /// <para>目标Key的数据库下标, 指定后可以跨下标复制</para>
        /// </param>
        /// <param name="replace">Removes the destination key before copying the value to it.
        /// <para>如果目标Key存在, 是否强行覆盖. 不管目标Key是什么类型, 都会强行覆盖</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Copy(string sourceKey, string destinationKey, ushort db, bool replace = false, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Copy(sourceKey, destinationKey, db, replace), "1", cancellationToken);
        }

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// <para>Available since: 1.0.0</para>
        /// <para>删除多个指定的Key, 如果Key不存在, 忽略</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="keys">Removes the specified keys
        /// <para>要删除的Key数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of keys that were removed.
        /// <para>删除的Key数量</para>
        /// </returns>
        public long Del(string[] keys, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(KeyCommands.Del(keys), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// <para>Available since: 1.0.0</para>
        /// <para>删除指定的Key, 如果Key不存在, 忽略</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Removes the specified key
        /// <para>要删除的Key</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Yes No Delete successfully
        /// <para>是否删除成功</para>
        /// </returns>
        public bool Del(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Del(key), "1", cancellationToken);
        }

        /// <summary>
        /// Serialize the value stored at key in a Redis-specific format and return it to the user.
        /// <para>The returned value can be synthesized back into a Redis key using the RESTORE command.</para>
        /// <para>Available since: 2.6.0</para>
        /// <para>以Redis特定的格式序列化key的值. 得到的值可以使用RESTORE命令重新写入到Redis</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The serialized value.
        /// <para>序列化之后的值</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? Dump(string key, CancellationToken cancellationToken = default)
#else
        public byte[] Dump(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(KeyCommands.Dump(key), cancellationToken);
        }

        /// <summary>
        /// Returns if key exists.
        /// <para>The user should be aware that if the same existing key is mentioned in the arguments multiple times, it will be counted multiple times. So if somekey exists, EXISTS somekey somekey will return 2.</para>
        /// <para>Available since: 3.0.3</para>
        /// <para>判断多个Key是否存在</para>
        /// <para>如果指定的Key存在重复, 且Key存在, 将累加, 不会去重</para>
        /// <para>支持此命令的Redis版本, 3.0.3+</para>
        /// </summary>
        /// <param name="keys">Keys
        /// <para>要判断的Key数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of keys that exist from those specified as arguments.
        /// <para>指定的Key数组中, 存在的数量</para>
        /// </returns>
        public long Exists(string[] keys, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(KeyCommands.Exists(keys), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns if key exists.
        /// <para>Available since: 1.0.0</para>
        /// <para>判断Key是否存在</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Existence or not
        /// <para>是否存在</para>
        /// </returns>
        public bool Exists(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Exists(key), "1", cancellationToken);
        }

        /// <summary>
        /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted.
        /// <para>Available since: 1.0.0 | 7.0.0</para>
        /// <para>设置一个Key的过期时间. 到期后Key将自动删除</para>
        /// <para>支持此命令的Redis版本, 1.0.0+ | 7.0.0</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="seconds">timeout, seconds
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 7.0.0. Nx: Set expiry only when the key has no expiry
        /// <para>Xx: Set expiry only when the key has an existing expiry</para>
        /// <para>Redis 7.0.0+才支持此参数. Nx: 仅当Key没有过期时间时设置</para>
        /// <para>Xx: 仅当Key有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Available since: 7.0.0. Gt: Set expiry only when the new expiry is greater than current one
        /// <para>Lt: Set expiry only when the new expiry is less than current one</para>
        /// <para>Redis 7.0.0+才支持此参数. Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Expire(string key, ulong seconds, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Expire(key, seconds, nxx, glt), "1", cancellationToken);
        }

        /// <summary>
        /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted.
        /// <para>Available since: 2.6.0 | 7.0.0</para>
        /// <para>设置一个Key的过期时间. 到期后Key将自动删除</para>
        /// <para>支持此命令的Redis版本, 2.6.0+ | 7.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="milliseconds">timeout, milliseconds
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 7.0.0. Nx: Set expiry only when the key has no expiry
        /// <para>Xx: Set expiry only when the key has an existing expiry</para>
        /// <para>Redis 7.0.0+才支持此参数. Nx: 仅当Key没有过期时设置</para>
        /// <para>Xx: 仅当Key有过期时设置</para>
        /// </param>
        /// <param name="glt">
        /// Available since: 7.0.0. Gt: Set expiry only when the new expiry is greater than current one
        /// <para>Lt: Set expiry only when the new expiry is less than current one</para>
        /// <para>Redis 7.0.0+才支持此参数. Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool PExpire(string key, ulong milliseconds, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.PExpire(key, milliseconds, nxx, glt), "1", cancellationToken);
        }

        /// <summary>
        /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted.
        /// <para>Available since: 1.0.0 | 7.0.0</para>
        /// <para>设置一个Key的过期时间. 到期后Key将自动删除</para>
        /// <para>支持此命令的Redis版本, 1.0.0+ | 7.0.0</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="timeout">timeout, Timespan
        /// <para>有效时间</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 7.0.0. Nx: Set expiry only when the key has no expiry
        /// <para>Xx: Set expiry only when the key has an existing expiry</para>
        /// <para>Redis 7.0.0+才支持此参数. Nx: 仅当Key没有过期时设置</para>
        /// <para>Xx: 仅当Key有过期时设置</para>
        /// </param>
        /// <param name="glt">
        /// Available since: 7.0.0. Gt: Set expiry only when the new expiry is greater than current one
        /// <para>Lt: Set expiry only when the new expiry is less than current one</para>
        /// <para>Redis 7.0.0+才支持此参数. Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Expire(string key, TimeSpan timeout, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return this.Expire(key, (ulong)timeout.TotalSeconds, nxx, glt, cancellationToken);
        }

        /// <summary>
        /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted.
        /// <para>Available since: 2.6.0 | 7.0.0</para>
        /// <para>设置一个Key的过期时间. 到期后Key将自动删除</para>
        /// <para>支持此命令的Redis版本, 2.6.0+ | 7.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="timeout">timeout, Timespan
        /// <para>有效时间</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 7.0.0. Nx: Set expiry only when the key has no expiry
        /// <para>Xx: Set expiry only when the key has an existing expiry</para>
        /// <para>Redis 7.0.0+才支持此参数. Nx: 仅当Key没有过期时设置</para>
        /// <para>Xx: 仅当Key有过期时设置</para>
        /// </param>
        /// <param name="glt">
        /// Available since: 7.0.0. Gt: Set expiry only when the new expiry is greater than current one
        /// <para>Lt: Set expiry only when the new expiry is less than current one</para>
        /// <para>Redis 7.0.0+才支持此参数. Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool PExpire(string key, TimeSpan timeout, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return this.PExpire(key, (ulong)timeout.TotalMilliseconds, nxx, glt, cancellationToken);
        }

        /// <summary>
        /// EXPIREAT has the same effect and semantic as EXPIRE, but instead of specifying the number of seconds representing the TTL (time to live), it takes an absolute Unix timestamp (seconds since January 1, 1970).
        /// <para>Available since: 1.2.0 | 7.0.0</para>
        /// <para>设置一个Key的到期时间, 到期后Key将自动删除</para>
        /// <para>支持此命令的Redis版本, 1.2.0+ | 7.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="expiration">Expiration Date
        /// <para>到期时间</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 7.0.0. Nx: Set expiry only when the key has no expiry
        /// <para>Xx: Set expiry only when the key has an existing expiry</para>
        /// <para>Redis 7.0.0+才支持此参数. Nx: 仅当Key没有过期时设置</para>
        /// <para>Xx: 仅当Key有过期时设置</para>
        /// </param>
        /// <param name="glt">
        /// Available since: 7.0.0. Gt: Set expiry only when the new expiry is greater than current one
        /// <para>Lt: Set expiry only when the new expiry is less than current one</para>
        /// <para>Redis 7.0.0+才支持此参数. Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool ExpireAt(string key, DateTimeOffset expiration, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.ExpireAt(key, Extend.GetUnixTimeSeconds(expiration), nxx, glt), "1", cancellationToken);
        }

        /// <summary>
        /// EXPIREAT has the same effect and semantic as EXPIRE, but instead of specifying the number of seconds representing the TTL (time to live), it takes an absolute Unix timestamp (seconds since January 1, 1970).
        /// <para>Available since: 1.2.0 | 7.0.0</para>
        /// <para>设置一个Key的到期时间, 到期后Key将自动删除</para>
        /// <para>支持此命令的Redis版本, 1.2.0+ | 7.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="unixTimeSeconds">unix-time-seconds
        /// <para>到期时间戳, 秒级</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 7.0.0. Nx: Set expiry only when the key has no expiry
        /// <para>Xx: Set expiry only when the key has an existing expiry</para>
        /// <para>Redis 7.0.0+才支持此参数. Nx: 仅当Key没有过期时设置</para>
        /// <para>Xx: 仅当Key有过期时设置</para>
        /// </param>
        /// <param name="glt">
        /// Available since: 7.0.0. Gt: Set expiry only when the new expiry is greater than current one
        /// <para>Lt: Set expiry only when the new expiry is less than current one</para>
        /// <para>Redis 7.0.0+才支持此参数. Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool ExpireAt(string key, long unixTimeSeconds, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.ExpireAt(key, unixTimeSeconds, nxx, glt), "1", cancellationToken);
        }

        /// <summary>
        /// PEXPIREAT has the same effect and semantic as EXPIREAT, but the Unix time at which the key will expire is specified in milliseconds instead of seconds.
        /// <para>Available since: 2.6.0 | 7.0.0</para>
        /// <para>设置一个Key的到期时间, 到期后Key将自动删除</para>
        /// <para>支持此命令的Redis版本, 2.6.0+ | 7.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="expiration">Expiration Date
        /// <para>到期时间</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 7.0.0. Nx: Set expiry only when the key has no expiry
        /// <para>Xx: Set expiry only when the key has an existing expiry</para>
        /// <para>Redis 7.0.0+才支持此参数. Nx: 仅当Key没有过期时设置</para>
        /// <para>Xx: 仅当Key有过期时设置</para>
        /// </param>
        /// <param name="glt">
        /// Available since: 7.0.0. Gt: Set expiry only when the new expiry is greater than current one
        /// <para>Lt: Set expiry only when the new expiry is less than current one</para>
        /// <para>Redis 7.0.0+才支持此参数. Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool PExpireAt(string key, DateTimeOffset expiration, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.PExpireAt(key, Extend.GetUnixTimeSeconds(expiration), nxx, glt), "1", cancellationToken);
        }

        /// <summary>
        /// PEXPIREAT has the same effect and semantic as EXPIREAT, but the Unix time at which the key will expire is specified in milliseconds instead of seconds.
        /// <para>Available since: 2.6.0 | 7.0.0</para>
        /// <para>设置一个Key的到期时间, 到期后Key将自动删除</para>
        /// <para>支持此命令的Redis版本, 2.6.0+ | 7.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="unixTimeMilliseconds">unix-time-milliseconds
        /// <para>到期时间戳, 毫秒级</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 7.0.0. Nx: Set expiry only when the key has no expiry
        /// <para>Xx: Set expiry only when the key has an existing expiry</para>
        /// <para>Redis 7.0.0+才支持此参数. Nx: 仅当Key没有过期时设置</para>
        /// <para>Xx: 仅当Key有过期时设置</para>
        /// </param>
        /// <param name="glt">
        /// Available since: 7.0.0. Gt: Set expiry only when the new expiry is greater than current one
        /// <para>Lt: Set expiry only when the new expiry is less than current one</para>
        /// <para>Redis 7.0.0+才支持此参数. Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool PExpireAt(string key, long unixTimeMilliseconds, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.PExpireAt(key, unixTimeMilliseconds, nxx, glt), "1", cancellationToken);
        }

        /// <summary>
        /// Returns the absolute Unix timestamp (since January 1, 1970) in seconds at which the given key will expire.
        /// <para>Available since: 7.0.0</para>
        /// <para>返回指定Key的到期时间戳, 秒级时间戳. 如果Key没有过期时间, 返回-1. 如果Key不存在, 返回-2</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Expiration Unix timestamp in seconds, or a negative value in order to signal an error (see the description below).
        /// <para>The command returns -1 if the key exists but has no associated expiration time.</para>
        /// <para>The command returns -2 if the key does not exist.</para>
        /// <para>秒级时间戳</para>
        /// <para>如果Key没有过期时间, 返回-1</para>
        /// <para>如果Key不存在, 返回-2</para>
        /// </returns>
        public long ExpireTime(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(KeyCommands.ExpireTime(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the absolute Unix timestamp (since January 1, 1970) in milliseconds at which the given key will expire.
        /// <para>Available since: 7.0.0</para>
        /// <para>返回指定Key的到期时间戳, 毫秒级时间戳. 如果Key没有过期时间, 返回-1. 如果Key不存在, 返回-2</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Expiration Unix timestamp in milliseconds, or a negative value in order to signal an error (see the description below).
        /// <para>The command returns -1 if the key exists but has no associated expiration time.</para>
        /// <para>The command returns -2 if the key does not exist.</para>
        /// <para>毫秒级时间戳</para>
        /// <para>如果Key没有过期时间, 返回-1</para>
        /// <para>如果Key不存在, 返回-2</para>
        /// </returns>
        public long PExpireTime(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(KeyCommands.PExpireTime(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns all keys matching pattern.
        /// <para>Warning: consider KEYS as a command that should only be used in production environments with extreme care.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>返回匹配到的所有Key.</para>
        /// <para>此命令会扫描所有Key, 在生产环境中, 谨慎使用</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="pattern">pattern</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string?[]? Keys(string pattern, CancellationToken cancellationToken = default)
#else
        public string[] Keys(string pattern, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(KeyCommands.Keys(pattern), ResultType.Array | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Move key from the currently selected database (see SELECT) to the specified destination database.
        /// <para>When key already exists in the destination database, or it does not exist in the source database, it does nothing. It is possible to use MOVE as a locking primitive because of this.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>移动指定Key到指定数据库. 如果源数据库不存在Key, 或目标数据库已经存在Key, 什么都不做, 会返回false</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="db">specified destination database, 0 - 15
        /// <para>指定的数据库下标, 0 - 15</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Move(string key, ushort db, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Move(key, db), "1", cancellationToken);
        }

        /// <summary>
        /// Remove the existing timeout on key, turning the key from volatile (a key with an expire set) to persistent (a key that will never expire as no timeout is associated).
        /// <para>Available since: 2.2.0</para>
        /// <para>移除指定Key的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>True if the timeout was removed.
        /// <para>False if key does not exist or does not have an associated timeout.</para>
        /// <para>True: 移除成功</para>
        /// <para>False: Key不存在或本身就没有设置过期时间</para>
        /// </returns>
        public bool Persist(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Persist(key), "1", cancellationToken);
        }

        /// <summary>
        /// Returns the remaining time to live of a key that has a timeout. This introspection capability allows a Redis client to check how many seconds a given key will continue to be part of the dataset.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回指定Key到期剩余的秒数</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>TTL in seconds
        /// <para>-1 if the key exists but has no associated expiration</para>
        /// <para>-2 if the key does not exist. Available since: 2.8.0</para>
        /// <para>正数表示有效期剩余秒数. -1表示不存在Key或没有到期时间</para>
        /// <para>-2: key不存在. Redis 2.8.0+才会返回</para>
        /// </returns>
        public long Ttl(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(KeyCommands.Ttl(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Like TTL this command returns the remaining time to live of a key that has an expire set, with the sole difference that TTL returns the amount of remaining time in seconds while PTTL returns it in milliseconds.
        /// <para>Available since: 2.6.0</para>
        /// <para>返回指定Key到期剩余的毫秒数</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>TTL in milliseconds
        /// <para>-1 if the key exists but has no associated expiration</para>
        /// <para>-2 if the key does not exist. Available since: 2.8.0</para>
        /// <para>正数表示有效期剩余毫秒数. -1表示不存在Key或没有到期时间</para>
        /// <para>-2: key不存在. Redis 2.8.0+才会返回</para>
        /// </returns>
        public long PTtl(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(KeyCommands.PTtl(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Return a random key from the currently selected database.
        /// <para>Available since: 1.0.0</para>
        /// <para>从当前数据库中随机返回一个Key</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? RandomKey(CancellationToken cancellationToken = default)
#else
        public string RandomKey(CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(KeyCommands.RandomKey(), cancellationToken);
        }

        /// <summary>
        /// Renames key to newkey. It returns an error when key does not exist. If newkey already exists it is overwritten,
        /// <para>when this happens RENAME executes an implicit DEL operation, so if the deleted key contains a very big value it may cause high latency even if RENAME itself is usually a constant-time operation.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>将指定的Key重命名. 如果newkey已存在, 会被覆盖</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="newkey">newkey
        /// <para>新名称</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Rename(string key, string newkey, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Rename(key, newkey), "OK", cancellationToken);
        }

        /// <summary>
        /// Renames key to newkey if newkey does not yet exist. It returns an error when key does not exist.
        /// <para>Available since: 1.0.0</para>
        /// <para>将指定的Key重命名, 只有在新Key不存在的时候才重命名</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="newkey">newkey
        /// <para>新名称</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool RenameNx(string key, string newkey, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.RenameNx(key, newkey), "1", cancellationToken);
        }

        /// <summary>
        /// Returns the string representation of the type of the value stored at key. The different types that can be returned are: string, list, set, zset, hash and stream.
        /// <para>Available since: 1.0.0</para>
        /// <para>获得Key的类型. 支持返回的类型有: string, list, set, zset, hash和stream</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? Type(string key, CancellationToken cancellationToken = default)
#else
        public string Type(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(KeyCommands.Type(key), cancellationToken);
        }

        /// <summary>
        /// This command is very similar to DEL: it removes the specified keys. Just like DEL a key is ignored if it does not exist. However the command performs the actual memory reclaiming in a different thread, so it is not blocking, while DEL is
        /// <para>Available since: 4.0.0</para>
        /// <para>非阻塞方式删除一个Key. 和DEL相似, 不过DEL是阻塞的, UNLINK是非阻塞的, 异步删除Key</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Unlink(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Unlink(key), "1", cancellationToken);
        }

        /// <summary>
        /// This command is very similar to DEL: it removes the specified keys. Just like DEL a key is ignored if it does not exist. However the command performs the actual memory reclaiming in a different thread, so it is not blocking, while DEL is
        /// <para>Available since: 4.0.0</para>
        /// <para>非阻塞方式删除一个Key. 和DEL相似, 不过DEL是阻塞的, UNLINK是非阻塞的, 异步删除Key</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of keys that were unlinked
        /// <para>删除的Key数量</para>
        /// </returns>
        public long Unlink(string[] keys, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(KeyCommands.Unlink(keys), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Create a key associated with a value that is obtained by deserializing the provided serialized value (obtained via DUMP)
        /// <para>Available since: 2.6.0</para>
        /// <para>从序列化的值中恢复Key. (从DUMP命令的返回值中恢复Key)</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="serializedValue">serialized-value<para>序列化后的值</para></param>
        /// <param name="ttl">If ttl is 0 the key is created without any expire, otherwise the specified expire time (in milliseconds) is set
        /// <para>有效期, 0表示不过期, 其它值表示有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Restore(string key, byte[] serializedValue, ulong ttl, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Restore(key, ttl, false, serializedValue, false, null, null), "OK", cancellationToken);
        }

        /// <summary>
        /// Create a key associated with a value that is obtained by deserializing the provided serialized value (obtained via DUMP)
        /// <para>Available since: 5.0.0</para>
        /// <para>从序列化的值中恢复Key. (从DUMP命令的返回值中恢复Key)</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="serializedValue">serialized-value<para>序列化后的值</para></param>
        /// <param name="timeout">Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Restore(string key, byte[] serializedValue, DateTimeOffset timeout, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Restore(key, (ulong)Extend.GetUnixTimeMilliseconds(timeout), true, serializedValue, false, null, null), "OK", cancellationToken);
        }

        /// <summary>
        /// Create a key associated with a value that is obtained by deserializing the provided serialized value (obtained via DUMP)
        /// <para>Available since: 5.0.0</para>
        /// <para>从序列化的值中恢复Key. (从DUMP命令的返回值中恢复Key)</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="serializedValue">serialized-value<para>序列化后的值</para></param>
        /// <param name="timeout">Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="replace">Whether to override the key if it exists
        /// <para>如果恢复时指定的Key存在, 是否进行覆盖</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Restore(string key, byte[] serializedValue, DateTimeOffset timeout, bool replace, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Restore(key, (ulong)Extend.GetUnixTimeMilliseconds(timeout), true, serializedValue, replace, null, null), "OK", cancellationToken);
        }

        /// <summary>
        /// Create a key associated with a value that is obtained by deserializing the provided serialized value (obtained via DUMP)
        /// <para>Available since: 2.6.0 | 3.0.0</para>
        /// <para>从序列化的值中恢复Key. (从DUMP命令的返回值中恢复Key)</para>
        /// <para>支持此命令的Redis版本, 2.6.0+ | 3.0.0</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="serializedValue">serialized-value<para>序列化后的值</para></param>
        /// <param name="ttl">If ttl is 0 the key is created without any expire, otherwise the specified expire time (in milliseconds) is set
        /// <para>有效期, 0表示不过期, 其它值表示有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="replace">Available since: 3.0.0. Whether to override the key if it exists
        /// <para>支持此参数的Redis版本: 3.0.0+. 如果恢复时指定的Key存在, 是否进行覆盖</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Restore(string key, byte[] serializedValue, ulong ttl, bool replace, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Restore(key, ttl, false, serializedValue, replace, null, null), "OK", cancellationToken);
        }

        /// <summary>
        /// Create a key associated with a value that is obtained by deserializing the provided serialized value (obtained via DUMP)
        /// <para>Available since: 5.0.0</para>
        /// <para>从序列化的值中恢复Key. (从DUMP命令的返回值中恢复Key)</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="serializedValue">serialized-value<para>序列化后的值</para></param>
        /// <param name="timeout">Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="replace">Whether to override the key if it exists
        /// <para>如果恢复时指定的Key存在, 是否进行覆盖</para>
        /// </param>
        /// <param name="frequency">frequency</param>
        /// <param name="idletime">idletime, seconds</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Restore(string key, byte[] serializedValue, DateTimeOffset timeout, bool replace, ulong? idletime = null, ulong? frequency = null, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Restore(key, (ulong)Extend.GetUnixTimeMilliseconds(timeout), true, serializedValue, replace, idletime, frequency), "OK", cancellationToken);
        }

        /// <summary>
        /// Alters the last access time of a key
        /// <para>Available since: 3.2.1</para>
        /// <para>更改Key的最后访问时间</para>
        /// <para>支持此命令的Redis版本, 3.2.1+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Touch(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Touch(key), "1", cancellationToken);
        }

        /// <summary>
        /// Alters the last access time of keys
        /// <para>Available since: 3.2.1</para>
        /// <para>更改Key的最后访问时间</para>
        /// <para>支持此命令的Redis版本, 3.2.1+</para>
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of touched keys
        /// <para>更改的Key数量</para>
        /// </returns>
        public long Touch(string[] keys, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(KeyCommands.Touch(keys), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the internal encoding for the Redis object stored at key
        /// <para>Available since: 2.2.3</para>
        /// <para>返回指定Key的编码格式</para>
        /// <para>支持此命令的Redis版本, 2.2.3+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? ObjectEncoding(string key, CancellationToken cancellationToken = default)
#else
        public string ObjectEncoding(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(KeyCommands.ObjectEncoding(key), cancellationToken);
        }

        /// <summary>
        /// This command returns the logarithmic access frequency counter of a Redis object stored at key.
        /// <para>The command is only available when the maxmemory-policy configuration directive is set to one of the LFU policies</para>
        /// <para>Available since: 4.0.0</para>
        /// <para>获得指定Key的访问次数. 仅当 maxmemory-policy 配置指令设置为 LFU 策略之一时，该命令才可用</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public long? ObjectFreq(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullNumber<long>(KeyCommands.ObjectFreq(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command returns the time in seconds since the last access to the value stored at key
        /// <para>The command is only available when the maxmemory-policy configuration directive is not set to one of the LFU policies</para>
        /// <para>Available since: 2.2.3</para>
        /// <para>获得指定Key自上次被访问后的闲置时间. 单位: 秒</para>
        /// <para>支持此命令的Redis版本, 2.2.3+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public long? ObjectIdleTime(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullNumber<long>(KeyCommands.ObjectIdleTime(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command returns the reference count of the stored at key
        /// <para>Available since: 2.2.3</para>
        /// <para>获得指定Key的引用计数</para>
        /// <para>支持此命令的Redis版本, 2.2.3+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public long? ObjectRefCount(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullNumber<long>(KeyCommands.ObjectRefCount(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Atomically transfer a key from a source Redis instance to a destination Redis instance. On success the key is deleted from the original instance and is guaranteed to exist in the target instance
        /// <para>Available since: 2.6.0</para>
        /// <para>以原子方式将Key从源 Redis 实例传输到目标 Redis 实例。成功后，Key将从源实例中删除，并保证存在于目标实例中</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="host">Destination redis host
        /// <para>目标Redis host</para>
        /// </param>
        /// <param name="port">Destination redis port
        /// <para>目标Redis端口</para>
        /// </param>
        /// <param name="key">Key to migrate<para>要迁移的Key</para></param>
        /// <param name="destinationDb">Migrate to the target Redis database
        /// <para>要迁移到目标Redis的数据库下标</para>
        /// </param>
        /// <param name="timeout">Migration timeout interval, milliseconds
        /// <para>迁移超时时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Migrate(string host, ushort port, string key, ushort destinationDb, ulong timeout, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Migrate(host, port, key, destinationDb, timeout, false, false, null, null), "OK", cancellationToken);
        }

        /// <summary>
        /// Atomically transfer a key from a source Redis instance to a destination Redis instance. On success the key is deleted from the original instance and is guaranteed to exist in the target instance
        /// <para>Available since: 4.0.7</para>
        /// <para>以原子方式将Key从源 Redis 实例传输到目标 Redis 实例。成功后，Key将从源实例中删除，并保证存在于目标实例中</para>
        /// <para>支持此命令的Redis版本, 4.0.7+</para>
        /// </summary>
        /// <param name="host">Destination redis host
        /// <para>目标Redis host</para>
        /// </param>
        /// <param name="port">Destination redis port
        /// <para>目标Redis端口</para>
        /// </param>
        /// <param name="key">Key to migrate<para>要迁移的Key</para></param>
        /// <param name="destinationDb">Migrate to the target Redis database
        /// <para>要迁移到目标Redis的数据库下标</para>
        /// </param>
        /// <param name="timeout">Migration timeout interval, milliseconds
        /// <para>迁移超时时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="copy">Do not remove the key from the local instance
        /// <para>不要在源Redis上删除Key</para>
        /// </param>
        /// <param name="replace">Replace existing key on the remote instance
        /// <para>是否覆盖目标Redis上Key</para>
        /// </param>
        /// <param name="passwrod">Destination redis password
        /// <para>目标Redis认证的密码</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Migrate(string host, ushort port, string key, ushort destinationDb, ulong timeout, bool copy, bool replace, string passwrod, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Migrate(host, port, key, destinationDb, timeout, copy, replace, passwrod, null), "OK", cancellationToken);
        }

        /// <summary>
        /// Atomically transfer a key from a source Redis instance to a destination Redis instance. On success the key is deleted from the original instance and is guaranteed to exist in the target instance
        /// <para>Available since: 6.0.0</para>
        /// <para>以原子方式将Key从源 Redis 实例传输到目标 Redis 实例。成功后，Key将从源实例中删除，并保证存在于目标实例中</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="host">Destination redis host
        /// <para>目标Redis host</para>
        /// </param>
        /// <param name="port">Destination redis port
        /// <para>目标Redis端口</para>
        /// </param>
        /// <param name="key">Key to migrate<para>要迁移的Key</para></param>
        /// <param name="destinationDb">Migrate to the target Redis database
        /// <para>要迁移到目标Redis的数据库下标</para>
        /// </param>
        /// <param name="timeout">Migration timeout interval, milliseconds
        /// <para>迁移超时时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="copy">Do not remove the key from the local instance
        /// <para>不要在源Redis上删除Key</para>
        /// </param>
        /// <param name="replace">Replace existing key on the remote instance
        /// <para>是否覆盖目标Redis上Key</para>
        /// </param>
        /// <param name="passwrod">Destination redis password
        /// <para>目标Redis认证的密码</para>
        /// </param>
        /// <param name="username">Destination redis username
        /// <para>目标Redis认证的用户名</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Migrate(string host, ushort port, string key, ushort destinationDb, ulong timeout, bool copy, bool replace, string passwrod, string username, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Migrate(host, port, key, destinationDb, timeout, copy, replace, passwrod, username), "OK", cancellationToken);
        }

        /// <summary>
        /// Atomically transfer a key from a source Redis instance to a destination Redis instance. On success the key is deleted from the original instance and is guaranteed to exist in the target instance
        /// <para>Available since: 2.6.0 | 3.0.0</para>
        /// <para>以原子方式将Key从源 Redis 实例传输到目标 Redis 实例。成功后，Key将从源实例中删除，并保证存在于目标实例中</para>
        /// <para>支持此命令的Redis版本, 2.6.0+ | 3.0.0+</para>
        /// </summary>
        /// <param name="host">Destination redis host
        /// <para>目标Redis host</para>
        /// </param>
        /// <param name="port">Destination redis port
        /// <para>目标Redis端口</para>
        /// </param>
        /// <param name="key">Key to migrate<para>要迁移的Key</para></param>
        /// <param name="destinationDb">Migrate to the target Redis database
        /// <para>要迁移到目标Redis的数据库下标</para>
        /// </param>
        /// <param name="timeout">Migration timeout interval, milliseconds
        /// <para>迁移超时时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="copy">Available since: 3.0.0. Do not remove the key from the local instance
        /// <para>支持此参数的Redis版本, 3.0.0+. 不要在源Redis上删除Key</para>
        /// </param>
        /// <param name="replace">Available since: 3.0.0. Replace existing key on the remote instance
        /// <para>支持此参数的Redis版本, 3.0.0+. 是否覆盖目标Redis上Key</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Migrate(string host, ushort port, string key, ushort destinationDb, ulong timeout, bool copy, bool replace, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Migrate(host, port, key, destinationDb, timeout, copy, replace, null, null), "OK", cancellationToken);
        }

        /// <summary>
        /// Atomically transfer a key from a source Redis instance to a destination Redis instance. On success the key is deleted from the original instance and is guaranteed to exist in the target instance
        /// <para>Available since: 3.0.6</para>
        /// <para>以原子方式将Key从源 Redis 实例传输到目标 Redis 实例。成功后，Key将从源实例中删除，并保证存在于目标实例中</para>
        /// <para>支持此命令的Redis版本, 3.0.6+</para>
        /// </summary>
        /// <param name="host">Destination redis host
        /// <para>目标Redis host</para>
        /// </param>
        /// <param name="port">Destination redis port
        /// <para>目标Redis端口</para>
        /// </param>
        /// <param name="keys">Keys to migrate<para>要迁移的Key数组</para></param>
        /// <param name="destinationDb">Migrate to the target Redis database
        /// <para>要迁移到目标Redis的数据库下标</para>
        /// </param>
        /// <param name="timeout">Migration timeout interval, milliseconds
        /// <para>迁移超时时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="copy">Do not remove the key from the local instance
        /// <para>不要在源Redis上删除Key</para>
        /// </param>
        /// <param name="replace">Replace existing key on the remote instance
        /// <para>是否覆盖目标Redis上Key</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Migrate(string host, ushort port, string[] keys, ushort destinationDb, ulong timeout, bool copy, bool replace, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Migrate(host, port, string.Empty, destinationDb, timeout, copy, replace, null, null, keys), "OK", cancellationToken);
        }

        /// <summary>
        /// Atomically transfer a key from a source Redis instance to a destination Redis instance. On success the key is deleted from the original instance and is guaranteed to exist in the target instance
        /// <para>Available since: 4.0.7</para>
        /// <para>以原子方式将Key从源 Redis 实例传输到目标 Redis 实例。成功后，Key将从源实例中删除，并保证存在于目标实例中</para>
        /// <para>支持此命令的Redis版本, 4.0.7+</para>
        /// </summary>
        /// <param name="host">Destination redis host
        /// <para>目标Redis host</para>
        /// </param>
        /// <param name="port">Destination redis port
        /// <para>目标Redis端口</para>
        /// </param>
        /// <param name="keys">Keys to migrate<para>要迁移的Key数组</para></param>
        /// <param name="destinationDb">Migrate to the target Redis database
        /// <para>要迁移到目标Redis的数据库下标</para>
        /// </param>
        /// <param name="timeout">Migration timeout interval, milliseconds
        /// <para>迁移超时时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="copy">Do not remove the key from the local instance
        /// <para>不要在源Redis上删除Key</para>
        /// </param>
        /// <param name="replace">Replace existing key on the remote instance
        /// <para>是否覆盖目标Redis上Key</para>
        /// </param>
        /// <param name="passwrod">Destination redis password
        /// <para>目标Redis认证的密码</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Migrate(string host, ushort port, string[] keys, ushort destinationDb, ulong timeout, bool copy, bool replace, string passwrod, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Migrate(host, port, string.Empty, destinationDb, timeout, copy, replace, passwrod, null, keys), "OK", cancellationToken);
        }

        /// <summary>
        /// Atomically transfer a key from a source Redis instance to a destination Redis instance. On success the key is deleted from the original instance and is guaranteed to exist in the target instance
        /// <para>Available since: 6.0.0</para>
        /// <para>以原子方式将Key从源 Redis 实例传输到目标 Redis 实例。成功后，Key将从源实例中删除，并保证存在于目标实例中</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="host">Destination redis host
        /// <para>目标Redis host</para>
        /// </param>
        /// <param name="port">Destination redis port
        /// <para>目标Redis端口</para>
        /// </param>
        /// <param name="keys">Keys to migrate<para>要迁移的Key数组</para></param>
        /// <param name="destinationDb">Migrate to the target Redis database
        /// <para>要迁移到目标Redis的数据库下标</para>
        /// </param>
        /// <param name="timeout">Migration timeout interval, milliseconds
        /// <para>迁移超时时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="copy">Do not remove the key from the local instance
        /// <para>不要在源Redis上删除Key</para>
        /// </param>
        /// <param name="replace">Replace existing key on the remote instance
        /// <para>是否覆盖目标Redis上Key</para>
        /// </param>
        /// <param name="passwrod">Destination redis password
        /// <para>目标Redis认证的密码</para>
        /// </param>
        /// <param name="username">Destination redis username
        /// <para>目标Redis认证的用户名</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Migrate(string host, ushort port, string[] keys, ushort destinationDb, ulong timeout, bool copy, bool replace, string passwrod, string username, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(KeyCommands.Migrate(host, port, string.Empty, destinationDb, timeout, copy, replace, passwrod, username, keys), "OK", cancellationToken);
        }

        /// <summary>
        /// Iterates keys
        /// <para>Available since: 2.8.0</para>
        /// <para>迭代Redis中的Key</para>
        /// <para>支持此命令的Redis版本, 2.8.0+</para>
        /// </summary>
        /// <param name="cursor">cursor</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public ScanValue<string[]>? Scan(long cursor, CancellationToken cancellationToken = default)
#else
        public ScanValue<string[]> Scan(long cursor, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallScan<string[]>(KeyCommands.Scan(cursor, null, null, null), ResultType.Scan | ResultType.Array | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Iterates keys
        /// <para>Available since: 2.8.0</para>
        /// <para>迭代Redis中的Key</para>
        /// <para>支持此命令的Redis版本, 2.8.0+</para>
        /// </summary>
        /// <param name="cursor">cursor</param>
        /// <param name="count">count</param>
        /// <param name="pattern">pattern</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public ScanValue<string[]>? Scan(long cursor, string? pattern, ulong? count, CancellationToken cancellationToken = default)
#else
        public ScanValue<string[]> Scan(long cursor, string pattern, ulong? count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallScan<string[]>(KeyCommands.Scan(cursor, pattern, count, null), ResultType.Scan | ResultType.Array | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Iterates keys
        /// <para>Available since: 2.8.0 | 6.0.0</para>
        /// <para>迭代Redis中的Key</para>
        /// <para>支持此命令的Redis版本, 2.8.0+ | 6.0.0</para>
        /// </summary>
        /// <param name="cursor">cursor</param>
        /// <param name="count">count</param>
        /// <param name="pattern">pattern</param>
        /// <param name="type">Available since: 6.0.0. type
        /// <para>支持此参数的Redis版本: 6.0.0+. 筛选指定的类型</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public ScanValue<string[]>? Scan(long cursor, string? pattern, ulong? count, string? type, CancellationToken cancellationToken = default)
#else
        public ScanValue<string[]> Scan(long cursor, string pattern, ulong? count, string type, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallScan<string[]>(KeyCommands.Scan(cursor, pattern, count, type), ResultType.Scan | ResultType.Array | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns or stores the elements contained in the list, set or sorted set at key
        /// <para>Available since: 1.0.0</para>
        /// <para>对指定Key的元素进行排序并返回</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="byPattern">by pattern</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        /// <param name="getPatterns"></param>
        /// <param name="orderType"></param>
        /// <param name="alpha"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string?[]? Sort(string key, string? byPattern = null, long? offset = null, long? count = null, string[]? getPatterns = null, OrderType orderType = OrderType.Default, bool alpha = false, CancellationToken cancellationToken = default)
#else
        public string[] Sort(string key, string byPattern = null, long? offset = null, long? count = null, string[] getPatterns = null, OrderType orderType = OrderType.Default, bool alpha = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(KeyCommands.Sort(key, byPattern, offset, count, getPatterns, orderType, alpha, null), ResultType.Array | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns or stores the elements contained in the list, set or sorted set at key
        /// <para>Available since: 1.0.0</para>
        /// <para>对指定Key的元素进行排序并返回</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="destinationKey">destination key
        /// <para>保存排序结果的Key</para>
        /// </param>
        /// <param name="byPattern">by pattern</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        /// <param name="getPatterns"></param>
        /// <param name="orderType"></param>
        /// <param name="alpha"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of sorted elements in the destination list
        /// <para>目标Key中已排序的元素数量</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public long Sort(string key, string destinationKey, string? byPattern = null, long? offset = null, long? count = null, string[]? getPatterns = null, OrderType orderType = OrderType.Default, bool alpha = false, CancellationToken cancellationToken = default)
#else
        public long Sort(string key, string destinationKey, string byPattern = null, long? offset = null, long? count = null, string[] getPatterns = null, OrderType orderType = OrderType.Default, bool alpha = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallNumber<long>(KeyCommands.Sort(key, byPattern, offset, count, getPatterns, orderType, alpha, destinationKey), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Read-only variant of the SORT command. It is exactly like the original SORT but refuses the STORE option and can safely be used in read-only replicas
        /// <para>Available since: 7.0.0</para>
        /// <para>SORT 命令的只读变体。它与原始 SORT 完全相同，但拒绝 STORE 选项，并且可以安全地在只读副本中使用</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="byPattern">by pattern</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        /// <param name="getPatterns"></param>
        /// <param name="orderType"></param>
        /// <param name="alpha"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string?[]? SortRo(string key, string? byPattern = null, long? offset = null, long? count = null, string[]? getPatterns = null, OrderType orderType = OrderType.Default, bool alpha = false, CancellationToken cancellationToken = default)
#else
        public string[] SortRo(string key, string byPattern = null, long? offset = null, long? count = null, string[] getPatterns = null, OrderType orderType = OrderType.Default, bool alpha = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(KeyCommands.SortRo(key, byPattern, offset, count, getPatterns, orderType, alpha), ResultType.Array | ResultType.String, cancellationToken);
        }
    }
}
