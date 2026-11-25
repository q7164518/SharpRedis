using SharpRedis;
using SharpRedis.Exceptions;
using SharpRedis.Modules;

namespace NET10.CountMinSketch;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class CountMinSketchAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region CMS_IncrBy

    private static async Task CMS_IncrByRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    private static void CMS_IncrByAssertSuccessAsync(
        RedisCountMinSketch.IncrByResult result,
        long expectedCount)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsError, Is.False);
            Assert.That(result.Count, Is.EqualTo(expectedCount));
            Assert.That(result.Error, Is.Null);
        }
    }

    private static void CMS_IncrByAssertOverflowAsync(
        RedisCountMinSketch.IncrByResult result)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsError, Is.True);
            Assert.That(result.Count, Is.Null);
            Assert.That(result.Error, Does.Contain("overflow").IgnoreCase);
        }
    }

    [Test]
    public async Task CMS_IncrBy_SingleAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", 10);

        CMS_IncrByAssertSuccessAsync(result, 10);

        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello");

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(10));
            Assert.That(info.Count, Is.EqualTo(10));
        }
    }

    [Test]
    public async Task CMS_IncrBy_Single_MultipleTimesAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        CMS_IncrByAssertSuccessAsync(
            await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 5), 5);

        CMS_IncrByAssertSuccessAsync(
            await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 10), 15);

        CMS_IncrByAssertSuccessAsync(
            await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 20), 35);

        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello");

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(35));
            Assert.That(info.Count, Is.EqualTo(35));
        }
    }

    [Test]
    public async Task CMS_IncrBy_SingleTupleAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, ((SharpValue)"hello", 12L));

        CMS_IncrByAssertSuccessAsync(result, 12);

        Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello"), Is.EqualTo(12));
    }

    [Test]
    public async Task CMS_IncrBy_SingleKeyValuePairAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, new KeyValuePair<SharpValue, long>("hello", 15));

        CMS_IncrByAssertSuccessAsync(result, 15);

        Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello"), Is.EqualTo(15));
    }

    [Test]
    public async Task CMS_IncrBy_TupleCollectionAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            new List<(SharpValue item, long increment)>
            {
            ("apple", 10),
            ("banana", 20),
            ("orange", 30)
            });

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccessAsync(results[0], 10);
        CMS_IncrByAssertSuccessAsync(results[1], 20);
        CMS_IncrByAssertSuccessAsync(results[2], 30);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        Assert.That(info.Count, Is.EqualTo(60));
    }

    [Test]
    public async Task CMS_IncrBy_KeyValuePairCollectionAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            new List<KeyValuePair<SharpValue, long>>
            {
            new("apple", 5),
            new("banana", 15),
            new("orange", 25)
            });

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccessAsync(results[0], 5);
        CMS_IncrByAssertSuccessAsync(results[1], 15);
        CMS_IncrByAssertSuccessAsync(results[2], 25);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        Assert.That(info.Count, Is.EqualTo(45));
    }

    [Test]
    public async Task CMS_IncrBy_TupleSpanAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ((SharpValue)"apple", 10L),
            ((SharpValue)"banana", 20L),
            ((SharpValue)"orange", 30L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccessAsync(results[0], 10);
        CMS_IncrByAssertSuccessAsync(results[1], 20);
        CMS_IncrByAssertSuccessAsync(results[2], 30);
    }

    [Test]
    public async Task CMS_IncrBy_KeyValuePairSpanAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                new KeyValuePair<SharpValue, long>("apple", 10),
            new KeyValuePair<SharpValue, long>("banana", 20),
            new KeyValuePair<SharpValue, long>("orange", 30)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccessAsync(results[0], 10);
        CMS_IncrByAssertSuccessAsync(results[1], 20);
        CMS_IncrByAssertSuccessAsync(results[2], 30);
    }

    [Test]
    public async Task CMS_IncrBy_DuplicateItemsAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ((SharpValue)"hello", 5L),
            ((SharpValue)"hello", 10L),
            ((SharpValue)"hello", 20L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccessAsync(results[0], 5);
        CMS_IncrByAssertSuccessAsync(results[1], 15);
        CMS_IncrByAssertSuccessAsync(results[2], 35);

        Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello"), Is.EqualTo(35));
    }

    [Test]
    public async Task CMS_IncrBy_MixedOverloadsAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        CMS_IncrByAssertSuccessAsync(
            await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "a", 10), 10);

        CMS_IncrByAssertSuccessAsync(
            await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, ((SharpValue)"a", 5L)), 15);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ((SharpValue)"a", 10L),
            ((SharpValue)"b", 20L)
            ]);

        Assert.That(results, Has.Length.EqualTo(2));

        CMS_IncrByAssertSuccessAsync(results[0], 25);
        CMS_IncrByAssertSuccessAsync(results[1], 20);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        Assert.That(info.Count, Is.EqualTo(45));
    }

    [Test]
    public async Task CMS_IncrBy_ZeroIncrementAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        CMS_IncrByAssertSuccessAsync(
            await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 10), 10);

        CMS_IncrByAssertSuccessAsync(
            await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 0), 10);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        Assert.That(info.Count, Is.EqualTo(10));
    }

    [Test]
    public async Task CMS_IncrBy_LargeIncrementAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        const long increment = 1_000_000_000L;

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", increment);

        CMS_IncrByAssertSuccessAsync(result, increment);

        Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello"), Is.EqualTo(increment));
    }

    [Test]
    public async Task CMS_IncrBy_Single_OverflowAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", 4_294_967_296L);

        CMS_IncrByAssertOverflowAsync(result);

        Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello"), Is.Zero);
    }

    [Test]
    public async Task CMS_IncrBy_Single_OverflowAfterIncrementAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        CMS_IncrByAssertSuccessAsync(
            await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 4_000_000_000L),
            4_000_000_000L);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", 500_000_000L);

        CMS_IncrByAssertOverflowAsync(result);

        Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello"), Is.EqualTo(4000000000));
    }

    [Test]
    public async Task CMS_IncrBy_Multiple_PartialOverflowAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ((SharpValue)"k", 12L),
                ((SharpValue)"kk", 4_294_967_296L),
                ((SharpValue)"k3", 1024L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccessAsync(results[0], 12);
        CMS_IncrByAssertOverflowAsync(results[1]);
        CMS_IncrByAssertSuccessAsync(results[2], 1024);

        var k = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "k");

        var kk = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "kk");

        var k3 = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "k3");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(k, Is.EqualTo(12));
            Assert.That(kk, Is.Zero);
            Assert.That(k3, Is.EqualTo(1024));
        }
    }

    [Test]
    public async Task CMS_IncrBy_Multiple_DuplicateOverflowAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ("hello", 4_000_000_000L),
                ("hello", 500_000_000L),
                ("other", 25L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccessAsync(results[0], 4_000_000_000L);
        CMS_IncrByAssertOverflowAsync(results[1]);
        CMS_IncrByAssertSuccessAsync(results[2], 25);

        Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello"), Is.EqualTo(4000000000));
    }

    [Test]
    public async Task CMS_IncrBy_Multiple_MultipleErrorsAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ((SharpValue)"a", 4_294_967_296L),
            ((SharpValue)"b", 5L),
            ((SharpValue)"c", 4_294_967_296L),
            ((SharpValue)"d", 10L)
            ]);

        Assert.That(results, Has.Length.EqualTo(4));

        CMS_IncrByAssertOverflowAsync(results[0]);
        CMS_IncrByAssertSuccessAsync(results[1], 5);
        CMS_IncrByAssertOverflowAsync(results[2]);
        CMS_IncrByAssertSuccessAsync(results[3], 10);
    }

    [Test]
    public async Task CMS_IncrBy_KeyNotExistsAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        await CMS_IncrByRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 10));
    }

    [Test]
    public async Task CMS_IncrBy_Multiple_KeyNotExistsAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        await CMS_IncrByRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key,
                [
                    ((SharpValue)"a", 1L),
                ((SharpValue)"b", 2L)
                ]));
    }

    [Test]
    public async Task CMS_IncrBy_WrongKeyTypeAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CMS_IncrByRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 1));
    }

    [Test]
    public async Task CMS_IncrBy_BloomKeyWrongTypeAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.True);

        await CMS_IncrByRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 1));
    }

    [Test]
    public async Task CMS_IncrBy_SpecialItemsAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ((SharpValue)"", 5L),
            ((SharpValue)"你好🌍", 10L),
            ((SharpValue)"a\nb\tc", 15L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccessAsync(results[0], 5);
        CMS_IncrByAssertSuccessAsync(results[1], 10);
        CMS_IncrByAssertSuccessAsync(results[2], 15);
    }

    [Test]
    public async Task CMS_IncrBy_CaseSensitiveAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ((SharpValue)"Hello", 10L),
            ((SharpValue)"hello", 20L),
            ((SharpValue)"HELLO", 30L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccessAsync(results[0], 10);
        CMS_IncrByAssertSuccessAsync(results[1], 20);
        CMS_IncrByAssertSuccessAsync(results[2], 30);
    }

    [Test]
    public async Task CMS_IncrBy_Flags_NoneAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", 10,
            flags: CommandFlags.None);

        CMS_IncrByAssertSuccessAsync(result, 10);
    }

    [Test]
    public async Task CMS_IncrBy_CancellationToken_DefaultAsync()
    {
        var key = $"Sketch:{{CMS_IncrByAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", 10,
            cancellationToken: CancellationToken.None);

        CMS_IncrByAssertSuccessAsync(result, 10);
    }
    #endregion

    #region CMS_InitByDim

    private static async Task CMS_InitByDimRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task CMS_InitByDim_DefaultAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByDim_MinimumDimensionsAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 1, 1), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(1));
            Assert.That(info.Depth, Is.EqualTo(1));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [TestCase(1L, 1L)]
    [TestCase(1L, 10L)]
    [TestCase(10L, 1L)]
    [TestCase(10L, 10L)]
    [TestCase(100L, 5L)]
    [TestCase(1000L, 7L)]
    [TestCase(2000L, 5L)]
    [TestCase(4096L, 16L)]
    public async Task CMS_InitByDim_ValidDimensionsAsync(
        long width, long depth)
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, width, depth), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(width));
            Assert.That(info.Depth, Is.EqualTo(depth));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [TestCase(3L)]
    [TestCase(7L)]
    [TestCase(13L)]
    [TestCase(101L)]
    [TestCase(1001L)]
    public async Task CMS_InitByDim_Width_NotPowerOfTwoAsync(long width)
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, width, 5), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        Assert.That(info.Width, Is.EqualTo(width));
    }

    [Test]
    public async Task CMS_InitByDim_Width_ZeroAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 0, 5));
    }

    [TestCase(-1L)]
    [TestCase(-10L)]
    [TestCase(-100L)]
    public async Task CMS_InitByDim_Width_NegativeAsync(long width)
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, width, 5));
    }

    [Test]
    public async Task CMS_InitByDim_Depth_ZeroAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 2000, 0));
    }

    [TestCase(-1L)]
    [TestCase(-10L)]
    [TestCase(-100L)]
    public async Task CMS_InitByDim_Depth_NegativeAsync(long depth)
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 2000, depth));
    }

    [TestCase(0L, 0L)]
    [TestCase(-1L, -1L)]
    [TestCase(0L, -1L)]
    [TestCase(-1L, 0L)]
    public async Task CMS_InitByDim_BothDimensions_InvalidAsync(
        long width, long depth)
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, width, depth));
    }

    [Test]
    public async Task CMS_InitByDim_InvalidDimensions_NoCreateAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 0, 5));

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InfoAsync(key));

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 100, 5), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
        }
    }

    [Test]
    public async Task CMS_InitByDim_KeyAlreadyExistsAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 2000, 5));

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByDim_KeyAlreadyExists_DifferentDimensionsAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 100, 3), Is.True);

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 2000, 10));

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(3));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByDim_KeyAlreadyHasDataAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 100, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Count, Is.EqualTo(10));
        }

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 2000, 10));

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);
        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.EqualTo(10));
            Assert.That(count, Is.EqualTo(10));
        }
    }

    [Test]
    public async Task CMS_InitByDim_KeyAlreadyCreatedByProbAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, 0.001, 0.01), Is.True);

        var before = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 100, 5));

        var after = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByDim_WrongKeyTypeAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 100, 5));
    }

    [Test]
    public async Task CMS_InitByDim_BloomKeyWrongTypeAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.True);

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 100, 5));
    }

    [Test]
    public async Task CMS_InitByDim_CuckooKeyWrongTypeAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        await CMS_InitByDimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 100, 5));
    }

    [Test]
    public async Task CMS_InitByDim_DifferentKeysAsync()
    {
        var key1 = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key1, 100, 3), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key2, 2000, 7), Is.True);
        }

        var info1 = await this._redis.Modules.Sketch.CMS_InfoAsync(key1);
        var info2 = await this._redis.Modules.Sketch.CMS_InfoAsync(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info1.Width, Is.EqualTo(100));
            Assert.That(info1.Depth, Is.EqualTo(3));
            Assert.That(info1.Count, Is.Zero);

            Assert.That(info2.Width, Is.EqualTo(2000));
            Assert.That(info2.Depth, Is.EqualTo(7));
            Assert.That(info2.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByDim_InitialCountAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var counts = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["a", "b", "c"]);

        Assert.That(counts, Is.EqualTo(new long[] { 0, 0, 0 }));

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        Assert.That(info.Count, Is.Zero);
    }

    [Test]
    public async Task CMS_InitByDim_AfterIncrementAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", 15);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Count, Is.EqualTo(15));
        }

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.EqualTo(15));
        }
    }

    [Test]
    public async Task CMS_InitByDim_Flags_NoneAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 100, 5,
            flags: CommandFlags.None), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
        }
    }

    [Test]
    public async Task CMS_InitByDim_CancellationToken_DefaultAsync()
    {
        var key = $"Sketch:{{CMS_InitByDimAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 100, 5,
            cancellationToken: CancellationToken.None), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
        }
    }

    #endregion

    #region CMS_InitByProb

    private static async Task CMS_InitByProbRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task CMS_InitByProb_DefaultAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, 0.001, 0.01), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [TestCase(0.5, 0.5)]
    [TestCase(0.1, 0.1)]
    [TestCase(0.01, 0.01)]
    [TestCase(0.001, 0.001)]
    [TestCase(0.0001, 0.01)]
    [TestCase(0.01, 0.0001)]
    [TestCase(0.00001, 0.001)]
    [TestCase(0.9, 0.9)]
    public async Task CMS_InitByProb_ValidParametersAsync(
        double error, double probability)
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, error, probability), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByProb_Error_AffectsWidthAsync()
    {
        var key1 = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                    key1, 0.1, 0.01), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key2, 0.001, 0.01), Is.True);
        }

        var info1 = await this._redis.Modules.Sketch.CMS_InfoAsync(key1);
        var info2 = await this._redis.Modules.Sketch.CMS_InfoAsync(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info2.Width, Is.GreaterThan(info1.Width));
            Assert.That(info2.Depth, Is.EqualTo(info1.Depth));
            Assert.That(info1.Count, Is.Zero);
            Assert.That(info2.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByProb_Probability_AffectsDepthAsync()
    {
        var key1 = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                    key1, 0.01, 0.1), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key2, 0.01, 0.0001), Is.True);
        }

        var info1 = await this._redis.Modules.Sketch.CMS_InfoAsync(key1);
        var info2 = await this._redis.Modules.Sketch.CMS_InfoAsync(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info2.Width, Is.EqualTo(info1.Width));
            Assert.That(info2.Depth, Is.GreaterThan(info1.Depth));
            Assert.That(info1.Count, Is.Zero);
            Assert.That(info2.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByProb_SameParameters_SameDimensionsAsync()
    {
        var key1 = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                    key1, 0.001, 0.01), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key2, 0.001, 0.01), Is.True);
        }

        var info1 = await this._redis.Modules.Sketch.CMS_InfoAsync(key1);
        var info2 = await this._redis.Modules.Sketch.CMS_InfoAsync(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info2.Width, Is.EqualTo(info1.Width));
            Assert.That(info2.Depth, Is.EqualTo(info1.Depth));
            Assert.That(info1.Count, Is.Zero);
            Assert.That(info2.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByProb_Error_ZeroAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0, 0.01));
    }

    [TestCase(-0.001)]
    [TestCase(-0.1)]
    [TestCase(-1.0)]
    [TestCase(-10.0)]
    public async Task CMS_InitByProb_Error_NegativeAsync(double error)
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, error, 0.01));
    }

    [TestCase(1.0)]
    [TestCase(1.5)]
    [TestCase(2.0)]
    [TestCase(10.0)]
    public async Task CMS_InitByProb_Error_GreaterThanOrEqualOneAsync(
        double error)
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, error, 0.01));
    }

    [Test]
    public async Task CMS_InitByProb_Probability_ZeroAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0.01, 0));
    }

    [TestCase(-0.001)]
    [TestCase(-0.1)]
    [TestCase(-1.0)]
    [TestCase(-10.0)]
    public async Task CMS_InitByProb_Probability_NegativeAsync(
        double probability)
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0.01, probability));
    }

    [TestCase(1.0)]
    [TestCase(1.5)]
    [TestCase(2.0)]
    [TestCase(10.0)]
    public async Task CMS_InitByProb_Probability_GreaterThanOrEqualOneAsync(
        double probability)
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0.01, probability));
    }

    [TestCase(0.0, 0.0)]
    [TestCase(-0.1, -0.1)]
    [TestCase(1.0, 1.0)]
    [TestCase(-1.0, 2.0)]
    [TestCase(2.0, -1.0)]
    public async Task CMS_InitByProb_BothParameters_InvalidAsync(
        double error, double probability)
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, error, probability));
    }

    [Test]
    public async Task CMS_InitByProb_InvalidParameters_NoCreateAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0, 0.01));

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InfoAsync(key));

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, 0.01, 0.01), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByProb_KeyAlreadyExistsAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, 0.001, 0.01), Is.True);

        var before = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0.001, 0.01));

        var after = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByProb_KeyAlreadyExists_DifferentParametersAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, 0.1, 0.1), Is.True);

        var before = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0.0001, 0.0001));

        var after = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByProb_KeyAlreadyHasDataAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, 0.001, 0.01), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", 15);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Count, Is.EqualTo(15));
        }

        var before = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0.1, 0.1));

        var after = await this._redis.Modules.Sketch.CMS_InfoAsync(key);
        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.EqualTo(15));
            Assert.That(count, Is.EqualTo(15));
        }
    }

    [Test]
    public async Task CMS_InitByProb_KeyAlreadyCreatedByDimAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 100, 5), Is.True);

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0.001, 0.01));

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByProb_WrongKeyTypeAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0.001, 0.01));
    }

    [Test]
    public async Task CMS_InitByProb_BloomKeyWrongTypeAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.True);

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0.001, 0.01));
    }

    [Test]
    public async Task CMS_InitByProb_CuckooKeyWrongTypeAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        await CMS_InitByProbRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key, 0.001, 0.01));
    }

    [Test]
    public async Task CMS_InitByProb_DifferentKeysAsync()
    {
        var key1 = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                    key1, 0.1, 0.1), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
                key2, 0.001, 0.001), Is.True);
        }

        var info1 = await this._redis.Modules.Sketch.CMS_InfoAsync(key1);
        var info2 = await this._redis.Modules.Sketch.CMS_InfoAsync(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info2.Width, Is.GreaterThan(info1.Width));
            Assert.That(info2.Depth, Is.GreaterThan(info1.Depth));
            Assert.That(info1.Count, Is.Zero);
            Assert.That(info2.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_InitByProb_InitialCountAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, 0.001, 0.01), Is.True);

        var counts = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["a", "b", "c"]);

        Assert.That(counts, Is.EqualTo(new long[] { 0, 0, 0 }));

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        Assert.That(info.Count, Is.Zero);
    }

    [Test]
    public async Task CMS_InitByProb_AfterIncrementAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, 0.001, 0.01), Is.True);

        var before = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", 20);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Count, Is.EqualTo(20));
        }

        var after = await this._redis.Modules.Sketch.CMS_InfoAsync(key);
        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.EqualTo(20));
            Assert.That(count, Is.EqualTo(20));
        }
    }

    [Test]
    public async Task CMS_InitByProb_Flags_NoneAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, 0.001, 0.01,
            flags: CommandFlags.None), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task CMS_InitByProb_CancellationToken_DefaultAsync()
    {
        var key = $"Sketch:{{CMS_InitByProbAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, 0.001, 0.01,
            cancellationToken: CancellationToken.None), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
        }
    }

    #endregion

    #region CMS_Merge

    private static async Task CMS_MergeRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    /// <summary>
    /// 辅助创建指定维度的 Sketch。
    /// </summary>
    private async Task CMS_MergeInitAsync(params string[] keys)
    {
        foreach (var key in keys)
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key, 1000, 5), Is.True);
        }
    }

    /// <summary>
    /// 辅助添加计数，并检查 INCRBY 返回值。
    /// </summary>
    private async Task CMS_MergeIncrementAsync(
        string key, string item, long increment)
    {
        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, item, increment);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Count, Is.EqualTo(increment));
        }
    }

    [Test]
    public async Task CMS_Merge_Single_DefaultWeightAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this.CMS_MergeInitAsync(destination, source);
        await this.CMS_MergeIncrementAsync(source, "apple", 10);

        Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
            destination, source), Is.True);

        var destCount = await this._redis.Modules.Sketch.CMS_QueryAsync(
            destination, "apple");
        var sourceCount = await this._redis.Modules.Sketch.CMS_QueryAsync(
            source, "apple");
        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(destCount, Is.EqualTo(10));
            Assert.That(sourceCount, Is.EqualTo(10));
            Assert.That(info.Count, Is.EqualTo(10));
        }
    }

    [Test]
    public async Task CMS_Merge_Single_CustomWeightAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this.CMS_MergeInitAsync(destination, source);
        await this.CMS_MergeIncrementAsync(source, "apple", 10);

        Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
            destination, source, weight: 3), Is.True);

        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            destination, "apple");
        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(30));
            Assert.That(info.Count, Is.EqualTo(30));
        }
    }

    [Test]
    public async Task CMS_Merge_Multiple_DefaultWeightsAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_MergeAsync}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_MergeAsync}}:{id}:source2";

        await this.CMS_MergeInitAsync(destination, source1, source2);
        await this.CMS_MergeIncrementAsync(source1, "apple", 10);
        await this.CMS_MergeIncrementAsync(source2, "apple", 20);

        Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
            destination, sources: [source1, source2]), Is.True);

        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            destination, "apple");
        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(30));
            Assert.That(info.Count, Is.EqualTo(30));
        }
    }

    [Test]
    public async Task CMS_Merge_Multiple_CustomWeightsAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_MergeAsync}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_MergeAsync}}:{id}:source2";

        await this.CMS_MergeInitAsync(destination, source1, source2);
        await this.CMS_MergeIncrementAsync(source1, "apple", 10);
        await this.CMS_MergeIncrementAsync(source2, "apple", 20);

        Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
            destination,
            sources: [source1, source2],
            weights: [2, 3]), Is.True);

        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            destination, "apple");
        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(80));
            Assert.That(info.Count, Is.EqualTo(80));
        }
    }

    [Test]
    public async Task CMS_Merge_TupleCollectionAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_MergeAsync}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_MergeAsync}}:{id}:source2";

        await this.CMS_MergeInitAsync(destination, source1, source2);
        await this.CMS_MergeIncrementAsync(source1, "apple", 5);
        await this.CMS_MergeIncrementAsync(source2, "apple", 10);

        var sources = new List<(SharpValue source, long weight)>
        {
            (source1, 2),
            (source2, 4)
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
                    destination, sources), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                destination, "apple"), Is.EqualTo(50));
        }
    }

    [Test]
    public async Task CMS_Merge_KeyValuePairCollectionAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_MergeAsync}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_MergeAsync}}:{id}:source2";

        await this.CMS_MergeInitAsync(destination, source1, source2);
        await this.CMS_MergeIncrementAsync(source1, "apple", 5);
        await this.CMS_MergeIncrementAsync(source2, "apple", 10);

        var sources = new List<KeyValuePair<SharpValue, long>>
        {
            new(source1, 2),
            new(source2, 4)
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
                    destination, sources), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                destination, "apple"), Is.EqualTo(50));
        }
    }

    [Test]
    public async Task CMS_Merge_TupleSpanAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_MergeAsync}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_MergeAsync}}:{id}:source2";

        await this.CMS_MergeInitAsync(destination, source1, source2);
        await this.CMS_MergeIncrementAsync(source1, "apple", 5);
        await this.CMS_MergeIncrementAsync(source2, "apple", 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination,
                sourceWeights:
                [
                    (source1, 2L),
                    (source2, 4L)
                ]), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                destination, "apple"), Is.EqualTo(50));
        }
    }

    [Test]
    public async Task CMS_Merge_KeyValuePairSpanAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_MergeAsync}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_MergeAsync}}:{id}:source2";

        await this.CMS_MergeInitAsync(destination, source1, source2);
        await this.CMS_MergeIncrementAsync(source1, "apple", 5);
        await this.CMS_MergeIncrementAsync(source2, "apple", 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination,
                sourceWeights: (ReadOnlySpan<KeyValuePair<SharpValue, long>>)
                [
                    new(source1, 2),
                    new(source2, 4)
                ]), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                destination, "apple"), Is.EqualTo(50));
        }
    }

    [Test]
    public async Task CMS_Merge_DifferentItemsAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_MergeAsync}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_MergeAsync}}:{id}:source2";

        await this.CMS_MergeInitAsync(destination, source1, source2);
        await this.CMS_MergeIncrementAsync(source1, "apple", 10);
        await this.CMS_MergeIncrementAsync(source2, "banana", 20);

        Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
            destination, sources: [source1, source2]), Is.True);

        var apple = await this._redis.Modules.Sketch.CMS_QueryAsync(
            destination, "apple");
        var banana = await this._redis.Modules.Sketch.CMS_QueryAsync(
            destination, "banana");

        var destInfo = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);
        var source1Info = await this._redis.Modules.Sketch.CMS_InfoAsync(source1);
        var source2Info = await this._redis.Modules.Sketch.CMS_InfoAsync(source2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(apple, Is.EqualTo(10));
            Assert.That(banana, Is.EqualTo(20));
            Assert.That(destInfo.Count, Is.EqualTo(30));
            Assert.That(source1Info.Count, Is.EqualTo(10));
            Assert.That(source2Info.Count, Is.EqualTo(20));
        }
    }

    [Test]
    public async Task CMS_Merge_RepeatedMergeAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this.CMS_MergeInitAsync(destination, source);
        await this.CMS_MergeIncrementAsync(source, "apple", 10);

        Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
            destination, source), Is.True);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
                    destination, source), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                destination, "apple"), Is.EqualTo(10));
        }
    }

    [Test]
    public async Task CMS_Merge_OverwriteDestinationAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this.CMS_MergeInitAsync(destination, source);
        await this.CMS_MergeIncrementAsync(destination, "apple", 100);
        await this.CMS_MergeIncrementAsync(source, "apple", 10);

        Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
            destination, source), Is.True);

        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            destination, "apple");
        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(10));
            Assert.That(info.Count, Is.EqualTo(10));
        }
    }

    [Test]
    public async Task CMS_Merge_ZeroWeightAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_MergeAsync}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_MergeAsync}}:{id}:source2";

        await this.CMS_MergeInitAsync(destination, source1, source2);
        await this.CMS_MergeIncrementAsync(source1, "apple", 10);
        await this.CMS_MergeIncrementAsync(source2, "apple", 20);

        Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
            destination,
            sources: [source1, source2],
            weights: [0, 2]), Is.True);

        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            destination, "apple");
        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(40));
            Assert.That(info.Count, Is.EqualTo(40));
        }
    }

    [Test]
    public async Task CMS_Merge_DimensionsPreservedAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    destination, 123, 7), Is.True);
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                source, 123, 7), Is.True);
        }

        await this.CMS_MergeIncrementAsync(source, "apple", 10);

        Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
            destination, source), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(123));
            Assert.That(info.Depth, Is.EqualTo(7));
            Assert.That(info.Count, Is.EqualTo(10));
        }
    }

    [Test]
    public async Task CMS_Merge_DestinationNotExistsAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this.CMS_MergeInitAsync(source);

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, source));
    }

    [Test]
    public async Task CMS_Merge_SourceNotExistsAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this.CMS_MergeInitAsync(destination);

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, source));
    }

    [Test]
    public async Task CMS_Merge_Multiple_SourceNotExistsAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_MergeAsync}}:{id}:source1";
        var missing = $"Sketch:{{CMS_MergeAsync}}:{id}:missing";

        await this.CMS_MergeInitAsync(destination, source1);

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, sources: [source1, missing]));

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);

        Assert.That(info.Count, Is.Zero);
    }

    [Test]
    public async Task CMS_Merge_WidthMismatchAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    destination, 100, 5), Is.True);
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                source, 200, 5), Is.True);
        }

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, source));
    }

    [Test]
    public async Task CMS_Merge_DepthMismatchAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    destination, 100, 5), Is.True);
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                source, 100, 7), Is.True);
        }

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, source));
    }

    [Test]
    public async Task CMS_Merge_Multiple_DimensionMismatchAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_MergeAsync}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_MergeAsync}}:{id}:source2";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    destination, 100, 5), Is.True);
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                source1, 100, 5), Is.True);
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                source2, 200, 5), Is.True);
        }

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, sources: [source1, source2]));

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);

        Assert.That(info.Count, Is.Zero);
    }

    [Test]
    public async Task CMS_Merge_WeightsLengthMismatchAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_MergeAsync}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_MergeAsync}}:{id}:source2";

        await this.CMS_MergeInitAsync(destination, source1, source2);

        await CMS_MergeRedisErrorAsync<NotSupportedException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination,
                sources: [source1, source2],
                weights: [2]));
    }

    [Test]
    public async Task CMS_Merge_Overflow_NoChangesAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    destination, 100, 5), Is.True);
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                source, 100, 5), Is.True);
        }

        await this.CMS_MergeIncrementAsync(destination, "old", 7);
        await this.CMS_MergeIncrementAsync(source, "apple", 3_000_000_000L);

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, source, weight: 2));

        var oldCount = await this._redis.Modules.Sketch.CMS_QueryAsync(
            destination, "old");
        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);
        var sourceCount = await this._redis.Modules.Sketch.CMS_QueryAsync(
            source, "apple");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(oldCount, Is.EqualTo(7));
            Assert.That(info.Count, Is.EqualTo(7));
            Assert.That(sourceCount, Is.EqualTo(3_000_000_000L));
        }
    }

    [Test]
    public async Task CMS_Merge_NegativeWeight_UnderflowAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    destination, 100, 5), Is.True);
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                source, 100, 5), Is.True);
        }

        await this.CMS_MergeIncrementAsync(source, "apple", 10);

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, source, weight: -1));

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(destination);

        Assert.That(info.Count, Is.Zero);
    }

    [Test]
    public async Task CMS_Merge_DestinationWrongKeyTypeAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this._redis.String.SetAsync(destination, "hello");
        await this.CMS_MergeInitAsync(source);

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, source));
    }

    [Test]
    public async Task CMS_Merge_SourceWrongKeyTypeAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this.CMS_MergeInitAsync(destination);
        await this._redis.String.SetAsync(source, "hello");

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, source));
    }

    [Test]
    public async Task CMS_Merge_BloomKeyWrongTypeAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this.CMS_MergeInitAsync(destination);

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            source, "hello"), Is.True);

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, source));
    }

    [Test]
    public async Task CMS_Merge_CuckooKeyWrongTypeAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this.CMS_MergeInitAsync(destination);

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            source, "hello"), Is.True);

        await CMS_MergeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_MergeAsync(
                destination, source));
    }

    [Test]
    public async Task CMS_Merge_Flags_NoneAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this.CMS_MergeInitAsync(destination, source);

        Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
            destination, source,
            flags: CommandFlags.None), Is.True);
    }

    [Test]
    public async Task CMS_Merge_CancellationToken_DefaultAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_MergeAsync}}:{id}:dest";
        var source = $"Sketch:{{CMS_MergeAsync}}:{id}:source";

        await this.CMS_MergeInitAsync(destination, source);

        Assert.That(await this._redis.Modules.Sketch.CMS_MergeAsync(
            destination, source,
            cancellationToken: CancellationToken.None), Is.True);
    }

    #endregion

    #region CMS_Query

    private static async Task CMS_QueryRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task CMS_Query_Single_EmptySketchAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 1000, 5), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, "hello"), Is.Zero);
        }
    }

    [Test]
    public async Task CMS_Query_Multiple_EmptySketchAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 1000, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["a", "b", "c", "d"]);

        Assert.That(result, Is.EqualTo(new long[] { 0, 0, 0, 0 }));
    }

    [Test]
    public async Task CMS_Query_Single_ExistingItemAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 1000, 5), Is.True);

        var added = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(added.IsSuccess, Is.True);
            Assert.That(added.Count, Is.EqualTo(10));

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, "hello"), Is.EqualTo(10));
        }
    }

    [Test]
    public async Task CMS_Query_Single_MultipleIncrementsAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 1000, 5), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 5)).IsSuccess, Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, "hello"), Is.EqualTo(5));

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 10)).IsSuccess, Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                    key, "hello"), Is.EqualTo(15));

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 20)).IsSuccess, Is.True);
        }

        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello");

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(35));
            Assert.That(info.Count, Is.EqualTo(35));
        }
    }

    [Test]
    public async Task CMS_Query_Multiple_SingleItemAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 1000, 5), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 12)).IsSuccess, Is.True);
        }

        var result = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["hello"]);

        Assert.That(result, Is.EqualTo(new long[] { 12 }));
    }

    [Test]
    public async Task CMS_Query_Multiple_ExistingItemsAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 1000, 5), Is.True);

        var added = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ((SharpValue)"apple", 10L),
            ((SharpValue)"banana", 20L),
            ((SharpValue)"orange", 30L)
            ]);

        Assert.That(added, Has.Length.EqualTo(3));

        foreach (var item in added)
            Assert.That(item.IsSuccess, Is.True);

        var counts = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["apple", "banana", "orange"]);

        var apple = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "apple");

        var banana = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "banana");

        var orange = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "orange");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(counts, Has.Length.EqualTo(3));

            Assert.That(counts[0], Is.GreaterThanOrEqualTo(10));
            Assert.That(counts[1], Is.GreaterThanOrEqualTo(20));
            Assert.That(counts[2], Is.GreaterThanOrEqualTo(30));

            Assert.That(counts[0], Is.EqualTo(apple));
            Assert.That(counts[1], Is.EqualTo(banana));
            Assert.That(counts[2], Is.EqualTo(orange));
        }
    }

    [Test]
    public async Task CMS_Query_Multiple_ResultOrderAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 1000, 5), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "a", 5)).IsSuccess, Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "b", 10)).IsSuccess, Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "c", 15)).IsSuccess, Is.True);
        }

        var result = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["c", "a", "b", "a", "c"]);

        var c = await this._redis.Modules.Sketch.CMS_QueryAsync(key, "c");
        var a = await this._redis.Modules.Sketch.CMS_QueryAsync(key, "a");
        var b = await this._redis.Modules.Sketch.CMS_QueryAsync(key, "b");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(5));
            Assert.That(result[0], Is.EqualTo(c));
            Assert.That(result[1], Is.EqualTo(a));
            Assert.That(result[2], Is.EqualTo(b));
            Assert.That(result[3], Is.EqualTo(a));
            Assert.That(result[4], Is.EqualTo(c));
        }
    }

    [Test]
    public async Task CMS_Query_Multiple_DuplicateQueriesAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 1000, 5), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 25)).IsSuccess, Is.True);
        }

        var result = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["hello", "hello", "hello"]);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(new long[] { 25, 25, 25 }));
            Assert.That(info.Count, Is.EqualTo(25));
        }
    }

    [Test]
    public async Task CMS_Query_Multiple_MixedItemsAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 1000, 5), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "known", 100)).IsSuccess, Is.True);
        }

        var result = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["unknown1", "known", "unknown2", "known"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(4));
            Assert.That(result[0], Is.GreaterThanOrEqualTo(0));
            Assert.That(result[1], Is.EqualTo(100));
            Assert.That(result[2], Is.GreaterThanOrEqualTo(0));
            Assert.That(result[3], Is.EqualTo(100));
        }
    }

    [Test]
    public async Task CMS_Query_Collision_WidthOneDepthOneAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 1, 1), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "apple", 10)).IsSuccess, Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "banana", 20)).IsSuccess, Is.True);
        }

        var result = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["apple", "banana", "never-inserted"]);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(new long[] { 30, 30, 30 }));
            Assert.That(info.Count, Is.EqualTo(30));
        }
    }

    [Test]
    public async Task CMS_Query_ReadOnlyAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 1000, 5), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 50)).IsSuccess, Is.True);
        }

        var before = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, "hello"), Is.EqualTo(50));

            var result = await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, ["hello", "hello"]);

            Assert.That(result, Is.EqualTo(new long[] { 50, 50 }));
        }

        var after = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.EqualTo(before.Count));
        }
    }

    [Test]
    public async Task CMS_Query_Overflow_SaturatedValueAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 1000, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", (long)uint.MaxValue + 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.Error, Does.Contain("overflow").IgnoreCase);
        }

        var count = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello");

        var counts = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["hello", "hello"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.Zero);
            Assert.That(counts, Is.EqualTo(
            [
                0,
                0
            ]));
        }
    }

    [Test]
    public async Task CMS_Query_CountBoundariesAsync()
    {
        var key1 = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key1, 1000, 5), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key2, 1000, 5), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                key1, "hello"), Is.Zero);
        }

        var added = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key2, "hello", (long)uint.MaxValue);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(added.IsSuccess, Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                key2, "hello"), Is.EqualTo((long)uint.MaxValue));
        }
    }

    [Test]
    public async Task CMS_Query_SpecialItemsAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var added = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ((SharpValue)"", 5L),
            ((SharpValue)"你好🌍", 10L),
            ((SharpValue)"a b c", 15L),
            ((SharpValue)"a\nb\tc", 20L)
            ]);

        Assert.That(added, Has.Length.EqualTo(4));

        foreach (var item in added)
            Assert.That(item.IsSuccess, Is.True);

        var result = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["", "你好🌍", "a b c", "a\nb\tc"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(4));
            Assert.That(result[0], Is.GreaterThanOrEqualTo(5));
            Assert.That(result[1], Is.GreaterThanOrEqualTo(10));
            Assert.That(result[2], Is.GreaterThanOrEqualTo(15));
            Assert.That(result[3], Is.GreaterThanOrEqualTo(20));
        }
    }

    [Test]
    public async Task CMS_Query_CaseSensitiveAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 2000, 5), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "Hello", 10)).IsSuccess, Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 20)).IsSuccess, Is.True);
        }

        var result = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["Hello", "hello", "HELLO"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result[0], Is.GreaterThanOrEqualTo(10));
            Assert.That(result[1], Is.GreaterThanOrEqualTo(20));
            Assert.That(result[2], Is.GreaterThanOrEqualTo(0));
        }
    }

    [Test]
    public async Task CMS_Query_DifferentKeysAsync()
    {
        var key1 = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key1, 1000, 5), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key2, 1000, 5), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key1, "hello", 10)).IsSuccess, Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key2, "hello", 30)).IsSuccess, Is.True);
        }

        var count1 = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key1, "hello");

        var count2 = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key2, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count1, Is.EqualTo(10));
            Assert.That(count2, Is.EqualTo(30));
        }
    }

    [Test]
    public async Task CMS_Query_Single_KeyNotExistsAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        await CMS_QueryRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, "hello"));
    }

    [Test]
    public async Task CMS_Query_Multiple_KeyNotExistsAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        await CMS_QueryRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, ["a", "b"]));
    }

    [Test]
    public async Task CMS_Query_WrongKeyTypeAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CMS_QueryRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, "hello"));

        await CMS_QueryRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, ["a", "b"]));
    }

    [Test]
    public async Task CMS_Query_BloomKeyWrongTypeAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.True);

        await CMS_QueryRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, "hello"));
    }

    [Test]
    public async Task CMS_Query_CuckooKeyWrongTypeAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        await CMS_QueryRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, ["hello"]));
    }

    [Test]
    public async Task CMS_Query_Flags_NoneAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 1000, 5), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, "hello",
                flags: CommandFlags.None), Is.Zero);
        }

        var counts = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["a", "b"],
            flags: CommandFlags.None);

        Assert.That(counts, Is.EqualTo(new long[] { 0, 0 }));
    }

    [Test]
    public async Task CMS_Query_CancellationToken_DefaultAsync()
    {
        var key = $"Sketch:{{CMS_QueryAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 1000, 5), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_QueryAsync(
                key, "hello",
                cancellationToken: CancellationToken.None), Is.Zero);
        }

        var counts = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["a", "b"],
            cancellationToken: CancellationToken.None);

        Assert.That(counts, Is.EqualTo(new long[] { 0, 0 }));
    }

    #endregion

    #region CMS_Info

    private static async Task CMS_InfoRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task CMS_Info_InitByDimAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 7), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(7));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_Info_MinimumDimensionsAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 1, 1), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(1));
            Assert.That(info.Depth, Is.EqualTo(1));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [TestCase(10L, 3L)]
    [TestCase(100L, 5L)]
    [TestCase(123L, 7L)]
    [TestCase(1000L, 10L)]
    [TestCase(4096L, 16L)]
    public async Task CMS_Info_DifferentDimensionsAsync(long width, long depth)
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, width, depth), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(width));
            Assert.That(info.Depth, Is.EqualTo(depth));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_Info_InitByProbAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByProbAsync(
            key, 0.001, 0.01), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_Info_AfterSingleIncrementAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", 15);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Count, Is.EqualTo(15));
        }

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.EqualTo(15));
        }
    }

    [Test]
    public async Task CMS_Info_AfterMultipleIncrementsAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 2000, 5), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 10)).IsSuccess, Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 20)).IsSuccess, Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 30)).IsSuccess, Is.True);
        }

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.EqualTo(60));
        }
    }

    [Test]
    public async Task CMS_Info_AfterBatchIncrementAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ((SharpValue)"apple", 10L),
            ((SharpValue)"banana", 20L),
            ((SharpValue)"orange", 30L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        foreach (var result in results)
            Assert.That(result.IsSuccess, Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.EqualTo(60));
        }
    }

    [Test]
    public async Task CMS_Info_CountNotDistinctItemsAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var results = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key,
            [
                ((SharpValue)"same", 5L),
            ((SharpValue)"same", 10L),
            ((SharpValue)"same", 20L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        foreach (var result in results)
            Assert.That(result.IsSuccess, Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        Assert.That(info.Count, Is.EqualTo(35));
    }

    [Test]
    public async Task CMS_Info_QueryDoesNotChangeInfoAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 2000, 5), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "hello", 25)).IsSuccess, Is.True);
        }

        var before = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        _ = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, "hello");

        _ = await this._redis.Modules.Sketch.CMS_QueryAsync(
            key, ["hello", "other", "hello"]);

        var after = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.EqualTo(before.Count));
        }
    }

    [Test]
    public async Task CMS_Info_RepeatedCallsAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 100, 3), Is.True);

        for (int i = 0; i < 5; i++)
        {
            var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(info.Width, Is.EqualTo(100));
                Assert.That(info.Depth, Is.EqualTo(3));
                Assert.That(info.Count, Is.Zero);
            }
        }
    }

    [Test]
    public async Task CMS_Info_CountGreaterThanIntMaxValueAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        const long increment = 3_000_000_000L;

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", increment);

        Assert.That(result.IsSuccess, Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        Assert.That(info.Count, Is.EqualTo(increment));
    }

    [Test]
    public async Task CMS_Info_CountAtUintMaxValueAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 2000, 5), Is.True);

        var result = await this._redis.Modules.Sketch.CMS_IncrByAsync(
            key, "hello", (long)uint.MaxValue);

        Assert.That(result.IsSuccess, Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        Assert.That(info.Count, Is.EqualTo((long)uint.MaxValue));
    }

    [Test]
    public async Task CMS_Info_CollisionCountAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key, 1, 1), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "apple", 10)).IsSuccess, Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key, "banana", 20)).IsSuccess, Is.True);
        }

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(1));
            Assert.That(info.Depth, Is.EqualTo(1));
            Assert.That(info.Count, Is.EqualTo(30));
        }
    }

    [Test]
    public async Task CMS_Info_DifferentKeysAsync()
    {
        var key1 = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                    key1, 100, 3), Is.True);

            Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
                key2, 2000, 7), Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key1, "hello", 10)).IsSuccess, Is.True);

            Assert.That((await this._redis.Modules.Sketch.CMS_IncrByAsync(
                key2, "hello", 50)).IsSuccess, Is.True);
        }

        var info1 = await this._redis.Modules.Sketch.CMS_InfoAsync(key1);
        var info2 = await this._redis.Modules.Sketch.CMS_InfoAsync(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info1.Width, Is.EqualTo(100));
            Assert.That(info1.Depth, Is.EqualTo(3));
            Assert.That(info1.Count, Is.EqualTo(10));

            Assert.That(info2.Width, Is.EqualTo(2000));
            Assert.That(info2.Depth, Is.EqualTo(7));
            Assert.That(info2.Count, Is.EqualTo(50));
        }
    }

    [Test]
    public async Task CMS_Info_KeyNotExistsAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        await CMS_InfoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InfoAsync(key));
    }

    [Test]
    public async Task CMS_Info_WrongKeyTypeAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CMS_InfoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InfoAsync(key));
    }

    [Test]
    public async Task CMS_Info_BloomKeyWrongTypeAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.True);

        await CMS_InfoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InfoAsync(key));
    }

    [Test]
    public async Task CMS_Info_CuckooKeyWrongTypeAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        await CMS_InfoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Sketch.CMS_InfoAsync(key));
    }

    [Test]
    public async Task CMS_Info_Flags_NoneAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 100, 5), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(
            key, flags: CommandFlags.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.Zero);
        }
    }

    [Test]
    public async Task CMS_Info_CancellationToken_DefaultAsync()
    {
        var key = $"Sketch:{{CMS_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Sketch.CMS_InitByDimAsync(
            key, 100, 5), Is.True);

        var info = await this._redis.Modules.Sketch.CMS_InfoAsync(
            key, cancellationToken: CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.Zero);
        }
    }

    #endregion

}
