using System.Net.Sockets;
using System.Diagnostics;
using SharpRedis;

internal static class UnixSocketChecks
{
    internal static async Task RunTopologies()
    {
        int master = await SentinelMaster();
        int replica = master == 30101 ? 30102 : 30101;
        using (var redis = Redis.UseMasterSlave($"host=/tmp/redis-{master}.sock,port={master}",
            [$"host=/tmp/redis-{replica}.sock,port={replica}"], o => o.ConnectName = "unix-replica-test"))
        {
            await VerifyReplica(redis);
            await VerifyUnixClients("unix-replica-test", master, replica);
        }
        using (var sentinel = Redis.UseSentinel(true, $"host=/tmp/redis-{master}.sock,port={master},mastername=unixmaster",
            ["host=/tmp/redis-30103.sock,port=30103"], o =>
            {
                o.ConnectName = "unix-sentinel-test";
                o.SentinelRefreshInterval = 250;
                o.DataNodeUnixSockets[("127.0.0.1", replica)] = $"/tmp/redis-{replica}.sock";
            }))
        {
            await VerifyReplica(sentinel);
            var sentinelClients = (await Cli(30103, "CLIENT", "LIST")).Split('\n');
            ClusterChecks.Check(sentinelClients.Any(line => line.Contains("laddr=/tmp/redis-30103.sock:0 ")
                && !line.Contains("cmd=client|list ")), "Expected a Unix connection to Sentinel");
            await VerifyUnixClients("unix-sentinel-test", master, replica);
            try
            {
                await Cli(30103, "SENTINEL", "FAILOVER", "unixmaster");
                await ClusterChecks.Until(async () => await SentinelMaster() == replica);
                await ClusterChecks.Until(async () =>
                {
                    string key = "unix-promoted:" + Guid.NewGuid().ToString("N");
                    await sentinel.String.SetAsync(key, "promoted", TimeSpan.FromMinutes(1));
                    return await Cli(replica, "GET", key) == "promoted";
                });
                await VerifyUnixClients("unix-sentinel-test", replica);
            }
            finally
            {
                if (await SentinelMaster() != master)
                {
                    await ClusterChecks.Until(async () =>
                        (await Cli(master, "INFO", "replication")).Contains("master_link_status:up"));
                    await Cli(30103, "SENTINEL", "FAILOVER", "unixmaster");
                    await ClusterChecks.Until(async () => await SentinelMaster() == master);
                }
            }
        }
        using (var cluster = Redis.UseCluster(true, o =>
        {
            o.Host = "/tmp/redis-30110.sock";
            o.Port = 30110;
            o.ConnectName = "unix-cluster-test";
            o.NodeUnixSockets[("NODE.invalid", 30110)] = "/tmp/redis-30110.sock";
            for (int port = 30111; port <= 30115; port++)
                o.NodeUnixSockets[("127.0.0.1", port)] = $"/tmp/redis-{port}.sock";
        }))
        {
            const System.Reflection.BindingFlags hidden = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            var pool = ClusterChecks.Pool(cluster);
            var settings = pool.GetType().GetMethod("Settings", hidden)!;
            object? UnixEndpoint(string host)
            {
                var node = settings.Invoke(pool, [host, 30110])!;
                return node.GetType().GetProperty("UnixEndpoint", hidden)!.GetValue(node);
            }
            ClusterChecks.Check(UnixEndpoint("node.invalid")?.ToString() == "/tmp/redis-30110.sock",
                "Mapped DNS names must work without DNS and regardless of case");
            ClusterChecks.Check(UnixEndpoint("127.0.0.2") is null, "Distinct loopback IPs must not share a socket mapping");
            await VerifyReplica(cluster);
            await VerifyUnixClients("unix-cluster-test", Enumerable.Range(30110, 6).ToArray());
        }
        Console.WriteLine("PASS Unix sockets on master/replica, Sentinel discovery and failover, and all six Cluster nodes");
    }

