#pragma warning disable IDE0060
#if !LOW_NET
using System.Threading;
#else
#pragma warning disable IDE0130
#endif
using SharpRedis.Commands;
using SharpRedis.Extensions;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Generic;

namespace SharpRedis
{
    /// <summary>
    /// Hash
    /// <para>Redis Hash类型操作类</para>
    /// </summary>
    public sealed partial class RedisHash : BaseType
    {
        internal RedisHash(BaseCall call) : base(call)
        {
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="field">field<para>Hash中的字段名</para></param>
        /// <param name="value">value<para>值</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of fields that were added
        /// <para>成功添加的field数. 如果field已经存在会覆盖值, 但不计数</para>
        /// </returns>
        public long HSet(string key, string field, string value, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HSet(key, field, value), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="fieldValue">field-value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of fields that were added
        /// <para>成功添加的field数. 如果field已经存在会覆盖值, 但不计数</para>
        /// </returns>
        public long HSet(string key, KeyValuePair<string, string> fieldValue, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HSet(key, fieldValue.Key, fieldValue.Value), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="fieldValue">field-value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of fields that were added
        /// <para>成功添加的field数. 如果field已经存在会覆盖值, 但不计数</para>
        /// </returns>
        public long HSet(string key, KeyValuePair<string, byte[]> fieldValue, CancellationToken cancellationToken = default)
        {
            var command = HashCommands.HSet(key)
                .WriteArg(fieldValue.Key)
                .WriteValue(fieldValue.Value);
            return base._call.CallNumber<long>(command, ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="field">field<para>Hash中的字段名</para></param>
        /// <param name="value">value<para>值</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of fields that were added
        /// <para>成功添加的field数. 如果field已经存在会覆盖值, 但不计数</para>
        /// </returns>
        public long HSet(string key, string field, byte[] value, CancellationToken cancellationToken = default)
        {
            var command = HashCommands.HSet(key)
                .WriteArg(field)
                .WriteValue(value);
            return base._call.CallNumber<long>(command, ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 4.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="fieldValues">field-value pair array<para>field-value数组</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of fields that were added.
        /// <para>成功添加的field数. 如果field已经存在会覆盖值, 但不计数</para>
        /// </returns>
        public long HSet(string key, string[] fieldValues, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HSet(key, fieldValues), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 4.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="fieldValue">field - value, Dictionary</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of fields that were added.
        /// <para>成功添加的field数. 如果field已经存在会覆盖值, 但不计数</para>
        /// </returns>
        public long HSet(string key, Dictionary<string, string> fieldValue, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HSet(key, fieldValue), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 4.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="fieldValue">field - value, Dictionary</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of fields that were added.
        /// <para>成功添加的field数. 如果field已经存在会覆盖值, 但不计数</para>
        /// </returns>
        public long HSet(string key, Dictionary<string, byte[]> fieldValue, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HSet(key, fieldValue), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 4.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="fieldValues">field - value array</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of fields that were added.
        /// <para>成功添加的field数. 如果field已经存在会覆盖值, 但不计数</para>
        /// </returns>
        public long HSet(string key, KeyValuePair<string, string>[] fieldValues, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HSet(key, fieldValues), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 4.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="fieldValues">field - value array</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of fields that were added.
        /// <para>成功添加的field数. 如果field已经存在会覆盖值, 但不计数</para>
        /// </returns>
        public long HSet(string key, KeyValuePair<string, byte[]>[] fieldValues, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HSet(key, fieldValues), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key. This command overwrites any specified fields already existing in the hash. If key does not exist, a new key holding a hash is created.
        /// <para>Available since: 2.0.0</para>
        /// <para>批量设置多个字段的值到指定Key的Hash中, 如果存在则覆盖. 不存在则创建再设置值</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="fieldValue">field - value, Dictionary</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool HMSet(string key, Dictionary<string, string> fieldValue, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(HashCommands.HMSet(key, fieldValue), "OK", cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key. This command overwrites any specified fields already existing in the hash. If key does not exist, a new key holding a hash is created.
        /// <para>Available since: 2.0.0</para>
        /// <para>批量设置多个字段的值到指定Key的Hash中, 如果存在则覆盖. 不存在则创建再设置值</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="fieldValue">field - value, Dictionary</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool HMSet(string key, Dictionary<string, byte[]> fieldValue, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(HashCommands.HMSet(key, fieldValue), "OK", cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="fieldValues">field-value pair array<para>field-value数组</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool HMSet(string key, string[] fieldValues, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(HashCommands.HMSet(key, fieldValues), "OK", cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="fieldValues">field-value pair array<para>field-value数组</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool HMSet(string key, KeyValuePair<string, string>[] fieldValues, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(HashCommands.HMSet(key, fieldValues), "OK", cancellationToken);
        }

        /// <summary>
        /// Sets the specified fields to their respective values in the hash stored at key.
        /// <para>This command overwrites the values of specified fields that exist in the hash. If key doesn't exist, a new key holding a hash is created.</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>将一个指定的字段的值存入Hash中</para>
        /// <para>此命令会覆盖Hash中存在的指定字段的值。如果 key 不存在，则创建一个包含哈希的新键。</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key<para>Hash的key</para></param>
        /// <param name="fieldValues">field-value pair array<para>field-value数组</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool HMSet(string key, KeyValuePair<string, byte[]>[] fieldValues, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(HashCommands.HMSet(key, fieldValues), "OK", cancellationToken);
        }

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定Key的Hash中的指定字段的值</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash Key</param>
        /// <param name="field">Hash field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? HGet(string key, string field, CancellationToken cancellationToken = default)
#else
        public string HGet(string key, string field, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(HashCommands.HGet(key, field), cancellationToken);
        }

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定Key的Hash中的指定字段的值</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash Key</param>
        /// <param name="field">Hash field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? HGetBytes(string key, string field, CancellationToken cancellationToken = default)
#else
        public byte[] HGetBytes(string key, string field, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(HashCommands.HGet(key, field), cancellationToken);
        }

        /// <summary>
        /// Returns the values associated with the specified fields in the hash stored at key.
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定Key的Hash中指定的多个field的值</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="fields">fields</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string?[]? HMGet(string key, string[] fields, CancellationToken cancellationToken = default)
#else
        public string[] HMGet(string key, string[] fields, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(HashCommands.HMGet(key, fields), ResultType.Array | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns the values associated with the specified fields in the hash stored at key.
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定Key的Hash中指定的多个field的值</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="fields">fields</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]?[]? HMGetBytes(string key, string[] fields, CancellationToken cancellationToken = default)
#else
        public byte[][] HMGetBytes(string key, string[] fields, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(HashCommands.HMGet(key, fields), ResultType.Array | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Removes the specified fields from the hash stored at key
        /// <para>Available since: 2.0.0</para>
        /// <para>删除指定Key的Hash中指定的field</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">del field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of fields that were removed from the hash, excluding any specified but non-existing fields.
        /// <para>成功删除的数量, 不包含不存在的field</para>
        /// </returns>
        public long HDel(string key, string field, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HDel(key, field), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Removes the specified fields from the hash stored at key
        /// <para>Available since: 2.4.0</para>
        /// <para>删除指定Key的Hash中指定的field</para>
        /// <para>支持此命令的Redis版本, 2.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="fields">del fields</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of fields that were removed from the hash, excluding any specified but non-existing fields.
        /// <para>成功删除的数量, 不包含不存在的field</para>
        /// </returns>
        public long HDel(string key, string[] fields, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HDel(key, fields), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns if field is an existing field in the hash stored at key.
        /// <para>Available since: 2.0.0</para>
        /// <para>指定Key的Hash中是否存在指定的field</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool HExists(string key, string field, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(HashCommands.HExists(key, field), "1", cancellationToken);
        }

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定Key的Hash的所有值</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Dictionary<string, string>? HGetAll(string key, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, string> HGetAll(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallDictionaryClassValue<string>(HashCommands.HGetAll(key), ResultType.Dictionary | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定Key的Hash的所有值</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Dictionary<string, byte[]>? HGetAllBytes(string key, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, byte[]> HGetAllBytes(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallDictionaryClassValue<byte[]>(HashCommands.HGetAll(key), ResultType.Dictionary | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Increments the number stored at field in the hash stored at key by increment. If key does not exist, a new key holding a hash is created. If field does not exist the value is set to 0 before the operation is performed.
        /// <para>Available since: 2.0.0</para>
        /// <para>将指定Key的Hash中指定的field进行累加. 如果指定的Key或field不存在, 则从0开始并设置</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">increment field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the value of the field after the increment operation.
        /// <para>累加之后的值</para>
        /// </returns>
        public NumberValue HIncrBy(string key, string field, CancellationToken cancellationToken = default)
        {
            return this.HIncrBy(key, field, 1, cancellationToken);
        }

        /// <summary>
        /// Increments the number stored at field in the hash stored at key by increment. If key does not exist, a new key holding a hash is created. If field does not exist the value is set to 0 before the operation is performed.
        /// <para>Available since: 2.0.0</para>
        /// <para>将指定Key的Hash中指定的field进行累加. 如果指定的Key或field不存在, 则从0开始并设置</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">increment field</param>
        /// <param name="increment">increment value<para>累加的值</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the value of the field after the increment operation.
        /// <para>累加之后的值</para>
        /// </returns>
        public NumberValue HIncrBy<TIncrement>(string key, string field, TIncrement increment, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TIncrement : struct, System.Numerics.INumber<TIncrement>
#else
            where TIncrement : struct, IEquatable<TIncrement>
#endif
        {
#if NET7_0_OR_GREATER
            if (increment == TIncrement.Zero)
#else
            if (increment.Equals(default))
#endif
            {
                throw new InvalidOperationException("The increment value is 0, which is meaningless");
            }
            Extend.CheckIntegerType(increment, "The [HINCRBY] command supports only integer value types");
            return base._call.CallNumber(HashCommands.HIncrBy(key, field, increment), cancellationToken);
        }

        /// <summary>
        /// Decrements the number stored at field in the hash stored at key by increment. If key does not exist, a new key holding a hash is created. If field does not exist the value is set to 0 before the operation is performed.
        /// <para>Available since: 2.0.0</para>
        /// <para>将指定Key的Hash中指定的field进行递减. 如果指定的Key或field不存在, 则从0开始并设置</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">decrement field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the value of the field after the decrement operation.
        /// <para>递减之后的值</para>
        /// </returns>
        public NumberValue HDecrBy(string key, string field, CancellationToken cancellationToken = default)
        {
            return this.HIncrBy(key, field, -1, cancellationToken);
        }

        /// <summary>
        /// Decrements the number stored at field in the hash stored at key by increment. If key does not exist, a new key holding a hash is created. If field does not exist the value is set to 0 before the operation is performed.
        /// <para>Available since: 2.0.0</para>
        /// <para>将指定Key的Hash中指定的field进行递减. 如果指定的Key或field不存在, 则从0开始并设置</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">decrement field</param>
        /// <param name="decrement">decrement value
        /// <para>要减少的数值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the value of the field after the decrement operation.
        /// <para>递减之后的值</para>
        /// </returns>
        public NumberValue HDecrBy<TDecrement>(string key, string field, TDecrement decrement, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TDecrement : struct, System.Numerics.INumber<TDecrement>
#else
            where TDecrement : struct, IEquatable<TDecrement>
#endif
        {
#if NET7_0_OR_GREATER
            return this.HIncrBy(key, field, -decrement, cancellationToken);
#else
            TDecrement number = Extend.GetOppositeValue(decrement);
            return this.HIncrBy(key, field, number, cancellationToken);
#endif
        }

        /// <summary>
        /// Increment the specified field of a hash stored at key, and representing a floating point number, by the specified increment.
        /// <para>Available since: 2.6.0</para>
        /// <para>将指定Key的Hash中指定的field进行递增. 如果指定的Key或field不存在, 则从0开始并设置</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">Increment field</param>
        /// <param name="increment">Increment value<para>递增的值</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the value of the field after the increment operation.
        /// <para>递增之后的值</para>
        /// </returns>
        public NumberValue HIncrByFloat<TIncrement>(string key, string field, TIncrement increment, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TIncrement : struct, System.Numerics.INumber<TIncrement>
#else
            where TIncrement : struct, IEquatable<TIncrement>
#endif
        {
#if NET7_0_OR_GREATER
            if (increment == TIncrement.Zero)
#else
            if (increment.Equals(default))
#endif
            {
                throw new InvalidOperationException("The increment value is 0, which is meaningless");
            }
            Extend.CheckNumberType(increment, "The [HINCRBYFLOAT] command supports only Floating-point value types");
            return base._call.CallNumber(HashCommands.HIncrByFloat(key, field, increment), cancellationToken);
        }

        /// <summary>
        /// Decrements the specified field of a hash stored at key, and representing a floating point number, by the specified decrement.
        /// <para>Available since: 2.6.0</para>
        /// <para>将指定Key的Hash中指定的field进行递减. 如果指定的Key或field不存在, 则从0开始并设置</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">decrement field</param>
        /// <param name="decrement">decrement value<para>递减的值</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the value of the field after the decrement operation.
        /// <para>递减之后的值</para>
        /// </returns>
        public NumberValue HDecrByFloat<TDecrement>(string key, string field, TDecrement decrement, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TDecrement : struct, System.Numerics.INumber<TDecrement>
#else
            where TDecrement : struct, IEquatable<TDecrement>
#endif
        {
#if NET7_0_OR_GREATER
            return this.HIncrByFloat(key, field, -decrement, cancellationToken);
#else
            TDecrement number = Extend.GetOppositeValue(decrement);
            return this.HIncrByFloat(key, field, number, cancellationToken);
#endif
        }

        /// <summary>
        /// Returns all field names in the hash stored at key.
        /// <para>Available since: 2.0.0</para>
        /// <para>返回指定Key的Hash中的所有field</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? HKeys(string key, CancellationToken cancellationToken = default)
#else
        public string[] HKeys(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(HashCommands.HKeys(key), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// <para>Available since: 2.0.0</para>
        /// <para>返回指定Key的Hash中field的数量</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public long HLen(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HLen(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Return a random field from the hash value stored at key.
        /// <para>Available since: 6.2.0</para>
        /// <para>返回一个随机的field</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>field</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? HRandField(string key, CancellationToken cancellationToken = default)
#else
        public string HRandField(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(HashCommands.HRandField(key, 0, false), cancellationToken);
        }

        /// <summary>
        /// Return a random field from the hash value stored at key. with value
        /// <para>Available since: 6.2.0</para>
        /// <para>返回一个随机的field, 包含值</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>field-value</returns>
        public KeyValuePair<string, string>? HRandFieldWithValues(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<KeyValuePair<string, string>>(HashCommands.HRandField(key, 1, true), ResultType.KeyValuePair | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Return a random field from the hash value stored at key. with value
        /// <para>Available since: 6.2.0</para>
        /// <para>返回一个随机的field, 包含值</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>field-value</returns>
        public KeyValuePair<string, byte[]>? HRandFieldWithValuesBytes(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(HashCommands.HRandField(key, 1, true), ResultType.KeyValuePair | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Return multiple fields randomly at key
        /// <para>Available since: 6.2.0</para>
        /// <para>返回随机多个field</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="count">
        /// When the count argument is a positive value this command behaves as follows:
        /// <para>1. No repeated fields are returned.</para>
        /// <para>2. If count is bigger than the number of fields in the hash, the command will only return the whole hash without additional fields.</para>
        /// <para>3. The order of fields in the reply is not truly random, so it is up to the client to shuffle them if needed.</para>
        /// <para>When the count is a negative value, the behavior changes as follows:</para>
        /// <para>1. Repeating fields are possible.</para>
        /// <para>2. Exactly count fields, or an empty array if the hash is empty (non-existing key), are always returned.</para>
        /// <para>3. The order of fields in the reply is truly random.</para>
        /// <para>如果count为正整数时, 返回的field不会重复, 如果count大于Hash的field总数, 则返回所有field. 返回可能不是真随机的, 如果需要随机, 可以自己打乱顺序</para>
        /// <para>如果count为负数, 返回的field可能会重复. 如果Hash不存在或者没有field, 返回空数组. 返回的field顺序是随机的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>fields</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? HRandField(string key, long count, CancellationToken cancellationToken = default)
#else
        public string[] HRandField(string key, long count, CancellationToken cancellationToken = default)
#endif
        {
            if (count == 0) throw new RedisException("count cannot be 0");
            return base._call.CallClassArray<string>(HashCommands.HRandField(key, count, false), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return multiple fields randomly at key, with value
        /// <para>Available since: 6.2.0</para>
        /// <para>返回随机多个field且包含field的值</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="count">
        /// When the count argument is a positive value this command behaves as follows:
        /// <para>1. No repeated fields are returned.</para>
        /// <para>2. If count is bigger than the number of fields in the hash, the command will only return the whole hash without additional fields.</para>
        /// <para>3. The order of fields in the reply is not truly random, so it is up to the client to shuffle them if needed.</para>
        /// <para>When the count is a negative value, the behavior changes as follows:</para>
        /// <para>1. Repeating fields are possible.</para>
        /// <para>2. Exactly count fields, or an empty array if the hash is empty (non-existing key), are always returned.</para>
        /// <para>3. The order of fields in the reply is truly random.</para>
        /// <para>如果count为正整数时, 返回的field不会重复, 如果count大于Hash的field总数, 则返回所有field. 返回可能不是真随机的, 如果需要随机, 可以自己打乱顺序</para>
        /// <para>如果count为负数, 返回的field可能会重复. 如果Hash不存在或者没有field, 返回空数组. 返回的field顺序是随机的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>field-value, Dictionary</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public KeyValuePair<string, string>[]? HRandFieldWithValues(string key, long count, CancellationToken cancellationToken = default)
#else
        public KeyValuePair<string, string>[] HRandFieldWithValues(string key, long count, CancellationToken cancellationToken = default)
#endif
        {
            if (count == 0) throw new RedisException("count cannot be 0");
            return base._call.CallStructArray<KeyValuePair<string, string>>(HashCommands.HRandField(key, count, true), ResultType.KeyValuePairArray | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return multiple fields randomly at key, with value
        /// <para>Available since: 6.2.0</para>
        /// <para>返回随机多个field且包含field的值</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="count">
        /// When the count argument is a positive value this command behaves as follows:
        /// <para>1. No repeated fields are returned.</para>
        /// <para>2. If count is bigger than the number of fields in the hash, the command will only return the whole hash without additional fields.</para>
        /// <para>3. The order of fields in the reply is not truly random, so it is up to the client to shuffle them if needed.</para>
        /// <para>When the count is a negative value, the behavior changes as follows:</para>
        /// <para>1. Repeating fields are possible.</para>
        /// <para>2. Exactly count fields, or an empty array if the hash is empty (non-existing key), are always returned.</para>
        /// <para>3. The order of fields in the reply is truly random.</para>
        /// <para>如果count为正整数时, 返回的field不会重复, 如果count大于Hash的field总数, 则返回所有field. 返回可能不是真随机的, 如果需要随机, 可以自己打乱顺序</para>
        /// <para>如果count为负数, 返回的field可能会重复. 如果Hash不存在或者没有field, 返回空数组. 返回的field顺序是随机的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>field-value, Dictionary</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public KeyValuePair<string, byte[]>[]? HRandFieldWithValuesBytes(string key, long count, CancellationToken cancellationToken = default)
#else
        public KeyValuePair<string, byte[]>[] HRandFieldWithValuesBytes(string key, long count, CancellationToken cancellationToken = default)
#endif
        {
            if (count == 0) throw new RedisException("count cannot be 0");
            return base._call.CallStructArray<KeyValuePair<string, byte[]>>(HashCommands.HRandField(key, count, true), ResultType.KeyValuePairArray | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Sets field in the hash stored at key to value, only if field does not yet exist. If key does not exist, a new key holding a hash is created. If field already exists, this operation has no effect.
        /// <para>Available since: 2.0.0</para>
        /// <para>仅在指定Key的Hash中不存在指定的field时才设置值, 否则不设置</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool HSetNx(string key, string field, string value, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(HashCommands.HSetNx(key, field, value), "1", cancellationToken);
        }

        /// <summary>
        /// Sets field in the hash stored at key to value, only if field does not yet exist. If key does not exist, a new key holding a hash is created. If field already exists, this operation has no effect.
        /// <para>Available since: 2.0.0</para>
        /// <para>仅在指定Key的Hash中不存在指定的field时才设置值, 否则不设置</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool HSetNx(string key, string field, byte[] value, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(HashCommands.HSetNx(key, field, value), "1", cancellationToken);
        }

        /// <summary>
        /// Returns the string length of the value associated with field in the hash stored at key. If the key or the field do not exist, 0 is returned.
        /// <para>Available since: 3.2.0</para>
        /// <para>返回指定Key的Hash中指定field的值长度. 如果不存在field或hash返回0</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public long HStrLen(string key, string field, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HStrLen(key, field), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns all values in the hash stored at key.
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定Key的Hash中所有的field的值, 不包含field, 只有值</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? HVals(string key, CancellationToken cancellationToken = default)
#else
        public string[] HVals(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(HashCommands.HVals(key), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns all values in the hash stored at key.
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定Key的Hash中所有的field的值, 不包含field, 只有值</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? HValsBytes(string key, CancellationToken cancellationToken = default)
#else
        public byte[][] HValsBytes(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(HashCommands.HVals(key), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one field of a given hash key
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="seconds">TTL or time to live, seconds
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="field">field</param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
        public HashFieldExpirationStatus HExpire(string key, ulong seconds, string field, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallEnum<HashFieldExpirationStatus>(HashCommands.HExpire(key, seconds, nxx, glt, field), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one field of a given hash key
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="ttl">TTL or time to live
        /// <para>有效时间</para>
        /// </param>
        /// <param name="field">field</param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
        public HashFieldExpirationStatus HExpire(string key, TimeSpan ttl, string field, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            if (ttl.TotalSeconds < 0) throw new RedisException("TLL seconds cannot be negative");
            return base._call.CallEnum<HashFieldExpirationStatus>(HashCommands.HExpire(key, (ulong)ttl.TotalSeconds, nxx, glt, field), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one or more fields of a given hash key. You must specify at least one field.
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的多个field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="seconds">TTL or time to live, seconds
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="fields">fields<para>要设置的field数组</para></param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public HashFieldExpirationStatus[]? HExpire(string key, ulong seconds, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#else
        public HashFieldExpirationStatus[] HExpire(string key, ulong seconds, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallEnumArray<HashFieldExpirationStatus>(HashCommands.HExpire(key, seconds, nxx, glt, fields), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one or more fields of a given hash key. You must specify at least one field.
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的多个field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="ttl">TTL or time to live
        /// <para>有效时间</para>
        /// </param>
        /// <param name="fields">fields<para>要设置的field数组</para></param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public HashFieldExpirationStatus[]? HExpire(string key, TimeSpan ttl, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#else
        public HashFieldExpirationStatus[] HExpire(string key, TimeSpan ttl, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#endif
        {
            if (ttl.TotalSeconds < 0) throw new RedisException("TLL seconds cannot be negative");
            return base._call.CallEnumArray<HashFieldExpirationStatus>(HashCommands.HExpire(key, (ulong)ttl.TotalSeconds, nxx, glt, fields), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one field of a given hash key
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="milliseconds">TTL or time to live, milliseconds
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="field">field</param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
        public HashFieldExpirationStatus HPExpire(string key, ulong milliseconds, string field, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallEnum<HashFieldExpirationStatus>(HashCommands.HPExpire(key, milliseconds, nxx, glt, field), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one field of a given hash key
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="ttl">TTL or time to live
        /// <para>有效时间</para>
        /// </param>
        /// <param name="field">field</param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
        public HashFieldExpirationStatus HPExpire(string key, TimeSpan ttl, string field, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            if (ttl.TotalSeconds < 0) throw new RedisException("TLL seconds cannot be negative");
            return base._call.CallEnum<HashFieldExpirationStatus>(HashCommands.HPExpire(key, (ulong)ttl.TotalMilliseconds, nxx, glt, field), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one or more fields of a given hash key. You must specify at least one field.
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的多个field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="milliseconds">TTL or time to live, milliseconds
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="fields">fields<para>要设置的field数组</para></param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public HashFieldExpirationStatus[]? HPExpire(string key, ulong milliseconds, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#else
        public HashFieldExpirationStatus[] HPExpire(string key, ulong milliseconds, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallEnumArray<HashFieldExpirationStatus>(HashCommands.HPExpire(key, milliseconds, nxx, glt, fields), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one or more fields of a given hash key. You must specify at least one field.
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的多个field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="ttl">TTL or time to live
        /// <para>有效时间</para>
        /// </param>
        /// <param name="fields">fields<para>要设置的field数组</para></param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public HashFieldExpirationStatus[]? HPExpire(string key, TimeSpan ttl, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#else
        public HashFieldExpirationStatus[] HPExpire(string key, TimeSpan ttl, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#endif
        {
            if (ttl.TotalSeconds < 0) throw new RedisException("TLL seconds cannot be negative");
            return base._call.CallEnumArray<HashFieldExpirationStatus>(HashCommands.HPExpire(key, (ulong)ttl.TotalMilliseconds, nxx, glt, fields), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one field of a given hash key
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="timeout">Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="field">field</param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
        public HashFieldExpirationStatus HPExpireAt(string key, DateTimeOffset timeout, string field, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallEnum<HashFieldExpirationStatus>(HashCommands.HPExpireAt(key, (ulong)Extend.GetUnixTimeMilliseconds(timeout), nxx, glt, field), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one field of a given hash key
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="timeout">Expiration time, unix-time-milliseconds
        /// <para>到期时间, 时间戳, 毫秒级</para>
        /// </param>
        /// <param name="field">field</param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
        public HashFieldExpirationStatus HPExpireAt(string key, ulong timeout, string field, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallEnum<HashFieldExpirationStatus>(HashCommands.HPExpireAt(key, timeout, nxx, glt, field), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one or more fields of a given hash key. You must specify at least one field.
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的多个field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="timeout">Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="fields">fields<para>要设置的field数组</para></param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public HashFieldExpirationStatus[]? HPExpireAt(string key, DateTimeOffset timeout, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#else
        public HashFieldExpirationStatus[] HPExpireAt(string key, DateTimeOffset timeout, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallEnumArray<HashFieldExpirationStatus>(HashCommands.HPExpireAt(key, (ulong)Extend.GetUnixTimeMilliseconds(timeout), nxx, glt, fields), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one or more fields of a given hash key. You must specify at least one field.
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的多个field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="timeout">Expiration time, unix-time-milliseconds
        /// <para>到期时间, 时间戳, 毫秒级</para>
        /// </param>
        /// <param name="fields">fields<para>要设置的field数组</para></param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public HashFieldExpirationStatus[]? HPExpireAt(string key, ulong timeout, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#else
        public HashFieldExpirationStatus[] HPExpireAt(string key, ulong timeout, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallEnumArray<HashFieldExpirationStatus>(HashCommands.HPExpireAt(key, timeout, nxx, glt, fields), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one field of a given hash key
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="timeout">Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="field">field</param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
        public HashFieldExpirationStatus HExpireAt(string key, DateTimeOffset timeout, string field, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallEnum<HashFieldExpirationStatus>(HashCommands.HExpireAt(key, (ulong)Extend.GetUnixTimeSeconds(timeout), nxx, glt, field), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one field of a given hash key
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="timeout">Expiration time, unix-time-seconds
        /// <para>到期时间, 时间戳, 秒级</para>
        /// </param>
        /// <param name="field">field</param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
        public HashFieldExpirationStatus HExpireAt(string key, ulong timeout, string field, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallEnum<HashFieldExpirationStatus>(HashCommands.HExpireAt(key, timeout, nxx, glt, field), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one or more fields of a given hash key. You must specify at least one field.
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的多个field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="timeout">Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="fields">fields<para>要设置的field数组</para></param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public HashFieldExpirationStatus[]? HExpireAt(string key, DateTimeOffset timeout, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#else
        public HashFieldExpirationStatus[] HExpireAt(string key, DateTimeOffset timeout, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallEnumArray<HashFieldExpirationStatus>(HashCommands.HExpireAt(key, (ulong)Extend.GetUnixTimeSeconds(timeout), nxx, glt, fields), cancellationToken);
        }

        /// <summary>
        /// Set an expiration (TTL or time to live) on one or more fields of a given hash key. You must specify at least one field.
        /// <para>Available since: 7.4.0</para>
        /// <para>给指定Key的Hash中指定的多个field设置过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="timeout">Expiration time, unix-time-seconds
        /// <para>到期时间, 时间戳, 秒级</para>
        /// </param>
        /// <param name="fields">fields<para>要设置的field数组</para></param>
        /// <param name="nxx">
        /// Nx: For each specified field, set expiration only when the field has no expiration.
        /// <para>Xx: For each specified field, set expiration only when the field has an existing expiration</para>
        /// <para>Nx: 仅当field没有过期时间时设置</para>
        /// <para>Xx: 仅当field有过期时间时设置</para>
        /// </param>
        /// <param name="glt">
        /// Gt: For each specified field, set expiration only when the new expiration is greater than current one
        /// <para>Lt: For each specified field, set expiration only when the new expiration is less than current one</para>
        /// <para>Gt: 仅当新到期时间大于当前过期时间时设置过期时间</para>
        /// <para>Lt: 仅当新到期时间小于当前过期时间时设置过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>status</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public HashFieldExpirationStatus[]? HExpireAt(string key, ulong timeout, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#else
        public HashFieldExpirationStatus[] HExpireAt(string key, ulong timeout, string[] fields, NxXx nxx = NxXx.None, GtLt glt = GtLt.None, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallEnumArray<HashFieldExpirationStatus>(HashCommands.HExpireAt(key, timeout, nxx, glt, fields), cancellationToken);
        }

        /// <summary>
        /// Returns the absolute Unix timestamp in seconds since Unix epoch at which the given key's field(s) will expire.
        /// <para>Available since: 7.4.0</para>
        /// <para>获得指定Hash中指定field的到期时间, 返回时间戳, 秒级</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>
        /// -2 if no such field exists in the provided hash key, or the provided key does not exist
        /// <para>-1 if the field exists but has no associated expiration set</para>
        /// <para>The expiration (Unix timestamp) in seconds</para>
        /// <para>Hash Key或field不存在返回-2, field没有过期时间返回-1, 其它值表示到期时间戳, 秒级</para>
        /// </returns>
        public long HExpireTime(string key, string field, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HExpireTime(key, field), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the absolute Unix timestamp in seconds since Unix epoch at which the given key's field(s) will expire.
        /// <para>Available since: 7.4.0</para>
        /// <para>获得指定Hash中指定的多个field的到期时间, 返回时间戳, 秒级</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="fields">fields</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>
        /// -2 if no such field exists in the provided hash key, or the provided key does not exist
        /// <para>-1 if the field exists but has no associated expiration set</para>
        /// <para>The expiration (Unix timestamp) in seconds</para>
        /// <para>Hash Key或field不存在返回-2, field没有过期时间返回-1, 其它值表示到期时间戳, 秒级</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public long[]? HExpireTime(string key, string[] fields, CancellationToken cancellationToken = default)
#else
        public long[] HExpireTime(string key, string[] fields, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<long>(HashCommands.HExpireTime(key, fields), ResultType.Array | ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// HPEXPIRETIME has the same semantics as HEXPIRETIME, but returns the absolute Unix expiration timestamp in milliseconds since Unix epoch instead of seconds
        /// <para>Available since: 7.4.0</para>
        /// <para>获得指定Hash中指定field的到期时间, 返回时间戳, 毫秒级</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>
        /// -2 if no such field exists in the provided hash key, or the provided key does not exist
        /// <para>-1 if the field exists but has no associated expiration set</para>
        /// <para>The expiration (Unix timestamp) in milliseconds</para>
        /// <para>Hash Key或field不存在返回-2, field没有过期时间返回-1, 其它值表示到期时间戳, 毫秒级</para>
        /// </returns>
        public long HPExpireTime(string key, string field, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HPExpireTime(key, field), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// HPEXPIRETIME has the same semantics as HEXPIRETIME, but returns the absolute Unix expiration timestamp in milliseconds since Unix epoch instead of seconds
        /// <para>Available since: 7.4.0</para>
        /// <para>获得指定Hash中指定的多个field的到期时间, 返回时间戳, 毫秒级</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="fields">fields</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>
        /// -2 if no such field exists in the provided hash key, or the provided key does not exist
        /// <para>-1 if the field exists but has no associated expiration set</para>
        /// <para>The expiration (Unix timestamp) in milliseconds</para>
        /// <para>Hash Key或field不存在返回-2, field没有过期时间返回-1, 其它值表示到期时间戳, 毫秒级</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public long[]? HPExpireTime(string key, string[] fields, CancellationToken cancellationToken = default)
#else
        public long[] HPExpireTime(string key, string[] fields, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<long>(HashCommands.HPExpireTime(key, fields), ResultType.Array | ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Remove the existing expiration on a hash key field
        /// <para>Available since: 7.4.0</para>
        /// <para>删除指定Hash中指定field的过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public HashPersistStatus HPersist(string key, string field, CancellationToken cancellationToken = default)
        {
            return base._call.CallEnum<HashPersistStatus>(HashCommands.HPersist(key, field), cancellationToken);
        }

        /// <summary>
        /// Remove the existing expiration on a hash key field
        /// <para>Available since: 7.4.0</para>
        /// <para>删除指定Hash中指定field的过期时间</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="fields">fields<para>要删除过期时间的field数组</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public HashPersistStatus[]? HPersist(string key, string[] fields, CancellationToken cancellationToken = default)
#else
        public HashPersistStatus[] HPersist(string key, string[] fields, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallEnumArray<HashPersistStatus>(HashCommands.HPersist(key, fields), cancellationToken);
        }

        /// <summary>
        /// Returns the remaining TTL (time to live) of a hash key field(s) that have a set expiration
        /// <para>Available since: 7.4.0</para>
        /// <para>获得指定Hash中指定field的剩余有效期</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>
        /// -2 if no such field exists in the provided hash key, or the provided key does not exist
        /// <para>-1 if the field exists but has no associated expiration set</para>
        /// <para>the TTL in seconds</para>
        /// <para>Hash Key或field不存在返回-2, field没有过期时间返回-1, 其它值表示剩余有效期秒数</para>
        /// </returns>
        public long HTtl(string key, string field, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HTtl(key, field), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the remaining TTL (time to live) of a hash key field(s) that have a set expiration
        /// <para>Available since: 7.4.0</para>
        /// <para>获得指定Hash中指定field的剩余有效期</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="fields">field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>
        /// -2 if no such field exists in the provided hash key, or the provided key does not exist
        /// <para>-1 if the field exists but has no associated expiration set</para>
        /// <para>the TTL in seconds</para>
        /// <para>Hash Key或field不存在返回-2, field没有过期时间返回-1, 其它值表示剩余有效期秒数</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public long[]? HTtl(string key, string[] fields, CancellationToken cancellationToken = default)
#else
        public long[] HTtl(string key, string[] fields, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<long>(HashCommands.HTtl(key, fields), ResultType.Array | ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the remaining TTL (time to live) of a hash key field(s) that have a set expiration
        /// <para>Available since: 7.4.0</para>
        /// <para>获得指定Hash中指定field的剩余有效期</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="field">field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>
        /// -2 if no such field exists in the provided hash key, or the provided key does not exist
        /// <para>-1 if the field exists but has no associated expiration set</para>
        /// <para>the TTL in milliseconds</para>
        /// <para>Hash Key或field不存在返回-2, field没有过期时间返回-1, 其它值表示剩余有效期毫秒数</para>
        /// </returns>
        public long HPTtl(string key, string field, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(HashCommands.HPTtl(key, field), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the remaining TTL (time to live) of a hash key field(s) that have a set expiration
        /// <para>Available since: 7.4.0</para>
        /// <para>获得指定Hash中指定field的剩余有效期</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="fields">field</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>
        /// -2 if no such field exists in the provided hash key, or the provided key does not exist
        /// <para>-1 if the field exists but has no associated expiration set</para>
        /// <para>the TTL in milliseconds</para>
        /// <para>Hash Key或field不存在返回-2, field没有过期时间返回-1, 其它值表示剩余有效期秒数</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public long[]? HPTtl(string key, string[] fields, CancellationToken cancellationToken = default)
#else
        public long[] HPTtl(string key, string[] fields, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<long>(HashCommands.HPTtl(key, fields), ResultType.Array | ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Iterates fields of Hash types and their associated values
        /// <para>Available since: 2.8.0</para>
        /// <para>迭代Hash的field和value</para>
        /// <para>支持此命令的Redis版本, 2.8.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cursor">cursor</param>
        /// <param name="pattern">pattern<para>匹配模式</para></param>
        /// <param name="count">count
        /// <para>要返回的个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public ScanValue<Dictionary<string, string>>? HScan(string key, long cursor, string? pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#else
        public ScanValue<Dictionary<string, string>> HScan(string key, long cursor, string pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallScan<Dictionary<string, string>>(HashCommands.HScan(key, cursor, pattern, count, false), ResultType.Scan | ResultType.Dictionary | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Iterates fields of Hash types and their associated values
        /// <para>Available since: 2.8.0</para>
        /// <para>迭代Hash的field和value</para>
        /// <para>支持此命令的Redis版本, 2.8.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cursor">cursor</param>
        /// <param name="pattern">pattern<para>匹配模式</para></param>
        /// <param name="count">count
        /// <para>要返回的个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public ScanValue<Dictionary<string, byte[]>>? HScanBytes(string key, long cursor, string? pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#else
        public ScanValue<Dictionary<string, byte[]>> HScanBytes(string key, long cursor, string pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallScan<Dictionary<string, byte[]>>(HashCommands.HScan(key, cursor, pattern, count, false), ResultType.Scan | ResultType.Dictionary | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Iterates fields of Hash types
        /// <para>Available since: 7.4.0</para>
        /// <para>迭代Hash的field</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cursor">cursor</param>
        /// <param name="novalues">The value of the field is not returned. This parameter is meaningless and always true, only to distinguish method overloading
        /// <para>不返回field的值, 只返回field. 此参数无意义, 永远为true, 只用作方法重载区分</para>
        /// </param>
        /// <param name="pattern">pattern<para>匹配模式</para></param>
        /// <param name="count">count
        /// <para>要返回的个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public ScanValue<string[]>? HScan(string key, long cursor, bool novalues, string? pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#else
        public ScanValue<string[]> HScan(string key, long cursor, bool novalues, string pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#endif
        {
            return this.HScanNovalues(key, cursor, pattern, count, cancellationToken);
        }

        /// <summary>
        /// Iterates fields of Hash types
        /// <para>Available since: 7.4.0</para>
        /// <para>迭代Hash的field</para>
        /// <para>支持此命令的Redis版本, 7.4.0+</para>
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="cursor">cursor</param>
        /// <param name="pattern">pattern<para>匹配模式</para></param>
        /// <param name="count">count
        /// <para>要返回的个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public ScanValue<string[]>? HScanNovalues(string key, long cursor, string? pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#else
        public ScanValue<string[]> HScanNovalues(string key, long cursor, string pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallScan<string[]>(HashCommands.HScan(key, cursor, pattern, count, true), ResultType.Scan | ResultType.Array | ResultType.String, cancellationToken);
        }
    }
}
