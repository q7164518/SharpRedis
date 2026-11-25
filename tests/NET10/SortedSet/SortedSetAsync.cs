using SharpRedis;

namespace NET10.SortedSet;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal sealed class SortedSetAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public async Task ZAddAsync()
    {
        const string key1 = "{82}:SortedSet:ZAddAsync:Key1";

        var addCount = await this._redis.SortedSet.ZAddAsync(key1, "a", 123);
        Assert.AreEqual(1, (int)addCount);

        double aScore = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.AreEqual(123, aScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "a", 122);
        Assert.AreEqual(0, (int)addCount);

        aScore = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.AreEqual(122, aScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "a", 456, RedisSortedSet.ZAddWhen.NX);
        Assert.AreEqual(0, (int)addCount);

        aScore = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.AreEqual(122, aScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "a", 456, RedisSortedSet.ZAddWhen.XX);
        Assert.AreEqual(0, (int)addCount);

        aScore = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.AreEqual(456, aScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "a", 100, scoreWhen: RedisSortedSet.ZAddScoreWhen.LT);
        Assert.AreEqual(0, (int)addCount);

        aScore = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.AreEqual(100, aScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "a", 520, scoreWhen: RedisSortedSet.ZAddScoreWhen.LT);
        Assert.AreEqual(0, (int)addCount);

        aScore = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.AreEqual(100, aScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "a", 520, scoreWhen: RedisSortedSet.ZAddScoreWhen.GT);
        Assert.AreEqual(0, (int)addCount);

        aScore = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.AreEqual(520, aScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "a", 321, scoreWhen: RedisSortedSet.ZAddScoreWhen.GT);
        Assert.AreEqual(0, (int)addCount);

        aScore = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.AreEqual(520, aScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "a", 1314, scoreWhen: RedisSortedSet.ZAddScoreWhen.GT, ch: true);
        Assert.AreEqual(1, (int)addCount);

        aScore = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.AreEqual(1314, aScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "a", 1, incr: true);
        Assert.AreEqual(1315, (int)addCount);

        aScore = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.AreEqual(1315, aScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "a", 1, ch: true, incr: true);
        Assert.AreEqual(1316, (int)addCount);

        aScore = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.AreEqual(1316, aScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "b", 1, ch: true);
        Assert.AreEqual(1, (int)addCount);

        var bScore = await this._redis.SortedSet.ZScoreAsync(key1, "b");
        Assert.AreEqual(1, (double)bScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, "b", 1, ch: true);
        Assert.AreEqual(0, (int)addCount);

        bScore = await this._redis.SortedSet.ZScoreAsync(key1, "b");
        Assert.AreEqual(1, (double)bScore);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", -10 },
            { "c", 321 },
            { "d", 3.14 },
            { "e", 9527 },
        });
        Assert.AreEqual(3, (int)addCount);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", -13 },
            { "f", 1.11 },
            { "g", 2.22 },
            { "h", 9.99 },
        }, ch: true);
        Assert.AreEqual(4, (int)addCount);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", -130 },
            { "f", 10.11 },
            { "g", 22.22 },
            { "h", 99.99 },
        }, when: RedisSortedSet.ZAddWhen.NX);
        Assert.AreEqual(0, (int)addCount);

        var fScore = await this._redis.SortedSet.ZScoreAsync(key1, "f");
        Assert.AreEqual(1.11, (double)fScore);

        var total = await this._redis.SortedSet.ZCardAsync(key1);
        Assert.AreEqual(8, total);

        addCount = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", -130 },
            { "f", 10.11 },
            { "g", 22.22 },
            { "h", 99.99 },
            { "i", 99.99 },
        }, when: RedisSortedSet.ZAddWhen.NX);
        Assert.AreEqual(1, (int)addCount);

        total = await this._redis.SortedSet.ZCardAsync(key1);
        Assert.AreEqual(9, total);
    }

    [Test]
    public async Task ZCountAsync()
    {
        const string key1 = "{83}:SortedSet:ZCountAsync:Key1";

        var count = await this._redis.SortedSet.ZCountAsync(key1, 0, 100);
        Assert.AreEqual(0, count);

        var addCount = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 11 },
            { "c", 22 },
            { "d", double.NegativeInfinity },
        }, when: RedisSortedSet.ZAddWhen.NX);
        Assert.AreEqual(4, (int)addCount);

        count = await this._redis.SortedSet.ZCountAsync(key1, 0, 100);
        Assert.AreEqual(3, count);

        count = await this._redis.SortedSet.ZCountAsync(key1, double.NegativeInfinity, double.PositiveInfinity);
        Assert.AreEqual(4, count);
    }

    [Test]
    public async Task ZDiffStoreAsync()
    {
        const string key1 = "{84}:SortedSet:ZDiffStoreAsync:Key1";
        const string key2 = "{84}:SortedSet:ZDiffStoreAsync:Key2";
        const string key3 = "{84}:SortedSet:ZDiffStoreAsync:Key3";
        const string destinationKey = "{84}:SortedSet:ZDiffStoreAsync:Destination";

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 11 },
            { "c", 22 },
            { "d", 33.33 },
        });
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.SortedSet.ZAdd(key2, "c", 3.141592653);
        Assert.That(count, Is.EqualTo(1));

        count = await this._redis.SortedSet.ZAddAsync(key3, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "c", 22 },
            { "e", 43.521 },
        });
        Assert.That(count, Is.EqualTo(3));

        var resultCount = await this._redis.SortedSet.ZDiffStoreAsync(destinationKey, key1, key2);
        Assert.That(resultCount, Is.EqualTo(3));

        var result = await this._redis.SortedSet.ZRangeAsync(destinationKey, 0, -1);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("a"));
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("d"));
        }

        var dict = await this._redis.SortedSet.ZRangeWithScoresAsync(destinationKey, 0, -1);
        Assert.That(dict, Is.Not.Null);
        Assert.That(dict, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dict.Any(f => f.Key == "a"), Is.True);
            Assert.That(dict.Any(f => f.Key == "b"), Is.True);
            Assert.That(dict.Any(f => f.Key == "d"), Is.True);
            Assert.That((double)dict.Single(f => f.Key == "a").Value, Is.EqualTo(1));
            Assert.That((double)dict.Single(f => f.Key == "b").Value, Is.EqualTo(11));
            Assert.That((double)dict.Single(f => f.Key == "d").Value, Is.EqualTo(33.33));
        }

        resultCount = await this._redis.SortedSet.ZDiffStoreAsync(destinationKey, [key1, key2, key3]);
        Assert.That(resultCount, Is.EqualTo(2));

        result = await this._redis.SortedSet.ZRangeAsync(destinationKey, 0, -1);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("d"));
        }

        dict = await this._redis.SortedSet.ZRangeWithScoresAsync(destinationKey, 0, -1);
        Assert.That(dict, Is.Not.Null);
        Assert.That(dict, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dict.Any(f => f.Key == "b"), Is.True);
            Assert.That(dict.Any(f => f.Key == "d"), Is.True);
            Assert.That((double)dict.Single(f => f.Key == "b").Value, Is.EqualTo(11));
            Assert.That((double)dict.Single(f => f.Key == "d").Value, Is.EqualTo(33.33));
        }

        resultCount = await this._redis.SortedSet.ZDiffStoreAsync(destinationKey, key1, "SortedSet:ZDiff:None2");
        Assert.That(resultCount, Is.EqualTo(4));

        result = await this._redis.SortedSet.ZRangeAsync(destinationKey, 0, -1);
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("a"));
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("c"));
            Assert.That(result.Contains("d"));
        }
    }

    [Test]
    public async Task ZIncrByAsync()
    {
        const string key1 = "{85}:SortedSet:ZIncrByAsync:Key1";

        var score = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        Assert.That(score.HasValue, Is.False);

        int count = await this._redis.SortedSet.ZAddAsync(key1, "a", 3.141592653);
        Assert.That(count, Is.EqualTo(1));

        score = await this._redis.SortedSet.ZScoreAsync(key1, "a");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(score.HasValue, Is.True);
            Assert.That((double)score, Is.EqualTo(3.141592653));
        }

        score = await this._redis.SortedSet.ZIncrByAsync(key1, "a", 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(score.HasValue, Is.True);
            Assert.That((double)score, Is.EqualTo(4.141592653));
        }

        score = await this._redis.SortedSet.ZIncrByAsync(key1, "a", -1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(score.HasValue, Is.True);
            Assert.That((double)score, Is.EqualTo(3.141592653));
        }

        score = await this._redis.SortedSet.ZIncrByAsync(key1, "a", double.PositiveInfinity);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(score.HasValue, Is.True);
            Assert.That((double)score, Is.EqualTo(double.PositiveInfinity));
        }

        score = await this._redis.SortedSet.ZIncrByAsync(key1, "b", 1314);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(score.HasValue, Is.True);
            Assert.That((double)score, Is.EqualTo(1314));
        }
    }

    [Test]
    public async Task ZInterAsync()
    {
        const string key1 = "{86}:SortedSet:ZInterAsync:Key1";
        const string key2 = "{86}:SortedSet:ZInterAsync:Key2";
        const string key3 = "{86}:SortedSet:ZInterAsync:Key3";

        var members = await this._redis.SortedSet.ZInterAsync(key1, key2);
        Assert.That(members, Is.Null);

        members = await this._redis.SortedSet.ZInterAsync([key1, key2, key3]);
        Assert.That(members, Is.Null);

        var memberScores = await this._redis.SortedSet.ZInterWithScoresAsync(key1, key2);
        Assert.That(memberScores, Is.Null);

        memberScores = await this._redis.SortedSet.ZInterWithScoresAsync([key1, key2, key3]);
        Assert.That(memberScores, Is.Null);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        count = this._redis.SortedSet.ZAdd(key2, new Dictionary<SharpValue, double>
        {
            { "one", 11 },
            { "four", 44 },
            { "five", 55 },
            { "six", 66 },
        });
        Assert.That(count, Is.EqualTo(4));

        count = await this._redis.SortedSet.ZAddAsync(key3, new Dictionary<SharpValue, double>
        {
            { "one", 111 },
            { "five", 555 },
        });
        Assert.That(count, Is.EqualTo(2));

        members = await this._redis.SortedSet.ZInterAsync(key1, key2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(members, Is.Not.Null);
            Assert.That(members, Has.Length.EqualTo(3));
            Assert.That(members.Contains("one"));
            Assert.That(members.Contains("four"));
            Assert.That(members.Contains("five"));
        }

        memberScores = await this._redis.SortedSet.ZInterWithScoresAsync(key1, key2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScores, Is.Not.Null);
            Assert.That(memberScores, Has.Length.EqualTo(3));
            Assert.That(memberScores!.Any(static f => f.Key == "one"));
            Assert.That(memberScores!.Any(static f => f.Key == "four"));
            Assert.That(memberScores!.Any(static f => f.Key == "five"));
            Assert.That((double)memberScores!.Single(static f => f.Key == "one").Value, Is.EqualTo(12));
            Assert.That((double)memberScores!.Single(static f => f.Key == "four").Value, Is.EqualTo(48));
            Assert.That((double)memberScores!.Single(static f => f.Key == "five").Value, Is.EqualTo(60));
        }

        memberScores = await this._redis.SortedSet.ZInterWithScoresAsync(key1, key2, aggregate: RedisSortedSet.Aggregate.Min);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScores, Is.Not.Null);
            Assert.That(memberScores, Has.Length.EqualTo(3));
            Assert.That(memberScores!.Any(static f => f.Key == "one"));
            Assert.That(memberScores!.Any(static f => f.Key == "four"));
            Assert.That(memberScores!.Any(static f => f.Key == "five"));
            Assert.That((double)memberScores!.Single(static f => f.Key == "one").Value, Is.EqualTo(1));
            Assert.That((double)memberScores!.Single(static f => f.Key == "four").Value, Is.EqualTo(4));
            Assert.That((double)memberScores!.Single(static f => f.Key == "five").Value, Is.EqualTo(5));
        }

        memberScores = await this._redis.SortedSet.ZInterWithScoresAsync(key1, key2, key1Weight: 100, aggregate: RedisSortedSet.Aggregate.Max);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScores, Is.Not.Null);
            Assert.That(memberScores, Has.Length.EqualTo(3));
            Assert.That(memberScores!.Any(static f => f.Key == "one"));
            Assert.That(memberScores!.Any(static f => f.Key == "four"));
            Assert.That(memberScores!.Any(static f => f.Key == "five"));
            Assert.That((double)memberScores!.Single(static f => f.Key == "one").Value, Is.EqualTo(100));
            Assert.That((double)memberScores!.Single(static f => f.Key == "four").Value, Is.EqualTo(400));
            Assert.That((double)memberScores!.Single(static f => f.Key == "five").Value, Is.EqualTo(500));
        }

        memberScores = await this._redis.SortedSet.ZInterWithScoresAsync([key3, key1, key2]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScores, Is.Not.Null);
            Assert.That(memberScores, Has.Length.EqualTo(2));
            Assert.That(memberScores!.Any(static f => f.Key == "one"));
            Assert.That(memberScores!.Any(static f => f.Key == "five"));
            Assert.That((double)memberScores!.Single(static f => f.Key == "one").Value, Is.EqualTo(123));
            Assert.That((double)memberScores!.Single(static f => f.Key == "five").Value, Is.EqualTo(615));
        }

        memberScores = await this._redis.SortedSet.ZInterWithScoresAsync([key3, key1, key2], [3.3, 1.1, 2.2]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScores, Is.Not.Null);
            Assert.That(memberScores, Has.Length.EqualTo(2));
            Assert.That(memberScores!.Any(static f => f.Key == "one"));
            Assert.That(memberScores!.Any(static f => f.Key == "five"));
            Assert.That((double)memberScores!.Single(static f => f.Key == "one").Value, Is.EqualTo((1 * 1.1) + (11 * 2.2) + (111 * 3.3)));
            Assert.That((double)memberScores!.Single(static f => f.Key == "five").Value, Is.EqualTo((5 * 1.1) + (55 * 2.2) + (555 * 3.3)));
        }

        memberScores = await this._redis.SortedSet.ZInterWithScoresAsync([key3, key1, key2], [3.3, 1.1, 2.2], RedisSortedSet.Aggregate.Max);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScores, Is.Not.Null);
            Assert.That(memberScores, Has.Length.EqualTo(2));
            Assert.That(memberScores!.Any(static f => f.Key == "one"));
            Assert.That(memberScores!.Any(static f => f.Key == "five"));
            Assert.That((double)memberScores!.Single(static f => f.Key == "one").Value, Is.EqualTo((111 * 3.3)));
            Assert.That((double)memberScores!.Single(static f => f.Key == "five").Value, Is.EqualTo((555 * 3.3)));
        }
    }

    [Test]
    public async Task ZInterCardAsync()
    {
        const string key1 = "{87}:SortedSet:ZInterCardAsync:Key1";
        const string key2 = "{87}:SortedSet:ZInterCardAsync:Key2";
        const string key3 = "{87}:SortedSet:ZInterCardAsync:Key3";

        var members = await this._redis.SortedSet.ZInterCardAsync(key1, key2);
        Assert.That(members, Is.Zero);

        members = await this._redis.SortedSet.ZInterCardAsync([key1, key2, key3]);
        Assert.That(members, Is.Zero);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        count = await this._redis.SortedSet.ZAddAsync(key2, new Dictionary<SharpValue, double>
        {
            { "one", 11 },
            { "four", 44 },
            { "five", 55 },
            { "six", 66 },
        });
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.SortedSet.ZAdd(key3, new Dictionary<SharpValue, double>
        {
            { "one", 111 },
            { "five", 555 },
        });
        Assert.That(count, Is.EqualTo(2));

        members = await this._redis.SortedSet.ZInterCardAsync(key1, key2);
        Assert.That(members, Is.EqualTo(3));

        members = await this._redis.SortedSet.ZInterCardAsync(key1, key2, 2);
        Assert.That(members, Is.EqualTo(2));

        members = await this._redis.SortedSet.ZInterCardAsync([key1, key2, key3], 2);
        Assert.That(members, Is.EqualTo(2));

        members = await this._redis.SortedSet.ZInterCardAsync([key1, key2, key3], 1);
        Assert.That(members, Is.EqualTo(1));
    }

    [Test]
    public async Task ZInterStoreAsync()
    {
        const string key1 = "{88}:SortedSet:ZInterStoreAsync:Key1";
        const string key2 = "{88}:SortedSet:ZInterStoreAsync:Key2";
        const string key3 = "{88}:SortedSet:ZInterStoreAsync:Key3";
        const string destinationKey = "{88}:SortedSet:ZInterStoreAsync:Destination";

        var members = await this._redis.SortedSet.ZInterStoreAsync(destinationKey, key1, key2);
        Assert.That(members, Is.Zero);

        members = await this._redis.SortedSet.ZInterStoreAsync(destinationKey, [key1, key2, key3]);
        Assert.That(members, Is.Zero);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        count = await this._redis.SortedSet.ZAddAsync(key2, new Dictionary<SharpValue, double>
        {
            { "one", 11 },
            { "four", 44 },
            { "five", 55 },
            { "six", 66 },
        });
        Assert.That(count, Is.EqualTo(4));

        count = await this._redis.SortedSet.ZAddAsync(key3, new Dictionary<SharpValue, double>
        {
            { "one", 111 },
            { "five", 555 },
        });
        Assert.That(count, Is.EqualTo(2));

        members = await this._redis.SortedSet.ZInterStoreAsync(destinationKey, key1, key2);
        Assert.That(members, Is.EqualTo(3));

        double oneScore = await this._redis.SortedSet.ZScoreAsync(destinationKey, "one");
        Assert.That(oneScore, Is.EqualTo(12));

        double fourScore = await this._redis.SortedSet.ZScoreAsync(destinationKey, "four");
        Assert.That(fourScore, Is.EqualTo(48));

        double fiveScore = this._redis.SortedSet.ZScore(destinationKey, "five");
        Assert.That(fiveScore, Is.EqualTo(60));

        members = await this._redis.SortedSet.ZInterStoreAsync(destinationKey, key1, key2, 10, 10, aggregate: RedisSortedSet.Aggregate.Max);
        Assert.That(members, Is.EqualTo(3));

        oneScore = await this._redis.SortedSet.ZScoreAsync(destinationKey, "one");
        Assert.That(oneScore, Is.EqualTo(110));

        fourScore = await this._redis.SortedSet.ZScoreAsync(destinationKey, "four");
        Assert.That(fourScore, Is.EqualTo(440));

        fiveScore = await this._redis.SortedSet.ZScoreAsync(destinationKey, "five");
        Assert.That(fiveScore, Is.EqualTo(550));

        members = await this._redis.SortedSet.ZInterStoreAsync(destinationKey, [key1, key2, key3]);
        Assert.That(members, Is.EqualTo(2));

        oneScore = await this._redis.SortedSet.ZScoreAsync(destinationKey, "one");
        Assert.That(oneScore, Is.EqualTo(123));

        fiveScore = await this._redis.SortedSet.ZScoreAsync(destinationKey, "five");
        Assert.That(fiveScore, Is.EqualTo(615));
    }

    [Test]
    public async Task ZLexCountAsync()
    {
        const string key1 = "{89}:SortedSet:ZLexCountAsync:Key1";

        var count = await this._redis.SortedSet.ZLexCountAsync(key1, "a", "z");
        Assert.That(count, Is.Zero);

        int result = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 1 },
            { "c", 1 },
            { "d", 1 },
            { "e", 1 },
            { "f", 1 },
            { "g", 1 },
            { "h", 1 },
            { "i", 1 },
            { "j", 1 },
            { "k", 1 },
            { "l", 1 },
        });
        Assert.That(result, Is.EqualTo(12));

        count = await this._redis.SortedSet.ZLexCountAsync(key1, "a", "z");
        Assert.That(count, Is.EqualTo(12));

        count = await this._redis.SortedSet.ZLexCountAsync(key1, "a", "d");
        Assert.That(count, Is.EqualTo(4));

        count = await this._redis.SortedSet.ZLexCountAsync(key1, "a", ("f", false));
        Assert.That(count, Is.EqualTo(5));

        count = await this._redis.SortedSet.ZLexCountAsync(key1, "-", "+");
        Assert.That(count, Is.EqualTo(12));
    }

    [Test]
    public async Task ZMPopAsync()
    {
        const string key1 = "{90}:SortedSet:ZMPopAsync:Key1";
        const string key2 = "{90}:SortedSet:ZMPopAsync:Key2";

        var result = await this._redis.SortedSet.ZMPopAsync(key1, key2, RedisSortedSet.PopHow.Min);
        Assert.That(result.IsNull, Is.True);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        result = await this._redis.SortedSet.ZMPopAsync(key1, key2, RedisSortedSet.PopHow.Min);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsNull, Is.False);
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result.Key.ToString(), Does.EndWith(key1));
            var member = result[0];
            Assert.That(member.Key.ToString(), Is.EqualTo("one"));
            Assert.That((double)member.Value, Is.EqualTo(1));
        }

        result = await this._redis.SortedSet.ZMPopAsync(key1, key2, RedisSortedSet.PopHow.Max, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsNull, Is.False);
            Assert.That(result, Has.Length.EqualTo(4));
            Assert.That(result.Key.ToString(), Does.EndWith(key1));
            var member = result[0];
            Assert.That(member.Key.ToString(), Is.EqualTo("five"));
            Assert.That((double)member.Value, Is.EqualTo(5));

            member = result[3];
            Assert.That(member.Key.ToString(), Is.EqualTo("two"));
            Assert.That((double)member.Value, Is.EqualTo(2));
        }

        count = await this._redis.SortedSet.ZAddAsync(key2, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        result = await this._redis.SortedSet.ZMPopAsync([key1, key2], RedisSortedSet.PopHow.Min);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsNull, Is.False);
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result.Key.ToString(), Does.EndWith(key2));
            var member = result[0];
            Assert.That(member.Key.ToString(), Is.EqualTo("one"));
            Assert.That((double)member.Value, Is.EqualTo(1));
        }

        result = await this._redis.SortedSet.ZMPopAsync([key1, key2], RedisSortedSet.PopHow.Max, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsNull, Is.False);
            Assert.That(result, Has.Length.EqualTo(4));
            Assert.That(result.Key.ToString(), Does.EndWith(key2));
            var member = result[0];
            Assert.That(member.Key.ToString(), Is.EqualTo("five"));
            Assert.That((double)member.Value, Is.EqualTo(5));

            member = result[3];
            Assert.That(member.Key.ToString(), Is.EqualTo("two"));
            Assert.That((double)member.Value, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task ZMScoreAsync()
    {
        const string key1 = "{91}:SortedSet:ZMScoreAsync:Key1";

        var scores = await this._redis.SortedSet.ZMScoreAsync(key1, ["one", "two"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(scores, Is.Not.Null);
            Assert.That(scores, Has.Length.EqualTo(2));
            Assert.That(scores![0].HasValue, Is.False);
            Assert.That(scores![1].HasValue, Is.False);
        }

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        scores = await this._redis.SortedSet.ZMScoreAsync(key1, ["one", "two", "none"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(scores, Is.Not.Null);
            Assert.That(scores, Has.Length.EqualTo(3));
            Assert.That(scores![0].HasValue, Is.True);
            Assert.That(scores![1].HasValue, Is.True);
            Assert.That(scores![2].HasValue, Is.False);
            Assert.That((int)scores![0], Is.EqualTo(1));
            Assert.That((int)scores![1], Is.EqualTo(2));
        }
    }

    [Test]
    public async Task ZPopMaxAsync()
    {
        const string key1 = "{92}:SortedSet:ZPopMaxAsync:Key1";

        var single = await this._redis.SortedSet.ZPopMaxAsync(key1);
        Assert.That(single.HasValue, Is.False);

        var array = await this._redis.SortedSet.ZPopMaxAsync(key1, 3);
        Assert.That(array, Is.Null);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        single = await this._redis.SortedSet.ZPopMaxAsync(key1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(single.HasValue, Is.True);
            Assert.That(single!.Value.Key.ToString(), Is.EqualTo("five"));
            Assert.That((int)single.Value.Value, Is.EqualTo(5));
        }

        array = await this._redis.SortedSet.ZPopMaxAsync(key1, 30);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array, Is.Not.Null);
            Assert.That(array, Has.Length.EqualTo(4));
            Assert.That(array![0].Key.ToString(), Is.EqualTo("four"));
            Assert.That((int)array[0].Value, Is.EqualTo(4));

            Assert.That(array![3].Key.ToString(), Is.EqualTo("one"));
            Assert.That((int)array[3].Value, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task ZPopMinAsync()
    {
        const string key1 = "{93}:SortedSet:ZPopMinAsync:Key1";

        var single = await this._redis.SortedSet.ZPopMinAsync(key1);
        Assert.That(single.HasValue, Is.False);

        var array = await this._redis.SortedSet.ZPopMinAsync(key1, 3);
        Assert.That(array, Is.Null);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        single = await this._redis.SortedSet.ZPopMinAsync(key1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(single.HasValue, Is.True);
            Assert.That(single!.Value.Key.ToString(), Is.EqualTo("one"));
            Assert.That((int)single.Value.Value, Is.EqualTo(1));
        }

        array = await this._redis.SortedSet.ZPopMinAsync(key1, 30);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array, Is.Not.Null);
            Assert.That(array, Has.Length.EqualTo(4));
            Assert.That(array![0].Key.ToString(), Is.EqualTo("two"));
            Assert.That((int)array[0].Value, Is.EqualTo(2));

            Assert.That(array![3].Key.ToString(), Is.EqualTo("five"));
            Assert.That((int)array[3].Value, Is.EqualTo(5));
        }
    }

    [Test]
    public async Task ZRandMemberAsync()
    {
        const string key1 = "{94}:SortedSet:ZRandMemberAsync:Key1";

        var single = await this._redis.SortedSet.ZRandMemberAsync(key1);
        Assert.That(single.HasValue, Is.False);

        var singleWithScore = await this._redis.SortedSet.ZRandMemberWithScoresAsync(key1);
        Assert.That(singleWithScore.HasValue, Is.False);

        var array = await this._redis.SortedSet.ZRandMemberAsync(key1, 3);
        Assert.That(array, Is.Null);

        var arrayWithScore = await this._redis.SortedSet.ZRandMemberWithScoresAsync(key1, 3);
        Assert.That(arrayWithScore, Is.Null);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        single = await this._redis.SortedSet.ZRandMemberAsync(key1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(single.HasValue, Is.True);
            Assert.That(single.IsNull, Is.False);
        }

        singleWithScore = await this._redis.SortedSet.ZRandMemberWithScoresAsync(key1);
        Assert.That(singleWithScore.HasValue, Is.True);

        array = await this._redis.SortedSet.ZRandMemberAsync(key1, 3);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array, Is.Not.Null);
            Assert.That(array, Has.Length.EqualTo(3));
        }

        array = await this._redis.SortedSet.ZRandMemberAsync(key1, 0);
        Assert.That(array, Is.Null);

        arrayWithScore = await this._redis.SortedSet.ZRandMemberWithScoresAsync(key1, 3);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(arrayWithScore, Is.Not.Null);
            Assert.That(arrayWithScore, Has.Length.EqualTo(3));
        }

        arrayWithScore = await this._redis.SortedSet.ZRandMemberWithScoresAsync(key1, 0);
        Assert.That(arrayWithScore, Is.Null);

        array = await this._redis.SortedSet.ZRandMemberAsync(key1, 30);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array, Is.Not.Null);
            Assert.That(array, Has.Length.EqualTo(5));
        }

        arrayWithScore = await this._redis.SortedSet.ZRandMemberWithScoresAsync(key1, 30);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(arrayWithScore, Is.Not.Null);
            Assert.That(arrayWithScore, Has.Length.EqualTo(5));
        }

        array = await this._redis.SortedSet.ZRandMemberAsync(key1, -30);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array, Is.Not.Null);
            Assert.That(array, Has.Length.EqualTo(30));
        }

        arrayWithScore = await this._redis.SortedSet.ZRandMemberWithScoresAsync(key1, -30);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(arrayWithScore, Is.Not.Null);
            Assert.That(arrayWithScore, Has.Length.EqualTo(30));
        }
    }

    [Test]
    public async Task ZRangeAsync()
    {
        const string key1 = "{95}:SortedSet:ZRangeAsync:Key1";
        const string key2 = "{95}:SortedSet:ZRangeAsync:Key2";

        var range = await this._redis.SortedSet.ZRangeAsync(key1, 0, -1);
        Assert.That(range, Is.Null);

        var rangeWithScore = await this._redis.SortedSet.ZRangeWithScoresAsync(key1, 0, -1);
        Assert.That(rangeWithScore, Is.Null);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
            { "k", 11 },
            { "l", 12 },
            { "m", 13 },
            { "n", 14 },
            { "o", 15 },
            { "p", 16 },
            { "q", 17 },
            { "r", 18 },
            { "s", 19 },
            { "t", 20 },
            { "u", 21 },
            { "v", 22 },
            { "w", 23 },
            { "x", 24 },
            { "y", 25 },
            { "z", 26 },
        });
        Assert.That(count, Is.EqualTo(26));

        count = await this._redis.SortedSet.ZAddAsync(key2, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 1 },
            { "c", 1 },
            { "d", 1 },
            { "e", 1 },
            { "f", 1 },
            { "g", 1 },
            { "h", 1 },
            { "i", 1 },
            { "j", 1 },
            { "k", 1 },
            { "l", 1 },
            { "m", 1 },
            { "n", 1 },
            { "o", 1 },
            { "p", 1 },
            { "q", 1 },
            { "r", 1 },
            { "s", 1 },
            { "t", 1 },
            { "u", 1 },
            { "v", 1 },
            { "w", 1 },
            { "x", 1 },
            { "y", 1 },
            { "z", 1 },
        });
        Assert.That(count, Is.EqualTo(26));

        range = await this._redis.SortedSet.ZRangeAsync(key1, 0, -1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(26));
            Assert.That(range![0].ToString(), Is.EqualTo("a"));
            Assert.That(range![25].ToString(), Is.EqualTo("z"));
        }

        rangeWithScore = await this._redis.SortedSet.ZRangeWithScoresAsync(key1, 0, -1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(26));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("a"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(1));
            Assert.That(rangeWithScore![25].Key.ToString(), Is.EqualTo("z"));
            Assert.That((double)rangeWithScore![25].Value, Is.EqualTo(26));
        }

        range = await this._redis.SortedSet.ZRangeAsync(key1, 100, 0, by: RedisSortedSet.RangeBy.ByScore, rev: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(26));
            Assert.That(range![0].ToString(), Is.EqualTo("z"));
            Assert.That(range![25].ToString(), Is.EqualTo("a"));
        }

        rangeWithScore = await this._redis.SortedSet.ZRangeWithScoresAsync(key1, 100, 0, by: RedisSortedSet.RangeBy.ByScore, rev: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(26));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("z"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(26));
            Assert.That(rangeWithScore![25].Key.ToString(), Is.EqualTo("a"));
            Assert.That((double)rangeWithScore![25].Value, Is.EqualTo(1));
        }

        range = await this._redis.SortedSet.ZRangeAsync(key1, 0, 200, by: RedisSortedSet.RangeBy.ByScore, rev: false, offset: 1, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(5));
            Assert.That(range![0].ToString(), Is.EqualTo("b"));
            Assert.That(range![4].ToString(), Is.EqualTo("f"));
        }

        rangeWithScore = await this._redis.SortedSet.ZRangeWithScoresAsync(key1, 0, 100, by: RedisSortedSet.RangeBy.ByScore, rev: false, offset: 1, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(5));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("b"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(2));
            Assert.That(rangeWithScore![4].Key.ToString(), Is.EqualTo("f"));
            Assert.That((double)rangeWithScore![4].Value, Is.EqualTo(6));
        }

        range = await this._redis.SortedSet.ZRangeAsync(key2, "a", "h", by: RedisSortedSet.RangeBy.ByLex, rev: false, offset: 1, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(5));
            Assert.That(range![0].ToString(), Is.EqualTo("b"));
            Assert.That(range![4].ToString(), Is.EqualTo("f"));
        }

        range = await this._redis.SortedSet.ZRangeAsync(key2, ("a", false), "h", by: RedisSortedSet.RangeBy.ByLex, rev: false, offset: 1, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(5));
            Assert.That(range![0].ToString(), Is.EqualTo("c"));
            Assert.That(range![4].ToString(), Is.EqualTo("g"));
        }

        rangeWithScore = await this._redis.SortedSet.ZRangeWithScoresAsync(key1, (1, false), (5, false), by: RedisSortedSet.RangeBy.ByScore, rev: false, offset: 1, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(2));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("c"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(3));
            Assert.That(rangeWithScore![1].Key.ToString(), Is.EqualTo("d"));
            Assert.That((double)rangeWithScore![1].Value, Is.EqualTo(4));
        }
    }

    [Test]
    public async Task ZRangeByLexAsync()
    {
        const string key1 = "{96}:SortedSet:ZRangeByLexAsync:Key1";

        var range = await this._redis.SortedSet.ZRangeByLexAsync(key1, "a", "z");
        Assert.That(range, Is.Null);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
            { "k", 11 },
            { "l", 12 },
            { "m", 13 },
            { "n", 14 },
            { "o", 15 },
            { "p", 16 },
            { "q", 17 },
            { "r", 18 },
            { "s", 19 },
            { "t", 20 },
            { "u", 21 },
            { "v", 22 },
            { "w", 23 },
            { "x", 24 },
            { "y", 25 },
            { "z", 26 },
        });
        Assert.That(count, Is.EqualTo(26));

        range = await this._redis.SortedSet.ZRangeByLexAsync(key1, "a", "z");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(26));
            Assert.That(range![0].ToString(), Is.EqualTo("a"));
            Assert.That(range![25].ToString(), Is.EqualTo("z"));
        }

        range = await this._redis.SortedSet.ZRangeByLexAsync(key1, ("a", false), "z");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(25));
            Assert.That(range![0].ToString(), Is.EqualTo("b"));
            Assert.That(range![24].ToString(), Is.EqualTo("z"));
        }

        range = await this._redis.SortedSet.ZRangeByLexAsync(key1, ("a", false), ("z", false), 2, 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(5));
            Assert.That(range![0].ToString(), Is.EqualTo("d"));
            Assert.That(range![4].ToString(), Is.EqualTo("h"));
        }
    }

    [Test]
    public async Task ZRangeByScoreAsync()
    {
        const string key1 = "{97}:SortedSet:ZRangeByScoreAsync:Key1";

        var range = await this._redis.SortedSet.ZRangeByScoreAsync(key1, double.NegativeInfinity, double.PositiveInfinity);
        Assert.That(range, Is.Null);

        var rangeWithScore = await this._redis.SortedSet.ZRangeByScoreWithScoresAsync(key1, double.NegativeInfinity, double.PositiveInfinity);
        Assert.That(rangeWithScore, Is.Null);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
            { "k", 11 },
            { "l", 12 },
            { "m", 13 },
            { "n", 14 },
            { "o", 15 },
            { "p", 16 },
            { "q", 17 },
            { "r", 18 },
            { "s", 19 },
            { "t", 20 },
            { "u", 21 },
            { "v", 22 },
            { "w", 23 },
            { "x", 24 },
            { "y", 25 },
            { "z", 26 },
        });
        Assert.That(count, Is.EqualTo(26));

        range = await this._redis.SortedSet.ZRangeByScoreAsync(key1, double.NegativeInfinity, double.PositiveInfinity);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(26));
            Assert.That(range![0].ToString(), Is.EqualTo("a"));
            Assert.That(range![25].ToString(), Is.EqualTo("z"));
        }

        range = await this._redis.SortedSet.ZRangeByScoreAsync(key1, (5, false), double.PositiveInfinity);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(21));
            Assert.That(range![0].ToString(), Is.EqualTo("f"));
            Assert.That(range![20].ToString(), Is.EqualTo("z"));
        }

        range = await this._redis.SortedSet.ZRangeByScoreAsync(key1, (5, false), double.PositiveInfinity, 3, 3);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(3));
            Assert.That(range![0].ToString(), Is.EqualTo("i"));
            Assert.That(range![2].ToString(), Is.EqualTo("k"));
        }

        rangeWithScore = await this._redis.SortedSet.ZRangeByScoreWithScoresAsync(key1, double.NegativeInfinity, double.PositiveInfinity);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(26));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("a"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(1));
            Assert.That(rangeWithScore![25].Key.ToString(), Is.EqualTo("z"));
            Assert.That((double)rangeWithScore![25].Value, Is.EqualTo(26));
        }

        rangeWithScore = await this._redis.SortedSet.ZRangeByScoreWithScoresAsync(key1, double.NegativeInfinity, double.PositiveInfinity, 1, 100);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(25));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("b"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(2));
            Assert.That(rangeWithScore![24].Key.ToString(), Is.EqualTo("z"));
            Assert.That((double)rangeWithScore![24].Value, Is.EqualTo(26));
        }

        rangeWithScore = await this._redis.SortedSet.ZRangeByScoreWithScoresAsync(key1, (2, false), (10, false), 1, 100);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(6));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("d"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(4));
            Assert.That(rangeWithScore![5].Key.ToString(), Is.EqualTo("i"));
            Assert.That((double)rangeWithScore![5].Value, Is.EqualTo(9));
        }
    }

    [Test]
    public async Task ZRangeStoreAsync()
    {
        const string key1 = "{98}:SortedSet:ZRangeStoreAsync:Key1";
        const string destinationKey = "{98}:SortedSet:ZRangeStoreAsync:DestinationKey";

        var result = await this._redis.SortedSet.ZRangeStoreAsync(destinationKey, key1, "-", "+", RedisSortedSet.RangeBy.ByLex);
        Assert.That(result, Is.Zero);

        var length = await this._redis.SortedSet.ZCardAsync(destinationKey);
        Assert.That(length, Is.Zero);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
            { "k", 11 },
            { "l", 12 },
            { "m", 13 },
            { "n", 14 },
            { "o", 15 },
            { "p", 16 },
            { "q", 17 },
            { "r", 18 },
            { "s", 19 },
            { "t", 20 },
            { "u", 21 },
            { "v", 22 },
            { "w", 23 },
            { "x", 24 },
            { "y", 25 },
            { "z", 26 },
        });
        Assert.That(count, Is.EqualTo(26));

        result = await this._redis.SortedSet.ZRangeStoreAsync(destinationKey, key1, "-", "+", RedisSortedSet.RangeBy.ByLex);
        Assert.That(result, Is.EqualTo(26));

        length = await this._redis.SortedSet.ZCardAsync(destinationKey);
        Assert.That(length, Is.EqualTo(26));

        result = await this._redis.SortedSet.ZRangeStoreAsync(destinationKey, key1, 100, 0, RedisSortedSet.RangeBy.ByScore, true);
        Assert.That(result, Is.EqualTo(26));

        length = this._redis.SortedSet.ZCard(destinationKey);
        Assert.That(length, Is.EqualTo(26));

        result = await this._redis.SortedSet.ZRangeStoreAsync(destinationKey, key1, 0, 100, RedisSortedSet.RangeBy.ByScore, offset: 1, count: 5);
        Assert.That(result, Is.EqualTo(5));

        length = await this._redis.SortedSet.ZCardAsync(destinationKey);
        Assert.That(length, Is.EqualTo(5));
    }

    [Test]
    public async Task ZRankAsync()
    {
        const string key1 = "{99}:SortedSet:ZRankAsync:Key1";

        var rank = await this._redis.SortedSet.ZRankAsync(key1, "c");
        Assert.Null(rank);

        var rankScore = await this._redis.SortedSet.ZRankWithScoreAsync(key1, "g");
        Assert.True(rankScore.IsNull);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
            { "k", 11 },
            { "l", 12 },
            { "m", 13 },
            { "n", 14 },
            { "o", 15 },
            { "p", 16 },
            { "q", 17 },
            { "r", 18 },
            { "s", 19 },
            { "t", 20 },
            { "u", 21 },
            { "v", 22 },
            { "w", 23 },
            { "x", 24 },
            { "y", 25 },
            { "z", 26 },
        });
        Assert.That(count, Is.EqualTo(26));

        rank = await this._redis.SortedSet.ZRankAsync(key1, "cc");
        Assert.Null(rank);

        rankScore = await this._redis.SortedSet.ZRankWithScoreAsync(key1, "gg");
        Assert.True(rankScore.IsNull);

        rank = await this._redis.SortedSet.ZRankAsync(key1, "c");
        Assert.AreEqual(rank, 2);

        rankScore = await this._redis.SortedSet.ZRankWithScoreAsync(key1, "g");
        Assert.False(rankScore.IsNull);
        (var r, var score) = rankScore;
        Assert.AreEqual(r, 6);
        Assert.AreEqual((int)score, 7);
    }

    [Test]
    public async Task ZRemAsync()
    {
        const string key1 = "{100}:SortedSet:ZRemAsync:Key1";

        var result = await this._redis.SortedSet.ZRemAsync(key1, "a");
        Assert.Zero(result);

        result = await this._redis.SortedSet.ZRemAsync(key1, ["a", "b"]);
        Assert.Zero(result);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
        });
        Assert.That(count, Is.EqualTo(10));

        result = await this._redis.SortedSet.ZRemAsync(key1, "a");
        Assert.AreEqual(1, result);

        result = await this._redis.SortedSet.ZRemAsync(key1, ["a", "b", "c"]);
        Assert.AreEqual(2, result);

        var length = await this._redis.SortedSet.ZCardAsync(key1);
        Assert.AreEqual(7, length);
    }

    [Test]
    public async Task ZRemRangeByLexAsync()
    {
        const string key1 = "{101}:SortedSet:ZRemRangeByLexAsync:Key1";

        var result = await this._redis.SortedSet.ZRemRangeByLexAsync(key1, "a", "z");
        Assert.Zero(result);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
        });
        Assert.That(count, Is.EqualTo(10));

        result = await this._redis.SortedSet.ZRemRangeByLexAsync(key1, "a", "d");
        Assert.AreEqual(4, result);

        result = await this._redis.SortedSet.ZRemRangeByLexAsync(key1, "a", ("j", false));
        Assert.AreEqual(5, result);

        var length = await this._redis.SortedSet.ZCardAsync(key1);
        Assert.AreEqual(1, length);
    }

    [Test]
    public async Task ZRemRangeByRankAsync()
    {
        const string key1 = "{102}:SortedSet:ZRemRangeByRankAsync:Key1";

        var result = await this._redis.SortedSet.ZRemRangeByRankAsync(key1, 0, -1);
        Assert.Zero(result);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
        });
        Assert.That(count, Is.EqualTo(10));

        result = await this._redis.SortedSet.ZRemRangeByRankAsync(key1, 0, 3);
        Assert.AreEqual(4, result);

        result = await this._redis.SortedSet.ZRemRangeByRankAsync(key1, 0, 4);
        Assert.AreEqual(5, result);

        var length = await this._redis.SortedSet.ZCardAsync(key1);
        Assert.AreEqual(1, length);
    }

    [Test]
    public async Task ZRemRangeByScoreAsync()
    {
        const string key1 = "{103}:SortedSet:ZRemRangeByScoreAsync:Key1";

        var result = await this._redis.SortedSet.ZRemRangeByScoreAsync(key1, 0, 100);
        Assert.Zero(result);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
        });
        Assert.That(count, Is.EqualTo(10));

        result = await this._redis.SortedSet.ZRemRangeByScoreAsync(key1, 0, 3);
        Assert.AreEqual(3, result);

        result = await this._redis.SortedSet.ZRemRangeByScoreAsync(key1, 0, (10, false));
        Assert.AreEqual(6, result);

        var length = await this._redis.SortedSet.ZCardAsync(key1);
        Assert.AreEqual(1, length);
    }

    [Test]
    public async Task ZRevRangeAsync()
    {
        const string key1 = "{104}:SortedSet:ZRevRangeAsync:Key1";

        var range = await this._redis.SortedSet.ZRevRangeAsync(key1, 0, 100);
        Assert.Null(range);

        var rangeScore = await this._redis.SortedSet.ZRevRangeWithScoresAsync(key1, 0, 100);
        Assert.Null(rangeScore);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
        });
        Assert.That(count, Is.EqualTo(10));

        range = await this._redis.SortedSet.ZRevRangeAsync(key1, 1, 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.NotNull(range);
            Assert.That(range, Has.Length.EqualTo(5));
            Assert.That(range![0].ToString(), Is.EqualTo("i"));
            Assert.That(range![4].ToString(), Is.EqualTo("e"));
        }

        rangeScore = await this._redis.SortedSet.ZRevRangeWithScoresAsync(key1, 0, -2);
        using (Assert.EnterMultipleScope())
        {
            Assert.NotNull(rangeScore);
            Assert.That(rangeScore, Has.Length.EqualTo(9));
            Assert.That(rangeScore![0].Key.ToString(), Is.EqualTo("j"));
            Assert.That((int)rangeScore![0].Value, Is.EqualTo(10));
            Assert.That(rangeScore![8].Key.ToString(), Is.EqualTo("b"));
            Assert.That((int)rangeScore![8].Value, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task ZRevRangeByLexAsync()
    {
        const string key1 = "{105}:SortedSet:ZRevRangeByLexAsync:Key1";

        var range = await this._redis.SortedSet.ZRevRangeByLexAsync(key1, "c", "-");
        Assert.Null(range);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
        });
        Assert.That(count, Is.EqualTo(10));

        range = await this._redis.SortedSet.ZRevRangeByLexAsync(key1, "c", "-");
        Assert.That(range, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("c"));
            Assert.That(range[2].ToString(), Is.EqualTo("a"));
        }

        range = await this._redis.SortedSet.ZRevRangeByLexAsync(key1, "+", "-");
        Assert.That(range, Has.Length.EqualTo(10));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("j"));
            Assert.That(range[9].ToString(), Is.EqualTo("a"));
        }

        range = await this._redis.SortedSet.ZRevRangeByLexAsync(key1, ("j", false), "d");
        Assert.That(range, Has.Length.EqualTo(6));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("i"));
            Assert.That(range[5].ToString(), Is.EqualTo("d"));
        }

        range = await this._redis.SortedSet.ZRevRangeByLexAsync(key1, ("j", false), ("d", false));
        Assert.That(range, Has.Length.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("i"));
            Assert.That(range[4].ToString(), Is.EqualTo("e"));
        }
    }

    [Test]
    public async Task ZRevRangeByScoreAsync()
    {
        const string key1 = "{106}:SortedSet:ZRevRangeByScoreAsync:Key1";

        var range = await this._redis.SortedSet.ZRevRangeByScoreAsync(key1, double.PositiveInfinity, double.NegativeInfinity);
        Assert.Null(range);

        var rangeScore = await this._redis.SortedSet.ZRevRangeByScoreWithScoresAsync(key1, double.PositiveInfinity, double.NegativeInfinity);
        Assert.Null(rangeScore);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
        });
        Assert.That(count, Is.EqualTo(10));

        range = await this._redis.SortedSet.ZRevRangeByScoreAsync(key1, double.PositiveInfinity, double.NegativeInfinity);
        Assert.NotNull(range);
        Assert.That(range, Has.Length.EqualTo(10));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("j"));
            Assert.That(range[9].ToString(), Is.EqualTo("a"));
        }

        range = await this._redis.SortedSet.ZRevRangeByScoreAsync(key1, double.PositiveInfinity, double.NegativeInfinity, 1, 3);
        Assert.NotNull(range);
        Assert.That(range, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("i"));
            Assert.That(range[2].ToString(), Is.EqualTo("g"));
        }

        range = await this._redis.SortedSet.ZRevRangeByScoreAsync(key1, (8, false), double.NegativeInfinity, 0, 100);
        Assert.NotNull(range);
        Assert.That(range, Has.Length.EqualTo(7));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("g"));
            Assert.That(range[6].ToString(), Is.EqualTo("a"));
        }

        rangeScore = await this._redis.SortedSet.ZRevRangeByScoreWithScoresAsync(key1, double.PositiveInfinity, double.NegativeInfinity);
        Assert.NotNull(rangeScore);
        Assert.That(rangeScore, Has.Length.EqualTo(10));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeScore[0].Key.ToString(), Is.EqualTo("j"));
            Assert.That((int)rangeScore[0].Value, Is.EqualTo(10));
            Assert.That(rangeScore[9].Key.ToString(), Is.EqualTo("a"));
            Assert.That((int)rangeScore[9].Value, Is.EqualTo(1));
        }

        rangeScore = await this._redis.SortedSet.ZRevRangeByScoreWithScoresAsync(key1, double.PositiveInfinity, double.NegativeInfinity, 1, 3);
        Assert.NotNull(rangeScore);
        Assert.That(rangeScore, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeScore[0].Key.ToString(), Is.EqualTo("i"));
            Assert.That((int)rangeScore[0].Value, Is.EqualTo(9));
            Assert.That(rangeScore[2].Key.ToString(), Is.EqualTo("g"));
            Assert.That((int)rangeScore[2].Value, Is.EqualTo(7));
        }

        rangeScore = await this._redis.SortedSet.ZRevRangeByScoreWithScoresAsync(key1, (8, false), double.NegativeInfinity, 0, 100);
        Assert.NotNull(rangeScore);
        Assert.That(rangeScore, Has.Length.EqualTo(7));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeScore[0].Key.ToString(), Is.EqualTo("g"));
            Assert.That((int)rangeScore[0].Value, Is.EqualTo(7));
            Assert.That(rangeScore[6].Key.ToString(), Is.EqualTo("a"));
            Assert.That((int)rangeScore[6].Value, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task ZRevRankAsync()
    {
        const string key1 = "{107}:SortedSet:ZRevRankAsync:Key1";

        var rank = await this._redis.SortedSet.ZRevRankAsync(key1, "c");
        Assert.Null(rank);

        var rankScore = await this._redis.SortedSet.ZRevRankWithScoreAsync(key1, "g");
        Assert.True(rankScore.IsNull);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "e", 5 },
            { "f", 6 },
            { "g", 7 },
            { "h", 8 },
            { "i", 9 },
            { "j", 10 },
            { "k", 11 },
            { "l", 12 },
            { "m", 13 },
            { "n", 14 },
            { "o", 15 },
            { "p", 16 },
            { "q", 17 },
            { "r", 18 },
            { "s", 19 },
            { "t", 20 },
            { "u", 21 },
            { "v", 22 },
            { "w", 23 },
            { "x", 24 },
            { "y", 25 },
            { "z", 26 },
        });
        Assert.That(count, Is.EqualTo(26));

        rank = await this._redis.SortedSet.ZRevRankAsync(key1, "cc");
        Assert.Null(rank);

        rankScore = await this._redis.SortedSet.ZRevRankWithScoreAsync(key1, "gg");
        Assert.True(rankScore.IsNull);

        rank = await this._redis.SortedSet.ZRevRankAsync(key1, "c");
        Assert.AreEqual(rank, 23);

        rankScore = await this._redis.SortedSet.ZRevRankWithScoreAsync(key1, "g");
        Assert.False(rankScore.IsNull);
        (var r, var score) = rankScore;
        Assert.AreEqual(r, 19);
        Assert.AreEqual((int)score, 7);
    }

    [Test]
    public async Task ZUnionAsync()
    {
        const string key1 = "{108}:SortedSet:ZUnionAsync:Key1";
        const string key2 = "{108}:SortedSet:ZUnionAsync:Key2";
        const string key3 = "{108}:SortedSet:ZUnionAsync:Key3";

        var members = await this._redis.SortedSet.ZUnionAsync(key1, key2);
        Assert.Null(members);

        members = await this._redis.SortedSet.ZUnionAsync([key1, key2, key3]);
        Assert.Null(members);

        var memberScore = await this._redis.SortedSet.ZUnionWithScoresAsync(key1, key2);
        Assert.Null(memberScore);

        memberScore = await this._redis.SortedSet.ZUnionWithScoresAsync([key1, key2, key3]);
        Assert.Null(memberScore);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
        });
        Assert.That(count, Is.EqualTo(2));

        count = await this._redis.SortedSet.ZAddAsync(key2, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        count = await this._redis.SortedSet.ZAddAsync(key3, new Dictionary<SharpValue, double>
        {
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        members = await this._redis.SortedSet.ZUnionAsync(key1, key2);
        Assert.NotNull(members);
        Assert.That(members, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(members.Contains("one"));
            Assert.That(members.Contains("two"));
            Assert.That(members.Contains("three"));
        }

        members = await this._redis.SortedSet.ZUnionAsync([key1, key2, key3]);
        Assert.NotNull(members);
        Assert.That(members, Has.Length.EqualTo(6));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(members.Contains("one"));
            Assert.That(members.Contains("two"));
            Assert.That(members.Contains("three"));
            Assert.That(members.Contains("1"));
            Assert.That(members.Contains("2"));
            Assert.That(members.Contains("3"));
        }

        memberScore = await this._redis.SortedSet.ZUnionWithScoresAsync(key1, key3);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(1));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(1));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(3));
        }

        memberScore = await this._redis.SortedSet.ZUnionWithScoresAsync(key1, key3, 2, 3);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(4));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(9));
        }

        memberScore = await this._redis.SortedSet.ZUnionWithScoresAsync(key1, key3, 2, 3, RedisSortedSet.Aggregate.Max);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(4));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(9));
        }

        memberScore = await this._redis.SortedSet.ZUnionWithScoresAsync(key1, key2, 5, 3, RedisSortedSet.Aggregate.Min);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "three"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "three").Value, Is.EqualTo(9));
        }

        memberScore = await this._redis.SortedSet.ZUnionWithScoresAsync([key1, key3, key2], [1, 3, 2]);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(6));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "three"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "three").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(9));
        }

        memberScore = await this._redis.SortedSet.ZUnionWithScoresAsync([key1, key3, key2], [1, 3, 2], RedisSortedSet.Aggregate.Max);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(6));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "three"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(4));
            Assert.That((double)memberScore.Single(f => f.Key == "three").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(9));
        }

        memberScore = await this._redis.SortedSet.ZUnionWithScoresAsync([key1, key3, key2], [1, 3, 2], RedisSortedSet.Aggregate.Min);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(6));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "three"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(1));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "three").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(9));
        }
    }

    [Test]
    public async Task ZUnionStoreAsync()
    {
        const string key1 = "{109}:SortedSet:ZUnionStoreAsync:Key1";
        const string key2 = "{109}:SortedSet:ZUnionStoreAsync:Key2";
        const string key3 = "{109}:SortedSet:ZUnionStoreAsync:Key3";
        const string destinationKey = "{109}:SortedSet:ZUnionStoreAsync:DestinationKey";

        var memberCount = await this._redis.SortedSet.ZUnionStoreAsync(destinationKey, key1, key2);
        Assert.Zero(memberCount);

        memberCount = await this._redis.SortedSet.ZUnionStoreAsync(destinationKey, [key1, key2, key3]);
        Assert.Zero(memberCount);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
        });
        Assert.That(count, Is.EqualTo(2));

        count = await this._redis.SortedSet.ZAddAsync(key2, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        count = await this._redis.SortedSet.ZAddAsync(key3, new Dictionary<SharpValue, double>
        {
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        memberCount = await this._redis.SortedSet.ZUnionStoreAsync(destinationKey, key1, key2);
        Assert.AreEqual(memberCount, 3);

        var members = await this._redis.SortedSet.ZRangeAsync(destinationKey, 0, -1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(members.Contains("one"));
            Assert.That(members.Contains("two"));
            Assert.That(members.Contains("three"));
        }

        memberCount = await this._redis.SortedSet.ZUnionStoreAsync(destinationKey, [key1, key2, key3]);
        Assert.AreEqual(memberCount, 6);

        members = this._redis.SortedSet.ZRange(destinationKey, 0, -1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(members.Contains("one"));
            Assert.That(members.Contains("two"));
            Assert.That(members.Contains("three"));
            Assert.That(members.Contains("1"));
            Assert.That(members.Contains("2"));
            Assert.That(members.Contains("3"));
        }

        memberCount = await this._redis.SortedSet.ZUnionStoreAsync(destinationKey, key1, key3);
        Assert.AreEqual(memberCount, 5);

        var memberScore = await this._redis.SortedSet.ZRangeWithScoresAsync(destinationKey, 0, -1);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(1));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(1));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(3));
        }

        memberCount = await this._redis.SortedSet.ZUnionStoreAsync(destinationKey, key1, key3, 2, 3);
        Assert.AreEqual(memberCount, 5);

        memberScore = await this._redis.SortedSet.ZRangeWithScoresAsync(destinationKey, 0, -1);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(4));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(9));
        }

        memberCount = await this._redis.SortedSet.ZUnionStoreAsync(destinationKey, key1, key3, 2, 3, RedisSortedSet.Aggregate.Max);
        Assert.AreEqual(memberCount, 5);

        memberScore = await this._redis.SortedSet.ZRangeWithScoresAsync(destinationKey, 0, -1);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(4));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(9));
        }

        memberCount = await this._redis.SortedSet.ZUnionStoreAsync(destinationKey, key1, key2, 5, 3, RedisSortedSet.Aggregate.Min);
        Assert.AreEqual(memberCount, 3);

        memberScore = await this._redis.SortedSet.ZRangeWithScoresAsync(destinationKey, 0, -1);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "three"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "three").Value, Is.EqualTo(9));
        }

        memberCount = await this._redis.SortedSet.ZUnionStoreAsync(destinationKey, [key1, key3, key2], [1, 3, 2]);
        Assert.AreEqual(memberCount, 6);

        memberScore = await this._redis.SortedSet.ZRangeWithScoresAsync(destinationKey, 0, -1);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(6));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "three"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "three").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(9));
        }

        memberCount = await this._redis.SortedSet.ZUnionStoreAsync(destinationKey, [key1, key3, key2], [1, 3, 2], RedisSortedSet.Aggregate.Max);
        Assert.AreEqual(memberCount, 6);

        memberScore = await this._redis.SortedSet.ZRangeWithScoresAsync(destinationKey, 0, -1);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(6));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "three"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(4));
            Assert.That((double)memberScore.Single(f => f.Key == "three").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(9));
        }

        memberCount = await this._redis.SortedSet.ZUnionStoreAsync(destinationKey, [key1, key3, key2], [1, 3, 2], RedisSortedSet.Aggregate.Min);
        Assert.AreEqual(memberCount, 6);

        memberScore = await this._redis.SortedSet.ZRangeWithScoresAsync(destinationKey, 0, -1);
        Assert.NotNull(memberScore);
        Assert.That(memberScore, Has.Length.EqualTo(6));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScore.Any(f => f.Key == "one"));
            Assert.That(memberScore.Any(f => f.Key == "two"));
            Assert.That(memberScore.Any(f => f.Key == "three"));
            Assert.That(memberScore.Any(f => f.Key == "1"));
            Assert.That(memberScore.Any(f => f.Key == "2"));
            Assert.That(memberScore.Any(f => f.Key == "3"));

            Assert.That((double)memberScore.Single(f => f.Key == "one").Value, Is.EqualTo(1));
            Assert.That((double)memberScore.Single(f => f.Key == "two").Value, Is.EqualTo(2));
            Assert.That((double)memberScore.Single(f => f.Key == "three").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "1").Value, Is.EqualTo(3));
            Assert.That((double)memberScore.Single(f => f.Key == "2").Value, Is.EqualTo(6));
            Assert.That((double)memberScore.Single(f => f.Key == "3").Value, Is.EqualTo(9));
        }
    }

    [Test]
    public async Task BZPopMinAsync()
    {
        const string key1 = "{110}:SortedSet:BZPopMinAsync:Key1";
        const string key2 = "{110}:SortedSet:BZPopMinAsync:Key2";

        var result = await this._redis.SortedSet.BZPopMinAsync(1, key1);
        Assert.IsTrue(result.IsNull);
        Assert.IsFalse(result.HasValue);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        count = await this._redis.SortedSet.ZAddAsync(key2, new Dictionary<SharpValue, double>
        {
            { "one", 0.1 },
            { "two", 0.2 },
            { "three", 0.03 },
        });
        Assert.That(count, Is.EqualTo(3));

        result = await this._redis.SortedSet.BZPopMinAsync(1, key1);
        Assert.IsFalse(result.IsNull);
        Assert.IsTrue(result.HasValue);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key.ToString(), Does.EndWith(key1));
            Assert.That(result.Member.ToString(), Is.EqualTo("one"));
            Assert.That((double)result.Score, Is.EqualTo(1));
        }

        result = await this._redis.SortedSet.BZPopMinAsync(1.1, [key1, key2]);
        Assert.IsFalse(result.IsNull);
        Assert.IsTrue(result.HasValue);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key.ToString(), Does.EndWith(key1));
            Assert.That(result.Member.ToString(), Is.EqualTo("two"));
            Assert.That((double)result.Score, Is.EqualTo(2));
        }

        result = await this._redis.SortedSet.BZPopMinAsync(1.1, [key2, key1]);
        Assert.IsFalse(result.IsNull);
        Assert.IsTrue(result.HasValue);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key.ToString(), Does.EndWith(key2));
            Assert.That(result.Member.ToString(), Is.EqualTo("three"));
            Assert.That((double)result.Score, Is.EqualTo(0.03));
        }
    }

    [Test]
    public async Task BZPopMaxAsync()
    {
        const string key1 = "{111}:SortedSet:BZPopMaxAsync:Key1";
        const string key2 = "{111}:SortedSet:BZPopMaxAsync:Key2";

        var result = await this._redis.SortedSet.BZPopMaxAsync(1, key1);
        Assert.IsTrue(result.IsNull);
        Assert.IsFalse(result.HasValue);

        int count = await this._redis.SortedSet.ZAddAsync(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        count = await this._redis.SortedSet.ZAddAsync(key2, new Dictionary<SharpValue, double>
        {
            { "one", 0.1 },
            { "two", 0.2 },
            { "three", 0.03 },
        });
        Assert.That(count, Is.EqualTo(3));

        result = await this._redis.SortedSet.BZPopMaxAsync(1, key1);
        Assert.IsFalse(result.IsNull);
        Assert.IsTrue(result.HasValue);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key.ToString(), Does.EndWith(key1));
            Assert.That(result.Member.ToString(), Is.EqualTo("three"));
            Assert.That((double)result.Score, Is.EqualTo(3));
        }

        result = await this._redis.SortedSet.BZPopMaxAsync(1.1, [key1, key2]);
        Assert.IsFalse(result.IsNull);
        Assert.IsTrue(result.HasValue);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key.ToString(), Does.EndWith(key1));
            Assert.That(result.Member.ToString(), Is.EqualTo("two"));
            Assert.That((double)result.Score, Is.EqualTo(2));
        }

        result = await this._redis.SortedSet.BZPopMaxAsync(1.1, [key2, key1]);
        Assert.IsFalse(result.IsNull);
        Assert.IsTrue(result.HasValue);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key.ToString(), Does.EndWith(key2));
            Assert.That(result.Member.ToString(), Is.EqualTo("two"));
            Assert.That((double)result.Score, Is.EqualTo(0.2));
        }
    }
}
