using SharpRedis.Modules;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SharpRedis.Extensions;

internal readonly struct SharpTask
{
    private readonly Task<RedisResult> _task;

    internal readonly Task<RedisResult> Task => this._task;

    internal SharpTask(Task<RedisResult> task)
    {
        this._task = task;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisResult>(SharpTask task)
        => task._task;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisNumber>(SharpTask task)
        => task._task.AsRedisNumber();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<long>(SharpTask task)
        => task._task.AsInt64();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<long?>(SharpTask task)
        => task._task.AsNullableInt64();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<int>(SharpTask task)
        => task._task.AsInt32();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<double>(SharpTask task)
        => task._task.AsDouble();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<double?>(SharpTask task)
        => task._task.AsNullableDouble();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<bool>(SharpTask task)
        => task._task.AsBool();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<bool[]>(SharpTask task)
        => task._task.AsBoolArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<string?>(SharpTask task)
#else
    public static implicit operator Task<string>(SharpTask task)
#endif
        => task._task.AsString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<string?[]?>(SharpTask task)
#else
    public static implicit operator Task<string[]>(SharpTask task)
#endif
        => task._task.AsStringArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisResult[]?>(SharpTask task)
#else
    public static implicit operator Task<RedisResult[]>(SharpTask task)
#endif
        => task._task.AsArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisString.RedisLcsResult>(SharpTask task)
        => task._task.AsLcsResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<Dictionary<RedisResult, RedisResult>?>(SharpTask task)
#else
    public static implicit operator Task<Dictionary<RedisResult, RedisResult>>(SharpTask task)
#endif
        => task._task.AsDictionary();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<KeyValuePair<RedisResult, RedisNumber>[]?>(SharpTask task)
#else
    public static implicit operator Task<KeyValuePair<RedisResult, RedisNumber>[]>(SharpTask task)
#endif
        => task._task.AsKeyNumberValueArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<KeyValuePair<RedisResult, RedisNumber>?>(SharpTask task)
        => task._task.AsKeyNumberValue();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisHash.HashScanResult>(SharpTask task)
        => task._task.AsHashScan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisHash.HashScanNoValuesResult>(SharpTask task)
        => task._task.AsHashScanNoValues();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisList.ListPopResult>(SharpTask task)
        => task._task.AsListPop();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisList.ListBlockPopResult>(SharpTask task)
        => task._task.AsListBlockPop();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisNumber[]?>(SharpTask task)
#else
    public static implicit operator Task<RedisNumber[]>(SharpTask task)
#endif
        => task._task.AsNumberArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisSet.SetScanResult>(SharpTask task)
        => task._task.AsSetScan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisSortedSet.SortedSetMPopResult>(SharpTask task)
        => task._task.AsZMPop();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisSortedSet.SortedSetPopResult>(SharpTask task)
        => task._task.AsZPop();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisSortedSet.SortedSetRankResult>(SharpTask task)
        => task._task.AsZRank();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisString.IncrExResult>(SharpTask task)
        => task._task.AsIncrExResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisGeospatial.GeoCoordinate?>(SharpTask task)
        => task._task.AsGeoCoordinate();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisGeospatial.GeoCoordinate?[]>(SharpTask task)
       => task._task.AsGeoCoordinateArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisGeospatial.GeoSearchResult[]?>(SharpTask task)
#else
    public static implicit operator Task<RedisGeospatial.GeoSearchResult[]>(SharpTask task)
#endif
        => task._task.AsGeoRadiusResultArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<byte[]?>(SharpTask task)
#else
    public static implicit operator Task<byte[]>(SharpTask task)
#endif
        => task._task.AsBytes();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisFunction.ListResult[]?>(SharpTask task)
#else
    public static implicit operator Task<RedisFunction.ListResult[]>(SharpTask task)
#endif
        => task._task.AsLibraryInfoArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisFunction.StatsResult>(SharpTask task)
        => task._task.AsFunctionStatsResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisStream.XReadResult?>(SharpTask task)
        => task._task.AsXReadResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisStream.XReadResult[]?>(SharpTask task)
#else
    public static implicit operator Task<RedisStream.XReadResult[]>(SharpTask task)
#endif
        => task._task.AsXReadResultArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisStream.DeleteResult>(SharpTask task)
    {
        return task._task.AsAckDeleteResult();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisStream.DeleteResult[]>(SharpTask task)
    {
        return task._task.AsAckDeleteResultArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisStream.EntryItem[]?>(SharpTask task)
#else
    public static implicit operator Task<RedisStream.EntryItem[]>(SharpTask task)
#endif
        => task._task.AsEntryItemArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisStream.XAutoClaimResult>(SharpTask task)
    {
        return task._task.AsXAutoClaimResult();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisStream.XPendingSummary>(SharpTask task)
    {
        return task._task.AsXPendingSummary();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisStream.XPendingItem[]?>(SharpTask task)
#else
    public static implicit operator Task<RedisStream.XPendingItem[]>(SharpTask task)
#endif
        => task._task.AsXPendingItemArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisStream.EntryItem?>(SharpTask task)
        => task._task.AsEntryItem();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisStream.ConsumerInfo[]?>(SharpTask task)
#else
    public static implicit operator Task<RedisStream.ConsumerInfo[]>(SharpTask task)
#endif
        => task._task.AsConsumerInfoArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisStream.GroupInfo[]?>(SharpTask task)
#else
    public static implicit operator Task<RedisStream.GroupInfo[]>(SharpTask task)
#endif
        => task._task.AsGroupInfoArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisStream.StreamInfo>(SharpTask task)
        => task._task.AsStreamInfo();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisBloom.BloomInfo>(SharpTask task)
        => task._task.AsBloomInfo();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisBloom.AddStatus[]>(SharpTask task)
        => task._task.AsBfInsertStatusArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisBloom.AddStatus>(SharpTask task)
        => task._task.AsBfInsertStatus();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisBloom.ScanDumpResult>(SharpTask task)
        => task._task.AsBfScanDumpResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisCuckoo.InsertNxStatus[]>(SharpTask task)
        => task._task.AsCfInsertNxStatusArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisCuckoo.InsertNxStatus>(SharpTask task)
        => task._task.AsCfInsertNxStatus();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisCuckoo.ScanDumpResult>(SharpTask task)
        => task._task.AsCfScanDumpResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisCuckoo.CuckooInfo>(SharpTask task)
        => task._task.AsCuckooInfo();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<long[]?>(SharpTask task)
#else
    public static implicit operator Task<long[]>(SharpTask task)
#endif
        => task._task.AsInt64Array();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisCountMinSketch.SketchInfo>(SharpTask task)
        => task._task.AsSketchInfo();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisCountMinSketch.IncrByResult>(SharpTask task)
        => task._task.AsSketchIncrByResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisCountMinSketch.IncrByResult[]>(SharpTask task)
        => task._task.AsSketchIncrByResultArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisCluster.SlotInfo[]>(SharpTask task)
        => task._task.AsClusterSlotInfoArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisKeyspace.WaitAofResult>(SharpTask task)
        => task._task.AsWaitAofResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisKeyspace.ScanResult>(SharpTask task)
        => task._task.AsScanResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisServer.AclUserInfo>(SharpTask task)
        => task._task.AsAclUserInfo();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisServer.AclLogEntry[]?>(SharpTask task)
#else
    public static implicit operator Task<RedisServer.AclLogEntry[]>(SharpTask task)
#endif
        => task._task.AsAclLogEntryArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisServer.HotKeysInfo?>(SharpTask task)
#else
    public static implicit operator Task<RedisServer.HotKeysInfo>(SharpTask task)
#endif
        => task._task.AsHotKeysInfo();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisServer.BackupStatusInfo>(SharpTask task)
        => task._task.AsBackupStatusInfo();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisServer.CommandMetadata?[]>(SharpTask task)
#else
    public static implicit operator Task<RedisServer.CommandMetadata[]>(SharpTask task)
#endif
        => task._task.AsCommandMetadataArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<Dictionary<string, RedisServer.CommandDocumentation>>(SharpTask task)
        => task._task.AsCommandDocumentation();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisServer.CommandKeyFlags[]>(SharpTask task)
        => task._task.AsCommandKeyFlagsArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<Dictionary<string, string>?>(SharpTask task)
#else
    public static implicit operator Task<Dictionary<string, string>>(SharpTask task)
#endif
        => task._task.AsStringDictionary();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisServer.ModuleInfo[]>(SharpTask task)
        => task._task.AsModuleInfoArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    public static implicit operator Task<RedisArray.ArItem[]?>(SharpTask task)
#else
    public static implicit operator Task<RedisArray.ArItem[]>(SharpTask task)
#endif
        => task._task.AsGrepResultArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisServer.SlowLogEntry[]>(SharpTask task)
        => task._task.AsSlowLogEntryArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisServer.TimeResult>(SharpTask task)
        => task._task.AsTimeResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisServer.RoleResult>(SharpTask task)
        => task._task.AsRoleResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisConnection.TrackingInfo>(SharpTask task)
        => task._task.AsTrackingInfo();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisConnection.HelloResult>(SharpTask task)
        => task._task.AsHelloResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisArray.ArInfoResult>(SharpTask task)
        => task._task.AsArInfoResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisSet.SetExpireResult>(SharpTask task)
        => task._task.AsSetExpireResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisSet.SetExpireResult[]>(SharpTask task)
        => task._task.AsSetExpireResultArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisSet.SetPersistResult>(SharpTask task)
        => task._task.AsSetPersistResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<RedisSet.SetPersistResult[]>(SharpTask task)
        => task._task.AsSetPersistResultArray();
}
