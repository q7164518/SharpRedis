using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using static SharpRedis.RedisScript;

namespace SharpRedis.Commands;

internal static class ScriptCommands
{
    private readonly static SharpValue _eval = new("EVAL", CommandItemCategory.Command);
    private readonly static SharpValue _eval_ro = new("EVAL_RO", CommandItemCategory.Command);
    private readonly static SharpValue _evalsha = new("EVALSHA", CommandItemCategory.Command);
    private readonly static SharpValue _evalsha_ro = new("EVALSHA_RO", CommandItemCategory.Command);
    private readonly static SharpValue _script = new("SCRIPT", CommandItemCategory.Command);
    private readonly static SharpValue _load = new("LOAD", CommandItemCategory.Command);
    private readonly static SharpValue _debug = new("DEBUG", CommandItemCategory.Command);
    private readonly static SharpValue _exists = new("EXISTS", CommandItemCategory.Command);
    private readonly static SharpValue _flush = new("FLUSH", CommandItemCategory.Command);
    private readonly static SharpValue _kill = new("KILL", CommandItemCategory.Command);

    internal static CommandPacket Eval(string script, ReadOnlySpan<string> keys, SharpValueSpan args)
    {
        var command = new CommandPacket(ScriptCommands._eval, CommandMode.LuaScript)
            .WriteArguments(script)
            .WriteArguments(keys.Length.ToBytes());
        foreach (var key in keys)
        {
            _ = command.WriteKeys(key);
        }
        foreach (var arg in args)
        {
            _ = command.WriteArguments(arg);
        }
        return command;
    }

#if MODERN_NET
    internal static CommandPacket Eval(string script, ICollection<string>? keys, SharpValueSpan args)
#else
    internal static CommandPacket Eval(string script, ICollection<string> keys, SharpValueSpan args)
#endif
    {
        var command = new CommandPacket(ScriptCommands._eval, CommandMode.LuaScript)
            .WriteArguments(script);
        if (keys is { Count: > 0 })
        {
            _ = command.WriteArguments(keys.Count.ToBytes());
            foreach (var key in keys)
            {
                _ = command.WriteKeys(key);
            }
        }
        else
        {
            _ = command.WriteArguments("0");
        }
        foreach (var arg in args)
        {
            _ = command.WriteArguments(arg);
        }
        return command;
    }

    internal static CommandPacket Eval_Ro(string script, ReadOnlySpan<string> keys, SharpValueSpan args)
    {
        var command = new CommandPacket(ScriptCommands._eval_ro, CommandMode.LuaScript | CommandMode.WithRead)
            .WriteArguments(script)
            .WriteArguments(keys.Length.ToBytes());
        foreach (var key in keys)
        {
            _ = command.WriteKeys(key);
        }
        foreach (var arg in args)
        {
            _ = command.WriteArguments(arg);
        }
        return command;
    }

#if MODERN_NET
    internal static CommandPacket Eval_Ro(string script, ICollection<string>? keys, SharpValueSpan args)
#else
    internal static CommandPacket Eval_Ro(string script, ICollection<string> keys, SharpValueSpan args)
#endif
    {
        var command = new CommandPacket(ScriptCommands._eval_ro, CommandMode.LuaScript | CommandMode.WithRead)
            .WriteArguments(script);
        if (keys is { Count: > 0 })
        {
            _ = command.WriteArguments(keys.Count.ToBytes());
            foreach (var key in keys)
            {
                _ = command.WriteKeys(key);
            }
        }
        else
        {
            _ = command.WriteArguments("0");
        }
        foreach (var arg in args)
        {
            _ = command.WriteArguments(arg);
        }
        return command;
    }

    internal static CommandPacket ScriptLoad(string script)
    {
        return new CommandPacket(ScriptCommands._script, CommandMode.LuaScript)
            .WriteCommand(ScriptCommands._load)
            .WriteArguments(script);
    }

    internal static CommandPacket EvalSHA(string sha1, ReadOnlySpan<string> keys, SharpValueSpan args)
    {
        if (string.IsNullOrEmpty(sha1)) throw new NotSupportedException("The SHA1 value of the Lua script is empty.");
        var command = new CommandPacket(ScriptCommands._evalsha, CommandMode.LuaScript)
            .WriteArguments(sha1)
            .WriteArguments(keys.Length.ToBytes());
        foreach (var key in keys)
        {
            _ = command.WriteKeys(key);
        }
        foreach (var arg in args)
        {
            _ = command.WriteArguments(arg);
        }
        return command;
    }

#if MODERN_NET
    internal static CommandPacket EvalSHA(string sha1, ICollection<string>? keys, SharpValueSpan args)
#else
    internal static CommandPacket EvalSHA(string sha1, ICollection<string> keys, SharpValueSpan args)
#endif
    {
        if (string.IsNullOrEmpty(sha1)) throw new NotSupportedException("The SHA1 value of the Lua script is empty.");
        var command = new CommandPacket(ScriptCommands._evalsha, CommandMode.LuaScript)
            .WriteArguments(sha1);
        if (keys is { Count: > 0 })
        {
            _ = command.WriteArguments(keys.Count.ToBytes());
            foreach (var key in keys)
            {
                _ = command.WriteKeys(key);
            }
        }
        else
        {
            _ = command.WriteArguments("0");
        }
        foreach (var arg in args)
        {
            _ = command.WriteArguments(arg);
        }
        return command;
    }

