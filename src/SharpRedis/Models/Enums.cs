#pragma warning disable IDE0130

namespace SharpRedis
{
    public enum RedisServerMode
    {
        /// <summary>
        /// Standalone mode
        /// <para>单机模式</para>
        /// </summary>
        Standalone,

        /// <summary>
        /// master/slave mode
        /// <para>主从模式</para>
        /// </summary>
        MasterSlave,

        /// <summary>
        /// Sentinel mode
        /// <para>哨兵模式</para>
        /// </summary>
        Sentinel,

        /// <summary>
        /// Cluster mode
        /// <para>集群模式</para>
        /// </summary>
        Cluster,
    }

    internal enum ConnectionType
    {
        Master,

        Slave,

        Sentinel,

        PubSub,
    }

    internal enum RespVersion
    {
        V2 = 2,

        V3 = 3,
    }

    internal enum ResultType : ulong
    {
        Object = 1,

        String = 1 << 1,

        Int32 = 1 << 2,

        Int64 = 1 << 3,

        UInt64 = 1 << 4,

        Boolean = 1 << 5,

        Nullable = 1 << 6,

        Dictionary = 1 << 7,

        Array = 1 << 8,

        Lcs = 1 << 9,

        KeyValuePair = 1 << 10,

        MemberScoreValue = 1 << 11,

        ScoreRankValue = 1 << 12,

        Number = 1 << 13,

        Enum = 1 << 14,

        Scan = 1 << 15,

        Bytes = 1 << 16,

        KeyValuePairArray = 1 << 17,

        Double = 1 << 18,

        Coordinate = 1 << 19,

        GeoRadiusValue = 1 << 20,

        Stream = 1 << 21,

        XAutoClaimValue = 1 << 22,

        XInfoConsumersValue = 1 << 23,

        XInfoGroupsValue = 1 << 24,

        XInfoStreamValue = 1 << 25,

        XInfoStreamFullValue = 1 << 26,

        FunctionInfoValue = 1 << 27,

        FunctionStatsValue = 1 << 28,
    }

    internal enum CommandMode : uint
    {
        WithoutResult = 1,

        WithoutActiveTime = 1 << 1,

        WithClientSideCache = 1 << 2,

        Read = 1 << 3,

        Write = 1 << 4,

        Connection = 1 << 5,

        Server = 1 << 6,

        Pub = 1 << 7,

        Sub = 1 << 8,

        UnSub = 1 << 9,

        Transaction = 1 << 10,

        Sentinel = 1 << 11,

        Script = 1 << 12,

        WithBlock = 1 << 13,

        Keyspace = 1 << 14,
    }

    public enum NxXx
    {
        None,

        Nx,

        Xx,
    }

    public enum GtLt
    {
        None,

        Gt,

        Lt
    }

    public enum OrderType
    {
        Default,

        Asc,

        Desc,
    }

    internal enum ChannelModeEnum
    {
        Default,

        Shard,
    }

    internal enum ClientSideCachingDefaultMatchType
    {
        Unmatch = 1,

        Include = 1 << 1,

        Exclude = 1 << 2,
    }

    /// <summary>
    /// Redis返回值数据格式
    /// <para>Redis返回值数据格式</para>
    /// </summary>
    public enum ResultDataType
    {
        /// <summary>
        /// The default value is usually string
        /// <para>默认, 一般解析为string</para>
        /// </summary>
        Default,

        /// <summary>
        /// byte[]
        /// <para>字节</para>
        /// </summary>
        Bytes,
    }

