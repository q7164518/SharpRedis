using SharpRedis.Modules;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SharpRedis.Extensions;

internal static class TaskExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisNumber> AsRedisNumber(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<long> AsInt64(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<long?> AsNullableInt64(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<int> AsInt32(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<double> AsDouble(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<double?> AsNullableDouble(this Task<RedisResult> task)
    {
        double? result = await task.ConfigureAwait(false);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<bool> AsBool(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<bool[]> AsBoolArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<string?> AsString(this Task<RedisResult> task)
#else
    internal static async Task<string> AsString(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<string?[]?> AsStringArray(this Task<RedisResult> task)
#else
    internal static async Task<string[]> AsStringArray(this Task<RedisResult> task)
#endif
    {
        return await task.ConfigureAwait(false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisResult[]?> AsArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisResult[]> AsArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisString.RedisLcsResult> AsLcsResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<Dictionary<RedisResult, RedisResult>?> AsDictionary(this Task<RedisResult> task)
#else
    internal static async Task<Dictionary<RedisResult, RedisResult>> AsDictionary(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<KeyValuePair<RedisResult, RedisNumber>[]?> AsKeyNumberValueArray(this Task<RedisResult> task)
#else
    internal static async Task<KeyValuePair<RedisResult, RedisNumber>[]> AsKeyNumberValueArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<KeyValuePair<RedisResult, RedisNumber>?> AsKeyNumberValue(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisHash.HashScanResult> AsHashScan(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisHash.HashScanNoValuesResult> AsHashScanNoValues(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisList.ListPopResult> AsListPop(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisList.ListBlockPopResult> AsListBlockPop(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisNumber[]?> AsNumberArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisNumber[]> AsNumberArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisSet.SetScanResult> AsSetScan(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisSortedSet.SortedSetMPopResult> AsZMPop(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    internal static async Task<RedisSortedSet.SortedSetPopResult> AsZPop(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisSortedSet.SortedSetRankResult> AsZRank(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisString.IncrExResult> AsIncrExResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisGeospatial.GeoCoordinate?> AsGeoCoordinate(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisGeospatial.GeoCoordinate?[]> AsGeoCoordinateArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisGeospatial.GeoSearchResult[]?> AsGeoRadiusResultArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisGeospatial.GeoSearchResult[]> AsGeoRadiusResultArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<byte[]?> AsBytes(this Task<RedisResult> task)
#else
    internal static async Task<byte[]> AsBytes(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisFunction.ListResult[]?> AsLibraryInfoArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisFunction.ListResult[]> AsLibraryInfoArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisFunction.StatsResult> AsFunctionStatsResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisStream.XReadResult?> AsXReadResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisStream.XReadResult[]?> AsXReadResultArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisStream.XReadResult[]> AsXReadResultArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisStream.DeleteResult> AsAckDeleteResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisStream.DeleteResult[]> AsAckDeleteResultArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisStream.EntryItem[]?> AsEntryItemArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisStream.EntryItem[]> AsEntryItemArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisStream.XAutoClaimResult> AsXAutoClaimResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisStream.XPendingSummary> AsXPendingSummary(this Task<RedisResult> task)
       => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisStream.XPendingItem[]?> AsXPendingItemArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisStream.XPendingItem[]> AsXPendingItemArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisStream.EntryItem?> AsEntryItem(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisStream.ConsumerInfo[]?> AsConsumerInfoArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisStream.ConsumerInfo[]> AsConsumerInfoArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisStream.GroupInfo[]?> AsGroupInfoArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisStream.GroupInfo[]> AsGroupInfoArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisStream.StreamInfo> AsStreamInfo(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisBloom.BloomInfo> AsBloomInfo(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisBloom.AddStatus[]> AsBfInsertStatusArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisBloom.AddStatus> AsBfInsertStatus(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisBloom.ScanDumpResult> AsBfScanDumpResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisCuckoo.InsertNxStatus[]> AsCfInsertNxStatusArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisCuckoo.InsertNxStatus> AsCfInsertNxStatus(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisCuckoo.ScanDumpResult> AsCfScanDumpResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisCuckoo.CuckooInfo> AsCuckooInfo(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<long[]?> AsInt64Array(this Task<RedisResult> task)
#else
    internal static async Task<long[]> AsInt64Array(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisCountMinSketch.SketchInfo> AsSketchInfo(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisCountMinSketch.IncrByResult> AsSketchIncrByResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisCountMinSketch.IncrByResult[]> AsSketchIncrByResultArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisCluster.SlotInfo[]> AsClusterSlotInfoArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisKeyspace.WaitAofResult> AsWaitAofResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisKeyspace.ScanResult> AsScanResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisServer.AclUserInfo> AsAclUserInfo(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisServer.AclLogEntry[]?> AsAclLogEntryArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisServer.AclLogEntry[]> AsAclLogEntryArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisServer.HotKeysInfo?> AsHotKeysInfo(this Task<RedisResult> task)
#else
    internal static async Task<RedisServer.HotKeysInfo> AsHotKeysInfo(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisServer.BackupStatusInfo> AsBackupStatusInfo(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisServer.CommandMetadata?[]> AsCommandMetadataArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisServer.CommandMetadata[]> AsCommandMetadataArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<Dictionary<string, RedisServer.CommandDocumentation>> AsCommandDocumentation(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisServer.CommandKeyFlags[]> AsCommandKeyFlagsArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<Dictionary<string, string>?> AsStringDictionary(this Task<RedisResult> task)
#else
    internal static async Task<Dictionary<string, string>> AsStringDictionary(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisServer.ModuleInfo[]> AsModuleInfoArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    internal static async Task<RedisArray.ArItem[]?> AsGrepResultArray(this Task<RedisResult> task)
#else
    internal static async Task<RedisArray.ArItem[]> AsGrepResultArray(this Task<RedisResult> task)
#endif
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisServer.SlowLogEntry[]> AsSlowLogEntryArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisServer.TimeResult> AsTimeResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisServer.RoleResult> AsRoleResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisConnection.TrackingInfo> AsTrackingInfo(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    internal static async Task<RedisConnection.HelloResult> AsHelloResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisArray.ArInfoResult> AsArInfoResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisSet.SetExpireResult> AsSetExpireResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisSet.SetExpireResult[]> AsSetExpireResultArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisSet.SetPersistResult> AsSetPersistResult(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<RedisSet.SetPersistResult[]> AsSetPersistResultArray(this Task<RedisResult> task)
        => await task.ConfigureAwait(false);

    #region WaitAsync
#if !NET6_0_OR_GREATER
    internal static async Task<RedisResult> WaitAsync(this Task<RedisResult> task, System.TimeSpan timeout, System.Threading.CancellationToken cancellationToken)
    {
        if (task.IsCompleted) return await task.ConfigureAwait(false);
        using var cts = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, default);
        cts.CancelAfter(timeout);
        var timeoutTask = Task.Delay(System.Threading.Timeout.Infinite, cts.Token);
        if (object.ReferenceEquals(task, await Task.WhenAny(task, timeoutTask).ConfigureAwait(false)))
        {
            cts.Cancel();
            return await task.ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();
        throw new System.TimeoutException();
    }
#endif
    #endregion
}
