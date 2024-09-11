#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
using SharpRedis.Models;

namespace SharpRedis.Commands
{
    internal static class PubSubCommands
    {
        internal static CommandPacket Publish<TMessage>(string channel, TMessage message) where TMessage : class
        {
            return new CommandPacket("PUBLISH", CommandMode.Pub)
                .WriteValue(channel)
                .WriteValue(message);
        }

        internal static CommandPacket SPublish<TMessage>(string shardChannel, TMessage message) where TMessage : class
        {
            return new CommandPacket("SPUBLISH", CommandMode.Pub)
                .WriteValue(shardChannel)
                .WriteValue(message);
        }

        internal static CommandPacket Subscribe() => new CommandPacket("SUBSCRIBE", CommandMode.Sub | CommandMode.WithoutResult);

        internal static CommandPacket SSubscribe() => new CommandPacket("SSUBSCRIBE", CommandMode.Sub | CommandMode.WithoutResult);

        internal static CommandPacket PSubscribe() => new CommandPacket("PSUBSCRIBE", CommandMode.Sub | CommandMode.WithoutResult);

        internal static CommandPacket UnSubscribe() => new CommandPacket("UNSUBSCRIBE", CommandMode.UnSub | CommandMode.WithoutResult);

        internal static CommandPacket PUnSubscribe() => new CommandPacket("PUNSUBSCRIBE", CommandMode.UnSub | CommandMode.WithoutResult);

        internal static CommandPacket SUnSubscribe() => new CommandPacket("SUNSUBSCRIBE", CommandMode.UnSub | CommandMode.WithoutResult);

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket PubSubChannels(string? pattern)
#else
        internal static CommandPacket PubSubChannels(string pattern)
#endif
        {
            return new CommandPacket("PUBSUB", CommandMode.Pub)
                .WriteArg("CHANNELS")
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteValue(!string.IsNullOrEmpty(pattern), pattern!);
#else
                .WriteValue(!string.IsNullOrEmpty(pattern), pattern);
#endif
        }

        internal static CommandPacket PubSubNumPAt() => new CommandPacket("PUBSUB", CommandMode.Pub).WriteArg("NUMPAT");

        internal static CommandPacket PubSubNumSub(string[] channels)
        {
            return new CommandPacket("PUBSUB", CommandMode.Pub)
                .WriteArg("NUMSUB")
                .WriteValues(channels);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket PubSubShardChannels(string? pattern)
#else
        internal static CommandPacket PubSubShardChannels(string pattern)
#endif
        {
            return new CommandPacket("PUBSUB", CommandMode.Pub)
                .WriteArg("SHARDCHANNELS")
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteValue(!string.IsNullOrEmpty(pattern), pattern!);
#else
                .WriteValue(!string.IsNullOrEmpty(pattern), pattern);
#endif
        }

        internal static CommandPacket PubSubShardNumSub(string[] shardchannels)
        {
            return new CommandPacket("PUBSUB", CommandMode.Pub)
                .WriteArg("SHARDNUMSUB")
                .WriteValues(shardchannels);
        }
    }
}
