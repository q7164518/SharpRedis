using SharpRedis;
using SharpRedis.Exceptions;
using static SharpRedis.RedisKeyspace;

namespace NET10.Keyspace;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class KeyspaceSync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public void Exists()
    {
        const string key = "Keyspace:{Exists}:Key";
        const string nonekey = "Keyspace:{Exists}:NoneKey";

        var exists = this._redis.Key.Exists(key);
        Assert.That(exists, Is.False);

        var existsArray = this._redis.Key.Exists([key, nonekey]);
        Assert.That(existsArray, Is.Zero);

        _ = this._redis.String.Set(key, "1");

        exists = this._redis.Key.Exists(key);
        Assert.That(exists, Is.True);

        existsArray = this._redis.Key.Exists([key, nonekey]);
        Assert.That(existsArray, Is.EqualTo(1));
    }

    [Test]
    public void Copy()
    {
        const string key = "Keyspace:{Copy}:Key";
        const string destkey = "Keyspace:{Copy}:DestKey";

        var exists = this._redis.Key.Exists(destkey);
        Assert.That(exists, Is.False);

        exists = this._redis.Key.Exists(key);
        Assert.That(exists, Is.False);

        _ = this._redis.String.Set(key, "This Copy");

        exists = this._redis.Key.Exists(key);
        Assert.That(exists, Is.True);

        exists = this._redis.Key.Exists(destkey);
        Assert.That(exists, Is.False);

        var copyOk = this._redis.Key.Copy(key, destkey);
        Assert.That(copyOk, Is.True);

        exists = this._redis.Key.Exists(destkey);
        Assert.That(exists, Is.True);

        string? copyValue = this._redis.String.Get(destkey);
        Assert.NotNull(copyValue);
        Assert.That(copyValue, Is.EqualTo("This Copy"));

        using (var db1 = this._redis.Select(1))
        {
            exists = db1.Key.Exists(destkey);
            Assert.That(exists, Is.False);

            copyOk = this._redis.Key.Copy(key, destkey, 1);
            Assert.That(copyOk, Is.True);

            exists = db1.Key.Exists(destkey);
            Assert.That(exists, Is.True);

            copyValue = db1.String.Get(destkey);
            Assert.NotNull(copyValue);
            Assert.That(copyValue, Is.EqualTo("This Copy"));
        }

        using (var db2 = this._redis.Select(2))
        {
            _ = db2.List.LPush(destkey, "db2");

            exists = db2.Key.Exists(destkey);
            Assert.That(exists, Is.True);

            copyOk = this._redis.Key.Copy(key, destkey, 2);
            Assert.That(copyOk, Is.False);

            copyOk = this._redis.Key.Copy(key, destkey, 2, true);
            Assert.That(copyOk, Is.True);

            copyValue = db2.String.Get(destkey);
            Assert.NotNull(copyValue);
            Assert.That(copyValue, Is.EqualTo("This Copy"));
        }
    }

    [Test]
    public void Del()
    {
        const string key1 = "Keyspace:{Del}:Key1";
        const string key2 = "Keyspace:{Del}:Key2";

        var single = this._redis.Key.Del(key1);
        Assert.IsFalse(single);

        _ = this._redis.String.Set(key1, "key1");

        single = this._redis.Key.Del(key1);
        Assert.IsTrue(single);

        var arr = this._redis.Key.Del([key1, key2]);
        Assert.Zero(arr);

        _ = this._redis.String.Set(key1, "key1");
        _ = this._redis.String.Set(key2, "key2");

        arr = this._redis.Key.Del([key1, key2]);
        Assert.That(arr, Is.EqualTo(2));

        _ = this._redis.String.Set(key2, "key2");

        arr = this._redis.Key.Del([key1, key2]);
        Assert.That(arr, Is.EqualTo(1));
    }

    [Test]
    public void Dump()
    {
        const string key = "Keyspace:{Dump}:Key";

        var bytes = this._redis.Key.Dump(key);
        Assert.IsNull(bytes);

        _ = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue> { { "h1", "h1" } });

        bytes = this._redis.Key.Dump(key);
        Assert.IsNotNull(bytes);
    }

    [Test]
    public void Restore()
    {
        const string key = "Keyspace:{Restore}:Key";
        const string reskey = "Keyspace:{Restore}:ResKey";

        var exists = this._redis.Key.Exists(reskey);
        Assert.That(exists, Is.False);

        _ = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue> { { "h1", "h1" } });

        var bytes = this._redis.Key.Dump(key);
        Assert.IsNotNull(bytes);

        var restore = this._redis.Key.Restore(reskey, bytes);
        Assert.IsTrue(restore);

        string? h1 = this._redis.Hash.HGet(reskey, "h1");
        Assert.That(h1, Is.EqualTo("h1"));

        restore = this._redis.Key.Restore(reskey, bytes, 60 * 1000, true);
        Assert.IsTrue(restore);

        var ttl = this._redis.Key.Ttl(reskey);
        Assert.That(ttl >= 50 && ttl <= 60);

        restore = this._redis.Key.Restore(reskey, bytes, DateTime.Now.AddMinutes(10), true, 50);
        Assert.IsTrue(restore);

        ttl = this._redis.Key.Ttl(reskey);
        Assert.That(ttl >= 590 && ttl <= 600);

        h1 = this._redis.Hash.HGet(reskey, "h1");
        Assert.That(h1, Is.EqualTo("h1"));
    }

    [Test]
    public void Expire()
    {
        const string key = "Keyspace:{Expire}:Key";

        var ok = this._redis.Key.Expire(key, 60);
        Assert.IsFalse(ok);

        ok = this._redis.Key.Expire(key, 60, RedisKeyspace.ExpireCondition.NX);
        Assert.IsFalse(ok);

        _ = this._redis.String.Set(key, "Expire");

        ok = this._redis.Key.Expire(key, 60);
        Assert.IsTrue(ok);

        ok = this._redis.Key.Expire(key, 60, RedisKeyspace.ExpireCondition.NX);
        Assert.IsFalse(ok);

        ok = this._redis.Key.Expire(key, 60, RedisKeyspace.ExpireCondition.XX);
        Assert.IsTrue(ok);

        ok = this._redis.Key.Expire(key, 30, RedisKeyspace.ExpireCondition.GT);
        Assert.IsFalse(ok);

        ok = this._redis.Key.Expire(key, 30, RedisKeyspace.ExpireCondition.LT);
        Assert.IsTrue(ok);

        ok = this._redis.Key.Expire(key, TimeSpan.FromHours(1), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);
    }

    [Test]
    public void PExpire()
    {
        const string key = "Keyspace:{PExpire}:Key";

        var ok = this._redis.Key.PExpire(key, 3000);
        Assert.IsFalse(ok);

        ok = this._redis.Key.PExpire(key, TimeSpan.FromMinutes(1), RedisKeyspace.ExpireCondition.NX);
        Assert.IsFalse(ok);

        _ = this._redis.String.Set(key, "Expire");

        ok = this._redis.Key.PExpire(key, TimeSpan.FromMinutes(1));
        Assert.IsTrue(ok);

        ok = this._redis.Key.PExpire(key, 60 * 1000, RedisKeyspace.ExpireCondition.NX);
        Assert.IsFalse(ok);

        ok = this._redis.Key.PExpire(key, 60 * 1000, RedisKeyspace.ExpireCondition.XX);
        Assert.IsTrue(ok);

        ok = this._redis.Key.PExpire(key, 30 * 1000, RedisKeyspace.ExpireCondition.GT);
        Assert.IsFalse(ok);

        ok = this._redis.Key.PExpire(key, 30 * 1000, RedisKeyspace.ExpireCondition.LT);
        Assert.IsTrue(ok);

        ok = this._redis.Key.PExpire(key, TimeSpan.FromHours(1), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);

        var time = this._redis.Key.ExpireTime(key);
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
        Assert.That(dateTime, Is.GreaterThanOrEqualTo(DateTime.Now.AddHours(1).AddSeconds(-5)));
    }

    [Test]
    public void ExpireAt()
    {
        const string key = "Keyspace:{ExpireAt}:Key";

        var ok = this._redis.Key.ExpireAt(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeSeconds());
        Assert.IsFalse(ok);

        _ = this._redis.String.Set(key, "ExpireAt");

        ok = this._redis.Key.ExpireAt(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeSeconds(), RedisKeyspace.ExpireCondition.NX);
        Assert.IsTrue(ok);

        ok = this._redis.Key.ExpireAt(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeSeconds(), RedisKeyspace.ExpireCondition.XX);
        Assert.IsTrue(ok);

        ok = this._redis.Key.ExpireAt(key, DateTimeOffset.UtcNow.AddSeconds(5).ToUnixTimeSeconds(), RedisKeyspace.ExpireCondition.LT);
        Assert.IsTrue(ok);

        ok = this._redis.Key.ExpireAt(key, DateTimeOffset.UtcNow.AddSeconds(50).ToUnixTimeSeconds(), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);

        ok = this._redis.Key.ExpireAt(key, DateTime.Now.AddSeconds(60), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);

        var time = this._redis.Key.ExpireTime(key);
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
        Assert.That(dateTime, Is.GreaterThanOrEqualTo(DateTime.Now.AddSeconds(57)));
    }

    [Test]
    public void PExpireAt()
    {
        const string key = "Keyspace:{PExpireAt}:Key";

        var ok = this._redis.Key.PExpireAt(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeMilliseconds());
        Assert.IsFalse(ok);

        _ = this._redis.String.Set(key, "ExpireAt");

        ok = this._redis.Key.PExpireAt(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeMilliseconds(), RedisKeyspace.ExpireCondition.NX);
        Assert.IsTrue(ok);

        ok = this._redis.Key.PExpireAt(key, DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeMilliseconds(), RedisKeyspace.ExpireCondition.XX);
        Assert.IsTrue(ok);

        ok = this._redis.Key.PExpireAt(key, DateTimeOffset.UtcNow.AddSeconds(5).ToUnixTimeMilliseconds(), RedisKeyspace.ExpireCondition.LT);
        Assert.IsTrue(ok);

        ok = this._redis.Key.PExpireAt(key, DateTimeOffset.UtcNow.AddSeconds(50).ToUnixTimeMilliseconds(), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);

        ok = this._redis.Key.PExpireAt(key, DateTime.Now.AddSeconds(60), RedisKeyspace.ExpireCondition.GT);
        Assert.IsTrue(ok);

        var time = this._redis.Key.ExpireTime(key);
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
        Assert.That(dateTime, Is.GreaterThanOrEqualTo(DateTime.Now.AddSeconds(57)));
    }

    [Test]
    public void ExpireTime()
    {
        const string key = "Keyspace:{ExpireTime}:Key";

        var time = this._redis.Key.ExpireTime(key);
        Assert.That(time, Is.EqualTo(-2));

        _ = this._redis.String.Set(key, "ExpireAt");

        time = this._redis.Key.ExpireTime(key);
        Assert.That(time, Is.EqualTo(-1));

        _ = this._redis.Key.ExpireAt(key, DateTime.Now.AddSeconds(60));

        time = this._redis.Key.ExpireTime(key);
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
        Assert.That(dateTime, Is.GreaterThanOrEqualTo(DateTime.Now.AddSeconds(57)));
    }

    [Test]
    public void PExpireTime()
    {
        const string key = "Keyspace:{PExpireTime}:Key";

        var time = this._redis.Key.PExpireTime(key);
        Assert.That(time, Is.EqualTo(-2));

        _ = this._redis.String.Set(key, "ExpireAt");

        time = this._redis.Key.PExpireTime(key);
        Assert.That(time, Is.EqualTo(-1));

        _ = this._redis.Key.ExpireAt(key, DateTime.Now.AddSeconds(60));

        time = this._redis.Key.PExpireTime(key);
        var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(time).LocalDateTime;
        Assert.That(dateTime, Is.GreaterThanOrEqualTo(DateTime.Now.AddSeconds(57)));
    }


    #region Sort

    private static void SortRedisError<TError>(Action action)
        where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    private static void SortAssert(
        RedisResult[] result,
        params string[] expected)
    {
        Assert.That(result, Has.Length.EqualTo(expected.Length));

        for (int i = 0; i < expected.Length; i++)
        {
            string actual = result[i]!;
            Assert.That(actual, Is.EqualTo(expected[i]),
                $"Sort result index: {i}");
        }
    }

    private void SortSeedNumbers(string key)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)this._redis.List.RPush(key, "3"), Is.EqualTo(1));
            Assert.That((int)this._redis.List.RPush(key, "1"), Is.EqualTo(2));
            Assert.That((int)this._redis.List.RPush(key, "5"), Is.EqualTo(3));
            Assert.That((int)this._redis.List.RPush(key, "2"), Is.EqualTo(4));
            Assert.That((int)this._redis.List.RPush(key, "4"), Is.EqualTo(5));
        }
    }

    /// <summary>
    /// 默认数值升序。
    /// </summary>
    [Test]
    public void Sort_Default()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(key);

        SortAssert(result!, "1", "2", "3", "4", "5");
    }

    /// <summary>
    /// 显式 ASC。
    /// </summary>
    [Test]
    public void Sort_Ascending()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, sort: SortDirection.Asc);

        SortAssert(result!, "1", "2", "3", "4", "5");
    }

    /// <summary>
    /// DESC 降序。
    /// </summary>
    [Test]
    public void Sort_Descending()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, sort: SortDirection.Desc);

        SortAssert(result!, "5", "4", "3", "2", "1");
    }

    /// <summary>
    /// LIMIT 0 3，返回前三项。
    /// </summary>
    [Test]
    public void Sort_Limit_FirstPage()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, offset: 0, count: 3);

        SortAssert(result!, "1", "2", "3");
    }

    /// <summary>
    /// LIMIT 2 2。
    /// </summary>
    [Test]
    public void Sort_Limit_MiddlePage()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, offset: 2, count: 2);

        SortAssert(result!, "3", "4");
    }

    /// <summary>
    /// LIMIT offset count + DESC。
    /// </summary>
    [Test]
    public void Sort_Limit_Descending()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, offset: 1, count: 3,
            sort: SortDirection.Desc);

        SortAssert(result!, "4", "3", "2");
    }

    /// <summary>
    /// LIMIT count=0 返回空数组。
    /// </summary>
    [Test]
    public void Sort_Limit_ZeroCount()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, offset: 0, count: 0);

        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// LIMIT offset 超出集合长度。
    /// </summary>
    [Test]
    public void Sort_Limit_OffsetOutOfRange()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, offset: 100, count: 10);

        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// LIMIT count=-1 返回 offset 后的所有元素。
    /// </summary>
    [Test]
    public void Sort_Limit_CountNegativeOne()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, offset: 2, count: -1);

        SortAssert(result!, "3", "4", "5");
    }

    /// <summary>
    /// ALPHA 字典序。
    /// 注意字典序 "10" 小于 "2"。
    /// </summary>
    [Test]
    public void Sort_Alpha()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this._redis.List.RPush(key, "20");
        this._redis.List.RPush(key, "3");
        this._redis.List.RPush(key, "100");
        this._redis.List.RPush(key, "10");

        var result = this._redis.Key.Sort(
            key, alpha: true);

        SortAssert(result!, "10", "100", "20", "3");
    }

    /// <summary>
    /// ALPHA + DESC。
    /// </summary>
    [Test]
    public void Sort_Alpha_Descending()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this._redis.List.RPush(key, "apple");
        this._redis.List.RPush(key, "banana");
        this._redis.List.RPush(key, "cherry");

        var result = this._redis.Key.Sort(
            key, sort: SortDirection.Desc, alpha: true);

        SortAssert(result!, "cherry", "banana", "apple");
    }

    /// <summary>
    /// GET # 返回排序元素本身。
    /// </summary>
    [Test]
    public void Sort_Get_Self()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, get: ["#"]);

        SortAssert(result!, "1", "2", "3", "4", "5");
    }

    /// <summary>
    /// 多次 GET #，返回展平后的数组。
    /// </summary>
    [Test]
    public void Sort_Get_Multiple()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this._redis.List.RPush(key, "2");
        this._redis.List.RPush(key, "1");

        var result = this._redis.Key.Sort(
            key, get: ["#", "#"]);

        SortAssert(result!, "1", "1", "2", "2");
    }

    /// <summary>
    /// BY nosort 跳过排序，List 保留原始顺序。
    /// </summary>
    [Test]
    public void Sort_By_NoSort()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, pattern: "nosort");

        SortAssert(result!, "3", "1", "5", "2", "4");
    }

    /// <summary>
    /// 不存在的 Key 返回空数组。
    /// </summary>
    [Test]
    public void Sort_KeyNotExists()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        var result = this._redis.Key.Sort(key);

        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// 已存在但为空的集合无法长期保持为 Redis Key，
    /// 这里使用不存在 Key 验证空集合行为。
    /// </summary>
    [Test]
    public void Sort_EmptyResult()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        var result = this._redis.Key.Sort(
            key, offset: 0, count: 10);

        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// 默认数值排序不接受无法解析为数字的元素。
    /// </summary>
    [Test]
    public void Sort_NonNumeric_WithoutAlpha()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this._redis.List.RPush(key, "apple");
        this._redis.List.RPush(key, "banana");

        SortRedisError<RedisErrorException>(() =>
            this._redis.Key.Sort(key));
    }

    /// <summary>
    /// String 类型不能排序。
    /// </summary>
    [Test]
    public void Sort_WrongKeyType()
    {
        var key = $"String:{{Sort}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "123");

        SortRedisError<RedisErrorException>(() =>
            this._redis.Key.Sort(key));
    }

    /// <summary>
    /// 重复元素保留重复值。
    /// </summary>
    [Test]
    public void Sort_DuplicateValues()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this._redis.List.RPush(key, "2");
        this._redis.List.RPush(key, "1");
        this._redis.List.RPush(key, "2");
        this._redis.List.RPush(key, "1");

        var result = this._redis.Key.Sort(key);

        SortAssert(result!, "1", "1", "2", "2");
    }

    /// <summary>
    /// SORT 不带 STORE 时不修改原始 List。
    /// </summary>
    [Test]
    public void Sort_ReadOnlyData()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(key);

        SortAssert(result!, "1", "2", "3", "4", "5");

        var original = this._redis.List.LRange(key, 0, -1);

        Assert.That(original!.Select(x => (string)x!),
            Is.EqualTo(["3", "1", "5", "2", "4"]));
    }

    /// <summary>
    /// CommandFlags.None。
    /// </summary>
    [Test]
    public void Sort_Flags_None()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, flags: CommandFlags.None);

        SortAssert(result!, "1", "2", "3", "4", "5");
    }

    /// <summary>
    /// CancellationToken.None。
    /// </summary>
    [Test]
    public void Sort_CancellationToken_Default()
    {
        var key = $"List:{{Sort}}:{Guid.NewGuid():N}";

        this.SortSeedNumbers(key);

        var result = this._redis.Key.Sort(
            key, cancellationToken: CancellationToken.None);

        SortAssert(result!, "1", "2", "3", "4", "5");
    }

    #endregion

    #region Sort_ExternalPatterns

    /// <summary>
    /// BY 外部 String Key 的数值进行排序。
    /// 兼容 RESP2 / RESP3 的 Key 前缀。
    /// </summary>
    [Test]
    public void Sort_By_ExternalWeights()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        this._redis.List.RPush(key, "1");
        this._redis.List.RPush(key, "2");
        this._redis.List.RPush(key, "3");

        this._redis.String.Set($"{prefix}weight:1", "30");
        this._redis.String.Set($"{prefix}weight:2", "10");
        this._redis.String.Set($"{prefix}weight:3", "20");

        var result = this._redis.Key.Sort(
            key,
            pattern: $"{redisPrefix}{prefix}weight:*");

        SortAssert(result!, "2", "3", "1");
    }

    /// <summary>
    /// GET 外部 String Key，同时返回元素自身。
    /// </summary>
    [Test]
    public void Sort_Get_ExternalValues()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        this._redis.List.RPush(key, "2");
        this._redis.List.RPush(key, "1");

        this._redis.String.Set($"{prefix}name:1", "Alice");
        this._redis.String.Set($"{prefix}name:2", "Bob");

        var result = this._redis.Key.Sort(
            key,
            get:
            [
                "#",
            $"{redisPrefix}{prefix}name:*"
            ]);

        SortAssert(result!, "1", "Alice", "2", "Bob");
    }

    /// <summary>
    /// GET 匹配不到外部 Key 时返回 Null。
    /// </summary>
    [Test]
    public void Sort_Get_MissingExternalKey()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        this._redis.List.RPush(key, "1");
        this._redis.List.RPush(key, "2");

        this._redis.String.Set($"{prefix}name:1", "Alice");

        var result = this._redis.Key.Sort(
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

    private static void SortStoreRedisError<TError>(Action action)
        where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void SortStore_Default()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStore}}:{id}:source";
        var store = $"List:{{SortStore}}:{id}:dest";

        this.SortSeedNumbers(key);

        var count = this._redis.Key.SortStore(key, store);

        Assert.That(count, Is.EqualTo(5));

        var result = this._redis.List.LRange(store, 0, -1);

        Assert.That(result!.Select(x => (string)x!),
            Is.EqualTo(["1", "2", "3", "4", "5"]));
    }

    [Test]
    public void SortStore_Descending()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStore}}:{id}:source";
        var store = $"List:{{SortStore}}:{id}:dest";

        this.SortSeedNumbers(key);

        var count = this._redis.Key.SortStore(
            key, store, sort: SortDirection.Desc);

        Assert.That(count, Is.EqualTo(5));

        var result = this._redis.List.LRange(store, 0, -1);

        Assert.That(result!.Select(x => (string)x!),
            Is.EqualTo(["5", "4", "3", "2", "1"]));
    }

    [Test]
    public void SortStore_Limit()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStore}}:{id}:source";
        var store = $"List:{{SortStore}}:{id}:dest";

        this.SortSeedNumbers(key);

        var count = this._redis.Key.SortStore(
            key, store, offset: 1, count: 2);

        Assert.That(count, Is.EqualTo(2));

        var result = this._redis.List.LRange(store, 0, -1);

        Assert.That(result!.Select(x => (string)x!),
            Is.EqualTo(["2", "3"]));
    }

    [Test]
    public void SortStore_Alpha()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStore}}:{id}:source";
        var store = $"List:{{SortStore}}:{id}:dest";

        this._redis.List.RPush(key, "banana");
        this._redis.List.RPush(key, "apple");
        this._redis.List.RPush(key, "cherry");

        var count = this._redis.Key.SortStore(
            key, store, alpha: true);

        Assert.That(count, Is.EqualTo(3));

        var result = this._redis.List.LRange(store, 0, -1);

        Assert.That(result!.Select(x => (string)x!),
            Is.EqualTo(["apple", "banana", "cherry"]));
    }

    /// <summary>
    /// SORT STORE 覆盖已存在的目标 List。
    /// </summary>
    [Test]
    public void SortStore_OverwriteDestination()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStore}}:{id}:source";
        var store = $"List:{{SortStore}}:{id}:dest";

        this.SortSeedNumbers(key);
        this._redis.List.RPush(store, "old");

        Assert.That(this._redis.Key.SortStore(
            key, store), Is.EqualTo(5));

        var result = this._redis.List.LRange(store, 0, -1);

        Assert.That(result!.Select(x => (string)x!),
            Is.EqualTo(["1", "2", "3", "4", "5"]));
    }

    /// <summary>
    /// STORE 允许覆盖已有的其他数据类型。
    /// </summary>
    [Test]
    public void SortStore_OverwriteString()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStore}}:{id}:source";
        var store = $"List:{{SortStore}}:{id}:dest";

        this.SortSeedNumbers(key);
        this._redis.String.Set(store, "old");

        Assert.That(this._redis.Key.SortStore(
            key, store), Is.EqualTo(5));

        var result = this._redis.List.LRange(store, 0, -1);

        Assert.That(result!.Select(x => (string)x!),
            Is.EqualTo(["1", "2", "3", "4", "5"]));
    }

    /// <summary>
    /// 源不存在：存储数量为零。
    /// </summary>
    [Test]
    public void SortStore_SourceNotExists()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStore}}:{id}:source";
        var store = $"List:{{SortStore}}:{id}:dest";

        var count = this._redis.Key.SortStore(key, store);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.Zero);
            Assert.That(this._redis.List.LRange(store, 0, -1), Is.Null);
        }
    }

    /// <summary>
    /// LIMIT 0 0：不存储任何元素。
    /// </summary>
    [Test]
    public void SortStore_ZeroCount()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStore}}:{id}:source";
        var store = $"List:{{SortStore}}:{id}:dest";

        this.SortSeedNumbers(key);

        var count = this._redis.Key.SortStore(
            key, store, offset: 0, count: 0);

        Assert.That(count, Is.Zero);
    }

    /// <summary>
    /// GET # 可用于 STORE。
    /// </summary>
    [Test]
    public void SortStore_Get_Self()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStore}}:{id}:source";
        var store = $"List:{{SortStore}}:{id}:dest";

        this.SortSeedNumbers(key);

        var count = this._redis.Key.SortStore(
            key, store, get: ["#"]);

        Assert.That(count, Is.EqualTo(5));

        var result = this._redis.List.LRange(store, 0, -1);

        Assert.That(result!.Select(x => (string)x!),
            Is.EqualTo(["1", "2", "3", "4", "5"]));
    }

    [Test]
    public void SortStore_WrongKeyType()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"String:{{SortStore}}:{id}:source";
        var store = $"String:{{SortStore}}:{id}:dest";

        this._redis.String.Set(key, "hello");

        SortStoreRedisError<RedisErrorException>(() =>
            this._redis.Key.SortStore(key, store));
    }

    [Test]
    public void SortStore_Flags_None()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStore}}:{id}:source";
        var store = $"List:{{SortStore}}:{id}:dest";

        this.SortSeedNumbers(key);

        Assert.That(this._redis.Key.SortStore(
            key, store, flags: CommandFlags.None),
            Is.EqualTo(5));
    }

    [Test]
    public void SortStore_CancellationToken_Default()
    {
        var id = Guid.NewGuid().ToString("N");
        var key = $"List:{{SortStore}}:{id}:source";
        var store = $"List:{{SortStore}}:{id}:dest";

        this.SortSeedNumbers(key);

        Assert.That(this._redis.Key.SortStore(
            key, store,
            cancellationToken: CancellationToken.None),
            Is.EqualTo(5));
    }

    #endregion

    #region Sort_Ro

    private static void Sort_RoRedisError<TError>(Action action)
        where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    private static void Sort_RoAssert(
        RedisResult[]? result,
        params string[] expected)
    {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(expected.Length));

        for (int i = 0; i < expected.Length; i++)
        {
            string actual = result![i]!;
            Assert.That(actual, Is.EqualTo(expected[i]),
                $"SORT_RO result index: {i}");
        }
    }

    private void Sort_RoSeedNumbers(string key)
    {
        this._redis.List.RPush(key, "3");
        this._redis.List.RPush(key, "1");
        this._redis.List.RPush(key, "5");
        this._redis.List.RPush(key, "2");
        this._redis.List.RPush(key, "4");
    }

    /// <summary>
    /// 默认数值升序排序。
    /// </summary>
    [Test]
    public void Sort_Ro_Default()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(key);

        Sort_RoAssert(result, "1", "2", "3", "4", "5");
    }

    /// <summary>
    /// 显式指定 ASC。
    /// </summary>
    [Test]
    public void Sort_Ro_Ascending()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key, sort: SortDirection.Asc);

        Sort_RoAssert(result, "1", "2", "3", "4", "5");
    }

    /// <summary>
    /// DESC 数值降序排序。
    /// </summary>
    [Test]
    public void Sort_Ro_Descending()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key, sort: SortDirection.Desc);

        Sort_RoAssert(result, "5", "4", "3", "2", "1");
    }

    /// <summary>
    /// LIMIT 0 3，返回前三个元素。
    /// </summary>
    [Test]
    public void Sort_Ro_Limit_FirstPage()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key, offset: 0, count: 3);

        Sort_RoAssert(result, "1", "2", "3");
    }

    /// <summary>
    /// LIMIT 2 2，验证偏移量。
    /// </summary>
    [Test]
    public void Sort_Ro_Limit_MiddlePage()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key, offset: 2, count: 2);

        Sort_RoAssert(result, "3", "4");
    }

    /// <summary>
    /// LIMIT 与 DESC 组合。
    /// </summary>
    [Test]
    public void Sort_Ro_Limit_Descending()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key,
            offset: 1,
            count: 3,
            sort: SortDirection.Desc);

        Sort_RoAssert(result, "4", "3", "2");
    }

    /// <summary>
    /// LIMIT 0 0 返回空数组。
    /// </summary>
    [Test]
    public void Sort_Ro_Limit_ZeroCount()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key, offset: 0, count: 0);
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// LIMIT 起始偏移量超过元素数量。
    /// </summary>
    [Test]
    public void Sort_Ro_Limit_OffsetOutOfRange()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key, offset: 100, count: 10);
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// LIMIT count=-1 返回剩余所有元素。
    /// </summary>
    [Test]
    public void Sort_Ro_Limit_CountNegativeOne()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key, offset: 2, count: -1);

        Sort_RoAssert(result, "3", "4", "5");
    }

    /// <summary>
    /// ALPHA 按字符串字典序排序。
    /// </summary>
    [Test]
    public void Sort_Ro_Alpha()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this._redis.List.RPush(key, "20");
        this._redis.List.RPush(key, "3");
        this._redis.List.RPush(key, "100");
        this._redis.List.RPush(key, "10");

        var result = this._redis.Key.Sort_Ro(
            key, alpha: true);

        Sort_RoAssert(result, "10", "100", "20", "3");
    }

    /// <summary>
    /// ALPHA + DESC 字典序降序。
    /// </summary>
    [Test]
    public void Sort_Ro_Alpha_Descending()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this._redis.List.RPush(key, "apple");
        this._redis.List.RPush(key, "banana");
        this._redis.List.RPush(key, "cherry");

        var result = this._redis.Key.Sort_Ro(
            key, sort: SortDirection.Desc, alpha: true);

        Sort_RoAssert(result, "cherry", "banana", "apple");
    }

    /// <summary>
    /// GET # 返回当前排序元素自身。
    /// </summary>
    [Test]
    public void Sort_Ro_Get_Self()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key, get: ["#"]);

        Sort_RoAssert(result, "1", "2", "3", "4", "5");
    }

    /// <summary>
    /// 多个 GET #，验证展平结果的顺序。
    /// </summary>
    [Test]
    public void Sort_Ro_Get_Multiple()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this._redis.List.RPush(key, "2");
        this._redis.List.RPush(key, "1");

        var result = this._redis.Key.Sort_Ro(
            key, get: ["#", "#"]);

        Sort_RoAssert(result, "1", "1", "2", "2");
    }

    /// <summary>
    /// BY nosort 跳过排序。
    /// List 应保留原始元素顺序。
    /// </summary>
    [Test]
    public void Sort_Ro_By_NoSort()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key, pattern: "nosort");

        Sort_RoAssert(result, "3", "1", "5", "2", "4");
    }

    /// <summary>
    /// 按外部 String Key 中的权重排序。
    /// 使用当前 Redis 实例的物理 Key 前缀。
    /// </summary>
    [Test]
    public void Sort_Ro_By_ExternalWeights()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort_Ro:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        this._redis.List.RPush(key, "1");
        this._redis.List.RPush(key, "2");
        this._redis.List.RPush(key, "3");

        this._redis.String.Set($"{prefix}weight:1", "30");
        this._redis.String.Set($"{prefix}weight:2", "10");
        this._redis.String.Set($"{prefix}weight:3", "20");

        var result = this._redis.Key.Sort_Ro(
            key, pattern: $"{redisPrefix}{prefix}weight:*");

        Sort_RoAssert(result, "2", "3", "1");
    }

    /// <summary>
    /// GET 外部 String Key，同时使用 GET # 返回元素自身。
    /// </summary>
    [Test]
    public void Sort_Ro_Get_ExternalValues()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort_Ro:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        this._redis.List.RPush(key, "2");
        this._redis.List.RPush(key, "1");

        this._redis.String.Set($"{prefix}name:1", "Alice");
        this._redis.String.Set($"{prefix}name:2", "Bob");

        var result = this._redis.Key.Sort_Ro(
            key,
            get: ["#", $"{redisPrefix}{prefix}name:*"]);

        Sort_RoAssert(result, "1", "Alice", "2", "Bob");
    }

    /// <summary>
    /// GET 外部 Key 不存在时，保留 Redis Null 元素。
    /// </summary>
    [Test]
    public void Sort_Ro_Get_MissingExternalKey()
    {
        var id = Guid.NewGuid().ToString("N");
        var prefix = $"Sort_Ro:{{{id}}}:";
        var redisPrefix = Factory.GetPrefix(this._redis);
        var key = $"{prefix}source";

        this._redis.List.RPush(key, "1");
        this._redis.List.RPush(key, "2");

        this._redis.String.Set($"{prefix}name:1", "Alice");

        var result = this._redis.Key.Sort_Ro(
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

    /// <summary>
    /// SORT_RO 不存在 STORE 选项，不会修改原 List。
    /// 还要验证结果与 SORT 一致。
    /// </summary>
    [Test]
    public void Sort_Ro_ReadOnly()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var before = this._redis.List.LRange(key, 0, -1);

        var ro = this._redis.Key.Sort_Ro(key);
        var normal = this._redis.Key.Sort(key);

        var after = this._redis.List.LRange(key, 0, -1);

        Sort_RoAssert(ro, "1", "2", "3", "4", "5");
        Sort_RoAssert(normal, "1", "2", "3", "4", "5");

        Assert.That(after, Is.EqualTo(before));
    }

    /// <summary>
    /// 重复值不会被 SORT_RO 去重。
    /// </summary>
    [Test]
    public void Sort_Ro_DuplicateValues()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this._redis.List.RPush(key, "2");
        this._redis.List.RPush(key, "1");
        this._redis.List.RPush(key, "2");
        this._redis.List.RPush(key, "1");

        var result = this._redis.Key.Sort_Ro(key);

        Sort_RoAssert(result, "1", "1", "2", "2");
    }

    /// <summary>
    /// List 中的非数字元素，在默认数值排序时应报错。
    /// </summary>
    [Test]
    public void Sort_Ro_NonNumeric_WithoutAlpha()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this._redis.List.RPush(key, "apple");
        this._redis.List.RPush(key, "banana");

        Sort_RoRedisError<RedisErrorException>(() =>
            this._redis.Key.Sort_Ro(key));
    }

    /// <summary>
    /// 不存在的 Key 返回空数组。
    /// </summary>
    [Test]
    public void Sort_Ro_KeyNotExists()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";
        var result = this._redis.Key.Sort_Ro(key);
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// String 类型的 Key 不支持 SORT_RO。
    /// </summary>
    [Test]
    public void Sort_Ro_WrongKeyType()
    {
        var key = $"String:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this._redis.String.Set(key, "123");

        Sort_RoRedisError<RedisErrorException>(() =>
            this._redis.Key.Sort_Ro(key));
    }

    /// <summary>
    /// 显式传入 CommandFlags.None。
    /// </summary>
    [Test]
    public void Sort_Ro_Flags_None()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key, flags: CommandFlags.None);

        Sort_RoAssert(result, "1", "2", "3", "4", "5");
    }

    /// <summary>
    /// 显式传入 CancellationToken.None。
    /// </summary>
    [Test]
    public void Sort_Ro_CancellationToken_Default()
    {
        var key = $"List:{{Sort_Ro}}:{Guid.NewGuid():N}";

        this.Sort_RoSeedNumbers(key);

        var result = this._redis.Key.Sort_Ro(
            key, cancellationToken: CancellationToken.None);

        Sort_RoAssert(result, "1", "2", "3", "4", "5");
    }

    #endregion

}
