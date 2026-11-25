using SharpRedis;
using SharpRedis.Exceptions;
using static SharpRedis.RedisKeyspace;

namespace NET10.Keyspace;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class KeyspaceAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public async Task ExistsAsync()
    {
        const string key = "Keyspace:{ExistsAsync}:Key";
        const string nonekey = "Keyspace:{ExistsAsync}:NoneKey";

        var exists = await this._redis.Key.ExistsAsync(key);
        Assert.That(exists, Is.False);

        var existsArray = await this._redis.Key.ExistsAsync([key, nonekey]);
        Assert.That(existsArray, Is.Zero);

        _ = await this._redis.String.SetAsync(key, "1");

        exists = await this._redis.Key.ExistsAsync(key);
        Assert.That(exists, Is.True);

        existsArray = await this._redis.Key.ExistsAsync([key, nonekey]);
        Assert.That(existsArray, Is.EqualTo(1));
    }

    [Test]
    public async Task CopyAsync()
    {
        const string key = "Keyspace:{CopyAsync}:Key";
        const string destkey = "Keyspace:{CopyAsync}:DestKey";

        var exists = await this._redis.Key.ExistsAsync(destkey);
        Assert.That(exists, Is.False);

        exists = await this._redis.Key.ExistsAsync(key);
        Assert.That(exists, Is.False);

        _ = await this._redis.String.SetAsync(key, "This Copy");

        exists = await this._redis.Key.ExistsAsync(key);
        Assert.That(exists, Is.True);

        exists = await this._redis.Key.ExistsAsync(destkey);
        Assert.That(exists, Is.False);

        var copyOk = await this._redis.Key.CopyAsync(key, destkey);
        Assert.That(copyOk, Is.True);

        exists = await this._redis.Key.ExistsAsync(destkey);
        Assert.That(exists, Is.True);

        string? copyValue = await this._redis.String.GetAsync(destkey);
        Assert.NotNull(copyValue);
        Assert.That(copyValue, Is.EqualTo("This Copy"));

        using (var db1 = this._redis.Select(1))
        {
            exists = await db1.Key.ExistsAsync(destkey);
            Assert.That(exists, Is.False);

            copyOk = await this._redis.Key.CopyAsync(key, destkey, 1);
            Assert.That(copyOk, Is.True);

            exists = await db1.Key.ExistsAsync(destkey);
            Assert.That(exists, Is.True);

            copyValue = await db1.String.GetAsync(destkey);
            Assert.NotNull(copyValue);
            Assert.That(copyValue, Is.EqualTo("This Copy"));
        }

        using (var db2 = this._redis.Select(2))
        {
            _ = await db2.List.LPushAsync(destkey, "db2");

            exists = await db2.Key.ExistsAsync(destkey);
            Assert.That(exists, Is.True);

            copyOk = await this._redis.Key.CopyAsync(key, destkey, 2);
            Assert.That(copyOk, Is.False);

            copyOk = await this._redis.Key.CopyAsync(key, destkey, 2, true);
            Assert.That(copyOk, Is.True);

            copyValue = await db2.String.GetAsync(destkey);
            Assert.NotNull(copyValue);
            Assert.That(copyValue, Is.EqualTo("This Copy"));
        }
    }

    [Test]
    public async Task DelAsync()
    {
        const string key1 = "Keyspace:{DelAsync}:Key1";
        const string key2 = "Keyspace:{DelAsync}:Key2";

        var single = await this._redis.Key.DelAsync(key1);
        Assert.IsFalse(single);

        _ = await this._redis.String.SetAsync(key1, "key1");

        single = await this._redis.Key.DelAsync(key1);
        Assert.IsTrue(single);

        var arr = await this._redis.Key.DelAsync([key1, key2]);
        Assert.Zero(arr);

        _ = await this._redis.String.SetAsync(key1, "key1");
        _ = this._redis.String.Set(key2, "key2");

        arr = await this._redis.Key.DelAsync([key1, key2]);
        Assert.That(arr, Is.EqualTo(2));

        _ = await this._redis.String.SetAsync(key2, "key2");

        arr = await this._redis.Key.DelAsync([key1, key2]);
        Assert.That(arr, Is.EqualTo(1));
    }

    [Test]
    public async Task RestoreAsync()
    {
        const string key = "Keyspace:{RestoreAsync}:Key";
        const string reskey = "Keyspace:{RestoreAsync}:ResKey";

        var exists = await this._redis.Key.ExistsAsync(reskey);
        Assert.That(exists, Is.False);

        _ = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue> { { "h1", "h1" } });

        var bytes = await this._redis.Key.DumpAsync(key);
        Assert.IsNotNull(bytes);

        var restore = await this._redis.Key.RestoreAsync(reskey, bytes);
        Assert.IsTrue(restore);

        string? h1 = await this._redis.Hash.HGetAsync(reskey, "h1");
        Assert.That(h1, Is.EqualTo("h1"));

        restore = await this._redis.Key.RestoreAsync(reskey, bytes, 60 * 1000, true);
        Assert.IsTrue(restore);

        var ttl = await this._redis.Key.TtlAsync(reskey);
        Assert.That(ttl >= 50 && ttl <= 60);

        restore = await this._redis.Key.RestoreAsync(reskey, bytes, DateTime.Now.AddMinutes(10), true, 50);
        Assert.IsTrue(restore);

        ttl = await this._redis.Key.TtlAsync(reskey);
        Assert.That(ttl >= 590 && ttl <= 600);

        h1 = await this._redis.Hash.HGetAsync(reskey, "h1");
        Assert.That(h1, Is.EqualTo("h1"));
    }

    [Test]
    public async Task ExpireAsync()
    {
        const string key = "Keyspace:{ExpireAsync}:Key";

        var ok = await this._redis.Key.ExpireAsync(key, 60);
        Assert.IsFalse(ok);

        ok = await this._redis.Key.ExpireAsync(key, 60, RedisKeyspace.ExpireCondition.NX);
        Assert.IsFalse(ok);

        _ = await this._redis.String.SetAsync(key, "Expire");

        ok = await this._redis.Key.ExpireAsync(key, 60);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.ExpireAsync(key, 60, RedisKeyspace.ExpireCondition.NX);
        Assert.IsFalse(ok);

        ok = await this._redis.Key.ExpireAsync(key, 60, RedisKeyspace.ExpireCondition.XX);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.ExpireAsync(key, 30, RedisKeyspace.ExpireCondition.GT);
        Assert.IsFalse(ok);

        ok = await this._redis.Key.ExpireAsync(key, 30, RedisKeyspace.ExpireCondition.LT);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.ExpireAsync(key, TimeSpan.FromHours(1), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);
    }

    [Test]
    public async Task PExpireAsync()
    {
        const string key = "Keyspace:{PExpireAsync}:Key";

        var ok = await this._redis.Key.PExpireAsync(key, 3000);
        Assert.IsFalse(ok);

        ok = await this._redis.Key.PExpireAsync(key, TimeSpan.FromMinutes(1), RedisKeyspace.ExpireCondition.NX);
        Assert.IsFalse(ok);

        _ = await this._redis.String.SetAsync(key, "Expire");

        ok = await this._redis.Key.PExpireAsync(key, TimeSpan.FromMinutes(1));
        Assert.IsTrue(ok);

        ok = await this._redis.Key.PExpireAsync(key, 60 * 1000, RedisKeyspace.ExpireCondition.NX);
        Assert.IsFalse(ok);

        ok = await this._redis.Key.PExpireAsync(key, 60 * 1000, RedisKeyspace.ExpireCondition.XX);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.PExpireAsync(key, 30 * 1000, RedisKeyspace.ExpireCondition.GT);
        Assert.IsFalse(ok);

        ok = await this._redis.Key.PExpireAsync(key, 30 * 1000, RedisKeyspace.ExpireCondition.LT);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.PExpireAsync(key, TimeSpan.FromHours(1), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);

        var time = await this._redis.Key.ExpireTimeAsync(key);
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
        Assert.That(dateTime, Is.GreaterThanOrEqualTo(DateTime.Now.AddHours(1).AddSeconds(-5)));
    }

    [Test]
    public async Task ExpireAtAsync()
    {
        const string key = "Keyspace:{ExpireAtAsync}:Key";

        var ok = await this._redis.Key.ExpireAtAsync(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeSeconds());
        Assert.IsFalse(ok);

        _ = await this._redis.String.SetAsync(key, "ExpireAt");

        ok = await this._redis.Key.ExpireAtAsync(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeSeconds(), RedisKeyspace.ExpireCondition.NX);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.ExpireAtAsync(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeSeconds(), RedisKeyspace.ExpireCondition.XX);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.ExpireAtAsync(key, DateTimeOffset.UtcNow.AddSeconds(5).ToUnixTimeSeconds(), RedisKeyspace.ExpireCondition.LT);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.ExpireAtAsync(key, DateTimeOffset.UtcNow.AddSeconds(50).ToUnixTimeSeconds(), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.ExpireAtAsync(key, DateTime.Now.AddSeconds(60), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);

        var time = await this._redis.Key.ExpireTimeAsync(key);
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
        Assert.That(dateTime, Is.GreaterThanOrEqualTo(DateTime.Now.AddSeconds(57)));
    }

    [Test]
    public async Task PExpireAtAsync()
    {
        const string key = "Keyspace:{PExpireAtAsync}:Key";

        var ok = await this._redis.Key.PExpireAtAsync(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeMilliseconds());
        Assert.IsFalse(ok);

        _ = await this._redis.String.SetAsync(key, "ExpireAt");

        ok = await this._redis.Key.PExpireAtAsync(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeMilliseconds(), RedisKeyspace.ExpireCondition.NX);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.PExpireAtAsync(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeMilliseconds(), RedisKeyspace.ExpireCondition.XX);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.PExpireAtAsync(key, DateTimeOffset.UtcNow.AddSeconds(5).ToUnixTimeMilliseconds(), RedisKeyspace.ExpireCondition.LT);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.PExpireAtAsync(key, DateTimeOffset.UtcNow.AddSeconds(50).ToUnixTimeMilliseconds(), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);

        ok = await this._redis.Key.PExpireAtAsync(key, DateTime.Now.AddSeconds(60), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);

        var time = await this._redis.Key.ExpireTimeAsync(key);
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
        Assert.That(dateTime, Is.GreaterThanOrEqualTo(DateTime.Now.AddSeconds(57)));
    }

    [Test]
    public async Task ExpireTimeAsync()
    {
        const string key = "Keyspace:{ExpireTimeAsync}:Key";

        var time = await this._redis.Key.ExpireTimeAsync(key);
        Assert.That(time, Is.EqualTo(-2));

        _ = await this._redis.String.SetAsync(key, "ExpireAt");

        time = await this._redis.Key.ExpireTimeAsync(key);
        Assert.That(time, Is.EqualTo(-1));

        _ = await this._redis.Key.ExpireAtAsync(key, DateTime.Now.AddSeconds(60));

        time = await this._redis.Key.ExpireTimeAsync(key);
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
        Assert.That(dateTime, Is.GreaterThanOrEqualTo(DateTime.Now.AddSeconds(57)));
    }

    [Test]
    public async Task PExpireTimeAsync()
    {
        const string key = "Keyspace:{PExpireTimeAsync}:Key";

        var time = await this._redis.Key.PExpireTimeAsync(key);
        Assert.That(time, Is.EqualTo(-2));

        _ = await this._redis.String.SetAsync(key, "ExpireAt");

        time = await this._redis.Key.PExpireTimeAsync(key);
        Assert.That(time, Is.EqualTo(-1));

        _ = await this._redis.Key.ExpireAtAsync(key, DateTime.Now.AddSeconds(60));

        time = await this._redis.Key.PExpireTimeAsync(key);
        var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(time).LocalDateTime;
        Assert.That(dateTime, Is.GreaterThanOrEqualTo(DateTime.Now.AddSeconds(57)));
    }

    #region Sort

    private static async Task SortRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    private static void SortAssertAsync(
        RedisResult[] result,
        params string[] expected)
    {
        Assert.That(result, Has.Length.EqualTo(expected.Length));

        for (int i = 0; i < expected.Length; i++)
        {
            string actual = result[i]!;
            Assert.That(actual, Is.EqualTo(expected[i]),
                $"SortAsync result index: {i}");
        }
    }

    private async Task SortSeedNumbersAsync(string key)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)await this._redis.List.RPushAsync(
                    key, "3"), Is.EqualTo(1));
            Assert.That((int)await this._redis.List.RPushAsync(
                key, "1"), Is.EqualTo(2));
            Assert.That((int)await this._redis.List.RPushAsync(
                key, "5"), Is.EqualTo(3));
            Assert.That((int)await this._redis.List.RPushAsync(
                key, "2"), Is.EqualTo(4));
            Assert.That((int)await this._redis.List.RPushAsync(
                key, "4"), Is.EqualTo(5));
        }
    }

    [Test]
    public async Task Sort_DefaultAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(key);

        SortAssertAsync(result!, "1", "2", "3", "4", "5");
    }

    [Test]
    public async Task Sort_AscendingAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, sort: SortDirection.Asc);

        SortAssertAsync(result!, "1", "2", "3", "4", "5");
    }

    [Test]
    public async Task Sort_DescendingAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, sort: SortDirection.Desc);

        SortAssertAsync(result!, "5", "4", "3", "2", "1");
    }

    [Test]
    public async Task Sort_Limit_FirstPageAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, offset: 0, count: 3);

        SortAssertAsync(result!, "1", "2", "3");
    }

    [Test]
    public async Task Sort_Limit_MiddlePageAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, offset: 2, count: 2);

        SortAssertAsync(result!, "3", "4");
    }

    [Test]
    public async Task Sort_Limit_DescendingAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, offset: 1, count: 3,
            sort: SortDirection.Desc);

        SortAssertAsync(result!, "4", "3", "2");
    }

    [Test]
    public async Task Sort_Limit_ZeroCountAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, offset: 0, count: 0);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Sort_Limit_OffsetOutOfRangeAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, offset: 100, count: 10);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Sort_Limit_CountNegativeOneAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, offset: 2, count: -1);

        SortAssertAsync(result!, "3", "4", "5");
    }

    [Test]
    public async Task Sort_AlphaAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this._redis.List.RPushAsync(key, "20");
        await this._redis.List.RPushAsync(key, "3");
        await this._redis.List.RPushAsync(key, "100");
        await this._redis.List.RPushAsync(key, "10");

        var result = await this._redis.Key.SortAsync(
            key, alpha: true);

        SortAssertAsync(result!, "10", "100", "20", "3");
    }

    [Test]
    public async Task Sort_Alpha_DescendingAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this._redis.List.RPushAsync(key, "apple");
        await this._redis.List.RPushAsync(key, "banana");
        await this._redis.List.RPushAsync(key, "cherry");

        var result = await this._redis.Key.SortAsync(
            key, sort: SortDirection.Desc, alpha: true);

        SortAssertAsync(result!, "cherry", "banana", "apple");
    }

    [Test]
    public async Task Sort_Get_SelfAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, get: ["#"]);

        SortAssertAsync(result!, "1", "2", "3", "4", "5");
    }

    [Test]
    public async Task Sort_Get_MultipleAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this._redis.List.RPushAsync(key, "2");
        await this._redis.List.RPushAsync(key, "1");

        var result = await this._redis.Key.SortAsync(
            key, get: ["#", "#"]);

        SortAssertAsync(result!, "1", "1", "2", "2");
    }

    [Test]
    public async Task Sort_By_NoSortAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, pattern: "nosort");

        SortAssertAsync(result!, "3", "1", "5", "2", "4");
    }

    [Test]
    public async Task Sort_KeyNotExistsAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Key.SortAsync(key);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Sort_EmptyResultAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Key.SortAsync(
            key, offset: 0, count: 10);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Sort_NonNumeric_WithoutAlphaAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this._redis.List.RPushAsync(key, "apple");
        await this._redis.List.RPushAsync(key, "banana");

        await SortRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Key.SortAsync(key));
    }

    [Test]
    public async Task Sort_WrongKeyTypeAsync()
    {
        var key = $"String:{{SortAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "123");

        await SortRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Key.SortAsync(key));
    }

    [Test]
    public async Task Sort_DuplicateValuesAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this._redis.List.RPushAsync(key, "2");
        await this._redis.List.RPushAsync(key, "1");
        await this._redis.List.RPushAsync(key, "2");
        await this._redis.List.RPushAsync(key, "1");

        var result = await this._redis.Key.SortAsync(key);

        SortAssertAsync(result!, "1", "1", "2", "2");
    }

    [Test]
    public async Task Sort_ReadOnlyDataAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(key);

        SortAssertAsync(result!, "1", "2", "3", "4", "5");

        var original = await this._redis.List.LRangeAsync(key, 0, -1);

        Assert.That(original!.Select(x => (string)x!),
            Is.EqualTo(["3", "1", "5", "2", "4"]));
    }

    [Test]
    public async Task Sort_Flags_NoneAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, flags: CommandFlags.None);

        SortAssertAsync(result!, "1", "2", "3", "4", "5");
    }

    [Test]
    public async Task Sort_CancellationToken_DefaultAsync()
    {
        var key = $"List:{{SortAsync}}:{Guid.NewGuid():N}";

        await this.SortSeedNumbersAsync(key);

        var result = await this._redis.Key.SortAsync(
            key, cancellationToken: CancellationToken.None);

        SortAssertAsync(result!, "1", "2", "3", "4", "5");
    }

    #endregion

    #region Sort_ExternalPatterns

    [Test]
    public async Task Sort_By_ExternalWeightsAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        await this._redis.List.RPushAsync(key, "1");
        await this._redis.List.RPushAsync(key, "2");
        await this._redis.List.RPushAsync(key, "3");

        await this._redis.String.SetAsync($"{prefix}weight:1", "30");
        await this._redis.String.SetAsync($"{prefix}weight:2", "10");
        await this._redis.String.SetAsync($"{prefix}weight:3", "20");

        var result = await this._redis.Key.SortAsync(
            key,
            pattern: $"{redisPrefix}{prefix}weight:*");

        SortAssertAsync(result!, "2", "3", "1");
    }

    [Test]
    public async Task Sort_Get_ExternalValuesAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        await this._redis.List.RPushAsync(key, "2");
        await this._redis.List.RPushAsync(key, "1");

        await this._redis.String.SetAsync($"{prefix}name:1", "Alice");
        await this._redis.String.SetAsync($"{prefix}name:2", "Bob");

        var result = await this._redis.Key.SortAsync(
            key,
            get:
            [
                "#",
                $"{redisPrefix}{prefix}name:*"
            ]);

        SortAssertAsync(result!, "1", "Alice", "2", "Bob");
    }

    [Test]
    public async Task Sort_Get_MissingExternalKeyAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        await this._redis.List.RPushAsync(key, "1");
        await this._redis.List.RPushAsync(key, "2");

        await this._redis.String.SetAsync($"{prefix}name:1", "Alice");

        var result = await this._redis.Key.SortAsync(
            key,
            get:
            [
                "#",
                $"{redisPrefix}{prefix}name:*"
            ]);

        Assert.That(result, Has.Length.EqualTo(4));

        using (Assert.EnterMultipleScope())
        {
            Assert.That((string)result[0]!, Is.EqualTo("1"));
            Assert.That((string)result[1]!, Is.EqualTo("Alice"));
            Assert.That((string)result[2]!, Is.EqualTo("2"));
            Assert.That(result[3].IsNull, Is.True);
        }
    }

    #endregion

    #region SortStore

    private static async Task SortStoreRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task SortStore_DefaultAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStoreAsync}}:{id}:source";
        var store = $"List:{{SortStoreAsync}}:{id}:dest";

        await this.SortSeedNumbersAsync(key);

        var count = await this._redis.Key.SortStoreAsync(key, store);

        var result = await this._redis.List.LRangeAsync(store, 0, -1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(5));
            Assert.That(result!.Select(x => (string)x!),
                Is.EqualTo(["1", "2", "3", "4", "5"]));
        }
    }

    [Test]
    public async Task SortStore_DescendingAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStoreAsync}}:{id}:source";
        var store = $"List:{{SortStoreAsync}}:{id}:dest";

        await this.SortSeedNumbersAsync(key);

        var count = await this._redis.Key.SortStoreAsync(
            key, store, sort: SortDirection.Desc);

        var result = await this._redis.List.LRangeAsync(store, 0, -1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(5));
            Assert.That(result!.Select(x => (string)x!),
                Is.EqualTo(["5", "4", "3", "2", "1"]));
        }
    }

    [Test]
    public async Task SortStore_LimitAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStoreAsync}}:{id}:source";
        var store = $"List:{{SortStoreAsync}}:{id}:dest";

        await this.SortSeedNumbersAsync(key);

        var count = await this._redis.Key.SortStoreAsync(
            key, store, offset: 1, count: 2);

        var result = await this._redis.List.LRangeAsync(store, 0, -1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(2));
            Assert.That(result!.Select(x => (string)x!),
                Is.EqualTo(["2", "3"]));
        }
    }

    [Test]
    public async Task SortStore_AlphaAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStoreAsync}}:{id}:source";
        var store = $"List:{{SortStoreAsync}}:{id}:dest";

        await this._redis.List.RPushAsync(key, "banana");
        await this._redis.List.RPushAsync(key, "apple");
        await this._redis.List.RPushAsync(key, "cherry");

        var count = await this._redis.Key.SortStoreAsync(
            key, store, alpha: true);

        var result = await this._redis.List.LRangeAsync(store, 0, -1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(3));
            Assert.That(result!.Select(x => (string)x!),
                Is.EqualTo(["apple", "banana", "cherry"]));
        }
    }

    [Test]
    public async Task SortStore_OverwriteDestinationAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStoreAsync}}:{id}:source";
        var store = $"List:{{SortStoreAsync}}:{id}:dest";

        await this.SortSeedNumbersAsync(key);
        await this._redis.List.RPushAsync(store, "old");

        Assert.That(await this._redis.Key.SortStoreAsync(
            key, store), Is.EqualTo(5));

        var result = await this._redis.List.LRangeAsync(store, 0, -1);

        Assert.That(result!.Select(x => (string)x!),
            Is.EqualTo(["1", "2", "3", "4", "5"]));
    }

    [Test]
    public async Task SortStore_OverwriteStringAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStoreAsync}}:{id}:source";
        var store = $"List:{{SortStoreAsync}}:{id}:dest";

        await this.SortSeedNumbersAsync(key);
        await this._redis.String.SetAsync(store, "old");

        Assert.That(await this._redis.Key.SortStoreAsync(
            key, store), Is.EqualTo(5));

        var result = await this._redis.List.LRangeAsync(store, 0, -1);

        Assert.That(result!.Select(x => (string)x!),
            Is.EqualTo(["1", "2", "3", "4", "5"]));
    }

    [Test]
    public async Task SortStore_SourceNotExistsAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStoreAsync}}:{id}:source";
        var store = $"List:{{SortStoreAsync}}:{id}:dest";

        var count = await this._redis.Key.SortStoreAsync(key, store);

        var result = await this._redis.List.LRangeAsync(store, 0, -1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.Zero);
            Assert.That(result, Is.Null);
        }
    }

    [Test]
    public async Task SortStore_ZeroCountAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStoreAsync}}:{id}:source";
        var store = $"List:{{SortStoreAsync}}:{id}:dest";

        await this.SortSeedNumbersAsync(key);

        Assert.That(await this._redis.Key.SortStoreAsync(
            key, store, offset: 0, count: 0), Is.Zero);
    }

    [Test]
    public async Task SortStore_Get_SelfAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStoreAsync}}:{id}:source";
        var store = $"List:{{SortStoreAsync}}:{id}:dest";

        await this.SortSeedNumbersAsync(key);

        Assert.That(await this._redis.Key.SortStoreAsync(
            key, store, get: ["#"]), Is.EqualTo(5));

        var result = await this._redis.List.LRangeAsync(store, 0, -1);

        Assert.That(result!.Select(x => (string)x!),
            Is.EqualTo(["1", "2", "3", "4", "5"]));
    }

    [Test]
    public async Task SortStore_WrongKeyTypeAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"String:{{SortStoreAsync}}:{id}:source";
        var store = $"String:{{SortStoreAsync}}:{id}:dest";

        await this._redis.String.SetAsync(key, "hello");

        await SortStoreRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Key.SortStoreAsync(
                key, store));
    }

    [Test]
    public async Task SortStore_Flags_NoneAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStoreAsync}}:{id}:source";
        var store = $"List:{{SortStoreAsync}}:{id}:dest";

        await this.SortSeedNumbersAsync(key);

        Assert.That(await this._redis.Key.SortStoreAsync(
            key, store, flags: CommandFlags.None),
            Is.EqualTo(5));
    }

    [Test]
    public async Task SortStore_CancellationToken_DefaultAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStoreAsync}}:{id}:source";
        var store = $"List:{{SortStoreAsync}}:{id}:dest";

        await this.SortSeedNumbersAsync(key);

        Assert.That(await this._redis.Key.SortStoreAsync(
            key, store,
            cancellationToken: CancellationToken.None),
            Is.EqualTo(5));
    }

    #endregion

    #region Sort_Ro

    private static async Task Sort_RoRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    private static void Sort_RoAssertAsync(
        RedisResult[]? result,
        params string[] expected)
    {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(expected.Length));

        for (int i = 0; i < expected.Length; i++)
        {
            string actual = result![i]!;
            Assert.That(actual, Is.EqualTo(expected[i]),
                $"SORT_RO Async result index: {i}");
        }
    }

    private async Task Sort_RoSeedNumbersAsync(string key)
    {
        await this._redis.List.RPushAsync(key, "3");
        await this._redis.List.RPushAsync(key, "1");
        await this._redis.List.RPushAsync(key, "5");
        await this._redis.List.RPushAsync(key, "2");
        await this._redis.List.RPushAsync(key, "4");
    }

    [Test]
    public async Task Sort_Ro_DefaultAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(key);

        Sort_RoAssertAsync(result, "1", "2", "3", "4", "5");
    }

    [Test]
    public async Task Sort_Ro_AscendingAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key, sort: SortDirection.Asc);

        Sort_RoAssertAsync(result, "1", "2", "3", "4", "5");
    }

    [Test]
    public async Task Sort_Ro_DescendingAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key, sort: SortDirection.Desc);

        Sort_RoAssertAsync(result, "5", "4", "3", "2", "1");
    }

    [Test]
    public async Task Sort_Ro_Limit_FirstPageAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key, offset: 0, count: 3);

        Sort_RoAssertAsync(result, "1", "2", "3");
    }

    [Test]
    public async Task Sort_Ro_Limit_MiddlePageAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key, offset: 2, count: 2);

        Sort_RoAssertAsync(result, "3", "4");
    }

    [Test]
    public async Task Sort_Ro_Limit_DescendingAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key,
            offset: 1,
            count: 3,
            sort: SortDirection.Desc);

        Sort_RoAssertAsync(result, "4", "3", "2");
    }

    [Test]
    public async Task Sort_Ro_Limit_ZeroCountAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key, offset: 0, count: 0);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Sort_Ro_Limit_OffsetOutOfRangeAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key, offset: 100, count: 10);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Sort_Ro_Limit_CountNegativeOneAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key, offset: 2, count: -1);

        Sort_RoAssertAsync(result, "3", "4", "5");
    }

    [Test]
    public async Task Sort_Ro_AlphaAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this._redis.List.RPushAsync(key, "20");
        await this._redis.List.RPushAsync(key, "3");
        await this._redis.List.RPushAsync(key, "100");
        await this._redis.List.RPushAsync(key, "10");

        var result = await this._redis.Key.Sort_RoAsync(
            key, alpha: true);

        Sort_RoAssertAsync(result, "10", "100", "20", "3");
    }

    [Test]
    public async Task Sort_Ro_Alpha_DescendingAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this._redis.List.RPushAsync(key, "apple");
        await this._redis.List.RPushAsync(key, "banana");
        await this._redis.List.RPushAsync(key, "cherry");

        var result = await this._redis.Key.Sort_RoAsync(
            key, sort: SortDirection.Desc, alpha: true);

        Sort_RoAssertAsync(result, "cherry", "banana", "apple");
    }

    [Test]
    public async Task Sort_Ro_Get_SelfAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key, get: ["#"]);

        Sort_RoAssertAsync(result, "1", "2", "3", "4", "5");
    }

    [Test]
    public async Task Sort_Ro_Get_MultipleAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this._redis.List.RPushAsync(key, "2");
        await this._redis.List.RPushAsync(key, "1");

        var result = await this._redis.Key.Sort_RoAsync(
            key, get: ["#", "#"]);

        Sort_RoAssertAsync(result, "1", "1", "2", "2");
    }

    [Test]
    public async Task Sort_Ro_By_NoSortAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key, pattern: "nosort");

        Sort_RoAssertAsync(result, "3", "1", "5", "2", "4");
    }

    [Test]
    public async Task Sort_Ro_By_ExternalWeightsAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort_Ro:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        await this._redis.List.RPushAsync(key, "1");
        await this._redis.List.RPushAsync(key, "2");
        await this._redis.List.RPushAsync(key, "3");

        await this._redis.String.SetAsync($"{prefix}weight:1", "30");
        await this._redis.String.SetAsync($"{prefix}weight:2", "10");
        await this._redis.String.SetAsync($"{prefix}weight:3", "20");

        var result = await this._redis.Key.Sort_RoAsync(
            key,
            pattern: $"{redisPrefix}{prefix}weight:*");

        Sort_RoAssertAsync(result, "2", "3", "1");
    }

    [Test]
    public async Task Sort_Ro_Get_ExternalValuesAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort_Ro:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        await this._redis.List.RPushAsync(key, "2");
        await this._redis.List.RPushAsync(key, "1");

        await this._redis.String.SetAsync($"{prefix}name:1", "Alice");
        await this._redis.String.SetAsync($"{prefix}name:2", "Bob");

        var result = await this._redis.Key.Sort_RoAsync(
            key,
            get: ["#", $"{redisPrefix}{prefix}name:*"]);

        Sort_RoAssertAsync(result, "1", "Alice", "2", "Bob");
    }

    [Test]
    public async Task Sort_Ro_Get_MissingExternalKeyAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort_Ro:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        await this._redis.List.RPushAsync(key, "1");
        await this._redis.List.RPushAsync(key, "2");

        await this._redis.String.SetAsync($"{prefix}name:1", "Alice");

        var result = await this._redis.Key.Sort_RoAsync(
            key,
            get: ["#", $"{redisPrefix}{prefix}name:*"]);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(4));

        using (Assert.EnterMultipleScope())
        {
            Assert.That((string)result![0]!, Is.EqualTo("1"));
            Assert.That((string)result[1]!, Is.EqualTo("Alice"));
            Assert.That((string)result[2]!, Is.EqualTo("2"));
            Assert.That(result[3].IsNull, Is.True);
        }
    }

    [Test]
    public async Task Sort_Ro_ReadOnlyAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var before = await this._redis.List.LRangeAsync(key, 0, -1);

        var ro = await this._redis.Key.Sort_RoAsync(key);
        var normal = await this._redis.Key.SortAsync(key);

        var after = await this._redis.List.LRangeAsync(key, 0, -1);

        Sort_RoAssertAsync(ro, "1", "2", "3", "4", "5");
        Sort_RoAssertAsync(normal, "1", "2", "3", "4", "5");

        Assert.That(after, Is.EqualTo(before));
    }

    [Test]
    public async Task Sort_Ro_DuplicateValuesAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this._redis.List.RPushAsync(key, "2");
        await this._redis.List.RPushAsync(key, "1");
        await this._redis.List.RPushAsync(key, "2");
        await this._redis.List.RPushAsync(key, "1");

        var result = await this._redis.Key.Sort_RoAsync(key);

        Sort_RoAssertAsync(result, "1", "1", "2", "2");
    }

    [Test]
    public async Task Sort_Ro_NonNumeric_WithoutAlphaAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this._redis.List.RPushAsync(key, "apple");
        await this._redis.List.RPushAsync(key, "banana");

        await Sort_RoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Key.Sort_RoAsync(key));
    }

    [Test]
    public async Task Sort_Ro_KeyNotExistsAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        var result = await this._redis.Key.Sort_RoAsync(key);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Sort_Ro_WrongKeyTypeAsync()
    {
        var key = $"String:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this._redis.String.SetAsync(key, "123");

        await Sort_RoRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Key.Sort_RoAsync(key));
    }

    [Test]
    public async Task Sort_Ro_Flags_NoneAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key, flags: CommandFlags.None);

        Sort_RoAssertAsync(result, "1", "2", "3", "4", "5");
    }

    [Test]
    public async Task Sort_Ro_CancellationToken_DefaultAsync()
    {
        var key = $"List:{{Sort_RoAsync}}:{Guid.NewGuid():N}";

        await this.Sort_RoSeedNumbersAsync(key);

        var result = await this._redis.Key.Sort_RoAsync(
            key, cancellationToken: CancellationToken.None);

        Sort_RoAssertAsync(result, "1", "2", "3", "4", "5");
    }

    #endregion

}
