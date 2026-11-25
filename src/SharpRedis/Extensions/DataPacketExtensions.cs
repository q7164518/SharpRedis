#nullable enable
using SharpRedis.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpRedis.Extensions;

internal static class DataPacketExtensions
{
    // Created only when an aggregate crosses a receive boundary.
    internal sealed class PartialAggregate
    {
        internal RespType _type;
        internal Array _items = null!;
        internal int _index;
        internal RedisResult? _key;
        internal PartialAggregate? _child;
    }

    internal static bool GetRawValue(byte[] buffer, int end, ref int position, Encoding encoding, bool isBytes,
        ref PartialAggregate? partial, out RedisResult result)
    {
        result = default;
        if (partial is not null)
            return ReadItems(buffer, end, ref position, encoding, isBytes, partial._type, partial._items, ref partial, out result);
        if (position == end) return false;

        int start = position + 1;
        var type = (RespType)buffer[position];
        switch (type)
        {
            case RespType.BulkStrings:
            case RespType.BulkErrors:
            case RespType.VerbatimString:
                if (!ReadLength(buffer, end, ref start, out int length)) return false;
                if (length == -1 && type == RespType.BulkStrings)
                {
                    position = start;
                    result = new(type, DBNull.Value, encoding);
                    return true;
                }
                if (length < 0 || (type == RespType.VerbatimString && length < 4))
                    throw new FormatException("Invalid RESP payload length.");
                if (end - start - 2 < length) return false;
                CheckTerminator(buffer, start + length);
                position = start + length + 2;
                if (type == RespType.VerbatimString)
                {
                    if (buffer[start + 3] != ':') throw new FormatException("Invalid verbatim string.");
                    start += 4;
                    length -= 4;
                }
                result = new(type, ReadText(buffer, start, length, encoding, isBytes && type != RespType.BulkErrors), encoding);
                return true;

            case RespType.Null:
                if (end - start < 2) return false;
                CheckTerminator(buffer, start);
                position = start + 2;
                result = new(type, DBNull.Value, encoding);
                return true;

            case RespType.Array:
            case RespType.Set:
            case RespType.Push:
            case RespType.Maps:
                if (!ReadLength(buffer, end, ref start, out int count)) return false;
                position = start;
                if (type == RespType.Set) type = RespType.Array; // Existing result representation.
                if (count <= 0)
                {
                    result = new(type, DBNull.Value, encoding);
                    return true;
                }
                Array items = type == RespType.Maps ? new KeyValuePair<RedisResult, RedisResult>[count] : new RedisResult[count];
                return ReadItems(buffer, end, ref position, encoding, isBytes, type, items, ref partial, out result);

            case RespType.SimpleString:
            case RespType.SimpleErrors:
            case RespType.Boolean:
            case RespType.Integer:
            case RespType.BigNumber:
            case RespType.Double:
                int size = buffer.AsSpan(start, end - start).IndexOf(Config.CR);
                if (size < 0 || end - start - size < 2) return false;
                CheckTerminator(buffer, start + size);
                position = start + size + 2;
                result = new(type, size == 0 && type == RespType.SimpleErrors ? Config.UNKNOWN_ERROR_MSG
                    : ReadText(buffer, start, size, encoding, isBytes && type == RespType.SimpleString), encoding);
                return true;
            default:
                throw new FormatException($"Unsupported RESP type: {(char)buffer[position]}");
        }
    }

    private static object ReadText(byte[] buffer, int start, int length, Encoding encoding, bool isBytes)
        => isBytes ? buffer.AsSpan(start, length).ToArray() : encoding.GetString(buffer, start, length);

    private static bool ReadItems(byte[] buffer, int end, ref int position, Encoding encoding, bool isBytes,
        RespType type, Array items, ref PartialAggregate? partial, out RedisResult result)
    {
        int i = partial?._index ?? 0;
        var key = partial?._key;
        var child = partial?._child;
        if (items is RedisResult[] array)
        {
            for (; i < array.Length; i++)
            {
                if (!GetRawValue(buffer, end, ref position, encoding, isBytes, ref child, out array[i])) break;
            }
        }
        else
        {
            var maps = (KeyValuePair<RedisResult, RedisResult>[])items;
            for (; i < maps.Length; i++)
            {
                if (!key.HasValue)
                {
                    if (!GetRawValue(buffer, end, ref position, encoding, isBytes, ref child, out var nextKey)) break;
                    key = nextKey;
                }
                if (!GetRawValue(buffer, end, ref position, encoding, isBytes, ref child, out var value)) break;
                maps[i] = new(key.Value, value);
                key = null;
            }
        }

        if (i == items.Length)
        {
            partial = null;
            result = new(type, items, encoding);
            return true;
        }
        partial ??= new() { _type = type, _items = items };
        partial._index = i;
        partial._key = key;
        partial._child = child;
        result = default;
        return false;
    }

    private static bool ReadLength(byte[] buffer, int end, ref int start, out int length)
    {
        // The common one-digit length needs no loop or overflow checks.
        if (end - start >= 3 && buffer[start + 1] == Config.CR)
        {
            length = buffer[start] - '0';
            if ((uint)length > 9) throw new FormatException("Invalid RESP length.");
            CheckTerminator(buffer, start + 1);
            start += 3;
            return true;
        }
        length = 0;
        int i = start;
        if (i == end) return false;
        bool negative = buffer[i] == '-';
        if (negative) i++;
        int digits = i;
        for (; i < end; i++)
        {
            int digit = buffer[i] - '0';
            if ((uint)digit <= 9)
            {
                length = checked(length * 10 + digit);
                continue;
            }
            if (i == digits || buffer[i] != Config.CR) throw new FormatException("Invalid RESP length.");
            if (end - i < 2) return false;
            CheckTerminator(buffer, i);
            start = i + 2;
            if (negative) length = -length;
            return true;
        }
        return false;
    }

    private static void CheckTerminator(byte[] buffer, int i)
    {
        if (buffer[i] != Config.CR || buffer[i + 1] != Config.LF)
            throw new FormatException("Invalid RESP terminator.");
    }
}
