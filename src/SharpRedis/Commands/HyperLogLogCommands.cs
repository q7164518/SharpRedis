using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using System;

namespace SharpRedis.Commands;

internal static class HyperLogLogCommands
{
    private static readonly SharpValue _pfadd = new("PFADD", CommandItemCategory.Command);
    private static readonly SharpValue _pfcount = new("PFCOUNT", CommandItemCategory.Command);
    private static readonly SharpValue _pfmerge = new("PFMERGE", CommandItemCategory.Command);

    internal static CommandPacket PFAdd(SharpValue key, SharpValue element)
    {
        return new CommandPacket(HyperLogLogCommands._pfadd, CommandMode.HyperLogLog | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(element);
    }

    internal static CommandPacket PFAdd(SharpValue key, SharpValueSpan elements)
    {
        if (elements.IsEmpty) throw new NotSupportedException("Please specify at least one element.");
        return new CommandPacket(HyperLogLogCommands._pfadd, CommandMode.HyperLogLog | CommandMode.WithWrite)
        .WriteKeys(key)
        .WriteArguments(elements);
    }

    internal static CommandPacket PFCount(SharpValue key)
    {
        return new CommandPacket(HyperLogLogCommands._pfcount, CommandMode.HyperLogLog | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket PFCount(SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        return new CommandPacket(HyperLogLogCommands._pfcount, CommandMode.HyperLogLog | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(keys);
    }

    internal static CommandPacket PFMerge(SharpValue destKey, SharpValue sourceKey)
    {
        return new CommandPacket(HyperLogLogCommands._pfmerge, CommandMode.HyperLogLog | CommandMode.WithWrite)
            .WriteKeys(destKey)
            .WriteKeys(sourceKey);
    }

    internal static CommandPacket PFMerge(SharpValue destKey, SharpValueSpan sourceKeys)
    {
        if (sourceKeys.IsEmpty) throw new NotSupportedException("Please specify at least one source key.");
        return new CommandPacket(HyperLogLogCommands._pfmerge, CommandMode.HyperLogLog | CommandMode.WithWrite)
            .WriteKeys(destKey)
            .WriteKeys(sourceKeys);
    }
}
