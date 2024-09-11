#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable IDE0063
#endif
#if !LOW_NET
using System.Threading;
#else
#pragma warning disable IDE0130
#endif
using System.Collections.Generic;
using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using System;

namespace SharpRedis
{
    /// <summary>
    /// Sorted Set
    /// <para>有序不重复集合</para>
    /// </summary>
    public sealed partial class RedisSortedSet : BaseType
    {
        internal RedisSortedSet(BaseCall call) : base(call)
        {
        }

        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key.
        /// <para>Available since: 1.2.0 | 3.0.2 | 6.2.0</para>
        /// <para>往指定的SortedSet中插入一个带有排序分数的元素</para>
        /// <para>支持此命令的Redis版本, 1.2.0+ | 3.0.2+ | 6.2.0</para>
        /// </summary>
        /// <typeparam name="TScore">Score</typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="member">member<para>元素值</para></param>
        /// <param name="score">score<para>排序分数</para></param>
        /// <param name="ch">
        /// Available since: 3.0.2. Whether to change the return value to the number of affected members. By default, only the number of new members is counted.
        /// <para>If the elements and scores are the same, they are not counted</para>
        /// <para>支持此参数的Redis版本, 3.0.2+. 是否返回受影响的元素个数. 默认情况只统计新增元素数量. 如果元素和分数都一样, 也不计入统计</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 3.0.2. Nx: Only add new elements. Don't update already existing elements.
        /// <para>Xx: Only update elements that already exist. Don't add new elements.</para>
        /// <para>支持此参数的Redis版本, 3.0.2+. Nx: 仅添加新元素, 不更新已经存在的元素</para>
        /// <para>Xx: 只更新已经存在的元素, 不添加新元素</para>
        /// </param>
        /// <param name="gl">
        /// Available since: 6.2.0. Lt: Only update existing elements if the new score is less than the current score. This flag doesn't prevent adding new elements.
        /// <para>Gt: Only update existing elements if the new score is greater than the current score. This flag doesn't prevent adding new elements.</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. Lt: 如果新分数小于当前分数，则仅更新现有元素. 该标志不会阻止添加新元素</para>
        /// <para>Gt: 仅当新分数大于当前分数时才更新现有元素. 该标志不会阻止添加新元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of new members when the CH option is not used.
        /// <para>The number of new or updated members when the CH option is used.</para>
        /// <para>ch为false新增元素数量. ch为true新增和更新的元素数量</para>
        /// </returns>
        public long ZAdd<TScore>(string key, string member, TScore score, bool ch = false, NxXx nxx = NxXx.None, GtLt gl = GtLt.None, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZAdd(key, member, score, nxx: nxx, ch: ch, gl: gl), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// INCR: When this option is specified ZADD acts like ZINCRBY. Only one score-element pair can be specified in this mode.
        /// <para>Available since: 3.0.2</para>
        /// <para>和ZINCRBY命令类似</para>
        /// <para>支持此命令的Redis版本, 3.0.2+</para>
        /// </summary>
        /// <typeparam name="TScore">Score</typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="member">member<para>元素值</para></param>
        /// <param name="score">score<para>排序分数</para></param>
        /// <param name="nxx">
        /// Nx: Only add new elements. Don't update already existing elements.
        /// <para>Xx: Only update elements that already exist. Don't add new elements.</para>
        /// <para>Nx: 仅添加新元素, 不更新已经存在的元素</para>
        /// <para>Xx: 只更新已经存在的元素, 不添加新元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the updated score of the member when the INCR option is used
        /// <para>元素增加后的排序分</para>
        /// </returns>
        public NumberValue ZAddIncr<TScore>(string key, string member, TScore score, NxXx nxx = NxXx.None, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallNumber(SortedSetCommands.ZAdd(key, member, score, nxx: nxx, incr: true), cancellationToken);
        }

        /// <summary>
        /// INCR: When this option is specified ZADD acts like ZINCRBY. Only one score-element pair can be specified in this mode.
        /// <para>Available since: 3.0.2</para>
        /// <para>和ZINCRBY命令类似</para>
        /// <para>支持此命令的Redis版本, 3.0.2+</para>
        /// </summary>
        /// <typeparam name="TScore">Score</typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="member">member<para>元素值</para></param>
        /// <param name="score">score<para>排序分数</para></param>
        /// <param name="nxx">
        /// Nx: Only add new elements. Don't update already existing elements.
        /// <para>Xx: Only update elements that already exist. Don't add new elements.</para>
        /// <para>Nx: 仅添加新元素, 不更新已经存在的元素</para>
        /// <para>Xx: 只更新已经存在的元素, 不添加新元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the updated score of the member when the INCR option is used
        /// <para>元素增加后的排序分</para>
        /// </returns>
        public NumberValue ZAddIncr<TScore>(string key, byte[] member, TScore score, NxXx nxx = NxXx.None, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallNumber(SortedSetCommands.ZAdd(key, member, score, nxx: nxx, incr: true), cancellationToken);
        }

        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key.
        /// <para>Available since: 1.2.0 | 3.0.2 | 6.2.0</para>
        /// <para>往指定的SortedSet中插入一个带有排序分数的元素</para>
        /// <para>支持此命令的Redis版本, 1.2.0+ | 3.0.2 | 6.2.0</para>
        /// </summary>
        /// <typeparam name="TScore">Score</typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="member">member<para>元素值</para></param>
        /// <param name="score">score<para>排序分数</para></param>
        /// <param name="ch">
        /// Available since: 3.0.2. Whether to change the return value to the number of affected members. By default, only the number of new members is counted.
        /// <para>If the elements and scores are the same, they are not counted</para>
        /// <para>支持此参数的Redis版本, 3.0.2+. 是否返回受影响的元素个数. 默认情况只统计新增元素数量. 如果元素和分数都一样, 也不计入统计</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 3.0.2. Nx: Only add new elements. Don't update already existing elements.
        /// <para>Xx: Only update elements that already exist. Don't add new elements.</para>
        /// <para>支持此参数的Redis版本, 3.0.2+. Nx: 仅添加新元素, 不更新已经存在的元素</para>
        /// <para>Xx: 只更新已经存在的元素, 不添加新元素</para>
        /// </param>
        /// <param name="gl">
        /// Available since: 6.2.0. Lt: Only update existing elements if the new score is less than the current score. This flag doesn't prevent adding new elements.
        /// <para>Gt: Only update existing elements if the new score is greater than the current score. This flag doesn't prevent adding new elements.</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. Lt: 如果新分数小于当前分数，则仅更新现有元素. 该标志不会阻止添加新元素</para>
        /// <para>Gt: 仅当新分数大于当前分数时才更新现有元素. 该标志不会阻止添加新元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of new members
        /// <para>新增元素数量</para>
        /// </returns>
        public long ZAdd<TScore>(string key, byte[] member, TScore score, bool ch = false, NxXx nxx = NxXx.None, GtLt gl = GtLt.None, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZAdd(key, member, score, nxx: nxx, ch: ch, gl: gl), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key.
        /// <para>Available since: 2.4.0 | 3.0.2 | 6.2.0</para>
        /// <para>往指定的SortedSet中插入多个带有排序分数的元素</para>
        /// <para>支持此命令的Redis版本, 2.4.0+ | 3.0.2 | 6.2.0</para>
        /// </summary>
        /// <typeparam name="TScore">Score</typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="ms">member-score</param>
        /// <param name="ch">
        /// Available since: 3.0.2. Whether to change the return value to the number of affected members. By default, only the number of new members is counted.
        /// <para>If the elements and scores are the same, they are not counted</para>
        /// <para>支持此参数的Redis版本, 3.0.2+. 是否返回受影响的元素个数. 默认情况只统计新增元素数量. 如果元素和分数都一样, 也不计入统计</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 3.0.2. Nx: Only add new elements. Don't update already existing elements.
        /// <para>Xx: Only update elements that already exist. Don't add new elements.</para>
        /// <para>支持此参数的Redis版本, 3.0.2+. Nx: 仅添加新元素, 不更新已经存在的元素</para>
        /// <para>Xx: 只更新已经存在的元素, 不添加新元素</para>
        /// </param>
        /// <param name="gl">
        /// Available since: 6.2.0. Lt: Only update existing elements if the new score is less than the current score. This flag doesn't prevent adding new elements.
        /// <para>Gt: Only update existing elements if the new score is greater than the current score. This flag doesn't prevent adding new elements.</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. Lt: 如果新分数小于当前分数，则仅更新现有元素. 该标志不会阻止添加新元素</para>
        /// <para>Gt: 仅当新分数大于当前分数时才更新现有元素. 该标志不会阻止添加新元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of new members
        /// <para>新增元素数量</para>
        /// </returns>
        public long ZAdd<TScore>(string key, Dictionary<string, TScore> ms, bool ch = false, NxXx nxx = NxXx.None, GtLt gl = GtLt.None, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZAdd(key, ms, nxx: nxx, ch: ch, gl: gl), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key.
        /// <para>Available since: 2.4.0 | 3.0.2 | 6.2.0</para>
        /// <para>往指定的SortedSet中插入多个带有排序分数的元素</para>
        /// <para>支持此命令的Redis版本, 2.4.0+ | 3.0.2 | 6.2.0</para>
        /// </summary>
        /// <typeparam name="TScore">Score</typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="ms">member-score</param>
        /// <param name="ch">
        /// Available since: 3.0.2. Whether to change the return value to the number of affected members. By default, only the number of new members is counted.
        /// <para>If the elements and scores are the same, they are not counted</para>
        /// <para>支持此参数的Redis版本, 3.0.2+. 是否返回受影响的元素个数. 默认情况只统计新增元素数量. 如果元素和分数都一样, 也不计入统计</para>
        /// </param>
        /// <param name="nxx">
        /// Available since: 3.0.2. Nx: Only add new elements. Don't update already existing elements.
        /// <para>Xx: Only update elements that already exist. Don't add new elements.</para>
        /// <para>支持此参数的Redis版本, 3.0.2+. Nx: 仅添加新元素, 不更新已经存在的元素</para>
        /// <para>Xx: 只更新已经存在的元素, 不添加新元素</para>
        /// </param>
        /// <param name="gl">
        /// Available since: 6.2.0. Lt: Only update existing elements if the new score is less than the current score. This flag doesn't prevent adding new elements.
        /// <para>Gt: Only update existing elements if the new score is greater than the current score. This flag doesn't prevent adding new elements.</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. Lt: 如果新分数小于当前分数，则仅更新现有元素. 该标志不会阻止添加新元素</para>
        /// <para>Gt: 仅当新分数大于当前分数时才更新现有元素. 该标志不会阻止添加新元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of new members
        /// <para>新增元素数量</para>
        /// </returns>
        public long ZAdd<TScore>(string key, Dictionary<byte[], TScore> ms, bool ch = false, NxXx nxx = NxXx.None, GtLt gl = GtLt.None, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZAdd(key, ms, nxx: nxx, ch: ch, gl: gl), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the sorted set cardinality (number of elements) of the sorted set stored at key.
        /// <para>Available since: 1.2.0</para>
        /// <para>返回指定Key的SortedSet集合中的元素总个数</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>
        /// the cardinality (number of members) of the sorted set, or 0 if the key doesn't exist.
        /// <para>SortedSet集合中的元素总个数, 不存在Key返回0</para>
        /// </returns>
        public long ZCard(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZCard(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the number of elements in the sorted set at key with a score between min and max.
        /// <para>Available since: 2.0.0</para>
        /// <para>返回指定Key的SortedSet集合中介于min 和 max直接的元素数量</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <typeparam name="TScore">
        /// Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="withMax">
        /// Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="withMin">
        /// Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of members in the specified score range.
        /// <para>min 到 max之间的元素数量</para>
        /// </returns>
        public long ZCount<TScore>(string key, TScore min, TScore max, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZCount(key, min, max, withMin, withMax), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZDiff(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public string[] ZDiff(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArray<string>(SortedSetCommands.ZDiff(false, [key, .. keys]), ResultType.Array | ResultType.String, cancellationToken)!;
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallClassArray<string>(SortedSetCommands.ZDiff(false, liskKeys.ToArray()), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
#endif
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">SortedSet key1</param>
        /// <param name="key2">SortedSet key2</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZDiff(string key1, string key2, CancellationToken cancellationToken = default)
#else
        public string[] ZDiff(string key1, string key2, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZDiff(false, key1, key2), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZDiff(string[] keys, CancellationToken cancellationToken = default)
#else
        public string[] ZDiff(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZDiff(false, keys), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZDiffBytes(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public byte[][] ZDiffBytes(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZDiff(false, [key, .. keys]), ResultType.Array | ResultType.Bytes, cancellationToken)!;
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZDiff(false, liskKeys.ToArray()), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
#endif
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">SortedSet key1</param>
        /// <param name="key2">SortedSet key2</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZDiffBytes(string key1, string key2, CancellationToken cancellationToken = default)
#else
        public byte[][] ZDiffBytes(string key1, string key2, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZDiff(false, key1, key2), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZDiffBytes(string[] keys, CancellationToken cancellationToken = default)
#else
        public byte[][] ZDiffBytes(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZDiff(false, keys), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZDiffWithScores(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZDiffWithScores(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZDiff(false, [key, .. keys]), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZDiff(false, liskKeys.ToArray()), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#endif
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">SortedSet key1</param>
        /// <param name="key2">SortedSet key2</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZDiffWithScores(string key1, string key2, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZDiffWithScores(string key1, string key2, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZDiff(true, key1, key2), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZDiffWithScores(string[] keys, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZDiffWithScores(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZDiff(true, keys), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZDiffWithScoresBytes(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZDiffWithScoresBytes(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZDiff(false, [key, .. keys]), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZDiff(false, liskKeys.ToArray()), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#endif
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">SortedSet key1</param>
        /// <param name="key2">SortedSet key2</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZDiffWithScoresBytes(string key1, string key2, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZDiffWithScoresBytes(string key1, string key2, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZDiff(true, key1, key2), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// This command is similar to ZDIFFSTORE, but instead of storing the resulting sorted set, it is returned to the client.
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于 ZDIFFSTORE, 但不是转存结果, 而是将结果返回</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Set of difference set members
        /// <para>差集元素的集合</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZDiffWithScoresBytes(string[] keys, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZDiffWithScoresBytes(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZDiff(true, keys), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Computes the difference between the first and all successive input sorted sets and stores the result in destination
        /// <para>Available since: 6.2.0</para>
        /// <para>计算第一个和所有连续输入排序集之间的差异并将结果存储在 destination 中</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="destination">destination key</param>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of members in the resulting sorted set at destination.
        /// <para>插入destination的元素个数</para>
        /// </returns>
        public long ZDiffStore(string destination, string key, string[] keys, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumber<long>(SortedSetCommands.ZDiffStore(destination, [key, .. keys]), ResultType.Int64, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallNumber<long>(SortedSetCommands.ZDiffStore(destination, liskKeys.ToArray()), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the difference between the first and all successive input sorted sets and stores the result in destination
        /// <para>Available since: 6.2.0</para>
        /// <para>计算第一个和所有连续输入排序集之间的差异并将结果存储在 destination 中</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="destination">destination key</param>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of members in the resulting sorted set at destination.
        /// <para>插入destination的元素个数</para>
        /// </returns>
        public long ZDiffStore(string destination, string key1, string key2, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZDiffStore(destination, key1, key2), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Computes the difference between the first and all successive input sorted sets and stores the result in destination
        /// <para>Available since: 6.2.0</para>
        /// <para>计算第一个和所有连续输入排序集之间的差异并将结果存储在 destination 中</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="destination">destination key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of members in the resulting sorted set at destination.
        /// <para>插入destination的元素个数</para>
        /// </returns>
        public long ZDiffStore(string destination, string[] keys, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZDiffStore(destination, keys), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0). If key does not exist, a new sorted set with the specified member as its sole member is created.
        /// <para>Available since: 1.2.0</para>
        /// <para>将指定Key的SortedSet集合中指定的元素分数递增</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="increment">increment
        /// <para>递增的分数</para>
        /// </param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the new score of member.
        /// <para>递增之后的分数</para>
        /// </returns>
        public NumberValue ZIncrBy<TIncrement>(string key, TIncrement increment, string member, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TIncrement : struct, System.Numerics.INumber<TIncrement>
#else
            where TIncrement : struct, System.IEquatable<TIncrement>
#endif
        {
            return base._call.CallNumber(SortedSetCommands.ZIncrBy(key, increment, member), cancellationToken);
        }

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0). If key does not exist, a new sorted set with the specified member as its sole member is created.
        /// <para>Available since: 1.2.0</para>
        /// <para>将指定Key的SortedSet集合中指定的元素分数递增</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="increment">increment
        /// <para>递增的分数</para>
        /// </param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the new score of member.
        /// <para>递增之后的分数</para>
        /// </returns>
        public NumberValue ZIncrBy<TIncrement>(string key, TIncrement increment, byte[] member, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TIncrement : struct, System.Numerics.INumber<TIncrement>
#else
            where TIncrement : struct, System.IEquatable<TIncrement>
#endif
        {
            return base._call.CallNumber(SortedSetCommands.ZIncrBy(key, increment, member), cancellationToken);
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members
        /// <para>交集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZInter(string key1, string key2, CancellationToken cancellationToken = default)
#else
        public string[] ZInter(string key1, string key2, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZInter(Aggregate.Sum, false, key1, key2), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members
        /// <para>交集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZInter(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public string[] ZInter(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArray<string>(SortedSetCommands.ZInter(Aggregate.Sum, false, [key, .. keys]), ResultType.Array | ResultType.String, cancellationToken)!;
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallClassArray<string>(SortedSetCommands.ZInter(Aggregate.Sum, false, liskKeys.ToArray()), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members
        /// <para>交集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZInter(string[] keys, CancellationToken cancellationToken = default)
#else
        public string[] ZInter(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZInter(Aggregate.Sum, false, keys), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members
        /// <para>交集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZInterBytes(string key1, string key2, CancellationToken cancellationToken = default)
#else
        public byte[][] ZInterBytes(string key1, string key2, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZInter(Aggregate.Sum, false, key1, key2), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members
        /// <para>交集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZInterBytes(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public byte[][] ZInterBytes(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZInter(Aggregate.Sum, false, [key, .. keys]), ResultType.Array | ResultType.Bytes, cancellationToken)!;
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZInter(Aggregate.Sum, false, liskKeys.ToArray()), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members
        /// <para>交集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZInterBytes(string[] keys, CancellationToken cancellationToken = default)
#else
        public byte[][] ZInterBytes(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZInter(Aggregate.Sum, false, keys), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZInterWithScores(string key1, string key2, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZInterWithScores(string key1, string key2, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZInter(aggregate, true, key1, key2), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZInterWithScores(string key, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZInterWithScores(string key, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZInter(aggregate, true, [key, .. keys]), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZInter(aggregate, true, liskKeys.ToArray()), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZInterWithScores(string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZInterWithScores(string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZInter(aggregate, true, keys), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZInterWithScoresBytes(string key1, string key2, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZInterWithScoresBytes(string key1, string key2, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZInter(aggregate, true, key1, key2), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZInterWithScoresBytes(string key, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZInterWithScoresBytes(string key, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZInter(aggregate, true, [key, .. keys]), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZInter(aggregate, true, liskKeys.ToArray()), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZInterWithScoresBytes(string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZInterWithScoresBytes(string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZInter(aggregate, true, keys), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key1Weight">Key1 multiplication factor<para>Key1排序分数要乘的数</para></param>
        /// <param name="key2">key2</param>
        /// <param name="key2Weight">Key2 multiplication factor<para>Key2排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZInterWithScores<TWeight>(string key1, TWeight key1Weight, string key2, TWeight key2Weight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZInterWithScores<TWeight>(string key1, TWeight key1Weight, string key2, TWeight key2Weight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZInter([key1, key2], [key1Weight, key2Weight], aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#else
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZInter(new string[] { key1, key2 }, new TWeight[] { key1Weight, key2Weight }, aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keyWeight">Key multiplication factor<para>Key排序分数要乘的数</para></param>
        /// <param name="keys">keys</param>
        /// <param name="keysWeight">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZInterWithScores<TWeight>(string key, TWeight keyWeight, string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZInterWithScores<TWeight>(string key, TWeight keyWeight, string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZInter([key, .. keys], [keyWeight, .. keysWeight], aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            var liskWeights = new List<TWeight>(keys.Length + 1) { keyWeight };
            liskWeights.AddRange(keysWeight);
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZInter(liskKeys.ToArray(), liskWeights.ToArray(), aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="keysWeight">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZInterWithScores<TWeight>(string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZInterWithScores<TWeight>(string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZInter(keys, keysWeight, aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key1Weight">Key1 multiplication factor<para>Key1排序分数要乘的数</para></param>
        /// <param name="key2">key2</param>
        /// <param name="key2Weight">Key2 multiplication factor<para>Key2排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZInterWithScoresBytes<TWeight>(string key1, TWeight key1Weight, string key2, TWeight key2Weight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZInterWithScoresBytes<TWeight>(string key1, TWeight key1Weight, string key2, TWeight key2Weight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZInter([key1, key2], [key1Weight, key2Weight], aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#else
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZInter(new string[] { key1, key2 }, new TWeight[] { key1Weight, key2Weight }, aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keyWeight">Key multiplication factor<para>Key排序分数要乘的数</para></param>
        /// <param name="keys">keys</param>
        /// <param name="keysWeight">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZInterWithScoresBytes<TWeight>(string key, TWeight keyWeight, string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZInterWithScoresBytes<TWeight>(string key, TWeight keyWeight, string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZInter([key, .. keys], [keyWeight, .. keysWeight], aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            var liskWeights = new List<TWeight>(keys.Length + 1) { keyWeight };
            liskWeights.AddRange(keysWeight);
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZInter(liskKeys.ToArray(), liskWeights.ToArray(), aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="keysWeight">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Intersection members with score
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZInterWithScoresBytes<TWeight>(string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZInterWithScoresBytes<TWeight>(string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZInter(keys, keysWeight, aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// This command is similar to ZINTER, but instead of returning the result set, it returns just the cardinality of the result.
        /// <para>Available since: 7.0.0</para>
        /// <para>和ZINTER类似, 不过只返回交集元素个数</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="limit">When provided with the optional LIMIT argument (which defaults to 0 and means unlimited), if the intersection cardinality reaches limit partway through the computation, the algorithm will exit and yield limit as the cardinality
        /// <para>最大计算个数, 默认为0表示无限制. 如果给定值, 则如果交集个数达到该值将结束计算. 可以提高性能</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members in the resulting intersection
        /// <para>交集元素个数</para>
        /// </returns>
        public long ZInterCard(string key1, string key2, ulong limit = 0, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZInterCard(limit, key1, key2), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is similar to ZINTER, but instead of returning the result set, it returns just the cardinality of the result.
        /// <para>Available since: 7.0.0</para>
        /// <para>和ZINTER类似, 不过只返回交集元素个数</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="limit">When provided with the optional LIMIT argument (which defaults to 0 and means unlimited), if the intersection cardinality reaches limit partway through the computation, the algorithm will exit and yield limit as the cardinality
        /// <para>最大计算个数, 默认为0表示无限制. 如果给定值, 则如果交集个数达到该值将结束计算. 可以提高性能</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members in the resulting intersection
        /// <para>交集元素个数</para>
        /// </returns>
        public long ZInterCard(string key, string[] keys, ulong limit = 0, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumber<long>(SortedSetCommands.ZInterCard(limit, [key, .. keys]), ResultType.Int64, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallNumber<long>(SortedSetCommands.ZInterCard(limit, liskKeys.ToArray()), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// This command is similar to ZINTER, but instead of returning the result set, it returns just the cardinality of the result.
        /// <para>Available since: 7.0.0</para>
        /// <para>和ZINTER类似, 不过只返回交集元素个数</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="limit">When provided with the optional LIMIT argument (which defaults to 0 and means unlimited), if the intersection cardinality reaches limit partway through the computation, the algorithm will exit and yield limit as the cardinality
        /// <para>最大计算个数, 默认为0表示无限制. 如果给定值, 则如果交集个数达到该值将结束计算. 可以提高性能</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members in the resulting intersection
        /// <para>交集元素个数</para>
        /// </returns>
        public long ZInterCard(string[] keys, ulong limit = 0, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZInterCard(limit, keys), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的交集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <typeparam name="TWeight">TWeight</typeparam>
        /// <param name="destination">Save the key of the intersection result
        /// <para>保存交集结果的key</para>
        /// </param>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="key1Weight">Key1 multiplication factor<para>Key1排序分数要乘的数</para></param>
        /// <param name="key2Weight">Key2 multiplication factor<para>Key2排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members in the resulting sorted set at the destination
        /// <para>存入destination中的交集元素个数</para>
        /// </returns>
        public long ZInterStore<TWeight>(string destination, string key1, TWeight key1Weight, string key2, TWeight key2Weight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            var weights = new TWeight[] { key1Weight, key2Weight };
#if NET8_0_OR_GREATER
            return base._call.CallNumber<long>(SortedSetCommands.ZInterStore(destination, [key1, key2], weights, aggregate), ResultType.Int64, cancellationToken);
#else
            return base._call.CallNumber<long>(SortedSetCommands.ZInterStore(destination, new string[] { key1, key2 }, weights, aggregate), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的交集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="destination">Save the key of the intersection result
        /// <para>保存交集结果的key</para>
        /// </param>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members in the resulting sorted set at the destination
        /// <para>存入destination中的交集元素个数</para>
        /// </returns>
        public long ZInterStore(string destination, string key1, string key2, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumber<long>(SortedSetCommands.ZInterStore<int>(destination, [key1, key2], null, aggregate), ResultType.Int64, cancellationToken);
#else
            return base._call.CallNumber<long>(SortedSetCommands.ZInterStore<int>(destination, new string[] { key1, key2 }, null, aggregate), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的交集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <typeparam name="TWeight">TWeight</typeparam>
        /// <param name="destination">Save the key of the intersection result
        /// <para>保存交集结果的key</para>
        /// </param>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="keyWeight">Key multiplication factor<para>Key排序分数要乘的数</para></param>
        /// <param name="keysWeights">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members in the resulting sorted set at the destination
        /// <para>存入destination中的交集元素个数</para>
        /// </returns>
        public long ZInterStore<TWeight>(string destination, string key, TWeight keyWeight, string[] keys, TWeight[] keysWeights, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumber<long>(SortedSetCommands.ZInterStore(destination, [key, .. keys], [keyWeight, .. keysWeights], aggregate), ResultType.Int64, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            var weights = new List<TWeight>(keys.Length + 1) { keyWeight };
            weights.AddRange(keysWeights);
            return base._call.CallNumber<long>(SortedSetCommands.ZInterStore(destination, liskKeys.ToArray(), weights.ToArray(), aggregate), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的交集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="destination">Save the key of the intersection result
        /// <para>保存交集结果的key</para>
        /// </param>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members in the resulting sorted set at the destination
        /// <para>存入destination中的交集元素个数</para>
        /// </returns>
        public long ZInterStore(string destination, string key, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumber<long>(SortedSetCommands.ZInterStore<int>(destination, [key, .. keys], null, aggregate), ResultType.Int64, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallNumber<long>(SortedSetCommands.ZInterStore<int>(destination, liskKeys.ToArray(), null, aggregate), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的交集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <typeparam name="TWeight">TWeight</typeparam>
        /// <param name="destination">Save the key of the intersection result
        /// <para>保存交集结果的key</para>
        /// </param>
        /// <param name="keys">keys</param>
        /// <param name="weights">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members in the resulting sorted set at the destination
        /// <para>存入destination中的交集元素个数</para>
        /// </returns>
        public long ZInterStore<TWeight>(string destination, string[] keys, TWeight[] weights, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZInterStore(destination, keys, weights, aggregate), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Computes the intersection of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的交集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="destination">Save the key of the intersection result
        /// <para>保存交集结果的key</para>
        /// </param>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members in the resulting sorted set at the destination
        /// <para>存入destination中的交集元素个数</para>
        /// </returns>
        public long ZInterStore(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZInterStore<int>(destination, keys, null, aggregate), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns the number of elements in the sorted set at key with a value between min and max
        /// <para>Available since: 2.8.9</para>
        /// <para>计算排序集合（sorted set）中指定字典序范围内的成员数量</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members in the specified score range
        /// <para>指定范围内的元素个数</para>
        /// </returns>
        public long ZLexCount(string key, string min, string max, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZLexCount(key, min, max), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Pops one element, that are member-score pairs, from the first non-empty sorted set in the provided list of key name
        /// <para>Available since: 7.0.0</para>
        /// <para>从给定Key的第一个非空有序集合中弹出一个元素, 带排序分返回</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="mm">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped
        /// <para>Max: 从最大排序分弹出, Min: 从最小排序分弹出</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: member-score
        /// <para>Key: 弹出的集合Key, Value: 弹出的元素和排序分数</para>
        /// </returns>
        public KeyValuePair<string, MemberScoreValue<string>>? ZMPop(string key, MaxMin mm, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.ZMPop([key], mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#else
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.ZMPop(new string[] { key }, mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#endif
        }

        /// <summary>
        /// Pops one or more elements, that are member-score pairs, from the first non-empty sorted set in the provided list of key names
        /// <para>Available since: 7.0.0</para>
        /// <para>从给定Key的第一个非空有序集合中弹出N个元素, 带排序分返回</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="mm">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped
        /// <para>Max: 从最大排序分弹出, Min: 从最小排序分弹出</para>
        /// </param>
        /// <param name="count">The COUNT can be used to specify the number of elements to pop
        /// <para>弹出个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: member-score[]
        /// <para>Key: 弹出的集合Key, Value: 弹出的元素和排序分数</para>
        /// </returns>
        public KeyValuePair<string, MemberScoreValue<string>[]>? ZMPop(string key, MaxMin mm, ulong count, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>[]>>(SortedSetCommands.ZMPop([key], mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#else
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>[]>>(SortedSetCommands.ZMPop(new string[] { key }, mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#endif
        }

        /// <summary>
        /// Pops one element, that are member-score pairs, from the first non-empty sorted set in the provided list of key name
        /// <para>Available since: 7.0.0</para>
        /// <para>从给定Key的第一个非空有序集合中弹出一个元素, 带排序分返回</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="mm">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped
        /// <para>Max: 从最大排序分弹出, Min: 从最小排序分弹出</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: member-score
        /// <para>Key: 弹出的集合Key, Value: 弹出的元素和排序分数</para>
        /// </returns>
        public KeyValuePair<string, MemberScoreValue<string>>? ZMPop(string[] keys, MaxMin mm, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.ZMPop(keys, mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Pops one or more elements, that are member-score pairs, from the first non-empty sorted set in the provided list of key names
        /// <para>Available since: 7.0.0</para>
        /// <para>从给定Key的第一个非空有序集合中弹出N个元素, 带排序分返回</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="mm">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped
        /// <para>Max: 从最大排序分弹出, Min: 从最小排序分弹出</para>
        /// </param>
        /// <param name="count">The COUNT can be used to specify the number of elements to pop
        /// <para>弹出个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: member-score[]
        /// <para>Key: 弹出的集合Key, Value: 弹出的元素和排序分数</para>
        /// </returns>
        public KeyValuePair<string, MemberScoreValue<string>[]>? ZMPop(string[] keys, MaxMin mm, ulong count, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>[]>>(SortedSetCommands.ZMPop(keys, mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Pops one element, that are member-score pairs, from the first non-empty sorted set in the provided list of key name
        /// <para>Available since: 7.0.0</para>
        /// <para>从给定Key的第一个非空有序集合中弹出一个元素, 带排序分返回</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="mm">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped
        /// <para>Max: 从最大排序分弹出, Min: 从最小排序分弹出</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: member-score
        /// <para>Key: 弹出的集合Key, Value: 弹出的元素和排序分数</para>
        /// </returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? ZMPopBytes(string key, MaxMin mm, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.ZMPop([key], mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#else
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.ZMPop(new string[] { key }, mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#endif
        }

        /// <summary>
        /// Pops one or more elements, that are member-score pairs, from the first non-empty sorted set in the provided list of key names
        /// <para>Available since: 7.0.0</para>
        /// <para>从给定Key的第一个非空有序集合中弹出N个元素, 带排序分返回</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="mm">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped
        /// <para>Max: 从最大排序分弹出, Min: 从最小排序分弹出</para>
        /// </param>
        /// <param name="count">The COUNT can be used to specify the number of elements to pop
        /// <para>弹出个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: member-score[]
        /// <para>Key: 弹出的集合Key, Value: 弹出的元素和排序分数</para>
        /// </returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>[]>? ZMPopBytes(string key, MaxMin mm, ulong count, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>[]>>(SortedSetCommands.ZMPop([key], mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#else
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>[]>>(SortedSetCommands.ZMPop(new string[] { key }, mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#endif
        }

        /// <summary>
        /// Pops one element, that are member-score pairs, from the first non-empty sorted set in the provided list of key name
        /// <para>Available since: 7.0.0</para>
        /// <para>从给定Key的第一个非空有序集合中弹出一个元素, 带排序分返回</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="mm">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped
        /// <para>Max: 从最大排序分弹出, Min: 从最小排序分弹出</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: member-score
        /// <para>Key: 弹出的集合Key, Value: 弹出的元素和排序分数</para>
        /// </returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? ZMPopBytes(string[] keys, MaxMin mm, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.ZMPop(keys, mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Pops one or more elements, that are member-score pairs, from the first non-empty sorted set in the provided list of key names
        /// <para>Available since: 7.0.0</para>
        /// <para>从给定Key的第一个非空有序集合中弹出N个元素, 带排序分返回</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="mm">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped
        /// <para>Max: 从最大排序分弹出, Min: 从最小排序分弹出</para>
        /// </param>
        /// <param name="count">The COUNT can be used to specify the number of elements to pop
        /// <para>弹出个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: member-score[]
        /// <para>Key: 弹出的集合Key, Value: 弹出的元素和排序分数</para>
        /// </returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>[]>? ZMPopBytes(string[] keys, MaxMin mm, ulong count, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>[]>>(SortedSetCommands.ZMPop(keys, mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Returns the scores associated with the specified members in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>获得指定元素的排序分数</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Score</returns>
        public NumberValue ZMScore(string key, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber(SortedSetCommands.ZMScore(key, member), cancellationToken);
        }

        /// <summary>
        /// Returns the scores associated with the specified members in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>获得指定元素的排序分数</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Score</returns>
        public NumberValue[] ZMScore(string key, string[] members, CancellationToken cancellationToken = default)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return base._call.CallClassArray<NumberValue>(SortedSetCommands.ZMScore(key, members), ResultType.Array | ResultType.Number, cancellationToken)!;
#else
            return base._call.CallClassArray<NumberValue>(SortedSetCommands.ZMScore(key, members), ResultType.Array | ResultType.Number, cancellationToken);
#endif
        }

        /// <summary>
        /// Returns the scores associated with the specified members in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>获得指定元素的排序分数</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Score</returns>
        public NumberValue ZMScore(string key, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber(SortedSetCommands.ZMScore<byte[]>(key, member), cancellationToken);
        }

        /// <summary>
        /// Returns the scores associated with the specified members in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>获得指定元素的排序分数</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Score</returns>
        public NumberValue[] ZMScore(string key, byte[][] members, CancellationToken cancellationToken = default)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return base._call.CallClassArray<NumberValue>(SortedSetCommands.ZMScore(key, members), ResultType.Array | ResultType.Number, cancellationToken)!;
#else
            return base._call.CallClassArray<NumberValue>(SortedSetCommands.ZMScore(key, members), ResultType.Array | ResultType.Number, cancellationToken);
#endif
        }

        /// <summary>
        /// Removes and returns up a members with the highest scores in the sorted set stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>删除并返回排序分最高的一个元素, 包含分数</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public MemberScoreValue<string>? ZPopMax(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<MemberScoreValue<string>>(SortedSetCommands.ZPopMax(key, 1), ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Removes and returns up to count members with the highest scores in the sorted set stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>删除并返回排序分最高的N个元素, 包含分数</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="count">count<para>要返回的个数</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZPopMax(string key, ulong count, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZPopMax(string key, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZPopMax(key, count), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Removes and returns up a members with the highest scores in the sorted set stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>删除并返回排序分最高的一个元素, 包含分数</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public MemberScoreValue<byte[]>? ZPopMaxBytes(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<MemberScoreValue<byte[]>>(SortedSetCommands.ZPopMax(key, 1), ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Removes and returns up to count members with the highest scores in the sorted set stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>删除并返回排序分最高的N个元素, 包含分数</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="count">count<para>要返回的个数</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZPopMaxBytes(string key, ulong count, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZPopMaxBytes(string key, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZPopMax(key, count), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Removes and returns up a members with the lowest scores in the sorted set stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>删除并返回排序分最低的一个元素, 包含分数</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public MemberScoreValue<string>? ZPopMin(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<MemberScoreValue<string>>(SortedSetCommands.ZPopMin(key, 1), ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Removes and returns up to count members with the lowest scores in the sorted set stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>删除并返回排序分最低的N个元素, 包含分数</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="count">count<para>要返回的个数</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZPopMin(string key, ulong count, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZPopMin(string key, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZPopMin(key, count), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Removes and returns up a members with the lowest scores in the sorted set stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>删除并返回排序分最低的一个元素, 包含分数</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public MemberScoreValue<byte[]>? ZPopMinBytes(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<MemberScoreValue<byte[]>>(SortedSetCommands.ZPopMin(key, 1), ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Removes and returns up to count members with the lowest scores in the sorted set stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>删除并返回排序分最低的N个元素, 包含分数</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="count">count<para>要返回的个数</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZPopMinBytes(string key, ulong count, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZPopMinBytes(string key, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZPopMin(key, count), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Return a random element from the sorted set value stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定的SortedSet中随机获得一个元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>member</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? ZRandMember(string key, CancellationToken cancellationToken = default)
#else
        public string ZRandMember(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(SortedSetCommands.ZRandMember(key, null, false), cancellationToken);
        }

        /// <summary>
        /// Return a random element from the sorted set value stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定的SortedSet中随机获得一个元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>member</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? ZRandMemberBytes(string key, CancellationToken cancellationToken = default)
#else
        public byte[] ZRandMemberBytes(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(SortedSetCommands.ZRandMember(key, null, false), cancellationToken);
        }

        /// <summary>
        /// Return a random element from the sorted set value stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定的SortedSet中随机获得一个元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public MemberScoreValue<string>? ZRandMemberWithScores(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<MemberScoreValue<string>>(SortedSetCommands.ZRandMember(key, 1, true), ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Return a random element from the sorted set value stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定的SortedSet中随机获得一个元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public MemberScoreValue<byte[]>? ZRandMemberWithScoresBytes(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<MemberScoreValue<byte[]>>(SortedSetCommands.ZRandMember(key, 1, true), ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Returns the specified count of random elements from the sorted set value stored in key
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定的SortedSet中随机获得指定数量的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="count">When it is negative, there may be duplicate elements. Is a positive number, and there are no duplicate elements
        /// <para>为负数时, 可能存在重复元素. 为正数, 不存在重复元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>members</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRandMember(string key, long count, CancellationToken cancellationToken = default)
#else
        public string[] ZRandMember(string key, long count, CancellationToken cancellationToken = default)
#endif
        {
            if (count == 0) throw new RedisException("The count argument cannot be 0");
            return base._call.CallClassArray<string>(SortedSetCommands.ZRandMember(key, count, false), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns the specified count of random elements from the sorted set value stored in key
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定的SortedSet中随机获得指定数量的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="count">When it is negative, there may be duplicate elements. Is a positive number, and there are no duplicate elements
        /// <para>为负数时, 可能存在重复元素. 为正数, 不存在重复元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>members</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRandMemberBytes(string key, long count, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRandMemberBytes(string key, long count, CancellationToken cancellationToken = default)
#endif
        {
            if (count == 0) throw new RedisException("The count argument cannot be 0");
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRandMember(key, count, false), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns the specified count of random elements from the sorted set value stored in key
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定的SortedSet中随机获得指定数量的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="count">When it is negative, there may be duplicate elements. Is a positive number, and there are no duplicate elements
        /// <para>为负数时, 可能存在重复元素. 为正数, 不存在重复元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZRandMemberWithScores(string key, long count, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZRandMemberWithScores(string key, long count, CancellationToken cancellationToken = default)
#endif
        {
            if (count == 0) throw new RedisException("The count argument cannot be 0");
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZRandMember(key, count, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns the specified count of random elements from the sorted set value stored in key
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定的SortedSet中随机获得指定数量的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="count">When it is negative, there may be duplicate elements. Is a positive number, and there are no duplicate elements
        /// <para>为负数时, 可能存在重复元素. 为正数, 不存在重复元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZRandMemberWithScoresBytes(string key, long count, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZRandMemberWithScoresBytes(string key, long count, CancellationToken cancellationToken = default)
#endif
        {
            if (count == 0) throw new RedisException("The count argument cannot be 0");
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZRandMember(key, count, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 1.2.0 | 6.2.0</para>
        /// <para>根据下标返回指定范围内的元素, 下标前后都将包含在内</para>
        /// <para>支持此命令的Redis版本, 1.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start index<para>开始下标, 包含此下标</para></param>
        /// <param name="stop">stop index<para>结束下标, 包含此下标</para></param>
        /// <param name="rev">Available since: 6.2.0. The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>支持此参数的Redis版本, 6.2.0+. 是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>members</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRange(string key, long start, long stop, bool rev = false, CancellationToken cancellationToken = default)
#else
        public string[] ZRange(string key, long start, long stop, bool rev = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRange(key, start, false, stop, false, null, rev, null, null, false), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 1.2.0 | 6.2.0</para>
        /// <para>根据下标返回指定范围内的元素, 下标前后都将包含在内</para>
        /// <para>支持此命令的Redis版本, 1.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start index<para>开始下标, 包含此下标</para></param>
        /// <param name="stop">stop index<para>结束下标, 包含此下标</para></param>
        /// <param name="rev">Available since: 6.2.0. The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>支持此参数的Redis版本, 6.2.0+. 是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>members</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRangeBytes(string key, long start, long stop, bool rev = false, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRangeBytes(string key, long start, long stop, bool rev = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRange(key, start, false, stop, false, null, rev, null, null, false), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 1.2.0 | 6.2.0</para>
        /// <para>根据下标返回指定范围内的元素, 下标前后都将包含在内</para>
        /// <para>支持此命令的Redis版本, 1.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start index<para>开始下标, 包含此下标</para></param>
        /// <param name="stop">stop index<para>结束下标, 包含此下标</para></param>
        /// <param name="rev">Available since: 6.2.0. The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>支持此参数的Redis版本, 6.2.0+. 是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>members</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZRangeWithScores(string key, long start, long stop, bool rev = false, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZRangeWithScores(string key, long start, long stop, bool rev = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZRange(key, start, false, stop, false, null, rev, null, null, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 1.2.0 | 6.2.0</para>
        /// <para>根据下标返回指定范围内的元素, 下标前后都将包含在内</para>
        /// <para>支持此命令的Redis版本, 1.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start index<para>开始下标, 包含此下标</para></param>
        /// <param name="stop">stop index<para>结束下标, 包含此下标</para></param>
        /// <param name="rev">Available since: 6.2.0. The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>支持此参数的Redis版本, 6.2.0+. 是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>members</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZRangeWithScoresBytes(string key, long start, long stop, bool rev = false, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZRangeWithScoresBytes(string key, long start, long stop, bool rev = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZRange(key, start, false, stop, false, null, rev, null, null, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>根据元素或排序分数返回指定范围内的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TArg">TArg</typeparam>
        /// <param name="by">BYSCORE | BYLEX
        /// <para>根据排序分或元素排序</para>
        /// </param>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start</param>
        /// <param name="stop">stop</param>
        /// <param name="rev">The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="withStart">The BYSCORE takes effect only when specified. Whether to include a start score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含开始分数, 默认包含</para>
        /// </param>
        /// <param name="withStop">The BYSCORE takes effect only when specified. Whether to include a stop score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含结束分数, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRange<TArg>(ZRangeBy by, string key, TArg start, TArg stop, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#else
        public string[] ZRange<TArg>(ZRangeBy by, string key, TArg start, TArg stop, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRange(key, start, withStart, stop, withStop, by, rev, null, null, false), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>根据元素或排序分数返回指定范围内的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TArg">TArg</typeparam>
        /// <param name="by">BYSCORE | BYLEX
        /// <para>根据排序分或元素排序</para>
        /// </param>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start</param>
        /// <param name="stop">stop</param>
        /// <param name="rev">The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="withStart">The BYSCORE takes effect only when specified. Whether to include a start score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含开始分数, 默认包含</para>
        /// </param>
        /// <param name="withStop">The BYSCORE takes effect only when specified. Whether to include a stop score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含结束分数, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRangeBytes<TArg>(ZRangeBy by, string key, TArg start, TArg stop, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRangeBytes<TArg>(ZRangeBy by, string key, TArg start, TArg stop, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRange(key, start, withStart, stop, withStop, by, rev, null, null, false), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>根据元素或排序分数返回指定范围内的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TArg">TArg</typeparam>
        /// <param name="by">BYSCORE | BYLEX
        /// <para>根据排序分或元素排序</para>
        /// </param>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start</param>
        /// <param name="stop">stop</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="rev">The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="withStart">The BYSCORE takes effect only when specified. Whether to include a start score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含开始分数, 默认包含</para>
        /// </param>
        /// <param name="withStop">The BYSCORE takes effect only when specified. Whether to include a stop score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含结束分数, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRange<TArg>(ZRangeBy by, string key, TArg start, TArg stop, ulong offset, ulong count, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#else
        public string[] ZRange<TArg>(ZRangeBy by, string key, TArg start, TArg stop, ulong offset, ulong count, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRange(key, start, withStart, stop, withStop, by, rev, offset, count, false), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>根据元素或排序分数返回指定范围内的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TArg">TArg</typeparam>
        /// <param name="by">BYSCORE | BYLEX
        /// <para>根据排序分或元素排序</para>
        /// </param>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start</param>
        /// <param name="stop">stop</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="rev">The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="withStart">The BYSCORE takes effect only when specified. Whether to include a start score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含开始分数, 默认包含</para>
        /// </param>
        /// <param name="withStop">The BYSCORE takes effect only when specified. Whether to include a stop score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含结束分数, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRangeBytes<TArg>(ZRangeBy by, string key, TArg start, TArg stop, ulong offset, ulong count, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRangeBytes<TArg>(ZRangeBy by, string key, TArg start, TArg stop, ulong offset, ulong count, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRange(key, start, withStart, stop, withStop, by, rev, offset, count, false), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>根据元素或排序分数返回指定范围内的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TArg">TArg</typeparam>
        /// <param name="by">BYSCORE | BYLEX
        /// <para>根据排序分或元素排序</para>
        /// </param>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start</param>
        /// <param name="stop">stop</param>
        /// <param name="rev">The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="withStart">The BYSCORE takes effect only when specified. Whether to include a start score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含开始分数, 默认包含</para>
        /// </param>
        /// <param name="withStop">The BYSCORE takes effect only when specified. Whether to include a stop score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含结束分数, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZRangeWithScores<TArg>(ZRangeBy by, string key, TArg start, TArg stop, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZRangeWithScores<TArg>(ZRangeBy by, string key, TArg start, TArg stop, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZRange(key, start, withStart, stop, withStop, by, rev, null, null, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>根据元素或排序分数返回指定范围内的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TArg">TArg</typeparam>
        /// <param name="by">BYSCORE | BYLEX
        /// <para>根据排序分或元素排序</para>
        /// </param>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start</param>
        /// <param name="stop">stop</param>
        /// <param name="rev">The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="withStart">The BYSCORE takes effect only when specified. Whether to include a start score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含开始分数, 默认包含</para>
        /// </param>
        /// <param name="withStop">The BYSCORE takes effect only when specified. Whether to include a stop score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含结束分数, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZRangeWithScoresBytes<TArg>(ZRangeBy by, string key, TArg start, TArg stop, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZRangeWithScoresBytes<TArg>(ZRangeBy by, string key, TArg start, TArg stop, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZRange(key, start, withStart, stop, withStop, by, rev, null, null, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>根据元素或排序分数返回指定范围内的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TArg">TArg</typeparam>
        /// <param name="by">BYSCORE | BYLEX
        /// <para>根据排序分或元素排序</para>
        /// </param>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start</param>
        /// <param name="stop">stop</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="rev">The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="withStart">The BYSCORE takes effect only when specified. Whether to include a start score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含开始分数, 默认包含</para>
        /// </param>
        /// <param name="withStop">The BYSCORE takes effect only when specified. Whether to include a stop score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含结束分数, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZRangeWithScores<TArg>(ZRangeBy by, string key, TArg start, TArg stop, ulong offset, ulong count, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZRangeWithScores<TArg>(ZRangeBy by, string key, TArg start, TArg stop, ulong offset, ulong count, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZRange(key, start, withStart, stop, withStop, by, rev, offset, count, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>根据元素或排序分数返回指定范围内的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TArg">TArg</typeparam>
        /// <param name="by">BYSCORE | BYLEX
        /// <para>根据排序分或元素排序</para>
        /// </param>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start</param>
        /// <param name="stop">stop</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="rev">The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="withStart">The BYSCORE takes effect only when specified. Whether to include a start score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含开始分数, 默认包含</para>
        /// </param>
        /// <param name="withStop">The BYSCORE takes effect only when specified. Whether to include a stop score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含结束分数, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZRangeWithScoresBytes<TArg>(ZRangeBy by, string key, TArg start, TArg stop, ulong offset, ulong count, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZRangeWithScoresBytes<TArg>(ZRangeBy by, string key, TArg start, TArg stop, ulong offset, ulong count, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZRange(key, start, withStart, stop, withStop, by, rev, offset, count, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between min and max
        /// <para>Available since: 2.8.9</para>
        /// <para>当所有元素在插入排序集合时使用相同的分数，为了强制使用字典序排列，此命令返回在键为key的排序集合中，值介于min和max之间的所有元素</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRangeByLex(string key, string min, string max, CancellationToken cancellationToken = default)
#else
        public string[] ZRangeByLex(string key, string min, string max, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRangeByLex(key, min, max, null, null), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between min and max
        /// <para>Available since: 2.8.9</para>
        /// <para>当所有元素在插入排序集合时使用相同的分数，为了强制使用字典序排列，此命令返回在键为key的排序集合中，值介于min和max之间的所有元素</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRangeByLex(string key, string min, string max, ulong offset, ulong count, CancellationToken cancellationToken = default)
#else
        public string[] ZRangeByLex(string key, string min, string max, ulong offset, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRangeByLex(key, min, max, offset, count), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between min and max
        /// <para>Available since: 2.8.9</para>
        /// <para>当所有元素在插入排序集合时使用相同的分数，为了强制使用字典序排列，此命令返回在键为key的排序集合中，值介于min和max之间的所有元素</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRangeByLexBytes(string key, string min, string max, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRangeByLexBytes(string key, string min, string max, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRangeByLex(key, min, max, null, null), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between min and max
        /// <para>Available since: 2.8.9</para>
        /// <para>当所有元素在插入排序集合时使用相同的分数，为了强制使用字典序排列，此命令返回在键为key的排序集合中，值介于min和max之间的所有元素</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRangeByLexBytes(string key, string min, string max, ulong offset, ulong count, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRangeByLexBytes(string key, string min, string max, ulong offset, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRangeByLex(key, min, max, offset, count), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between min and max. The elements are considered to be ordered from low to high scores
        /// <para>Available since: 1.0.5</para>
        /// <para>获得指定排序分数范围内的元素, 升序排序方式(低到高)</para>
        /// <para>支持此命令的Redis版本, 1.0.5+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRangeByScore<TScore>(string key, TScore min, TScore max, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#else
        public string[] ZRangeByScore<TScore>(string key, TScore min, TScore max, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRangeByScore(key, min, withMin, max, withMax, null, null, false), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between min and max. The elements are considered to be ordered from low to high scores
        /// <para>Available since: 1.0.5</para>
        /// <para>获得指定排序分数范围内的元素, 升序排序方式(低到高)</para>
        /// <para>支持此命令的Redis版本, 1.0.5+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRangeByScore<TScore>(string key, TScore min, TScore max, ulong offset, ulong count, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#else
        public string[] ZRangeByScore<TScore>(string key, TScore min, TScore max, ulong offset, ulong count, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRangeByScore(key, min, withMin, max, withMax, offset, count, false), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between min and max. The elements are considered to be ordered from low to high scores
        /// <para>Available since: 1.0.5</para>
        /// <para>获得指定排序分数范围内的元素, 升序排序方式(低到高)</para>
        /// <para>支持此命令的Redis版本, 1.0.5+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRangeByScoreBytes<TScore>(string key, TScore min, TScore max, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRangeByScoreBytes<TScore>(string key, TScore min, TScore max, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRangeByScore(key, min, withMin, max, withMax, null, null, false), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between min and max. The elements are considered to be ordered from low to high scores
        /// <para>Available since: 1.0.5</para>
        /// <para>获得指定排序分数范围内的元素, 升序排序方式(低到高)</para>
        /// <para>支持此命令的Redis版本, 1.0.5+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRangeByScoreBytes<TScore>(string key, TScore min, TScore max, ulong offset, ulong count, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRangeByScoreBytes<TScore>(string key, TScore min, TScore max, ulong offset, ulong count, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRangeByScore(key, min, withMin, max, withMax, offset, count, false), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between min and max. The elements are considered to be ordered from low to high scores
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定排序分数范围内的元素, 升序排序方式(低到高)</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZRangeByScoreWithScores<TScore>(string key, TScore min, TScore max, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZRangeByScoreWithScores<TScore>(string key, TScore min, TScore max, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZRangeByScore(key, min, withMin, max, withMax, null, null, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between min and max. The elements are considered to be ordered from low to high scores
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定排序分数范围内的元素, 升序排序方式(低到高)</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZRangeByScoreWithScores<TScore>(string key, TScore min, TScore max, ulong offset, ulong count, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZRangeByScoreWithScores<TScore>(string key, TScore min, TScore max, ulong offset, ulong count, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZRangeByScore(key, min, withMin, max, withMax, offset, count, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between min and max. The elements are considered to be ordered from low to high scores
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定排序分数范围内的元素, 升序排序方式(低到高)</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZRangeByScoreWithScoresBytes<TScore>(string key, TScore min, TScore max, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZRangeByScoreWithScoresBytes<TScore>(string key, TScore min, TScore max, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZRangeByScore(key, min, withMin, max, withMax, null, null, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between min and max. The elements are considered to be ordered from low to high scores
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定排序分数范围内的元素, 升序排序方式(低到高)</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZRangeByScoreWithScoresBytes<TScore>(string key, TScore min, TScore max, ulong offset, ulong count, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZRangeByScoreWithScoresBytes<TScore>(string key, TScore min, TScore max, ulong offset, ulong count, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZRangeByScore(key, min, withMin, max, withMax, offset, count, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// This command is like ZRANGE, but stores the result in the destination destination key
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于ZRANGE ，但将结果存储在destination目标Key中</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="destination">destination<para>存储结果的key</para></param>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start index<para>开始下标, 包含此下标</para></param>
        /// <param name="stop">stop index<para>结束下标, 包含此下标</para></param>
        /// <param name="rev">The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting sorted set
        /// <para>存入destination中的结果元素个数</para>
        /// </returns>
        public long ZRangeStore(string destination, string key, long start, long stop, bool rev = false, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZRangeStore(destination, key, start, false, stop, false, null, rev, null, null), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is like ZRANGE, but stores the result in the destination destination key
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于ZRANGE ，但将结果存储在destination目标Key中</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="by">BYSCORE | BYLEX
        /// <para>根据排序分或元素排序</para>
        /// </param>
        /// <param name="destination">destination<para>存储结果的key</para></param>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start</param>
        /// <param name="stop">stop</param>
        /// <param name="rev">The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="withStart">The BYSCORE takes effect only when specified. Whether to include a start score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含开始分数, 默认包含</para>
        /// </param>
        /// <param name="withStop">The BYSCORE takes effect only when specified. Whether to include a stop score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含结束分数, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting sorted set
        /// <para>存入destination中的结果元素个数</para>
        /// </returns>
        public long ZRangeStore<TArg>(ZRangeBy by, string destination, string key, TArg start, TArg stop, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZRangeStore(destination, key, start, withStart, stop, withStop, by, rev, null, null), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is like ZRANGE, but stores the result in the destination destination key
        /// <para>Available since: 6.2.0</para>
        /// <para>此命令类似于ZRANGE ，但将结果存储在destination目标Key中</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="by">BYSCORE | BYLEX
        /// <para>根据排序分或元素排序</para>
        /// </param>
        /// <param name="destination">destination<para>存储结果的key</para></param>
        /// <param name="key">SortedSet Key</param>
        /// <param name="start">start</param>
        /// <param name="stop">stop</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="rev">The optional REV argument reverses the ordering, so elements are ordered from highest to lowest score
        /// <para>是否反转排序, 传true表示从高到低排序</para>
        /// </param>
        /// <param name="withStart">The BYSCORE takes effect only when specified. Whether to include a start score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含开始分数, 默认包含</para>
        /// </param>
        /// <param name="withStop">The BYSCORE takes effect only when specified. Whether to include a stop score. Include by default
        /// <para>指定为BYSCORE才有效, 是否包含结束分数, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting sorted set
        /// <para>存入destination中的结果元素个数</para>
        /// </returns>
        public long ZRangeStore<TArg>(ZRangeBy by, string destination, string key, TArg start, TArg stop, ulong offset, ulong count, bool rev = false, bool withStart = true, bool withStop = true, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZRangeStore(destination, key, start, withStart, stop, withStop, by, rev, offset, count), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, with the scores ordered from low to high. The rank (or index) is 0-based, which means that the member with the lowest score has rank 0
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定元素的排名. 排序分从低到高的方式的排名. 排名从0开始</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member<para>元素</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public long? ZRank(string key, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<long>(SortedSetCommands.ZRank(key, member, false), ResultType.Int64 | ResultType.Nullable, cancellationToken);
        }

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, with the scores ordered from low to high. The rank (or index) is 0-based, which means that the member with the lowest score has rank 0
        /// <para>Available since: 7.2.0</para>
        /// <para>获得指定元素的排名. 排序分从低到高的方式的排名. 排名从0开始</para>
        /// <para>支持此命令的Redis版本, 7.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member<para>元素</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public ScoreRankValue? ZRankWithScore(string key, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<ScoreRankValue>(SortedSetCommands.ZRank(key, member, true), ResultType.ScoreRankValue | ResultType.Nullable, cancellationToken);
        }

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, with the scores ordered from low to high. The rank (or index) is 0-based, which means that the member with the lowest score has rank 0
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定元素的排名. 排序分从低到高的方式的排名. 排名从0开始</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member<para>元素</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public long? ZRank(string key, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<long>(SortedSetCommands.ZRank(key, member, false), ResultType.Int64 | ResultType.Nullable, cancellationToken);
        }

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, with the scores ordered from low to high. The rank (or index) is 0-based, which means that the member with the lowest score has rank 0
        /// <para>Available since: 7.2.0</para>
        /// <para>获得指定元素的排名. 排序分从低到高的方式的排名. 排名从0开始</para>
        /// <para>支持此命令的Redis版本, 7.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member<para>元素</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public ScoreRankValue? ZRankWithScore(string key, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<ScoreRankValue>(SortedSetCommands.ZRank(key, member, true), ResultType.ScoreRankValue | ResultType.Nullable, cancellationToken);
        }

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored
        /// <para>Available since: 1.2.0</para>
        /// <para>删除一个指定的元素</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member<para>要删除的元素</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool ZRem(string key, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(SortedSetCommands.ZRem(key, member), "1", cancellationToken);
        }

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored
        /// <para>Available since: 1.2.0</para>
        /// <para>删除一个指定的元素</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member<para>要删除的元素</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool ZRem(string key, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(SortedSetCommands.ZRem<byte[]>(key, member), "1", cancellationToken);
        }

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored
        /// <para>Available since: 2.4.0</para>
        /// <para>删除多个指定的元素</para>
        /// <para>支持此命令的Redis版本, 2.4.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members removed from the sorted set, not including non-existing members
        /// <para>成功删除的元素数量, 不包含不存在的元素</para>
        /// </returns>
        public long ZRem(string key, string[] members, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZRem(key, members), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored
        /// <para>Available since: 2.4.0</para>
        /// <para>删除多个指定的元素</para>
        /// <para>支持此命令的Redis版本, 2.4.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members removed from the sorted set, not including non-existing members
        /// <para>成功删除的元素数量, 不包含不存在的元素</para>
        /// </returns>
        public long ZRem(string key, byte[][] members, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZRem(key, members), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command removes all elements in the sorted set stored at key between the lexicographical range specified by min and max
        /// <para>Available since: 2.8.9</para>
        /// <para>当所有元素排序分都一样时, 将元素值按照字典排序, 然后删除指定区间的所有元素</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members removed
        /// <para>成功删除的元素数量</para>
        /// </returns>
        public long ZRemRangeByLex(string key, string min, string max, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZRemRangeByLex(key, min, max), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start and stop. Both start and stop are 0 -based indexes with 0 being the element with the lowest score
        /// <para>Available since: 2.0.0</para>
        /// <para>删除指定下标区间的所有元素. 按照排序分从低到高的方式排序得到的下标进行删除的. 0表示最低分元素</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="start">These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score. For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth
        /// <para>开始索引, 可以是负数, 如-1表示最高分, -2表示第二高分. 也可以理解为索引, -1是最后一个, -2是倒数第二个</para>
        /// </param>
        /// <param name="stop">These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score. For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth
        /// <para>结束索引, 可以是负数, 如-1表示最高分, -2表示第二高分. 也可以理解为索引, -1是最后一个, -2是倒数第二个</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members removed
        /// <para>成功删除的元素个数</para>
        /// </returns>
        public long ZRemRangeByRank(string key, long start, long stop, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZRemRangeByRank(key, start, stop), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max
        /// <para>Available since: 1.2.0</para>
        /// <para>删除指定排序分区间的所有元素</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Score</typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <param name="withMax">
        /// Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="withMin">
        /// Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members removed
        /// <para>删除的元素数量</para>
        /// </returns>
        public long ZRemRangeByScore<TScore>(string key, TScore min, TScore max, bool withMin = true, bool withMax = true, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZRemRangeByScore(key, min, withMin, max, withMax), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. The elements are considered to be ordered from the highest to the lowest score. Descending lexicographical order is used for elements with equal score
        /// <para>Available since: 1.2.0</para>
        /// <para>排序分从高到低(倒序)排序, 并返回指定下标区间的元素</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="start">start<para>开始下标, 包含此下标</para></param>
        /// <param name="stop">stop<para>结束下标, 包含此下标</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRevRange(string key, long start, long stop, CancellationToken cancellationToken = default)
#else
        public string[] ZRevRange(string key, long start, long stop, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRevRange(key, start, stop, false), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. The elements are considered to be ordered from the highest to the lowest score. Descending lexicographical order is used for elements with equal score
        /// <para>Available since: 1.2.0</para>
        /// <para>排序分从高到低(倒序)排序, 并返回指定下标区间的元素</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="start">start<para>开始下标, 包含此下标</para></param>
        /// <param name="stop">stop<para>结束下标, 包含此下标</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRevRangeBytes(string key, long start, long stop, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRevRangeBytes(string key, long start, long stop, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRevRange(key, start, stop, false), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. The elements are considered to be ordered from the highest to the lowest score. Descending lexicographical order is used for elements with equal score
        /// <para>Available since: 1.2.0</para>
        /// <para>排序分从高到低(倒序)排序, 并返回指定下标区间的元素, 包含排序分返回</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="start">start<para>开始下标, 包含此下标</para></param>
        /// <param name="stop">stop<para>结束下标, 包含此下标</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZRevRangeWithScores(string key, long start, long stop, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZRevRangeWithScores(string key, long start, long stop, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZRevRange(key, start, stop, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. The elements are considered to be ordered from the highest to the lowest score. Descending lexicographical order is used for elements with equal score
        /// <para>Available since: 1.2.0</para>
        /// <para>排序分从高到低(倒序)排序, 并返回指定下标区间的元素, 包含排序分返回</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="start">start<para>开始下标, 包含此下标</para></param>
        /// <param name="stop">stop<para>结束下标, 包含此下标</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZRevRangeWithScoresBytes(string key, long start, long stop, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZRevRangeWithScoresBytes(string key, long start, long stop, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZRevRange(key, start, stop, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between max and min
        /// <para>Available since: 2.8.9</para>
        /// <para>当排序集中的所有元素都以相同的分数插入时，为了强制按字典顺序排序，此命令返回排序集中key处的所有元素，其值介于max和min之间</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>List of the elements in the specified score range
        /// <para>指定范围内的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRevRangeByLex(string key, string max, string min, CancellationToken cancellationToken = default)
#else
        public string[] ZRevRangeByLex(string key, string max, string min, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRevRangeByLex(key, max, min), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between max and min
        /// <para>Available since: 2.8.9</para>
        /// <para>当排序集中的所有元素都以相同的分数插入时，为了强制按字典顺序排序，此命令返回排序集中key处的所有元素，其值介于max和min之间</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>List of the elements in the specified score range
        /// <para>指定范围内的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRevRangeByLex(string key, string max, string min, ulong offset, ulong count, CancellationToken cancellationToken = default)
#else
        public string[] ZRevRangeByLex(string key, string max, string min, ulong offset, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRevRangeByLex(key, max, min, offset, count), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between max and min
        /// <para>Available since: 2.8.9</para>
        /// <para>当排序集中的所有元素都以相同的分数插入时，为了强制按字典顺序排序，此命令返回排序集中key处的所有元素，其值介于max和min之间</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>List of the elements in the specified score range
        /// <para>指定范围内的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRevRangeByLexBytes(string key, string max, string min, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRevRangeByLexBytes(string key, string max, string min, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRevRangeByLex(key, max, min), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between max and min
        /// <para>Available since: 2.8.9</para>
        /// <para>当排序集中的所有元素都以相同的分数插入时，为了强制按字典顺序排序，此命令返回排序集中key处的所有元素，其值介于max和min之间</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>List of the elements in the specified score range
        /// <para>指定范围内的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRevRangeByLexBytes(string key, string max, string min, ulong offset, ulong count, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRevRangeByLexBytes(string key, string max, string min, ulong offset, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRevRangeByLex(key, max, min, offset, count), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between max and min (including elements with score equal to max or min). In contrary to the default ordering of sorted sets, for this command the elements are considered to be ordered from high to low scores
        /// <para>Available since: 2.2.0</para>
        /// <para>获得指定排序分数范围内的元素, 降序排序方式(高到低)</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRevRangeByScore<TScore>(string key, TScore max, TScore min, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#else
        public string[] ZRevRangeByScore<TScore>(string key, TScore max, TScore min, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRevRangeByScore(key, max, withMax, min, withMin, false), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between max and min (including elements with score equal to max or min). In contrary to the default ordering of sorted sets, for this command the elements are considered to be ordered from high to low scores
        /// <para>Available since: 2.2.0</para>
        /// <para>获得指定排序分数范围内的元素, 降序排序方式(高到低)</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZRevRangeByScore<TScore>(string key, TScore max, TScore min, ulong offset, ulong count, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#else
        public string[] ZRevRangeByScore<TScore>(string key, TScore max, TScore min, ulong offset, ulong count, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZRevRangeByScore(key, max, withMax, min, withMin, false, offset, count), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between max and min (including elements with score equal to max or min). In contrary to the default ordering of sorted sets, for this command the elements are considered to be ordered from high to low scores
        /// <para>Available since: 2.2.0</para>
        /// <para>获得指定排序分数范围内的元素, 降序排序方式(高到低)</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRevRangeByScoreBytes<TScore>(string key, TScore max, TScore min, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRevRangeByScoreBytes<TScore>(string key, TScore max, TScore min, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRevRangeByScore(key, max, withMax, min, withMin, false), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between max and min (including elements with score equal to max or min). In contrary to the default ordering of sorted sets, for this command the elements are considered to be ordered from high to low scores
        /// <para>Available since: 2.2.0</para>
        /// <para>获得指定排序分数范围内的元素, 降序排序方式(高到低)</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZRevRangeByScoreBytes<TScore>(string key, TScore max, TScore min, ulong offset, ulong count, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#else
        public byte[][] ZRevRangeByScoreBytes<TScore>(string key, TScore max, TScore min, ulong offset, ulong count, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZRevRangeByScore(key, max, withMax, min, withMin, false, offset, count), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between max and min (including elements with score equal to max or min). In contrary to the default ordering of sorted sets, for this command the elements are considered to be ordered from high to low scores
        /// <para>Available since: 2.2.0</para>
        /// <para>获得指定排序分数范围内的元素, 降序排序方式(高到低)</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZRevRangeByScoreWithScores<TScore>(string key, TScore max, TScore min, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZRevRangeByScoreWithScores<TScore>(string key, TScore max, TScore min, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZRevRangeByScore(key, max, withMax, min, withMin, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between max and min (including elements with score equal to max or min). In contrary to the default ordering of sorted sets, for this command the elements are considered to be ordered from high to low scores
        /// <para>Available since: 2.2.0</para>
        /// <para>获得指定排序分数范围内的元素, 降序排序方式(高到低)</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZRevRangeByScoreWithScores<TScore>(string key, TScore max, TScore min, ulong offset, ulong count, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZRevRangeByScoreWithScores<TScore>(string key, TScore max, TScore min, ulong offset, ulong count, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZRevRangeByScore(key, max, withMax, min, withMin, true, offset, count), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between max and min (including elements with score equal to max or min). In contrary to the default ordering of sorted sets, for this command the elements are considered to be ordered from high to low scores
        /// <para>Available since: 2.2.0</para>
        /// <para>获得指定排序分数范围内的元素, 降序排序方式(高到低)</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZRevRangeByScoreWithScoresBytes<TScore>(string key, TScore max, TScore min, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZRevRangeByScoreWithScoresBytes<TScore>(string key, TScore max, TScore min, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZRevRangeByScore(key, max, withMax, min, withMin, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Returns all the elements in the sorted set at key with a score between max and min (including elements with score equal to max or min). In contrary to the default ordering of sorted sets, for this command the elements are considered to be ordered from high to low scores
        /// <para>Available since: 2.2.0</para>
        /// <para>获得指定排序分数范围内的元素, 降序排序方式(高到低)</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <typeparam name="TScore">Sort scores generic, only numeric types are supported, otherwise it will be abnormal
        /// <para>排序分数泛型, 只支持数字类型, 否则会异常</para>
        /// </typeparam>
        /// <param name="key">SortedSet key</param>
        /// <param name="max">max</param>
        /// <param name="min">min</param>
        /// <param name="offset">offset<para>偏移量</para></param>
        /// <param name="count">count<para>返回个数</para></param>
        /// <param name="withMax">Whether the value of max is included. By default included
        /// <para>是否包含max自身值, 默认包含</para>
        /// </param>
        /// <param name="withMin">Whether the value of min is included. By default included
        /// <para>是否包含min自身值, 默认包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZRevRangeByScoreWithScoresBytes<TScore>(string key, TScore max, TScore min, ulong offset, ulong count, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZRevRangeByScoreWithScoresBytes<TScore>(string key, TScore max, TScore min, ulong offset, ulong count, bool withMax = true, bool withMin = true, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZRevRangeByScore(key, max, withMax, min, withMin, true, offset, count), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, with the scores ordered from high to low. The rank (or index) is 0-based, which means that the member with the highest score has rank 0
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定元素的排名. 排序分从高到低的方式的排名. 排名从0开始</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member<para>元素</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public long? ZRevRank(string key, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<long>(SortedSetCommands.ZRevRank(key, member, false), ResultType.Int64 | ResultType.Nullable, cancellationToken);
        }

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, with the scores ordered from high to low. The rank (or index) is 0-based, which means that the member with the highest score has rank 0
        /// <para>Available since: 7.2.0</para>
        /// <para>获得指定元素的排名. 排序分从高到低的方式的排名. 排名从0开始</para>
        /// <para>支持此命令的Redis版本, 7.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member<para>元素</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public ScoreRankValue? ZRevRankWithScore(string key, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<ScoreRankValue>(SortedSetCommands.ZRevRank(key, member, true), ResultType.ScoreRankValue | ResultType.Nullable, cancellationToken);
        }

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, with the scores ordered from high to low. The rank (or index) is 0-based, which means that the member with the highest score has rank 0
        /// <para>Available since: 2.0.0</para>
        /// <para>获得指定元素的排名. 排序分从高到低的方式的排名. 排名从0开始</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member<para>元素</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public long? ZRevRank(string key, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<long>(SortedSetCommands.ZRevRank(key, member, false), ResultType.Int64 | ResultType.Nullable, cancellationToken);
        }

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, with the scores ordered from high to low. The rank (or index) is 0-based, which means that the member with the highest score has rank 0
        /// <para>Available since: 7.2.0</para>
        /// <para>获得指定元素的排名. 排序分从高到低的方式的排名. 排名从0开始</para>
        /// <para>支持此命令的Redis版本, 7.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member<para>元素</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public ScoreRankValue? ZRevRankWithScore(string key, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<ScoreRankValue>(SortedSetCommands.ZRevRank(key, member, true), ResultType.ScoreRankValue | ResultType.Nullable, cancellationToken);
        }

        /// <summary>
        /// Returns the score of member in the sorted set at key
        /// <para>Available since: 1.2.0</para>
        /// <para>获得指定元素的排序分数</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Score
        /// <para>排序分</para>
        /// </returns>
        public NumberValue ZScore(string key, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber(SortedSetCommands.ZScore(key, member), cancellationToken);
        }

        /// <summary>
        /// Returns the score of member in the sorted set at key
        /// <para>Available since: 1.2.0</para>
        /// <para>获得指定元素的排序分数</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="key">SortedSet Key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Score
        /// <para>排序分</para>
        /// </returns>
        public NumberValue ZScore(string key, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber(SortedSetCommands.ZScore(key, member), cancellationToken);
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的交集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members
        /// <para>并集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZUnion(string key1, string key2, CancellationToken cancellationToken = default)
#else
        public string[] ZUnion(string key1, string key2, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZUnion(Aggregate.Sum, false, key1, key2), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members
        /// <para>并集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZUnion(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public string[] ZUnion(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArray<string>(SortedSetCommands.ZUnion(Aggregate.Sum, false, [key, .. keys]), ResultType.Array | ResultType.String, cancellationToken)!;
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallClassArray<string>(SortedSetCommands.ZUnion(Aggregate.Sum, false, liskKeys.ToArray()), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members
        /// <para>并集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? ZUnion(string[] keys, CancellationToken cancellationToken = default)
#else
        public string[] ZUnion(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(SortedSetCommands.ZUnion(Aggregate.Sum, false, keys), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members
        /// <para>并集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZUnionBytes(string key1, string key2, CancellationToken cancellationToken = default)
#else
        public byte[][] ZUnionBytes(string key1, string key2, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZUnion(Aggregate.Sum, false, key1, key2), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members
        /// <para>并集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZUnionBytes(string key, string[] keys, CancellationToken cancellationToken = default)
#else
        public byte[][] ZUnionBytes(string key, string[] keys, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZUnion(Aggregate.Sum, false, [key, .. keys]), ResultType.Array | ResultType.Bytes, cancellationToken)!;
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZUnion(Aggregate.Sum, false, liskKeys.ToArray()), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members
        /// <para>并集元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? ZUnionBytes(string[] keys, CancellationToken cancellationToken = default)
#else
        public byte[][] ZUnionBytes(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(SortedSetCommands.ZUnion(Aggregate.Sum, false, keys), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZUnionWithScores(string key1, string key2, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZUnionWithScores(string key1, string key2, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZUnion(aggregate, true, key1, key2), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZUnionWithScores(string key, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZUnionWithScores(string key, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZUnion(aggregate, true, [key, .. keys]), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZUnion(aggregate, true, liskKeys.ToArray()), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZUnionWithScores(string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZUnionWithScores(string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZUnion(aggregate, true, keys), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZUnionWithScoresBytes(string key1, string key2, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZUnionWithScoresBytes(string key1, string key2, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZUnion(aggregate, true, key1, key2), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZUnionWithScoresBytes(string key, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZUnionWithScoresBytes(string key, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZUnion(aggregate, true, [key, .. keys]), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZUnion(aggregate, true, liskKeys.ToArray()), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZUnionWithScoresBytes(string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZUnionWithScoresBytes(string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZUnion(aggregate, true, keys), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key1Weight">Key1 multiplication factor<para>Key1排序分数要乘的数</para></param>
        /// <param name="key2">key2</param>
        /// <param name="key2Weight">Key2 multiplication factor<para>Key2排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZUnionWithScores<TWeight>(string key1, TWeight key1Weight, string key2, TWeight key2Weight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZUnionWithScores<TWeight>(string key1, TWeight key1Weight, string key2, TWeight key2Weight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZUnion([key1, key2], [key1Weight, key2Weight], aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#else
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZUnion(new string[] { key1, key2 }, new TWeight[] { key1Weight, key2Weight }, aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keyWeight">Key multiplication factor<para>Key排序分数要乘的数</para></param>
        /// <param name="keys">keys</param>
        /// <param name="keysWeight">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZUnionWithScores<TWeight>(string key, TWeight keyWeight, string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZUnionWithScores<TWeight>(string key, TWeight keyWeight, string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZUnion([key, .. keys], [keyWeight, .. keysWeight], aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            var liskWeights = new List<TWeight>(keys.Length + 1) { keyWeight };
            liskWeights.AddRange(keysWeight);
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZUnion(liskKeys.ToArray(), liskWeights.ToArray(), aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="keysWeight">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<string>[]? ZUnionWithScores<TWeight>(string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<string>[] ZUnionWithScores<TWeight>(string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<string>>(SortedSetCommands.ZUnion(keys, keysWeight, aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key1Weight">Key1 multiplication factor<para>Key1排序分数要乘的数</para></param>
        /// <param name="key2">key2</param>
        /// <param name="key2Weight">Key2 multiplication factor<para>Key2排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZUnionWithScoresBytes<TWeight>(string key1, TWeight key1Weight, string key2, TWeight key2Weight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZUnionWithScoresBytes<TWeight>(string key1, TWeight key1Weight, string key2, TWeight key2Weight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZUnion([key1, key2], [key1Weight, key2Weight], aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#else
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZUnion(new string[] { key1, key2 }, new TWeight[] { key1Weight, key2Weight }, aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="keyWeight">Key multiplication factor<para>Key排序分数要乘的数</para></param>
        /// <param name="keys">keys</param>
        /// <param name="keysWeight">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZUnionWithScoresBytes<TWeight>(string key, TWeight keyWeight, string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZUnionWithScoresBytes<TWeight>(string key, TWeight keyWeight, string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZUnion([key, .. keys], [keyWeight, .. keysWeight], aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            var liskWeights = new List<TWeight>(keys.Length + 1) { keyWeight };
            liskWeights.AddRange(keysWeight);
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZUnion(liskKeys.ToArray(), liskWeights.ToArray(), aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys
        /// <para>Available since: 6.2.0</para>
        /// <para>计算给出Key的并集</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="keysWeight">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Union members with scores
        /// <para>包含排序分数的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public MemberScoreValue<byte[]>[]? ZUnionWithScoresBytes<TWeight>(string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#else
        public MemberScoreValue<byte[]>[] ZUnionWithScoresBytes<TWeight>(string[] keys, TWeight[] keysWeight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            return base._call.CallStructArray<MemberScoreValue<byte[]>>(SortedSetCommands.ZUnion(keys, keysWeight, aggregate, true), ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的并集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <typeparam name="TWeight">TWeight</typeparam>
        /// <param name="destination">Save the key that combines the results
        /// <para>保存并集结果的key</para>
        /// </param>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="key1Weight">Key1 multiplication factor<para>Key1排序分数要乘的数</para></param>
        /// <param name="key2Weight">Key2 multiplication factor<para>Key2排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting sorted set
        /// <para>存入destination中的并集元素个数</para>
        /// </returns>
        public long ZUnionStore<TWeight>(string destination, string key1, TWeight key1Weight, string key2, TWeight key2Weight, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            var weights = new TWeight[] { key1Weight, key2Weight };
#if NET8_0_OR_GREATER
            return base._call.CallNumber<long>(SortedSetCommands.ZUnionStore(destination, [key1, key2], weights, aggregate), ResultType.Int64, cancellationToken);
#else
            return base._call.CallNumber<long>(SortedSetCommands.ZUnionStore(destination, new string[] { key1, key2 }, weights, aggregate), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的并集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="destination">Save the key that combines the results
        /// <para>保存并集结果的key</para>
        /// </param>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting sorted set
        /// <para>存入destination中的并集元素个数</para>
        /// </returns>
        public long ZUnionStore(string destination, string key1, string key2, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumber<long>(SortedSetCommands.ZUnionStore<int>(destination, [key1, key2], null, aggregate), ResultType.Int64, cancellationToken);
#else
            return base._call.CallNumber<long>(SortedSetCommands.ZUnionStore<int>(destination, new string[] { key1, key2 }, null, aggregate), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的并集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <typeparam name="TWeight">TWeight</typeparam>
        /// <param name="destination">Save the key that combines the results
        /// <para>保存并集结果的key</para>
        /// </param>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="keyWeight">Key multiplication factor<para>Key排序分数要乘的数</para></param>
        /// <param name="keysWeights">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting sorted set
        /// <para>存入destination中的并集元素个数</para>
        /// </returns>
        public long ZUnionStore<TWeight>(string destination, string key, TWeight keyWeight, string[] keys, TWeight[] keysWeights, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumber<long>(SortedSetCommands.ZUnionStore(destination, [key, .. keys], [keyWeight, .. keysWeights], aggregate), ResultType.Int64, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            var weights = new List<TWeight>(keys.Length + 1) { keyWeight };
            weights.AddRange(keysWeights);
            return base._call.CallNumber<long>(SortedSetCommands.ZUnionStore(destination, liskKeys.ToArray(), weights.ToArray(), aggregate), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的并集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="destination">Save the key that combines the results
        /// <para>保存并集结果的key</para>
        /// </param>
        /// <param name="key">key</param>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting sorted set
        /// <para>存入destination中的并集元素个数</para>
        /// </returns>
        public long ZUnionStore(string destination, string key, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumber<long>(SortedSetCommands.ZUnionStore<int>(destination, [key, .. keys], null, aggregate), ResultType.Int64, cancellationToken);
#else
            var liskKeys = new List<string>(keys.Length + 1) { key };
            liskKeys.AddRange(keys);
            return base._call.CallNumber<long>(SortedSetCommands.ZUnionStore<int>(destination, liskKeys.ToArray(), null, aggregate), ResultType.Int64, cancellationToken);
#endif
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的并集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <typeparam name="TWeight">TWeight</typeparam>
        /// <param name="destination">Save the key that combines the results
        /// <para>保存并集结果的key</para>
        /// </param>
        /// <param name="keys">keys</param>
        /// <param name="weights">Keys multiplication factor<para>Keys排序分数要乘的数</para></param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting sorted set
        /// <para>存入destination中的并集元素个数</para>
        /// </returns>
        public long ZUnionStore<TWeight>(string destination, string[] keys, TWeight[] weights, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZUnionStore(destination, keys, weights, aggregate), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Computes the union of numkeys sorted sets given by the specified keys, and stores the result in destination
        /// <para>Available since: 2.0.0</para>
        /// <para>计算给的Key的并集, 并将结果存入指定的destination中</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="destination">Save the key that combines the results
        /// <para>保存并集结果的key</para>
        /// </param>
        /// <param name="keys">keys</param>
        /// <param name="aggregate">With the AGGREGATE option, it is possible to specify how the results of the union are aggregated. This option defaults to SUM
        /// <para>排序分数聚合方式, 默认为求和, 可以指定取最大值, 最小值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting sorted set
        /// <para>存入destination中的并集元素个数</para>
        /// </returns>
        public long ZUnionStore(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(SortedSetCommands.ZUnionStore<int>(destination, keys, null, aggregate), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Iterates sorted set
        /// <para>Available since: 2.8.0</para>
        /// <para>迭代Sorted Set</para>
        /// <para>支持此命令的Redis版本, 2.8.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="cursor">cursor</param>
        /// <param name="pattern">pattern</param>
        /// <param name="count">count</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para></param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public ScanValue<MemberScoreValue<string>[]>? ZScan(string key, long cursor, string? pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#else
        public ScanValue<MemberScoreValue<string>[]> ZScan(string key, long cursor, string pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallScan<MemberScoreValue<string>[]>(SortedSetCommands.ZScan(key, cursor, pattern, count), ResultType.Scan | ResultType.Array | ResultType.MemberScoreValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Iterates sorted set
        /// <para>Available since: 2.8.0</para>
        /// <para>迭代Sorted Set</para>
        /// <para>支持此命令的Redis版本, 2.8.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="cursor">cursor</param>
        /// <param name="pattern">pattern</param>
        /// <param name="count">count</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para></param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public ScanValue<MemberScoreValue<byte[]>[]>? ZScanBytes(string key, long cursor, string? pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#else
        public ScanValue<MemberScoreValue<byte[]>[]> ZScanBytes(string key, long cursor, string pattern = null, ulong? count = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallScan<MemberScoreValue<byte[]>[]>(SortedSetCommands.ZScan(key, cursor, pattern, count), ResultType.Scan | ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// When all sorted sets are empty, Redis will block the connection until another client adds members to one of the keys or until the timeout (a double value specifying the maximum number of seconds to block) elapses. A timeout of zero can be used to block indefinitely
        /// <para>Available since: 7.0.0</para>
        /// <para>从指定的SortedSet中弹出一个元素, 如果不存在元素, 将进行阻塞</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <param name="key">SortedSet key</param>
        /// <param name="mm">Max | Min
        /// <para>从最大排序分弹出还是从最小排序分弹出</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>>? BZMPop(double timeout, string key, MaxMin mm)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZMPop(timeout, [key], mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZMPop(timeout, new string[] { key }, mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// When all sorted sets are empty, Redis will block the connection until another client adds members to one of the keys or until the timeout (a double value specifying the maximum number of seconds to block) elapses. A timeout of zero can be used to block indefinitely
        /// <para>Available since: 7.0.0</para>
        /// <para>从指定的SortedSet中弹出一个元素, 如果不存在元素, 将进行阻塞</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="mm">Max | Min
        /// <para>从最大排序分弹出还是从最小排序分弹出</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>>? BZMPop(double timeout, string[] keys, MaxMin mm)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZMPop(timeout, keys, mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
            }
        }

        /// <summary>
        /// When all sorted sets are empty, Redis will block the connection until another client adds members to one of the keys or until the timeout (a double value specifying the maximum number of seconds to block) elapses. A timeout of zero can be used to block indefinitely
        /// <para>Available since: 7.0.0</para>
        /// <para>从指定的SortedSet中弹出一个元素, 如果不存在元素, 将进行阻塞</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <param name="key">SortedSet key</param>
        /// <param name="mm">Max | Min
        /// <para>从最大排序分弹出还是从最小排序分弹出</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? BZMPopBytes(double timeout, string key, MaxMin mm)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZMPop(timeout, [key], mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZMPop(timeout, new string[] { key }, mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// When all sorted sets are empty, Redis will block the connection until another client adds members to one of the keys or until the timeout (a double value specifying the maximum number of seconds to block) elapses. A timeout of zero can be used to block indefinitely
        /// <para>Available since: 7.0.0</para>
        /// <para>从指定的SortedSet中弹出一个元素, 如果不存在元素, 将进行阻塞</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="mm">Max | Min
        /// <para>从最大排序分弹出还是从最小排序分弹出</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? BZMPopBytes(double timeout, string[] keys, MaxMin mm)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZMPop(timeout, keys, mm, 1), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
            }
        }

        /// <summary>
        /// When all sorted sets are empty, Redis will block the connection until another client adds members to one of the keys or until the timeout (a double value specifying the maximum number of seconds to block) elapses. A timeout of zero can be used to block indefinitely
        /// <para>Available since: 7.0.0</para>
        /// <para>从指定的SortedSet中弹出一个元素, 如果不存在元素, 将进行阻塞</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <param name="key">SortedSet key</param>
        /// <param name="mm">Max | Min
        /// <para>从最大排序分弹出还是从最小排序分弹出</para>
        /// </param>
        /// <param name="count">count<para>弹出个数</para></param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>[]>? BZMPop(double timeout, string key, MaxMin mm, ulong count)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>[]>>(SortedSetCommands.BZMPop(timeout, [key], mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>[]>>(SortedSetCommands.BZMPop(timeout, new string[] { key }, mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// When all sorted sets are empty, Redis will block the connection until another client adds members to one of the keys or until the timeout (a double value specifying the maximum number of seconds to block) elapses. A timeout of zero can be used to block indefinitely
        /// <para>Available since: 7.0.0</para>
        /// <para>从指定的SortedSet中弹出一个元素, 如果不存在元素, 将进行阻塞</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="mm">Max | Min
        /// <para>从最大排序分弹出还是从最小排序分弹出</para>
        /// </param>
        /// <param name="count">count<para>弹出个数</para></param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>[]>? BZMPop(double timeout, string[] keys, MaxMin mm, ulong count)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>[]>>(SortedSetCommands.BZMPop(timeout, keys, mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
            }
        }

        /// <summary>
        /// When all sorted sets are empty, Redis will block the connection until another client adds members to one of the keys or until the timeout (a double value specifying the maximum number of seconds to block) elapses. A timeout of zero can be used to block indefinitely
        /// <para>Available since: 7.0.0</para>
        /// <para>从指定的SortedSet中弹出一个元素, 如果不存在元素, 将进行阻塞</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <param name="key">SortedSet key</param>
        /// <param name="mm">Max | Min
        /// <para>从最大排序分弹出还是从最小排序分弹出</para>
        /// </param>
        /// <param name="count">count<para>弹出个数</para></param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>[]>? BZMPopBytes(double timeout, string key, MaxMin mm, ulong count)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>[]>>(SortedSetCommands.BZMPop(timeout, [key], mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>[]>>(SortedSetCommands.BZMPop(timeout, new string[] { key }, mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// When all sorted sets are empty, Redis will block the connection until another client adds members to one of the keys or until the timeout (a double value specifying the maximum number of seconds to block) elapses. A timeout of zero can be used to block indefinitely
        /// <para>Available since: 7.0.0</para>
        /// <para>从指定的SortedSet中弹出一个元素, 如果不存在元素, 将进行阻塞</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="mm">Max | Min
        /// <para>从最大排序分弹出还是从最小排序分弹出</para>
        /// </param>
        /// <param name="count">count<para>弹出个数</para></param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>[]>? BZMPopBytes(double timeout, string[] keys, MaxMin mm, ulong count)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>[]>>(SortedSetCommands.BZMPop(timeout, keys, mm, count), ResultType.KeyValuePair | ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
            }
        }

        /// <summary>
        /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive
        /// <para>Available since: 5.0.0</para>
        /// <para>BZPOPMAX阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="timeout">A integer value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>>? BZPopMax(string key, ulong timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMax([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMax(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive
        /// <para>Available since: 5.0.0</para>
        /// <para>BZPOPMAX阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="timeout">A integer value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>>? BZPopMax(string[] keys, ulong timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMax(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
            }
        }

        /// <summary>
        /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive
        /// <para>Available since: 6.0.0</para>
        /// <para>BZPOPMAX阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>>? BZPopMax(string key, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMax([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMax(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive
        /// <para>Available since: 6.0.0</para>
        /// <para>BZPOPMAX阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>>? BZPopMax(string[] keys, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMax(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
            }
        }

        /// <summary>
        /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive
        /// <para>Available since: 5.0.0</para>
        /// <para>BZPOPMAX阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="timeout">A integer value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? BZPopMaxBytes(string key, ulong timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMax([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMax(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive
        /// <para>Available since: 5.0.0</para>
        /// <para>BZPOPMAX阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="timeout">A integer value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? BZPopMaxBytes(string[] keys, ulong timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMax(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
            }
        }

        /// <summary>
        /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive
        /// <para>Available since: 6.0.0</para>
        /// <para>BZPOPMAX阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? BZPopMaxBytes(string key, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMax([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMax(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive
        /// <para>Available since: 6.0.0</para>
        /// <para>BZPOPMAX阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? BZPopMaxBytes(string[] keys, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMax(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
            }
        }

        /// <summary>
        /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive
        /// <para>Available since: 5.0.0</para>
        /// <para>BZPOPMIN阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="timeout">A integer value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>>? BZPopMin(string key, ulong timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMin([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMin(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive
        /// <para>Available since: 5.0.0</para>
        /// <para>BZPOPMIN阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="timeout">A integer value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>>? BZPopMin(string[] keys, ulong timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMin(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
            }
        }

        /// <summary>
        /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive
        /// <para>Available since: 6.0.0</para>
        /// <para>BZPOPMIN阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>>? BZPopMin(string key, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMin([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMin(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive
        /// <para>Available since: 6.0.0</para>
        /// <para>BZPOPMIN阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<string>>? BZPopMin(string[] keys, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<string>>>(SortedSetCommands.BZPopMin(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.String, tokenSource.Token);
            }
        }

        /// <summary>
        /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive
        /// <para>Available since: 5.0.0</para>
        /// <para>BZPOPMIN阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="timeout">A integer value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? BZPopMinBytes(string key, ulong timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMin([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMin(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive
        /// <para>Available since: 5.0.0</para>
        /// <para>BZPOPMIN阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="timeout">A integer value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? BZPopMinBytes(string[] keys, ulong timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMin(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
            }
        }

        /// <summary>
        /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive
        /// <para>Available since: 6.0.0</para>
        /// <para>BZPOPMIN阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">SortedSet key</param>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? BZPopMinBytes(string key, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMin([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMin(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive
        /// <para>Available since: 6.0.0</para>
        /// <para>BZPOPMIN阻塞变体. 如果指定key没有元素, 则阻塞等待</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="keys">SortedSet keys</param>
        /// <param name="timeout">A double value specifying the maximum number of seconds to block
        /// <para>最大阻塞秒数, 0表示无限期, 不建议无期限</para>
        /// </param>
        /// <returns></returns>
        public KeyValuePair<string, MemberScoreValue<byte[]>>? BZPopMinBytes(string[] keys, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, MemberScoreValue<byte[]>>>(SortedSetCommands.BZPopMin(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.MemberScoreValue | ResultType.Bytes, tokenSource.Token);
            }
        }
    }
}
