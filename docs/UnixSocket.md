# Unix Domain Socket

在支持 Unix Domain Socket 的环境中，`Host` 可以填写绝对 socket 路径。连接字符串使用 `host=/path/redis.sock`，也兼容原来的 `unixsocket=/path/redis.sock`。需要 SharpRedis 的 .NET Standard 2.1 或较新构建。Unix Socket 端点不可同时启用 `ssl=true`。

单节点和固定主从直接填写各节点的路径。

```csharp
using var standalone = Redis.UseStandalone("host=/tmp/sharpredis.sock");

using var replica = Redis.UseMasterSlave(
    "host=/tmp/redis-30101.sock",
    ["host=/tmp/redis-30102.sock"]);
```

Sentinel 继续按服务端报告的地址和端口判断拓扑。用 `DataNodeUnixSockets` 给所有可能成为主节点的数据节点配置本地路径。对于报告为回环地址的数据节点，也可以用顶层 `Host` 和 `Port` 匹配其中一个节点。

```csharp
using var sentinel = Redis.UseSentinel(true, "mastername=unixmaster",
    ["host=/tmp/redis-30103.sock"], o =>
    {
        o.DataNodeUnixSockets[("127.0.0.1", 30101)] = "/tmp/redis-30101.sock";
        o.DataNodeUnixSockets[("127.0.0.1", 30102)] = "/tmp/redis-30102.sock";
    });
```

集群可以通过 Unix Socket 入口发现整个拓扑。配置 Unix 入口时，`Port` 用于匹配节点报告的回环端口，实际连接使用 socket 文件。其他地址可以用 `NodeUnixSockets` 映射。该映射也用于重定向和拓扑刷新。Sentinel 和集群中没有本地映射的节点按报告的地址走 TCP。

```csharp
using var cluster = Redis.UseCluster(true, o =>
{
    o.Host = "/tmp/redis-30110.sock";
    o.Port = 30110;
    for (int port = 30111; port <= 30115; port++)
        o.NodeUnixSockets[("127.0.0.1", port)] = $"/tmp/redis-{port}.sock";
});
```

本地测试容器 `sharpredis-unix-test` 提供单节点 socket。容器 `sharpredis-unix-topologies` 提供上述主从、Sentinel 和集群地址。应用需要在能够访问 socket 文件的环境中运行。

```powershell
wslc exec sharpredis-unix-test redis-cli -s /tmp/sharpredis.sock PING
wslc exec sharpredis-unix-topologies redis-cli -s /tmp/redis-30103.sock SENTINEL GET-MASTER-ADDR-BY-NAME unixmaster
```
