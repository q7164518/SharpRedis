#if !LOW_NET
using SharpRedis.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis
{
    public sealed partial class RedisScript
    {
        #region Script
        /// <summary>
        /// Invoke the execution of a server-side Lua script
        /// <para>Available since: 2.6.0</para>
        /// <para>执行Lua脚本</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="script">Lua script code
        /// <para>Lua脚本代码</para>
        /// </param>
        /// <param name="keys">Keys</param>
        /// <param name="args">args</param>
        /// <param name="dataType">The script returns a value parsing type, which defaults to string
        /// <para>脚本返回值解析类型, 默认为string</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<object?> EvalAsync(string script, string[]? keys, string[]? args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#else
        public Task<object> EvalAsync(string script, string[] keys, string[] args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallAsync(ScriptCommands.Eval(script, keys, args).SetResultDataType(dataType), ResultType.Object, cancellationToken);
        }

        /// <summary>
        /// Evaluate a script from the server's cache by its SHA1 digest.
        /// <para>Available since: 2.6.0</para>
        /// <para>根据缓存脚本的SHA1值执行脚本</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="sha1">The SHA1 digest of the script added into the script cache.
        /// <para>缓存的脚本SHA1</para>
        /// </param>
        /// <param name="keys">Keys</param>
        /// <param name="args">args</param>
        /// <param name="dataType">The script returns a value parsing type, which defaults to string
        /// <para>脚本返回值解析类型, 默认为string</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<object?> EvalSHAAsync(string sha1, string[]? keys, string[]? args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#else
        public Task<object> EvalSHAAsync(string sha1, string[] keys, string[] args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallAsync(ScriptCommands.EvalSHA(sha1, keys, args).SetResultDataType(dataType), ResultType.Object, cancellationToken);
        }

        /// <summary>
        /// Flush the Lua scripts cache.
        /// <para>Available since: 2.6.0</para>
        /// <para>清除所有Lua缓存</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="mode">Flushing mode modifier
        /// <para>刷新模式</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> ScriptFlushAsync(FlushingMode mode = FlushingMode.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(ScriptCommands.ScriptFlush(mode), "OK", cancellationToken);
        }

        /// <summary>
        /// Load a script into the scripts cache, without executing it. After the specified command is loaded into the script cache it will be callable using EVALSHA with the correct SHA1 digest of the script, exactly like after the first successful invocation of EVAL.
        /// <para>Available since: 2.6.0</para>
        /// <para>加载一段LUA脚本到Redis脚本缓存中, 返回脚本的SHA1值, 后面可以用SHA1执行该脚本</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="script">Lua script code
        /// <para>Lua脚本代码</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The SHA1 digest of the script added into the script cache.
        /// <para>
        /// 缓存之后的SHA1值
        /// </para>
        /// </returns>
        public Task<string> ScriptLoadAsync(string script, CancellationToken cancellationToken = default)
        {
            return base._call.CallStringAsync(ScriptCommands.ScriptLoad(script), cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Kills the currently executing EVAL script, assuming no write operation was yet performed by the script
        /// <para>Available since: 2.6.0</para>
        /// <para>终止当前正在运行的脚本, 假设该脚本当前还没执行任何写入操作</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para></param>
        /// <returns></returns>
        /// <remarks>If the script has already performed write operations, it can not be killed in this way because it would violate Lua's script atomicity contract. In such a case, only SHUTDOWN NOSAVE can kill the script, killing the Redis process in a hard way and preventing it from persisting with half-written information
        /// <para>如果脚本已经执行了写操作，则不能通过这种方式杀死它，因为这会违反Lua的脚本原子性契约。在这种情况下，只有SHUTDOWN NOSAVE可以杀掉脚本，硬杀掉Redis进程，防止其持久化写了一半的信息</para>
        /// </remarks>
        public Task<bool> ScriptKillAsync(CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(ScriptCommands.ScriptKill(), "OK", cancellationToken);
        }

        /// <summary>
        /// Returns information about the existence of the scripts in the script cache.
        /// <para>Available since: 2.6.0</para>
        /// <para>根据SHA1判断LUA是否已缓存了该脚本</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="sha1">SHA1
        /// <para>SHA1值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> ScriptExistsAsync(string sha1, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(ScriptCommands.ScriptExists(sha1), "1", cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns information about the existence of the scripts in the script cache.
        /// <para>Available since: 2.6.0</para>
        /// <para>根据SHA1判断LUA是否已缓存了该脚本</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="sha1Array">SHA1 array
        /// <para>SHA1值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool[]> ScriptExistsAsync(string[] sha1Array, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionArrayAsync(ScriptCommands.ScriptExists(sha1Array), "1", cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Set the debug mode for subsequent scripts executed with EVAL
        /// <para>Available since: 3.2.0</para>
        /// <para>为使用EVAL执行的后续脚本设置调试模式</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="mode">mode</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> ScriptDebugAsync(ScriptDebugMode mode, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(ScriptCommands.ScriptDebug(mode), "OK", cancellationToken);
        }

        /// <summary>
        /// This is a read-only variant of the EVAL command that cannot execute commands that modify data
        /// <para>Available since: 7.0.0</para>
        /// <para>执行只读的Lua脚本. 不能执行写入, 否则异常</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="script">Lua script code
        /// <para>Lua脚本代码</para>
        /// </param>
        /// <param name="keys">Keys</param>
        /// <param name="args">args</param>
        /// <param name="dataType">The script returns a value parsing type, which defaults to string
        /// <para>脚本返回值解析类型, 默认为string</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<object?> Eval_RoAsync(string script, string[]? keys, string[]? args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#else
        public Task<object> Eval_RoAsync(string script, string[] keys, string[] args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallAsync(ScriptCommands.Eval_Ro(script, keys, args).SetResultDataType(dataType), ResultType.Object, cancellationToken);
        }

        /// <summary>
        /// This is a read-only variant of the EVALSHA command that cannot execute commands that modify data
        /// <para>Available since: 7.0.0</para>
        /// <para>根据缓存的只读脚本SHA1值执行脚本</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="sha1">The SHA1 digest of the script added into the script cache.
        /// <para>缓存的脚本SHA1</para>
        /// </param>
        /// <param name="keys">Keys</param>
        /// <param name="args">args</param>
        /// <param name="dataType">The script returns a value parsing type, which defaults to string
        /// <para>脚本返回值解析类型, 默认为string</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<object?> EvalSHA_RoAsync(string sha1, string[]? keys, string[]? args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#else
        public Task<object> EvalSHA_RoAsync(string sha1, string[] keys, string[] args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallAsync(ScriptCommands.EvalSHA_Ro(sha1, keys, args).SetResultDataType(dataType), ResultType.Object, cancellationToken);
        }
        #endregion

        #region Function
        /// <summary>
        /// Load a library to Redis
        /// <para>Available since: 7.0.0</para>
        /// <para>加载一个函数库到Redis</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="functionCode">function lua code
        /// <para>函数库Lua代码</para>
        /// </param>
        /// <param name="replace">Whether to overwrite the library if it already exists. Default no override
        /// <para>如果库已经存在, 是否覆盖. 默认不覆盖</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The library name that was loaded
        /// <para>加载成功的函数库名称</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string?> FunctionLoadAsync(string functionCode, bool replace = false, CancellationToken cancellationToken = default)
#else
        public Task<string> FunctionLoadAsync(string functionCode, bool replace = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStringAsync(ScriptCommands.FunctionLoad(functionCode, replace), cancellationToken);
        }

        /// <summary>
        /// Delete a library and all its functions. If the library doesn't exist, the server returns an error
        /// <para>Available since: 7.0.0</para>
        /// <para>删除一个函数库, 如果函数库不存在将引发异常</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="libraryName">The name of the library to delete
        /// <para>要删除的函数库名称</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> FunctionDeleteAsync(string libraryName, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(ScriptCommands.FunctionDelete(libraryName), "OK", cancellationToken);
        }

        /// <summary>
        /// Invoke a function
        /// <para>Available since: 7.0.0</para>
        /// <para>执行Lua函数</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="function">function name
        /// <para>要执行的函数名称</para>
        /// </param>
        /// <param name="keys">Keys</param>
        /// <param name="args">args</param>
        /// <param name="dataType">The function returns a value parsing type, which defaults to string
        /// <para>函数返回值解析类型, 默认为string</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<object?> FCallAsync(string function, string[]? keys, string[]? args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#else
        public Task<object> FCallAsync(string function, string[] keys, string[] args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallAsync(ScriptCommands.FCall(function, keys, args).SetResultDataType(dataType), ResultType.Object, cancellationToken);
        }

        /// <summary>
        /// This is a read-only variant of the FCALL command that cannot execute commands that modify data
        /// <para>Available since: 7.0.0</para>
        /// <para>执行只读的Lua函数. 无法执行包含写入操作的函数</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="function">function name
        /// <para>要执行的函数名称</para>
        /// </param>
        /// <param name="keys">Keys</param>
        /// <param name="args">args</param>
        /// <param name="dataType">The function returns a value parsing type, which defaults to string
        /// <para>函数返回值解析类型, 默认为string</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<object?> FCall_RoAsync(string function, string[]? keys, string[]? args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#else
        public Task<object> FCall_RoAsync(string function, string[] keys, string[] args, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallAsync(ScriptCommands.FCall_Ro(function, keys, args).SetResultDataType(dataType), ResultType.Object, cancellationToken);
        }

        /// <summary>
        /// Return the serialized payload of loaded libraries. You can restore the serialized payload later with the FUNCTION RESTORE command
        /// <para>Available since: 7.0.0</para>
        /// <para>序列化已加载的函数库。可以使用FUNCTION RESTORE命令恢复函数库</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>serialized payload
        /// <para>序列化后的函数库</para>
        /// </returns>
        public Task<byte[]> FunctionDumpAsync(CancellationToken cancellationToken = default)
        {
            return base._call.CallBytesAsync(ScriptCommands.FunctionDump(), cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Deletes all the libraries
        /// <para>Available since: 7.0.0</para>
        /// <para>删除所有函数库</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="mode">Flushing Mode
        /// <para>删除模式, 可选同步或异步. 默认不设置</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> FunctionFlushAsync(FlushingMode mode = FlushingMode.None, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(ScriptCommands.FunctionFlush(mode), "OK", cancellationToken);
        }

        /// <summary>
        /// Restore libraries from the serialized payload
        /// <para>Available since: 7.0.0</para>
        /// <para>把序列化的函数库恢复函数到Redis</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="serializedPayload">serialized payload
        /// <para>序列化的函数</para>
        /// </param>
        /// <param name="mode">Restore mode, defalut append
        /// <para>恢复模式, 默认为Append模式</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> FunctionRestoreAsync(byte[] serializedPayload, FunctionRestoreMode mode = FunctionRestoreMode.Append, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(ScriptCommands.FunctionRestore(serializedPayload, mode), "OK", cancellationToken);
        }

        /// <summary>
        /// Kill a function that is currently executing
        /// <para>Available since: 7.0.0</para>
        /// <para>杀死(结束)当前正在运行的函数</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> FunctionKillAsync(CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(ScriptCommands.FunctionKill(), "OK", cancellationToken);
        }

        /// <summary>
        /// Return information about the functions and libraries
        /// <para>Available since: 7.0.0</para>
        /// <para>返回有关函数和库的信息</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="libraryNamePattern">You can use the optional LIBRARYNAME argument to specify a pattern for matching library names
        /// <para>库名称筛选模式, 默认没有</para>
        /// </param>
        /// <param name="withCode">The optional WITHCODE modifier will cause the server to include the libraries source implementation in the reply
        /// <para>是否包含源码返回, 默认不包含</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<FunctionInfoValue[]?> FunctionListAsync(string? libraryNamePattern = null, bool withCode = false, CancellationToken cancellationToken = default)
#else
        public Task<FunctionInfoValue[]> FunctionListAsync(string libraryNamePattern = null, bool withCode = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArrayAsync<FunctionInfoValue>(ScriptCommands.FunctionList(libraryNamePattern, withCode), ResultType.Array | ResultType.FunctionInfoValue, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return information about the function that's currently running and information about the available execution engines
        /// <para>Available since: 7.0.0</para>
        /// <para>获得当前正在运行的函数信息和可用的引擎信息</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<FunctionStatsValue> FunctionStatsAsync(CancellationToken cancellationToken = default)
        {
            return base._call.CallClassAsync<FunctionStatsValue>(ScriptCommands.FunctionStats().SetResultDataType(ResultDataType.Default), ResultType.FunctionStatsValue, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }
        #endregion
    }
}
#endif
