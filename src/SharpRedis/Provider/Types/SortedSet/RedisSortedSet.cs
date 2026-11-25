using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis sorted sets
/// <para>Redis SortedSet类型</para>
/// <para>If you are new to sorted sets, see <see href="https://redis.io/docs/data-types/sorted-sets">Introduction to Redis sorted sets</see></para>
/// <para>如果你不太懂sorted sets类型, 请<see href="https://redis.io/docs/data-types/sorted-sets">查看sorted sets文档</see></para>
/// </summary>
public sealed partial class RedisSortedSet : RedisTypeBase
{
    #region SortedSet enums
    public enum ZAddWhen : byte
    {
        /// <summary>
        /// No specification
        /// <para>不指定</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Only add new members. Don't update already existing members.
        /// <para>仅在元素不存在时, 进行添加. 如果元素存在, 不进行任何操作</para>
        /// </summary>
        NX,

        /// <summary>
        /// Only update members that already exist. Don't add new members.
        /// <para>仅在元素存在时, 更新其分数. 如果元素不存在, 不进行任何操作</para>
        /// </summary>
        XX,
    }

    public enum ZAddScoreWhen : byte
    {
        /// <summary>
        /// No specification
        /// <para>不指定</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Only update existing members if the new score is less than the current score. This flag doesn't prevent adding new members.
        /// <para>仅在新分数小于元素当前分数时才更新元素. 如果集合不存在当前元素, 不进行任何操作</para>
        /// </summary>
        LT,

        /// <summary>
        /// Only update existing members if the new score is greater than the current score. This flag doesn't prevent adding new members.
        /// <para>仅在新分数大于元素当前分数时才更新元素. 如果集合不存在当前元素, 不进行任何操作</para>
        /// </summary>
        GT,
    }

    public enum RangeBy : byte
    {
        /// <summary>
        /// No specification
        /// <para>不指定</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Filter by score range
        /// <para>Available since: 6.2.0</para>
        /// <para>根据 score 过滤范围</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        ByScore,

        /// <summary>
        /// Filter based on the lexicographical order of the "member" string dictionary
        /// <para>Available since: 6.2.0</para>
        /// <para>根据元素字符串字典序过滤</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        ByLex,
    }

    public enum Aggregate : byte
    {
        /// <summary>
        /// Return the sum of the members scores. This is the default setting.
        /// <para>返回元素的分数和, 默认行为</para>
        /// </summary>
        Sum = 0,

        /// <summary>
        /// Return the min score
        /// <para>返回最小分数</para>
        /// </summary>
        Min,

        /// <summary>
        /// Return the max score
        /// <para>返回最大分数</para>
        /// </summary>
        Max,
    }

    public enum PopHow : byte
    {
        /// <summary>
        /// The elements popped are those with the lowest scores from the first non-empty sorted set
        /// <para>从第一个非空的集合中, 弹出分数最低的</para>
        /// </summary>
        Min = 0,

        /// <summary>
        /// The elements popped are those with the highest scores from the first non-empty sorted set
        /// <para>从第一个非空的集合中, 弹出分数最高的</para>
        /// </summary>
        Max,
    }

    /// <summary>
    /// Specifies the member selection mode for the ZMOVE command.
    /// <para>指定 ZMOVE 命令从源 Sorted Set 中选择成员进行移动的方式。</para>
    /// <para>支持 MIN、MAX 和 BYSCORE 三种模式。</para>
    /// </summary>
    public enum MoveHow : byte
    {
        /// <summary>
        /// Select members with the lowest scores.
        /// <para>选择源 Sorted Set 中分数最低的成员进行移动。</para>
        /// <para>对应 Redis 命令参数：MIN。</para>
        /// </summary>
        Min = 0,

        /// <summary>
        /// Select members with the highest scores.
        /// <para>选择源 Sorted Set 中分数最高的成员进行移动。</para>
        /// <para>对应 Redis 命令参数：MAX。</para>
        /// </summary>
        Max = 1,

        /// <summary>
        /// Select members within the specified score range.
        /// <para>选择源 Sorted Set 中分数位于指定范围内的成员进行移动。</para>
        /// <para>需要额外指定 score-min 和 score-max 参数。</para>
        /// <para>对应 Redis 命令参数：BYSCORE score-min score-max。</para>
        /// </summary>
        ByScore = 2,
    }

    public enum MoveWhen : byte
    {
        /// <summary>
        /// No specification
        /// <para>不指定</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// NX moves only members that do not exist in destination.
        /// <para>只移动目标集合中不存在的成员</para>
        /// </summary>
        NX,

        /// <summary>
        /// XX moves only members that already exist in destination.
        /// <para>只会移动目标集合中已存在的成员。</para>
        /// </summary>
        XX,
    }
    #endregion

    #region Types
    /// <summary>
    /// The <b>ZRANGE</b> command, range parameter type
    /// <para>Implicit conversions can be performed between numbers, strings and byte arrays.</para>
    /// <para><b>ZRANGE</b> 命令, 范围参数类型. 兼容对数字, 字符串, byte[]的隐式转换</para>
    /// </summary>
    /// <example>
    /// <code>
    /// RangeArg start = 1; // with 1
    /// RangeArg end = 100; // with 100
    /// RangeArg memberStart = "a"; // with a
    /// RangeArg memberEnd = "b"; // with b
    /// RangeArg without = ("a", false);// Or RangeArg with = new ValueTuple&lt;string, bool&gt;("a", false);
    /// RangeArg with = ("a", true);// Or RangeArg with = new ValueTuple&lt;string, bool&gt;("a", true);
    /// RangeArg numWithout = (1, false);// Or RangeArg with = new ValueTuple&lt;double, bool&gt;(1, false);
    /// </code>
    /// </example>
    public readonly struct RangeArg
    {
        private readonly string _value;

        internal readonly string Value => this._value;

        public RangeArg()
        {
            throw new NotSupportedException();
        }

        public RangeArg(string val, bool with)
        {
            if (val is "-" or "+")
            {
                this._value = val;
                return;
            }
            if (with)
            {
                this._value = $"[{val}";
            }
            else
            {
                this._value = $"({val}";
            }
        }

        public RangeArg(double val, bool with)
        {
            if (!with)
            {
                if (val == double.PositiveInfinity)
                {
                    this._value = "(inf";
                }
                else if (val == double.NegativeInfinity)
                {
                    this._value = "(-inf";
                }
                else
                {
                    this._value = $"({val}";
                }
            }
            else
            {
                if (val == double.PositiveInfinity)
                {
                    this._value = "inf";
                }
                else if (val == double.NegativeInfinity)
                {
                    this._value = "-inf";
                }
                else
                {
                    this._value = val.ToString();
                }
            }
        }

        public RangeArg(long val, bool with)
        {
            if (!with)
            {
                this._value = $"({val}";
            }
            else
            {
                this._value = val.ToString();
            }
        }

        public RangeArg(decimal val, bool with)
        {
            if (!with)
            {
                this._value = $"({val}";
            }
            else
            {
                this._value = val.ToString();
            }
        }

        public static implicit operator RangeArg(string member)
        {
            return new(member, true);
        }

        public static implicit operator RangeArg(double score)
        {
            return new(score, true);
        }

        public static implicit operator RangeArg(decimal score)
        {
            return new(score, true);
        }

        public static implicit operator RangeArg(long score)
        {
            return new(score, true);
        }

        public static implicit operator RangeArg((string member, bool with) val)
        {
            return new(val.member, val.with);
        }

        public static implicit operator RangeArg((double score, bool with) val)
        {
            return new(val.score, val.with);
        }

        public static implicit operator RangeArg((decimal score, bool with) val)
        {
            return new(val.score, val.with);
        }

        public static implicit operator RangeArg((long score, bool with) val)
        {
            return new(val.score, val.with);
        }
    }
    #endregion

    internal RedisSortedSet(BaseRedis redis)
       : base(redis)
    { }

    /// <summary>
    /// Adds all the specified members with the specified scores to the sorted set stored at key.
    /// <para>Available since: 1.2.0</para>
    /// <para>将元素和分数添加到指定的Key中</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key">SortedSet key</param>
    /// <param name="member">member<para>元素</para></param>
    /// <param name="score">score<para>分数</para></param>
    /// <param name="when">
    /// <b>Starting from version 3.0.2 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 3.0.2及以上版本才支持此参数</b></para>
    /// <para>NX: Only add new members. Don't update already existing members.</para>
    /// <para>XX: Only update members that already exist. Don't add new members.</para>
    /// <para>NX: 仅在元素不存在时, 进行添加. 如果元素存在, 不进行任何操作</para>
    /// <para>XX: 仅在元素存在时, 更新其分数. 如果元素不存在, 不进行任何操作</para>
    /// </param>
    /// <param name="scoreWhen">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>LT: Only update existing members if the new score is less than the current score. This flag doesn't prevent adding new members.</para>
    /// <para>GT: Only update existing members if the new score is greater than the current score. This flag doesn't prevent adding new members.</para>
    /// <para>LT: 仅在新分数小于元素当前分数时才更新元素. 如果集合不存在当前元素, 不进行任何操作</para>
    /// <para>GT: 仅在新分数大于元素当前分数时才更新元素. 如果集合不存在当前元素, 不进行任何操作</para>
    /// </param>
    /// <param name="ch">
    /// <b>Starting from version 3.0.2 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 3.0.2及以上版本才支持此参数</b></para>
    /// <para>Modify the return value from the number of new members added, to the total number of members changed</para>
    /// <para>是否将返回值从新添加元素的数量修改为更新的元素数量</para>
    /// </param>
    /// <param name="incr">
    /// <b>Starting from version 3.0.2 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 3.0.2及以上版本才支持此参数</b></para>
    /// <para>When this option is specified ZADD acts like <b>[ZINCRBY]</b>. Only one score-element pair can be specified in this mode.</para>
    /// <para>是否累加元素的分数, 如果传true, 和 <b>[ZINCRBY]</b> 命令行为一样, 且只能传入一对元素和分数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// NULL: if the operation was aborted because of a conflict with one of the XX/NX/LT/GT options.
    /// <para>Integer: the number of new members when the CH option is not used.</para>
    /// <para>Integer: the number of new or updated members when the CH option is used.</para>
    /// <para>Double: the updated score of the member when the INCR option is used.</para>
    /// <para>NULL: 如果作因与 XX/NX/LT/GT 选项冲突而中止</para>
    /// <para>Integer: 如果 CH 没有指定true, 返回新增的元素数量</para>
    /// <para>Integer: 如果 CH 指定为true, 返回更新的元素数量</para>
    /// <para>Double: 如果 INCR 指定为true, 返回累加后的元素分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZAdd">命令文档</see></para>
    /// </remarks>
    public RedisNumber ZAdd(
        SharpValue key,
        SharpValue member,
        double score,
        ZAddWhen when = ZAddWhen.None,
        ZAddScoreWhen scoreWhen = ZAddScoreWhen.None,
        bool ch = false,
        bool incr = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZAdd(
            key: key,
            member: member,
            score: score,
            when: when,
            scoreWhen: scoreWhen,
            ch: ch,
            incr: incr) | flags, cancellationToken);

