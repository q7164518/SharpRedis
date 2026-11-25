using SharpRedis.Commands;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis;

public sealed partial class RedisString
{
    /// <summary>
    /// If key already exists and is a string, this command appends the value at the end of the string. If key does not exist it is created and set as an empty string
    /// <para>Available since: 2.0.0</para>
    /// <para>在指定Key的后面追加值, 如果Key不存在, 则先赋为""再追加</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the string after the append operation
    /// <para>追加之后, 新值的长度</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Append">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Append">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> AppendAsync(SharpValue key, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Append(key, value) | flags, cancellationToken);

    /// <summary>
    /// Decrements the number stored at key by one. If the key does not exist, it is set to 0 before performing the operation. This operation is limited to 64 bit signed integers.
    /// <para>Available since: 1.0.0</para>
    /// <para>将指定Key里面的数值递减1, 如果Key不存在, 则先赋值为0再递减1. 该操作只适用于64位有符号的整数</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the key after decrementing it.
    /// <para>递减之后Key的数值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/decr">Command documentation</see>
    /// <para><see href="https://redis.io/commands/decr">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> DecrAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Decr(key) | flags, cancellationToken);

    /// <summary>
    /// The DECRBY command reduces the value stored at the specified key by the specified decrement. If the key does not exist, it is initialized with a value of 0 before performing the operation. This operation is limited to 64 bit signed integers.
    /// <para>Available since: 1.0.0</para>
    /// <para>将Key里面的数值递减指定的值, 如果Key不存在, 则先赋值为0再递减. 该操作只适用于64位有符号的整数</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="decrement">The value to be decrement
    /// <para>要减去的数值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the key after decrementing it.
    /// <para>递减之后Key的数值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/DecrBy">Command documentation</see>
    /// <para><see href="https://redis.io/commands/DecrBy">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> DecrByAsync(SharpValue key, long decrement, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.DecrBy(key, decrement) | flags, cancellationToken);

    /// <summary>
    /// Conditionally removes the specified key based on value or hash digest comparison. The following four condition parameters can only be used one at a time.
    /// <para>Available since: 8.4.0</para>
    /// <para>根据指定的值或者值的哈希摘要, 有条件删除指定的Key. 四个条件参数, 只能使用一个</para>
    /// <para>支持此命令的Redis版本, 8.4.0+</para>
    /// </summary>
    /// <param name="key">The key to be deleted
    /// <para>要删除的Key</para>
    /// </param>
    /// <param name="condition">The conditions, values or hash summaries that are to be deleted
    /// <para>删除的条件, 指定的值或者哈希摘要</para>
    /// </param>
    /// <param name="conditionFlag">The comparison method for deleting conditions
    /// <para>删除条件的使用方式</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Was the deletion successful
    /// <para>Key是否删除成功</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/DelEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/DelEx">命令文档</see></para>
    /// </remarks>
    public Task<bool> DelExAsync(SharpValue key, SharpValue condition, ValueCondition conditionFlag, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.DelEx(key, condition, conditionFlag) | flags, cancellationToken);

    /// <summary>
    /// Get the hash digest for the value stored in the specified key as a hexadecimal string. Keys must be of type string.
    /// <para>Available since: 8.4.0</para>
    /// <para>获取Key中值的哈希摘要（以十六进制字符串形式呈现）。Key必须为字符串类型。</para>
    /// <para>支持此命令的Redis版本, 8.4.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The hash digest of the value stored in the key as a hexadecimal string
    /// <para>值的哈希摘要, Key不存在返回null</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Digest">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Digest">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> DigestAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Digest(key) | flags, cancellationToken);

    /// <summary>
    /// Get the value of key. If the key does not exist the special value null is returned. An error is returned if the value stored at key is not a string, because GET only handles string values.
    /// <para>Available since: 1.0.0</para>
    /// <para>获取 key 的值。如果键不存在，则返回null 。如果存储在 key 的值不是字符串，则返回错误，因为 GET 只处理字符串值</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Get">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Get">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> GetAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Get(key) | flags, cancellationToken);

    /// <summary>
    /// Get the value of key and delete the key.
    /// <para>Available since: 6.2.0</para>
    /// <para>获得并删除指定Key的值</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the key.
    /// <para>Key的值, Key不存在返回null</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GetDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GetDel">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> GetDelAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.GetDel(key) | flags, cancellationToken);

