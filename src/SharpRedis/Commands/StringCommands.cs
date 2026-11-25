using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static SharpRedis.RedisString;

namespace SharpRedis.Commands;

internal static class StringCommands
{
    private readonly static SharpValue _append = new("APPEND", CommandItemCategory.Command);
    private readonly static SharpValue _decr = new("DECR", CommandItemCategory.Command);
    private readonly static SharpValue _decrBy = new("DECRBY", CommandItemCategory.Command);
    private readonly static SharpValue _delEx = new("DELEX", CommandItemCategory.Command);
    private readonly static SharpValue _digest = new("DIGEST", CommandItemCategory.Command);
    private readonly static SharpValue _set = new("SET", CommandItemCategory.Command);
    private readonly static SharpValue _get = new("GET", CommandItemCategory.Command);
    private readonly static SharpValue _getDel = new("GETDEL", CommandItemCategory.Command);
    private readonly static SharpValue _getEx = new("GETEX", CommandItemCategory.Command);
    private readonly static SharpValue _getRange = new("GETRANGE", CommandItemCategory.Command);
    private readonly static SharpValue _getSet = new("GETSET", CommandItemCategory.Command);
    private readonly static SharpValue _incr = new("INCR", CommandItemCategory.Command);
    private readonly static SharpValue _incrBy = new("INCRBY", CommandItemCategory.Command);
    private readonly static SharpValue _incrByFloat = new("INCRBYFLOAT", CommandItemCategory.Command);
    private readonly static SharpValue _mget = new("MGET", CommandItemCategory.Command);
    private readonly static SharpValue _mset = new("MSET", CommandItemCategory.Command);
    private readonly static SharpValue _msetEx = new("MSETEX", CommandItemCategory.Command);
    private readonly static SharpValue _msetNx = new("MSETNX", CommandItemCategory.Command);
    private readonly static SharpValue _psetEx = new("PSETEX", CommandItemCategory.Command);
    private readonly static SharpValue _setEx = new("SETEX", CommandItemCategory.Command);
    private readonly static SharpValue _setNx = new("SETNX", CommandItemCategory.Command);
    private readonly static SharpValue _setRange = new("SETRANGE", CommandItemCategory.Command);
    private readonly static SharpValue _strLen = new("STRLEN", CommandItemCategory.Command);
    private readonly static SharpValue _subStr = new("SUBSTR", CommandItemCategory.Command);
    private readonly static SharpValue _lcs = new("LCS", CommandItemCategory.Command);
    private readonly static SharpValue _increx = new("INCREX", CommandItemCategory.Command);