    /// <summary>
    /// Adds all the specified members with the specified scores to the sorted set stored at key.
    /// <para>Available since: 1.2.0</para>
    /// <para>将元素和分数添加到指定的Key中</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key">SortedSet key</param>
    /// <param name="member">member<para>元素</para></param>
    /// <param name="score">score<para>分数</para></param>
    /// <param name="when">
    /// <b>Starting from version 3.0.2 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 3.0.2及以上版本才支持此参数</b></para>
    /// <para>NX: Only add new members. Don't update already existing members.</para>
    /// <para>XX: Only update members that already exist. Don't add new members.</para>
    /// <para>NX: 仅在元素不存在时, 进行添加. 如果元素存在, 不进行任何操作</para>
    /// <para>XX: 仅在元素存在时, 更新其分数. 如果元素不存在, 不进行任何操作</para>
    /// </param>
    /// <param name="scoreWhen">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>LT: Only update existing members if the new score is less than the current score. This flag doesn't prevent adding new members.</para>
    /// <para>GT: Only update existing members if the new score is greater than the current score. This flag doesn't prevent adding new members.</para>
    /// <para>LT: 仅在新分数小于元素当前分数时才更新元素. 如果集合不存在当前元素, 不进行任何操作</para>
    /// <para>GT: 仅在新分数大于元素当前分数时才更新元素. 如果集合不存在当前元素, 不进行任何操作</para>
    /// </param>
    /// <param name="ch">
    /// <b>Starting from version 3.0.2 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 3.0.2及以上版本才支持此参数</b></para>
    /// <para>Modify the return value from the number of new members added, to the total number of members changed</para>
    /// <para>是否将返回值从新添加元素的数量修改为更新的元素数量</para>
    /// </param>
    /// <param name="incr">
    /// <b>Starting from version 3.0.2 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 3.0.2及以上版本才支持此参数</b></para>
    /// <para>When this option is specified ZADD acts like <b>[ZINCRBY]</b>. Only one score-element pair can be specified in this mode.</para>
    /// <para>是否累加元素的分数, 如果传true, 和 <b>[ZINCRBY]</b> 命令行为一样, 且只能传入一对元素和分数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// NULL: if the operation was aborted because of a conflict with one of the XX/NX/LT/GT options.
    /// <para>Integer: the number of new members when the CH option is not used.</para>
    /// <para>Integer: the number of new or updated members when the CH option is used.</para>
    /// <para>Double: the updated score of the member when the INCR option is used.</para>
    /// <para>NULL: 如果作因与 XX/NX/LT/GT 选项冲突而中止</para>
    /// <para>Integer: 如果 CH 没有指定true, 返回新增的元素数量</para>
    /// <para>Integer: 如果 CH 指定为true, 返回更新的元素数量</para>
    /// <para>Double: 如果 INCR 指定为true, 返回累加后的元素分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZAdd">命令文档</see></para>
    /// </remarks>
    public RedisNumber ZAdd(
        SharpValue key,
        double score,
        SharpValue member,
        ZAddWhen when = ZAddWhen.None,
        ZAddScoreWhen scoreWhen = ZAddScoreWhen.None,
        bool ch = false,
        bool incr = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZAdd(
            key: key,
            member: member,
            score: score,
            when: when,
            scoreWhen: scoreWhen,
            ch: ch,
            incr: incr) | flags, cancellationToken);

    /// <summary>
    /// Adds all the specified members with the specified scores to the sorted set stored at key.
    /// <para>Available since: 2.4.0</para>
    /// <para>将元素和分数添加到指定的Key中</para>
    /// <para>支持此命令的Redis版本, 2.4.0+</para>
    /// </summary>
    /// <param name="key">SortedSet key</param>
    /// <param name="members">Set of element score key-value pairs
    /// <para>元素-分数的键值对集合</para>
    /// </param>
    /// <param name="when">
    /// <b>Starting from version 3.0.2 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 3.0.2及以上版本才支持此参数</b></para>
    /// <para>NX: Only add new members. Don't update already existing members.</para>
    /// <para>XX: Only update members that already exist. Don't add new members.</para>
    /// <para>NX: 仅在元素不存在时, 进行添加. 如果元素存在, 不进行任何操作</para>
    /// <para>XX: 仅在元素存在时, 更新其分数. 如果元素不存在, 不进行任何操作</para>
    /// </param>
    /// <param name="scoreWhen">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>LT: Only update existing members if the new score is less than the current score. This flag doesn't prevent adding new members.</para>
    /// <para>GT: Only update existing members if the new score is greater than the current score. This flag doesn't prevent adding new members.</para>
    /// <para>LT: 仅在新分数小于元素当前分数时才更新元素. 如果集合不存在当前元素, 不进行任何操作</para>
    /// <para>GT: 仅在新分数大于元素当前分数时才更新元素. 如果集合不存在当前元素, 不进行任何操作</para>
    /// </param>
    /// <param name="ch">
    /// <b>Starting from version 3.0.2 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 3.0.2及以上版本才支持此参数</b></para>
    /// <para>Modify the return value from the number of new members added, to the total number of members changed</para>
    /// <para>是否将返回值从新添加元素的数量修改为更新的元素数量</para>
    /// </param>
    /// <param name="incr">
    /// <b>Starting from version 3.0.2 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 3.0.2及以上版本才支持此参数</b></para>
    /// <para>When this option is specified ZADD acts like <b>[ZINCRBY]</b>. Only one score-element pair can be specified in this mode.</para>
    /// <para>是否累加元素的分数, 如果传true, 和 <b>[ZINCRBY]</b> 命令行为一样, 且只能传入一对元素和分数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// NULL: if the operation was aborted because of a conflict with one of the XX/NX/LT/GT options.
    /// <para>Integer: the number of new members when the CH option is not used.</para>
    /// <para>Integer: the number of new or updated members when the CH option is used.</para>
    /// <para>Double: the updated score of the member when the INCR option is used.</para>
    /// <para>NULL: 如果作因与 XX/NX/LT/GT 选项冲突而中止</para>
    /// <para>Integer: 如果 CH 没有指定true, 返回新增的元素数量</para>
    /// <para>Integer: 如果 CH 指定为true, 返回更新的元素数量</para>
    /// <para>Double: 如果 INCR 指定为true, 返回累加后的元素分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZAdd">命令文档</see></para>
    /// </remarks>
    public RedisNumber ZAdd(
        SharpValue key,
        ICollection<KeyValuePair<SharpValue, double>> members,
        ZAddWhen when = ZAddWhen.None,
        ZAddScoreWhen scoreWhen = ZAddScoreWhen.None,
        bool ch = false,
        bool incr = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZAdd(
            key: key,
            members: members,
            when: when,
            scoreWhen: scoreWhen,
            ch: ch,
            incr: incr) | flags, cancellationToken);

    /// <summary>
    /// Returns the sorted set cardinality (number of members) of the sorted set stored at key.
    /// <para>Available since: 1.2.0</para>
    /// <para>获得指定Key中的元素数量</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The cardinality (number of members) of the sorted set, or 0 if the key doesn't exist.
    /// <para>元素数量, 如果Key不存在返回0</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZCard">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZCard">命令文档</see></para>
    /// </remarks>
    public long ZCard(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZCard(key) | flags, cancellationToken);

    /// <summary>
    /// Returns the score of member in the sorted set at key.
    /// <para>Available since: 1.2.0</para>
    /// <para>获得指定元素的分数</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member">member<para>要获得分数的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The score of the member
    /// <para>元素分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZScore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZScore">命令文档</see></para>
    /// </remarks>
    public RedisNumber ZScore(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZScore(key, member) | flags, cancellationToken);

    /// <summary>
    /// Returns the number of members in the sorted set at key with a score between min and max.
    /// <para>Available since: 2.0.0</para>
    /// <para>获得指定分数范围内的所有元素数量</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min">min score<para>最小分数</para></param>
    /// <param name="max">max score<para>最大分数</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of members in the specified score range.
    /// <para>分数范围内的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZCount">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZCount">命令文档</see></para>
    /// </remarks>
    public long ZCount(SharpValue key, double min, double max, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZCount(key, min, max) | flags, cancellationToken);

