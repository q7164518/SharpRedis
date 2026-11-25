using SharpRedis.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis;

public sealed partial class RedisHyperLogLog
{
    /// <summary>
    /// Adds all the element arguments to the HyperLogLog data structure stored at the variable name specified as first argument.
    /// <para>Available since: 2.8.9</para>
    /// <para>添加一个元素到指定Key的HyperLogLog数据结构中</para>
    /// <para>支持此命令的Redis版本, 2.8.9+</para>
    /// </summary>
    /// <param name="key">HyperLogLog key</param>
    /// <param name="element">element<para>元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if at least one HyperLogLog internal register was altered.
    /// <para>False if no HyperLogLog internal registers were altered.</para>
    /// <para>存在被修改的情况返回true, 否则返回false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PFAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PFAdd">命令文档</see></para>
    /// </remarks>
    public Task<bool> PFAddAsync(SharpValue key, SharpValue element, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(HyperLogLogCommands.PFAdd(key, element) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds all the element arguments to the HyperLogLog data structure stored at the variable name specified as first argument.
    /// <para>Available since: 2.8.9</para>
    /// <para>添加多个元素到指定Key的HyperLogLog数据结构中</para>
    /// <para>支持此命令的Redis版本, 2.8.9+</para>
    /// </summary>
    /// <param name="key">HyperLogLog key</param>
    /// <param name="elements">elements<para>元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if at least one HyperLogLog internal register was altered.
    /// <para>False if no HyperLogLog internal registers were altered.</para>
    /// <para>存在被修改的情况返回true, 否则返回false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PFAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PFAdd">命令文档</see></para>
    /// </remarks>
    public Task<bool> PFAddAsync(SharpValue key, SharpValueSpan elements, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(HyperLogLogCommands.PFAdd(key, elements) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the approximated cardinality computed by the HyperLogLog data structure stored at the specified variable, which is 0 if the variable does not exist.
    /// <para>Available since: 2.8.9</para>
    /// <para>返回指定HyperLogLog数据结构计算出的近似元素数量，如果Key不存在则返回0</para>
    /// <para>支持此命令的Redis版本, 2.8.9+</para>
    /// </summary>
    /// <param name="key">HyperLogLog key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The approximated cardinality of the HyperLogLog data structure.
    /// <para>近似元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PFCount">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PFCount">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> PFCountAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(HyperLogLogCommands.PFCount(key) | flags, cancellationToken);
    }

    /// <summary>
    /// returns the approximated cardinality of the union of the HyperLogLogs passed, by internally merging the HyperLogLogs stored at the provided keys into a temporary HyperLogLog.
    /// <para>Available since: 2.8.9</para>
    /// <para>返回指定的多个HyperLogLog数据结构计算出的近似元素数量</para>
    /// <para>支持此命令的Redis版本, 2.8.9+</para>
    /// </summary>
    /// <param name="keys">HyperLogLog keys</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The approximated cardinality of the HyperLogLog data structure.
    /// <para>近似元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PFCount">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PFCount">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> PFCountAsync(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(HyperLogLogCommands.PFCount(keys) | flags, cancellationToken);
    }

    /// <summary>
    /// Merge multiple HyperLogLog values into a unique value that will approximate the cardinality of the union of the observed Sets of the source HyperLogLog structures.
    /// <para>Available since: 2.8.9</para>
    /// <para>将多个HyperLogLog合并为一个</para>
    /// </summary>
    /// <param name="destKey">The key for storing the result
    /// <para>存储合并后结果的Key</para>
    /// </param>
    /// <param name="sourceKey">Source key
    /// <para>源Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>OK</returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PFMerge">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PFMerge">命令文档</see></para>
    /// </remarks>
    public Task<bool> PFMergeAsync(SharpValue destKey, SharpValue sourceKey, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(HyperLogLogCommands.PFMerge(destKey, sourceKey) | flags, cancellationToken);
    }

    /// <summary>
    /// Merge multiple HyperLogLog values into a unique value that will approximate the cardinality of the union of the observed Sets of the source HyperLogLog structures.
    /// <para>Available since: 2.8.9</para>
    /// <para>将多个HyperLogLog合并为一个</para>
    /// </summary>
    /// <param name="destKey">The key for storing the result
    /// <para>存储合并后结果的Key</para>
    /// </param>
    /// <param name="sourceKeys">Source key list
    /// <para>源Key集合</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>OK</returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PFMerge">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PFMerge">命令文档</see></para>
    /// </remarks>
    public Task<bool> PFMergeAsync(SharpValue destKey, SharpValueSpan sourceKeys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(HyperLogLogCommands.PFMerge(destKey, sourceKeys) | flags, cancellationToken);
    }
}
