using SharpRedis.Commands;
using SharpRedis.Extensions;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis Stream
/// <para>If you are new to streams, see <see href="https://redis.io/docs/data-types/streams">Introduction to Redis Streams</see></para>
/// <para>如果你不太懂Stream类型, 请<see href="https://redis.io/docs/data-types/streams">查看Stream文档</see></para>
/// </summary>
public sealed partial class RedisStream : RedisTypeBase
{
    #region Enums
    /// <summary>
    /// Specifies how Stream entries and consumer group references are handled during deletion.
    /// <para>指定删除 Stream 消息时的处理策略。</para>
    /// <para>Available since Redis 8.2.0.</para>
    /// </summary>
    public enum DeletionPolicy : byte
    {
        /// <summary>
        /// No specification (use Redis default behavior)
        /// <para>不指定（使用 Redis 默认行为，等同于 KEEPREF）</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Keep references in consumer groups even if the stream entry is deleted
        /// <para>Available since: 8.2.0</para>
        /// <para>即使 Stream 中的消息被删除，也保留消费者组中的引用（PEL）</para>
        /// <para>可能会出现“消息内容已删除，但仍存在未确认引用”的情况</para>
        /// <para>支持此命令的Redis版本, 8.2.0+</para>
        /// </summary>
        KeepRef,

        /// <summary>
        /// Delete all references from consumer groups when the stream entry is deleted
        /// <para>Available since: 8.2.0</para>
        /// <para>当 Stream 中的消息被删除时，同时删除消费者组中的引用</para>
        /// <para>可能会导致未确认消息丢失</para>
        /// <para>支持此命令的Redis版本, 8.2.0+</para>
        /// </summary>
        DelRef,

        /// <summary>
        /// Only delete entries that have been acknowledged by all consumer groups
        /// <para>Available since: 8.2.0</para>
        /// <para>仅删除已经被所有消费者组确认（ACK）的消息</para>
        /// <para>保证未确认消息不会被删除（推荐用于消息队列场景）</para>
        /// <para>支持此命令的Redis版本, 8.2.0+</para>
        /// </summary>
        Acked,
    }

    /// <summary>
    /// The trimming strategy
    /// <para>修剪策略枚举</para>
    /// </summary>
    public enum TrimOperator : byte
    {
        /// <summary>
        /// No operator specified (Redis default)
        /// <para>未指定操作符（使用 Redis 默认行为）</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Exact trimming
        /// <para>精确裁剪，严格符合阈值（性能较低）</para>
        /// </summary>
        Exact, // =

        /// <summary>
        /// Approximate trimming
        /// <para>近似裁剪，允许误差以提升性能（推荐）</para>
        /// </summary>
        Approximate, // ~
    }

    /// <summary>
    /// Specifies the negative acknowledgment mode for the XNACK command.
    /// <para>Available since: 8.8.0</para>
    /// <para>指定 XNACK 命令的否定确认模式，用于控制消息释放后投递次数的变化。</para>
    /// <para>支持此命令的 Redis 版本：8.8.0+</para>
    /// </summary>
    public enum NackMode : byte
    {
        /// <summary>
        /// Silently releases the message and decrements its delivery counter by one.
        /// <para>静默释放消息，并将投递次数减 1，相当于撤销本次投递计数。</para>
        /// <para>适用于消费者正常关闭、内部错误或其他与消息内容无关的失败。</para>
        /// </summary>
        Silent = 0,

        /// <summary>
        /// Releases the message without changing its delivery counter.
        /// <para>释放消息，但保持投递次数不变。</para>
        /// <para>适用于当前消费者处理失败，但其他消费者仍可能成功处理的情况。</para>
        /// </summary>
        Fail = 1,

        /// <summary>
        /// Releases the message and sets its delivery counter to Int64.MaxValue.
        /// <para>释放消息，并将投递次数设置为 long.MaxValue（9223372036854775807）。</para>
        /// <para>表示消息发生永久性失败，例如格式错误、毒消息或可疑恶意消息。</para>
        /// <para>可用于识别需要进入死信队列的消息。</para>
        /// </summary>
        Fatal = 2,
    }
    #endregion

    #region structs
    /// <summary>
    /// The idempotent id for a stream entry, ensuring at-most-once production. Each producer should have a unique <b>[ProducerId]</b> that is stable and consistent between runs. When adding stream entries, the caller can specify an <b>[IdempotentId]</b> that is unique and repeatable for a given data item, or omit it and let the server generate it from the content of the data item. In either event: duplicates are rejected.
    /// <para>Available since: 8.6.0</para>
    /// <para>幂等标识符, 使用此参数可以保证同一条消息不会重复添加. 可以指定幂等生产者唯一Id <b>[ProducerId]</b> 和幂等Id <b>[IdempotentId]</b>. 如果两个Id都一致的时候, 消息不会重复添加</para>
    /// <para>支持此命令的Redis版本, 8.6.0+</para>
    /// </summary>
    /// <example>
    /// <code>
    /// StreamIdempotentId id = "producerId"; //Create a new StreamIdempotentId with the given producer-id. The idempotent-id auto-generation
    /// StreamIdempotentId id = ("producerId", "idempotentId");
    /// 
    /// StreamIdempotentId id = new StreamIdempotentId("producerId"); //Create a new StreamIdempotentId with the given producer-id. The idempotent-id auto-generation
    /// StreamIdempotentId id = new StreamIdempotentId("producerId", "idempotentId");
    /// 
    /// StreamIdempotentId id = StreamIdempotentId.Create("producerId"); //Create a new StreamIdempotentId with the given producer-id. The idempotent-id auto-generation
    /// StreamIdempotentId id = StreamIdempotentId.Create("producerId", "idempotentId");
    /// </code>
    /// </example>
    public readonly struct IdempotentArg : IEquatable<IdempotentArg>
    {
        public static readonly IdempotentArg Empty = new();

        private readonly SharpValue _producerId = SharpValue.Empty;
        private readonly SharpValue _idempotentId = SharpValue.Empty;

        /// <summary>
        /// Gets the producer-id
        /// <para>获得生产者Id</para>
        /// </summary>
        public readonly SharpValue ProducerId => this._producerId;

        /// <summary>
        /// Gets the idempotent-id
        /// <para>获得幂等Id</para>
        /// </summary>
        public readonly SharpValue Id => this._idempotentId;

        public readonly bool HasValue => this._producerId.HasValue;

        public readonly bool IsNull => !this._producerId.HasValue;

        /// <summary>
        /// Create a new <see cref="RedisStream.IdempotentArg"/> with the given producer-id. The idempotent-id auto-generation
        /// <para>Available since: 8.6.0</para>
        /// <para>指定生产者Id创建, 幂等Id自动生成</para>
        /// <para>支持此命令的Redis版本, 8.6.0+</para>
        /// </summary>
        /// <param name="producerId">producer id
        /// <para>生产者Id</para>
        /// </param>
        public IdempotentArg(SharpValue producerId)
        {
            this._producerId = producerId;
        }

        /// <summary>
        /// Create a new <see cref="RedisStream.IdempotentArg"/> with the given producer-id and idempotent-id
        /// <para>Available since: 8.6.0</para>
        /// <para>指定生成者Id和幂等Id创建</para>
        /// <para>支持此命令的Redis版本, 8.6.0+</para>
        /// </summary>
        /// <param name="producerId">producer id
        /// <para>生产者Id</para>
        /// </param>
        /// <param name="idempotentId">idempotent-id
        /// <para>幂等Id</para>
        /// </param>
        public IdempotentArg(SharpValue producerId, SharpValue idempotentId)
        {
            this._producerId = producerId;
            this._idempotentId = idempotentId;
        }

        public override string ToString()
        {
            return this.ToString(Encoding.UTF8);
        }

        public string ToString(Encoding encoding)
        {
            if (this.IsNull) return string.Empty;
            if (this._idempotentId.HasValue)
            {
                return $"IDMP {this._producerId.ToString(encoding)} {this._idempotentId.ToString(encoding)}";
            }
            return $"IDMPAUTO {this._producerId.ToString(encoding)}";
        }

        public bool Equals(IdempotentArg other)
        {
            if (other.IsNull && this.IsNull) return true;
            if (!other.IsNull && this.IsNull) return false;
            if (other.IsNull && !this.IsNull) return false;
            return other._idempotentId == this._idempotentId && this._producerId == other._producerId;
        }

#if MODERN_NET
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
#else
        public override bool Equals(object obj)
#endif
        {
            if (obj is null) return false;
            if (obj is IdempotentArg v) return v == this;
            return false;
        }

        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            unchecked
            {
                int hash = 23;
                hash = hash * 397 + this._producerId.GetHashCode();
                hash = hash * 397 + this._idempotentId.GetHashCode();
                return hash;
            }
#else
            return HashCode.Combine(this._producerId, this._idempotentId);
#endif
        }

        public static bool operator ==(IdempotentArg left, IdempotentArg right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IdempotentArg left, IdempotentArg right)
        {
            return !(left == right);
        }

        public static IdempotentArg Create(SharpValue producerId) => producerId;

        public static IdempotentArg Create(SharpValue producerId, SharpValue idempotentId) => new(producerId, idempotentId);

        public static implicit operator IdempotentArg(string producerId)
        {
            return new(producerId);
        }

        public static implicit operator IdempotentArg((string producerId, string idempotentId) val)
        {
            return new(val.producerId, val.idempotentId);
        }

        public static implicit operator IdempotentArg(SharpValue producerId)
        {
            return new(producerId);
        }