    /// <summary>
    /// Get the value of key and optionally set its expiration.
    /// <para>Available since: 6.2.0</para>
    /// <para>获得指定Key的值, 且可以顺带设置过期时间 (非必须设置, 不设置就是单纯获取值)</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="expiry">The due date can be passed as one of the five types: TimeSpan | DateTime | DateTimeOffset | long | int
    /// <para>到期时间, 可以使用TimeSpan | DateTime | DateTimeOffset | long | int 五个类型之一</para>
    /// </param>
    /// <param name="expiryFlags">Effective time processing method, default indicates the number of seconds until expiration
    /// <para>有效时间处理方式, 默认为有效期秒数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the key.
    /// <para>Key的值, Key不存在返回null</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GetEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GetEx">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> GetExAsync(SharpValue key, SharpExpiry? expiry = null, ExpirationFlags expiryFlags = ExpirationFlags.EX, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.GetEx(key, expiry, expiryFlags) | flags, cancellationToken);

    /// <summary>
    /// Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive)
    /// <para>Available since: 2.4.0</para>
    /// <para>获得Key中指定范围的值, 包含指定的开始和结束位置</para>
    /// <para>支持此命令的Redis版本, 2.4.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="start">Offsets start. -1 means the last character, -2 the penultimate and so forth.
    /// <para>开始偏移量(下标), -1表示倒数第一个, -2表示倒数第二个, 以此类推</para>
    /// </param>
    /// <param name="end">Offsets end. -1 means the last character, -2 the penultimate and so forth.
    /// <para>结束偏移量(下标), -1表示倒数第一个, -2表示倒数第二个, 以此类推</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The substring of the string value stored at key, determined by the offsets start and end (both are inclusive).
    /// <para>Key中指定范围内的值, 包含指定的开始和结束位置</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GetRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GetRange">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> GetRangeAsync(SharpValue key, long start, long end, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.GetRange(key, start, end) | flags, cancellationToken);

    /// <summary>
    /// Atomically sets key to value and returns the old value stored at key. Returns an error when key exists but does not hold a string value. Any previous time to live associated with the key is discarded on successful SET operation.
    /// <para>Available since: 1.0.0 (As of Redis version 6.2.0, this command is regarded as deprecated.)</para>
    /// <para>原子性地设置Key的值为指定的Value, 并返回以前Key中存储的值. 如果Key存在但不是字符串类型, 则返回错误. 并覆盖以前设置的过期时间, 设置为不过期</para>
    /// <para>支持此命令的Redis版本, 1.0.0+ (在Redis 6.2.0+及以上的版本中, 此命令被视为过期, 不推荐使用)</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="newValue">New value
    /// <para>要覆盖的新值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The old value stored at the key.
    /// <para>Key中存储的老值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GetSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GetSet">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> GetSetAsync(SharpValue key, SharpValue newValue, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.GetSet(key, newValue) | flags, cancellationToken);

    /// <summary>
    /// Increments the number stored at key by one. If the key does not exist, it is set to 0 before performing the operation. This operation is limited to 64 bit signed integers.
    /// <para>Available since: 1.0.0</para>
    /// <para>将Key里面的数值递增1, 如果Key不存在, 则先赋值为0再递增1. 该操作只适用于64位有符号的整数</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the key after the increment.
    /// <para>递增1之后Key的数值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Incr">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Incr">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> IncrAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Incr(key) | flags, cancellationToken);

    /// <summary>
    /// Increments the number stored at key by increment. If the key does not exist, it is set to 0 before performing the operation. This operation is limited to 64 bit signed integers.
    /// <para>Available since: 1.0.0</para>
    /// <para>将Key里面的数值递增指定的值, 如果Key不存在, 则先赋值为0再递增. 该操作只适用于64位有符号的整数</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="increment">The value to be increment
    /// <para>要递增的数值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the key after the increment.
    /// <para>递增之后Key的数值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/IncrBy">Command documentation</see>
    /// <para><see href="https://redis.io/commands/IncrBy">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> IncrByAsync(SharpValue key, long increment, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.IncrBy(key, increment) | flags, cancellationToken);

    /// <summary>
    /// Increment the string representing a floating point number stored at key by the specified increment. If the key does not exist, it is set to 0 before performing the operation
    /// <para>Available since: 2.6.0</para>
    /// <para>将Key里面的数值递增指定的小数, 如果Key不存在, 则先赋值为0再递增.</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="increment">The value to be increment
    /// <para>要递增的数值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the key after the increment.
    /// <para>递增之后Key的数值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/IncrByFloat">Command documentation</see>
    /// <para><see href="https://redis.io/commands/IncrByFloat">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> IncrByFloatAsync(SharpValue key, double increment, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.IncrByFloat(key, increment) | flags, cancellationToken);

