using System;
using System.Buffers.Text;
using System.Text;

namespace SharpRedis.Extensions;

internal static class RESP
{
    internal static void WriteArrayHeader(int count, ref byte[] buffer, ref int pos)
    {
        EnsureCapacity(13, ref buffer, pos);
        WriteHeader(Config.ARRAY, count, buffer, ref pos);
    }

    internal static unsafe void SharpValueToResp(in SharpValue value, ref byte[] buffer, ref int pos,
        byte[] prefix, byte[] suffix, Encoding encoding)
    {
        var text = value.RawString;
        var chars = value.MemoryChar;
        var bytes = value.MemoryByte;
        int length;
        if (text is not null) length = encoding.GetByteCount(text);
        else if (bytes.HasValue) length = bytes.Value.Length;
        else if (chars.HasValue)
        {
#if MODERN_NET
            length = encoding.GetByteCount(chars.Value.Span);
#else
            fixed (char* source = chars.Value.Span)
                length = chars.Value.IsEmpty ? 0 : encoding.GetByteCount(source, chars.Value.Length);
#endif
        }
        else throw new NullReferenceException("There is no available data, so it cannot be converted into byte[].");

        int payloadLength = checked(length + prefix.Length + suffix.Length);
        // '$', up to ten decimal digits, header CRLF, payload, trailing CRLF.
        EnsureCapacity(checked(payloadLength + 15), ref buffer, pos);
        WriteHeader(Config.BULK_STRINGS, payloadLength, buffer, ref pos);
        if (prefix.Length != 0)
        {
            prefix.CopyTo(buffer.AsSpan(pos));
            pos += prefix.Length;
        }
        if (text is not null)
            pos += encoding.GetBytes(text, 0, text.Length, buffer, pos);
        else if (bytes.HasValue)
        {
            bytes.Value.Span.CopyTo(buffer.AsSpan(pos));
            pos += length;
        }
        else if (length != 0)
        {
#if MODERN_NET
            pos += encoding.GetBytes(chars!.Value.Span, buffer.AsSpan(pos, length));
#else
            fixed (char* source = chars.Value.Span)
            fixed (byte* destination = buffer)
                pos += encoding.GetBytes(source, chars.Value.Length, destination + pos, length);
#endif
        }
        if (suffix.Length != 0)
        {
            suffix.CopyTo(buffer.AsSpan(pos));
            pos += suffix.Length;
        }
        buffer[pos++] = Config.CR;
        buffer[pos++] = Config.LF;
    }

    private static void WriteHeader(byte type, int count, byte[] buffer, ref int pos)
    {
        buffer[pos++] = type;
        if (count < 10) buffer[pos++] = (byte)('0' + count);
        else
        {
            Utf8Formatter.TryFormat(count, buffer.AsSpan(pos), out int digits);
            pos += digits;
        }
        buffer[pos++] = Config.CR;
        buffer[pos++] = Config.LF;
    }

    private static void EnsureCapacity(int need, ref byte[] buffer, int pos)
    {
        if (need <= buffer.Length - pos) return;
        var newBuffer = SharedPool.BytesShard.Rent(checked(pos + need));
        Buffer.BlockCopy(buffer, 0, newBuffer, 0, pos);
        SharedPool.BytesShard.Return(buffer);
        buffer = newBuffer;
    }
}