        public static implicit operator IdempotentArg((SharpValue producerId, SharpValue idempotentId) val)
        {
            return new(val.producerId, val.idempotentId);
        }
    }

    /// <summary>
    /// The stream entry ID
    /// <para>流消息Id</para>
    /// </summary>
    /// <example>
    /// <code>
    /// StreamEntryId id = "*"; // StreamEntryId id = default; or StreamEntryId id = StreamEntryId.Empty;
    /// StreamEntryId id = "1-1";
    /// StreamEntryId id = "1-*"; //Redis 7.0.0 and above versions are required for this to be supported.
    /// StreamEntryId id = 1; //Only specify the "ms" part, and "seq" will be automatically generated. Redis 7.0.0 and above versions are required for this to be supported.
    /// StreamEntryId id = (1, 1); //Specify ms and seq. The first one is ms, and the second one is seq.
    /// 
    /// StreamEntryId id = StreamEntryId.Create("1-1");
    /// StreamEntryId id = StreamEntryId.Create("1-*");
    /// StreamEntryId id = StreamEntryId.Create("*"); // StreamEntryId id = default; or StreamEntryId id = StreamEntryId.Empty;
    /// StreamEntryId id = StreamEntryId.Create(1); //Only specify the "ms" part, and "seq" will be automatically generated.
    /// StreamEntryId id = StreamEntryId.Create(1, 1); //Specify ms and seq. The first one is ms, and the second one is seq.
    /// 
    /// StreamEntryId id = new StreamEntryId("1-1");
    /// StreamEntryId id = new StreamEntryId("*"); // StreamEntryId id = default; or StreamEntryId id = StreamEntryId.Empty;
    /// StreamEntryId id = new StreamEntryId("1-*"); //Redis 7.0.0 and above versions are required for this to be supported.
    /// StreamEntryId id = new StreamEntryId(1); //Only specify the "ms" part, and "seq" will be automatically generated.
    /// StreamEntryId id = new StreamEntryId(1, 1); //Specify ms and seq. The first one is ms, and the second one is seq.
    /// </code>
    /// </example>
    public readonly struct EntryId : IEquatable<EntryId>
    {
        public static readonly EntryId Empty = default;

        private readonly string _id;

        /// <summary>
        /// Gets the stream entry ID
        /// </summary>
        public readonly string Id
        {
            get
            {
                if (this._id is null) return "*";
                return this._id;
            }
        }

        public bool HasValue => this._id is not null;

        /// <summary>
        /// Use * to auto-generate a unique ID
        /// <para>使用*自动生成唯一Id</para>
        /// </summary>
        public EntryId()
        {
            this._id = "*";
        }

        /// <summary>
        /// Specify ms, automatically generate seq
        /// <para>Available since: 7.0.0</para>
        /// <para>指定ms, 自动生成seq</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="ms"></param>
        public EntryId(ulong ms)
        {
            this._id = $"{ms}-*";
        }

        /// <summary>
        /// Specify the complete stream entry ID
        /// <para>指定完整的流消息Id</para>
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="seq"></param>
        public EntryId(ulong ms, ulong seq)
        {
            this._id = $"{ms}-{seq}";
        }

        /// <summary>
        /// Specify the complete stream entry ID
        /// <para>指定完整的流消息Id</para>
        /// </summary>
        /// <param name="id">message-id, 1-1, *, 1-*
        /// <para>消息Id, 如: *, 1-1, 1-*, $, 0, >都可以</para>
        /// </param>
        /// <exception cref="ArgumentException"></exception>
        public EntryId(string id)
        {
            if (!EntryId.IsValid(id))
            {
                throw new ArgumentException("Invalid stream ID specified as stream command argument", nameof(id));
            }
            this._id = id;
        }

#if MODERN_NET
        public override string? ToString()
#else
        public override string ToString()
#endif
        {
            return this._id;
        }

        public override int GetHashCode()
        {
            if (this._id is null) return -1;
            return this._id.GetHashCode();
        }

#if MODERN_NET
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
#else
        public override bool Equals(object obj)
#endif
        {
            if (obj is null) return false;
            if (obj is EntryId id) return this == id;
            return false;
        }

        public bool Equals(EntryId other)
        {
            return other._id == this._id;
        }

        public static bool operator ==(EntryId left, EntryId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntryId left, EntryId right)
        {
            return !(left == right);
        }

        private static bool IsValid(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            if (value.Length == 1 &&
                (value == "*" || value == "$" || value == "0" ||
                 value == ">" || value == "-" || value == "+"))
                return true;

            ReadOnlySpan<char> span = value;

            // 开区间，例如 (1-0、(100
            if (span[0] == '(')
            {
#if MODERN_NET
                span = span[1..];
#else
                span = span.Slice(1);
#endif
                // (100：合法的开区间 ID
                if (span.IsValidUInt64()) return true;
            }

            int dashIndex = span.IndexOf('-');

            // 没有分隔符或分隔符位置不合法
            if (dashIndex <= 0 || dashIndex == span.Length - 1)
                return false;

            // 不允许出现多个 -
#if MODERN_NET
            if (span[(dashIndex + 1)..].IndexOf('-') >= 0)
                return false;
#else
            if (span.Slice(dashIndex + 1).IndexOf('-') >= 0)
                return false;
#endif

#if MODERN_NET
            var part1 = span[..dashIndex];
            var part2 = span[(dashIndex + 1)..];
#else
            var part1 = span.Slice(0, dashIndex);
            var part2 = span.Slice(dashIndex + 1);
#endif

            if (!part1.IsValidUInt64()) return false;

            // 只允许普通 ID 使用 * 作为序列号
            // 例如 1-*，但不允许 (1-*
            if (part2.Length == 1 && part2[0] == '*')
                return value[0] != '(';

            return part2.IsValidUInt64();
        }

        public static EntryId Create(string id) => id;

        public static EntryId Create(ulong ms) => ms;

        public static EntryId Create(ulong ms, ulong seq) => new(ms, seq);

        public static implicit operator EntryId(string id)
        {
            return new(id);
        }

        public static implicit operator EntryId(ulong ms)
        {
            return new(ms);
        }

        public static implicit operator EntryId((ulong ms, ulong seq) id)
        {
            return new(id.ms, id.seq);
        }
    }

    /// <summary>
    /// Represents the trimming options for Redis Stream commands.
    /// <para>Supports MAXLEN and MINID trimming strategies.</para>
    /// <para>Can be used with XADD and XTRIM commands.</para>
    /// <para>Redis Stream 消息裁剪选项，用于控制 Stream 中旧消息的删除策略。</para>
    /// <para>支持 MAXLEN（按消息数量）和 MINID（按消息 ID）两种裁剪策略。</para>
    /// <para>可用于 XADD 和 XTRIM 命令。</para>
    /// </summary>
    public readonly struct TrimOptions
    {
        private readonly TrimOperator _operator;
        private readonly long? _maxLen;
        private readonly long? _limit;

        /// <summary>
        /// Gets the minimum Stream entry ID to retain when using MINID. <b>Available since Redis 6.2.0.</b>
        /// <para>获得使用 MINID 策略时，获取需要保留的最小消息 ID。 <b>自Redis 6.2.0开始支持</b></para>
        /// </summary>
#if MODERN_NET
        public readonly string? MinId => this._minId;

        private readonly string? _minId;
#else
        public readonly string MinId => this._minId;

        private readonly string _minId;
#endif

        /// <summary>
        /// Gets the trimming operator.
        /// <para>获取裁剪操作符，Exact 表示精确裁剪（=），Approximate 表示近似裁剪（~）。</para>
        /// </summary>
        public readonly TrimOperator Operator => this._operator;

        /// <summary>
        /// Gets the maximum number of entries to retain when using MAXLEN.
        /// <para>获得使用 MAXLEN 策略时，获取 Stream 最多保留的消息数量。</para>
        /// </summary>
        public long? MaxLen => this._maxLen;

        /// <summary>
        /// Gets the maximum number of entries to examine during approximate trimming. <b>Available since Redis 6.2.0.</b>
        /// <para>获得近似裁剪过程中最多检查的消息条目数量。<b>自Redis 6.2.0开始支持</b></para>
        /// </summary>
        public readonly long? Limit => this._limit;

        public readonly bool HasValue
        {
            get
            {
                return !string.IsNullOrEmpty(this._minId)
                    || this._maxLen.HasValue;
            }
        }

        /// <summary>
        /// Creates trimming options based on the maximum Stream length (MAXLEN).
        /// <para>创建基于最多保留消息数量（MAXLEN）的 Stream 裁剪选项。</para>
        /// </summary>
        /// <param name="trimOperator">
        /// The trimming operator.
        /// <para>裁剪操作符。</para>
        /// </param>
        /// <param name="maxLen">
        /// Specifying the maximum number of entries that may remain in the stream after trimming. Redis enforces this by removing the oldest entries - that is, the entries with the lowest stream IDs - so that only the newest entries are kept.
        /// <para>A value of 0 means no entries are retained when exact trimming is applied.</para>
        /// <para>指定修剪后 Stream 中可保留的最多消息数量。Redis会通过删除最旧的消息（即具有最小ID的消息）来强制执行此限制，从而只保留最新的消息。</para>
        /// <para>使用精确裁剪时，值为 0 表示不保留任何消息。</para>
        /// </param>
        /// <param name="limit">
        /// Limits the number of entries to examine during trimming. <b>Available since Redis 6.2.0.</b>
        /// <para>A value of 0 disables the limit. Null means the LIMIT option is omitted.</para>
        /// <para>Only supported with the approximate trimming operator (~).</para>
        /// <para>裁剪时, 最多检查的消息条目数量。<b>自Redis 6.2.0开始支持</b></para>
        /// <para>0 表示不限制检查数量；null 表示不指定 LIMIT 参数，使用 Redis 默认行为。</para>
        /// <para>仅支持近似裁剪操作符（~）。</para>
        /// </param>
        public TrimOptions(long maxLen, TrimOperator trimOperator, long? limit = null)
        {
            if (maxLen < 0) throw new ArgumentOutOfRangeException(nameof(maxLen), "MAXLEN does not support negative numbers");
            TrimOptions.ValidateLimit(trimOperator, limit);
            this._operator = trimOperator;
            this._maxLen = maxLen;
            this._limit = limit;
        }

        /// <summary>
        /// Creates trimming options based on the minimum Stream entry ID (MINID).
        /// <para>创建基于最小消息 ID（MINID）的 Stream 裁剪选项。</para>
        /// </summary>
        /// <param name="trimOperator">
        /// The trimming operator.
        /// <para>裁剪操作符。</para>
        /// </param>
        /// <param name="minId">
        /// Evicts entries with IDs lower than the specified threshold <b>Available since Redis 6.2.0.</b>
        /// <para>Supports IDs such as "0", "123456789" and "123456789-0".</para>
        /// <para>移除小于此消息ID的消息。<b>自Redis 6.2.0开始支持</b></para>
        /// <para>支持 "0"、"123456789"、"123456789-0" 等格式。</para>
        /// </param>
        /// <param name="limit">
        /// Limits the number of entries to examine during trimming. <b>Available since Redis 6.2.0.</b>
        /// <para>A value of 0 disables the limit. Null means the LIMIT option is omitted.</para>
        /// <para>Only supported with the approximate trimming operator (~).</para>
        /// <para>裁剪时, 最多检查的消息条目数量。<b>自Redis 6.2.0开始支持</b></para>
        /// <para>0 表示不限制检查数量；null 表示不指定 LIMIT 参数，使用 Redis 默认行为。</para>
        /// <para>仅支持近似裁剪操作符（~）。</para>
        /// </param>
        public TrimOptions(string minId, TrimOperator trimOperator, long? limit = null)
        {
            TrimOptions.ValidateMinId(minId);
            TrimOptions.ValidateLimit(trimOperator, limit);
            this._operator = trimOperator;
            this._minId = minId;
            this._limit = limit;
        }

        /// <inheritdoc cref="TrimOptions(long, TrimOperator, long?)"/>
        public static TrimOptions ByMaxLen(
            long maxLen,
            TrimOperator trimOperator,
            long? limit = null)
        {
            return new(maxLen, trimOperator, limit);
        }

        /// <inheritdoc cref="TrimOptions(string, TrimOperator, long?)"/>
        public static TrimOptions ByMinId(
            string minId,
            TrimOperator trimOperator,
            long? limit = null)
        {
            return new(minId, trimOperator, limit);
        }

        private static void ValidateLimit(TrimOperator trimOperator, long? limit)
        {
            if (!limit.HasValue) return;
            if (limit.Value < 0) throw new ArgumentOutOfRangeException(nameof(limit), "LIMIT does not support negative numbers");
            if (trimOperator != TrimOperator.Approximate)
                throw new ArgumentException("LIMIT requires approximate trimming.", nameof(limit));
        }

        private static void ValidateMinId(string minId)
        {
            if (string.IsNullOrEmpty(minId)) throw new ArgumentException("Invalid stream ID specified as stream command argument", nameof(minId));

            ReadOnlySpan<char> span = minId.AsSpan();

            int dashIndex = span.IndexOf('-');
            if (dashIndex == 0 || dashIndex == span.Length - 1) throw new ArgumentException("Invalid stream ID specified as stream command argument", nameof(minId));

            ReadOnlySpan<char> milliseconds = dashIndex < 0
                ? span
#if MODERN_NET
                : span[..dashIndex];
#else
                : span.Slice(0, dashIndex);
#endif

            if (!milliseconds.IsValidUInt64()) throw new ArgumentException("Invalid stream ID specified as stream command argument", nameof(minId));
            if (dashIndex >= 0)
            {
                ReadOnlySpan<char> sequence =
#if MODERN_NET
                    span[(dashIndex + 1)..];
#else
                    span.Slice(dashIndex + 1);
#endif
                if (!sequence.IsValidUInt64()) throw new ArgumentException("Invalid stream ID specified as stream command argument", nameof(minId));
            }
        }
    }

    /// <summary>
    /// Represents the options for querying detailed pending entries using XPENDING.
    /// <para>Encapsulates the ID range, maximum result count, idle time filter, and consumer filter.</para>
    /// <para>表示 Redis XPENDING 命令查询待处理消息详细信息的选项。</para>
    /// <para>封装消息ID范围、最大返回数量、最小闲置时间以及消费者过滤条件。</para>
    /// </summary>
    public readonly struct PendingOptions
    {
        private readonly EntryId _start;
        private readonly EntryId _end;
        private readonly long _count;
        private readonly ulong? _minIdleTime;

        /// <summary>
        /// Gets the consumer name used to filter pending entries.
        /// <para>Null means querying pending entries from all consumers in the group.</para>
        /// <para>获取用于过滤待处理消息的消费者名称。</para>
        /// <para>Null 表示不指定消费者，查询整个消费者组的待处理消息。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? Consumer => this._consumer;
        private readonly string? _consumer;
#else
        public readonly string Consumer => this._consumer;
        private readonly string _consumer;
#endif

        /// <summary>
        /// Gets the starting ID of the pending entry range, inclusive by default.
        /// <para>Use "-" for the smallest ID, or "(ID" for an exclusive boundary.</para>
        /// <para>获取待处理消息范围的起始ID，默认包含该ID。</para>
        /// <para>可使用 "-" 表示最小ID，或使用 "(ID" 表示不包含指定ID。</para>
        /// </summary>
        public readonly EntryId Start => this._start;

        /// <summary>
        /// Gets the ending ID of the pending entry range, inclusive by default.
        /// <para>Use "+" for the greatest ID, or "(ID" for an exclusive boundary.</para>
        /// <para>获取待处理消息范围的结束ID，默认包含该ID。</para>
        /// <para>可使用 "+" 表示最大ID，或使用 "(ID" 表示不包含指定ID。</para>
        /// </summary>
        public readonly EntryId End => this._end;

        /// <summary>
        /// Gets the maximum number of pending entries to return.
        /// <para>获取最多返回的待处理消息数量。</para>
        /// </summary>
        public readonly long Count => this._count;

        /// <summary>
        /// Gets the minimum idle time required for a pending entry, in milliseconds.
        /// <para>Null means the IDLE filter is not specified.</para>
        /// <para>获取待处理消息的最小闲置时间，单位为毫秒。</para>
        /// <para>Null 表示不指定 IDLE 参数，不按闲置时间过滤。</para>
        /// </summary>
        public readonly ulong? MinIdleTime => this._minIdleTime;

        public bool HasValue
        {
            get
            {
                return this._start != default
                    && this._end != default
                    && this._count > 0;
            }
        }

        /// <summary>
        /// Creates options for querying detailed pending entries using XPENDING.
        /// <para>创建 XPENDING 命令详细查询模式的选项。</para>
        /// </summary>
        /// <param name="start">The starting message ID.
        /// <para><b>Exclusive range intervals using "(ID" are available since Redis 6.2.0.</b></para>
        /// <para>开始扫描的消息ID，可使用 "-"。</para>
        /// <para><b>使用 "(ID" 表示开区间（不包含边界ID），需要 Redis 6.2.0+。</b></para>
        /// </param>
        /// <param name="end">The ending message ID.
        /// <para><b>Exclusive range intervals using "(ID" are available since Redis 6.2.0.</b></para>
        /// <para>结束扫描的消息ID，可使用 "+"。</para>
        /// <para><b>使用 "(ID" 表示开区间（不包含边界ID），需要 Redis 6.2.0+。</b></para>
        /// </param>
        /// <param name="count">The maximum number of entries to return.
        /// <para>最多返回的消息数量，必须大于0。</para>
        /// </param>
        /// <param name="minIdleTime"><b>Available since: 6.2.0</b> The minimum idle time in milliseconds.
        /// <para><b>自Redis 6.2.0+起支持</b> 最小闲置时间，单位毫秒。Null 表示不启用 IDLE 过滤。</para>
        /// </param>
        /// <param name="consumer">The consumer name to filter by.
        /// <para>指定消费者名称。Null 表示不过滤消费者。</para>
        /// </param>
        public PendingOptions(
            EntryId start,
            EntryId end,
            long count,
            ulong? minIdleTime = null,
#if MODERN_NET
            string? consumer = null
#else
            string consumer = null
#endif
        )
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "COUNT must be greater than zero.");
            this._start = start;
            this._end = end;
            this._count = count;
            this._minIdleTime = minIdleTime;
            this._consumer = consumer;
        }
    }
    #endregion

    internal RedisStream(BaseRedis redis) : base(redis) { }

    #region XAdd
    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="field">The field name for the stream entry.
    /// <para>消息字段名称</para>
    /// </param>
    /// <param name="value">The value to set in the stream entry.
    /// <para>消息字段的值</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        EntryId id,
        SharpValue field,
        SharpValue value,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            field: field,
            value: value) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValue">The field name and value for the stream entry. (field, value)
    /// <para>消息字段名称和值的元组 (field, value)</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        EntryId id,
        (SharpValue field, SharpValue value) fieldValue,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValue: fieldValue) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValue">The field name and value for the stream entry. (Key: field, Value: value)
    /// <para>消息字段名称和值, 键值对格式 (Key: field, Value: value)</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        EntryId id,
        KeyValuePair<SharpValue, SharpValue> fieldValue,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValue: fieldValue) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="field">The field name for the stream entry.
    /// <para>消息字段名称</para>
    /// </param>
    /// <param name="value">The value to set in the stream entry.
    /// <para>消息字段的值</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        SharpValue field,
        SharpValue value,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAdd(
            key: key,
            id: "*",
            field: field,
            value: value,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValue">The field name and value for the stream entry. (field, value)
    /// <para>消息字段名称和值的元组 (field, value)</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        (SharpValue field, SharpValue value) fieldValue,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAdd(
            key: key,
            id: "*",
            fieldValue: fieldValue,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValue">The field name and value for the stream entry. (Key: field, Value: value)
    /// <para>消息字段名称和值, 键值对格式 (Key: field, Value: value)</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        KeyValuePair<SharpValue, SharpValue> fieldValue,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAdd(
            key: key,
            id: "*",
            fieldValue: fieldValue,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueCollection">The collection of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        EntryId id,
        ICollection<(SharpValue field, SharpValue value)> fieldValueCollection,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValueCollection: fieldValueCollection) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueCollection">The collection of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        ICollection<(SharpValue field, SharpValue value)> fieldValueCollection,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAdd(
            key: key,
            id: "*",
            fieldValueCollection: fieldValueCollection,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueCollection">The collection of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        EntryId id,
        ICollection<KeyValuePair<SharpValue, SharpValue>> fieldValueCollection,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValueCollection: fieldValueCollection) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueCollection">The collection of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        ICollection<KeyValuePair<SharpValue, SharpValue>> fieldValueCollection,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAdd(
            key: key,
            id: "*",
            fieldValueCollection: fieldValueCollection,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueSpan">The span of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        EntryId id,
        ReadOnlySpan<(SharpValue field, SharpValue value)> fieldValueSpan,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValueSpan: fieldValueSpan) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueSpan">The span of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        ReadOnlySpan<(SharpValue field, SharpValue value)> fieldValueSpan,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAdd(
            key: key,
            id: "*",
            fieldValueSpan: fieldValueSpan,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueSpan">The span of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        EntryId id,
        ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>> fieldValueSpan,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValueSpan: fieldValueSpan) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueSpan">The span of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string?
