using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static SharpRedis.RedisStream;

namespace SharpRedis.Commands;

internal static class StreamCommands
{
    private readonly static SharpValue _xadd = new("XADD", CommandItemCategory.Command);
    private readonly static SharpValue _xlen = new("XLEN", CommandItemCategory.Command);
    private readonly static SharpValue _xdel = new("XDEL", CommandItemCategory.Command);
    private readonly static SharpValue _xdelex = new("XDELEX", CommandItemCategory.Command);
    private readonly static SharpValue _xack = new("XACK", CommandItemCategory.Command);
    private readonly static SharpValue _xread = new("XREAD", CommandItemCategory.Command);
    private readonly static SharpValue _xreadgroup = new("XREADGROUP", CommandItemCategory.Command);
    private readonly static SharpValue _xackdel = new("XACKDEL", CommandItemCategory.Command);
    private readonly static SharpValue _xautoclaim = new("XAUTOCLAIM", CommandItemCategory.Command);
    private readonly static SharpValue _xrange = new("XRANGE", CommandItemCategory.Command);
    private readonly static SharpValue _xrevrange = new("XREVRANGE", CommandItemCategory.Command);
    private readonly static SharpValue _xpending = new("XPENDING", CommandItemCategory.Command);
    private readonly static SharpValue _xcfgset = new("XCFGSET", CommandItemCategory.Command);
    private readonly static SharpValue _xclaim = new("XCLAIM", CommandItemCategory.Command);
    private readonly static SharpValue _xnack = new("XNACK", CommandItemCategory.Command);
    private readonly static SharpValue _xsetid = new("XSETID", CommandItemCategory.Command);
    private readonly static SharpValue _xtrim = new("XTRIM", CommandItemCategory.Command);
    private readonly static SharpValue _xidmprecord = new("XIDMPRECORD", CommandItemCategory.Command);

    private readonly static SharpValue _xgroup = new("XGROUP", CommandItemCategory.Command);
    private readonly static SharpValue _create = new("CREATE", CommandItemCategory.Command);
    private readonly static SharpValue _createconsumer = new("CREATECONSUMER", CommandItemCategory.Command);
    private readonly static SharpValue _delconsumer = new("DELCONSUMER", CommandItemCategory.Command);
    private readonly static SharpValue _destroy = new("DESTROY", CommandItemCategory.Command);
    private readonly static SharpValue _setid = new("SETID", CommandItemCategory.Command);

    private readonly static SharpValue _xinfo = new("XINFO", CommandItemCategory.Command);
    private readonly static SharpValue _consumers = new("CONSUMERS", CommandItemCategory.Command);
    private readonly static SharpValue _groups = new("GROUPS", CommandItemCategory.Command);
    private readonly static SharpValue _stream = new("STREAM", CommandItemCategory.Command);

