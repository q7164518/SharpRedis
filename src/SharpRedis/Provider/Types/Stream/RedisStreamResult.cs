using SharpRedis.Enums;
using System;
using System.Collections.Generic;

namespace SharpRedis;

public partial class RedisStream
{
    /// <summary>
    /// The return value of the <b>XREAD</b> command
    /// <para><b>XREAD</b> 命令返回值</para>
    /// </summary>
    public readonly struct XReadResult
    {
        private readonly RedisResult _name;
#if MODERN_NET
        private readonly EntryItem[]? _entries;
#else
        private readonly EntryItem[] _entries;
#endif

        /// <summary>
        /// Gets the Stream key for the returned message
        /// <para>获得返回消息的Stream key</para>
        /// </summary>
        public readonly RedisResult Name => this._name;

        /// <summary>
        /// Gets the returned message entries
        /// <para>获得返回的消息条目</para>
        /// </summary>
#if MODERN_NET
        public readonly EntryItem[]? Entries => this._entries;
#else
        public readonly EntryItem[] Entries => this._entries;
#endif

        internal XReadResult(RedisResult value)
        {
            if (value.Type is Enums.RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] { Length: 1 } kvs)
            {
                var item = kvs[0];
                this._name = item.Key;
                if (item.Value.RawValue is RedisResult[] { Length: > 0 } items)
                {
                    this._entries = new EntryItem[items.Length];
                    for (int i = 0; i < this._entries.Length; i++)
                    {
                        this._entries[i] = new(items[i]);
                    }
                }
            }
            else if (value.RawValue is RedisResult[] { Length: 1 } array)
            {
                var item = array[0];
                if (item.RawValue is RedisResult[] { Length: 2 } kv)
                {
                    this._name = kv[0];
                    if (kv[1].RawValue is RedisResult[] { Length: > 0 } items)
                    {
                        this._entries = new EntryItem[items.Length];
                        for (int i = 0; i < this._entries.Length; i++)
                        {
                            this._entries[i] = new(items[i]);
                        }
                    }
                }
            }
            else
            {
                throw new InvalidCastException("Unrecognizable data cannot be converted into XReadResult");
            }
        }

        internal XReadResult(KeyValuePair<RedisResult, RedisResult> kv)
        {
            this._name = kv.Key;
            if (kv.Value.RawValue is RedisResult[] { Length: > 0 } items)
            {
                this._entries = new EntryItem[items.Length];
                for (int i = 0; i < this._entries.Length; i++)
                {
                    this._entries[i] = new(items[i]);
                }
            }
        }

