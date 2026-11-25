using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using SharpRedis;
using SharpRedis.Exceptions;
using SharpRedis.Provider.Standard;

internal static class MasterSlaveCachingChecks
{
    internal static async Task Run(string host)
    {
        using var writer = Redis.UseStandalone(o => { o.Host = host; o.Port = 16379; });
        using var replica = Redis.UseStandalone(o => { o.Host = host; o.Port = 16380; });
        foreach (var resp in new[] { Resp.RESP2, Resp.RESP3 })
        foreach (var mode in new[] { ClientCachingBase.ClientCachingMode.Default, ClientCachingBase.ClientCachingMode.Broadcast })
        {
            var prefix = "cache-test:" + Guid.NewGuid().ToString("N");
            var key = prefix + ":cached:value";
            var other = prefix + ":uncached:value";
            const string physicalPrefix = "physical:";
            const string suffix = ":suffix";
            var physical = physicalPrefix + key + suffix;
            var cache = new InspectableCache(mode, physicalPrefix + prefix + ":cached:");
            using var redis = Redis.UseMasterSlave(o =>
            {
                o.Host = host; o.Port = 16379; o.RespVersion = resp; o.HeartbeatInterval = 150; o.ReconnectInterval = 100;
                o.Prefix = physicalPrefix; o.Suffix = suffix;
                o.SlaveSelectionMode = SlaveSelectionMode.Random;
                o.Slaves.Add(new() { Host = host, Port = 16380 }); o.Slaves.Add(new() { Host = host, Port = 16381 });
                o.UseClientCaching(cache);
            });
            await redis.String.SetAsync(key, "old", TimeSpan.FromMinutes(5));
            await writer.String.SetAsync(physicalPrefix + other + suffix, "other", TimeSpan.FromMinutes(5));
            await Until(async () => (string?)await replica.String.GetAsync(physical) == "old" && (string?)await replica.String.GetAsync(physicalPrefix + other + suffix) == "other");
            Check((string?)await redis.String.GetAsync(key) == "old", "Cacheable miss failed");
            var stored = cache.Sets;
            Check((string?)redis.String.Get(key) == "old" && cache.Sets == stored && cache.Hits > 0, "Cache hit failed");
            Check(await WrongTypePort(redis, key) == 16379, "Cacheable Key was not routed to master");
            Check(await WrongTypePort(redis, other) is 16380 or 16381, "Unmatched Key was not routed to replica");
            await redis.String.GetAsync(key, CommandFlags.ForceSlave);
            Check(cache.Sets == stored, "ForceSlave polluted cache");
            await redis.String.GetAsync(key, CommandFlags.ForceMaster);
            Check(cache.Sets == stored, "ForceMaster should bypass cache");
            var bytes = await redis.String.GetAsync(key, CommandFlags.ReturnBytes);
            Check(bytes.RawValue is byte[] b && Encoding.UTF8.GetString(b) == "old", "ReturnBytes cache collision");
            Check((string?)await redis.String.GetAsync(key) == "old", "Text cache corrupted by ReturnBytes");

            // The invalidation is received from the master while replicas still return the old value.
            await Raw(host, 16380, "CLIENT", "PAUSE", "1800", "WRITE");
            await Raw(host, 16381, "CLIENT", "PAUSE", "1800", "WRITE");
            await writer.String.SetAsync(physical, "new", TimeSpan.FromMinutes(5));
            await Until(() => Task.FromResult(!cache.Contains("GET " + physical)));
            Check((string?)await replica.String.GetAsync(physical) == "old", "Replication-lag fixture did not pause");
            Check((string?)await redis.String.GetAsync(key) == "new", "Cached Key was repopulated from stale replica");
            await Task.Delay(1900);

            // NOLOOP must not suppress invalidations for writes on our own pooled master connection.
            await redis.String.SetAsync(key, "own-write", TimeSpan.FromMinutes(5));
            await Until(() => Task.FromResult(!cache.Contains("GET " + physical)));
            Check((string?)await redis.String.GetAsync(key) == "own-write", "Own-write invalidation failed");
            using (var db = redis.Select(1))
            {
                await db.String.SetAsync(key, "db1", TimeSpan.FromMinutes(5));
                Check((string?)await db.String.GetAsync(key) == "db1", "DB1 cache miss failed");
                await db.String.SetAsync(key, "db1-new", TimeSpan.FromMinutes(5));
                await Until(() => Task.FromResult(!cache.Contains("GET " + physical)));
                Check((string?)db.String.Get(key) == "db1-new", "Temporary DB cache invalidation failed");
                Check((string?)await redis.String.GetAsync(key) == "own-write", "DB cache collision");
            }
            using (var pipe = redis.StartPipelining())
            {
                await pipe.String.GetAsync(key); await pipe.String.GetAsync(other);
                var values = await pipe.ExecutePipeliningAsync();
                Check((string?)values![0] == "own-write" && (string?)values[1] == "other", "Mixed cache eligibility pipeline failed");
                await pipe.String.SetAsync(key, "pipeline-write", TimeSpan.FromMinutes(5)); await pipe.String.GetAsync(key);
                values = await pipe.ExecutePipeliningAsync();
                Check((string?)values![1] == "pipeline-write", "Pipeline used cached read after write");
            }
            var expireKey = prefix + ":cached:expire";
            await redis.String.SetAsync(expireKey, "short", TimeSpan.FromSeconds(1));
            await redis.String.GetAsync(expireKey);
            await Until(async () => !await writer.Key.ExistsAsync(physicalPrefix + expireKey + suffix));
            await Until(() => Task.FromResult(!cache.Contains("GET " + physicalPrefix + expireKey + suffix)));

            // Verify reconnection clears old cache and configures the replacement tracking redirect.
            var slots = Slots(redis);
            foreach (var index in new[] { 1, 0 })
            {
                var previous = slots[index];
                Invalidate(previous);
                await Until(() => Task.FromResult(!ReferenceEquals(previous, slots[index])));
                await writer.String.SetAsync(physical, "after-reconnect", TimeSpan.FromMinutes(5));
                await Until(() => Task.FromResult(!cache.Contains("GET " + physical)));
                Check((string?)await redis.String.GetAsync(key) == "after-reconnect", "Cache after reconnect failed");
                await writer.String.SetAsync(physical, "after-invalidation", TimeSpan.FromMinutes(5));
                await Until(() => Task.FromResult(!cache.Contains("GET " + physical)));
                Check((string?)await redis.String.GetAsync(key) == "after-invalidation", "Reconnected tracking failed");
            }
            Check(slots.Skip(2).All(c => (long)c.GetType().BaseType!.GetProperty("RedirectConnectionId", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(c)! == 0), "Replica tracking was enabled");
            Console.WriteLine($"PASS cache {mode}, {resp}: master misses, replica bypass, hits, own/external/expiry/DB invalidation, lag, pipeline and reconnect");
        }
    }

    private static async Task<int> WrongTypePort(Redis redis, string key)
    {
        try { await redis.Hash.HGetAsync(key, "f"); throw new Exception("Expected WRONGTYPE"); }
        catch (RedisErrorException ex) { return ex.Port; }
    }
    private static object[] Slots(Redis redis)
    {
        const BindingFlags fields = BindingFlags.Instance | BindingFlags.NonPublic;
        var context = typeof(Redis).BaseType!.GetField("_context", fields)!.GetValue(redis)!;
        var pool = context.GetType().GetField("_connectionPool", fields)!.GetValue(context)!;
        return (object[])pool.GetType().GetField("_connections", fields)!.GetValue(pool)!;
    }
    private static void Invalidate(object c) => c.GetType().BaseType!.GetMethod("Invalidate", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(c, null);
    private static void Check(bool pass, string message) { if (!pass) throw new Exception(message); }
    private static async Task Until(Func<Task<bool>> predicate)
    {
        using var timeout = new CancellationTokenSource(10000);
        while (!await predicate()) await Task.Delay(15, timeout.Token);
    }
    private static async Task Raw(string host, int port, params string[] args)
    {
        using var tcp = new TcpClient(); await tcp.ConnectAsync(host, port);
        var request = "*" + args.Length + "\r\n" + string.Concat(args.Select(a => "$" + Encoding.UTF8.GetByteCount(a) + "\r\n" + a + "\r\n"));
        await tcp.GetStream().WriteAsync(Encoding.UTF8.GetBytes(request));
        using var reader = new StreamReader(tcp.GetStream());
        if (await reader.ReadLineAsync() != "+OK") throw new Exception("CLIENT PAUSE failed");
    }
    private sealed class InspectableCache(ClientCachingBase.ClientCachingMode mode, string prefix) : ClientCachingBase
    {
        private readonly ConcurrentDictionary<string, RedisResult> _values = new();
        internal int Sets;
        internal int Hits;
        protected override ClientCachingMode Mode => mode;
        protected override string[]? KeyPrefixes => mode == ClientCachingMode.Broadcast ? new[] { prefix } : null;
        protected override bool KeyMatching(string key) => key.StartsWith(prefix, StringComparison.Ordinal);
        protected override void Set(string key, in RedisResult value) { _values[key] = value; Interlocked.Increment(ref Sets); }
        protected override bool TryGet(string key, [NotNullWhen(true)] out RedisResult? value)
        {
            if (_values.TryGetValue(key, out var result)) { value = result; Interlocked.Increment(ref Hits); return true; }
            value = null; return false;
        }
        protected override void Remove(string key) => _values.TryRemove(key, out _);
        protected override void Clear() => _values.Clear();
        internal bool Contains(string commandPart) => _values.Keys.Any(k => k.Contains(commandPart, StringComparison.Ordinal));
    }
}