    /// <summary>
    /// Decrement the string representing a floating point number stored at key by the specified increment. If the key does not exist, it is set to 0 before performing the operation
    /// <para>Available since: 2.6.0 <b>(Redis itself does not have this command.)</b></para>
    /// <para>将Key里面的数值递减指定的小数, 如果Key不存在, 则先赋值为0再递增.</para>
    /// <para>支持此命令的Redis版本, 2.6.0+ <b>Redis本身没有该命令, 这个只是模拟. 自动将参数设为负数实现的</b></para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="decrement">The value to be decrement
    /// <para>要递减的数值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the key after the decrement.
    /// <para>递减之后Key的数值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/IncrByFloat">Command documentation</see>
    /// <para><see href="https://redis.io/commands/IncrByFloat">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> DecrByFloatAsync(SharpValue key, double decrement, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.IncrByFloat(key, -decrement) | flags, cancellationToken);

    /// <summary>
    /// Returns the values of all specified keys.
    /// <para>Available since: 1.0.0</para>
    /// <para>批量获取多个Key的值</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="keys">Key array
    /// <para>要获得的Key数组</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of values at the specified keys.
    /// <para>指定Key数组的返回值数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/MGet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/MGet">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult[]> MGetAsync(ReadOnlySpan<SharpValue> keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.MGet(keys) | flags, cancellationToken)!;

    /// <summary>
    /// Sets the given keys to their respective values. MSET replaces existing values with new values, just as regular SET.
    /// <para>Available since: 1.0.1</para>
    /// <para>批量设置多个值</para>
    /// <para>支持此命令的Redis版本, 1.0.1+</para>
    /// </summary>
    /// <param name="kvList">Key-value pair parameters, which can be passed as arrays, dictionary, etc.
    /// <para>键值对参数, 可传入数组, 字典等类型</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Always OK because MSET can't fail. Unless the kvList parameter is empty
    /// <para>理论上永远返回true, 因为MSET不会失败. 除非传入的kvList参数是空的</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/MSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/MSet">命令文档</see></para>
    /// </remarks>
    public Task<bool> MSetAsync(IEnumerable<KeyValuePair<SharpValue, SharpValue>> kvList, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        if (kvList is null) return Task.FromResult(false);
        return base.InvokeAsync(StringCommands.MSet(kvList) | flags, cancellationToken);
    }

