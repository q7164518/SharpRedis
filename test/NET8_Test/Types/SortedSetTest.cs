#pragma warning disable CA1861
using SharpRedis;
using System.Text;

namespace NET8_Test.Types;

public class SortedSetTest
{
    [Theory, ClassData(typeof(RedisProvider))]
    public void ZAdd(Redis redis)
    {
        const string key = "zset_key_test";
        const string member = "a";
        const int score = 100;
        _ = redis.Key.Del(key);
        var result = redis.SortedSet.ZAdd(key, member, score);
        Assert.Equal(1, result);

        result = redis.SortedSet.ZAdd(key, Encoding.UTF8.GetBytes("bytes"), 888);
        Assert.Equal(1, result);

        var multiResult = redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 5 },
        });
        Assert.Equal(3, multiResult);

        result = redis.SortedSet.ZAdd(key, member, score);
        Assert.Equal(0, result);

        multiResult = redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 5 },
        });
        Assert.Equal(0, multiResult);

        result = redis.SortedSet.ZAdd(key, member, 88, true);
        Assert.Equal(1, result);

        multiResult = redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 61 },
            { "e", -66.66 },
        }, true);
        Assert.Equal(2, multiResult);

        multiResult = redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 61 },
            { "e", -66.66 },
        }, true, NxXx.Nx);
        Assert.Equal(0, multiResult);

        multiResult = redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 61 },
            { "e", -66.66 },
            { "f", -68.66 },
            { "g", 20.35 },
        }, true, NxXx.Nx);
        Assert.Equal(2, multiResult);

        multiResult = redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 61 },
            { "e", -66.66 },
            { "f", -68.66 },
            { "g", 20.35 },
        }, true, NxXx.Xx);
        Assert.Equal(0, multiResult);

        multiResult = redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "b", 201.1 },
            { "c", 102.1 },
            { "d", 18 },
            { "e", -66.66 },
            { "f", -68.66 },
            { "g", 20.35 },
            { "h", 535 },
        }, true, NxXx.Xx);
        Assert.Equal(2, multiResult);

        multiResult = redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "b", 301.1 },
            { "c", 102.1 },
            { "d", 18 },
            { "e", -66.66 },
            { "f", -68.66 },
            { "g", 20.35 },
            { "h", 535 },
        }, false, NxXx.Xx);
        Assert.Equal(0, multiResult);

        result = redis.SortedSet.ZAdd(key, "h", 535);
        Assert.Equal(1, result);

        var scoreNv = redis.SortedSet.ZAddIncr(key, "h", 10);
        Assert.Equal(545, scoreNv);

        scoreNv = redis.SortedSet.ZAddIncr(key, "h", 10, NxXx.Nx);
        Assert.False(scoreNv.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZAddAsync(Redis redis)
    {
        const string key = "zset_key_test_async";
        const string member = "a";
        const int score = 100;
        _ = await redis.Key.DelAsync(key);
        var result = await redis.SortedSet.ZAddAsync(key, member, score);
        Assert.Equal(1, result);

        result = await redis.SortedSet.ZAddAsync(key, Encoding.UTF8.GetBytes("bytes"), 888);
        Assert.Equal(1, result);

        var multiResult = await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 5 },
        });
        Assert.Equal(3, multiResult);

        result = await redis.SortedSet.ZAddAsync(key, member, score);
        Assert.Equal(0, result);

        multiResult = await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 5 },
        });
        Assert.Equal(0, multiResult);

        result = await redis.SortedSet.ZAddAsync(key, member, 88, true);
        Assert.Equal(1, result);

        multiResult = await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 61 },
            { "e", -66.66 },
        }, true);
        Assert.Equal(2, multiResult);

        multiResult = await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 61 },
            { "e", -66.66 },
        }, true, NxXx.Nx);
        Assert.Equal(0, multiResult);

        multiResult = await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 61 },
            { "e", -66.66 },
            { "f", -68.66 },
            { "g", 20.35 },
        }, true, NxXx.Nx);
        Assert.Equal(2, multiResult);

        multiResult = await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "b", 101.1 },
            { "c", 102.1 },
            { "d", 61 },
            { "e", -66.66 },
            { "f", -68.66 },
            { "g", 20.35 },
        }, true, NxXx.Xx);
        Assert.Equal(0, multiResult);

        multiResult = await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "b", 201.1 },
            { "c", 102.1 },
            { "d", 18 },
            { "e", -66.66 },
            { "f", -68.66 },
            { "g", 20.35 },
            { "h", 535 },
        }, true, NxXx.Xx);
        Assert.Equal(2, multiResult);

        multiResult = await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "b", 301.1 },
            { "c", 102.1 },
            { "d", 18 },
            { "e", -66.66 },
            { "f", -68.66 },
            { "g", 20.35 },
            { "h", 535 },
        }, false, NxXx.Xx);
        Assert.Equal(0, multiResult);

        result = await redis.SortedSet.ZAddAsync(key, "h", 535);
        Assert.Equal(1, result);

        var scoreNv = await redis.SortedSet.ZAddIncrAsync(key, "h", 10);
        Assert.Equal(545, scoreNv);

        scoreNv = await redis.SortedSet.ZAddIncrAsync(key, "h", 10, NxXx.Nx);
        Assert.False(scoreNv.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZCount(Redis redis)
    {
        const string key = "zcount_key";
        _ = redis.Key.Del(key);
        var multiResult = redis.SortedSet.ZAdd(key, new Dictionary<string, int>
        {
            { "a", -2 },
            { "b", 0 },
            { "c", 1 },
            { "d", 2 },
            { "e", 10 },
        });
        Assert.Equal(5, multiResult);

        var count = redis.SortedSet.ZCount(key, 0, 6);
        Assert.Equal(3, count);

        count = redis.SortedSet.ZCount(key, 0, 6, false);
        Assert.Equal(2, count);

        count = redis.SortedSet.ZCount(key, 0, 10, false);
        Assert.Equal(3, count);

        count = redis.SortedSet.ZCount(key, 0, 10, false, false);
        Assert.Equal(2, count);

        count = redis.SortedSet.ZCount(key, 0, double.PositiveInfinity, false, false);
        Assert.Equal(3, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZCountAsync(Redis redis)
    {
        const string key = "zcount_key_async";
        _ = await redis.Key.DelAsync(key);
        var multiResult = await redis.SortedSet.ZAddAsync(key, new Dictionary<string, int>
        {
            { "a", -2 },
            { "b", 0 },
            { "c", 1 },
            { "d", 2 },
            { "e", 10 },
        });
        Assert.Equal(5, multiResult);

        var count = await redis.SortedSet.ZCountAsync(key, 0, 6);
        Assert.Equal(3, count);

        count = await redis.SortedSet.ZCountAsync(key, 0, 6, false);
        Assert.Equal(2, count);

        count = await redis.SortedSet.ZCountAsync(key, 0, 10, false);
        Assert.Equal(3, count);

        count = await redis.SortedSet.ZCountAsync(key, 0, 10, false, false);
        Assert.Equal(2, count);

        count = await redis.SortedSet.ZCountAsync(key, 0, double.PositiveInfinity, false, false);
        Assert.Equal(3, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZDiff(Redis redis)
    {
        _ = redis.Key.Del(["zset1", "zset2"]);
        redis.SortedSet.ZAdd("zset1", new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });

        redis.SortedSet.ZAdd("zset2", new Dictionary<string, int>
        {
            { "one", 1 },
        });

        var diffScore = redis.SortedSet.ZDiffWithScores("zset1", "zset2");

        Assert.Equal("two", diffScore![0]!.Member);
        Assert.Equal("three", diffScore![1]!.Member);
        Assert.Equal(2, diffScore![0]!.Score);
        Assert.Equal(3, diffScore![1]!.Score);

        var diffScoreBytes = redis.SortedSet.ZDiffWithScoresBytes("zset1", "zset2");
        Assert.Equal(Encoding.UTF8.GetBytes("two"), diffScoreBytes![0]!.Member);
        Assert.Equal(Encoding.UTF8.GetBytes("three"), diffScoreBytes![1]!.Member);
        Assert.Equal(2, diffScoreBytes![0]!.Score);
        Assert.Equal(3, diffScoreBytes![1]!.Score);

        _ = redis.Key.Del(["zset1", "zset2"]);

        redis.SortedSet.ZAdd("zset1", new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });

        redis.SortedSet.ZAdd("zset2", new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        });

        var diff = redis.SortedSet.ZDiff("zset1", "zset2");
        Assert.Equal(new string[] { "three" }, diff);

        var diffBytes = redis.SortedSet.ZDiffBytes("zset1", "zset2");
        Assert.Equal([Encoding.UTF8.GetBytes("three")], diffBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZDiffAsync(Redis redis)
    {
        _ = await redis.Key.DelAsync(["zset1_async", "zset2_async"]);
        await redis.SortedSet.ZAddAsync("zset1_async", new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });

        await redis.SortedSet.ZAddAsync("zset2_async", new Dictionary<string, int>
        {
            { "one", 1 },
        });

        var diffScore = await redis.SortedSet.ZDiffWithScoresAsync("zset1_async", "zset2_async");

        Assert.Equal("two", diffScore![0]!.Member);
        Assert.Equal("three", diffScore![1]!.Member);
        Assert.Equal(2, diffScore![0]!.Score);
        Assert.Equal(3, diffScore![1]!.Score);

        var diffScoreBytes = await redis.SortedSet.ZDiffWithScoresBytesAsync("zset1_async", "zset2_async");
        Assert.Equal(Encoding.UTF8.GetBytes("two"), diffScoreBytes![0]!.Member);
        Assert.Equal(Encoding.UTF8.GetBytes("three"), diffScoreBytes![1]!.Member);
        Assert.Equal(2, diffScoreBytes![0]!.Score);
        Assert.Equal(3, diffScoreBytes![1]!.Score);

        _ = await redis.Key.DelAsync(["zset1_async", "zset2_async"]);

        await redis.SortedSet.ZAddAsync("zset1_async", new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });

        await redis.SortedSet.ZAddAsync("zset2_async", new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        });

        var diff = await redis.SortedSet.ZDiffAsync("zset1_async", "zset2_async");
        Assert.Equal(new string[] { "three" }, diff);

        var diffBytes = await redis.SortedSet.ZDiffBytesAsync("zset1_async", "zset2_async");
        Assert.Equal([Encoding.UTF8.GetBytes("three")], diffBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZDiffStore(Redis redis)
    {
        const string destination = "diffstore_destination";
        const string key1 = "zset_diffstore1";
        const string key2 = "zset_diffstore2";

        _ = redis.Key.Del([key1, key2]);
        redis.SortedSet.ZAdd(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, int>
        {
            { "one", 1 },
        });

        var diffCount = redis.SortedSet.ZDiffStore(destination, key1, key2);
        Assert.Equal(2, diffCount);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZDiffStoreAsync(Redis redis)
    {
        const string destination = "diffstore_destination_async";
        const string key1 = "zset_diffstore1_async";
        const string key2 = "zset_diffstore2_async";

        _ = await redis.Key.DelAsync([key1, key2]);
        await redis.SortedSet.ZAddAsync(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });

        await redis.SortedSet.ZAddAsync(key2, new Dictionary<string, int>
        {
            { "one", 1 },
        });

        var diffCount = await redis.SortedSet.ZDiffStoreAsync(destination, key1, key2);
        Assert.Equal(2, diffCount);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZIncrBy(Redis redis)
    {
        const string key = "zset_incr_key";
        _ = redis.Key.Del(key);
        var result = redis.SortedSet.ZAdd(key, "a", 100);
        Assert.Equal(1, result);

        var incr = redis.SortedSet.ZIncrBy(key, 2, "a");
        Assert.Equal(102, incr);

        incr = redis.SortedSet.ZIncrBy(key, 2.6, "a");
        Assert.Equal(104.6D, incr);

        incr = redis.SortedSet.ZIncrBy(key, -100.1, "a");
        Assert.Equal(4.5D, incr);

        incr = redis.SortedSet.ZIncrBy(key, -100, "a");
        Assert.Equal(-95.5D, incr);

        incr = redis.SortedSet.ZIncrBy(key, 100, Encoding.UTF8.GetBytes("a"));
        Assert.Equal(4.5D, incr);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZIncrByAsync(Redis redis)
    {
        const string key = "zset_incr_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.SortedSet.ZAddAsync(key, "a", 100);
        Assert.Equal(1, result);

        var incr = await redis.SortedSet.ZIncrByAsync(key, 2, "a");
        Assert.Equal(102, incr);

        incr = await redis.SortedSet.ZIncrByAsync(key, 2.6, "a");
        Assert.Equal(104.6D, incr);

        incr = await redis.SortedSet.ZIncrByAsync(key, -100.1, "a");
        Assert.Equal(4.5D, incr);

        incr = await redis.SortedSet.ZIncrByAsync(key, -100, "a");
        Assert.Equal(-95.5D, incr);

        incr = await redis.SortedSet.ZIncrByAsync(key, 100, Encoding.UTF8.GetBytes("a"));
        Assert.Equal(4.5D, incr);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZInter(Redis redis)
    {
        const string key1 = "zset_inter1";
        const string key2 = "zset_inter2";

        _ = redis.Key.Del([key1, key2]);
        redis.SortedSet.ZAdd(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var inter = redis.SortedSet.ZInter(key1, key2);
        Assert.Equal<string?[]>(["one", "two"], inter);

        var interBytes = redis.SortedSet.ZInterBytes(key1, key2);
        Assert.Equal([Encoding.UTF8.GetBytes("one"), Encoding.UTF8.GetBytes("two")], interBytes);

        var interWithScore = redis.SortedSet.ZInterWithScores(key1, key2);
        Assert.Equal("one", interWithScore![0]!.Member);
        Assert.Equal("two", interWithScore![1]!.Member);
        Assert.Equal(11, interWithScore![0]!.Score);
        Assert.Equal(22, interWithScore![1]!.Score);

        interWithScore = redis.SortedSet.ZInterWithScores(key1, key2, Aggregate.Max);
        Assert.Equal("one", interWithScore![0]!.Member);
        Assert.Equal("two", interWithScore![1]!.Member);
        Assert.Equal(10, interWithScore![0]!.Score);
        Assert.Equal(20, interWithScore![1]!.Score);

        interWithScore = redis.SortedSet.ZInterWithScores(key1, key2, Aggregate.Min);
        Assert.Equal("one", interWithScore![0]!.Member);
        Assert.Equal("two", interWithScore![1]!.Member);
        Assert.Equal(1, interWithScore![0]!.Score);
        Assert.Equal(2, interWithScore![1]!.Score);

        var interWithScoreBytes = redis.SortedSet.ZInterWithScoresBytes(key1, key2);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), interWithScoreBytes![0]!.Member);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), interWithScoreBytes![1]!.Member);
        Assert.Equal(11, interWithScoreBytes![0]!.Score);
        Assert.Equal(22, interWithScoreBytes![1]!.Score);

        redis.SortedSet.ZAdd(key1, new Dictionary<string, double>
        {
            { "one", 1.5 },
            { "two", 2.6 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, double>
        {
            { "one", 10.5 },
            { "two", 20.22 },
            { "three", 30 },
        });

        var interWithScoreWeight = redis.SortedSet.ZInterWithScores(key1, 3, key2, 5);
        Assert.Equal("one", interWithScoreWeight![0]!.Member);
        Assert.Equal("two", interWithScoreWeight![1]!.Member);
        Assert.Equal(1.5 * 3 + 10.5 * 5, interWithScoreWeight![0]!.Score);
        Assert.Equal(2.6 * 3 + 20.22 * 5, interWithScoreWeight![1]!.Score);

        var interWithScoreWeightBytes = redis.SortedSet.ZInterWithScoresBytes(key1, 3, key2, 5);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), interWithScoreWeightBytes![0]!.Member);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), interWithScoreWeightBytes![1]!.Member);
        Assert.Equal(1.5 * 3 + 10.5 * 5, interWithScoreWeightBytes![0]!.Score);
        Assert.Equal(2.6 * 3 + 20.22 * 5, interWithScoreWeightBytes![1]!.Score);


        var interNot = redis.SortedSet.ZInter(key1, "jjjjjjjjjjj");
        Assert.Null(interNot);

        _ = redis.Key.Del([key1, key2]);
        redis.SortedSet.ZAdd(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, int>
        {
            { "one1", 10 },
            { "two1", 20 },
            { "three1", 30 },
        });

        var interNon = redis.SortedSet.ZInterWithScores(key1, key2);
        Assert.Null(interNon);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZInterAsync(Redis redis)
    {
        const string key1 = "zset_inter1_async";
        const string key2 = "zset_inter2_async";

        _ = await redis.Key.DelAsync([key1, key2]);
        await redis.SortedSet.ZAddAsync(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        });

        await redis.SortedSet.ZAddAsync(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var inter = await redis.SortedSet.ZInterAsync(key1, key2);
        Assert.Equal<string?[]>(["one", "two"], inter);

        var interBytes = await redis.SortedSet.ZInterBytesAsync(key1, key2);
        Assert.Equal([Encoding.UTF8.GetBytes("one"), Encoding.UTF8.GetBytes("two")], interBytes);

        var interWithScore = await redis.SortedSet.ZInterWithScoresAsync(key1, key2);
        Assert.Equal("one", interWithScore![0]!.Member);
        Assert.Equal("two", interWithScore![1]!.Member);
        Assert.Equal(11, interWithScore![0]!.Score);
        Assert.Equal(22, interWithScore![1]!.Score);

        interWithScore = await redis.SortedSet.ZInterWithScoresAsync(key1, key2, Aggregate.Max);
        Assert.Equal("one", interWithScore![0]!.Member);
        Assert.Equal("two", interWithScore![1]!.Member);
        Assert.Equal(10, interWithScore![0]!.Score);
        Assert.Equal(20, interWithScore![1]!.Score);

        interWithScore = await redis.SortedSet.ZInterWithScoresAsync(key1, key2, Aggregate.Min);
        Assert.Equal("one", interWithScore![0]!.Member);
        Assert.Equal("two", interWithScore![1]!.Member);
        Assert.Equal(1, interWithScore![0]!.Score);
        Assert.Equal(2, interWithScore![1]!.Score);

        var interWithScoreBytes = await redis.SortedSet.ZInterWithScoresBytesAsync(key1, key2);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), interWithScoreBytes![0]!.Member);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), interWithScoreBytes![1]!.Member);
        Assert.Equal(11, interWithScoreBytes![0]!.Score);
        Assert.Equal(22, interWithScoreBytes![1]!.Score);

        await redis.SortedSet.ZAddAsync(key1, new Dictionary<string, double>
        {
            { "one", 1.5 },
            { "two", 2.6 },
        });

        await redis.SortedSet.ZAddAsync(key2, new Dictionary<string, double>
        {
            { "one", 10.5 },
            { "two", 20.22 },
            { "three", 30 },
        });

        var interWithScoreWeight = await redis.SortedSet.ZInterWithScoresAsync(key1, 3, key2, 5);
        Assert.Equal("one", interWithScoreWeight![0]!.Member);
        Assert.Equal("two", interWithScoreWeight![1]!.Member);
        Assert.Equal(1.5 * 3 + 10.5 * 5, interWithScoreWeight![0]!.Score);
        Assert.Equal(2.6 * 3 + 20.22 * 5, interWithScoreWeight![1]!.Score);

        var interWithScoreWeightBytes = await redis.SortedSet.ZInterWithScoresBytesAsync(key1, 3, key2, 5);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), interWithScoreWeightBytes![0]!.Member);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), interWithScoreWeightBytes![1]!.Member);
        Assert.Equal(1.5 * 3 + 10.5 * 5, interWithScoreWeightBytes![0]!.Score);
        Assert.Equal(2.6 * 3 + 20.22 * 5, interWithScoreWeightBytes![1]!.Score);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZInterCard(Redis redis)
    {
        const string key1 = "zset_intercard1";
        const string key2 = "zset_intercard2";

        _ = redis.Key.Del([key1, key2]);
        redis.SortedSet.ZAdd(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var count = redis.SortedSet.ZInterCard(key1, key2);
        Assert.Equal(3, count);

        count = redis.SortedSet.ZInterCard(key1, key2, 2);
        Assert.Equal(2, count);

        count = redis.SortedSet.ZInterCard(key1, "fasdgfhfdhfdh");
        Assert.Equal(0, count);

        _ = redis.Key.Del([key1, key2]);

        redis.SortedSet.ZAdd(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, int>
        {
            { "one11", 10 },
            { "two11", 20 },
            { "three11", 30 },
        });
        count = redis.SortedSet.ZInterCard(key1, key2);
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZInterCardAsync(Redis redis)
    {
        const string key1 = "zset_intercard1_async";
        const string key2 = "zset_intercard2_async";

        _ = await redis.Key.DelAsync([key1, key2]);
        await redis.SortedSet.ZAddAsync(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        await redis.SortedSet.ZAddAsync(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var count = await redis.SortedSet.ZInterCardAsync(key1, key2);
        Assert.Equal(3, count);

        count = await redis.SortedSet.ZInterCardAsync(key1, key2, 2);
        Assert.Equal(2, count);

        count = await redis.SortedSet.ZInterCardAsync(key1, "fasdgfhfdhfdh");
        Assert.Equal(0, count);

        _ = await redis.Key.DelAsync([key1, key2]);

        await redis.SortedSet.ZAddAsync(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        await redis.SortedSet.ZAddAsync(key2, new Dictionary<string, int>
        {
            { "one11", 10 },
            { "two11", 20 },
            { "three11", 30 },
        });
        count = await redis.SortedSet.ZInterCardAsync(key1, key2);
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZInterStore(Redis redis)
    {
        const string key1 = "zset_interstore1";
        const string key2 = "zset_interstore2";
        const string destination = "zset_interstore_result";

        _ = redis.Key.Del([key1, key2]);
        redis.SortedSet.ZAdd(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var count = redis.SortedSet.ZInterStore(destination, key1, key2);
        Assert.Equal(3, count);

        count = redis.SortedSet.ZInterStore(destination, key1, "asdgfjd");
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZInterStoreAsync(Redis redis)
    {
        const string key1 = "zset_interstore1_async";
        const string key2 = "zset_interstore2_async";
        const string destination = "zset_interstore_result_async";

        _ = await redis.Key.DelAsync([key1, key2]);
        await redis.SortedSet.ZAddAsync(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        await redis.SortedSet.ZAddAsync(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var count = await redis.SortedSet.ZInterStoreAsync(destination, key1, key2);
        Assert.Equal(3, count);

        count = await redis.SortedSet.ZInterStoreAsync(destination, key1, "asdgfjd");
        Assert.Equal(0, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZLexCount(Redis redis)
    {
        const string key = "zlexcount_key";

        _ = redis.Key.Del(key);
        redis.SortedSet.ZAdd(key, new Dictionary<string, int>
        {
            { "a", 1 },
            { "b", 1 },
            { "c", 1 },
            { "d", 1 },
            { "e", 1 },
            { "f", 1 },
            { "g", 1 },
        });

        var count = redis.SortedSet.ZLexCount(key, "-", "+");
        Assert.Equal(7, count);

        count = redis.SortedSet.ZLexCount(key, "[b", "[f");
        Assert.Equal(5, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZLexCountAsync(Redis redis)
    {
        const string key = "zlexcount_key_async";

        _ = await redis.Key.DelAsync(key);
        await redis.SortedSet.ZAddAsync(key, new Dictionary<string, int>
        {
            { "a", 1 },
            { "b", 1 },
            { "c", 1 },
            { "d", 1 },
            { "e", 1 },
            { "f", 1 },
            { "g", 1 },
        });

        var count = await redis.SortedSet.ZLexCountAsync(key, "-", "+");
        Assert.Equal(7, count);

        count = await redis.SortedSet.ZLexCountAsync(key, "[b", "[f");
        Assert.Equal(5, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZMPop(Redis redis)
    {
        const string key1 = "zset_zmpop1";
        const string key2 = "zset_zmpop2";

        _ = redis.Key.Del([key1, key2]);
        redis.SortedSet.ZAdd(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var mpop = redis.SortedSet.ZMPop(key1, MaxMin.Max, 3);
        Assert.Equal(key1, mpop!.Value.Key);
        Assert.True(mpop.HasValue);
        Assert.Equal(3, mpop!.Value.Value!.Length);
        Assert.Equal("four", mpop!.Value.Value[0].Member);

        mpop = redis.SortedSet.ZMPop("fsdfds", MaxMin.Max, 3);
        Assert.False(mpop.HasValue);

        mpop = redis.SortedSet.ZMPop([key2, key1], MaxMin.Min, 2);
        Assert.Equal(key2, mpop!.Value.Key);
        Assert.True(mpop.HasValue);
        Assert.Equal(2, mpop!.Value.Value!.Length);
        Assert.Equal("one", mpop!.Value.Value[0].Member);
        Assert.Equal(10, mpop!.Value.Value[0].Score);

        var mpopBytes = redis.SortedSet.ZMPopBytes(key1, MaxMin.Max);
        Assert.True(mpopBytes.HasValue);
        Assert.Equal(key1, mpopBytes.Value.Key);
        Assert.Equal(1, mpopBytes!.Value.Value.Score);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), mpopBytes!.Value.Value.Member);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZMPopAsync(Redis redis)
    {
        const string key1 = "zset_zmpop1_async";
        const string key2 = "zset_zmpop2_async";

        _ = await redis.Key.DelAsync([key1, key2]);
        await redis.SortedSet.ZAddAsync(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        await redis.SortedSet.ZAddAsync(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var mpop = await redis.SortedSet.ZMPopAsync(key1, MaxMin.Max, 3);
        Assert.Equal(key1, mpop!.Value.Key);
        Assert.True(mpop.HasValue);
        Assert.Equal(3, mpop!.Value.Value!.Length);
        Assert.Equal("four", mpop!.Value.Value[0].Member);

        mpop = await redis.SortedSet.ZMPopAsync("fsdfds", MaxMin.Max, 3);
        Assert.False(mpop.HasValue);

        mpop = await redis.SortedSet.ZMPopAsync([key2, key1], MaxMin.Min, 2);
        Assert.Equal(key2, mpop!.Value.Key);
        Assert.True(mpop.HasValue);
        Assert.Equal(2, mpop!.Value.Value!.Length);
        Assert.Equal("one", mpop!.Value.Value[0].Member);
        Assert.Equal(10, mpop!.Value.Value[0].Score);

        var mpopBytes = redis.SortedSet.ZMPopBytes(key1, MaxMin.Max);
        Assert.True(mpopBytes.HasValue);
        Assert.Equal(key1, mpopBytes.Value.Key);
        Assert.Equal(1, mpopBytes!.Value.Value.Score);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), mpopBytes!.Value.Value.Member);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZMScore(Redis redis)
    {
        const string key = "zset_zmscore_key";
        _ = redis.Key.Del(key);
        redis.SortedSet.ZAdd(key, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        var score = redis.SortedSet.ZMScore(key, "one");
        Assert.True(score.HasValue);
        Assert.Equal(1, score);

        score = redis.SortedSet.ZMScore(key, Encoding.UTF8.GetBytes("three"));
        Assert.True(score.HasValue);
        Assert.Equal(3, score);

        score = redis.SortedSet.ZMScore(key, "one_none");
        Assert.False(score.HasValue);

        var scoreArray = redis.SortedSet.ZMScore(key, ["one", "two", "four"]);
        Assert.Equal(3, scoreArray!.Length);
        Assert.True(scoreArray![0]!.HasValue);
        Assert.True(scoreArray![1]!.HasValue);
        Assert.True(scoreArray![2]!.HasValue);
        Assert.Equal(1, scoreArray![0]!);
        Assert.Equal(2, scoreArray![1]!);
        Assert.Equal(4, scoreArray![2]!);

        scoreArray = redis.SortedSet.ZMScore(key, ["one", "two_none", "four_none"]);
        Assert.Equal(3, scoreArray!.Length);
        Assert.True(scoreArray![0]!.HasValue);
        Assert.False(scoreArray![1]!.HasValue);
        Assert.False(scoreArray![2]!.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZMScoreAsync(Redis redis)
    {
        const string key = "zset_zmscore_key_async";
        _ = await redis.Key.DelAsync(key);
        await redis.SortedSet.ZAddAsync(key, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        var score = await redis.SortedSet.ZMScoreAsync(key, "one");
        Assert.True(score.HasValue);
        Assert.Equal(1, score);

        score = await redis.SortedSet.ZMScoreAsync(key, Encoding.UTF8.GetBytes("three"));
        Assert.True(score.HasValue);
        Assert.Equal(3, score);

        score = await redis.SortedSet.ZMScoreAsync(key, "one_none");
        Assert.False(score.HasValue);

        var scoreArray = await redis.SortedSet.ZMScoreAsync(key, ["one", "two", "four"]);
        Assert.Equal(3, scoreArray!.Length);
        Assert.True(scoreArray![0]!.HasValue);
        Assert.True(scoreArray![1]!.HasValue);
        Assert.True(scoreArray![2]!.HasValue);
        Assert.Equal(1, scoreArray![0]!);
        Assert.Equal(2, scoreArray![1]!);
        Assert.Equal(4, scoreArray![2]!);

        scoreArray = await redis.SortedSet.ZMScoreAsync(key, ["one", "two_none", "four_none"]);
        Assert.Equal(3, scoreArray!.Length);
        Assert.True(scoreArray![0]!.HasValue);
        Assert.False(scoreArray![1]!.HasValue);
        Assert.False(scoreArray![2]!.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZPopMax(Redis redis)
    {
        const string key = "zset_zpopmax_key";

        _ = redis.Key.Del(key);
        redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "one", 10.33 },
            { "two", 2.66 },
            { "three", 333.66 },
            { "four", 4 },
        });

        var max = redis.SortedSet.ZPopMax(key);
        Assert.True(max.HasValue);
        Assert.Equal("three", max.Value.Member);
        Assert.Equal(333.66, max.Value.Score);

        max = redis.SortedSet.ZPopMax("fsdfdsfkkkkkk_none");
        Assert.False(max.HasValue);

        var maxArray = redis.SortedSet.ZPopMax(key, 10);
        Assert.NotNull(maxArray);
        Assert.Equal(3, maxArray.Length);
        Assert.Equal("one", maxArray[0]!.Member);

        maxArray = redis.SortedSet.ZPopMax(key, 10);
        Assert.Null(maxArray);

        redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "one", 10.33 },
            { "two", 2.66 },
            { "three", 333.66 },
            { "four", 4 },
        });

        var maxBytes = redis.SortedSet.ZPopMaxBytes(key);
        Assert.True(maxBytes.HasValue);
        Assert.Equal(Encoding.UTF8.GetBytes("three"), maxBytes.Value.Member);
        Assert.Equal(333.66, maxBytes.Value.Score);

        maxBytes = redis.SortedSet.ZPopMaxBytes("fsdfdsfkkkkkk_none");
        Assert.Null(maxBytes);

        var maxArrayBytes = redis.SortedSet.ZPopMaxBytes(key, 10);
        Assert.NotNull(maxArrayBytes);
        Assert.Equal(3, maxArrayBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), maxArrayBytes[0]!.Member);

        maxArrayBytes = redis.SortedSet.ZPopMaxBytes(key, 10);
        Assert.Null(maxArrayBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZPopMaxAsync(Redis redis)
    {
        const string key = "zset_zpopmax_key_async";

        _ = await redis.Key.DelAsync(key);
        await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "one", 10.33 },
            { "two", 2.66 },
            { "three", 333.66 },
            { "four", 4 },
        });

        var max = await redis.SortedSet.ZPopMaxAsync(key);
        Assert.True(max.HasValue);
        Assert.Equal("three", max.Value.Member);
        Assert.Equal(333.66, max.Value.Score);

        max = await redis.SortedSet.ZPopMaxAsync("fsdfdsfkkkkkk_none");
        Assert.Null(max);

        var maxArray = await redis.SortedSet.ZPopMaxAsync(key, 10);
        Assert.NotNull(maxArray);
        Assert.Equal(3, maxArray.Length);
        Assert.Equal("one", maxArray[0]!.Member);

        maxArray = await redis.SortedSet.ZPopMaxAsync(key, 10);
        Assert.Null(maxArray);

        await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "one", 10.33 },
            { "two", 2.66 },
            { "three", 333.66 },
            { "four", 4 },
        });

        var maxBytes = await redis.SortedSet.ZPopMaxBytesAsync(key);
        Assert.True(maxBytes.HasValue);
        Assert.Equal(Encoding.UTF8.GetBytes("three"), maxBytes.Value.Member);
        Assert.Equal(333.66, maxBytes.Value.Score);

        maxBytes = await redis.SortedSet.ZPopMaxBytesAsync("fsdfdsfkkkkkk_none");
        Assert.Null(maxBytes);

        var maxArrayBytes = await redis.SortedSet.ZPopMaxBytesAsync(key, 10);
        Assert.NotNull(maxArrayBytes);
        Assert.Equal(3, maxArrayBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), maxArrayBytes[0]!.Member);

        maxArrayBytes = await redis.SortedSet.ZPopMaxBytesAsync(key, 10);
        Assert.Null(maxArrayBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZPopMin(Redis redis)
    {
        const string key = "zset_zpopmin_key";

        _ = redis.Key.Del(key);
        redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "one", 10.33 },
            { "two", 2.66 },
            { "three", 333.66 },
            { "four", 4 },
        });

        var min = redis.SortedSet.ZPopMin(key);
        Assert.True(min.HasValue);
        Assert.Equal("two", min.Value.Member);
        Assert.Equal(2.66, min.Value.Score);

        min = redis.SortedSet.ZPopMin("fsdfdsfkkkkkk_none");
        Assert.Null(min);

        var minArray = redis.SortedSet.ZPopMin(key, 10);
        Assert.NotNull(minArray);
        Assert.Equal(3, minArray.Length);
        Assert.Equal("four", minArray[0]!.Member);
        Assert.Equal(4, minArray[0]!.Score);

        minArray = redis.SortedSet.ZPopMin(key, 10);
        Assert.Null(minArray);

        redis.SortedSet.ZAdd(key, new Dictionary<string, double>
        {
            { "one", 10.33 },
            { "two", 2.66 },
            { "three", 333.66 },
            { "four", 4 },
        });

        var minBytes = redis.SortedSet.ZPopMinBytes(key);
        Assert.True(minBytes.HasValue);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), minBytes.Value.Member);
        Assert.Equal(2.66, minBytes.Value.Score);

        minBytes = redis.SortedSet.ZPopMinBytes("fsdfdsfkkkkkk_none");
        Assert.Null(minBytes);

        var minArrayBytes = redis.SortedSet.ZPopMinBytes(key, 10);
        Assert.NotNull(minArrayBytes);
        Assert.Equal(3, minArrayBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("four"), minArrayBytes[0]!.Member);

        minArrayBytes = redis.SortedSet.ZPopMinBytes(key, 10);
        Assert.Null(minArrayBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZPopMinAsync(Redis redis)
    {
        const string key = "zset_zpopmin_key_async";

        _ = await redis.Key.DelAsync(key);
        await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "one", 10.33 },
            { "two", 2.66 },
            { "three", 333.66 },
            { "four", 4 },
        });

        var min = await redis.SortedSet.ZPopMinAsync(key);
        Assert.NotNull(min);
        Assert.Equal("two", min.Value.Member);
        Assert.Equal(2.66, min.Value.Score);

        min = await redis.SortedSet.ZPopMinAsync("fsdfdsfkkkkkk_none");
        Assert.Null(min);

        var minArray = await redis.SortedSet.ZPopMinAsync(key, 10);
        Assert.NotNull(minArray);
        Assert.Equal(3, minArray.Length);
        Assert.Equal("four", minArray[0]!.Member);
        Assert.Equal(4, minArray[0]!.Score);

        minArray = await redis.SortedSet.ZPopMinAsync(key, 10);
        Assert.Null(minArray);

        await redis.SortedSet.ZAddAsync(key, new Dictionary<string, double>
        {
            { "one", 10.33 },
            { "two", 2.66 },
            { "three", 333.66 },
            { "four", 4 },
        });

        var minBytes = await redis.SortedSet.ZPopMinBytesAsync(key);
        Assert.NotNull(minBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), minBytes.Value.Member);
        Assert.Equal(2.66, minBytes.Value.Score);

        minBytes = await redis.SortedSet.ZPopMinBytesAsync("fsdfdsfkkkkkk_none");
        Assert.Null(minBytes);

        var minArrayBytes = await redis.SortedSet.ZPopMinBytesAsync(key, 10);
        Assert.NotNull(minArrayBytes);
        Assert.Equal(3, minArrayBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("four"), minArrayBytes[0]!.Member);

        minArrayBytes = await redis.SortedSet.ZPopMinBytesAsync(key, 10);
        Assert.Null(minArrayBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRandMember(Redis redis)
    {
        const string key = "zset_zrandmember_key";
        var members = new Dictionary<string, double>
        {
            { "one", 10.33 },
            { "two", 2.66 },
            { "three", 333.66 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6.6666 },
        };
        redis.SortedSet.ZAdd(key, members);

        var randMember = redis.SortedSet.ZRandMember(key);
        Assert.False(string.IsNullOrEmpty(randMember));
        randMember = redis.SortedSet.ZRandMember("flllll_none_ffff");
        Assert.Null(randMember);

        var randMemberBytes = redis.SortedSet.ZRandMemberBytes(key);
        Assert.NotNull(randMemberBytes);
        Assert.True(members.ContainsKey(Encoding.UTF8.GetString(randMemberBytes)));

        var randMemberArray = redis.SortedSet.ZRandMember(key, 3);
        Assert.NotNull(randMemberArray);
        Assert.Equal(3, randMemberArray.Length);

        var randMemberArrayBytes = redis.SortedSet.ZRandMemberBytes(key, 3);
        Assert.NotNull(randMemberArrayBytes);

        randMemberArray = redis.SortedSet.ZRandMember(key, 15);
        Assert.NotNull(randMemberArray);
        Assert.Equal(6, randMemberArray.Length);

        randMemberArray = redis.SortedSet.ZRandMember(key, -15);
        Assert.NotNull(randMemberArray);
        Assert.Equal(15, randMemberArray.Length);

        var randMemberScore = redis.SortedSet.ZRandMemberWithScores(key);
        Assert.NotNull(randMemberScore);
        Assert.True(members.ContainsKey(randMemberScore.Value.Member));
        Assert.Equal(members[randMemberScore.Value.Member], randMemberScore.Value.Score);
        randMemberScore = redis.SortedSet.ZRandMemberWithScores("flllll_none_ffff");
        Assert.Null(randMemberScore);

        var randMemberScoreArray = redis.SortedSet.ZRandMemberWithScores(key, 2);
        Assert.NotNull(randMemberScoreArray);
        Assert.Equal(2, randMemberScoreArray.Length);

        var randMemberScoreBytes = redis.SortedSet.ZRandMemberWithScoresBytes(key);
        Assert.NotNull(randMemberScoreBytes);
        Assert.True(members.ContainsKey(Encoding.UTF8.GetString(randMemberScoreBytes.Value.Member)));
        Assert.Equal(members[Encoding.UTF8.GetString(randMemberScoreBytes.Value.Member)], randMemberScoreBytes.Value.Score);

        var randMemberScoreArrayBytes = redis.SortedSet.ZRandMemberWithScoresBytes(key, -20);
        Assert.NotNull(randMemberScoreArrayBytes);
        Assert.Equal(20, randMemberScoreArrayBytes.Length);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRandMemberAsync(Redis redis)
    {
        const string key = "zset_zrandmember_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 10.33 },
            { "two", 2.66 },
            { "three", 333.66 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6.6666 },
        };
        await redis.SortedSet.ZAddAsync(key, members);

        var randMember = await redis.SortedSet.ZRandMemberAsync(key);
        Assert.False(string.IsNullOrEmpty(randMember));
        randMember = await redis.SortedSet.ZRandMemberAsync("flllll_none_ffff");
        Assert.Null(randMember);

        var randMemberBytes = await redis.SortedSet.ZRandMemberBytesAsync(key);
        Assert.NotNull(randMemberBytes);
        Assert.True(members.ContainsKey(Encoding.UTF8.GetString(randMemberBytes)));

        var randMemberArray = await redis.SortedSet.ZRandMemberAsync(key, 3);
        Assert.NotNull(randMemberArray);
        Assert.Equal(3, randMemberArray.Length);

        var randMemberArrayBytes = await redis.SortedSet.ZRandMemberBytesAsync(key, 3);
        Assert.NotNull(randMemberArrayBytes);

        randMemberArray = await redis.SortedSet.ZRandMemberAsync(key, 15);
        Assert.NotNull(randMemberArray);
        Assert.Equal(6, randMemberArray.Length);

        randMemberArray = await redis.SortedSet.ZRandMemberAsync(key, -15);
        Assert.NotNull(randMemberArray);
        Assert.Equal(15, randMemberArray.Length);

        var randMemberScore = await redis.SortedSet.ZRandMemberWithScoresAsync(key);
        Assert.NotNull(randMemberScore);
        Assert.True(members.ContainsKey(randMemberScore.Value.Member));
        Assert.Equal(members[randMemberScore.Value.Member], randMemberScore.Value.Score);
        randMemberScore = await redis.SortedSet.ZRandMemberWithScoresAsync("flllll_none_ffff");
        Assert.Null(randMemberScore);

        var randMemberScoreArray = await redis.SortedSet.ZRandMemberWithScoresAsync(key, 2);
        Assert.NotNull(randMemberScoreArray);
        Assert.Equal(2, randMemberScoreArray.Length);

        var randMemberScoreBytes = await redis.SortedSet.ZRandMemberWithScoresBytesAsync(key);
        Assert.NotNull(randMemberScoreBytes);
        Assert.True(members.ContainsKey(Encoding.UTF8.GetString(randMemberScoreBytes.Value.Member)));
        Assert.Equal(members[Encoding.UTF8.GetString(randMemberScoreBytes.Value.Member)], randMemberScoreBytes.Value.Score);

        var randMemberScoreArrayBytes = await redis.SortedSet.ZRandMemberWithScoresBytesAsync(key, -20);
        Assert.NotNull(randMemberScoreArrayBytes);
        Assert.Equal(20, randMemberScoreArrayBytes.Length);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRange(Redis redis)
    {
        const string key = "zset_zrange_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var range = redis.SortedSet.ZRange(key, 2, -1);
        Assert.NotNull(range);
        Assert.Equal(8, range.Length);
        Assert.Equal("three", range[0]);

        range = redis.SortedSet.ZRange(key, 2, -1, true);
        Assert.NotNull(range);
        Assert.Equal(8, range.Length);
        Assert.Equal("eight", range[0]);

        range = redis.SortedSet.ZRange(ZRangeBy.ByScore, key, 2, 7);
        Assert.NotNull(range);
        Assert.Equal(6, range.Length);
        Assert.Equal("two", range[0]);

        range = redis.SortedSet.ZRange(ZRangeBy.ByScore, key, 2, 7, 2, 2);
        Assert.NotNull(range);
        Assert.Equal(2, range.Length);
        Assert.Equal("four", range[0]);
        Assert.Equal("five", range[1]);

        range = redis.SortedSet.ZRange(ZRangeBy.ByScore, key, 7, 2, true);
        Assert.NotNull(range);
        Assert.Equal(6, range.Length);
        Assert.Equal("seven", range[0]);

        range = redis.SortedSet.ZRange(ZRangeBy.ByLex, key, "-", "+");
        Assert.NotNull(range);
        Assert.Equal(10, range.Length);
        Assert.Equal("one", range[0]);

        range = redis.SortedSet.ZRange(ZRangeBy.ByLex, key, "+", "-", true);
        Assert.NotNull(range);
        Assert.Equal(10, range.Length);
        Assert.Equal("ten", range[0]);

        var rangeBytes = redis.SortedSet.ZRangeBytes(key, 0, -2);
        Assert.NotNull(rangeBytes);
        Assert.Equal(9, rangeBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("nine"), rangeBytes[^1]);

        rangeBytes = redis.SortedSet.ZRangeBytes(key, 0, -2, true);
        Assert.NotNull(rangeBytes);
        Assert.Equal(9, rangeBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), rangeBytes[^1]);

        var rangeScore = redis.SortedSet.ZRangeWithScores(key, 2, -1);
        Assert.NotNull(rangeScore);
        Assert.Equal(8, rangeScore.Length);
        Assert.Equal("three", rangeScore[0].Member);

        var rangeScoreBytes = redis.SortedSet.ZRangeWithScoresBytes(key, 0, -2);
        Assert.NotNull(rangeScoreBytes);
        Assert.Equal(9, rangeScoreBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("nine"), rangeScoreBytes[^1].Member);

        rangeScore = redis.SortedSet.ZRangeWithScores(ZRangeBy.ByScore, key, 2, 7);
        Assert.NotNull(rangeScore);
        Assert.Equal(6, rangeScore.Length);
        Assert.Equal("two", rangeScore[0].Member);
        Assert.Equal(2, rangeScore[0].Score);

        rangeScore = redis.SortedSet.ZRangeWithScores(ZRangeBy.ByScore, "fsdagfdsagfd_none", 2, 7);
        Assert.Null(rangeScore);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRangeAsync(Redis redis)
    {
        const string key = "zset_zrange_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var range = await redis.SortedSet.ZRangeAsync(key, 2, -1);
        Assert.NotNull(range);
        Assert.Equal(8, range.Length);
        Assert.Equal("three", range[0]);

        range = await redis.SortedSet.ZRangeAsync(key, 2, -1, true);
        Assert.NotNull(range);
        Assert.Equal(8, range.Length);
        Assert.Equal("eight", range[0]);

        range = await redis.SortedSet.ZRangeAsync(ZRangeBy.ByScore, key, 2, 7);
        Assert.NotNull(range);
        Assert.Equal(6, range.Length);
        Assert.Equal("two", range[0]);

        range = await redis.SortedSet.ZRangeAsync(ZRangeBy.ByScore, key, 2, 7, 2, 2);
        Assert.NotNull(range);
        Assert.Equal(2, range.Length);
        Assert.Equal("four", range[0]);
        Assert.Equal("five", range[1]);

        range = await redis.SortedSet.ZRangeAsync(ZRangeBy.ByScore, key, 7, 2, true);
        Assert.NotNull(range);
        Assert.Equal(6, range.Length);
        Assert.Equal("seven", range[0]);

        range = await redis.SortedSet.ZRangeAsync(ZRangeBy.ByLex, key, "-", "+");
        Assert.NotNull(range);
        Assert.Equal(10, range.Length);
        Assert.Equal("one", range[0]);

        range = await redis.SortedSet.ZRangeAsync(ZRangeBy.ByLex, key, "+", "-", true);
        Assert.NotNull(range);
        Assert.Equal(10, range.Length);
        Assert.Equal("ten", range[0]);

        var rangeBytes = await redis.SortedSet.ZRangeBytesAsync(key, 0, -2);
        Assert.NotNull(rangeBytes);
        Assert.Equal(9, rangeBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("nine"), rangeBytes[^1]);

        rangeBytes = await redis.SortedSet.ZRangeBytesAsync(key, 0, -2, true);
        Assert.NotNull(rangeBytes);
        Assert.Equal(9, rangeBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), rangeBytes[^1]);

        var rangeScore = await redis.SortedSet.ZRangeWithScoresAsync(key, 2, -1);
        Assert.NotNull(rangeScore);
        Assert.Equal(8, rangeScore.Length);
        Assert.Equal("three", rangeScore[0].Member);

        var rangeScoreBytes = await redis.SortedSet.ZRangeWithScoresBytesAsync(key, 0, -2);
        Assert.NotNull(rangeScoreBytes);
        Assert.Equal(9, rangeScoreBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("nine"), rangeScoreBytes[^1].Member);

        rangeScore = await redis.SortedSet.ZRangeWithScoresAsync(ZRangeBy.ByScore, key, 2, 7);
        Assert.NotNull(rangeScore);
        Assert.Equal(6, rangeScore.Length);
        Assert.Equal("two", rangeScore[0].Member);
        Assert.Equal(2, rangeScore[0].Score);

        rangeScore = await redis.SortedSet.ZRangeWithScoresAsync(ZRangeBy.ByScore, "fsdagfdsagfd_none", 2, 7);
        Assert.Null(rangeScore);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRangeByLex(Redis redis)
    {
        const string key = "zset_zrangebylex_key";
        _ = redis.Key.Del(key);
        _ = redis.SortedSet.ZAdd(key, new Dictionary<string, int>
        {
            { "a", 0 },
            { "b", 0 },
            { "c", 0 },
            { "d", 0 },
            { "e", 0 },
            { "f", 0 },
            { "g", 0 },
        });

        var rangeLex = redis.SortedSet.ZRangeByLex(key, "-", "[c");
        Assert.NotNull(rangeLex);
        Assert.Equal(new string[] { "a", "b", "c" }, rangeLex);

        rangeLex = redis.SortedSet.ZRangeByLex(key, "[aaa", "(g");
        Assert.NotNull(rangeLex);
        Assert.Equal(new string[] { "b", "c", "d", "e", "f" }, rangeLex);

        rangeLex = redis.SortedSet.ZRangeByLex(key, "[aaa", "(g", 1, 3);
        Assert.NotNull(rangeLex);
        Assert.Equal(3, rangeLex.Length);
        Assert.Equal(new string[] { "c", "d", "e" }, rangeLex);

        var rangeLexBytes = redis.SortedSet.ZRangeByLexBytes(key, "-", "[c");
        Assert.NotNull(rangeLexBytes);
        Assert.Equal(3, rangeLexBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("c")], rangeLexBytes);

        rangeLexBytes = redis.SortedSet.ZRangeByLexBytes(key, "[aaa", "(g", 1, 3);
        Assert.NotNull(rangeLexBytes);
        Assert.Equal(3, rangeLexBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("c"), Encoding.UTF8.GetBytes("d"), Encoding.UTF8.GetBytes("e")], rangeLexBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRangeByLexAsync(Redis redis)
    {
        const string key = "zset_zrangebylex_key_async";
        _ = await redis.Key.DelAsync(key);
        _ = await redis.SortedSet.ZAddAsync(key, new Dictionary<string, int>
        {
            { "a", 0 },
            { "b", 0 },
            { "c", 0 },
            { "d", 0 },
            { "e", 0 },
            { "f", 0 },
            { "g", 0 },
        });

        var rangeLex = await redis.SortedSet.ZRangeByLexAsync(key, "-", "[c");
        Assert.NotNull(rangeLex);
        Assert.Equal(new string[] { "a", "b", "c" }, rangeLex);

        rangeLex = await redis.SortedSet.ZRangeByLexAsync(key, "[aaa", "(g");
        Assert.NotNull(rangeLex);
        Assert.Equal(new string[] { "b", "c", "d", "e", "f" }, rangeLex);

        rangeLex = await redis.SortedSet.ZRangeByLexAsync(key, "[aaa", "(g", 1, 3);
        Assert.NotNull(rangeLex);
        Assert.Equal(3, rangeLex.Length);
        Assert.Equal(new string[] { "c", "d", "e" }, rangeLex);

        var rangeLexBytes = await redis.SortedSet.ZRangeByLexBytesAsync(key, "-", "[c");
        Assert.NotNull(rangeLexBytes);
        Assert.Equal(3, rangeLexBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("c")], rangeLexBytes);

        rangeLexBytes = await redis.SortedSet.ZRangeByLexBytesAsync(key, "[aaa", "(g", 1, 3);
        Assert.NotNull(rangeLexBytes);
        Assert.Equal(3, rangeLexBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("c"), Encoding.UTF8.GetBytes("d"), Encoding.UTF8.GetBytes("e")], rangeLexBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRangeByScore(Redis redis)
    {
        const string key = "zset_zrangebyscore_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var range = redis.SortedSet.ZRangeByScore(key, 0, 100);
        Assert.NotNull(range);
        Assert.Equal(10, range.Length);
        Assert.Equal("two", range[1]);

        range = redis.SortedSet.ZRangeByScore(key, 1, 5);
        Assert.NotNull(range);
        Assert.Equal(5, range.Length);
        Assert.Equal("five", range[^1]);

        range = redis.SortedSet.ZRangeByScore(key, 1, 5, false, false);
        Assert.NotNull(range);
        Assert.Equal(3, range.Length);
        Assert.Equal("four", range[^1]);

        range = redis.SortedSet.ZRangeByScore(key, 0, 100, 3, 5);
        Assert.NotNull(range);
        Assert.Equal(5, range.Length);
        Assert.Equal("five", range[1]);
        Assert.Equal("eight", range[^1]);

        var rangeBytes = redis.SortedSet.ZRangeByScoreBytes(key, 1, 5);
        Assert.NotNull(rangeBytes);
        Assert.Equal(5, rangeBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("four"), rangeBytes[^2]);

        rangeBytes = redis.SortedSet.ZRangeByScoreBytes(key, 0, 100, 3, 5);
        Assert.NotNull(rangeBytes);
        Assert.Equal(5, rangeBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("four"), rangeBytes[0]);
        Assert.Equal(Encoding.UTF8.GetBytes("eight"), rangeBytes[^1]);

        var rangScore = redis.SortedSet.ZRangeByScoreWithScores(key, 0, 100);
        Assert.NotNull(rangScore);
        Assert.Equal(10, rangScore.Length);
        Assert.Equal("two", rangScore[1].Member);
        Assert.Equal(2, rangScore[1].Score);

        var rangScoreBytes = redis.SortedSet.ZRangeByScoreWithScoresBytes(key, 0, 100, 2, 6);
        Assert.NotNull(rangScoreBytes);
        Assert.Equal(6, rangScoreBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("four"), rangScoreBytes[1].Member);
        Assert.Equal(4, rangScoreBytes[1].Score);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRangeByScoreAsync(Redis redis)
    {
        const string key = "zset_zrangebyscore_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var range = await redis.SortedSet.ZRangeByScoreAsync(key, 0, 100);
        Assert.NotNull(range);
        Assert.Equal(10, range.Length);
        Assert.Equal("two", range[1]);

        range = await redis.SortedSet.ZRangeByScoreAsync(key, 1, 5);
        Assert.NotNull(range);
        Assert.Equal(5, range.Length);
        Assert.Equal("five", range[^1]);

        range = await redis.SortedSet.ZRangeByScoreAsync(key, 1, 5, false, false);
        Assert.NotNull(range);
        Assert.Equal(3, range.Length);
        Assert.Equal("four", range[^1]);

        range = await redis.SortedSet.ZRangeByScoreAsync(key, 0, 100, 3, 5);
        Assert.NotNull(range);
        Assert.Equal(5, range.Length);
        Assert.Equal("five", range[1]);
        Assert.Equal("eight", range[^1]);

        var rangeBytes = await redis.SortedSet.ZRangeByScoreBytesAsync(key, 1, 5);
        Assert.NotNull(rangeBytes);
        Assert.Equal(5, rangeBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("four"), rangeBytes[^2]);

        rangeBytes = await redis.SortedSet.ZRangeByScoreBytesAsync(key, 0, 100, 3, 5);
        Assert.NotNull(rangeBytes);
        Assert.Equal(5, rangeBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("four"), rangeBytes[0]);
        Assert.Equal(Encoding.UTF8.GetBytes("eight"), rangeBytes[^1]);

        var rangScore = await redis.SortedSet.ZRangeByScoreWithScoresAsync(key, 0, 100);
        Assert.NotNull(rangScore);
        Assert.Equal(10, rangScore.Length);
        Assert.Equal("two", rangScore[1].Member);
        Assert.Equal(2, rangScore[1].Score);

        var rangScoreBytes = await redis.SortedSet.ZRangeByScoreWithScoresBytesAsync(key, 0, 100, 2, 6);
        Assert.NotNull(rangScoreBytes);
        Assert.Equal(6, rangScoreBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("four"), rangScoreBytes[1].Member);
        Assert.Equal(4, rangScoreBytes[1].Score);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRangeStore(Redis redis)
    {
        const string destination = "zset_zrangestore_destination";
        const string key = "zset_zrangestore_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var count = redis.SortedSet.ZRangeStore(destination, key, 3, -2);
        Assert.Equal(6, count);

        count = redis.SortedSet.ZRangeStore(ZRangeBy.ByScore, destination, key, 4, 8, false, false, true);
        Assert.Equal(4, count);

        count = redis.SortedSet.ZRangeStore(ZRangeBy.ByScore, destination, key, 1, 8, 1, 4);
        Assert.Equal(4, count);

        var range = redis.SortedSet.ZRange(destination, 0, -1);
        Assert.NotNull(range);
        Assert.Equal(4, range.Length);
        Assert.Equal("five", range[3]);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRangeStoreAsync(Redis redis)
    {
        const string destination = "zset_zrangestore_destination_async";
        const string key = "zset_zrangestore_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var count = await redis.SortedSet.ZRangeStoreAsync(destination, key, 3, -2);
        Assert.Equal(6, count);

        count = await redis.SortedSet.ZRangeStoreAsync(ZRangeBy.ByScore, destination, key, 4, 8, false, false, true);
        Assert.Equal(4, count);

        count = await redis.SortedSet.ZRangeStoreAsync(ZRangeBy.ByScore, destination, key, 1, 8, 1, 4);
        Assert.Equal(4, count);

        var range = await redis.SortedSet.ZRangeAsync(destination, 0, -1);
        Assert.NotNull(range);
        Assert.Equal(4, range.Length);
        Assert.Equal("five", range[3]);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRank(Redis redis)
    {
        const string key = "zset_zrank_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var rank = redis.SortedSet.ZRank(key, "one");
        Assert.Equal(0, rank);

        rank = redis.SortedSet.ZRank(key, "eight");
        Assert.Equal(7, rank);

        rank = redis.SortedSet.ZRank(key, Encoding.UTF8.GetBytes("nine"));
        Assert.Equal(8, rank);

        rank = redis.SortedSet.ZRank(key, "one_none");
        Assert.False(rank.HasValue);

        rank = redis.SortedSet.ZRank(key, Encoding.UTF8.GetBytes("nine_none"));
        Assert.False(rank.HasValue);

        var scoreRank = redis.SortedSet.ZRankWithScore(key, "six");
        Assert.True(scoreRank.HasValue);
        Assert.Equal(5, scoreRank.Value.Rank);
        Assert.Equal(6, scoreRank.Value.Score);

        scoreRank = redis.SortedSet.ZRankWithScore(key, Encoding.UTF8.GetBytes("four"));
        Assert.True(scoreRank.HasValue);
        Assert.Equal(3, scoreRank.Value.Rank);
        Assert.Equal(4, scoreRank.Value.Score);

        scoreRank = redis.SortedSet.ZRankWithScore(key, "six_none");
        Assert.False(scoreRank.HasValue);

        scoreRank = redis.SortedSet.ZRankWithScore(key, Encoding.UTF8.GetBytes("six_none"));
        Assert.False(scoreRank.HasValue);

        scoreRank = redis.SortedSet.ZRankWithScore("none_zrank_fasfdf", "six");
        Assert.False(scoreRank.HasValue);

        scoreRank = redis.SortedSet.ZRankWithScore("none_zrank_fasfdf", Encoding.UTF8.GetBytes("four"));
        Assert.False(scoreRank.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRankAsync(Redis redis)
    {
        const string key = "zset_zrank_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var rank = await redis.SortedSet.ZRankAsync(key, "one");
        Assert.Equal(0, rank);

        rank = await redis.SortedSet.ZRankAsync(key, "eight");
        Assert.Equal(7, rank);

        rank = await redis.SortedSet.ZRankAsync(key, Encoding.UTF8.GetBytes("nine"));
        Assert.Equal(8, rank);

        rank = await redis.SortedSet.ZRankAsync(key, "one_none");
        Assert.False(rank.HasValue);

        rank = await redis.SortedSet.ZRankAsync(key, Encoding.UTF8.GetBytes("nine_none"));
        Assert.False(rank.HasValue);

        var scoreRank = await redis.SortedSet.ZRankWithScoreAsync(key, "six");
        Assert.True(scoreRank.HasValue);
        Assert.Equal(5, scoreRank.Value.Rank);
        Assert.Equal(6, scoreRank.Value.Score);

        scoreRank = await redis.SortedSet.ZRankWithScoreAsync(key, Encoding.UTF8.GetBytes("four"));
        Assert.True(scoreRank.HasValue);
        Assert.Equal(3, scoreRank.Value.Rank);
        Assert.Equal(4, scoreRank.Value.Score);

        scoreRank = await redis.SortedSet.ZRankWithScoreAsync(key, "six_none");
        Assert.False(scoreRank.HasValue);

        scoreRank = await redis.SortedSet.ZRankWithScoreAsync(key, Encoding.UTF8.GetBytes("six_none"));
        Assert.False(scoreRank.HasValue);

        scoreRank = await redis.SortedSet.ZRankWithScoreAsync("none_zrank_fasfdf", "six");
        Assert.False(scoreRank.HasValue);

        scoreRank = await redis.SortedSet.ZRankWithScoreAsync("none_zrank_fasfdf", Encoding.UTF8.GetBytes("four"));
        Assert.False(scoreRank.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRem(Redis redis)
    {
        const string key = "zset_zrem_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var rank = redis.SortedSet.ZRank(key, "two");
        Assert.True(rank.HasValue);
        Assert.Equal(1, rank);

        var remOk = redis.SortedSet.ZRem(key, "two");
        Assert.True(remOk);

        remOk = redis.SortedSet.ZRem("zrem_none_key", "two");
        Assert.False(remOk);

        var count = redis.SortedSet.ZCount(key, 0, double.PositiveInfinity);
        Assert.Equal(9, count);

        rank = redis.SortedSet.ZRank(key, "two");
        Assert.False(rank.HasValue);

        remOk = redis.SortedSet.ZRem(key, "two");
        Assert.False(remOk);

        remOk = redis.SortedSet.ZRem(key, Encoding.UTF8.GetBytes("five"));
        Assert.True(remOk);

        count = redis.SortedSet.ZCount(key, 0, double.PositiveInfinity);
        Assert.Equal(8, count);

        var remCount = redis.SortedSet.ZRem(key, members.Keys.ToArray()[5..]);
        Assert.Equal(5, remCount);

        remCount = redis.SortedSet.ZRem(key, members.Keys.Select(f => Encoding.UTF8.GetBytes(f)).ToArray());
        Assert.Equal(3, remCount);

        var exists = redis.Key.Exists(key);
        Assert.False(exists);

        remCount = redis.SortedSet.ZRem("zrem_none_key", members.Keys.ToArray());
        Assert.Equal(0, remCount);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRemAsync(Redis redis)
    {
        const string key = "zset_zrem_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var rank = await redis.SortedSet.ZRankAsync(key, "two");
        Assert.True(rank.HasValue);
        Assert.Equal(1, rank);

        var remOk = await redis.SortedSet.ZRemAsync(key, "two");
        Assert.True(remOk);

        remOk = await redis.SortedSet.ZRemAsync("zrem_none_key", "two");
        Assert.False(remOk);

        var count = await redis.SortedSet.ZCountAsync(key, 0, double.PositiveInfinity);
        Assert.Equal(9, count);

        rank = await redis.SortedSet.ZRankAsync(key, "two");
        Assert.False(rank.HasValue);

        remOk = await redis.SortedSet.ZRemAsync(key, "two");
        Assert.False(remOk);

        remOk = await redis.SortedSet.ZRemAsync(key, Encoding.UTF8.GetBytes("five"));
        Assert.True(remOk);

        count = await redis.SortedSet.ZCountAsync(key, 0, double.PositiveInfinity);
        Assert.Equal(8, count);

        var remCount = await redis.SortedSet.ZRemAsync(key, members.Keys.ToArray()[5..]);
        Assert.Equal(5, remCount);

        remCount = await redis.SortedSet.ZRemAsync(key, members.Keys.Select(f => Encoding.UTF8.GetBytes(f)).ToArray());
        Assert.Equal(3, remCount);

        var exists = await redis.Key.ExistsAsync(key);
        Assert.False(exists);

        remCount = await redis.SortedSet.ZRemAsync("zrem_none_key", members.Keys.ToArray());
        Assert.Equal(0, remCount);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRemRangeByLex(Redis redis)
    {
        const string key = "zset_zremrangebylex_key";
        var members = new Dictionary<string, double>
        {
            { "ALPHA", 1 },
            { "aaaa", 1 },
            { "alpha", 1 },
            { "b", 1 },
            { "c", 1 },
            { "d", 1 },
            { "e", 1 },
            { "foo", 1 },
            { "zap", 1 },
            { "zip", 1 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var range = redis.SortedSet.ZRange(key, 0, -1);
        Assert.NotNull(range);
        Assert.Equal<string[]>([.. members.Keys], range);

        var remCount = redis.SortedSet.ZRemRangeByLex(key, "[alpha", "[omega");
        Assert.Equal(6, remCount);

        range = redis.SortedSet.ZRange(key, 0, -1);
        Assert.NotNull(range);
        Assert.Equal(4, range.Length);
        Assert.Equal<string[]>(["ALPHA", "aaaa", "zap", "zip"], range);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRemRangeByLexAsync(Redis redis)
    {
        const string key = "zset_zremrangebylex_key_async";
        var members = new Dictionary<string, double>
        {
            { "ALPHA", 1 },
            { "aaaa", 1 },
            { "alpha", 1 },
            { "b", 1 },
            { "c", 1 },
            { "d", 1 },
            { "e", 1 },
            { "foo", 1 },
            { "zap", 1 },
            { "zip", 1 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var range = await redis.SortedSet.ZRangeAsync(key, 0, -1);
        Assert.NotNull(range);
        Assert.Equal<string[]>([.. members.Keys], range);

        var remCount = await redis.SortedSet.ZRemRangeByLexAsync(key, "[alpha", "[omega");
        Assert.Equal(6, remCount);

        range = await redis.SortedSet.ZRangeAsync(key, 0, -1);
        Assert.NotNull(range);
        Assert.Equal(4, range.Length);
        Assert.Equal<string[]>(["ALPHA", "aaaa", "zap", "zip"], range);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRemRangeByRank(Redis redis)
    {
        const string key = "zset_zremrangebyrank_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var remCount = redis.SortedSet.ZRemRangeByRank(key, 0, -3);
        Assert.Equal(8, remCount);

        var range = redis.SortedSet.ZRange(key, 0, -1);
        Assert.NotNull(range);
        Assert.Equal<string[]>(["nine", "ten"], range);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRemRangeByRankAsync(Redis redis)
    {
        const string key = "zset_zremrangebyrank_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var remCount = await redis.SortedSet.ZRemRangeByRankAsync(key, 0, -3);
        Assert.Equal(8, remCount);

        var range = await redis.SortedSet.ZRangeAsync(key, 0, -1);
        Assert.NotNull(range);
        Assert.Equal<string[]>(["nine", "ten"], range);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRemRangeByScore(Redis redis)
    {
        const string key = "zset_zremrangebyscore_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var remCount = redis.SortedSet.ZRemRangeByScore(key, double.NegativeInfinity, 2, true, false);
        Assert.Equal(1, remCount);

        var range = redis.SortedSet.ZRange(key, 0, -1);
        Assert.NotNull(range);
        Assert.Equal<string[]>(["two", "three"], range);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRemRangeByScoreAsync(Redis redis)
    {
        const string key = "zset_zremrangebyscore_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var remCount = await redis.SortedSet.ZRemRangeByScoreAsync(key, double.NegativeInfinity, 2, true, false);
        Assert.Equal(1, remCount);

        var range = await redis.SortedSet.ZRangeAsync(key, 0, -1);
        Assert.NotNull(range);
        Assert.Equal<string[]>(["two", "three"], range);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRevRange(Redis redis)
    {
        const string key = "zset_zrevrange_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1.234 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8.8 },
            { "nine", 9 },
            { "ten", 10.65 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var range = redis.SortedSet.ZRevRange(key, 0, -2);
        Assert.NotNull(range);
        Assert.Equal(9, range.Length);
        Assert.Equal<string[]>(members.Keys.ToArray().Reverse().ToArray()[..^1], range);

        range = redis.SortedSet.ZRevRange("ZRevRange_none", 0, -2);
        Assert.Null(range);

        var rangeBytes = redis.SortedSet.ZRevRangeBytes(key, 2, -3);
        Assert.NotNull(rangeBytes);
        Assert.Equal(6, rangeBytes.Length);
        Assert.Equal(members.Keys.ToArray().Reverse().ToArray()[2..^2].Select(f => Encoding.UTF8.GetBytes(f)).ToArray(), rangeBytes);

        rangeBytes = redis.SortedSet.ZRevRangeBytes("ZRevRange_none", 2, -3);
        Assert.Null(rangeBytes);

        var rangeScore = redis.SortedSet.ZRevRangeWithScores(key, 1, 5);
        Assert.NotNull(rangeScore);
        Assert.Equal(5, rangeScore.Length);
        Assert.Equal(9, rangeScore[0].Score);
        Assert.Equal(5, rangeScore[^1].Score);

        rangeScore = redis.SortedSet.ZRevRangeWithScores("ZRevRange_none", 1, 5);
        Assert.Null(rangeScore);

        var rangeScoreBytes = redis.SortedSet.ZRevRangeWithScoresBytes(key, 1, 5);
        Assert.NotNull(rangeScoreBytes);
        Assert.Equal(5, rangeScoreBytes.Length);
        Assert.Equal(9, rangeScoreBytes[0].Score);
        Assert.Equal(5, rangeScoreBytes[^1].Score);
        Assert.Equal(Encoding.UTF8.GetBytes("nine"), rangeScoreBytes[0].Member);

        rangeScoreBytes = redis.SortedSet.ZRevRangeWithScoresBytes("ZRevRange_none", 1, 5);
        Assert.Null(rangeScoreBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRevRangeAsync(Redis redis)
    {
        const string key = "zset_zrevrange_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1.234 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8.8 },
            { "nine", 9 },
            { "ten", 10.65 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var range = await redis.SortedSet.ZRevRangeAsync(key, 0, -2);
        Assert.NotNull(range);
        Assert.Equal(9, range.Length);
        Assert.Equal<string[]>(members.Keys.ToArray().Reverse().ToArray()[..^1], range);

        range = await redis.SortedSet.ZRevRangeAsync("ZRevRange_none", 0, -2);
        Assert.Null(range);

        var rangeBytes = await redis.SortedSet.ZRevRangeBytesAsync(key, 2, -3);
        Assert.NotNull(rangeBytes);
        Assert.Equal(6, rangeBytes.Length);
        Assert.Equal(members.Keys.ToArray().Reverse().ToArray()[2..^2].Select(f => Encoding.UTF8.GetBytes(f)).ToArray(), rangeBytes);

        rangeBytes = await redis.SortedSet.ZRevRangeBytesAsync("ZRevRange_none", 2, -3);
        Assert.Null(rangeBytes);

        var rangeScore = await redis.SortedSet.ZRevRangeWithScoresAsync(key, 1, 5);
        Assert.NotNull(rangeScore);
        Assert.Equal(5, rangeScore.Length);
        Assert.Equal(9, rangeScore[0].Score);
        Assert.Equal(5, rangeScore[^1].Score);

        rangeScore = await redis.SortedSet.ZRevRangeWithScoresAsync("ZRevRange_none", 1, 5);
        Assert.Null(rangeScore);

        var rangeScoreBytes = await redis.SortedSet.ZRevRangeWithScoresBytesAsync(key, 1, 5);
        Assert.NotNull(rangeScoreBytes);
        Assert.Equal(5, rangeScoreBytes.Length);
        Assert.Equal(9, rangeScoreBytes[0].Score);
        Assert.Equal(5, rangeScoreBytes[^1].Score);
        Assert.Equal(Encoding.UTF8.GetBytes("nine"), rangeScoreBytes[0].Member);

        rangeScoreBytes = await redis.SortedSet.ZRevRangeWithScoresBytesAsync("ZRevRange_none", 1, 5);
        Assert.Null(rangeScoreBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRevRangeByLex(Redis redis)
    {
        const string key = "zset_zrevrangebylex_key";
        var members = new Dictionary<string, double>
        {
            { "a", 0 },
            { "b", 0 },
            { "c", 0 },
            { "d", 0 },
            { "e", 0 },
            { "f", 0 },
            { "g", 0 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var range = redis.SortedSet.ZRevRangeByLex(key, "[c", "-");
        Assert.NotNull(range);
        Assert.Equal(3, range.Length);
        Assert.Equal<string[]>(["c", "b", "a"], range);

        range = redis.SortedSet.ZRevRangeByLex(key, "(g", "[aaa");
        Assert.NotNull(range);
        Assert.Equal(5, range.Length);
        Assert.Equal<string[]>(["f", "e", "d", "c", "b"], range);

        range = redis.SortedSet.ZRevRangeByLex(key, "(g", "[aaa", 1, 3);
        Assert.NotNull(range);
        Assert.Equal(3, range.Length);
        Assert.Equal<string[]>(["e", "d", "c"], range);

        range = redis.SortedSet.ZRevRangeByLex("ZRevRangeByLex_none", "(g", "[aaa", 1, 3);
        Assert.Null(range);

        var rangeBytes = redis.SortedSet.ZRevRangeByLexBytes(key, "[c", "-");
        Assert.NotNull(rangeBytes);
        Assert.Equal(3, rangeBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("c"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("a")], rangeBytes);

        rangeBytes = redis.SortedSet.ZRevRangeByLexBytes(key, "(g", "[aaa", 1, 3);
        Assert.NotNull(rangeBytes);
        Assert.Equal(3, rangeBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("e"), Encoding.UTF8.GetBytes("d"), Encoding.UTF8.GetBytes("c")], rangeBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRevRangeByLexAsync(Redis redis)
    {
        const string key = "zset_zrevrangebylex_key_async";
        var members = new Dictionary<string, double>
        {
            { "a", 0 },
            { "b", 0 },
            { "c", 0 },
            { "d", 0 },
            { "e", 0 },
            { "f", 0 },
            { "g", 0 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var range = await redis.SortedSet.ZRevRangeByLexAsync(key, "[c", "-");
        Assert.NotNull(range);
        Assert.Equal(3, range.Length);
        Assert.Equal<string[]>(["c", "b", "a"], range);

        range = await redis.SortedSet.ZRevRangeByLexAsync(key, "(g", "[aaa");
        Assert.NotNull(range);
        Assert.Equal(5, range.Length);
        Assert.Equal<string[]>(["f", "e", "d", "c", "b"], range);

        range = await redis.SortedSet.ZRevRangeByLexAsync(key, "(g", "[aaa", 1, 3);
        Assert.NotNull(range);
        Assert.Equal(3, range.Length);
        Assert.Equal<string[]>(["e", "d", "c"], range);

        range = await redis.SortedSet.ZRevRangeByLexAsync("ZRevRangeByLex_none", "(g", "[aaa", 1, 3);
        Assert.Null(range);

        var rangeBytes = await redis.SortedSet.ZRevRangeByLexBytesAsync(key, "[c", "-");
        Assert.NotNull(rangeBytes);
        Assert.Equal(3, rangeBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("c"), Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("a")], rangeBytes);

        rangeBytes = await redis.SortedSet.ZRevRangeByLexBytesAsync(key, "(g", "[aaa", 1, 3);
        Assert.NotNull(rangeBytes);
        Assert.Equal(3, rangeBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("e"), Encoding.UTF8.GetBytes("d"), Encoding.UTF8.GetBytes("c")], rangeBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRevRangeByScore(Redis redis)
    {
        const string key = "zset_zrevrangebyscore_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1.234 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8.8 },
            { "nine", 9 },
            { "ten", 10.65 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var range = redis.SortedSet.ZRevRangeByScore(key, 11.23, 2);
        Assert.NotNull(range);
        Assert.Equal(9, range.Length);

        range = redis.SortedSet.ZRevRangeByScore(key, 11.23, 2, true, false);
        Assert.NotNull(range);
        Assert.Equal(8, range.Length);
        Assert.Equal("ten", range[0]);
        Assert.Equal("three", range[^1]);

        range = redis.SortedSet.ZRevRangeByScore("ZRevRangeByScore_none", double.PositiveInfinity, double.NegativeInfinity, false, false);
        Assert.Null(range);

        var rangeScore = redis.SortedSet.ZRevRangeByScoreWithScores(key, 10.65, 5, false);
        Assert.NotNull(rangeScore);
        Assert.Equal(5, rangeScore.Length);
        Assert.Equal("nine", rangeScore[0].Member);
        Assert.Equal(9, rangeScore[0].Score);

        rangeScore = redis.SortedSet.ZRevRangeByScoreWithScores(key, 10.65, 5, 0, 2);
        Assert.NotNull(rangeScore);
        Assert.Equal(2, rangeScore.Length);
        Assert.Equal("ten", rangeScore[0].Member);
        Assert.Equal(10.65, rangeScore[0].Score);

        var rangeBytes = redis.SortedSet.ZRevRangeByScoreBytes(key, 11.23, 2);
        Assert.NotNull(rangeBytes);
        Assert.Equal(9, rangeBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("ten"), rangeBytes[0]);

        var rangeScoreBytes = redis.SortedSet.ZRevRangeByScoreWithScoresBytes(key, 10.65, 5);
        Assert.NotNull(rangeScoreBytes);
        Assert.Equal(6, rangeScoreBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("ten"), rangeScoreBytes[0].Member);
        Assert.Equal(10.65, rangeScoreBytes[0].Score);
        Assert.Equal(5, rangeScoreBytes[^1].Score);

        rangeScoreBytes = redis.SortedSet.ZRevRangeByScoreWithScoresBytes("ZRevRangeByScore_none", 10.65, 5);
        Assert.Null(rangeScoreBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRevRangeByScoreAsync(Redis redis)
    {
        const string key = "zset_zrevrangebyscore_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1.234 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8.8 },
            { "nine", 9 },
            { "ten", 10.65 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var range = await redis.SortedSet.ZRevRangeByScoreAsync(key, 11.23, 2);
        Assert.NotNull(range);
        Assert.Equal(9, range.Length);

        range = await redis.SortedSet.ZRevRangeByScoreAsync(key, 11.23, 2, true, false);
        Assert.NotNull(range);
        Assert.Equal(8, range.Length);
        Assert.Equal("ten", range[0]);
        Assert.Equal("three", range[^1]);

        range = await redis.SortedSet.ZRevRangeByScoreAsync("ZRevRangeByScore_none", double.PositiveInfinity, double.NegativeInfinity, false, false);
        Assert.Null(range);

        var rangeScore = await redis.SortedSet.ZRevRangeByScoreWithScoresAsync(key, 10.65, 5, false);
        Assert.NotNull(rangeScore);
        Assert.Equal(5, rangeScore.Length);
        Assert.Equal("nine", rangeScore[0].Member);
        Assert.Equal(9, rangeScore[0].Score);

        rangeScore = await redis.SortedSet.ZRevRangeByScoreWithScoresAsync(key, 10.65, 5, 0, 2);
        Assert.NotNull(rangeScore);
        Assert.Equal(2, rangeScore.Length);
        Assert.Equal("ten", rangeScore[0].Member);
        Assert.Equal(10.65, rangeScore[0].Score);

        var rangeBytes = await redis.SortedSet.ZRevRangeByScoreBytesAsync(key, 11.23, 2);
        Assert.NotNull(rangeBytes);
        Assert.Equal(9, rangeBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("ten"), rangeBytes[0]);

        var rangeScoreBytes = await redis.SortedSet.ZRevRangeByScoreWithScoresBytesAsync(key, 10.65, 5);
        Assert.NotNull(rangeScoreBytes);
        Assert.Equal(6, rangeScoreBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("ten"), rangeScoreBytes[0].Member);
        Assert.Equal(10.65, rangeScoreBytes[0].Score);
        Assert.Equal(5, rangeScoreBytes[^1].Score);

        rangeScoreBytes = await redis.SortedSet.ZRevRangeByScoreWithScoresBytesAsync("ZRevRangeByScore_none", 10.65, 5);
        Assert.Null(rangeScoreBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZRevRank(Redis redis)
    {
        const string key = "zset_zrevrank_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var rank = redis.SortedSet.ZRevRank(key, "one");
        Assert.Equal(9, rank);

        rank = redis.SortedSet.ZRevRank(key, "eight");
        Assert.Equal(2, rank);

        rank = redis.SortedSet.ZRevRank(key, Encoding.UTF8.GetBytes("nine"));
        Assert.Equal(1, rank);

        rank = redis.SortedSet.ZRevRank(key, "one_none");
        Assert.False(rank.HasValue);

        rank = redis.SortedSet.ZRevRank(key, Encoding.UTF8.GetBytes("nine_none"));
        Assert.False(rank.HasValue);

        var scoreRank = redis.SortedSet.ZRevRankWithScore(key, "six");
        Assert.True(scoreRank.HasValue);
        Assert.Equal(4, scoreRank.Value.Rank);
        Assert.Equal(6, scoreRank.Value.Score);

        scoreRank = redis.SortedSet.ZRevRankWithScore(key, Encoding.UTF8.GetBytes("four"));
        Assert.True(scoreRank.HasValue);
        Assert.Equal(6, scoreRank.Value.Rank);
        Assert.Equal(4, scoreRank.Value.Score);

        scoreRank = redis.SortedSet.ZRevRankWithScore(key, "six_none");
        Assert.False(scoreRank.HasValue);

        scoreRank = redis.SortedSet.ZRevRankWithScore(key, Encoding.UTF8.GetBytes("six_none"));
        Assert.False(scoreRank.HasValue);

        scoreRank = redis.SortedSet.ZRevRankWithScore("none_zrank_fasfdf", "six");
        Assert.False(scoreRank.HasValue);

        scoreRank = redis.SortedSet.ZRevRankWithScore("none_zrank_fasfdf", Encoding.UTF8.GetBytes("four"));
        Assert.False(scoreRank.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZRevRankAsync(Redis redis)
    {
        const string key = "zset_zrevrank_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var rank = await redis.SortedSet.ZRevRankAsync(key, "one");
        Assert.Equal(9, rank);

        rank = await redis.SortedSet.ZRevRankAsync(key, "eight");
        Assert.Equal(2, rank);

        rank = await redis.SortedSet.ZRevRankAsync(key, Encoding.UTF8.GetBytes("nine"));
        Assert.Equal(1, rank);

        rank = await redis.SortedSet.ZRevRankAsync(key, "one_none");
        Assert.False(rank.HasValue);

        rank = await redis.SortedSet.ZRevRankAsync(key, Encoding.UTF8.GetBytes("nine_none"));
        Assert.False(rank.HasValue);

        var scoreRank = await redis.SortedSet.ZRevRankWithScoreAsync(key, "six");
        Assert.True(scoreRank.HasValue);
        Assert.Equal(4, scoreRank.Value.Rank);
        Assert.Equal(6, scoreRank.Value.Score);

        scoreRank = await redis.SortedSet.ZRevRankWithScoreAsync(key, Encoding.UTF8.GetBytes("four"));
        Assert.True(scoreRank.HasValue);
        Assert.Equal(6, scoreRank.Value.Rank);
        Assert.Equal(4, scoreRank.Value.Score);

        scoreRank = await redis.SortedSet.ZRevRankWithScoreAsync(key, "six_none");
        Assert.False(scoreRank.HasValue);

        scoreRank = await redis.SortedSet.ZRevRankWithScoreAsync(key, Encoding.UTF8.GetBytes("six_none"));
        Assert.False(scoreRank.HasValue);

        scoreRank = await redis.SortedSet.ZRevRankWithScoreAsync("none_zrank_fasfdf", "six");
        Assert.False(scoreRank.HasValue);

        scoreRank = await redis.SortedSet.ZRevRankWithScoreAsync("none_zrank_fasfdf", Encoding.UTF8.GetBytes("four"));
        Assert.False(scoreRank.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZScore(Redis redis)
    {
        const string key = "zset_zscore_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1.123456 },
            { "two", 2.9874 },
            { "three", 3.3336 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var score = redis.SortedSet.ZScore(key, "two");
        Assert.True(score.HasValue);
        Assert.Equal(2.9874M, score);

        score = redis.SortedSet.ZScore(key, Encoding.UTF8.GetBytes("three"));
        Assert.True(score.HasValue);
        Assert.Equal(3.3336d, score);

        score = redis.SortedSet.ZScore(key, Encoding.UTF8.GetBytes("none"));
        Assert.False(score.HasValue);

        score = redis.SortedSet.ZScore("ZScore_none", Encoding.UTF8.GetBytes("one"));
        Assert.False(score.HasValue);

        score = redis.SortedSet.ZScore(key, "two_none");
        Assert.False(score.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZScoreAsync(Redis redis)
    {
        const string key = "zset_zscore_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1.123456 },
            { "two", 2.9874 },
            { "three", 3.3336 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var score = await redis.SortedSet.ZScoreAsync(key, "two");
        Assert.True(score.HasValue);
        Assert.Equal(2.9874M, score);

        score = await redis.SortedSet.ZScoreAsync(key, Encoding.UTF8.GetBytes("three"));
        Assert.True(score.HasValue);
        Assert.Equal(3.3336d, score);

        score = await redis.SortedSet.ZScoreAsync(key, Encoding.UTF8.GetBytes("none"));
        Assert.False(score.HasValue);

        score = await redis.SortedSet.ZScoreAsync("ZScore_none", Encoding.UTF8.GetBytes("one"));
        Assert.False(score.HasValue);

        score = await redis.SortedSet.ZScoreAsync(key, "two_none");
        Assert.False(score.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZUnion(Redis redis)
    {
        const string key1 = "zset_zunion1";
        const string key2 = "zset_zunion2";

        _ = redis.Key.Del([key1, key2]);
        redis.SortedSet.ZAdd(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var union = redis.SortedSet.ZUnion(key1, key2);
        Assert.Equal<string?[]>(["one", "two", "three"], union);

        var unionBytes = redis.SortedSet.ZUnionBytes(key1, key2);
        Assert.Equal([Encoding.UTF8.GetBytes("one"), Encoding.UTF8.GetBytes("two"), Encoding.UTF8.GetBytes("three")], unionBytes);

        var unionWithScore = redis.SortedSet.ZUnionWithScores(key1, key2);
        Assert.NotNull(unionWithScore);
        Assert.Equal(3, unionWithScore.Length);
        Assert.Equal("one", unionWithScore![0]!.Member);
        Assert.Equal("two", unionWithScore![1]!.Member);
        Assert.Equal(11, unionWithScore![0]!.Score);
        Assert.Equal(22, unionWithScore![1]!.Score);

        unionWithScore = redis.SortedSet.ZUnionWithScores(key1, key2, Aggregate.Max);
        Assert.Equal("one", unionWithScore![0]!.Member);
        Assert.Equal("two", unionWithScore![1]!.Member);
        Assert.Equal(10, unionWithScore![0]!.Score);
        Assert.Equal(20, unionWithScore![1]!.Score);

        unionWithScore = redis.SortedSet.ZUnionWithScores(key1, key2, Aggregate.Min);
        Assert.Equal("one", unionWithScore![0]!.Member);
        Assert.Equal("two", unionWithScore![1]!.Member);
        Assert.Equal(1, unionWithScore![0]!.Score);
        Assert.Equal(2, unionWithScore![1]!.Score);

        var interWithScoreBytes = redis.SortedSet.ZUnionWithScoresBytes(key1, key2);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), interWithScoreBytes![0]!.Member);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), interWithScoreBytes![1]!.Member);
        Assert.Equal(11, interWithScoreBytes![0]!.Score);
        Assert.Equal(22, interWithScoreBytes![1]!.Score);

        redis.SortedSet.ZAdd(key1, new Dictionary<string, double>
        {
            { "one", 1.5 },
            { "two", 2.6 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, double>
        {
            { "one", 10.5 },
            { "two", 20.22 },
            { "three", 30 },
        });

        var interWithScoreWeight = redis.SortedSet.ZUnionWithScores(key1, 3, key2, 5);
        Assert.Equal("one", interWithScoreWeight![0]!.Member);
        Assert.Equal("two", interWithScoreWeight![1]!.Member);
        Assert.Equal(1.5 * 3 + 10.5 * 5, interWithScoreWeight![0]!.Score);
        Assert.Equal(2.6 * 3 + 20.22 * 5, interWithScoreWeight![1]!.Score);

        var interWithScoreWeightBytes = redis.SortedSet.ZUnionWithScoresBytes(key1, 3, key2, 5);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), interWithScoreWeightBytes![0]!.Member);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), interWithScoreWeightBytes![1]!.Member);
        Assert.Equal(1.5 * 3 + 10.5 * 5, interWithScoreWeightBytes![0]!.Score);
        Assert.Equal(2.6 * 3 + 20.22 * 5, interWithScoreWeightBytes![1]!.Score);


        var interNot = redis.SortedSet.ZUnion(key1, "jjjjjjjjjjj");
        Assert.NotNull(interNot);
        Assert.Equal<string[]>(["one", "two"], interNot);

        interNot = redis.SortedSet.ZUnion("noenoeneoneo", "jjjjjjjjjjj");
        Assert.Null(interNot);

        _ = redis.Key.Del([key1, key2]);
        redis.SortedSet.ZAdd(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, int>
        {
            { "one1", 10 },
            { "two1", 20 },
            { "three1", 30 },
        });

        var interNon = redis.SortedSet.ZUnionWithScores(key1, key2);
        Assert.NotNull(interNon);
        Assert.Equal(5, interNon.Length);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZUnionAsync(Redis redis)
    {
        const string key1 = "zset_zunion1_async";
        const string key2 = "zset_zunion2_async";

        _ = await redis.Key.DelAsync([key1, key2]);
        await redis.SortedSet.ZAddAsync(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        });

        await redis.SortedSet.ZAddAsync(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var union = await redis.SortedSet.ZUnionAsync(key1, key2);
        Assert.Equal<string?[]>(["one", "two", "three"], union);

        var unionBytes = await redis.SortedSet.ZUnionBytesAsync(key1, key2);
        Assert.Equal([Encoding.UTF8.GetBytes("one"), Encoding.UTF8.GetBytes("two"), Encoding.UTF8.GetBytes("three")], unionBytes);

        var unionWithScore = await redis.SortedSet.ZUnionWithScoresAsync(key1, key2);
        Assert.NotNull(unionWithScore);
        Assert.Equal(3, unionWithScore.Length);
        Assert.Equal("one", unionWithScore![0]!.Member);
        Assert.Equal("two", unionWithScore![1]!.Member);
        Assert.Equal(11, unionWithScore![0]!.Score);
        Assert.Equal(22, unionWithScore![1]!.Score);

        unionWithScore = await redis.SortedSet.ZUnionWithScoresAsync(key1, key2, Aggregate.Max);
        Assert.Equal("one", unionWithScore![0]!.Member);
        Assert.Equal("two", unionWithScore![1]!.Member);
        Assert.Equal(10, unionWithScore![0]!.Score);
        Assert.Equal(20, unionWithScore![1]!.Score);

        unionWithScore = await redis.SortedSet.ZUnionWithScoresAsync(key1, key2, Aggregate.Min);
        Assert.Equal("one", unionWithScore![0]!.Member);
        Assert.Equal("two", unionWithScore![1]!.Member);
        Assert.Equal(1, unionWithScore![0]!.Score);
        Assert.Equal(2, unionWithScore![1]!.Score);

        var interWithScoreBytes = await redis.SortedSet.ZUnionWithScoresBytesAsync(key1, key2);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), interWithScoreBytes![0]!.Member);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), interWithScoreBytes![1]!.Member);
        Assert.Equal(11, interWithScoreBytes![0]!.Score);
        Assert.Equal(22, interWithScoreBytes![1]!.Score);

        await redis.SortedSet.ZAddAsync(key1, new Dictionary<string, double>
        {
            { "one", 1.5 },
            { "two", 2.6 },
        });

        await redis.SortedSet.ZAddAsync(key2, new Dictionary<string, double>
        {
            { "one", 10.5 },
            { "two", 20.22 },
            { "three", 30 },
        });

        var interWithScoreWeight = await redis.SortedSet.ZUnionWithScoresAsync(key1, 3, key2, 5);
        Assert.Equal("one", interWithScoreWeight![0]!.Member);
        Assert.Equal("two", interWithScoreWeight![1]!.Member);
        Assert.Equal(1.5 * 3 + 10.5 * 5, interWithScoreWeight![0]!.Score);
        Assert.Equal(2.6 * 3 + 20.22 * 5, interWithScoreWeight![1]!.Score);

        var interWithScoreWeightBytes = await redis.SortedSet.ZUnionWithScoresBytesAsync(key1, 3, key2, 5);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), interWithScoreWeightBytes![0]!.Member);
        Assert.Equal(Encoding.UTF8.GetBytes("two"), interWithScoreWeightBytes![1]!.Member);
        Assert.Equal(1.5 * 3 + 10.5 * 5, interWithScoreWeightBytes![0]!.Score);
        Assert.Equal(2.6 * 3 + 20.22 * 5, interWithScoreWeightBytes![1]!.Score);


        var interNot = await redis.SortedSet.ZUnionAsync(key1, "jjjjjjjjjjj");
        Assert.NotNull(interNot);
        Assert.Equal<string[]>(["one", "two"], interNot);

        interNot = await redis.SortedSet.ZUnionAsync("noenoeneoneo", "jjjjjjjjjjj");
        Assert.Null(interNot);

        _ = await redis.Key.DelAsync([key1, key2]);
        await redis.SortedSet.ZAddAsync(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
        });

        await redis.SortedSet.ZAddAsync(key2, new Dictionary<string, int>
        {
            { "one1", 10 },
            { "two1", 20 },
            { "three1", 30 },
        });

        var interNon = await redis.SortedSet.ZUnionWithScoresAsync(key1, key2);
        Assert.NotNull(interNon);
        Assert.Equal(5, interNon.Length);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZUnionStore(Redis redis)
    {
        const string key1 = "zset_zunionstore1";
        const string key2 = "zset_zunionstore2";
        const string destination = "zset_zunionstore_result";

        _ = redis.Key.Del([key1, key2]);
        redis.SortedSet.ZAdd(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        redis.SortedSet.ZAdd(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var count = redis.SortedSet.ZUnionStore(destination, key1, key2);
        Assert.Equal(4, count);

        count = redis.SortedSet.ZUnionStore(destination, key1, "asdgfjd");
        Assert.Equal(4, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZUnionStoreAsync(Redis redis)
    {
        const string key1 = "zset_zunionstore1_async";
        const string key2 = "zset_zunionstore2_async";
        const string destination = "zset_zunionstore_result_async";

        _ = await redis.Key.DelAsync([key1, key2]);
        await redis.SortedSet.ZAddAsync(key1, new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
        });

        await redis.SortedSet.ZAddAsync(key2, new Dictionary<string, int>
        {
            { "one", 10 },
            { "two", 20 },
            { "three", 30 },
        });

        var count = await redis.SortedSet.ZUnionStoreAsync(destination, key1, key2);
        Assert.Equal(4, count);

        count = await redis.SortedSet.ZUnionStoreAsync(destination, key1, "asdgfjd");
        Assert.Equal(4, count);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void ZScan(Redis redis)
    {
        const string key = "zset_zscan_key";
        var members = new Dictionary<string, double>
        {
            { "one", 1.234 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8.8 },
            { "nine", 9 },
            { "ten", 10.65 },
        };
        _ = redis.SortedSet.ZAdd(key, members);

        var scan = redis.SortedSet.ZScan(key, 0);
        Assert.NotNull(scan);
        Assert.NotNull(scan.Data);
        Assert.Equal(10, scan.Data.Length);
        Assert.Equal("one", scan.Data[0].Member);
        Assert.Equal(1.234, scan.Data[0].Score);

        scan = redis.SortedSet.ZScan(key, 0, "kjkjk*");
        Assert.NotNull(scan);
        Assert.Null(scan.Data);

        scan = redis.SortedSet.ZScan("nononfosdfdsf_none", 0);
        Assert.NotNull(scan);
        Assert.Null(scan.Data);

        var scanBytes = redis.SortedSet.ZScanBytes(key, 0);
        Assert.NotNull(scanBytes);
        Assert.NotNull(scanBytes.Data);
        Assert.Equal(10, scanBytes.Data.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), scanBytes.Data[0].Member);
        Assert.Equal(1.234, scanBytes.Data[0].Score);

        scanBytes = redis.SortedSet.ZScanBytes("nononfosdfdsf_none", 0);
        Assert.NotNull(scanBytes);
        Assert.Null(scanBytes.Data);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task ZScanAsync(Redis redis)
    {
        const string key = "zset_zscan_key_async";
        var members = new Dictionary<string, double>
        {
            { "one", 1.234 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8.8 },
            { "nine", 9 },
            { "ten", 10.65 },
        };
        _ = await redis.SortedSet.ZAddAsync(key, members);

        var scan = await redis.SortedSet.ZScanAsync(key, 0);
        Assert.NotNull(scan);
        Assert.NotNull(scan.Data);
        Assert.Equal(10, scan.Data.Length);
        Assert.Equal("one", scan.Data[0].Member);
        Assert.Equal(1.234, scan.Data[0].Score);

        scan = await redis.SortedSet.ZScanAsync(key, 0, "kjkjk*");
        Assert.NotNull(scan);
        Assert.Null(scan.Data);

        scan = await redis.SortedSet.ZScanAsync("nononfosdfdsf_none", 0);
        Assert.NotNull(scan);
        Assert.Null(scan.Data);

        var scanBytes = await redis.SortedSet.ZScanBytesAsync(key, 0);
        Assert.NotNull(scanBytes);
        Assert.NotNull(scanBytes.Data);
        Assert.Equal(10, scanBytes.Data.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("one"), scanBytes.Data[0].Member);
        Assert.Equal(1.234, scanBytes.Data[0].Score);

        scanBytes = await redis.SortedSet.ZScanBytesAsync("nononfosdfdsf_none", 0);
        Assert.NotNull(scanBytes);
        Assert.Null(scanBytes.Data);
    }
}
