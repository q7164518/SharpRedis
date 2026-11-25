using SharpRedis;

internal static class DispatcherGroupingChecks
{
    internal static async Task Run(string host)
    {
        foreach (var masterSlave in new[] { false, true })
        foreach (var resp in new[] { Resp.RESP2, Resp.RESP3 })
        foreach (var batch in new[] { 1, 3, 80, 300 })
        {
            using var redis = masterSlave ? Redis.UseMasterSlave(o =>
            {
                o.Host = host; o.Port = 16379; o.RespVersion = resp; o.BatchCount = batch;
                o.CommandTimeout = 10000;
                o.Slaves.Add(new() { Host = host, Port = 16380 });
                o.Slaves.Add(new() { Host = host, Port = 16381 });
            }) : Redis.UseStandalone(o =>
            {
                o.Host = host; o.Port = 16379; o.RespVersion = resp; o.BatchCount = batch;
                o.CommandTimeout = 10000;
            });
            // Distinct connections, one shared dispatcher, interleaved commands.
            using var a = redis.Select(1);
            using var b = redis.Select(2);
            using var c = redis.Select(3);
            var key = "grouping:" + Guid.NewGuid().ToString("N");
            await a.String.SetAsync(key, "0", TimeSpan.FromMinutes(1));
            await b.String.SetAsync(key, "0", TimeSpan.FromMinutes(1));
            await c.String.SetAsync(key, "0", TimeSpan.FromMinutes(1));
            var tasks = new List<Task<RedisNumber>>();
            for (int i = 0; i < 400; i++)
            {
                tasks.Add(a.String.IncrAsync(key));
                tasks.Add(b.String.IncrAsync(key));
                tasks.Add(c.String.IncrAsync(key));
            }
            var values = await Task.WhenAll(tasks).WaitAsync(TimeSpan.FromSeconds(15));
            for (int i = 0; i < values.Length; i++)
                if ((long)values[i] != i / 3 + 1) throw new Exception("Per-connection order or response mismatch");
            using var canceled = new CancellationTokenSource(); canceled.Cancel();
            try { await a.String.IncrAsync(key, cancellationToken: canceled.Token); throw new Exception("Cancellation ignored"); }
            catch (OperationCanceledException) { }
            if ((long)await a.String.IncrAsync(key) != 401) throw new Exception("Canceled command was sent");
            await a.Key.DelAsync(key); await b.Key.DelAsync(key); await c.Key.DelAsync(key);
            Console.WriteLine($"PASS masterSlave={masterSlave}, {resp}, BatchCount={batch}: interleaved FIFO, response matching and cancellation");
        }
    }
}
