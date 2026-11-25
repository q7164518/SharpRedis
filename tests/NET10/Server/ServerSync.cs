using SharpRedis;
using SharpRedis.Exceptions;
using static SharpRedis.RedisServer;

namespace NET10.Server;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class ServerSync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region Memory

    [Test]
    public void MemoryDoctor_ReturnsReport()
    {
        var report = this._redis.Server.MemoryDoctor();
        Assert.That(report, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void MemoryMallocStats_ReturnsAllocatorReport()
    {
        var report = this._redis.Server.MemoryMallocStats();
        Assert.That(report, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void MemoryPurge_ReturnsOk()
    {
        Assert.That(this._redis.Server.MemoryPurge(), Is.True);
    }

    [Test]
    public void MemoryStats_ReturnsAllocationMetrics()
    {
        var stats = this._redis.Server.MemoryStats();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(stats, Is.Not.Null);
            Assert.That((long)stats!["total.allocated"], Is.GreaterThan(0));
        }
        Assert.That(stats.ContainsKey("peak.allocated"), Is.True);
    }

    [Test]
    public void MemoryUsage_ReturnsBytesForHashAndNullForMissingKey()
    {
        var key = $"memory:usage:{Guid.NewGuid():N}";
        Assert.That(this._redis.Server.MemoryUsage(key), Is.Null);
        try
        {
            _ = this._redis.Hash.HSet(key, "field", "value");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(this._redis.Server.MemoryUsage(key), Is.GreaterThan(0));
                Assert.That(this._redis.Server.MemoryUsage(key, samples: 1), Is.GreaterThan(0));
                Assert.That(this._redis.Server.MemoryUsage(key, samples: 0), Is.GreaterThan(0));
            }
        }
        finally
        {
            _ = this._redis.Key.Del(key);
        }
    }

    #endregion

    #region HotKeys

    /// <summary>
    /// 清理 HOTKEYS 跟踪状态。
    /// STOP 在未启动时可能报错，因此仅忽略该预期错误。
    /// </summary>
    private void HotKeysPrepare()
    {
        try
        {
            _ = this._redis.Server.HotKeysStop();
        }
        catch (RedisErrorException)
        {
            // Tracking is not active.
        }

        Assert.That(this._redis.Server.HotKeysReset(), Is.True);
    }

    private void HotKeysCleanup()
    {
        try
        {
            _ = this._redis.Server.HotKeysStop();
        }
        catch (RedisErrorException)
        {
            // Tracking is not active.
        }

        _ = this._redis.Server.HotKeysReset();
    }

    private static void HotKeysRedisError<TError>(Action action)
        where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// RESET 后没有监控数据，GET 返回 null。
    /// </summary>
    [Test]
    [NonParallelizable]
    public void HotKeysGet_AfterReset()
    {
        this.HotKeysPrepare();

        try
        {
            var result = this._redis.Server.HotKeysGet();

            Assert.That(result, Is.Null);
        }
        finally
        {
            this.HotKeysCleanup();
        }
    }

    /// <summary>
    /// START 后 GET 能获得正在运行的监控会话信息。
    /// </summary>
    [Test]
    [NonParallelizable]
    public void HotKeysStart_Get_Active()
    {
        this.HotKeysPrepare();

        try
        {
            Assert.That(
                this._redis.Server.HotKeysStart(
                    HotKeysMetrics.All,
                    count: 10,
                    sampleRatio: 1),
                Is.True);

            var result = this._redis.Server.HotKeysGet();

            Assert.That(result, Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result!.TrackingActive, Is.True);
                Assert.That(result.SampleRatio, Is.EqualTo(1));
                Assert.That(result.CollectionStartTimeUnixMs,
                    Is.GreaterThan(0));
                Assert.That(result.CollectionDurationMs,
                    Is.GreaterThanOrEqualTo(0));
            }
        }
        finally
        {
            this.HotKeysCleanup();
        }
    }

    /// <summary>
    /// 只启用 CPU 指标。
    /// </summary>
    [Test]
    [NonParallelizable]
    public void HotKeysStart_Cpu()
    {
        this.HotKeysPrepare();

        try
        {
            Assert.That(
                this._redis.Server.HotKeysStart(
                    HotKeysMetrics.Cpu,
                    count: 5,
                    sampleRatio: 1),
                Is.True);

            var key = $"String:{{HotKeysStart_Cpu}}:{Guid.NewGuid():N}";

            this._redis.String.Set(key, "hello");

            for (int i = 0; i < 50; i++)
                _ = this._redis.String.Get(key);

            var result = this._redis.Server.HotKeysGet();

            Assert.That(result, Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result!.TrackingActive, Is.True);
                Assert.That(result.ByCpuTimeUs, Is.Not.Null);
                Assert.That(result.TotalCpuTimeUserMs, Is.Not.Null);
                Assert.That(result.TotalCpuTimeSysMs, Is.Not.Null);

                Assert.That(result.ByNetBytes, Is.Null);
                Assert.That(result.TotalNetBytes, Is.Null);
            }
        }
        finally
        {
            this.HotKeysCleanup();
        }
    }

    /// <summary>
    /// 只启用 NET 指标。
    /// </summary>
    [Test]
    [NonParallelizable]
    public void HotKeysStart_Net()
    {
        this.HotKeysPrepare();

        try
        {
            Assert.That(
                this._redis.Server.HotKeysStart(
                    HotKeysMetrics.Net,
                    count: 5,
                    sampleRatio: 1),
                Is.True);

            var key = $"String:{{HotKeysStart_Net}}:{Guid.NewGuid():N}";

            this._redis.String.Set(key, "hello");

            for (int i = 0; i < 50; i++)
                _ = this._redis.String.Get(key);

            var result = this._redis.Server.HotKeysGet();

            Assert.That(result, Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result!.TrackingActive, Is.True);
                Assert.That(result.ByNetBytes, Is.Not.Null);
                Assert.That(result.TotalNetBytes, Is.Not.Null);

                Assert.That(result.ByCpuTimeUs, Is.Null);
                Assert.That(result.TotalCpuTimeUserMs, Is.Null);
                Assert.That(result.TotalCpuTimeSysMs, Is.Null);
            }
        }
        finally
        {
            this.HotKeysCleanup();
        }
    }

    /// <summary>
    /// 同时启用 CPU 与 NET。
    /// </summary>
    [Test]
    [NonParallelizable]
    public void HotKeysStart_AllMetrics()
    {
        this.HotKeysPrepare();

        try
        {
            Assert.That(
                this._redis.Server.HotKeysStart(
                    HotKeysMetrics.All,
                    count: 10,
                    sampleRatio: 1),
                Is.True);

            var key = $"String:{{HotKeysStart_AllMetrics}}:{Guid.NewGuid():N}";

            this._redis.String.Set(key, "hello");

            for (int i = 0; i < 100; i++)
                _ = this._redis.String.Get(key);

            var result = this._redis.Server.HotKeysGet();

            Assert.That(result, Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result!.TrackingActive, Is.True);
                Assert.That(result.ByCpuTimeUs, Is.Not.Null);
                Assert.That(result.ByNetBytes, Is.Not.Null);
                Assert.That(result.TotalCpuTimeUserMs, Is.Not.Null);
                Assert.That(result.TotalCpuTimeSysMs, Is.Not.Null);
                Assert.That(result.TotalNetBytes, Is.Not.Null);
            }
        }
        finally
        {
            this.HotKeysCleanup();
        }
    }

    /// <summary>
    /// 指定 SAMPLE 采样比例。
    /// </summary>
    [Test]
    [NonParallelizable]
    public void HotKeysStart_SampleRatio()
    {
        this.HotKeysPrepare();

        try
        {
            Assert.That(
                this._redis.Server.HotKeysStart(
                    HotKeysMetrics.All,
                    count: 10,
                    sampleRatio: 5),
                Is.True);

            var result = this._redis.Server.HotKeysGet();

            Assert.That(result, Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result!.TrackingActive, Is.True);
                Assert.That(result.SampleRatio, Is.EqualTo(5));
            }
        }
        finally
        {
            this.HotKeysCleanup();
        }
    }

    /// <summary>
    /// 跟踪已经运行时，不能再次 START。
    /// </summary>
    [Test]
    [NonParallelizable]
    public void HotKeysStart_AlreadyRunning()
    {
        this.HotKeysPrepare();

        try
        {
            Assert.That(
                this._redis.Server.HotKeysStart(HotKeysMetrics.All),
                Is.True);

            HotKeysRedisError<RedisErrorException>(() =>
                this._redis.Server.HotKeysStart(HotKeysMetrics.Cpu));

            var result = this._redis.Server.HotKeysGet();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TrackingActive, Is.True);
        }
        finally
        {
            this.HotKeysCleanup();
        }
    }

    /// <summary>
    /// 跟踪期间 RESET 应返回 Redis 错误。
    /// </summary>
    [Test]
    [NonParallelizable]
    public void HotKeysReset_WhileRunning()
    {
        this.HotKeysPrepare();

        try
        {
            Assert.That(
                this._redis.Server.HotKeysStart(HotKeysMetrics.Cpu),
                Is.True);

            HotKeysRedisError<RedisErrorException>(() =>
                this._redis.Server.HotKeysReset());

            var result = this._redis.Server.HotKeysGet();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TrackingActive, Is.True);
        }
        finally
        {
            this.HotKeysCleanup();
        }
    }

    /// <summary>
    /// STOP 之后，GET 仍然可以获得最后一次跟踪数据。
    /// </summary>
    [Test]
    [NonParallelizable]
    public void HotKeysStop_PreservesData()
    {
        this.HotKeysPrepare();

        try
        {
            Assert.That(
                this._redis.Server.HotKeysStart(
                    HotKeysMetrics.All,
                    count: 10,
                    sampleRatio: 1),
                Is.True);

            var key = $"String:{{HotKeysStop_PreservesData}}:{Guid.NewGuid():N}";

            this._redis.String.Set(key, "hello");

            for (int i = 0; i < 50; i++)
                _ = this._redis.String.Get(key);

            Assert.That(this._redis.Server.HotKeysStop(), Is.True);

            var result = this._redis.Server.HotKeysGet();

            Assert.That(result, Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result!.TrackingActive, Is.False);
                Assert.That(result.ByCpuTimeUs, Is.Not.Null);
                Assert.That(result.ByNetBytes, Is.Not.Null);
                Assert.That(result.CollectionStartTimeUnixMs,
                    Is.GreaterThan(0));
            }
        }
        finally
        {
            this.HotKeysCleanup();
        }
    }

    /// <summary>
    /// RESET 释放统计数据后，GET 应返回 null。
    /// </summary>
    [Test]
    [NonParallelizable]
    public void HotKeysReset_ClearsData()
    {
        this.HotKeysPrepare();

        try
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            this._redis.Server.HotKeysStart(
                                HotKeysMetrics.Cpu),
                            Is.True);

                Assert.That(this._redis.Server.HotKeysGet(), Is.Not.Null);

                Assert.That(this._redis.Server.HotKeysStop(), Is.True);
                Assert.That(this._redis.Server.HotKeysReset(), Is.True);
            }

            var result = this._redis.Server.HotKeysGet();

            Assert.That(result, Is.Null);
        }
        finally
        {
            this.HotKeysCleanup();
        }
    }

    /// <summary>
    /// GET 多次读取，不会停止监控。
    /// </summary>
    [Test]
    [NonParallelizable]
    public void HotKeysGet_RepeatedCalls()
    {
        this.HotKeysPrepare();

        try
        {
            Assert.That(
                this._redis.Server.HotKeysStart(
                    HotKeysMetrics.All,
                    sampleRatio: 1),
                Is.True);

            for (int i = 0; i < 5; i++)
            {
                var result = this._redis.Server.HotKeysGet();

                Assert.That(result, Is.Not.Null);
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(result!.TrackingActive, Is.True);
                    Assert.That(result.SampleRatio, Is.EqualTo(1));
                }
            }
        }
        finally
        {
            this.HotKeysCleanup();
        }
    }

    #endregion


    [Test]
    public void CommandAndCount()
    {
        var entries = this._redis.Server.Command();
        Assert.That(entries.Length, Is.EqualTo(this._redis.Server.CommandCount()));
        var get = entries.Single(entry => entry.Name == "get");
        Assert.That(get.Arity, Is.EqualTo(2));
        Assert.That(get.Flags, Does.Contain("readonly"));
    }

    [Test]
    public void CommandInfo()
    {
        var entries = this._redis.Server.CommandInfo(["GET", "sharpredis:unknown", "COMMAND"]);
        Assert.That(entries, Has.Length.EqualTo(3));
        Assert.That(entries[0]!.FirstKey, Is.EqualTo(1));
        Assert.That(entries[1], Is.Null);
        var spec = entries[0]!.KeySpecifications.Single();
        Assert.That(spec.BeginSearch.Type, Is.EqualTo("index"));
        Assert.That((int)spec.BeginSearch.Specification["index"], Is.EqualTo(1));
        Assert.That(spec.FindKeys.Type, Is.EqualTo("range"));
        Assert.That(entries[2]!.Subcommands.Any(entry => entry.Name == "command|docs"), Is.True);
    }

    [Test]
    public void CommandDocs()
    {
        var docs = this._redis.Server.CommandDocs(["SET", "COMMAND", "sharpredis:unknown"]);
        Assert.That(docs, Has.Count.EqualTo(2));
        Assert.That(docs["set"].Summary, Is.Not.Empty);
        Assert.That(docs["set"].Arguments.Any(arg => arg.Name == "key"), Is.True);
        Assert.That(docs["set"].Arguments.Any(arg => arg.Arguments.Length > 0), Is.True);
        Assert.That(docs["command"].Subcommands.ContainsKey("command|list"), Is.True);
        var allDocs = this._redis.Server.CommandDocs();
        Assert.That(allDocs, Has.Count.GreaterThan(docs.Count));
        Assert.That((this._redis.Server.CommandInfo()).Any(entry => entry?.Name == "get"), Is.True);
    }

    [Test]
    public void CommandGetKeys()
    {
        byte[] binary = [0, 255, 65];
        var keys = this._redis.Server.CommandGetKeys(["MSET", binary, "one", "other", "two"], CommandFlags.ReturnBytes);
        Assert.That(keys, Has.Length.EqualTo(2));
        Assert.That(keys![0].RawValue, Is.EqualTo(binary));
        Assert.That((string?)keys[1], Is.EqualTo("other"));
    }

    [Test]
    public void CommandGetKeysAndFlags()
    {
        var keys = this._redis.Server.CommandGetKeysAndFlags(["COPY", "source", "destination"]);
        Assert.That(keys, Has.Length.EqualTo(2));
        Assert.That((string?)keys[0].Key, Is.EqualTo("source"));
        Assert.That(keys[0].Flags, Does.Contain("access"));
        Assert.That((string?)keys[1].Key, Is.EqualTo("destination"));
        Assert.That(keys[1].Flags, Does.Contain("update"));
    }

    [Test]
    public void CommandList()
    {
        Assert.That(this._redis.Server.CommandList(), Does.Contain("get"));
        Assert.That(this._redis.Server.CommandList(CommandListFilter.Pattern("ge?")), Is.EqualTo(new[] { "get" }));
        Assert.That(this._redis.Server.CommandList(CommandListFilter.AclCat("read")), Does.Contain("get"));
        Assert.That(this._redis.Server.CommandList(CommandListFilter.Module("sharpredis:missing")), Is.Null);
    }

    [Test]
    public void ModuleList()
    {
        var modules = this._redis.Server.ModuleList();
        Assert.That(modules, Is.Not.Null);
        Assert.That(modules.Select(module => module.Name), Is.Unique);
        foreach (var module in modules)
        {
            Assert.That(module.Name, Is.Not.Empty);
            Assert.That(module.Args, Is.Not.Null);
        }
    }
}
