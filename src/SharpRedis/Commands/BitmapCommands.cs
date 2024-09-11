using SharpRedis.Models;

namespace SharpRedis.Commands
{
    internal static class BitmapCommands
    {
        static internal CommandPacket SetBit(string key, uint offset, bool value)
        {
            return new CommandPacket("SETBIT", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(offset)
                .WriteValue(value, "1")
                .WriteValue(!value, "0");
        }

        internal static CommandPacket GetBit(string key, uint offset)
        {
            return new CommandPacket("GETBIT", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(offset);
        }

        internal static CommandPacket BitPos(string key, bool bit, long? start = null, long? end = null, ByteBit bb = ByteBit.None)
        {
            if (!start.HasValue && end.HasValue) throw new RedisException("Setting only the end location is not allowed");
            return new CommandPacket("BITPOS", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValue(bit, "1")
                .WriteValue(!bit, "0")
                .WriteArg(start ?? 0)
                .WriteArg(end ?? -1)
                .WriteArg(bb == ByteBit.Bit, "BIT")
                .WriteArg(bb == ByteBit.Byte, "BYTE");
        }

        internal static CommandPacket BitOp(BitOperation bitOperation, string destkey, params string[] keys)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("Ensure that at least one key participates in the calculation");
            if (bitOperation != BitOperation.Not && keys.Length <= 1) throw new RedisException("A non-NOT bit operation requires at least two keys");
            return new CommandPacket("BITOP", CommandMode.Write)
                .WriteArg(bitOperation is BitOperation.And, "AND")
                .WriteArg(bitOperation is BitOperation.Or, "OR")
                .WriteArg(bitOperation is BitOperation.Xor, "XOR")
                .WriteArg(bitOperation is BitOperation.Not, "NOT")
                .WriteKey(destkey)
                .WriteKeys(keys);
        }

        internal static CommandPacket BitCount(string key, long? start = null, long? end = null, ByteBit bb = ByteBit.None)
        {
            if (!start.HasValue && end.HasValue) throw new RedisException("Setting only the end location is not allowed");
            return new CommandPacket("BITCOUNT", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(start ?? 0)
                .WriteArg(end ?? -1)
                .WriteArg(bb == ByteBit.Bit, "BIT")
                .WriteArg(bb == ByteBit.Byte, "BYTE");
        }

        internal static CommandPacket BitField(string key, params IBitFieldArg[] args)
        {
            if (args is null || args.Length == 0) throw new RedisException("The BITFIELD command parameter cannot be empty");
            var command = new CommandPacket("BITFIELD", CommandMode.Write).WriteKey(key);
            for (uint i = 0; i < args.Length; i++)
            {
                if (args[i] is null) throw new RedisException("The BITFIELD command is null");
                command.WriteValues(args[i].Convert());
            }
            return command;
        }

        internal static CommandPacket BitField_Ro(string key, params IBitFieldArg[] args)
        {
            if (args is null || args.Length == 0) throw new RedisException("The BITFIELD command parameter cannot be empty");
            var command = new CommandPacket("BITFIELD_RO", CommandMode.Read | CommandMode.WithClientSideCache).WriteKey(key);
            for (uint i = 0; i < args.Length; i++)
            {
                if (args[i] is null) throw new RedisException("The BITFIELD command is null");
                if (args[i].ArgType != BitFieldArgType.Get) throw new RedisException("The BITFIELD_RO command supports only the GET arg");
                command.WriteValues(args[i].Convert());
            }
            return command;
        }
    }
}
