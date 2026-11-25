# MasterSlave 集成测试

此项目独立于 `NET10` 测试集，不使用可能执行 FLUSHALL 的 Factory。

准备隔离的一主两从：主节点 `127.0.0.1:16379`，从节点 `16380`、`16381`，无密码，至少支持两个数据库。先在 Redis 侧配置复制。可使用 `SHARPREDIS_TEST_HOST` 改变地址；三个端口固定。

执行：

```powershell
dotnet run --project tests/MasterSlave/MasterSlave.csproj -c Release -p:GeneratePackageOnBuild=false
```

覆盖轮询/随机、RESP2/RESP3、主从强制路由及冲突、混合 Pipeline、事务、DB 切换、临时连接释放、订阅确认/恢复、阻塞取消、从节点故障跳过/回退、各节点心跳重连、活动连接随池释放，以及配置校验。

另覆盖默认匹配/前缀广播两种本地缓存模式下的主节点回填、缓存命中、非匹配 Key 从节点路由、显式路由绕过缓存、字节/文本结果隔离、自身及外部写入失效、过期、多数据库、Pipeline 和 tracking 重连。

测试写入随机前缀的短期 TTL 键，不执行 FLUSH、不改变复制配置。通过关闭客户端自己的连接模拟网络故障；缓存测试会对两个从节点执行 `CLIENT PAUSE 1800 WRITE`，验证复制延迟时仍从主节点回填。请只连接隔离的测试实例。

已保留本地 WSLC 容器 `sharpredis-master-slave-test-20261113`，其中主节点映射 `127.0.0.1:16379`，两个从节点映射 `16380`、`16381`，均无密码，供后续手动测试。
