using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;

namespace SharpRedis.Commands;

internal static class ConnectionCommands
{
    private readonly static SharpValue _trackingInfo = new("TRACKINGINFO", CommandItemCategory.Command);
    private readonly static SharpValue _unblock = new("UNBLOCK", CommandItemCategory.Command);
    private readonly static SharpValue _reply = new("REPLY", CommandItemCategory.Command);
    private readonly static SharpValue _pause = new("PAUSE", CommandItemCategory.Command);
    private readonly static SharpValue _auth = new("AUTH", CommandItemCategory.Command);
    private readonly static SharpValue _hello = new("HELLO", CommandItemCategory.Command);
    private readonly static SharpValue _echo = new("ECHO", CommandItemCategory.Command);
    private readonly static SharpValue _select = new("SELECT", CommandItemCategory.Command);
    private readonly static SharpValue _ping = new("PING", CommandItemCategory.Command);
    private readonly static SharpValue _client = new("CLIENT", CommandItemCategory.Command);
    private readonly static SharpValue _setName = new("SETNAME", CommandItemCategory.Command);
    private readonly static SharpValue _getName = new("GETNAME", CommandItemCategory.Command);
    private readonly static SharpValue _id = new("ID", CommandItemCategory.Command);
    private readonly static SharpValue _tracking = new("TRACKING", CommandItemCategory.Command);
    private readonly static SharpValue _caching = new("CACHING", CommandItemCategory.Command);
    private readonly static SharpValue _getRedir = new("GETREDIR", CommandItemCategory.Command);
    private readonly static SharpValue _setInfo = new("SETINFO", CommandItemCategory.Command);
    private readonly static SharpValue _info = new("INFO", CommandItemCategory.Command);
    private readonly static SharpValue _list = new("LIST", CommandItemCategory.Command);
    private readonly static SharpValue _kill = new("KILL", CommandItemCategory.Command);
    private readonly static SharpValue _noevict = new("NO-EVICT", CommandItemCategory.Command);
    private readonly static SharpValue _notouch = new("NO-TOUCH", CommandItemCategory.Command);
    private readonly static SharpValue _unpause = new("UNPAUSE", CommandItemCategory.Command);
    private readonly static SharpValue _quit = new("QUIT", CommandItemCategory.Command);
    private readonly static SharpValue _reset = new("RESET", CommandItemCategory.Command);

#if MODERN_NET
    internal static CommandPacket Auth(string? user, string? password)
#else
    internal static CommandPacket Auth(string user, string password)
#endif
    {
        var command = new CommandPacket(ConnectionCommands._auth, CommandMode.ConnectionManagement | CommandMode.DoNotReuseConnection);
        if (!string.IsNullOrEmpty(user))
        {
            _ = command.WriteArguments(user);
        }
        if (!string.IsNullOrEmpty(password))
        {
            _ = command.WriteArguments(password);
        }
        return command;
    }

    internal static CommandPacket Hello()
        => new(ConnectionCommands._hello, CommandMode.ConnectionManagement);

#if MODERN_NET
    internal static CommandPacket Hello(Resp protover, string? user, string? password, string? clientname)
#else
    internal static CommandPacket Hello(Resp protover, string user, string password, string clientname)
#endif
    {
        var command = new CommandPacket(ConnectionCommands._hello, CommandMode.ConnectionManagement | CommandMode.DoNotReuseConnection);
        _ = protover switch
        {
            Resp.RESP3 => command.WriteArguments("3"),
            _ => command.WriteArguments("2"),
        };

        if (!string.IsNullOrEmpty(user) || !string.IsNullOrEmpty(password))
        {
            _ = command.WriteCommand(ConnectionCommands._auth);
            if (!string.IsNullOrEmpty(user))
            {
                _ = command.WriteArguments(user);
            }
            else
            {
                _ = command.WriteArguments(Config.DEFAULT_USER);
            }

            if (!string.IsNullOrEmpty(password))
            {
                _ = command.WriteArguments(password);
            }
        }

        if (!string.IsNullOrEmpty(clientname))
        {
            _ = command.WriteCommand(ConnectionCommands._setName)
                .WriteArguments(clientname);
        }
        return command;
    }

