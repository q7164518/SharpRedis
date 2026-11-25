using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using SharpRedis;
using SharpRedis.Exceptions;
using SharpRedis.Provider.Standard;

internal static class SentinelChecks
{
    private const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    internal static async Task Run(string host)
    {
        foreach (var readWriteSeparation in new[] { false, true })
        foreach (var resp in new[] { Resp.RESP2, Resp.RESP3 })
        foreach (var broadcast in new[] { false, true })
        {
            string prefix = "sentinel:" + Guid.NewGuid().ToString("N") + ":";
            var cache = new Cache(prefix, broadcast);
            void Configure(SentinelConnectionOptions o)
            {
                o.MasterName = "mymaster"; o.Password = "sharpredis-data-test";
                o.RespVersion = resp; o.CommandTimeout = 1500; o.SentinelCommandTimeout = 500;
                o.SentinelRefreshInterval = 200; o.HeartbeatInterval = 500; o.ReconnectInterval = 200;
                o.SlaveSelectionMode = broadcast ? SlaveSelectionMode.Random : SlaveSelectionMode.RoundRobin;
                if (o.Sentinels.Count == 0)
                foreach (int port in new[] { 27379, 27380, 27381 })
                    o.Sentinels.Add(new() { Host = host, Port = port, Password = "sharpredis-sentinel-test" });
                o.UseClientCaching(cache);
            }
            using var redis = broadcast
                ? Redis.UseSentinel(readWriteSeparation, "mastername=mymaster,password=sharpredis-data-test",
                    new[] { $"host={host},port=27379,password=sharpredis-sentinel-test", $"host={host},port=27380,password=sharpredis-sentinel-test", $"host={host},port=27381,password=sharpredis-sentinel-test" }, Configure)
                : Redis.UseSentinel(readWriteSeparation, Configure);
            Check(redis.RedisMode == RedisServerMode.Sentinel, "Mode");
            var context = typeof(Redis).BaseType!.GetField("_context", Hidden)!.GetValue(redis)!;
            var pool = context.GetType().GetField("_connectionPool", Hidden)!.GetValue(context)!;
            string Address() => (string)pool.GetType().GetField("_masterAddress", Hidden)!.GetValue(pool)!;
            bool Ready() => !(bool)pool.GetType().GetField("_switching", Hidden)!.GetValue(pool)!;
            void CheckMasterOnly()
            {
                if (readWriteSeparation) return;
                var current = pool.GetType().GetField("_current", Hidden)!.GetValue(pool)!;
                var slots = (object[])current.GetType().GetField("_connections", Hidden)!.GetValue(current)!;
                Check(slots.Length == 2, "Master-only mode allocated replica slots");
            }
            CheckMasterOnly();
            var key = prefix + "value";
            await redis.String.SetAsync(key, "before", TimeSpan.FromMinutes(5));
            Check((string?)await redis.String.GetAsync(key) == "before" && cache.Count == 1, "Initial cache");
            async Task<int> ReadPort(CommandFlags flags)
            {
                try { await redis.Hash.HGetAsync(key, "field", flags); return 0; }
                catch (RedisErrorException ex) { return ex.Port; }
            }
            Check(await ReadPort(CommandFlags.ForceMaster) == int.Parse(Address().Split(':')[1]), "ForceMaster route");
            await Until(async () =>
            {
                int port = await ReadPort(CommandFlags.ForceSlave);
                return port != 0 && (readWriteSeparation ? port != int.Parse(Address().Split(':')[1]) : port == int.Parse(Address().Split(':')[1]));
            });
            var received = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            await redis.PubSub.SubscribeAsync(prefix + "channel", (_, value) => { received.TrySetResult(value.ToString()!); return Task.CompletedTask; });
            string old = Address();
            await Until(async () => { await Cli(27379, "sharpredis-sentinel-test", "SENTINEL", "FAILOVER", "mymaster"); return true; });
            await Until(() => Task.FromResult(Address() != old && Ready()));
            // Allow the replica list to converge, which can cause another pool replacement.
            await Until(async () =>
            {
                await redis.String.SetAsync(key, "after", TimeSpan.FromMinutes(5));
                return (string?)await redis.String.GetAsync(key) == "after" && cache.Count == 1;
            });
            CheckMasterOnly();
            int masterPort = int.Parse(Address().Split(':')[1]);
            await Cli(masterPort, "sharpredis-data-test", "SET", key, "external", "EX", "300");
            await Until(() => Task.FromResult(cache.Count == 0));
            Check((string?)await redis.String.GetAsync(key) == "external", "Tracking migrated");
            await Until(async () => { await redis.PubSub.PublishAsync(prefix + "channel", "migrated"); return received.Task.IsCompleted; });
            Check(await received.Task == "migrated", "Subscription migrated");
            using (var pipe = redis.StartPipelining())
            {
                await pipe.String.GetAsync(key);
                Check((string?)(await pipe.ExecutePipeliningAsync())![0] == "external", "Pipeline");
            }
            using (var db = redis.Select(1))
            {
                await db.String.SetAsync(key, "db1", TimeSpan.FromMinutes(5));
                Check((string?)await db.String.GetAsync(key) == "db1", "Database");
            }
            using (var tran = await redis.MultiAsync())
            {
                await tran.String.SetAsync(prefix + "tx", "ok", TimeSpan.FromMinutes(5));
                Check((await tran.ExecAsync()).Result?.Length == 1, "Transaction");
            }
            if (resp == Resp.RESP3 && broadcast)
            {
                using var switched = redis.Select(2);
                await switched.String.SetAsync(key, "old-db", TimeSpan.FromMinutes(5));
                string beforeCrash = Address();
                try
                {
                    await Cli(27379, "sharpredis-sentinel-test", "SHUTDOWN", "NOSAVE");
                    await Cli(int.Parse(beforeCrash.Split(':')[1]), "sharpredis-data-test", "SHUTDOWN", "NOSAVE");
                    await Until(() => Task.FromResult(Address() != beforeCrash && Ready()));
                    await Until(async () =>
                    {
                        await redis.String.SetAsync(key, "crash-recovered", TimeSpan.FromMinutes(5));
                        return (string?)await redis.String.GetAsync(key) == "crash-recovered";
                    });
                    CheckMasterOnly();
                    await switched.String.SetAsync(key, "new-db", TimeSpan.FromMinutes(5));
                    Check((string?)await switched.String.GetAsync(key) == "new-db", "Existing database client after failover");
                    await Cli(int.Parse(Address().Split(':')[1]), "sharpredis-data-test", "SET", key, "crash-external", "EX", "300");
                    await Until(async () => (string?)await redis.String.GetAsync(key) == "crash-external");
                    Console.WriteLine("PASS crashed master + one Sentinel down: automatic promotion, writes, tracking and existing database client recovered");
                }
                finally
                {
                    await StartServer(beforeCrash.Split(':')[1], false);
                    await StartServer("27379", true);
                }
            }
            Console.WriteLine($"PASS Sentinel readWriteSeparation={readWriteSeparation} {resp} broadcast={broadcast}: separate passwords, failover {old} -> {Address()}, cache tracking, subscriptions, pipeline, database, transaction");
        }
    }
    private static async Task Cli(int port, string password, params string[] args)
    {
        var start = new ProcessStartInfo("wslc") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true };
        foreach (string arg in new[] { "exec", "sharpredis-sentinel-test-20261117", "redis-cli", "-p", port.ToString(), "-a", password, "--no-auth-warning" }.Concat(args)) start.ArgumentList.Add(arg);
        using var process = Process.Start(start)!;
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        Check(process.ExitCode == 0 && (output.Trim() == "OK" || (args[0] == "SHUTDOWN" && output.Trim() == "")), output + error);
    }
    private static async Task StartServer(string port, bool sentinel)
    {
        var start = new ProcessStartInfo("wslc") { UseShellExecute = false, CreateNoWindow = true };
        foreach (string arg in new[] { "exec", "sharpredis-sentinel-test-20261117", "redis-server", $"/data/{port}/{(sentinel ? "sentinel" : "redis")}.conf" }) start.ArgumentList.Add(arg);
        if (sentinel) start.ArgumentList.Add("--sentinel");
        using var process = Process.Start(start)!;
        await process.WaitForExitAsync();
        Check(process.ExitCode == 0, "Test server restart failed");
    }
    private static async Task Until(Func<Task<bool>> predicate)
    {
        var watch = Stopwatch.StartNew(); Exception? last = null;
        while (watch.Elapsed < TimeSpan.FromSeconds(35))
        {
            try { if (await predicate()) return; } catch (Exception ex) { last = ex; }
            await Task.Delay(100);
        }
        throw new Exception("Sentinel recovery timed out", last);
    }
    private static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
    private sealed class Cache(string prefix, bool broadcast) : ClientCachingBase
    {
        private readonly ConcurrentDictionary<string, RedisResult> _items = new();
        internal int Count => _items.Count;
        protected override ClientCachingMode Mode => broadcast ? ClientCachingMode.Broadcast : ClientCachingMode.Default;
        protected override string[]? KeyPrefixes => broadcast ? new[] { prefix } : null;
        protected override bool KeyMatching(string key) => key.StartsWith(prefix, StringComparison.Ordinal);
        protected override void Set(string key, in RedisResult value) => _items[key] = value;
        protected override bool TryGet(string key, [NotNullWhen(true)] out RedisResult? value)
        { if (_items.TryGetValue(key, out var result)) { value = result; return true; } value = null; return false; }
        protected override void Remove(string key) => _items.TryRemove(key, out _);
        protected override void Clear() => _items.Clear();
    }
}
