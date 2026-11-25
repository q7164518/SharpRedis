using SharpRedis.Enums;
using System;
using System.Collections.Generic;

namespace SharpRedis.Modules;

public sealed partial class RedisCountMinSketch
{
    /// <summary>
    /// Result of a CMS.INCRBY operation.
    /// <para>Available since: RedisBloom 2.0.0</para>
    /// <para>CMS.INCRBY 单个元素的执行结果。</para>
    /// <para>批量操作允许部分成功、部分失败。</para>
    /// </summary>
    public readonly struct IncrByResult
    {
        private readonly long _count;

        /// <summary>
        /// Whether the increment operation succeeded.
        /// <para>本次增量操作是否成功。</para>
        /// </summary>
        public readonly bool IsSuccess => this._error is null;

        /// <summary>
        /// Whether the increment operation failed.
        /// <para>本次增量操作是否失败。</para>
        /// </summary>
        public readonly bool IsError => this._error is not null;

        /// <summary>
        /// Estimated count after the increment.
        /// <para>增量操作后的估算计数值。</para>
        /// <para>如果操作失败，则返回 null。</para>
        /// </summary>
        public readonly long? Count => this.IsSuccess ? this._count : null;

        /// <summary>
        /// Error message, if the operation failed.
        /// <para>操作失败时返回 Redis 错误信息。</para>
        /// <para>成功时返回 null。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? Error => this._error;
        private readonly string? _error;
#else
        public readonly string Error => this._error;
        private readonly string _error;
#endif

        internal IncrByResult(RedisResult value)
        {
            if (value.Type == RespType.SimpleErrors || value.Type == RespType.BulkErrors)
            {
                this._error = value.RawValue?.ToString() ?? "Unknown Redis error.";
            }
            else
            {
                this._count = value;
            }
        }

        /// <summary>
        /// Gets the count if the operation succeeded.
        /// <para>成功时获取计数值。</para>
        /// </summary>
        public bool TryGetCount(out long count)
        {
            count = this._count;
            return this.IsSuccess;
        }

        public override string ToString()
        {
            return this.IsSuccess
                ? this._count.ToString()
                : this._error!;
        }
    }

    /// <summary>
    /// Count-Min Sketch information.
    /// <para>Available since: RedisBloom 2.0.0</para>
    /// <para>Count-Min Sketch 的统计信息。</para>
    /// <para>支持此命令的 RedisBloom 版本：2.0.0+</para>
    /// </summary>
    public readonly struct SketchInfo
    {
        /// <summary>
        /// The width of the sketch.
        /// <para>Sketch 的宽度，即每一行计数器的数量。</para>
        /// <para>宽度越大，计数估算的误差通常越小。</para>
        /// </summary>
        public readonly long Width { get; }

        /// <summary>
        /// The depth of the sketch.
        /// <para>Sketch 的深度，即哈希计数器数组的行数。</para>
        /// <para>深度越大，出现较大估算误差的概率通常越低。</para>
        /// </summary>
        public readonly long Depth { get; }

        /// <summary>
        /// The total count of the sketch.
        /// <para>Sketch 的累计总计数，不是不同元素的数量。</para>
        /// </summary>
        public readonly long Count { get; }

        internal SketchInfo(RedisResult value)
        {
            if (value.Type is RespType.Array && value.RawValue is RedisResult[] values && values.Length % 2 == 0)
            {
                for (int i = 0; i < values.Length; i += 2)
                {
                    string field = values[i]!;
                    if ("width".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Width = values[i + 1];
                    }
                    else if ("depth".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Depth = values[i + 1];
                    }
                    else if ("count".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Count = values[i + 1];
                    }
                }
            }
            else if (value.Type is RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] { Length: > 0 } maps)
            {
                foreach (var map in maps)
                {
                    string field = map.Key!;
                    if ("width".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Width = map.Value;
                    }
                    else if ("depth".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Depth = map.Value;
                    }
                    else if ("count".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Count = map.Value;
                    }
                }
            }
            else
            {
                throw new InvalidCastException("Invalid CMS.INFO response.");
            }
        }
    }
}
