#if !LOW_NET
using SharpRedis.Commands;
using SharpRedis.Models;
using SharpRedis.Network;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Task<long> PublishAsync(string channel, string message, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(PubSubCommands.Publish(channel, message), ResultType.Int64, cancellationToken);
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
        public Task<long> PublishAsync(string channel, byte[] message, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(PubSubCommands.Publish(channel, message), ResultType.Int64, cancellationToken);
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
        public Task<long> SPublishAsync(string shardChannel, string message, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(PubSubCommands.SPublish(shardChannel, message), ResultType.Int64, cancellationToken);
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
        public Task<long> SPublishAsync(string shardChannel, byte[] message, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(PubSubCommands.SPublish(shardChannel, message), ResultType.Int64, cancellationToken);
        }

        #region Subscribe
        /// <summary>
        /// Subscribes the client to the specified channels.
        /// <para>Available since:2.0.0</para>
        /// <para>订阅一个或多个通道</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="channels">channels
        /// <para>要订阅的通道数组</para>
        /// </param>
        /// <param name="onReceive">Receive subscription event
        /// <para>收到订阅消息事件</para>
        /// </param>
        /// <param name="dataType">Subscribe to the received data format, default by string
        /// <para>订阅收到的数据格式, 默认为string, 可设置为byte[]</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public Task SubscribeAsync(string[] channels, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
            return this.SubscribeReturnConnectionAsync(channels, onReceive, dataType, cancellationToken);
        }

        /// <summary>
        /// Subscribes the client to the specified channels. Return the client connection ID
        /// <para>Available since:2.0.0</para>
        /// <para>订阅一个或多个通道, 并返回连接ID</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="channels">channels
        /// <para>要订阅的通道数组</para>
        /// </param>
        /// <param name="onReceive">Receive subscription event
        /// <para>收到订阅消息事件</para>
        /// </param>
        /// <param name="dataType">Subscribe to the received data format, default by string
        /// <para>订阅收到的数据格式, 默认为string, 可设置为byte[]</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Client ID
        /// <para>连接ID</para>
        /// </returns>
        public async Task<long> SubscribeReturnClientIDAsync(string[] channels, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
            var connection = await this.SubscribeReturnConnectionAsync(channels, onReceive, dataType, cancellationToken).ConfigureAwait(false);
            return connection.ConnectionId;
        }

        /// <summary>
        /// Subscribes the client to the specified channel.
        /// <para>Available since:2.0.0</para>
        /// <para>订阅一个通道</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="channel">Channel
        /// <para>要订阅的通道</para>
        /// </param>
        /// <param name="onReceive">Receive subscription event
        /// <para>收到订阅消息事件</para>
        /// </param>
        /// <param name="dataType">Subscribe to the received data format, default by string
        /// <para>订阅收到的数据格式, 默认为string, 可设置为byte[]</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public Task SubscribeAsync(string channel, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return this.SubscribeAsync([channel], onReceive, dataType, cancellationToken);
#else
            return this.SubscribeAsync(new string[] { channel }, onReceive, dataType, cancellationToken);
#endif
        }

        /// <summary>
        /// Subscribes the client to the specified channel. Return the client connection ID
        /// <para>Available since:2.0.0</para>
        /// <para>订阅一个通道, 并返回连接ID</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="channel">Channel
        /// <para>要订阅的通道</para>
        /// </param>
        /// <param name="onReceive">Receive subscription event
        /// <para>收到订阅消息事件</para>
        /// </param>
        /// <param name="dataType">Subscribe to the received data format, default by string
        /// <para>订阅收到的数据格式, 默认为string, 可设置为byte[]</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Client ID
        /// <para>连接ID</para>
        /// </returns>
        public async Task<long> SubscribeReturnClientIDAsync(string channel, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            var connection = await this.SubscribeReturnConnectionAsync([channel], onReceive, dataType, cancellationToken).ConfigureAwait(false);
#else
            var connection = await this.SubscribeReturnConnectionAsync(new string[] { channel }, onReceive, dataType, cancellationToken).ConfigureAwait(false);
#endif
            return connection.ConnectionId;
        }

        /// <summary>
        /// Subscribes the client to the specified shard channels.
        /// <para>Available since:7.0.0</para>
        /// <para>订阅到指定的一个或多个分片通道</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="shardChannels">Shard channels
        /// <para>分片通道数组</para>
        /// </param>
        /// <param name="onReceive">Receive subscription event
        /// <para>收到订阅消息事件</para>
        /// </param>
        /// <param name="dataType">Subscribe to the received data format, default by string
        /// <para>订阅收到的数据格式, 默认为string, 可设置为byte[]</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public Task SSubscribeAsync(string[] shardChannels, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [SSUBSCRIBE]");
            var afterChannels = shardChannels?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Shard)).ToArray();
            if (afterChannels is null || afterChannels.Length is 0) throw new RedisException("There are no valid channels");
            return base._call.ConnectionPool.GetSubConnection().SubscribeAsync(PubSubCommands.SSubscribe(), afterChannels, new OnReceiveModel<OnReceive>(dataType, onReceive), cancellationToken);
        }

        /// <summary>
        /// Subscribes the client to the specified shard channel.
        /// <para>Available since:7.0.0</para>
        /// <para>订阅一个指定的分片通道</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="shardChannel">Shard channel
        /// <para>分片通道</para>
        /// </param>
        /// <param name="onReceive">Receive subscription event
        /// <para>收到订阅消息事件</para>
        /// </param>
        /// <param name="dataType">Subscribe to the received data format, default by string
        /// <para>订阅收到的数据格式, 默认为string, 可设置为byte[]</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public Task SSubscribeAsync(string shardChannel, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return this.SSubscribeAsync([shardChannel], onReceive, dataType, cancellationToken);
