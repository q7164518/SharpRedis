using SharpRedis.Models;

namespace SharpRedis.Commands
{
    internal static class SetCommands
    {
        internal static CommandPacket SAdd<TMember>(string key, params TMember[] members) where TMember : class
        {
            return new CommandPacket("SADD", CommandMode.Write)
                .WriteKey(key)
                .WriteValues(members);
        }

        internal static CommandPacket SCard(string key)
        {
            return new CommandPacket("SCARD", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
        }

        internal static CommandPacket SDiff(string[] keys)
        {
            return new CommandPacket("SDIFF", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKeys(keys);
        }

        internal static CommandPacket SDiffStore(string destinationKey, string[] keys)
        {
            return new CommandPacket("SDIFFSTORE", CommandMode.Write)
                .WriteKey(destinationKey)
                .WriteKeys(keys);
        }

        internal static CommandPacket SInter(string[] keys)
        {
            return new CommandPacket("SINTER", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKeys(keys);
        }

        internal static CommandPacket SInterCard(string[] keys, ulong limit)
        {
            return new CommandPacket("SINTERCARD", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteArg(keys.Length)
                .WriteKeys(keys)
                .WriteArg(limit > 0, "LIMIT", limit);
        }

        internal static CommandPacket SInterStore(string destinationKey, string[] keys)
        {
            return new CommandPacket("SINTERSTORE", CommandMode.Write)
                .WriteKey(destinationKey)
                .WriteKeys(keys);
        }

        internal static CommandPacket SIsMember<TMember>(string key, TMember member) where TMember : class
        {
            return new CommandPacket("SISMEMBER", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValue(member);
        }

        internal static CommandPacket SMembers(string key)
        {
            return new CommandPacket("SMEMBERS", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
        }

        internal static CommandPacket SMIsMember<TMember>(string key, params TMember[] members) where TMember : class
        {
            return new CommandPacket("SMISMEMBER", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValues(members);
        }

        internal static CommandPacket SMove<TMember>(string source, string destination, TMember member) where TMember : class
        {
            return new CommandPacket("SMOVE", CommandMode.Write)
                .WriteKey(source)
                .WriteKey(destination)
                .WriteValue(member);
        }

        internal static CommandPacket SPop(string key, ulong count)
        {
            if (count <= 0) throw new RedisException("Count cannot be 0");
            return new CommandPacket("SPOP", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(count > 1, count);
        }

        internal static CommandPacket SRandMember(string key, long count)
        {
            return new CommandPacket("SRANDMEMBER", CommandMode.Read)
                .WriteKey(key)
                .WriteArg(count != 0, count);
        }

        internal static CommandPacket SRem<TMember>(string key, params TMember[] members) where TMember : class
        {
            return new CommandPacket("SREM", CommandMode.Write)
                .WriteKey(key)
                .WriteValues(members);
        }

        internal static CommandPacket SUnion(string[] keys)
        {
            return new CommandPacket("SUNION", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKeys(keys);
        }

        internal static CommandPacket SUnionStore(string destination, string[] keys)
        {
            return new CommandPacket("SUNIONSTORE", CommandMode.Write)
                .WriteKey(destination)
                .WriteKeys(keys);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket SScan(string key, long cursor, string? pattern, ulong? count)
#else
        internal static CommandPacket SScan(string key, long cursor, string pattern, ulong? count)
#endif
        {
            return new CommandPacket("SSCAN", CommandMode.Read)
                .WriteKey(key)
                .WriteArg(cursor)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArg(!string.IsNullOrEmpty(pattern), "MATCH", pattern!)
#else
                .WriteArg(!string.IsNullOrEmpty(pattern), "MATCH", pattern)
#endif
                .WriteArg(count.HasValue, "COUNT", count ?? 0);
        }
    }
}
