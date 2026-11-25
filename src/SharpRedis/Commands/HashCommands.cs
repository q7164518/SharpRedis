using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using static SharpRedis.RedisHash;

namespace SharpRedis.Commands;

internal static class HashCommands
{
    private const string _fields = "FIELDS";
    private readonly static SharpValue _hget = new("HGET", CommandItemCategory.Command);
    private readonly static SharpValue _hset = new("HSET", CommandItemCategory.Command);
    private readonly static SharpValue _hdel = new("HDEL", CommandItemCategory.Command);
    private readonly static SharpValue _hexists = new("HEXISTS", CommandItemCategory.Command);
    private readonly static SharpValue _hexpire = new("HEXPIRE", CommandItemCategory.Command);
    private readonly static SharpValue _hpexpire = new("HPEXPIRE", CommandItemCategory.Command);
    private readonly static SharpValue _hexpiretime = new("HEXPIRETIME", CommandItemCategory.Command);
    private readonly static SharpValue _hpexpiretime = new("HPEXPIRETIME", CommandItemCategory.Command);
    private readonly static SharpValue _hexpireat = new("HEXPIREAT", CommandItemCategory.Command);
    private readonly static SharpValue _hpexpireat = new("HPEXPIREAT", CommandItemCategory.Command);
    private readonly static SharpValue _hgetall = new("HGETALL", CommandItemCategory.Command);
    private readonly static SharpValue _hgetdel = new("HGETDEL", CommandItemCategory.Command);
    private readonly static SharpValue _hgetex = new("HGETEX", CommandItemCategory.Command);
    private readonly static SharpValue _httl = new("HTTL", CommandItemCategory.Command);
    private readonly static SharpValue _hpttl = new("HPTTL", CommandItemCategory.Command);
    private readonly static SharpValue _hincrby = new("HINCRBY", CommandItemCategory.Command);
    private readonly static SharpValue _hincrbyfloat = new("HINCRBYFLOAT", CommandItemCategory.Command);
    private readonly static SharpValue _hkeys = new("HKEYS", CommandItemCategory.Command);
    private readonly static SharpValue _hlen = new("HLEN", CommandItemCategory.Command);
    private readonly static SharpValue _hmget = new("HMGET", CommandItemCategory.Command);
    private readonly static SharpValue _hmset = new("HMSET", CommandItemCategory.Command);
    private readonly static SharpValue _hpersist = new("HPERSIST", CommandItemCategory.Command);
    private readonly static SharpValue _hrandfield = new("HRANDFIELD", CommandItemCategory.Command);
    private readonly static SharpValue _hsetex = new("HSETEX", CommandItemCategory.Command);
    private readonly static SharpValue _hsetnx = new("HSETNX", CommandItemCategory.Command);
    private readonly static SharpValue _hstrlen = new("HSTRLEN", CommandItemCategory.Command);
    private readonly static SharpValue _hvals = new("HVALS", CommandItemCategory.Command);
    private readonly static SharpValue _hscan = new("HSCAN", CommandItemCategory.Command);
    private readonly static SharpValue _himport = new("HIMPORT", CommandItemCategory.Command);
    private readonly static SharpValue _prepare = new("PREPARE", CommandItemCategory.Command);
    private readonly static SharpValue _discard = new("DISCARD", CommandItemCategory.Command);
    private readonly static SharpValue _discardall = new("DISCARDALL", CommandItemCategory.Command);
    private readonly static SharpValue _set = new("SET", CommandItemCategory.Command);

    internal static CommandPacket HSet(SharpValue key, ICollection<KeyValuePair<SharpValue, SharpValue>> kv)
    {
        var command = new CommandPacket(HashCommands._hset, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key);
        foreach (var item in kv)
        {
            _ = command.WriteArguments(item.Key)
                .WriteArguments(item.Value);
        }
        return command;
    }