    /// <summary>
    /// Returns the members of the set resulting from the difference between the first set and all the successive sets.
    /// <para>Available since: 6.2.0</para>
    /// <para>获得第一个Key与第二个Key的差值</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// <para></para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list with members of the resulting set.
    /// <para>差值结果数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZDiff">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZDiff">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZDiff(SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZDiff(key1, key2, false) | flags, cancellationToken);

    /// <summary>
    /// Returns the members of the set resulting from the difference between the first set and all the successive sets.
    /// <para>Available since: 6.2.0</para>
    /// <para>获得第一个Key与后面Key的差值</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// <para></para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list with members of the resulting set.
    /// <para>差值结果数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZDiff">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZDiff">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZDiff(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZDiff(keys, false) | flags, cancellationToken);

    /// <summary>
    /// Returns the members of the set resulting from the difference between the first set and all the successive sets. Return with score included
    /// <para>Available since: 6.2.0</para>
    /// <para>获得第一个Key与第二个Key的差值, 包含元素分数返回</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// <para></para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list with members of the resulting set.
    /// <para>差值结果数组, 包含元素分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZDiff">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZDiff">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZDiffWithScores(SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZDiff(key1, key2, true) | flags, cancellationToken);

    /// <summary>
    /// Returns the members of the set resulting from the difference between the first set and all the successive sets. Return with score included
    /// <para>Available since: 6.2.0</para>
    /// <para>获得第一个Key与后面Key的差值, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// <para></para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list with members of the resulting set.
    /// <para>差值结果数组, 包含元素分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZDiff">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZDiff">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZDiffWithScores(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZDiff(keys, true) | flags, cancellationToken);

    /// <summary>
    /// Computes the difference between the first and all successive input sorted sets and stores the result in destination.
    /// <para>Available since: 6.2.0</para>
    /// <para>计算第一个Key与第二个Key的差值, 并将差集结果存入目标Key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destination">destination key<para>保存差集结果的Key</para></param>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of members in the resulting sorted set at destination.
    /// <para>差集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZDiffStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZDiffStore">命令文档</see></para>
    /// </remarks>
    public long ZDiffStore(SharpValue destination, SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZDiffStore(key1, key2, destination) | flags, cancellationToken);

    /// <summary>
    /// Computes the difference between the first and all successive input sorted sets and stores the result in destination.
    /// <para>Available since: 6.2.0</para>
    /// <para>计算第一个Key与后续Key的差值, 并将差集结果存入目标Key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destination">destination key<para>保存差集结果的Key</para></param>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of members in the resulting sorted set at destination.
    /// <para>差集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZDiffStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZDiffStore">命令文档</see></para>
    /// </remarks>
    public long ZDiffStore(SharpValue destination, SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZDiffStore(keys, destination) | flags, cancellationToken);

