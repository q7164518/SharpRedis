using SharpRedis.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis;

public sealed partial class RedisHash
{
    /// <summary>
    /// Sets the specified fields to their respective values in the hash stored at key.
    /// <para>Available since: 2.0.0</para>
    /// <para>设置一个field的值</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field
    /// <para>Hash中的字段名称</para>
    /// </param>
    /// <param name="value">value</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of fields that were added.
    /// <para>成功设置的字段数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HSet">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> HSetAsync(SharpValue key, SharpValue field, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HSet(key, new KeyValuePair<SharpValue, SharpValue>(field, value)) | flags, cancellationToken);

    /// <summary>
    /// Sets the specified fields to their respective values in the hash stored at key.
    /// <para>Available since: 2.0.0</para>
    /// <para>指定Key的Hash中设置一个或多个值</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="kv">Key-value pair parameters, which can be passed as arrays, dictionary, etc.
    /// <para>Starting with Redis version 4.0.0: Accepts multiple field and value arguments.</para>
    /// <para>键值对参数, 可传入数组, 字典等类型. Redis 4.0.0及以上版本才支持同时设置多个字段值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of fields that were added.
    /// <para>成功设置的字段数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HSet">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> HSetAsync(SharpValue key, ICollection<KeyValuePair<SharpValue, SharpValue>> kv, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HSet(key, kv) | flags, cancellationToken);

    /// <summary>
    /// Returns the value associated with field in the hash stored at key.
    /// <para>Available since: 2.0.0</para>
    /// <para>获得Key中指定字段的值</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash Key</param>
    /// <param name="field">field<para>字段名称</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value associated with the field
    /// <para>字段里面的值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HGet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HGet">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> HGetAsync(SharpValue key, SharpValue field, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HGet(key, field) | flags, cancellationToken);

    /// <summary>
    /// Removes the specified fields from the hash stored at key.
    /// <para>Available since: 2.0.0</para>
    /// <para>删除Key中指定的字段</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>要删除的字段</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of fields that were removed from the hash, excluding any specified but non-existing fields.
    /// <para>成功删除的字段数量, 不包含不存在的字段</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HDel">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> HDelAsync(SharpValue key, SharpValue field, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HDel(key, field) | flags, cancellationToken);

    /// <summary>
    /// Removes the specified fields from the hash stored at key.
    /// <para>Available since: 2.0.0</para>
    /// <para>删除Key中指定的字段</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields<para>要删除的字段集合</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of fields that were removed from the hash, excluding any specified but non-existing fields.
    /// <para>成功删除的字段数量, 不包含不存在的字段</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HDel">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> HDelAsync(SharpValue key, SharpValueSpan fields, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HDel(key, fields) | flags, cancellationToken);

    /// <summary>
    /// Returns if field is an existing field in the hash stored at key.
    /// <para>Available since: 2.0.0</para>
    /// <para>判断Key中指定的字段是否存在</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>要判断是否存在的字段名称</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if the hash does not contain the field, or the key does not exist. True if the hash contains the field.
    /// <para>字段存在返回true, Key或者field不存在都返回false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HExists">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HExists">命令文档</see></para>
    /// </remarks>
    public Task<bool> HExistsAsync(SharpValue key, SharpValue field, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HExists(key, field) | flags, cancellationToken);

    /// <summary>
    /// Set an expiration on one field of a given hash key
    /// <para>Available since: 7.4.0</para>
    /// <para>给指定的字段设置一个有效期</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>要设置有效期的字段名称</para></param>
    /// <param name="seconds">Effective time, unit: seconds
    /// <para>有效时间, 单位: 秒</para>
    /// </param>
    /// <param name="when">
    /// NX -- Set expiration only when the field has no expiration.
    /// <para>XX -- Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HExpire">命令文档</see></para>
    /// </remarks>
    public Task<bool> HExpireAsync(SharpValue key, SharpValue field, long seconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HExpire(key, seconds, when, field) | flags, cancellationToken);

    /// <summary>
    /// Set an expiration on one field of a given hash key
    /// <para>Available since: 7.4.0</para>
    /// <para>给指定的字段设置一个有效期</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>要设置有效期的字段名称</para></param>
    /// <param name="seconds">Effective time, unit: seconds
    /// <para>有效时间, 单位: 秒</para>
    /// </param>
    /// <param name="when">
    /// NX -- Set expiration only when the field has no expiration.
    /// <para>XX -- Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HExpire">命令文档</see></para>
    /// </remarks>
    public Task<bool> HExpireAsync(SharpValue key, SharpValue field, TimeSpan seconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HExpire(key, (long)seconds.TotalSeconds, when, field) | flags, cancellationToken);

    /// <summary>
    /// Set an expiration on one or more fields of a given hash key.
    /// <para>Available since: 7.4.0</para>
    /// <para>给指定的多个字段设置一个有效期</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields<para>要设置有效期的字段名称集合</para></param>
    /// <param name="seconds">Effective time, unit: seconds
    /// <para>有效时间, 单位: 秒</para>
    /// </param>
    /// <param name="when">
    /// NX -- For each specified field, Set expiration only when the field has no expiration.
    /// <para>XX -- For each specified field, Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- For each specified field, Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- For each specified field, Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 针对每个字段, 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 针对每个字段, 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 针对每个字段, 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 针对每个字段, 仅在新的到期时间小于字段当前到期时间才设置有效时间</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HExpire">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> HExpireAsync(SharpValue key, SharpValueSpan fields, long seconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HExpire(key, seconds, when, fields) | flags, cancellationToken);

