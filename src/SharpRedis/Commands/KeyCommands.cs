using SharpRedis.Models;

namespace SharpRedis.Commands
{
    internal static class KeyCommands
    {
        internal static CommandPacket Copy(string source, string destination, ushort? db, bool replace)
        {
            return new CommandPacket("COPY", CommandMode.Keyspace | CommandMode.Write)
                .WriteKey(source)
                .WriteKey(destination)
                .WriteArg(db.HasValue, "DB", db ?? 0)
                .WriteArg(replace, "REPLACE");
        }

        internal static CommandPacket Del(params string[] keys)
        {
            return new CommandPacket("DEL", CommandMode.Keyspace | CommandMode.Write)
                .WriteKeys(keys);
        }

        internal static CommandPacket Dump(string key)
        {
            return new CommandPacket("DUMP", CommandMode.Keyspace | CommandMode.Read)
                .WriteKey(key);
        }

        internal static CommandPacket Restore(string key, ulong ttl, bool absttl, byte[] serializedValue, bool replace, ulong? idleTime, ulong? frequency)
        {
            return new CommandPacket("RESTORE", CommandMode.Keyspace | CommandMode.Write)
                .WriteKey(key)
                .WriteArg(ttl)
                .WriteValue(serializedValue)
                .WriteArg(replace, "REPLACE")
                .WriteArg(absttl, "ABSTTL")
                .WriteArg(idleTime.HasValue, "IDLETIME", idleTime ?? 0)
                .WriteArg(frequency.HasValue, "FREQ", frequency ?? 0);
        }

        internal static CommandPacket Exists(params string[] keys)
        {
            return new CommandPacket("EXISTS", CommandMode.Keyspace | CommandMode.Read)
                .WriteKeys(keys);
        }

        internal static CommandPacket Expire(string key, ulong seconds, NxXx nxx = NxXx.None, GtLt glt = GtLt.None)
        {
            if (nxx == NxXx.Nx && glt != GtLt.None) throw new RedisException("The GT, LT and NX options are mutually exclusive.");
            return new CommandPacket("EXPIRE", CommandMode.Keyspace | CommandMode.Write)
                .WriteKey(key)
                .WriteArg(seconds)
                .WriteArg(nxx != NxXx.None, nxx == NxXx.Xx ? "XX" : "NX")
                .WriteArg(glt != GtLt.None, glt == GtLt.Gt ? "GT" : "LT");
        }

        internal static CommandPacket PExpire(string key, ulong milliseconds, NxXx nxx = NxXx.None, GtLt glt = GtLt.None)
        {
            if (nxx == NxXx.Nx && glt != GtLt.None) throw new RedisException("The GT, LT and NX options are mutually exclusive.");
            return new CommandPacket("PEXPIRE", CommandMode.Keyspace | CommandMode.Write)
                .WriteKey(key)
                .WriteArg(milliseconds)
                .WriteArg(nxx != NxXx.None, nxx == NxXx.Xx ? "XX" : "NX")
                .WriteArg(glt != GtLt.None, glt == GtLt.Gt ? "GT" : "LT");
        }

        internal static CommandPacket ExpireAt(string key, long unixTimeSeconds, NxXx nxx = NxXx.None, GtLt glt = GtLt.None)
        {
            if (nxx == NxXx.Nx && glt != GtLt.None) throw new RedisException("The GT, LT and NX options are mutually exclusive.");
            return new CommandPacket("EXPIREAT", CommandMode.Keyspace | CommandMode.Write)
                .WriteKey(key)
                .WriteArg(unixTimeSeconds)
                .WriteArg(nxx != NxXx.None, nxx == NxXx.Xx ? "XX" : "NX")
                .WriteArg(glt != GtLt.None, glt == GtLt.Gt ? "GT" : "LT");
        }

        internal static CommandPacket PExpireAt(string key, long unixTimeMilliseconds, NxXx nxx = NxXx.None, GtLt glt = GtLt.None)
        {
            if (nxx == NxXx.Nx && glt != GtLt.None) throw new RedisException("The GT, LT and NX options are mutually exclusive.");
            return new CommandPacket("PEXPIREAT", CommandMode.Keyspace | CommandMode.Write)
                .WriteKey(key)
                .WriteArg(unixTimeMilliseconds)
                .WriteArg(nxx != NxXx.None, nxx == NxXx.Xx ? "XX" : "NX")
                .WriteArg(glt != GtLt.None, glt == GtLt.Gt ? "GT" : "LT");
        }

