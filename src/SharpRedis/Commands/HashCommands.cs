using SharpRedis.Models;
using System;
using System.Collections.Generic;

namespace SharpRedis.Commands
{
    internal static class HashCommands
    {
        internal static CommandPacket HSet(string key)
        {
            return new CommandPacket("HSET", CommandMode.Write).WriteKey(key);
        }

        internal static CommandPacket HSet(string key, params string[] fieldValues)
        {
            if (fieldValues is null || fieldValues.Length <= 0) throw new InvalidOperationException("Make sure you have at least one field-value");
            if (fieldValues.Length % 2 != 0) throw new InvalidOperationException("Make sure the array is the correct field-value pair");
            var command = HashCommands.HSet(key);
            for (uint i = 0; i < fieldValues.Length; i += 2)
            {
                command.WriteArg(fieldValues[i])
                    .WriteValue(fieldValues[i + 1]);
            }
            return command;
        }

        internal static CommandPacket HSet<TValue>(string key, Dictionary<string, TValue> fieldValue) where TValue : class
        {
            if (fieldValue is null || fieldValue.Count <= 0) throw new InvalidOperationException("Ensure that there is at least one set of key-value pairs");
            var command = HashCommands.HSet(key);
            foreach (var item in fieldValue)
            {
                command.WriteArg(item.Key)
                    .WriteValue(item.Value);
            }
            return command;
        }

        internal static CommandPacket HSet<TValue>(string key, KeyValuePair<string, TValue>[] fieldValues) where TValue : class
        {
            if (fieldValues is null || fieldValues.Length <= 0) throw new InvalidOperationException("Ensure that there is at least one set of key-value pairs");
            var command = HashCommands.HSet(key);
            foreach (var item in fieldValues)
            {
                command.WriteArg(item.Key)
                    .WriteValue(item.Value);
            }
            return command;
        }

        internal static CommandPacket HMSet(string key)
        {
            return new CommandPacket("HMSET", CommandMode.Write).WriteKey(key);
        }

        internal static CommandPacket HMSet(string key, params string[] fieldValues)
        {
            if (fieldValues is null || fieldValues.Length <= 0) throw new InvalidOperationException("Make sure you have at least one field-value");
            if (fieldValues.Length % 2 != 0) throw new InvalidOperationException("Make sure the array is the correct field-value pair");
            var command = HashCommands.HMSet(key);
            for (uint i = 0; i < fieldValues.Length; i += 2)
            {
                command.WriteArg(fieldValues[i])
                    .WriteValue(fieldValues[i + 1]);
            }
            return command;
        }

        internal static CommandPacket HMSet<TValue>(string key, Dictionary<string, TValue> fieldValue) where TValue : class
        {
            if (fieldValue is null || fieldValue.Count <= 0) throw new InvalidOperationException("Ensure that there is at least one set of key-value pairs");
            var command = HashCommands.HMSet(key);
            foreach (var item in fieldValue)
            {
                command.WriteArg(item.Key)
                    .WriteValue(item.Value);
            }
            return command;
        }

        internal static CommandPacket HMSet<TValue>(string key, KeyValuePair<string, TValue>[] fieldValues) where TValue : class
        {
            if (fieldValues is null || fieldValues.Length <= 0) throw new InvalidOperationException("Ensure that there is at least one set of key-value pairs");
            var command = HashCommands.HMSet(key);
            foreach (var item in fieldValues)
            {
                command.WriteArg(item.Key)
                    .WriteValue(item.Value);
            }
            return command;
        }

