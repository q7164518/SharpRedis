#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable IDE0063
#endif
#if !LOW_NET
using System.Threading;
#else
#pragma warning disable IDE0130
#endif
using System;
using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using System.Collections.Generic;

namespace SharpRedis
{
    /// <summary>
    /// Redis Stream action class
    /// <para>Redis Stream操作类</para>
    /// </summary>
    public sealed partial class RedisStream : BaseType
    {
        internal RedisStream(BaseCall call) : base(call)
        { }

        /// <summary>
        /// Appends the specified stream entry to the stream at the specified key
        /// <para>Available since: 5.0.0</para>
        /// <para>将内容添加到指定Key的Stream中</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="milliseconds">The first part of the unique ID, the timestamp. If null is passed, Redis will automatically generate a unique ID
        /// <para>唯一ID的第一部分, 时间戳, 毫秒级. 如果传入null, Redis将自动生成唯一ID</para>
        /// </param>
        /// <param name="sequenceNumber">The second part of the unique ID is a serial number. If null is passed, Redis will automatically generate the serial number. Redis7.0.0 and later support automatic serial number generation
        /// <para>唯一ID的第二部分, 是一个序列号. 如果传入null, Redis将自动生成序列号. Redis7.0.0及以上版本才支持自动生成序列号</para>
        /// </param>
        /// <param name="nomkStream">If the key does not exist, as a side effect of running this command the key is created with a stream value. The creation of stream's key can be disabled with the NOMKSTREAM option
        /// <para>Available since: 6.2.0</para>
        /// <para>是否禁用Key不存在的时候创建Stream. 默认不禁止. 需要Redis 6.2.0+才支持设置为true</para>
        /// </param>
        /// <param name="trimOptions">Stream Trim Options
        /// <para>Stream修剪参数, 默认不修剪</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A unique ID that is automatically generated. Null reply: if the NOMKSTREAM option is given and the key doesn't exist
        /// <para>自动生成的唯一ID. 如果NOMKSTREAM设置为true, 则Key不存在的时候返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? XAdd(string key, string field, string value, ulong? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions? trimOptions = null, CancellationToken cancellationToken = default)
#else
        public string XAdd(string key, string field, string value, ulong? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions trimOptions = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StreamCommands.XAdd(key, nomkStream, trimOptions, milliseconds, sequenceNumber, field, value), cancellationToken);
        }

        /// <summary>
        /// Appends the specified stream entry to the stream at the specified key.
        /// <para>Available since: 5.0.0</para>
        /// <para>将内容添加到指定Key的Stream中</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="milliseconds">The first part of the unique ID, the timestamp. If null is passed, Redis will automatically generate a unique ID
        /// <para>唯一ID的第一部分, 时间戳, 毫秒级. 如果传入null, Redis将自动生成唯一ID</para>
        /// </param>
        /// <param name="sequenceNumber">The second part of the unique ID is a serial number. If null is passed, Redis will automatically generate the serial number. Redis7.0.0 and later support automatic serial number generation
        /// <para>唯一ID的第二部分, 是一个序列号. 如果传入null, Redis将自动生成序列号. Redis7.0.0及以上版本才支持自动生成序列号</para>
        /// </param>
        /// <param name="nomkStream">If the key does not exist, as a side effect of running this command the key is created with a stream value. The creation of stream's key can be disabled with the NOMKSTREAM option
        /// <para>Available since: 6.2.0</para>
        /// <para>是否禁用Key不存在的时候创建Stream. 默认不禁止. 需要Redis 6.2.0+才支持设置为true</para>
        /// </param>
        /// <param name="trimOptions">Stream Trim Options
        /// <para>Stream修剪参数, 默认不修剪</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A unique ID that is automatically generated. Null reply: if the NOMKSTREAM option is given and the key doesn't exist
        /// <para>自动生成的唯一ID. 如果NOMKSTREAM设置为true, 则Key不存在的时候返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? XAdd(string key, string field, byte[] value, ulong? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions? trimOptions = null, CancellationToken cancellationToken = default)
#else
        public string XAdd(string key, string field, byte[] value, ulong? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions trimOptions = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StreamCommands.XAdd(key, nomkStream, trimOptions, milliseconds, sequenceNumber, field, value), cancellationToken);
        }

        /// <summary>
        /// Appends the specified stream entry to the stream at the specified key
        /// <para>Available since: 5.0.0</para>
        /// <para>将内容添加到指定Key的Stream中</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="fvArray">field-value array
        /// <para>field-value数组</para>
        /// </param>
        /// <param name="milliseconds">The first part of the unique ID, the timestamp. If null is passed, Redis will automatically generate a unique ID
        /// <para>唯一ID的第一部分, 时间戳. 如果传入null, Redis将自动生成唯一ID</para>
        /// </param>
        /// <param name="sequenceNumber">The second part of the unique ID is a serial number. If null is passed, Redis will automatically generate the serial number. Redis7.0.0 and later support automatic serial number generation
        /// <para>唯一ID的第二部分, 是一个序列号. 如果传入null, Redis将自动生成序列号. Redis7.0.0及以上版本才支持自动生成序列号</para>
        /// </param>
        /// <param name="nomkStream">If the key does not exist, as a side effect of running this command the key is created with a stream value. The creation of stream's key can be disabled with the NOMKSTREAM option
        /// <para>Available since: 6.2.0</para>
        /// <para>是否禁用Key不存在的时候创建Stream. 默认不禁止. 需要Redis 6.2.0+才支持设置为true</para>
        /// </param>
        /// <param name="trimOptions">Stream Trim Options
        /// <para>Stream修剪参数, 默认不修剪</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A unique ID that is automatically generated. Null reply: if the NOMKSTREAM option is given and the key doesn't exist
        /// <para>自动生成的唯一ID. 如果NOMKSTREAM设置为true, 则Key不存在的时候返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? XAdd(string key, KeyValuePair<string, string>[] fvArray, DateTimeOffset? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions? trimOptions = null, CancellationToken cancellationToken = default)
#else
        public string XAdd(string key, KeyValuePair<string, string>[] fvArray, DateTimeOffset? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions trimOptions = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StreamCommands.XAdd(key, nomkStream, trimOptions, milliseconds, sequenceNumber, fvArray), cancellationToken);
        }

        /// <summary>
        /// Appends the specified stream entry to the stream at the specified key
        /// <para>Available since: 5.0.0</para>
        /// <para>将内容添加到指定Key的Stream中</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="fvArray">field-value array
        /// <para>field-value数组</para>
        /// </param>
        /// <param name="milliseconds">The first part of the unique ID, the timestamp. If null is passed, Redis will automatically generate a unique ID
        /// <para>唯一ID的第一部分, 时间戳. 如果传入null, Redis将自动生成唯一ID</para>
        /// </param>
        /// <param name="sequenceNumber">The second part of the unique ID is a serial number. If null is passed, Redis will automatically generate the serial number. Redis7.0.0 and later support automatic serial number generation
        /// <para>唯一ID的第二部分, 是一个序列号. 如果传入null, Redis将自动生成序列号. Redis7.0.0及以上版本才支持自动生成序列号</para>
        /// </param>
        /// <param name="nomkStream">If the key does not exist, as a side effect of running this command the key is created with a stream value. The creation of stream's key can be disabled with the NOMKSTREAM option
        /// <para>Available since: 6.2.0</para>
        /// <para>是否禁用Key不存在的时候创建Stream. 默认不禁止. 需要Redis 6.2.0+才支持设置为true</para>
        /// </param>
        /// <param name="trimOptions">Stream Trim Options
        /// <para>Stream修剪参数, 默认不修剪</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A unique ID that is automatically generated. Null reply: if the NOMKSTREAM option is given and the key doesn't exist
        /// <para>自动生成的唯一ID. 如果NOMKSTREAM设置为true, 则Key不存在的时候返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? XAdd(string key, KeyValuePair<string, byte[]>[] fvArray, DateTimeOffset? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions? trimOptions = null, CancellationToken cancellationToken = default)