        internal XReadResult(RedisResult[] kv)
        {
            this._name = kv[0];
            if (kv[1].RawValue is RedisResult[] { Length: > 0 } items)
            {
                this._entries = new EntryItem[items.Length];
                for (int i = 0; i < this._entries.Length; i++)
                {
                    this._entries[i] = new(items[i]);
                }
            }
        }
    }

    /// <summary>
    /// Represents a message entry in the Stream
    /// <para>表示Stream中的一个消息条目</para>
    /// </summary>
    public readonly struct EntryItem
    {
        private readonly string _id;
        public readonly long? _idleTime;
        public readonly long? _deliveryCount;

        /// <summary>
        /// Gets the content of the message entry
        /// <para>获得消息内容</para>
        /// </summary>
#if MODERN_NET
        public readonly KeyValuePair<RedisResult, RedisResult>[]? FieldValue => this._fieldValue;
        private readonly KeyValuePair<RedisResult, RedisResult>[]? _fieldValue;
#else
        public readonly KeyValuePair<RedisResult, RedisResult>[] FieldValue => this._fieldValue;
        private readonly KeyValuePair<RedisResult, RedisResult>[] _fieldValue;
#endif

        /// <summary>
        /// Gets the message ID
        /// <para>获得消息ID</para>
        /// </summary>
        public readonly string Id => this._id;

        /// <summary>
        /// When CLAIM min-idle-time is used, gets milliseconds since the last time this entry was delivered
        /// <para>当使用了CLAIM参数时, 获取上次投递至今的时间间隔, 单位: 毫秒</para>
        /// </summary>
        public readonly long? IdleTime => this._idleTime;

        /// <summary>
        /// When CLAIM min-idle-time is used, gets number of times this entry was previously delivered
        /// <para>当使用了CLAIM参数时, 获取在此次读取之前, 该消息累计被读取的次数</para>
        /// </summary>
        public readonly long? DeliveryCount => this._deliveryCount;

        internal EntryItem(RedisResult value)
        {
            if (value.Type == RespType.BulkStrings)
            {
                this._id = value!;
                return;
            }
            if (value.RawValue is not RedisResult[] { Length: 2 or 4 } array
                || array[1].RawValue is not RedisResult[] values
                || values.Length % 2 != 0)
            {
                throw new InvalidCastException("Unrecognizable data cannot be converted into XReadItem");
            }
            this._id = array[0]!;
            this._fieldValue = new KeyValuePair<RedisResult, RedisResult>[values.Length / 2];
            for (int i = 0, index = 0; i < values.Length; i += 2, index++)
            {
                this._fieldValue[index] = new(values[i], values[i + 1]);
            }
            if (array.Length is 4)
            {
                this._idleTime = array[2];
                this._deliveryCount = array[3];
            }
        }
    }

    /// <summary>
    /// Represents the result of the Redis XAUTOCLAIM command.
    /// <para>Contains the next cursor, successfully claimed entries, and deleted entry IDs.</para>
    /// <para>表示 Redis XAUTOCLAIM 命令的执行结果。</para>
    /// <para>包含下一次扫描的起始 ID、成功认领的消息以及已从 PEL 中清理的消息 ID。</para>
    /// </summary>
    public readonly struct XAutoClaimResult
    {
        /// <summary>
        /// Gets the starting ID for the next XAUTOCLAIM call.
        /// <para>A value of "0-0" indicates that the current scan has reached the end of the PEL.</para>
        /// <para>It does not necessarily mean that all pending messages have been claimed.</para>
        /// <para>获取下一次执行 XAUTOCLAIM 命令时使用的起始消息 ID。</para>
        /// <para>值为 "0-0" 表示本次扫描已到达待处理消息列表（PEL）的末尾。</para>
        /// <para>不代表所有待处理消息均已成功认领。</para>
        /// </summary>
        public readonly string NextStartId => this._nextStartId;
        private readonly string _nextStartId;

        /// <summary>
        /// Gets the IDs of entries that no longer exist in the Stream and were removed from the PEL. <b>Available since Redis 7.0.0, Otherwise, return null.</b>
        /// <para>These entries were previously pending but their message data had been deleted.</para>
        /// <para>May be empty if no stale PEL references were found.</para>
        /// <para>获取已不存在于 Stream 中、并且已从待处理消息列表（PEL）中清理的消息 ID。<b>自 Redis 7.0.0 起可用，否则返回 null。</b></para>
        /// <para>这些消息此前仍存在待确认引用，但实际消息内容已经被删除。</para>
        /// <para>如果没有发现需要清理的失效 PEL 引用，则集合可能为空。</para>
        /// </summary>
#if MODERN_NET
        public readonly string[]? DeletedIds => this._deletedIds;
        private readonly string[]? _deletedIds;
#else
        public readonly string[] DeletedIds => this._deletedIds;
        private readonly string[] _deletedIds;
#endif

        /// <summary>
        /// Gets the successfully claimed Stream entries.
        /// <para>Each entry contains its message ID and field-value pairs, in the same format as XRANGE.</para>
        /// <para>May be empty if no entries were successfully claimed.</para>
        /// <para>获取成功认领的 Stream 消息集合。</para>
        /// <para>每条消息包含消息 ID 以及字段和值，与 XRANGE 命令返回的消息结构一致。</para>
        /// <para>如果没有成功认领任何消息，则集合可能为空。</para>
        /// </summary>
#if MODERN_NET
        public readonly EntryItem[]? Entries => this._entries;
        private readonly EntryItem[]? _entries;
#else
        public readonly EntryItem[] Entries => this._entries;
        private readonly EntryItem[] _entries;
#endif

        internal XAutoClaimResult(RedisResult value)
        {
            if (value.Type != RespType.Array || value.RawValue is not RedisResult[] { Length: 2 or 3 } array)
            {
                throw new InvalidCastException("Unrecognizable data cannot be converted into XAutoClaimResult");
            }
            this._nextStartId = array[0]!;

            if (array[1].RawValue is RedisResult[] { Length: > 0 })
            {
                this._entries = array[1];
            }

            if (array.Length is 3)
            {
                this._deletedIds = array[2];
            }
        }
    }

    /// <summary>
    /// Represents summary information about pending entries in a Redis Stream consumer group.
    /// <para>表示 Redis Stream 消费者组待处理消息的摘要信息。</para>
    /// </summary>
    public readonly struct XPendingSummary
    {
        private readonly long _count;

#if MODERN_NET
        private readonly string? _minId;
        private readonly string? _maxId;
        private readonly ConsumerItem[]? _consumers;
#else
        private readonly string _minId;
        private readonly string _maxId;
        private readonly ConsumerItem[] _consumers;
#endif

        /// <summary>
        /// Gets the total number of pending entries in the consumer group.
        /// <para>获取消费者组中待处理消息的总数量。</para>
        /// </summary>
        public readonly long Count => this._count;

        /// <summary>
        /// Gets the smallest pending message ID.
        /// <para>Returns null when no pending entries exist.</para>
        /// <para>获取待处理消息中最小的消息ID。</para>
        /// <para>没有待处理消息时返回null。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? MinId => this._minId;
#else
        public readonly string MinId => this._minId;
#endif

        /// <summary>
        /// Gets the greatest pending message ID.
        /// <para>Returns null when no pending entries exist.</para>
        /// <para>获取待处理消息中最大的消息ID。</para>
        /// <para>没有待处理消息时返回null。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? MaxId => this._maxId;
#else
        public readonly string MaxId => this._maxId;
#endif

        /// <summary>
        /// Gets the consumers with pending entries and their pending counts.
        /// <para>Returns null when no pending entries exist.</para>
        /// <para>获取拥有待处理消息的消费者以及各自的待处理消息数量。</para>
        /// <para>没有待处理消息时返回null。</para>
        /// </summary>
#if MODERN_NET
        public readonly ConsumerItem[]? Consumers => this._consumers;
#else
        public readonly ConsumerItem[] Consumers => this._consumers;
#endif

        internal XPendingSummary(RedisResult value)
        {
            if (value.RawValue is not RedisResult[] { Length: 4 } array)
            {
                throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(XPendingSummary)}");
            }

            this._count = array[0];
            this._minId = array[1];
            this._maxId = array[2];

            if (array[3].RawValue is RedisResult[] consumers)
            {
                this._consumers = new ConsumerItem[consumers.Length];

                for (int i = 0; i < consumers.Length; i++)
                {
                    this._consumers[i] = new ConsumerItem(consumers[i]);
                }
            }
        }

        /// <summary>
        /// Represents the pending entry statistics of a consumer.
        /// <para>表示消费者的待处理消息统计信息。</para>
        /// </summary>
        public readonly struct ConsumerItem
        {
            private readonly string _name;
            private readonly long _count;

            /// <summary>
            /// Gets the consumer name.
            /// <para>获取消费者名称。</para>
            /// </summary>
            public readonly string Name => this._name;

            /// <summary>
            /// Gets the number of pending entries owned by this consumer.
            /// <para>获取当前消费者拥有的待处理消息数量。</para>
            /// </summary>
            public readonly long Count => this._count;

            internal ConsumerItem(RedisResult value)
            {
                if (value.RawValue is not RedisResult[] { Length: 2 } array)
                {
                    throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(ConsumerItem)}");
                }

                this._name = array[0]!;
                this._count = array[1];
            }
        }
    }

    /// <summary>
    /// Represents a pending entry returned by the extended XPENDING command.
    /// <para>表示 XPENDING 详细模式返回的一条待处理消息。</para>
    /// </summary>
    public readonly struct XPendingItem
    {
        private readonly string _id;
        private readonly string _consumer;
        private readonly long _idleTime;
        private readonly long _deliveryCount;

        /// <summary>
        /// Gets the pending entry ID.
        /// <para>获取待处理消息的ID。</para>
        /// </summary>
        public readonly string Id => this._id;

        /// <summary>
        /// Gets the name of the consumer currently owning this pending entry.
        /// <para>An empty string may indicate that the entry has no assigned consumer.</para>
        /// <para>获取当前拥有该待处理消息的消费者名称。</para>
        /// <para>空字符串可能表示消息当前没有分配给具体消费者。</para>
        /// </summary>
        public readonly string Consumer => this._consumer;

        /// <summary>
        /// Gets the elapsed time since the last delivery, in milliseconds.
        /// <para>获取自上次投递消息以来的闲置时间，单位为毫秒。</para>
        /// </summary>
        public readonly long IdleTime => this._idleTime;

        /// <summary>
        /// Gets the number of times this pending entry has been delivered.
        /// <para>获取该消息累计被投递的次数。</para>
        /// <para>使用 XCLAIM 或 XREADGROUP 重新投递时，该值可能增加。</para>
        /// </summary>
        public readonly long DeliveryCount => this._deliveryCount;

        internal XPendingItem(RedisResult value)
        {
            if (value.RawValue is not RedisResult[] { Length: 4 } array)
            {
                throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(XPendingItem)}");
            }

            this._id = array[0]!;
            this._consumer = array[1]!;
            this._idleTime = array[2];
            this._deliveryCount = array[3];
        }
    }

    /// <summary>
    /// Represents information about a consumer returned by XINFO CONSUMERS.
    /// <para>Available since: 5.0.0</para>
    /// <para>表示 XINFO CONSUMERS 命令返回的消费者信息。</para>
    /// <para>支持 Redis 5.0.0+，兼容 Redis 7.2.0 前后的返回格式。</para>
    /// </summary>
    public readonly struct ConsumerInfo
    {
        private readonly string _name;
        private readonly long _pending;
        private readonly long _idle;
        private readonly long? _inactive;

        /// <summary>
        /// Gets the consumer name.
        /// <para>获取消费者名称。</para>
        /// </summary>
        public readonly string Name => this._name;

        /// <summary>
        /// Gets the number of pending messages owned by this consumer.
        /// <para>获取当前消费者 PEL 中尚未确认的消息数量。</para>
        /// </summary>
        public readonly long Pending => this._pending;

        /// <summary>
        /// Gets the consumer idle time, in milliseconds.
        /// <para>Before Redis 7.2.0, this is the time elapsed since
        /// the last successful interaction.</para>
        /// <para>Since Redis 7.2.0, this is the time elapsed since
        /// the last attempted interaction.</para>
        /// <para>获取消费者空闲时间，单位毫秒。</para>
        /// <para>Redis 7.2.0 之前：距离上次成功交互的时间。</para>
        /// <para>Redis 7.2.0 及之后：距离上次尝试交互的时间。</para>
        /// </summary>
        public readonly long Idle => this._idle;

        /// <summary>
        /// Gets the time elapsed since the consumer's last
        /// successful interaction, in milliseconds.
        /// <para>Available since: 7.2.0</para>
        /// <para>Null if the field is not returned by Redis.</para>
        /// <para>获取距离消费者上次成功交互的时间，单位毫秒。</para>
        /// <para>Redis 7.2.0+ 开始支持。</para>
        /// <para>旧版本不返回此字段，因此为 Null。</para>
        /// </summary>
        public readonly long? Inactive => this._inactive;

        internal ConsumerInfo(RedisResult value)
        {
            bool hasPending = false;
            bool hasIdle = false;
            if (value.Type is RespType.Array && value.RawValue is RedisResult[] values && values.Length % 2 == 0)
            {
                for (int i = 0; i < values.Length; i += 2)
                {
                    string field = values[i]!;
                    if ("name".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._name = values[i + 1]!;
                    }
                    else if ("pending".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._pending = values[i + 1];
                        hasPending = true;
                    }
                    else if ("idle".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._idle = values[i + 1];
                        hasIdle = true;
                    }
                    else if ("inactive".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._inactive = values[i + 1];
                    }
                }
            }
            else if (value.Type is RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] { Length: >= 3 } maps)
            {
                foreach (var map in maps)
                {
                    string field = map.Key!;
                    if ("name".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._name = map.Value!;
                    }
                    else if ("pending".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._pending = map.Value!;
                        hasPending = true;
                    }
                    else if ("idle".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._idle = map.Value!;
                        hasIdle = true;
                    }
                    else if ("inactive".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._inactive = map.Value!;
                    }
                }
            }
            else
            {
                throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(ConsumerInfo)}.");
            }

            if (this._name is null || !hasPending || !hasIdle)
            {
                throw new InvalidCastException("Missing required fields in XINFO CONSUMERS response.");
            }
        }
    }

    /// <summary>
    /// Represents information about a consumer group returned by XINFO GROUPS.
    /// <para>Available since: 5.0.0</para>
    /// <para>表示 XINFO GROUPS 命令返回的消费者组信息。</para>
    /// <para>支持 Redis 5.0.0+，兼容 Redis 7.0.0 前后的返回格式。</para>
    /// </summary>
    public readonly struct GroupInfo
    {
        private readonly string _name;
        private readonly long _consumers;
        private readonly long _pending;
        private readonly string _lastDeliveredId;
        private readonly long? _entriesRead;
        private readonly long? _lag;

        /// <summary>
        /// Gets the consumer group name.
        /// <para>获取消费者组名称。</para>
        /// </summary>
        public readonly string Name => this._name;

        /// <summary>
        /// Gets the number of consumers in the group.
        /// <para>获取消费者组中的消费者数量。</para>
        /// </summary>
        public readonly long Consumers => this._consumers;

        /// <summary>
        /// Gets the number of pending entries in the group's PEL.
        /// <para>获取消费者组 PEL 中尚未确认的消息数量。</para>
        /// </summary>
        public readonly long Pending => this._pending;

        /// <summary>
        /// Gets the ID of the last entry delivered to the group.
        /// <para>获取消费者组最后投递的消息 ID。</para>
        /// <para>此值不代表最后确认的消息 ID。</para>
        /// </summary>
        public readonly string LastDeliveredId => this._lastDeliveredId;

        /// <summary>
        /// Gets the logical number of entries read by the consumer group.
        /// <para>Available since: 7.0.0</para>
        /// <para>Returns null when the field is unavailable
        /// or the logical read counter is unknown.</para>
        /// <para>获取消费者组逻辑已读取消息数量。</para>
        /// <para>Redis 7.0.0+ 开始支持。</para>
        /// <para>旧版本不返回此字段，或者逻辑读取计数未知时为 Null。</para>
        /// </summary>
        public readonly long? EntriesRead => this._entriesRead;

        /// <summary>
        /// Gets the number of entries that have not yet been
        /// delivered to this consumer group.
        /// <para>Available since: 7.0.0</para>
        /// <para>Returns null when the lag cannot be determined.</para>
        /// <para>获取尚未投递给消费者组的消息数量。</para>
        /// <para>Redis 7.0.0+ 开始支持。</para>
        /// <para>旧版本不返回此字段，或无法计算 Lag 时为 Null。</para>
        /// <para>此值不包含已经投递但尚未确认的 PEL 消息。</para>
        /// </summary>
        public readonly long? Lag => this._lag;

        /// <summary>
        /// Creates consumer group information from a Redis response.
        /// <para>解析 XINFO GROUPS 返回的单个消费者组信息。</para>
        /// <para>兼容 Redis 7.0 之前的 4 个字段和之后的 6 个字段。</para>
        /// </summary>
        internal GroupInfo(RedisResult value)
        {
            bool hasConsumers = false;
            bool hasPending = false;
            if (value.Type is RespType.Array && value.RawValue is RedisResult[] values && values.Length % 2 == 0)
            {
                for (int i = 0; i < values.Length; i += 2)
                {
                    string field = values[i]!;
                    var fieldValue = values[i + 1];
                    if ("name".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._name = fieldValue!;
                    }
                    else if ("consumers".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._consumers = fieldValue;
                        hasConsumers = true;
                    }
                    else if ("pending".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._pending = fieldValue;
                        hasPending = true;
                    }
                    else if ("last-delivered-id".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._lastDeliveredId = fieldValue!;
                    }
                    else if ("entries-read".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._entriesRead = fieldValue.IsNullOrEmpty ? null : (long)fieldValue;
                    }
                    else if ("lag".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._lag = fieldValue.IsNullOrEmpty ? null : (long)fieldValue;
                    }
                }
            }
            else if (value.Type is RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] { Length: >= 4 } maps)
            {
                foreach (var map in maps)
                {
                    string field = map.Key!;
                    var fieldValue = map.Value;
                    if ("name".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._name = fieldValue!;
                    }
                    else if ("consumers".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._consumers = fieldValue;
                        hasConsumers = true;
                    }
                    else if ("pending".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._pending = fieldValue;
                        hasPending = true;
                    }
                    else if ("last-delivered-id".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._lastDeliveredId = fieldValue!;
                    }
                    else if ("entries-read".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._entriesRead = fieldValue.IsNullOrEmpty ? null : (long)fieldValue;
                    }
                    else if ("lag".Equals(field, StringComparison.OrdinalIgnoreCase))
                    {
                        this._lag = fieldValue.IsNullOrEmpty ? null : (long)fieldValue;
                    }
                }
            }
            else
            {
                throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(GroupInfo)}.");
            }

            if (this._name is null || this._lastDeliveredId is null || !hasConsumers || !hasPending)
            {
                throw new InvalidCastException("Missing required fields in XINFO GROUPS response.");
            }
        }
    }

    /// <summary>
    /// Represents the result of XINFO STREAM.
    /// <para>表示 XINFO STREAM 返回的 Stream 状态信息。</para>
    /// </summary>
    public readonly struct StreamInfo
    {
        /// <summary>
        /// Gets whether the reply is in FULL mode.
        /// <para>是否为 FULL 详细模式。</para>
        /// </summary>
        public readonly bool IsFull { get; }

        /// <summary>
        /// Gets the current number of entries in the Stream.
        /// <para>当前 Stream 消息数量，相当于 XLEN。</para>
        /// </summary>
        public readonly long Length { get; }

        /// <summary>
        /// Gets the number of radix-tree keys.
        /// <para>底层基数树的键数量。</para>
        /// </summary>
        public readonly long RadixTreeKeys { get; }

        /// <summary>
        /// Gets the number of radix-tree nodes.
        /// <para>底层基数树节点数量。</para>
        /// </summary>
        public readonly long RadixTreeNodes { get; }

        /// <summary>
        /// Gets the total number of entries ever added.
        /// <para>Available since: 7.0.0.</para>
        /// <para>Stream 历史累计添加的消息数量。</para>
        /// <para>删除消息不会减少此计数。</para>
        /// </summary>
        public readonly long? EntriesAdded { get; }

        /// <summary>
        /// Gets the number of consumer groups in summary mode.
        /// <para>普通模式中的消费者组数量。</para>
        /// <para>FULL 模式返回 Null，请使用 FullGroups。</para>
        /// </summary>
        public readonly long? GroupCount { get; }

        /// <summary>
        /// Gets the first existing stream entry in summary mode.
        /// <para>普通模式中的首条现存消息。</para>
        /// <para>Stream 为空或处于 FULL 模式时为 Null。</para>
        /// </summary>
        public readonly EntryItem? FirstEntry { get; }

        /// <summary>
        /// Gets the last existing stream entry in summary mode.
        /// <para>普通模式中的最后一条现存消息。</para>
        /// <para>Stream 为空或处于 FULL 模式时为 Null。</para>
        /// </summary>
        public readonly EntryItem? LastEntry { get; }

        /// <summary>
        /// Gets the IDMP retention duration in seconds.
        /// <para>Available since: 8.6.0.</para>
        /// <para>IDMP 幂等 ID 保留时长，单位秒。</para>
        /// <para>未返回该字段时为 Null。</para>
        /// </summary>
        public readonly long? IdmpDuration { get; }

        /// <summary>
        /// Gets the maximum tracked idempotent IDs per producer.
        /// <para>Available since: 8.6.0.</para>
        /// <para>每个生产者最多跟踪的幂等 ID 数量。</para>
        /// </summary>
        public readonly long? IdmpMaxSize { get; }

        /// <summary>
        /// Gets the number of tracked producer IDs.
        /// <para>Available since: 8.6.0.</para>
        /// <para>当前正在跟踪的生产者 ID 数量。</para>
        /// </summary>
        public readonly long? PidsTracked { get; }

        /// <summary>
        /// Gets the number of currently tracked idempotent IDs.
        /// <para>Available since: 8.6.0.</para>
        /// <para>所有生产者当前跟踪的幂等 ID 总数。</para>
        /// </summary>
        public readonly long? IidsTracked { get; }

        /// <summary>
        /// Gets the lifetime count of added idempotent IDs.
        /// <para>Available since: 8.6.0.</para>
        /// <para>历史累计添加的幂等 ID 数量。</para>
        /// </summary>
        public readonly long? IidsAdded { get; }

        /// <summary>
        /// Gets the lifetime count of detected duplicate IDs.
        /// <para>Available since: 8.6.0.</para>
        /// <para>历史累计识别并阻止的重复消息数量。</para>
        /// </summary>
        public readonly long? IidsDuplicates { get; }

#if MODERN_NET
        /// <summary>
        /// Gets the last generated stream entry ID.
        /// <para>Stream 最后生成的消息 ID。</para>
        /// <para>即使最后一条消息已被删除，此值也可能保留。</para>
        /// </summary>
        public readonly string? LastGeneratedId { get; }

        /// <summary>
        /// Gets the greatest deleted entry ID.
        /// <para>Available since: 7.0.0.</para>
        /// <para>历史上被删除的最大消息 ID。</para>
        /// <para>旧版本返回 Null。</para>
        /// </summary>
        public readonly string? MaxDeletedEntryId { get; }

        /// <summary>
        /// Gets the recorded first entry ID.
        /// <para>Available since: 7.0.0.</para>
        /// <para>Redis 记录的首条消息 ID。</para>
        /// <para>可能不同于当前实际存在的首条消息 ID。</para>
        /// </summary>
        public readonly string? RecordedFirstEntryId { get; }

        /// <summary>
        /// Gets stream entries in FULL mode.
        /// <para>FULL 模式返回的消息数组。</para>
        /// <para>受 COUNT 参数限制；Null 表示未返回或没有消息。</para>
        /// </summary>
        public readonly EntryItem[]? Entries { get; }

        /// <summary>
        /// Gets detailed consumer groups in FULL mode.
        /// <para>FULL 模式返回的消费者组详细信息。</para>
        /// <para>Null 表示未返回或没有消费者组。</para>
        /// </summary>
        public readonly FullGroupInfo[]? FullGroups { get; }
#else
        /// <summary>
        /// Gets the last generated stream entry ID.
        /// <para>Stream 最后生成的消息 ID。</para>
        /// <para>即使最后一条消息已被删除，此值也可能保留。</para>
        /// </summary>
        public readonly string LastGeneratedId { get; }

        /// <summary>
        /// Gets the greatest deleted entry ID.
        /// <para>Available since: 7.0.0.</para>
        /// <para>历史上被删除的最大消息 ID。</para>
        /// <para>旧版本返回 Null。</para>
        /// </summary>
        public readonly string MaxDeletedEntryId { get; }

        /// <summary>
        /// Gets the recorded first entry ID.
        /// <para>Available since: 7.0.0.</para>
        /// <para>Redis 记录的首条消息 ID。</para>
        /// <para>可能不同于当前实际存在的首条消息 ID。</para>
        /// </summary>
        public readonly string RecordedFirstEntryId { get; }

        /// <summary>
        /// Gets stream entries in FULL mode.
        /// <para>FULL 模式返回的消息数组。</para>
        /// <para>受 COUNT 参数限制；Null 表示未返回或没有消息。</para>
        /// </summary>
        public readonly EntryItem[] Entries { get; }

        /// <summary>
        /// Gets detailed consumer groups in FULL mode.
        /// <para>FULL 模式返回的消费者组详细信息。</para>
        /// <para>Null 表示未返回或没有消费者组。</para>
        /// </summary>
        public readonly FullGroupInfo[] FullGroups { get; }
#endif

        internal StreamInfo(RedisResult value)
        {
            bool full = false;
            bool hasLength = false, hasLastId = false;
            RedisResult groupsValue = default;
            bool hasGroups = false;
            foreach (var pair in StreamInfoParser.Fields(value))
            {
                var v = pair.Value;
                if ("length".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.Length = v;
                    hasLength = true;
                }
                else if ("radix-tree-keys".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.RadixTreeKeys = v;
                }
                else if ("radix-tree-nodes".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.RadixTreeNodes = v;
                }
                else if ("last-generated-id".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.LastGeneratedId = v;
                    hasLastId = true;
                }
                else if ("max-deleted-entry-id".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.MaxDeletedEntryId = v;
                }
                else if ("entries-added".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.EntriesAdded = v;
                }
                else if ("recorded-first-entry-id".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.RecordedFirstEntryId = v;
                }
                else if ("first-entry".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    if (!v.IsNullOrEmpty) this.FirstEntry = new EntryItem(v);
                }
                else if ("last-entry".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    if (!v.IsNullOrEmpty) this.LastEntry = new EntryItem(v);
                }
                else if ("entries".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.IsFull = true;
                    full = true;
                    this.Entries = StreamInfoParser.Array(v, static x => new EntryItem(x));
                }
                else if ("groups".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    groupsValue = v;
                    hasGroups = true;
                }
                else if ("idmp-duration".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.IdmpDuration = v;
                }
                else if ("idmp-maxsize".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.IdmpMaxSize = v;
                }
                else if ("pids-tracked".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.PidsTracked = v;
                }
                else if ("iids-tracked".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.IidsTracked = v;
                }
                else if ("iids-added".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.IidsAdded = v;
                }
                else if ("iids-duplicates".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    this.IidsDuplicates = v;
                }
            }

            if (!hasLength || !hasLastId || !hasGroups)
                throw new InvalidCastException("Invalid XINFO STREAM response.");

            if (full)
            {
                this.FullGroups = StreamInfoParser.Array(groupsValue, static x => new FullGroupInfo(x));
            }
            else
            {
                this.GroupCount = groupsValue;
            }
        }

        /// <summary>
        /// Represents a consumer group in XINFO STREAM FULL.
        /// <para>Available since: 6.0.0.</para>
        /// <para>表示 XINFO STREAM FULL 返回的消费者组详情。</para>
        /// </summary>
        public readonly struct FullGroupInfo
        {
            /// <summary>
            /// Gets the consumer group name.
            /// <para>消费者组名称。</para>
            /// </summary>
            public readonly string Name { get; }

            /// <summary>
            /// Gets the last delivered entry ID.
            /// <para>消费者组最后投递的消息 ID。</para>
            /// </summary>
            public readonly string LastDeliveredId { get; }

            /// <summary>
            /// Gets the logical read counter.
            /// <para>Available since: 7.0.0.</para>
            /// <para>逻辑已读取消息数量，可能为 Null。</para>
            /// </summary>
            public readonly long? EntriesRead { get; }

            /// <summary>
            /// Gets the number of entries not yet delivered.
            /// <para>Available since: 7.0.0.</para>
            /// <para>尚未投递的消息数量；无法计算时为 Null。</para>
            /// </summary>
            public readonly long? Lag { get; }

            /// <summary>
            /// Gets the total number of pending entries.
            /// <para>消费组 PEL 中的待确认消息总数。</para>
            /// </summary>
            public readonly long PelCount { get; }

            /// <summary>
            /// Gets the number of NACKed pending entries.
            /// <para>Available since: 8.8.0.</para>
            /// <para>处于 NACKed 状态的待处理消息数量。</para>
            /// <para>旧版本不返回此字段，值为 Null。</para>
            /// </summary>
            public readonly long? NackedCount { get; }

#if MODERN_NET
            /// <summary>
            /// Gets the detailed pending entries.
            /// <para>消费组 PEL 条目，可能受 FULL COUNT 限制。</para>
            /// </summary>
            public readonly FullPendingInfo[]? Pending { get; }

            /// <summary>
            /// Gets consumers belonging to the group.
            /// <para>该消费组中的消费者详细信息。</para>
            /// </summary>
            public readonly FullConsumerInfo[]? Consumers { get; }
#else
            /// <summary>
            /// Gets the detailed pending entries.
            /// <para>消费组 PEL 条目，可能受 FULL COUNT 限制。</para>
            /// </summary>
            public readonly FullPendingInfo[] Pending { get; }

            /// <summary>
            /// Gets consumers belonging to the group.
            /// <para>该消费组中的消费者详细信息。</para>
            /// </summary>
            public readonly FullConsumerInfo[] Consumers { get; }
#endif

            internal FullGroupInfo(RedisResult value)
            {
                foreach (var pair in StreamInfoParser.Fields(value))
                {
                    var v = pair.Value;
                    if ("name".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Name = v!;
                    }
                    else if ("last-delivered-id".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.LastDeliveredId = v!;
                    }
                    else if ("entries-read".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.EntriesRead = v;
                    }
                    else if ("lag".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Lag = v;
                    }
                    else if ("pel-count".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.PelCount = v;
                    }
                    else if ("nacked-count".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.NackedCount = v;
                    }
                    else if ("pending".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Pending = StreamInfoParser.Array(v, static x => new FullPendingInfo(x));
                    }
                    else if ("consumers".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Consumers = StreamInfoParser.Array(v, static x => new FullConsumerInfo(x));
                    }
                }

                if (this.Name is null || this.LastDeliveredId is null) throw new InvalidCastException("Invalid XINFO STREAM FULL group.");
            }
        }

        /// <summary>
        /// Represents a consumer in XINFO STREAM FULL.
        /// <para>Available since: 6.0.0.</para>
        /// <para>表示 FULL 模式中的消费者详细信息。</para>
        /// </summary>
        public readonly struct FullConsumerInfo
        {
            /// <summary>
            /// Gets the consumer name.
            /// <para>消费者名称。</para>
            /// </summary>
            public readonly string Name { get; }

            /// <summary>
            /// Gets the last interaction timestamp in Unix milliseconds.
            /// <para>Before Redis 7.2: last successful interaction.</para>
            /// <para>Since Redis 7.2: last attempted interaction.</para>
            /// <para>Redis 7.2 之前：最后成功交互时间。</para>
            /// <para>Redis 7.2 及之后：最后尝试交互时间。</para>
            /// <para>单位为 Unix 毫秒时间戳，-1 表示没有对应记录。</para>
            /// </summary>
            public readonly long SeenTime { get; }

            /// <summary>
            /// Gets the last successful interaction timestamp.
            /// <para>Available since: 7.2.0.</para>
            /// <para>最后成功交互的 Unix 毫秒时间戳。</para>
            /// <para>Null 表示旧版本未返回，-1 表示没有成功交互记录。</para>
            /// </summary>
            public readonly long? ActiveTime { get; }

            /// <summary>
            /// Gets the consumer's total pending count.
            /// <para>该消费者 PEL 待确认消息总数。</para>
            /// </summary>
            public readonly long PelCount { get; }

            /// <summary>
            /// Gets detailed pending entries for this consumer.
            /// <para>消费者自己的 PEL 明细，不包含重复的消费者名称。</para>
            /// </summary>
#if MODERN_NET
            public readonly FullConsumerPendingInfo[]? Pending { get; }
#else
            public readonly FullConsumerPendingInfo[] Pending { get; }
#endif

            internal FullConsumerInfo(RedisResult value)
            {
                foreach (var pair in StreamInfoParser.Fields(value))
                {
                    var v = pair.Value;
                    if ("name".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Name = v!;
                    }
                    else if ("seen-time".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.SeenTime = v;
                    }
                    else if ("active-time".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.ActiveTime = v;
                    }
                    else if ("pel-count".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.PelCount = v;
                    }
                    else if ("pending".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this.Pending = StreamInfoParser.Array(v, static x => new FullConsumerPendingInfo(x));
                    }
                }

                if (this.Name is null)
                    throw new InvalidCastException("Invalid XINFO STREAM FULL consumer.");
            }
        }

        /// <summary>
        /// Represents a group-level pending entry in XINFO STREAM FULL.
        /// <para>表示 FULL 模式消费组 PEL 中的一条待处理记录。</para>
        /// </summary>
        public readonly struct FullPendingInfo
        {
            /// <summary>
            /// Gets the stream entry ID.
            /// <para>消息 ID。</para>
            /// </summary>
            public readonly string Id { get; }

            /// <summary>
            /// Gets the current owner name.
            /// <para>当前持有消息的消费者名称。</para>
            /// <para>Redis 8.8+ 中被 XNACK 释放的消息可能返回空字符串。</para>
            /// </summary>
            public readonly string Consumer { get; }

            /// <summary>
            /// Gets the last delivery timestamp in Unix milliseconds.
            /// <para>最后一次投递的 Unix 毫秒时间戳。</para>
            /// <para>注意不是 XPENDING 的 IdleTime。</para>
            /// </summary>
            public readonly long DeliveryTime { get; }

            /// <summary>
            /// Gets the number of deliveries.
            /// <para>累计投递次数。</para>
            /// </summary>
            public readonly long DeliveryCount { get; }

            internal FullPendingInfo(RedisResult value)
            {
                if (value.RawValue is not RedisResult[] { Length: 4 } arr)
                    throw new InvalidCastException("Invalid XINFO STREAM FULL pending entry.");

                this.Id = arr[0]!;
                this.Consumer = arr[1]!;
                this.DeliveryTime = arr[2];
                this.DeliveryCount = arr[3];
            }
        }

        /// <summary>
        /// Represents a consumer-level pending entry in XINFO STREAM FULL.
        /// <para>表示 FULL 模式单个消费者 PEL 中的一条待处理记录。</para>
        /// <para>与消费组 PEL 不同，不包含 Consumer 字段。</para>
        /// </summary>
        public readonly struct FullConsumerPendingInfo
        {
            /// <summary>
            /// Gets the stream entry ID.
            /// <para>消息 ID。</para>
            /// </summary>
            public readonly string Id { get; }

            /// <summary>
            /// Gets the last delivery timestamp in Unix milliseconds.
            /// <para>最后一次投递的 Unix 毫秒时间戳。</para>
            /// </summary>
            public readonly long DeliveryTime { get; }

            /// <summary>
            /// Gets the number of deliveries.
            /// <para>累计投递次数。</para>
            /// </summary>
            public readonly long DeliveryCount { get; }

            internal FullConsumerPendingInfo(RedisResult value)
            {
                if (value.RawValue is not RedisResult[] { Length: 3 } arr)
                    throw new InvalidCastException("Invalid XINFO STREAM FULL consumer pending entry.");

                this.Id = arr[0]!;
                this.DeliveryTime = arr[1];
                this.DeliveryCount = arr[2];
            }
        }

        /// <summary>
        /// Helpers for parsing XINFO STREAM replies.
        /// <para>XINFO STREAM 返回值解析辅助类。</para>
        /// </summary>
        private static class StreamInfoParser
        {
            /// <summary>
            /// Enumerates fields from RESP2 alternating arrays or RESP3 maps.
            /// <para>从 RESP2 交替数组或 RESP3 Map 中枚举字段。</para>
            /// </summary>
            internal static IEnumerable<KeyValuePair<string, RedisResult>> Fields(RedisResult value)
            {
                if (value.RawValue is RedisResult[] array)
                {
                    if ((array.Length & 1) != 0) throw new InvalidCastException("Invalid XINFO field/value array.");

                    for (int i = 0; i < array.Length; i += 2)
                    {
                        string key = array[i]!;
                        yield return new(key, array[i + 1]);
                    }
                    yield break;
                }
                else if (value.RawValue is KeyValuePair<RedisResult, RedisResult>[] pairs)
                {
                    foreach (var pair in pairs)
                    {
                        string key = pair.Key!;
                        yield return new(key, pair.Value);
                    }
                    yield break;
                }

                throw new InvalidCastException("Unsupported XINFO field/value representation.");
            }

            /// <summary>
            /// Converts a Redis array into a typed array.
            /// <para>将 Redis 数组转换成指定结构体数组。</para>
            /// <para>空数组按 SharpRedis 当前约定返回 Null。</para>
            /// </summary>
#if MODERN_NET
            internal static T[]? Array<T>(RedisResult value, Func<RedisResult, T> factory)
#else
            internal static T[] Array<T>(RedisResult value, Func<RedisResult, T> factory)
#endif
            {
                if (value.IsNullOrEmpty) return null;

                if (value.RawValue is not RedisResult[] array)
                    throw new InvalidCastException("Invalid XINFO array representation.");

                if (array.Length == 0)
                    return null;

                var result = new T[array.Length];

                for (int i = 0; i < array.Length; i++)
                    result[i] = factory(array[i]);

                return result;
            }
        }
    }

    /// <summary>
    /// Represents the result of acknowledging and deleting a Stream entry using <b>XACKDEL</b> or <b>XDELEX</b>.
    /// <para>表示 <b>XACKDEL</b> 或 <b>XDELEX</b> 命令确认并删除 Stream 消息的执行结果。</para>
    /// </summary>
    public enum DeleteResult : sbyte
    {
        /// <summary>
        /// unknown
        /// <para>未知的</para>
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The specified entry ID does not exist.
        /// <para>指定的消息 ID 不存在，或无法找到对应的待确认消息。</para>
        /// </summary>
        NotFound = -1,

        /// <summary>
        /// The entry was acknowledged and deleted from the Stream.
        /// <para>消息已确认，并成功从 Stream 中删除。</para>
        /// </summary>
        Deleted = 1,

        /// <summary>
        /// The entry was acknowledged but not deleted because it is still referenced.
        /// <para>消息已确认，但由于仍存在其他消费者组的引用，因此未删除。</para>
        /// <para>通常出现在使用 ACKED 策略时。</para>
        /// </summary>
        StillReferenced = 2,
    }
}
