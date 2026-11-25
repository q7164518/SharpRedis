using SharpRedis.Enums;
using SharpRedis.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpRedis.Extensions;

internal static class ConvertExtensions
{
    internal static bool TryParseInfinity(this byte[] buffer, out double result)
    {
        result = default;
        var length = buffer.Length;
        var offset = 0;
        if (length == 3)
        {
            // inf
            if ((buffer[offset] | 0x20) == 'i' &&
                (buffer[offset + 1] | 0x20) == 'n' &&
                (buffer[offset + 2] | 0x20) == 'f')
            {
                result = double.PositiveInfinity;
                return true;
            }
        }
        else if (length == 4)
        {
            byte sign = buffer[offset];

            if (sign == '+' || sign == '-')
            {
                if ((buffer[offset + 1] | 0x20) == 'i' &&
                    (buffer[offset + 2] | 0x20) == 'n' &&
                    (buffer[offset + 3] | 0x20) == 'f')
                {
                    result = sign == '-'
                        ? double.NegativeInfinity
                        : double.PositiveInfinity;

                    return true;
                }
            }
        }

        return false;
    }

    internal static bool ToBoolean(this byte[] bytes)
    {
        if (bytes.Length is 2)
        {
            return (bytes[0] is Config.O or Config.o)
                && (bytes[1] is Config.K or Config.k); //OK --> 79,75 or ok --> 111,107 --> true
        }
        else if (bytes.Length is 1)
        {
            return bytes[0] is Config.T or Config.t or Config.ONE; //T --> 84 or t --> 116 or 1 --> 49 --> true
        }
        return false;
    }

    internal static bool ToBoolean(this RedisResult redisValue)
    {
        if (redisValue.RawValue is string str)
        {
            return "OK".Equals(str, StringComparison.OrdinalIgnoreCase)
                || "t".Equals(str, StringComparison.OrdinalIgnoreCase)
                || "1".Equals(str, StringComparison.OrdinalIgnoreCase);
        }
        else if (redisValue.RawValue is byte[] bytes)
        {
            return bytes.ToBoolean();
        }
        else if (redisValue.RawValue is RedisResult[] array and { Length: 1 })
        {
            return array[0].ToBoolean();
        }
        return false;
    }

    internal static long? ToNullableInt64(this RedisResult value)
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type is RespType.Array && value.RawValue is RedisResult[] { Length: 1 } arr)
        {
            if (arr[0].IsNullOrEmpty) return null;
            return arr[0];
        }
        else if (value.Type is RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] { Length: 1 } maps)
        {
            if (maps[0].Value.IsNullOrEmpty) return null;
            return maps[0].Value;
        }
        return new RedisNumber(value);
    }

#if MODERN_NET
    internal static string? ToString(this RedisResult redisValue, Encoding encoding)
#else
    internal static string ToString(this RedisResult redisValue, Encoding encoding)
#endif
    {
        if (redisValue.IsNull) return null;
        if (redisValue.Type == RespType.PipelineOrTranEmpty || redisValue.RawValue is null or DBNull) return null;
        if (redisValue.RawValue is string str)
        {
            return str;
        }
        else if (redisValue.RawValue is RedisResult[] arr and { Length: 1 })
        {
            return arr[0];
        }
        else if (redisValue.RawValue is byte[] bytes)
        {
            if (bytes.Length is 0) return string.Empty;
            return encoding.GetString(bytes);
        }
        throw new FormatException($"The data is not a valid String, The actual type is {redisValue.RawValue.GetType().FullName}");
    }

#if MODERN_NET
    internal static string?[]? ToStringArray(this RedisResult redisValue, Encoding encoding)
#else
    internal static string[] ToStringArray(this RedisResult redisValue, Encoding encoding)
