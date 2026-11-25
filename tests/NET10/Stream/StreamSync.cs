using SharpRedis;
using SharpRedis.Exceptions;
using static SharpRedis.RedisStream;

namespace NET10.Stream;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class StreamSync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region XDel
    private static string XDelKey() => $"Stream:{{XDel}}:{Guid.NewGuid():N}";

    private void XDelSeed(string key)
    {
        this._redis.Stream.XAdd(key, "1-0", "f", "v1");
        this._redis.Stream.XAdd(key, "2-0", "f", "v2");
        this._redis.Stream.XAdd(key, "3-0", "f", "v3");
        this._redis.Stream.XAdd(key, "4-0", "f", "v4");
        this._redis.Stream.XAdd(key, "5-0", "f", "v5");
    }

    private void XDelAssertLength(string key, long expected)
    {
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(expected));
    }

    private static void XDelRedisError(Action action)
    {
        Assert.That(action, Throws.TypeOf<RedisErrorException>());
    }

    [Test]
    public void XDel_AllOverloads()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        Assert.That(this._redis.Stream.XDel(key, "1-0"),
            Is.EqualTo(1));

        ICollection<EntryId> collection = ["2-0", "3-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDel(key, collection),
                    Is.EqualTo(2));

            Assert.That(this._redis.Stream.XDel(key, ["4-0", "5-0"]),
                Is.EqualTo(2));
        }

        this.XDelAssertLength(key, 0);
    }

    [Test]
    public void XDel_SingleId_Success()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        Assert.That(this._redis.Stream.XDel(key, "3-0"),
            Is.EqualTo(1));

        this.XDelAssertLength(key, 4);
    }

    [Test]
    public void XDel_SingleId_NotExists()
    {
        var key = XDelKey();
        this.XDelSeed(key);
        Assert.That(this._redis.Stream.XDel(key, "100-0"), Is.Zero);
        this.XDelAssertLength(key, 5);
    }

    [Test]
    public void XDel_SingleId_DeleteTwice()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        Assert.That(this._redis.Stream.XDel(key, "2-0"),
            Is.EqualTo(1));

        Assert.That(this._redis.Stream.XDel(key, "2-0"),
            Is.Zero);

        this.XDelAssertLength(key, 4);
    }

    [Test]
    public void XDel_Collection_DeleteMultiple()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        ICollection<EntryId> ids = ["1-0", "3-0", "5-0"];

        Assert.That(this._redis.Stream.XDel(key, ids),
            Is.EqualTo(3));

        this.XDelAssertLength(key, 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDel(key, "2-0"),
                    Is.EqualTo(1));

            Assert.That(this._redis.Stream.XDel(key, "4-0"),
                Is.EqualTo(1));
        }

        this.XDelAssertLength(key, 0);
    }

    [Test]
    public void XDel_Collection_MixedExistingAndMissing()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        ICollection<EntryId> ids =
        [
            "1-0",
            "100-0",
            "3-0",
            "200-0"
        ];

        Assert.That(this._redis.Stream.XDel(key, ids),
            Is.EqualTo(2));

        this.XDelAssertLength(key, 3);
    }

    [Test]
    public void XDel_Collection_DuplicateIds()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        ICollection<EntryId> ids =
        [
            "1-0",
            "1-0",
            "2-0",
            "2-0"
        ];

        Assert.That(this._redis.Stream.XDel(key, ids),
            Is.EqualTo(2));

        this.XDelAssertLength(key, 3);
    }

    [Test]
    public void XDel_Collection_Empty()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        ICollection<EntryId> ids = Array.Empty<EntryId>();
        Assert.That(() => this._redis.Stream.XDel(key, ids), Throws.TypeOf<NotSupportedException>());
        ids = null!;
        Assert.That(() => this._redis.Stream.XDel(key, ids), Throws.TypeOf<NotSupportedException>());
        this.XDelAssertLength(key, 5);
    }

    [Test]
    public void XDel_Span_DeleteMultiple()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        Assert.That(this._redis.Stream.XDel(key, ["1-0", "2-0", "3-0"]),
            Is.EqualTo(3));

        this.XDelAssertLength(key, 2);
    }

    [Test]
    public void XDel_Span_MixedExistingAndMissing()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        Assert.That(this._redis.Stream.XDel(key, ["1-0", "100-0", "2-0", "200-0"]),
            Is.EqualTo(2));

        this.XDelAssertLength(key, 3);
    }

    [Test]
    public void XDel_Span_DuplicateIds()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        Assert.That(this._redis.Stream.XDel(key, ["1-0", "1-0", "3-0", "3-0"]),
            Is.EqualTo(2));

        this.XDelAssertLength(key, 3);
    }

    [Test]
    public void XDel_Span_Empty()
    {
        var key = XDelKey();
        this.XDelSeed(key);
        Assert.That(() =>
        {
            ReadOnlySpan<EntryId> span = default;
            this._redis.Stream.XDel(key, span);
        }, Throws.TypeOf<NotSupportedException>());
        Assert.That(() => this._redis.Stream.XDel(key, []), Throws.TypeOf<NotSupportedException>());
        this.XDelAssertLength(key, 5);
    }

    [Test]
    public void XDel_KeyNotExists()
    {
        var key = XDelKey();

        Assert.That(this._redis.Stream.XDel(key, "1-0"),
            Is.Zero);

        ICollection<EntryId> collection = ["1-0", "2-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDel(key, collection),
                    Is.Zero);

            Assert.That(this._redis.Stream.XDel(key, ["1-0", "2-0"]),
                Is.Zero);
        }

        this.XDelAssertLength(key, 0);
    }

    [Test]
    public void XDel_WrongKeyType()
    {
        var key = XDelKey();

        this._redis.String.Set(key, "hello");

        XDelRedisError(() => this._redis.Stream.XDel(
            key, "1-0"));

        ICollection<EntryId> collection = ["1-0"];

        XDelRedisError(() => this._redis.Stream.XDel(
            key, collection));

        XDelRedisError(() => this._redis.Stream.XDel(
            key, ["1-0"]));
    }

    [Test]
    public void XDel_InvalidId_Asterisk()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        XDelRedisError(() => this._redis.Stream.XDel(
            key, "*"));

        this.XDelAssertLength(key, 5);
    }

    [Test]
    public void XDel_InvalidId_PartialId()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        XDelRedisError(() => this._redis.Stream.XDel(
            key, "1-*"));

        this.XDelAssertLength(key, 5);
    }

    [Test]
    public void XDel_InvalidId_Overflow()
    {
        var key = XDelKey();
        this.XDelSeed(key);
        Assert.That(() => this._redis.Stream.XDel(key, "18446744073709551616-0"), Throws.TypeOf<ArgumentException>());

        this.XDelAssertLength(key, 5);
    }

    [Test]
    public void XDel_Id_ZeroZero()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        Assert.That(this._redis.Stream.XDel(
            key, "0-0"), Is.Zero);

        this.XDelAssertLength(key, 5);
    }

    [Test]
    public void XDel_DeletedId_CannotBeReused()
    {
        var key = XDelKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(
                    key, "1-0", "f", "v"), Is.EqualTo("1-0"));

            Assert.That(this._redis.Stream.XAdd(
                key, "2-0", "f", "v"), Is.EqualTo("2-0"));

            Assert.That(this._redis.Stream.XDel(
                key, "2-0"), Is.EqualTo(1));
        }

        this.XDelAssertLength(key, 1);

        XDelRedisError(() => this._redis.Stream.XAdd(
            key, "2-0", "f", "v"));

        XDelRedisError(() => this._redis.Stream.XAdd(
            key, "1-1", "f", "v"));

        Assert.That(this._redis.Stream.XAdd(
            key, "2-1", "f", "v"), Is.EqualTo("2-1"));

        this.XDelAssertLength(key, 2);
    }

    [Test]
    public void XDel_DeleteAll_ThenAddNewEntry()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        ICollection<EntryId> ids =
        [
            "1-0", "2-0", "3-0", "4-0", "5-0"
        ];

        Assert.That(this._redis.Stream.XDel(key, ids),
            Is.EqualTo(5));

        this.XDelAssertLength(key, 0);

        XDelRedisError(() => this._redis.Stream.XAdd(
            key, "5-0", "f", "v"));

        Assert.That(this._redis.Stream.XAdd(
            key, "6-0", "f", "v"), Is.EqualTo("6-0"));

        this.XDelAssertLength(key, 1);
    }

    [Test]
    public void XDel_EntryId_DifferentConstructors()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDel(
                    key, new EntryId("1-0")), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XDel(
                key, new EntryId(2UL, 0UL)), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XDel(
                key, EntryId.Create("3-0")), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XDel(
                key, EntryId.Create(4UL, 0UL)), Is.EqualTo(1));
        }

        EntryId id = (5UL, 0UL);

        Assert.That(this._redis.Stream.XDel(key, id),
            Is.EqualTo(1));

        this.XDelAssertLength(key, 0);
    }

    [Test]
    public void XDel_Collection_ListAndArray()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        List<EntryId> list = ["1-0", "2-0"];
        EntryId[] array = ["3-0", "4-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDel(key, list),
                    Is.EqualTo(2));

            Assert.That(this._redis.Stream.XDel(key, array),
                Is.EqualTo(2));
        }

        this.XDelAssertLength(key, 1);
    }

    [Test]
    public void XDel_Flags_None()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        Assert.That(this._redis.Stream.XDel(
            key,
            "1-0",
            flags: CommandFlags.None), Is.EqualTo(1));

        this.XDelAssertLength(key, 4);
    }

    [Test]
    public void XDel_CancellationToken_Default()
    {
        var key = XDelKey();
        this.XDelSeed(key);

        Assert.That(this._redis.Stream.XDel(
            key,
            "1-0",
            cancellationToken: CancellationToken.None),
            Is.EqualTo(1));

        this.XDelAssertLength(key, 4);
    }

    #endregion

    #region XAck

    private static string XAckKey() =>
        $"Stream:{{XAck}}:{Guid.NewGuid():N}";

    private static void XAckRedisError(Action action)
    {
        Assert.That(action, Throws.TypeOf<RedisErrorException>());
    }

    private void XAckPrepareGroup(string key, string group = "group1")
    {
        this._redis.Stream.XAdd(key, "1-0", "name", "v1");
        this._redis.Stream.XAdd(key, "2-0", "name", "v2");
        this._redis.Stream.XAdd(key, "3-0", "name", "v3");
        this._redis.Stream.XAdd(key, "4-0", "name", "v4");
        this._redis.Stream.XAdd(key, "5-0", "name", "v5");

        this._redis.Stream.XGroupCreate(key, group, "0-0");
    }

    private void XAckPreparePending(string key, string group = "group1")
    {
        this.XAckPrepareGroup(key, group);
        this._redis.Stream.XReadGroup(key, ">", group, "consumer1");
    }

    [Test]
    public void XAck_SingleId_Success()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(key, "group1", (EntryId)"1-0"), Is.EqualTo(1));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XAck_SingleId_Repeat()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);
        Assert.That(this._redis.Stream.XAck(key, "group1", (EntryId)"1-0"), Is.EqualTo(1));
        Assert.That(this._redis.Stream.XAck(key, "group1", (EntryId)"1-0"), Is.Zero);
    }

    [Test]
    public void XAck_SingleId_NotExists()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        Assert.That(this._redis.Stream.XAck(key, "group1", (EntryId)"100-0"), Is.Zero);
    }

    [Test]
    public void XAck_SingleId_NotPending()
    {
        var key = XAckKey();
        this.XAckPrepareGroup(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(key, "group1", (EntryId)"1-0"), Is.Zero);
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XAck_Collection_Multiple()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        ICollection<EntryId> ids = ["1-0", "2-0", "3-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(key, "group1", ids), Is.EqualTo(3));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XAck_Collection_Mixed()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        ICollection<EntryId> ids = ["1-0", "2-0", "100-0"];

        Assert.That(this._redis.Stream.XAck(key, "group1", ids), Is.EqualTo(2));
        Assert.That(this._redis.Stream.XAck(key, "group1", ids), Is.Zero);
    }

    [Test]
    public void XAck_Collection_Duplicate()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        ICollection<EntryId> ids = ["1-0", "1-0", "2-0", "2-0"];

        Assert.That(this._redis.Stream.XAck(key, "group1", ids),
            Is.EqualTo(2));
    }

    [Test]
    public void XAck_Collection_Empty()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        ICollection<EntryId> ids = [];

        Assert.That(() => this._redis.Stream.XAck(key, "group1", ids), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public void XAck_Span_Multiple()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        Assert.That(this._redis.Stream.XAck(
            key, "group1", (ReadOnlySpan<EntryId>)["1-0", "2-0", "3-0"]),
            Is.EqualTo(3));
    }

    [Test]
    public void XAck_Span_Mixed()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        Assert.That(this._redis.Stream.XAck(
            key, "group1", ["1-0", "100-0", "2-0"]),
            Is.EqualTo(2));
    }

    [Test]
    public void XAck_Span_Duplicate()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        Assert.That(this._redis.Stream.XAck(
            key, "group1", ["1-0", "1-0", "2-0"]),
            Is.EqualTo(2));
    }

    [Test]
    public void XAck_Span_Empty()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        Assert.That(() => this._redis.Stream.XAck(key, "group1", []), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public void XAck_WrongKeyType()
    {
        var key = XAckKey();
        this._redis.String.Set(key, "hello");

        XAckRedisError(() => this._redis.Stream.XAck(
            key, "group1", "1-0"));
    }

    [Test]
    public void XAck_InvalidId_Asterisk()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        XAckRedisError(() => this._redis.Stream.XAck(
            key, "group1", (EntryId)"*"));
    }

    [Test]
    public void XAck_InvalidId_Partial()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        XAckRedisError(() => this._redis.Stream.XAck(
            key, "group1", (EntryId)"1-*"));
    }

    [Test]
    public void XAck_Id_ZeroZero()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        Assert.That(this._redis.Stream.XAck(key, "group1", (EntryId)"0-0"), Is.Zero);
    }

    [Test]
    public void XAck_Flags_None()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        Assert.That(this._redis.Stream.XAck(
            key, "group1", (EntryId)"1-0",
            flags: CommandFlags.None), Is.EqualTo(1));
    }

    [Test]
    public void XAck_CancellationToken_Default()
    {
        var key = XAckKey();
        this.XAckPreparePending(key);

        Assert.That(this._redis.Stream.XAck(
            key, "group1", (EntryId)"1-0",
            cancellationToken: CancellationToken.None), Is.EqualTo(1));
    }

    #endregion

    #region XRead / XReadGroup
    private static string XReadKey() => $"Stream:{{XRead}}:{Guid.NewGuid():N}";
    private static string XReadGroupKey() => $"Stream:{{XReadGroup}}:{Guid.NewGuid():N}";
    private static void XReadRedisError(Action action) => Assert.That(action, Throws.TypeOf<RedisErrorException>());
    private static void XReadGroupRedisError(Action action) => Assert.That(action, Throws.TypeOf<RedisErrorException>());

    private void XReadSeed(string key)
    {
        for (int i = 1; i <= 5; i++)
            Assert.That(this._redis.Stream.XAdd(key, $"{i}-0", "name", $"v{i}"), Is.EqualTo($"{i}-0"));
    }
    private void XReadGroupSeed(string key, string group = "group1", string start = "0-0")
    {
        this.XReadSeed(key);
        Assert.That(this._redis.Stream.XGroupCreate(key, group, start), Is.True);
    }
    private void XReadAssert(XReadResult? result, string key, params string[] ids)
    {
        var expectedKey = Factory.GetPrefix(this._redis) + key;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Name.ToString(), Is.EqualTo(expectedKey));
            Assert.That(result.Value.Entries, Has.Length.EqualTo(ids.Length));
        }
        for (int i = 0; i < ids.Length; i++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Value.Entries![i].Id, Is.EqualTo(ids[i]));
                Assert.That(result.Value.Entries[i].FieldValue, Has.Length.EqualTo(1));
            }
        }
    }
    private void XReadAssertMany(XReadResult[]? results, params (string key, string[] ids)[] expected)
    {
        Assert.That(results, Is.Not.Null);
        Assert.That(results, Has.Length.EqualTo(expected.Length));
        foreach (var (key, ids) in expected)
        {
            var expectedKey = Factory.GetPrefix(this._redis) + key;
            var found = Array.Find(results!, x => x.Name.ToString() == expectedKey);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(found.Name.ToString(), Is.EqualTo(expectedKey));
                Assert.That(found.Entries, Has.Length.EqualTo(ids.Length));
            }
            for (int i = 0; i < ids.Length; i++)
                Assert.That(found.Entries![i].Id, Is.EqualTo(ids[i]));
        }
    }

    [Test]
    public void XRead_Single_AllEntries()
    {
        var key = XReadKey(); this.XReadSeed(key);
        XReadAssert(this._redis.Stream.XRead(key, "0-0"), key, "1-0", "2-0", "3-0", "4-0", "5-0");
    }
    [Test]
    public void XRead_Count_And_IdExclusive()
    {
        var key = XReadKey(); this.XReadSeed(key);
        XReadAssert(this._redis.Stream.XRead(key, "2-0", count: 2), key, "3-0", "4-0");
        XReadAssert(this._redis.Stream.XRead(key, "4-0", count: 10), key, "5-0");
        Assert.That(this._redis.Stream.XRead(key, "5-0"), Is.Null);
    }
    [Test]
    public void XRead_NoStream_And_NoNewEntries()
    {
        var key = XReadKey();
        Assert.That(this._redis.Stream.XRead(key, "0-0"), Is.Null);
        this.XReadSeed(key);
        Assert.That(this._redis.Stream.XRead(key, "$"), Is.Null);
    }
    [Test]
    public void XRead_FieldValues_Unicode()
    {
        var key = XReadKey();
        this._redis.Stream.XAdd(key, "1-0", "姓名", "张三");
        var result = this._redis.Stream.XRead(key, "0-0");
        XReadAssert(result, key, "1-0");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.Value.Entries![0].FieldValue![0].Key.ToString(), Is.EqualTo("姓名"));
            Assert.That(result.Value.Entries[0].FieldValue![0].Value.ToString(), Is.EqualTo("张三"));
        }
    }
    [Test]
    public void XRead_Block_Timeout()
    {
        var key = XReadKey();
        Assert.That(this._redis.Stream.XRead(key, "0-0", block: 30), Is.Null);
    }
    [Test]
    public void XRead_Collection_Kvp()
    {
        var a = XReadKey(); var b = XReadKey(); this.XReadSeed(a); this.XReadSeed(b);
        ICollection<KeyValuePair<SharpValue, EntryId>> keys = [new(a, "3-0"), new(b, "4-0")];
        XReadAssertMany(this._redis.Stream.XRead(keys), (a, ["4-0", "5-0"]), (b, ["5-0"]));
    }
    [Test]
    public void XRead_Span_Kvp()
    {
        var a = XReadKey(); var b = XReadKey(); this.XReadSeed(a); this.XReadSeed(b);
        XReadAssertMany(this._redis.Stream.XRead((ReadOnlySpan<KeyValuePair<SharpValue, EntryId>>)[new(a, "4-0"), new(b, "3-0")]), (a, ["5-0"]), (b, ["4-0", "5-0"]));
    }
    [Test]
    public void XRead_Collection_Tuple()
    {
        var a = XReadKey(); var b = XReadKey(); this.XReadSeed(a); this.XReadSeed(b);
        ICollection<(SharpValue key, EntryId id)> keys = [(a, "4-0"), (b, "4-0")];
        XReadAssertMany(this._redis.Stream.XRead(keys), (a, ["5-0"]), (b, ["5-0"]));
    }
    [Test]
    public void XRead_Span_Tuple()
    {
        var a = XReadKey(); var b = XReadKey(); this.XReadSeed(a); this.XReadSeed(b);
        XReadAssertMany(this._redis.Stream.XRead([(a, "3-0"), (b, "4-0")], count: 1), (a, ["4-0"]), (b, ["5-0"]));
    }
    [Test]
    public void XRead_Multiple_SomeStreamsEmpty()
    {
        var a = XReadKey(); var b = XReadKey(); this.XReadSeed(a);
        ICollection<(SharpValue key, EntryId id)> keys = [(a, "4-0"), (b, "0-0")];
        XReadAssertMany(this._redis.Stream.XRead(keys), (a, ["5-0"]));
    }
    [Test]
    public void XRead_Flags_And_CancellationToken()
    {
        var key = XReadKey(); this.XReadSeed(key);
        XReadAssert(this._redis.Stream.XRead(key, "4-0", flags: CommandFlags.None, cancellationToken: CancellationToken.None), key, "5-0");
    }
    [Test]
    public void XRead_WrongType()
    {
        var key = XReadKey(); this._redis.String.Set(key, "not-stream");
        XReadRedisError(() => this._redis.Stream.XRead(key, "0-0"));
    }
    [Test]
    public void XRead_InvalidId()
    {
        var key = XReadKey(); this.XReadSeed(key);
        XReadRedisError(() => this._redis.Stream.XRead(key, "*"));
    }

    [Test]
    public void XReadGroup_Single_NewMessages_PEL_And_Ack()
    {
        var key = XReadGroupKey(); this.XReadGroupSeed(key);
        XReadAssert(this._redis.Stream.XReadGroup(key, ">", "group1", "alice", count: 2), key, "1-0", "2-0");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(key, "group1", "1-0"), Is.EqualTo(1));
            Assert.That(this._redis.Stream.XAck(key, "group1", (EntryId)"2-0"), Is.EqualTo(1));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }
    [Test]
    public void XReadGroup_GroupStartDollar()
    {
        var key = XReadGroupKey(); this.XReadGroupSeed(key, start: "$");
        Assert.That(this._redis.Stream.XReadGroup(key, ">", "group1", "alice"), Is.Null);
        this._redis.Stream.XAdd(key, "6-0", "name", "v6");
        XReadAssert(this._redis.Stream.XReadGroup(key, ">", "group1", "alice"), key, "6-0");
    }
    [Test]
    public void XReadGroup_Pending_ReReadOwnHistory()
    {
        var key = XReadGroupKey(); this.XReadGroupSeed(key);
        XReadAssert(this._redis.Stream.XReadGroup(key, ">", "group1", "alice", count: 2), key, "1-0", "2-0");
        XReadAssert(this._redis.Stream.XReadGroup(key, "0", "group1", "alice"), key, "1-0", "2-0");
        Assert.IsNull(this._redis.Stream.XReadGroup(key, "0", "group1", "bob")!.Value.Entries);
    }
    [Test]
    public void XReadGroup_NoAck_NotPending()
    {
        var key = XReadGroupKey(); this.XReadGroupSeed(key);
        XReadAssert(this._redis.Stream.XReadGroup(key, ">", "group1", "alice", count: 2, noack: true), key, "1-0", "2-0");
        Assert.That(this._redis.Stream.XAck(key, "group1", "1-0"), Is.Zero);
        Assert.IsNull(this._redis.Stream.XReadGroup(key, "0", "group1", "alice")!.Value.Entries);
    }
    [Test]
    public void XReadGroup_NewMessages_AreNotRedelivered()
    {
        var key = XReadGroupKey(); this.XReadGroupSeed(key);
        XReadAssert(this._redis.Stream.XReadGroup(key, ">", "group1", "alice", count: 5), key, "1-0", "2-0", "3-0", "4-0", "5-0");
        Assert.That(this._redis.Stream.XReadGroup(key, ">", "group1", "bob"), Is.Null);
    }
    [Test]
    public void XReadGroup_Collection_Kvp()
    {
        var a = XReadGroupKey(); var b = XReadGroupKey(); this.XReadGroupSeed(a); this.XReadGroupSeed(b);
        ICollection<KeyValuePair<SharpValue, EntryId>> keys = [new(a, ">"), new(b, ">")];
        XReadAssertMany(this._redis.Stream.XReadGroup(keys, "group1", "alice", count: 2), (a, ["1-0", "2-0"]), (b, ["1-0", "2-0"]));
    }
    [Test]
    public void XReadGroup_Span_Kvp()
    {
        var a = XReadGroupKey(); var b = XReadGroupKey(); this.XReadGroupSeed(a); this.XReadGroupSeed(b);
        XReadAssertMany(this._redis.Stream.XReadGroup((ReadOnlySpan<KeyValuePair<SharpValue, EntryId>>)[new(a, ">"), new(b, ">")], "group1", "alice", count: 1), (a, ["1-0"]), (b, ["1-0"]));
    }
    [Test]
    public void XReadGroup_Collection_Tuple()
    {
        var a = XReadGroupKey(); var b = XReadGroupKey(); this.XReadGroupSeed(a); this.XReadGroupSeed(b);
        ICollection<(SharpValue key, EntryId id)> keys = [(a, ">"), (b, ">")];
        XReadAssertMany(this._redis.Stream.XReadGroup(keys, "group1", "alice", count: 1), (a, ["1-0"]), (b, ["1-0"]));
    }
    [Test]
    public void XReadGroup_Span_Tuple()
    {
        var a = XReadGroupKey(); var b = XReadGroupKey(); this.XReadGroupSeed(a); this.XReadGroupSeed(b);
        XReadAssertMany(this._redis.Stream.XReadGroup((ReadOnlySpan<(SharpValue key, EntryId id)>)[(a, ">"), (b, ">")], "group1", "alice", count: 1), (a, ["1-0"]), (b, ["1-0"]));
    }
    [Test]
    public void XReadGroup_Block_Timeout()
    {
        var key = XReadGroupKey(); this.XReadGroupSeed(key, start: "$");
        Assert.That(this._redis.Stream.XReadGroup(key, ">", "group1", "alice", block: 30), Is.Null);
    }
    [Test]
    public void XReadGroup_GroupDoesNotExist()
    {
        var key = XReadGroupKey(); this.XReadSeed(key);
        XReadGroupRedisError(() => this._redis.Stream.XReadGroup(key, ">", "missing", "alice"));
    }
    [Test]
    public void XReadGroup_StreamDoesNotExist()
    {
        var key = XReadGroupKey();
        XReadGroupRedisError(() => this._redis.Stream.XReadGroup(key, ">", "group1", "alice"));
    }
    [Test]
    public void XReadGroup_WrongType()
    {
        var key = XReadGroupKey(); this._redis.String.Set(key, "not-stream");
        XReadGroupRedisError(() => this._redis.Stream.XReadGroup(key, ">", "group1", "alice"));
    }
    [Test]
    public void XReadGroup_InvalidId()
    {
        var key = XReadGroupKey(); this.XReadGroupSeed(key);
        XReadGroupRedisError(() => this._redis.Stream.XReadGroup(key, "*", "group1", "alice"));
    }
    [Test]
    public void XReadGroup_Flags_And_CancellationToken()
    {
        var key = XReadGroupKey(); this.XReadGroupSeed(key);
        XReadAssert(this._redis.Stream.XReadGroup(key, ">", "group1", "alice", count: 1, flags: CommandFlags.None, cancellationToken: CancellationToken.None), key, "1-0");
    }

    [Test]
    public void XRead_MaxCount_MaxSize_Redis810()
    {
        var key = XReadKey(); this.XReadSeed(key);
        var result = this._redis.Stream.XRead(key, "0-0", maxCount: 2, maxSize: 4096);
        XReadAssert(result, key, "1-0", "2-0");
    }

    [Test]
    public void XReadGroup_MaxCount_MaxSize_Redis810()
    {
        var key = XReadGroupKey(); this.XReadGroupSeed(key);
        XReadAssert(this._redis.Stream.XReadGroup(key, ">", "group1", "alice", maxCount: 2, maxSize: 4096), key, "1-0", "2-0");
    }

    [Test]
    public void XReadGroup_Claim_Redis84()
    {
        var key = XReadGroupKey(); this.XReadGroupSeed(key);
        Assert.That(this._redis.Stream.XReadGroup(key, ">", "group1", "alice", count: 1, claim: 0), Is.Not.Null);
    }
    #endregion

    #region XAckDel

    private static string XAckDelKey() => $"Stream:{{XAckDel}}:{Guid.NewGuid():N}";

    private static void XAckDelRedisError(Action action) => Assert.That(action, Throws.TypeOf<RedisErrorException>());

    private void XAckDelSeed(string key)
    {
        for (int i = 1; i <= 5; i++)
            Assert.That(this._redis.Stream.XAdd(key, $"{i}-0", "f", $"v{i}"), Is.EqualTo($"{i}-0"));
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    private void XAckDelCreateAndRead(string key, string group, string consumer)
    {
        Assert.That(this._redis.Stream.XGroupCreate(key, group, "0-0"), Is.True);
        var items = this._redis.Stream.XReadGroup(key, ">", group, consumer, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(items.HasValue, Is.True);
            Assert.That(items!.Value.Entries, Has.Length.EqualTo(5));
        }
        for (int i = 0; i < 5; i++)
            Assert.That(items.Value.Entries![i].Id, Is.EqualTo($"{i + 1}-0"));
    }

    private void XAckDelPrepare(string key)
    {
        this.XAckDelSeed(key);
        this.XAckDelCreateAndRead(key, "group1", "consumer1");
    }

    [Test]
    public void XAckDel_Single_Default()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0"), Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
            Assert.That(this._redis.Stream.XAck(key, "group1", "1-0"), Is.Zero);
        }
    }

    [Test]
    public void XAckDel_Single_ExplicitKeepRef()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0", delPolicy: DeletionPolicy.KeepRef),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XAckDel_Single_NotFound()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "100-0"), Is.EqualTo(DeleteResult.NotFound));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XAckDel_Single_Repeat()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0"), Is.EqualTo(DeleteResult.Deleted));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0"), Is.EqualTo(DeleteResult.NotFound));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XAckDel_Single_NotPending()
    {
        var key = XAckDelKey();
        this.XAckDelSeed(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreate(key, "group1", "0-0"), Is.True);
            // Stream 有该 ID，但该组还没有领取过，因此没有 PEL 记录。
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0"), Is.EqualTo(DeleteResult.NotFound));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XAckDel_Collection_Multiple()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        ICollection<EntryId> ids = ["1-0", "2-0", "3-0"];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", ids), Is.EqualTo(
            [
                DeleteResult.Deleted, DeleteResult.Deleted, DeleteResult.Deleted
            ]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(2));
        }
    }

    [Test]
    public void XAckDel_Collection_Mixed()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        ICollection<EntryId> ids = ["1-0", "100-0", "2-0"];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", ids), Is.EqualTo(
            [
                DeleteResult.Deleted, DeleteResult.NotFound, DeleteResult.Deleted
            ]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }
    }

    [Test]
    public void XAckDel_Span_Multiple()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", (ReadOnlySpan<EntryId>)["1-0", "2-0", "3-0"]),
                    Is.EqualTo([DeleteResult.Deleted, DeleteResult.Deleted, DeleteResult.Deleted]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(2));
        }
    }

    [Test]
    public void XAckDel_Span_Mixed()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", ["1-0", "100-0", "2-0"]),
                    Is.EqualTo([DeleteResult.Deleted, DeleteResult.NotFound, DeleteResult.Deleted]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }
    }

    [Test]
    public void XAckDel_KeepRef_OtherGroupPELRemains()
    {
        var key = XAckDelKey();
        this.XAckDelSeed(key);
        this.XAckDelCreateAndRead(key, "group1", "consumer1");
        this.XAckDelCreateAndRead(key, "group2", "consumer2");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0", delPolicy: DeletionPolicy.KeepRef),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
            Assert.That(this._redis.Stream.XAck(key, "group2", "1-0"), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAckDel_DelRef_OtherGroupPELCleared()
    {
        var key = XAckDelKey();
        this.XAckDelSeed(key);
        this.XAckDelCreateAndRead(key, "group1", "consumer1");
        this.XAckDelCreateAndRead(key, "group2", "consumer2");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0", delPolicy: DeletionPolicy.DelRef),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
            Assert.That(this._redis.Stream.XAck(key, "group2", "1-0"), Is.Zero);
        }
    }

    [Test]
    public void XAckDel_Acked_StillReferencedThenLastGroupDeletes()
    {
        var key = XAckDelKey();
        this.XAckDelSeed(key);
        this.XAckDelCreateAndRead(key, "group1", "consumer1");
        this.XAckDelCreateAndRead(key, "group2", "consumer2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0", delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo(DeleteResult.StillReferenced));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
            Assert.That(this._redis.Stream.XAck(key, "group1", "1-0"), Is.Zero);
        }

        using (Assert.EnterMultipleScope())
        {
            // 同一组已经确认；不应期待它第二次返回 Deleted。
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.NotFound));
            // 最后由仍有 PEL 的 group2 执行 XACKDEL。
            Assert.That(this._redis.Stream.XAckDel(key, "group2", "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XAckDel_Acked_OtherGroupAcknowledgedFirst()
    {
        var key = XAckDelKey();
        this.XAckDelSeed(key);
        this.XAckDelCreateAndRead(key, "group1", "consumer1");
        this.XAckDelCreateAndRead(key, "group2", "consumer2");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(key, "group2", "1-0"), Is.EqualTo(1));
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XAckDel_Acked_CollectionMixed()
    {
        var key = XAckDelKey();
        this.XAckDelSeed(key);
        this.XAckDelCreateAndRead(key, "group1", "consumer1");
        this.XAckDelCreateAndRead(key, "group2", "consumer2");
        Assert.That(this._redis.Stream.XAck(key, "group2", "1-0"), Is.EqualTo(1));
        ICollection<EntryId> ids = ["1-0", "2-0", "100-0"];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", ids, delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo([DeleteResult.Deleted, DeleteResult.StillReferenced, DeleteResult.NotFound]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
            // 2-0 仍由 group2 持有，可以由 group2 最后确认并删除。
            Assert.That(this._redis.Stream.XAckDel(key, "group2", "2-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.Deleted));
        }
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
    }

    [Test]
    public void XAckDel_GroupNotExists()
    {
        var key = XAckDelKey();
        this.XAckDelSeed(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "missing", "1-0"), Is.EqualTo(DeleteResult.NotFound));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XAckDel_KeyNotExists()
    {
        var key = XAckDelKey();
        Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0"), Is.EqualTo(DeleteResult.NotFound));
    }

    [Test]
    public void XAckDel_WrongKeyType()
    {
        var key = XAckDelKey();
        this._redis.String.Set(key, "hello");
        XAckDelRedisError(() => this._redis.Stream.XAckDel(key, "group1", "1-0"));
    }

    [Test]
    public void XAckDel_InvalidId()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        XAckDelRedisError(() => this._redis.Stream.XAckDel(key, "group1", "1-*"));
        Assert.That(() => this._redis.Stream.XAckDel(key, "group1", "18446744073709551616-0"), Throws.TypeOf<ArgumentException>());
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    [Test]
    public void XAckDel_Collection_Empty()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        ICollection<EntryId> ids = [];
        Assert.That(() => this._redis.Stream.XAckDel(key, "group1", ids), Throws.TypeOf<NotSupportedException>());
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    [Test]
    public void XAckDel_Span_Empty()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        Assert.That(() => this._redis.Stream.XAckDel(key, "group1", []), Throws.TypeOf<NotSupportedException>());
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    [Test]
    public void XAckDel_Flags_None()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0", flags: CommandFlags.None),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XAckDel_CancellationToken_Default()
    {
        var key = XAckDelKey();
        this.XAckDelPrepare(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAckDel(key, "group1", "1-0", cancellationToken: CancellationToken.None),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }
    #endregion

    #region XAdd

    private static string XAddKey() => $"Stream:{{XAdd}}:{Guid.NewGuid():N}";

    private static void XAddRedisError(Action action)
    {
        Assert.That(action, Throws.TypeOf<RedisErrorException>());
    }

    private void XAddSeed(string key, int count = 5)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(this._redis.Stream.XAdd(key, $"{i}-0", "f", $"v{i}"), Is.EqualTo($"{i}-0"));
        }
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(count));
    }

    [Test]
    public void XAdd_Overload_FieldValue_ExplicitId()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", "field", "value"), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_FieldValue_AutoId()
    {
        var key = XAddKey();
        var id = this._redis.Stream.XAdd(key, "field", "value");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_Tuple_ExplicitId()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", ("field", "value")), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_Tuple_AutoId()
    {
        var key = XAddKey();
        var id = this._redis.Stream.XAdd(key, ("field", "value"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_KeyValuePair_ExplicitId()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", new KeyValuePair<SharpValue, SharpValue>("field", "value")), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_KeyValuePair_AutoId()
    {
        var key = XAddKey();
        var id = this._redis.Stream.XAdd(key, new KeyValuePair<SharpValue, SharpValue>("field", "value"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_TupleCollection_ExplicitId()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", (ICollection<(SharpValue field, SharpValue value)>)[("field", "value"), ("field2", "value2")]), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_TupleCollection_AutoId()
    {
        var key = XAddKey();
        var id = this._redis.Stream.XAdd(key, (ICollection<(SharpValue field, SharpValue value)>)new List<(SharpValue field, SharpValue value)> { ("field", "value"), ("field2", "value2") });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_PairCollection_ExplicitId()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", (ICollection<KeyValuePair<SharpValue, SharpValue>>)[new("field", "value"), new("field2", "value2")]), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_PairCollection_AutoId()
    {
        var key = XAddKey();
        var id = this._redis.Stream.XAdd(key, new List<KeyValuePair<SharpValue, SharpValue>> { new("field", "value"), new("field2", "value2") });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_TupleSpan_ExplicitId()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", [("field", "value"), ("field2", "value2")]), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_TupleSpan_AutoId()
    {
        var key = XAddKey();
        var id = this._redis.Stream.XAdd(key, (ReadOnlySpan<(SharpValue field, SharpValue value)>)[("field", "value"), ("field2", "value2")]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_PairSpan_ExplicitId()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", (ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>>)[new("field", "value"), new("field2", "value2")]), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Overload_PairSpan_AutoId()
    {
        var key = XAddKey();
        var id = this._redis.Stream.XAdd(key, (ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>>)[new("field", "value"), new("field2", "value2")]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Id_StrictIncreasing()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", "f", "a"), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XAdd(key, "1-1", "f", "b"), Is.EqualTo("1-1"));
            Assert.That(this._redis.Stream.XAdd(key, "2-0", "f", "c"), Is.EqualTo("2-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }
    }

    [Test]
    public void XAdd_Id_Duplicate()
    {
        var key = XAddKey();
        this._redis.Stream.XAdd(key, "2-0", "f", "v");
        XAddRedisError(() => this._redis.Stream.XAdd(key, "2-0", "f", "v"));
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
    }

    [Test]
    public void XAdd_Id_Decreasing()
    {
        var key = XAddKey();
        this._redis.Stream.XAdd(key, "2-0", "f", "v");
        XAddRedisError(() => this._redis.Stream.XAdd(key, "1-9", "f", "v"));
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
    }

    [Test]
    public void XAdd_Id_ZeroZero()
    {
        var key = XAddKey();
        XAddRedisError(() => this._redis.Stream.XAdd(key, "0-0", "f", "v"));
        Assert.That(this._redis.Stream.XLen(key), Is.Zero);
    }

    [Test]
    public void XAdd_Id_DeletedDoesNotResetLastId()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "2-0", "f", "a"), Is.EqualTo("2-0"));
            Assert.That(this._redis.Stream.XDel(key, (EntryId)"2-0"), Is.EqualTo(1));
        }
        XAddRedisError(() => this._redis.Stream.XAdd(key, "2-0", "f", "b"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "2-1", "f", "c"), Is.EqualTo("2-1"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Id_MillisecondAutoSequence()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, (EntryId)1UL, "f", "a"), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XAdd(key, (EntryId)1UL, "f", "b"), Is.EqualTo("1-1"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(2));
        }
    }

    [Test]
    public void XAdd_Nomkstream_NewAndExisting()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "f", "v", nomkstream: true), Is.Null);
            Assert.That(this._redis.Stream.XLen(key), Is.Zero);
            Assert.That(this._redis.Stream.XAdd(key, "1-0", "f", "a"), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XAdd(key, "2-0", "f", "b", nomkstream: true), Is.EqualTo("2-0"));
        }
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(2));
    }

    [Test]
    public void XAdd_Trim_MaxLen_Exact()
    {
        var key = XAddKey();
        this.XAddSeed(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "6-0", "f", "v6", trimOptions: TrimOptions.ByMaxLen(3, TrimOperator.Exact)), Is.EqualTo("6-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
            Assert.That(this._redis.Stream.XAdd(key, "7-0", "f", "v7", trimOptions: new TrimOptions(2, TrimOperator.None)), Is.EqualTo("7-0"));
        }
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(2));
    }

    [Test]
    public void XAdd_Trim_MaxLen_Zero()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", "f", "v", trimOptions: TrimOptions.ByMaxLen(0, TrimOperator.Exact)), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.Zero);
        }
    }

    [Test]
    public void XAdd_Trim_MaxLen_Approximate()
    {
        var key = XAddKey();
        this.XAddSeed(key, 150);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "151-0", "f", "v", trimOptions: TrimOptions.ByMaxLen(10, TrimOperator.Approximate, limit: 0)), Is.EqualTo("151-0"));
            // ~ 为近似裁剪，可能多留旧消息；只断言长度合法。
            Assert.That(this._redis.Stream.XLen(key), Is.InRange(1, 151));
        }
    }

    [Test]
    public void XAdd_Trim_MinId_Exact()
    {
        var key = XAddKey();
        this.XAddSeed(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "6-0", "f", "v6", trimOptions: TrimOptions.ByMinId("3-0", TrimOperator.Exact)), Is.EqualTo("6-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XAdd_Trim_MinId_Milliseconds()
    {
        var key = XAddKey();
        this.XAddSeed(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "6-0", "f", "v6", trimOptions: new TrimOptions("4", TrimOperator.None)), Is.EqualTo("6-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }
    }

    [Test]
    public void XAdd_Trim_MinId_Approximate()
    {
        var key = XAddKey();
        this.XAddSeed(key, 150);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "151-0", "f", "v151", trimOptions: TrimOptions.ByMinId("100-0", TrimOperator.Approximate, limit: 0)), Is.EqualTo("151-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.InRange(52, 151));
        }
    }

    [Test]
    public void XAdd_Trim_Default_None()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", "f", "v", trimOptions: default), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Trim_MaxLen_ConstructorValidation()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TrimOptions(-1, TrimOperator.Exact));
        Assert.Throws<ArgumentOutOfRangeException>(() => TrimOptions.ByMaxLen(3, TrimOperator.Approximate, -1));
        Assert.Throws<ArgumentException>(() => TrimOptions.ByMaxLen(3, TrimOperator.Exact, 1));
        Assert.Throws<ArgumentException>(() => TrimOptions.ByMaxLen(3, TrimOperator.None, 0));
    }

    [Test]
    public void XAdd_Trim_MinId_ConstructorValidation()
    {
        foreach (var invalid in new[] { "", "abc", "*", "$", ">", "-", "+", "-1", "1-", "1-a", "1-2-3", "18446744073709551616-0" })
        {
            Assert.Throws<ArgumentException>(() => TrimOptions.ByMinId(invalid, TrimOperator.Exact), invalid);
        }
        Assert.Throws<ArgumentOutOfRangeException>(() => TrimOptions.ByMinId("2-0", TrimOperator.Approximate, -1));
        Assert.Throws<ArgumentException>(() => TrimOptions.ByMinId("2-0", TrimOperator.Exact, 1));
        foreach (var valid in new[] { "0", "1", "1-0", "18446744073709551615-18446744073709551615" })
            Assert.That(TrimOptions.ByMinId(valid, TrimOperator.Exact).MinId, Is.EqualTo(valid));
    }

    [Test]
    public void XAdd_Trim_Properties()
    {
        var max = TrimOptions.ByMaxLen(5, TrimOperator.Approximate, 0);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(max.HasValue, Is.True);
            Assert.That(max.MaxLen, Is.EqualTo(5));
            Assert.That(max.MinId, Is.Null);
            Assert.That(max.Limit, Is.Zero);
            Assert.That(max.Operator, Is.EqualTo(TrimOperator.Approximate));
            Assert.That(default(TrimOptions).HasValue, Is.False);
        }
        var min = TrimOptions.ByMinId("3-0", TrimOperator.Exact);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(min.HasValue, Is.True);
            Assert.That(min.MinId, Is.EqualTo("3-0"));
            Assert.That(min.MaxLen, Is.Null);
        }
    }

    [Test]
    public void XAdd_WrongKeyType()
    {
        var key = XAddKey();
        this._redis.String.Set(key, "not-a-stream");
        XAddRedisError(() => this._redis.Stream.XAdd(key, "f", "v"));
    }

    [Test]
    public void XAdd_Flags_NoneAndCancellationToken()
    {
        var key = XAddKey();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", "f", "v", flags: CommandFlags.None, cancellationToken: CancellationToken.None), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_UnicodeAndMultipleFields()
    {
        var key = XAddKey();
        ICollection<(SharpValue field, SharpValue value)> fields = [("中文字段", "你好🌍"), ("emoji", "😃")];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "1-0", fields), Is.EqualTo("1-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_DelPolicy_NoGroups()
    {
        foreach (var policy in new[] { DeletionPolicy.KeepRef, DeletionPolicy.DelRef, DeletionPolicy.Acked })
        {
            var key = XAddKey();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(this._redis.Stream.XAdd(key, "1-0", "f", "v", delPolicy: policy, trimOptions: TrimOptions.ByMaxLen(1, TrimOperator.Exact)), Is.EqualTo("1-0"));
                Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
            }
        }
    }

    [Test]
    public void XAdd_DelPolicy_KeepRefAndDelRef()
    {
        foreach (var policy in new[] { DeletionPolicy.KeepRef, DeletionPolicy.DelRef })
        {
            var key = XAddKey();
            this.XAddSeed(key, 2);
            Assert.That(this._redis.Stream.XGroupCreate(key, "g", "0-0"), Is.True);
            var read = this._redis.Stream.XReadGroup(key, ">", "g", "c", count: 2);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(read.HasValue, Is.True);
                Assert.That(this._redis.Stream.XAdd(key, "3-0", "f", "v3", trimOptions: TrimOptions.ByMaxLen(1, TrimOperator.Exact), delPolicy: policy), Is.EqualTo("3-0"));
                Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
                Assert.That(this._redis.Stream.XAck(key, "g", (EntryId)"1-0"), Is.EqualTo(policy == DeletionPolicy.KeepRef ? 1 : 0));
            }
        }
    }

    [Test]
    public void XAdd_DelPolicy_AckedProtectsPending()
    {
        var key = XAddKey();
        this.XAddSeed(key, 2);
        Assert.That(this._redis.Stream.XGroupCreate(key, "g", "0-0"), Is.True);
        var read = this._redis.Stream.XReadGroup(key, ">", "g", "c", count: 2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(this._redis.Stream.XAdd(key, "3-0", "f", "v3", trimOptions: TrimOptions.ByMaxLen(1, TrimOperator.Exact), delPolicy: DeletionPolicy.Acked), Is.EqualTo("3-0"));
            Assert.That(this._redis.Stream.XLen(key), Is.GreaterThan(1));
            Assert.That(this._redis.Stream.XAck(key, "g", (EntryId)"1-0"), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Idempotent_Manual_Deduplicated()
    {
        var key = XAddKey();
        var a = this._redis.Stream.XAdd(key, "f", "v", idempotent: ("p", "m1"));
        var b = this._redis.Stream.XAdd(key, "f", "v", idempotent: ("p", "m1"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(a, Is.Not.Null.And.Not.Empty);
            Assert.That(b, Is.EqualTo(a));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Idempotent_Auto_Deduplicated()
    {
        var key = XAddKey();
        var a = this._redis.Stream.XAdd(key, "f", "v", idempotent: "producer1");
        var b = this._redis.Stream.XAdd(key, "f", "v", idempotent: "producer1");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(a, Is.Not.Null.And.Not.Empty);
            Assert.That(b, Is.EqualTo(a));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAdd_Idempotent_ExplicitStreamIdRejected()
    {
        var key = XAddKey();
        XAddRedisError(() => this._redis.Stream.XAdd(key, "1-0", "f", "v", idempotent: ("p", "m1")));
    }

    [Test]
    public void XAdd_Trim_Limit_Positive()
    {
        var key = XAddKey();
        this.XAddSeed(key, 150);
        var trim = TrimOptions.ByMaxLen(10, TrimOperator.Approximate, limit: 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(key, "151-0", "f", "v", trimOptions: trim), Is.EqualTo("151-0"));
            // LIMIT=1 限制扫描规模，实际删除量由 Redis 节点结构决定。
            Assert.That(this._redis.Stream.XLen(key), Is.InRange(1, 151));
        }
    }

    [Test]
    public void XAdd_EmptyTupleCollection()
    {
        var key = XAddKey();
        ICollection<(SharpValue field, SharpValue value)> fields = [];
        Assert.That(() => this._redis.Stream.XAdd(key, fields), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public void XAdd_EmptyPairCollection()
    {
        var key = XAddKey();
        ICollection<KeyValuePair<SharpValue, SharpValue>> fields = [];
        Assert.That(() => this._redis.Stream.XAdd(key, fields), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public void XAdd_EmptyTupleSpan()
    {
        var key = XAddKey();
        Assert.That(() =>
        {
            ReadOnlySpan<(SharpValue field, SharpValue value)> fields = [];
            this._redis.Stream.XAdd(key, fields);
        }, Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public void XAdd_EmptyPairSpan()
    {
        var key = XAddKey();
        ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>> fields = [];
        Assert.That(() =>
        {
            ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>> fields = [];
            this._redis.Stream.XAdd(key, fields);
        }, Throws.TypeOf<NotSupportedException>());
    }
    #endregion

    #region XRange

    private static string XRangeKey() => $"Stream:{{XRange}}:{Guid.NewGuid():N}";

    private void XRangeSeed(string key)
    {
        for (int i = 1; i <= 5; i++)
            Assert.That(this._redis.Stream.XAdd(key, $"{i}-0", "field", $"value{i}"), Is.EqualTo($"{i}-0"));
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    // SharpRedis 的公开 API 对空结果可能返回 null；非空结果必须完整核对。
    private static void XRangeAssert(EntryItem[]? items, params string[] ids)
    {
        Assert.That(items, Is.Not.Null);
        Assert.That(items, Has.Length.EqualTo(ids.Length));
        for (int i = 0; i < ids.Length; i++)
        {
            var item = items![i];
            using (Assert.EnterMultipleScope())
            {
                Assert.That(item.Id, Is.EqualTo(ids[i]));
                Assert.That(item.FieldValue, Has.Length.EqualTo(1));
                Assert.That(item.FieldValue![0].Key.ToString(), Is.EqualTo("field"));
                Assert.That(item.FieldValue[0].Value.ToString(), Is.EqualTo("value" + ids[i].Split('-')[0]));
                Assert.That(item.IdleTime, Is.Null);
                Assert.That(item.DeliveryCount, Is.Null);
            }
        }
    }

    private static void XRangeAssertEmpty(EntryItem[]? items)
    {
        // Redis 原生返回空数组 []；按当前 SharpRedis 公共 API 的空结果约定断言 null。
        Assert.That(items, Is.Null);
    }

    [Test]
    public void XRange_All()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "-", "+"), "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XRange_Inclusive()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "2-0", "4-0"), "2-0", "3-0", "4-0");
    }

    [Test]
    public void XRange_SingleEntry()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "3-0", "3-0"), "3-0");
    }

    [Test]
    public void XRange_StartExclusive()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "(2-0", "4-0"), "3-0", "4-0");
    }

    [Test]
    public void XRange_EndExclusive()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "2-0", "(4-0"), "2-0", "3-0");
    }

    [Test]
    public void XRange_BothExclusive()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "(2-0", "(4-0"), "3-0");
    }

    [Test]
    public void XRange_ExclusiveSameId()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssertEmpty(this._redis.Stream.XRange(key, "(3-0", "3-0"));
        XRangeAssertEmpty(this._redis.Stream.XRange(key, "3-0", "(3-0"));
    }

    [Test]
    public void XRange_OnlyStartBoundary()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "3-0", "+"), "3-0", "4-0", "5-0");
    }

    [Test]
    public void XRange_OnlyEndBoundary()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "-", "3-0"), "1-0", "2-0", "3-0");
    }

    [Test]
    public void XRange_NoMatchingEntries()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssertEmpty(this._redis.Stream.XRange(key, "100-0", "200-0"));
    }

    [Test]
    public void XRange_StartGreaterThanEnd()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssertEmpty(this._redis.Stream.XRange(key, "4-0", "2-0"));
    }

    [Test]
    public void XRange_Count_Null()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "-", "+", count: null), "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XRange_Count_One()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "-", "+", count: 1), "1-0");
    }

    [Test]
    public void XRange_Count_Two()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "-", "+", count: 2), "1-0", "2-0");
    }

    [Test]
    public void XRange_Count_GreaterThanLength()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "-", "+", count: 100), "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XRange_Pagination()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        var page1 = this._redis.Stream.XRange(key, "-", "+", count: 2);
        XRangeAssert(page1, "1-0", "2-0");
        var page2 = this._redis.Stream.XRange(key, $"({page1![^1].Id}", "+", count: 2);
        XRangeAssert(page2, "3-0", "4-0");
        var page3 = this._redis.Stream.XRange(key, $"({page2![^1].Id}", "+", count: 2);
        XRangeAssert(page3, "5-0");
        XRangeAssertEmpty(this._redis.Stream.XRange(key, $"({page3![^1].Id}", "+", count: 2));
    }

    [Test]
    public void XRange_SameMillisecond_Sequences()
    {
        var key = XRangeKey();
        for (int i = 0; i < 4; i++)
            Assert.That(this._redis.Stream.XAdd(key, $"100-{i}", "f", $"v{i}"), Is.EqualTo($"100-{i}"));
        var result = this._redis.Stream.XRange(key, "100-1", "100-2");
        Assert.That(result, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result![0].Id, Is.EqualTo("100-1"));
            Assert.That(result[1].Id, Is.EqualTo("100-2"));
        }
    }

    [Test]
    public void XRange_MillisecondRange_ExplicitBounds()
    {
        var key = XRangeKey();
        this._redis.Stream.XAdd(key, "100-0", "f", "a");
        this._redis.Stream.XAdd(key, "100-1", "f", "b");
        this._redis.Stream.XAdd(key, "101-0", "f", "c");
        // 不传单独的 "100"，避免依赖 EntryId 当前是否支持不完整数字 ID。
        var items = this._redis.Stream.XRange(key, "100-0", "100-18446744073709551615");
        Assert.That(items, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(items![0].Id, Is.EqualTo("100-0"));
            Assert.That(items[1].Id, Is.EqualTo("100-1"));
        }
    }

    [Test]
    public void XRange_FieldValue()
    {
        var key = XRangeKey();
        Assert.That(this._redis.Stream.XAdd(key, "1-0", "name", "测试中文"), Is.EqualTo("1-0"));
        var items = this._redis.Stream.XRange(key, "-", "+");
        Assert.That(items, Has.Length.EqualTo(1));
        var entry = items![0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entry.Id, Is.EqualTo("1-0"));
            Assert.That(entry.FieldValue, Has.Length.EqualTo(1));
            Assert.That(entry.FieldValue![0].Key.ToString(), Is.EqualTo("name"));
            Assert.That(entry.FieldValue[0].Value.ToString(), Is.EqualTo("测试中文"));
            Assert.That(entry.IdleTime, Is.Null);
            Assert.That(entry.DeliveryCount, Is.Null);
        }
    }

    [Test]
    public void XRange_MultipleFields()
    {
        var key = XRangeKey();
        Assert.That(this._redis.Stream.XAdd(key, "1-0",
            (ReadOnlySpan<(SharpValue field, SharpValue value)>)[("name", "张三"), ("age", "18"), ("city", "上海")]),
            Is.EqualTo("1-0"));
        var items = this._redis.Stream.XRange(key, "-", "+");
        Assert.That(items, Has.Length.EqualTo(1));
        var fields = items![0].FieldValue;
        Assert.That(fields, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(fields[0].Key.ToString(), Is.EqualTo("name"));
            Assert.That(fields[0].Value.ToString(), Is.EqualTo("张三"));
            Assert.That(fields[1].Key.ToString(), Is.EqualTo("age"));
            Assert.That(fields[1].Value.ToString(), Is.EqualTo("18"));
            Assert.That(fields[2].Key.ToString(), Is.EqualTo("city"));
            Assert.That(fields[2].Value.ToString(), Is.EqualTo("上海"));
        }
    }

    [Test]
    public void XRange_AfterDelete()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        Assert.That(this._redis.Stream.XDel(key, (EntryId)"3-0"), Is.EqualTo(1));
        XRangeAssert(this._redis.Stream.XRange(key, "-", "+"), "1-0", "2-0", "4-0", "5-0");
        XRangeAssertEmpty(this._redis.Stream.XRange(key, "3-0", "3-0"));
    }

    [Test]
    public void XRange_KeyNotExists()
    {
        var key = XRangeKey();
        XRangeAssertEmpty(this._redis.Stream.XRange(key, "-", "+"));
    }

    [Test]
    public void XRange_WrongKeyType()
    {
        var key = XRangeKey();
        this._redis.String.Set(key, "hello");
        Assert.That(() => this._redis.Stream.XRange(key, "-", "+"), Throws.TypeOf<RedisErrorException>());
    }

    [Test]
    public void XRange_InvalidId_ClientValidation()
    {
        Assert.Throws<ArgumentException>(() => new EntryId("abc"));
        Assert.Throws<ArgumentException>(() => new EntryId("("));
        Assert.Throws<ArgumentException>(() => new EntryId("1-2-3"));
        Assert.Throws<ArgumentException>(() => new EntryId("(1-*"));
    }

    [Test]
    public void XRange_InvalidId_Overflow()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        Assert.That(() => this._redis.Stream.XRange(key, "18446744073709551616-0", "+"),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void XRange_Flags_None()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "-", "+", flags: CommandFlags.None), "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XRange_CancellationToken_Default()
    {
        var key = XRangeKey();
        this.XRangeSeed(key);
        XRangeAssert(this._redis.Stream.XRange(key, "-", "+", cancellationToken: CancellationToken.None), "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    #endregion

    #region XAutoClaim

    private static string XAutoClaimKey() =>
        $"Stream:{{XAutoClaim}}:{Guid.NewGuid():N}";

    private static void XAutoClaimAssertIds(
        EntryItem[]? entries, params string[] ids)
    {
        Assert.That(entries?.Length ?? 0, Is.EqualTo(ids.Length));

        for (int i = 0; i < ids.Length; i++)
        {
            Assert.That(entries![i].Id, Is.EqualTo(ids[i]));
        }
    }

    private void XAutoClaimPrepare(string key, int count = 5)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(this._redis.Stream.XGroupCreate(
            key, "group1", "0-0"), Is.True);

        var read = this._redis.Stream.XReadGroup(
            key, ">", "group1", "consumer1", count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(read!.Value.Entries, Has.Length.EqualTo(count));

            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(count));
        }
    }

    [Test]
    public void XAutoClaim_All()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "0-0");

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));

        XAutoClaimAssertIds(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");

        Assert.IsNull(result.DeletedIds);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Entries![0].FieldValue![0].Key.ToString(),
                    Is.EqualTo("field"));

            Assert.That(result.Entries[0].FieldValue![0].Value.ToString(),
                Is.EqualTo("value1"));
        }
    }

    [Test]
    public void XAutoClaim_Count_One()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "0-0", count: 1);

        XAutoClaimAssertIds(result.Entries, "1-0");

        Assert.That(result.NextStartId, Is.Not.EqualTo("0-0"));
    }

    [Test]
    public void XAutoClaim_Count_Two()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "0-0", count: 2);

        XAutoClaimAssertIds(result.Entries, "1-0", "2-0");
    }

    [Test]
    public void XAutoClaim_Count_Null()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "0-0", count: null);

        XAutoClaimAssertIds(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XAutoClaim_Count_GreaterThanPending()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "0-0", count: 100);

        XAutoClaimAssertIds(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public void XAutoClaim_StartId()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "3-0");

        XAutoClaimAssertIds(result.Entries,
            "3-0", "4-0", "5-0");

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public void XAutoClaim_StartId_Zero()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "0");

        XAutoClaimAssertIds(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XAutoClaim_StartId_BeyondPending()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "100-0");

        XAutoClaimAssertIds(result.Entries);
        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public void XAutoClaim_MinIdleTime_NotReached()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        // 远大于测试刚创建消息后的闲置时间
        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2",
            minIdleTime: 3_600_000,
            startId: "0-0");

        XAutoClaimAssertIds(result.Entries);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.NextStartId, Is.EqualTo("0-0"));

            // 没有发生认领，仍可确认 consumer1 的 PEL 消息
            Assert.That(this._redis.Stream.XAck(
                key, "group1", "1-0"), Is.EqualTo(1));
        }
    }

    [Test]
    public void XAutoClaim_Pagination()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var allIds = new List<string>();
        EntryId cursor = "0-0";

        for (int page = 0; page < 10; page++)
        {
            var result = this._redis.Stream.XAutoClaim(
                key, "group1", "consumer2", 0, cursor, count: 2);

            if (result.Entries is not null)
            {
                foreach (var entry in result.Entries)
                {
                    allIds.Add(entry.Id);
                }
            }

            if (result.NextStartId == "0-0")
                break;

            cursor = result.NextStartId;
        }

        Assert.That(allIds, Is.EqualTo(
        [
        "1-0", "2-0", "3-0", "4-0", "5-0"
        ]));
    }

    [Test]
    public void XAutoClaim_DeletedIds()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDel(
                    key, "2-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XDel(
                key, "4-0"), Is.EqualTo(1));
        }

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "0-0");

        XAutoClaimAssertIds(result.Entries,
            "1-0", "3-0", "5-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.DeletedIds,
                    Is.EqualTo(["2-0", "4-0"]));

            Assert.That(result.NextStartId, Is.EqualTo("0-0"));
        }
    }

    [Test]
    public void XAutoClaim_NoPending()
    {
        var key = XAutoClaimKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(
                    key, "1-0", "field", "value1"), Is.EqualTo("1-0"));

            Assert.That(this._redis.Stream.XGroupCreate(
                key, "group1", "0-0"), Is.True);
        }

        // 没有执行 XREADGROUP，不存在 PEL 消息
        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "0-0");

        XAutoClaimAssertIds(result.Entries);
        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public void XAutoClaim_AfterAck()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        Assert.That(this._redis.Stream.XAck(
            key, "group1", "1-0"), Is.EqualTo(1));

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "0-0");

        XAutoClaimAssertIds(result.Entries,
            "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XAutoClaim_Flags_None()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "0-0",
            flags: CommandFlags.None);

        XAutoClaimAssertIds(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XAutoClaim_CancellationToken_Default()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2", 0, "0-0",
            cancellationToken: CancellationToken.None);

        XAutoClaimAssertIds(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XAutoClaim_GroupNotExists()
    {
        var key = XAutoClaimKey();

        Assert.That(this._redis.Stream.XAdd(
            key, "1-0", "field", "value1"), Is.EqualTo("1-0"));

        Assert.That(
            () => this._redis.Stream.XAutoClaim(
                key, "not_exists", "consumer2", 0, "0-0"),
            Throws.TypeOf<RedisErrorException>());
    }

    [Test]
    public void XAutoClaim_KeyNotExists()
    {
        var key = XAutoClaimKey();

        Assert.That(
            () => this._redis.Stream.XAutoClaim(
                key, "group1", "consumer2", 0, "0-0"),
            Throws.TypeOf<RedisErrorException>());
    }

    [Test]
    public void XAutoClaim_WrongKeyType()
    {
        var key = XAutoClaimKey();
        this._redis.String.Set(key, "hello");

        Assert.That(
            () => this._redis.Stream.XAutoClaim(
                key, "group1", "consumer2", 0, "0-0"),
            Throws.TypeOf<RedisErrorException>());
    }


    [Test]
    public void XAutoClaim_JustId()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "0-0",
            justid: true);

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));

        XAutoClaimAssertIds(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");

        Assert.IsNull(result.DeletedIds);

        foreach (var entry in result.Entries!)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(entry.FieldValue, Is.Null);
                Assert.That(entry.IdleTime, Is.Null);
                Assert.That(entry.DeliveryCount, Is.Null);
            }
        }
    }

    [Test]
    public void XAutoClaim_JustId_Count()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "0-0",
            count: 2,
            justid: true);

        XAutoClaimAssertIds(result.Entries, "1-0", "2-0");

        Assert.That(result.NextStartId, Is.EqualTo("3-0"));
    }

    [Test]
    public void XAutoClaim_JustId_StartId()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "3-0",
            justid: true);

        XAutoClaimAssertIds(result.Entries,
            "3-0", "4-0", "5-0");

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public void XAutoClaim_JustId_MinIdleTime()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2",
            minIdleTime: 3_600_000,
            startId: "0-0",
            justid: true);

        XAutoClaimAssertIds(result.Entries);

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public void XAutoClaim_JustId_NoPending()
    {
        var key = XAutoClaimKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(
                    key, "1-0", "field", "value1"),
                    Is.EqualTo("1-0"));

            Assert.That(this._redis.Stream.XGroupCreate(
                key, "group1", "0-0"), Is.True);
        }

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "0-0",
            justid: true);

        XAutoClaimAssertIds(result.Entries);

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public void XAutoClaim_JustId_DeletedIds()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDel(
                    key, "2-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XDel(
                key, "4-0"), Is.EqualTo(1));
        }

        var result = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "0-0",
            justid: true);

        XAutoClaimAssertIds(result.Entries,
            "1-0", "3-0", "5-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.DeletedIds,
                    Is.EqualTo(["2-0", "4-0"]));

            Assert.That(result.NextStartId, Is.EqualTo("0-0"));
        }
    }

    [Test]
    public void XAutoClaim_JustId_Pagination()
    {
        var key = XAutoClaimKey();
        this.XAutoClaimPrepare(key);

        var ids = new List<string>();
        EntryId startId = "0-0";
        bool completed = false;

        for (int i = 0; i < 10; i++)
        {
            var result = this._redis.Stream.XAutoClaim(
                key, "group1", "consumer2",
                minIdleTime: 0,
                startId: startId,
                count: 2,
                justid: true);

            if (result.Entries is not null)
            {
                foreach (var entry in result.Entries)
                {
                    Assert.That(entry.FieldValue, Is.Null);
                    ids.Add(entry.Id);
                }
            }

            if (result.NextStartId == "0-0")
            {
                completed = true;
                break;
            }

            startId = result.NextStartId;
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(completed, Is.True);
            Assert.That(ids, Is.EqualTo(
            [
                "1-0", "2-0", "3-0", "4-0", "5-0"
            ]));
        }
    }

    #endregion

    #region XPending

    private static string XPendingKey() =>
        $"Stream:{{XPending}}:{Guid.NewGuid():N}";

    private void XPendingSeed(string key, int count = 5)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(count));
            Assert.That(this._redis.Stream.XGroupCreate(key, "group1", "0-0"), Is.True);
        }
    }

    private void XPendingRead(
        string key, string consumer, int count)
    {
        var result = this._redis.Stream.XReadGroup(
            key, ">", "group1", consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries, Has.Length.EqualTo(count));
        }
    }

    private void XPendingPrepare(string key)
    {
        this.XPendingSeed(key);
        this.XPendingRead(key, "consumer1", 5);

        Assert.That(this._redis.Stream.XPending(
            key, "group1").Count, Is.EqualTo(5));
    }

    private static void XPendingAssertIds(
        XPendingItem[]? entries, params string[] ids)
    {
        Assert.That(entries?.Length ?? 0, Is.EqualTo(ids.Length));

        for (int i = 0; i < ids.Length; i++)
        {
            Assert.That(entries![i].Id, Is.EqualTo(ids[i]));
        }
    }

    private static void XPendingAssertDelivery(
        XPendingItem entry, string id,
        string consumer, long deliveries)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entry.Id, Is.EqualTo(id));
            Assert.That(entry.Consumer, Is.EqualTo(consumer));
            Assert.That(entry.DeliveryCount, Is.EqualTo(deliveries));
            Assert.That(entry.IdleTime, Is.GreaterThanOrEqualTo(0));
        }
    }

    [Test]
    public void XPending_Summary_NoPending()
    {
        var key = XPendingKey();
        this.XPendingSeed(key);

        var summary = this._redis.Stream.XPending(
            key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.Zero);
            Assert.That(summary.MinId, Is.Null);
            Assert.That(summary.MaxId, Is.Null);
            Assert.That(summary.Consumers?.Length ?? 0, Is.Zero);
        }
    }

    [Test]
    public void XPending_Summary_SingleConsumer()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var summary = this._redis.Stream.XPending(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.EqualTo(5));
            Assert.That(summary.MinId, Is.EqualTo("1-0"));
            Assert.That(summary.MaxId, Is.EqualTo("5-0"));
            Assert.That(summary.Consumers, Has.Length.EqualTo(1));
            Assert.That(summary.Consumers![0].Name, Is.EqualTo("consumer1"));
            Assert.That(summary.Consumers[0].Count, Is.EqualTo(5));
        }
    }

    [Test]
    public void XPending_Summary_MultipleConsumers()
    {
        var key = XPendingKey();
        this.XPendingSeed(key);

        this.XPendingRead(key, "consumer1", 2);
        this.XPendingRead(key, "consumer2", 3);

        var summary = this._redis.Stream.XPending(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.EqualTo(5));
            Assert.That(summary.MinId, Is.EqualTo("1-0"));
            Assert.That(summary.MaxId, Is.EqualTo("5-0"));
            Assert.IsNotNull(summary.Consumers);
            Assert.That(summary.Consumers, Has.Length.EqualTo(2));

            Assert.That(summary.Consumers!.Single(
                x => x.Name == "consumer1").Count,
                Is.EqualTo(2));

            Assert.That(summary.Consumers!.Single(
                x => x.Name == "consumer2").Count,
                Is.EqualTo(3));
        }
    }

    [Test]
    public void XPending_Summary_AfterAck()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(
                    key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XAck(
                key, "group1", "5-0"), Is.EqualTo(1));
        }

        var summary = this._redis.Stream.XPending(
            key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.EqualTo(3));
            Assert.That(summary.MinId, Is.EqualTo("2-0"));
            Assert.That(summary.MaxId, Is.EqualTo("4-0"));
            Assert.That(summary.Consumers![0].Count,
                Is.EqualTo(3));
        }
    }

    [Test]
    public void XPending_Detail_All()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var entries = this._redis.Stream.XPending(
            key, "group1", new PendingOptions("-", "+", 10));

        XPendingAssertIds(entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");

        for (int i = 0; i < 5; i++)
        {
            XPendingAssertDelivery(entries![i],
                $"{i + 1}-0", "consumer1", 1);
        }
    }

    [Test]
    public void XPending_Detail_InclusiveRange()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var entries = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("2-0", "4-0", 10));

        XPendingAssertIds(entries,
            "2-0", "3-0", "4-0");
    }

    [Test]
    public void XPending_Detail_StartExclusive()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var entries = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("(2-0", "5-0", 10));

        XPendingAssertIds(entries,
            "3-0", "4-0", "5-0");
    }

    [Test]
    public void XPending_Detail_EndExclusive()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var entries = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("2-0", "(5-0", 10));

        XPendingAssertIds(entries,
            "2-0", "3-0", "4-0");
    }

    [Test]
    public void XPending_Detail_BothExclusive()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var entries = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("(2-0", "(5-0", 10));

        XPendingAssertIds(entries, "3-0", "4-0");
    }

    [Test]
    public void XPending_Detail_EmptyRange()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        XPendingAssertIds(this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("(3-0", "3-0", 10)));

        XPendingAssertIds(this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("4-0", "2-0", 10)));

        XPendingAssertIds(this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("100-0", "+", 10)));
    }

    [Test]
    public void XPending_Detail_Count()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        XPendingAssertIds(this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 1)), "1-0");

        XPendingAssertIds(this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 2)),
            "1-0", "2-0");

        XPendingAssertIds(this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 100)),
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XPending_Detail_Pagination()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var page1 = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 2));

        XPendingAssertIds(page1, "1-0", "2-0");

        var page2 = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions($"({page1![^1].Id}", "+", 2));

        XPendingAssertIds(page2, "3-0", "4-0");

        var page3 = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions($"({page2![^1].Id}", "+", 2));

        XPendingAssertIds(page3, "5-0");

        var page4 = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions($"({page3![^1].Id}", "+", 2));

        XPendingAssertIds(page4);
    }

    [Test]
    public void XPending_Detail_ConsumerFilter()
    {
        var key = XPendingKey();
        this.XPendingSeed(key);

        this.XPendingRead(key, "consumer1", 2);
        this.XPendingRead(key, "consumer2", 3);

        var consumer1 = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10,
                consumer: "consumer1"));

        var consumer2 = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10,
                consumer: "consumer2"));

        XPendingAssertIds(consumer1, "1-0", "2-0");
        XPendingAssertIds(consumer2, "3-0", "4-0", "5-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(consumer1!.All(
                    x => x.Consumer == "consumer1"), Is.True);

            Assert.That(consumer2!.All(
                x => x.Consumer == "consumer2"), Is.True);
        }
    }

    [Test]
    public void XPending_Detail_ConsumerNotExists()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var entries = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10,
                consumer: "consumer_not_exists"));

        XPendingAssertIds(entries);
    }

    [Test]
    public void XPending_Detail_MinIdleTime_Zero()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var entries = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10,
                minIdleTime: 0));

        XPendingAssertIds(entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XPending_Detail_MinIdleTime_NotReached()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var entries = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10,
                minIdleTime: 3_600_000));

        XPendingAssertIds(entries);
    }

    [Test]
    public void XPending_Detail_MinIdleTime_AndConsumer()
    {
        var key = XPendingKey();
        this.XPendingSeed(key);

        this.XPendingRead(key, "consumer1", 2);
        this.XPendingRead(key, "consumer2", 3);

        var entries = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10,
                minIdleTime: 0,
                consumer: "consumer2"));

        XPendingAssertIds(entries,
            "3-0", "4-0", "5-0");
    }

    [Test]
    public void XPending_Detail_AfterAck()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(
                    key, "group1", "2-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XAck(
                key, "group1", "4-0"), Is.EqualTo(1));
        }

        var entries = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10));

        XPendingAssertIds(entries, "1-0", "3-0", "5-0");
    }

    [Test]
    public void XPending_Detail_AfterXDel()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        Assert.That(this._redis.Stream.XDel(
            key, "3-0"), Is.EqualTo(1));

        // XDEL 删除消息内容，但默认不清理 PEL
        var entries = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10));

        XPendingAssertIds(entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XPending_Detail_AutoClaim_JustId()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var before = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10));

        XPendingAssertDelivery(before![0],
            "1-0", "consumer1", 1);

        var claimed = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "0-0",
            count: 1,
            justid: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(claimed.Entries?.Length, Is.EqualTo(1));
            Assert.That(claimed.Entries![0].Id, Is.EqualTo("1-0"));
            Assert.That(claimed.Entries[0].FieldValue, Is.Null);
        }

        var after = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("1-0", "1-0", 1));

        XPendingAssertIds(after, "1-0");

        // 所有权改变，但投递次数没有增加
        XPendingAssertDelivery(after![0],
            "1-0", "consumer2", 1);
    }

    [Test]
    public void XPending_Detail_AutoClaim_Normal()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        var before = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("1-0", "1-0", 1));

        XPendingAssertDelivery(before![0],
            "1-0", "consumer1", 1);

        var claimed = this._redis.Stream.XAutoClaim(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "0-0",
            count: 1,
            justid: false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(claimed.Entries?.Length, Is.EqualTo(1));
            Assert.That(claimed.Entries![0].Id, Is.EqualTo("1-0"));
            Assert.That(claimed.Entries[0].FieldValue,
                Is.Not.Null.And.Not.Empty);
        }

        var after = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("1-0", "1-0", 1));

        // 普通认领增加投递次数
        XPendingAssertDelivery(after![0],
            "1-0", "consumer2", 2);
    }

    [Test]
    public void XPending_Options_Default()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        Assert.That(
            () => this._redis.Stream.XPending(
                key, "group1", default(PendingOptions)),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void XPending_Options_InvalidCount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PendingOptions("-", "+", 0));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PendingOptions("-", "+", -1));
    }

    [Test]
    public void XPending_Flags_None()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        Assert.That(this._redis.Stream.XPending(
            key, "group1",
            flags: CommandFlags.None).Count,
            Is.EqualTo(5));

        XPendingAssertIds(this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10),
            flags: CommandFlags.None),
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XPending_CancellationToken_Default()
    {
        var key = XPendingKey();
        this.XPendingPrepare(key);

        Assert.That(this._redis.Stream.XPending(
            key, "group1",
            cancellationToken: CancellationToken.None).Count,
            Is.EqualTo(5));

        XPendingAssertIds(this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10),
            cancellationToken: CancellationToken.None),
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XPending_GroupNotExists()
    {
        var key = XPendingKey();
        this.XPendingSeed(key);

        Assert.That(
            () => this._redis.Stream.XPending(
                key, "not_exists"),
            Throws.TypeOf<RedisErrorException>());

        Assert.That(
            () => this._redis.Stream.XPending(
                key, "not_exists",
                new PendingOptions("-", "+", 10)),
            Throws.TypeOf<RedisErrorException>());
    }

    [Test]
    public void XPending_WrongKeyType()
    {
        var key = XPendingKey();
        this._redis.String.Set(key, "hello");

        Assert.That(
            () => this._redis.Stream.XPending(
                key, "group1"),
            Throws.TypeOf<RedisErrorException>());

        Assert.That(
            () => this._redis.Stream.XPending(
                key, "group1",
                new PendingOptions("-", "+", 10)),
            Throws.TypeOf<RedisErrorException>());
    }

    [Test]
    public void XPending_Detail_ReturnNull()
    {
        var key = XPendingKey();
        this.XPendingSeed(key);

        // 已创建 Stream 和消费者组，但尚未读取消息
        // 因此 PEL 中没有任何待处理消息
        var result = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions("-", "+", 10));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Null);

            // 验证摘要模式确实没有待处理消息
            Assert.That(this._redis.Stream.XPending(
                key, "group1").Count, Is.Zero);
        }
    }
    #endregion

    #region XCfgSet

    private static string XCfgSetKey() =>
        $"Stream:{{XCfgSet}}:{Guid.NewGuid():N}";

    private void XCfgSetPrepare(string key)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(
                    key, "1-0", "field", "value"),
                    Is.EqualTo("1-0"));

            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    private static void XCfgSetRedisError<TError>(Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XCfgSet_Default()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        Assert.That(this._redis.Stream.XCfgSet(key), Is.True);
    }

    [Test]
    public void XCfgSet_DurationOnly()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        Assert.That(this._redis.Stream.XCfgSet(
            key, duration: 300, maxsize: null), Is.True);
    }

    [Test]
    public void XCfgSet_MaxSizeOnly()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        Assert.That(this._redis.Stream.XCfgSet(
            key, duration: null, maxsize: 500), Is.True);
    }

    [Test]
    public void XCfgSet_BothParameters()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        Assert.That(this._redis.Stream.XCfgSet(
            key, duration: 600, maxsize: 1000), Is.True);
    }

    [Test]
    public void XCfgSet_Duration_Min()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        Assert.That(this._redis.Stream.XCfgSet(
            key, duration: 1, maxsize: null), Is.True);
    }

    [Test]
    public void XCfgSet_Duration_Max()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        Assert.That(this._redis.Stream.XCfgSet(
            key, duration: 86400, maxsize: null), Is.True);
    }

    [Test]
    public void XCfgSet_MaxSize_Min()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        Assert.That(this._redis.Stream.XCfgSet(
            key, duration: null, maxsize: 1), Is.True);
    }

    [Test]
    public void XCfgSet_MaxSize_Max()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        Assert.That(this._redis.Stream.XCfgSet(
            key, duration: null, maxsize: 10000), Is.True);
    }

    [Test]
    public void XCfgSet_Duration_Zero()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        XCfgSetRedisError<ArgumentOutOfRangeException>(() => this._redis.Stream.XCfgSet(
            key, duration: 0, maxsize: null));
    }

    [Test]
    public void XCfgSet_Duration_Negative()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        XCfgSetRedisError<ArgumentOutOfRangeException>(() => this._redis.Stream.XCfgSet(
            key, duration: -1, maxsize: null));
    }

    [Test]
    public void XCfgSet_Duration_Overflow()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        XCfgSetRedisError<ArgumentOutOfRangeException>(() => this._redis.Stream.XCfgSet(
            key, duration: 86401, maxsize: null));
    }

    [Test]
    public void XCfgSet_MaxSize_Zero()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        XCfgSetRedisError<ArgumentOutOfRangeException>(() => this._redis.Stream.XCfgSet(
            key, duration: null, maxsize: 0));
    }

    [Test]
    public void XCfgSet_MaxSize_Negative()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        XCfgSetRedisError<ArgumentOutOfRangeException>(() => this._redis.Stream.XCfgSet(
            key, duration: null, maxsize: -1));
    }

    [Test]
    public void XCfgSet_MaxSize_Overflow()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        XCfgSetRedisError<ArgumentOutOfRangeException>(() => this._redis.Stream.XCfgSet(
            key, duration: null, maxsize: 10001));
    }

    [Test]
    public void XCfgSet_BothNull()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        // 至少必须指定一个配置参数
        XCfgSetRedisError<ArgumentException>(() => this._redis.Stream.XCfgSet(
            key, duration: null, maxsize: null));
    }

    [Test]
    public void XCfgSet_KeyNotExists()
    {
        var key = XCfgSetKey();

        // Stream 不存在时不能执行 XCFGSET
        XCfgSetRedisError<RedisErrorException>(() => this._redis.Stream.XCfgSet(
            key, duration: 300, maxsize: 500));
    }

    [Test]
    public void XCfgSet_WrongKeyType()
    {
        var key = XCfgSetKey();

        this._redis.String.Set(key, "hello");

        XCfgSetRedisError<RedisErrorException>(() => this._redis.Stream.XCfgSet(
            key, duration: 300, maxsize: 500));
    }

    [Test]
    public void XCfgSet_UpdateConfiguration()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XCfgSet(
                    key, duration: 100, maxsize: 100), Is.True);

            Assert.That(this._redis.Stream.XCfgSet(
                key, duration: 300, maxsize: 500), Is.True);

            Assert.That(this._redis.Stream.XCfgSet(
                key, duration: 600, maxsize: 1000), Is.True);

            // 修改配置不会删除 Stream 消息
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XCfgSet_Flags_None()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        Assert.That(this._redis.Stream.XCfgSet(
            key, duration: 300, maxsize: 500,
            flags: CommandFlags.None), Is.True);
    }

    [Test]
    public void XCfgSet_CancellationToken_Default()
    {
        var key = XCfgSetKey();
        this.XCfgSetPrepare(key);

        Assert.That(this._redis.Stream.XCfgSet(
            key, duration: 300, maxsize: 500,
            cancellationToken: CancellationToken.None), Is.True);
    }

    #endregion

    #region XClaim

    private static string XClaimKey() => $"Stream:{{XClaim}}:{Guid.NewGuid():N}";

    private void XClaimSeed(string key, int count = 5, int delivered = 5)
    {
        for (int i = 1; i <= count; i++)
            Assert.That(this._redis.Stream.XAdd(key, $"{i}-0", "field", $"value{i}"), Is.EqualTo($"{i}-0"));

        Assert.That(this._redis.Stream.XGroupCreate(key, "group1", "0-0"), Is.True);
        if (delivered > 0)
        {
            var read = this._redis.Stream.XReadGroup(key, ">", "group1", "consumer1", count: delivered);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(read.HasValue, Is.True);
                Assert.That(read!.Value.Entries, Has.Length.EqualTo(delivered));
            }
        }
        Assert.That(this._redis.Stream.XPending(key, "group1").Count, Is.EqualTo(delivered));
    }

    private XPendingItem XClaimPending(string key, string id)
    {
        var entries = this._redis.Stream.XPending(key, "group1", new PendingOptions(id, id, 1));
        Assert.That(entries, Is.Not.Null.And.Length.EqualTo(1));
        return entries![0];
    }

    private static void XClaimAssertIds(EntryItem[]? entries, params string[] ids)
    {
        Assert.That(entries?.Length ?? 0, Is.EqualTo(ids.Length));
        for (int i = 0; i < ids.Length; i++)
            Assert.That(entries![i].Id, Is.EqualTo(ids[i]));
    }

    private static void XClaimAssertOwner(XPendingItem pending, string id, string consumer, long deliveryCount)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pending.Id, Is.EqualTo(id));
            Assert.That(pending.Consumer, Is.EqualTo(consumer));
            Assert.That(pending.DeliveryCount, Is.EqualTo(deliveryCount));
            Assert.That(pending.IdleTime, Is.GreaterThanOrEqualTo(0));
        }
    }

    private static void XClaimRedisError<TError>(Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XClaim_Single_Normal()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "1-0");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Id, Is.EqualTo("1-0"));
            Assert.That(result.Value.FieldValue, Has.Length.EqualTo(1));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value.FieldValue![0].Key.ToString(), Is.EqualTo("field"));
            Assert.That(result.Value.FieldValue[0].Value.ToString(), Is.EqualTo("value1"));
        }
        XClaimAssertOwner(this.XClaimPending(key, "1-0"), "1-0", "consumer2", 2);
    }

    [Test]
    public void XClaim_Single_JustId()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "1-0", justid: true);
        Assert.That(result.HasValue, Is.True);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.Value.Id, Is.EqualTo("1-0"));
            Assert.That(result.Value.FieldValue, Is.Null);
            Assert.That(result.Value.IdleTime, Is.Null);
            Assert.That(result.Value.DeliveryCount, Is.Null);
        }
        XClaimAssertOwner(this.XClaimPending(key, "1-0"), "1-0", "consumer2", 1);
    }

    [Test]
    public void XClaim_Single_MinIdleTime_NotReached()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 86_400_000, "1-0");
        Assert.That(!result.HasValue || string.IsNullOrEmpty(result.Value.Id), Is.True);
        XClaimAssertOwner(this.XClaimPending(key, "1-0"), "1-0", "consumer1", 1);
    }

    [Test]
    public void XClaim_Single_NotPending()
    {
        var key = XClaimKey();
        this.XClaimSeed(key, delivered: 2);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "5-0");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(!result.HasValue || string.IsNullOrEmpty(result.Value.Id), Is.True);
            Assert.That(this._redis.Stream.XPending(key, "group1").Count, Is.EqualTo(2));
        }
    }

    [Test]
    public void XClaim_Single_NotExists()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "100-0");
        Assert.That(!result.HasValue || string.IsNullOrEmpty(result.Value.Id), Is.True);
    }

    [Test]
    public void XClaim_Single_Force()
    {
        var key = XClaimKey();
        this.XClaimSeed(key, delivered: 2);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "5-0", force: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Id, Is.EqualTo("5-0"));
            Assert.That(this._redis.Stream.XPending(key, "group1").Count, Is.EqualTo(3));
            Assert.That(this.XClaimPending(key, "5-0").Consumer, Is.EqualTo("consumer2"));
        }
    }

    [Test]
    public void XClaim_Single_Force_NotExistingId()
    {
        var key = XClaimKey();
        this.XClaimSeed(key, delivered: 2);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "100-0", force: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(!result.HasValue || string.IsNullOrEmpty(result.Value.Id), Is.True);
            Assert.That(this._redis.Stream.XPending(key, "group1").Count, Is.EqualTo(2));
        }
    }

    [Test]
    public void XClaim_Single_AfterXDel()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        Assert.That(this._redis.Stream.XDel(key, "2-0"), Is.EqualTo(1));
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "2-0", force: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(!result.HasValue || string.IsNullOrEmpty(result.Value.Id), Is.True);
            // Redis 7.0+：认领时发现原消息已删除，则清理该 ID 对应的 PEL。
            Assert.That(this._redis.Stream.XPending(key, "group1", new PendingOptions("2-0", "2-0", 1))?.Length ?? 0, Is.Zero);
        }
    }

    [Test]
    public void XClaim_Collection_Multiple()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        ICollection<EntryId> ids = ["1-0", "3-0", "5-0"];
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, ids);
        XClaimAssertIds(result, "1-0", "3-0", "5-0");
        foreach (var id in new[] { "1-0", "3-0", "5-0" })
            XClaimAssertOwner(this.XClaimPending(key, id), id, "consumer2", 2);
    }

    [Test]
    public void XClaim_Collection_Mixed()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        ICollection<EntryId> ids = ["1-0", "100-0", "3-0"];
        XClaimAssertIds(this._redis.Stream.XClaim(key, "group1", "consumer2", 0, ids), "1-0", "3-0");
    }

    [Test]
    public void XClaim_Collection_JustId()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        ICollection<EntryId> ids = ["1-0", "2-0"];
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, ids, justid: true);
        XClaimAssertIds(result, "1-0", "2-0");
        Assert.That(result!.All(x => x.FieldValue is null), Is.True);
        XClaimAssertOwner(this.XClaimPending(key, "1-0"), "1-0", "consumer2", 1);
        XClaimAssertOwner(this.XClaimPending(key, "2-0"), "2-0", "consumer2", 1);
    }

    [Test]
    public void XClaim_Span_Multiple()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0,
            (ReadOnlySpan<EntryId>)["2-0", "4-0"]);
        XClaimAssertIds(result, "2-0", "4-0");
    }

    [Test]
    public void XClaim_Span_JustId()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0,
            (ReadOnlySpan<EntryId>)["2-0", "4-0"], justid: true);
        XClaimAssertIds(result, "2-0", "4-0");
        Assert.That(result!.All(x => x.FieldValue is null), Is.True);
    }

    [Test]
    public void XClaim_Collection_NoMatch()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        ICollection<EntryId> ids = ["100-0", "101-0"];
        XClaimAssertIds(this._redis.Stream.XClaim(key, "group1", "consumer2", 0, ids));
    }

    [Test]
    public void XClaim_Idle()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "1-0", idle: 20_000);
        Assert.That(result.HasValue, Is.True);
        var pending = this.XClaimPending(key, "1-0");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pending.Consumer, Is.EqualTo("consumer2"));
            Assert.That(pending.IdleTime, Is.GreaterThanOrEqualTo(10_000));
        }
    }

    [Test]
    public void XClaim_Time()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        ulong timestamp = (ulong)DateTimeOffset.UtcNow.AddSeconds(-30).ToUnixTimeMilliseconds();
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "1-0", time: timestamp);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(this.XClaimPending(key, "1-0").IdleTime, Is.GreaterThanOrEqualTo(20_000));
        }
    }

    [Test]
    public void XClaim_RetryCount()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "1-0", retryCount: 7);
        Assert.That(result.HasValue, Is.True);
        XClaimAssertOwner(this.XClaimPending(key, "1-0"), "1-0", "consumer2", 7);
    }

    [Test]
    public void XClaim_LastId()
    {
        var key = XClaimKey();
        this.XClaimSeed(key, delivered: 2);
        var result = this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "1-0", lastid: "5-0");
        Assert.That(result.HasValue, Is.True);
        // LASTID 将组的 last-delivered-id 设置为 5-0，故 > 不再获取 3-0、4-0、5-0。
        var unread = this._redis.Stream.XReadGroup(key, ">", "group1", "consumer3", count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(unread is null || unread.Value.Entries is null || unread.Value.Entries.Length == 0, Is.True);
            Assert.That(this._redis.Stream.XPending(key, "group1").Count, Is.EqualTo(2));
        }
    }

    [Test]
    public void XClaim_Flags_None()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        Assert.That(this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "1-0", flags: CommandFlags.None)?.Id,
            Is.EqualTo("1-0"));
    }

    [Test]
    public void XClaim_CancellationToken_None()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        Assert.That(this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "1-0", cancellationToken: CancellationToken.None)?.Id,
            Is.EqualTo("1-0"));
    }

    [Test]
    public void XClaim_GroupNotExists()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);
        XClaimRedisError<RedisErrorException>(() => this._redis.Stream.XClaim(key, "missing", "consumer2", 0, "1-0"));
    }

    [Test]
    public void XClaim_KeyNotExists()
    {
        var key = XClaimKey();
        XClaimRedisError<RedisErrorException>(() => this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "1-0"));
    }

    [Test]
    public void XClaim_WrongKeyType()
    {
        var key = XClaimKey();
        this._redis.String.Set(key, "hello");
        XClaimRedisError<RedisErrorException>(() => this._redis.Stream.XClaim(key, "group1", "consumer2", 0, "1-0"));
    }

    private void XClaimAssertTime(
        string key,
        string id,
        string consumer,
        long minIdle = 3_000,
        long maxIdle = 60_000)
    {
        var entries = this._redis.Stream.XPending(
            key, "group1",
            new PendingOptions(id, id, 1));

        Assert.That(entries, Is.Not.Null);
        Assert.That(entries, Has.Length.EqualTo(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(entries![0].Id, Is.EqualTo(id));
            Assert.That(entries[0].Consumer, Is.EqualTo(consumer));
            Assert.That(entries[0].IdleTime,
                Is.InRange(minIdle, maxIdle));
            Assert.That(entries[0].DeliveryCount, Is.EqualTo(2));
        }
    }

    [Test]
    public void XClaim_Time_TimeSpan()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);

        // 将上次投递时间设置为当前时间之前 8 秒
        var result = this._redis.Stream.XClaim(
            key, "group1", "consumer2", 0, "1-0",
            time: TimeSpan.FromSeconds(-8));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Id, Is.EqualTo("1-0"));
        }

        this.XClaimAssertTime(key, "1-0", "consumer2");
    }

    [Test]
    public void XClaim_Time_DateTime()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);

        DateTime time = DateTime.UtcNow.AddSeconds(-8);

        var result = this._redis.Stream.XClaim(
            key, "group1", "consumer2", 0, "1-0",
            time: time);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Id, Is.EqualTo("1-0"));
        }

        this.XClaimAssertTime(key, "1-0", "consumer2");
    }

    [Test]
    public void XClaim_Time_DateTimeOffset()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);

        DateTimeOffset time = DateTimeOffset.UtcNow.AddSeconds(-8);

        var result = this._redis.Stream.XClaim(
            key, "group1", "consumer2", 0, "1-0",
            time: time);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Id, Is.EqualTo("1-0"));
        }

        this.XClaimAssertTime(key, "1-0", "consumer2");
    }

    [Test]
    public void XClaim_Time_SharpExpiry()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);

        var time = new SharpExpiry(
            DateTimeOffset.UtcNow.AddSeconds(-8));

        var result = this._redis.Stream.XClaim(
            key, "group1", "consumer2", 0, "1-0",
            time: time);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Id, Is.EqualTo("1-0"));
        }

        this.XClaimAssertTime(key, "1-0", "consumer2");
    }

    [Test]
    public void XClaim_Time_Collection()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);

        ICollection<EntryId> ids = ["1-0", "2-0"];

        var result = this._redis.Stream.XClaim(
            key, "group1", "consumer2", 0, ids,
            time: DateTimeOffset.UtcNow.AddSeconds(-8));

        Assert.That(result, Has.Length.EqualTo(2));

        this.XClaimAssertTime(key, "1-0", "consumer2");
        this.XClaimAssertTime(key, "2-0", "consumer2");
    }

    [Test]
    public void XClaim_Time_Span()
    {
        var key = XClaimKey();
        this.XClaimSeed(key);

        var result = this._redis.Stream.XClaim(
            key, "group1", "consumer2", 0,
            ["1-0", "2-0"],
            time: DateTime.UtcNow.AddSeconds(-8));

        Assert.That(result, Has.Length.EqualTo(2));

        this.XClaimAssertTime(key, "1-0", "consumer2");
        this.XClaimAssertTime(key, "2-0", "consumer2");
    }

    #endregion

    #region XDelEx

    private static string XDelExKey() =>
        $"Stream:{{XDelEx}}:{Guid.NewGuid():N}";

    private static void XDelExRedisError<TError>(Action action)
        where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    private void XDelExSeed(string key)
    {
        for (int i = 1; i <= 5; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    private void XDelExCreateGroup(string key, string group)
    {
        Assert.That(this._redis.Stream.XGroupCreate(
            key, group, "0-0"), Is.True);
    }

    private void XDelExRead(string key, string group, string consumer)
    {
        var read = this._redis.Stream.XReadGroup(
            key, ">", group, consumer, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(read!.Value.Entries, Has.Length.EqualTo(5));
        }
    }

    private void XDelExAssertPending(string key, string group, long expected)
    {
        Assert.That(this._redis.Stream.XPending(key, group).Count,
            Is.EqualTo(expected));
    }

    [Test]
    public void XDelEx_Single_Default()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDelEx(key, "1-0"),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XDelEx_Single_NotFound()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDelEx(key, "99-0"),
                    Is.EqualTo(DeleteResult.NotFound));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XDelEx_Single_Repeat()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);

        Assert.That(this._redis.Stream.XDelEx(key, "1-0"),
            Is.EqualTo(DeleteResult.Deleted));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDelEx(key, "1-0"),
                    Is.EqualTo(DeleteResult.NotFound));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XDelEx_Collection_All()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        ICollection<EntryId> ids = ["1-0", "2-0", "3-0"];

        var result = this._redis.Stream.XDelEx(key, ids);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                DeleteResult.Deleted, DeleteResult.Deleted, DeleteResult.Deleted
            ]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(2));
        }
    }

    [Test]
    public void XDelEx_Collection_Mixed()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        ICollection<EntryId> ids = ["1-0", "99-0", "3-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDelEx(key, ids), Is.EqualTo(
            [
                DeleteResult.Deleted,
                DeleteResult.NotFound,
                DeleteResult.Deleted
            ]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }
    }

    [Test]
    public void XDelEx_Span_All()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);

        var result = this._redis.Stream.XDelEx(
            key, (ReadOnlySpan<EntryId>)["1-0", "2-0", "3-0"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                DeleteResult.Deleted, DeleteResult.Deleted, DeleteResult.Deleted
            ]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(2));
        }
    }

    [Test]
    public void XDelEx_Span_Mixed()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);

        var result = this._redis.Stream.XDelEx(
            key, (ReadOnlySpan<EntryId>)["2-0", "99-0", "4-0"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                DeleteResult.Deleted, DeleteResult.NotFound, DeleteResult.Deleted
            ]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }
    }

    [Test]
    public void XDelEx_KeyNotExists()
    {
        var key = XDelExKey();

        Assert.That(this._redis.Stream.XDelEx(key, "1-0"),
            Is.EqualTo(DeleteResult.NotFound));
        ICollection<EntryId> ids = ["1-0", "2-0"];
        Assert.That(this._redis.Stream.XDelEx(key, ids),
            Is.EqualTo([DeleteResult.NotFound, DeleteResult.NotFound]));
    }

    [Test]
    public void XDelEx_KeepRef_Default()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");

        Assert.That(this._redis.Stream.XDelEx(key, "1-0"),
            Is.EqualTo(DeleteResult.Deleted));
        this.XDelExAssertPending(key, "group1", 5);
        Assert.That(this._redis.Stream.XAck(key, "group1", "1-0"),
            Is.EqualTo(1));
    }

    [Test]
    public void XDelEx_KeepRef_Explicit()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");

        Assert.That(this._redis.Stream.XDelEx(
            key, "1-0", delPolicy: DeletionPolicy.KeepRef),
            Is.EqualTo(DeleteResult.Deleted));
        this.XDelExAssertPending(key, "group1", 5);
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
    }

    [Test]
    public void XDelEx_DelRef_TwoGroups()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExCreateGroup(key, "group2");
        this.XDelExRead(key, "group1", "consumer1");
        this.XDelExRead(key, "group2", "consumer2");

        Assert.That(this._redis.Stream.XDelEx(
            key, "1-0", delPolicy: DeletionPolicy.DelRef),
            Is.EqualTo(DeleteResult.Deleted));
        this.XDelExAssertPending(key, "group1", 4);
        this.XDelExAssertPending(key, "group2", 4);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(key, "group1", "1-0"), Is.Zero);
            Assert.That(this._redis.Stream.XAck(key, "group2", "1-0"), Is.Zero);
        }
    }

    [Test]
    public void XDelEx_DelRef_DanglingPending()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");

        Assert.That(this._redis.Stream.XDel(key, "1-0"), Is.EqualTo(1));
        this.XDelExAssertPending(key, "group1", 5);
        // 已没有 Stream 消息，但 DELREF 仍须清理悬挂的 PEL 引用。
        var result = this._redis.Stream.XDelEx(
            key, "1-0", delPolicy: DeletionPolicy.DelRef);
        Assert.That(result, Is.EqualTo(DeleteResult.NotFound));
        this.XDelExAssertPending(key, "group1", 4);
        Assert.That(this._redis.Stream.XAck(key, "group1", "1-0"), Is.Zero);
    }

    [Test]
    public void XDelEx_Acked_NoGroups()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDelEx(
                    key, "1-0", delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XDelEx_Acked_NotRead()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDelEx(
                    key, "1-0", delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo(DeleteResult.StillReferenced));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XDelEx_Acked_Pending()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");

        Assert.That(this._redis.Stream.XDelEx(
            key, "1-0", delPolicy: DeletionPolicy.Acked),
            Is.EqualTo(DeleteResult.StillReferenced));
        this.XDelExAssertPending(key, "group1", 5);
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    [Test]
    public void XDelEx_Acked_AfterAck()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XDelEx(
                key, "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XDelEx_Acked_TwoGroups_PartialAck()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExCreateGroup(key, "group2");
        this.XDelExRead(key, "group1", "consumer1");
        this.XDelExRead(key, "group2", "consumer2");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XDelEx(
                key, "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.StillReferenced));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
        this.XDelExAssertPending(key, "group2", 5);
    }

    [Test]
    public void XDelEx_Acked_TwoGroups_AllAck()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExCreateGroup(key, "group2");
        this.XDelExRead(key, "group1", "consumer1");
        this.XDelExRead(key, "group2", "consumer2");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(key, "group1", "1-0"), Is.EqualTo(1));
            Assert.That(this._redis.Stream.XAck(key, "group2", "1-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XDelEx(
                key, "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.Deleted));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XDelEx_Acked_Collection_Mixed()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");
        Assert.That(this._redis.Stream.XAck(key, "group1", "1-0"), Is.EqualTo(1));
        ICollection<EntryId> ids = ["1-0", "2-0", "0-1"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDelEx(
                    key, ids, delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo(
                    [
                        DeleteResult.Deleted,
                        DeleteResult.StillReferenced,
                        DeleteResult.NotFound
                    ]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XDelEx_Acked_Span_Mixed()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");
        Assert.That(this._redis.Stream.XAck(key, "group1", "3-0"), Is.EqualTo(1));

        var result = this._redis.Stream.XDelEx(
            key, ["2-0", "3-0", "0-1"],
            delPolicy: DeletionPolicy.Acked);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                DeleteResult.StillReferenced,
                DeleteResult.Deleted,
                DeleteResult.NotFound
            ]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }
    }

    [Test]
    public void XDelEx_Flags_None()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);

        Assert.That(this._redis.Stream.XDelEx(
            key, "1-0", flags: CommandFlags.None),
            Is.EqualTo(DeleteResult.Deleted));
    }

    [Test]
    public void XDelEx_CancellationToken_None()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);

        Assert.That(this._redis.Stream.XDelEx(
            key, "1-0", cancellationToken: CancellationToken.None),
            Is.EqualTo(DeleteResult.Deleted));
    }

    [Test]
    public void XDelEx_WrongKeyType()
    {
        var key = XDelExKey();
        this._redis.String.Set(key, "hello");

        XDelExRedisError<RedisErrorException>(() =>
            this._redis.Stream.XDelEx(key, "1-0"));
    }

    [Test]
    public void XDelEx_InvalidId()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);

        XDelExRedisError<ArgumentException>(() =>
            this._redis.Stream.XDelEx(key, "18446744073709551616-0"));
    }

    [Test]
    public void XDelEx_Collection_Empty()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        ICollection<EntryId> ids = [];

        XDelExRedisError<NotSupportedException>(() =>
            this._redis.Stream.XDelEx(key, ids));
    }

    [Test]
    public void XDelEx_Span_Empty()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);

        XDelExRedisError<NotSupportedException>(() =>
            this._redis.Stream.XDelEx(key, (ReadOnlySpan<EntryId>)[]));
    }


    [Test]
    public void XDelEx_Acked_NonexistentAfterLastDelivered()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            // 99-0 不在 Stream 中，但超过 group1 的 last-delivered-id (5-0)。
            Assert.That(this._redis.Stream.XDelEx(
                key, "99-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.StillReferenced));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XDelEx_Acked_NonexistentBeforeLastDelivered()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            // 0-1 不在 Stream 中，而且早于消费组的 last-delivered-id。
            Assert.That(this._redis.Stream.XDelEx(
                key, "0-1", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.NotFound));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XDelEx_Acked_NoGroups_NotFound()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);

        using (Assert.EnterMultipleScope())
        {
            // 无组时，不存在的 ID 仍返回 -1。
            Assert.That(this._redis.Stream.XDelEx(
                key, "99-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.NotFound));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XDelEx_Acked_TwoGroups_OneNotRead()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExCreateGroup(key, "group2");
        this.XDelExRead(key, "group1", "consumer1");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(
                    key, "group1", "1-0"), Is.EqualTo(1));

            // group2 尚未读过消息，所以即使 group1 已 ACK，也不能删除。
            Assert.That(this._redis.Stream.XDelEx(
                key, "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.StillReferenced));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XDelEx_KeepRef_TwoGroups()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExCreateGroup(key, "group2");
        this.XDelExRead(key, "group1", "consumer1");
        this.XDelExRead(key, "group2", "consumer2");

        Assert.That(this._redis.Stream.XDelEx(
            key, "1-0", delPolicy: DeletionPolicy.KeepRef),
            Is.EqualTo(DeleteResult.Deleted));
        this.XDelExAssertPending(key, "group1", 5);
        this.XDelExAssertPending(key, "group2", 5);
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
    }

    [Test]
    public void XDelEx_DelRef_Collection_Mixed()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");
        ICollection<EntryId> ids = ["1-0", "0-1", "3-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDelEx(
                    key, ids, delPolicy: DeletionPolicy.DelRef),
                    Is.EqualTo(
                    [
                        DeleteResult.Deleted,
                        DeleteResult.NotFound,
                        DeleteResult.Deleted
                    ]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }
        this.XDelExAssertPending(key, "group1", 3);
    }

    [Test]
    public void XDelEx_DelRef_Span_Mixed()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDelEx(
                    key, (ReadOnlySpan<EntryId>)["2-0", "0-1", "4-0"],
                    delPolicy: DeletionPolicy.DelRef),
                    Is.EqualTo(
                    [
                        DeleteResult.Deleted,
                        DeleteResult.NotFound,
                        DeleteResult.Deleted
                    ]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }
        this.XDelExAssertPending(key, "group1", 3);
    }

    [Test]
    public void XDelEx_Acked_Collection_AllAcknowledged()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(
                    key, "group1", "1-0"), Is.EqualTo(1));
            Assert.That(this._redis.Stream.XAck(
                key, "group1", "2-0"), Is.EqualTo(1));
        }
        ICollection<EntryId> ids = ["1-0", "2-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDelEx(
                    key, ids, delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo([DeleteResult.Deleted, DeleteResult.Deleted]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }
        this.XDelExAssertPending(key, "group1", 3);
    }

    [Test]
    public void XDelEx_Acked_Span_AllPending()
    {
        var key = XDelExKey();
        this.XDelExSeed(key);
        this.XDelExCreateGroup(key, "group1");
        this.XDelExRead(key, "group1", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDelEx(
                    key, (ReadOnlySpan<EntryId>)["1-0", "2-0"],
                    delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo(
                    [
                        DeleteResult.StillReferenced,
                        DeleteResult.StillReferenced
                    ]));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
        this.XDelExAssertPending(key, "group1", 5);
    }

    #endregion

    #region XGroupCreateConsumer

    private static string XGroupCreateConsumerKey() =>
        $"Stream:{{XGroupCreateConsumer}}:{Guid.NewGuid():N}";

    private void XGroupCreateConsumerPrepare(string key)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(
                    key, "1-0", "field", "value"),
                    Is.EqualTo("1-0"));

            Assert.That(this._redis.Stream.XGroupCreate(
                key, "group1", "0-0"), Is.True);
        }
    }

    private static void XGroupCreateConsumerRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XGroupCreateConsumer_NewConsumer()
    {
        var key = XGroupCreateConsumerKey();
        this.XGroupCreateConsumerPrepare(key);

        var result = this._redis.Stream.XGroupCreateConsumer(
            key, "group1", "consumer1");

        Assert.That(result, Is.True);
    }

    [Test]
    public void XGroupCreateConsumer_ExistingConsumer()
    {
        var key = XGroupCreateConsumerKey();
        this.XGroupCreateConsumerPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreateConsumer(
            key, "group1", "consumer1"), Is.True);

        Assert.That(this._redis.Stream.XGroupCreateConsumer(
            key, "group1", "consumer1"), Is.False);
    }

    [Test]
    public void XGroupCreateConsumer_MultipleConsumers()
    {
        var key = XGroupCreateConsumerKey();
        this.XGroupCreateConsumerPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                    key, "group1", "consumer1"), Is.True);

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer2"), Is.True);

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer3"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                    key, "group1", "consumer1"), Is.False);

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer2"), Is.False);

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer3"), Is.False);
        }
    }

    [Test]
    public void XGroupCreateConsumer_DifferentGroups()
    {
        var key = XGroupCreateConsumerKey();
        this.XGroupCreateConsumerPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreate(
                    key, "group2", "0-0"), Is.True);

            // 不同消费者组允许存在相同名称的消费者
            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer1"), Is.True);

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group2", "consumer1"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                    key, "group1", "consumer1"), Is.False);

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group2", "consumer1"), Is.False);
        }
    }

    [Test]
    public void XGroupCreateConsumer_ExistingFromXReadGroup()
    {
        var key = XGroupCreateConsumerKey();
        this.XGroupCreateConsumerPrepare(key);

        // XREADGROUP 会自动创建 consumer1
        var entries = this._redis.Stream.XReadGroup(
            key, ">", "group1", "consumer1", count: 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(entries.HasValue, Is.True);

            // 已经由 XREADGROUP 创建，因此返回 false
            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer1"), Is.False);

            Assert.That(this._redis.Stream.XPending(
                key, "group1").Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void XGroupCreateConsumer_NoPendingEntries()
    {
        var key = XGroupCreateConsumerKey();
        this.XGroupCreateConsumerPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                    key, "group1", "consumer1"), Is.True);

            // 显式创建消费者，不会自动读取消息
            Assert.That(this._redis.Stream.XPending(
                key, "group1").Count, Is.Zero);

            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(1));
        }
    }

    [Test]
    public void XGroupCreateConsumer_StreamNotExists()
    {
        var key = XGroupCreateConsumerKey();

        XGroupCreateConsumerRedisError<RedisErrorException>(() =>
            this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer1"));
    }

    [Test]
    public void XGroupCreateConsumer_GroupNotExists()
    {
        var key = XGroupCreateConsumerKey();
        this.XGroupCreateConsumerPrepare(key);

        XGroupCreateConsumerRedisError<RedisErrorException>(() =>
            this._redis.Stream.XGroupCreateConsumer(
                key, "not_exists", "consumer1"));
    }

    [Test]
    public void XGroupCreateConsumer_WrongKeyType()
    {
        var key = XGroupCreateConsumerKey();

        this._redis.String.Set(key, "hello");

        XGroupCreateConsumerRedisError<RedisErrorException>(() =>
            this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer1"));
    }

    [Test]
    public void XGroupCreateConsumer_Flags_None()
    {
        var key = XGroupCreateConsumerKey();
        this.XGroupCreateConsumerPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreateConsumer(
            key, "group1", "consumer1",
            flags: CommandFlags.None), Is.True);
    }

    [Test]
    public void XGroupCreateConsumer_CancellationToken_Default()
    {
        var key = XGroupCreateConsumerKey();
        this.XGroupCreateConsumerPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreateConsumer(
            key, "group1", "consumer1",
            cancellationToken: CancellationToken.None), Is.True);
    }

    #endregion

    #region XGroupDelConsumer

    private static string XGroupDelConsumerKey() =>
        $"Stream:{{XGroupDelConsumer}}:{Guid.NewGuid():N}";

    private void XGroupDelConsumerPrepare(string key)
    {
        for (int i = 1; i <= 5; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(this._redis.Stream.XGroupCreate(
            key, "group1", "0-0"), Is.True);
    }

    private void XGroupDelConsumerRead(
        string key, string consumer, int count)
    {
        var result = this._redis.Stream.XReadGroup(
            key, ">", "group1", consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries, Has.Length.EqualTo(count));
        }
    }

    private void XGroupDelConsumerAssertPending(
        string key, long count)
    {
        Assert.That(this._redis.Stream.XPending(
            key, "group1").Count, Is.EqualTo(count));
    }

    private static void XGroupDelConsumerRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XGroupDelConsumer_Existing_NoPending()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                    key, "group1", "consumer1"), Is.True);

            // 存在消费者，但没有未确认消息
            Assert.That(this._redis.Stream.XGroupDelConsumer(
                key, "group1", "consumer1"), Is.Zero);
        }

        // 重新创建成功，证明原消费者已删除
        Assert.That(this._redis.Stream.XGroupCreateConsumer(
            key, "group1", "consumer1"), Is.True);

        this.XGroupDelConsumerAssertPending(key, 0);
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    [Test]
    public void XGroupDelConsumer_NotExists()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);

        Assert.That(this._redis.Stream.XGroupDelConsumer(
            key, "group1", "not_exists"), Is.Zero);

        this.XGroupDelConsumerAssertPending(key, 0);
    }

    [Test]
    public void XGroupDelConsumer_RepeatDelete()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                    key, "group1", "consumer1"), Is.True);

            Assert.That(this._redis.Stream.XGroupDelConsumer(
                key, "group1", "consumer1"), Is.Zero);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupDelConsumer(
                    key, "group1", "consumer1"), Is.Zero);

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer1"), Is.True);
        }
    }

    [Test]
    public void XGroupDelConsumer_OnePending()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);
        this.XGroupDelConsumerRead(key, "consumer1", 1);

        this.XGroupDelConsumerAssertPending(key, 1);

        Assert.That(this._redis.Stream.XGroupDelConsumer(
            key, "group1", "consumer1"), Is.EqualTo(1));

        this.XGroupDelConsumerAssertPending(key, 0);

        using (Assert.EnterMultipleScope())
        {
            // 删除消费者不会删除 Stream 中的消息
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer1"), Is.True);
        }
    }

    [Test]
    public void XGroupDelConsumer_MultiplePending()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);
        this.XGroupDelConsumerRead(key, "consumer1", 3);

        this.XGroupDelConsumerAssertPending(key, 3);

        Assert.That(this._redis.Stream.XGroupDelConsumer(
            key, "group1", "consumer1"), Is.EqualTo(3));

        this.XGroupDelConsumerAssertPending(key, 0);
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    [Test]
    public void XGroupDelConsumer_AfterAck()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);
        this.XGroupDelConsumerRead(key, "consumer1", 3);

        Assert.That(this._redis.Stream.XAck(
            key, "group1", "1-0"), Is.EqualTo(1));

        this.XGroupDelConsumerAssertPending(key, 2);

        // 返回未确认的数量，不包含已 ACK 的消息
        Assert.That(this._redis.Stream.XGroupDelConsumer(
            key, "group1", "consumer1"), Is.EqualTo(2));

        this.XGroupDelConsumerAssertPending(key, 0);
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    [Test]
    public void XGroupDelConsumer_AllAcknowledged()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);
        this.XGroupDelConsumerRead(key, "consumer1", 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(
                    key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XAck(
                key, "group1", "2-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XGroupDelConsumer(
                key, "group1", "consumer1"), Is.Zero);

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer1"), Is.True);
        }

        this.XGroupDelConsumerAssertPending(key, 0);
    }

    [Test]
    public void XGroupDelConsumer_MultipleConsumers()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);

        this.XGroupDelConsumerRead(key, "consumer1", 2);
        this.XGroupDelConsumerRead(key, "consumer2", 3);

        this.XGroupDelConsumerAssertPending(key, 5);

        Assert.That(this._redis.Stream.XGroupDelConsumer(
            key, "group1", "consumer1"), Is.EqualTo(2));

        // consumer2 的三条消息不受影响
        this.XGroupDelConsumerAssertPending(key, 3);

        var entries = this._redis.Stream.XPending(
            key, "group1", new PendingOptions("-", "+", 10));

        Assert.That(entries, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entries!.Select(x => x.Id),
                    Is.EqualTo(["3-0", "4-0", "5-0"]));

            Assert.That(entries.All(x => x.Consumer == "consumer2"),
                Is.True);

            Assert.That(this._redis.Stream.XGroupDelConsumer(
                key, "group1", "consumer2"), Is.EqualTo(3));
        }

        this.XGroupDelConsumerAssertPending(key, 0);
        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    [Test]
    public void XGroupDelConsumer_DifferentGroups()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreate(
            key, "group2", "0-0"), Is.True);

        this.XGroupDelConsumerRead(key, "consumer1", 2);

        var group2Read = this._redis.Stream.XReadGroup(
            key, ">", "group2", "consumer1", count: 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group2Read.HasValue, Is.True);

            Assert.That(this._redis.Stream.XGroupDelConsumer(
                key, "group1", "consumer1"), Is.EqualTo(2));

            Assert.That(this._redis.Stream.XPending(
                key, "group1").Count, Is.Zero);

            // group2 中的同名消费者和 PEL 保持不变
            Assert.That(this._redis.Stream.XPending(
                key, "group2").Count, Is.EqualTo(3));

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group2", "consumer1"), Is.False);
        }
    }

    [Test]
    public void XGroupDelConsumer_PendingCannotAckAfterDelete()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);
        this.XGroupDelConsumerRead(key, "consumer1", 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupDelConsumer(
                    key, "group1", "consumer1"), Is.EqualTo(2));

            // PEL 已被移除，XACK 不再找到对应记录
            Assert.That(this._redis.Stream.XAck(
                key, "group1", "1-0"), Is.Zero);

            Assert.That(this._redis.Stream.XAck(
                key, "group1", "2-0"), Is.Zero);
        }

        this.XGroupDelConsumerAssertPending(key, 0);
    }

    [Test]
    public void XGroupDelConsumer_CanReadRemainingMessages()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);
        this.XGroupDelConsumerRead(key, "consumer1", 2);

        Assert.That(this._redis.Stream.XGroupDelConsumer(
            key, "group1", "consumer1"), Is.EqualTo(2));

        // 原消费组投递进度没有倒退，其他消费者读取剩余消息
        this.XGroupDelConsumerRead(key, "consumer2", 3);

        this.XGroupDelConsumerAssertPending(key, 3);
    }

    [Test]
    public void XGroupDelConsumer_StreamNotExists()
    {
        var key = XGroupDelConsumerKey();

        XGroupDelConsumerRedisError<RedisErrorException>(() =>
            this._redis.Stream.XGroupDelConsumer(
                key, "group1", "consumer1"));
    }

    [Test]
    public void XGroupDelConsumer_GroupNotExists()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);

        XGroupDelConsumerRedisError<RedisErrorException>(() =>
            this._redis.Stream.XGroupDelConsumer(
                key, "not_exists", "consumer1"));
    }

    [Test]
    public void XGroupDelConsumer_WrongKeyType()
    {
        var key = XGroupDelConsumerKey();
        this._redis.String.Set(key, "hello");

        XGroupDelConsumerRedisError<RedisErrorException>(() =>
            this._redis.Stream.XGroupDelConsumer(
                key, "group1", "consumer1"));
    }

    [Test]
    public void XGroupDelConsumer_Flags_None()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);
        this.XGroupDelConsumerRead(key, "consumer1", 1);

        Assert.That(this._redis.Stream.XGroupDelConsumer(
            key, "group1", "consumer1",
            flags: CommandFlags.None), Is.EqualTo(1));
    }

    [Test]
    public void XGroupDelConsumer_CancellationToken_Default()
    {
        var key = XGroupDelConsumerKey();
        this.XGroupDelConsumerPrepare(key);
        this.XGroupDelConsumerRead(key, "consumer1", 1);

        Assert.That(this._redis.Stream.XGroupDelConsumer(
            key, "group1", "consumer1",
            cancellationToken: CancellationToken.None),
            Is.EqualTo(1));
    }

    #endregion

    #region XGroupDestroy

    private static string XGroupDestroyKey() =>
        $"Stream:{{XGroupDestroy}}:{Guid.NewGuid():N}";

    private void XGroupDestroyPrepare(string key, int count = 5)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(this._redis.Stream.XGroupCreate(
            key, "group1", "0-0"), Is.True);
    }

    private void XGroupDestroyRead(
        string key, string group, string consumer, int count)
    {
        var result = this._redis.Stream.XReadGroup(
            key, ">", group, consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries,
                Has.Length.EqualTo(count));
        }
    }

    private static void XGroupDestroyRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XGroupDestroy_Existing()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupDestroy(
                    key, "group1"), Is.True);

            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XGroupDestroy_NotExists()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupDestroy(
                    key, "not_exists"), Is.False);

            // 不影响原有的 group1
            Assert.That(this._redis.Stream.XGroupDestroy(
                key, "group1"), Is.True);
        }
    }

    [Test]
    public void XGroupDestroy_Repeat()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);

        Assert.That(this._redis.Stream.XGroupDestroy(
            key, "group1"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupDestroy(
                    key, "group1"), Is.False);

            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XGroupDestroy_WithPending()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);
        this.XGroupDestroyRead(key, "group1", "consumer1", 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XPending(
                    key, "group1").Count, Is.EqualTo(3));

            Assert.That(this._redis.Stream.XGroupDestroy(
                key, "group1"), Is.True);

            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }

        // 销毁后原消费组不能再查询 PEL
        XGroupDestroyRedisError<RedisErrorException>(() =>
            this._redis.Stream.XPending(key, "group1"));
    }

    [Test]
    public void XGroupDestroy_WithMultipleConsumers()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);

        this.XGroupDestroyRead(key, "group1", "consumer1", 2);
        this.XGroupDestroyRead(key, "group1", "consumer2", 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XPending(
                    key, "group1").Count, Is.EqualTo(5));

            Assert.That(this._redis.Stream.XGroupDestroy(
                key, "group1"), Is.True);
        }

        // 所有消费者及其 PEL 都随消费组销毁
        XGroupDestroyRedisError<RedisErrorException>(() =>
            this._redis.Stream.XPending(key, "group1"));

        Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
    }

    [Test]
    public void XGroupDestroy_WithoutPending()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XPending(
                    key, "group1").Count, Is.Zero);

            Assert.That(this._redis.Stream.XGroupDestroy(
                key, "group1"), Is.True);

            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XGroupDestroy_AfterAllAck()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);

        this.XGroupDestroyRead(key, "group1", "consumer1", 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(
                    key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XAck(
                key, "group1", "2-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XPending(
                key, "group1").Count, Is.Zero);

            Assert.That(this._redis.Stream.XGroupDestroy(
                key, "group1"), Is.True);
        }
    }

    [Test]
    public void XGroupDestroy_MultipleGroups()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreate(
            key, "group2", "0-0"), Is.True);

        this.XGroupDestroyRead(key, "group1", "consumer1", 2);
        this.XGroupDestroyRead(key, "group2", "consumer2", 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XPending(
                    key, "group1").Count, Is.EqualTo(2));

            Assert.That(this._redis.Stream.XPending(
                key, "group2").Count, Is.EqualTo(3));

            Assert.That(this._redis.Stream.XGroupDestroy(
                key, "group1"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            // group2 不受影响
            Assert.That(this._redis.Stream.XPending(
                key, "group2").Count, Is.EqualTo(3));

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group2", "consumer2"), Is.False);

            Assert.That(this._redis.Stream.XGroupDestroy(
                key, "group2"), Is.True);

            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XGroupDestroy_Recreate()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);

        this.XGroupDestroyRead(key, "group1", "consumer1", 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupDestroy(
                    key, "group1"), Is.True);

            // 同名消费组可以重新创建
            Assert.That(this._redis.Stream.XGroupCreate(
                key, "group1", "0-0"), Is.True);

            // 重新创建后不会继承原 PEL
            Assert.That(this._redis.Stream.XPending(
                key, "group1").Count, Is.Zero);
        }

        // 因为从 0-0 重新建立消费进度，所以可以重新读取消息
        this.XGroupDestroyRead(key, "group1", "consumer2", 5);

        Assert.That(this._redis.Stream.XPending(
            key, "group1").Count, Is.EqualTo(5));
    }

    [Test]
    public void XGroupDestroy_AfterDestroy_XReadGroup()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);

        Assert.That(this._redis.Stream.XGroupDestroy(
            key, "group1"), Is.True);

        XGroupDestroyRedisError<RedisErrorException>(() =>
            this._redis.Stream.XReadGroup(
                key, ">", "group1", "consumer1", count: 1));
    }

    [Test]
    public void XGroupDestroy_StreamNotExists()
    {
        var key = XGroupDestroyKey();

        XGroupDestroyRedisError<RedisErrorException>(() =>
            this._redis.Stream.XGroupDestroy(
                key, "group1"));
    }

    [Test]
    public void XGroupDestroy_WrongKeyType()
    {
        var key = XGroupDestroyKey();

        this._redis.String.Set(key, "hello");

        XGroupDestroyRedisError<RedisErrorException>(() =>
            this._redis.Stream.XGroupDestroy(
                key, "group1"));
    }

    [Test]
    public void XGroupDestroy_Flags_None()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);

        Assert.That(this._redis.Stream.XGroupDestroy(
            key, "group1",
            flags: CommandFlags.None), Is.True);
    }

    [Test]
    public void XGroupDestroy_CancellationToken_Default()
    {
        var key = XGroupDestroyKey();
        this.XGroupDestroyPrepare(key);

        Assert.That(this._redis.Stream.XGroupDestroy(
            key, "group1",
            cancellationToken: CancellationToken.None), Is.True);
    }

    #endregion

    #region XGroupSetID

    private static string XGroupSetIDKey() =>
        $"Stream:{{XGroupSetID}}:{Guid.NewGuid():N}";

    private void XGroupSetIDPrepare(string key)
    {
        for (int i = 1; i <= 5; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(this._redis.Stream.XGroupCreate(
            key, "group1", "0-0"), Is.True);
    }

    private void XGroupSetIDAssertRead(
        string key, string consumer, params string[] expectedIds)
    {
        var result = this._redis.Stream.XReadGroup(
            key, ">", "group1", consumer, count: 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries, Has.Length.EqualTo(expectedIds.Length));
        }

        for (int i = 0; i < expectedIds.Length; i++)
        {
            Assert.That(result.Value.Entries![i].Id,
                Is.EqualTo(expectedIds[i]));
        }
    }

    private static void XGroupSetIDRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XGroupSetID_Zero()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        Assert.That(this._redis.Stream.XGroupSetID(
            key, "group1", "0-0"), Is.True);

        this.XGroupSetIDAssertRead(
            key, "consumer1",
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XGroupSetID_MiddleID()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        Assert.That(this._redis.Stream.XGroupSetID(
            key, "group1", "3-0"), Is.True);

        this.XGroupSetIDAssertRead(
            key, "consumer1", "4-0", "5-0");
    }

    [Test]
    public void XGroupSetID_LastID()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        Assert.That(this._redis.Stream.XGroupSetID(
            key, "group1", "5-0"), Is.True);

        var result = this._redis.Stream.XReadGroup(
            key, ">", "group1", "consumer1", count: 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue &&
                    (result.Value.Entries?.Length ?? 0) > 0, Is.False);

            Assert.That(this._redis.Stream.XPending(
                key, "group1").Count, Is.Zero);
        }
    }

    [Test]
    public void XGroupSetID_Dollar()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupSetID(
                    key, "group1", "$"), Is.True);

            // $ 跳过现有消息，只读取之后新增的消息
            Assert.That(this._redis.Stream.XAdd(
                key, "6-0", "field", "value6"),
                Is.EqualTo("6-0"));
        }

        this.XGroupSetIDAssertRead(
            key, "consumer1", "6-0");
    }

    [Test]
    public void XGroupSetID_Rewind()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        this.XGroupSetIDAssertRead(
            key, "consumer1",
            "1-0", "2-0", "3-0", "4-0", "5-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(
                    key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XGroupSetID(
                key, "group1", "0-0"), Is.True);
        }

        // 重置消费位置后可重新读取已有消息
        this.XGroupSetIDAssertRead(
            key, "consumer2",
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XGroupSetID_PreservesPEL()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        var read = this._redis.Stream.XReadGroup(
            key, ">", "group1", "consumer1", count: 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(read!.Value.Entries, Has.Length.EqualTo(2));

            Assert.That(this._redis.Stream.XPending(
                key, "group1").Count, Is.EqualTo(2));

            // SETID 只调整消费进度，不会清除 PEL
            Assert.That(this._redis.Stream.XGroupSetID(
                key, "group1", "4-0"), Is.True);
        }

        Assert.That(this._redis.Stream.XPending(
            key, "group1").Count, Is.EqualTo(2));

        this.XGroupSetIDAssertRead(
            key, "consumer2", "5-0");

        Assert.That(this._redis.Stream.XPending(
            key, "group1").Count, Is.EqualTo(3));
    }

    [Test]
    public void XGroupSetID_EntriesRead_Zero()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        Assert.That(this._redis.Stream.XGroupSetID(
            key, "group1", "0-0",
            entriesRead: 0), Is.True);

        this.XGroupSetIDAssertRead(
            key, "consumer1", "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XGroupSetID_EntriesRead_Middle()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        // 已投递到第三条，逻辑已读取数量设为 3
        Assert.That(this._redis.Stream.XGroupSetID(
            key, "group1", "3-0",
            entriesRead: 3), Is.True);

        this.XGroupSetIDAssertRead(
            key, "consumer1", "4-0", "5-0");
    }

    [Test]
    public void XGroupSetID_EntriesRead_Last()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupSetID(
                    key, "group1", "5-0",
                    entriesRead: 5), Is.True);

            // SETID 不改变 Stream 内容
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
            Assert.That(this._redis.Stream.XPending(
                key, "group1").Count, Is.Zero);

            Assert.That(this._redis.Stream.XAdd(
                key, "6-0", "field", "value6"),
                Is.EqualTo("6-0"));
        }

        this.XGroupSetIDAssertRead(
            key, "consumer1", "6-0");
    }

    [Test]
    public void XGroupSetID_DifferentGroups()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreate(
                    key, "group2", "0-0"), Is.True);

            Assert.That(this._redis.Stream.XGroupSetID(
                key, "group1", "3-0",
                entriesRead: 3), Is.True);
        }

        this.XGroupSetIDAssertRead(
            key, "consumer1", "4-0", "5-0");

        // group2 不受 group1 的 SETID 影响
        var group2 = this._redis.Stream.XReadGroup(
            key, ">", "group2", "consumer2", count: 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group2.HasValue, Is.True);
            Assert.That(group2!.Value.Entries, Has.Length.EqualTo(5));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.That(group2.Value.Entries![i].Id,
                Is.EqualTo($"{i + 1}-0"));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XPending(
                    key, "group1").Count, Is.EqualTo(2));

            Assert.That(this._redis.Stream.XPending(
                key, "group2").Count, Is.EqualTo(5));
        }
    }

    [Test]
    public void XGroupSetID_Flags_None()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        Assert.That(this._redis.Stream.XGroupSetID(
            key, "group1", "2-0",
            flags: CommandFlags.None), Is.True);

        this.XGroupSetIDAssertRead(
            key, "consumer1", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XGroupSetID_CancellationToken_Default()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        Assert.That(this._redis.Stream.XGroupSetID(
            key, "group1", "2-0",
            cancellationToken: CancellationToken.None), Is.True);

        this.XGroupSetIDAssertRead(
            key, "consumer1", "3-0", "4-0", "5-0");
    }

    [Test]
    public void XGroupSetID_StreamNotExists()
    {
        var key = XGroupSetIDKey();

        XGroupSetIDRedisError<RedisErrorException>(() =>
            this._redis.Stream.XGroupSetID(
                key, "group1", "0-0"));
    }

    [Test]
    public void XGroupSetID_GroupNotExists()
    {
        var key = XGroupSetIDKey();
        this.XGroupSetIDPrepare(key);

        XGroupSetIDRedisError<RedisErrorException>(() =>
            this._redis.Stream.XGroupSetID(
                key, "not_exists", "0-0"));
    }

    [Test]
    public void XGroupSetID_WrongKeyType()
    {
        var key = XGroupSetIDKey();
        this._redis.String.Set(key, "hello");

        XGroupSetIDRedisError<RedisErrorException>(() =>
            this._redis.Stream.XGroupSetID(
                key, "group1", "0-0"));
    }

    #endregion

    #region XInfoConsumers

    private static string XInfoConsumersKey() =>
        $"Stream:{{XInfoConsumers}}:{Guid.NewGuid():N}";

    private void XInfoConsumersPrepare(string key)
    {
        for (int i = 1; i <= 5; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(this._redis.Stream.XGroupCreate(
            key, "group1", "0-0"), Is.True);
    }

    private void XInfoConsumersRead(
        string key, string group, string consumer, int count)
    {
        var result = this._redis.Stream.XReadGroup(
            key, ">", group, consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries,
                Has.Length.EqualTo(count));
        }
    }

    private ConsumerInfo XInfoConsumersFind(
        string key, string group, string consumer)
    {
        var consumers = this._redis.Stream.XInfoConsumers(
            key, group);

        Assert.That(consumers, Is.Not.Null);

        return consumers!.Single(x => x.Name == consumer);
    }

    private static void XInfoConsumersAssertTime(ConsumerInfo consumer)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(consumer.Idle, Is.GreaterThanOrEqualTo(0));

            // Redis 7.2+ 返回 Inactive，旧版本不存在此字段
            if (consumer.Inactive.HasValue)
            {
                Assert.That(consumer.Inactive.Value,
                    Is.GreaterThanOrEqualTo(-1));
            }
        }
    }

    private static void XInfoConsumersRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XInfoConsumers_Empty()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        // 刚创建消费组，还没有任何消费者
        var result = this._redis.Stream.XInfoConsumers(
            key, "group1");

        // SharpRedis 空数组转换成 null
        Assert.That(result, Is.Null);
    }

    [Test]
    public void XInfoConsumers_CreateConsumer()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreateConsumer(
            key, "group1", "consumer1"), Is.True);

        var result = this._redis.Stream.XInfoConsumers(
            key, "group1");

        Assert.That(result, Has.Length.EqualTo(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result![0].Name,
                Is.EqualTo("consumer1"));

            Assert.That(result[0].Pending, Is.Zero);
        }

        XInfoConsumersAssertTime(result![0]);
    }

    [Test]
    public void XInfoConsumers_AutoCreatedByRead()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        // XREADGROUP 自动创建消费者
        this.XInfoConsumersRead(
            key, "group1", "consumer1", 2);

        var consumer = this.XInfoConsumersFind(
            key, "group1", "consumer1");

        Assert.That(consumer.Pending, Is.EqualTo(2));
        XInfoConsumersAssertTime(consumer);
    }

    [Test]
    public void XInfoConsumers_MultipleConsumers()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        this.XInfoConsumersRead(
            key, "group1", "consumer1", 2);

        this.XInfoConsumersRead(
            key, "group1", "consumer2", 3);

        Assert.That(this._redis.Stream.XGroupCreateConsumer(
            key, "group1", "consumer3"), Is.True);

        var result = this._redis.Stream.XInfoConsumers(
            key, "group1");

        Assert.That(result, Has.Length.EqualTo(3));

        // 不依赖 Redis 返回消费者的顺序
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.Single(x =>
                x.Name == "consumer1").Pending, Is.EqualTo(2));

            Assert.That(result.Single(x =>
                x.Name == "consumer2").Pending, Is.EqualTo(3));

            Assert.That(result.Single(x =>
                x.Name == "consumer3").Pending, Is.Zero);

            Assert.That(result.Sum(x => x.Pending),
                Is.EqualTo(5));
        }
    }

    [Test]
    public void XInfoConsumers_AfterAck()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        this.XInfoConsumersRead(
            key, "group1", "consumer1", 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.XInfoConsumersFind(
                    key, "group1", "consumer1").Pending, Is.EqualTo(3));

            Assert.That(this._redis.Stream.XAck(
                key, "group1", "1-0"), Is.EqualTo(1));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.XInfoConsumersFind(
                    key, "group1", "consumer1").Pending, Is.EqualTo(2));

            Assert.That(this._redis.Stream.XAck(
                key, "group1", "2-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XAck(
                key, "group1", "3-0"), Is.EqualTo(1));
        }

        // PEL 归零，但消费者仍存在
        var consumer = this.XInfoConsumersFind(
            key, "group1", "consumer1");

        Assert.That(consumer.Pending, Is.Zero);
    }

    [Test]
    public void XInfoConsumers_AfterClaim()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        this.XInfoConsumersRead(
            key, "group1", "consumer1", 3);

        var claimed = this._redis.Stream.XClaim(
            key, "group1", "consumer2", 0, "1-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(claimed.HasValue, Is.True);
            Assert.That(claimed!.Value.Id, Is.EqualTo("1-0"));
        }

        var result = this._redis.Stream.XInfoConsumers(
            key, "group1");

        Assert.That(result, Has.Length.EqualTo(2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.Single(x =>
                x.Name == "consumer1").Pending, Is.EqualTo(2));

            Assert.That(result.Single(x =>
                x.Name == "consumer2").Pending, Is.EqualTo(1));
        }
    }

    [Test]
    public void XInfoConsumers_AfterDelConsumer()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        this.XInfoConsumersRead(
            key, "group1", "consumer1", 2);

        this.XInfoConsumersRead(
            key, "group1", "consumer2", 3);

        Assert.That(this._redis.Stream.XGroupDelConsumer(
            key, "group1", "consumer1"), Is.EqualTo(2));

        var result = this._redis.Stream.XInfoConsumers(
            key, "group1");

        Assert.That(result, Has.Length.EqualTo(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result![0].Name,
                Is.EqualTo("consumer2"));

            Assert.That(result[0].Pending, Is.EqualTo(3));
        }
    }

    [Test]
    public void XInfoConsumers_AfterAllConsumersDeleted()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                    key, "group1", "consumer1"), Is.True);

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer2"), Is.True);

            Assert.That(this._redis.Stream.XGroupDelConsumer(
                key, "group1", "consumer1"), Is.Zero);

            Assert.That(this._redis.Stream.XGroupDelConsumer(
                key, "group1", "consumer2"), Is.Zero);

            Assert.That(this._redis.Stream.XInfoConsumers(
                key, "group1"), Is.Null);
        }
    }

    [Test]
    public void XInfoConsumers_DifferentGroups()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreate(
            key, "group2", "0-0"), Is.True);

        this.XInfoConsumersRead(
            key, "group1", "consumer1", 2);

        this.XInfoConsumersRead(
            key, "group2", "consumer1", 4);

        var group1 = this.XInfoConsumersFind(
            key, "group1", "consumer1");

        var group2 = this.XInfoConsumersFind(
            key, "group2", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group1.Pending, Is.EqualTo(2));
            Assert.That(group2.Pending, Is.EqualTo(4));
        }
    }

    [Test]
    public void XInfoConsumers_AfterXDel()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        this.XInfoConsumersRead(
            key, "group1", "consumer1", 2);

        // 默认 XDEL 不清理消费者组的 PEL 引用
        Assert.That(this._redis.Stream.XDel(
            key, "1-0"), Is.EqualTo(1));

        var consumer = this.XInfoConsumersFind(
            key, "group1", "consumer1");

        Assert.That(consumer.Pending, Is.EqualTo(2));
    }

    [Test]
    public void XInfoConsumers_Idle_Inactive()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        this.XInfoConsumersRead(
            key, "group1", "consumer1", 1);

        var consumer = this.XInfoConsumersFind(
            key, "group1", "consumer1");

        XInfoConsumersAssertTime(consumer);

        // XINFO 只是查询，不应改变消费者的 PEL
        Assert.That(consumer.Pending, Is.EqualTo(1));

        var again = this.XInfoConsumersFind(
            key, "group1", "consumer1");

        Assert.That(again.Pending, Is.EqualTo(1));
        XInfoConsumersAssertTime(again);
    }

    [Test]
    public void XInfoConsumers_Flags_None()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreateConsumer(
            key, "group1", "consumer1"), Is.True);

        var result = this._redis.Stream.XInfoConsumers(
            key, "group1", flags: CommandFlags.None);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result![0].Name, Is.EqualTo("consumer1"));
    }

    [Test]
    public void XInfoConsumers_CancellationToken_Default()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreateConsumer(
            key, "group1", "consumer1"), Is.True);

        var result = this._redis.Stream.XInfoConsumers(
            key, "group1",
            cancellationToken: CancellationToken.None);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result![0].Name, Is.EqualTo("consumer1"));
    }

    [Test]
    public void XInfoConsumers_StreamNotExists()
    {
        var key = XInfoConsumersKey();

        XInfoConsumersRedisError<RedisErrorException>(() =>
            this._redis.Stream.XInfoConsumers(
                key, "group1"));
    }

    [Test]
    public void XInfoConsumers_GroupNotExists()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        XInfoConsumersRedisError<RedisErrorException>(() =>
            this._redis.Stream.XInfoConsumers(
                key, "not_exists"));
    }

    [Test]
    public void XInfoConsumers_WrongKeyType()
    {
        var key = XInfoConsumersKey();

        this._redis.String.Set(key, "hello");

        XInfoConsumersRedisError<RedisErrorException>(() =>
            this._redis.Stream.XInfoConsumers(
                key, "group1"));
    }

    [Test]
    public void XInfoConsumers_AfterGroupDestroy()
    {
        var key = XInfoConsumersKey();
        this.XInfoConsumersPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                    key, "group1", "consumer1"), Is.True);

            Assert.That(this._redis.Stream.XGroupDestroy(
                key, "group1"), Is.True);
        }

        XInfoConsumersRedisError<RedisErrorException>(() =>
            this._redis.Stream.XInfoConsumers(
                key, "group1"));
    }

    #endregion

    #region XInfoGroups

    private static string XInfoGroupsKey() =>
        $"Stream:{{XInfoGroups}}:{Guid.NewGuid():N}";

    private void XInfoGroupsPrepare(string key, int count = 5)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }
    }

    private void XInfoGroupsCreate(string key, string group = "group1")
    {
        Assert.That(this._redis.Stream.XGroupCreate(
            key, group, "0-0"), Is.True);
    }

    private void XInfoGroupsRead(
        string key, string group, string consumer, int count)
    {
        var read = this._redis.Stream.XReadGroup(
            key, ">", group, consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(read!.Value.Entries, Has.Length.EqualTo(count));
        }
    }

    private GroupInfo XInfoGroupsFind(string key, string group)
    {
        var groups = this._redis.Stream.XInfoGroups(key);
        Assert.That(groups, Is.Not.Null);

        return groups!.Single(x => x.Name == group);
    }

    private static void XInfoGroupsAssertCounters(
        GroupInfo group, long entriesRead, long lag)
    {
        // Redis 5.x/6.x 不返回这两个字段。
        // Redis 7.0+ 正常可计算时，必须同时验证数值。
        if (group.EntriesRead.HasValue)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(group.EntriesRead.Value,
                            Is.EqualTo(entriesRead));
                Assert.That(group.Lag, Is.EqualTo(lag));
            }
        }
    }

    private static void XInfoGroupsRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XInfoGroups_NoGroups()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);

        var result = this._redis.Stream.XInfoGroups(key);

        // SharpRedis 当前约定：Redis 空数组转换为 null
        Assert.That(result, Is.Null);
    }

    [Test]
    public void XInfoGroups_EmptyGroup()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);
        this.XInfoGroupsCreate(key);

        var result = this._redis.Stream.XInfoGroups(key);

        Assert.That(result, Has.Length.EqualTo(1));

        var group = result![0];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Name, Is.EqualTo("group1"));
            Assert.That(group.Consumers, Is.Zero);
            Assert.That(group.Pending, Is.Zero);
            Assert.That(group.LastDeliveredId, Is.EqualTo("0-0"));
        }

        XInfoGroupsAssertCounters(group, 0, 5);
    }

    [Test]
    public void XInfoGroups_MultipleGroups()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);

        this.XInfoGroupsCreate(key, "group1");
        this.XInfoGroupsCreate(key, "group2");
        this.XInfoGroupsCreate(key, "group3");

        var result = this._redis.Stream.XInfoGroups(key);

        Assert.That(result, Has.Length.EqualTo(3));

        Assert.That(result!.Select(x => x.Name),
            Is.EquivalentTo(
            [
                "group1", "group2", "group3"
            ]));

        foreach (var group in result)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(group.Consumers, Is.Zero);
                Assert.That(group.Pending, Is.Zero);
                Assert.That(group.LastDeliveredId, Is.EqualTo("0-0"));
            }

            XInfoGroupsAssertCounters(group, 0, 5);
        }
    }

    [Test]
    public void XInfoGroups_CreateConsumer()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);
        this.XInfoGroupsCreate(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                    key, "group1", "consumer1"), Is.True);

            Assert.That(this._redis.Stream.XGroupCreateConsumer(
                key, "group1", "consumer2"), Is.True);
        }

        var group = this.XInfoGroupsFind(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Consumers, Is.EqualTo(2));
            Assert.That(group.Pending, Is.Zero);
        }

        XInfoGroupsAssertCounters(group, 0, 5);
    }

    [Test]
    public void XInfoGroups_AfterRead()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);
        this.XInfoGroupsCreate(key);

        this.XInfoGroupsRead(
            key, "group1", "consumer1", 2);

        var group = this.XInfoGroupsFind(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Name, Is.EqualTo("group1"));
            Assert.That(group.Consumers, Is.EqualTo(1));
            Assert.That(group.Pending, Is.EqualTo(2));
            Assert.That(group.LastDeliveredId, Is.EqualTo("2-0"));
        }

        XInfoGroupsAssertCounters(group, 2, 3);
    }

    [Test]
    public void XInfoGroups_AfterAllRead()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);
        this.XInfoGroupsCreate(key);

        this.XInfoGroupsRead(
            key, "group1", "consumer1", 5);

        var group = this.XInfoGroupsFind(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Consumers, Is.EqualTo(1));
            Assert.That(group.Pending, Is.EqualTo(5));
            Assert.That(group.LastDeliveredId, Is.EqualTo("5-0"));
        }

        // 五条消息全部已投递，但都尚未确认
        XInfoGroupsAssertCounters(group, 5, 0);
    }

    [Test]
    public void XInfoGroups_AfterAck()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);
        this.XInfoGroupsCreate(key);

        this.XInfoGroupsRead(
            key, "group1", "consumer1", 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAck(
                    key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XAck(
                key, "group1", "2-0"), Is.EqualTo(1));
        }

        var group = this.XInfoGroupsFind(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Pending, Is.EqualTo(1));
            Assert.That(group.LastDeliveredId, Is.EqualTo("3-0"));
        }

        // ACK 只减少 Pending，不改变已投递进度与 Lag
        XInfoGroupsAssertCounters(group, 3, 2);
    }

    [Test]
    public void XInfoGroups_MultipleConsumers()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);
        this.XInfoGroupsCreate(key);

        this.XInfoGroupsRead(
            key, "group1", "consumer1", 2);

        this.XInfoGroupsRead(
            key, "group1", "consumer2", 3);

        var group = this.XInfoGroupsFind(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Consumers, Is.EqualTo(2));
            Assert.That(group.Pending, Is.EqualTo(5));
            Assert.That(group.LastDeliveredId, Is.EqualTo("5-0"));
        }

        XInfoGroupsAssertCounters(group, 5, 0);
    }

    [Test]
    public void XInfoGroups_DifferentGroupProgress()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);

        this.XInfoGroupsCreate(key, "group1");
        this.XInfoGroupsCreate(key, "group2");

        this.XInfoGroupsRead(
            key, "group1", "consumer1", 2);

        this.XInfoGroupsRead(
            key, "group2", "consumer2", 4);

        var result = this._redis.Stream.XInfoGroups(key);

        Assert.That(result, Has.Length.EqualTo(2));

        var group1 = result!.Single(x => x.Name == "group1");
        var group2 = result.Single(x => x.Name == "group2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group1.Pending, Is.EqualTo(2));
            Assert.That(group1.LastDeliveredId, Is.EqualTo("2-0"));

            Assert.That(group2.Pending, Is.EqualTo(4));
            Assert.That(group2.LastDeliveredId, Is.EqualTo("4-0"));
        }

        XInfoGroupsAssertCounters(group1, 2, 3);
        XInfoGroupsAssertCounters(group2, 4, 1);
    }

    [Test]
    public void XInfoGroups_AfterDelConsumer()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);
        this.XInfoGroupsCreate(key);

        this.XInfoGroupsRead(
            key, "group1", "consumer1", 2);

        this.XInfoGroupsRead(
            key, "group1", "consumer2", 3);

        Assert.That(this._redis.Stream.XGroupDelConsumer(
            key, "group1", "consumer1"), Is.EqualTo(2));

        var group = this.XInfoGroupsFind(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Consumers, Is.EqualTo(1));
            Assert.That(group.Pending, Is.EqualTo(3));
            Assert.That(group.LastDeliveredId, Is.EqualTo("5-0"));
        }

        XInfoGroupsAssertCounters(group, 5, 0);
    }

    [Test]
    public void XInfoGroups_AfterGroupDestroy()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);

        this.XInfoGroupsCreate(key, "group1");
        this.XInfoGroupsCreate(key, "group2");

        Assert.That(this._redis.Stream.XGroupDestroy(
            key, "group1"), Is.True);

        var result = this._redis.Stream.XInfoGroups(key);

        Assert.That(result, Has.Length.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result![0].Name, Is.EqualTo("group2"));

            Assert.That(this._redis.Stream.XGroupDestroy(
                key, "group2"), Is.True);

            Assert.That(this._redis.Stream.XInfoGroups(key),
                Is.Null);

            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public void XInfoGroups_SetID()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);
        this.XInfoGroupsCreate(key);

        Assert.That(this._redis.Stream.XGroupSetID(
            key, "group1", "3-0", entriesRead: 3), Is.True);

        var group = this.XInfoGroupsFind(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.LastDeliveredId, Is.EqualTo("3-0"));
            Assert.That(group.Pending, Is.Zero);
        }

        XInfoGroupsAssertCounters(group, 3, 2);

        this.XInfoGroupsRead(
            key, "group1", "consumer1", 2);

        group = this.XInfoGroupsFind(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.LastDeliveredId, Is.EqualTo("5-0"));
            Assert.That(group.Pending, Is.EqualTo(2));
        }

        XInfoGroupsAssertCounters(group, 5, 0);
    }

    [Test]
    public void XInfoGroups_SetID_UnknownLag()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);
        this.XInfoGroupsCreate(key);

        // 任意中间 ID，不显式指定 ENTRIESREAD
        Assert.That(this._redis.Stream.XGroupSetID(
            key, "group1", "3-0"), Is.True);

        var group = this.XInfoGroupsFind(key, "group1");

        Assert.That(group.LastDeliveredId, Is.EqualTo("3-0"));

        // Redis 7.0+ 的 Lag 可能为 null，
        // 因为无法确定任意中间 ID 对应的逻辑计数
        if (group.EntriesRead.HasValue || group.Lag.HasValue)
        {
            Assert.That(group.Lag, Is.Null);
        }
    }

    [Test]
    public void XInfoGroups_Flags_None()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);
        this.XInfoGroupsCreate(key);

        var result = this._redis.Stream.XInfoGroups(
            key, flags: CommandFlags.None);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result![0].Name, Is.EqualTo("group1"));
    }

    [Test]
    public void XInfoGroups_CancellationToken_Default()
    {
        var key = XInfoGroupsKey();
        this.XInfoGroupsPrepare(key);
        this.XInfoGroupsCreate(key);

        var result = this._redis.Stream.XInfoGroups(
            key, cancellationToken: CancellationToken.None);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result![0].Name, Is.EqualTo("group1"));
    }

    [Test]
    public void XInfoGroups_StreamNotExists()
    {
        var key = XInfoGroupsKey();

        XInfoGroupsRedisError<RedisErrorException>(() =>
            this._redis.Stream.XInfoGroups(key));
    }

    [Test]
    public void XInfoGroups_WrongKeyType()
    {
        var key = XInfoGroupsKey();
        this._redis.String.Set(key, "hello");

        XInfoGroupsRedisError<RedisErrorException>(() =>
            this._redis.Stream.XInfoGroups(key));
    }

    #endregion

    #region XInfoStream

    private static string XInfoStreamKey() =>
        $"Stream:{{XInfoStream}}:{Guid.NewGuid():N}";

    private void XInfoStreamSeed(string key, int count = 12)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(this._redis.Stream.XLen(key),
            Is.EqualTo(count));
    }

    private void XInfoStreamCreateGroup(
        string key, string group = "group1")
    {
        Assert.That(this._redis.Stream.XGroupCreate(
            key, group, "0-0"), Is.True);
    }

    private void XInfoStreamRead(
        string key, string group, string consumer, int count)
    {
        var result = this._redis.Stream.XReadGroup(
            key, ">", group, consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries,
                Has.Length.EqualTo(count));
        }
    }

    private static void XInfoStreamAssertIds(
        EntryItem[]? entries, params string[] expected)
    {
        Assert.That(entries?.Length ?? 0,
            Is.EqualTo(expected.Length));

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.That(entries![i].Id,
                Is.EqualTo(expected[i]));
        }
    }

    private static void XInfoStreamRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XInfoStream_Summary_Basic()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key);

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.IsFull, Is.False);
            Assert.That(info.Length, Is.EqualTo(12));
            Assert.That(info.RadixTreeKeys, Is.GreaterThan(0));
            Assert.That(info.RadixTreeNodes, Is.GreaterThan(0));
            Assert.That(info.LastGeneratedId, Is.EqualTo("12-0"));
            Assert.That(info.GroupCount, Is.Zero);
            Assert.That(info.FirstEntry?.Id, Is.EqualTo("1-0"));
            Assert.That(info.LastEntry?.Id, Is.EqualTo("12-0"));
            Assert.That(info.Entries, Is.Null);
            Assert.That(info.FullGroups, Is.Null);
        }
    }

    [Test]
    public void XInfoStream_Summary_WithGroups()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key);

        this.XInfoStreamCreateGroup(key, "group1");
        this.XInfoStreamCreateGroup(key, "group2");

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.IsFull, Is.False);
            Assert.That(info.GroupCount, Is.EqualTo(2));
            Assert.That(info.Length, Is.EqualTo(12));
        }
        Assert.That(info.FullGroups, Is.Null);
    }

    [Test]
    public void XInfoStream_Summary_AfterDelete()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDel(
                    key, "1-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XDel(
                key, "12-0"), Is.EqualTo(1));
        }

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Length, Is.EqualTo(10));
            Assert.That(info.FirstEntry?.Id, Is.EqualTo("2-0"));
            Assert.That(info.LastEntry?.Id, Is.EqualTo("11-0"));
            Assert.That(info.LastGeneratedId, Is.EqualTo("12-0"));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("12-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(12));
        }
    }

    [Test]
    public void XInfoStream_Full_DefaultCount()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key);

        var info = this._redis.Stream.XInfoStream(
            key, full: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.IsFull, Is.True);
            Assert.That(info.Length, Is.EqualTo(12));
            Assert.That(info.GroupCount, Is.Null);
            Assert.That(info.FirstEntry, Is.Null);
            Assert.That(info.LastEntry, Is.Null);
        }

        XInfoStreamAssertIds(info.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0",
            "6-0", "7-0", "8-0", "9-0", "10-0");
    }

    [Test]
    public void XInfoStream_Full_Count()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key);

        var info = this._redis.Stream.XInfoStream(
            key, full: true, count: 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.IsFull, Is.True);
            Assert.That(info.Length, Is.EqualTo(12));
        }

        XInfoStreamAssertIds(info.Entries,
            "1-0", "2-0", "3-0");
    }

    [Test]
    public void XInfoStream_Full_CountZero()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key);

        var info = this._redis.Stream.XInfoStream(
            key, full: true, count: 0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.IsFull, Is.True);
            Assert.That(info.Entries, Has.Length.EqualTo(12));
        }

        for (int i = 0; i < 12; i++)
        {
            Assert.That(info.Entries![i].Id,
                Is.EqualTo($"{i + 1}-0"));
        }
    }

    [Test]
    public void XInfoStream_Full_WithGroup_NoPending()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 5);
        this.XInfoStreamCreateGroup(key);

        var info = this._redis.Stream.XInfoStream(
            key, full: true, count: 0);

        Assert.That(info.FullGroups, Has.Length.EqualTo(1));

        var group = info.FullGroups![0];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Name, Is.EqualTo("group1"));
            Assert.That(group.LastDeliveredId, Is.EqualTo("0-0"));
            Assert.That(group.PelCount, Is.Zero);
            Assert.That(group.EntriesRead, Is.Null);
            Assert.That(group.Lag, Is.EqualTo(5));
            Assert.That(group.Pending?.Length ?? 0, Is.Zero);
            Assert.That(group.Consumers?.Length ?? 0, Is.Zero);
        }
    }

    [Test]
    public void XInfoStream_Full_GroupAndConsumerPending()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 5);
        this.XInfoStreamCreateGroup(key);
        this.XInfoStreamRead(key, "group1", "consumer1", 3);

        var info = this._redis.Stream.XInfoStream(
            key, full: true, count: 0);

        Assert.That(info.FullGroups, Has.Length.EqualTo(1));

        var group = info.FullGroups![0];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.LastDeliveredId, Is.EqualTo("3-0"));
            Assert.That(group.EntriesRead, Is.EqualTo(3));
            Assert.That(group.Lag, Is.EqualTo(2));
            Assert.That(group.PelCount, Is.EqualTo(3));
            Assert.That(group.Pending, Has.Length.EqualTo(3));
            Assert.That(group.Consumers, Has.Length.EqualTo(1));
        }

        var pending = group.Pending![0];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(pending.Id, Is.EqualTo("1-0"));
            Assert.That(pending.Consumer, Is.EqualTo("consumer1"));
            Assert.That(pending.DeliveryCount, Is.EqualTo(1));
            Assert.That(pending.DeliveryTime, Is.GreaterThan(0));
        }

        var consumer = group.Consumers![0];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(consumer.Name, Is.EqualTo("consumer1"));
            Assert.That(consumer.PelCount, Is.EqualTo(3));
            Assert.That(consumer.Pending, Has.Length.EqualTo(3));
            Assert.That(consumer.SeenTime, Is.GreaterThan(0));
        }

        // Redis 7.2+ 增加 active-time
        if (consumer.ActiveTime.HasValue)
        {
            Assert.That(consumer.ActiveTime.Value,
                Is.GreaterThan(0));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(consumer.Pending![0].Id, Is.EqualTo("1-0"));
            Assert.That(consumer.Pending[0].DeliveryCount,
                Is.EqualTo(1));
            Assert.That(consumer.Pending[0].DeliveryTime,
                Is.GreaterThan(0));
        }
    }

    [Test]
    public void XInfoStream_Full_MultipleGroups()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 5);

        this.XInfoStreamCreateGroup(key, "group1");
        this.XInfoStreamCreateGroup(key, "group2");

        this.XInfoStreamRead(key, "group1", "consumer1", 2);
        this.XInfoStreamRead(key, "group2", "consumer2", 4);

        var info = this._redis.Stream.XInfoStream(
            key, full: true, count: 0);

        Assert.That(info.FullGroups, Has.Length.EqualTo(2));

        var group1 = info.FullGroups!.Single(
            x => x.Name == "group1");

        var group2 = info.FullGroups.Single(
            x => x.Name == "group2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group1.PelCount, Is.EqualTo(2));
            Assert.That(group1.LastDeliveredId, Is.EqualTo("2-0"));
            Assert.That(group1.EntriesRead, Is.EqualTo(2));
            Assert.That(group1.Lag, Is.EqualTo(3));

            Assert.That(group2.PelCount, Is.EqualTo(4));
            Assert.That(group2.LastDeliveredId, Is.EqualTo("4-0"));
            Assert.That(group2.EntriesRead, Is.EqualTo(4));
            Assert.That(group2.Lag, Is.EqualTo(1));
        }
    }

    [Test]
    public void XInfoStream_Full_PendingCountLimit()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 12);
        this.XInfoStreamCreateGroup(key);
        this.XInfoStreamRead(key, "group1", "consumer1", 12);

        var info = this._redis.Stream.XInfoStream(
            key, full: true, count: 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Entries, Has.Length.EqualTo(2));
            Assert.That(info.FullGroups, Has.Length.EqualTo(1));
        }

        var group = info.FullGroups![0];
        var consumer = group.Consumers!.Single(
            x => x.Name == "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.PelCount, Is.EqualTo(12));
            Assert.That(group.Pending, Has.Length.EqualTo(2));
            Assert.That(consumer.PelCount, Is.EqualTo(12));
            Assert.That(consumer.Pending, Has.Length.EqualTo(2));
            Assert.That(group.Lag, Is.Zero);
        }
    }

    [Test]
    public void XInfoStream_Full_AfterAck()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 5);
        this.XInfoStreamCreateGroup(key);
        this.XInfoStreamRead(key, "group1", "consumer1", 3);

        Assert.That(this._redis.Stream.XAck(
            key, "group1", "1-0"), Is.EqualTo(1));

        var info = this._redis.Stream.XInfoStream(
            key, full: true, count: 0);

        var group = info.FullGroups![0];
        var consumer = group.Consumers![0];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.PelCount, Is.EqualTo(2));
            Assert.That(group.Pending, Has.Length.EqualTo(2));
            Assert.That(consumer.PelCount, Is.EqualTo(2));
            Assert.That(consumer.Pending, Has.Length.EqualTo(2));
            Assert.That(group.EntriesRead, Is.EqualTo(3));
            Assert.That(group.Lag, Is.EqualTo(2));
        }

        Assert.That(group.Pending!.Select(x => x.Id),
            Is.EquivalentTo(["2-0", "3-0"]));
    }

    [Test]
    public void XInfoStream_Full_AfterClaim()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 5);
        this.XInfoStreamCreateGroup(key);
        this.XInfoStreamRead(key, "group1", "consumer1", 2);

        var claimed = this._redis.Stream.XClaim(
            key, "group1", "consumer2", 0, "1-0");

        Assert.That(claimed.HasValue, Is.True);

        var info = this._redis.Stream.XInfoStream(
            key, full: true, count: 0);

        var group = info.FullGroups![0];

        Assert.That(group.PelCount, Is.EqualTo(2));

        var pending = group.Pending!.Single(x => x.Id == "1-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(pending.Consumer, Is.EqualTo("consumer2"));
            Assert.That(pending.DeliveryCount, Is.EqualTo(2));
        }

        Assert.That(group.Consumers, Has.Length.EqualTo(2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Consumers!.Single(
                    x => x.Name == "consumer1").PelCount, Is.EqualTo(1));

            Assert.That(group.Consumers.Single(
                x => x.Name == "consumer2").PelCount, Is.EqualTo(1));
        }
    }

    [Test]
    public void XInfoStream_Full_SeenTime_ActiveTime()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 5);
        this.XInfoStreamCreateGroup(key);

        // 显式创建但未读取的消费者
        Assert.That(this._redis.Stream.XGroupCreateConsumer(
            key, "group1", "consumer1"), Is.True);

        var info = this._redis.Stream.XInfoStream(
            key, full: true);

        var consumer = info.FullGroups![0].Consumers!.Single(
            x => x.Name == "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(consumer.PelCount, Is.Zero);
            Assert.That(consumer.SeenTime, Is.GreaterThanOrEqualTo(0));
        }

        // Redis 7.2+ 无成功交互时可能返回 -1
        if (consumer.ActiveTime.HasValue)
        {
            Assert.That(consumer.ActiveTime.Value,
                Is.GreaterThanOrEqualTo(-1));
        }

        this.XInfoStreamRead(key, "group1", "consumer1", 1);

        info = this._redis.Stream.XInfoStream(key, full: true);

        consumer = info.FullGroups![0].Consumers!.Single(
            x => x.Name == "consumer1");

        Assert.That(consumer.SeenTime, Is.GreaterThan(0));

        if (consumer.ActiveTime.HasValue)
        {
            Assert.That(consumer.ActiveTime.Value,
                Is.GreaterThan(0));
        }
    }

    [Test]
    public void XInfoStream_Summary_IdmpConfiguration()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 5);

        Assert.That(this._redis.Stream.XCfgSet(
            key, duration: 300, maxsize: 40), Is.True);

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.IdmpDuration, Is.EqualTo(300));
            Assert.That(info.IdmpMaxSize, Is.EqualTo(40));
            Assert.That(info.PidsTracked, Is.Zero);
            Assert.That(info.IidsTracked, Is.Zero);
            Assert.That(info.IidsAdded, Is.Zero);
            Assert.That(info.IidsDuplicates, Is.Zero);
        }
    }

    [Test]
    public void XInfoStream_Full_IdmpConfiguration()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 5);

        Assert.That(this._redis.Stream.XCfgSet(
            key, duration: 600, maxsize: 100), Is.True);

        var info = this._redis.Stream.XInfoStream(
            key, full: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.IsFull, Is.True);
            Assert.That(info.IdmpDuration, Is.EqualTo(600));
            Assert.That(info.IdmpMaxSize, Is.EqualTo(100));
        }
    }

    [Test]
    public void XInfoStream_Full_NackedCount()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 5);
        this.XInfoStreamCreateGroup(key);
        this.XInfoStreamRead(key, "group1", "consumer1", 2);

        var info = this._redis.Stream.XInfoStream(
            key, full: true);

        var group = info.FullGroups![0];

        // Redis 8.8+ 才提供此字段。
        if (group.NackedCount.HasValue)
        {
            Assert.That(group.NackedCount.Value,
                Is.Zero);
        }

        Assert.That(group.PelCount, Is.EqualTo(2));
    }

    [Test]
    public void XInfoStream_Flags_None()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 5);

        var info = this._redis.Stream.XInfoStream(
            key, flags: CommandFlags.None);

        Assert.That(info.Length, Is.EqualTo(5));
    }

    [Test]
    public void XInfoStream_CancellationToken_Default()
    {
        var key = XInfoStreamKey();
        this.XInfoStreamSeed(key, 5);

        var info = this._redis.Stream.XInfoStream(
            key, cancellationToken: CancellationToken.None);

        Assert.That(info.Length, Is.EqualTo(5));
    }

    [Test]
    public void XInfoStream_StreamNotExists()
    {
        var key = XInfoStreamKey();

        XInfoStreamRedisError<RedisErrorException>(() =>
            this._redis.Stream.XInfoStream(key));
    }

    [Test]
    public void XInfoStream_WrongKeyType()
    {
        var key = XInfoStreamKey();

        this._redis.String.Set(key, "hello");

        XInfoStreamRedisError<RedisErrorException>(() =>
            this._redis.Stream.XInfoStream(key));
    }

    #endregion

    #region XNAck

    [Test]
    public void XNAck_Single_Fail()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = redis.Stream.XAdd(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        redis.Stream.XGroupCreate(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            // FORCE 创建待处理记录
            Assert.That(redis.Stream.XNAck(
                key, group, id, NackMode.Fail, force: true), Is.EqualTo(1));

            // 普通释放仍能处理该 PEL 记录
            Assert.That(redis.Stream.XNAck(
                key, group, id, NackMode.Fail), Is.EqualTo(1));

            // XNACK 不删除 PEL 记录
            Assert.That(redis.Stream.XAck(key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public void XNAck_Single_Silent()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = redis.Stream.XAdd(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        redis.Stream.XGroupCreate(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(redis.Stream.XNAck(
                    key, group, id, NackMode.Silent, force: true), Is.EqualTo(1));

            Assert.That(redis.Stream.XNAck(
                key, group, id, NackMode.Silent), Is.EqualTo(1));

            Assert.That(redis.Stream.XAck(key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public void XNAck_Single_Fatal()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = redis.Stream.XAdd(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        redis.Stream.XGroupCreate(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(redis.Stream.XNAck(
                    key, group, id, NackMode.Fatal, force: true), Is.EqualTo(1));

            Assert.That(redis.Stream.XAck(key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public void XNAck_Collection_Multiple()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id1 = redis.Stream.XAdd(key, "field", "value1");
        var id2 = redis.Stream.XAdd(key, "field", "value2");
        var id3 = redis.Stream.XAdd(key, "field", "value3");

        Assert.That(id1 != null && id2 != null && id3 != null, Is.True);

        redis.Stream.XGroupCreate(key, group, "0-0");

        ICollection<EntryId> ids = [id1!, id2!, id3!];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(redis.Stream.XNAck(
                    key, group, ids, NackMode.Fail, force: true), Is.EqualTo(3));

            Assert.That(redis.Stream.XNAck(
                key, group, ids, NackMode.Silent), Is.EqualTo(3));

            Assert.That(redis.Stream.XAck(key, group, ids), Is.EqualTo(3));
        }
    }

    [Test]
    public void XNAck_ReadOnlySpan_Multiple()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id1 = redis.Stream.XAdd(key, "field", "value1");
        var id2 = redis.Stream.XAdd(key, "field", "value2");

        Assert.That(id1 != null && id2 != null, Is.True);

        redis.Stream.XGroupCreate(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            // ReadOnlySpan<EntryId> 直接使用集合表达式
            Assert.That(redis.Stream.XNAck(
                key, group, [id1!, id2!],
                NackMode.Fail, force: true), Is.EqualTo(2));

            Assert.That(redis.Stream.XNAck(
                key, group, [id1!, id2!],
                NackMode.Fatal), Is.EqualTo(2));
        }
    }

    [Test]
    public void XNAck_Force_False_NotPending()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = redis.Stream.XAdd(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        redis.Stream.XGroupCreate(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            // 消息存在于 Stream，但尚未进入 PEL
            Assert.That(redis.Stream.XNAck(
                key, group, id, NackMode.Fail), Is.Zero);

            Assert.That(redis.Stream.XAck(key, group, id), Is.Zero);
        }
    }

    [Test]
    public void XNAck_Force_True_CreatesPending()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = redis.Stream.XAdd(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        redis.Stream.XGroupCreate(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(redis.Stream.XNAck(
                    key, group, id, NackMode.Fail, force: true), Is.EqualTo(1));

            Assert.That(redis.Stream.XAck(key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public void XNAck_Force_True_NonexistentEntry()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = redis.Stream.XAdd(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        redis.Stream.XGroupCreate(key, group, "0-0");

        // ID 格式合法，但该消息不存在
        Assert.That(redis.Stream.XNAck(
            key, group, "999999999999999-0",
            NackMode.Fail, force: true), Is.Zero);
    }

    [Test]
    public void XNAck_RetryCount_Zero()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = redis.Stream.XAdd(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        redis.Stream.XGroupCreate(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(redis.Stream.XNAck(
                    key, group, id, NackMode.Fatal,
                    retryCount: 0, force: true), Is.EqualTo(1));

            Assert.That(redis.Stream.XAck(key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public void XNAck_RetryCount_Positive()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = redis.Stream.XAdd(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        redis.Stream.XGroupCreate(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(redis.Stream.XNAck(
                    key, group, id, NackMode.Silent,
                    retryCount: 10, force: true), Is.EqualTo(1));

            Assert.That(redis.Stream.XAck(key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public void XNAck_RetryCount_Negative_Throws()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = redis.Stream.XAdd(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        redis.Stream.XGroupCreate(key, group, "0-0");

        XNAckRedisError<RedisErrorException>(() =>
            redis.Stream.XNAck(
                key, group, id, NackMode.Fail,
                retryCount: -1, force: true));
    }

    [Test]
    public void XNAck_GroupNotExists_Throws()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";

        var id = redis.Stream.XAdd(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        XNAckRedisError<RedisErrorException>(() =>
            redis.Stream.XNAck(
                key, "nonexistent-group", id, NackMode.Fail));
    }

    [Test]
    public void XNAck_Collection_MixedPendingAndNonPending()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id1 = redis.Stream.XAdd(key, "field", "value1");
        var id2 = redis.Stream.XAdd(key, "field", "value2");

        Assert.That(id1 != null && id2 != null, Is.True);

        redis.Stream.XGroupCreate(key, group, "0-0");

        Assert.That(redis.Stream.XNAck(
            key, group, id1!, NackMode.Fail,
            force: true), Is.EqualTo(1));

        ICollection<EntryId> ids = [id1!, id2!];

        // id1 已在 PEL，id2 不在 PEL
        Assert.That(redis.Stream.XNAck(
            key, group, ids, NackMode.Fail), Is.EqualTo(1));
    }

    private static void XNAckRedisError<TError>(Action action)
        where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    #endregion

    #region XRevRange

    private static string XRevRangeKey() =>
        $"Stream:{{XRevRange}}:{Guid.NewGuid():N}";

    private void XRevRangePrepare(string key)
    {
        for (int i = 1; i <= 5; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }
    }

    private static void XRevRangeAssertIds(
        EntryItem[]? entries, params string[] expected)
    {
        Assert.That(entries, Has.Length.EqualTo(expected.Length));

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.That(entries![i].Id, Is.EqualTo(expected[i]),
                $"Entry index {i}");
        }
    }

    private static void XRevRangeRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XRevRange_All()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(key, "+", "-");

        XRevRangeAssertIds(result,
            "5-0", "4-0", "3-0", "2-0", "1-0");
    }

    [Test]
    public void XRevRange_SpecifiedRange()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(
            key, "4-0", "2-0");

        XRevRangeAssertIds(result, "4-0", "3-0", "2-0");
    }

    [Test]
    public void XRevRange_BoundariesInclusive()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(
            key, "5-0", "1-0");

        XRevRangeAssertIds(result,
            "5-0", "4-0", "3-0", "2-0", "1-0");
    }

    [Test]
    public void XRevRange_SingleEntry()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(
            key, "3-0", "3-0");

        XRevRangeAssertIds(result, "3-0");
    }

    [Test]
    public void XRevRange_EntryContent()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(
            key, "3-0", "2-0");

        XRevRangeAssertIds(result, "3-0", "2-0");

        using (Assert.EnterMultipleScope())
        {
            // 验证消息 ID 对应的实际内容，
            // 避免只解析 ID 正确、FieldValue 解析错误。
            Assert.That(result![0].FieldValue, Is.Not.Null);
            Assert.That(result[1].FieldValue, Is.Not.Null);
        }
    }

    [Test]
    public void XRevRange_CountOne()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(
            key, "+", "-", count: 1);

        XRevRangeAssertIds(result, "5-0");
    }

    [Test]
    public void XRevRange_CountThree()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(
            key, "+", "-", count: 3);

        XRevRangeAssertIds(result, "5-0", "4-0", "3-0");
    }

    [Test]
    public void XRevRange_CountGreaterThanEntries()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(
            key, "+", "-", count: 100);

        XRevRangeAssertIds(result,
            "5-0", "4-0", "3-0", "2-0", "1-0");
    }

    [Test]
    public void XRevRange_CountWithinRange()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(
            key, "4-0", "2-0", count: 2);

        XRevRangeAssertIds(result, "4-0", "3-0");
    }

    [Test]
    public void XRevRange_EndLessThanStart()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        // 最大 ID 比最小 ID 小，结果为空。
        var result = this._redis.Stream.XRevRange(
            key, "2-0", "4-0");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void XRevRange_NoMatchingEntries()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(
            key, "100-0", "99-0");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void XRevRange_MissingKey()
    {
        var key = XRevRangeKey();

        // XREVRANGE 读取不存在的 Stream，返回空数组。
        var result = this._redis.Stream.XRevRange(
            key, "+", "-");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void XRevRange_AfterDelete()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        Assert.That(this._redis.Stream.XDel(
            key, "3-0"), Is.EqualTo(1));

        var result = this._redis.Stream.XRevRange(
            key, "+", "-");

        XRevRangeAssertIds(result,
            "5-0", "4-0", "2-0", "1-0");
    }

    [Test]
    public void XRevRange_SameTimestampDifferentSequence()
    {
        var key = XRevRangeKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(
                    key, "100-0", "field", "a"), Is.EqualTo("100-0"));

            Assert.That(this._redis.Stream.XAdd(
                key, "100-1", "field", "b"), Is.EqualTo("100-1"));

            Assert.That(this._redis.Stream.XAdd(
                key, "100-2", "field", "c"), Is.EqualTo("100-2"));

            Assert.That(this._redis.Stream.XAdd(
                key, "101-0", "field", "d"), Is.EqualTo("101-0"));
        }

        var result = this._redis.Stream.XRevRange(
            key, "+", "-");

        XRevRangeAssertIds(result,
            "101-0", "100-2", "100-1", "100-0");
    }

    [Test]
    public void XRevRange_ExclusiveRange()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        // Redis 6.2+：
        // (4-0 排除 4-0
        // (2-0 排除 2-0
        var result = this._redis.Stream.XRevRange(
            key, "(4-0", "(2-0");

        XRevRangeAssertIds(result, "3-0");
    }

    [Test]
    public void XRevRange_Flags_None()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(
            key, "+", "-", flags: CommandFlags.None);

        XRevRangeAssertIds(result,
            "5-0", "4-0", "3-0", "2-0", "1-0");
    }

    [Test]
    public void XRevRange_CancellationToken_Default()
    {
        var key = XRevRangeKey();
        this.XRevRangePrepare(key);

        var result = this._redis.Stream.XRevRange(
            key, "+", "-",
            cancellationToken: CancellationToken.None);

        XRevRangeAssertIds(result,
            "5-0", "4-0", "3-0", "2-0", "1-0");
    }

    [Test]
    public void XRevRange_WrongKeyType()
    {
        var key = XRevRangeKey();
        this._redis.String.Set(key, "hello");

        XRevRangeRedisError<RedisErrorException>(() =>
            this._redis.Stream.XRevRange(key, "+", "-"));
    }

    #endregion

    #region XSetID

    private static string XSetIDKey() =>
        $"Stream:{{XSetID}}:{Guid.NewGuid():N}";

    private void XSetIDPrepare(string key)
    {
        for (int i = 1; i <= 3; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }
    }

    private static void XSetIDRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XSetID_OnlyLastId()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        Assert.That(this._redis.Stream.XSetID(
            key, "100-0"), Is.True);

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
            Assert.That(info.Length, Is.EqualTo(3));
            Assert.That(info.EntriesAdded, Is.EqualTo(3));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("0-0"));
        }
    }

    [Test]
    public void XSetID_LastIdEqualsCurrent()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        Assert.That(this._redis.Stream.XSetID(
            key, "3-0"), Is.True);

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("3-0"));
            Assert.That(info.Length, Is.EqualTo(3));
        }
    }

    [Test]
    public void XSetID_EntriesAdded()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        Assert.That(this._redis.Stream.XSetID(
            key, "100-0", entriesAdded: 50), Is.True);

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(50));
            Assert.That(info.Length, Is.EqualTo(3));
        }
    }

    [Test]
    public void XSetID_EntriesAddedEqualsLength()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        // entriesAdded 可以等于 XLEN
        Assert.That(this._redis.Stream.XSetID(
            key, "100-0", entriesAdded: 3), Is.True);

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.EntriesAdded, Is.EqualTo(3));
            Assert.That(info.Length, Is.EqualTo(3));
        }
    }

    [Test]
    public void XSetID_MaxDeletedId()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        Assert.That(this._redis.Stream.XSetID(
            key, "100-0", maxDeletedId: "50-0"), Is.True);

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("50-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(3));
            Assert.That(info.Length, Is.EqualTo(3));
        }
    }

    [Test]
    public void XSetID_MaxDeletedIdEqualsLastId()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        Assert.That(this._redis.Stream.XSetID(
            key, "100-0", maxDeletedId: "100-0"), Is.True);

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("100-0"));
        }
    }

    [Test]
    public void XSetID_AllParameters()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        Assert.That(this._redis.Stream.XSetID(
            key,
            "100-0",
            entriesAdded: 50,
            maxDeletedId: "90-0"), Is.True);

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(50));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("90-0"));
            Assert.That(info.Length, Is.EqualTo(3));
            Assert.That(info.FirstEntry?.Id, Is.EqualTo("1-0"));
            Assert.That(info.LastEntry?.Id, Is.EqualTo("3-0"));
        }
    }

    [Test]
    public void XSetID_AfterXAdd()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XSetID(
                    key, "100-0", entriesAdded: 50), Is.True);

            // 新增消息必须大于修改后的 last-generated-id
            Assert.That(this._redis.Stream.XAdd(
                key, "101-0", "field", "value101"),
                Is.EqualTo("101-0"));
        }

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("101-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(51));
            Assert.That(info.Length, Is.EqualTo(4));
        }
    }

    [Test]
    public void XSetID_AfterXDel()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XDel(
                    key, "3-0"), Is.EqualTo(1));

            Assert.That(this._redis.Stream.XSetID(
                key, "100-0"), Is.True);
        }

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("3-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(3));
            Assert.That(info.Length, Is.EqualTo(2));
        }
    }

    [Test]
    public void XSetID_LastIdLessThanTopItem()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        // 当前最大有效 ID 为 3-0
        XSetIDRedisError<RedisErrorException>(() =>
            this._redis.Stream.XSetID(key, "2-0"));

        var info = this._redis.Stream.XInfoStream(key);

        Assert.That(info.LastGeneratedId, Is.EqualTo("3-0"));
    }

    [Test]
    public void XSetID_LastIdLessThanMaxDeletedId()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        Assert.That(this._redis.Stream.XDel(
            key, "3-0"), Is.EqualTo(1));

        // 当前最大有效 ID 为 2-0，
        // 但历史最大删除 ID 为 3-0
        XSetIDRedisError<RedisErrorException>(() =>
            this._redis.Stream.XSetID(key, "2-0"));

        var info = this._redis.Stream.XInfoStream(key);

        Assert.That(info.MaxDeletedEntryId, Is.EqualTo("3-0"));
    }

    [Test]
    public void XSetID_EntriesAddedLessThanLength()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        // XLEN = 3，不能将 entriesAdded 设置为 2
        XSetIDRedisError<RedisErrorException>(() =>
            this._redis.Stream.XSetID(
                key, "100-0", entriesAdded: 2));

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("3-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(3));
            Assert.That(info.Length, Is.EqualTo(3));
        }
    }

    [Test]
    public void XSetID_MaxDeletedIdGreaterThanLastId()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        // MAXDELETEDID 101-0 > lastId 100-0
        XSetIDRedisError<RedisErrorException>(() =>
            this._redis.Stream.XSetID(
                key, "100-0", maxDeletedId: "101-0"));

        var info = this._redis.Stream.XInfoStream(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("3-0"));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("0-0"));
        }
    }

    [Test]
    public void XSetID_Flags_None()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XSetID(
                    key, "100-0", flags: CommandFlags.None), Is.True);

            Assert.That(
                this._redis.Stream.XInfoStream(key).LastGeneratedId,
                Is.EqualTo("100-0"));
        }
    }

    [Test]
    public void XSetID_CancellationToken_Default()
    {
        var key = XSetIDKey();
        this.XSetIDPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XSetID(
                    key, "100-0",
                    cancellationToken: CancellationToken.None), Is.True);

            Assert.That(
                this._redis.Stream.XInfoStream(key).LastGeneratedId,
                Is.EqualTo("100-0"));
        }
    }

    [Test]
    public void XSetID_StreamNotExists()
    {
        var key = XSetIDKey();

        XSetIDRedisError<RedisErrorException>(() =>
            this._redis.Stream.XSetID(key, "100-0"));

        // XSETID 不会自动创建 Stream
        Assert.That(this._redis.Key.Exists(key), Is.False);
    }

    [Test]
    public void XSetID_WrongKeyType()
    {
        var key = XSetIDKey();

        this._redis.String.Set(key, "hello");

        XSetIDRedisError<RedisErrorException>(() =>
            this._redis.Stream.XSetID(key, "100-0"));
    }

    #endregion

    #region XTrim

    private static string XTrimKey() =>
        $"Stream:{{XTrim}}:{Guid.NewGuid():N}";

    private void XTrimPrepare(string key, int count = 6)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(this._redis.Stream.XAdd(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }
    }

    private void XTrimAssertEntries(
        string key, params string[] expectedIds)
    {
        var result = this._redis.Stream.XRevRange(key, "+", "-");

        if (expectedIds.Length == 0)
        {
            Assert.That(result, Is.Null);
            return;
        }

        Assert.That(result, Has.Length.EqualTo(expectedIds.Length));

        for (int i = 0; i < expectedIds.Length; i++)
        {
            Assert.That(result![i].Id,
                Is.EqualTo(expectedIds[i]));
        }
    }

    private static void XTrimRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XTrim_MaxLen_Exact()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.EqualTo(3));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }

        this.XTrimAssertEntries(key, "6-0", "5-0", "4-0");
    }

    [Test]
    public void XTrim_MaxLen_EqualLength()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(6, TrimOperator.Exact),
            DeletionPolicy.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.Zero);
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(6));
        }
    }

    [Test]
    public void XTrim_MaxLen_GreaterThanLength()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(100, TrimOperator.Exact),
            DeletionPolicy.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.Zero);
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(6));
        }
    }

    [Test]
    public void XTrim_MaxLen_Zero()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(0, TrimOperator.Exact),
            DeletionPolicy.None);

        Assert.That(deleted, Is.EqualTo(6));

        this.XTrimAssertEntries(key);
    }

    [Test]
    public void XTrim_MinId_Exact()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMinId("4-0", TrimOperator.Exact),
            DeletionPolicy.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.EqualTo(3));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }

        // MINID 是包含边界：4-0 保留。
        this.XTrimAssertEntries(key, "6-0", "5-0", "4-0");
    }

    [Test]
    public void XTrim_MinId_Zero()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMinId("0-0", TrimOperator.Exact),
            DeletionPolicy.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.Zero);
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(6));
        }
    }

    [Test]
    public void XTrim_MinId_GreaterThanAllEntries()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMinId("100-0", TrimOperator.Exact),
            DeletionPolicy.None);

        Assert.That(deleted, Is.EqualTo(6));

        this.XTrimAssertEntries(key);
    }

    [Test]
    public void XTrim_MinId_SameTimestamp()
    {
        var key = XTrimKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(
                    key, "100-0", "field", "a"), Is.EqualTo("100-0"));
            Assert.That(this._redis.Stream.XAdd(
                key, "100-1", "field", "b"), Is.EqualTo("100-1"));
            Assert.That(this._redis.Stream.XAdd(
                key, "100-2", "field", "c"), Is.EqualTo("100-2"));
            Assert.That(this._redis.Stream.XAdd(
                key, "101-0", "field", "d"), Is.EqualTo("101-0"));
        }

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMinId("100-2", TrimOperator.Exact),
            DeletionPolicy.None);

        Assert.That(deleted, Is.EqualTo(2));
        this.XTrimAssertEntries(key, "101-0", "100-2");
    }

    [Test]
    public void XTrim_MaxLen_Approximate()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key, 100);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(50, TrimOperator.Approximate),
            DeletionPolicy.None);

        var length = this._redis.Stream.XLen(key);

        // 近似裁剪允许保留超过阈值的消息。
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.GreaterThanOrEqualTo(0));
            Assert.That(deleted, Is.LessThanOrEqualTo(50));
            Assert.That(length, Is.EqualTo(100 - deleted));
            Assert.That(length, Is.GreaterThanOrEqualTo(50));
        }
    }

    [Test]
    public void XTrim_MinId_Approximate()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key, 100);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMinId("51-0", TrimOperator.Approximate),
            DeletionPolicy.None);

        var length = this._redis.Stream.XLen(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.GreaterThanOrEqualTo(0));
            Assert.That(deleted, Is.LessThanOrEqualTo(50));
            Assert.That(length, Is.EqualTo(100 - deleted));
        }
    }

    [Test]
    public void XTrim_Approximate_Limit()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key, 100);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(
                20, TrimOperator.Approximate, limit: 10),
            DeletionPolicy.None);

        // LIMIT 限制扫描的消息数量；
        // 近似裁剪可能一条也不删除。
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.GreaterThanOrEqualTo(0));
            Assert.That(deleted, Is.LessThanOrEqualTo(10));
            Assert.That(this._redis.Stream.XLen(key),
                Is.EqualTo(100 - deleted));
        }
    }

    [Test]
    public void XTrim_KeepRef_PendingPreserved()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreate(
            key, "group1", "0-0"), Is.True);

        var read = this._redis.Stream.XReadGroup(
            key, ">", "group1", "consumer1", count: 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(read!.Value.Entries, Has.Length.EqualTo(3));
        }

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.KeepRef);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.EqualTo(3));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }

        var pending = this._redis.Stream.XPending(key, "group1");

        // 消息内容已被修剪，但 PEL 引用保留。
        Assert.That(pending.Count, Is.EqualTo(3));
        this.XTrimAssertEntries(key, "6-0", "5-0", "4-0");
    }

    [Test]
    public void XTrim_DelRef_PendingRemoved()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreate(
            key, "group1", "0-0"), Is.True);

        var read = this._redis.Stream.XReadGroup(
            key, ">", "group1", "consumer1", count: 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(read!.Value.Entries, Has.Length.EqualTo(3));
        }

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.DelRef);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.EqualTo(3));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(3));
        }

        var pending = this._redis.Stream.XPending(key, "group1");

        Assert.That(pending.Count, Is.Zero);
        this.XTrimAssertEntries(key, "6-0", "5-0", "4-0");
    }

    [Test]
    public void XTrim_DelRef_MultipleGroups()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreate(
                    key, "group1", "0-0"), Is.True);
            Assert.That(this._redis.Stream.XGroupCreate(
                key, "group2", "0-0"), Is.True);
        }

        var read1 = this._redis.Stream.XReadGroup(
            key, ">", "group1", "consumer1", count: 3);
        var read2 = this._redis.Stream.XReadGroup(
            key, ">", "group2", "consumer2", count: 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read1.HasValue, Is.True);
            Assert.That(read2.HasValue, Is.True);
        }

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.DelRef);

        Assert.That(deleted, Is.EqualTo(3));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                this._redis.Stream.XPending(key, "group1").Count,
                Is.Zero);

            Assert.That(
                this._redis.Stream.XPending(key, "group2").Count,
                Is.Zero);
        }
    }

    [Test]
    public void XTrim_Acked_OnlyAcknowledgedEntries()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        Assert.That(this._redis.Stream.XGroupCreate(
            key, "group1", "0-0"), Is.True);

        var read = this._redis.Stream.XReadGroup(
            key, ">", "group1", "consumer1", count: 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(read!.Value.Entries, Has.Length.EqualTo(3));

            Assert.That(this._redis.Stream.XAck(
                key, "group1", "1-0"), Is.EqualTo(1));
            Assert.That(this._redis.Stream.XAck(
                key, "group1", "2-0"), Is.EqualTo(1));
        }

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(2, TrimOperator.Exact),
            DeletionPolicy.Acked);

        using (Assert.EnterMultipleScope())
        {
            // 1-0、2-0 已确认，3-0 仍处于 PEL。
            Assert.That(deleted, Is.EqualTo(2));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(4));
        }

        this.XTrimAssertEntries(
            key, "6-0", "5-0", "4-0", "3-0");

        Assert.That(
            this._redis.Stream.XPending(key, "group1").Count,
            Is.EqualTo(1));
    }

    [Test]
    public void XTrim_Acked_MultipleGroups()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XGroupCreate(
                    key, "group1", "0-0"), Is.True);
            Assert.That(this._redis.Stream.XGroupCreate(
                key, "group2", "0-0"), Is.True);
        }

        var read1 = this._redis.Stream.XReadGroup(
            key, ">", "group1", "consumer1", count: 3);
        var read2 = this._redis.Stream.XReadGroup(
            key, ">", "group2", "consumer2", count: 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read1.HasValue, Is.True);
            Assert.That(read2.HasValue, Is.True);

            // 1-0 在两个消费组中都确认。
            Assert.That(this._redis.Stream.XAck(
                key, "group1", "1-0"), Is.EqualTo(1));
            Assert.That(this._redis.Stream.XAck(
                key, "group2", "1-0"), Is.EqualTo(1));

            // 2-0 只在 group1 中确认。
            Assert.That(this._redis.Stream.XAck(
                key, "group1", "2-0"), Is.EqualTo(1));
        }

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(2, TrimOperator.Exact),
            DeletionPolicy.Acked);

        Assert.That(deleted, Is.EqualTo(1));

        this.XTrimAssertEntries(
            key, "6-0", "5-0", "4-0", "3-0", "2-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                this._redis.Stream.XPending(key, "group1").Count,
                Is.EqualTo(1));

            Assert.That(
                this._redis.Stream.XPending(key, "group2").Count,
                Is.EqualTo(2));
        }
    }

    [Test]
    public void XTrim_MissingKey()
    {
        var key = XTrimKey();

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.None);

        Assert.That(deleted, Is.Zero);
    }

    [Test]
    public void XTrim_Flags_None()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.None,
            flags: CommandFlags.None);

        Assert.That(deleted, Is.EqualTo(3));
    }

    [Test]
    public void XTrim_CancellationToken_Default()
    {
        var key = XTrimKey();
        this.XTrimPrepare(key);

        var deleted = this._redis.Stream.XTrim(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.None,
            cancellationToken: CancellationToken.None);

        Assert.That(deleted, Is.EqualTo(3));
    }

    [Test]
    public void XTrim_WrongKeyType()
    {
        var key = XTrimKey();
        this._redis.String.Set(key, "hello");

        XTrimRedisError<RedisErrorException>(() =>
            this._redis.Stream.XTrim(
                key,
                TrimOptions.ByMaxLen(3, TrimOperator.Exact),
                DeletionPolicy.None));
    }

    #endregion

    #region XIdmpRecord

    private static string XIdmpRecordKey() =>
        $"Stream:{{XIdmpRecord}}:{Guid.NewGuid():N}";

    private void XIdmpRecordPrepare(string key)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XAdd(
                    key, "1-0", "field", "value1"), Is.EqualTo("1-0"));

            Assert.That(this._redis.Stream.XAdd(
                key, "2-0", "field", "value2"), Is.EqualTo("2-0"));

            Assert.That(this._redis.Stream.XAdd(
                key, "3-0", "field", "value3"), Is.EqualTo("3-0"));
        }
    }

    private static void XIdmpRecordRedisError<TError>(
        Action action) where TError : Exception
    {
        Assert.That(action, Throws.TypeOf<TError>());
    }

    [Test]
    public void XIdmpRecord_Basic()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XIdmpRecord(
                    key, "producer1", "iid1", "1-0"), Is.True);

            Assert.That(this._redis.Stream.XLen(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public void XIdmpRecord_SameMapping_Idempotent()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        Assert.That(this._redis.Stream.XIdmpRecord(
            key, "producer1", "iid1", "1-0"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            // 相同 pid、iid、streamId 再次记录，属于幂等操作。
            Assert.That(this._redis.Stream.XIdmpRecord(
                key, "producer1", "iid1", "1-0"), Is.True);

            Assert.That(this._redis.Stream.XLen(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public void XIdmpRecord_MultipleProducers()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XIdmpRecord(
                    key, "producer1", "iid1", "1-0"), Is.True);

            Assert.That(this._redis.Stream.XIdmpRecord(
                key, "producer2", "iid1", "2-0"), Is.True);

            // 不同生产者可以使用相同 iid。
            Assert.That(this._redis.Stream.XIdmpRecord(
                key, "producer3", "iid1", "3-0"), Is.True);

            Assert.That(this._redis.Stream.XLen(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public void XIdmpRecord_SameProducer_MultipleIids()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Stream.XIdmpRecord(
                    key, "producer1", "iid1", "1-0"), Is.True);

            Assert.That(this._redis.Stream.XIdmpRecord(
                key, "producer1", "iid2", "2-0"), Is.True);

            Assert.That(this._redis.Stream.XIdmpRecord(
                key, "producer1", "iid3", "3-0"), Is.True);

            Assert.That(this._redis.Stream.XLen(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public void XIdmpRecord_SameMappingDifferentStreamId()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        Assert.That(this._redis.Stream.XIdmpRecord(
            key, "producer1", "iid1", "1-0"), Is.True);

        // 相同 pid + iid 不允许映射到不同消息。
        XIdmpRecordRedisError<RedisErrorException>(() =>
            this._redis.Stream.XIdmpRecord(
                key, "producer1", "iid1", "2-0"));

        // 失败后原映射仍应有效。
        Assert.That(this._redis.Stream.XIdmpRecord(
            key, "producer1", "iid1", "1-0"), Is.True);
    }

    [Test]
    public void XIdmpRecord_StreamIdNotExists()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        XIdmpRecordRedisError<RedisErrorException>(() =>
            this._redis.Stream.XIdmpRecord(
                key, "producer1", "iid1", "100-0"));

        Assert.That(this._redis.Stream.XLen(key),
            Is.EqualTo(3));
    }

    [Test]
    public void XIdmpRecord_DeletedStreamId()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        Assert.That(this._redis.Stream.XDel(
            key, "2-0"), Is.EqualTo(1));

        XIdmpRecordRedisError<RedisErrorException>(() =>
            this._redis.Stream.XIdmpRecord(
                key, "producer1", "iid1", "2-0"));

        Assert.That(this._redis.Stream.XLen(key),
            Is.EqualTo(2));
    }

    [Test]
    public void XIdmpRecord_EmptyProducerId()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        XIdmpRecordRedisError<RedisErrorException>(() =>
            this._redis.Stream.XIdmpRecord(
                key, "", "iid1", "1-0"));
    }

    [Test]
    public void XIdmpRecord_EmptyIdempotencyId()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        XIdmpRecordRedisError<RedisErrorException>(() =>
            this._redis.Stream.XIdmpRecord(
                key, "producer1", "", "1-0"));
    }

    [Test]
    public void XIdmpRecord_BothIdsEmpty()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        XIdmpRecordRedisError<RedisErrorException>(() =>
            this._redis.Stream.XIdmpRecord(
                key, "", "", "1-0"));
    }

    [Test]
    public void XIdmpRecord_MissingKey()
    {
        var key = XIdmpRecordKey();

        XIdmpRecordRedisError<RedisErrorException>(() =>
            this._redis.Stream.XIdmpRecord(
                key, "producer1", "iid1", "1-0"));
    }

    [Test]
    public void XIdmpRecord_WrongKeyType()
    {
        var key = XIdmpRecordKey();

        this._redis.String.Set(key, "hello");

        XIdmpRecordRedisError<RedisErrorException>(() =>
            this._redis.Stream.XIdmpRecord(
                key, "producer1", "iid1", "1-0"));
    }

    [Test]
    public void XIdmpRecord_Flags_None()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        Assert.That(this._redis.Stream.XIdmpRecord(
            key, "producer1", "iid1", "1-0",
            flags: CommandFlags.None), Is.True);
    }

    [Test]
    public void XIdmpRecord_CancellationToken_Default()
    {
        var key = XIdmpRecordKey();
        this.XIdmpRecordPrepare(key);

        Assert.That(this._redis.Stream.XIdmpRecord(
            key, "producer1", "iid1", "1-0",
            cancellationToken: CancellationToken.None), Is.True);
    }

    #endregion
}
