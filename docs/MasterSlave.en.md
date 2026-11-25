# Read/write separation

Configure a primary and replicas with replication already established. SharpRedis manages connections, routing, heartbeats, and reconnection. Redis replication setup and failover are outside the scope of this mode.

```csharp
using var redis = Redis.UseMasterSlave(options =>
{
    // Primary
    options.Host = "127.0.0.1";
    options.Port = 6379;
    options.Password = "master-password";
    options.Database = 0;

    // RoundRobin is the default. Random is also available.
    options.SlaveSelectionMode = SlaveSelectionMode.RoundRobin;
    options.Slaves.Add(new StandaloneConnectionOptions
    {
        Host = "127.0.0.1", Port = 6380, Password = "slave-password"
    });
    options.Slaves.Add(new StandaloneConnectionOptions
    {
        Host = "127.0.0.1", Port = 6381, Password = "slave-password"
    });
});

await redis.String.SetAsync("key", "value"); // Primary
var value = await redis.String.GetAsync("key"); // Replica
var latest = await redis.String.GetAsync("key", CommandFlags.ForceMaster);
```

Connection strings use the same comma-separated format as standalone mode.

```csharp
using var redis = Redis.UseMasterSlave(
    "host=127.0.0.1,port=6379,password=master-password",
    new[]
    {
        "host=127.0.0.1,port=6380,password=slave-password",
        "host=127.0.0.1,port=6381,password=slave-password"
    },
    options => options.SlaveSelectionMode = SlaveSelectionMode.Random);
```

Replica entries use only `Host`, `Port`, `User`, and `Password`. Configure each replica's credentials separately. The outer `MasterSlaveConnectionOptions` supplies shared settings such as the database, RESP version, encoding, key prefixes and suffixes, and timeouts.

## Routing rules

- Known read-only commands go to a replica. Writes and commands without a read-only flag go to the primary.
- Round-robin selection skips unavailable replicas. Random selection gives each available replica an equal chance.
- `ForceMaster` routes a command to the primary. `ForceSlave` is valid only for known read-only commands and is mutually exclusive with `ForceMaster`.
- If all replicas are unavailable, ordinary reads fall back to the primary. `ForceSlave` returns an error.
- If the initial connection to the primary fails, client setup fails. If an initial replica connection fails, SharpRedis skips that replica and retries its configured address during heartbeat checks. The configured primary stays fixed.
- An entirely read-only pipeline goes to a replica. A pipeline containing a write or an unknown command goes to the primary as a batch. Batch and per-command routing flags must agree.
- A transaction stays on a dedicated primary connection. Operations explicitly bound to a connection stay on it, and routing flags cannot move transaction commands to another connection.
- Subscriptions use a dedicated connection to the primary. After reconnecting, the client waits for resubscription acknowledgments.
- Blocking commands, non-default database operations, and temporary replacements for failed primary connections use single-use connections that are released on completion, cancellation, or failure. Ordinary operations on `redis.Select(db)` still follow the read/write routing rules. Transactions stay on the primary and use the specified database.
- Replication can lag. Use `ForceMaster` when you need to read the latest value immediately after writing.

Each connection pool shares one sending thread. Acquiring a healthy primary connection is lock-free. Round-robin selection across replicas adds one atomic counter increment, and random selection uses a thread-local random number generator.

## Client-side caching

You can use `UseClientCaching` in a `UseMasterSlave` configuration callback.

```csharp
options.UseClientCaching(key => key.StartsWith("cached:", StringComparison.Ordinal));
// Or match by prefix. options.UseClientCaching(new[] { "cached:" });
```

- A read-only command eligible for caching returns the cached result on a hit. On a miss, it reads from the primary and caches the result. Other read-only commands follow the configured replica routing.
- Matching uses the full key, including `Prefix` and `Suffix`. All keys in a multi-key command must match for it to be cached. Commands without keys are excluded.
- Broadcast tracking and invalidation subscriptions run only on the primary. With a custom predicate, the primary broadcasts invalidations for all keys and the client applies the filter. Prefix mode subscribes to the configured prefixes.
- Calling `UseClientCaching()` without arguments matches all keys, so all cacheable reads use the primary.
- Explicit `ForceMaster` or `ForceSlave` requests bypass the local cache and preserve the requested routing.
- If a pipeline contains a cacheable command, the entire batch goes to the primary. For pipelines that include writes, transactions, or explicit routing flags, SharpRedis does not substitute cached results, preserving execution order.
- Caching pauses if the primary or the subscription connection is unavailable. Reconnection clears the cache and restores tracking. When the subscription reconnects, SharpRedis disables old tracking before registering the prefixes and the new subscription connection ID.

Because the primary supplies cached values, replication lag cannot populate the cache with stale replica data. Cache invalidations are asynchronous, so strict read-after-write consistency is not guaranteed. Use `ForceMaster` to read directly from the primary. Concurrent cache misses can issue multiple `GET` requests because this implementation does not coalesce them.

## Integration tests

See the [MasterSlave integration tests](../tests/MasterSlave/README.md).