#endif
    {
        if (redisValue.IsNull) return null;
        if (redisValue.Type == RespType.PipelineOrTranEmpty || redisValue.RawValue is null or DBNull) return null;
        if (redisValue.RawValue is not RedisResult[] arr)
        {
            throw new FormatException($"The data is not a valid String[], The actual type is {redisValue.RawValue.GetType().FullName}");
        }
#if MODERN_NET
        var result = new string?[arr.Length];
#else
        var result = new string[arr.Length];
#endif
        for (int i = 0; i < arr.Length; i++)
        {
            result[i] = arr[i];
        }
        return result;
    }

    internal static bool[] ToBoolArray(this RedisResult redisValue)
    {
        if (redisValue.RawValue is RedisResult[] array and { Length: > 0 })
        {
            var result = new bool[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i].ToBoolean();
            }
            return result;
        }
        else
        {
            if (redisValue.Type == RespType.PipelineOrTranEmpty) return null!;
            return [];
        }
    }

#if MODERN_NET
    internal static Dictionary<RedisResult, RedisResult>? ToDictionary(this RedisResult value)
#else
    internal static Dictionary<RedisResult, RedisResult> ToDictionary(this RedisResult value)
#endif
    {
        if (value.IsNull || !value.HasValue || value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type is RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] maps)
        {
            var result = new Dictionary<RedisResult, RedisResult>(maps.Length);
            foreach (var item in maps)
            {
                result[item.Key] = item.Value;
            }
            return result;
        }
        else if (value.Type is RespType.Array && value.RawValue is RedisResult[] array)
        {
            if (array[0].Type is RespType.Array)
            {
                var result = new Dictionary<RedisResult, RedisResult>(array.Length);
                foreach (var item in array)
                {
                    if (item.RawValue is RedisResult[] { Length: 2 } itemArray)
                    {
                        result[itemArray[0]] = itemArray[1];
                    }
                    else
                    {
                        throw new FormatException($"The data is not a valid Dictionary, The actual type is {value.RawValue.GetType().FullName}");
                    }
                }
                return result;
            }
            else
            {
                if (array.Length <= 0) return [];
                if (array.Length % 2 != 0) throw new FormatException($"The data is not a valid Dictionary, The actual type is {value.RawValue.GetType().FullName}");
                var result = new Dictionary<RedisResult, RedisResult>(array.Length / 2);
                for (int i = 0; i < array.Length; i += 2)
                {
                    result.Add(array[i], array[i + 1]);
                }
                return result;
            }
        }
        else
        {
            throw new FormatException($"The data is not a valid Dictionary, The actual type is {value.RawValue.GetType().FullName}");
        }
    }

#if MODERN_NET
    internal static KeyValuePair<RedisResult, RedisNumber>[]? ToKeyNumberValueArray(this RedisResult value)
#else
    internal static KeyValuePair<RedisResult, RedisNumber>[] ToKeyNumberValueArray(this RedisResult value)
#endif
    {
        if (value.IsNull || !value.HasValue || value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.RawValue is KeyValuePair<RedisResult, RedisNumber>[] result1)
        {
            return result1;
        }
        if (value.RawValue is not RedisResult[] array)
        {
            throw new FormatException($"The data is not a valid KeyValuePair[], The actual type is {value.RawValue.GetType().FullName}");
        }

        if (array[0].Type is RespType.Array)
        {
            if (array.Length <= 0) return [];
            var result = new KeyValuePair<RedisResult, RedisNumber>[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                var item = array[i];
                if (item.RawValue is RedisResult[] { Length: 2 } itemArray)
                {
                    result[i] = new(itemArray[0], itemArray[1]);
                }
                else
                {
                    throw new FormatException($"The data is not a valid KeyValuePair[], The actual type is {value.RawValue.GetType().FullName}");
                }
            }
            return result;
        }
        else
        {
            if (array.Length <= 0) return [];
            if (array.Length % 2 != 0) throw new FormatException($"The data is not a valid KeyValuePair[], The actual type is {value.RawValue.GetType().FullName}");
            var result = new KeyValuePair<RedisResult, RedisNumber>[array.Length / 2];
            for (int i = 0, index = 0; i < array.Length; i += 2, index++)
            {
                result[index] = new(array[i], array[i + 1]);
            }
            return result;
        }
    }

