using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis arrays operation class
/// <para>If you are new to arrays, see <see href="https://redis.io/docs/develop/data-types/arrays">Introduction to Redis arrays</see></para>
/// <para><b>ℹ️Note: Array is a new data type that is currently in preview and may be subject to change.</b></para>
/// <para>Redis arrays(数组) 类型操作类型</para>
/// <para>如果你不太懂Arrays类型, 请<see href="https://redis.io/docs/develop/data-types/arrays">查看Arrays文档</see></para>
/// <para><b>ℹ️提示: 数组(Arrays)是一种新的数据类型, 目前在Redis中处于预览阶段, 未来可能有会有所更改</b></para>
/// </summary>
public sealed partial class RedisArray : RedisTypeBase
{
    /// <summary>
    /// The aggregate function to apply to all non-empty elements in [start, end].
    /// <para>聚合操作类型枚举</para>
    /// </summary>
    public enum AggregateOp
    {
        /// <summary>
        /// Returns the sum of all numeric values. If there is no numeric value, return null.
        /// <para>返回所有数值的总和. 如果没有数值, 返回null</para>
        /// </summary>
        Sum = 0,

        /// <summary>
        /// Returns the minimum numeric value. If there is no numeric value, return null.
        /// <para>返回最小的数值. 如果没有数值, 返回null</para>
        /// </summary>
        Min,

        /// <summary>
        /// Returns the maximum numeric value. If there is no numeric value, return null.
        /// <para>返回最大的数值. 如果没有数值, 返回null</para>
        /// </summary>
        Max,

        /// <summary>
        /// Returns the bitwise AND of all values, treating each as an integer (floats are truncated toward zero). If there is no numeric value, return null.
        /// <para>返回所有数值的按位与运算结果，将每个值视为整数（浮点数向零截断）. 如果没有数值, 返回null</para>
        /// </summary>
        And,

        /// <summary>
        /// Returns the bitwise OR of all values, treating each as an integer (floats are truncated toward zero). If there is no numeric value, return null.
        /// <para>返回所有数值的按位或运算结果，将每个值视为整数（浮点数向零截断）. 如果没有数值, 返回null</para>
        /// </summary>
        Or,

        /// <summary>
        /// Returns the bitwise XOR of all values, treating each as an integer (floats are truncated toward zero). If there is no numeric value, return null.
        /// <para>返回所有数值的按位异或结果，将每个值视为整数（浮点数向零截断）. 如果没有数值, 返回null</para>
        /// </summary>
        Xor,

        /// <summary>
        /// Returns the count of elements whose value equals value as an integer reply
        /// <para>统计元素值等于 value 的元素的数量</para>
        /// </summary>
        Match,

        /// <summary>
        /// Returns the count of non-empty elements in the range as an integer reply.
        /// <para>返回指定范围内非空元素的数量</para>
        /// </summary>
        Used,
    }

    /// <summary>
    /// Text matching modes for ARGREP.
    /// <para>ARGREP 的文本匹配模式。</para>
    /// </summary>
    public enum GrepPredicate : byte
    {
        /// <summary>
        /// Match the whole value.
        /// <para>匹配完整值。</para>
        /// </summary>
        Exact,

        /// <summary>
        /// Match a literal substring.
        /// <para>匹配字面子串。</para>
        /// </summary>
        Match,

        /// <summary>
        /// Match a glob pattern.
        /// <para>按通配符模式匹配。</para>
        /// </summary>
        Glob,

        /// <summary>
        /// Match a regular expression.
        /// <para>按正则表达式匹配。</para>
        /// </summary>
        Re,
    }

    /// <summary>
    /// How ARGREP combines all predicates.
    /// <para>ARGREP 合并全部匹配条件的方式。</para>
    /// </summary>
    public enum GrepOperator : byte
    {
        /// <summary>
        /// Default behavior, not specified
        /// <para>默认行为, 不指定</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Combine multiple predicates with logical OR. An element matches if any predicate matches. This is the default.
        /// <para>使用逻辑 OR 组合多个谓词。如果任何一个谓词匹配，则该元素匹配。</para>
        /// </summary>
        Or,

        /// <summary>
        /// Combine multiple predicates with logical AND. An element matches only if every predicate matches.
        /// <para>使用逻辑 AND 组合多个谓词。只有当所有谓词都匹配时，元素才匹配。</para>
        /// </summary>
        And,
    }