    /// <summary>
    /// Returns the specified range of members in the sorted set stored at key
    /// <para>Available since: 1.2.0</para>
    /// <para>返回Key中指定范围内的元素</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">range start
    /// <para>范围开始</para>
    /// </param>
    /// <param name="stop">range stop<para>范围结束</para></param>
    /// <param name="by">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>The interval search method can be specified as either a score or an element lexicographical order.</para>
    /// <para>区间查找方式, 可指定为按分数查找, 和按元素字典序查找</para>
    /// </param>
    /// <param name="rev">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>The REV options reverses the order of the <b>start</b> and <b>stop</b> members</para>
    /// <para>是否反转查找</para>
    /// </param>
    /// <param name="offset">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>The number of skipped members</para>
    /// <para>跳过的元素个数</para>
    /// </param>
    /// <param name="count">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>The number of returned members</para>
    /// <para>返回的元素个数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of members in the specified range with
    /// <para>指定范围内的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRange">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZRange(
        SharpValue key,
        RangeArg start,
        RangeArg stop,
        RangeBy by = RangeBy.None,
        bool rev = false,
        ulong? offset = null,
        int? count = null,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRange(key: key, start: start, stop: stop, by: by, rev: rev, offset: offset, count: count) | flags, cancellationToken);

    /// <summary>
    /// Returns the specified range of members in the sorted set stored at key. Return with score included
    /// <para>Available since: 1.2.0</para>
    /// <para>返回Key中指定范围内的元素, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">range start
    /// <para>范围开始</para>
    /// </param>
    /// <param name="stop">range stop<para>范围结束</para></param>
    /// <param name="by">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>The interval search method can be specified as either a score or an element lexicographical order.</para>
    /// <para>区间查找方式, 可指定为按分数查找, 和按元素字典序查找</para>
    /// </param>
    /// <param name="rev">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>The REV options reverses the order of the <b>start</b> and <b>stop</b> members</para>
    /// <para>是否反转查找</para>
    /// </param>
    /// <param name="offset">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>The number of skipped members</para>
    /// <para>跳过的元素个数</para>
    /// </param>
    /// <param name="count">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>The number of returned members</para>
    /// <para>返回的元素个数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of members in the specified range with, with score
    /// <para>指定范围内的元素数组, 包含分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRange">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZRangeWithScores(
        SharpValue key,
        RangeArg start,
        RangeArg stop,
        RangeBy by = RangeBy.None,
        bool rev = false,
        ulong? offset = null,
        int? count = null,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRange(key: key, start: start, stop: stop, by: by, rev: rev, offset: offset, count: count, withScores: true) | flags, cancellationToken);

    /// <summary>
    /// Increments the score of member in the sorted set stored at key by increment.
    /// <para>Available since: 1.2.0</para>
    /// <para>给指定元素的分数递增</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member">member<para>要递增分数的元素</para></param>
    /// <param name="increment">increment<para>要递增的分数</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The new score of member.
    /// <para>递增后, 元素的新分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZIncrBy">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZIncrBy">命令文档</see></para>
    /// </remarks>
    public RedisNumber ZIncrBy(SharpValue key, SharpValue member, double increment, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZIncrBy(key, increment, member) | flags, cancellationToken);

    /// <summary>
    /// Increments the score of member in the sorted set stored at key by increment.
    /// <para>Available since: 1.2.0</para>
    /// <para>给指定元素的分数递增</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member">member<para>要递增分数的元素</para></param>
    /// <param name="increment">increment<para>要递增的分数</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The new score of member.
    /// <para>递增后, 元素的新分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZIncrBy">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZIncrBy">命令文档</see></para>
    /// </remarks>
    public RedisNumber ZIncrBy(SharpValue key, double increment, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZIncrBy(key, increment, member) | flags, cancellationToken);

    /// <summary>
    /// Computes the intersection of numkeys sorted sets given by the specified keys
    /// <para>Available since: 6.2.0</para>
    /// <para>获得第一个Key与第二个Key的交集</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of intersection result members
    /// <para>交集结果元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZInter">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZInter">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZInter(SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZInter(key1: key1, key2: key2, key1Weight: 1, key2Weight: 1, aggregate: Aggregate.Sum, withScores: false) | flags, cancellationToken);

    /// <summary>
    /// Computes the intersection of numkeys sorted sets given by the specified keys
    /// <para>Available since: 6.2.0</para>
    /// <para>获得第一个Key与后续Key的交集</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of intersection result members
    /// <para>交集结果元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZInter">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZInter">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZInter(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZInter(keys: keys, weights: default, aggregate: Aggregate.Sum, withScores: false) | flags, cancellationToken);

    /// <summary>
    /// Computes the intersection of numkeys sorted sets given by the specified keys. Return with score included
    /// <para>Available since: 6.2.0</para>
    /// <para>获得第一个Key与第二个Key的交集, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="key1Weight">The multiplier factor for the score of the first Key, with a default value of 1
    /// <para>第一个Key分数的乘数因子, 默认为1</para>
    /// </param>
    /// <param name="key2Weight">The multiplier factor for the score of the second Key, with a default value of 1
    /// <para>第二个Key分数的乘数因子, 默认为1</para>
    /// </param>
    /// <param name="aggregate">
    /// MIN: Return the min score
    /// <para>MAX: Return the max score</para>
    /// <para>SUM: Return the sum of the scores</para>
    /// <para>MIN: 返回最小的分数</para>
    /// <para>MAX: 返回最大的分数</para>
    /// <para>SUM: 返回分数之和</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of intersection result members
    /// <para>交集结果元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZInter">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZInter">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZInterWithScores(
        SharpValue key1,
        SharpValue key2,
        double key1Weight = 1,
        double key2Weight = 1,
        Aggregate aggregate = Aggregate.Sum,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZInter(key1: key1, key2: key2, key1Weight: key1Weight, key2Weight: key2Weight, aggregate: aggregate, withScores: true) | flags, cancellationToken);

    /// <summary>
    /// Computes the intersection of numkeys sorted sets given by the specified keys. Return with score included
    /// <para>Available since: 6.2.0</para>
    /// <para>获得指定Key的交集, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="weights">Using the WEIGHTS option, it is possible to specify a multiplication factor for each input sorted set. This means that the score of every element in every input sorted set is multiplied by this factor before being passed to the aggregation function. When WEIGHTS is not given, the multiplication factors default to 1.
    /// <para>按keys的顺序, 指定每个Key的分数的乘数, 不指定默认都为1</para>
    /// </param>
    /// <param name="aggregate">
    /// MIN: Return the min score
    /// <para>MAX: Return the max score</para>
    /// <para>SUM: Return the sum of the scores</para>
    /// <para>MIN: 返回最小的分数</para>
    /// <para>MAX: 返回最大的分数</para>
    /// <para>SUM: 返回分数之和</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of intersection result members
    /// <para>交集结果元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZInter">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZInter">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZInterWithScores(
        SharpValueSpan keys,
        ReadOnlySpan<double> weights = default,
        Aggregate aggregate = Aggregate.Sum,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZInter(keys: keys, weights: weights, aggregate: aggregate, withScores: true) | flags, cancellationToken);

    /// <summary>
    /// This command is similar to <b>[ZINTER]</b>, but instead of returning the result set, it returns just the cardinality of the result.
    /// <para>Available since: 7.0.0</para>
    /// <para>和 <b>[ZINTER]</b> 命令类似, 不过返回的是交集元素数量</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="limit"> When provided with the optional LIMIT argument (which defaults to 0 and means unlimited), if the intersection cardinality reaches limit partway through the computation, the algorithm will exit and yield limit as the cardinality. 
    /// <para>最大交集元素数量限定, 如果计算得到的元素超过此数, 则停止计算交集并返回. 默认为0无限制</para>
    /// </param>
    /// <param name="flags"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The number of members in the resulting intersection.
    /// <para>交集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZInterCard">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZInterCard">命令文档</see></para>
    /// </remarks>
    public long ZInterCard(SharpValue key1, SharpValue key2, ulong limit = 0, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZInterCard(key1, key2, limit) | flags, cancellationToken);

    /// <summary>
    /// This command is similar to <b>[ZINTER]</b>, but instead of returning the result set, it returns just the cardinality of the result.
    /// <para>Available since: 7.0.0</para>
    /// <para>和 <b>[ZINTER]</b> 命令类似, 不过返回的是交集元素数量</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="limit"> When provided with the optional LIMIT argument (which defaults to 0 and means unlimited), if the intersection cardinality reaches limit partway through the computation, the algorithm will exit and yield limit as the cardinality. 
    /// <para>最大交集元素数量限定, 如果计算得到的元素超过此数, 则停止计算交集并返回. 默认为0无限制</para>
    /// </param>
    /// <param name="flags"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The number of members in the resulting intersection.
    /// <para>交集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZInterCard">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZInterCard">命令文档</see></para>
    /// </remarks>
    public long ZInterCard(SharpValueSpan keys, ulong limit = 0, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZInterCard(keys, limit) | flags, cancellationToken);

    /// <summary>
    /// Computes the intersection of numkeys sorted sets given by the specified keys, and stores the result in destination.
    /// <para>Available since: 2.0.0</para>
    /// <para>获得第一个Key与第二个Key的交集, 并将交集结果存入指定的Key</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="destination">destination key<para>保存交集结果的key</para></param>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="key1Weight">The multiplier factor for the score of the first Key, with a default value of 1
    /// <para>第一个Key分数的乘数因子, 默认为1</para>
    /// </param>
    /// <param name="key2Weight">The multiplier factor for the score of the second Key, with a default value of 1
    /// <para>第二个Key分数的乘数因子, 默认为1</para>
    /// </param>
    /// <param name="aggregate">
    /// MIN: Return the min score
    /// <para>MAX: Return the max score</para>
    /// <para>SUM: Return the sum of the scores</para>
    /// <para>MIN: 返回最小的分数</para>
    /// <para>MAX: 返回最大的分数</para>
    /// <para>SUM: 返回分数之和</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the number of members in the resulting sorted set at the destination.
    /// <para>存入目标Key的交集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZInterStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZInterStore">命令文档</see></para>
    /// </remarks>
    public long ZInterStore(
        SharpValue destination,
        SharpValue key1,
        SharpValue key2,
        double key1Weight = 1,
        double key2Weight = 1,
        Aggregate aggregate = Aggregate.Sum,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZInterStore(destination: destination, key1: key1, key2: key2, key1Weight: key1Weight, key2Weight: key2Weight, aggregate: aggregate) | flags, cancellationToken);

    /// <summary>
    /// Computes the intersection of numkeys sorted sets given by the specified keys, and stores the result in destination.
    /// <para>Available since: 2.0.0</para>
    /// <para>获得指定的Key的交集, 并将交集结果存入指定的Key</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="destination">destination key<para>保存交集结果的key</para></param>
    /// <param name="keys"></param>
    /// <param name="weights">Using the WEIGHTS option, it is possible to specify a multiplication factor for each input sorted set. This means that the score of every element in every input sorted set is multiplied by this factor before being passed to the aggregation function. When WEIGHTS is not given, the multiplication factors default to 1.
    /// <para>按keys的顺序, 指定每个Key的分数的乘数, 不指定默认都为1</para>
    /// </param>
    /// <param name="aggregate">
    /// MIN: Return the min score
    /// <para>MAX: Return the max score</para>
    /// <para>SUM: Return the sum of the scores</para>
    /// <para>MIN: 返回最小的分数</para>
    /// <para>MAX: 返回最大的分数</para>
    /// <para>SUM: 返回分数之和</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the number of members in the resulting sorted set at the destination.
    /// <para>存入目标Key的交集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZInterStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZInterStore">命令文档</see></para>
    /// </remarks>
    public long ZInterStore(
        SharpValue destination,
        SharpValueSpan keys,
        ReadOnlySpan<double> weights = default,
        Aggregate aggregate = Aggregate.Sum,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZInterStore(destination: destination, keys: keys, weights: weights, aggregate: aggregate) | flags, cancellationToken);

    /// <summary>
    /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns the number of elements in the sorted set at key with a value between min and max.
    /// <para>Available since: 2.8.9</para>
    /// <para>当元素的分数都一致时, 根据元素字典序返回元素个数</para>
    /// <para>支持此命令的Redis版本, 2.8.9+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of members in the specified score range.
    /// <para>指定字典序范围内的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZLexCount">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZLexCount">命令文档</see></para>
    /// </remarks>
    public long ZLexCount(SharpValue key, RangeArg min, RangeArg max, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZLexCount(key, min: min, max: max) | flags, cancellationToken);

    /// <summary>
    /// Pops one or more elements, that are member-score pairs, from the first non-empty sorted set in the provided list of key names.
    /// <para>Available since: 7.0.0</para>
    /// <para>从指定Key的第一个非空集合中移除并返回元素, 包含分数</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="how">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped.
    /// <para>MIN: 从最低分开始弹出, MAX: 从最高分开始弹出</para>
    /// </param>
    /// <param name="count">The optional COUNT can be used to specify the number of elements to pop, and is set to 1 by default.
    /// <para>可选项, 弹出的个数, 默认为1个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZMPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZMPop">命令文档</see></para>
    /// </remarks>
    public SortedSetMPopResult ZMPop(
        SharpValue key1,
        SharpValue key2,
        PopHow how,
        int count = 1,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZMPop(key1, key2, how, count) | flags, cancellationToken);

    /// <summary>
    /// Pops one or more elements, that are member-score pairs, from the first non-empty sorted set in the provided list of key names.
    /// <para>Available since: 7.0.0</para>
    /// <para>从指定Key的第一个非空集合中移除并返回元素, 包含分数</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="how">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped.
    /// <para>MIN: 从最低分开始弹出, MAX: 从最高分开始弹出</para>
    /// </param>
    /// <param name="count">The optional COUNT can be used to specify the number of elements to pop, and is set to 1 by default.
    /// <para>可选项, 弹出的个数, 默认为1个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZMPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZMPop">命令文档</see></para>
    /// </remarks>
    public SortedSetMPopResult ZMPop(
        SharpValueSpan keys,
        PopHow how,
        int count = 1,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZMPop(keys, how, count) | flags, cancellationToken);

    /// <summary>
    /// Returns the scores associated with the specified members in the sorted set stored at key.
    /// <para>Available since: 6.2.0</para>
    /// <para>获取指定集合中的多个元素分数</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="members">members<para>要获取的元素集合</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of Double reply member scores as double-precision floating point numbers.
    /// <para>元素分数数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZMScore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZMScore">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisNumber[]?
#else
    RedisNumber[]
