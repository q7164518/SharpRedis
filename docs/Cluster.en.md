# Cluster mode

```csharp
using var redis = Redis.UseCluster(true, options =>
{
    options.Nodes.Add(new StandaloneConnectionOptions { Host = "127.0.0.1", Port = 7000 });
    options.Nodes.Add(new StandaloneConnectionOptions { Host = "127.0.0.1", Port = 7001 });
    options.User = "default";
    options.Password = "redis-password";
    options.CommandTimeout = 5000;
    options.ClusterRefreshInterval = 1000;
    options.SlaveSelectionMode = SlaveSelectionMode.RoundRobin; // Also supports Random

    // options.UseClientCaching(cache);
});

await redis.String.SetAsync("user:{42}:name", "Alice");
var name = await redis.String.GetAsync("user:{42}:name");
```

You can also use `Redis.UseCluster(enableReadWriteSeparation, nodeConnectionStrings, optionsAction)`. SharpRedis uses a single set of `User` and `Password` credentials for all discovered data nodes, replicas, and redirect targets. The connection-string overload initially reads credentials from the first seed node. After the configuration callback completes, SharpRedis validates that all explicitly configured node credentials match the shared credentials. A mismatch throws `ArgumentException` before any connection is established. Comparisons are case-sensitive, and the exception identifies the conflicting option without exposing its value. Missing node credentials inherit the shared settings. If a shared credential is unset, SharpRedis takes the first explicitly configured value in the node list. This is a SharpRedis configuration rule. Validation checks that the client settings agree, and each server still authenticates the credentials independently. Node addresses advertised by the server must be reachable from the client.

When the first argument is `false`, SharpRedis connects only to slot primaries and sends all operations to them, including commands with `ForceSlave`. With `true`, ordinary reads can use a round-robin or randomly selected replica of the corresponding primary via `READONLY`. Writes go to the primary. Reads that qualify for client-side caching go to the primary, while explicit `ForceSlave` requests bypass the cache. If no replica is available, ordinary reads fall back to the primary and `ForceSlave` throws an exception.

## Keys, pipelines, and transactions

Cluster mode uses database **0** exclusively. Configuring a different database fails before the client connects. `Select` and `Switch` are unavailable in cluster mode.

CRC16 is calculated over the complete key bytes sent to Redis, including `Prefix` and `Suffix`. SharpRedis supports string, character-memory, and binary keys, as well as Redis `{hash-tag}` rules.

All keys in a multi-key command must belong to **the same hash slot**, even if several slots are on the same node. SharpRedis uses the first key to determine the slot. At the first mismatch, it throws `ArgumentException` with a message starting with `CROSSSLOT`, stops reading keys, and leaves the command unsent.

The same rule applies across the **entire pipeline batch**. Every key from every command must belong to the same slot. SharpRedis validates the batch before sending any commands, so a cross-slot error cannot leave part of the batch executed. Commands without keys do not affect slot selection. A single-slot pipeline uses that slot's primary and preserves client-side cache lookup and batched sending.

```csharp
using var pipe = redis.StartPipelining();
await pipe.String.SetAsync("user:{42}:name", "Alice");
await pipe.String.SetAsync("user:{42}:city", "Shanghai");
var results = await pipe.ExecutePipeliningAsync();
```

`RedisTranClient` prepares cluster transactions and sends them through a dedicated execution path. The middleware that routes ordinary commands does not determine transaction types. The first key fixes the slot, and SharpRedis defers `MULTI` until it selects a connection. `WATCH`, `EXEC`, and `DISCARD` use the same connection. A cross-slot error invalidates the transaction connection and prevents the queued commands from being committed. If a command without keys is queued before the first key, the transaction binds to the default node and stays on it.

Ordinary commands without keys use a default primary. Commands such as `SCAN`, `KEYS`, `DBSIZE`, `INFO`, and `SCRIPT LOAD` run on that node without cluster-wide broadcasting or aggregation. Scripts must explicitly declare the keys they access and follow the same-slot rule. Regular Pub/Sub uses a dedicated subscription connection managed separately from cache invalidation subscriptions.

## Topology and failover

SharpRedis refreshes its slot routing table in the background with `CLUSTER SLOTS` and tries other known nodes if the seed node is unavailable. Routes follow topology changes reported by Redis, including node additions, removals, and role changes. The Redis servers must complete the necessary slot migrations when nodes are added or removed. Data redistribution and elections are handled by the Redis cluster.

SharpRedis handles `MOVED` and `ASK` and follows up to five redirects by default. Set `MaxRedirects` to change the limit. For `ASK`, SharpRedis sends `ASKING` on a dedicated temporary connection for that request. `MOVED` also triggers a topology refresh. A pipeline retries only the entries that explicitly returned redirect errors. Previously successful writes are left as executed. Operations with uncertain outcomes due to disconnections, timeouts, or transaction failures are left to the caller, who can decide whether a retry is safe based on idempotency.

A topology change replaces the connection pool as a whole, so commands or transactions in progress may fail. When a failed primary has an eligible replica and Redis completes an election, SharpRedis follows the new primary for the slot. If Redis lacks a recoverable replica, has an unassigned slot, or returns `CLUSTERDOWN`, SharpRedis returns the corresponding error.

## Client-side caching and connection lifecycle

Each primary maintains its own invalidation subscription and redirect ID. The cached data for all primaries uses the same user-registered `ClientCachingBase` instance. Both default and broadcast modes are supported. SharpRedis bypasses cache hits and skips populating the cache for nodes whose subscriptions are unavailable. It rebinds tracking after recovery. When the topology changes, SharpRedis clears the cache and prevents old connections from populating it. Caching resumes after subscriptions recover.

Cluster temporary connections are single-use. SharpRedis closes them when a command completes, is canceled, or fails. Disposing the pool also closes any remaining temporary connections. Slot information, node ownership, and transaction state are stored in `ClusterConnection` and the cluster connection pool, with the existing connection classes and `ICommand` retaining their current fields. All nodes share one command dispatcher and one cluster monitoring thread. Normal slot lookup and connection selection are lock-free.

## Dedicated integration tests

The `sharpredis-cluster-test-20261117` container is retained for future tests. Data ports 18379–18384 serve three primaries and three replicas, and port 18385 is used by the node addition and removal tests. The local test password is `sharpredis-cluster-test`.

```text
dotnet run --project tests/MasterSlave -- --cluster
dotnet run --project tests/MasterSlave -- --cluster-topology
```

The first suite covers CRC16, hash tags, read/write separation, RESP2 and RESP3, default and broadcast caching, subscription disconnections, early termination on cross-slot errors, pipelines that send zero commands on validation failure, transactions, and temporary connection disposal. The second suite migrates slots, adds and removes a test node, and stops and restarts a primary in the dedicated test cluster. It verifies `ASK`, `MOVED`, automatic failover, and cache and subscription recovery. Run the test setup script only against the dedicated test cluster.