    /// <summary>
    /// Atomically sets multiple string keys with an optional shared expiration in a single operation.
    /// <para>Available since: 8.4.0</para>
    /// <para>批量设置多个值, 并可顺带统一设置过期时间(非必须设置)</para>
    /// <para>支持此命令的Redis版本, 8.4.0+</para>
    /// </summary>
    /// <param name="kvList">Key-value pair parameters, which can be passed as arrays, dictionary, etc.
    /// <para>键值对参数, 可传入数组, 字典等类型</para>
    /// </param>
    /// <param name="expiry">The due date can be passed as one of the five types: TimeSpan | DateTime | DateTimeOffset | long | int
    /// <para>到期时间, 可以使用TimeSpan | DateTime | DateTimeOffset | long | int 五个类型之一</para>
    /// </param>
    /// <param name="expiryFlags">Effective time processing method, default indicates the number of seconds until expiration
    /// <para>有效时间处理方式, 默认为有效期秒数</para>
    /// </param>
    /// <param name="when">
    /// NX -- Set the keys and their expiration time only if none of the specified keys exist.
    /// <para>XX -- Set the keys and their expiration time only if all of the specified keys already exist.</para>
    /// <para>NX: 所有Key都不存在时, 才执行</para>
    /// <para>XX: 所有Key都存在时, 才执行</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>0(false) if none of the keys were set; 1(true) if all of the keys were set.
    /// <para>所有Key都设置成功返回true, 所有Key没都设置成功返回false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/MSetEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/MSetEx">命令文档</see></para>
    /// </remarks>
    public Task<bool> MSetExAsync(IEnumerable<KeyValuePair<SharpValue, SharpValue>> kvList, SharpExpiry? expiry = null, ExpirationFlags expiryFlags = ExpirationFlags.EX, StringExist when = StringExist.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        if (kvList is null) return Task.FromResult(false);
        return base.InvokeAsync(StringCommands.MSetEx(kvList, expiry, expiryFlags, when) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets the given keys to their respective values. MSETNX will not perform any operation at all even if just a single key already exists.
    /// <para>Available since: 1.0.1</para>
    /// <para>批量设置多个值, 只有当所有Key都不存在的时候才会设置. 只要有一个Key存在, 都不设置</para>
    /// <para>支持此命令的Redis版本, 1.0.1+</para>
    /// </summary>
    /// <param name="kvList">Key-value pair parameters, which can be passed as arrays, dictionary, etc.
    /// <para>键值对参数, 可传入数组, 字典等类型</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>0(false) if no key was set (at least one key already existed). 1(true) if all the keys were set.
    /// <para>false: 指定的Key至少有一个存在, 都没设置成功, true: 所有Key都设置成功</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/MSetNx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/MSetNx">命令文档</see></para>
    /// </remarks>
    public Task<bool> MSetNxAsync(IEnumerable<KeyValuePair<SharpValue, SharpValue>> kvList, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        if (kvList is null) return Task.FromResult(false);
        return base.InvokeAsync(StringCommands.MSetNx(kvList) | flags, cancellationToken);
    }

    /// <summary>
    /// PSETEX works exactly like SETEX with the sole difference that the expire time is specified in milliseconds instead of seconds.
    /// <para>Available since: 2.6.0 (As of Redis version 2.6.12, this command is regarded as deprecated.)</para>
    /// <para>设置一个值, 并同时设置以毫秒为单位的过期时间</para>
    /// <para>支持此命令的Redis版本, 2.6.0+ (Redis 2.6.12及以后的版已将该命令视为过期, 不推荐使用)</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="milliseconds">Effective time, unit: milliseconds
    /// <para>有效时间, 单位: 毫秒</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PSetEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PSetEx">命令文档</see></para>
    /// </remarks>
    public Task<bool> PSetExAsync(SharpValue key, SharpValue value, long milliseconds, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.PSetEx(key, value, milliseconds) | flags, cancellationToken);

    /// <summary>
    /// PSETEX works exactly like SETEX with the sole difference that the expire time is specified in milliseconds instead of seconds.
    /// <para>Available since: 2.6.0 (As of Redis version 2.6.12, this command is regarded as deprecated.)</para>
    /// <para>设置一个值, 并同时设置以毫秒为单位的过期时间</para>
    /// <para>支持此命令的Redis版本, 2.6.0+ (Redis 2.6.12及以后的版已将该命令视为过期, 不推荐使用)</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="milliseconds">Effective time, unit: milliseconds
    /// <para>有效时间, 单位: 毫秒</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PSetEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PSetEx">命令文档</see></para>
    /// </remarks>
    public Task<bool> PSetExAsync(SharpValue key, SharpValue value, TimeSpan milliseconds, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.PSetEx(key, value, (long)milliseconds.TotalMilliseconds) | flags, cancellationToken);

    /// <summary>
    /// Set key to hold the string value and set key to timeout after a given number of seconds
    /// <para>Available since: 2.6.0 (As of Redis version 2.6.12, this command is regarded as deprecated.)</para>
    /// <para>设置一个值, 并同时设置以秒为单位的过期时间</para>
    /// <para>支持此命令的Redis版本, 2.6.0+ (Redis 2.6.12及以后的版已将该命令视为过期, 不推荐使用)</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="seconds">Effective time, unit: seconds
    /// <para>有效时间, 单位: 秒</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SetEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SetEx">命令文档</see></para>
    /// </remarks>
    public Task<bool> SetExAsync(SharpValue key, SharpValue value, long seconds, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.SetEx(key, value, seconds) | flags, cancellationToken);

    /// <summary>
    /// Set key to hold the string value and set key to timeout after a given number of seconds
    /// <para>Available since: 2.6.0 (As of Redis version 2.6.12, this command is regarded as deprecated.)</para>
    /// <para>设置一个值, 并同时设置以秒为单位的过期时间</para>
    /// <para>支持此命令的Redis版本, 2.6.0+ (Redis 2.6.12及以后的版已将该命令视为过期, 不推荐使用)</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="seconds">Effective time, unit: seconds
    /// <para>有效时间, 单位: 秒</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SetEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SetEx">命令文档</see></para>
    /// </remarks>
    public Task<bool> SetExAsync(SharpValue key, SharpValue value, TimeSpan seconds, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.SetEx(key, value, (long)seconds.TotalSeconds) | flags, cancellationToken);

    /// <summary>
    /// Set key to hold string value if key does not exist. In that case, it is equal to SET. When key already holds a value, no operation is performed.
    /// <para>Available since: 1.0.0 (As of Redis version 2.6.12, this command is regarded as deprecated.)</para>
    /// <para>仅在Key不存在的时候才设置值, 如果Key存在, 不执行任何操作</para>
    /// <para>支持此命令的Redis版本, 1.0.0+ (Redis 2.6.12及以后的版已将该命令视为过期, 不推荐使用)</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SetNx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SetNx">命令文档</see></para>
    /// </remarks>
    public Task<bool> SetNxAsync(SharpValue key, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.SetNx(key, value) | flags, cancellationToken);

    /// <summary>
    /// Set key to hold string value if key does not exist. In that case, it is equal to SET. When key already holds a value, no operation is performed.
    /// <para>Available since: 2.6.12</para>
    /// <para>仅在Key不存在的时候才设置值, 如果Key存在, 不执行任何操作</para>
    /// <para>支持此命令的Redis版本, 2.6.12+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="expiry">
    /// <b>Versions of Redis 2.6.12 and above support seconds or milliseconds. Setting an absolute expiration time requires Redis 6.2.0 and above.</b>
    /// <para><b>Redis 2.6.12及以上版本支持秒或毫秒. 设置绝对过期时间需要Redis 6.2.0及以上版本</b></para>
    /// <para>The due date can be passed as one of the five types: TimeSpan | DateTime | DateTimeOffset | long | int</para>
    /// <para>到期时间, 可以使用TimeSpan | DateTime | DateTimeOffset | long | int 五个类型之一</para>
    /// </param>
    /// <param name="expiryFlags">Effective time processing method, default indicates the number of seconds until expiration
    /// <para><b>Starting with Redis version 6.0.0: Added the KEEPTTL option.</b></para>
    /// <para>有效时间处理方式, 默认为有效期秒数</para>
    /// <para><b>从Redis 6.0.0及以上版本才支持KEEPTTL</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Set">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Set">命令文档</see></para>
    /// </remarks>
    public Task<bool> SetNxAsync(SharpValue key, SharpValue value, SharpExpiry expiry, ExpirationFlags expiryFlags = ExpirationFlags.EX, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Set(key, value, StringExist.NX, expiry: expiry, expiryFlags: expiryFlags) | flags, cancellationToken);

    /// <summary>
    /// Overwrites part of the string stored at key, starting at the specified offset, for the entire length of value. If the offset is larger than the current length of the string at key, the string is padded with zero-bytes to make offset fit.
    /// <para>Available since: 2.2.0</para>
    /// <para>从指定的下标(字节偏移数)处, 设置并覆盖范围内的值. 如果指定的下标超出原有值, 会用空字符填充</para>
    /// <para>支持此命令的Redis版本, 2.2.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="offset">offset
    /// <para>下标位置(字节偏移数, 跟下标有区别)</para>
    /// </param>
    /// <param name="value">Value</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the string after it was modified by the command.
    /// <para>设置之后, 值的长度</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SetRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SetRange">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> SetRangeAsync(SharpValue key, long offset, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.SetRange(key, offset, value) | flags, cancellationToken);

    /// <summary>
    /// Returns the length of the string value stored at key. An error is returned when key holds a non-string value.
    /// <para>Available since: 2.2.0</para>
    /// <para>获得指定Key中值的字节长度, 如果指定的Key不是Redis String类型, 会异常</para>
    /// <para>支持此命令的Redis版本, 2.2.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the string stored at key, or 0 when the key does not exist.
    /// <para>Key中值的字节数, 如果Key不存在返回0</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/StrLen">Command documentation</see>
    /// <para><see href="https://redis.io/commands/StrLen">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> StrLenAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.StrLen(key) | flags, cancellationToken);

    /// <summary>
    /// Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive).
    /// <para>Available since: 1.0.0 (As of Redis version 2.0.0, this command is regarded as deprecated.)</para>
    /// <para>获得Key中指定范围的值, 包含指定的开始和结束位置</para>
    /// <para>支持此命令的Redis版本, 1.0.0+ (Redis 2.0.0及以后的版已将该命令视为过期, 不推荐使用)</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="start">Offsets start. -1 means the last character, -2 the penultimate and so forth.
    /// <para>开始偏移量(下标), -1表示倒数第一个, -2表示倒数第二个, 以此类推</para>
    /// </param>
    /// <param name="end">Offsets end. -1 means the last character, -2 the penultimate and so forth.
    /// <para>结束偏移量(下标), -1表示倒数第一个, -2表示倒数第二个, 以此类推</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The substring of the string value stored at key, determined by the offsets start and end (both are inclusive).
    /// <para>Key中指定范围内的值, 包含指定的开始和结束位置</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SubStr">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SubStr">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> SubStrAsync(SharpValue key, long start, long end, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.SubStr(key, start, end) | flags, cancellationToken);

    /// <summary>
    /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Any previous time to live associated with the key is discarded on successful SET operation.
    /// <para>Available since: 1.0.0</para>
    /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 并覆盖以前设置的过期时间, 设置为不过期</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="expiry">
    /// <b>Versions of Redis 2.6.12 and above support seconds or milliseconds. Setting an absolute expiration time requires Redis 6.2.0 and above.</b>
    /// <para><b>Redis 2.6.12及以上版本支持秒或毫秒. 设置绝对过期时间需要Redis 6.2.0及以上版本</b></para>
    /// <para>The due date can be passed as one of the five types: TimeSpan | DateTime | DateTimeOffset | long | int</para>
    /// <para>到期时间, 可以使用TimeSpan | DateTime | DateTimeOffset | long | int 五个类型之一</para>
    /// </param>
    /// <param name="when">
    /// <b>Starting from version 2.6.12 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 2.6.12及以上版本才支持此参数</b></para>
    /// <para>NX: Only set the key if it does not already exist.</para>
    /// <para>XX: Only set the key if it already exists.</para>
    /// <para>NX: 只有Key不存在的时候才会执行</para>
    /// <para>XX: 只有Key存在的时候才会执行</para>
    /// </param>
    /// <param name="expiryFlags">Effective time processing method
    /// <para><b>Starting with Redis version 6.0.0: Added the KEEPTTL option.</b></para>
    /// <para>有效时间处理方式</para>
    /// <para><b>从Redis 6.0.0及以上版本才支持KEEPTTL</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="condition">
    /// <b>Starting from version 8.4.0 of Redis, this parameter has been supported.</b>
    /// <para><b>从 Redis 8.4.0 版本开始支持此参数</b></para>
    /// <para>The set conditions, the specified values or the hash digest</para>
    /// <para>设置的条件, 值或者值的哈希摘要</para>
    /// </param>
    /// <param name="conditionFlag">
    /// <b>Starting from version 8.4.0 of Redis, this parameter has been supported.</b>
    /// <para><b>从 Redis 8.4.0 版本开始支持此参数</b></para>
    /// <para>The usage method of conditions</para>
    /// <para>条件的使用方式</para>
    /// </param>
    /// <param name="get">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>从 Redis 6.2.0 版本开始支持此参数</b></para>
    /// <para>Return the old string stored at key, or nil if key did not exist</para>
    /// <para>是否返回Key中老的值, 如果Key不存在, 返回null</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// If it is a simple Set, return a bool value indicating whether the setting was successful.
    /// <para>If the "get" parameter is set to "true", or if the "condition" parameter is specified, then a string or an empty string will be returned.</para>
    /// <para>如果是简单的Set一个值, 返回bool, 表示设置是否成功</para>
    /// <para>如果指定了get参数为true, 或者指定了condition参数, 返回string</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Set">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Set">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> SetAsync(
        SharpValue key,
        SharpValue value,
        SharpExpiry? expiry,
        ExpirationFlags expiryFlags,
        StringExist when,
        bool get,
        SharpValue? condition,
        ValueCondition? conditionFlag,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Set(
            key: key,
            value: value,
            when: when,
            conditionFlag: conditionFlag,
            condition: condition,
            get: get,
            expiry: expiry,
            expiryFlags: expiryFlags) | flags, cancellationToken);

    /// <summary>
    /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Any previous time to live associated with the key is discarded on successful SET operation.
    /// <para>Available since: 1.0.0</para>
    /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 并覆盖以前设置的过期时间, 设置为不过期</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The key was set.
    /// <para>设置是否成功</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Set">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Set">命令文档</see></para>
    /// </remarks>
    public Task<bool> SetAsync(SharpValue key, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Set(key, value) | flags, cancellationToken);

    /// <summary>
    /// Set key to hold the string value, And at the same time, an expiration time was also set. If key already holds a value, it is overwritten, regardless of its type.
    /// <para>Available since: 2.6.12</para>
    /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型)</para>
    /// <para>支持此命令的Redis版本, 2.6.12+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="expiry">
    /// <b>Versions of Redis 2.6.12 and above support seconds or milliseconds. Setting an absolute expiration time requires Redis 6.2.0 and above.</b>
    /// <para><b>Redis 2.6.12及以上版本支持秒或毫秒. 设置绝对过期时间需要Redis 6.2.0及以上版本</b></para>
    /// <para>The due date can be passed as one of the five types: TimeSpan | DateTime | DateTimeOffset | long | int</para>
    /// <para>到期时间, 可以使用TimeSpan | DateTime | DateTimeOffset | long | int 五个类型之一</para>
    /// </param>
    /// <param name="expiryFlags">Effective time processing method, default indicates the number of seconds until expiration
    /// <para><b>Starting with Redis version 6.0.0: Added the KEEPTTL option.</b></para>
    /// <para>有效时间处理方式, 默认为有效期秒数</para>
    /// <para><b>从Redis 6.0.0及以上版本才支持KEEPTTL</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The key was set.
    /// <para>设置是否成功</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Set">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Set">命令文档</see></para>
    /// </remarks>
    public Task<bool> SetAsync(SharpValue key, SharpValue value, SharpExpiry expiry, ExpirationFlags expiryFlags = ExpirationFlags.EX, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Set(key, value, expiry: expiry, expiryFlags: expiryFlags) | flags, cancellationToken);

