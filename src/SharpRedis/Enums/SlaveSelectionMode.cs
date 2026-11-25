namespace SharpRedis;

/// <summary>
/// Slave selection strategy.
/// <para>从节点选择策略。</para>
/// </summary>
public enum SlaveSelectionMode : byte
{
    /// <summary>
    /// Select available slaves in turn. 
    /// <para>轮询可用从节点。</para>
    /// </summary>
    RoundRobin,

    /// <summary>
    /// Select randomly among available slaves.
    /// <para>随机选择可用从节点。</para>
    /// </summary>
    Random,
}
