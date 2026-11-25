using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using static SharpRedis.RedisFunction;

namespace SharpRedis.Commands;

internal static class FunctionCommands
{
    private static readonly SharpValue _fcall = new("FCALL", CommandItemCategory.Command);
    private static readonly SharpValue _fcall_ro = new("FCALL_RO", CommandItemCategory.Command);
    private static readonly SharpValue _function = new("FUNCTION", CommandItemCategory.Command);
    private static readonly SharpValue _load = new("LOAD", CommandItemCategory.Command);
    private static readonly SharpValue _delete = new("DELETE", CommandItemCategory.Command);
    private static readonly SharpValue _dump = new("DUMP", CommandItemCategory.Command);
    private static readonly SharpValue _restore = new("RESTORE", CommandItemCategory.Command);
    private static readonly SharpValue _flush = new("FLUSH", CommandItemCategory.Command);
    private static readonly SharpValue _kill = new("KILL", CommandItemCategory.Command);
    private static readonly SharpValue _list = new("LIST", CommandItemCategory.Command);
    private static readonly SharpValue _stats = new("STATS", CommandItemCategory.Command);

    internal static CommandPacket FunctionLoad(string functionCode, bool replace)
    {
        if (string.IsNullOrEmpty(functionCode)) throw new NotSupportedException("The function code cannot be empty.");
        return new CommandPacket(FunctionCommands._function, CommandMode.Function)
            .WriteCommand(FunctionCommands._load)
            .WriteArguments(replace, "REPLACE")
            .WriteArguments(functionCode);
    }

    internal static CommandPacket FCall(string function, ReadOnlySpan<string> keys, SharpValueSpan args)
    {
        var command = new CommandPacket(FunctionCommands._fcall, CommandMode.Function)
            .WriteArguments(function)
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
    internal static CommandPacket FCall(string function, ICollection<string>? keys, SharpValueSpan args)
#else
    internal static CommandPacket FCall(string function, ICollection<string> keys, SharpValueSpan args)
#endif
    {
        var command = new CommandPacket(FunctionCommands._fcall, CommandMode.Function)
            .WriteArguments(function);
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

    internal static CommandPacket FCall_Ro(string function, ReadOnlySpan<string> keys, SharpValueSpan args)
    {
        var command = new CommandPacket(FunctionCommands._fcall_ro, CommandMode.Function | CommandMode.WithRead)
            .WriteArguments(function)
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
    internal static CommandPacket FCall_Ro(string function, ICollection<string>? keys, SharpValueSpan args)
#else
    internal static CommandPacket FCall_Ro(string function, ICollection<string> keys, SharpValueSpan args)
#endif
    {
        var command = new CommandPacket(FunctionCommands._fcall_ro, CommandMode.Function | CommandMode.WithRead)
            .WriteArguments(function);
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

    internal static CommandPacket FunctionDelete(string libraryName)
    {
        if (string.IsNullOrEmpty(libraryName)) throw new NotSupportedException("The library-name cannot be empty.");
        return new CommandPacket(FunctionCommands._function, CommandMode.Function)
            .WriteCommand(FunctionCommands._delete)
            .WriteArguments(libraryName);
    }

    internal static CommandPacket FunctionDump()
    {
        return new CommandPacket(FunctionCommands._function, CommandMode.Function)
            .WriteCommand(FunctionCommands._dump);
    }

    internal static CommandPacket FunctionRestore(byte[] serializedValue, RestoreHandling handling)
    {
        var command = new CommandPacket(FunctionCommands._function, CommandMode.Function)
            .WriteCommand(FunctionCommands._restore)
            .WriteArguments(serializedValue);
        return handling switch
        {
            RestoreHandling.Append => command.WriteArguments("APPEND"),
            RestoreHandling.Flush => command.WriteArguments("FLUSH"),
            RestoreHandling.Replace => command.WriteArguments("REPLACE"),
            _ => command,
        };
    }

    internal static CommandPacket FunctionFlush(AsyncSync mode)
    {
        var command = new CommandPacket(FunctionCommands._function, CommandMode.Function)
            .WriteCommand(FunctionCommands._flush);
        return mode switch
        {
            AsyncSync.Async => command.WriteArguments("ASYNC"),
            AsyncSync.Sync => command.WriteArguments("SYNC"),
            _ => command,
        };
    }

    internal static CommandPacket FunctionKill()
    {
        return new CommandPacket(FunctionCommands._function, CommandMode.Function)
            .WriteCommand(FunctionCommands._kill);
    }

#if MODERN_NET
    internal static CommandPacket FunctionList(string? libraryNamePattern, bool withCode)
#else
    internal static CommandPacket FunctionList(string libraryNamePattern, bool withCode)
#endif
    {
        var command = new CommandPacket(FunctionCommands._function, CommandMode.Function)
            .WriteCommand(FunctionCommands._list);
        if (!string.IsNullOrEmpty(libraryNamePattern))
        {
            _ = command
                .WriteArguments("LIBRARYNAME")
                .WriteArguments(libraryNamePattern);
        }

        return command
            .WriteArguments(withCode, "WITHCODE");
    }

    internal static CommandPacket FunctionStats()
    {
        return new CommandPacket(FunctionCommands._function, CommandMode.Function)
            .WriteCommand(FunctionCommands._stats);
    }
}
