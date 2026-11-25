using SharpRedis;
using System.Text;
using static SharpRedis.RedisString;

namespace NET10.String;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class StringAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public async Task SetAsync()
    {
        const string key = "{26}:String:SetAsync:Key";
        const string value = "String:SetAsync:Value";
        bool setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        string? getResult = await this._redis.String.GetAsync(key);
        Assert.That(getResult, Is.EqualTo(value));

        var getResultBytes = await this._redis.String.GetAsync(key, CommandFlags.ReturnBytes);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(getResultBytes.RawValue, Is.InstanceOf<byte[]>());
            Assert.That(getResultBytes.ToString(), Is.EqualTo(value));
        }

        setResult = await this._redis.String.SetAsync(key, value, TimeSpan.FromSeconds(10));
        Assert.That(setResult);

        var ttl = await this._redis.Key.TtlAsync(key);
        Assert.That(ttl >= 8 && ttl <= 10);

        setResult = await this._redis.String.SetAsync(key, value, DateTime.Now.AddMinutes(10));
        Assert.That(setResult);

        ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl >= 598 && ttl <= 600);
    }

    [Test]
    public async Task GetAsync()
    {
        const string key = "{27}:String:GetAsync:Key";
        const string value = "String:GetAsync:Value";
        bool setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        string? getResult = await this._redis.String.GetAsync(key);
        Assert.That(getResult, Is.EqualTo(value));

        var getResultBytes = await this._redis.String.GetAsync(key, CommandFlags.ReturnBytes);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(getResultBytes.RawValue, Is.InstanceOf<byte[]>());
            Assert.That(getResultBytes.ToString(), Is.EqualTo(value));
        }

        getResult = await this._redis.String.GetAsync("none");
        Assert.That(getResult, Is.Null);
    }

    [Test]
    public async Task AppendAsync()
    {
        const string key = "{28}:String:AppendAsync:Key";
        const string value = "String:AppendAsync:Value";
        const string append = ":Appended";
        bool setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        int appendLength = await this._redis.String.AppendAsync(key, append);
        Assert.That(appendLength, Is.EqualTo(value.Length + append.Length));
    }

    [Test]
    public async Task DecrAsync()
    {
        const string key = "{29}:String:DecrAsync:Key";
        const string value = "101";

        int number = await this._redis.String.DecrAsync(key);
        Assert.That(number, Is.EqualTo(-1));

        bool setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        number = await this._redis.String.DecrAsync(key);
        Assert.That(number, Is.EqualTo(100));
    }

    [Test]
    public async Task DecrByAsync()
    {
        const string key = "{30}:String:DecrByAsync:Key";
        const string value = "1024";

        int number = await this._redis.String.DecrByAsync(key, 1);
        Assert.That(number, Is.EqualTo(-1));

        bool setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        number = await this._redis.String.DecrByAsync(key, -1);
        Assert.That(number, Is.EqualTo(1025));

        number = await this._redis.String.DecrByAsync(key, 100);
        Assert.That(number, Is.EqualTo(925));
    }

    [Test]
    public async Task DelExAsync()
    {
        const string key = "{31}:String:DelExAsync:Key";
        const string value = "String:DelExAsync:Value";

        bool setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        bool delResult = await this._redis.String.DelExAsync(key, value, ValueCondition.IFEQ);
        Assert.That(delResult);

        string? get = await this._redis.String.GetAsync(key);
        Assert.That(get, Is.Null);

        setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        delResult = await this._redis.String.DelExAsync(key, "test", ValueCondition.IFEQ);
        Assert.That(!delResult);

        delResult = await this._redis.String.DelExAsync(key, "test", ValueCondition.IFNE);
        Assert.That(delResult);

        get = await this._redis.String.GetAsync(key);
        Assert.That(get, Is.Null);

        setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        var hash = await this._redis.String.DigestAsync(key);
        Assert.That(!string.IsNullOrEmpty(hash));

        delResult = await this._redis.String.DelExAsync(key, hash, ValueCondition.IFDEQ);
        Assert.That(delResult);

        get = await this._redis.String.GetAsync(key);
        Assert.That(get, Is.Null);

        setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        delResult = await this._redis.String.DelExAsync(key, "0123456789123456", ValueCondition.IFDNE);
        Assert.That(delResult);

        get = await this._redis.String.GetAsync(key);
        Assert.That(get, Is.Null);
    }

    [Test]
    public async Task DigestAsync()
    {
        const string key = "{32}:String:DigestAsync:Key";
        const string value = "String:DigestAsync:Value";

        bool setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        var digest = await this._redis.String.DigestAsync(key);
        Assert.That(!string.IsNullOrEmpty(digest));
    }

    [Test]
    public async Task GetDelAsync()
    {
        const string key = "{33}:String:GetDelAsync:Key";
        const string value = "String:GetDelAsync:Value";

        bool setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        string? getDelResult = await this._redis.String.GetDelAsync(key);
        Assert.That(getDelResult, Is.EqualTo(value));

        string? getResult = await this._redis.String.GetAsync(key);
        Assert.That(getResult, Is.Null);
    }

    [Test]
    public async Task GetExAsync()
    {
        const string key = "{34}:String:GetExAsync:Key";
        const string value = "String:GetExAsync:Value";

        string? getExResult = await this._redis.String.GetExAsync(key);
        Assert.That(getExResult, Is.Null);

        bool setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        getExResult = await this._redis.String.GetExAsync(key);
        Assert.That(getExResult, Is.EqualTo(value));

        var ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl, Is.EqualTo(-1));

        getExResult = await this._redis.String.GetExAsync(key, TimeSpan.FromSeconds(30));
        Assert.That(getExResult, Is.EqualTo(value));

        ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl >= 0 && ttl <= 30);

        getExResult = await this._redis.String.GetExAsync(key, DateTime.Now.AddHours(1), ExpirationFlags.PXAT);
        Assert.That(getExResult, Is.EqualTo(value));

        ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl, Is.GreaterThanOrEqualTo(3590));
        Assert.That(ttl, Is.LessThanOrEqualTo(3601));

        getExResult = await this._redis.String.GetExAsync(key, null, ExpirationFlags.PERSIST);
        Assert.That(getExResult, Is.EqualTo(value));

        ttl = await this._redis.Key.TtlAsync(key);
        Assert.That(ttl, Is.EqualTo(-1));
    }

    [Test]
    public async Task GetRangeAsync()
    {
        const string key = "{35}:String:GetRangeAsync:Key";
        const string value = "String:GetRangeAsync:Value";

        bool setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        string? range = await this._redis.String.GetRangeAsync(key, 0, 5);
        Assert.That(range, Is.EqualTo(value[..6]));

        range = this._redis.String.GetRange(key, 0, -2);
        Assert.That(range, Is.EqualTo(value[..(value.Length - 1)]));

        range = await this._redis.String.GetRangeAsync("none", 0, -2);
        Assert.That(range, Is.Empty);
    }

    [Test]
    public async Task GetSetAsync()
    {
        const string key = "{36}:String:GetSetAsync:Key";
        const string value = "String:GetSetAsync:Value";

        var result = await this._redis.String.GetSetAsync(key, value);
        Assert.That(string.IsNullOrEmpty(result));

        string? str = await this._redis.String.GetSetAsync(key, "123");
        Assert.That(str, Is.EqualTo(value));
    }

    [Test]
    public async Task IncrAsync()
    {
        const string key = "{37}:String:IncrAsync:Key";
        const string value = "1313";

        int number = await this._redis.String.IncrAsync(key);
        Assert.That(number, Is.EqualTo(1));

        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        number = await this._redis.String.IncrAsync(key, CommandFlags.ReturnBytes);
        Assert.That(number, Is.EqualTo(1314));
    }

    [Test]
    public async Task IncrByAsync()
    {
        const string key = "{38}:String:IncrByAsync:Key";

        long number = await this._redis.String.IncrByAsync(key, 133);
        Assert.That(number, Is.EqualTo(133));

        number = await this._redis.String.IncrByAsync(key, 1181);
        Assert.That(number, Is.EqualTo(1314));

        number = await this._redis.String.IncrByAsync(key, -794, CommandFlags.ReturnBytes);
        Assert.That(number, Is.EqualTo(520));
    }

    [Test]
    public async Task IncrByFloatAsync()
    {
        const string key = "{39}:String:IncrByFloatAsync:Key";
        double number = await this._redis.String.IncrByFloatAsync(key, 133);
        Assert.That(number, Is.EqualTo(133));

        number = await this._redis.String.IncrByFloatAsync(key, 1181.1314, CommandFlags.ReturnBytes);
        Assert.That(number, Is.EqualTo(1314.1314));

        number = await this._redis.String.IncrByFloatAsync(key, -794);
        Assert.That(number, Is.EqualTo(520.1314));
    }

    [Test]
    public async Task DecrByFloatAsync()
    {
        const string key = "{40}:String:DecrByFloatAsync:Key";
        double number = await this._redis.String.DecrByFloatAsync(key, 133);
        Assert.That(number, Is.EqualTo(-133));

        number = await this._redis.String.DecrByFloatAsync(key, 1181.1314, CommandFlags.ReturnBytes);
        Assert.That(number, Is.EqualTo(-1314.1314));

        number = await this._redis.String.DecrByFloatAsync(key, -794);
        Assert.That(number, Is.EqualTo(-520.1314));
    }

    [Test]
    public async Task LcsAsync()
    {
        const string key1 = "{41}:String:LcsAsync:Key1";
        const string key2 = "{41}:String:LcsAsync:Key2";

        const string value1 = "ohmytext";
        const string value2 = "mynewtext";

        var mset = await this._redis.String.MSetAsync(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
        });
        Assert.That(mset);

        string? lcs = await this._redis.String.LcsAsync(key1, key2);
        Assert.That(lcs, Is.EqualTo("mytext"));

        int lscLen = await this._redis.String.LcsLenAsync(key1, key2);
        Assert.That(lscLen, Is.EqualTo(6));

        var lcsIdx = await this._redis.String.LcsIdxAsync(key1, key2, true);
        Assert.That(lcsIdx.Length, Is.EqualTo(6));
        Assert.That(lcsIdx.Matches, Has.Length.EqualTo(2));

        var first = value1.Substring((int)lcsIdx.Matches[0].FirstIndex.Start, (int)lcsIdx.Matches[0].Length!);
        Assert.That(first, Is.EqualTo("my"));
    }

    [Test]
    public async Task MGetAsync()
    {
        const string key1 = "{42}:String:MGetAsync:Key1";
        const string key2 = "{42}:String:MGetAsync:Key2";
        const string key3 = "{42}:String:MGetAsync:Key3";

        const string value1 = "ohmytext";
        const string value2 = "李心心是猪";
        const string value3 = "mynewtext";

        var mget = await this._redis.String.MGetAsync([key1, key2, key3, "none"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(mget, Has.Length.EqualTo(4));
            Assert.That(!mget[0].HasValue);
            Assert.That(!mget[3].HasValue);
        }

        var mset = await this._redis.String.MSetAsync(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        });
        Assert.That(mset);

        mget = await this._redis.String.MGetAsync([key1, key2, key3, "none"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(mget, Has.Length.EqualTo(4));
            Assert.That(mget[0].ToString(), Is.EqualTo(value1));
            Assert.That(mget[1].ToString(), Is.EqualTo(value2));
            Assert.That(!mget[3].HasValue);
        }
    }

    [Test]
    public async Task MSetExAsync()
    {
        const string key1 = "{43}:String:MSetExAsync:Key1";
        const string key2 = "{43}:String:MSetExAsync:Key2";
        const string key3 = "{43}:String:MSetExAsync:Key3";

        const string value1 = "ohmytext";
        const string value2 = "😘李心心❤️";
        const string value3 = "mynewtext";

        var result = await this._redis.String.MSetExAsync(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        });
        Assert.That(result);

        var ttl = this._redis.Key.Ttl(key2);
        Assert.That(ttl, Is.EqualTo(-1));

        result = await this._redis.String.MSetExAsync(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        }, TimeSpan.FromSeconds(5));
        Assert.That(result);
        ttl = this._redis.Key.Ttl(key2);
        Assert.That(ttl >= 3 && ttl <= 5);

        result = await this._redis.String.MSetExAsync(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        }, TimeSpan.FromSeconds(15), ExpirationFlags.KEEPTTL);
        Assert.That(result);

        ttl = await this._redis.Key.TtlAsync(key2);
        Assert.That(ttl >= 3 && ttl <= 5);

        result = await this._redis.String.MSetExAsync(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        }, TimeSpan.FromSeconds(15), when: StringExist.NX);
        Assert.That(!result);

        result = await this._redis.String.MSetExAsync(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        }, 15, when: StringExist.XX);
        Assert.That(result);

        ttl = await this._redis.Key.TtlAsync(key2);
        Assert.That(ttl >= 13 && ttl <= 15);

        var get = this._redis.String.Get(key2);
        Assert.That(get.ToString(), Is.EqualTo(value2));
    }

    [Test]
    public async Task MSetNxAsync()
    {
        const string key1 = "{44}:String:MSetNxAsync:Key1";
        const string key2 = "{44}:String:MSetNxAsync:Key2";
        const string key3 = "{44}:String:MSetNxAsync:Key3";

        const string value1 = "而我独缺你一生的了解";
        const string value2 = "情字何解";
        const string value3 = "怎落笔都不对";

        var result = await this._redis.String.MSetNxAsync(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        });
        Assert.That(result);

        var get = await this._redis.String.GetAsync(key1);
        Assert.That(get.ToString(), Is.EqualTo(value1));

        result = await this._redis.String.MSetNxAsync(new Dictionary<SharpValue, SharpValue>
        {
            { key1 + "1", value1 },
            { key3, value3 },
        });
        Assert.That(!result);
    }

    [Test]
    public async Task PSetExAsync()
    {
        const string key = "{45}:String:PSetExAsync:Key";
        const string value = "最美的不是下雨天";

        var ok = await this._redis.String.PSetExAsync(key, value, 2010);
        Assert.That(ok);

        var pttl = this._redis.Key.PTtl(key);
        Assert.That(pttl >= 2000 && pttl <= 2010);

        ok = await this._redis.String.PSetExAsync(key, value, TimeSpan.FromSeconds(5));
        Assert.That(ok);

        pttl = await this._redis.Key.PTtlAsync(key);
        Assert.That(pttl >= 4990 && pttl <= 5000);
    }

    [Test]
    public async Task SetExAsync()
    {
        const string key = "{46}:String:SetExAsync:Key";
        const string value = "是曾与你躲过雨的屋檐";

        var ok = await this._redis.String.SetExAsync(key, value, 2);
        Assert.That(ok);

        var pttl = await this._redis.Key.PTtlAsync(key);
        Assert.That(pttl >= 1990 && pttl <= 2000);

        ok = await this._redis.String.SetExAsync(key, value, TimeSpan.FromSeconds(5));
        Assert.That(ok);

        pttl = await this._redis.Key.PTtlAsync(key);
        Assert.That(pttl >= 4990 && pttl <= 5000);
    }

    [Test]
    public async Task SetNxAsync()
    {
        const string key = "{47}:String:SetNxAsync:Key";
        const string value = "天青色等烟雨, 而我在等你";

        var get = await this._redis.String.GetAsync(key);
        Assert.That(get.IsNull);

        var ok = await this._redis.String.SetNxAsync(key, value);
        Assert.That(ok);

        get = await this._redis.String.GetAsync(key);
        Assert.That(get.ToString(), Is.EqualTo(value));

        ok = await this._redis.String.SetNxAsync(key, "天涯过客");
        Assert.That(!ok);

        get = await this._redis.String.GetAsync(key);
        Assert.That(get.ToString(), Is.EqualTo(value));
    }

    [Test]
    public async Task SetRangeAsync()
    {
        const string key = "{48}:String:SetRangeAsync:Key";
        const string value = "I love forever";

        var ok = await this._redis.String.SetNxAsync(key, value);
        Assert.That(ok);

        int length = await this._redis.String.SetRangeAsync(key, 7, "李心心");
        Assert.That(length, Is.EqualTo(16));

        var get = this._redis.String.Get(key);
        Assert.That(get.ToString(), Is.EqualTo("I love 李心心"));
    }

    [Test]
    public async Task StrLenAsync()
    {
        const string key = "{49}:String:StrLenAsync:Key";
        const string value = "I love LiXinxin forever";
        const string zh_value = "今天是周末";

        var ok = await this._redis.String.SetAsync(key, value);
        Assert.That(ok);

        int length = await this._redis.String.StrLenAsync(key);
        Assert.That(length, Is.EqualTo(value.Length));

        ok = await this._redis.String.SetAsync(key, zh_value);
        Assert.That(ok);

        length = await this._redis.String.StrLenAsync(key);
        Assert.That(length, Is.EqualTo(Encoding.UTF8.GetBytes(zh_value).Length));
    }

    [Test]
    public async Task SubStrAsync()
    {
        const string key = "{50}:String:SubStrAsync:Key";
        const string value = "String:SubStrAsync:Value";

        bool setResult = await this._redis.String.SetAsync(key, value);
        Assert.That(setResult);

        string? range = await this._redis.String.SubStrAsync(key, 0, 5);
        Assert.That(range, Is.EqualTo(value[..6]));

        range = await this._redis.String.SubStrAsync(key, 0, -2);
        Assert.That(range, Is.EqualTo(value[..(value.Length - 1)]));

        range = await this._redis.String.SubStrAsync("none", 0, -2);
        Assert.That(range, Is.Empty);
    }

    [Test]
    public async Task IncrExAsync()
    {
        const string key = "{IncrExAsync}:String:Key";

        var result = await this._redis.String.IncrExAsync(key, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(10));
            Assert.That((int)result.Increment, Is.EqualTo(10));
        }

        result = await this._redis.String.IncrExAsync(key, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(20));
            Assert.That((int)result.Increment, Is.EqualTo(10));
        }

        result = await this._redis.String.IncrExAsync(key, 10, 10, 25);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(20));
            Assert.That((int)result.Increment, Is.Zero);
        }

        result = await this._redis.String.IncrExAsync(key, -100, 10, 25);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(20));
            Assert.That((int)result.Increment, Is.Zero);
        }

        result = await this._redis.String.IncrExAsync(key, 10, 10, 25, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(25));
            Assert.That((int)result.Increment, Is.EqualTo(5));
        }

        result = await this._redis.String.IncrExAsync(key, -100, 10, 25, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(10));
            Assert.That((int)result.Increment, Is.EqualTo(-15));
        }

        result = await this._redis.String.IncrExAsync(key, 3.141592653, 10, 25, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((double)result.Value, Is.EqualTo(13.141592653));
            Assert.That((double)result.Increment, Is.EqualTo(3.141592653));
        }

        var ttl = await this._redis.Key.TtlAsync(key);
        Assert.That(ttl, Is.EqualTo(-1));

        result = await this._redis.String.IncrExAsync(key, -3.141592653, 10, 25, true, TimeSpan.FromSeconds(60), ExpirationFlags.EX, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((double)result.Value, Is.EqualTo(10));
            Assert.That((double)result.Increment, Is.EqualTo(-3.141592653));
        }

        ttl = await this._redis.Key.TtlAsync(key);
        Assert.That(ttl, Is.GreaterThanOrEqualTo(58));
    }
}
