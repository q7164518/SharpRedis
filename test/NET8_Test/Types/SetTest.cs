using SharpRedis;
using System.Text;

namespace NET8_Test.Types;

public class SetTest
{
    [Theory, ClassData(typeof(RedisProvider))]
    public void SAdd(Redis redis)
    {
        const string key = "set_sadd_key";
        _ = redis.Key.Del(key);
        var result = redis.Set.SAdd(key, "abc");
        Assert.Equal(1, result);

        result = redis.Set.SAdd(key, "abc");
        Assert.Equal(0, result);

        result = redis.Set.SAdd(key, Encoding.UTF8.GetBytes("redis"));
        Assert.Equal(1, result);

        result = redis.Set.SAdd(key, Encoding.UTF8.GetBytes("redis"));
        Assert.Equal(0, result);

        result = redis.Set.SAdd(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        result = redis.Set.SAdd(key, ["a", "b", "cv"]);
        Assert.Equal(0, result);

        result = redis.Set.SAdd(key, [Encoding.UTF8.GetBytes("hello"), Encoding.UTF8.GetBytes("jk"), Encoding.UTF8.GetBytes("redis")]);
        Assert.Equal(2, result);

        result = redis.Set.SAdd(key, [Encoding.UTF8.GetBytes("hello"), Encoding.UTF8.GetBytes("jk"), Encoding.UTF8.GetBytes("redis")]);
        Assert.Equal(0, result);
        _ = redis.Key.Del(key);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SAddAsync(Redis redis)
    {
        const string key = "set_sadd_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.Set.SAddAsync(key, "abc");
        Assert.Equal(1, result);

        result = await redis.Set.SAddAsync(key, "abc");
        Assert.Equal(0, result);

        result = await redis.Set.SAddAsync(key, Encoding.UTF8.GetBytes("redis"));
        Assert.Equal(1, result);

        result = await redis.Set.SAddAsync(key, Encoding.UTF8.GetBytes("redis"));
        Assert.Equal(0, result);

        result = await redis.Set.SAddAsync(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        result = await redis.Set.SAddAsync(key, ["a", "b", "cv"]);
        Assert.Equal(0, result);

        result = await redis.Set.SAddAsync(key, [Encoding.UTF8.GetBytes("hello"), Encoding.UTF8.GetBytes("jk"), Encoding.UTF8.GetBytes("redis")]);
        Assert.Equal(2, result);

        result = await redis.Set.SAddAsync(key, [Encoding.UTF8.GetBytes("hello"), Encoding.UTF8.GetBytes("jk"), Encoding.UTF8.GetBytes("redis")]);
        Assert.Equal(0, result);
        _ = await redis.Key.DelAsync(key);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SCard(Redis redis)
    {
        const string key = "set_scard_key";
        _ = redis.Key.Del(key);
        var result = redis.Set.SAdd(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        var count = redis.Set.SCard(key);
        Assert.Equal(3, count);

        count = redis.Set.SCard("nonoe_set_scard");
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SCardAsync(Redis redis)
    {
        const string key = "set_scard_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.Set.SAddAsync(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        var count = await redis.Set.SCardAsync(key);
        Assert.Equal(3, count);

        count = await redis.Set.SCardAsync("nonoe_set_scard");
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SDiff(Redis redis)
    {
        const string key1 = "set_sdiff_key1";
        const string key2 = "set_sdiff_key2";
        const string key3 = "set_sdiff_key3";
        _ = redis.Key.Del([key1, key2, key3]);
        var result = redis.Set.SAdd(key1, ["a", "b", "cv", "c1"]);
        Assert.Equal(4, result);

        result = redis.Set.SAdd(key2, ["a", "b", "c"]);
        Assert.Equal(3, result);

        result = redis.Set.SAdd(key3, ["hello", "redis", "yyds", "speed", "a"]);
        Assert.Equal(5, result);

        var diff = redis.Set.SDiff(key1, key2);
        Assert.NotNull(diff);
        Assert.Equal<string[]>(["cv", "c1"], diff);

        diff = redis.Set.SDiff(key1, [key2, key3]);
        Assert.NotNull(diff);
        Assert.Equal<string[]>(["cv", "c1"], diff);

        diff = redis.Set.SDiff([key1, key2, key3]);
        Assert.NotNull(diff);
        Assert.Equal<string[]>(["cv", "c1"], diff);

        diff = redis.Set.SDiff([key1, "nonono"]);
        Assert.NotNull(diff);
        Assert.Equal<string[]>(["a", "b", "cv", "c1"], diff);

        diff = redis.Set.SDiff(["nononoffff", "nonono"]);
        Assert.Null(diff);

        var diffBytes = redis.Set.SDiffBytes(key1, key2);
        Assert.NotNull(diffBytes);
        Assert.Equal([Encoding.UTF8.GetBytes("cv"), Encoding.UTF8.GetBytes("c1")], diffBytes);

        diffBytes = redis.Set.SDiffBytes(key1, [key2, key3]);
        Assert.NotNull(diffBytes);
        Assert.Equal([Encoding.UTF8.GetBytes("cv"), Encoding.UTF8.GetBytes("c1")], diffBytes);

        diffBytes = redis.Set.SDiffBytes([key1, key2, key3]);
        Assert.NotNull(diffBytes);
        Assert.Equal([Encoding.UTF8.GetBytes("cv"), Encoding.UTF8.GetBytes("c1")], diffBytes);

        _ = redis.Key.Del([key1, key2, key3]);

        result = redis.Set.SAdd(key1, ["a", "b", "cv", "c1"]);
        Assert.Equal(4, result);

        result = redis.Set.SAdd(key2, ["a", "b", "cv", "c1"]);
        Assert.Equal(4, result);

        diffBytes = redis.Set.SDiffBytes(key1, key2);
        Assert.Null(diffBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SDiffAsync(Redis redis)
    {
        const string key1 = "set_sdiff_key1_async";
        const string key2 = "set_sdiff_key2_async";
        const string key3 = "set_sdiff_key3_async";
        _ = await redis.Key.DelAsync([key1, key2, key3]);
        var result = await redis.Set.SAddAsync(key1, ["a", "b", "cv", "c1"]);
        Assert.Equal(4, result);

        result = await redis.Set.SAddAsync(key2, ["a", "b", "c"]);
        Assert.Equal(3, result);

        result = await redis.Set.SAddAsync(key3, ["hello", "redis", "yyds", "speed", "a"]);
        Assert.Equal(5, result);

        var diff = await redis.Set.SDiffAsync(key1, key2);
        Assert.NotNull(diff);
        Assert.Equal<string[]>(["cv", "c1"], diff);

        diff = await redis.Set.SDiffAsync(key1, [key2, key3]);
        Assert.NotNull(diff);
        Assert.Equal<string[]>(["cv", "c1"], diff);

        diff = await redis.Set.SDiffAsync([key1, key2, key3]);
        Assert.NotNull(diff);
        Assert.Equal<string[]>(["cv", "c1"], diff);

        diff = await redis.Set.SDiffAsync([key1, "nonono"]);
        Assert.NotNull(diff);
        Assert.Equal<string[]>(["a", "b", "cv", "c1"], diff);

        diff = await redis.Set.SDiffAsync(["nononoffff", "nonono"]);
        Assert.Null(diff);

        var diffBytes = await redis.Set.SDiffBytesAsync(key1, key2);
        Assert.NotNull(diffBytes);
        Assert.Equal([Encoding.UTF8.GetBytes("cv"), Encoding.UTF8.GetBytes("c1")], diffBytes);

        diffBytes = await redis.Set.SDiffBytesAsync(key1, [key2, key3]);
        Assert.NotNull(diffBytes);
        Assert.Equal([Encoding.UTF8.GetBytes("cv"), Encoding.UTF8.GetBytes("c1")], diffBytes);

        diffBytes = await redis.Set.SDiffBytesAsync([key1, key2, key3]);
        Assert.NotNull(diffBytes);
        Assert.Equal([Encoding.UTF8.GetBytes("cv"), Encoding.UTF8.GetBytes("c1")], diffBytes);

        _ = await redis.Key.DelAsync([key1, key2, key3]);

        result = await redis.Set.SAddAsync(key1, ["a", "b", "cv", "c1"]);
        Assert.Equal(4, result);

        result = await redis.Set.SAddAsync(key2, ["a", "b", "cv", "c1"]);
        Assert.Equal(4, result);

        diffBytes = await redis.Set.SDiffBytesAsync(key1, key2);
        Assert.Null(diffBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SDiffStore(Redis redis)
    {
        const string key1 = "set_sdiffstore_key1";
        const string key2 = "set_sdiffstore_key2";
        const string key3 = "set_sdiffstore_key3";
        const string destination = "set_sdiffstore_dest";
        _ = redis.Key.Del([key1, key2, key3, destination]);
        var result = redis.Set.SAdd(key1, ["a", "b", "cv", "c1"]);
        Assert.Equal(4, result);

        result = redis.Set.SAdd(key2, ["a", "b", "c"]);
        Assert.Equal(3, result);

        result = redis.Set.SAdd(key3, ["hello", "redis", "yyds", "speed", "a"]);
        Assert.Equal(5, result);

        var count = redis.Set.SDiffStore(destination, key1, key2);
        Assert.Equal(2, count);

        count = redis.Set.SDiffStore(destination, key1, key3);
        Assert.Equal(3, count);

        count = redis.Set.SDiffStore(destination, key1, [key2, key3]);
        Assert.Equal(2, count);

        count = redis.Set.SDiffStore(destination, [key1, key2, key3]);
        Assert.Equal(2, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SDiffStoreAsync(Redis redis)
    {
        const string key1 = "set_sdiffstore_key1_async";
        const string key2 = "set_sdiffstore_key2_async";
        const string key3 = "set_sdiffstore_key3_async";
        const string destination = "set_sdiffstore_dest_async";
        _ = await redis.Key.DelAsync([key1, key2, key3, destination]);
        var result = await redis.Set.SAddAsync(key1, ["a", "b", "cv", "c1"]);
        Assert.Equal(4, result);

        result = await redis.Set.SAddAsync(key2, ["a", "b", "c"]);
        Assert.Equal(3, result);

        result = await redis.Set.SAddAsync(key3, ["hello", "redis", "yyds", "speed", "a"]);
        Assert.Equal(5, result);

        var count = await redis.Set.SDiffStoreAsync(destination, key1, key2);
        Assert.Equal(2, count);

        count = await redis.Set.SDiffStoreAsync(destination, key1, key3);
        Assert.Equal(3, count);

        count = await redis.Set.SDiffStoreAsync(destination, key1, [key2, key3]);
        Assert.Equal(2, count);

        count = await redis.Set.SDiffStoreAsync(destination, [key1, key2, key3]);
        Assert.Equal(2, count);

        count = await redis.Set.SDiffStoreAsync(destination, [key1, "nonon"]);
        Assert.Equal(4, count);

        count = await redis.Set.SDiffStoreAsync(destination, ["nononon11111", "nonon"]);
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SInter(Redis redis)
    {
        const string key1 = "set_sinter_key1";
        const string key2 = "set_sinter_key2";
        const string key3 = "set_sinter_key3";
        _ = redis.Key.Del([key1, key2, key3]);
        var result = redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        result = redis.Set.SAdd(key2, "c");
        Assert.Equal(1, result);

        result = redis.Set.SAdd(key3, ["a", "c", "e"]);
        Assert.Equal(3, result);

        var inter = redis.Set.SInter(key1, key2);
        Assert.Equal<string[]>(["c"], inter);

        inter = redis.Set.SInter(key1, key3);
        Assert.Equal<string[]>(["a", "c"], inter);

        inter = redis.Set.SInter(key1, [key2, key3]);
        Assert.Equal<string[]>(["c"], inter);

        inter = redis.Set.SInter([key1, key2, key3]);
        Assert.Equal<string[]>(["c"], inter);

        inter = redis.Set.SInter([key1, "fsdfdsf"]);
        Assert.Null(inter);

        var interBytes = redis.Set.SInterBytes(key1, key2);
        Assert.Equal([Encoding.UTF8.GetBytes("c")], interBytes);

        interBytes = redis.Set.SInterBytes(key1, key3);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("c")], interBytes);

        interBytes = redis.Set.SInterBytes(key1, [key2, key3]);
        Assert.Equal([Encoding.UTF8.GetBytes("c")], interBytes);

        interBytes = redis.Set.SInterBytes([key1, key2, key3]);
        Assert.Equal([Encoding.UTF8.GetBytes("c")], interBytes);

        interBytes = redis.Set.SInterBytes(key1, "nononononnnnnn");
        Assert.Null(interBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SInterAsync(Redis redis)
    {
        const string key1 = "set_sinter_key1_async";
        const string key2 = "set_sinter_key2_async";
        const string key3 = "set_sinter_key3_async";
        _ = await redis.Key.DelAsync([key1, key2, key3]);
        var result = await redis.Set.SAddAsync(key1, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        result = await redis.Set.SAddAsync(key2, "c");
        Assert.Equal(1, result);

        result = await redis.Set.SAddAsync(key3, ["a", "c", "e"]);
        Assert.Equal(3, result);

        var inter = await redis.Set.SInterAsync(key1, key2);
        Assert.Equal<string[]>(["c"], inter);

        inter = await redis.Set.SInterAsync(key1, key3);
        Assert.Equal<string[]>(["a", "c"], inter);

        inter = await redis.Set.SInterAsync(key1, [key2, key3]);
        Assert.Equal<string[]>(["c"], inter);

        inter = await redis.Set.SInterAsync([key1, key2, key3]);
        Assert.Equal<string[]>(["c"], inter);

        inter = await redis.Set.SInterAsync([key1, "fsdfdsf"]);
        Assert.Null(inter);

        var interBytes = await redis.Set.SInterBytesAsync(key1, key2);
        Assert.Equal([Encoding.UTF8.GetBytes("c")], interBytes);

        interBytes = await redis.Set.SInterBytesAsync(key1, key3);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("c")], interBytes);

        interBytes = await redis.Set.SInterBytesAsync(key1, [key2, key3]);
        Assert.Equal([Encoding.UTF8.GetBytes("c")], interBytes);

        interBytes = await redis.Set.SInterBytesAsync([key1, key2, key3]);
        Assert.Equal([Encoding.UTF8.GetBytes("c")], interBytes);

        interBytes = await redis.Set.SInterBytesAsync(key1, "nononononnnnnn");
        Assert.Null(interBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SInterCard(Redis redis)
    {
        const string key1 = "set_sintercard_key1";
        const string key2 = "set_sintercard_key2";
        const string key3 = "set_sintercard_key3";
        _ = redis.Key.Del([key1, key2, key3]);
        var result = redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        result = redis.Set.SAdd(key2, "c");
        Assert.Equal(1, result);

        result = redis.Set.SAdd(key3, ["a", "c", "e"]);
        Assert.Equal(3, result);

        var count = redis.Set.SInterCard(key1, key2);
        Assert.Equal(1, count);

        count = redis.Set.SInterCard(key1, key3);
        Assert.Equal(2, count);

        count = redis.Set.SInterCard(key1, key3, 1);
        Assert.Equal(1, count);

        count = redis.Set.SInterCard(key1, [key2, key3]);
        Assert.Equal(1, count);

        count = redis.Set.SInterCard([key1, key2, key3]);
        Assert.Equal(1, count);

        count = redis.Set.SInterCard([key1, key2, key3, "fsdfsdfdsfnonono"]);
        Assert.Equal(0, count);

        count = redis.Set.SInterCard(["121212", "fsdfdsfds", "nonono"]);
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SInterCardAsync(Redis redis)
    {
        const string key1 = "set_sintercard_key1_async";
        const string key2 = "set_sintercard_key2_async";
        const string key3 = "set_sintercard_key3_async";
        _ = await redis.Key.DelAsync([key1, key2, key3]);
        var result = await redis.Set.SAddAsync(key1, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        result = await redis.Set.SAddAsync(key2, "c");
        Assert.Equal(1, result);

        result = await redis.Set.SAddAsync(key3, ["a", "c", "e"]);
        Assert.Equal(3, result);

        var count = await redis.Set.SInterCardAsync(key1, key2);
        Assert.Equal(1, count);

        count = await redis.Set.SInterCardAsync(key1, key3);
        Assert.Equal(2, count);

        count = await redis.Set.SInterCardAsync(key1, key3, 1);
        Assert.Equal(1, count);

        count = await redis.Set.SInterCardAsync(key1, [key2, key3]);
        Assert.Equal(1, count);

        count = await redis.Set.SInterCardAsync([key1, key2, key3]);
        Assert.Equal(1, count);

        count = await redis.Set.SInterCardAsync([key1, key2, key3, "fsdfsdfdsfnonono"]);
        Assert.Equal(0, count);

        count = await redis.Set.SInterCardAsync(["121212", "fsdfdsfds", "nonono"]);
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SInterStore(Redis redis)
    {
        const string key1 = "set_sinterstore_key1";
        const string key2 = "set_sinterstore_key2";
        const string key3 = "set_sinterstore_key3";
        const string destination = "set_sinterstore_dest";

        _ = redis.Key.Del([key1, key2, key3]);
        var result = redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        result = redis.Set.SAdd(key2, "c");
        Assert.Equal(1, result);

        result = redis.Set.SAdd(key3, ["a", "c", "e"]);
        Assert.Equal(3, result);

        var count = redis.Set.SInterStore(destination, key1, key2);
        Assert.Equal(1, count);

        count = redis.Set.SInterStore(destination, key1, key3);
        Assert.Equal(2, count);

        count = redis.Set.SInterStore(destination, key1, [key2, key3]);
        Assert.Equal(1, count);

        count = redis.Set.SInterStore(destination, [key1, key2, key3]);
        Assert.Equal(1, count);

        count = redis.Set.SInterStore(destination, [key1, key2, key3, "fsdfsdfdsfnonono"]);
        Assert.Equal(0, count);

        count = redis.Set.SInterStore(destination, ["121212", "fsdfdsfds", "nonono"]);
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SInterStoreAsync(Redis redis)
    {
        const string key1 = "set_sinterstore_key1_async";
        const string key2 = "set_sinterstore_key2_async";
        const string key3 = "set_sinterstore_key3_async";
        const string destination = "set_sinterstore_dest_async";

        _ = await redis.Key.DelAsync([key1, key2, key3]);
        var result = await redis.Set.SAddAsync(key1, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        result = await redis.Set.SAddAsync(key2, "c");
        Assert.Equal(1, result);

        result = await redis.Set.SAddAsync(key3, ["a", "c", "e"]);
        Assert.Equal(3, result);

        var count = await redis.Set.SInterStoreAsync(destination, key1, key2);
        Assert.Equal(1, count);

        count = await redis.Set.SInterStoreAsync(destination, key1, key3);
        Assert.Equal(2, count);

        count = await redis.Set.SInterStoreAsync(destination, key1, [key2, key3]);
        Assert.Equal(1, count);

        count = await redis.Set.SInterStoreAsync(destination, [key1, key2, key3]);
        Assert.Equal(1, count);

        count = await redis.Set.SInterStoreAsync(destination, [key1, key2, key3, "fsdfsdfdsfnonono"]);
        Assert.Equal(0, count);

        count = await redis.Set.SInterStoreAsync(destination, ["121212", "fsdfdsfds", "nonono"]);
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SIsMember(Redis redis)
    {
        const string key = "set_sismember_key";
        _ = redis.Key.Del(key);
        var result = redis.Set.SAdd(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        var any = redis.Set.SIsMember(key, "a");
        Assert.True(any);

        any = redis.Set.SIsMember(key, Encoding.UTF8.GetBytes("cv"));
        Assert.True(any);

        any = redis.Set.SIsMember(key, "aa");
        Assert.False(any);

        any = redis.Set.SIsMember("nonono_key", "a");
        Assert.False(any);

        any = redis.Set.SIsMember(key, Encoding.UTF8.GetBytes("ccv"));
        Assert.False(any);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SIsMemberAsync(Redis redis)
    {
        const string key = "set_sismember_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.Set.SAddAsync(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        var any = await redis.Set.SIsMemberAsync(key, "a");
        Assert.True(any);

        any = await redis.Set.SIsMemberAsync(key, Encoding.UTF8.GetBytes("cv"));
        Assert.True(any);

        any = await redis.Set.SIsMemberAsync(key, "aa");
        Assert.False(any);

        any = await redis.Set.SIsMemberAsync("nonono_key", "a");
        Assert.False(any);

        any = await redis.Set.SIsMemberAsync(key, Encoding.UTF8.GetBytes("ccv"));
        Assert.False(any);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SMembers(Redis redis)
    {
        const string key = "set_smembers_key";
        _ = redis.Key.Del(key);
        var result = redis.Set.SAdd(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        var members = redis.Set.SMembers(key);
        Assert.NotNull(members);
        Assert.Equal<string[]>(["a", "b", "cv"], members);

        members = redis.Set.SMembers("notkey_set");
        Assert.Null(members);

        var membersBytes = redis.Set.SMembersBytes(key);
        Assert.NotNull(membersBytes);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("cv")], membersBytes);

        membersBytes = redis.Set.SMembersBytes("notkey_set");
        Assert.Null(membersBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SMembersAsync(Redis redis)
    {
        const string key = "set_smembers_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.Set.SAddAsync(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        var members = await redis.Set.SMembersAsync(key);
        Assert.NotNull(members);
        Assert.Equal<string[]>(["a", "b", "cv"], members);

        members = await redis.Set.SMembersAsync("notkey_set");
        Assert.Null(members);

        var membersBytes = await redis.Set.SMembersBytesAsync(key);
        Assert.NotNull(membersBytes);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("cv")], membersBytes);

        membersBytes = await redis.Set.SMembersBytesAsync("notkey_set");
        Assert.Null(membersBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SMIsMember(Redis redis)
    {
        const string key = "set_smismember_key";
        _ = redis.Key.Del(key);
        var result = redis.Set.SAdd(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        var anys = redis.Set.SMIsMember(key, ["a", "cv", "b"]);
        Assert.NotNull(anys);
        Assert.Equal(3, anys.Length);
        Assert.Equal<bool[]>([true, true, true], anys);

        anys = redis.Set.SMIsMember(key, ["a", "cv", "b", "ff", "a"]);
        Assert.NotNull(anys);
        Assert.Equal(5, anys.Length);
        Assert.Equal<bool[]>([true, true, true, false, true], anys);

        anys = redis.Set.SMIsMember(key, ["aaa", "caav", "baaa"]);
        Assert.NotNull(anys);
        Assert.Equal(3, anys.Length);
        Assert.Equal<bool[]>([false, false, false], anys);

        anys = redis.Set.SMIsMember("nokey_sssssss_fsdf", ["aaa", "caav", "baaa"]);
        Assert.NotNull(anys);
        Assert.Equal(3, anys.Length);
        Assert.Equal<bool[]>([false, false, false], anys);

        anys = redis.Set.SMIsMember(key, [Encoding.UTF8.GetBytes("a")]);
        Assert.NotNull(anys);
        Assert.Equal<bool[]>([true], anys);

        anys = redis.Set.SMIsMember(key, [Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("aaaa")]);
        Assert.NotNull(anys);
        Assert.Equal<bool[]>([true, false], anys);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SMIsMemberAsync(Redis redis)
    {
        const string key = "set_smismember_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.Set.SAddAsync(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        var anys = await redis.Set.SMIsMemberAsync(key, ["a", "cv", "b"]);
        Assert.NotNull(anys);
        Assert.Equal(3, anys.Length);
        Assert.Equal<bool[]>([true, true, true], anys);

        anys = await redis.Set.SMIsMemberAsync(key, ["a", "cv", "b", "ff", "a"]);
        Assert.NotNull(anys);
        Assert.Equal(5, anys.Length);
        Assert.Equal<bool[]>([true, true, true, false, true], anys);

        anys = await redis.Set.SMIsMemberAsync(key, ["aaa", "caav", "baaa"]);
        Assert.NotNull(anys);
        Assert.Equal(3, anys.Length);
        Assert.Equal<bool[]>([false, false, false], anys);

        anys = await redis.Set.SMIsMemberAsync("nokey_sssssss_fsdf", ["aaa", "caav", "baaa"]);
        Assert.NotNull(anys);
        Assert.Equal(3, anys.Length);
        Assert.Equal<bool[]>([false, false, false], anys);

        anys = await redis.Set.SMIsMemberAsync(key, [Encoding.UTF8.GetBytes("a")]);
        Assert.NotNull(anys);
        Assert.Equal<bool[]>([true], anys);

        anys = await redis.Set.SMIsMemberAsync(key, [Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("aaaa")]);
        Assert.NotNull(anys);
        Assert.Equal<bool[]>([true, false], anys);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SMove(Redis redis)
    {
        const string key = "set_smove_key";
        const string destination = "set_smove_dest";

        _ = redis.Key.Del([key, destination]);
        var result = redis.Set.SAdd(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        var destMembers = redis.Set.SMembers(destination);
        Assert.Null(destMembers);

        var ok = redis.Set.SMove(key, destination, "a");
        Assert.True(ok);

        destMembers = redis.Set.SMembers(destination);
        Assert.Equal<string[]>(["a"], destMembers);

        ok = redis.Set.SMove(key, destination, "a");
        Assert.False(ok);

        var members = redis.Set.SMembers(key);
        Assert.Equal<string[]>(["b", "cv"], members);

        ok = redis.Set.SMove("nononono___kje", destination, "a");
        Assert.False(ok);

        ok = redis.Set.SMove(key, destination, Encoding.UTF8.GetBytes("cv"));
        Assert.True(ok);

        destMembers = redis.Set.SMembers(destination);
        Assert.Equal<string[]>(["a", "cv"], destMembers);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SMoveAsync(Redis redis)
    {
        const string key = "set_smove_key_async";
        const string destination = "set_smove_dest_async";

        _ = await redis.Key.DelAsync([key, destination]);
        var result = await redis.Set.SAddAsync(key, ["a", "b", "cv"]);
        Assert.Equal(3, result);

        var destMembers = await redis.Set.SMembersAsync(destination);
        Assert.Null(destMembers);

        var ok = await redis.Set.SMoveAsync(key, destination, "a");
        Assert.True(ok);

        destMembers = await redis.Set.SMembersAsync(destination);
        Assert.Equal<string[]>(["a"], destMembers);

        ok = await redis.Set.SMoveAsync(key, destination, "a");
        Assert.False(ok);

        var members = await redis.Set.SMembersAsync(key);
        Assert.Equal<string[]>(["b", "cv"], members);

        ok = await redis.Set.SMoveAsync("nononono___kje", destination, "a");
        Assert.False(ok);

        ok = await redis.Set.SMoveAsync(key, destination, Encoding.UTF8.GetBytes("cv"));
        Assert.True(ok);

        destMembers = await redis.Set.SMembersAsync(destination);
        Assert.Equal<string[]>(["a", "cv"], destMembers);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SPop(Redis redis)
    {
        const string key = "set_spop_key";

        _ = redis.Key.Del(key);
        _ = redis.Set.SAdd(key, "one");
        _ = redis.Set.SAdd(key, "two");
        _ = redis.Set.SAdd(key, Encoding.UTF8.GetBytes("three"));

        var pop = redis.Set.SPop(key);
        Assert.NotNull(pop);

        pop = redis.Set.SPop("nonono_kkkkey");
        Assert.Null(pop);

        _ = redis.Set.SAdd(key, "four");
        _ = redis.Set.SAdd(key, "five");

        var pops = redis.Set.SPop(key, 3);
        Assert.NotNull(pops);
        Assert.Equal(3, pops.Length);

        pops = redis.Set.SPop("nononono)keykeykkk", 3);
        Assert.Null(pops);

        var popsBytes = redis.Set.SPopBytes(key, 3);
        Assert.NotNull(popsBytes);
        Assert.Single(popsBytes);

        popsBytes = redis.Set.SPopBytes("aaaaaaanonoekey", 3);
        Assert.Null(popsBytes);

        _ = redis.Set.SAdd(key, "one");

        var popBytes = redis.Set.SPopBytes(key);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), popBytes);

        popBytes = redis.Set.SPopBytes(key);
        Assert.Null(popBytes);

        _ = redis.Set.SAdd(key, "one");
        _ = redis.Set.SAdd(key, "two");
        _ = redis.Set.SAdd(key, Encoding.UTF8.GetBytes("three"));

        pops = redis.Set.SPop(key, 1);
        Assert.NotNull(pops);
        Assert.Single(pops);

        popsBytes = redis.Set.SPopBytes(key, 1);
        Assert.NotNull(popsBytes);
        Assert.Single(popsBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SPopAsync(Redis redis)
    {
        const string key = "set_spop_key_async";

        _ = await redis.Key.DelAsync(key);
        _ = await redis.Set.SAddAsync(key, "one");
        _ = await redis.Set.SAddAsync(key, "two");
        _ = await redis.Set.SAddAsync(key, Encoding.UTF8.GetBytes("three"));

        var pop = await redis.Set.SPopAsync(key);
        Assert.NotNull(pop);

        pop = await redis.Set.SPopAsync("nonono_kkkkey");
        Assert.Null(pop);

        _ = await redis.Set.SAddAsync(key, "four");
        _ = await redis.Set.SAddAsync(key, "five");

        var pops = await redis.Set.SPopAsync(key, 3);
        Assert.NotNull(pops);
        Assert.Equal(3, pops.Length);

        pops = await redis.Set.SPopAsync("nononono)keykeykkk", 3);
        Assert.Null(pops);

        var popsBytes = await redis.Set.SPopBytesAsync(key, 3);
        Assert.NotNull(popsBytes);
        Assert.Single(popsBytes);

        popsBytes = await redis.Set.SPopBytesAsync("aaaaaaanonoekey", 3);
        Assert.Null(popsBytes);

        _ = await redis.Set.SAddAsync(key, "one");

        var popBytes = await redis.Set.SPopBytesAsync(key);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), popBytes);

        popBytes = await redis.Set.SPopBytesAsync(key);
        Assert.Null(popBytes);

        _ = await redis.Set.SAddAsync(key, "one");
        _ = await redis.Set.SAddAsync(key, "two");
        _ = await redis.Set.SAddAsync(key, Encoding.UTF8.GetBytes("three"));

        pops = await redis.Set.SPopAsync(key, 1);
        Assert.NotNull(pops);
        Assert.Single(pops);

        popsBytes = await redis.Set.SPopBytesAsync(key, 1);
        Assert.NotNull(popsBytes);
        Assert.Single(popsBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SRandMember(Redis redis)
    {
        const string key = "set_srandmember_key";

        _ = redis.Key.Del(key);
        var result = redis.Set.SAdd(key, ["a", "b", "c", "d", "ee", "fff", "gggg", "redis"]);
        Assert.Equal(8, result);

        var rand = redis.Set.SRandMember(key);
        Assert.NotNull(rand);

        rand = redis.Set.SRandMember("not_rand_key");
        Assert.Null(rand);

        var rands = redis.Set.SRandMember(key, 8);
        Assert.NotNull(rands);
        Assert.Equal(8, rands.Length);

        rands = redis.Set.SRandMember(key, 80);
        Assert.NotNull(rands);
        Assert.Equal(8, rands.Length);

        rands = redis.Set.SRandMember(key, -4);
        Assert.NotNull(rands);
        Assert.Equal(4, rands.Length);

        rands = redis.Set.SRandMember(key, -8);
        Assert.NotNull(rands);
        Assert.Equal(8, rands.Length);

        rands = redis.Set.SRandMember(key, -86);
        Assert.NotNull(rands);
        Assert.Equal(86, rands.Length);

        rands = redis.Set.SRandMember("fsdfsdfa_nbononone", 106);
        Assert.Null(rands);

        rands = redis.Set.SRandMember(key, 1);
        Assert.NotNull(rands);
        Assert.Single(rands);

        rands = redis.Set.SRandMember(key, -1);
        Assert.NotNull(rands);
        Assert.Single(rands);

        var randBytes = redis.Set.SRandMemberBytes(key);
        Assert.NotNull(randBytes);

        randBytes = redis.Set.SRandMemberBytes("not_rand_key");
        Assert.Null(randBytes);

        var randsBytes = redis.Set.SRandMemberBytes(key, 3);
        Assert.NotNull(randsBytes);
        Assert.Equal(3, randsBytes.Length);

        randsBytes = redis.Set.SRandMemberBytes(key, 30);
        Assert.NotNull(randsBytes);
        Assert.Equal(8, randsBytes.Length);

        randsBytes = redis.Set.SRandMemberBytes(key, -30);
        Assert.NotNull(randsBytes);
        Assert.Equal(30, randsBytes.Length);

        randsBytes = redis.Set.SRandMemberBytes("not_rand_key", -30);
        Assert.Null(randsBytes);

        randsBytes = redis.Set.SRandMemberBytes(key, 1);
        Assert.NotNull(randsBytes);
        Assert.Single(randsBytes);

        randsBytes = redis.Set.SRandMemberBytes(key, -1);
        Assert.NotNull(randsBytes);
        Assert.Single(randsBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SRandMemberAsync(Redis redis)
    {
        const string key = "set_srandmember_key_async";

        _ = await redis.Key.DelAsync(key);
        var result = await redis.Set.SAddAsync(key, ["a", "b", "c", "d", "ee", "fff", "gggg", "redis"]);
        Assert.Equal(8, result);

        var rand = await redis.Set.SRandMemberAsync(key);
        Assert.NotNull(rand);

        rand = await redis.Set.SRandMemberAsync("not_rand_key");
        Assert.Null(rand);

        var rands = await redis.Set.SRandMemberAsync(key, 8);
        Assert.NotNull(rands);
        Assert.Equal(8, rands.Length);

        rands = await redis.Set.SRandMemberAsync(key, 80);
        Assert.NotNull(rands);
        Assert.Equal(8, rands.Length);

        rands = await redis.Set.SRandMemberAsync(key, -4);
        Assert.NotNull(rands);
        Assert.Equal(4, rands.Length);

        rands = await redis.Set.SRandMemberAsync(key, -8);
        Assert.NotNull(rands);
        Assert.Equal(8, rands.Length);

        rands = await redis.Set.SRandMemberAsync(key, -86);
        Assert.NotNull(rands);
        Assert.Equal(86, rands.Length);

        rands = await redis.Set.SRandMemberAsync("fsdfsdfa_nbononone", 106);
        Assert.Null(rands);

        rands = await redis.Set.SRandMemberAsync(key, 1);
        Assert.NotNull(rands);
        Assert.Single(rands);

        rands = await redis.Set.SRandMemberAsync(key, -1);
        Assert.NotNull(rands);
        Assert.Single(rands);

        var randBytes = await redis.Set.SRandMemberBytesAsync(key);
        Assert.NotNull(randBytes);

        randBytes = await redis.Set.SRandMemberBytesAsync("not_rand_key");
        Assert.Null(randBytes);

        var randsBytes = await redis.Set.SRandMemberBytesAsync(key, 3);
        Assert.NotNull(randsBytes);
        Assert.Equal(3, randsBytes.Length);

        randsBytes = await redis.Set.SRandMemberBytesAsync(key, 30);
        Assert.NotNull(randsBytes);
        Assert.Equal(8, randsBytes.Length);

        randsBytes = await redis.Set.SRandMemberBytesAsync(key, -30);
        Assert.NotNull(randsBytes);
        Assert.Equal(30, randsBytes.Length);

        randsBytes = await redis.Set.SRandMemberBytesAsync("not_rand_key", -30);
        Assert.Null(randsBytes);

        randsBytes = await redis.Set.SRandMemberBytesAsync(key, 1);
        Assert.NotNull(randsBytes);
        Assert.Single(randsBytes);

        randsBytes = await redis.Set.SRandMemberBytesAsync(key, -1);
        Assert.NotNull(randsBytes);
        Assert.Single(randsBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SRem(Redis redis)
    {
        const string key = "set_srem_key";

        _ = redis.Key.Del(key);
        var result = redis.Set.SAdd(key, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        var remCount = redis.Set.SRem(key, "a");
        Assert.Equal(1, remCount);

        remCount = redis.Set.SRem(key, "aa");
        Assert.Equal(0, remCount);

        remCount = redis.Set.SRem(key, ["a", "b", "c"]);
        Assert.Equal(2, remCount);

        var count = redis.Set.SCard(key);
        Assert.Equal(1, count);

        remCount = redis.Set.SRem(key, Encoding.UTF8.GetBytes("a"));
        Assert.Equal(0, remCount);

        remCount = redis.Set.SRem(key, [Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("d"), Encoding.UTF8.GetBytes("b")]);
        Assert.Equal(1, remCount);

        count = redis.Set.SCard(key);
        Assert.Equal(0, count);

        remCount = redis.Set.SRem("nonononononono", "aa");
        Assert.Equal(0, remCount);

        remCount = redis.Set.SRem("nonononononono", Encoding.UTF8.GetBytes("a"));
        Assert.Equal(0, remCount);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SRemAsync(Redis redis)
    {
        const string key = "set_srem_key_async";

        _ = await redis.Key.DelAsync(key);
        var result = await redis.Set.SAddAsync(key, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        var remCount = await redis.Set.SRemAsync(key, "a");
        Assert.Equal(1, remCount);

        remCount = await redis.Set.SRemAsync(key, "aa");
        Assert.Equal(0, remCount);

        remCount = await redis.Set.SRemAsync(key, ["a", "b", "c"]);
        Assert.Equal(2, remCount);

        var count = await redis.Set.SCardAsync(key);
        Assert.Equal(1, count);

        remCount = await redis.Set.SRemAsync(key, Encoding.UTF8.GetBytes("a"));
        Assert.Equal(0, remCount);

        remCount = await redis.Set.SRemAsync(key, [Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("d"), Encoding.UTF8.GetBytes("b")]);
        Assert.Equal(1, remCount);

        count = await redis.Set.SCardAsync(key);
        Assert.Equal(0, count);

        remCount = await redis.Set.SRemAsync("nonononononono", "aa");
        Assert.Equal(0, remCount);

        remCount = await redis.Set.SRemAsync("nonononononono", Encoding.UTF8.GetBytes("a"));
        Assert.Equal(0, remCount);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SUnion(Redis redis)
    {
        const string key1 = "set_sunion_key1";
        const string key2 = "set_sunion_key2";
        const string key3 = "set_sunion_key3";
        _ = redis.Key.Del([key1, key2, key3]);
        var result = redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        result = redis.Set.SAdd(key2, "c");
        Assert.Equal(1, result);

        result = redis.Set.SAdd(key3, ["a", "c", "e"]);
        Assert.Equal(3, result);

        var union = redis.Set.SUnion(key1, key2);
        Assert.NotNull(union);
        Assert.Equal(4, union.Length);
        Assert.Equal<string[]>(["a", "b", "c", "d"], union);

        union = redis.Set.SUnion(key1, [key2, key3]);
        Assert.NotNull(union);
        Assert.Equal(5, union.Length);
        Assert.Equal<string[]>(["a", "b", "c", "d", "e"], union);

        union = redis.Set.SUnion([key1, key2, key3]);
        Assert.NotNull(union);
        Assert.Equal(5, union.Length);
        Assert.Equal<string[]>(["a", "b", "c", "d", "e"], union);

        union = redis.Set.SUnion(key1, "fasdfdsf_nonoe");
        Assert.NotNull(union);
        Assert.Equal<string[]>(["a", "b", "c", "d"], union);

        union = redis.Set.SUnion("faaaaaaaaaaa", "fasdfdsf_nonoe");
        Assert.Null(union);

        var unionBytes = redis.Set.SUnionBytes(key1, key2);
        Assert.NotNull(unionBytes);
        Assert.Equal(4, unionBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("c"), Encoding.UTF8.GetBytes("d")], unionBytes);

        unionBytes = redis.Set.SUnionBytes(key1, [key2, key3]);
        Assert.NotNull(unionBytes);
        Assert.Equal(5, unionBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("c"), Encoding.UTF8.GetBytes("d"), Encoding.UTF8.GetBytes("e")], unionBytes);

        unionBytes = redis.Set.SUnionBytes([key1, key2, key3]);
        Assert.NotNull(unionBytes);
        Assert.Equal(5, unionBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("c"), Encoding.UTF8.GetBytes("d"), Encoding.UTF8.GetBytes("e")], unionBytes);

        unionBytes = redis.Set.SUnionBytes("faaaaaaaaaaa", "fasdfdsf_nonoe");
        Assert.Null(unionBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SUnionAsync(Redis redis)
    {
        const string key1 = "set_sunion_key1_async";
        const string key2 = "set_sunion_key2_async";
        const string key3 = "set_sunion_key3_async";
        _ = await redis.Key.DelAsync([key1, key2, key3]);
        var result = await redis.Set.SAddAsync(key1, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        result = await redis.Set.SAddAsync(key2, "c");
        Assert.Equal(1, result);

        result = await redis.Set.SAddAsync(key3, ["a", "c", "e"]);
        Assert.Equal(3, result);

        var union = await redis.Set.SUnionAsync(key1, key2);
        Assert.NotNull(union);
        Assert.Equal(4, union.Length);
        Assert.Equal<string[]>(["a", "b", "c", "d"], union);

        union = await redis.Set.SUnionAsync(key1, [key2, key3]);
        Assert.NotNull(union);
        Assert.Equal(5, union.Length);
        Assert.Equal<string[]>(["a", "b", "c", "d", "e"], union);

        union = await redis.Set.SUnionAsync([key1, key2, key3]);
        Assert.NotNull(union);
        Assert.Equal(5, union.Length);
        Assert.Equal<string[]>(["a", "b", "c", "d", "e"], union);

        union = await redis.Set.SUnionAsync(key1, "fasdfdsf_nonoe");
        Assert.NotNull(union);
        Assert.Equal<string[]>(["a", "b", "c", "d"], union);

        union = await redis.Set.SUnionAsync("faaaaaaaaaaa", "fasdfdsf_nonoe");
        Assert.Null(union);

        var unionBytes = await redis.Set.SUnionBytesAsync(key1, key2);
        Assert.NotNull(unionBytes);
        Assert.Equal(4, unionBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("c"), Encoding.UTF8.GetBytes("d")], unionBytes);

        unionBytes = await redis.Set.SUnionBytesAsync(key1, [key2, key3]);
        Assert.NotNull(unionBytes);
        Assert.Equal(5, unionBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("c"), Encoding.UTF8.GetBytes("d"), Encoding.UTF8.GetBytes("e")], unionBytes);

        unionBytes = await redis.Set.SUnionBytesAsync([key1, key2, key3]);
        Assert.NotNull(unionBytes);
        Assert.Equal(5, unionBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("c"), Encoding.UTF8.GetBytes("d"), Encoding.UTF8.GetBytes("e")], unionBytes);

        unionBytes = await redis.Set.SUnionBytesAsync("faaaaaaaaaaa", "fasdfdsf_nonoe");
        Assert.Null(unionBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void SUnionStore(Redis redis)
    {
        const string key1 = "set_sunionstore_key1";
        const string key2 = "set_sunionstore_key2";
        const string key3 = "set_sunionstore_key3";
        const string destination = "set_sunionstore_dest";

        _ = redis.Key.Del([key1, key2, key3]);
        var result = redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        result = redis.Set.SAdd(key2, "c");
        Assert.Equal(1, result);

        result = redis.Set.SAdd(key3, ["a", "c", "e"]);
        Assert.Equal(3, result);

        var count = redis.Set.SUnionStore(destination, key1, key2);
        Assert.Equal(4, count);

        var destMembers = redis.Set.SMembers(destination);
        Assert.Equal<string[]>(["a", "b", "c", "d"], destMembers);

        count = redis.Set.SUnionStore(destination, key1, [key2, key3]);
        Assert.Equal(5, count);

        destMembers = redis.Set.SMembers(destination);
        Assert.Equal<string[]>(["a", "b", "c", "d", "e"], destMembers);

        count = redis.Set.SUnionStore(destination, [key1, key2, key3]);
        Assert.Equal(5, count);

        destMembers = redis.Set.SMembers(destination);
        Assert.Equal<string[]>(["a", "b", "c", "d", "e"], destMembers);

        count = redis.Set.SUnionStore(destination, "nonononon_set", "nonoe___set");
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task SUnionStoreAsync(Redis redis)
    {
        const string key1 = "set_sunionstore_key1_async";
        const string key2 = "set_sunionstore_key2_async";
        const string key3 = "set_sunionstore_key3_async";
        const string destination = "set_sunionstore_dest_async";

        _ = await redis.Key.DelAsync([key1, key2, key3]);
        var result = await redis.Set.SAddAsync(key1, ["a", "b", "c", "d"]);
        Assert.Equal(4, result);

        result = await redis.Set.SAddAsync(key2, "c");
        Assert.Equal(1, result);

        result = await redis.Set.SAddAsync(key3, ["a", "c", "e"]);
        Assert.Equal(3, result);

        var count = await redis.Set.SUnionStoreAsync(destination, key1, key2);
        Assert.Equal(4, count);

        var destMembers = await redis.Set.SMembersAsync(destination);
        Assert.Equal<string[]>(["a", "b", "c", "d"], destMembers);

        count = await redis.Set.SUnionStoreAsync(destination, key1, [key2, key3]);
        Assert.Equal(5, count);

        destMembers = await redis.Set.SMembersAsync(destination);
        Assert.Equal<string[]>(["a", "b", "c", "d", "e"], destMembers);

        count = await redis.Set.SUnionStoreAsync(destination, [key1, key2, key3]);
        Assert.Equal(5, count);

        destMembers = await redis.Set.SMembersAsync(destination);
        Assert.Equal<string[]>(["a", "b", "c", "d", "e"], destMembers);

        count = await redis.Set.SUnionStoreAsync(destination, "nonononon_set", "nonoe___set");
        Assert.Equal(0, count);
    }
}
