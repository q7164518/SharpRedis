using SharpRedis;
using System.Text;

namespace NET8_Test.Types;

public class ListTest
{
    [Theory, ClassData(typeof(RedisProvider))]
    public void LIndex(Redis redis)
    {
        const string key = "list_lindex_key";
        _ = redis.Key.Del(key);
        var result = redis.List.RPush(key, ["a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(7, result);

        var member = redis.List.LIndex(key, 1);
        Assert.Equal("b", member);

        member = redis.List.LIndex(key, 100);
        Assert.Null(member);
        member = redis.List.LIndex("noneone_noen", 0);
        Assert.Null(member);

        var memberBytes = redis.List.LIndexBytes(key, -1);
        Assert.NotNull(memberBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("hello list"), memberBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LIndexAsync(Redis redis)
    {
        const string key = "list_lindex_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.List.RPushAsync(key, ["a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(7, result);

        var member = await redis.List.LIndexAsync(key, 1);
        Assert.Equal("b", member);

        member = await redis.List.LIndexAsync(key, 100);
        Assert.Null(member);
        member = await redis.List.LIndexAsync("noneone_noen", 0);
        Assert.Null(member);

        var memberBytes = await redis.List.LIndexBytesAsync(key, -1);
        Assert.NotNull(memberBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("hello list"), memberBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LInsert(Redis redis)
    {
        const string key = "list_linsert_key";
        _ = redis.Key.Del(key);
        _ = redis.List.RPush(key, ["a", "b", "c", "d", "e", "f", "hello list"]);

        var afterCount = redis.List.LInsert(key, "ahhh", "b", BeforeAfter.Before);
        Assert.Equal(8, afterCount);

        var element = redis.List.LIndex(key, 1);
        Assert.Equal("ahhh", element);

        afterCount = redis.List.LInsert(key, Encoding.UTF8.GetBytes("Redis list !!!"), "b", BeforeAfter.After);
        Assert.Equal(9, afterCount);

        element = redis.List.LIndex(key, 3);
        Assert.Equal("Redis list !!!", element);

        afterCount = redis.List.LInsert(key, Encoding.UTF8.GetBytes("list~~~   "), Encoding.UTF8.GetBytes("Redis list !!!"), BeforeAfter.After);
        Assert.Equal(10, afterCount);

        element = redis.List.LIndex(key, 4);
        Assert.Equal("list~~~   ", element);

        afterCount = redis.List.LInsert(key, "none", "jjjjjjj", BeforeAfter.After);
        Assert.Equal(-1, afterCount);

        afterCount = redis.List.LInsert("nononono", "none", "jjjjjjj", BeforeAfter.After);
        Assert.Equal(0, afterCount);

        var len = redis.List.LLen(key);
        Assert.Equal(10, len);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LInsertAsync(Redis redis)
    {
        const string key = "list_linsert_key_async";
        _ = await redis.Key.DelAsync(key);
        _ = await redis.List.RPushAsync(key, ["a", "b", "c", "d", "e", "f", "hello list"]);

        var afterCount = await redis.List.LInsertAsync(key, "ahhh", "b", BeforeAfter.Before);
        Assert.Equal(8, afterCount);

        var element = await redis.List.LIndexAsync(key, 1);
        Assert.Equal("ahhh", element);

        afterCount = await redis.List.LInsertAsync(key, Encoding.UTF8.GetBytes("Redis list !!!"), "b", BeforeAfter.After);
        Assert.Equal(9, afterCount);

        element = await redis.List.LIndexAsync(key, 3);
        Assert.Equal("Redis list !!!", element);

        afterCount = await redis.List.LInsertAsync(key, Encoding.UTF8.GetBytes("list~~~   "), Encoding.UTF8.GetBytes("Redis list !!!"), BeforeAfter.After);
        Assert.Equal(10, afterCount);

        element = await redis.List.LIndexAsync(key, 4);
        Assert.Equal("list~~~   ", element);

        afterCount = await redis.List.LInsertAsync(key, "none", "jjjjjjj", BeforeAfter.After);
        Assert.Equal(-1, afterCount);

        afterCount = await redis.List.LInsertAsync("nononono", "none", "jjjjjjj", BeforeAfter.After);
        Assert.Equal(0, afterCount);

        var len = await redis.List.LLenAsync(key);
        Assert.Equal(10, len);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LLen(Redis redis)
    {
        const string key = "list_llen_key";
        _ = redis.Key.Del(key);
        _ = redis.List.RPush(key, ["a", "b", "c", "d", "e", "f", "hello list"]);

        var len = redis.List.LLen(key);
        Assert.Equal(7, len);

        len = redis.List.LLen("none_list");
        Assert.Equal(0, len);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LLenAsync(Redis redis)
    {
        const string key = "list_llen_key_async";
        _ = await redis.Key.DelAsync(key);
        _ = await redis.List.RPushAsync(key, ["a", "b", "c", "d", "e", "f", "hello list"]);

        var len = await redis.List.LLenAsync(key);
        Assert.Equal(7, len);

        len = await redis.List.LLenAsync("none_list");
        Assert.Equal(0, len);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LMove(Redis redis)
    {
        const string key = "list_lmove_key";
        const string destination = "list_lmove_destination";
        _ = redis.Key.Del([key, destination]);
        _ = redis.List.RPush(key, ["a", "b", "c", "d", "e", "f", "hello list"]);

        var element = redis.List.LIndex(key, 0);
        Assert.NotNull(element);
        Assert.Equal("a", element);

        var destLen = redis.List.LLen(destination);
        Assert.Equal(0, destLen);

        element = redis.List.LMove(key, destination, LeftRight.Left, LeftRight.Right);
        Assert.NotNull(element);
        Assert.Equal("a", element);

        var keyLen = redis.List.LLen(key);
        Assert.Equal(6, keyLen);

        destLen = redis.List.LLen(destination);
        Assert.Equal(1, destLen);

        var destElement = redis.List.LIndex(destination, 0);
        Assert.NotNull(destElement);
        Assert.Equal("a", destElement);

        var elementBytes = redis.List.LMoveBytes(key, destination, LeftRight.Right, LeftRight.Left);
        Assert.NotNull(elementBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("hello list"), elementBytes);

        destElement = redis.List.LIndex(destination, -1);
        Assert.NotNull(destElement);
        Assert.Equal("a", destElement);

        var destElementBytes = redis.List.LIndexBytes(destination, 0);
        Assert.NotNull(destElementBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("hello list"), destElementBytes);

        destLen = redis.List.LLen(destination);
        Assert.Equal(2, destLen);

        element = redis.List.LMove("nonononono", destination, LeftRight.Left, LeftRight.Right);
        Assert.Null(element);

        destLen = redis.List.LLen(destination);
        Assert.Equal(2, destLen);

        elementBytes = redis.List.LMoveBytes("nonononono", destination, LeftRight.Right, LeftRight.Left);
        Assert.Null(elementBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LMoveAsync(Redis redis)
    {
        const string key = "list_lmove_key_async";
        const string destination = "list_lmove_destination_async";
        _ = await redis.Key.DelAsync([key, destination]);
        _ = await redis.List.RPushAsync(key, ["a", "b", "c", "d", "e", "f", "hello list"]);

        var element = await redis.List.LIndexAsync(key, 0);
        Assert.NotNull(element);
        Assert.Equal("a", element);

        var destLen = await redis.List.LLenAsync(destination);
        Assert.Equal(0, destLen);

        element = await redis.List.LMoveAsync(key, destination, LeftRight.Left, LeftRight.Right);
        Assert.NotNull(element);
        Assert.Equal("a", element);

        var keyLen = await redis.List.LLenAsync(key);
        Assert.Equal(6, keyLen);

        destLen = await redis.List.LLenAsync(destination);
        Assert.Equal(1, destLen);

        var destElement = await redis.List.LIndexAsync(destination, 0);
        Assert.NotNull(destElement);
        Assert.Equal("a", destElement);

        var elementBytes = await redis.List.LMoveBytesAsync(key, destination, LeftRight.Right, LeftRight.Left);
        Assert.NotNull(elementBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("hello list"), elementBytes);

        destElement = await redis.List.LIndexAsync(destination, -1);
        Assert.NotNull(destElement);
        Assert.Equal("a", destElement);

        var destElementBytes = await redis.List.LIndexBytesAsync(destination, 0);
        Assert.NotNull(destElementBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("hello list"), destElementBytes);

        destLen = await redis.List.LLenAsync(destination);
        Assert.Equal(2, destLen);

        element = await redis.List.LMoveAsync("nonononono", destination, LeftRight.Left, LeftRight.Right);
        Assert.Null(element);

        destLen = await redis.List.LLenAsync(destination);
        Assert.Equal(2, destLen);

        elementBytes = await redis.List.LMoveBytesAsync("nonononono", destination, LeftRight.Right, LeftRight.Left);
        Assert.Null(elementBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LMPop(Redis redis)
    {
        const string key1 = "list_lmpop_key1";
        const string key2 = "list_lmpop_key2";
        _ = redis.Key.Del([key1, key2]);
        _ = redis.List.RPush(key1, ["a", "b", "c", "d", "e", "f"]);
        _ = redis.List.RPush(key2, ["g", "h", "i", "j", "k", "l"]);

        var lmpop = redis.List.LMPop([key1, key2]);
        Assert.True(lmpop.HasValue);
        Assert.Equal(key1, lmpop.Value.Key);
        Assert.Equal("a", lmpop.Value.Value);

        lmpop = redis.List.LMPop([key1, key2]);
        Assert.True(lmpop.HasValue);
        Assert.Equal(key1, lmpop.Value.Key);
        Assert.Equal("b", lmpop.Value.Value);

        lmpop = redis.List.LMPop([key1, key2], LeftRight.Right);
        Assert.True(lmpop.HasValue);
        Assert.Equal(key1, lmpop.Value.Key);
        Assert.Equal("f", lmpop.Value.Value);

        lmpop = redis.List.LMPop([key2, key1], LeftRight.Right);
        Assert.True(lmpop.HasValue);
        Assert.Equal(key2, lmpop.Value.Key);
        Assert.Equal("l", lmpop.Value.Value);

        lmpop = redis.List.LMPop(["dasfdsafsd", "nononononononono"], LeftRight.Right);
        Assert.False(lmpop.HasValue);

        var lmpopArray = redis.List.LMPop([key2, key1], 3, LeftRight.Right);
        Assert.True(lmpopArray.HasValue);
        Assert.Equal(key2, lmpopArray.Value.Key);
        Assert.Equal(3, lmpopArray.Value.Value.Length);
        Assert.Equal<string[]>(["k", "j", "i"], lmpopArray.Value.Value);

        lmpopArray = redis.List.LMPop(["dasfdsafsd", "nononononononono"], 3, LeftRight.Right);
        Assert.False(lmpopArray.HasValue);

        lmpopArray = redis.List.LMPop([key2, key1], 30, LeftRight.Right);
        Assert.True(lmpopArray.HasValue);
        Assert.Equal(key2, lmpopArray.Value.Key);
        Assert.Equal(2, lmpopArray.Value.Value.Length);
        Assert.Equal<string[]>(["h", "g"], lmpopArray.Value.Value);

        var lmpopBytes = redis.List.LMPopBytes([key1, key2]);
        Assert.True(lmpopBytes.HasValue);
        Assert.Equal(key1, lmpopBytes.Value.Key);
        Assert.Equal(Encoding.UTF8.GetBytes("c"), lmpopBytes.Value.Value);

        var lmpopBytesArray = redis.List.LMPopBytes([key1, key2], 100, LeftRight.Right);
        Assert.True(lmpopBytesArray.HasValue);
        Assert.Equal(key1, lmpopBytesArray.Value.Key);
        Assert.Equal([Encoding.UTF8.GetBytes("e"), Encoding.UTF8.GetBytes("d")], lmpopBytesArray.Value.Value);

        lmpopBytesArray = redis.List.LMPopBytes([key1, key2], 100, LeftRight.Right);
        Assert.Null(lmpopBytesArray);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LMPopAsync(Redis redis)
    {
        const string key1 = "list_lmpop_key1_async";
        const string key2 = "list_lmpop_key2_async";
        _ = await redis.Key.DelAsync([key1, key2]);
        _ = await redis.List.RPushAsync(key1, ["a", "b", "c", "d", "e", "f"]);
        _ = await redis.List.RPushAsync(key2, ["g", "h", "i", "j", "k", "l"]);

        var lmpop = await redis.List.LMPopAsync([key1, key2]);
        Assert.True(lmpop.HasValue);
        Assert.Equal(key1, lmpop.Value.Key);
        Assert.Equal("a", lmpop.Value.Value);

        lmpop = await redis.List.LMPopAsync([key1, key2]);
        Assert.True(lmpop.HasValue);
        Assert.Equal(key1, lmpop.Value.Key);
        Assert.Equal("b", lmpop.Value.Value);

        lmpop = await redis.List.LMPopAsync([key1, key2], LeftRight.Right);
        Assert.True(lmpop.HasValue);
        Assert.Equal(key1, lmpop.Value.Key);
        Assert.Equal("f", lmpop.Value.Value);

        lmpop = await redis.List.LMPopAsync([key2, key1], LeftRight.Right);
        Assert.True(lmpop.HasValue);
        Assert.Equal(key2, lmpop.Value.Key);
        Assert.Equal("l", lmpop.Value.Value);

        lmpop = await redis.List.LMPopAsync(["dasfdsafsd", "nononononononono"], LeftRight.Right);
        Assert.False(lmpop.HasValue);

        var lmpopArray = await redis.List.LMPopAsync([key2, key1], 3, LeftRight.Right);
        Assert.True(lmpopArray.HasValue);
        Assert.Equal(key2, lmpopArray.Value.Key);
        Assert.Equal(3, lmpopArray.Value.Value.Length);
        Assert.Equal<string[]>(["k", "j", "i"], lmpopArray.Value.Value);

        lmpopArray = await redis.List.LMPopAsync(["dasfdsafsd", "nononononononono"], 3, LeftRight.Right);
        Assert.False(lmpopArray.HasValue);

        lmpopArray = await redis.List.LMPopAsync([key2, key1], 30, LeftRight.Right);
        Assert.True(lmpopArray.HasValue);
        Assert.Equal(key2, lmpopArray.Value.Key);
        Assert.Equal(2, lmpopArray.Value.Value.Length);
        Assert.Equal<string[]>(["h", "g"], lmpopArray.Value.Value);

        var lmpopBytes = await redis.List.LMPopBytesAsync([key1, key2]);
        Assert.True(lmpopBytes.HasValue);
        Assert.Equal(key1, lmpopBytes.Value.Key);
        Assert.Equal(Encoding.UTF8.GetBytes("c"), lmpopBytes.Value.Value);

        var lmpopBytesArray = await redis.List.LMPopBytesAsync([key1, key2], 100, LeftRight.Right);
        Assert.True(lmpopBytesArray.HasValue);
        Assert.Equal(key1, lmpopBytesArray.Value.Key);
        Assert.Equal([Encoding.UTF8.GetBytes("e"), Encoding.UTF8.GetBytes("d")], lmpopBytesArray.Value.Value);

        lmpopBytesArray = await redis.List.LMPopBytesAsync([key1, key2], 100, LeftRight.Right);
        Assert.Null(lmpopBytesArray);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LPop(Redis redis)
    {
        const string key = "list_lpop_key";
        _ = redis.Key.Del(key);
        _ = redis.List.RPush(key, ["a", "b", "c", "d", "e", "f", "hello list"]);

        var left = redis.List.LPop(key);
        Assert.Equal("a", left);

        var leftArray = redis.List.LPop(key, 2);
        Assert.Equal<string[]>(["b", "c"], leftArray);

        var leftBytes = redis.List.LPopBytes(key);
        Assert.Equal(Encoding.UTF8.GetBytes("d"), leftBytes);

        var leftArrayBytes = redis.List.LPopBytes(key, 3);
        Assert.Equal([Encoding.UTF8.GetBytes("e"), Encoding.UTF8.GetBytes("f"), Encoding.UTF8.GetBytes("hello list")], leftArrayBytes);

        left = redis.List.LPop(key);
        Assert.Null(left);

        leftBytes = redis.List.LPopBytes(key);
        Assert.Null(leftBytes);

        leftArray = redis.List.LPop(key, 2);
        Assert.Null(leftArray);

        leftArrayBytes = redis.List.LPopBytes(key, 3);
        Assert.Null(leftArrayBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LPopAsync(Redis redis)
    {
        const string key = "list_lpop_key_async";
        _ = await redis.Key.DelAsync(key);
        _ = await redis.List.RPushAsync(key, ["a", "b", "c", "d", "e", "f", "hello list"]);

        var left = await redis.List.LPopAsync(key);
        Assert.Equal("a", left);

        var leftArray = await redis.List.LPopAsync(key, 2);
        Assert.Equal<string[]>(["b", "c"], leftArray);

        var leftBytes = await redis.List.LPopBytesAsync(key);
        Assert.Equal(Encoding.UTF8.GetBytes("d"), leftBytes);

        var leftArrayBytes = await redis.List.LPopBytesAsync(key, 30);
        Assert.Equal([Encoding.UTF8.GetBytes("e"), Encoding.UTF8.GetBytes("f"), Encoding.UTF8.GetBytes("hello list")], leftArrayBytes);

        left = await redis.List.LPopAsync(key);
        Assert.Null(left);

        leftBytes = await redis.List.LPopBytesAsync(key);
        Assert.Null(leftBytes);

        leftArray = await redis.List.LPopAsync(key, 2);
        Assert.Null(leftArray);

        leftArrayBytes = await redis.List.LPopBytesAsync(key, 3);
        Assert.Null(leftArrayBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LPos(Redis redis)
    {
        const string key = "list_lpos_key";
        _ = redis.Key.Del(key);
        _ = redis.List.RPush(key, ["a", "b", "c", "1", "2", "3", "c", "c"]);

        var index = redis.List.LPos(key, "c");
        Assert.True(index.HasValue);
        Assert.Equal(2, index.Value);

        index = redis.List.LPos(key, "c", 2);
        Assert.True(index.HasValue);
        Assert.Equal(6, index.Value);

        index = redis.List.LPos(key, "c", -1);
        Assert.True(index.HasValue);
        Assert.Equal(7, index.Value);

        index = redis.List.LPos(key, Encoding.UTF8.GetBytes("3"), -1);
        Assert.True(index.HasValue);
        Assert.Equal(5, index.Value);

        index = redis.List.LPos(key, Encoding.UTF8.GetBytes("3333"), -1);
        Assert.False(index.HasValue);

        var indexArray = redis.List.LPos(2, key, "c");
        Assert.NotNull(indexArray);
        Assert.Equal(2, indexArray.Length);
        Assert.Equal([2, 6], indexArray);

        indexArray = redis.List.LPos(2, key, "c", -1);
        Assert.NotNull(indexArray);
        Assert.Equal(2, indexArray.Length);
        Assert.Equal([7, 6], indexArray);

        indexArray = redis.List.LPos(0, key, "c");
        Assert.NotNull(indexArray);
        Assert.Equal(3, indexArray.Length);
        Assert.Equal([2, 6, 7], indexArray);

        indexArray = redis.List.LPos(1, key, Encoding.UTF8.GetBytes("c"));
        Assert.NotNull(indexArray);
        Assert.Single(indexArray);
        Assert.Equal([2], indexArray);

        indexArray = redis.List.LPos(0, key, "ccccc");
        Assert.Null(indexArray);

        indexArray = redis.List.LPos(0, "noniononono", "c");
        Assert.Null(indexArray);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LPosAsync(Redis redis)
    {
        const string key = "list_lpos_key_async";
        _ = await redis.Key.DelAsync(key);
        _ = await redis.List.RPushAsync(key, ["a", "b", "c", "1", "2", "3", "c", "c"]);

        var index = await redis.List.LPosAsync(key, "c");
        Assert.True(index.HasValue);
        Assert.Equal(2, index.Value);

        index = await redis.List.LPosAsync(key, "c", 2);
        Assert.True(index.HasValue);
        Assert.Equal(6, index.Value);

        index = await redis.List.LPosAsync(key, "c", -1);
        Assert.True(index.HasValue);
        Assert.Equal(7, index.Value);

        index = await redis.List.LPosAsync(key, Encoding.UTF8.GetBytes("3"), -1);
        Assert.True(index.HasValue);
        Assert.Equal(5, index.Value);

        index = await redis.List.LPosAsync(key, Encoding.UTF8.GetBytes("3333"), -1);
        Assert.False(index.HasValue);

        var indexArray = await redis.List.LPosAsync(2, key, "c");
        Assert.NotNull(indexArray);
        Assert.Equal(2, indexArray.Length);
        Assert.Equal([2, 6], indexArray);

        indexArray = await redis.List.LPosAsync(2, key, "c", -1);
        Assert.NotNull(indexArray);
        Assert.Equal(2, indexArray.Length);
        Assert.Equal([7, 6], indexArray);

        indexArray = await redis.List.LPosAsync(0, key, "c");
        Assert.NotNull(indexArray);
        Assert.Equal(3, indexArray.Length);
        Assert.Equal([2, 6, 7], indexArray);

        indexArray = await redis.List.LPosAsync(1, key, Encoding.UTF8.GetBytes("c"));
        Assert.NotNull(indexArray);
        Assert.Single(indexArray);
        Assert.Equal([2], indexArray);

        indexArray = await redis.List.LPosAsync(0, key, "ccccc");
        Assert.Null(indexArray);

        indexArray = await redis.List.LPosAsync(0, "noniononono", "c");
        Assert.Null(indexArray);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LPush(Redis redis)
    {
        const string key = "list_lpush_key";
        _ = redis.Key.Del(key);
        var result = redis.List.LPush(key, ["a", "b", "c", "1", "2", "3", "c", "c"]);
        Assert.Equal(8, result);

        var len = redis.List.LLen(key);
        Assert.Equal(8, len);

        var element = redis.List.LIndex(key, 0);
        Assert.Equal("c", element);

        element = redis.List.LIndex(key, -1);
        Assert.Equal("a", element);

        result = redis.List.LPush(key, "hello");
        Assert.Equal(9, result);

        result = redis.List.LPush(key, Encoding.UTF8.GetBytes("China"));
        Assert.Equal(10, result);

        element = redis.List.LIndex(key, 0);
        Assert.Equal("China", element);

        result = redis.List.LPush(key, [Encoding.UTF8.GetBytes("USA"), Encoding.UTF8.GetBytes("AAA")]);
        Assert.Equal(12, result);

        element = redis.List.LIndex(key, 0);
        Assert.Equal("AAA", element);

        element = redis.List.LIndex(key, -1);
        Assert.Equal("a", element);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LPushAsync(Redis redis)
    {
        const string key = "list_lpush_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.List.LPushAsync(key, ["a", "b", "c", "1", "2", "3", "c", "c"]);
        Assert.Equal(8, result);

        var len = await redis.List.LLenAsync(key);
        Assert.Equal(8, len);

        var element = await redis.List.LIndexAsync(key, 0);
        Assert.Equal("c", element);

        element = await redis.List.LIndexAsync(key, -1);
        Assert.Equal("a", element);

        result = await redis.List.LPushAsync(key, "hello");
        Assert.Equal(9, result);

        result = await redis.List.LPushAsync(key, Encoding.UTF8.GetBytes("China"));
        Assert.Equal(10, result);

        element = await redis.List.LIndexAsync(key, 0);
        Assert.Equal("China", element);

        result = await redis.List.LPushAsync(key, [Encoding.UTF8.GetBytes("USA"), Encoding.UTF8.GetBytes("AAA")]);
        Assert.Equal(12, result);

        element = await redis.List.LIndexAsync(key, 0);
        Assert.Equal("AAA", element);

        element = await redis.List.LIndexAsync(key, -1);
        Assert.Equal("a", element);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LPushX(Redis redis)
    {
        const string key = "list_lpushx_key";
        _ = redis.Key.Del(key);

        var result = redis.List.LPushX(key, ["a", "b", "c"]);
        Assert.Equal(0, result);

        result = redis.List.LPushX(key, Encoding.UTF8.GetBytes("aa"));
        Assert.Equal(0, result);

        var element = redis.List.LIndex(key, 0);
        Assert.Null(element);

        _ = redis.List.LPush(key, ["a", "b", "c"]);

        result = redis.List.LPushX(key, ["d", "e", "f"]);
        Assert.Equal(6, result);

        element = redis.List.LIndex(key, 0);
        Assert.Equal("f", element);

        result = redis.List.LPushX(key, "haha");
        Assert.Equal(7, result);

        result = redis.List.LPushX(key, [Encoding.UTF8.GetBytes("aa"), Encoding.UTF8.GetBytes("bb")]);
        Assert.Equal(9, result);

        element = redis.List.LIndex(key, 0);
        Assert.Equal("bb", element);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LPushXAsync(Redis redis)
    {
        const string key = "list_lpushx_key_async";
        _ = await redis.Key.DelAsync(key);

        var result = await redis.List.LPushXAsync(key, ["a", "b", "c"]);
        Assert.Equal(0, result);

        result = await redis.List.LPushXAsync(key, Encoding.UTF8.GetBytes("aa"));
        Assert.Equal(0, result);

        var element = await redis.List.LIndexAsync(key, 0);
        Assert.Null(element);

        _ = await redis.List.LPushAsync(key, ["a", "b", "c"]);

        result = await redis.List.LPushXAsync(key, ["d", "e", "f"]);
        Assert.Equal(6, result);

        element = await redis.List.LIndexAsync(key, 0);
        Assert.Equal("f", element);

        result = await redis.List.LPushXAsync(key, "haha");
        Assert.Equal(7, result);

        result = await redis.List.LPushXAsync(key, [Encoding.UTF8.GetBytes("aa"), Encoding.UTF8.GetBytes("bb")]);
        Assert.Equal(9, result);

        element = await redis.List.LIndexAsync(key, 0);
        Assert.Equal("bb", element);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void RPush(Redis redis)
    {
        const string key = "list_rpush_key";
        _ = redis.Key.Del(key);
        var result = redis.List.RPush(key, ["a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(7, result);

        result = redis.List.RPush(key, "lwj");
        Assert.Equal(8, result);

        result = redis.List.RPush(key, Encoding.UTF8.GetBytes("luozi"));
        Assert.Equal(9, result);

        var element = redis.List.LIndex(key, 0);
        Assert.Equal("a", element);

        element = redis.List.LIndex(key, -1);
        Assert.Equal("luozi", element);

        element = redis.List.LIndex(key, -2);
        Assert.Equal("lwj", element);

        result = redis.List.RPush(key, [Encoding.UTF8.GetBytes("feizi"), Encoding.UTF8.GetBytes("gpxiiii")]);
        Assert.Equal(11, result);

        element = redis.List.LIndex(key, -2);
        Assert.Equal("feizi", element);

        var len = redis.List.LLen(key);
        Assert.Equal(11, len);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task RPushAsync(Redis redis)
    {
        const string key = "list_rpush_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.List.RPushAsync(key, ["a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(7, result);

        result = await redis.List.RPushAsync(key, "lwj");
        Assert.Equal(8, result);

        result = await redis.List.RPushAsync(key, Encoding.UTF8.GetBytes("luozi"));
        Assert.Equal(9, result);

        var element = await redis.List.LIndexAsync(key, 0);
        Assert.Equal("a", element);

        element = await redis.List.LIndexAsync(key, -1);
        Assert.Equal("luozi", element);

        element = await redis.List.LIndexAsync(key, -2);
        Assert.Equal("lwj", element);

        result = await redis.List.RPushAsync(key, [Encoding.UTF8.GetBytes("feizi"), Encoding.UTF8.GetBytes("gpxiiii")]);
        Assert.Equal(11, result);

        element = await redis.List.LIndexAsync(key, -2);
        Assert.Equal("feizi", element);

        var len = await redis.List.LLenAsync(key);
        Assert.Equal(11, len);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void RPushX(Redis redis)
    {
        const string key = "list_rpushx_key";
        _ = redis.Key.Del(key);
        var result = redis.List.RPushX(key, ["a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(0, result);

        result = redis.List.RPush(key, "kk");
        Assert.Equal(1, result);

        result = redis.List.RPushX(key, ["a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(8, result);

        result = redis.List.RPushX(key, "lwj");
        Assert.Equal(9, result);

        result = redis.List.RPushX(key, Encoding.UTF8.GetBytes("luozi"));
        Assert.Equal(10, result);

        var element = redis.List.LIndex(key, 0);
        Assert.Equal("kk", element);

        element = redis.List.LIndex(key, -1);
        Assert.Equal("luozi", element);

        element = redis.List.LIndex(key, -2);
        Assert.Equal("lwj", element);

        result = redis.List.RPushX(key, [Encoding.UTF8.GetBytes("feizi"), Encoding.UTF8.GetBytes("gpxiiii")]);
        Assert.Equal(12, result);

        element = redis.List.LIndex(key, -2);
        Assert.Equal("feizi", element);

        var len = redis.List.LLen(key);
        Assert.Equal(12, len);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task RPushXAsync(Redis redis)
    {
        const string key = "list_rpushx_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.List.RPushXAsync(key, ["a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(0, result);

        result = await redis.List.RPushAsync(key, "kk");
        Assert.Equal(1, result);

        result = await redis.List.RPushXAsync(key, ["a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(8, result);

        result = await redis.List.RPushXAsync(key, "lwj");
        Assert.Equal(9, result);

        result = await redis.List.RPushXAsync(key, Encoding.UTF8.GetBytes("luozi"));
        Assert.Equal(10, result);

        var element = await redis.List.LIndexAsync(key, 0);
        Assert.Equal("kk", element);

        element = await redis.List.LIndexAsync(key, -1);
        Assert.Equal("luozi", element);

        element = await redis.List.LIndexAsync(key, -2);
        Assert.Equal("lwj", element);

        result = await redis.List.RPushXAsync(key, [Encoding.UTF8.GetBytes("feizi"), Encoding.UTF8.GetBytes("gpxiiii")]);
        Assert.Equal(12, result);

        element = await redis.List.LIndexAsync(key, -2);
        Assert.Equal("feizi", element);

        var len = await redis.List.LLenAsync(key);
        Assert.Equal(12, len);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LRange(Redis redis)
    {
        const string key = "list_lrange_key";
        _ = redis.Key.Del(key);
        var result = redis.List.RPush(key, ["one", "two", "three"]);
        Assert.Equal(3, result);

        var range = redis.List.LRange(key, 0, 0);
        Assert.NotNull(range);
        Assert.Single(range);
        Assert.Equal("one", range[0]);

        range = redis.List.LRange(key, -3, 2);
        Assert.NotNull(range);
        Assert.Equal(3, range.Length);
        Assert.Equal<string[]>(["one", "two", "three"], range);

        range = redis.List.LRange(key, -100, 100);
        Assert.NotNull(range);
        Assert.Equal(3, range.Length);
        Assert.Equal<string[]>(["one", "two", "three"], range);

        range = redis.List.LRange(key, 5, 100);
        Assert.Null(range);

        var rangeBytes = redis.List.LRangeBytes(key, 1, 6);
        Assert.NotNull(rangeBytes);
        Assert.Equal(2, rangeBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("two"), Encoding.UTF8.GetBytes("three")], rangeBytes);

        rangeBytes = redis.List.LRangeBytes(key, 10, 60);
        Assert.Null(rangeBytes);

        rangeBytes = redis.List.LRangeBytes("nonononno", 0, -1);
        Assert.Null(rangeBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LRangeAsync(Redis redis)
    {
        const string key = "list_lrange_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.List.RPushAsync(key, ["one", "two", "three"]);
        Assert.Equal(3, result);

        var range = await redis.List.LRangeAsync(key, 0, 0);
        Assert.NotNull(range);
        Assert.Single(range);
        Assert.Equal("one", range[0]);

        range = await redis.List.LRangeAsync(key, -3, 2);
        Assert.NotNull(range);
        Assert.Equal(3, range.Length);
        Assert.Equal<string[]>(["one", "two", "three"], range);

        range = await redis.List.LRangeAsync(key, -100, 100);
        Assert.NotNull(range);
        Assert.Equal(3, range.Length);
        Assert.Equal<string[]>(["one", "two", "three"], range);

        range = await redis.List.LRangeAsync(key, 5, 100);
        Assert.Null(range);

        var rangeBytes = await redis.List.LRangeBytesAsync(key, 1, 6);
        Assert.NotNull(rangeBytes);
        Assert.Equal(2, rangeBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("two"), Encoding.UTF8.GetBytes("three")], rangeBytes);

        rangeBytes = await redis.List.LRangeBytesAsync(key, 10, 60);
        Assert.Null(rangeBytes);

        rangeBytes = await redis.List.LRangeBytesAsync("nonononno", 0, -1);
        Assert.Null(rangeBytes);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LRem(Redis redis)
    {
        const string key = "list_lrem_key";
        _ = redis.Key.Del(key);
        var result = redis.List.RPush(key, ["hello", "hello", "foo", "hello"]);
        Assert.Equal(4, result);

        var rem = redis.List.LRem(key, -2, "hello");
        Assert.Equal(2, rem);

        rem = redis.List.LRem(key, -2, "hello111");
        Assert.Equal(0, rem);

        var range = redis.List.LRange(key, 0, -1);
        Assert.Equal<string[]>(["hello", "foo"], range);

        rem = redis.List.LRem(key, 2, Encoding.UTF8.GetBytes("foo"));
        Assert.Equal(1, rem);

        range = redis.List.LRange(key, 0, -1);
        Assert.Equal<string[]>(["hello"], range);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LRemAsync(Redis redis)
    {
        const string key = "list_lrem_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.List.RPushAsync(key, ["hello", "hello", "foo", "hello"]);
        Assert.Equal(4, result);

        var rem = await redis.List.LRemAsync(key, -2, "hello");
        Assert.Equal(2, rem);

        rem = await redis.List.LRemAsync(key, -2, "hello111");
        Assert.Equal(0, rem);

        var range = await redis.List.LRangeAsync(key, 0, -1);
        Assert.Equal<string[]>(["hello", "foo"], range);

        rem = await redis.List.LRemAsync(key, 2, Encoding.UTF8.GetBytes("foo"));
        Assert.Equal(1, rem);

        range = await redis.List.LRangeAsync(key, 0, -1);
        Assert.Equal<string[]>(["hello"], range);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LSet(Redis redis)
    {
        const string key = "list_lset_key";
        _ = redis.Key.Del(key);
        var result = redis.List.RPush(key, ["one", "two", "three"]);
        Assert.Equal(3, result);

        var set = redis.List.LSet(key, 0, "one_  ");
        Assert.True(set);

        var element = redis.List.LIndex(key, 0);
        Assert.Equal("one_  ", element);

        set = redis.List.LSet(key, 1, Encoding.UTF8.GetBytes("Redis  aa !!"));
        Assert.True(set);

        element = redis.List.LIndex(key, 1);
        Assert.Equal("Redis  aa !!", element);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LSetAsync(Redis redis)
    {
        const string key = "list_lset_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.List.RPushAsync(key, ["one", "two", "three"]);
        Assert.Equal(3, result);

        var set = await redis.List.LSetAsync(key, 0, "one_  ");
        Assert.True(set);

        var element = await redis.List.LIndexAsync(key, 0);
        Assert.Equal("one_  ", element);

        set = await redis.List.LSetAsync(key, 1, Encoding.UTF8.GetBytes("Redis  aa !!"));
        Assert.True(set);

        element = await redis.List.LIndexAsync(key, 1);
        Assert.Equal("Redis  aa !!", element);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void LTrim(Redis redis)
    {
        const string key = "list_ltrim_key";
        _ = redis.Key.Del(key);
        var result = redis.List.RPush(key, ["a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(7, result);

        var trim = redis.List.LTrim(key, 0, 2);
        Assert.True(trim);

        var range = redis.List.LRange(key, 0, -1);
        Assert.Equal<string[]>(["a", "b", "c"], range);

        trim = redis.List.LTrim(key, 0, 1000);
        Assert.True(trim);

        range = redis.List.LRange(key, 0, -1);
        Assert.Equal<string[]>(["a", "b", "c"], range);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task LTrimAsync(Redis redis)
    {
        const string key = "list_ltrim_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.List.RPushAsync(key, ["a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(7, result);

        var trim = await redis.List.LTrimAsync(key, 0, -2);
        Assert.True(trim);

        var len = await redis.List.LLenAsync(key);
        Assert.Equal(6, len);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void RPop(Redis redis)
    {
        const string key = "list_rpop_key";
        _ = redis.Key.Del(key);
        var result = redis.List.RPush(key, ["00000", "11", "222", "a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(10, result);

        var pop = redis.List.RPop(key);
        Assert.NotNull(pop);
        Assert.Equal("hello list", pop);

        var popArray = redis.List.RPop(key, 2);
        Assert.NotNull(popArray);
        Assert.Equal(2, popArray.Length);
        Assert.Equal<string[]>(["f", "e"], popArray);

        popArray = redis.List.RPop(key, 1);
        Assert.Equal<string[]>(["d"], popArray);

        var popBytes = redis.List.RPopBytes(key);
        Assert.NotNull(popBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("c"), popBytes);

        var popArrayBytes = redis.List.RPopBytes(key, 3);
        Assert.NotNull(popArrayBytes);
        Assert.Equal(3, popArrayBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("222")], popArrayBytes);

        popArrayBytes = redis.List.RPopBytes(key, 1);
        Assert.Equal([Encoding.UTF8.GetBytes("11")], popArrayBytes);

        popBytes = redis.List.RPopBytes(key);
        Assert.NotNull(popBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("00000"), popBytes);

        pop = redis.List.RPop(key);
        Assert.Null(pop);

        popBytes = redis.List.RPopBytes(key);
        Assert.Null(popBytes);

        popArrayBytes = redis.List.RPopBytes(key, 3);
        Assert.Null(popArrayBytes);

        popArray = redis.List.RPop(key, 2);
        Assert.Null(popArray);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task RPopAsync(Redis redis)
    {
        const string key = "list_rpop_key_async";
        _ = await redis.Key.DelAsync(key);
        var result = await redis.List.RPushAsync(key, ["00000", "11", "222", "a", "b", "c", "d", "e", "f", "hello list"]);
        Assert.Equal(10, result);

        var pop = await redis.List.RPopAsync(key);
        Assert.NotNull(pop);
        Assert.Equal("hello list", pop);

        var popArray = await redis.List.RPopAsync(key, 2);
        Assert.NotNull(popArray);
        Assert.Equal(2, popArray.Length);
        Assert.Equal<string[]>(["f", "e"], popArray);

        popArray = await redis.List.RPopAsync(key, 1);
        Assert.Equal<string[]>(["d"], popArray);

        var popBytes = await redis.List.RPopBytesAsync(key);
        Assert.NotNull(popBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("c"), popBytes);

        var popArrayBytes = await redis.List.RPopBytesAsync(key, 3);
        Assert.NotNull(popArrayBytes);
        Assert.Equal(3, popArrayBytes.Length);
        Assert.Equal([Encoding.UTF8.GetBytes("b"), Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("222")], popArrayBytes);

        popArrayBytes = await redis.List.RPopBytesAsync(key, 1);
        Assert.Equal([Encoding.UTF8.GetBytes("11")], popArrayBytes);

        popBytes = await redis.List.RPopBytesAsync(key);
        Assert.NotNull(popBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("00000"), popBytes);

        pop = await redis.List.RPopAsync(key);
        Assert.Null(pop);

        popBytes = await redis.List.RPopBytesAsync(key);
        Assert.Null(popBytes);

        popArrayBytes = await redis.List.RPopBytesAsync(key, 3);
        Assert.Null(popArrayBytes);

        popArray = await redis.List.RPopAsync(key, 2);
        Assert.Null(popArray);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void RPopLPush(Redis redis)
    {
        const string key = "list_rpoplpush_key";
        const string destination = "list_rpoplpush_destination";
        _ = redis.Key.Del([key, destination]);
        _ = redis.List.RPush(key, ["00000", "11", "222", "a", "b", "c"]);

        var destLen = redis.List.LLen(destination);
        Assert.Equal(0, destLen);

        var element = redis.List.RPopLPush(key, destination);
        Assert.Equal("c", element);

        var keyLen = redis.List.LLen(key);
        Assert.Equal(5, keyLen);

        destLen = redis.List.LLen(destination);
        Assert.Equal(1, destLen);

        element = redis.List.RPopLPush("nononononon", destination);
        Assert.Null(element);

        destLen = redis.List.LLen(destination);
        Assert.Equal(1, destLen);

        var elementBytes = redis.List.RPopLPushBytes(key, destination);
        Assert.Equal(Encoding.UTF8.GetBytes("b"), elementBytes);

        keyLen = redis.List.LLen(key);
        Assert.Equal(4, keyLen);

        destLen = redis.List.LLen(destination);
        Assert.Equal(2, destLen);

        elementBytes = redis.List.RPopLPushBytes("nononojnnnnnn", destination);
        Assert.Null(elementBytes);
        destLen = redis.List.LLen(destination);
        Assert.Equal(2, destLen);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task RPopLPushAsync(Redis redis)
    {
        const string key = "list_rpoplpush_key_async";
        const string destination = "list_rpoplpush_destination_async";
        _ = redis.Key.Del([key, destination]);
        _ = redis.List.RPush(key, ["00000", "11", "222", "a", "b", "c"]);

        var destLen = await redis.List.LLenAsync(destination);
        Assert.Equal(0, destLen);

        var element = await redis.List.RPopLPushAsync(key, destination);
        Assert.Equal("c", element);

        var keyLen = await redis.List.LLenAsync(key);
        Assert.Equal(5, keyLen);

        destLen = await redis.List.LLenAsync(destination);
        Assert.Equal(1, destLen);

        element = await redis.List.RPopLPushAsync("nononononon", destination);
        Assert.Null(element);

        destLen = await redis.List.LLenAsync(destination);
        Assert.Equal(1, destLen);

        var elementBytes = await redis.List.RPopLPushBytesAsync(key, destination);
        Assert.Equal(Encoding.UTF8.GetBytes("b"), elementBytes);

        keyLen = await redis.List.LLenAsync(key);
        Assert.Equal(4, keyLen);

        destLen = await redis.List.LLenAsync(destination);
        Assert.Equal(2, destLen);

        elementBytes = await redis.List.RPopLPushBytesAsync("nononojnnnnnn", destination);
        Assert.Null(elementBytes);
        destLen = await redis.List.LLenAsync(destination);
        Assert.Equal(2, destLen);
    }
}
