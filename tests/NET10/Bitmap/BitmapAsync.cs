using SharpRedis;
using static SharpRedis.RedisBitmap;

namespace NET10.Bitmap;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class BitmapAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public async Task SetGetBitAsync()
    {
        const string key = "Bitmap:{SetGetBitAsync}:Key";

        var getResult = await this._redis.Bitmap.GetBitAsync(key, 0);
        Assert.IsFalse(getResult);

        getResult = await this._redis.Bitmap.GetBitAsync(key, 1);
        Assert.IsFalse(getResult);

        getResult = await this._redis.Bitmap.GetBitAsync(key, 2);
        Assert.IsFalse(getResult);

        var setResult = await this._redis.Bitmap.SetBitAsync(key, 0, true);
        Assert.IsFalse(setResult);

        setResult = await this._redis.Bitmap.SetBitAsync(key, 2, true);
        Assert.IsFalse(setResult);

        int count = await this._redis.Bitmap.BitCountAsync(key);
        Assert.AreEqual(2, count);

        getResult = await this._redis.Bitmap.GetBitAsync(key, 0);
        Assert.IsTrue(getResult);

        getResult = await this._redis.Bitmap.GetBitAsync(key, 1);
        Assert.IsFalse(getResult);

        getResult = await this._redis.Bitmap.GetBitAsync(key, 2);
        Assert.IsTrue(getResult);

        setResult = await this._redis.Bitmap.SetBitAsync(key, 2, false);
        Assert.IsTrue(setResult);

        getResult = await this._redis.Bitmap.GetBitAsync(key, 2);
        Assert.IsFalse(getResult);

        count = await this._redis.Bitmap.BitCountAsync(key);
        Assert.AreEqual(1, count);
    }

    [Test]
    public async Task BitCountAsync()
    {
        const string key = "Bitmap:{BitCountAsync}:Key";

        var set = await this._redis.String.SetAsync(key, "foobar");
        Assert.IsTrue(set);

        int count = await this._redis.Bitmap.BitCountAsync(key);
        Assert.AreEqual(26, count);

        count = await this._redis.Bitmap.BitCountAsync(key, 0, 0);
        Assert.AreEqual(4, count);

        count = await this._redis.Bitmap.BitCountAsync(key, 1, 1);
        Assert.AreEqual(6, count);

        count = await this._redis.Bitmap.BitCountAsync(key, 1, 1, RedisBitmap.BitUnit.Byte);
        Assert.AreEqual(6, count);

        count = await this._redis.Bitmap.BitCountAsync(key, 5, 30, RedisBitmap.BitUnit.Bit);
        Assert.AreEqual(17, count);

        count = await this._redis.Bitmap.BitCountAsync(key, 0..0);
        Assert.AreEqual(4, count);

        count = await this._redis.Bitmap.BitCountAsync(key, ^5..^2);
        Assert.AreEqual(18, count);

        count = await this._redis.Bitmap.BitCountAsync(key, 0..);
        Assert.AreEqual(26, count);
    }

    [Test]
    public async Task BitFieldAsync()
    {
        const string key = "Bitmap:{BitFieldAsync}:Key";
        const string key1 = "Bitmap:{BitFieldAsync1}:Key";
        const string key2 = "Bitmap:{BitFieldAsync2}:Key";
        const string key3 = "Bitmap:{BitFieldAsync3}:Key";

        var result = await this._redis.Bitmap.BitFieldAsync(key,
        [
            BitFieldArg.IncrBy().IEncoding(5).Offset(100).Increment(1),
            BitFieldArg.Get().UEncoding(4).Offset(0),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That((int)result[0], Is.EqualTo(1));
            Assert.That((int)result[1], Is.Zero);
        }

        result = await this._redis.Bitmap.BitFieldAsync(key1,
        [
            BitFieldArg.IncrBy("i5","100",1),
            BitFieldArg.Get("u4","0"),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That((int)result[0], Is.EqualTo(1));
            Assert.That((int)result[1], Is.Zero);
        }

        result = await this._redis.Bitmap.BitFieldAsync(key2,
        [
            BitFieldArg.IncrBy().UEncoding(2).Offset(100).Increment(1),
            BitFieldArg.Overflow().Sat(),
            BitFieldArg.IncrBy().UEncoding(2).Offset(102).Increment(1),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That((int)result[0], Is.EqualTo(1));
            Assert.That((int)result[1], Is.EqualTo(1));
        }

        result = await this._redis.Bitmap.BitFieldAsync(key2,
        [
            BitFieldArg.IncrBy().UEncoding(2).Offset(100).Increment(1),
            BitFieldArg.Overflow().Sat(),
            BitFieldArg.IncrBy().UEncoding(2).Offset(102).Increment(1),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That((int)result[0], Is.EqualTo(2));
            Assert.That((int)result[1], Is.EqualTo(2));
        }

        result = await this._redis.Bitmap.BitFieldAsync(key2,
        [
            BitFieldArg.IncrBy().UEncoding(2).Offset(100).Increment(1),
            BitFieldArg.Overflow().Sat(),
            BitFieldArg.IncrBy().UEncoding(2).Offset(102).Increment(1),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That((int)result[0], Is.EqualTo(3));
            Assert.That((int)result[1], Is.EqualTo(3));
        }

        result = await this._redis.Bitmap.BitFieldAsync(key2,
        [
            BitFieldArg.IncrBy().UEncoding(2).Offset(100).Increment(1),
            BitFieldArg.Overflow().Sat(),
            BitFieldArg.IncrBy().UEncoding(2).Offset(102).Increment(1),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That((int)result[0], Is.Zero);
            Assert.That((int)result[1], Is.EqualTo(3));
        }

        result = await this._redis.Bitmap.BitFieldAsync(key2,
        [
            BitFieldArg.Overflow("FAIL"),
            BitFieldArg.IncrBy().UEncoding(2).Offset(102).Increment(1),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.True(result[0].IsNull);
            Assert.False(result[0].HasValue);
        }

        result = await this._redis.Bitmap.BitFieldAsync(key3,
        [
            BitFieldArg.Set().IEncoding(32).Offset(0).Value(12138),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.True(result[0].HasValue);
            Assert.That((int)result[0], Is.Zero);
        }

        result = await this._redis.Bitmap.BitFieldAsync(key3,
        [
            BitFieldArg.Get().IEncoding(32).Offset(0),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.True(result[0].HasValue);
            Assert.That((int)result[0], Is.EqualTo(12138));
        }

        result = await this._redis.Bitmap.BitFieldAsync(key3,
        [
            BitFieldArg.IncrBy("i32", "0", -2)
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.True(result[0].HasValue);
            Assert.That((int)result[0], Is.EqualTo(12136));
        }
    }

    [Test]
    public async Task BitField_RoAsync()
    {
        const string key = "Bitmap:{BitField_RoAsync}:Key";
        const string nonekey = "Bitmap:{BitField_RoAsync}:Key:None";

        var single = await this._redis.Bitmap.BitField_RoAsync(key, BitFieldArg.Get().IEncoding(21).Offset(0));
        Assert.True(single.HasValue);
        Assert.That((int)single, Is.Zero);

        single = await this._redis.Bitmap.BitField_RoAsync(nonekey, BitFieldArg.Get().IEncoding(21).Offset(0));
        Assert.True(single.HasValue);
        Assert.That((int)single, Is.Zero);

        var result = await this._redis.Bitmap.BitFieldAsync(key,
        [
            BitFieldArg.Set().IEncoding(32).Offset(0).Value(12138),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.True(result[0].HasValue);
            Assert.That((int)result[0], Is.Zero);
        }

        single = this._redis.Bitmap.BitField_Ro(key, BitFieldArg.Get().IEncoding(32).Offset(0));
        Assert.True(single.HasValue);
        Assert.That((int)single, Is.EqualTo(12138));

        result = await this._redis.Bitmap.BitFieldAsync(key,
        [
            BitFieldArg.Set().IEncoding(32).Offset(0).Value(2026),
            BitFieldArg.Set().IEncoding(32).Offset(1, true).Value(9527),
            BitFieldArg.Set().IEncoding(32).Offset(2, true).Value(6379),
            BitFieldArg.Set().IEncoding(32).Offset(96).Value(1433),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(4));
            Assert.True(result[0].HasValue);
            Assert.That((int)result[0], Is.EqualTo(12138));
            Assert.That((int)result[1], Is.Zero);
            Assert.That((int)result[2], Is.Zero);
            Assert.That((int)result[3], Is.Zero);
        }

        result = await this._redis.Bitmap.BitField_RoAsync(key,
        [
            BitFieldArg.Get().IEncoding(32).Offset(0),
            BitFieldArg.Get().IEncoding(32).Offset(1, true),
            BitFieldArg.Get().IEncoding(32).Offset(2, true),
            BitFieldArg.Get().IEncoding(32).Offset(96),
        ]);

        Assert.NotNull(result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(4));
            Assert.That((int)result[0], Is.EqualTo(2026));
            Assert.That((int)result[1], Is.EqualTo(9527));
            Assert.That((int)result[2], Is.EqualTo(6379));
            Assert.That((int)result[3], Is.EqualTo(1433));
        }
    }

    [Test]
    public async Task BitOpAsync()
    {
        const string key1 = "Bitmap:{BitOpAsync}:Key1";
        const string key2 = "Bitmap:{BitOpAsync}:Key2";
        const string destkey = "Bitmap:{BitOpAsync}:Destkey";

        _ = await this._redis.Bitmap.BitFieldAsync(key1, [BitFieldArg.Set().IEncoding(8).Offset(0).Value(255)]);
        _ = await this._redis.Bitmap.BitFieldAsync(key2, [BitFieldArg.Set().IEncoding(8).Offset(0).Value(85)]);

        long result = await this._redis.Bitmap.BitOpAsync(RedisBitmap.BitOperation.And, destkey, [key1, key2]);
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task BitPosAsync()
    {
        const string key = "Bitmap:{BitPosAsync}:Key";

        _ = await this._redis.String.SetAsync(key, new byte[]
        {
            0xff,
            0xf0,
            0x00,
        });

        long index = await this._redis.Bitmap.BitPosAsync(key, false);
        Assert.That(index, Is.EqualTo(12));

        _ = this._redis.String.Set(key, new byte[]
        {
            0x00,
            0xff,
            0xf0,
        });

        index = await this._redis.Bitmap.BitPosAsync(key, true, 0);
        Assert.That(index, Is.EqualTo(8));

        index = await this._redis.Bitmap.BitPosAsync(key, true, 2);
        Assert.That(index, Is.EqualTo(16));

        index = await this._redis.Bitmap.BitPosAsync(key, true, 2, -1, RedisBitmap.BitUnit.Byte);
        Assert.That(index, Is.EqualTo(16));

        index = await this._redis.Bitmap.BitPosAsync(key, true, 7, 15, RedisBitmap.BitUnit.Bit);
        Assert.That(index, Is.EqualTo(8));

        _ = await this._redis.String.SetAsync(key, new byte[]
        {
            0x00,
            0x00,
            0x00,
        });

        index = await this._redis.Bitmap.BitPosAsync(key, true);
        Assert.That(index, Is.EqualTo(-1));
    }
}