#else
        public string XAdd(string key, KeyValuePair<string, byte[]>[] fvArray, DateTimeOffset? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions trimOptions = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StreamCommands.XAdd(key, nomkStream, trimOptions, milliseconds, sequenceNumber, fvArray), cancellationToken);
        }

        /// <summary>
        /// Appends the specified stream entry to the stream at the specified key
        /// <para>Available since: 5.0.0</para>
        /// <para>将内容添加到指定Key的Stream中</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="fvArray">field-value array
        /// <para>field-value数组</para>
        /// </param>
        /// <param name="milliseconds">The first part of the unique ID, the timestamp. If null is passed, Redis will automatically generate a unique ID
        /// <para>唯一ID的第一部分, 时间戳, 毫秒级. 如果传入null, Redis将自动生成唯一ID</para>
        /// </param>
        /// <param name="sequenceNumber">The second part of the unique ID is a serial number. If null is passed, Redis will automatically generate the serial number. Redis7.0.0 and later support automatic serial number generation
        /// <para>唯一ID的第二部分, 是一个序列号. 如果传入null, Redis将自动生成序列号. Redis7.0.0及以上版本才支持自动生成序列号</para>
        /// </param>
        /// <param name="nomkStream">If the key does not exist, as a side effect of running this command the key is created with a stream value. The creation of stream's key can be disabled with the NOMKSTREAM option
        /// <para>Available since: 6.2.0</para>
        /// <para>是否禁用Key不存在的时候创建Stream. 默认不禁止. 需要Redis 6.2.0+才支持设置为true</para>
        /// </param>
        /// <param name="trimOptions">Stream Trim Options
        /// <para>Stream修剪参数, 默认不修剪</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A unique ID that is automatically generated. Null reply: if the NOMKSTREAM option is given and the key doesn't exist
        /// <para>自动生成的唯一ID. 如果NOMKSTREAM设置为true, 则Key不存在的时候返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? XAdd(string key, KeyValuePair<string, string>[] fvArray, ulong? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions? trimOptions = null, CancellationToken cancellationToken = default)
#else
        public string XAdd(string key, KeyValuePair<string, string>[] fvArray, ulong? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions trimOptions = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StreamCommands.XAdd(key, nomkStream, trimOptions, milliseconds, sequenceNumber, fvArray), cancellationToken);
        }

        /// <summary>
        /// Appends the specified stream entry to the stream at the specified key
        /// <para>Available since: 5.0.0</para>
        /// <para>将内容添加到指定Key的Stream中</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="fvArray">field-value array
        /// <para>field-value数组</para>
        /// </param>
        /// <param name="milliseconds">The first part of the unique ID, the timestamp. If null is passed, Redis will automatically generate a unique ID
        /// <para>唯一ID的第一部分, 时间戳, 毫秒级. 如果传入null, Redis将自动生成唯一ID</para>
        /// </param>
        /// <param name="sequenceNumber">The second part of the unique ID is a serial number. If null is passed, Redis will automatically generate the serial number. Redis7.0.0 and later support automatic serial number generation
        /// <para>唯一ID的第二部分, 是一个序列号. 如果传入null, Redis将自动生成序列号. Redis7.0.0及以上版本才支持自动生成序列号</para>
        /// </param>
        /// <param name="nomkStream">If the key does not exist, as a side effect of running this command the key is created with a stream value. The creation of stream's key can be disabled with the NOMKSTREAM option
        /// <para>Available since: 6.2.0</para>
        /// <para>是否禁用Key不存在的时候创建Stream. 默认不禁止. 需要Redis 6.2.0+才支持设置为true</para>
        /// </param>
        /// <param name="trimOptions">Stream Trim Options
        /// <para>Stream修剪参数, 默认不修剪</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A unique ID that is automatically generated. Null reply: if the NOMKSTREAM option is given and the key doesn't exist
        /// <para>自动生成的唯一ID. 如果NOMKSTREAM设置为true, 则Key不存在的时候返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? XAdd(string key, KeyValuePair<string, byte[]>[] fvArray, ulong? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions? trimOptions = null, CancellationToken cancellationToken = default)
