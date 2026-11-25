using SharpRedis.Enums;
using System;
using System.Collections.Generic;

namespace SharpRedis;

public sealed partial class RedisCluster
{
    /// <summary>
    /// Redis Cluster slot range information.
    /// <para>CLUSTER SLOTS 返回的单个哈希槽范围信息。</para>
    /// </summary>
    public readonly struct SlotInfo
    {
        /// <summary>
        /// Starting slot, inclusive.
        /// <para>起始哈希槽，包含此槽位。</para>
        /// </summary>
        public readonly int StartSlot { get; }

        /// <summary>
        /// Ending slot, inclusive.
        /// <para>结束哈希槽，包含此槽位。</para>
        /// </summary>
        public readonly int EndSlot { get; }

        /// <summary>
        /// Master node responsible for this slot range.
        /// <para>负责该哈希槽范围的主节点。</para>
        /// </summary>
        public readonly SlotNode Master { get; }

        /// <summary>
        /// Replica nodes for this slot range.
        /// <para>该哈希槽范围对应的从节点集合。</para>
        /// <para>没有从节点时返回空数组。</para>
        /// </summary>
        public readonly SlotNode[] Replicas { get; }

        internal SlotInfo(RedisResult value)
        {
            if (value.Type != RespType.Array || value.RawValue is not RedisResult[] { Length: >= 3 } values)
            {
                throw new InvalidCastException("Invalid CLUSTER SLOTS slot range response.");
            }

            this.StartSlot = values[0];
            this.EndSlot = values[1];

            if (this.StartSlot < 0 ||
                this.EndSlot > 16383 ||
                this.StartSlot > this.EndSlot)
            {
                throw new InvalidCastException("Invalid CLUSTER SLOTS slot range.");
            }

            this.Master = new SlotNode(values[2]);

            this.Replicas = new SlotNode[values.Length - 3];

            for (int i = 3; i < values.Length; i++)
            {
                this.Replicas[i - 3] = new SlotNode(values[i]);
            }
        }

        /// <summary>
        /// Whether the slot belongs to this range.
        /// <para>判断指定哈希槽是否属于当前范围。</para>
        /// </summary>
        public bool Contains(int slot)
        {
            return slot >= this.StartSlot && slot <= this.EndSlot;
        }

        /// <summary>
        /// Parse the complete CLUSTER SLOTS response.
        /// <para>解析完整的 CLUSTER SLOTS 命令返回值。</para>
        /// </summary>
        internal static SlotInfo[] Parse(RedisResult value)
        {
            if (value.Type != RespType.Array || value.RawValue is not RedisResult[] values)
            {
                throw new InvalidCastException("Invalid CLUSTER SLOTS response.");
            }
            var result = new SlotInfo[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = new(values[i]);
            }
            return result;
        }
    }

    /// <summary>
    /// Redis Cluster node networking information.
    /// <para>CLUSTER SLOTS 返回的节点网络信息。</para>
    /// </summary>
    public readonly struct SlotNode
    {
        /// <summary>
        /// Preferred endpoint (IP address or hostname).
        /// <para>首选连接端点，可以是 IP、域名、空字符串、问号或 null。</para>
        /// <para>Redis 7.0+ 支持 hostname 和未知端点。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? Endpoint { get; }
#else
        public readonly string Endpoint { get; }
#endif

        /// <summary>
        /// Redis service port.
        /// <para>Redis 服务端口。</para>
        /// </summary>
        public readonly int Port { get; }

        /// <summary>
        /// Redis Cluster node ID.
        /// <para>Available since: Redis 4.0.0</para>
        /// <para>节点唯一 ID，Redis 4.0 之前可能不存在。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? NodeId { get; }
#else
        public readonly string NodeId { get; }
#endif

        /// <summary>
        /// Additional advertised IP address.
        /// <para>Available since: Redis 7.0.0</para>
        /// <para>附加网络元数据中的 IP 地址。</para>
        /// <para>未提供时返回 null。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? Ip { get; }
#else
        public readonly string Ip { get; }
#endif

        /// <summary>
        /// Additional advertised hostname.
        /// <para>Available since: Redis 7.0.0</para>
        /// <para>附加网络元数据中的主机名。</para>
        /// <para>未提供时返回 null。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? Hostname { get; }
#else
        public readonly string Hostname { get; }
#endif

        /// <summary>
        /// Whether the client should reuse its connection endpoint.
        /// <para>是否需要回退使用发送 CLUSTER SLOTS 时的连接端点。</para>
        /// <para>注意：应使用当前节点返回的 Port。</para>
        /// </summary>
        public readonly bool RequiresEndpointFallback => string.IsNullOrEmpty(this.Endpoint);

        /// <summary>
        /// Whether the preferred endpoint is unknown.
        /// <para>首选端点是否为问号，表示节点地址未知。</para>
        /// <para>不能将问号当作当前连接地址。</para>
        /// </summary>
        public readonly bool HasUnknownEndpoint => this.Endpoint == "?";

        internal SlotNode(RedisResult value)
        {
            if (value.Type != RespType.Array || value.RawValue is not RedisResult[] { Length: >= 2 } values)
            {
                throw new InvalidCastException("Invalid CLUSTER SLOTS node response.");
            }

            this.Endpoint = SlotNode.GetNullableString(values[0]);
            this.Port = values[1];

            // Redis 4.0+ 新增 Node ID。
            this.NodeId = values.Length >= 3
                ? SlotNode.GetNullableString(values[2])
                : null;

            this.Ip = null;
            this.Hostname = null;

            // Redis 7.0+ 新增网络元数据。
            if (values.Length >= 4)
            {
                SlotNode.ParseMetadata(
                    values[3],
                    out var ip,
                    out var hostname);

                this.Ip = ip;
                this.Hostname = hostname;
            }
        }

#if MODERN_NET
        private static string? GetNullableString(RedisResult value)
#else
        private static string GetNullableString(RedisResult value)
#endif
        {
            if (value.IsNullOrEmpty) return null;
            return value;
        }

        private static void ParseMetadata(
            RedisResult value,
#if MODERN_NET
            out string? ip,
            out string? hostname
#else
            out string ip,
            out string hostname
#endif
        )
        {
            ip = null;
            hostname = null;
            if (value.IsNullOrEmpty) return;

            // RESP3 Map。
            if (value.Type == RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] maps)
            {
                foreach (var entry in maps)
                {
                    SlotNode.AssignMetadata(
                        SlotNode.GetNullableString(entry.Key),
                        SlotNode.GetNullableString(entry.Value),
                        ref ip,
                        ref hostname);
                }

                return;
            }

            // RESP2：交替排列的 Key/Value 数组。
            if (value.Type == RespType.Array &&
                value.RawValue is RedisResult[] items &&
                items.Length % 2 == 0)
            {
                for (int i = 0; i < items.Length; i += 2)
                {
                    SlotNode.AssignMetadata(
                        SlotNode.GetNullableString(items[i]),
                        SlotNode.GetNullableString(items[i + 1]),
                        ref ip,
                        ref hostname);
                }

                return;
            }

            throw new InvalidCastException("Invalid CLUSTER SLOTS networking metadata.");
        }

        private static void AssignMetadata(
#if MODERN_NET
            string? name,
            string? value,
            ref string? ip,
            ref string? hostname
#else
            string name,
            string value,
            ref string ip,
            ref string hostname
#endif
        )
        {
            if (string.Equals(name, "ip", StringComparison.OrdinalIgnoreCase))
            {
                ip = value;
            }
            else if (string.Equals(name, "hostname", StringComparison.OrdinalIgnoreCase))
            {
                hostname = value;
            }
        }
    }
}
