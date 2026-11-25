using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using SharpRedis;
using SharpRedis.Provider.Standard;

internal static class StandaloneTrackingChecks
{
    private const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    internal static async Task Run(string host)
    {
        foreach (var resp in new[] { Resp.RESP2, Resp.RESP3 })
        foreach (var broadcast in new[] { true, false })
        {
            var prefix = "tracking-check:" + Guid.NewGuid().ToString("N") + ":";
            var cache = new Cache(prefix, broadcast);
            using var redis = Redis.UseStandalone(o =>
            {
                o.Host = host; o.Port = 16379; o.RespVersion = resp;
                o.HeartbeatInterval = 250; o.ReconnectInterval = 100;
                o.UseClientCaching(cache);
            });
            using var writer = Redis.UseStandalone(o => { o.Host = host; o.Port = 16379; });
            var context = typeof(Redis).BaseType!.GetField("_context", Hidden)!.GetValue(redis)!;
            var pool = context.GetType().GetField("_connectionPool", Hidden)!.GetValue(context)!;
            var slots = (object[])pool.GetType().GetField("_connections", Hidden)!.GetValue(pool)!;
            var getTemp = pool.GetType().GetMethod("GetTempConnection")!;
            var temporary = getTemp.Invoke(pool, new object[] { 0 })!;
            try
            {
                for (var round = 0; round < 3; round++)
                {
                    var oldSubscriber = slots[1];
                    oldSubscriber.GetType().BaseType!.GetMethod("Invalidate", Hidden)!.Invoke(oldSubscriber, null);
                    await Until(() => !ReferenceEquals(oldSubscriber, slots[1]));
                    Check(Id(slots[0], "RedirectConnectionId") == Id(slots[1], "ClientId"), "Master redirect not restored");
                    // Hold a borrowed connection across reconnection to avoid idle retirement.
                    temporary.GetType().GetMethod("Return", Hidden)!.Invoke(temporary, new object[] { true });
                    var reused = getTemp.Invoke(pool, new object[] { 0 })!;
                    Check(ReferenceEquals(temporary, reused), "Temporary connection not reused");
                    Check(Id(reused, "RedirectConnectionId") == Id(slots[1], "ClientId"), "Temporary redirect not restored");
                    var key = prefix + round;
                    await writer.String.SetAsync(key, "old", TimeSpan.FromMinutes(1));
                    Check((string?)await redis.String.GetAsync(key) == "old", "Initial GET failed");
                    Check(cache.Count > 0, "Read was not cached");
                    await writer.String.SetAsync(key, "new", TimeSpan.FromMinutes(1));
                    await Until(() => cache.Count == 0);
                    Check((string?)await redis.String.GetAsync(key) == "new", "Invalidation after rebind failed");
                }
            }
            finally { ((IDisposable)temporary).Dispose(); }
            Console.WriteLine($"PASS {resp}, {(broadcast ? "BCAST prefixes" : "OPTIN")}: repeated subscription restore, temporary reuse and invalidation");
        }
    }
    private static long Id(object connection, string name) => (long)connection.GetType().BaseType!.GetProperty(name, Hidden)!.GetValue(connection)!;
    private static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
    private static async Task Until(Func<bool> check)
    {
        using var timeout = new CancellationTokenSource(6000);
        while (!check()) await Task.Delay(10, timeout.Token);
    }
    private sealed class Cache(string prefix, bool broadcast) : ClientCachingBase
    {
        private readonly ConcurrentDictionary<string, RedisResult> _items = new();
        internal int Count => _items.Count;
        protected override ClientCachingMode Mode => broadcast ? ClientCachingMode.Broadcast : ClientCachingMode.Default;
        protected override string[]? KeyPrefixes => broadcast ? new[] { prefix } : null;
        protected override bool KeyMatching(string key) => key.StartsWith(prefix, StringComparison.Ordinal);
        protected override void Set(string key, in RedisResult value) => _items[key] = value;
        protected override bool TryGet(string key, [NotNullWhen(true)] out RedisResult? value)
        {
            if (_items.TryGetValue(key, out var result)) { value = result; return true; }
            value = null; return false;
        }
        protected override void Remove(string key) => _items.TryRemove(key, out _);
        protected override void Clear() => _items.Clear();
    }
}