#else
            return this.SSubscribeAsync(new string[] { shardChannel }, onReceive, dataType, cancellationToken);
#endif
        }

        /// <summary>
        /// Subscribes the client to the given pattern.
        /// <para>Available since:2.0.0</para>
        /// <para>单个匹配规则订阅</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="pattern">Pattern
        /// <para>匹配规则</para>
        /// </param>
        /// <param name="onReceive">Receive subscription event
        /// <para>收到订阅消息事件</para>
        /// </param>
        /// <param name="dataType">Subscribe to the received data format, default by string
        /// <para>订阅收到的数据格式, 默认为string, 可设置为byte[]</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public Task PSubscribeAsync(string pattern, POnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return this.PSubscribeAsync([pattern], onReceive, dataType, cancellationToken);
#else
            return this.PSubscribeAsync(new string[] { pattern }, onReceive, dataType, cancellationToken);
#endif
        }

        /// <summary>
        /// Subscribes the client to the given patterns.
        /// <para>Available since:2.0.0</para>
        /// <para>多个匹配规则订阅</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="patterns">Patterns
        /// <para>匹配规则数组</para>
        /// </param>
        /// <param name="onReceive">Receive subscription event
        /// <para>收到订阅消息事件</para>
        /// </param>
        /// <param name="dataType">Subscribe to the received data format, default by string
        /// <para>订阅收到的数据格式, 默认为string, 可设置为byte[]</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public Task PSubscribeAsync(string[] patterns, POnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [PSUBSCRIBE]");
            var afterPatterns = patterns?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Default)).ToArray();
            if (afterPatterns is null || afterPatterns.Length is 0) throw new RedisException("There are no valid patterns");
            return base._call.ConnectionPool.GetSubConnection().PSubscribeAsync(PubSubCommands.PSubscribe(), afterPatterns, new OnReceiveModel<POnReceive>(dataType, onReceive), cancellationToken);
        }

        /// <summary>
        /// Unsubscribes the client from the given channels
        /// <para>Available since:2.0.0</para>
        /// <para>取消指定的一个或多个通道订阅.</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="channels">The given channels
        /// <para>指定要取消的通道数组, 可以一次性多个</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public async Task UnSubscribeAsync(string[] channels, CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [UNSUBSCRIBE]");
            var afterChannels = channels?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Default)).ToArray();
            if (afterChannels is null || afterChannels.Length is 0) throw new RedisException("There are no valid channels");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
                await connection.UnSubscribeAsync(PubSubCommands.UnSubscribe(), afterChannels, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Unsubscribes the client from the given channel
        /// <para>Available since:2.0.0</para>
        /// <para>取消指定的一个通道订阅.</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="channel">The given channel
        /// <para>要取消的订阅通道</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public Task UnSubscribeAsync(string channel, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return this.UnSubscribeAsync([channel], cancellationToken);
#else
            return this.UnSubscribeAsync(new string[] { channel }, cancellationToken);
#endif
        }

        /// <summary>
        /// Unsubscribes the client all channels
        /// <para>Available since:2.0.0</para>
        /// <para>取消所有的通道订阅</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        public async Task UnSubscribeAllAsync(CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [UNSUBSCRIBE]");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                await connection.UnSubscribeAsync(PubSubCommands.UnSubscribe(), null!, cancellationToken).ConfigureAwait(false);
#else
                await connection.UnSubscribeAsync(PubSubCommands.UnSubscribe(), null, cancellationToken).ConfigureAwait(false);
