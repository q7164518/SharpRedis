using System;
using System.Collections.Generic;

namespace SharpRedis;

public sealed partial class RedisString
{
    /// <summary>
    /// The return value structure of the <b>INCREX</b> command
    /// <para><b>INCREX</b>命令返回值结构体</para>
    /// </summary>
    public readonly struct IncrExResult
    {
        private readonly RedisNumber _value;
        private readonly RedisNumber _increment;

        /// <summary>
        /// The value of the key after the increment, or the unchanged current value when an out-of-bounds result caused the operation to be skipped.
        /// <para>获得递增之后的数值</para>
        /// </summary>
        public readonly RedisNumber Value => this._value;

        /// <summary>
        /// The increment that was actually applied. May differ from the requested increment when SATURATE caps the result at a bound, and is always 0 when an out-of-bounds result caused the operation to be skipped.
        /// <para>获得递增的值(本次操作递增了多少).</para>
        /// </summary>
        public readonly RedisNumber Increment => this._increment;

        internal IncrExResult(RedisResult value)
        {
            if (value.RawValue is not RedisResult[] { Length: 2 } array)
            {
                throw new InvalidCastException("Unrecognizable return value of [INCREX] command");
            }
            this._value = array[0];
            this._increment = array[1];
        }

        public override string ToString()
        {
            return $"New Value: {this._value}, Actual increment: {this._increment}";
        }
    }

    /// <summary>
    /// The indices of the substrings that match in each group
    /// <para>每组子字符串匹配的下标</para>
    /// </summary>
    public readonly struct RedisLcsResult
    {
        private readonly long _length;
        private readonly RedisLcsResultItem[] _matches;

        /// <summary>
        /// Get the length of the longest common subsequence
        /// <para>获得最长公共子序列的长度</para>
        /// </summary>
        public readonly long Length => this._length;

        /// <summary>
        /// Get the matched positions of all the sub-matched strings.
        /// <para>获得所有子字符串匹配的位置下标信息</para>
        /// </summary>
        public readonly RedisLcsResultItem[] Matches => this._matches;

        public RedisLcsResult()
        {
            this._matches = null!;
        }

        internal RedisLcsResult(RedisResult result)
        {
            if (!result.HasValue) throw new InvalidCastException("Failed to obtain the return value of the LCS command.");

#if MODERN_NET
            RedisResult[]? matches = null;
#else
            RedisResult[] matches = null;
#endif
            if (result.Type is Enums.RespType.Maps)
            {
                if (result.RawValue is not KeyValuePair<RedisResult, RedisResult>[] maps)
                {
                    throw new InvalidCastException("Unrecognizable return value of LCS command.");
                }
                foreach (var item in maps)
                {
                    var sKey = item.Key.ToString();
                    if ("len".Equals(sKey, StringComparison.OrdinalIgnoreCase))
                    {
                        this._length = item.Value;
                    }
                    else if ("matches".Equals(sKey, StringComparison.OrdinalIgnoreCase))
                    {
                        if (item.Value.RawValue is not RedisResult[] matchesArray)
                        {
                            throw new InvalidCastException("Unrecognizable LCS command return value, incorrect return value type of \"matches\".");
                        }
                        matches = matchesArray;
                    }
                }
            }
            else if (result.Type is Enums.RespType.Array)
            {
                if (result.RawValue is not RedisResult[] array || array.Length != 4)
                {
                    throw new InvalidCastException("Unrecognizable return value type of LCS command.");
                }
                this._length = array[3];
                if (array[1].RawValue is not RedisResult[] matchesArray)
                {
                    throw new InvalidCastException("Unrecognizable LCS command return value, incorrect return value type of \"matches\".");
                }
                matches = matchesArray;
            }
            else
            {
                throw new InvalidCastException("Unrecognizable return value type of LCS command.");
            }

            if (matches is null)
            {
                throw new InvalidCastException("Unable to obtain the \"matches\" return value");
            }
            this._matches = new RedisLcsResultItem[matches.Length];
            int matchesIndex = 0;
            for (int i = matches.Length - 1; i >= 0; i--)
            {
                var matcheItem = matches[i].RawValue;
                if (matcheItem is not RedisResult[] items)
                {
                    throw new InvalidCastException("Unrecognizable LCS command return value, incorrect return value type of \"matches\".");
                }
                var firstItem = items[0];
                var secondItem = items[1];
                if (firstItem.RawValue is not RedisResult[] firstIndexArray || secondItem.RawValue is not RedisResult[] secondIndexArray)
                {
                    throw new InvalidCastException("Unrecognizable LCS command return value, incorrect return value type of \"matches\".");
                }
                var firstIndex = new RedisLcsIndex(firstIndexArray[0], firstIndexArray[1]);
                var secondIndex = new RedisLcsIndex(secondIndexArray[0], secondIndexArray[1]);
                if (items.Length is 3)
                {
                    //with length
                    this._matches[matchesIndex++] = new(firstIndex, secondIndex, items[2]);
                }
                else
                {
                    this._matches[matchesIndex++] = new(firstIndex, secondIndex);
                }
            }
        }

        public readonly struct RedisLcsResultItem
        {
            private readonly RedisLcsIndex _first;
            private readonly RedisLcsIndex _second;
            private readonly long? _length;

            internal RedisLcsResultItem(RedisLcsIndex first, RedisLcsIndex second, long? length = null)
            {
                this._first = first;
                this._second = second;
                this._length = length;
            }

            /// <summary>
            /// The first index of the matched substring in the first Key.
            /// <para>获得在第一个Key中匹配的子序列位置索引</para>
            /// </summary>
            public readonly RedisLcsIndex FirstIndex => this._first;

            /// <summary>
            /// The first index of the matched substring in the second Key.
            /// <para>获得在第二个Key中匹配的子序列位置索引</para>
            /// </summary>
            public readonly RedisLcsIndex SecondIndex => this._second;

            /// <summary>
            /// Get the length of the current substring. If the WITHMATCHLEN parameter is not specified, return null.
            /// <para>获得当前子字符串的长度. 如果没有指定WITHMATCHLEN参数, 返回null</para>
            /// </summary>
            public readonly long? Length => this._length;
        }

        public readonly struct RedisLcsIndex
        {
            private readonly long _start;
            private readonly long _end;

            internal RedisLcsIndex(long start, long end)
            {
                this._start = start;
                this._end = end;
            }

            /// <summary>
            /// Obtain the starting index (including the current index) of the current subsequence in the Key
            /// <para>获得当前子序列在Key中的开始下标 (包含当前下标)</para>
            /// </summary>
            public readonly long Start => this._start;

            /// <summary>
            /// Obtain the ending index (including the current index) of the current subsequence in the Key
            /// <para>获得当前子序列在Key中的结束下标 (包含当前下标)</para>
            /// </summary>
            public readonly long End => this._end;

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            public static implicit operator Range(RedisLcsIndex index)
                => new(new((int)index.Start), new((int)(index.End + 1)));
#endif
        }
    }
}