#else
    string
#endif
    XAdd(
        SharpValue key,
        ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>> fieldValueSpan,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAdd(
            key: key,
            id: "*",
            fieldValueSpan: fieldValueSpan,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }
    #endregion

    /// <summary>
    /// Returns the number of entries inside a stream. If the specified key does not exist the command returns zero, as if the stream was empty. However note that unlike other Redis types, zero-length streams are possible, so you should call TYPE or EXISTS in order to check if a key exists or not.
    /// <para>Available since: 5.0.0</para>
    /// <para>获得消息条目数量, 如果Key不存在返回0. 注意, Stream和其它类型不一样, 0条消息的Stream是可以存在的, 因此如需判断Stream是否存在应该使用 <b>[EXISTS]</b> 命令</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of entries of the stream at key.
    /// <para>消息条目数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XLen">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XLen">命令文档</see></para>
    /// </remarks>
    public long XLen(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(StreamCommands.XLen(key) | flags, cancellationToken);

    /// <summary>
    /// Removes the specified entries from a stream, and returns the number of entries deleted.
    /// <para>Available since: 5.0.0</para>
    /// <para>根据指定的消息Id删除消息</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="id">One message ID to delete.
    /// <para>要删除消息的ID</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of entries that were deleted.
    /// <para>已删除的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDel">命令文档</see></para>
    /// </remarks>
    public int XDel(SharpValue key, EntryId id, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XDel(key, id) | flags, cancellationToken);
    }

    /// <summary>
    /// Removes the specified entries from a stream, and returns the number of entries deleted.
    /// <para>Available since: 5.0.0</para>
    /// <para>根据指定的消息Id删除消息</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="ids">One or more message IDs to delete.
    /// <para>要删除消息的ID, 一个或多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of entries that were deleted.
    /// <para>已删除的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDel">命令文档</see></para>
    /// </remarks>
    public int XDel(SharpValue key, ICollection<EntryId> ids, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XDel(key, ids) | flags, cancellationToken);
    }

    /// <summary>
    /// Removes the specified entries from a stream, and returns the number of entries deleted.
    /// <para>Available since: 5.0.0</para>
    /// <para>根据指定的消息Id删除消息</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="ids">One or more message IDs to delete.
    /// <para>要删除消息的ID, 一个或多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of entries that were deleted.
    /// <para>已删除的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDel">命令文档</see></para>
    /// </remarks>
    public int XDel(SharpValue key, ReadOnlySpan<EntryId> ids, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XDel(key, ids) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XACK</b> command removes one or multiple messages from the Pending Entries List (PEL) of a stream consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XACK</b> 确认消费者已成功处理指定消息，消息将从 PEL（待确认列表）中移除。返回被确认的消息数量。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="id">One message ID to acknowledge.
    /// <para>要确认的消息ID</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The command returns the number of messages successfully acknowledged.
    /// <para>被确认的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAck">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAck">命令文档</see></para>
    /// </remarks>
    public int XAck(SharpValue key, string group, EntryId id, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAck(key, group, id) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XACK</b> command removes one or multiple messages from the Pending Entries List (PEL) of a stream consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XACK</b> 确认消费者已成功处理指定消息，消息将从 PEL（待确认列表）中移除。返回被确认的消息数量。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="ids">One or more message IDs to acknowledge.
    /// <para>要确认的消息ID, 一个或多个的集合</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The command returns the number of messages successfully acknowledged.
    /// <para>被确认的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAck">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAck">命令文档</see></para>
    /// </remarks>
    public int XAck(SharpValue key, string group, ICollection<EntryId> ids, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAck(key, group, ids) | flags, cancellationToken);
    }

    /// <inheritdoc cref="XAck(SharpValue, string, ICollection{EntryId}, CommandFlags, CancellationToken)"/>
    public int XAck(SharpValue key, string group, ReadOnlySpan<EntryId> ids, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAck(key, group, ids) | flags, cancellationToken);
    }

    /// <summary>
    /// Create a new consumer group uniquely identified by groupname for the stream stored at key
    /// <para>Available since: 5.0.0</para>
    /// <para>创建一个新的消费者组</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>消息流Key</para>
    /// </param>
    /// <param name="group">The name of the consumer group to create.
    /// <para>要创建的消费者组名称</para>
    /// </param>
    /// <param name="id">The ID of the last delivered message the group starts reading after. Use $ for the last message currently in the stream, or 0 to read from the start.
    /// <para>起始消息ID, $表示最新，0表示从头</para>
    /// </param>
    /// <param name="mkStream">Create the stream as an empty stream if it does not already exist.
    /// <para>如果消息流不存在, 是否创建一个空的消息流</para>
    /// </param>
    /// <param name="entriesRead">Set the group's initial entries-read counter, used to compute its lag.
    /// <para><b>Starting from version 7.0.0 of Redis, this parameter has been supported.</b></para>
    /// <para>初始化消费者组的逻辑已读取消息计数器</para>
    /// <para><b>Redis 7.0.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xgroup-create">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xgroup-create">命令文档</see></para>
    /// </remarks>
    public bool XGroupCreate(
        SharpValue key,
        string group,
        EntryId id,
        bool mkStream = false,
        long? entriesRead = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XGroupCreate(key, group, id, mkStream, entriesRead) | flags, cancellationToken);
    }

    /// <summary>
    /// Read data from one streams, returning only entries with an ID greater than the last received ID reported by the caller. This command has an option to block if items are not available, in a similar fashion to BRPOP or BZPOPMIN and others.
    /// <para>Available since: 5.0.0</para>
    /// <para>从一个 Stream 中读取消息。支持阻塞读取（BLOCK）、从指定 ID 之后读取。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The key to read from
    /// <para>读取的Stream Key</para>
    /// </param>
    /// <param name="id">For each stream, entries with an ID greater than the given one are returned. Use $ to read only entries added after the call.
    /// <para>起始消息 ID（$ 最新，0-0 从头）</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRead">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRead">命令文档</see></para>
    /// </remarks>
    public XReadResult? XRead(
        SharpValue key,
        EntryId id,
        long? count = null,
        ulong? block = null,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XRead(
            key: key,
            id: id,
            count: count,
            block: block,
            maxCount: maxCount,
            maxSize: maxSize) | flags, cancellationToken);
    }

    /// <summary>
    /// Read data from one streams, returning only entries with an ID greater than the last received ID reported by the caller. This command has an option to block if items are not available, in a similar fashion to BRPOP or BZPOPMIN and others.
    /// <para>Available since: 5.0.0</para>
    /// <para>从一个 Stream 中读取消息。支持阻塞读取（BLOCK）、从指定 ID 之后读取。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array where each element is an array composed of a two elements containing the key name and the entries reported for that key.
    /// <para>一个数组, 每项都包含返回消息的Stream key和消息条目</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRead">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRead">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    XReadResult[]?
