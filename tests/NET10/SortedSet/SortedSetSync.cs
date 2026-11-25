using SharpRedis;

namespace NET10.SortedSet;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal sealed class SortedSetSync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public void ZAdd()
    {
        const string key1 = "{51}:SortedSet:ZAdd:Key1";

        var addCount = this._redis.SortedSet.ZAdd(key1, "a", 123);
        Assert.AreEqual(1, (int)addCount);

        double aScore = this._redis.SortedSet.ZScore(key1, "a");
        Assert.AreEqual(123, aScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "a", 122);
        Assert.AreEqual(0, (int)addCount);

        aScore = this._redis.SortedSet.ZScore(key1, "a");
        Assert.AreEqual(122, aScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "a", 456, RedisSortedSet.ZAddWhen.NX);
        Assert.AreEqual(0, (int)addCount);

        aScore = this._redis.SortedSet.ZScore(key1, "a");
        Assert.AreEqual(122, aScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "a", 456, RedisSortedSet.ZAddWhen.XX);
        Assert.AreEqual(0, (int)addCount);

        aScore = this._redis.SortedSet.ZScore(key1, "a");
        Assert.AreEqual(456, aScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "a", 100, scoreWhen: RedisSortedSet.ZAddScoreWhen.LT);
        Assert.AreEqual(0, (int)addCount);

        aScore = this._redis.SortedSet.ZScore(key1, "a");
        Assert.AreEqual(100, aScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "a", 520, scoreWhen: RedisSortedSet.ZAddScoreWhen.LT);
        Assert.AreEqual(0, (int)addCount);

        aScore = this._redis.SortedSet.ZScore(key1, "a");
        Assert.AreEqual(100, aScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "a", 520, scoreWhen: RedisSortedSet.ZAddScoreWhen.GT);
        Assert.AreEqual(0, (int)addCount);

        aScore = this._redis.SortedSet.ZScore(key1, "a");
        Assert.AreEqual(520, aScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "a", 321, scoreWhen: RedisSortedSet.ZAddScoreWhen.GT);
        Assert.AreEqual(0, (int)addCount);

        aScore = this._redis.SortedSet.ZScore(key1, "a");
        Assert.AreEqual(520, aScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "a", 1314, scoreWhen: RedisSortedSet.ZAddScoreWhen.GT, ch: true);
        Assert.AreEqual(1, (int)addCount);

        aScore = this._redis.SortedSet.ZScore(key1, "a");
        Assert.AreEqual(1314, aScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "a", 1, incr: true);
        Assert.AreEqual(1315, (int)addCount);

        aScore = this._redis.SortedSet.ZScore(key1, "a");
        Assert.AreEqual(1315, aScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "a", 1, ch: true, incr: true);
        Assert.AreEqual(1316, (int)addCount);

        aScore = this._redis.SortedSet.ZScore(key1, "a");
        Assert.AreEqual(1316, aScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "b", 1, ch: true);
        Assert.AreEqual(1, (int)addCount);

        var bScore = this._redis.SortedSet.ZScore(key1, "b");
        Assert.AreEqual(1, (double)bScore);

        addCount = this._redis.SortedSet.ZAdd(key1, "b", 1, ch: true);
        Assert.AreEqual(0, (int)addCount);

        bScore = this._redis.SortedSet.ZScore(key1, "b");
        Assert.AreEqual(1, (double)bScore);

        addCount = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "a", -10 },
            { "c", 321 },
            { "d", 3.14 },
            { "e", 9527 },
        });
        Assert.AreEqual(3, (int)addCount);

        addCount = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "a", -13 },
            { "f", 1.11 },
            { "g", 2.22 },
            { "h", 9.99 },
        }, ch: true);
        Assert.AreEqual(4, (int)addCount);

        addCount = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "a", -130 },
            { "f", 10.11 },
            { "g", 22.22 },
            { "h", 99.99 },
        }, when: RedisSortedSet.ZAddWhen.NX);
        Assert.AreEqual(0, (int)addCount);

        var fScore = this._redis.SortedSet.ZScore(key1, "f");
        Assert.AreEqual(1.11, (double)fScore);

        var total = this._redis.SortedSet.ZCard(key1);
        Assert.AreEqual(8, total);

        addCount = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "a", -130 },
            { "f", 10.11 },
            { "g", 22.22 },
            { "h", 99.99 },
            { "i", 99.99 },
        }, when: RedisSortedSet.ZAddWhen.NX);
        Assert.AreEqual(1, (int)addCount);

        total = this._redis.SortedSet.ZCard(key1);
        Assert.AreEqual(9, total);
    }

    [Test]
    public void ZCount()
    {
        const string key1 = "{52}:SortedSet:ZCount:Key1";

        var count = this._redis.SortedSet.ZCount(key1, 0, 100);
        Assert.AreEqual(0, count);

        var addCount = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 11 },
            { "c", 22 },
            { "d", 33.22 },
        }, when: RedisSortedSet.ZAddWhen.NX);
        Assert.AreEqual(4, (int)addCount);

        count = this._redis.SortedSet.ZCount(key1, 0, 100);
        Assert.AreEqual(4, count);

        count = this._redis.SortedSet.ZCount(key1, 0, double.PositiveInfinity);
        Assert.AreEqual(4, count);
    }

    [Test]
    public void ZDiff()
    {
        const string key1 = "{53}:SortedSet:ZDiff:Key1";
        const string key2 = "{53}:SortedSet:ZDiff:Key2";
        const string key3 = "{53}:SortedSet:ZDiff:Key3";

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 11 },
            { "c", 22 },
            { "d", 33.33 },
        });
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.SortedSet.ZAdd(key2, "c", 3.141592653);
        Assert.That(count, Is.EqualTo(1));

        count = this._redis.SortedSet.ZAdd(key3, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "c", 22 },
            { "e", 43.521 },
        });
        Assert.That(count, Is.EqualTo(3));

        var result = this._redis.SortedSet.ZDiff(key1, key2);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("a"));
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("d"));
        }

        var dict = this._redis.SortedSet.ZDiffWithScores(key1, key2);
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

        result = this._redis.SortedSet.ZDiff([key1, key2, key3]);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("d"));
        }

        dict = this._redis.SortedSet.ZDiffWithScores([key1, key2, key3]);
        Assert.That(dict, Is.Not.Null);
        Assert.That(dict, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dict.Any(f => f.Key == "b"), Is.True);
            Assert.That(dict.Any(f => f.Key == "d"), Is.True);
            Assert.That((double)dict.Single(f => f.Key == "b").Value, Is.EqualTo(11));
            Assert.That((double)dict.Single(f => f.Key == "d").Value, Is.EqualTo(33.33));
        }

        result = this._redis.SortedSet.ZDiff("SortedSet:ZDiff:None1", "SortedSet:ZDiff:None2");
        Assert.That(result, Is.Null);

        result = this._redis.SortedSet.ZDiff(key1, "SortedSet:ZDiff:None2");
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
    public void ZDiffStore()
    {
        const string key1 = "{54}:SortedSet:ZDiffStore:Key1";
        const string key2 = "{54}:SortedSet:ZDiffStore:Key2";
        const string key3 = "{54}:SortedSet:ZDiffStore:Key3";
        const string destinationKey = "{54}:SortedSet:ZDiffStore:Destination";

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "b", 11 },
            { "c", 22 },
            { "d", 33.33 },
        });
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.SortedSet.ZAdd(key2, "c", 3.141592653);
        Assert.That(count, Is.EqualTo(1));

        count = this._redis.SortedSet.ZAdd(key3, new Dictionary<SharpValue, double>
        {
            { "a", 1 },
            { "c", 22 },
            { "e", 43.521 },
        });
        Assert.That(count, Is.EqualTo(3));

        var resultCount = this._redis.SortedSet.ZDiffStore(destinationKey, key1, key2);
        Assert.That(resultCount, Is.EqualTo(3));

        var result = this._redis.SortedSet.ZRange(destinationKey, 0, -1);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("a"));
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("d"));
        }

        var dict = this._redis.SortedSet.ZRangeWithScores(destinationKey, 0, -1);
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

        resultCount = this._redis.SortedSet.ZDiffStore(destinationKey, [key1, key2, key3]);
        Assert.That(resultCount, Is.EqualTo(2));

        result = this._redis.SortedSet.ZRange(destinationKey, 0, -1);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("d"));
        }

        dict = this._redis.SortedSet.ZRangeWithScores(destinationKey, 0, -1);
        Assert.That(dict, Is.Not.Null);
        Assert.That(dict, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dict.Any(f => f.Key == "b"), Is.True);
            Assert.That(dict.Any(f => f.Key == "d"), Is.True);
            Assert.That((double)dict.Single(f => f.Key == "b").Value, Is.EqualTo(11));
            Assert.That((double)dict.Single(f => f.Key == "d").Value, Is.EqualTo(33.33));
        }

        resultCount = this._redis.SortedSet.ZDiffStore(destinationKey, key1, "SortedSet:ZDiff:None2");
        Assert.That(resultCount, Is.EqualTo(4));

        result = this._redis.SortedSet.ZRange(destinationKey, 0, -1);
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
    public void ZIncrBy()
    {
        const string key1 = "{55}:SortedSet:ZIncrBy:Key1";

        var score = this._redis.SortedSet.ZScore(key1, "a");
        Assert.That(score.HasValue, Is.False);

        int count = this._redis.SortedSet.ZAdd(key1, "a", 3.141592653);
        Assert.That(count, Is.EqualTo(1));

        score = this._redis.SortedSet.ZScore(key1, "a");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(score.HasValue, Is.True);
            Assert.That((double)score, Is.EqualTo(3.141592653));
        }

        score = this._redis.SortedSet.ZIncrBy(key1, "a", 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(score.HasValue, Is.True);
            Assert.That((double)score, Is.EqualTo(4.141592653));
        }

        score = this._redis.SortedSet.ZIncrBy(key1, "a", -1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(score.HasValue, Is.True);
            Assert.That((double)score, Is.EqualTo(3.141592653));
        }

        score = this._redis.SortedSet.ZIncrBy(key1, "a", double.PositiveInfinity);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(score.HasValue, Is.True);
            Assert.That((double)score, Is.EqualTo(double.PositiveInfinity));
        }

        score = this._redis.SortedSet.ZIncrBy(key1, "b", 1314);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(score.HasValue, Is.True);
            Assert.That((double)score, Is.EqualTo(1314));
        }
    }

    [Test]
    public void ZInter()
    {
        const string key1 = "{56}:SortedSet:ZInter:Key1";
        const string key2 = "{56}:SortedSet:ZInter:Key2";
        const string key3 = "{56}:SortedSet:ZInter:Key3";

        var members = this._redis.SortedSet.ZInter(key1, key2);
        Assert.That(members, Is.Null);

        members = this._redis.SortedSet.ZInter([key1, key2, key3]);
        Assert.That(members, Is.Null);

        var memberScores = this._redis.SortedSet.ZInterWithScores(key1, key2);
        Assert.That(memberScores, Is.Null);

        memberScores = this._redis.SortedSet.ZInterWithScores([key1, key2, key3]);
        Assert.That(memberScores, Is.Null);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        count = this._redis.SortedSet.ZAdd(key3, new Dictionary<SharpValue, double>
        {
            { "one", 111 },
            { "five", 555 },
        });
        Assert.That(count, Is.EqualTo(2));

        members = this._redis.SortedSet.ZInter(key1, key2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(members, Is.Not.Null);
            Assert.That(members, Has.Length.EqualTo(3));
            Assert.That(members.Contains("one"));
            Assert.That(members.Contains("four"));
            Assert.That(members.Contains("five"));
        }

        memberScores = this._redis.SortedSet.ZInterWithScores(key1, key2);
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

        memberScores = this._redis.SortedSet.ZInterWithScores(key1, key2, aggregate: RedisSortedSet.Aggregate.Min);
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

        memberScores = this._redis.SortedSet.ZInterWithScores(key1, key2, key1Weight: 100, aggregate: RedisSortedSet.Aggregate.Max);
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

        memberScores = this._redis.SortedSet.ZInterWithScores([key3, key1, key2]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScores, Is.Not.Null);
            Assert.That(memberScores, Has.Length.EqualTo(2));
            Assert.That(memberScores!.Any(static f => f.Key == "one"));
            Assert.That(memberScores!.Any(static f => f.Key == "five"));
            Assert.That((double)memberScores!.Single(static f => f.Key == "one").Value, Is.EqualTo(123));
            Assert.That((double)memberScores!.Single(static f => f.Key == "five").Value, Is.EqualTo(615));
        }

        memberScores = this._redis.SortedSet.ZInterWithScores([key3, key1, key2], [3.3, 1.1, 2.2]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(memberScores, Is.Not.Null);
            Assert.That(memberScores, Has.Length.EqualTo(2));
            Assert.That(memberScores!.Any(static f => f.Key == "one"));
            Assert.That(memberScores!.Any(static f => f.Key == "five"));
            Assert.That((double)memberScores!.Single(static f => f.Key == "one").Value, Is.EqualTo((1 * 1.1) + (11 * 2.2) + (111 * 3.3)));
            Assert.That((double)memberScores!.Single(static f => f.Key == "five").Value, Is.EqualTo((5 * 1.1) + (55 * 2.2) + (555 * 3.3)));
        }

        memberScores = this._redis.SortedSet.ZInterWithScores([key3, key1, key2], [3.3, 1.1, 2.2], RedisSortedSet.Aggregate.Max);
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
    public void ZInterCard()
    {
        const string key1 = "{57}:SortedSet:ZInterCard:Key1";
        const string key2 = "{57}:SortedSet:ZInterCard:Key2";
        const string key3 = "{57}:SortedSet:ZInterCard:Key3";

        var members = this._redis.SortedSet.ZInterCard(key1, key2);
        Assert.That(members, Is.Zero);

        members = this._redis.SortedSet.ZInterCard([key1, key2, key3]);
        Assert.That(members, Is.Zero);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        count = this._redis.SortedSet.ZAdd(key3, new Dictionary<SharpValue, double>
        {
            { "one", 111 },
            { "five", 555 },
        });
        Assert.That(count, Is.EqualTo(2));

        members = this._redis.SortedSet.ZInterCard(key1, key2);
        Assert.That(members, Is.EqualTo(3));

        members = this._redis.SortedSet.ZInterCard(key1, key2, 2);
        Assert.That(members, Is.EqualTo(2));

        members = this._redis.SortedSet.ZInterCard([key1, key2, key3], 2);
        Assert.That(members, Is.EqualTo(2));

        members = this._redis.SortedSet.ZInterCard([key1, key2, key3], 1);
        Assert.That(members, Is.EqualTo(1));
    }

    [Test]
    public void ZInterStore()
    {
        const string key1 = "{58}:SortedSet:ZInterStore:Key1";
        const string key2 = "{58}:SortedSet:ZInterStore:Key2";
        const string key3 = "{58}:SortedSet:ZInterStore:Key3";
        const string destinationKey = "{58}:SortedSet:ZInterStore:Destination";

        var members = this._redis.SortedSet.ZInterStore(destinationKey, key1, key2);
        Assert.That(members, Is.Zero);

        members = this._redis.SortedSet.ZInterStore(destinationKey, [key1, key2, key3]);
        Assert.That(members, Is.Zero);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        count = this._redis.SortedSet.ZAdd(key3, new Dictionary<SharpValue, double>
        {
            { "one", 111 },
            { "five", 555 },
        });
        Assert.That(count, Is.EqualTo(2));

        members = this._redis.SortedSet.ZInterStore(destinationKey, key1, key2);
        Assert.That(members, Is.EqualTo(3));

        double oneScore = this._redis.SortedSet.ZScore(destinationKey, "one");
        Assert.That(oneScore, Is.EqualTo(12));

        double fourScore = this._redis.SortedSet.ZScore(destinationKey, "four");
        Assert.That(fourScore, Is.EqualTo(48));

        double fiveScore = this._redis.SortedSet.ZScore(destinationKey, "five");
        Assert.That(fiveScore, Is.EqualTo(60));

        members = this._redis.SortedSet.ZInterStore(destinationKey, key1, key2, 10, 10, aggregate: RedisSortedSet.Aggregate.Max);
        Assert.That(members, Is.EqualTo(3));

        oneScore = this._redis.SortedSet.ZScore(destinationKey, "one");
        Assert.That(oneScore, Is.EqualTo(110));

        fourScore = this._redis.SortedSet.ZScore(destinationKey, "four");
        Assert.That(fourScore, Is.EqualTo(440));

        fiveScore = this._redis.SortedSet.ZScore(destinationKey, "five");
        Assert.That(fiveScore, Is.EqualTo(550));

        members = this._redis.SortedSet.ZInterStore(destinationKey, [key1, key2, key3]);
        Assert.That(members, Is.EqualTo(2));

        oneScore = this._redis.SortedSet.ZScore(destinationKey, "one");
        Assert.That(oneScore, Is.EqualTo(123));

        fiveScore = this._redis.SortedSet.ZScore(destinationKey, "five");
        Assert.That(fiveScore, Is.EqualTo(615));
    }

    [Test]
    public void ZLexCount()
    {
        const string key1 = "{59}:SortedSet:ZLexCount:Key1";

        var count = this._redis.SortedSet.ZLexCount(key1, "a", "z");
        Assert.That(count, Is.Zero);

        int result = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        count = this._redis.SortedSet.ZLexCount(key1, "a", "z");
        Assert.That(count, Is.EqualTo(12));

        count = this._redis.SortedSet.ZLexCount(key1, "a", "d");
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.SortedSet.ZLexCount(key1, "a", ("f", false));
        Assert.That(count, Is.EqualTo(5));

        count = this._redis.SortedSet.ZLexCount(key1, "-", "+");
        Assert.That(count, Is.EqualTo(12));
    }

    [Test]
    public void ZMPop()
    {
        const string key1 = "{60}:SortedSet:ZMPop:Key1";
        const string key2 = "{60}:SortedSet:ZMPop:Key2";

        var result = this._redis.SortedSet.ZMPop(key1, key2, RedisSortedSet.PopHow.Min);
        Assert.That(result.IsNull, Is.True);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        result = this._redis.SortedSet.ZMPop(key1, key2, RedisSortedSet.PopHow.Min);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsNull, Is.False);
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result.Key.ToString(), Does.EndWith(key1));
            var member = result[0];
            Assert.That(member.Key.ToString(), Is.EqualTo("one"));
            Assert.That((double)member.Value, Is.EqualTo(1));
        }

        result = this._redis.SortedSet.ZMPop(key1, key2, RedisSortedSet.PopHow.Max, 10);
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

        count = this._redis.SortedSet.ZAdd(key2, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        result = this._redis.SortedSet.ZMPop([key1, key2], RedisSortedSet.PopHow.Min);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsNull, Is.False);
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result.Key.ToString(), Does.EndWith(key2));
            var member = result[0];
            Assert.That(member.Key.ToString(), Is.EqualTo("one"));
            Assert.That((double)member.Value, Is.EqualTo(1));
        }

        result = this._redis.SortedSet.ZMPop([key1, key2], RedisSortedSet.PopHow.Max, 10);
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
    public void ZMScore()
    {
        const string key1 = "{61}:SortedSet:ZMScore:Key1";

        var scores = this._redis.SortedSet.ZMScore(key1, ["one", "two"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(scores, Is.Not.Null);
            Assert.That(scores, Has.Length.EqualTo(2));
            Assert.That(scores![0].HasValue, Is.False);
            Assert.That(scores![1].HasValue, Is.False);
        }

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        scores = this._redis.SortedSet.ZMScore(key1, ["one", "two", "none"]);
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
    public void ZPopMax()
    {
        const string key1 = "{62}:SortedSet:ZPopMax:Key1";

        var single = this._redis.SortedSet.ZPopMax(key1);
        Assert.That(single.HasValue, Is.False);

        var array = this._redis.SortedSet.ZPopMax(key1, 3);
        Assert.That(array, Is.Null);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        single = this._redis.SortedSet.ZPopMax(key1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(single.HasValue, Is.True);
            Assert.That(single!.Value.Key.ToString(), Is.EqualTo("five"));
            Assert.That((int)single.Value.Value, Is.EqualTo(5));
        }

        array = this._redis.SortedSet.ZPopMax(key1, 30);
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
    public void ZPopMin()
    {
        const string key1 = "{63}:SortedSet:ZPopMin:Key1";

        var single = this._redis.SortedSet.ZPopMin(key1);
        Assert.That(single.HasValue, Is.False);

        var array = this._redis.SortedSet.ZPopMin(key1, 3);
        Assert.That(array, Is.Null);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        single = this._redis.SortedSet.ZPopMin(key1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(single.HasValue, Is.True);
            Assert.That(single!.Value.Key.ToString(), Is.EqualTo("one"));
            Assert.That((int)single.Value.Value, Is.EqualTo(1));
        }

        array = this._redis.SortedSet.ZPopMin(key1, 30);
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
    public void ZRandMember()
    {
        const string key1 = "{64}:SortedSet:ZRandMember:Key1";

        var single = this._redis.SortedSet.ZRandMember(key1);
        Assert.That(single.HasValue, Is.False);

        var singleWithScore = this._redis.SortedSet.ZRandMemberWithScores(key1);
        Assert.That(singleWithScore.HasValue, Is.False);

        var array = this._redis.SortedSet.ZRandMember(key1, 3);
        Assert.That(array, Is.Null);

        var arrayWithScore = this._redis.SortedSet.ZRandMemberWithScores(key1, 3);
        Assert.That(arrayWithScore, Is.Null);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2  },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
        });
        Assert.That(count, Is.EqualTo(5));

        single = this._redis.SortedSet.ZRandMember(key1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(single.HasValue, Is.True);
            Assert.That(single.IsNull, Is.False);
        }

        singleWithScore = this._redis.SortedSet.ZRandMemberWithScores(key1);
        Assert.That(singleWithScore.HasValue, Is.True);

        array = this._redis.SortedSet.ZRandMember(key1, 3);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array, Is.Not.Null);
            Assert.That(array, Has.Length.EqualTo(3));
        }

        array = this._redis.SortedSet.ZRandMember(key1, 0);
        Assert.That(array, Is.Null);

        arrayWithScore = this._redis.SortedSet.ZRandMemberWithScores(key1, 3);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(arrayWithScore, Is.Not.Null);
            Assert.That(arrayWithScore, Has.Length.EqualTo(3));
        }

        arrayWithScore = this._redis.SortedSet.ZRandMemberWithScores(key1, 0);
        Assert.That(arrayWithScore, Is.Null);

        array = this._redis.SortedSet.ZRandMember(key1, 30);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array, Is.Not.Null);
            Assert.That(array, Has.Length.EqualTo(5));
        }

        arrayWithScore = this._redis.SortedSet.ZRandMemberWithScores(key1, 30);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(arrayWithScore, Is.Not.Null);
            Assert.That(arrayWithScore, Has.Length.EqualTo(5));
        }

        array = this._redis.SortedSet.ZRandMember(key1, -30);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array, Is.Not.Null);
            Assert.That(array, Has.Length.EqualTo(30));
        }

        arrayWithScore = this._redis.SortedSet.ZRandMemberWithScores(key1, -30);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(arrayWithScore, Is.Not.Null);
            Assert.That(arrayWithScore, Has.Length.EqualTo(30));
        }
    }

    [Test]
    public void ZRange()
    {
        const string key1 = "{65}:SortedSet:ZRange:Key1";
        const string key2 = "{65}:SortedSet:ZRange:Key2";

        var range = this._redis.SortedSet.ZRange(key1, 0, -1);
        Assert.That(range, Is.Null);

        var rangeWithScore = this._redis.SortedSet.ZRangeWithScores(key1, 0, -1);
        Assert.That(rangeWithScore, Is.Null);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        count = this._redis.SortedSet.ZAdd(key2, new Dictionary<SharpValue, double>
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

        range = this._redis.SortedSet.ZRange(key1, 0, -1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(26));
            Assert.That(range![0].ToString(), Is.EqualTo("a"));
            Assert.That(range![25].ToString(), Is.EqualTo("z"));
        }

        rangeWithScore = this._redis.SortedSet.ZRangeWithScores(key1, 0, -1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(26));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("a"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(1));
            Assert.That(rangeWithScore![25].Key.ToString(), Is.EqualTo("z"));
            Assert.That((double)rangeWithScore![25].Value, Is.EqualTo(26));
        }

        range = this._redis.SortedSet.ZRange(key1, 100, 0, by: RedisSortedSet.RangeBy.ByScore, rev: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(26));
            Assert.That(range![0].ToString(), Is.EqualTo("z"));
            Assert.That(range![25].ToString(), Is.EqualTo("a"));
        }

        rangeWithScore = this._redis.SortedSet.ZRangeWithScores(key1, 100, 0, by: RedisSortedSet.RangeBy.ByScore, rev: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(26));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("z"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(26));
            Assert.That(rangeWithScore![25].Key.ToString(), Is.EqualTo("a"));
            Assert.That((double)rangeWithScore![25].Value, Is.EqualTo(1));
        }

        range = this._redis.SortedSet.ZRange(key1, 0, 200, by: RedisSortedSet.RangeBy.ByScore, rev: false, offset: 1, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(5));
            Assert.That(range![0].ToString(), Is.EqualTo("b"));
            Assert.That(range![4].ToString(), Is.EqualTo("f"));
        }

        rangeWithScore = this._redis.SortedSet.ZRangeWithScores(key1, 0, 100, by: RedisSortedSet.RangeBy.ByScore, rev: false, offset: 1, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(5));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("b"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(2));
            Assert.That(rangeWithScore![4].Key.ToString(), Is.EqualTo("f"));
            Assert.That((double)rangeWithScore![4].Value, Is.EqualTo(6));
        }

        range = this._redis.SortedSet.ZRange(key2, "a", "h", by: RedisSortedSet.RangeBy.ByLex, rev: false, offset: 1, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(5));
            Assert.That(range![0].ToString(), Is.EqualTo("b"));
            Assert.That(range![4].ToString(), Is.EqualTo("f"));
        }

        range = this._redis.SortedSet.ZRange(key2, ("a", false), "h", by: RedisSortedSet.RangeBy.ByLex, rev: false, offset: 1, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(5));
            Assert.That(range![0].ToString(), Is.EqualTo("c"));
            Assert.That(range![4].ToString(), Is.EqualTo("g"));
        }

        rangeWithScore = this._redis.SortedSet.ZRangeWithScores(key1, (1, false), (5, false), by: RedisSortedSet.RangeBy.ByScore, rev: false, offset: 1, count: 5);
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
    public void ZRangeByLex()
    {
        const string key1 = "{66}:SortedSet:ZRangeByLex:Key1";

        var range = this._redis.SortedSet.ZRangeByLex(key1, "a", "z");
        Assert.That(range, Is.Null);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        range = this._redis.SortedSet.ZRangeByLex(key1, "a", "z");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(26));
            Assert.That(range![0].ToString(), Is.EqualTo("a"));
            Assert.That(range![25].ToString(), Is.EqualTo("z"));
        }

        range = this._redis.SortedSet.ZRangeByLex(key1, ("a", false), "z");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(25));
            Assert.That(range![0].ToString(), Is.EqualTo("b"));
            Assert.That(range![24].ToString(), Is.EqualTo("z"));
        }

        range = this._redis.SortedSet.ZRangeByLex(key1, ("a", false), ("z", false), 2, 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(5));
            Assert.That(range![0].ToString(), Is.EqualTo("d"));
            Assert.That(range![4].ToString(), Is.EqualTo("h"));
        }
    }

    [Test]
    public void ZRangeByScore()
    {
        const string key1 = "{67}:SortedSet:ZRangeByScore:Key1";

        var range = this._redis.SortedSet.ZRangeByScore(key1, double.NegativeInfinity, double.PositiveInfinity);
        Assert.That(range, Is.Null);

        var rangeWithScore = this._redis.SortedSet.ZRangeByScoreWithScores(key1, double.NegativeInfinity, double.PositiveInfinity);
        Assert.That(rangeWithScore, Is.Null);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        range = this._redis.SortedSet.ZRangeByScore(key1, double.NegativeInfinity, double.PositiveInfinity);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(26));
            Assert.That(range![0].ToString(), Is.EqualTo("a"));
            Assert.That(range![25].ToString(), Is.EqualTo("z"));
        }

        range = this._redis.SortedSet.ZRangeByScore(key1, (5, false), double.PositiveInfinity);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(21));
            Assert.That(range![0].ToString(), Is.EqualTo("f"));
            Assert.That(range![20].ToString(), Is.EqualTo("z"));
        }

        range = this._redis.SortedSet.ZRangeByScore(key1, (5, false), double.PositiveInfinity, 3, 3);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range, Is.Not.Null);
            Assert.That(range, Has.Length.EqualTo(3));
            Assert.That(range![0].ToString(), Is.EqualTo("i"));
            Assert.That(range![2].ToString(), Is.EqualTo("k"));
        }

        rangeWithScore = this._redis.SortedSet.ZRangeByScoreWithScores(key1, double.NegativeInfinity, double.PositiveInfinity);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(26));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("a"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(1));
            Assert.That(rangeWithScore![25].Key.ToString(), Is.EqualTo("z"));
            Assert.That((double)rangeWithScore![25].Value, Is.EqualTo(26));
        }

        rangeWithScore = this._redis.SortedSet.ZRangeByScoreWithScores(key1, double.NegativeInfinity, double.PositiveInfinity, 1, 100);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeWithScore, Is.Not.Null);
            Assert.That(rangeWithScore, Has.Length.EqualTo(25));
            Assert.That(rangeWithScore![0].Key.ToString(), Is.EqualTo("b"));
            Assert.That((double)rangeWithScore![0].Value, Is.EqualTo(2));
            Assert.That(rangeWithScore![24].Key.ToString(), Is.EqualTo("z"));
            Assert.That((double)rangeWithScore![24].Value, Is.EqualTo(26));
        }

        rangeWithScore = this._redis.SortedSet.ZRangeByScoreWithScores(key1, (2, false), (10, false), 1, 100);
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
    public void ZRangeStore()
    {
        const string key1 = "{68}:SortedSet:ZRangeStore:Key1";
        const string destinationKey = "{68}:SortedSet:ZRangeStore:DestinationKey";

        var result = this._redis.SortedSet.ZRangeStore(destinationKey, key1, "-", "+", RedisSortedSet.RangeBy.ByLex);
        Assert.That(result, Is.Zero);

        var length = this._redis.SortedSet.ZCard(destinationKey);
        Assert.That(length, Is.Zero);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        result = this._redis.SortedSet.ZRangeStore(destinationKey, key1, "-", "+", RedisSortedSet.RangeBy.ByLex);
        Assert.That(result, Is.EqualTo(26));

        length = this._redis.SortedSet.ZCard(destinationKey);
        Assert.That(length, Is.EqualTo(26));

        result = this._redis.SortedSet.ZRangeStore(destinationKey, key1, 100, 0, RedisSortedSet.RangeBy.ByScore, true);
        Assert.That(result, Is.EqualTo(26));

        length = this._redis.SortedSet.ZCard(destinationKey);
        Assert.That(length, Is.EqualTo(26));

        result = this._redis.SortedSet.ZRangeStore(destinationKey, key1, 0, 100, RedisSortedSet.RangeBy.ByScore, offset: 1, count: 5);
        Assert.That(result, Is.EqualTo(5));

        length = this._redis.SortedSet.ZCard(destinationKey);
        Assert.That(length, Is.EqualTo(5));
    }

    [Test]
    public void ZRank()
    {
        const string key1 = "{69}:SortedSet:ZRank:Key1";

        var rank = this._redis.SortedSet.ZRank(key1, "c");
        Assert.Null(rank);

        var rankScore = this._redis.SortedSet.ZRankWithScore(key1, "g");
        Assert.True(rankScore.IsNull);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        rank = this._redis.SortedSet.ZRank(key1, "cc");
        Assert.Null(rank);

        rankScore = this._redis.SortedSet.ZRankWithScore(key1, "gg");
        Assert.True(rankScore.IsNull);

        rank = this._redis.SortedSet.ZRank(key1, "c");
        Assert.AreEqual(rank, 2);

        rankScore = this._redis.SortedSet.ZRankWithScore(key1, "g");
        Assert.False(rankScore.IsNull);
        (var r, var score) = rankScore;
        Assert.AreEqual(r, 6);
        Assert.AreEqual((int)score, 7);
    }

    [Test]
    public void ZRem()
    {
        const string key1 = "{70}:SortedSet:ZRem:Key1";

        var result = this._redis.SortedSet.ZRem(key1, "a");
        Assert.Zero(result);

        result = this._redis.SortedSet.ZRem(key1, ["a", "b"]);
        Assert.Zero(result);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        result = this._redis.SortedSet.ZRem(key1, "a");
        Assert.AreEqual(1, result);

        result = this._redis.SortedSet.ZRem(key1, ["a", "b", "c"]);
        Assert.AreEqual(2, result);

        var length = this._redis.SortedSet.ZCard(key1);
        Assert.AreEqual(7, length);
    }

    [Test]
    public void ZRemRangeByLex()
    {
        const string key1 = "{71}:SortedSet:ZRemRangeByLex:Key1";

        var result = this._redis.SortedSet.ZRemRangeByLex(key1, "a", "z");
        Assert.Zero(result);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        result = this._redis.SortedSet.ZRemRangeByLex(key1, "a", "d");
        Assert.AreEqual(4, result);

        result = this._redis.SortedSet.ZRemRangeByLex(key1, "a", ("j", false));
        Assert.AreEqual(5, result);

        var length = this._redis.SortedSet.ZCard(key1);
        Assert.AreEqual(1, length);
    }

    [Test]
    public void ZRemRangeByRank()
    {
        const string key1 = "{72}:SortedSet:ZRemRangeByRank:Key1";

        var result = this._redis.SortedSet.ZRemRangeByRank(key1, 0, -1);
        Assert.Zero(result);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        result = this._redis.SortedSet.ZRemRangeByRank(key1, 0, 3);
        Assert.AreEqual(4, result);

        result = this._redis.SortedSet.ZRemRangeByRank(key1, 0, 4);
        Assert.AreEqual(5, result);

        var length = this._redis.SortedSet.ZCard(key1);
        Assert.AreEqual(1, length);
    }

    [Test]
    public void ZRemRangeByScore()
    {
        const string key1 = "{73}:SortedSet:ZRemRangeByScore:Key1";

        var result = this._redis.SortedSet.ZRemRangeByScore(key1, 0, 100);
        Assert.Zero(result);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        result = this._redis.SortedSet.ZRemRangeByScore(key1, 0, 3);
        Assert.AreEqual(3, result);

        result = this._redis.SortedSet.ZRemRangeByScore(key1, 0, (10, false));
        Assert.AreEqual(6, result);

        var length = this._redis.SortedSet.ZCard(key1);
        Assert.AreEqual(1, length);
    }

    [Test]
    public void ZRevRange()
    {
        const string key1 = "{74}:SortedSet:ZRevRange:Key1";

        var range = this._redis.SortedSet.ZRevRange(key1, 0, 100);
        Assert.Null(range);

        var rangeScore = this._redis.SortedSet.ZRevRangeWithScores(key1, 0, 100);
        Assert.Null(rangeScore);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        range = this._redis.SortedSet.ZRevRange(key1, 1, 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.NotNull(range);
            Assert.That(range, Has.Length.EqualTo(5));
            Assert.That(range![0].ToString(), Is.EqualTo("i"));
            Assert.That(range![4].ToString(), Is.EqualTo("e"));
        }

        rangeScore = this._redis.SortedSet.ZRevRangeWithScores(key1, 0, -2);
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
    public void ZRevRangeByLex()
    {
        const string key1 = "{75}:SortedSet:ZRevRangeByLex:Key1";

        var range = this._redis.SortedSet.ZRevRangeByLex(key1, "c", "-");
        Assert.Null(range);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        range = this._redis.SortedSet.ZRevRangeByLex(key1, "c", "-");
        Assert.That(range, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("c"));
            Assert.That(range[2].ToString(), Is.EqualTo("a"));
        }

        range = this._redis.SortedSet.ZRevRangeByLex(key1, "+", "-");
        Assert.That(range, Has.Length.EqualTo(10));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("j"));
            Assert.That(range[9].ToString(), Is.EqualTo("a"));
        }

        range = this._redis.SortedSet.ZRevRangeByLex(key1, ("j", false), "d");
        Assert.That(range, Has.Length.EqualTo(6));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("i"));
            Assert.That(range[5].ToString(), Is.EqualTo("d"));
        }

        range = this._redis.SortedSet.ZRevRangeByLex(key1, ("j", false), ("d", false));
        Assert.That(range, Has.Length.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("i"));
            Assert.That(range[4].ToString(), Is.EqualTo("e"));
        }
    }

    [Test]
    public void ZRevRangeByScore()
    {
        const string key1 = "{76}:SortedSet:ZRevRangeByScore:Key1";

        var range = this._redis.SortedSet.ZRevRangeByScore(key1, double.PositiveInfinity, double.NegativeInfinity);
        Assert.Null(range);

        var rangeScore = this._redis.SortedSet.ZRevRangeByScoreWithScores(key1, double.PositiveInfinity, double.NegativeInfinity);
        Assert.Null(rangeScore);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        range = this._redis.SortedSet.ZRevRangeByScore(key1, double.PositiveInfinity, double.NegativeInfinity);
        Assert.NotNull(range);
        Assert.That(range, Has.Length.EqualTo(10));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("j"));
            Assert.That(range[9].ToString(), Is.EqualTo("a"));
        }

        range = this._redis.SortedSet.ZRevRangeByScore(key1, double.PositiveInfinity, double.NegativeInfinity, 1, 3);
        Assert.NotNull(range);
        Assert.That(range, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("i"));
            Assert.That(range[2].ToString(), Is.EqualTo("g"));
        }

        range = this._redis.SortedSet.ZRevRangeByScore(key1, (8, false), double.NegativeInfinity, 0, 100);
        Assert.NotNull(range);
        Assert.That(range, Has.Length.EqualTo(7));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("g"));
            Assert.That(range[6].ToString(), Is.EqualTo("a"));
        }

        rangeScore = this._redis.SortedSet.ZRevRangeByScoreWithScores(key1, double.PositiveInfinity, double.NegativeInfinity);
        Assert.NotNull(rangeScore);
        Assert.That(rangeScore, Has.Length.EqualTo(10));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeScore[0].Key.ToString(), Is.EqualTo("j"));
            Assert.That((int)rangeScore[0].Value, Is.EqualTo(10));
            Assert.That(rangeScore[9].Key.ToString(), Is.EqualTo("a"));
            Assert.That((int)rangeScore[9].Value, Is.EqualTo(1));
        }

        rangeScore = this._redis.SortedSet.ZRevRangeByScoreWithScores(key1, double.PositiveInfinity, double.NegativeInfinity, 1, 3);
        Assert.NotNull(rangeScore);
        Assert.That(rangeScore, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rangeScore[0].Key.ToString(), Is.EqualTo("i"));
            Assert.That((int)rangeScore[0].Value, Is.EqualTo(9));
            Assert.That(rangeScore[2].Key.ToString(), Is.EqualTo("g"));
            Assert.That((int)rangeScore[2].Value, Is.EqualTo(7));
        }

        rangeScore = this._redis.SortedSet.ZRevRangeByScoreWithScores(key1, (8, false), double.NegativeInfinity, 0, 100);
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
    public void ZRevRank()
    {
        const string key1 = "{77}:SortedSet:ZRevRank:Key1";

        var rank = this._redis.SortedSet.ZRevRank(key1, "c");
        Assert.Null(rank);

        var rankScore = this._redis.SortedSet.ZRevRankWithScore(key1, "g");
        Assert.True(rankScore.IsNull);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
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

        rank = this._redis.SortedSet.ZRevRank(key1, "cc");
        Assert.Null(rank);

        rankScore = this._redis.SortedSet.ZRevRankWithScore(key1, "gg");
        Assert.True(rankScore.IsNull);

        rank = this._redis.SortedSet.ZRevRank(key1, "c");
        Assert.AreEqual(rank, 23);

        rankScore = this._redis.SortedSet.ZRevRankWithScore(key1, "g");
        Assert.False(rankScore.IsNull);
        (var r, var score) = rankScore;
        Assert.AreEqual(r, 19);
        Assert.AreEqual((int)score, 7);
    }

    [Test]
    public void ZUnion()
    {
        const string key1 = "{78}:SortedSet:ZUnion:Key1";
        const string key2 = "{78}:SortedSet:ZUnion:Key2";
        const string key3 = "{78}:SortedSet:ZUnion:Key3";

        var members = this._redis.SortedSet.ZUnion(key1, key2);
        Assert.Null(members);

        members = this._redis.SortedSet.ZUnion([key1, key2, key3]);
        Assert.Null(members);

        var memberScore = this._redis.SortedSet.ZUnionWithScores(key1, key2);
        Assert.Null(memberScore);

        memberScore = this._redis.SortedSet.ZUnionWithScores([key1, key2, key3]);
        Assert.Null(memberScore);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
        });
        Assert.That(count, Is.EqualTo(2));

        count = this._redis.SortedSet.ZAdd(key2, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        count = this._redis.SortedSet.ZAdd(key3, new Dictionary<SharpValue, double>
        {
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        members = this._redis.SortedSet.ZUnion(key1, key2);
        Assert.NotNull(members);
        Assert.That(members, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(members.Contains("one"));
            Assert.That(members.Contains("two"));
            Assert.That(members.Contains("three"));
        }

        members = this._redis.SortedSet.ZUnion([key1, key2, key3]);
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

        memberScore = this._redis.SortedSet.ZUnionWithScores(key1, key3);
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

        memberScore = this._redis.SortedSet.ZUnionWithScores(key1, key3, 2, 3);
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

        memberScore = this._redis.SortedSet.ZUnionWithScores(key1, key3, 2, 3, RedisSortedSet.Aggregate.Max);
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

        memberScore = this._redis.SortedSet.ZUnionWithScores(key1, key2, 5, 3, RedisSortedSet.Aggregate.Min);
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

        memberScore = this._redis.SortedSet.ZUnionWithScores([key1, key3, key2], [1, 3, 2]);
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

        memberScore = this._redis.SortedSet.ZUnionWithScores([key1, key3, key2], [1, 3, 2], RedisSortedSet.Aggregate.Max);
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

        memberScore = this._redis.SortedSet.ZUnionWithScores([key1, key3, key2], [1, 3, 2], RedisSortedSet.Aggregate.Min);
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
    public void ZUnionStore()
    {
        const string key1 = "{79}:SortedSet:ZUnionStore:Key1";
        const string key2 = "{79}:SortedSet:ZUnionStore:Key2";
        const string key3 = "{79}:SortedSet:ZUnionStore:Key3";
        const string destinationKey = "{79}:SortedSet:ZUnionStore:DestinationKey";

        var memberCount = this._redis.SortedSet.ZUnionStore(destinationKey, key1, key2);
        Assert.Zero(memberCount);

        memberCount = this._redis.SortedSet.ZUnionStore(destinationKey, [key1, key2, key3]);
        Assert.Zero(memberCount);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
        });
        Assert.That(count, Is.EqualTo(2));

        count = this._redis.SortedSet.ZAdd(key2, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        count = this._redis.SortedSet.ZAdd(key3, new Dictionary<SharpValue, double>
        {
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        memberCount = this._redis.SortedSet.ZUnionStore(destinationKey, key1, key2);
        Assert.AreEqual(memberCount, 3);

        var members = this._redis.SortedSet.ZRange(destinationKey, 0, -1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(members.Contains("one"));
            Assert.That(members.Contains("two"));
            Assert.That(members.Contains("three"));
        }

        memberCount = this._redis.SortedSet.ZUnionStore(destinationKey, [key1, key2, key3]);
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

        memberCount = this._redis.SortedSet.ZUnionStore(destinationKey, key1, key3);
        Assert.AreEqual(memberCount, 5);

        var memberScore = this._redis.SortedSet.ZRangeWithScores(destinationKey, 0, -1);
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

        memberCount = this._redis.SortedSet.ZUnionStore(destinationKey, key1, key3, 2, 3);
        Assert.AreEqual(memberCount, 5);

        memberScore = this._redis.SortedSet.ZRangeWithScores(destinationKey, 0, -1);
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

        memberCount = this._redis.SortedSet.ZUnionStore(destinationKey, key1, key3, 2, 3, RedisSortedSet.Aggregate.Max);
        Assert.AreEqual(memberCount, 5);

        memberScore = this._redis.SortedSet.ZRangeWithScores(destinationKey, 0, -1);
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

        memberCount = this._redis.SortedSet.ZUnionStore(destinationKey, key1, key2, 5, 3, RedisSortedSet.Aggregate.Min);
        Assert.AreEqual(memberCount, 3);

        memberScore = this._redis.SortedSet.ZRangeWithScores(destinationKey, 0, -1);
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

        memberCount = this._redis.SortedSet.ZUnionStore(destinationKey, [key1, key3, key2], [1, 3, 2]);
        Assert.AreEqual(memberCount, 6);

        memberScore = this._redis.SortedSet.ZRangeWithScores(destinationKey, 0, -1);
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

        memberCount = this._redis.SortedSet.ZUnionStore(destinationKey, [key1, key3, key2], [1, 3, 2], RedisSortedSet.Aggregate.Max);
        Assert.AreEqual(memberCount, 6);

        memberScore = this._redis.SortedSet.ZRangeWithScores(destinationKey, 0, -1);
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

        memberCount = this._redis.SortedSet.ZUnionStore(destinationKey, [key1, key3, key2], [1, 3, 2], RedisSortedSet.Aggregate.Min);
        Assert.AreEqual(memberCount, 6);

        memberScore = this._redis.SortedSet.ZRangeWithScores(destinationKey, 0, -1);
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
    public void BZPopMin()
    {
        const string key1 = "{80}:SortedSet:BZPopMin:Key1";
        const string key2 = "{80}:SortedSet:BZPopMin:Key2";

        var result = this._redis.SortedSet.BZPopMin(1, key1);
        Assert.IsTrue(result.IsNull);
        Assert.IsFalse(result.HasValue);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        count = this._redis.SortedSet.ZAdd(key2, new Dictionary<SharpValue, double>
        {
            { "one", 0.1 },
            { "two", 0.2 },
            { "three", 0.03 },
        });
        Assert.That(count, Is.EqualTo(3));

        result = this._redis.SortedSet.BZPopMin(1, key1);
        Assert.IsFalse(result.IsNull);
        Assert.IsTrue(result.HasValue);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key.ToString(), Does.EndWith(key1));
            Assert.That(result.Member.ToString(), Is.EqualTo("one"));
            Assert.That((double)result.Score, Is.EqualTo(1));
        }

        result = this._redis.SortedSet.BZPopMin(1.1, [key1, key2]);
        Assert.IsFalse(result.IsNull);
        Assert.IsTrue(result.HasValue);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key.ToString(), Does.EndWith(key1));
            Assert.That(result.Member.ToString(), Is.EqualTo("two"));
            Assert.That((double)result.Score, Is.EqualTo(2));
        }

        result = this._redis.SortedSet.BZPopMin(1.1, [key2, key1]);
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
    public void BZPopMax()
    {
        const string key1 = "{81}:SortedSet:BZPopMax:Key1";
        const string key2 = "{81}:SortedSet:BZPopMax:Key2";

        var result = this._redis.SortedSet.BZPopMax(1, key1);
        Assert.IsTrue(result.IsNull);
        Assert.IsFalse(result.HasValue);

        int count = this._redis.SortedSet.ZAdd(key1, new Dictionary<SharpValue, double>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
        });
        Assert.That(count, Is.EqualTo(3));

        count = this._redis.SortedSet.ZAdd(key2, new Dictionary<SharpValue, double>
        {
            { "one", 0.1 },
            { "two", 0.2 },
            { "three", 0.03 },
        });
        Assert.That(count, Is.EqualTo(3));

        result = this._redis.SortedSet.BZPopMax(1, key1);
        Assert.IsFalse(result.IsNull);
        Assert.IsTrue(result.HasValue);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key.ToString(), Does.EndWith(key1));
            Assert.That(result.Member.ToString(), Is.EqualTo("three"));
            Assert.That((double)result.Score, Is.EqualTo(3));
        }

        result = this._redis.SortedSet.BZPopMax(1.1, [key1, key2]);
        Assert.IsFalse(result.IsNull);
        Assert.IsTrue(result.HasValue);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key.ToString(), Does.EndWith(key1));
            Assert.That(result.Member.ToString(), Is.EqualTo("two"));
            Assert.That((double)result.Score, Is.EqualTo(2));
        }

        result = this._redis.SortedSet.BZPopMax(1.1, [key2, key1]);
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
