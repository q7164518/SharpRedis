using SharpRedis;
using static SharpRedis.RedisBitmap;

namespace NET10.Bitmap;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class BitmapSync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public void SetGetBit()
    {
        const string key = "Bitmap:{SetGetBit}:Key";

        var getResult = this._redis.Bitmap.GetBit(key, 0);
        Assert.IsFalse(getResult);

        getResult = this._redis.Bitmap.GetBit(key, 1);
        Assert.IsFalse(getResult);

        getResult = this._redis.Bitmap.GetBit(key, 2);
        Assert.IsFalse(getResult);

        var setResult = this._redis.Bitmap.SetBit(key, 0, true);
        Assert.IsFalse(setResult);

        setResult = this._redis.Bitmap.SetBit(key, 2, true);
        Assert.IsFalse(setResult);

        getResult = this._redis.Bitmap.GetBit(key, 0);
        Assert.IsTrue(getResult);

        getResult = this._redis.Bitmap.GetBit(key, 1);
        Assert.IsFalse(getResult);

        getResult = this._redis.Bitmap.GetBit(key, 2);
        Assert.IsTrue(getResult);

        setResult = this._redis.Bitmap.SetBit(key, 2, false);
        Assert.IsTrue(setResult);

        getResult = this._redis.Bitmap.GetBit(key, 2);
        Assert.IsFalse(getResult);
    }

    [Test]
    public void BitCount()
    {
        const string key = "Bitmap:{BitCount}:Key";

        var set = this._redis.String.Set(key, "foobar");
        Assert.IsTrue(set);

        int count = this._redis.Bitmap.BitCount(key);
        Assert.AreEqual(26, count);

        count = this._redis.Bitmap.BitCount(key, 0, 0);
        Assert.AreEqual(4, count);

        count = this._redis.Bitmap.BitCount(key, 1, 1);
        Assert.AreEqual(6, count);

        count = this._redis.Bitmap.BitCount(key, 1, 1, RedisBitmap.BitUnit.Byte);
        Assert.AreEqual(6, count);

        count = this._redis.Bitmap.BitCount(key, 5, 30, RedisBitmap.BitUnit.Bit);
        Assert.AreEqual(17, count);

        count = this._redis.Bitmap.BitCount(key, 0..0);
        Assert.AreEqual(4, count);

        count = this._redis.Bitmap.BitCount(key, ^5..^2);
        Assert.AreEqual(18, count);

        count = this._redis.Bitmap.BitCount(key, 0..);
        Assert.AreEqual(26, count);
    }

    [Test]
    public void BitField()
    {
        const string key = "Bitmap:{BitField}:Key";
        const string key1 = "Bitmap:{BitField1}:Key";
        const string key2 = "Bitmap:{BitField2}:Key";
        const string key3 = "Bitmap:{BitField3}:Key";

        var result = this._redis.Bitmap.BitField(key,
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

        result = this._redis.Bitmap.BitField(key1,
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

        result = this._redis.Bitmap.BitField(key2,
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

        result = this._redis.Bitmap.BitField(key2,
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

        result = this._redis.Bitmap.BitField(key2,
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

        result = this._redis.Bitmap.BitField(key2,
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

        result = this._redis.Bitmap.BitField(key2,
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

        result = this._redis.Bitmap.BitField(key3,
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

        result = this._redis.Bitmap.BitField(key3,
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

        result = this._redis.Bitmap.BitField(key3,
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
    public void BitField_Ro()
    {
        const string key = "Bitmap:{BitField_Ro}:Key";
        const string nonekey = "Bitmap:{BitField_Ro}:Key:None";

        var single = this._redis.Bitmap.BitField_Ro(key, BitFieldArg.Get().IEncoding(21).Offset(0));
        Assert.True(single.HasValue);
        Assert.That((int)single, Is.Zero);

        single = this._redis.Bitmap.BitField_Ro(nonekey, BitFieldArg.Get().IEncoding(21).Offset(0));
        Assert.True(single.HasValue);
        Assert.That((int)single, Is.Zero);

        var result = this._redis.Bitmap.BitField(key,
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

        result = this._redis.Bitmap.BitField(key,
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

        result = this._redis.Bitmap.BitField_Ro(key,
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
    public void BitOp()
    {
        const string key1 = "Bitmap:{BitOp}:Key1";
        const string key2 = "Bitmap:{BitOp}:Key2";
        const string destkey = "Bitmap:{BitOp}:Destkey";

        _ = this._redis.Bitmap.BitField(key1, [BitFieldArg.Set().IEncoding(8).Offset(0).Value(255)]);
        _ = this._redis.Bitmap.BitField(key2, [BitFieldArg.Set().IEncoding(8).Offset(0).Value(85)]);

        long result = this._redis.Bitmap.BitOp(RedisBitmap.BitOperation.And, destkey, [key1, key2]);
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void BitPos()
    {
        const string key = "Bitmap:{BitPos}:Key";

        _ = this._redis.String.Set(key, new byte[]
        {
            0xff,
            0xf0,
            0x00,
        });

        long index = this._redis.Bitmap.BitPos(key, false);
        Assert.That(index, Is.EqualTo(12));

        _ = this._redis.String.Set(key, new byte[]
        {
            0x00,
            0xff,
            0xf0,
        });

        index = this._redis.Bitmap.BitPos(key, true, 0);
        Assert.That(index, Is.EqualTo(8));

        index = this._redis.Bitmap.BitPos(key, true, 2);
        Assert.That(index, Is.EqualTo(16));

        index = this._redis.Bitmap.BitPos(key, true, 2, -1, RedisBitmap.BitUnit.Byte);
        Assert.That(index, Is.EqualTo(16));

        index = this._redis.Bitmap.BitPos(key, true, 7, 15, RedisBitmap.BitUnit.Bit);
        Assert.That(index, Is.EqualTo(8));

        _ = this._redis.String.Set(key, new byte[]
        {
            0x00,
            0x00,
            0x00,
        });

        index = this._redis.Bitmap.BitPos(key, true);
        Assert.That(index, Is.EqualTo(-1));
    }
}
