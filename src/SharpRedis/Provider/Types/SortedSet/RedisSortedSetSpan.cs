#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using SharpRedis.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis
{
    public sealed partial class RedisSortedSet
    {
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
        public long ZAdd<TScore>(string key, ReadOnlySpan<char> member, TScore score, bool ch = false, NxXx nxx = NxXx.None, GtLt gl = GtLt.None, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return this.ZAdd(key, member.SpanToBytes(base._call.Encoding), score, ch, nxx, gl, cancellationToken);
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
        public Task<long> ZAddAsync<TScore>(string key, ReadOnlySpan<char> member, TScore score, bool ch = false, NxXx nxx = NxXx.None, GtLt gl = GtLt.None, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return this.ZAddAsync(key, member.SpanToBytes(base._call.Encoding), score, ch, nxx, gl, cancellationToken);
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
        public NumberValue ZAddIncr<TScore>(string key, ReadOnlySpan<char> member, TScore score, NxXx nxx = NxXx.None, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return this.ZAddIncr(key, member.SpanToBytes(base._call.Encoding), score, nxx, cancellationToken);
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
        public Task<NumberValue> ZAddIncrAsync<TScore>(string key, ReadOnlySpan<char> member, TScore score, NxXx nxx = NxXx.None, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            return this.ZAddIncrAsync(key, member.SpanToBytes(base._call.Encoding), score, nxx, cancellationToken);
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
        public NumberValue ZIncrBy<TIncrement>(string key, TIncrement increment, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TIncrement : struct, System.Numerics.INumber<TIncrement>
#else
            where TIncrement : struct, System.IEquatable<TIncrement>
#endif
        {
            return this.ZIncrBy(key, increment, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<NumberValue> ZIncrByAsync<TIncrement>(string key, TIncrement increment, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TIncrement : struct, System.Numerics.INumber<TIncrement>
#else
            where TIncrement : struct, System.IEquatable<TIncrement>
#endif
        {
            return this.ZIncrByAsync(key, increment, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public NumberValue ZMScore(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZMScore(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<NumberValue> ZMScoreAsync(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZMScoreAsync(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public long? ZRank(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZRank(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<long?> ZRankAsync(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZRankAsync(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public ScoreRankValue? ZRankWithScore(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZRankWithScore(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<ScoreRankValue?> ZRankWithScoreAsync(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZRankWithScoreAsync(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public bool ZRem(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZRem(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<bool> ZRemAsync(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZRemAsync(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public long? ZRevRank(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZRevRank(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<long?> ZRevRankAsync(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZRevRankAsync(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public ScoreRankValue? ZRevRankWithScore(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZRevRankWithScore(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<ScoreRankValue?> ZRevRankWithScoreAsync(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZRevRankWithScoreAsync(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public NumberValue ZScore(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZScore(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<NumberValue> ZScoreAsync(string key, ReadOnlySpan<char> member, CancellationToken cancellationToken = default)
        {
            return this.ZScoreAsync(key, member.SpanToBytes(base._call.Encoding), cancellationToken);
        }
    }
}
#endif
