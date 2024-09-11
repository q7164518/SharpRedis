using SharpRedis.Models;

namespace SharpRedis.Commands
{
    internal static class TransactionCommands
    {
        static internal CommandPacket Discard()
        {
            return new CommandPacket("DISCARD", CommandMode.Transaction);
        }

        static internal CommandPacket Exec()
        {
            return new CommandPacket("EXEC", CommandMode.Transaction);
        }

        static internal CommandPacket Multi()
        {
            return new CommandPacket("MULTI", CommandMode.Transaction);
        }

        static internal CommandPacket Watch(params string[] keys)
        {
            return new CommandPacket("WATCH", CommandMode.Transaction)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteKeys(keys?.Length > 0, keys!);
#else
                .WriteKeys(keys?.Length > 0, keys);
#endif
        }

        static internal CommandPacket Unwatch()
        {
            return new CommandPacket("UNWATCH", CommandMode.Transaction);
        }
    }
}
