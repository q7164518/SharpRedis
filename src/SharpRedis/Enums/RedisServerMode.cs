namespace SharpRedis;

public enum RedisServerMode : byte
{
    /// <summary>
    /// Single-instance mode
    /// <para>单机模式</para>
    /// </summary>
    Standalone = 0,

    /// <summary>
    /// master/slave mode
    /// <para>主从模式</para>
    /// </summary>
    MasterSlave = 1,

    /// <summary>
    /// Sentinel mode
    /// <para>哨兵模式</para>
    /// </summary>
    Sentinel = 2,

    /// <summary>
    /// Cluster mode
    /// <para>集群模式</para>
    /// </summary>
    Cluster = 3,
}
