using SharpRedis.Models;
using System.Collections.Generic;

namespace SharpRedis.Commands
{
    internal static class SortedSetCommands
    {
        private static CommandPacket ZAdd(string key, NxXx nxx, GtLt gl, bool ch, bool incr)
        {
            if (nxx == NxXx.Nx && gl != GtLt.None) throw new RedisException("The GT, LT and NX options are mutually exclusive");
            return new CommandPacket("ZADD", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(nxx != NxXx.None, nxx == NxXx.Xx ? "XX" : "NX")
                .WriteArg(gl != GtLt.None, gl == GtLt.Gt ? "GT" : "LT")
                .WriteArg(ch, "CH")
                .WriteArg(incr, "INCR");
        }

        internal static CommandPacket ZAdd<TMember, TScore>(string key, TMember member, TScore score, NxXx nxx = NxXx.None, GtLt gl = GtLt.None, bool ch = false, bool incr = false)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
            where TMember : class
        {
            var scoreString = score.ToString() ?? throw new RedisException("Score is null!");
            var command = SortedSetCommands.ZAdd(key, nxx, gl, ch, incr);
            if (score is double dou)
            {
                if (dou == double.PositiveInfinity)
                {
                    command.WriteValue("+inf");
                }
                else if (dou == double.NegativeInfinity)
                {
                    command.WriteValue("-inf");
                }
                else
                {
                    command.WriteValue(scoreString);
                }
            }
            else if (score is float @float)
            {
                if (@float == float.PositiveInfinity)
                {
                    command.WriteValue("+inf");
                }
                else if (@float == float.NegativeInfinity)
                {
                    command.WriteValue("-inf");
                }
                else
                {
                    command.WriteValue(scoreString);
                }
            }
            else
            {
                command.WriteValue(scoreString);
            }
            command.WriteValue(member);
            return command;
        }

        internal static CommandPacket ZAdd<TMember, TScore>(string key, Dictionary<TMember, TScore> members, NxXx nxx = NxXx.None, GtLt gl = GtLt.None, bool ch = false, bool incr = false)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
            where TMember : class
        {
            if (members is null || members.Count is 0) throw new RedisException("There are no valid member");
            var command = SortedSetCommands.ZAdd(key, nxx, gl, ch, incr);
            foreach (var item in members)
            {
                var scoreString = item.Value.ToString() ?? throw new RedisException("Score is null!");
                if (item.Value is double dou)
                {
                    if (dou == double.PositiveInfinity)
                    {
                        command.WriteValue("+inf");
                    }
                    else if (dou == double.NegativeInfinity)
                    {
                        command.WriteValue("-inf");
                    }
                    else
                    {
                        command.WriteValue(scoreString);
                    }
                }
                else if (item.Value is float @float)
                {
                    if (@float == float.PositiveInfinity)
                    {
                        command.WriteValue("+inf");
                    }
                    else if (@float == float.NegativeInfinity)
                    {
                        command.WriteValue("-inf");
                    }
                    else
                    {
                        command.WriteValue(scoreString);
                    }
                }
                else
                {
                    command.WriteValue(scoreString);
                }
                command.WriteValue(item.Key);
            }

            return command;
        }

        internal static CommandPacket ZCard(string key)
            => new CommandPacket("ZCARD", CommandMode.Read | CommandMode.WithClientSideCache).WriteKey(key);

        internal static CommandPacket ZCount<TScore>(string key, TScore min, TScore max, bool withMin, bool withMax)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            var minString = min.ToString() ?? throw new RedisException("min is null!");
            var maxString = max.ToString() ?? throw new RedisException("max is null!");
            var command = new CommandPacket("ZCOUNT", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
            if (min is double minDouble)
            {
                if (minDouble == double.PositiveInfinity)
                {
                    minString = "+inf";
                }
                else if (minDouble == double.NegativeInfinity)
                {
                    minString = "-inf";
                }
            }
            else if (min is float minFloat)
            {
                if (minFloat == float.PositiveInfinity)
                {
                    minString = "+inf";
                }
                else if (minFloat == float.NegativeInfinity)
                {
                    minString = "-inf";
                }
            }

            if (max is double maxDouble)
            {
                if (maxDouble == double.PositiveInfinity)
                {
                    maxString = "+inf";
                }
                else if (maxDouble == double.NegativeInfinity)
                {
                    maxString = "-inf";
                }
            }
            else if (max is float maxFloat)
            {
                if (maxFloat == float.PositiveInfinity)
                {
                    maxString = "+inf";
                }
                else if (maxFloat == float.NegativeInfinity)
                {
                    maxString = "-inf";
                }
            }

            if (!withMin) minString = $"({minString}";
            if (!withMax) maxString = $"({maxString}";

            return command
                .WriteArg(minString)
                .WriteArg(maxString);
        }

        internal static CommandPacket ZDiff(bool withScores, params string[] keys)
        {
            if (keys == null || keys.Length <= 0) throw new RedisException("Make sure you have at least one key");
            return new CommandPacket("ZDIFF", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteArg(keys.Length)
                .WriteKeys(keys)
                .WriteArg(withScores, "WITHSCORES");
        }

        internal static CommandPacket ZDiffStore(string destination, params string[] keys)
        {
            if (keys == null || keys.Length <= 0) throw new RedisException("Make sure you have at least one key");
            return new CommandPacket("ZDIFFSTORE", CommandMode.Write)
                .WriteKey(destination)
                .WriteArg(keys.Length)
                .WriteKeys(keys);
        }

        internal static CommandPacket ZIncrBy<TIncrement, TMember>(string key, TIncrement increment, TMember member)
#if NET7_0_OR_GREATER
            where TIncrement : struct, System.Numerics.INumber<TIncrement>
#else
            where TIncrement : struct, System.IEquatable<TIncrement>
#endif
            where TMember : class
        {
            return new CommandPacket("ZINCRBY", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(increment)
                .WriteValue(member);
        }

        private static CommandPacket ZInter(params string[] keys)
        {
            if (keys is null || keys.Length <= 0) throw new RedisException("Make sure you have at least one Key");
            return new CommandPacket("ZINTER", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteArg(keys.Length)
                .WriteKeys(keys);
        }

        internal static CommandPacket ZInter(Aggregate aggregate, bool withScores, params string[] keys)
        {
            var command = SortedSetCommands.ZInter(keys);
            return command
                .WriteArg(aggregate == Aggregate.Min, "AGGREGATE", "MIN")
                .WriteArg(aggregate == Aggregate.Max, "AGGREGATE", "MAX")
                .WriteArg(withScores, "WITHSCORES");
        }

        internal static CommandPacket ZInter<TWeight>(string[] keys, TWeight[] weights, Aggregate aggregate, bool withScores)
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            if (keys.Length != weights.Length) throw new RedisException("The number of multiplier factors and keys does not agree");
            var command = SortedSetCommands.ZInter(keys);
            return command
                .WriteArgs("WEIGHTS", weights)
                .WriteArg(aggregate == Aggregate.Min, "AGGREGATE", "MIN")
                .WriteArg(aggregate == Aggregate.Max, "AGGREGATE", "MAX")
                .WriteArg(withScores, "WITHSCORES");
        }

        internal static CommandPacket ZInterCard(ulong limit, params string[] keys)
        {
            if (keys is null || keys.Length <= 0) throw new RedisException("Make sure you have at least one Key");
            return new CommandPacket("ZINTERCARD", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteArg(keys.Length)
                .WriteKeys(keys)
                .WriteArg(limit != 0, "LIMIT", limit);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket ZInterStore<TWeight>(string destination, string[] keys, TWeight[]? weights, Aggregate aggregate)
#else
        internal static CommandPacket ZInterStore<TWeight>(string destination, string[] keys, TWeight[] weights, Aggregate aggregate)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            if (keys is null || keys.Length <= 0) throw new RedisException("Make sure you have at least one Key");
            if (weights?.Length > 0)
            {
                if (keys.Length != weights.Length) throw new RedisException("The number of multiplier factors and keys does not agree");
            }
            return new CommandPacket("ZINTERSTORE", CommandMode.Write)
                .WriteKey(destination)
                .WriteArg(keys.Length)
                .WriteKeys(keys)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArgs(weights?.Length > 0, "WEIGHTS", weights!)
#else
                .WriteArgs(weights?.Length > 0, "WEIGHTS", weights)
#endif
                .WriteArg(aggregate == Aggregate.Min, "AGGREGATE", "MIN")
                .WriteArg(aggregate == Aggregate.Max, "AGGREGATE", "MAX");
        }

        internal static CommandPacket ZLexCount(string key, string min, string max)
        {
            return new CommandPacket("ZLEXCOUNT", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValue(min)
                .WriteValue(max);
        }

        internal static CommandPacket ZMPop(string[] keys, MaxMin mm, ulong count)
        {
            if (keys is null || keys.Length <= 0) throw new RedisException("Make sure you have at least one Key");
            if (count <= 0) throw new RedisException("Count cannot be 0");
            return new CommandPacket("ZMPOP", CommandMode.Write)
                .WriteArg(keys.Length)
                .WriteKeys(keys)
                .WriteArg(mm == MaxMin.Min, "MIN")
                .WriteArg(mm == MaxMin.Max, "MAX")
                .WriteArg(count > 1, "COUNT", count);
        }

        internal static CommandPacket ZMScore<TMember>(string key, params TMember[] members)
            where TMember : class
        {
            if (members is null || members.Length <= 0) throw new RedisException("Please make sure there is at least one member");
            return new CommandPacket("ZMSCORE", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValues(members);
        }

        internal static CommandPacket ZPopMax(string key, ulong count)
        {
            if (count <= 0) throw new RedisException("Count cannot be 0");
            return new CommandPacket("ZPOPMAX", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(count > 1, count);
        }

        internal static CommandPacket ZPopMin(string key, ulong count)
        {
            if (count <= 0) throw new RedisException("Count cannot be 0");
            return new CommandPacket("ZPOPMIN", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(count > 1, count);
        }

        internal static CommandPacket ZRandMember(string key, long? count, bool withScores)
        {
            return new CommandPacket("ZRANDMEMBER", CommandMode.Read)
                .WriteKey(key)
                .WriteArg(count.HasValue, count ?? 0)
                .WriteArg(withScores, "WITHSCORES");
        }

        internal static CommandPacket ZRange<TArg>(string key, TArg start, bool withStart, TArg stop, bool withStop, ZRangeBy? by, bool? rev, ulong? offset, ulong? count, bool withScores)
        {
            var startString = start?.ToString() ?? throw new RedisException("start is null!");
            var stopString = stop?.ToString() ?? throw new RedisException("stop is null!");

            var command = new CommandPacket("ZRANGE", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);

            if (start is double startDouble)
            {
                if (startDouble == double.PositiveInfinity)
                {
                    startString = "+inf";
                }
                else if (startDouble == double.NegativeInfinity)
                {
                    startString = "-inf";
                }
            }
            else if (start is float startFloat)
            {
                if (startFloat == float.PositiveInfinity)
                {
                    startString = "+inf";
                }
                else if (startFloat == float.NegativeInfinity)
                {
                    startString = "-inf";
                }
            }

            if (stop is double stopDouble)
            {
                if (stopDouble == double.PositiveInfinity)
                {
                    stopString = "+inf";
                }
                else if (stopDouble == double.NegativeInfinity)
                {
                    stopString = "-inf";
                }
            }
            else if (stop is float maxFloat)
            {
                if (maxFloat == float.PositiveInfinity)
                {
                    stopString = "+inf";
                }
                else if (maxFloat == float.NegativeInfinity)
                {
                    stopString = "-inf";
                }
            }

            if (by == ZRangeBy.ByScore && !withStart) startString = $"({startString}";
            if (by == ZRangeBy.ByScore && !withStop) stopString = $"({stopString}";

            return command
                .WriteArg(startString)
                .WriteArg(stopString)
                .WriteArg(by == ZRangeBy.ByScore, "BYSCORE")
                .WriteArg(by == ZRangeBy.ByLex, "BYLEX")
                .WriteArg(rev.HasValue && rev.Value, "REV")
                .WriteArg(offset.HasValue, "LIMIT", offset ?? 0)
                .WriteArg(count.HasValue, count ?? 0)
                .WriteArg(withScores, "WITHSCORES");
        }

        internal static CommandPacket ZRangeByLex(string key, string min, string max, ulong? offset, ulong? count)
        {
            return new CommandPacket("ZRANGEBYLEX", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(min)
                .WriteArg(max)
                .WriteArg(offset.HasValue, "LIMIT", offset ?? 0)
                .WriteArg(count.HasValue, count ?? 0);
        }

        internal static CommandPacket ZRangeByScore<TScore>(string key, TScore min, bool withMin, TScore max, bool withMax, ulong? offset, ulong? count, bool withScores)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            var minString = min.ToString() ?? throw new RedisException("min is null!");
            var maxString = max.ToString() ?? throw new RedisException("max is null!");
            var command = new CommandPacket("ZRANGEBYSCORE", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key);
            if (min is double minDouble)
            {
                if (minDouble == double.PositiveInfinity)
                {
                    minString = "+inf";
                }
                else if (minDouble == double.NegativeInfinity)
                {
                    minString = "-inf";
                }
            }
            else if (min is float minFloat)
            {
                if (minFloat == float.PositiveInfinity)
                {
                    minString = "+inf";
                }
                else if (minFloat == float.NegativeInfinity)
                {
                    minString = "-inf";
                }
            }

            if (max is double maxDouble)
            {
                if (maxDouble == double.PositiveInfinity)
                {
                    maxString = "+inf";
                }
                else if (maxDouble == double.NegativeInfinity)
                {
                    maxString = "-inf";
                }
            }
            else if (max is float maxFloat)
            {
                if (maxFloat == float.PositiveInfinity)
                {
                    maxString = "+inf";
                }
                else if (maxFloat == float.NegativeInfinity)
                {
                    maxString = "-inf";
                }
            }

            if (!withMin) minString = $"({minString}";
            if (!withMax) maxString = $"({maxString}";

            return command
                .WriteArg(minString)
                .WriteArg(maxString)
                .WriteArg(withScores, "WITHSCORES")
                .WriteArg(offset.HasValue, "LIMIT", offset ?? 0)
                .WriteArg(count.HasValue, count ?? 0);
        }

        internal static CommandPacket ZRangeStore<TArg>(string destination, string key, TArg start, bool withStart, TArg stop, bool withStop, ZRangeBy? by, bool? rev, ulong? offset, ulong? count)
        {
            var startString = start?.ToString() ?? throw new RedisException("start is null!");
            var stopString = stop?.ToString() ?? throw new RedisException("stop is null!");

            var command = new CommandPacket("ZRANGESTORE", CommandMode.Write)
                .WriteKey(destination)
                .WriteKey(key);

            if (start is double startDouble)
            {
                if (startDouble == double.PositiveInfinity)
                {
                    startString = "+inf";
                }
                else if (startDouble == double.NegativeInfinity)
                {
                    startString = "-inf";
                }
            }
            else if (start is float startFloat)
            {
                if (startFloat == float.PositiveInfinity)
                {
                    startString = "+inf";
                }
                else if (startFloat == float.NegativeInfinity)
                {
                    startString = "-inf";
                }
            }

            if (stop is double stopDouble)
            {
                if (stopDouble == double.PositiveInfinity)
                {
                    stopString = "+inf";
                }
                else if (stopDouble == double.NegativeInfinity)
                {
                    stopString = "-inf";
                }
            }
            else if (stop is float maxFloat)
            {
                if (maxFloat == float.PositiveInfinity)
                {
                    stopString = "+inf";
                }
                else if (maxFloat == float.NegativeInfinity)
                {
                    stopString = "-inf";
                }
            }

            if (by == ZRangeBy.ByScore && !withStart) startString = $"({startString}";
            if (by == ZRangeBy.ByScore && !withStop) stopString = $"({stopString}";

            return command
                .WriteArg(startString)
                .WriteArg(stopString)
                .WriteArg(by == ZRangeBy.ByScore, "BYSCORE")
                .WriteArg(by == ZRangeBy.ByLex, "BYLEX")
                .WriteArg(rev.HasValue && rev.Value, "REV")
                .WriteArg(offset.HasValue, "LIMIT", offset ?? 0)
                .WriteArg(count.HasValue, count ?? 0);
        }

        internal static CommandPacket ZRank<TMember>(string key, TMember member, bool withScore)
            where TMember : class
        {
            return new CommandPacket("ZRANK", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValue(member)
                .WriteArg(withScore, "WITHSCORE");
        }

        internal static CommandPacket ZRem<TMember>(string key, params TMember[] members)
            where TMember : class
        {
            return new CommandPacket("ZREM", CommandMode.Write)
                .WriteKey(key)
                .WriteValues(members);
        }

        internal static CommandPacket ZRemRangeByLex(string key, string min, string max)
        {
            return new CommandPacket("ZREMRANGEBYLEX", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(min)
                .WriteValue(max);
        }

        internal static CommandPacket ZRemRangeByRank(string key, long start, long stop)
        {
            return new CommandPacket("ZREMRANGEBYRANK", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(start)
                .WriteArg(stop);
        }

        internal static CommandPacket ZRemRangeByScore<TScore>(string key, TScore min, bool withMin, TScore max, bool withMax)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            var minString = min.ToString() ?? throw new RedisException("min is null!");
            var maxString = max.ToString() ?? throw new RedisException("max is null!");
            if (min is double minDouble)
            {
                if (minDouble == double.PositiveInfinity)
                {
                    minString = "+inf";
                }
                else if (minDouble == double.NegativeInfinity)
                {
                    minString = "-inf";
                }
            }
            else if (min is float minFloat)
            {
                if (minFloat == float.PositiveInfinity)
                {
                    minString = "+inf";
                }
                else if (minFloat == float.NegativeInfinity)
                {
                    minString = "-inf";
                }
            }

            if (max is double maxDouble)
            {
                if (maxDouble == double.PositiveInfinity)
                {
                    maxString = "+inf";
                }
                else if (maxDouble == double.NegativeInfinity)
                {
                    maxString = "-inf";
                }
            }
            else if (max is float maxFloat)
            {
                if (maxFloat == float.PositiveInfinity)
                {
                    maxString = "+inf";
                }
                else if (maxFloat == float.NegativeInfinity)
                {
                    maxString = "-inf";
                }
            }

            if (!withMin) minString = $"({minString}";
            if (!withMax) maxString = $"({maxString}";
            return new CommandPacket("ZREMRANGEBYSCORE", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(minString)
                .WriteArg(maxString);
        }

        internal static CommandPacket ZRevRange(string key, long start, long stop, bool withScore)
        {
            return new CommandPacket("ZREVRANGE", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(start)
                .WriteArg(stop)
                .WriteArg(withScore, "WITHSCORES");
        }

        internal static CommandPacket ZRevRangeByLex(string key, string max, string min, ulong? offset = null, ulong? count = null)
        {
            return new CommandPacket("ZREVRANGEBYLEX", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(max)
                .WriteArg(min)
                .WriteArg(offset.HasValue, "LIMIT", offset ?? 0)
                .WriteArg(count.HasValue, count ?? 0);
        }

        internal static CommandPacket ZRevRangeByScore<TScore>(string key, TScore max, bool withMax, TScore min, bool withMin, bool withScores, ulong? offset = null, ulong? count = null)
#if NET7_0_OR_GREATER
            where TScore : struct, System.Numerics.INumber<TScore>
#else
            where TScore : struct, System.IEquatable<TScore>
#endif
        {
            var minString = min.ToString() ?? throw new RedisException("min is null!");
            var maxString = max.ToString() ?? throw new RedisException("max is null!");
            if (min is double minDouble)
            {
                if (minDouble == double.PositiveInfinity)
                {
                    minString = "+inf";
                }
                else if (minDouble == double.NegativeInfinity)
                {
                    minString = "-inf";
                }
            }
            else if (min is float minFloat)
            {
                if (minFloat == float.PositiveInfinity)
                {
                    minString = "+inf";
                }
                else if (minFloat == float.NegativeInfinity)
                {
                    minString = "-inf";
                }
            }

            if (max is double maxDouble)
            {
                if (maxDouble == double.PositiveInfinity)
                {
                    maxString = "+inf";
                }
                else if (maxDouble == double.NegativeInfinity)
                {
                    maxString = "-inf";
                }
            }
            else if (max is float maxFloat)
            {
                if (maxFloat == float.PositiveInfinity)
                {
                    maxString = "+inf";
                }
                else if (maxFloat == float.NegativeInfinity)
                {
                    maxString = "-inf";
                }
            }

            if (!withMin) minString = $"({minString}";
            if (!withMax) maxString = $"({maxString}";

            return new CommandPacket("ZREVRANGEBYSCORE", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(maxString)
                .WriteArg(minString)
                .WriteArg(withScores, "WITHSCORES")
                .WriteArg(offset.HasValue, "LIMIT", offset ?? 0)
                .WriteArg(count.HasValue, count ?? 0);
        }

        internal static CommandPacket ZRevRank<TMember>(string key, TMember member, bool withScore)
            where TMember : class
        {
            return new CommandPacket("ZREVRANK", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValue(member)
                .WriteArg(withScore, "WITHSCORE");
        }

        internal static CommandPacket ZScore<TMember>(string key, TMember member)
            where TMember : class
        {
            return new CommandPacket("ZSCORE", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValue(member);
        }

        private static CommandPacket ZUnion(params string[] keys)
        {
            if (keys is null || keys.Length <= 0) throw new RedisException("Make sure you have at least one Key");
            return new CommandPacket("ZUNION", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteArg(keys.Length)
                .WriteKeys(keys);
        }

        internal static CommandPacket ZUnion(Aggregate aggregate, bool withScores, params string[] keys)
        {
            var command = SortedSetCommands.ZUnion(keys);
            return command
                .WriteArg(aggregate == Aggregate.Min, "AGGREGATE", "MIN")
                .WriteArg(aggregate == Aggregate.Max, "AGGREGATE", "MAX")
                .WriteArg(withScores, "WITHSCORES");
        }

        internal static CommandPacket ZUnion<TWeight>(string[] keys, TWeight[] weights, Aggregate aggregate, bool withScores)
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            if (keys.Length != weights.Length) throw new RedisException("The number of multiplier factors and keys does not agree");
            var command = SortedSetCommands.ZUnion(keys);
            return command
                .WriteArgs("WEIGHTS", weights)
                .WriteArg(aggregate == Aggregate.Min, "AGGREGATE", "MIN")
                .WriteArg(aggregate == Aggregate.Max, "AGGREGATE", "MAX")
                .WriteArg(withScores, "WITHSCORES");
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket ZUnionStore<TWeight>(string destination, string[] keys, TWeight[]? weights, Aggregate aggregate)
#else
        internal static CommandPacket ZUnionStore<TWeight>(string destination, string[] keys, TWeight[] weights, Aggregate aggregate)
#endif
#if NET7_0_OR_GREATER
            where TWeight : struct, System.Numerics.INumber<TWeight>
#else
            where TWeight : struct, System.IEquatable<TWeight>
#endif
        {
            if (keys is null || keys.Length <= 0) throw new RedisException("Make sure you have at least one Key");
            if (weights?.Length > 0)
            {
                if (keys.Length != weights.Length) throw new RedisException("The number of multiplier factors and keys does not agree");
            }
            return new CommandPacket("ZUNIONSTORE", CommandMode.Write)
                .WriteKey(destination)
                .WriteArg(keys.Length)
                .WriteKeys(keys)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArgs(weights?.Length > 0, "WEIGHTS", weights!)
#else
                .WriteArgs(weights?.Length > 0, "WEIGHTS", weights)
#endif
                .WriteArg(aggregate == Aggregate.Min, "AGGREGATE", "MIN")
                .WriteArg(aggregate == Aggregate.Max, "AGGREGATE", "MAX");
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket ZScan(string key, long cursor, string? pattern, ulong? count)
#else
        internal static CommandPacket ZScan(string key, long cursor, string pattern, ulong? count)
#endif
        {
            return new CommandPacket("ZSCAN", CommandMode.Read)
                .WriteKey(key)
                .WriteArg(cursor)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArg(!string.IsNullOrEmpty(pattern), "MATCH", pattern!)
#else
                .WriteArg(!string.IsNullOrEmpty(pattern), "MATCH", pattern)
#endif
                .WriteArg(count.HasValue, "COUNT", count ?? 0)
                ;
        }

        internal static CommandPacket BZMPop(double timeout, string[] keys, MaxMin mm, ulong count)
        {
            if (count <= 0) throw new RedisException("Count cannot be 0");
            return new CommandPacket("BZMPOP", CommandMode.Write | CommandMode.WithBlock)
                .WriteArg(timeout)
                .WriteArg(keys.Length)
                .WriteKeys(keys)
                .WriteArg(mm == MaxMin.Min, "MIN")
                .WriteArg(mm == MaxMin.Max, "MAX")
                .WriteArg(count > 1, "COUNT", count);
        }

        internal static CommandPacket BZPopMax(string[] keys, string timeout)
        {
            return new CommandPacket("BZPOPMAX", CommandMode.Write | CommandMode.WithBlock)
                .WriteKeys(keys)
                .WriteArg(timeout);
        }

        internal static CommandPacket BZPopMin(string[] keys, string timeout)
        {
            return new CommandPacket("BZPOPMIN", CommandMode.Write | CommandMode.WithBlock)
                .WriteKeys(keys)
                .WriteArg(timeout);
        }
    }
}
