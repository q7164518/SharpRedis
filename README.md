<h1 align="center">⚔️ SharpRedis</h1>
<p align="center">
    <span>English</span> |  
    <a href="./README_zh_cn.md">中文</a>
</p>
<p align="center">
  <b>SharpRedis</b> is a high-performance Redis driver implemented in C#, designed to solve common developer pain points—<b>No more Timeout issues</b>, <b>No more data packet misalignment</b>, and no limitations on older .NET versions. If you're facing these problems, try SharpRedis today!
</p>

---

## ✨ Why Choose SharpRedis?

**SharpRedis** offers the following exceptional features to help you handle complex Redis needs with ease:

- **🚀 Asynchronous Driver:** Fully based on an asynchronous programming model, taking full advantage of modern .NET capabilities to deliver high performance and responsiveness.

- **🛠️ Wide Framework Support:** From .NET Framework 3.0 to .NET 8.0, SharpRedis covers almost all .NET versions, making it suitable for a wide range of project requirements.

- **⚡ Fully Asynchronous Event-Driven:** Ensures all operations are handled via asynchronous event processing, designed for high concurrency scenarios, maintaining top-tier performance even under heavy traffic and high throughput.

- **✔️ Perfect Asynchronous Support:** Since **.NET Framework 4.0+**, all SharpRedis methods support asynchronous operations, making it easy to handle complex non-blocking tasks.

- **🏎️ Redis Local Caching Support:** Designed for efficient read operations, fully compatible with the local caching feature introduced in Redis 6.x, significantly improving data access speed.

- **📦 Supports Latest Redis Commands:** SharpRedis stays up to date with Redis version updates, fully supporting up to Redis 7.4.0, including the latest commands like Hash expiry, ensuring you're always working with the latest features.

- **📡 Pipeline Command Support:** Supports Redis's pipeline command feature, allowing you to execute multiple commands in batches, reducing network latency and improving overall performance.

- **🛡️ Command Isolation:** Commands are not mixed up; each command type is isolated. For example, use redis.String.Get... to operate on strings, and redis.Hash.HSet... to work with hashes.

- **🐦‍🔥 Flexible Cancellation or Timeout:** All methods support the passing of a CancellationToken, allowing for graceful operation cancellations.

- **🧮 Span\<char\> Support:** Span is a high-performance feature of .NET. If you've transformed a string into Span\<char\> for calculation or slicing, it can be directly stored in Redis without generating additional strings.
---

## 💼 Who Should Use SharpRedis?

**SharpRedis** is designed for all .NET developers as a Redis client, providing a powerful, flexible, and efficient solution, particularly suitable for scenarios that require asynchronous programming and high concurrency. Whether you're handling large-scale data storage or building an efficient distributed cache, SharpRedis offers an ideal solution.

## 🚀 Quick Start
```csharp
using SharpRedis;

var redis = Redis.UseStandalone("host=127.0.0.1,port=6379");
redis.String.Set("key1", "key1");

var get = redis.String.Get("key1");

// redis.Hash operates on Hashes
// redis.PubSub operates on Publish/Subscribe
// redis.Connection operates on connection (currently limited, more features will be added)
// redis.List operates on Lists
// redis.Bitmap operates on Bitmaps (though essentially strings, they are distinguished)
// redis.Set operates on Sets
// redis.SortedSet operates on Sorted Sets
// redis.Stream operates on Streams
// redis.HyperLogLog operates on HyperLogLogs
// redis.Geospatial operates on GEO types
// redis.Script operates on LUA scripts and functions
// redis.Key operates on Redis keys
// redis.Server operates on Redis server (currently limited, more features will be added)
```

## 🚀 Using with Microsoft.Extensions.DependencyInjection
```csharp
// Install dependency
// Install-Package SharpRedis.DependencyInjection

using SharpRedis.DependencyInjection;

// Register SharpRedis
services.AddSharpRedisStandalone("host=127.0.0.1,port=6379");
// Register SharpRedis with local cache support
services.AddSharpRedisStandalone<LocalCache>("host=127.0.0.1,port=6379");

// Named registration
// Requires Microsoft.Extensions.DependencyInjection.Abstractions 8.0.0 or higher
// The required NuGet package is SharpRedis.DependencyInjectionKeyedService
// Install-Package SharpRedis.DependencyInjectionKeyedService
services.AddSharpRedisStandalone("host=127.0.0.1,port=6379", serviceName: "named");
```

## 🚀 Using with Autofac
```csharp
// Install dependency
// Install-Package SharpRedis.Autofac

using SharpRedis.Autofac;

// Register SharpRedis
containerBuilder.AddSharpRedisStandalone("host=127.0.0.1,port=6379");
// Register SharpRedis with local cache support
containerBuilder.AddSharpRedisStandalone<LocalCache>("host=127.0.0.1,port=6379");

// Named registration
containerBuilder.AddSharpRedisStandalone("host=127.0.0.1,port=6379", serviceName: "named");
```

## 🚀 Using with Unity
```csharp
// Install dependency
// Install-Package SharpRedis.Unity

using SharpRedis.Unity;

// Register SharpRedis
unityContainer.AddSharpRedisStandalone("host=127.0.0.1,port=6379");
// Register SharpRedis with local cache support
unityContainer.AddSharpRedisStandalone<LocalCache>("host=127.0.0.1,port=6379");

// Named registration
unityContainer.AddSharpRedisStandalone("host=127.0.0.1,port=6379", serviceName: "named");
```

