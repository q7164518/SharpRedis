using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis cluster management
/// <para>Redis集群管理类, 非集群模式下, 无法执行这些命令</para>
/// <para>If you are new to Redis cluster, see <see href="https://redis.io/docs/operate/oss_and_stack/reference/cluster-spec">Introduction to Redis cluster</see></para>
/// <para>如果你不太懂Redis集群, 请<see href="https://redis.io/docs/operate/oss_and_stack/reference/cluster-spec">查看Redis集群文档</see></para>
/// </summary>
public sealed partial class RedisCluster : RedisTypeBase
{
    internal RedisCluster(BaseRedis redis) : base(redis) { }

    /// <summary>
    /// <b>CLUSTER SLOTS</b> returns details about which cluster slots map to which Redis instances.
    /// <para>Available since: 3.0.0</para>
    /// <para><b>CLUSTER SLOTS</b> 返回集群中所有槽到节点的映射关系</para>
    /// <para>支持此命令的Redis版本, 3.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cluster-slots">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cluster-slots">命令文档</see></para>
    /// </remarks>
    public SlotInfo[] Slots(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ClusterCommands.Slots() | flags, cancellationToken);
    }

    /// <summary>
    /// When a cluster client receives an -ASK redirect, the ASKING command is sent to the target node followed by the command which was redirected. This is normally done automatically by cluster clients.
    /// <para>Available since: 3.0.0</para>
    /// <para>将下一个命令发送给正在迁移槽位的节点。用于处理 ASK 重定向。</para>
    /// <para>支持此命令的Redis版本, 3.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/askings">Command documentation</see>
    /// <para><see href="https://redis.io/commands/asking">命令文档</see></para>
    /// </remarks>
    public bool Asking(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ClusterCommands.Asking() | flags, cancellationToken);
    }

    /// <summary>
    /// Enables read queries for a connection to a Redis Cluster replica node.
    /// <para>Available since: 3.0.0</para>
    /// <para>在 Redis Cluster 副本节点上启用只读查询。默认副本节点拒绝读命令。</para>
    /// <para>支持此命令的Redis版本, 3.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Readonly">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Readonly">命令文档</see></para>
    /// </remarks>
    public bool Readonly(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ClusterCommands.Readonly() | flags, cancellationToken);
    }

    /// <summary>
    /// Each node in a Redis Cluster has its view of the current cluster configuration, given by the set of known nodes, the state of the connection we have with such nodes, their flags, properties and assigned slots, and so forth.
    /// <para>Available since: 3.0.0</para>
    /// <para>返回集群中所有节点的配置信息、状态、角色和连接关系，是 Redis Cluster 运维和诊断的核心命令</para>
    /// <para>支持此命令的Redis版本, 3.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the serialized cluster configuration.
    /// <para>多行文本，每行代表一个节点</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cluster-nodes">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cluster-nodes">命令文档</see></para>
    /// </remarks>
    public string Nodes(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ClusterCommands.Nodes() | flags, cancellationToken)!;
    }

    /// <summary>
    /// Returns an integer identifying the hash slot the specified key hashes to.
    /// <para>Available since: 3.0.0</para>
    /// <para>返回指定 key 的哈希槽编号，用于判断 key 在集群中的存储位置</para>
    /// <para>支持此命令的Redis版本, 3.0.0+</para>
    /// </summary>
    /// <param name="key">The key name to compute the hash slot for.
    /// <para>要计算哈希槽的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The hash slot number for the specified key
    /// <para>该 key 对应的哈希槽编号，范围 0 ~ 16383</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cluster-keyslot">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cluster-keyslot">命令文档</see></para>
    /// </remarks>
    public long KeySlot(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ClusterCommands.KeySlot(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Disables read queries for a connection to a Redis Cluster replica node.
    /// <para>Available since: 3.0.0</para>
    /// <para>将当前连接的只读副本节点切换为读写模式（仅适用于副本节点）</para>
    /// <para>支持此命令的Redis版本, 3.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/readwrite">Command documentation</see>
    /// <para><see href="https://redis.io/commands/readwrite">命令文档</see></para>
    /// </remarks>
    public bool Readwrite(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ClusterCommands.Readwrite() | flags, cancellationToken);
    }

    /// <summary>
    /// This command is useful in order to modify a node's view of the cluster configuration. Specifically it assigns a set of hash slots to the node receiving the command. If the command is successful, the node will map the specified hash slots to itself, and will start broadcasting the new configuration.
    /// <para>Available since: 3.0.0</para>
    /// <para>将指定槽位分配给当前节点</para>
    /// <para>支持此命令的Redis版本, 3.0.0+</para>
    /// </summary>
    /// <param name="slot">One hash slots to assign to the current node.
    /// <para>槽位编号（0-16383）</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cluster-addslots">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cluster-addslots">命令文档</see></para>
    /// </remarks>
    public bool AddSlots(int slot, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ClusterCommands.AddSlots(slot) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is useful in order to modify a node's view of the cluster configuration. Specifically it assigns a set of hash slots to the node receiving the command. If the command is successful, the node will map the specified hash slots to itself, and will start broadcasting the new configuration.
    /// <para>Available since: 3.0.0</para>
    /// <para>将指定槽位分配给当前节点</para>
    /// <para>支持此命令的Redis版本, 3.0.0+</para>
    /// </summary>
    /// <param name="slots">One or more hash slots to assign to the current node.
    /// <para>槽位编号（0-16383），可指定多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cluster-addslots">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cluster-addslots">命令文档</see></para>
    /// </remarks>
    public bool AddSlots(ReadOnlySpan<int> slots, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ClusterCommands.AddSlots(slots) | flags, cancellationToken);
    }
}
