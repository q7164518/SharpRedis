using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpRedis;

public sealed partial class RedisSortedSet
{
    /// <summary>
    /// <b>[ZMPOP] | [BZMPOP]</b> command return value structure
    /// <para><b>[ZMPOP] | [BZMPOP]</b> 命令返回值结构体</para>
    /// </summary>
    public readonly struct SortedSetMPopResult : IEnumerable<KeyValuePair<RedisResult, RedisNumber>>
    {
#if MODERN_NET
        private readonly KeyValuePair<RedisResult, RedisNumber>[]? _value;

        /// <summary>
        /// Gets the popped-out members and the score
        /// <para>获得移除的元素和分数</para>
        /// </summary>
        public readonly KeyValuePair<RedisResult, RedisNumber>[]? Value => this._value;

        /// <summary>
        /// Gets the popped-out members and the score
        /// <para>获得移除的元素和分数</para>
        /// </summary>
        public readonly KeyValuePair<RedisResult, RedisNumber>[]? Members => this._value;
#else
        private readonly KeyValuePair<RedisResult, RedisNumber>[] _value;

        /// <summary>
        /// Gets the popped-out members and the score
        /// <para>获得移除的元素和分数</para>
        /// </summary>
        public readonly KeyValuePair<RedisResult, RedisNumber>[] Value => this._value;

        /// <summary>
        /// Gets the popped-out members and the score
        /// <para>获得移除的元素和分数</para>
        /// </summary>
        public readonly KeyValuePair<RedisResult, RedisNumber>[] Members => this._value;
#endif

        private readonly RedisResult _key;

        /// <summary>
        /// Gets the key of the popped-out member
        /// <para>获得弹出元素的Key</para>
        /// </summary>
        public readonly RedisResult Key => this._key;

        /// <summary>
        /// Indicates whether the value should be considered a null value
        /// <para>当前值是不是应该视为null</para>
        /// </summary>
        public readonly bool IsNull => this._value is null;

        /// <summary>
        /// Indicates whether the value is either null or a zero-length value.
        /// <para>当前值是不是空的, 和null有区别, 比如返回了[]</para>
        /// </summary>
        public readonly bool IsNullOrEmpty => this._value is null || this._value.Length == 0;

        /// <summary>
        /// Indicates whether there is a value at present. As long as it is not null, there is a value.
        /// <para>当前是否有值. 只要不是null, 就是有</para>
        /// </summary>
        public readonly bool HasValue => !this.IsNull;

        /// <summary>
        /// Gets the number of popped-out members
        /// <para>获得弹出的元素数量</para>
        /// </summary>
        public readonly int Length => this._value?.Length ?? -1;

        /// <summary>
        /// Gets the number of popped-out members
        /// <para>获得弹出的元素数量</para>
        /// </summary>
        public readonly int Count => this._value?.Length ?? -1;

        internal SortedSetMPopResult(RedisResult result)
        {
            if (result.IsNullOrEmpty) return;

            if (result.RawValue is not RedisResult[] { Length: 2 } array)
            {
                throw new FormatException("Unrecognizable return value of [ZMPOP] | [BZMPOP] command");
            }

            this._key = array[0];
            this._value = array[1];
        }

        /// <summary>
        /// Retrieve the member based on the subscript
        /// <para>根据下标获取元素</para>
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>element</returns>
        public readonly KeyValuePair<RedisResult, RedisNumber> this[int index]
        {
            get
            {
                if (this.IsNull) throw new NullReferenceException("No members were popped-out.");
                return this._value![index];
            }
        }

        public IEnumerator<KeyValuePair<RedisResult, RedisNumber>> GetEnumerator()
        {
            if (this.IsNull) throw new NullReferenceException("No elements were returned.");
            return new SharpEnumerator<KeyValuePair<RedisResult, RedisNumber>>(this._value!);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

#if MODERN_NET
        public void Deconstruct(out RedisResult key, out KeyValuePair<RedisResult, RedisNumber>[]? members)
#else
        public void Deconstruct(out RedisResult key, out KeyValuePair<RedisResult, RedisNumber>[] members)
#endif
        {
            key = this._key;
            members = this._value;
        }
    }

    /// <summary>
    /// <b>[BZPOPMAX] | [BZPOPMIN]</b> command return value structure
    /// <para><b>[BZPOPMAX] | [BZPOPMIN]</b> 命令返回值结构体</para>
    /// </summary>
    public readonly struct SortedSetPopResult
    {
        private readonly RedisResult _member;
        private readonly RedisNumber _score;
        private readonly RedisResult _key;

        /// <summary>
        /// Gets the key of the popped-out member
        /// <para>获得弹出元素的Key</para>
        /// </summary>
        public readonly RedisResult Key => this._key;

        /// <summary>
        /// Gets the popped-out member
        /// <para>获得弹出的元素</para>
        /// </summary>
        public readonly RedisResult Member => this._member;

        /// <summary>
        /// Gets the score of the popped-out member
        /// <para>获得弹出的元素分数</para>
        /// </summary>
        public readonly RedisNumber Score => this._score;

        /// <summary>
        /// Indicates whether the value should be considered a null value
        /// <para>当前值是不是应该视为null</para>
        /// </summary>
        public readonly bool IsNull => this._key.IsNull;

        /// <summary>
        /// Indicates whether there is a value at present. As long as it is not null, there is a value.
        /// <para>当前是否有值. 只要不是null, 就是有</para>
        /// </summary>
        public readonly bool HasValue => !this.IsNull;

        internal SortedSetPopResult(RedisResult result)
        {
            if (result.IsNullOrEmpty) return;

            if (result.RawValue is not RedisResult[] { Length: 3 } array)
            {
                throw new FormatException("Unrecognizable return value of [BZPOPMAX] | [BZPOPMIN] command");
            }

            this._key = array[0];
            this._member = array[1];
            this._score = array[2];
        }

        public void Deconstruct(out RedisResult key, out RedisResult member, out RedisNumber score)
        {
            if (!this.HasValue) throw new NullReferenceException();
            key = this._key;
            member = this._member;
            score = this._score;
        }
    }

    /// <summary>
    /// The <b>[ZRANK] or [ZREVRANK]</b> command includes a structure that returns the score value.
    /// <para><b>[ZRANK] 或 [ZREVRANK]</b> 命令, 在指定了包含分数返回参数时的返回值结构体</para>
    /// </summary>
    public readonly struct SortedSetRankResult
    {
        private readonly long _rank = -1;
        private readonly RedisNumber _score = RedisNumber._zero;

        /// <summary>
        /// Indicates whether the value should be considered a null value
        /// <para>当前值是不是应该视为null</para>
        /// </summary>
        public readonly bool IsNull => this._rank is -1;

        /// <summary>
        /// Gets a value indicating whether the current SortedSetRankResult object has a valid value.
        /// <para>获取一个值, 表示当前SortedSetRankResult是否有一个有效的值</para>
        /// </summary>
        public readonly bool HasValue => this._rank >= 0;

        /// <summary>
        /// Gets the rank
        /// <para>获取排名</para>
        /// </summary>
        public readonly long Rank => this._rank;

        /// <summary>
        /// Gets the score of the member
        /// <para>获得元素的分数</para>
        /// </summary>
        public readonly RedisNumber Score => this._score;

        internal SortedSetRankResult(RedisResult value)
        {
            if (value.IsNullOrEmpty) return;
            if (value.RawValue is not RedisResult[] { Length: 2 } array)
            {
                throw new FormatException("Unrecognizable return value of [ZRANK] or [ZREVRANK] command");
            }
            this._rank = array[0];
            this._score = array[1];
        }

        public void Deconstruct(out long rank, out RedisNumber score)
        {
            if (this.IsNull) throw new NullReferenceException();
            rank = this._rank;
            score = this._score;
        }
    }
}
