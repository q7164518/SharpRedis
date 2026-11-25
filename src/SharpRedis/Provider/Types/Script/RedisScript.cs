using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis lua script
/// <para>Lua脚本操作类型</para>
/// </summary>
public sealed partial class RedisScript : RedisTypeBase
{
    #region enums
    /// <summary>
    /// Enumeration of Lua script debugging modes
    /// <para>Lua脚本调试模式枚举</para>
    /// </summary>
    public enum DebugMode : byte
    {
        /// <summary>
        /// Enable non-blocking asynchronous debugging of Lua scripts (changes are discarded).
        /// <para>启用 Lua 脚本的非阻塞异步调试（更改将被丢弃）。</para>
        /// </summary>
        Yes = 0,

        /// <summary>
        /// Enable blocking synchronous debugging of Lua scripts (saves changes to data).
        /// <para>启用 Lua 脚本的同步调试阻断功能（保存数据更改）。</para>
        /// </summary>
        Sync,

        /// <summary>
        /// Disable Lua script debug mode. Subsequent scripts run normally, without LDB.
        /// <para>禁用 Lua 脚本调试模式。后续脚本将正常运行，无需 LDB。</para>
        /// </summary>
        No,
    }
    #endregion

    internal RedisScript(BaseRedis redis) : base(redis) { }

