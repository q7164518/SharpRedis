using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using static SharpRedis.RedisServer;

namespace SharpRedis.Commands;

internal static class ServerCommands
{
    private readonly static SharpValue _role = new("ROLE", CommandItemCategory.Command);
    private readonly static SharpValue _time = new("TIME", CommandItemCategory.Command);
    private readonly static SharpValue _swapdb = new("SWAPDB", CommandItemCategory.Command);
    private readonly static SharpValue _slowlog = new("SLOWLOG", CommandItemCategory.Command);
    private readonly static SharpValue _len = new("LEN", CommandItemCategory.Command);
    private readonly static SharpValue _shutdown = new("SHUTDOWN", CommandItemCategory.Command);
    private readonly static SharpValue _replicaof = new("REPLICAOF", CommandItemCategory.Command);
    private readonly static SharpValue _slaveof = new("SLAVEOF", CommandItemCategory.Command);
    private readonly static SharpValue _module = new("MODULE", CommandItemCategory.Command);
    private readonly static SharpValue _loadex = new("LOADEX", CommandItemCategory.Command);
    private readonly static SharpValue _unload = new("UNLOAD", CommandItemCategory.Command);
    private readonly static SharpValue _failover = new("FAILOVER", CommandItemCategory.Command);
    private readonly static SharpValue _info = new("INFO", CommandItemCategory.Command);
    private readonly static SharpValue _command = new("COMMAND", CommandItemCategory.Command);
    private readonly static SharpValue _count = new("COUNT", CommandItemCategory.Command);
    private readonly static SharpValue _docs = new("DOCS", CommandItemCategory.Command);
    private readonly static SharpValue _getkeys = new("GETKEYS", CommandItemCategory.Command);
    private readonly static SharpValue _getkeysandflags = new("GETKEYSANDFLAGS", CommandItemCategory.Command);
    private readonly static SharpValue _lolwut = new("LOLWUT", CommandItemCategory.Command);

    private readonly static SharpValue _flushAll = new("FLUSHALL", CommandItemCategory.Command);
    private readonly static SharpValue _flushdb = new("FLUSHDB", CommandItemCategory.Command);
    private readonly static SharpValue _bgsave = new("BGSAVE", CommandItemCategory.Command);
    private readonly static SharpValue _bgrewriteaof = new("BGREWRITEAOF", CommandItemCategory.Command);
    private readonly static SharpValue _dbsize = new("DBSIZE", CommandItemCategory.Command);
    private readonly static SharpValue _lastsave = new("LASTSAVE", CommandItemCategory.Command);

    private readonly static SharpValue _acl = new("ACL", CommandItemCategory.Command);
    private readonly static SharpValue _cat = new("CAT", CommandItemCategory.Command);
    private readonly static SharpValue _deluser = new("DELUSER", CommandItemCategory.Command);
    private readonly static SharpValue _dryrun = new("DRYRUN", CommandItemCategory.Command);
    private readonly static SharpValue _genpass = new("GENPASS", CommandItemCategory.Command);
    private readonly static SharpValue _getuser = new("GETUSER", CommandItemCategory.Command);
    private readonly static SharpValue _list = new("LIST", CommandItemCategory.Command);
    private readonly static SharpValue _load = new("LOAD", CommandItemCategory.Command);
    private readonly static SharpValue _log = new("LOG", CommandItemCategory.Command);
    private readonly static SharpValue _save = new("SAVE", CommandItemCategory.Command);
    private readonly static SharpValue _setuser = new("SETUSER", CommandItemCategory.Command);
    private readonly static SharpValue _users = new("USERS", CommandItemCategory.Command);
    private readonly static SharpValue _whoami = new("WHOAMI", CommandItemCategory.Command);