        internal static CommandPacket HGet(string key, string field)
        {
            return new CommandPacket("HGET", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(field);
        }

        internal static CommandPacket HMGet(string key, params string[] fields)
        {
            if (fields is null || fields.Length <= 0) throw new InvalidOperationException("Make sure there is at least one field");
            return new CommandPacket("HMGET", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArgs(fields);
        }

        internal static CommandPacket HDel(string key, params string[] fields)
        {
            if (fields is null || fields.Length <= 0) throw new InvalidOperationException("Make sure there is at least one field");
            return new CommandPacket("HDEL", CommandMode.Write)
                .WriteKey(key)
                .WriteArgs(fields);
        }

        internal static CommandPacket HExists(string key, string field)
        {
            return new CommandPacket("HEXISTS", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(field);
        }

        internal static CommandPacket HGetAll(string key)
        {
            return new CommandPacket("HGETALL", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
        }

        internal static CommandPacket HIncrBy<TNumber>(string key, string field, TNumber increment)
            where TNumber : struct
        {
            return new CommandPacket("HINCRBY", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(field)
                .WriteArg(increment);
        }

        internal static CommandPacket HIncrByFloat<TNumber>(string key, string field, TNumber increment)
            where TNumber : struct
        {
            return new CommandPacket("HINCRBYFLOAT", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(field)
                .WriteArg(increment);
        }

        internal static CommandPacket HKeys(string key)
        {
            return new CommandPacket("HKEYS", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
        }

        internal static CommandPacket HLen(string key)
        {
            return new CommandPacket("HLEN", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
        }

        internal static CommandPacket HRandField(string key, long count, bool withValues)
        {
            return new CommandPacket("HRANDFIELD", CommandMode.Read)
                .WriteKey(key)
                .WriteArg(count != 0, count)
                .WriteArg(withValues, "WITHVALUES");
        }

        internal static CommandPacket HSetNx<TValue>(string key, string field, TValue value) where TValue : class
        {
            return new CommandPacket("HSETNX", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(field)
                .WriteValue(value);
        }

        internal static CommandPacket HStrLen(string key, string field)
        {
            return new CommandPacket("HSTRLEN", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(field);
        }

        internal static CommandPacket HVals(string key)
        {
            return new CommandPacket("HVALS", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
        }

        internal static CommandPacket HExpire(string key, ulong seconds, NxXx nxx, GtLt glt, params string[] fields)
        {
            if (fields is null || fields.Length == 0) throw new InvalidOperationException("Make sure there is at least one field");
            if (nxx != NxXx.None && glt != GtLt.None) throw new RedisException("The NX, XX, GT, and LT options are mutually exclusive");
            return new CommandPacket("HEXPIRE", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(seconds)
                .WriteArg(nxx == NxXx.Nx, "NX")
                .WriteArg(nxx == NxXx.Xx, "XX")
                .WriteArg(glt == GtLt.Lt, "LT")
                .WriteArg(glt == GtLt.Gt, "GT")
                .WriteArg("FIELDS")
                .WriteArg(fields.Length)
                .WriteArgs(fields);
        }

        internal static CommandPacket HPExpire(string key, ulong milliseconds, NxXx nxx, GtLt glt, params string[] fields)
        {
            if (fields is null || fields.Length == 0) throw new InvalidOperationException("Make sure there is at least one field");
            if (nxx != NxXx.None && glt != GtLt.None) throw new RedisException("The NX, XX, GT, and LT options are mutually exclusive");
            return new CommandPacket("HPEXPIRE", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(milliseconds)
                .WriteArg(nxx == NxXx.Nx, "NX")
                .WriteArg(nxx == NxXx.Xx, "XX")
                .WriteArg(glt == GtLt.Lt, "LT")
                .WriteArg(glt == GtLt.Gt, "GT")
                .WriteArg("FIELDS")
                .WriteArg(fields.Length)
                .WriteArgs(fields);
        }

        internal static CommandPacket HPExpireAt(string key, ulong timeout, NxXx nxx, GtLt glt, params string[] fields)
        {
            if (fields is null || fields.Length == 0) throw new InvalidOperationException("Make sure there is at least one field");
            if (nxx != NxXx.None && glt != GtLt.None) throw new RedisException("The NX, XX, GT, and LT options are mutually exclusive");
            return new CommandPacket("HPEXPIREAT", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(timeout)
                .WriteArg(nxx == NxXx.Nx, "NX")
                .WriteArg(nxx == NxXx.Xx, "XX")
                .WriteArg(glt == GtLt.Lt, "LT")
                .WriteArg(glt == GtLt.Gt, "GT")
                .WriteArg("FIELDS")
                .WriteArg(fields.Length)
                .WriteArgs(fields);
        }

        internal static CommandPacket HExpireAt(string key, ulong timeout, NxXx nxx, GtLt glt, params string[] fields)
        {
            if (fields is null || fields.Length == 0) throw new InvalidOperationException("Make sure there is at least one field");
            if (nxx != NxXx.None && glt != GtLt.None) throw new RedisException("The NX, XX, GT, and LT options are mutually exclusive");
            return new CommandPacket("HEXPIREAT", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(timeout)
                .WriteArg(nxx == NxXx.Nx, "NX")
                .WriteArg(nxx == NxXx.Xx, "XX")
                .WriteArg(glt == GtLt.Lt, "LT")
                .WriteArg(glt == GtLt.Gt, "GT")
                .WriteArg("FIELDS")
                .WriteArg(fields.Length)
                .WriteArgs(fields);
        }

        internal static CommandPacket HExpireTime(string key, params string[] fields)
        {
            return new CommandPacket("HEXPIRETIME", CommandMode.Read)
                .WriteKey(key)
                .WriteArg("FIELDS")
                .WriteArg(fields.Length)
                .WriteArgs(fields);
        }

        internal static CommandPacket HPExpireTime(string key, params string[] fields)
        {
            return new CommandPacket("HPEXPIRETIME", CommandMode.Read)
                .WriteKey(key)
                .WriteArg("FIELDS")
                .WriteArg(fields.Length)
                .WriteArgs(fields);
        }

        internal static CommandPacket HPersist(string key, params string[] fields)
        {
            return new CommandPacket("HPERSIST", CommandMode.Write)
                .WriteKey(key)
                .WriteArg("FIELDS")
                .WriteArg(fields.Length)
                .WriteArgs(fields);
        }

        internal static CommandPacket HTtl(string key, params string[] fields)
        {
            return new CommandPacket("HTTL", CommandMode.Read)
                .WriteKey(key)
                .WriteArg("FIELDS")
                .WriteArg(fields.Length)
                .WriteArgs(fields);
        }

        internal static CommandPacket HPTtl(string key, params string[] fields)
        {
            return new CommandPacket("HPTTL", CommandMode.Read)
                .WriteKey(key)
                .WriteArg("FIELDS")
                .WriteArg(fields.Length)
                .WriteArgs(fields);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket HScan(string key, long cursor, string? pattern, ulong? count, bool novalues)
#else
        internal static CommandPacket HScan(string key, long cursor, string pattern, ulong? count, bool novalues)
#endif
        {
            return new CommandPacket("HSCAN", CommandMode.Read)
                .WriteKey(key)
                .WriteArg(cursor)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArg(!string.IsNullOrEmpty(pattern), "MATCH", pattern!)
#else
                .WriteArg(!string.IsNullOrEmpty(pattern), "MATCH", pattern)
#endif
                .WriteArg(count > 0, "COUNT", count ?? 0)
                .WriteArg(novalues, "NOVALUES");
        }
    }
}
