using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using static SharpRedis.RedisBitmap;

namespace SharpRedis.Commands;

internal static class BitmapCommands
{
    private static readonly SharpValue _setbit = new("SETBIT", CommandItemCategory.Command);
    private static readonly SharpValue _getbit = new("GETBIT", CommandItemCategory.Command);
    private static readonly SharpValue _bitcount = new("BITCOUNT", CommandItemCategory.Command);
    private static readonly SharpValue _bitfield = new("BITFIELD", CommandItemCategory.Command);
    private static readonly SharpValue _bitfield_ro = new("BITFIELD_RO", CommandItemCategory.Command);
    private static readonly SharpValue _bitop = new("BITOP", CommandItemCategory.Command);
    private static readonly SharpValue _bitpos = new("BITPOS", CommandItemCategory.Command);
    private static readonly SharpValue _bitrangeintarray = new("BITRANGEINTARRAY", CommandItemCategory.Command);

    internal static CommandPacket BitRangeIntArray(SharpValue key, long start, long end, RedisBitmap.BitUnit unit, long? limit, bool zeros)
    {
        var command = new CommandPacket(BitmapCommands._bitrangeintarray, CommandMode.Bitmap | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(start.ToBytes())
            .WriteArguments(end.ToBytes());
        switch (unit)
        {
            case BitUnit.Byte:
                _ = command.WriteArguments("BYTE");
                break;
            case BitUnit.Bit:
                _ = command.WriteArguments("BIT");
                break;
            default:
                break;
        }
        if (limit.HasValue)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(limit.Value.ToBytes());
        }
        return command.WriteArguments(zeros, "ZEROS");
    }

    internal static CommandPacket SetBit(SharpValue key, uint offset, bool value)
    {
        return new CommandPacket(BitmapCommands._setbit, CommandMode.Bitmap | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(offset.ToBytes())
            .WriteArguments(value ? "1" : "0");
    }

    internal static CommandPacket GetBit(SharpValue key, uint offset)
    {
        return new CommandPacket(BitmapCommands._getbit, CommandMode.Bitmap | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(offset.ToBytes());
    }

    internal static CommandPacket BitCount(SharpValue key, long? start, long? end, RedisBitmap.BitUnit unit)
    {
        var command = new CommandPacket(BitmapCommands._bitcount, CommandMode.Bitmap | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);

        if (start.HasValue && end.HasValue)
        {
            _ = command
                .WriteArguments(start.Value.ToBytes())
                .WriteArguments(end.Value.ToBytes());
        }

        return unit switch
        {
            RedisBitmap.BitUnit.Byte => command.WriteArguments("BYTE"),
            RedisBitmap.BitUnit.Bit => command.WriteArguments("BIT"),
            _ => command,
        };
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    internal static CommandPacket BitCount(SharpValue key, global::System.Range range, RedisBitmap.BitUnit unit)
    {
        long start, end;
        if (range.Start.IsFromEnd)
        {
            if (range.Start.Value is 0)
            {
                start = -1;
            }
            else
            {
                start = -range.Start.Value;
            }
        }
        else
        {
            start = range.Start.Value;
        }

        if (range.End.IsFromEnd)
        {
            if (range.End.Value is 0)
            {
                end = -1;
            }
            else
            {
                end = -range.End.Value;
            }
        }
        else
        {
            end = range.End.Value;
        }
        return BitmapCommands.BitCount(key, start, end, unit);
    }
#endif

    internal static CommandPacket BitField(SharpValue key, ReadOnlySpan<BitFieldArg> args)
    {
        if (args.IsEmpty) throw new NotSupportedException("The BITFIELD command parameter cannot be empty.");
        var command = new CommandPacket(BitmapCommands._bitfield, CommandMode.Bitmap | CommandMode.WithWrite)
            .WriteKeys(key);
        foreach (IBitFieldArg arg in args)
        {
            if (arg is null) throw new NullReferenceException("The BITFIELD command is null");
            foreach (var item in arg.Convert()) _ = command.WriteArguments(item);
        }
        return command;
    }

    internal static CommandPacket BitField_Ro(SharpValue key, IBitFieldArg arg)
    {
        if (arg is null) throw new NullReferenceException("The BITFIELD command is null");

        var command = new CommandPacket(BitmapCommands._bitfield_ro, CommandMode.Bitmap | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
        if (arg.Operation != RedisBitmap.BitFieldOperation.Get) throw new NotSupportedException("BITFIELD_RO only supports GET.");
        foreach (var item in arg.Convert()) _ = command.WriteArguments(item);
        return command;
    }

    internal static CommandPacket BitField_Ro(SharpValue key, ReadOnlySpan<BitFieldArg> args)
    {
        if (args.IsEmpty) throw new NotSupportedException("The BITFIELD_RO command parameter cannot be empty.");
        var command = new CommandPacket(BitmapCommands._bitfield_ro, CommandMode.Bitmap | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
        foreach (IBitFieldArg arg in args)
        {
            if (arg is null) throw new NullReferenceException("The BITFIELD command is null");
            if (arg.Operation != RedisBitmap.BitFieldOperation.Get) throw new NotSupportedException("BITFIELD_RO only supports GET.");
            foreach (var item in arg.Convert()) _ = command.WriteArguments(item);
        }
        return command;
    }

    internal static CommandPacket BitOp(BitOperation op, SharpValue destkey, SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        var command = new CommandPacket(BitmapCommands._bitop, CommandMode.Bitmap | CommandMode.WithWrite);
        switch (op)
        {
            case BitOperation.And:
                _ = command.WriteArguments("AND");
                break;
            case BitOperation.Or:
                _ = command.WriteArguments("OR");
                break;
            case BitOperation.Xor:
                _ = command.WriteArguments("XOR");
                break;
            case BitOperation.Not:
                if (keys.Count > 1) throw new NotSupportedException("NOT is a unary operator and only supports a single source bitmap.");
                _ = command.WriteArguments("NOT");
                break;
            case BitOperation.Diff:
                _ = command.WriteArguments("DIFF");
                break;
            case BitOperation.Diff1:
                _ = command.WriteArguments("DIFF1");
                break;
            case BitOperation.AndOr:
                _ = command.WriteArguments("ANDOR");
                break;
            case BitOperation.One:
                _ = command.WriteArguments("ONE");
                break;
            default:
                break;
        }

        return command
            .WriteKeys(destkey)
            .WriteKeys(keys);
    }

    internal static CommandPacket BitPos(SharpValue key, bool bit, long? start, long? end, BitUnit unit)
    {
        var command = new CommandPacket(BitmapCommands._bitpos, CommandMode.Bitmap | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
        if (bit)
        {
            _ = command.WriteArguments("1");
        }
        else
        {
            _ = command.WriteArguments("0");
        }

        if (start.HasValue && end.HasValue)
        {
            _ = command
                .WriteArguments(start.Value.ToBytes())
                .WriteArguments(end.Value.ToBytes());
        }
        else if (start.HasValue && !end.HasValue)
        {
            _ = command
                .WriteArguments(start.Value.ToBytes());
        }
        else if (!start.HasValue && end.HasValue)
        {
            _ = command
                .WriteArguments(0.ToBytes())
                .WriteArguments(end.Value.ToBytes());
        }

        return unit switch
        {
            BitUnit.Byte => command.WriteArguments("BYTE"),
            BitUnit.Bit => command.WriteArguments("BIT"),
            _ => command,
        };
    }
}