    internal static CommandPacket Append(SharpValue key, SharpValue value)
    {
        return new CommandPacket(StringCommands._append, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(value);
    }

    internal static CommandPacket Decr(SharpValue key)
    {
        return new CommandPacket(StringCommands._decr, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key);
    }

    internal static CommandPacket DecrBy(SharpValue key, long decrement)
    {
        return new CommandPacket(StringCommands._decrBy, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(decrement.ToBytes());
    }

    internal static CommandPacket DelEx(SharpValue key, SharpValue condition, ValueCondition flag)
    {
        var command = new CommandPacket(StringCommands._delEx, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key);
        switch (flag)
        {
            case ValueCondition.IFEQ:
                _ = command.WriteArguments(nameof(ValueCondition.IFEQ));
                break;
            case ValueCondition.IFNE:
                _ = command.WriteArguments(nameof(ValueCondition.IFNE));
                break;
            case ValueCondition.IFDEQ:
                _ = command.WriteArguments(nameof(ValueCondition.IFDEQ));
                break;
            case ValueCondition.IFDNE:
                _ = command.WriteArguments(nameof(ValueCondition.IFDNE));
                break;
            default:
                break;
        }

        return command.WriteArguments(condition);
    }

    internal static CommandPacket Digest(SharpValue key)
    {
        return new CommandPacket(StringCommands._digest, CommandMode.String | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket Get(SharpValue key)
    {
        return new CommandPacket(StringCommands._get, CommandMode.String | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket GetDel(SharpValue key)
    {
        return new CommandPacket(StringCommands._getDel, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key);
    }

    internal static CommandPacket GetEx(SharpValue key, SharpExpiry? expiry, ExpirationFlags expiryFlags)
    {
        return new CommandPacket(StringCommands._getEx, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteExpiry(expiry, expiryFlags);
    }

    internal static CommandPacket GetRange(SharpValue key, long start, long end)
    {
        return new CommandPacket(StringCommands._getRange, CommandMode.String | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(start.ToBytes())
            .WriteArguments(end.ToBytes());
    }

    internal static CommandPacket GetSet(SharpValue key, SharpValue value)
    {
        return new CommandPacket(StringCommands._getSet, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(value);
    }

    internal static CommandPacket Incr(SharpValue key)
    {
        return new CommandPacket(StringCommands._incr, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key);
    }

    internal static CommandPacket IncrBy(SharpValue key, long increment)
    {
        return new CommandPacket(StringCommands._incrBy, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(increment.ToBytes());
    }

    internal static CommandPacket IncrByFloat(SharpValue key, double increment)
    {
        return new CommandPacket(StringCommands._incrByFloat, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(increment.ToBytes());
    }

    internal static CommandPacket MGet(ReadOnlySpan<SharpValue> keys)
    {
        return new CommandPacket(StringCommands._mget, CommandMode.String | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(keys);
    }

    internal static CommandPacket MSet(IEnumerable<KeyValuePair<SharpValue, SharpValue>> kvList)
    {
        var command = new CommandPacket(StringCommands._mset, CommandMode.String | CommandMode.WithWrite);
        foreach (var item in kvList)
        {
            command = command
                .WriteKeys(item.Key)
                .WriteArguments(item.Value);
        }
        return command;
    }

    internal static CommandPacket MSetEx(IEnumerable<KeyValuePair<SharpValue, SharpValue>> kvList, SharpExpiry? expiry, ExpirationFlags expiryFlags, StringExist when)
    {
        var command = new CommandPacket(StringCommands._msetEx, CommandMode.String | CommandMode.WithWrite)
            .WriteArguments(kvList.Count().ToBytes());
        foreach (var item in kvList)
        {
            _ = command
                .WriteKeys(item.Key)
                .WriteArguments(item.Value);
        }
        return command.WriteNxXx(when)
            .WriteExpiry(expiry, expiryFlags);
    }

    internal static CommandPacket MSetNx(IEnumerable<KeyValuePair<SharpValue, SharpValue>> kvList)
    {
        var command = new CommandPacket(StringCommands._msetNx, CommandMode.String | CommandMode.WithWrite);
        foreach (var item in kvList)
        {
            command = command
                .WriteKeys(item.Key)
                .WriteArguments(item.Value);
        }
        return command;
    }

    internal static CommandPacket PSetEx(SharpValue key, SharpValue value, long milliseconds)
    {
        return new CommandPacket(StringCommands._psetEx, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(milliseconds.ToBytes())
            .WriteArguments(value);
    }

    internal static CommandPacket SetEx(SharpValue key, SharpValue value, long seconds)
    {
        return new CommandPacket(StringCommands._setEx, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(seconds.ToBytes())
            .WriteArguments(value);
    }

    internal static CommandPacket SetNx(SharpValue key, SharpValue value)
    {
        return new CommandPacket(StringCommands._setNx, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(value);
    }

    internal static CommandPacket SetRange(SharpValue key, long offset, SharpValue value)
    {
        return new CommandPacket(StringCommands._setRange, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(offset.ToBytes())
            .WriteArguments(value);
    }

    internal static CommandPacket StrLen(SharpValue key)
    {
        return new CommandPacket(StringCommands._strLen, CommandMode.String | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket SubStr(SharpValue key, long start, long end)
    {
        return new CommandPacket(StringCommands._subStr, CommandMode.String | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(start.ToBytes())
            .WriteArguments(end.ToBytes());
    }

    internal static CommandPacket Set(
        SharpValue key,
        SharpValue value,
        StringExist when = StringExist.None,
        ValueCondition? conditionFlag = null,
        SharpValue? condition = null,
        bool get = false,
        SharpExpiry? expiry = null,
        ExpirationFlags expiryFlags = ExpirationFlags.EX)
    {
        var command = new CommandPacket(StringCommands._set, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(value)
            .WriteNxXx(when);
        if (conditionFlag.HasValue && condition.HasValue && condition.Value.HasValue)
        {
            switch (conditionFlag.Value)
            {
                case ValueCondition.IFEQ:
                    _ = command.WriteArguments(nameof(ValueCondition.IFEQ));
                    break;
                case ValueCondition.IFNE:
                    _ = command.WriteArguments(nameof(ValueCondition.IFNE));
                    break;
                case ValueCondition.IFDEQ:
                    _ = command.WriteArguments(nameof(ValueCondition.IFDEQ));
                    break;
                case ValueCondition.IFDNE:
                    _ = command.WriteArguments(nameof(ValueCondition.IFDNE));
                    break;
                default:
                    break;
            }
            _ = command.WriteArguments(condition.Value);
        }
        if (get) _ = command.WriteArguments("GET");
        return command.WriteExpiry(expiry, expiryFlags);
    }

    internal static CommandPacket Lcs(SharpValue key1, SharpValue key2, bool len = false, bool idx = false, long? minMatchLen = null, bool withMatchLen = false)
    {
        var command = new CommandPacket(StringCommands._lcs, CommandMode.String | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key1)
            .WriteKeys(key2)
            .WriteArguments(len, "LEN")
            .WriteArguments(idx, "IDX");
        if (minMatchLen.HasValue)
        {
            _ = command.WriteArguments("MINMATCHLEN")
                .WriteArguments(minMatchLen.Value.ToBytes());
        }

        return command
            .WriteArguments(withMatchLen, "WITHMATCHLEN");
    }

    internal static CommandPacket IncrEx(
        SharpValue key,
        IncrExNumber? increment,
        IncrExNumber? lowerbound,
        IncrExNumber? upperbound,
        bool saturate,
        SharpExpiry? expiry,
        ExpirationFlags expiryFlags,
        bool enx)
    {
        var command = new CommandPacket(StringCommands._increx, CommandMode.String | CommandMode.WithWrite)
            .WriteKeys(key);
        if (increment.HasValue)
        {
            _ = increment.Value.IsDouble switch
            {
                false => command.WriteArguments("BYINT"),
                _ => command.WriteArguments("BYFLOAT"),
            };
            _ = command.WriteArguments(increment.Value.Bytes);
        }

        if (lowerbound.HasValue)
        {
            _ = command
                .WriteArguments("LBOUND")
                .WriteArguments(lowerbound.Value.Bytes);
        }
        if (upperbound.HasValue)
        {
            _ = command
                .WriteArguments("UBOUND")
                .WriteArguments(upperbound.Value.Bytes);
        }

        return command
            .WriteArguments(saturate, "SATURATE")
            .WriteExpiry(expiry, expiryFlags)
            .WriteArguments(enx, "ENX");
    }

    private static CommandPacket WriteNxXx(this CommandPacket command, StringExist when)
    {
        if (when != StringExist.None)
        {
            return command
                .WriteArguments(when == StringExist.NX, nameof(StringExist.NX))
                .WriteArguments(when == StringExist.XX, nameof(StringExist.XX));
        }
        return command;
    }
}
