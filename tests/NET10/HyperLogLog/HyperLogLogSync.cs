using SharpRedis;

namespace NET10.HyperLogLog;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class HyperLogLogSync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public void PFAdd()
    {
        const string key = "HyperLogLog:{PFAdd}:Key";

        var ok = this._redis.HyperLogLog.PFAdd(key, "a");
        Assert.IsTrue(ok);

        ok = this._redis.HyperLogLog.PFAdd(key, "a");
        Assert.IsFalse(ok);

        ok = this._redis.HyperLogLog.PFAdd(key, ["a", "b", "hello"]);
        Assert.IsTrue(ok);

        ok = this._redis.HyperLogLog.PFAdd(key, ["a", "b", "hello"]);
        Assert.IsFalse(ok);
    }

    [Test]
    public void PFCount()
    {
        const string key = "HyperLogLog:{PFCount}:Key";

        int count = this._redis.HyperLogLog.PFCount(key);
        Assert.Zero(count);

        var ok = this._redis.HyperLogLog.PFAdd(key, "a");
        Assert.IsTrue(ok);

        count = this._redis.HyperLogLog.PFCount(key);
        Assert.That(count, Is.EqualTo(1));

        ok = this._redis.HyperLogLog.PFAdd(key, "a");
        Assert.IsFalse(ok);

        count = this._redis.HyperLogLog.PFCount(key);
        Assert.That(count, Is.EqualTo(1));

        ok = this._redis.HyperLogLog.PFAdd(key, ["a", "b", "hello"]);
        Assert.IsTrue(ok);

        count = this._redis.HyperLogLog.PFCount(key);
        Assert.That(count, Is.EqualTo(3));

        ok = this._redis.HyperLogLog.PFAdd(key, ["a", "b", "hello"]);
        Assert.IsFalse(ok);

        count = this._redis.HyperLogLog.PFCount(key);
        Assert.That(count, Is.EqualTo(3));
    }

    [Test]
    public void PFMerge()
    {
        const string key1 = "HyperLogLog:{PFMerge}:Key1";
        const string key2 = "HyperLogLog:{PFMerge}:Key2";
        const string destkey = "HyperLogLog:{PFMerge}:Destkey";

        var ok = this._redis.HyperLogLog.PFAdd(key1, ["foo", "bar", "zap", "a"]);
        Assert.IsTrue(ok);

        ok = this._redis.HyperLogLog.PFMerge(destkey, key1);
        Assert.IsTrue(ok);

        ok = this._redis.HyperLogLog.PFAdd(key2, ["a", "b", "c", "foo"]);
        Assert.IsTrue(ok);

        int count = this._redis.HyperLogLog.PFCount([key1, key2]);
        Assert.That(count, Is.EqualTo(6));

        ok = this._redis.HyperLogLog.PFMerge(destkey, [key1, key2]);
        Assert.IsTrue(ok);

        count = this._redis.HyperLogLog.PFCount(destkey);
        Assert.That(count, Is.EqualTo(6));
    }
}
