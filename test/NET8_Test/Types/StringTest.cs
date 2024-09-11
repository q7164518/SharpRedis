using SharpRedis;
using System.Text;

namespace NET8_Test.Types;

public class StringTest
{
    [Theory, ClassData(typeof(RedisProvider))]
    public void SetAndGet(Redis redis)
    {
        var setOk = redis.String.Set("set_str", "hello redis!");
        Assert.True(setOk);
        var get = redis.String.Get("set_str");
        Assert.Equal("hello redis!", get);
        var notKeyGet = redis.String.Get("notkey");
        Assert.Null(notKeyGet);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SetAndGetAsync(Redis redis)
    {
        var setOk = await redis.String.SetAsync("set_str_async", "hello redis!");
        Assert.True(setOk);
        var get = await redis.String.GetAsync("set_str_async");
        Assert.Equal("hello redis!", get);
        var notKeyGet = await redis.String.GetAsync("notkey_async");
        Assert.Null(notKeyGet);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SetAndGetBytes(Redis redis)
    {
        var bytes = Encoding.UTF8.GetBytes("hello redis bytes!");
        var setOk = redis.String.Set("set_str_bytes", bytes);
        Assert.True(setOk);
        var get = redis.String.GetBytes("set_str_bytes");
        Assert.Equal(bytes, get);
        var notKeyGet = redis.String.GetBytes("notkey");
        Assert.Null(notKeyGet);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    async public Task SetAndGetBytesAsync(Redis redis)
    {
        var bytes = Encoding.UTF8.GetBytes("hello redis bytes!");
        var setOk = await redis.String.SetAsync("set_str_bytes_async", bytes);
        Assert.True(setOk);
        var get = await redis.String.GetBytesAsync("set_str_bytes_async");
        Assert.Equal(bytes, get);
        var notKeyGet = await redis.String.GetBytesAsync("notkey");
        Assert.Null(notKeyGet);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void Append(Redis redis)
    {
        _ = redis.Key.Del(["append", "not_append_key"]);
        _ = redis.String.Set("append", "append");
        var length = redis.String.Append("append", "-append");
        Assert.Equal(13, length);
        var notKeyLength = redis.String.Append("not_append_key", "append");
        Assert.Equal(6, notKeyLength);
        var appendBytesLength = redis.String.Append("append", Encoding.UTF8.GetBytes("abc"));
        Assert.Equal(16, appendBytesLength);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task AppendAsync(Redis redis)
    {
        _ = redis.Key.Del(["append_async", "not_append_key_async"]);
        _ = await redis.String.SetAsync("append_async", "append");
        var length = await redis.String.AppendAsync("append_async", "-append");
        Assert.Equal(13, length);
        var notKeyLength = await redis.String.AppendAsync("not_append_key_async", "append");
        Assert.Equal(6, notKeyLength);
        var appendBytesLength = await redis.String.AppendAsync("append_async", Encoding.UTF8.GetBytes("abc"));
        Assert.Equal(16, appendBytesLength);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void Decr(Redis redis)
    {
        _ = redis.Key.Del("decr");
        var num = redis.String.Decr("decr");
        Assert.Equal(-1, num);
        num = redis.String.Decr("decr");
        Assert.Equal(-2, num);
        num = redis.String.DecrBy("decr", 1);
        Assert.Equal(-3, num);
        num = redis.String.DecrBy("decr", 10);
        Assert.Equal(-13, num);
        num = redis.String.DecrBy("decr", -9);
        Assert.Equal(-4, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    async public Task DecrAsync(Redis redis)
    {
        _ = await redis.Key.DelAsync("decr_async");
        var num = await redis.String.DecrAsync("decr_async");
        Assert.Equal(-1, num);
        num = await redis.String.DecrAsync("decr_async");
        Assert.Equal(-2, num);
        num = await redis.String.DecrByAsync("decr_async", 1);
        Assert.Equal(-3, num);
        num = await redis.String.DecrByAsync("decr_async", 10);
        Assert.Equal(-13, num);
        num = await redis.String.DecrByAsync("decr_async", -9);
        Assert.Equal(-4, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GetDel(Redis redis)
    {
        var setOk = redis.String.Set("get_del", "hello redis!");
        Assert.True(setOk);
        var getDelData = redis.String.GetDel("get_del");
        Assert.Equal("hello redis!", getDelData);
        getDelData = redis.String.GetDel("get_del");
        Assert.Null(getDelData);
        getDelData = redis.String.Get("get_del");
        Assert.Null(getDelData);
        var exists = redis.Key.Exists("get_del");
        Assert.False(exists);

        setOk = redis.String.Set("get_del", "hello redis bytes!");
        Assert.True(setOk);
        var bytes = redis.String.GetDelBytes("get_del");
        Assert.Equal(Encoding.UTF8.GetBytes("hello redis bytes!"), bytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GetDelAsync(Redis redis)
    {
        var setOk = await redis.String.SetAsync("get_del_async", "hello redis!");
        Assert.True(setOk);
        var getDelData = await redis.String.GetDelAsync("get_del_async");
        Assert.Equal("hello redis!", getDelData);
        getDelData = await redis.String.GetDelAsync("get_del_async");
        Assert.Null(getDelData);
        getDelData = await redis.String.GetAsync("get_del_async");
        Assert.Null(getDelData);
        var exists = await redis.Key.ExistsAsync("get_del_async");
        Assert.False(exists);

        setOk = await redis.String.SetAsync("get_del_async", "hello redis bytes!");
        Assert.True(setOk);
        var bytes = await redis.String.GetDelBytesAsync("get_del_async");
        Assert.Equal(Encoding.UTF8.GetBytes("hello redis bytes!"), bytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GetEx(Redis redis)
    {
        const string key = "getex_key";
        const string notKey = "getex_key_not";
        const string value = "getex_value!";
        var setOk = redis.String.Set(key, value);
        Assert.True(setOk);
        var getex = redis.String.GetEx(key, 60);
        Assert.Equal(value, getex);

        var ttl = redis.Key.Ttl(key);
        Assert.Equal(60, ttl);
        getex = redis.String.GetEx(key, true);
        Assert.Equal(value, getex);
        ttl = redis.Key.Ttl(key);
        Assert.Equal(-1, ttl);

        var dateTime = DateTime.Now.AddHours(2);
        var unixTimeSeconds = new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        getex = redis.String.GetEx(key, dateTime);
        Assert.Equal(value, getex);
        var expireTimeSeconds = redis.Key.ExpireTime(key);
        Assert.True((expireTimeSeconds - unixTimeSeconds) <= 1);


        getex = redis.String.GetExPx(key, 1000 * 40);
        Assert.Equal(value, getex);
        ttl = redis.Key.PTtl(key);
        Assert.True((1000 * 40 - ttl) <= 30);

        getex = redis.String.GetEx(key, TimeSpan.FromSeconds(60));
        Assert.Equal(value, getex);
        ttl = redis.Key.PTtl(key);
        Assert.True((1000 * 60 - ttl) <= 30);

        var bytes = redis.String.GetExBytes(key, true);
        Assert.Equal(Encoding.UTF8.GetBytes(value), bytes);
        ttl = redis.Key.Ttl(key);
        Assert.Equal(-1, ttl);

        bytes = redis.String.GetExBytes(key, TimeSpan.FromSeconds(60));
        Assert.Equal(Encoding.UTF8.GetBytes(value), bytes);
        ttl = redis.Key.PTtl(key);
        Assert.True((1000 * 60 - ttl) <= 50);

        var notBytes = redis.String.GetExBytes(notKey, 60);
        Assert.Null(notBytes);

        var not = redis.String.GetEx(notKey, 60);
        Assert.Null(not);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GetExAsync(Redis redis)
    {
        const string key = "getex_key_async";
        const string notKey = "getex_key_not_async";
        const string value = "getex_value!_async";
        var setOk = await redis.String.SetAsync(key, value);
        Assert.True(setOk);
        var getex = await redis.String.GetExAsync(key, 60);
        Assert.Equal(value, getex);

        var ttl = await redis.Key.TtlAsync(key);
        Assert.Equal(60, ttl);
        getex = await redis.String.GetExAsync(key, true);
        Assert.Equal(value, getex);
        ttl = await redis.Key.TtlAsync(key);
        Assert.Equal(-1, ttl);

        var dateTime = DateTime.Now.AddHours(2);
        var unixTimeSeconds = new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        getex = await redis.String.GetExAsync(key, dateTime);
        Assert.Equal(value, getex);
        var expireTimeSeconds = await redis.Key.ExpireTimeAsync(key);
        Assert.True((expireTimeSeconds - unixTimeSeconds) <= 1);


        getex = await redis.String.GetExPxAsync(key, 1000 * 40);
        Assert.Equal(value, getex);
        ttl = await redis.Key.PTtlAsync(key);
        Assert.True((1000 * 40 - ttl) <= 30);

        getex = await redis.String.GetExAsync(key, TimeSpan.FromSeconds(60));
        Assert.Equal(value, getex);
        ttl = await redis.Key.PTtlAsync(key);
        Assert.True((1000 * 60 - ttl) <= 10);

        var bytes = await redis.String.GetExBytesAsync(key, true);
        Assert.Equal(Encoding.UTF8.GetBytes(value), bytes);
        ttl = redis.Key.Ttl(key);
        Assert.Equal(-1, ttl);

        bytes = await redis.String.GetExBytesAsync(key, TimeSpan.FromSeconds(60));
        Assert.Equal(Encoding.UTF8.GetBytes(value), bytes);
        ttl = await redis.Key.PTtlAsync(key);
        Assert.True((1000 * 60 - ttl) <= 10);

        var notBytes = await redis.String.GetExBytesAsync(notKey, 60);
        Assert.Null(notBytes);

        var not = await redis.String.GetExAsync(notKey, 60);
        Assert.Null(not);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GetRange(Redis redis)
    {
        const string key = "get_range_test";
        const string notKey = "get_range_test_not";
        const string value = "abcdefghijklmnopqrstuvwxyz";
        var setOk = redis.String.Set(key, value);
        Assert.True(setOk);
        var length = redis.String.StrLen(key);
        Assert.Equal(Encoding.UTF8.GetBytes(value).Length, length);

        var range = redis.String.GetRange(key, 0, 4);
        Assert.Equal("abcde", range);

        range = redis.String.GetRange(key, 3, 4);
        Assert.Equal("de", range);

        range = redis.String.GetRange(key, 0, -1);
        Assert.Equal(value, range);

        range = redis.String.GetRange(key, 0, -2);
        Assert.Equal("abcdefghijklmnopqrstuvwxy", range);

        range = redis.String.GetRange(key, -5, -2);
        Assert.Equal("vwxy", range);

        var rangeBytes = redis.String.GetRangeBytes(key, 0, 4);
        Assert.Equal(Encoding.UTF8.GetBytes("abcde"), rangeBytes);

        var notRange = redis.String.GetRange(notKey, 0, -1);
        Assert.True(string.IsNullOrEmpty(notRange));

        var notRangeBytes = redis.String.GetRangeBytes(notKey, 0, -1);
        Assert.True(notRangeBytes is null or { Length: 0 });
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GetRangeAsync(Redis redis)
    {
        const string key = "get_range_test_async";
        const string notKey = "get_range_test_not_async";
        const string value = "abcdefghijklmnopqrstuvwxyz";
        var setOk = await redis.String.SetAsync(key, value);
        Assert.True(setOk);
        var length = await redis.String.StrLenAsync(key);
        Assert.Equal(Encoding.UTF8.GetBytes(value).Length, length);

        var range = await redis.String.GetRangeAsync(key, 0, 4);
        Assert.Equal("abcde", range);

        range = await redis.String.GetRangeAsync(key, 3, 4);
        Assert.Equal("de", range);

        range = await redis.String.GetRangeAsync(key, 0, -1);
        Assert.Equal(value, range);

        range = await redis.String.GetRangeAsync(key, 0, -2);
        Assert.Equal("abcdefghijklmnopqrstuvwxy", range);

        range = await redis.String.GetRangeAsync(key, -5, -2);
        Assert.Equal("vwxy", range);

        var rangeBytes = await redis.String.GetRangeBytesAsync(key, 0, 4);
        Assert.Equal(Encoding.UTF8.GetBytes("abcde"), rangeBytes);

        var notRange = await redis.String.GetRangeAsync(notKey, 0, -1);
        Assert.True(string.IsNullOrEmpty(notRange));

        var notRangeBytes = await redis.String.GetRangeBytesAsync(notKey, 0, -1);
        Assert.True(notRangeBytes is null or { Length: 0 });
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GetSet(Redis redis)
    {
        const string key = "getset_test";
        const string value = "getset";

        _ = redis.Key.Del(key);
        var getset = redis.String.GetSet(key, value);
        Assert.Null(getset);

        getset = redis.String.GetSet(key, "getset_new");
        Assert.Equal(value, getset);

        getset = redis.String.GetSet(key, Encoding.UTF8.GetBytes("hello"));
        Assert.Equal("getset_new", getset);

        var getsetBytes = redis.String.GetSetBytes(key, "redis");
        Assert.Equal(Encoding.UTF8.GetBytes("hello"), getsetBytes);

        getsetBytes = redis.String.GetSetBytes(key, Encoding.UTF8.GetBytes("0"));
        Assert.Equal(Encoding.UTF8.GetBytes("redis"), getsetBytes);

        var get = redis.String.Get(key);
        Assert.Equal("0", get);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GetSetAsync(Redis redis)
    {
        const string key = "getset_test_async";
        const string value = "getset";

        _ = await redis.Key.DelAsync(key);
        var getset = await redis.String.GetSetAsync(key, value);
        Assert.Null(getset);

        getset = await redis.String.GetSetAsync(key, "getset_new");
        Assert.Equal(value, getset);

        getset = await redis.String.GetSetAsync(key, Encoding.UTF8.GetBytes("hello"));
        Assert.Equal("getset_new", getset);

        var getsetBytes = await redis.String.GetSetBytesAsync(key, "redis");
        Assert.Equal(Encoding.UTF8.GetBytes("hello"), getsetBytes);

        getsetBytes = await redis.String.GetSetBytesAsync(key, Encoding.UTF8.GetBytes("0"));
        Assert.Equal(Encoding.UTF8.GetBytes("redis"), getsetBytes);

        var get = await redis.String.GetAsync(key);
        Assert.Equal("0", get);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void Incr(Redis redis)
    {
        const string key = "incr_key";
        _ = redis.Key.Del(key);
        int num = redis.String.Incr(key);
        Assert.Equal(1, num);
        num = redis.String.Incr(key);
        Assert.Equal(2, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task IncrAsync(Redis redis)
    {
        const string key = "incr_key_async";
        _ = await redis.Key.DelAsync(key);
        int num = await redis.String.IncrAsync(key);
        Assert.Equal(1, num);
        num = await redis.String.IncrAsync(key);
        Assert.Equal(2, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void IncrBy(Redis redis)
    {
        const string key = "incrby_key";
        _ = redis.Key.Del(key);
        int num = redis.String.IncrBy(key, 2);
        Assert.Equal(2, num);
        num = redis.String.IncrBy(key, 1);
        Assert.Equal(3, num);
        num = redis.String.IncrBy(key, -2);
        Assert.Equal(1, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task IncrByAsync(Redis redis)
    {
        const string key = "incrby_key_async";
        _ = await redis.Key.DelAsync(key);
        int num = await redis.String.IncrByAsync(key, 2);
        Assert.Equal(2, num);
        num = await redis.String.IncrByAsync(key, 1);
        Assert.Equal(3, num);
        num = await redis.String.IncrByAsync(key, -2);
        Assert.Equal(1, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void IncrByFloat(Redis redis)
    {
        const string key = "incrbyfloat_key";
        _ = redis.Key.Del(key);
        double @double = redis.String.IncrByFloat(key, 2);
        Assert.Equal(2, @double);

        decimal @decimal = redis.String.IncrByFloat(key, 2.2);
        Assert.Equal(4.2M, @decimal);

        float @float = redis.String.IncrByFloat(key, 1.5);
        Assert.Equal(5.7f, @float);

        long @long = redis.String.IncrByFloat(key, 0.3);
        Assert.Equal(6, @long);

        @decimal = redis.String.IncrByFloat(key, -0.8M);
        Assert.Equal(5.2M, @decimal);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task IncrByFloatAsync(Redis redis)
    {
        const string key = "incrbyfloat_key_async";
        _ = await redis.Key.DelAsync(key);
        double @double = await redis.String.IncrByFloatAsync(key, 2);
        Assert.Equal(2, @double);

        decimal @decimal = await redis.String.IncrByFloatAsync(key, 2.2);
        Assert.Equal(4.2M, @decimal);

        float @float = await redis.String.IncrByFloatAsync(key, 1.5);
        Assert.Equal(5.7f, @float);

        long @long = await redis.String.IncrByFloatAsync(key, 0.3);
        Assert.Equal(6, @long);

        @decimal = await redis.String.IncrByFloatAsync(key, -0.8M);
        Assert.Equal(5.2M, @decimal);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void DecrByFloat(Redis redis)
    {
        const string key = "decrbyfloat_key";
        _ = redis.Key.Del(key);
        double @double = redis.String.DecrByFloat(key, 2);
        Assert.Equal(-2, @double);

        decimal @decimal = redis.String.DecrByFloat(key, 2.2);
        Assert.Equal(-4.2M, @decimal);

        @decimal = redis.String.DecrByFloat(key, -10.2);
        Assert.Equal(6M, @decimal);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task DecrByFloatAsync(Redis redis)
    {
        const string key = "decrbyfloat_key_async";
        _ = await redis.Key.DelAsync(key);
        double @double = await redis.String.DecrByFloatAsync(key, 2);
        Assert.Equal(-2, @double);

        decimal @decimal = await redis.String.DecrByFloatAsync(key, 2.2);
        Assert.Equal(-4.2M, @decimal);

        @decimal = await redis.String.DecrByFloatAsync(key, -10.2);
        Assert.Equal(6M, @decimal);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void Lcs(Redis redis)
    {
        const string lcsKey1 = "lcs_key1";
        const string lcsKey2 = "lcs_key2";
        var mset = redis.String.MSet([lcsKey1, "ohmytext", lcsKey2, "mynewtext"]);
        Assert.True(mset);

        var lscResult = redis.String.Lcs(lcsKey1, lcsKey2);
        Assert.Equal("mytext", lscResult);

        var lscLen = redis.String.LcsLen(lcsKey1, lcsKey2);
        Assert.Equal(6, lscLen);

        var lcsIdx = redis.String.LcsIdx(lcsKey1, lcsKey2);
        Assert.Equal(6, lcsIdx!.Len);

        List<long> idx = [];
        foreach (var i in lcsIdx) idx.Add(i);
        Assert.Equal([4, 7, 5, 8, 2, 3, 0, 1], idx);

        lcsIdx = redis.String.LcsIdx(lcsKey1, lcsKey2, 4);
        Assert.Equal(6, lcsIdx!.Len);

        idx = [];
        foreach (var i in lcsIdx) idx.Add(i);
        Assert.Equal([4, 7, 5, 8], idx);

        lcsIdx = redis.String.LcsIdx(lcsKey1, lcsKey2, 4, true);
        Assert.Equal(6, lcsIdx!.Len);

        idx = [];
        foreach (var i in lcsIdx) idx.Add(i);
        Assert.Equal([4, 7, 5, 8], idx);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LcsAsync(Redis redis)
    {
        const string lcsKey1 = "lcs_key1_async";
        const string lcsKey2 = "lcs_key2_async";
        var mset = await redis.String.MSetAsync([lcsKey1, "ohmytext", lcsKey2, "mynewtext"]);
        Assert.True(mset);

        var lscResult = await redis.String.LcsAsync(lcsKey1, lcsKey2);
        Assert.Equal("mytext", lscResult);

        var lscLen = await redis.String.LcsLenAsync(lcsKey1, lcsKey2);
        Assert.Equal(6, lscLen);

        var lcsIdx = await redis.String.LcsIdxAsync(lcsKey1, lcsKey2);
        Assert.Equal(6, lcsIdx!.Len);

        List<long> idx = [];
        foreach (var i in lcsIdx) idx.Add(i);
        Assert.Equal([4, 7, 5, 8, 2, 3, 0, 1], idx);

        lcsIdx = await redis.String.LcsIdxAsync(lcsKey1, lcsKey2, 4);
        Assert.Equal(6, lcsIdx!.Len);

        idx = [];
        foreach (var i in lcsIdx) idx.Add(i);
        Assert.Equal([4, 7, 5, 8], idx);

        lcsIdx = await redis.String.LcsIdxAsync(lcsKey1, lcsKey2, 4, true);
        Assert.Equal(6, lcsIdx!.Len);

        idx = [];
        foreach (var i in lcsIdx) idx.Add(i);
        Assert.Equal([4, 7, 5, 8], idx);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void MGet_Set(Redis redis)
    {
        const string key1 = "mset_get1";
        const string key2 = "mset_get2";
        const string key3 = "mset_get3";
        const string notKey = "get_not_key";

        var mset = redis.String.MSet([key1, key1, key2, key2, key3, key3]);
        Assert.True(mset);

        var mget = redis.String.MGet([key1, key2, key3, notKey]);
        Assert.Equal(new string?[] { key1, key2, key3, null }, mget);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task MGet_SetAsync(Redis redis)
    {
        const string key1 = "mset_get1_async";
        const string key2 = "mset_get2_async";
        const string key3 = "mset_get3_async";
        const string notKey = "get_not_key_async";

        var mset = await redis.String.MSetAsync([key1, key1, key2, key2, key3, key3]);
        Assert.True(mset);

        var mget = await redis.String.MGetAsync([key1, key2, key3, notKey]);
        Assert.Equal(new string?[] { key1, key2, key3, null }, mget);
    }
}
