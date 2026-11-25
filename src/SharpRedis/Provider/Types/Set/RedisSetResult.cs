using SharpRedis.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpRedis;

public sealed partial class RedisSet
{
    /// <summary>
    /// The return value structure of the <b>[SSCAN]</b> command
    /// <para><b>[SSCAN]</b> 命令返回值结构体</para>
    /// </summary>
    public readonly struct SetScanResult : IEnumerable<RedisResult>
    {
        internal static readonly SetScanResult _empty = new();
        private readonly ulong _cursor;
        private readonly RedisResult[] _members;

        /// <summary>
        /// Get the next available cursor. If it is 0, it indicates that the current reading has been completed.
        /// <para>获得下次可用的游标值, 如果返回0, 表示读取已经结束</para>
        /// </summary>
        public readonly ulong NextCursor => this._cursor;

        /// <summary>
        /// Get the number of returned elements.
        /// <para>获取返回的元素数量</para>
        /// </summary>
        public readonly int Count => this._members.Length;

        /// <summary>
        /// Get the number of returned elements.
        /// <para>获取返回的元素数量</para>
        /// </summary>
        public readonly int Length => this._members.Length;

        /// <summary>
        /// Gets the result set
        /// <para>获得结果集</para>
        /// </summary>
        public readonly RedisResult[] Members => this._members;

        public SetScanResult()
        {
            this._members = null!;
        }

        internal SetScanResult(RedisResult value)
        {
            if (value.Type != RespType.Array
                || value.RawValue is not RedisResult[] { Length: 2 } array
                || array[1].Type != RespType.Array)
            {
                throw new InvalidCastException("The return value is not an array and cannot be converted to SetScanResult.");
            }

            this._cursor = array[0];
            if (array[1].IsNull || !array[1].HasValue)
            {
                this._members = [];
                return;
            }

            if (array[1].RawValue is RedisResult[] members)
            {
                this._members = members;
                return;
            }
            throw new InvalidCastException("The return value is not an array and cannot be converted to SetScanResult.");
        }

        /// <summary>
        /// Retrieve the element based on the subscript
        /// <para>根据下标获取元素</para>
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>element</returns>
        public readonly RedisResult this[int index]
            => this._members[index];

        public IEnumerator<RedisResult> GetEnumerator()
            => new SharpEnumerator<RedisResult>(this._members);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }

    /// <summary>
    /// Represents the result of setting expiration for a Set member.
    /// <para>表示设置 Set 成员过期时间的操作结果。</para>
    /// </summary>
    public enum SetExpireResult : sbyte
    {
        /// <summary>
        /// The specified member does not exist, or the key does not exist.
        /// <para>指定的成员不存在，或者 Set Key 不存在。</para>
        /// <para>对应 Redis 返回值：-2。</para>
        /// </summary>
        NotExists = -2,

        /// <summary>
        /// The expiration was not set because the specified condition was not met.
        /// <para>由于指定的 NX、XX、GT 或 LT 条件不满足，未设置或更新成员的过期时间。</para>
        /// <para>对应 Redis 返回值：0。</para>
        /// </summary>
        ConditionNotMet = 0,

        /// <summary>
        /// The expiration time was successfully set or updated.
        /// <para>成功设置或更新成员的过期时间。</para>
        /// <para>对应 Redis 返回值：1。</para>
        /// </summary>
        Success = 1,

        /// <summary>
        /// The member was removed because the specified TTL was zero.
        /// <para>由于指定的 TTL 为 0，成员被立即删除。</para>
        /// <para>对应 Redis 返回值：2。</para>
        /// </summary>
        Deleted = 2,
    }

    /// <summary>
    /// Represents the result of removing expiration from a Set member.
    /// <para>表示移除 Set 成员过期时间的操作结果。</para>
    /// <para>适用于 SPERSIST 命令。</para>
    /// </summary>
    public enum SetPersistResult : sbyte
    {
        /// <summary>
        /// The specified member does not exist, or the key does not exist.
        /// <para>指定的 Set 成员不存在，或者 Set Key 不存在。</para>
        /// <para>对应 Redis 返回值：-2。</para>
        /// </summary>
        NotExists = -2,

        /// <summary>
        /// The member exists but has no associated expiration.
        /// <para>成员存在，但没有设置过期时间，已经是永久有效状态。</para>
        /// <para>无需移除过期时间。</para>
        /// <para>对应 Redis 返回值：-1。</para>
        /// </summary>
        NoExpiration = -1,

        /// <summary>
        /// The expiration was successfully removed.
        /// <para>成功移除成员的过期时间，使成员变为永久有效。</para>
        /// <para>对应 Redis 返回值：1。</para>
        /// </summary>
        Success = 1,
    }
}
