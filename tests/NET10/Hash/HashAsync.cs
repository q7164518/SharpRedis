using SharpRedis;
using System.Text;
using static SharpRedis.RedisHash;

namespace NET10.Hash;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class HashAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public async Task HSetAsync()
    {
        const string key = "{203}:Hash:HSetAsync:Key";
        const string field = "field";
        const string value = "Hash:HSetAsync:Value";

        string? get = await this._redis.Hash.HGetAsync(key, field);
        Assert.That(get, Is.Null);

        int result = await this._redis.Hash.HSetAsync(key, field, value);
        Assert.That(result, Is.EqualTo(1));

        get = await this._redis.Hash.HGetAsync(key, field);
        Assert.That(get, Is.EqualTo(value));

        result = await this._redis.Hash.HSetAsync(key, "field", "value1");
        Assert.That(result, Is.Zero);

        get = await this._redis.Hash.HGetAsync(key, field);
        Assert.That(get, Is.EqualTo("value1"));

        result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { "field", "value2" },
            { "", "" },
            { "3", "33" },
        });
        Assert.That(result, Is.EqualTo(2));

        get = await this._redis.Hash.HGetAsync(key, "3");
        Assert.That(get, Is.EqualTo("33"));
    }

    [Test]
    public async Task HDelAsync()
    {
        const string key = "{204}:Hash:HDelAsync:Key";
        const string field = "field";

        int del = await this._redis.Hash.HDelAsync(key, field);
        Assert.That(del, Is.Zero);

        int result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field, field },
            { "2", string.Empty },
            { "3", "33" },
        });
        Assert.That(result, Is.EqualTo(3));

        del = await this._redis.Hash.HDelAsync(key, field);
        Assert.That(del, Is.EqualTo(1));

        del = await this._redis.Hash.HDelAsync(key, [field, "2", "3", "44"]);
        Assert.That(del, Is.EqualTo(2));
    }

    [Test]
    public async Task HExistsAsync()
    {
        const string key = "{205}:Hash:HExistsAsync:Key";
        const string field = "field";

        var exists = await this._redis.Hash.HExistsAsync(key, field);
        Assert.That(!exists);

        int set = await this._redis.Hash.HSetAsync(key, field, string.Empty);
        Assert.That(set, Is.EqualTo(1));

        exists = this._redis.Hash.HExists(key, field);
        Assert.That(exists);

        exists = await this._redis.Hash.HExistsAsync(key, "none_field");
        Assert.That(!exists);
    }

    [Test]
    public async Task HExpireAsync()
    {
        const string key = "{206}:Hash:HExpireAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        int result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        var expire = await this._redis.Hash.HExpireAsync(key, field1, 100);
        Assert.That(expire);

        expire = await this._redis.Hash.HExpireAsync(key, "none", TimeSpan.FromSeconds(100));
        Assert.That(!expire);

        expire = await this._redis.Hash.HExpireAsync("none_hash", field1, 100);
        Assert.That(!expire);

        expire = await this._redis.Hash.HExpireAsync(key, field1, 100, ExpirationWhen.LT);
        Assert.That(!expire);

        expire = await this._redis.Hash.HExpireAsync(key, field1, 100, ExpirationWhen.XX);
        Assert.That(expire);

        expire = await this._redis.Hash.HExpireAsync(key, field1, 100, ExpirationWhen.NX);
        Assert.That(!expire);

        expire = await this._redis.Hash.HExpireAsync(key, field2, 100, ExpirationWhen.XX);
        Assert.That(!expire);

        expire = await this._redis.Hash.HExpireAsync(key, field2, TimeSpan.FromSeconds(100), ExpirationWhen.NX);
        Assert.That(expire);

        var expires = await this._redis.Hash.HExpireAsync(key, new List<SharpValue> { field1, field2 }, 100);
        Assert.That(expires, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(expires[0]);
            Assert.That(expires[1]);
        }

        var unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expireTime = await this._redis.Hash.HExpireTimeAsync(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeSeconds));
        Assert.That(expireTime - unixTimeSeconds, Is.GreaterThanOrEqualTo(98));

        var expireTimes = await this._redis.Hash.HExpireTimeAsync(key, [field2, "none", "field3"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((long)expireTimes[0], Is.GreaterThan(unixTimeSeconds));
            Assert.That(expireTimes[0] - unixTimeSeconds, Is.GreaterThanOrEqualTo(98));
            Assert.That((int)expireTimes[1], Is.EqualTo(-2));
            Assert.That((int)expireTimes[2], Is.EqualTo(-1));
        }
    }

    [Test]
    public async Task HPExpireAsync()
    {
        const string key = "{207}:Hash:HPExpireAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        int result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        var expire = await this._redis.Hash.HPExpireAsync(key, field1, 5000);
        Assert.That(expire);

        expire = await this._redis.Hash.HPExpireAsync(key, "none", TimeSpan.FromMilliseconds(5000));
        Assert.That(!expire);

        expire = await this._redis.Hash.HPExpireAsync("none_hash", field1, 5000);
        Assert.That(!expire);

        expire = await this._redis.Hash.HPExpireAsync(key, field1, 5000, ExpirationWhen.LT);
        Assert.That(!expire);

        expire = await this._redis.Hash.HPExpireAsync(key, field1, 5000, ExpirationWhen.XX);
        Assert.That(expire);

        expire = await this._redis.Hash.HPExpireAsync(key, field1, TimeSpan.FromMilliseconds(5000), ExpirationWhen.NX);
        Assert.That(!expire);

        expire = await this._redis.Hash.HPExpireAsync(key, field2, 5000, ExpirationWhen.XX);
        Assert.That(!expire);

        expire = await this._redis.Hash.HPExpireAsync(key, field2, 5000, ExpirationWhen.NX);
        Assert.That(expire);

        

        var expires = await this._redis.Hash.HPExpireAsync(key, new List<SharpValue> { field1, field2, "none" }, 3000);
        Assert.That(expires, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(expires[0]);
            Assert.That(expires[1]);
            Assert.That(!expires[2]);
        }

        var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var expireTime = await this._redis.Hash.HPExpireTimeAsync(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeMilliseconds));
        Assert.That(expireTime - unixTimeMilliseconds, Is.GreaterThanOrEqualTo(10));

        var expireTimes = await this._redis.Hash.HPExpireTimeAsync(key, [field2, "none", "field3"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((long)expireTimes[0], Is.GreaterThan(unixTimeMilliseconds));
            Assert.That(expireTimes[0] - unixTimeMilliseconds, Is.GreaterThanOrEqualTo(10));
            Assert.That((short)expireTimes[1], Is.EqualTo(-2));
            Assert.That((short)expireTimes[2], Is.EqualTo(-1));
        }
    }

    [Test]
    public async Task HExpireAtAsync()
    {
        const string key = "{208}:Hash:HExpireAtAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var ok = await this._redis.Hash.HExpireAtAsync(key, field1, DateTime.Now.AddSeconds(100));
        Assert.That(!ok);

        int result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        ok = await this._redis.Hash.HExpireAtAsync(key, field1, DateTime.Now.AddSeconds(100));
        Assert.That(ok);

        ok = this._redis.Hash.HExpireAt(key, field1, DateTime.Now.AddSeconds(10), ExpirationWhen.GT);
        Assert.That(!ok);

        var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var expireTime = this._redis.Hash.HPExpireTime(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeMilliseconds));

        ok = await this._redis.Hash.HExpireAtAsync(key, field2, DateTime.Now.AddSeconds(100), ExpirationWhen.XX);
        Assert.That(!ok);

        ok = await this._redis.Hash.HExpireAtAsync(key, field2, DateTime.Now.AddSeconds(100), ExpirationWhen.NX);
        Assert.That(ok);

        ok = await this._redis.Hash.HExpireAtAsync(key, field2, new DateTimeOffset(DateTime.Now.AddSeconds(80)).ToUnixTimeSeconds(), ExpirationWhen.LT);
        Assert.That(ok);

        expireTime = this._redis.Hash.HPExpireTime(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeMilliseconds));
        Assert.That(expireTime - unixTimeMilliseconds, Is.GreaterThanOrEqualTo(78));

        var oks = await this._redis.Hash.HExpireAtAsync(key, [field2, field1, "none", "field3"], DateTime.Now.AddSeconds(100));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(oks[0]);
            Assert.That(oks[1]);
            Assert.That(!oks[2]);
            Assert.That(oks[3]);
        }

        oks = await this._redis.Hash.HExpireAtAsync(key, [field2, field1, "none", "field3"], new DateTimeOffset(DateTime.Now.AddSeconds(80)).ToUnixTimeSeconds(), ExpirationWhen.XX);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(oks[0]);
            Assert.That(oks[1]);
            Assert.That(!oks[2]);
            Assert.That(oks[3]);
        }
    }

    [Test]
    public async Task HPExpireAtAsync()
    {
        const string key = "{209}:Hash:HPExpireAtAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var ok = await this._redis.Hash.HPExpireAtAsync(key, field1, DateTime.Now.AddSeconds(100));
        Assert.That(!ok);

        int result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        ok = await this._redis.Hash.HPExpireAtAsync(key, field1, DateTime.Now.AddSeconds(100));
        Assert.That(ok);

        ok = await this._redis.Hash.HPExpireAtAsync(key, field1, DateTime.Now.AddSeconds(10), ExpirationWhen.GT);
        Assert.That(!ok);

        var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var expireTime = await this._redis.Hash.HPExpireTimeAsync(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeMilliseconds));

        ok = await this._redis.Hash.HPExpireAtAsync(key, field2, DateTime.Now.AddSeconds(100), ExpirationWhen.XX);
        Assert.That(!ok);

        ok = await this._redis.Hash.HPExpireAtAsync(key, field2, DateTime.Now.AddSeconds(100), ExpirationWhen.NX);
        Assert.That(ok);

        ok = await this._redis.Hash.HPExpireAtAsync(key, field2, new DateTimeOffset(DateTime.Now.AddSeconds(80)).ToUnixTimeMilliseconds(), ExpirationWhen.LT);
        Assert.That(ok);

        expireTime = this._redis.Hash.HPExpireTime(key, field1);
        Assert.That(expireTime, Is.GreaterThan(unixTimeMilliseconds));
        Assert.That(expireTime - unixTimeMilliseconds, Is.GreaterThanOrEqualTo(78));

        var oks = await this._redis.Hash.HPExpireAtAsync(key, [field2, field1, "none", "field3"], DateTime.Now.AddSeconds(100));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(oks[0]);
            Assert.That(oks[1]);
            Assert.That(!oks[2]);
            Assert.That(oks[3]);
        }

        oks = await this._redis.Hash.HPExpireAtAsync(key, [field2, field1, "none", "field3"], new DateTimeOffset(DateTime.Now.AddSeconds(80)).ToUnixTimeMilliseconds(), ExpirationWhen.XX);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(oks[0]);
            Assert.That(oks[1]);
            Assert.That(!oks[2]);
            Assert.That(oks[3]);
        }
    }

    [Test]
    public async Task HGetAllAsync()
    {
        const string key = "{210}:Hash:HGetAllAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var all = await this._redis.Hash.HGetAllAsync(key);
        Assert.That(all, Is.Null);

        int result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        all = await this._redis.Hash.HGetAllAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(all!, Has.Count.EqualTo(3));
            Assert.That(all![field1].ToString(), Is.EqualTo(field1));
            Assert.That(all!["field3"].ToString(), Is.EqualTo(string.Empty));
        }

        var allBytes = await this._redis.Hash.HGetAllAsync(key, CommandFlags.ReturnBytes);
        Assert.That(allBytes!, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(allBytes.First().Value.RawValue, Is.InstanceOf<byte[]>());
            Assert.That(allBytes.First().Key.RawValue, Is.InstanceOf<byte[]>());
        }
    }

    [Test]
    public async Task HGetDelAsync()
    {
        const string key = "{211}:Hash:HGetDelAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var getdel = await this._redis.Hash.HGetDelAsync(key, field2);
        Assert.That(getdel.IsNullOrEmpty);

        int result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        getdel = await this._redis.Hash.HGetDelAsync(key, field2);
        Assert.That(getdel.ToString(), Is.EqualTo(field2));

        var getdelArray = await this._redis.Hash.HGetDelAsync(key, [field2, field1, "", "none", "field3"]);
        Assert.That(getdelArray, Has.Length.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(getdelArray[0].IsNull);
            Assert.That(getdelArray[1].ToString(), Is.EqualTo(field1));
            Assert.That(getdelArray[2].IsNull);
            Assert.That(getdelArray[3].IsNull);
        }
    }

    [Test]
    public async Task HGetExAsync()
    {
        const string key = "{212}:Hash:HGetExAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var getex = await this._redis.Hash.HGetExAsync(key, field2);
        Assert.That(getex.IsNull);

        int result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { "field3", string.Empty },
        });
        Assert.That(result, Is.EqualTo(3));

        getex = await this._redis.Hash.HGetExAsync(key, field2);
        Assert.That(getex.ToString(), Is.EqualTo(field2));

        int ttl = await this._redis.Hash.HTtlAsync(key, field2);
        Assert.That(ttl, Is.EqualTo(-1));

        ttl = await this._redis.Hash.HTtlAsync("none", field2);
        Assert.That(ttl, Is.EqualTo(-2));

        getex = await this._redis.Hash.HGetExAsync(key, field1, TimeSpan.FromSeconds(30));
        Assert.That(getex.ToString(), Is.EqualTo(field1));

        ttl = await this._redis.Hash.HTtlAsync(key, field1);
        Assert.That(ttl >= 28 && ttl <= 31);

        getex = await this._redis.Hash.HGetExAsync(key, "field3", DateTime.Now.AddMinutes(1));
        Assert.That(getex.ToString(), Is.EqualTo(string.Empty));

        ttl = await this._redis.Hash.HTtlAsync(key, "field3");
        Assert.That(ttl >= 58 && ttl <= 61);

        int pttl = await this._redis.Hash.HPTtlAsync(key, "field3");
        Assert.That(pttl >= 58 * 1000 && ttl <= 61 * 1000);

        pttl = await this._redis.Hash.HPTtlAsync(key, "none");
        Assert.That(pttl, Is.EqualTo(-2));

        var getexs = await this._redis.Hash.HGetExAsync(key, [field1, field2, "none"], DateTimeOffset.UtcNow.AddSeconds(30));
        Assert.That(getexs, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(getexs[0].ToString(), Is.EqualTo(field1));
            Assert.That(getexs[1].ToString(), Is.EqualTo(field2));
            Assert.That(getexs[2].IsNull);
        }

        var pttls = await this._redis.Hash.HPTtlAsync(key, [field1, field2, "none"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pttls[0] >= 28 * 1000 && pttls[0] <= 31 * 1000);
            Assert.That((int)pttls[2], Is.EqualTo(-2));
        }

        var ttls = await this._redis.Hash.HTtlAsync(key, [field1, field2, "not key"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ttls[0] >= 28 && ttls[0] <= 31);
            Assert.That((int)ttls[2], Is.EqualTo(-2));
        }

        getexs = await this._redis.Hash.HGetExAsync(key, [field1, field2, "none"], DateTimeOffset.UtcNow.AddSeconds(30), ExpirationFlags.PERSIST);
        Assert.That(getexs, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(getexs[0].ToString(), Is.EqualTo(field1));
            Assert.That(getexs[1].ToString(), Is.EqualTo(field2));
            Assert.That(getexs[2].IsNull);
        }

        ttls = await this._redis.Hash.HTtlAsync(key, [field1, field2, "not key"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)ttls[0], Is.EqualTo(-1));
            Assert.That((int)ttls[2], Is.EqualTo(-2));
        }
    }

    [Test]
    public async Task HIncrByAsync()
    {
        const string key = "{213}:Hash:HIncrByAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        int number = await this._redis.Hash.HIncrByAsync(key, field1);
        Assert.That(number, Is.EqualTo(1));

        number = await this._redis.Hash.HIncrByAsync(key, field1);
        Assert.That(number, Is.EqualTo(2));

        number = await this._redis.Hash.HIncrByAsync(key, field2);
        Assert.That(number, Is.EqualTo(1));

        number = await this._redis.Hash.HIncrByAsync(key, field2, 519);
        Assert.That(number, Is.EqualTo(520));

        number = await this._redis.Hash.HIncrByAsync(key, field1, -522);
        Assert.That(number, Is.EqualTo(-520));
    }

    [Test]
    public async Task HIncrByFloatAsync()
    {
        const string key = "{214}:Hash:HIncrByFloatAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        double number = await this._redis.Hash.HIncrByFloatAsync(key, field1, 1);
        Assert.That(number, Is.EqualTo(1));

        number = await this._redis.Hash.HIncrByFloatAsync(key, field1, 1.1);
        Assert.That(number, Is.EqualTo(2.1));

        number = await this._redis.Hash.HIncrByFloatAsync(key, field2, 1);
        Assert.That(number, Is.EqualTo(1));

        number = await this._redis.Hash.HIncrByFloatAsync(key, field2, 519.1314);
        Assert.That(number, Is.EqualTo(520.1314));

        number = await this._redis.Hash.HIncrByFloatAsync(key, field1, -522);
        Assert.That(number, Is.EqualTo(-519.9));
    }

    [Test]
    public async Task HKeysAsync()
    {
        const string key = "{215}:Hash:HKeysAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        var keys = await this._redis.Hash.HKeysAsync(key);
        Assert.That(keys, Is.Null);

        int result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
        });
        Assert.That(result, Is.EqualTo(2));

        keys = await this._redis.Hash.HKeysAsync(key);
        Assert.That(keys, Has.Length.EqualTo(2));
        Assert.That(keys, Has.Some.Matches<RedisResult>(u => u == field1));
        Assert.That(keys, Has.Some.Matches<RedisResult>(u => u == field2));
    }

    [Test]
    public async Task HLenAsync()
    {
        const string key = "{216}:Hash:HLenAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";

        short len = await this._redis.Hash.HLenAsync(key);
        Assert.That(len, Is.Zero);

        int result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
        });
        Assert.That(result, Is.EqualTo(2));

        len = await this._redis.Hash.HLenAsync(key);
        Assert.That(len, Is.EqualTo(2));

        short del = await this._redis.Hash.HDelAsync(key, field1);
        Assert.That(del, Is.EqualTo(1));

        len = await this._redis.Hash.HLenAsync(key);
        Assert.That(len, Is.EqualTo(1));
    }

    [Test]
    public async Task HMGetAsync()
    {
        const string key = "{217}:Hash:HMGetAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";

        var hmget = await this._redis.Hash.HMGetAsync(key, [field1, field2, field3]);
        Assert.That(hmget, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(hmget[0].IsNull);
            Assert.That(hmget[1].IsNull);
            Assert.That(hmget[2].IsNull);
        }

        int result = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { field3, field3 },
        });
        Assert.That(result, Is.EqualTo(3));

        hmget = await this._redis.Hash.HMGetAsync(key, [field1, field3, "none"]);
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
    public async Task HMSetAsync()
    {
        const string key = "{218}:Hash:HMSetAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";

        var fieldReuslt = await this._redis.Hash.HGetAsync(key, field1);
        Assert.That(fieldReuslt.IsNull);

        var result = await this._redis.Hash.HMSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { field3, field3 },
        });
        Assert.That(result);

        int len = await this._redis.Hash.HLenAsync(key);
        Assert.That(len, Is.EqualTo(3));

        fieldReuslt = await this._redis.Hash.HGetAsync(key, field1);
        Assert.That(fieldReuslt.ToString(), Is.EqualTo(field1));
    }

    [Test]
    public async Task HPersistAsync()
    {
        const string key = "{219}:Hash:HPersistAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";

        var result = await this._redis.Hash.HPersistAsync(key, field1);
        Assert.That(result, Is.EqualTo(-2));

        var set = await this._redis.Hash.HMSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, field1 },
            { field2, field2 },
            { field3, field3 },
        });
        Assert.That(set);

        result = await this._redis.Hash.HPersistAsync(key, field1);
        Assert.That(result, Is.EqualTo(-1));

        _ = this._redis.Hash.HExpire(key, [field1, field2, field3], 100);

        long ttl1 = await this._redis.Hash.HTtlAsync(key, field1);
        Assert.That(ttl1, Is.GreaterThan(98));

        result = await this._redis.Hash.HPersistAsync(key, field1);
        Assert.That(result, Is.EqualTo(1));

        var resultArr = await this._redis.Hash.HPersistAsync(key, [field1, field2, field3, "none"]);
        Assert.That(resultArr, Has.Length.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)resultArr[0], Is.EqualTo(-1));
            Assert.That((int)resultArr[1], Is.EqualTo(1));
            Assert.That((int)resultArr[2], Is.EqualTo(1));
            Assert.That((int)resultArr[3], Is.EqualTo(-2));
        }

        ttl1 = await this._redis.Hash.HTtlAsync(key, field1);
        Assert.That(ttl1, Is.EqualTo(-1));
    }

    [Test]
    public async Task HRandFieldAsync()
    {
        const string key = "{220}:Hash:HRandFieldAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";
        const string field4 = "field4";

        var result = await this._redis.Hash.HRandFieldAsync(key);
        Assert.That(result.IsNull);

        int set = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "赤道的边境万里无云天很清" },
            { field2, "爱你的事情说了千遍有回音" },
            { field3, "岸边的丘陵崎岖不平浪入侵" },
            { field4, "我却很专心分辨得出你的声音" },
        });
        Assert.That(set, Is.EqualTo(4));

        string? str = await this._redis.Hash.HRandFieldAsync(key);
        Assert.That(!string.IsNullOrEmpty(str));

        result = await this._redis.Hash.HRandFieldAsync(key, 2);

        RedisResult[]? fieldArray = result;
        Assert.That(fieldArray, Has.Length.EqualTo(2));

        fieldArray = await this._redis.Hash.HRandFieldAsync(key, 10);
        Assert.That(fieldArray, Has.Length.EqualTo(4));

        fieldArray = await this._redis.Hash.HRandFieldAsync(key, -10);
        Assert.That(fieldArray, Has.Length.EqualTo(10));

        KeyValuePair<RedisResult, RedisResult>[]? array = await this._redis.Hash.HRandFieldAsync(key, -10, true);
        Assert.That(array, Has.Length.EqualTo(10));

        array = await this._redis.Hash.HRandFieldAsync(key, 10, true);
        Assert.That(array, Has.Length.EqualTo(4));

        array = await this._redis.Hash.HRandFieldAsync("none", 10, true);
        Assert.That(array, Is.Null);
    }

    [Test]
    public async Task HSetExAsync()
    {
        const string key = "{221}:Hash:HSetExAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";
        const string field4 = "field4";

        var result = await this._redis.Hash.HSetExAsync(key, field1, "你出现在我诗的每一页", TimeSpan.FromSeconds(10), when: FieldExistFlags.FXX);
        Assert.That(result, Is.False);

        result = await this._redis.Hash.HSetExAsync(key, field1, "你出现在我诗的每一页", TimeSpan.FromSeconds(10));
        Assert.That(result, Is.True);

        int field1Ttl = await this._redis.Hash.HTtlAsync(key, field1);
        Assert.That(field1Ttl, Is.GreaterThan(8));

        result = await this._redis.Hash.HSetExAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "我接着写" },
            { field2, "把永远爱你写进诗的结尾" },
            { field3, "你是我唯一想要的了解" },
            { field4, "李❤❤" },
        }, DateTime.Now.AddSeconds(30));
        Assert.That(result, Is.True);

        field1Ttl = await this._redis.Hash.HTtlAsync(key, field1);
        Assert.That(field1Ttl, Is.GreaterThan(28));

        result = await this._redis.Hash.HSetExAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "我接着写" },
            { field2, "把永远爱你写进诗的结尾" },
            { field3, "你是我唯一想要的了解" },
            { field4, "李❤❤" },
        }, DateTime.Now.AddSeconds(60), ExpirationFlags.KEEPTTL);
        Assert.That(result, Is.True);

        field1Ttl = await this._redis.Hash.HTtlAsync(key, field3);
        Assert.That(field1Ttl, Is.GreaterThan(28));

        result = await this._redis.Hash.HSetExAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "我接着写" },
            { field2, "把永远爱你写进诗的结尾" },
            { field3, "你是我唯一想要的了解" },
            { field4, "李❤❤" },
        }, when: FieldExistFlags.FNX);
        Assert.That(result, Is.False);

        result = await this._redis.Hash.HSetExAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "我接着写" },
            { field2, "把永远爱你写进诗的结尾" },
            { field3, "你是我唯一想要的了解" },
            { field4, "李❤❤" },
        }, when: FieldExistFlags.FXX);
        Assert.That(result, Is.True);

        _ = await this._redis.Hash.HDelAsync(key, field1);

        result = await this._redis.Hash.HSetExAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, "我接着写" },
            { field2, "把永远爱你写进诗的结尾" },
            { field3, "你是我唯一想要的了解" },
            { field4, "李❤❤" },
        }, when: FieldExistFlags.FXX);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HSetNxAsync()
    {
        const string key = "{222}:Hash:HSetNxAsync:Key";
        const string field1 = "field1";

        var ok = await this._redis.Hash.HSetNxAsync(key, field1, field1, CommandFlags.ReturnBytes);
        Assert.That(ok, Is.True);

        ok = await this._redis.Hash.HSetNxAsync(key, field1, "甜甜的");
        Assert.That(ok, Is.False);

        string? field1Value = await this._redis.Hash.HGetAsync(key, field1);
        Assert.That(field1Value, Is.EqualTo(field1));
    }

    [Test]
    public async Task HStrLenAsync()
    {
        const string key = "{223}:Hash:HStrLenAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string value1 = "不管是演动物或植物，你一定都要演的让人家羡慕。";
        const string value2 = "How old arr you!!";

        int set = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1, value1 },
            { field2, value2 },
        });
        Assert.That(set, Is.EqualTo(2));

        int field1Len = await this._redis.Hash.HStrLenAsync(key, field1);
        Assert.That(field1Len, Is.EqualTo(Encoding.UTF8.GetBytes(value1).Length));

        int field2Len = await this._redis.Hash.HStrLenAsync(key, field2, CommandFlags.ReturnBytes);
        Assert.That(field2Len, Is.EqualTo(Encoding.UTF8.GetBytes(value2).Length));

        int noneLen = await this._redis.Hash.HStrLenAsync(key, "none");
        Assert.That(noneLen, Is.Zero);

        noneLen = await this._redis.Hash.HStrLenAsync("none_hash", field2);
        Assert.That(noneLen, Is.Zero);
    }

    [Test]
    public async Task HValsAsync()
    {
        const string key = "{224}:Hash:HValsAsync:Key";
        const string field1 = "field1";
        const string field2 = "field2";
        const string field3 = "field3";
        const string field4 = "field4";

        const string value1 = "💕😍😂👨🏻‍🏫😊👌🐧";

        var vals = await this._redis.Hash.HValsAsync(key);
        Assert.That(vals, Is.Null);

        int set = await this._redis.Hash.HSetAsync(key, new Dictionary<SharpValue, SharpValue>
        {
            { field1,  value1},
            { field2, "How are you!~" },
            { field3, string.Empty },
            { field4, "这是一个申请的测试" },
        });
        Assert.That(set, Is.EqualTo(4));

        vals = await this._redis.Hash.HValsAsync(key);
        Assert.That(vals, Has.Length.EqualTo(4));
        Assert.That(vals[0].ToString(), Is.EqualTo(value1));
    }
}
