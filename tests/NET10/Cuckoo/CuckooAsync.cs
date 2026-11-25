using SharpRedis;
using SharpRedis.Exceptions;
using SharpRedis.Modules;

namespace NET10.Cuckoo;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class CuckooAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region CF_Add

    private static async Task CF_AddRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task CF_Add_AutoCreateAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "item1"), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Buckets, Is.GreaterThan(0));
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "item1"), Is.True);
    }

    [Test]
    public async Task CF_Add_FirstInsertAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello"), Is.GreaterThanOrEqualTo(1));
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(1));
    }

    [Test]
    public async Task CF_Add_DuplicateAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);
        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(3));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(count, Is.GreaterThanOrEqualTo(3));
        }
    }

    [Test]
    public async Task CF_Add_MultipleItemsAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(20));

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task CF_Add_ItemsIncreaseOnDuplicateAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 1; i <= 5; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "same-item"), Is.True);

            var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

            Assert.That(info.Items, Is.EqualTo(i));
        }
    }

    [Test]
    public async Task CF_Add_CompareWithAddNxAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.False);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(2));
    }

    [Test]
    public async Task CF_Add_UnicodeAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "你好，世界🌍"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "你好，世界🌍"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "你好，世界🌍"), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(2));
    }

    [Test]
    public async Task CF_Add_EmptyItemAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, ""), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, ""), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, ""), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(2));
    }

    [Test]
    public async Task CF_Add_SpecialCharactersAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a b c"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a\nb\tc"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a:b/c?d=e"), Is.True);
        }

        var first = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "a b c");

        var second = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "a\nb\tc");

        var third = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "a:b/c?d=e");

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.True);
            Assert.That(second, Is.True);
            Assert.That(third, Is.True);
            Assert.That(info.Items, Is.EqualTo(3));
        }
    }

    [Test]
    public async Task CF_Add_CaseSensitiveAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "Hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "HELLO"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(3));
    }

    [Test]
    public async Task CF_Add_ReservedFilterAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key,
                    capacity: 100,
                    bucketSize: 4,
                    maxIterations: 50,
                    expansion: 2), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.BucketSize, Is.EqualTo(4));
            Assert.That(info.MaxIterations, Is.EqualTo(50));
            Assert.That(info.Expansion, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_Add_NonScaling_FullAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 4,
            bucketSize: 1,
            maxIterations: 1,
            expansion: 0), Is.True);

        int succeeded = 0;
        bool full = false;

        for (int i = 0; i < 100; i++)
        {
            try
            {
                Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
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

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(succeeded));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    [Test]
    public async Task CF_Add_AutoScalingAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(20));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task CF_Add_WrongKeyTypeAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CF_AddRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"));
    }

    [Test]
    public async Task CF_Add_BloomKeyWrongTypeAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        await CF_AddRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item2"));
    }

    [Test]
    public async Task CF_Add_Flags_NoneAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "item1",
            flags: CommandFlags.None), Is.True);

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "item1",
            flags: CommandFlags.None), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(2));
    }

    [Test]
    public async Task CF_Add_CancellationToken_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_AddAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.True);

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(2));
    }

    #endregion

    #region CF_AddNx

    private static async Task CF_AddNxRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task CF_AddNx_AutoCreateAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "item1"), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Size, Is.GreaterThan(0));
            Assert.That(info.Buckets, Is.GreaterThan(0));
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "item1"), Is.True);
    }

    [Test]
    public async Task CF_AddNx_FirstInsertAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "hello"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(1));
    }

    [Test]
    public async Task CF_AddNx_DuplicateAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "hello"), Is.False);

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "hello"), Is.False);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
        }
    }

    [Test]
    public async Task CF_AddNx_Duplicate_DoesNotIncreaseItemsAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "item1"), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "item1"), Is.False);
        }

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before.Items, Is.EqualTo(1));
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
        }
    }

    [Test]
    public async Task CF_AddNx_MultipleDifferentItemsAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        long added = 0;

        for (int i = 0; i < 20; i++)
        {
            if (await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, $"item:{i}"))
            {
                added++;
            }
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(added, Is.GreaterThan(0));
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.EqualTo(1));
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task CF_AddNx_CompareWithAddAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.False);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.False);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);
        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(count, Is.GreaterThanOrEqualTo(2));
        }
    }

    [Test]
    public async Task CF_AddNx_AfterDeleteAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello"), Is.Zero);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "hello"), Is.True);
        }
    }

    [Test]
    public async Task CF_AddNx_EmptyItemAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, ""), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                    key, ""), Is.False);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, ""), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(1));
    }

    [Test]
    public async Task CF_AddNx_UnicodeAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "你好，世界🌍"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                    key, "你好，世界🌍"), Is.False);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "你好，世界🌍"), Is.True);
        }
    }

    [Test]
    public async Task CF_AddNx_SpecialCharactersAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "a b c"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "a\nb\tc"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "a:b/c?d=e"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "a b c"), Is.False);

        var first = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "a b c");

        var second = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "a\nb\tc");

        var third = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "a:b/c?d=e");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.True);
            Assert.That(second, Is.True);
            Assert.That(third, Is.True);
        }
    }

    [Test]
    public async Task CF_AddNx_CaseSensitiveAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "Hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "HELLO"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "Hello"), Is.False);

        var first = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "Hello");

        var second = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "hello");

        var third = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "HELLO");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.True);
            Assert.That(second, Is.True);
            Assert.That(third, Is.True);
        }
    }

    [Test]
    public async Task CF_AddNx_ReservedFilterAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key,
                    capacity: 100,
                    bucketSize: 4,
                    maxIterations: 50,
                    expansion: 2), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "item1"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.BucketSize, Is.EqualTo(4));
            Assert.That(info.MaxIterations, Is.EqualTo(50));
            Assert.That(info.Expansion, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_AddNx_NonScaling_FullAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
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
                if (await this._redis.Modules.Cuckoo.CF_AddNxAsync(
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

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(added, Is.GreaterThan(0));
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    [Test]
    public async Task CF_AddNx_NonScaling_ExistingItemAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key,
                    capacity: 4,
                    bucketSize: 1,
                    maxIterations: 1,
                    expansion: 0), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "hello"), Is.False);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "hello"), Is.False);

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(after.Items, Is.EqualTo(before.Items));
    }

    [Test]
    public async Task CF_AddNx_AutoScalingAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        long added = 0;

        for (int i = 0; i < 20; i++)
        {
            if (await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, $"item:{i}"))
            {
                added++;
            }
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task CF_AddNx_WrongKeyTypeAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CF_AddNxRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "item1"));
    }

    [Test]
    public async Task CF_AddNx_BloomKeyWrongTypeAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        await CF_AddNxRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "item1"));
    }

    [Test]
    public async Task CF_AddNx_Flags_NoneAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "item1",
            flags: CommandFlags.None), Is.True);

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "item1",
            flags: CommandFlags.None), Is.False);
    }

    [Test]
    public async Task CF_AddNx_CancellationToken_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_AddNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.True);

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.False);
    }

    #endregion

    #region CF_Count

    [Test]
    public async Task CF_Count_KeyNotExistsAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                    key, "item1"), Is.Zero);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "item2"), Is.Zero);
        }
    }

    [Test]
    public async Task CF_Count_EmptyFilterAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "item1"), Is.Zero);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, ""), Is.Zero);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.Zero);
    }

    [Test]
    public async Task CF_Count_SingleItemAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        Assert.That(count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task CF_Count_DuplicateItemsAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 1; i <= 5; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);

            var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello");

            Assert.That(count, Is.GreaterThanOrEqualTo(i));
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(5));
    }

    [Test]
    public async Task CF_Count_MultipleItemsAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item2"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "item2"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item3"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "item3"), Is.True);

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "item3"), Is.True);

        var count1 = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "item1");

        var count2 = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "item2");

        var count3 = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "item3");

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count1, Is.GreaterThanOrEqualTo(1));
            Assert.That(count2, Is.GreaterThanOrEqualTo(2));
            Assert.That(count3, Is.GreaterThanOrEqualTo(3));
            Assert.That(info.Items, Is.EqualTo(6));
        }
    }

    [Test]
    public async Task CF_Count_AddNx_DuplicateAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.True);
        }

        var before = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        for (int i = 0; i < 5; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.False);
        }

        var after = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.GreaterThanOrEqualTo(1));
            Assert.That(after, Is.EqualTo(before));
            Assert.That(info.Items, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_Count_AfterDeleteOnceAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        var before = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
            key, "hello"), Is.True);

        var after = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.GreaterThanOrEqualTo(3));
            Assert.That(after, Is.EqualTo(before - 1));
            Assert.That(after, Is.GreaterThanOrEqualTo(2));
        }
    }

    [Test]
    public async Task CF_Count_AfterDeleteAllAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                    key, "hello"), Is.Zero);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "hello"), Is.False);
        }
    }

    [Test]
    public async Task CF_Count_DeleteOtherItemAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "target"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "target"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "other"), Is.True);
        }

        var before = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "target");

        Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
            key, "other"), Is.True);

        var after = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "target");

        Assert.That(after, Is.EqualTo(before));
    }

    [Test]
    public async Task CF_Count_DeleteAndReAddAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello"), Is.Zero);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello"), Is.GreaterThanOrEqualTo(1));
        }
    }

    [Test]
    public async Task CF_Count_UnicodeAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "你好🌍"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "你好🌍"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "你好🌍"), Is.GreaterThanOrEqualTo(2));
        }
    }

    [Test]
    public async Task CF_Count_EmptyItemAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, ""), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, ""), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, ""), Is.GreaterThanOrEqualTo(2));
        }
    }

    [Test]
    public async Task CF_Count_SpecialCharactersAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a b c"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a\nb\tc"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "a\nb\tc"), Is.True);

        var count1 = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "a b c");

        var count2 = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "a\nb\tc");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count1, Is.GreaterThanOrEqualTo(1));
            Assert.That(count2, Is.GreaterThanOrEqualTo(2));
        }
    }

    [Test]
    public async Task CF_Count_CaseSensitiveAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "Hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "HELLO"), Is.True);
        }

        var count1 = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "Hello");

        var count2 = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        var count3 = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "HELLO");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count1, Is.GreaterThanOrEqualTo(1));
            Assert.That(count2, Is.GreaterThanOrEqualTo(2));
            Assert.That(count3, Is.GreaterThanOrEqualTo(1));
        }
    }

    [Test]
    public async Task CF_Count_ReadOnlyAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello"), Is.GreaterThanOrEqualTo(1));
        }

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.DeletedItems, Is.EqualTo(before.DeletedItems));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
        }
    }

    [Test]
    public async Task CF_Count_AfterScalingAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Filters, Is.GreaterThan(1));

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, $"item:{i}"), Is.GreaterThanOrEqualTo(1));
        }
    }

    [Test]
    public async Task CF_Count_Flags_NoneAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello",
                flags: CommandFlags.None), Is.GreaterThanOrEqualTo(1));
        }
    }

    [Test]
    public async Task CF_Count_CancellationToken_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_CountAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello",
                cancellationToken: CancellationToken.None),
                Is.GreaterThanOrEqualTo(1));
        }
    }

    #endregion

    #region CF_Del

    private static async Task CF_DelRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task CF_Del_KeyNotExistsAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        await CF_DelRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "item1"));
    }

    [Test]
    public async Task CF_Del_EmptyFilterAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "item1"), Is.False);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "item1"), Is.Zero);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_Del_SingleItemAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello"), Is.Zero);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "hello"), Is.False);
        }
    }

    [Test]
    public async Task CF_Del_DuplicateDeleteAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                    key, "hello"), Is.False);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello"), Is.Zero);
        }
    }

    [Test]
    public async Task CF_Del_DuplicateItem_DeleteOnceAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        var before = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
            key, "hello"), Is.True);

        var after = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        var exists = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.GreaterThanOrEqualTo(3));
            Assert.That(after, Is.EqualTo(before - 1));
            Assert.That(after, Is.GreaterThanOrEqualTo(2));
            Assert.That(exists, Is.True);
        }
    }

    [Test]
    public async Task CF_Del_DuplicateItem_DeleteAllAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);
        }

        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        var exists = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "hello");

        var deletedAgain = await this._redis.Modules.Cuckoo.CF_DelAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.Zero);
            Assert.That(exists, Is.False);
            Assert.That(deletedAgain, Is.False);
        }
    }

    [Test]
    public async Task CF_Del_MultipleItems_DeleteOneAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item2"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item3"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "item2"), Is.True);
        }

        var deletedCount = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "item2");

        var exists1 = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "item1");

        var exists3 = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "item3");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedCount, Is.Zero);
            Assert.That(exists1, Is.True);
            Assert.That(exists3, Is.True);
        }
    }

    [Test]
    public async Task CF_Del_DeleteAndReAddAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello"), Is.Zero);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello"), Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task CF_Del_InfoDeletedItemsAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 5; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                    key, "item:1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "item:3"), Is.True);
        }

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before.DeletedItems, Is.Zero);
            Assert.That(after.DeletedItems,
                Is.EqualTo(before.DeletedItems + 2));

            Assert.That(after.Filters, Is.EqualTo(before.Filters));
        }
    }

    [Test]
    public async Task CF_Del_UnicodeAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "你好🌍"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "你好🌍"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "你好🌍"), Is.Zero);
        }
    }

    [Test]
    public async Task CF_Del_EmptyItemAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, ""), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, ""), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, ""), Is.Zero);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
            key, ""), Is.False);
    }

    [Test]
    public async Task CF_Del_SpecialCharactersAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a b c"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a\nb\tc"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "a\nb\tc"), Is.True);
        }

        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "a\nb\tc");

        var exists = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "a b c");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.Zero);
            Assert.That(exists, Is.True);
        }
    }

    [Test]
    public async Task CF_Del_CaseSensitiveAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "Hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "HELLO"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);
        }

        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        var upper = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "Hello");

        var allCaps = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "HELLO");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.Zero);
            Assert.That(upper, Is.True);
            Assert.That(allCaps, Is.True);
        }
    }

    [Test]
    public async Task CF_Del_AfterScalingAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(before.Filters, Is.GreaterThan(1));

        for (int i = 0; i < 20; i += 2)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, $"item:{i}"), Is.True);
        }

        for (int i = 1; i < 20; i += 2)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task CF_Del_NonScalingAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key,
                    capacity: 100,
                    bucketSize: 2,
                    expansion: 0), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "item1"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);
        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "item1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
            Assert.That(count, Is.Zero);
        }
    }

    [Test]
    public async Task CF_Del_FilterRemainsAfterDeleteAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "item1"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item2"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "item2"), Is.True);
        }
    }

    [Test]
    public async Task CF_Del_WrongKeyTypeAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CF_DelRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "item1"));
    }

    [Test]
    public async Task CF_Del_BloomKeyWrongTypeAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        await CF_DelRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "item1"));
    }

    [Test]
    public async Task CF_Del_Flags_NoneAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "item1",
                flags: CommandFlags.None), Is.True);
        }
    }

    [Test]
    public async Task CF_Del_CancellationToken_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_DelAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "item1",
                cancellationToken: CancellationToken.None), Is.True);
        }
    }

    #endregion

    #region CF_Exists

    [Test]
    public async Task CF_Exists_KeyNotExistsAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                    key, "item1"), Is.False);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, ""), Is.False);
        }
    }

    [Test]
    public async Task CF_Exists_EmptyFilterAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "item1"), Is.False);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "item2"), Is.False);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.Zero);
    }

    [Test]
    public async Task CF_Exists_AutoCreateAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                    key, "hello"), Is.False);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "hello"), Is.True);
    }

    [Test]
    public async Task CF_Exists_SingleItemAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "item1"), Is.True);
        }
    }

    [Test]
    public async Task CF_Exists_MultipleItemsAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(20));
    }

    [Test]
    public async Task CF_Exists_DuplicateItemsAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 5; i++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                            key, "hello"), Is.True);

                Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                    key, "hello"), Is.True);
            }
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);
        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(5));
            Assert.That(count, Is.GreaterThanOrEqualTo(5));
        }
    }

    [Test]
    public async Task CF_Exists_AfterAddNxAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                    key, "hello"), Is.False);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "hello"), Is.True);
        }
    }

    [Test]
    public async Task CF_Exists_AfterDeleteAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                    key, "hello"), Is.False);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello"), Is.Zero);
        }
    }

    [Test]
    public async Task CF_Exists_Duplicate_DeleteOnceAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
            key, "hello"), Is.True);

        var exists = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "hello");

        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exists, Is.True);
            Assert.That(count, Is.GreaterThanOrEqualTo(2));
        }
    }

    [Test]
    public async Task CF_Exists_Duplicate_DeleteAllAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);
        }

        var exists = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "hello");

        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exists, Is.False);
            Assert.That(count, Is.Zero);
        }
    }

    [Test]
    public async Task CF_Exists_DeleteAndReAddAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "hello"), Is.False);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "hello"), Is.True);
        }
    }

    [Test]
    public async Task CF_Exists_UnicodeAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "你好，世界🌍"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "你好，世界🌍"), Is.True);
        }
    }

    [Test]
    public async Task CF_Exists_EmptyItemAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, ""), Is.False);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, ""), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, ""), Is.True);
    }

    [Test]
    public async Task CF_Exists_SpecialCharactersAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a b c"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a\nb\tc"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a:b/c?d=e"), Is.True);
        }

        var first = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "a b c");

        var second = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "a\nb\tc");

        var third = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "a:b/c?d=e");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.True);
            Assert.That(second, Is.True);
            Assert.That(third, Is.True);
        }
    }

    [Test]
    public async Task CF_Exists_CaseSensitiveAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "Hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "HELLO"), Is.True);
        }

        var first = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "Hello");

        var second = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "hello");

        var third = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "HELLO");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.True);
            Assert.That(second, Is.True);
            Assert.That(third, Is.True);
        }
    }

    [Test]
    public async Task CF_Exists_DifferentKeysAsync()
    {
        var key1 = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key1, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key2, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key1, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key2, "item2"), Is.True);
        }

        var exists1 = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key1, "item1");

        var exists2 = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key2, "item2");

        var info1 = await this._redis.Modules.Cuckoo.CF_InfoAsync(key1);
        var info2 = await this._redis.Modules.Cuckoo.CF_InfoAsync(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exists1, Is.True);
            Assert.That(exists2, Is.True);
            Assert.That(info1.Items, Is.EqualTo(1));
            Assert.That(info2.Items, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_Exists_ReadOnlyAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "hello"), Is.True);
        }

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.DeletedItems, Is.EqualTo(before.DeletedItems));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
            Assert.That(after.Size, Is.EqualTo(before.Size));
        }
    }

    [Test]
    public async Task CF_Exists_AfterScalingAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, capacity: 4, bucketSize: 2, expansion: 2), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Filters, Is.GreaterThan(1));

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task CF_Exists_NonScalingAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, capacity: 100, bucketSize: 2, expansion: 0), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item2"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        var exists1 = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "item1");

        var exists2 = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "item2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
            Assert.That(exists1, Is.True);
            Assert.That(exists2, Is.True);
        }
    }

    [Test]
    public async Task CF_Exists_WrongKeyTypeAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "item1"), Is.False);
    }

    [Test]
    public async Task CF_Exists_BloomKeyWrongTypeAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "item1"), Is.False);
        }
    }

    [Test]
    public async Task CF_Exists_Flags_NoneAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "item1",
                flags: CommandFlags.None), Is.True);
        }
    }

    [Test]
    public async Task CF_Exists_CancellationToken_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_ExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, "item1",
                cancellationToken: CancellationToken.None), Is.True);
        }
    }

    #endregion

    #region CF_Info

    private static async Task CF_InfoRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task CF_Info_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

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

    [Test]
    public async Task CF_Info_AutoCreatedAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "item1"), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

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

    [Test]
    public async Task CF_Info_CustomOptionsAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 1000,
            bucketSize: 4,
            maxIterations: 50,
            expansion: 4), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

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

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(8)]
    [TestCase(16)]
    public async Task CF_Info_BucketSizeAsync(int bucketSize)
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1024, bucketSize: bucketSize), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.BucketSize, Is.EqualTo(bucketSize));
            Assert.That(info.Buckets, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    [TestCase(1)]
    [TestCase(10)]
    [TestCase(50)]
    [TestCase(100)]
    [TestCase(65535)]
    public async Task CF_Info_MaxIterationsAsync(int maxIterations)
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100,
            maxIterations: maxIterations), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.MaxIterations, Is.EqualTo(maxIterations));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(8)]
    public async Task CF_Info_ExpansionAsync(int expansion)
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100,
            expansion: expansion), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

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

    [TestCase(3, 4)]
    [TestCase(5, 8)]
    [TestCase(7, 8)]
    public async Task CF_Info_Expansion_RoundedAsync(
        int expansion, int expected)
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100,
            expansion: expansion), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Expansion, Is.EqualTo(expected));
    }

    [Test]
    public async Task CF_Info_EmptyFilterAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.Zero);
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Size, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task CF_Info_AfterSingleAddAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_Info_AfterMultipleAddsAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(20));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_Info_DuplicateItemsAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 5; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(5));
            Assert.That(info.DeletedItems, Is.Zero);
        }
    }

    [Test]
    public async Task CF_Info_AddNx_DuplicateAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
                key, "hello"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddNxAsync(
            key, "hello"), Is.False);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
        }
    }

    [Test]
    public async Task CF_Info_AfterDeleteAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item2"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item3"), Is.True);
        }

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
            key, "item2"), Is.True);

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before.Items, Is.EqualTo(3));
            Assert.That(before.DeletedItems, Is.Zero);
            Assert.That(after.DeletedItems, Is.EqualTo(1));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
        }
    }

    [Test]
    public async Task CF_Info_MultipleDeletesAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 5; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                    key, "item:1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "item:3"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.DeletedItems, Is.EqualTo(2));
    }

    [Test]
    public async Task CF_Info_DeleteAndReAddAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);
        }

        var deleted = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted.DeletedItems, Is.EqualTo(1));

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        var readded = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(readded.Items, Is.GreaterThan(0));
            Assert.That(readded.Filters, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_Info_AfterScalingAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        for (int i = 0; i < 40; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Filters, Is.GreaterThanOrEqualTo(before.Filters));
            Assert.That(after.Buckets, Is.GreaterThanOrEqualTo(before.Buckets));
            Assert.That(after.Size, Is.GreaterThanOrEqualTo(before.Size));
            Assert.That(after.Items, Is.EqualTo(40));
            Assert.That(after.Expansion, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task CF_Info_NonScalingAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key,
                    capacity: 100,
                    bucketSize: 2,
                    expansion: 0), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
            Assert.That(info.Items, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_Info_ReadOnlyAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        for (int i = 0; i < 10; i++)
        {
            var current = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(current.Items, Is.EqualTo(before.Items));
                Assert.That(current.DeletedItems,
                    Is.EqualTo(before.DeletedItems));
            }
        }

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

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

    [Test]
    public async Task CF_Info_KeyNotExistsAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        await CF_InfoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InfoAsync(key));
    }

    [Test]
    public async Task CF_Info_WrongKeyTypeAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CF_InfoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InfoAsync(key));
    }

    [Test]
    public async Task CF_Info_BloomKeyWrongTypeAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.True);

        await CF_InfoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InfoAsync(key));
    }

    [Test]
    public async Task CF_Info_Flags_NoneAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(
            key, flags: CommandFlags.None);

        Assert.That(info.Filters, Is.EqualTo(1));
    }

    [Test]
    public async Task CF_Info_CancellationToken_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_InfoAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(
            key, cancellationToken: CancellationToken.None);

        Assert.That(info.Filters, Is.EqualTo(1));
    }

    #endregion

    #region CF_Insert

    private static async Task CF_InsertRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task CF_Insert_Single_AutoCreateAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, "item1"), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);
        var exists = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "item1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(exists, Is.True);
        }
    }

    [Test]
    public async Task CF_Insert_Multiple_AutoCreateAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(3));

        for (int i = 1; i <= 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                key, $"item{i}"), Is.True);
        }
    }

    [Test]
    public async Task CF_Insert_Single_ExistingFilterAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "hello"), Is.True);
        }

        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.GreaterThanOrEqualTo(1));
            Assert.That(info.Items, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_Insert_Multiple_ExistingFilterAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["a", "b", "c", "d", "e"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true, true
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(5));
    }

    [Test]
    public async Task CF_Insert_Single_DuplicateAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "hello"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);
        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(3));
            Assert.That(count, Is.GreaterThanOrEqualTo(3));
        }
    }

    [Test]
    public async Task CF_Insert_Multiple_DuplicateAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["hello", "hello", "world", "hello"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);
        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(4));
            Assert.That(count, Is.GreaterThanOrEqualTo(3));
        }
    }

    [Test]
    public async Task CF_Insert_SingleAndMultipleAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "a"), Is.True);
        }

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["b", "c", "a"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(4));
    }

    [Test]
    public async Task CF_Insert_Single_CustomCapacityAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, "item1", capacity: 1000), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Buckets, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task CF_Insert_Multiple_CustomCapacityAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key,
            ["item1", "item2", "item3"],
            capacity: 1000);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(3));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Buckets, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task CF_Insert_Capacity_IgnoredForExistingFilterAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100, bucketSize: 4), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["a", "b"], capacity: 1000);

        Assert.That(result, Is.EqualTo([true, true]));

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.BucketSize, Is.EqualTo(4));
            Assert.That(after.Filters, Is.EqualTo(1));
            Assert.That(after.Items, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task CF_Insert_Single_NoCreate_KeyNotExistsAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        await CF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "item1", nocreate: true));
    }

    [Test]
    public async Task CF_Insert_Multiple_NoCreate_KeyNotExistsAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        await CF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, ["item1", "item2"], nocreate: true));
    }

    [Test]
    public async Task CF_Insert_Single_NoCreate_ExistingFilterAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "item1", nocreate: true), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(1));
    }

    [Test]
    public async Task CF_Insert_Multiple_NoCreate_ExistingFilterAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["item1", "item2", "item3"], nocreate: true);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(3));
    }

    [Test]
    public async Task CF_Insert_Single_FilterFullAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key,
                    capacity: 2,
                    bucketSize: 1,
                    expansion: 0), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "same1"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
                    key, "same2"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "same3"), Is.False);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    [Test]
    public async Task CF_Insert_Multiple_FilterFullAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 2,
            bucketSize: 1,
            expansion: 0), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["same1", "same2", "same3", "same4"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, false, false
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    [Test]
    public async Task CF_Insert_Multiple_PartialSuccessAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key,
                    capacity: 2,
                    bucketSize: 1,
                    expansion: 0), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "same1"), Is.True);
        }

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["same2", "same3", "same4"]);

        Assert.That(result, Is.EqualTo(
        [
            true, false, false
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(2));
    }

    [Test]
    public async Task CF_Insert_FilterFull_AfterDeleteAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key,
                    capacity: 2,
                    bucketSize: 1,
                    expansion: 0), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "same1"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, "same2"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
                    key, "same3"), Is.False);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "same1"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, "same4"), Is.True);
    }

    [Test]
    public async Task CF_Insert_AutoScalingAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        for (int i = 0; i < 30; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, $"item:{i}"), Is.True);
        }

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Filters, Is.GreaterThan(before.Filters));
            Assert.That(after.Items, Is.EqualTo(30));
            Assert.That(after.Expansion, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task CF_Insert_Multiple_SpecialItemsAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["", "你好🌍", "a b c", "a\nb\tc", "a:b/c"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true, true
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(5));
    }

    [Test]
    public async Task CF_Insert_Multiple_CaseSensitiveAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["Hello", "hello", "HELLO"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(3));
    }

    [Test]
    public async Task CF_Insert_Capacity_ZeroAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        await CF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "item1", capacity: 0));
    }

    [Test]
    public async Task CF_Insert_Capacity_NegativeAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        await CF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, ["item1", "item2"], capacity: -1));
    }

    [Test]
    public async Task CF_Insert_Capacity_TooLargeAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        await CF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "item1", capacity: 1073741825));
    }

    [Test]
    public async Task CF_Insert_WrongKeyTypeAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "item1"));

        await CF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, ["item1", "item2"]));
    }

    [Test]
    public async Task CF_Insert_BloomKeyWrongTypeAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        await CF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, "item2"));

        await CF_InsertRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertAsync(
                key, ["item2", "item3"]));
    }

    [Test]
    public async Task CF_Insert_Flags_NoneAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, "item1",
            flags: CommandFlags.None), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["item1", "item2"],
            flags: CommandFlags.None);

        Assert.That(result, Is.EqualTo([true, true]));
    }

    [Test]
    public async Task CF_Insert_CancellationToken_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_InsertAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, "item1",
            cancellationToken: CancellationToken.None), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertAsync(
            key, ["item1", "item2"],
            cancellationToken: CancellationToken.None);

        Assert.That(result, Is.EqualTo([true, true]));
    }

    #endregion

    #region CF_InsertNx

    private static async Task CF_InsertNxRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task CF_InsertNx_Single_AutoCreateAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, "item1");

        Assert.That(result, Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);
        var exists = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "item1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.DeletedItems, Is.Zero);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(exists, Is.True);
        }
    }

    [Test]
    public async Task CF_InsertNx_Multiple_AutoCreateAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(3));
    }

    [Test]
    public async Task CF_InsertNx_Single_ExistingFilterAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, "hello"),
                Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(1));
    }

    [Test]
    public async Task CF_InsertNx_Multiple_ExistingFilterAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, ["a", "b", "c"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(3));
    }

    [Test]
    public async Task CF_InsertNx_Single_DuplicateAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, "hello"),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        for (int i = 0; i < 5; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, "hello"),
                Is.EqualTo(RedisCuckoo.InsertNxStatus.Exists));
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(1));
    }

    [Test]
    public async Task CF_InsertNx_Multiple_DuplicateAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, ["hello", "hello", "hello"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Exists,
            RedisCuckoo.InsertNxStatus.Exists
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(1));
    }

    [Test]
    public async Task CF_InsertNx_Multiple_MixedStatusAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "c"), Is.True);
        }

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, ["a", "b", "c", "d", "a"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Exists,
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Exists,
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Exists
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(4));
    }

    [Test]
    public async Task CF_InsertNx_SingleAndMultipleAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, "a"),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, ["a", "b", "b"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                RedisCuckoo.InsertNxStatus.Exists,
                RedisCuckoo.InsertNxStatus.Added,
                RedisCuckoo.InsertNxStatus.Exists
            ]));

            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, "b"),
                Is.EqualTo(RedisCuckoo.InsertNxStatus.Exists));
        }
    }

    [Test]
    public async Task CF_InsertNx_Single_CustomCapacityAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, "item1", capacity: 1000),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(1));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Buckets, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task CF_InsertNx_Multiple_CustomCapacityAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, ["a", "b", "c"], capacity: 1000);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(3));
    }

    [Test]
    public async Task CF_InsertNx_Capacity_IgnoredForExistingFilterAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100, bucketSize: 4), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, "item1", capacity: 1000),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.BucketSize, Is.EqualTo(4));
            Assert.That(after.Filters, Is.EqualTo(1));
            Assert.That(after.Items, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_InsertNx_Single_NoCreate_KeyNotExistsAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        await CF_InsertNxRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, "item1", nocreate: true));
    }

    [Test]
    public async Task CF_InsertNx_Multiple_NoCreate_KeyNotExistsAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        await CF_InsertNxRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, ["a", "b"], nocreate: true));
    }

    [Test]
    public async Task CF_InsertNx_Single_NoCreate_ExistingFilterAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, "item1", nocreate: true),
                Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, "item1", nocreate: true),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Exists));
    }

    [Test]
    public async Task CF_InsertNx_Multiple_NoCreate_ExistingFilterAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, ["a", "a", "b"], nocreate: true);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Exists,
            RedisCuckoo.InsertNxStatus.Added
        ]));
    }

    [Test]
    public async Task CF_InsertNx_Single_FilterFullAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 4,
            bucketSize: 1,
            maxIterations: 1,
            expansion: 0), Is.True);

        int added = 0;
        bool foundFull = false;

        for (int i = 0; i < 1000; i++)
        {
            var status = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
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

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    [Test]
    public async Task CF_InsertNx_Multiple_FilterFullAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 4,
            bucketSize: 1,
            maxIterations: 1,
            expansion: 0), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
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

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(added, Is.GreaterThan(0));
            Assert.That(full, Is.GreaterThan(0));
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    [Test]
    public async Task CF_InsertNx_FilterFull_ExistingItemAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key,
                    capacity: 4,
                    bucketSize: 1,
                    maxIterations: 1,
                    expansion: 0), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, "known"),
                Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));
        }

        for (int i = 0; i < 1000; i++)
        {
            var status = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, $"other:{i}");

            if (status == RedisCuckoo.InsertNxStatus.Full)
                break;
        }

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, "known"),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Exists));

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(after.Items, Is.EqualTo(before.Items));
    }

    [Test]
    public async Task CF_InsertNx_Multiple_OrderAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
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

    [Test]
    public async Task CF_InsertNx_AutoScalingAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        long added = 0;

        for (int i = 0; i < 50; i++)
        {
            var status = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, $"item:{i}");

            Assert.That(status, Is.Not.EqualTo(
                RedisCuckoo.InsertNxStatus.Full));

            if (status == RedisCuckoo.InsertNxStatus.Added)
                added++;
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(added));
            Assert.That(info.Filters, Is.GreaterThan(1));
            Assert.That(info.Expansion, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task CF_InsertNx_AfterDeleteAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                    key, "hello"),
                    Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_CountAsync(
                key, "hello"), Is.Zero);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, "hello"),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));
    }

    [Test]
    public async Task CF_InsertNx_Multiple_SpecialItemsAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, ["", "你好🌍", "a b c", "a\nb\tc"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(4));
    }

    [Test]
    public async Task CF_InsertNx_CaseSensitiveAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, ["Hello", "hello", "HELLO"]);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added,
            RedisCuckoo.InsertNxStatus.Added
        ]));
    }

    [Test]
    public async Task CF_InsertNx_Capacity_ZeroAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        await CF_InsertNxRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, "item1", capacity: 0));
    }

    [Test]
    public async Task CF_InsertNx_Capacity_NegativeAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        await CF_InsertNxRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, ["a", "b"], capacity: -1));
    }

    [Test]
    public async Task CF_InsertNx_Capacity_TooLargeAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        await CF_InsertNxRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, "item1", capacity: 1073741825));
    }

    [Test]
    public async Task CF_InsertNx_WrongKeyTypeAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CF_InsertNxRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, "item1"));

        await CF_InsertNxRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, ["a", "b"]));
    }

    [Test]
    public async Task CF_InsertNx_BloomKeyWrongTypeAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        await CF_InsertNxRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, "item2"));

        await CF_InsertNxRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
                key, ["a", "b"]));
    }

    [Test]
    public async Task CF_InsertNx_Flags_NoneAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, "hello",
            flags: CommandFlags.None),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, ["hello", "hello"],
            flags: CommandFlags.None);

        Assert.That(result, Is.EqualTo(
        [
            RedisCuckoo.InsertNxStatus.Exists,
            RedisCuckoo.InsertNxStatus.Exists
        ]));
    }

    [Test]
    public async Task CF_InsertNx_CancellationToken_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_InsertNxAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
            key, "hello",
            cancellationToken: CancellationToken.None),
            Is.EqualTo(RedisCuckoo.InsertNxStatus.Added));

        var result = await this._redis.Modules.Cuckoo.CF_InsertNxAsync(
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

    private static async Task CF_ScanDump_LoadChunkRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    /// <summary>
    /// 异步完整导出 Cuckoo Filter。
    /// </summary>
    private async Task<List<(long Iterator, byte[] Data)>> CF_ScanDumpAllAsync(
        string key)
    {
        var chunks = new List<(long Iterator, byte[] Data)>();
        long iterator = 0;

        for (int i = 0; i < 1024; i++)
        {
            var result = await this._redis.Modules.Cuckoo.CF_ScanDumpAsync(
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
    /// 异步恢复所有数据块。
    /// </summary>
    private async Task CF_LoadAllChunksAsync(
        string key,
        List<(long Iterator, byte[] Data)> chunks)
    {
        foreach (var (Iterator, Data) in chunks)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_LoadChunkAsync(
                key, Iterator, Data), Is.True);
        }
    }

    [Test]
    public async Task CF_ScanDump_LoadChunk_SingleItemAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            source, "hello"), Is.True);

        var chunks = await this.CF_ScanDumpAllAsync(source);

        Assert.That(chunks, Has.Count.GreaterThanOrEqualTo(2));

        await this.CF_LoadAllChunksAsync(target, chunks);

        var sourceExists = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            source, "hello");
        var targetExists = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            target, "hello");
        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            target, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sourceExists, Is.True);
            Assert.That(targetExists, Is.True);
            Assert.That(count, Is.GreaterThanOrEqualTo(1));
        }
    }

    [Test]
    public async Task CF_ScanDump_LoadChunk_DuplicateItemsAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            source, 1000), Is.True);

        for (int i = 0; i < 5; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                source, "hello"), Is.True);
        }

        var beforeCount = await this._redis.Modules.Cuckoo.CF_CountAsync(
            source, "hello");

        var chunks = await this.CF_ScanDumpAllAsync(source);
        await this.CF_LoadAllChunksAsync(target, chunks);

        var afterCount = await this._redis.Modules.Cuckoo.CF_CountAsync(
            target, "hello");

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(beforeCount, Is.GreaterThanOrEqualTo(5));
            Assert.That(afterCount, Is.EqualTo(beforeCount));
            Assert.That(info.Items, Is.EqualTo(5));
        }
    }

    [Test]
    public async Task CF_ScanDump_LoadChunk_MultipleItemsAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    source, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                source, "a"), Is.True);
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                source, "b"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    source, "b"), Is.True);
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                source, "c"), Is.True);
        }
        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            source, "c"), Is.True);
        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            source, "c"), Is.True);

        var chunks = await this.CF_ScanDumpAllAsync(source);
        await this.CF_LoadAllChunksAsync(target, chunks);

        foreach (var item in new[] { "a", "b", "c" })
        {
            var beforeCount = await this._redis.Modules.Cuckoo.CF_CountAsync(
                source, item);
            var afterCount = await this._redis.Modules.Cuckoo.CF_CountAsync(
                target, item);

            Assert.That(afterCount, Is.EqualTo(beforeCount));
        }

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(source);
        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.DeletedItems, Is.EqualTo(before.DeletedItems));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
        }
    }

    [Test]
    public async Task CF_ScanDump_LoadChunk_CustomOptionsAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    source,
                    capacity: 1000,
                    bucketSize: 4,
                    maxIterations: 50,
                    expansion: 4), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                source, "item1"), Is.True);
        }

        var chunks = await this.CF_ScanDumpAllAsync(source);
        await this.CF_LoadAllChunksAsync(target, chunks);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(source);
        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(target);

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

    [Test]
    public async Task CF_ScanDump_LoadChunk_ScalingFilterAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            source,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        for (int i = 0; i < 100; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                source, $"item:{i}"), Is.True);
        }

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(source);

        Assert.That(before.Filters, Is.GreaterThan(1));

        var chunks = await this.CF_ScanDumpAllAsync(source);
        await this.CF_LoadAllChunksAsync(target, chunks);

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.Expansion, Is.EqualTo(before.Expansion));
        }

        for (int i = 0; i < 100; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                target, $"item:{i}"), Is.True);
        }
    }

    [Test]
    public async Task CF_ScanDump_LoadChunk_NonScalingAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    source,
                    capacity: 100,
                    bucketSize: 2,
                    expansion: 0), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                source, "item1"), Is.True);
        }

        var chunks = await this.CF_ScanDumpAllAsync(source);
        await this.CF_LoadAllChunksAsync(target, chunks);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(target);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
            Assert.That(info.Items, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_ScanDump_LoadChunk_AfterDeleteAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    source, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                source, "a"), Is.True);
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                source, "b"), Is.True);
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                source, "c"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                source, "b"), Is.True);
        }

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(source);

        var chunks = await this.CF_ScanDumpAllAsync(source);
        await this.CF_LoadAllChunksAsync(target, chunks);

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(target);

        var a = await this._redis.Modules.Cuckoo.CF_ExistsAsync(target, "a");
        var c = await this._redis.Modules.Cuckoo.CF_ExistsAsync(target, "c");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.DeletedItems, Is.EqualTo(before.DeletedItems));
            Assert.That(a, Is.True);
            Assert.That(c, Is.True);
        }
    }

    [Test]
    public async Task CF_ScanDump_LoadChunk_SpecialItemsAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            source, 1000), Is.True);

        foreach (var item in new[]
        {
            "", "你好🌍", "a b c", "a\nb\tc", "HELLO"
        })
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                source, item), Is.True);
        }

        var chunks = await this.CF_ScanDumpAllAsync(source);
        await this.CF_LoadAllChunksAsync(target, chunks);

        foreach (var item in new[]
        {
            "", "你好🌍", "a b c", "a\nb\tc", "HELLO"
        })
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
                target, item), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(target);

        Assert.That(info.Items, Is.EqualTo(5));
    }

    [Test]
    public async Task CF_ScanDump_IteratorAndCompletionAsync()
    {
        var key = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var first = await this._redis.Modules.Cuckoo.CF_ScanDumpAsync(
            key, 0);

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
            var chunk = await this._redis.Modules.Cuckoo.CF_ScanDumpAsync(
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

    [Test]
    public async Task CF_ScanDump_RepeatDumpAsync()
    {
        var key = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var first = await this.CF_ScanDumpAllAsync(key);
        var second = await this.CF_ScanDumpAllAsync(key);

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

    [Test]
    public async Task CF_ScanDump_KeyNotExistsAsync()
    {
        var key = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";

        await CF_ScanDump_LoadChunkRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ScanDumpAsync(
                key, 0));
    }

    [Test]
    public async Task CF_ScanDump_WrongKeyTypeAsync()
    {
        var key = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CF_ScanDump_LoadChunkRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ScanDumpAsync(
                key, 0));
    }

    [Test]
    public async Task CF_ScanDump_BloomKeyWrongTypeAsync()
    {
        var key = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.True);

        await CF_ScanDump_LoadChunkRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ScanDumpAsync(
                key, 0));
    }

    [Test]
    public async Task CF_LoadChunk_WrongKeyTypeAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            source, "hello"), Is.True);

        var chunks = await this.CF_ScanDumpAllAsync(source);

        Assert.That(chunks, Is.Not.Empty);

        await this._redis.String.SetAsync(target, "hello");

        await CF_ScanDump_LoadChunkRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_LoadChunkAsync(
                target, chunks[0].Iterator, chunks[0].Data));
    }

    [Test]
    public async Task CF_LoadChunk_BloomKeyWrongTypeAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            source, "hello"), Is.True);

        var chunks = await this.CF_ScanDumpAllAsync(source);

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            target, "hello"), Is.True);

        await CF_ScanDump_LoadChunkRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_LoadChunkAsync(
                target, chunks[0].Iterator, chunks[0].Data));
    }

    [Test]
    public async Task CF_LoadChunk_InvalidHeaderAsync()
    {
        var key = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        await CF_ScanDump_LoadChunkRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_LoadChunkAsync(
                key, 1, [1, 2, 3, 4, 5]));
    }

    [Test]
    public async Task CF_LoadChunk_MissingHeaderAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            source, "hello"), Is.True);

        var chunks = await this.CF_ScanDumpAllAsync(source);

        Assert.That(chunks, Has.Count.GreaterThanOrEqualTo(2));

        await CF_ScanDump_LoadChunkRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_LoadChunkAsync(
                target, chunks[1].Iterator, chunks[1].Data));
    }

    [Test]
    public async Task CF_ScanDump_LoadChunk_Flags_NoneAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            source, "hello"), Is.True);

        long iterator = 0;

        for (int i = 0; i < 1024; i++)
        {
            var chunk = await this._redis.Modules.Cuckoo.CF_ScanDumpAsync(
                source, iterator, flags: CommandFlags.None);

            if (chunk.IsCompleted)
                break;

            Assert.That(await this._redis.Modules.Cuckoo.CF_LoadChunkAsync(
                target, chunk.Iterator, chunk.Data!,
                flags: CommandFlags.None), Is.True);

            iterator = chunk.Iterator;
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            target, "hello"), Is.True);
    }

    [Test]
    public async Task CF_ScanDump_LoadChunk_CancellationToken_DefaultAsync()
    {
        var source = $"Cuckoo:{{CF_ScanDumpAsync}}:{Guid.NewGuid():N}";
        var target = $"Cuckoo:{{CF_LoadChunkAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            source, "hello"), Is.True);

        long iterator = 0;

        for (int i = 0; i < 1024; i++)
        {
            var chunk = await this._redis.Modules.Cuckoo.CF_ScanDumpAsync(
                source, iterator,
                cancellationToken: CancellationToken.None);

            if (chunk.IsCompleted)
                break;

            Assert.That(await this._redis.Modules.Cuckoo.CF_LoadChunkAsync(
                target, chunk.Iterator, chunk.Data!,
                cancellationToken: CancellationToken.None), Is.True);

            iterator = chunk.Iterator;
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            target, "hello"), Is.True);
    }

    #endregion

    #region CF_MExists

    [Test]
    public async Task CF_MExists_Single_KeyNotExistsAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, "item1"), Is.False);
    }

    [Test]
    public async Task CF_MExists_Multiple_KeyNotExistsAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo([false, false, false]));
    }

    [Test]
    public async Task CF_MExists_Single_EmptyFilterAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 100), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_MExistsAsync(
                key, "hello"), Is.False);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.Zero);
    }

    [Test]
    public async Task CF_MExists_Multiple_EmptyFilterAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["a", "b", "c", ""]);

        Assert.That(result, Is.EqualTo(
        [
            false, false, false, false
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.Zero);
    }

    [Test]
    public async Task CF_MExists_Single_ExistingItemAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_MExistsAsync(
                key, "hello"), Is.True);
        }
    }

    [Test]
    public async Task CF_MExists_Multiple_SingleItemAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["hello"]);

        Assert.That(result, Is.EqualTo([true]));
    }

    [Test]
    public async Task CF_MExists_Multiple_ExistingItemsAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item2"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item3"), Is.True);
        }

        var result = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["item1", "item2", "item3"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    [Test]
    public async Task CF_MExists_Multiple_ResultOrderAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["c", "a", "b"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before, Is.EqualTo(
                [
                    false, false, false
                ]));

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "b"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "c"), Is.True);
        }

        var after = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["c", "a", "b"]);

        Assert.That(after, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    [Test]
    public async Task CF_MExists_Multiple_DuplicateQueriesAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var result = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["hello", "hello", "hello", "hello"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true
        ]));

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.EqualTo(1));
    }

    [Test]
    public async Task CF_MExists_DuplicateInsertAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        var single = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, "hello");

        var multiple = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["hello", "hello"]);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(single, Is.True);
            Assert.That(multiple, Is.EqualTo([true, true]));
            Assert.That(info.Items, Is.EqualTo(3));
        }
    }

    [Test]
    public async Task CF_MExists_AfterDeleteAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);
        }

        var single = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, "hello");

        var multiple = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["hello", "hello"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(single, Is.False);
            Assert.That(multiple, Is.EqualTo([false, false]));
        }
    }

    [Test]
    public async Task CF_MExists_Duplicate_DeleteOnceAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
            key, "hello"), Is.True);

        var single = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, "hello");

        var multiple = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["hello", "hello"]);

        var count = await this._redis.Modules.Cuckoo.CF_CountAsync(
            key, "hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(single, Is.True);
            Assert.That(multiple, Is.EqualTo([true, true]));
            Assert.That(count, Is.GreaterThanOrEqualTo(2));
        }
    }

    [Test]
    public async Task CF_MExists_Duplicate_DeleteAllAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);
        }

        for (int i = 0; i < 3; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);
        }

        var single = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, "hello");

        var multiple = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["hello"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(single, Is.False);
            Assert.That(multiple, Is.EqualTo([false]));
        }
    }

    [Test]
    public async Task CF_MExists_DeleteAndReAddAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_DelAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_MExistsAsync(
                key, "hello"), Is.False);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_MExistsAsync(
                key, "hello"), Is.True);
        }
    }

    [Test]
    public async Task CF_MExists_CompareWithExistsAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "b"), Is.True);
        }

        var result = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["a", "b", "c", "d"]);

        var existsA = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "a");

        var existsB = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "b");

        var existsC = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "c");

        var existsD = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "d");

        var single = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, "a");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(4));
            Assert.That(result[0], Is.EqualTo(existsA));
            Assert.That(result[1], Is.EqualTo(existsB));
            Assert.That(result[2], Is.EqualTo(existsC));
            Assert.That(result[3], Is.EqualTo(existsD));
            Assert.That(single, Is.EqualTo(result[0]));
        }
    }

    [Test]
    public async Task CF_MExists_SpecialItemsAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, ""), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "你好🌍"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a b c"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "a\nb\tc"), Is.True);
        }

        var result = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["", "你好🌍", "a b c", "a\nb\tc"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true
        ]));
    }

    [Test]
    public async Task CF_MExists_CaseSensitiveAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "Hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "hello"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "HELLO"), Is.True);
        }

        var result = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["HELLO", "Hello", "hello"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true
        ]));
    }

    [Test]
    public async Task CF_MExists_ReadOnlyAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_MExistsAsync(
                key, "hello"), Is.True);

            var result = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
                key, ["hello", "hello"]);

            Assert.That(result, Is.EqualTo([true, true]));
        }

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Items, Is.EqualTo(before.Items));
            Assert.That(after.DeletedItems, Is.EqualTo(before.DeletedItems));
            Assert.That(after.Filters, Is.EqualTo(before.Filters));
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.Size, Is.EqualTo(before.Size));
        }
    }

    [Test]
    public async Task CF_MExists_AfterScalingAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, capacity: 4, bucketSize: 2, expansion: 2), Is.True);

        for (int i = 0; i < 50; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Filters, Is.GreaterThan(1));

        var result = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["item:0", "item:10", "item:25", "item:49"]);

        Assert.That(result, Is.EqualTo(
        [
            true, true, true, true
        ]));
    }

    [Test]
    public async Task CF_MExists_NonScalingAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, capacity: 100, expansion: 0), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item2"), Is.True);
        }

        var result = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["item1", "item2"]);

        Assert.That(result, Is.EqualTo([true, true]));
    }

    [Test]
    public async Task CF_MExists_DifferentKeysAsync()
    {
        var key1 = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key1, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key2, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key1, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key2, "item2"), Is.True);
        }

        var result1 = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key1, ["item1"]);

        var result2 = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key2, ["item2"]);

        var info1 = await this._redis.Modules.Cuckoo.CF_InfoAsync(key1);
        var info2 = await this._redis.Modules.Cuckoo.CF_InfoAsync(key2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Is.EqualTo([true]));
            Assert.That(result2, Is.EqualTo([true]));
            Assert.That(info1.Items, Is.EqualTo(1));
            Assert.That(info2.Items, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task CF_MExists_WrongKeyTypeAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        var single = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, "item1");

        var multiple = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["item1", "item2"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(single, Is.False);
            Assert.That(multiple, Is.EqualTo([false, false]));
        }
    }

    [Test]
    public async Task CF_MExists_BloomKeyWrongTypeAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "item1"), Is.True);

        var single = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, "item1");

        var multiple = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["item1", "item2"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(single, Is.False);
            Assert.That(multiple, Is.EqualTo([false, false]));
        }
    }

    [Test]
    public async Task CF_MExists_Flags_NoneAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var single = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, "hello", flags: CommandFlags.None);

        var multiple = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["hello", "hello"], flags: CommandFlags.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(single, Is.True);
            Assert.That(multiple, Is.EqualTo([true, true]));
        }
    }

    [Test]
    public async Task CF_MExists_CancellationToken_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_MExistsAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        var single = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, "hello",
            cancellationToken: CancellationToken.None);

        var multiple = await this._redis.Modules.Cuckoo.CF_MExistsAsync(
            key, ["hello", "hello"],
            cancellationToken: CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(single, Is.True);
            Assert.That(multiple, Is.EqualTo([true, true]));
        }
    }

    #endregion

    #region CF_Reserve

    private static async Task CF_ReserveRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task CF_Reserve_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1000), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

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

    [Test]
    public async Task CF_Reserve_CustomOptionsAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 1000,
            bucketSize: 4,
            maxIterations: 50,
            expansion: 4), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

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

    [TestCase(2, 1)]
    [TestCase(4, 2)]
    [TestCase(8, 4)]
    [TestCase(16, 8)]
    [TestCase(510, 255)]
    public async Task CF_Reserve_MinimumCapacityAsync(
        int capacity, int bucketSize)
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, capacity, bucketSize: bucketSize), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.BucketSize, Is.EqualTo(bucketSize));
            Assert.That(info.Buckets, Is.GreaterThan(0));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Items, Is.Zero);
        }
    }

    [TestCase(0, 2)]
    [TestCase(-1, 2)]
    [TestCase(1, 1)]
    [TestCase(3, 2)]
    [TestCase(7, 4)]
    [TestCase(509, 255)]
    public async Task CF_Reserve_Capacity_TooSmallAsync(
        int capacity, int bucketSize)
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        await CF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key, capacity, bucketSize: bucketSize));
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(8)]
    [TestCase(16)]
    [TestCase(128)]
    [TestCase(255)]
    public async Task CF_Reserve_BucketSize_ValidAsync(int bucketSize)
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 1024, bucketSize: bucketSize), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.BucketSize, Is.EqualTo(bucketSize));
            Assert.That(info.Buckets, Is.GreaterThan(0));
            Assert.That(info.Items, Is.Zero);
        }
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(256)]
    [TestCase(1000)]
    public async Task CF_Reserve_BucketSize_InvalidAsync(int bucketSize)
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        await CF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key, 1024, bucketSize: bucketSize));
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(10)]
    [TestCase(20)]
    [TestCase(100)]
    [TestCase(65535)]
    public async Task CF_Reserve_MaxIterations_ValidAsync(int maxIterations)
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100,
            maxIterations: maxIterations), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.MaxIterations, Is.EqualTo(maxIterations));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(65536)]
    public async Task CF_Reserve_MaxIterations_InvalidAsync(int maxIterations)
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        await CF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key, 100,
                maxIterations: maxIterations));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(8)]
    [TestCase(32768)]
    public async Task CF_Reserve_Expansion_ValidAsync(int expansion)
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100,
            expansion: expansion), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

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

    [TestCase(3, 4)]
    [TestCase(5, 8)]
    [TestCase(6, 8)]
    [TestCase(7, 8)]
    [TestCase(9, 16)]
    [TestCase(100, 128)]
    public async Task CF_Reserve_Expansion_RoundedAsync(
        int expansion, int expected)
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100,
            expansion: expansion), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Expansion, Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(32769)]
    [TestCase(65536)]
    public async Task CF_Reserve_Expansion_InvalidAsync(int expansion)
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        await CF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key, 100,
                expansion: expansion));
    }

    [Test]
    public async Task CF_Reserve_Capacity_TooLargeAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        await CF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key, 1073741825));
    }

    [Test]
    public async Task CF_Reserve_KeyAlreadyExistsAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        await CF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key, 1000));

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Buckets, Is.EqualTo(before.Buckets));
            Assert.That(after.Size, Is.EqualTo(before.Size));
            Assert.That(after.BucketSize, Is.EqualTo(before.BucketSize));
            Assert.That(after.Items, Is.Zero);
        }
    }

    [Test]
    public async Task CF_Reserve_KeyAlreadyAutoCreatedAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
            key, "hello"), Is.True);

        await CF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key, 100));

        Assert.That(await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "hello"), Is.True);
    }

    [Test]
    public async Task CF_Reserve_AfterInsertAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key, 1000), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item1"), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, "item2"), Is.True);
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);
        var exists1 = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "item1");
        var exists2 = await this._redis.Modules.Cuckoo.CF_ExistsAsync(
            key, "item2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Items, Is.EqualTo(2));
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(exists1, Is.True);
            Assert.That(exists2, Is.True);
        }
    }

    [Test]
    public async Task CF_Reserve_NonScalingAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
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
                Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                    key, $"item:{i}"), Is.True);
            }
            catch (RedisErrorException)
            {
                full = true;
                break;
            }
        }

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(full, Is.True);
            Assert.That(info.Filters, Is.EqualTo(1));
            Assert.That(info.Expansion, Is.Zero);
        }
    }

    [Test]
    public async Task CF_Reserve_AutoScalingAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key,
            capacity: 4,
            bucketSize: 2,
            expansion: 2), Is.True);

        var before = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        for (int i = 0; i < 40; i++)
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_AddAsync(
                key, $"item:{i}"), Is.True);
        }

        var after = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(after.Filters, Is.GreaterThan(before.Filters));
            Assert.That(after.Items, Is.EqualTo(40));
            Assert.That(after.Expansion, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task CF_Reserve_DifferentKeysAsync()
    {
        var key1 = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";
        var key2 = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                    key1, 100, bucketSize: 2, expansion: 1), Is.True);

            Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key2, 100, bucketSize: 4, expansion: 4), Is.True);
        }

        var info1 = await this._redis.Modules.Cuckoo.CF_InfoAsync(key1);
        var info2 = await this._redis.Modules.Cuckoo.CF_InfoAsync(key2);

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

    [Test]
    public async Task CF_Reserve_WrongKeyTypeAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "hello");

        await CF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key, 100));
    }

    [Test]
    public async Task CF_Reserve_BloomKeyWrongTypeAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Bloom.BF_AddAsync(
            key, "hello"), Is.True);

        await CF_ReserveRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Modules.Cuckoo.CF_ReserveAsync(
                key, 100));
    }

    [Test]
    public async Task CF_Reserve_Flags_NoneAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100,
            flags: CommandFlags.None), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Items, Is.Zero);
    }

    [Test]
    public async Task CF_Reserve_CancellationToken_DefaultAsync()
    {
        var key = $"Cuckoo:{{CF_ReserveAsync}}:{Guid.NewGuid():N}";

        Assert.That(await this._redis.Modules.Cuckoo.CF_ReserveAsync(
            key, 100,
            cancellationToken: CancellationToken.None), Is.True);

        var info = await this._redis.Modules.Cuckoo.CF_InfoAsync(key);

        Assert.That(info.Filters, Is.EqualTo(1));
    }

    #endregion
}
