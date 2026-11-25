using System;
using System.Collections.Generic;
using System.Text;

namespace SharpRedis;

public sealed partial class RedisArray
{
    /// <summary>
    /// A matching array index and its optionally requested value.
    /// <para>匹配的数组索引及按需返回的元素值。</para>
    /// </summary>
    public readonly struct ArItem : IEquatable<ArItem>
    {
        /// <summary>Matching array index.
        /// <para>匹配的数组索引。</para>
        /// </summary>
        public readonly long Index { get; }

        /// <summary>Element value, or RedisResult.Empty when WITHVALUES was omitted.
        /// <para>元素值，省略 WITHVALUES 时为 RedisResult.Empty。</para>
        /// </summary>
        public readonly RedisResult Value { get; }

        internal ArItem(RedisResult row)
        {
            if (row.RawValue is RedisResult[] pair)
            {
                this.Index = pair[0];
                this.Value = pair[1];
            }
            else
            {
                this.Index = row;
                this.Value = RedisResult.Empty;
            }
        }

        public bool Equals(ArItem other) => this.Index == other.Index && this.Value.Equals(other.Value);

#if MODERN_NET
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
#else
        public override bool Equals(object obj)
#endif
            => obj is ArItem other && this.Equals(other);

        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            return unchecked(this.Index.GetHashCode() * 31 + this.Value.GetHashCode());
#else
            return HashCode.Combine(this.Index, this.Value);
#endif
        }

        public static bool operator ==(ArItem left, ArItem right) => left.Equals(right);

        public static bool operator !=(ArItem left, ArItem right) => !left.Equals(right);
    }

    /// <summary>
    /// Array metadata returned by ARINFO.
    /// <para>ARINFO 返回的数组元数据。</para>
    /// </summary>
    public readonly struct ArInfoResult
    {
        /// <summary>
        /// Number of stored elements.
        /// <para>已存储的元素数。</para>
        /// </summary>
        public readonly ulong Count { get; }

        /// <summary>
        /// Logical array length.
        /// <para>数组逻辑长度。</para>
        /// </summary>
        public readonly ulong Length { get; }

        /// <summary>
        /// Index for the next insertion.
        /// <para>下次插入的索引。</para>
        /// </summary>
        public readonly ulong NextInsertIndex { get; }

        /// <summary>
        /// Number of allocated slices.
        /// <para>已分配的切片数。</para>
        /// </summary>
        public readonly long Slices { get; }

        /// <summary>
        /// Allocated directory capacity.
        /// <para>已分配的目录容量。</para>
        /// </summary>
        public readonly long DirectorySize { get; }

        /// <summary>
        /// Number of super-directory entries.
        /// <para>超级目录的条目数。</para>
        /// </summary>
        public readonly long SuperDirEntries { get; }

        /// <summary>
        /// Number of indices covered by a slice.
        /// <para>每个切片覆盖的索引数。</para>
        /// </summary>
        public readonly long SliceSize { get; }

        /// <summary>
        /// Number of dense slices. Only returned by FULL.
        /// <para>密集切片数。仅 FULL 模式返回。</para>
        /// </summary>
        public readonly long? DenseSlices { get; }

        /// <summary>
        /// Number of sparse slices. Only returned by FULL.
        /// <para>稀疏切片数。仅 FULL 模式返回。</para>
        /// </summary>
        public readonly long? SparseSlices { get; }

        /// <summary>
        /// Average dense slice window size. Only returned by FULL.
        /// <para>密集切片的平均窗口大小。仅 FULL 模式返回。</para>
        /// </summary>
        public readonly double? AvgDenseSize { get; }

        /// <summary>
        /// Fraction of dense window slots in use. Only returned by FULL.
        /// <para>密集切片窗口中已使用位置的比例。仅 FULL 模式返回。</para>
        /// </summary>
        public readonly double? AvgDenseFill { get; }

        /// <summary>
        /// Average sparse slice capacity. Only returned by FULL.
        /// <para>稀疏切片的平均容量。仅 FULL 模式返回。</para>
        /// </summary>
        public readonly double? AvgSparseSize { get; }

        internal ArInfoResult(RedisResult result)
        {
            ulong count = 0, length = 0, nextInsertIndex = 0;
            long slices = 0, directorySize = 0, superDirEntries = 0, sliceSize = 0;
            long? denseSlices = null, sparseSlices = null;
            double? avgDenseSize = null, avgDenseFill = null, avgSparseSize = null;

            RedisServer.AclReplyReader.ReadFields(result, (name, value) =>
            {
                switch (name)
                {
                    case "count": count = value; break;
                    case "len": length = value; break;
                    case "next-insert-index": nextInsertIndex = value; break;
                    case "slices": slices = value; break;
                    case "directory-size": directorySize = value; break;
                    case "super-dir-entries": superDirEntries = value; break;
                    case "slice-size": sliceSize = value; break;
                    case "dense-slices": denseSlices = value; break;
                    case "sparse-slices": sparseSlices = value; break;
                    case "avg-dense-size": avgDenseSize = ReadAverage(value); break;
                    case "avg-dense-fill": avgDenseFill = ReadAverage(value); break;
                    case "avg-sparse-size": avgSparseSize = ReadAverage(value); break;
                }
            });

            this.Count = count;
            this.Length = length;
            this.NextInsertIndex = nextInsertIndex;
            this.Slices = slices;
            this.DirectorySize = directorySize;
            this.SuperDirEntries = superDirEntries;
            this.SliceSize = sliceSize;
            this.DenseSlices = denseSlices;
            this.SparseSlices = sparseSlices;
            this.AvgDenseSize = avgDenseSize;
            this.AvgDenseFill = avgDenseFill;
            this.AvgSparseSize = avgSparseSize;
        }

        private static double ReadAverage(RedisResult value)
            => value.RawValue is string text
                ? double.Parse(text, System.Globalization.CultureInfo.InvariantCulture)
                : (double)value;
    }
}
