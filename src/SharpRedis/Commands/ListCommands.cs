using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using static SharpRedis.RedisList;

namespace SharpRedis.Commands;

internal static class ListCommands
{
    private static readonly SharpValue _linsert = new("LINSERT", CommandItemCategory.Command);
    private static readonly SharpValue _lindex = new("LINDEX", CommandItemCategory.Command);
    private static readonly SharpValue _llen = new("LLEN", CommandItemCategory.Command);
    private static readonly SharpValue _lpush = new("LPUSH", CommandItemCategory.Command);
    private static readonly SharpValue _rpush = new("RPUSH", CommandItemCategory.Command);
    private static readonly SharpValue _lmove = new("LMOVE", CommandItemCategory.Command);
    private static readonly SharpValue _lmpop = new("LMPOP", CommandItemCategory.Command);
    private static readonly SharpValue _lpop = new("LPOP", CommandItemCategory.Command);
    private static readonly SharpValue _lpos = new("LPOS", CommandItemCategory.Command);
    private static readonly SharpValue _lpushx = new("LPUSHX", CommandItemCategory.Command);
    private static readonly SharpValue _lrange = new("LRANGE", CommandItemCategory.Command);
    private static readonly SharpValue _lrem = new("LREM", CommandItemCategory.Command);
    private static readonly SharpValue _lset = new("LSET", CommandItemCategory.Command);
    private static readonly SharpValue _ltrim = new("LTRIM", CommandItemCategory.Command);
    private static readonly SharpValue _rpop = new("RPOP", CommandItemCategory.Command);
    private static readonly SharpValue _rpoplpush = new("RPOPLPUSH", CommandItemCategory.Command);
    private static readonly SharpValue _rpushx = new("RPUSHX", CommandItemCategory.Command);
    private static readonly SharpValue _blmove = new("BLMOVE", CommandItemCategory.Command);
    private static readonly SharpValue _blmpop = new("BLMPOP", CommandItemCategory.Command);
    private static readonly SharpValue _blpop = new("BLPOP", CommandItemCategory.Command);
    private static readonly SharpValue _brpop = new("BRPOP", CommandItemCategory.Command);
    private static readonly SharpValue _brpoplpush = new("BRPOPLPUSH", CommandItemCategory.Command);
    private static readonly SharpValue _lmovem = new("LMOVEM", CommandItemCategory.Command);
    private static readonly SharpValue _blmovem = new("BLMOVEM", CommandItemCategory.Command);