#endif
            }
        }

        /// <summary>
        /// Unsubscribes the client from the given patterns
        /// <para>Available since:2.0.0</para>
        /// <para>取消指定的一个或多个匹配规则订阅.</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="patterns">The given patterns
        /// <para>要取消的匹配规则订阅, 可以一次性多个</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public async Task PUnSubscribeAsync(string[] patterns, CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [PUNSUBSCRIBE]");
            var afterPatterns = patterns?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Default)).ToArray();
            if (afterPatterns is null || afterPatterns.Length is 0) throw new RedisException("There are no valid patterns");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
                await connection.UnSubscribeAsync(PubSubCommands.PUnSubscribe(), afterPatterns, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Unsubscribes the client from the given pattern
        /// <para>Available since:2.0.0</para>
        /// <para>取消指定的一个匹配规则订阅.</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="pattern">The given pattern
        /// <para>要取消的匹配规则订阅</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public Task PUnSubscribeAsync(string pattern, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return this.PUnSubscribeAsync([pattern], cancellationToken);
#else
            return this.PUnSubscribeAsync(new string[] { pattern }, cancellationToken);
#endif
        }

        /// <summary>
        /// Unsubscribes the client all patterns
        /// <para>Available since:2.0.0</para>
        /// <para>取消所有的匹配规则订阅</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        public async Task PUnSubscribeAllAsync(CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [PUNSUBSCRIBE]");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                await connection.UnSubscribeAsync(PubSubCommands.PUnSubscribe(), null!, cancellationToken).ConfigureAwait(false);
#else
                await connection.UnSubscribeAsync(PubSubCommands.PUnSubscribe(), null, cancellationToken).ConfigureAwait(false);
#endif
            }
        }

        /// <summary>
        /// Unsubscribes the client from the given shard channels.
        /// <para>Available since:7.0.0</para>
        /// <para>从给定的分片通道取消一个或多个订阅</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="shardChannels">Shard channels
        /// <para>要取消的分片通道名称数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public async Task SUnSubscribeAsync(string[] shardChannels, CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [SUNSUBSCRIBE]");
            var afterPatterns = shardChannels?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Shard)).ToArray();
            if (afterPatterns is null || afterPatterns.Length is 0) throw new RedisException("There are no valid patterns");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
                await connection.UnSubscribeAsync(PubSubCommands.SUnSubscribe(), afterPatterns, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Unsubscribes the client from the given shard channel.
        /// <para>Available since:7.0.0</para>
        /// <para>从给定的分片通道取消一个订阅</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="shardChannel">Shard channel
        /// <para>要取消的分片通道名称</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public Task SUnSubscribeAsync(string shardChannel, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return this.SUnSubscribeAsync([shardChannel], cancellationToken);
#else
            return this.SUnSubscribeAsync(new string[] { shardChannel }, cancellationToken);
#endif
        }

        /// <summary>
        /// Unsubscribes the client all shard channel
        /// <para>Available since:7.0.0</para>
        /// <para>取消所有分片通道订阅</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Task</returns>
        public async Task SUnSubscribeAllAsync(CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [SUNSUBSCRIBE]");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                await connection.UnSubscribeAsync(PubSubCommands.SUnSubscribe(), null!, cancellationToken).ConfigureAwait(false);
#else
                await connection.UnSubscribeAsync(PubSubCommands.SUnSubscribe(), null, cancellationToken).ConfigureAwait(false);
#endif
            }
        }

        private async Task<SubConnection> SubscribeReturnConnectionAsync(string[] channels, OnReceive onReceive, ResultDataType dataType, CancellationToken cancellationToken)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [SUBSCRIBE]");
            var afterChannels = channels?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Default)).ToArray();
            if (afterChannels is null || afterChannels.Length is 0) throw new RedisException("There are no valid channels");
            var connection = base._call.ConnectionPool.GetSubConnection();
            await connection.SubscribeAsync(PubSubCommands.Subscribe(), afterChannels, new OnReceiveModel<OnReceive>(dataType, onReceive), cancellationToken).ConfigureAwait(false);
            return connection;
        }
        #endregion

        /// <summary>
        /// Lists the currently active channels.
        /// <para>Available since:2.8.0</para>
        /// <para>列出当前所有活动的订阅通道</para>
        /// <para>支持的Redis版本: 2.8.0+</para>
        /// </summary>
        /// <param name="pattern">If no pattern is specified, all the channels are listed, otherwise if pattern is specified only channels matching the specified glob-style pattern are listed.
        /// <para>筛选匹配规则</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public Task<string?[]?> PubSubChannelsAsync(string? pattern = null, CancellationToken cancellationToken = default)