#endif
    ZMScore(SharpValue key, SharpValueSpan members, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZMScore(key, members) | flags, cancellationToken);

    /// <summary>
    /// Removes and returns up to count members with the highest scores in the sorted set stored at key.
    /// <para>Available since: 5.0.0</para>
    /// <para>从指定Key集合中, 移除并返回分数最高的一个元素, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Member:Score
    /// <para>键值对, 元素:分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZPopMax">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZPopMax">命令文档</see></para>
    /// </remarks>
    public KeyValuePair<RedisResult, RedisNumber>? ZPopMax(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    => base.Invoke(SortedSetCommands.ZPopMax(key, 1) | flags, cancellationToken);

    /// <summary>
    /// Removes and returns up to count members with the highest scores in the sorted set stored at key.
    /// <para>Available since: 5.0.0</para>
    /// <para>从指定Key集合中, 移除并返回分数最高的元素, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">Specifying a count value that is higher than the sorted set's cardinality will not produce an error. When returning multiple elements, the one with the highest score will be the first, followed by the elements with lower scores.
    /// <para>弹出的个数, 会按分数从高到低以此移除并返回</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of popped elements and scores.
    /// <para>移除的元素和分数键值对数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZPopMax">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZPopMax">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZPopMax(SharpValue key, int count, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZPopMax(key, count) | flags, cancellationToken);

    /// <summary>
    /// Removes and returns up to count members with the lowest scores in the sorted set stored at key.
    /// <para>Available since: 5.0.0</para>
    /// <para>从指定Key集合中, 移除并返回分数最低的一个元素, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Member:Score
    /// <para>键值对, 元素:分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZPopMin">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZPopMin">命令文档</see></para>
    /// </remarks>
    public KeyValuePair<RedisResult, RedisNumber>? ZPopMin(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    => base.Invoke(SortedSetCommands.ZPopMin(key, 1) | flags, cancellationToken);

    /// <summary>
    /// Removes and returns up to count members with the lowest scores in the sorted set stored at key.
    /// <para>Available since: 5.0.0</para>
    /// <para>从指定Key集合中, 移除并返回分数最低的元素, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">Specifying a count value that is higher than the sorted set's cardinality will not produce an error. When returning multiple elements, the one with the lowest score will be the first, followed by the elements with greater scores.
    /// <para>弹出的个数, 会按分数从低到高以此移除并返回</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of popped elements and scores.
    /// <para>移除的元素和分数键值对数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZPopMin">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZPopMin">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZPopMin(SharpValue key, int count, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZPopMin(key, count) | flags, cancellationToken);

    /// <summary>
    /// Return a random element from the sorted set value stored at key.
    /// <para>Available since: 6.2.0</para>
    /// <para>随机获得一个指定集合中的元素</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Randomly gets member
    /// <para>随机获得的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRandMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRandMember">命令文档</see></para>
    /// </remarks>
    public RedisResult ZRandMember(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRandMember(key, 1, false) | flags, cancellationToken);

    /// <summary>
    /// Return a random element from the sorted set value stored at key.
    /// <para>Available since: 6.2.0</para>
    /// <para>随机获得多个指定集合中的元素</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">If the provided count argument is positive, return an array of distinct elements. The array's length is either count or the sorted set's cardinality (ZCARD), whichever is lower.
    /// <para>If called with a negative count, the behavior changes and the command is allowed to return the same element multiple times. In this case, the number of returned elements is the absolute value of the specified count.</para>
    /// <para>获得的个数, 正数: 元素不会重复. 负数: 可能会有重复的元素, 且返回个数和指定的count一致</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Randomly gets member
    /// <para>随机获得的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRandMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRandMember">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZRandMember(SharpValue key, int count, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRandMember(key, count, false) | flags, cancellationToken);

    /// <summary>
    /// Return a random element from the sorted set value stored at key. Return with score included
    /// <para>Available since: 6.2.0</para>
    /// <para>随机获得一个指定集合中的元素, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Randomly gets member with score
    /// <para>随机获得的元素和分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRandMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRandMember">命令文档</see></para>
    /// </remarks>
    public KeyValuePair<RedisResult, RedisNumber>? ZRandMemberWithScores(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRandMember(key, 1, true) | flags, cancellationToken);

    /// <summary>
    /// Return a random element from the sorted set value stored at key. Return with score included
    /// <para>Available since: 6.2.0</para>
    /// <para>随机获得多个指定集合中的元素, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">If the provided count argument is positive, return an array of distinct elements. The array's length is either count or the sorted set's cardinality (ZCARD), whichever is lower.
    /// <para>If called with a negative count, the behavior changes and the command is allowed to return the same element multiple times. In this case, the number of returned elements is the absolute value of the specified count.</para>
    /// <para>获得的个数, 正数: 元素不会重复. 负数: 可能会有重复的元素, 且返回个数和指定的count一致</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Randomly gets member with score
    /// <para>随机获得的元素和分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRandMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRandMember">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZRandMemberWithScores(SharpValue key, int count, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRandMember(key, count, true) | flags, cancellationToken);

    /// <summary>
    /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between min and max.
    /// <para>Available since: 2.8.9 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>在集合中元素分数都一致的时候, 强制按照元素字典序排序并返回区间元素</para>
    /// <para>支持此命令的Redis版本, 2.8.9+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="offset">offset<para>跳过的个数</para></param>
    /// <param name="count">count<para>返回的元素个数</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of elements in the specified score range.
    /// <para>在指定字典序区间的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRangeByLex">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRangeByLex">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZRangeByLex(SharpValue key, RangeArg min, RangeArg max, ulong? offset = null, int? count = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRangeByLex(key, min: min, max: max, offset: offset, count: count) | flags, cancellationToken);

    /// <summary>
    /// Returns all the elements in the sorted set at key with a score between min and max (including elements with score equal to min or max). The elements are considered to be ordered from low to high scores.
    /// <para>Available since: 1.0.5 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>根据分数区间返回元素, 按分数从低到高排序的</para>
    /// <para>支持此命令的Redis版本, 1.0.5+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min">min score<para>最小分数</para></param>
    /// <param name="max">max score<para>最大分数</para></param>
    /// <param name="offset">offset<para>跳过的个数</para></param>
    /// <param name="count">count<para>返回的元素个数</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of the members
    /// <para>分数区间内的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRangeByScore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRangeByScore">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZRangeByScore(SharpValue key, RangeArg min, RangeArg max, ulong? offset = null, int? count = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRangeByScore(key, min: min, max: max, withScores: false, offset: offset, count: count) | flags, cancellationToken);

    /// <summary>
    /// Returns all the elements in the sorted set at key with a score between min and max (including elements with score equal to min or max). The elements are considered to be ordered from low to high scores.
    /// <para>Available since: 2.0.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>根据分数区间返回元素, 按分数从低到高排序的. 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 2.0.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min">min score<para>最小分数</para></param>
    /// <param name="max">max score<para>最大分数</para></param>
    /// <param name="offset">offset<para>跳过的个数</para></param>
    /// <param name="count">count<para>返回的元素个数</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of the members
    /// <para>分数区间内的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRangeByScore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRangeByScore">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZRangeByScoreWithScores(SharpValue key, RangeArg min, RangeArg max, ulong? offset = null, int? count = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRangeByScore(key, min: min, max: max, withScores: true, offset: offset, count: count) | flags, cancellationToken);

    /// <summary>
    /// This command is like <b>[ZRANGE]</b>, but stores the result in the destination key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和  <b>[ZRANGE]</b> 类似, 不过不返回区间结果, 而是将结果存入指定的目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destination">destination key<para>保存结果的Key</para></param>
    /// <param name="key">key<para>源Key</para></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="by">
    /// The interval search method can be specified as either a score or an element lexicographical order.
    /// <para>区间查找方式, 可指定为按分数查找, 和按元素字典序查找</para>
    /// </param>
    /// <param name="rev">
    /// The REV options reverses the order of the <b>start</b> and <b>stop</b> members
    /// <para>是否反转查找</para>
    /// </param>
    /// <param name="offset">
    /// <para>The number of skipped members</para>
    /// <para>跳过的元素个数</para>
    /// </param>
    /// <param name="count">
    /// <para>The number of returned members</para>
    /// <para>返回的元素个数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting sorted set.
    /// <para>存入目标Key中的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRangeStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRangeStore">命令文档</see></para>
    /// </remarks>
    public long ZRangeStore(
        SharpValue destination,
        SharpValue key,
        RangeArg min,
        RangeArg max,
        RangeBy by = RangeBy.None,
        bool rev = false,
        ulong? offset = null,
        int? count = null,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRangeStore(destination: destination, key: key, min: min, max: max, by: by, rev: rev, offset: offset, count: count) | flags, cancellationToken);

    /// <summary>
    /// Returns the rank of member in the sorted set stored at key, with the scores ordered from low to high. The rank (or index) is 0-based, which means that the member with the lowest score has rank 0.
    /// <para>Available since: 2.0.0</para>
    /// <para>返回指定元素在集合中的排名, 按分数升序排名(低到高). 也就是分数最低的为0</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member">member<para>要获得排名的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The rank of the member
    /// <para>元素的排名</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRank">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRank">命令文档</see></para>
    /// </remarks>
    public long? ZRank(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRank(key, member, false) | flags, cancellationToken);

    /// <summary>
    /// Returns the rank of member in the sorted set stored at key, with the scores ordered from low to high. The rank (or index) is 0-based, which means that the member with the lowest score has rank 0.
    /// <para>Available since: 7.2.0</para>
    /// <para>返回指定元素在集合中的排名, 按分数升序排名(低到高). 也就是分数最低的为0. 包含元素分数返回</para>
    /// <para>支持此命令的Redis版本, 7.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member">member<para>要获得排名的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The rank and score of the member
    /// <para>元素的排名和分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRank">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRank">命令文档</see></para>
    /// </remarks>
    public SortedSetRankResult ZRankWithScore(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRank(key, member, true) | flags, cancellationToken);

    /// <summary>
    /// Removes the specified member from the sorted set stored at key
    /// <para>Available since: 1.2.0</para>
    /// <para>从指定Key集合中, 移除一个指定的元素</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member">The member to be remove<para>要移除的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of members removed from the sorted set, not including non-existing members.
    /// <para>从集合中移除的元素数量, 不包括不存在的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRem">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRem">命令文档</see></para>
    /// </remarks>
    public long ZRem(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRem(key, member) | flags, cancellationToken);

    /// <summary>
    /// Removes the specified members from the sorted set stored at key
    /// <para>Available since: 2.4.0</para>
    /// <para>从指定Key集合中, 移除多个指定的元素</para>
    /// <para>支持此命令的Redis版本, 2.4.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="members">The members to be remove<para>要移除的多个元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of members removed from the sorted set, not including non-existing members.
    /// <para>从集合中移除的元素数量, 不包括不存在的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRem">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRem">命令文档</see></para>
    /// </remarks>
    public long ZRem(SharpValue key, SharpValueSpan members, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRem(key, members) | flags, cancellationToken);

    /// <summary>
    /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command removes all elements in the sorted set stored at key between the lexicographical range specified by min and max.
    /// <para>Available since: 2.8.9</para>
    /// <para>按元素的字典序, 移除区间内的元素</para>
    /// <para>支持此命令的Redis版本, 2.8.9+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of members removed
    /// <para>移除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRemRangeByLex">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRemRangeByLex">命令文档</see></para>
    /// </remarks>
    public long ZRemRangeByLex(SharpValue key, RangeArg min, RangeArg max, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRemRangeByLex(key, min: min, max: max) | flags, cancellationToken);

    /// <summary>
    /// Removes all elements in the sorted set stored at key with rank between start and stop. Both start and stop are 0 -based indexes with 0 being the element with the lowest score. These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score. For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
    /// <para>Available since: 2.0.0</para>
    /// <para>移除排名区间内的元素. 按分数升序排序的</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">Both start and stop are 0 -based indexes with 0 being the element with the lowest score. These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score. For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
    /// <para>排名开始, 0表示第一个, -1表示最后一个, -2表示倒数第二个, 以此类推</para>
    /// </param>
    /// <param name="stop">Both start and stop are 0 -based indexes with 0 being the element with the lowest score. These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score. For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
    /// <para>排名结束, -1表示最后一个, -2表示倒数第二个, 以此类推</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of members removed.
    /// <para>移除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRemRangeByRank">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRemRangeByRank">命令文档</see></para>
    /// </remarks>
    public long ZRemRangeByRank(SharpValue key, long start, long stop, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRemRangeByRank(key, start: start, stop: stop) | flags, cancellationToken);

    /// <summary>
    /// Removes all members in the sorted set stored at key with a score between min and max.
    /// <para>Available since: 1.2.0</para>
    /// <para>根据分数区间, 移除元素</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min">min score<para>最小分数</para></param>
    /// <param name="max">max score<para>最大分数</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of members removed.
    /// <para>移除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRemRangeByScore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRemRangeByScore">命令文档</see></para>
    /// </remarks>
    public long ZRemRangeByScore(SharpValue key, RangeArg min, RangeArg max, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRemRangeByScore(key, min: min, max: max) | flags, cancellationToken);

    /// <summary>
    /// Returns the specified range of elements in the sorted set stored at key. The elements are considered to be ordered from the highest to the lowest score. Descending lexicographical order is used for elements with equal score.
    /// <para>Available since: 1.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>根据元素分数降序排名(从高到低), 返回指定区间内的元素. 如果元素分数一致, 则按元素字典序排名</para>
    /// <para>支持此命令的Redis版本, 1.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">The ranking can be negative. -1 indicates the element with the highest score, and -2 indicates the element with the second-highest score.
    /// <para>排名开始, 0表示第一个, -1表示最后一个, -2表示倒数第二个, 以此类推</para>
    /// </param>
    /// <param name="stop">The ranking can be negative. -1 indicates the element with the highest score, and -2 indicates the element with the second-highest score.
    /// <para>排名结束, -1表示最后一个, -2表示倒数第二个, 以此类推</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of members in the specified range
    /// <para>排名区间内的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRevRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRevRange">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZRevRange(SharpValue key, long start, long stop, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRevRange(key, start: start, stop: stop, withScores: false) | flags, cancellationToken);

    /// <summary>
    /// Returns the specified range of elements in the sorted set stored at key. The elements are considered to be ordered from the highest to the lowest score. Descending lexicographical order is used for elements with equal score.
    /// <para>Available since: 1.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>根据元素分数降序排名(从高到低), 返回指定区间内的元素. 如果元素分数一致, 则按元素字典序排名</para>
    /// <para>支持此命令的Redis版本, 1.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">The ranking can be negative. -1 indicates the element with the highest score, and -2 indicates the element with the second-highest score.
    /// <para>排名开始, 0表示第一个, -1表示最后一个, -2表示倒数第二个, 以此类推</para>
    /// </param>
    /// <param name="stop">The ranking can be negative. -1 indicates the element with the highest score, and -2 indicates the element with the second-highest score.
    /// <para>排名结束, -1表示最后一个, -2表示倒数第二个, 以此类推</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of members in the specified range
    /// <para>排名区间内的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRevRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRevRange">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
     KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZRevRangeWithScores(SharpValue key, long start, long stop, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRevRange(key, start: start, stop: stop, withScores: true) | flags, cancellationToken);

    /// <summary>
    /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between max and min.
    /// <para>Available since: 2.8.9 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>根据元素的字典序倒序(从大到小), 返回指定区间内的元素</para>
    /// <para>支持此命令的Redis版本, 2.8.9+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="max"></param>
    /// <param name="min"></param>
    /// <param name="offset">offset<para>跳过的元素数量</para></param>
    /// <param name="count">count<para>返回的元素数量</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of the elements in the specified score range.
    /// <para>指定范围内的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRevRangeByLex">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRevRangeByLex">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZRevRangeByLex(SharpValue key, RangeArg max, RangeArg min, ulong? offset = null, int? count = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRevRangeByLex(key, max: max, min: min, offset: offset, count: count) | flags, cancellationToken);

    /// <summary>
    /// Returns all the elements in the sorted set at key with a score between max and min (including elements with score equal to max or min). In contrary to the default ordering of sorted sets, for this command the elements are considered to be ordered from high to low scores.
    /// <para>Available since: 2.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>根据元素分数倒序(从高到低), 返回指定分数区间内的元素</para>
    /// <para>支持此命令的Redis版本, 2.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="max">max score<para>最高分</para></param>
    /// <param name="min">min score<para>最低分</para></param>
    /// <param name="offset">offset<para>跳过的元素数量</para></param>
    /// <param name="count">count<para>返回的元素数量</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of the elements in the specified score range.
    /// <para>指定范围内的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRevRangeByScore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRevRangeByScore">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZRevRangeByScore(SharpValue key, RangeArg max, RangeArg min, ulong? offset = null, int? count = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRevRangeByScore(key, max: max, min: min, withScores: false, offset: offset, count: count) | flags, cancellationToken);

    /// <summary>
    /// Returns all the elements in the sorted set at key with a score between max and min (including elements with score equal to max or min). In contrary to the default ordering of sorted sets, for this command the elements are considered to be ordered from high to low scores.
    /// <para>Available since: 2.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>根据元素分数倒序(从高到低), 返回指定分数区间内的元素, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 2.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="max">max score<para>最高分</para></param>
    /// <param name="min">min score<para>最低分</para></param>
    /// <param name="offset">offset<para>跳过的元素数量</para></param>
    /// <param name="count">count<para>返回的元素数量</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of the elements in the specified score range. Return with score included
    /// <para>指定范围内的元素-分数 数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRevRangeByScore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRevRangeByScore">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZRevRangeByScoreWithScores(SharpValue key, RangeArg max, RangeArg min, ulong? offset = null, int? count = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRevRangeByScore(key, max: max, min: min, withScores: true, offset: offset, count: count) | flags, cancellationToken);

    /// <summary>
    /// Returns the rank of member in the sorted set stored at key, with the scores ordered from high to low. The rank (or index) is 0-based, which means that the member with the highest score has rank 0.
    /// <para>Available since: 2.0.0</para>
    /// <para>返回指定元素在集合中的排名, 按分数降序排名(高到低). 也就是分数最高的为0</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member">member<para>要获得排名的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The rank of the member
    /// <para>元素的排名</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRevRank">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRevRank">命令文档</see></para>
    /// </remarks>
    public long? ZRevRank(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRevRank(key, member, false) | flags, cancellationToken);

    /// <summary>
    /// Returns the rank of member in the sorted set stored at key, with the scores ordered from high to low. The rank (or index) is 0-based, which means that the member with the highest score has rank 0.
    /// <para>Available since: 7.2.0</para>
    /// <para>返回指定元素在集合中的排名, 按分数降序排名(高到低). 也就是分数最高的为0, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 7.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member">member<para>要获得排名的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The rank and score of the member
    /// <para>元素的排名和分数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZRevRank">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZRevRank">命令文档</see></para>
    /// </remarks>
    public SortedSetRankResult ZRevRankWithScore(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZRevRank(key, member, true) | flags, cancellationToken);

    /// <summary>
    /// Computes the union of numkeys sorted sets given by the specified keys
    /// <para>Available since: 6.2.0</para>
    /// <para>获得第一个Key与第二个Key的并集</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of union result members
    /// <para>并集结果元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZUnion">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZUnion">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZUnion(SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZUnion(key1: key1, key2: key2, key1Weight: 1, key2Weight: 1, aggregate: Aggregate.Sum, withScores: false) | flags, cancellationToken);

    /// <summary>
    /// Computes the union of numkeys sorted sets given by the specified keys
    /// <para>Available since: 6.2.0</para>
    /// <para>获得第一个Key与后续Key的并集</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of union result members
    /// <para>并集结果元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZUnion">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZUnion">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ZUnion(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZUnion(keys: keys, weights: default, aggregate: Aggregate.Sum, withScores: false) | flags, cancellationToken);

    /// <summary>
    /// Computes the union of numkeys sorted sets given by the specified keys. Return with score included
    /// <para>Available since: 6.2.0</para>
    /// <para>获得第一个Key与第二个Key的并集, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="key1Weight">The multiplier factor for the score of the first Key, with a default value of 1
    /// <para>第一个Key分数的乘数因子, 默认为1</para>
    /// </param>
    /// <param name="key2Weight">The multiplier factor for the score of the second Key, with a default value of 1
    /// <para>第二个Key分数的乘数因子, 默认为1</para>
    /// </param>
    /// <param name="aggregate">
    /// MIN: Return the min score
    /// <para>MAX: Return the max score</para>
    /// <para>SUM: Return the sum of the scores</para>
    /// <para>MIN: 返回最小的分数</para>
    /// <para>MAX: 返回最大的分数</para>
    /// <para>SUM: 返回分数之和</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of union result members
    /// <para>并集结果元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZUnion">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZUnion">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZUnionWithScores(
        SharpValue key1,
        SharpValue key2,
        double key1Weight = 1,
        double key2Weight = 1,
        Aggregate aggregate = Aggregate.Sum,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZUnion(key1: key1, key2: key2, key1Weight: key1Weight, key2Weight: key2Weight, aggregate: aggregate, withScores: true) | flags, cancellationToken);

    /// <summary>
    /// Computes the union of numkeys sorted sets given by the specified keys. Return with score included
    /// <para>Available since: 6.2.0</para>
    /// <para>获得指定Key的并集, 包含分数返回</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="weights">Using the WEIGHTS option, it is possible to specify a multiplication factor for each input sorted set. This means that the score of every element in every input sorted set is multiplied by this factor before being passed to the aggregation function. When WEIGHTS is not given, the multiplication factors default to 1.
    /// <para>按keys的顺序, 指定每个Key的分数的乘数, 不指定默认都为1</para>
    /// </param>
    /// <param name="aggregate">
    /// MIN: Return the min score
    /// <para>MAX: Return the max score</para>
    /// <para>SUM: Return the sum of the scores</para>
    /// <para>MIN: 返回最小的分数</para>
    /// <para>MAX: 返回最大的分数</para>
    /// <para>SUM: 返回分数之和</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of union result members
    /// <para>并集结果元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZUnion">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZUnion">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZUnionWithScores(
        SharpValueSpan keys,
        ReadOnlySpan<double> weights = default,
        Aggregate aggregate = Aggregate.Sum,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZUnion(keys: keys, weights: weights, aggregate: aggregate, withScores: true) | flags, cancellationToken);

    /// <summary>
    /// Computes the union of numkeys sorted sets given by the specified keys, and stores the result in destination.
    /// <para>Available since: 2.0.0</para>
    /// <para>获得第一个Key与第二个Key的并集, 并将并集结果存入指定的Key</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="destination">destination key<para>保存并集结果的key</para></param>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="key1Weight">The multiplier factor for the score of the first Key, with a default value of 1
    /// <para>第一个Key分数的乘数因子, 默认为1</para>
    /// </param>
    /// <param name="key2Weight">The multiplier factor for the score of the second Key, with a default value of 1
    /// <para>第二个Key分数的乘数因子, 默认为1</para>
    /// </param>
    /// <param name="aggregate">
    /// MIN: Return the min score
    /// <para>MAX: Return the max score</para>
    /// <para>SUM: Return the sum of the scores</para>
    /// <para>MIN: 返回最小的分数</para>
    /// <para>MAX: 返回最大的分数</para>
    /// <para>SUM: 返回分数之和</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the number of members in the resulting sorted set at the destination.
    /// <para>存入目标Key的并集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZUnionStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZUnionStore">命令文档</see></para>
    /// </remarks>
    public long ZUnionStore(
        SharpValue destination,
        SharpValue key1,
        SharpValue key2,
        double key1Weight = 1,
        double key2Weight = 1,
        Aggregate aggregate = Aggregate.Sum,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZUnionStore(destination: destination, key1: key1, key2: key2, key1Weight: key1Weight, key2Weight: key2Weight, aggregate: aggregate) | flags, cancellationToken);

    /// <summary>
    /// Computes the union of numkeys sorted sets given by the specified keys, and stores the result in destination.
    /// <para>Available since: 2.0.0</para>
    /// <para>获得指定Key的并集, 并将并集结果存入指定的Key</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="destination">destination key<para>保存并集结果的key</para></param>
    /// <param name="keys"></param>
    /// <param name="weights">Using the WEIGHTS option, it is possible to specify a multiplication factor for each input sorted set. This means that the score of every element in every input sorted set is multiplied by this factor before being passed to the aggregation function. When WEIGHTS is not given, the multiplication factors default to 1.
    /// <para>按keys的顺序, 指定每个Key的分数的乘数, 不指定默认都为1</para>
    /// </param>
    /// <param name="aggregate">
    /// MIN: Return the min score
    /// <para>MAX: Return the max score</para>
    /// <para>SUM: Return the sum of the scores</para>
    /// <para>MIN: 返回最小的分数</para>
    /// <para>MAX: 返回最大的分数</para>
    /// <para>SUM: 返回分数之和</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the number of members in the resulting sorted set at the destination.
    /// <para>存入目标Key的并集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZUnionStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZUnionStore">命令文档</see></para>
    /// </remarks>
    public long ZUnionStore(
        SharpValue destination,
        SharpValueSpan keys,
        ReadOnlySpan<double> weights = default,
        Aggregate aggregate = Aggregate.Sum,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.ZUnionStore(destination: destination, keys: keys, weights: weights, aggregate: aggregate) | flags, cancellationToken);

    /// <summary>
    /// When all sorted sets are empty, Redis will block the connection until another client adds members to one of the keys or until the timeout (a double value specifying the maximum number of seconds to block) elapses. A timeout of zero can be used to block indefinitely.
    /// <para>Available since: 7.0.0</para>
    /// <para>从指定的集合中, 弹出(删除并返回)指定数量的元素. 如果集合为空, 则阻塞, 直到超时或有元素可以弹出</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="timeout">A double value specifying the maximum number of seconds to block elapses. A timeout of zero can be used to block indefinitely.
    /// <para>阻塞超时时间, 单位: 秒. 指定0表示无限阻塞. 不建议无限阻塞, 因为长时间阻塞连接可能会丢失</para>
    /// </param>
    /// <param name="key"></param>
    /// <param name="how">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped.
    /// <para>MIN: 从最低分开始弹出, MAX: 从最高分开始弹出</para>
    /// </param>
    /// <param name="count">The optional COUNT can be used to specify the number of elements to pop, and is set to 1 by default.
    /// <para>可选项, 弹出的个数, 默认为1个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BZMPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BZMPop">命令文档</see></para>
    /// </remarks>
    public SortedSetMPopResult BZMPop(double timeout, SharpValue key, PopHow how, int count = 1, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.BZMPop(timeout, key, how, count) | flags, cancellationToken);

    /// <summary>
    /// When all sorted sets are empty, Redis will block the connection until another client adds members to one of the keys or until the timeout (a double value specifying the maximum number of seconds to block) elapses. A timeout of zero can be used to block indefinitely.
    /// <para>Available since: 7.0.0</para>
    /// <para>从指定的集合中, 弹出(删除并返回)指定数量的元素. 如果集合为空, 则阻塞, 直到超时或有元素可以弹出</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="timeout">A double value specifying the maximum number of seconds to block elapses. A timeout of zero can be used to block indefinitely.
    /// <para>阻塞超时时间, 单位: 秒. 指定0表示无限阻塞. 不建议无限阻塞, 因为长时间阻塞连接可能会丢失</para>
    /// </param>
    /// <param name="keys"></param>
    /// <param name="how">When the MIN modifier is used, the elements popped are those with the lowest scores from the first non-empty sorted set. The MAX modifier causes elements with the highest scores to be popped.
    /// <para>MIN: 从最低分开始弹出, MAX: 从最高分开始弹出</para>
    /// </param>
    /// <param name="count">The optional COUNT can be used to specify the number of elements to pop, and is set to 1 by default.
    /// <para>可选项, 弹出的个数, 默认为1个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BZMPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BZMPop">命令文档</see></para>
    /// </remarks>
    public SortedSetMPopResult BZMPop(double timeout, SharpValueSpan keys, PopHow how, int count = 1, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.BZMPop(timeout, keys, how, count) | flags, cancellationToken);

    /// <summary>
    /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive. It is the blocking version because it blocks the connection when there are no members to pop from any of the given sorted sets. A member with the highest score is popped from first sorted set that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 5.0.0</para>
    /// <para>从指定的非空集合中, 弹出(移除并返回)一个得分最高的元素</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="timeout">A int value specifying the maximum number of seconds to block elapses. A timeout of zero can be used to block indefinitely.
    /// <para>阻塞超时时间, 单位: 秒. 指定0表示无限阻塞. 不建议无限阻塞, 因为长时间阻塞连接可能会丢失</para>
    /// </param>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BZPopMax">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BZPopMax">命令文档</see></para>
    /// </remarks>
    public SortedSetPopResult BZPopMax(int timeout, SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.BZPopMax(timeout, key) | flags, cancellationToken);

    /// <summary>
    /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive. It is the blocking version because it blocks the connection when there are no members to pop from any of the given sorted sets. A member with the highest score is popped from first sorted set that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 6.0.0</para>
    /// <para>从指定的非空集合中, 弹出(移除并返回)一个得分最高的元素</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="timeout">A double value specifying the maximum number of seconds to block elapses. A timeout of zero can be used to block indefinitely.
    /// <para>阻塞超时时间, 单位: 秒. 指定0表示无限阻塞. 不建议无限阻塞, 因为长时间阻塞连接可能会丢失</para>
    /// </param>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BZPopMax">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BZPopMax">命令文档</see></para>
    /// </remarks>
    public SortedSetPopResult BZPopMax(double timeout, SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.BZPopMax(timeout, key) | flags, cancellationToken);

    /// <summary>
    /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive. It is the blocking version because it blocks the connection when there are no members to pop from any of the given sorted sets. A member with the highest score is popped from first sorted set that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 5.0.0</para>
    /// <para>从指定的非空集合中, 弹出(移除并返回)一个得分最高的元素</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="timeout">A int value specifying the maximum number of seconds to block elapses. A timeout of zero can be used to block indefinitely.
    /// <para>阻塞超时时间, 单位: 秒. 指定0表示无限阻塞. 不建议无限阻塞, 因为长时间阻塞连接可能会丢失</para>
    /// </param>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BZPopMax">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BZPopMax">命令文档</see></para>
    /// </remarks>
    public SortedSetPopResult BZPopMax(int timeout, SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.BZPopMax(timeout, keys) | flags, cancellationToken);

    /// <summary>
    /// BZPOPMAX is the blocking variant of the sorted set ZPOPMAX primitive. It is the blocking version because it blocks the connection when there are no members to pop from any of the given sorted sets. A member with the highest score is popped from first sorted set that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 6.0.0</para>
    /// <para>从指定的非空集合中, 弹出(移除并返回)一个得分最高的元素</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="timeout">A double value specifying the maximum number of seconds to block elapses. A timeout of zero can be used to block indefinitely.
    /// <para>阻塞超时时间, 单位: 秒. 指定0表示无限阻塞. 不建议无限阻塞, 因为长时间阻塞连接可能会丢失</para>
    /// </param>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BZPopMax">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BZPopMax">命令文档</see></para>
    /// </remarks>
    public SortedSetPopResult BZPopMax(double timeout, SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.BZPopMax(timeout, keys) | flags, cancellationToken);

    /// <summary>
    /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive. It is the blocking version because it blocks the connection when there are no members to pop from any of the given sorted sets. A member with the lowest score is popped from first sorted set that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 5.0.0</para>
    /// <para>从指定的非空集合中, 弹出(移除并返回)一个得分最低的元素</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="timeout">A int value specifying the maximum number of seconds to block elapses. A timeout of zero can be used to block indefinitely.
    /// <para>阻塞超时时间, 单位: 秒. 指定0表示无限阻塞. 不建议无限阻塞, 因为长时间阻塞连接可能会丢失</para>
    /// </param>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BZPopMin">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BZPopMin">命令文档</see></para>
    /// </remarks>
    public SortedSetPopResult BZPopMin(int timeout, SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.BZPopMin(timeout, key) | flags, cancellationToken);

    /// <summary>
    /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive. It is the blocking version because it blocks the connection when there are no members to pop from any of the given sorted sets. A member with the lowest score is popped from first sorted set that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 6.0.0</para>
    /// <para>从指定的非空集合中, 弹出(移除并返回)一个得分最低的元素</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="timeout">A double value specifying the maximum number of seconds to block elapses. A timeout of zero can be used to block indefinitely.
    /// <para>阻塞超时时间, 单位: 秒. 指定0表示无限阻塞. 不建议无限阻塞, 因为长时间阻塞连接可能会丢失</para>
    /// </param>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BZPopMin">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BZPopMin">命令文档</see></para>
    /// </remarks>
    public SortedSetPopResult BZPopMin(double timeout, SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.BZPopMin(timeout, key) | flags, cancellationToken);

    /// <summary>
    /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive. It is the blocking version because it blocks the connection when there are no members to pop from any of the given sorted sets. A member with the lowest score is popped from first sorted set that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 5.0.0</para>
    /// <para>从指定的非空集合中, 弹出(移除并返回)一个得分最低的元素</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="timeout">A int value specifying the maximum number of seconds to block elapses. A timeout of zero can be used to block indefinitely.
    /// <para>阻塞超时时间, 单位: 秒. 指定0表示无限阻塞. 不建议无限阻塞, 因为长时间阻塞连接可能会丢失</para>
    /// </param>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BZPopMin">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BZPopMin">命令文档</see></para>
    /// </remarks>
    public SortedSetPopResult BZPopMin(int timeout, SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.BZPopMin(timeout, keys) | flags, cancellationToken);

    /// <summary>
    /// BZPOPMIN is the blocking variant of the sorted set ZPOPMIN primitive. It is the blocking version because it blocks the connection when there are no members to pop from any of the given sorted sets. A member with the lowest score is popped from first sorted set that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 6.0.0</para>
    /// <para>从指定的非空集合中, 弹出(移除并返回)一个得分最低的元素</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="timeout">A double value specifying the maximum number of seconds to block elapses. A timeout of zero can be used to block indefinitely.
    /// <para>阻塞超时时间, 单位: 秒. 指定0表示无限阻塞. 不建议无限阻塞, 因为长时间阻塞连接可能会丢失</para>
    /// </param>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BZPopMin">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BZPopMin">命令文档</see></para>
    /// </remarks>
    public SortedSetPopResult BZPopMin(double timeout, SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(SortedSetCommands.BZPopMin(timeout, keys) | flags, cancellationToken);

    /// <summary>
    /// Atomically moves members from the sorted set at source to the sorted set at destination, preserving their scores. Select members by lowest score (MIN), highest score (MAX), or a score range (BYSCORE).
    /// <para>Available since: 8.12.0</para>
    /// <para>将源Set集合中的成员移动到目标Set集合中.</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="source">The key of the sorted set to move members from.
    /// <para>源Set集合的Key (要移除成员的Set集合)</para>
    /// </param>
    /// <param name="destination">The key of the sorted set to move members to.
    /// <para>接受移动成员的Set集合Key</para>
    /// </param>
    /// <param name="how">Specifies the member selection mode for the ZMOVE command.
    /// <para>指定 ZMOVE 命令从源 Sorted Set 中选择成员进行移动的方式。</para>
    /// </param>
    /// <param name="scoreMin">Minimum score. Only valid in the BYSCORE mode
    /// <para>最小分数, 仅在 BYSCORE 模式下有效</para>
    /// </param>
    /// <param name="scoreMax">
    /// Maximum score. Only valid in the BYSCORE mode
    /// <para>最大分数, 仅在 BYSCORE 模式下有效</para>
    /// </param>
    /// <param name="count">Move at most count selected members. count must be a positive integer.
    /// <para>最多移动成员的数量</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>an array of two-element member/score arrays
    /// <para>被移动的成员-分数 数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZMove">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZMove">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZMove(
        SharpValue source,
        SharpValue destination,
        MoveHow how,
        double? scoreMin = null,
        double? scoreMax = null,
        long? count = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(SortedSetCommands.ZMove(
            source: source,
            destination: destination,
            how: how,
            scoreMin: scoreMin,
            scoreMax: scoreMax,
            count: count) | flags, cancellationToken);
    }

    /// <summary>
    /// BZMOVE is the blocking variant of ZMOVE for moving lowest-scoring or highest-scoring members between sorted sets.
    /// <para>Available since: 8.12.0</para>
    /// <para>ZMOVE命令的阻塞版</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="source">The key of the sorted set to move members from.
    /// <para>源Set集合的Key (要移除成员的Set集合)</para>
    /// </param>
    /// <param name="destination">The key of the sorted set to move members to.
    /// <para>接受移动成员的Set集合Key</para>
    /// </param>
    /// <param name="timeout">The maximum time to block, in seconds. This is a non-negative double, so fractional seconds are allowed. A timeout of 0 blocks indefinitely.
    /// <para>阻塞最长时间, 单位: 秒. 0表示无限期阻塞</para>
    /// </param>
    /// <param name="how">Specifies the member selection mode for the ZMOVE command.
    /// <para>指定 ZMOVE 命令从源 Sorted Set 中选择成员进行移动的方式。</para>
    /// </param>
    /// <param name="count">Move at most count available members. The default is 1. count must be a positive integer.
    /// <para>最多移动成员数量. 默认值为1</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>an array of two-element member/score arrays
    /// <para>被移动的成员-分数 数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BZMove">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BZMove">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    BZMove(
        SharpValue source,
        SharpValue destination,
        double timeout,
        MoveHow how,
        long? count = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(SortedSetCommands.BZMove(
            source: source,
            destination: destination,
            timeout: timeout,
            how: how,
            count: count) | flags, cancellationToken);
    }

    /// <summary>
    /// Atomically moves named members from the sorted set at source to the sorted set at destination, preserving their scores.
    /// <para>Available since: 8.12.0</para>
    /// <para>从源集合中, 移动指定的成员到目标集合</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="source">The key of the sorted set to move members from.
    /// <para>源Set集合的Key (要移除成员的Set集合)</para>
    /// </param>
    /// <param name="destination">The key of the sorted set to move members to.
    /// <para>接受移动成员的Set集合Key</para>
    /// </param>
    /// <param name="member">The member to be moved
    /// <para>要移动的成员</para>
    /// </param>
    /// <param name="when">The moving option, targeting the target set
    /// <para>移动的选项, 针对目标集合</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>an array of two-element member/score arrays
    /// <para>被移动的成员-分数 数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZMoveMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZMoveMember">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZMoveMember(
        SharpValue source,
        SharpValue destination,
        SharpValue member,
        MoveWhen when = MoveWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(SortedSetCommands.ZMoveMember(
            source: source,
            destination: destination,
            when: when,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// Atomically moves named members from the sorted set at source to the sorted set at destination, preserving their scores.
    /// <para>Available since: 8.12.0</para>
    /// <para>从源集合中, 移动指定的成员到目标集合</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="source">The key of the sorted set to move members from.
    /// <para>源Set集合的Key (要移除成员的Set集合)</para>
    /// </param>
    /// <param name="destination">The key of the sorted set to move members to.
    /// <para>接受移动成员的Set集合Key</para>
    /// </param>
    /// <param name="members">The member to be moved, Support multiple
    /// <para>要移动的成员, 支持多个</para>
    /// </param>
    /// <param name="when">The moving option, targeting the target set
    /// <para>移动的选项, 针对目标集合</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>an array of two-element member/score arrays
    /// <para>被移动的成员-分数 数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ZMoveMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ZMoveMember">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    KeyValuePair<RedisResult, RedisNumber>[]?
#else
    KeyValuePair<RedisResult, RedisNumber>[]
#endif
    ZMoveMember(
        SharpValue source,
        SharpValue destination,
        SharpValueSpan members,
        MoveWhen when = MoveWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(SortedSetCommands.ZMoveMember(
            source: source,
            destination: destination,
            when: when,
            members: members) | flags, cancellationToken);
    }
}
