using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using SharpRedis;
using SharpRedis.Provider.Standard;

internal static class CacheAvailabilityChecks
{
    private const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    internal static async Task Run(string host)
    {
        foreach (var masterSlave in new[] { false, true })
        foreach (var resp in new[] { Resp.RESP2, Resp.RESP3 })
        foreach (var broadcast in new[] { true, false })
        {
            var prefix = "availability:" + Guid.NewGuid().ToString("N") + ":";
            var cache = new Cache(prefix, broadcast);
            using var redis = masterSlave ? Redis.UseMasterSlave(o =>
            {
                o.Host = host; o.Port = 16379; o.RespVersion = resp; o.HeartbeatInterval = 120000;
                o.Slaves.Add(new() { Host = host, Port = 16380 }); o.UseClientCaching(cache);
            }) : Redis.UseStandalone(o =>
            {
                o.Host = host; o.Port = 16379; o.RespVersion = resp; o.HeartbeatInterval = 120000;
                o.UseClientCaching(cache);
            });
            using var writer = Redis.UseStandalone(o => { o.Host = host; o.Port = 16379; });
            var context = typeof(Redis).BaseType!.GetField("_context", Hidden)!.GetValue(redis)!;
            var pool = context.GetType().GetField("_connectionPool", Hidden)!.GetValue(context)!;
            var slots = (object[])pool.GetType().GetField("_connections", Hidden)!.GetValue(pool)!;
            void Disconnect() => slots[1].GetType().BaseType!.GetMethod("Invalidate", Hidden)!.Invoke(slots[1], null);
            void Restore()
            {
                var old = slots[1];
                slots[1] = old.GetType().GetMethod("Copy", Hidden)!.Invoke(old, new[] { pool })!;
                ((IDisposable)old).Dispose();
            }
            var key = prefix + "value";
            await writer.String.SetAsync(key, "old", TimeSpan.FromMinutes(1));
            await Task.Delay(50);
            Check((string?)await redis.String.GetAsync(key) == "old");
            Check(cache.Count == 1);
            Disconnect();
            await writer.String.SetAsync(key, "new", TimeSpan.FromMinutes(1));
            var sets = cache.Sets;
            Check((string?)redis.String.Get(key) == "new");
            Check((string?)await redis.String.GetAsync(key) == "new");
            using (var pipe = redis.StartPipelining())
            {
                await pipe.String.GetAsync(key);
                Check((string?)(await pipe.ExecutePipeliningAsync())![0] == "new");
                await pipe.String.GetAsync(key);
                Check((string?)pipe.ExecutePipelining()![0] == "new");
            }
            Check(cache.Sets == sets);
            Restore();
            Check((string?)await redis.String.GetAsync(key) == "new");
            Check(cache.Sets == sets + 1);
            await writer.String.SetAsync(key, "latest", TimeSpan.FromMinutes(1));
            using (var timeout = new CancellationTokenSource(5000))
                while (cache.Count != 0) await Task.Delay(10, timeout.Token);
            // Rebuild the subscription during a cache miss, before the read completes.
            sets = cache.Sets;
            cache.BeforeMiss = () => { Disconnect(); Restore(); };
            Check((string?)await redis.String.GetAsync(key) == "latest");
            Check(cache.Sets == sets);
            Check((string?)redis.String.Get(key) == "latest");
            Check(cache.Sets == sets + 1);
            Console.WriteLine($"PASS masterSlave={masterSlave}, {resp}, broadcast={broadcast}: outage bypass, sync/async pipeline, recovery and cross-generation backfill prevention");
        }
    }
    private static void Check(bool ok) { if (!ok) throw new Exception("Cache availability assertion failed"); }
    private sealed class Cache(string prefix, bool broadcast) : ClientCachingBase
    {
        private readonly ConcurrentDictionary<string, RedisResult> _items = new();
        internal int Count => _items.Count;
        internal int Sets;
        internal Action? BeforeMiss;
        protected override ClientCachingMode Mode => broadcast ? ClientCachingMode.Broadcast : ClientCachingMode.Default;
        protected override string[]? KeyPrefixes => broadcast ? new[] { prefix } : null;
        protected override bool KeyMatching(string key) => key.StartsWith(prefix, StringComparison.Ordinal);
        protected override void Set(string key, in RedisResult value) { _items[key] = value; Interlocked.Increment(ref Sets); }
        protected override bool TryGet(string key, [NotNullWhen(true)] out RedisResult? value)
        {
            if (_items.TryGetValue(key, out var result)) { value = result; return true; }
            var hook = BeforeMiss; BeforeMiss = null; hook?.Invoke();
            value = null; return false;
        }
        protected override void Remove(string key) => _items.TryRemove(key, out _);
        protected override void Clear() => _items.Clear();
    }
}