#else
        public Task<string[]> PubSubChannelsAsync(string pattern = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArrayAsync<string>(PubSubCommands.PubSubChannels(pattern), ResultType.String | ResultType.Array, cancellationToken);
        }

        /// <summary>
        /// Returns the number of unique patterns that are subscribed to by clients (that are performed using the PSUBSCRIBE command).
        /// <para>Available since:2.8.0</para>
        /// <para>获得所有客户端匹配规则订阅的数量</para>
        /// <para>支持的Redis版本: 2.8.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of patterns all the clients are subscribed to.
        /// <para>所有客户端匹配规则订阅的数量</para>
        /// </returns>
        public Task<long> PubSubNumPAtAsync(CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(PubSubCommands.PubSubNumPAt(), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the number of subscribers (exclusive of clients subscribed to patterns) for the specified channels.
        /// <para>Available since:2.8.0</para>
        /// <para>返回指定通道的订阅者数量（不包括匹配规则的订阅）</para>
        /// <para>支持的Redis版本: 2.8.0+</para>
        /// </summary>
        /// <param name="channels">Channels
        /// <para>指定的一个或多个筛选通道</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of subscribers per channel, key is channel name, value is the number of subscribers
        /// <para>每个通道订阅者的数量, Key为通道名称, Value为订阅者数量</para>
        /// </returns>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public Task<Dictionary<string, long>?> PubSubNumSubAsync(string[] channels, CancellationToken cancellationToken = default)
#else
        public Task<Dictionary<string, long>> PubSubNumSubAsync(string[] channels, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallDictionaryStructValueAsync<long>(PubSubCommands.PubSubNumSub(channels), ResultType.Dictionary | ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the number of subscribers (exclusive of clients subscribed to patterns) for the specified channel.
        /// <para>Available since:2.8.0</para>
        /// <para>返回指定通道的订阅者数量（不包括匹配规则的订阅）</para>
        /// <para>支持的Redis版本: 2.8.0+</para>
        /// </summary>
        /// <param name="channel">Channel
        /// <para>指定的一个通道</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of subscribers per channel, key is channel name, value is the number of subscribers
        /// <para>每个通道订阅者的数量, Key为通道名称, Value为订阅者数量</para>
        /// </returns>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public Task<Dictionary<string, long>?> PubSubNumSubAsync(string channel, CancellationToken cancellationToken = default)
#else
        public Task<Dictionary<string, long>> PubSubNumSubAsync(string channel, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return this.PubSubNumSubAsync([channel], cancellationToken);
#else
            return this.PubSubNumSubAsync(new string[] { channel }, cancellationToken);
#endif
        }

        /// <summary>
        /// Lists the currently active shard channels.
        /// <para>Available since:7.0.0</para>
        /// <para>列出当前活动的分片通道</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="pattern">Pattern
        /// <para>筛选匹配规则</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A list of active channels, optionally matching the specified pattern.
        /// <para>活动的通道数组</para>
        /// </returns>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public Task<string?[]?> PubSubShardChannelsAsync(string? pattern = null, CancellationToken cancellationToken = default)
#else
        public Task<string[]> PubSubShardChannelsAsync(string pattern = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArrayAsync<string>(PubSubCommands.PubSubShardChannels(pattern), ResultType.Array | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns the number of subscribers for the specified shard channels.
        /// <para>Available since:7.0.0</para>
        /// <para>返回指定分片通道的订阅者数量</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="shardchannels">shardchannels
        /// <para>指定的一个或多个分片通道名称</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of subscribers per shard channel, Key is channel name, Value is the number of subscribers.
        /// <para>分片通道订阅者数量, Key为分片通道名称, Value为分片通道订阅者数量</para>
        /// </returns>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public Task<Dictionary<string, long>?> PubSubShardNumSubAsync(string[] shardchannels, CancellationToken cancellationToken = default)
#else
        public Task<Dictionary<string, long>> PubSubShardNumSubAsync(string[] shardchannels, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallDictionaryStructValueAsync<long>(PubSubCommands.PubSubShardNumSub(shardchannels), ResultType.Dictionary | ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the number of subscribers for the specified shard channel.
        /// <para>Available since:7.0.0</para>
        /// <para>返回指定分片通道的订阅者数量</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="shardchannel">shardchannel
        /// <para>指定的通道名称</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of subscribers per shard channel, Key is channel name, Value is the number of subscribers.
        /// <para>分片通道订阅者数量, Key为分片通道名称, Value为分片通道订阅者数量</para>
        /// </returns>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public Task<Dictionary<string, long>?> PubSubShardNumSubAsync(string shardchannel, CancellationToken cancellationToken = default)
#else
        public Task<Dictionary<string, long>> PubSubShardNumSubAsync(string shardchannel, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return this.PubSubShardNumSubAsync([shardchannel], cancellationToken);
#else
            return this.PubSubShardNumSubAsync(new string[] { shardchannel }, cancellationToken);
#endif
        }
    }
}
#endif
