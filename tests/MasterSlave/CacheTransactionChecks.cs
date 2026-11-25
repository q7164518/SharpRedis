using System.Net.Sockets;
using System.Text;
using SharpRedis;
using SharpRedis.Exceptions;

internal static class CacheTransactionChecks
{
    internal static async Task Run(string host)
    {
        foreach (var masterSlave in new[] { false, true })
        foreach (var resp in new[] { Resp.RESP2, Resp.RESP3 })
        foreach (var broadcast in new[] { false, true })
        {
            var prefix = "cache-tran:" + Guid.NewGuid().ToString("N") + ":";
            using var redis = masterSlave ? Redis.UseMasterSlave(o =>
            {
                o.Host = host; o.Port = 16379; o.RespVersion = resp; o.CommandTimeout = 100;
                o.Slaves.Add(new() { Host = host, Port = 16380 });
                if (broadcast) o.UseClientCaching(new[] { prefix }, TimeSpan.FromMinutes(1)); else o.UseClientCaching(k => k.StartsWith(prefix), TimeSpan.FromMinutes(1));
            }) : Redis.UseStandalone(o =>
            {
                o.Host = host; o.Port = 16379; o.RespVersion = resp; o.CommandTimeout = 100;
                if (broadcast) o.UseClientCaching(new[] { prefix }, TimeSpan.FromMinutes(1)); else o.UseClientCaching(k => k.StartsWith(prefix), TimeSpan.FromMinutes(1));
            });
            try
            {
                await redis.Hash.HSetAsync(prefix + "a b", "c", "first");
                await redis.Hash.HSetAsync(prefix + "a", "b c", "second");
                Check((string?)await redis.Hash.HGetAsync(prefix + "a b", "c") == "first");
                Check((string?)redis.Hash.HGet(prefix + "a", "b c") == "second");
                byte[] field1 = [255], field2 = [254];
                await redis.Hash.HSetAsync(prefix + "binary", field1, "binary-first");
                await redis.Hash.HSetAsync(prefix + "binary", field2, "binary-second");
                Check((string?)await redis.Hash.HGetAsync(prefix + "binary", field1) == "binary-first");
                Check((string?)await redis.Hash.HGetAsync(prefix + "binary", field2) == "binary-second");
                await redis.String.SetAsync(prefix + "value", "bytes");
                await redis.String.SetAsync(prefix + "value [ReturnBytes]", "text");
                Check((await redis.String.GetAsync(prefix + "value", CommandFlags.ReturnBytes)).RawValue is byte[]);
                Check((string?)await redis.String.GetAsync(prefix + "value [ReturnBytes]") == "text");
                using (var pipe = redis.StartPipelining())
                {
                    await pipe.Hash.HGetAsync(prefix + "a", "b c");
                    await pipe.Hash.HGetAsync(prefix + "a b", "c");
                    var values = await pipe.ExecutePipeliningAsync();
                    Check((string?)values![0] == "second" && (string?)values[1] == "first");
                }
                foreach (var sync in new[] { false, true })
                {
                    using (var tran = await redis.MultiAsync())
                    {
                        await tran.String.SetAsync(prefix + "cancel", "must-not-commit");
                        using var cancel = new CancellationTokenSource(); cancel.Cancel();
                        await Throws<RedisCanceledException>(async () =>
                        {
                            if (sync) tran.Exec(cancellationToken: cancel.Token);
                            else await tran.ExecAsync(cancellationToken: cancel.Token);
                        });
                        await Throws<ObjectDisposedException>(() => { tran.Discard(); return Task.CompletedTask; });
                        await Throws<ObjectDisposedException>(async () => { await tran.MultiAsync(); });
                    }
                    using (var tran = await redis.MultiAsync())
                    {
                        await tran.String.SetAsync(prefix + "timeout", "unknown");
                        await Pause(host);
                        await Throws<RedisTimeoutException>(async () =>
                        {
                            if (sync) tran.Exec(); else await tran.ExecAsync();
                        });
                        await Throws<ObjectDisposedException>(() => { tran.Discard(); return Task.CompletedTask; });
                        await Task.Delay(500);
                    }
                }
                using (var tran = await redis.MultiAsync())
                {
                    await tran.String.SetAsync(prefix + "ok", "committed");
                    await tran.ExecAsync();
                    Check(await tran.MultiAsync());
                    Check(await tran.DiscardAsync());
                }
                Console.WriteLine($"PASS masterSlave={masterSlave}, {resp}, broadcast={broadcast}: cache collisions, pipeline, EXEC cancellation/timeout and normal reuse");
            }
            finally
            {
                // Random test keys only; never flush the test server.
                foreach (var suffix in new[] { "a b", "a", "binary", "value", "value [ReturnBytes]", "cancel", "timeout", "ok" })
                    await redis.Key.DelAsync(prefix + suffix);
            }
        }
    }
    private static async Task Pause(string host)
    {
        using var tcp = new TcpClient(); await tcp.ConnectAsync(host, 16379);
        await tcp.GetStream().WriteAsync(Encoding.ASCII.GetBytes("*4\r\n$6\r\nCLIENT\r\n$5\r\nPAUSE\r\n$3\r\n400\r\n$3\r\nALL\r\n"));
        using var reader = new StreamReader(tcp.GetStream());
        Check(await reader.ReadLineAsync() == "+OK");
    }
    private static void Check(bool ok) { if (!ok) throw new Exception("Regression assertion failed"); }
    private static async Task Throws<T>(Func<Task> action) where T : Exception
    {
        try { await action(); } catch (T) { return; }
        throw new Exception("Expected " + typeof(T).Name);
    }
}
