using SharpRedis.Modules;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis;

/// <summary>
/// Redis client interface
/// <para>Redis客户端接口</para>
/// </summary>
public interface IRedis : IDisposable
{
    /// <summary>
    /// Gets the current Redis service mode
    /// <para>获得当前Redis服务模式</para>
    /// </summary>
    RedisServerMode RedisMode { get; }

    /// <summary>
    /// Gets the index of the currently connected database.
    /// <para>获得当前连接到的数据库的下标</para>
    /// </summary>
    int DataBase { get; }

    /// <summary>
    /// Gets an object of type Redis connection
    /// <para>获得操作Redis连接的对象</para>
    /// </summary>
    RedisConnection Connection { get; }

    /// <summary>
    /// Gets the operation property of the Redis Strings type
    /// <para>If you are new to strings, see <see href="https://redis.io/docs/data-types/strings">Introduction to Redis Strings</see></para>
    /// <para>Redis String类型操作属性</para>
    /// <para>如果你不太懂String类型, 请<see href="https://redis.io/docs/data-types/strings">查看Strings文档</see></para>
    /// </summary>
    RedisString String { get; }

    /// <summary>
    /// Gets the operation property of the Redis Server type
    /// <para>获得Redis 服务操作属性</para>
    /// </summary>
    RedisServer Server { get; }

    /// <summary>
    /// Gets the operation property of Redis Key
    /// <para>Redis Key操作属性</para>
    /// </summary>
    RedisKeyspace Key { get; }

    /// <summary>
    /// Gets the operation property of Redis Hash
    /// <para>If you are new to hashes, see <see href="https://redis.io/docs/data-types/hashes">Introduction to Redis hashes</see></para>
    /// <para>Redis Hash操作属性</para>
    /// <para>如果你不太懂hashes类型, 请<see href="https://redis.io/docs/data-types/hashes">查看hashes文档</see></para>
    /// </summary>
    RedisHash Hash { get; }

    /// <summary>
    /// Gets the operation property of Redis List
    /// <para>If you are new to lists, see <see href="https://redis.io/docs/data-types/lists">Introduction to Redis lists</see></para>
    /// <para>Redis List操作属性</para>
    /// <para>如果你不太懂lists类型, 请<see href="https://redis.io/docs/data-types/lists">查看lists文档</see></para>
    /// </summary>
    RedisList List { get; }

    /// <summary>
    /// Gets the operation property of Redis Set
    /// <para>If you are new to sets, see <see href="https://redis.io/docs/data-types/sets">Introduction to Redis sets</see></para>
    /// <para>Redis Set操作属性</para>
    /// <para>如果你不太懂Sets类型, 请<see href="https://redis.io/docs/data-types/sets">查看Sets文档</see></para>
    /// </summary>
    RedisSet Set { get; }

    /// <summary>
    /// Gets the operation property of Redis SortedSet
    /// <para>If you are new to sorted sets, see <see href="https://redis.io/docs/data-types/sorted-sets">Introduction to Redis sorted sets</see></para>
    /// <para>Redis SortedSet操作属性</para>
    /// <para>如果你不太懂sorted sets类型, 请<see href="https://redis.io/docs/data-types/sorted-sets">查看sorted sets文档</see></para>
    /// </summary>
    RedisSortedSet SortedSet { get; }

    /// <summary>
    /// Gets the subscribe publish action object
    /// <para>If you are new to Pub/sub, see <see href="https://redis.io/docs/develop/pubsub/">Introduction to Redis Pub/sub</see></para>
    /// <para>Redis订阅发布操作属性</para>
    /// <para>如果你不太懂Pub/sub类型, 请<see href="https://redis.io/docs/develop/pubsub/">查看Pub/sub文档</see></para>
    /// </summary>
    RedisPubSub PubSub { get; }

    /// <summary>
    /// Gets the operation property of Redis Stream
    /// <para>If you are new to streams, see <see href="https://redis.io/docs/data-types/streams">Introduction to Redis Streams</see></para>
    /// <para>Redis Stream操作属性</para>
    /// <para>如果你不太懂Stream类型, 请<see href="https://redis.io/docs/data-types/streams">查看Stream文档</see></para>
    /// </summary>
    RedisStream Stream { get; }

    /// <summary>
    /// Gets the operation property of Redis HyperLogLog
    /// <para>If you are new to HyperLogLog, see <see href="https://redis.io/docs/develop/data-types/probabilistic/hyperloglogs/">Introduction to Redis HyperLogLog</see></para>
    /// <para>Redis HyperLogLog操作属性</para>
    /// <para>如果你不太懂HyperLogLog类型, 请<see href="https://redis.io/docs/develop/data-types/probabilistic/hyperloglogs/">查看HyperLogLog文档</see></para>
    /// </summary>
    RedisHyperLogLog HyperLogLog { get; }