        internal static CommandPacket ExpireTime(string key)
        {
            return new CommandPacket("EXPIRETIME", CommandMode.Keyspace | CommandMode.Read)
                .WriteKey(key);
        }

        internal static CommandPacket PExpireTime(string key)
        {
            return new CommandPacket("PEXPIRETIME", CommandMode.Keyspace | CommandMode.Read)
                .WriteKey(key);
        }

        internal static CommandPacket Keys(string pattern)
        {
            return new CommandPacket("KEYS", CommandMode.Keyspace | CommandMode.Read)
                .WriteArg(pattern);
        }

        internal static CommandPacket Move(string key, ushort db)
        {
            return new CommandPacket("MOVE", CommandMode.Keyspace | CommandMode.Write)
                .WriteKey(key)
                .WriteArg(db);
        }

        internal static CommandPacket Persist(string key)
        {
            return new CommandPacket("PERSIST", CommandMode.Keyspace | CommandMode.Write)
                .WriteKey(key);
        }

        internal static CommandPacket Ttl(string key)
        {
            return new CommandPacket("TTL", CommandMode.Keyspace | CommandMode.Read)
                .WriteKey(key);
        }

        internal static CommandPacket PTtl(string key)
        {
            return new CommandPacket("PTTL", CommandMode.Keyspace | CommandMode.Read)
                .WriteKey(key);
        }

        internal static CommandPacket RandomKey()
        {
            return new CommandPacket("RANDOMKEY", CommandMode.Keyspace | CommandMode.Read);
        }

        internal static CommandPacket Rename(string key, string newKey)
        {
            return new CommandPacket("RENAME", CommandMode.Keyspace | CommandMode.Write)
                .WriteKey(key)
                .WriteKey(newKey);
        }

        internal static CommandPacket RenameNx(string key, string newKey)
        {
            return new CommandPacket("RENAMENX", CommandMode.Keyspace | CommandMode.Write)
                .WriteKey(key)
                .WriteKey(newKey);
        }

        internal static CommandPacket Type(string key)
        {
            return new CommandPacket("TYPE", CommandMode.Keyspace | CommandMode.Read)
                .WriteKey(key);
        }

        internal static CommandPacket Unlink(params string[] keys)
        {
            return new CommandPacket("UNLINK", CommandMode.Keyspace | CommandMode.Write)
                .WriteKeys(keys);
        }

        internal static CommandPacket Touch(params string[] keys)
        {
            return new CommandPacket("TOUCH", CommandMode.Keyspace | CommandMode.Write)
                .WriteKeys(keys);
        }

        internal static CommandPacket ObjectEncoding(string key)
        {
            return new CommandPacket("OBJECT", CommandMode.Keyspace | CommandMode.Read)
                .WriteArg("ENCODING")
                .WriteKey(key);
        }

        internal static CommandPacket ObjectFreq(string key)
        {
            return new CommandPacket("OBJECT", CommandMode.Keyspace | CommandMode.Read)
                .WriteArg("FREQ")
                .WriteKey(key);
        }

        internal static CommandPacket ObjectIdleTime(string key)
        {
            return new CommandPacket("OBJECT", CommandMode.Keyspace | CommandMode.Read)
                .WriteArg("IDLETIME")
                .WriteKey(key);
        }

