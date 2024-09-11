#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
using SharpRedis.Models;

namespace SharpRedis.Commands
{
    internal static class ServerCommands
    {
        internal static CommandPacket FlushAll(FlushingMode mode)
        {
            return new CommandPacket("FLUSHALL", CommandMode.Server | CommandMode.Write)
                .WriteArg(mode == FlushingMode.Sync, "SYNC")
                .WriteArg(mode == FlushingMode.Async, "ASYNC");
        }

        internal static CommandPacket FlushDb(FlushingMode mode)
        {
            return new CommandPacket("FLUSHDB", CommandMode.Server | CommandMode.Write)
                .WriteArg(mode == FlushingMode.Sync, "SYNC")
                .WriteArg(mode == FlushingMode.Async, "ASYNC");
        }

        internal static CommandPacket Save()
            => new CommandPacket("SAVE", CommandMode.Server);

        internal static CommandPacket BgSave(bool schedule)
            => new CommandPacket("BGSAVE", CommandMode.Server).WriteArg(schedule, "SCHEDULE");

        internal static CommandPacket Info(params string[] sections)
        {
            return new CommandPacket("INFO", CommandMode.Server)
                .WriteArgs(sections);
        }

        internal static CommandPacket DbSize()
        {
            return new CommandPacket("DBSIZE", CommandMode.Server | CommandMode.Read);
        }

        internal static CommandPacket LastSave()
        {
            return new CommandPacket("LASTSAVE", CommandMode.Server);
        }

        internal static CommandPacket BgRewriteAof()
        {
            return new CommandPacket("BGREWRITEAOF", CommandMode.Server);
        }

        internal static CommandPacket CommandCount()
        {
            return new CommandPacket("COMMAND", CommandMode.Server).WriteArg("COUNT");
        }

        internal static CommandPacket CommandGetKeys(params string[] command)
        {
            return new CommandPacket("COMMAND", CommandMode.Server)
                .WriteArg("GETKEYS")
                .WriteArgs(command);
        }
    }
}
