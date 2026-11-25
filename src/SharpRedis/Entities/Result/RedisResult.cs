using SharpRedis.Enums;
using SharpRedis.Extensions;
using SharpRedis.Modules;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SharpRedis;

/// <summary>
/// The return value of executing Redis commands
/// <para>执行Redis命令的返回值</para>
/// </summary>
public readonly struct RedisResult : IEquatable<RedisResult>
{
    public static readonly RedisResult Empty = new(RespType.Empty, DBNull.Value, Encoding.UTF8);
    internal static readonly RedisResult _pipelineOrTranEmpty = new(RespType.PipelineOrTranEmpty, DBNull.Value, Encoding.UTF8);
    internal static readonly RedisResult _zero = new(RespType.Integer, "0", Encoding.UTF8);

    private readonly Encoding _encoding;

    internal readonly RespType Type { get; }

    /// <summary>
    /// Get the original data of the return value
    /// <para>获得返回值的原始数据</para>
    /// </summary>
    public readonly object RawValue { get; }

    /// <summary>
    /// Indicates whether there is a value at present. As long as it is not null, there is a value.
    /// <para>当前是否有值. 只要不是null, 就是有</para>
    /// </summary>
    public readonly bool HasValue => !this.IsNull;

    /// <summary>
    /// Indicates whether the value should be considered a null value
    /// <para>当前值是不是应该视为null</para>
    /// </summary>
    public readonly bool IsNull
    {
        get
        {
            if (this.RawValue is null || this.RawValue is DBNull) return true;
            return false;
        }
    }

    /// <summary>
    /// Indicates whether the value is either null or a zero-length value.
    /// <para>当前值是不是空的, 和null有区别, 比如返回了""</para>
    /// </summary>
    public readonly bool IsNullOrEmpty
    {
        get
        {
            if (this.IsNull) return true;
            if (this.RawValue is string { Length: 0 }) return true;
            if (this.RawValue is byte[] { Length: 0 }) return true;
            return false;
        }
    }

    /// <summary>
    /// If the return value is an exception, then it is true.
    /// <para>返回值是否是一个异常</para>
    /// </summary>
    public readonly bool HasException
    {
        get
        {
            if (this.Type is RespType.Exception or RespType.SimpleErrors or RespType.BulkErrors)
            {
                return true;
            }
            if (this.RawValue is Exception) return true;
            return false;
        }
    }

    public RedisResult()
    {
        throw new NotSupportedException("This type does not support creation.");
    }

    internal RedisResult(RespType type, object value, Encoding encoding)
    {
        this.Type = type;
        this.RawValue = value;
        this._encoding = encoding;
    }

#if MODERN_NET
    public override string? ToString()
#else
    public override string ToString()
#endif
    {
        try
        {
            return this.ToString(this._encoding);
        }
        catch
        {
            return this.RawValue.ToString();
        }
    }

    public override int GetHashCode()
    {
#if NETSTANDARD2_0
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + this.Type.GetHashCode();
            hash = hash * 31 + (this.RawValue?.GetHashCode() ?? 0);
            return hash;
        }
#else
        return HashCode.Combine(this.Type, this.RawValue);
#endif
    }

    public bool Equals(RedisResult other)
    {
        return other.Type == this.Type
            && Equals(this.RawValue, other.RawValue);
    }

#if MODERN_NET
    public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
#else
    public override bool Equals(object obj)
#endif
    {
        if (obj is not RedisResult other) return false;
        return other == this;
    }

    public static bool operator ==(RedisResult left, RedisResult right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RedisResult left, RedisResult right)
    {
        return !left.Equals(right);
    }

#if MODERN_NET
    public static implicit operator RedisResult(string? value)
#else
    public static implicit operator RedisResult(string value)
#endif
    {
        if (value is null) return RedisResult.Empty;
        return new(RespType.BulkStrings, value, Encoding.UTF8);
    }

#if MODERN_NET
    public static implicit operator RedisResult(byte[]? value)
#else
    public static implicit operator RedisResult(byte[] value)
#endif
    {
        if (value is null) return RedisResult.Empty;
        return new(RespType.BulkStrings, value, Encoding.UTF8);
    }

    #region Implicit operator methods

#if MODERN_NET
    public static implicit operator string?(RedisResult result)
#else
    public static implicit operator string(RedisResult result)
#endif
    {
        return result.ToString(result._encoding);
    }

