using System;

namespace SharpRedis.Enums;

[Flags]
internal enum CommandMode : ulong
{
    WithoutResult = 1,

    WithoutActiveTime = 1 << 1,

    WithBlocking = 1 << 2,

    WithWrite = 1 << 3,

    WithRead = 1 << 4,

    WithClientCaching = 1 << 5,

    WithPipelining = 1 << 6,

    String = 1 << 7,

    Hash = 1 << 8,

    List = 1 << 9,

    Set = 1 << 10,

    ZSet = 1 << 11,

    Geo = 1 << 12,

    Stream = 1 << 13,

    Bitmap = 1 << 14,

    Bloom = 1 << 15,

    HyperLogLog = 1 << 16,

    JSON = 1 << 17,

    LuaScript = 1 << 18,

    Function = 1 << 19,

    Cuckoo = 1 << 20,

    ConnectionManagement = 1 << 21,

    ServerManagement = 1 << 22,

    Pub = 1 << 23,

    Sub = 1 << 24,

    PSub = 1 << 25,

    Transaction = 1 << 26,

    Sentinel = 1 << 27,

    Keyspace = 1 << 28,

    ClusterManagement = 1 << 29,

    CountMinSketch = 1UL << 30,

    AutoSuggest = 1UL << 31,

    TDigest = 1UL << 32,

    TimeSeries = 1UL << 33,

    TopK = 1UL << 34,

    Vector = 1UL << 35,

    Search = 1UL << 36,

    /// <summary>
    /// The temporary connection must be closed after executing this command.
    /// </summary>
    DoNotReuseConnection = 1UL << 37,

    Call = 1UL << 38,

    Shutdown = 1UL << 39,

    Array = 1UL << 40,
}