    /// <summary>
    /// Redis client cache mode
    /// <para><see href="https://redis.io/docs/manual/client-side-caching/">Click Me to view the official Redis documentation</see></para>
    /// <para>The Broadcasting mode is recommended</para>
    /// <para>Redis客户端缓存模式</para>
    /// <para><see href="https://redis.io/docs/manual/client-side-caching/">详细信息点我查看Redis官方文档</see></para>
    /// <para>推荐使用广播模式</para>
    /// </summary>
    public enum ClientSideCachingMode
    {
        /// <summary>
        /// In the default mode, the server remembers what keys a given client accessed, and sends invalidation messages when the same keys are modified.
        /// <para>This costs memory in the server side, but sends invalidation messages only for the set of keys that the client might have in memory.</para>
        /// <para><see href="https://redis.io/docs/manual/client-side-caching/#the-redis-implementation-of-client-side-caching">Click Me to view the official Redis documentation</see></para>
        /// <para>默认模式, 服务端记住给定客户端访问的密钥，并在修改相同密钥时发送无效消息</para>
        /// <para>这会消耗服务器端的内存，但只会为客户端内存中可能存在的Key发送无效消息。</para>
        /// <see href="https://redis.io/docs/manual/client-side-caching/#the-redis-implementation-of-client-side-caching">详细信息点我查看Redis官方文档</see>
        /// </summary>
        Default,

        /// <summary>
        /// Broadcasting mode
        /// <para><see href="https://redis.io/docs/manual/client-side-caching/#broadcasting-mode">Click Me to view the official Redis documentation</see></para>
        /// <para>广播模式</para>
        /// <para><see href="https://redis.io/docs/manual/client-side-caching/#broadcasting-mode">详细信息点我查看Redis官方文档</see></para>
        /// </summary>
        Broadcasting,
    }

    public enum FlushingMode
    {
        /// <summary>
        /// Default
        /// </summary>
        None,

        /// <summary>
        /// Flushes the database synchronously
        /// <para>Starting with Redis version 6.2.0: Added the SYNC flushing mode modifier.</para>
        /// <para>同步刷新</para>
        /// <para>Redis6.2.0及以后版本支持</para>
        /// </summary>
        Sync,

        /// <summary>
        /// Flushes the database asynchronously
        /// <para>Starting with Redis version 4.0.0: Added the ASYNC flushing mode modifier.</para>
        /// <para>异步刷新</para>
        /// <para>Redis 4.0.0及以后版本支持</para>
        /// </summary>
        Async,
    }

    public enum BeforeAfter
    {
        Before,

        After,
    }

    public enum LeftRight
    {
        Left,

        Right,
    }

    public enum ByteBit
    {
        None,

        Byte,

        Bit,
    }

    public enum BitOperation
    {
        And,

        Or,

        Xor,

        Not,
    }

    public enum BitFieldArgType
    {
        Get,

        Set,

        Overflow,

        Increment,
    }

    public enum HashFieldExpirationStatus
    {
        /// <summary>
        /// No such field exists in the provided hash key, or the provided key does not exist.
        /// <para>Hash不存在或Hash中的field不存在</para>
        /// </summary>
        KeyOrFieldNotExist = -2,

        /// <summary>
        /// The specified NX | XX | GT | LT condition has not been met.
        /// <para>指定的 NX | XX | GT | LT 条件不满足</para>
        /// </summary>
        ConditionNotMet = 0,

        /// <summary>
        /// The expiration time was set/updated
        /// <para>成功为field更新或设置了过期时间</para>
        /// </summary>
        ExpirationSet = 1,

        /// <summary>
        /// HEXPIRE/HPEXPIRE is called with 0 seconds/milliseconds or HEXPIREAT/HPEXPIREAT is called with a past Unix time in seconds/milliseconds.
        /// <para>为field设置了过期时间为0, 或者设置的过期时间已小于当前时间(设置即过期状态)</para>
        /// </summary>
        ImmediateExpiration = 2,
    }

    public enum HashPersistStatus
    {
        /// <summary>
        /// No such field exists in the provided hash key, or the provided key does not exist
        /// <para>Hash Key不存在或field不存在</para>
        /// </summary>
        KeyOrFieldNotExist = -2,

        /// <summary>
        /// The field exists but has no associated expiration set
        /// <para>field没有设置过期时间</para>
        /// </summary>
        FieldNoExpiration = -1,

        /// <summary>
        /// The expiration was removed
        /// <para>成功移除了field的过期时间, 设置为永不过期</para>
        /// </summary>
        ExpirationRemoved = 1,
    }

    public enum Aggregate
    {
        Sum,

        Min,

        Max,
    }

    public enum MaxMin
    {
        Min,

        Max,
    }

    public enum ZRangeBy
    {
        ByScore,

