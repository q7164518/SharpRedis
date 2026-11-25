using SharpRedis.Entities.Command;
using SharpRedis.Enums;

namespace SharpRedis.Commands;

internal static class TransactionCommands
{
    private readonly static SharpValue _multi = new("MULTI", CommandItemCategory.Command);
    private readonly static SharpValue _discard = new("DISCARD", CommandItemCategory.Command);
    private readonly static SharpValue _exec = new("EXEC", CommandItemCategory.Command);
    private readonly static SharpValue _watch = new("WATCH", CommandItemCategory.Command);
    private readonly static SharpValue _unwatch = new("UNWATCH", CommandItemCategory.Command);

    internal static CommandPacket Discard()
        => new(TransactionCommands._discard, Enums.CommandMode.Transaction);

    internal static CommandPacket Exec()
        => new(TransactionCommands._exec, Enums.CommandMode.Transaction);

    internal static CommandPacket Multi()
        => new(TransactionCommands._multi, CommandMode.Transaction | CommandMode.DoNotReuseConnection);

    internal static CommandPacket Unwatch()
        => new(TransactionCommands._unwatch, Enums.CommandMode.Transaction);

    internal static CommandPacket Watch(SharpValue key)
        => new CommandPacket(TransactionCommands._watch, CommandMode.Transaction | CommandMode.DoNotReuseConnection)
            .WriteKeys(key);

    internal static CommandPacket Watch(SharpValueSpan keys)
        => new CommandPacket(TransactionCommands._watch, CommandMode.Transaction | CommandMode.DoNotReuseConnection)
            .WriteKeys(keys);
}
