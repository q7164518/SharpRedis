using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Provider.Standard;

/// <summary>
/// Execution interface for command pipeline operations
/// <para>命令管道的执行接口</para>
/// </summary>
public interface IRedisPipeliningClient : IRedis
{
    /// <summary>
    /// Execute all the commands in the pipeline and obtain the return value
    /// <para>Return an array of values in the same order as the commands in the pipe</para>
    /// <para>执行管道中的所有命令, 并获得返回值. 返回值数组顺序和管道执行的命令顺序一致</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    ExecutePipelining(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute all the commands in the pipeline and obtain the return value
    /// <para>Return an array of values in the same order as the commands in the pipe</para>
    /// <para>执行管道中的所有命令, 并获得返回值. 返回值数组顺序和管道执行的命令顺序一致</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    ExecutePipeliningAsync(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute all the commands in the pipeline and obtain the return value
    /// <para>Return an array of values in the same order as the commands in the pipe</para>
    /// <para>执行管道中的所有命令, 并获得返回值. 返回值数组顺序和管道执行的命令顺序一致</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    InvokePipelining(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute all the commands in the pipeline and obtain the return value
    /// <para>Return an array of values in the same order as the commands in the pipe</para>
    /// <para>执行管道中的所有命令, 并获得返回值. 返回值数组顺序和管道执行的命令顺序一致</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    InvokePipeliningAsync(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute all the commands in the pipeline and obtain the return value
    /// <para>Return an array of values in the same order as the commands in the pipe</para>
    /// <para>执行管道中的所有命令, 并获得返回值. 返回值数组顺序和管道执行的命令顺序一致</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    RunPipelining(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute all the commands in the pipeline and obtain the return value
    /// <para>Return an array of values in the same order as the commands in the pipe</para>
    /// <para>执行管道中的所有命令, 并获得返回值. 返回值数组顺序和管道执行的命令顺序一致</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    RunPipeliningAsync(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute all the commands in the pipeline and obtain the return value
    /// <para>Return an array of values in the same order as the commands in the pipe</para>
    /// <para>执行管道中的所有命令, 并获得返回值. 返回值数组顺序和管道执行的命令顺序一致</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    EndPipelining(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute all the commands in the pipeline and obtain the return value
    /// <para>Return an array of values in the same order as the commands in the pipe</para>
    /// <para>执行管道中的所有命令, 并获得返回值. 返回值数组顺序和管道执行的命令顺序一致</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    EndPipeliningAsync(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);
}
