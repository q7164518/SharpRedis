using SharpRedis.Models;

namespace SharpRedis.Commands
{
    internal static class ListCommands
    {
        internal static CommandPacket LIndex(string key, long index)
        {
            return new CommandPacket("LINDEX", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(index);
        }

        internal static CommandPacket LInsert<TElement, TPivot>(string key, TElement element, TPivot pivot, BeforeAfter ba)
            where TElement : class
            where TPivot : class
        {
            return new CommandPacket("LINSERT", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(ba == BeforeAfter.After, "AFTER")
                .WriteArg(ba == BeforeAfter.Before, "BEFORE")
                .WriteValue(pivot)
                .WriteValue(element);
        }

        internal static CommandPacket LLen(string key)
        {
            return new CommandPacket("LLEN", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
        }

        internal static CommandPacket LPush<TElement>(string key, params TElement[] elements) where TElement : class
        {
            return new CommandPacket("LPUSH", CommandMode.Write)
                .WriteKey(key)
                .WriteValues(elements);
        }

        internal static CommandPacket LPushX<TElement>(string key, params TElement[] elements) where TElement : class
        {
            return new CommandPacket("LPUSHX", CommandMode.Write)
                .WriteKey(key)
                .WriteValues(elements);
        }

        internal static CommandPacket RPush<TElement>(string key, params TElement[] elements) where TElement : class
        {
            return new CommandPacket("RPUSH", CommandMode.Write)
                .WriteKey(key)
                .WriteValues(elements);
        }

        internal static CommandPacket RPushX<TElement>(string key, params TElement[] elements) where TElement : class
        {
            return new CommandPacket("RPUSHX", CommandMode.Write)
                .WriteKey(key)
                .WriteValues(elements);
        }

        internal static CommandPacket LMove(string source, string destination, LeftRight whereFrom, LeftRight whereTo)
        {
            return new CommandPacket("LMOVE", CommandMode.Write)
                .WriteKey(source)
                .WriteKey(destination)
                .WriteArg(whereFrom == LeftRight.Left, "LEFT")
                .WriteArg(whereFrom == LeftRight.Right, "RIGHT")
                .WriteArg(whereTo == LeftRight.Left, "LEFT")
                .WriteArg(whereTo == LeftRight.Right, "RIGHT");
        }

        internal static CommandPacket LPop(string key, ulong count)
        {
            if (count <= 0) throw new RedisException("Count cannot be 0");
            return new CommandPacket("LPOP", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(count > 1, count);
        }

        internal static CommandPacket LPos<TElement>(string key, TElement element, long? rank, ulong? count, ulong? maxlen) where TElement : class
        {
            if (rank == 0) throw new RedisException("RANK can't be zero: use 1 to start from the first match, 2 from the second ... or use negative to start from the end of the list");

            return new CommandPacket("LPOS", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValue(element)
                .WriteArg(rank.HasValue, "RANK", rank ?? 0)
                .WriteArg(count.HasValue, "COUNT", count ?? 0)
                .WriteArg(maxlen.HasValue && maxlen.Value > 0, "MAXLEN", maxlen ?? 0);
        }

        internal static CommandPacket LRange(string key, long start, long stop)
        {
            return new CommandPacket("LRANGE", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(start)
                .WriteArg(stop);
        }

        internal static CommandPacket LRem<TElement>(string key, long count, TElement element) where TElement : class
        {
            return new CommandPacket("LREM", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(count)
                .WriteValue(element);
        }

        internal static CommandPacket LSet<TElement>(string key, long index, TElement element) where TElement : class
        {
            return new CommandPacket("LSET", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(index)
                .WriteValue(element);
        }

        internal static CommandPacket LTrim(string key, long start, long stop)
        {
            return new CommandPacket("LTRIM", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(start)
                .WriteArg(stop);
        }

        internal static CommandPacket RPop(string key, ulong count)
        {
            if (count <= 0) throw new RedisException("Count cannot be 0");
            return new CommandPacket("RPOP", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(count > 1, count);
        }

        internal static CommandPacket RPopLPush(string source, string destination)
        {
            return new CommandPacket("RPOPLPUSH", CommandMode.Write)
                .WriteKey(source)
                .WriteKey(destination);
        }

        internal static CommandPacket LMPop(string[] keys, LeftRight lr, ulong count)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("Please specify at least one key");
            return new CommandPacket("LMPOP", CommandMode.Write)
                .WriteArg(keys.Length)
                .WriteKeys(keys)
                .WriteArg(lr == LeftRight.Left, "LEFT")
                .WriteArg(lr == LeftRight.Right, "RIGHT")
                .WriteArg(count > 1, "COUNT", count);
        }
        
        internal static CommandPacket BLPop(string[] keys, string timeout)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("Please specify at least one key");
            return new CommandPacket("BLPOP", CommandMode.Write | CommandMode.WithBlock)
                .WriteKeys(keys)
                .WriteArg(timeout);
        }
        
        internal static CommandPacket BRPop(string[] keys, string timeout)
        {
            if (keys is null || keys.Length == 0) throw new RedisException("Please specify at least one key");
            return new CommandPacket("BRPOP", CommandMode.Write | CommandMode.WithBlock)
                .WriteKeys(keys)
                .WriteArg(timeout);
        }

        internal static CommandPacket BRPopLPush(string source, string destination, string timeout)
        {
            return new CommandPacket("BRPOPLPUSH", CommandMode.Write | CommandMode.WithBlock)
                .WriteKey(source)
                .WriteKey(destination)
                .WriteArg(timeout);
        }

        internal static CommandPacket BLMove(string source, string destination, LeftRight whereFrom, LeftRight whereTo, string timeout)
        {
            return new CommandPacket("BLMOVE", CommandMode.Write | CommandMode.WithBlock)
                .WriteKey(source)
                .WriteKey(destination)
                .WriteArg(whereFrom == LeftRight.Left, "LEFT")
                .WriteArg(whereFrom == LeftRight.Right, "RIGHT")
                .WriteArg(whereTo == LeftRight.Left, "LEFT")
                .WriteArg(whereTo == LeftRight.Right, "RIGHT")
                .WriteArg(timeout);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket BLMPop(string timeout, string key, string[]? keys, LeftRight lr, ulong count)
#else
        internal static CommandPacket BLMPop(string timeout, string key, string[] keys, LeftRight lr, ulong count)
#endif
        {
            return new CommandPacket("BLMPOP", CommandMode.Write | CommandMode.WithBlock)
                .WriteArg(timeout)
                .WriteArg(1 + (keys?.Length ?? 0))
                .WriteKey(key)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteKeys(keys?.Length > 0, keys!)
#else
                .WriteKeys(keys?.Length > 0, keys)
#endif
                .WriteArg(lr == LeftRight.Left, "LEFT")
                .WriteArg(lr == LeftRight.Right, "RIGHT")
                .WriteArg(count != 0, "COUNT", count);
        }
    }
}
