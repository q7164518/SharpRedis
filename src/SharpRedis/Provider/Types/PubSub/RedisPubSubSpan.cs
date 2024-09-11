#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using SharpRedis.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis
{
    public sealed partial class RedisPubSub
    {
        /// <summary>
        /// Posts a message to the given channel.
        /// <para>Available since:2.0.0</para>
        /// <para>向指定的通道发送消息</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="channel">Channel<para>通道</para></param>
        /// <param name="message">Message</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of clients that received the message. Note that in a Redis Cluster, only clients that are connected to the same node as the publishing client are included in the count.
        /// <para>接受消息的客户端数量. 在集群模式中, 只有与发布客户端连接到同一个节点的客户端才包括在计数中。</para>
        /// </returns>
        public long Publish(string channel, ReadOnlySpan<char> message, CancellationToken cancellationToken = default)
        {
            return this.Publish(channel, message.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Posts a message to the given channel.
        /// <para>Available since:2.0.0</para>
        /// <para>向指定的通道发送消息</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="channel">Channel<para>通道</para></param>
        /// <param name="message">Message</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>the number of clients that received the message. Note that in a Redis Cluster, only clients that are connected to the same node as the publishing client are included in the count.
        /// <para>接受消息的客户端数量. 在集群模式中, 只有与发布客户端连接到同一个节点的客户端才包括在计数中。</para>
        /// </returns>
        public Task<long> PublishAsync(string channel, ReadOnlySpan<char> message, CancellationToken cancellationToken = default)
        {
            return this.PublishAsync(channel, message.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Posts a message to the given shard channel.
        /// <para>Available since:7.0.0</para>
        /// <para>向给定的分片通道发布消息</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="shardChannel">Shard channel
        /// <para>分片通道名称</para>
        /// </param>
        /// <param name="message">Message</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of clients that received the message. Note that in a Redis Cluster, only clients that are connected to the same node as the publishing client are included in the count
        /// <para>接收到消息的客户端数量。在集群模式中，只有与发布客户端连接到同一节点的客户端才会被计算在内</para>
        /// </returns>
        public long SPublish(string shardChannel, ReadOnlySpan<char> message, CancellationToken cancellationToken = default)
        {
            return this.SPublish(shardChannel, message.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Posts a message to the given shard channel.
        /// <para>Available since:7.0.0</para>
        /// <para>向给定的分片通道发布消息</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="shardChannel">Shard channel
        /// <para>分片通道名称</para>
        /// </param>
        /// <param name="message">Message</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of clients that received the message. Note that in a Redis Cluster, only clients that are connected to the same node as the publishing client are included in the count
        /// <para>接收到消息的客户端数量。在集群模式中，只有与发布客户端连接到同一节点的客户端才会被计算在内</para>
        /// </returns>
        public Task<long> SPublishAsync(string shardChannel, ReadOnlySpan<char> message, CancellationToken cancellationToken = default)
        {
            return this.SPublishAsync(shardChannel, message.SpanToBytes(base._call.Encoding), cancellationToken);
        }
    }
}
#endif
