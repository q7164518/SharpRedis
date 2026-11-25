using System;
using System.Buffers.Text;
using System.Numerics;
using System.Text;

namespace SharpRedis.Extensions;

internal static class NumberExtensions
{
    internal static Memory<byte> ToBytes(this short value)
    {
        int defaultLength = 6;
        TryAgain:
        var bytes = new byte[defaultLength];
        if (Utf8Formatter.TryFormat(value, bytes, out int length))
        {
            return new(bytes, 0, length);
        }
        else
        {
            defaultLength *= 2;
            goto TryAgain;
        }
    }

    internal static Memory<byte> ToBytes(this ushort value)
    {
        int defaultLength = 5;
        TryAgain:
        var bytes = new byte[defaultLength];
        if (Utf8Formatter.TryFormat(value, bytes, out int length))
        {
            return new(bytes, 0, length);
        }
        else
        {
            defaultLength *= 2;
            goto TryAgain;
        }
    }

    internal static Memory<byte> ToBytes(this int value)
    {
        int defaultLength = 11;
        TryAgain:
        var bytes = new byte[defaultLength];
        if (Utf8Formatter.TryFormat(value, bytes, out int length))
        {
            return new(bytes, 0, length);
        }
        else
        {
            defaultLength *= 2;
            goto TryAgain;
        }
    }

    internal static Memory<byte> ToBytes(this uint value)
    {
        int defaultLength = 10;
        TryAgain:
        var bytes = new byte[defaultLength];
        if (Utf8Formatter.TryFormat(value, bytes, out int length))
        {
            return new(bytes, 0, length);
        }
        else
        {
            defaultLength *= 2;
            goto TryAgain;
        }
    }

    internal static Memory<byte> ToBytes(this long value)
    {
        int defaultLength = 20;
        TryAgain:
        var bytes = new byte[defaultLength];
        if (Utf8Formatter.TryFormat(value, bytes, out int length))
        {
            return new(bytes, 0, length);
        }
        else
        {
            defaultLength *= 2;
            goto TryAgain;
        }
    }

    internal static Memory<byte> ToBytes(this ulong value)
    {
        int defaultLength = 20;
        TryAgain:
        var bytes = new byte[defaultLength];
        if (Utf8Formatter.TryFormat(value, bytes, out int length))
        {
            return new(bytes, 0, length);
        }
        else
        {
            defaultLength *= 2;
            goto TryAgain;
        }
    }

    internal static Memory<byte> ToBytes(this float value)
    {
        int defaultLength = 16;
        TryAgain:
        var bytes = new byte[defaultLength];
        if (Utf8Formatter.TryFormat(value, bytes, out int length))
        {
            return new(bytes, 0, length);
        }
        else
        {
            defaultLength *= 2;
            goto TryAgain;
        }
    }

    internal static Memory<byte> ToBytes(this double value)
    {
        int defaultLength = 16;
        TryAgain:
        var bytes = new byte[defaultLength];
        if (Utf8Formatter.TryFormat(value, bytes, out int length))
        {
            return new(bytes, 0, length);
        }
        else
        {
            defaultLength *= 2;
            goto TryAgain;
        }
    }

    internal static Memory<byte> ToBytes(this decimal value)
    {
        int defaultLength = 16;
        TryAgain:
        var bytes = new byte[defaultLength];
        if (Utf8Formatter.TryFormat(value, bytes, out int length))
        {
            return new(bytes, 0, length);
        }
        else
        {
            defaultLength *= 2;
            goto TryAgain;
        }
    }

    internal static Memory<byte> ToBytes(this BigInteger value)
    {
        return Encoding.UTF8.GetBytes(value.ToString());
    }

#if NET7_0_OR_GREATER
    internal static Memory<byte> ToBytes(this Int128 value)
    {
        return Encoding.UTF8.GetBytes(value.ToString());
    }

    internal static Memory<byte> ToBytes(this UInt128 value)
    {
        return Encoding.UTF8.GetBytes(value.ToString());
    }
#endif

    internal static bool IsValidUInt64(this ReadOnlySpan<char> span)
    {
        if (span.IsEmpty) return false;

#if NETSTANDARD2_0
        ulong value = 0;
        foreach (char c in span)
        {
            if (c < '0' || c > '9') return false;
            ulong digit = (ulong)(c - '0');
            if (value > (ulong.MaxValue - digit) / 10) return false;
            value = value * 10 + digit;
        }
        return true;
#else
        return ulong.TryParse(span, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out _);
#endif
    }
}
