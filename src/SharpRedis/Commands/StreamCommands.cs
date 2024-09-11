using SharpRedis.Extensions;
using SharpRedis.Models;
using System;
using System.Collections.Generic;

namespace SharpRedis.Commands
{
    internal static class StreamCommands
    {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static CommandPacket XAdd(string key, bool nomkStream, StreamTrimOptions? trimOptionsArg, DateTimeOffset? milliseconds, ulong? sequenceNumber)
#else
        private static CommandPacket XAdd(string key, bool nomkStream, StreamTrimOptions trimOptionsArg, DateTimeOffset? milliseconds, ulong? sequenceNumber)
#endif
        {
            var trimOptions = trimOptionsArg is null ? StreamTrimOptions.Empty : trimOptionsArg;
            if (trimOptions.TrimStrategy is StreamTrimStrategy.Exact && trimOptions.Limit > 0)
            {
                throw new RedisException("LIMIT cannot be used without the special ~ option");
            }

            string id;
            if (!milliseconds.HasValue)
            {
                id = "*";
            }
            else
            {
                if (sequenceNumber.HasValue)
                {
                    id = $"{Extend.GetUnixTimeMilliseconds(milliseconds.Value)}-{sequenceNumber.Value}";
                }
                else
                {
                    id = $"{Extend.GetUnixTimeMilliseconds(milliseconds.Value)}-*";
                }
            }

            return new CommandPacket("XADD", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(nomkStream, "NOMKSTREAM")
                .WriteArg(trimOptions.TrimMode == StreamTrimMode.MaxLen, "MAXLEN")
                .WriteArg(trimOptions.TrimMode == StreamTrimMode.MinId, "MINID")
                .WriteArg(trimOptions.TrimMode != StreamTrimMode.None && trimOptions.TrimStrategy == StreamTrimStrategy.Exact, "=")
                .WriteArg(trimOptions.TrimMode != StreamTrimMode.None && trimOptions.TrimStrategy == StreamTrimStrategy.Approx, "~")
                .WriteArg(trimOptions.TrimMode != StreamTrimMode.None && !string.IsNullOrEmpty(trimOptions.Threshold), trimOptions.Threshold)
                .WriteArg(trimOptions.TrimMode != StreamTrimMode.None && trimOptions.TrimStrategy == StreamTrimStrategy.Approx && trimOptions.Limit > 0, "LIMIT", trimOptions.Limit)
                .WriteArg(id);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static CommandPacket XAdd(string key, bool nomkStream, StreamTrimOptions? trimOptionsArg, ulong? milliseconds, ulong? sequenceNumber)
#else
        private static CommandPacket XAdd(string key, bool nomkStream, StreamTrimOptions trimOptionsArg, ulong? milliseconds, ulong? sequenceNumber)
#endif
        {
            var trimOptions = trimOptionsArg is null ? StreamTrimOptions.Empty : trimOptionsArg;
            if (trimOptions.TrimStrategy is StreamTrimStrategy.Exact && trimOptions.Limit > 0)
            {
                throw new RedisException("LIMIT cannot be used without the special ~ option");
            }

            string id;
            if (!milliseconds.HasValue)
            {
                id = "*";
            }
            else
            {
                if (sequenceNumber.HasValue)
                {
                    id = $"{milliseconds.Value}-{sequenceNumber.Value}";
                }
                else
                {
                    id = $"{milliseconds.Value}-*";
                }
            }

            return new CommandPacket("XADD", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(nomkStream, "NOMKSTREAM")
                .WriteArg(trimOptions.TrimMode == StreamTrimMode.MaxLen, "MAXLEN")
                .WriteArg(trimOptions.TrimMode == StreamTrimMode.MinId, "MINID")
                .WriteArg(trimOptions.TrimMode != StreamTrimMode.None && trimOptions.TrimStrategy == StreamTrimStrategy.Exact, "=")
                .WriteArg(trimOptions.TrimMode != StreamTrimMode.None && trimOptions.TrimStrategy == StreamTrimStrategy.Approx, "~")
                .WriteArg(trimOptions.TrimMode != StreamTrimMode.None && !string.IsNullOrEmpty(trimOptions.Threshold), trimOptions.Threshold)
                .WriteArg(trimOptions.TrimMode != StreamTrimMode.None && trimOptions.TrimStrategy == StreamTrimStrategy.Approx && trimOptions.Limit > 0, "LIMIT", trimOptions.Limit)
                .WriteArg(id);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket XAdd<TValue>(string key, bool nomkStream, StreamTrimOptions? trimOptions, DateTimeOffset? milliseconds, ulong? sequenceNumber, params KeyValuePair<string, TValue>[] fvArray)
#else
        internal static CommandPacket XAdd<TValue>(string key, bool nomkStream, StreamTrimOptions trimOptions, DateTimeOffset? milliseconds, ulong? sequenceNumber, params KeyValuePair<string, TValue>[] fvArray)
#endif
            where TValue : class
        {
            if (fvArray == null || fvArray.Length is 0) throw new ArgumentNullException(nameof(fvArray));
            var command = StreamCommands.XAdd(key, nomkStream, trimOptions, milliseconds, sequenceNumber);
            for (uint i = 0; i < fvArray.Length; i++)
            {
                command
                    .WriteValue(fvArray[i].Key)
                    .WriteValue(fvArray[i].Value);
            }
            return command;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket XAdd<TValue>(string key, bool nomkStream, StreamTrimOptions? trimOptions, DateTimeOffset? milliseconds, ulong? sequenceNumber, string field, TValue value)
#else
        internal static CommandPacket XAdd<TValue>(string key, bool nomkStream, StreamTrimOptions trimOptions, DateTimeOffset? milliseconds, ulong? sequenceNumber, string field, TValue value)
#endif
            where TValue : class
        {
            var command = StreamCommands.XAdd(key, nomkStream, trimOptions, milliseconds, sequenceNumber);
            return command
                .WriteValue(field)
                .WriteValue(value);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket XAdd<TValue>(string key, bool nomkStream, StreamTrimOptions? trimOptions, ulong? milliseconds, ulong? sequenceNumber, params KeyValuePair<string, TValue>[] fvArray)
#else
        internal static CommandPacket XAdd<TValue>(string key, bool nomkStream, StreamTrimOptions trimOptions, ulong? milliseconds, ulong? sequenceNumber, params KeyValuePair<string, TValue>[] fvArray)
#endif
            where TValue : class
        {
            if (fvArray == null || fvArray.Length is 0) throw new ArgumentNullException(nameof(fvArray));
            var command = StreamCommands.XAdd(key, nomkStream, trimOptions, milliseconds, sequenceNumber);
            for (uint i = 0; i < fvArray.Length; i++)
            {
                command
                    .WriteValue(fvArray[i].Key)
                    .WriteValue(fvArray[i].Value);
            }
            return command;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket XAdd<TValue>(string key, bool nomkStream, StreamTrimOptions? trimOptions, ulong? milliseconds, ulong? sequenceNumber, string field, TValue value)
#else
        internal static CommandPacket XAdd<TValue>(string key, bool nomkStream, StreamTrimOptions trimOptions, ulong? milliseconds, ulong? sequenceNumber, string field, TValue value)
#endif
            where TValue : class
        {
            var command = StreamCommands.XAdd(key, nomkStream, trimOptions, milliseconds, sequenceNumber);
            return command
                .WriteValue(field)
                .WriteValue(value);
        }

        internal static CommandPacket XLen(string key)
        {
            return new CommandPacket("XLEN", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
        }

        internal static CommandPacket XDel(string key, params string[] ids)
        {
            return new CommandPacket("XDEL", CommandMode.Write)
                .WriteKey(key)
                .WriteValues(ids);
        }

        internal static CommandPacket XRange(string key, string start, string end, ulong count = 0)
        {
            return new CommandPacket("XRANGE", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(start)
                .WriteArg(end)
                .WriteArg(count > 0, "COUNT", count);
        }

        internal static CommandPacket XRevRange(string key, string end, string start, ulong count = 0)
        {
            return new CommandPacket("XREVRANGE", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(end)
                .WriteArg(start)
                .WriteArg(count > 0, "COUNT", count);
        }

        private static CommandPacket XRead(ulong count, ulong? milliseconds)
        {
            return new CommandPacket("XREAD", CommandMode.Read | CommandMode.WithBlock)
                .WriteArg(count > 0, "COUNT", count)
                .WriteArg(milliseconds.HasValue, "BLOCK", milliseconds ?? 0)
                .WriteArg("STREAMS");
        }

        internal static CommandPacket XRead(ulong count, ulong? milliseconds, string key, string id)
        {
            var command = StreamCommands.XRead(count, milliseconds);
            return command
                .WriteKey(key)
                .WriteArg(id);
        }

        internal static CommandPacket XRead(ulong count, ulong? milliseconds, string[] keys, string[] ids)
        {
            if (keys is null || keys.Length is 0) throw new ArgumentNullException(nameof(keys));
            if (ids is null || ids.Length is 0) throw new ArgumentNullException(nameof(ids));
            if (keys.Length != ids.Length) throw new RedisException("The number of keys and ids must be the same");

            var command = StreamCommands.XRead(count, milliseconds);
            return command
                .WriteKeys(keys)
                .WriteArgs(ids);
        }

        internal static CommandPacket XTrim(string key, StreamTrimMode trimMode, StreamTrimStrategy trimStrategy, string threshold, ulong count)
        {
            if (trimMode is StreamTrimMode.None) throw new RedisException("Trim mode can only be MAXLEN or MINID");
            if (trimStrategy is StreamTrimStrategy.Exact && count > 0)
            {
                throw new RedisException("LIMIT cannot be used without the special ~ option");
            }

            return new CommandPacket("XTRIM", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(trimMode == StreamTrimMode.MaxLen, "MAXLEN")
                .WriteArg(trimMode == StreamTrimMode.MinId, "MINID")
                .WriteArg(trimStrategy == StreamTrimStrategy.Exact, "=")
                .WriteArg(trimStrategy == StreamTrimStrategy.Approx, "~")
                .WriteArg(!string.IsNullOrEmpty(threshold), threshold)
                .WriteArg(count > 0, "LIMIT", count);
        }

        internal static CommandPacket XGroupCreate(string key, string group, string id, bool mkStream, ulong? entriesRead)
        {
            return new CommandPacket("XGROUP", CommandMode.Write)
                .WriteArg("CREATE")
                .WriteKey(key)
                .WriteValue(group)
                .WriteValue(id)
                .WriteArg(mkStream, "MKSTREAM")
                .WriteArg(entriesRead.HasValue, "ENTRIESREAD", entriesRead ?? 0);
        }

        private static CommandPacket XReadGroup(string group, string consumer, ulong count, ulong? milliseconds, bool noAck)
        {
            return new CommandPacket("XREADGROUP", CommandMode.Write | CommandMode.WithBlock)
                .WriteArg("GROUP")
                .WriteValue(group)
                .WriteValue(consumer)
                .WriteArg(count > 0, "COUNT", count)
                .WriteArg(milliseconds.HasValue, "BLOCK", milliseconds ?? 0)
                .WriteArg(noAck, "NOACK")
                .WriteArg("STREAMS");
        }

        internal static CommandPacket XReadGroup(string group, string consumer, ulong count, ulong? milliseconds, bool noAck, string key, string id)
        {
            var command = StreamCommands.XReadGroup(group, consumer, count, milliseconds, noAck);
            return command
                .WriteKey(key)
                .WriteValue(id);
        }

        internal static CommandPacket XReadGroup(string group, string consumer, ulong count, ulong? milliseconds, bool noAck, string[] keys, string[] ids)
        {
            if (keys is null || keys.Length is 0) throw new ArgumentNullException(nameof(keys));
            if (ids is null || ids.Length is 0) throw new ArgumentNullException(nameof(ids));
            if (keys.Length != ids.Length) throw new RedisException("The number of keys and ids must be the same");

            var command = StreamCommands.XReadGroup(group, consumer, count, milliseconds, noAck);
            return command
                .WriteKeys(keys)
                .WriteValues(ids);
        }

        private static CommandPacket XAck(string key, string group)
        {
            return new CommandPacket("XACK", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(group);
        }

        internal static CommandPacket XAck(string key, string group, string id)
        {
            return StreamCommands.XAck(key, group)
                .WriteValue(id);
        }

        internal static CommandPacket XAck(string key, string group, string[] ids)
        {
            return StreamCommands.XAck(key, group)
                .WriteValues(ids);
        }

        internal static CommandPacket XAutoClaim(string key, string group, string consumer, ulong minIdleTime, string start, ulong count, bool justID)
        {
            return new CommandPacket("XAUTOCLAIM", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(group)
                .WriteValue(consumer)
                .WriteArg(minIdleTime)
                .WriteValue(start)
                .WriteArg(count > 0, "COUNT", count)
                .WriteArg(justID, "JUSTID");
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket XClaim(string key, string group, string consumer, ulong minIdleTime, string id, ulong? idle, ulong? time, ulong? retryCount, bool force, bool justid, string? lastid)
#else
        internal static CommandPacket XClaim(string key, string group, string consumer, ulong minIdleTime, string id, ulong? idle, ulong? time, ulong? retryCount, bool force, bool justid, string lastid)
#endif
        {
            return new CommandPacket("XCLAIM", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(group)
                .WriteValue(consumer)
                .WriteArg(minIdleTime)
                .WriteValue(id)
                .WriteArg(idle.HasValue, "IDLE", idle ?? 0)
                .WriteArg(time.HasValue, "TIME", time ?? 0)
                .WriteArg(retryCount.HasValue, "RETRYCOUNT", retryCount ?? 0)
                .WriteArg(force, "FORCE")
                .WriteArg(justid, "JUSTID")
                .WriteValue(lastid != null, "LASTID", lastid ?? string.Empty);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket XClaim(string key, string group, string consumer, ulong minIdleTime, string[] ids, ulong? idle, ulong? time, ulong? retryCount, bool force, bool justid, string? lastid)
#else
        internal static CommandPacket XClaim(string key, string group, string consumer, ulong minIdleTime, string[] ids, ulong? idle, ulong? time, ulong? retryCount, bool force, bool justid, string lastid)
#endif
        {
            return new CommandPacket("XCLAIM", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(group)
                .WriteValue(consumer)
                .WriteArg(minIdleTime)
                .WriteValues(ids)
                .WriteArg(idle.HasValue, "IDLE", idle ?? 0)
                .WriteArg(time.HasValue, "TIME", time ?? 0)
                .WriteArg(retryCount.HasValue, "RETRYCOUNT", retryCount ?? 0)
                .WriteArg(force, "FORCE")
                .WriteArg(justid, "JUSTID")
                .WriteValue(lastid != null, "LASTID", lastid ?? string.Empty);
        }

        internal static CommandPacket XGroupCreateConsumer(string key, string group, string consumer)
        {
            return new CommandPacket("XGROUP", CommandMode.Write)
                .WriteArg("CREATECONSUMER")
                .WriteKey(key)
                .WriteValue(group)
                .WriteValue(consumer);
        }

        internal static CommandPacket XGroupDelConsumer(string key, string group, string consumer)
        {
            return new CommandPacket("XGROUP", CommandMode.Write)
                .WriteArg("DELCONSUMER")
                .WriteKey(key)
                .WriteValue(group)
                .WriteValue(consumer);
        }

        internal static CommandPacket XGroupDestroy(string key, string group)
        {
            return new CommandPacket("XGROUP", CommandMode.Write)
                .WriteArg("DESTROY")
                .WriteKey(key)
                .WriteValue(group);
        }

        internal static CommandPacket XGroupSetID(string key, string group, string id, ulong? entriesRead)
        {
            return new CommandPacket("XGROUP", CommandMode.Write)
                .WriteArg("SETID")
                .WriteKey(key)
                .WriteValue(group)
                .WriteValue(id)
                .WriteArg(entriesRead.HasValue, "ENTRIESREAD", entriesRead ?? 0);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket XSetID(string key, string lastid, ulong? entriesAdded, string? maxDeletedId)
#else
        internal static CommandPacket XSetID(string key, string lastid, ulong? entriesAdded, string maxDeletedId)
#endif
        {
            return new CommandPacket("XSETID", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(lastid)
                .WriteArg(entriesAdded.HasValue, "ENTRIESADDED", entriesAdded ?? 0)
                .WriteArg(maxDeletedId != null, "MAXDELETEDID", maxDeletedId ?? string.Empty);
        }

        internal static CommandPacket XInfoConsumers(string key, string group)
        {
            return new CommandPacket("XINFO", CommandMode.Read)
                .WriteArg("CONSUMERS")
                .WriteKey(key)
                .WriteValue(group);
        }

        internal static CommandPacket XInfoGroups(string key)
        {
            return new CommandPacket("XINFO", CommandMode.Read)
                .WriteArg("GROUPS")
                .WriteKey(key);
        }

        internal static CommandPacket XInfoStream(string key, bool full, ulong count)
        {
            return new CommandPacket("XINFO", CommandMode.Read)
                .WriteArg("STREAM")
                .WriteKey(key)
                .WriteArg(full, "FULL")
                .WriteArg(count > 0, "COUNT", count);
        }
    }
}
