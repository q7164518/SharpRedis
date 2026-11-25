using System.Reflection;
using SharpRedis;
using SharpRedis.Exceptions;

// Run only against an isolated master on 16379 and replicas on 16380/16381.
// All written keys use a random prefix and expire; no FLUSH or replication changes are issued here.
var host = Environment.GetEnvironmentVariable("SHARPREDIS_TEST_HOST") ?? "127.0.0.1";
if (args.Contains("--call-keys"))
{
    using var redis = Redis.UseStandalone(o => { o.Host = host; o.Port = 30020; o.Prefix = Guid.NewGuid().ToString("N"); });
    SharpValue[] keys = Enumerable.Range(0, 11).Select(i => (SharpValue)i.ToString()).ToArray();
    keys[10] = new byte[] { 0, 255 };
    redis.Call(["SET", "KEYS[10]".AsMemory(), "prefix-KEYS[0]", "EX", "60"], keys);
    ClusterChecks.Check((string?)await redis.String.GetAsync(keys[10]) == "prefix-KEYS[0]", "Call must mark the substituted binary key");
    var results = (RedisResult[])(await redis.CallAsync(["MGET", "KEYS[0]", "KEYS[10]"u8.ToArray(), "KEYS[10]"], keys))!;
    ClusterChecks.Check(results[0].IsNullOrEmpty && (string?)results[1] == "prefix-KEYS[0]" && (string?)results[2] == "prefix-KEYS[0]", "CallAsync key positions");
    foreach (var placeholder in new[] { "KEYS[11]", "KEYS[4294967296]" })
    {
        try { redis.Call(["GET", placeholder], keys); throw new Exception("Expected invalid key index"); }
        catch (ArgumentOutOfRangeException) { }
    }
    Console.WriteLine("PASS Call and CallAsync key placeholders");
    return;
}
if (args is ["--unix-socket", var socketPath])
{
    await UnixSocketChecks.Run(socketPath);
    return;
}
if (args is ["--unix-topologies"])
{
    await UnixSocketChecks.RunTopologies();
    return;
}
if (args.Contains("--standalone-tls"))
{
    await StandaloneTlsChecks.Run();
    return;
}
if (args.Contains("--topologies-tls"))
{
    await TlsTopologyChecks.Run();
    await TemporaryConnectionChecks.Run("127.0.0.1", 30020, 30021);
    return;
}
if (args.Contains("--cluster-credentials"))
{
    ClusterCredentialChecks.Run();
    return;
}
if (args.Contains("--cluster-topology"))
{
    await ClusterTopologyChecks.Run(host);
    return;
}
if (args.Contains("--cluster"))
{
    await ClusterChecks.Run(host);
    return;
}
if (args.Contains("--sentinel"))
{
    await SentinelChecks.Run(host);
    return;
}
if (args.Contains("--cache-availability"))
{
    await CacheAvailabilityChecks.Run(host);
    return;
}
if (args.Contains("--cache-transaction"))
{
    await CacheTransactionChecks.Run(host);
    return;
}
if (args.Contains("--standalone-tracking"))
{
    await StandaloneTrackingChecks.Run(host);
    return;
}
if (args.Contains("--temporary-lifecycle"))
{
    await TemporaryConnectionChecks.Run(host);
    return;
}
if (args.Contains("--dispatcher-grouping"))
{
    await DispatcherGroupingChecks.Run(host);
    return;
}
var prefix = "sharpredis:master-slave:" + Guid.NewGuid().ToString("N");
using var master = Redis.UseStandalone(o => { o.Host = host; o.Port = 16379; });
using var slave1 = Redis.UseStandalone(o => { o.Host = host; o.Port = 16380; });
using var slave2 = Redis.UseStandalone(o => { o.Host = host; o.Port = 16381; });
Check(await master.String.SetAsync(prefix, "value", TimeSpan.FromMinutes(5)), "Seed SET failed");
await Until(async () => (string?)await slave1.String.GetAsync(prefix) == "value" && (string?)await slave2.String.GetAsync(prefix) == "value");
foreach (var mode in new[] { SlaveSelectionMode.RoundRobin, SlaveSelectionMode.Random })
foreach (var resp in new[] { Resp.RESP2, Resp.RESP3 })
{
    using var redis = Create(mode, resp, 120000);
    Check(redis.RedisMode == RedisServerMode.MasterSlave, "Wrong server mode");
    var middlewares = (System.Collections.IDictionary)Field(Context(redis), "_middlewares");
    Check(middlewares.Contains("MasterSlaveSelectConnection") && !middlewares.Contains("StandaloneSelectConnection"), "Wrong selector registration");
    var ports = new List<int>();
    for (int i = 0; i < 24; i++) ports.Add(await WrongTypePort(redis));
    Check(ports.All(p => p is 16380 or 16381) && ports.Distinct().Count() == 2, "Reads not distributed across replicas");
    if (mode == SlaveSelectionMode.RoundRobin) Check(ports.Zip(ports.Skip(1)).All(pair => pair.First != pair.Second), "Round robin did not alternate");
    Check(await WrongTypePort(redis, CommandFlags.ForceMaster) == 16379, "ForceMaster failed");
    Check(await WrongTypePort(redis, CommandFlags.ForceSlave) is 16380 or 16381, "ForceSlave failed");
    Check(await redis.String.SetAsync(prefix + ":write", "master", TimeSpan.FromMinutes(5)), "Write failed");
    await Throws<ArgumentException>(async () => { await redis.String.SetAsync(prefix + ":forbidden", "x", flags: CommandFlags.ForceSlave); });
    await Throws<ArgumentException>(async () => { await redis.String.GetAsync(prefix, CommandFlags.ForceSlave | CommandFlags.ForceMaster); });
    using (var db = redis.Select(1))
    {
        Check(await db.String.SetAsync(prefix, "db1", TimeSpan.FromMinutes(5)), "Switched DB write failed");
        Check((string?)db.String.Get(prefix, CommandFlags.ForceMaster) == "db1", "Synchronous switched DB failed");
        Check((string?)await db.String.GetAsync(prefix, CommandFlags.ForceMaster) == "db1", "Switched DB master read failed");
        await Until(async () => (string?)await db.String.GetAsync(prefix, CommandFlags.ForceSlave) == "db1");
        using var pipe = db.StartPipelining();
        await pipe.String.GetAsync(prefix);
        Check((string?)((await pipe.ExecutePipeliningAsync())![0]) == "db1", "Switched pipeline wrong DB");
    }
    // Switched DB operations must leave no reusable/leased temporary connections behind.
    Check(LeaseCount(redis) == 0, "Switched DB temporary connections retained");
    using (var pipe = redis.StartPipelining())
    {
        await pipe.String.GetAsync(prefix);
        Check((string?)((await pipe.ExecutePipeliningAsync(CommandFlags.ForceSlave))![0]) == "value", "Read pipeline failed");
        await pipe.String.SetAsync(prefix + ":pipe", "p", TimeSpan.FromMinutes(5));
        await pipe.String.GetAsync(prefix + ":pipe");
        Check((string?)((await pipe.ExecutePipeliningAsync())![1]) == "p", "Mixed pipeline not sent to master");
        await pipe.String.SetAsync(prefix + ":reject-pipe", "no", TimeSpan.FromMinutes(5));
        await Throws<ArgumentException>(async () => { await pipe.ExecutePipeliningAsync(CommandFlags.ForceSlave); });
        await pipe.String.GetAsync(prefix);
        Check((string?)((await pipe.ExecutePipeliningAsync())![0]) == "value", "Pipeline did not recover from routing failure");
    }
    using (var transaction = await redis.MultiAsync())
    {
        await transaction.String.SetAsync(prefix + ":transaction", "tx", TimeSpan.FromMinutes(5));
        await transaction.String.GetAsync(prefix + ":transaction");
        var result = await transaction.ExecAsync();
        Check(result.Queued?.Length == 2 && (string?)result.Result![1] == "tx", "Transaction not pinned to master");
    }
    var message = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
    await redis.PubSub.SubscribeAsync(prefix + ":channel", (_, value) => { message.TrySetResult(value.ToString()!); return Task.CompletedTask; });
    await redis.PubSub.PublishAsync(prefix + ":channel", "hello");
    Check(await message.Task.WaitAsync(TimeSpan.FromSeconds(3)) == "hello", "PubSub failed");
    using (var cancel = new CancellationTokenSource(150))
        await Throws<RedisCanceledException>(async () => { await redis.Stream.XReadAsync(prefix + ":stream", "0-0", block: 0, cancellationToken: cancel.Token); });
    Check(TempCount(redis) == 0, "Canceled temporary connection retained");
    await Task.WhenAll(Enumerable.Range(0, 40).Select(async _ =>
    {
        for (int i = 0; i < 25; i++) Check((string?)await redis.String.GetAsync(prefix) == "value", "Concurrent GET mismatch");
    }));
    var slots = Slots(redis);
    Invalidate(slots[2]);
    Check(await WrongTypePort(redis) == 16381, "Unavailable slave not skipped");
    Invalidate(slots[3]);
    Check(await WrongTypePort(redis) == 16379, "No-slave fallback failed");
    await Throws<RedisInitializationException>(async () => { await redis.String.GetAsync(prefix, CommandFlags.ForceSlave); });
    Invalidate(slots[0]);
    Check((string?)await redis.String.GetAsync(prefix, CommandFlags.ForceMaster) == "value", "Master temporary fallback failed");
    Console.WriteLine($"PASS {mode}, {resp}: routing, flags, DB, pipeline, transaction, PubSub, cancellation and fallback");
}
// Short heartbeat: each replaced replica must reconnect to its own endpoint; subscriptions must be restored.
using (var redis = Create(SlaveSelectionMode.RoundRobin, Resp.RESP3, 150))
{
    var channel = prefix + ":restore";
    var delivered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    await redis.PubSub.SubscribeAsync(channel, (_, _) => { delivered.TrySetResult(); return Task.CompletedTask; });
    var slots = Slots(redis);
    var old = slots.ToArray();
    foreach (var connection in old) Invalidate(connection);
    await Until(() => Task.FromResult(Enumerable.Range(0, slots.Length).All(i => !ReferenceEquals(old[i], slots[i]))));
    Check(await WrongTypePort(redis) is 16380 or 16381, "Replica reconnect used wrong endpoint");
    await redis.PubSub.PublishAsync(channel, "restored");
    await delivered.Task.WaitAsync(TimeSpan.FromSeconds(3));
    var pending = redis.Stream.XReadAsync(prefix + ":shutdown", "0-0", block: 0);
    await Task.Delay(100);
    redis.Dispose();
    await Throws<RedisException>(async () => { await pending.WaitAsync(TimeSpan.FromSeconds(3)); });
    Console.WriteLine("PASS heartbeat reconnect, subscription restore and disposal of active temporary connections");
}
using (var redis = Redis.UseMasterSlave($"host={host},port=16379", new[] { $"host={host},port=16380", $"host={host},port=16381" }))
    Check((string?)redis.String.Get(prefix) == "value", "Connection string overload failed");