    /// <summary>
    /// Set key to hold the string value, Retain the time to live associated with the key. If key already holds a value, it is overwritten, regardless of its type.
    /// <para>Available since: 6.0.0</para>
    /// <para>根据Key设置一个Value, 并保留原有的过期时间. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型).</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The key was set.
    /// <para>设置是否成功</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Set">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Set">命令文档</see></para>
    /// </remarks>
    public Task<bool> SetKeepTtlAsync(SharpValue key, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Set(key, value, expiryFlags: ExpirationFlags.KEEPTTL) | flags, cancellationToken);

    /// <summary>
    /// Set key to hold string value if key does exist. In that case, it is equal to SET. If Key does not exist, no operation will be performed.
    /// <para>Available since: 2.6.12</para>
    /// <para>仅在Key存在的时候才设置值, 如果Key不存在, 不执行任何操作</para>
    /// <para>支持此命令的Redis版本, 2.6.12+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Set">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Set">命令文档</see></para>
    /// </remarks>
    public Task<bool> SetXxAsync(SharpValue key, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Set(key, value, StringExist.XX) | flags, cancellationToken);

    /// <summary>
    /// Set key to hold string value if key does exist. In that case, it is equal to SET. If Key does not exist, no operation will be performed.
    /// <para>Available since: 2.6.12</para>
    /// <para>仅在Key存在的时候才设置值, 如果Key不存在, 不执行任何操作</para>
    /// <para>支持此命令的Redis版本, 2.6.12+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="expiry">
    /// <b>Versions of Redis 2.6.12 and above support seconds or milliseconds. Setting an absolute expiration time requires Redis 6.2.0 and above.</b>
    /// <para><b>Redis 2.6.12及以上版本支持秒或毫秒. 设置绝对过期时间需要Redis 6.2.0及以上版本</b></para>
    /// <para>The due date can be passed as one of the five types: TimeSpan | DateTime | DateTimeOffset | long | int</para>
    /// <para>到期时间, 可以使用TimeSpan | DateTime | DateTimeOffset | long | int 五个类型之一</para>
    /// </param>
    /// <param name="expiryFlags">Effective time processing method, default indicates the number of seconds until expiration
    /// <para><b>Starting with Redis version 6.0.0: Added the KEEPTTL option.</b></para>
    /// <para>有效时间处理方式, 默认为有效期秒数</para>
    /// <para><b>从Redis 6.0.0及以上版本才支持KEEPTTL</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Set">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Set">命令文档</see></para>
    /// </remarks>
    public Task<bool> SetXxAsync(SharpValue key, SharpValue value, SharpExpiry expiry, ExpirationFlags expiryFlags = ExpirationFlags.EX, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Set(key, value, StringExist.XX, expiry: expiry, expiryFlags: expiryFlags) | flags, cancellationToken);

