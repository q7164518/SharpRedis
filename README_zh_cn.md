<h1 align="center">
  <img src="https://raw.githubusercontent.com/q7164518/SharpRedis/main/SharpRedis.png" width="44" height="44" align="middle" alt="">
  SharpRedis
</h1>

<p align="center">
  <strong>中文</strong> · <a href="./README.md">English</a>
</p>

**SharpRedis** 是面向 .NET 的高性能 Redis 客户端。它采用异步事件驱动架构，提供同步和异步命令 API，支持命令管道、事务、本地缓存，以及多种 Redis 部署模式。

[快速开始](#快速开始) · [连接模式](#连接模式) · [进阶用法](#进阶用法) · [依赖注入](#依赖注入) · [连接配置](#连接配置) · [NuGet](#nuget)

## 核心特性

| 特性 | 说明 |
| --- | --- |
| 异步 I/O 与取消 | 网络 I/O 采用异步事件驱动。可通过命令的 `CancellationToken` 参数取消等待。 |
| 多目标框架 | 提供 .NET Standard 2.0、2.1 和 .NET 5 至 .NET 11 构建。 |
| 按类型组织命令 | 通过 `redis.String`、`redis.Hash` 等入口访问对应命令，支持 Hash 字段过期等特性。 |
| 命令管道 | 将多条命令合并发送，减少网络往返。 |
| 客户端缓存 | 利用 Redis 6 引入的客户端缓存机制，减少重复读取。 |
| Unix Domain Socket | 支持通过 Unix 套接字连接，需使用 .NET Standard 2.1 或更新的构建。参见 [Unix 套接字配置](./docs/UnixSocket.md)。 |
| Span 字符数据 | 可将 `ReadOnlySpan<char>` 传入接受 `SharpValue` 的命令，无需先创建中间字符串，转换时会复制字符数据。 |

## 快速开始

通过 NuGet 安装 SharpRedis。

```bash
dotnet add package SharpRedis
```

连接到本地 Redis，写入并读取一个键。

```csharp
using SharpRedis;

using var redis = Redis.UseStandalone("host=127.0.0.1,port=6379");

await redis.String.SetAsync("greeting", "你好，Redis");
var greeting = await redis.String.GetAsync("greeting");
```

同步调用可使用 `redis.String.Set(...)` 和 `redis.String.Get(...)`。

### 命令入口

| 用途 | 入口 |
| --- | --- |
| 基础数据类型 | `redis.String`、`redis.Hash`、`redis.List`、`redis.Set`、`redis.SortedSet` |
| 专用数据类型 | `redis.Bitmap`、`redis.Stream`、`redis.HyperLogLog`、`redis.Geo`、`redis.Array` |
| 发布订阅与脚本 | `redis.PubSub`、`redis.Script`（Lua）、`redis.Func`（Redis Functions） |
| 键、连接与服务端 | `redis.Key`、`redis.Connection`、`redis.Server` |
| 概率型数据结构 | `redis.Modules.Bloom`（布隆过滤器）、`redis.Modules.Cuckoo`（布谷鸟过滤器） |

## 连接模式

| 模式 | 入口 | 适用场景 |
| --- | --- | --- |
| 单机 | `Redis.UseStandalone` | 连接单个 Redis 实例 |
| 固定主从 | `Redis.UseMasterSlave` | 使用已有的主从复制拓扑实现读写分离 |
| 哨兵 | `Redis.UseSentinel` | 通过 Sentinel 发现当前主节点并跟随故障切换 |
| 集群 | `Redis.UseCluster` | 通过 Redis Cluster 的哈希槽将命令路由到对应节点 |

### 固定主从

写操作发往主节点，普通读操作由从节点分担。各从节点使用独立的连接字符串。参见 [主从模式配置](./docs/MasterSlave.md)。

```csharp
using var redis = Redis.UseMasterSlave(
    "host=127.0.0.1,port=6379",
    new[] { "host=127.0.0.1,port=6380", "host=127.0.0.1,port=6381" });
```

### 哨兵

第一个参数控制读写分离。传入 `false` 时，所有命令都发往当前主节点，客户端仍会跟随哨兵切换主节点。参见 [哨兵模式配置](./docs/Sentinel.md)。

```csharp
using var redis = Redis.UseSentinel(
    false,
    "mastername=mymaster",
    new[] { "host=127.0.0.1,port=26379", "host=127.0.0.1,port=26380" });
```

### 集群

提供一个入口节点，SharpRedis 会自动发现集群拓扑。第一个参数控制读写分离。参见 [集群模式配置](./docs/Cluster.md)。

```csharp
using var redis = Redis.UseCluster(true, "host=127.0.0.1,port=7000");
```

## 进阶用法

以下示例沿用快速开始中的 `redis` 实例。

### 命令管道

先将命令加入管道，再通过 `ExecutePipelining()` 一次执行并获取结果。

```csharp
using var pipe = redis.BeginPipelining();
_ = pipe.String.Set("greeting", "你好，Redis");
_ = pipe.String.Get("greeting");
_ = pipe.Hash.HGet("profile", "name");

var results = pipe.ExecutePipelining();
```

### 临时切换数据库

通过 `Switch(1)` 获取临时客户端，也可以在该客户端上开启命令管道。Redis Cluster 固定使用数据库 `0`。

```csharp
using var db = redis.Switch(1);
var value = db.String.Get("greeting");

using var pipe = db.BeginPipelining();
_ = pipe.String.Get("greeting");
var results = pipe.ExecutePipelining();
```

### 事务

通过 `UseTransaction()` 创建事务，添加命令后调用 `Exec()` 执行。

```csharp
using var transaction = redis.UseTransaction();
transaction.String.Set("greeting", "你好，Redis");

var results = transaction.Exec();
```

### 客户端缓存

使用内置缓存按键前缀匹配。以下示例缓存 `profile:` 开头的键，缓存时长为 1 分钟。

```csharp
using System;
using SharpRedis;

using var redis = Redis.UseStandalone("host=127.0.0.1,port=6379", options =>
{
    options.UseClientCaching(new[] { "profile:" }, TimeSpan.FromMinutes(1));
});
```

自定义缓存可继承 `ClientCachingBase`，并通过 `options.UseClientCaching(cache)` 注册。参见 [自定义缓存示例](./examples/Example_NET10/LocalCache.cs)。

## 依赖注入

各示例中的 `LocalCache` 为应用提供的缓存实现。基础注册、启用缓存和命名注册是三种独立用法。

### Microsoft.Extensions.DependencyInjection

基础注册使用 `SharpRedis.DependencyInjection`。命名注册使用 `SharpRedis.DependencyInjectionKeyedService`，并要求 `Microsoft.Extensions.DependencyInjection.Abstractions` 8.0.0 或更高版本。

```csharp
using SharpRedis.DependencyInjection;

// 基础注册
services.AddSharpRedisStandalone("host=127.0.0.1,port=6379");

// 启用本地缓存
services.AddSharpRedisStandalone<LocalCache>("host=127.0.0.1,port=6379");

// 命名注册
services.AddSharpRedisStandalone("host=127.0.0.1,port=6379", serviceName: "named");
```

### Autofac

安装 `SharpRedis.Autofac`。

```csharp
using SharpRedis.Autofac;

containerBuilder.AddSharpRedisStandalone("host=127.0.0.1,port=6379");
containerBuilder.AddSharpRedisStandalone<LocalCache>("host=127.0.0.1,port=6379");
containerBuilder.AddSharpRedisStandalone("host=127.0.0.1,port=6379", serviceName: "named");
```

### Unity

安装 `SharpRedis.Unity`。

```csharp
using SharpRedis.Unity;

unityContainer.AddSharpRedisStandalone("host=127.0.0.1,port=6379");
unityContainer.AddSharpRedisStandalone<LocalCache>("host=127.0.0.1,port=6379");
unityContainer.AddSharpRedisStandalone("host=127.0.0.1,port=6379", serviceName: "named");
```

## 连接配置

### 连接字符串

连接字符串由逗号分隔的 `键=值` 组成，配置项顺序不限。以下是常用配置项。

| 配置项 | 默认值 | 说明 |
| --- | --- | --- |
| `host` | `127.0.0.1` | Redis 主机地址，也可填写 Unix 套接字的绝对路径。 |
| `port` | `6379` | Redis TCP 端口。 |
| `user` | 未设置 | Redis ACL 用户名。 |
| `password` | 未设置 | Redis 认证密码。 |
| `encoding` | `utf-8` | 数据编码。 |
| `connectname` | 空字符串 | 连接名称前缀。 |
| `prefix` | 未设置 | 自动添加到 Redis 键前的前缀。 |
| `suffix` | 未设置 | 自动添加到 Redis 键后的后缀。 |
| `database` | `0` | 默认数据库编号。 |
| `commandtimeout` | `60000` | 命令执行超时，单位为毫秒。 |
| `heartbeatinterval` | `60000` | 心跳间隔，单位为毫秒。 |
| `reconnectinterval` | `5000` | 重连间隔，单位为毫秒。 |
| `resp` | `2` | RESP 协议版本，可设为 `2` 或 `3`。RESP3 需要 Redis 6 或更新版本。 |
| `ssl` | `false` | 是否启用 TLS。 |
| `sslhost` | 使用 `host` | 证书校验使用的服务器名称。 |
| `checkcertificaterevocation` | `false` | 是否检查证书吊销状态。 |

```text
host=127.0.0.1,port=6379,database=0,resp=3,prefix=myapp:
```

### 代码配置

也可以通过配置回调设置连接参数和读缓冲区大小。

```csharp
using var redis = Redis.UseStandalone(options =>
{
    options.Host = "127.0.0.1";
    options.Port = 6379;
    options.BuffSize = 4096;
});
```

## NuGet

| 包名 | 版本 | 下载次数 |
| --- | --- | --- |
| SharpRedis                                 | [![nuget](https://img.shields.io/nuget/v/SharpRedis.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis?groupby=Version) |
| SharpRedis.Autofac                         | [![nuget](https://img.shields.io/nuget/v/SharpRedis.Autofac.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.Autofac) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.Autofac.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.Autofac?groupby=Version) |
| SharpRedis.Unity                           | [![nuget](https://img.shields.io/nuget/v/SharpRedis.Unity.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.Unity) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.Unity.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.Unity?groupby=Version) |
| SharpRedis.DependencyInjection             | [![nuget](https://img.shields.io/nuget/v/SharpRedis.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.DependencyInjection) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.DependencyInjection?groupby=Version) |
| SharpRedis.DependencyInjectionKeyedService | [![nuget](https://img.shields.io/nuget/v/SharpRedis.DependencyInjectionKeyedService.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.DependencyInjectionKeyedService) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.DependencyInjectionKeyedService.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.DependencyInjectionKeyedService?groupby=Version) |

## 支持项目

如果 SharpRedis 帮到了你，欢迎为项目点个 Star。感谢使用和支持 SharpRedis 的开发者！

<table>
  <tr><th>微信支付</th><th>支付宝</th></tr>
  <tr>
    <td>
      <img src="https://raw.githubusercontent.com/q7164518/SharpRedis/main/wechat.png" alt="微信支付收款码" width="240">
    </td>
    <td>
      <img src="https://raw.githubusercontent.com/q7164518/SharpRedis/main/alipay.png" alt="支付宝收款码" width="240">
    </td>
  </tr>
</table>

## 合法使用声明

SharpRedis 是一个开源项目，旨在用于合法的软件开发、研究和教育。

SharpRedis 项目不认可、鼓励或支持将本软件用于违法活动、网络犯罪、未经授权的访问、数据窃取、欺诈或其他违反适用法律法规的活动。

用户应自行负责确保其使用 SharpRedis 的行为符合所有适用的法律法规。

本声明表达项目对负责任使用的立场，不对 MIT 许可证授予的权利施加任何额外限制。

## 许可证

[MIT](./LICENSE)