    /// <summary>
    /// Gets the operation property of Redis Bitmap
    /// <para>If you are new to bitmaps, see <see href="https://redis.io/docs/develop/data-types/strings/bitmaps">Introduction to Redis bitmaps</see></para>
    /// <para>Redis Bitmap(位图)操作属性</para>
    /// <para>如果你不太懂bitmaps类型, 请<see href="https://redis.io/docs/develop/data-types/strings/bitmaps">查看bitmaps文档</see></para>
    /// </summary>
    RedisBitmap Bitmap { get; }

    /// <summary>
    /// Gets the operation property of Redis Geospatial
    /// <para>If you are new to geospatial, see <see href="https://redis.io/docs/data-types/geospatial">Introduction to Redis Geospatial</see></para>
    /// <para>Redis Geospatial操作属性</para>
    /// <para>如果你不太懂Geospatial类型, 请<see href="https://redis.io/docs/data-types/geospatial">查看Geospatial文档</see></para>
    /// </summary>
    RedisGeospatial Geo { get; }

    /// <summary>
    /// Gets the operation property of Redis lua script
    /// <para>Redis Lua脚本操作属性</para>
    /// </summary>
    RedisScript Script { get; }

    /// <summary>
    /// Gets the operation property of Redis functions
    /// <para>Redis 函数操作属性</para>
    /// </summary>
    RedisFunction Func { get; }

    /// <summary>
    /// Gets the operation property of Redis Cluster
    /// <para>Redis集群操作属性</para>
    /// </summary>
    RedisCluster Cluster { get; }

    /// <summary>
    /// Gets the operation property of the Redis extension module
    /// <para><b>ℹ️By using the corresponding extension module, you need to enable the corresponding module or use Redis version 8.0 or above. Otherwise, the operation will result in an exception.</b></para>
    /// <para>获得Redis扩展模块操作属性</para>
    /// <para><b>ℹ️使用对应的扩展模块, 需要按照启用对应的模块, 或者使用Redis 8.0及以上版本, 否则操作都会引发异常</b></para>
    /// </summary>
    RedisModules Modules { get; }

    /// <summary>
    /// Gets the operation property of the Redis arryas type
    /// <para>If you are new to arrays, see <see href="https://redis.io/docs/develop/data-types/arrays">Introduction to Redis arrays</see></para>
    /// <para><b>ℹ️Note: Array is a new data type that is currently in preview and may be subject to change.</b></para>
    /// <para>获得Redis arryas(数组)类型操作属性</para>
    /// <para>如果你不太懂Arrays类型, 请<see href="https://redis.io/docs/develop/data-types/arrays">查看Arrays文档</see></para>
    /// <para><b>ℹ️提示: 数组(Arrays)是一种新的数据类型, 目前在Redis中处于预览阶段, 未来可能有会有所更改</b></para>
    /// </summary>
    RedisArray Array { get; }

    /// <summary>
    /// Executing commands that are not yet supported by <b>SharpRedis</b>
    /// <para>If the command involves Key operations, please replace it with KEYS[index], and then pass the corresponding key as the "keys" parameter.</para>
    /// <para>执行 <b>SharpRedis</b> 暂未适配的命令</para>
    /// <para>如果命令涉及Key操作, 请用KEYS[index]代替, 然后用keys参数传入对应的key</para>
    /// </summary>
    /// <param name="command">The commands to be executed, such as ["GET", "KEYS[0]"], ["SET", "KEYS[0]", "hello"]
    /// <para>要执行的命令, 如["GET", "KEYS[0]"], ["SET", "KEYS[0]", "hello"]</para>
    /// </param>
    /// <param name="keys">The corresponding Keys for the command, one or more
    /// <para>命令对应的key, 一个或多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    RedisResult Call(SharpValueSpan command, SharpValueSpan keys = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executing commands that are not yet supported by <b>SharpRedis</b>
    /// <para>If the command involves Key operations, please replace it with KEYS[index], and then pass the corresponding key as the "keys" parameter.</para>
    /// <para>执行 <b>SharpRedis</b> 暂未适配的命令</para>
    /// <para>如果命令涉及Key操作, 请用KEYS[index]代替, 然后用keys参数传入对应的key</para>
    /// </summary>
    /// <param name="command">The commands to be executed, such as ["GET", "KEYS[0]"], ["SET", "KEYS[0]", "hello"]
    /// <para>要执行的命令, 如["GET", "KEYS[0]"], ["SET", "KEYS[0]", "hello"]</para>
    /// </param>
    /// <param name="keys">The corresponding Keys for the command, one or more
    /// <para>命令对应的key, 一个或多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    Task<RedisResult> CallAsync(SharpValueSpan command, SharpValueSpan keys = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);
}