    internal static CommandPacket Echo(string message)
    {
        if (string.IsNullOrEmpty(message)) throw new ArgumentException("The response message cannot be empty.", nameof(message));
        return new CommandPacket(ConnectionCommands._echo, CommandMode.ConnectionManagement)
            .WriteArguments(message);
    }

    internal static CommandPacket Select(int index)
    {
        return new CommandPacket(ConnectionCommands._select, CommandMode.ConnectionManagement | CommandMode.DoNotReuseConnection)
            .WriteArguments(index.ToBytes());
    }

    internal static CommandPacket ClientSetName(SharpValue clientname)
    {
        return new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement | CommandMode.DoNotReuseConnection)
            .WriteCommand(ConnectionCommands._setName)
            .WriteArguments(clientname);
    }

    internal static CommandPacket ClientGetName()
        => new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._getName);

#if MODERN_NET
    internal static CommandPacket Ping(string? message)
#else
    internal static CommandPacket Ping(string message)
#endif
    {
        var command = new CommandPacket(ConnectionCommands._ping, CommandMode.ConnectionManagement | CommandMode.WithoutActiveTime);
        if (string.IsNullOrEmpty(message))
        {
            return command;
        }
        return command.WriteArguments(message);
    }

    internal static CommandPacket ClientId()
        => new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._id);

#if MODERN_NET
    internal static CommandPacket ClientTracking(bool onOff, long? redirect, string[]? keyPrefixes, bool bcast, bool optin, bool optout, bool noloop)
#else
    internal static CommandPacket ClientTracking(bool onOff, long? redirect, string[] keyPrefixes, bool bcast, bool optin, bool optout, bool noloop)
