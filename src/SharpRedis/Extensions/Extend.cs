using System;
using System.Collections.Generic;

namespace SharpRedis.Extensions
{
    internal static class Extend
    {
#if NET8_0_OR_GREATER
        private static readonly char[] _separator = [',', '，'];
#else
        private static readonly char[] _separator = new char[] { ',', '，' };
#endif

        internal static long GetUnixTimeMilliseconds(in DateTimeOffset dateTime)
        {
#if !NET40 && !NET45 && !LOW_NET
            return dateTime.ToUnixTimeMilliseconds();
#else
            return (long)(dateTime.UtcDateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
#endif
        }

        internal static long GetUnixTimeSeconds(in DateTimeOffset dateTime)
        {
#if !NET40 && !NET45 && !LOW_NET
            return dateTime.ToUnixTimeSeconds();
#else
            return (long)(dateTime.UtcDateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
#endif
        }

        internal static Dictionary<string, string> ConnectionToDictionary(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new RedisInitializationException("Connection string is empty!");

            var result = new Dictionary<string, string>();
            var props = connectionString.Split(Extend._separator, StringSplitOptions.RemoveEmptyEntries)
                ?? throw new RedisInitializationException("Connection string invalid");
            for (uint i = 0; i < props.Length; i++)
            {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                var kv = props[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
#else
                var kv = props[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
#endif
                if (kv.Length != 2 || string.IsNullOrEmpty(kv[0]) || string.IsNullOrEmpty(kv[1]))
                    throw new RedisInitializationException($"Connection string in [{props[i]}] invalid");

                var key = kv[0].Trim();
                var val = kv[1].Trim();
                result.Add(key.ToLower(), val);
            }
            return result;
        }

        unsafe internal static bool ConvertByteArrayToInt(byte[] bytes, out int result)
        {
            result = 0;
            bool isNegative = false;
            int index = 0;
            fixed (byte* pBytes = bytes)
            {
                byte* p = pBytes;
                if (*p == 45) //-: negative
                {
                    isNegative = true;
                    index = 1;
                    p++;
                }

                for (; index < bytes.Length; index++, p++)
                {
                    if (*p < 48 || *p > 57) return false;
                    result = result * 10 + (*p - 48);
                }
            }
            if (isNegative) result = -result;
            return true;
        }

        internal static void CheckIntegerType<TNumber>(in TNumber number, string exceptionMsg)
#if NET7_0_OR_GREATER
            where TNumber : struct, System.Numerics.INumber<TNumber>
#else
            where TNumber : struct, IEquatable<TNumber>
#endif
        {
            if (number is long)
            {
                return;
            }
            else if (number is ulong)
            {
                return;
            }
            else if (number is int)
            {
                return;
            }
            else if (number is uint)
            {
                return;
            }
            else if (number is short)
            {
                return;
            }
            else if (number is ushort)
            {
                return;
            }
#if !LOW_NET
            else if (number is System.Numerics.BigInteger)
            {
                return;
            }
#endif
#if NET7_0_OR_GREATER
            else if (number is Int128)
            {
                return;
            }
            else if (number is UInt128)
            {
                return;
            }
#endif
            throw new NotSupportedException(exceptionMsg);
        }

        internal static void CheckNumberType<TNumber>(in TNumber number, string exceptionMsg)
#if NET7_0_OR_GREATER
            where TNumber : struct, System.Numerics.INumber<TNumber>
#else
            where TNumber : struct, IEquatable<TNumber>
#endif
        {
            if (number is long)
            {
                return;
            }
            else if (number is ulong)
            {
                return;
            }
            else if (number is int)
            {
                return;
            }
            else if (number is uint)
            {
                return;
            }
            else if (number is short)
            {
                return;
            }
            else if (number is ushort)
            {
                return;
            }
            else if (number is float)
            {
                return;
            }
            else if (number is double)
            {
                return;
            }
            else if (number is decimal)
            {
                return;
            }
#if !LOW_NET
            else if (number is System.Numerics.BigInteger)
            {
                return;
            }
#endif
#if NET7_0_OR_GREATER
            else if (number is Int128)
            {
                return;
            }
            else if (number is UInt128)
            {
                return;
            }
#endif
            throw new NotSupportedException(exceptionMsg);
        }

        internal static TNumber GetOppositeValue<TNumber>(TNumber number)
            where TNumber : struct, IEquatable<TNumber>
        {
            if (number is long @long)
            {
                @long = -@long;
                if (@long is TNumber result) return result;
            }
            else if (number is int @int)
            {
                @int = -@int;
                if (@int is TNumber result) return result;
            }
            else if (number is short @short)
            {
                @short = (short)-@short;
                if (@short is TNumber result) return result;
            }
            else if (number is float @float)
            {
                @float = -@float;
                if (@float is TNumber result) return result;
            }
            else if (number is double @double)
            {
                @double = -@double;
                if (@double is TNumber result) return result;
            }
            else if (number is decimal @decimal)
            {
                @decimal = -@decimal;
                if (@decimal is TNumber result) return result;
            }
#if !LOW_NET
            else if (number is System.Numerics.BigInteger bigInteger)
            {
                bigInteger = -bigInteger;
                if (bigInteger is TNumber result) return result;
            }
#endif
#if NET7_0_OR_GREATER
            else if (number is Int128 int128)
            {
                int128 = -int128;
                if (int128 is TNumber result) return result;
            }
#endif
            throw new NotSupportedException("The opposite value is not supported");
        }

        unsafe internal static TEnum IntToEnum<TEnum>(int value)
            where TEnum : unmanaged, Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                throw new FormatException($"Value {value} is not defined in enum {typeof(TEnum).FullName}");
            }
            return *(TEnum*)&value;
        }

        internal unsafe static TTo As<TFrom, TTo>(TFrom* source)
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            return *(TTo*)source;
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        unsafe internal static TEnum[]? IntsToEnums<TEnum>(int[] values)
#else
        unsafe internal static TEnum[] IntsToEnums<TEnum>(int[] values)
#endif
            where TEnum : unmanaged, Enum
        {
            var result = new TEnum[values.Length];
            fixed (int* pStatusArray = values)
            {
                int* pCurrent = pStatusArray;
                for (int i = 0; i < values.Length; i++, pCurrent++)
                {
                    if (!Enum.IsDefined(typeof(TEnum), *pCurrent))
                    {
                        throw new FormatException($"Value {*pCurrent} is not defined in enum {typeof(TEnum).FullName}");
                    }
                    result[i] = *(TEnum*)pCurrent;
                }
            }
            return result;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        unsafe internal static byte[] SpanToBytes(this ReadOnlySpan<char> value, System.Text.Encoding encoding)
        {
            Span<byte> bytesSpan = new byte[encoding.GetMaxByteCount(value.Length)];
            int byteCount = encoding.GetBytes(value, bytesSpan);
            return bytesSpan[..byteCount].ToArray();
        }
#endif

        internal static long GetByteArrayHashCode(byte[] byteArray)
        {
            if (byteArray is null || byteArray.Length == 0) return 0;

            unchecked
            {
                long hash = 17;
                for (uint i = 0; i < byteArray.Length; i++)
                {
                    hash = hash * 31 + byteArray[i];
                }
                return hash;
            }
        }
    }
}
