using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SharpRedis.Extensions;

namespace SharpRedis;

/// <summary>
/// The unified digital type encapsulation can be implicitly converted to any digital type, such as: int, uint, long, ulong, double, float, etc.
/// <para>数字返回值统一封装, 可隐式转换为int, uint, long, ulong, double, float等原生C#基本类型</para>
/// </summary>
public readonly struct RedisNumber
{
    internal static readonly RedisNumber _zero = new(RedisResult._zero);

#if MODERN_NET
    private readonly string? _numberString;
    private readonly byte[]? _numberBytes;
#else
    private readonly string _numberString;
    private readonly byte[] _numberBytes;
#endif

    /// <summary>
    /// Gets a value indicating whether the current RedisNumber object has a valid value of its number type.
    /// <para>获取一个值, 表示当前RedisNumber是否有一个有效的数字类型的值</para>
    /// </summary>
    public readonly bool HasValue
    {
        get
        {
            if (this._numberBytes is null && this._numberString is null) return false;
            return true;
        }
    }

    /// <summary>
    /// Indicates whether the value should be considered a null value
    /// <para>当前值是不是应该视为null</para>
    /// </summary>
    public readonly bool IsNull
    {
        get
        {
            if (this._numberBytes is null && this._numberString is null) return true;
            return false;
        }
    }

    internal RedisNumber(RedisResult number)
    {
        if (number.IsNullOrEmpty) return;

        if (number.RawValue is string str)
        {
            this._numberString = str;
            return;
        }
        else if (number.RawValue is byte[] bytes)
        {
            this._numberBytes = bytes;
            return;
        }
        else if (number.Type is Enums.RespType.Array && number.RawValue is RedisResult[] { Length: 1 } arr)
        {
            if (arr[0].RawValue is string str1)
            {
                this._numberString = str1;
                return;
            }
            else if (arr[0].RawValue is byte[] bytes1)
            {
                this._numberBytes = bytes1;
                return;
            }
        }
        else if (number.Type is Enums.RespType.Maps && number.RawValue is KeyValuePair<RedisResult, RedisResult>[] { Length: 1 } maps)
        {
            var item = maps[0].Value;
            if (item.RawValue is string mapStr)
            {
                this._numberString = mapStr;
                return;
            }
            else if (item.RawValue is byte[] mapbytes)
            {
                this._numberBytes = mapbytes;
                return;
            }
        }
        throw new FormatException("The RedisResult is not a valid number format.");
    }

#if MODERN_NET
    public override string? ToString()
#else
    public override string ToString()
#endif
    {
        if (this._numberString is not null) return this._numberString;
        if (this._numberBytes is not null) return Encoding.UTF8.GetString(this._numberBytes);
        return null;
    }

    public static implicit operator byte(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return byte.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            if (Utf8Parser.TryParse(number._numberBytes, out byte value, out _))
            {
                return value;
            }
            else
            {
                throw new FormatException("The byte array could not be parsed as an byte.");
            }
        }
        else
        {
            throw new FormatException("The data could not be parsed as an byte.");
        }
    }

    public static implicit operator sbyte(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return sbyte.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            if (Utf8Parser.TryParse(number._numberBytes, out sbyte value, out _))
            {
                return value;
            }
            else
            {
                throw new FormatException("The byte array could not be parsed as an sbyte.");
            }
        }
        else
        {
            throw new FormatException("The data could not be parsed as an sbyte.");
        }
    }

    public static implicit operator short(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return short.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            if (Utf8Parser.TryParse(number._numberBytes, out short value, out _))
            {
                return value;
            }
            else
            {
                throw new FormatException("The byte array could not be parsed as an short.");
            }
        }
        else
        {
            throw new FormatException("The data could not be parsed as an short.");
        }
    }

    public static implicit operator ushort(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return ushort.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            if (Utf8Parser.TryParse(number._numberBytes, out ushort value, out _))
            {
                return value;
            }
            else
            {
                throw new FormatException("The byte array could not be parsed as an ushort.");
            }
        }
        else
        {
            throw new FormatException("The data could not be parsed as an ushort.");
        }
    }

    public static implicit operator int(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return int.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            if (Utf8Parser.TryParse(number._numberBytes, out int value, out _))
            {
                return value;
            }
            else
            {
                throw new FormatException("The byte array could not be parsed as an int.");
            }
        }
        else
        {
            throw new FormatException("The data could not be parsed as an int.");
        }
    }

    public static implicit operator uint(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return uint.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            if (Utf8Parser.TryParse(number._numberBytes, out uint value, out _))
            {
                return value;
            }
            else
            {
                throw new FormatException("The byte array could not be parsed as an uint.");
            }
        }
        else
        {
            throw new FormatException("The data could not be parsed as an uint.");
        }
    }

    public static implicit operator long(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return long.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            if (Utf8Parser.TryParse(number._numberBytes, out long value, out _))
            {
                return value;
            }
            else
            {
                throw new FormatException("The byte array could not be parsed as an long.");
            }
        }
        else
        {
            throw new FormatException("The data could not be parsed as an long.");
        }
    }

    public static implicit operator ulong(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return ulong.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            if (Utf8Parser.TryParse(number._numberBytes, out ulong value, out _))
            {
                return value;
            }
            else
            {
                throw new FormatException("The byte array could not be parsed as an ulong.");
            }
        }
        else
        {
            throw new FormatException("The data could not be parsed as an ulong.");
        }
    }

    public static implicit operator float(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return float.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            if (Utf8Parser.TryParse(number._numberBytes, out float value, out _))
            {
                return value;
            }
            else
            {
                throw new FormatException("The byte array could not be parsed as an float.");
            }
        }
        else
        {
            throw new FormatException("The data could not be parsed as an float.");
        }
    }

    public static implicit operator double(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            if (number._numberString.Equals("inf", StringComparison.OrdinalIgnoreCase) || number._numberString.Equals("+inf", StringComparison.OrdinalIgnoreCase))
            {
                return double.PositiveInfinity;
            }
            else if (number._numberString.Equals("-inf", StringComparison.OrdinalIgnoreCase))
            {
                return double.NegativeInfinity;
            }
            else
            {
                return double.Parse(number._numberString);
            }
        }
        else if (number._numberBytes is not null)
        {
            if (number._numberBytes.TryParseInfinity(out var result))
            {
                return result;
            }

            if (Utf8Parser.TryParse(number._numberBytes, out double value, out _))
            {
                return value;
            }
            else
            {
                throw new FormatException("The byte array could not be parsed as an double.");
            }
        }
        else
        {
            throw new FormatException("The data could not be parsed as an double.");
        }
    }

    public static implicit operator decimal(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return decimal.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            if (Utf8Parser.TryParse(number._numberBytes, out decimal value, out _))
            {
                return value;
            }
            else
            {
                throw new FormatException("The byte array could not be parsed as an decimal.");
            }
        }
        else
        {
            throw new FormatException("The data could not be parsed as an decimal.");
        }
    }

    public static implicit operator BigInteger(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return BigInteger.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            return BigInteger.Parse(Encoding.UTF8.GetString(number._numberBytes));
        }
        else
        {
            throw new FormatException("The data could not be parsed as an BigInteger.");
        }
    }

#if NET7_0_OR_GREATER
    public static implicit operator Int128(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return Int128.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            return Int128.Parse(Encoding.UTF8.GetString(number._numberBytes));
        }
        else
        {
            throw new FormatException("The data could not be parsed as an Int128.");
        }
    }

    public static implicit operator UInt128(RedisNumber number)
    {
        if (!number.HasValue) throw new NullReferenceException("There are currently no valid numerical data.");
        if (number._numberString is not null)
        {
            return UInt128.Parse(number._numberString);
        }
        else if (number._numberBytes is not null)
        {
            return UInt128.Parse(Encoding.UTF8.GetString(number._numberBytes));
        }
        else
        {
            throw new FormatException("The data could not be parsed as an UInt128.");
        }
    }
#endif
}
