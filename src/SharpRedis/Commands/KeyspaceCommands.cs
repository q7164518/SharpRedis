using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using static SharpRedis.RedisKeyspace;

namespace SharpRedis.Commands;

internal class KeyspaceCommands
{
    private readonly static SharpValue _exists = new("EXISTS", CommandItemCategory.Command);
    private readonly static SharpValue _ttl = new("TTL", CommandItemCategory.Command);
    private readonly static SharpValue _pttl = new("PTTL", CommandItemCategory.Command);
    private readonly static SharpValue _copy = new("COPY", CommandItemCategory.Command);
    private readonly static SharpValue _del = new("DEL", CommandItemCategory.Command);
    private readonly static SharpValue _dump = new("DUMP", CommandItemCategory.Command);
    private readonly static SharpValue _restore = new("RESTORE", CommandItemCategory.Command);
    private readonly static SharpValue _expire = new("EXPIRE", CommandItemCategory.Command);
    private readonly static SharpValue _pexpire = new("PEXPIRE", CommandItemCategory.Command);
    private readonly static SharpValue _expireat = new("EXPIREAT", CommandItemCategory.Command);
    private readonly static SharpValue _expiretime = new("EXPIRETIME", CommandItemCategory.Command);
    private readonly static SharpValue _pexpiretime = new("PEXPIRETIME", CommandItemCategory.Command);
    private readonly static SharpValue _pexpireat = new("PEXPIREAT", CommandItemCategory.Command);
    private readonly static SharpValue _keys = new("KEYS", CommandItemCategory.Command);
    private readonly static SharpValue _migrate = new("MIGRATE", CommandItemCategory.Command);
    private readonly static SharpValue _move = new("MOVE", CommandItemCategory.Command);
    private readonly static SharpValue _persist = new("PERSIST", CommandItemCategory.Command);
    private readonly static SharpValue _randomkey = new("RANDOMKEY", CommandItemCategory.Command);
    private readonly static SharpValue _rename = new("RENAME", CommandItemCategory.Command);
    private readonly static SharpValue _renamenx = new("RENAMENX", CommandItemCategory.Command);
    private readonly static SharpValue _type = new("TYPE", CommandItemCategory.Command);
    private readonly static SharpValue _unlink = new("UNLINK", CommandItemCategory.Command);
    private readonly static SharpValue _wait = new("WAIT", CommandItemCategory.Command);
    private readonly static SharpValue _waitaof = new("WAITAOF", CommandItemCategory.Command);
    private readonly static SharpValue _touch = new("TOUCH", CommandItemCategory.Command);
    private readonly static SharpValue _sort = new("SORT", CommandItemCategory.Command);
    private readonly static SharpValue _sort_ro = new("SORT_RO", CommandItemCategory.Command);
    private readonly static SharpValue _scan = new("SCAN", CommandItemCategory.Command);

    private readonly static SharpValue _object = new("OBJECT", CommandItemCategory.Command);
    private readonly static SharpValue _encoding = new("ENCODING", CommandItemCategory.Command);
    private readonly static SharpValue _freq = new("FREQ", CommandItemCategory.Command);
    private readonly static SharpValue _idletime = new("IDLETIME", CommandItemCategory.Command);
    private readonly static SharpValue _refcount = new("REFCOUNT", CommandItemCategory.Command);

    private readonly static SharpValue _bless = new("BLESS", CommandItemCategory.Command);
    private readonly static SharpValue _set = new("SET", CommandItemCategory.Command);
    private readonly static SharpValue _get = new("GET", CommandItemCategory.Command);
    private readonly static SharpValue _clear = new("CLEAR", CommandItemCategory.Command);

    internal static CommandPacket BlessSet(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._bless, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteCommand(KeyspaceCommands._set)
            .WriteKeys(key)
            .WriteCommand("NO-EVICT");
    }

