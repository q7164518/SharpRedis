#if !NET30
using System.Linq;
#endif
#if !LOW_NET
using System.Threading;
#else
#pragma warning disable IDE0130
#endif
using SharpRedis.Commands;
using SharpRedis.Models;
using SharpRedis.Network;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Generic;

namespace SharpRedis
{
    /// <summary>
    /// Redis publishes subscription operations classes
    /// <para>Redis发布订阅操作类</para>
    /// </summary>
    public sealed partial class RedisPubSub : BaseType
    {
        internal RedisPubSub(BaseCall call) : base(call)
        {
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
        public long Publish(string channel, string message, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(PubSubCommands.Publish(channel, message), ResultType.Int64, cancellationToken);
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
        public long Publish(string channel, byte[] message, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(PubSubCommands.Publish(channel, message), ResultType.Int64, cancellationToken);
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
        public long SPublish(string shardChannel, string message, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(PubSubCommands.SPublish(shardChannel, message), ResultType.Int64, cancellationToken);
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
        public long SPublish(string shardChannel, byte[] message, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(PubSubCommands.SPublish(shardChannel, message), ResultType.Int64, cancellationToken);
        }

        #region Subscribe
        private SubConnection SubscribeReturnConnection(string[] channels, OnReceive onReceive, ResultDataType dataType, CancellationToken cancellationToken)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [SUBSCRIBE]");
#if NET30
            var afterChannels = RedisPubSub.ChannelNamesToChannelModes(channels, ChannelModeEnum.Default);
#else
            var afterChannels = channels?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Default)).ToArray();
#endif
            if (afterChannels is null || afterChannels.Length is 0) throw new RedisException("There are no valid channels");
            var connection = base._call.ConnectionPool.GetSubConnection();
            connection.Subscribe(PubSubCommands.Subscribe(), afterChannels, new OnReceiveModel<OnReceive>(dataType, onReceive), cancellationToken);
            return connection;
        }

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
        public void Subscribe(string[] channels, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
            _ = this.SubscribeReturnConnection(channels, onReceive, dataType, cancellationToken);
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
        public long SubscribeReturnClientID(string[] channels, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
            var connection = this.SubscribeReturnConnection(channels, onReceive, dataType, cancellationToken);
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
        public void Subscribe(string channel, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            this.Subscribe([channel], onReceive, dataType, cancellationToken);
#else
            this.Subscribe(new string[] { channel }, onReceive, dataType, cancellationToken);
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
        public long SubscribeReturnClientID(string channel, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            var connection = this.SubscribeReturnConnection([channel], onReceive, dataType, cancellationToken);
#else
            var connection = this.SubscribeReturnConnection(new string[] { channel }, onReceive, dataType, cancellationToken);
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
        public void SSubscribe(string[] shardChannels, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [SSUBSCRIBE]");
#if NET30
            var afterChannels = RedisPubSub.ChannelNamesToChannelModes(shardChannels, ChannelModeEnum.Shard);
#else
            var afterChannels = shardChannels?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Shard)).ToArray();
#endif
            if (afterChannels is null || afterChannels.Length is 0) throw new RedisException("There are no valid channels");
            base._call.ConnectionPool.GetSubConnection().Subscribe(PubSubCommands.SSubscribe(), afterChannels, new OnReceiveModel<OnReceive>(dataType, onReceive), cancellationToken);
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
        public void SSubscribe(string shardChannel, OnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            this.SSubscribe([shardChannel], onReceive, dataType, cancellationToken);
#else
            this.SSubscribe(new string[] { shardChannel }, onReceive, dataType, cancellationToken);
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
        public void PSubscribe(string pattern, POnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            this.PSubscribe([pattern], onReceive, dataType, cancellationToken);
#else
            this.PSubscribe(new string[] { pattern }, onReceive, dataType, cancellationToken);
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
        public void PSubscribe(string[] patterns, POnReceive onReceive, ResultDataType dataType = ResultDataType.Default, CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [PSUBSCRIBE]");
#if NET30
            var afterPatterns = RedisPubSub.ChannelNamesToChannelModes(patterns, ChannelModeEnum.Default);
#else
            var afterPatterns = patterns?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Default)).ToArray();
#endif
            if (afterPatterns is null || afterPatterns.Length is 0) throw new RedisException("There are no valid patterns");
            base._call.ConnectionPool.GetSubConnection().PSubscribe(PubSubCommands.PSubscribe(), afterPatterns, new OnReceiveModel<POnReceive>(dataType, onReceive), cancellationToken);
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
        public void UnSubscribe(string[] channels, CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [UNSUBSCRIBE]");
#if NET30
            var afterChannels = RedisPubSub.ChannelNamesToChannelModes(channels, ChannelModeEnum.Default);
#else
            var afterChannels = channels?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Default)).ToArray();
#endif
            if (afterChannels is null || afterChannels.Length is 0) throw new RedisException("There are no valid channels");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
                connection.UnSubscribe(PubSubCommands.UnSubscribe(), afterChannels, cancellationToken);
            }
        }

        /// <summary>
        /// Unsubscribes the client from the given channel
        /// <para>Available since:2.0.0</para>
        /// <para>取消指定的一个通道订阅.</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="channel">The given channel
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        public void UnSubscribe(string channel, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            this.UnSubscribe([channel], cancellationToken);
#else
            this.UnSubscribe(new string[] { channel }, cancellationToken);
#endif
        }

        /// <summary>
        /// Unsubscribes the client all channels
        /// <para>Available since:2.0.0</para>
        /// <para>取消所有的通道订阅</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        public void UnSubscribeAll(CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [UNSUBSCRIBE]");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                connection.UnSubscribe(PubSubCommands.UnSubscribe(), null!, cancellationToken);
#else
                connection.UnSubscribe(PubSubCommands.UnSubscribe(), null, cancellationToken);
#endif
            }
        }

