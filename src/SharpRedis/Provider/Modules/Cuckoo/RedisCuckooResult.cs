using System;
using System.Collections.Generic;

namespace SharpRedis.Modules;

public sealed partial class RedisCuckoo
{
    /// <summary>
    /// Represents a chunk returned by the CF.SCANDUMP command.
    /// <para>表示 CF.SCANDUMP 命令返回的布谷鸟过滤器数据块。</para>
    /// <para>包含下一次扫描所需的游标，以及用于恢复过滤器的二进制数据。</para>
    /// </summary>
    public readonly struct ScanDumpResult
    {
        private readonly long _iterator;

        /// <summary>
        /// Gets the iterator for the next CF.SCANDUMP call.
        /// <para>获取下一次 CF.SCANDUMP 调用使用的游标。</para>
        /// <para>首次调用传入 0，后续调用传入上一次返回的 Iterator。</para>
        /// <para>返回 0 表示扫描完成。</para>
        /// </summary>
        public readonly long Iterator => this._iterator;

        /// <summary>
        /// Gets the raw binary data of the current Cuckoo filter chunk.
        /// <para>获取当前布谷鸟过滤器数据块的原始二进制内容。</para>
        /// <para>必须原样保留字节内容，用于 CF.LOADCHUNK 恢复。</para>
        /// <para>扫描结束时通常为null数组。</para>
        /// </summary>
#if MODERN_NET
        public readonly byte[]? Data => this._data;
        private readonly byte[]? _data;
#else
        public readonly byte[] Data => this._data;
        private readonly byte[] _data;
#endif

        /// <summary>
        /// Gets whether the scan has completed.
        /// <para>获取扫描是否已经完成。</para>
        /// <para>当 Iterator 为 0 时表示扫描结束。</para>
        /// </summary>
        public bool IsCompleted => this._iterator == 0;

        internal ScanDumpResult(RedisResult value)
        {
            if (value.RawValue is not RedisResult[] { Length: 2 } values)
            {
                throw new InvalidCastException("Invalid BF.SCANDUMP response.");
            }

            this._iterator = values[0];
            this._data = values[1];
        }
    }