#else
        public string XAdd(string key, KeyValuePair<string, byte[]>[] fvArray, ulong? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions trimOptions = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StreamCommands.XAdd(key, nomkStream, trimOptions, milliseconds, sequenceNumber, fvArray), cancellationToken);
        }

        /// <summary>
        /// Returns the number of entries inside a stream. If the specified key does not exist the command returns zero, as if the stream was empty. However note that unlike other Redis types, zero-length streams are possible, so you should call TYPE or EXISTS in order to check if a key exists or not.
        /// <para>Available since: 5.0.0</para>
        /// <para>获得Stream中的条目个数. 注意, 如果Key不存在或是空Stream都将返回0. 因此不能通过判断0确定Stream key是否存在</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of entries of the stream at key
        /// <para>Stream里面的内容条目个数</para>
        /// </returns>
        public long XLen(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(StreamCommands.XLen(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Removes the specified entries from a stream, and returns the number of entries deleted
        /// <para>Available since: 5.0.0</para>
        /// <para>删除Stream中指定的项</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="id">ID
        /// <para>内容条目ID</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public long XDel(string key, string id, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(StreamCommands.XDel(key, id), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Removes the specified entries from a stream, and returns the number of entries deleted
        /// <para>Available since: 5.0.0</para>
        /// <para>删除Stream中指定的项</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="ids">ID array
        /// <para>要删除的内容条目ID数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of entries that were deleted
        /// <para>成功删除的内容条目个数</para>
        /// </returns>
        public long XDel(string key, string[] ids, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(StreamCommands.XDel(key, ids), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// The command returns the stream entries matching a given range of IDs. The range is specified by a minimum and maximum ID. All the entries having an ID between the two specified or exactly one of the two IDs specified (closed interval) are returned
        /// <para>Available since: 5.0.0</para>
        /// <para>获得指定范围内的内容条目</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="start">start id. Starting with Redis version 6.2.0: Added exclusive ranges
        /// <para>开始ID. 从Redis6.2.0及以后的版本中支持传入(开头排除开始ID. 如: (1724744328746</para>
        /// </param>
        /// <param name="end">end id. Starting with Redis version 6.2.0: Added exclusive ranges
        /// <para>结束ID. 从Redis6.2.0及以后的版本中支持传入(开头排除开始ID. 如: (1724744328746</para>
        /// </param>
        /// <param name="count">Using the COUNT option it is possible to reduce the number of entries reported. The default value is 0 to obtain all
        /// <para>获取的个数, 默认为0表示获取所有</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public StreamValue<string>[]? XRange(string key, string start, string end, ulong count = 0, CancellationToken cancellationToken = default)
#else
        public StreamValue<string>[] XRange(string key, string start, string end, ulong count = 0, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<StreamValue<string>>(StreamCommands.XRange(key, start, end, count), ResultType.Array | ResultType.Stream | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// The command returns the stream entries matching a given range of IDs. The range is specified by a minimum and maximum ID. All the entries having an ID between the two specified or exactly one of the two IDs specified (closed interval) are returned
        /// <para>Available since: 5.0.0</para>
        /// <para>获得指定范围内的内容条目</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="start">start id. Starting with Redis version 6.2.0: Added exclusive ranges
        /// <para>开始ID. 从Redis6.2.0及以后的版本中支持传入(开头排除开始ID. 如: (1724744328746</para>
        /// </param>
        /// <param name="end">end id. Starting with Redis version 6.2.0: Added exclusive ranges
        /// <para>结束ID. 从Redis6.2.0及以后的版本中支持传入(开头排除开始ID. 如: (1724744328746</para>
        /// </param>
        /// <param name="count">Using the COUNT option it is possible to reduce the number of entries reported. The default value is 0 to obtain all
        /// <para>获取的个数, 默认为0表示获取所有</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public StreamValue<byte[]>[]? XRangeBytes(string key, string start, string end, ulong count = 0, CancellationToken cancellationToken = default)
#else
        public StreamValue<byte[]>[] XRangeBytes(string key, string start, string end, ulong count = 0, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<StreamValue<byte[]>>(StreamCommands.XRange(key, start, end, count), ResultType.Array | ResultType.Stream | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like XRANGE, but with the notable difference of returning the entries in reverse order, and also taking the start-end range in reverse order
        /// <para>Available since: 5.0.0</para>
        /// <para>获得指定范围内的内容条目. 顺序和XRANGE相反, 是从end读到start</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="start">start id. Starting with Redis version 6.2.0: Added exclusive ranges
        /// <para>开始ID. 从Redis6.2.0及以后的版本中支持传入(开头排除开始ID. 如: (1724744328746</para>
        /// </param>
        /// <param name="end">end id. Starting with Redis version 6.2.0: Added exclusive ranges
        /// <para>结束ID. 从Redis6.2.0及以后的版本中支持传入(开头排除开始ID. 如: (1724744328746</para>
        /// </param>
        /// <param name="count">Using the COUNT option it is possible to reduce the number of entries reported. The default value is 0 to obtain all
        /// <para>获取的个数, 默认为0表示获取所有</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public StreamValue<string>[]? XRevRange(string key, string end, string start, ulong count = 0, CancellationToken cancellationToken = default)
#else
        public StreamValue<string>[] XRevRange(string key, string end, string start, ulong count = 0, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<StreamValue<string>>(StreamCommands.XRevRange(key, end, start, count), ResultType.Array | ResultType.Stream | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like XRANGE, but with the notable difference of returning the entries in reverse order, and also taking the start-end range in reverse order
        /// <para>Available since: 5.0.0</para>
        /// <para>获得指定范围内的内容条目. 顺序和XRANGE相反, 是从end读到start</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="start">start id. Starting with Redis version 6.2.0: Added exclusive ranges
        /// <para>开始ID. 从Redis6.2.0及以后的版本中支持传入(开头排除开始ID. 如: (1724744328746</para>
        /// </param>
        /// <param name="end">end id. Starting with Redis version 6.2.0: Added exclusive ranges
        /// <para>结束ID. 从Redis6.2.0及以后的版本中支持传入(开头排除开始ID. 如: (1724744328746</para>
        /// </param>
        /// <param name="count">Using the COUNT option it is possible to reduce the number of entries reported. The default value is 0 to obtain all
        /// <para>获取的个数, 默认为0表示获取所有</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public StreamValue<byte[]>[]? XRevRangeBytes(string key, string end, string start, ulong count = 0, CancellationToken cancellationToken = default)
#else
        public StreamValue<byte[]>[] XRevRangeBytes(string key, string end, string start, ulong count = 0, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<StreamValue<byte[]>>(StreamCommands.XRevRange(key, end, start, count), ResultType.Array | ResultType.Stream | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Read data from one or multiple streams, only returning entries with an ID greater than the last received ID reported by the caller
        /// <para>Available since: 5.0.0</para>
        /// <para>从指定的Stream和ID中返回内容条目. 只会返回比指定ID大的</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="id">ID</param>
        /// <param name="blockMilliseconds">Number of blocking milliseconds. 0 indicates unlimited blocking
        /// <para>Unrestricted blocking is not recommended because it may cause connection loss</para>
        /// <para>阻塞毫秒数, 0表示无限阻塞. 不建议无限制阻塞, 可能会导致连接丢失</para>
        /// </param>
        /// <param name="count">The default value is 0, indicating that all data is read
        /// <para>要读取的数量, 默认0, 表示读取所有满足条件的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Dictionary<string, StreamValue<string>[]>? XRead(string key, string id, ulong? blockMilliseconds = null, ulong count = 0, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, StreamValue<string>[]> XRead(string key, string id, ulong? blockMilliseconds = null, ulong count = 0, CancellationToken cancellationToken = default)
#endif
        {
            var command = StreamCommands.XRead(count, blockMilliseconds, key, id);
            if (blockMilliseconds.HasValue && blockMilliseconds.Value > 0)
            {
                using (var tokenSource = new CancellationTokenSource())
                {
                    tokenSource.CancelAfter(TimeSpan.FromMilliseconds(blockMilliseconds.Value + 2000));
                    return base._call.CallClass<Dictionary<string, StreamValue<string>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.String, tokenSource.Token)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
                }
            }

            return base._call.CallClass<Dictionary<string, StreamValue<string>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Read data from one or multiple streams, only returning entries with an ID greater than the last received ID reported by the caller
        /// <para>Available since: 5.0.0</para>
        /// <para>从指定的Stream和ID中返回内容条目. 只会返回比指定ID大的</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="id">ID</param>
        /// <param name="blockMilliseconds">Number of blocking milliseconds. 0 indicates unlimited blocking
        /// <para>Unrestricted blocking is not recommended because it may cause connection loss</para>
        /// <para>阻塞毫秒数, 0表示无限阻塞. 不建议无限制阻塞, 可能会导致连接丢失</para>
        /// </param>
        /// <param name="count">The default value is 0, indicating that all data is read
        /// <para>要读取的数量, 默认0, 表示读取所有满足条件的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Dictionary<string, StreamValue<byte[]>[]>? XReadBytes(string key, string id, ulong? blockMilliseconds = null, ulong count = 0, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, StreamValue<byte[]>[]> XReadBytes(string key, string id, ulong? blockMilliseconds = null, ulong count = 0, CancellationToken cancellationToken = default)
#endif
        {
            var command = StreamCommands.XRead(count, blockMilliseconds, key, id);
            if (blockMilliseconds.HasValue && blockMilliseconds.Value > 0)
            {
                using (var tokenSource = new CancellationTokenSource())
                {
                    tokenSource.CancelAfter(TimeSpan.FromMilliseconds(blockMilliseconds.Value + 2000));
                    return base._call.CallClass<Dictionary<string, StreamValue<byte[]>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.Bytes, tokenSource.Token)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
                }
            }
            return base._call.CallClass<Dictionary<string, StreamValue<byte[]>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
        }

        /// <summary>
        /// Read data from one or multiple streams, only returning entries with an ID greater than the last received ID reported by the caller
        /// <para>Available since: 5.0.0</para>
        /// <para>从指定的Stream和ID中返回内容条目. 只会返回比指定ID大的</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="keys">Stream key array</param>
        /// <param name="ids">ID array</param>
        /// <param name="blockMilliseconds">Number of blocking milliseconds. 0 indicates unlimited blocking
        /// <para>Unrestricted blocking is not recommended because it may cause connection loss</para>
        /// <para>阻塞毫秒数, 0表示无限阻塞. 不建议无限制阻塞, 可能会导致连接丢失</para>
        /// </param>
        /// <param name="count">The default value is 0, indicating that all data is read
        /// <para>要读取的数量, 默认0, 表示读取所有满足条件的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Dictionary<string, StreamValue<string>[]>? XRead(string[] keys, string[] ids, ulong? blockMilliseconds = null, ulong count = 0, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, StreamValue<string>[]> XRead(string[] keys, string[] ids, ulong? blockMilliseconds = null, ulong count = 0, CancellationToken cancellationToken = default)
#endif
        {
            var command = StreamCommands.XRead(count, blockMilliseconds, keys, ids);
            if (blockMilliseconds.HasValue && blockMilliseconds.Value > 0)
            {
                using (var tokenSource = new CancellationTokenSource())
                {
                    tokenSource.CancelAfter(TimeSpan.FromMilliseconds(blockMilliseconds.Value + 2000));
                    return base._call.CallClass<Dictionary<string, StreamValue<string>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.String, tokenSource.Token)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
                }
            }

            return base._call.CallClass<Dictionary<string, StreamValue<string>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
        }

        /// <summary>
        /// Read data from one or multiple streams, only returning entries with an ID greater than the last received ID reported by the caller
        /// <para>Available since: 5.0.0</para>
        /// <para>从指定的Stream和ID中返回内容条目. 只会返回比指定ID大的</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="keys">Stream key array</param>
        /// <param name="ids">ID array</param>
        /// <param name="blockMilliseconds">Number of blocking milliseconds. 0 indicates unlimited blocking
        /// <para>Unrestricted blocking is not recommended because it may cause connection loss</para>
        /// <para>阻塞毫秒数, 0表示无限阻塞. 不建议无限制阻塞, 可能会导致连接丢失</para>
        /// </param>
        /// <param name="count">The default value is 0, indicating that all data is read
        /// <para>要读取的数量, 默认0, 表示读取所有满足条件的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Dictionary<string, StreamValue<byte[]>[]>? XReadBytes(string[] keys, string[] ids, ulong? blockMilliseconds = null, ulong count = 0, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, StreamValue<byte[]>[]> XReadBytes(string[] keys, string[] ids, ulong? blockMilliseconds = null, ulong count = 0, CancellationToken cancellationToken = default)
#endif
        {
            var command = StreamCommands.XRead(count, blockMilliseconds, keys, ids);
            if (blockMilliseconds.HasValue && blockMilliseconds.Value > 0)
            {
                using (var tokenSource = new CancellationTokenSource())
                {
                    tokenSource.CancelAfter(TimeSpan.FromMilliseconds(blockMilliseconds.Value + 2000));
                    return base._call.CallClass<Dictionary<string, StreamValue<byte[]>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.Bytes, tokenSource.Token)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
                }
            }

            return base._call.CallClass<Dictionary<string, StreamValue<byte[]>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
        }

        /// <summary>
        /// XTRIM trims the stream by evicting older entries (entries with lower IDs) if needed
        /// <para>Available since: 5.0.0</para>
        /// <para>修剪Stream, 只会删除比传入的threshold小的内容条目</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="trimMode">trim mode<para>修剪模式</para></param>
        /// <param name="threshold">threshold<para>条件</para></param>
        /// <param name="trimStrategy">trim strategy</param>
        /// <param name="count">The amount of pruning, which defaults to 0, means pruning all elements that meet the condition
        /// <para>修剪个数, 默认为0, 表示修剪所有满足条件的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of entries deleted from the stream
        /// <para>成功修剪的个数</para>
        /// </returns>
        public long XTrim(string key, StreamTrimMode trimMode, string threshold, StreamTrimStrategy trimStrategy = StreamTrimStrategy.None, ulong count = 0, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(StreamCommands.XTrim(key, trimMode, trimStrategy, threshold, count), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Create a new consumer group uniquely identified by groupname for the stream stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>给指定的Stream创建一个消费组</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">group name<para>消费组名称</para></param>
        /// <param name="id">The start ID or $of the consumer group (spend from the latest entry in the stream)
        /// <para> 消费者组的起始 ID 或 $（从流的最新条目开始消费）</para>
        /// </param>
        /// <param name="mkStream">If the specified stream does not exist, an empty stream is automatically created. Default not created
        /// <para>如果指定的Stream不存在, 是否自动创建一个空的Stream. 默认不创建</para>
        /// </param>
        /// <param name="entriesRead">Available since: 7.0.0. Used to specify how many entries in the stream have been read by this consumer group
        /// <para>指定该消费者组已经读取了多少条流中的条目. Redis 7.0.0+才支持此参数设置</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool XGroupCreate(string key, string group, string id, bool mkStream = false, ulong? entriesRead = null, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StreamCommands.XGroupCreate(key, group, id, mkStream, entriesRead), "OK", cancellationToken);
        }

        /// <summary>
        /// The XREADGROUP command is a special version of the XREAD command with support for consumer groups
        /// <para>Available since: 5.0.0</para>
        /// <para>消费组消费Stream</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="id">id</param>
        /// <param name="group">group name
        /// <para>消费组名称</para>
        /// </param>
        /// <param name="consumer">consumer
        /// <para>消费者</para>
        /// </param>
        /// <param name="count">Specifies the maximum number of entries to be read from the stream. If not specified, Redis returns all available entries
        /// <para>指定从流中读取的最大条目数。如果没有指定，Redis 将返回所有可用的条目. 默认不指定</para>
        /// </param>
        /// <param name="blockMilliseconds">Specifies the maximum amount of time (in milliseconds) that a consumer should block to wait when no entry is available
        /// <para>指定在没有可用条目时，消费者应该阻塞等待的最大时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="noAck">If NOACK is specified, the message is immediately removed from the Pending Entries List (PEL) without explicit acknowledgment using the XACK command
        /// <para>如果指定了 NOACK，消息将立即从待处理条目队列（PEL，Pending Entries List）中移除，而不需要使用 XACK 命令显式确认</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Dictionary<string, StreamValue<string>[]>? XReadGroup(string key, string id, string group, string consumer, ulong count = 0, ulong? blockMilliseconds = null, bool noAck = false, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, StreamValue<string>[]> XReadGroup(string key, string id, string group, string consumer, ulong count = 0, ulong? blockMilliseconds = null, bool noAck = false, CancellationToken cancellationToken = default)
#endif
        {
            var command = StreamCommands.XReadGroup(group, consumer, count, blockMilliseconds, noAck, key, id);
            if (blockMilliseconds.HasValue && blockMilliseconds.Value > 0)
            {
                using (var tokenSource = new CancellationTokenSource())
                {
                    tokenSource.CancelAfter(TimeSpan.FromMilliseconds(blockMilliseconds.Value + 2000));
                    return base._call.CallClass<Dictionary<string, StreamValue<string>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.String, tokenSource.Token)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
                }
            }
            return base._call.CallClass<Dictionary<string, StreamValue<string>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
        }

        /// <summary>
        /// The XREADGROUP command is a special version of the XREAD command with support for consumer groups
        /// <para>Available since: 5.0.0</para>
        /// <para>消费组消费Stream</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="keys">Stream keys array</param>
        /// <param name="ids">ids</param>
        /// <param name="group">group name
        /// <para>消费组名称</para>
        /// </param>
        /// <param name="consumer">consumer
        /// <para>消费者</para>
        /// </param>
        /// <param name="count">Specifies the maximum number of entries to be read from the stream. If not specified, Redis returns all available entries
        /// <para>指定从流中读取的最大条目数。如果没有指定，Redis 将返回所有可用的条目. 默认不指定</para>
        /// </param>
        /// <param name="blockMilliseconds">Specifies the maximum amount of time (in milliseconds) that a consumer should block to wait when no entry is available
        /// <para>指定在没有可用条目时，消费者应该阻塞等待的最大时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="noAck">If NOACK is specified, the message is immediately removed from the Pending Entries List (PEL) without explicit acknowledgment using the XACK command
        /// <para>如果指定了 NOACK，消息将立即从待处理条目队列（PEL，Pending Entries List）中移除，而不需要使用 XACK 命令显式确认</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Dictionary<string, StreamValue<string>[]>? XReadGroup(string[] keys, string[] ids, string group, string consumer, ulong count = 0, ulong? blockMilliseconds = null, bool noAck = false, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, StreamValue<string>[]> XReadGroup(string[] keys, string[] ids, string group, string consumer, ulong count = 0, ulong? blockMilliseconds = null, bool noAck = false, CancellationToken cancellationToken = default)
#endif
        {
            var command = StreamCommands.XReadGroup(group, consumer, count, blockMilliseconds, noAck, keys, ids);
            if (blockMilliseconds.HasValue && blockMilliseconds.Value > 0)
            {
                using (var tokenSource = new CancellationTokenSource())
                {
                    tokenSource.CancelAfter(TimeSpan.FromMilliseconds(blockMilliseconds.Value + 2000));
                    return base._call.CallClass<Dictionary<string, StreamValue<string>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.String, tokenSource.Token)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
                }
            }
            return base._call.CallClass<Dictionary<string, StreamValue<string>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
        }

        /// <summary>
        /// The XREADGROUP command is a special version of the XREAD command with support for consumer groups
        /// <para>Available since: 5.0.0</para>
        /// <para>消费组消费Stream</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="id">id</param>
        /// <param name="group">group name
        /// <para>消费组名称</para>
        /// </param>
        /// <param name="consumer">consumer
        /// <para>消费者</para>
        /// </param>
        /// <param name="count">Specifies the maximum number of entries to be read from the stream. If not specified, Redis returns all available entries
        /// <para>指定从流中读取的最大条目数。如果没有指定，Redis 将返回所有可用的条目. 默认不指定</para>
        /// </param>
        /// <param name="blockMilliseconds">Specifies the maximum amount of time (in milliseconds) that a consumer should block to wait when no entry is available
        /// <para>指定在没有可用条目时，消费者应该阻塞等待的最大时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="noAck">If NOACK is specified, the message is immediately removed from the Pending Entries List (PEL) without explicit acknowledgment using the XACK command
        /// <para>如果指定了 NOACK，消息将立即从待处理条目队列（PEL，Pending Entries List）中移除，而不需要使用 XACK 命令显式确认</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Dictionary<string, StreamValue<byte[]>[]>? XReadGroupBytes(string key, string id, string group, string consumer, ulong count = 0, ulong? blockMilliseconds = null, bool noAck = false, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, StreamValue<byte[]>[]> XReadGroupBytes(string key, string id, string group, string consumer, ulong count = 0, ulong? blockMilliseconds = null, bool noAck = false, CancellationToken cancellationToken = default)
#endif
        {
            var command = StreamCommands.XReadGroup(group, consumer, count, blockMilliseconds, noAck, key, id);
            if (blockMilliseconds.HasValue && blockMilliseconds.Value > 0)
            {
                using (var tokenSource = new CancellationTokenSource())
                {
                    tokenSource.CancelAfter(TimeSpan.FromMilliseconds(blockMilliseconds.Value + 2000));
                    return base._call.CallClass<Dictionary<string, StreamValue<byte[]>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.Bytes, tokenSource.Token)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
                }
            }
            return base._call.CallClass<Dictionary<string, StreamValue<byte[]>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
        }

        /// <summary>
        /// The XREADGROUP command is a special version of the XREAD command with support for consumer groups
        /// <para>Available since: 5.0.0</para>
        /// <para>消费组消费Stream</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="keys">Stream keys array</param>
        /// <param name="ids">ids</param>
        /// <param name="group">group name
        /// <para>消费组名称</para>
        /// </param>
        /// <param name="consumer">consumer
        /// <para>消费者</para>
        /// </param>
        /// <param name="count">Specifies the maximum number of entries to be read from the stream. If not specified, Redis returns all available entries
        /// <para>指定从流中读取的最大条目数。如果没有指定，Redis 将返回所有可用的条目. 默认不指定</para>
        /// </param>
        /// <param name="blockMilliseconds">Specifies the maximum amount of time (in milliseconds) that a consumer should block to wait when no entry is available
        /// <para>指定在没有可用条目时，消费者应该阻塞等待的最大时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="noAck">If NOACK is specified, the message is immediately removed from the Pending Entries List (PEL) without explicit acknowledgment using the XACK command
        /// <para>如果指定了 NOACK，消息将立即从待处理条目队列（PEL，Pending Entries List）中移除，而不需要使用 XACK 命令显式确认</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Dictionary<string, StreamValue<byte[]>[]>? XReadGroupBytes(string[] keys, string[] ids, string group, string consumer, ulong count = 0, ulong? blockMilliseconds = null, bool noAck = false, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, StreamValue<byte[]>[]> XReadGroupBytes(string[] keys, string[] ids, string group, string consumer, ulong count = 0, ulong? blockMilliseconds = null, bool noAck = false, CancellationToken cancellationToken = default)
#endif
        {
            var command = StreamCommands.XReadGroup(group, consumer, count, blockMilliseconds, noAck, keys, ids);
            if (blockMilliseconds.HasValue && blockMilliseconds.Value > 0)
            {
                using (var tokenSource = new CancellationTokenSource())
                {
                    tokenSource.CancelAfter(TimeSpan.FromMilliseconds(blockMilliseconds.Value + 2000));
                    return base._call.CallClass<Dictionary<string, StreamValue<byte[]>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.Bytes, tokenSource.Token)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
                }
            }
            return base._call.CallClass<Dictionary<string, StreamValue<byte[]>[]>>(command, ResultType.Dictionary | ResultType.Array | ResultType.Stream | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    !
#endif
                    ;
        }

        /// <summary>
        /// The XACK command removes one messages from the Pending Entries List (PEL) of a stream consumer group
        /// <para>Available since: 5.0.0</para>
        /// <para>XACK命令从流消费者组的Pending Entries List (PEL)中删除一条消息</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">group name
        /// <para>消费组名称</para>
        /// </param>
        /// <param name="id">id</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The command returns the number of messages successfully acknowledged
        /// <para>成功确认的消息条目数量</para>
        /// </returns>
        public long XAck(string key, string group, string id, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(StreamCommands.XAck(key, group, id), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// The XACK command removes one or multiple messages from the Pending Entries List (PEL) of a stream consumer group
        /// <para>Available since: 5.0.0</para>
        /// <para>XACK命令从流消费者组的Pending Entries List (PEL)中删除一条或多条消息</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">group name
        /// <para>消费组名称</para>
        /// </param>
        /// <param name="ids">id array</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The command returns the number of messages successfully acknowledged
        /// <para>成功确认的消息条目数量</para>
        /// </returns>
        public long XAck(string key, string group, string[] ids, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(StreamCommands.XAck(key, group, ids), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command transfers ownership of pending stream entries that match the specified criteria
        /// <para>Available since: 6.2.0</para>
        /// <para>在消费者组（consumer group）中重新分配（claim）空闲时间超过一定阈值（min-idle-time）的消息</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">group name<para>消费组名称</para></param>
        /// <param name="consumer">Specify which consumer will get the eligible message back. This consumer is part of a consumer group that is used to redistribute messages
        /// <para>指定哪个消费者将重新获得符合条件的消息。此消费者是消费者组的一部分，用于重新分配消息</para>
        /// </param>
        /// <param name="minIdleTime">Specifies a time threshold (in milliseconds) for determining which messages should be redistributed. Only messages that have been idle for at least min-idle-time milliseconds since they were last read will be reassigned to the specified consumer
        /// <para>指定一个时间阈值（以毫秒为单位），用于确定哪些消息应该被重新分配。只有那些自上次被读取后已经空闲了至少 min-idle-time 毫秒的消息，才会被重新分配给指定的消费者</para>
        /// </param>
        /// <param name="start">Specify the start ID of the reassignment operation (that is, the ID from which to start looking for messages that match the min-idle-time condition)
        /// <para>指定重新分配操作的起始 ID（即，开始查找符合 min-idle-time 条件的消息的 ID）</para>
        /// </param>
        /// <param name="count">Specifies the maximum number of messages that can be redistributed in a single XAUTOCLAIM operation. If not specified, Redis will attempt to redistribute all messages that match min-idle-time
        /// <para>指定在一次 XAUTOCLAIM 操作中，最多可以重新分配的消息数量。如果没有指定，Redis 将尝试重新分配所有符合 min-idle-time 的消息</para>
        /// </param>
        /// <param name="justID">An optional flag indicating that the returned result contains only the ID of the reassigned message and not its content
        /// <para>可选标志，指示返回的结果仅包含被重新分配的消息的 ID，而不包括消息的内容</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public XAutoClaimValue<string>? XAutoClaim(string key, string group, string consumer, ulong minIdleTime, string start, ulong count = 0, bool justID = false, CancellationToken cancellationToken = default)
#else
        public XAutoClaimValue<string> XAutoClaim(string key, string group, string consumer, ulong minIdleTime, string start, ulong count = 0, bool justID = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClass<XAutoClaimValue<string>>(StreamCommands.XAutoClaim(key, group, consumer, minIdleTime, start, count, justID), ResultType.XAutoClaimValue | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// This command transfers ownership of pending stream entries that match the specified criteria
        /// <para>Available since: 6.2.0</para>
        /// <para>在消费者组（consumer group）中重新分配（claim）空闲时间超过一定阈值（min-idle-time）的消息</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">group name<para>消费组名称</para></param>
        /// <param name="consumer">Specify which consumer will get the eligible message back. This consumer is part of a consumer group that is used to redistribute messages
        /// <para>指定哪个消费者将重新获得符合条件的消息。此消费者是消费者组的一部分，用于重新分配消息</para>
        /// </param>
        /// <param name="minIdleTime">Specifies a time threshold (in milliseconds) for determining which messages should be redistributed. Only messages that have been idle for at least min-idle-time milliseconds since they were last read will be reassigned to the specified consumer
        /// <para>指定一个时间阈值（以毫秒为单位），用于确定哪些消息应该被重新分配。只有那些自上次被读取后已经空闲了至少 min-idle-time 毫秒的消息，才会被重新分配给指定的消费者</para>
        /// </param>
        /// <param name="start">Specify the start ID of the reassignment operation (that is, the ID from which to start looking for messages that match the min-idle-time condition)
        /// <para>指定重新分配操作的起始 ID（即，开始查找符合 min-idle-time 条件的消息的 ID）</para>
        /// </param>
        /// <param name="count">Specifies the maximum number of messages that can be redistributed in a single XAUTOCLAIM operation. If not specified, Redis will attempt to redistribute all messages that match min-idle-time
        /// <para>指定在一次 XAUTOCLAIM 操作中，最多可以重新分配的消息数量。如果没有指定，Redis 将尝试重新分配所有符合 min-idle-time 的消息</para>
        /// </param>
        /// <param name="justID">An optional flag indicating that the returned result contains only the ID of the reassigned message and not its content
        /// <para>可选标志，指示返回的结果仅包含被重新分配的消息的 ID，而不包括消息的内容</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public XAutoClaimValue<byte[]>? XAutoClaimBytes(string key, string group, string consumer, ulong minIdleTime, string start, ulong count = 0, bool justID = false, CancellationToken cancellationToken = default)
#else
        public XAutoClaimValue<byte[]> XAutoClaimBytes(string key, string group, string consumer, ulong minIdleTime, string start, ulong count = 0, bool justID = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClass<XAutoClaimValue<byte[]>>(StreamCommands.XAutoClaim(key, group, consumer, minIdleTime, start, count, justID), ResultType.XAutoClaimValue | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// In the context of a stream consumer group, this command changes the ownership of a pending message, so that the new owner is the consumer specified as the command argument
        /// <para>Available since: 5.0.0</para>
        /// <para>将流中的待处理条目（Pending Entries List，PEL）重新分配给另一个消费者</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">Specify the consumer group name. Messages are reassigned from the Pending Items List (PEL) for that consumer group
        /// <para>指定消费者组名称。消息将从该消费者组的待处理条目列表（PEL）中重新分配</para>
        /// </param>
        /// <param name="consumer">Specify the consumer name to which the message is to be reassigned. Consumers are part of a consumer group that is used to redistribute messages
        /// <para>指定要将消息重新分配给的消费者名称。消费者是消费者组的一部分，用于重新分配消息</para>
        /// </param>
        /// <param name="minIdleTime">Minimum idle time (in milliseconds)
        /// <para>最小空闲时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="id">id</param>
        /// <param name="idle">Reset the idle time of the message(in milliseconds)
        /// <para>重置消息的空闲时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="time">Sets the last processing time of the reassigned message to the specified UNIX timestamp in milliseconds
        /// <para>将重新分配的消息的最后处理时间设置为指定的 UNIX 时间戳（以毫秒为单位）</para>
        /// </param>
        /// <param name="retryCount">Sets the retry count for messages
        /// <para>设置消息的重试计数</para>
        /// </param>
        /// <param name="force">Force reassignment of messages
        /// <para>强制重新分配消息</para>
        /// </param>
        /// <param name="justid">An optional flag indicating that the returned result contains only the ID of the reassigned message and not its content
        /// <para>可选标志，指示返回的结果仅包含被重新分配的消息的 ID，而不包括消息的内容</para>
        /// </param>
        /// <param name="lastid">This parameter is optional and is used to update the final processing ID of the consumer group
        /// <para>可选参数，用于更新消费者组的最后处理 ID</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public StreamValue<string>[]? XClaim(string key, string group, string consumer, ulong minIdleTime, string id, ulong? idle = null, ulong? time = null, ulong? retryCount = null, bool force = false, bool justid = false, string? lastid = null, CancellationToken cancellationToken = default)
#else
        public StreamValue<string>[] XClaim(string key, string group, string consumer, ulong minIdleTime, string id, ulong? idle = null, ulong? time = null, ulong? retryCount = null, bool force = false, bool justid = false, string lastid = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<StreamValue<string>>(StreamCommands.XClaim(
                key,
                group,
                consumer,
                minIdleTime,
                id,
                idle,
                time,
                retryCount,
                force,
                justid,
                lastid), ResultType.Array | ResultType.Stream | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// In the context of a stream consumer group, this command changes the ownership of a pending message, so that the new owner is the consumer specified as the command argument
        /// <para>Available since: 5.0.0</para>
        /// <para>将流中的待处理条目（Pending Entries List，PEL）重新分配给另一个消费者</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">Specify the consumer group name. Messages are reassigned from the Pending Items List (PEL) for that consumer group
        /// <para>指定消费者组名称。消息将从该消费者组的待处理条目列表（PEL）中重新分配</para>
        /// </param>
        /// <param name="consumer">Specify the consumer name to which the message is to be reassigned. Consumers are part of a consumer group that is used to redistribute messages
        /// <para>指定要将消息重新分配给的消费者名称。消费者是消费者组的一部分，用于重新分配消息</para>
        /// </param>
        /// <param name="minIdleTime">Minimum idle time (in milliseconds)
        /// <para>最小空闲时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="id">id</param>
        /// <param name="idle">Reset the idle time of the message(in milliseconds)
        /// <para>重置消息的空闲时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="time">Sets the last processing time of the reassigned message to the specified UNIX timestamp in milliseconds
        /// <para>将重新分配的消息的最后处理时间设置为指定的 UNIX 时间戳（以毫秒为单位）</para>
        /// </param>
        /// <param name="retryCount">Sets the retry count for messages
        /// <para>设置消息的重试计数</para>
        /// </param>
        /// <param name="force">Force reassignment of messages
        /// <para>强制重新分配消息</para>
        /// </param>
        /// <param name="justid">An optional flag indicating that the returned result contains only the ID of the reassigned message and not its content
        /// <para>可选标志，指示返回的结果仅包含被重新分配的消息的 ID，而不包括消息的内容</para>
        /// </param>
        /// <param name="lastid">This parameter is optional and is used to update the final processing ID of the consumer group
        /// <para>可选参数，用于更新消费者组的最后处理 ID</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public StreamValue<byte[]>[]? XClaimBytes(string key, string group, string consumer, ulong minIdleTime, string id, ulong? idle = null, ulong? time = null, ulong? retryCount = null, bool force = false, bool justid = false, string? lastid = null, CancellationToken cancellationToken = default)
#else
        public StreamValue<byte[]>[] XClaimBytes(string key, string group, string consumer, ulong minIdleTime, string id, ulong? idle = null, ulong? time = null, ulong? retryCount = null, bool force = false, bool justid = false, string lastid = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<StreamValue<byte[]>>(StreamCommands.XClaim(
                key,
                group,
                consumer,
                minIdleTime,
                id,
                idle,
                time,
                retryCount,
                force,
                justid,
                lastid), ResultType.Array | ResultType.Stream | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// In the context of a stream consumer group, this command changes the ownership of a pending message, so that the new owner is the consumer specified as the command argument
        /// <para>Available since: 5.0.0</para>
        /// <para>将流中的待处理条目（Pending Entries List，PEL）重新分配给另一个消费者</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">Specify the consumer group name. Messages are reassigned from the Pending Items List (PEL) for that consumer group
        /// <para>指定消费者组名称。消息将从该消费者组的待处理条目列表（PEL）中重新分配</para>
        /// </param>
        /// <param name="consumer">Specify the consumer name to which the message is to be reassigned. Consumers are part of a consumer group that is used to redistribute messages
        /// <para>指定要将消息重新分配给的消费者名称。消费者是消费者组的一部分，用于重新分配消息</para>
        /// </param>
        /// <param name="minIdleTime">Minimum idle time (in milliseconds)
        /// <para>最小空闲时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="ids">id array</param>
        /// <param name="idle">Reset the idle time of the message(in milliseconds)
        /// <para>重置消息的空闲时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="time">Sets the last processing time of the reassigned message to the specified UNIX timestamp in milliseconds
        /// <para>将重新分配的消息的最后处理时间设置为指定的 UNIX 时间戳（以毫秒为单位）</para>
        /// </param>
        /// <param name="retryCount">Sets the retry count for messages
        /// <para>设置消息的重试计数</para>
        /// </param>
        /// <param name="force">Force reassignment of messages
        /// <para>强制重新分配消息</para>
        /// </param>
        /// <param name="justid">An optional flag indicating that the returned result contains only the ID of the reassigned message and not its content
        /// <para>可选标志，指示返回的结果仅包含被重新分配的消息的 ID，而不包括消息的内容</para>
        /// </param>
        /// <param name="lastid">This parameter is optional and is used to update the final processing ID of the consumer group
        /// <para>可选参数，用于更新消费者组的最后处理 ID</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public StreamValue<string>[]? XClaim(string key, string group, string consumer, ulong minIdleTime, string[] ids, ulong? idle = null, ulong? time = null, ulong? retryCount = null, bool force = false, bool justid = false, string? lastid = null, CancellationToken cancellationToken = default)
#else
        public StreamValue<string>[] XClaim(string key, string group, string consumer, ulong minIdleTime, string[] ids, ulong? idle = null, ulong? time = null, ulong? retryCount = null, bool force = false, bool justid = false, string lastid = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<StreamValue<string>>(StreamCommands.XClaim(
                key,
                group,
                consumer,
                minIdleTime,
                ids,
                idle,
                time,
                retryCount,
                force,
                justid,
                lastid), ResultType.Array | ResultType.Stream | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// In the context of a stream consumer group, this command changes the ownership of a pending message, so that the new owner is the consumer specified as the command argument
        /// <para>Available since: 5.0.0</para>
        /// <para>将流中的待处理条目（Pending Entries List，PEL）重新分配给另一个消费者</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">Specify the consumer group name. Messages are reassigned from the Pending Items List (PEL) for that consumer group
        /// <para>指定消费者组名称。消息将从该消费者组的待处理条目列表（PEL）中重新分配</para>
        /// </param>
        /// <param name="consumer">Specify the consumer name to which the message is to be reassigned. Consumers are part of a consumer group that is used to redistribute messages
        /// <para>指定要将消息重新分配给的消费者名称。消费者是消费者组的一部分，用于重新分配消息</para>
        /// </param>
        /// <param name="minIdleTime">Minimum idle time (in milliseconds)
        /// <para>最小空闲时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="ids">id array</param>
        /// <param name="idle">Reset the idle time of the message(in milliseconds)
        /// <para>重置消息的空闲时间（以毫秒为单位）</para>
        /// </param>
        /// <param name="time">Sets the last processing time of the reassigned message to the specified UNIX timestamp in milliseconds
        /// <para>将重新分配的消息的最后处理时间设置为指定的 UNIX 时间戳（以毫秒为单位）</para>
        /// </param>
        /// <param name="retryCount">Sets the retry count for messages
        /// <para>设置消息的重试计数</para>
        /// </param>
        /// <param name="force">Force reassignment of messages
        /// <para>强制重新分配消息</para>
        /// </param>
        /// <param name="justid">An optional flag indicating that the returned result contains only the ID of the reassigned message and not its content
        /// <para>可选标志，指示返回的结果仅包含被重新分配的消息的 ID，而不包括消息的内容</para>
        /// </param>
        /// <param name="lastid">This parameter is optional and is used to update the final processing ID of the consumer group
        /// <para>可选参数，用于更新消费者组的最后处理 ID</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public StreamValue<byte[]>[]? XClaimBytes(string key, string group, string consumer, ulong minIdleTime, string[] ids, ulong? idle = null, ulong? time = null, ulong? retryCount = null, bool force = false, bool justid = false, string? lastid = null, CancellationToken cancellationToken = default)
#else
        public StreamValue<byte[]>[] XClaimBytes(string key, string group, string consumer, ulong minIdleTime, string[] ids, ulong? idle = null, ulong? time = null, ulong? retryCount = null, bool force = false, bool justid = false, string lastid = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<StreamValue<byte[]>>(StreamCommands.XClaim(
                key,
                group,
                consumer,
                minIdleTime,
                ids,
                idle,
                time,
                retryCount,
                force,
                justid,
                lastid), ResultType.Array | ResultType.Stream | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Create a consumer named consumername in the consumer group groupname of the stream that's stored at key
        /// <para>Available since: 6.2.0</para>
        /// <para>在指定的Stream和消费者组中创建一个新的消费者</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">group name
        /// <para>消费组名称</para>
        /// </param>
        /// <param name="consumer">consumer name
        /// <para>消费者名称</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool XGroupCreateConsumer(string key, string group, string consumer, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StreamCommands.XGroupCreateConsumer(key, group, consumer), "1", cancellationToken);
        }

        /// <summary>
        /// The XGROUP DELCONSUMER command deletes a consumer from the consumer group
        /// <para>Available since: 5.0.0</para>
        /// <para>在指定的Stream和消费者组中删除一个消费者</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">group name
        /// <para>消费组名称</para>
        /// </param>
        /// <param name="consumer">Specify the consumers to delete
        /// <para>指定要删除的消费者</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of pending messages the consumer had before it was deleted
        /// <para>消费者删除之前待处理的消息数量</para>
        /// </returns>
        public long XGroupDelConsumer(string key, string group, string consumer, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(StreamCommands.XGroupDelConsumer(key, group, consumer), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// The XGROUP DESTROY command completely destroys a consumer group
        /// <para>Available since: 5.0.0</para>
        /// <para>在指定的Stream中销毁(删除)一个消费者组</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">Specify the consumer group name to delete
        /// <para>指定要删除的消费组名称</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool XGroupDestroy(string key, string group, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StreamCommands.XGroupDestroy(key, group), "1", cancellationToken);
        }

        /// <summary>
        /// Set the last delivered ID for a consumer group
        /// <para>Available since: 5.0.0</para>
        /// <para>设置消费组最后消费的ID</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">Consumer group name
        /// <para>消费组名称</para>
        /// </param>
        /// <param name="id">id</param>
        /// <param name="entriesRead">Available since: 7.0.0. Used to specify how many entries in the stream have been read by this consumer group
        /// <para>指定该消费者组已经读取了多少条流中的条目. Redis 7.0.0+才支持此参数设置</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool XGroupSetID(string key, string group, string id, ulong? entriesRead = null, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StreamCommands.XGroupSetID(key, group, id, entriesRead), "OK", cancellationToken);
        }

        /// <summary>
        /// The XSETID command is an internal command. It is used by a Redis master to replicate the last delivered ID of streams
        /// <para>Available since: 5.0.0</para>
        /// <para>内部命令。 Redis master 使用它来复制最后传递的Stream ID</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="lastid">lastid</param>
        /// <param name="entriesAdded">entriesAdded</param>
        /// <param name="maxDeletedId">maxDeletedId</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public bool XSetID(string key, string lastid, ulong? entriesAdded = null, string? maxDeletedId = null, CancellationToken cancellationToken = default)
#else
        public bool XSetID(string key, string lastid, ulong? entriesAdded = null, string maxDeletedId = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallCondition(StreamCommands.XSetID(key, lastid, entriesAdded, maxDeletedId), "OK", cancellationToken);
        }

        /// <summary>
        /// This command returns the list of consumers that belong to the groupname consumer group of the stream stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>该命令返回指定Stream和指定消费组中的消费者列表</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="group">group
        /// <para>消费组名称</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public XInfoConsumersValue[]? XInfoConsumers(string key, string group, CancellationToken cancellationToken = default)
#else
        public XInfoConsumersValue[] XInfoConsumers(string key, string group, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<XInfoConsumersValue>(StreamCommands.XInfoConsumers(key, group), ResultType.Array | ResultType.XInfoConsumersValue, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command returns the list of all consumer groups of the stream stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>该命令返回指定Stream中的消费组列表</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public XInfoGroupsValue[]? XInfoGroups(string key, CancellationToken cancellationToken = default)
#else
        public XInfoGroupsValue[] XInfoGroups(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<XInfoGroupsValue>(StreamCommands.XInfoGroups(key), ResultType.Array | ResultType.XInfoGroupsValue, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command returns information about the stream stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>获得一个Stream的信息</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public XInfoStreamValue<string> XInfoStream(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallClass<XInfoStreamValue<string>>(StreamCommands.XInfoStream(key, false, 0), ResultType.XInfoStreamValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command returns information about the stream stored at key
        /// <para>Available since: 5.0.0</para>
        /// <para>获得一个Stream的信息</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public XInfoStreamValue<byte[]> XInfoStreamBytes(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallClass<XInfoStreamValue<byte[]>>(StreamCommands.XInfoStream(key, false, 0), ResultType.XInfoStreamValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command returns information about the stream stored at key
        /// <para>Available since: 6.0.0</para>
        /// <para>获得一个Stream的信息</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="count">Returns the amount of content in the Stream, which defaults to 0, indicating that all content entries are returned
        /// <para>返回Stream中内容条目数量, 默认0, 表示返回所有内容条目</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public XInfoStreamFullValue<string> XInfoStreamFull(string key, ulong count = 0, CancellationToken cancellationToken = default)
        {
            return base._call.CallClass<XInfoStreamFullValue<string>>(StreamCommands.XInfoStream(key, true, count).SetResultDataType(ResultDataType.Default), ResultType.XInfoStreamFullValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command returns information about the stream stored at key
        /// <para>Available since: 6.0.0</para>
        /// <para>获得一个Stream的信息</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">Stream key</param>
        /// <param name="count">Returns the amount of content in the Stream, which defaults to 0, indicating that all content entries are returned
        /// <para>返回Stream中内容条目数量, 默认0, 表示返回所有内容条目</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public XInfoStreamFullValue<byte[]> XInfoStreamFullBytes(string key, ulong count = 0, CancellationToken cancellationToken = default)
        {
            return base._call.CallClass<XInfoStreamFullValue<byte[]>>(StreamCommands.XInfoStream(key, true, count), ResultType.XInfoStreamFullValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }
    }
}