await Throws<ArgumentException>(() => { Redis.UseMasterSlave(_ => { }); return Task.CompletedTask; });
await MasterSlaveCachingChecks.Run(host);
await Throws<ArgumentOutOfRangeException>(() => { Redis.UseMasterSlave(o => { o.Slaves.Add(new()); o.SlaveSelectionMode = (SlaveSelectionMode)255; }); return Task.CompletedTask; });
// One invalid replica must not prevent startup or hide healthy replicas.
using (var degraded = Redis.UseMasterSlave(o =>
{
    o.Host = host; o.Port = 16379; o.CommandTimeout = 300; o.HeartbeatInterval = 120000;
    o.Slaves.Add(new() { Host = host, Port = 16380, Password = "intentionally-invalid" });
    o.Slaves.Add(new() { Host = host, Port = 16381 });
}))
    Check(await WrongTypePort(degraded) == 16381, "Startup did not skip failed replica");
Console.WriteLine("All master/slave integration checks passed.");

Redis Create(SlaveSelectionMode mode, Resp resp, int heartbeat) => Redis.UseMasterSlave(o =>
{
    o.Host = host; o.Port = 16379; o.CommandTimeout = 2500; o.HeartbeatInterval = heartbeat; o.ReconnectInterval = 100;
    o.SlaveSelectionMode = mode; o.RespVersion = resp;
    o.Slaves.Add(new() { Host = host, Port = 16380 }); o.Slaves.Add(new() { Host = host, Port = 16381 });
});
async Task<int> WrongTypePort(Redis redis, CommandFlags flags = CommandFlags.None)
{
    try { await redis.Hash.HGetAsync(prefix, "field", flags); throw new Exception("Expected WRONGTYPE"); }
    catch (RedisErrorException ex) { return ex.Port; }
}
static object Field(object target, string name) => target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(target)!;
static object Context(Redis redis) => typeof(Redis).BaseType!.GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(redis)!;
static int TempCount(Redis redis)
{
    var leases = (System.Collections.IEnumerable)Field(Field(Context(redis), "_connectionPool"), "_leasedConnections");
    return leases.Cast<object>().Count(c => c.GetType().Name == "TempConnection");
}
static int LeaseCount(Redis redis)
{
    var leases = Field(Field(Context(redis), "_connectionPool"), "_leasedConnections");
    return (int)leases.GetType().GetProperty("Count")!.GetValue(leases)!;
}
static object[] Slots(Redis redis) => (object[])Field(Field(Context(redis), "_connectionPool"), "_connections");
static void Invalidate(object connection) => connection.GetType().BaseType!.GetMethod("Invalidate", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(connection, null);
static void Check(bool value, string message) { if (!value) throw new Exception(message); }
static async Task Throws<T>(Func<Task> action) where T : Exception
{
    try { await action(); } catch (T) { return; }
    throw new Exception("Expected " + typeof(T).Name);
}
static async Task Until(Func<Task<bool>> predicate)
{
    using var timeout = new CancellationTokenSource(12000);
    while (!await predicate()) await Task.Delay(30, timeout.Token);
}