        /// <summary>
        /// Unsubscribes the client from the given pattern
        /// <para>Available since:2.0.0</para>
        /// <para>取消指定的一个匹配规则订阅.</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="pattern">The given patterns
        /// <para>要取消的匹配规则订阅</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        public void PUnSubscribe(string pattern, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            this.PUnSubscribe([pattern], cancellationToken);
#else
            this.PUnSubscribe(new string[] { pattern }, cancellationToken);
#endif
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
        public void PUnSubscribe(string[] patterns, CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [PUNSUBSCRIBE]");
#if NET30
            var afterPatterns = RedisPubSub.ChannelNamesToChannelModes(patterns, ChannelModeEnum.Default);
#else
            var afterPatterns = patterns?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Default)).ToArray();
#endif
            if (afterPatterns is null || afterPatterns.Length is 0) throw new RedisException("There are no valid patterns");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
                connection.UnSubscribe(PubSubCommands.PUnSubscribe(), afterPatterns, cancellationToken);
            }
        }

        /// <summary>
        /// Unsubscribes the client all patterns
        /// <para>Available since:2.0.0</para>
        /// <para>取消所有的匹配规则订阅</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        public void PUnSubscribeAll(CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [PUNSUBSCRIBE]");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                connection.UnSubscribe(PubSubCommands.PUnSubscribe(), null!, cancellationToken);
#else
                connection.UnSubscribe(PubSubCommands.PUnSubscribe(), null, cancellationToken);
#endif
            }
        }

        /// <summary>
        /// Unsubscribes the client from the given shard channel.
        /// <para>Available since:7.0.0</para>
        /// <para>从给定的分片通道取消一个订阅</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="shardChannel">Shard channels
        /// <para>要取消的分片通道名称</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        public void SUnSubscribe(string shardChannel, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            this.SUnSubscribe([shardChannel], cancellationToken);
#else
            this.SUnSubscribe(new string[] { shardChannel }, cancellationToken);
#endif
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
        public void SUnSubscribe(string[] shardChannels, CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [SUNSUBSCRIBE]");
#if NET30
            var afterChannels = RedisPubSub.ChannelNamesToChannelModes(shardChannels, ChannelModeEnum.Shard);
#else
            var afterChannels = shardChannels?.Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).Select(f => new ChannelMode(f, ChannelModeEnum.Shard)).ToArray();