    internal static CommandPacket BlessGet(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._bless, CommandMode.Keyspace)
            .WriteCommand(KeyspaceCommands._get)
            .WriteKeys(key);
    }

    internal static CommandPacket BlessClear(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._bless, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteCommand(KeyspaceCommands._clear)
            .WriteKeys(key)
            .WriteCommand("NO-EVICT");
    }

    internal static CommandPacket BlessScan(long cursor, long? count)
    {
        var command = new CommandPacket(KeyspaceCommands._bless, CommandMode.Keyspace)
            .WriteCommand(KeyspaceCommands._scan)
            .WriteArguments(cursor.ToBytes())
            .WriteArguments("NO-EVICT");
        if (count.HasValue)
        {
            return command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket Ttl(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._ttl, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteKeys(key);
    }

    internal static CommandPacket PTtl(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._pttl, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteKeys(key);
    }

    internal static CommandPacket Exists(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._exists, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteKeys(key);
    }

    internal static CommandPacket Exists(SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        return new CommandPacket(KeyspaceCommands._exists, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteKeys(keys);
    }

    internal static CommandPacket Copy(SharpValue source, SharpValue destination, int? dbIndex, bool replace)
    {
        var command = new CommandPacket(KeyspaceCommands._copy, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(source)
            .WriteKeys(destination);
        if (dbIndex.HasValue)
        {
            _ = command
                .WriteArguments("DB")
                .WriteArguments(dbIndex.Value.ToBytes());
        }
        return command.WriteArguments(replace, "REPLACE");
    }

    internal static CommandPacket Del(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._del, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key);
    }

    internal static CommandPacket Del(SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        return new CommandPacket(KeyspaceCommands._del, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(keys);
    }

    internal static CommandPacket Dump(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._dump, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteKeys(key);
    }

    internal static CommandPacket Restore(SharpValue key, SharpExpiry ttl, byte[] serializedValue, bool replace, ulong? idletime, ulong? frequency)
    {
        var command = new CommandPacket(KeyspaceCommands._restore, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key);
        bool absttl = false;
        if (ttl == default)
        {
            _ = command.WriteArguments(0.ToBytes());
        }
        else
        {
            switch (ttl.ValueKind)
            {
                case ExpirationValueKind.DateTime:
                    absttl = true;
                    _ = command.WriteArguments(ttl.GetUnixTimeMilliseconds.ToBytes());
                    break;
                case ExpirationValueKind.TimeSpan:
                case ExpirationValueKind.Number:
                    _ = command.WriteArguments(ttl.GetMilliseconds.ToBytes());
                    break;
                default:
                    _ = command.WriteArguments(0.ToBytes());
                    break;
            }
        }

        _ = command
            .WriteArguments(serializedValue)
            .WriteArguments(replace, "REPLACE")
            .WriteArguments(absttl, "ABSTTL");

        if (idletime.HasValue && frequency.HasValue)
        {
            throw new System.NotSupportedException("IDLETIME and FREQ can only be selected one at a time.");
        }

        if (idletime.HasValue)
        {
            _ = command
                .WriteArguments("IDLETIME")
                .WriteArguments(idletime.Value.ToBytes());
        }

        if (frequency.HasValue)
        {
            _ = command
                .WriteArguments("FREQ")
                .WriteArguments(frequency.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket Expire(SharpValue key, long seconds, ExpireCondition condition)
    {
        var command = new CommandPacket(KeyspaceCommands._expire, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(seconds.ToBytes());

        return condition switch
        {
            ExpireCondition.NX => command.WriteArguments(nameof(ExpireCondition.NX)),
            ExpireCondition.XX => command.WriteArguments(nameof(ExpireCondition.XX)),
            ExpireCondition.GT => command.WriteArguments(nameof(ExpireCondition.GT)),
            ExpireCondition.LT => command.WriteArguments(nameof(ExpireCondition.LT)),
            _ => command,
        };
    }

    internal static CommandPacket PExpire(SharpValue key, long milliseconds, ExpireCondition condition)
    {
        var command = new CommandPacket(KeyspaceCommands._pexpire, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(milliseconds.ToBytes());

        return condition switch
        {
            ExpireCondition.NX => command.WriteArguments(nameof(ExpireCondition.NX)),
            ExpireCondition.XX => command.WriteArguments(nameof(ExpireCondition.XX)),
            ExpireCondition.GT => command.WriteArguments(nameof(ExpireCondition.GT)),
            ExpireCondition.LT => command.WriteArguments(nameof(ExpireCondition.LT)),
            _ => command,
        };
    }

    internal static CommandPacket ExpireAt(SharpValue key, long unixTimeSeconds, ExpireCondition condition)
    {
        var command = new CommandPacket(KeyspaceCommands._expireat, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(unixTimeSeconds.ToBytes());

        return condition switch
        {
            ExpireCondition.NX => command.WriteArguments(nameof(ExpireCondition.NX)),
            ExpireCondition.XX => command.WriteArguments(nameof(ExpireCondition.XX)),
            ExpireCondition.GT => command.WriteArguments(nameof(ExpireCondition.GT)),
            ExpireCondition.LT => command.WriteArguments(nameof(ExpireCondition.LT)),
            _ => command,
        };
    }

    internal static CommandPacket PExpireAt(SharpValue key, long unixTimeMilliseconds, ExpireCondition condition)
    {
        var command = new CommandPacket(KeyspaceCommands._pexpireat, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(unixTimeMilliseconds.ToBytes());

        return condition switch
        {
            ExpireCondition.NX => command.WriteArguments(nameof(ExpireCondition.NX)),
            ExpireCondition.XX => command.WriteArguments(nameof(ExpireCondition.XX)),
            ExpireCondition.GT => command.WriteArguments(nameof(ExpireCondition.GT)),
            ExpireCondition.LT => command.WriteArguments(nameof(ExpireCondition.LT)),
            _ => command,
        };
    }

    internal static CommandPacket ExpireTime(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._expiretime, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteKeys(key);
    }

    internal static CommandPacket PExpireTime(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._pexpiretime, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteKeys(key);
    }

    internal static CommandPacket Keys(SharpValue pattern)
    {
        return new CommandPacket(KeyspaceCommands._keys, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteArguments(pattern);
    }

    internal static CommandPacket Migrate(
        string host,
        ushort port,
        SharpValue key,
        int destinationDb,
        long timeout,
        bool copy,
        bool replace,
#if MODERN_NET
        string? password,
        string? username,
#else
        string password,
        string username,
#endif
        SharpValueSpan keys)
    {
        if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
        {
            throw new System.NotSupportedException("If a authenticate username is specified, a authenticate password must also be specified.");
        }

        var command = new CommandPacket(KeyspaceCommands._migrate, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteArguments(host)
            .WriteArguments(port.ToBytes());
        if (key == default)
        {
            _ = command.WriteArguments(string.Empty);
        }
        else
        {
            _ = command.WriteKeys(key);
        }
        _ = command
            .WriteArguments(destinationDb.ToBytes())
            .WriteArguments(timeout.ToBytes())
            .WriteArguments(copy, "COPY")
            .WriteArguments(replace, "REPLACE");

        if (!string.IsNullOrEmpty(password))
        {
            if (!string.IsNullOrEmpty(username))
            {
                _ = command
                    .WriteArguments("AUTH2")
                    .WriteArguments(username)
                    .WriteArguments(password);
            }
            else
            {
                _ = command
                    .WriteArguments("AUTH")
                    .WriteArguments(password);
            }
        }

        if (!keys.IsEmpty)
        {
            _ = command
                .WriteArguments("KEYS")
                .WriteKeys(keys);
        }
        return command;
    }

    internal static CommandPacket Move(SharpValue key, int db)
    {
        return new CommandPacket(KeyspaceCommands._move, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(db.ToBytes());
    }

    internal static CommandPacket Persist(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._persist, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key);
    }

    internal static CommandPacket RandomKey()
    {
        return new CommandPacket(KeyspaceCommands._randomkey, CommandMode.Keyspace | CommandMode.WithRead);
    }

    internal static CommandPacket Rename(SharpValue key, SharpValue newKey)
    {
        return new CommandPacket(KeyspaceCommands._rename, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteKeys(newKey);
    }

    internal static CommandPacket RenameNX(SharpValue key, SharpValue newKey)
    {
        return new CommandPacket(KeyspaceCommands._renamenx, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteKeys(newKey);
    }

    internal static CommandPacket ObjectEncoding(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._object, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteCommand(KeyspaceCommands._encoding)
            .WriteKeys(key);
    }

    internal static CommandPacket ObjectFreq(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._object, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteCommand(KeyspaceCommands._freq)
            .WriteKeys(key);
    }

    internal static CommandPacket ObjectIdleTime(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._object, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteCommand(KeyspaceCommands._idletime)
            .WriteKeys(key);
    }

    internal static CommandPacket ObjectRefCount(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._object, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteCommand(KeyspaceCommands._refcount)
            .WriteKeys(key);
    }

    internal static CommandPacket Type(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._type, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteKeys(key);
    }

    internal static CommandPacket Unlink(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._unlink, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key);
    }

    internal static CommandPacket Unlink(SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        return new CommandPacket(KeyspaceCommands._unlink, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(keys);
    }

    internal static CommandPacket Wait(int numreplicas, long timeout)
    {
        return new CommandPacket(KeyspaceCommands._wait, CommandMode.Keyspace)
            .WriteArguments(numreplicas.ToBytes())
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket WaitAof(bool numlocal, int numreplicas, long timeout)
    {
        return new CommandPacket(KeyspaceCommands._waitaof, CommandMode.Keyspace)
            .WriteArguments(numlocal, "1")
            .WriteArguments(!numlocal, "0")
            .WriteArguments(numreplicas.ToBytes())
            .WriteArguments(timeout.ToBytes());
    }

    internal static CommandPacket Touch(SharpValue key)
    {
        return new CommandPacket(KeyspaceCommands._touch, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteKeys(key);
    }

    internal static CommandPacket Touch(SharpValueSpan keys)
    {
        if (keys.IsEmpty) throw new NotSupportedException("Please specify at least one key.");
        return new CommandPacket(KeyspaceCommands._touch, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteKeys(keys);
    }

    internal static CommandPacket Sort(
        SharpValue key,
        SharpValue pattern,
        long? offset,
        long? count,
        SharpValueSpan get,
        SortDirection sort,
        bool alpha,
        SharpValue store
        )
    {
        if ((offset.HasValue && !count.HasValue) || (!offset.HasValue && count.HasValue))
        {
            throw new ArgumentException("Both \"offset\" and \"count\" must be specified simultaneously.");
        }
        var command = new CommandPacket(KeyspaceCommands._sort, CommandMode.Keyspace | CommandMode.WithWrite)
            .WriteKeys(key);
        if (pattern.HasValue)
        {
            _ = command
               .WriteArguments("BY")
               .WriteArguments(pattern);
        }
        if (offset.HasValue && count.HasValue)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(offset.Value.ToBytes())
                .WriteArguments(count.Value.ToBytes());
        }
        if (!get.IsEmpty)
        {
            foreach (var item in get)
            {
                _ = command
                    .WriteArguments("GET")
                    .WriteArguments(item);
            }
        }
        switch (sort)
        {
            case SortDirection.Asc:
                _ = command.WriteArguments("ASC");
                break;
            case SortDirection.Desc:
                _ = command.WriteArguments("DESC");
                break;
            default:
                break;
        }
        if (alpha)
        {
            _ = command.WriteArguments("ALPHA");
        }
        if (store.HasValue)
        {
            _ = command
                .WriteArguments("STORE")
                .WriteKeys(store);
        }
        return command;
    }

    internal static CommandPacket Sort_Ro(
        SharpValue key,
        SharpValue pattern,
        long? offset,
        long? count,
        SharpValueSpan get,
        SortDirection sort,
        bool alpha
        )
    {
        if ((offset.HasValue && !count.HasValue) || (!offset.HasValue && count.HasValue))
        {
            throw new ArgumentException("Both \"offset\" and \"count\" must be specified simultaneously.");
        }
        var command = new CommandPacket(KeyspaceCommands._sort_ro, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteKeys(key);
        if (pattern.HasValue)
        {
            _ = command
               .WriteArguments("BY")
               .WriteArguments(pattern);
        }
        if (offset.HasValue && count.HasValue)
        {
            _ = command
                .WriteArguments("LIMIT")
                .WriteArguments(offset.Value.ToBytes())
                .WriteArguments(count.Value.ToBytes());
        }
        if (!get.IsEmpty)
        {
            foreach (var item in get)
            {
                _ = command
                    .WriteArguments("GET")
                    .WriteArguments(item);
            }
        }
        switch (sort)
        {
            case SortDirection.Asc:
                _ = command.WriteArguments("ASC");
                break;
            case SortDirection.Desc:
                _ = command.WriteArguments("DESC");
                break;
            default:
                break;
        }
        return command.WriteArguments(alpha, "ALPHA");
    }

    internal static CommandPacket Scan(
        long cursor,
        SharpValue pattern,
        long? count,
#if MODERN_NET
        string? type
#else
        string type
#endif
    )
    {
        var command = new CommandPacket(KeyspaceCommands._scan, CommandMode.Keyspace | CommandMode.WithRead)
            .WriteArguments(cursor.ToBytes());
        if (pattern.HasValue)
        {
            _ = command
                .WriteArguments("MATCH")
                .WriteArguments(pattern);
        }
        if (count.HasValue)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes());
        }
        if (!string.IsNullOrEmpty(type))
        {
            _ = command
                .WriteArguments("TYPE")
                .WriteArguments(type);
        }
        return command;
    }
}
