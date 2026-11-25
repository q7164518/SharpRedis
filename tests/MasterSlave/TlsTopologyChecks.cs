using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using SharpRedis;

internal static class TlsTopologyChecks
{
    private const string Address = "host=127.0.0.1,ssl=true";
    private const string DataPassword = "tls-data-test";
    private const string SentinelPassword = "tls-sentinel-test";

    internal static async Task Run()
    {
        using var ca = X509Certificate2.CreateFromPem(File.ReadAllText("ssl/ca.pem"));
        using var client = X509CertificateLoader.LoadPkcs12FromFile(
            "ssl/client.pfx", "sharpredis-ssl-test", X509KeyStorageFlags.DefaultKeySet);

        void Configure(BasalConnectionOptions options)
        {
            options.ClientCertificates.Add(client);
            options.CertificateValidation = (_, cert, chain, errors) => StandaloneTlsChecks.Validate(ca, cert, chain, errors);
        }

        int master = await SentinelMaster();
        int replica = master == 30001 ? 30002 : 30001;
        string key = "tls-topology:" + Guid.NewGuid().ToString("N");
        using (var redis = Redis.UseStandalone(Address + ",port=30000", Configure))
        {
            await redis.String.SetAsync(key, "standalone", TimeSpan.FromMinutes(2));
            ClusterChecks.Check((string?)await redis.String.GetAsync(key) == "standalone", "Standalone TLS");
        }
        using (var check = Redis.UseCluster(false, [Address + ",port=30010",
            "host=LOCALHOST,port=30010,sslHost=seed.example,checkCertificateRevocation=true"], Configure))
        {
            const System.Reflection.BindingFlags hidden = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            var pool = ClusterChecks.Pool(check);
            var settings = pool.GetType().GetMethod("Settings", hidden)!.Invoke(pool, ["localhost", 30010])!;
            var tls = settings.GetType().GetProperty("Tls", hidden)!.GetValue(settings)!;
            ClusterChecks.Check((string?)tls.GetType().GetProperty("Host", hidden)!.GetValue(tls) == "seed.example"
                && (bool)tls.GetType().GetProperty("CheckCertificateRevocation", hidden)!.GetValue(tls)!,
                "Case-insensitive seed TLS overrides and revocation setting");
        }
        using (var redis = Redis.UseMasterSlave(Address + $",port={master},password={DataPassword}",
            [$"host=127.0.0.1,port={replica},password={DataPassword}"], Configure))
        {
            await VerifyReplica(redis, key, "master-slave");
            await redis.Stream.XReadAsync(key + ":stream", "0-0", block: 30);
            Console.WriteLine("PASS master/replica TLS inheritance and temporary blocking connection");
        }
        using (var sentinel = Redis.UseSentinel(true, Address + $",mastername=tlsmaster,password={DataPassword}",
            [Address + $",port=30003,password={SentinelPassword}"], o =>
            {
                Configure(o);
                Configure(o.Sentinels[0]);
                o.SentinelRefreshInterval = 250;
                o.CommandTimeout = 1500;
            }))
        {
            await VerifyReplica(sentinel, key, "sentinel");
            try
            {
                await Cli(30003, SentinelPassword, "SENTINEL", "FAILOVER", "tlsmaster");
                await ClusterChecks.Until(async () => await SentinelMaster() == replica);
                await ClusterChecks.Until(async () => await ClusterChecks.ErrorPort(sentinel, key, CommandFlags.ForceMaster) == replica);
                await sentinel.String.SetAsync(key, "sentinel-promoted", TimeSpan.FromMinutes(2));
                ClusterChecks.Check((string?)await sentinel.String.GetAsync(key, CommandFlags.ForceMaster) == "sentinel-promoted",
                    "Sentinel promoted master TLS");
                Console.WriteLine("PASS Sentinel TLS discovery, separate authentication and failover");
            }
            finally
            {
                if (await SentinelMaster() != master)
                {
                    await ClusterChecks.Until(async () =>
                        (await Cli(master, DataPassword, "INFO", "replication")).Contains("master_link_status:up"));
                    await Cli(30003, SentinelPassword, "SENTINEL", "FAILOVER", "tlsmaster");
                    await ClusterChecks.Until(async () => await SentinelMaster() == master);
                }
            }
        }
        using (var cluster = Redis.UseCluster(true, [Address + ",port=30010"], o =>
        {
            Configure(o);
            o.ClusterRefreshInterval = 120000;
        }))
        {
            await VerifyReplica(cluster, key, "cluster");
            int owner = await ClusterChecks.ErrorPort(cluster, key, CommandFlags.ForceMaster);
            ClusterChecks.Check(owner is >= 30010 and <= 30012, "TLS cluster discovery");
            int target = owner == 30010 ? 30011 : 30010;
            var slot = await Cli(owner, null, "CLUSTER", "KEYSLOT", key);
            var ownerId = await Cli(owner, null, "CLUSTER", "MYID");
            var targetId = await Cli(target, null, "CLUSTER", "MYID");
            await Cli(target, null, "CLUSTER", "SETSLOT", slot, "IMPORTING", ownerId);
            try
            {
                await Cli(owner, null, "CLUSTER", "SETSLOT", slot, "MIGRATING", targetId);
                await Cli(owner, null, "MIGRATE", "127.0.0.1", target.ToString(), key, "0", "5000");
                ClusterChecks.Check((string?)await cluster.String.GetAsync(key, CommandFlags.ForceMaster) == "cluster",
                    "TLS cluster ASK");
            }
            finally
            {
                await Cli(owner, null, "CLUSTER", "SETSLOT", slot, "STABLE");
                await Cli(target, null, "CLUSTER", "SETSLOT", slot, "STABLE");
            }
            Console.WriteLine("PASS cluster TLS discovery from one seed, replica read and ASK redirect");

            string failoverKey = key + ":failover";
            await VerifyReplica(cluster, failoverKey, "cluster-before-failover");
            int oldMaster = await ClusterChecks.ErrorPort(cluster, failoverKey, CommandFlags.ForceMaster);
            string oldMasterId = await Cli(oldMaster, null, "CLUSTER", "MYID");
            var replicaInfo = await Cli(oldMaster, null, "CLUSTER", "REPLICAS", oldMasterId);
            int newMaster = int.Parse(replicaInfo.Split(' ')[1].Split('@')[0].Split(':')[1]);
            try
            {
                await Cli(newMaster, null, "CLUSTER", "FAILOVER");
                await WaitForRole(newMaster, "master");
                // The normal topology interval has not elapsed. This write follows MOVED over TLS.
                await cluster.String.SetAsync(failoverKey, "cluster-after-failover", TimeSpan.FromMinutes(2));
                await ClusterChecks.Until(async () =>
                    await ClusterChecks.ErrorPort(cluster, failoverKey, CommandFlags.ForceMaster) == newMaster);
                ClusterChecks.Check((string?)await cluster.String.GetAsync(failoverKey, CommandFlags.ForceMaster)
                    == "cluster-after-failover", "TLS cluster promoted master");
                Console.WriteLine("PASS cluster TLS MOVED redirect and topology refresh");
            }
            finally
            {
                if ((await Cli(newMaster, null, "ROLE")).StartsWith("master"))
                {
                    await WaitForRole(oldMaster, "slave");
                    await Cli(oldMaster, null, "CLUSTER", "FAILOVER");
                    await WaitForRole(oldMaster, "master");
                }
            }
        }
    }