    /// <summary>
    /// A matching mode and its literal text or pattern.
    /// <para>一个匹配模式及其文本或模式参数。</para>
    /// </summary>
    public readonly struct GrepCondition : IEquatable<GrepCondition>
    {
        private readonly GrepPredicate _type;
        private readonly SharpValue _pattern;
        private readonly GrepOperator _operator = GrepOperator.None;

        /// <summary>
        /// Matching mode.
        /// <para>匹配模式。</para>
        /// </summary>
        public readonly GrepPredicate Type => this._type;

        /// <summary>
        /// Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </summary>
        public readonly SharpValue Pattern => this._pattern;

        /// <summary>
        /// Predicate combination filtering logic
        /// <para>谓词筛选组合逻辑</para>
        /// </summary>
        public readonly GrepOperator Operator => this._operator;

        /// <summary>
        /// Create a matching predicate based on the matching pattern
        /// <para>根据匹配模式创建一个匹配谓词</para>
        /// </summary>
        /// <param name="type">Text matching modes for ARGREP.
        /// <para>ARGREP 的文本匹配模式。</para>
        /// </param>
        /// <param name="pattern">Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </param>
        public GrepCondition(GrepPredicate type, SharpValue pattern)
        {
            if (pattern.IsNullOrEmpty) throw new ArgumentException("The matched text or pattern cannot be empty.", nameof(pattern));
            this._type = type;
            this._pattern = pattern;
        }

        internal GrepCondition(GrepOperator @operator, GrepPredicate type, SharpValue pattern)
        {
            if (pattern.IsNullOrEmpty) throw new ArgumentException("The matched text or pattern cannot be empty.", nameof(pattern));
            this._operator = @operator;
            this._type = type;
            this._pattern = pattern;
        }

        public bool Equals(GrepCondition other)
        {
            return this._operator == other._operator
                && this._type == other._type
                && this._pattern == other._pattern;
        }

#if MODERN_NET
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
#else
        public override bool Equals(object obj)
#endif
        {
            return obj is GrepCondition other && this.Equals(other);
        }

        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            unchecked
            {
                int hash = 23;
                hash = hash * 31 + this._operator.GetHashCode();
                hash = hash * 31 + this._type.GetHashCode();
                hash = hash * 31 + this._pattern.GetHashCode();
                return hash;
            }
#else
            return HashCode.Combine(this._operator, this._type, this._pattern);
#endif
        }

        public static bool operator ==(GrepCondition left, GrepCondition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GrepCondition left, GrepCondition right)
        {
            return !left.Equals(right);
        }

        public static implicit operator GrepCondition((GrepPredicate Type, SharpValue Pattern) condition)
            => new(condition.Type, condition.Pattern);
    }

    /// <summary>
    /// A nonnegative array index or a symbolic range boundary.
    /// <para>非负数组索引或符号边界。</para>
    /// </summary>
    public readonly struct GrepIndex : IEquatable<GrepIndex>
    {
        private readonly long _index;

        /// <summary>
        /// Numeric index, or zero for a symbolic boundary.
        /// <para>数值索引，符号边界时为 0。</para>
        /// </summary>
        public readonly long Index => this._index < 0 ? 0 : this._index;

        /// <summary>
        /// Boundary symbol, or the null character for a numeric index.
        /// <para>边界符号，数值索引时为空字符。</para>
        /// </summary>
        public readonly char Symbol => this._index switch { -1 => '-', -2 => '+', _ => '\0' };

        /// <summary>
        /// Creates a numeric index.
        /// <para>创建非负数值索引。</para>
        /// </summary>
        public GrepIndex(long index)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
#endif
            this._index = index;
        }

        /// <summary>
        /// Creates a boundary from a minus or plus sign.
        /// <para>使用减号或加号创建边界。</para>
        /// </summary>
        public GrepIndex(char symbol)
        {
            this._index = symbol switch
            {
                '-' => -1,
                '+' => -2,
                _ => throw new ArgumentOutOfRangeException(nameof(symbol)),
            };
        }

        public static implicit operator GrepIndex(long index) => new(index);

        public static implicit operator GrepIndex(char symbol) => new(symbol);

        public static implicit operator GrepIndex(string symbol) => symbol switch
        {
            "-" => new('-'),
            "+" => new('+'),
            _ => throw new ArgumentOutOfRangeException(nameof(symbol)),
        };

        public bool Equals(GrepIndex other) => this._index == other._index;

#if MODERN_NET
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
#else
        public override bool Equals(object obj)