    /// <summary>
    /// Set key to hold string value if key does exist, Retain the time to live associated with the key. In that case, it is equal to SET. If Key does not exist, no operation will be performed.
    /// <para>Available since: 6.0.0</para>
    /// <para>仅在Key存在的时候才设置值, 并保留原来的过期时间. 如果Key不存在, 不执行任何操作</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Set">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Set">命令文档</see></para>
    /// </remarks>
    public Task<bool> SetXxKeepTtlAsync(SharpValue key, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Set(key, value, StringExist.XX, expiryFlags: ExpirationFlags.KEEPTTL) | flags, cancellationToken);

    /// <summary>
    /// Return the longest common subsequence of the two keys
    /// <para>Available since: 7.0.0</para>
    /// <para>返回指定两个Key的最长公共子序列</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="key1">Key1</param>
    /// <param name="key2">Key2</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The longest common subsequence
    /// <para>最长公共子序列</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Lcs">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Lcs">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> LcsAsync(SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Lcs(key1, key2) | flags, cancellationToken);

    /// <summary>
    /// Return the length of the longest common subsequence of these two keys
    /// <para>Available since: 7.0.0</para>
    /// <para>返回指定两个Key的最长公共子序列长度</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="key1">Key1</param>
    /// <param name="key2">Key2</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the longest common subsequence
    /// <para>最长公共子序列的长度</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Lcs">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Lcs">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> LcsLenAsync(SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Lcs(key1, key2, len: true) | flags, cancellationToken);

