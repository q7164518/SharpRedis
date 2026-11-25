using SharpRedis;
using SharpRedis.Exceptions;
using SharpRedis.Modules;

namespace NET10.CountMinSketch;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class CountMinSketchSync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region CMS_IncrBy

    private static void CMS_IncrByRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 验证单项成功结果。
    /// </summary>
    private static void CMS_IncrByAssertSuccess(
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

    /// <summary>
    /// 验证单项溢出结果。
    /// </summary>
    private static void CMS_IncrByAssertOverflow(
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
    public void CMS_IncrBy_Single()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", 10);

        CMS_IncrByAssertSuccess(result, 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                    key, "hello"), Is.EqualTo(10));

            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                key).Count, Is.EqualTo(10));
        }
    }

    [Test]
    public void CMS_IncrBy_Single_MultipleTimes()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        CMS_IncrByAssertSuccess(
            this._redis.Modules.Sketch.CMS_IncrBy(key, "hello", 5), 5);

        CMS_IncrByAssertSuccess(
            this._redis.Modules.Sketch.CMS_IncrBy(key, "hello", 10), 15);

        CMS_IncrByAssertSuccess(
            this._redis.Modules.Sketch.CMS_IncrBy(key, "hello", 20), 35);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                    key, "hello"), Is.EqualTo(35));

            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                key).Count, Is.EqualTo(35));
        }
    }

    [Test]
    public void CMS_IncrBy_SingleTuple()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, ("hello", 12L));

        CMS_IncrByAssertSuccess(result, 12);

        Assert.That(this._redis.Modules.Sketch.CMS_Query(
            key, "hello"), Is.EqualTo(12));
    }

    [Test]
    public void CMS_IncrBy_SingleKeyValuePair()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, new KeyValuePair<SharpValue, long>("hello", 15));

        CMS_IncrByAssertSuccess(result, 15);

        Assert.That(this._redis.Modules.Sketch.CMS_Query(
            key, "hello"), Is.EqualTo(15));
    }

    [Test]
    public void CMS_IncrBy_TupleCollection()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            new List<(SharpValue item, long increment)>
            {
            ("apple", 10),
            ("banana", 20),
            ("orange", 30)
            });

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccess(results[0], 10);
        CMS_IncrByAssertSuccess(results[1], 20);
        CMS_IncrByAssertSuccess(results[2], 30);

        Assert.That(this._redis.Modules.Sketch.CMS_Info(
            key).Count, Is.EqualTo(60));
    }

    [Test]
    public void CMS_IncrBy_KeyValuePairCollection()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            new List<KeyValuePair<SharpValue, long>>
            {
            new("apple", 5),
            new("banana", 15),
            new("orange", 25)
            });

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccess(results[0], 5);
        CMS_IncrByAssertSuccess(results[1], 15);
        CMS_IncrByAssertSuccess(results[2], 25);

        Assert.That(this._redis.Modules.Sketch.CMS_Info(
            key).Count, Is.EqualTo(45));
    }

    [Test]
    public void CMS_IncrBy_TupleSpan()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                ("apple", 10L),
            ("banana", 20L),
            ("orange", 30L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccess(results[0], 10);
        CMS_IncrByAssertSuccess(results[1], 20);
        CMS_IncrByAssertSuccess(results[2], 30);
    }

    [Test]
    public void CMS_IncrBy_KeyValuePairSpan()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                new KeyValuePair<SharpValue, long>("apple", 10),
            new KeyValuePair<SharpValue, long>("banana", 20),
            new KeyValuePair<SharpValue, long>("orange", 30)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccess(results[0], 10);
        CMS_IncrByAssertSuccess(results[1], 20);
        CMS_IncrByAssertSuccess(results[2], 30);
    }

    [Test]
    public void CMS_IncrBy_DuplicateItems()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                ("hello", 5L),
            ("hello", 10L),
            ("hello", 20L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccess(results[0], 5);
        CMS_IncrByAssertSuccess(results[1], 15);
        CMS_IncrByAssertSuccess(results[2], 35);

        Assert.That(this._redis.Modules.Sketch.CMS_Query(
            key, "hello"), Is.EqualTo(35));
    }

    [Test]
    public void CMS_IncrBy_MixedOverloads()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        CMS_IncrByAssertSuccess(
            this._redis.Modules.Sketch.CMS_IncrBy(key, "a", 10), 10);

        CMS_IncrByAssertSuccess(
            this._redis.Modules.Sketch.CMS_IncrBy(
                key, ("a", 5L)), 15);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                ("a", 10L),
            ("b", 20L)
            ]);

        Assert.That(results, Has.Length.EqualTo(2));

        CMS_IncrByAssertSuccess(results[0], 25);
        CMS_IncrByAssertSuccess(results[1], 20);

        Assert.That(this._redis.Modules.Sketch.CMS_Info(
            key).Count, Is.EqualTo(45));
    }

    /// <summary>
    /// 增量为 0，计数保持不变。
    /// </summary>
    [Test]
    public void CMS_IncrBy_ZeroIncrement()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        CMS_IncrByAssertSuccess(
            this._redis.Modules.Sketch.CMS_IncrBy(key, "hello", 10), 10);

        CMS_IncrByAssertSuccess(
            this._redis.Modules.Sketch.CMS_IncrBy(key, "hello", 0), 10);

        Assert.That(this._redis.Modules.Sketch.CMS_Info(
            key).Count, Is.EqualTo(10));
    }

    /// <summary>
    /// 测试较大的合法增量。
    /// </summary>
    [Test]
    public void CMS_IncrBy_LargeIncrement()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        const long increment = 1_000_000_000L;

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", increment);

        CMS_IncrByAssertSuccess(result, increment);

        Assert.That(this._redis.Modules.Sketch.CMS_Query(
            key, "hello"), Is.EqualTo(increment));
    }

    /// <summary>
    /// 单元素溢出，错误应该被封装成 IncrByResult。
    /// </summary>
    [Test]
    public void CMS_IncrBy_Single_Overflow()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", 4_294_967_296L);

        CMS_IncrByAssertOverflow(result);

        Assert.That(this._redis.Modules.Sketch.CMS_Query(
            key, "hello"), Is.Zero);
    }

    /// <summary>
    /// 已有计数再次增加导致溢出。
    /// </summary>
    [Test]
    public void CMS_IncrBy_Single_OverflowAfterIncrement()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        CMS_IncrByAssertSuccess(
            this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 4_000_000_000L),
            4_000_000_000L);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", 500_000_000L);

        CMS_IncrByAssertOverflow(result);

        Assert.That(this._redis.Modules.Sketch.CMS_Query(
            key, "hello"), Is.EqualTo(4000000000));
    }

    /// <summary>
    /// 批量返回：成功、溢出、成功。
    /// 重点测试数组中嵌套 Redis 错误的解析。
    /// </summary>
    [Test]
    public void CMS_IncrBy_Multiple_PartialOverflow()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                ("k", 12L),
                ("kk", 4_294_967_296L),
                ("k3", 1024L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccess(results[0], 12);
        CMS_IncrByAssertOverflow(results[1]);
        CMS_IncrByAssertSuccess(results[2], 1024);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "k"), Is.EqualTo(12));

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "kk"), Is.Zero);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "k3"), Is.EqualTo(1024));
        }
    }

    /// <summary>
    /// 同批次重复元素发生溢出，后续其他元素仍成功。
    /// </summary>
    [Test]
    public void CMS_IncrBy_Multiple_DuplicateOverflow()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                ("hello", 4_000_000_000L),
                ("hello", 500_000_000L),
                ("other", 25L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccess(results[0], 4_000_000_000L);
        CMS_IncrByAssertOverflow(results[1]);
        CMS_IncrByAssertSuccess(results[2], 25);

        Assert.That(this._redis.Modules.Sketch.CMS_Query(
            key, "hello"), Is.EqualTo(4000000000));
    }

    /// <summary>
    /// 连续多次溢出不会影响后续正常元素。
    /// </summary>
    [Test]
    public void CMS_IncrBy_Multiple_MultipleErrors()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                ("a", 4_294_967_296L),
            ("b", 5L),
            ("c", 4_294_967_296L),
            ("d", 10L)
            ]);

        Assert.That(results, Has.Length.EqualTo(4));

        CMS_IncrByAssertOverflow(results[0]);
        CMS_IncrByAssertSuccess(results[1], 5);
        CMS_IncrByAssertOverflow(results[2]);
        CMS_IncrByAssertSuccess(results[3], 10);
    }

    /// <summary>
    /// Key 不存在时抛出异常，不会自动创建 Sketch。
    /// </summary>
    [Test]
    public void CMS_IncrBy_KeyNotExists()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        CMS_IncrByRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 10));
    }

    [Test]
    public void CMS_IncrBy_Multiple_KeyNotExists()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        CMS_IncrByRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_IncrBy(
                key,
                [
                    ("a", 1L),
                ("b", 2L)
                ]));
    }

    [Test]
    public void CMS_IncrBy_WrongKeyType()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CMS_IncrByRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 1));
    }

    [Test]
    public void CMS_IncrBy_BloomKeyWrongType()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "hello"), Is.True);

        CMS_IncrByRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 1));
    }

    [Test]
    public void CMS_IncrBy_SpecialItems()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                ("", 5L),
                ("你好🌍", 10L),
                ("a\nb\tc", 15L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccess(results[0], 5);
        CMS_IncrByAssertSuccess(results[1], 10);
        CMS_IncrByAssertSuccess(results[2], 15);
    }

    [Test]
    public void CMS_IncrBy_CaseSensitive()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                ("Hello", 10L),
                ("hello", 20L),
                ("HELLO", 30L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        CMS_IncrByAssertSuccess(results[0], 10);
        CMS_IncrByAssertSuccess(results[1], 20);
        CMS_IncrByAssertSuccess(results[2], 30);
    }

    [Test]
    public void CMS_IncrBy_Flags_None()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", 10,
            flags: CommandFlags.None);

        CMS_IncrByAssertSuccess(result, 10);
    }

    [Test]
    public void CMS_IncrBy_CancellationToken_Default()
    {
        var key = $"Sketch:{{CMS_IncrBy}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", 10,
            cancellationToken: CancellationToken.None);

        CMS_IncrByAssertSuccess(result, 10);
    }
    #endregion

    #region CMS_InitByDim

    private static void CMS_InitByDimRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 标准宽度、深度创建 Sketch。
    /// </summary>
    [Test]
    public void CMS_InitByDim_Default()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 验证最小有效维度：width=1, depth=1。
    /// </summary>
    [Test]
    public void CMS_InitByDim_MinimumDimensions()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 1, 1), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(1));
            Assert.That(info.Depth, Is.EqualTo(1));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 验证多组合法宽度和深度。
    /// </summary>
    [TestCase(1L, 1L)]
    [TestCase(1L, 10L)]
    [TestCase(10L, 1L)]
    [TestCase(10L, 10L)]
    [TestCase(100L, 5L)]
    [TestCase(1000L, 7L)]
    [TestCase(2000L, 5L)]
    [TestCase(4096L, 16L)]
    public void CMS_InitByDim_ValidDimensions(long width, long depth)
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, width, depth), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(width));
            Assert.That(info.Depth, Is.EqualTo(depth));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 宽度不要求为 2 的幂。
    /// </summary>
    [TestCase(3L)]
    [TestCase(7L)]
    [TestCase(13L)]
    [TestCase(101L)]
    [TestCase(1001L)]
    public void CMS_InitByDim_Width_NotPowerOfTwo(long width)
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, width, 5), Is.True);

            Assert.That(
                this._redis.Modules.Sketch.CMS_Info(key).Width,
                Is.EqualTo(width));
        }
    }

    /// <summary>
    /// width=0 应返回 Redis 错误。
    /// </summary>
    [Test]
    public void CMS_InitByDim_Width_Zero()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, 0, 5));
    }

    /// <summary>
    /// width 为负数应返回 Redis 错误。
    /// </summary>
    [TestCase(-1L)]
    [TestCase(-10L)]
    [TestCase(-100L)]
    public void CMS_InitByDim_Width_Negative(long width)
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, width, 5));
    }

    /// <summary>
    /// depth=0 应返回 Redis 错误。
    /// </summary>
    [Test]
    public void CMS_InitByDim_Depth_Zero()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, 2000, 0));
    }

    /// <summary>
    /// depth 为负数应返回 Redis 错误。
    /// </summary>
    [TestCase(-1L)]
    [TestCase(-10L)]
    [TestCase(-100L)]
    public void CMS_InitByDim_Depth_Negative(long depth)
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, 2000, depth));
    }

    /// <summary>
    /// width 和 depth 同时无效。
    /// </summary>
    [TestCase(0L, 0L)]
    [TestCase(-1L, -1L)]
    [TestCase(0L, -1L)]
    [TestCase(-1L, 0L)]
    public void CMS_InitByDim_BothDimensions_Invalid(
        long width, long depth)
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, width, depth));
    }

    /// <summary>
    /// 非法参数不应创建 Sketch。
    /// </summary>
    [Test]
    public void CMS_InitByDim_InvalidDimensions_NoCreate()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, 0, 5));

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Info(key));

        // 参数修正后，同一个 Key 应可以成功创建。
        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 100, 5), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
        }
    }

    /// <summary>
    /// 不允许重复创建相同的 Sketch。
    /// </summary>
    [Test]
    public void CMS_InitByDim_KeyAlreadyExists()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, 2000, 5));

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 使用不同尺寸重复初始化，原配置不应改变。
    /// </summary>
    [Test]
    public void CMS_InitByDim_KeyAlreadyExists_DifferentDimensions()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 100, 3), Is.True);

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, 2000, 10));

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(3));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 已有数据的 Sketch 不允许重新初始化。
    /// </summary>
    [Test]
    public void CMS_InitByDim_KeyAlreadyHasData()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 100, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Count, Is.EqualTo(10));
        }

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, 2000, 10));

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.EqualTo(10));
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "hello"), Is.EqualTo(10));
        }
    }

    /// <summary>
    /// 已通过 CMS.INITBYPROB 创建，不能再通过 INITBYDIM 创建。
    /// </summary>
    [Test]
    public void CMS_InitByDim_KeyAlreadyCreatedByProb()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, 0.001, 0.01), Is.True);

        var before = this._redis.Modules.Sketch.CMS_Info(key);

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, 100, 5));

        var after = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.Zero);
        }
    }

    /// <summary>
    /// String 类型 Key 已存在时，返回错误。
    /// </summary>
    [Test]
    public void CMS_InitByDim_WrongKeyType()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, 100, 5));
    }

    /// <summary>
    /// Bloom Filter Key 已存在时，返回错误。
    /// </summary>
    [Test]
    public void CMS_InitByDim_BloomKeyWrongType()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "hello"), Is.True);

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, 100, 5));
    }

    /// <summary>
    /// Cuckoo Filter Key 已存在时，返回错误。
    /// </summary>
    [Test]
    public void CMS_InitByDim_CuckooKeyWrongType()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        CMS_InitByDimRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByDim(
                key, 100, 5));
    }

    /// <summary>
    /// 创建不同 Key，验证 Width、Depth 相互独立。
    /// </summary>
    [Test]
    public void CMS_InitByDim_DifferentKeys()
    {
        var key1 = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key1, 100, 3), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                key2, 2000, 7), Is.True);
        }

        var info1 = this._redis.Modules.Sketch.CMS_Info(key1);
        var info2 = this._redis.Modules.Sketch.CMS_Info(key2);

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

    /// <summary>
    /// 初始化后的计数器全部为零，查询不会修改 Count。
    /// </summary>
    [Test]
    public void CMS_InitByDim_InitialCount()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var counts = this._redis.Modules.Sketch.CMS_Query(
            key, ["a", "b", "c"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(counts, Is.EqualTo(new long[] { 0, 0, 0 }));

            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                key).Count, Is.Zero);
        }
    }

    /// <summary>
    /// 创建后支持 CMS.INCRBY，验证实际尺寸有效。
    /// </summary>
    [Test]
    public void CMS_InitByDim_AfterIncrement()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", 15);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Count, Is.EqualTo(15));
        }

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.EqualTo(15));
        }
    }

    /// <summary>
    /// 显式传入 CommandFlags.None。
    /// </summary>
    [Test]
    public void CMS_InitByDim_Flags_None()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 100, 5,
            flags: CommandFlags.None), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
        }
    }

    /// <summary>
    /// 显式传入 CancellationToken.None。
    /// </summary>
    [Test]
    public void CMS_InitByDim_CancellationToken_Default()
    {
        var key = $"Sketch:{{CMS_InitByDim}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 100, 5,
            cancellationToken: CancellationToken.None), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
        }
    }

    #endregion

    #region CMS_InitByProb

    private static void CMS_InitByProbRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 标准参数初始化，检查返回值和 Sketch 元数据。
    /// </summary>
    [Test]
    public void CMS_InitByProb_Default()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, 0.001, 0.01), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 多组合法 error / probability 组合。
    /// </summary>
    [TestCase(0.5, 0.5)]
    [TestCase(0.1, 0.1)]
    [TestCase(0.01, 0.01)]
    [TestCase(0.001, 0.001)]
    [TestCase(0.0001, 0.01)]
    [TestCase(0.01, 0.0001)]
    [TestCase(0.00001, 0.001)]
    [TestCase(0.9, 0.9)]
    public void CMS_InitByProb_ValidParameters(
        double error, double probability)
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, error, probability), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 更小的 error 应产生更大的 Width。
    /// </summary>
    [Test]
    public void CMS_InitByProb_Error_AffectsWidth()
    {
        var key1 = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
                    key1, 0.1, 0.01), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
                key2, 0.001, 0.01), Is.True);
        }

        var info1 = this._redis.Modules.Sketch.CMS_Info(key1);
        var info2 = this._redis.Modules.Sketch.CMS_Info(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info2.Width, Is.GreaterThan(info1.Width));
            Assert.That(info2.Depth, Is.EqualTo(info1.Depth));
            Assert.That(info1.Count, Is.Zero);
            Assert.That(info2.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 更小的 probability 应产生更大的 Depth。
    /// </summary>
    [Test]
    public void CMS_InitByProb_Probability_AffectsDepth()
    {
        var key1 = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
                    key1, 0.01, 0.1), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
                key2, 0.01, 0.0001), Is.True);
        }

        var info1 = this._redis.Modules.Sketch.CMS_Info(key1);
        var info2 = this._redis.Modules.Sketch.CMS_Info(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info2.Width, Is.EqualTo(info1.Width));
            Assert.That(info2.Depth, Is.GreaterThan(info1.Depth));
            Assert.That(info1.Count, Is.Zero);
            Assert.That(info2.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 相同参数应产生相同的宽度和深度。
    /// </summary>
    [Test]
    public void CMS_InitByProb_SameParameters_SameDimensions()
    {
        var key1 = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
                    key1, 0.001, 0.01), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
                key2, 0.001, 0.01), Is.True);
        }

        var info1 = this._redis.Modules.Sketch.CMS_Info(key1);
        var info2 = this._redis.Modules.Sketch.CMS_Info(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info2.Width, Is.EqualTo(info1.Width));
            Assert.That(info2.Depth, Is.EqualTo(info1.Depth));
            Assert.That(info1.Count, Is.Zero);
            Assert.That(info2.Count, Is.Zero);
        }
    }

    /// <summary>
    /// error = 0 非法。
    /// </summary>
    [Test]
    public void CMS_InitByProb_Error_Zero()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0, 0.01));
    }

    /// <summary>
    /// error 为负数非法。
    /// </summary>
    [TestCase(-0.001)]
    [TestCase(-0.1)]
    [TestCase(-1.0)]
    [TestCase(-10.0)]
    public void CMS_InitByProb_Error_Negative(double error)
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, error, 0.01));
    }

    /// <summary>
    /// error >= 1 非法。
    /// </summary>
    [TestCase(1.0)]
    [TestCase(1.5)]
    [TestCase(2.0)]
    [TestCase(10.0)]
    public void CMS_InitByProb_Error_GreaterThanOrEqualOne(double error)
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, error, 0.01));
    }

    /// <summary>
    /// probability = 0 非法。
    /// </summary>
    [Test]
    public void CMS_InitByProb_Probability_Zero()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0.01, 0));
    }

    /// <summary>
    /// probability 为负数非法。
    /// </summary>
    [TestCase(-0.001)]
    [TestCase(-0.1)]
    [TestCase(-1.0)]
    [TestCase(-10.0)]
    public void CMS_InitByProb_Probability_Negative(double probability)
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0.01, probability));
    }

    /// <summary>
    /// probability >= 1 非法。
    /// </summary>
    [TestCase(1.0)]
    [TestCase(1.5)]
    [TestCase(2.0)]
    [TestCase(10.0)]
    public void CMS_InitByProb_Probability_GreaterThanOrEqualOne(
        double probability)
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0.01, probability));
    }

    /// <summary>
    /// error 和 probability 同时非法。
    /// </summary>
    [TestCase(0.0, 0.0)]
    [TestCase(-0.1, -0.1)]
    [TestCase(1.0, 1.0)]
    [TestCase(-1.0, 2.0)]
    [TestCase(2.0, -1.0)]
    public void CMS_InitByProb_BothParameters_Invalid(
        double error, double probability)
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, error, probability));
    }

    /// <summary>
    /// 非法参数失败后，不应该占用 Key。
    /// 修正参数后可成功初始化。
    /// </summary>
    [Test]
    public void CMS_InitByProb_InvalidParameters_NoCreate()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0, 0.01));

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Info(key));

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, 0.01, 0.01), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 同一个 Key 不能重复使用 INITBYPROB 初始化。
    /// </summary>
    [Test]
    public void CMS_InitByProb_KeyAlreadyExists()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, 0.001, 0.01), Is.True);

        var before = this._redis.Modules.Sketch.CMS_Info(key);

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0.001, 0.01));

        var after = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 重复创建时，即使参数不同也不能修改已存在的 Sketch。
    /// </summary>
    [Test]
    public void CMS_InitByProb_KeyAlreadyExists_DifferentParameters()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, 0.1, 0.1), Is.True);

        var before = this._redis.Modules.Sketch.CMS_Info(key);

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0.0001, 0.0001));

        var after = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 已有计数数据的 Sketch 不允许重新初始化。
    /// </summary>
    [Test]
    public void CMS_InitByProb_KeyAlreadyHasData()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, 0.001, 0.01), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", 15);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Count, Is.EqualTo(15));
        }

        var before = this._redis.Modules.Sketch.CMS_Info(key);

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0.1, 0.1));

        var after = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.EqualTo(15));
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "hello"), Is.EqualTo(15));
        }
    }

    /// <summary>
    /// INITBYDIM 创建的 Sketch，不能再使用 INITBYPROB 初始化。
    /// </summary>
    [Test]
    public void CMS_InitByProb_KeyAlreadyCreatedByDim()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 100, 5), Is.True);

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0.001, 0.01));

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 已有 String Key，不能创建 Sketch。
    /// </summary>
    [Test]
    public void CMS_InitByProb_WrongKeyType()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0.001, 0.01));
    }

    /// <summary>
    /// 已有 Bloom Filter Key，不能创建 Sketch。
    /// </summary>
    [Test]
    public void CMS_InitByProb_BloomKeyWrongType()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "hello"), Is.True);

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0.001, 0.01));
    }

    /// <summary>
    /// 已有 Cuckoo Filter Key，不能创建 Sketch。
    /// </summary>
    [Test]
    public void CMS_InitByProb_CuckooKeyWrongType()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        CMS_InitByProbRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_InitByProb(
                key, 0.001, 0.01));
    }

    /// <summary>
    /// 不同 Key 使用独立的概率配置。
    /// </summary>
    [Test]
    public void CMS_InitByProb_DifferentKeys()
    {
        var key1 = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
                    key1, 0.1, 0.1), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
                key2, 0.001, 0.001), Is.True);
        }

        var info1 = this._redis.Modules.Sketch.CMS_Info(key1);
        var info2 = this._redis.Modules.Sketch.CMS_Info(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info2.Width, Is.GreaterThan(info1.Width));
            Assert.That(info2.Depth, Is.GreaterThan(info1.Depth));
            Assert.That(info1.Count, Is.Zero);
            Assert.That(info2.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 初始化后查询未插入元素，结果为零。
    /// </summary>
    [Test]
    public void CMS_InitByProb_InitialCount()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, 0.001, 0.01), Is.True);

        var counts = this._redis.Modules.Sketch.CMS_Query(
            key, ["a", "b", "c"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(counts, Is.EqualTo(new long[] { 0, 0, 0 }));

            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                key).Count, Is.Zero);
        }
    }

    /// <summary>
    /// 初始化后进行计数，验证尺寸保持不变。
    /// </summary>
    [Test]
    public void CMS_InitByProb_AfterIncrement()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, 0.001, 0.01), Is.True);

        var before = this._redis.Modules.Sketch.CMS_Info(key);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", 20);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Count, Is.EqualTo(20));
        }

        var after = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.EqualTo(20));
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "hello"), Is.EqualTo(20));
        }
    }

    /// <summary>
    /// 显式传入 CommandFlags.None。
    /// </summary>
    [Test]
    public void CMS_InitByProb_Flags_None()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, 0.001, 0.01,
            flags: CommandFlags.None), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
        }
    }

    /// <summary>
    /// 显式传入 CancellationToken.None。
    /// </summary>
    [Test]
    public void CMS_InitByProb_CancellationToken_Default()
    {
        var key = $"Sketch:{{CMS_InitByProb}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, 0.001, 0.01,
            cancellationToken: CancellationToken.None), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
        }
    }

    #endregion

    #region CMS_Merge

    private static void CMS_MergeRedisError<TError>(Action action)
        where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 单个源 Sketch，默认权重 1。
    /// </summary>
    [Test]
    public void CMS_Merge_Single_DefaultWeight()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source, "apple", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination, source), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(10));
            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                destination).Count, Is.EqualTo(10));
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                source, "apple"), Is.EqualTo(10));
        }
    }

    /// <summary>
    /// 单个源 Sketch，指定权重。
    /// </summary>
    [Test]
    public void CMS_Merge_Single_CustomWeight()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source, "apple", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination, source, weight: 3), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(30));
            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                destination).Count, Is.EqualTo(30));
        }
    }

    /// <summary>
    /// 多个源 Sketch，SharpValueSpan 重载，不指定权重。
    /// </summary>
    [Test]
    public void CMS_Merge_Multiple_DefaultWeights()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_Merge}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_Merge}}:{id}:source2";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source1, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source2, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source1, "apple", 10).IsSuccess, Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source2, "apple", 20).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination, sources: [source1, source2]), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(30));
            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                destination).Count, Is.EqualTo(30));
        }
    }

    /// <summary>
    /// 多个源 Sketch，SharpValueSpan + weights 数组。
    /// </summary>
    [Test]
    public void CMS_Merge_Multiple_CustomWeights()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_Merge}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_Merge}}:{id}:source2";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source1, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source2, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source1, "apple", 10).IsSuccess, Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source2, "apple", 20).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination,
                sources: [source1, source2],
                weights: [2, 3]), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(80));
            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                destination).Count, Is.EqualTo(80));
        }
    }

    /// <summary>
    /// ICollection<ValueTuple<SharpValue,long>> 重载。
    /// </summary>
    [Test]
    public void CMS_Merge_TupleCollection()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_Merge}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_Merge}}:{id}:source2";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source1, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source2, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source1, "apple", 5).IsSuccess, Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source2, "apple", 10).IsSuccess, Is.True);
        }

        var sources = new List<(SharpValue source, long weight)>
        {
            (source1, 2),
            (source2, 4)
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                    destination, sources), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(50));
        }
    }

    /// <summary>
    /// ICollection<KeyValuePair<SharpValue,long>> 重载。
    /// </summary>
    [Test]
    public void CMS_Merge_KeyValuePairCollection()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_Merge}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_Merge}}:{id}:source2";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source1, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source2, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source1, "apple", 5).IsSuccess, Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source2, "apple", 10).IsSuccess, Is.True);
        }

        var sources = new List<KeyValuePair<SharpValue, long>>
        {
            new(source1, 2),
            new(source2, 4)
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                    destination, sources), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(50));
        }
    }

    /// <summary>
    /// ReadOnlySpan<ValueTuple<SharpValue,long>> 重载。
    /// </summary>
    [Test]
    public void CMS_Merge_TupleSpan()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_Merge}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_Merge}}:{id}:source2";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source1, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source2, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source1, "apple", 5).IsSuccess, Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source2, "apple", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination,
                sourceWeights: (ReadOnlySpan<(SharpValue source, long weight)>)
                [
                    (source1, 2L),
            (source2, 4L)
                ]), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(50));
        }
    }

    /// <summary>
    /// ReadOnlySpan<KeyValuePair<SharpValue,long>> 重载。
    /// </summary>
    [Test]
    public void CMS_Merge_KeyValuePairSpan()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_Merge}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_Merge}}:{id}:source2";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source1, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source2, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source1, "apple", 5).IsSuccess, Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source2, "apple", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination,
                sourceWeights: (ReadOnlySpan<KeyValuePair<SharpValue, long>>)
                [
                    new(source1, 2),
            new(source2, 4)
                ]), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(50));
        }
    }

    /// <summary>
    /// 多源合并不同元素，并检查源 Sketch 不变。
    /// </summary>
    [Test]
    public void CMS_Merge_DifferentItems()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_Merge}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_Merge}}:{id}:source2";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source1, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source2, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source1, "apple", 10).IsSuccess, Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source2, "banana", 20).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination, sources: [source1, source2]), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(10));
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "banana"), Is.EqualTo(20));
            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                destination).Count, Is.EqualTo(30));
            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                source1).Count, Is.EqualTo(10));
            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                source2).Count, Is.EqualTo(20));
        }
    }

    /// <summary>
    /// 重复合并同一批源，结果不应该重复累加。
    /// </summary>
    [Test]
    public void CMS_Merge_RepeatedMerge()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source, "apple", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination, source), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                    destination, source), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(10));
        }
    }

    /// <summary>
    /// 验证 Merge 替换目标旧数据，而不是追加到旧值。
    /// </summary>
    [Test]
    public void CMS_Merge_OverwriteDestination()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                destination, "apple", 100).IsSuccess, Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source, "apple", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination, source), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(10));
            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                destination).Count, Is.EqualTo(10));
        }
    }

    /// <summary>
    /// 权重 0：对应源的贡献为 0。
    /// </summary>
    [Test]
    public void CMS_Merge_ZeroWeight()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_Merge}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_Merge}}:{id}:source2";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source1, 1000, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source2, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source1, "apple", 10).IsSuccess, Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source2, "apple", 20).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination,
                sources: [source1, source2],
                weights: [0, 2]), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "apple"), Is.EqualTo(40));
            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                destination).Count, Is.EqualTo(40));
        }
    }

    /// <summary>
    /// 源与目标维度一致时保留维度配置。
    /// </summary>
    [Test]
    public void CMS_Merge_DimensionsPreserved()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 123, 7), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source, 123, 7), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source, "apple", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination, source), Is.True);
        }

        var info = this._redis.Modules.Sketch.CMS_Info(destination);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(123));
            Assert.That(info.Depth, Is.EqualTo(7));
            Assert.That(info.Count, Is.EqualTo(10));
        }
    }

    /// <summary>
    /// 目标 Sketch 不存在。
    /// </summary>
    [Test]
    public void CMS_Merge_DestinationNotExists()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            source, 100, 5), Is.True);

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(destination, source));
    }

    /// <summary>
    /// 单个源 Sketch 不存在。
    /// </summary>
    [Test]
    public void CMS_Merge_SourceNotExists()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            destination, 100, 5), Is.True);

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(destination, source));
    }

    /// <summary>
    /// 多个源中有不存在的 Key。
    /// </summary>
    [Test]
    public void CMS_Merge_Multiple_SourceNotExists()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_Merge}}:{id}:source1";
        var missing = $"Sketch:{{CMS_Merge}}:{id}:missing";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 100, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source1, 100, 5), Is.True);
        }

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(
                destination, sources: [source1, missing]));

        Assert.That(this._redis.Modules.Sketch.CMS_Info(
            destination).Count, Is.Zero);
    }

    /// <summary>
    /// Width 不一致。
    /// </summary>
    [Test]
    public void CMS_Merge_WidthMismatch()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 100, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source, 200, 5), Is.True);
        }

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(
                destination, source));
    }

    /// <summary>
    /// Depth 不一致。
    /// </summary>
    [Test]
    public void CMS_Merge_DepthMismatch()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 100, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source, 100, 7), Is.True);
        }

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(
                destination, source));
    }

    /// <summary>
    /// 多个源中有一个 Width 不匹配。
    /// </summary>
    [Test]
    public void CMS_Merge_Multiple_DimensionMismatch()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_Merge}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_Merge}}:{id}:source2";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 100, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source1, 100, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source2, 200, 5), Is.True);
        }

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(
                destination, sources: [source1, source2]));

        Assert.That(this._redis.Modules.Sketch.CMS_Info(
            destination).Count, Is.Zero);
    }

    /// <summary>
    /// WEIGHTS 数量与源数量不一致。
    /// </summary>
    [Test]
    public void CMS_Merge_WeightsLengthMismatch()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source1 = $"Sketch:{{CMS_Merge}}:{id}:source1";
        var source2 = $"Sketch:{{CMS_Merge}}:{id}:source2";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 100, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source1, 100, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source2, 100, 5), Is.True);
        }

        CMS_MergeRedisError<NotSupportedException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(
                destination,
                sources: [source1, source2],
                weights: [2]));
    }

    /// <summary>
    /// 正权重导致计数器溢出，目标应保持不变。
    /// 与 CMS.INCRBY 饱和语义不同。
    /// </summary>
    [Test]
    public void CMS_Merge_Overflow_NoChanges()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 100, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source, 100, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                destination, "old", 7).IsSuccess, Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source, "apple", 3_000_000_000L).IsSuccess, Is.True);
        }

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(
                destination, source, weight: 2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                destination, "old"), Is.EqualTo(7));
            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                destination).Count, Is.EqualTo(7));
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                source, "apple"), Is.EqualTo(3_000_000_000L));
        }
    }

    /// <summary>
    /// 负权重造成 underflow，应返回 Redis 错误。
    /// </summary>
    [Test]
    public void CMS_Merge_NegativeWeight_Underflow()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 100, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source, 100, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                source, "apple", 10).IsSuccess, Is.True);
        }

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(
                destination, source, weight: -1));

        Assert.That(this._redis.Modules.Sketch.CMS_Info(
            destination).Count, Is.Zero);
    }

    /// <summary>
    /// String 类型的目标 Key。
    /// </summary>
    [Test]
    public void CMS_Merge_DestinationWrongKeyType()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        this._redis.String.Set(destination, "hello");

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            source, 100, 5), Is.True);

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(
                destination, source));
    }

    /// <summary>
    /// String 类型的源 Key。
    /// </summary>
    [Test]
    public void CMS_Merge_SourceWrongKeyType()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            destination, 100, 5), Is.True);

        this._redis.String.Set(source, "hello");

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(
                destination, source));
    }

    /// <summary>
    /// Bloom Filter 不能作为源 Sketch。
    /// </summary>
    [Test]
    public void CMS_Merge_BloomKeyWrongType()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 100, 5), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                source, "hello"), Is.True);
        }

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(
                destination, source));
    }

    /// <summary>
    /// Cuckoo Filter 不能作为源 Sketch。
    /// </summary>
    [Test]
    public void CMS_Merge_CuckooKeyWrongType()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 100, 5), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, "hello"), Is.True);
        }

        CMS_MergeRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Merge(
                destination, source));
    }

    /// <summary>
    /// 显式传入 CommandFlags.None。
    /// </summary>
    [Test]
    public void CMS_Merge_Flags_None()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 100, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source, 100, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination, source, flags: CommandFlags.None), Is.True);
        }
    }

    /// <summary>
    /// 显式传入 CancellationToken.None。
    /// </summary>
    [Test]
    public void CMS_Merge_CancellationToken_Default()
    {
        var id = Guid.NewGuid().ToString("N");
        var destination = $"Sketch:{{CMS_Merge}}:{id}:dest";
        var source = $"Sketch:{{CMS_Merge}}:{id}:source";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    destination, 100, 5), Is.True);
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                source, 100, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Merge(
                destination, source,
                cancellationToken: CancellationToken.None), Is.True);
        }
    }

    #endregion

    #region CMS_Query

    private static void CMS_QueryRedisError<TError>(Action action)
        where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 新建的 Sketch 查询单个元素，返回 0。
    /// </summary>
    [Test]
    public void CMS_Query_Single_EmptySketch()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "hello"), Is.Zero);
        }
    }

    /// <summary>
    /// 新建的 Sketch 批量查询，返回相同长度的零数组。
    /// </summary>
    [Test]
    public void CMS_Query_Multiple_EmptySketch()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 1000, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_Query(
            key, ["a", "b", "c", "d"]);

        Assert.That(result, Is.EqualTo(new long[] { 0, 0, 0, 0 }));
    }

    /// <summary>
    /// 单元素查询，验证基本计数。
    /// </summary>
    [Test]
    public void CMS_Query_Single_ExistingItem()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 1000, 5), Is.True);

        var added = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(added.IsSuccess, Is.True);
            Assert.That(added.Count, Is.EqualTo(10));

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "hello"), Is.EqualTo(10));
        }
    }

    /// <summary>
    /// 单元素增加多次，查询累计计数。
    /// </summary>
    [Test]
    public void CMS_Query_Single_MultipleIncrements()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 5).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "hello"), Is.EqualTo(5));

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 10).IsSuccess, Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                    key, "hello"), Is.EqualTo(15));

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 20).IsSuccess, Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                    key, "hello"), Is.EqualTo(35));

            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                key).Count, Is.EqualTo(35));
        }
    }

    /// <summary>
    /// 只查询一个元素的集合重载，返回长度为 1 的 long[]。
    /// </summary>
    [Test]
    public void CMS_Query_Multiple_SingleItem()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 12).IsSuccess, Is.True);
        }

        var result = this._redis.Modules.Sketch.CMS_Query(
            key, ["hello"]);

        Assert.That(result, Is.EqualTo(new long[] { 12 }));
    }

    /// <summary>
    /// 查询多个元素，单项结果与批量结果保持一致。
    /// 对不同元素不强制精确相等，以兼容概率碰撞。
    /// </summary>
    [Test]
    public void CMS_Query_Multiple_ExistingItems()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 1000, 5), Is.True);

        var added = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                ((SharpValue)"apple", 10L),
            ((SharpValue)"banana", 20L),
            ((SharpValue)"orange", 30L)
            ]);

        Assert.That(added, Has.Length.EqualTo(3));

        foreach (var result in added)
            Assert.That(result.IsSuccess, Is.True);

        var counts = this._redis.Modules.Sketch.CMS_Query(
            key, ["apple", "banana", "orange"]);

        Assert.That(counts, Has.Length.EqualTo(3));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(counts[0], Is.GreaterThanOrEqualTo(10));
            Assert.That(counts[1], Is.GreaterThanOrEqualTo(20));
            Assert.That(counts[2], Is.GreaterThanOrEqualTo(30));

            Assert.That(counts[0], Is.EqualTo(
                this._redis.Modules.Sketch.CMS_Query(key, "apple")));

            Assert.That(counts[1], Is.EqualTo(
                this._redis.Modules.Sketch.CMS_Query(key, "banana")));

            Assert.That(counts[2], Is.EqualTo(
                this._redis.Modules.Sketch.CMS_Query(key, "orange")));
        }
    }

    /// <summary>
    /// 验证结果数组与输入查询顺序对应。
    /// </summary>
    [Test]
    public void CMS_Query_Multiple_ResultOrder()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "a", 5).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "b", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "c", 15).IsSuccess, Is.True);
        }

        var result = this._redis.Modules.Sketch.CMS_Query(
            key, ["c", "a", "b", "a", "c"]);

        Assert.That(result, Has.Length.EqualTo(5));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0], Is.EqualTo(
                this._redis.Modules.Sketch.CMS_Query(key, "c")));

            Assert.That(result[1], Is.EqualTo(
                this._redis.Modules.Sketch.CMS_Query(key, "a")));

            Assert.That(result[2], Is.EqualTo(
                this._redis.Modules.Sketch.CMS_Query(key, "b")));

            Assert.That(result[3], Is.EqualTo(result[1]));
            Assert.That(result[4], Is.EqualTo(result[0]));
        }
    }

    /// <summary>
    /// 多次查询同一个元素，重复位置应返回相同结果。
    /// </summary>
    [Test]
    public void CMS_Query_Multiple_DuplicateQueries()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 25).IsSuccess, Is.True);
        }

        var result = this._redis.Modules.Sketch.CMS_Query(
            key, ["hello", "hello", "hello"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(new long[] { 25, 25, 25 }));

            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                key).Count, Is.EqualTo(25));
        }
    }

    /// <summary>
    /// 同一个命令中混合已计数和未计数元素。
    /// 未计数元素的估算结果可能大于零。
    /// </summary>
    [Test]
    public void CMS_Query_Multiple_MixedItems()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "known", 100).IsSuccess, Is.True);
        }

        var result = this._redis.Modules.Sketch.CMS_Query(
            key, ["unknown1", "known", "unknown2", "known"]);

        Assert.That(result, Has.Length.EqualTo(4));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0], Is.GreaterThanOrEqualTo(0));
            Assert.That(result[1], Is.EqualTo(100));
            Assert.That(result[2], Is.GreaterThanOrEqualTo(0));
            Assert.That(result[3], Is.EqualTo(100));
        }
    }

    /// <summary>
    /// 测试确定发生碰撞的 Sketch：只有一个计数器。
    /// 所有不同元素的查询结果都等于累计计数。
    /// </summary>
    [Test]
    public void CMS_Query_Collision_WidthOneDepthOne()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 1, 1), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "apple", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "banana", 20).IsSuccess, Is.True);
        }

        var result = this._redis.Modules.Sketch.CMS_Query(
            key, ["apple", "banana", "never-inserted"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(new long[] { 30, 30, 30 }));

            Assert.That(this._redis.Modules.Sketch.CMS_Info(
                key).Count, Is.EqualTo(30));
        }
    }

    /// <summary>
    /// 查询操作是只读的，不修改 Count、Width 或 Depth。
    /// </summary>
    [Test]
    public void CMS_Query_ReadOnly()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 50).IsSuccess, Is.True);
        }

        var before = this._redis.Modules.Sketch.CMS_Info(key);

        for (int i = 0; i < 10; i++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(this._redis.Modules.Sketch.CMS_Query(
                            key, "hello"), Is.EqualTo(50));

                Assert.That(this._redis.Modules.Sketch.CMS_Query(
                    key, ["hello", "hello"]),
                    Is.EqualTo(new long[] { 50, 50 }));
            }
        }

        var after = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.EqualTo(before.Count));
        }
    }

    /// <summary>
    /// 查询溢出后饱和的计数值。
    /// </summary>
    [Test]
    public void CMS_Query_Overflow_SaturatedValue()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 1000, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", (long)uint.MaxValue + 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.Error, Does.Contain("overflow").IgnoreCase);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "hello"), Is.Zero);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, ["hello", "hello"]),
                Is.EqualTo(
                [
                    0,
                    0
                ]));
        }
    }

    /// <summary>
    /// 计数为零的元素与计数为 uint.MaxValue 的元素。
    /// 使用独立 Sketch，避免碰撞影响边界断言。
    /// </summary>
    [Test]
    public void CMS_Query_CountBoundaries()
    {
        var key1 = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key1, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                key2, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key1, "hello"), Is.Zero);
        }

        var added = this._redis.Modules.Sketch.CMS_IncrBy(
            key2, "hello", (long)uint.MaxValue);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(added.IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key2, "hello"), Is.EqualTo((long)uint.MaxValue));
        }
    }

    /// <summary>
    /// Unicode、空字符串、换行和空格。
    /// </summary>
    [Test]
    public void CMS_Query_SpecialItems()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var added = this._redis.Modules.Sketch.CMS_IncrBy(
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

        var result = this._redis.Modules.Sketch.CMS_Query(
            key, ["", "你好🌍", "a b c", "a\nb\tc"]);

        Assert.That(result, Has.Length.EqualTo(4));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0], Is.GreaterThanOrEqualTo(5));
            Assert.That(result[1], Is.GreaterThanOrEqualTo(10));
            Assert.That(result[2], Is.GreaterThanOrEqualTo(15));
            Assert.That(result[3], Is.GreaterThanOrEqualTo(20));
        }
    }

    /// <summary>
    /// 大小写不同的元素分别查询。
    /// </summary>
    [Test]
    public void CMS_Query_CaseSensitive()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 2000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "Hello", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 20).IsSuccess, Is.True);
        }

        var result = this._redis.Modules.Sketch.CMS_Query(
            key, ["Hello", "hello", "HELLO"]);

        Assert.That(result, Has.Length.EqualTo(3));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0], Is.GreaterThanOrEqualTo(10));
            Assert.That(result[1], Is.GreaterThanOrEqualTo(20));
            Assert.That(result[2], Is.GreaterThanOrEqualTo(0));
        }
    }

    /// <summary>
    /// 查询两个不同 Key，验证计数独立。
    /// </summary>
    [Test]
    public void CMS_Query_DifferentKeys()
    {
        var key1 = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key1, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                key2, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key1, "hello", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key2, "hello", 30).IsSuccess, Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key1, "hello"), Is.EqualTo(10));

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key2, "hello"), Is.EqualTo(30));
        }
    }

    /// <summary>
    /// Key 不存在，单项查询抛出异常。
    /// </summary>
    [Test]
    public void CMS_Query_Single_KeyNotExists()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        CMS_QueryRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Query(key, "hello"));
    }

    /// <summary>
    /// Key 不存在，批量查询抛出异常。
    /// </summary>
    [Test]
    public void CMS_Query_Multiple_KeyNotExists()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        CMS_QueryRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Query(
                key, ["a", "b"]));
    }

    /// <summary>
    /// String 类型 Key。
    /// </summary>
    [Test]
    public void CMS_Query_WrongKeyType()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CMS_QueryRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Query(
                key, "hello"));

        CMS_QueryRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Query(
                key, ["a", "b"]));
    }

    /// <summary>
    /// Bloom Filter 类型 Key。
    /// </summary>
    [Test]
    public void CMS_Query_BloomKeyWrongType()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "hello"), Is.True);

        CMS_QueryRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Query(
                key, "hello"));
    }

    /// <summary>
    /// Cuckoo Filter 类型 Key。
    /// </summary>
    [Test]
    public void CMS_Query_CuckooKeyWrongType()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        CMS_QueryRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Query(
                key, ["hello"]));
    }

    /// <summary>
    /// 显式传入 CommandFlags.None。
    /// </summary>
    [Test]
    public void CMS_Query_Flags_None()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "hello",
                flags: CommandFlags.None), Is.Zero);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, ["a", "b"],
                flags: CommandFlags.None), Is.EqualTo(new long[] { 0, 0 }));
        }
    }

    /// <summary>
    /// 显式传入 CancellationToken.None。
    /// </summary>
    [Test]
    public void CMS_Query_CancellationToken_Default()
    {
        var key = $"Sketch:{{CMS_Query}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 1000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, "hello",
                cancellationToken: CancellationToken.None), Is.Zero);

            Assert.That(this._redis.Modules.Sketch.CMS_Query(
                key, ["a", "b"],
                cancellationToken: CancellationToken.None),
                Is.EqualTo(new long[] { 0, 0 }));
        }
    }

    #endregion

    #region CMS_Info

    private static void CMS_InfoRedisError<TError>(Action action)
        where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 使用 INITBYDIM 创建后，验证三个属性。
    /// </summary>
    [Test]
    public void CMS_Info_InitByDim()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 7), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(7));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 最小维度的 Sketch 元数据。
    /// </summary>
    [Test]
    public void CMS_Info_MinimumDimensions()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 1, 1), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(1));
            Assert.That(info.Depth, Is.EqualTo(1));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 不同维度配置的准确解析。
    /// </summary>
    [TestCase(10L, 3L)]
    [TestCase(100L, 5L)]
    [TestCase(123L, 7L)]
    [TestCase(1000L, 10L)]
    [TestCase(4096L, 16L)]
    public void CMS_Info_DifferentDimensions(long width, long depth)
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, width, depth), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(width));
            Assert.That(info.Depth, Is.EqualTo(depth));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 使用 INITBYPROB 创建后读取元数据。
    /// </summary>
    [Test]
    public void CMS_Info_InitByProb()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByProb(
            key, 0.001, 0.01), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.GreaterThan(0));
            Assert.That(info.Depth, Is.GreaterThan(0));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 新建 Sketch 后增加单个元素的计数。
    /// </summary>
    [Test]
    public void CMS_Info_AfterSingleIncrement()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", 15);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Count, Is.EqualTo(15));
        }

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.EqualTo(15));
        }
    }

    /// <summary>
    /// 多次增加同一个元素，Count 应累计。
    /// </summary>
    [Test]
    public void CMS_Info_AfterMultipleIncrements()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 2000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 20).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 30).IsSuccess, Is.True);
        }

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.EqualTo(60));
        }
    }

    /// <summary>
    /// 批量增加不同元素，Count 是总增量，而非元素数量。
    /// </summary>
    [Test]
    public void CMS_Info_AfterBatchIncrement()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                ((SharpValue)"apple", 10L),
            ((SharpValue)"banana", 20L),
            ((SharpValue)"orange", 30L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        foreach (var result in results)
            Assert.That(result.IsSuccess, Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(2000));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.EqualTo(60));
        }
    }

    /// <summary>
    /// Count 不是去重后的元素数量。
    /// </summary>
    [Test]
    public void CMS_Info_CountNotDistinctItems()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var results = this._redis.Modules.Sketch.CMS_IncrBy(
            key,
            [
                ((SharpValue)"same", 5L),
            ((SharpValue)"same", 10L),
            ((SharpValue)"same", 20L)
            ]);

        Assert.That(results, Has.Length.EqualTo(3));

        foreach (var result in results)
            Assert.That(result.IsSuccess, Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        Assert.That(info.Count, Is.EqualTo(35));
    }

    /// <summary>
    /// 查询操作不修改 Sketch 的元数据。
    /// </summary>
    [Test]
    public void CMS_Info_QueryDoesNotChangeInfo()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 2000, 5), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "hello", 25).IsSuccess, Is.True);
        }

        var before = this._redis.Modules.Sketch.CMS_Info(key);

        _ = this._redis.Modules.Sketch.CMS_Query(key, "hello");
        _ = this._redis.Modules.Sketch.CMS_Query(
            key, ["hello", "other", "hello"]);

        var after = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Width, Is.EqualTo(before.Width));
            Assert.That(after.Depth, Is.EqualTo(before.Depth));
            Assert.That(after.Count, Is.EqualTo(before.Count));
        }
    }

    /// <summary>
    /// 重复执行 CMS.INFO，结果应保持一致。
    /// </summary>
    [Test]
    public void CMS_Info_RepeatedCalls()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 100, 3), Is.True);

        for (int i = 0; i < 5; i++)
        {
            var info = this._redis.Modules.Sketch.CMS_Info(key);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(info.Width, Is.EqualTo(100));
                Assert.That(info.Depth, Is.EqualTo(3));
                Assert.That(info.Count, Is.Zero);
            }
        }
    }

    /// <summary>
    /// Count 在不发生溢出的情况下可以超过 int.MaxValue。
    /// </summary>
    [Test]
    public void CMS_Info_CountGreaterThanIntMaxValue()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        const long increment = 3_000_000_000L;

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", increment);

        Assert.That(result.IsSuccess, Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        Assert.That(info.Count, Is.EqualTo(increment));
    }

    /// <summary>
    /// CMS.INFO 可以读取计数器已达到 uint.MaxValue 的 Sketch。
    /// 使用恰好等于上限的合法增量，不依赖溢出后更新行为。
    /// </summary>
    [Test]
    public void CMS_Info_CountAtUintMaxValue()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 2000, 5), Is.True);

        var result = this._redis.Modules.Sketch.CMS_IncrBy(
            key, "hello", (long)uint.MaxValue);

        Assert.That(result.IsSuccess, Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        Assert.That(info.Count, Is.EqualTo((long)uint.MaxValue));
    }

    /// <summary>
    /// 宽度为 1 时，多个元素产生碰撞，但 Count 仍是累计增量。
    /// </summary>
    [Test]
    public void CMS_Info_CollisionCount()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key, 1, 1), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "apple", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key, "banana", 20).IsSuccess, Is.True);
        }

        var info = this._redis.Modules.Sketch.CMS_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(1));
            Assert.That(info.Depth, Is.EqualTo(1));
            Assert.That(info.Count, Is.EqualTo(30));
        }
    }

    /// <summary>
    /// 不同 Key 的元数据互不影响。
    /// </summary>
    [Test]
    public void CMS_Info_DifferentKeys()
    {
        var key1 = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";
        var key2 = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                    key1, 100, 3), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
                key2, 2000, 7), Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key1, "hello", 10).IsSuccess, Is.True);

            Assert.That(this._redis.Modules.Sketch.CMS_IncrBy(
                key2, "hello", 50).IsSuccess, Is.True);
        }

        var info1 = this._redis.Modules.Sketch.CMS_Info(key1);
        var info2 = this._redis.Modules.Sketch.CMS_Info(key2);

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

    /// <summary>
    /// Key 不存在。
    /// </summary>
    [Test]
    public void CMS_Info_KeyNotExists()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        CMS_InfoRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Info(key));
    }

    /// <summary>
    /// String 类型的 Key。
    /// </summary>
    [Test]
    public void CMS_Info_WrongKeyType()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CMS_InfoRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Info(key));
    }

    /// <summary>
    /// Bloom Filter 类型的 Key。
    /// </summary>
    [Test]
    public void CMS_Info_BloomKeyWrongType()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "hello"), Is.True);

        CMS_InfoRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Info(key));
    }

    /// <summary>
    /// Cuckoo Filter 类型的 Key。
    /// </summary>
    [Test]
    public void CMS_Info_CuckooKeyWrongType()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        CMS_InfoRedisError<RedisErrorException>(() =>
            this._redis.Modules.Sketch.CMS_Info(key));
    }

    /// <summary>
    /// 显式传入 CommandFlags.None。
    /// </summary>
    [Test]
    public void CMS_Info_Flags_None()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 100, 5), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(
            key, flags: CommandFlags.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Width, Is.EqualTo(100));
            Assert.That(info.Depth, Is.EqualTo(5));
            Assert.That(info.Count, Is.Zero);
        }
    }

    /// <summary>
    /// 显式传入 CancellationToken.None。
    /// </summary>
    [Test]
    public void CMS_Info_CancellationToken_Default()
    {
        var key = $"Sketch:{{CMS_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Sketch.CMS_InitByDim(
            key, 100, 5), Is.True);

        var info = this._redis.Modules.Sketch.CMS_Info(
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