    internal static CommandPacket LIndex(SharpValue key, long index)
    {
        return new CommandPacket(ListCommands._lindex, CommandMode.List | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(index.ToBytes());
    }

    internal static CommandPacket LInsert(SharpValue key, InsertPosition position, SharpValue pivot, SharpValue element)
    {
        return new CommandPacket(ListCommands._linsert, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(position == InsertPosition.Before ? "BEFORE" : "AFTER")
            .WriteArguments(pivot)
            .WriteArguments(element);
    }

    internal static CommandPacket LLen(SharpValue key)
    {
        return new CommandPacket(ListCommands._llen, CommandMode.List | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket LPush(SharpValue key, SharpValue element)
    {
        return new CommandPacket(ListCommands._lpush, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(element);
    }

    internal static CommandPacket LPush(SharpValue key, SharpValueSpan elements, bool reverse)
    {
        if (elements.IsEmpty) throw new NotSupportedException("Please specify at least one element.");
        var command = new CommandPacket(ListCommands._lpush, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key);
        if (!reverse)
        {
            return command.WriteArguments(elements);
        }
        else
        {
            var length = elements.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                _ = command.WriteArguments(elements[i]);
            }
            return command;
        }
    }

    internal static CommandPacket RPush(SharpValue key, SharpValue element)
    {
        return new CommandPacket(ListCommands._rpush, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(element);
    }

    internal static CommandPacket RPush(SharpValue key, SharpValueSpan elements, bool reverse)
    {
        if (elements.IsEmpty) throw new NotSupportedException("Please specify at least one element.");
        var command = new CommandPacket(ListCommands._rpush, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key);
        if (!reverse)
        {
            return command.WriteArguments(elements);
        }
        else
        {
            var length = elements.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                _ = command.WriteArguments(elements[i]);
            }
            return command;
        }
    }

    internal static CommandPacket LMove(SharpValue sourceKey, SharpValue destinationKey, WherePosition whereFrom, WherePosition whereTo)
    {
        return new CommandPacket(ListCommands._lmove, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(sourceKey)
            .WriteKeys(destinationKey)
            .WriteArguments(whereFrom == WherePosition.Left ? "LEFT" : "RIGHT")
            .WriteArguments(whereTo == WherePosition.Left ? "LEFT" : "RIGHT");
    }

    internal static CommandPacket LMPop(SharpValueSpan keys, WherePosition where, uint count)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(ListCommands._lmpop, CommandMode.List | CommandMode.WithWrite)
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys)
            .WriteArguments(where == WherePosition.Left ? "LEFT" : "RIGHT")
            .WriteArguments("COUNT")
            .WriteArguments(count.ToBytes());
    }

    internal static CommandPacket LPop(SharpValue key, uint? count = null)
    {
        var command = new CommandPacket(ListCommands._lpop, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key);
        if (!count.HasValue) return command;
        return command.WriteArguments(count.Value.ToBytes());
    }

    internal static CommandPacket LPos(SharpValue key, SharpValue element, long? rank = null, long? count = null, long? maxlen = null)
    {
        var command = new CommandPacket(ListCommands._lpos, CommandMode.List | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(element);
        if (rank.HasValue)
        {
            _ = command.WriteArguments("RANK").WriteArguments(rank.Value.ToBytes());
        }
        if (count.HasValue)
        {
            _ = command.WriteArguments("COUNT").WriteArguments(count.Value.ToBytes());
        }
        if (maxlen.HasValue)
        {
            _ = command.WriteArguments("MAXLEN").WriteArguments(maxlen.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket LPushX(SharpValue key, SharpValue element)
    {
        return new CommandPacket(ListCommands._lpushx, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(element);
    }

    internal static CommandPacket LPushX(SharpValue key, SharpValueSpan elements, bool reverse)
    {
        if (elements.IsEmpty) throw new NotSupportedException("Please specify at least one element.");
        var command = new CommandPacket(ListCommands._lpushx, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key);
        if (!reverse)
        {
            return command.WriteArguments(elements);
        }
        else
        {
            var length = elements.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                _ = command.WriteArguments(elements[i]);
            }
            return command;
        }
    }

    internal static CommandPacket LRange(SharpValue key, long start, long stop)
    {
        return new CommandPacket(ListCommands._lrange, CommandMode.List | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(start.ToBytes())
            .WriteArguments(stop.ToBytes());
    }

    internal static CommandPacket LRem(SharpValue key, long count, SharpValue element)
    {
        return new CommandPacket(ListCommands._lrem, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(count.ToBytes())
            .WriteArguments(element);
    }

    internal static CommandPacket LSet(SharpValue key, long index, SharpValue element)
    {
        return new CommandPacket(ListCommands._lset, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(index.ToBytes())
            .WriteArguments(element);
    }

    internal static CommandPacket LTrim(SharpValue key, long start, long stop)
    {
        return new CommandPacket(ListCommands._ltrim, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(start.ToBytes())
            .WriteArguments(stop.ToBytes());
    }

    internal static CommandPacket RPop(SharpValue key, uint? count = null)
    {
        var command = new CommandPacket(ListCommands._rpop, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key);
        if (!count.HasValue) return command;
        return command.WriteArguments(count.Value.ToBytes());
    }

    internal static CommandPacket RPopLPush(SharpValue sourceKey, SharpValue destinationKey)
    {
        return new CommandPacket(ListCommands._rpoplpush, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(sourceKey)
            .WriteKeys(destinationKey);
    }

    internal static CommandPacket RPushX(SharpValue key, SharpValue element)
    {
        return new CommandPacket(ListCommands._rpushx, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(element);
    }

    internal static CommandPacket RPushX(SharpValue key, SharpValueSpan elements, bool reverse)
    {
        if (elements.IsEmpty) throw new NotSupportedException("Please specify at least one element.");
        var command = new CommandPacket(ListCommands._rpushx, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(key);
        if (!reverse)
        {
            return command.WriteArguments(elements);
        }
        else
        {
            var length = elements.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                _ = command.WriteArguments(elements[i]);
            }
            return command;
        }
    }

    internal static CommandPacket BLMove(SharpValue sourceKey, SharpValue destinationKey, WherePosition whereFrom, WherePosition whereTo, double timeout)
    {
        return new CommandPacket(ListCommands._blmove, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(sourceKey)
            .WriteKeys(destinationKey)
            .WriteArguments(whereFrom == WherePosition.Left ? "LEFT" : "RIGHT")
            .WriteArguments(whereTo == WherePosition.Left ? "LEFT" : "RIGHT")
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BLMPop(SharpValueSpan keys, WherePosition where, uint count, double timeout)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(ListCommands._blmpop, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteArguments(timeout.ToBytes())
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys)
            .WriteArguments(where == WherePosition.Left ? "LEFT" : "RIGHT")
            .WriteArguments("COUNT")
            .WriteArguments(count.ToBytes());
    }

    internal static CommandPacket BLPop(SharpValue key, int timeout)
    {
        return new CommandPacket(ListCommands._blpop, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(key)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BLPop(SharpValue key, double timeout)
    {
        return new CommandPacket(ListCommands._blpop, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(key)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BLPop(SharpValueSpan keys, int timeout)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(ListCommands._blpop, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(keys)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BLPop(SharpValueSpan keys, double timeout)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(ListCommands._blpop, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(keys)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BRPop(SharpValue key, int timeout)
    {
        return new CommandPacket(ListCommands._brpop, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(key)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BRPop(SharpValue key, double timeout)
    {
        return new CommandPacket(ListCommands._brpop, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(key)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BRPop(SharpValueSpan keys, int timeout)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(ListCommands._brpop, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(keys)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BRPop(SharpValueSpan keys, double timeout)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(ListCommands._brpop, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(keys)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BRPopLPush(SharpValue source, SharpValue destination, int timeout)
    {
        return new CommandPacket(ListCommands._brpoplpush, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(source)
            .WriteKeys(destination)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BRPopLPush(SharpValue source, SharpValue destination, double timeout)
    {
        return new CommandPacket(ListCommands._brpoplpush, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(source)
            .WriteKeys(destination)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket LMoveM(
        SharpValue source,
        SharpValue destination,
        WherePosition whereFrom,
        WherePosition whereTo,
        uint? count,
        MoveCountMode? countMode,
        MoveReplyMode? replyMode)
    {
        var command = new CommandPacket(ListCommands._lmovem, CommandMode.List | CommandMode.WithWrite)
            .WriteKeys(source)
            .WriteKeys(destination)
            .WriteArguments(whereFrom == WherePosition.Left ? "LEFT" : "RIGHT")
            .WriteArguments(whereTo == WherePosition.Left ? "LEFT" : "RIGHT");
        if (count.HasValue && countMode.HasValue && replyMode.HasValue)
        {
            _ = command
                .WriteArguments(countMode.Value == MoveCountMode.Count ? "COUNT" : "EXACTLY")
                .WriteArguments(count.Value.ToBytes())
                .WriteArguments(replyMode.Value == MoveReplyMode.Bulk ? "BULK" : "OBO");
        }
        return command;
    }

    internal static CommandPacket BLMoveM(
        SharpValue source,
        SharpValue destination,
        WherePosition whereFrom,
        WherePosition whereTo,
        double timeout,
        uint? count,
        MoveCountMode? countMode,
        MoveReplyMode? replyMode)
    {
        var command = new CommandPacket(ListCommands._blmovem, CommandMode.List | CommandMode.WithWrite | CommandMode.WithBlocking)
            .WriteKeys(source)
            .WriteKeys(destination)
            .WriteArguments(whereFrom == WherePosition.Left ? "LEFT" : "RIGHT")
            .WriteArguments(whereTo == WherePosition.Left ? "LEFT" : "RIGHT")
            .WriteArguments(timeout.ToBytes());
        if (count.HasValue && countMode.HasValue && replyMode.HasValue)
        {
            _ = command
                .WriteArguments(countMode.Value == MoveCountMode.Count ? "COUNT" : "EXACTLY")
                .WriteArguments(count.Value.ToBytes())
                .WriteArguments(replyMode.Value == MoveReplyMode.Bulk ? "BULK" : "OBO");
        }
        return command;
    }
}
