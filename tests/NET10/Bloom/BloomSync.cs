using SharpRedis;
using SharpRedis.Exceptions;
using SharpRedis.Modules;

namespace NET10.Bloom;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class BloomSync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region BF_Add

    private static string BF_AddKey() =>
        $"Bloom:{{BF_Add}}:{Guid.NewGuid():N}";

    private static void BF_AddRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 测试不存在的 Key 是否自动创建 Bloom Filter。
    /// </summary>
    [Test]
    public void BF_Add_AutoCreate()
    {
        var key = BF_AddKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item1"), Is.True);
        }

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Capacity, Is.GreaterThan(0));
        }
    }

    /// <summary>
    /// 测试首次添加元素。
    /// </summary>
    [Test]
    public void BF_Add_FirstInsert()
    {
        var key = BF_AddKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.000001, 1000), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "hello"), Is.True);
        }
    }

    /// <summary>
    /// 测试重复添加相同元素。
    /// </summary>
    [Test]
    public void BF_Add_Duplicate()
    {
        var key = BF_AddKey();

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "hello"), Is.True);

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "hello"), Is.False);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Bloom.BF_Info(key).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试添加多个不同元素。
    /// </summary>
    [Test]
    public void BF_Add_MultipleItems()
    {
        var key = BF_AddKey();

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, $"item:{i}"), Is.True);
        }

        var info = this._redis.Modules.Bloom.BF_Info(key);

        Assert.That(info.Items, Is.EqualTo(20));

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, $"item:{i}"), Is.True);
        }
    }

    /// <summary>
    /// 测试添加中文及 Unicode 内容。
    /// </summary>
    [Test]
    public void BF_Add_Unicode()
    {
        var key = BF_AddKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "你好，世界🌍"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "你好，世界🌍"), Is.True);
        }

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "你好，世界🌍"), Is.False);
    }

    /// <summary>
    /// 测试大小写不同的元素。
    /// </summary>
    [Test]
    public void BF_Add_CaseSensitive()
    {
        var key = BF_AddKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.000001, 1000), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "HELLO"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Info(key).Items,
                Is.EqualTo(3));
        }
    }

    /// <summary>
    /// 测试空字符串元素。
    /// Redis 允许 Bloom Filter 存储空字符串。
    /// </summary>
    [Test]
    public void BF_Add_EmptyItem()
    {
        var key = BF_AddKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, ""), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, ""), Is.True);
        }

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, ""), Is.False);
    }

    /// <summary>
    /// 测试特殊字符元素。
    /// </summary>
    [Test]
    public void BF_Add_SpecialCharacters()
    {
        var key = BF_AddKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.000001, 1000), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "a\nb\tc"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "a:b/c?d=e"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Info(key).Items,
                Is.EqualTo(3));
        }
    }

    /// <summary>
    /// 测试使用 BF.RESERVE 创建的过滤器。
    /// </summary>
    [Test]
    public void BF_Add_ReservedFilter()
    {
        var key = BF_AddKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, errorRate: 0.001, capacity: 100), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item2"), Is.True);
        }

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试不可扩容过滤器容量耗尽时返回错误。
    /// </summary>
    [Test]
    public void BF_Add_NonScaling_Full()
    {
        var key = BF_AddKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item1"), Is.True);
        }

        BF_AddRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Add(
                key, "item2"));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
        }

        // 即使过滤器已满，已经存在的元素仍可以检查。
        Assert.That(this._redis.Modules.Bloom.BF_Exists(
            key, "item1"), Is.True);
    }

    /// <summary>
    /// 测试在已满过滤器中重复添加已有元素。
    /// </summary>
    [Test]
    public void BF_Add_NonScaling_DuplicateAfterFull()
    {
        var key = BF_AddKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item1"), Is.True);
        }

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.False);
    }

    /// <summary>
    /// 测试可扩容过滤器达到初始容量后继续添加。
    /// </summary>
    [Test]
    public void BF_Add_Scaling()
    {
        var key = BF_AddKey();

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, $"item:{i}"), Is.True);
        }

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(10));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Capacity, Is.GreaterThan(2));
        }
    }

    /// <summary>
    /// 测试错误的 Key 数据类型。
    /// </summary>
    [Test]
    public void BF_Add_WrongKeyType()
    {
        var key = BF_AddKey();

        this._redis.String.Set(key, "hello");

        BF_AddRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Add(
                key, "item1"));
    }

    /// <summary>
    /// 测试 CommandFlags.None。
    /// </summary>
    [Test]
    public void BF_Add_Flags_None()
    {
        var key = BF_AddKey();

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1",
            flags: CommandFlags.None), Is.True);

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1",
            flags: CommandFlags.None), Is.False);
    }

    /// <summary>
    /// 测试默认 CancellationToken。
    /// </summary>
    [Test]
    public void BF_Add_CancellationToken_Default()
    {
        var key = BF_AddKey();

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.True);

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.False);
    }

    #endregion

    #region BF_Card

    private static string BF_CardKey() =>
        $"Bloom:{{BF_Card}}:{Guid.NewGuid():N}";

    private static void BF_CardRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void BF_Card_KeyNotExists()
    {
        var key = BF_CardKey();

        Assert.That(this._redis.Modules.Bloom.BF_Card(key),
            Is.Zero);
    }

    [Test]
    public void BF_Card_EmptyFilter()
    {
        var key = BF_CardKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.001, 100), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.Zero);
        }
    }

    [Test]
    public void BF_Card_AfterFirstAdd()
    {
        var key = BF_CardKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public void BF_Card_AfterDuplicateAdd()
    {
        var key = BF_CardKey();

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.False);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.False);

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public void BF_Card_MultipleItems()
    {
        var key = BF_CardKey();

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        for (int i = 1; i <= 10; i++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(this._redis.Modules.Bloom.BF_Add(
                            key, $"item:{i}"), Is.True);

                Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                    Is.EqualTo(i));
            }
        }
    }

    [Test]
    public void BF_Card_AfterMAdd()
    {
        var key = BF_CardKey();

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, ["item1", "item2", "item3"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added
            ]));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public void BF_Card_UnicodeItems()
    {
        var key = BF_CardKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.000001, 1000), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "你好"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "世界🌍"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "你好"), Is.False);

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(2));
        }
    }

    [Test]
    public void BF_Card_EqualsInfoItems()
    {
        var key = BF_CardKey();

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, $"value:{i}"), Is.True);
        }

        var card = this._redis.Modules.Bloom.BF_Card(key);
        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(card, Is.EqualTo(10));
            Assert.That(card, Is.EqualTo(info.Items));
        }
    }

    [Test]
    public void BF_Card_AfterScaling()
    {
        var key = BF_CardKey();

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, $"item:{i}"), Is.True);
        }

        var card = this._redis.Modules.Bloom.BF_Card(key);
        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(card, Is.EqualTo(10));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(card, Is.EqualTo(info.Items));
        }
    }

    [Test]
    public void BF_Card_WrongKeyType()
    {
        var key = BF_CardKey();

        this._redis.String.Set(key, "hello");

        BF_CardRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Card(key));
    }

    [Test]
    public void BF_Card_Flags_None()
    {
        var key = BF_CardKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Card(
                key, flags: CommandFlags.None), Is.EqualTo(1));
        }
    }

    [Test]
    public void BF_Card_CancellationToken_Default()
    {
        var key = BF_CardKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Card(
                key,
                cancellationToken: CancellationToken.None),
                Is.EqualTo(1));
        }
    }

    #endregion

    #region BF_Exists

    private static string BF_ExistsKey() =>
        $"Bloom:{{BF_Exists}}:{Guid.NewGuid():N}";

    [Test]
    public void BF_Exists_KeyNotExists()
    {
        var key = BF_ExistsKey();

        Assert.That(this._redis.Modules.Bloom.BF_Exists(
            key, "item1"), Is.False);
    }

    [Test]
    public void BF_Exists_EmptyFilter()
    {
        var key = BF_ExistsKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.001, 100), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item1"), Is.False);
        }
    }

    [Test]
    public void BF_Exists_AfterAdd()
    {
        var key = BF_ExistsKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item1"), Is.True);
        }
    }

    [Test]
    public void BF_Exists_AfterDuplicateAdd()
    {
        var key = BF_ExistsKey();

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.False);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item1"), Is.True);
        }
    }

    [Test]
    public void BF_Exists_MultipleItems()
    {
        var key = BF_ExistsKey();

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, $"item:{i}"), Is.True);
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public void BF_Exists_NotAdded_EmptyFilter()
    {
        var key = BF_ExistsKey();

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100), Is.True);

        // 空过滤器不会产生假阳性
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "world"), Is.False);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "123456"), Is.False);
        }
    }

    [Test]
    public void BF_Exists_Unicode()
    {
        var key = BF_ExistsKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "你好，世界🌍"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "你好，世界🌍"), Is.True);
        }
    }

    [Test]
    public void BF_Exists_EmptyItem()
    {
        var key = BF_ExistsKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, ""), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, ""), Is.True);
        }
    }

    [Test]
    public void BF_Exists_SpecialCharacters()
    {
        var key = BF_ExistsKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.000001, 1000), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "a\nb\tc"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "a:b/c?d=e"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "a\nb\tc"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "a:b/c?d=e"), Is.True);
        }
    }

    [Test]
    public void BF_Exists_CaseSensitive()
    {
        var key = BF_ExistsKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.000001, 1000), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "Hello"), Is.True);
        }

        // 大小写不同属于不同字节序列。
        // 但未添加的 hello 仍可能发生假阳性，
        // 因此这里不强制断言其返回 false。
    }

    [Test]
    public void BF_Exists_AfterMAdd()
    {
        var key = BF_ExistsKey();

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        var added = this._redis.Modules.Bloom.BF_MAdd(
            key, ["item1", "item2", "item3"]);

        Assert.That(added, Is.EqualTo(
        [
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added
        ]));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item2"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item3"), Is.True);
        }
    }

    [Test]
    public void BF_Exists_NonScalingFilter()
    {
        var key = BF_ExistsKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item1"), Is.True);
        }

        // 已达到容量的过滤器仍然允许检查已有元素。
    }

    [Test]
    public void BF_Exists_ReadDoesNotChangeCardinality()
    {
        var key = BF_ExistsKey();

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        var before = this._redis.Modules.Bloom.BF_Card(key);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item1"), Is.True);
        }

        var after = this._redis.Modules.Bloom.BF_Card(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.EqualTo(1));
            Assert.That(after, Is.EqualTo(before));
        }
    }

    [Test]
    public void BF_Exists_Flags_None()
    {
        var key = BF_ExistsKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item1",
                flags: CommandFlags.None), Is.True);
        }
    }

    [Test]
    public void BF_Exists_CancellationToken_Default()
    {
        var key = BF_ExistsKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item1",
                cancellationToken: CancellationToken.None), Is.True);
        }
    }

    #endregion

    #region BF_Info

    private static void BF_InfoRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void BF_Info_KeyNotExists()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        BF_InfoRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Info(key));

        BF_InfoRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Info(
                key, RedisBloom.InfoType.Capacity));
    }

    [Test]
    public void BF_Info_EmptyFilter()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    [Test]
    public void BF_Info_AutoCreatedFilter()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.GreaterThan(0));
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    [Test]
    public void BF_Info_CustomCapacity()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, errorRate: 0.001, capacity: 500), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(500));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Size, Is.GreaterThan(0));
        }
    }

    [Test]
    public void BF_Info_ItemsAfterAdd()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, $"item:{i}"), Is.True);
        }

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(10));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Capacity, Is.EqualTo(1000));
        }
    }

    [Test]
    public void BF_Info_DuplicateDoesNotIncreaseItems()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.False);

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.False);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        Assert.That(info.Items, Is.EqualTo(1));
    }

    [Test]
    public void BF_Info_AllInfoTypes()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.001,
                    capacity: 200,
                    expansion: 4), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item2"), Is.True);
        }

        var info = this._redis.Modules.Bloom.BF_Info(key);

        var capacity = this._redis.Modules.Bloom.BF_Info(
            key, RedisBloom.InfoType.Capacity);

        var size = this._redis.Modules.Bloom.BF_Info(
            key, RedisBloom.InfoType.Size);

        var filters = this._redis.Modules.Bloom.BF_Info(
            key, RedisBloom.InfoType.Filters);

        var items = this._redis.Modules.Bloom.BF_Info(
            key, RedisBloom.InfoType.Items);

        var expansion = this._redis.Modules.Bloom.BF_Info(
            key, RedisBloom.InfoType.Expansion);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(capacity, Is.EqualTo(info.Capacity));
            Assert.That(size, Is.EqualTo(info.Size));
            Assert.That(filters, Is.EqualTo(info.Filters));
            Assert.That(items, Is.EqualTo(info.Items));
            Assert.That(expansion, Is.EqualTo(info.Expansion));

            Assert.That(capacity, Is.EqualTo(200));
            Assert.That(filters, Is.EqualTo(1));
            Assert.That(items, Is.EqualTo(2));
            Assert.That(expansion, Is.EqualTo(4));
        }
    }

    [Test]
    public void BF_Info_CustomExpansion()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key,
            errorRate: 0.001,
            capacity: 100,
            expansion: 8), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Expansion.HasValue, Is.True);
            Assert.That(info.Expansion, Is.EqualTo(8));
            Assert.That(info.Filters, Is.EqualTo(1));
        }

        Assert.That(this._redis.Modules.Bloom.BF_Info(
            key, RedisBloom.InfoType.Expansion), Is.EqualTo(8));
    }

    [Test]
    public void BF_Info_NonScaling_ExpansionIsNull()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key,
            errorRate: 0.001,
            capacity: 100,
            nonscaling: true), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Expansion, Is.Null);
            Assert.That(info.Expansion.HasValue, Is.False);
        }
    }

    [Test]
    public void BF_Info_NonScaling_AfterAdd()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key,
            errorRate: 0.000001,
            capacity: 100,
            nonscaling: true), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, $"item:{i}"), Is.True);
        }

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.EqualTo(10));
            Assert.That(info.Expansion, Is.Null);
        }
    }

    [Test]
    public void BF_Info_AfterScaling()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, $"item:{i}"), Is.True);
        }

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.GreaterThan(2));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Items, Is.EqualTo(10));
            Assert.That(info.Expansion, Is.EqualTo(2));
            Assert.That(info.Size, Is.GreaterThan(0));
        }
    }

    [Test]
    public void BF_Info_WrongKeyType()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        BF_InfoRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Info(key));

        BF_InfoRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Info(
                key, RedisBloom.InfoType.Items));
    }

    [Test]
    public void BF_Info_Flags_None()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(
            key, flags: CommandFlags.None);

        var capacity = this._redis.Modules.Bloom.BF_Info(
            key,
            RedisBloom.InfoType.Capacity,
            flags: CommandFlags.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(capacity, Is.EqualTo(100));
        }
    }

    [Test]
    public void BF_Info_CancellationToken_Default()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(
            key, cancellationToken: CancellationToken.None);

        var capacity = this._redis.Modules.Bloom.BF_Info(
            key,
            RedisBloom.InfoType.Capacity,
            cancellationToken: CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(capacity, Is.EqualTo(100));
        }
    }

    [Test]
    public void BF_Info_NonScaling_ExpansionSelector()
    {
        var key = $"Bloom:{{BF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100, nonscaling: true), Is.True);

        long? expansion = this._redis.Modules.Bloom.BF_Info(
            key, RedisBloom.InfoType.Expansion);

        Assert.That(expansion, Is.Null);
    }
    #endregion

    #region BF_Insert

    private static void BF_InsertRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 测试单元素插入时自动创建过滤器。
    /// </summary>
    [Test]
    public void BF_Insert_Single_AutoCreate()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Bloom.BF_Insert(
            key, "item1");

        Assert.That(result, Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Capacity, Is.GreaterThan(0));
        }
    }

    /// <summary>
    /// 测试单元素重复插入。
    /// </summary>
    [Test]
    public void BF_Insert_Single_Duplicate()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Insert(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));

        Assert.That(this._redis.Modules.Bloom.BF_Insert(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Insert(
                    key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试多元素插入时自动创建过滤器。
    /// </summary>
    [Test]
    public void BF_Insert_Multiple_AutoCreate()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Bloom.BF_Insert(
            key, ["item1", "item2", "item3"],
            error: 0.000001,
            capacity: 1000);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added
            ]));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(3));
        }
    }

    /// <summary>
    /// 测试批量插入包含重复元素。
    /// </summary>
    [Test]
    public void BF_Insert_Multiple_Duplicate()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Bloom.BF_Insert(
            key, ["item1", "item2", "item1", "item2", "item3"],
            capacity: 1000,
            error: 0.000001);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Exists,
                RedisBloom.AddStatus.Exists,
                RedisBloom.AddStatus.Added
            ]));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(3));
        }
    }

    /// <summary>
    /// 测试向已存在的过滤器插入元素。
    /// </summary>
    [Test]
    public void BF_Insert_ExistingFilter()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 100), Is.True);

        var result = this._redis.Modules.Bloom.BF_Insert(
            key, ["item1", "item2"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added
        ]));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Items, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试 CAPACITY 参数控制自动创建过滤器的容量。
    /// </summary>
    [Test]
    public void BF_Insert_CustomCapacity()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Insert(
            key, "item1",
            capacity: 500,
            error: 0.001), Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(500));
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试 EXPANSION 参数设置扩容因子。
    /// </summary>
    [Test]
    public void BF_Insert_CustomExpansion()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Bloom.BF_Insert(
            key, "item1",
            capacity: 100,
            error: 0.001,
            expansion: 4);

        Assert.That(result, Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Expansion, Is.EqualTo(4));
            Assert.That(info.Items, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试过滤器已存在时 CAPACITY 和 EXPANSION 不改变原配置。
    /// </summary>
    [Test]
    public void BF_Insert_ExistingFilter_OptionsIgnored()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.001,
                    capacity: 100,
                    expansion: 2), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Insert(
                key, "item1",
                capacity: 500,
                error: 0.0001,
                expansion: 8), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Expansion, Is.EqualTo(2));
            Assert.That(info.Items, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试 NOCREATE 且 Key 不存在时抛出异常。
    /// </summary>
    [Test]
    public void BF_Insert_NoCreate_KeyNotExists()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        BF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Insert(
                key, "item1", nocreate: true));

        BF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Insert(
                key, ["item1", "item2"], nocreate: true));

        Assert.That(this._redis.Modules.Bloom.BF_Card(key),
            Is.Zero);
    }

    /// <summary>
    /// 测试 NOCREATE 且过滤器存在时正常添加。
    /// </summary>
    [Test]
    public void BF_Insert_NoCreate_ExistingFilter()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.000001, 100), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Insert(
                key, "item1", nocreate: true),
                Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        var result = this._redis.Modules.Bloom.BF_Insert(
            key, ["item1", "item2"], nocreate: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Exists,
                RedisBloom.AddStatus.Added
            ]));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试 NONSCALING 创建不可扩容过滤器。
    /// </summary>
    [Test]
    public void BF_Insert_NonScaling_Create()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Insert(
            key, "item1",
            capacity: 100,
            error: 0.001,
            nonscaling: true),
            Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Null);
        }
    }

    /// <summary>
    /// 测试单元素插入时容量已满返回 Error。
    /// </summary>
    [Test]
    public void BF_Insert_Single_NonScalingFull()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Insert(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));

            Assert.That(this._redis.Modules.Bloom.BF_Insert(
                key, "item2"), Is.EqualTo(RedisBloom.AddStatus.Error));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试批量插入时容量已满，只返回一个 Error 元素。
    /// </summary>
    [Test]
    public void BF_Insert_Multiple_NonScalingFull()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Insert(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        var result = this._redis.Modules.Bloom.BF_Insert(
            key, ["item2", "item3", "item4"]);

        using (Assert.EnterMultipleScope())
        {
            // 第一个元素就遇到容量已满。
            // 返回数组长度为 1，而不是 3。
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Error
            ]));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试批量插入部分成功后遇到容量已满，提前结束。
    /// </summary>
    [Test]
    public void BF_Insert_Multiple_PartialSuccess()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key,
            errorRate: 0.000001,
            capacity: 2,
            nonscaling: true), Is.True);

        var result = this._redis.Modules.Bloom.BF_Insert(
            key, ["item1", "item2", "item3", "item4"]);

        // 前两个元素成功，第三个元素触发容量错误。
        // 第四个元素不会继续处理。
        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Error
        ]));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试已满过滤器中的重复元素返回 Exists。
    /// </summary>
    [Test]
    public void BF_Insert_NonScalingFull_Duplicate()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Insert(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        Assert.That(this._redis.Modules.Bloom.BF_Insert(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

        var result = this._redis.Modules.Bloom.BF_Insert(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Exists,
            RedisBloom.AddStatus.Error
        ]));
    }

    /// <summary>
    /// 测试超过初始容量时允许自动扩容。
    /// </summary>
    [Test]
    public void BF_Insert_AutoScaling()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Bloom.BF_Insert(
            key, ["item1", "item2", "item3", "item4", "item5"],
            capacity: 2,
            error: 0.000001,
            expansion: 2);

        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added
        ]));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(5));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Capacity, Is.GreaterThan(2));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试 Unicode、空字符串与特殊字符。
    /// </summary>
    [Test]
    public void BF_Insert_SpecialItems()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Bloom.BF_Insert(
            key, ["", "你好🌍", "a b c", "a\nb\tc"],
            capacity: 1000,
            error: 0.000001);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added
            ]));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(4));
        }
    }

    /// <summary>
    /// 测试非法误判率。
    /// </summary>
    [Test]
    public void BF_Insert_InvalidErrorRate()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        BF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Insert(
                key, "item1", error: -0.1));

        BF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Insert(
                key, ["item1", "item2"], error: 0));
    }

    /// <summary>
    /// 测试非法容量。
    /// </summary>
    [Test]
    public void BF_Insert_InvalidCapacity()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        BF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Insert(
                key, "item1", capacity: 0));

        BF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Insert(
                key, ["item1", "item2"], capacity: -1));
    }

    /// <summary>
    /// 测试错误 Key 类型。
    /// </summary>
    [Test]
    public void BF_Insert_WrongKeyType()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        BF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Insert(
                key, "item1"));

        BF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Insert(
                key, ["item1", "item2"]));
    }

    /// <summary>
    /// 测试 CommandFlags.None。
    /// </summary>
    [Test]
    public void BF_Insert_Flags_None()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Insert(
            key, "item1",
            flags: CommandFlags.None),
            Is.EqualTo(RedisBloom.AddStatus.Added));

        Assert.That(this._redis.Modules.Bloom.BF_Insert(
            key, "item1",
            flags: CommandFlags.None),
            Is.EqualTo(RedisBloom.AddStatus.Exists));
    }

    /// <summary>
    /// 测试 CancellationToken.None。
    /// </summary>
    [Test]
    public void BF_Insert_CancellationToken_Default()
    {
        var key = $"Bloom:{{BF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Insert(
                    key, "item1",
                    cancellationToken: CancellationToken.None),
                    Is.EqualTo(RedisBloom.AddStatus.Added));

            Assert.That(this._redis.Modules.Bloom.BF_Insert(
                key, ["item1", "item2"],
                cancellationToken: CancellationToken.None),
                Is.EqualTo(
                    [
                    RedisBloom.AddStatus.Exists,
                RedisBloom.AddStatus.Added
                ]));
        }
    }

    #endregion

    #region BF_ScanDump_BF_LoadChunk

    private static void BF_ScanDumpRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 导出全部有效数据块并恢复到目标过滤器。
    /// </summary>
    private void BF_ScanDumpRestore(string source, string target)
    {
        long iterator = 0;

        do
        {
            var chunk = this._redis.Modules.Bloom.BF_ScanDump(
                source, iterator);

            iterator = chunk.Iterator;

            if (chunk.IsCompleted)
                break;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(chunk.Iterator, Is.GreaterThan(0));
                Assert.That(chunk.Data, Is.Not.Null.And.Not.Empty);

                Assert.That(this._redis.Modules.Bloom.BF_LoadChunk(
                    target, chunk.Iterator, chunk.Data), Is.True);
            }

        } while (true);
    }

    /// <summary>
    /// 测试首次 SCANDUMP 返回有效数据块。
    /// </summary>
    [Test]
    public void BF_ScanDump_FirstChunk()
    {
        var key = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        var result = this._redis.Modules.Bloom.BF_ScanDump(key, 0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Iterator, Is.GreaterThan(0));
            Assert.That(result.Data, Is.Not.Null.And.Not.Empty);
            Assert.That(result.IsCompleted, Is.False);
        }
    }

    /// <summary>
    /// 测试导出游标最终返回 0。
    /// </summary>
    [Test]
    public void BF_ScanDump_IteratorCompletion()
    {
        var key = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        long iterator = 0;
        int count = 0;
        bool completed = false;

        for (int i = 0; i < 10000; i++)
        {
            var chunk = this._redis.Modules.Bloom.BF_ScanDump(
                key, iterator);

            count++;

            if (chunk.IsCompleted)
            {
                Assert.That(chunk.Iterator, Is.Zero);
                completed = true;
                break;
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(chunk.Iterator, Is.GreaterThan(0));
                Assert.That(chunk.Data, Is.Not.Null.And.Not.Empty);
            }

            iterator = chunk.Iterator;
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(completed, Is.True);
            Assert.That(count, Is.GreaterThanOrEqualTo(2));
        }
    }

    /// <summary>
    /// 测试单个元素完整导出并恢复。
    /// </summary>
    [Test]
    public void BF_LoadChunk_SingleItemRestore()
    {
        var source = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            source, "hello"), Is.True);

        this.BF_ScanDumpRestore(source, target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                    target, "hello"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Card(target),
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试多个元素完整恢复。
    /// </summary>
    [Test]
    public void BF_LoadChunk_MultipleItemsRestore()
    {
        var source = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            source, 0.000001, 1000), Is.True);

        long inserted = 0;

        for (int i = 0; i < 100; i++)
        {
            if (this._redis.Modules.Bloom.BF_Add(
                source, $"item:{i}"))
            {
                inserted++;
            }
        }

        this.BF_ScanDumpRestore(source, target);

        var sourceInfo = this._redis.Modules.Bloom.BF_Info(source);
        var targetInfo = this._redis.Modules.Bloom.BF_Info(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(targetInfo.Capacity, Is.EqualTo(sourceInfo.Capacity));
            Assert.That(targetInfo.Filters, Is.EqualTo(sourceInfo.Filters));
            Assert.That(targetInfo.Items, Is.EqualTo(sourceInfo.Items));
            Assert.That(targetInfo.Items, Is.EqualTo(inserted));
            Assert.That(targetInfo.Expansion, Is.EqualTo(sourceInfo.Expansion));
            Assert.That(targetInfo.Size, Is.EqualTo(sourceInfo.Size));
        }

        for (int i = 0; i < 100; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                target, $"item:{i}"), Is.True);
        }
    }

    /// <summary>
    /// 测试空 Bloom Filter 的备份与恢复。
    /// </summary>
    [Test]
    public void BF_LoadChunk_EmptyFilterRestore()
    {
        var source = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            source, 0.001, 100), Is.True);

        this.BF_ScanDumpRestore(source, target);

        var info = this._redis.Modules.Bloom.BF_Info(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试 Unicode、空字符串和特殊字符完整恢复。
    /// </summary>
    [Test]
    public void BF_LoadChunk_SpecialItemsRestore()
    {
        var source = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    source, 0.000001, 1000), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                source, ""), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                source, "中文🌍"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                source, "a\nb\tc"), Is.True);
        }

        this.BF_ScanDumpRestore(source, target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                target, ""), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                target, "中文🌍"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                target, "a\nb\tc"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Card(target),
                Is.EqualTo(3));
        }
    }

    /// <summary>
    /// 测试不可扩容过滤器恢复后仍保持配置。
    /// </summary>
    [Test]
    public void BF_LoadChunk_NonScalingRestore()
    {
        var source = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    source,
                    errorRate: 0.000001,
                    capacity: 2,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                source, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                source, "item2"), Is.True);
        }

        this.BF_ScanDumpRestore(source, target);

        var info = this._redis.Modules.Bloom.BF_Info(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(2));
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Null);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                target, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                target, "item2"), Is.True);
        }
    }

    /// <summary>
    /// 测试已扩容过滤器恢复后保留多个子过滤器。
    /// </summary>
    [Test]
    public void BF_LoadChunk_ScalingRestore()
    {
        var source = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            source,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            this._redis.Modules.Bloom.BF_Add(source, $"item:{i}");
        }

        var before = this._redis.Modules.Bloom.BF_Info(source);

        Assert.That(before.Filters, Is.GreaterThan(1));

        this.BF_ScanDumpRestore(source, target);

        var after = this._redis.Modules.Bloom.BF_Info(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Capacity, Is.EqualTo(before.Capacity));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.Expansion, Is.EqualTo(before.Expansion));
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                target, $"item:{i}"), Is.True);
        }
    }

    /// <summary>
    /// 测试较大过滤器的多数据块恢复。
    /// </summary>
    [Test]
    public void BF_LoadChunk_MultipleChunksRestore()
    {
        var source = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            source,
            errorRate: 0.000001,
            capacity: 100000), Is.True);

        for (int i = 0; i < 100; i++)
        {
            this._redis.Modules.Bloom.BF_Add(source, $"item:{i}");
        }

        long iterator = 0;
        int chunkCount = 0;

        while (true)
        {
            var chunk = this._redis.Modules.Bloom.BF_ScanDump(
                source, iterator);

            if (chunk.IsCompleted)
                break;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(chunk.Data, Is.Not.Null.And.Not.Empty);

                Assert.That(this._redis.Modules.Bloom.BF_LoadChunk(
                    target, chunk.Iterator, chunk.Data), Is.True);
            }

            iterator = chunk.Iterator;
            chunkCount++;
        }

        var sourceInfo = this._redis.Modules.Bloom.BF_Info(source);
        var targetInfo = this._redis.Modules.Bloom.BF_Info(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(chunkCount, Is.GreaterThan(1));
            Assert.That(targetInfo.Capacity, Is.EqualTo(sourceInfo.Capacity));
            Assert.That(targetInfo.Items, Is.EqualTo(sourceInfo.Items));
            Assert.That(targetInfo.Size, Is.EqualTo(sourceInfo.Size));
        }

        for (int i = 0; i < 100; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                target, $"item:{i}"), Is.True);
        }
    }

    /// <summary>
    /// 测试源 Key 不存在时无法导出。
    /// </summary>
    [Test]
    public void BF_ScanDump_KeyNotExists()
    {
        var key = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";

        BF_ScanDumpRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_ScanDump(key, 0));
    }

    /// <summary>
    /// 测试 SCANDUMP 遇到错误 Key 类型。
    /// </summary>
    [Test]
    public void BF_ScanDump_WrongKeyType()
    {
        var key = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        BF_ScanDumpRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_ScanDump(key, 0));
    }

    /// <summary>
    /// 测试 LOADCHUNK 无效二进制数据。
    /// </summary>
    [Test]
    public void BF_LoadChunk_InvalidData()
    {
        var key = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        BF_ScanDumpRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_LoadChunk(
                key, 1, [0x01, 0x02, 0x03]));
    }

    /// <summary>
    /// 测试 LOADCHUNK 遇到错误 Key 类型。
    /// </summary>
    [Test]
    public void BF_LoadChunk_WrongKeyType()
    {
        var key = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        var source = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            source, "item1"), Is.True);

        var chunk = this._redis.Modules.Bloom.BF_ScanDump(source, 0);

        BF_ScanDumpRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_LoadChunk(
                key, chunk.Iterator, chunk.Data));
    }

    /// <summary>
    /// 测试 LOADCHUNK 未传入首块元数据。
    /// </summary>
    [Test]
    public void BF_LoadChunk_MissingHeader()
    {
        var source = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            source, "item1"), Is.True);

        var first = this._redis.Modules.Bloom.BF_ScanDump(source, 0);

        Assert.That(first.IsCompleted, Is.False);

        var second = this._redis.Modules.Bloom.BF_ScanDump(
            source, first.Iterator);

        Assert.That(second.IsCompleted, Is.False);

        BF_ScanDumpRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_LoadChunk(
                target, second.Iterator, second.Data));
    }


    [Test]
    public void BF_ScanDump_LoadChunk_Flags_None()
    {
        var source = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            source, "item1"), Is.True);

        long iterator = 0;

        while (true)
        {
            var chunk = this._redis.Modules.Bloom.BF_ScanDump(
                source, iterator, flags: CommandFlags.None);

            if (chunk.IsCompleted)
                break;

            Assert.That(this._redis.Modules.Bloom.BF_LoadChunk(
                target, chunk.Iterator, chunk.Data,
                flags: CommandFlags.None), Is.True);

            iterator = chunk.Iterator;
        }

        Assert.That(this._redis.Modules.Bloom.BF_Exists(
            target, "item1"), Is.True);
    }

    [Test]
    public void BF_ScanDump_LoadChunk_CancellationToken_Default()
    {
        var source = $"Bloom:{{BF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            source, "item1"), Is.True);

        long iterator = 0;

        while (true)
        {
            var chunk = this._redis.Modules.Bloom.BF_ScanDump(
                source, iterator,
                cancellationToken: CancellationToken.None);

            if (chunk.IsCompleted)
                break;

            Assert.That(this._redis.Modules.Bloom.BF_LoadChunk(
                target, chunk.Iterator, chunk.Data,
                cancellationToken: CancellationToken.None), Is.True);

            iterator = chunk.Iterator;
        }

        Assert.That(this._redis.Modules.Bloom.BF_Exists(
            target, "item1"), Is.True);
    }


    #endregion

    #region BF_MAdd

    private static void BF_MAddRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 测试单元素添加时自动创建布隆过滤器。
    /// </summary>
    [Test]
    public void BF_MAdd_Single_AutoCreate()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, "item1");

        Assert.That(result, Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Capacity, Is.GreaterThan(0));
        }

        Assert.That(this._redis.Modules.Bloom.BF_Exists(
            key, "item1"), Is.True);
    }

    /// <summary>
    /// 测试单元素重复添加。
    /// </summary>
    [Test]
    public void BF_MAdd_Single_Duplicate()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_MAdd(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));

        Assert.That(this._redis.Modules.Bloom.BF_MAdd(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_MAdd(
                    key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试批量添加时自动创建过滤器。
    /// </summary>
    [Test]
    public void BF_MAdd_Multiple_AutoCreate()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Has.Length.EqualTo(3));
        Assert.That(result[0], Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items,
                Is.EqualTo(result.Count(x => x == RedisBloom.AddStatus.Added)));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item2"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "item3"), Is.True);
        }
    }

    /// <summary>
    /// 测试批量添加包含重复元素。
    /// </summary>
    [Test]
    public void BF_MAdd_Multiple_Duplicate()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, ["item1", "item1", "item2", "item2", "item1"]);

        Assert.That(result, Has.Length.EqualTo(5));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0], Is.EqualTo(RedisBloom.AddStatus.Added));
            Assert.That(result[1], Is.EqualTo(RedisBloom.AddStatus.Exists));
            Assert.That(result[3], Is.EqualTo(RedisBloom.AddStatus.Exists));
            Assert.That(result[4], Is.EqualTo(RedisBloom.AddStatus.Exists));
        }

        Assert.That(this._redis.Modules.Bloom.BF_Card(key),
            Is.EqualTo(result.Count(x => x == RedisBloom.AddStatus.Added)));
    }

    /// <summary>
    /// 测试向已有过滤器批量添加元素。
    /// </summary>
    [Test]
    public void BF_MAdd_ExistingFilter()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.000001, 1000), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item1"), Is.True);
        }

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Has.Length.EqualTo(3));

        Assert.That(result[0],
            Is.EqualTo(RedisBloom.AddStatus.Exists));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(1000));
            Assert.That(info.Items,
                Is.EqualTo(1 + result.Count(x => x == RedisBloom.AddStatus.Added)));
        }
    }

    /// <summary>
    /// 测试单元素和多元素重载返回值的一致性。
    /// </summary>
    [Test]
    public void BF_MAdd_SingleAndMultiple()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_MAdd(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, ["item1"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Exists
            ]));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试批量添加后再次批量添加相同元素。
    /// </summary>
    [Test]
    public void BF_MAdd_Multiple_RepeatedBatch()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        var first = this._redis.Modules.Bloom.BF_MAdd(
            key, ["a", "b", "c"]);

        Assert.That(first, Has.Length.EqualTo(3));

        var before = this._redis.Modules.Bloom.BF_Card(key);

        var second = this._redis.Modules.Bloom.BF_MAdd(
            key, ["a", "b", "c"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(second, Is.EqualTo(
            [
                RedisBloom.AddStatus.Exists,
                RedisBloom.AddStatus.Exists,
                RedisBloom.AddStatus.Exists
            ]));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(before));
        }
    }

    /// <summary>
    /// 测试中文、Unicode、空字符串及特殊字符。
    /// </summary>
    [Test]
    public void BF_MAdd_SpecialItems()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, ["", "中文🌍", "hello world", "a\nb\tc"]);

        Assert.That(result, Has.Length.EqualTo(4));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, ""), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "中文🌍"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "hello world"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "a\nb\tc"), Is.True);
        }

        Assert.That(this._redis.Modules.Bloom.BF_Card(key),
            Is.EqualTo(result.Count(x => x == RedisBloom.AddStatus.Added)));
    }

    /// <summary>
    /// 测试不同大小写的元素。
    /// </summary>
    [Test]
    public void BF_MAdd_CaseSensitive()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, ["Hello", "hello", "HELLO"]);

        Assert.That(result, Has.Length.EqualTo(3));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, "HELLO"), Is.True);
        }
    }

    /// <summary>
    /// 测试不可扩容过滤器满时单元素返回 Error。
    /// </summary>
    [Test]
    public void BF_MAdd_Single_NonScalingFull()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MAdd(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_MAdd(
                    key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

            // 如果单元素重载采用 BF.MADD 并解析数组内错误，
            // 容量满时应返回 AddStatus.Error。
            Assert.That(this._redis.Modules.Bloom.BF_MAdd(
                key, "item2"), Is.EqualTo(RedisBloom.AddStatus.Error));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试不可扩容过滤器满时批量结果提前终止。
    /// </summary>
    [Test]
    public void BF_MAdd_Multiple_NonScalingFull()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MAdd(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, ["item2", "item3", "item4"]);

        using (Assert.EnterMultipleScope())
        {
            // 第一个元素触发容量错误，后续元素不再处理。
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Error
            ]));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试批量添加部分成功后遇到容量错误。
    /// </summary>
    [Test]
    public void BF_MAdd_Multiple_PartialSuccess()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key,
            errorRate: 0.000001,
            capacity: 2,
            nonscaling: true), Is.True);

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, ["item1", "item2", "item3", "item4"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Error
            ]));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试已满过滤器中，重复元素仍返回 Exists。
    /// </summary>
    [Test]
    public void BF_MAdd_NonScalingFull_Duplicate()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MAdd(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Exists,
            RedisBloom.AddStatus.Error
        ]));
    }

    /// <summary>
    /// 测试可扩容过滤器超过初始容量后继续添加。
    /// </summary>
    [Test]
    public void BF_MAdd_AutoScaling()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        var result = this._redis.Modules.Bloom.BF_MAdd(
            key, ["item1", "item2", "item3", "item4", "item5"]);

        Assert.That(result, Has.Length.EqualTo(5));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items,
                Is.EqualTo(result.Count(x => x == RedisBloom.AddStatus.Added)));

            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Capacity, Is.GreaterThan(2));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试错误 Key 类型。
    /// </summary>
    [Test]
    public void BF_MAdd_WrongKeyType()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        BF_MAddRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_MAdd(
                key, "item1"));

        BF_MAddRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_MAdd(
                key, ["item1", "item2"]));
    }

    /// <summary>
    /// 测试 CommandFlags.None。
    /// </summary>
    [Test]
    public void BF_MAdd_Flags_None()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_MAdd(
                    key, "item1",
                    flags: CommandFlags.None),
                    Is.EqualTo(RedisBloom.AddStatus.Added));

            Assert.That(this._redis.Modules.Bloom.BF_MAdd(
                key, ["item1", "item2"],
                flags: CommandFlags.None),
                Is.EqualTo(
                [
                    RedisBloom.AddStatus.Exists,
                    RedisBloom.AddStatus.Added
                ]));
        }
    }

    /// <summary>
    /// 测试 CancellationToken.None。
    /// </summary>
    [Test]
    public void BF_MAdd_CancellationToken_Default()
    {
        var key = $"Bloom:{{BF_MAdd}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_MAdd(
                    key, "item1",
                    cancellationToken: CancellationToken.None),
                    Is.EqualTo(RedisBloom.AddStatus.Added));

            Assert.That(this._redis.Modules.Bloom.BF_MAdd(
                key, ["item1", "item2"],
                cancellationToken: CancellationToken.None),
                Is.EqualTo(
                [
                    RedisBloom.AddStatus.Exists,
                    RedisBloom.AddStatus.Added
                ]));
        }
    }

    #endregion

    #region BF_MExists

    /// <summary>
    /// 测试不存在的 Key，单元素查询返回 false。
    /// </summary>
    [Test]
    public void BF_MExists_Single_KeyNotExists()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_MExists(
            key, "item1"), Is.False);
    }

    /// <summary>
    /// 测试不存在的 Key，批量查询全部返回 false。
    /// </summary>
    [Test]
    public void BF_MExists_Multiple_KeyNotExists()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Bloom.BF_MExists(
            key, ["item1", "item2", "item3"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                false, false, false
            ]));

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.Zero);
        }
    }

    /// <summary>
    /// 测试已创建但没有插入元素的过滤器。
    /// </summary>
    [Test]
    public void BF_MExists_EmptyFilter()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.001, 100), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, "item1"), Is.False);

            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, ["item1", "item2", "item3"]),
                Is.EqualTo([false, false, false]));
        }
    }

    /// <summary>
    /// 测试单元素插入后查询。
    /// </summary>
    [Test]
    public void BF_MExists_Single_AfterAdd()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, "item1"), Is.True);
        }
    }

    /// <summary>
    /// 测试单元素重复添加后仍然存在。
    /// </summary>
    [Test]
    public void BF_MExists_Single_Duplicate()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.False);

            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Card(key),
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试多个已添加元素，批量查询全部返回 true。
    /// </summary>
    [Test]
    public void BF_MExists_Multiple_AllExists()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.000001, 1000), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MAdd(
                key, ["item1", "item2", "item3"]),
                Has.Length.EqualTo(3));
        }

        var result = this._redis.Modules.Bloom.BF_MExists(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    /// <summary>
    /// 测试单元素与多元素重载的结果一致。
    /// </summary>
    [Test]
    public void BF_MExists_SingleAndMultiple()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        var single = this._redis.Modules.Bloom.BF_MExists(
            key, "item1");

        var multiple = this._redis.Modules.Bloom.BF_MExists(
            key, ["item1"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(single, Is.True);
            Assert.That(multiple, Is.EqualTo([true]));
        }
    }

    /// <summary>
    /// 测试批量查询重复元素，返回结果与输入顺序对应。
    /// </summary>
    [Test]
    public void BF_MExists_Multiple_DuplicateItems()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "hello"), Is.True);

        var result = this._redis.Modules.Bloom.BF_MExists(
            key, ["hello", "hello", "hello", "hello"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true
        ]));
    }

    /// <summary>
    /// 测试先查询不存在的元素，再插入后查询存在。
    /// </summary>
    [Test]
    public void BF_MExists_BeforeAndAfterAdd()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.001, 100), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, ["item1", "item2"]),
                Is.EqualTo([false, false]));

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item2"), Is.True);
        }

        Assert.That(this._redis.Modules.Bloom.BF_MExists(
            key, ["item1", "item2"]),
            Is.EqualTo([true, true]));
    }

    /// <summary>
    /// 测试 Unicode、中文、空字符串、特殊字符。
    /// </summary>
    [Test]
    public void BF_MExists_SpecialItems()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        this._redis.Modules.Bloom.BF_MAdd(
            key, ["", "你好🌍", "a b c", "a\nb\tc"]);

        var result = this._redis.Modules.Bloom.BF_MExists(
            key, ["", "你好🌍", "a b c", "a\nb\tc"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true
        ]));
    }

    /// <summary>
    /// 测试大小写不同的元素。
    /// </summary>
    [Test]
    public void BF_MExists_CaseSensitive()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.000001, 1000), Is.True);

        this._redis.Modules.Bloom.BF_MAdd(
            key, ["Hello", "hello", "HELLO"]);

        var result = this._redis.Modules.Bloom.BF_MExists(
            key, ["Hello", "hello", "HELLO"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    /// <summary>
    /// 测试查询操作不改变过滤器的元素数量。
    /// </summary>
    [Test]
    public void BF_MExists_ReadOnly()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        var before = this._redis.Modules.Bloom.BF_Card(key);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, ["item1", "item1"]),
                Is.EqualTo([true, true]));
        }

        var after = this._redis.Modules.Bloom.BF_Card(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.EqualTo(1));
            Assert.That(after, Is.EqualTo(before));
        }
    }

    /// <summary>
    /// 测试已扩容过滤器中的元素查询。
    /// </summary>
    [Test]
    public void BF_MExists_ScalingFilter()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            this._redis.Modules.Bloom.BF_Add(
                key, $"item:{i}");
        }

        var info = this._redis.Modules.Bloom.BF_Info(key);

        Assert.That(info.Filters, Is.GreaterThan(1));

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, $"item:{i}"), Is.True);
        }

        var result = this._redis.Modules.Bloom.BF_MExists(
            key, ["item:0", "item:5", "item:10", "item:19"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true
        ]));
    }

    /// <summary>
    /// 测试不可扩容过滤器中的元素查询。
    /// </summary>
    [Test]
    public void BF_MExists_NonScalingFilter()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.000001,
                    capacity: 2,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item2"), Is.True);
        }

        var result = this._redis.Modules.Bloom.BF_MExists(
            key, ["item1", "item2", "item1"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    /// <summary>
    /// 测试 BF.MEXISTS 与 BF.EXISTS 返回结果一致。
    /// </summary>
    [Test]
    public void BF_MExists_CompareWithExists()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.000001, 1000), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item1"), Is.True);
        }

        var result = this._redis.Modules.Bloom.BF_MExists(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Has.Length.EqualTo(3));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0], Is.True);

            Assert.That(result[0], Is.EqualTo(
                this._redis.Modules.Bloom.BF_Exists(key, "item1")));

            Assert.That(result[1], Is.EqualTo(
                this._redis.Modules.Bloom.BF_Exists(key, "item2")));

            Assert.That(result[2], Is.EqualTo(
                this._redis.Modules.Bloom.BF_Exists(key, "item3")));
        }
    }

    /// <summary>
    /// 测试 CommandFlags.None。
    /// </summary>
    [Test]
    public void BF_MExists_Flags_None()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, "item1",
                flags: CommandFlags.None), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, ["item1", "item1"],
                flags: CommandFlags.None),
                Is.EqualTo([true, true]));
        }
    }

    /// <summary>
    /// 测试 CancellationToken.None。
    /// </summary>
    [Test]
    public void BF_MExists_CancellationToken_Default()
    {
        var key = $"Bloom:{{BF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, "item1",
                cancellationToken: CancellationToken.None), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_MExists(
                key, ["item1", "item1"],
                cancellationToken: CancellationToken.None),
                Is.EqualTo([true, true]));
        }
    }

    #endregion

    #region BF_Reserve

    private static void BF_ReserveRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 测试使用默认可选参数创建过滤器。
    /// </summary>
    [Test]
    public void BF_Reserve_Default()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试最小合法容量。
    /// </summary>
    [Test]
    public void BF_Reserve_MinCapacity()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.01, 1), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(1));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试自定义容量。
    /// </summary>
    [Test]
    public void BF_Reserve_CustomCapacity()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 10000), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(10000));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Size, Is.GreaterThan(0));
        }
    }

    /// <summary>
    /// 测试自定义误判率创建过滤器。
    /// </summary>
    [Test]
    public void BF_Reserve_CustomErrorRate()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.0001, 1000), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(1000));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Size, Is.GreaterThan(0));
        }
    }

    /// <summary>
    /// 测试较低误判率需要更多内存。
    /// </summary>
    [Test]
    public void BF_Reserve_ErrorRateAffectsSize()
    {
        var key1 = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";
        var key2 = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key1, 0.01, 10000), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                key2, 0.000001, 10000), Is.True);
        }

        var info1 = this._redis.Modules.Bloom.BF_Info(key1);
        var info2 = this._redis.Modules.Bloom.BF_Info(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info1.Capacity, Is.EqualTo(10000));
            Assert.That(info2.Capacity, Is.EqualTo(10000));
            Assert.That(info2.Size, Is.GreaterThan(info1.Size));
        }
    }

    /// <summary>
    /// 测试显式指定扩容因子为 1。
    /// </summary>
    [Test]
    public void BF_Reserve_Expansion_One()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100, expansion: 1), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试自定义扩容因子为 4。
    /// </summary>
    [Test]
    public void BF_Reserve_Expansion_Four()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100, expansion: 4), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Expansion, Is.EqualTo(4));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试自定义扩容因子为 8。
    /// </summary>
    [Test]
    public void BF_Reserve_Expansion_Eight()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100, expansion: 8), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        Assert.That(info.Expansion, Is.EqualTo(8));
    }

    /// <summary>
    /// 测试 NONSCALING 创建不可扩容过滤器。
    /// </summary>
    [Test]
    public void BF_Reserve_NonScaling()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100, nonscaling: true), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Null);
        }
    }

    /// <summary>
    /// 测试 EXPANSION 0 创建不可扩容过滤器。
    /// </summary>
    [Test]
    public void BF_Reserve_Expansion_Zero()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100, expansion: 0), Is.True);

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Null);
        }
    }

    /// <summary>
    /// 测试不可扩容过滤器达到容量后拒绝新增元素。
    /// </summary>
    [Test]
    public void BF_Reserve_NonScaling_Full()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Add(
                key, "item1"), Is.True);
        }

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Add(key, "item2"));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Null);
        }
    }

    /// <summary>
    /// 测试允许扩容的过滤器超过初始容量后自动扩容。
    /// </summary>
    [Test]
    public void BF_Reserve_AutoScaling()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 10; i++)
        {
            this._redis.Modules.Bloom.BF_Add(key, $"item:{i}");
        }

        var info = this._redis.Modules.Bloom.BF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Capacity, Is.GreaterThan(2));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Bloom.BF_Exists(
                key, $"item:{i}"), Is.True);
        }
    }

    /// <summary>
    /// 测试重复创建同一个过滤器会报错。
    /// </summary>
    [Test]
    public void BF_Reserve_DuplicateKey()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Reserve(
            key, 0.001, 100), Is.True);

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Reserve(
                key, 0.001, 200));

        var info = this._redis.Modules.Bloom.BF_Info(key);

        Assert.That(info.Capacity, Is.EqualTo(100));
    }

    /// <summary>
    /// 测试已经由 BF.ADD 自动创建的过滤器不能再次 RESERVE。
    /// </summary>
    [Test]
    public void BF_Reserve_KeyAlreadyCreatedByAdd()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Reserve(
                key, 0.001, 100));

        Assert.That(this._redis.Modules.Bloom.BF_Exists(
            key, "item1"), Is.True);
    }

    /// <summary>
    /// 测试 Key 已经是 Redis String 类型时不能创建过滤器。
    /// </summary>
    [Test]
    public void BF_Reserve_WrongKeyType()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Reserve(
                key, 0.001, 100));
    }

    /// <summary>
    /// 测试非法误判率：0。
    /// </summary>
    [Test]
    public void BF_Reserve_ErrorRate_Zero()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Reserve(
                key, 0, 100));
    }

    /// <summary>
    /// 测试非法误判率：负数。
    /// </summary>
    [Test]
    public void BF_Reserve_ErrorRate_Negative()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Reserve(
                key, -0.01, 100));
    }

    /// <summary>
    /// 测试非法误判率：大于 1。
    /// </summary>
    [Test]
    public void BF_Reserve_ErrorRate_GreaterThanOne()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Reserve(
                key, 1.1, 100));
    }

    /// <summary>
    /// 测试非法容量：0。
    /// </summary>
    [Test]
    public void BF_Reserve_Capacity_Zero()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Reserve(
                key, 0.001, 0));
    }

    /// <summary>
    /// 测试非法容量：负数。
    /// </summary>
    [Test]
    public void BF_Reserve_Capacity_Negative()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Reserve(
                key, 0.001, -1));
    }

    /// <summary>
    /// 测试超过 RedisBloom 支持的最大容量。
    /// </summary>
    [Test]
    public void BF_Reserve_Capacity_TooLarge()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Reserve(
                key, 0.001, 1073741825L));
    }

    /// <summary>
    /// 测试扩容因子超出允许范围。
    /// </summary>
    [Test]
    public void BF_Reserve_Expansion_TooLarge()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Reserve(
                key, 0.001, 100, expansion: 32769));
    }

    /// <summary>
    /// 测试 NONSCALING 与正数 EXPANSION 同时指定。
    /// </summary>
    [Test]
    public void BF_Reserve_NonScaling_WithExpansion()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        BF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Bloom.BF_Reserve(
                key,
                errorRate: 0.001,
                capacity: 100,
                expansion: 2,
                nonscaling: true));
    }

    /// <summary>
    /// 测试 CommandFlags.None。
    /// </summary>
    [Test]
    public void BF_Reserve_Flags_None()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.001, 100,
                    flags: CommandFlags.None), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Info(key).Capacity,
                Is.EqualTo(100));
        }
    }

    /// <summary>
    /// 测试 CancellationToken.None。
    /// </summary>
    [Test]
    public void BF_Reserve_CancellationToken_Default()
    {
        var key = $"Bloom:{{BF_Reserve}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Reserve(
                    key, 0.001, 100,
                    cancellationToken: CancellationToken.None), Is.True);

            Assert.That(this._redis.Modules.Bloom.BF_Info(key).Capacity,
                Is.EqualTo(100));
        }
    }

    #endregion

}