    private readonly static SharpValue _hotkeys = new("HOTKEYS", CommandItemCategory.Command);
    private readonly static SharpValue _get = new("GET", CommandItemCategory.Command);
    private readonly static SharpValue _reset = new("RESET", CommandItemCategory.Command);
    private readonly static SharpValue _start = new("START", CommandItemCategory.Command);
    private readonly static SharpValue _stop = new("STOP", CommandItemCategory.Command);

    private readonly static SharpValue _backup = new("BACKUP", CommandItemCategory.Command);
    private readonly static SharpValue _abort = new("ABORT", CommandItemCategory.Command);
    private readonly static SharpValue _cleanup = new("CLEANUP", CommandItemCategory.Command);
    private readonly static SharpValue _seal = new("SEAL", CommandItemCategory.Command);
    private readonly static SharpValue _status = new("STATUS", CommandItemCategory.Command);

    private readonly static SharpValue _latency = new("LATENCY", CommandItemCategory.Command);
    private readonly static SharpValue _doctor = new("DOCTOR", CommandItemCategory.Command);
    private readonly static SharpValue _graph = new("GRAPH", CommandItemCategory.Command);

    private readonly static SharpValue _memory = new("MEMORY", CommandItemCategory.Command);
    private readonly static SharpValue _mallocstats = new("MALLOC-STATS", CommandItemCategory.Command);
    private readonly static SharpValue _purge = new("PURGE", CommandItemCategory.Command);
    private readonly static SharpValue _stats = new("STATS", CommandItemCategory.Command);
    private readonly static SharpValue _usage = new("USAGE", CommandItemCategory.Command);

    private readonly static SharpValue _config = new("CONFIG", CommandItemCategory.Command);
    private readonly static SharpValue _resetstat = new("RESETSTAT", CommandItemCategory.Command);
    private readonly static SharpValue _rewrite = new("REWRITE", CommandItemCategory.Command);
    private readonly static SharpValue _set = new("SET", CommandItemCategory.Command);

    internal static CommandPacket MemoryDoctor()
    {
        return new CommandPacket(ServerCommands._memory, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._doctor);
    }

    internal static CommandPacket MemoryMallocStats()
    {
        return new CommandPacket(ServerCommands._memory, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._mallocstats);
    }

    internal static CommandPacket MemoryPurge()
    {
        return new CommandPacket(ServerCommands._memory, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._purge);
    }

    internal static CommandPacket MemoryStats()
    {
        return new CommandPacket(ServerCommands._memory, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._stats);
    }

    internal static CommandPacket MemoryUsage(SharpValue key, long? samples)
    {
        if (samples < 0) throw new ArgumentOutOfRangeException(nameof(samples));
        var command = new CommandPacket(ServerCommands._memory, CommandMode.ServerManagement | CommandMode.WithRead)
            .WriteCommand(ServerCommands._usage)
            .WriteKeys(key);
        if (samples.HasValue)
        {
            _ = command.WriteArguments("SAMPLES").WriteArguments(samples.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket FlushAll(AsyncSync @as)
    {
        return new CommandPacket(ServerCommands._flushAll, CommandMode.ServerManagement | CommandMode.WithWrite)
            .WriteArguments(@as is AsyncSync.Async, "ASYNC")
            .WriteArguments(@as is AsyncSync.Sync, "SYNC");
    }

    internal static CommandPacket FlushDB(AsyncSync @as)
    {
        return new CommandPacket(ServerCommands._flushdb, CommandMode.ServerManagement | CommandMode.WithWrite)
            .WriteArguments(@as is AsyncSync.Async, "ASYNC")
            .WriteArguments(@as is AsyncSync.Sync, "SYNC");
    }

#if MODERN_NET
    internal static CommandPacket AclCat(string? category)
#else
    internal static CommandPacket AclCat(string category)
#endif
    {
        var command = new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._cat);
        if (string.IsNullOrEmpty(category))
        {
            return command;
        }
        return command.WriteArguments(category);
    }

    internal static CommandPacket AclDelUser(string username)
    {
        if (username is null) throw new ArgumentException("The username to be deleted cannot be null.", nameof(username));
        return new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._deluser)
            .WriteArguments(username);
    }

    internal static CommandPacket AclDelUser(ReadOnlySpan<string> usernames)
    {
        if (usernames.IsEmpty) throw new ArgumentException("At least one username to be deleted must be specified.", nameof(usernames));
        var command = new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._deluser);
        foreach (var item in usernames)
        {
            if (item is null) throw new ArgumentException("The username to be deleted cannot be null.");
            _ = command.WriteArguments(item);
        }
        return command;
    }

