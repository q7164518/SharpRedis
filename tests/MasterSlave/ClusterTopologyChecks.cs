using System.Diagnostics;
using SharpRedis;
using static ClusterChecks;

internal static class ClusterTopologyChecks
{
    private const string Password = "sharpredis-cluster-test";
    internal static async Task Run(string host)
    {
        string prefix = "cluster-topology:" + Guid.NewGuid().ToString("N") + ":";
        string key = prefix + "{migration}:value", counter = prefix + "{migration}:counter";
        var cache = new Cache(prefix, false);
        using var redis = Redis.UseCluster(false, o =>
        {
            o.Nodes.Add(new() { Host = host, Port = 18379 }); o.Password = Password;
            o.CommandTimeout = 1500; o.ClusterRefreshInterval = 120000;
            o.UseClientCaching(cache);
        });
        await redis.String.SetAsync(key, "original", TimeSpan.FromMinutes(5));
        int source = await ErrorPort(redis, key, CommandFlags.ForceMaster);
        string sourceId = await Cli(source, "CLUSTER", "MYID");
        int slot = int.Parse(await Cli(source, "CLUSTER", "KEYSLOT", key));
        var messages = new List<string>();
        await redis.PubSub.SubscribeAsync(prefix + "channel", (_, value) => { lock (messages) messages.Add(value.ToString()!); return Task.CompletedTask; });
        await Cli(source, "CLUSTER", "MEET", "127.0.0.1", "18385");
        string targetId = await Cli(18385, "CLUSTER", "MYID");
        await ClusterChecks.Until(async () => (await Cli(source, "CLUSTER", "NODES")).Contains(targetId));
        await ClusterChecks.Until(async () => (await Cli(18385, "CLUSTER", "INFO")).Contains("cluster_state:ok"));
        await Cli(18385, "CLUSTER", "SETSLOT", slot.ToString(), "IMPORTING", sourceId);
        await Cli(source, "CLUSTER", "SETSLOT", slot.ToString(), "MIGRATING", targetId);
        await Cli(source, "MIGRATE", "127.0.0.1", "18385", key, "0", "5000", "AUTH", Password);
        ClusterChecks.Check((string?)await redis.String.GetAsync(key) == "original", "ASK failed");
        ClusterChecks.Check(Nodes(Pool(redis)).Length == 3, "ASK permanently changed topology");
        var masters = await Masters(source);
        foreach (int port in masters.OrderBy(p => p == 18385 ? 0 : 1))
            await Cli(port, "CLUSTER", "SETSLOT", slot.ToString(), "NODE", targetId);
        using (var pipe = redis.StartPipelining())
        {
            await pipe.String.IncrAsync(counter);
            await pipe.String.GetAsync(key);
            var results = (await pipe.ExecutePipeliningAsync())!;
            ClusterChecks.Check((long)results[0] == 1 && (string?)results[1] == "original", "MOVED pipeline retry failed");
        }
        await ClusterChecks.Until(() => Task.FromResult(Nodes(Pool(redis)).Length == 4));
        ClusterChecks.Check((string?)await redis.String.GetAsync(counter) == "1", "Pipeline write was replayed");
        await Cli(18385, "EXPIRE", counter, "300");
        await redis.String.GetAsync(key);
        await Cli(18385, "SET", key, "new-owner", "EX", "300");
        await ClusterChecks.Until(async () => (string?)await redis.String.GetAsync(key) == "new-owner");
        await ClusterChecks.Until(async () => { await redis.PubSub.PublishAsync(prefix + "channel", "added"); lock (messages) return messages.Contains("added"); });
        Console.WriteLine("PASS real ASK, MOVED pipeline without duplicate writes, added slot owner, tracking and subscription recovery");

        // Return the slot and all test keys before removing the extra node.
        await Cli(source, "CLUSTER", "SETSLOT", slot.ToString(), "IMPORTING", targetId);
        await Cli(18385, "CLUSTER", "SETSLOT", slot.ToString(), "MIGRATING", sourceId);
        var keys = (await Cli(18385, "CLUSTER", "GETKEYSINSLOT", slot.ToString(), "1000")).Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (string movedKey in keys) await Cli(18385, "MIGRATE", "127.0.0.1", source.ToString(), movedKey.Trim(), "0", "5000", "AUTH", Password);
        foreach (int port in masters.OrderBy(p => p == source ? 0 : 1))
        {
            try { await Cli(port, "CLUSTER", "SETSLOT", slot.ToString(), "NODE", sourceId); }
            catch (Exception ex) when (port == 18385 && ex.Message.Contains("only with masters"))
            { /* Redis automatically demotes a master after its final slot is removed. */ }
        }
        await ClusterChecks.Until(async () => (string?)await redis.String.GetAsync(key) == "new-owner" && Nodes(Pool(redis)).Length == 3);
        foreach (int port in Enumerable.Range(18379, 6)) await Cli(port, "CLUSTER", "FORGET", targetId);
        await Cli(18385, "CLUSTER", "RESET", "HARD");
        await ClusterChecks.Until(async () => { await redis.PubSub.PublishAsync(prefix + "channel", "removed"); lock (messages) return messages.Contains("removed"); });
        Console.WriteLine("PASS removed slot owner: refreshed slot table, subscriptions and cache tracking");

        var crashCache = new Cache(prefix, true);
        using var failover = Redis.UseCluster(true, o =>
        {
            o.Nodes.Add(new() { Host = host, Port = source }); o.Password = Password;
            o.RespVersion = Resp.RESP3; o.CommandTimeout = 1500; o.ClusterRefreshInterval = 200; o.HeartbeatInterval = 500;
            o.UseClientCaching(crashCache);
        });
        await failover.String.SetAsync(key, "before-crash", TimeSpan.FromMinutes(5));
        await ClusterChecks.Until(async () => (string?)await failover.String.GetAsync(key, CommandFlags.ForceSlave) == "before-crash");
        await failover.String.GetAsync(key);
        var restored = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        await failover.PubSub.SubscribeAsync(prefix + "crash", (_, value) => { restored.TrySetResult(value.ToString()!); return Task.CompletedTask; });
        try
        {
            await Cli(source, "SHUTDOWN", "NOSAVE");
            await ClusterChecks.Until(async () =>
            {
                await failover.String.SetAsync(key, "after-crash", TimeSpan.FromMinutes(5));
                return (string?)await failover.String.GetAsync(key) == "after-crash";
            });
            int promoted = await ErrorPort(failover, key, CommandFlags.ForceMaster);
            ClusterChecks.Check(promoted != source, "Master did not change");
            await Cli(promoted, "SET", key, "external-after-crash", "EX", "300");
            await ClusterChecks.Until(async () => (string?)await failover.String.GetAsync(key) == "external-after-crash");
            await ClusterChecks.Until(async () => { await failover.PubSub.PublishAsync(prefix + "crash", "recovered"); return restored.Task.IsCompleted; });
            Console.WriteLine("PASS crashed master: replica promotion, writes, per-master tracking and Pub/Sub recovery");
        }
        finally
        {
            var start = new ProcessStartInfo("wslc") { UseShellExecute = false, CreateNoWindow = true };
            foreach (string arg in new[] { "exec", "sharpredis-cluster-test-20261117", "redis-server", $"/data/{source}/redis.conf" }) start.ArgumentList.Add(arg);
            using var process = Process.Start(start)!;
            await process.WaitForExitAsync();
            ClusterChecks.Check(process.ExitCode == 0, "Restart failed");
        }
    }
    private static async Task<int[]> Masters(int port)
        => (await Cli(port, "CLUSTER", "NODES")).Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(' ')).Where(fields => fields[2].Contains("master"))
            .Select(fields => int.Parse(fields[1].Split('@')[0].Split(':')[1])).ToArray();
}
