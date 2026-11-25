using SharpRedis;
using SharpRedis.Exceptions;
using System.Diagnostics;

namespace NET10.List;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class ListSync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public void LPush()
    {
        const string key = "{144}:List:LPush:Key";
        const string value1 = "value1";
        const string value2 = "value2";
        const string value3 = "value3";
        const string value4 = "value4";
        const string value5 = "value5";

        int len = this._redis.List.LLen(key);
        Assert.That(len, Is.Zero);

        string? first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.Null);

        int lpush = this._redis.List.LPush(key, value1);
        Assert.That(lpush, Is.EqualTo(1));

        first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.EqualTo(value1));

        lpush = this._redis.List.LPush(key, [value2, value3]);
        Assert.That(lpush, Is.EqualTo(3));

        first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.EqualTo(value3));

        string? last = this._redis.List.LIndex(key, -1);
        Assert.That(last, Is.EqualTo(value1));

        lpush = this._redis.List.LPush(key, [value4, value5], true);
        Assert.That(lpush, Is.EqualTo(5));

        first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.EqualTo(value4));

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(5));
    }

    [Test]
    public void LInsert()
    {
        const string key = "{145}:List:LInsert:Key";
        const string value1 = "一起长大的约定";
        const string value2 = "那样清晰";
        const string value3 = "打过勾的我相信";

        int insertResult = this._redis.List.LInsert(key, value1, value2, RedisList.InsertPosition.Before);
        Assert.That(insertResult, Is.Zero);

        int lpush = this._redis.List.LPush(key, [value1, value2, value3]);
        Assert.That(lpush, Is.EqualTo(3));

        string? element = this._redis.List.LIndex(key, 0);
        Assert.That(element, Is.EqualTo(value3));

        insertResult = this._redis.List.LInsert(key, value1, "LInsert_Before", RedisList.InsertPosition.Before);
        Assert.That(insertResult, Is.EqualTo(4));

        element = this._redis.List.LIndex(key, -1);
        Assert.That(element, Is.EqualTo(value1));

        element = this._redis.List.LIndex(key, -2);
        Assert.That(element, Is.EqualTo("LInsert_Before"));

        insertResult = this._redis.List.LInsert(key, value3, "LInsert_After", RedisList.InsertPosition.After);
        Assert.That(insertResult, Is.EqualTo(5));

        element = this._redis.List.LIndex(key, 1);
        Assert.That(element, Is.EqualTo("LInsert_After"));

        insertResult = this._redis.List.LInsert(key, "none", "LInsert_After", RedisList.InsertPosition.After);
        Assert.That(insertResult, Is.EqualTo(-1));
    }

    [Test]
    public void RPush()
    {
        const string key = "{146}:List:RPush:Key";
        const string value0 = "";
        const string value1 = "漂亮的让我面红的可爱女人💕";
        const string value2 = "💕温柔的让我心疼的可爱女人";

        long rpush = this._redis.List.RPush(key, value0);
        Assert.That(rpush, Is.EqualTo(1));

        rpush = this._redis.List.RPush(key, [value1, value2]);
        Assert.That(rpush, Is.EqualTo(3));

        string? element = this._redis.List.LIndex(key, 0);
        Assert.That(element, Is.EqualTo(string.Empty));

        element = this._redis.List.LIndex(key, 1);
        Assert.That(element, Is.EqualTo(value1));

        element = this._redis.List.LIndex(key, 2);
        Assert.That(element, Is.EqualTo(value2));

        rpush = this._redis.List.RPush(key, [value1 + "reverse", value2 + "reverse"], true);
        Assert.That(rpush, Is.EqualTo(5));

        element = this._redis.List.LIndex(key, -1);
        Assert.That(element, Is.EqualTo(value1 + "reverse"));

        element = this._redis.List.LIndex(key, -2);
        Assert.That(element, Is.EqualTo(value2 + "reverse"));
    }

    [Test]
    public void LMove()
    {
        const string sourceKey = "{147}:List:LMove:SourceKey";
        const string destinationKey = "{147}:List:LMove:DestinationKey";

        int sourcePushResult = this._redis.List.RPush(sourceKey, ["one", "two", "three"]);
        Assert.That(sourcePushResult, Is.EqualTo(3));

        string? sourceLast = this._redis.List.LIndex(sourceKey, -1);
        Assert.That(sourceLast, Is.EqualTo("three"));

        string? movedElement = this._redis.List.LMove(sourceKey, RedisList.WherePosition.Right, destinationKey, RedisList.WherePosition.Left);
        Assert.That(movedElement, Is.EqualTo("three"));

        sourceLast = this._redis.List.LIndex(sourceKey, -1);
        Assert.That(sourceLast, Is.EqualTo("two"));

        int sourceLen = this._redis.List.LLen(sourceKey);
        Assert.That(sourceLen, Is.EqualTo(2));

        int destinationLen = this._redis.List.LLen(destinationKey);
        Assert.That(destinationLen, Is.EqualTo(1));

        string? sourceFirst = this._redis.List.LIndex(sourceKey, 0);
        Assert.That(sourceFirst, Is.EqualTo("one"));

        movedElement = this._redis.List.LMove(sourceKey, RedisList.WherePosition.Right, sourceKey, RedisList.WherePosition.Left);
        Assert.That(movedElement, Is.EqualTo("two"));

        sourceFirst = this._redis.List.LIndex(sourceKey, 0);
        Assert.That(sourceFirst, Is.EqualTo("two"));

        movedElement = this._redis.List.LMove("none_list", RedisList.WherePosition.Right, destinationKey, RedisList.WherePosition.Left);
        Assert.That(movedElement, Is.Null);

        destinationLen = this._redis.List.LLen(destinationKey);
        Assert.That(destinationLen, Is.EqualTo(1));
    }

    [Test]
    public void LMPop()
    {
        const string key1 = "{148}:List:LMPop:Key1";
        const string key2 = "{148}:List:LMPop:Key2";
        const string key3 = "{148}:List:LMPop:Key3";

        var popReult = this._redis.List.LMPop([key1, key2, key3], RedisList.WherePosition.Left, 10);
        Assert.That(popReult.IsNull, Is.True);

        int list1PushResult = this._redis.List.RPush(key1, ["one", "two", "three"]);
        Assert.That(list1PushResult, Is.EqualTo(3));

        popReult = this._redis.List.LMPop([key2, key1], RedisList.WherePosition.Left, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(3));

            Assert.That(popReult[0].ToString(), Is.EqualTo("one"));
            Assert.That(popReult[1].ToString(), Is.EqualTo("two"));
            Assert.That(popReult[2].ToString(), Is.EqualTo("three"));
        }

        int list1Len = this._redis.List.LLen(key1);
        Assert.That(list1Len, Is.Zero);

        list1PushResult = this._redis.List.RPush(key1, ["one", "two", "three"]);
        Assert.That(list1PushResult, Is.EqualTo(3));

        int list2PushResult = this._redis.List.RPush(key2, ["1", "2", "3", "4", "5", "6"]);
        Assert.That(list2PushResult, Is.EqualTo(6));

        int list3PushResult = this._redis.List.LPush(key3, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"], true);
        Assert.That(list3PushResult, Is.EqualTo(26));

        popReult = this._redis.List.LMPop([key2, key1, key3], RedisList.WherePosition.Right, 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(1));

            Assert.That(popReult[0].ToString(), Is.EqualTo("6"));
        }

        int list2Len = this._redis.List.LLen(key2);
        Assert.That(list2Len, Is.EqualTo(5));

        list1Len = this._redis.List.LLen(key1);
        Assert.That(list1Len, Is.EqualTo(3));

        int list3Len = this._redis.List.LLen(key3);
        Assert.That(list3Len, Is.EqualTo(26));

        popReult = this._redis.List.LMPop([key3, key2, key1], RedisList.WherePosition.Right, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(10));

            Assert.That(popReult[0].ToString(), Is.EqualTo("z"));
            Assert.That(popReult[9].ToString(), Is.EqualTo("q"));
        }

        list2Len = this._redis.List.LLen(key2);
        Assert.That(list2Len, Is.EqualTo(5));

        list1Len = this._redis.List.LLen(key1);
        Assert.That(list1Len, Is.EqualTo(3));

        list3Len = this._redis.List.LLen(key3);
        Assert.That(list3Len, Is.EqualTo(16));
    }

    [Test]
    public void LPop()
    {
        const string key = "{149}:List:LPop:Key";

        string? pop = this._redis.List.LPop(key);
        Assert.That(pop, Is.Null);

        var pops = this._redis.List.LPop(key, 10);
        Assert.That(pops, Is.Null);

        int pushResult = this._redis.List.RPush(key, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"], true);
        Assert.That(pushResult, Is.EqualTo(26));

        pop = this._redis.List.LPop(key);
        Assert.That(pop, Is.EqualTo("z"));

        int len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(25));

        pops = this._redis.List.LPop(key, 10);
        Assert.That(pops, Is.Not.Null);
        Assert.That(pops, Has.Length.EqualTo(10));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pops[0].ToString(), Is.EqualTo("y"));
            Assert.That(pops[9].ToString(), Is.EqualTo("p"));
        }

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(15));

        pops = this._redis.List.LPop(key, 26);
        Assert.That(pops, Is.Not.Null);
        Assert.That(pops, Has.Length.EqualTo(15));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pops[0].ToString(), Is.EqualTo("o"));
            Assert.That(pops[14].ToString(), Is.EqualTo("a"));
        }

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.Zero);
    }

    [Test]
    public void LPos()
    {
        const string key = "{150}:List:LPos:Key";

        int pushResult = this._redis.List.RPush(key, ["a", "b", "c", "d", "1", "2", "3", "4", "3", "3", "3"]);
        Assert.That(pushResult, Is.EqualTo(11));

        var pos = this._redis.List.LPos(key, "111");
        Assert.That(pos.HasValue, Is.False);

        pos = this._redis.List.LPos("none_list", "a");
        Assert.That(pos.HasValue, Is.False);

        pos = this._redis.List.LPos(key, "c");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pos.HasValue, Is.True);
            Assert.That((int)pos, Is.EqualTo(2));
        }

        pos = this._redis.List.LPos(key, "c", rank: 2);
        Assert.That(pos.HasValue, Is.False);

        pos = this._redis.List.LPos(key, "3", rank: -1);
        Assert.That((int)pos, Is.EqualTo(10));

        var posArray = this._redis.List.LPos("none_list", "a", 3);
        Assert.That(posArray, Is.Null);

        posArray = this._redis.List.LPos(key, "3", 3);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(posArray, Has.Length.EqualTo(3));
            Assert.That((int)posArray![0], Is.EqualTo(6));
            Assert.That((int)posArray![2], Is.EqualTo(9));
        }

        posArray = this._redis.List.LPos(key, "3", 3, 2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(posArray, Has.Length.EqualTo(3));
            Assert.That((int)posArray![0], Is.EqualTo(8));
            Assert.That((int)posArray![2], Is.EqualTo(10));
        }

        posArray = this._redis.List.LPos(key, "3", 3, -2, 2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(posArray, Has.Length.EqualTo(1));
            Assert.That((int)posArray![0], Is.EqualTo(9));
        }
    }

    [Test]
    public void LPushX()
    {
        const string key = "{151}:List:LPushX:Key";
        const string value1 = "value1";
        const string value2 = "value2";
        const string value3 = "value3";
        const string value4 = "value4";
        const string value5 = "value5";

        int len = this._redis.List.LLen(key);
        Assert.That(len, Is.Zero);

        string? first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.Null);

        int lpush = this._redis.List.LPushX(key, value1);
        Assert.That(lpush, Is.Zero);

        lpush = this._redis.List.LPush(key, value1);
        Assert.That(lpush, Is.EqualTo(1));

        first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.EqualTo(value1));

        lpush = this._redis.List.LPushX(key, [value2, value3]);
        Assert.That(lpush, Is.EqualTo(3));

        first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.EqualTo(value3));

        string? last = this._redis.List.LIndex(key, -1);
        Assert.That(last, Is.EqualTo(value1));

        lpush = this._redis.List.LPushX(key, [value4, value5], true);
        Assert.That(lpush, Is.EqualTo(5));

        first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.EqualTo(value4));

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(5));
    }

    [Test]
    public void LRange()
    {
        const string key = "{152}:List:LRange:Key";
        int pushResult = this._redis.List.RPush(key, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"]);
        Assert.That(pushResult, Is.EqualTo(26));

        var range = this._redis.List.LRange(key, 0, 25);
        Assert.That(range, Has.Length.EqualTo(26));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("a"));
            Assert.That(range[25].ToString(), Is.EqualTo("z"));
            Assert.That(range[10].ToString(), Is.EqualTo("k"));
        }

        range = this._redis.List.LRange(key, -3, 25);
        Assert.That(range, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("x"));
            Assert.That(range[1].ToString(), Is.EqualTo("y"));
            Assert.That(range[2].ToString(), Is.EqualTo("z"));
        }

        range = this._redis.List.LRange("none_list", 0, 25);
        Assert.That(range, Is.Null);
    }

    [Test]
    public void LRem()
    {
        const string key = "{153}:List:LRem:Key";

        int pushResult = this._redis.List.RPush(key, ["a", "b", "c", "d", "1", "2", "3", "4", "3", "3", "3"]);
        Assert.That(pushResult, Is.EqualTo(11));

        int remResult = this._redis.List.LRem("none_list", "3", 0);
        Assert.That(remResult, Is.Zero);

        remResult = this._redis.List.LRem(key, "3", 0);
        Assert.That(remResult, Is.EqualTo(4));

        int len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(7));

        string? last = this._redis.List.LIndex(key, -1);
        Assert.That(last, Is.EqualTo("4"));

        pushResult = this._redis.List.RPush(key, ["a", "b", "c", "d", "1", "2", "3", "4", "3", "3", "3"]);
        Assert.That(pushResult, Is.EqualTo(18));

        remResult = this._redis.List.LRem(key, "3", -2);
        Assert.That(remResult, Is.EqualTo(2));

        last = this._redis.List.LIndex(key, -1);
        Assert.That(last, Is.EqualTo("3"));

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(16));

        remResult = this._redis.List.LRem(key, "3", 2);
        Assert.That(remResult, Is.EqualTo(2));

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(14));

        last = this._redis.List.LIndex(key, -1);
        Assert.That(last, Is.EqualTo("4"));

        remResult = this._redis.List.LRem(key, "333", 2);
        Assert.That(remResult, Is.Zero);
    }

    [Test]
    public void LSet()
    {
        const string key1 = "{154}:List:LSet:Key1";
        const string key2 = "{154}:List:LSet:Key2";

        int pushResult = this._redis.List.RPush(key1, ["a", "b", "c", "d", "1", "2", "3", "4", "3", "3", "3"]);
        Assert.That(pushResult, Is.EqualTo(11));

        int len = this._redis.List.LLen(key1);
        Assert.That(len, Is.EqualTo(11));

        string? element = this._redis.List.LIndex(key1, 0);
        Assert.That(element, Is.EqualTo("a"));

        element = this._redis.List.LIndex(key1, 1);
        Assert.That(element, Is.EqualTo("b"));

        var set = this._redis.List.LSet(key1, "Hello", 0);
        Assert.That(set, Is.True);

        len = this._redis.List.LLen(key1);
        Assert.That(len, Is.EqualTo(11));

        element = this._redis.List.LIndex(key1, 0);
        Assert.That(element, Is.EqualTo("Hello"));

        set = this._redis.List.LSet(key1, "Redis", 1);
        Assert.That(set, Is.True);

        element = this._redis.List.LIndex(key1, 1);
        Assert.That(element, Is.EqualTo("Redis"));

        set = this._redis.List.LSet(key1, "C# 666", -1);
        Assert.That(set, Is.True);

        element = this._redis.List.LIndex(key1, -1);
        Assert.That(element, Is.EqualTo("C# 666"));

        len = this._redis.List.LLen(key1);
        Assert.That(len, Is.EqualTo(11));

        len = this._redis.List.LLen(key2);
        Assert.That(len, Is.Zero);

        Assert.That(
            () => this._redis.List.LSet(key2, "Hello", 0),
            Throws.TypeOf<RedisErrorException>().And.Message.Contain("ERR no such key")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("LSET"));

        Assert.That(
            () => this._redis.List.LSet(key1, "C# 666", 20),
            Throws.TypeOf<RedisErrorException>().And.Message.Contain("ERR index out of range")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("LSET"));
    }

    [Test]
    public void LTrim()
    {
        const string key = "{155}:List:LTrim:Key";

        int pushResult = this._redis.List.RPush(key, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"]);
        Assert.That(pushResult, Is.EqualTo(26));

        string? last = this._redis.List.LIndex(key, -1);
        Assert.That(last, Is.EqualTo("z"));

        var trim = this._redis.List.LTrim(key, 0, 5);
        Assert.That(trim, Is.True);

        int len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(6));

        last = this._redis.List.LIndex(key, -1);
        Assert.That(last, Is.EqualTo("f"));

        trim = this._redis.List.LTrim(key, -3, -1);
        Assert.That(trim, Is.True);

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(3));

        last = this._redis.List.LIndex(key, -1);
        Assert.That(last, Is.EqualTo("f"));

        string? first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.EqualTo("d"));
    }

    [Test]
    public void RPop()
    {
        const string key = "{156}:List:RPop:Key";

        string? pop = this._redis.List.RPop(key);
        Assert.That(pop, Is.Null);

        var pops = this._redis.List.RPop(key, 10);
        Assert.That(pops, Is.Null);

        int pushResult = this._redis.List.RPush(key, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"]);
        Assert.That(pushResult, Is.EqualTo(26));

        pop = this._redis.List.RPop(key);
        Assert.That(pop, Is.EqualTo("z"));

        int len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(25));

        pops = this._redis.List.RPop(key, 10);
        Assert.That(pops, Is.Not.Null);
        Assert.That(pops, Has.Length.EqualTo(10));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pops[0].ToString(), Is.EqualTo("y"));
            Assert.That(pops[9].ToString(), Is.EqualTo("p"));
        }

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(15));

        pops = this._redis.List.RPop(key, 26);
        Assert.That(pops, Is.Not.Null);
        Assert.That(pops, Has.Length.EqualTo(15));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pops[0].ToString(), Is.EqualTo("o"));
            Assert.That(pops[14].ToString(), Is.EqualTo("a"));
        }

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.Zero);
    }

    [Test]
    public void RPopLPush()
    {
        const string sourceKey = "{157}:List:RPopLPush:SourceKey";
        const string destinationKey = "{157}:List:RPopLPush:DestinationKey";

        string? element = this._redis.List.RPopLPush(sourceKey, destinationKey);
        Assert.That(element, Is.Null);

        string? destinationFirst = this._redis.List.LIndex(destinationKey, 0);
        Assert.That(destinationFirst, Is.Null);

        int destinationLen = this._redis.List.LLen(destinationKey);
        Assert.That(destinationLen, Is.Zero);

        int sourceLen = this._redis.List.LLen(sourceKey);
        Assert.That(sourceLen, Is.Zero);

        int pushResult = this._redis.List.RPush(sourceKey, ["one", "two", "three"]);
        Assert.That(pushResult, Is.EqualTo(3));

        sourceLen = this._redis.List.LLen(sourceKey);
        Assert.That(sourceLen, Is.EqualTo(3));

        element = this._redis.List.RPopLPush(sourceKey, destinationKey);
        Assert.That(element, Is.EqualTo("three"));

        sourceLen = this._redis.List.LLen(sourceKey);
        Assert.That(sourceLen, Is.EqualTo(2));

        destinationLen = this._redis.List.LLen(destinationKey);
        Assert.That(destinationLen, Is.EqualTo(1));

        destinationFirst = this._redis.List.LIndex(destinationKey, 0);
        Assert.That(destinationFirst, Is.EqualTo("three"));

        element = this._redis.List.RPopLPush(sourceKey, destinationKey);
        Assert.That(element, Is.EqualTo("two"));

        destinationFirst = this._redis.List.LIndex(destinationKey, 0);
        Assert.That(destinationFirst, Is.EqualTo("two"));
    }

    [Test]
    public void RPushX()
    {
        const string key = "List:RPushX:Key";
        const string value1 = "value1";
        const string value2 = "value2";
        const string value3 = "value3";
        const string value4 = "value4";
        const string value5 = "value5";

        int len = this._redis.List.LLen(key);
        Assert.That(len, Is.Zero);

        string? first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.Null);

        int lpush = this._redis.List.RPushX(key, value1);
        Assert.That(lpush, Is.Zero);

        lpush = this._redis.List.RPush(key, value1);
        Assert.That(lpush, Is.EqualTo(1));

        first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.EqualTo(value1));

        lpush = this._redis.List.RPushX(key, [value2, value3]);
        Assert.That(lpush, Is.EqualTo(3));

        first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.EqualTo(value1));

        string? last = this._redis.List.LIndex(key, -1);
        Assert.That(last, Is.EqualTo(value3));

        lpush = this._redis.List.RPushX(key, [value4, value5], true);
        Assert.That(lpush, Is.EqualTo(5));

        first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.EqualTo(value1));

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(5));

        last = this._redis.List.LIndex(key, -1);
        Assert.That(last, Is.EqualTo(value4));
    }

    [Test]
    public void BLMove()
    {
        const string sourceKey = "{158}:List:BLMove:SourceKey";
        const string destinationKey = "{158}:List:BLMove:DestinationKey";

        var sw = Stopwatch.StartNew();
        string? movedElement = this._redis.List.BLMove(sourceKey, RedisList.WherePosition.Right, destinationKey, RedisList.WherePosition.Left, 1);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(100, 2500));
            Assert.That(movedElement, Is.Null);
        }

        int sourcePushResult = this._redis.List.RPush(sourceKey, ["one", "two", "three"]);
        Assert.That(sourcePushResult, Is.EqualTo(3));

        string? sourceLast = this._redis.List.LIndex(sourceKey, -1);
        Assert.That(sourceLast, Is.EqualTo("three"));

        movedElement = this._redis.List.BLMove(sourceKey, RedisList.WherePosition.Right, destinationKey, RedisList.WherePosition.Left, 0);
        Assert.That(movedElement, Is.EqualTo("three"));

        sourceLast = this._redis.List.LIndex(sourceKey, -1);
        Assert.That(sourceLast, Is.EqualTo("two"));

        int sourceLen = this._redis.List.LLen(sourceKey);
        Assert.That(sourceLen, Is.EqualTo(2));

        int destinationLen = this._redis.List.LLen(destinationKey);
        Assert.That(destinationLen, Is.EqualTo(1));

        string? sourceFirst = this._redis.List.LIndex(sourceKey, 0);
        Assert.That(sourceFirst, Is.EqualTo("one"));

        movedElement = this._redis.List.BLMove(sourceKey, RedisList.WherePosition.Right, sourceKey, RedisList.WherePosition.Left, 1);
        Assert.That(movedElement, Is.EqualTo("two"));

        sourceFirst = this._redis.List.LIndex(sourceKey, 0);
        Assert.That(sourceFirst, Is.EqualTo("two"));

        movedElement = this._redis.List.BLMove("none_list", RedisList.WherePosition.Right, destinationKey, RedisList.WherePosition.Left, 1);
        Assert.That(movedElement, Is.Null);

        destinationLen = this._redis.List.LLen(destinationKey);
        Assert.That(destinationLen, Is.EqualTo(1));
    }

    [Test]
    public void BLMPop()
    {
        const string key1 = "{159}:List:BLMPop:Key1";
        const string key2 = "{159}:List:BLMPop:Key2";
        const string key3 = "{159}:List:BLMPop:Key3";

        var sw = Stopwatch.StartNew();
        var popReult = this._redis.List.BLMPop([key1, key2, key3], RedisList.WherePosition.Left, 1);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(200, 2200));
            Assert.That(popReult.IsNull, Is.True);
        }

        int list1PushResult = this._redis.List.RPush(key1, ["one", "two", "three"]);
        Assert.That(list1PushResult, Is.EqualTo(3));

        popReult = this._redis.List.BLMPop([key2, key1], RedisList.WherePosition.Left, 1, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(3));

            Assert.That(popReult[0].ToString(), Is.EqualTo("one"));
            Assert.That(popReult[1].ToString(), Is.EqualTo("two"));
            Assert.That(popReult[2].ToString(), Is.EqualTo("three"));
        }

        int list1Len = this._redis.List.LLen(key1);
        Assert.That(list1Len, Is.Zero);

        list1PushResult = this._redis.List.RPush(key1, ["one", "two", "three"]);
        Assert.That(list1PushResult, Is.EqualTo(3));

        int list2PushResult = this._redis.List.RPush(key2, ["1", "2", "3", "4", "5", "6"]);
        Assert.That(list2PushResult, Is.EqualTo(6));

        int list3PushResult = this._redis.List.LPush(key3, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"], true);
        Assert.That(list3PushResult, Is.EqualTo(26));

        popReult = this._redis.List.BLMPop([key2, key1, key3], RedisList.WherePosition.Right, 1, 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(1));

            Assert.That(popReult[0].ToString(), Is.EqualTo("6"));
        }

        int list2Len = this._redis.List.LLen(key2);
        Assert.That(list2Len, Is.EqualTo(5));

        list1Len = this._redis.List.LLen(key1);
        Assert.That(list1Len, Is.EqualTo(3));

        int list3Len = this._redis.List.LLen(key3);
        Assert.That(list3Len, Is.EqualTo(26));

        popReult = this._redis.List.BLMPop([key3, key2, key1], RedisList.WherePosition.Right, 1, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(10));

            Assert.That(popReult[0].ToString(), Is.EqualTo("z"));
            Assert.That(popReult[9].ToString(), Is.EqualTo("q"));
        }

        list2Len = this._redis.List.LLen(key2);
        Assert.That(list2Len, Is.EqualTo(5));

        list1Len = this._redis.List.LLen(key1);
        Assert.That(list1Len, Is.EqualTo(3));

        list3Len = this._redis.List.LLen(key3);
        Assert.That(list3Len, Is.EqualTo(16));
    }

    [Test]
    public void BLPop()
    {
        const string key1 = "{160}:List:BLPop:Key1";
        const string key2 = "{160}:List:BLPop:Key2";
        const string key3 = "{160}:List:BLPop:Key3";

        var sw = Stopwatch.StartNew();
        var popReult = this._redis.List.BLPop([key1, key2, key3], 1);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(200, 1500));
            Assert.That(popReult.IsNull, Is.True);
        }

        sw = Stopwatch.StartNew();
        popReult = this._redis.List.BLPop([key1, key2, key3], 0.5);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(300, 800));
            Assert.That(popReult.IsNull, Is.True);
        }

        int ff = this._redis.List.RPush(key1, ["one", "two", "three"]);
        Assert.That(ff, Is.EqualTo(3));

        popReult = this._redis.List.BLPop([key1, key2, key3], 0.5);
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("one"));
        }

        popReult = this._redis.List.BLPop(key1, 5);
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("two"));
        }

        popReult = this._redis.List.BLPop(key1, 5.5);
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("three"));
        }
    }

    [Test]
    public void BRPop()
    {
        const string key1 = "{161}:List:BRPop:Key1";
        const string key2 = "{161}:List:BRPop:Key2";
        const string key3 = "{161}:List:BRPop:Key3";

        var sw = Stopwatch.StartNew();
        var popReult = this._redis.List.BRPop([key1, key2, key3], 1);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(500, 1500));
            Assert.That(popReult.IsNull, Is.True);
        }

        sw = Stopwatch.StartNew();
        popReult = this._redis.List.BRPop([key1, key2, key3], 0.5);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(300, 800));
            Assert.That(popReult.IsNull, Is.True);
        }

        int ff = this._redis.List.RPush(key1, ["one", "two", "three"]);
        Assert.That(ff, Is.EqualTo(3));

        popReult = this._redis.List.BRPop([key1, key2, key3], 0.5);
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("three"));
        }

        popReult = this._redis.List.BRPop(key1, 5);
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("two"));
        }

        popReult = this._redis.List.BRPop(key1, TimeSpan.FromSeconds(5.5));
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("one"));
        }
    }
}