    /// <summary>
    /// Set an expiration on one or more fields of a given hash key.
    /// <para>Available since: 7.4.0</para>
    /// <para>给指定的多个字段设置一个有效期</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields<para>要设置有效期的字段名称集合</para></param>
    /// <param name="seconds">Effective time, unit: seconds
    /// <para>有效时间, 单位: 秒</para>
    /// </param>
    /// <param name="when">
    /// NX -- For each specified field, Set expiration only when the field has no expiration.
    /// <para>XX -- For each specified field, Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- For each specified field, Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- For each specified field, Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 针对每个字段, 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 针对每个字段, 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 针对每个字段, 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 针对每个字段, 仅在新的到期时间小于字段当前到期时间才设置有效时间</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HExpire">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> HExpireAsync(SharpValue key, SharpValueSpan fields, TimeSpan seconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HExpire(key, (long)seconds.TotalSeconds, when, fields) | flags, cancellationToken);

    /// <summary>
    /// Set an expiration on one field of a given hash key
    /// <para>Available since: 7.4.0</para>
    /// <para>给指定的字段设置一个有效期</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>要设置有效期的字段名称</para></param>
    /// <param name="milliseconds">Effective time, unit: milliseconds
    /// <para>有效时间, 单位: 毫秒</para>
    /// </param>
    /// <param name="when">
    /// NX -- Set expiration only when the field has no expiration.
    /// <para>XX -- Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPExpire">命令文档</see></para>
    /// </remarks>
    public Task<bool> HPExpireAsync(SharpValue key, SharpValue field, long milliseconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPExpire(key, milliseconds, when, field) | flags, cancellationToken);

    /// <summary>
    /// Set an expiration on one field of a given hash key
    /// <para>Available since: 7.4.0</para>
    /// <para>给指定的字段设置一个有效期</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>要设置有效期的字段名称</para></param>
    /// <param name="milliseconds">Effective time, unit: milliseconds
    /// <para>有效时间, 单位: 毫秒</para>
    /// </param>
    /// <param name="when">
    /// NX -- Set expiration only when the field has no expiration.
    /// <para>XX -- Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPExpire">命令文档</see></para>
    /// </remarks>
    public Task<bool> HPExpireAsync(SharpValue key, SharpValue field, TimeSpan milliseconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPExpire(key, (long)milliseconds.TotalMilliseconds, when, field) | flags, cancellationToken);

    /// <summary>
    /// Set an expiration on one or more fields of a given hash key.
    /// <para>Available since: 7.4.0</para>
    /// <para>给指定的多个字段设置一个有效期</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields<para>要设置有效期的字段名称集合</para></param>
    /// <param name="milliseconds">Effective time, unit: milliseconds
    /// <para>有效时间, 单位: 毫秒</para>
    /// </param>
    /// <param name="when">
    /// NX -- For each specified field, Set expiration only when the field has no expiration.
    /// <para>XX -- For each specified field, Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- For each specified field, Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- For each specified field, Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 针对每个字段, 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 针对每个字段, 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 针对每个字段, 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 针对每个字段, 仅在新的到期时间小于字段当前到期时间才设置有效时间</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPExpire">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> HPExpireAsync(SharpValue key, SharpValueSpan fields, long milliseconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPExpire(key, milliseconds, when, fields) | flags, cancellationToken);

    /// <summary>
    /// Set an expiration on one or more fields of a given hash key.
    /// <para>Available since: 7.4.0</para>
    /// <para>给指定的多个字段设置一个有效期</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields<para>要设置有效期的字段名称集合</para></param>
    /// <param name="milliseconds">Effective time, unit: milliseconds
    /// <para>有效时间, 单位: 毫秒</para>
    /// </param>
    /// <param name="when">
    /// NX -- For each specified field, Set expiration only when the field has no expiration.
    /// <para>XX -- For each specified field, Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- For each specified field, Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- For each specified field, Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 针对每个字段, 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 针对每个字段, 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 针对每个字段, 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 针对每个字段, 仅在新的到期时间小于字段当前到期时间才设置有效时间</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPExpire">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> HPExpireAsync(SharpValue key, SharpValueSpan fields, TimeSpan milliseconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPExpire(key, (long)milliseconds.TotalMilliseconds, when, fields) | flags, cancellationToken);

    /// <summary>
    /// Returns the absolute Unix timestamp in seconds since Unix epoch at which the given key's field will expire.
    /// <para>Available since: 7.4.0</para>
    /// <para>获得指定字段的绝对到期时间, 以秒级时间戳形式返回</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>字段名称</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>-2 if no such field exists in the provided hash key, or the provided key does not exist.
    /// <para>-1 if the field exists but has no associated expiration set.</para>
    /// <para>The expiration (Unix timestamp) in seconds.</para>
    /// <para>到时间的秒级时间戳, Hash不存在或者field不存在返回-2, field没有过期时间返回-1.</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HExpireTime">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HExpireTime">命令文档</see></para>
    /// </remarks>
    public Task<long> HExpireTimeAsync(SharpValue key, SharpValue field, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HExpireTime(key, field) | flags, cancellationToken);