    /// <summary>
    /// Obtain the matching index information for each group of substrings.
    /// <para>Available since: 7.0.0</para>
    /// <para>获得每组子字符串匹配的下标信息</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="key1">Key1</param>
    /// <param name="key2">Key2</param>
    /// <param name="withMatchLen">Whether to return the length of each group of substring matches
    /// <para>是否返回每组子字符串匹配的长度</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The indices of the substrings that match in each group
    /// <para>每组子字符串匹配的下标</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Lcs">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Lcs">命令文档</see></para>
    /// </remarks>
    public Task<RedisLcsResult> LcsIdxAsync(SharpValue key1, SharpValue key2, bool withMatchLen = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Lcs(key1, key2, idx: true, withMatchLen: withMatchLen) | flags, cancellationToken);

    /// <summary>
    /// Obtain the matching index information for each group of substrings. The minimum length of each group of sub-strings can be specified for filtering.
    /// <para>Available since: 7.0.0</para>
    /// <para>获得每组子字符串匹配的下标信息, 可指定每组子字符串的长度进行筛选</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="key1">Key1</param>
    /// <param name="key2">Key2</param>
    /// <param name="minMatchLen">Minimal length
    /// <para>每组最小长度筛选</para>
    /// </param>
    /// <param name="withMatchLen">Whether to return the length of each group of substring matches
    /// <para>是否返回每组子字符串匹配的长度</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The indices of the substrings that match in each group
    /// <para>每组子字符串匹配的下标</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Lcs">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Lcs">命令文档</see></para>
    /// </remarks>
    public Task<RedisLcsResult> LcsIdxAsync(SharpValue key1, SharpValue key2, long minMatchLen, bool withMatchLen = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StringCommands.Lcs(key1, key2, idx: true, minMatchLen: minMatchLen, withMatchLen: withMatchLen) | flags, cancellationToken);

