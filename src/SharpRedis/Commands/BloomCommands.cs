using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using static SharpRedis.Modules.RedisBloom;

namespace SharpRedis.Commands;

internal static class BloomCommands
{
    private static readonly SharpValue _bfadd = new("BF.ADD", CommandItemCategory.Command);
    private static readonly SharpValue _bfcard = new("BF.CARD", CommandItemCategory.Command);
    private static readonly SharpValue _bfexists = new("BF.EXISTS", CommandItemCategory.Command);
    private static readonly SharpValue _bfinfo = new("BF.INFO", CommandItemCategory.Command);
    private static readonly SharpValue _bfinsert = new("BF.INSERT", CommandItemCategory.Command);
    private static readonly SharpValue _bfreserve = new("BF.RESERVE", CommandItemCategory.Command);
    private static readonly SharpValue _bfscandump = new("BF.SCANDUMP", CommandItemCategory.Command);
    private static readonly SharpValue _bfloadchunk = new("BF.LOADCHUNK", CommandItemCategory.Command);
    private static readonly SharpValue _bfmadd = new("BF.MADD", CommandItemCategory.Command);
    private static readonly SharpValue _bfmexists = new("BF.MEXISTS", CommandItemCategory.Command);

    internal static CommandPacket BF_Add(SharpValue key, SharpValue item)
    {
        return new CommandPacket(BloomCommands._bfadd, CommandMode.Bloom | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(item);
    }

    internal static CommandPacket BF_Card(SharpValue key)
    {
        return new CommandPacket(BloomCommands._bfcard, CommandMode.Bloom | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket BF_Exists(SharpValue key, SharpValue item)
    {
        return new CommandPacket(BloomCommands._bfexists, CommandMode.Bloom | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(item);
    }

    internal static CommandPacket BF_Info(SharpValue key, InfoType? type)
    {
        var command = new CommandPacket(BloomCommands._bfinfo, CommandMode.Bloom | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
        if (!type.HasValue) return command;
        return type.Value switch
        {
            InfoType.Capacity => command.WriteArguments("CAPACITY"),
            InfoType.Size => command.WriteArguments("SIZE"),
            InfoType.Filters => command.WriteArguments("FILTERS"),
            InfoType.Items => command.WriteArguments("ITEMS"),
            InfoType.Expansion => command.WriteArguments("EXPANSION"),
            _ => command,
        };
    }

    internal static CommandPacket BF_Insert(
        SharpValue key,
        SharpValue item,
        long? capacity,
        double? error,
        uint? expansion,
        bool nocreate,
        bool nonscaling)
    {
        return BloomCommands.InitBfInsert(
            key: key,
            capacity: capacity,
            error: error,
            expansion: expansion,
            nocreate: nocreate,
            nonscaling: nonscaling)
        .WriteArguments(item);
    }

    internal static CommandPacket BF_Insert(
        SharpValue key,
        SharpValueSpan items,
        long? capacity,
        double? error,
        uint? expansion,
        bool nocreate,
        bool nonscaling)
    {
        if (items.IsEmpty) throw new ArgumentException("Please ensure that there is at least one item.", nameof(items));
        return BloomCommands.InitBfInsert(
            key: key,
            capacity: capacity,
            error: error,
            expansion: expansion,
            nocreate: nocreate,
            nonscaling: nonscaling)
        .WriteArguments(items);
    }

    private static CommandPacket InitBfInsert(
        SharpValue key,
        long? capacity,
        double? error,
        uint? expansion,
        bool nocreate,
        bool nonscaling)
    {
        var command = new CommandPacket(BloomCommands._bfinsert, CommandMode.Bloom | CommandMode.WithWrite)
            .WriteKeys(key);
        if (capacity.HasValue)
        {
            _ = command
                .WriteArguments("CAPACITY")
                .WriteArguments(capacity.Value.ToBytes());
        }
        if (error.HasValue)
        {
            _ = command
                .WriteArguments("ERROR")
                .WriteArguments(error.Value.ToBytes());
        }
        if (expansion.HasValue)
        {
            _ = command
                .WriteArguments("EXPANSION")
                .WriteArguments(expansion.Value.ToBytes());
        }
        return command
            .WriteArguments(nocreate, "NOCREATE")
            .WriteArguments(nonscaling, "NONSCALING")
            .WriteArguments("ITEMS");
    }

    internal static CommandPacket BF_Reserve(
        SharpValue key,
        double errorRate,
        long capacity,
        uint? expansion,
        bool nonscaling)
    {
        var command = new CommandPacket(BloomCommands._bfreserve, CommandMode.Bloom | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(errorRate.ToBytes())
            .WriteArguments(capacity.ToBytes());
        if (expansion.HasValue)
        {
            _ = command
                .WriteArguments("EXPANSION")
                .WriteArguments(expansion.Value.ToBytes());
        }
        return command.WriteArguments(nonscaling, "NONSCALING");
    }

    internal static CommandPacket BF_ScanDump(SharpValue key, long iterator)
    {
        return new CommandPacket(BloomCommands._bfscandump, CommandMode.Bloom | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(iterator.ToBytes());
    }

    internal static CommandPacket BF_LoadChunk(SharpValue key, long iterator, byte[] data)
    {
        if (data is null or { Length: <= 0 }) throw new ArgumentException("The recovery data block cannot be empty.", nameof(data));
        return new CommandPacket(BloomCommands._bfloadchunk, CommandMode.Bloom | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(iterator.ToBytes())
            .WriteArguments(data);
    }

    internal static CommandPacket BF_MAdd(SharpValue key, SharpValue item)
    {
        return new CommandPacket(BloomCommands._bfmadd, CommandMode.Bloom | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(item);
    }

    internal static CommandPacket BF_MAdd(SharpValue key, SharpValueSpan items)
    {
        if (items.IsEmpty) throw new ArgumentException("Please ensure that there is at least one item.", nameof(items));
        return new CommandPacket(BloomCommands._bfmadd, CommandMode.Bloom | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(items);
    }

    internal static CommandPacket BF_MExists(SharpValue key, SharpValue item)
    {
        return new CommandPacket(BloomCommands._bfmexists, CommandMode.Bloom | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(item);
    }

    internal static CommandPacket BF_MExists(SharpValue key, SharpValueSpan items)
    {
        if (items.IsEmpty) throw new ArgumentException("Please ensure that there is at least one item.", nameof(items));
        return new CommandPacket(BloomCommands._bfmexists, CommandMode.Bloom | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(items);
    }
}