    /// <summary>
    /// Returns the absolute Unix timestamp in seconds since Unix epoch at which the given key's fields will expire.
    /// <para>Available since: 7.4.0</para>
    /// <para>获得指定字段的绝对到期时间, 以秒级时间戳形式返回 (可一次性指定多个)</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields<para>字段名称集合</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>-2 if no such field exists in the provided hash key, or the provided key does not exist.
    /// <para>-1 if the field exists but has no associated expiration set.</para>
    /// <para>The expiration (Unix timestamp) in seconds.</para>
    /// <para>到时间的秒级时间戳, Hash不存在或者field不存在返回-2, field没有过期时间返回-1.</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HExpireTime">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HExpireTime">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber[]> HExpireTimeAsync(SharpValue key, SharpValueSpan fields, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HExpireTime(key, fields) | flags, cancellationToken)!;

    /// <summary>
    /// Returns the absolute Unix timestamp in seconds since Unix epoch at which the given key's field will expire.
    /// <para>Available since: 7.4.0</para>
    /// <para>获得指定字段的绝对到期时间, 以毫秒级时间戳形式返回</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>字段名称</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>-2 if no such field exists in the provided hash key, or the provided key does not exist.
    /// <para>-1 if the field exists but has no associated expiration set.</para>
    /// <para>The expiration (Unix timestamp) in milliseconds.</para>
    /// <para>到时间的毫秒级时间戳, Hash不存在或者field不存在返回-2, field没有过期时间返回-1.</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPExpireTime">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPExpireTime">命令文档</see></para>
    /// </remarks>
    public Task<long> HPExpireTimeAsync(SharpValue key, SharpValue field, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPExpireTime(key, field) | flags, cancellationToken);

    /// <summary>
    /// Returns the absolute Unix timestamp in seconds since Unix epoch at which the given key's fields will expire.
    /// <para>Available since: 7.4.0</para>
    /// <para>获得指定字段的绝对到期时间, 以毫秒级时间戳形式返回 (可一次性指定多个)</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields<para>字段名称集合</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>-2 if no such field exists in the provided hash key, or the provided key does not exist.
    /// <para>-1 if the field exists but has no associated expiration set.</para>
    /// <para>The expiration (Unix timestamp) in milliseconds.</para>
    /// <para>到时间的毫秒级时间戳, Hash不存在或者field不存在返回-2, field没有过期时间返回-1.</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPExpireTime">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPExpireTime">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber[]> HPExpireTimeAsync(SharpValue key, SharpValueSpan fields, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPExpireTime(key, fields) | flags, cancellationToken)!;

    /// <summary>
    /// Set the absolute expiration time for a field
    /// <para>Available since: 7.4.0</para>
    /// <para>设置一个字段的绝对到期时间</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field</param>
    /// <param name="expiry">An absolute expiration time
    /// <para>绝对到期时间</para>
    /// </param>
    /// <param name="when">NX -- Set expiration only when the field has no expiration.
    /// <para>XX -- Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<bool> HExpireAtAsync(SharpValue key, SharpValue field, DateTimeOffset expiry, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HExpireAt(key, expiry.ToUnixTimeSeconds(), when, field) | flags, cancellationToken);

    /// <summary>
    /// Set the absolute expiration time for a field
    /// <para>Available since: 7.4.0</para>
    /// <para>设置一个字段的绝对到期时间</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field</param>
    /// <param name="unixTimeSeconds">An absolute expiration time, it takes an absolute Unix timestamp in seconds since Unix epoch
    /// <para>绝对到期时间戳, 以秒为单位的时间戳</para>
    /// </param>
    /// <param name="when">NX -- Set expiration only when the field has no expiration.
    /// <para>XX -- Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<bool> HExpireAtAsync(SharpValue key, SharpValue field, long unixTimeSeconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HExpireAt(key, unixTimeSeconds, when, field) | flags, cancellationToken);

    /// <summary>
    /// Set the absolute expiration time for a fields
    /// <para>Available since: 7.4.0</para>
    /// <para>设置多个字段的绝对到期时间</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields</param>
    /// <param name="expiry">An absolute expiration time
    /// <para>绝对到期时间</para>
    /// </param>
    /// <param name="when">NX -- For each specified field, Set expiration only when the field has no expiration.
    /// <para>XX -- For each specified field, Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- For each specified field, Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- For each specified field, Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> HExpireAtAsync(SharpValue key, SharpValueSpan fields, DateTimeOffset expiry, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HExpireAt(key, expiry.ToUnixTimeSeconds(), when, fields) | flags, cancellationToken);

    /// <summary>
    /// Set the absolute expiration time for a fields
    /// <para>Available since: 7.4.0</para>
    /// <para>设置多个字段的绝对到期时间</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields</param>
    /// <param name="unixTimeSeconds">An absolute expiration time, it takes an absolute Unix timestamp in seconds since Unix epoch
    /// <para>绝对到期时间戳, 以秒为单位的时间戳</para>
    /// </param>
    /// <param name="when">NX -- For each specified field, Set expiration only when the field has no expiration.
    /// <para>XX -- For each specified field, Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- For each specified field, Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- For each specified field, Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> HExpireAtAsync(SharpValue key, SharpValueSpan fields, long unixTimeSeconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HExpireAt(key, unixTimeSeconds, when, fields) | flags, cancellationToken);

