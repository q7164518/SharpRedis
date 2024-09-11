#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
using SharpRedis.Extensions;
using SharpRedis.Models;
using System;
using System.Collections.Generic;

namespace SharpRedis.Commands
{
    internal static class StringCommands
    {
        internal static CommandPacket Set<TValue>(string key,
            TValue value,
            NxXx nxx = NxXx.None,
            bool get = false,
            ulong? milliseconds = null,
            DateTimeOffset? expireTime = null,
            bool keepTtl = false) where TValue : class
        {
            return new CommandPacket("SET", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(value)
                .WriteArg(nxx != NxXx.None, nxx == NxXx.Xx ? "XX" : "NX")
                .WriteArg(get, "GET")
                .WriteArg(milliseconds.HasValue, "PX", milliseconds ?? 0L)
                .WriteArg(expireTime.HasValue, "PXAT", Extend.GetUnixTimeMilliseconds(expireTime ?? default))
                .WriteArg(keepTtl, "KEEPTTL");
        }

        internal static CommandPacket PSetEx<TValue>(string key, ulong milliseconds, TValue value) where TValue : class
        {
            return new CommandPacket("PSETEX", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(milliseconds)
                .WriteValue(value);
        }

        internal static CommandPacket SetEx<TValue>(string key, ulong seconds, TValue value) where TValue : class
        {
            return new CommandPacket("SETEX", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(seconds)
                .WriteValue(value);
        }

        internal static CommandPacket SetNx<TValue>(string key, TValue value) where TValue : class
        {
            return new CommandPacket("SETNX", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(value);
        }

        internal static CommandPacket Get(string key)
        {
            return new CommandPacket("GET", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
        }

        internal static CommandPacket Append<TValue>(string key, TValue appendValue) where TValue : class
        {
            return new CommandPacket("APPEND", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(appendValue);
        }

        internal static CommandPacket DecrBy<TNumber>(string key, TNumber decrement)
            where TNumber : struct
        {
            return new CommandPacket("DECRBY", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(decrement);
        }

        internal static CommandPacket IncrBy<TNumber>(string key, TNumber decrement)
            where TNumber : struct
        {
            return new CommandPacket("INCRBY", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(decrement);
        }

        internal static CommandPacket GetDel(string key)
        {
            return new CommandPacket("GETDEL", CommandMode.Write)
                .WriteKey(key);
        }

        internal static CommandPacket GetEx(string key, ulong? milliseconds = null, DateTimeOffset? expireTime = null, bool persist = false)
        {
            if (persist && (milliseconds.HasValue || expireTime.HasValue)) throw new RedisException("The PERSIST parameter cannot be set with the expiration time");
            return new CommandPacket("GETEX", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(milliseconds.HasValue, "PX", milliseconds ?? 0L)
                .WriteArg(expireTime.HasValue, "PXAT", Extend.GetUnixTimeMilliseconds(expireTime ?? default))
                .WriteArg(persist, "PERSIST");
        }

        internal static CommandPacket GetRange(string key, long start, long end)
        {
            return new CommandPacket("GETRANGE", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(start)
                .WriteArg(end);
        }

        internal static CommandPacket GetSet<TValue>(string key, TValue value) where TValue : class
        {
            return new CommandPacket("GETSET", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(value);
        }

        internal static CommandPacket IncrByFloat<TNumber>(string key, TNumber increment)
            where TNumber : struct
        {
            return new CommandPacket("INCRBYFLOAT", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(increment);
        }

        internal static CommandPacket MGet(params string[] keys)
        {
            if (keys == null || keys.Length == 0) throw new InvalidOperationException("Make sure you have at least one key");
            return new CommandPacket("MGET", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKeys(keys);
        }

        internal static CommandPacket MSet() => new CommandPacket("MSET", CommandMode.Write);

        internal static CommandPacket MSet<TValue>(params KeyValuePair<string, TValue>[] kvs) where TValue : class
        {
            if (kvs == null || kvs.Length == 0) throw new InvalidOperationException("Make sure you have at least one key-value");
            var command = StringCommands.MSet();
            for (uint i = 0; i < kvs.Length; i++)
            {
                command.WriteKey(kvs[i].Key)
                    .WriteValue(kvs[i].Value);
            }
            return command;
        }

        internal static CommandPacket MSetNx() => new CommandPacket("MSETNX", CommandMode.Write);

        internal static CommandPacket MSetNx<TValue>(params KeyValuePair<string, TValue>[] kvs) where TValue : class
        {
            if (kvs == null || kvs.Length == 0) throw new InvalidOperationException("Make sure you have at least one key-value");
            var command = StringCommands.MSetNx();
            for (uint i = 0; i < kvs.Length; i++)
            {
                command.WriteKey(kvs[i].Key)
                    .WriteValue(kvs[i].Value);
            }
            return command;
        }

        internal static CommandPacket SetRange<TValue>(string key, long offset, TValue value) where TValue : class
        {
            return new CommandPacket("SETRANGE", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(offset)
                .WriteValue(value);
        }

        internal static CommandPacket StrLen(string key)
        {
            return new CommandPacket("STRLEN", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
        }

        internal static CommandPacket SubStr(string key, long start, long end)
        {
            return new CommandPacket("SUBSTR", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(start)
                .WriteArg(end);
        }

        internal static CommandPacket Lcs(string key1, string key2, bool len = false, bool idx = false, uint minMatchLen = 0, bool withMatchLen = false)
        {
            var command = new CommandPacket("LCS", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key1)
                .WriteKey(key2)
                .WriteArg(len, "LEN")
                .WriteArg(idx, "IDX")
                .WriteArg(minMatchLen > 0, "MINMATCHLEN", minMatchLen)
                .WriteArg(withMatchLen, "WITHMATCHLEN");
            return command;
        }
    }
}
