<h1 align="center">
  <img src="https://raw.githubusercontent.com/q7164518/SharpRedis/main/SharpRedis.png" width="44" height="44" align="middle" alt="">
  SharpRedis
</h1>

<p align="center">
  <a href="./README_zh_cn.md">中文</a> · <strong>English</strong>
</p>

**SharpRedis** is a high-performance Redis client for .NET. It uses an asynchronous, event-driven architecture and provides synchronous and asynchronous command APIs, pipelining, transactions, client-side caching, and support for multiple Redis deployment modes.

[Quick start](#quick-start) · [Connection modes](#connection-modes) · [Advanced usage](#advanced-usage) · [Dependency injection](#dependency-injection) · [Connection configuration](#connection-configuration) · [NuGet](#nuget)

## Key features

| Feature | Description |
| --- | --- |
| Asynchronous I/O and cancellation | Network I/O is asynchronous and event-driven. Use a command's `CancellationToken` parameter to cancel the wait. |
| Target frameworks | Builds are available for .NET Standard 2.0 and 2.1, and for .NET 5 through .NET 11. |
| Commands grouped by type | Access commands through APIs such as `redis.String` and `redis.Hash`. Supported commands include hash field expiration. |
| Pipelining | Send multiple commands in batches to reduce network round trips. |
| Client-side caching | Use the client-side caching mechanism introduced in Redis 6 to reduce repeated reads. |
| Unix domain sockets | Connect through a Unix domain socket using the .NET Standard 2.1 build or later. See the [Unix socket configuration guide](./docs/UnixSocket.en.md). |
| Character spans | Pass a `ReadOnlySpan<char>` to commands that accept `SharpValue` without creating an intermediate string. The conversion copies the character data. |

## Quick start

Install SharpRedis from NuGet.

```bash
dotnet add package SharpRedis
```

Connect to a local Redis instance, then write and read a key.

```csharp
using SharpRedis;

using var redis = Redis.UseStandalone("host=127.0.0.1,port=6379");

await redis.String.SetAsync("greeting", "Hello, Redis!");
var greeting = await redis.String.GetAsync("greeting");
```

For synchronous calls, use `redis.String.Set(...)` and `redis.String.Get(...)`.

### Command groups

| Purpose | API |
| --- | --- |
| Core data types | `redis.String`, `redis.Hash`, `redis.List`, `redis.Set`, `redis.SortedSet` |
| Specialized data types | `redis.Bitmap`, `redis.Stream`, `redis.HyperLogLog`, `redis.Geo`, `redis.Array` |
| Pub/Sub and scripting | `redis.PubSub`, `redis.Script` (Lua), `redis.Func` (Redis Functions) |
| Keys, connections, and server operations | `redis.Key`, `redis.Connection`, `redis.Server` |
| Probabilistic data structures | `redis.Modules.Bloom` (Bloom filters), `redis.Modules.Cuckoo` (Cuckoo filters) |

## Connection modes

| Mode | API | When to use |
| --- | --- | --- |
| Standalone | `Redis.UseStandalone` | Connect to a single Redis instance. |
| Fixed primary and replicas | `Redis.UseMasterSlave` | Split reads and writes across an existing replication topology. |
| Sentinel | `Redis.UseSentinel` | Discover the current primary through Redis Sentinel and follow failovers. |
| Cluster | `Redis.UseCluster` | Route commands to Redis Cluster nodes by hash slot. |

### Fixed primary and replicas

Writes go to the primary, and replicas handle routine reads. Each replica has its own connection string. See the [primary and replica configuration guide](./docs/MasterSlave.en.md).

```csharp
using var redis = Redis.UseMasterSlave(
    "host=127.0.0.1,port=6379",
    new[] { "host=127.0.0.1,port=6380", "host=127.0.0.1,port=6381" });
```

### Sentinel

The first argument controls whether reads and writes are routed separately. With `false`, all commands go to the current primary, and SharpRedis follows Sentinel failovers. See the [Sentinel configuration guide](./docs/Sentinel.en.md).

```csharp
using var redis = Redis.UseSentinel(
    false,
    "mastername=mymaster",
    new[] { "host=127.0.0.1,port=26379", "host=127.0.0.1,port=26380" });
```

### Cluster

Provide a seed node and SharpRedis discovers the cluster topology. The first argument controls whether reads and writes are routed separately. See the [cluster configuration guide](./docs/Cluster.en.md).

```csharp
using var redis = Redis.UseCluster(true, "host=127.0.0.1,port=7000");
```

## Advanced usage

The examples below use the `redis` instance from the quick start where applicable.

### Pipelining

Queue commands in the pipeline, then call `ExecutePipelining()` to run them and retrieve the results.

```csharp
using var pipe = redis.BeginPipelining();
_ = pipe.String.Set("greeting", "Hello, Redis!");
_ = pipe.String.Get("greeting");
_ = pipe.Hash.HGet("profile", "name");

var results = pipe.ExecutePipelining();
```

### Temporarily switch databases

Call `Switch(1)` to get a temporary client for database 1. You can also start a pipeline on that client. Redis Cluster always uses database `0`.

```csharp
using var db = redis.Switch(1);
var value = db.String.Get("greeting");

using var pipe = db.BeginPipelining();
_ = pipe.String.Get("greeting");
var results = pipe.ExecutePipelining();
```

### Transactions

Call `UseTransaction()` to start a transaction, queue commands, and call `Exec()` to execute it.

```csharp
using var transaction = redis.UseTransaction();
transaction.String.Set("greeting", "Hello, Redis!");

var results = transaction.Exec();
```

### Client-side caching

The built-in cache can match keys by prefix. This example caches keys that start with `profile:` for one minute.

```csharp
using System;
using SharpRedis;

using var redis = Redis.UseStandalone("host=127.0.0.1,port=6379", options =>
{
    options.UseClientCaching(new[] { "profile:" }, TimeSpan.FromMinutes(1));
});
```

To provide a custom cache, derive from `ClientCachingBase` and register it with `options.UseClientCaching(cache)`. See the [custom cache example](./examples/Example_NET10/LocalCache.cs).

## Dependency injection

In these examples, `LocalCache` is provided by your application. Basic registration, registration with client-side caching, and named registration are separate options.

### Microsoft.Extensions.DependencyInjection

Basic registration uses `SharpRedis.DependencyInjection`. Named registration uses `SharpRedis.DependencyInjectionKeyedService` and requires `Microsoft.Extensions.DependencyInjection.Abstractions` 8.0.0 or later.

```csharp
using SharpRedis.DependencyInjection;

// Basic registration
services.AddSharpRedisStandalone("host=127.0.0.1,port=6379");

// Enable client-side caching
services.AddSharpRedisStandalone<LocalCache>("host=127.0.0.1,port=6379");

// Named registration
services.AddSharpRedisStandalone("host=127.0.0.1,port=6379", serviceName: "named");
```

### Autofac

Install `SharpRedis.Autofac`.

```csharp
using SharpRedis.Autofac;

containerBuilder.AddSharpRedisStandalone("host=127.0.0.1,port=6379");
containerBuilder.AddSharpRedisStandalone<LocalCache>("host=127.0.0.1,port=6379");
containerBuilder.AddSharpRedisStandalone("host=127.0.0.1,port=6379", serviceName: "named");
```

### Unity

Install `SharpRedis.Unity`.

```csharp
using SharpRedis.Unity;

unityContainer.AddSharpRedisStandalone("host=127.0.0.1,port=6379");
unityContainer.AddSharpRedisStandalone<LocalCache>("host=127.0.0.1,port=6379");
unityContainer.AddSharpRedisStandalone("host=127.0.0.1,port=6379", serviceName: "named");
```

## Connection configuration

### Connection strings

A connection string contains comma-separated `key=value` pairs in any order. The table lists common options.

| Option | Default | Description |
| --- | --- | --- |
| `host` | `127.0.0.1` | Redis host or the absolute path to a Unix socket. |
| `port` | `6379` | Redis TCP port. |
| `user` | Unset | Redis ACL username. |
| `password` | Unset | Redis authentication password. |
| `encoding` | `utf-8` | Data encoding. |
| `connectname` | Empty string | Connection name prefix. |
| `prefix` | Unset | Prefix automatically prepended to Redis keys. |
| `suffix` | Unset | Suffix automatically appended to Redis keys. |
| `database` | `0` | Default database index. |
| `commandtimeout` | `60000` | Command execution timeout in milliseconds. |
| `heartbeatinterval` | `60000` | Heartbeat interval in milliseconds. |
| `reconnectinterval` | `5000` | Reconnect interval in milliseconds. |
| `resp` | `2` | RESP protocol version. Use `2` or `3`. RESP3 requires Redis 6 or later. |
| `ssl` | `false` | Enables TLS. |
| `sslhost` | Value of `host` | Server name used for certificate validation. |
| `checkcertificaterevocation` | `false` | Enables certificate revocation checks. |

```text
host=127.0.0.1,port=6379,database=0,resp=3,prefix=myapp:
```

### Configuration in code

Use a configuration callback to set connection options, including the read buffer size.

```csharp
using var redis = Redis.UseStandalone(options =>
{
    options.Host = "127.0.0.1";
    options.Port = 6379;
    options.BuffSize = 4096;
});
```

## NuGet

| Package | Version | Downloads |
| --- | --- | --- |
| SharpRedis                                 | [![nuget](https://img.shields.io/nuget/v/SharpRedis.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis?groupby=Version) |
| SharpRedis.Autofac                         | [![nuget](https://img.shields.io/nuget/v/SharpRedis.Autofac.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.Autofac) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.Autofac.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.Autofac?groupby=Version) |
| SharpRedis.Unity                           | [![nuget](https://img.shields.io/nuget/v/SharpRedis.Unity.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.Unity) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.Unity.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.Unity?groupby=Version) |
| SharpRedis.DependencyInjection             | [![nuget](https://img.shields.io/nuget/v/SharpRedis.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.DependencyInjection) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.DependencyInjection?groupby=Version) |
| SharpRedis.DependencyInjectionKeyedService | [![nuget](https://img.shields.io/nuget/v/SharpRedis.DependencyInjectionKeyedService.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.DependencyInjectionKeyedService) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.DependencyInjectionKeyedService.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.DependencyInjectionKeyedService?groupby=Version) |

## Support the project

If SharpRedis has helped you, consider giving the project a star. Thanks for using and supporting SharpRedis!

<table>
  <tr><th>WeChat Pay</th><th>Alipay</th></tr>
  <tr>
    <td>
      <img src="https://raw.githubusercontent.com/q7164518/SharpRedis/main/wechat.png" alt="WeChat Pay QR code" width="240">
    </td>
    <td>
      <img src="https://raw.githubusercontent.com/q7164518/SharpRedis/main/alipay.png" alt="Alipay QR code" width="240">
    </td>
  </tr>
</table>

## Legal Use Disclaimer

SharpRedis is an open-source project intended for legitimate software development, research, and educational purposes.

The SharpRedis Project does not endorse, encourage, or support the use of this software for illegal activities, cybercrime, unauthorized access, data theft, fraud, or any other activities that violate applicable laws and regulations.

Users are solely responsible for ensuring that their use of SharpRedis complies with all applicable laws and regulations.

This statement expresses the project's position on responsible use and does not impose additional restrictions on the rights granted under the MIT License.

## License

[MIT](./LICENSE)