    internal static CommandPacket AclDryRun(string username, SharpValueSpan command)
    {
        if (username is null) throw new ArgumentException("The username cannot be null.", nameof(username));
        if (command.IsEmpty) throw new ArgumentException("The command to be simulated for execution cannot be empty.", nameof(command));
        return new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._dryrun)
            .WriteArguments(username)
            .WriteArguments(command);
    }

    internal static CommandPacket AclGenPass(int? bits)
    {
        var command = new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._genpass);
        if (bits.HasValue)
        {
            _ = command.WriteArguments(bits.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket AclGetUser(string username)
    {
        if (username is null) throw new ArgumentException("The username cannot be null.", nameof(username));
        return new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._getuser)
            .WriteArguments(username);
    }

    internal static CommandPacket AclList()
    {
        return new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._list);
    }

    internal static CommandPacket AclLoad()
    {
        return new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._load);
    }

    internal static CommandPacket AclLog(bool reset)
    {
        return new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._log)
            .WriteArguments(reset, "RESET");
    }

    internal static CommandPacket AclLog(int? count)
    {
        var command = new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._log);
        if (count.HasValue)
        {
            _ = command.WriteArguments(count.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket AclSave()
    {
        return new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._save);
    }

    internal static CommandPacket AclSetUser(string username, ReadOnlySpan<string> rules)
    {
        if (username is null) throw new ArgumentException("The username cannot be null.", nameof(username));
        var command = new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._setuser)
            .WriteArguments(username);
        if (rules.IsEmpty)
        {
            return command;
        }
        foreach (var rule in rules)
        {
            if (string.IsNullOrWhiteSpace(rule)) throw new ArgumentException("The rule permissions cannot be left blank.");
            _ = command.WriteArguments(rule);
        }
        return command;
    }

    internal static CommandPacket AclSetUser(string username, string rule)
    {
        if (username is null) throw new ArgumentException("The username cannot be null.", nameof(username));
        if (string.IsNullOrWhiteSpace(rule)) throw new ArgumentException("The rule permissions cannot be left blank.", nameof(rule));
        return new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._setuser)
            .WriteArguments(username)
            .WriteArguments(rule);
    }

    internal static CommandPacket AclUsers()
    {
        return new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._users);
    }

    internal static CommandPacket AclWhoAmI()
    {
        return new CommandPacket(ServerCommands._acl, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._whoami);
    }

    internal static CommandPacket HotKeysGet()
    {
        return new CommandPacket(ServerCommands._hotkeys, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._get);
    }

    internal static CommandPacket HotKeysReset()
    {
        return new CommandPacket(ServerCommands._hotkeys, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._reset);
    }

    internal static CommandPacket HotKeysStart(
        HotKeysMetrics metrics,
        long? count,
        long? duration,
        long? sampleRatio,
        ReadOnlySpan<int> slots)
    {
        if (!metrics.IsValid) throw new ArgumentException("The METRICS parameter is invalid. At least one of the CPU or NET metrics needs to be tracked.", nameof(metrics));
        var command = new CommandPacket(ServerCommands._hotkeys, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._start)
            .WriteArguments("METRICS")
            .WriteArguments(metrics.Count.ToBytes())
            .WriteArguments(metrics.CPU, "CPU")
            .WriteArguments(metrics.NET, "NET");
        if (count.HasValue)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes());
        }
        if (duration.HasValue)
        {
            _ = command
                .WriteArguments("DURATION")
                .WriteArguments(duration.Value.ToBytes());
        }
        if (sampleRatio.HasValue)
        {
            _ = command
                .WriteArguments("SAMPLE")
                .WriteArguments(sampleRatio.Value.ToBytes());
        }
        if (!slots.IsEmpty)
        {
            _ = command
                .WriteArguments("SLOTS")
                .WriteArguments(slots.Length.ToBytes());
            foreach (var slot in slots)
            {
                _ = command.WriteArguments(slot.ToBytes());
            }
        }
        return command;
    }

    internal static CommandPacket HotKeysStop()
    {
        return new CommandPacket(ServerCommands._hotkeys, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._stop);
    }

    internal static CommandPacket BgSave(bool schedule)
    {
        return new CommandPacket(ServerCommands._bgsave, CommandMode.ServerManagement)
            .WriteArguments(schedule, "SCHEDULE");
    }

    internal static CommandPacket BackupAbort()
    {
        return new CommandPacket(ServerCommands._backup, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._abort);
    }

    internal static CommandPacket BackupCleanup()
    {
        return new CommandPacket(ServerCommands._backup, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._cleanup);
    }

    internal static CommandPacket BackupList()
    {
        return new CommandPacket(ServerCommands._backup, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._list);
    }

    internal static CommandPacket BackupSeal()
    {
        return new CommandPacket(ServerCommands._backup, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._seal);
    }

    internal static CommandPacket BackupStart()
    {
        return new CommandPacket(ServerCommands._backup, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._start);
    }

    internal static CommandPacket BackupStatus()
    {
        return new CommandPacket(ServerCommands._backup, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._status);
    }

    internal static CommandPacket BgRewriteAof()
    {
        return new CommandPacket(ServerCommands._bgrewriteaof, CommandMode.ServerManagement);
    }

    internal static CommandPacket DBSize()
    {
        return new CommandPacket(ServerCommands._dbsize, CommandMode.ServerManagement | CommandMode.WithRead);
    }

    internal static CommandPacket LastSave()
    {
        return new CommandPacket(ServerCommands._lastsave, CommandMode.ServerManagement);
    }

    internal static CommandPacket LatencyDoctor()
    {
        return new CommandPacket(ServerCommands._latency, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._doctor);
    }

    internal static CommandPacket LatencyGraph(string @event)
    {
        return new CommandPacket(ServerCommands._latency, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._graph)
            .WriteArguments(@event);
    }

    internal static CommandPacket Command()
        => new(ServerCommands._command, CommandMode.ServerManagement);

    internal static CommandPacket CommandCount()
        => ServerCommands.Command().WriteCommand(ServerCommands._count);

    internal static CommandPacket CommandDocs(SharpValueSpan names)
        => ServerCommands.Command().WriteCommand(ServerCommands._docs).WriteArguments(names);

    internal static CommandPacket CommandInfo(SharpValueSpan names)
        => ServerCommands.Command().WriteCommand(ServerCommands._info).WriteArguments(names);

    internal static CommandPacket CommandGetKeys(SharpValueSpan command, bool withFlags)
    {
        if (command.IsEmpty) throw new ArgumentException("The command must include a command name.", nameof(command));
        return ServerCommands.Command().WriteCommand(withFlags ? ServerCommands._getkeysandflags : ServerCommands._getkeys).WriteArguments(command);
    }

    internal static CommandPacket CommandList(CommandListFilter filter)
    {
        var command = ServerCommands.Command().WriteCommand(ServerCommands._list);
        if (filter.Kind != null) command.WriteArguments("FILTERBY").WriteArguments(filter.Kind).WriteArguments(filter.Value);
        return command;
    }

    internal static CommandPacket ConfigGet(string parameter)
    {
        if (string.IsNullOrWhiteSpace(parameter)) throw new ArgumentException("The configuration parameter name cannot be empty.", nameof(parameter));
        return new CommandPacket(ServerCommands._config, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._get)
            .WriteArguments(parameter);
    }

    internal static CommandPacket ConfigGet(ReadOnlySpan<string> parameters)
    {
        if (parameters.IsEmpty) throw new ArgumentException("Please specify at least one configuration parameter name", nameof(parameters));
        var command = new CommandPacket(ServerCommands._config, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._get);
        foreach (var parameter in parameters)
        {
            if (string.IsNullOrWhiteSpace(parameter)) throw new ArgumentException("The configuration parameter name cannot be empty.");
            _ = command.WriteArguments(parameter);
        }
        return command;
    }

    internal static CommandPacket ConfigResetStat()
    {
        return new CommandPacket(ServerCommands._config, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._resetstat);
    }

    internal static CommandPacket ConfigRewrite()
    {
        return new CommandPacket(ServerCommands._config, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._rewrite);
    }

    internal static CommandPacket ConfigSet(string parameter, string value)
    {
        if (string.IsNullOrWhiteSpace(parameter)) throw new ArgumentException("The configuration parameter name cannot be empty.", nameof(parameter));
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The value of the configuration item cannot be empty.", nameof(value));
        return new CommandPacket(ServerCommands._config, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._set)
            .WriteArguments(parameter)
            .WriteArguments(value);
    }

    internal static CommandPacket ConfigSet(Dictionary<string, string> parameterValues)
    {
        if (parameterValues is null or { Count: <= 0 }) throw new ArgumentException("Please specify at least one configuration name and its corresponding value.", nameof(parameterValues));
        var command = new CommandPacket(ServerCommands._config, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._set);
        foreach (var item in parameterValues)
        {
            _ = command
                .WriteArguments(item.Key)
                .WriteArguments(item.Value);
        }
        return command;
    }

    internal static CommandPacket Failover(long? timeout)
    {
        if (timeout <= 0) throw new ArgumentOutOfRangeException(nameof(timeout));
        var command = new CommandPacket(ServerCommands._failover, CommandMode.ServerManagement);
        if (timeout.HasValue) _ = command.WriteArguments("TIMEOUT").WriteArguments(timeout.Value.ToBytes());
        return command;
    }

    internal static CommandPacket Failover(string host, int port, bool force, long? timeout)
    {
        if (string.IsNullOrWhiteSpace(host)) throw new ArgumentException("A replica host is required.", nameof(host));
        if (port is < 1 or > 65535) throw new ArgumentOutOfRangeException(nameof(port));
        if (force && !timeout.HasValue) throw new ArgumentException("FORCE requires a timeout.", nameof(timeout));
        return ServerCommands.Failover(timeout)
            .WriteArguments("TO").WriteArguments(host).WriteArguments(port.ToBytes())
            .WriteArguments(force, "FORCE");
    }

    internal static CommandPacket FailoverAbort()
    {
        return new CommandPacket(ServerCommands._failover, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._abort);
    }

    internal static CommandPacket Info()
    {
        return new CommandPacket(ServerCommands._info, CommandMode.ServerManagement);
    }

    internal static CommandPacket Lolwut(int? version)
    {
        var command = new CommandPacket(ServerCommands._lolwut, CommandMode.ServerManagement);
        if (version.HasValue)
        {
            _ = command
                .WriteArguments("VERSION")
                .WriteArguments(version.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket ModuleList()
    {
        return new CommandPacket(ServerCommands._module, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._list);
    }

    internal static CommandPacket ModuleLoad(string path, SharpValueSpan args)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("A module path is required.", nameof(path));
        return new CommandPacket(ServerCommands._module, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._load).WriteArguments(path).WriteArguments(args);
    }

    internal static CommandPacket ModuleLoadEx(string path, ReadOnlySpan<KeyValuePair<string, SharpValue>> configs, SharpValueSpan args)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("A module path is required.", nameof(path));
        var command = new CommandPacket(ServerCommands._module, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._loadex).WriteArguments(path);
        foreach (var config in configs)
        {
            _ = command.WriteArguments("CONFIG").WriteArguments(config.Key).WriteArguments(config.Value);
        }
        if (!args.IsEmpty) _ = command.WriteArguments("ARGS").WriteArguments(args);
        return command;
    }

    internal static CommandPacket ModuleUnload(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A module name is required.", nameof(name));
        return new CommandPacket(ServerCommands._module, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._unload).WriteArguments(name);
    }

    internal static CommandPacket ReplicaOf(string host, int port)
        => ReplicaOf(_replicaof, host, port);

    internal static CommandPacket SlaveOf(string host, int port)
        => ReplicaOf(_slaveof, host, port);

    private static CommandPacket ReplicaOf(SharpValue command, string host, int port)
    {
        if (string.IsNullOrWhiteSpace(host)) throw new ArgumentException("A primary host is required.", nameof(host));
        if (port is < 1 or > 65535) throw new ArgumentOutOfRangeException(nameof(port));
        return new CommandPacket(command, CommandMode.ServerManagement)
            .WriteArguments(host).WriteArguments(port.ToBytes());
    }

    internal static CommandPacket ReplicaOfNoOne()
        => ReplicaOfNoOne(_replicaof);

    internal static CommandPacket SlaveOfNoOne()
        => ReplicaOfNoOne(_slaveof);

    private static CommandPacket ReplicaOfNoOne(SharpValue command)
    {
        return new CommandPacket(command, CommandMode.ServerManagement)
            .WriteArguments("NO")
            .WriteArguments("ONE");
    }

    internal static CommandPacket Save()
    {
        return new CommandPacket(ServerCommands._save, CommandMode.ServerManagement);
    }

    internal static CommandPacket Shutdown(bool? save, bool now, bool force)
    {
        return new CommandPacket(ServerCommands._shutdown, CommandMode.ServerManagement | CommandMode.WithBlocking
            | CommandMode.DoNotReuseConnection | CommandMode.Shutdown)
            .WriteArguments(save == true, "SAVE")
            .WriteArguments(save == false, "NOSAVE")
            .WriteArguments(now, "NOW")
            .WriteArguments(force, "FORCE");
    }

    internal static CommandPacket ShutdownAbort()
    {
        return new CommandPacket(ServerCommands._shutdown, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._abort);
    }

    internal static CommandPacket SlowLogGet(long? count)
    {
        if (count < -1) throw new ArgumentOutOfRangeException(nameof(count));
        var command = new CommandPacket(ServerCommands._slowlog, CommandMode.ServerManagement)
            .WriteCommand(ServerCommands._get);
        if (count.HasValue) _ = command.WriteArguments(count.Value.ToBytes());
        return command;
    }

    internal static CommandPacket SlowLogLen()
        => new CommandPacket(ServerCommands._slowlog, CommandMode.ServerManagement).WriteCommand(ServerCommands._len);

    internal static CommandPacket SlowLogReset()
        => new CommandPacket(ServerCommands._slowlog, CommandMode.ServerManagement).WriteCommand(ServerCommands._reset);

    internal static CommandPacket SwapDB(int index1, int index2)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(index1);
        ArgumentOutOfRangeException.ThrowIfNegative(index2);
#else
        if (index1 < 0) throw new ArgumentOutOfRangeException(nameof(index1));
        if (index2 < 0) throw new ArgumentOutOfRangeException(nameof(index2));
#endif
        return new CommandPacket(ServerCommands._swapdb, CommandMode.ServerManagement | CommandMode.WithWrite)
            .WriteArguments(index1.ToBytes())
            .WriteArguments(index2.ToBytes());
    }

    internal static CommandPacket Time()
        => new(ServerCommands._time, CommandMode.ServerManagement);

    internal static CommandPacket Role()
        => new(ServerCommands._role, CommandMode.ServerManagement);
}
