using SharpRedis;
using System.Text;
using static SharpRedis.RedisHash;

namespace NET10.Hash;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class HashSync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public void HSet()
    {
        const string key = "{181}:Hash:HSet:Key";
        const string field = "field";
        const string value = "Hash:HSet:Value";

        string? get = this._redis.Hash.HGet(key, field);
        Assert.That(get, Is.Null);

        short result = this._redis.Hash.HSet(key, field, value);
        Assert.That(result, Is.EqualTo(1));

        get = this._redis.Hash.HGet(key, field);
        Assert.That(get, Is.EqualTo(value));

        result = this._redis.Hash.HSet(key, "field", "value1");
        Assert.That(result, Is.Zero);

        get = this._redis.Hash.HGet(key, field);
        Assert.That(get, Is.EqualTo("value1"));

        result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { "field", "value2" },
            { "", "" },
            { "3", "33" },
        });
        Assert.That(result, Is.EqualTo(2));

        get = this._redis.Hash.HGet(key, "3");
        Assert.That(get, Is.EqualTo("33"));
    }

    [Test]
    public void HDel()
    {
        const string key = "{182}:Hash:HDel:Key";
        const string field = "field";

        int del = this._redis.Hash.HDel(key, field);
        Assert.That(del, Is.Zero);

        int result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field, field },
            { "2", string.Empty },
            { "3", "33" },
        });
        Assert.That(result, Is.EqualTo(3));

        del = this._redis.Hash.HDel(key, field);
        Assert.That(del, Is.EqualTo(1));

        del = this._redis.Hash.HDel(key, [field, "2", "3", "44"]);
        Assert.That(del, Is.EqualTo(2));
    }

    [Test]
    public void HExists()
    {
        const string key = "{183}:Hash:HExists:Key";
        const string field = "field";

        var exists = this._redis.Hash.HExists(key, field);
        Assert.That(!exists);

        int set = this._redis.Hash.HSet(key, field, string.Empty);
        Assert.That(set, Is.EqualTo(1));

        exists = this._redis.Hash.HExists(key, field);
        Assert.That(exists);

        exists = this._redis.Hash.HExists(key, "none_field");
        Assert.That(!exists);
    }

    [Test]
    public void HExpire()
    {
        const string key = "{184}:Hash:HExpire:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        short result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        var expire = this._redis.Hash.HExpire(key, field1, 100);
        Assert.That(expire);

        expire = this._redis.Hash.HExpire(key, "none", 100);
        Assert.That(!expire);

        expire = this._redis.Hash.HExpire("none_hash", field1, TimeSpan.FromSeconds(100));
        Assert.That(!expire);

        expire = this._redis.Hash.HExpire(key, field1, 100, ExpirationWhen.LT);
        Assert.That(!expire);

        expire = this._redis.Hash.HExpire(key, field1, 100, ExpirationWhen.XX);
        Assert.That(expire);

        expire = this._redis.Hash.HExpire(key, field1, 100, ExpirationWhen.NX);
        Assert.That(!expire);

        expire = this._redis.Hash.HExpire(key, field2, TimeSpan.FromSeconds(100), ExpirationWhen.XX);
        Assert.That(!expire);

        expire = this._redis.Hash.HExpire(key, field2, 100, ExpirationWhen.NX);
        Assert.That(expire);

        var expires = this._redis.Hash.HExpire(key, new List<SharpValue> { field1, field2 }, 100);
        Assert.That(expires, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(expires[0]);
            Assert.That(expires[1]);
        }

        var unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expireTime = this._redis.Hash.HExpireTime(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeSeconds));
        Assert.That(expireTime - unixTimeSeconds, Is.GreaterThanOrEqualTo(98));

        var expireTimes = this._redis.Hash.HExpireTime(key, [field2, "none", "field3"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((long)expireTimes[0], Is.GreaterThan(unixTimeSeconds));
            Assert.That(expireTimes[0] - unixTimeSeconds, Is.GreaterThanOrEqualTo(98));
            Assert.That((int)expireTimes[1], Is.EqualTo(-2));
            Assert.That((int)expireTimes[2], Is.EqualTo(-1));
        }
    }

    [Test]
    public void HPExpire()
    {
        const string key = "{185}:Hash:HPExpire:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        int result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        var expire = this._redis.Hash.HPExpire(key, field1, TimeSpan.FromMilliseconds(5000));
        Assert.That(expire);

        expire = this._redis.Hash.HPExpire(key, "none", 5000);
        Assert.That(!expire);

        expire = this._redis.Hash.HPExpire("none_hash", field1, 5000);
        Assert.That(!expire);

        expire = this._redis.Hash.HPExpire(key, field1, 5000, ExpirationWhen.LT);
        Assert.That(!expire);

        expire = this._redis.Hash.HPExpire(key, field1, 5000, ExpirationWhen.XX);
        Assert.That(expire);

        expire = this._redis.Hash.HPExpire(key, field1, 5000, ExpirationWhen.NX);
        Assert.That(!expire);

        expire = this._redis.Hash.HPExpire(key, field2, TimeSpan.FromMilliseconds(5000), ExpirationWhen.XX);
        Assert.That(!expire);

        expire = this._redis.Hash.HPExpire(key, field2, 5000, ExpirationWhen.NX);
        Assert.That(expire);

        var expires = this._redis.Hash.HPExpire(key, new List<SharpValue> { field1, field2, "none" }, 3000);
        Assert.That(expires, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(expires[0]);
            Assert.That(expires[1]);
            Assert.That(!expires[2]);
        }

        var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var expireTime = this._redis.Hash.HPExpireTime(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeMilliseconds));
        Assert.That(expireTime - unixTimeMilliseconds, Is.GreaterThanOrEqualTo(10));

        var expireTimes = this._redis.Hash.HPExpireTime(key, [field2, "none", "field3"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((long)expireTimes[0], Is.GreaterThan(unixTimeMilliseconds));
            Assert.That(expireTimes[0] - unixTimeMilliseconds, Is.GreaterThanOrEqualTo(10));
            Assert.That((short)expireTimes[1], Is.EqualTo(-2));
            Assert.That((short)expireTimes[2], Is.EqualTo(-1));
        }
    }

    [Test]
    public void HExpireAt()
    {
        const string key = "{186}:Hash:HExpireAt:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var ok = this._redis.Hash.HExpireAt(key, field1, DateTime.Now.AddSeconds(100));
        Assert.That(!ok);

        int result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        ok = this._redis.Hash.HExpireAt(key, field1, DateTime.Now.AddSeconds(100));
        Assert.That(ok);

        ok = this._redis.Hash.HExpireAt(key, field1, DateTime.Now.AddSeconds(10), ExpirationWhen.GT);
        Assert.That(!ok);

        var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var expireTime = this._redis.Hash.HPExpireTime(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeMilliseconds));

        ok = this._redis.Hash.HExpireAt(key, field2, DateTime.Now.AddSeconds(100), ExpirationWhen.XX);
        Assert.That(!ok);

        ok = this._redis.Hash.HExpireAt(key, field2, DateTime.Now.AddSeconds(100), ExpirationWhen.NX);
        Assert.That(ok);

        ok = this._redis.Hash.HExpireAt(key, field2, new DateTimeOffset(DateTime.Now.AddSeconds(80)).ToUnixTimeSeconds(), ExpirationWhen.LT);
        Assert.That(ok);

        expireTime = this._redis.Hash.HPExpireTime(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeMilliseconds));
        Assert.That(expireTime - unixTimeMilliseconds, Is.GreaterThanOrEqualTo(78));

        var oks = this._redis.Hash.HExpireAt(key, [field2, field1, "none", "field3"], DateTime.Now.AddSeconds(100));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(oks[0]);
            Assert.That(oks[1]);
            Assert.That(!oks[2]);
            Assert.That(oks[3]);
        }

        oks = this._redis.Hash.HExpireAt(key, [field2, field1, "none", "field3"], new DateTimeOffset(DateTime.Now.AddSeconds(80)).ToUnixTimeSeconds(), ExpirationWhen.XX);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(oks[0]);
            Assert.That(oks[1]);
            Assert.That(!oks[2]);
            Assert.That(oks[3]);
        }
    }

    [Test]
    public void HPExpireAt()
    {
        const string key = "{187}:Hash:HPExpireAt:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var ok = this._redis.Hash.HPExpireAt(key, field1, DateTime.Now.AddSeconds(100));
        Assert.That(!ok);

        int result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        ok = this._redis.Hash.HPExpireAt(key, field1, DateTime.Now.AddSeconds(100));
        Assert.That(ok);

        ok = this._redis.Hash.HPExpireAt(key, field1, DateTime.Now.AddSeconds(10), ExpirationWhen.GT);
        Assert.That(!ok);

        var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var expireTime = this._redis.Hash.HPExpireTime(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeMilliseconds));

        ok = this._redis.Hash.HPExpireAt(key, field2, DateTime.Now.AddSeconds(100), ExpirationWhen.XX);
        Assert.That(!ok);

        ok = this._redis.Hash.HPExpireAt(key, field2, DateTime.Now.AddSeconds(100), ExpirationWhen.NX);
        Assert.That(ok);

        ok = this._redis.Hash.HPExpireAt(key, field2, new DateTimeOffset(DateTime.Now.AddSeconds(80)).ToUnixTimeMilliseconds(), ExpirationWhen.LT);
        Assert.That(ok);

        expireTime = this._redis.Hash.HPExpireTime(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeMilliseconds));
        Assert.That(expireTime - unixTimeMilliseconds, Is.GreaterThanOrEqualTo(78));

        var oks = this._redis.Hash.HPExpireAt(key, [field2, field1, "none", "field3"], DateTime.Now.AddSeconds(100));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(oks[0]);
            Assert.That(oks[1]);
            Assert.That(!oks[2]);
            Assert.That(oks[3]);
        }

        oks = this._redis.Hash.HPExpireAt(key, [field2, field1, "none", "field3"], new DateTimeOffset(DateTime.Now.AddSeconds(80)).ToUnixTimeMilliseconds(), ExpirationWhen.XX);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(oks[0]);
            Assert.That(oks[1]);
            Assert.That(!oks[2]);
            Assert.That(oks[3]);
        }
    }

    [Test]
    public void HGetAll()
    {
        const string key = "{188}:Hash:HGetAll:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var all = this._redis.Hash.HGetAll(key);
        Assert.That(all, Is.Null);

        int result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        all = this._redis.Hash.HGetAll(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(all!, Has.Count.EqualTo(3));
            Assert.That(all![field1].ToString(), Is.EqualTo(field1));
            Assert.That(all!["field3"].ToString(), Is.EqualTo(string.Empty));
        }

        var allBytes = this._redis.Hash.HGetAll(key, CommandFlags.ReturnBytes);
        Assert.That(allBytes!, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(allBytes.First().Value.RawValue, Is.InstanceOf<byte[]>());
            Assert.That(allBytes.First().Key.RawValue, Is.InstanceOf<byte[]>());
        }
    }

    [Test]
    public void HGetDel()
    {
        const string key = "{189}:Hash:HGetDel:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var getdel = this._redis.Hash.HGetDel(key, field2);
        Assert.That(getdel.IsNullOrEmpty);

        int result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        getdel = this._redis.Hash.HGetDel(key, field2);
        Assert.That(getdel.ToString(), Is.EqualTo(field2));

        var getdelArray = this._redis.Hash.HGetDel(key, [field2, field1, "", "none", "field3"]);
        Assert.That(getdelArray, Has.Length.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(getdelArray[0].IsNull);
            Assert.That(getdelArray[1].ToString(), Is.EqualTo(field1));
            Assert.That(getdelArray[2].IsNull);
            Assert.That(getdelArray[3].IsNull);
            Assert.That(getdelArray[3].ToString(), Is.Null);
        }
    }

    [Test]
    public void HGetEx()
    {
        const string key = "{190}:Hash:HGetEx:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var getex = this._redis.Hash.HGetEx(key, field2);
        Assert.That(getex.IsNull);

        int result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        getex = this._redis.Hash.HGetEx(key, field2);
        Assert.That(getex.ToString(), Is.EqualTo(field2));

        long ttl = this._redis.Hash.HTtl(key, field2);
        Assert.That(ttl, Is.EqualTo(-1));

        ttl = this._redis.Hash.HTtl("none", field2);
        Assert.That(ttl, Is.EqualTo(-2));

        getex = this._redis.Hash.HGetEx(key, field1, TimeSpan.FromSeconds(30));
        Assert.That(getex.ToString(), Is.EqualTo(field1));

        ttl = this._redis.Hash.HTtl(key, field1);
        Assert.That(ttl >= 28 && ttl <= 31);

        getex = this._redis.Hash.HGetEx(key, "field3", DateTime.Now.AddMinutes(1));
        Assert.That(getex.ToString(), Is.EqualTo(string.Empty));

        ttl = this._redis.Hash.HTtl(key, "field3");
        Assert.That(ttl >= 58 && ttl <= 61);

        long pttl = this._redis.Hash.HPTtl(key, "field3");
        Assert.That(pttl >= 58 * 1000 && ttl <= 60 * 1000);

        pttl = this._redis.Hash.HPTtl(key, "none");
        Assert.That(pttl, Is.EqualTo(-2));

        var getexs = this._redis.Hash.HGetEx(key, [field1, field2, "none"], DateTimeOffset.UtcNow.AddSeconds(30));
        Assert.That(getexs, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(getexs[0].ToString(), Is.EqualTo(field1));
            Assert.That(getexs[1].ToString(), Is.EqualTo(field2));
            Assert.That(getexs[2].IsNull);
        }

        var pttls = this._redis.Hash.HPTtl(key, [field1, field2, "none"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pttls[0] >= 28 * 1000 && pttls[0] <= 31 * 1000);
            Assert.That((sbyte)pttls[2], Is.EqualTo(-2));
        }

        var ttls = this._redis.Hash.HTtl(key, [field1, field2, "not key"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ttls[0] >= 28 && ttls[0] <= 31);
            Assert.That((int)ttls[2], Is.EqualTo(-2));
        }

        getexs = this._redis.Hash.HGetEx(key, [field1, field2, "none"], DateTimeOffset.UtcNow.AddSeconds(30), ExpirationFlags.PERSIST);
        Assert.That(getexs, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(getexs[0].ToString(), Is.EqualTo(field1));
            Assert.That(getexs[1].ToString(), Is.EqualTo(field2));
            Assert.That(getexs[2].IsNull);
        }

        ttls = this._redis.Hash.HTtl(key, [field1, field2, "not key"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)ttls[0], Is.EqualTo(-1));
            Assert.That((int)ttls[2], Is.EqualTo(-2));
        }
    }

    [Test]
    public void HIncrBy()
    {
        const string key = "{191}:Hash:HIncrBy:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        int number = this._redis.Hash.HIncrBy(key, field1);
        Assert.That(number, Is.EqualTo(1));

        number = this._redis.Hash.HIncrBy(key, field1);
        Assert.That(number, Is.EqualTo(2));

        number = this._redis.Hash.HIncrBy(key, field2);
        Assert.That(number, Is.EqualTo(1));

        number = this._redis.Hash.HIncrBy(key, field2, 519);
        Assert.That(number, Is.EqualTo(520));

        number = this._redis.Hash.HIncrBy(key, field1, -522);
        Assert.That(number, Is.EqualTo(-520));
    }

    [Test]
    public void HIncrByFloat()
    {
        const string key = "{192}:Hash:HIncrByFloat:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        double number = this._redis.Hash.HIncrByFloat(key, field1, 1);
        Assert.That(number, Is.EqualTo(1));

        number = this._redis.Hash.HIncrByFloat(key, field1, 1.1);
        Assert.That(number, Is.EqualTo(2.1));

        number = this._redis.Hash.HIncrByFloat(key, field2, 1);
        Assert.That(number, Is.EqualTo(1));

        number = this._redis.Hash.HIncrByFloat(key, field2, 519.1314);
        Assert.That(number, Is.EqualTo(520.1314));

        number = this._redis.Hash.HIncrByFloat(key, field1, -522);
        Assert.That(number, Is.EqualTo(-519.9));
    }

    [Test]
    public void HKeys()
    {
        const string key = "{193}:Hash:HKeys:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var keys = this._redis.Hash.HKeys(key);
        Assert.That(keys, Is.Null);

        int result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
        });
        Assert.That(result, Is.EqualTo(2));

        keys = this._redis.Hash.HKeys(key);
        Assert.That(keys, Has.Length.EqualTo(2));
        Assert.That(keys, Has.Some.Matches<RedisResult>(u => u == field1));
        Assert.That(keys, Has.Some.Matches<RedisResult>(u => u == field2));
    }

    [Test]
    public void HLen()
    {
        const string key = "{194}:Hash:HLen:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        short len = this._redis.Hash.HLen(key);
        Assert.That(len, Is.Zero);

        int result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
        });
        Assert.That(result, Is.EqualTo(2));

        len = this._redis.Hash.HLen(key);
        Assert.That(len, Is.EqualTo(2));

        int del = this._redis.Hash.HDel(key, field1);
        Assert.That(del, Is.EqualTo(1));

        len = this._redis.Hash.HLen(key);
        Assert.That(len, Is.EqualTo(1));
    }

    [Test]
    public void HMGet()
    {
        const string key = "{195}:Hash:HMGet:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";

        var hmget = this._redis.Hash.HMGet(key, [field1, field2, field3]);
        Assert.That(hmget, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(hmget[0].IsNull);
            Assert.That(hmget[1].IsNull);
            Assert.That(hmget[2].IsNull);
        }

        int result = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { field3, field3 },
        });
        Assert.That(result, Is.EqualTo(3));

        hmget = this._redis.Hash.HMGet(key, [field1, field3, "none"]);
        Assert.That(hmget, Has.Length.EqualTo(3));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(!hmget[0].IsNull);
            Assert.That(!hmget[1].IsNull);
            Assert.That(hmget[2].IsNull);

            Assert.That(hmget, Has.Some.Matches<RedisResult>(u => u == field3));
            Assert.That(hmget, Has.Some.Matches<RedisResult>(u => u == field1));
        }
    }

    [Test]
    public void HMSet()
    {
        const string key = "{196}:Hash:HMSet:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";

        var fieldReuslt = this._redis.Hash.HGet(key, field1);
        Assert.That(fieldReuslt.IsNull);

        var result = this._redis.Hash.HMSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { field3, field3 },
        });
        Assert.That(result);

        int len = this._redis.Hash.HLen(key);
        Assert.That(len, Is.EqualTo(3));

        fieldReuslt = this._redis.Hash.HGet(key, field1);
        Assert.That(fieldReuslt.ToString(), Is.EqualTo(field1));
    }

    [Test]
    public void HPersist()
    {
        const string key = "{197}:Hash:HPersist:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";

        var result = this._redis.Hash.HPersist(key, field1);
        Assert.That(result, Is.EqualTo(-2));

        var set = this._redis.Hash.HMSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { field3, field3 },
        });
        Assert.That(set);

        result = this._redis.Hash.HPersist(key, field1);
        Assert.That(result, Is.EqualTo(-1));

        _ = this._redis.Hash.HExpire(key, [field1, field2, field3], 100);

        int ttl1 = this._redis.Hash.HTtl(key, field1);
        Assert.That(ttl1, Is.GreaterThan(98));

        result = this._redis.Hash.HPersist(key, field1);
        Assert.That(result, Is.EqualTo(1));

        var resultArr = this._redis.Hash.HPersist(key, [field1, field2, field3, "none"]);
        Assert.That(resultArr, Has.Length.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)resultArr[0], Is.EqualTo(-1));
            Assert.That((int)resultArr[1], Is.EqualTo(1));
            Assert.That((int)resultArr[2], Is.EqualTo(1));
            Assert.That((int)resultArr[3], Is.EqualTo(-2));
        }

        ttl1 = this._redis.Hash.HTtl(key, field1);
        Assert.That(ttl1, Is.EqualTo(-1));
    }

    [Test]
    public void HRandField()
    {
        const string key = "{198}:Hash:HRandField:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";
        const string field4 = "field4";

        var result = this._redis.Hash.HRandField(key);
        Assert.That(result.IsNull);

        int set = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "赤道的边境万里无云天很清" },
            { field2, "爱你的事情说了千遍有回音" },
            { field3, "岸边的丘陵崎岖不平浪入侵" },
            { field4, "我却很专心分辨得出你的声音" },
        });
        Assert.That(set, Is.EqualTo(4));

        string? str = this._redis.Hash.HRandField(key);
        Assert.That(!string.IsNullOrEmpty(str));

        result = this._redis.Hash.HRandField(key, 2);

        RedisResult[]? fieldArray = result;
        Assert.That(fieldArray, Has.Length.EqualTo(2));

        fieldArray = this._redis.Hash.HRandField(key, 10);
        Assert.That(fieldArray, Has.Length.EqualTo(4));

        fieldArray = this._redis.Hash.HRandField(key, -10);
        Assert.That(fieldArray, Has.Length.EqualTo(10));

        KeyValuePair<RedisResult, RedisResult>[]? array = this._redis.Hash.HRandField(key, -10, true);
        Assert.That(array, Has.Length.EqualTo(10));

        array = this._redis.Hash.HRandField(key, 10, true);
        Assert.That(array, Has.Length.EqualTo(4));

        array = this._redis.Hash.HRandField("none", 10, true);
        Assert.That(array, Is.Null);
    }

    [Test]
    public void HSetEx()
    {
        const string key = "{199}:Hash:HSetEx:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";
        const string field4 = "field4";

        var result = this._redis.Hash.HSetEx(key, field1, "你出现在我诗的每一页", TimeSpan.FromSeconds(10), when: FieldExistFlags.FXX);
        Assert.That(result, Is.False);

        result = this._redis.Hash.HSetEx(key, field1, "你出现在我诗的每一页", TimeSpan.FromSeconds(10));
        Assert.That(result, Is.True);

        int field1Ttl = this._redis.Hash.HTtl(key, field1);
        Assert.That(field1Ttl, Is.GreaterThan(8));

        result = this._redis.Hash.HSetEx(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "我接着写" },
            { field2, "把永远爱你写进诗的结尾" },
            { field3, "你是我唯一想要的了解" },
            { field4, "李❤❤" },
        }, DateTime.Now.AddSeconds(30));
        Assert.That(result, Is.True);

        field1Ttl = this._redis.Hash.HTtl(key, field1);
        Assert.That(field1Ttl, Is.GreaterThan(28));

        result = this._redis.Hash.HSetEx(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "我接着写" },
            { field2, "把永远爱你写进诗的结尾" },
            { field3, "你是我唯一想要的了解" },
            { field4, "李❤❤" },
        }, DateTime.Now.AddSeconds(60), ExpirationFlags.KEEPTTL);
        Assert.That(result, Is.True);

        field1Ttl = this._redis.Hash.HTtl(key, field3);
        Assert.That(field1Ttl, Is.GreaterThan(28));

        result = this._redis.Hash.HSetEx(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "我接着写" },
            { field2, "把永远爱你写进诗的结尾" },
            { field3, "你是我唯一想要的了解" },
            { field4, "李❤❤" },
        }, when: FieldExistFlags.FNX);
        Assert.That(result, Is.False);

        result = this._redis.Hash.HSetEx(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "我接着写" },
            { field2, "把永远爱你写进诗的结尾" },
            { field3, "你是我唯一想要的了解" },
            { field4, "李❤❤" },
        }, when: FieldExistFlags.FXX);
        Assert.That(result, Is.True);

        _ = this._redis.Hash.HDel(key, field1);

        result = this._redis.Hash.HSetEx(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "我接着写" },
            { field2, "把永远爱你写进诗的结尾" },
            { field3, "你是我唯一想要的了解" },
            { field4, "李❤❤" },
        }, when: FieldExistFlags.FXX);
        Assert.That(result, Is.False);
    }

    [Test]
    public void HSetNx()
    {
        const string key = "{200}:Hash:HSetNx:Key";
        const string field1 = "field1";

        var ok = this._redis.Hash.HSetNx(key, field1, field1, CommandFlags.ReturnBytes);
        Assert.That(ok, Is.True);

        ok = this._redis.Hash.HSetNx(key, field1, "甜甜的");
        Assert.That(ok, Is.False);

        string? field1Value = this._redis.Hash.HGet(key, field1);
        Assert.That(field1Value, Is.EqualTo(field1));
    }

    [Test]
    public void HStrLen()
    {
        const string key = "{201}:Hash:HStrLen:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string value1 = "不管是演动物或植物，你一定都要演的让人家羡慕。";
        const string value2 = "How old arr you!!";

        int set = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, value1 },
            { field2, value2 },
        });
        Assert.That(set, Is.EqualTo(2));

        int field1Len = this._redis.Hash.HStrLen(key, field1);
        Assert.That(field1Len, Is.EqualTo(Encoding.UTF8.GetBytes(value1).Length));

        int field2Len = this._redis.Hash.HStrLen(key, field2, CommandFlags.ReturnBytes);
        Assert.That(field2Len, Is.EqualTo(Encoding.UTF8.GetBytes(value2).Length));

        int noneLen = this._redis.Hash.HStrLen(key, "none");
        Assert.That(noneLen, Is.Zero);

        noneLen = this._redis.Hash.HStrLen("none_hash", field2);
        Assert.That(noneLen, Is.Zero);
    }

    [Test]
    public void HVals()
    {
        const string key = "{202}:Hash:HVals:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";
        const string field4 = "field4";

        const string value1 = "💕😍😂👨🏻‍🏫😊👌🐧";

        var vals = this._redis.Hash.HVals(key);
        Assert.That(vals, Is.Null);

        int set = this._redis.Hash.HSet(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1,  value1},
            { field2, "How are you!~" },
            { field3, string.Empty },
            { field4, "这是一个申请的测试" },
        });
        Assert.That(set, Is.EqualTo(4));

        vals = this._redis.Hash.HVals(key);
        Assert.That(vals, Has.Length.EqualTo(4));
        Assert.That(vals[0].ToString(), Is.EqualTo(value1));
    }
}
