using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using SharpRedis;
using SharpRedis.Exceptions;
using SharpRedis.Provider.Standard;

internal static class ClusterChecks
{
    private const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    internal static async Task Run(string host)
    {
        await CheckCrc();
        foreach (bool split in new[] { false, true })
        foreach (var resp in new[] { Resp.RESP2, Resp.RESP3 })
        foreach (bool broadcast in new[] { false, true })
        {
            string prefix = "cluster:" + Guid.NewGuid().ToString("N") + ":";
            var cache = new Cache(prefix, broadcast);
            using var redis = Redis.UseCluster(split, o =>
            {
                o.Nodes.Add(new() { Host = host, Port = 18379 }); o.Password = "sharpredis-cluster-test";
                o.RespVersion = resp; o.CommandTimeout = 1500; o.ClusterRefreshInterval = 120000;
                o.HeartbeatInterval = 1000; o.SlaveSelectionMode = broadcast ? SlaveSelectionMode.Random : SlaveSelectionMode.RoundRobin;
                o.UseClientCaching(cache);
            });
            Check(redis.RedisMode == RedisServerMode.Cluster && redis.DataBase == 0, "Mode/database");
            try { using var invalid = redis.Select(1); throw new Exception("SELECT accepted"); }
            catch (NotSupportedException) { }
            string a = prefix + "{a}:1", a2 = prefix + "{a}:2", b = prefix + "{b}:1";
            foreach (string key in new[] { a, b, prefix + "{c}:1" })
            {
                Check(await redis.String.SetAsync(key, "value", TimeSpan.FromMinutes(5)), "SET");
                Check((string?)await redis.String.GetAsync(key) == "value", "GET");
            }
            Check(cache.Count == 3, "Per-master caching");
            int master = await ErrorPort(redis, a, CommandFlags.ForceMaster);
            await Until(async () =>
            {
                int selected = await ErrorPort(redis, a, CommandFlags.ForceSlave);
                return split ? selected != master && selected > 0 : selected == master;
            });
            var owner = Nodes(Pool(redis)).Single(node => (int)node.GetType().GetProperty("Port", Hidden)!.GetValue(node)! == master);
            var subscriber = owner.GetType().GetField("_subscriber", Hidden)!.GetValue(owner)!;
            subscriber.GetType().BaseType!.GetMethod("Invalidate", Hidden)!.Invoke(subscriber, null);
            int sets = cache.Sets;
            await Cli(master, "SET", a, "subscriber-down", "EX", "300");
            Check((string?)await redis.String.GetAsync(a) == "subscriber-down" && cache.Sets == sets, "Cache used during subscriber outage");
            owner.GetType().GetMethod("Repair", Hidden)!.Invoke(owner, null);
            Check((string?)await redis.String.GetAsync(a) == "subscriber-down" && cache.Sets > sets, "Subscriber recovery did not rebind tracking");
            await Cli(master, "SET", a, "external", "EX", "300");
            await Until(async () => (string?)await redis.String.GetAsync(a) == "external");
            await redis.String.SetAsync(a2, "two", TimeSpan.FromMinutes(5));
            var values = await redis.String.MGetAsync(new SharpValue[] { a, a2 });
            Check((string?)values[0] == "external" && (string?)values[1] == "two", "Same-slot MGET");
            using var poison = new PoisonMemory();
            try { await redis.String.MGetAsync(new SharpValue[] { a, b, poison.Value }); throw new Exception("Cross-slot accepted"); }
            catch (ArgumentException ex) { Check(ex.Message.StartsWith("CROSSSLOT"), ex.Message); }
            using (var pipe = redis.StartPipelining())
            {
                await pipe.String.SetAsync(a, "must-not-execute", TimeSpan.FromMinutes(5));
                try
                {
                    await pipe.String.GetAsync(b);
                    await pipe.String.GetAsync(poison.Value);
                    throw new Exception("Cross-slot pipeline accepted at enqueue");
                }
                catch (ArgumentException ex) { Check(ex.Message.StartsWith("CROSSSLOT"), ex.Message); }
                try { pipe.String.Get(b); throw new Exception("Sync cross-slot pipeline accepted at enqueue"); }
                catch (ArgumentException ex) { Check(ex.Message.StartsWith("CROSSSLOT"), ex.Message); }
                try { await pipe.String.MGetAsync(new SharpValue[] { a, b, poison.Value }); throw new Exception("Cross-slot MGET accepted at enqueue"); }
                catch (ArgumentException ex) { Check(ex.Message.StartsWith("CROSSSLOT"), ex.Message); }
                Check((string?)await redis.String.GetAsync(a, CommandFlags.ForceMaster) == "external", "Pipeline partially executed");
                await pipe.String.SetAsync(a, "pipeline", TimeSpan.FromMinutes(5));
                await pipe.String.GetAsync(a2);
                var pipelineResults = await pipe.ExecutePipeliningAsync();
                Check(pipelineResults!.Length == 3 && (string?)pipelineResults[2] == "two", "Rejected commands were added to the pipeline");
                pipe.String.Set(b, "reset-slot", TimeSpan.FromMinutes(5));
                pipe.String.Get(b);
                Check((string?)pipe.ExecutePipelining()![1] == "reset-slot", "Pipeline slot was not reset after execution");
            }
            using (var transaction = await redis.MultiAsync())
            {
                await transaction.String.SetAsync(a, "transaction", TimeSpan.FromMinutes(5));
                await transaction.String.GetAsync(a2);
                Check((string?)(await transaction.ExecAsync()).Result![1] == "two", "Transaction routing");
            }
            using (var transaction = await redis.MultiAsync())
            {
                await transaction.String.SetAsync(a, "must-not-commit", TimeSpan.FromMinutes(5));
                try { await transaction.String.SetAsync(b, "bad"); throw new Exception("Cross-slot transaction accepted"); }
                catch (ArgumentException) { }
                try { await transaction.ExecAsync(); throw new Exception("Invalid transaction executed"); }
                catch (RedisInitializationException) { }
            }
            Check((string?)await redis.String.GetAsync(a, CommandFlags.ForceMaster) == "transaction", "Partial transaction committed");
            using (var transaction = redis.CreateTransaction(multi: false))
            {
                Check(transaction.Watch(a), "WATCH");
                Check(transaction.Multi(), "Deferred MULTI");
                Check(transaction.Discard(), "Deferred DISCARD");
                await redis.String.SetAsync(a, "changed-after-discard", TimeSpan.FromMinutes(5));
                Check(transaction.Multi(), "MULTI after DISCARD");
                await transaction.String.SetAsync(a, "watch-cleared", TimeSpan.FromMinutes(5));
                Check((await transaction.ExecAsync()).Result?.Length == 1, "DISCARD left WATCH on the socket");
            }
            using (var cancellation = new CancellationTokenSource(100))
            {
                try { await redis.Stream.XReadAsync(prefix + "{a}:stream", "0-0", block: 0, cancellationToken: cancellation.Token); throw new Exception("Blocking command was not canceled"); }
                catch (RedisCanceledException) { }
            }
            object pool = Pool(redis);
            var nodes = Nodes(pool);
            foreach (var node in nodes)
            {
                var dedicated = (System.Collections.ICollection)node.GetType().GetField("_dedicated", Hidden)!.GetValue(node)!;
                await Until(() => Task.FromResult(dedicated.Count == 0));
                if (!split) Check(((Array)node.GetType().GetField("_replicas", Hidden)!.GetValue(node)!).Length == 0, "Replica allocated with split disabled");
            }
            Console.WriteLine($"PASS Cluster split={split} {resp} broadcast={broadcast}: routing, per-master tracking, fail-fast multi-key/pipeline, transaction and canceled temporary connection");
        }
    }
    private delegate ushort Slot(ReadOnlySpan<byte> key);
    private delegate ushort FullSlot(in SharpValue key, Encoding encoding, byte[] prefix, byte[] suffix);
    private static async Task CheckCrc()
    {
        var crc = typeof(Redis).Assembly.GetType("SharpRedis.Extensions.CRC16")!;
        var slot = (Slot)crc.GetMethod("GetClusterSlot", BindingFlags.Static | BindingFlags.NonPublic, new[] { typeof(ReadOnlySpan<byte>) })!.CreateDelegate(typeof(Slot));
        var full = (FullSlot)crc.GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Single(m => m.GetParameters().Length == 4).CreateDelegate(typeof(FullSlot));
        foreach (string key in new[] { "123456789", "foo{bar}zap", "foo{}bar{baz}", "foo{{bar}}zap", "中文{标签}:尾", "{}", "" })
            Check(slot(Encoding.UTF8.GetBytes(key)) == int.Parse(await Cli(18379, "CLUSTER", "KEYSLOT", key)), "CRC/tag mismatch: " + key);
        foreach (SharpValue key in new SharpValue[] { "中文", "中文".AsMemory(), Encoding.UTF8.GetBytes("中文") })
            Check(full(in key, Encoding.UTF8, Encoding.UTF8.GetBytes("{pre"), Encoding.UTF8.GetBytes("fix}")) == int.Parse(await Cli(18379, "CLUSTER", "KEYSLOT", "{pre中文fix}")), "Full key hashing");
        foreach (SharpValue empty in new SharpValue[] { "", "".AsMemory(), Array.Empty<byte>() })
            Check(full(in empty, Encoding.UTF8, Array.Empty<byte>(), Array.Empty<byte>()) == 0, "Empty key hashing");
        Console.WriteLine("PASS CRC16 against Redis, empty/nested tags, UTF8, char memory, byte memory and prefix/suffix");
    }
    internal static object Pool(Redis redis)
    {
        var context = typeof(Redis).BaseType!.GetField("_context", Hidden)!.GetValue(redis)!;
        return context.GetType().GetField("_connectionPool", Hidden)!.GetValue(context)!;
    }
    internal static object[] Nodes(object pool)
    {
        var snapshot = pool.GetType().GetField("_snapshot", Hidden)!.GetValue(pool)!;
        return ((Array)snapshot.GetType().GetField("_nodes", Hidden)!.GetValue(snapshot)!).Cast<object>().ToArray();
    }
    internal static async Task<int> ErrorPort(Redis redis, string key, CommandFlags flags)
    {
        try { await redis.Hash.HGetAsync(key, "field", flags); return 0; }
        catch (RedisErrorException ex) { return ex.Port; }
    }
    internal static async Task<string> Cli(int port, params string[] args)
    {
        var start = new ProcessStartInfo("wslc") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true };
        foreach (string arg in new[] { "exec", "sharpredis-cluster-test-20261117", "redis-cli", "--raw", "-p", port.ToString(), "-a", "sharpredis-cluster-test", "--no-auth-warning" }.Concat(args)) start.ArgumentList.Add(arg);
        using var process = Process.Start(start)!;
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        Check(process.ExitCode == 0 && !output.StartsWith("ERR"), output + error);
        return output.Trim();
    }
    internal static async Task Until(Func<Task<bool>> predicate)
    {
        var watch = Stopwatch.StartNew(); Exception? last = null;
        while (watch.Elapsed < TimeSpan.FromSeconds(40))
        {
            try { if (await predicate()) return; } catch (Exception ex) { last = ex; }
            await Task.Delay(100);
        }
        throw new Exception("Cluster recovery timed out", last);
    }
    internal static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
    private sealed class PoisonMemory : MemoryManager<byte>
    {
        internal ReadOnlyMemory<byte> Value => CreateMemory(1);
        public override Span<byte> GetSpan() => throw new InvalidOperationException("Later key was accessed after the slot mismatch");
        public override MemoryHandle Pin(int elementIndex = 0) => throw new NotSupportedException();
        public override void Unpin() { }
        protected override void Dispose(bool disposing) { }
    }
    internal sealed class Cache(string prefix, bool broadcast) : ClientCachingBase
    {
        private readonly ConcurrentDictionary<string, RedisResult> _items = new();
        internal int Count => _items.Count;
        internal int Sets;
        protected override ClientCachingMode Mode => broadcast ? ClientCachingMode.Broadcast : ClientCachingMode.Default;
        protected override string[]? KeyPrefixes => broadcast ? new[] { prefix } : null;
        protected override bool KeyMatching(string key) => key.StartsWith(prefix, StringComparison.Ordinal);
        protected override void Set(string key, in RedisResult value) { _items[key] = value; Interlocked.Increment(ref Sets); }
        protected override bool TryGet(string key, [NotNullWhen(true)] out RedisResult? value)
        { if (_items.TryGetValue(key, out var result)) { value = result; return true; } value = null; return false; }
        protected override void Remove(string key) => _items.TryRemove(key, out _);
        protected override void Clear() => _items.Clear();
    }
}
