using SharpRedis;

namespace NET10.Set;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class SetAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public async Task SAddAsync()
    {
        const string key = "{128}:Set:SAddAsync:Key";

        var count = await this._redis.Set.SAddAsync(key, "a");
        Assert.That(count, Is.EqualTo(1));

        count = await this._redis.Set.SAddAsync(key, ["b", "c"]);
        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public async Task SCardAsync()
    {
        const string key1 = "{129}:Set:SCardAsync:Key1";
        const string key2 = "{129}:Set:SCardAsync:Key2";

        var total = await this._redis.Set.SCardAsync(key1);
        Assert.That(total, Is.Zero);

        var count = await this._redis.Set.SAddAsync(key1, "a");
        Assert.That(count, Is.EqualTo(1));

        count = await this._redis.Set.SAddAsync(key1, ["b", "c"]);
        Assert.That(count, Is.EqualTo(2));

        total = await this._redis.Set.SCardAsync(key1);
        Assert.That(total, Is.EqualTo(3));

        total = await this._redis.Set.SCardAsync(key2);
        Assert.That(total, Is.Zero);
    }

    [Test]
    public async Task SDiffAsync()
    {
        const string key1 = "{130}:Set:SDiffAsync:Key1";
        const string key2 = "{130}:Set:SDiffAsync:Key2";
        const string key3 = "{130}:Set:SDiffAsync:Key3";

        var count = await this._redis.Set.SAddAsync(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        count = await this._redis.Set.SAddAsync(key2, "c");
        Assert.That(count, Is.EqualTo(1));

        count = await this._redis.Set.SAddAsync(key3, ["a", "c", "e"]);
        Assert.That(count, Is.EqualTo(3));

        var result = await this._redis.Set.SDiffAsync(key1, key2);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("a"));
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("d"));
        }

        result = await this._redis.Set.SDiffAsync([key1, key2, key3]);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("d"));
        }

        result = await this._redis.Set.SDiffAsync("Set:SDiffAsync:None1", "Set:SDiffAsync:None2");
        Assert.That(result, Is.Null);

        result = await this._redis.Set.SDiffAsync(key1, "Set:SDiffAsync:None2");
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
    public async Task SDiffStoreAsync()
    {
        const string key1 = "{131}:Set:SDiffStoreAsync:Key1";
        const string key2 = "{131}:Set:SDiffStoreAsync:Key2";
        const string key3 = "{131}:Set:SDiffStoreAsync:Key3";
        const string destination = "{131}:Set:SDiffStoreAsync:Destination";

        var count = await this._redis.Set.SAddAsync(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.Set.SAdd(key2, "c");
        Assert.That(count, Is.EqualTo(1));

        count = await this._redis.Set.SAddAsync(key3, ["a", "c", "e"]);
        Assert.That(count, Is.EqualTo(3));

        var result = await this._redis.Set.SDiffStoreAsync(destination, key1, key2);
        Assert.That(result, Is.EqualTo(3));

        var array = await this._redis.Set.SMembersAsync(destination);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("a"));
            Assert.That(array.Contains("b"));
            Assert.That(array.Contains("d"));
        }

        result = await this._redis.Set.SDiffStoreAsync(destination, [key1, key2, key3]);
        Assert.That(result, Is.EqualTo(2));

        array = await this._redis.Set.SMembersAsync(destination);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("b"));
            Assert.That(array.Contains("d"));
        }

        result = await this._redis.Set.SDiffStoreAsync(destination, key1, "Set:SDiffStoreAsync:None2");
        Assert.That(result, Is.EqualTo(4));

        array = await this._redis.Set.SMembersAsync(destination);
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
    public async Task SInterAsync()
    {
        const string key1 = "{132}:Set:SInterAsync:Key1";
        const string key2 = "{132}:Set:SInterAsync:Key2";
        const string key3 = "{132}:Set:SInterAsync:Key3";

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        count = await this._redis.Set.SAddAsync(key2, "c");
        Assert.That(count, Is.EqualTo(1));

        count = this._redis.Set.SAdd(key3, ["a", "c", "e"]);
        Assert.That(count, Is.EqualTo(3));

        var result = await this._redis.Set.SInterAsync(key1, key2);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("c"));
        }

        result = await this._redis.Set.SInterAsync([key1, key2, key3]);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("c"));
        }

        result = await this._redis.Set.SInterAsync("Set:SInterAsync:None1", "Set:SInterAsync:None2");
        Assert.That(result, Is.Null);

        result = await this._redis.Set.SInterAsync(key1, "Set:SInterAsync:None2");
        Assert.That(result, Is.Null);

        result = await this._redis.Set.SInterAsync([key1]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Contains("a"));
            Assert.That(result.Contains("b"));
            Assert.That(result.Contains("c"));
            Assert.That(result.Contains("d"));
        }
    }

    [Test]
    public async Task SInterCardAsync()
    {
        const string key1 = "{133}:Set:SInterCardAsync:Key1";
        const string key2 = "{133}:Set:SInterCardAsync:Key2";
        const string key3 = "{133}:Set:SInterCardAsync:Key3";

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
    public async Task SInterStoreAsync()
    {
        const string key1 = "{134}:Set:SInterStoreAsync:Key1";
        const string key2 = "{134}:Set:SInterStoreAsync:Key2";
        const string key3 = "{134}:Set:SInterStoreAsync:Key3";
        const string destinationKey = "{134}:Set:SInterStoreAsync:Destination";

        var count = await this._redis.Set.SAddAsync(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        count = await this._redis.Set.SAddAsync(key2, ["b", "c", "d", "e", "f"]);
        Assert.That(count, Is.EqualTo(5));

        var result = await this._redis.Set.SInterStoreAsync(destinationKey, key1, key2);
        Assert.That(result, Is.EqualTo(3));

        var array = await this._redis.Set.SMembersAsync(destinationKey);
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

        result = await this._redis.Set.SInterStoreAsync(destinationKey, [key1, key2, key3]);
        Assert.That(result, Is.EqualTo(3));

        array = await this._redis.Set.SMembersAsync(destinationKey);
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
    public async Task SIsMemberAsync()
    {
        const string key1 = "{135}:Set:SIsMemberAsync:Key1";

        var any = await this._redis.Set.SIsMemberAsync(key1, "a");
        Assert.That(any, Is.False);

        var count = await this._redis.Set.SAddAsync(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        any = await this._redis.Set.SIsMemberAsync(key1, "a");
        Assert.That(any, Is.True);

        any = await this._redis.Set.SIsMemberAsync(key1, "z");
        Assert.That(any, Is.False);
    }

    [Test]
    public async Task SMIsMemberAsync()
    {
        const string key1 = "{136}:Set:SMIsMemberAsync:Key1";
        var any = await this._redis.Set.SMIsMemberAsync(key1, ["a"]);
        Assert.That(any, Is.Not.Null);
        Assert.That(any, Has.Length.EqualTo(1));
        Assert.That(any[0], Is.False);

        var count = await this._redis.Set.SAddAsync(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        any = await this._redis.Set.SMIsMemberAsync(key1, ["a", "b", "c", "d", "e", "f"]);
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
    public async Task SMoveAsync()
    {
        const string key1 = "{137}:Set:SMoveAsync:Key1";
        const string key2 = "{137}:Set:SMoveAsync:Key2";

        var result = await this._redis.Set.SMoveAsync(key1, key2, "a");
        Assert.That(result, Is.False);

        var count = this._redis.Set.SAdd(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        result = await this._redis.Set.SMoveAsync(key1, key2, "a");
        Assert.That(result, Is.True);

        var array = await this._redis.Set.SMembersAsync(key2);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("a"));
        }

        array = await this._redis.Set.SMembersAsync(key1);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("b"));
            Assert.That(array.Contains("c"));
            Assert.That(array.Contains("d"));
        }

        result = await this._redis.Set.SMoveAsync(key1, key2, "a");
        Assert.That(result, Is.False);

        var length = await this._redis.Set.SCardAsync(key1);
        Assert.That(length, Is.EqualTo(3));

        length = await this._redis.Set.SCardAsync(key2);
        Assert.That(length, Is.EqualTo(1));

        result = await this._redis.Set.SMoveAsync(key1, key2, "b");
        Assert.That(result, Is.True);

        length = this._redis.Set.SCard(key1);
        Assert.That(length, Is.EqualTo(2));

        length = await this._redis.Set.SCardAsync(key2);
        Assert.That(length, Is.EqualTo(2));

        array = await this._redis.Set.SMembersAsync(key2);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("a"));
            Assert.That(array.Contains("b"));
        }

        array = await this._redis.Set.SMembersAsync(key1);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(array.Contains("c"));
            Assert.That(array.Contains("d"));
        }
    }

    [Test]
    public async Task SPopAsync()
    {
        const string key1 = "{138}:Set:SPopAsync:Key1";

        var single = await this._redis.Set.SPopAsync(key1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(single.IsNull, Is.True);
            Assert.That((string?)single, Is.Null);
        }

        var array = await this._redis.Set.SPopAsync(key1, 3);
        Assert.That(array, Is.Null);

        var count = await this._redis.Set.SAddAsync(key1, ["a", "b", "c", "d"]);
        Assert.That(count, Is.EqualTo(4));

        single = await this._redis.Set.SPopAsync(key1);
        Assert.That(single.IsNull, Is.False);

        var length = this._redis.Set.SCard(key1);
        Assert.That(length, Is.EqualTo(3));

        array = await this._redis.Set.SPopAsync(key1, 30);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(3));

        length = await this._redis.Set.SCardAsync(key1);
        Assert.That(length, Is.Zero);
    }

    [Test]
    public async Task SRandMemberAsync()
    {
        const string key1 = "{139}:Set:SRandMemberAsync:Key1";

        var single = await this._redis.Set.SRandMemberAsync(key1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(single.IsNull, Is.True);
            Assert.That((string?)single, Is.Null);
        }

        var array = await this._redis.Set.SRandMemberAsync(key1, 3);
        Assert.That(array, Is.Null);

        var count = await this._redis.Set.SAddAsync(key1, ["a", "b", "c", "d", "e", "f", "g"]);
        Assert.That(count, Is.EqualTo(7));

        single = await this._redis.Set.SRandMemberAsync(key1);
        Assert.That(single.IsNull, Is.False);

        var length = await this._redis.Set.SCardAsync(key1);
        Assert.That(length, Is.EqualTo(7));

        array = await this._redis.Set.SRandMemberAsync(key1, 3);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(3));

        array = await this._redis.Set.SRandMemberAsync(key1, 30);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(7));

        array = await this._redis.Set.SRandMemberAsync(key1, -30);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(30));

        length = await this._redis.Set.SCardAsync(key1);
        Assert.That(length, Is.EqualTo(7));

        array = await this._redis.Set.SRandMemberAsync(key1, 1);
        Assert.That(array, Is.Not.Null);
        Assert.That(array, Has.Length.EqualTo(1));
    }

    [Test]
    public async Task SRemAsync()
    {
        const string key1 = "{140}:Set:SRemAsync:Key1";

        var result = await this._redis.Set.SRemAsync(key1, "a");
        Assert.That(result, Is.Zero);

        result = await this._redis.Set.SRemAsync(key1, ["a", "b"]);
        Assert.That(result, Is.Zero);

        var count = await this._redis.Set.SAddAsync(key1, ["a", "b", "c", "d", "e", "f", "g"]);
        Assert.That(count, Is.EqualTo(7));

        result = await this._redis.Set.SRemAsync(key1, "a");
        Assert.That(result, Is.EqualTo(1));

        result = await this._redis.Set.SRemAsync(key1, ["a", "b"]);
        Assert.That(result, Is.EqualTo(1));

        result = await this._redis.Set.SRemAsync(key1, "a");
        Assert.That(result, Is.Zero);

        result = await this._redis.Set.SRemAsync(key1, ["a", "b", "c", "d", "e",]);
        Assert.That(result, Is.EqualTo(3));

        var length = await this._redis.Set.SCardAsync(key1);
        Assert.That(length, Is.EqualTo(2));

        var any = await this._redis.Set.SIsMemberAsync(key1, "f");
        Assert.That(any, Is.True);

        any = await this._redis.Set.SIsMemberAsync(key1, "a");
        Assert.That(any, Is.False);
    }

    [Test]
    public async Task SUnionAsync()
    {
        const string key1 = "{141}:Set:SUnionAsync:Key1";
        const string key2 = "{141}:Set:SUnionAsync:Key2";
        const string key3 = "{141}:Set:SUnionAsync:Key3";

        var union = await this._redis.Set.SUnionAsync(key1, key2);
        Assert.That(union, Is.Null);

        var count = await this._redis.Set.SAddAsync(key1, ["a", "b", "c", "d",]);
        Assert.That(count, Is.EqualTo(4));

        count = this._redis.Set.SAdd(key2, ["c"]);
        Assert.That(count, Is.EqualTo(1));

        count = await this._redis.Set.SAddAsync(key3, ["a", "b", "c", "d", "e",]);
        Assert.That(count, Is.EqualTo(5));

        union = await this._redis.Set.SUnionAsync(key1, key2);
        Assert.That(union, Is.Not.Null);
        Assert.That(union, Has.Length.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(union.Contains("a"));
            Assert.That(union.Contains("b"));
            Assert.That(union.Contains("c"));
            Assert.That(union.Contains("d"));
        }

        union = await this._redis.Set.SUnionAsync([key1, key2, key3]);
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
    public async Task SUnionStoreAsync()
    {
        const string key1 = "{142}:Set:SUnionStoreAsync:Key1";
        const string key2 = "{142}:Set:SUnionStoreAsync:Key2";
        const string key3 = "{142}:Set:SUnionStoreAsync:Key3";
        const string destinationKey = "{142}:Set:SUnionStoreAsync:Destination";

        var result = await this._redis.Set.SUnionStoreAsync(destinationKey, key1, key2);
        Assert.That(result, Is.Zero);

        var count = await this._redis.Set.SAddAsync(key1, ["a", "b", "c", "d",]);
        Assert.That(count, Is.EqualTo(4));

        count = await this._redis.Set.SAddAsync(key2, ["c"]);
        Assert.That(count, Is.EqualTo(1));

        count = await this._redis.Set.SAddAsync(key3, ["a", "b", "c", "d", "e",]);
        Assert.That(count, Is.EqualTo(5));

        result = await this._redis.Set.SUnionStoreAsync(destinationKey, key1, key2);
        Assert.That(result, Is.EqualTo(4));

        var destLent = await this._redis.Set.SCardAsync(destinationKey);
        Assert.That(destLent, Is.EqualTo(4));

        result = await this._redis.Set.SUnionStoreAsync(destinationKey, [key1, key2, key3]);
        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public async Task SScanAsync()
    {
        const string key1 = "{143}:Set:SScanAsync:Key1";

        var scan = await this._redis.Set.SScanAsync(key1);
        Assert.That(scan, Is.Empty);

        var count = await this._redis.Set.SAddAsync(key1, ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "a"]);
        Assert.That(count, Is.EqualTo(26));

        scan = await this._redis.Set.SScanAsync(key1);
        Assert.That(scan, Has.Length.GreaterThan(10));
    }
}