    internal static CommandPacket HSet(SharpValue key, KeyValuePair<SharpValue, SharpValue> kv)
    {
        var command = new CommandPacket(HashCommands._hset, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(kv.Key)
            .WriteArguments(kv.Value);
        return command;
    }

    internal static CommandPacket HGet(SharpValue key, SharpValue field)
    {
        return new CommandPacket(HashCommands._hget, CommandMode.Hash | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(field);
    }

    internal static CommandPacket HDel(SharpValue key, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        return new CommandPacket(HashCommands._hdel, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(fields);
    }

    internal static CommandPacket HDel(SharpValue key, SharpValue field)
    {
        return new CommandPacket(HashCommands._hdel, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(field);
    }

    internal static CommandPacket HExists(SharpValue key, SharpValue field)
    {
        return new CommandPacket(HashCommands._hexists, CommandMode.Hash | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(field);
    }

    internal static CommandPacket HExpire(SharpValue key, long seconds, ExpirationWhen ec, SharpValue field)
    {
        var command = new CommandPacket(HashCommands._hexpire, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(seconds.ToBytes());

        switch (ec)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments(nameof(ExpirationWhen.NX));
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments(nameof(ExpirationWhen.XX));
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments(nameof(ExpirationWhen.GT));
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments(nameof(ExpirationWhen.LT));
                break;
            default: break;
        }

        return command
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field);
    }

    internal static CommandPacket HExpire(SharpValue key, long seconds, ExpirationWhen ec, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        var command = new CommandPacket(HashCommands._hexpire, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(seconds.ToBytes());

        switch (ec)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments(nameof(ExpirationWhen.NX));
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments(nameof(ExpirationWhen.XX));
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments(nameof(ExpirationWhen.GT));
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments(nameof(ExpirationWhen.LT));
                break;
            default: break;
        }

        return command
            .WriteArguments(HashCommands._fields)
            .WriteArguments(fields.Count.ToBytes())
            .WriteArguments(fields);
    }

    internal static CommandPacket HPExpire(SharpValue key, long milliseconds, ExpirationWhen ec, SharpValue field)
    {
        var command = new CommandPacket(HashCommands._hpexpire, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(milliseconds.ToBytes());

        switch (ec)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments(nameof(ExpirationWhen.NX));
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments(nameof(ExpirationWhen.XX));
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments(nameof(ExpirationWhen.GT));
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments(nameof(ExpirationWhen.LT));
                break;
            default: break;
        }

        return command
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field);
    }

    internal static CommandPacket HPExpire(SharpValue key, long milliseconds, ExpirationWhen ec, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        var command = new CommandPacket(HashCommands._hpexpire, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(milliseconds.ToBytes());

        switch (ec)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments(nameof(ExpirationWhen.NX));
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments(nameof(ExpirationWhen.XX));
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments(nameof(ExpirationWhen.GT));
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments(nameof(ExpirationWhen.LT));
                break;
            default: break;
        }

        return command
            .WriteArguments(HashCommands._fields)
            .WriteArguments(fields.Length.ToBytes())
            .WriteArguments(fields);
    }

    internal static CommandPacket HExpireTime(SharpValue key, SharpValue field)
    {
        return new CommandPacket(HashCommands._hexpiretime, CommandMode.Hash | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field);
    }

    internal static CommandPacket HExpireTime(SharpValue key, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        return new CommandPacket(HashCommands._hexpiretime, CommandMode.Hash | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments(fields.Length.ToBytes())
            .WriteArguments(fields);
    }

    internal static CommandPacket HPExpireTime(SharpValue key, SharpValue field)
    {
        return new CommandPacket(HashCommands._hpexpiretime, CommandMode.Hash | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field);
    }

    internal static CommandPacket HPExpireTime(SharpValue key, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        return new CommandPacket(HashCommands._hpexpiretime, CommandMode.Hash | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments(fields.Count.ToBytes())
            .WriteArguments(fields);
    }

    internal static CommandPacket HExpireAt(SharpValue key, long unixTimeSeconds, ExpirationWhen ec, SharpValue field)
    {
        var command = new CommandPacket(HashCommands._hexpireat, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(unixTimeSeconds.ToBytes());

        switch (ec)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments(nameof(ExpirationWhen.NX));
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments(nameof(ExpirationWhen.XX));
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments(nameof(ExpirationWhen.GT));
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments(nameof(ExpirationWhen.LT));
                break;
            default: break;
        }

        return command
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field);
    }

    internal static CommandPacket HExpireAt(SharpValue key, long unixTimeSeconds, ExpirationWhen ec, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        var command = new CommandPacket(HashCommands._hexpireat, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(unixTimeSeconds.ToBytes());

        switch (ec)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments(nameof(ExpirationWhen.NX));
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments(nameof(ExpirationWhen.XX));
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments(nameof(ExpirationWhen.GT));
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments(nameof(ExpirationWhen.LT));
                break;
            default: break;
        }

        return command
            .WriteArguments(HashCommands._fields)
            .WriteArguments(fields.Count.ToBytes())
            .WriteArguments(fields);
    }

    internal static CommandPacket HPExpireAt(SharpValue key, long unixTimeMilliseconds, ExpirationWhen ec, SharpValue field)
    {
        var command = new CommandPacket(HashCommands._hpexpireat, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(unixTimeMilliseconds.ToBytes());

        switch (ec)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments(nameof(ExpirationWhen.NX));
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments(nameof(ExpirationWhen.XX));
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments(nameof(ExpirationWhen.GT));
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments(nameof(ExpirationWhen.LT));
                break;
            default: break;
        }

        return command
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field);
    }

    internal static CommandPacket HPExpireAt(SharpValue key, long unixTimeMilliseconds, ExpirationWhen ec, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        var command = new CommandPacket(HashCommands._hpexpireat, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(unixTimeMilliseconds.ToBytes());

        switch (ec)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments(nameof(ExpirationWhen.NX));
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments(nameof(ExpirationWhen.XX));
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments(nameof(ExpirationWhen.GT));
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments(nameof(ExpirationWhen.LT));
                break;
            default: break;
        }

        return command
            .WriteArguments(HashCommands._fields)
            .WriteArguments(fields.Count.ToBytes())
            .WriteArguments(fields);
    }

    internal static CommandPacket HGetAll(SharpValue key)
    {
        return new CommandPacket(HashCommands._hgetall, CommandMode.Hash | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket HGetDel(SharpValue key, SharpValue field)
    {
        return new CommandPacket(HashCommands._hgetdel, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field);
    }

    internal static CommandPacket HGetDel(SharpValue key, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        return new CommandPacket(HashCommands._hgetdel, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments(fields.Count.ToBytes())
            .WriteArguments(fields);
    }

    internal static CommandPacket HGetEx(SharpValue key, SharpExpiry? expiry, ExpirationFlags expiryFlags, SharpValue field)
    {
        return new CommandPacket(HashCommands._hgetex, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteExpiry(expiry, expiryFlags)
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field);
    }

    internal static CommandPacket HGetEx(SharpValue key, SharpExpiry? expiry, ExpirationFlags expiryFlags, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        return new CommandPacket(HashCommands._hgetex, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteExpiry(expiry, expiryFlags)
            .WriteArguments(HashCommands._fields)
            .WriteArguments(fields.Count.ToBytes())
            .WriteArguments(fields);
    }

    internal static CommandPacket HTtl(SharpValue key, SharpValue field)
    {
        return new CommandPacket(HashCommands._httl, CommandMode.Hash | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field);
    }

    internal static CommandPacket HTtl(SharpValue key, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        return new CommandPacket(HashCommands._httl, CommandMode.Hash | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments(fields.Count.ToBytes())
            .WriteArguments(fields);
    }

    internal static CommandPacket HPTtl(SharpValue key, SharpValue field)
    {
        return new CommandPacket(HashCommands._hpttl, CommandMode.Hash | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field);
    }

    internal static CommandPacket HPTtl(SharpValue key, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        return new CommandPacket(HashCommands._hpttl, CommandMode.Hash | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments(fields.Count.ToBytes())
            .WriteArguments(fields);
    }

    internal static CommandPacket HIncrBy(SharpValue key, SharpValue field, long increment)
    {
        return new CommandPacket(HashCommands._hincrby, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(field)
            .WriteArguments(increment.ToBytes());
    }

    internal static CommandPacket HIncrByFloat(SharpValue key, SharpValue field, double increment)
    {
        return new CommandPacket(HashCommands._hincrbyfloat, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(field)
            .WriteArguments(increment.ToBytes());
    }

    internal static CommandPacket HKeys(SharpValue key)
    {
        return new CommandPacket(HashCommands._hkeys, CommandMode.Hash | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket HLen(SharpValue key)
    {
        return new CommandPacket(HashCommands._hlen, CommandMode.Hash | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket HMGet(SharpValue key, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        return new CommandPacket(HashCommands._hmget, CommandMode.Hash | CommandMode.WithRead | CommandMode.WithClientCaching)
           .WriteKeys(key)
           .WriteArguments(fields);
    }

    internal static CommandPacket HMSet(SharpValue key, ICollection<KeyValuePair<SharpValue, SharpValue>> kv)
    {
        var command = new CommandPacket(HashCommands._hmset, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key);
        foreach (var item in kv)
        {
            _ = command.WriteArguments(item.Key)
                .WriteArguments(item.Value);
        }
        return command;
    }

    internal static CommandPacket HPersist(SharpValue key, SharpValue field)
    {
        return new CommandPacket(HashCommands._hpersist, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field);
    }

    internal static CommandPacket HPersist(SharpValue key, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        return new CommandPacket(HashCommands._hpersist, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(HashCommands._fields)
            .WriteArguments(fields.Count.ToBytes())
            .WriteArguments(fields);
    }

    internal static CommandPacket HRandField(SharpValue key, int count, bool withValues)
    {
        var command = new CommandPacket(HashCommands._hrandfield, CommandMode.Hash | CommandMode.WithRead)
            .WriteKeys(key);

        if (withValues)
        {
            _ = command.WriteArguments(count.ToBytes())
                .WriteArguments("WITHVALUES");
        }
        else
        {
            if (count != 1)
            {
                _ = command.WriteArguments(count.ToBytes());
            }
        }

        return command;
    }

    internal static CommandPacket HSetEx(SharpValue key, FieldExistFlags when, SharpExpiry? expiry, ExpirationFlags expiryFlags, SharpValue field, SharpValue value)
    {
        return new CommandPacket(HashCommands._hsetex, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(when is FieldExistFlags.FNX, nameof(FieldExistFlags.FNX))
            .WriteArguments(when is FieldExistFlags.FXX, nameof(FieldExistFlags.FXX))
            .WriteExpiry(expiry, expiryFlags)
            .WriteArguments(HashCommands._fields)
            .WriteArguments("1")
            .WriteArguments(field)
            .WriteArguments(value);
    }

    internal static CommandPacket HSetEx(SharpValue key, FieldExistFlags when, SharpExpiry? expiry, ExpirationFlags expiryFlags, ICollection<KeyValuePair<SharpValue, SharpValue>> hashEntries)
    {
        var command = new CommandPacket(HashCommands._hsetex, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(when is FieldExistFlags.FNX, nameof(FieldExistFlags.FNX))
            .WriteArguments(when is FieldExistFlags.FXX, nameof(FieldExistFlags.FXX))
            .WriteExpiry(expiry, expiryFlags)
            .WriteArguments(HashCommands._fields)
            .WriteArguments(hashEntries.Count.ToBytes());

        foreach (var kv in hashEntries)
        {
            _ = command
                .WriteArguments(kv.Key)
                .WriteArguments(kv.Value);
        }
        return command;
    }

    internal static CommandPacket HSetNx(SharpValue key, SharpValue field, SharpValue value)
    {
        return new CommandPacket(HashCommands._hsetnx, CommandMode.Hash | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(field)
            .WriteArguments(value);
    }

    internal static CommandPacket HStrLen(SharpValue key, SharpValue field)
    {
        return new CommandPacket(HashCommands._hstrlen, CommandMode.Hash | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(field);
    }

    internal static CommandPacket HVals(SharpValue key)
    {
        return new CommandPacket(HashCommands._hvals, CommandMode.Hash | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket HScan(SharpValue key, ulong cursor, SharpValue? pattern, uint count, bool novalues)
    {
        var command = new CommandPacket(HashCommands._hscan, CommandMode.Hash | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments(cursor.ToBytes());
        if (pattern.HasValue)
        {
            _ = command.WriteArguments("MATCH")
                .WriteArguments(pattern.Value);
        }
        return command
            .WriteArguments("COUNT")
            .WriteArguments(count.ToBytes())
            .WriteArguments(novalues, "NOVALUES");
    }

    internal static CommandPacket HImportPrepare(SharpValue fieldsetName, SharpValue field)
    {
        return new CommandPacket(HashCommands._himport, CommandMode.Hash)
            .WriteCommand(HashCommands._prepare)
            .WriteArguments(fieldsetName)
            .WriteArguments(field);
    }

    internal static CommandPacket HImportPrepare(SharpValue fieldsetName, SharpValueSpan fields)
    {
        if (fields.IsEmpty) throw new NotSupportedException("Please specify at least one field.");
        return new CommandPacket(HashCommands._himport, CommandMode.Hash)
            .WriteCommand(HashCommands._prepare)
            .WriteArguments(fieldsetName)
            .WriteArguments(fields);
    }

    internal static CommandPacket HImportDiscard(SharpValue fieldsetName)
    {
        return new CommandPacket(HashCommands._himport, CommandMode.Hash)
            .WriteCommand(HashCommands._discard)
            .WriteArguments(fieldsetName);
    }

    internal static CommandPacket HImportDiscardAll()
    {
        return new CommandPacket(HashCommands._himport, CommandMode.Hash)
            .WriteCommand(HashCommands._discardall);
    }

    internal static CommandPacket HImportSet(SharpValue key, SharpValue fieldSetName, SharpValue value)
    {
        return new CommandPacket(HashCommands._himport, CommandMode.Hash | CommandMode.WithWrite)
            .WriteCommand(HashCommands._set)
            .WriteKeys(key)
            .WriteArguments(fieldSetName)
            .WriteArguments(value);
    }

    internal static CommandPacket HImportSet(SharpValue key, SharpValue fieldSetName, SharpValueSpan values)
    {
        return new CommandPacket(HashCommands._himport, CommandMode.Hash | CommandMode.WithWrite)
            .WriteCommand(HashCommands._set)
            .WriteKeys(key)
            .WriteArguments(fieldSetName)
            .WriteArguments(values);
    }
}
