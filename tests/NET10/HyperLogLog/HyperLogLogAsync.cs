using SharpRedis;

namespace NET10.HyperLogLog;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class HyperLogLogAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public async Task PFAddAsync()
    {
        const string key = "HyperLogLog:{PFAddAsync}:Key";

        var ok = await this._redis.HyperLogLog.PFAddAsync(key, "a");
        Assert.IsTrue(ok);

        ok = await this._redis.HyperLogLog.PFAddAsync(key, "a");
        Assert.IsFalse(ok);

        ok = await this._redis.HyperLogLog.PFAddAsync(key, ["a", "b", "hello"]);
        Assert.IsTrue(ok);

        ok = await this._redis.HyperLogLog.PFAddAsync(key, ["a", "b", "hello"]);
        Assert.IsFalse(ok);
    }

    [Test]
    public async Task PFCountAsync()
    {
        const string key = "HyperLogLog:{PFCountAsync}:Key";

        int count = await this._redis.HyperLogLog.PFCountAsync(key);
        Assert.Zero(count);

        var ok = await this._redis.HyperLogLog.PFAddAsync(key, "a");
        Assert.IsTrue(ok);

        count = await this._redis.HyperLogLog.PFCountAsync(key);
        Assert.That(count, Is.EqualTo(1));

        ok = await this._redis.HyperLogLog.PFAddAsync(key, "a");
        Assert.IsFalse(ok);

        count = await this._redis.HyperLogLog.PFCountAsync(key);
        Assert.That(count, Is.EqualTo(1));

        ok = this._redis.HyperLogLog.PFAdd(key, ["a", "b", "hello"]);
        Assert.IsTrue(ok);

        count = await this._redis.HyperLogLog.PFCountAsync(key);
        Assert.That(count, Is.EqualTo(3));

        ok = await this._redis.HyperLogLog.PFAddAsync(key, ["a", "b", "hello"]);
        Assert.IsFalse(ok);

        count = await this._redis.HyperLogLog.PFCountAsync(key);
        Assert.That(count, Is.EqualTo(3));
    }

    [Test]
    public async Task PFMergeAsync()
    {
        const string key1 = "HyperLogLog:{PFMergeAsync}:Key1";
        const string key2 = "HyperLogLog:{PFMergeAsync}:Key2";
        const string destkey = "HyperLogLog:{PFMergeAsync}:Destkey";

        var ok = await this._redis.HyperLogLog.PFAddAsync(key1, ["foo", "bar", "zap", "a"]);
        Assert.IsTrue(ok);

        ok = this._redis.HyperLogLog.PFAdd(key2, ["a", "b", "c", "foo"]);
        Assert.IsTrue(ok);

        int count = await this._redis.HyperLogLog.PFCountAsync([key1, key2]);
        Assert.That(count, Is.EqualTo(6));

        ok = await this._redis.HyperLogLog.PFMergeAsync(destkey, [key1, key2]);
        Assert.IsTrue(ok);

        count = await this._redis.HyperLogLog.PFCountAsync(destkey);
        Assert.That(count, Is.EqualTo(6));
    }
}