#endif
            if (afterChannels is null || afterChannels.Length is 0) throw new RedisException("There are no valid patterns");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
                connection.UnSubscribe(PubSubCommands.SUnSubscribe(), afterChannels, cancellationToken);
            }
        }

        /// <summary>
        /// Unsubscribes the client all shard channel
        /// <para>Available since:7.0.0</para>
        /// <para>取消所有分片通道订阅</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        public void SUnSubscribeAll(CancellationToken cancellationToken = default)
        {
            if (!base._call.SubUsable) throw new NotSupportedException($"{base._call.CallMode} does not support the command [SUNSUBSCRIBE]");
            var allConnection = base._call.ConnectionPool.GetAllSubConnections();
            if (allConnection is null || allConnection.Length is 0) return;
            foreach (var connection in allConnection)
            {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                connection.UnSubscribe(PubSubCommands.SUnSubscribe(), null!, cancellationToken);
#else
                connection.UnSubscribe(PubSubCommands.SUnSubscribe(), null, cancellationToken);
#endif
            }
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
        public string?[]? PubSubChannels(string? pattern = null, CancellationToken cancellationToken = default)
#else
        public string[] PubSubChannels(string pattern = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(PubSubCommands.PubSubChannels(pattern), ResultType.String | ResultType.Array, cancellationToken);
        }

        /// <summary>
        /// Returns the number of unique patterns that are subscribed to by clients (that are performed using the PSUBSCRIBE command).
        /// <para>Available since:2.8.0</para>
        /// <para>获得所有客户端匹配规则订阅的数量</para>
        /// <para>支持的Redis版本: 2.8.0+</para>
        /// </summary>
        /// <returns>The number of patterns all the clients are subscribed to.
        /// <para>所有客户端匹配规则订阅的数量</para>
        /// </returns>
        public long PubSubNumPAt(CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(PubSubCommands.PubSubNumPAt(), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the number of subscribers (exclusive of clients subscribed to patterns) for the specified channel.
        /// <para>Available since:2.8.0</para>
        /// <para>返回指定通道的订阅者数量（不包括匹配规则的订阅）</para>
        /// <para>支持的Redis版本: 2.8.0+</para>
        /// </summary>
        /// <param name="channel">Channel
        /// <para>指定的一个筛选通道</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of subscribers per channel, key is channel name, value is the number of subscribers
        /// <para>每个通道订阅者的数量, Key为通道名称, Value为订阅者数量</para>
        /// </returns>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public Dictionary<string, long>? PubSubNumSub(string channel, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, long> PubSubNumSub(string channel, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return this.PubSubNumSub([channel], cancellationToken);
#else
            return this.PubSubNumSub(new string[] { channel }, cancellationToken);
#endif
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
        public Dictionary<string, long>? PubSubNumSub(string[] channels, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, long> PubSubNumSub(string[] channels, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallDictionaryStructValue<long>(PubSubCommands.PubSubNumSub(channels), ResultType.Dictionary | ResultType.Int64, cancellationToken);
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
        public string?[]? PubSubShardChannels(string? pattern = null, CancellationToken cancellationToken = default)
#else
        public string[] PubSubShardChannels(string pattern = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(PubSubCommands.PubSubShardChannels(pattern), ResultType.Array | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Returns the number of subscribers for the specified shard channel.
        /// <para>Available since:7.0.0</para>
        /// <para>返回指定分片通道的订阅者数量</para>
        /// <para>支持的Redis版本: 7.0.0+</para>
        /// </summary>
        /// <param name="shardchannel">shardchannels
        /// <para>指定的一个分片通道名称</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of subscribers per shard channel, Key is channel name, Value is the number of subscribers.
        /// <para>分片通道订阅者数量, Key为分片通道名称, Value为分片通道订阅者数量</para>
        /// </returns>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public Dictionary<string, long>? PubSubShardNumSub(string shardchannel, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, long> PubSubShardNumSub(string shardchannel, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return this.PubSubShardNumSub([shardchannel], cancellationToken);
#else
            return this.PubSubShardNumSub(new string[] { shardchannel }, cancellationToken);
#endif
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
        public Dictionary<string, long>? PubSubShardNumSub(string[] shardchannels, CancellationToken cancellationToken = default)
#else
        public Dictionary<string, long> PubSubShardNumSub(string[] shardchannels, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallDictionaryStructValue<long>(PubSubCommands.PubSubShardNumSub(shardchannels), ResultType.Dictionary | ResultType.Int64, cancellationToken);
        }

#if NET30
        private static ChannelMode[] ChannelNamesToChannelModes(string[] channels, ChannelModeEnum channelMode)
        {
            if (channels?.Length > 0)
            {
                var afterChannelList = new List<ChannelMode>();
                for (uint i = 0; i < channels.Length; i++)
                {
                    var channel = channels[i].Trim();
                    if (!string.IsNullOrEmpty(channel))
                    {
                        afterChannelList.Add(new ChannelMode(channel, channelMode));
                    }
                }
                if (afterChannelList.Count > 0) return afterChannelList.ToArray();
            }
            return null;
        }
#endif
    }
}
