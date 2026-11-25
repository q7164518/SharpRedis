using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using static SharpRedis.Enums.CommandMode;
using static SharpRedis.RedisSortedSet;

namespace SharpRedis.Commands;

internal static class SortedSetCommands
{
    private const string _withScoresString = "WITHSCORES";

    private static readonly SharpValue _zadd = new("ZADD", CommandItemCategory.Command);
    private static readonly SharpValue _zcard = new("ZCARD", CommandItemCategory.Command);
    private static readonly SharpValue _zscore = new("ZSCORE", CommandItemCategory.Command);
    private static readonly SharpValue _zcount = new("ZCOUNT", CommandItemCategory.Command);
    private static readonly SharpValue _zdiff = new("ZDIFF", CommandItemCategory.Command);
    private static readonly SharpValue _zdiffStore = new("ZDIFFSTORE", CommandItemCategory.Command);
    private static readonly SharpValue _zrange = new("ZRANGE", CommandItemCategory.Command);
    private static readonly SharpValue _zincrby = new("ZINCRBY", CommandItemCategory.Command);
    private static readonly SharpValue _zinter = new("ZINTER", CommandItemCategory.Command);
    private static readonly SharpValue _zinterCard = new("ZINTERCARD", CommandItemCategory.Command);
    private static readonly SharpValue _zinterStore = new("ZINTERSTORE", CommandItemCategory.Command);
    private static readonly SharpValue _zlexCount = new("ZLEXCOUNT", CommandItemCategory.Command);
    private static readonly SharpValue _zmpop = new("ZMPOP", CommandItemCategory.Command);
    private static readonly SharpValue _zmscore = new("ZMSCORE", CommandItemCategory.Command);
    private static readonly SharpValue _zpopmax = new("ZPOPMAX", CommandItemCategory.Command);
    private static readonly SharpValue _zpopmin = new("ZPOPMIN", CommandItemCategory.Command);
    private static readonly SharpValue _zrandmember = new("ZRANDMEMBER", CommandItemCategory.Command);
    private static readonly SharpValue _zrangebylex = new("ZRANGEBYLEX", CommandItemCategory.Command);
    private static readonly SharpValue _zrangebyscore = new("ZRANGEBYSCORE", CommandItemCategory.Command);
    private static readonly SharpValue _zrangestore = new("ZRANGESTORE", CommandItemCategory.Command);
    private static readonly SharpValue _zrank = new("ZRANK", CommandItemCategory.Command);
    private static readonly SharpValue _zrem = new("ZREM", CommandItemCategory.Command);
    private static readonly SharpValue _zremrangebylex = new("ZREMRANGEBYLEX", CommandItemCategory.Command);
    private static readonly SharpValue _zremrangebyrank = new("ZREMRANGEBYRANK", CommandItemCategory.Command);
    private static readonly SharpValue _zremrangebyscore = new("ZREMRANGEBYSCORE", CommandItemCategory.Command);
    private static readonly SharpValue _zrevrange = new("ZREVRANGE", CommandItemCategory.Command);
    private static readonly SharpValue _zrevrangebylex = new("ZREVRANGEBYLEX", CommandItemCategory.Command);
    private static readonly SharpValue _zrevrangebyscore = new("ZREVRANGEBYSCORE", CommandItemCategory.Command);
    private static readonly SharpValue _zrevrank = new("ZREVRANK", CommandItemCategory.Command);
    private static readonly SharpValue _zunion = new("ZUNION", CommandItemCategory.Command);
    private static readonly SharpValue _zunionstore = new("ZUNIONSTORE", CommandItemCategory.Command);
    private static readonly SharpValue _bzmpop = new("BZMPOP", CommandItemCategory.Command);
    private static readonly SharpValue _bzpopmax = new("BZPOPMAX", CommandItemCategory.Command);
    private static readonly SharpValue _bzpopmin = new("BZPOPMIN", CommandItemCategory.Command);
    private static readonly SharpValue _bzmove = new("BZMOVE", CommandItemCategory.Command);
    private static readonly SharpValue _zmove = new("ZMOVE", CommandItemCategory.Command);
    private static readonly SharpValue _zmovemember = new("ZMOVEMEMBER", CommandItemCategory.Command);


    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static CommandPacket WriteDouble(this CommandPacket command, double value)
    {
        if (value == double.PositiveInfinity)
        {
            return command.WriteArguments("inf");
        }
        else if (value == double.NegativeInfinity)
        {
            return command.WriteArguments("-inf");
        }
        else
        {
            return command.WriteArguments(value.ToBytes());
        }
    }

