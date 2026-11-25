using SharpRedis.Enums;
using System;
using System.Collections.Generic;

namespace SharpRedis;

public sealed partial class RedisKeyspace
{
    /// <summary>
    /// Result of the WAITAOF command.
    /// <para>WAITAOF 命令的返回结果，表示已完成 AOF 持久化确认的本地节点和从节点数量。</para>
    /// </summary>
    public readonly struct WaitAofResult
    {
        /// <summary>
        /// Number of local Redis instances that have fsynced the previous writes to AOF.
        /// <para>本地 Redis 已将当前连接先前的写入完成 AOF fsync 的数量。</para>
        /// <para>返回值只能为 0 或 1。</para>
        /// </summary>
        public readonly int Local { get; }

        /// <summary>
        /// Number of replicas that have fsynced the previous writes to AOF.
        /// <para>已将当前连接先前的写入完成 AOF fsync 的从节点数量。</para>
        /// <para>返回值为 0 或更大的整数。</para>
        /// </summary>
        public readonly long Replicas { get; }

        internal WaitAofResult(RedisResult value)
        {
            if (value.Type is not RespType.Array || value.RawValue is not RedisResult[] { Length: 2 } values)
            {
                throw new InvalidCastException("Invalid WAITAOF response.");
            }

            this.Local = values[0];
            this.Replicas = values[1];
        }
    }

    /// <summary>
    /// Result of the SCAN command.
    /// <para>SCAN 命令遍历Key的返回值结构体</para>
    /// </summary>
    public readonly struct ScanResult
    {
#if MODERN_NET
        private readonly RedisResult[]? _keys;
#else
        private readonly RedisResult[] _keys;
#endif

        /// <summary>
        /// that represents an unsigned 64-bit number, the cursor.
        /// <para>下一次遍历时的游标</para>
        /// </summary>
        public readonly long NextCursor { get; }

        /// <summary>
        /// Gets the names of scanned keys.
        /// <para>获得返回的Key集合</para>
        /// </summary>
#if MODERN_NET
        public readonly IReadOnlyList<RedisResult>? Keys => this._keys;
#else
        public readonly IReadOnlyList<RedisResult> Keys => this._keys;
#endif

        /// <summary>
        /// The number of keys obtained from this scan
        /// <para>获得本次扫描返回的Key数量</para>
        /// </summary>
        public readonly int Count => this._keys?.Length ?? 0;

        internal ScanResult(RedisResult value)
        {
            if (value.Type != RespType.Array || value.RawValue is not RedisResult[] { Length: 2 } array)
            {
                throw new InvalidCastException("Invalid SCAN response.");
            }
            this.NextCursor = array[0];
            this._keys = array[1];
        }
    }
}