#endif
            => obj is GrepIndex other && this.Equals(other);

        public override int GetHashCode() => this._index.GetHashCode();

        public static bool operator ==(GrepIndex left, GrepIndex right) => left.Equals(right);

        public static bool operator !=(GrepIndex left, GrepIndex right) => !left.Equals(right);
    }

    /// <summary>
    /// Multiple filtering predicates combined
    /// <para>多个筛选谓词组合</para>
    /// </summary>
    public sealed class GrepConditions
    {
        private readonly List<GrepCondition> _conditions;

        /// <summary>
        /// Gets the pre-assembled set of predicates
        /// <para>获取已组装好的谓词集合</para>
        /// </summary>
        public IReadOnlyList<GrepCondition> Conditions => this._conditions;

        /// <summary>
        /// Create a combination based on a predicate
        /// <para>根据谓词创建组合</para>
        /// </summary>
        /// <param name="condition">predicate
        /// <para>筛选谓词</para>
        /// </param>
        public GrepConditions(GrepCondition condition)
        {
            this._conditions = [with(4), condition];
        }

        /// <summary>
        /// Create a combination based on a predicate
        /// <para>根据谓词创建组合</para>
        /// </summary>
        /// <param name="type">Text matching modes for ARGREP.
        /// <para>ARGREP 的文本匹配模式。</para>
        /// </param>
        /// <param name="pattern">Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </param>
        public GrepConditions(GrepPredicate type, SharpValue pattern)
            : this(new(type, pattern))
        {
        }

        /// <summary>
        /// Use logical "AND" to concatenate the next predicate
        /// <para>使用逻辑AND拼接下一个谓词</para>
        /// </summary>
        /// <param name="condition">predicate
        /// <para>筛选谓词</para>
        /// </param>
        /// <returns></returns>
        public GrepConditions And(GrepCondition condition)
        {
            this._conditions.Add(new(GrepOperator.And, condition.Type, condition.Pattern));
            return this;
        }

        /// <summary>
        /// Use logical "OR" to concatenate the next predicate
        /// <para>使用逻辑OR拼接下一个谓词</para>
        /// </summary>
        /// <param name="condition">predicate
        /// <para>筛选谓词</para>
        /// </param>
        /// <returns></returns>
        public GrepConditions Or(GrepCondition condition)
        {
            this._conditions.Add(new(GrepOperator.Or, condition.Type, condition.Pattern));
            return this;
        }

        /// <summary>
        /// Use logical AND and complete value matching to concatenate a predicate
        /// <para>使用逻辑AND和完整值匹配拼接一个谓词</para>
        /// </summary>
        /// <param name="pattern">Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </param>
        /// <returns></returns>
        public GrepConditions AndExact(SharpValue pattern)
        {
            this._conditions.Add(new(GrepOperator.And, GrepPredicate.Exact, pattern));
            return this;
        }

        /// <summary>
        /// Use logical AND and match literal strings to concatenate a predicate
        /// <para>使用逻辑AND和匹配字面字串拼接一个谓词</para>
        /// </summary>
        /// <param name="pattern">Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </param>
        /// <returns></returns>
        public GrepConditions AndMatch(SharpValue pattern)
        {
            this._conditions.Add(new(GrepOperator.And, GrepPredicate.Match, pattern));
            return this;
        }

        /// <summary>
        /// Use logical AND and wildcard pattern matching to concatenate a predicate
        /// <para>使用逻辑AND和通配符模式匹配拼接一个谓词</para>
        /// </summary>
        /// <param name="pattern">Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </param>
        /// <returns></returns>
        public GrepConditions AndGlob(SharpValue pattern)
        {
            this._conditions.Add(new(GrepOperator.And, GrepPredicate.Glob, pattern));
            return this;
        }

        /// <summary>
        /// Use logical AND and regular expression matching to concatenate a predicate
        /// <para>使用逻辑AND和正则匹配拼接一个谓词</para>
        /// </summary>
        /// <param name="pattern">Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </param>
        /// <returns></returns>
        public GrepConditions AndRe(SharpValue pattern)
        {
            this._conditions.Add(new(GrepOperator.And, GrepPredicate.Re, pattern));
            return this;
        }

        /// <summary>
        /// Use logical OR and complete value matching to concatenate a predicate
        /// <para>使用逻辑OR和完整值匹配拼接一个谓词</para>
        /// </summary>
        /// <param name="pattern">Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </param>
        /// <returns></returns>
        public GrepConditions OrExact(SharpValue pattern)
        {
            this._conditions.Add(new(GrepOperator.Or, GrepPredicate.Exact, pattern));
            return this;
        }

        /// <summary>
        /// Use logical OR and match literal strings to concatenate a predicate
        /// <para>使用逻辑OR和匹配字面字串拼接一个谓词</para>
        /// </summary>
        /// <param name="pattern">Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </param>
        /// <returns></returns>
        public GrepConditions OrMatch(SharpValue pattern)
        {
            this._conditions.Add(new(GrepOperator.Or, GrepPredicate.Match, pattern));
            return this;
        }

        /// <summary>
        /// Use logical AND and wildcard pattern matching to concatenate a predicate
        /// <para>使用逻辑AND和通配符模式匹配拼接一个谓词</para>
        /// </summary>
        /// <param name="pattern">Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </param>
        /// <returns></returns>
        public GrepConditions OrGlob(SharpValue pattern)
        {
            this._conditions.Add(new(GrepOperator.Or, GrepPredicate.Glob, pattern));
            return this;
        }

        /// <summary>
        /// Use logical OR and regular expression matching to concatenate a predicate
        /// <para>使用逻辑OR和正则匹配拼接一个谓词</para>
        /// </summary>
        /// <param name="pattern">Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </param>
        /// <returns></returns>
        public GrepConditions OrRe(SharpValue pattern)
        {
            this._conditions.Add(new(GrepOperator.Or, GrepPredicate.Re, pattern));
            return this;
        }

        /// <summary>
        /// Create a combination based on a predicate
        /// <para>根据谓词创建组合</para>
        /// </summary>
        /// <param name="condition">predicate
        /// <para>筛选谓词</para>
        /// </param>
        public static GrepConditions Create(GrepCondition condition)
        {
            return new(condition);
        }

        /// <summary>
        /// Create a combination based on a predicate
        /// <para>根据谓词创建组合</para>
        /// </summary>
        /// <param name="type">Text matching modes for ARGREP.
        /// <para>ARGREP 的文本匹配模式。</para>
        /// </param>
        /// <param name="pattern">Literal text or pattern.
        /// <para>匹配文本或模式。</para>
        /// </param>
        public static GrepConditions Create(GrepPredicate type, SharpValue pattern)
        {
            return new(type, pattern);
        }
    }

    /// <summary>
    /// Represent a range has start and end indexes.
    /// <para>表示一个具有开始和结束的范围索引</para>
    /// </summary>
    /// <example>
    /// <code>
    /// Range range = new(0, 100);
    /// Range range = Range.Create(0, 100);
    /// Range range = (0, 100);
    /// </code>
    /// </example>
    public readonly struct Range : IEquatable<Range>
    {
        private readonly long _start;
        private readonly long _end;

        /// <summary>
        /// Represent the inclusive start index of the Range
        /// <para>获得范围索引的开始索引</para>
        /// </summary>
        public readonly long Start => this._start;

        /// <summary>
        /// Represent the inclusive end index of the Range
        /// <para>获得范围索引的结束索引</para>
        /// </summary>
        public readonly long End => this._end;

        /// <summary>
        /// Create a range index based on the start and end points
        /// <para>根据开始和结束创建一个范围索引</para>
        /// </summary>
        /// <param name="start">Represent the inclusive start index of the range
        /// <para>开始的索引</para>
        /// </param>
        /// <param name="end">Represent the inclusive end index of the range
        /// <para>结束的索引</para>
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Range(long start, long end)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(start);
            ArgumentOutOfRangeException.ThrowIfNegative(end);
#else
            if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
            if (end < 0) throw new ArgumentOutOfRangeException(nameof(end));
#endif
            this._start = start;
            this._end = end;
        }

        /// <inheritdoc cref="Range(long, long)"/>
        public static Range Create(long start, long end)
        {
            return new(start, end);
        }

        public bool Equals(Range other)
        {
            return this._start == other._start
                && this._end == other._end;
        }