    /// <summary>
    /// Set the absolute expiration time for a field
    /// <para>Available since: 7.4.0</para>
    /// <para>设置一个字段的绝对到期时间</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field</param>
    /// <param name="expiry">An absolute expiration time
    /// <para>绝对到期时间</para>
    /// </param>
    /// <param name="when">NX -- Set expiration only when the field has no expiration.
    /// <para>XX -- Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<bool> HPExpireAtAsync(SharpValue key, SharpValue field, DateTimeOffset expiry, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPExpireAt(key, expiry.ToUnixTimeMilliseconds(), when, field) | flags, cancellationToken);

    /// <summary>
    /// Set the absolute expiration time for a field
    /// <para>Available since: 7.4.0</para>
    /// <para>设置一个字段的绝对到期时间</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field</param>
    /// <param name="unixTimeMilliseconds">An absolute expiration time, it takes an absolute Unix timestamp in milliseconds since Unix epoch
    /// <para>绝对到期时间戳, 以毫秒为单位的时间戳</para>
    /// </param>
    /// <param name="when">NX -- Set expiration only when the field has no expiration.
    /// <para>XX -- Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<bool> HPExpireAtAsync(SharpValue key, SharpValue field, long unixTimeMilliseconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPExpireAt(key, unixTimeMilliseconds, when, field) | flags, cancellationToken);

    /// <summary>
    /// Set the absolute expiration time for a fields
    /// <para>Available since: 7.4.0</para>
    /// <para>设置多个字段的绝对到期时间</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields</param>
    /// <param name="expiry">An absolute expiration time
    /// <para>绝对到期时间</para>
    /// </param>
    /// <param name="when">NX -- For each specified field, Set expiration only when the field has no expiration.
    /// <para>XX -- For each specified field, Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- For each specified field, Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- For each specified field, Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> HPExpireAtAsync(SharpValue key, SharpValueSpan fields, DateTimeOffset expiry, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPExpireAt(key, expiry.ToUnixTimeMilliseconds(), when, fields) | flags, cancellationToken);

    /// <summary>
    /// Set the absolute expiration time for a fields
    /// <para>Available since: 7.4.0</para>
    /// <para>设置多个字段的绝对到期时间</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields</param>
    /// <param name="unixTimeMilliseconds">An absolute expiration time, it takes an absolute Unix timestamp in milliseconds since Unix epoch
    /// <para>绝对到期时间戳, 以毫秒为单位的时间戳</para>
    /// </param>
    /// <param name="when">NX -- For each specified field, Set expiration only when the field has no expiration.
    /// <para>XX -- For each specified field, Set expiration only when the field has an existing expiration.</para>
    /// <para>GT -- For each specified field, Set expiration only when the new expiration is greater than current one.</para>
    /// <para>LT -- For each specified field, Set expiration only when the new expiration is less than current one.</para>
    /// <para>NX: 仅在字段没有到期时间时才设置有效时间</para>
    /// <para>XX: 仅在字段已有到期时间时才设置有效时间</para>
    /// <para>GT: 仅在新的到期时间大于字段当前到期时间才设置有效时间</para>
    /// <para>LT: 仅在新的到期时间小于字段当前到期时间才设置有效时间</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the expiration time was set/updated.
    /// <para>是否成功设置了过期时间</para></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> HPExpireAtAsync(SharpValue key, SharpValueSpan fields, long unixTimeMilliseconds, ExpirationWhen when = ExpirationWhen.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPExpireAt(key, unixTimeMilliseconds, when, fields) | flags, cancellationToken);

    /// <summary>
    /// Returns all fields and values of the hash stored at key.
    /// <para>Available since: 2.0.0</para>
    /// <para>获得Hash中所有字段的值</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Key-Value</returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HGetAll">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HGetAll">命令文档</see></para>
    /// </remarks>
#if MODERN_NET
    public Task<Dictionary<RedisResult, RedisResult>?> HGetAllAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#else
    public Task<Dictionary<RedisResult, RedisResult>> HGetAllAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#endif
        => base.InvokeAsync(HashCommands.HGetAll(key) | flags, cancellationToken);

    /// <summary>
    /// Get and delete the value of one or more fields of a given hash key.
    /// <para>Available since: 8.0.0</para>
    /// <para>获得并删除指定字段的值</para>
    /// <para>支持此命令的Redis版本, 8.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HGetDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HGetDel">命令文档</see></para>
    /// </remarks>
    public async Task<RedisResult> HGetDelAsync(SharpValue key, SharpValue field, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        var result = await base.InvokeAsync(HashCommands.HGetDel(key, field) | flags, cancellationToken).Task.ConfigureAwait(false);
        if (result.RawValue is RedisResult[] array and { Length: 1 })
        {
            return array[0];
        }
        return RedisResult.Empty;
    }