    internal static CommandPacket EvalSHA_Ro(string sha1, ReadOnlySpan<string> keys, SharpValueSpan args)
    {
        if (string.IsNullOrEmpty(sha1)) throw new NotSupportedException("The SHA1 value of the Lua script is empty.");
        var command = new CommandPacket(ScriptCommands._evalsha_ro, CommandMode.LuaScript | CommandMode.WithRead)
            .WriteArguments(sha1)
            .WriteArguments(keys.Length.ToBytes());
        foreach (var key in keys)
        {
            _ = command.WriteKeys(key);
        }
        foreach (var arg in args)
        {
            _ = command.WriteArguments(arg);
        }
        return command;
    }

#if MODERN_NET
    internal static CommandPacket EvalSHA_Ro(string sha1, ICollection<string>? keys, SharpValueSpan args)
#else
    internal static CommandPacket EvalSHA_Ro(string sha1, ICollection<string> keys, SharpValueSpan args)
#endif
    {
        if (string.IsNullOrEmpty(sha1)) throw new NotSupportedException("The SHA1 value of the Lua script is empty.");
        var command = new CommandPacket(ScriptCommands._evalsha_ro, CommandMode.LuaScript | CommandMode.WithRead)
            .WriteArguments(sha1);
        if (keys is { Count: > 0 })
        {
            _ = command.WriteArguments(keys.Count.ToBytes());
            foreach (var key in keys)
            {
                _ = command.WriteKeys(key);
            }
        }
        else
        {
            _ = command.WriteArguments("0");
        }
        foreach (var arg in args)
        {
            _ = command.WriteArguments(arg);
        }
        return command;
    }

    internal static CommandPacket ScriptDebug(DebugMode mode)
    {
        var command = new CommandPacket(ScriptCommands._script, CommandMode.LuaScript | CommandMode.DoNotReuseConnection)
            .WriteCommand(ScriptCommands._debug);

        return mode switch
        {
            DebugMode.Yes => command.WriteArguments("YES"),
            DebugMode.Sync => command.WriteArguments("SYNC"),
            DebugMode.No => command.WriteArguments("NO"),
            _ => throw new NotSupportedException("Unsupported debugging mode"),
        };
    }

    internal static CommandPacket ScriptExists(string sha1)
    {
        if (string.IsNullOrEmpty(sha1)) throw new NotSupportedException("The SHA1 value of the Lua script is empty.");
        return new CommandPacket(ScriptCommands._script, CommandMode.LuaScript)
            .WriteCommand(ScriptCommands._exists)
            .WriteArguments(sha1);
    }

    internal static CommandPacket ScriptExists(ReadOnlySpan<string> sha1)
    {
        if (sha1.IsEmpty) throw new NotSupportedException("Please specify at least one sha1 value.");
        var command = new CommandPacket(ScriptCommands._script, CommandMode.LuaScript)
            .WriteCommand(ScriptCommands._exists);
        foreach (var item in sha1)
        {
            _ = command.WriteArguments(item);
        }
        return command;
    }

    internal static CommandPacket ScriptExists(ICollection<string> sha1)
    {
        if (sha1 is null or { Count: <= 0 }) throw new NotSupportedException("Please specify at least one sha1 value.");
        var command = new CommandPacket(ScriptCommands._script, CommandMode.LuaScript)
            .WriteCommand(ScriptCommands._exists);
        foreach (var item in sha1)
        {
            _ = command.WriteArguments(item);
        }
        return command;
    }

    internal static CommandPacket ScriptFlush(AsyncSync mode)
    {
        var command = new CommandPacket(ScriptCommands._script, CommandMode.LuaScript)
            .WriteCommand(ScriptCommands._flush);

        return mode switch
        {
            AsyncSync.Async => command.WriteArguments("ASYNC"),
            AsyncSync.Sync => command.WriteArguments("SYNC"),
            _ => command,
        };
    }

    internal static CommandPacket ScriptKill()
    {
        return new CommandPacket(ScriptCommands._script, CommandMode.LuaScript)
            .WriteCommand(ScriptCommands._kill);
    }
}