#if MODERN_NET
    public static implicit operator string?[]?(RedisResult result)
#else
    public static implicit operator string[](RedisResult result)
#endif
    {
        return result.ToStringArray(result._encoding);
    }

    public static implicit operator RedisNumber(RedisResult result)
    {
        if (result.Type == RespType.PipelineOrTranEmpty)
        {
            return RedisNumber._zero;
        }
        return new(result);
    }

    #region Integer methods
    public static implicit operator sbyte(RedisResult result) => (RedisNumber)result;

    public static implicit operator byte(RedisResult result) => (RedisNumber)result;

    public static implicit operator short(RedisResult result) => (RedisNumber)result;

    public static implicit operator ushort(RedisResult result) => (RedisNumber)result;

    public static implicit operator int(RedisResult result) => (RedisNumber)result;

    public static implicit operator uint(RedisResult result) => (RedisNumber)result;

    public static implicit operator long(RedisResult result) => (RedisNumber)result;

    public static implicit operator long?(RedisResult result)
    {
        return result.ToNullableInt64();
    }

    public static implicit operator ulong(RedisResult result) => (RedisNumber)result;

    public static implicit operator BigInteger(RedisResult result) => (RedisNumber)result;

#if NET7_0_OR_GREATER
    public static implicit operator Int128(RedisResult result) => (RedisNumber)result;

    public static implicit operator UInt128(RedisResult result) => (RedisNumber)result;
#endif
    #endregion

    public static implicit operator double(RedisResult result) => (RedisNumber)result;

    public static implicit operator double?(RedisResult result)
    {
        if (result.IsNull || result.Type == RespType.PipelineOrTranEmpty) return null;
        return (double)result;
    }

    public static implicit operator bool(RedisResult result) => result.ToBoolean();

#if MODERN_NET
    public static implicit operator RedisResult[]?(RedisResult result)
#else
    public static implicit operator RedisResult[](RedisResult result)
#endif
        => result.ToArray();

    public static implicit operator RedisString.RedisLcsResult(RedisResult result)
    {
        if (result.Type == RespType.PipelineOrTranEmpty) return default;
        return new(result);
    }

    public static implicit operator bool[](RedisResult result) => result.ToBoolArray();

#if MODERN_NET
    public static implicit operator Dictionary<RedisResult, RedisResult>?(RedisResult result)
#else
    public static implicit operator Dictionary<RedisResult, RedisResult>(RedisResult result)
#endif
        => result.ToDictionary();

#if MODERN_NET
    public static implicit operator KeyValuePair<RedisResult, RedisNumber>[]?(RedisResult result)
#else
    public static implicit operator KeyValuePair<RedisResult, RedisNumber>[](RedisResult result)
#endif
        => result.ToKeyNumberValueArray();

    public static implicit operator KeyValuePair<RedisResult, RedisNumber>?(RedisResult result)
    {
        var array = result.ToKeyNumberValueArray();
        if (array is null or { Length: <= 0 }) return null;
        return array[0];
    }

#if MODERN_NET
    public static implicit operator KeyValuePair<RedisResult, RedisResult>[]?(RedisResult result)
#else
    public static implicit operator KeyValuePair<RedisResult, RedisResult>[](RedisResult result)
#endif
        => result.ToKeyValuePairArray();

    public static implicit operator RedisHash.HashScanResult(RedisResult result)
    {
        if (result.Type == RespType.PipelineOrTranEmpty) return RedisHash.HashScanResult._empty;
        return new(result);
    }

    public static implicit operator RedisHash.HashScanNoValuesResult(RedisResult result)
    {
        if (result.Type == RespType.PipelineOrTranEmpty) return RedisHash.HashScanNoValuesResult._empty;
        return new(result);
    }

    public static implicit operator RedisList.ListPopResult(RedisResult result)
    {
        if (result.Type == RespType.PipelineOrTranEmpty) return RedisList.ListPopResult._empty;
        return new(result);
    }

#if MODERN_NET
    public static implicit operator RedisNumber[]?(RedisResult value)
#else
    public static implicit operator RedisNumber[](RedisResult value)