    /// <summary>
    /// Executes a server-side Lua script with the embedded Redis Lua 5.1 interpreter.
    /// <para>Available since: 2.6.0</para>
    /// <para>执行Lua脚本</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="script">The Lua script to evaluate.
    /// <para>要执行的Lua脚本代码</para>
    /// </param>
    /// <param name="keys">The key names the script accesses, provided to it via the Lua global KEYS runtime variable.
    /// <para>提供给Lua脚本的Key, 在脚本里面使用KEYS访问</para>
    /// </param>
    /// <param name="argv">Additional arguments provided to the script via the Lua ARGV variable.
    /// <para>提供给Lua脚本的其它参数, 在脚本里面通过ARGV访问</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the script that was executed.
    /// <para>脚本返回的值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Eval">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Eval">命令文档</see></para>
    /// </remarks>
    public RedisResult Eval(string script, ReadOnlySpan<string> keys = default, SharpValueSpan argv = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.Eval(script, keys, argv) | flags, cancellationToken);
    }

    /// <summary>
    /// Executes a server-side Lua script with the embedded Redis Lua 5.1 interpreter.
    /// <para>Available since: 2.6.0</para>
    /// <para>执行Lua脚本</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="script">The Lua script to evaluate.
    /// <para>要执行的Lua脚本代码</para>
    /// </param>
    /// <param name="keys">The key names the script accesses, provided to it via the Lua global KEYS runtime variable.
    /// <para>提供给Lua脚本的Key, 在脚本里面使用KEYS访问</para>
    /// </param>
    /// <param name="argv">Additional arguments provided to the script via the Lua ARGV variable.
    /// <para>提供给Lua脚本的其它参数, 在脚本里面通过ARGV访问</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the script that was executed.
    /// <para>脚本返回的值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Eval">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Eval">命令文档</see></para>
    /// </remarks>
    public RedisResult Eval(
        string script,
#if MODERN_NET
        ICollection<string>? keys,
#else
        ICollection<string> keys,
#endif
        SharpValueSpan argv = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.Eval(script, keys, argv) | flags, cancellationToken);
    }

    /// <summary>
    /// Runs a read-only Lua script. Unlike <b>EVAL</b>, this command can’t run commands that modify data.
    /// <para>Available since: 7.0.0</para>
    /// <para>执行只读的Lua脚本, 与 <b>EVAL</b> 不同的是: 此命令不能执行修改数据的脚本</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="script">The Lua script to evaluate.
    /// <para>要执行的Lua脚本代码</para></param>
    /// <param name="keys">The key names the script accesses, provided to it via the Lua global KEYS runtime variable.
    /// <para>提供给Lua脚本的Key, 在脚本里面使用KEYS访问</para>
    /// </param>
    /// <param name="argv">Additional arguments provided to the script via the Lua ARGV variable.
    /// <para>提供给Lua脚本的其它参数, 在脚本里面通过ARGV访问</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the script that was executed.
    /// <para>脚本返回的值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Eval_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Eval_Ro">命令文档</see></para>
    /// </remarks>
    public RedisResult Eval_Ro(string script, ReadOnlySpan<string> keys = default, SharpValueSpan argv = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.Eval_Ro(script, keys, argv) | flags, cancellationToken);
    }

    /// <summary>
    /// Runs a read-only Lua script. Unlike <b>EVAL</b>, this command can’t run commands that modify data.
    /// <para>Available since: 7.0.0</para>
    /// <para>执行只读的Lua脚本, 与 <b>EVAL</b> 不同的是: 此命令不能执行修改数据的脚本</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="script">The Lua script to evaluate.
    /// <para>要执行的Lua脚本代码</para></param>
    /// <param name="keys">The key names the script accesses, provided to it via the Lua global KEYS runtime variable.
    /// <para>提供给Lua脚本的Key, 在脚本里面使用KEYS访问</para>
    /// </param>
    /// <param name="argv">Additional arguments provided to the script via the Lua ARGV variable.
    /// <para>提供给Lua脚本的其它参数, 在脚本里面通过ARGV访问</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the script that was executed.
    /// <para>脚本返回的值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Eval_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Eval_Ro">命令文档</see></para>
    /// </remarks>
    public RedisResult Eval_Ro(
        string script,
#if MODERN_NET
        ICollection<string>? keys,
#else
        ICollection<string> keys,
#endif
        SharpValueSpan argv = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.Eval_Ro(script, keys, argv) | flags, cancellationToken);
    }

    /// <summary>
    /// Load a script into the scripts cache, without executing it. After the specified command is loaded into the script cache it will be callable using <b>EVALSHA</b> with the correct SHA1 digest of the script, exactly like after the first successful invocation of <b>EVAL</b>.
    /// <para>Available since: 2.6.0</para>
    /// <para>将Lua脚本加载并缓存, 获得脚本的SHA1值. 此操作不会执行脚本, 后续需要使用 <b>EVALSHA</b> 命令搭配正确的脚本SHA1值执行脚本</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="script">The Lua script to load into the script cache.
    /// <para>要加载并缓存的Lua脚本代码</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The SHA1 digest of the script added into the script cache.
    /// <para>加载到缓存中的Lua脚本SHA1值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/script-load">Command documentation</see>
    /// <para><see href="https://redis.io/commands/script-load">命令文档</see></para>
    /// </remarks>
    public string ScriptLoad(string script, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.ScriptLoad(script) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Evaluate a script from the server's cache by its SHA1 digest.
    /// <para>Available since: 2.6.0</para>
    /// <para>通过Lua脚本的SHA1值运行脚本</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="sha1">The SHA1 digest of a script previously cached with <b>SCRIPT LOAD</b> or EVAL.
    /// <para>使用 <b>SCRIPT LOAD</b> 命令加载缓存Lua脚本返回的SHA1值</para>
    /// </param>
    /// <param name="keys">The key names the script accesses, provided to it via the Lua global KEYS runtime variable.
    /// <para>提供给Lua脚本的Key, 在脚本里面使用KEYS访问</para>
    /// </param>
    /// <param name="argv">Additional arguments provided to the script via the Lua ARGV variable.
    /// <para>提供给Lua脚本的其它参数, 在脚本里面通过ARGV访问</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the script that was executed.
    /// <para>脚本返回的值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/EvalSHA">Command documentation</see>
    /// <para><see href="https://redis.io/commands/EvalSHA">命令文档</see></para>
    /// </remarks>
    public RedisResult EvalSHA(string sha1, ReadOnlySpan<string> keys = default, SharpValueSpan argv = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.EvalSHA(sha1, keys, argv) | flags, cancellationToken);
    }

    /// <summary>
    /// Evaluate a script from the server's cache by its SHA1 digest.
    /// <para>Available since: 2.6.0</para>
    /// <para>通过Lua脚本的SHA1值运行脚本</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="sha1">The SHA1 digest of a script previously cached with <b>SCRIPT LOAD</b> or EVAL.
    /// <para>使用 <b>SCRIPT LOAD</b> 命令加载缓存Lua脚本返回的SHA1值</para>
    /// </param>
    /// <param name="keys">The key names the script accesses, provided to it via the Lua global KEYS runtime variable.
    /// <para>提供给Lua脚本的Key, 在脚本里面使用KEYS访问</para>
    /// </param>
    /// <param name="argv">Additional arguments provided to the script via the Lua ARGV variable.
    /// <para>提供给Lua脚本的其它参数, 在脚本里面通过ARGV访问</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the script that was executed.
    /// <para>脚本返回的值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/EvalSHA">Command documentation</see>
    /// <para><see href="https://redis.io/commands/EvalSHA">命令文档</see></para>
    /// </remarks>
    public RedisResult EvalSHA(
        string sha1,
#if MODERN_NET
        ICollection<string>? keys,
#else
        ICollection<string> keys,
#endif
        SharpValueSpan argv = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.EvalSHA(sha1, keys, argv) | flags, cancellationToken);
    }

    /// <summary>
    /// This is a read-only variant of the <b>EVALSHA</b> command that cannot execute commands that modify data.
    /// <para>Available since: 7.0.0</para>
    /// <para><b>EVALSHA</b> 命令的只读版. 此命令不能执行修改数据的脚本</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="sha1">The SHA1 digest of a script previously cached with <b>SCRIPT LOAD</b> or EVAL.
    /// <para>使用 <b>SCRIPT LOAD</b> 命令加载缓存Lua脚本返回的SHA1值</para>
    /// </param>
    /// <param name="keys">The key names the script accesses, provided to it via the Lua global KEYS runtime variable.
    /// <para>提供给Lua脚本的Key, 在脚本里面使用KEYS访问</para>
    /// </param>
    /// <param name="argv">Additional arguments provided to the script via the Lua ARGV variable.
    /// <para>提供给Lua脚本的其它参数, 在脚本里面通过ARGV访问</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the script that was executed.
    /// <para>脚本返回的值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/EvalSHA_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/EvalSHA_Ro">命令文档</see></para>
    /// </remarks>
    public RedisResult EvalSHA_Ro(string sha1, ReadOnlySpan<string> keys = default, SharpValueSpan argv = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.EvalSHA_Ro(sha1, keys, argv) | flags, cancellationToken);
    }

    /// <summary>
    /// This is a read-only variant of the <b>EVALSHA</b> command that cannot execute commands that modify data.
    /// <para>Available since: 7.0.0</para>
    /// <para><b>EVALSHA</b> 命令的只读版. 此命令不能执行修改数据的脚本</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="sha1">The SHA1 digest of a script previously cached with <b>SCRIPT LOAD</b> or EVAL.
    /// <para>使用 <b>SCRIPT LOAD</b> 命令加载缓存Lua脚本返回的SHA1值</para>
    /// </param>
    /// <param name="keys">The key names the script accesses, provided to it via the Lua global KEYS runtime variable.
    /// <para>提供给Lua脚本的Key, 在脚本里面使用KEYS访问</para>
    /// </param>
    /// <param name="argv">Additional arguments provided to the script via the Lua ARGV variable.
    /// <para>提供给Lua脚本的其它参数, 在脚本里面通过ARGV访问</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The return value depends on the script that was executed.
    /// <para>脚本返回的值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/EvalSHA_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/EvalSHA_Ro">命令文档</see></para>
    /// </remarks>
    public RedisResult EvalSHA_Ro(
        string sha1,
#if MODERN_NET
        ICollection<string>? keys,
#else
        ICollection<string> keys,
#endif
        SharpValueSpan argv = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.EvalSHA_Ro(sha1, keys, argv) | flags, cancellationToken);
    }

    /// <summary>
    /// Set the debug mode for subsequent scripts executed with EVAL.
    /// <para>Available since: 3.2.0</para>
    /// <para>为后续使用 <b>EVAL</b> 执行的Lua脚本设置调试模式</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="mode">Set the debugging mode
    /// <para>调试模式</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/script-debug">Command documentation</see>
    /// <para><see href="https://redis.io/commands/script-debug">命令文档</see></para>
    /// </remarks>
    [Obsolete("Avoid debugging Lua scripts using your production Redis server. Use a development server instead.")]
    public bool ScriptDebug(DebugMode mode, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.ScriptDebug(mode) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns information about the existence of the scripts in the script cache.
    /// <para>Available since: 2.6.0</para>
    /// <para>根据指定的Lua脚本SHA1值, 返回是否加载缓存了该Lua脚本</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="sha1">One SHA1 digests to look up in the script cache.
    /// <para>要判断的Lua脚本SHA1值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Returns true if present, otherwise returns false.
    /// <para>存在返回true, 否则返回false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/script-exists">Command documentation</see>
    /// <para><see href="https://redis.io/commands/script-exists">命令文档</see></para>
    /// </remarks>
    public bool ScriptExists(string sha1, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.ScriptExists(sha1) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns information about the existence of the scripts in the script cache.
    /// <para>Available since: 2.6.0</para>
    /// <para>根据指定的Lua脚本SHA1值, 返回是否加载缓存了该Lua脚本</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="sha1">One or more SHA1 digests to look up in the script cache.
    /// <para>要判断的Lua脚本SHA1值, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Returns true if present, otherwise returns false.
    /// <para>存在返回true, 否则返回false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/script-exists">Command documentation</see>
    /// <para><see href="https://redis.io/commands/script-exists">命令文档</see></para>
    /// </remarks>
    public bool[] ScriptExists(ReadOnlySpan<string> sha1, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.ScriptExists(sha1) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns information about the existence of the scripts in the script cache.
    /// <para>Available since: 2.6.0</para>
    /// <para>根据指定的Lua脚本SHA1值, 返回是否加载缓存了该Lua脚本</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="sha1">One or more SHA1 digests to look up in the script cache.
    /// <para>要判断的Lua脚本SHA1值, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Returns true if present, otherwise returns false.
    /// <para>存在返回true, 否则返回false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/script-exists">Command documentation</see>
    /// <para><see href="https://redis.io/commands/script-exists">命令文档</see></para>
    /// </remarks>
    public bool[] ScriptExists(ICollection<string> sha1, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.ScriptExists(sha1) | flags, cancellationToken);
    }

    /// <summary>
    /// Flush the Lua scripts cache.
    /// <para>Available since: 2.6.0</para>
    /// <para>清除所有已加载缓存的Lua脚本</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="mode">Flush the script cache asynchronously (<b>ASYNC</b>) or synchronously (<b>SYNC</b>).
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>指定异步或同步清除</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/script-flush">Command documentation</see>
    /// <para><see href="https://redis.io/commands/script-flush">命令文档</see></para>
    /// </remarks>
    public bool ScriptFlush(AsyncSync mode = AsyncSync.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.ScriptFlush(mode) | flags, cancellationToken);
    }

    /// <summary>
    /// Kills the currently executing <b>EVAL</b> script, assuming no write operation was yet performed by the script.
    /// <para>Available since: 2.6.0</para>
    /// <para>终止当前通过 <b>EVAL</b> 命令正在执行的Lua脚本, 前提是该脚本尚未执行任何写入操作</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/script-kill">Command documentation</see>
    /// <para><see href="https://redis.io/commands/script-kill">命令文档</see></para>
    /// </remarks>
    public bool ScriptKill(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ScriptCommands.ScriptKill() | flags, cancellationToken);
    }
}
