using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;

namespace SharpRedis.Commands;

internal static class CuckooCommands
{
    private static readonly SharpValue _cfadd = new("CF.ADD", CommandItemCategory.Command);
    private static readonly SharpValue _cfaddnx = new("CF.ADDNX", CommandItemCategory.Command);
    private static readonly SharpValue _cfcount = new("CF.COUNT", CommandItemCategory.Command);
    private static readonly SharpValue _cfdel = new("CF.DEL", CommandItemCategory.Command);
    private static readonly SharpValue _cfexists = new("CF.EXISTS", CommandItemCategory.Command);
    private static readonly SharpValue _cfinsert = new("CF.INSERT", CommandItemCategory.Command);
    private static readonly SharpValue _cfinsertnx = new("CF.INSERTNX", CommandItemCategory.Command);
    private static readonly SharpValue _cfloadchunk = new("CF.LOADCHUNK", CommandItemCategory.Command);
    private static readonly SharpValue _cfscandump = new("CF.SCANDUMP", CommandItemCategory.Command);
    private static readonly SharpValue _cfmexists = new("CF.MEXISTS", CommandItemCategory.Command);
    private static readonly SharpValue _cfreserve = new("CF.RESERVE", CommandItemCategory.Command);
    private static readonly SharpValue _cfinfo = new("CF.INFO", CommandItemCategory.Command);

    internal static CommandPacket CF_Add(SharpValue key, SharpValue item)
    {
        return new CommandPacket(CuckooCommands._cfadd, CommandMode.Cuckoo | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(item);
    }

    internal static CommandPacket CF_AddNx(SharpValue key, SharpValue item)
    {
        return new CommandPacket(CuckooCommands._cfaddnx, CommandMode.Cuckoo | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(item);
    }

    internal static CommandPacket CF_Count(SharpValue key, SharpValue item)
    {
        return new CommandPacket(CuckooCommands._cfcount, CommandMode.Cuckoo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(item);
    }

    internal static CommandPacket CF_Del(SharpValue key, SharpValue item)
    {
        return new CommandPacket(CuckooCommands._cfdel, CommandMode.Cuckoo | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(item);
    }

    internal static CommandPacket CF_Exists(SharpValue key, SharpValue item)
    {
        return new CommandPacket(CuckooCommands._cfexists, CommandMode.Cuckoo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(item);
    }

    internal static CommandPacket CF_Insert(
        SharpValue key,
        SharpValue item,
        int? capacity,
        bool nocreate)
    {
        return CuckooCommands.InitCF_Insert(
            key: key,
            capacity: capacity,
            nocreate: nocreate)
        .WriteArguments(item);
    }

    internal static CommandPacket CF_Insert(
        SharpValue key,
        SharpValueSpan items,
        int? capacity,
        bool nocreate)
    {
        if (items.IsEmpty) throw new ArgumentException("Please ensure that there is at least one item.", nameof(items));
        return CuckooCommands.InitCF_Insert(
            key: key,
            capacity: capacity,
            nocreate: nocreate)
        .WriteArguments(items);
    }

    private static CommandPacket InitCF_Insert(
        SharpValue key,
        int? capacity,
        bool nocreate)
    {
        var command = new CommandPacket(CuckooCommands._cfinsert, CommandMode.Cuckoo | CommandMode.WithWrite)
            .WriteKeys(key);
        if (capacity.HasValue)
        {
            _ = command
                .WriteArguments("CAPACITY")
                .WriteArguments(capacity.Value.ToBytes());
        }
        return command
            .WriteArguments(nocreate, "NOCREATE")
            .WriteArguments("ITEMS");
    }

    internal static CommandPacket CF_InsertNx(
        SharpValue key,
        SharpValue item,
        int? capacity,
        bool nocreate)
    {
        return CuckooCommands.InitCF_InsertNx(
            key: key,
            capacity: capacity,
            nocreate: nocreate)
        .WriteArguments(item);
    }

    internal static CommandPacket CF_InsertNx(
        SharpValue key,
        SharpValueSpan items,
        int? capacity,
        bool nocreate)
    {
        if (items.IsEmpty) throw new ArgumentException("Please ensure that there is at least one item.", nameof(items));
        return CuckooCommands.InitCF_InsertNx(
            key: key,
            capacity: capacity,
            nocreate: nocreate)
        .WriteArguments(items);
    }

    private static CommandPacket InitCF_InsertNx(
        SharpValue key,
        int? capacity,
        bool nocreate)
    {
        var command = new CommandPacket(CuckooCommands._cfinsertnx, CommandMode.Cuckoo | CommandMode.WithWrite)
            .WriteKeys(key);
        if (capacity.HasValue)
        {
            _ = command
                .WriteArguments("CAPACITY")
                .WriteArguments(capacity.Value.ToBytes());
        }
        return command
            .WriteArguments(nocreate, "NOCREATE")
            .WriteArguments("ITEMS");
    }

    internal static CommandPacket CF_LoadChunk(SharpValue key, long iterator, byte[] data)
    {
        if (data is null or { Length: <= 0 }) throw new ArgumentException("The recovery data block cannot be empty.", nameof(data));
        return new CommandPacket(CuckooCommands._cfloadchunk, CommandMode.Cuckoo | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(iterator.ToBytes())
            .WriteArguments(data);
    }

    internal static CommandPacket CF_ScanDump(SharpValue key, long iterator)
    {
        return new CommandPacket(CuckooCommands._cfscandump, CommandMode.Cuckoo | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(iterator.ToBytes());
    }

    internal static CommandPacket CF_MExists(SharpValue key, SharpValue item)
    {
        return new CommandPacket(CuckooCommands._cfmexists, CommandMode.Cuckoo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(item);
    }

    internal static CommandPacket CF_MExists(SharpValue key, SharpValueSpan items)
    {
        if (items.IsEmpty) throw new ArgumentException("Please ensure that there is at least one item.", nameof(items));
        return new CommandPacket(CuckooCommands._cfmexists, CommandMode.Cuckoo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(items);
    }

    internal static CommandPacket CF_Reserve(
        SharpValue key,
        int capacity,
        int? bucketSize,
        int? maxIterations,
        int? expansion)
    {
        var command = new CommandPacket(CuckooCommands._cfreserve, CommandMode.Cuckoo | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(capacity.ToBytes());
        if (bucketSize.HasValue)
        {
            _ = command
                .WriteArguments("BUCKETSIZE")
                .WriteArguments(bucketSize.Value.ToBytes());
        }
        if (maxIterations.HasValue)
        {
            _ = command
                .WriteArguments("MAXITERATIONS")
                .WriteArguments(maxIterations.Value.ToBytes());
        }
        if (expansion.HasValue)
        {
            _ = command
                .WriteArguments("EXPANSION")
                .WriteArguments(expansion.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket CF_Info(SharpValue key)
    {
        return new CommandPacket(CuckooCommands._cfinfo, CommandMode.Cuckoo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }
}
