using SharpRedis;
using SharpRedis.Exceptions;
using static SharpRedis.RedisServer;

namespace NET10.Server;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class ServerAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region Memory

    [Test]
    public async Task MemoryDoctorAsync_ReturnsReport()
    {
        var report = await this._redis.Server.MemoryDoctorAsync();
        Assert.That(report, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task MemoryMallocStatsAsync_ReturnsAllocatorReport()
    {
        var report = await this._redis.Server.MemoryMallocStatsAsync();
        Assert.That(report, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task MemoryPurgeAsync_ReturnsOk()
    {
        Assert.That(await this._redis.Server.MemoryPurgeAsync(), Is.True);
    }

    [Test]
    public async Task MemoryStatsAsync_ReturnsAllocationMetrics()
    {
        var stats = await this._redis.Server.MemoryStatsAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(stats, Is.Not.Null);
            Assert.That((long)stats!["total.allocated"], Is.GreaterThan(0));
        }
        Assert.That(stats.ContainsKey("peak.allocated"), Is.True);
    }

    [Test]
    public async Task MemoryUsageAsync_ReturnsBytesForHashAndNullForMissingKey()
    {
        var key = $"memory:usage:{Guid.NewGuid():N}";
        Assert.That(await this._redis.Server.MemoryUsageAsync(key), Is.Null);
        try
        {
            _ = await this._redis.Hash.HSetAsync(key, "field", "value");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(await this._redis.Server.MemoryUsageAsync(key), Is.GreaterThan(0));
                Assert.That(await this._redis.Server.MemoryUsageAsync(key, samples: 1), Is.GreaterThan(0));
                Assert.That(await this._redis.Server.MemoryUsageAsync(key, samples: 0), Is.GreaterThan(0));
            }
        }
        finally
        {
            _ = await this._redis.Key.DelAsync(key);
        }
    }

    #endregion


    #region HotKeys

    /// <summary>
    /// 异步准备 HOTKEYS 测试环境。
    /// </summary>
    private async Task HotKeysPrepareAsync()
    {
        try
        {
            _ = await this._redis.Server.HotKeysStopAsync();
        }
        catch (RedisErrorException)
        {
            // Tracking is not active.
        }

        Assert.That(
            await this._redis.Server.HotKeysResetAsync(),
            Is.True);
    }

    /// <summary>
    /// 异步释放 HOTKEYS 测试环境。
    /// </summary>
    private async Task HotKeysCleanupAsync()
    {
        try
        {
            _ = await this._redis.Server.HotKeysStopAsync();
        }
        catch (RedisErrorException)
        {
            // Tracking is not active.
        }

        _ = await this._redis.Server.HotKeysResetAsync();
    }

    private static async Task HotKeysRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    [NonParallelizable]
    public async Task HotKeysGet_AfterResetAsync()
    {
        await this.HotKeysPrepareAsync();

        try
        {
            var result = await this._redis.Server.HotKeysGetAsync();

            Assert.That(result, Is.Null);
        }
        finally
        {
            await this.HotKeysCleanupAsync();
        }
    }

    [Test]
    [NonParallelizable]
    public async Task HotKeysStart_Get_ActiveAsync()
    {
        await this.HotKeysPrepareAsync();

        try
        {
            Assert.That(
                await this._redis.Server.HotKeysStartAsync(
                    HotKeysMetrics.All,
                    count: 10,
                    sampleRatio: 1),
                Is.True);

            var result = await this._redis.Server.HotKeysGetAsync();

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
            await this.HotKeysCleanupAsync();
        }
    }

    [Test]
    [NonParallelizable]
    public async Task HotKeysStart_CpuAsync()
    {
        await this.HotKeysPrepareAsync();

        try
        {
            Assert.That(
                await this._redis.Server.HotKeysStartAsync(
                    HotKeysMetrics.Cpu,
                    count: 5,
                    sampleRatio: 1),
                Is.True);

            var key = $"String:{{HotKeysStart_CpuAsync}}:{Guid.NewGuid():N}";

            await this._redis.String.SetAsync(key, "hello");

            for (int i = 0; i < 50; i++)
                _ = await this._redis.String.GetAsync(key);

            var result = await this._redis.Server.HotKeysGetAsync();

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
            await this.HotKeysCleanupAsync();
        }
    }

    [Test]
    [NonParallelizable]
    public async Task HotKeysStart_NetAsync()
    {
        await this.HotKeysPrepareAsync();

        try
        {
            Assert.That(
                await this._redis.Server.HotKeysStartAsync(
                    HotKeysMetrics.Net,
                    count: 5,
                    sampleRatio: 1),
                Is.True);

            var key = $"String:{{HotKeysStart_NetAsync}}:{Guid.NewGuid():N}";

            await this._redis.String.SetAsync(key, "hello");

            for (int i = 0; i < 50; i++)
                _ = await this._redis.String.GetAsync(key);

            var result = await this._redis.Server.HotKeysGetAsync();

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
            await this.HotKeysCleanupAsync();
        }
    }

    [Test]
    [NonParallelizable]
    public async Task HotKeysStart_AllMetricsAsync()
    {
        await this.HotKeysPrepareAsync();

        try
        {
            Assert.That(
                await this._redis.Server.HotKeysStartAsync(
                    HotKeysMetrics.All,
                    count: 10,
                    sampleRatio: 1),
                Is.True);

            var key = $"String:{{HotKeysStart_AllMetricsAsync}}:{Guid.NewGuid():N}";

            await this._redis.String.SetAsync(key, "hello");

            for (int i = 0; i < 100; i++)
                _ = await this._redis.String.GetAsync(key);

            var result = await this._redis.Server.HotKeysGetAsync();

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
            await this.HotKeysCleanupAsync();
        }
    }

    [Test]
    [NonParallelizable]
    public async Task HotKeysStart_SampleRatioAsync()
    {
        await this.HotKeysPrepareAsync();

        try
        {
            Assert.That(
                await this._redis.Server.HotKeysStartAsync(
                    HotKeysMetrics.All,
                    count: 10,
                    sampleRatio: 5),
                Is.True);

            var result = await this._redis.Server.HotKeysGetAsync();

            Assert.That(result, Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result!.TrackingActive, Is.True);
                Assert.That(result.SampleRatio, Is.EqualTo(5));
            }
        }
        finally
        {
            await this.HotKeysCleanupAsync();
        }
    }

    [Test]
    [NonParallelizable]
    public async Task HotKeysStart_AlreadyRunningAsync()
    {
        await this.HotKeysPrepareAsync();

        try
        {
            Assert.That(
                await this._redis.Server.HotKeysStartAsync(
                    HotKeysMetrics.All),
                Is.True);

            await HotKeysRedisErrorAsync<RedisErrorException>(
                async () => await this._redis.Server.HotKeysStartAsync(
                    HotKeysMetrics.Cpu));

            var result = await this._redis.Server.HotKeysGetAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TrackingActive, Is.True);
        }
        finally
        {
            await this.HotKeysCleanupAsync();
        }
    }

    [Test]
    [NonParallelizable]
    public async Task HotKeysReset_WhileRunningAsync()
    {
        await this.HotKeysPrepareAsync();

        try
        {
            Assert.That(
                await this._redis.Server.HotKeysStartAsync(
                    HotKeysMetrics.Cpu),
                Is.True);

            await HotKeysRedisErrorAsync<RedisErrorException>(
                async () => await this._redis.Server.HotKeysResetAsync());

            var result = await this._redis.Server.HotKeysGetAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TrackingActive, Is.True);
        }
        finally
        {
            await this.HotKeysCleanupAsync();
        }
    }

    [Test]
    [NonParallelizable]
    public async Task HotKeysStop_PreservesDataAsync()
    {
        await this.HotKeysPrepareAsync();

        try
        {
            Assert.That(
                await this._redis.Server.HotKeysStartAsync(
                    HotKeysMetrics.All,
                    count: 10,
                    sampleRatio: 1),
                Is.True);

            var key = $"String:{{HotKeysStop_PreservesDataAsync}}:{Guid.NewGuid():N}";

            await this._redis.String.SetAsync(key, "hello");

            for (int i = 0; i < 50; i++)
                _ = await this._redis.String.GetAsync(key);

            Assert.That(
                await this._redis.Server.HotKeysStopAsync(),
                Is.True);

            var result = await this._redis.Server.HotKeysGetAsync();

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
            await this.HotKeysCleanupAsync();
        }
    }

    [Test]
    [NonParallelizable]
    public async Task HotKeysReset_ClearsDataAsync()
    {
        await this.HotKeysPrepareAsync();

        try
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            await this._redis.Server.HotKeysStartAsync(
                                HotKeysMetrics.Cpu),
                            Is.True);

                Assert.That(
                    await this._redis.Server.HotKeysGetAsync(),
                    Is.Not.Null);

                Assert.That(
                    await this._redis.Server.HotKeysStopAsync(),
                    Is.True);

                Assert.That(
                    await this._redis.Server.HotKeysResetAsync(),
                    Is.True);
            }

            var result = await this._redis.Server.HotKeysGetAsync();

            Assert.That(result, Is.Null);
        }
        finally
        {
            await this.HotKeysCleanupAsync();
        }
    }

    [Test]
    [NonParallelizable]
    public async Task HotKeysGet_RepeatedCallsAsync()
    {
        await this.HotKeysPrepareAsync();

        try
        {
            Assert.That(
                await this._redis.Server.HotKeysStartAsync(
                    HotKeysMetrics.All,
                    sampleRatio: 1),
                Is.True);

            for (int i = 0; i < 5; i++)
            {
                var result = await this._redis.Server.HotKeysGetAsync();

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
            await this.HotKeysCleanupAsync();
        }
    }

    #endregion


    [Test]
    public async Task CommandAndCountAsync()
    {
        var entries = await this._redis.Server.CommandAsync();
        Assert.That(entries.Length, Is.EqualTo(await this._redis.Server.CommandCountAsync()));
        var get = entries.Single(entry => entry.Name == "get");
        Assert.That(get.Arity, Is.EqualTo(2));
        Assert.That(get.Flags, Does.Contain("readonly"));
    }

    [Test]
    public async Task CommandInfoAsync()
    {
        var entries = await this._redis.Server.CommandInfoAsync(["GET", "sharpredis:unknown", "COMMAND"]);
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
    public async Task CommandDocsAsync()
    {
        var docs = await this._redis.Server.CommandDocsAsync(["SET", "COMMAND", "sharpredis:unknown"]);
        Assert.That(docs, Has.Count.EqualTo(2));
        Assert.That(docs["set"].Summary, Is.Not.Empty);
        Assert.That(docs["set"].Arguments.Any(arg => arg.Name == "key"), Is.True);
        Assert.That(docs["set"].Arguments.Any(arg => arg.Arguments.Length > 0), Is.True);
        Assert.That(docs["command"].Subcommands.ContainsKey("command|list"), Is.True);
        var allDocs = await this._redis.Server.CommandDocsAsync();
        Assert.That(allDocs, Has.Count.GreaterThan(docs.Count));
        Assert.That((await this._redis.Server.CommandInfoAsync()).Any(entry => entry?.Name == "get"), Is.True);
    }

    [Test]
    public async Task CommandGetKeysAsync()
    {
        byte[] binary = [0, 255, 65];
        var keys = await this._redis.Server.CommandGetKeysAsync(["MSET", binary, "one", "other", "two"], CommandFlags.ReturnBytes);
        Assert.That(keys, Has.Length.EqualTo(2));
        Assert.That(keys![0].RawValue, Is.EqualTo(binary));
        Assert.That((string?)keys[1], Is.EqualTo("other"));
    }

    [Test]
    public async Task CommandGetKeysAndFlagsAsync()
    {
        var keys = await this._redis.Server.CommandGetKeysAndFlagsAsync(["COPY", "source", "destination"]);
        Assert.That(keys, Has.Length.EqualTo(2));
        Assert.That((string?)keys[0].Key, Is.EqualTo("source"));
        Assert.That(keys[0].Flags, Does.Contain("access"));
        Assert.That((string?)keys[1].Key, Is.EqualTo("destination"));
        Assert.That(keys[1].Flags, Does.Contain("update"));
    }

    [Test]
    public async Task CommandListAsync()
    {
        Assert.That(await this._redis.Server.CommandListAsync(), Does.Contain("get"));
        Assert.That(await this._redis.Server.CommandListAsync(CommandListFilter.Pattern("ge?")), Is.EqualTo(new[] { "get" }));
        Assert.That(await this._redis.Server.CommandListAsync(CommandListFilter.AclCat("read")), Does.Contain("get"));
        Assert.That(await this._redis.Server.CommandListAsync(CommandListFilter.Module("sharpredis:missing")), Is.Null);
    }

    [Test]
    public async Task ModuleListAsync()
    {
        var modules = await this._redis.Server.ModuleListAsync();
        Assert.That(modules, Is.Not.Null);
        Assert.That(modules.Select(module => module.Name), Is.Unique);
        foreach (var module in modules)
        {
            Assert.That(module.Name, Is.Not.Empty);
            Assert.That(module.Args, Is.Not.Null);
        }
    }
}
