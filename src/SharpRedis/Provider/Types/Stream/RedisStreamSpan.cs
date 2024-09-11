#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using SharpRedis.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis
{
    public sealed partial class RedisStream
    {
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
        public string? XAdd(string key, string field, ReadOnlySpan<char> value, ulong? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions? trimOptions = null, CancellationToken cancellationToken = default)
        {
            return this.XAdd(key, field, value.SpanToBytes(base._call.Encoding), milliseconds, sequenceNumber, nomkStream, trimOptions, cancellationToken);
        }

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
        public Task<string?> XAddAsync(string key, string field, ReadOnlySpan<char> value, ulong? milliseconds = null, ulong? sequenceNumber = null, bool nomkStream = false, StreamTrimOptions? trimOptions = null, CancellationToken cancellationToken = default)
        {
            return this.XAddAsync(key, field, value.SpanToBytes(base._call.Encoding), milliseconds, sequenceNumber, nomkStream, trimOptions, cancellationToken);
        }
    }
}
#endif
