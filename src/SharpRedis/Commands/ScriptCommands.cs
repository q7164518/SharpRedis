using SharpRedis.Models;

namespace SharpRedis.Commands
{
    internal static class ScriptCommands
    {
        #region Script
        internal static CommandPacket ScriptFlush(FlushingMode mode)
        {
            return new CommandPacket("SCRIPT", CommandMode.Script)
                .WriteArg("FLUSH")
                .WriteArg(mode == FlushingMode.Async, "ASYNC")
                .WriteArg(mode == FlushingMode.Sync, "SYNC");
        }

        internal static CommandPacket ScriptLoad(string script)
        {
            if (string.IsNullOrEmpty(script)) throw new RedisException("The loaded script cannot be empty or null");
            return new CommandPacket("SCRIPT", CommandMode.Script)
                .WriteArg("LOAD")
                .WriteArg(script);
        }

        internal static CommandPacket ScriptKill()
        {
            return new CommandPacket("SCRIPT", CommandMode.Script)
                .WriteArg("KILL");
        }

        internal static CommandPacket ScriptExists(params string[] shaArray)
        {
            if (shaArray is null || shaArray.Length == 0) throw new RedisException("The SHA1 value cannot be empty");
            return new CommandPacket("SCRIPT", CommandMode.Script)
                .WriteArg("EXISTS")
                .WriteArgs(shaArray);
        }

