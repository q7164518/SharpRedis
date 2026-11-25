using SharpRedis;
using SharpRedis.Exceptions;
using static SharpRedis.RedisStream;

namespace NET10.Stream;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class StreamAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region XDel
    private static string XDelKey() => $"Stream:{{XDelAsync}}:{Guid.NewGuid():N}";

    private async Task XDelSeedAsync(string key)
    {
        await this._redis.Stream.XAddAsync(key, "1-0", "f", "v1").ConfigureAwait(false);
        await this._redis.Stream.XAddAsync(key, "2-0", "f", "v2").ConfigureAwait(false);
        await this._redis.Stream.XAddAsync(key, "3-0", "f", "v3").ConfigureAwait(false);
        await this._redis.Stream.XAddAsync(key, "4-0", "f", "v4").ConfigureAwait(false);
        await this._redis.Stream.XAddAsync(key, "5-0", "f", "v5").ConfigureAwait(false);
    }

    private async Task XDelAssertLengthAsync(string key, long expected)
    {
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(expected));
    }

    private static async Task XDelRedisErrorAsync(Func<Task> action)
    {
        await Assert.ThrowsAsync<RedisErrorException>(action);
    }

    [Test]
    public async Task XDel_AllOverloadsAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(key, "1-0"),
            Is.EqualTo(1));

        ICollection<EntryId> collection = ["2-0", "3-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelAsync(key, collection),
                    Is.EqualTo(2));

            Assert.That(await this._redis.Stream.XDelAsync(key, ["4-0", "5-0"]),
                Is.EqualTo(2));
        }

        await this.XDelAssertLengthAsync(key, 0);
    }

    [Test]
    public async Task XDel_SingleId_SuccessAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(key, "3-0"),
            Is.EqualTo(1));

        await this.XDelAssertLengthAsync(key, 4);
    }

    [Test]
    public async Task XDel_SingleId_NotExistsAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);
        Assert.That(await this._redis.Stream.XDelAsync(key, "100-0"), Is.Zero);
        await this.XDelAssertLengthAsync(key, 5);
    }

    [Test]
    public async Task XDel_SingleId_DeleteTwiceAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(key, "2-0"),
            Is.EqualTo(1));

        Assert.That(await this._redis.Stream.XDelAsync(key, "2-0"),
            Is.Zero);

        await this.XDelAssertLengthAsync(key, 4);
    }

    [Test]
    public async Task XDel_Collection_DeleteMultipleAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        ICollection<EntryId> ids = ["1-0", "3-0", "5-0"];

        Assert.That(await this._redis.Stream.XDelAsync(key, ids),
            Is.EqualTo(3));

        await this.XDelAssertLengthAsync(key, 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelAsync(key, "2-0"),
                    Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XDelAsync(key, "4-0"),
                Is.EqualTo(1));
        }

        await this.XDelAssertLengthAsync(key, 0);
    }

    [Test]
    public async Task XDel_Collection_MixedExistingAndMissingAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        ICollection<EntryId> ids =
        [
            "1-0",
            "100-0",
            "3-0",
            "200-0"
        ];

        Assert.That(await this._redis.Stream.XDelAsync(key, ids),
            Is.EqualTo(2));

        await this.XDelAssertLengthAsync(key, 3);
    }

    [Test]
    public async Task XDel_Collection_DuplicateIdsAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        ICollection<EntryId> ids =
        [
            "1-0",
            "1-0",
            "2-0",
            "2-0"
        ];

        Assert.That(await this._redis.Stream.XDelAsync(key, ids),
            Is.EqualTo(2));

        await this.XDelAssertLengthAsync(key, 3);
    }

    [Test]
    public async Task XDel_Collection_EmptyAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        ICollection<EntryId> ids = Array.Empty<EntryId>();
        await Assert.ThrowsAsync<NotSupportedException>(() => this._redis.Stream.XDelAsync(key, ids));
        ids = null!;
        await Assert.ThrowsAsync<NotSupportedException>(() => this._redis.Stream.XDelAsync(key, ids));
        await this.XDelAssertLengthAsync(key, 5);
    }

    [Test]
    public async Task XDel_Span_DeleteMultipleAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(key, ["1-0", "2-0", "3-0"]),
            Is.EqualTo(3));

        await this.XDelAssertLengthAsync(key, 2);
    }

    [Test]
    public async Task XDel_Span_MixedExistingAndMissingAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(key, ["1-0", "100-0", "2-0", "200-0"]),
            Is.EqualTo(2));

        await this.XDelAssertLengthAsync(key, 3);
    }

    [Test]
    public async Task XDel_Span_DuplicateIdsAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(key, ["1-0", "1-0", "3-0", "3-0"]),
            Is.EqualTo(2));

        await this.XDelAssertLengthAsync(key, 3);
    }

    [Test]
    public async Task XDel_Span_EmptyAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);
        await Assert.ThrowsAsync<NotSupportedException>(() =>
        {
            ReadOnlySpan<EntryId> span = default;
            return this._redis.Stream.XDelAsync(key, span);
        });
        await Assert.ThrowsAsync<NotSupportedException>(() => this._redis.Stream.XDelAsync(key, []));
        await this.XDelAssertLengthAsync(key, 5);
    }

    [Test]
    public async Task XDel_KeyNotExistsAsync()
    {
        var key = XDelKey();

        Assert.That(await this._redis.Stream.XDelAsync(key, "1-0"),
            Is.Zero);

        ICollection<EntryId> collection = ["1-0", "2-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelAsync(key, collection),
                    Is.Zero);

            Assert.That(await this._redis.Stream.XDelAsync(key, ["1-0", "2-0"]),
                Is.Zero);
        }

        await this.XDelAssertLengthAsync(key, 0);
    }

    [Test]
    public async Task XDel_WrongKeyTypeAsync()
    {
        var key = XDelKey();

        this._redis.String.Set(key, "hello");

        await XDelRedisErrorAsync(() => this._redis.Stream.XDelAsync(
            key, "1-0"));

        ICollection<EntryId> collection = ["1-0"];

        await XDelRedisErrorAsync(() => this._redis.Stream.XDelAsync(
            key, collection));

        await XDelRedisErrorAsync(() => this._redis.Stream.XDelAsync(
            key, ["1-0"]));
    }

    [Test]
    public async Task XDel_InvalidId_AsteriskAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        await XDelRedisErrorAsync(() => this._redis.Stream.XDelAsync(
            key, "*"));

        await this.XDelAssertLengthAsync(key, 5);
    }

    [Test]
    public async Task XDel_InvalidId_PartialIdAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        await XDelRedisErrorAsync(() => this._redis.Stream.XDelAsync(
            key, "1-*"));

        await this.XDelAssertLengthAsync(key, 5);
    }

    [Test]
    public async Task XDel_InvalidId_OverflowAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);
        Assert.That(() => this._redis.Stream.XDelAsync(key, "18446744073709551616-0"), Throws.TypeOf<ArgumentException>());

        await this.XDelAssertLengthAsync(key, 5);
    }

    [Test]
    public async Task XDel_Id_ZeroZeroAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(
            key, "0-0"), Is.Zero);

        await this.XDelAssertLengthAsync(key, 5);
    }

    [Test]
    public async Task XDel_DeletedId_CannotBeReusedAsync()
    {
        var key = XDelKey();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                    key, "1-0", "f", "v"), Is.EqualTo("1-0"));

            Assert.That(await this._redis.Stream.XAddAsync(
                key, "2-0", "f", "v"), Is.EqualTo("2-0"));

            Assert.That(await this._redis.Stream.XDelAsync(
                key, "2-0"), Is.EqualTo(1));
        }

        await this.XDelAssertLengthAsync(key, 1);

        await XDelRedisErrorAsync(() => this._redis.Stream.XAddAsync(
            key, "2-0", "f", "v"));

        await XDelRedisErrorAsync(() => this._redis.Stream.XAddAsync(
            key, "1-1", "f", "v"));

        Assert.That(await this._redis.Stream.XAddAsync(
            key, "2-1", "f", "v"), Is.EqualTo("2-1"));

        await this.XDelAssertLengthAsync(key, 2);
    }

    [Test]
    public async Task XDel_DeleteAll_ThenAddNewEntryAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        ICollection<EntryId> ids =
        [
            "1-0", "2-0", "3-0", "4-0", "5-0"
        ];

        Assert.That(await this._redis.Stream.XDelAsync(key, ids),
            Is.EqualTo(5));

        await this.XDelAssertLengthAsync(key, 0);

        await XDelRedisErrorAsync(() => this._redis.Stream.XAddAsync(
            key, "5-0", "f", "v"));

        Assert.That(await this._redis.Stream.XAddAsync(
            key, "6-0", "f", "v"), Is.EqualTo("6-0"));

        await this.XDelAssertLengthAsync(key, 1);
    }

    [Test]
    public async Task XDel_EntryId_DifferentConstructorsAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelAsync(
                    key, new EntryId("1-0")), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XDelAsync(
                key, new EntryId(2UL, 0UL)), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XDelAsync(
                key, EntryId.Create("3-0")), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XDelAsync(
                key, EntryId.Create(4UL, 0UL)), Is.EqualTo(1));
        }

        EntryId id = (5UL, 0UL);

        Assert.That(await this._redis.Stream.XDelAsync(key, id),
            Is.EqualTo(1));

        await this.XDelAssertLengthAsync(key, 0);
    }

    [Test]
    public async Task XDel_Collection_ListAndArrayAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        List<EntryId> list = ["1-0", "2-0"];
        EntryId[] array = ["3-0", "4-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelAsync(key, list),
                    Is.EqualTo(2));

            Assert.That(await this._redis.Stream.XDelAsync(key, array),
                Is.EqualTo(2));
        }

        await this.XDelAssertLengthAsync(key, 1);
    }

    [Test]
    public async Task XDel_Flags_NoneAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(
            key,
            "1-0",
            flags: CommandFlags.None), Is.EqualTo(1));

        await this.XDelAssertLengthAsync(key, 4);
    }

    [Test]
    public async Task XDel_CancellationToken_DefaultAsync()
    {
        var key = XDelKey();
        await this.XDelSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(
            key,
            "1-0",
            cancellationToken: CancellationToken.None),
            Is.EqualTo(1));

        await this.XDelAssertLengthAsync(key, 4);
    }

    #endregion

    #region XAck

    private static string XAckKeyAsync() =>
        $"Stream:{{XAckAsync}}:{Guid.NewGuid():N}";

    private static async Task XAckRedisErrorAsync(Func<Task> action)
    {
        await Assert.ThrowsAsync<RedisErrorException>(action);
    }

    private async Task XAckPrepareGroupAsync(string key, string group = "group1")
    {
        await this._redis.Stream.XAddAsync(key, "1-0", "name", "v1");
        await this._redis.Stream.XAddAsync(key, "2-0", "name", "v2");
        await this._redis.Stream.XAddAsync(key, "3-0", "name", "v3");
        await this._redis.Stream.XAddAsync(key, "4-0", "name", "v4");
        await this._redis.Stream.XAddAsync(key, "5-0", "name", "v5");

        await this._redis.Stream.XGroupCreateAsync(key, group, "0-0");
    }

    private async Task XAckPreparePendingAsync(string key, string group = "group1")
    {
        await this.XAckPrepareGroupAsync(key, group);

        await this._redis.Stream.XReadGroupAsync(key, ">", group, "consumer1");
    }

    [Test]
    public async Task XAck_SingleId_SuccessAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(key, "group1", (EntryId)"1-0"), Is.EqualTo(1));
            Assert.That(this._redis.Stream.XLen(key), Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XAck_SingleId_RepeatAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", (EntryId)"1-0"), Is.EqualTo(1));
        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", (EntryId)"1-0"), Is.Zero);
    }

    [Test]
    public async Task XAck_SingleId_NotExistsAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", (EntryId)"100-0"), Is.Zero);
    }

    [Test]
    public async Task XAck_SingleId_NotPendingAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPrepareGroupAsync(key);

        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", "1-0"), Is.Zero);
    }

    [Test]
    public async Task XAck_Collection_MultipleAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        ICollection<EntryId> ids = ["1-0", "2-0", "3-0"];
        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", ids), Is.EqualTo(3));
    }

    [Test]
    public async Task XAck_Collection_MixedAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        ICollection<EntryId> ids = ["1-0", "2-0", "100-0"];

        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", ids), Is.EqualTo(2));

        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", ids), Is.Zero);
    }

    [Test]
    public async Task XAck_Collection_DuplicateAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        ICollection<EntryId> ids = ["1-0", "1-0", "2-0", "2-0"];

        Assert.That(await this._redis.Stream.XAckAsync(
            key, "group1", ids), Is.EqualTo(2));
    }

    [Test]
    public async Task XAck_Collection_EmptyAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        ICollection<EntryId> ids = [];

        Assert.That(async () => await this._redis.Stream.XAckAsync(key, "group1", ids), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public async Task XAck_Span_MultipleAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        Assert.That(await this._redis.Stream.XAckAsync(
            key, "group1", ["1-0", "2-0", "3-0"]),
            Is.EqualTo(3));
    }

    [Test]
    public async Task XAck_Span_MixedAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        Assert.That(await this._redis.Stream.XAckAsync(
            key, "group1", ["1-0", "100-0", "2-0"]),
            Is.EqualTo(2));
    }

    [Test]
    public async Task XAck_Span_DuplicateAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        Assert.That(await this._redis.Stream.XAckAsync(
            key, "group1", (ReadOnlySpan<EntryId>)["1-0", "1-0", "2-0"]),
            Is.EqualTo(2));
    }

    [Test]
    public async Task XAck_Span_EmptyAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        Assert.That(async () => await this._redis.Stream.XAckAsync(key, "group1", []), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public async Task XAck_GroupNotExistsAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPrepareGroupAsync(key);

        Assert.That(await this._redis.Stream.XAckAsync(key, "not_exists", "1-0"), Is.Zero);
    }

    [Test]
    public async Task XAck_StreamNotExistsAsync()
    {
        var key = XAckKeyAsync();

        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", (EntryId)"1-0"), Is.Zero);
    }

    [Test]
    public async Task XAck_WrongKeyTypeAsync()
    {
        var key = XAckKeyAsync();
        this._redis.String.Set(key, "hello");

        await XAckRedisErrorAsync(async () =>
            await this._redis.Stream.XAckAsync(
                key, "group1", (EntryId)"1-0"));
    }

    [Test]
    public async Task XAck_InvalidId_AsteriskAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        await XAckRedisErrorAsync(async () =>
            await this._redis.Stream.XAckAsync(
                key, "group1", (EntryId)"*"));
    }

    [Test]
    public async Task XAck_InvalidId_PartialAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        await XAckRedisErrorAsync(async () =>
            await this._redis.Stream.XAckAsync(
                key, "group1", (EntryId)"1-*"));
    }

    [Test]
    public async Task XAck_Id_ZeroZeroAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        Assert.That(await this._redis.Stream.XAckAsync(
            key, "group1", (EntryId)"0-0"), Is.Zero);
    }

    [Test]
    public async Task XAck_Flags_NoneAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        Assert.That(await this._redis.Stream.XAckAsync(
            key, "group1", (EntryId)"1-0",
            flags: CommandFlags.None), Is.EqualTo(1));
    }

    [Test]
    public async Task XAck_CancellationToken_DefaultAsync()
    {
        var key = XAckKeyAsync();
        await this.XAckPreparePendingAsync(key);

        Assert.That(await this._redis.Stream.XAckAsync(
            key, "group1", (EntryId)"1-0",
            cancellationToken: CancellationToken.None), Is.EqualTo(1));
    }

    #endregion

    #region XRead / XReadGroup
    private static string XReadKeyAsync() => $"Stream:{{XReadAsync}}:{Guid.NewGuid():N}";
    private static string XReadGroupKeyAsync() => $"Stream:{{XReadGroupAsync}}:{Guid.NewGuid():N}";
    private static async Task XReadRedisErrorAsync(Func<Task> action) { await Assert.ThrowsAsync<RedisErrorException>(action); }
    private static async Task XReadGroupRedisErrorAsync(Func<Task> action) { await Assert.ThrowsAsync<RedisErrorException>(action); }

    private async Task XReadSeedAsync(string key)
    {
        for (int i = 1; i <= 5; i++)
            Assert.That(await this._redis.Stream.XAddAsync(key, $"{i}-0", "name", $"v{i}"), Is.EqualTo($"{i}-0"));
    }
    private async Task XReadGroupSeedAsync(string key, string group = "group1", string start = "0-0")
    {
        await this.XReadSeedAsync(key);
        Assert.That(await this._redis.Stream.XGroupCreateAsync(key, group, start), Is.True);
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
    public async Task XRead_Single_AllEntriesAsync()
    {
        var key = XReadKeyAsync(); await this.XReadSeedAsync(key);
        XReadAssert(await this._redis.Stream.XReadAsync(key, "0-0"), key, "1-0", "2-0", "3-0", "4-0", "5-0");
    }
    [Test]
    public async Task XRead_Count_And_IdExclusiveAsync()
    {
        var key = XReadKeyAsync(); await this.XReadSeedAsync(key);
        XReadAssert(await this._redis.Stream.XReadAsync(key, "2-0", count: 2), key, "3-0", "4-0");
        XReadAssert(await this._redis.Stream.XReadAsync(key, "4-0", count: 10), key, "5-0");
        Assert.That(await this._redis.Stream.XReadAsync(key, "5-0"), Is.Null);
    }
    [Test]
    public async Task XRead_NoStream_And_NoNewEntriesAsync()
    {
        var key = XReadKeyAsync();
        Assert.That(await this._redis.Stream.XReadAsync(key, "0-0"), Is.Null);
        await this.XReadSeedAsync(key);
        Assert.That(await this._redis.Stream.XReadAsync(key, "$"), Is.Null);
    }
    [Test]
    public async Task XRead_FieldValues_UnicodeAsync()
    {
        var key = XReadKeyAsync();
        await this._redis.Stream.XAddAsync(key, "1-0", "姓名", "张三");
        var result = await this._redis.Stream.XReadAsync(key, "0-0");
        XReadAssert(result, key, "1-0");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.Value.Entries![0].FieldValue![0].Key.ToString(), Is.EqualTo("姓名"));
            Assert.That(result.Value.Entries[0].FieldValue![0].Value.ToString(), Is.EqualTo("张三"));
        }
    }
    [Test]
    public async Task XRead_Block_TimeoutAsync()
    {
        var key = XReadKeyAsync();
        Assert.That(await this._redis.Stream.XReadAsync(key, "0-0", block: 30), Is.Null);
    }
    [Test]
    public async Task XRead_Collection_KvpAsync()
    {
        var a = XReadKeyAsync();
        var b = XReadKeyAsync();
        await this.XReadSeedAsync(a);
        await this.XReadSeedAsync(b);
        ICollection<KeyValuePair<SharpValue, EntryId>> keys = [new(a, "3-0"), new(b, "4-0")];
        XReadAssertMany(await this._redis.Stream.XReadAsync(keys), (a, ["4-0", "5-0"]), (b, ["5-0"]));
    }
    [Test]
    public async Task XRead_Span_KvpAsync()
    {
        var a = XReadKeyAsync();
        var b = XReadKeyAsync();
        await this.XReadSeedAsync(a);
        await this.XReadSeedAsync(b);
        XReadAssertMany(await this._redis.Stream.XReadAsync((ReadOnlySpan<KeyValuePair<SharpValue, EntryId>>)[new(a, "4-0"), new(b, "3-0")]), (a, ["5-0"]), (b, ["4-0", "5-0"]));
    }
    [Test]
    public async Task XRead_Collection_TupleAsync()
    {
        var a = XReadKeyAsync();
        var b = XReadKeyAsync();
        await this.XReadSeedAsync(a);
        await this.XReadSeedAsync(b);
        ICollection<(SharpValue key, EntryId id)> keys = [(a, "4-0"), (b, "4-0")];
        XReadAssertMany(await this._redis.Stream.XReadAsync(keys), (a, ["5-0"]), (b, ["5-0"]));
    }
    [Test]
    public async Task XRead_Span_TupleAsync()
    {
        var a = XReadKeyAsync();
        var b = XReadKeyAsync();
        await this.XReadSeedAsync(a);
        await this.XReadSeedAsync(b);
        XReadAssertMany(await this._redis.Stream.XReadAsync([(a, "3-0"), (b, "4-0")], count: 1), (a, ["4-0"]), (b, ["5-0"]));
    }
    [Test]
    public async Task XRead_Multiple_SomeStreamsEmptyAsync()
    {
        var a = XReadKeyAsync();
        var b = XReadKeyAsync();
        await this.XReadSeedAsync(a);
        ICollection<(SharpValue key, EntryId id)> keys = [(a, "4-0"), (b, "0-0")];
        XReadAssertMany(await this._redis.Stream.XReadAsync(keys), (a, ["5-0"]));
    }
    [Test]
    public async Task XRead_Flags_And_CancellationTokenAsync()
    {
        var key = XReadKeyAsync(); await this.XReadSeedAsync(key);
        XReadAssert(await this._redis.Stream.XReadAsync(key, "4-0", flags: CommandFlags.None, cancellationToken: CancellationToken.None), key, "5-0");
    }
    [Test]
    public async Task XRead_WrongTypeAsync()
    {
        var key = XReadKeyAsync(); await this._redis.String.SetAsync(key, "not-stream");
        await XReadRedisErrorAsync(async () => await this._redis.Stream.XReadAsync(key, "0-0"));
    }
    [Test]
    public async Task XRead_InvalidIdAsync()
    {
        var key = XReadKeyAsync(); await this.XReadSeedAsync(key);
        await XReadRedisErrorAsync(async () => await this._redis.Stream.XReadAsync(key, "*"));
    }

    [Test]
    public async Task XReadGroup_Single_NewMessages_PEL_And_AckAsync()
    {
        var key = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(key);
        XReadAssert(await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice", count: 2), key, "1-0", "2-0");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(key, "group1", (EntryId)"1-0"), Is.EqualTo(1));
            Assert.That(await this._redis.Stream.XAckAsync(key, "group1", (EntryId)"2-0"), Is.EqualTo(1));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
    }
    [Test]
    public async Task XReadGroup_GroupStartDollarAsync()
    {
        var key = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(key, start: "$");
        Assert.That(await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice"), Is.Null);
        await this._redis.Stream.XAddAsync(key, "6-0", "name", "v6");
        XReadAssert(await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice"), key, "6-0");
    }
    [Test]
    public async Task XReadGroup_Pending_ReReadOwnHistoryAsync()
    {
        var key = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(key);
        XReadAssert(await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice", count: 2), key, "1-0", "2-0");
        XReadAssert(await this._redis.Stream.XReadGroupAsync(key, "0", "group1", "alice"), key, "1-0", "2-0");
        Assert.That((await this._redis.Stream.XReadGroupAsync(key, "0", "group1", "bob"))!.Value.Entries, Is.Null);
    }
    [Test]
    public async Task XReadGroup_NoAck_NotPendingAsync()
    {
        var key = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(key);
        XReadAssert(await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice", count: 2, noack: true), key, "1-0", "2-0");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(key, "group1", (EntryId)"1-0"), Is.Zero);
            Assert.That((await this._redis.Stream.XReadGroupAsync(key, "0", "group1", "alice"))!.Value.Entries, Is.Null);
        }
    }
    [Test]
    public async Task XReadGroup_NewMessages_AreNotRedeliveredAsync()
    {
        var key = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(key);
        XReadAssert(await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice", count: 5), key, "1-0", "2-0", "3-0", "4-0", "5-0");
        Assert.That(await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "bob"), Is.Null);
    }
    [Test]
    public async Task XReadGroup_Collection_KvpAsync()
    {
        var a = XReadGroupKeyAsync(); var b = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(a); await this.XReadGroupSeedAsync(b);
        ICollection<KeyValuePair<SharpValue, EntryId>> keys = [new(a, ">"), new(b, ">")];
        XReadAssertMany(await this._redis.Stream.XReadGroupAsync(keys, "group1", "alice", count: 2), (a, ["1-0", "2-0"]), (b, ["1-0", "2-0"]));
    }
    [Test]
    public async Task XReadGroup_Span_KvpAsync()
    {
        var a = XReadGroupKeyAsync(); var b = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(a); await this.XReadGroupSeedAsync(b);
        XReadAssertMany(await this._redis.Stream.XReadGroupAsync((ReadOnlySpan<KeyValuePair<SharpValue, EntryId>>)[new(a, ">"), new(b, ">")], "group1", "alice", count: 1), (a, ["1-0"]), (b, ["1-0"]));
    }
    [Test]
    public async Task XReadGroup_Collection_TupleAsync()
    {
        var a = XReadGroupKeyAsync(); var b = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(a); await this.XReadGroupSeedAsync(b);
        ICollection<(SharpValue key, EntryId id)> keys = [(a, ">"), (b, ">")];
        XReadAssertMany(await this._redis.Stream.XReadGroupAsync(keys, "group1", "alice", count: 1), (a, ["1-0"]), (b, ["1-0"]));
    }
    [Test]
    public async Task XReadGroup_Span_TupleAsync()
    {
        var a = XReadGroupKeyAsync(); var b = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(a); await this.XReadGroupSeedAsync(b);
        XReadAssertMany(await this._redis.Stream.XReadGroupAsync((ReadOnlySpan<(SharpValue key, EntryId id)>)[(a, ">"), (b, ">")], "group1", "alice", count: 1), (a, ["1-0"]), (b, ["1-0"]));
    }
    [Test]
    public async Task XReadGroup_Block_TimeoutAsync()
    {
        var key = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(key, start: "$");
        Assert.That(await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice", block: 30), Is.Null);
    }
    [Test]
    public async Task XReadGroup_GroupDoesNotExistAsync()
    {
        var key = XReadGroupKeyAsync(); await this.XReadSeedAsync(key);
        await XReadGroupRedisErrorAsync(async () => await this._redis.Stream.XReadGroupAsync(key, ">", "missing", "alice"));
    }
    [Test]
    public async Task XReadGroup_StreamDoesNotExistAsync()
    {
        var key = XReadGroupKeyAsync();
        await XReadGroupRedisErrorAsync(async () => await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice"));
    }
    [Test]
    public async Task XReadGroup_WrongTypeAsync()
    {
        var key = XReadGroupKeyAsync(); await this._redis.String.SetAsync(key, "not-stream");
        await XReadGroupRedisErrorAsync(async () => await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice"));
    }
    [Test]
    public async Task XReadGroup_InvalidIdAsync()
    {
        var key = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(key);
        await XReadGroupRedisErrorAsync(async () => await this._redis.Stream.XReadGroupAsync(key, "*", "group1", "alice"));
    }
    [Test]
    public async Task XReadGroup_Flags_And_CancellationTokenAsync()
    {
        var key = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(key);
        XReadAssert(await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice", count: 1, flags: CommandFlags.None, cancellationToken: CancellationToken.None), key, "1-0");
    }

    [Test]
    public async Task XRead_MaxCount_MaxSize_Redis810Async()
    {
        var key = XReadKeyAsync(); await this.XReadSeedAsync(key);
        var result = await this._redis.Stream.XReadAsync(key, "0-0", maxCount: 2, maxSize: 4096);
        XReadAssert(result, key, "1-0", "2-0");
    }
    [Test]
    public async Task XReadGroup_MaxCount_MaxSize_Redis810Async()
    {
        var key = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(key);
        XReadAssert(await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice", maxCount: 2, maxSize: 4096), key, "1-0", "2-0");
    }
    [Test]
    public async Task XReadGroup_Claim_Redis84Async()
    {
        var key = XReadGroupKeyAsync(); await this.XReadGroupSeedAsync(key);
        Assert.That(await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "alice", count: 1, claim: 0), Is.Not.Null);
    }
    #endregion

    #region XAckDelAsync

    private static string XAckDelKeyAsync() => $"Stream:{{XAckDelAsync}}:{Guid.NewGuid():N}";

    private static async Task XAckDelRedisErrorAsync(Func<Task> action)
    {
        await Assert.ThrowsAsync<RedisErrorException>(action);
    }

    private async Task XAckDelSeedAsync(string key)
    {
        for (int i = 1; i <= 5; i++)
            Assert.That(await this._redis.Stream.XAddAsync(key, $"{i}-0", "f", $"v{i}"), Is.EqualTo($"{i}-0"));
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
    }

    private async Task XAckDelCreateAndReadAsync(string key, string group, string consumer)
    {
        Assert.That(await this._redis.Stream.XGroupCreateAsync(key, group, "0-0"), Is.True);
        var items = await this._redis.Stream.XReadGroupAsync(key, ">", group, consumer, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(items.HasValue, Is.True);
            Assert.That(items!.Value.Entries, Has.Length.EqualTo(5));
        }
        for (int i = 0; i < 5; i++)
            Assert.That(items.Value.Entries![i].Id, Is.EqualTo($"{i + 1}-0"));
    }

    private async Task XAckDelPrepareAsync(string key)
    {
        await this.XAckDelSeedAsync(key);
        await this.XAckDelCreateAndReadAsync(key, "group1", "consumer1");
    }

    [Test]
    public async Task XAckDel_Single_DefaultAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0"), Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
            Assert.That(await this._redis.Stream.XAckAsync(key, "group1", "1-0"), Is.Zero);
        }
    }

    [Test]
    public async Task XAckDel_Single_ExplicitKeepRefAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0", delPolicy: DeletionPolicy.KeepRef),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XAckDel_Single_NotFoundAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "100-0"), Is.EqualTo(DeleteResult.NotFound));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XAckDel_Single_RepeatAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0"), Is.EqualTo(DeleteResult.Deleted));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0"), Is.EqualTo(DeleteResult.NotFound));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XAckDel_Single_NotPendingAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelSeedAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateAsync(key, "group1", "0-0"), Is.True);
            // Stream 有该 ID，但该组还没有领取过，因此没有 PEL 记录。
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0"), Is.EqualTo(DeleteResult.NotFound));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XAckDel_Collection_MultipleAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        ICollection<EntryId> ids = ["1-0", "2-0", "3-0"];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", ids), Is.EqualTo(
            [
                DeleteResult.Deleted, DeleteResult.Deleted, DeleteResult.Deleted
            ]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task XAckDel_Collection_MixedAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        ICollection<EntryId> ids = ["1-0", "100-0", "2-0"];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", ids), Is.EqualTo(
            [
                DeleteResult.Deleted, DeleteResult.NotFound, DeleteResult.Deleted
            ]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XAckDel_Span_MultipleAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", ["1-0", "2-0", "3-0"]),
                    Is.EqualTo([DeleteResult.Deleted, DeleteResult.Deleted, DeleteResult.Deleted]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task XAckDel_Span_MixedAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", (ReadOnlySpan<EntryId>)["1-0", "100-0", "2-0"]),
                    Is.EqualTo([DeleteResult.Deleted, DeleteResult.NotFound, DeleteResult.Deleted]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XAckDel_KeepRef_OtherGroupPELRemainsAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelSeedAsync(key);
        await this.XAckDelCreateAndReadAsync(key, "group1", "consumer1");
        await this.XAckDelCreateAndReadAsync(key, "group2", "consumer2");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0", delPolicy: DeletionPolicy.KeepRef),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
            Assert.That(await this._redis.Stream.XAckAsync(key, "group2", "1-0"), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAckDel_DelRef_OtherGroupPELClearedAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelSeedAsync(key);
        await this.XAckDelCreateAndReadAsync(key, "group1", "consumer1");
        await this.XAckDelCreateAndReadAsync(key, "group2", "consumer2");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0", delPolicy: DeletionPolicy.DelRef),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
            Assert.That(await this._redis.Stream.XAckAsync(key, "group2", "1-0"), Is.Zero);
        }
    }

    [Test]
    public async Task XAckDel_Acked_StillReferencedThenLastGroupDeletesAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelSeedAsync(key);
        await this.XAckDelCreateAndReadAsync(key, "group1", "consumer1");
        await this.XAckDelCreateAndReadAsync(key, "group2", "consumer2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0", delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo(DeleteResult.StillReferenced));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
            Assert.That(await this._redis.Stream.XAckAsync(key, "group1", "1-0"), Is.Zero);
        }

        using (Assert.EnterMultipleScope())
        {
            // 同一组已经确认；不应期待它第二次返回 Deleted。
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.NotFound));
            // 最后由仍有 PEL 的 group2 执行 XACKDEL。
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group2", "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XAckDel_Acked_OtherGroupAcknowledgedFirstAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelSeedAsync(key);
        await this.XAckDelCreateAndReadAsync(key, "group1", "consumer1");
        await this.XAckDelCreateAndReadAsync(key, "group2", "consumer2");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(key, "group2", "1-0"), Is.EqualTo(1));
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XAckDel_Acked_CollectionMixedAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelSeedAsync(key);
        await this.XAckDelCreateAndReadAsync(key, "group1", "consumer1");
        await this.XAckDelCreateAndReadAsync(key, "group2", "consumer2");
        Assert.That(await this._redis.Stream.XAckAsync(key, "group2", "1-0"), Is.EqualTo(1));
        ICollection<EntryId> ids = ["1-0", "2-0", "100-0"];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", ids, delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo([DeleteResult.Deleted, DeleteResult.StillReferenced, DeleteResult.NotFound]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
            // 2-0 仍由 group2 持有，可以由 group2 最后确认并删除。
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group2", "2-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.Deleted));
        }
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(3));
    }

    [Test]
    public async Task XAckDel_GroupNotExistsAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelSeedAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "missing", "1-0"), Is.EqualTo(DeleteResult.NotFound));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XAckDel_KeyNotExistsAsync()
    {
        var key = XAckDelKeyAsync();
        Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0"), Is.EqualTo(DeleteResult.NotFound));
    }

    [Test]
    public async Task XAckDel_WrongKeyTypeAsync()
    {
        var key = XAckDelKeyAsync();
        await this._redis.String.SetAsync(key, "hello");
        await XAckDelRedisErrorAsync(async () => await this._redis.Stream.XAckDelAsync(key, "group1", "1-0"));
    }

    [Test]
    public async Task XAckDel_InvalidIdAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        await XAckDelRedisErrorAsync(async () => await this._redis.Stream.XAckDelAsync(key, "group1", "1-*"));
        Assert.That(async () => await this._redis.Stream.XAckDelAsync(key, "group1", "18446744073709551616-0"), Throws.TypeOf<ArgumentException>());
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
    }

    [Test]
    public async Task XAckDel_Collection_EmptyAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        ICollection<EntryId> ids = [];
        Assert.That(async () => await this._redis.Stream.XAckDelAsync(key, "group1", ids), Throws.TypeOf<NotSupportedException>());
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
    }

    [Test]
    public async Task XAckDel_Span_EmptyAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        Assert.That(async () => await this._redis.Stream.XAckDelAsync(key, "group1", []), Throws.TypeOf<NotSupportedException>());
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
    }

    [Test]
    public async Task XAckDel_Flags_NoneAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0", flags: CommandFlags.None),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XAckDel_CancellationToken_DefaultAsync()
    {
        var key = XAckDelKeyAsync();
        await this.XAckDelPrepareAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckDelAsync(key, "group1", "1-0", cancellationToken: CancellationToken.None),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    #endregion

    #region XAdd

    private static string XAddKeyAsync() => $"Stream:{{XAddAsync}}:{Guid.NewGuid():N}";

    private async static Task XAddRedisErrorAsync(Func<Task> action)
    {
        await Assert.ThrowsAsync<RedisErrorException>(action);
    }

    private async Task XAddSeedAsync(string key, int count = 5)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, $"{i}-0", "f", $"v{i}"), Is.EqualTo($"{i}-0"));
        }
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(count));
    }

    [Test]
    public async Task XAdd_Overload_FieldValue_ExplicitIdAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", "field", "value"), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_FieldValue_AutoIdAsync()
    {
        var key = XAddKeyAsync();
        var id = await this._redis.Stream.XAddAsync(key, "field", "value");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_Tuple_ExplicitIdAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", ("field", "value")), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_Tuple_AutoIdAsync()
    {
        var key = XAddKeyAsync();
        var id = await this._redis.Stream.XAddAsync(key, ("field", "value"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_KeyValuePair_ExplicitIdAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", new KeyValuePair<SharpValue, SharpValue>("field", "value")), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_KeyValuePair_AutoIdAsync()
    {
        var key = XAddKeyAsync();
        var id = await this._redis.Stream.XAddAsync(key, new KeyValuePair<SharpValue, SharpValue>("field", "value"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_TupleCollection_ExplicitIdAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", (ICollection<(SharpValue field, SharpValue value)>)[("field", "value"), ("field2", "value2")]), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_TupleCollection_AutoIdAsync()
    {
        var key = XAddKeyAsync();
        var id = await this._redis.Stream.XAddAsync(key, (ICollection<(SharpValue field, SharpValue value)>)new List<(SharpValue field, SharpValue value)> { ("field", "value"), ("field2", "value2") });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_PairCollection_ExplicitIdAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", new List<KeyValuePair<SharpValue, SharpValue>> { new("field", "value"), new("field2", "value2") }), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_PairCollection_AutoIdAsync()
    {
        var key = XAddKeyAsync();
        var id = await this._redis.Stream.XAddAsync(key, (ICollection<KeyValuePair<SharpValue, SharpValue>>)new List<KeyValuePair<SharpValue, SharpValue>> { new("field", "value"), new("field2", "value2") });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_TupleSpan_ExplicitIdAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", [("field", "value"), ("field2", "value2")]), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_TupleSpan_AutoIdAsync()
    {
        var key = XAddKeyAsync();
        var id = await this._redis.Stream.XAddAsync(key, (ReadOnlySpan<(SharpValue field, SharpValue value)>)[("field", "value"), ("field2", "value2")]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_PairSpan_ExplicitIdAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", (ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>>)[new("field", "value"), new("field2", "value2")]), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Overload_PairSpan_AutoIdAsync()
    {
        var key = XAddKeyAsync();
        var id = await this._redis.Stream.XAddAsync(key, (ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>>)[new("field", "value"), new("field2", "value2")]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Id_StrictIncreasingAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", "f", "a"), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-1", "f", "b"), Is.EqualTo("1-1"));
            Assert.That(await this._redis.Stream.XAddAsync(key, "2-0", "f", "c"), Is.EqualTo("2-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XAdd_Id_DuplicateAsync()
    {
        var key = XAddKeyAsync();
        await this._redis.Stream.XAddAsync(key, "2-0", "f", "v");
        await XAddRedisErrorAsync(async () => { await this._redis.Stream.XAddAsync(key, "2-0", "f", "v"); });
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
    }

    [Test]
    public async Task XAdd_Id_DecreasingAsync()
    {
        var key = XAddKeyAsync();
        await this._redis.Stream.XAddAsync(key, "2-0", "f", "v");
        await XAddRedisErrorAsync(async () => { await this._redis.Stream.XAddAsync(key, "1-9", "f", "v"); });
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
    }

    [Test]
    public async Task XAdd_Id_ZeroZeroAsync()
    {
        var key = XAddKeyAsync();
        await XAddRedisErrorAsync(async () => { await this._redis.Stream.XAddAsync(key, "0-0", "f", "v"); });
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.Zero);
    }

    [Test]
    public async Task XAdd_Id_DeletedDoesNotResetLastIdAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "2-0", "f", "a"), Is.EqualTo("2-0"));
            Assert.That(await this._redis.Stream.XDelAsync(key, (EntryId)"2-0"), Is.EqualTo(1));
        }
        await XAddRedisErrorAsync(async () => { await this._redis.Stream.XAddAsync(key, "2-0", "f", "b"); });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "2-1", "f", "c"), Is.EqualTo("2-1"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Id_MillisecondAutoSequenceAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, (EntryId)1UL, "f", "a"), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XAddAsync(key, (EntryId)1UL, "f", "b"), Is.EqualTo("1-1"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task XAdd_Nomkstream_NewAndExistingAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "f", "v", nomkstream: true), Is.Null);
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.Zero);
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", "f", "a"), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XAddAsync(key, "2-0", "f", "b", nomkstream: true), Is.EqualTo("2-0"));
        }
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(2));
    }

    [Test]
    public async Task XAdd_Trim_MaxLen_ExactAsync()
    {
        var key = XAddKeyAsync();
        await this.XAddSeedAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "6-0", "f", "v6", trimOptions: TrimOptions.ByMaxLen(3, TrimOperator.Exact)), Is.EqualTo("6-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(3));
            Assert.That(await this._redis.Stream.XAddAsync(key, "7-0", "f", "v7", trimOptions: new TrimOptions(2, TrimOperator.None)), Is.EqualTo("7-0"));
        }
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(2));
    }

    [Test]
    public async Task XAdd_Trim_MaxLen_ZeroAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", "f", "v", trimOptions: TrimOptions.ByMaxLen(0, TrimOperator.Exact)), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.Zero);
        }
    }

    [Test]
    public async Task XAdd_Trim_MaxLen_ApproximateAsync()
    {
        var key = XAddKeyAsync();
        await this.XAddSeedAsync(key, 150);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "151-0", "f", "v", trimOptions: TrimOptions.ByMaxLen(10, TrimOperator.Approximate, limit: 0)), Is.EqualTo("151-0"));
            // ~ 为近似裁剪，可能多留旧消息；只断言长度合法。
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.InRange(1, 151));
        }
    }

    [Test]
    public async Task XAdd_Trim_MinId_ExactAsync()
    {
        var key = XAddKeyAsync();
        await this.XAddSeedAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "6-0", "f", "v6", trimOptions: TrimOptions.ByMinId("3-0", TrimOperator.Exact)), Is.EqualTo("6-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XAdd_Trim_MinId_MillisecondsAsync()
    {
        var key = XAddKeyAsync();
        await this.XAddSeedAsync(key);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "6-0", "f", "v6", trimOptions: new TrimOptions("4", TrimOperator.None)), Is.EqualTo("6-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XAdd_Trim_MinId_ApproximateAsync()
    {
        var key = XAddKeyAsync();
        await this.XAddSeedAsync(key, 150);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "151-0", "f", "v151", trimOptions: TrimOptions.ByMinId("100-0", TrimOperator.Approximate, limit: 0)), Is.EqualTo("151-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.InRange(52, 151));
        }
    }

    [Test]
    public async Task XAdd_Trim_Default_NoneAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", "f", "v", trimOptions: default), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Trim_MaxLen_ConstructorValidationAsync()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TrimOptions(-1, TrimOperator.Exact));
        Assert.Throws<ArgumentOutOfRangeException>(() => TrimOptions.ByMaxLen(3, TrimOperator.Approximate, -1));
        Assert.Throws<ArgumentException>(() => TrimOptions.ByMaxLen(3, TrimOperator.Exact, 1));
        Assert.Throws<ArgumentException>(() => TrimOptions.ByMaxLen(3, TrimOperator.None, 0));
    }

    [Test]
    public async Task XAdd_Trim_MinId_ConstructorValidationAsync()
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
    public async Task XAdd_Trim_PropertiesAsync()
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
    public async Task XAdd_WrongKeyTypeAsync()
    {
        var key = XAddKeyAsync();
        await this._redis.String.SetAsync(key, "not-a-stream");
        await XAddRedisErrorAsync(async () => { await this._redis.Stream.XAddAsync(key, "f", "v"); });
    }

    [Test]
    public async Task XAdd_Flags_NoneAndCancellationTokenAsync()
    {
        var key = XAddKeyAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", "f", "v", flags: CommandFlags.None, cancellationToken: CancellationToken.None), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_UnicodeAndMultipleFieldsAsync()
    {
        var key = XAddKeyAsync();
        ICollection<(SharpValue field, SharpValue value)> fields = [("中文字段", "你好🌍"), ("emoji", "😃")];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", fields), Is.EqualTo("1-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_DelPolicy_NoGroupsAsync()
    {
        foreach (var policy in new[] { DeletionPolicy.KeepRef, DeletionPolicy.DelRef, DeletionPolicy.Acked })
        {
            var key = XAddKeyAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", "f", "v", delPolicy: policy, trimOptions: TrimOptions.ByMaxLen(1, TrimOperator.Exact)), Is.EqualTo("1-0"));
                Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
            }
        }
    }

    [Test]
    public async Task XAdd_DelPolicy_KeepRefAndDelRefAsync()
    {
        foreach (var policy in new[] { DeletionPolicy.KeepRef, DeletionPolicy.DelRef })
        {
            var key = XAddKeyAsync();
            await this.XAddSeedAsync(key, 2);
            Assert.That(await this._redis.Stream.XGroupCreateAsync(key, "g", "0-0"), Is.True);
            var read = await this._redis.Stream.XReadGroupAsync(key, ">", "g", "c", count: 2);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(read.HasValue, Is.True);
                Assert.That(await this._redis.Stream.XAddAsync(key, "3-0", "f", "v3", trimOptions: TrimOptions.ByMaxLen(1, TrimOperator.Exact), delPolicy: policy), Is.EqualTo("3-0"));
                Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
                Assert.That(await this._redis.Stream.XAckAsync(key, "g", (EntryId)"1-0"), Is.EqualTo(policy == DeletionPolicy.KeepRef ? 1 : 0));
            }
        }
    }

    [Test]
    public async Task XAdd_DelPolicy_AckedProtectsPendingAsync()
    {
        var key = XAddKeyAsync();
        await this.XAddSeedAsync(key, 2);
        Assert.That(await this._redis.Stream.XGroupCreateAsync(key, "g", "0-0"), Is.True);
        var read = await this._redis.Stream.XReadGroupAsync(key, ">", "g", "c", count: 2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(await this._redis.Stream.XAddAsync(key, "3-0", "f", "v3", trimOptions: TrimOptions.ByMaxLen(1, TrimOperator.Exact), delPolicy: DeletionPolicy.Acked), Is.EqualTo("3-0"));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.GreaterThan(1));
            Assert.That(await this._redis.Stream.XAckAsync(key, "g", (EntryId)"1-0"), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Idempotent_Manual_DeduplicatedAsync()
    {
        var key = XAddKeyAsync();
        var a = await this._redis.Stream.XAddAsync(key, "f", "v", idempotent: ("p", "m1"));
        var b = await this._redis.Stream.XAddAsync(key, "f", "v", idempotent: ("p", "m1"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(a, Is.Not.Null.And.Not.Empty);
            Assert.That(b, Is.EqualTo(a));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Idempotent_Auto_DeduplicatedAsync()
    {
        var key = XAddKeyAsync();
        var a = await this._redis.Stream.XAddAsync(key, "f", "v", idempotent: "producer1");
        var b = await this._redis.Stream.XAddAsync(key, "f", "v", idempotent: "producer1");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(a, Is.Not.Null.And.Not.Empty);
            Assert.That(b, Is.EqualTo(a));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAdd_Idempotent_ExplicitStreamIdRejectedAsync()
    {
        var key = XAddKeyAsync();
        await XAddRedisErrorAsync(async () => { await this._redis.Stream.XAddAsync(key, "1-0", "f", "v", idempotent: ("p", "m1")); });
    }

    [Test]
    public async Task XAdd_Trim_Limit_PositiveAsync()
    {
        var key = XAddKeyAsync();
        await this.XAddSeedAsync(key, 150);
        var trim = TrimOptions.ByMaxLen(10, TrimOperator.Approximate, limit: 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(key, "151-0", "f", "v", trimOptions: trim), Is.EqualTo("151-0"));
            // LIMIT=1 限制扫描规模，实际删除量由 Redis 节点结构决定。
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.InRange(1, 151));
        }
    }

    [Test]
    public async Task XAdd_EmptyTupleCollectionAsync()
    {
        var key = XAddKeyAsync();
        ICollection<(SharpValue field, SharpValue value)> fields = [];
        await Assert.ThatAsync(async () => await this._redis.Stream.XAddAsync(key, fields), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public async Task XAdd_EmptyPairCollectionAsync()
    {
        var key = XAddKeyAsync();
        ICollection<KeyValuePair<SharpValue, SharpValue>> fields = [];
        await Assert.ThatAsync(async () => await this._redis.Stream.XAddAsync(key, fields), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public async Task XAdd_EmptyTupleSpanAsync()
    {
        var key = XAddKeyAsync();
        await Assert.ThatAsync(async () =>
        {
            ReadOnlySpan<(SharpValue field, SharpValue value)> fields = [];
            await this._redis.Stream.XAddAsync(key, fields);
        }, Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public async Task XAdd_EmptyPairSpanAsync()
    {
        var key = XAddKeyAsync();
        ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>> fields = [];
        await Assert.ThatAsync(async () =>
        {
            ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>> fields = [];
            await this._redis.Stream.XAddAsync(key, fields);
        }, Throws.TypeOf<NotSupportedException>());
    }
    #endregion

    #region XRange

    private static string XRangeKeyAsync() => $"Stream:{{XRangeAsync}}:{Guid.NewGuid():N}";

    private async Task XRangeSeedAsync(string key)
    {
        for (int i = 1; i <= 5; i++)
            Assert.That(await this._redis.Stream.XAddAsync(key, $"{i}-0", "field", $"value{i}"), Is.EqualTo($"{i}-0"));
        Assert.That(await this._redis.Stream.XLenAsync(key).ConfigureAwait(false), Is.EqualTo(5));
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
    public async Task XRange_AllAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "-", "+"), "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XRange_InclusiveAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "2-0", "4-0"), "2-0", "3-0", "4-0");
    }

    [Test]
    public async Task XRange_SingleEntryAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "3-0", "3-0"), "3-0");
    }

    [Test]
    public async Task XRange_StartExclusiveAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "(2-0", "4-0"), "3-0", "4-0");
    }

    [Test]
    public async Task XRange_EndExclusiveAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "2-0", "(4-0"), "2-0", "3-0");
    }

    [Test]
    public async Task XRange_BothExclusiveAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "(2-0", "(4-0"), "3-0");
    }

    [Test]
    public async Task XRange_ExclusiveSameIdAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssertEmpty(await this._redis.Stream.XRangeAsync(key, "(3-0", "3-0"));
        XRangeAssertEmpty(await this._redis.Stream.XRangeAsync(key, "3-0", "(3-0"));
    }

    [Test]
    public async Task XRange_OnlyStartBoundaryAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "3-0", "+"), "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XRange_OnlyEndBoundaryAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "-", "3-0"), "1-0", "2-0", "3-0");
    }

    [Test]
    public async Task XRange_NoMatchingEntriesAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssertEmpty(await this._redis.Stream.XRangeAsync(key, "100-0", "200-0"));
    }

    [Test]
    public async Task XRange_StartGreaterThanEndAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssertEmpty(await this._redis.Stream.XRangeAsync(key, "4-0", "2-0"));
    }

    [Test]
    public async Task XRange_Count_NullAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "-", "+", count: null), "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XRange_Count_OneAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "-", "+", count: 1), "1-0");
    }

    [Test]
    public async Task XRange_Count_TwoAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "-", "+", count: 2), "1-0", "2-0");
    }

    [Test]
    public async Task XRange_Count_GreaterThanLengthAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "-", "+", count: 100), "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XRange_PaginationAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        var page1 = await this._redis.Stream.XRangeAsync(key, "-", "+", count: 2);
        XRangeAssert(page1, "1-0", "2-0");
        var page2 = await this._redis.Stream.XRangeAsync(key, $"({page1![^1].Id}", "+", count: 2);
        XRangeAssert(page2, "3-0", "4-0");
        var page3 = await this._redis.Stream.XRangeAsync(key, $"({page2![^1].Id}", "+", count: 2);
        XRangeAssert(page3, "5-0");
        XRangeAssertEmpty(await this._redis.Stream.XRangeAsync(key, $"({page3![^1].Id}", "+", count: 2));
    }

    [Test]
    public async Task XRange_SameMillisecond_SequencesAsync()
    {
        var key = XRangeKeyAsync();
        for (int i = 0; i < 4; i++)
            Assert.That(await this._redis.Stream.XAddAsync(key, $"100-{i}", "f", $"v{i}"), Is.EqualTo($"100-{i}"));
        var result = await this._redis.Stream.XRangeAsync(key, "100-1", "100-2");
        Assert.That(result, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result![0].Id, Is.EqualTo("100-1"));
            Assert.That(result[1].Id, Is.EqualTo("100-2"));
        }
    }

    [Test]
    public async Task XRange_MillisecondRange_ExplicitBoundsAsync()
    {
        var key = XRangeKeyAsync();
        await this._redis.Stream.XAddAsync(key, "100-0", "f", "a");
        await this._redis.Stream.XAddAsync(key, "100-1", "f", "b");
        await this._redis.Stream.XAddAsync(key, "101-0", "f", "c");
        // 不传单独的 "100"，避免依赖 EntryId 当前是否支持不完整数字 ID。
        var items = await this._redis.Stream.XRangeAsync(key, "100-0", "100-18446744073709551615");
        Assert.That(items, Has.Length.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(items![0].Id, Is.EqualTo("100-0"));
            Assert.That(items[1].Id, Is.EqualTo("100-1"));
        }
    }

    [Test]
    public async Task XRange_FieldValueAsync()
    {
        var key = XRangeKeyAsync();
        Assert.That(await this._redis.Stream.XAddAsync(key, "1-0", "name", "测试中文"), Is.EqualTo("1-0"));
        var items = await this._redis.Stream.XRangeAsync(key, "-", "+");
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
    public async Task XRange_MultipleFieldsAsync()
    {
        var key = XRangeKeyAsync();
        Assert.That(await this._redis.Stream.XAddAsync(key, "1-0",
            (ReadOnlySpan<(SharpValue field, SharpValue value)>)[("name", "张三"), ("age", "18"), ("city", "上海")]),
            Is.EqualTo("1-0"));
        var items = await this._redis.Stream.XRangeAsync(key, "-", "+");
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
    public async Task XRange_AfterDeleteAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        Assert.That(await this._redis.Stream.XDelAsync(key, (EntryId)"3-0"), Is.EqualTo(1));
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "-", "+"), "1-0", "2-0", "4-0", "5-0");
        XRangeAssertEmpty(await this._redis.Stream.XRangeAsync(key, "3-0", "3-0"));
    }

    [Test]
    public async Task XRange_KeyNotExistsAsync()
    {
        var key = XRangeKeyAsync();
        XRangeAssertEmpty(await this._redis.Stream.XRangeAsync(key, "-", "+"));
    }

    [Test]
    public async Task XRange_WrongKeyTypeAsync()
    {
        var key = XRangeKeyAsync();
        await this._redis.String.SetAsync(key, "hello");
        Assert.That(() => this._redis.Stream.XRangeAsync(key, "-", "+"), Throws.TypeOf<RedisErrorException>());
    }

    [Test]
    public async Task XRange_InvalidId_OverflowAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        Assert.That(() => this._redis.Stream.XRangeAsync(key, "18446744073709551616-0", "+"),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public async Task XRange_Flags_NoneAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "-", "+", flags: CommandFlags.None), "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XRange_CancellationToken_DefaultAsync()
    {
        var key = XRangeKeyAsync();
        await this.XRangeSeedAsync(key);
        XRangeAssert(await this._redis.Stream.XRangeAsync(key, "-", "+", cancellationToken: CancellationToken.None), "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    #endregion

    #region XAutoClaim

    private static string XAutoClaimKeyAsync() =>
        $"Stream:{{XAutoClaimAsync}}:{Guid.NewGuid():N}";

    private static void XAutoClaimAssertIdsAsync(
        EntryItem[]? entries, params string[] ids)
    {
        Assert.That(entries?.Length ?? 0, Is.EqualTo(ids.Length));

        for (int i = 0; i < ids.Length; i++)
        {
            Assert.That(entries![i].Id, Is.EqualTo(ids[i]));
        }
    }

    private async Task XAutoClaimPrepareAsync(string key, int count = 5)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, "group1", "0-0"), Is.True);

        var read = await this._redis.Stream.XReadGroupAsync(
            key, ">", "group1", "consumer1", count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(read!.Value.Entries, Has.Length.EqualTo(count));

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(count));
        }
    }

    [Test]
    public async Task XAutoClaim_AllAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "0-0");

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));

        XAutoClaimAssertIdsAsync(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.IsNull(result.DeletedIds);

            Assert.That(result.Entries![0].FieldValue![0].Value.ToString(),
                Is.EqualTo("value1"));
        }
    }

    [Test]
    public async Task XAutoClaim_Count_OneAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "0-0", count: 1);

        XAutoClaimAssertIdsAsync(result.Entries, "1-0");
        Assert.That(result.NextStartId, Is.Not.EqualTo("0-0"));
    }

    [Test]
    public async Task XAutoClaim_Count_TwoAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "0-0", count: 2);

        XAutoClaimAssertIdsAsync(result.Entries, "1-0", "2-0");
    }

    [Test]
    public async Task XAutoClaim_Count_NullAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "0-0", count: null);

        XAutoClaimAssertIdsAsync(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XAutoClaim_Count_GreaterThanPendingAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "0-0", count: 100);

        XAutoClaimAssertIdsAsync(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public async Task XAutoClaim_StartIdAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "3-0");

        XAutoClaimAssertIdsAsync(result.Entries,
            "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XAutoClaim_StartId_ZeroAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "0");

        XAutoClaimAssertIdsAsync(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XAutoClaim_StartId_BeyondPendingAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "100-0");

        XAutoClaimAssertIdsAsync(result.Entries);
        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public async Task XAutoClaim_MinIdleTime_NotReachedAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2",
            minIdleTime: 3_600_000,
            startId: "0-0");

        XAutoClaimAssertIdsAsync(result.Entries);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.NextStartId, Is.EqualTo("0-0"));

            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "1-0"), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XAutoClaim_PaginationAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var allIds = new List<string>();
        EntryId cursor = "0-0";

        for (int page = 0; page < 10; page++)
        {
            var result = await this._redis.Stream.XAutoClaimAsync(
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
    public async Task XAutoClaim_DeletedIdsAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelAsync(
                    key, "2-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XDelAsync(
                key, "4-0"), Is.EqualTo(1));
        }

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "0-0");

        XAutoClaimAssertIdsAsync(result.Entries,
            "1-0", "3-0", "5-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.DeletedIds,
                    Is.EqualTo(["2-0", "4-0"]));

            Assert.That(result.NextStartId, Is.EqualTo("0-0"));
        }
    }

    [Test]
    public async Task XAutoClaim_NoPendingAsync()
    {
        var key = XAutoClaimKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                    key, "1-0", "field", "value1"), Is.EqualTo("1-0"));

            Assert.That(await this._redis.Stream.XGroupCreateAsync(
                key, "group1", "0-0"), Is.True);
        }

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "0-0");

        XAutoClaimAssertIdsAsync(result.Entries);
        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public async Task XAutoClaim_AfterAckAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        Assert.That(await this._redis.Stream.XAckAsync(
            key, "group1", "1-0"), Is.EqualTo(1));

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "0-0");

        XAutoClaimAssertIdsAsync(result.Entries,
            "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XAutoClaim_Flags_NoneAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "0-0",
            flags: CommandFlags.None);

        XAutoClaimAssertIdsAsync(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XAutoClaim_CancellationToken_DefaultAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2", 0, "0-0",
            cancellationToken: CancellationToken.None);

        XAutoClaimAssertIdsAsync(result.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XAutoClaim_GroupNotExistsAsync()
    {
        var key = XAutoClaimKeyAsync();

        Assert.That(await this._redis.Stream.XAddAsync(
            key, "1-0", "field", "value1"), Is.EqualTo("1-0"));

        await Assert.ThrowsAsync<RedisErrorException>(async () => await this._redis.Stream.XAutoClaimAsync(key, "not_exists", "consumer2", 0, "0-0"));
    }

    [Test]
    public async Task XAutoClaim_KeyNotExistsAsync()
    {
        var key = XAutoClaimKeyAsync();
        await Assert.ThrowsAsync<RedisErrorException>(async () => await this._redis.Stream.XAutoClaimAsync(key, "group1", "consumer2", 0, "0-0"));
    }

    [Test]
    public async Task XAutoClaim_WrongKeyTypeAsync()
    {
        var key = XAutoClaimKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await Assert.ThrowsAsync<RedisErrorException>(async () => await this._redis.Stream.XAutoClaimAsync(key, "group1", "consumer2", 0, "0-0"));
    }


    [Test]
    public async Task XAutoClaim_JustIdAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "0-0",
            justid: true);

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));

        XAutoClaimAssertIdsAsync(result.Entries,
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
    public async Task XAutoClaim_JustId_CountAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "0-0",
            count: 2,
            justid: true);

        XAutoClaimAssertIdsAsync(result.Entries, "1-0", "2-0");

        Assert.That(result.NextStartId, Is.EqualTo("3-0"));
    }

    [Test]
    public async Task XAutoClaim_JustId_StartIdAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "3-0",
            justid: true);

        XAutoClaimAssertIdsAsync(result.Entries,
            "3-0", "4-0", "5-0");

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public async Task XAutoClaim_JustId_MinIdleTimeAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2",
            minIdleTime: 3_600_000,
            startId: "0-0",
            justid: true);

        XAutoClaimAssertIdsAsync(result.Entries);

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public async Task XAutoClaim_JustId_NoPendingAsync()
    {
        var key = XAutoClaimKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                    key, "1-0", "field", "value1"),
                    Is.EqualTo("1-0"));

            Assert.That(await this._redis.Stream.XGroupCreateAsync(
                key, "group1", "0-0"), Is.True);
        }

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "0-0",
            justid: true);

        XAutoClaimAssertIdsAsync(result.Entries);

        Assert.That(result.NextStartId, Is.EqualTo("0-0"));
    }

    [Test]
    public async Task XAutoClaim_JustId_DeletedIdsAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelAsync(
                    key, "2-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XDelAsync(
                key, "4-0"), Is.EqualTo(1));
        }

        var result = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "0-0",
            justid: true);

        XAutoClaimAssertIdsAsync(result.Entries,
            "1-0", "3-0", "5-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.DeletedIds, Is.EqualTo(["2-0", "4-0"]));

            Assert.That(result.NextStartId, Is.EqualTo("0-0"));
        }
    }

    [Test]
    public async Task XAutoClaim_JustId_PaginationAsync()
    {
        var key = XAutoClaimKeyAsync();
        await this.XAutoClaimPrepareAsync(key);

        var ids = new List<string>();
        EntryId startId = "0-0";
        bool completed = false;

        for (int i = 0; i < 10; i++)
        {
            var result = await this._redis.Stream.XAutoClaimAsync(
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

    private static string XPendingKeyAsync() =>
        $"Stream:{{XPendingAsync}}:{Guid.NewGuid():N}";

    private async Task XPendingSeedAsync(string key, int count = 5)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(count));
            Assert.That(await this._redis.Stream.XGroupCreateAsync(key, "group1", "0-0"), Is.True);
        }
    }

    private async Task XPendingReadAsync(
        string key, string consumer, int count)
    {
        var result = await this._redis.Stream.XReadGroupAsync(
            key, ">", "group1", consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries,
                Has.Length.EqualTo(count));
        }
    }

    private async Task XPendingPrepareAsync(string key)
    {
        await this.XPendingSeedAsync(key);
        await this.XPendingReadAsync(key, "consumer1", 5);

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(summary.Count, Is.EqualTo(5));
    }

    private static void XPendingAssertIdsAsync(
        XPendingItem[]? entries, params string[] ids)
    {
        Assert.That(entries?.Length ?? 0, Is.EqualTo(ids.Length));

        for (int i = 0; i < ids.Length; i++)
        {
            Assert.That(entries![i].Id, Is.EqualTo(ids[i]));
        }
    }

    private static void XPendingAssertDeliveryAsync(
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
    public async Task XPending_Summary_NoPendingAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingSeedAsync(key);

        var summary = await this._redis.Stream.XPendingAsync(
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
    public async Task XPending_Summary_SingleConsumerAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.EqualTo(5));
            Assert.That(summary.MinId, Is.EqualTo("1-0"));
            Assert.That(summary.MaxId, Is.EqualTo("5-0"));
            Assert.That(summary.Consumers, Has.Length.EqualTo(1));
            Assert.That(summary.Consumers![0].Name,
                Is.EqualTo("consumer1"));
            Assert.That(summary.Consumers[0].Count,
                Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XPending_Summary_MultipleConsumersAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingSeedAsync(key);

        await this.XPendingReadAsync(key, "consumer1", 2);
        await this.XPendingReadAsync(key, "consumer2", 3);

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.EqualTo(5));
            Assert.That(summary.MinId, Is.EqualTo("1-0"));
            Assert.That(summary.MaxId, Is.EqualTo("5-0"));
            Assert.That(summary.Consumers, Has.Length.EqualTo(2));
            Assert.IsNotNull(summary.Consumers);
            Assert.That(summary.Consumers!.Single(
                x => x.Name == "consumer1").Count,
                Is.EqualTo(2));

            Assert.That(summary.Consumers!.Single(
                x => x.Name == "consumer2").Count,
                Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XPending_Summary_AfterAckAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(
                    key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "5-0"), Is.EqualTo(1));
        }

        var summary = await this._redis.Stream.XPendingAsync(
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
    public async Task XPending_Detail_AllAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        var entries = await this._redis.Stream.XPendingAsync(
            key, "group1", new PendingOptions("-", "+", 10));

        XPendingAssertIdsAsync(entries,
            "1-0", "2-0", "3-0", "4-0", "5-0");

        for (int i = 0; i < 5; i++)
        {
            XPendingAssertDeliveryAsync(entries![i],
                $"{i + 1}-0", "consumer1", 1);
        }
    }

    [Test]
    public async Task XPending_Detail_InclusiveRangeAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("2-0", "4-0", 10)),
            "2-0", "3-0", "4-0");
    }

    [Test]
    public async Task XPending_Detail_StartExclusiveAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("(2-0", "5-0", 10)),
            "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XPending_Detail_EndExclusiveAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("2-0", "(5-0", 10)),
            "2-0", "3-0", "4-0");
    }

    [Test]
    public async Task XPending_Detail_BothExclusiveAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("(2-0", "(5-0", 10)),
            "3-0", "4-0");
    }

    [Test]
    public async Task XPending_Detail_EmptyRangeAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("(3-0", "3-0", 10)));

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("4-0", "2-0", 10)));

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("100-0", "+", 10)));
    }

    [Test]
    public async Task XPending_Detail_CountAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 1)), "1-0");

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 2)),
            "1-0", "2-0");

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 100)),
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XPending_Detail_PaginationAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        var page1 = await this._redis.Stream.XPendingAsync(
            key, "group1",
            new PendingOptions("-", "+", 2));

        XPendingAssertIdsAsync(page1, "1-0", "2-0");

        var page2 = await this._redis.Stream.XPendingAsync(
            key, "group1",
            new PendingOptions($"({page1![^1].Id}", "+", 2));

        XPendingAssertIdsAsync(page2, "3-0", "4-0");

        var page3 = await this._redis.Stream.XPendingAsync(
            key, "group1",
            new PendingOptions($"({page2![^1].Id}", "+", 2));

        XPendingAssertIdsAsync(page3, "5-0");

        var page4 = await this._redis.Stream.XPendingAsync(
            key, "group1",
            new PendingOptions($"({page3![^1].Id}", "+", 2));

        XPendingAssertIdsAsync(page4);
    }

    [Test]
    public async Task XPending_Detail_ConsumerFilterAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingSeedAsync(key);

        await this.XPendingReadAsync(key, "consumer1", 2);
        await this.XPendingReadAsync(key, "consumer2", 3);

        var consumer1 = await this._redis.Stream.XPendingAsync(
            key, "group1",
            new PendingOptions("-", "+", 10,
                consumer: "consumer1"));

        var consumer2 = await this._redis.Stream.XPendingAsync(
            key, "group1",
            new PendingOptions("-", "+", 10,
                consumer: "consumer2"));

        XPendingAssertIdsAsync(consumer1, "1-0", "2-0");
        XPendingAssertIdsAsync(consumer2, "3-0", "4-0", "5-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(consumer1!.All(
                    x => x.Consumer == "consumer1"), Is.True);

            Assert.That(consumer2!.All(
                x => x.Consumer == "consumer2"), Is.True);
        }
    }

    [Test]
    public async Task XPending_Detail_ConsumerNotExistsAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 10,
                    consumer: "consumer_not_exists")));
    }

    [Test]
    public async Task XPending_Detail_MinIdleTime_ZeroAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 10,
                    minIdleTime: 0)),
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XPending_Detail_MinIdleTime_NotReachedAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 10,
                    minIdleTime: 3_600_000)));
    }

    [Test]
    public async Task XPending_Detail_MinIdleTime_AndConsumerAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingSeedAsync(key);

        await this.XPendingReadAsync(key, "consumer1", 2);
        await this.XPendingReadAsync(key, "consumer2", 3);

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 10,
                    minIdleTime: 0,
                    consumer: "consumer2")),
            "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XPending_Detail_AfterAckAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(
                    key, "group1", "2-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "4-0"), Is.EqualTo(1));
        }

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 10)),
            "1-0", "3-0", "5-0");
    }

    [Test]
    public async Task XPending_Detail_AfterXDelAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(
            key, "3-0"), Is.EqualTo(1));

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 10)),
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XPending_Detail_AutoClaim_JustIdAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        var before = await this._redis.Stream.XPendingAsync(
            key, "group1",
            new PendingOptions("1-0", "1-0", 1));

        XPendingAssertDeliveryAsync(before![0],
            "1-0", "consumer1", 1);

        var claimed = await this._redis.Stream.XAutoClaimAsync(
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

        var after = await this._redis.Stream.XPendingAsync(
            key, "group1",
            new PendingOptions("1-0", "1-0", 1));

        XPendingAssertDeliveryAsync(after![0],
            "1-0", "consumer2", 1);
    }

    [Test]
    public async Task XPending_Detail_AutoClaim_NormalAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        var before = await this._redis.Stream.XPendingAsync(
            key, "group1",
            new PendingOptions("1-0", "1-0", 1));

        XPendingAssertDeliveryAsync(before![0],
            "1-0", "consumer1", 1);

        var claimed = await this._redis.Stream.XAutoClaimAsync(
            key, "group1", "consumer2",
            minIdleTime: 0,
            startId: "0-0",
            count: 1,
            justid: false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(claimed.Entries?.Length, Is.EqualTo(1));
            Assert.That(claimed.Entries![0].FieldValue,
                Is.Not.Null.And.Not.Empty);
        }

        var after = await this._redis.Stream.XPendingAsync(
            key, "group1",
            new PendingOptions("1-0", "1-0", 1));

        XPendingAssertDeliveryAsync(after![0],
            "1-0", "consumer2", 2);
    }

    [Test]
    public async Task XPending_Options_DefaultAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await this._redis.Stream.XPendingAsync(
                key, "group1", default(PendingOptions)));
    }

    [Test]
    public void XPending_Options_InvalidCountAsync()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PendingOptions("-", "+", 0));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PendingOptions("-", "+", -1));
    }

    [Test]
    public async Task XPending_Flags_NoneAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1", flags: CommandFlags.None);

        Assert.That(summary.Count, Is.EqualTo(5));

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 10),
                flags: CommandFlags.None),
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XPending_CancellationToken_DefaultAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingPrepareAsync(key);

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1",
            cancellationToken: CancellationToken.None);

        Assert.That(summary.Count, Is.EqualTo(5));

        XPendingAssertIdsAsync(
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 10),
                cancellationToken: CancellationToken.None),
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XPending_GroupNotExistsAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingSeedAsync(key);

        await Assert.ThrowsAsync<RedisErrorException>(async () =>
            await this._redis.Stream.XPendingAsync(
                key, "not_exists"));

        await Assert.ThrowsAsync<RedisErrorException>(async () =>
            await this._redis.Stream.XPendingAsync(
                key, "not_exists",
                new PendingOptions("-", "+", 10)));
    }

    [Test]
    public async Task XPending_WrongKeyTypeAsync()
    {
        var key = XPendingKeyAsync();
        await this._redis.String.SetAsync(key, "hello");

        await Assert.ThrowsAsync<RedisErrorException>(async () =>
            await this._redis.Stream.XPendingAsync(
                key, "group1"));

        await Assert.ThrowsAsync<RedisErrorException>(async () =>
            await this._redis.Stream.XPendingAsync(
                key, "group1",
                new PendingOptions("-", "+", 10)));
    }

    [Test]
    public async Task XPending_Detail_ReturnNullAsync()
    {
        var key = XPendingKeyAsync();
        await this.XPendingSeedAsync(key);

        // 已创建 Stream 和消费者组，但尚未读取消息
        // 因此 PEL 中没有任何待处理消息
        var result = await this._redis.Stream.XPendingAsync(
            key, "group1",
            new PendingOptions("-", "+", 10));

        Assert.That(result, Is.Null);

        // 验证摘要模式确实没有待处理消息
        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(summary.Count, Is.Zero);
    }
    #endregion

    #region XCfgSet

    private static string XCfgSetKeyAsync() =>
        $"Stream:{{XCfgSetAsync}}:{Guid.NewGuid():N}";

    private async Task XCfgSetPrepareAsync(string key)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                    key, "1-0", "field", "value"),
                    Is.EqualTo("1-0"));

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(1));
        }
    }

    private static async Task XCfgSetRedisErrorAsync<TError>(Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XCfgSet_DefaultAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        Assert.That(await this._redis.Stream.XCfgSetAsync(key),
            Is.True);
    }

    [Test]
    public async Task XCfgSet_DurationOnlyAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        Assert.That(await this._redis.Stream.XCfgSetAsync(
            key, duration: 300, maxsize: null), Is.True);
    }

    [Test]
    public async Task XCfgSet_MaxSizeOnlyAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        Assert.That(await this._redis.Stream.XCfgSetAsync(
            key, duration: null, maxsize: 500), Is.True);
    }

    [Test]
    public async Task XCfgSet_BothParametersAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        Assert.That(await this._redis.Stream.XCfgSetAsync(
            key, duration: 600, maxsize: 1000), Is.True);
    }

    [Test]
    public async Task XCfgSet_Duration_MinAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        Assert.That(await this._redis.Stream.XCfgSetAsync(
            key, duration: 1, maxsize: null), Is.True);
    }

    [Test]
    public async Task XCfgSet_Duration_MaxAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        Assert.That(await this._redis.Stream.XCfgSetAsync(
            key, duration: 86400, maxsize: null), Is.True);
    }

    [Test]
    public async Task XCfgSet_MaxSize_MinAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        Assert.That(await this._redis.Stream.XCfgSetAsync(
            key, duration: null, maxsize: 1), Is.True);
    }

    [Test]
    public async Task XCfgSet_MaxSize_MaxAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        Assert.That(await this._redis.Stream.XCfgSetAsync(
            key, duration: null, maxsize: 10000), Is.True);
    }

    [Test]
    public async Task XCfgSet_Duration_ZeroAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        await XCfgSetRedisErrorAsync<ArgumentOutOfRangeException>(async () =>
            await this._redis.Stream.XCfgSetAsync(
                key, duration: 0, maxsize: null));
    }

    [Test]
    public async Task XCfgSet_Duration_NegativeAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        await XCfgSetRedisErrorAsync<ArgumentOutOfRangeException>(async () =>
            await this._redis.Stream.XCfgSetAsync(
                key, duration: -1, maxsize: null));
    }

    [Test]
    public async Task XCfgSet_Duration_OverflowAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        await XCfgSetRedisErrorAsync<ArgumentOutOfRangeException>(async () =>
            await this._redis.Stream.XCfgSetAsync(
                key, duration: 86401, maxsize: null));
    }

    [Test]
    public async Task XCfgSet_MaxSize_ZeroAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        await XCfgSetRedisErrorAsync<ArgumentOutOfRangeException>(async () =>
            await this._redis.Stream.XCfgSetAsync(
                key, duration: null, maxsize: 0));
    }

    [Test]
    public async Task XCfgSet_MaxSize_NegativeAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        await XCfgSetRedisErrorAsync<ArgumentOutOfRangeException>(async () =>
            await this._redis.Stream.XCfgSetAsync(
                key, duration: null, maxsize: -1));
    }

    [Test]
    public async Task XCfgSet_MaxSize_OverflowAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        await XCfgSetRedisErrorAsync<ArgumentOutOfRangeException>(async () =>
            await this._redis.Stream.XCfgSetAsync(
                key, duration: null, maxsize: 10001));
    }

    [Test]
    public async Task XCfgSet_BothNullAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        await XCfgSetRedisErrorAsync<ArgumentException>(async () =>
            await this._redis.Stream.XCfgSetAsync(
                key, duration: null, maxsize: null));
    }

    [Test]
    public async Task XCfgSet_KeyNotExistsAsync()
    {
        var key = XCfgSetKeyAsync();

        await XCfgSetRedisErrorAsync<RedisErrorException>(async () =>
            await this._redis.Stream.XCfgSetAsync(
                key, duration: 300, maxsize: 500));
    }

    [Test]
    public async Task XCfgSet_WrongKeyTypeAsync()
    {
        var key = XCfgSetKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await XCfgSetRedisErrorAsync<RedisErrorException>(async () =>
            await this._redis.Stream.XCfgSetAsync(
                key, duration: 300, maxsize: 500));
    }

    [Test]
    public async Task XCfgSet_UpdateConfigurationAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XCfgSetAsync(
                    key, duration: 100, maxsize: 100), Is.True);

            Assert.That(await this._redis.Stream.XCfgSetAsync(
                key, duration: 300, maxsize: 500), Is.True);

            Assert.That(await this._redis.Stream.XCfgSetAsync(
                key, duration: 600, maxsize: 1000), Is.True);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XCfgSet_Flags_NoneAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        Assert.That(await this._redis.Stream.XCfgSetAsync(
            key, duration: 300, maxsize: 500,
            flags: CommandFlags.None), Is.True);
    }

    [Test]
    public async Task XCfgSet_CancellationToken_DefaultAsync()
    {
        var key = XCfgSetKeyAsync();
        await this.XCfgSetPrepareAsync(key);

        Assert.That(await this._redis.Stream.XCfgSetAsync(
            key, duration: 300, maxsize: 500,
            cancellationToken: CancellationToken.None), Is.True);
    }

    #endregion

    #region XClaim

    private static string XClaimKeyAsync() => $"Stream:{{XClaimAsync}}:{Guid.NewGuid():N}";

    private async Task XClaimSeedAsync(string key, int count = 5, int delivered = 5)
    {
        for (int i = 1; i <= count; i++)
            Assert.That(await this._redis.Stream.XAddAsync(key, $"{i}-0", "field", $"value{i}"), Is.EqualTo($"{i}-0"));

        Assert.That(await this._redis.Stream.XGroupCreateAsync(key, "group1", "0-0"), Is.True);
        if (delivered > 0)
        {
            var read = await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "consumer1", count: delivered);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(read.HasValue, Is.True);
                Assert.That(read!.Value.Entries, Has.Length.EqualTo(delivered));
            }
        }
        Assert.That((await this._redis.Stream.XPendingAsync(key, "group1")).Count, Is.EqualTo(delivered));
    }

    private async Task<XPendingItem> XClaimPendingAsync(string key, string id)
    {
        var entries = await this._redis.Stream.XPendingAsync(key, "group1", new PendingOptions(id, id, 1));
        Assert.That(entries, Is.Not.Null.And.Length.EqualTo(1));
        return entries![0];
    }

    private static void XClaimAssertIdsAsync(EntryItem[]? entries, params string[] ids)
    {
        Assert.That(entries?.Length ?? 0, Is.EqualTo(ids.Length));
        for (int i = 0; i < ids.Length; i++)
            Assert.That(entries![i].Id, Is.EqualTo(ids[i]));
    }

    private static void XClaimAssertOwnerAsync(XPendingItem pending, string id, string consumer, long deliveryCount)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pending.Id, Is.EqualTo(id));
            Assert.That(pending.Consumer, Is.EqualTo(consumer));
            Assert.That(pending.DeliveryCount, Is.EqualTo(deliveryCount));
            Assert.That(pending.IdleTime, Is.GreaterThanOrEqualTo(0));
        }
    }

    private static async Task XClaimRedisErrorAsync<TError>(Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XClaim_Single_NormalAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "1-0");
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
        XClaimAssertOwnerAsync(await this.XClaimPendingAsync(key, "1-0"), "1-0", "consumer2", 2);
    }

    [Test]
    public async Task XClaim_Single_JustIdAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "1-0", justid: true);
        Assert.That(result.HasValue, Is.True);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.Value.Id, Is.EqualTo("1-0"));
            Assert.That(result.Value.FieldValue, Is.Null);
            Assert.That(result.Value.IdleTime, Is.Null);
            Assert.That(result.Value.DeliveryCount, Is.Null);
        }
        XClaimAssertOwnerAsync(await this.XClaimPendingAsync(key, "1-0"), "1-0", "consumer2", 1);
    }

    [Test]
    public async Task XClaim_Single_MinIdleTime_NotReachedAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 86_400_000, "1-0");
        Assert.That(!result.HasValue || string.IsNullOrEmpty(result.Value.Id), Is.True);
        XClaimAssertOwnerAsync(await this.XClaimPendingAsync(key, "1-0"), "1-0", "consumer1", 1);
    }

    [Test]
    public async Task XClaim_Single_NotPendingAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key, delivered: 2);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "5-0");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(!result.HasValue || string.IsNullOrEmpty(result.Value.Id), Is.True);
            Assert.That((await this._redis.Stream.XPendingAsync(key, "group1")).Count, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task XClaim_Single_NotExistsAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "100-0");
        Assert.That(!result.HasValue || string.IsNullOrEmpty(result.Value.Id), Is.True);
    }

    [Test]
    public async Task XClaim_Single_ForceAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key, delivered: 2);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "5-0", force: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Id, Is.EqualTo("5-0"));
            Assert.That((await this._redis.Stream.XPendingAsync(key, "group1")).Count, Is.EqualTo(3));
            Assert.That((await this.XClaimPendingAsync(key, "5-0")).Consumer, Is.EqualTo("consumer2"));
        }
    }

    [Test]
    public async Task XClaim_Single_Force_NotExistingIdAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key, delivered: 2);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "100-0", force: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(!result.HasValue || string.IsNullOrEmpty(result.Value.Id), Is.True);
            Assert.That((await this._redis.Stream.XPendingAsync(key, "group1")).Count, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task XClaim_Single_AfterXDelAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        Assert.That(await this._redis.Stream.XDelAsync(key, "2-0"), Is.EqualTo(1));
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "2-0", force: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(!result.HasValue || string.IsNullOrEmpty(result.Value.Id), Is.True);
            // Redis 7.0+：认领时发现原消息已删除，则清理该 ID 对应的 PEL。
            Assert.That((await this._redis.Stream.XPendingAsync(key, "group1", new PendingOptions("2-0", "2-0", 1)))?.Length ?? 0, Is.Zero);
        }
    }

    [Test]
    public async Task XClaim_Collection_MultipleAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        ICollection<EntryId> ids = ["1-0", "3-0", "5-0"];
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, ids);
        XClaimAssertIdsAsync(result, "1-0", "3-0", "5-0");
        foreach (var id in new[] { "1-0", "3-0", "5-0" })
            XClaimAssertOwnerAsync(await this.XClaimPendingAsync(key, id), id, "consumer2", 2);
    }

    [Test]
    public async Task XClaim_Collection_MixedAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        ICollection<EntryId> ids = ["1-0", "100-0", "3-0"];
        XClaimAssertIdsAsync(await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, ids), "1-0", "3-0");
    }

    [Test]
    public async Task XClaim_Collection_JustIdAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        ICollection<EntryId> ids = ["1-0", "2-0"];
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, ids, justid: true);
        XClaimAssertIdsAsync(result, "1-0", "2-0");
        Assert.That(result!.All(x => x.FieldValue is null), Is.True);
        XClaimAssertOwnerAsync(await this.XClaimPendingAsync(key, "1-0"), "1-0", "consumer2", 1);
        XClaimAssertOwnerAsync(await this.XClaimPendingAsync(key, "2-0"), "2-0", "consumer2", 1);
    }

    [Test]
    public async Task XClaim_Span_MultipleAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0,
            (ReadOnlySpan<EntryId>)["2-0", "4-0"]);
        XClaimAssertIdsAsync(result, "2-0", "4-0");
    }

    [Test]
    public async Task XClaim_Span_JustIdAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0,
            (ReadOnlySpan<EntryId>)["2-0", "4-0"], justid: true);
        XClaimAssertIdsAsync(result, "2-0", "4-0");
        Assert.That(result!.All(x => x.FieldValue is null), Is.True);
    }

    [Test]
    public async Task XClaim_Collection_NoMatchAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        ICollection<EntryId> ids = ["100-0", "101-0"];
        XClaimAssertIdsAsync(await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, ids));
    }

    [Test]
    public async Task XClaim_IdleAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "1-0", idle: 20_000);
        Assert.That(result.HasValue, Is.True);
        var pending = await this.XClaimPendingAsync(key, "1-0");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pending.Consumer, Is.EqualTo("consumer2"));
            Assert.That(pending.IdleTime, Is.GreaterThanOrEqualTo(10_000));
        }
    }

    [Test]
    public async Task XClaim_TimeAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        ulong timestamp = (ulong)DateTimeOffset.UtcNow.AddSeconds(-30).ToUnixTimeMilliseconds();
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "1-0", time: timestamp);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That((await this.XClaimPendingAsync(key, "1-0")).IdleTime, Is.GreaterThanOrEqualTo(20_000));
        }
    }

    [Test]
    public async Task XClaim_RetryCountAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "1-0", retryCount: 7);
        Assert.That(result.HasValue, Is.True);
        XClaimAssertOwnerAsync(await this.XClaimPendingAsync(key, "1-0"), "1-0", "consumer2", 7);
    }

    [Test]
    public async Task XClaim_LastIdAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key, delivered: 2);
        var result = await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "1-0", lastid: "5-0");
        Assert.That(result.HasValue, Is.True);
        // LASTID 将组的 last-delivered-id 设置为 5-0，故 > 不再获取 3-0、4-0、5-0。
        var unread = await this._redis.Stream.XReadGroupAsync(key, ">", "group1", "consumer3", count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(unread is null || unread.Value.Entries is null || unread.Value.Entries.Length == 0, Is.True);
            Assert.That((await this._redis.Stream.XPendingAsync(key, "group1")).Count, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task XClaim_Flags_NoneAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        Assert.That((await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "1-0", flags: CommandFlags.None))?.Id,
            Is.EqualTo("1-0"));
    }

    [Test]
    public async Task XClaim_CancellationToken_NoneAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        Assert.That((await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "1-0", cancellationToken: CancellationToken.None))?.Id,
            Is.EqualTo("1-0"));
    }

    [Test]
    public async Task XClaim_GroupNotExistsAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);
        await XClaimRedisErrorAsync<RedisErrorException>(async () => await this._redis.Stream.XClaimAsync(key, "missing", "consumer2", 0, "1-0"));
    }

    [Test]
    public async Task XClaim_KeyNotExistsAsync()
    {
        var key = XClaimKeyAsync();
        await XClaimRedisErrorAsync<RedisErrorException>(async () => await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "1-0"));
    }

    [Test]
    public async Task XClaim_WrongKeyTypeAsync()
    {
        var key = XClaimKeyAsync();
        await this._redis.String.SetAsync(key, "hello");
        await XClaimRedisErrorAsync<RedisErrorException>(async () => await this._redis.Stream.XClaimAsync(key, "group1", "consumer2", 0, "1-0"));
    }

    private async Task XClaimAssertTimeAsync(
        string key,
        string id,
        string consumer,
        long minIdle = 3_000,
        long maxIdle = 60_000)
    {
        var entries = await this._redis.Stream.XPendingAsync(
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
    public async Task XClaim_Time_TimeSpanAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);

        var result = await this._redis.Stream.XClaimAsync(
            key, "group1", "consumer2", 0, "1-0",
            time: TimeSpan.FromSeconds(-8));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Id, Is.EqualTo("1-0"));
        }

        await this.XClaimAssertTimeAsync(key, "1-0", "consumer2");
    }

    [Test]
    public async Task XClaim_Time_DateTimeAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);

        DateTime time = DateTime.UtcNow.AddSeconds(-8);

        var result = await this._redis.Stream.XClaimAsync(
            key, "group1", "consumer2", 0, "1-0",
            time: time);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Id, Is.EqualTo("1-0"));
        }

        await this.XClaimAssertTimeAsync(key, "1-0", "consumer2");
    }

    [Test]
    public async Task XClaim_Time_DateTimeOffsetAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);

        DateTimeOffset time = DateTimeOffset.UtcNow.AddSeconds(-8);

        var result = await this._redis.Stream.XClaimAsync(
            key, "group1", "consumer2", 0, "1-0",
            time: time);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Id, Is.EqualTo("1-0"));
        }

        await this.XClaimAssertTimeAsync(key, "1-0", "consumer2");
    }

    [Test]
    public async Task XClaim_Time_SharpExpiryAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);

        var time = new SharpExpiry(
            DateTimeOffset.UtcNow.AddSeconds(-8));

        var result = await this._redis.Stream.XClaimAsync(
            key, "group1", "consumer2", 0, "1-0",
            time: time);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Id, Is.EqualTo("1-0"));
        }

        await this.XClaimAssertTimeAsync(key, "1-0", "consumer2");
    }

    [Test]
    public async Task XClaim_Time_CollectionAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);

        ICollection<EntryId> ids = ["1-0", "2-0"];

        var result = await this._redis.Stream.XClaimAsync(
            key, "group1", "consumer2", 0, ids,
            time: DateTimeOffset.UtcNow.AddSeconds(-8));

        Assert.That(result, Has.Length.EqualTo(2));

        await this.XClaimAssertTimeAsync(key, "1-0", "consumer2");
        await this.XClaimAssertTimeAsync(key, "2-0", "consumer2");
    }

    [Test]
    public async Task XClaim_Time_SpanAsync()
    {
        var key = XClaimKeyAsync();
        await this.XClaimSeedAsync(key);

        var result = await this._redis.Stream.XClaimAsync(
            key, "group1", "consumer2", 0,
            ["1-0", "2-0"],
            time: DateTime.UtcNow.AddSeconds(-8));

        Assert.That(result, Has.Length.EqualTo(2));

        await this.XClaimAssertTimeAsync(key, "1-0", "consumer2");
        await this.XClaimAssertTimeAsync(key, "2-0", "consumer2");
    }

    #endregion

    #region XDelEx

    private static string XDelExKeyAsync() =>
        $"Stream:{{XDelExAsync}}:{Guid.NewGuid():N}";

    private static async Task XDelExRedisErrorAsync<TError>(Func<Task> action)
        where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    private async Task XDelExSeedAsync(string key)
    {
        for (int i = 1; i <= 5; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
    }

    private async Task XDelExCreateGroupAsync(string key, string group)
    {
        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, group, "0-0"), Is.True);
    }

    private async Task XDelExReadAsync(string key, string group, string consumer)
    {
        var read = await this._redis.Stream.XReadGroupAsync(
            key, ">", group, consumer, count: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(read!.Value.Entries, Has.Length.EqualTo(5));
        }
    }

    private async Task XDelExAssertPendingAsync(string key, string group, long expected)
    {
        var summary = await this._redis.Stream.XPendingAsync(key, group);
        Assert.That(summary.Count, Is.EqualTo(expected));
    }

    [Test]
    public async Task XDelEx_Single_DefaultAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelExAsync(key, "1-0"),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XDelEx_Single_NotFoundAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelExAsync(key, "99-0"),
                    Is.EqualTo(DeleteResult.NotFound));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XDelEx_Single_RepeatAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelExAsync(key, "1-0"),
            Is.EqualTo(DeleteResult.Deleted));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelExAsync(key, "1-0"),
                    Is.EqualTo(DeleteResult.NotFound));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XDelEx_Collection_AllAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        ICollection<EntryId> ids = ["1-0", "2-0", "3-0"];

        var result = await this._redis.Stream.XDelExAsync(key, ids);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                DeleteResult.Deleted, DeleteResult.Deleted, DeleteResult.Deleted
            ]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task XDelEx_Collection_MixedAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        ICollection<EntryId> ids = ["1-0", "99-0", "3-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelExAsync(key, ids),
                    Is.EqualTo(
                    [
                        DeleteResult.Deleted,
                        DeleteResult.NotFound,
                        DeleteResult.Deleted
                    ]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XDelEx_Span_AllAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);

        var result = await this._redis.Stream.XDelExAsync(
            key, (ReadOnlySpan<EntryId>)["1-0", "2-0", "3-0"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                DeleteResult.Deleted, DeleteResult.Deleted, DeleteResult.Deleted
            ]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task XDelEx_Span_MixedAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);

        var result = await this._redis.Stream.XDelExAsync(
            key, ["2-0", "99-0", "4-0"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
            [
                DeleteResult.Deleted, DeleteResult.NotFound, DeleteResult.Deleted
            ]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XDelEx_KeyNotExistsAsync()
    {
        var key = XDelExKeyAsync();

        Assert.That(await this._redis.Stream.XDelExAsync(key, "1-0"),
            Is.EqualTo(DeleteResult.NotFound));
        ICollection<EntryId> ids = ["1-0", "2-0"];
        Assert.That(await this._redis.Stream.XDelExAsync(key, ids),
            Is.EqualTo([DeleteResult.NotFound, DeleteResult.NotFound]));
    }

    [Test]
    public async Task XDelEx_KeepRef_DefaultAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");

        Assert.That(await this._redis.Stream.XDelExAsync(key, "1-0"),
            Is.EqualTo(DeleteResult.Deleted));
        await this.XDelExAssertPendingAsync(key, "group1", 5);
        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", "1-0"),
            Is.EqualTo(1));
    }

    [Test]
    public async Task XDelEx_KeepRef_ExplicitAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");

        Assert.That(await this._redis.Stream.XDelExAsync(
            key, "1-0", delPolicy: DeletionPolicy.KeepRef),
            Is.EqualTo(DeleteResult.Deleted));
        await this.XDelExAssertPendingAsync(key, "group1", 5);
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
    }

    [Test]
    public async Task XDelEx_DelRef_TwoGroupsAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExCreateGroupAsync(key, "group2");
        await this.XDelExReadAsync(key, "group1", "consumer1");
        await this.XDelExReadAsync(key, "group2", "consumer2");

        Assert.That(await this._redis.Stream.XDelExAsync(
            key, "1-0", delPolicy: DeletionPolicy.DelRef),
            Is.EqualTo(DeleteResult.Deleted));
        await this.XDelExAssertPendingAsync(key, "group1", 4);
        await this.XDelExAssertPendingAsync(key, "group2", 4);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(key, "group1", "1-0"), Is.Zero);
            Assert.That(await this._redis.Stream.XAckAsync(key, "group2", "1-0"), Is.Zero);
        }
    }

    [Test]
    public async Task XDelEx_DelRef_DanglingPendingAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");

        Assert.That(await this._redis.Stream.XDelAsync(key, "1-0"), Is.EqualTo(1));
        await this.XDelExAssertPendingAsync(key, "group1", 5);
        // 已没有 Stream 消息，但 DELREF 仍须清理悬挂的 PEL 引用。
        var result = await this._redis.Stream.XDelExAsync(
            key, "1-0", delPolicy: DeletionPolicy.DelRef);
        Assert.That(result, Is.EqualTo(DeleteResult.NotFound));
        await this.XDelExAssertPendingAsync(key, "group1", 4);
        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", "1-0"), Is.Zero);
    }

    [Test]
    public async Task XDelEx_Acked_NoGroupsAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelExAsync(
                    key, "1-0", delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XDelEx_Acked_NotReadAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelExAsync(
                    key, "1-0", delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo(DeleteResult.StillReferenced));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XDelEx_Acked_PendingAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");

        Assert.That(await this._redis.Stream.XDelExAsync(
            key, "1-0", delPolicy: DeletionPolicy.Acked),
            Is.EqualTo(DeleteResult.StillReferenced));
        await this.XDelExAssertPendingAsync(key, "group1", 5);
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
    }

    [Test]
    public async Task XDelEx_Acked_AfterAckAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XDelExAsync(
                key, "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XDelEx_Acked_TwoGroups_PartialAckAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExCreateGroupAsync(key, "group2");
        await this.XDelExReadAsync(key, "group1", "consumer1");
        await this.XDelExReadAsync(key, "group2", "consumer2");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XDelExAsync(
                key, "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.StillReferenced));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
        await this.XDelExAssertPendingAsync(key, "group2", 5);
    }

    [Test]
    public async Task XDelEx_Acked_TwoGroups_AllAckAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExCreateGroupAsync(key, "group2");
        await this.XDelExReadAsync(key, "group1", "consumer1");
        await this.XDelExReadAsync(key, "group2", "consumer2");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(key, "group1", "1-0"), Is.EqualTo(1));
            Assert.That(await this._redis.Stream.XAckAsync(key, "group2", "1-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XDelExAsync(
                key, "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.Deleted));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XDelEx_Acked_Collection_MixedAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");
        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", "1-0"), Is.EqualTo(1));
        ICollection<EntryId> ids = ["1-0", "2-0", "0-1"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelExAsync(
                    key, ids, delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo(
                    [
                        DeleteResult.Deleted,
                        DeleteResult.StillReferenced,
                        DeleteResult.NotFound
                    ]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XDelEx_Acked_Span_MixedAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");
        Assert.That(await this._redis.Stream.XAckAsync(key, "group1", "3-0"), Is.EqualTo(1));

        var result = await this._redis.Stream.XDelExAsync(
            key, (ReadOnlySpan<EntryId>)["2-0", "3-0", "0-1"],
            delPolicy: DeletionPolicy.Acked);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(
                [
                    DeleteResult.StillReferenced,
                    DeleteResult.Deleted,
                    DeleteResult.NotFound
                ]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XDelEx_Flags_NoneAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelExAsync(
            key, "1-0", flags: CommandFlags.None),
            Is.EqualTo(DeleteResult.Deleted));
    }

    [Test]
    public async Task XDelEx_CancellationToken_NoneAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);

        Assert.That(await this._redis.Stream.XDelExAsync(
            key, "1-0", cancellationToken: CancellationToken.None),
            Is.EqualTo(DeleteResult.Deleted));
    }

    [Test]
    public async Task XDelEx_WrongKeyTypeAsync()
    {
        var key = XDelExKeyAsync();
        await this._redis.String.SetAsync(key, "hello");

        await XDelExRedisErrorAsync<RedisErrorException>(async () =>
            await this._redis.Stream.XDelExAsync(key, "1-0"));
    }

    [Test]
    public async Task XDelEx_InvalidIdAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);

        await XDelExRedisErrorAsync<ArgumentException>(async () =>
            await this._redis.Stream.XDelExAsync(key, "18446744073709551616-0"));
    }

    [Test]
    public async Task XDelEx_Collection_EmptyAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        ICollection<EntryId> ids = [];

        await XDelExRedisErrorAsync<NotSupportedException>(async () =>
            await this._redis.Stream.XDelExAsync(key, ids));
    }

    [Test]
    public async Task XDelEx_Span_EmptyAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);

        await XDelExRedisErrorAsync<NotSupportedException>(async () =>
            await this._redis.Stream.XDelExAsync(key, []));
    }


    [Test]
    public async Task XDelEx_Acked_NonexistentAfterLastDeliveredAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            // 99-0 不在 Stream 中，但超过 group1 的 last-delivered-id (5-0)。
            Assert.That(await this._redis.Stream.XDelExAsync(
                key, "99-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.StillReferenced));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XDelEx_Acked_NonexistentBeforeLastDeliveredAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            // 0-1 不在 Stream 中，而且早于消费组的 last-delivered-id。
            Assert.That(await this._redis.Stream.XDelExAsync(
                key, "0-1", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.NotFound));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XDelEx_Acked_NoGroups_NotFoundAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);

        using (Assert.EnterMultipleScope())
        {
            // 无组时，不存在的 ID 仍返回 -1。
            Assert.That(await this._redis.Stream.XDelExAsync(
                key, "99-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.NotFound));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XDelEx_Acked_TwoGroups_OneNotReadAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExCreateGroupAsync(key, "group2");
        await this.XDelExReadAsync(key, "group1", "consumer1");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(
                    key, "group1", "1-0"), Is.EqualTo(1));

            // group2 尚未读过消息，所以即使 group1 已 ACK，也不能删除。
            Assert.That(await this._redis.Stream.XDelExAsync(
                key, "1-0", delPolicy: DeletionPolicy.Acked),
                Is.EqualTo(DeleteResult.StillReferenced));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XDelEx_KeepRef_TwoGroupsAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExCreateGroupAsync(key, "group2");
        await this.XDelExReadAsync(key, "group1", "consumer1");
        await this.XDelExReadAsync(key, "group2", "consumer2");

        Assert.That(await this._redis.Stream.XDelExAsync(
            key, "1-0", delPolicy: DeletionPolicy.KeepRef),
            Is.EqualTo(DeleteResult.Deleted));
        await this.XDelExAssertPendingAsync(key, "group1", 5);
        await this.XDelExAssertPendingAsync(key, "group2", 5);
        Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(4));
    }

    [Test]
    public async Task XDelEx_DelRef_Collection_MixedAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");
        ICollection<EntryId> ids = ["1-0", "0-1", "3-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelExAsync(
                    key, ids, delPolicy: DeletionPolicy.DelRef),
                    Is.EqualTo(
                    [
                        DeleteResult.Deleted,
                        DeleteResult.NotFound,
                        DeleteResult.Deleted,
                    ]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(3));
        }
        await this.XDelExAssertPendingAsync(key, "group1", 3);
    }

    [Test]
    public async Task XDelEx_DelRef_Span_MixedAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelExAsync(
                    key, ["2-0", "0-1", "4-0"],
                    delPolicy: DeletionPolicy.DelRef),
                    Is.EqualTo(
                    [
                        DeleteResult.Deleted,
                        DeleteResult.NotFound,
                        DeleteResult.Deleted
                    ]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(3));
        }
        await this.XDelExAssertPendingAsync(key, "group1", 3);
    }

    [Test]
    public async Task XDelEx_Acked_Collection_AllAcknowledgedAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(
                    key, "group1", "1-0"), Is.EqualTo(1));
            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "2-0"), Is.EqualTo(1));
        }
        ICollection<EntryId> ids = ["1-0", "2-0"];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelExAsync(
                    key, ids, delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo([DeleteResult.Deleted, DeleteResult.Deleted]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(3));
        }
        await this.XDelExAssertPendingAsync(key, "group1", 3);
    }

    [Test]
    public async Task XDelEx_Acked_Span_AllPendingAsync()
    {
        var key = XDelExKeyAsync();
        await this.XDelExSeedAsync(key);
        await this.XDelExCreateGroupAsync(key, "group1");
        await this.XDelExReadAsync(key, "group1", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelExAsync(
                    key, (ReadOnlySpan<EntryId>)["1-0", "2-0"],
                    delPolicy: DeletionPolicy.Acked),
                    Is.EqualTo(
                    [
                        DeleteResult.StillReferenced,
                        DeleteResult.StillReferenced,
                    ]));
            Assert.That(await this._redis.Stream.XLenAsync(key), Is.EqualTo(5));
        }
        await this.XDelExAssertPendingAsync(key, "group1", 5);
    }

    #endregion

    #region XGroupCreateConsumer

    private static string XGroupCreateConsumerKeyAsync() =>
        $"Stream:{{XGroupCreateConsumerAsync}}:{Guid.NewGuid():N}";

    private async Task XGroupCreateConsumerPrepareAsync(string key)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                    key, "1-0", "field", "value"),
                    Is.EqualTo("1-0"));

            Assert.That(await this._redis.Stream.XGroupCreateAsync(
                key, "group1", "0-0"), Is.True);
        }
    }

    private static async Task XGroupCreateConsumerRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XGroupCreateConsumer_NewConsumerAsync()
    {
        var key = XGroupCreateConsumerKeyAsync();
        await this.XGroupCreateConsumerPrepareAsync(key);

        var result = await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer1");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task XGroupCreateConsumer_ExistingConsumerAsync()
    {
        var key = XGroupCreateConsumerKeyAsync();
        await this.XGroupCreateConsumerPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer1"), Is.True);

        Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer1"), Is.False);
    }

    [Test]
    public async Task XGroupCreateConsumer_MultipleConsumersAsync()
    {
        var key = XGroupCreateConsumerKeyAsync();
        await this.XGroupCreateConsumerPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                    key, "group1", "consumer1"), Is.True);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer2"), Is.True);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer3"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                    key, "group1", "consumer1"), Is.False);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer2"), Is.False);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer3"), Is.False);
        }
    }

    [Test]
    public async Task XGroupCreateConsumer_DifferentGroupsAsync()
    {
        var key = XGroupCreateConsumerKeyAsync();
        await this.XGroupCreateConsumerPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateAsync(
                    key, "group2", "0-0"), Is.True);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer1"), Is.True);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group2", "consumer1"), Is.True);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                    key, "group1", "consumer1"), Is.False);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group2", "consumer1"), Is.False);
        }
    }

    [Test]
    public async Task XGroupCreateConsumer_ExistingFromXReadGroupAsync()
    {
        var key = XGroupCreateConsumerKeyAsync();
        await this.XGroupCreateConsumerPrepareAsync(key);

        // XREADGROUP 自动创建 consumer1
        var entries = await this._redis.Stream.XReadGroupAsync(
            key, ">", "group1", "consumer1", count: 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(entries.HasValue, Is.True);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer1"), Is.False);
        }

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(summary.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task XGroupCreateConsumer_NoPendingEntriesAsync()
    {
        var key = XGroupCreateConsumerKeyAsync();
        await this.XGroupCreateConsumerPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer1"), Is.True);

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.Zero);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XGroupCreateConsumer_StreamNotExistsAsync()
    {
        var key = XGroupCreateConsumerKeyAsync();

        await XGroupCreateConsumerRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer1"));
    }

    [Test]
    public async Task XGroupCreateConsumer_GroupNotExistsAsync()
    {
        var key = XGroupCreateConsumerKeyAsync();
        await this.XGroupCreateConsumerPrepareAsync(key);

        await XGroupCreateConsumerRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "not_exists", "consumer1"));
    }

    [Test]
    public async Task XGroupCreateConsumer_WrongKeyTypeAsync()
    {
        var key = XGroupCreateConsumerKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await XGroupCreateConsumerRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer1"));
    }

    [Test]
    public async Task XGroupCreateConsumer_Flags_NoneAsync()
    {
        var key = XGroupCreateConsumerKeyAsync();
        await this.XGroupCreateConsumerPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer1",
            flags: CommandFlags.None), Is.True);
    }

    [Test]
    public async Task XGroupCreateConsumer_CancellationToken_DefaultAsync()
    {
        var key = XGroupCreateConsumerKeyAsync();
        await this.XGroupCreateConsumerPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer1",
            cancellationToken: CancellationToken.None), Is.True);
    }

    #endregion

    #region XGroupDelConsumer

    private static string XGroupDelConsumerKeyAsync() =>
        $"Stream:{{XGroupDelConsumerAsync}}:{Guid.NewGuid():N}";

    private async Task XGroupDelConsumerPrepareAsync(string key)
    {
        for (int i = 1; i <= 5; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, "group1", "0-0"), Is.True);
    }

    private async Task XGroupDelConsumerReadAsync(
        string key, string consumer, int count)
    {
        var result = await this._redis.Stream.XReadGroupAsync(
            key, ">", "group1", consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries, Has.Length.EqualTo(count));
        }
    }

    private async Task XGroupDelConsumerAssertPendingAsync(
        string key, long count)
    {
        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(summary.Count, Is.EqualTo(count));
    }

    private static async Task XGroupDelConsumerRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XGroupDelConsumer_Existing_NoPendingAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                    key, "group1", "consumer1"), Is.True);

            Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
                key, "group1", "consumer1"), Is.Zero);
        }

        Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer1"), Is.True);

        await this.XGroupDelConsumerAssertPendingAsync(key, 0);

        Assert.That(await this._redis.Stream.XLenAsync(key),
            Is.EqualTo(5));
    }

    [Test]
    public async Task XGroupDelConsumer_NotExistsAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
            key, "group1", "not_exists"), Is.Zero);

        await this.XGroupDelConsumerAssertPendingAsync(key, 0);
    }

    [Test]
    public async Task XGroupDelConsumer_RepeatDeleteAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                    key, "group1", "consumer1"), Is.True);

            Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
                key, "group1", "consumer1"), Is.Zero);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
                    key, "group1", "consumer1"), Is.Zero);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer1"), Is.True);
        }
    }

    [Test]
    public async Task XGroupDelConsumer_OnePendingAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);
        await this.XGroupDelConsumerReadAsync(key, "consumer1", 1);

        await this.XGroupDelConsumerAssertPendingAsync(key, 1);

        Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
            key, "group1", "consumer1"), Is.EqualTo(1));

        await this.XGroupDelConsumerAssertPendingAsync(key, 0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XLenAsync(key),
                    Is.EqualTo(5));

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer1"), Is.True);
        }
    }

    [Test]
    public async Task XGroupDelConsumer_MultiplePendingAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);
        await this.XGroupDelConsumerReadAsync(key, "consumer1", 3);

        await this.XGroupDelConsumerAssertPendingAsync(key, 3);

        Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
            key, "group1", "consumer1"), Is.EqualTo(3));

        await this.XGroupDelConsumerAssertPendingAsync(key, 0);

        Assert.That(await this._redis.Stream.XLenAsync(key),
            Is.EqualTo(5));
    }

    [Test]
    public async Task XGroupDelConsumer_AfterAckAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);
        await this.XGroupDelConsumerReadAsync(key, "consumer1", 3);

        Assert.That(await this._redis.Stream.XAckAsync(
            key, "group1", "1-0"), Is.EqualTo(1));

        await this.XGroupDelConsumerAssertPendingAsync(key, 2);

        Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
            key, "group1", "consumer1"), Is.EqualTo(2));

        await this.XGroupDelConsumerAssertPendingAsync(key, 0);

        Assert.That(await this._redis.Stream.XLenAsync(key),
            Is.EqualTo(5));
    }

    [Test]
    public async Task XGroupDelConsumer_AllAcknowledgedAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);
        await this.XGroupDelConsumerReadAsync(key, "consumer1", 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(
                    key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "2-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
                key, "group1", "consumer1"), Is.Zero);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer1"), Is.True);
        }

        await this.XGroupDelConsumerAssertPendingAsync(key, 0);
    }

    [Test]
    public async Task XGroupDelConsumer_MultipleConsumersAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);

        await this.XGroupDelConsumerReadAsync(key, "consumer1", 2);
        await this.XGroupDelConsumerReadAsync(key, "consumer2", 3);

        await this.XGroupDelConsumerAssertPendingAsync(key, 5);

        Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
            key, "group1", "consumer1"), Is.EqualTo(2));

        await this.XGroupDelConsumerAssertPendingAsync(key, 3);

        var entries = await this._redis.Stream.XPendingAsync(
            key, "group1", new PendingOptions("-", "+", 10));

        Assert.That(entries, Has.Length.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entries!.Select(x => x.Id),
                    Is.EqualTo(["3-0", "4-0", "5-0"]));

            Assert.That(entries.All(x => x.Consumer == "consumer2"),
                Is.True);

            Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
                key, "group1", "consumer2"), Is.EqualTo(3));
        }

        await this.XGroupDelConsumerAssertPendingAsync(key, 0);

        Assert.That(await this._redis.Stream.XLenAsync(key),
            Is.EqualTo(5));
    }

    [Test]
    public async Task XGroupDelConsumer_DifferentGroupsAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, "group2", "0-0"), Is.True);

        await this.XGroupDelConsumerReadAsync(key, "consumer1", 2);

        var group2Read = await this._redis.Stream.XReadGroupAsync(
            key, ">", "group2", "consumer1", count: 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group2Read.HasValue, Is.True);

            Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
                key, "group1", "consumer1"), Is.EqualTo(2));
        }

        var group1 = await this._redis.Stream.XPendingAsync(
            key, "group1");

        var group2 = await this._redis.Stream.XPendingAsync(
            key, "group2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group1.Count, Is.Zero);
            Assert.That(group2.Count, Is.EqualTo(3));

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group2", "consumer1"), Is.False);
        }
    }

    [Test]
    public async Task XGroupDelConsumer_PendingCannotAckAfterDeleteAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);
        await this.XGroupDelConsumerReadAsync(key, "consumer1", 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
                    key, "group1", "consumer1"), Is.EqualTo(2));

            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "1-0"), Is.Zero);

            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "2-0"), Is.Zero);
        }

        await this.XGroupDelConsumerAssertPendingAsync(key, 0);
    }

    [Test]
    public async Task XGroupDelConsumer_CanReadRemainingMessagesAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);
        await this.XGroupDelConsumerReadAsync(key, "consumer1", 2);

        Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
            key, "group1", "consumer1"), Is.EqualTo(2));

        await this.XGroupDelConsumerReadAsync(key, "consumer2", 3);

        await this.XGroupDelConsumerAssertPendingAsync(key, 3);
    }

    [Test]
    public async Task XGroupDelConsumer_StreamNotExistsAsync()
    {
        var key = XGroupDelConsumerKeyAsync();

        await XGroupDelConsumerRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XGroupDelConsumerAsync(
                key, "group1", "consumer1"));
    }

    [Test]
    public async Task XGroupDelConsumer_GroupNotExistsAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);

        await XGroupDelConsumerRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XGroupDelConsumerAsync(
                key, "not_exists", "consumer1"));
    }

    [Test]
    public async Task XGroupDelConsumer_WrongKeyTypeAsync()
    {
        var key = XGroupDelConsumerKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await XGroupDelConsumerRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XGroupDelConsumerAsync(
                key, "group1", "consumer1"));
    }

    [Test]
    public async Task XGroupDelConsumer_Flags_NoneAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);
        await this.XGroupDelConsumerReadAsync(key, "consumer1", 1);

        Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
            key, "group1", "consumer1",
            flags: CommandFlags.None), Is.EqualTo(1));
    }

    [Test]
    public async Task XGroupDelConsumer_CancellationToken_DefaultAsync()
    {
        var key = XGroupDelConsumerKeyAsync();
        await this.XGroupDelConsumerPrepareAsync(key);
        await this.XGroupDelConsumerReadAsync(key, "consumer1", 1);

        Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
            key, "group1", "consumer1",
            cancellationToken: CancellationToken.None),
            Is.EqualTo(1));
    }

    #endregion

    #region XGroupDestroy

    private static string XGroupDestroyKeyAsync() =>
        $"Stream:{{XGroupDestroyAsync}}:{Guid.NewGuid():N}";

    private async Task XGroupDestroyPrepareAsync(
        string key, int count = 5)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, "group1", "0-0"), Is.True);
    }

    private async Task XGroupDestroyReadAsync(
        string key, string group, string consumer, int count)
    {
        var result = await this._redis.Stream.XReadGroupAsync(
            key, ">", group, consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries,
                Has.Length.EqualTo(count));
        }
    }

    private static async Task XGroupDestroyRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XGroupDestroy_ExistingAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                    key, "group1"), Is.True);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XGroupDestroy_NotExistsAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                    key, "not_exists"), Is.False);

            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                key, "group1"), Is.True);
        }
    }

    [Test]
    public async Task XGroupDestroy_RepeatAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupDestroyAsync(
            key, "group1"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                    key, "group1"), Is.False);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XGroupDestroy_WithPendingAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);
        await this.XGroupDestroyReadAsync(
            key, "group1", "consumer1", 3);

        var before = await this._redis.Stream.XPendingAsync(
            key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before.Count, Is.EqualTo(3));

            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                key, "group1"), Is.True);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(5));
        }

        await XGroupDestroyRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XPendingAsync(
                key, "group1"));
    }

    [Test]
    public async Task XGroupDestroy_WithMultipleConsumersAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);

        await this.XGroupDestroyReadAsync(
            key, "group1", "consumer1", 2);

        await this.XGroupDestroyReadAsync(
            key, "group1", "consumer2", 3);

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.EqualTo(5));

            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                key, "group1"), Is.True);
        }

        await XGroupDestroyRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XPendingAsync(
                key, "group1"));

        Assert.That(await this._redis.Stream.XLenAsync(key),
            Is.EqualTo(5));
    }

    [Test]
    public async Task XGroupDestroy_WithoutPendingAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.Zero);

            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                key, "group1"), Is.True);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XGroupDestroy_AfterAllAckAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);

        await this.XGroupDestroyReadAsync(
            key, "group1", "consumer1", 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(
                    key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "2-0"), Is.EqualTo(1));
        }

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.Zero);

            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                key, "group1"), Is.True);
        }
    }

    [Test]
    public async Task XGroupDestroy_MultipleGroupsAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, "group2", "0-0"), Is.True);

        await this.XGroupDestroyReadAsync(
            key, "group1", "consumer1", 2);

        await this.XGroupDestroyReadAsync(
            key, "group2", "consumer2", 3);

        var group1Before = await this._redis.Stream.XPendingAsync(
            key, "group1");

        var group2Before = await this._redis.Stream.XPendingAsync(
            key, "group2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group1Before.Count, Is.EqualTo(2));
            Assert.That(group2Before.Count, Is.EqualTo(3));

            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                key, "group1"), Is.True);
        }

        var group2After = await this._redis.Stream.XPendingAsync(
            key, "group2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group2After.Count, Is.EqualTo(3));

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group2", "consumer2"), Is.False);

            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                key, "group2"), Is.True);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XGroupDestroy_RecreateAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);

        await this.XGroupDestroyReadAsync(
            key, "group1", "consumer1", 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                    key, "group1"), Is.True);

            Assert.That(await this._redis.Stream.XGroupCreateAsync(
                key, "group1", "0-0"), Is.True);
        }

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(summary.Count, Is.Zero);

        await this.XGroupDestroyReadAsync(
            key, "group1", "consumer2", 5);

        summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(summary.Count, Is.EqualTo(5));
    }

    [Test]
    public async Task XGroupDestroy_AfterDestroy_XReadGroupAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupDestroyAsync(
            key, "group1"), Is.True);

        await XGroupDestroyRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XReadGroupAsync(
                key, ">", "group1", "consumer1", count: 1));
    }

    [Test]
    public async Task XGroupDestroy_StreamNotExistsAsync()
    {
        var key = XGroupDestroyKeyAsync();

        await XGroupDestroyRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XGroupDestroyAsync(
                key, "group1"));
    }

    [Test]
    public async Task XGroupDestroy_WrongKeyTypeAsync()
    {
        var key = XGroupDestroyKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await XGroupDestroyRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XGroupDestroyAsync(
                key, "group1"));
    }

    [Test]
    public async Task XGroupDestroy_Flags_NoneAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupDestroyAsync(
            key, "group1",
            flags: CommandFlags.None), Is.True);
    }

    [Test]
    public async Task XGroupDestroy_CancellationToken_DefaultAsync()
    {
        var key = XGroupDestroyKeyAsync();
        await this.XGroupDestroyPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupDestroyAsync(
            key, "group1",
            cancellationToken: CancellationToken.None), Is.True);
    }

    #endregion

    #region XGroupSetID

    private static string XGroupSetIDKeyAsync() =>
        $"Stream:{{XGroupSetIDAsync}}:{Guid.NewGuid():N}";

    private async Task XGroupSetIDPrepareAsync(string key)
    {
        for (int i = 1; i <= 5; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, "group1", "0-0"), Is.True);
    }

    private async Task XGroupSetIDAssertReadAsync(
        string key, string consumer, params string[] expectedIds)
    {
        var result = await this._redis.Stream.XReadGroupAsync(
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

    private static async Task XGroupSetIDRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XGroupSetID_ZeroAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupSetIDAsync(
            key, "group1", "0-0"), Is.True);

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer1",
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XGroupSetID_MiddleIDAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupSetIDAsync(
            key, "group1", "3-0"), Is.True);

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer1", "4-0", "5-0");
    }

    [Test]
    public async Task XGroupSetID_LastIDAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupSetIDAsync(
            key, "group1", "5-0"), Is.True);

        var result = await this._redis.Stream.XReadGroupAsync(
            key, ">", "group1", "consumer1", count: 10);

        Assert.That(result.HasValue &&
            (result.Value.Entries?.Length ?? 0) > 0, Is.False);

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(summary.Count, Is.Zero);
    }

    [Test]
    public async Task XGroupSetID_DollarAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupSetIDAsync(
                    key, "group1", "$"), Is.True);

            Assert.That(await this._redis.Stream.XAddAsync(
                key, "6-0", "field", "value6"),
                Is.EqualTo("6-0"));
        }

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer1", "6-0");
    }

    [Test]
    public async Task XGroupSetID_RewindAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer1",
            "1-0", "2-0", "3-0", "4-0", "5-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(
                    key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XGroupSetIDAsync(
                key, "group1", "0-0"), Is.True);
        }

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer2",
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XGroupSetID_PreservesPELAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        var read = await this._redis.Stream.XReadGroupAsync(
            key, ">", "group1", "consumer1", count: 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(read!.Value.Entries, Has.Length.EqualTo(2));
        }

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.EqualTo(2));

            Assert.That(await this._redis.Stream.XGroupSetIDAsync(
                key, "group1", "4-0"), Is.True);
        }

        summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(summary.Count, Is.EqualTo(2));

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer2", "5-0");

        summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(summary.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task XGroupSetID_EntriesRead_ZeroAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupSetIDAsync(
            key, "group1", "0-0",
            entriesRead: 0), Is.True);

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer1",
            "1-0", "2-0", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XGroupSetID_EntriesRead_MiddleAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupSetIDAsync(
            key, "group1", "3-0",
            entriesRead: 3), Is.True);

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer1", "4-0", "5-0");
    }

    [Test]
    public async Task XGroupSetID_EntriesRead_LastAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupSetIDAsync(
                    key, "group1", "5-0",
                    entriesRead: 5), Is.True);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(5));
        }

        var summary = await this._redis.Stream.XPendingAsync(
            key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(summary.Count, Is.Zero);

            Assert.That(await this._redis.Stream.XAddAsync(
                key, "6-0", "field", "value6"),
                Is.EqualTo("6-0"));
        }

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer1", "6-0");
    }

    [Test]
    public async Task XGroupSetID_DifferentGroupsAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateAsync(
                    key, "group2", "0-0"), Is.True);

            Assert.That(await this._redis.Stream.XGroupSetIDAsync(
                key, "group1", "3-0",
                entriesRead: 3), Is.True);
        }

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer1", "4-0", "5-0");

        var group2 = await this._redis.Stream.XReadGroupAsync(
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

        var group1Pending = await this._redis.Stream.XPendingAsync(
            key, "group1");

        var group2Pending = await this._redis.Stream.XPendingAsync(
            key, "group2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group1Pending.Count, Is.EqualTo(2));
            Assert.That(group2Pending.Count, Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XGroupSetID_Flags_NoneAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupSetIDAsync(
            key, "group1", "2-0",
            flags: CommandFlags.None), Is.True);

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer1", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XGroupSetID_CancellationToken_DefaultAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupSetIDAsync(
            key, "group1", "2-0",
            cancellationToken: CancellationToken.None), Is.True);

        await this.XGroupSetIDAssertReadAsync(
            key, "consumer1", "3-0", "4-0", "5-0");
    }

    [Test]
    public async Task XGroupSetID_StreamNotExistsAsync()
    {
        var key = XGroupSetIDKeyAsync();

        await XGroupSetIDRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XGroupSetIDAsync(
                key, "group1", "0-0"));
    }

    [Test]
    public async Task XGroupSetID_GroupNotExistsAsync()
    {
        var key = XGroupSetIDKeyAsync();
        await this.XGroupSetIDPrepareAsync(key);

        await XGroupSetIDRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XGroupSetIDAsync(
                key, "not_exists", "0-0"));
    }

    [Test]
    public async Task XGroupSetID_WrongKeyTypeAsync()
    {
        var key = XGroupSetIDKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await XGroupSetIDRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XGroupSetIDAsync(
                key, "group1", "0-0"));
    }

    #endregion

    #region XInfoConsumers

    private static string XInfoConsumersKeyAsync() =>
        $"Stream:{{XInfoConsumersAsync}}:{Guid.NewGuid():N}";

    private async Task XInfoConsumersPrepareAsync(string key)
    {
        for (int i = 1; i <= 5; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, "group1", "0-0"), Is.True);
    }

    private async Task XInfoConsumersReadAsync(
        string key, string group, string consumer, int count)
    {
        var result = await this._redis.Stream.XReadGroupAsync(
            key, ">", group, consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries,
                Has.Length.EqualTo(count));
        }
    }

    private async Task<ConsumerInfo> XInfoConsumersFindAsync(
        string key, string group, string consumer)
    {
        var consumers = await this._redis.Stream.XInfoConsumersAsync(
            key, group);

        Assert.That(consumers, Is.Not.Null);

        return consumers!.Single(x => x.Name == consumer);
    }

    private static void XInfoConsumersAssertTimeAsync(
        ConsumerInfo consumer)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(consumer.Idle,
                Is.GreaterThanOrEqualTo(0));

            if (consumer.Inactive.HasValue)
            {
                Assert.That(consumer.Inactive.Value,
                    Is.GreaterThanOrEqualTo(-1));
            }
        }
    }

    private static async Task XInfoConsumersRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XInfoConsumers_EmptyAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        var result = await this._redis.Stream.XInfoConsumersAsync(
            key, "group1");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task XInfoConsumers_CreateConsumerAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer1"), Is.True);

        var result = await this._redis.Stream.XInfoConsumersAsync(
            key, "group1");

        Assert.That(result, Has.Length.EqualTo(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result![0].Name,
                Is.EqualTo("consumer1"));

            Assert.That(result[0].Pending, Is.Zero);
        }

        XInfoConsumersAssertTimeAsync(result![0]);
    }

    [Test]
    public async Task XInfoConsumers_AutoCreatedByReadAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        await this.XInfoConsumersReadAsync(
            key, "group1", "consumer1", 2);

        var consumer = await this.XInfoConsumersFindAsync(
            key, "group1", "consumer1");

        Assert.That(consumer.Pending, Is.EqualTo(2));
        XInfoConsumersAssertTimeAsync(consumer);
    }

    [Test]
    public async Task XInfoConsumers_MultipleConsumersAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        await this.XInfoConsumersReadAsync(
            key, "group1", "consumer1", 2);

        await this.XInfoConsumersReadAsync(
            key, "group1", "consumer2", 3);

        Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer3"), Is.True);

        var result = await this._redis.Stream.XInfoConsumersAsync(
            key, "group1");

        Assert.That(result, Has.Length.EqualTo(3));

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
    public async Task XInfoConsumers_AfterAckAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        await this.XInfoConsumersReadAsync(
            key, "group1", "consumer1", 3);

        var consumer = await this.XInfoConsumersFindAsync(
            key, "group1", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(consumer.Pending, Is.EqualTo(3));

            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "1-0"), Is.EqualTo(1));
        }

        consumer = await this.XInfoConsumersFindAsync(
            key, "group1", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(consumer.Pending, Is.EqualTo(2));

            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "2-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "3-0"), Is.EqualTo(1));
        }

        consumer = await this.XInfoConsumersFindAsync(
            key, "group1", "consumer1");

        Assert.That(consumer.Pending, Is.Zero);
    }

    [Test]
    public async Task XInfoConsumers_AfterClaimAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        await this.XInfoConsumersReadAsync(
            key, "group1", "consumer1", 3);

        var claimed = await this._redis.Stream.XClaimAsync(
            key, "group1", "consumer2", 0, "1-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(claimed.HasValue, Is.True);
            Assert.That(claimed!.Value.Id, Is.EqualTo("1-0"));
        }

        var result = await this._redis.Stream.XInfoConsumersAsync(
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
    public async Task XInfoConsumers_AfterDelConsumerAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        await this.XInfoConsumersReadAsync(
            key, "group1", "consumer1", 2);

        await this.XInfoConsumersReadAsync(
            key, "group1", "consumer2", 3);

        Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
            key, "group1", "consumer1"), Is.EqualTo(2));

        var result = await this._redis.Stream.XInfoConsumersAsync(
            key, "group1");

        Assert.That(result, Has.Length.EqualTo(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result![0].Name, Is.EqualTo("consumer2"));
            Assert.That(result[0].Pending, Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XInfoConsumers_AfterAllConsumersDeletedAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                    key, "group1", "consumer1"), Is.True);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer2"), Is.True);

            Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
                key, "group1", "consumer1"), Is.Zero);

            Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
                key, "group1", "consumer2"), Is.Zero);

            Assert.That(await this._redis.Stream.XInfoConsumersAsync(
                key, "group1"), Is.Null);
        }
    }

    [Test]
    public async Task XInfoConsumers_DifferentGroupsAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, "group2", "0-0"), Is.True);

        await this.XInfoConsumersReadAsync(
            key, "group1", "consumer1", 2);

        await this.XInfoConsumersReadAsync(
            key, "group2", "consumer1", 4);

        var group1 = await this.XInfoConsumersFindAsync(
            key, "group1", "consumer1");

        var group2 = await this.XInfoConsumersFindAsync(
            key, "group2", "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group1.Pending, Is.EqualTo(2));
            Assert.That(group2.Pending, Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XInfoConsumers_AfterXDelAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        await this.XInfoConsumersReadAsync(
            key, "group1", "consumer1", 2);

        Assert.That(await this._redis.Stream.XDelAsync(
            key, "1-0"), Is.EqualTo(1));

        var consumer = await this.XInfoConsumersFindAsync(
            key, "group1", "consumer1");

        Assert.That(consumer.Pending, Is.EqualTo(2));
    }

    [Test]
    public async Task XInfoConsumers_Idle_InactiveAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        await this.XInfoConsumersReadAsync(
            key, "group1", "consumer1", 1);

        var consumer = await this.XInfoConsumersFindAsync(
            key, "group1", "consumer1");

        XInfoConsumersAssertTimeAsync(consumer);
        Assert.That(consumer.Pending, Is.EqualTo(1));

        var again = await this.XInfoConsumersFindAsync(
            key, "group1", "consumer1");

        Assert.That(again.Pending, Is.EqualTo(1));
        XInfoConsumersAssertTimeAsync(again);
    }

    [Test]
    public async Task XInfoConsumers_Flags_NoneAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer1"), Is.True);

        var result = await this._redis.Stream.XInfoConsumersAsync(
            key, "group1", flags: CommandFlags.None);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result![0].Name, Is.EqualTo("consumer1"));
    }

    [Test]
    public async Task XInfoConsumers_CancellationToken_DefaultAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer1"), Is.True);

        var result = await this._redis.Stream.XInfoConsumersAsync(
            key, "group1",
            cancellationToken: CancellationToken.None);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result![0].Name, Is.EqualTo("consumer1"));
    }

    [Test]
    public async Task XInfoConsumers_StreamNotExistsAsync()
    {
        var key = XInfoConsumersKeyAsync();

        await XInfoConsumersRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XInfoConsumersAsync(
                key, "group1"));
    }

    [Test]
    public async Task XInfoConsumers_GroupNotExistsAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        await XInfoConsumersRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XInfoConsumersAsync(
                key, "not_exists"));
    }

    [Test]
    public async Task XInfoConsumers_WrongKeyTypeAsync()
    {
        var key = XInfoConsumersKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await XInfoConsumersRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XInfoConsumersAsync(
                key, "group1"));
    }

    [Test]
    public async Task XInfoConsumers_AfterGroupDestroyAsync()
    {
        var key = XInfoConsumersKeyAsync();
        await this.XInfoConsumersPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                    key, "group1", "consumer1"), Is.True);

            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                key, "group1"), Is.True);
        }

        await XInfoConsumersRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XInfoConsumersAsync(
                key, "group1"));
    }

    #endregion

    #region XInfoGroups

    private static string XInfoGroupsKeyAsync() =>
        $"Stream:{{XInfoGroupsAsync}}:{Guid.NewGuid():N}";

    private async Task XInfoGroupsPrepareAsync(string key, int count = 5)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }
    }

    private async Task XInfoGroupsCreateAsync(
        string key, string group = "group1")
    {
        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, group, "0-0"), Is.True);
    }

    private async Task XInfoGroupsReadAsync(
        string key, string group, string consumer, int count)
    {
        var read = await this._redis.Stream.XReadGroupAsync(
            key, ">", group, consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.HasValue, Is.True);
            Assert.That(read!.Value.Entries,
                Has.Length.EqualTo(count));
        }
    }

    private async Task<GroupInfo> XInfoGroupsFindAsync(
        string key, string group)
    {
        var groups = await this._redis.Stream.XInfoGroupsAsync(key);
        Assert.That(groups, Is.Not.Null);

        return groups!.Single(x => x.Name == group);
    }

    private static void XInfoGroupsAssertCountersAsync(
        GroupInfo group, long entriesRead, long lag)
    {
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

    private static async Task XInfoGroupsRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XInfoGroups_NoGroupsAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);

        var result = await this._redis.Stream.XInfoGroupsAsync(key);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task XInfoGroups_EmptyGroupAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);
        await this.XInfoGroupsCreateAsync(key);

        var result = await this._redis.Stream.XInfoGroupsAsync(key);

        Assert.That(result, Has.Length.EqualTo(1));

        var group = result![0];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Name, Is.EqualTo("group1"));
            Assert.That(group.Consumers, Is.Zero);
            Assert.That(group.Pending, Is.Zero);
            Assert.That(group.LastDeliveredId, Is.EqualTo("0-0"));
        }

        XInfoGroupsAssertCountersAsync(group, 0, 5);
    }

    [Test]
    public async Task XInfoGroups_MultipleGroupsAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);

        await this.XInfoGroupsCreateAsync(key, "group1");
        await this.XInfoGroupsCreateAsync(key, "group2");
        await this.XInfoGroupsCreateAsync(key, "group3");

        var result = await this._redis.Stream.XInfoGroupsAsync(key);

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

            XInfoGroupsAssertCountersAsync(group, 0, 5);
        }
    }

    [Test]
    public async Task XInfoGroups_CreateConsumerAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);
        await this.XInfoGroupsCreateAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                    key, "group1", "consumer1"), Is.True);

            Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
                key, "group1", "consumer2"), Is.True);
        }

        var group = await this.XInfoGroupsFindAsync(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Consumers, Is.EqualTo(2));
            Assert.That(group.Pending, Is.Zero);
        }

        XInfoGroupsAssertCountersAsync(group, 0, 5);
    }

    [Test]
    public async Task XInfoGroups_AfterReadAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);
        await this.XInfoGroupsCreateAsync(key);

        await this.XInfoGroupsReadAsync(
            key, "group1", "consumer1", 2);

        var group = await this.XInfoGroupsFindAsync(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Name, Is.EqualTo("group1"));
            Assert.That(group.Consumers, Is.EqualTo(1));
            Assert.That(group.Pending, Is.EqualTo(2));
            Assert.That(group.LastDeliveredId, Is.EqualTo("2-0"));
        }

        XInfoGroupsAssertCountersAsync(group, 2, 3);
    }

    [Test]
    public async Task XInfoGroups_AfterAllReadAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);
        await this.XInfoGroupsCreateAsync(key);

        await this.XInfoGroupsReadAsync(
            key, "group1", "consumer1", 5);

        var group = await this.XInfoGroupsFindAsync(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Consumers, Is.EqualTo(1));
            Assert.That(group.Pending, Is.EqualTo(5));
            Assert.That(group.LastDeliveredId, Is.EqualTo("5-0"));
        }

        XInfoGroupsAssertCountersAsync(group, 5, 0);
    }

    [Test]
    public async Task XInfoGroups_AfterAckAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);
        await this.XInfoGroupsCreateAsync(key);

        await this.XInfoGroupsReadAsync(
            key, "group1", "consumer1", 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(
                    key, "group1", "1-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "2-0"), Is.EqualTo(1));
        }

        var group = await this.XInfoGroupsFindAsync(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Pending, Is.EqualTo(1));
            Assert.That(group.LastDeliveredId, Is.EqualTo("3-0"));
        }

        XInfoGroupsAssertCountersAsync(group, 3, 2);
    }

    [Test]
    public async Task XInfoGroups_MultipleConsumersAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);
        await this.XInfoGroupsCreateAsync(key);

        await this.XInfoGroupsReadAsync(
            key, "group1", "consumer1", 2);

        await this.XInfoGroupsReadAsync(
            key, "group1", "consumer2", 3);

        var group = await this.XInfoGroupsFindAsync(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Consumers, Is.EqualTo(2));
            Assert.That(group.Pending, Is.EqualTo(5));
            Assert.That(group.LastDeliveredId, Is.EqualTo("5-0"));
        }

        XInfoGroupsAssertCountersAsync(group, 5, 0);
    }

    [Test]
    public async Task XInfoGroups_DifferentGroupProgressAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);

        await this.XInfoGroupsCreateAsync(key, "group1");
        await this.XInfoGroupsCreateAsync(key, "group2");

        await this.XInfoGroupsReadAsync(
            key, "group1", "consumer1", 2);

        await this.XInfoGroupsReadAsync(
            key, "group2", "consumer2", 4);

        var result = await this._redis.Stream.XInfoGroupsAsync(key);

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

        XInfoGroupsAssertCountersAsync(group1, 2, 3);
        XInfoGroupsAssertCountersAsync(group2, 4, 1);
    }

    [Test]
    public async Task XInfoGroups_AfterDelConsumerAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);
        await this.XInfoGroupsCreateAsync(key);

        await this.XInfoGroupsReadAsync(
            key, "group1", "consumer1", 2);

        await this.XInfoGroupsReadAsync(
            key, "group1", "consumer2", 3);

        Assert.That(await this._redis.Stream.XGroupDelConsumerAsync(
            key, "group1", "consumer1"), Is.EqualTo(2));

        var group = await this.XInfoGroupsFindAsync(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Consumers, Is.EqualTo(1));
            Assert.That(group.Pending, Is.EqualTo(3));
            Assert.That(group.LastDeliveredId, Is.EqualTo("5-0"));
        }

        XInfoGroupsAssertCountersAsync(group, 5, 0);
    }

    [Test]
    public async Task XInfoGroups_AfterGroupDestroyAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);

        await this.XInfoGroupsCreateAsync(key, "group1");
        await this.XInfoGroupsCreateAsync(key, "group2");

        Assert.That(await this._redis.Stream.XGroupDestroyAsync(
            key, "group1"), Is.True);

        var result = await this._redis.Stream.XInfoGroupsAsync(key);

        Assert.That(result, Has.Length.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result![0].Name, Is.EqualTo("group2"));

            Assert.That(await this._redis.Stream.XGroupDestroyAsync(
                key, "group2"), Is.True);

            Assert.That(await this._redis.Stream.XInfoGroupsAsync(key),
                Is.Null);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(5));
        }
    }

    [Test]
    public async Task XInfoGroups_SetIDAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);
        await this.XInfoGroupsCreateAsync(key);

        Assert.That(await this._redis.Stream.XGroupSetIDAsync(
            key, "group1", "3-0", entriesRead: 3), Is.True);

        var group = await this.XInfoGroupsFindAsync(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.LastDeliveredId, Is.EqualTo("3-0"));
            Assert.That(group.Pending, Is.Zero);
        }

        XInfoGroupsAssertCountersAsync(group, 3, 2);

        await this.XInfoGroupsReadAsync(
            key, "group1", "consumer1", 2);

        group = await this.XInfoGroupsFindAsync(key, "group1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.LastDeliveredId, Is.EqualTo("5-0"));
            Assert.That(group.Pending, Is.EqualTo(2));
        }

        XInfoGroupsAssertCountersAsync(group, 5, 0);
    }

    [Test]
    public async Task XInfoGroups_SetID_UnknownLagAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);
        await this.XInfoGroupsCreateAsync(key);

        Assert.That(await this._redis.Stream.XGroupSetIDAsync(
            key, "group1", "3-0"), Is.True);

        var group = await this.XInfoGroupsFindAsync(key, "group1");

        Assert.That(group.LastDeliveredId, Is.EqualTo("3-0"));

        if (group.EntriesRead.HasValue || group.Lag.HasValue)
        {
            Assert.That(group.Lag, Is.Null);
        }
    }

    [Test]
    public async Task XInfoGroups_Flags_NoneAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);
        await this.XInfoGroupsCreateAsync(key);

        var result = await this._redis.Stream.XInfoGroupsAsync(
            key, flags: CommandFlags.None);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result![0].Name, Is.EqualTo("group1"));
    }

    [Test]
    public async Task XInfoGroups_CancellationToken_DefaultAsync()
    {
        var key = XInfoGroupsKeyAsync();
        await this.XInfoGroupsPrepareAsync(key);
        await this.XInfoGroupsCreateAsync(key);

        var result = await this._redis.Stream.XInfoGroupsAsync(
            key, cancellationToken: CancellationToken.None);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result![0].Name, Is.EqualTo("group1"));
    }

    [Test]
    public async Task XInfoGroups_StreamNotExistsAsync()
    {
        var key = XInfoGroupsKeyAsync();

        await XInfoGroupsRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XInfoGroupsAsync(key));
    }

    [Test]
    public async Task XInfoGroups_WrongKeyTypeAsync()
    {
        var key = XInfoGroupsKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await XInfoGroupsRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XInfoGroupsAsync(key));
    }

    #endregion

    #region XInfoStream

    private static string XInfoStreamKeyAsync() =>
        $"Stream:{{XInfoStreamAsync}}:{Guid.NewGuid():N}";

    private async Task XInfoStreamSeedAsync(
        string key, int count = 12)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }

        Assert.That(await this._redis.Stream.XLenAsync(key),
            Is.EqualTo(count));
    }

    private async Task XInfoStreamCreateGroupAsync(
        string key, string group = "group1")
    {
        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, group, "0-0"), Is.True);
    }

    private async Task XInfoStreamReadAsync(
        string key, string group, string consumer, int count)
    {
        var result = await this._redis.Stream.XReadGroupAsync(
            key, ">", group, consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries,
                Has.Length.EqualTo(count));
        }
    }

    private static void XInfoStreamAssertIdsAsync(
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

    private static async Task XInfoStreamRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XInfoStream_Summary_BasicAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key);

        var info = await this._redis.Stream.XInfoStreamAsync(key);

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
    public async Task XInfoStream_Summary_WithGroupsAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key);

        await this.XInfoStreamCreateGroupAsync(key, "group1");
        await this.XInfoStreamCreateGroupAsync(key, "group2");

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.IsFull, Is.False);
            Assert.That(info.GroupCount, Is.EqualTo(2));
            Assert.That(info.Length, Is.EqualTo(12));
        }
    }

    [Test]
    public async Task XInfoStream_Summary_AfterDeleteAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelAsync(
                    key, "1-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XDelAsync(
                key, "12-0"), Is.EqualTo(1));
        }

        var info = await this._redis.Stream.XInfoStreamAsync(key);

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
    public async Task XInfoStream_Full_DefaultCountAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.IsFull, Is.True);
            Assert.That(info.Length, Is.EqualTo(12));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.GroupCount, Is.Null);
            Assert.That(info.FirstEntry, Is.Null);
            Assert.That(info.LastEntry, Is.Null);
        }

        XInfoStreamAssertIdsAsync(info.Entries,
            "1-0", "2-0", "3-0", "4-0", "5-0",
            "6-0", "7-0", "8-0", "9-0", "10-0");
    }

    [Test]
    public async Task XInfoStream_Full_CountAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true, count: 3);

        Assert.That(info.Length, Is.EqualTo(12));

        XInfoStreamAssertIdsAsync(info.Entries,
            "1-0", "2-0", "3-0");
    }

    [Test]
    public async Task XInfoStream_Full_CountZeroAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true, count: 0);

        Assert.That(info.Entries, Has.Length.EqualTo(12));

        for (int i = 0; i < 12; i++)
        {
            Assert.That(info.Entries![i].Id,
                Is.EqualTo($"{i + 1}-0"));
        }
    }

    [Test]
    public async Task XInfoStream_Full_WithGroup_NoPendingAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key, 5);
        await this.XInfoStreamCreateGroupAsync(key);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true, count: 0);

        Assert.That(info.FullGroups, Has.Length.EqualTo(1));

        var group = info.FullGroups![0];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Name, Is.EqualTo("group1"));
            Assert.That(group.LastDeliveredId, Is.EqualTo("0-0"));
            Assert.That(group.EntriesRead, Is.Null);
            Assert.That(group.Lag, Is.EqualTo(5));
            Assert.That(group.PelCount, Is.Zero);
            Assert.That(group.Pending?.Length ?? 0, Is.Zero);
            Assert.That(group.Consumers?.Length ?? 0, Is.Zero);
        }
    }

    [Test]
    public async Task XInfoStream_Full_GroupAndConsumerPendingAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key, 5);
        await this.XInfoStreamCreateGroupAsync(key);
        await this.XInfoStreamReadAsync(
            key, "group1", "consumer1", 3);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true, count: 0);

        var group = info.FullGroups![0];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group.Name, Is.EqualTo("group1"));
            Assert.That(group.PelCount, Is.EqualTo(3));
            Assert.That(group.LastDeliveredId, Is.EqualTo("3-0"));
            Assert.That(group.EntriesRead, Is.EqualTo(3));
            Assert.That(group.Lag, Is.EqualTo(2));
            Assert.That(group.Pending, Has.Length.EqualTo(3));
            Assert.That(group.Consumers, Has.Length.EqualTo(1));
        }

        var pending = group.Pending![0];
        var consumer = group.Consumers![0];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(pending.Id, Is.EqualTo("1-0"));
            Assert.That(pending.Consumer, Is.EqualTo("consumer1"));
            Assert.That(pending.DeliveryCount, Is.EqualTo(1));
            Assert.That(pending.DeliveryTime, Is.GreaterThan(0));

            Assert.That(consumer.Name, Is.EqualTo("consumer1"));
            Assert.That(consumer.PelCount, Is.EqualTo(3));
            Assert.That(consumer.Pending, Has.Length.EqualTo(3));
            Assert.That(consumer.SeenTime, Is.GreaterThan(0));

            Assert.That(consumer.Pending![0].Id, Is.EqualTo("1-0"));
            Assert.That(consumer.Pending[0].DeliveryCount, Is.EqualTo(1));
            Assert.That(consumer.Pending[0].DeliveryTime,
                Is.GreaterThan(0));
        }

        if (consumer.ActiveTime.HasValue)
        {
            Assert.That(consumer.ActiveTime.Value,
                Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task XInfoStream_Full_MultipleGroupsAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key, 5);

        await this.XInfoStreamCreateGroupAsync(key, "group1");
        await this.XInfoStreamCreateGroupAsync(key, "group2");

        await this.XInfoStreamReadAsync(
            key, "group1", "consumer1", 2);

        await this.XInfoStreamReadAsync(
            key, "group2", "consumer2", 4);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true, count: 0);

        var group1 = info.FullGroups!.Single(
            x => x.Name == "group1");

        var group2 = info.FullGroups!.Single(
            x => x.Name == "group2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(group1.PelCount, Is.EqualTo(2));
            Assert.That(group1.EntriesRead, Is.EqualTo(2));
            Assert.That(group1.Lag, Is.EqualTo(3));

            Assert.That(group2.PelCount, Is.EqualTo(4));
            Assert.That(group2.EntriesRead, Is.EqualTo(4));
            Assert.That(group2.Lag, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XInfoStream_Full_PendingCountLimitAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key);
        await this.XInfoStreamCreateGroupAsync(key);

        await this.XInfoStreamReadAsync(
            key, "group1", "consumer1", 12);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true, count: 2);

        var group = info.FullGroups![0];
        var consumer = group.Consumers!.Single(
            x => x.Name == "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Entries, Has.Length.EqualTo(2));
            Assert.That(group.PelCount, Is.EqualTo(12));
            Assert.That(group.Pending, Has.Length.EqualTo(2));
            Assert.That(consumer.PelCount, Is.EqualTo(12));
            Assert.That(consumer.Pending, Has.Length.EqualTo(2));
        }
    }

    [Test]
    public async Task XInfoStream_Full_AfterAckAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key, 5);
        await this.XInfoStreamCreateGroupAsync(key);

        await this.XInfoStreamReadAsync(
            key, "group1", "consumer1", 3);

        Assert.That(await this._redis.Stream.XAckAsync(
            key, "group1", "1-0"), Is.EqualTo(1));

        var info = await this._redis.Stream.XInfoStreamAsync(
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
    public async Task XInfoStream_Full_AfterClaimAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key, 5);
        await this.XInfoStreamCreateGroupAsync(key);

        await this.XInfoStreamReadAsync(
            key, "group1", "consumer1", 2);

        var claimed = await this._redis.Stream.XClaimAsync(
            key, "group1", "consumer2", 0, "1-0");

        Assert.That(claimed.HasValue, Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true, count: 0);

        var group = info.FullGroups![0];
        var pending = group.Pending!.Single(x => x.Id == "1-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(pending.Consumer, Is.EqualTo("consumer2"));
            Assert.That(pending.DeliveryCount, Is.EqualTo(2));
            Assert.That(group.PelCount, Is.EqualTo(2));
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
    public async Task XInfoStream_Full_SeenTime_ActiveTimeAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key, 5);
        await this.XInfoStreamCreateGroupAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateConsumerAsync(
            key, "group1", "consumer1"), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true);

        var consumer = info.FullGroups![0].Consumers!.Single(
            x => x.Name == "consumer1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(consumer.PelCount, Is.Zero);
            Assert.That(consumer.SeenTime,
                Is.GreaterThanOrEqualTo(0));
        }

        if (consumer.ActiveTime.HasValue)
        {
            Assert.That(consumer.ActiveTime.Value,
                Is.GreaterThanOrEqualTo(-1));
        }

        await this.XInfoStreamReadAsync(
            key, "group1", "consumer1", 1);

        info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true);

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
    public async Task XInfoStream_Summary_IdmpConfigurationAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key, 5);

        Assert.That(await this._redis.Stream.XCfgSetAsync(
            key, duration: 300, maxsize: 40), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(key);

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
    public async Task XInfoStream_Full_IdmpConfigurationAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key, 5);

        Assert.That(await this._redis.Stream.XCfgSetAsync(
            key, duration: 600, maxsize: 100), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.IsFull, Is.True);
            Assert.That(info.IdmpDuration, Is.EqualTo(600));
            Assert.That(info.IdmpMaxSize, Is.EqualTo(100));
        }
    }

    [Test]
    public async Task XInfoStream_Full_NackedCountAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key, 5);
        await this.XInfoStreamCreateGroupAsync(key);

        await this.XInfoStreamReadAsync(
            key, "group1", "consumer1", 2);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, full: true);

        var group = info.FullGroups![0];

        if (group.NackedCount.HasValue)
        {
            Assert.That(group.NackedCount.Value,
                Is.Zero);
        }

        Assert.That(group.PelCount, Is.EqualTo(2));
    }

    [Test]
    public async Task XInfoStream_Flags_NoneAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key, 5);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, flags: CommandFlags.None);

        Assert.That(info.Length, Is.EqualTo(5));
    }

    [Test]
    public async Task XInfoStream_CancellationToken_DefaultAsync()
    {
        var key = XInfoStreamKeyAsync();
        await this.XInfoStreamSeedAsync(key, 5);

        var info = await this._redis.Stream.XInfoStreamAsync(
            key, cancellationToken: CancellationToken.None);

        Assert.That(info.Length, Is.EqualTo(5));
    }

    [Test]
    public async Task XInfoStream_StreamNotExistsAsync()
    {
        var key = XInfoStreamKeyAsync();

        await XInfoStreamRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XInfoStreamAsync(key));
    }

    [Test]
    public async Task XInfoStream_WrongKeyTypeAsync()
    {
        var key = XInfoStreamKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await XInfoStreamRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XInfoStreamAsync(key));
    }

    #endregion

    #region XNAckAsync

    [Test]
    public async Task XNAck_Single_FailAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = await redis.Stream.XAddAsync(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        await redis.Stream.XGroupCreateAsync(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await redis.Stream.XNAckAsync(
                    key, group, id, NackMode.Fail,
                    force: true), Is.EqualTo(1));

            Assert.That(await redis.Stream.XNAckAsync(
                key, group, id, NackMode.Fail), Is.EqualTo(1));

            Assert.That(await redis.Stream.XAckAsync(
                key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XNAck_Single_SilentAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = await redis.Stream.XAddAsync(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        await redis.Stream.XGroupCreateAsync(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await redis.Stream.XNAckAsync(
                    key, group, id, NackMode.Silent,
                    force: true), Is.EqualTo(1));

            Assert.That(await redis.Stream.XNAckAsync(
                key, group, id, NackMode.Silent), Is.EqualTo(1));

            Assert.That(await redis.Stream.XAckAsync(
                key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XNAck_Single_FatalAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = await redis.Stream.XAddAsync(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        await redis.Stream.XGroupCreateAsync(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await redis.Stream.XNAckAsync(
                    key, group, id, NackMode.Fatal,
                    force: true), Is.EqualTo(1));

            Assert.That(await redis.Stream.XAckAsync(
                key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XNAck_Collection_MultipleAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id1 = await redis.Stream.XAddAsync(key, "field", "value1");
        var id2 = await redis.Stream.XAddAsync(key, "field", "value2");
        var id3 = await redis.Stream.XAddAsync(key, "field", "value3");

        Assert.That(id1 != null && id2 != null && id3 != null, Is.True);

        await redis.Stream.XGroupCreateAsync(key, group, "0-0");

        ICollection<EntryId> ids = [id1!, id2!, id3!];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await redis.Stream.XNAckAsync(
                    key, group, ids, NackMode.Fail,
                    force: true), Is.EqualTo(3));

            Assert.That(await redis.Stream.XNAckAsync(
                key, group, ids, NackMode.Silent), Is.EqualTo(3));

            Assert.That(await redis.Stream.XAckAsync(
                key, group, ids), Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XNAck_Force_False_NotPendingAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = await redis.Stream.XAddAsync(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        await redis.Stream.XGroupCreateAsync(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await redis.Stream.XNAckAsync(
                    key, group, id, NackMode.Fail), Is.Zero);

            Assert.That(await redis.Stream.XAckAsync(
                key, group, id), Is.Zero);
        }
    }

    [Test]
    public async Task XNAck_Force_True_CreatesPendingAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = await redis.Stream.XAddAsync(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        await redis.Stream.XGroupCreateAsync(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await redis.Stream.XNAckAsync(
                    key, group, id, NackMode.Fail,
                    force: true), Is.EqualTo(1));

            Assert.That(await redis.Stream.XAckAsync(
                key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XNAck_Force_True_NonexistentEntryAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = await redis.Stream.XAddAsync(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        await redis.Stream.XGroupCreateAsync(key, group, "0-0");

        Assert.That(await redis.Stream.XNAckAsync(
            key, group, "999999999999999-0",
            NackMode.Fail, force: true), Is.Zero);
    }

    [Test]
    public async Task XNAck_RetryCount_ZeroAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = await redis.Stream.XAddAsync(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        await redis.Stream.XGroupCreateAsync(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await redis.Stream.XNAckAsync(
                    key, group, id, NackMode.Fatal,
                    retryCount: 0, force: true), Is.EqualTo(1));

            Assert.That(await redis.Stream.XAckAsync(
                key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XNAck_RetryCount_PositiveAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = await redis.Stream.XAddAsync(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        await redis.Stream.XGroupCreateAsync(key, group, "0-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await redis.Stream.XNAckAsync(
                    key, group, id, NackMode.Silent,
                    retryCount: 10, force: true), Is.EqualTo(1));

            Assert.That(await redis.Stream.XAckAsync(
                key, group, id), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task XNAck_RetryCount_Negative_ThrowsAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id = await redis.Stream.XAddAsync(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        await redis.Stream.XGroupCreateAsync(key, group, "0-0");

        await XNAckRedisErrorAsync<RedisErrorException>(async () =>
            await redis.Stream.XNAckAsync(
                key, group, id, NackMode.Fail,
                retryCount: -1, force: true));
    }

    [Test]
    public async Task XNAck_GroupNotExists_ThrowsAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";

        var id = await redis.Stream.XAddAsync(key, "field", "value");
        Assert.That(id, Is.Not.Null);

        await XNAckRedisErrorAsync<RedisErrorException>(async () =>
            await redis.Stream.XNAckAsync(
                key, "nonexistent-group",
                id, NackMode.Fail));
    }

    [Test]
    public async Task XNAck_Collection_MixedPendingAndNonPendingAsync()
    {
        var redis = this._redis;
        string key = $"Stream:XNAck:{Guid.NewGuid():N}";
        const string group = "group1";

        var id1 = await redis.Stream.XAddAsync(key, "field", "value1");
        var id2 = await redis.Stream.XAddAsync(key, "field", "value2");

        Assert.That(id1 != null && id2 != null, Is.True);

        await redis.Stream.XGroupCreateAsync(key, group, "0-0");

        Assert.That(await redis.Stream.XNAckAsync(
            key, group, id1!, NackMode.Fail,
            force: true), Is.EqualTo(1));

        ICollection<EntryId> ids = [id1!, id2!];

        Assert.That(await redis.Stream.XNAckAsync(
            key, group, ids, NackMode.Fail), Is.EqualTo(1));
    }

    private static async Task XNAckRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(
            async () => await action());
    }

    #endregion

    #region XRevRange

    private static string XRevRangeKeyAsync() =>
        $"Stream:{{XRevRangeAsync}}:{Guid.NewGuid():N}";

    private async Task XRevRangePrepareAsync(string key)
    {
        for (int i = 1; i <= 5; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }
    }

    private static void XRevRangeAssertIdsAsync(
        EntryItem[]? entries, params string[] expected)
    {
        Assert.That(entries, Has.Length.EqualTo(expected.Length));

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.That(entries![i].Id, Is.EqualTo(expected[i]),
                $"Entry index {i}");
        }
    }

    private static async Task XRevRangeRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XRevRange_AllAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "+", "-");

        XRevRangeAssertIdsAsync(result,
            "5-0", "4-0", "3-0", "2-0", "1-0");
    }

    [Test]
    public async Task XRevRange_SpecifiedRangeAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "4-0", "2-0");

        XRevRangeAssertIdsAsync(result, "4-0", "3-0", "2-0");
    }

    [Test]
    public async Task XRevRange_BoundariesInclusiveAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "5-0", "1-0");

        XRevRangeAssertIdsAsync(result,
            "5-0", "4-0", "3-0", "2-0", "1-0");
    }

    [Test]
    public async Task XRevRange_SingleEntryAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "3-0", "3-0");

        XRevRangeAssertIdsAsync(result, "3-0");
    }

    [Test]
    public async Task XRevRange_EntryContentAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "3-0", "2-0");

        XRevRangeAssertIdsAsync(result, "3-0", "2-0");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result![0].FieldValue, Is.Not.Null);
            Assert.That(result[1].FieldValue, Is.Not.Null);
        }
    }

    [Test]
    public async Task XRevRange_CountOneAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "+", "-", count: 1);

        XRevRangeAssertIdsAsync(result, "5-0");
    }

    [Test]
    public async Task XRevRange_CountThreeAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "+", "-", count: 3);

        XRevRangeAssertIdsAsync(result, "5-0", "4-0", "3-0");
    }

    [Test]
    public async Task XRevRange_CountGreaterThanEntriesAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "+", "-", count: 100);

        XRevRangeAssertIdsAsync(result,
            "5-0", "4-0", "3-0", "2-0", "1-0");
    }

    [Test]
    public async Task XRevRange_CountWithinRangeAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "4-0", "2-0", count: 2);

        XRevRangeAssertIdsAsync(result, "4-0", "3-0");
    }

    [Test]
    public async Task XRevRange_EndLessThanStartAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "2-0", "4-0");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task XRevRange_NoMatchingEntriesAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "100-0", "99-0");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task XRevRange_MissingKeyAsync()
    {
        var key = XRevRangeKeyAsync();

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "+", "-");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task XRevRange_AfterDeleteAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(
            key, "3-0"), Is.EqualTo(1));

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "+", "-");

        XRevRangeAssertIdsAsync(result,
            "5-0", "4-0", "2-0", "1-0");
    }

    [Test]
    public async Task XRevRange_SameTimestampDifferentSequenceAsync()
    {
        var key = XRevRangeKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                    key, "100-0", "field", "a"), Is.EqualTo("100-0"));

            Assert.That(await this._redis.Stream.XAddAsync(
                key, "100-1", "field", "b"), Is.EqualTo("100-1"));

            Assert.That(await this._redis.Stream.XAddAsync(
                key, "100-2", "field", "c"), Is.EqualTo("100-2"));

            Assert.That(await this._redis.Stream.XAddAsync(
                key, "101-0", "field", "d"), Is.EqualTo("101-0"));
        }

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "+", "-");

        XRevRangeAssertIdsAsync(result,
            "101-0", "100-2", "100-1", "100-0");
    }

    [Test]
    public async Task XRevRange_ExclusiveRangeAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        // Redis 6.2+ 支持排除边界。
        var result = await this._redis.Stream.XRevRangeAsync(
            key, "(4-0", "(2-0");

        XRevRangeAssertIdsAsync(result, "3-0");
    }

    [Test]
    public async Task XRevRange_Flags_NoneAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "+", "-", flags: CommandFlags.None);

        XRevRangeAssertIdsAsync(result,
            "5-0", "4-0", "3-0", "2-0", "1-0");
    }

    [Test]
    public async Task XRevRange_CancellationToken_DefaultAsync()
    {
        var key = XRevRangeKeyAsync();
        await this.XRevRangePrepareAsync(key);

        var result = await this._redis.Stream.XRevRangeAsync(
            key, "+", "-",
            cancellationToken: CancellationToken.None);

        XRevRangeAssertIdsAsync(result,
            "5-0", "4-0", "3-0", "2-0", "1-0");
    }

    [Test]
    public async Task XRevRange_WrongKeyTypeAsync()
    {
        var key = XRevRangeKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await XRevRangeRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XRevRangeAsync(
                key, "+", "-"));
    }

    #endregion

    #region XSetID

    private static string XSetIDKeyAsync() =>
        $"Stream:{{XSetIDAsync}}:{Guid.NewGuid():N}";

    private async Task XSetIDPrepareAsync(string key)
    {
        for (int i = 1; i <= 3; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }
    }

    private static async Task XSetIDRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XSetID_OnlyLastIdAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XSetIDAsync(
            key, "100-0"), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
            Assert.That(info.Length, Is.EqualTo(3));
            Assert.That(info.EntriesAdded, Is.EqualTo(3));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("0-0"));
        }
    }

    [Test]
    public async Task XSetID_LastIdEqualsCurrentAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XSetIDAsync(
            key, "3-0"), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("3-0"));
            Assert.That(info.Length, Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XSetID_EntriesAddedAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XSetIDAsync(
            key, "100-0", entriesAdded: 50), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(50));
            Assert.That(info.Length, Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XSetID_EntriesAddedEqualsLengthAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XSetIDAsync(
            key, "100-0", entriesAdded: 3), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.EntriesAdded, Is.EqualTo(3));
            Assert.That(info.Length, Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XSetID_MaxDeletedIdAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XSetIDAsync(
            key, "100-0", maxDeletedId: "50-0"), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("50-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(3));
            Assert.That(info.Length, Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XSetID_MaxDeletedIdEqualsLastIdAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XSetIDAsync(
            key, "100-0", maxDeletedId: "100-0"), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("100-0"));
        }
    }

    [Test]
    public async Task XSetID_AllParametersAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XSetIDAsync(
            key,
            "100-0",
            entriesAdded: 50,
            maxDeletedId: "90-0"), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(key);

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
    public async Task XSetID_AfterXAddAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XSetIDAsync(
                    key, "100-0", entriesAdded: 50), Is.True);

            Assert.That(await this._redis.Stream.XAddAsync(
                key, "101-0", "field", "value101"),
                Is.EqualTo("101-0"));
        }

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("101-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(51));
            Assert.That(info.Length, Is.EqualTo(4));
        }
    }

    [Test]
    public async Task XSetID_AfterXDelAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XDelAsync(
                    key, "3-0"), Is.EqualTo(1));

            Assert.That(await this._redis.Stream.XSetIDAsync(
                key, "100-0"), Is.True);
        }

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("3-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(3));
            Assert.That(info.Length, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task XSetID_LastIdLessThanTopItemAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        await XSetIDRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XSetIDAsync(
                key, "2-0"));

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        Assert.That(info.LastGeneratedId, Is.EqualTo("3-0"));
    }

    [Test]
    public async Task XSetID_LastIdLessThanMaxDeletedIdAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(
            key, "3-0"), Is.EqualTo(1));

        await XSetIDRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XSetIDAsync(
                key, "2-0"));

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        Assert.That(info.MaxDeletedEntryId, Is.EqualTo("3-0"));
    }

    [Test]
    public async Task XSetID_EntriesAddedLessThanLengthAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        await XSetIDRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XSetIDAsync(
                key, "100-0", entriesAdded: 2));

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("3-0"));
            Assert.That(info.EntriesAdded, Is.EqualTo(3));
            Assert.That(info.Length, Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XSetID_MaxDeletedIdGreaterThanLastIdAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        await XSetIDRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XSetIDAsync(
                key, "100-0", maxDeletedId: "101-0"));

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.LastGeneratedId, Is.EqualTo("3-0"));
            Assert.That(info.MaxDeletedEntryId, Is.EqualTo("0-0"));
        }
    }

    [Test]
    public async Task XSetID_Flags_NoneAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XSetIDAsync(
            key, "100-0", flags: CommandFlags.None), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
    }

    [Test]
    public async Task XSetID_CancellationToken_DefaultAsync()
    {
        var key = XSetIDKeyAsync();
        await this.XSetIDPrepareAsync(key);

        Assert.That(await this._redis.Stream.XSetIDAsync(
            key, "100-0",
            cancellationToken: CancellationToken.None), Is.True);

        var info = await this._redis.Stream.XInfoStreamAsync(key);

        Assert.That(info.LastGeneratedId, Is.EqualTo("100-0"));
    }

    [Test]
    public async Task XSetID_StreamNotExistsAsync()
    {
        var key = XSetIDKeyAsync();

        await XSetIDRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XSetIDAsync(
                key, "100-0"));
    }

    [Test]
    public async Task XSetID_WrongKeyTypeAsync()
    {
        var key = XSetIDKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await XSetIDRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XSetIDAsync(
                key, "100-0"));
    }

    #endregion

    #region XTrim

    private static string XTrimKeyAsync() =>
        $"Stream:{{XTrimAsync}}:{Guid.NewGuid():N}";

    private async Task XTrimPrepareAsync(string key, int count = 6)
    {
        for (int i = 1; i <= count; i++)
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                key, $"{i}-0", "field", $"value{i}"),
                Is.EqualTo($"{i}-0"));
        }
    }

    private async Task XTrimAssertEntriesAsync(
        string key, params string[] expectedIds)
    {
        var result = await this._redis.Stream.XRevRangeAsync(
            key, "+", "-");

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

    private async Task XTrimReadAsync(
        string key, string group, string consumer, int count)
    {
        var result = await this._redis.Stream.XReadGroupAsync(
            key, ">", group, consumer, count: count);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result!.Value.Entries,
                Has.Length.EqualTo(count));
        }
    }

    private static async Task XTrimRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XTrim_MaxLen_ExactAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.EqualTo(3));
            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(3));
        }

        await this.XTrimAssertEntriesAsync(
            key, "6-0", "5-0", "4-0");
    }

    [Test]
    public async Task XTrim_MaxLen_EqualLengthAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(6, TrimOperator.Exact),
            DeletionPolicy.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.Zero);
            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(6));
        }
    }

    [Test]
    public async Task XTrim_MaxLen_GreaterThanLengthAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(100, TrimOperator.Exact),
            DeletionPolicy.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.Zero);
            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(6));
        }
    }

    [Test]
    public async Task XTrim_MaxLen_ZeroAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(0, TrimOperator.Exact),
            DeletionPolicy.None);

        Assert.That(deleted, Is.EqualTo(6));
        await this.XTrimAssertEntriesAsync(key);
    }

    [Test]
    public async Task XTrim_MinId_ExactAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMinId("4-0", TrimOperator.Exact),
            DeletionPolicy.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.EqualTo(3));
            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(3));
        }

        await this.XTrimAssertEntriesAsync(
            key, "6-0", "5-0", "4-0");
    }

    [Test]
    public async Task XTrim_MinId_ZeroAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMinId("0-0", TrimOperator.Exact),
            DeletionPolicy.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.Zero);
            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(6));
        }
    }

    [Test]
    public async Task XTrim_MinId_GreaterThanAllEntriesAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMinId("100-0", TrimOperator.Exact),
            DeletionPolicy.None);

        Assert.That(deleted, Is.EqualTo(6));
        await this.XTrimAssertEntriesAsync(key);
    }

    [Test]
    public async Task XTrim_MinId_SameTimestampAsync()
    {
        var key = XTrimKeyAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                    key, "100-0", "field", "a"), Is.EqualTo("100-0"));
            Assert.That(await this._redis.Stream.XAddAsync(
                key, "100-1", "field", "b"), Is.EqualTo("100-1"));
            Assert.That(await this._redis.Stream.XAddAsync(
                key, "100-2", "field", "c"), Is.EqualTo("100-2"));
            Assert.That(await this._redis.Stream.XAddAsync(
                key, "101-0", "field", "d"), Is.EqualTo("101-0"));
        }

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMinId("100-2", TrimOperator.Exact),
            DeletionPolicy.None);

        Assert.That(deleted, Is.EqualTo(2));

        await this.XTrimAssertEntriesAsync(
            key, "101-0", "100-2");
    }

    [Test]
    public async Task XTrim_MaxLen_ApproximateAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key, 100);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(50, TrimOperator.Approximate),
            DeletionPolicy.None);

        var length = await this._redis.Stream.XLenAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.GreaterThanOrEqualTo(0));
            Assert.That(deleted, Is.LessThanOrEqualTo(50));
            Assert.That(length, Is.EqualTo(100 - deleted));
            Assert.That(length, Is.GreaterThanOrEqualTo(50));
        }
    }

    [Test]
    public async Task XTrim_MinId_ApproximateAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key, 100);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMinId("51-0", TrimOperator.Approximate),
            DeletionPolicy.None);

        var length = await this._redis.Stream.XLenAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.GreaterThanOrEqualTo(0));
            Assert.That(deleted, Is.LessThanOrEqualTo(50));
            Assert.That(length, Is.EqualTo(100 - deleted));
        }
    }

    [Test]
    public async Task XTrim_Approximate_LimitAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key, 100);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(
                20, TrimOperator.Approximate, limit: 10),
            DeletionPolicy.None);

        var length = await this._redis.Stream.XLenAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.GreaterThanOrEqualTo(0));
            Assert.That(deleted, Is.LessThanOrEqualTo(10));
            Assert.That(length, Is.EqualTo(100 - deleted));
        }
    }

    [Test]
    public async Task XTrim_KeepRef_PendingPreservedAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, "group1", "0-0"), Is.True);

        await this.XTrimReadAsync(
            key, "group1", "consumer1", 3);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.KeepRef);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.EqualTo(3));
            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(3));
        }

        var pending = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(pending.Count, Is.EqualTo(3));

        await this.XTrimAssertEntriesAsync(
            key, "6-0", "5-0", "4-0");
    }

    [Test]
    public async Task XTrim_DelRef_PendingRemovedAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, "group1", "0-0"), Is.True);

        await this.XTrimReadAsync(
            key, "group1", "consumer1", 3);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.DelRef);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.EqualTo(3));
            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(3));
        }

        var pending = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(pending.Count, Is.Zero);

        await this.XTrimAssertEntriesAsync(
            key, "6-0", "5-0", "4-0");
    }

    [Test]
    public async Task XTrim_DelRef_MultipleGroupsAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateAsync(
                    key, "group1", "0-0"), Is.True);
            Assert.That(await this._redis.Stream.XGroupCreateAsync(
                key, "group2", "0-0"), Is.True);
        }

        await this.XTrimReadAsync(
            key, "group1", "consumer1", 3);
        await this.XTrimReadAsync(
            key, "group2", "consumer2", 3);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.DelRef);

        Assert.That(deleted, Is.EqualTo(3));

        var pending1 = await this._redis.Stream.XPendingAsync(
            key, "group1");
        var pending2 = await this._redis.Stream.XPendingAsync(
            key, "group2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(pending1.Count, Is.Zero);
            Assert.That(pending2.Count, Is.Zero);
        }
    }

    [Test]
    public async Task XTrim_Acked_OnlyAcknowledgedEntriesAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        Assert.That(await this._redis.Stream.XGroupCreateAsync(
            key, "group1", "0-0"), Is.True);

        await this.XTrimReadAsync(
            key, "group1", "consumer1", 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(
                    key, "group1", "1-0"), Is.EqualTo(1));
            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "2-0"), Is.EqualTo(1));
        }

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(2, TrimOperator.Exact),
            DeletionPolicy.Acked);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(deleted, Is.EqualTo(2));
            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(4));
        }

        await this.XTrimAssertEntriesAsync(
            key, "6-0", "5-0", "4-0", "3-0");

        var pending = await this._redis.Stream.XPendingAsync(
            key, "group1");

        Assert.That(pending.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task XTrim_Acked_MultipleGroupsAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XGroupCreateAsync(
                    key, "group1", "0-0"), Is.True);
            Assert.That(await this._redis.Stream.XGroupCreateAsync(
                key, "group2", "0-0"), Is.True);
        }

        await this.XTrimReadAsync(
            key, "group1", "consumer1", 3);
        await this.XTrimReadAsync(
            key, "group2", "consumer2", 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAckAsync(
                    key, "group1", "1-0"), Is.EqualTo(1));
            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group2", "1-0"), Is.EqualTo(1));
            Assert.That(await this._redis.Stream.XAckAsync(
                key, "group1", "2-0"), Is.EqualTo(1));
        }

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(2, TrimOperator.Exact),
            DeletionPolicy.Acked);

        Assert.That(deleted, Is.EqualTo(1));

        await this.XTrimAssertEntriesAsync(
            key, "6-0", "5-0", "4-0", "3-0", "2-0");

        var pending1 = await this._redis.Stream.XPendingAsync(
            key, "group1");
        var pending2 = await this._redis.Stream.XPendingAsync(
            key, "group2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(pending1.Count, Is.EqualTo(1));
            Assert.That(pending2.Count, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task XTrim_MissingKeyAsync()
    {
        var key = XTrimKeyAsync();

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.None);

        Assert.That(deleted, Is.Zero);
    }

    [Test]
    public async Task XTrim_Flags_NoneAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.None,
            flags: CommandFlags.None);

        Assert.That(deleted, Is.EqualTo(3));
    }

    [Test]
    public async Task XTrim_CancellationToken_DefaultAsync()
    {
        var key = XTrimKeyAsync();
        await this.XTrimPrepareAsync(key);

        var deleted = await this._redis.Stream.XTrimAsync(
            key,
            TrimOptions.ByMaxLen(3, TrimOperator.Exact),
            DeletionPolicy.None,
            cancellationToken: CancellationToken.None);

        Assert.That(deleted, Is.EqualTo(3));
    }

    [Test]
    public async Task XTrim_WrongKeyTypeAsync()
    {
        var key = XTrimKeyAsync();
        await this._redis.String.SetAsync(key, "hello");

        await XTrimRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XTrimAsync(
                key,
                TrimOptions.ByMaxLen(3, TrimOperator.Exact),
                DeletionPolicy.None));
    }

    #endregion

    #region XIdmpRecord

    private static string XIdmpRecordKeyAsync() =>
        $"Stream:{{XIdmpRecordAsync}}:{Guid.NewGuid():N}";

    private async Task XIdmpRecordPrepareAsync(string key)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XAddAsync(
                    key, "1-0", "field", "value1"), Is.EqualTo("1-0"));

            Assert.That(await this._redis.Stream.XAddAsync(
                key, "2-0", "field", "value2"), Is.EqualTo("2-0"));

            Assert.That(await this._redis.Stream.XAddAsync(
                key, "3-0", "field", "value3"), Is.EqualTo("3-0"));
        }
    }

    private static async Task XIdmpRecordRedisErrorAsync<TError>(
        Func<Task> action) where TError : Exception
    {
        await Assert.ThrowsAsync<TError>(async () => await action());
    }

    [Test]
    public async Task XIdmpRecord_BasicAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XIdmpRecordAsync(
                    key, "producer1", "iid1", "1-0"), Is.True);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XIdmpRecord_SameMapping_IdempotentAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        Assert.That(await this._redis.Stream.XIdmpRecordAsync(
            key, "producer1", "iid1", "1-0"), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XIdmpRecordAsync(
                    key, "producer1", "iid1", "1-0"), Is.True);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XIdmpRecord_MultipleProducersAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XIdmpRecordAsync(
                    key, "producer1", "iid1", "1-0"), Is.True);

            Assert.That(await this._redis.Stream.XIdmpRecordAsync(
                key, "producer2", "iid1", "2-0"), Is.True);

            Assert.That(await this._redis.Stream.XIdmpRecordAsync(
                key, "producer3", "iid1", "3-0"), Is.True);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XIdmpRecord_SameProducer_MultipleIidsAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Stream.XIdmpRecordAsync(
                    key, "producer1", "iid1", "1-0"), Is.True);

            Assert.That(await this._redis.Stream.XIdmpRecordAsync(
                key, "producer1", "iid2", "2-0"), Is.True);

            Assert.That(await this._redis.Stream.XIdmpRecordAsync(
                key, "producer1", "iid3", "3-0"), Is.True);

            Assert.That(await this._redis.Stream.XLenAsync(key),
                Is.EqualTo(3));
        }
    }

    [Test]
    public async Task XIdmpRecord_SameMappingDifferentStreamIdAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        Assert.That(await this._redis.Stream.XIdmpRecordAsync(
            key, "producer1", "iid1", "1-0"), Is.True);

        await XIdmpRecordRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XIdmpRecordAsync(
                key, "producer1", "iid1", "2-0"));

        Assert.That(await this._redis.Stream.XIdmpRecordAsync(
            key, "producer1", "iid1", "1-0"), Is.True);
    }

    [Test]
    public async Task XIdmpRecord_StreamIdNotExistsAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        await XIdmpRecordRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XIdmpRecordAsync(
                key, "producer1", "iid1", "100-0"));

        Assert.That(await this._redis.Stream.XLenAsync(key),
            Is.EqualTo(3));
    }

    [Test]
    public async Task XIdmpRecord_DeletedStreamIdAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        Assert.That(await this._redis.Stream.XDelAsync(
            key, "2-0"), Is.EqualTo(1));

        await XIdmpRecordRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XIdmpRecordAsync(
                key, "producer1", "iid1", "2-0"));

        Assert.That(await this._redis.Stream.XLenAsync(key),
            Is.EqualTo(2));
    }

    [Test]
    public async Task XIdmpRecord_EmptyProducerIdAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        await XIdmpRecordRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XIdmpRecordAsync(
                key, "", "iid1", "1-0"));
    }

    [Test]
    public async Task XIdmpRecord_EmptyIdempotencyIdAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        await XIdmpRecordRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XIdmpRecordAsync(
                key, "producer1", "", "1-0"));
    }

    [Test]
    public async Task XIdmpRecord_BothIdsEmptyAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        await XIdmpRecordRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XIdmpRecordAsync(
                key, "", "", "1-0"));
    }

    [Test]
    public async Task XIdmpRecord_MissingKeyAsync()
    {
        var key = XIdmpRecordKeyAsync();

        await XIdmpRecordRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XIdmpRecordAsync(
                key, "producer1", "iid1", "1-0"));
    }

    [Test]
    public async Task XIdmpRecord_WrongKeyTypeAsync()
    {
        var key = XIdmpRecordKeyAsync();

        await this._redis.String.SetAsync(key, "hello");

        await XIdmpRecordRedisErrorAsync<RedisErrorException>(
            async () => await this._redis.Stream.XIdmpRecordAsync(
                key, "producer1", "iid1", "1-0"));
    }

    [Test]
    public async Task XIdmpRecord_Flags_NoneAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        Assert.That(await this._redis.Stream.XIdmpRecordAsync(
            key, "producer1", "iid1", "1-0",
            flags: CommandFlags.None), Is.True);
    }

    [Test]
    public async Task XIdmpRecord_CancellationToken_DefaultAsync()
    {
        var key = XIdmpRecordKeyAsync();
        await this.XIdmpRecordPrepareAsync(key);

        Assert.That(await this._redis.Stream.XIdmpRecordAsync(
            key, "producer1", "iid1", "1-0",
            cancellationToken: CancellationToken.None), Is.True);
    }

    #endregion
}