#else
    XReadResult[]
#endif
    XRead(ICollection<KeyValuePair<SharpValue, EntryId>> keysIds,
        long? count = null,
        ulong? block = null,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XRead(
            keysIds: keysIds,
            count: count,
            block: block,
            maxCount: maxCount,
            maxSize: maxSize) | flags, cancellationToken);
    }

    /// <summary>
    /// Read data from one streams, returning only entries with an ID greater than the last received ID reported by the caller. This command has an option to block if items are not available, in a similar fashion to BRPOP or BZPOPMIN and others.
    /// <para>Available since: 5.0.0</para>
    /// <para>从一个 Stream 中读取消息。支持阻塞读取（BLOCK）、从指定 ID 之后读取。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array where each element is an array composed of a two elements containing the key name and the entries reported for that key.
    /// <para>一个数组, 每项都包含返回消息的Stream key和消息条目</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRead">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRead">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    XReadResult[]?
#else
    XReadResult[]
#endif
    XRead(
        ReadOnlySpan<KeyValuePair<SharpValue, EntryId>> keysIds,
        long? count = null,
        ulong? block = null,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XRead(
            keysIds: keysIds,
            count: count,
            block: block,
            maxCount: maxCount,
            maxSize: maxSize) | flags, cancellationToken);
    }

    /// <summary>
    /// Read data from one streams, returning only entries with an ID greater than the last received ID reported by the caller. This command has an option to block if items are not available, in a similar fashion to BRPOP or BZPOPMIN and others.
    /// <para>Available since: 5.0.0</para>
    /// <para>从一个 Stream 中读取消息。支持阻塞读取（BLOCK）、从指定 ID 之后读取。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array where each element is an array composed of a two elements containing the key name and the entries reported for that key.
    /// <para>一个数组, 每项都包含返回消息的Stream key和消息条目</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRead">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRead">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    XReadResult[]?
#else
    XReadResult[]
#endif
    XRead(ICollection<(SharpValue key, EntryId id)> keysIds,
        long? count = null,
        ulong? block = null,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XRead(
            keysIds: keysIds,
            count: count,
            block: block,
            maxCount: maxCount,
            maxSize: maxSize) | flags, cancellationToken);
    }

    /// <summary>
    /// Read data from one streams, returning only entries with an ID greater than the last received ID reported by the caller. This command has an option to block if items are not available, in a similar fashion to BRPOP or BZPOPMIN and others.
    /// <para>Available since: 5.0.0</para>
    /// <para>从一个 Stream 中读取消息。支持阻塞读取（BLOCK）、从指定 ID 之后读取。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array where each element is an array composed of a two elements containing the key name and the entries reported for that key.
    /// <para>一个数组, 每项都包含返回消息的Stream key和消息条目</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRead">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRead">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    XReadResult[]?
#else
    XReadResult[]
