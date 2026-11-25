using SharpRedis.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SharpRedis.Extensions;

internal static class Utilities
{
    internal static Dictionary<string, string> ConnectionToDictionary(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString)) throw new RedisInitializationException("Connection string is empty!");

        var props = connectionString.Split([',', '，'], StringSplitOptions.RemoveEmptyEntries)
            ?? throw new ArgumentException("Connection string invalid", nameof(connectionString));

        var result = new Dictionary<string, string>();
        for (uint i = 0; i < props.Length; i++)
        {
            int separator = props[i].IndexOf('=');
            if (separator <= 0 || separator == props[i].Length - 1)
                throw new ArgumentException("Invalid connection string option.");

            var key = props[i].Substring(0, separator).Trim().ToLowerInvariant();
            var val = props[i].Substring(separator + 1);
            result.Add(key, val);
        }
        return result;
    }

    internal static unsafe bool ConvertByteArrayToInt(byte[] bytes, out int result)
    {
        result = 0;
        var isNegative = false;
        var index = 0;
        fixed (byte* pBytes = bytes)
        {
            var p = pBytes;
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

    internal static unsafe bool ConvertByteArrayToInt(ref readonly ReadOnlySpan<byte> bytes, out int result)
    {
        result = 0;
        var isNegative = false;
        var index = 0;
        fixed (byte* pBytes = bytes)
        {
            var p = pBytes;
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

    internal static unsafe string SpanToString(ref readonly ReadOnlySpan<byte> bytes, Encoding encoding)
    {
#if MODERN_NET
        return encoding.GetString(bytes);
#else
        fixed (byte* pBytes = bytes)
        {
            return encoding.GetString(pBytes, bytes.Length);
        }
#endif
    }

    internal static string GetVersion()
    {
        var assembly = Assembly.GetEntryAssembly()
                       ?? Assembly.GetExecutingAssembly();

        var info = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        return info?.InformationalVersion
               ?? assembly.GetName().Version?.ToString()
               ?? "Unknown";
    }
}
