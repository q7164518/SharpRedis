using SharpRedis.Commands;
using SharpRedis.Entities.Command;
using SharpRedis.Extensions;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis Pub/Sub
/// <para>Redis发布订阅操作类型</para>
/// <para>If you are new to Pub/sub, see <see href="https://redis.io/docs/develop/pubsub/">Introduction to Redis Pub/sub</see></para>
/// <para>如果你不太懂Pub/sub类型, 请<see href="https://redis.io/docs/develop/pubsub/">查看Pub/sub文档</see></para>
/// </summary>
public sealed partial class RedisPubSub : RedisTypeBase
{
    internal RedisPubSub(BaseRedis redis)
        : base(redis)
    { }

    /// <summary>
    /// Subscribes the client to the specified channel.
    /// <para>Available since: 2.0.0</para>
    /// <para>订阅一个频道消息</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="channel">channel<para>要订阅的频道名称</para></param>
    /// <param name="onReceive">Handling of subscription message receipt requests
    /// <para>收到订阅消息的处理委托</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>When successful, this command doesn't return anything
    /// <para>订阅成功, 不会返回任何结果</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Subscribe">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Subscribe">命令文档</see></para>
    /// </remarks>
    public void Subscribe(string channel, OnReceive onReceive, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        if (channel == Config.CLIENT_CACHING_SUB_CHANNEL) throw new NotSupportedException($"[{Config.CLIENT_CACHING_SUB_CHANNEL}] is a system channel and cannot be subscribed to.");
        base.Invoke(new CommandSubscribe(PubSubCommands.Subscribe(channel) | flags, onReceive), cancellationToken);
    }

    /// <summary>
    /// Subscribes the client to the specified channels.
    /// <para>Available since: 2.0.0</para>
    /// <para>订阅多个频道消息</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="channels">channel array<para>要订阅的频道名称集合</para></param>
    /// <param name="onReceive">Handling of subscription message receipt requests
    /// <para>收到订阅消息的处理委托</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>When successful, this command doesn't return anything
    /// <para>订阅成功, 不会返回任何结果</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Subscribe">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Subscribe">命令文档</see></para>
    /// </remarks>
    public void Subscribe(ICollection<string> channels, OnReceive onReceive, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        if (channels.Contains(Config.CLIENT_CACHING_SUB_CHANNEL)) throw new NotSupportedException($"[{Config.CLIENT_CACHING_SUB_CHANNEL}] is a system channel and cannot be subscribed to.");
        base.Invoke(new CommandSubscribe(PubSubCommands.Subscribe(new SharpValueSpan(channels)) | flags, onReceive), cancellationToken);
    }

    /// <summary>
    /// Subscribes the client to the given pattern.
    /// <para>Available since: 2.0.0</para>
    /// <para>根据一个匹配规则订阅频道</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="pattern">
    /// h?llo subscribes to hello, hallo and hxllo
    /// <para>h*llo subscribes to hllo and heeeello</para>
    /// <para>h[ae]llo subscribes to hello and hallo, but not hillo</para>
    /// <para>匹配符支持: ?表现任意一个字符, *表示任意多个字符, [az]表示az出现其中一个</para>
    /// </param>
    /// <param name="onReceive">Handling of subscription message receipt requests
    /// <para>收到订阅消息的处理委托</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>When successful, this command doesn't return anything
    /// <para>订阅成功, 不会返回任何结果</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PSubscribe">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PSubscribe">命令文档</see></para>
    /// </remarks>
    public void PSubscribe(string pattern, POnReceive onReceive, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(new CommandSubscribe(PubSubCommands.PSubscribe(pattern) | flags, onReceive), cancellationToken);

    /// <summary>
    /// Subscribes the client to the given patterns.
    /// <para>Available since: 2.0.0</para>
    /// <para>根据多个匹配规则订阅频道</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="patterns">
    /// h?llo subscribes to hello, hallo and hxllo
    /// <para>h*llo subscribes to hllo and heeeello</para>
    /// <para>h[ae]llo subscribes to hello and hallo, but not hillo</para>
    /// <para>匹配符支持: ?表现任意一个字符, *表示任意多个字符, [az]表示az出现其中一个</para>
    /// </param>
    /// <param name="onReceive">Handling of subscription message receipt requests
    /// <para>收到订阅消息的处理委托</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>When successful, this command doesn't return anything
    /// <para>订阅成功, 不会返回任何结果</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/PSubscribe">Command documentation</see>
    /// <para><see href="https://redis.io/commands/PSubscribe">命令文档</see></para>
    /// </remarks>
    public void PSubscribe(ICollection<string> patterns, POnReceive onReceive, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(new CommandSubscribe(PubSubCommands.PSubscribe(new SharpValueSpan(patterns)) | flags, onReceive), cancellationToken);

    /// <summary>
    /// Posts a message to the given channel.
    /// <para>Available since: 2.0.0</para>
    /// <para>向指定频道发布消息</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="channel">channel<para>频道名称</para></param>
    /// <param name="message">message<para>要发布的消息</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of clients that the message was sent to.
    /// <para>收到该消息的客户端(订阅者)数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Publish">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Publish">命令文档</see></para>
    /// </remarks>
    public RedisNumber Publish(string channel, SharpValue message, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        if (channel == Config.CLIENT_CACHING_SUB_CHANNEL) throw new NotSupportedException($"[{Config.CLIENT_CACHING_SUB_CHANNEL}] is a system channel and cannot be publish to.");
        return base.Invoke(PubSubCommands.Publish(channel, message) | flags, cancellationToken);
    }
}
