# Sentinel mode

```csharp
using var redis = Redis.UseSentinel(true, options =>
{
    options.MasterName = "mymaster";
    // Credentials for the discovered primary and replicas.
    options.User = "default";
    options.Password = "redis-password";
    options.SlaveSelectionMode = SlaveSelectionMode.RoundRobin; // Also supports Random
    options.SentinelRefreshInterval = 1000; // Sentinel heartbeat and topology polling interval, in milliseconds
    options.SentinelCommandTimeout = 1000; // Sentinel connection and command timeout, in milliseconds
    options.CommandTimeout = 5000; // Redis data node connection and command timeout, in milliseconds

    foreach (var host in new[] { "sentinel-a", "sentinel-b", "sentinel-c" })
        options.Sentinels.Add(new StandaloneConnectionOptions
        {
            Host = host,
            Port = 26379,
            User = "default",
            Password = "sentinel-password",
        });

    // Register a client-side cache with options.UseClientCaching(cache), as in standalone and fixed replication modes.
});

await redis.String.SetAsync("example", "value");
var value = await redis.String.GetAsync("example");
```

You can also use `Redis.UseSentinel(enableReadWriteSeparation, masterConnectionString, sentinelConnectionStrings, optionsAction)`. Credentials in the Sentinel connection strings apply to the corresponding Sentinel instances. The `User` and `Password` set in the callback apply to the data nodes.

The first argument to `UseSentinel` is `enableReadWriteSeparation`. Set it to `true` to enable read/write separation. With `false`, the client skips replica discovery and connections and sends all operations to the primary, including commands with `ForceSlave`. Sentinel heartbeats, automatic primary failover, subscription recovery, and migration of client-side cache tracking remain active. Replica-list changes do not rebuild the connection pool in this mode.

`Sentinels` contains the discovery endpoints for a single `MasterName` service. Create a separate Redis client instance for each independent primary group. Data node addresses returned by Sentinel must be reachable from the client. The Redis servers manage replication and elections.

Sentinel mode uses the same rules as fixed primary and replica mode for read/write routing, round-robin and random selection, `CommandFlags`, pipelines, transactions, and temporary connections. When no replica is available, ordinary reads fall back to the primary and `ForceSlave` throws an exception. Reads eligible for client-side caching and invalidation subscriptions use the primary.

The client polls all configured Sentinel instances in the background. It checks availability with `PING`, queries the replication topology, and prefers newer configuration epochs. It uses `ROLE` to verify node roles when opening data connections. If Sentinel is unavailable, SharpRedis retains the last known topology and continues trying to connect. It uses the existing heartbeat mechanism to check data node availability and roles.

When the topology changes, SharpRedis stops the old connection pool, clears the cache, reconnects to the primary and replicas, and waits for regular and invalidation subscriptions to be acknowledged before updating the tracking redirect on the new primary. Caching is suspended during the switch, and requests that began before the switch cannot populate the cache afterward. SharpRedis makes the new pool available once recovery finishes. Clients using a different database can send subsequent commands to the new primary. Transactions already in progress fail and must be recreated.

Commands can throw connection or timeout exceptions during failure detection and recovery. SharpRedis leaves retry decisions for previously sent writes, pipelines, and transactions to the application, which can use operation idempotency to decide whether to retry. Reads from replicas are still subject to Redis asynchronous replication lag.

## Local integration tests

`dotnet run --project tests/MasterSlave -- --sentinel`

These tests target the dedicated `sharpredis-sentinel-test-20261117` container. The data nodes use ports 17379, 17380, and 17381. Sentinel uses ports 27379, 27380, and 27381. The primary group is `mymaster`. For local testing, the data node password is `sharpredis-data-test` and the Sentinel password is `sharpredis-sentinel-test`.

The tests cover both read/write separation settings, both registration overloads, RESP2 and RESP3, default and broadcast caching, manually triggered failover, primary failure while one Sentinel is unavailable, subscription and tracking migration, pipelines, transactions, and database switching. They change the primary in the dedicated cluster, restart the test process they stopped, and leave the container in place. Use `tests/MasterSlave/sentinel-fixture.sh` only to initialize an empty dedicated test container.
