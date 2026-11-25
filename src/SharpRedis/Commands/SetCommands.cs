using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using static SharpRedis.RedisSet;

namespace SharpRedis.Commands;

internal static class SetCommands
{
    private readonly static SharpValue _sadd = new("SADD", CommandItemCategory.Command);
    private readonly static SharpValue _scard = new("SCARD", CommandItemCategory.Command);
    private readonly static SharpValue _sdiff = new("SDIFF", CommandItemCategory.Command);
    private readonly static SharpValue _sdiffStore = new("SDIFFSTORE", CommandItemCategory.Command);
    private readonly static SharpValue _smembers = new("SMEMBERS", CommandItemCategory.Command);
    private readonly static SharpValue _sinter = new("SINTER", CommandItemCategory.Command);
    private readonly static SharpValue _sinterCard = new("SINTERCARD", CommandItemCategory.Command);
    private readonly static SharpValue _sinterStore = new("SINTERSTORE", CommandItemCategory.Command);
    private readonly static SharpValue _sismember = new("SISMEMBER", CommandItemCategory.Command);
    private readonly static SharpValue _smismember = new("SMISMEMBER", CommandItemCategory.Command);
    private readonly static SharpValue _smove = new("SMOVE", CommandItemCategory.Command);
    private readonly static SharpValue _spop = new("SPOP", CommandItemCategory.Command);
    private readonly static SharpValue _srandmember = new("SRANDMEMBER", CommandItemCategory.Command);
    private readonly static SharpValue _srem = new("SREM", CommandItemCategory.Command);
    private readonly static SharpValue _sunion = new("SUNION", CommandItemCategory.Command);
    private readonly static SharpValue _sunionStore = new("SUNIONSTORE", CommandItemCategory.Command);
    private readonly static SharpValue _sscan = new("SSCAN", CommandItemCategory.Command);
    private readonly static SharpValue _sdiffcard = new("SDIFFCARD", CommandItemCategory.Command);
    private readonly static SharpValue _sunioncard = new("SUNIONCARD", CommandItemCategory.Command);
    private readonly static SharpValue _saddex = new("SADDEX", CommandItemCategory.Command);
    private readonly static SharpValue _sexpire = new("SEXPIRE", CommandItemCategory.Command);
    private readonly static SharpValue _sexpireat = new("SEXPIREAT", CommandItemCategory.Command);
    private readonly static SharpValue _sexpiretime = new("SEXPIRETIME", CommandItemCategory.Command);
    private readonly static SharpValue _spersist = new("SPERSIST", CommandItemCategory.Command);
    private readonly static SharpValue _spexpire = new("SPEXPIRE", CommandItemCategory.Command);
    private readonly static SharpValue _spexpireat = new("SPEXPIREAT", CommandItemCategory.Command);
    private readonly static SharpValue _spexpiretime = new("SPEXPIRETIME", CommandItemCategory.Command);
    private readonly static SharpValue _sttl = new("STTL", CommandItemCategory.Command);
    private readonly static SharpValue _spttl = new("SPTTL", CommandItemCategory.Command);

