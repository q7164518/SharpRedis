using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;

namespace SharpRedis.Commands;

internal static class ClusterCommands
{
    private static readonly SharpValue _cluster = new("CLUSTER", CommandItemCategory.Command);
    private static readonly SharpValue _asking = new("ASKING", CommandItemCategory.Command);
    private static readonly SharpValue _readonly = new("READONLY", CommandItemCategory.Command);
    private static readonly SharpValue _readwrite = new("READWRITE", CommandItemCategory.Command);
    private static readonly SharpValue _slots = new("SLOTS", CommandItemCategory.Command);
    private static readonly SharpValue _nodes = new("NODES", CommandItemCategory.Command);
    private static readonly SharpValue _keyslot = new("KEYSLOT", CommandItemCategory.Command);
    private static readonly SharpValue _addslots = new("ADDSLOTS", CommandItemCategory.Command);
    private static readonly SharpValue _addslotsrange = new("ADDSLOTSRANGE", CommandItemCategory.Command);

    internal static CommandPacket Slots()
    {
        return new CommandPacket(ClusterCommands._cluster, CommandMode.ClusterManagement)
            .WriteCommand(ClusterCommands._slots);
    }

    internal static CommandPacket Asking()
    {
        return new CommandPacket(ClusterCommands._asking, CommandMode.ClusterManagement);
    }

    internal static CommandPacket Readonly()
    {
        return new CommandPacket(ClusterCommands._readonly, CommandMode.ClusterManagement);
    }

    internal static CommandPacket Readwrite()
    {
        return new CommandPacket(ClusterCommands._readwrite, CommandMode.ClusterManagement);
    }

    internal static CommandPacket Nodes()
    {
        return new CommandPacket(ClusterCommands._cluster, CommandMode.ClusterManagement)
            .WriteCommand(ClusterCommands._nodes);
    }

    internal static CommandPacket KeySlot(SharpValue key)
    {
        return new CommandPacket(ClusterCommands._cluster, CommandMode.ClusterManagement)
            .WriteCommand(ClusterCommands._keyslot)
            .WriteKeys(key);
    }

    internal static CommandPacket AddSlots(int slot)
    {
        if (slot < 0 || slot > 16383) throw new ArgumentOutOfRangeException(nameof(slot), "The hash slots range from 0 to 16383.");
        return new CommandPacket(ClusterCommands._cluster, CommandMode.ClusterManagement)
            .WriteCommand(ClusterCommands._addslots)
            .WriteArguments(slot.ToBytes());
    }

    internal static CommandPacket AddSlots(ReadOnlySpan<int> slots)
    {
        if (slots.IsEmpty) throw new ArgumentException("Please specify at least one hash slot.", nameof(slots));
        var command = new CommandPacket(ClusterCommands._cluster, CommandMode.ClusterManagement)
            .WriteCommand(ClusterCommands._addslots);
        foreach (var slot in slots)
        {
            if (slot < 0 || slot > 16383) throw new ArgumentException("The hash slots range from 0 to 16383.");
            _ = command.WriteArguments(slot.ToBytes());
        }
        return command;
    }
}
