using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis functions
/// <para>Redis函数操作类</para>
/// </summary>
public sealed partial class RedisFunction : RedisTypeBase
{
    #region enums
    /// <summary>
    /// Function restore policy
    /// <para>库函数还原策略</para>
    /// <para></para>
    /// </summary>
    public enum RestoreHandling : byte
    {
        /// <summary>
        /// No specification
        /// <para>不指定</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Appends the restored libraries to the existing libraries and aborts on collision. This is the default policy.
        /// <para>将还原的库追加到现有库中, 并在发送冲突时终止, 这是默认策略</para>
        /// </summary>
        Append,

        /// <summary>
        /// Deletes all existing libraries before restoring the payload.
        /// <para>在还原库之前，删除所有现有库</para>
        /// </summary>
        Flush,

        /// <summary>
        /// Appends the restored libraries to the existing libraries, replacing any existing ones in case of name collisions. Note that this policy doesn't prevent function name collisions, only libraries.
        /// <para>将还原的库追加到现有库中, 如果库名冲突, 则替换现有库. 注意: 此策略仅防止库名冲突, 不防止函数名冲突</para>
        /// </summary>
        Replace,
    }
    #endregion

    internal RedisFunction(BaseRedis redis) : base(redis)
    { }

    /// <summary>
    /// Load a library to Redis.
    /// <para>Available since: 7.0.0</para>
    /// <para>将一个函数载入到Redis函数库中</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="functionCode">The library's source code. It must begin with a Shebang statement that declares the engine and library name, for example #!lua name=mylib.
    /// <para>函数源代码。它必须以 Shebang 语句开头，该语句声明脚本引擎和库名称，例如 #!lua name=mylib 。</para>
    /// </param>
    /// <param name="replace">Replace an existing library that has the same name.
    /// <para>库存在的情况下, 是否进行替换. 默认不替换</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The library name that was loaded.
    /// <para>载入成功后的库名称</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/function-load">Command documentation</see>
    /// <para><see href="https://redis.io/commands/function-load">命令文档</see></para>
    /// </remarks>
    public string FunctionLoad(string functionCode, bool replace = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(FunctionCommands.FunctionLoad(functionCode, replace) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Invoke a function.
    /// <para>Available since: 7.0.0</para>
    /// <para>执行一个函数</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="function">The name of the function to call.
    /// <para>要执行的函数名称</para>
    /// </param>
    /// <param name="keys">The key names the function accesses, provided to it via the Lua KEYS global variable.
    /// <para>函数可访问的Key, 在脚本内可通过KEYS参数访问</para>
    /// </param>
    /// <param name="args">Additional arguments provided to the function via the Lua ARGV variable.
    /// <para>函数可通过ARGV参数访问的其它参数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the function that was executed.
    /// <para>返回值取决于函数返回类型</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/FCall">Command documentation</see>
    /// <para><see href="https://redis.io/commands/FCall">命令文档</see></para>
    /// </remarks>
    public RedisResult FCall(string function, ReadOnlySpan<string> keys = default, SharpValueSpan args = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(FunctionCommands.FCall(function, keys, args) | flags, cancellationToken);
    }

    /// <summary>
    /// Invoke a function.
    /// <para>Available since: 7.0.0</para>
    /// <para>执行一个函数</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="function">The name of the function to call.
    /// <para>要执行的函数名称</para>
    /// </param>
    /// <param name="keys">The key names the function accesses, provided to it via the Lua KEYS global variable.
    /// <para>函数可访问的Key, 在脚本内可通过KEYS参数访问</para>
    /// </param>
    /// <param name="args">Additional arguments provided to the function via the Lua ARGV variable.
    /// <para>函数可通过ARGV参数访问的其它参数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the function that was executed.
    /// <para>返回值取决于函数返回类型</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/FCall">Command documentation</see>
    /// <para><see href="https://redis.io/commands/FCall">命令文档</see></para>
    /// </remarks>
    public RedisResult FCall(
        string function,
#if MODERN_NET
        ICollection<string>? keys,
#else
        ICollection<string> keys,
#endif
        SharpValueSpan args = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(FunctionCommands.FCall(function, keys, args) | flags, cancellationToken);
    }

    /// <summary>
    /// This is a read-only variant of the <b>FCALL</b> command that cannot execute commands that modify data.
    /// <para>Available since: 7.0.0</para>
    /// <para><b>FCALL</b> 的只读版, 不能执行修改操作</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="function">The name of the function to call.
    /// <para>要执行的函数名称</para>
    /// </param>
    /// <param name="keys">The key names the function accesses, provided to it via the Lua KEYS global variable.
    /// <para>函数可访问的Key, 在脚本内可通过KEYS参数访问</para>
    /// </param>
    /// <param name="args">Additional arguments provided to the function via the Lua ARGV variable.
    /// <para>函数可通过ARGV参数访问的其它参数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the function that was executed.
    /// <para>返回值取决于函数返回类型</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/FCall_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/FCall_Ro">命令文档</see></para>
    /// </remarks>
    public RedisResult FCall_Ro(string function, ReadOnlySpan<string> keys = default, SharpValueSpan args = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(FunctionCommands.FCall_Ro(function, keys, args) | flags, cancellationToken);
    }

    /// <summary>
    /// This is a read-only variant of the <b>FCALL</b> command that cannot execute commands that modify data.
    /// <para>Available since: 7.0.0</para>
    /// <para><b>FCALL</b> 的只读版, 不能执行修改操作</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="function">The name of the function to call.
    /// <para>要执行的函数名称</para>
    /// </param>
    /// <param name="keys">The key names the function accesses, provided to it via the Lua KEYS global variable.
    /// <para>函数可访问的Key, 在脚本内可通过KEYS参数访问</para>
    /// </param>
    /// <param name="args">Additional arguments provided to the function via the Lua ARGV variable.
    /// <para>函数可通过ARGV参数访问的其它参数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the function that was executed.
    /// <para>返回值取决于函数返回类型</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/FCall_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/FCall_Ro">命令文档</see></para>
    /// </remarks>
    public RedisResult FCall_Ro(
        string function,
#if MODERN_NET
        ICollection<string>? keys,
#else
        ICollection<string> keys,
#endif
        SharpValueSpan args = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(FunctionCommands.FCall_Ro(function, keys, args) | flags, cancellationToken);
    }

    /// <summary>
    /// Delete a library and all its functions.
    /// <para>Available since: 7.0.0</para>
    /// <para>删除库和库下所有函数</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="libraryName">The name of the library to delete.
    /// <para>要删除的库的名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/function-delete">Command documentation</see>
    /// <para><see href="https://redis.io/commands/function-delete">命令文档</see></para>
    /// </remarks>
    public bool FunctionDelete(string libraryName, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(FunctionCommands.FunctionDelete(libraryName) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the serialized payload of loaded libraries. You can restore the serialized payload later with the <b>FUNCTION RESTORE</b> command.
    /// <para>Available since: 7.0.0</para>
    /// <para>序列化并导出已加载的库和函数, 后续可使用 <b>FUNCTION RESTORE</b> 命令还原</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The serialized payload
    /// <para>序列化后的二进制数据</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/function-dump">Command documentation</see>
    /// <para><see href="https://redis.io/commands/function-dump">命令文档</see></para>
    /// </remarks>
    public byte[] FunctionDump(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(FunctionCommands.FunctionDump() | flags | CommandFlags.ReturnBytes, cancellationToken)!;
    }

    /// <summary>
    /// Restore libraries from serialized payload.
    /// <para>Available since: 7.0.0</para>
    /// <para>根据序列化得到的库和函数的二进制数据, 还原库和函数</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="serializedValue">The payload produced by <b>FUNCTION DUMP</b>.
    /// <para><b>FUNCTION DUMP</b> 命令序列化得到的库和函数的二进制数据</para>
    /// </param>
    /// <param name="handling">Restore policy
    /// <para>还原策略</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/function-restore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/function-restore">命令文档</see></para>
    /// </remarks>
    public bool FunctionRestore(byte[] serializedValue, RestoreHandling handling = RestoreHandling.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(FunctionCommands.FunctionRestore(serializedValue, handling) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes all the libraries.
    /// <para>Available since: 7.0.0</para>
    /// <para>删除所有库和函数</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="mode">Flush the function libraries asynchronously (ASYNC) or synchronously (SYNC).
    /// <para>指定删除模式, 异步或同步</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/function-flush">Command documentation</see>
    /// <para><see href="https://redis.io/commands/function-flush">命令文档</see></para>
    /// </remarks>
    public bool FunctionFlush(AsyncSync mode = AsyncSync.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(FunctionCommands.FunctionFlush(mode) | flags, cancellationToken);
    }

    /// <summary>
    /// Kill a function that is currently executing.
    /// <para>Available since: 7.0.0</para>
    /// <para>终止当前正在执行的函数</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/function-kill">Command documentation</see>
    /// <para><see href="https://redis.io/commands/function-kill">命令文档</see></para>
    /// </remarks>
    public bool FunctionKill(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(FunctionCommands.FunctionKill() | flags, cancellationToken);
    }

    /// <summary>
    /// Return information about loaded functions and libraries.
    /// <para>Available since: 7.0.0</para>
    /// <para>列出有关的, 且已加载的函数和库信息</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="libraryNamePattern">List only libraries whose names match the given pattern.
    /// <para>要列出的库名匹配表达式, 不传列出所有的</para>
    /// </param>
    /// <param name="withCode">Include the source code of each library in the reply.
    /// <para>是否包含源代码返回</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/function-list">Command documentation</see>
    /// <para><see href="https://redis.io/commands/function-list">命令文档</see></para>
    /// </remarks>
#if MODERN_NET
    public ListResult[]? FunctionList(string? libraryNamePattern = null, bool withCode = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#else
    public ListResult[] FunctionList(string libraryNamePattern = null, bool withCode = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#endif
    {
        return base.Invoke(FunctionCommands.FunctionList(libraryNamePattern, withCode) | flags, cancellationToken);
    }

    /// <summary>
    /// Return information about the function that's currently running and information about the available execution engines.
    /// <para>Available since: 7.0.0</para>
    /// <para>获得当前正在运行的函数信息, 以及有关可用的执行引擎信息</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Information about the function that's currently running and information about the available execution engines.
    /// <para>当前正在运行的函数信息</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/function-stats">Command documentation</see>
    /// <para><see href="https://redis.io/commands/function-stats">命令文档</see></para>
    /// </remarks>
    public StatsResult FunctionStats(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(FunctionCommands.FunctionStats() | flags, cancellationToken);
    }
}
