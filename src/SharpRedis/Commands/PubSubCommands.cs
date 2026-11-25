using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using System;

namespace SharpRedis.Commands;

internal static class PubSubCommands
{
    private readonly static SharpValue _subscribe = new("SUBSCRIBE", CommandItemCategory.Command);
    private readonly static SharpValue _publish = new("PUBLISH", CommandItemCategory.Command);
    private readonly static SharpValue _psubscribe = new("PSUBSCRIBE", CommandItemCategory.Command);

    internal static CommandPacket Subscribe(string channel)
    {
        return new CommandPacket(PubSubCommands._subscribe, CommandMode.Sub | CommandMode.WithoutResult | CommandMode.DoNotReuseConnection)
            .WriteArguments(channel);
    }

    internal static CommandPacket Subscribe(SharpValueSpan channels)
    {
        if (channels.IsEmpty) throw new NotSupportedException("Please specify at least one channel.");
        return new CommandPacket(PubSubCommands._subscribe, CommandMode.Sub | CommandMode.WithoutResult | CommandMode.DoNotReuseConnection)
            .WriteArguments(channels);
    }

    internal static CommandPacket Publish(string channel, SharpValue message)
    {
        return new CommandPacket(PubSubCommands._publish, CommandMode.Pub)
            .WriteArguments(channel)
            .WriteArguments(message);
    }

    internal static CommandPacket PSubscribe(string pattern)
    {
        return new CommandPacket(PubSubCommands._psubscribe, CommandMode.PSub | CommandMode.WithoutResult | CommandMode.DoNotReuseConnection)
            .WriteArguments(pattern);
    }

    internal static CommandPacket PSubscribe(SharpValueSpan patterns)
    {
        if (patterns.IsEmpty) throw new NotSupportedException("Please specify at least one pattern.");
        return new CommandPacket(PubSubCommands._psubscribe, CommandMode.PSub | CommandMode.WithoutResult | CommandMode.DoNotReuseConnection)
            .WriteArguments(patterns);
    }
}