#if MODERN_NET
    internal static KeyValuePair<RedisResult, RedisResult>[]? ToKeyValuePairArray(this RedisResult value)
#else
    internal static KeyValuePair<RedisResult, RedisResult>[] ToKeyValuePairArray(this RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.RawValue is not RedisResult[] array)
        {
            throw new FormatException($"The data is not a valid KeyValuePair array, The actual type is {value.RawValue.GetType().FullName}");
        }

        if (array[0].Type is RespType.Array)
        {
            var result = new KeyValuePair<RedisResult, RedisResult>[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                var item = array[i];
                if (item.RawValue is not RedisResult[] { Length: 2 } itemArr)
                {
                    throw new FormatException($"The data is not a valid KeyValuePair array, The actual type is {value.RawValue.GetType().FullName}");
                }
                result[i] = new KeyValuePair<RedisResult, RedisResult>(itemArr[0], itemArr[1]);
            }
            return result;
        }
        else
        {
            var result = new KeyValuePair<RedisResult, RedisResult>[array.Length / 2];
            for (int i = 0, index = 0; i < array.Length; i += 2, index++)
            {
                result[index] = new KeyValuePair<RedisResult, RedisResult>(array[i], array[i + 1]);
            }
            return result;
        }
    }

#if MODERN_NET
    internal static RedisResult[]? ToArray(this RedisResult value)
#else
    internal static RedisResult[] ToArray(this RedisResult value)
#endif
    {
        if (value.Type is RespType.Array)
        {
            if (value.RawValue is RedisResult[] array)
            {
                return array;
            }
            else if (value.RawValue is DBNull or null)
            {
                return null;
            }
        }
        else if (value.Type == RespType.PipelineOrTranEmpty || value.Type == RespType.Null || (value.RawValue is DBNull or null))
        {
            return null;
        }
        throw new InvalidCastException("The return value is not an array, so it cannot be converted.");
    }

#if MODERN_NET
    internal static RedisNumber[]? ToRedisNumberArray(this RedisResult value)
#else
    internal static RedisNumber[] ToRedisNumberArray(this RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type is RespType.Array && value.RawValue is RedisResult[] array)
        {
            if (array.Length <= 0) return [];
            var arrayResult = new RedisNumber[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                arrayResult[i] = array[i];
            }
            return arrayResult;
        }
        throw new InvalidCastException($"The return value is not a RedisNumber array and cannot be converted to {nameof(RedisNumber)}[]. The actual type is {value.RawValue.GetType().FullName}");
    }

    internal static RedisGeospatial.GeoCoordinate?[] ToGeoCoordinateArray(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        if (value.Type != RespType.Array || value.RawValue is not RedisResult[] array)
        {
            throw new InvalidCastException($"The return value is not a GeoCoordinate array and cannot be converted to {nameof(RedisGeospatial.GeoCoordinate)}[]. The actual type is {value.RawValue.GetType().FullName}");
        }

        var result = new RedisGeospatial.GeoCoordinate?[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            var item = array[i];
            if (item.IsNullOrEmpty)
            {
                result[i] = null;
                continue;
            }
            if (item.RawValue is not RedisResult[] { Length: 2 } pos)
            {
                throw new InvalidCastException($"The current value cannot be converted to {nameof(RedisGeospatial.GeoCoordinate)}");
            }
            result[i] = new(pos[0], pos[1]);
        }
        return result;
    }

#if MODERN_NET
    internal static RedisGeospatial.GeoSearchResult[]? ToGeoRadiusArray(this RedisResult value)
#else
    internal static RedisGeospatial.GeoSearchResult[] ToGeoRadiusArray(this RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type != RespType.Array || value.RawValue is not RedisResult[] array)
        {
            throw new InvalidCastException($"The return value is not a GeoRadiusResult array and cannot be converted to {nameof(RedisGeospatial.GeoSearchResult)}[]. The actual type is {value.RawValue.GetType().FullName}");
        }

        var result = new RedisGeospatial.GeoSearchResult[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = new(array[i]);
        }
        return result;
    }