#endif
        => value.ToRedisNumberArray();

    public static implicit operator RedisList.ListBlockPopResult(RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisSet.SetScanResult(RedisResult result)
    {
        if (result.Type == RespType.PipelineOrTranEmpty) return RedisSet.SetScanResult._empty;
        return new(result);
    }

    public static implicit operator RedisSortedSet.SortedSetMPopResult(RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisSortedSet.SortedSetRankResult(RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisSortedSet.SortedSetPopResult(RedisResult value)
    {
        if (value.Type == RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisString.IncrExResult(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

#if MODERN_NET
    public static implicit operator byte[]?(RedisResult value)
#else
    public static implicit operator byte[](RedisResult value)
#endif
    {
        if (value.IsNull || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.RawValue is byte[] bytes) return bytes;
        throw new InvalidCastException($"The data is not a valid byte[], The actual type is {value.RawValue.GetType().FullName}");
    }
    #endregion

    public static implicit operator RedisGeospatial.GeoCoordinate?(RedisResult value)
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        if (value.RawValue is not RedisResult[] { Length: 1 } array)
        {
            throw new InvalidCastException("The current value cannot be converted to GeoPosition");
        }
        if (array[0].IsNullOrEmpty) return null;
        return new RedisGeospatial.GeoCoordinate(array[0]);
    }

    public static implicit operator RedisGeospatial.GeoCoordinate?[](RedisResult value)
    {
        return value.ToGeoCoordinateArray();
    }

#if MODERN_NET
    public static implicit operator RedisGeospatial.GeoSearchResult[]?(RedisResult value)
#else
    public static implicit operator RedisGeospatial.GeoSearchResult[](RedisResult value)
#endif
    {
        return value.ToGeoRadiusArray();
    }

#if MODERN_NET
    public static implicit operator RedisFunction.ListResult[]?(RedisResult value)
#else
    public static implicit operator RedisFunction.ListResult[](RedisResult value)
#endif
    {
        return value.ToLibraryInfoArray();
    }

    public static implicit operator RedisFunction.StatsResult(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisStream.XReadResult?(RedisResult value)
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        return new(value);
    }

#if MODERN_NET
    public static implicit operator RedisStream.XReadResult[]?(RedisResult value)
#else
    public static implicit operator RedisStream.XReadResult[](RedisResult value)
#endif
    {
        return value.ToXReadResultArray();
    }

    public static implicit operator RedisStream.DeleteResult(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return (RedisStream.DeleteResult)(sbyte)value;
    }

    public static implicit operator RedisStream.DeleteResult[](RedisResult value)
    {
        return value.ToAckDeleteResultArray();
    }

    public static implicit operator RedisStream.EntryItem?(RedisResult value)
    {
        return value.ToEntryItem();
    }

#if MODERN_NET
    public static implicit operator RedisStream.EntryItem[]?(RedisResult value)
#else
    public static implicit operator RedisStream.EntryItem[](RedisResult value)
#endif
    {
        return value.ToEntryItemArray();
    }

    public static implicit operator RedisStream.XAutoClaimResult(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisStream.XPendingSummary(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

#if MODERN_NET
    public static implicit operator RedisStream.XPendingItem[]?(RedisResult value)
#else
    public static implicit operator RedisStream.XPendingItem[](RedisResult value)
#endif
    {
        return value.ToXPendingItemArray();
    }

#if MODERN_NET
    public static implicit operator RedisStream.ConsumerInfo[]?(RedisResult value)
#else
    public static implicit operator RedisStream.ConsumerInfo[](RedisResult value)
#endif
    {
        return value.ToConsumerInfoArray();
    }

#if MODERN_NET
    public static implicit operator RedisStream.GroupInfo[]?(RedisResult value)
#else
    public static implicit operator RedisStream.GroupInfo[](RedisResult value)
#endif
    {
        return value.ToGroupInfoArray();
    }

    public static implicit operator RedisStream.StreamInfo(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisBloom.BloomInfo(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisBloom.AddStatus[](RedisResult value)
    {
        return value.ToBfInsertStatusArray();
    }

    public static implicit operator RedisBloom.AddStatus(RedisResult value)
    {
        return value.ToBfInsertStatus();
    }

    public static implicit operator RedisBloom.ScanDumpResult(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisCuckoo.InsertNxStatus(RedisResult value)
    {
        return value.ToCfInsertNxStatus();
    }

    public static implicit operator RedisCuckoo.InsertNxStatus[](RedisResult value)
    {
        return value.ToCfInsertNxStatusArray();
    }

    public static implicit operator RedisCuckoo.ScanDumpResult(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisCuckoo.CuckooInfo(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

#if MODERN_NET
    public static implicit operator long[]?(RedisResult value)
#else
    public static implicit operator long[](RedisResult value)
#endif
    {
        return value.ToInt64Array();
    }

    public static implicit operator RedisCountMinSketch.SketchInfo(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisCountMinSketch.IncrByResult(RedisResult value)
    {
        return value.ToSketchIncrByResult();
    }

    public static implicit operator RedisCountMinSketch.IncrByResult[](RedisResult value)
    {
        return value.ToSketchIncrByResultArray();
    }

    public static implicit operator RedisCluster.SlotInfo[](RedisResult value)
    {
        return value.ToClusterSlotInfoArray();
    }

    public static implicit operator RedisKeyspace.WaitAofResult(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisKeyspace.ScanResult(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

    public static implicit operator RedisServer.AclUserInfo(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return null!;
        return new(value);
    }

#if MODERN_NET
    public static implicit operator RedisServer.AclLogEntry[]?(RedisResult value)
#else
    public static implicit operator RedisServer.AclLogEntry[](RedisResult value)
#endif
    {
        return value.ToAclLogEntryArray();
    }

#if MODERN_NET
    public static implicit operator RedisServer.HotKeysInfo?(RedisResult value)
#else
    public static implicit operator RedisServer.HotKeysInfo(RedisResult value)
#endif
    {
        if (value.IsNullOrEmpty || value.Type == RespType.PipelineOrTranEmpty) return null;
        return new(value);
    }

    public static implicit operator RedisServer.BackupStatusInfo(RedisResult value)
    {
        if (value.Type is RespType.PipelineOrTranEmpty) return default;
        return new(value);
    }

#if MODERN_NET
    public static implicit operator RedisServer.CommandMetadata?[](RedisResult value)
#else
    public static implicit operator RedisServer.CommandMetadata[](RedisResult value)
#endif
    {
        return value.ToCommandMetadataArray();
    }

    public static implicit operator Dictionary<string, RedisServer.CommandDocumentation>(RedisResult value)
    {
        return value.ToCommandDocumentation();
    }

    public static implicit operator RedisServer.CommandKeyFlags[](RedisResult value)
    {
        return value.ToCommandKeyFlagsArray();
    }

#if MODERN_NET
    public static implicit operator Dictionary<string, string>?(RedisResult result)
#else
    public static implicit operator Dictionary<string, string>(RedisResult result)
#endif
        => result.ToStringDictionary();

    public static implicit operator RedisServer.ModuleInfo[](RedisResult value)
        => value.ToModuleInfoArray();

#if MODERN_NET
    public static implicit operator RedisArray.ArItem[]?(RedisResult value)
#else
    public static implicit operator RedisArray.ArItem[](RedisResult value)
#endif
        => value.Type == RespType.PipelineOrTranEmpty ? null! : value.RawValue is RedisResult[] rows
            ? Array.ConvertAll(rows, static row => new RedisArray.ArItem(row))
            : null;

    public static implicit operator RedisServer.SlowLogEntry[](RedisResult value)
        => value.ToSlowLogEntryArray();

    public static implicit operator RedisServer.TimeResult(RedisResult value)
        => value.Type == RespType.PipelineOrTranEmpty ? default : new RedisServer.TimeResult(value);

    public static implicit operator RedisServer.RoleResult(RedisResult value)
        => value.Type == RespType.PipelineOrTranEmpty ? default : new RedisServer.RoleResult(value);

    public static implicit operator RedisConnection.TrackingInfo(RedisResult value)
        => value.Type == RespType.PipelineOrTranEmpty ? default : new RedisConnection.TrackingInfo(value);

    public static implicit operator RedisConnection.HelloResult(RedisResult value)
        => value.Type == RespType.PipelineOrTranEmpty ? default : new RedisConnection.HelloResult(value);

    public static implicit operator RedisArray.ArInfoResult(RedisResult value)
        => value.Type == RespType.PipelineOrTranEmpty ? default : new RedisArray.ArInfoResult(value);

    public static implicit operator RedisSet.SetExpireResult(RedisResult value)
    {
        return value.ToSetExpireResult();
    }

    public static implicit operator RedisSet.SetExpireResult[](RedisResult value)
    {
        return value.ToSetExpireResultArray();
    }

    public static implicit operator RedisSet.SetPersistResult(RedisResult value)
    {
        return value.ToSetPersistResult();
    }

    public static implicit operator RedisSet.SetPersistResult[](RedisResult value)
    {
        return value.ToSetPersistResultArray();
    }
}
