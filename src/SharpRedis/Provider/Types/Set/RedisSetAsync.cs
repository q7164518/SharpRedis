#if !LOW_NET
using SharpRedis.Commands;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis
{
    public sealed partial class RedisSet
    {
        /// <summary>
        /// Add the specified members to the set stored at key. Specified members that are already a member of this set are ignored. If key does not exist, a new set is created before adding the specified members.
        /// <para>Available since: 1.0.0</para>
        /// <para>添加一个或多个元素到指定Key的Set结构中.</para>
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
        public Task<long> SAddAsync(string key, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(SetCommands.SAdd(key, member), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Add the specified members to the set stored at key. Specified members that are already a member of this set are ignored. If key does not exist, a new set is created before adding the specified members.
        /// <para>Available since: 1.0.0</para>
        /// <para>添加一个或多个元素到指定Key的Set结构中.</para>
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
        public Task<long> SAddAsync(string key, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(SetCommands.SAdd<byte[]>(key, member), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Add the specified members to the set stored at key. Specified members that are already a member of this set are ignored. If key does not exist, a new set is created before adding the specified members.
        /// <para>Available since: 1.0.0 | 2.4.0</para>
        /// <para>添加一个或多个元素到指定Key的Set结构中.</para>
        /// <para>支持此命令的Redis版本, 1.0.0+ | 2.4.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements that were added to the set, not including all the elements already present in the set.
        /// <para>成功添加到集合中的元素. 不包含已存在的</para>
        /// </returns>
        public Task<long> SAddAsync(string key, string[] members, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(SetCommands.SAdd(key, members), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Add the specified members to the set stored at key. Specified members that are already a member of this set are ignored. If key does not exist, a new set is created before adding the specified members.
        /// <para>Available since: 1.0.0 | 2.4.0</para>
        /// <para>添加一个或多个元素到指定Key的Set结构中.</para>
        /// <para>支持此命令的Redis版本, 1.0.0+ | 2.4.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements that were added to the set, not including all the elements already present in the set.
        /// <para>成功添加到集合中的元素. 不包含已存在的</para>
        /// </returns>
        public Task<long> SAddAsync(string key, byte[][] members, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(SetCommands.SAdd(key, members), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the set cardinality (number of elements) of the set stored at key.
        /// <para>Available since: 1.0.0</para>
        /// <para>获得指定Key的Set中元素个数</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The cardinality (number of elements) of the set, or 0 if the key does not exist.
        /// <para>Set集合的元素个数, Key不存在返回0</para>
        /// </returns>
        public Task<long> SCardAsync(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(SetCommands.SCard(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the members of the set resulting from the difference between the first set and all the successive sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中差值集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="secondKey">second key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SDiffAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SDiffAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<string>(SetCommands.SDiff([key, secondKey]), ResultType.Array | ResultType.String, cancellationToken)!;
#else
            return base._call.CallClassArrayAsync<string>(SetCommands.SDiff(new string[] { key, secondKey }), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the difference between the first set and all the successive sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中差值集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SDiffAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SDiffAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<string>(SetCommands.SDiff([key, .. keys]), ResultType.Array | ResultType.String, cancellationToken)!;
#else
            var listKeys = new List<string>(keys.Length + 1) { key };
            listKeys.AddRange(keys);
            return base._call.CallClassArrayAsync<string>(SetCommands.SDiff(listKeys.ToArray()), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the difference between the first set and all the successive sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中差值集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SDiffAsync(string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SDiffAsync(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
            return base._call.CallClassArrayAsync<string>(SetCommands.SDiff(keys), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns the members of the set resulting from the difference between the first set and all the successive sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中差值集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="secondKey">second key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SDiffBytesAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SDiffBytesAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SDiff([key, secondKey]), ResultType.Array | ResultType.Bytes, cancellationToken)!;
#else
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SDiff(new string[] { key, secondKey }), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the difference between the first set and all the successive sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中差值集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SDiffBytesAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SDiffBytesAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SDiff([key, .. keys]), ResultType.Array | ResultType.Bytes, cancellationToken)!;
#else
            var listKeys = new List<string>(keys.Length + 1) { key };
            listKeys.AddRange(keys);
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SDiff(listKeys.ToArray()), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the difference between the first set and all the successive sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中差值集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SDiffBytesAsync(string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SDiffBytesAsync(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SDiff(keys), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is equal to SDIFF, but instead of returning the resulting set, it is stored in destination.
        /// <para>Available since: 1.0.0</para>
        /// <para>和SDiff一样, 不过不返回差值结果, 将结果存入destination指定的key中</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="destination">destination key
        /// <para>存入差值结果的Key</para>
        /// </param>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting set.
        /// <para>插入到destination key中的元素数量</para>
        /// </returns>
        public Task<long> SDiffStoreAsync(string destination, string key, string[] keys, CancellationToken cancellationToken = default)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
#if NET8_0_OR_GREATER
            return base._call.CallNumberAsync<long>(SetCommands.SDiffStore(destination, [key, .. keys]), ResultType.Int64, cancellationToken);
#else
            var listKeys = new List<string>(keys.Length + 1) { key };
            listKeys.AddRange(keys);
            return base._call.CallNumberAsync<long>(SetCommands.SDiffStore(destination, listKeys.ToArray()), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// This command is equal to SDIFF, but instead of returning the resulting set, it is stored in destination.
        /// <para>Available since: 1.0.0</para>
        /// <para>和SDiff一样, 不过不返回差值结果, 将结果存入destination指定的key中</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="destination">destination key
        /// <para>存入差值结果的Key</para>
        /// </param>
        /// <param name="key">key</param>
        /// <param name="secondKey">second key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting set.
        /// <para>插入到destination key中的元素数量</para>
        /// </returns>
        public Task<long> SDiffStoreAsync(string destination, string key, string secondKey, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumberAsync<long>(SetCommands.SDiffStore(destination, [key, secondKey]), ResultType.Int64, cancellationToken);
#else
            return base._call.CallNumberAsync<long>(SetCommands.SDiffStore(destination, new string[] { key, secondKey }), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// This command is equal to SDIFF, but instead of returning the resulting set, it is stored in destination.
        /// <para>Available since: 1.0.0</para>
        /// <para>和SDiff一样, 不过不返回差值结果, 将结果存入destination指定的key中</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="destination">destination key
        /// <para>存入差值结果的Key</para>
        /// </param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting set.
        /// <para>插入到destination key中的元素数量</para>
        /// </returns>
        public Task<long> SDiffStoreAsync(string destination, string[] keys, CancellationToken cancellationToken = default)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
            return base._call.CallNumberAsync<long>(SetCommands.SDiffStore(destination, keys), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the members of the set resulting from the intersection of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中交集集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>a list with the members of the resulting set.
        /// <para>交集数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SInterAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SInterAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<string>(SetCommands.SInter([key, .. keys]), ResultType.Array | ResultType.String, cancellationToken)!;
#else
            var listKeys = new List<string>(keys.Length + 1) { key };
            listKeys.AddRange(keys);
            return base._call.CallClassArrayAsync<string>(SetCommands.SInter(listKeys.ToArray()), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the intersection of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中交集集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="secondKey">second key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>a list with the members of the resulting set.
        /// <para>交集数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SInterAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SInterAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<string>(SetCommands.SInter([key, secondKey]), ResultType.Array | ResultType.String, cancellationToken)!;
#else
            return base._call.CallClassArrayAsync<string>(SetCommands.SInter(new string[] { key, secondKey }), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the intersection of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中交集集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>a list with the members of the resulting set.
        /// <para>交集数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SInterAsync(string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SInterAsync(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
            return base._call.CallClassArrayAsync<string>(SetCommands.SInter(keys), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns the members of the set resulting from the intersection of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中交集集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>a list with the members of the resulting set.
        /// <para>交集数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SInterBytesAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SInterBytesAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SInter([key, .. keys]), ResultType.Array | ResultType.Bytes, cancellationToken)!;
#else
            var listKeys = new List<string>(keys.Length + 1) { key };
            listKeys.AddRange(keys);
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SInter(listKeys.ToArray()), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the intersection of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中交集集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="secondKey">second key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>a list with the members of the resulting set.
        /// <para>交集数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SInterBytesAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SInterBytesAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SInter([key, secondKey]), ResultType.Array | ResultType.Bytes, cancellationToken)!;
#else
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SInter(new string[] { key, secondKey }), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the intersection of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回与第一个Key Set中交集集合</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>a list with the members of the resulting set.
        /// <para>交集数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SInterBytesAsync(string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SInterBytesAsync(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SInter(keys), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is similar to SINTER, but instead of returning the result set, it returns just the cardinality of the result. Returns the cardinality of the set which would result from the intersection of all the given sets.
        /// <para>Available since: 7.0.0</para>
        /// <para>与SInter类似, 不过不返回交集结果, 只返回交集个数</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="limit">By default, the command calculates the cardinality of the intersection of all given sets
        /// <para>默认为0, 表示计算所有交集数. 可以设定对应的次数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting intersection.
        /// <para>交集个数</para>
        /// </returns>
        public Task<long> SInterCardAsync(string key, string[] keys, ulong limit = 0, CancellationToken cancellationToken = default)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
#if NET8_0_OR_GREATER
            return base._call.CallNumberAsync<long>(SetCommands.SInterCard([key, .. keys], limit), ResultType.Int64, cancellationToken);
#else
            var listKeys = new List<string>(keys.Length + 1) { key };
            listKeys.AddRange(keys);
            return base._call.CallNumberAsync<long>(SetCommands.SInterCard(listKeys.ToArray(), limit), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// This command is similar to SINTER, but instead of returning the result set, it returns just the cardinality of the result. Returns the cardinality of the set which would result from the intersection of all the given sets.
        /// <para>Available since: 7.0.0</para>
        /// <para>与SInter类似, 不过不返回交集结果, 只返回交集个数</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="secondKey">second key</param>
        /// <param name="limit">By default, the command calculates the cardinality of the intersection of all given sets
        /// <para>默认为0, 表示计算所有交集数. 可以设定对应的次数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting intersection.
        /// <para>交集个数</para>
        /// </returns>
        public Task<long> SInterCardAsync(string key, string secondKey, ulong limit = 0, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumberAsync<long>(SetCommands.SInterCard([key, secondKey], limit), ResultType.Int64, cancellationToken);
#else
            return base._call.CallNumberAsync<long>(SetCommands.SInterCard(new string[] { key, secondKey }, limit), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// This command is similar to SINTER, but instead of returning the result set, it returns just the cardinality of the result. Returns the cardinality of the set which would result from the intersection of all the given sets.
        /// <para>Available since: 7.0.0</para>
        /// <para>与SInter类似, 不过不返回交集结果, 只返回交集个数</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="limit">By default, the command calculates the cardinality of the intersection of all given sets
        /// <para>默认为0, 表示计算所有交集数. 可以设定对应的次数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting intersection.
        /// <para>交集个数</para>
        /// </returns>
        public Task<long> SInterCardAsync(string[] keys, ulong limit = 0, CancellationToken cancellationToken = default)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
            return base._call.CallNumberAsync<long>(SetCommands.SInterCard(keys, limit), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is equal to SINTER, but instead of returning the resulting set, it is stored in destination.
        /// <para>Available since: 1.0.0</para>
        /// <para>与SINTER类似, 不过不返回交集结果, 只返回交集元素个数, 并将结果存入destination key中</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="destination">destination key</param>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting set.
        /// <para>交集元素个数</para>
        /// </returns>
        public Task<long> SInterStoreAsync(string destination, string key, string[] keys, CancellationToken cancellationToken = default)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
#if NET8_0_OR_GREATER
            return base._call.CallNumberAsync<long>(SetCommands.SInterStore(destination, [key, .. keys]), ResultType.Int64, cancellationToken);
#else
            var listKeys = new List<string>(keys.Length + 1) { key };
            listKeys.AddRange(keys);
            return base._call.CallNumberAsync<long>(SetCommands.SInterStore(destination, listKeys.ToArray()), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// This command is equal to SINTER, but instead of returning the resulting set, it is stored in destination.
        /// <para>Available since: 1.0.0</para>
        /// <para>与SINTER类似, 不过不返回交集结果, 只返回交集元素个数, 并将结果存入destination key中</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="destination">destination key</param>
        /// <param name="key">key</param>
        /// <param name="secondKey">second key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting set.
        /// <para>交集元素个数</para>
        /// </returns>
        public Task<long> SInterStoreAsync(string destination, string key, string secondKey, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumberAsync<long>(SetCommands.SInterStore(destination, [key, secondKey]), ResultType.Int64, cancellationToken);
#else
            return base._call.CallNumberAsync<long>(SetCommands.SInterStore(destination, new string[] { key, secondKey }), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// This command is equal to SINTER, but instead of returning the resulting set, it is stored in destination.
        /// <para>Available since: 1.0.0</para>
        /// <para>与SINTER类似, 不过不返回交集结果, 只返回交集元素个数, 并将结果存入destination key中</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="destination">destination key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting set.
        /// <para>交集元素个数</para>
        /// </returns>
        public Task<long> SInterStoreAsync(string destination, string[] keys, CancellationToken cancellationToken = default)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
            return base._call.CallNumberAsync<long>(SetCommands.SInterStore(destination, keys), ResultType.Int64, cancellationToken);
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
        public Task<bool> SIsMemberAsync(string key, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(SetCommands.SIsMember(key, member), "1", cancellationToken);
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
        public Task<bool> SIsMemberAsync(string key, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(SetCommands.SIsMember(key, member), "1", cancellationToken);
        }

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回指定Set中的所有元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>all members of the set.
        /// <para>所有元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SMembersAsync(string key, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SMembersAsync(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArrayAsync<string>(SetCommands.SMembers(key), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// <para>Available since: 1.0.0</para>
        /// <para>返回指定Set中的所有元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>all members of the set.
        /// <para>所有元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SMembersBytesAsync(string key, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SMembersBytesAsync(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SMembers(key), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns whether each member is a member of the set stored at key.
        /// <para>Available since: 6.2.0</para>
        /// <para>判断多个元素是否存在在指定Key的Set中</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<bool[]?> SMIsMemberAsync(string key, string[] members, CancellationToken cancellationToken = default)
#else
        public Task<bool[]> SMIsMemberAsync(string key, string[] members, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallConditionArrayAsync(SetCommands.SMIsMember(key, members), "1", cancellationToken);
        }

        /// <summary>
        /// Returns whether each member is a member of the set stored at key.
        /// <para>Available since: 6.2.0</para>
        /// <para>判断多个元素是否存在在指定Key的Set中</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<bool[]?> SMIsMemberAsync(string key, byte[][] members, CancellationToken cancellationToken = default)
#else
        public Task<bool[]> SMIsMemberAsync(string key, byte[][] members, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallConditionArrayAsync(SetCommands.SMIsMember(key, members), "1", cancellationToken);
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
        public Task<bool> SMoveAsync(string source, string destination, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(SetCommands.SMove(source, destination, member), "1", cancellationToken);
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
        public Task<bool> SMoveAsync(string source, string destination, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(SetCommands.SMove(source, destination, member), "1", cancellationToken);
        }

        /// <summary>
        /// Removes and returns one random members from the set value store at key.
        /// <para>Available since: 1.0.0</para>
        /// <para>从指定Key的Set集合中随机删除并返回一个元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string?> SPopAsync(string key, CancellationToken cancellationToken = default)
#else
        public Task<string> SPopAsync(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStringAsync(SetCommands.SPop(key, 1), cancellationToken);
        }

        /// <summary>
        /// Removes and returns one or more random members from the set value store at key.
        /// <para>Available since: 3.2.0</para>
        /// <para>从指定Key的Set集合中随机删除并返回一个或多个元素</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="count">count
        /// <para>要删除并返回的元素个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SPopAsync(string key, ulong count, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SPopAsync(string key, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArrayAsync<string>(SetCommands.SPop(key, count), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Removes and returns one random members from the set value store at key.
        /// <para>Available since: 1.0.0</para>
        /// <para>从指定Key的Set集合中随机删除并返回一个元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[]?> SPopBytesAsync(string key, CancellationToken cancellationToken = default)
#else
        public Task<byte[]> SPopBytesAsync(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytesAsync(SetCommands.SPop(key, 1), cancellationToken);
        }

        /// <summary>
        /// Removes and returns one or more random members from the set value store at key.
        /// <para>Available since: 3.2.0</para>
        /// <para>从指定Key的Set集合中随机删除并返回一个或多个元素</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="count">count
        /// <para>要删除并返回的元素个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SPopBytesAsync(string key, ulong count, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SPopBytesAsync(string key, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SPop(key, count), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// return a random element from the set value stored at key.
        /// <para>Available since: 1.0.0</para>
        /// <para>从指定Key的Set集合中随机获得一个元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string?> SRandMemberAsync(string key, CancellationToken cancellationToken = default)
#else
        public Task<string> SRandMemberAsync(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStringAsync(SetCommands.SRandMember(key, 0), cancellationToken);
        }

        /// <summary>
        /// return one or more random element from the set value stored at key.
        /// <para>Available since: 2.6.0</para>
        /// <para>从指定Key的Set集合中随机获得一个或多个元素</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="count">When the number is positive, the returned elements are not repeated, and if count is greater than the total, the entire Set is returned
        /// <para>When negative, the returned element may be duplicate</para>
        /// <para>为正数, 返回的元素不会重复, 且如果count大于Set里面的总数, 则将返回整个Set</para>
        /// <para>为负数, 返回的元素可能会存在重复</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SRandMemberAsync(string key, long count, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SRandMemberAsync(string key, long count, CancellationToken cancellationToken = default)
#endif
        {
            if (count == 0) throw new RedisException("Count cannot be 0");
            return base._call.CallClassArrayAsync<string>(SetCommands.SRandMember(key, count), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// return a random element from the set value stored at key.
        /// <para>Available since: 1.0.0</para>
        /// <para>从指定Key的Set集合中随机获得一个元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[]?> SRandMemberBytesAsync(string key, CancellationToken cancellationToken = default)
#else
        public Task<byte[]> SRandMemberBytesAsync(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytesAsync(SetCommands.SRandMember(key, 0), cancellationToken);
        }

        /// <summary>
        /// return one or more random element from the set value stored at key.
        /// <para>Available since: 2.6.0</para>
        /// <para>从指定Key的Set集合中随机获得一个或多个元素</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="count">When the number is positive, the returned elements are not repeated, and if count is greater than the total, the entire Set is returned
        /// <para>When negative, the returned element may be duplicate</para>
        /// <para>为正数, 返回的元素不会重复, 且如果count大于Set里面的总数, 则将返回整个Set</para>
        /// <para>为负数, 返回的元素可能会存在重复</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SRandMemberBytesAsync(string key, long count, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SRandMemberBytesAsync(string key, long count, CancellationToken cancellationToken = default)
#endif
        {
            if (count == 0) throw new RedisException("Count cannot be 0");
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SRandMember(key, count), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
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
        public Task<long> SRemAsync(string key, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(SetCommands.SRem(key, member), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Remove the specified members from the set stored at key. Specified members that are not a member of this set are ignored. If key does not exist, it is treated as an empty set and this command returns 0.
        /// <para>Available since: 2.4.0</para>
        /// <para>从指定Set集合中删除指定的元素</para>
        /// <para>支持此命令的Redis版本, 2.4.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of members that were removed from the set, not including non existing members.
        /// <para>成功移除的元素数量. 不存在在Set集合中的元素不计数</para>
        /// </returns>
        public Task<long> SRemAsync(string key, string[] members, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(SetCommands.SRem(key, members), ResultType.Int64, cancellationToken);
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
        public Task<long> SRemAsync(string key, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(SetCommands.SRem<byte[]>(key, member), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Remove the specified members from the set stored at key. Specified members that are not a member of this set are ignored. If key does not exist, it is treated as an empty set and this command returns 0.
        /// <para>Available since: 2.4.0</para>
        /// <para>从指定Set集合中删除指定的元素</para>
        /// <para>支持此命令的Redis版本, 2.4.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of members that were removed from the set, not including non existing members.
        /// <para>成功移除的元素数量. 不存在在Set集合中的元素不计数</para>
        /// </returns>
        public Task<long> SRemAsync(string key, byte[][] members, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(SetCommands.SRem(key, members), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the members of the set resulting from the union of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>获得指定Key的Set集合的并集</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SUnionAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SUnionAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<string>(SetCommands.SUnion([key, .. keys]), ResultType.Array | ResultType.String, cancellationToken)!;
#else
            var listKeys = new List<string>(keys.Length + 1) { key };
            listKeys.AddRange(keys);
            return base._call.CallClassArrayAsync<string>(SetCommands.SUnion(listKeys.ToArray()), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the union of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>获得指定Key的Set集合的并集</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="secondKey">second key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SUnionAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SUnionAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<string>(SetCommands.SUnion([key, secondKey]), ResultType.Array | ResultType.String, cancellationToken)!;
#else
            return base._call.CallClassArrayAsync<string>(SetCommands.SUnion(new string[] { key, secondKey }), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the union of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>获得指定Key的Set集合的并集</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string[]?> SUnionAsync(string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<string[]> SUnionAsync(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
            return base._call.CallClassArrayAsync<string>(SetCommands.SUnion(keys), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns the members of the set resulting from the union of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>获得指定Key的Set集合的并集</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SUnionBytesAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SUnionBytesAsync(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SUnion([key, .. keys]), ResultType.Array | ResultType.Bytes, cancellationToken)!;
#else
            var listKeys = new List<string>(keys.Length + 1) { key };
            listKeys.AddRange(keys);
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SUnion(listKeys.ToArray()), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the union of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>获得指定Key的Set集合的并集</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="secondKey">second key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SUnionBytesAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SUnionBytesAsync(string key, string secondKey, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SUnion([key, secondKey]), ResultType.Array | ResultType.Bytes, cancellationToken)!;
#else
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SUnion(new string[] { key, secondKey }), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
#endif
        }

        /// <summary>
        /// Returns the members of the set resulting from the union of all the given sets.
        /// <para>Available since: 1.0.0</para>
        /// <para>获得指定Key的Set集合的并集</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<byte[][]?> SUnionBytesAsync(string[] keys, CancellationToken cancellationToken = default)
#else
        public Task<byte[][]> SUnionBytesAsync(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
            return base._call.CallClassArrayAsync<byte[]>(SetCommands.SUnion(keys), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is equal to SUNION, but instead of returning the resulting set, it is stored in destination.
        /// <para>Available since: 1.0.0</para>
        /// <para>和[SUNION]一样, 不过不返回并集结果, 只返回并集元素数量, 并将结果设置到destination中</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="destination">destination key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting set.
        /// <para>并集元素个数</para>
        /// </returns>
        public Task<long> SUnionStoreAsync(string destination, string key, string[] keys, CancellationToken cancellationToken = default)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
#if NET8_0_OR_GREATER
            return base._call.CallNumberAsync<long>(SetCommands.SUnionStore(destination, [key, .. keys]), ResultType.Int64, cancellationToken);
#else
            var listKeys = new List<string>(keys.Length + 1) { key };
            listKeys.AddRange(keys);
            return base._call.CallNumberAsync<long>(SetCommands.SUnionStore(destination, listKeys.ToArray()), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// This command is equal to SUNION, but instead of returning the resulting set, it is stored in destination.
        /// <para>Available since: 1.0.0</para>
        /// <para>和[SUNION]一样, 不过不返回并集结果, 只返回并集元素数量, 并将结果设置到destination中</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="destination">destination key</param>
        /// <param name="secondKey">second key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting set.
        /// <para>并集元素个数</para>
        /// </returns>
        public Task<long> SUnionStoreAsync(string destination, string key, string secondKey, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumberAsync<long>(SetCommands.SUnionStore(destination, [key, secondKey]), ResultType.Int64, cancellationToken);
#else
            return base._call.CallNumberAsync<long>(SetCommands.SUnionStore(destination, new string[] { key, secondKey }), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// This command is equal to SUNION, but instead of returning the resulting set, it is stored in destination.
        /// <para>Available since: 1.0.0</para>
        /// <para>和[SUNION]一样, 不过不返回并集结果, 只返回并集元素数量, 并将结果设置到destination中</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="destination">destination key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of elements in the resulting set.
        /// <para>并集元素个数</para>
        /// </returns>
        public Task<long> SUnionStoreAsync(string destination, string[] keys, CancellationToken cancellationToken = default)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("keys cannot be empty");
            return base._call.CallNumberAsync<long>(SetCommands.SUnionStore(destination, keys), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Iterates set
        /// <para>Available since: 2.8.0</para>
        /// <para>迭代Set</para>
        /// <para>支持此命令的Redis版本, 2.8.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="cursor">cursor</param>
        /// <param name="pattern">pattern</param>
        /// <param name="count">count</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para></param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<ScanValue<string[]>?> SScanAsync(string key, long cursor, string? pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#else
        public Task<ScanValue<string[]>> SScanAsync(string key, long cursor, string pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallScanAsync<string[]>(SetCommands.SScan(key, cursor, pattern, count), ResultType.Scan | ResultType.Array | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Iterates set
        /// <para>Available since: 2.8.0</para>
        /// <para>迭代Set</para>
        /// <para>支持此命令的Redis版本, 2.8.0+</para>
        /// </summary>
        /// <param name="key">Set key</param>
        /// <param name="cursor">cursor</param>
        /// <param name="pattern">pattern</param>
        /// <param name="count">count</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para></param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<ScanValue<byte[][]>?> SScanBytesAsync(string key, long cursor, string? pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#else
        public Task<ScanValue<byte[][]>> SScanBytesAsync(string key, long cursor, string pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallScanAsync<byte[][]>(SetCommands.SScan(key, cursor, pattern, count), ResultType.Scan | ResultType.Array | ResultType.Bytes, cancellationToken);
        }
    }
}
#endif