#if MODERN_NET
    internal static RedisFunction.ListResult[]? ToLibraryInfoArray(this RedisResult value)
#else
    internal static RedisFunction.ListResult[] ToLibraryInfoArray(this RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type != RespType.Array || value.RawValue is not RedisResult[] array)
        {
            throw new InvalidCastException($"The return value is not a LibraryInfo array and cannot be converted to {nameof(RedisFunction.ListResult)}[]. The actual type is {value.RawValue.GetType().FullName}");
        }
        var result = new RedisFunction.ListResult[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = new(array[i]);
        }
        return result;
    }

#if MODERN_NET
    internal static RedisStream.XReadResult[]? ToXReadResultArray(this RedisResult value)
#else
    internal static RedisStream.XReadResult[] ToXReadResultArray(this RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type is RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] maps)
        {
            var result = new RedisStream.XReadResult[maps.Length];
            for (int i = 0; i < maps.Length; i++)
            {
                result[i] = new(maps[i]);
            }
            return result;
        }
        else if (value.Type is RespType.Array && value.RawValue is RedisResult[] array)
        {
            var result = new RedisStream.XReadResult[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].RawValue is not RedisResult[] { Length: 2 } item)
                {
                    throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisStream.XReadResult)}[]");
                }
                result[i] = new(item);
            }
            return result;
        }
        else
        {
            throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisStream.XReadResult)}[]");
        }
    }

    internal static RedisStream.DeleteResult[] ToAckDeleteResultArray(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        if (value.RawValue is not RedisResult[] array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisStream.DeleteResult)}[]");
        var result = new RedisStream.DeleteResult[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = (RedisStream.DeleteResult)(sbyte)array[i];
        }
        return result;
    }

    internal static RedisStream.EntryItem? ToEntryItem(this RedisResult value)
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type != RespType.Array || value.RawValue is not RedisResult[] { Length: 1 } array)
        {
            throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisStream.EntryItem)}");
        }
        return new(array[0]);
    }

#if MODERN_NET
    internal static RedisStream.EntryItem[]? ToEntryItemArray(this RedisResult value)
#else
    internal static RedisStream.EntryItem[] ToEntryItemArray(this RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type != RespType.Array || value.RawValue is not RedisResult[] array)
        {
            throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisStream.EntryItem)}[]");
        }
        var result = new RedisStream.EntryItem[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = new(array[i]);
        }
        return result;
    }

#if MODERN_NET
    internal static RedisStream.XPendingItem[]? ToXPendingItemArray(this RedisResult value)
#else
    internal static RedisStream.XPendingItem[] ToXPendingItemArray(this RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.RawValue is not RedisResult[] array)
        {
            throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisStream.XPendingItem)}[]");
        }

        var result = new RedisStream.XPendingItem[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            result[i] = new(array[i]);
        }

        return result;
    }

#if MODERN_NET
    internal static RedisStream.ConsumerInfo[]? ToConsumerInfoArray(this RedisResult value)
#else
    internal static RedisStream.ConsumerInfo[] ToConsumerInfoArray(this RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type != RespType.Array || value.RawValue is not RedisResult[] array)
        {
            throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisStream.ConsumerInfo)}[]");
        }
        var result = new RedisStream.ConsumerInfo[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = new(array[i]);
        }
        return result;
    }

#if MODERN_NET
    internal static RedisStream.GroupInfo[]? ToGroupInfoArray(this RedisResult value)