    private static Task WaitForRole(int port, string role)
        => ClusterChecks.Until(async () => (await Cli(port, null, "ROLE")).StartsWith(role + "\n", StringComparison.Ordinal));

    private static async Task VerifyReplica(Redis redis, string key, string value)
    {
        await redis.String.SetAsync(key, value, TimeSpan.FromMinutes(2));
        await ClusterChecks.Until(async () => (string?)await redis.String.GetAsync(key, CommandFlags.ForceSlave) == value);
    }

    private static async Task<int> SentinelMaster()
        => int.Parse((await Cli(30003, SentinelPassword, "SENTINEL", "get-master-addr-by-name", "tlsmaster")).Split('\n')[1]);

    private static async Task<string> Cli(int port, string? password, params string[] command)
    {
        var start = new ProcessStartInfo("wslc") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        foreach (var arg in new[] { "exec", "sharpredis-tls-topologies-20261119", "redis-cli", "--raw",
            "--tls", "--cacert", "/tls/ca.pem", "--cert", "/tls/client.pem", "--key", "/tls/client-key.pem", "-p", port.ToString() })
            start.ArgumentList.Add(arg);
        if (password is not null)
        {
            start.ArgumentList.Add("--no-auth-warning");
            start.ArgumentList.Add("-a");
            start.ArgumentList.Add(password);
        }
        foreach (var arg in command) start.ArgumentList.Add(arg);
        using var process = Process.Start(start)!;
        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        var output = (await outputTask).Trim();
        var error = await errorTask;
        ClusterChecks.Check(process.ExitCode == 0 && !output.StartsWith("ERR"), output + error);
        return output;
    }
}