    #region AXDD
    internal static CommandPacket XAdd(
        SharpValue key,
        bool nomkstream,
        DeletionPolicy delPolicy,
        IdempotentArg idempotent,
        TrimOptions trimOptions,
        EntryId id,
        SharpValue field,
        SharpValue value)
    {
        return StreamCommands.InitXAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id)
        .WriteArguments(field)
        .WriteArguments(value);
    }

    internal static CommandPacket XAdd(
        SharpValue key,
        bool nomkstream,
        DeletionPolicy delPolicy,
        IdempotentArg idempotent,
        TrimOptions trimOptions,
        EntryId id,
        (SharpValue field, SharpValue value) fieldValue)
    {
        return StreamCommands.InitXAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id)
        .WriteArguments(fieldValue.field)
        .WriteArguments(fieldValue.value);
    }

    internal static CommandPacket XAdd(
        SharpValue key,
        bool nomkstream,
        DeletionPolicy delPolicy,
        IdempotentArg idempotent,
        TrimOptions trimOptions,
        EntryId id,
        KeyValuePair<SharpValue, SharpValue> fieldValue)
    {
        return StreamCommands.InitXAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id)
        .WriteArguments(fieldValue.Key)
        .WriteArguments(fieldValue.Value);
    }

    internal static CommandPacket XAdd(
        SharpValue key,
        bool nomkstream,
        DeletionPolicy delPolicy,
        IdempotentArg idempotent,
        TrimOptions trimOptions,
        EntryId id,
        ICollection<(SharpValue field, SharpValue value)> fieldValueCollection)
    {
        if (fieldValueCollection is null or { Count: <= 0 }) throw new NotSupportedException("Please specify at least one field-value.");
        var command = StreamCommands.InitXAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id);
        foreach (var (field, value) in fieldValueCollection)
        {
            _ = command
                .WriteArguments(field)
                .WriteArguments(value);
        }
        return command;
    }

    internal static CommandPacket XAdd(
        SharpValue key,
        bool nomkstream,
        DeletionPolicy delPolicy,
        IdempotentArg idempotent,
        TrimOptions trimOptions,
        EntryId id,
        ICollection<KeyValuePair<SharpValue, SharpValue>> fieldValueCollection)
    {
        if (fieldValueCollection is null or { Count: <= 0 }) throw new NotSupportedException("Please specify at least one field-value.");
        var command = StreamCommands.InitXAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id);
        foreach (var fieldValue in fieldValueCollection)
        {
            _ = command
                .WriteArguments(fieldValue.Key)
                .WriteArguments(fieldValue.Value);
        }
        return command;
    }

    internal static CommandPacket XAdd(
        SharpValue key,
        bool nomkstream,
        DeletionPolicy delPolicy,
        IdempotentArg idempotent,
        TrimOptions trimOptions,
        EntryId id,
        ReadOnlySpan<(SharpValue field, SharpValue value)> fieldValueSpan)
    {
        if (fieldValueSpan.IsEmpty) throw new NotSupportedException("Please specify at least one field-value.");
        var command = StreamCommands.InitXAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id);
        foreach (var (field, value) in fieldValueSpan)
        {
            _ = command
                .WriteArguments(field)
                .WriteArguments(value);
        }
        return command;
    }

    internal static CommandPacket XAdd(
        SharpValue key,
        bool nomkstream,
        DeletionPolicy delPolicy,
        IdempotentArg idempotent,
        TrimOptions trimOptions,
        EntryId id,
        ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>> fieldValueSpan)
    {
        if (fieldValueSpan.IsEmpty) throw new NotSupportedException("Please specify at least one field-value.");
        var command = StreamCommands.InitXAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id);
        foreach (var fieldValue in fieldValueSpan)
        {
            _ = command
                .WriteArguments(fieldValue.Key)
                .WriteArguments(fieldValue.Value);
        }
        return command;
    }

    internal static CommandPacket InitXAdd(
        SharpValue key,
        bool nomkstream,
        DeletionPolicy delPolicy,
        IdempotentArg idempotent,
        TrimOptions trimOptions,
        EntryId id)
    {
        if (!id.HasValue) throw new ArgumentException("Entry ID is required.", nameof(id));
        var command = new CommandPacket(StreamCommands._xadd, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(nomkstream, "NOMKSTREAM");

        switch (delPolicy)
        {
            case DeletionPolicy.KeepRef:
                _ = command.WriteArguments("KEEPREF");
                break;
            case DeletionPolicy.DelRef:
                _ = command.WriteArguments("DELREF");
                break;
            case DeletionPolicy.Acked:
                _ = command.WriteArguments("ACKED");
                break;
            default:
                break;
        }

        if (idempotent.HasValue)
        {
            if (idempotent.Id.HasValue)
            {
                _ = command
                    .WriteArguments("IDMP")
                    .WriteArguments(idempotent.ProducerId)
                    .WriteArguments(idempotent.Id);
            }
            else
            {
                _ = command
                    .WriteArguments("IDMPAUTO")
                    .WriteArguments(idempotent.ProducerId);
            }
        }

        if (trimOptions.HasValue)
        {
            if (trimOptions.MaxLen.HasValue)
            {
                _ = command.WriteArguments("MAXLEN");
                switch (trimOptions.Operator)
                {
                    case TrimOperator.Exact:
                        _ = command.WriteArguments("=");
                        break;
                    case TrimOperator.Approximate:
                        _ = command.WriteArguments("~");
                        break;
                    default:
                        break;
                }
                _ = command.WriteArguments(trimOptions.MaxLen.Value.ToBytes());
            }
            else if (!string.IsNullOrEmpty(trimOptions.MinId))
            {
                _ = command.WriteArguments("MINID");
                switch (trimOptions.Operator)
                {
                    case TrimOperator.Exact:
                        _ = command.WriteArguments("=");
                        break;
                    case TrimOperator.Approximate:
                        _ = command.WriteArguments("~");
                        break;
                    default:
                        break;
                }
                _ = command.WriteArguments(trimOptions.MinId);
            }
            if (trimOptions.Limit.HasValue)
            {
                _ = command
                    .WriteArguments("LIMIT")
                    .WriteArguments(trimOptions.Limit.Value.ToBytes());
            }
        }

        return command.WriteArguments(id.Id);
    }
    #endregion

    internal static CommandPacket XLen(SharpValue key)
    {
        return new CommandPacket(StreamCommands._xlen, CommandMode.Stream | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket XDel(SharpValue key, EntryId id)
    {
        if (!id.HasValue) throw new ArgumentException("Entry ID is required.", nameof(id));
        return new CommandPacket(StreamCommands._xdel, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(id.Id);
    }

    internal static CommandPacket XDel(SharpValue key, ICollection<EntryId> ids)
    {
        if (ids is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = new CommandPacket(StreamCommands._xdel, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key);
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    internal static CommandPacket XDel(SharpValue key, ReadOnlySpan<EntryId> ids)
    {
        if (ids.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = new CommandPacket(StreamCommands._xdel, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key);
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    internal static CommandPacket XAck(SharpValue key, string group, EntryId id)
    {
        if (!id.HasValue) throw new ArgumentException("Entry ID is required.", nameof(id));
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        return new CommandPacket(StreamCommands._xack, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(group)
            .WriteArguments(id.Id);
    }

    internal static CommandPacket XAck(SharpValue key, string group, ICollection<EntryId> ids)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        if (ids is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = new CommandPacket(StreamCommands._xack, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(group);
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    internal static CommandPacket XAck(SharpValue key, string group, ReadOnlySpan<EntryId> ids)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        if (ids.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = new CommandPacket(StreamCommands._xack, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(group);
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    internal static CommandPacket XGroupCreate(
        SharpValue key,
        string group,
        EntryId id,
        bool mkStream,
        long? entriesRead)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        if (!id.HasValue) throw new ArgumentException("Entry ID is required.", nameof(id));
        var command = new CommandPacket(StreamCommands._xgroup, CommandMode.Stream | CommandMode.WithWrite)
            .WriteCommand(StreamCommands._create)
            .WriteKeys(key)
            .WriteArguments(group)
            .WriteArguments(id.Id)
            .WriteArguments(mkStream, "MKSTREAM");
        if (entriesRead.HasValue)
        {
            _ = command
                .WriteArguments("ENTRIESREAD")
                .WriteArguments(entriesRead.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket XRead(
        SharpValue key,
        EntryId id,
        long? count,
        ulong? block,
        ulong? maxCount,
        ulong? maxSize)
    {
        if (!id.HasValue) throw new ArgumentException("Entry ID is required.", nameof(id));
        var command = new CommandPacket(StreamCommands._xread, CommandMode.Stream | CommandMode.WithRead | CommandMode.WithClientCaching)
            .XReadAppendArgs(
                count: count,
                block: block,
                maxCount: maxCount,
                maxSize: maxSize);
        return command
            .WriteKeys(key)
            .WriteArguments(id.Id);
    }

    internal static CommandPacket XRead(
        ICollection<KeyValuePair<SharpValue, EntryId>> keysIds,
        long? count,
        ulong? block,
        ulong? maxCount,
        ulong? maxSize)
    {
        if (keysIds is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one Key-ID.");
        var command = new CommandPacket(StreamCommands._xread, CommandMode.Stream | CommandMode.WithRead | CommandMode.WithClientCaching)
            .XReadAppendArgs(
                count: count,
                block: block,
                maxCount: maxCount,
                maxSize: maxSize);
        foreach (var item in keysIds)
        {
            _ = command.WriteKeys(item.Key);
        }
        foreach (var item in keysIds)
        {
            if (!item.Value.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(item.Value.Id);
        }
        return command;
    }

    internal static CommandPacket XRead(
        ICollection<(SharpValue key, EntryId id)> keysIds,
        long? count,
        ulong? block,
        ulong? maxCount,
        ulong? maxSize)
    {
        if (keysIds is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one Key-ID.");
        var command = new CommandPacket(StreamCommands._xread, CommandMode.Stream | CommandMode.WithRead | CommandMode.WithClientCaching)
            .XReadAppendArgs(
                count: count,
                block: block,
                maxCount: maxCount,
                maxSize: maxSize);
        foreach (var (key, _) in keysIds)
        {
            _ = command.WriteKeys(key);
        }
        foreach (var (_, id) in keysIds)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    internal static CommandPacket XRead(
        ReadOnlySpan<KeyValuePair<SharpValue, EntryId>> keysIds,
        long? count,
        ulong? block,
        ulong? maxCount,
        ulong? maxSize)
    {
        if (keysIds.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one Key-ID.");
        var command = new CommandPacket(StreamCommands._xread, CommandMode.Stream | CommandMode.WithRead | CommandMode.WithClientCaching)
            .XReadAppendArgs(
                count: count,
                block: block,
                maxCount: maxCount,
                maxSize: maxSize);
        foreach (var item in keysIds)
        {
            _ = command.WriteKeys(item.Key);
        }
        foreach (var item in keysIds)
        {
            if (!item.Value.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(item.Value.Id);
        }
        return command;
    }

    internal static CommandPacket XRead(
        ReadOnlySpan<(SharpValue key, EntryId id)> keysIds,
        long? count,
        ulong? block,
        ulong? maxCount,
        ulong? maxSize)
    {
        if (keysIds.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one Key-ID.");
        var command = new CommandPacket(StreamCommands._xread, CommandMode.Stream | CommandMode.WithRead | CommandMode.WithClientCaching)
            .XReadAppendArgs(
                count: count,
                block: block,
                maxCount: maxCount,
                maxSize: maxSize);
        foreach (var (key, _) in keysIds)
        {
            _ = command.WriteKeys(key);
        }
        foreach (var (_, id) in keysIds)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CommandPacket XReadAppendArgs(
        this CommandPacket command,
        long? count,
        ulong? block,
        ulong? maxCount,
        ulong? maxSize)
    {
        if (count.HasValue)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes());
        }
        if (maxCount.HasValue)
        {
            _ = command
                .WriteArguments("MAXCOUNT")
                .WriteArguments(maxCount.Value.ToBytes());
        }
        if (maxSize.HasValue)
        {
            _ = command
                .WriteArguments("MAXSIZE")
                .WriteArguments(maxSize.Value.ToBytes());
        }
        if (block.HasValue)
        {
            _ = command & CommandMode.WithClientCaching;
            _ = command | CommandMode.WithBlocking;
            _ = command
                .WriteArguments("BLOCK")
                .WriteArguments(block.Value.ToBytes());
        }
        return command.WriteArguments("STREAMS");
    }

    internal static CommandPacket XReadGroup(
        SharpValue key,
        EntryId id,
        string group,
        string consumer,
        long? count,
        ulong? maxCount,
        ulong? maxSize,
        ulong? block,
        ulong? claim,
        bool noack)
    {
        if (!id.HasValue) throw new ArgumentException("Entry ID is required.", nameof(id));
        var command = new CommandPacket(StreamCommands._xreadgroup, CommandMode.Stream | CommandMode.WithWrite)
            .XReadGroupAppendArgs(
                group: group,
                consumer: consumer,
                count: count,
                maxCount: maxCount,
                maxSize: maxSize,
                block: block,
                claim: claim,
                noack: noack);
        return command
            .WriteKeys(key)
            .WriteArguments(id.Id);
    }

    internal static CommandPacket XReadGroup(
        ICollection<KeyValuePair<SharpValue, EntryId>> keysIds,
        string group,
        string consumer,
        long? count,
        ulong? maxCount,
        ulong? maxSize,
        ulong? block,
        ulong? claim,
        bool noack)
    {
        if (keysIds is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one Key-ID.");
        var command = new CommandPacket(StreamCommands._xreadgroup, CommandMode.Stream | CommandMode.WithWrite)
            .XReadGroupAppendArgs(
                group: group,
                consumer: consumer,
                count: count,
                maxCount: maxCount,
                maxSize: maxSize,
                block: block,
                claim: claim,
                noack: noack);
        foreach (var item in keysIds)
        {
            _ = command.WriteKeys(item.Key);
        }
        foreach (var item in keysIds)
        {
            if (!item.Value.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(item.Value.Id);
        }
        return command;
    }

    internal static CommandPacket XReadGroup(
        ICollection<(SharpValue key, EntryId id)> keysIds,
        string group,
        string consumer,
        long? count,
        ulong? maxCount,
        ulong? maxSize,
        ulong? block,
        ulong? claim,
        bool noack)
    {
        if (keysIds is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one Key-ID.");
        var command = new CommandPacket(StreamCommands._xreadgroup, CommandMode.Stream | CommandMode.WithWrite)
            .XReadGroupAppendArgs(
                group: group,
                consumer: consumer,
                count: count,
                maxCount: maxCount,
                maxSize: maxSize,
                block: block,
                claim: claim,
                noack: noack);
        foreach (var (key, _) in keysIds)
        {
            _ = command.WriteKeys(key);
        }
        foreach (var (_, id) in keysIds)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    internal static CommandPacket XReadGroup(
        ReadOnlySpan<KeyValuePair<SharpValue, EntryId>> keysIds,
        string group,
        string consumer,
        long? count,
        ulong? maxCount,
        ulong? maxSize,
        ulong? block,
        ulong? claim,
        bool noack)
    {
        if (keysIds.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one Key-ID.");
        var command = new CommandPacket(StreamCommands._xreadgroup, CommandMode.Stream | CommandMode.WithWrite)
            .XReadGroupAppendArgs(
                group: group,
                consumer: consumer,
                count: count,
                maxCount: maxCount,
                maxSize: maxSize,
                block: block,
                claim: claim,
                noack: noack);
        foreach (var item in keysIds)
        {
            _ = command.WriteKeys(item.Key);
        }
        foreach (var item in keysIds)
        {
            if (!item.Value.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(item.Value.Id);
        }
        return command;
    }

    internal static CommandPacket XReadGroup(
        ReadOnlySpan<(SharpValue key, EntryId id)> keysIds,
        string group,
        string consumer,
        long? count,
        ulong? maxCount,
        ulong? maxSize,
        ulong? block,
        ulong? claim,
        bool noack)
    {
        if (keysIds.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one Key-ID.");
        var command = new CommandPacket(StreamCommands._xreadgroup, CommandMode.Stream | CommandMode.WithWrite)
            .XReadGroupAppendArgs(
                group: group,
                consumer: consumer,
                count: count,
                maxCount: maxCount,
                maxSize: maxSize,
                block: block,
                claim: claim,
                noack: noack);
        foreach (var (key, _) in keysIds)
        {
            _ = command.WriteKeys(key);
        }
        foreach (var (_, id) in keysIds)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CommandPacket XReadGroupAppendArgs(
        this CommandPacket command,
        string group,
        string consumer,
        long? count,
        ulong? maxCount,
        ulong? maxSize,
        ulong? block,
        ulong? claim,
        bool noack)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        if (consumer is null) throw new NotSupportedException("The name of the consumer cannot be null.");
        _ = command
            .WriteArguments("GROUP")
            .WriteArguments(group)
            .WriteArguments(consumer);
        if (count.HasValue)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes());
        }
        if (maxCount.HasValue)
        {
            _ = command
                .WriteArguments("MAXCOUNT")
                .WriteArguments(maxCount.Value.ToBytes());
        }
        if (maxSize.HasValue)
        {
            _ = command
                .WriteArguments("MAXSIZE")
                .WriteArguments(maxSize.Value.ToBytes());
        }
        if (block.HasValue)
        {
            _ = command | CommandMode.WithBlocking;
            _ = command
                .WriteArguments("BLOCK")
                .WriteArguments(block.Value.ToBytes());
        }
        if (claim.HasValue)
        {
            _ = command
                .WriteArguments("CLAIM")
                .WriteArguments(claim.Value.ToBytes());
        }
        if (noack)
        {
            _ = command.WriteArguments("NOACK");
        }
        return command.WriteArguments("STREAMS");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CommandPacket InitXAckDel(SharpValue key, string group, DeletionPolicy delPolicy)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        var command = new CommandPacket(StreamCommands._xackdel, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(group);
        switch (delPolicy)
        {
            case DeletionPolicy.KeepRef:
                _ = command.WriteArguments("KEEPREF");
                break;
            case DeletionPolicy.DelRef:
                _ = command.WriteArguments("DELREF");
                break;
            case DeletionPolicy.Acked:
                _ = command.WriteArguments("ACKED");
                break;
            default:
                break;
        }
        return command.WriteArguments("IDS");
    }

    internal static CommandPacket XAckDel(SharpValue key, string group, DeletionPolicy delPolicy, EntryId id)
    {
        if (!id.HasValue) throw new ArgumentException("Entry ID is required.", nameof(id));
        return StreamCommands.InitXAckDel(key: key, group, delPolicy: delPolicy)
            .WriteArguments("1")
            .WriteArguments(id.Id);
    }

    internal static CommandPacket XAckDel(SharpValue key, string group, DeletionPolicy delPolicy, ICollection<EntryId> ids)
    {
        if (ids is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = StreamCommands.InitXAckDel(key: key, group, delPolicy: delPolicy);
        _ = command.WriteArguments(ids.Count.ToBytes());
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    internal static CommandPacket XAckDel(SharpValue key, string group, DeletionPolicy delPolicy, ReadOnlySpan<EntryId> ids)
    {
        if (ids.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = StreamCommands.InitXAckDel(key: key, group, delPolicy: delPolicy);
        _ = command.WriteArguments(ids.Length.ToBytes());
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    internal static CommandPacket XAutoClaim(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        EntryId start,
        long? count,
        bool justid)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        if (consumer is null) throw new NotSupportedException("The name of the consumer cannot be null.");
        if (!start.HasValue) throw new ArgumentException("Start entry ID is required.", nameof(start));
        var command = new CommandPacket(StreamCommands._xautoclaim, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(group)
            .WriteArguments(consumer)
            .WriteArguments(minIdleTime.ToBytes())
            .WriteArguments(start.Id);
        if (count.HasValue)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes());
        }
        return command.WriteArguments(justid, "JUSTID");
    }

    internal static CommandPacket XRange(SharpValue key, EntryId start, EntryId end, long? count)
    {
        if (!start.HasValue) throw new ArgumentException("Start entry ID is required.", nameof(start));
        if (!end.HasValue) throw new ArgumentException("End entry ID is required.", nameof(end));
        var command = new CommandPacket(StreamCommands._xrange, CommandMode.Stream | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(start.Id)
            .WriteArguments(end.Id);
        if (count.HasValue)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket XRevRange(SharpValue key, EntryId end, EntryId start, long? count)
    {
        if (!end.HasValue) throw new ArgumentException("End entry ID is required.", nameof(end));
        if (!start.HasValue) throw new ArgumentException("Start entry ID is required.", nameof(start));
        var command = new CommandPacket(StreamCommands._xrevrange, CommandMode.Stream | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(end.Id)
            .WriteArguments(start.Id);
        if (count.HasValue)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket XPending(SharpValue key, string group, PendingOptions options)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        var command = new CommandPacket(StreamCommands._xpending, CommandMode.Stream | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(group);
        if (options.HasValue)
        {
            if (options.MinIdleTime.HasValue)
            {
                _ = command
                    .WriteArguments("IDLE")
                    .WriteArguments(options.MinIdleTime.Value.ToBytes());
            }
            _ = command
                .WriteArguments(options.Start.Id)
                .WriteArguments(options.End.Id)
                .WriteArguments(options.Count.ToBytes());
            if (options.Consumer != null)
            {
                _ = command.WriteArguments(options.Consumer);
            }
        }
        return command;
    }

    internal static CommandPacket XCfgSet(SharpValue key, int? duration, int? maxsize)
    {
        if (!duration.HasValue && !maxsize.HasValue)
        {
            throw new ArgumentException("At least one of 'duration' or 'maxsize' must be specified.");
        }
        if (duration is < 1 or > 86_400)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), "'duration' valid range: 1-86,400 seconds.");
        }
        if (maxsize is < 1 or > 10_000)
        {
            throw new ArgumentOutOfRangeException(nameof(maxsize), "'maxsize' valid range: 1-10,000 entries.");
        }
        var command = new CommandPacket(StreamCommands._xcfgset, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key);
        if (duration.HasValue)
        {
            _ = command
                .WriteArguments("IDMP-DURATION")
                .WriteArguments(duration.Value.ToBytes());
        }
        if (maxsize.HasValue)
        {
            _ = command
                .WriteArguments("IDMP-MAXSIZE")
                .WriteArguments(maxsize.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket XClaim(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        EntryId id,
        ulong? idle,
        SharpExpiry? time,
        ulong? retryCount,
        bool force,
        bool justid,
        EntryId lastid)
    {
        if (!id.HasValue) throw new ArgumentException("Entry ID is required.", nameof(id));
        return StreamCommands.InitXClaim(
            key: key,
            group: group,
            consumer: consumer,
            minIdleTime: minIdleTime)
        .WriteArguments(id.Id)
        .AppendXClaimArgs(idle, time, retryCount, force, justid, lastid);
    }

    internal static CommandPacket XClaim(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        ICollection<EntryId> ids,
        ulong? idle,
        SharpExpiry? time,
        ulong? retryCount,
        bool force,
        bool justid,
        EntryId lastid)
    {
        if (ids is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = StreamCommands.InitXClaim(
            key: key,
            group: group,
            consumer: consumer,
            minIdleTime: minIdleTime);
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command
            .AppendXClaimArgs(idle, time, retryCount, force, justid, lastid);
    }

    internal static CommandPacket XClaim(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        ReadOnlySpan<EntryId> ids,
        ulong? idle,
        SharpExpiry? time,
        ulong? retryCount,
        bool force,
        bool justid,
        EntryId lastid)
    {
        if (ids.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = StreamCommands.InitXClaim(
            key: key,
            group: group,
            consumer: consumer,
            minIdleTime: minIdleTime);
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command
            .AppendXClaimArgs(idle, time, retryCount, force, justid, lastid);
    }

    private static CommandPacket InitXClaim(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        if (consumer is null) throw new NotSupportedException("The name of the consumer cannot be null.");
        return new CommandPacket(StreamCommands._xclaim, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(group)
            .WriteArguments(consumer)
            .WriteArguments(minIdleTime.ToBytes());
    }

    private static CommandPacket AppendXClaimArgs(
        this CommandPacket command,
        ulong? idle,
        SharpExpiry? time,
        ulong? retryCount,
        bool force,
        bool justid,
        EntryId lastid)
    {
        if (idle.HasValue)
        {
            _ = command
                .WriteArguments("IDLE")
                .WriteArguments(idle.Value.ToBytes());
        }
        if (time.HasValue)
        {
            _ = command
                .WriteArguments("TIME")
                .WriteArguments(time.Value.GetUnixTimeMilliseconds.ToBytes());
        }
        if (retryCount.HasValue)
        {
            _ = command
                .WriteArguments("RETRYCOUNT")
                .WriteArguments(retryCount.Value.ToBytes());
        }
        _ = command
            .WriteArguments(force, "FORCE")
            .WriteArguments(justid, "JUSTID");
        if (lastid.HasValue)
        {
            _ = command
                .WriteArguments("LASTID")
                .WriteArguments(lastid.Id);
        }
        return command;
    }

    internal static CommandPacket XDelEx(SharpValue key, EntryId id, DeletionPolicy delPolicy)
    {
        if (!id.HasValue) throw new ArgumentException("Entry ID is required.", nameof(id));
        return StreamCommands.InitXDelEx(key, delPolicy)
            .WriteArguments("1")
            .WriteArguments(id.Id);
    }

    internal static CommandPacket XDelEx(SharpValue key, ICollection<EntryId> ids, DeletionPolicy delPolicy)
    {
        if (ids is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = StreamCommands.InitXDelEx(key, delPolicy)
            .WriteArguments(ids.Count.ToBytes());
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    internal static CommandPacket XDelEx(SharpValue key, ReadOnlySpan<EntryId> ids, DeletionPolicy delPolicy)
    {
        if (ids.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = StreamCommands.InitXDelEx(key, delPolicy)
            .WriteArguments(ids.Length.ToBytes());
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command;
    }

    private static CommandPacket InitXDelEx(SharpValue key, DeletionPolicy delPolicy)
    {
        var command = new CommandPacket(StreamCommands._xdelex, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key);
        switch (delPolicy)
        {
            case DeletionPolicy.KeepRef:
                _ = command.WriteArguments("KEEPREF");
                break;
            case DeletionPolicy.DelRef:
                _ = command.WriteArguments("DELREF");
                break;
            case DeletionPolicy.Acked:
                _ = command.WriteArguments("ACKED");
                break;
            default:
                break;
        }
        return command.WriteArguments("IDS");
    }

    internal static CommandPacket XGroupCreateConsumer(SharpValue key, string group, string consumer)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        if (consumer is null) throw new NotSupportedException("The name of the consumer cannot be null.");
        return new CommandPacket(StreamCommands._xgroup, CommandMode.Stream | CommandMode.WithWrite)
            .WriteCommand(StreamCommands._createconsumer)
            .WriteKeys(key)
            .WriteArguments(group)
            .WriteArguments(consumer);
    }

    internal static CommandPacket XGroupDelConsumer(SharpValue key, string group, string consumer)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        if (consumer is null) throw new NotSupportedException("The name of the consumer cannot be null.");
        return new CommandPacket(StreamCommands._xgroup, CommandMode.Stream | CommandMode.WithWrite)
            .WriteCommand(StreamCommands._delconsumer)
            .WriteKeys(key)
            .WriteArguments(group)
            .WriteArguments(consumer);
    }

    internal static CommandPacket XGroupDestroy(SharpValue key, string group)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        return new CommandPacket(StreamCommands._xgroup, CommandMode.Stream | CommandMode.WithWrite)
            .WriteCommand(StreamCommands._destroy)
            .WriteKeys(key)
            .WriteArguments(group);
    }

    internal static CommandPacket XGroupSetID(
        SharpValue key,
        string group,
        EntryId id,
        long? entriesRead)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        if (!id.HasValue) throw new ArgumentException("Entry ID is required.", nameof(id));
        var command = new CommandPacket(StreamCommands._xgroup, CommandMode.Stream | CommandMode.WithWrite)
            .WriteCommand(StreamCommands._setid)
            .WriteKeys(key)
            .WriteArguments(group)
            .WriteArguments(id.Id);
        if (entriesRead.HasValue)
        {
            _ = command
                .WriteArguments("ENTRIESREAD")
                .WriteArguments(entriesRead.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket XInfoConsumers(SharpValue key, string group)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        return new CommandPacket(StreamCommands._xinfo, CommandMode.Stream | CommandMode.WithRead)
            .WriteCommand(StreamCommands._consumers)
            .WriteKeys(key)
            .WriteArguments(group);
    }

    internal static CommandPacket XInfoGroups(SharpValue key)
    {
        return new CommandPacket(StreamCommands._xinfo, CommandMode.Stream | CommandMode.WithRead)
            .WriteCommand(StreamCommands._groups)
            .WriteKeys(key);
    }

    internal static CommandPacket XInfoStream(SharpValue key, bool full, long? count)
    {
        var command = new CommandPacket(StreamCommands._xinfo, CommandMode.Stream | CommandMode.WithRead)
            .WriteCommand(StreamCommands._stream)
            .WriteKeys(key);
        if (count.HasValue)
        {
            return command
                .WriteArguments("FULL")
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes());
        }
        else
        {
            return command.WriteArguments(full, "FULL");
        }
    }

    internal static CommandPacket XNAck(
        SharpValue key,
        string group,
        NackMode mode,
        EntryId id,
        long? retryCount,
        bool force)
    {
        if (!id.HasValue) throw new ArgumentException("Entry ID is required.", nameof(id));
        return StreamCommands.InitXNAck(key, group, mode)
            .WriteArguments("1")
            .WriteArguments(id.Id)
            .AppendXNAckArgs(retryCount, force);
    }

    internal static CommandPacket XNAck(
        SharpValue key,
        string group,
        NackMode mode,
        ICollection<EntryId> ids,
        long? retryCount,
        bool force)
    {
        if (ids is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = StreamCommands.InitXNAck(key, group, mode)
            .WriteArguments(ids.Count.ToBytes());
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command.AppendXNAckArgs(retryCount, force);
    }

    internal static CommandPacket XNAck(
        SharpValue key,
        string group,
        NackMode mode,
        ReadOnlySpan<EntryId> ids,
        long? retryCount,
        bool force)
    {
        if (ids.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one ID.");
        var command = StreamCommands.InitXNAck(key, group, mode)
            .WriteArguments(ids.Length.ToBytes());
        foreach (var id in ids)
        {
            if (!id.HasValue) throw new ArgumentException("Entry ID is required.");
            _ = command.WriteArguments(id.Id);
        }
        return command.AppendXNAckArgs(retryCount, force);
    }

    private static CommandPacket InitXNAck(SharpValue key, string group, NackMode mode)
    {
        if (group is null) throw new NotSupportedException("The name of the consumer group cannot be null.");
        var command = new CommandPacket(StreamCommands._xnack, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(group);
        _ = mode switch
        {
            NackMode.Silent => command.WriteArguments("SILENT"),
            NackMode.Fail => command.WriteArguments("FAIL"),
            NackMode.Fatal => command.WriteArguments("FATAL"),
            _ => throw new NotSupportedException("Unsupported Controls the delivery counter adjustment."),
        };
        return command.WriteArguments("IDS");
    }

    private static CommandPacket AppendXNAckArgs(this CommandPacket command, long? retryCount, bool force)
    {
        if (retryCount.HasValue)
        {
            _ = command
                .WriteArguments("RETRYCOUNT")
                .WriteArguments(retryCount.Value.ToBytes());
        }
        return command
            .WriteArguments(force, "FORCE");
    }

    internal static CommandPacket XSetID(
        SharpValue key,
        EntryId lastId,
        long? entriesAdded,
        EntryId maxDeletedId)
    {
        var command = new CommandPacket(StreamCommands._xsetid, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(lastId.Id);
        if (entriesAdded.HasValue)
        {
            _ = command
                .WriteArguments("ENTRIESADDED")
                .WriteArguments(entriesAdded.Value.ToBytes());
        }
        if (maxDeletedId.HasValue)
        {
            _ = command
                .WriteArguments("MAXDELETEDID")
                .WriteArguments(maxDeletedId.Id);
        }
        return command;
    }

    internal static CommandPacket XTrim(
        SharpValue key,
        TrimOptions trimOptions,
        DeletionPolicy delPolicy)
    {
        if (!trimOptions.HasValue) throw new ArgumentException("TrimOptions is required.", nameof(trimOptions));
        var command = new CommandPacket(StreamCommands._xtrim, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key);

        if (trimOptions.MaxLen.HasValue)
        {
            _ = command.WriteArguments("MAXLEN");
            switch (trimOptions.Operator)
            {
                case TrimOperator.Exact:
                    _ = command.WriteArguments("=");
                    break;
                case TrimOperator.Approximate:
                    _ = command.WriteArguments("~");
                    break;
                default:
                    break;
            }
            _ = command.WriteArguments(trimOptions.MaxLen.Value.ToBytes());
        }
        else if (!string.IsNullOrEmpty(trimOptions.MinId))
        {
            _ = command.WriteArguments("MINID");
            switch (trimOptions.Operator)
            {
                case TrimOperator.Exact:
                    _ = command.WriteArguments("=");
                    break;
                case TrimOperator.Approximate:
                    _ = command.WriteArguments("~");
                    break;
                default:
                    break;
            }
            _ = command.WriteArguments(trimOptions.MinId);
        }
        if (trimOptions.Limit.HasValue)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(trimOptions.Limit.Value.ToBytes());
        }
        return delPolicy switch
        {
            DeletionPolicy.KeepRef => command.WriteArguments("KEEPREF"),
            DeletionPolicy.DelRef => command.WriteArguments("DELREF"),
            DeletionPolicy.Acked => command.WriteArguments("ACKED"),
            _ => command,
        };
    }

    internal static CommandPacket XIdmpRecord(
        SharpValue key,
        string pid,
        string iid,
        EntryId streamId)
    {
        if (pid is null) throw new NotSupportedException("The pid cannot be null.");
        if (iid is null) throw new NotSupportedException("The iid cannot be null.");
        if (!streamId.HasValue) throw new ArgumentException("Stream ID is required.");
        return new CommandPacket(StreamCommands._xidmprecord, CommandMode.Stream | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(pid)
            .WriteArguments(iid)
            .WriteArguments(streamId.Id);
    }
}