    internal static CommandPacket SAdd(SharpValue key, SharpValue member)
        => new CommandPacket(SetCommands._sadd, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(member);

    internal static CommandPacket SAdd(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(SetCommands._sadd, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(members);
    }

    internal static CommandPacket SCard(SharpValue key)
        => new CommandPacket(SetCommands._scard, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);

    internal static CommandPacket SDiff(SharpValue key1, SharpValue key2)
        => new CommandPacket(SetCommands._sdiff, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key1)
            .WriteKeys(key2);

    internal static CommandPacket SDiff(SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        return new CommandPacket(SetCommands._sdiff, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(keys);
    }

    internal static CommandPacket SDiffStore(SharpValue key1, SharpValue key2, SharpValue destination)
       => new CommandPacket(SetCommands._sdiffStore, CommandMode.Set | CommandMode.WithWrite)
           .WriteKeys(destination)
           .WriteKeys(key1)
           .WriteKeys(key2);

    internal static CommandPacket SDiffStore(SharpValueSpan keys, SharpValue destination)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        return new CommandPacket(SetCommands._sdiffStore, CommandMode.Set | CommandMode.WithWrite)
           .WriteKeys(destination)
           .WriteKeys(keys);
    }

    internal static CommandPacket SMembers(SharpValue key)
        => new CommandPacket(SetCommands._smembers, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
        .WriteKeys(key);

    internal static CommandPacket SInter(SharpValue key1, SharpValue key2)
        => new CommandPacket(SetCommands._sinter, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key1)
            .WriteKeys(key2);

    internal static CommandPacket SInter(SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        return new CommandPacket(SetCommands._sinter, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(keys);
    }

    internal static CommandPacket SInterCard(SharpValue key1, SharpValue key2, ulong limit)
    {
        var command = new CommandPacket(SetCommands._sinterCard, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
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

    internal static CommandPacket SInterCard(SharpValueSpan keys, ulong limit)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        var command = new CommandPacket(SetCommands._sinterCard, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
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

    internal static CommandPacket SInterStore(SharpValue key1, SharpValue key2, SharpValue destination)
        => new CommandPacket(SetCommands._sinterStore, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(destination)
            .WriteKeys(key1)
            .WriteKeys(key2);

    internal static CommandPacket SInterStore(SharpValueSpan keys, SharpValue destination)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        return new CommandPacket(SetCommands._sinterStore, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(destination)
            .WriteKeys(keys);
    }

    internal static CommandPacket SIsMember(SharpValue key, SharpValue member)
        => new CommandPacket(SetCommands._sismember, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(member);

    internal static CommandPacket SMIsMember(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(SetCommands._smismember, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(members);
    }

    internal static CommandPacket SMove(SharpValue source, SharpValue destination, SharpValue member)
        => new CommandPacket(SetCommands._smove, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(source)
            .WriteKeys(destination)
            .WriteArguments(member);

    internal static CommandPacket SPop(SharpValue key, ulong? count)
    {
        var command = new CommandPacket(SetCommands._spop, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key);
        if (count.HasValue)
        {
            _ = command
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket SRandMember(SharpValue key, long? count)
    {
        var command = new CommandPacket(SetCommands._srandmember, CommandMode.Set | CommandMode.WithRead)
            .WriteKeys(key);
        if (count.HasValue)
        {
            _ = command
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket SRem(SharpValue key, SharpValue member)
        => new CommandPacket(SetCommands._srem, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(member);

    internal static CommandPacket SRem(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(SetCommands._srem, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(members);
    }

    internal static CommandPacket SUnion(SharpValue key1, SharpValue key2)
        => new CommandPacket(SetCommands._sunion, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key1)
            .WriteKeys(key2);

    internal static CommandPacket SUnion(SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        return new CommandPacket(SetCommands._sunion, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(keys);
    }

    internal static CommandPacket SUnionStore(SharpValue key1, SharpValue key2, SharpValue destination)
        => new CommandPacket(SetCommands._sunionStore, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(destination)
            .WriteKeys(key1)
            .WriteKeys(key2);

    internal static CommandPacket SUnionStore(SharpValueSpan keys, SharpValue destination)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        return new CommandPacket(SetCommands._sunionStore, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(destination)
            .WriteKeys(keys);
    }

    internal static CommandPacket SScan(SharpValue key, ulong cursor, SharpValue? pattern, uint count)
    {
        var command = new CommandPacket(SetCommands._sscan, CommandMode.Set | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments(cursor.ToBytes());
        if (pattern.HasValue)
        {
            _ = command.WriteArguments("MATCH")
                .WriteArguments(pattern.Value);
        }
        return command
            .WriteArguments("COUNT")
            .WriteArguments(count.ToBytes());
    }

    internal static CommandPacket SDiffCard(SharpValue key1, SharpValue key2, ulong limit)
    {
        var command = new CommandPacket(SetCommands._sdiffcard, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
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

    internal static CommandPacket SDiffCard(SharpValueSpan keys, ulong limit)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        var command = new CommandPacket(SetCommands._sdiffcard, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
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

    internal static CommandPacket SUnionCard(SharpValue key1, SharpValue key2, bool approx, ulong limit)
    {
        var command = new CommandPacket(SetCommands._sunioncard, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteArguments("2")
            .WriteKeys(key1)
            .WriteKeys(key2)
            .WriteArguments(approx, "APPROX");
        if (limit > 0)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(limit.ToBytes());
        }
        return command;
    }

    internal static CommandPacket SUnionCard(SharpValueSpan keys, bool approx, ulong limit)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        var command = new CommandPacket(SetCommands._sunioncard, CommandMode.Set | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteArguments(keys.Count.ToBytes())
            .WriteKeys(keys)
            .WriteArguments(approx, "APPROX");
        if (limit > 0)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(limit.ToBytes());
        }
        return command;
    }

    internal static CommandPacket SAddEx(
        SharpValue key,
        AddExWhen when,
        SharpExpiry? expiry,
        ExpirationFlags expiryFlags,
        SharpValue member)
    {
        return SetCommands.InitSAddEx(key, when, expiry, expiryFlags)
            .WriteArguments("1")
            .WriteArguments(member);
    }

    internal static CommandPacket SAddEx(
        SharpValue key,
        AddExWhen when,
        SharpExpiry? expiry,
        ExpirationFlags expiryFlags,
        SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return SetCommands.InitSAddEx(key, when, expiry, expiryFlags)
            .WriteArguments(members.Length.ToBytes())
            .WriteArguments(members);
    }

    private static CommandPacket InitSAddEx(
        SharpValue key,
        AddExWhen when,
        SharpExpiry? expiry,
        ExpirationFlags expiryFlags)
    {
        if (expiryFlags == ExpirationFlags.PERSIST) throw new ArgumentException("The expiry of SADDEX does not support PERSIST.", nameof(expiryFlags));
        var command = new CommandPacket(SetCommands._saddex, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key);
        switch (when)
        {
            case AddExWhen.MNX:
                _ = command.WriteArguments("MNX");
                break;
            case AddExWhen.MXX:
                _ = command.WriteArguments("MXX");
                break;
            default:
                break;
        }
        return command
            .WriteExpiry(expiry, expiryFlags)
            .WriteArguments("MEMBERS");
    }

    internal static CommandPacket SExpire(SharpValue key, long seconds, ExpirationWhen when, SharpValue member)
    {
        return SetCommands.InitSExpire(key, seconds, when)
            .WriteArguments("1")
            .WriteArguments(member);
    }

    internal static CommandPacket SExpire(SharpValue key, long seconds, ExpirationWhen when, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return SetCommands.InitSExpire(key, seconds, when)
            .WriteArguments(members.Length.ToBytes())
            .WriteArguments(members);
    }

    private static CommandPacket InitSExpire(SharpValue key, long seconds, ExpirationWhen when)
    {
        var command = new CommandPacket(SetCommands._sexpire, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(seconds.ToBytes());
        switch (when)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments("NX");
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments("XX");
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments("GT");
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments("LT");
                break;
            default:
                break;
        }
        return command.WriteArguments("MEMBERS");
    }

    internal static CommandPacket SExpireAt(SharpValue key, long timestamp, ExpirationWhen when, SharpValue member)
    {
        return SetCommands.InitSExpireAt(key, timestamp, when)
            .WriteArguments("1")
            .WriteArguments(member);
    }

    internal static CommandPacket SExpireAt(SharpValue key, long timestamp, ExpirationWhen when, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return SetCommands.InitSExpireAt(key, timestamp, when)
            .WriteArguments(members.Length.ToBytes())
            .WriteArguments(members);
    }

    private static CommandPacket InitSExpireAt(SharpValue key, long timestamp, ExpirationWhen when)
    {
        var command = new CommandPacket(SetCommands._sexpireat, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(timestamp.ToBytes());
        switch (when)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments("NX");
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments("XX");
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments("GT");
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments("LT");
                break;
            default:
                break;
        }
        return command.WriteArguments("MEMBERS");
    }

    internal static CommandPacket SExpireTime(SharpValue key, SharpValue member)
    {
        return new CommandPacket(SetCommands._sexpiretime, CommandMode.Set | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments("MEMBERS")
            .WriteArguments("1")
            .WriteArguments(member);
    }

    internal static CommandPacket SExpireTime(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(SetCommands._sexpiretime, CommandMode.Set | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments("MEMBERS")
            .WriteArguments(members.Length.ToBytes())
            .WriteArguments(members);
    }

    internal static CommandPacket SPersist(SharpValue key, SharpValue member)
    {
        return new CommandPacket(SetCommands._spersist, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments("MEMBERS")
            .WriteArguments("1")
            .WriteArguments(member);
    }

    internal static CommandPacket SPersist(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(SetCommands._spersist, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments("MEMBERS")
            .WriteArguments(members.Length.ToBytes())
            .WriteArguments(members);
    }

    internal static CommandPacket SPExpire(SharpValue key, long milliseconds, ExpirationWhen when, SharpValue member)
    {
        return SetCommands.InitSPExpire(key, milliseconds, when)
            .WriteArguments("1")
            .WriteArguments(member);
    }

    internal static CommandPacket SPExpire(SharpValue key, long milliseconds, ExpirationWhen when, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return SetCommands.InitSPExpire(key, milliseconds, when)
            .WriteArguments(members.Length.ToBytes())
            .WriteArguments(members);
    }

    private static CommandPacket InitSPExpire(SharpValue key, long milliseconds, ExpirationWhen when)
    {
        var command = new CommandPacket(SetCommands._spexpire, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(milliseconds.ToBytes());
        switch (when)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments("NX");
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments("XX");
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments("GT");
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments("LT");
                break;
            default:
                break;
        }
        return command.WriteArguments("MEMBERS");
    }

    internal static CommandPacket SPExpireAt(SharpValue key, long timestamp, ExpirationWhen when, SharpValue member)
    {
        return SetCommands.InitSPExpireAt(key, timestamp, when)
            .WriteArguments("1")
            .WriteArguments(member);
    }

    internal static CommandPacket SPExpireAt(SharpValue key, long timestamp, ExpirationWhen when, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return SetCommands.InitSPExpireAt(key, timestamp, when)
            .WriteArguments(members.Length.ToBytes())
            .WriteArguments(members);
    }

    private static CommandPacket InitSPExpireAt(SharpValue key, long timestamp, ExpirationWhen when)
    {
        var command = new CommandPacket(SetCommands._spexpireat, CommandMode.Set | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(timestamp.ToBytes());
        switch (when)
        {
            case ExpirationWhen.NX:
                _ = command.WriteArguments("NX");
                break;
            case ExpirationWhen.XX:
                _ = command.WriteArguments("XX");
                break;
            case ExpirationWhen.GT:
                _ = command.WriteArguments("GT");
                break;
            case ExpirationWhen.LT:
                _ = command.WriteArguments("LT");
                break;
            default:
                break;
        }
        return command.WriteArguments("MEMBERS");
    }

    internal static CommandPacket SPExpireTime(SharpValue key, SharpValue member)
    {
        return new CommandPacket(SetCommands._spexpiretime, CommandMode.Set | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments("MEMBERS")
            .WriteArguments("1")
            .WriteArguments(member);
    }

    internal static CommandPacket SPExpireTime(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(SetCommands._spexpiretime, CommandMode.Set | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments("MEMBERS")
            .WriteArguments(members.Length.ToBytes())
            .WriteArguments(members);
    }

    internal static CommandPacket STtl(SharpValue key, SharpValue member)
    {
        return new CommandPacket(SetCommands._sttl, CommandMode.Set | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments("MEMBERS")
            .WriteArguments("1")
            .WriteArguments(member);
    }

    internal static CommandPacket STtl(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(SetCommands._sttl, CommandMode.Set | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments("MEMBERS")
            .WriteArguments(members.Length.ToBytes())
            .WriteArguments(members);
    }

    internal static CommandPacket SPTtl(SharpValue key, SharpValue member)
    {
        return new CommandPacket(SetCommands._spttl, CommandMode.Set | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments("MEMBERS")
            .WriteArguments("1")
            .WriteArguments(member);
    }

    internal static CommandPacket SPTtl(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(SetCommands._spttl, CommandMode.Set | CommandMode.WithRead)
            .WriteKeys(key)
            .WriteArguments("MEMBERS")
            .WriteArguments(members.Length.ToBytes())
            .WriteArguments(members);
    }
}
