using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;

namespace SharpRedis.Commands;

internal static class ArraysCommands
{
    private static readonly SharpValue _arset = new("ARSET", CommandItemCategory.Command);
    private static readonly SharpValue _arcount = new("ARCOUNT", CommandItemCategory.Command);
    private static readonly SharpValue _ardel = new("ARDEL", CommandItemCategory.Command);
    private static readonly SharpValue _ardelrange = new("ARDELRANGE", CommandItemCategory.Command);
    private static readonly SharpValue _arget = new("ARGET", CommandItemCategory.Command);
    private static readonly SharpValue _argetrange = new("ARGETRANGE", CommandItemCategory.Command);
    private static readonly SharpValue _argrep = new("ARGREP", CommandItemCategory.Command);
    private static readonly SharpValue _arinsert = new("ARINSERT", CommandItemCategory.Command);
    private static readonly SharpValue _arlastitems = new("ARLASTITEMS", CommandItemCategory.Command);
    private static readonly SharpValue _arlen = new("ARLEN", CommandItemCategory.Command);
    private static readonly SharpValue _armget = new("ARMGET", CommandItemCategory.Command);
    private static readonly SharpValue _armset = new("ARMSET", CommandItemCategory.Command);
    private static readonly SharpValue _arnext = new("ARNEXT", CommandItemCategory.Command);
    private static readonly SharpValue _arop = new("AROP", CommandItemCategory.Command);
    private static readonly SharpValue _arring = new("ARRING", CommandItemCategory.Command);
    private static readonly SharpValue _arscan = new("ARSCAN", CommandItemCategory.Command);
    private static readonly SharpValue _arseek = new("ARSEEK", CommandItemCategory.Command);
    private static readonly SharpValue _arinfo = new("ARINFO", CommandItemCategory.Command);

    internal static CommandPacket ArSet(SharpValue key, long index, SharpValue value)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
#endif
        return new CommandPacket(ArraysCommands._arset, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(index.ToBytes())
            .WriteArguments(value);
    }

    internal static CommandPacket ArSet(SharpValue key, long index, SharpValueSpan values)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
#endif
        if (values.IsEmpty) throw new ArgumentException("Please ensure that at least one value is inserted.", nameof(values));
        var command = new CommandPacket(ArraysCommands._arset, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(index.ToBytes());
        foreach (var value in values)
        {
            _ = command.WriteArguments(value);
        }
        return command;
    }