#if MODERN_NET
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
#else
        public override bool Equals(object obj)
#endif
        {
            if (obj is not Range other) return false;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            unchecked
            {
                int hash = 23;
                hash = hash * 31 + this._start.GetHashCode();
                hash = hash * 31 + this._end.GetHashCode();
                return hash;
            }
#else
            return HashCode.Combine(this._start, this._end);
#endif
        }

        public static implicit operator Range((long start, long end) range) => new(range.start, range.end);

        public static bool operator ==(Range left, Range right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Range left, Range right)
        {
            return !left.Equals(right);
        }
    }

    internal RedisArray(BaseRedis redis) : base(redis)
    {
    }

    /// <summary>
    /// Sets one or more contiguous values starting at an index in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据指定的下标, 设置数组的值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key, 如果Key不存在则自动创建</para>
    /// </param>
    /// <param name="index">The zero-based integer index at which to start writing.
    /// <para>要设置值的索引, 索引从 0 开始</para>
    /// </param>
    /// <param name="value">The value to be inserted
    /// <para>要插入的值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of new slots that were set (previously empty).
    /// <para>新增的数组槽位数量 (非设置成功的值数量)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArSet">命令文档</see></para>
    /// </remarks>
    public long ArSet(
        SharpValue key,
        long index,
        SharpValue value,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArSet(key, index, value) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets one or more contiguous values starting at an index in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据指定的下标, 设置数组的值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key, 如果Key不存在则自动创建</para>
    /// </param>
    /// <param name="index">The zero-based integer index at which to start writing.
    /// <para>要设置值的索引, 索引从 0 开始</para>
    /// </param>
    /// <param name="values">One or more string values to store at consecutive indices beginning at index. The number of new (previously empty) slots filled is returned.
    /// <para>要插入的值, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of new slots that were set (previously empty).
    /// <para>新增的数组槽位数量 (非设置成功的值数量)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArSet">命令文档</see></para>
    /// </remarks>
    public long ArSet(
        SharpValue key,
        long index,
        SharpValueSpan values,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArSet(key, index, values) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the number of non-empty elements in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>统计数组中, 非空元素的数量</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of non-empty elements, or 0 if key does not exist.
    /// <para>非空元素的数量, 如果Key不存在返回0</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArCount">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArCount">命令文档</see></para>
    /// </remarks>
    public long ArCount(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArCount(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes elements at the specified indices in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除数组中, 指定索引处的元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组key</para>
    /// </param>
    /// <param name="index">One zero-based integer indices of the elements to delete
    /// <para>要删除元素的索引, 从0开始的索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDel">命令文档</see></para>
    /// </remarks>
    public long ArDel(SharpValue key, long index, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArDel(key, index) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes elements at the specified indices in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除数组中, 指定索引处的元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组key</para>
    /// </param>
    /// <param name="indexs">One or more zero-based integer indices of the elements to delete. 
    /// <para>要删除元素的索引, 从0开始的索引. 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDel">命令文档</see></para>
    /// </remarks>
    public long ArDel(SharpValue key, ReadOnlySpan<long> indexs, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArDel(key, indexs) | flags, cancellationToken);
    }

    /// <summary>
    /// Delete elements in one range.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除范围索引内的元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="start">the start index
    /// <para>开始的索引</para>
    /// </param>
    /// <param name="end">the end index
    /// <para>结束的索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDelRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDelRange">命令文档</see></para>
    /// </remarks>
    public long ArDelRange(SharpValue key, long start, long end, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArDelRange(key, start, end) | flags, cancellationToken);
    }

    /// <summary>
    /// Delete elements in one range.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除范围索引内的元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="range">One start end pairs, each defining an inclusive range of indices to delete. 
    /// <para>包含开始和结束的范围索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDelRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDelRange">命令文档</see></para>
    /// </remarks>
    public long ArDelRange(SharpValue key, RedisArray.Range range, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArDelRange(key, range) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes elements in one or more ranges.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除范围索引内的元素, 可指定一组或多组范围索引</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="ranges">One or more start end pairs, each defining an inclusive range of indices to delete.
    /// <para>包含开始和结束的范围索引, 支持多组传入</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDelRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDelRange">命令文档</see></para>
    /// </remarks>
    public long ArDelRange(SharpValue key, ReadOnlySpan<RedisArray.Range> ranges, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArDelRange(key, ranges) | flags, cancellationToken);
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Delete elements in one range.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除范围索引内的元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="range">One start end pairs, each defining an inclusive range of indices to delete. 
    /// <para>包含开始和结束的范围索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDelRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDelRange">命令文档</see></para>
    /// </remarks>
    public long ArDelRange(SharpValue key, System.Range range, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArDelRange(key, range) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes elements in one or more ranges.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除范围索引内的元素, 可指定一组或多组范围索引</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="ranges">One or more start end pairs, each defining an inclusive range of indices to delete.
    /// <para>包含开始和结束的范围索引, 支持多组传入</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDelRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDelRange">命令文档</see></para>
    /// </remarks>
    public long ArDelRange(SharpValue key, ReadOnlySpan<System.Range> ranges, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArDelRange(key, ranges) | flags, cancellationToken);
    }

    /// <summary>
    /// Gets values in a range of indices. If start is greater than end, elements are returned in reverse index order.
    /// <para>Available since: 8.8.0</para>
    /// <para>获得指定范围索引内的元素值. 如果开始的索引大于结束索引, 则按索引逆序返回元素列表</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的key</para>
    /// </param>
    /// <param name="range">One start end pairs, each defining an inclusive range of indices to delete. 
    /// <para>包含开始和结束的范围索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArGetRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArGetRange">命令文档</see></para>
    /// </remarks>
    public RedisResult[] ArGetRange(SharpValue key, System.Range range, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArGetRange(key, range) | flags, cancellationToken)!;
    }
#endif

    /// <summary>
    /// Gets the value at an index in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据索引获得元素值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="index">The zero-based integer index of the element to retrieve.
    /// <para>要获取元素值的索引, 从0开始的</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value at the given index.
    /// <para>索引处的元素值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArGet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArGet">命令文档</see></para>
    /// </remarks>
    public RedisResult ArGet(SharpValue key, long index, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArGet(key, index) | flags, cancellationToken);
    }

    /// <summary>
    /// Gets values in a range of indices. If start is greater than end, elements are returned in reverse index order.
    /// <para>Available since: 8.8.0</para>
    /// <para>获得指定范围索引内的元素值. 如果开始的索引大于结束索引, 则按索引逆序返回元素列表</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的key</para>
    /// </param>
    /// <param name="start">The zero-based integer index of the first element to return.
    /// <para>开始的索引, 从0开始</para>
    /// </param>
    /// <param name="end">The zero-based integer index of the last element to return (inclusive)
    /// <para>结束的索引, 从0开始</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArGetRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArGetRange">命令文档</see></para>
    /// </remarks>
    public RedisResult[] ArGetRange(SharpValue key, long start, long end, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArGetRange(key, start, end) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Gets values in a range of indices. If start is greater than end, elements are returned in reverse index order.
    /// <para>Available since: 8.8.0</para>
    /// <para>获得指定范围索引内的元素值. 如果开始的索引大于结束索引, 则按索引逆序返回元素列表</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的key</para>
    /// </param>
    /// <param name="range">One start end pairs, each defining an inclusive range of indices to delete. 
    /// <para>包含开始和结束的范围索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArGetRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArGetRange">命令文档</see></para>
    /// </remarks>
    public RedisResult[] ArGetRange(SharpValue key, RedisArray.Range range, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArGetRange(key, range) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Searches array elements in the inclusive range from start to end.
    /// <para>Available since: 8.8.0</para>
    /// <para>按起止索引指定的方向搜索数组元素，包含两个端点。</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">Array key.
    /// <para>数组的 key。</para>
    /// </param>
    /// <param name="start">First index or boundary.
    /// <para>起始索引或边界符号。</para>
    /// </param>
    /// <param name="end">Last index or boundary.
    /// <para>结束索引或边界符号。</para>
    /// </param>
    /// <param name="conditions">One or more predicates and their common operator.
    /// <para>包含至少一个匹配条件及整组的逻辑运算符。</para>
    /// </param>
    /// <param name="limit">Maximum number of matches. A specified limit must be positive.
    /// <para>最多返回的匹配数，须为正数，省略时返回全部匹配项。</para>
    /// </param>
    /// <param name="withValues">Include matching values.
    /// <para>同时返回匹配元素的值。</para>
    /// </param>
    /// <param name="noCase">Ignore case for all predicates.
    /// <para>全部条件均忽略大小写。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Matches in traversal order. Value is RedisResult.Empty unless withValues is enabled.
    /// <para>按遍历顺序返回匹配项，省略 withValues 时 Value 为 RedisResult.Empty。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/argrep">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/argrep">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    ArItem[]?
#else
    ArItem[]
#endif
    ArGrep(
        SharpValue key,
        GrepIndex start,
        GrepIndex end,
        GrepConditions conditions,
        long? limit = null,
        bool withValues = false,
        bool noCase = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArGrep(key, start, end, conditions, limit, withValues, noCase) | flags, cancellationToken);
    }

    /// <summary>
    /// Searches array elements in the inclusive range from start to end.
    /// <para>Available since: 8.8.0</para>
    /// <para>按起止索引指定的方向搜索数组元素，包含两个端点。</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">Array key.
    /// <para>数组的 key。</para>
    /// </param>
    /// <param name="start">First index or boundary.
    /// <para>起始索引或边界符号。</para>
    /// </param>
    /// <param name="end">Last index or boundary.
    /// <para>结束索引或边界符号。</para>
    /// </param>
    /// <param name="condition">One predicate
    /// <para>一个匹配条件。</para>
    /// </param>
    /// <param name="limit">Maximum number of matches. A specified limit must be positive.
    /// <para>最多返回的匹配数，须为正数，省略时返回全部匹配项。</para>
    /// </param>
    /// <param name="withValues">Include matching values.
    /// <para>同时返回匹配元素的值。</para>
    /// </param>
    /// <param name="noCase">Ignore case for all predicates.
    /// <para>全部条件均忽略大小写。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Matches in traversal order. Value is RedisResult.Empty unless withValues is enabled.
    /// <para>按遍历顺序返回匹配项，省略 withValues 时 Value 为 RedisResult.Empty。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/argrep">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/argrep">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    ArItem[]?
#else
    ArItem[]
#endif
    ArGrep(
        SharpValue key,
        GrepIndex start,
        GrepIndex end,
        GrepCondition condition,
        long? limit = null,
        bool withValues = false,
        bool noCase = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArGrep(key, start, end, condition, limit, withValues, noCase) | flags, cancellationToken);
    }

    /// <summary>
    /// Inserts one or more values at consecutive indices.
    /// <para>Available since: 8.8.0</para>
    /// <para>在连续的索引处插入一个或多个值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="value">One or more string values to insert at consecutive indices, beginning at the current insert cursor position. The cursor advances by one for each value inserted.
    /// <para>要插入的值, 从当前插入索引位置开始, 每次插入一个值, 索引则向前进一</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The last index where a value was inserted.
    /// <para>上次插入值的索引下标</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arinsert">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arinsert">命令文档</see></para>
    /// </remarks>
    public long ArInsert(SharpValue key, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArInsert(key, value) | flags, cancellationToken);
    }

    /// <summary>
    /// Inserts one or more values at consecutive indices.
    /// <para>Available since: 8.8.0</para>
    /// <para>在连续的索引处插入一个或多个值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="values">One or more string values to insert at consecutive indices, beginning at the current insert cursor position. The cursor advances by one for each value inserted.
    /// <para>要插入的值, 支持多个. 从当前插入索引位置开始, 每次插入一个值, 索引则向前进一</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The last index where a value was inserted.
    /// <para>上次插入值的索引下标</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arinsert">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arinsert">命令文档</see></para>
    /// </remarks>
    public long ArInsert(SharpValue key, SharpValueSpan values, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArInsert(key, values) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the most recently inserted elements.
    /// <para>Available since: 8.8.0</para>
    /// <para>获得最近插入的元素列表</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="count">The maximum number of most recently inserted elements to return. If the array contains fewer elements than count, all elements are returned.
    /// <para>要返回的最近插入元素的最多数量。如果数组中的元素数量少于 count ，则返回所有元素。</para>
    /// </param>
    /// <param name="rev">When present, returns elements in reverse chronological order (most recent first) instead of the default oldest-first order.
    /// <para>是否按插入时间, 倒序返回. 默认为: 插入最早的在前</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arlastitems">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arlastitems">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ArLastItems(SharpValue key, long count, bool rev = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArLastItems(key, count, rev) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the length of an array (max index + 1).
    /// <para>Available since: 8.8.0</para>
    /// <para>获得数组的长度</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the array (max index + 1), or 0 if key does not exist.
    /// <para>数组的长度, 如果Key不存在返回0</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arlen">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arlen">命令文档</see></para>
    /// </remarks>
    public long ArLen(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArLen(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Gets values at multiple indices in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据多个索引下标, 批量获取元素值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="indexs">One or more zero-based integer indices of the elements to retrieve. The reply preserves the order of the requested indices and returns nil for any index that is not set.
    /// <para>一个获多个索引下标</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of elements that are saved in the same order as the specified index sequence
    /// <para>和指定索引下标顺序保存一致的元素数组. 如果索引处没有元素, 返回null(RedisResult.IsNull == true)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/armget">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/armget">命令文档</see></para>
    /// </remarks>
    public RedisResult[] ArMGet(SharpValue key, ReadOnlySpan<long> indexs, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArMGet(key, indexs) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Sets multiple index-value pairs in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据索引和值, 批量设置索引和值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="indexValues">One or more index value pairs. Each index is a zero-based integer specifying where to write, and each value is the string to store at that position. Pairs may be non-contiguous and in any order.
    /// <para>一个或多个 索引和值 的集合. 索引可以不是连续的, 可以任意指定</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of new slots that were set (previously empty).
    /// <para>新增的数组槽位数量 (非设置成功的值数量)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/armset">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/armset">命令文档</see></para>
    /// </remarks>
    public long ArMSet(
        SharpValue key,
        ReadOnlySpan<(long index, SharpValue value)> indexValues,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArMSet(key, indexValues) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets multiple index-value pairs in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据索引和值, 批量设置索引和值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="indexValues">One or more index value pairs. Each index is a zero-based integer specifying where to write, and each value is the string to store at that position. Pairs may be non-contiguous and in any order.
    /// <para>一个或多个 索引和值 的集合. 索引可以不是连续的, 可以任意指定</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of new slots that were set (previously empty).
    /// <para>新增的数组槽位数量 (非设置成功的值数量)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/armset">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/armset">命令文档</see></para>
    /// </remarks>
    public long ArMSet(
        SharpValue key,
        ReadOnlySpan<KeyValuePair<long, SharpValue>> indexValues,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArMSet(key, indexValues) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the next index ARINSERT would use.
    /// <para>Available since: 8.8.0</para>
    /// <para>返回 ARINSERT 将使用的下一个索引下标</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The next index ARINSERT would use. Returns 0 for missing keys or when no insert happened yet.
    /// <para>Null when the insertion cursor is exhausted (next insert would overflow).</para>
    /// <para>ARINSERT 将使用的下一个索引下标. Key不存在或者没有插入操作返回0</para>
    /// <para>当插入索引下标耗尽时（下一次插入将溢出）, 返回null</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arnext">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arnext">命令文档</see></para>
    /// </remarks>
    public long? ArNext(SharpValue key,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArNext(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Performs aggregate operations on array elements in a range.
    /// <para>Available since: 8.8.0</para>
    /// <para>对指定范围内的元素进行聚合操作</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="start">The zero-based integer index of the first element in the range to aggregate.
    /// <para>要聚合的开始元素下标</para>
    /// </param>
    /// <param name="end">The zero-based integer index of the last element in the range to aggregate (inclusive).
    /// <para>要聚合的结束元素下标</para>
    /// </param>
    /// <param name="op">Aggregation operation type
    /// <para>聚合操作类型</para>
    /// </param>
    /// <param name="matchValue">Counts the values of the elements that specify a certain quantity. The parameter is invalid if it is not used for specifying the quantity of elements.
    /// <para>统计指定元素值数量的值, 非统计元素值数量时, 该参数无效</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arop">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arop">命令文档</see></para>
    /// </remarks>
    public RedisNumber ArOp(
        SharpValue key,
        long start,
        long end,
        RedisArray.AggregateOp op,
        SharpValue matchValue = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArOp(key: key, start: start, end: end, op: op, matchValue: matchValue) | flags, cancellationToken);
    }

    /// <summary>
    /// Performs aggregate operations on array elements in a range.
    /// <para>Available since: 8.8.0</para>
    /// <para>对指定范围内的元素进行聚合操作</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="range">The index range of the elements to be aggregated
    /// <para>要聚合的元素索引范围</para>
    /// </param>
    /// <param name="op">Aggregation operation type
    /// <para>聚合操作类型</para>
    /// </param>
    /// <param name="matchValue">Counts the values of the elements that specify a certain quantity. The parameter is invalid if it is not used for specifying the quantity of elements.
    /// <para>统计指定元素值数量的值, 非统计元素值数量时, 该参数无效</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arop">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arop">命令文档</see></para>
    /// </remarks>
    public RedisNumber ArOp(
        SharpValue key,
        RedisArray.Range range,
        RedisArray.AggregateOp op,
        SharpValue matchValue = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArOp(key: key, range: range, op: op, matchValue: matchValue) | flags, cancellationToken);
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Performs aggregate operations on array elements in a range.
    /// <para>Available since: 8.8.0</para>
    /// <para>对指定范围内的元素进行聚合操作</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="range">The index range of the elements to be aggregated
    /// <para>要聚合的元素索引范围</para>
    /// </param>
    /// <param name="op">Aggregation operation type
    /// <para>聚合操作类型</para>
    /// </param>
    /// <param name="matchValue">Counts the values of the elements that specify a certain quantity. The parameter is invalid if it is not used for specifying the quantity of elements.
    /// <para>统计指定元素值数量的值, 非统计元素值数量时, 该参数无效</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arop">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arop">命令文档</see></para>
    /// </remarks>
    public RedisNumber ArOp(
        SharpValue key,
        System.Range range,
        RedisArray.AggregateOp op,
        SharpValue matchValue = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArOp(key: key, range: range, op: op, matchValue: matchValue) | flags, cancellationToken);
    }

    /// <summary>
    /// Iterates existing elements in a range, returning index-value pairs.
    /// <para>Available since: 8.8.0</para>
    /// <para>遍历指定范围内的现有元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="range">The index range of the elements to be scanned
    /// <para>要扫描的元素索引范围. 如果开始大于结束, 则按索引逆序返回元素</para>
    /// </param>
    /// <param name="limit">The maximum number of index-value pairs to return. When omitted, all elements in the range are returned.
    /// <para>做多返回元素数量. 如果不设置此参数, 则返回索引范围内的所有元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arscan">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arscan">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    ArItem[]?
#else
    GrepResult[]
#endif
    ArScan(
        SharpValue key,
        System.Range range,
        long? limit = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArScan(key: key, range: range, limit: limit) | flags, cancellationToken);
    }
#endif

    /// <summary>
    /// Inserts values into a ring buffer of specified size, wrapping and truncating as needed.
    /// <para>Available since: 8.8.0</para>
    /// <para>将值插入指定大小的环形缓冲区，根据需要进行循环和截断。</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="size">The size of the ring buffer window. Each value is inserted at insert_idx % size, wrapping back to index 0 when the end of the window is reached. When the buffer is full, newer values overwrite older ones. If size is smaller than the current window, the array is truncated to fit.
    /// <para>环形缓冲区窗口的大小。每个值都会被插入到 insert_idx % size 的位置，当窗口到达末尾时会循环回到索引 0。当缓冲区满时，新值会覆盖旧值。如果大小小于当前窗口，则数组会被截断以适应。</para>
    /// </param>
    /// <param name="value">One or more string values to insert into the ring buffer. Each value is placed at the next position in the ring and the cursor advances accordingly.
    /// <para>一个或多个字符串值要插入到环形缓冲区中。每个值都会被放置到环形缓冲区的下一个位置，同时光标相应地向前移动。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The last index where a value was inserted.
    /// <para>最后一个插入值的索引下标</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arring">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arring">命令文档</see></para>
    /// </remarks>
    public long ArRing(SharpValue key, long size, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArRing(key: key, size: size, value: value) | flags, cancellationToken);
    }

    /// <summary>
    /// Inserts values into a ring buffer of specified size, wrapping and truncating as needed.
    /// <para>Available since: 8.8.0</para>
    /// <para>将值插入指定大小的环形缓冲区，根据需要进行循环和截断。</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="size">The size of the ring buffer window. Each value is inserted at insert_idx % size, wrapping back to index 0 when the end of the window is reached. When the buffer is full, newer values overwrite older ones. If size is smaller than the current window, the array is truncated to fit.
    /// <para>环形缓冲区窗口的大小。每个值都会被插入到 insert_idx % size 的位置，当窗口到达末尾时会循环回到索引 0。当缓冲区满时，新值会覆盖旧值。如果大小小于当前窗口，则数组会被截断以适应。</para>
    /// </param>
    /// <param name="values">One or more string values to insert into the ring buffer. Each value is placed at the next position in the ring and the cursor advances accordingly.
    /// <para>一个或多个字符串值要插入到环形缓冲区中。每个值都会被放置到环形缓冲区的下一个位置，同时光标相应地向前移动。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The last index where a value was inserted.
    /// <para>最后一个插入值的索引下标</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arring">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arring">命令文档</see></para>
    /// </remarks>
    public long ArRing(SharpValue key, long size, SharpValueSpan values, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArRing(key: key, size: size, values: values) | flags, cancellationToken);
    }

    /// <summary>
    /// Iterates existing elements in a range, returning index-value pairs.
    /// <para>Available since: 8.8.0</para>
    /// <para>遍历指定范围内的现有元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="start">The zero-based integer index at which to begin scanning. If start is greater than end, elements are returned in reverse index order.
    /// <para>开始扫描的元素下标, 从0开始. 如果开始大于结束, 则按索引逆序返回元素</para>
    /// </param>
    /// <param name="end">The zero-based integer index at which to stop scanning (inclusive).
    /// <para>停止扫描的元素下标</para>
    /// </param>
    /// <param name="limit">The maximum number of index-value pairs to return. When omitted, all elements in the range are returned.
    /// <para>做多返回元素数量. 如果不设置此参数, 则返回索引范围内的所有元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arscan">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arscan">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    ArItem[]?
#else
    ArItem[]
#endif
    ArScan(
        SharpValue key,
        long start,
        long end,
        long? limit = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArScan(key: key, start: start, end: end, limit: limit) | flags, cancellationToken);
    }

    /// <summary>
    /// Iterates existing elements in a range, returning index-value pairs.
    /// <para>Available since: 8.8.0</para>
    /// <para>遍历指定范围内的现有元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="range">The index range of the elements to be scanned
    /// <para>要扫描的元素索引范围. 如果开始大于结束, 则按索引逆序返回元素</para>
    /// </param>
    /// <param name="limit">The maximum number of index-value pairs to return. When omitted, all elements in the range are returned.
    /// <para>做多返回元素数量. 如果不设置此参数, 则返回索引范围内的所有元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arscan">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arscan">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    ArItem[]?
#else
    ArItem[]
#endif
    ArScan(
        SharpValue key,
        RedisArray.Range range,
        long? limit = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArScan(key: key, range: range, limit: limit) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets the ARINSERT / ARRING cursor to a specific index.
    /// <para>Available since: 8.8.0</para>
    /// <para>将 ARINSERT / ARRING 游标设置为指定索引</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="index">The zero-based integer index to set as the new insert cursor position for subsequent ARINSERT calls.
    /// <para>要设置为后续 ARINSERT 调用的新插入光标位置的从零开始的整数索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the cursor was set, False if the key does not exist.
    /// <para>True: 设置成功. False: Key不存在</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arseek">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arseek">命令文档</see></para>
    /// </remarks>
    public bool ArSeek(SharpValue key, long index, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArSeek(key: key, index: index) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns metadata about an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>获得数组的元数据</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="full">When present, includes per-slice statistics in the reply: the number of dense and sparse slices and their average sizes and fill rates. Raises the complexity from O(1) to O(N) where N is the number of slices.
    /// <para>当存在时，回复中包含每片的统计信息：密集片和稀疏片的数量及其平均大小和填充率。将复杂度从 O(1) 提升到 O(N)，其中 N 为片数。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array metadata. Slice statistics are null unless FULL is requested.
    /// <para>数组元数据。FULL 专属统计字段在普通模式下为 null。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arinfo">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arinfo">命令文档</see></para>
    /// </remarks>
    public ArInfoResult ArInfo(SharpValue key, bool full = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ArraysCommands.ArInfo(key, full) | flags, cancellationToken);
    }
}