        ByLex,
    }

    /// <summary>
    /// Script debug mode
    /// </summary>
    public enum ScriptDebugMode
    {
        /// <summary>
        /// Enable non-blocking asynchronous debugging of Lua scripts (changes are discarded)
        /// <para>启用 Lua 脚本的非阻塞异步调试（更改将被丢弃）</para>
        /// </summary>
        Yes,

        /// <summary>
        /// Enable blocking synchronous debugging of Lua scripts (saves changes to data)
        /// <para>启用 Lua 脚本的阻塞同步调试（保存对数据的更改）</para>
        /// </summary>
        Sync,

        /// <summary>
        /// Disables scripts debug mode
        /// <para>禁用脚本调试模式</para>
        /// </summary>
        No,
    }

    /// <summary>
    /// distance unit
    /// <para>距离单位</para>
    /// </summary>
    public enum DistanceUnit
    {
        /// <summary>
        /// m for meters
        /// <para>米</para>
        /// </summary>
        m,

        /// <summary>
        /// km for kilometers
        /// <para>千米</para>
        /// </summary>
        km,

        /// <summary>
        /// mi for miles
        /// <para>英里</para>
        /// </summary>
        mi,

        /// <summary>
        /// ft for feet
        /// <para>英尺</para>
        /// </summary>
        ft,
    }

    /// <summary>
    /// Stream Trim Mode
    /// </summary>
    public enum StreamTrimMode : short
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// Limit the maximum length of a stream. When the maximum length is reached, the oldest entry in the stream is deleted
        /// <para>限制流的最大长度。当达到最大长度时，流中的最旧条目将被删除</para>
        /// </summary>
        MaxLen,

        /// <summary>
        /// Restricts the entry with the smallest ID in the stream. Entries smaller than the specified ID will be deleted
        /// <para>Available since: 6.2.0</para>
        /// <para>限制流中最小ID的条目。比指定的ID小的条目将被删除</para>
        /// <para>支持此参数的Redis版本: 6.2.0+</para>
        /// </summary>
        MinId,
    }

    /// <summary>
    /// Stream Trim Strategy
    /// </summary>
    public enum StreamTrimStrategy : short
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// =
        /// <para>(Exact match): Force the length of the stream to be exactly equal to threshold (when using MAXLEN) or delete the ID to be less than or equal to threshold (when using MINID).</para>
        /// <para>Available since: 6.2.0</para>
        /// <para> (精确匹配): 强制流的长度精确地等于 threshold（当使用 MAXLEN）或删除ID小于等于 threshold（当使用 MINID）。</para>
        /// <para>支持此参数的Redis版本: 6.2.0+</para>
        /// </summary>
        Exact,

        /// <summary>
        /// ~
        /// <para>(near match): The length of the stream is limited to about threshold, possibly a little more. This is to optimize performance and reduce blocking operations. Redis does not strictly check the number of entries when deleting them, but deletes them in bulk. This is recommended to improve performance.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para> (接近匹配): 流的长度大约限制为 threshold，可能稍微多一点。这是为了优化性能，减少阻塞操作。Redis在删除条目时不会严格检查条目数目，而是批量删除。推荐使用这种方式来提高性能。</para>
        /// <para>支持此参数的Redis版本: 6.2.0+</para>
        /// </summary>
        Approx,
    }

    public enum FunctionRestoreMode : short
    {
        /// <summary>
        /// Appends the restored libraries to the existing libraries and aborts on collision. This is the default policy
        /// <para>将恢复的函数库追加到现有库中，并在冲突时中止。这是默认策略。</para>
        /// </summary>
        Append,

        /// <summary>
        /// Deletes all existing libraries before restoring the payload
        /// <para>恢复之前先删除所有函数库</para>
        /// </summary>
        Flush,

        /// <summary>
        /// Appends the restored libraries to the existing libraries, replacing any existing ones in case of name collisions. Note that this policy doesn't prevent function name collisions, only libraries
        /// <para>将恢复的库附加到现有库，在名称冲突的情况下替换任何现有库。请注意，此策略不能防止函数名称冲突，只能防止库冲突</para>
        /// </summary>
        Replace,
    }
}
