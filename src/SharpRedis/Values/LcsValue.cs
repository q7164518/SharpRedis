#pragma warning disable IDE0130
#if NET8_0
#pragma warning disable IDE0305
#pragma warning disable IDE0300
#endif
using System.Collections;
using System.Collections.Generic;

namespace SharpRedis
{
    /// <summary>
    /// LCS Result
    /// </summary>
    public sealed class LcsValue : IEnumerable<long>
    {
        public long Len { get; }
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public LcsItem[]? Matches { get; }
#else
        public LcsItem[] Matches { get; }
#endif

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private LcsValue(long len, LcsItem[]? matches)
#else
        private LcsValue(long len, LcsItem[] matches)
#endif
        {
            this.Len = len;
            this.Matches = matches;
        }

        public override int GetHashCode()
        {
            if (this.Matches is null) return -1;
            return this.Matches.GetHashCode() ^ this.Len.GetHashCode();
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public long[][][]? ToArray()
#else
        public long[][][] ToArray()
#endif
        {
            if (this.Matches == null) return null;
            var result = new long[this.Matches.Length][][];
            for (uint i = 0; i < this.Matches.Length; i++)
            {
                result[i] = this.Matches[i].Matches;
            }
            return result;
        }

        public IEnumerator<long> GetEnumerator()
        {
            if (this.Matches is null || this.Matches.Length == 0) yield break;
            for (uint i = 0; i < this.Matches.Length; i++)
            {
                yield return this.Matches[i].Matches[0][0];
                yield return this.Matches[i].Matches[0][1];
                yield return this.Matches[i].Matches[1][0];
                yield return this.Matches[i].Matches[1][1];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static implicit operator long[][][]?(LcsValue? lcsValue)
#else
        public static implicit operator long[][][](LcsValue lcsValue)
#endif
        {
            if (lcsValue is null || lcsValue.Matches is null) return null;
            return lcsValue.ToArray();
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static LcsValue Parse(Dictionary<string, object?> dict)
#else
        internal static LcsValue Parse(Dictionary<string, object> dict)
#endif
        {
            if (dict.Count == 0)
            {
                return new LcsValue(0, null);
            }
            long len = 0;
            if (dict.TryGetValue("len", out var lenVal))
            {
                if (lenVal is NumberValue numberValue)
                {
                    len = numberValue.ToInt64();
                }
                else if (lenVal is string numberString)
                {
                    len = long.Parse(numberString);
                }
            }
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            LcsItem[]? matches = null;
#else
            LcsItem[] matches = null;
#endif
            if (dict.TryGetValue("matches", out var matchesVal) && matchesVal != null)
            {
#if NET5_0_OR_GREATER
                if (matchesVal is not object[] matchesValue || matchesValue.Length == 0) matches = null;
#else
                if (!(matchesVal is object[] matchesValue) || matchesValue.Length == 0) matches = null;
#endif
                else
                {
                    matches = new LcsItem[matchesValue.Length];
                    for (uint i = 0; i < matchesValue.Length; i++)
                    {
                        if (matchesValue[i] is object[] l1
                            && l1[0] is object[] l2
                            && l1[1] is object[] l3)
                        {
                            LcsMatchLen lcsMatchLen;
                            if (l1.Length == 3)
                            {
                                if (l1[2] is NumberValue numberValue)
                                {
                                    lcsMatchLen = new LcsMatchLen(true, numberValue.ToInt64());
                                }
                                else if (l1[2] is string matchLenString)
                                {
                                    lcsMatchLen = new LcsMatchLen(true, long.Parse(matchLenString));
                                }
                                else
                                {
                                    lcsMatchLen = new LcsMatchLen(false, -1);
                                }
                            }
                            else
                            {
                                lcsMatchLen = new LcsMatchLen(false, -1);
                            }

                            if (l2[0] is NumberValue numberValue1
                                && l2[1] is NumberValue numberValue2
                                && l3[0] is NumberValue numberValue3
                                && l3[1] is NumberValue numberValue4)
                            {
                                matches[i] = new LcsItem(new long[][] { new long[] { numberValue1.ToInt64(), numberValue2.ToInt64() }, new long[] { numberValue3.ToInt64(), numberValue4.ToInt64() } }, lcsMatchLen);
                            }
                            else if (l2[0] is string numString1 && long.TryParse(numString1, out var num1)
                                && l2[1] is string numString2 && long.TryParse(numString2, out var num2)
                                && l3[0] is string numString3 && long.TryParse(numString3, out var num3)
                                && l3[1] is string numString4 && long.TryParse(numString4, out var num4))
                            {
                                matches[i] = new LcsItem(new long[][] { new long[] { num1, num2 }, new long[] { num3, num4 } }, lcsMatchLen);
                            }
                            else
                            {
                                matches[i] = new LcsItem(new long[][] { new long[] { -1, -1 }, new long[] { -1, -1 } }, lcsMatchLen);
                            }
                        }
                    }
                }
            }
            return new LcsValue(len, matches);
        }
    }

    public sealed class LcsItem
    {
        public LcsMatchLen MatchLen { get; }
        public long[][] Matches { get; }

        internal LcsItem(long[][] matches, LcsMatchLen matchLen)
        {
            this.MatchLen = matchLen;
            this.Matches = matches;
        }

        public override int GetHashCode()
        {
            return this.Matches.GetHashCode() ^ this.MatchLen.GetHashCode();
        }
    }

    public readonly struct LcsMatchLen
    {
        public bool WithMatchLen { get; }
        public long MatchLen { get; }

        internal LcsMatchLen(bool withMatchLen, long matchLen)
        {
            this.WithMatchLen = withMatchLen;
            this.MatchLen = matchLen;
        }

        public override int GetHashCode()
        {
            return this.MatchLen.GetHashCode() ^ this.WithMatchLen.GetHashCode();
        }
    }
}
