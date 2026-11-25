using SharpRedis;
using SharpRedis.Exceptions;
using System.Diagnostics;

namespace NET10.List;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class ListAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public async Task LPushAsync()
    {
        const string key = "{162}:List:LPushAsync:Key";
        const string value1 = "value1";
        const string value2 = "value2";
        const string value3 = "value3";
        const string value4 = "value4";
        const string value5 = "value5";

        int len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.Zero);

        string? first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.Null);

        int lpush = await this._redis.List.LPushAsync(key, value1);
        Assert.That(lpush, Is.EqualTo(1));

        first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.EqualTo(value1));

        lpush = await this._redis.List.LPushAsync(key, [value2, value3]);
        Assert.That(lpush, Is.EqualTo(3));

        first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.EqualTo(value3));

        string? last = await this._redis.List.LIndexAsync(key, -1);
        Assert.That(last, Is.EqualTo(value1));

        lpush = await this._redis.List.LPushAsync(key, [value4, value5], true);
        Assert.That(lpush, Is.EqualTo(5));

        first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.EqualTo(value4));

        len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.EqualTo(5));
    }

    [Test]
    public async Task LInsertAsync()
    {
        const string key = "{163}:List:LInsertAsync:Key";
        const string value1 = "一起长大的约定";
        const string value2 = "那样清晰";
        const string value3 = "打过勾的我相信";

        int insertResult = await this._redis.List.LInsertAsync(key, value1, value2, RedisList.InsertPosition.Before);
        Assert.That(insertResult, Is.Zero);

        int lpush = await this._redis.List.LPushAsync(key, [value1, value2, value3]);
        Assert.That(lpush, Is.EqualTo(3));

        string? element = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(element, Is.EqualTo(value3));

        insertResult = await this._redis.List.LInsertAsync(key, value1, "LInsert_Before", RedisList.InsertPosition.Before);
        Assert.That(insertResult, Is.EqualTo(4));

        element = await this._redis.List.LIndexAsync(key, -1);
        Assert.That(element, Is.EqualTo(value1));

        element = await this._redis.List.LIndexAsync(key, -2);
        Assert.That(element, Is.EqualTo("LInsert_Before"));

        insertResult = await this._redis.List.LInsertAsync(key, value3, "LInsert_After", RedisList.InsertPosition.After);
        Assert.That(insertResult, Is.EqualTo(5));

        element = await this._redis.List.LIndexAsync(key, 1);
        Assert.That(element, Is.EqualTo("LInsert_After"));

        insertResult = await this._redis.List.LInsertAsync(key, "none", "LInsert_After", RedisList.InsertPosition.After);
        Assert.That(insertResult, Is.EqualTo(-1));
    }

    [Test]
    public async Task RPushAsync()
    {
        const string key = "{164}:List:RPushAsync:Key";
        const string value0 = "";
        const string value1 = "漂亮的让我面红的可爱女人💕";
        const string value2 = "💕温柔的让我心疼的可爱女人";

        long rpush = await this._redis.List.RPushAsync(key, value0);
        Assert.That(rpush, Is.EqualTo(1));

        rpush = await this._redis.List.RPushAsync(key, [value1, value2]);
        Assert.That(rpush, Is.EqualTo(3));

        string? element = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(element, Is.EqualTo(string.Empty));

        element = await this._redis.List.LIndexAsync(key, 1);
        Assert.That(element, Is.EqualTo(value1));

        element = await this._redis.List.LIndexAsync(key, 2);
        Assert.That(element, Is.EqualTo(value2));

        rpush = await this._redis.List.RPushAsync(key, [value1 + "reverse", value2 + "reverse"], true);
        Assert.That(rpush, Is.EqualTo(5));

        element = await this._redis.List.LIndexAsync(key, -1);
        Assert.That(element, Is.EqualTo(value1 + "reverse"));

        element = await this._redis.List.LIndexAsync(key, -2);
        Assert.That(element, Is.EqualTo(value2 + "reverse"));
    }

    [Test]
    public async Task LMoveAsync()
    {
        const string sourceKey = "{165}:List:LMoveAsync:SourceKey";
        const string destinationKey = "{165}:List:LMoveAsync:DestinationKey";

        int sourcePushResult = await this._redis.List.RPushAsync(sourceKey, ["one", "two", "three"]);
        Assert.That(sourcePushResult, Is.EqualTo(3));

        string? sourceLast = await this._redis.List.LIndexAsync(sourceKey, -1);
        Assert.That(sourceLast, Is.EqualTo("three"));

        string? movedElement = await this._redis.List.LMoveAsync(sourceKey, RedisList.WherePosition.Right, destinationKey, RedisList.WherePosition.Left);
        Assert.That(movedElement, Is.EqualTo("three"));

        sourceLast = await this._redis.List.LIndexAsync(sourceKey, -1);
        Assert.That(sourceLast, Is.EqualTo("two"));

        int sourceLen = await this._redis.List.LLenAsync(sourceKey);
        Assert.That(sourceLen, Is.EqualTo(2));

        int destinationLen = await this._redis.List.LLenAsync(destinationKey);
        Assert.That(destinationLen, Is.EqualTo(1));

        string? sourceFirst = await this._redis.List.LIndexAsync(sourceKey, 0);
        Assert.That(sourceFirst, Is.EqualTo("one"));

        movedElement = await this._redis.List.LMoveAsync(sourceKey, RedisList.WherePosition.Right, sourceKey, RedisList.WherePosition.Left);
        Assert.That(movedElement, Is.EqualTo("two"));

        sourceFirst = await this._redis.List.LIndexAsync(sourceKey, 0);
        Assert.That(sourceFirst, Is.EqualTo("two"));

        movedElement = this._redis.List.LMove("none_list", RedisList.WherePosition.Right, destinationKey, RedisList.WherePosition.Left);
        Assert.That(movedElement, Is.Null);

        destinationLen = await this._redis.List.LLenAsync(destinationKey);
        Assert.That(destinationLen, Is.EqualTo(1));
    }

    [Test]
    public async Task LMPopAsync()
    {
        const string key1 = "{166}:List:LMPopAsync:Key1";
        const string key2 = "{166}:List:LMPopAsync:Key2";
        const string key3 = "{166}:List:LMPopAsync:Key3";

        var popReult = await this._redis.List.LMPopAsync([key1, key2, key3], RedisList.WherePosition.Left, 10);
        Assert.That(popReult.IsNull, Is.True);

        int list1PushResult = await this._redis.List.RPushAsync(key1, ["one", "two", "three"]);
        Assert.That(list1PushResult, Is.EqualTo(3));

        popReult = await this._redis.List.LMPopAsync([key2, key1], RedisList.WherePosition.Left, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(3));

            Assert.That(popReult[0].ToString(), Is.EqualTo("one"));
            Assert.That(popReult[1].ToString(), Is.EqualTo("two"));
            Assert.That(popReult[2].ToString(), Is.EqualTo("three"));
        }

        int list1Len = await this._redis.List.LLenAsync(key1);
        Assert.That(list1Len, Is.Zero);

        list1PushResult = await this._redis.List.RPushAsync(key1, ["one", "two", "three"]);
        Assert.That(list1PushResult, Is.EqualTo(3));

        int list2PushResult = await this._redis.List.RPushAsync(key2, ["1", "2", "3", "4", "5", "6"]);
        Assert.That(list2PushResult, Is.EqualTo(6));

        int list3PushResult = await this._redis.List.LPushAsync(key3, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"], true);
        Assert.That(list3PushResult, Is.EqualTo(26));

        popReult = await this._redis.List.LMPopAsync([key2, key1, key3], RedisList.WherePosition.Right, 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(1));

            Assert.That(popReult[0].ToString(), Is.EqualTo("6"));
        }

        int list2Len = await this._redis.List.LLenAsync(key2);
        Assert.That(list2Len, Is.EqualTo(5));

        list1Len = this._redis.List.LLen(key1);
        Assert.That(list1Len, Is.EqualTo(3));

        int list3Len = await this._redis.List.LLenAsync(key3);
        Assert.That(list3Len, Is.EqualTo(26));

        popReult = await this._redis.List.LMPopAsync([key3, key2, key1], RedisList.WherePosition.Right, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(10));

            Assert.That(popReult[0].ToString(), Is.EqualTo("z"));
            Assert.That(popReult[9].ToString(), Is.EqualTo("q"));
        }

        list2Len = await this._redis.List.LLenAsync(key2);
        Assert.That(list2Len, Is.EqualTo(5));

        list1Len = await this._redis.List.LLenAsync(key1);
        Assert.That(list1Len, Is.EqualTo(3));

        list3Len = await this._redis.List.LLenAsync(key3);
        Assert.That(list3Len, Is.EqualTo(16));
    }

    [Test]
    public async Task LPopAsync()
    {
        const string key = "{167}:List:LPopAsync:Key";

        string? pop = await this._redis.List.LPopAsync(key);
        Assert.That(pop, Is.Null);

        var pops = await this._redis.List.LPopAsync(key, 10);
        Assert.That(pops, Is.Null);

        int pushResult = await this._redis.List.RPushAsync(key, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"], true);
        Assert.That(pushResult, Is.EqualTo(26));

        pop = await this._redis.List.LPopAsync(key);
        Assert.That(pop, Is.EqualTo("z"));

        int len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.EqualTo(25));

        pops = await this._redis.List.LPopAsync(key, 10);
        Assert.That(pops, Is.Not.Null);
        Assert.That(pops, Has.Length.EqualTo(10));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pops[0].ToString(), Is.EqualTo("y"));
            Assert.That(pops[9].ToString(), Is.EqualTo("p"));
        }

        len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.EqualTo(15));

        pops = await this._redis.List.LPopAsync(key, 26);
        Assert.That(pops, Is.Not.Null);
        Assert.That(pops, Has.Length.EqualTo(15));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pops[0].ToString(), Is.EqualTo("o"));
            Assert.That(pops[14].ToString(), Is.EqualTo("a"));
        }

        len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.Zero);
    }

    [Test]
    public async Task LPosAsync()
    {
        const string key = "{168}:List:LPosAsync:Key";

        int pushResult = await this._redis.List.RPushAsync(key, ["a", "b", "c", "d", "1", "2", "3", "4", "3", "3", "3"]);
        Assert.That(pushResult, Is.EqualTo(11));

        var pos = await this._redis.List.LPosAsync(key, "111");
        Assert.That(pos.HasValue, Is.False);

        pos = await this._redis.List.LPosAsync("none_list", "a");
        Assert.That(pos.HasValue, Is.False);

        pos = this._redis.List.LPos(key, "c");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pos.HasValue, Is.True);
            Assert.That((int)pos, Is.EqualTo(2));
        }

        pos = await this._redis.List.LPosAsync(key, "c", rank: 2);
        Assert.That(pos.HasValue, Is.False);

        pos = await this._redis.List.LPosAsync(key, "3", rank: -1);
        Assert.That((int)pos, Is.EqualTo(10));

        var posArray = await this._redis.List.LPosAsync("none_list", "a", 3);
        Assert.That(posArray, Is.Null);

        posArray = await this._redis.List.LPosAsync(key, "3", 3);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(posArray, Has.Length.EqualTo(3));
            Assert.That((int)posArray![0], Is.EqualTo(6));
            Assert.That((int)posArray![2], Is.EqualTo(9));
        }

        posArray = await this._redis.List.LPosAsync(key, "3", 3, 2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(posArray, Has.Length.EqualTo(3));
            Assert.That((int)posArray![0], Is.EqualTo(8));
            Assert.That((int)posArray![2], Is.EqualTo(10));
        }

        posArray = await this._redis.List.LPosAsync(key, "3", 3, -2, 2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(posArray, Has.Length.EqualTo(1));
            Assert.That((int)posArray![0], Is.EqualTo(9));
        }
    }

    [Test]
    public async Task LPushXAsync()
    {
        const string key = "{169}:List:LPushXAsync:Key";
        const string value1 = "value1";
        const string value2 = "value2";
        const string value3 = "value3";
        const string value4 = "value4";
        const string value5 = "value5";

        int len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.Zero);

        string? first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.Null);

        int lpush = await this._redis.List.LPushXAsync(key, value1);
        Assert.That(lpush, Is.Zero);

        lpush = await this._redis.List.LPushAsync(key, value1);
        Assert.That(lpush, Is.EqualTo(1));

        first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.EqualTo(value1));

        lpush = await this._redis.List.LPushXAsync(key, [value2, value3]);
        Assert.That(lpush, Is.EqualTo(3));

        first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.EqualTo(value3));

        string? last = await this._redis.List.LIndexAsync(key, -1);
        Assert.That(last, Is.EqualTo(value1));

        lpush = await this._redis.List.LPushXAsync(key, [value4, value5], true);
        Assert.That(lpush, Is.EqualTo(5));

        first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.EqualTo(value4));

        len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.EqualTo(5));
    }

    [Test]
    public async Task LRangeAsync()
    {
        const string key = "{170}:List:LRangeAsync:Key";
        int pushResult = await this._redis.List.RPushAsync(key, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"]);
        Assert.That(pushResult, Is.EqualTo(26));

        var range = await this._redis.List.LRangeAsync(key, 0, 25);
        Assert.That(range, Has.Length.EqualTo(26));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("a"));
            Assert.That(range[25].ToString(), Is.EqualTo("z"));
            Assert.That(range[10].ToString(), Is.EqualTo("k"));
        }

        range = await this._redis.List.LRangeAsync(key, -3, 25);
        Assert.That(range, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(range[0].ToString(), Is.EqualTo("x"));
            Assert.That(range[1].ToString(), Is.EqualTo("y"));
            Assert.That(range[2].ToString(), Is.EqualTo("z"));
        }

        range = await this._redis.List.LRangeAsync("none_list", 0, 25);
        Assert.That(range, Is.Null);
    }

    [Test]
    public async Task LRemAsync()
    {
        const string key = "{171}:List:LRemAsync:Key";

        int pushResult = await this._redis.List.RPushAsync(key, ["a", "b", "c", "d", "1", "2", "3", "4", "3", "3", "3"]);
        Assert.That(pushResult, Is.EqualTo(11));

        int remResult = await this._redis.List.LRemAsync("none_list", "3", 0);
        Assert.That(remResult, Is.Zero);

        remResult = await this._redis.List.LRemAsync(key, "3", 0);
        Assert.That(remResult, Is.EqualTo(4));

        int len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.EqualTo(7));

        string? last = await this._redis.List.LIndexAsync(key, -1);
        Assert.That(last, Is.EqualTo("4"));

        pushResult = await this._redis.List.RPushAsync(key, ["a", "b", "c", "d", "1", "2", "3", "4", "3", "3", "3"]);
        Assert.That(pushResult, Is.EqualTo(18));

        remResult = await this._redis.List.LRemAsync(key, "3", -2);
        Assert.That(remResult, Is.EqualTo(2));

        last = await this._redis.List.LIndexAsync(key, -1);
        Assert.That(last, Is.EqualTo("3"));

        len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.EqualTo(16));

        remResult = await this._redis.List.LRemAsync(key, "3", 2);
        Assert.That(remResult, Is.EqualTo(2));

        len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.EqualTo(14));

        last = await this._redis.List.LIndexAsync(key, -1);
        Assert.That(last, Is.EqualTo("4"));

        remResult = await this._redis.List.LRemAsync(key, "none", 2);
        Assert.That(remResult, Is.Zero);
    }


    [Test]
    public async Task LSetAsync()
    {
        const string key1 = "{172}:List:LSetAsync:Key1";
        const string key2 = "{172}:List:LSetAsync:Key2";

        int pushResult = await this._redis.List.RPushAsync(key1, ["a", "b", "c", "d", "1", "2", "3", "4", "3", "3", "3"]);
        Assert.That(pushResult, Is.EqualTo(11));

        int len = await this._redis.List.LLenAsync(key1);
        Assert.That(len, Is.EqualTo(11));

        string? element = await this._redis.List.LIndexAsync(key1, 0);
        Assert.That(element, Is.EqualTo("a"));

        element = await this._redis.List.LIndexAsync(key1, 1);
        Assert.That(element, Is.EqualTo("b"));

        var set = await this._redis.List.LSetAsync(key1, "Hello", 0);
        Assert.That(set, Is.True);

        len = await this._redis.List.LLenAsync(key1);
        Assert.That(len, Is.EqualTo(11));

        element = await this._redis.List.LIndexAsync(key1, 0);
        Assert.That(element, Is.EqualTo("Hello"));

        set = await this._redis.List.LSetAsync(key1, "Redis", 1);
        Assert.That(set, Is.True);

        element = await this._redis.List.LIndexAsync(key1, 1);
        Assert.That(element, Is.EqualTo("Redis"));

        set = await this._redis.List.LSetAsync(key1, "C# 666", -1);
        Assert.That(set, Is.True);

        element = await this._redis.List.LIndexAsync(key1, -1);
        Assert.That(element, Is.EqualTo("C# 666"));

        len = await this._redis.List.LLenAsync(key1);
        Assert.That(len, Is.EqualTo(11));

        len = await this._redis.List.LLenAsync(key2);
        Assert.That(len, Is.Zero);

        Assert.That(
            async () => await this._redis.List.LSetAsync(key2, "Hello", 0).ConfigureAwait(false),
            Throws.TypeOf<RedisErrorException>().And.Message.Contain("ERR no such key")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("LSET"));

        Assert.That(
            async () => await this._redis.List.LSetAsync(key1, "C# 666", 20).ConfigureAwait(false),
            Throws.TypeOf<RedisErrorException>().And.Message.Contain("ERR index out of range")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("LSET"));
    }

    [Test]
    public async Task LTrimAsync()
    {
        const string key = "{173}:List:LTrimAsync:Key";

        int pushResult = await this._redis.List.RPushAsync(key, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"]);
        Assert.That(pushResult, Is.EqualTo(26));

        string? last = this._redis.List.LIndex(key, -1);
        Assert.That(last, Is.EqualTo("z"));

        var trim = await this._redis.List.LTrimAsync(key, 0, 5);
        Assert.That(trim, Is.True);

        int len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(6));

        last = await this._redis.List.LIndexAsync(key, -1);
        Assert.That(last, Is.EqualTo("f"));

        trim = await this._redis.List.LTrimAsync(key, -3, - 1);
        Assert.That(trim, Is.True);

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(3));

        last = await this._redis.List.LIndexAsync(key, -1);
        Assert.That(last, Is.EqualTo("f"));

        string? first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.EqualTo("d"));
    }

    [Test]
    public async Task RPopAsync()
    {
        const string key = "{174}:List:RPopAsync:Key";

        string? pop = await this._redis.List.RPopAsync(key);
        Assert.That(pop, Is.Null);

        var pops = await this._redis.List.RPopAsync(key, 10);
        Assert.That(pops, Is.Null);

        int pushResult = await this._redis.List.RPushAsync(key, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"]);
        Assert.That(pushResult, Is.EqualTo(26));

        pop = await this._redis.List.RPopAsync(key);
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

        len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.EqualTo(15));

        pops = await this._redis.List.RPopAsync(key, 26);
        Assert.That(pops, Is.Not.Null);
        Assert.That(pops, Has.Length.EqualTo(15));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pops[0].ToString(), Is.EqualTo("o"));
            Assert.That(pops[14].ToString(), Is.EqualTo("a"));
        }

        len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.Zero);
    }

    [Test]
    public async Task RPopLPushAsync()
    {
        const string sourceKey = "{175}:List:RPopLPushAsync:SourceKey";
        const string destinationKey = "{175}:List:RPopLPushAsync:DestinationKey";

        string? element = await this._redis.List.RPopLPushAsync(sourceKey, destinationKey);
        Assert.That(element, Is.Null);

        string? destinationFirst = await this._redis.List.LIndexAsync(destinationKey, 0);
        Assert.That(destinationFirst, Is.Null);

        int destinationLen = this._redis.List.LLen(destinationKey);
        Assert.That(destinationLen, Is.Zero);

        int sourceLen = await this._redis.List.LLenAsync(sourceKey);
        Assert.That(sourceLen, Is.Zero);

        int pushResult = await this._redis.List.RPushAsync(sourceKey, ["one", "two", "three"]);
        Assert.That(pushResult, Is.EqualTo(3));

        sourceLen = this._redis.List.LLen(sourceKey);
        Assert.That(sourceLen, Is.EqualTo(3));

        element = await this._redis.List.RPopLPushAsync(sourceKey, destinationKey);
        Assert.That(element, Is.EqualTo("three"));

        sourceLen = this._redis.List.LLen(sourceKey);
        Assert.That(sourceLen, Is.EqualTo(2));

        destinationLen = this._redis.List.LLen(destinationKey);
        Assert.That(destinationLen, Is.EqualTo(1));

        destinationFirst = await this._redis.List.LIndexAsync(destinationKey, 0);
        Assert.That(destinationFirst, Is.EqualTo("three"));

        element = await this._redis.List.RPopLPushAsync(sourceKey, destinationKey);
        Assert.That(element, Is.EqualTo("two"));

        destinationFirst = await this._redis.List.LIndexAsync(destinationKey, 0);
        Assert.That(destinationFirst, Is.EqualTo("two"));
    }

    [Test]
    public async Task RPushXAsync()
    {
        const string key = "{176}:List:RPushXAsync:Key";
        const string value1 = "value1";
        const string value2 = "value2";
        const string value3 = "value3";
        const string value4 = "value4";
        const string value5 = "value5";

        int len = await this._redis.List.LLenAsync(key);
        Assert.That(len, Is.Zero);

        string? first = this._redis.List.LIndex(key, 0);
        Assert.That(first, Is.Null);

        int lpush = await this._redis.List.RPushXAsync(key, value1);
        Assert.That(lpush, Is.Zero);

        lpush = await this._redis.List.RPushAsync(key, value1);
        Assert.That(lpush, Is.EqualTo(1));

        first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.EqualTo(value1));

        lpush = await this._redis.List.RPushXAsync(key, [value2, value3]);
        Assert.That(lpush, Is.EqualTo(3));

        first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.EqualTo(value1));

        string? last = await this._redis.List.LIndexAsync(key, -1);
        Assert.That(last, Is.EqualTo(value3));

        lpush = await this._redis.List.RPushXAsync(key, [value4, value5], true);
        Assert.That(lpush, Is.EqualTo(5));

        first = await this._redis.List.LIndexAsync(key, 0);
        Assert.That(first, Is.EqualTo(value1));

        len = this._redis.List.LLen(key);
        Assert.That(len, Is.EqualTo(5));

        last = await this._redis.List.LIndexAsync(key, -1);
        Assert.That(last, Is.EqualTo(value4));
    }

    [Test]
    public async Task BLMoveAsync()
    {
        const string sourceKey = "{177}:List:BLMoveAsync:SourceKey";
        const string destinationKey = "{177}:List:BLMoveAsync:DestinationKey";

        var sw = Stopwatch.StartNew();
        string? movedElement = await this._redis.List.BLMoveAsync(sourceKey, RedisList.WherePosition.Right, destinationKey, RedisList.WherePosition.Left, 1);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(500, 1500));
            Assert.That(movedElement, Is.Null);
        }

        int sourcePushResult = await this._redis.List.RPushAsync(sourceKey, ["one", "two", "three"]);
        Assert.That(sourcePushResult, Is.EqualTo(3));

        string? sourceLast = this._redis.List.LIndex(sourceKey, -1);
        Assert.That(sourceLast, Is.EqualTo("three"));

        movedElement = await this._redis.List.BLMoveAsync(sourceKey, RedisList.WherePosition.Right, destinationKey, RedisList.WherePosition.Left, 0);
        Assert.That(movedElement, Is.EqualTo("three"));

        sourceLast = await this._redis.List.LIndexAsync(sourceKey, -1);
        Assert.That(sourceLast, Is.EqualTo("two"));

        int sourceLen = await this._redis.List.LLenAsync(sourceKey);
        Assert.That(sourceLen, Is.EqualTo(2));

        int destinationLen = await this._redis.List.LLenAsync(destinationKey);
        Assert.That(destinationLen, Is.EqualTo(1));

        string? sourceFirst = await this._redis.List.LIndexAsync(sourceKey, 0);
        Assert.That(sourceFirst, Is.EqualTo("one"));

        movedElement = this._redis.List.BLMove(sourceKey, RedisList.WherePosition.Right, sourceKey, RedisList.WherePosition.Left, 1);
        Assert.That(movedElement, Is.EqualTo("two"));

        sourceFirst = await this._redis.List.LIndexAsync(sourceKey, 0);
        Assert.That(sourceFirst, Is.EqualTo("two"));

        movedElement = await this._redis.List.BLMoveAsync("none_list", RedisList.WherePosition.Right, destinationKey, RedisList.WherePosition.Left, 1);
        Assert.That(movedElement, Is.Null);

        destinationLen = await this._redis.List.LLenAsync(destinationKey);
        Assert.That(destinationLen, Is.EqualTo(1));
    }

    [Test]
    public async Task BLMPopAsync()
    {
        const string key1 = "{178}:List:BLMPopAsync:Key1";
        const string key2 = "{178}:List:BLMPopAsync:Key2";
        const string key3 = "{178}:List:BLMPopAsync:Key3";

        var sw = Stopwatch.StartNew();
        var popReult = await this._redis.List.BLMPopAsync([key1, key2, key3], RedisList.WherePosition.Left, 1);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(500, 2500));
            Assert.That(popReult.IsNull, Is.True);
        }

        int list1PushResult = await this._redis.List.RPushAsync(key1, ["one", "two", "three"]);
        Assert.That(list1PushResult, Is.EqualTo(3));

        popReult = await this._redis.List.BLMPopAsync([key2, key1], RedisList.WherePosition.Left, 1, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(3));

            Assert.That(popReult[0].ToString(), Is.EqualTo("one"));
            Assert.That(popReult[1].ToString(), Is.EqualTo("two"));
            Assert.That(popReult[2].ToString(), Is.EqualTo("three"));
        }

        int list1Len = await this._redis.List.LLenAsync(key1);
        Assert.That(list1Len, Is.Zero);

        list1PushResult = this._redis.List.RPush(key1, ["one", "two", "three"]);
        Assert.That(list1PushResult, Is.EqualTo(3));

        int list2PushResult = await this._redis.List.RPushAsync(key2, ["1", "2", "3", "4", "5", "6"]);
        Assert.That(list2PushResult, Is.EqualTo(6));

        int list3PushResult = this._redis.List.LPush(key3, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"], true);
        Assert.That(list3PushResult, Is.EqualTo(26));

        popReult = await this._redis.List.BLMPopAsync([key2, key1, key3], RedisList.WherePosition.Right, 1, 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(1));

            Assert.That(popReult[0].ToString(), Is.EqualTo("6"));
        }

        int list2Len = await this._redis.List.LLenAsync(key2);
        Assert.That(list2Len, Is.EqualTo(5));

        list1Len = await this._redis.List.LLenAsync(key1);
        Assert.That(list1Len, Is.EqualTo(3));

        int list3Len = await this._redis.List.LLenAsync(key3);
        Assert.That(list3Len, Is.EqualTo(26));

        popReult = await this._redis.List.BLMPopAsync([key3, key2, key1], RedisList.WherePosition.Right, 1, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(popReult, Has.Count.EqualTo(10));

            Assert.That(popReult[0].ToString(), Is.EqualTo("z"));
            Assert.That(popReult[9].ToString(), Is.EqualTo("q"));
        }

        list2Len = await this._redis.List.LLenAsync(key2);
        Assert.That(list2Len, Is.EqualTo(5));

        list1Len = await this._redis.List.LLenAsync(key1);
        Assert.That(list1Len, Is.EqualTo(3));

        list3Len = await this._redis.List.LLenAsync(key3);
        Assert.That(list3Len, Is.EqualTo(16));
    }

    [Test]
    public async Task BLPopAsync()
    {
        const string key1 = "{179}:List:BLPopAsync:Key1";
        const string key2 = "{179}:List:BLPopAsync:Key2";
        const string key3 = "{179}:List:BLPopAsync:Key3";

        var sw = Stopwatch.StartNew();
        var popReult = await this._redis.List.BLPopAsync([key1, key2, key3], 1);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(100, 3000));
            Assert.That(popReult.IsNull, Is.True);
        }

        sw = Stopwatch.StartNew();
        popReult = await this._redis.List.BLPopAsync([key1, key2, key3], 0.5);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(100, 800));
            Assert.That(popReult.IsNull, Is.True);
        }

        int ff = await this._redis.List.RPushAsync(key1, ["one", "two", "three"]);
        Assert.That(ff, Is.EqualTo(3));

        popReult = await this._redis.List.BLPopAsync([key1, key2, key3], 0.5);
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("one"));
        }

        popReult = await this._redis.List.BLPopAsync(key1, 5);
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("two"));
        }

        popReult = await this._redis.List.BLPopAsync(key1, 5.5);
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("three"));
        }
    }

    [Test]
    public async Task BRPopAsync()
    {
        const string key1 = "{180}:List:BRPopAsync:Key1";
        const string key2 = "{180}:List:BRPopAsync:Key2";
        const string key3 = "{180}:List:BRPopAsync:Key3";

        var sw = Stopwatch.StartNew();
        var popReult = await this._redis.List.BRPopAsync([key1, key2, key3], 1);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(150, 1500));
            Assert.That(popReult.IsNull, Is.True);
        }

        sw = Stopwatch.StartNew();
        popReult = await this._redis.List.BRPopAsync([key1, key2, key3], 0.5);
        sw.Stop();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sw.ElapsedMilliseconds, Is.InRange(300, 800));
            Assert.That(popReult.IsNull, Is.True);
        }

        int ff = await this._redis.List.RPushAsync(key1, ["one", "two", "three"]);
        Assert.That(ff, Is.EqualTo(3));

        popReult = await this._redis.List.BRPopAsync([key1, key2, key3], 0.5);
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("three"));
        }

        popReult = await this._redis.List.BRPopAsync(key1, 5);
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("two"));
        }

        popReult = await this._redis.List.BRPopAsync(key1, TimeSpan.FromSeconds(5.5));
        using (Assert.EnterMultipleScope())
        {
            var (key, element) = popReult;
            Assert.That(popReult.IsNull, Is.False);
            Assert.That(key.ToString(), Does.EndWith(key1));
            Assert.That(element.ToString(), Is.EqualTo("one"));
        }
    }
}