    internal static CommandPacket ArCount(SharpValue key)
    {
        return new CommandPacket(ArraysCommands._arcount, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket ArDel(SharpValue key, long index)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
#endif
        return new CommandPacket(ArraysCommands._ardel, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(index.ToBytes());
    }

    internal static CommandPacket ArDel(SharpValue key, ReadOnlySpan<long> indexs)
    {
        if (indexs.IsEmpty) throw new ArgumentException("Please specify at least one index to be deleted.", nameof(indexs));
        var command = new CommandPacket(ArraysCommands._ardel, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key);
        foreach (var index in indexs)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
#endif
            _ = command.WriteArguments(index.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ArDelRange(SharpValue key, long start, long end)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(end);
#else
        if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
        if (end < 0) throw new ArgumentOutOfRangeException(nameof(end));
#endif
        return new CommandPacket(ArraysCommands._ardelrange, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(start.ToBytes())
            .WriteArguments(end.ToBytes());
    }

    internal static CommandPacket ArDelRange(SharpValue key, RedisArray.Range range)
    {
        return new CommandPacket(ArraysCommands._ardelrange, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(range.Start.ToBytes())
            .WriteArguments(range.End.ToBytes());
    }

    internal static CommandPacket ArDelRange(SharpValue key, ReadOnlySpan<RedisArray.Range> ranges)
    {
        if (ranges.IsEmpty) throw new ArgumentException("Please specify at least one set of range indexes", nameof(ranges));
        var command = new CommandPacket(ArraysCommands._ardelrange, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key);
        foreach (var range in ranges)
        {
            _ = command
                .WriteArguments(range.Start.ToBytes())
                .WriteArguments(range.End.ToBytes());
        }
        return command;
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    internal static CommandPacket ArDelRange(SharpValue key, Range range)
    {
        return new CommandPacket(ArraysCommands._ardelrange, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(range.Start.Value.ToBytes())
            .WriteArguments(range.End.Value.ToBytes());
    }

    internal static CommandPacket ArDelRange(SharpValue key, ReadOnlySpan<Range> ranges)
    {
        if (ranges.IsEmpty) throw new ArgumentException("Please specify at least one set of range indexes", nameof(ranges));
        var command = new CommandPacket(ArraysCommands._ardelrange, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key);
        foreach (var range in ranges)
        {
            _ = command
                .WriteArguments(range.Start.Value.ToBytes())
                .WriteArguments(range.End.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ArGetRange(SharpValue key, System.Range range)
    {
        return new CommandPacket(ArraysCommands._argetrange, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(range.Start.Value.ToBytes())
            .WriteArguments(range.End.Value.ToBytes());
    }
#endif

    internal static CommandPacket ArGet(SharpValue key, long index)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
#endif
        return new CommandPacket(ArraysCommands._arget, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(index.ToBytes());
    }

    internal static CommandPacket ArGetRange(SharpValue key, long start, long end)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(end);
#else
        if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
        if (end < 0) throw new ArgumentOutOfRangeException(nameof(end));
#endif
        return new CommandPacket(ArraysCommands._argetrange, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(start.ToBytes())
            .WriteArguments(end.ToBytes());
    }

    internal static CommandPacket ArGetRange(SharpValue key, RedisArray.Range range)
    {
        return new CommandPacket(ArraysCommands._argetrange, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(range.Start.ToBytes())
            .WriteArguments(range.End.ToBytes());
    }

    internal static CommandPacket ArGrep(
        SharpValue key,
        RedisArray.GrepIndex start,
        RedisArray.GrepIndex end,
        RedisArray.GrepConditions conditions,
        long? limit,
        bool withValues,
        bool noCase)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(conditions);
#else
        if (conditions is null) throw new ArgumentNullException(nameof(conditions));
#endif
        if (limit.HasValue && limit.Value < 0) throw new ArgumentOutOfRangeException(nameof(limit));
        var command = ArraysCommands.InitArGrep(key: key, start: start, end: end);
        foreach (var condition in conditions.Conditions)
        {
            var keyword = condition.Type switch
            {
                RedisArray.GrepPredicate.Exact => "EXACT",
                RedisArray.GrepPredicate.Match => "MATCH",
                RedisArray.GrepPredicate.Glob => "GLOB",
                RedisArray.GrepPredicate.Re => "RE",
                _ => throw new ArgumentOutOfRangeException(nameof(conditions)),
            };
            switch (condition.Operator)
            {
                case RedisArray.GrepOperator.Or:
                    _ = command.WriteArguments("OR");
                    break;
                case RedisArray.GrepOperator.And:
                    _ = command.WriteArguments("AND");
                    break;
                default:
                    break;
            }
            _ = command
                .WriteArguments(keyword)
                .WriteArguments(condition.Pattern);
        }
        if (limit.HasValue) _ = command.WriteArguments("LIMIT").WriteArguments(limit.Value.ToBytes());
        return command
            .WriteArguments(withValues, "WITHVALUES")
            .WriteArguments(noCase, "NOCASE");
    }

    internal static CommandPacket ArGrep(
        SharpValue key,
        RedisArray.GrepIndex start,
        RedisArray.GrepIndex end,
        RedisArray.GrepCondition condition,
        long? limit,
        bool withValues,
        bool noCase)
    {
        if (limit.HasValue && limit.Value < 0) throw new ArgumentOutOfRangeException(nameof(limit));
        var command = ArraysCommands.InitArGrep(key: key, start: start, end: end);
        var keyword = condition.Type switch
        {
            RedisArray.GrepPredicate.Exact => "EXACT",
            RedisArray.GrepPredicate.Match => "MATCH",
            RedisArray.GrepPredicate.Glob => "GLOB",
            RedisArray.GrepPredicate.Re => "RE",
            _ => throw new ArgumentOutOfRangeException(nameof(condition)),
        };
        switch (condition.Operator)
        {
            case RedisArray.GrepOperator.Or:
                _ = command.WriteArguments("OR");
                break;
            case RedisArray.GrepOperator.And:
                _ = command.WriteArguments("AND");
                break;
            default:
                break;
        }
        _ = command
            .WriteArguments(keyword)
            .WriteArguments(condition.Pattern);
        if (limit.HasValue) _ = command.WriteArguments("LIMIT").WriteArguments(limit.Value.ToBytes());
        return command
            .WriteArguments(withValues, "WITHVALUES")
            .WriteArguments(noCase, "NOCASE");
    }

    private static SharpValue GrepBound(RedisArray.GrepIndex index) => index.Symbol switch
    {
        '-' => "-",
        '+' => "+",
        _ => index.Index.ToBytes(),
    };

    private static CommandPacket InitArGrep(
        SharpValue key,
        RedisArray.GrepIndex start,
        RedisArray.GrepIndex end)
    {
        return new CommandPacket(ArraysCommands._argrep, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(ArraysCommands.GrepBound(start))
            .WriteArguments(ArraysCommands.GrepBound(end));
    }

    internal static CommandPacket ArInsert(SharpValue key, SharpValue value)
    {
        return new CommandPacket(ArraysCommands._arinsert, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(value);
    }

    internal static CommandPacket ArInsert(SharpValue key, SharpValueSpan values)
    {
        if (values.IsEmpty) throw new ArgumentException("Please specify at least one value to be inserted.", nameof(values));
        return new CommandPacket(ArraysCommands._arinsert, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(values);
    }

    internal static CommandPacket ArLastItems(SharpValue key, long count, bool rev)
    {
        return new CommandPacket(ArraysCommands._arlastitems, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(count.ToBytes())
            .WriteArguments(rev, "REV");
    }

    internal static CommandPacket ArLen(SharpValue key)
    {
        return new CommandPacket(ArraysCommands._arlen, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket ArMGet(SharpValue key, ReadOnlySpan<long> indexs)
    {
        if (indexs.IsEmpty) throw new ArgumentException("Please specify at least one index subscript.", nameof(indexs));
        var command = new CommandPacket(ArraysCommands._armget, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
        foreach (var index in indexs)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
#endif
            _ = command.WriteArguments(index.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ArMSet(SharpValue key, ReadOnlySpan<(long index, SharpValue value)> indexValues)
    {
        if (indexValues.IsEmpty) throw new ArgumentException("Please set at least one pair of index-value.", nameof(indexValues));
        var command = new CommandPacket(ArraysCommands._armset, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key);
        foreach (var (index, value) in indexValues)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
#endif
            _ = command
                .WriteArguments(index.ToBytes())
                .WriteArguments(value);
        }
        return command;
    }

    internal static CommandPacket ArMSet(SharpValue key, ReadOnlySpan<KeyValuePair<long, SharpValue>> indexValues)
    {
        if (indexValues.IsEmpty) throw new ArgumentException("Please set at least one pair of index-value.", nameof(indexValues));
        var command = new CommandPacket(ArraysCommands._armset, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key);
        foreach (var item in indexValues)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(item.Key);
#else
            if (item.Key < 0) throw new ArgumentOutOfRangeException("index");
#endif
            _ = command
                .WriteArguments(item.Key.ToBytes())
                .WriteArguments(item.Value);
        }
        return command;
    }

    internal static CommandPacket ArNext(SharpValue key)
    {
        return new CommandPacket(ArraysCommands._arnext, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket ArOp(SharpValue key, long start, long end, RedisArray.AggregateOp op, SharpValue matchValue)
    {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(start);
            ArgumentOutOfRangeException.ThrowIfNegative(end);
#else
        if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
        if (end < 0) throw new ArgumentOutOfRangeException(nameof(end));
#endif
        if (op == RedisArray.AggregateOp.Match && matchValue.IsNullOrEmpty) throw new ArgumentException("The matching aggregation value cannot be empty.", nameof(matchValue));
        return new CommandPacket(ArraysCommands._arop, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(start.ToBytes())
            .WriteArguments(end.ToBytes())
            .AppendArOp(op, matchValue);
    }

    internal static CommandPacket ArOp(SharpValue key, RedisArray.Range range, RedisArray.AggregateOp op, SharpValue matchValue)
    {
        if (op == RedisArray.AggregateOp.Match && matchValue.IsNullOrEmpty) throw new ArgumentException("The matching aggregation value cannot be empty.", nameof(matchValue));
        return new CommandPacket(ArraysCommands._arop, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(range.Start.ToBytes())
            .WriteArguments(range.End.ToBytes())
            .AppendArOp(op, matchValue);
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    internal static CommandPacket ArOp(SharpValue key, System.Range range, RedisArray.AggregateOp op, SharpValue matchValue)
    {
        if (op == RedisArray.AggregateOp.Match && matchValue.IsNullOrEmpty) throw new ArgumentException("The matching aggregation value cannot be empty.", nameof(matchValue));
        return new CommandPacket(ArraysCommands._arop, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(range.Start.Value.ToBytes())
            .WriteArguments(range.End.Value.ToBytes())
            .AppendArOp(op, matchValue);
    }

    internal static CommandPacket ArScan(SharpValue key, System.Range range, long? limit)
    {
        var command = new CommandPacket(ArraysCommands._arscan, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(range.Start.Value.ToBytes())
            .WriteArguments(range.End.Value.ToBytes());
        if (limit.HasValue)
        {
            return command
                .WriteArguments("LIMIT")
                .WriteArguments(limit.Value.ToBytes());
        }
        return command;
    }
#endif

    private static CommandPacket AppendArOp(this CommandPacket command, RedisArray.AggregateOp op, SharpValue matchValue)
    {
        return op switch
        {
            RedisArray.AggregateOp.Sum => command.WriteArguments("SUM"),
            RedisArray.AggregateOp.Min => command.WriteArguments("MIN"),
            RedisArray.AggregateOp.Max => command.WriteArguments("MAX"),
            RedisArray.AggregateOp.And => command.WriteArguments("AND"),
            RedisArray.AggregateOp.Or => command.WriteArguments("OR"),
            RedisArray.AggregateOp.Xor => command.WriteArguments("XOR"),
            RedisArray.AggregateOp.Match => command.WriteArguments("MATCH").WriteArguments(matchValue),
            RedisArray.AggregateOp.Used => command.WriteArguments("USED"),
            _ => throw new ArgumentException("Unsupported aggregation operation", nameof(op)),
        };
    }

    internal static CommandPacket ArRing(SharpValue key, long size, SharpValue value)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(size);
#else
        if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
#endif
        return new CommandPacket(ArraysCommands._arring, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(size.ToBytes())
            .WriteArguments(value);
    }

    internal static CommandPacket ArRing(SharpValue key, long size, SharpValueSpan values)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(size);
#else
        if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
#endif
        return new CommandPacket(ArraysCommands._arring, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(size.ToBytes())
            .WriteArguments(values);
    }

    internal static CommandPacket ArScan(SharpValue key, long start, long end, long? limit)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(end);
#else
        if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
        if (end < 0) throw new ArgumentOutOfRangeException(nameof(end));
#endif
        var command = new CommandPacket(ArraysCommands._arscan, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(start.ToBytes())
            .WriteArguments(end.ToBytes());
        if (limit.HasValue)
        {
            return command
                .WriteArguments("LIMIT")
                .WriteArguments(limit.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ArScan(SharpValue key, RedisArray.Range range, long? limit)
    {
        var command = new CommandPacket(ArraysCommands._arscan, CommandMode.Array | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(range.Start.ToBytes())
            .WriteArguments(range.End.ToBytes());
        if (limit.HasValue)
        {
            return command
                .WriteArguments("LIMIT")
                .WriteArguments(limit.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ArSeek(SharpValue key, long index)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
#endif
        return new CommandPacket(ArraysCommands._arseek, CommandMode.Array | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(index.ToBytes());
    }

    internal static CommandPacket ArInfo(SharpValue key, bool full)
    {
        return new CommandPacket(ArraysCommands._arinfo, CommandMode.Array | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments(full, "FULL");
    }
}