    internal static CommandPacket ZAdd(SharpValue key, SharpValue member, double score, ZAddWhen when, ZAddScoreWhen scoreWhen, bool ch, bool incr)
    {
        return new CommandPacket(SortedSetCommands._zadd, ZSet | WithWrite)
            .WriteKeys(key)
            .WriteArguments(when is ZAddWhen.NX, nameof(ZAddWhen.NX))
            .WriteArguments(when is ZAddWhen.XX, nameof(ZAddWhen.XX))
            .WriteArguments(scoreWhen is ZAddScoreWhen.LT, nameof(ZAddScoreWhen.LT))
            .WriteArguments(scoreWhen is ZAddScoreWhen.GT, nameof(ZAddScoreWhen.GT))
            .WriteArguments(ch, "CH")
            .WriteArguments(incr, "INCR")
            .WriteDouble(score)
            .WriteArguments(member);
    }

    internal static CommandPacket ZAdd(SharpValue key, IEnumerable<KeyValuePair<SharpValue, double>> members, ZAddWhen when, ZAddScoreWhen scoreWhen, bool ch, bool incr)
    {
        var command = new CommandPacket(SortedSetCommands._zadd, ZSet | WithWrite)
            .WriteKeys(key)
            .WriteArguments(when is ZAddWhen.NX, nameof(ZAddWhen.NX))
            .WriteArguments(when is ZAddWhen.XX, nameof(ZAddWhen.XX))
            .WriteArguments(scoreWhen is ZAddScoreWhen.LT, nameof(ZAddScoreWhen.LT))
            .WriteArguments(scoreWhen is ZAddScoreWhen.GT, nameof(ZAddScoreWhen.GT))
            .WriteArguments(ch, "CH")
            .WriteArguments(incr, "INCR");

        foreach (var member in members)
        {
            _ = command
                .WriteDouble(member.Value)
                .WriteArguments(member.Key);
        }

        return command;
    }

    internal static CommandPacket ZCard(SharpValue key)
        => new CommandPacket(SortedSetCommands._zcard, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key);

    internal static CommandPacket ZScore(SharpValue key, SharpValue member)
        => new CommandPacket(SortedSetCommands._zscore, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(member);

    internal static CommandPacket ZCount(SharpValue key, double min, double max)
    {
        return new CommandPacket(SortedSetCommands._zcount, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteDouble(min)
            .WriteDouble(max);
    }

    internal static CommandPacket ZDiff(SharpValue key1, SharpValue key2, bool withScores)
        => new CommandPacket(SortedSetCommands._zdiff, ZSet | WithRead | WithClientCaching)
            .WriteArguments("2")
            .WriteKeys(key1)
            .WriteKeys(key2)
            .WriteArguments(withScores, SortedSetCommands._withScoresString);

    internal static CommandPacket ZDiff(SharpValueSpan keys, bool withScores)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(SortedSetCommands._zdiff, ZSet | WithRead | WithClientCaching)
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys)
            .WriteArguments(withScores, SortedSetCommands._withScoresString);
    }

    internal static CommandPacket ZDiffStore(SharpValue key1, SharpValue key2, SharpValue destination)
        => new CommandPacket(SortedSetCommands._zdiffStore, ZSet | WithWrite)
            .WriteKeys(destination)
            .WriteArguments("2")
            .WriteKeys(key1)
            .WriteKeys(key2);