#else
    internal static RedisStream.GroupInfo[] ToGroupInfoArray(this RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type != RespType.Array || value.RawValue is not RedisResult[] array)
        {
            throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisStream.GroupInfo)}[]");
        }
        var result = new RedisStream.GroupInfo[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = new(array[i]);
        }
        return result;
    }

    internal static RedisBloom.AddStatus[] ToBfInsertStatusArray(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        if (value.RawValue is not RedisResult[] array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisBloom.AddStatus)}[]");
        var result = new RedisBloom.AddStatus[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            var item = array[i];
            if (item.Type is RespType.SimpleErrors || item.Type is RespType.BulkErrors)
            {
                result[i] = RedisBloom.AddStatus.Error;
            }
            else
            {
                if (item.Type is RespType.Boolean)
                {
                    if (item)
                    {
                        result[i] = RedisBloom.AddStatus.Added;
                    }
                    else
                    {
                        result[i] = RedisBloom.AddStatus.Exists;
                    }
                }
                else
                {
                    result[i] = (RedisBloom.AddStatus)(byte)item;
                }
            }
        }
        return result;
    }

    internal static RedisBloom.AddStatus ToBfInsertStatus(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return default;
        if (value.RawValue is not RedisResult[] { Length: 1 } array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisBloom.AddStatus)}");
        var item = array[0];
        if (item.Type is RespType.BulkErrors || item.Type is RespType.SimpleErrors)
        {
            return RedisBloom.AddStatus.Error;
        }
        else
        {
            if (item.Type is RespType.Boolean)
            {
                if (item)
                {
                    return RedisBloom.AddStatus.Added;
                }
                else
                {
                    return RedisBloom.AddStatus.Exists;
                }
            }
            else
            {
                return (RedisBloom.AddStatus)(byte)array[0];
            }
        }
    }

    internal static RedisCuckoo.InsertNxStatus ToCfInsertNxStatus(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return default;
        if (value.RawValue is not RedisResult[] { Length: 1 } array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisCuckoo.InsertNxStatus)}");
        return (RedisCuckoo.InsertNxStatus)(sbyte)array[0];
    }

    internal static RedisCuckoo.InsertNxStatus[] ToCfInsertNxStatusArray(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        if (value.RawValue is not RedisResult[] array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisCuckoo.InsertNxStatus)}[]");
        var result = new RedisCuckoo.InsertNxStatus[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = (RedisCuckoo.InsertNxStatus)(sbyte)array[i];
        }
        return result;
    }

#if MODERN_NET
    internal static long[]? ToInt64Array(this RedisResult value)
#else
    internal static long[] ToInt64Array(this RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type != RespType.Array || value.RawValue is not RedisResult[] array)
            throw new InvalidCastException("Unrecognizable data cannot be converted into long[]");
        var result = new long[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = array[i];
        }
        return result;
    }

    internal static RedisCountMinSketch.IncrByResult ToSketchIncrByResult(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return default;
        if (value.RawValue is not RedisResult[] { Length: 1 } array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisCountMinSketch.IncrByResult)}");
        return new(array[0]);
    }

    internal static RedisCountMinSketch.IncrByResult[] ToSketchIncrByResultArray(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        if (value.RawValue is not RedisResult[] array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisCountMinSketch.IncrByResult)}[]");
        var result = new RedisCountMinSketch.IncrByResult[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = new(array[i]);
        }
        return result;
    }

    internal static RedisCluster.SlotInfo[] ToClusterSlotInfoArray(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        if (value.RawValue is not RedisResult[] array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisCluster.SlotInfo)}[]");
        var result = new RedisCluster.SlotInfo[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = new(array[i]);
        }
        return result;
    }

#if MODERN_NET
    internal static RedisServer.AclLogEntry[]? ToAclLogEntryArray(this RedisResult value)
#else
    internal static RedisServer.AclLogEntry[] ToAclLogEntryArray(this RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.RawValue is not RedisResult[] array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisServer.AclLogEntry)}[]");
        var result = new RedisServer.AclLogEntry[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = new(array[i]);
        }
        return result;
    }

#if MODERN_NET
    internal static RedisServer.CommandMetadata?[] ToCommandMetadataArray(this RedisResult value)
#else
    internal static RedisServer.CommandMetadata[] ToCommandMetadataArray(this RedisResult value)
