using SharpRedis;
using SharpRedis.Exceptions;
using SharpRedis.Modules;

namespace NET10.Cuckoo;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class CuckooSync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region CF_Add

    private static void CF_AddRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 测试 Key 不存在时自动创建布谷鸟过滤器。
    /// </summary>
    [Test]
    public void CF_Add_AutoCreate()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "item1"), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Buckets, Is.GreaterThan(0));
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
            key, "item1"), Is.True);
    }

    /// <summary>
    /// 测试已创建的空过滤器首次添加元素。
    /// </summary>
    [Test]
    public void CF_Add_FirstInsert()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.GreaterThanOrEqualTo(1));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试 CF.ADD 允许重复插入相同元素。
    /// </summary>
    [Test]
    public void CF_Add_Duplicate()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(3));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Count(
            key, "hello"), Is.GreaterThanOrEqualTo(3));
    }

    /// <summary>
    /// 测试添加多个不同元素。
    /// </summary>
    [Test]
    public void CF_Add_MultipleItems()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(info.Items, Is.EqualTo(20));

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, $"item:{i}"), Is.True);
        }
    }

    /// <summary>
    /// 测试重复插入时 Items 持续增加。
    /// </summary>
    [Test]
    public void CF_Add_ItemsIncreaseOnDuplicate()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 1; i <= 5; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "same-item"), Is.True);

            var info = this._redis.Modules.Cuckoo.CF_Info(key);

            Assert.That(info.Items, Is.EqualTo(i));
        }
    }

    /// <summary>
    /// 测试通过 CF.ADD 添加后使用 CF.ADDNX 不重复插入。
    /// </summary>
    [Test]
    public void CF_Add_CompareWithAddNx()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.False);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试中文和 Unicode 元素。
    /// </summary>
    [Test]
    public void CF_Add_Unicode()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "你好，世界🌍"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "你好，世界🌍"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "你好，世界🌍"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试空字符串元素。
    /// </summary>
    [Test]
    public void CF_Add_EmptyItem()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, ""), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, ""), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, ""), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试包含空格、换行符、制表符的元素。
    /// </summary>
    [Test]
    public void CF_Add_SpecialCharacters()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a\nb\tc"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a:b/c?d=e"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "a\nb\tc"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "a:b/c?d=e"), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
            Is.EqualTo(3));
    }

    /// <summary>
    /// 测试不同大小写的元素。
    /// </summary>
    [Test]
    public void CF_Add_CaseSensitive()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "HELLO"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(info.Items, Is.EqualTo(3));
    }

    /// <summary>
    /// 测试已存在过滤器的配置不受 CF.ADD 影响。
    /// </summary>
    [Test]
    public void CF_Add_ReservedFilter()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key,
                    capacity: 100,
                    bucketSize: 4,
                    maxIterations: 50,
                    expansion: 2), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.BucketSize, Is.EqualTo(4));
            Assert.That(info.MaxIterations, Is.EqualTo(50));
            Assert.That(info.Expansion, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试不可扩容过滤器满时 CF.ADD 抛出 Redis 错误。
    /// </summary>
    [Test]
    public void CF_Add_NonScaling_Full()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 1,
            maxIterations: 1,
            expansion: 0), Is.True);

        int succeeded = 0;
        bool full = false;

        // Cuckoo Filter 可能在标称容量之前提前满。
        for (int i = 0; i < 100; i++)
        {
            try
            {
                Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, $"item:{i}"), Is.True);

                succeeded++;
            }
            catch (RedisErrorException)
            {
                full = true;
                break;
            }
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(full, Is.True);
            Assert.That(succeeded, Is.GreaterThan(0));
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(succeeded));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    /// <summary>
    /// 测试允许扩容时可以继续添加元素。
    /// </summary>
    [Test]
    public void CF_Add_AutoScaling()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(20));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试 Redis String Key 类型错误。
    /// </summary>
    [Test]
    public void CF_Add_WrongKeyType()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CF_AddRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"));
    }

    /// <summary>
    /// 测试 Bloom Filter Key 不能用于 CF.ADD。
    /// </summary>
    [Test]
    public void CF_Add_BloomKeyWrongType()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        CF_AddRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Add(
                key, "item2"));
    }

    /// <summary>
    /// 测试 CommandFlags.None。
    /// </summary>
    [Test]
    public void CF_Add_Flags_None()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "item1",
            flags: CommandFlags.None), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "item1",
                    flags: CommandFlags.None), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试 CancellationToken.None。
    /// </summary>
    [Test]
    public void CF_Add_CancellationToken_Default()
    {
        var key = $"Cuckoo:{{CF_Add}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "item1",
                    cancellationToken: CancellationToken.None), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(2));
        }
    }

    #endregion

    #region CF_AddNx

    private static void CF_AddNxRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 测试 Key 不存在时自动创建布谷鸟过滤器。
    /// </summary>
    [Test]
    public void CF_AddNx_AutoCreate()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "item1"), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Buckets, Is.GreaterThan(0));
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
            key, "item1"), Is.True);
    }

    /// <summary>
    /// 测试在已创建的空过滤器中首次添加元素。
    /// </summary>
    [Test]
    public void CF_AddNx_FirstInsert()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试重复添加相同元素返回 false。
    /// </summary>
    [Test]
    public void CF_AddNx_Duplicate()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "hello"), Is.False);

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "hello"), Is.False);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
        }
    }

    /// <summary>
    /// 测试重复添加不会增加元素计数。
    /// </summary>
    [Test]
    public void CF_AddNx_Duplicate_DoesNotIncreaseItems()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "item1"), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "item1"), Is.False);
        }

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before.Items, Is.EqualTo(1));
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
        }
    }

    /// <summary>
    /// 测试添加多个不同元素。
    /// Cuckoo Filter 存在假阳性，因此使用实际成功数量断言。
    /// </summary>
    [Test]
    public void CF_AddNx_MultipleDifferentItems()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        long added = 0;

        for (int i = 0; i < 20; i++)
        {
            if (this._redis.Modules.Cuckoo.CF_AddNx(
                key, $"item:{i}"))
            {
                added++;
            }
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(added, Is.GreaterThan(0));
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.EqualTo(1));
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, $"item:{i}"), Is.True);
        }
    }

    /// <summary>
    /// 测试 CF.ADD 与 CF.ADDNX 对重复元素的不同处理。
    /// </summary>
    [Test]
    public void CF_AddNx_CompareWithAdd()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.False);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(2));

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.GreaterThanOrEqualTo(2));
        }
    }

    /// <summary>
    /// 测试已有元素被删除后可以重新添加。
    /// </summary>
    [Test]
    public void CF_AddNx_AfterDelete()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.Zero);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.True);
        }
    }

    /// <summary>
    /// 测试空字符串元素。
    /// </summary>
    [Test]
    public void CF_AddNx_EmptyItem()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, ""), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                    key, ""), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, ""), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试中文和 Unicode 元素。
    /// </summary>
    [Test]
    public void CF_AddNx_Unicode()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "你好，世界🌍"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                    key, "你好，世界🌍"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "你好，世界🌍"), Is.True);
        }
    }

    /// <summary>
    /// 测试特殊字符。
    /// </summary>
    [Test]
    public void CF_AddNx_SpecialCharacters()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "a\nb\tc"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "a:b/c?d=e"), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "a b c"), Is.False);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "a\nb\tc"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "a:b/c?d=e"), Is.True);
        }
    }

    /// <summary>
    /// 测试大小写不同的元素。
    /// </summary>
    [Test]
    public void CF_AddNx_CaseSensitive()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "HELLO"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                    key, "Hello"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "HELLO"), Is.True);
        }
    }

    /// <summary>
    /// 测试已存在过滤器的配置保持不变。
    /// </summary>
    [Test]
    public void CF_AddNx_ReservedFilter()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key,
                    capacity: 100,
                    bucketSize: 4,
                    maxIterations: 50,
                    expansion: 2), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "item1"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.BucketSize, Is.EqualTo(4));
            Assert.That(info.MaxIterations, Is.EqualTo(50));
            Assert.That(info.Expansion, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试不可扩容过滤器达到容量限制时抛出异常。
    /// </summary>
    [Test]
    public void CF_AddNx_NonScaling_Full()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 1,
            maxIterations: 1,
            expansion: 0), Is.True);

        int added = 0;
        bool full = false;

        for (int i = 0; i < 1000; i++)
        {
            try
            {
                if (this._redis.Modules.Cuckoo.CF_AddNx(
                    key, $"item:{i}"))
                {
                    added++;
                }
            }
            catch (RedisErrorException)
            {
                full = true;
                break;
            }
        }

        Assert.That(full, Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(added, Is.GreaterThan(0));
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    /// <summary>
    /// 测试不可扩容过滤器满时，已存在的元素仍返回 false。
    /// </summary>
    [Test]
    public void CF_AddNx_NonScaling_ExistingItem()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key,
                    capacity: 4,
                    bucketSize: 1,
                    maxIterations: 1,
                    expansion: 0), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "hello"), Is.False);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "hello"), Is.False);

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(after.Items, Is.EqualTo(before.Items));
    }

    /// <summary>
    /// 测试过滤器允许自动扩容时的插入行为。
    /// </summary>
    [Test]
    public void CF_AddNx_AutoScaling()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        long added = 0;

        for (int i = 0; i < 20; i++)
        {
            if (this._redis.Modules.Cuckoo.CF_AddNx(
                key, $"item:{i}"))
            {
                added++;
            }
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, $"item:{i}"), Is.True);
        }
    }

    /// <summary>
    /// 测试 Redis String 类型的 Key。
    /// </summary>
    [Test]
    public void CF_AddNx_WrongKeyType()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CF_AddNxRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_AddNx(
                key, "item1"));
    }

    /// <summary>
    /// 测试 Bloom Filter 的 Key 无法用于 CF.ADDNX。
    /// </summary>
    [Test]
    public void CF_AddNx_BloomKeyWrongType()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        CF_AddNxRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_AddNx(
                key, "item1"));
    }

    /// <summary>
    /// 测试 CommandFlags.None。
    /// </summary>
    [Test]
    public void CF_AddNx_Flags_None()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "item1",
            flags: CommandFlags.None), Is.True);

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "item1",
            flags: CommandFlags.None), Is.False);
    }

    /// <summary>
    /// 测试 CancellationToken.None。
    /// </summary>
    [Test]
    public void CF_AddNx_CancellationToken_Default()
    {
        var key = $"Cuckoo:{{CF_AddNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.True);

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.False);
    }

    #endregion

    #region CF_Count

    /// <summary>
    /// 测试 Key 不存在时返回 0，且不会自动创建过滤器。
    /// </summary>
    [Test]
    public void CF_Count_KeyNotExists()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                    key, "item1"), Is.Zero);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "item2"), Is.Zero);
        }
    }

    /// <summary>
    /// 测试已创建的空过滤器，任意元素计数为 0。
    /// </summary>
    [Test]
    public void CF_Count_EmptyFilter()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "item1"), Is.Zero);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, ""), Is.Zero);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.Zero);
        }
    }

    /// <summary>
    /// 测试添加单个元素后计数至少为 1。
    /// </summary>
    [Test]
    public void CF_Count_SingleItem()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        var count = this._redis.Modules.Cuckoo.CF_Count(
            key, "hello");

        Assert.That(count, Is.GreaterThanOrEqualTo(1));
    }

    /// <summary>
    /// 测试重复插入相同元素，计数增加。
    /// </summary>
    [Test]
    public void CF_Count_DuplicateItems()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 1; i <= 5; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);

            var count = this._redis.Modules.Cuckoo.CF_Count(
                key, "hello");

            Assert.That(count, Is.GreaterThanOrEqualTo(i));
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
            Is.EqualTo(5));
    }

    /// <summary>
    /// 测试添加多个不同元素，每个元素的计数。
    /// </summary>
    [Test]
    public void CF_Count_MultipleItems()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item2"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "item2"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item3"), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "item3"), Is.True);

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "item3"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "item1"), Is.GreaterThanOrEqualTo(1));

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "item2"), Is.GreaterThanOrEqualTo(2));

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "item3"), Is.GreaterThanOrEqualTo(3));
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
            Is.EqualTo(6));
    }

    /// <summary>
    /// 测试 CF.ADDNX 对重复元素不会增加计数。
    /// </summary>
    [Test]
    public void CF_Count_AddNx_Duplicate()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.True);
        }

        var before = this._redis.Modules.Cuckoo.CF_Count(
            key, "hello");

        for (int i = 0; i < 5; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.False);
        }

        var after = this._redis.Modules.Cuckoo.CF_Count(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.GreaterThanOrEqualTo(1));
            Assert.That(after, Is.EqualTo(before));
            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试删除一次后，重复元素的计数减 1。
    /// </summary>
    [Test]
    public void CF_Count_AfterDeleteOnce()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        var before = this._redis.Modules.Cuckoo.CF_Count(
            key, "hello");

        Assert.That(this._redis.Modules.Cuckoo.CF_Del(
            key, "hello"), Is.True);

        var after = this._redis.Modules.Cuckoo.CF_Count(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.GreaterThanOrEqualTo(3));
            Assert.That(after, Is.EqualTo(before - 1));
            Assert.That(after, Is.GreaterThanOrEqualTo(2));
        }
    }

    /// <summary>
    /// 测试全部删除后计数归零。
    /// </summary>
    [Test]
    public void CF_Count_AfterDeleteAll()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                    key, "hello"), Is.Zero);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.False);
        }
    }

    /// <summary>
    /// 测试删除其他元素不影响目标元素的计数。
    /// </summary>
    [Test]
    public void CF_Count_DeleteOtherItem()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "target"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "target"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "other"), Is.True);
        }

        var before = this._redis.Modules.Cuckoo.CF_Count(
            key, "target");

        Assert.That(this._redis.Modules.Cuckoo.CF_Del(
            key, "other"), Is.True);

        var after = this._redis.Modules.Cuckoo.CF_Count(
            key, "target");

        Assert.That(after, Is.EqualTo(before));
    }

    /// <summary>
    /// 测试删除后重新添加，计数重新增加。
    /// </summary>
    [Test]
    public void CF_Count_DeleteAndReAdd()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.Zero);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.GreaterThanOrEqualTo(1));
        }
    }

    /// <summary>
    /// 测试 Unicode 中文及 Emoji 元素计数。
    /// </summary>
    [Test]
    public void CF_Count_Unicode()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "你好🌍"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "你好🌍"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "你好🌍"), Is.GreaterThanOrEqualTo(2));
        }
    }

    /// <summary>
    /// 测试空字符串元素计数。
    /// </summary>
    [Test]
    public void CF_Count_EmptyItem()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, ""), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, ""), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, ""), Is.GreaterThanOrEqualTo(2));
        }
    }

    /// <summary>
    /// 测试带空格、换行、制表符的元素。
    /// </summary>
    [Test]
    public void CF_Count_SpecialCharacters()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a\nb\tc"), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "a\nb\tc"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "a b c"), Is.GreaterThanOrEqualTo(1));

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "a\nb\tc"), Is.GreaterThanOrEqualTo(2));
        }
    }

    /// <summary>
    /// 测试大小写不同的元素具有不同的计数。
    /// </summary>
    [Test]
    public void CF_Count_CaseSensitive()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "HELLO"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "Hello"), Is.GreaterThanOrEqualTo(1));

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.GreaterThanOrEqualTo(2));

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "HELLO"), Is.GreaterThanOrEqualTo(1));
        }
    }

    /// <summary>
    /// 测试多次查询不影响过滤器统计信息。
    /// </summary>
    [Test]
    public void CF_Count_ReadOnly()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.GreaterThanOrEqualTo(1));
        }

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.DeletedItems, Is.EqualTo(before.DeletedItems));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
        }
    }

    /// <summary>
    /// 测试经过扩容后，所有已添加元素仍可以正确计数。
    /// </summary>
    [Test]
    public void CF_Count_AfterScaling()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(info.Filters, Is.GreaterThan(1));

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, $"item:{i}"), Is.GreaterThanOrEqualTo(1));
        }
    }

    /// <summary>
    /// 测试 CommandFlags.None。
    /// </summary>
    [Test]
    public void CF_Count_Flags_None()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello",
                flags: CommandFlags.None), Is.GreaterThanOrEqualTo(1));
        }
    }

    /// <summary>
    /// 测试 CancellationToken.None。
    /// </summary>
    [Test]
    public void CF_Count_CancellationToken_Default()
    {
        var key = $"Cuckoo:{{CF_Count}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello",
                cancellationToken: CancellationToken.None),
                Is.GreaterThanOrEqualTo(1));
        }
    }

    #endregion

    #region CF_Del

    private static void CF_DelRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// Key 不存在时，CF.DEL 应抛出 Redis 错误。
    /// </summary>
    [Test]
    public void CF_Del_KeyNotExists()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        CF_DelRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Del(key, "item1"));
    }

    /// <summary>
    /// 空过滤器中删除不存在的元素，返回 false。
    /// </summary>
    [Test]
    public void CF_Del_EmptyFilter()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "item1"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "item1"), Is.Zero);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 单个元素插入后删除成功。
    /// </summary>
    [Test]
    public void CF_Del_SingleItem()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.Zero);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.False);
        }
    }

    /// <summary>
    /// 删除唯一实例后，再次删除返回 false。
    /// </summary>
    [Test]
    public void CF_Del_DuplicateDelete()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                    key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.Zero);
        }
    }

    /// <summary>
    /// 重复插入三次，删除一次后仍然存在。
    /// </summary>
    [Test]
    public void CF_Del_DuplicateItem_DeleteOnce()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        var before = this._redis.Modules.Cuckoo.CF_Count(
            key, "hello");

        Assert.That(this._redis.Modules.Cuckoo.CF_Del(
            key, "hello"), Is.True);

        var after = this._redis.Modules.Cuckoo.CF_Count(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.GreaterThanOrEqualTo(3));
            Assert.That(after, Is.EqualTo(before - 1));
            Assert.That(after, Is.GreaterThanOrEqualTo(2));
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.True);
        }
    }

    /// <summary>
    /// 重复插入三次，逐次删除直到计数归零。
    /// </summary>
    [Test]
    public void CF_Del_DuplicateItem_DeleteAll()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.Zero);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.False);
        }
    }

    /// <summary>
    /// 删除一个元素后，其他已插入元素仍然存在。
    /// </summary>
    [Test]
    public void CF_Del_MultipleItems_DeleteOne()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item2"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item3"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "item2"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "item2"), Is.Zero);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item3"), Is.True);
        }
    }

    /// <summary>
    /// 删除指定元素后重新插入。
    /// </summary>
    [Test]
    public void CF_Del_DeleteAndReAdd()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.Zero);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Count(
            key, "hello"), Is.GreaterThanOrEqualTo(1));
    }

    /// <summary>
    /// 验证 CF.INFO 的删除统计。
    /// </summary>
    [Test]
    public void CF_Del_InfoDeletedItems()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 5; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                    key, "item:1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "item:3"), Is.True);
        }

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before.DeletedItems, Is.Zero);
            Assert.That(after.DeletedItems,
                Is.EqualTo(before.DeletedItems + 2));

            Assert.That(after.Filters, Is.EqualTo(before.Filters));
        }
    }

    /// <summary>
    /// 删除 Unicode 中文及 Emoji 元素。
    /// </summary>
    [Test]
    public void CF_Del_Unicode()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "你好🌍"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "你好🌍"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "你好🌍"), Is.Zero);
        }
    }

    /// <summary>
    /// 删除空字符串元素。
    /// </summary>
    [Test]
    public void CF_Del_EmptyItem()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, ""), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, ""), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, ""), Is.Zero);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Del(
            key, ""), Is.False);
    }

    /// <summary>
    /// 删除包含换行符、空格、制表符的元素。
    /// </summary>
    [Test]
    public void CF_Del_SpecialCharacters()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a\nb\tc"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "a\nb\tc"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "a\nb\tc"), Is.Zero);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "a b c"), Is.True);
        }
    }

    /// <summary>
    /// 测试大小写敏感的元素删除。
    /// </summary>
    [Test]
    public void CF_Del_CaseSensitive()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "HELLO"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.Zero);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "HELLO"), Is.True);
        }
    }

    /// <summary>
    /// 过滤器扩容后删除元素。
    /// </summary>
    [Test]
    public void CF_Del_AfterScaling()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(before.Filters, Is.GreaterThan(1));

        for (int i = 0; i < 20; i += 2)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, $"item:{i}"), Is.True);
        }

        for (int i = 1; i < 20; i += 2)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, $"item:{i}"), Is.True);
        }
    }

    /// <summary>
    /// 不可扩容的过滤器中正常删除元素。
    /// </summary>
    [Test]
    public void CF_Del_NonScaling()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key,
                    capacity: 100,
                    bucketSize: 2,
                    expansion: 0), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "item1"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "item1"), Is.Zero);
        }
    }

    /// <summary>
    /// 删除完成后，过滤器仍然存在，可以继续使用。
    /// </summary>
    [Test]
    public void CF_Del_FilterRemainsAfterDelete()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "item1"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item2"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item2"), Is.True);
        }
    }

    /// <summary>
    /// String 类型 Key 应抛出 Redis 错误。
    /// </summary>
    [Test]
    public void CF_Del_WrongKeyType()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CF_DelRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Del(key, "item1"));
    }

    /// <summary>
    /// Bloom Filter 的 Key 不可用于 CF.DEL。
    /// </summary>
    [Test]
    public void CF_Del_BloomKeyWrongType()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        CF_DelRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Del(key, "item1"));
    }

    /// <summary>
    /// 测试 CommandFlags.None。
    /// </summary>
    [Test]
    public void CF_Del_Flags_None()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "item1",
                flags: CommandFlags.None), Is.True);
        }
    }

    /// <summary>
    /// 测试 CancellationToken.None。
    /// </summary>
    [Test]
    public void CF_Del_CancellationToken_Default()
    {
        var key = $"Cuckoo:{{CF_Del}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "item1",
                cancellationToken: CancellationToken.None), Is.True);
        }
    }

    #endregion

    #region CF_Exists

    /// <summary>
    /// Key 不存在时返回 false。
    /// </summary>
    [Test]
    public void CF_Exists_KeyNotExists()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                    key, "item1"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, ""), Is.False);
        }
    }

    /// <summary>
    /// 空过滤器查询不存在的元素返回 false。
    /// </summary>
    [Test]
    public void CF_Exists_EmptyFilter()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item1"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item2"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.Zero);
        }
    }

    /// <summary>
    /// CF.ADD 自动创建过滤器后查询已添加元素。
    /// </summary>
    [Test]
    public void CF_Exists_AutoCreate()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                    key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
            key, "hello"), Is.True);
    }

    /// <summary>
    /// 已创建过滤器中添加单个元素后查询。
    /// </summary>
    [Test]
    public void CF_Exists_SingleItem()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item1"), Is.True);
        }
    }

    /// <summary>
    /// 查询多个已经添加的元素。
    /// </summary>
    [Test]
    public void CF_Exists_MultipleItems()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, $"item:{i}"), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
            Is.EqualTo(20));
    }

    /// <summary>
    /// 重复插入相同元素后仍然存在。
    /// </summary>
    [Test]
    public void CF_Exists_DuplicateItems()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 5; i++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                            key, "hello"), Is.True);

                Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                    key, "hello"), Is.True);
            }
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                    Is.EqualTo(5));

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.GreaterThanOrEqualTo(5));
        }
    }

    /// <summary>
    /// CF.ADDNX 添加成功后，CF.EXISTS 返回 true。
    /// </summary>
    [Test]
    public void CF_Exists_AfterAddNx()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                    key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.True);
        }
    }

    /// <summary>
    /// 删除唯一实例后查询返回 false。
    /// </summary>
    [Test]
    public void CF_Exists_AfterDelete()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                    key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.Zero);
        }
    }

    /// <summary>
    /// 重复插入后删除一次，元素仍然存在。
    /// </summary>
    [Test]
    public void CF_Exists_Duplicate_DeleteOnce()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.GreaterThanOrEqualTo(2));
        }
    }

    /// <summary>
    /// 重复插入三次并全部删除后，元素不再存在。
    /// </summary>
    [Test]
    public void CF_Exists_Duplicate_DeleteAll()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                    key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.Zero);
        }
    }

    /// <summary>
    /// 删除后重新添加，元素恢复存在。
    /// </summary>
    [Test]
    public void CF_Exists_DeleteAndReAdd()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.False);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.True);
        }
    }

    /// <summary>
    /// 中文和 Unicode 元素查询。
    /// </summary>
    [Test]
    public void CF_Exists_Unicode()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "你好，世界🌍"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "你好，世界🌍"), Is.True);
        }
    }

    /// <summary>
    /// 空字符串元素查询。
    /// </summary>
    [Test]
    public void CF_Exists_EmptyItem()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, ""), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, ""), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
            key, ""), Is.True);
    }

    /// <summary>
    /// 查询包含空格、换行、制表符及特殊符号的元素。
    /// </summary>
    [Test]
    public void CF_Exists_SpecialCharacters()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a\nb\tc"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a:b/c?d=e"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "a\nb\tc"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "a:b/c?d=e"), Is.True);
        }
    }

    /// <summary>
    /// 大小写不同的元素查询。
    /// </summary>
    [Test]
    public void CF_Exists_CaseSensitive()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "HELLO"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "HELLO"), Is.True);
        }
    }

    /// <summary>
    /// 不同 Key 下的过滤器数据相互隔离。
    /// </summary>
    [Test]
    public void CF_Exists_DifferentKeys()
    {
        var key1 = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";
        var key2 = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key1, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                key2, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key1, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key2, "item2"), Is.True);
        }

        // 使用足够大的过滤器降低不同 Key 之间的假阳性概率。
        // 已添加元素必须始终可以被查询到。
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key1, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key2, "item2"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key1).Items,
                Is.EqualTo(1));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key2).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 查询命令不会改变过滤器的统计信息。
    /// </summary>
    [Test]
    public void CF_Exists_ReadOnly()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "hello"), Is.True);
        }

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.DeletedItems, Is.EqualTo(before.DeletedItems));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
            Assert.That(after.Size, Is.EqualTo(before.Size));
        }
    }

    /// <summary>
    /// 过滤器扩容后查询已添加的元素。
    /// </summary>
    [Test]
    public void CF_Exists_AfterScaling()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, capacity: 4, bucketSize: 2, expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(info.Filters, Is.GreaterThan(1));

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, $"item:{i}"), Is.True);
        }
    }

    /// <summary>
    /// 不可扩容过滤器中查询已添加的元素。
    /// </summary>
    [Test]
    public void CF_Exists_NonScaling()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, capacity: 100, bucketSize: 2, expansion: 0), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item2"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item2"), Is.True);
        }
    }

    /// <summary>
    /// String 类型 Key 的查询行为。
    /// 按 Redis 官方 CF.EXISTS 返回值说明，错误类型返回 false。
    /// </summary>
    [Test]
    public void CF_Exists_WrongKeyType()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
            key, "item1"), Is.False);
    }

    /// <summary>
    /// Bloom Filter Key 的查询行为。
    /// </summary>
    [Test]
    public void CF_Exists_BloomKeyWrongType()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item1"), Is.False);
        }
    }

    /// <summary>
    /// 显式传入 CommandFlags.None。
    /// </summary>
    [Test]
    public void CF_Exists_Flags_None()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item1",
                flags: CommandFlags.None), Is.True);
        }
    }

    /// <summary>
    /// 显式传入 CancellationToken.None。
    /// </summary>
    [Test]
    public void CF_Exists_CancellationToken_Default()
    {
        var key = $"Cuckoo:{{CF_Exists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item1",
                cancellationToken: CancellationToken.None), Is.True);
        }
    }

    #endregion

    #region CF_Info

    private static void CF_InfoRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 测试默认参数创建的空过滤器，验证全部 8 个属性。
    /// </summary>
    [Test]
    public void CF_Info_Default()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Buckets, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.BucketSize, Is.EqualTo(2));
            Assert.That(info.Expansion, Is.EqualTo(1));
            Assert.That(info.MaxIterations, Is.EqualTo(20));
        }
    }

    /// <summary>
    /// 测试 CF.ADD 自动创建过滤器后的默认配置。
    /// </summary>
    [Test]
    public void CF_Info_AutoCreated()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "item1"), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Buckets, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.BucketSize, Is.EqualTo(2));
            Assert.That(info.Expansion, Is.EqualTo(1));
            Assert.That(info.MaxIterations, Is.EqualTo(20));
        }
    }

    /// <summary>
    /// 测试自定义全部可选参数。
    /// </summary>
    [Test]
    public void CF_Info_CustomOptions()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 1000,
            bucketSize: 4,
            maxIterations: 50,
            expansion: 4), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Buckets, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.BucketSize, Is.EqualTo(4));
            Assert.That(info.Expansion, Is.EqualTo(4));
            Assert.That(info.MaxIterations, Is.EqualTo(50));
        }
    }

    /// <summary>
    /// 测试不同 BUCKETSIZE 配置返回正确值。
    /// </summary>
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(8)]
    [TestCase(16)]
    public void CF_Info_BucketSize(int bucketSize)
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1024, bucketSize: bucketSize), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.BucketSize, Is.EqualTo(bucketSize));
            Assert.That(info.Buckets, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试不同 MAXITERATIONS 配置。
    /// </summary>
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(50)]
    [TestCase(100)]
    [TestCase(65535)]
    public void CF_Info_MaxIterations(int maxIterations)
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100,
            maxIterations: maxIterations), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(info.MaxIterations, Is.EqualTo(maxIterations));
    }

    /// <summary>
    /// 测试不同 EXPANSION 配置。
    /// </summary>
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(8)]
    public void CF_Info_Expansion(int expansion)
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100,
            expansion: expansion), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            if (expansion == 0)
            {
                Assert.That(info.Expansion, Is.Zero);
            }
            else
            {
                Assert.That(info.Expansion, Is.EqualTo(expansion));
            }

            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试非 2 的幂的 EXPANSION 会向上取整。
    /// </summary>
    [TestCase(3, 4)]
    [TestCase(5, 8)]
    [TestCase(7, 8)]
    public void CF_Info_Expansion_Rounded(int expansion, int expected)
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100,
            expansion: expansion), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(info.Expansion, Is.EqualTo(expected));
    }

    /// <summary>
    /// 测试刚创建过滤器时插入与删除统计均为 0。
    /// </summary>
    [Test]
    public void CF_Info_EmptyFilter()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Size, Is.GreaterThan(0));
        }
    }

    /// <summary>
    /// 测试插入单个元素后的统计信息。
    /// </summary>
    [Test]
    public void CF_Info_AfterSingleAdd()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试多个不同元素插入后的统计信息。
    /// </summary>
    [Test]
    public void CF_Info_AfterMultipleAdds()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(20));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试重复插入相同元素后 Items 增加。
    /// </summary>
    [Test]
    public void CF_Info_DuplicateItems()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 5; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(5));
            Assert.That(info.DeletedItems, Is.Zero);
        }
    }

    /// <summary>
    /// 测试 CF.ADDNX 重复插入不增加 Items。
    /// </summary>
    [Test]
    public void CF_Info_AddNx_Duplicate()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
                key, "hello"), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_AddNx(
            key, "hello"), Is.False);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
        }
    }

    /// <summary>
    /// 测试成功删除后 DeletedItems 增加。
    /// </summary>
    [Test]
    public void CF_Info_AfterDelete()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item2"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item3"), Is.True);
        }

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(this._redis.Modules.Cuckoo.CF_Del(
            key, "item2"), Is.True);

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before.Items, Is.EqualTo(3));
            Assert.That(before.DeletedItems, Is.Zero);
            Assert.That(after.DeletedItems, Is.EqualTo(1));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
        }
    }

    /// <summary>
    /// 测试多次删除时 DeletedItems 累积增加。
    /// </summary>
    [Test]
    public void CF_Info_MultipleDeletes()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 5; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                    key, "item:1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "item:3"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(info.DeletedItems, Is.EqualTo(2));
    }

    /// <summary>
    /// 测试删除后重新添加的统计信息。
    /// </summary>
    [Test]
    public void CF_Info_DeleteAndReAdd()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);
        }

        var deleted = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted.DeletedItems, Is.EqualTo(1));

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        var readded = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(readded.Items, Is.GreaterThan(0));
            Assert.That(readded.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试自动扩容后 Filters 和 Buckets 增加。
    /// </summary>
    [Test]
    public void CF_Info_AfterScaling()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        for (int i = 0; i < 40; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Filters, Is.GreaterThanOrEqualTo(before.Filters));
            Assert.That(after.Buckets, Is.GreaterThanOrEqualTo(before.Buckets));
            Assert.That(after.Size, Is.GreaterThanOrEqualTo(before.Size));
            Assert.That(after.Items, Is.EqualTo(40));
            Assert.That(after.Expansion, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试不允许扩容时 Expansion 为 0。
    /// </summary>
    [Test]
    public void CF_Info_NonScaling()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key,
                    capacity: 100,
                    bucketSize: 2,
                    expansion: 0), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
            Assert.That(info.Items, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试重复读取不会改变统计信息。
    /// </summary>
    [Test]
    public void CF_Info_ReadOnly()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        for (int i = 0; i < 10; i++)
        {
            var current = this._redis.Modules.Cuckoo.CF_Info(key);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(current.Items, Is.EqualTo(before.Items));
                Assert.That(current.DeletedItems,
                    Is.EqualTo(before.DeletedItems));
            }
        }

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Size, Is.EqualTo(before.Size));
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.DeletedItems, Is.EqualTo(before.DeletedItems));
            Assert.That(after.BucketSize, Is.EqualTo(before.BucketSize));
            Assert.That(after.Expansion, Is.EqualTo(before.Expansion));
            Assert.That(after.MaxIterations, Is.EqualTo(before.MaxIterations));
        }
    }

    /// <summary>
    /// Key 不存在时应抛出 Redis 错误。
    /// </summary>
    [Test]
    public void CF_Info_KeyNotExists()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        CF_InfoRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Info(key));
    }

    /// <summary>
    /// String 类型的 Key 应抛出 Redis 错误。
    /// </summary>
    [Test]
    public void CF_Info_WrongKeyType()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CF_InfoRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Info(key));
    }

    /// <summary>
    /// Bloom Filter 类型的 Key 应抛出 Redis 错误。
    /// </summary>
    [Test]
    public void CF_Info_BloomKeyWrongType()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "hello"), Is.True);

        CF_InfoRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Info(key));
    }

    /// <summary>
    /// 测试 CommandFlags.None。
    /// </summary>
    [Test]
    public void CF_Info_Flags_None()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(
            key, flags: CommandFlags.None);

        Assert.That(info.Filters, Is.EqualTo(1));
    }

    /// <summary>
    /// 测试 CancellationToken.None。
    /// </summary>
    [Test]
    public void CF_Info_CancellationToken_Default()
    {
        var key = $"Cuckoo:{{CF_Info}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(
            key, cancellationToken: CancellationToken.None);

        Assert.That(info.Filters, Is.EqualTo(1));
    }

    #endregion

    #region CF_Insert

    private static void CF_InsertRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 单元素插入，不存在的 Key 自动创建过滤器。
    /// </summary>
    [Test]
    public void CF_Insert_Single_AutoCreate()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
            key, "item1"), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
            key, "item1"), Is.True);
    }

    /// <summary>
    /// 多元素插入，不存在的 Key 自动创建过滤器。
    /// </summary>
    [Test]
    public void CF_Insert_Multiple_AutoCreate()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(3));

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item2"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item3"), Is.True);
        }
    }

    /// <summary>
    /// 在已有空过滤器中插入单个元素。
    /// </summary>
    [Test]
    public void CF_Insert_Single_ExistingFilter()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.GreaterThanOrEqualTo(1));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 在已有过滤器中批量插入多个元素。
    /// </summary>
    [Test]
    public void CF_Insert_Multiple_ExistingFilter()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["a", "b", "c", "d", "e"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                true, true, true, true, true
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(5));
        }
    }

    /// <summary>
    /// 单元素重复插入，每次成功都返回 true。
    /// </summary>
    [Test]
    public void CF_Insert_Single_Duplicate()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                    Is.EqualTo(3));

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.GreaterThanOrEqualTo(3));
        }
    }

    /// <summary>
    /// 批量插入包含重复元素，允许重复插入。
    /// </summary>
    [Test]
    public void CF_Insert_Multiple_Duplicate()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["hello", "hello", "world", "hello"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                true, true, true, true
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(4));

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.GreaterThanOrEqualTo(3));
        }
    }

    /// <summary>
    /// 单元素与集合重载配合使用。
    /// </summary>
    [Test]
    public void CF_Insert_SingleAndMultiple()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "a"), Is.True);
        }

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["b", "c", "a"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                true, true, true
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(4));
        }
    }

    /// <summary>
    /// 使用自定义 CAPACITY 自动创建过滤器。
    /// </summary>
    [Test]
    public void CF_Insert_Single_CustomCapacity()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
            key, "item1", capacity: 1000), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Buckets, Is.GreaterThan(0));
        }
    }

    /// <summary>
    /// 批量插入并指定自定义 CAPACITY。
    /// </summary>
    [Test]
    public void CF_Insert_Multiple_CustomCapacity()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key,
            ["item1", "item2", "item3"],
            capacity: 1000);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(3));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Buckets, Is.GreaterThan(0));
        }
    }

    /// <summary>
    /// Key 已存在时，合法的 CAPACITY 参数不会重新创建过滤器。
    /// </summary>
    [Test]
    public void CF_Insert_Capacity_IgnoredForExistingFilter()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100, bucketSize: 4), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["a", "b"], capacity: 1000);

        Assert.That(result, Is.EqualTo([true, true]));

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.BucketSize, Is.EqualTo(4));
            Assert.That(after.Filters, Is.EqualTo(1));
            Assert.That(after.Items, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// NOCREATE=true 且 Key 不存在，单元素插入抛出错误。
    /// </summary>
    [Test]
    public void CF_Insert_Single_NoCreate_KeyNotExists()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        CF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Insert(
                key, "item1", nocreate: true));
    }

    /// <summary>
    /// NOCREATE=true 且 Key 不存在，批量插入抛出错误。
    /// </summary>
    [Test]
    public void CF_Insert_Multiple_NoCreate_KeyNotExists()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        CF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Insert(
                key, ["item1", "item2"], nocreate: true));
    }

    /// <summary>
    /// NOCREATE=true 且 Key 已存在，单元素插入成功。
    /// </summary>
    [Test]
    public void CF_Insert_Single_NoCreate_ExistingFilter()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "item1", nocreate: true), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// NOCREATE=true 且 Key 已存在，批量插入成功。
    /// </summary>
    [Test]
    public void CF_Insert_Multiple_NoCreate_ExistingFilter()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["item1", "item2", "item3"], nocreate: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                true, true, true
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(3));
        }
    }

    /// <summary>
    /// 不可扩容过滤器已满时，单元素重载返回 false。
    /// 同时验证 Redis 原始 -1 不应转换为 true。
    /// </summary>
    [Test]
    public void CF_Insert_Single_FilterFull()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key,
                    capacity: 2,
                    bucketSize: 1,
                    expansion: 0), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "same1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "same2"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "same3"), Is.False);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    /// <summary>
    /// 批量插入超过容量时，按顺序返回 true/false。
    /// </summary>
    [Test]
    public void CF_Insert_Multiple_FilterFull()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 2,
            bucketSize: 1,
            expansion: 0), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["same1", "same2", "same3", "same4"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, false, false
        ]));

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    /// <summary>
    /// 批量插入部分成功后，再次插入时继续返回失败状态。
    /// </summary>
    [Test]
    public void CF_Insert_Multiple_PartialSuccess()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key,
                    capacity: 2,
                    bucketSize: 1,
                    expansion: 0), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "same1"), Is.True);
        }

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["same2", "same3", "same4"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                true, false, false
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 过滤器容量已满时，删除一个实例后可再次插入。
    /// </summary>
    [Test]
    public void CF_Insert_FilterFull_AfterDelete()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key,
                    capacity: 2,
                    bucketSize: 1,
                    expansion: 0), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "same1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "same2"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "same3"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "same1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, "same4"), Is.True);
        }
    }

    /// <summary>
    /// 已存在的过滤器可以随着插入数量增加而自动扩容。
    /// </summary>
    [Test]
    public void CF_Insert_AutoScaling()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        for (int i = 0; i < 30; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
                key, $"item:{i}"), Is.True);
        }

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Filters, Is.GreaterThan(before.Filters));
            Assert.That(after.Items, Is.EqualTo(30));
            Assert.That(after.Expansion, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// Unicode、空字符串及特殊字符批量插入。
    /// </summary>
    [Test]
    public void CF_Insert_Multiple_SpecialItems()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["", "你好🌍", "a b c", "a\nb\tc", "a:b/c"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                true, true, true, true, true
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(5));
        }
    }

    /// <summary>
    /// 大小写不同的元素均可以插入。
    /// </summary>
    [Test]
    public void CF_Insert_Multiple_CaseSensitive()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["Hello", "hello", "HELLO"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                true, true, true
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(3));
        }
    }

    /// <summary>
    /// CAPACITY 为 0，不允许自动创建过滤器。
    /// </summary>
    [Test]
    public void CF_Insert_Capacity_Zero()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        CF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Insert(
                key, "item1", capacity: 0));
    }

    /// <summary>
    /// CAPACITY 为负数，返回 Redis 错误。
    /// </summary>
    [Test]
    public void CF_Insert_Capacity_Negative()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        CF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Insert(
                key, ["item1", "item2"], capacity: -1));
    }

    /// <summary>
    /// CAPACITY 超出 RedisBloom 最大初始容量。
    /// </summary>
    [Test]
    public void CF_Insert_Capacity_TooLarge()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        CF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Insert(
                key, "item1", capacity: 1073741825));
    }

    /// <summary>
    /// Redis String 类型的 Key 不能用于 CF.INSERT。
    /// </summary>
    [Test]
    public void CF_Insert_WrongKeyType()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Insert(
                key, "item1"));

        CF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Insert(
                key, ["item1", "item2"]));
    }

    /// <summary>
    /// Bloom Filter 类型的 Key 不能用于 CF.INSERT。
    /// </summary>
    [Test]
    public void CF_Insert_BloomKeyWrongType()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        CF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Insert(
                key, "item2"));

        CF_InsertRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Insert(
                key, ["item2", "item3"]));
    }

    /// <summary>
    /// 显式使用 CommandFlags.None。
    /// </summary>
    [Test]
    public void CF_Insert_Flags_None()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
            key, "item1",
            flags: CommandFlags.None), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["item1", "item2"],
            flags: CommandFlags.None);

        Assert.That(result, Is.EqualTo([true, true]));
    }

    /// <summary>
    /// 显式使用 CancellationToken.None。
    /// </summary>
    [Test]
    public void CF_Insert_CancellationToken_Default()
    {
        var key = $"Cuckoo:{{CF_Insert}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Insert(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_Insert(
            key, ["item1", "item2"],
            cancellationToken: CancellationToken.None);

        Assert.That(result, Is.EqualTo([true, true]));
    }

    #endregion

    #region CF_InsertNx

    private static void CF_InsertNxRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 单元素插入，不存在的 Key 自动创建过滤器。
    /// </summary>
    [Test]
    public void CF_InsertNx_Single_AutoCreate()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, "item1");

        Assert.That(result, Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
            key, "item1"), Is.True);
    }

    /// <summary>
    /// 批量插入，不存在的 Key 自动创建过滤器。
    /// </summary>
    [Test]
    public void CF_InsertNx_Multiple_AutoCreate()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added
        ]));

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(info.Items, Is.EqualTo(3));
    }

    /// <summary>
    /// 单元素在已有空过滤器中首次插入。
    /// </summary>
    [Test]
    public void CF_InsertNx_Single_ExistingFilter()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
                key, "hello"),
                Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 批量在已有过滤器中插入多个元素。
    /// </summary>
    [Test]
    public void CF_InsertNx_Multiple_ExistingFilter()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["a", "b", "c"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Added
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(3));
        }
    }

    /// <summary>
    /// 单元素重复插入返回 Exists，不增加元素数量。
    /// </summary>
    [Test]
    public void CF_InsertNx_Single_Duplicate()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
            key, "hello"),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        for (int i = 0; i < 5; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
                key, "hello"),
                Is.EqualTo(RedisCuckoo.InsertNxStatus.Exists));
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
            Is.EqualTo(1));
    }

    /// <summary>
    /// 同一批次插入重复元素。
    /// </summary>
    [Test]
    public void CF_InsertNx_Multiple_Duplicate()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["hello", "hello", "hello"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Exists,
                RedisCuckoo.InsertNxStatus.Exists
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 部分元素已存在，批量插入返回混合状态。
    /// </summary>
    [Test]
    public void CF_InsertNx_Multiple_MixedStatus()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "c"), Is.True);
        }

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["a", "b", "c", "d", "a"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisCuckoo.InsertNxStatus.Exists,
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Exists,
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Exists
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(4));
        }
    }

    /// <summary>
    /// 单元素与数组重载交替使用。
    /// </summary>
    [Test]
    public void CF_InsertNx_SingleAndMultiple()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
            key, "a"),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["a", "b", "b"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisCuckoo.InsertNxStatus.Exists,
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Exists
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
                key, "b"),
                Is.EqualTo(RedisCuckoo.InsertNxStatus.Exists));
        }
    }

    /// <summary>
    /// 单元素指定 CAPACITY 自动创建过滤器。
    /// </summary>
    [Test]
    public void CF_InsertNx_Single_CustomCapacity()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
            key, "item1", capacity: 1000),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Buckets, Is.GreaterThan(0));
        }
    }

    /// <summary>
    /// 批量指定 CAPACITY 自动创建过滤器。
    /// </summary>
    [Test]
    public void CF_InsertNx_Multiple_CustomCapacity()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["a", "b", "c"], capacity: 1000);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Added
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(3));
        }
    }

    /// <summary>
    /// 已存在过滤器时，CAPACITY 不改变原有配置。
    /// </summary>
    [Test]
    public void CF_InsertNx_Capacity_IgnoredForExistingFilter()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100, bucketSize: 4), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
            key, "item1", capacity: 1000),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.BucketSize, Is.EqualTo(4));
            Assert.That(after.Filters, Is.EqualTo(1));
            Assert.That(after.Items, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// NOCREATE=true，Key 不存在，单元素抛出异常。
    /// </summary>
    [Test]
    public void CF_InsertNx_Single_NoCreate_KeyNotExists()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        CF_InsertNxRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_InsertNx(
                key, "item1", nocreate: true));
    }

    /// <summary>
    /// NOCREATE=true，Key 不存在，批量抛出异常。
    /// </summary>
    [Test]
    public void CF_InsertNx_Multiple_NoCreate_KeyNotExists()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        CF_InsertNxRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_InsertNx(
                key, ["a", "b"], nocreate: true));
    }

    /// <summary>
    /// NOCREATE=true，Key 存在，单元素正常插入。
    /// </summary>
    [Test]
    public void CF_InsertNx_Single_NoCreate_ExistingFilter()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
                key, "item1", nocreate: true),
                Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
            key, "item1", nocreate: true),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Exists));
    }

    /// <summary>
    /// NOCREATE=true，Key 存在，批量正常插入。
    /// </summary>
    [Test]
    public void CF_InsertNx_Multiple_NoCreate_ExistingFilter()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["a", "a", "b"], nocreate: true);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Exists,
            RedisCuckoo.InsertNxStatus.Added
        ]));
    }

    /// <summary>
    /// 不可扩容过滤器已满时，验证单元素重载能解析 Full 状态。
    /// </summary>
    [Test]
    public void CF_InsertNx_Single_FilterFull()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 1,
            maxIterations: 1,
            expansion: 0), Is.True);

        int added = 0;
        bool foundFull = false;

        for (int i = 0; i < 1000; i++)
        {
            var status = this._redis.Modules.Cuckoo.CF_InsertNx(
                key, $"item:{i}");

            if (status == RedisCuckoo.InsertNxStatus.Added)
            {
                added++;
            }
            else if (status == RedisCuckoo.InsertNxStatus.Full)
            {
                foundFull = true;
                break;
            }
            else
            {
                Assert.That(status,
                    Is.EqualTo(RedisCuckoo.InsertNxStatus.Exists));
            }
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(foundFull, Is.True);
            Assert.That(added, Is.GreaterThan(0));
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    /// <summary>
    /// 批量插入达到容量限制，验证 Added/Exists/Full 数组。
    /// 不依赖具体哪一个新元素触发 Full。
    /// </summary>
    [Test]
    public void CF_InsertNx_Multiple_FilterFull()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 1,
            maxIterations: 1,
            expansion: 0), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key,
            [
                "item:0", "item:1", "item:2", "item:3",
            "item:4", "item:5", "item:6", "item:7",
            "item:8", "item:9", "item:10", "item:11"
            ]);

        Assert.That(result, Has.Length.EqualTo(12));

        int added = 0;
        int full = 0;

        foreach (var status in result)
        {
            switch (status)
            {
                case RedisCuckoo.InsertNxStatus.Added:
                    added++;
                    break;

                case RedisCuckoo.InsertNxStatus.Full:
                    full++;
                    break;

                case RedisCuckoo.InsertNxStatus.Exists:
                    break;

                default:
                    Assert.Fail($"Unexpected CF.INSERTNX status: {status}");
                    break;
            }
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(added, Is.GreaterThan(0));
            Assert.That(full, Is.GreaterThan(0));
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    /// <summary>
    /// 已存在元素即使在过滤器满时仍返回 Exists。
    /// </summary>
    [Test]
    public void CF_InsertNx_FilterFull_ExistingItem()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key,
                    capacity: 4,
                    bucketSize: 1,
                    maxIterations: 1,
                    expansion: 0), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
                key, "known"),
                Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));
        }

        for (int i = 0; i < 1000; i++)
        {
            var status = this._redis.Modules.Cuckoo.CF_InsertNx(
                key, $"other:{i}");

            if (status == RedisCuckoo.InsertNxStatus.Full)
                break;
        }

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
            key, "known"),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Exists));

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(after.Items, Is.EqualTo(before.Items));
    }

    /// <summary>
    /// 批量插入重复元素，返回顺序与输入顺序对应。
    /// </summary>
    [Test]
    public void CF_InsertNx_Multiple_Order()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["x", "x", "y", "y", "x"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Exists,
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Exists,
            RedisCuckoo.InsertNxStatus.Exists
        ]));
    }

    /// <summary>
    /// 自动扩容时大量插入，统计实际成功数量。
    /// </summary>
    [Test]
    public void CF_InsertNx_AutoScaling()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        long added = 0;

        for (int i = 0; i < 50; i++)
        {
            var status = this._redis.Modules.Cuckoo.CF_InsertNx(
                key, $"item:{i}");

            Assert.That(status, Is.Not.EqualTo(
                RedisCuckoo.InsertNxStatus.Full));

            if (status == RedisCuckoo.InsertNxStatus.Added)
                added++;
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 删除已有元素后，可以使用 CF.INSERTNX 重新插入。
    /// </summary>
    [Test]
    public void CF_InsertNx_AfterDelete()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
                    key, "hello"),
                    Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.Zero);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
            key, "hello"),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));
    }

    /// <summary>
    /// 中文、Unicode、空字符串、特殊字符。
    /// </summary>
    [Test]
    public void CF_InsertNx_Multiple_SpecialItems()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["", "你好🌍", "a b c", "a\nb\tc"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Added
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(4));
        }
    }

    /// <summary>
    /// 大小写不同的元素分别插入。
    /// </summary>
    [Test]
    public void CF_InsertNx_CaseSensitive()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["Hello", "hello", "HELLO"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added
        ]));
    }

    /// <summary>
    /// CAPACITY 为 0。
    /// </summary>
    [Test]
    public void CF_InsertNx_Capacity_Zero()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        CF_InsertNxRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_InsertNx(
                key, "item1", capacity: 0));
    }

    /// <summary>
    /// CAPACITY 为负数。
    /// </summary>
    [Test]
    public void CF_InsertNx_Capacity_Negative()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        CF_InsertNxRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_InsertNx(
                key, ["a", "b"], capacity: -1));
    }

    /// <summary>
    /// CAPACITY 超过最大初始容量。
    /// </summary>
    [Test]
    public void CF_InsertNx_Capacity_TooLarge()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        CF_InsertNxRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_InsertNx(
                key, "item1", capacity: 1073741825));
    }

    /// <summary>
    /// String 类型 Key 应抛出异常。
    /// </summary>
    [Test]
    public void CF_InsertNx_WrongKeyType()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CF_InsertNxRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_InsertNx(
                key, "item1"));

        CF_InsertNxRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_InsertNx(
                key, ["a", "b"]));
    }

    /// <summary>
    /// Bloom Filter 类型 Key 应抛出异常。
    /// </summary>
    [Test]
    public void CF_InsertNx_BloomKeyWrongType()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "item1"), Is.True);

        CF_InsertNxRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_InsertNx(
                key, "item2"));

        CF_InsertNxRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_InsertNx(
                key, ["a", "b"]));
    }

    /// <summary>
    /// CommandFlags.None，单元素和数组重载。
    /// </summary>
    [Test]
    public void CF_InsertNx_Flags_None()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
            key, "hello",
            flags: CommandFlags.None),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["hello", "hello"],
            flags: CommandFlags.None);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Exists,
            RedisCuckoo.InsertNxStatus.Exists
        ]));
    }

    /// <summary>
    /// CancellationToken.None，单元素和数组重载。
    /// </summary>
    [Test]
    public void CF_InsertNx_CancellationToken_Default()
    {
        var key = $"Cuckoo:{{CF_InsertNx}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_InsertNx(
            key, "hello",
            cancellationToken: CancellationToken.None),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var result = this._redis.Modules.Cuckoo.CF_InsertNx(
            key, ["hello", "hello"],
            cancellationToken: CancellationToken.None);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Exists,
            RedisCuckoo.InsertNxStatus.Exists
        ]));
    }

    #endregion

    #region CF_ScanDump_LoadChunk

    private static void CF_ScanDump_LoadChunkRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 完整导出 Cuckoo Filter。
    /// 只保留有效的数据块，不保留最终的 (0, nil)。
    /// </summary>
    private List<(long Iterator, byte[] Data)> CF_ScanDumpAll(string key)
    {
        var chunks = new List<(long Iterator, byte[] Data)>();
        long iterator = 0;

        for (int i = 0; i < 1024; i++)
        {
            var result = this._redis.Modules.Cuckoo.CF_ScanDump(
                key, iterator);

            if (result.IsCompleted)
            {
                Assert.That(result.Iterator, Is.Zero);
                return chunks;
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Iterator, Is.GreaterThan(0));
                Assert.That(result.Data, Is.Not.Null.And.Not.Empty);
            }

            chunks.Add((result.Iterator, result.Data!));
            iterator = result.Iterator;
        }

        Assert.Fail("CF.SCANDUMP did not complete within 1024 iterations.");
        return chunks;
    }

    /// <summary>
    /// 将导出的所有数据块恢复为目标 Cuckoo Filter。
    /// </summary>
    private void CF_LoadAllChunks(
        string key,
        List<(long Iterator, byte[] Data)> chunks)
    {
        foreach (var (Iterator, Data) in chunks)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_LoadChunk(
                key, Iterator, Data), Is.True);
        }
    }

    /// <summary>
    /// 测试单元素过滤器的完整导出、恢复。
    /// </summary>
    [Test]
    public void CF_ScanDump_LoadChunk_SingleItem()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            source, "hello"), Is.True);

        var chunks = this.CF_ScanDumpAll(source);

        Assert.That(chunks, Has.Count.GreaterThanOrEqualTo(2));

        this.CF_LoadAllChunks(target, chunks);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                source, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                target, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                target, "hello"), Is.GreaterThanOrEqualTo(1));
        }
    }

    /// <summary>
    /// 测试重复元素的计数在恢复后保持一致。
    /// </summary>
    [Test]
    public void CF_ScanDump_LoadChunk_DuplicateItems()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            source, 1000), Is.True);

        for (int i = 0; i < 5; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, "hello"), Is.True);
        }

        var beforeCount = this._redis.Modules.Cuckoo.CF_Count(
            source, "hello");

        var chunks = this.CF_ScanDumpAll(source);
        this.CF_LoadAllChunks(target, chunks);

        var afterCount = this._redis.Modules.Cuckoo.CF_Count(
            target, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(beforeCount, Is.GreaterThanOrEqualTo(5));
            Assert.That(afterCount, Is.EqualTo(beforeCount));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(target).Items,
                Is.EqualTo(5));
        }
    }

    /// <summary>
    /// 测试多个不同元素和重复元素混合恢复。
    /// </summary>
    [Test]
    public void CF_ScanDump_LoadChunk_MultipleItems()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    source, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, "a"), Is.True);
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, "b"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    source, "b"), Is.True);
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, "c"), Is.True);
        }
        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            source, "c"), Is.True);
        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            source, "c"), Is.True);

        var chunks = this.CF_ScanDumpAll(source);
        this.CF_LoadAllChunks(target, chunks);

        foreach (var item in new[] { "a", "b", "c" })
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                target, item),
                Is.EqualTo(this._redis.Modules.Cuckoo.CF_Count(
                    source, item)));
        }

        var before = this._redis.Modules.Cuckoo.CF_Info(source);
        var after = this._redis.Modules.Cuckoo.CF_Info(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.DeletedItems, Is.EqualTo(before.DeletedItems));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
        }
    }

    /// <summary>
    /// 测试自定义配置在导出恢复后保持一致。
    /// </summary>
    [Test]
    public void CF_ScanDump_LoadChunk_CustomOptions()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    source,
                    capacity: 1000,
                    bucketSize: 4,
                    maxIterations: 50,
                    expansion: 4), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, "item1"), Is.True);
        }

        var chunks = this.CF_ScanDumpAll(source);
        this.CF_LoadAllChunks(target, chunks);

        var before = this._redis.Modules.Cuckoo.CF_Info(source);
        var after = this._redis.Modules.Cuckoo.CF_Info(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.BucketSize, Is.EqualTo(4));
            Assert.That(after.MaxIterations, Is.EqualTo(50));
            Assert.That(after.Expansion, Is.EqualTo(4));
        }
    }

    /// <summary>
    /// 测试经过自动扩容的过滤器能完整恢复。
    /// </summary>
    [Test]
    public void CF_ScanDump_LoadChunk_ScalingFilter()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            source,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 100; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, $"item:{i}"), Is.True);
        }

        var before = this._redis.Modules.Cuckoo.CF_Info(source);

        Assert.That(before.Filters, Is.GreaterThan(1));

        var chunks = this.CF_ScanDumpAll(source);
        this.CF_LoadAllChunks(target, chunks);

        var after = this._redis.Modules.Cuckoo.CF_Info(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.Expansion, Is.EqualTo(before.Expansion));
        }

        for (int i = 0; i < 100; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                target, $"item:{i}"), Is.True);
        }
    }

    /// <summary>
    /// 测试不可扩容过滤器的导出与恢复。
    /// </summary>
    [Test]
    public void CF_ScanDump_LoadChunk_NonScaling()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    source,
                    capacity: 100,
                    bucketSize: 2,
                    expansion: 0), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, "item1"), Is.True);
        }

        var chunks = this.CF_ScanDumpAll(source);
        this.CF_LoadAllChunks(target, chunks);

        var info = this._redis.Modules.Cuckoo.CF_Info(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
            Assert.That(info.Items, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试存在删除记录的过滤器恢复。
    /// </summary>
    [Test]
    public void CF_ScanDump_LoadChunk_AfterDelete()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    source, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, "a"), Is.True);
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, "b"), Is.True);
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, "c"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                source, "b"), Is.True);
        }

        var before = this._redis.Modules.Cuckoo.CF_Info(source);

        var chunks = this.CF_ScanDumpAll(source);
        this.CF_LoadAllChunks(target, chunks);

        var after = this._redis.Modules.Cuckoo.CF_Info(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.DeletedItems, Is.EqualTo(before.DeletedItems));

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                target, "a"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                target, "c"), Is.True);
        }
    }

    /// <summary>
    /// 测试 Unicode 和空字符串的完整恢复。
    /// </summary>
    [Test]
    public void CF_ScanDump_LoadChunk_SpecialItems()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            source, 1000), Is.True);

        foreach (var item in new[]
        {
        "", "你好🌍", "a b c", "a\nb\tc", "HELLO"
    })
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                source, item), Is.True);
        }

        var chunks = this.CF_ScanDumpAll(source);
        this.CF_LoadAllChunks(target, chunks);

        foreach (var item in new[]
        {
        "", "你好🌍", "a b c", "a\nb\tc", "HELLO"
    })
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                target, item), Is.True);
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Info(target).Items,
            Is.EqualTo(5));
    }

    /// <summary>
    /// 测试迭代器及终止结果。
    /// </summary>
    [Test]
    public void CF_ScanDump_IteratorAndCompletion()
    {
        var key = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        var first = this._redis.Modules.Cuckoo.CF_ScanDump(key, 0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first.Iterator, Is.GreaterThan(0));
            Assert.That(first.IsCompleted, Is.False);
            Assert.That(first.Data, Is.Not.Null.And.Not.Empty);
        }

        long iterator = first.Iterator;
        bool completed = false;

        for (int i = 0; i < 1024; i++)
        {
            var chunk = this._redis.Modules.Cuckoo.CF_ScanDump(
                key, iterator);

            if (chunk.IsCompleted)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(chunk.Iterator, Is.Zero);
                    Assert.That(chunk.Data, Is.Null);
                }
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

        Assert.That(completed, Is.True);
    }

    /// <summary>
    /// 测试重复导出时，数据块数量和内容一致。
    /// </summary>
    [Test]
    public void CF_ScanDump_RepeatDump()
    {
        var key = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        var first = this.CF_ScanDumpAll(key);
        var second = this.CF_ScanDumpAll(key);

        Assert.That(second, Has.Count.EqualTo(first.Count));

        for (int i = 0; i < first.Count; i++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(second[i].Iterator, Is.EqualTo(first[i].Iterator));
                Assert.That(second[i].Data, Is.EqualTo(first[i].Data));
            }
        }
    }

    /// <summary>
    /// 测试 CF.SCANDUMP 不存在的 Key。
    /// </summary>
    [Test]
    public void CF_ScanDump_KeyNotExists()
    {
        var key = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";

        CF_ScanDump_LoadChunkRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_ScanDump(key, 0));
    }

    /// <summary>
    /// 测试 CF.SCANDUMP 错误 Key 类型。
    /// </summary>
    [Test]
    public void CF_ScanDump_WrongKeyType()
    {
        var key = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CF_ScanDump_LoadChunkRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_ScanDump(key, 0));
    }

    /// <summary>
    /// 测试 CF.SCANDUMP 不能导出 Bloom Filter。
    /// </summary>
    [Test]
    public void CF_ScanDump_BloomKeyWrongType()
    {
        var key = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "hello"), Is.True);

        CF_ScanDump_LoadChunkRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_ScanDump(key, 0));
    }

    /// <summary>
    /// 测试 CF.LOADCHUNK 不能覆盖 String 类型的 Key。
    /// </summary>
    [Test]
    public void CF_LoadChunk_WrongKeyType()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            source, "hello"), Is.True);

        var chunks = this.CF_ScanDumpAll(source);

        Assert.That(chunks, Is.Not.Empty);

        this._redis.String.Set(target, "hello");

        CF_ScanDump_LoadChunkRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_LoadChunk(
                target, chunks[0].Iterator, chunks[0].Data));
    }

    /// <summary>
    /// 测试 CF.LOADCHUNK 不能覆盖 Bloom Filter。
    /// </summary>
    [Test]
    public void CF_LoadChunk_BloomKeyWrongType()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            source, "hello"), Is.True);

        var chunks = this.CF_ScanDumpAll(source);

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            target, "hello"), Is.True);

        CF_ScanDump_LoadChunkRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_LoadChunk(
                target, chunks[0].Iterator, chunks[0].Data));
    }

    /// <summary>
    /// 测试错误的 Header 数据。
    /// </summary>
    [Test]
    public void CF_LoadChunk_InvalidHeader()
    {
        var key = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        CF_ScanDump_LoadChunkRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_LoadChunk(
                key, 1, [1, 2, 3, 4, 5]));
    }

    /// <summary>
    /// 测试不加载 Header，直接加载后续数据块。
    /// </summary>
    [Test]
    public void CF_LoadChunk_MissingHeader()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            source, "hello"), Is.True);

        var chunks = this.CF_ScanDumpAll(source);

        Assert.That(chunks, Has.Count.GreaterThanOrEqualTo(2));

        CF_ScanDump_LoadChunkRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_LoadChunk(
                target, chunks[1].Iterator, chunks[1].Data));
    }

    /// <summary>
    /// 显式传入 CommandFlags.None。
    /// </summary>
    [Test]
    public void CF_ScanDump_LoadChunk_Flags_None()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            source, "hello"), Is.True);

        long iterator = 0;

        for (int i = 0; i < 1024; i++)
        {
            var chunk = this._redis.Modules.Cuckoo.CF_ScanDump(
                source, iterator, flags: CommandFlags.None);

            if (chunk.IsCompleted)
                break;

            Assert.That(this._redis.Modules.Cuckoo.CF_LoadChunk(
                target, chunk.Iterator, chunk.Data!,
                flags: CommandFlags.None), Is.True);

            iterator = chunk.Iterator;
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
            target, "hello"), Is.True);
    }

    /// <summary>
    /// 显式传入 CancellationToken.None。
    /// </summary>
    [Test]
    public void CF_ScanDump_LoadChunk_CancellationToken_Default()
    {
        var source = $"Cuckoo:{{CF_ScanDump}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunk}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            source, "hello"), Is.True);

        long iterator = 0;

        for (int i = 0; i < 1024; i++)
        {
            var chunk = this._redis.Modules.Cuckoo.CF_ScanDump(
                source, iterator,
                cancellationToken: CancellationToken.None);

            if (chunk.IsCompleted)
                break;

            Assert.That(this._redis.Modules.Cuckoo.CF_LoadChunk(
                target, chunk.Iterator, chunk.Data!,
                cancellationToken: CancellationToken.None), Is.True);

            iterator = chunk.Iterator;
        }

        Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
            target, "hello"), Is.True);
    }

    #endregion

    #region CF_MExists

    /// <summary>
    /// Key 不存在时，单元素查询返回 false。
    /// </summary>
    [Test]
    public void CF_MExists_Single_KeyNotExists()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
            key, "item1"), Is.False);
    }

    /// <summary>
    /// Key 不存在时，批量查询返回对应数量的 false。
    /// </summary>
    [Test]
    public void CF_MExists_Multiple_KeyNotExists()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        var result = this._redis.Modules.Cuckoo.CF_MExists(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo([false, false, false]));
    }

    /// <summary>
    /// 空过滤器中，单元素查询返回 false。
    /// </summary>
    [Test]
    public void CF_MExists_Single_EmptyFilter()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.Zero);
        }
    }

    /// <summary>
    /// 空过滤器中，批量查询全部返回 false。
    /// </summary>
    [Test]
    public void CF_MExists_Multiple_EmptyFilter()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_MExists(
            key, ["a", "b", "c", ""]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                false, false, false, false
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.Zero);
        }
    }

    /// <summary>
    /// 已添加的单个元素查询返回 true。
    /// </summary>
    [Test]
    public void CF_MExists_Single_ExistingItem()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, "hello"), Is.True);
        }
    }

    /// <summary>
    /// 单元素集合重载返回长度为 1 的数组。
    /// </summary>
    [Test]
    public void CF_MExists_Multiple_SingleItem()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_MExists(
            key, ["hello"]);

        Assert.That(result, Is.EqualTo([true]));
    }

    /// <summary>
    /// 多个已插入元素的批量查询。
    /// </summary>
    [Test]
    public void CF_MExists_Multiple_ExistingItems()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item2"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item3"), Is.True);
        }

        var result = this._redis.Modules.Cuckoo.CF_MExists(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    /// <summary>
    /// 验证批量结果与输入顺序对应。
    /// 在空过滤器中先查询全部不存在，再插入全部验证。
    /// </summary>
    [Test]
    public void CF_MExists_Multiple_ResultOrder()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_MExists(
            key, ["c", "a", "b"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.EqualTo(
            [
                false, false, false
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "b"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "c"), Is.True);
        }

        var after = this._redis.Modules.Cuckoo.CF_MExists(
            key, ["c", "a", "b"]);

        Assert.That(after, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    /// <summary>
    /// 输入包含重复元素时，每个位置均返回对应的存在状态。
    /// </summary>
    [Test]
    public void CF_MExists_Multiple_DuplicateQueries()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        var result = this._redis.Modules.Cuckoo.CF_MExists(
            key, ["hello", "hello", "hello", "hello"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                true, true, true, true
            ]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 元素被重复插入后，单元素和集合重载都返回 true。
    /// </summary>
    [Test]
    public void CF_MExists_DuplicateInsert()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, ["hello", "hello"]),
                Is.EqualTo([true, true]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.EqualTo(3));
        }
    }

    /// <summary>
    /// 删除唯一元素后查询返回 false。
    /// </summary>
    [Test]
    public void CF_MExists_AfterDelete()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, ["hello", "hello"]),
                Is.EqualTo([false, false]));
        }
    }

    /// <summary>
    /// 重复插入后删除一次，元素仍然存在。
    /// </summary>
    [Test]
    public void CF_MExists_Duplicate_DeleteOnce()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, ["hello", "hello"]),
                Is.EqualTo([true, true]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Count(
                key, "hello"), Is.GreaterThanOrEqualTo(2));
        }
    }

    /// <summary>
    /// 删除全部重复实例后返回 false。
    /// </summary>
    [Test]
    public void CF_MExists_Duplicate_DeleteAll()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);
        }

        for (int i = 0; i < 3; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                    key, "hello"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, ["hello"]),
                Is.EqualTo([false]));
        }
    }

    /// <summary>
    /// 删除后重新添加，查询恢复为 true。
    /// </summary>
    [Test]
    public void CF_MExists_DeleteAndReAdd()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Del(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, "hello"), Is.False);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, "hello"), Is.True);
        }
    }

    /// <summary>
    /// 单元素及批量重载的结果应与 CF.EXISTS 一致。
    /// </summary>
    [Test]
    public void CF_MExists_CompareWithExists()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "b"), Is.True);
        }

        var result = this._redis.Modules.Cuckoo.CF_MExists(
            key, ["a", "b", "c", "d"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(4));

            Assert.That(result[0], Is.EqualTo(
                this._redis.Modules.Cuckoo.CF_Exists(key, "a")));

            Assert.That(result[1], Is.EqualTo(
                this._redis.Modules.Cuckoo.CF_Exists(key, "b")));

            Assert.That(result[2], Is.EqualTo(
                this._redis.Modules.Cuckoo.CF_Exists(key, "c")));

            Assert.That(result[3], Is.EqualTo(
                this._redis.Modules.Cuckoo.CF_Exists(key, "d")));

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, "a"), Is.EqualTo(result[0]));
        }
    }

    /// <summary>
    /// Unicode、空字符串与特殊字符批量查询。
    /// </summary>
    [Test]
    public void CF_MExists_SpecialItems()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, ""), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "你好🌍"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a b c"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "a\nb\tc"), Is.True);
        }

        var result = this._redis.Modules.Cuckoo.CF_MExists(
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
    public void CF_MExists_CaseSensitive()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "Hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "HELLO"), Is.True);
        }

        var result = this._redis.Modules.Cuckoo.CF_MExists(
            key, ["HELLO", "Hello", "hello"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    /// <summary>
    /// 多次查询不会改变过滤器的统计信息。
    /// </summary>
    [Test]
    public void CF_MExists_ReadOnly()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        for (int i = 0; i < 10; i++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                            key, "hello"), Is.True);

                Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                    key, ["hello", "hello"]),
                    Is.EqualTo([true, true]));
            }
        }

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.DeletedItems, Is.EqualTo(before.DeletedItems));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.Size, Is.EqualTo(before.Size));
        }
    }

    /// <summary>
    /// 过滤器扩容之后批量查询。
    /// </summary>
    [Test]
    public void CF_MExists_AfterScaling()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, capacity: 4, bucketSize: 2, expansion: 2), Is.True);

        for (int i = 0; i < 50; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(info.Filters, Is.GreaterThan(1));

        var result = this._redis.Modules.Cuckoo.CF_MExists(
            key, ["item:0", "item:10", "item:25", "item:49"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true
        ]));
    }

    /// <summary>
    /// 不可扩容过滤器中，批量查询已添加元素。
    /// </summary>
    [Test]
    public void CF_MExists_NonScaling()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, capacity: 100, expansion: 0), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item2"), Is.True);
        }

        var result = this._redis.Modules.Cuckoo.CF_MExists(
            key, ["item1", "item2"]);

        Assert.That(result, Is.EqualTo([true, true]));
    }

    /// <summary>
    /// 多个不同 Key 的过滤器应分别查询。
    /// </summary>
    [Test]
    public void CF_MExists_DifferentKeys()
    {
        var key1 = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";
        var key2 = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key1, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                key2, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key1, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key2, "item2"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key1, ["item1"]),
                Is.EqualTo([true]));

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key2, ["item2"]),
                Is.EqualTo([true]));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key1).Items,
                Is.EqualTo(1));

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key2).Items,
                Is.EqualTo(1));
        }
    }

    /// <summary>
    /// String 类型 Key：按官方 CF.MEXISTS 返回说明应返回 false。
    /// </summary>
    [Test]
    public void CF_MExists_WrongKeyType()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                    key, "item1"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, ["item1", "item2"]),
                Is.EqualTo([false, false]));
        }
    }

    /// <summary>
    /// Bloom Filter Key：按官方返回说明应返回 false。
    /// </summary>
    [Test]
    public void CF_MExists_BloomKeyWrongType()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Bloom.BF_Add(
                    key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, "item1"), Is.False);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, ["item1", "item2"]),
                Is.EqualTo([false, false]));
        }
    }

    /// <summary>
    /// 显式传入 CommandFlags.None。
    /// </summary>
    [Test]
    public void CF_MExists_Flags_None()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, "hello",
                flags: CommandFlags.None), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, ["hello", "hello"],
                flags: CommandFlags.None),
                Is.EqualTo([true, true]));
        }
    }

    /// <summary>
    /// 显式传入 CancellationToken.None。
    /// </summary>
    [Test]
    public void CF_MExists_CancellationToken_Default()
    {
        var key = $"Cuckoo:{{CF_MExists}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, "hello"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, "hello",
                cancellationToken: CancellationToken.None), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_MExists(
                key, ["hello", "hello"],
                cancellationToken: CancellationToken.None),
                Is.EqualTo([true, true]));
        }
    }

    #endregion

    #region CF_Reserve

    private static void CF_ReserveRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    /// <summary>
    /// 测试使用默认参数创建 Cuckoo Filter。
    /// </summary>
    [Test]
    public void CF_Reserve_Default()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1000), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Buckets, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.BucketSize, Is.EqualTo(2));
            Assert.That(info.MaxIterations, Is.EqualTo(20));
            Assert.That(info.Expansion, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试自定义全部可选参数。
    /// </summary>
    [Test]
    public void CF_Reserve_CustomOptions()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 1000,
            bucketSize: 4,
            maxIterations: 50,
            expansion: 4), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Buckets, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.BucketSize, Is.EqualTo(4));
            Assert.That(info.MaxIterations, Is.EqualTo(50));
            Assert.That(info.Expansion, Is.EqualTo(4));
        }
    }

    /// <summary>
    /// 测试允许的最小容量及其边界。
    /// capacity 必须大于或等于 2 * bucketSize。
    /// </summary>
    [TestCase(2, 1)]
    [TestCase(4, 2)]
    [TestCase(8, 4)]
    [TestCase(16, 8)]
    [TestCase(510, 255)]
    public void CF_Reserve_MinimumCapacity(int capacity, int bucketSize)
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, capacity, bucketSize: bucketSize), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.BucketSize, Is.EqualTo(bucketSize));
            Assert.That(info.Buckets, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.Zero);
        }
    }

    /// <summary>
    /// 测试小于最小容量时返回 Redis 错误。
    /// </summary>
    [TestCase(0, 2)]
    [TestCase(-1, 2)]
    [TestCase(1, 1)]
    [TestCase(3, 2)]
    [TestCase(7, 4)]
    [TestCase(509, 255)]
    public void CF_Reserve_Capacity_TooSmall(
        int capacity, int bucketSize)
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        CF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Reserve(
                key, capacity, bucketSize: bucketSize));
    }

    /// <summary>
    /// 测试 BUCKETSIZE 的有效边界和常用值。
    /// </summary>
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(8)]
    [TestCase(16)]
    [TestCase(128)]
    [TestCase(255)]
    public void CF_Reserve_BucketSize_Valid(int bucketSize)
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 1024,
            bucketSize: bucketSize), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.BucketSize, Is.EqualTo(bucketSize));
            Assert.That(info.Buckets, Is.GreaterThan(0));
            Assert.That(info.Items, Is.Zero);
        }
    }

    /// <summary>
    /// 测试 BUCKETSIZE 非法值。
    /// </summary>
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(256)]
    [TestCase(1000)]
    public void CF_Reserve_BucketSize_Invalid(int bucketSize)
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        CF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Reserve(
                key, 1024,
                bucketSize: bucketSize));
    }

    /// <summary>
    /// 测试 MAXITERATIONS 的有效范围。
    /// </summary>
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(10)]
    [TestCase(20)]
    [TestCase(100)]
    [TestCase(65535)]
    public void CF_Reserve_MaxIterations_Valid(int maxIterations)
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100,
            maxIterations: maxIterations), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(info.MaxIterations, Is.EqualTo(maxIterations));
    }

    /// <summary>
    /// 测试 MAXITERATIONS 非法值。
    /// </summary>
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(65536)]
    public void CF_Reserve_MaxIterations_Invalid(int maxIterations)
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        CF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Reserve(
                key, 100,
                maxIterations: maxIterations));
    }

    /// <summary>
    /// 测试 EXPANSION 有效值。
    /// </summary>
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(8)]
    [TestCase(32768)]
    public void CF_Reserve_Expansion_Valid(int expansion)
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100,
            expansion: expansion), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            if (expansion == 0)
            {
                Assert.That(info.Expansion, Is.Zero);
            }
            else
            {
                Assert.That(info.Expansion, Is.EqualTo(expansion));
            }

            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// 测试 EXPANSION 向上取整到 2 的幂。
    /// </summary>
    [TestCase(3, 4)]
    [TestCase(5, 8)]
    [TestCase(6, 8)]
    [TestCase(7, 8)]
    [TestCase(9, 16)]
    [TestCase(100, 128)]
    public void CF_Reserve_Expansion_Rounded(
        int expansion, int expected)
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100,
            expansion: expansion), Is.True);

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        Assert.That(info.Expansion, Is.EqualTo(expected));
    }

    /// <summary>
    /// 测试 EXPANSION 非法值。
    /// </summary>
    [TestCase(-1)]
    [TestCase(32769)]
    [TestCase(65536)]
    public void CF_Reserve_Expansion_Invalid(int expansion)
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        CF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Reserve(
                key, 100,
                expansion: expansion));
    }

    /// <summary>
    /// 测试超过最大初始容量。
    /// </summary>
    [Test]
    public void CF_Reserve_Capacity_TooLarge()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        CF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Reserve(
                key, 1073741825));
    }

    /// <summary>
    /// 测试对同一 Key 重复创建过滤器。
    /// </summary>
    [Test]
    public void CF_Reserve_KeyAlreadyExists()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key, 100), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        CF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Reserve(
                key, 1000));

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.Size, Is.EqualTo(before.Size));
            Assert.That(after.BucketSize, Is.EqualTo(before.BucketSize));
            Assert.That(after.Items, Is.Zero);
        }
    }

    /// <summary>
    /// 测试 Key 已由 CF.ADD 自动创建后，不能再调用 CF.RESERVE。
    /// </summary>
    [Test]
    public void CF_Reserve_KeyAlreadyAutoCreated()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Add(
            key, "hello"), Is.True);

        CF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Reserve(
                key, 100));

        Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
            key, "hello"), Is.True);
    }

    /// <summary>
    /// 测试创建完成后可以正常添加和查询元素。
    /// </summary>
    [Test]
    public void CF_Reserve_AfterInsert()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 1000), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item1"), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, "item2"), Is.True);
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item1"), Is.True);
            Assert.That(this._redis.Modules.Cuckoo.CF_Exists(
                key, "item2"), Is.True);
        }
    }

    /// <summary>
    /// 测试 EXPANSION=0 时，过滤器不能自动扩容。
    /// </summary>
    [Test]
    public void CF_Reserve_NonScaling()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 1,
            maxIterations: 1,
            expansion: 0), Is.True);

        bool full = false;

        for (int i = 0; i < 1000; i++)
        {
            try
            {
                Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                    key, $"item:{i}"), Is.True);
            }
            catch (RedisErrorException)
            {
                full = true;
                break;
            }
        }

        var info = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(full, Is.True);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    /// <summary>
    /// 测试允许扩容时，插入数量超过初始容量。
    /// </summary>
    [Test]
    public void CF_Reserve_AutoScaling()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        var before = this._redis.Modules.Cuckoo.CF_Info(key);

        for (int i = 0; i < 40; i++)
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Add(
                key, $"item:{i}"), Is.True);
        }

        var after = this._redis.Modules.Cuckoo.CF_Info(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Filters, Is.GreaterThan(before.Filters));
            Assert.That(after.Items, Is.EqualTo(40));
            Assert.That(after.Expansion, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// 测试不同 Key 的过滤器配置互相独立。
    /// </summary>
    [Test]
    public void CF_Reserve_DifferentKeys()
    {
        var key1 = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";
        var key2 = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key1, 100, bucketSize: 2, expansion: 1), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                key2, 100, bucketSize: 4, expansion: 4), Is.True);
        }

        var info1 = this._redis.Modules.Cuckoo.CF_Info(key1);
        var info2 = this._redis.Modules.Cuckoo.CF_Info(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info1.BucketSize, Is.EqualTo(2));
            Assert.That(info1.Expansion, Is.EqualTo(1));
            Assert.That(info2.BucketSize, Is.EqualTo(4));
            Assert.That(info2.Expansion, Is.EqualTo(4));
            Assert.That(info1.Items, Is.Zero);
            Assert.That(info2.Items, Is.Zero);
        }
    }

    /// <summary>
    /// 测试 String 类型 Key 无法用于 CF.RESERVE。
    /// </summary>
    [Test]
    public void CF_Reserve_WrongKeyType()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "hello");

        CF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Reserve(
                key, 100));
    }

    /// <summary>
    /// 测试 Bloom Filter Key 无法用于 CF.RESERVE。
    /// </summary>
    [Test]
    public void CF_Reserve_BloomKeyWrongType()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        Assert.That(this._redis.Modules.Bloom.BF_Add(
            key, "hello"), Is.True);

        CF_ReserveRedisError<RedisErrorException>(() =>
            this._redis.Modules.Cuckoo.CF_Reserve(
                key, 100));
    }

    /// <summary>
    /// 显式指定 CommandFlags.None。
    /// </summary>
    [Test]
    public void CF_Reserve_Flags_None()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100,
                    flags: CommandFlags.None), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Items,
                Is.Zero);
        }
    }

    /// <summary>
    /// 显式指定 CancellationToken.None。
    /// </summary>
    [Test]
    public void CF_Reserve_CancellationToken_Default()
    {
        var key = $"Cuckoo:{{CF_Reserve}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Modules.Cuckoo.CF_Reserve(
                    key, 100,
                    cancellationToken: CancellationToken.None), Is.True);

            Assert.That(this._redis.Modules.Cuckoo.CF_Info(key).Filters,
                Is.EqualTo(1));
        }
    }

    #endregion
}
