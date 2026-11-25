using System;
using System.Collections.Generic;

namespace SharpRedis.Modules;

public sealed partial class RedisBloom
{
    /// <summary>
    /// Represents information about a Bloom filter returned by BF.INFO.
    /// <para>Available since: RedisBloom 1.0.0</para>
    /// <para>表示 BF.INFO 命令返回的布隆过滤器统计信息。</para>
    /// <para>包含容量、内存占用、子过滤器数量、插入数量和扩容因子。</para>
    /// </summary>
    public readonly struct BloomInfo
    {
        /// <summary>
        /// Gets the total capacity of the Bloom filter.
        /// <para>获取布隆过滤器的总容量。</para>
        /// <para>对于可扩容过滤器，该值包含所有子过滤器的容量。</para>
        /// </summary>
        public readonly long Capacity { get; }

        /// <summary>
        /// Gets the memory allocated to the Bloom filter, in bytes.
        /// <para>获取布隆过滤器已分配的内存大小，单位为字节。</para>
        /// </summary>
        public readonly long Size { get; }

        /// <summary>
        /// Gets the number of sub-filters.
        /// <para>获取当前布隆过滤器包含的子过滤器数量。</para>
        /// <para>当过滤器扩容时，此数量可能增加。</para>
        /// </summary>
        public readonly long Filters { get; }

        /// <summary>
        /// Gets the number of items inserted and detected as unique.
        /// <para>获取已添加且被布隆过滤器判定为新元素的数量。</para>
        /// <para>由于布隆过滤器可能产生误判，此值不一定等于实际唯一元素数量。</para>
        /// </summary>
        public readonly long Items { get; }

        /// <summary>
        /// Gets the expansion factor of the Bloom filter.
        /// <para>获取布隆过滤器的扩容因子。</para>
        /// </summary>
        public readonly long? Expansion { get; }

        /// <summary>
        /// Initializes Bloom filter information from a Redis response.
        /// <para>从 BF.INFO 命令返回值解析布隆过滤器统计信息。</para>
        /// </summary>
        internal BloomInfo(RedisResult value)
        {
            if (value.Type is Enums.RespType.Array && value.RawValue is RedisResult[] values && values.Length % 2 == 0)
            {
                for (int i = 0; i < values.Length; i += 2)
                {
                    string field = values[i]!;
                    if ("Capacity".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Capacity = values[i + 1];
                    }
                    else if ("Size".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Size = values[i + 1];
                    }
                    else if ("Number of filters".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Filters = values[i + 1];
                    }
                    else if ("Number of items inserted".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Items = values[i + 1];
                    }
                    else if ("Expansion rate".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Expansion = values[i + 1];
                    }
                }
            }
            else if (value.Type is Enums.RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] maps)
            {
                foreach (var map in maps)
                {
                    string field = map.Key!;
                    if ("Capacity".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Capacity = map.Value;
                    }
                    else if ("Size".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Size = map.Value;
                    }
                    else if ("Number of filters".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Filters = map.Value;
                    }
                    else if ("Number of items inserted".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Items = map.Value;
                    }
                    else if ("Expansion rate".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Expansion = map.Value;
                    }
                }
            }
            else
            {
                throw new InvalidCastException("Invalid BF.INFO response.");
            }
        }
    }

    /// <summary>
    /// Represents a chunk returned by the BF.SCANDUMP command.
    /// <para>表示 BF.SCANDUMP 命令返回的布隆过滤器数据块。</para>
    /// <para>包含下一次扫描所需的游标，以及用于恢复过滤器的二进制数据。</para>
    /// </summary>
    public readonly struct ScanDumpResult
    {
        private readonly long _iterator;
        private readonly byte[] _data;

        /// <summary>
        /// Gets the iterator for the next BF.SCANDUMP call.
        /// <para>获取下一次 BF.SCANDUMP 调用使用的游标。</para>
        /// <para>首次调用传入 0，后续调用传入上一次返回的 Iterator。</para>
        /// <para>返回 0 表示扫描完成。</para>
        /// </summary>
        public readonly long Iterator => this._iterator;

        /// <summary>
        /// Gets the raw binary data of the current Bloom filter chunk.
        /// <para>获取当前布隆过滤器数据块的原始二进制内容。</para>
        /// <para>必须原样保留字节内容，用于 BF.LOADCHUNK 恢复。</para>
        /// <para>扫描结束时通常为空数组。</para>
        /// </summary>
        public readonly byte[] Data => this._data;

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
            this._data = values[1]!;
        }
    }

    /// <summary>
    /// Represents the result status of adding an item to a Bloom filter.
    /// <para>Applicable to BF.MADD, and BF.INSERT commands.</para>
    /// <para>表示向布隆过滤器添加单个元素的结果状态。</para>
    /// <para>适用于BF.MADD 和 BF.INSERT 命令。</para>
    /// </summary>
    public enum AddStatus : byte
    {
        /// <summary>
        /// The item may already exist in the Bloom filter.
        /// <para>The item was not newly inserted due to a possible match.</para>
        /// <para>元素可能已经存在于布隆过滤器中，因此没有新增。</para>
        /// <para>由于布隆过滤器存在误判概率，不能保证元素一定存在。</para>
        /// </summary>
        Exists = 0,

        /// <summary>
        /// The item was successfully added to the Bloom filter.
        /// <para>元素成功添加到布隆过滤器中。</para>
        /// </summary>
        Added = 1,

        /// <summary>
        /// An error occurred while adding the item to the Bloom filter.
        /// <para>For example, a non-scaling Bloom filter has reached its capacity.</para>
        /// <para>向布隆过滤器添加元素时发生错误。</para>
        /// <para>例如：不可扩容的布隆过滤器已达到容量上限。</para>
        /// </summary>
        Error = 2,
    }
}
