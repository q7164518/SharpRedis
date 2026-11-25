# 哨兵模式

```csharp
using var redis = Redis.UseSentinel(true, options =>
{
    options.MasterName = "mymaster";
    // Redis 数据节点认证，供自动发现的主节点和所有从节点使用。
    options.User = "default";
    options.Password = "redis-password";
    options.SlaveSelectionMode = SlaveSelectionMode.RoundRobin; // 也支持 Random
    options.SentinelRefreshInterval = 1000; // 哨兵心跳、拓扑查询周期，毫秒
    options.SentinelCommandTimeout = 1000; // 哨兵连接及命令超时，毫秒
    options.CommandTimeout = 5000; // Redis 数据节点连接及命令超时，毫秒

    foreach (var host in new[] { "sentinel-a", "sentinel-b", "sentinel-c" })
        options.Sentinels.Add(new StandaloneConnectionOptions
        {
            Host = host,
            Port = 26379,
            User = "default",
            Password = "sentinel-password",
        });

    // 与单机、主从模式一样，可通过 options.UseClientCaching(cache) 注册本地缓存。
});

await redis.String.SetAsync("example", "value");
var value = await redis.String.GetAsync("example");
```

也支持 `Redis.UseSentinel(enableReadWriteSeparation, masterConnectionString, sentinelConnectionStrings, optionsAction)`；连接字符串里的认证用于各哨兵，回调里的 `User`、`Password` 用于数据节点。

`UseSentinel` 的第一个参数为 `enableReadWriteSeparation`：`true` 开启读写分离；`false` 不查询或连接从节点，所有操作均使用主节点（包括指定 `ForceSlave` 的命令）。关闭时仍保留哨兵心跳、自动主节点切换、订阅恢复与本地缓存 tracking 迁移，从节点列表变化不会触发连接池重建。

`Sentinels` 是同一个 `MasterName` 服务的多个发现入口。多个独立业务主节点组分别创建 Redis 实例。哨兵返回的数据节点地址必须能从客户端访问。客户端不会修改服务端复制关系或主动发起选举。

写主、读从、轮询/随机、CommandFlags、Pipeline、事务及临时连接规则复用主从模式。没有可用从节点时普通读取退回主节点，ForceSlave 抛出异常。符合本地缓存策略的读取及失效订阅使用主节点。

后台轮询所有配置的哨兵：PING 检测存活，查询主从拓扑，优先使用较新的配置纪元；建立数据连接时用 ROLE 验证角色。哨兵不可用时保留已知拓扑并继续尝试连接。Redis 节点通过原有心跳机制检查可用性和角色。

拓扑变化后，停止旧连接池、清空缓存、重新建立主从连接，等待普通订阅和失效订阅确认，再更新新主节点的 tracking redirect。切换期间不使用缓存，跨越切换的旧请求不能回填。完成恢复后发布新连接池。普通切库客户端后续命令可使用新主节点；已经开始的事务会失败，需要调用方重新创建。

故障检测和恢复期间命令可能抛出连接或超时异常。客户端不会自动重放已经发送的写命令、Pipeline 或事务；应用应根据操作幂等性决定是否重试。读取从节点仍具有 Redis 异步复制的延迟特性。

## 本地集成验证

`dotnet run --project tests/MasterSlave -- --sentinel`

测试仅针对专用容器 `sharpredis-sentinel-test-20261117`：数据节点 17379/17380/17381，哨兵 27379/27380/27381，主节点组 `mymaster`。数据密码 `sharpredis-data-test`，哨兵密码 `sharpredis-sentinel-test`。这些是本地测试凭据。

测试覆盖读写分离开启/关闭、两个注册重载、RESP2/RESP3、默认/广播缓存、主动故障转移、主节点退出且一个哨兵不可用、订阅与 tracking 迁移、Pipeline、事务、切库。测试会改变专用集群的主节点，最后重启关闭的测试进程，保留容器。`tests/MasterSlave/sentinel-fixture.sh` 仅用于首次初始化空的专用测试容器，不要在现有业务集群运行。