    private static async Task VerifyReplica(Redis redis)
    {
        string key = "unix-replication:" + Guid.NewGuid().ToString("N");
        await redis.String.SetAsync(key, "replicated", TimeSpan.FromMinutes(2));
        await ClusterChecks.Until(async () =>
            (string?)await redis.String.GetAsync(key, CommandFlags.ForceSlave) == "replicated");
    }

    private static async Task VerifyUnixClients(string name, params int[] ports)
    {
        foreach (int port in ports)
        {
            var clients = (await Cli(port, "CLIENT", "LIST")).Split('\n')
                .Where(line => line.Contains(name, StringComparison.Ordinal)).ToArray();
            ClusterChecks.Check(clients.Length > 0 && clients.All(line => line.Contains($"laddr=/tmp/redis-{port}.sock:0 ")),
                $"Expected Unix connections at {port} for {name}");
        }
    }

    private static async Task<int> SentinelMaster()
        => int.Parse((await Cli(30103, "SENTINEL", "GET-MASTER-ADDR-BY-NAME", "unixmaster")).Split('\n')[1]);

    private static async Task<string> Cli(int port, params string[] args)
    {
        var start = new ProcessStartInfo("redis-cli") { RedirectStandardOutput = true, UseShellExecute = false };
        foreach (var arg in new[] { "--raw", "-s", $"/tmp/redis-{port}.sock" }.Concat(args))
            start.ArgumentList.Add(arg);
        using var process = Process.Start(start)!;
        string output = (await process.StandardOutput.ReadToEndAsync()).Trim();
        await process.WaitForExitAsync();
        ClusterChecks.Check(process.ExitCode == 0 && !output.StartsWith("ERR"), output);
        return output;
    }

    internal static async Task Run(string path)
    {
        var options = new StandaloneConnectionOptions("host=/tmp/test=a.sock,password=a=b==");
        ClusterChecks.Check(options.Host == "/tmp/test=a.sock" && options.Password == "a=b==", "Embedded equals sign");
        foreach (var resp in new[] { Resp.RESP2, Resp.RESP3 })
        {
            using var redis = resp == Resp.RESP2
                ? Redis.UseStandalone($"host={path}", o => o.RespVersion = resp)
                : Redis.UseStandalone(o => { o.Host = path; o.RespVersion = resp; });
            string key = "unix-check:" + Guid.NewGuid().ToString("N");
            using (var pipe = redis.StartPipelining())
            {
                pipe.String.Set(key, "unix", TimeSpan.FromMinutes(2));
                pipe.String.Get(key);
                ClusterChecks.Check((string?)pipe.ExecutePipelining()![1] == "unix", "Unix socket pipeline");
            }
            using (var db = redis.Select(1))
            {
                await db.String.SetAsync(key, "other-db", TimeSpan.FromMinutes(2));
                ClusterChecks.Check((string?)await db.String.GetAsync(key) == "other-db"
                    && (string?)await redis.String.GetAsync(key) == "unix", "Unix socket database switch");
            }
            await redis.Stream.XReadAsync(key + ":stream", "0-0", block: 25);

            var received = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            await redis.PubSub.SubscribeAsync(key, (_, value) =>
            {
                received.TrySetResult(value.ToString()!);
                return Task.CompletedTask;
            });
            await redis.PubSub.PublishAsync(key, "unix-message");
            ClusterChecks.Check(await received.Task.WaitAsync(TimeSpan.FromSeconds(5)) == "unix-message", "Unix socket subscription");
            Console.WriteLine($"PASS Unix socket {resp}: pipeline, SELECT, blocking command and Pub/Sub");
        }

        try
        {
            using var missing = Redis.UseStandalone($"unixsocket={path}.missing");
            throw new InvalidOperationException("Expected a connection error for a missing Unix socket");
        }
        catch (SocketException)
        {
            Console.WriteLine("PASS missing Unix socket fails without TCP fallback");
        }
    }
}