    /// <summary>
    /// Increments or decrements the numeric value stored at key by the specified amount, with optional upper/lower bounds and expiration control, in a single atomic operation.
    /// <para>Available since: 8.8.0</para>
    /// <para>指定Key中的数值进行递增. 此操作可以限制最大值和最小值, 还可设置过期时间</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key to increment
    /// <para>要递增的Key</para>
    /// </param>
    /// <param name="increment">The value to be increment. The increment may be negative to decrement the value.
    /// <para>要递增的数值. 如果传入负数, 表示递减</para>
    /// </param>
    /// <param name="lowerbound">Sets a lower bound for the resulting value. If the computed result would fall below lowerbound, the operation is skipped and the reply is [current_value, 0] (or use the SATURATE flag to floor the result at lowerbound instead).
    /// <para>递增之后的最小值. 如果递增后结果会小于该值, 则跳过操作, 不进行递增. 如果saturate设置为true, 则表示向下取舍到该值</para>
    /// </param>
    /// <param name="upperbound">Sets an upper bound for the resulting value. If the computed result would exceed upperbound, the operation is skipped and the reply is [current_value, 0] (or use the SATURATE flag to cap the result at upperbound instead).
    /// <para>递增之后的最大值. 如果递增后结果会大于该值, 则跳过操作, 不进行递增. 如果saturate设置为true, 则表示向上取舍到该值</para>
    /// </param>
    /// <param name="saturate">When specified, an out-of-bounds result is capped at UBOUND or floored at LBOUND (or saturated to the type limits when no explicit bound is given).
    /// <para>是否设置: 递增后的结果超出了最大或最小值范围, 则结果只向上或向下取舍. 如: 当前值为10, 设置了最大值为12, 本次递增了5, 如果设置true, 则结果会向上取舍为12, 如果设置为false, 则不进行递增操作</para>
    /// </param>
    /// <param name="expiry">The due date can be passed as one of the five types: TimeSpan | DateTime | DateTimeOffset | long | int
    /// <para>到期时间, 可以使用TimeSpan | DateTime | DateTimeOffset | long | int 五个类型之一</para>
    /// </param>
    /// <param name="expiryFlags">Effective time processing method, default indicates the number of seconds until expiration
    /// <para>到期时间处理方式, 默认为秒数</para>
    /// </param>
    /// <param name="enx">Only sets the TTL/expiration if the key currently has no TTL/expiration.
    /// <para>是否仅在Key有到期时间的时候才设置过期时间</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/IncrEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/IncrEx">命令文档</see></para>
    /// </remarks>
    public Task<IncrExResult> IncrExAsync(
        SharpValue key,
        IncrExNumber? increment = null,
        IncrExNumber? lowerbound = null,
        IncrExNumber? upperbound = null,
        bool saturate = false,
        SharpExpiry? expiry = null,
        ExpirationFlags expiryFlags = ExpirationFlags.EX,
        bool enx = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StringCommands.IncrEx(
            key: key,
            increment: increment,
            lowerbound: lowerbound,
            upperbound: upperbound,
            saturate: saturate,
            expiry: expiry,
            expiryFlags: expiryFlags,
            enx: enx) | flags, cancellationToken);
    }
}