#endif
    XRead(ReadOnlySpan<(SharpValue key, EntryId id)> keysIds,
        long? count = null,
        ulong? block = null,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XRead(
            keysIds: keysIds,
            count: count,
            block: block,
            maxCount: maxCount,
            maxSize: maxSize) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XREADGROUP</b> command is a special version of the <b>XREAD</b> command with support for consumer groups.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XREADGROUP</b> 通过消费者组读取 Stream 中的消息。消息被读取后进入 PEL（待确认列表），需用 XACK 确认。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The key to read from
    /// <para>读取的Stream Key</para>
    /// </param>
    /// <param name="id">Use > to read messages never delivered to any other consumer, or a specific ID to read from this consumer's pending list.
    /// <para>> 表示从未消费的消息，其他 ID 表示已投递的消息</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The consumer name.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="claim"><b>Added in Redis 8.4.0.</b> Claim messages that have been idle for at least this long (in milliseconds) before reading.
    /// <para><b>Redis 8.4.0 新增。</b> 读取至少闲置了一定时间的消息, 单位: 毫秒</para>
    /// </param>
    /// <param name="noack">Do not add the read messages to the PEL; treat them as acknowledged immediately.
    /// <para>是否视为已确认消息, 不进入PEL(待确认列表)</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XReadGroup">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XReadGroup">命令文档</see></para>
    /// </remarks>
    public XReadResult? XReadGroup(
        SharpValue key,
        EntryId id,
        string group,
        string consumer,
        long? count = null,
        ulong? block = null,
        ulong? claim = null,
        bool noack = false,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XReadGroup(
            key: key,
            id: id,
            group: group,
            consumer: consumer,
            count: count,
            maxCount: maxCount,
            maxSize: maxSize,
            block: block,
            claim: claim,
            noack: noack) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XREADGROUP</b> command is a special version of the <b>XREAD</b> command with support for consumer groups.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XREADGROUP</b> 通过消费者组读取 Stream 中的消息。消息被读取后进入 PEL（待确认列表），需用 XACK 确认。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The consumer name.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="claim"><b>Added in Redis 8.4.0.</b> Claim messages that have been idle for at least this long (in milliseconds) before reading.
    /// <para><b>Redis 8.4.0 新增。</b> 读取至少闲置了一定时间的消息, 单位: 毫秒</para>
    /// </param>
    /// <param name="noack">Do not add the read messages to the PEL; treat them as acknowledged immediately.
    /// <para>是否视为已确认消息, 不进入PEL(待确认列表)</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XReadGroup">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XReadGroup">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    XReadResult[]?
#else
    XReadResult[]
#endif
    XReadGroup(
        ICollection<KeyValuePair<SharpValue, EntryId>> keysIds,
        string group,
        string consumer,
        long? count = null,
        ulong? block = null,
        ulong? claim = null,
        bool noack = false,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XReadGroup(
            keysIds: keysIds,
            group: group,
            consumer: consumer,
            count: count,
            maxCount: maxCount,
            maxSize: maxSize,
            block: block,
            claim: claim,
            noack: noack) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XREADGROUP</b> command is a special version of the <b>XREAD</b> command with support for consumer groups.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XREADGROUP</b> 通过消费者组读取 Stream 中的消息。消息被读取后进入 PEL（待确认列表），需用 XACK 确认。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The consumer name.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="claim"><b>Added in Redis 8.4.0.</b> Claim messages that have been idle for at least this long (in milliseconds) before reading.
    /// <para><b>Redis 8.4.0 新增。</b> 读取至少闲置了一定时间的消息, 单位: 毫秒</para>
    /// </param>
    /// <param name="noack">Do not add the read messages to the PEL; treat them as acknowledged immediately.
    /// <para>是否视为已确认消息, 不进入PEL(待确认列表)</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XReadGroup">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XReadGroup">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    XReadResult[]?
#else
    XReadResult[]
#endif
    XReadGroup(
        ICollection<(SharpValue key, EntryId id)> keysIds,
        string group,
        string consumer,
        long? count = null,
        ulong? block = null,
        ulong? claim = null,
        bool noack = false,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XReadGroup(
            keysIds: keysIds,
            group: group,
            consumer: consumer,
            count: count,
            maxCount: maxCount,
            maxSize: maxSize,
            block: block,
            claim: claim,
            noack: noack) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XREADGROUP</b> command is a special version of the <b>XREAD</b> command with support for consumer groups.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XREADGROUP</b> 通过消费者组读取 Stream 中的消息。消息被读取后进入 PEL（待确认列表），需用 XACK 确认。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The consumer name.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="claim"><b>Added in Redis 8.4.0.</b> Claim messages that have been idle for at least this long (in milliseconds) before reading.
    /// <para><b>Redis 8.4.0 新增。</b> 读取至少闲置了一定时间的消息, 单位: 毫秒</para>
    /// </param>
    /// <param name="noack">Do not add the read messages to the PEL; treat them as acknowledged immediately.
    /// <para>是否视为已确认消息, 不进入PEL(待确认列表)</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XReadGroup">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XReadGroup">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    XReadResult[]?
#else
    XReadResult[]
#endif
    XReadGroup(
        ReadOnlySpan<KeyValuePair<SharpValue, EntryId>> keysIds,
        string group,
        string consumer,
        long? count = null,
        ulong? block = null,
        ulong? claim = null,
        bool noack = false,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XReadGroup(
            keysIds: keysIds,
            group: group,
            consumer: consumer,
            count: count,
            maxCount: maxCount,
            maxSize: maxSize,
            block: block,
            claim: claim,
            noack: noack) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XREADGROUP</b> command is a special version of the <b>XREAD</b> command with support for consumer groups.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XREADGROUP</b> 通过消费者组读取 Stream 中的消息。消息被读取后进入 PEL（待确认列表），需用 XACK 确认。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The consumer name.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="claim"><b>Added in Redis 8.4.0.</b> Claim messages that have been idle for at least this long (in milliseconds) before reading.
    /// <para><b>Redis 8.4.0 新增。</b> 读取至少闲置了一定时间的消息, 单位: 毫秒</para>
    /// </param>
    /// <param name="noack">Do not add the read messages to the PEL; treat them as acknowledged immediately.
    /// <para>是否视为已确认消息, 不进入PEL(待确认列表)</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XReadGroup">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XReadGroup">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    XReadResult[]?
#else
    XReadResult[]
#endif
    XReadGroup(
        ReadOnlySpan<(SharpValue key, EntryId id)> keysIds,
        string group,
        string consumer,
        long? count = null,
        ulong? block = null,
        ulong? claim = null,
        bool noack = false,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XReadGroup(
            keysIds: keysIds,
            group: group,
            consumer: consumer,
            count: count,
            maxCount: maxCount,
            maxSize: maxSize,
            block: block,
            claim: claim,
            noack: noack) | flags, cancellationToken);
    }

    /// <summary>
    /// Acknowledges and conditionally deletes one entries (messages) for a stream consumer group at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>有条件的确认并删除指定Stream中的一个消息</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The name of the consumer group.
    /// <para>消费组名称</para>
    /// </param>
    /// <param name="id">One stream entry ID to acknowledge and delete
    /// <para>要确认并删除的Stream消息ID</para>
    /// </param>
    /// <param name="delPolicy">pecifies how to handle consumer group references when acknowledging and deleting entries.
    /// <para>指定在确认和删除消息时如何处理消费者组的引用</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAckDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAckDel">命令文档</see></para>
    /// </remarks>
    public DeleteResult XAckDel(
        SharpValue key,
        string group,
        EntryId id,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAckDel(
            key: key,
            group: group,
            delPolicy: delPolicy,
            id: id) | flags, cancellationToken);
    }

    /// <summary>
    /// Acknowledges and conditionally deletes one or multiple entries (messages) for a stream consumer group at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>有条件的确认并删除指定Stream中的一个或多个消息</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The name of the consumer group.
    /// <para>消费组名称</para>
    /// </param>
    /// <param name="ids">One or more stream entry IDs to acknowledge and delete
    /// <para>要确认并删除的Stream消息ID集合, 一个或多个</para>
    /// </param>
    /// <param name="delPolicy">pecifies how to handle consumer group references when acknowledging and deleting entries.
    /// <para>指定在确认和删除消息时如何处理消费者组的引用</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAckDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAckDel">命令文档</see></para>
    /// </remarks>
    public DeleteResult[] XAckDel(
        SharpValue key,
        string group,
        ICollection<EntryId> ids,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAckDel(
            key: key,
            group: group,
            delPolicy: delPolicy,
            ids: ids) | flags, cancellationToken);
    }

    /// <summary>
    /// Acknowledges and conditionally deletes one or multiple entries (messages) for a stream consumer group at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>有条件的确认并删除指定Stream中的一个或多个消息</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The name of the consumer group.
    /// <para>消费组名称</para>
    /// </param>
    /// <param name="ids">One or more stream entry IDs to acknowledge and delete
    /// <para>要确认并删除的Stream消息ID集合, 一个或多个</para>
    /// </param>
    /// <param name="delPolicy">pecifies how to handle consumer group references when acknowledging and deleting entries.
    /// <para>指定在确认和删除消息时如何处理消费者组的引用</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAckDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAckDel">命令文档</see></para>
    /// </remarks>
    public DeleteResult[] XAckDel(
        SharpValue key,
        string group,
        ReadOnlySpan<EntryId> ids,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAckDel(
            key: key,
            group: group,
            delPolicy: delPolicy,
            ids: ids) | flags, cancellationToken);
    }

    /// <summary>
    /// The command returns the stream entries matching a given range of IDs. The range is specified by a minimum and maximum ID. All the entries having an ID between the two specified or exactly one of the two IDs specified (closed interval) are returned.
    /// <para>Available since: 5.0.0</para>
    /// <para>根据指定的消息ID范围, 返回区间里面的消息. 包含ID值相等的也会返回</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="start">The start of the ID range, inclusive. Use - for the smallest ID.
    /// <para>开始扫描的消息ID, 可使用 - 表示最小的ID</para>
    /// </param>
    /// <param name="end">The end of the ID range, inclusive. Use + for the greatest ID.
    /// <para>结束扫描的消息ID, 可使用 + 表示最大的ID</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return.
    /// <para>最多返回消息条数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of stream entries with IDs matching the specified range.
    /// <para>一个包含消息ID和消息内容的数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRange">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    EntryItem[]?
#else
    EntryItem[]
#endif
    XRange(
        SharpValue key,
        EntryId start,
        EntryId end,
        long? count = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XRange(
            key: key,
            start: start,
            end: end,
            count: count) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like XRANGE, but returns entries in reverse order. Specify the end ID first, then the start ID.
    /// <para>Available since: 5.0.0</para>
    /// <para>返回 Stream 中指定 ID 范围内的消息，按 ID 从大到小排序。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="start">The start of the ID range, inclusive. Use - for the smallest ID.
    /// <para>开始扫描的消息ID, 可使用 - 表示最小的ID</para>
    /// </param>
    /// <param name="end">The end of the ID range, inclusive. Entries are returned in reverse order. Use + for the greatest ID.
    /// <para>结束扫描的消息ID, 可使用 + 表示最大的ID</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return.
    /// <para>最多返回消息条数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of stream entries with IDs matching the specified range.
    /// <para>一个包含消息ID和消息内容的数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRevRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRevRange">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    EntryItem[]?
#else
    EntryItem[]
#endif
    XRevRange(
        SharpValue key,
        EntryId end,
        EntryId start,
        long? count = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XRevRange(
            key: key,
            end: end,
            start: start,
            count: count) | flags, cancellationToken);
    }

    /// <summary>
    /// This command transfers ownership of pending stream entries that match the specified criteria. Conceptually, XAUTOCLAIM is equivalent to calling XPENDING and then XCLAIM, but provides a more straightforward way to deal with message delivery failures via SCAN-like semantics.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令会转移符合指定条件的待处理流条目的所有权。从概念上讲， XAUTOCLAIM 等同于先调用 XPENDING 再调用 XCLAIM ，但它提供了一种更直接的方式来处理类似 SCAN 语义的消息传递失败。</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>要操作的Stre key</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer that will own the claimed messages.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="minIdleTime">Claim only messages that have been idle for at least this long, in milliseconds.
    /// <para>仅认领闲置至少达到多少时长（以毫秒为单位）的消息。</para>
    /// </param>
    /// <param name="startId">The message ID to start scanning from. Use 0 to start at the beginning of the PEL.
    /// <para>开始扫描的消息ID, 使用0表示从头开始</para>
    /// </param>
    /// <param name="count">The number of messages to attempt to claim per call. Defaults to 100.
    /// <para>尝试认领的消息最多数量, 默认为100</para>
    /// </param>
    /// <param name="justid">Return only the IDs of the claimed messages, without their fields, and do not increment their retry counters.
    /// <para>仅返回已声明消息的 ID，不包含其字段，并且不增加其重试计数器。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAutoClaim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAutoClaim">命令文档</see></para>
    /// </remarks>
    public XAutoClaimResult XAutoClaim(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        EntryId startId,
        long? count = null,
        bool justid = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XAutoClaim(
            key: key,
            group: group,
            consumer: consumer,
            minIdleTime: minIdleTime,
            start: startId,
            count: count,
            justid: justid) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns summary information about the pending entries in a consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para>获取指定消费者组中待处理消息（PEL）的摘要信息</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>Stream键</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Summary information about pending entries.
    /// <para>待处理消息总数、最小ID、最大ID及各消费者的待处理数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XPending">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XPending">命令文档</see></para>
    /// </remarks>
    public XPendingSummary XPending(
        SharpValue key,
        string group,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XPending(
            key: key,
            group: group,
            options: default) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns detailed information about pending entries in a consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para>获取指定消费者组中待处理消息（PEL）的详细信息</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>Stream键</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="options">Represents the options for querying detailed pending entries using XPENDING.
    /// <para>Encapsulates the ID range, maximum result count, idle time filter, and consumer filter.</para>
    /// <para>表示 Redis XPENDING 命令查询待处理消息详细信息的选项。</para>
    /// <para>封装消息ID范围、最大返回数量、最小闲置时间以及消费者过滤条件。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Summary information about pending entries.
    /// <para>待处理消息总数、最小ID、最大ID及各消费者的待处理数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XPending">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XPending">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    XPendingItem[]?
#else
    XPendingItem[]
#endif
    XPending(
        SharpValue key,
        string group,
        PendingOptions options,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        if (!options.HasValue) throw new ArgumentException("XPENDING command requires options to be specified for detailed information retrieval.", nameof(options));
        return base.Invoke(StreamCommands.XPending(
            key: key,
            group: group,
            options: options) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets the IDMP (Idempotent Message Processing) configuration parameters for a stream. This command configures how long idempotent IDs are retained and the maximum number of idempotent IDs tracked per producer.
    /// <para>Available since: 8.6.0</para>
    /// <para>为 Stream 配置 IDMP（Idempotent Message Processing，幂等消息处理）参数，控制幂等 ID 的保留时长和容量上限。</para>
    /// <para>支持此命令的Redis版本, 8.6.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key. The stream must already exist.
    /// <para>Stream Key, 该Key必须存在, 否则异常</para>
    /// </param>
    /// <param name="duration">Sets the duration, in seconds, for which each idempotent ID (iid) is retained in the stream's IDMP map. Valid range: 1-86,400 seconds. Default: 100 seconds.
    /// <para>幂等 ID 保留时长（秒），范围 1~86400，默认 100</para>
    /// </param>
    /// <param name="maxsize">Sets the maximum number of most recent idempotent IDs kept for each producer in the stream's IDMP map. Valid range: 1-10,000 entries. Default: 100 entries.
    /// <para>每个生产者保留的最大幂等 ID 数，范围 1~10000，默认 100</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the configuration was set successfully.
    /// <para>设置成功返回 true，否则返回 false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XCfgSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XCfgSet">命令文档</see></para>
    /// </remarks>
    public bool XCfgSet(
        SharpValue key,
        int? duration = 100,
        int? maxsize = 100,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XCfgSet(
            key: key,
            duration: duration,
            maxsize: maxsize) | flags, cancellationToken);
    }

    /// <summary>
    /// XCLAIM is used to change (or obtain) the ownership of a certain message within the consumer group, and reassign the message to the specified consumer.
    /// <para>Available since: 5.0.0</para>
    /// <para>XCLAIM 用于改变（或获取）消费者组中某条消息的所有权，将消息重新分配给指定的消费者。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer that will own the claimed messages.
    /// <para>目标消费者名称</para>
    /// </param>
    /// <param name="minIdleTime">Claim only messages that have been idle for at least this long, in milliseconds.
    /// <para>最小空闲时间（毫秒），消息空闲时间必须大于此值才能被认领</para>
    /// </param>
    /// <param name="id">One message IDs to claim.
    /// <para>消息ID</para>
    /// </param>
    /// <param name="idle">Set the idle time (last time it was delivered) of the message. If IDLE is not specified, an idle time of 0 is assumed, that is, the time count is reset because the message now has a new owner trying to process it.
    /// <para>设置消息的空闲时间（毫秒）</para>
    /// </param>
    /// <param name="time">The same as IDLE but sets the idle time to a specific Unix time (in milliseconds) instead of a relative amount of milliseconds.
    /// <para>设置消息的空闲时间为指定的 Unix 时间戳（毫秒）</para>
    /// </param>
    /// <param name="retryCount">Set the retry counter to the specified value
    /// <para>设置消息的重试计数</para>
    /// </param>
    /// <param name="force">Create the pending message entry in the PEL even if the specified IDs are not already in the PEL assigned to a different client.
    /// <para>强制认领，即使消息不在 PEL 中</para>
    /// </param>
    /// <param name="justid">Return just an array of IDs of messages successfully claimed, without returning the actual messages. Using this option means the retry counter is not incremented.
    /// <para>仅返回消息 ID，不返回内容, 且重试计数也不会增加</para>
    /// </param>
    /// <param name="lastid">Update the consumer group's last-delivered ID to the given ID.
    /// <para>设置消费者组的最后投递 ID</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The claimed message entry. When <paramref name="justid"/> is true, only the message ID is populated and the field/value mapping is null. If no message is claimed, a default (empty) entry is returned.
    /// <para>被认领的消息条目。当 justid 为 true 时, 仅填充消息 ID, 不包含字段值; 未认领到任何消息时返回默认（空）条目</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XClaim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XClaim">命令文档</see></para>
    /// </remarks>
    public EntryItem? XClaim(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        EntryId id,
        ulong? idle = null,
        SharpExpiry? time = null,
        ulong? retryCount = null,
        bool force = false,
        bool justid = false,
        EntryId lastid = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XClaim(
            key: key,
            group: group,
            consumer: consumer,
            minIdleTime: minIdleTime,
            id: id,
            idle: idle,
            time: time,
            retryCount: retryCount,
            force: force,
            justid: justid,
            lastid: lastid) | flags, cancellationToken);
    }

    /// <summary>
    /// XCLAIM is used to change (or obtain) the ownership of one or more messages within the consumer group, and reassign the messages to the specified consumer.
    /// <para>Available since: 5.0.0</para>
    /// <para>XCLAIM 用于改变（或获取）消费者组中一条或多条消息的所有权，将消息重新分配给指定的消费者。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer that will own the claimed messages.
    /// <para>目标消费者名称</para>
    /// </param>
    /// <param name="minIdleTime">Claim only messages that have been idle for at least this long, in milliseconds.
    /// <para>最小空闲时间（毫秒），消息空闲时间必须大于此值才能被认领</para>
    /// </param>
    /// <param name="ids">One or more message IDs to claim.
    /// <para>要认领的消息ID, 一个或多个</para>
    /// </param>
    /// <param name="idle">Set the idle time (last time it was delivered) of the message. If IDLE is not specified, an idle time of 0 is assumed, that is, the time count is reset because the message now has a new owner trying to process it.
    /// <para>设置消息的空闲时间（毫秒）</para>
    /// </param>
    /// <param name="time">The same as IDLE but sets the idle time to a specific Unix time (in milliseconds) instead of a relative amount of milliseconds.
    /// <para>设置消息的空闲时间为指定的 Unix 时间戳（毫秒）</para>
    /// </param>
    /// <param name="retryCount">Set the retry counter to the specified value
    /// <para>设置消息的重试计数</para>
    /// </param>
    /// <param name="force">Create the pending message entry in the PEL even if the specified IDs are not already in the PEL assigned to a different client.
    /// <para>强制认领，即使消息不在 PEL 中</para>
    /// </param>
    /// <param name="justid">Return just an array of IDs of messages successfully claimed, without returning the actual messages. Using this option means the retry counter is not incremented.
    /// <para>仅返回消息 ID，不返回内容, 且重试计数也不会增加</para>
    /// </param>
    /// <param name="lastid">Update the consumer group's last-delivered ID to the given ID.
    /// <para>设置消费者组的最后投递 ID</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of the claimed message entries. When <paramref name="justid"/> is true, each entry only contains its message ID and the field/value mapping is null. An empty array is returned if no message is claimed.
    /// <para>被认领的消息条目数组。当 justid 为 true 时, 每个条目仅包含消息 ID, 不包含字段值; 未认领到任何消息时返回空数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XClaim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XClaim">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    EntryItem[]?
#else
    EntryItem[]
#endif
    XClaim(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        ICollection<EntryId> ids,
        ulong? idle = null,
        SharpExpiry? time = null,
        ulong? retryCount = null,
        bool force = false,
        bool justid = false,
        EntryId lastid = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XClaim(
            key: key,
            group: group,
            consumer: consumer,
            minIdleTime: minIdleTime,
            ids: ids,
            idle: idle,
            time: time,
            retryCount: retryCount,
            force: force,
            justid: justid,
            lastid: lastid) | flags, cancellationToken);
    }

    /// <summary>
    /// XCLAIM is used to change (or obtain) the ownership of one or more messages within the consumer group, and reassign the messages to the specified consumer.
    /// <para>Available since: 5.0.0</para>
    /// <para>XCLAIM 用于改变（或获取）消费者组中一条或多条消息的所有权，将消息重新分配给指定的消费者。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer that will own the claimed messages.
    /// <para>目标消费者名称</para>
    /// </param>
    /// <param name="minIdleTime">Claim only messages that have been idle for at least this long, in milliseconds.
    /// <para>最小空闲时间（毫秒），消息空闲时间必须大于此值才能被认领</para>
    /// </param>
    /// <param name="ids">One or more message IDs to claim.
    /// <para>要认领的消息ID, 一个或多个</para>
    /// </param>
    /// <param name="idle">Set the idle time (last time it was delivered) of the message. If IDLE is not specified, an idle time of 0 is assumed, that is, the time count is reset because the message now has a new owner trying to process it.
    /// <para>设置消息的空闲时间（毫秒）</para>
    /// </param>
    /// <param name="time">The same as IDLE but sets the idle time to a specific Unix time (in milliseconds) instead of a relative amount of milliseconds.
    /// <para>设置消息的空闲时间为指定的 Unix 时间戳（毫秒）</para>
    /// </param>
    /// <param name="retryCount">Set the retry counter to the specified value
    /// <para>设置消息的重试计数</para>
    /// </param>
    /// <param name="force">Create the pending message entry in the PEL even if the specified IDs are not already in the PEL assigned to a different client.
    /// <para>强制认领，即使消息不在 PEL 中</para>
    /// </param>
    /// <param name="justid">Return just an array of IDs of messages successfully claimed, without returning the actual messages. Using this option means the retry counter is not incremented.
    /// <para>仅返回消息 ID，不返回内容, 且重试计数也不会增加</para>
    /// </param>
    /// <param name="lastid">Update the consumer group's last-delivered ID to the given ID.
    /// <para>设置消费者组的最后投递 ID</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of the claimed message entries. When <paramref name="justid"/> is true, each entry only contains its message ID and the field/value mapping is null. An empty array is returned if no message is claimed.
    /// <para>被认领的消息条目数组。当 justid 为 true 时, 每个条目仅包含消息 ID, 不包含字段值; 未认领到任何消息时返回空数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XClaim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XClaim">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    EntryItem[]?
#else
    EntryItem[]
#endif
    XClaim(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        ReadOnlySpan<EntryId> ids,
        ulong? idle = null,
        SharpExpiry? time = null,
        ulong? retryCount = null,
        bool force = false,
        bool justid = false,
        EntryId lastid = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XClaim(
            key: key,
            group: group,
            consumer: consumer,
            minIdleTime: minIdleTime,
            ids: ids,
            idle: idle,
            time: time,
            retryCount: retryCount,
            force: force,
            justid: justid,
            lastid: lastid) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes one entries from the stream at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>从 Stream 中删除一个消息条目，并提供对消费组 PEL 引用的精细化控制。</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>Stream 键名</para>
    /// </param>
    /// <param name="id">One stream entry IDs to delete
    /// <para>要删除的消息条目 ID</para>
    /// </param>
    /// <param name="delPolicy">The deletion policy to use.
    /// <para>删除策略</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDelEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDelEx">命令文档</see></para>
    /// </remarks>
    public DeleteResult XDelEx(
        SharpValue key,
        EntryId id,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XDelEx(
            key: key,
            id: id,
            delPolicy: delPolicy) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes one or multiple entries from the stream at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>从 Stream 中删除一个或多个消息条目，并提供对消费组 PEL 引用的精细化控制。</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>Stream 键名</para>
    /// </param>
    /// <param name="ids">One or more stream entry IDs to delete
    /// <para>要删除的消息条目 ID, 一个或多个</para>
    /// </param>
    /// <param name="delPolicy">The deletion policy to use.
    /// <para>删除策略</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDelEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDelEx">命令文档</see></para>
    /// </remarks>
    public DeleteResult[] XDelEx(
        SharpValue key,
        ICollection<EntryId> ids,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XDelEx(
            key: key,
            ids: ids,
            delPolicy: delPolicy) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes one or multiple entries from the stream at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>从 Stream 中删除一个或多个消息条目，并提供对消费组 PEL 引用的精细化控制。</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>Stream 键名</para>
    /// </param>
    /// <param name="ids">One or more stream entry IDs to delete
    /// <para>要删除的消息条目 ID, 一个或多个</para>
    /// </param>
    /// <param name="delPolicy">The deletion policy to use.
    /// <para>删除策略</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDelEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDelEx">命令文档</see></para>
    /// </remarks>
    public DeleteResult[] XDelEx(
        SharpValue key,
        ReadOnlySpan<EntryId> ids,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XDelEx(
            key: key,
            ids: ids,
            delPolicy: delPolicy) | flags, cancellationToken);
    }

    /// <summary>
    /// Create a consumer named consumername in the consumer group groupname of the stream that's stored at key.
    /// <para>Available since: 6.2.0</para>
    /// <para>在指定的消费者组中创建一个新的消费者</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer to create.
    /// <para>要创建的消费者名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of created consumers, either 0 or 1. <b>0 -> False, 1 -> True</b>
    /// <para>成功创建消费者的数量, 1个或0个. <b>0 -> False, 1 -> True</b></para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xgroup-createconsumer">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xgroup-createconsumer">命令文档</see></para>
    /// </remarks>
    public bool XGroupCreateConsumer(SharpValue key, string group, string consumer, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XGroupCreateConsumer(
            key: key,
            group: group,
            consumer: consumer) | flags, cancellationToken);
    }

    /// <summary>
    /// The XGROUP DELCONSUMER command deletes a consumer from the consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para>从 Stream 消费者组中删除一个消费者</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer to delete.
    /// <para>要删除的消费者名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of pending messages the consumer had before it was deleted.
    /// <para>消费者被删除前待处理消息的数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xgroup-delconsumer">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xgroup-delconsumer">命令文档</see></para>
    /// </remarks>
    public int XGroupDelConsumer(SharpValue key, string group, string consumer, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XGroupDelConsumer(
            key: key,
            group: group,
            consumer: consumer) | flags, cancellationToken);
    }

    /// <summary>
    /// The XGROUP DESTROY command completely destroys a consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para>销毁一个 Stream 消费者组</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="group">The name of the consumer group to destroy.
    /// <para>要销毁的消费者组名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of destroyed consumer groups, either 0 or 1. <b>0 -> False, 1 -> True</b>
    /// <para>被摧毁的消费者组的数量， 0个 或 1个. <b>0 -> False, 1 -> True</b></para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xgroup-destroy">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xgroup-destroy">命令文档</see></para>
    /// </remarks>
    public bool XGroupDestroy(SharpValue key, string group, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XGroupDestroy(
           key: key,
           group: group) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets the last-delivered ID of a Stream consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para>设置 Stream 消费者组最后投递的消息 ID。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>Stream 键名</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="id">The last-delivered ID or "$".
    /// <para>最后投递的消息 ID，也可以使用 "$"。</para>
    /// </param>
    /// <param name="entriesRead">
    /// The logical number of entries read by the consumer group.
    /// <para>Available since: 7.0.0</para>
    /// <para>消费者组逻辑已读取消息数量，用于计算 Lag。</para>
    /// <para>Null 表示不指定此参数。</para>
    /// <para>支持此参数的Redis版本, 7.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// True if the last-delivered ID was updated successfully.
    /// <para>成功更新返回 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xgroup-setid">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xgroup-setid">命令文档</see></para>
    /// </remarks>
    public bool XGroupSetID(
        SharpValue key,
        string group,
        EntryId id,
        long? entriesRead = null,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XGroupSetID(
            key: key,
            group: group,
            id: id,
            entriesRead: entriesRead) | flags, cancellationToken);
    }

    /// <summary>
    /// This command returns the list of consumers that belong to the groupname consumer group of the stream stored at key.
    /// <para>Available since: 5.0.0</para>
    /// <para>返回指定 Stream 消费组中所有消费者的详细信息，包括 pending 数量、idle 时间等。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="group">The consumer group name.
    /// <para>消费组名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xinfo-consumers">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xinfo-consumers">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    ConsumerInfo[]?
#else
    ConsumerInfo[]
#endif
    XInfoConsumers(SharpValue key, string group, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XInfoConsumers(
            key: key,
            group: group) | flags, cancellationToken);
    }

    /// <summary>
    /// This command returns the list of all consumer groups of the stream stored at key.
    /// <para>Available since: 5.0.0</para>
    /// <para>获得指定 Stream 的所有消费组列表及其状态信息，包括消费者数量、pending 数量和消费延迟（lag）。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xinfo-groups">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xinfo-groups">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    GroupInfo[]?
#else
    GroupInfo[]
#endif
    XInfoGroups(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XInfoGroups(key: key) | flags, cancellationToken);
    }

    /// <summary>
    /// This command returns information about the stream stored at key.
    /// <para>Available since: 5.0.0</para>
    /// <para>返回指定 Stream 的详细统计信息，包括长度、radix tree 结构、消费组数量和首尾条目等。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="full">Return the full stream state, including all entries and detailed PEL information, instead of a summary.
    /// <para><b>Starting with Redis version 6.0.0</b></para>
    /// <para>返回扩展信息，包括 entries 数组和各消费组的完整状态</para>
    /// <para><b>自Redis 6.0.0+支持该参数</b></para>
    /// </param>
    /// <param name="count">With FULL, the number of entries to return. 0 returns all entries; the default is 10.
    /// <para><b>Starting with Redis version 6.0.0</b></para>
    /// <para>FULL 模式下限制 entries 和 pending 返回数量，默认 10；0 表示全部</para>
    /// <para><b>自Redis 6.0.0+支持该参数</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xinfo-stream">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xinfo-stream">命令文档</see></para>
    /// </remarks>
    public StreamInfo XInfoStream(
        SharpValue key,
        bool full = false,
        long? count = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XInfoStream(key: key, full: full, count: count) | flags, cancellationToken);
    }

    /// <summary>
    /// In the context of a stream consumer group, this command allows a consumer to explicitly release pending messages back to the group's Pending Entries List (PEL) without acknowledging them.
    /// <para>Available since: 8.8.0</para>
    /// <para>将消费者组中的已确认消息重新标记为待处理状态，将其重新放回 PEL（Pending Entries List）中</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">Name of the stream.</param>
    /// <param name="group">Name of the consumer group.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="id">The stream entry ID to be released
    /// <para>要处理的消息ID</para>
    /// </param>
    /// <param name="mode">Controls the delivery counter adjustment
    /// <para>否定确认模式，用于控制消息释放后投递次数的变化</para>
    /// </param>
    /// <param name="retryCount">Directly sets the delivery counter to the specified value, overriding the mode-based adjustment. This gives explicit control over the retry counter regardless of the mode selected.
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>直接设置消息投递次数，覆盖 NACK 模式默认的计数调整行为。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要指定。</i></b></para>
    /// </param>
    /// <param name="force">Creates new unowned PEL entries for IDs that are not already in the group PEL. Each entry must exist in the stream. When FORCE creates an entry, the delivery counter is set to 0 (or to RETRYCOUNT if specified, or to LLONG_MAX if mode is FATAL).
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>当消息存在于 Stream 中、但不在当前消费组 PEL 中时，强制创建无消费者所有权的 PEL 记录。不会创建新的 Stream 消息。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要启用。</i></b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of messages successfully released back to the group PEL. Messages that are not in the consumer group PEL will not be counted.
    /// <para>成功恢复为待处理状态的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XNAck">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XNAck">命令文档</see></para>
    /// </remarks>
    public long XNAck(
        SharpValue key,
        string group,
        EntryId id,
        NackMode mode,
        long? retryCount = null,
        bool force = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XNAck(
            key: key,
            group: group,
            mode: mode,
            id: id,
            retryCount: retryCount,
            force: force) | flags, cancellationToken);
    }

    /// <summary>
    /// In the context of a stream consumer group, this command allows a consumer to explicitly release pending messages back to the group's Pending Entries List (PEL) without acknowledging them.
    /// <para>Available since: 8.8.0</para>
    /// <para>将消费者组中的已确认消息重新标记为待处理状态，将其重新放回 PEL（Pending Entries List）中</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">Name of the stream.</param>
    /// <param name="group">Name of the consumer group.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="ids">Block of arguments that specifies the message IDs to release
    /// <para>要处理的消息ID集合, 一个或多个</para>
    /// </param>
    /// <param name="mode">Controls the delivery counter adjustment
    /// <para>否定确认模式，用于控制消息释放后投递次数的变化</para>
    /// </param>
    /// <param name="retryCount">Directly sets the delivery counter to the specified value, overriding the mode-based adjustment. This gives explicit control over the retry counter regardless of the mode selected.
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>直接设置消息投递次数，覆盖 NACK 模式默认的计数调整行为。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要指定。</i></b></para>
    /// </param>
    /// <param name="force">Creates new unowned PEL entries for IDs that are not already in the group PEL. Each entry must exist in the stream. When FORCE creates an entry, the delivery counter is set to 0 (or to RETRYCOUNT if specified, or to LLONG_MAX if mode is FATAL).
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>当消息存在于 Stream 中、但不在当前消费组 PEL 中时，强制创建无消费者所有权的 PEL 记录。不会创建新的 Stream 消息。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要启用。</i></b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of messages successfully released back to the group PEL. Messages that are not in the consumer group PEL will not be counted.
    /// <para>成功恢复为待处理状态的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XNAck">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XNAck">命令文档</see></para>
    /// </remarks>
    public long XNAck(
        SharpValue key,
        string group,
        ICollection<EntryId> ids,
        NackMode mode,
        long? retryCount = null,
        bool force = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XNAck(
            key: key,
            group: group,
            mode: mode,
            ids: ids,
            retryCount: retryCount,
            force: force) | flags, cancellationToken);
    }

    /// <summary>
    /// In the context of a stream consumer group, this command allows a consumer to explicitly release pending messages back to the group's Pending Entries List (PEL) without acknowledging them.
    /// <para>Available since: 8.8.0</para>
    /// <para>将消费者组中的已确认消息重新标记为待处理状态，将其重新放回 PEL（Pending Entries List）中</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">Name of the stream.</param>
    /// <param name="group">Name of the consumer group.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="ids">Block of arguments that specifies the message IDs to release
    /// <para>要处理的消息ID集合, 一个或多个</para>
    /// </param>
    /// <param name="mode">Controls the delivery counter adjustment
    /// <para>否定确认模式，用于控制消息释放后投递次数的变化</para>
    /// </param>
    /// <param name="retryCount">Directly sets the delivery counter to the specified value, overriding the mode-based adjustment. This gives explicit control over the retry counter regardless of the mode selected.
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>直接设置消息投递次数，覆盖 NACK 模式默认的计数调整行为。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要指定。</i></b></para>
    /// </param>
    /// <param name="force">Creates new unowned PEL entries for IDs that are not already in the group PEL. Each entry must exist in the stream. When FORCE creates an entry, the delivery counter is set to 0 (or to RETRYCOUNT if specified, or to LLONG_MAX if mode is FATAL).
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>当消息存在于 Stream 中、但不在当前消费组 PEL 中时，强制创建无消费者所有权的 PEL 记录。不会创建新的 Stream 消息。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要启用。</i></b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of messages successfully released back to the group PEL. Messages that are not in the consumer group PEL will not be counted.
    /// <para>成功恢复为待处理状态的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XNAck">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XNAck">命令文档</see></para>
    /// </remarks>
    public long XNAck(
        SharpValue key,
        string group,
        ReadOnlySpan<EntryId> ids,
        NackMode mode,
        long? retryCount = null,
        bool force = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XNAck(
            key: key,
            group: group,
            mode: mode,
            ids: ids,
            retryCount: retryCount,
            force: force) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets the last-generated ID of a stream, optionally updating the entries-added counter and maximum deleted entry ID.
    /// <para><b><i>⚠Note: XSETID is an internal command. Users should not call it directly.</i></b></para>
    /// <para>Available since: 5.0.0</para>
    /// <para>设置 Stream 的最后生成消息 ID，并可选择修改累计添加数量和最大已删除消息 ID。</para>
    /// <para><b><i>⚠警告提示: 此命令主要供 Redis 内部复制使用，普通业务代码不建议直接调用。</i></b></para>
    /// <para>支持此命令的 Redis 版本：5.0.0+</para>
    /// </summary>
    /// <param name="key">
    /// The stream key.
    /// <para>Stream 的 Key，必须已经存在。</para>
    /// </param>
    /// <param name="lastId">
    /// The new last-generated entry ID.
    /// <para>要设置的最后生成消息 ID。</para>
    /// <para>不能小于 Stream 中当前最大的有效消息 ID。</para>
    /// </param>
    /// <param name="entriesAdded">
    /// Sets the total number of entries added to the stream.
    /// <para>Available since: 7.0.0</para>
    /// <para>设置 Stream 历史累计添加的消息数量。</para>
    /// <para>该值不能小于 Stream 当前的消息数量。</para>
    /// <para>Null 表示不修改。</para>
    /// <para>自 Redis 7.0.0+ 支持该参数。</para>
    /// </param>
    /// <param name="maxDeletedId">
    /// Sets the maximum deleted entry ID.
    /// <para>Available since: 7.0.0</para>
    /// <para>设置 Stream 历史最大已删除消息 ID。</para>
    /// <para>Null 表示不修改。</para>
    /// <para>自 Redis 7.0.0+ 支持该参数。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// True if the operation succeeds.
    /// <para>执行成功返回 True，否则抛出对应异常。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xsetid">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xsetid">命令文档</see></para>
    /// </remarks>
    public bool XSetID(
        SharpValue key,
        EntryId lastId,
        long? entriesAdded = null,
        EntryId maxDeletedId = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XSetID(
            key: key,
            lastId: lastId,
            entriesAdded: entriesAdded,
            maxDeletedId: maxDeletedId) | flags, cancellationToken);
    }

    /// <summary>
    /// XTRIM trims the stream by evicting older entries (entries with lower IDs) if needed.
    /// <para>Available since: 5.0.0</para>
    /// <para>修剪 Stream 到指定长度或最小 ID。删除最旧的消息。</para>
    /// <para>支持此命令的 Redis 版本：5.0.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>要修剪的Stream Key</para>
    /// </param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of entries deleted from the stream.
    /// <para>从Stream中删除的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XTrim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XTrim">命令文档</see></para>
    /// </remarks>
    public long XTrim(
        SharpValue key,
        TrimOptions trimOptions,
        DeletionPolicy delPolicy,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XTrim(
            key: key,
            trimOptions: trimOptions,
            delPolicy: delPolicy) | flags, cancellationToken);
    }

    /// <summary>
    /// Records IDMP metadata for an existing stream entry.
    /// <para><b><i>⚠ Note: XIDMPRECORD is an internal command. Users should not call it directly.</i></b></para>
    /// <para>Available since: 8.8.0</para>
    /// <para>为 Stream 中已经存在的消息记录生产者 ID 和幂等 ID 的映射关系。</para>
    /// <para><b><i>⚠ 警告提示：这是 Redis 内部命令，主要用于 AOF 恢复，普通业务代码不建议直接调用。</i></b></para>
    /// <para>支持此命令的 Redis 版本：8.8.0+</para>
    /// </summary>
    /// <param name="key">
    /// The stream key.
    /// <para>Stream 的 Key，必须已经存在。</para>
    /// </param>
    /// <param name="pid">
    /// The producer ID.
    /// <para>生产者 ID，用于标识消息生产者，不能为空字符串。</para>
    /// </param>
    /// <param name="iid">
    /// The idempotency ID.
    /// <para>幂等 ID，用于标识生产者发送的唯一消息，不能为空字符串。</para>
    /// </param>
    /// <param name="streamId">
    /// The ID of an existing stream entry.
    /// <para>已经存在于 Stream 中的消息 ID。</para>
    /// <para>不能指定不存在或已经删除的消息 ID。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记，一般情况下无需设置。</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// True if the operation succeeds.
    /// <para>执行成功返回 True，否则抛出对应异常。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xidmprecord">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xidmprecord">命令文档</see></para>
    /// </remarks>
    public bool XIdmpRecord(
        SharpValue key,
        string pid,
        string iid,
        EntryId streamId,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(StreamCommands.XIdmpRecord(
            key: key,
            pid: pid,
            iid: iid,
            streamId: streamId) | flags, cancellationToken);
    }
}
