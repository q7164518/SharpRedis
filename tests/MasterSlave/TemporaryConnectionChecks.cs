using System.Reflection;
using SharpRedis;

internal static class TemporaryConnectionChecks
{
    internal static async Task Run(string host, int masterPort = 16379, int replicaPort = 16380)
    {
        foreach (var resp in new[] { Resp.RESP2, Resp.RESP3 })
        {
            Redis Create() => Redis.UseMasterSlave(o =>
            {
                o.Host = host; o.Port = masterPort; o.RespVersion = resp;
                o.HeartbeatInterval = 120000; o.CommandTimeout = 2000;
                o.Slaves.Add(new() { Host = host, Port = replicaPort });
            });
            using (var redis = Create())
            {
                var pool = Pool(redis);
                var key = "temporary-check:" + Guid.NewGuid().ToString("N");
                await redis.Stream.XReadAsync(key, "0-0", block: 20);
                Check(Count(pool) == 0, "Completed connection retained");
                using (var cancel = new CancellationTokenSource(100))
                    await Fails(async () => { await redis.Stream.XReadAsync(key, "0-0", block: 0, cancellationToken: cancel.Token); });
                Check(Count(pool) == 0, "Canceled connection retained");
                await Fails(() =>
                {
                    pool.GetType().GetMethod("GetTempConnection", new[] { typeof(int) })!.Invoke(pool, new object[] { -1 });
                    return Task.CompletedTask;
                });
                Check(Count(pool) == 0, "Failed SELECT connection retained");
                var pending = redis.Stream.XReadAsync(key, "0-0", block: 0);
                await Task.Delay(100);
                Check(Count(pool) == 1 && !pending.IsCompleted, "Blocking fixture not active");
                redis.Dispose();
                await Fails(async () => { await pending; });
                Check(Count(pool) == 0, "Shutdown connection retained");
            }
            for (var round = 0; round < 10; round++)
            {
                using var redis = Create();
                var pool = Pool(redis);
                var key = "temporary-race:" + Guid.NewGuid().ToString("N");
                var reads = Enumerable.Range(0, 12).Select(_ => Task.Run(async () =>
                {
                    try { await redis.Stream.XReadAsync(key, "0-0", block: 0); }
                    catch { }
                })).ToArray();
                await Task.Delay(round % 3);
                redis.Dispose();
                await Task.WhenAll(reads).WaitAsync(TimeSpan.FromSeconds(5));
                Check(Count(pool) == 0, "Creation/shutdown race retained connection");
            }
            Console.WriteLine($"PASS {resp}: temporary completion, cancellation, SELECT failure, active shutdown and concurrent creation/shutdown");
        }
    }
    private static object Pool(Redis redis)
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var context = typeof(Redis).BaseType!.GetField("_context", flags)!.GetValue(redis)!;
        return context.GetType().GetField("_connectionPool", flags)!.GetValue(context)!;
    }
    private static int Count(object pool)
    {
        var entries = pool.GetType().GetField("_temporaryConnections", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(pool)!;
        return (int)entries.GetType().GetProperty("Count")!.GetValue(entries)!;
    }
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    private static async Task Fails(Func<Task> action)
    {
        var failed = false;
        try { await action().WaitAsync(TimeSpan.FromSeconds(5)); }
        catch (TimeoutException) { throw; }
        catch { failed = true; }
        Check(failed, "Expected operation to fail");
    }
}
