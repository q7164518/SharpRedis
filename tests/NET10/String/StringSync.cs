using SharpRedis;
using System.Text;
using static SharpRedis.RedisString;

namespace NET10.String;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class StringSync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public void Set()
    {
        const string key = "{1}:String:Set:Key";
        const string value = "String:Set:Value";
        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        string? getResult = this._redis.String.Get(key);
        Assert.That(getResult, Is.EqualTo(value));

        var getResultBytes = this._redis.String.Get(key, CommandFlags.ReturnBytes);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(getResultBytes.RawValue, Is.InstanceOf<byte[]>());
            Assert.That(getResultBytes.ToString(), Is.EqualTo(value));
        }

        setResult = this._redis.String.Set(key, value, TimeSpan.FromSeconds(10));
        Assert.That(setResult);

        var ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl >= 8 && ttl <= 10);

        setResult = this._redis.String.Set(key, value, DateTime.Now.AddMinutes(10));
        Assert.That(setResult);

        ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl >= 598 && ttl <= 600);
    }

    [Test]
    public void Get()
    {
        const string key = "{2}:String:Get:Key";
        const string value = "String:Get:Value";
        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        string? getResult = this._redis.String.Get(key);
        Assert.That(getResult, Is.EqualTo(value));

        var getResultBytes = this._redis.String.Get(key, CommandFlags.ReturnBytes);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(getResultBytes.RawValue, Is.InstanceOf<byte[]>());
            Assert.That(getResultBytes.ToString(), Is.EqualTo(value));
        }

        getResult = this._redis.String.Get("none");
        Assert.That(getResult, Is.Null);
    }

    [Test]
    public void Append()
    {
        const string key = "{3}:String:Append:Key";
        const string value = "String:Append:Value";
        const string append = ":Appended";
        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        int appendLength = this._redis.String.Append(key, append);
        Assert.That(appendLength, Is.EqualTo(value.Length + append.Length));
    }

    [Test]
    public void Decr()
    {
        const string key = "{4}:String:Decr:Key";
        const string value = "101";

        int number = this._redis.String.Decr(key);
        Assert.That(number, Is.EqualTo(-1));

        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        number = this._redis.String.Decr(key);
        Assert.That(number, Is.EqualTo(100));
    }

    [Test]
    public void DecrBy()
    {
        const string key = "{5}:String:DecrBy:Key";
        const string value = "1024";

        int number = this._redis.String.DecrBy(key, 1);
        Assert.That(number, Is.EqualTo(-1));

        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        number = this._redis.String.DecrBy(key, -1);
        Assert.That(number, Is.EqualTo(1025));

        number = this._redis.String.DecrBy(key, 100);
        Assert.That(number, Is.EqualTo(925));
    }

    [Test]
    public void DelEx()
    {
        const string key = "{6}:String:DelEx:Key";
        const string value = "String:DelEx:Value";

        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        bool delResult = this._redis.String.DelEx(key, value, ValueCondition.IFEQ);
        Assert.That(delResult);

        string? get = this._redis.String.Get(key);
        Assert.That(get, Is.Null);

        setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        delResult = this._redis.String.DelEx(key, "test", ValueCondition.IFEQ);
        Assert.That(!delResult);

        delResult = this._redis.String.DelEx(key, "test", ValueCondition.IFNE);
        Assert.That(delResult);

        get = this._redis.String.Get(key);
        Assert.That(get, Is.Null);

        setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        var hash = this._redis.String.Digest(key);
        Assert.That(!string.IsNullOrEmpty(hash));

        delResult = this._redis.String.DelEx(key, hash, ValueCondition.IFDEQ);
        Assert.That(delResult);

        get = this._redis.String.Get(key);
        Assert.That(get, Is.Null);

        setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        delResult = this._redis.String.DelEx(key, "0123456789123456", ValueCondition.IFDNE);
        Assert.That(delResult);

        get = this._redis.String.Get(key);
        Assert.That(get, Is.Null);
    }

    [Test]
    public void Digest()
    {
        const string key = "{7}:String:Digest:Key";
        const string value = "String:Digest:Value";

        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        var digest = this._redis.String.Digest(key);
        Assert.That(!string.IsNullOrEmpty(digest));
    }

    [Test]
    public void GetDel()
    {
        const string key = "{8}:String:GetDel:Key";
        const string value = "String:GetDel:Value";

        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        string? getDelResult = this._redis.String.GetDel(key);
        Assert.That(getDelResult, Is.EqualTo(value));

        string? getResult = this._redis.String.Get(key);
        Assert.That(getResult, Is.Null);
    }

    [Test]
    public void GetEx()
    {
        const string key = "{9}:String:GetEx:Key";
        const string value = "String:GetEx:Value";

        string? getExResult = this._redis.String.GetEx(key);
        Assert.That(getExResult, Is.Null);

        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        getExResult = this._redis.String.GetEx(key);
        Assert.That(getExResult, Is.EqualTo(value));

        var ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl, Is.EqualTo(-1));

        getExResult = this._redis.String.GetEx(key, TimeSpan.FromSeconds(30));
        Assert.That(getExResult, Is.EqualTo(value));

        ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl >= 0 && ttl <= 30);

        getExResult = this._redis.String.GetEx(key, DateTime.Now.AddHours(1), ExpirationFlags.PXAT);
        Assert.That(getExResult, Is.EqualTo(value));

        ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl, Is.GreaterThanOrEqualTo(3590));
        Assert.That(ttl, Is.LessThanOrEqualTo(3601));

        getExResult = this._redis.String.GetEx(key, expiryFlags: ExpirationFlags.PERSIST);
        Assert.That(getExResult, Is.EqualTo(value));

        ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl, Is.EqualTo(-1));
    }

    [Test]
    public void GetRange()
    {
        const string key = "{10}:String:GetRange:Key";
        const string value = "String:GetRange:Value";

        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        string? range = this._redis.String.GetRange(key, 0, 5);
        Assert.That(range, Is.EqualTo(value[..6]));

        range = this._redis.String.GetRange(key, 0, -2);
        Assert.That(range, Is.EqualTo(value[..(value.Length - 1)]));

        range = this._redis.String.GetRange("none", 0, -2);
        Assert.That(range, Is.Empty);
    }

    [Test]
    public void GetSet()
    {
        const string key = "{11}:String:GetSet:Key";
        const string value = "String:GetSet:Value";

        var result = this._redis.String.GetSet(key, value);
        Assert.That(string.IsNullOrEmpty(result));

        string? str = this._redis.String.GetSet(key, "123");
        Assert.That(str, Is.EqualTo(value));
    }

    [Test]
    public void Incr()
    {
        const string key = "{12}:String:Incr:Key";
        const string value = "1313";

        int number = this._redis.String.Incr(key);
        Assert.That(number, Is.EqualTo(1));

        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        number = this._redis.String.Incr(key, CommandFlags.ReturnBytes);
        Assert.That(number, Is.EqualTo(1314));
    }

    [Test]
    public void IncrBy()
    {
        const string key = "{13}:String:IncrBy:Key";

        long number = this._redis.String.IncrBy(key, 133);
        Assert.That(number, Is.EqualTo(133));

        number = this._redis.String.IncrBy(key, 1181, CommandFlags.ReturnBytes);
        Assert.That(number, Is.EqualTo(1314));

        number = this._redis.String.IncrBy(key, -794);
        Assert.That(number, Is.EqualTo(520));
    }

    [Test]
    public void IncrByFloat()
    {
        const string key = "{14}:String:IncrByFloat:Key";
        double number = this._redis.String.IncrByFloat(key, 133);
        Assert.That(number, Is.EqualTo(133));

        number = this._redis.String.IncrByFloat(key, 1181.1314, CommandFlags.ReturnBytes);
        Assert.That(number, Is.EqualTo(1314.1314));

        number = this._redis.String.IncrByFloat(key, -794);
        Assert.That(number, Is.EqualTo(520.1314));
    }

    [Test]
    public void DecrByFloat()
    {
        const string key = "{15}:String:DecrByFloat:Key";
        double number = this._redis.String.DecrByFloat(key, 133);
        Assert.That(number, Is.EqualTo(-133));

        number = this._redis.String.DecrByFloat(key, 1181.1314, CommandFlags.ReturnBytes);
        Assert.That(number, Is.EqualTo(-1314.1314));

        number = this._redis.String.DecrByFloat(key, -794);
        Assert.That(number, Is.EqualTo(-520.1314));
    }

    [Test]
    public void Lcs()
    {
        const string key1 = "{16}:String:Lcs:Key1";
        const string key2 = "{16}:String:Lcs:Key2";

        const string value1 = "ohmytext";
        const string value2 = "mynewtext";

        var mset = this._redis.String.MSet(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
        });
        Assert.That(mset);

        string? lcs = this._redis.String.Lcs(key1, key2);
        Assert.That(lcs, Is.EqualTo("mytext"));

        int lscLen = this._redis.String.LcsLen(key1, key2);
        Assert.That(lscLen, Is.EqualTo(6));

        var lcsIdx = this._redis.String.LcsIdx(key1, key2, true);
        Assert.That(lcsIdx.Length, Is.EqualTo(6));
        Assert.That(lcsIdx.Matches, Has.Length.EqualTo(2));

        var first = value1.Substring((int)lcsIdx.Matches[0].FirstIndex.Start, (int)lcsIdx.Matches[0].Length!);
        Assert.That(first, Is.EqualTo("my"));
    }

    [Test]
    public void MGet()
    {
        const string key1 = "{17}:String:MGet:Key1";
        const string key2 = "{17}:String:MGet:Key2";
        const string key3 = "{17}:String:MGet:Key3";

        const string value1 = "ohmytext";
        const string value2 = "李心心是猪";
        const string value3 = "mynewtext";

        var mget = this._redis.String.MGet([key1, key2, key3, "none"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(mget, Has.Length.EqualTo(4));
            Assert.That(!mget[0].HasValue);
            Assert.That(!mget[3].HasValue);
        }

        var mset = this._redis.String.MSet(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        });
        Assert.That(mset);

        mget = this._redis.String.MGet([key1, key2, key3, "none"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(mget, Has.Length.EqualTo(4));
            Assert.That(mget[0].ToString(), Is.EqualTo(value1));
            Assert.That(mget[1].ToString(), Is.EqualTo(value2));
            Assert.That(!mget[3].HasValue);
        }
    }

    [Test]
    public void MSetEx()
    {
        const string key1 = "{18}:String:MsetEx:Key1";
        const string key2 = "{18}:String:MsetEx:Key2";
        const string key3 = "{18}:String:MsetEx:Key3";

        const string value1 = "ohmytext";
        const string value2 = "😘李心心❤️";
        const string value3 = "mynewtext";

        var result = this._redis.String.MSetEx(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        });
        Assert.That(result);

        var ttl = this._redis.Key.Ttl(key2);
        Assert.That(ttl, Is.EqualTo(-1));

        result = this._redis.String.MSetEx(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        }, TimeSpan.FromSeconds(5));
        Assert.That(result);
        ttl = this._redis.Key.Ttl(key2);
        Assert.That(ttl >= 3 && ttl <= 5);

        result = this._redis.String.MSetEx(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        }, TimeSpan.FromSeconds(15), ExpirationFlags.KEEPTTL);
        Assert.That(result);

        ttl = this._redis.Key.Ttl(key2);
        Assert.That(ttl >= 3 && ttl <= 5);

        result = this._redis.String.MSetEx(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        }, TimeSpan.FromSeconds(15), when: StringExist.NX);
        Assert.That(!result);

        result = this._redis.String.MSetEx(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        }, 15, when: StringExist.XX);
        Assert.That(result);

        ttl = this._redis.Key.Ttl(key2);
        Assert.That(ttl >= 13 && ttl <= 15);

        var get = this._redis.String.Get(key2);
        Assert.That(get.ToString(), Is.EqualTo(value2));
    }

    [Test]
    public void MSetNx()
    {
        const string key1 = "{19}:String:MSetNx:Key1";
        const string key2 = "{19}:String:MSetNx:Key2";
        const string key3 = "{19}:String:MSetNx:Key3";

        const string value1 = "ohmytext";
        const string value2 = "情字何解";
        const string value3 = "怎落笔都不对";

        var result = this._redis.String.MSetNx(new Dictionary<SharpValue, SharpValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key3, value3 },
        });
        Assert.That(result);

        var get = this._redis.String.Get(key2);
        Assert.That(get.ToString(), Is.EqualTo(value2));

        result = this._redis.String.MSetNx(new Dictionary<SharpValue, SharpValue>
        {
            { key1 + "1", value1 },
            { key3, value3 },
        });
        Assert.That(!result);
    }

    [Test]
    public void PSetEx()
    {
        const string key = "{20}:String:PSetEx:Key";
        const string value = "最美的不是下雨天";

        var ok = this._redis.String.PSetEx(key, value, 2010);
        Assert.That(ok);

        var pttl = this._redis.Key.PTtl(key);
        Assert.That(pttl >= 2000 && pttl <= 2010);

        ok = this._redis.String.PSetEx(key, value, TimeSpan.FromSeconds(5));
        Assert.That(ok);

        pttl = this._redis.Key.PTtl(key);
        Assert.That(pttl >= 4990 && pttl <= 5000);
    }

    [Test]
    public void SetEx()
    {
        const string key = "{21}:String:SetEx:Key";
        const string value = "是曾与你躲过雨的屋檐";

        var ok = this._redis.String.SetEx(key, value, 2);
        Assert.That(ok);

        var pttl = this._redis.Key.PTtl(key);
        Assert.That(pttl >= 1990 && pttl <= 2000);

        ok = this._redis.String.SetEx(key, value, TimeSpan.FromSeconds(5));
        Assert.That(ok);

        pttl = this._redis.Key.PTtl(key);
        Assert.That(pttl >= 4990 && pttl <= 5000);
    }

    [Test]
    public void SetNx()
    {
        const string key = "{22}:String:SetNx:Key";
        const string value = "天青色等烟雨, 而我在等你";

        var get = this._redis.String.Get(key);
        Assert.That(get.IsNull);

        var ok = this._redis.String.SetNx(key, value);
        Assert.That(ok);

        get = this._redis.String.Get(key);
        Assert.That(get.ToString(), Is.EqualTo(value));

        ok = this._redis.String.SetNx(key, "天涯过客");
        Assert.That(!ok);

        get = this._redis.String.Get(key);
        Assert.That(get.ToString(), Is.EqualTo(value));
    }

    [Test]
    public void SetRange()
    {
        const string key = "{23}:String:SetRange:Key";
        const string value = "I love forever";

        var ok = this._redis.String.SetNx(key, value);
        Assert.That(ok);

        int length = this._redis.String.SetRange(key, 7, "李心心");
        Assert.That(length, Is.EqualTo(16));

        var get = this._redis.String.Get(key);
        Assert.That(get.ToString(), Is.EqualTo("I love 李心心"));
    }

    [Test]
    public void StrLen()
    {
        const string key = "{24}:String:StrLen:Key";
        const string value = "I love LiXinxin forever";
        const string zh_value = "今天是周末";

        var ok = this._redis.String.Set(key, value);
        Assert.That(ok);

        int length = this._redis.String.StrLen(key);
        Assert.That(length, Is.EqualTo(value.Length));

        ok = this._redis.String.Set(key, zh_value);
        Assert.That(ok);

        length = this._redis.String.StrLen(key);
        Assert.That(length, Is.EqualTo(Encoding.UTF8.GetBytes(zh_value).Length));
    }

    [Test]
    public void SubStr()
    {
        const string key = "{25}:String:SubStr:Key";
        const string value = "String:SubStr:Value";

        bool setResult = this._redis.String.Set(key, value);
        Assert.That(setResult);

        string? range = this._redis.String.SubStr(key, 0, 5);
        Assert.That(range, Is.EqualTo(value[..6]));

        range = this._redis.String.SubStr(key, 0, -2);
        Assert.That(range, Is.EqualTo(value[..(value.Length - 1)]));

        range = this._redis.String.SubStr("none", 0, -2);
        Assert.That(range, Is.Empty);
    }

    [Test]
    public void IncrEx()
    {
        const string key = "{IncrEx}:String:Key";

        var result = this._redis.String.IncrEx(key, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(10));
            Assert.That((int)result.Increment, Is.EqualTo(10));
        }

        result = this._redis.String.IncrEx(key, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(20));
            Assert.That((int)result.Increment, Is.EqualTo(10));
        }

        result = this._redis.String.IncrEx(key, 10, 10, 25);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(20));
            Assert.That((int)result.Increment, Is.Zero);
        }

        result = this._redis.String.IncrEx(key, -100, 10, 25);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(20));
            Assert.That((int)result.Increment, Is.Zero);
        }

        result = this._redis.String.IncrEx(key, 10, 10, 25, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(25));
            Assert.That((int)result.Increment, Is.EqualTo(5));
        }

        result = this._redis.String.IncrEx(key, -100, 10, 25, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((int)result.Value, Is.EqualTo(10));
            Assert.That((int)result.Increment, Is.EqualTo(-15));
        }

        result = this._redis.String.IncrEx(key, 3.141592653, 10, 25, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((double)result.Value, Is.EqualTo(13.141592653));
            Assert.That((double)result.Increment, Is.EqualTo(3.141592653));
        }

        var ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl, Is.EqualTo(-1));

        result = this._redis.String.IncrEx(key, -3.141592653, 10, 25, true, TimeSpan.FromSeconds(60), ExpirationFlags.EX, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That((double)result.Value, Is.EqualTo(10));
            Assert.That((double)result.Increment, Is.EqualTo(-3.141592653));
        }

        ttl = this._redis.Key.Ttl(key);
        Assert.That(ttl, Is.GreaterThanOrEqualTo(58));
    }
}