    internal static CommandPacket ZDiffStore(SharpValueSpan keys, SharpValue destination)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(SortedSetCommands._zdiffStore, ZSet | WithWrite)
            .WriteKeys(destination)
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys);
    }

    internal static CommandPacket ZRange(SharpValue key, RangeArg start, RangeArg stop, RangeBy by, bool rev, ulong? offset, int? count, bool withScores = false)
    {
        var command = new CommandPacket(SortedSetCommands._zrange, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(start.Value)
            .WriteArguments(stop.Value)
            .WriteArguments(by is RangeBy.ByScore, "BYSCORE")
            .WriteArguments(by is RangeBy.ByLex, "BYLEX")
            .WriteArguments(rev, "REV");
        if (offset.HasValue && count.HasValue)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(offset.Value.ToBytes())
                .WriteArguments(count.Value.ToBytes());
        }

        return command.WriteArguments(withScores, SortedSetCommands._withScoresString);
    }

    internal static CommandPacket ZIncrBy(SharpValue key, double increment, SharpValue member)
    {
        return new CommandPacket(SortedSetCommands._zincrby, ZSet | WithWrite)
            .WriteKeys(key)
            .WriteDouble(increment)
            .WriteArguments(member);
    }

    internal static CommandPacket ZInter(
        SharpValue key1,
        SharpValue key2,
        double key1Weight,
        double key2Weight,
        Aggregate aggregate,
        bool withScores = false)
    {
        var command = new CommandPacket(SortedSetCommands._zinter, ZSet | WithRead | WithClientCaching)
            .WriteArguments("2")
            .WriteKeys(key1)
            .WriteKeys(key2)
            .WriteArguments("WEIGHTS")
            .WriteDouble(key1Weight)
            .WriteDouble(key2Weight);

        if (aggregate != Aggregate.Sum)
        {
            _ = command.WriteArguments("AGGREGATE");
            switch (aggregate)
            {
                case Aggregate.Min:
                    _ = command.WriteArguments("MIN");
                    break;
                case Aggregate.Max:
                    _ = command.WriteArguments("MAX");
                    break;
                default:
                    break;
            }
        }

        return command.WriteArguments(withScores, SortedSetCommands._withScoresString);
    }

    internal static CommandPacket ZInter(
        SharpValueSpan keys,
        ReadOnlySpan<double> weights,
        Aggregate aggregate,
        bool withScores = false)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        var command = new CommandPacket(SortedSetCommands._zinter, ZSet | WithRead | WithClientCaching)
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys)
            .WriteArguments("WEIGHTS");

        if (weights.IsEmpty)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                _ = command.WriteArguments("1");
            }
        }
        else
        {
            if (weights.Length == keys.Count)
            {
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                }
            }
            else if (weights.Length > keys.Count)
            {
                int i = 0;
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                    i++;
                    if (i >= keys.Count) break;
                }
            }
            else
            {
                int i = 0;
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                    i++;
                }
                for (; i < keys.Count; i++)
                {
                    _ = command.WriteArguments("1");
                }
            }
        }

        if (aggregate != Aggregate.Sum)
        {
            _ = command.WriteArguments("AGGREGATE");
            switch (aggregate)
            {
                case Aggregate.Min:
                    _ = command.WriteArguments("MIN");
                    break;
                case Aggregate.Max:
                    _ = command.WriteArguments("MAX");
                    break;
                default:
                    break;
            }
        }

        return command.WriteArguments(withScores, SortedSetCommands._withScoresString);
    }

    internal static CommandPacket ZInterCard(SharpValue key1, SharpValue key2, ulong limit)
    {
        var command = new CommandPacket(SortedSetCommands._zinterCard, ZSet | WithRead | WithClientCaching)
            .WriteArguments("2")
            .WriteKeys(key1)
            .WriteKeys(key2);
        if (limit > 0)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(limit.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ZInterCard(SharpValueSpan keys, ulong limit)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        var command = new CommandPacket(SortedSetCommands._zinterCard, ZSet | WithRead | WithClientCaching)
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys);
        if (limit > 0)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(limit.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ZInterStore(
        SharpValue destination,
        SharpValue key1,
        SharpValue key2,
        double key1Weight,
        double key2Weight,
        Aggregate aggregate)
    {
        var command = new CommandPacket(SortedSetCommands._zinterStore, ZSet | WithWrite)
            .WriteKeys(destination)
            .WriteArguments("2")
            .WriteKeys(key1)
            .WriteKeys(key2)
            .WriteArguments("WEIGHTS")
            .WriteDouble(key1Weight)
            .WriteDouble(key2Weight);

        if (aggregate != Aggregate.Sum)
        {
            _ = command.WriteArguments("AGGREGATE");
            switch (aggregate)
            {
                case Aggregate.Min:
                    _ = command.WriteArguments("MIN");
                    break;
                case Aggregate.Max:
                    _ = command.WriteArguments("MAX");
                    break;
                default:
                    break;
            }
        }

        return command;
    }

    internal static CommandPacket ZInterStore(
        SharpValue destination,
        SharpValueSpan keys,
        ReadOnlySpan<double> weights,
        Aggregate aggregate)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        var command = new CommandPacket(SortedSetCommands._zinterStore, ZSet | WithWrite)
            .WriteKeys(destination)
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys)
            .WriteArguments("WEIGHTS");

        if (weights.IsEmpty)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                _ = command.WriteArguments("1");
            }
        }
        else
        {
            if (weights.Length == keys.Count)
            {
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                }
            }
            else if (weights.Length > keys.Count)
            {
                int i = 0;
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                    i++;
                    if (i >= keys.Count) break;
                }
            }
            else
            {
                int i = 0;
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                    i++;
                }
                for (; i < keys.Count; i++)
                {
                    _ = command.WriteArguments("1");
                }
            }
        }

        if (aggregate != Aggregate.Sum)
        {
            _ = command.WriteArguments("AGGREGATE");
            switch (aggregate)
            {
                case Aggregate.Min:
                    _ = command.WriteArguments("MIN");
                    break;
                case Aggregate.Max:
                    _ = command.WriteArguments("MAX");
                    break;
                default:
                    break;
            }
        }

        return command;
    }

    internal static CommandPacket ZLexCount(SharpValue key, RangeArg min, RangeArg max)
    {
        return new CommandPacket(SortedSetCommands._zlexCount, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(min.Value)
            .WriteArguments(max.Value);
    }

    internal static CommandPacket ZMPop(SharpValue key1, SharpValue key2, PopHow how, int count)
    {
        var command = new CommandPacket(SortedSetCommands._zmpop, ZSet | WithWrite)
            .WriteArguments("2")
            .WriteKeys(key1)
            .WriteKeys(key2)
            .WriteArguments(how is PopHow.Min, "MIN")
            .WriteArguments(how is PopHow.Max, "MAX");
        if (count > 1)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.ToBytes());
        }

        return command;
    }

    internal static CommandPacket ZMPop(SharpValueSpan keys, PopHow how, int count)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        var command = new CommandPacket(SortedSetCommands._zmpop, ZSet | WithWrite)
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys)
            .WriteArguments(how is PopHow.Min, "MIN")
            .WriteArguments(how is PopHow.Max, "MAX");
        if (count > 1)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.ToBytes());
        }

        return command;
    }

    internal static CommandPacket ZMScore(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(SortedSetCommands._zmscore, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(members);
    }

    internal static CommandPacket ZPopMax(SharpValue key, int count)
    {
        var command = new CommandPacket(SortedSetCommands._zpopmax, ZSet | WithWrite)
            .WriteKeys(key);
        if (count > 1)
        {
            _ = command.WriteArguments(count.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ZPopMin(SharpValue key, int count)
    {
        var command = new CommandPacket(SortedSetCommands._zpopmin, ZSet | WithWrite)
            .WriteKeys(key);
        if (count > 1)
        {
            _ = command.WriteArguments(count.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ZRandMember(SharpValue key, int count, bool withScores)
    {
        return new CommandPacket(SortedSetCommands._zrandmember, ZSet | WithRead)
           .WriteKeys(key)
           .WriteArguments(count.ToBytes())
           .WriteArguments(withScores, SortedSetCommands._withScoresString);
    }

    internal static CommandPacket ZRangeByLex(SharpValue key, RangeArg min, RangeArg max, ulong? offset, int? count)
    {
        var command = new CommandPacket(SortedSetCommands._zrangebylex, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(min.Value)
            .WriteArguments(max.Value);
        if (offset.HasValue && count.HasValue)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(offset.Value.ToBytes())
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ZRangeByScore(SharpValue key, RangeArg min, RangeArg max, bool withScores, ulong? offset, int? count)
    {
        var command = new CommandPacket(SortedSetCommands._zrangebyscore, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(min.Value)
            .WriteArguments(max.Value)
            .WriteArguments(withScores, SortedSetCommands._withScoresString);
        if (offset.HasValue && count.HasValue)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(offset.Value.ToBytes())
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ZRangeStore(SharpValue destination, SharpValue key, RangeArg min, RangeArg max, RangeBy by, bool rev, ulong? offset, int? count)
    {
        var command = new CommandPacket(SortedSetCommands._zrangestore, ZSet | WithWrite)
            .WriteKeys(destination)
            .WriteKeys(key)
            .WriteArguments(min.Value)
            .WriteArguments(max.Value)
            .WriteArguments(by is RangeBy.ByScore, "BYSCORE")
            .WriteArguments(by is RangeBy.ByLex, "BYLEX")
            .WriteArguments(rev, "REV");
        if (offset.HasValue && count.HasValue)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(offset.Value.ToBytes())
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ZRank(SharpValue key, SharpValue member, bool withScores)
    {
        return new CommandPacket(SortedSetCommands._zrank, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(member)
            .WriteArguments(withScores, "WITHSCORE");
    }

    internal static CommandPacket ZRem(SharpValue key, SharpValue member)
    {
        return new CommandPacket(SortedSetCommands._zrem, ZSet | WithWrite)
            .WriteKeys(key)
            .WriteArguments(member);
    }

    internal static CommandPacket ZRem(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(SortedSetCommands._zrem, ZSet | WithWrite)
            .WriteKeys(key)
            .WriteArguments(members);
    }

    internal static CommandPacket ZRemRangeByLex(SharpValue key, RangeArg min, RangeArg max)
    {
        return new CommandPacket(SortedSetCommands._zremrangebylex, ZSet | WithWrite)
            .WriteKeys(key)
            .WriteArguments(min.Value)
            .WriteArguments(max.Value);
    }

    internal static CommandPacket ZRemRangeByRank(SharpValue key, long start, long stop)
    {
        return new CommandPacket(SortedSetCommands._zremrangebyrank, ZSet | WithWrite)
            .WriteKeys(key)
            .WriteArguments(start.ToBytes())
            .WriteArguments(stop.ToBytes());
    }

    internal static CommandPacket ZRemRangeByScore(SharpValue key, RangeArg min, RangeArg max)
    {
        return new CommandPacket(SortedSetCommands._zremrangebyscore, ZSet | WithWrite)
            .WriteKeys(key)
            .WriteArguments(min.Value)
            .WriteArguments(max.Value);
    }

    internal static CommandPacket ZRevRange(SharpValue key, long start, long stop, bool withScores)
    {
        return new CommandPacket(SortedSetCommands._zrevrange, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(start.ToBytes())
            .WriteArguments(stop.ToBytes())
            .WriteArguments(withScores, SortedSetCommands._withScoresString);
    }

    internal static CommandPacket ZRevRangeByLex(SharpValue key, RangeArg max, RangeArg min, ulong? offset, int? count)
    {
        var command = new CommandPacket(SortedSetCommands._zrevrangebylex, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(max.Value)
            .WriteArguments(min.Value);
        if (offset.HasValue && count.HasValue)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(offset.Value.ToBytes())
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ZRevRangeByScore(SharpValue key, RangeArg max, RangeArg min, bool withScores, ulong? offset, int? count)
    {
        var command = new CommandPacket(SortedSetCommands._zrevrangebyscore, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(max.Value)
            .WriteArguments(min.Value)
            .WriteArguments(withScores, SortedSetCommands._withScoresString);
        if (offset.HasValue && count.HasValue)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(offset.Value.ToBytes())
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ZRevRank(SharpValue key, SharpValue member, bool withScores)
    {
        return new CommandPacket(SortedSetCommands._zrevrank, ZSet | WithRead | WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(member)
            .WriteArguments(withScores, "WITHSCORE");
    }

    internal static CommandPacket ZUnion(
        SharpValue key1,
        SharpValue key2,
        double key1Weight,
        double key2Weight,
        Aggregate aggregate,
        bool withScores = false)
    {
        var command = new CommandPacket(SortedSetCommands._zunion, ZSet | WithRead | WithClientCaching)
            .WriteArguments("2")
            .WriteKeys(key1)
            .WriteKeys(key2)
            .WriteArguments("WEIGHTS")
            .WriteDouble(key1Weight)
            .WriteDouble(key2Weight);

        if (aggregate != Aggregate.Sum)
        {
            _ = command.WriteArguments("AGGREGATE");
            switch (aggregate)
            {
                case Aggregate.Min:
                    _ = command.WriteArguments("MIN");
                    break;
                case Aggregate.Max:
                    _ = command.WriteArguments("MAX");
                    break;
                default:
                    break;
            }
        }

        return command.WriteArguments(withScores, SortedSetCommands._withScoresString);
    }

    internal static CommandPacket ZUnion(
        SharpValueSpan keys,
        ReadOnlySpan<double> weights,
        Aggregate aggregate,
        bool withScores = false)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        var command = new CommandPacket(SortedSetCommands._zunion, ZSet | WithRead | WithClientCaching)
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys)
            .WriteArguments("WEIGHTS");

        if (weights.IsEmpty)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                _ = command.WriteArguments("1");
            }
        }
        else
        {
            if (weights.Length == keys.Count)
            {
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                }
            }
            else if (weights.Length > keys.Count)
            {
                int i = 0;
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                    i++;
                    if (i >= keys.Count) break;
                }
            }
            else
            {
                int i = 0;
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                    i++;
                }
                for (; i < keys.Count; i++)
                {
                    _ = command.WriteArguments("1");
                }
            }
        }

        if (aggregate != Aggregate.Sum)
        {
            _ = command.WriteArguments("AGGREGATE");
            switch (aggregate)
            {
                case Aggregate.Min:
                    _ = command.WriteArguments("MIN");
                    break;
                case Aggregate.Max:
                    _ = command.WriteArguments("MAX");
                    break;
                default:
                    break;
            }
        }

        return command.WriteArguments(withScores, SortedSetCommands._withScoresString);
    }

    internal static CommandPacket ZUnionStore(
        SharpValue destination,
        SharpValue key1,
        SharpValue key2,
        double key1Weight,
        double key2Weight,
        Aggregate aggregate)
    {
        var command = new CommandPacket(SortedSetCommands._zunionstore, ZSet | WithWrite)
            .WriteKeys(destination)
            .WriteArguments("2")
            .WriteKeys(key1)
            .WriteKeys(key2)
            .WriteArguments("WEIGHTS")
            .WriteDouble(key1Weight)
            .WriteDouble(key2Weight);

        if (aggregate != Aggregate.Sum)
        {
            _ = command.WriteArguments("AGGREGATE");
            switch (aggregate)
            {
                case Aggregate.Min:
                    _ = command.WriteArguments("MIN");
                    break;
                case Aggregate.Max:
                    _ = command.WriteArguments("MAX");
                    break;
                default:
                    break;
            }
        }

        return command;
    }

    internal static CommandPacket ZUnionStore(
        SharpValue destination,
        SharpValueSpan keys,
        ReadOnlySpan<double> weights,
        Aggregate aggregate)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        var command = new CommandPacket(SortedSetCommands._zunionstore, ZSet | WithWrite)
            .WriteKeys(destination)
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys)
            .WriteArguments("WEIGHTS");

        if (weights.IsEmpty)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                _ = command.WriteArguments("1");
            }
        }
        else
        {
            if (weights.Length == keys.Count)
            {
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                }
            }
            else if (weights.Length > keys.Count)
            {
                int i = 0;
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                    i++;
                    if (i >= keys.Count) break;
                }
            }
            else
            {
                int i = 0;
                foreach (var item in weights)
                {
                    _ = command.WriteDouble(item);
                    i++;
                }
                for (; i < keys.Count; i++)
                {
                    _ = command.WriteArguments("1");
                }
            }
        }

        if (aggregate != Aggregate.Sum)
        {
            _ = command.WriteArguments("AGGREGATE");
            switch (aggregate)
            {
                case Aggregate.Min:
                    _ = command.WriteArguments("MIN");
                    break;
                case Aggregate.Max:
                    _ = command.WriteArguments("MAX");
                    break;
                default:
                    break;
            }
        }

        return command;
    }

    internal static CommandPacket BZMPop(double timeout, SharpValue key, PopHow how, int count)
    {
        var command = new CommandPacket(SortedSetCommands._bzmpop, ZSet | WithWrite | WithBlocking)
            .WriteArguments(timeout.ToBytes())
            .WriteArguments("1")
            .WriteKeys(key)
            .WriteArguments(how is PopHow.Min, "MIN")
            .WriteArguments(how is PopHow.Max, "MAX");
        if (count > 1)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.ToBytes());
        }
        return command;
    }

    internal static CommandPacket BZMPop(double timeout, SharpValueSpan keys, PopHow how, int count)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        var command = new CommandPacket(SortedSetCommands._bzmpop, ZSet | WithWrite | WithBlocking)
            .WriteArguments(timeout.ToBytes())
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys)
            .WriteArguments(how is PopHow.Min, "MIN")
            .WriteArguments(how is PopHow.Max, "MAX");
        if (count > 1)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.ToBytes());
        }
        return command;
    }

    internal static CommandPacket BZPopMax(int timeout, SharpValue key)
    {
        return new CommandPacket(SortedSetCommands._bzpopmax, ZSet | WithWrite | WithBlocking)
            .WriteKeys(key)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BZPopMax(double timeout, SharpValue key)
    {
        return new CommandPacket(SortedSetCommands._bzpopmax, ZSet | WithWrite | WithBlocking)
            .WriteKeys(key)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BZPopMax(int timeout, SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(SortedSetCommands._bzpopmax, ZSet | WithWrite | WithBlocking)
            .WriteKeys(keys)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BZPopMax(double timeout, SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(SortedSetCommands._bzpopmax, ZSet | WithWrite | WithBlocking)
            .WriteKeys(keys)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BZPopMin(int timeout, SharpValue key)
    {
        return new CommandPacket(SortedSetCommands._bzpopmin, ZSet | WithWrite | WithBlocking)
            .WriteKeys(key)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BZPopMin(double timeout, SharpValue key)
    {
        return new CommandPacket(SortedSetCommands._bzpopmin, ZSet | WithWrite | WithBlocking)
            .WriteKeys(key)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BZPopMin(int timeout, SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(SortedSetCommands._bzpopmin, ZSet | WithWrite | WithBlocking)
            .WriteKeys(keys)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket BZPopMin(double timeout, SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one Key.");
        return new CommandPacket(SortedSetCommands._bzpopmin, ZSet | WithWrite | WithBlocking)
            .WriteKeys(keys)
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket ZMove(SharpValue source, SharpValue destination, MoveHow how, double? scoreMin, double? scoreMax, long? count)
    {
        if (how == MoveHow.ByScore)
        {
            if (!scoreMin.HasValue || !scoreMax.HasValue)
            {
                throw new ArgumentException("In the BYSCORE mode, the maximum and minimum scores need to be specified.");
            }
        }
        if (count.HasValue)
        {
            if (count.Value <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        }
        var command = new CommandPacket(SortedSetCommands._zmove, ZSet | WithWrite)
            .WriteKeys(source)
            .WriteKeys(destination);
        switch (how)
        {
            case MoveHow.Min:
                _ = command.WriteArguments("MIN");
                break;
            case MoveHow.Max:
                _ = command.WriteArguments("MAX");
                break;
            case MoveHow.ByScore:
                _ = command
                    .WriteArguments("BYSCORE")
                    .WriteDouble(scoreMin!.Value)
                    .WriteDouble(scoreMax!.Value);
                break;
            default:
                break;
        }
        if (count.HasValue)
        {
            return command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket BZMove(SharpValue source, SharpValue destination, double timeout, MoveHow how, long? count)
    {
        if (how == MoveHow.ByScore)
        {
            throw new ArgumentException("Does not support the BYSCORE mode.", nameof(how));
        }
        if (count.HasValue)
        {
            if (count.Value <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        }
        var command = new CommandPacket(SortedSetCommands._bzmove, ZSet | WithWrite | WithBlocking)
            .WriteKeys(source)
            .WriteKeys(destination);
        switch (how)
        {
            case MoveHow.Min:
                _ = command.WriteArguments("MIN");
                break;
            case MoveHow.Max:
                _ = command.WriteArguments("MAX");
                break;
            default:
                break;
        }
        _ = command.WriteArguments(timeout.ToBytes());
        if (count.HasValue)
        {
            return command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ZMoveMember(SharpValue source, SharpValue destination, MoveWhen when, SharpValue member)
    {
        return SortedSetCommands.InitZMoveMember(source, destination, when)
            .WriteArguments("1")
            .WriteArguments(member);
    }

    internal static CommandPacket ZMoveMember(SharpValue source, SharpValue destination, MoveWhen when, SharpValueSpan members)
    {
        return SortedSetCommands.InitZMoveMember(source, destination, when)
            .WriteArguments(members.Length.ToBytes())
            .WriteArguments(members);
    }

    private static CommandPacket InitZMoveMember(SharpValue source, SharpValue destination, MoveWhen when)
    {
        return new CommandPacket(SortedSetCommands._zmovemember, ZSet | WithWrite)
            .WriteKeys(source)
            .WriteKeys(destination)
            .WriteArguments(when == MoveWhen.NX, "NX")
            .WriteArguments(when == MoveWhen.XX, "XX")
            .WriteArguments("MEMBERS");
    }
}
