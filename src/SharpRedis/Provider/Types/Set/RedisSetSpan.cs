#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using SharpRedis.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis
{
    public sealed partial class RedisSet
    {
        /// <summary>
        /// Add the specified members to the set stored at key. Specified members that are already a member of this set are ignored. If key does not exist, a new set is created before adding the specified members.
        /// <para>Available since: 1.0.0</para>
        /// <para>添加一个元素到指定Key的Set结构中.</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements that were added to the set, not including all the elements already present in the set.
        /// <para>成功添加到集合中的元素. 不包含已存在的</para>
        /// </returns>
        public long SAdd(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.SAdd(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Add the specified members to the set stored at key. Specified members that are already a member of this set are ignored. If key does not exist, a new set is created before adding the specified members.
        /// <para>Available since: 1.0.0</para>
        /// <para>添加一个元素到指定Key的Set结构中.</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements that were added to the set, not including all the elements already present in the set.
        /// <para>成功添加到集合中的元素. 不包含已存在的</para>
        /// </returns>
        public Task<long> SAddAsync(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.SAddAsync(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Returns if member is a member of the set stored at key.
        /// <para>Available since: 1.0.0</para>
        /// <para>判断元素是否存在在指定Key的Set中</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SIsMember(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.SIsMember(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Returns if member is a member of the set stored at key.
        /// <para>Available since: 1.0.0</para>
        /// <para>判断元素是否存在在指定Key的Set中</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SIsMemberAsync(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.SIsMemberAsync(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Move member from the set at source to the set at destination. This operation is atomic. In every given moment the element will appear to be a member of source or destination for other clients.
        /// <para>If the source set does not exist or does not contain the specified element, no operation is performed and 0 is returned. Otherwise, the element is removed from the source set and added to the destination set. When the specified element already exists in the destination set, it is only removed from the source set.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>将指定的元素从source中移动到destination中</para>
        /// <para>如果destination已经存在指定的元素, 则只会在source中进行移除</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="source">source key</param>
        /// <param name="destination">destination key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SMove(string source, string destination, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.SMove(source, destination, member.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Move member from the set at source to the set at destination. This operation is atomic. In every given moment the element will appear to be a member of source or destination for other clients.
        /// <para>If the source set does not exist or does not contain the specified element, no operation is performed and 0 is returned. Otherwise, the element is removed from the source set and added to the destination set. When the specified element already exists in the destination set, it is only removed from the source set.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>将指定的元素从source中移动到destination中</para>
        /// <para>如果destination已经存在指定的元素, 则只会在source中进行移除</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="source">source key</param>
        /// <param name="destination">destination key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SMoveAsync(string source, string destination, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.SMoveAsync(source, destination, member.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Remove the specified members from the set stored at key. Specified members that are not a member of this set are ignored. If key does not exist, it is treated as an empty set and this command returns 0.
        /// <para>Available since: 1.0.0</para>
        /// <para>从指定Set集合中删除指定的元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of members that were removed from the set, not including non existing members.
        /// <para>成功移除的元素数量. 不存在在Set集合中的元素不计数</para>
        /// </returns>
        public long SRem(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.SRem(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Remove the specified members from the set stored at key. Specified members that are not a member of this set are ignored. If key does not exist, it is treated as an empty set and this command returns 0.
        /// <para>Available since: 1.0.0</para>
        /// <para>从指定Set集合中删除指定的元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of members that were removed from the set, not including non existing members.
        /// <para>成功移除的元素数量. 不存在在Set集合中的元素不计数</para>
        /// </returns>
        public Task<long> SRemAsync(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.SRemAsync(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
        }
    }
}
#endif
