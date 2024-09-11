using SharpRedis;
using System.Text;

namespace NET8_Test.Types;

public class HashTest
{
    [Theory, ClassData(typeof(RedisProvider))]
    public void HSet(Redis redis)
    {
        const string key = "hash_hset";

        _ = redis.Key.Del(key);

        var count = redis.Hash.HSet(key, "field1", "hello");
        Assert.Equal(1, count);

        count = redis.Hash.HSet(key, "field1", "hello");
        Assert.Equal(0, count);

        count = redis.Hash.HSet(key, new KeyValuePair<string, string>("field2", "redis"));
        Assert.Equal(1, count);

        count = redis.Hash.HSet(key, "field3", Encoding.UTF8.GetBytes("hash!! !"));
        Assert.Equal(1, count);

        count = redis.Hash.HSet(key, new KeyValuePair<string, byte[]>("field4", Encoding.UTF8.GetBytes("speed~~")));
        Assert.Equal(1, count);

        count = redis.Hash.HSet(key, ["field5", "abcd"]);
        Assert.Equal(1, count);

        count = redis.Hash.HSet(key, ["field5", "abcd", "filed6", "666", "field7", "7777"]);
        Assert.Equal(2, count);

        count = redis.Hash.HSet(key, new Dictionary<string, string>
        {
            { "field8", "888" },
            { "field9", "888" },
            { "field10", "888" },
        });
        Assert.Equal(3, count);

        count = redis.Hash.HSet(key, new Dictionary<string, byte[]>
        {
            { "field11", Encoding.UTF8.GetBytes("bytes[][][][][]]]][[") },
        });
        Assert.Equal(1, count);

        count = redis.Hash.HSet(key, new KeyValuePair<string, string>[]
        {
            new ("field12", "www.redis.io"),
            new ("field12", "www.redis.io"),
            new ("field13", "www.redis.com"),
        });
        Assert.Equal(2, count);

        count = redis.Hash.HSet(key, new KeyValuePair<string, byte[]>[]
        {
            new ("field14", Encoding.UTF8.GetBytes("Every day is a good day!")),
            new ("field15", Encoding.UTF8.GetBytes("Every day is a good day!!!")),
        });
        Assert.Equal(2, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HSetAsync(Redis redis)
    {
        const string key = "hash_hset_async";

        _ = await redis.Key.DelAsync(key);

        var count = await redis.Hash.HSetAsync(key, "field1", "hello");
        Assert.Equal(1, count);

        count = await redis.Hash.HSetAsync(key, "field1", "hello");
        Assert.Equal(0, count);

        count = await redis.Hash.HSetAsync(key, new KeyValuePair<string, string>("field2", "redis"));
        Assert.Equal(1, count);

        count = await redis.Hash.HSetAsync(key, "field3", Encoding.UTF8.GetBytes("hash!! !"));
        Assert.Equal(1, count);

        count = await redis.Hash.HSetAsync(key, new KeyValuePair<string, byte[]>("field4", Encoding.UTF8.GetBytes("speed~~")));
        Assert.Equal(1, count);

        count = await redis.Hash.HSetAsync(key, ["field5", "abcd"]);
        Assert.Equal(1, count);

        count = await redis.Hash.HSetAsync(key, ["field5", "abcd", "filed6", "666", "field7", "7777"]);
        Assert.Equal(2, count);

        count = await redis.Hash.HSetAsync(key, new Dictionary<string, string>
        {
            { "field8", "888" },
            { "field9", "888" },
            { "field10", "888" },
        });
        Assert.Equal(3, count);

        count = await redis.Hash.HSetAsync(key, new Dictionary<string, byte[]>
        {
            { "field11", Encoding.UTF8.GetBytes("bytes[][][][][]]]][[") },
        });
        Assert.Equal(1, count);

        count = await redis.Hash.HSetAsync(key, new KeyValuePair<string, string>[]
        {
            new ("field12", "www.redis.io"),
            new ("field12", "www.redis.io"),
            new ("field13", "www.redis.com"),
        });
        Assert.Equal(2, count);

        count = await redis.Hash.HSetAsync(key, new KeyValuePair<string, byte[]>[]
        {
            new ("field14", Encoding.UTF8.GetBytes("Every day is a good day!")),
            new ("field15", Encoding.UTF8.GetBytes("Every day is a good day!!!")),
        });
        Assert.Equal(2, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HMSet(Redis redis)
    {
        const string key = "hash_hmset";

        _ = redis.Key.Del(key);

        var ok = redis.Hash.HMSet(key, ["field5", "abcd"]);
        Assert.True(ok);

        ok = redis.Hash.HMSet(key, ["field5", "abcd", "filed6", "666", "field7", "7777"]);
        Assert.True(ok);

        ok = redis.Hash.HMSet(key, new Dictionary<string, string>
        {
            { "field8", "888" },
            { "field9", "888" },
            { "field10", "888" },
        });
        Assert.True(ok);

        ok = redis.Hash.HMSet(key, new Dictionary<string, byte[]>
        {
            { "field11", Encoding.UTF8.GetBytes("bytes[][][][][]]]][[") },
        });
        Assert.True(ok);

        ok = redis.Hash.HMSet(key, new KeyValuePair<string, string>[]
        {
            new ("field12", "www.redis.io"),
            new ("field12", "www.redis.io"),
            new ("field13", "www.redis.com"),
        });
        Assert.True(ok);

        ok = redis.Hash.HMSet(key, new KeyValuePair<string, byte[]>[]
        {
            new ("field14", Encoding.UTF8.GetBytes("Every day is a good day!")),
            new ("field15", Encoding.UTF8.GetBytes("Every day is a good day!!!")),
        });
        Assert.True(ok);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HMSetAsync(Redis redis)
    {
        const string key = "hash_hmset_async";

        _ = await redis.Key.DelAsync(key);

        var ok = await redis.Hash.HMSetAsync(key, ["field5", "abcd"]);
        Assert.True(ok);

        ok = await redis.Hash.HMSetAsync(key, ["field5", "abcd", "filed6", "666", "field7", "7777"]);
        Assert.True(ok);

        ok = await redis.Hash.HMSetAsync(key, new Dictionary<string, string>
        {
            { "field8", "888" },
            { "field9", "888" },
            { "field10", "888" },
        });
        Assert.True(ok);

        ok = await redis.Hash.HMSetAsync(key, new Dictionary<string, byte[]>
        {
            { "field11", Encoding.UTF8.GetBytes("bytes[][][][][]]]][[") },
        });
        Assert.True(ok);

        ok = await redis.Hash.HMSetAsync(key, new KeyValuePair<string, string>[]
        {
            new ("field12", "www.redis.io"),
            new ("field12", "www.redis.io"),
            new ("field13", "www.redis.com"),
        });
        Assert.True(ok);

        ok = await redis.Hash.HMSetAsync(key, new KeyValuePair<string, byte[]>[]
        {
            new ("field14", Encoding.UTF8.GetBytes("Every day is a good day!")),
            new ("field15", Encoding.UTF8.GetBytes("Every day is a good day!!!")),
        });
        Assert.True(ok);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HGet(Redis redis)
    {
        const string key = "hash_hget";

        _ = redis.Key.Del(key);

        var get = redis.Hash.HGet(key, "field1");
        Assert.Null(get);

        var getBytes = redis.Hash.HGetBytes(key, "field1");
        Assert.Null(getBytes);

        var count = redis.Hash.HSet(key, "field1", "hello");
        Assert.Equal(1, count);

        get = redis.Hash.HGet(key, "field1");
        Assert.Equal("hello", get);

        getBytes = redis.Hash.HGetBytes(key, "field1");
        Assert.Equal(Encoding.UTF8.GetBytes("hello"), getBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HGetAsync(Redis redis)
    {
        const string key = "hash_hget_async";

        _ = await redis.Key.DelAsync(key);

        var get = await redis.Hash.HGetAsync(key, "field1");
        Assert.Null(get);

        var getBytes = await redis.Hash.HGetBytesAsync(key, "field1");
        Assert.Null(getBytes);

        var count = await redis.Hash.HSetAsync(key, "field1", "hello");
        Assert.Equal(1, count);

        get = await redis.Hash.HGetAsync(key, "field1");
        Assert.Equal("hello", get);

        getBytes = await redis.Hash.HGetBytesAsync(key, "field1");
        Assert.Equal(Encoding.UTF8.GetBytes("hello"), getBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HMGet(Redis redis)
    {
        const string key = "hash_hmget";

        _ = redis.Key.Del(key);

        var count = redis.Hash.HSet(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
        });
        Assert.Equal(3, count);

        var gets = redis.Hash.HMGet(key, ["field1", "field3", "fsdfds", "field2", "gfgggg"]);
        Assert.NotNull(gets);
        Assert.Equal(5, gets.Length);
        Assert.Equal("hello", gets[0]);
        Assert.Equal("good redis  ", gets[1]);
        Assert.Null(gets[2]);
        Assert.Equal("redis", gets[3]);
        Assert.Null(gets[4]);

        gets = redis.Hash.HMGet(key, ["field3"]);
        Assert.NotNull(gets);
        Assert.Single(gets);
        Assert.Equal("good redis  ", gets[0]);

        gets = redis.Hash.HMGet("none_hasg_aaaaaa", ["field1", "field3"]);
        Assert.NotNull(gets);
        Assert.Equal(2, gets.Length);
        Assert.Null(gets[0]);
        Assert.Null(gets[1]);

        var getsBytes = redis.Hash.HMGetBytes(key, ["field1", "field3", "fsdfds", "field2", "gfgggg"]);
        Assert.NotNull(getsBytes);
        Assert.Equal(5, getsBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("hello"), getsBytes[0]);
        Assert.Equal(Encoding.UTF8.GetBytes("good redis  "), getsBytes[1]);
        Assert.Null(getsBytes[2]);
        Assert.Equal(Encoding.UTF8.GetBytes("redis"), getsBytes[3]);
        Assert.Null(getsBytes[4]);

        getsBytes = redis.Hash.HMGetBytes("none_hasg_aaaaaa", ["field1", "field3"]);
        Assert.NotNull(getsBytes);
        Assert.Equal(2, getsBytes.Length);
        Assert.Null(getsBytes[0]);
        Assert.Null(getsBytes[1]);

        getsBytes = redis.Hash.HMGetBytes(key, ["field3"]);
        Assert.NotNull(getsBytes);
        Assert.Single(getsBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("good redis  "), getsBytes[0]);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HMGetAsync(Redis redis)
    {
        const string key = "hash_hmget_async";

        _ = await redis.Key.DelAsync(key);

        var count = await redis.Hash.HSetAsync(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
        });
        Assert.Equal(3, count);

        var gets = await redis.Hash.HMGetAsync(key, ["field1", "field3", "fsdfds", "field2", "gfgggg"]);
        Assert.NotNull(gets);
        Assert.Equal(5, gets.Length);
        Assert.Equal("hello", gets[0]);
        Assert.Equal("good redis  ", gets[1]);
        Assert.Null(gets[2]);
        Assert.Equal("redis", gets[3]);
        Assert.Null(gets[4]);

        gets = await redis.Hash.HMGetAsync(key, ["field3"]);
        Assert.NotNull(gets);
        Assert.Single(gets);
        Assert.Equal("good redis  ", gets[0]);

        gets = await redis.Hash.HMGetAsync("none_hasg_aaaaaa", ["field1", "field3"]);
        Assert.NotNull(gets);
        Assert.Equal(2, gets.Length);
        Assert.Null(gets[0]);
        Assert.Null(gets[1]);

        var getsBytes = await redis.Hash.HMGetBytesAsync(key, ["field1", "field3", "fsdfds", "field2", "gfgggg"]);
        Assert.NotNull(getsBytes);
        Assert.Equal(5, getsBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("hello"), getsBytes[0]);
        Assert.Equal(Encoding.UTF8.GetBytes("good redis  "), getsBytes[1]);
        Assert.Null(getsBytes[2]);
        Assert.Equal(Encoding.UTF8.GetBytes("redis"), getsBytes[3]);
        Assert.Null(getsBytes[4]);

        getsBytes = await redis.Hash.HMGetBytesAsync("none_hasg_aaaaaa", ["field1", "field3"]);
        Assert.NotNull(getsBytes);
        Assert.Equal(2, getsBytes.Length);
        Assert.Null(getsBytes[0]);
        Assert.Null(getsBytes[1]);

        getsBytes = await redis.Hash.HMGetBytesAsync(key, ["field3"]);
        Assert.NotNull(getsBytes);
        Assert.Single(getsBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("good redis  "), getsBytes[0]);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HDel(Redis redis)
    {
        const string key = "hash_hdel";

        _ = redis.Key.Del(key);

        var delCount = redis.Hash.HDel(key, "field1");
        Assert.Equal(0, delCount);

        var count = redis.Hash.HSet(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        delCount = redis.Hash.HDel(key, "field1");
        Assert.Equal(1, delCount);

        var len = redis.Hash.HLen(key);
        Assert.Equal(3, len);

        delCount = redis.Hash.HDel(key, ["field1", "field2", "field3", "fsdfdsfds", "field4", "fsdfdsssssss"]);
        Assert.Equal(3, delCount);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HDelAsync(Redis redis)
    {
        const string key = "hash_hdel_async";

        _ = await redis.Key.DelAsync(key);

        var delCount = await redis.Hash.HDelAsync(key, "field1");
        Assert.Equal(0, delCount);

        var count = await redis.Hash.HSetAsync(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        delCount = await redis.Hash.HDelAsync(key, "field1");
        Assert.Equal(1, delCount);

        var len = await redis.Hash.HLenAsync(key);
        Assert.Equal(3, len);

        delCount = await redis.Hash.HDelAsync(key, ["field1", "field2", "field3", "fsdfdsfds", "field4", "fsdfdsssssss"]);
        Assert.Equal(3, delCount);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HExists(Redis redis)
    {
        const string key = "hash_hexistsl";

        _ = redis.Key.Del(key);

        var exist = redis.Hash.HExists(key, "field1");
        Assert.False(exist);

        var count = redis.Hash.HSet(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        exist = redis.Hash.HExists(key, "field1");
        Assert.True(exist);

        exist = redis.Hash.HExists(key, "field3");
        Assert.True(exist);

        exist = redis.Hash.HExists(key, "none_field");
        Assert.False(exist);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HExistsAsync(Redis redis)
    {
        const string key = "hash_hexistsl_async";

        _ = await redis.Key.DelAsync(key);

        var exist = await redis.Hash.HExistsAsync(key, "field1");
        Assert.False(exist);

        var count = await redis.Hash.HSetAsync(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        exist = await redis.Hash.HExistsAsync(key, "field1");
        Assert.True(exist);

        exist = await redis.Hash.HExistsAsync(key, "field3");
        Assert.True(exist);

        exist = await redis.Hash.HExistsAsync(key, "none_field");
        Assert.False(exist);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HGetAll(Redis redis)
    {
        const string key = "hash_hgetall";

        _ = redis.Key.Del(key);

        var all = redis.Hash.HGetAll(key);
        Assert.Null(all);

        var allBytes = redis.Hash.HGetAllBytes(key);
        Assert.Null(allBytes);

        var count = redis.Hash.HSet(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        all = redis.Hash.HGetAll(key);
        Assert.NotNull(all);
        Assert.Equal(4, all.Count);
        Assert.Equal("hello", all["field1"]);
        Assert.Equal("redis", all["field2"]);
        Assert.Equal("good redis  ", all["field3"]);
        Assert.Equal("www.redis.io", all["field4"]);

        allBytes = redis.Hash.HGetAllBytes(key);
        Assert.NotNull(allBytes);
        Assert.Equal(4, allBytes.Count);
        Assert.Equal(Encoding.UTF8.GetBytes("hello"), allBytes["field1"]);
        Assert.Equal(Encoding.UTF8.GetBytes("redis"), allBytes["field2"]);
        Assert.Equal(Encoding.UTF8.GetBytes("good redis  "), allBytes["field3"]);
        Assert.Equal(Encoding.UTF8.GetBytes("www.redis.io"), allBytes["field4"]);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HGetAllAsync(Redis redis)
    {
        const string key = "hash_hgetall_async";

        _ = await redis.Key.DelAsync(key);

        var all = await redis.Hash.HGetAllAsync(key);
        Assert.Null(all);

        var allBytes = await redis.Hash.HGetAllBytesAsync(key);
        Assert.Null(allBytes);

        var count = await redis.Hash.HSetAsync(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        all = await redis.Hash.HGetAllAsync(key);
        Assert.NotNull(all);
        Assert.Equal(4, all.Count);
        Assert.Equal("hello", all["field1"]);
        Assert.Equal("redis", all["field2"]);
        Assert.Equal("good redis  ", all["field3"]);
        Assert.Equal("www.redis.io", all["field4"]);

        allBytes = await redis.Hash.HGetAllBytesAsync(key);
        Assert.NotNull(allBytes);
        Assert.Equal(4, allBytes.Count);
        Assert.Equal(Encoding.UTF8.GetBytes("hello"), allBytes["field1"]);
        Assert.Equal(Encoding.UTF8.GetBytes("redis"), allBytes["field2"]);
        Assert.Equal(Encoding.UTF8.GetBytes("good redis  "), allBytes["field3"]);
        Assert.Equal(Encoding.UTF8.GetBytes("www.redis.io"), allBytes["field4"]);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HIncrBy(Redis redis)
    {
        const string key = "hash_hincrby";

        _ = redis.Key.Del(key);

        var num = redis.Hash.HIncrBy(key, "num");
        Assert.Equal(1, num);

        num = redis.Hash.HIncrBy(key, "num", 100);
        Assert.Equal(101, num);

        _ = redis.Key.Del(key);

        num = redis.Hash.HIncrBy(key, "num", 55);
        Assert.Equal(55, num);

        num = redis.Hash.HIncrBy(key, "num");
        Assert.Equal(56, num);

        num = redis.Hash.HIncrBy(key, "num", -5);
        Assert.Equal(51, num);

        _ = redis.Key.Del(key);

        num = redis.Hash.HIncrBy(key, "num", -5);
        Assert.Equal(-5, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HIncrByAsync(Redis redis)
    {
        const string key = "hash_hincrby_async";

        _ = await redis.Key.DelAsync(key);

        var num = await redis.Hash.HIncrByAsync(key, "num");
        Assert.Equal(1, num);

        num = await redis.Hash.HIncrByAsync(key, "num", 100);
        Assert.Equal(101, num);

        _ = await redis.Key.DelAsync(key);

        num = await redis.Hash.HIncrByAsync(key, "num", 55);
        Assert.Equal(55, num);

        num = await redis.Hash.HIncrByAsync(key, "num");
        Assert.Equal(56, num);

        num = await redis.Hash.HIncrByAsync(key, "num", -5);
        Assert.Equal(51, num);

        _ = await redis.Key.DelAsync(key);

        num = await redis.Hash.HIncrByAsync(key, "num", -5);
        Assert.Equal(-5, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HDecrBy(Redis redis)
    {
        const string key = "hash_hdecrby";

        _ = redis.Key.Del(key);

        var num = redis.Hash.HDecrBy(key, "num");
        Assert.Equal(-1, num);

        num = redis.Hash.HDecrBy(key, "num", 100);
        Assert.Equal(-101, num);

        _ = redis.Key.Del(key);

        num = redis.Hash.HDecrBy(key, "num", 55);
        Assert.Equal(-55, num);

        num = redis.Hash.HDecrBy(key, "num");
        Assert.Equal(-56, num);

        num = redis.Hash.HDecrBy(key, "num", -5);
        Assert.Equal(-51, num);

        _ = redis.Key.Del(key);

        num = redis.Hash.HDecrBy(key, "num", -5);
        Assert.Equal(5, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HDecrByAsync(Redis redis)
    {
        const string key = "hash_hdecrby_async";

        _ = await redis.Key.DelAsync(key);

        var num = await redis.Hash.HDecrByAsync(key, "num");
        Assert.Equal(-1, num);

        num = await redis.Hash.HDecrByAsync(key, "num", 100);
        Assert.Equal(-101, num);

        _ = await redis.Key.DelAsync(key);

        num = await redis.Hash.HDecrByAsync(key, "num", 55);
        Assert.Equal(-55, num);

        num = await redis.Hash.HDecrByAsync(key, "num");
        Assert.Equal(-56, num);

        num = await redis.Hash.HDecrByAsync(key, "num", -5);
        Assert.Equal(-51, num);

        _ = redis.Key.Del(key);

        num = await redis.Hash.HDecrByAsync(key, "num", -5);
        Assert.Equal(5, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HIncrByFloat(Redis redis)
    {
        const string key = "hash_hincrbyfloat";

        _ = redis.Key.Del(key);

        var num = redis.Hash.HIncrByFloat(key, "num", 1);
        Assert.Equal(1, num);

        num = redis.Hash.HIncrByFloat(key, "num", 1.23);
        Assert.Equal(2.23, num);

        _ = redis.Key.Del(key);

        num = redis.Hash.HIncrByFloat(key, "num", 3.336333);
        Assert.Equal(3.336333M, num);

        num = redis.Hash.HIncrByFloat(key, "num", -200.24546546);
        Assert.Equal(-196.90913246, num);

        num = redis.Hash.HIncrByFloat(key, "num", 5);
        Assert.Equal(-191.90913246, num);

        _ = redis.Key.Del(key);

        num = redis.Hash.HIncrByFloat(key, "num", -200);
        Assert.Equal(-200, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HIncrByFloatAsync(Redis redis)
    {
        const string key = "hash_hincrbyfloat_async";

        _ = await redis.Key.DelAsync(key);

        var num = await redis.Hash.HIncrByFloatAsync(key, "num", 1);
        Assert.Equal(1, num);

        num = await redis.Hash.HIncrByFloatAsync(key, "num", 1.23);
        Assert.Equal(2.23, num);

        _ = await redis.Key.DelAsync(key);

        num = await redis.Hash.HIncrByFloatAsync(key, "num", 3.336333);
        Assert.Equal(3.336333M, num);

        num = await redis.Hash.HIncrByFloatAsync(key, "num", -200.24546546);
        Assert.Equal(-196.90913246, num);

        num = await redis.Hash.HIncrByFloatAsync(key, "num", 5);
        Assert.Equal(-191.90913246, num);

        _ = await redis.Key.DelAsync(key);

        num = await redis.Hash.HIncrByFloatAsync(key, "num", -200);
        Assert.Equal(-200, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HDecrByFloat(Redis redis)
    {
        const string key = "hash_hdecrbyfloat";

        _ = redis.Key.Del(key);

        var num = redis.Hash.HDecrByFloat(key, "num", 1);
        Assert.Equal(-1, num);

        num = redis.Hash.HDecrByFloat(key, "num", 1.23);
        Assert.Equal(-2.23, num);

        _ = redis.Key.Del(key);

        num = redis.Hash.HDecrByFloat(key, "num", 3.336333);
        Assert.Equal(-3.336333M, num);

        num = redis.Hash.HDecrByFloat(key, "num", -200.24546546);
        Assert.Equal(196.90913246, num);

        num = redis.Hash.HDecrByFloat(key, "num", 5);
        Assert.Equal(191.90913246, num);

        _ = redis.Key.Del(key);

        num = redis.Hash.HDecrByFloat(key, "num", -200);
        Assert.Equal(200, num);

        num = redis.Hash.HDecrByFloat(key, "num", 200);
        Assert.Equal(0, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HDecrByFloatAsync(Redis redis)
    {
        const string key = "hash_hdecrbyfloat_async";

        _ = await redis.Key.DelAsync(key);

        var num = await redis.Hash.HDecrByFloatAsync(key, "num", 1);
        Assert.Equal(-1, num);

        num = await redis.Hash.HDecrByFloatAsync(key, "num", 1.23);
        Assert.Equal(-2.23, num);

        _ = await redis.Key.DelAsync(key);

        num = await redis.Hash.HDecrByFloatAsync(key, "num", 3.336333);
        Assert.Equal(-3.336333M, num);

        num = await redis.Hash.HDecrByFloatAsync(key, "num", -200.24546546);
        Assert.Equal(196.90913246, num);

        num = await redis.Hash.HDecrByFloatAsync(key, "num", 5);
        Assert.Equal(191.90913246, num);

        _ = await redis.Key.DelAsync(key);

        num = await redis.Hash.HDecrByFloatAsync(key, "num", -200);
        Assert.Equal(200, num);

        num = await redis.Hash.HDecrByFloatAsync(key, "num", 200);
        Assert.Equal(0, num);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HKeys(Redis redis)
    {
        const string key = "hash_hkeys";

        _ = redis.Key.Del(key);

        var keys = redis.Hash.HKeys(key);
        Assert.Null(keys);

        var count = redis.Hash.HSet(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        keys = redis.Hash.HKeys(key);
        Assert.NotNull(keys);
        Assert.Equal(4, keys.Length);
        Assert.Contains("field2", keys);
        Assert.Contains("field1", keys);
        Assert.Contains("field3", keys);
        Assert.Contains("field4", keys);
        Assert.DoesNotContain("field5", keys);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HKeysAsync(Redis redis)
    {
        const string key = "hash_hkeys_async";

        _ = await redis.Key.DelAsync(key);

        var keys = await redis.Hash.HKeysAsync(key);
        Assert.Null(keys);

        var count = await redis.Hash.HSetAsync(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        keys = await redis.Hash.HKeysAsync(key);
        Assert.NotNull(keys);
        Assert.Equal(4, keys.Length);
        Assert.Contains("field2", keys);
        Assert.Contains("field1", keys);
        Assert.Contains("field3", keys);
        Assert.Contains("field4", keys);
        Assert.DoesNotContain("field5", keys);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HLen(Redis redis)
    {
        const string key = "hash_hlen";

        _ = redis.Key.Del(key);

        var len = redis.Hash.HLen(key);
        Assert.Equal(0, len);

        var count = redis.Hash.HSet(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        len = redis.Hash.HLen(key);
        Assert.Equal(4, len);

        _ = redis.Hash.HDel(key, "field1");

        len = redis.Hash.HLen(key);
        Assert.Equal(3, len);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HLenAsync(Redis redis)
    {
        const string key = "hash_hlen_async";

        _ = await redis.Key.DelAsync(key);

        var len = await redis.Hash.HLenAsync(key);
        Assert.Equal(0, len);

        var count = await redis.Hash.HSetAsync(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        len = await redis.Hash.HLenAsync(key);
        Assert.Equal(4, len);

        _ = await redis.Hash.HDelAsync(key, "field1");

        len = await redis.Hash.HLenAsync(key);
        Assert.Equal(3, len);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HRandField(Redis redis)
    {
        const string key = "hash_hrandfield";

        _ = redis.Key.Del(key);

        var randField = redis.Hash.HRandField(key);
        Assert.Null(randField);

        var randFieldValue = redis.Hash.HRandFieldWithValues(key);
        Assert.Null(randFieldValue);
        Assert.False(randFieldValue.HasValue);

        var randFieldValueBytes = redis.Hash.HRandFieldWithValuesBytes(key);
        Assert.Null(randFieldValueBytes);
        Assert.False(randFieldValueBytes.HasValue);

        var randFields = redis.Hash.HRandField(key, 1);
        Assert.Null(randFields);

        var randFieldValues = redis.Hash.HRandFieldWithValues(key, 1);
        Assert.Null(randFieldValues);

        var randFieldValuesBytes = redis.Hash.HRandFieldWithValuesBytes(key, 1);
        Assert.Null(randFieldValuesBytes);

        var hash = new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        };

        var count = redis.Hash.HSet(key, hash);
        Assert.Equal(4, count);

        randField = redis.Hash.HRandField(key);
        Assert.NotNull(randField);
        Assert.Contains(randField, hash.Keys);

        randFieldValue = redis.Hash.HRandFieldWithValues(key);
        Assert.NotNull(randFieldValue);
        Assert.True(randFieldValue.HasValue);
        Assert.Contains(randFieldValue.Value.Key, hash.Keys);
        Assert.Equal(hash[randFieldValue.Value.Key], randFieldValue.Value.Value);

        randFieldValueBytes = redis.Hash.HRandFieldWithValuesBytes(key);
        Assert.NotNull(randFieldValueBytes);
        Assert.True(randFieldValueBytes.HasValue);
        Assert.Contains(randFieldValueBytes.Value.Key, hash.Keys);
        Assert.Equal(Encoding.UTF8.GetBytes(hash[randFieldValueBytes.Value.Key]), randFieldValueBytes.Value.Value);

        randFields = redis.Hash.HRandField(key, 1);
        Assert.NotNull(randFields);
        Assert.Single(randFields);
        Assert.Contains(randFields[0], hash.Keys);

        randFields = redis.Hash.HRandField(key, 8);
        Assert.NotNull(randFields);
        Assert.Equal(4, randFields.Length);
        Assert.Contains(randFields[0], hash.Keys);
        Assert.Contains(randFields[1], hash.Keys);
        Assert.Contains(randFields[2], hash.Keys);
        Assert.Contains(randFields[3], hash.Keys);

        randFields = redis.Hash.HRandField(key, 2);
        Assert.NotNull(randFields);
        Assert.Equal(2, randFields.Length);
        Assert.Contains(randFields[0], hash.Keys);
        Assert.Contains(randFields[1], hash.Keys);

        randFieldValues = redis.Hash.HRandFieldWithValues(key, 1);
        Assert.NotNull(randFieldValues);
        Assert.Single(randFieldValues);
        Assert.Contains(randFieldValues[0].Key, hash.Keys);
        Assert.Equal(randFieldValues[0].Value, hash[randFieldValues[0].Key]);

        randFieldValues = redis.Hash.HRandFieldWithValues(key, -10);
        Assert.NotNull(randFieldValues);
        Assert.Equal(10, randFieldValues.Length);

        randFieldValues = redis.Hash.HRandFieldWithValues(key, 10);
        Assert.NotNull(randFieldValues);
        Assert.Equal(4, randFieldValues.Length);

        randFieldValuesBytes = redis.Hash.HRandFieldWithValuesBytes(key, 1);
        Assert.NotNull(randFieldValuesBytes);
        Assert.Single(randFieldValuesBytes);
        Assert.Contains(randFieldValuesBytes[0].Key, hash.Keys);
        Assert.Equal(randFieldValuesBytes[0].Value, Encoding.UTF8.GetBytes(hash[randFieldValuesBytes[0].Key]));

        randFieldValuesBytes = redis.Hash.HRandFieldWithValuesBytes(key, 10);
        Assert.NotNull(randFieldValuesBytes);
        Assert.Equal(4, randFieldValuesBytes.Length);

        randFieldValuesBytes = redis.Hash.HRandFieldWithValuesBytes(key, -100);
        Assert.NotNull(randFieldValuesBytes);
        Assert.Equal(100, randFieldValuesBytes.Length);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HRandFieldAsync(Redis redis)
    {
        const string key = "hash_hrandfield_async";

        _ = await redis.Key.DelAsync(key);

        var randField = await redis.Hash.HRandFieldAsync(key);
        Assert.Null(randField);

        var randFieldValue = await redis.Hash.HRandFieldWithValuesAsync(key);
        Assert.Null(randFieldValue);
        Assert.False(randFieldValue.HasValue);

        var randFieldValueBytes = await redis.Hash.HRandFieldWithValuesBytesAsync(key);
        Assert.Null(randFieldValueBytes);
        Assert.False(randFieldValueBytes.HasValue);

        var randFields = await redis.Hash.HRandFieldAsync(key, 1);
        Assert.Null(randFields);

        var randFieldValues = await redis.Hash.HRandFieldWithValuesAsync(key, 1);
        Assert.Null(randFieldValues);

        var randFieldValuesBytes = await redis.Hash.HRandFieldWithValuesBytesAsync(key, 1);
        Assert.Null(randFieldValuesBytes);

        var hash = new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        };

        var count = await redis.Hash.HSetAsync(key, hash);
        Assert.Equal(4, count);

        randField = await redis.Hash.HRandFieldAsync(key);
        Assert.NotNull(randField);
        Assert.Contains(randField, hash.Keys);

        randFieldValue = await redis.Hash.HRandFieldWithValuesAsync(key);
        Assert.NotNull(randFieldValue);
        Assert.True(randFieldValue.HasValue);
        Assert.Contains(randFieldValue.Value.Key, hash.Keys);
        Assert.Equal(hash[randFieldValue.Value.Key], randFieldValue.Value.Value);

        randFieldValueBytes = await redis.Hash.HRandFieldWithValuesBytesAsync(key);
        Assert.NotNull(randFieldValueBytes);
        Assert.True(randFieldValueBytes.HasValue);
        Assert.Contains(randFieldValueBytes.Value.Key, hash.Keys);
        Assert.Equal(Encoding.UTF8.GetBytes(hash[randFieldValueBytes.Value.Key]), randFieldValueBytes.Value.Value);

        randFields = await redis.Hash.HRandFieldAsync(key, 1);
        Assert.NotNull(randFields);
        Assert.Single(randFields);
        Assert.Contains(randFields[0], hash.Keys);

        randFields = await redis.Hash.HRandFieldAsync(key, 8);
        Assert.NotNull(randFields);
        Assert.Equal(4, randFields.Length);
        Assert.Contains(randFields[0], hash.Keys);
        Assert.Contains(randFields[1], hash.Keys);
        Assert.Contains(randFields[2], hash.Keys);
        Assert.Contains(randFields[3], hash.Keys);

        randFields = await redis.Hash.HRandFieldAsync(key, 2);
        Assert.NotNull(randFields);
        Assert.Equal(2, randFields.Length);
        Assert.Contains(randFields[0], hash.Keys);
        Assert.Contains(randFields[1], hash.Keys);

        randFieldValues = await redis.Hash.HRandFieldWithValuesAsync(key, 1);
        Assert.NotNull(randFieldValues);
        Assert.Single(randFieldValues);
        Assert.Contains(randFieldValues[0].Key, hash.Keys);
        Assert.Equal(randFieldValues[0].Value, hash[randFieldValues[0].Key]);

        randFieldValues = await redis.Hash.HRandFieldWithValuesAsync(key, -10);
        Assert.NotNull(randFieldValues);
        Assert.Equal(10, randFieldValues.Length);

        randFieldValues = await redis.Hash.HRandFieldWithValuesAsync(key, 10);
        Assert.NotNull(randFieldValues);
        Assert.Equal(4, randFieldValues.Length);

        randFieldValuesBytes = await redis.Hash.HRandFieldWithValuesBytesAsync(key, 1);
        Assert.NotNull(randFieldValuesBytes);
        Assert.Single(randFieldValuesBytes);
        Assert.Contains(randFieldValuesBytes[0].Key, hash.Keys);
        Assert.Equal(randFieldValuesBytes[0].Value, Encoding.UTF8.GetBytes(hash[randFieldValuesBytes[0].Key]));

        randFieldValuesBytes = await redis.Hash.HRandFieldWithValuesBytesAsync(key, 10);
        Assert.NotNull(randFieldValuesBytes);
        Assert.Equal(4, randFieldValuesBytes.Length);

        randFieldValuesBytes = await redis.Hash.HRandFieldWithValuesBytesAsync(key, -100);
        Assert.NotNull(randFieldValuesBytes);
        Assert.Equal(100, randFieldValuesBytes.Length);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HSetNx(Redis redis)
    {
        const string key = "hash_hsetnx";

        _ = redis.Key.Del(key);

        var ok = redis.Hash.HSetNx(key, "field1", "ooo");
        Assert.True(ok);

        var get = redis.Hash.HGet(key, "field1");
        Assert.Equal("ooo", get);

        var count = redis.Hash.HSet(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(3, count);

        get = redis.Hash.HGet(key, "field1");
        Assert.Equal("hello", get);

        ok = redis.Hash.HSetNx(key, "field1", "aaaa");
        Assert.False(ok);

        get = redis.Hash.HGet(key, "field1");
        Assert.Equal("hello", get);

        ok = redis.Hash.HSetNx(key, "field11", "aaaa");
        Assert.True(ok);

        ok = redis.Hash.HSetNx(key, "field1", Encoding.UTF8.GetBytes("bytes"));
        Assert.False(ok);

        ok = redis.Hash.HSetNx(key, "field520", Encoding.UTF8.GetBytes("bytes"));
        Assert.True(ok);

        get = redis.Hash.HGet(key, "field520");
        Assert.Equal("bytes", get);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HSetNxAsync(Redis redis)
    {
        const string key = "hash_hsetnx_async";

        _ = await redis.Key.DelAsync(key);

        var ok = await redis.Hash.HSetNxAsync(key, "field1", "ooo");
        Assert.True(ok);

        var get = await redis.Hash.HGetAsync(key, "field1");
        Assert.Equal("ooo", get);

        var count = await redis.Hash.HSetAsync(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(3, count);

        get = await redis.Hash.HGetAsync(key, "field1");
        Assert.Equal("hello", get);

        ok = await redis.Hash.HSetNxAsync(key, "field1", "aaaa");
        Assert.False(ok);

        get = await redis.Hash.HGetAsync(key, "field1");
        Assert.Equal("hello", get);

        ok = await redis.Hash.HSetNxAsync(key, "field11", "aaaa");
        Assert.True(ok);

        ok = await redis.Hash.HSetNxAsync(key, "field1", Encoding.UTF8.GetBytes("bytes"));
        Assert.False(ok);

        ok = await redis.Hash.HSetNxAsync(key, "field520", Encoding.UTF8.GetBytes("bytes"));
        Assert.True(ok);

        get = await redis.Hash.HGetAsync(key, "field520");
        Assert.Equal("bytes", get);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HStrLen(Redis redis)
    {
        const string key = "hash_hstrlen";

        _ = redis.Key.Del(key);

        var strlen = redis.Hash.HStrLen(key, "field1");
        Assert.Equal(0, strlen);

        var count = redis.Hash.HSet(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        strlen = redis.Hash.HStrLen(key, "field1");
        Assert.Equal(5, strlen);

        strlen = redis.Hash.HStrLen(key, "field3");
        Assert.Equal(12, strlen);

        strlen = redis.Hash.HStrLen(key, "faaa");
        Assert.Equal(0, strlen);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HStrLenAsync(Redis redis)
    {
        const string key = "hash_hstrlen_async";

        _ = await redis.Key.DelAsync(key);

        var strlen = await redis.Hash.HStrLenAsync(key, "field1");
        Assert.Equal(0, strlen);

        var count = await redis.Hash.HSetAsync(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        strlen = await redis.Hash.HStrLenAsync(key, "field1");
        Assert.Equal(5, strlen);

        strlen = await redis.Hash.HStrLenAsync(key, "field3");
        Assert.Equal(12, strlen);

        strlen = await redis.Hash.HStrLenAsync(key, "faaa");
        Assert.Equal(0, strlen);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void HVals(Redis redis)
    {
        const string key = "hash_hvals";

        _ = redis.Key.Del(key);

        var vals = redis.Hash.HVals(key);
        Assert.Null(vals);

        var valsBytes = redis.Hash.HValsBytes(key);
        Assert.Null(valsBytes);

        var count = redis.Hash.HSet(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        vals = redis.Hash.HVals(key);
        Assert.NotNull(vals);
        Assert.Equal(4, vals.Length);
        Assert.Contains("redis", vals);
        Assert.Contains("good redis  ", vals);

        valsBytes = redis.Hash.HValsBytes(key);
        Assert.NotNull(valsBytes);
        Assert.Equal(4, valsBytes.Length);
        Assert.Contains(Encoding.UTF8.GetBytes("redis"), valsBytes);
        Assert.Contains(Encoding.UTF8.GetBytes("good redis  "), valsBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task HValsAsync(Redis redis)
    {
        const string key = "hash_hvals_async";

        _ = await redis.Key.DelAsync(key);

        var vals = await redis.Hash.HValsAsync(key);
        Assert.Null(vals);

        var valsBytes = await redis.Hash.HValsBytesAsync(key);
        Assert.Null(valsBytes);

        var count = await redis.Hash.HSetAsync(key, new Dictionary<string, string>
        {
            { "field1", "hello" },
            { "field2", "redis" },
            { "field3", "good redis  " },
            { "field4", "www.redis.io" },
        });
        Assert.Equal(4, count);

        vals = await redis.Hash.HValsAsync(key);
        Assert.NotNull(vals);
        Assert.Equal(4, vals.Length);
        Assert.Contains("redis", vals);
        Assert.Contains("good redis  ", vals);

        valsBytes = await redis.Hash.HValsBytesAsync(key);
        Assert.NotNull(valsBytes);
        Assert.Equal(4, valsBytes.Length);
        Assert.Contains(Encoding.UTF8.GetBytes("redis"), valsBytes);
        Assert.Contains(Encoding.UTF8.GetBytes("good redis  "), valsBytes);
    }
}