#endif
    {
        var command = new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement | CommandMode.DoNotReuseConnection)
            .WriteCommand(ConnectionCommands._tracking)
            .WriteArguments(onOff, "ON")
            .WriteArguments(!onOff, "OFF");

        if (redirect.HasValue)
        {
            _ = command.WriteArguments("REDIRECT")
                .WriteArguments(redirect.Value.ToBytes());
        }

        if (keyPrefixes is { Length: > 0 })
        {
            foreach (var item in keyPrefixes)
            {
                _ = command.WriteArguments("PREFIX")
                    .WriteArguments(item);
            }
        }

        return command
            .WriteArguments(bcast, "BCAST")
            .WriteArguments(optin, "OPTIN")
            .WriteArguments(optout, "OPTOUT")
            .WriteArguments(noloop, "NOLOOP");
    }

    internal static CommandPacket ClientCaching(bool yesNo)
        => new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement | CommandMode.DoNotReuseConnection)
            .WriteCommand(ConnectionCommands._caching)
            .WriteArguments(yesNo, "YES")
            .WriteArguments(!yesNo, "NO");

    internal static CommandPacket ClientGetRedir()
        => new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._getRedir);

    internal static CommandPacket ClientSetInfo(RedisConnection.SetInfoCategory category, string value)
    {
        if (string.IsNullOrEmpty(value)) throw new ArgumentException("The value of the parameter cannot be left blank.");
        return new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement | CommandMode.DoNotReuseConnection)
            .WriteCommand(ConnectionCommands._setInfo)
            .WriteArguments(category is RedisConnection.SetInfoCategory.LibName, "LIB-NAME")
            .WriteArguments(category is RedisConnection.SetInfoCategory.LibVersion, "LIB-VER")
            .WriteArguments(value);
    }

    internal static CommandPacket ClientInfo()
        => new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._info);

    internal static CommandPacket ClientKill(string address)
    {
        if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("A client address is required.", nameof(address));
        return new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._kill).WriteArguments(address);
    }

    internal static CommandPacket ClientKill(RedisConnection.KillFilter filter)
    {
        if (filter.Id is null && filter.Type is null && filter.User is null && filter.Address is null
            && filter.LocalAddress is null && filter.SkipMe is null && filter.MaxAge is null)
            throw new ArgumentException("Specify at least one CLIENT KILL filter.", nameof(filter));

        var type = filter.Type.HasValue ? ClientTypeName(filter.Type.Value) : null;
        var command = new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._kill);
        if (filter.Id is long id) _ = command.WriteArguments("ID").WriteArguments(id.ToBytes());
        if (type != null) _ = command.WriteArguments("TYPE").WriteArguments(type);
        if (filter.User is string user) _ = command.WriteArguments("USER").WriteArguments(user);
        if (filter.Address is string address) _ = command.WriteArguments("ADDR").WriteArguments(address);
        if (filter.LocalAddress is string local) _ = command.WriteArguments("LADDR").WriteArguments(local);
        if (filter.SkipMe is bool skipMe) _ = command.WriteArguments("SKIPME").WriteArguments(skipMe ? "YES" : "NO");
        if (filter.MaxAge is long maxAge) _ = command.WriteArguments("MAXAGE").WriteArguments(maxAge.ToBytes());
        return command;
    }

    private static string ClientTypeName(RedisConnection.ClientType type) => type switch
    {
        RedisConnection.ClientType.Normal => "NORMAL",
        RedisConnection.ClientType.Master => "MASTER",
        RedisConnection.ClientType.Slave => "SLAVE",
        RedisConnection.ClientType.Replica => "REPLICA",
        RedisConnection.ClientType.PubSub => "PUBSUB",
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    internal static CommandPacket ClientList()
        => new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._list);

    internal static CommandPacket ClientList(RedisConnection.ClientType type)
    {
        var name = ClientTypeName(type);
        return ClientList().WriteArguments("TYPE").WriteArguments(name);
    }

    internal static CommandPacket ClientList(ReadOnlySpan<long> ids)
    {
        if (ids.IsEmpty) throw new ArgumentException("Specify at least one client ID.", nameof(ids));
        var command = ClientList().WriteArguments("ID");
        foreach (var id in ids) _ = command.WriteArguments(id.ToBytes());
        return command;
    }

    internal static CommandPacket ClientNoEvict(bool on)
    {
        var command = new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._noevict);
        if (on)
        {
            return command.WriteArguments("ON");
        }
        return command.WriteArguments("OFF");
    }

    internal static CommandPacket ClientNoTouch(bool on)
    {
        var command = new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._notouch);
        if (on)
        {
            return command.WriteArguments("ON");
        }
        return command.WriteArguments("OFF");
    }

    internal static CommandPacket ClientPause(long timeout, RedisConnection.ClientPauseMode? mode)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(timeout);
#else
        if (timeout < 0) throw new ArgumentOutOfRangeException(nameof(timeout));
#endif
        var option = mode switch
        {
            RedisConnection.ClientPauseMode.Write => "WRITE",
            RedisConnection.ClientPauseMode.All => "ALL",
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
        var command = new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._pause)
            .WriteArguments(timeout.ToBytes());
        if (option != null) _ = command.WriteArguments(option);
        return command;
    }

    internal static CommandPacket ClientReplyOn()
        => new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._reply).WriteArguments("ON");

    internal static CommandPacket ClientTrackingInfo()
        => new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._trackingInfo);

    internal static CommandPacket ClientUnblock(long clientId, RedisConnection.ClientUnblockMode mode)
    {
        var option = mode switch
        {
            RedisConnection.ClientUnblockMode.None => null,
            RedisConnection.ClientUnblockMode.Timeout => "TIMEOUT",
            RedisConnection.ClientUnblockMode.Error => "ERROR",
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
        var command = new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._unblock).WriteArguments(clientId.ToBytes());
        return option is null ? command : command.WriteArguments(option);
    }

    internal static CommandPacket ClientUnpause()
        => new CommandPacket(ConnectionCommands._client, CommandMode.ConnectionManagement)
            .WriteCommand(ConnectionCommands._unpause);

    internal static CommandPacket Quit()
        => new(ConnectionCommands._quit, CommandMode.ConnectionManagement);

    internal static CommandPacket Reset()
        => new(ConnectionCommands._reset, CommandMode.ConnectionManagement);
}
