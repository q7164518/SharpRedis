using SharpRedis;

namespace NET10.Set;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class SetSync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public void SAdd()
    {
        const string key = "{112}:Set:SAdd:Key";

        var count = this._redis.Set.SAdd(key, "a");
        Assert.That(count, Is.EqualTo(1));

        count = this._redis.Set.SAdd(key, ["b", "c"]);
        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public void SCard()
    {
        const string key1 = "{113}:Set:SCard:Key1";
        const string key2 = "{113}:Set:SCard:Key2";

        var total = this._redis.Set.SCard(key1);
        Assert.That(total, Is.Zero);

        var count = this._redis.Set.SAdd(key1, "a");
        Assert.That(count, Is.EqualTo(1));

        count = this._redis.Set.SAdd(key1, ["b", "c"]);
        Assert.That(count, Is.EqualTo(2));

        total = this._redis.Set.SCard(key1);
        Assert.That(total, Is.EqualTo(3));

        total = this._redis.Set.SCard(key2);
        Assert.That(total, Is.Zero);
    }

    [Test]
    public void SDiff()
    {
        const string key1 = "{114}:Set:SDiff:Key1";
        const string key2 = "{114}:Set:SDiff:Key2";
        const string key3 = "{114}:Set:SDiff:Key3";

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.Set.SAdd(key2, "c");
        Assert.That(count, Is.EqualTo(1));

        count = this._redis.Set.SAdd(key3, ["a", "c", "e"]);
        Assert.That(count, Is.EqualTo(3));

        var result = this._redis.Set.SDiff(key1, key2);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("a"));
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("d"));
        }

        result = this._redis.Set.SDiff([key1, key2, key3]);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("d"));
        }

        result = this._redis.Set.SDiff("Set:SDiff:None1", "Set:SDiff:None2");
        Assert.That(result, Is.Null);

        result = this._redis.Set.SDiff(key1, "Set:SDiff:None2");
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
    public void SDiffStore()
    {
        const string key1 = "{115}:Set:SDiffStore:Key1";
        const string key2 = "{115}:Set:SDiffStore:Key2";
        const string key3 = "{115}:Set:SDiffStore:Key3";
        const string destination = "{115}:Set:SDiffStore:Destination";

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.Set.SAdd(key2, "c");
        Assert.That(count, Is.EqualTo(1));

        count = this._redis.Set.SAdd(key3, ["a", "c", "e"]);
        Assert.That(count, Is.EqualTo(3));

        var result = this._redis.Set.SDiffStore(destination, key1, key2);
        Assert.That(result, Is.EqualTo(3));

        var array = this._redis.Set.SMembers(destination);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("a"));
            Assert.That(array.Contains("b"));
            Assert.That(array.Contains("d"));
        }

        result = this._redis.Set.SDiffStore(destination, [key1, key2, key3]);
        Assert.That(result, Is.EqualTo(2));

        array = this._redis.Set.SMembers(destination);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("b"));
            Assert.That(array.Contains("d"));
        }

        result = this._redis.Set.SDiffStore(destination, key1, "Set:SDiffStore:None2");
        Assert.That(result, Is.EqualTo(4));

        array = this._redis.Set.SMembers(destination);
        Assert.That(array, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("a"));
            Assert.That(array.Contains("b"));
            Assert.That(array.Contains("c"));
            Assert.That(array.Contains("d"));
        }
    }

    [Test]
    public void SInter()
    {
        const string key1 = "{116}:Set:SInter:Key1";
        const string key2 = "{116}:Set:SInter:Key2";
        const string key3 = "{116}:Set:SInter:Key3";

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.Set.SAdd(key2, "c");
        Assert.That(count, Is.EqualTo(1));

        count = this._redis.Set.SAdd(key3, ["a", "c", "e"]);
        Assert.That(count, Is.EqualTo(3));

        var result = this._redis.Set.SInter(key1, key2);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("c"));
        }

        result = this._redis.Set.SInter([key1, key2, key3]);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("c"));
        }

        result = this._redis.Set.SInter("Set:SInter:None1", "Set:SInter:None2");
        Assert.That(result, Is.Null);

        result = this._redis.Set.SInter(key1, "Set:SInter:None2");
        Assert.That(result, Is.Null);

        result = this._redis.Set.SInter([key1]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("a"));
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("c"));
            Assert.That(result.Contains("d"));
        }
    }

    [Test]
    public void SInterCard()
    {
        const string key1 = "{117}:Set:SInterCard:Key1";
        const string key2 = "{117}:Set:SInterCard:Key2";
        const string key3 = "{117}:Set:SInterCard:Key3";

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.Set.SAdd(key2, ["b", "c", "d", "e", "f"]);
        Assert.That(count, Is.EqualTo(5));

        var result = this._redis.Set.SInterCard(key1, key2);
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Set.SInterCard(key1, key2, 2);
        Assert.That(result, Is.EqualTo(2));

        count = this._redis.Set.SAdd(key3, ["a", "b", "c", "d", "e", "f"]);
        Assert.That(count, Is.EqualTo(6));

        result = this._redis.Set.SInterCard([key1, key2, key3]);
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Set.SInterCard([key1, key2, key3], 100);
        Assert.That(result, Is.EqualTo(3));
    }

    [Test]
    public void SInterStore()
    {
        const string key1 = "{118}:Set:SInterStore:Key1";
        const string key2 = "{118}:Set:SInterStore:Key2";
        const string key3 = "{118}:Set:SInterStore:Key3";
        const string destinationKey = "{118}:Set:SInterStore:Destination";

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.Set.SAdd(key2, ["b", "c", "d", "e", "f"]);
        Assert.That(count, Is.EqualTo(5));

        var result = this._redis.Set.SInterStore(destinationKey, key1, key2);
        Assert.That(result, Is.EqualTo(3));

        var array = this._redis.Set.SMembers(destinationKey);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("b"));
            Assert.That(array.Contains("c"));
            Assert.That(array.Contains("d"));
        }

        count = this._redis.Set.SAdd(key3, ["a", "b", "c", "d", "e", "f"]);
        Assert.That(count, Is.EqualTo(6));

        result = this._redis.Set.SInterStore(destinationKey, [key1, key2, key3]);
        Assert.That(result, Is.EqualTo(3));

        array = this._redis.Set.SMembers(destinationKey);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("b"));
            Assert.That(array.Contains("c"));
            Assert.That(array.Contains("d"));
        }
    }

    [Test]
    public void SIsMember()
    {
        const string key1 = "{119}:Set:SIsMember:Key1";

        var any = this._redis.Set.SIsMember(key1, "a");
        Assert.That(any, Is.False);

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        any = this._redis.Set.SIsMember(key1, "a");
        Assert.That(any, Is.True);

        any = this._redis.Set.SIsMember(key1, "z");
        Assert.That(any, Is.False);
    }

    [Test]
    public void SMIsMember()
    {
        const string key1 = "{120}:Set:SMIsMember:Key1";
        var any = this._redis.Set.SMIsMember(key1, ["a"]);
        Assert.That(any, Is.Not.Null);
        Assert.That(any, Has.Length.EqualTo(1));
        Assert.That(any[0], Is.False);

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        any = this._redis.Set.SMIsMember(key1, ["a", "b", "c", "d", "e", "f"]);
        Assert.That(any, Is.Not.Null);
        Assert.That(any, Has.Length.EqualTo(6));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(any[0], Is.True);
            Assert.That(any[1], Is.True);
            Assert.That(any[2], Is.True);
            Assert.That(any[3], Is.True);
            Assert.That(any[4], Is.False);
            Assert.That(any[5], Is.False);
        }
    }

    [Test]
    public void SMove()
    {
        const string key1 = "{121}:Set:SMove:Key1";
        const string key2 = "{121}:Set:SMove:Key2";

        var result = this._redis.Set.SMove(key1, key2, "a");
        Assert.That(result, Is.False);

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        result = this._redis.Set.SMove(key1, key2, "a");
        Assert.That(result, Is.True);

        var array = this._redis.Set.SMembers(key2);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("a"));
        }

        array = this._redis.Set.SMembers(key1);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("b"));
            Assert.That(array.Contains("c"));
            Assert.That(array.Contains("d"));
        }

        result = this._redis.Set.SMove(key1, key2, "a");
        Assert.That(result, Is.False);

        var length = this._redis.Set.SCard(key1);
        Assert.That(length, Is.EqualTo(3));

        length = this._redis.Set.SCard(key2);
        Assert.That(length, Is.EqualTo(1));

        result = this._redis.Set.SMove(key1, key2, "b");
        Assert.That(result, Is.True);

        length = this._redis.Set.SCard(key1);
        Assert.That(length, Is.EqualTo(2));

        length = this._redis.Set.SCard(key2);
        Assert.That(length, Is.EqualTo(2));

        array = this._redis.Set.SMembers(key2);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("a"));
            Assert.That(array.Contains("b"));
        }

        array = this._redis.Set.SMembers(key1);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("c"));
            Assert.That(array.Contains("d"));
        }
    }

    [Test]
    public void SPop()
    {
        const string key1 = "{122}:Set:SPop:Key1";

        var single = this._redis.Set.SPop(key1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(single.IsNull, Is.True);
            Assert.That((string?)single, Is.Null);
        }

        var array = this._redis.Set.SPop(key1, 3);
        Assert.That(array, Is.Null);

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        single = this._redis.Set.SPop(key1);
        Assert.That(single.IsNull, Is.False);

        var length = this._redis.Set.SCard(key1);
        Assert.That(length, Is.EqualTo(3));

        array = this._redis.Set.SPop(key1, 30);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(3));

        length = this._redis.Set.SCard(key1);
        Assert.That(length, Is.Zero);
    }

    [Test]
    public void SRandMember()
    {
        const string key1 = "{123}:Set:SRandMember:Key1";

        var single = this._redis.Set.SRandMember(key1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(single.IsNull, Is.True);
            Assert.That((string?)single, Is.Null);
        }

        var array = this._redis.Set.SRandMember(key1, 3);
        Assert.That(array, Is.Null);

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d", "e", "f", "g"]);
        Assert.That(count, Is.EqualTo(7));

        single = this._redis.Set.SRandMember(key1);
        Assert.That(single.IsNull, Is.False);

        var length = this._redis.Set.SCard(key1);
        Assert.That(length, Is.EqualTo(7));

        array = this._redis.Set.SRandMember(key1, 3);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(3));

        array = this._redis.Set.SRandMember(key1, 30);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(7));

        array = this._redis.Set.SRandMember(key1, -30);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(30));

        length = this._redis.Set.SCard(key1);
        Assert.That(length, Is.EqualTo(7));

        array = this._redis.Set.SRandMember(key1, 1);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(1));
    }

    [Test]
    public void SRem()
    {
        const string key1 = "{124}:Set:SRem:Key1";

        var result = this._redis.Set.SRem(key1, "a");
        Assert.That(result, Is.Zero);

        result = this._redis.Set.SRem(key1, ["a", "b"]);
        Assert.That(result, Is.Zero);

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d", "e", "f", "g"]);
        Assert.That(count, Is.EqualTo(7));

        result = this._redis.Set.SRem(key1, "a");
        Assert.That(result, Is.EqualTo(1));

        result = this._redis.Set.SRem(key1, ["a", "b"]);
        Assert.That(result, Is.EqualTo(1));

        result = this._redis.Set.SRem(key1, "a");
        Assert.That(result, Is.Zero);

        result = this._redis.Set.SRem(key1, ["a", "b", "c", "d", "e",]);
        Assert.That(result, Is.EqualTo(3));

        var length = this._redis.Set.SCard(key1);
        Assert.That(length, Is.EqualTo(2));

        var any = this._redis.Set.SIsMember(key1, "f");
        Assert.That(any, Is.True);

        any = this._redis.Set.SIsMember(key1, "a");
        Assert.That(any, Is.False);
    }

    [Test]
    public void SUnion()
    {
        const string key1 = "{125}:Set:SUnion:Key1";
        const string key2 = "{125}:Set:SUnion:Key2";
        const string key3 = "{125}:Set:SUnion:Key3";

        var union = this._redis.Set.SUnion(key1, key2);
        Assert.That(union, Is.Null);

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d",]);
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.Set.SAdd(key2, ["c"]);
        Assert.That(count, Is.EqualTo(1));

        count = this._redis.Set.SAdd(key3, ["a", "b", "c", "d", "e",]);
        Assert.That(count, Is.EqualTo(5));

        union = this._redis.Set.SUnion(key1, key2);
        Assert.That(union, Is.Not.Null);
        Assert.That(union, Has.Length.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(union.Contains("a"));
            Assert.That(union.Contains("b"));
            Assert.That(union.Contains("c"));
            Assert.That(union.Contains("d"));
        }

        union = this._redis.Set.SUnion([key1, key2, key3]);
        Assert.That(union, Is.Not.Null);
        Assert.That(union, Has.Length.EqualTo(5));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(union.Contains("a"));
            Assert.That(union.Contains("b"));
            Assert.That(union.Contains("c"));
            Assert.That(union.Contains("d"));
            Assert.That(union.Contains("e"));
        }
    }

    [Test]
    public void SUnionStore()
    {
        const string key1 = "{126}:Set:SUnionStore:Key1";
        const string key2 = "{126}:Set:SUnionStore:Key2";
        const string key3 = "{126}:Set:SUnionStore:Key3";
        const string destinationKey = "{126}:Set:SUnionStore:Destination";

        var result = this._redis.Set.SUnionStore(destinationKey, key1, key2);
        Assert.That(result, Is.Zero);

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d",]);
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.Set.SAdd(key2, ["c"]);
        Assert.That(count, Is.EqualTo(1));

        count = this._redis.Set.SAdd(key3, ["a", "b", "c", "d", "e",]);
        Assert.That(count, Is.EqualTo(5));

        result = this._redis.Set.SUnionStore(destinationKey, key1, key2);
        Assert.That(result, Is.EqualTo(4));

        var destLent = this._redis.Set.SCard(destinationKey);
        Assert.That(destLent, Is.EqualTo(4));

        result = this._redis.Set.SUnionStore(destinationKey, [key1, key2, key3]);
        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public void SScan()
    {
        const string key1 = "{127}:Set:SScan:Key1";

        var scan = this._redis.Set.SScan(key1);
        Assert.That(scan, Is.Empty);

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "a"]);
        Assert.That(count, Is.EqualTo(26));

        scan = this._redis.Set.SScan(key1);
        Assert.That(scan, Has.Length.GreaterThan(10));
    }
}
