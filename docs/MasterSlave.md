# 读写分离

配置已经建立复制关系的主节点和从节点。SharpRedis 负责连接、路由、心跳和重连，不配置 Redis 复制关系，不执行主从自动切换。

```csharp
using var redis = Redis.UseMasterSlave(options =>
{
    // 主节点
    options.Host = "127.0.0.1";
    options.Port = 6379;
    options.Password = "master-password";
    options.Database = 0;

    // 默认 RoundRobin，也可以设置 Random。
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

await redis.String.SetAsync("key", "value"); // 主节点
var value = await redis.String.GetAsync("key"); // 从节点
var latest = await redis.String.GetAsync("key", CommandFlags.ForceMaster);
```

连接字符串沿用单节点模式的逗号分隔格式：

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

从节点条目只使用 Host、Port、User、Password；认证信息分别配置，不从主节点隐式继承。数据库、RESP、编码、前后缀、超时等使用外层 `MasterSlaveConnectionOptions` 的统一配置。

## 路由规则

- 已知只读命令走从节点，写命令以及没有只读标记的命令走主节点。
- 轮询跳过不可用从节点；随机模式在可用从节点中等概率选择。
- `ForceMaster` 强制走主节点。`ForceSlave` 只允许已知只读命令，不能与 `ForceMaster` 同时使用。
- 从节点全部不可用时，普通读回退主节点；`ForceSlave` 报错，不静默回退。
- 主节点首次连接失败会导致注册失败。从节点首次连接失败会被跳过，心跳继续尝试连接配置中的原地址，不会提升其他节点为主节点。
- Pipeline 全部为只读命令时走从节点，包含写命令或未知命令时整批走主节点。整批及子命令的强制路由标志不能冲突。
- 事务固定在独立主连接上；显式绑定连接的操作不重新路由，强制路由标志也不能把事务拆到其他连接。
- 订阅使用主节点上的独立订阅连接，重连后等待重新订阅确认。
- 阻塞命令、非默认数据库操作及主连接故障时的临时替代连接均不复用，调用完成、取消或失败后释放。`redis.Select(db)` 的普通操作仍按读写选择节点；事务固定主节点及指定数据库。
- 主从复制可能有延迟。需要写后立即读取最新值时，使用 `ForceMaster`。

一个连接池共用一个发送线程。健康的主连接获取不加锁；多从节点轮询只增加一次原子计数，随机选择使用线程内随机数实例。

## 本地缓存

可以沿用 `UseClientCaching`，例如在 `UseMasterSlave` 配置中加入：

```csharp
options.UseClientCaching(key => key.StartsWith("cached:", StringComparison.Ordinal));
// 或者按前缀匹配：options.UseClientCaching(new[] { "cached:" });
```

- 满足缓存条件的只读命令：命中本地缓存直接返回，未命中时读取主节点并缓存。其他只读命令按配置走从节点。
- 匹配的是带 `Prefix`、`Suffix` 的完整 Key。多 Key 命令必须全部匹配才缓存；无 Key 的命令不缓存。
- 只在主节点启用广播 tracking、订阅失效通知，从节点不启用 tracking。自定义匹配函数在客户端执行，主节点广播全部 Key 的失效消息；前缀模式则订阅对应前缀。
- 参数为空的 `UseClientCaching()` 匹配全部 Key，因此可缓存的读命令都走主节点。
- 显式 `ForceMaster` / `ForceSlave` 跳过本地缓存，保留用户指定节点的语义。
- Pipeline 中有可缓存命令时整批走主节点；包含写命令、事务或显式强制路由时不替换本地缓存结果，保持执行顺序。
- 主连接或订阅连接不可用时暂停缓存使用；重连清理缓存并恢复 tracking。订阅重连会先关闭旧 tracking，再注册前缀和新的订阅连接 ID。

复制延迟不会导致缓存从旧的从节点数据回填。但本地缓存仍通过异步通知失效，不保证严格的写后读一致性；需要直接读取主节点时使用 `ForceMaster`。并发缓存未命中也可能产生多次 GET，此实现不合并并发请求。


## 集成验证

见 [MasterSlave 集成测试](../tests/MasterSlave/README.md)。