        internal static CommandPacket ScriptDebug(ScriptDebugMode mode)
        {
            return new CommandPacket("SCRIPT", CommandMode.Script)
                .WriteArg("DEBUG")
                .WriteArg(mode == ScriptDebugMode.Sync, "SYNC")
                .WriteArg(mode == ScriptDebugMode.Yes, "YES")
                .WriteArg(mode == ScriptDebugMode.No, "NO");
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket EvalSHA(string sha1, string[]? keys, string[]? args)
#else
        internal static CommandPacket EvalSHA(string sha1, string[] keys, string[] args)
#endif
        {
            if (string.IsNullOrEmpty(sha1)) throw new RedisException("The SHA1 cannot be empty or null");
            var keysCount = keys?.Length ?? 0;
            return new CommandPacket("EVALSHA", CommandMode.Script)
                .WriteArg(sha1)
                .WriteArg(keysCount)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteKeys(keysCount > 0, keys!)
                .WriteArgs(args?.Length > 0, args!);
#else
                .WriteKeys(keysCount > 0, keys)
                .WriteArgs(args?.Length > 0, args);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket Eval(string script, string[]? keys, string[]? args)
#else
        internal static CommandPacket Eval(string script, string[] keys, string[] args)
#endif
        {
            if (string.IsNullOrEmpty(script)) throw new RedisException("The script to be executed cannot be empty or null");
            var keysCount = keys?.Length ?? 0;
            return new CommandPacket("EVAL", CommandMode.Script)
                .WriteArg(script)
                .WriteArg(keysCount)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteKeys(keysCount > 0, keys!)
                .WriteArgs(args?.Length > 0, args!);
#else
                .WriteKeys(keysCount > 0, keys)
                .WriteArgs(args?.Length > 0, args);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket Eval_Ro(string script, string[]? keys, string[]? args)
#else
        internal static CommandPacket Eval_Ro(string script, string[] keys, string[] args)
#endif
        {
            if (string.IsNullOrEmpty(script)) throw new RedisException("The script to be executed cannot be empty or null");
            var keysCount = keys?.Length ?? 0;
            return new CommandPacket("EVAL_RO", CommandMode.Script | CommandMode.Read)
                .WriteArg(script)
                .WriteArg(keysCount)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteKeys(keysCount > 0, keys!)
                .WriteArgs(args?.Length > 0, args!);
#else
                .WriteKeys(keysCount > 0, keys)
                .WriteArgs(args?.Length > 0, args);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket EvalSHA_Ro(string sha1, string[]? keys, string[]? args)
#else
        internal static CommandPacket EvalSHA_Ro(string sha1, string[] keys, string[] args)
#endif
        {
            if (string.IsNullOrEmpty(sha1)) throw new RedisException("The SHA1 cannot be empty or null");
            var keysCount = keys?.Length ?? 0;
            return new CommandPacket("EVALSHA_RO", CommandMode.Script | CommandMode.Read)
                .WriteArg(sha1)
                .WriteArg(keysCount)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteKeys(keysCount > 0, keys!)
                .WriteArgs(args?.Length > 0, args!);
#else
                .WriteKeys(keysCount > 0, keys)
                .WriteArgs(args?.Length > 0, args);
#endif
        }
        #endregion

        #region Function
        internal static CommandPacket FunctionLoad(string functionCode, bool replace)
        {
            return new CommandPacket("FUNCTION", CommandMode.Script)
                .WriteArg("LOAD")
                .WriteArg(replace, "REPLACE")
                .WriteValue(functionCode);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket FCall(string function, string[]? keys, string[]? args)
#else
        internal static CommandPacket FCall(string function, string[] keys, string[] args)
#endif
        {
            if (string.IsNullOrEmpty(function)) throw new RedisException("The function to be executed cannot be empty or null");
            var keysCount = keys?.Length ?? 0;
            return new CommandPacket("FCALL", CommandMode.Script)
                .WriteArg(function)
                .WriteArg(keysCount)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteKeys(keysCount > 0, keys!)
                .WriteArgs(args?.Length > 0, args!);
#else
                .WriteKeys(keysCount > 0, keys)
                .WriteArgs(args?.Length > 0, args);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket FCall_Ro(string function, string[]? keys, string[]? args)
#else
        internal static CommandPacket FCall_Ro(string function, string[] keys, string[] args)
#endif
        {
            if (string.IsNullOrEmpty(function)) throw new RedisException("The function to be executed cannot be empty or null");
            var keysCount = keys?.Length ?? 0;
            return new CommandPacket("FCALL_RO", CommandMode.Script | CommandMode.Read)
                .WriteArg(function)
                .WriteArg(keysCount)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteKeys(keysCount > 0, keys!)
                .WriteArgs(args?.Length > 0, args!);
#else
                .WriteKeys(keysCount > 0, keys)
                .WriteArgs(args?.Length > 0, args);
#endif
        }

        internal static CommandPacket FunctionDelete(string libraryName)
        {
            return new CommandPacket("FUNCTION", CommandMode.Script)
                .WriteArg("DELETE")
                .WriteValue(libraryName);
        }

        internal static CommandPacket FunctionDump()
        {
            return new CommandPacket("FUNCTION", CommandMode.Script)
                .WriteArg("DUMP");
        }

        internal static CommandPacket FunctionFlush(FlushingMode mode)
        {
            return new CommandPacket("FUNCTION", CommandMode.Script)
                .WriteArg("FLUSH")
                .WriteArg(mode == FlushingMode.Sync, "SYNC")
                .WriteArg(mode == FlushingMode.Async, "ASYNC");
        }

        internal static CommandPacket FunctionRestore(byte[] serializedPayload, FunctionRestoreMode mode)
        {
            return new CommandPacket("FUNCTION", CommandMode.Script)
                .WriteArg("RESTORE")
                .WriteValue(serializedPayload)
                .WriteArg(mode is FunctionRestoreMode.Append, "APPEND")
                .WriteArg(mode is FunctionRestoreMode.Flush, "FLUSH")
                .WriteArg(mode is FunctionRestoreMode.Replace, "REPLACE");
        }

        internal static CommandPacket FunctionKill()
        {
            return new CommandPacket("FUNCTION", CommandMode.Script)
                .WriteArg("KILL");
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket FunctionList(string? libraryNamePattern, bool withCode)
#else
        internal static CommandPacket FunctionList(string libraryNamePattern, bool withCode)
#endif
        {
            return new CommandPacket("FUNCTION", CommandMode.Script)
                .WriteArg("LIST")
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArg(libraryNamePattern != null, "LIBRARYNAME", libraryNamePattern!)
#else
                .WriteArg(libraryNamePattern != null, "LIBRARYNAME", libraryNamePattern)
#endif
                .WriteArg(withCode, "WITHCODE");
        }

        internal static CommandPacket FunctionStats()
        {
            return new CommandPacket("FUNCTION", CommandMode.Script)
                .WriteArg("STATS");
        }
        #endregion
    }
}
