using SharpRedis;
using SharpRedis.Exceptions;
using SharpRedis.Modules;

namespace NET10.Bloom;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class BloomAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region BF_Add

    private static string BF_AddKeyAsync() =>
        $"Bloom:{{BF_AddAsync}}:{Guid.NewGuid():N}";

    private static async Task BF_AddRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task BF_Add_AutoCreateAsync()
    {
        var key = BF_AddKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "item1"), Is.True);
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Capacity, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task BF_Add_FirstInsertAsync()
    {
        var key = BF_AddKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.000001, 1000), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "hello"), Is.True);
        }
    }

    [Test]
    public async Task BF_Add_DuplicateAsync()
    {
        var key = BF_AddKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.True);

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.False);

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.False);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(1));
    }

    [Test]
    public async Task BF_Add_MultipleItemsAsync()
    {
        var key = BF_AddKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(20));

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task BF_Add_UnicodeAsync()
    {
        var key = BF_AddKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "你好，世界🌍"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "你好，世界🌍"), Is.True);
        }

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "你好，世界🌍"), Is.False);
    }

    [Test]
    public async Task BF_Add_CaseSensitiveAsync()
    {
        var key = BF_AddKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.000001, 1000), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "Hello"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "HELLO"), Is.True);
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(3));
    }

    [Test]
    public async Task BF_Add_EmptyItemAsync()
    {
        var key = BF_AddKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, ""), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, ""), Is.True);
        }

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, ""), Is.False);
    }

    [Test]
    public async Task BF_Add_SpecialCharactersAsync()
    {
        var key = BF_AddKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.000001, 1000), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "a b c"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "a\nb\tc"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "a:b/c?d=e"), Is.True);
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(3));
    }

    [Test]
    public async Task BF_Add_ReservedFilterAsync()
    {
        var key = BF_AddKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, errorRate: 0.001, capacity: 100), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item2"), Is.True);
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Add_NonScaling_FullAsync()
    {
        var key = BF_AddKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item1"), Is.True);
        }

        await BF_AddRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item2"));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
        }

        Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "item1"), Is.True);
    }

    [Test]
    public async Task BF_Add_NonScaling_DuplicateAfterFullAsync()
    {
        var key = BF_AddKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item1"), Is.True);
        }

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.False);
    }

    [Test]
    public async Task BF_Add_ScalingAsync()
    {
        var key = BF_AddKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(10));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Capacity, Is.GreaterThan(2));
        }
    }

    [Test]
    public async Task BF_Add_WrongKeyTypeAsync()
    {
        var key = BF_AddKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await BF_AddRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item1"));
    }

    [Test]
    public async Task BF_Add_Flags_NoneAsync()
    {
        var key = BF_AddKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1",
            flags: CommandFlags.None), Is.True);

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1",
            flags: CommandFlags.None), Is.False);
    }

    [Test]
    public async Task BF_Add_CancellationToken_DefaultAsync()
    {
        var key = BF_AddKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.True);

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.False);
    }

    #endregion

    #region BF_Card

    private static string BF_CardKeyAsync() =>
        $"Bloom:{{BF_CardAsync}}:{Guid.NewGuid():N}";

    private static async Task BF_CardRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task BF_Card_KeyNotExistsAsync()
    {
        var key = BF_CardKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
            Is.Zero);
    }

    [Test]
    public async Task BF_Card_EmptyFilterAsync()
    {
        var key = BF_CardKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.001, 100), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.Zero);
        }
    }

    [Test]
    public async Task BF_Card_AfterFirstAddAsync()
    {
        var key = BF_CardKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Card_AfterDuplicateAddAsync()
    {
        var key = BF_CardKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.False);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.False);

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Card_MultipleItemsAsync()
    {
        var key = BF_CardKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        for (int i = 1; i <= 10; i++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                            key, $"item:{i}"), Is.True);

                Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                    Is.EqualTo(i));
            }
        }
    }

    [Test]
    public async Task BF_Card_AfterMAddAsync()
    {
        var key = BF_CardKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        // 异步接口的 SharpValueSpan 不一定能跨 await 保存，
        // 这里在调用处直接传集合表达式。
        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1", "item2", "item3"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added
            ]));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public async Task BF_Card_UnicodeItemsAsync()
    {
        var key = BF_CardKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.000001, 1000), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "你好"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "世界🌍"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "你好"), Is.False);

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(2));
        }
    }

    [Test]
    public async Task BF_Card_EqualsInfoItemsAsync()
    {
        var key = BF_CardKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, $"value:{i}"), Is.True);
        }

        var card = await this._redis.Modules.Bloom.BF_CardAsync(key);
        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(card, Is.EqualTo(10));
            Assert.That(card, Is.EqualTo(info.Items));
        }
    }

    [Test]
    public async Task BF_Card_AfterScalingAsync()
    {
        var key = BF_CardKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var card = await this._redis.Modules.Bloom.BF_CardAsync(key);
        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(card, Is.EqualTo(10));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(card, Is.EqualTo(info.Items));
        }
    }

    [Test]
    public async Task BF_Card_WrongKeyTypeAsync()
    {
        var key = BF_CardKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await BF_CardRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_CardAsync(key));
    }

    [Test]
    public async Task BF_Card_Flags_NoneAsync()
    {
        var key = BF_CardKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(
                key, flags: CommandFlags.None), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Card_CancellationToken_DefaultAsync()
    {
        var key = BF_CardKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(
                key,
                cancellationToken: CancellationToken.None),
                Is.EqualTo(1));
        }
    }

    #endregion

    #region BF_Exists

    private static string BF_ExistsKeyAsync() =>
        $"Bloom:{{BF_ExistsAsync}}:{Guid.NewGuid():N}";

    [Test]
    public async Task BF_Exists_KeyNotExistsAsync()
    {
        var key = BF_ExistsKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "item1"), Is.False);
    }

    [Test]
    public async Task BF_Exists_EmptyFilterAsync()
    {
        var key = BF_ExistsKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.001, 100), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "item1"), Is.False);
        }
    }

    [Test]
    public async Task BF_Exists_AfterAddAsync()
    {
        var key = BF_ExistsKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "item1"), Is.True);
        }
    }

    [Test]
    public async Task BF_Exists_AfterDuplicateAddAsync()
    {
        var key = BF_ExistsKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.False);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "item1"), Is.True);
        }
    }

    [Test]
    public async Task BF_Exists_MultipleItemsAsync()
    {
        var key = BF_ExistsKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task BF_Exists_NotAdded_EmptyFilterAsync()
    {
        var key = BF_ExistsKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100), Is.True);

        var hello = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "hello");

        var world = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "world");

        var number = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "123456");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hello, Is.False);
            Assert.That(world, Is.False);
            Assert.That(number, Is.False);
        }
    }

    [Test]
    public async Task BF_Exists_UnicodeAsync()
    {
        var key = BF_ExistsKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "你好，世界🌍"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "你好，世界🌍"), Is.True);
        }
    }

    [Test]
    public async Task BF_Exists_EmptyItemAsync()
    {
        var key = BF_ExistsKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, ""), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, ""), Is.True);
        }
    }

    [Test]
    public async Task BF_Exists_SpecialCharactersAsync()
    {
        var key = BF_ExistsKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.000001, 1000), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "a b c"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "a\nb\tc"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "a:b/c?d=e"), Is.True);
        }

        var first = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "a b c");

        var second = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "a\nb\tc");

        var third = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "a:b/c?d=e");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.True);
            Assert.That(second, Is.True);
            Assert.That(third, Is.True);
        }
    }

    [Test]
    public async Task BF_Exists_CaseSensitiveAsync()
    {
        var key = BF_ExistsKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.000001, 1000), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "Hello"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "Hello"), Is.True);
        }
    }

    [Test]
    public async Task BF_Exists_AfterMAddAsync()
    {
        var key = BF_ExistsKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        var added = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(added, Is.EqualTo(
        [
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added
        ]));

        var item1 = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "item1");

        var item2 = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "item2");

        var item3 = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "item3");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(item1, Is.True);
            Assert.That(item2, Is.True);
            Assert.That(item3, Is.True);
        }
    }

    [Test]
    public async Task BF_Exists_NonScalingFilterAsync()
    {
        var key = BF_ExistsKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "item1"), Is.True);
        }
    }

    [Test]
    public async Task BF_Exists_ReadDoesNotChangeCardinalityAsync()
    {
        var key = BF_ExistsKeyAsync();

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        var before = await this._redis.Modules.Bloom.BF_CardAsync(key);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "item1"), Is.True);
        }

        var after = await this._redis.Modules.Bloom.BF_CardAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.EqualTo(1));
            Assert.That(after, Is.EqualTo(before));
        }
    }

    [Test]
    public async Task BF_Exists_Flags_NoneAsync()
    {
        var key = BF_ExistsKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "item1",
                flags: CommandFlags.None), Is.True);
        }
    }

    [Test]
    public async Task BF_Exists_CancellationToken_DefaultAsync()
    {
        var key = BF_ExistsKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "item1",
                cancellationToken: CancellationToken.None), Is.True);
        }
    }

    #endregion

    #region BF_Info

    private static async Task BF_InfoRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task BF_Info_KeyNotExistsAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        await BF_InfoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InfoAsync(key));

        await BF_InfoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InfoAsync(
                key, RedisBloom.InfoType.Capacity));
    }

    [Test]
    public async Task BF_Info_EmptyFilterAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

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
    public async Task BF_Info_AutoCreatedFilterAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

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
    public async Task BF_Info_CustomCapacityAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, errorRate: 0.001, capacity: 500), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(500));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Size, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task BF_Info_ItemsAfterAddAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(10));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Capacity, Is.EqualTo(1000));
        }
    }

    [Test]
    public async Task BF_Info_DuplicateDoesNotIncreaseItemsAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.False);

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.False);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(1));
    }

    [Test]
    public async Task BF_Info_AllInfoTypesAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.001,
                    capacity: 200,
                    expansion: 4), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item2"), Is.True);
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        var capacity = await this._redis.Modules.Bloom.BF_InfoAsync(
            key, RedisBloom.InfoType.Capacity);

        var size = await this._redis.Modules.Bloom.BF_InfoAsync(
            key, RedisBloom.InfoType.Size);

        var filters = await this._redis.Modules.Bloom.BF_InfoAsync(
            key, RedisBloom.InfoType.Filters);

        var items = await this._redis.Modules.Bloom.BF_InfoAsync(
            key, RedisBloom.InfoType.Items);

        var expansion = await this._redis.Modules.Bloom.BF_InfoAsync(
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
    public async Task BF_Info_CustomExpansionAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key,
            errorRate: 0.001,
            capacity: 100,
            expansion: 8), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Expansion.HasValue, Is.True);
            Assert.That(info.Expansion, Is.EqualTo(8));
            Assert.That(info.Filters, Is.EqualTo(1));
        }

        Assert.That(await this._redis.Modules.Bloom.BF_InfoAsync(
            key, RedisBloom.InfoType.Expansion), Is.EqualTo(8));
    }

    [Test]
    public async Task BF_Info_NonScaling_ExpansionIsNullAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key,
            errorRate: 0.001,
            capacity: 100,
            nonscaling: true), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

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
    public async Task BF_Info_NonScaling_AfterAddAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key,
            errorRate: 0.000001,
            capacity: 100,
            nonscaling: true), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.EqualTo(10));
            Assert.That(info.Expansion, Is.Null);
        }
    }

    [Test]
    public async Task BF_Info_AfterScalingAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

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
    public async Task BF_Info_WrongKeyTypeAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await BF_InfoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InfoAsync(key));

        await BF_InfoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InfoAsync(
                key, RedisBloom.InfoType.Items));
    }

    [Test]
    public async Task BF_Info_Flags_NoneAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(
            key, flags: CommandFlags.None);

        var capacity = await this._redis.Modules.Bloom.BF_InfoAsync(
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
    public async Task BF_Info_CancellationToken_DefaultAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(
            key, cancellationToken: CancellationToken.None);

        var capacity = await this._redis.Modules.Bloom.BF_InfoAsync(
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
    public async Task BF_Info_NonScaling_ExpansionSelectorAsync()
    {
        var key = $"Bloom:{{BF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100, nonscaling: true), Is.True);

        var expansion = await this._redis.Modules.Bloom.BF_InfoAsync(
            key, RedisBloom.InfoType.Expansion);

        Assert.That(expansion, Is.Null);
    }
    #endregion

    #region BF_Insert

    private static async Task BF_InsertRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task BF_Insert_Single_AutoCreateAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
            key, "item1");

        Assert.That(result, Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Capacity, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task BF_Insert_Single_DuplicateAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));

        Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
                    key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Insert_Multiple_AutoCreateAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
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

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public async Task BF_Insert_Multiple_DuplicateAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
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

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public async Task BF_Insert_ExistingFilterAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 100), Is.True);

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
            key, ["item1", "item2"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added
        ]));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Items, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task BF_Insert_CustomCapacityAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
            key, "item1",
            capacity: 500,
            error: 0.001), Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(500));
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Insert_CustomExpansionAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
            key, "item1",
            capacity: 100,
            error: 0.001,
            expansion: 4);

        Assert.That(result, Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Expansion, Is.EqualTo(4));
            Assert.That(info.Items, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Insert_ExistingFilter_OptionsIgnoredAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.001,
                    capacity: 100,
                    expansion: 2), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
                key, "item1",
                capacity: 500,
                error: 0.0001,
                expansion: 8), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Expansion, Is.EqualTo(2));
            Assert.That(info.Items, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Insert_NoCreate_KeyNotExistsAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        await BF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InsertAsync(
                key, "item1", nocreate: true));

        await BF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InsertAsync(
                key, ["item1", "item2"], nocreate: true));

        Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
            Is.Zero);
    }

    [Test]
    public async Task BF_Insert_NoCreate_ExistingFilterAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.000001, 100), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
                key, "item1", nocreate: true),
                Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
            key, ["item1", "item2"], nocreate: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Exists,
                RedisBloom.AddStatus.Added
            ]));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(2));
        }
    }

    [Test]
    public async Task BF_Insert_NonScaling_CreateAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
            key, "item1",
            capacity: 100,
            error: 0.001,
            nonscaling: true),
            Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Null);
        }
    }

    [Test]
    public async Task BF_Insert_Single_NonScalingFullAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));

            Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
                key, "item2"), Is.EqualTo(RedisBloom.AddStatus.Error));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Insert_Multiple_NonScalingFullAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
            key, ["item2", "item3", "item4"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Error
            ]));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Insert_Multiple_PartialSuccessAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key,
            errorRate: 0.000001,
            capacity: 2,
            nonscaling: true), Is.True);

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
            key, ["item1", "item2", "item3", "item4"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Added,
            RedisBloom.AddStatus.Error
        ]));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Insert_NonScalingFull_DuplicateAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Exists,
            RedisBloom.AddStatus.Error
        ]));
    }

    [Test]
    public async Task BF_Insert_AutoScalingAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
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

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(5));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Capacity, Is.GreaterThan(2));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task BF_Insert_SpecialItemsAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
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

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(4));
        }
    }

    [Test]
    public async Task BF_Insert_InvalidErrorRateAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        await BF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InsertAsync(
                key, "item1", error: -0.1));

        await BF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InsertAsync(
                key, ["item1", "item2"], error: 0));
    }

    [Test]
    public async Task BF_Insert_InvalidCapacityAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        await BF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InsertAsync(
                key, "item1", capacity: 0));

        await BF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InsertAsync(
                key, ["item1", "item2"], capacity: -1));
    }

    [Test]
    public async Task BF_Insert_WrongKeyTypeAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await BF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InsertAsync(
                key, "item1"));

        await BF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_InsertAsync(
                key, ["item1", "item2"]));
    }

    [Test]
    public async Task BF_Insert_Flags_NoneAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
            key, "item1",
            flags: CommandFlags.None),
            Is.EqualTo(RedisBloom.AddStatus.Added));

        Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
            key, "item1",
            flags: CommandFlags.None),
            Is.EqualTo(RedisBloom.AddStatus.Exists));
    }

    [Test]
    public async Task BF_Insert_CancellationToken_DefaultAsync()
    {
        var key = $"Bloom:{{BF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_InsertAsync(
            key, "item1",
            cancellationToken: CancellationToken.None),
            Is.EqualTo(RedisBloom.AddStatus.Added));

        var result = await this._redis.Modules.Bloom.BF_InsertAsync(
            key, ["item1", "item2"],
            cancellationToken: CancellationToken.None);

        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Exists,
            RedisBloom.AddStatus.Added
        ]));
    }

    #endregion

    #region BF_ScanDump_BF_LoadChunk

    private static async Task BF_ScanDumpRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    /// <summary>
    /// 异步导出全部有效数据块并恢复到目标过滤器。
    /// </summary>
    private async Task BF_ScanDumpRestoreAsync(
        string source, string target)
    {
        long iterator = 0;

        while (true)
        {
            var chunk = await this._redis.Modules.Bloom.BF_ScanDumpAsync(
                source, iterator);

            if (chunk.IsCompleted)
                break;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(chunk.Iterator, Is.GreaterThan(0));
                Assert.That(chunk.Data, Is.Not.Null.And.Not.Empty);

                Assert.That(await this._redis.Modules.Bloom.BF_LoadChunkAsync(
                    target, chunk.Iterator, chunk.Data), Is.True);
            }

            iterator = chunk.Iterator;
        }
    }

    [Test]
    public async Task BF_ScanDump_FirstChunkAsync()
    {
        var key = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        var result = await this._redis.Modules.Bloom.BF_ScanDumpAsync(
            key, 0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Iterator, Is.GreaterThan(0));
            Assert.That(result.Data, Is.Not.Null.And.Not.Empty);
            Assert.That(result.IsCompleted, Is.False);
        }
    }

    [Test]
    public async Task BF_ScanDump_IteratorCompletionAsync()
    {
        var key = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        long iterator = 0;
        int count = 0;
        bool completed = false;

        for (int i = 0; i < 10000; i++)
        {
            var chunk = await this._redis.Modules.Bloom.BF_ScanDumpAsync(
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

    [Test]
    public async Task BF_LoadChunk_SingleItemRestoreAsync()
    {
        var source = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            source, "hello"), Is.True);

        await this.BF_ScanDumpRestoreAsync(source, target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                    target, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(target),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_LoadChunk_MultipleItemsRestoreAsync()
    {
        var source = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            source, 0.000001, 1000), Is.True);

        long inserted = 0;

        for (int i = 0; i < 100; i++)
        {
            if (await this._redis.Modules.Bloom.BF_AddAsync(
                source, $"item:{i}"))
            {
                inserted++;
            }
        }

        await this.BF_ScanDumpRestoreAsync(source, target);

        var sourceInfo = await this._redis.Modules.Bloom.BF_InfoAsync(source);
        var targetInfo = await this._redis.Modules.Bloom.BF_InfoAsync(target);

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
            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                target, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task BF_LoadChunk_EmptyFilterRestoreAsync()
    {
        var source = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            source, 0.001, 100), Is.True);

        await this.BF_ScanDumpRestoreAsync(source, target);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task BF_LoadChunk_SpecialItemsRestoreAsync()
    {
        var source = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    source, 0.000001, 1000), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                source, ""), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                source, "中文🌍"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                source, "a\nb\tc"), Is.True);
        }

        await this.BF_ScanDumpRestoreAsync(source, target);

        var empty = await this._redis.Modules.Bloom.BF_ExistsAsync(
            target, "");

        var unicode = await this._redis.Modules.Bloom.BF_ExistsAsync(
            target, "中文🌍");

        var special = await this._redis.Modules.Bloom.BF_ExistsAsync(
            target, "a\nb\tc");

        var card = await this._redis.Modules.Bloom.BF_CardAsync(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(empty, Is.True);
            Assert.That(unicode, Is.True);
            Assert.That(special, Is.True);
            Assert.That(card, Is.EqualTo(3));
        }
    }

    [Test]
    public async Task BF_LoadChunk_NonScalingRestoreAsync()
    {
        var source = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    source,
                    errorRate: 0.000001,
                    capacity: 2,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                source, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                source, "item2"), Is.True);
        }

        await this.BF_ScanDumpRestoreAsync(source, target);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(target);

        var first = await this._redis.Modules.Bloom.BF_ExistsAsync(
            target, "item1");

        var second = await this._redis.Modules.Bloom.BF_ExistsAsync(
            target, "item2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(2));
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Null);
            Assert.That(first, Is.True);
            Assert.That(second, Is.True);
        }
    }

    [Test]
    public async Task BF_LoadChunk_ScalingRestoreAsync()
    {
        var source = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            source,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            await this._redis.Modules.Bloom.BF_AddAsync(
                source, $"item:{i}");
        }

        var before = await this._redis.Modules.Bloom.BF_InfoAsync(source);

        Assert.That(before.Filters, Is.GreaterThan(1));

        await this.BF_ScanDumpRestoreAsync(source, target);

        var after = await this._redis.Modules.Bloom.BF_InfoAsync(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Capacity, Is.EqualTo(before.Capacity));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.Expansion, Is.EqualTo(before.Expansion));
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                target, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task BF_LoadChunk_MultipleChunksRestoreAsync()
    {
        var source = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            source,
            errorRate: 0.000001,
            capacity: 100000), Is.True);

        for (int i = 0; i < 100; i++)
        {
            await this._redis.Modules.Bloom.BF_AddAsync(
                source, $"item:{i}");
        }

        long iterator = 0;
        int chunkCount = 0;

        while (true)
        {
            var chunk = await this._redis.Modules.Bloom.BF_ScanDumpAsync(
                source, iterator);

            if (chunk.IsCompleted)
                break;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(chunk.Data, Is.Not.Null.And.Not.Empty);

                Assert.That(await this._redis.Modules.Bloom.BF_LoadChunkAsync(
                    target, chunk.Iterator, chunk.Data), Is.True);
            }

            iterator = chunk.Iterator;
            chunkCount++;
        }

        var sourceInfo = await this._redis.Modules.Bloom.BF_InfoAsync(source);
        var targetInfo = await this._redis.Modules.Bloom.BF_InfoAsync(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(chunkCount, Is.GreaterThan(1));
            Assert.That(targetInfo.Capacity, Is.EqualTo(sourceInfo.Capacity));
            Assert.That(targetInfo.Items, Is.EqualTo(sourceInfo.Items));
            Assert.That(targetInfo.Size, Is.EqualTo(sourceInfo.Size));
        }

        for (int i = 0; i < 100; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                target, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task BF_ScanDump_KeyNotExistsAsync()
    {
        var key = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";

        await BF_ScanDumpRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ScanDumpAsync(
                key, 0));
    }

    [Test]
    public async Task BF_ScanDump_WrongKeyTypeAsync()
    {
        var key = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await BF_ScanDumpRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ScanDumpAsync(
                key, 0));
    }

    [Test]
    public async Task BF_LoadChunk_InvalidDataAsync()
    {
        var key = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        await BF_ScanDumpRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_LoadChunkAsync(
                key, 1, [0x01, 0x02, 0x03]));
    }

    [Test]
    public async Task BF_LoadChunk_WrongKeyTypeAsync()
    {
        var key = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";
        var source = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            source, "item1"), Is.True);

        var chunk = await this._redis.Modules.Bloom.BF_ScanDumpAsync(
            source, 0);

        await BF_ScanDumpRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_LoadChunkAsync(
                key, chunk.Iterator, chunk.Data));
    }

    [Test]
    public async Task BF_LoadChunk_MissingHeaderAsync()
    {
        var source = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            source, "item1"), Is.True);

        var first = await this._redis.Modules.Bloom.BF_ScanDumpAsync(
            source, 0);

        Assert.That(first.IsCompleted, Is.False);

        var second = await this._redis.Modules.Bloom.BF_ScanDumpAsync(
            source, first.Iterator);

        Assert.That(second.IsCompleted, Is.False);

        await BF_ScanDumpRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_LoadChunkAsync(
                target, second.Iterator, second.Data));
    }

    [Test]
    public async Task BF_ScanDump_LoadChunk_Flags_NoneAsync()
    {
        var source = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            source, "item1"), Is.True);

        var chunk = await this._redis.Modules.Bloom.BF_ScanDumpAsync(
            source, 0,
            flags: CommandFlags.None);

        Assert.That(await this._redis.Modules.Bloom.BF_LoadChunkAsync(
            target, chunk.Iterator, chunk.Data,
            flags: CommandFlags.None), Is.True);

        // 将剩余数据块继续恢复，避免重复加载首块元数据。
        long iterator = chunk.Iterator;

        while (true)
        {
            var next = await this._redis.Modules.Bloom.BF_ScanDumpAsync(
                source, iterator);

            if (next.IsCompleted)
                break;

            Assert.That(await this._redis.Modules.Bloom.BF_LoadChunkAsync(
                target, next.Iterator, next.Data), Is.True);

            iterator = next.Iterator;
        }

        Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
            target, "item1"), Is.True);
    }

    [Test]
    public async Task BF_ScanDump_LoadChunk_CancellationToken_DefaultAsync()
    {
        var source = $"Bloom:{{BF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Bloom:{{BF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            source, "item1"), Is.True);

        var chunk = await this._redis.Modules.Bloom.BF_ScanDumpAsync(
            source, 0,
            cancellationToken: CancellationToken.None);

        Assert.That(await this._redis.Modules.Bloom.BF_LoadChunkAsync(
            target, chunk.Iterator, chunk.Data,
            cancellationToken: CancellationToken.None), Is.True);

        long iterator = chunk.Iterator;

        while (true)
        {
            var next = await this._redis.Modules.Bloom.BF_ScanDumpAsync(
                source, iterator);

            if (next.IsCompleted)
                break;

            Assert.That(await this._redis.Modules.Bloom.BF_LoadChunkAsync(
                target, next.Iterator, next.Data), Is.True);

            iterator = next.Iterator;
        }

        Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
            target, "item1"), Is.True);
    }

    #endregion

    #region BF_MAdd

    private static async Task BF_MAddRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task BF_MAdd_Single_AutoCreateAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, "item1");

        Assert.That(result, Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Capacity, Is.GreaterThan(0));
        }

        Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "item1"), Is.True);
    }

    [Test]
    public async Task BF_MAdd_Single_DuplicateAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_MAddAsync(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));

        Assert.That(await this._redis.Modules.Bloom.BF_MAddAsync(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_MAddAsync(
                    key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_MAdd_Multiple_AutoCreateAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Has.Length.EqualTo(3));
        Assert.That(result[0], Is.EqualTo(RedisBloom.AddStatus.Added));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items,
                Is.EqualTo(result.Count(x => x == RedisBloom.AddStatus.Added)));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "item2"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, "item3"), Is.True);
        }
    }

    [Test]
    public async Task BF_MAdd_Multiple_DuplicateAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1", "item1", "item2", "item2", "item1"]);

        Assert.That(result, Has.Length.EqualTo(5));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0], Is.EqualTo(RedisBloom.AddStatus.Added));
            Assert.That(result[1], Is.EqualTo(RedisBloom.AddStatus.Exists));
            Assert.That(result[3], Is.EqualTo(RedisBloom.AddStatus.Exists));
            Assert.That(result[4], Is.EqualTo(RedisBloom.AddStatus.Exists));
        }

        Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
            Is.EqualTo(result.Count(x => x == RedisBloom.AddStatus.Added)));
    }

    [Test]
    public async Task BF_MAdd_ExistingFilterAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.000001, 1000), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item1"), Is.True);
        }

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Has.Length.EqualTo(3));
        Assert.That(result[0], Is.EqualTo(RedisBloom.AddStatus.Exists));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(1000));
            Assert.That(info.Items,
                Is.EqualTo(1 + result.Count(x => x == RedisBloom.AddStatus.Added)));
        }
    }

    [Test]
    public async Task BF_MAdd_SingleAndMultipleAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_MAddAsync(
            key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Exists
            ]));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_MAdd_Multiple_RepeatedBatchAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        var first = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["a", "b", "c"]);

        Assert.That(first, Has.Length.EqualTo(3));

        var before = await this._redis.Modules.Bloom.BF_CardAsync(key);

        var second = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["a", "b", "c"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(second, Is.EqualTo(
            [
                RedisBloom.AddStatus.Exists,
                RedisBloom.AddStatus.Exists,
                RedisBloom.AddStatus.Exists
            ]));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(before));
        }
    }

    [Test]
    public async Task BF_MAdd_SpecialItemsAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["", "中文🌍", "hello world", "a\nb\tc"]);

        Assert.That(result, Has.Length.EqualTo(4));

        var empty = await this._redis.Modules.Bloom.BF_ExistsAsync(key, "");
        var unicode = await this._redis.Modules.Bloom.BF_ExistsAsync(key, "中文🌍");
        var space = await this._redis.Modules.Bloom.BF_ExistsAsync(key, "hello world");
        var special = await this._redis.Modules.Bloom.BF_ExistsAsync(key, "a\nb\tc");
        var card = await this._redis.Modules.Bloom.BF_CardAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(empty, Is.True);
            Assert.That(unicode, Is.True);
            Assert.That(space, Is.True);
            Assert.That(special, Is.True);
            Assert.That(card,
                Is.EqualTo(result.Count(x => x == RedisBloom.AddStatus.Added)));
        }
    }

    [Test]
    public async Task BF_MAdd_CaseSensitiveAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["Hello", "hello", "HELLO"]);

        Assert.That(result, Has.Length.EqualTo(3));

        var first = await this._redis.Modules.Bloom.BF_ExistsAsync(key, "Hello");
        var second = await this._redis.Modules.Bloom.BF_ExistsAsync(key, "hello");
        var third = await this._redis.Modules.Bloom.BF_ExistsAsync(key, "HELLO");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.True);
            Assert.That(second, Is.True);
            Assert.That(third, Is.True);
        }
    }

    [Test]
    public async Task BF_MAdd_Single_NonScalingFullAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_MAddAsync(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_MAddAsync(
                    key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Exists));

            Assert.That(await this._redis.Modules.Bloom.BF_MAddAsync(
                key, "item2"), Is.EqualTo(RedisBloom.AddStatus.Error));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_MAdd_Multiple_NonScalingFullAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_MAddAsync(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item2", "item3", "item4"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Error
            ]));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_MAdd_Multiple_PartialSuccessAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key,
            errorRate: 0.000001,
            capacity: 2,
            nonscaling: true), Is.True);

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1", "item2", "item3", "item4"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Added,
                RedisBloom.AddStatus.Error
            ]));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(2));
        }
    }

    [Test]
    public async Task BF_MAdd_NonScalingFull_DuplicateAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_MAddAsync(
                key, "item1"), Is.EqualTo(RedisBloom.AddStatus.Added));
        }

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Exists,
            RedisBloom.AddStatus.Error
        ]));
    }

    [Test]
    public async Task BF_MAdd_AutoScalingAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1", "item2", "item3", "item4", "item5"]);

        Assert.That(result, Has.Length.EqualTo(5));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items,
                Is.EqualTo(result.Count(x => x == RedisBloom.AddStatus.Added)));

            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Capacity, Is.GreaterThan(2));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task BF_MAdd_WrongKeyTypeAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await BF_MAddRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_MAddAsync(
                key, "item1"));

        await BF_MAddRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_MAddAsync(
                key, ["item1", "item2"]));
    }

    [Test]
    public async Task BF_MAdd_Flags_NoneAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_MAddAsync(
            key, "item1",
            flags: CommandFlags.None),
            Is.EqualTo(RedisBloom.AddStatus.Added));

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1", "item2"],
            flags: CommandFlags.None);

        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Exists,
            RedisBloom.AddStatus.Added
        ]));
    }

    [Test]
    public async Task BF_MAdd_CancellationToken_DefaultAsync()
    {
        var key = $"Bloom:{{BF_MAddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_MAddAsync(
            key, "item1",
            cancellationToken: CancellationToken.None),
            Is.EqualTo(RedisBloom.AddStatus.Added));

        var result = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1", "item2"],
            cancellationToken: CancellationToken.None);

        Assert.That(result, Is.EqualTo(
        [
            RedisBloom.AddStatus.Exists,
            RedisBloom.AddStatus.Added
        ]));
    }

    #endregion

    #region BF_MExists

    [Test]
    public async Task BF_MExists_Single_KeyNotExistsAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, "item1"), Is.False);
    }

    [Test]
    public async Task BF_MExists_Multiple_KeyNotExistsAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["item1", "item2", "item3"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                false, false, false
            ]));

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.Zero);
        }
    }

    [Test]
    public async Task BF_MExists_EmptyFilterAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.001, 100), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_MExistsAsync(
                key, "item1"), Is.False);
        }

        var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            false, false, false
        ]));
    }

    [Test]
    public async Task BF_MExists_Single_AfterAddAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_MExistsAsync(
                key, "item1"), Is.True);
        }
    }

    [Test]
    public async Task BF_MExists_Single_DuplicateAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.False);

            Assert.That(await this._redis.Modules.Bloom.BF_MExistsAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_CardAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_MExists_Multiple_AllExistsAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        var added = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(added, Has.Length.EqualTo(3));

        var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    [Test]
    public async Task BF_MExists_SingleAndMultipleAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        var single = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, "item1");

        var multiple = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["item1"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(single, Is.True);
            Assert.That(multiple, Is.EqualTo([true]));
        }
    }

    [Test]
    public async Task BF_MExists_Multiple_DuplicateItemsAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.True);

        var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["hello", "hello", "hello", "hello"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true
        ]));
    }

    [Test]
    public async Task BF_MExists_BeforeAndAfterAddAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100), Is.True);

        var before = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["item1", "item2"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.EqualTo([false, false]));

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_MExistsAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item2"), Is.True);
        }

        var after = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["item1", "item2"]);

        Assert.That(after, Is.EqualTo([true, true]));
    }

    [Test]
    public async Task BF_MExists_SpecialItemsAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        var added = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["", "你好🌍", "a b c", "a\nb\tc"]);

        Assert.That(added, Has.Length.EqualTo(4));

        var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["", "你好🌍", "a b c", "a\nb\tc"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true
        ]));
    }

    [Test]
    public async Task BF_MExists_CaseSensitiveAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.000001, 1000), Is.True);

        var added = await this._redis.Modules.Bloom.BF_MAddAsync(
            key, ["Hello", "hello", "HELLO"]);

        Assert.That(added, Has.Length.EqualTo(3));

        var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["Hello", "hello", "HELLO"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    [Test]
    public async Task BF_MExists_ReadOnlyAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        var before = await this._redis.Modules.Bloom.BF_CardAsync(key);

        for (int i = 0; i < 10; i++)
        {
            var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
                key, ["item1", "item1"]);

            Assert.That(result, Is.EqualTo([true, true]));
        }

        var after = await this._redis.Modules.Bloom.BF_CardAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.EqualTo(1));
            Assert.That(after, Is.EqualTo(before));
        }
    }

    [Test]
    public async Task BF_MExists_ScalingFilterAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            await this._redis.Modules.Bloom.BF_AddAsync(
                key, $"item:{i}");
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        Assert.That(info.Filters, Is.GreaterThan(1));

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_MExistsAsync(
                key, $"item:{i}"), Is.True);
        }

        var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["item:0", "item:5", "item:10", "item:19"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true
        ]));
    }

    [Test]
    public async Task BF_MExists_NonScalingFilterAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.000001,
                    capacity: 2,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item2"), Is.True);
        }

        var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["item1", "item2", "item1"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    [Test]
    public async Task BF_MExists_CompareWithExistsAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key, 0.000001, 1000), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item1"), Is.True);
        }

        var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["item1", "item2", "item3"]);

        var item1 = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "item1");

        var item2 = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "item2");

        var item3 = await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "item3");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result[0], Is.True);
            Assert.That(result[0], Is.EqualTo(item1));
            Assert.That(result[1], Is.EqualTo(item2));
            Assert.That(result[2], Is.EqualTo(item3));
        }
    }

    [Test]
    public async Task BF_MExists_Flags_NoneAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_MExistsAsync(
                key, "item1",
                flags: CommandFlags.None), Is.True);
        }

        var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["item1", "item1"],
            flags: CommandFlags.None);

        Assert.That(result, Is.EqualTo([true, true]));
    }

    [Test]
    public async Task BF_MExists_CancellationToken_DefaultAsync()
    {
        var key = $"Bloom:{{BF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_MExistsAsync(
                key, "item1",
                cancellationToken: CancellationToken.None), Is.True);
        }

        var result = await this._redis.Modules.Bloom.BF_MExistsAsync(
            key, ["item1", "item1"],
            cancellationToken: CancellationToken.None);

        Assert.That(result, Is.EqualTo([true, true]));
    }

    #endregion

    #region BF_Reserve

    private static async Task BF_ReserveRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task BF_Reserve_DefaultAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

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
    public async Task BF_Reserve_MinCapacityAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.01, 1), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(1));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Reserve_CustomCapacityAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 10000), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(10000));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Size, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task BF_Reserve_CustomErrorRateAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.0001, 1000), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(1000));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Size, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task BF_Reserve_ErrorRateAffectsSizeAsync()
    {
        var key1 = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key1, 0.01, 10000), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                key2, 0.000001, 10000), Is.True);
        }

        var info1 = await this._redis.Modules.Bloom.BF_InfoAsync(key1);
        var info2 = await this._redis.Modules.Bloom.BF_InfoAsync(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info1.Capacity, Is.EqualTo(10000));
            Assert.That(info2.Capacity, Is.EqualTo(10000));
            Assert.That(info2.Size, Is.GreaterThan(info1.Size));
        }
    }

    [Test]
    public async Task BF_Reserve_Expansion_OneAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100, expansion: 1), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Reserve_Expansion_FourAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100, expansion: 4), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Expansion, Is.EqualTo(4));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task BF_Reserve_Expansion_EightAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100, expansion: 8), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        Assert.That(info.Expansion, Is.EqualTo(8));
    }

    [Test]
    public async Task BF_Reserve_NonScalingAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100, nonscaling: true), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Null);
        }
    }

    [Test]
    public async Task BF_Reserve_Expansion_ZeroAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100, expansion: 0), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Capacity, Is.EqualTo(100));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Null);
        }
    }

    [Test]
    public async Task BF_Reserve_NonScaling_FullAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
                    key,
                    errorRate: 0.000001,
                    capacity: 1,
                    nonscaling: true), Is.True);

            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item1"), Is.True);
        }

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_AddAsync(
                key, "item2"));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Null);
        }
    }

    [Test]
    public async Task BF_Reserve_AutoScalingAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key,
            errorRate: 0.000001,
            capacity: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 10; i++)
        {
            await this._redis.Modules.Bloom.BF_AddAsync(
                key, $"item:{i}");
        }

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Capacity, Is.GreaterThan(2));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
                key, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task BF_Reserve_DuplicateKeyAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100), Is.True);

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ReserveAsync(
                key, 0.001, 200));

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        Assert.That(info.Capacity, Is.EqualTo(100));
    }

    [Test]
    public async Task BF_Reserve_KeyAlreadyCreatedByAddAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ReserveAsync(
                key, 0.001, 100));

        Assert.That(await this._redis.Modules.Bloom.BF_ExistsAsync(
            key, "item1"), Is.True);
    }

    [Test]
    public async Task BF_Reserve_WrongKeyTypeAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ReserveAsync(
                key, 0.001, 100));
    }

    [Test]
    public async Task BF_Reserve_ErrorRate_ZeroAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ReserveAsync(
                key, 0, 100));
    }

    [Test]
    public async Task BF_Reserve_ErrorRate_NegativeAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ReserveAsync(
                key, -0.01, 100));
    }

    [Test]
    public async Task BF_Reserve_ErrorRate_GreaterThanOneAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ReserveAsync(
                key, 1.1, 100));
    }

    [Test]
    public async Task BF_Reserve_Capacity_ZeroAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ReserveAsync(
                key, 0.001, 0));
    }

    [Test]
    public async Task BF_Reserve_Capacity_NegativeAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ReserveAsync(
                key, 0.001, -1));
    }

    [Test]
    public async Task BF_Reserve_Capacity_TooLargeAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ReserveAsync(
                key, 0.001, 1073741825L));
    }

    [Test]
    public async Task BF_Reserve_Expansion_TooLargeAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ReserveAsync(
                key, 0.001, 100, expansion: 32769));
    }

    [Test]
    public async Task BF_Reserve_NonScaling_WithExpansionAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        await BF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Bloom.BF_ReserveAsync(
                key,
                errorRate: 0.001,
                capacity: 100,
                expansion: 2,
                nonscaling: true));
    }

    [Test]
    public async Task BF_Reserve_Flags_NoneAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100,
            flags: CommandFlags.None), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        Assert.That(info.Capacity, Is.EqualTo(100));
    }

    [Test]
    public async Task BF_Reserve_CancellationToken_DefaultAsync()
    {
        var key = $"Bloom:{{BF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_ReserveAsync(
            key, 0.001, 100,
            cancellationToken: CancellationToken.None), Is.True);

        var info = await this._redis.Modules.Bloom.BF_InfoAsync(key);

        Assert.That(info.Capacity, Is.EqualTo(100));
    }

    #endregion

}
