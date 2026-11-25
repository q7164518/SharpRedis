# Unix domain sockets

On systems that support Unix domain sockets, set `Host` to an absolute socket path. In a connection string, use `host=/path/redis.sock`. The older `unixsocket=/path/redis.sock` option is also supported. Use the .NET Standard 2.1 build of SharpRedis or later. Leave `ssl` disabled for Unix socket endpoints.

For standalone and fixed primary and replica configurations, specify the socket path for each node directly.

```csharp
using var standalone = Redis.UseStandalone("host=/tmp/sharpredis.sock");

using var replica = Redis.UseMasterSlave(
    "host=/tmp/redis-30101.sock",
    ["host=/tmp/redis-30102.sock"]);
```

Sentinel uses the addresses and ports reported by the server to determine the topology. Set `DataNodeUnixSockets` to map a local socket path for every data node that could become the primary. If a data node is reported at a loopback address, you can also use the top-level `Host` and `Port` to match one such node.

```csharp
using var sentinel = Redis.UseSentinel(true, "mastername=unixmaster",
    ["host=/tmp/redis-30103.sock"], o =>
    {
        o.DataNodeUnixSockets[("127.0.0.1", 30101)] = "/tmp/redis-30101.sock";
        o.DataNodeUnixSockets[("127.0.0.1", 30102)] = "/tmp/redis-30102.sock";
    });
```

Cluster mode can discover the entire topology through a Unix socket seed. For a Unix socket seed, `Port` matches the loopback port advertised by the node, and the client connects through the socket file. Map other addresses with `NodeUnixSockets`. SharpRedis also uses this map for redirects and topology refreshes. In Sentinel and cluster modes, nodes without local mappings use TCP at their advertised addresses.

```csharp
using var cluster = Redis.UseCluster(true, o =>
{
    o.Host = "/tmp/redis-30110.sock";
    o.Port = 30110;
    for (int port = 30111; port <= 30115; port++)
        o.NodeUnixSockets[("127.0.0.1", port)] = $"/tmp/redis-{port}.sock";
});
```

The local `sharpredis-unix-test` container provides the standalone socket. The `sharpredis-unix-topologies` container provides the primary and replica, Sentinel, and cluster endpoints shown above. Run the application in an environment that can access the socket files.

```powershell
wslc exec sharpredis-unix-test redis-cli -s /tmp/sharpredis.sock PING
wslc exec sharpredis-unix-topologies redis-cli -s /tmp/redis-30103.sock SENTINEL GET-MASTER-ADDR-BY-NAME unixmaster
```