## ⚙️ Connection String Configuration
| Option          | Default Value | Description|
| --------------- | ------------- | ------------------- |
| host            | 127.0.01      | Redis server host address |
| port            | 6379          | Redis server port number |
| password        | null          | Redis password (optional if not set) |
| user            | null          | Redis user (optional if not set) |
| encoding        | utf-8         | Data encoding protocol |
| connectname     | null          | Connection name prefix |
| prefix          | null          | Key prefix, applied to all key operations if set |
| defaultdatabase | 0             | Default database connection |
| maxpoolsize     | 100           | Maximum pool size; it's not recommended to exceed 300 |
| minpoolsize     | 3             | Minimum pool size; it's not recommended to set this too high, as it will maintain many connections |
| commandtimeout  | 60000         | Global execution timeout in milliseconds; flexible control via method's CancellationToken is recommended instead of adjusting this |
| idletimeout     | 30000         | Idle connection reclaim time in milliseconds |
| subconcurrency  | 5             | Maximum number of subscriptions per connection; additional subscriptions will create new connections |
| resp            | 2             | RESP protocol version (2 or 3). Version 3 requires Redis 6.x or higher |
| buffer          | 4096 (4kb)    | Buffer size, not recommended to adjust unless your Redis data exceeds 4kb per read |

> Complete example: host=127.0.0.1,port=6379,password=123456,user=redis,encoding=utf-8,connectname=abc,prefix=myprefix,defaultdatabase=0,maxpoolsize=100,minpoolsize=3,commandtimeout=60000,idletimeout=30000,subconcurrency=5,resp=3,buffer=4096

> Options can be in any order

> If you don't want to use a connection string, you can configure it via code
```csharp
var redis = Redis.UseStandalone(option =>
{
	option.Host = "127.0.01";
	option.Port = 6379;
});
```

## 🔗 Nuget
| Package                                    | NuGet | Downloads |
|------------------------------------------- | -------- | ------ |
| SharpRedis                                 | [![nuget](https://img.shields.io/nuget/v/SharpRedis.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis?groupby=Version) |
| SharpRedis.Autofac                         | [![nuget](https://img.shields.io/nuget/v/SharpRedis.Autofac.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.Autofac) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.Autofac.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.Autofac?groupby=Version) |
| SharpRedis.Unity                           | [![nuget](https://img.shields.io/nuget/v/SharpRedis.Unity.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.Unity) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.Unity.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.Unity?groupby=Version) |
| SharpRedis.DependencyInjection             | [![nuget](https://img.shields.io/nuget/v/SharpRedis.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.DependencyInjection) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.DependencyInjection?groupby=Version) |
| SharpRedis.DependencyInjectionKeyedService | [![nuget](https://img.shields.io/nuget/v/SharpRedis.DependencyInjectionKeyedService.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis.DependencyInjectionKeyedService) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.DependencyInjectionKeyedService.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis.DependencyInjectionKeyedService?groupby=Version) |

## 📡 Using Command Pipelining
```csharp
using var pipe = redis.BeginPipelining();
_ = pipe.String.Set("key", "value");
_ = pipe.String.Get("key");
_ = pipe.Hash.HGet("hash", "field");
var result = pipe.ExecutePipelining();
```

## 📡 Temporary Database Switching
```csharp
using var db = redis.SwitchDatabase(1);
db.String.Get("key");
// After switching databases, you can continue to use pipelining
using var pipe = db.BeginPipelining();
...
var result = pipe.ExecutePipelining();
```

## ⏳ Using Transactions
```csharp
using var tran = redis.UseTransaction();
tran.String.Set("key", "value");
...
var result = tran.Exec(); // Execute the transaction
```

## 🏎️ Enabling Local Cache Support
```csharp
var redis = Redis.UseStandalone(option =>
{
    option.Host = "127.0.0.1";
    option.Port = 6379;
    option.Password = "123456";
    option.SetClientSideCaching(new LocalCache());
});

// Custom local cache implementation
public class LocalCache : ClientSideCachingStandard
{
    private readonly MemoryCache _cache;

    public LocalCache()
    {
        this._cache = new MemoryCache(new MemoryCacheOptions { });
    }

    public override ClientSideCachingMode Mode => ClientSideCachingMode.Default;

    public override string[]? KeyPatterns => ["localcache_test*"];

    public override string[]? WithoutKeyPatterns => ["nocache*"];

    public override string[]? KeyPrefixes => ["localcache_test:"];

    protected override bool Clear()
    {
        this._cache.Clear();
        return true;
    }

    protected override bool Delete(in ClientSideCacheKey key)
    {
        this._cache.Remove(key);
        return true;
    }

    protected override bool Set(in ClientSideCacheKey key, object value)
    {
        this._cache.Set(key, value);
        return true;
    }

    protected override bool TryGet(in ClientSideCacheKey key, [NotNullWhen(true)] out object? value)
    {
        return this._cache.TryGetValue(key, out value);
    }
}
```

> ❤ If this project has been helpful to you, feel free to donate. Your donation is the greatest support I can receive. Thanks to all the donors ❤

### Alipay
<img src="./alipay.png" alt="Alipay" width="200" height="200">

### WeChat
<img src="./wechat.png" alt="WeChat" width="200" height="200">

## 🗄 License

[MIT](LICENSE)