    /// <summary>
    /// Represents information about a Cuckoo filter returned by CF.INFO.
    /// <para>Available since: RedisBloom 1.0.0</para>
    /// <para>表示 CF.INFO 命令返回的布谷鸟过滤器统计信息。</para>
    /// <para>包含内存大小、桶数量、子过滤器数量、元素数量及扩容配置等信息。</para>
    /// </summary>
    public readonly struct CuckooInfo
    {
        /// <summary>
        /// Gets the memory allocated to the Cuckoo filter, in bytes.
        /// <para>获取布谷鸟过滤器占用的内存大小，单位为字节。</para>
        /// </summary>
        public readonly long Size { get; }

        /// <summary>
        /// Gets the total number of buckets in the Cuckoo filter.
        /// <para>获取布谷鸟过滤器的桶数量。</para>
        /// </summary>
        public readonly long Buckets { get; }

        /// <summary>
        /// Gets the number of sub-filters.
        /// <para>获取布谷鸟过滤器包含的子过滤器数量。</para>
        /// <para>过滤器扩容时，该值可能增加。</para>
        /// </summary>
        public readonly long Filters { get; }

        /// <summary>
        /// Gets the number of items inserted into the Cuckoo filter.
        /// <para>获取布谷鸟过滤器累计插入的元素数量。</para>
        /// <para>布谷鸟过滤器允许重复插入相同元素。</para>
        /// </summary>
        public readonly long Items { get; }

        /// <summary>
        /// Gets the number of items deleted from the Cuckoo filter.
        /// <para>获取布谷鸟过滤器已删除的元素数量。</para>
        /// </summary>
        public readonly long DeletedItems { get; }

        /// <summary>
        /// Gets the number of slots in each bucket.
        /// <para>获取每个桶可以存储的指纹数量。</para>
        /// <para>默认值为 2。</para>
        /// </summary>
        public readonly long BucketSize { get; }

        /// <summary>
        /// Gets the expansion factor of the Cuckoo filter.
        /// <para>获取布谷鸟过滤器的扩容因子。</para>
        /// <para>决定创建新子过滤器时容量的增长倍数。</para>
        /// </summary>
        public readonly long Expansion { get; }

        /// <summary>
        /// Gets the maximum number of relocation iterations.
        /// <para>获取插入元素时允许的最大重定位迭代次数。</para>
        /// <para>默认值为 20。</para>
        /// </summary>
        public readonly long MaxIterations { get; }

        /// <summary>
        /// Initializes Cuckoo filter information from a Redis response.
        /// <para>从 CF.INFO 命令返回值解析布谷鸟过滤器统计信息。</para>
        /// </summary>
        internal CuckooInfo(RedisResult value) : this()
        {
            if (value.Type is Enums.RespType.Array &&
                value.RawValue is RedisResult[] values &&
                values.Length % 2 == 0)
            {
                for (int i = 0; i < values.Length; i += 2)
                {
                    string field = values[i]!;

                    if ("Size".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Size = values[i + 1];
                    }
                    else if ("Number of buckets".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Buckets = values[i + 1];
                    }
                    else if ("Number of filters".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Filters = values[i + 1];
                    }
                    else if ("Number of items inserted".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Items = values[i + 1];
                    }
                    else if ("Number of items deleted".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.DeletedItems = values[i + 1];
                    }
                    else if ("Bucket size".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.BucketSize = values[i + 1];
                    }
                    else if ("Expansion rate".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Expansion = values[i + 1];
                    }
                    else if ("Max iterations".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.MaxIterations = values[i + 1];
                    }
                }
            }
            else if (value.Type is Enums.RespType.Maps &&
                value.RawValue is KeyValuePair<RedisResult, RedisResult>[] maps)
            {
                foreach (var map in maps)
                {
                    string field = map.Key!;

                    if ("Size".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Size = map.Value;
                    }
                    else if ("Number of buckets".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Buckets = map.Value;
                    }
                    else if ("Number of filters".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Filters = map.Value;
                    }
                    else if ("Number of items inserted".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Items = map.Value;
                    }
                    else if ("Number of items deleted".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.DeletedItems = map.Value;
                    }
                    else if ("Bucket size".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.BucketSize = map.Value;
                    }
                    else if ("Expansion rate".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Expansion = map.Value;
                    }
                    else if ("Max iterations".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.MaxIterations = map.Value;
                    }
                }
            }
            else
            {
                throw new InvalidCastException("Invalid CF.INFO response.");
            }
        }
    }

    /// <summary>
    /// Represents the result status of inserting an item into a Cuckoo filter
    /// only if the item does not already exist.
    /// <para>表示 CF.INSERTNX 命令中单个元素的插入结果状态。</para>
    /// <para>仅当元素可能不存在时才执行插入。</para>
    /// </summary>
    public enum InsertNxStatus : sbyte
    {
        /// <summary>
        /// The item could not be inserted because the Cuckoo filter is full.
        /// <para>布谷鸟过滤器容量已满，元素插入失败。</para>
        /// </summary>
        Full = -1,

        /// <summary>
        /// The item's fingerprint may already exist in the Cuckoo filter.
        /// <para>The item was not inserted.</para>
        /// <para>元素指纹可能已经存在，因此没有执行插入。</para>
        /// <para>由于布谷鸟过滤器存在误判概率，不能保证元素一定存在。</para>
        /// </summary>
        Exists = 0,

        /// <summary>
        /// The item was successfully inserted into the Cuckoo filter.
        /// <para>元素成功插入布谷鸟过滤器。</para>
        /// </summary>
        Added = 1,
    }
}
