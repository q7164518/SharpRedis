#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using SharpRedis.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis
{
    public sealed partial class RedisHash
    {
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
        public long HSet(string key, string field, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.HSet(key, field, value.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<long> HSetAsync(string key, string field, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.HSetAsync(key, field, value.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public bool HSetNx(string key, string field, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.HSetNx(key, field, value.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<bool> HSetNxAsync(string key, string field, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.HSetNxAsync(key, field, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }
    }
}
#endif