    /// <summary>
    /// Get and delete the value of one or more fields of a given hash key.
    /// <para>Available since: 8.0.0</para>
    /// <para>获得并删除指定字段的值</para>
    /// <para>支持此命令的Redis版本, 8.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HGetDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HGetDel">命令文档</see></para>
    /// </remarks>
#if MODERN_NET
    public Task<RedisResult[]?> HGetDelAsync(SharpValue key, SharpValueSpan fields, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#else
    public Task<RedisResult[]> HGetDelAsync(SharpValue key, SharpValueSpan fields, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#endif
        => base.InvokeAsync(HashCommands.HGetDel(key, fields) | flags, cancellationToken);

    /// <summary>
    /// Get the value of one field of a given hash key and optionally set their expiration time or time-to-live (TTL).
    /// <para>Available since: 8.0.0</para>
    /// <para>获得指定字段的值, 且可选的设置字段过期时间</para>
    /// <para>支持此命令的Redis版本, 8.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field</param>
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
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HGetEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HGetEx">命令文档</see></para>
    /// </remarks>
    public async Task<RedisResult> HGetExAsync(SharpValue key, SharpValue field, SharpExpiry? expiry = null, ExpirationFlags expiryFlags = ExpirationFlags.EX, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        var result = await base.InvokeAsync(HashCommands.HGetEx(key, expiry, expiryFlags, field) | flags, cancellationToken).Task.ConfigureAwait(false);
        if (result.RawValue is RedisResult[] array and { Length: 1 })
        {
            return array[0];
        }
        throw new InvalidCastException("The return value does not match the conversion.");
    }

    /// <summary>
    /// Get the value of one or more fields of a given hash key and optionally set their expiration time or time-to-live (TTL).
    /// <para>Available since: 8.0.0</para>
    /// <para>获得指定字段的值, 且可选的设置字段过期时间. 可同时指定多个字段</para>
    /// <para>支持此命令的Redis版本, 8.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields</param>
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
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HGetEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HGetEx">命令文档</see></para>
    /// </remarks>
#if MODERN_NET
    public Task<RedisResult[]?> HGetExAsync(SharpValue key, SharpValueSpan fields, SharpExpiry? expiry = null, ExpirationFlags expiryFlags = ExpirationFlags.EX, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#else
    public Task<RedisResult[]> HGetExAsync(SharpValue key, SharpValueSpan fields, SharpExpiry? expiry = null, ExpirationFlags expiryFlags = ExpirationFlags.EX, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#endif
        => base.InvokeAsync(HashCommands.HGetEx(key, expiry, expiryFlags, fields) | flags, cancellationToken);

    /// <summary>
    /// Returns the remaining TTL (time to live) of a hash key's field that have a set expiration. This introspection capability allows you to check how many seconds a given hash field will continue to be part of the hash key.
    /// <para>Available since: 7.4.0</para>
    /// <para>获得指定字段剩余的有效时间, 单位: 秒</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>-2 if no such field exists in the provided hash key, or the provided key does not exist
    /// <para>-1 if the field exists but has no associated expiration set.</para>
    /// <para>The TTL in seconds.</para>
    /// <para>剩余的有效期时间, 单位: 秒. 如果Hash或field不存在返回-2, 如果field永不过期返回-1</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HTtl">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HTtl">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> HTtlAsync(SharpValue key, SharpValue field, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HTtl(key, field) | flags, cancellationToken);

    /// <summary>
    /// Returns the remaining TTL (time to live) of a hash key's field(s) that have a set expiration. This introspection capability allows you to check how many seconds a given hash field will continue to be part of the hash key.
    /// <para>Available since: 7.4.0</para>
    /// <para>获得指定字段剩余的有效时间, 单位: 秒. 可指定多个</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>-2 if no such field exists in the provided hash key, or the provided key does not exist
    /// <para>-1 if the field exists but has no associated expiration set.</para>
    /// <para>The TTL in seconds.</para>
    /// <para>剩余的有效期时间, 单位: 秒. 如果Hash或field不存在返回-2, 如果field永不过期返回-1</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HTtl">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HTtl">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber[]> HTtlAsync(SharpValue key, SharpValueSpan fields, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HTtl(key, fields) | flags, cancellationToken)!;

    /// <summary>
    /// Returns the remaining TTL (time to live) of a hash key's field that have a set expiration. This introspection capability allows you to check how many milliseconds a given hash field will continue to be part of the hash key.
    /// <para>Available since: 7.4.0</para>
    /// <para>获得指定字段剩余的有效时间, 单位: 毫秒</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>-2 if no such field exists in the provided hash key, or the provided key does not exist
    /// <para>-1 if the field exists but has no associated expiration set.</para>
    /// <para>The TTL in milliseconds.</para>
    /// <para>剩余的有效期时间, 单位: 毫秒. 如果Hash或field不存在返回-2, 如果field永不过期返回-1</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPTtl">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPTtl">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> HPTtlAsync(SharpValue key, SharpValue field, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPTtl(key, field) | flags, cancellationToken);

    /// <summary>
    /// Returns the remaining TTL (time to live) of a hash key's field(s) that have a set expiration. This introspection capability allows you to check how many milliseconds a given hash field will continue to be part of the hash key.
    /// <para>Available since: 7.4.0</para>
    /// <para>获得指定字段剩余的有效时间, 单位: 毫秒. 可指定多个</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>-2 if no such field exists in the provided hash key, or the provided key does not exist
    /// <para>-1 if the field exists but has no associated expiration set.</para>
    /// <para>The TTL in milliseconds.</para>
    /// <para>剩余的有效期时间, 单位: 毫秒. 如果Hash或field不存在返回-2, 如果field永不过期返回-1</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPTtl">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPTtl">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber[]> HPTtlAsync(SharpValue key, SharpValueSpan fields, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPTtl(key, fields) | flags, cancellationToken)!;

    /// <summary>
    /// Increments the number stored at field in the hash stored at key by increment. If key does not exist, a new key holding a hash is created. If field does not exist the value is set to 0 before the operation is performed.
    /// <para>Available since: 2.0.0</para>
    /// <para>将指定字段的值进行递增, 如果Hash Key不存在, 会创建Hash, 如果field不存在, 会创建field并从0开始递增. 此操作只支持整数</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field</param>
    /// <param name="increment">The value to increment, with a default value of 1
    /// <para>要递增的值, 默认为1</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the field after the increment operation.
    /// <para>递增后的值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HIncrBy">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HIncrBy">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> HIncrByAsync(SharpValue key, SharpValue field, long increment = 1, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HIncrBy(key, field, increment) | flags, cancellationToken);

    /// <summary>
    /// Increment the specified field of a hash stored at key, and representing a floating point number, by the specified increment. If the increment value is negative, the result is to have the hash field value decremented instead of incremented. If the field does not exist, it is set to 0 before performing the operation.
    /// <para>Available since: 2.6.0</para>
    /// <para>将指定字段的值进行递增, 如果Hash Key不存在, 会创建Hash, 如果field不存在, 会创建field并从0开始递增. 此操作支持整数和浮点数</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field</param>
    /// <param name="increment">The value to increment
    /// <para>要递增的值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the field after the increment operation.
    /// <para>递增后的值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HIncrByFloat">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HIncrByFloat">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> HIncrByFloatAsync(SharpValue key, SharpValue field, double increment, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HIncrByFloat(key, field, increment) | flags, cancellationToken);

    /// <summary>
    /// Returns all field names in the hash stored at key.
    /// <para>Available since: 2.0.0</para>
    /// <para>获得指定Hash中所有的字段名称</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list of fields in the hash, or an empty list when the key does not exist
    /// <para>字段名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HKeys">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HKeys">命令文档</see></para>
    /// </remarks>
#if MODERN_NET
    public Task<RedisResult[]?> HKeysAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#else
    public Task<RedisResult[]> HKeysAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#endif
        => base.InvokeAsync(HashCommands.HKeys(key) | flags, cancellationToken);

    /// <summary>
    /// Returns the number of fields contained in the hash stored at key.
    /// <para>Available since: 2.0.0</para>
    /// <para>获得指定Hash中所有字段数量</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of fields in the hash, or 0 when the key does not exist.
    /// <para>Hash中的字段数量, Hash不存在返回0</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HLen">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HLen">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> HLenAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HLen(key) | flags, cancellationToken);

    /// <summary>
    /// Returns the values associated with the specified fields in the hash stored at key.
    /// <para>Available since: 2.0.0</para>
    /// <para>批量获取指定字段的值</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields<para>要获取的字段名称集合</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list of values associated with the given fields, in the same order as they are requested.
    /// <para>字段值数组, 顺序和指定的字段顺序相同</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HMGet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HMGet">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult[]> HMGetAsync(SharpValue key, SharpValueSpan fields, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HMGet(key, fields) | flags, cancellationToken)!;

    /// <summary>
    /// Sets the specified fields to their respective values in the hash stored at key. This command overwrites any specified fields already existing in the hash. If key does not exist, a new key holding a hash is created.
    /// <para>Available since: 2.0.0 <b>[As of Redis version 4.0.0, this command is regarded as deprecated.]</b></para>
    /// <para>批量设置多个字段和值</para>
    /// <para>支持此命令的Redis版本, 2.0.0+ <b>[此命令在Redis4.0.0及以上版本被视为过期]</b></para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="hashEntries">Key-value pair parameters, which can be passed as arrays, dictionary, etc.
    /// <para>键值对参数, 可传入数组, 字典等类型.</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HMSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HMSet">命令文档</see></para>
    /// </remarks>
    public Task<bool> HMSetAsync(SharpValue key, ICollection<KeyValuePair<SharpValue, SharpValue>> hashEntries, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HMSet(key, hashEntries) | flags, cancellationToken);

    /// <summary>
    /// Remove the existing expiration on a hash key field
    /// <para>Available since: 7.4.0</para>
    /// <para>移除一个字段的过期时间</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>要移除过期时间的字段名称</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>1 the expiration was removed.
    /// <para>-1 if the field exists but has no associated expiration set. -2 if no such field exists in the provided hash key, or the provided key does not exist.</para>
    /// <para>移除成功返回1. -1表示本身就没有过期时间, -2表示字段或Hash不存在</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPersist">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPersist">命令文档</see></para>
    /// </remarks>
    public async Task<int> HPersistAsync(SharpValue key, SharpValue field, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        var result = await base.InvokeAsync(HashCommands.HPersist(key, field) | flags, cancellationToken).Task.ConfigureAwait(false);
        if (result.RawValue is RedisResult[] array and { Length: 1 })
        {
            return array[0];
        }
        throw new InvalidCastException("The return value is not an integer and cannot be converted to Int32.");
    }

    /// <summary>
    /// Remove the existing expiration on a hash key fields
    /// <para>Available since: 7.4.0</para>
    /// <para>移除多个字段的过期时间</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="fields">fields<para>要移除过期时间的字段名称集合</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>1 the expiration was removed.
    /// <para>-1 if the field exists but has no associated expiration set. -2 if no such field exists in the provided hash key, or the provided key does not exist.</para>
    /// <para>移除成功返回1. -1表示本身就没有过期时间, -2表示字段或Hash不存在</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HPersist">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HPersist">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber[]> HPersistAsync(SharpValue key, SharpValueSpan fields, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HPersist(key, fields) | flags, cancellationToken)!;

    /// <summary>
    /// Return a random field from the hash value stored at key.
    /// <para>Available since: 6.2.0</para>
    /// <para>随机返回Hash中的字段</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="count">The number of returned items
    /// <para>If the provided count argument is positive, return an array of distinct fields. The array's length is either count or the hash's number of fields (HLEN), whichever is lower.</para>
    /// <para>If called with a negative count, the behavior changes and the command is allowed to return the same field multiple times. In this case, the number of returned fields is the absolute value of the specified count.</para>
    /// <para>要返回的字段个数, 默认返回1个</para>
    /// <para>如果传入正数, 则返回的字段个数不会重复且不会超过Hash字段总数. 如果传入负数, 则表示可能会重复返回, 返回的个数和count数量一致</para>
    /// </param>
    /// <param name="withValues">Whether to return the field values together
    /// <para>是否将字段中的值一起返回</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A single, randomly selected field when the count option is not used. Accept the return value using the string type.
    /// <para>A list containing count fields when the count option is used, or an empty array if the key does not exists. Accept the return value using the string[] or RedisResult[] type.</para>
    /// <para>A list of fields and their values when count and WITHVALUES were both used. Accept the return value using the Dictionary&lt;RedisResult, RedisResult&gt; type.</para>
    /// <para>如果没有指定count, 可以使用string接受返回值. 如果指定了count用string[]或者RedisResult[]接受返回值. 如果指定了withValues用Dictionary&lt;RedisResult, RedisResult&gt;接受返回值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HRandField">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HRandField">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> HRandFieldAsync(SharpValue key, int count = 1, bool withValues = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HRandField(key, count, withValues) | flags, cancellationToken);

    /// <summary>
    /// Set the value of one field of a given hash key and optionally set their expiration time or time-to-live (TTL). If the given key already holds a value, it is overwritten and any previous TTLs associated with the key are discarded.
    /// <para>Available since: 8.0.0</para>
    /// <para>在Hash中设置一个字段的值, 且可以同时设置字段的有效期. 如果字段已经存在, 字段以前的有效期都会丢弃或覆盖</para>
    /// <para>支持此命令的Redis版本, 8.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>字段名称</para></param>
    /// <param name="value">value<para>字段值</para></param>
    /// <param name="expiry">The due date can be passed as one of the five types: TimeSpan | DateTime | DateTimeOffset | long | int
    /// <para>到期时间, 可以使用TimeSpan | DateTime | DateTimeOffset | long | int 五个类型之一</para>
    /// </param>
    /// <param name="expiryFlags">Effective time processing method, default indicates the number of seconds until expiration
    /// <para>有效时间处理方式, 默认为有效期秒数</para>
    /// </param>
    /// <param name="when">FNX -- Only set the fields if none of them already exist.
    /// <para>FXX -- Only set the fields if all of them already exist.</para>
    /// <para>FNX: 指定的字段不存在才设置</para>
    /// <para>FXX: 指定的字段存在才设置</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HSetEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HSetEx">命令文档</see></para>
    /// </remarks>
    public Task<bool> HSetExAsync(SharpValue key, SharpValue field, SharpValue value, SharpExpiry? expiry = null, ExpirationFlags expiryFlags = ExpirationFlags.EX, FieldExistFlags when = FieldExistFlags.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HSetEx(key, when, expiry, expiryFlags, field, value) | flags, cancellationToken);

    /// <summary>
    /// Set the value of one or more fields of a given hash key and optionally set their expiration time or time-to-live (TTL). If the given key already holds a value, it is overwritten and any previous TTLs associated with the key are discarded.
    /// <para>Available since: 8.0.0</para>
    /// <para>在Hash中设置多个字段的值, 且可以同时设置字段的有效期. 如果字段已经存在, 字段以前的有效期都会丢弃或覆盖</para>
    /// <para>支持此命令的Redis版本, 8.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="hashEntries">Key-value pair parameters, which can be passed as arrays, dictionary, etc.
    /// <para>键值对参数, 可传入数组, 字典等类型.</para>
    /// </param>
    /// <param name="expiry">The due date can be passed as one of the five types: TimeSpan | DateTime | DateTimeOffset | long | int
    /// <para>到期时间, 可以使用TimeSpan | DateTime | DateTimeOffset | long | int 五个类型之一</para>
    /// </param>
    /// <param name="expiryFlags">Effective time processing method, default indicates the number of seconds until expiration
    /// <para>有效时间处理方式, 默认为有效期秒数</para>
    /// </param>
    /// <param name="when">FNX -- Only set the fields if none of them already exist.
    /// <para>FXX -- Only set the fields if all of them already exist.</para>
    /// <para>FNX: 指定的所有字段都不存在才设置</para>
    /// <para>FXX: 指定的所有字段都存在才设置</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if no fields were set. True if all the fields wereset.
    /// <para>所有字段都设置成功返回true, 否则返回false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HSetEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HSetEx">命令文档</see></para>
    /// </remarks>
    public Task<bool> HSetExAsync(SharpValue key, ICollection<KeyValuePair<SharpValue, SharpValue>> hashEntries, SharpExpiry? expiry = null, ExpirationFlags expiryFlags = ExpirationFlags.EX, FieldExistFlags when = FieldExistFlags.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HSetEx(key, when, expiry, expiryFlags, hashEntries) | flags, cancellationToken);

    /// <summary>
    /// Sets field in the hash stored at key to value, only if field does not yet exist. If key does not exist, a new key holding a hash is created. If field already exists, this operation has no effect. 
    /// <para>Available since: 2.0.0</para>
    /// <para>仅当Hash中不存在指定字段时, 才设置字段值</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>字段名称</para></param>
    /// <param name="value">value<para>字段的值</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para></param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if the field already exists in the hash and no operation was performed. True if the field is a new field in the hash and the value was set.
    /// <para>设置成功返回true, 否则返回false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HSetNx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HSetNx">命令文档</see></para>
    /// </remarks>
    public Task<bool> HSetNxAsync(SharpValue key, SharpValue field, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HSetNx(key, field, value) | flags, cancellationToken);

    /// <summary>
    /// Returns the string length of the value associated with field in the hash stored at key. If the key or the field do not exist, 0 is returned.
    /// <para>Available since: 3.2.0</para>
    /// <para>获得Hash中指定字段的值的长度(字节个数, 非字符个数)</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="field">field<para>字段名称</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The string length of the value associated with the field, or zero when the field isn't present in the hash or the key doesn't exist at all.
    /// <para>字段值的字节长度, Hash或者字段不存在时, 返回0</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HStrLen">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HStrLen">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> HStrLenAsync(SharpValue key, SharpValue field, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HStrLen(key, field) | flags, cancellationToken);

    /// <summary>
    /// Returns all values in the hash stored at key.
    /// <para>Available since: 2.0.0</para>
    /// <para>返回Hash所有字段的值</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list of values in the hash, or an empty list when the key does not exist
    /// <para>所有字段的值数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HVals">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HVals">命令文档</see></para>
    /// </remarks>
#if MODERN_NET
    public Task<RedisResult[]?> HValsAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#else
    public Task<RedisResult[]> HValsAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#endif
        => base.InvokeAsync(HashCommands.HVals(key) | flags, cancellationToken);

    /// <summary>
    /// HSCAN iterates fields of Hash types and their associated values.
    /// <para>Available since: 2.8.0</para>
    /// <para>遍历Hash里面的值</para>
    /// <para>支持此命令的Redis版本, 2.8.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="pattern">The pattern of keys to get entries for. Effective only for the field
    /// <para>匹配模式 (匹配规则), 仅对字段名称生效</para>
    /// </param>
    /// <param name="cursor">The cursor position to start at. default is 0
    /// <para>开始遍历的游标, 默认为0</para>
    /// </param>
    /// <param name="count">The number of items returned this time, default is 10
    /// <para>本次返回的条目个数, 默认为10个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HScan">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HScan">命令文档</see></para>
    /// </remarks>
    public Task<HashScanResult> HScanAsync(SharpValue key, SharpValue? pattern = null, ulong cursor = 0, uint count = 10, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HScan(key, cursor, pattern, count, false) | flags, cancellationToken);

    /// <summary>
    /// HSCAN iterates fields of Hash types.
    /// <para>Available since: 7.4.0</para>
    /// <para>遍历Hash里面的字段</para>
    /// <para>支持此命令的Redis版本, 7.4.0+</para>
    /// </summary>
    /// <param name="key">Hash key</param>
    /// <param name="pattern">The pattern of keys to get entries for. Effective only for the field
    /// <para>匹配模式 (匹配规则), 仅对字段名称生效</para>
    /// </param>
    /// <param name="cursor">The cursor position to start at. default is 0
    /// <para>开始遍历的游标, 默认为0</para>
    /// </param>
    /// <param name="count">The number of items returned this time, default is 10
    /// <para>本次返回的条目个数, 默认为10个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HScan">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HScan">命令文档</see></para>
    /// </remarks>
    public Task<HashScanNoValuesResult> HScanNoValuesAsync(SharpValue key, SharpValue? pattern = null, ulong cursor = 0, uint count = 10, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(HashCommands.HScan(key, cursor, pattern, count, true) | flags, cancellationToken);
}
