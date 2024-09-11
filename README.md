<h1 align="center">⚔️ SharpRedis</h1>

<p align="center">
  <b>SharpRedis</b> 是一个用 C# 实现的高性能 Redis 驱动，解决了开发者常见的痛点——<b>不再有 Timeout 的烦恼</b>，<b>不再有数据串包的困扰</b>，也没有低版本 .NET 无法使用的局限。如果你正在经历这些问题，赶快来试试 SharpRedis 吧！
</p>

---

## ✨ 为什么选择 SharpRedis？

**SharpRedis** 拥有以下卓越特性，让你从容应对 Redis 的各种复杂需求：

- 🚀 **异步驱动**：完全基于异步编程模型，充分利用现代 .NET 的优势，提供极高的性能和响应速度。
  
- 🛠️ **广泛的框架支持**：从 **.NET Framework 3.0** 到 **.NET 8.0**，SharpRedis 几乎覆盖了所有 .NET 版本，为广泛的项目需求提供支持。

- ⚡ **全异步事件驱动**：保证所有操作都通过异步事件处理，专为高并发场景而设计，确保在大流量和高吞吐量情况下依旧保持卓越性能。

- ✔️ **完美的异步支持**：自 **.NET Framework 4.0+** 开始，SharpRedis 的所有方法都支持异步操作，轻松处理复杂的非阻塞任务。

- 🏎️ **Redis 本地缓存支持**：为高效读取操作而生，完全兼容 **Redis 6.x** 引入的本地缓存特性，显著提升数据访问速度。

- 📦 **支持最新 Redis 命令**：SharpRedis 紧跟 Redis 版本更新，完美支持到 **Redis 7.4.0**，包括如 Hash 过期等最新命令，保持功能的最新状态。

- 📡 **命令管道支持**：支持 Redis 的命令管道功能，一次性批量执行多个命令，大幅降低网络延迟，提升整体性能。

- 📡 **命令隔离**：命令不在混淆, 每个类型的命令都做了隔离, 如操作String需redis.String.Get..., 操作Hash需redis.Hash.HSet...

- ⏳ **灵活取消或超时**：所有方法都支持传入CancellationToken, 进行优雅取消操作
---

## 💼 谁适合使用 SharpRedis？

**SharpRedis** 是为所有 .NET 开发者打造的 Redis 客户端，旨在提供强大、灵活、高效的解决方案，尤其适用于需要异步编程、高并发处理的场景。无论是处理大规模的数据存储还是构建高效的分布式缓存，SharpRedis 都能为你提供理想的解决方案。

## 🚀 快速开始
```csharp
using SharpRedis;

var redis = Redis.UseStandalone("host=127.0.0.1,port=6379");
redis.String.Set("key1", "key1");

var get = redis.String.Get("key1");

//redis.Hash 操作Hash
//redis.PubSub 操作发布订阅
//redis.Connection 操作连接, 目前支持的不多, 后续会增加
//redis.List 操作List类型
//redis.Bitmap 操作Bitmap类型 (虽然本质还是String, 单还是做了区分)
//redis.Set 操作Set类型
//redis.SortedSet 操作SortedSet类型
//redis.Stream 操作Stream类型
//redis.HyperLogLog 操作HyperLogLog类型
//redis.Geospatial 操作GEO类型
//redis.Script 操作LUA脚本和Function
//redis.Key 操作Redis Key
//redis.Server 操作Redis服务端, 支持的较少, 后续会增加完善
```

## ⚙️ 连接字符串配置项

| 配置项             | 默认值     | 说明|
| :---------------- | --------: | :------------------- |
| host            | 127.0.01      | Redis服务主机地址 |
| port            | 6379          | Redis服务端口号 |
| password        | null          | Redis密码, 没有设置密码不需要设置 |
| user            | null          | Redis用户, 没有用户不需要设置 |
| encoding        | utf-8         | 数据编码协议 |
| connectname     | null          | 连接名称前缀 |
| prefix          | null          | key前缀, 如果设置了, 所有操作中的Key都会加上此前缀 |
| defaultdatabase | 0             | 默认连接数据库 |
| maxpoolsize     | 100           | 连接池最大数量, 不建议设置超过300, 适量调整 |
| minpoolsize     | 3             | 连接池最小数量, 不建议设置过大. 否则会一直保持大量连接 |
| commandtimeout  | 60000         | 全局执行超时时间, 单位: 毫秒. 不建议调整. 可以使用方法的CancellationToken参数灵活控制 |
| idletimeout     | 30000         | 连接空闲回收时间, 单位: 毫秒 |
| subconcurrency  | 5             | 单个连接最多订阅数量, 超过该值, 新增的订阅将创建新连接 |
| resp            | 2             | RESP协议版本, 只能是2或3. 3需要Redis6.x及以上才支持 |
| buffer          | 4096 (4kb)    | 缓冲区大小, 不建议调整, 除非你的每次读取的redis数据都超过4kb |

> 完整示例: host=127.0.0.1,port=6379,password=123456,user=redis,encoding=utf-8,connectname=abc,prefix=myprefix,defaultdatabase=0,maxpoolsize=100,minpoolsize=3,commandtimeout=60000,idletimeout=30000,subconcurrency=5,resp=3,buffer=4096

> 配置项不分顺序先后

> 如果你不想使用连接字符串, 可以使用代码配置

```csharp
var redis = Redis.UseStandalone(option =>
{
	option.Host = "127.0.01";
	option.Port = 6379;
});
```

## 🔗 Nuget
| 包名 |  NuGet | 下次次数  |
|--------------|  ------- |  ----  |
| SharpRedis  | [![nuget](https://img.shields.io/nuget/v/SharpRedis.svg?style=flat-square)](https://www.nuget.org/packages/SharpRedis) | [![stats](https://img.shields.io/nuget/dt/SharpRedis.svg?style=flat-square)](https://www.nuget.org/stats/packages/SharpRedis?groupby=Version) |

## 📡 命令管道使用方式
```csharp
using var pipe = redis.BeginPipelining();
_ = pipe.String.Set("key", "value");
_ = pipe.String.Get("key");
_ = pipe.Hash.HGet("hash", "field");
var result = pipe.ExecutePipelining();
```

## 📡 临时切换数据库
```csharp
using var db = redis.SwitchDatabase(1);
db.String.Get("key");
//切库之后继续使用管道
using var pipe = db.BeginPipelining();
...
var result = pipe.ExecutePipelining();
```

## ⏳ 事务使用
```csharp
using var tran = redis.UseTransaction();
tran.String.Set("key", "value");
...
var result = tran.Exec(); //执行事务
```

## 🏎️ 开启本地缓存支持
```csharp
var redis = Redis.UseStandalone(option =>
{
	option.Host = "127.0.01";
	option.Port = 6379;
	option.Password = "123456";
	option.SetClientSideCaching(new LocalCache());
});

//需要自定义本地缓存实现
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

> ❤ 如果您觉得此项目给你提供了帮助, 您也可以选择进行捐赠. 您的捐赠是对我最大的支持. 感谢捐赠者 ❤
### 支付宝
<img src="./alipay.png" alt="支付宝" width="200" height="200">

### 微信
<img src="./wechat.png" alt="微信" width="200" height="200">

## 🗄 License

[MIT](LICENSE)