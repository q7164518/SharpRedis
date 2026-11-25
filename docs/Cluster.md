# 集群模式

```csharp
using var redis = Redis.UseCluster(true, options =>
{
    options.Nodes.Add(new StandaloneConnectionOptions { Host = "127.0.0.1", Port = 7000 });
    options.Nodes.Add(new StandaloneConnectionOptions { Host = "127.0.0.1", Port = 7001 });
    options.User = "default";
    options.Password = "redis-password";
    options.CommandTimeout = 5000;
    options.ClusterRefreshInterval = 1000;
    options.SlaveSelectionMode = SlaveSelectionMode.RoundRobin; // 也可以 Random

    // options.UseClientCaching(cache);
});

await redis.String.SetAsync("user:{42}:name", "Alice");
var name = await redis.String.GetAsync("user:{42}:name");
```

也可以使用 `Redis.UseCluster(enableReadWriteSeparation, nodeConnectionStrings, optionsAction)`。当前驱动采用统一认证：所有发现的数据节点、从节点及重定向目标都使用同一组 `User`、`Password`。连接字符串重载初始读取第一个入口的认证信息；注册时在配置回调执行完毕后再次校验，所有显式填写的节点凭据必须与统一配置一致，否则在建立连接前抛出 `ArgumentException`。用户名和密码按大小写精确比较，异常只指出冲突项，不输出凭据。未填写的凭据继承统一配置；统一配置也未填写时，从节点列表中第一项显式凭据补全。此限制是当前驱动的配置约定，不是 Redis 要求各节点密码相同；校验只能保证客户端输入一致，实际服务器是否接受这组凭据仍由各节点认证结果决定。服务端返回的节点地址必须能从客户端访问。

第一个参数为 `false` 时，只建立各 slot 主节点的数据连接，所有操作均走主节点，包括指定 `ForceSlave` 的命令。为 `true` 时，普通读命令可按轮询或随机选择对应主节点的从节点，并使用 `READONLY`；写命令走主节点。符合本地缓存策略的读取走主节点，显式 `ForceSlave` 则绕过本地缓存。没有可用从节点时普通读取回退到主节点，`ForceSlave` 抛异常。

## Key、Pipeline 与事务

数据库固定为 **0**，不能切库。配置非 0 数据库会在建连前失败，`Select`/`Switch` 在集群模式下不支持。

CRC16 计算的是实际发送的完整 Key 字节，包括 Prefix、Suffix，支持字符串、字符内存、二进制 Key 和 Redis `{hash-tag}` 规则。

多 Key 命令要求所有 Key 属于**同一个 slot**，仅处于同一个节点还不够。校验以第一个 Key 为基准，遇到第一个不匹配的 Key 就抛 `ArgumentException`（消息以 `CROSSSLOT` 开头），不再读取后续 Key，也不发送该命令。

Pipeline 采用相同规则，但校验范围是**整个批次**：所有命令的 Key 都必须同 slot。校验发生在任何命令发送之前，不会出现前半部分已经执行、后半部分才发现跨 slot 的情况。无 Key 命令不改变 slot 基准。同 slot Pipeline 使用该 slot 的主节点，保留现有本地缓存命中处理和批量发送。

```csharp
using var pipe = redis.StartPipelining();
await pipe.String.SetAsync("user:{42}:name", "Alice");
await pipe.String.SetAsync("user:{42}:city", "Shanghai");
var results = await pipe.ExecutePipeliningAsync();
```

集群事务在 `RedisTranClient` 内准备并通过独立执行链发送，普通命令的选择中间件不判断事务类型。事务由第一个 Key 固定 slot，`MULTI` 延迟到确定目标连接后发送。`WATCH`、`EXEC`、`DISCARD` 使用同一连接。跨 slot 会使事务连接失效，不能继续提交已排队的部分命令。若事务在第一个 Key 之前已排入无 Key 命令，则已经绑定默认节点，后续不能改到其他节点。

普通无 Key 命令使用一个默认主节点；`SCAN`、`KEYS`、`DBSIZE`、`INFO`、`SCRIPT LOAD` 等不会自动广播或聚合所有节点。脚本仍须明确声明所操作的 Key，并满足同 slot 规则。普通 Pub/Sub 采用独立订阅连接，与缓存失效订阅分开管理。

## 拓扑与故障转移

后台通过 `CLUSTER SLOTS` 更新客户端 slot 路由表，发现入口不可用时尝试其他已知节点。新增、移除节点及主从角色变化后的路由随服务端拓扑更新；节点增删过程中服务端须完成相应的 slot 迁移。驱动不负责重新分配服务端数据，也不会主动触发选举。

客户端处理 `MOVED` 与 `ASK`，默认最多重定向 5 次，可通过 `MaxRedirects` 配置。`ASK` 在独占临时连接上先发送 `ASKING`，仅作用于这次请求；`MOVED` 同时触发拓扑刷新。Pipeline 仅重试明确返回重定向错误的条目，已成功的写入不会重放。网络断开、超时、事务失败等结果不明的操作不会自动重放，调用方应按操作幂等性决定是否重试。

拓扑切换采用整体连接池快照替换，因此切换时进行中的命令或事务可能失败。故障主节点有可提升的从节点且 Redis 集群完成选举后，驱动会跟随新的 slot 主节点。服务端缺少可恢复副本、未分配 slot 或返回 `CLUSTERDOWN` 时，驱动返回相应错误。

## 本地缓存与连接生命周期

每个主节点分别维护失效订阅和 redirect id，缓存内容仍使用用户注册的同一个 `ClientCachingBase`。默认模式和广播模式均支持。订阅不可用时绕过该节点的缓存命中及回填；恢复后重新绑定 tracking。拓扑替换时清空缓存并禁用旧连接回填，恢复订阅后重新使用缓存。

集群临时连接不复用，命令完成、取消或异常后直接关闭，连接池释放时也会关闭尚未结束的临时连接。插槽、节点归属和事务状态只保存在新增的 `ClusterConnection` 及集群连接池中；现有连接类和 `ICommand` 没有新增字段。各节点共享一个命令发送器和一个集群监控线程，不会为每个节点创建发送线程。正常的 slot 查表和连接选择不加锁。

## 独立集成测试

测试容器 `sharpredis-cluster-test-20261117` 保留用于后续测试。数据端口为 18379—18384（三主三从），18385 是扩缩容测试节点，密码为本地测试凭据 `sharpredis-cluster-test`。

```text
dotnet run --project tests/MasterSlave -- --cluster
dotnet run --project tests/MasterSlave -- --cluster-topology
```

第一组覆盖 CRC16、hash-tag、读写开关、RESP2/RESP3、默认/广播缓存、订阅断线、跨 slot 提前终止、Pipeline 零执行、事务和临时连接释放。第二组会在专用测试集群中迁移 slot、加入及移除测试节点、关闭并重启一个主节点，验证 `ASK`/`MOVED`、自动故障转移和缓存/订阅恢复。不要对业务集群执行测试启动脚本。