        internal static CommandPacket ObjectRefCount(string key)
        {
            return new CommandPacket("OBJECT", CommandMode.Keyspace | CommandMode.Read)
                .WriteArg("REFCOUNT")
                .WriteKey(key);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket Migrate(string host, ushort port, string key, ushort destinationDb, ulong timeout, bool copy,bool replace, string? password, string? username, params string[] keys)
#else
        internal static CommandPacket Migrate(string host, ushort port, string key, ushort destinationDb, ulong timeout, bool copy, bool replace, string password, string username, params string[] keys)
#endif
        {
            return new CommandPacket("MIGRATE", CommandMode.Keyspace | CommandMode.Write)
                .WriteArg(host)
                .WriteArg(port)
                .WriteArg(key)
                .WriteArg(destinationDb)
                .WriteArg(timeout)
                .WriteArg(copy, "COPY")
                .WriteArg(replace, "REPLACE")
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArg(!string.IsNullOrEmpty(password) && string.IsNullOrEmpty(username), "AUTH", password!)
                .WriteArg(!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(username), "AUTH2", username!)
                .WriteArg(!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(username), "AUTH2", password!)
#else
                .WriteArg(!string.IsNullOrEmpty(password) && string.IsNullOrEmpty(username), "AUTH", password)
                .WriteArg(!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(username), "AUTH2", username)
                .WriteArg(!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(username), "AUTH2", password)
#endif
                .WriteArg(keys?.Length > 0, "KEYS")
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteKeys(keys?.Length > 0, keys!);
#else
                .WriteKeys(keys?.Length > 0, keys);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket Scan(long cursor, string? pattern, ulong? count, string? type)
#else
        internal static CommandPacket Scan(long cursor, string pattern, ulong? count, string type)
#endif
        {
            return new CommandPacket("SCAN", CommandMode.Keyspace | CommandMode.Read)
                .WriteArg(cursor)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArg(!string.IsNullOrEmpty(pattern), "MATCH", pattern!)
#else
                .WriteArg(!string.IsNullOrEmpty(pattern), "MATCH", pattern)
#endif
                .WriteArg(count.HasValue, "COUNT", count ?? 0)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArg(!string.IsNullOrEmpty(type), "TYPE", type!);
#else
                .WriteArg(!string.IsNullOrEmpty(type), "TYPE", type);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket Sort(string key, string? byPattern, long? offset, long? count, string[]? getPatterns, OrderType orderType, bool alpha, string? storeKey)
#else
        internal static CommandPacket Sort(string key, string byPattern, long? offset, long? count, string[] getPatterns, OrderType orderType, bool alpha, string storeKey)
#endif
        {
            var command = new CommandPacket("SORT", CommandMode.Sentinel | CommandMode.Write)
                .WriteKey(key)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArg(!string.IsNullOrEmpty(byPattern), "BY", byPattern!)
#else
                .WriteArg(!string.IsNullOrEmpty(byPattern), "BY", byPattern)
#endif
                .WriteArg(offset.HasValue || count.HasValue, "LIMIT", offset ?? 0)
                .WriteArg(offset.HasValue || count.HasValue, count ?? 0);
            if (getPatterns?.Length > 0)
            {
                foreach (var get in getPatterns)
                {
                    command.WriteArg("GET", get);
                }
            }
            command.WriteArg(orderType == OrderType.Asc, "ASC")
                .WriteArg(orderType == OrderType.Desc, "DESC")
                .WriteArg(alpha, "ALPHA")
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArg(!string.IsNullOrEmpty(storeKey), "STORE", storeKey!);
#else
                .WriteArg(!string.IsNullOrEmpty(storeKey), "STORE", storeKey);
#endif
            return command;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket SortRo(string key, string? byPattern, long? offset, long? count, string[]? getPatterns, OrderType orderType, bool alpha)
#else
        internal static CommandPacket SortRo(string key, string byPattern, long? offset, long? count, string[] getPatterns, OrderType orderType, bool alpha)
#endif
        {
            var command = new CommandPacket("SORT_RO", CommandMode.Sentinel | CommandMode.Read)
                .WriteKey(key)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArg(!string.IsNullOrEmpty(byPattern), "BY", byPattern!)
#else
                .WriteArg(!string.IsNullOrEmpty(byPattern), "BY", byPattern)
#endif
                .WriteArg(offset.HasValue || count.HasValue, "LIMIT", offset ?? 0)
                .WriteArg(offset.HasValue || count.HasValue, count ?? 0);
            if (getPatterns?.Length > 0)
            {
                foreach (var get in getPatterns)
                {
                    command.WriteArg("GET", get);
                }
            }
            command.WriteArg(orderType == OrderType.Asc, "ASC")
                .WriteArg(orderType == OrderType.Desc, "DESC")
                .WriteArg(alpha, "ALPHA");
            return command;
        }
    }
}
