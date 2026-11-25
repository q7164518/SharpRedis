# 单节点 TLS 测试

本地容器 `sharpredis-ssl-test-20261119` 监听 `127.0.0.1:16688`，要求客户端证书。容器以只读方式挂载此目录中的服务端证书、私钥和 CA。生成的 PEM 和 PFX 文件仅供本地测试，已排除在 Git 之外。

从仓库根目录运行集成测试：

```powershell
dotnet run --project tests/MasterSlave -- --standalone-tls
```

生产环境可通过 `ssl=true` 启用 TLS。默认使用系统信任链和主机名校验。证书名称与连接地址不同时可配置 `sslHost`。双向认证通过 `ClientCertificates` 提供带私钥的客户端证书，或通过 `CertificateSelection` 回调选择证书。证书应在 Redis 客户端释放后再释放。

`StandaloneTlsChecks.cs` 演示了如何通过 `CertificateValidation` 为测试连接指定私有 CA，同时保留主机名校验。测试 PFX 密码为 `sharpredis-ssl-test`。

## 主从、哨兵和集群

- `UseMasterSlave` 的主节点连接串支持 `ssl=true`。副本继承主配置，也可在各副本配置上单独启用 TLS，指定证书名称或提供证书。
- `UseSentinel` 的主节点连接串配置 Redis 数据节点的 TLS。每个哨兵的连接串单独配置 `ssl=true`，回调可分别设置 `o.ClientCertificates` 和 `o.Sentinels[i].ClientCertificates`。
- `UseCluster` 在集群配置或任一入口启用 TLS 后，所有节点都使用 TLS。为自动发现的节点设置集群级 `ClientCertificates`、`CertificateValidation` 和可选的 `SslHost`。入口上的证书配置仅覆盖对应地址。默认使用每个实际连接地址校验主机名。

新测试容器 `sharpredis-tls-topologies-20261119` 使用本机以下端口：

| 端口 | 用途 |
| --- | --- |
| `30000` | 单节点双向 TLS |
| `30001`、`30002` | 主从双向 TLS，数据密码 `tls-data-test` |
| `30003` | 哨兵双向 TLS，密码 `tls-sentinel-test`，主组 `tlsmaster` |
| `30010–30015` | 集群双向 TLS，任取一个作为入口 |
| `30020–30021` | 明文主从生命周期回归 |

```powershell
dotnet run --project tests/MasterSlave -- --topologies-tls
```