#endif
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        return value.RawValue is RedisResult[] rows
            ? Array.ConvertAll(rows, static row => row.IsNullOrEmpty ? null : new RedisServer.CommandMetadata(row))
            : [];
    }

    internal static Dictionary<string, RedisServer.CommandDocumentation> ToCommandDocumentation(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        var docs = new Dictionary<string, RedisServer.CommandDocumentation>(StringComparer.OrdinalIgnoreCase);
        if (!value.IsNull) RedisServer.AclReplyReader.ReadFields(value, (name, data) => docs.Add(name, new(name, data)));
        return docs;
    }

    internal static RedisServer.CommandKeyFlags[] ToCommandKeyFlagsArray(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        return value.RawValue is RedisResult[] rows
            ? Array.ConvertAll(rows, static row => new RedisServer.CommandKeyFlags(row))
            : [];
    }

#if MODERN_NET
    internal static Dictionary<string, string>? ToStringDictionary(this RedisResult value)
#else
    internal static Dictionary<string, string> ToStringDictionary(this RedisResult value)
#endif
    {
        if (value.IsNull || !value.HasValue || value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.Type is RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] maps)
        {
            var result = new Dictionary<string, string>(maps.Length);
            foreach (var item in maps)
            {
                result[item.Key!] = item.Value!;
            }
            return result;
        }
        else if (value.Type is RespType.Array && value.RawValue is RedisResult[] array)
        {
            if (array[0].Type is RespType.Array)
            {
                var result = new Dictionary<string, string>(array.Length);
                foreach (var item in array)
                {
                    if (item.RawValue is RedisResult[] { Length: 2 } itemArray)
                    {
                        result[itemArray[0]!] = itemArray[1]!;
                    }
                    else
                    {
                        throw new FormatException($"The data is not a valid Dictionary, The actual type is {value.RawValue.GetType().FullName}");
                    }
                }
                return result;
            }
            else
            {
                if (array.Length <= 0) return [];
                if (array.Length % 2 != 0) throw new FormatException($"The data is not a valid Dictionary, The actual type is {value.RawValue.GetType().FullName}");
                var result = new Dictionary<string, string>(array.Length / 2);
                for (int i = 0; i < array.Length; i += 2)
                {
                    result.Add(array[i]!, array[i + 1]!);
                }
                return result;
            }
        }
        else
        {
            throw new FormatException($"The data is not a valid Dictionary, The actual type is {value.RawValue.GetType().FullName}");
        }
    }

    internal static RedisServer.ModuleInfo[] ToModuleInfoArray(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        return value.RawValue is RedisResult[] rows
            ? Array.ConvertAll(rows, static row => new RedisServer.ModuleInfo(row))
            : [];
    }

    internal static RedisServer.SlowLogEntry[] ToSlowLogEntryArray(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        return value.RawValue is RedisResult[] rows
            ? Array.ConvertAll(rows, static row => new RedisServer.SlowLogEntry(row))
            : [];
    }

    internal static RedisSet.SetExpireResult ToSetExpireResult(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return default;
        if (value.RawValue is not RedisResult[] { Length: 1 } array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisSet.SetExpireResult)}");
        return (RedisSet.SetExpireResult)(sbyte)array[0];
    }

    internal static RedisSet.SetExpireResult[] ToSetExpireResultArray(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        if (value.RawValue is not RedisResult[] array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisSet.SetExpireResult)}[]");
        var result = new RedisSet.SetExpireResult[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = (RedisSet.SetExpireResult)(sbyte)array[i];
        }
        return result;
    }

    internal static RedisSet.SetPersistResult ToSetPersistResult(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return default;
        if (value.RawValue is not RedisResult[] { Length: 1 } array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisSet.SetPersistResult)}");
        return (RedisSet.SetPersistResult)(sbyte)array[0];
    }

    internal static RedisSet.SetPersistResult[] ToSetPersistResultArray(this RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return null!;
        if (value.RawValue is not RedisResult[] array) throw new InvalidCastException($"Unrecognizable data cannot be converted into {nameof(RedisSet.SetPersistResult)}[]");
        var result = new RedisSet.SetPersistResult[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = (RedisSet.SetPersistResult)(sbyte)array[i];
        }
        return result;
    }
}
