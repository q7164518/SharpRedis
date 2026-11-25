using SharpRedis;
using static SharpRedis.RedisGeospatial;

namespace NET10.Geospatial;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class GeospatialSync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public void GeoAdd()
    {
        const string key = "Geo:{GeoAdd}:Key";

        var result = this._redis.Geo.GeoAdd(key, 13.361389, 38.115556, "member1");
        Assert.That(result, Is.EqualTo(1));

        result = this._redis.Geo.GeoAdd(key, 13.361389, 38.115556, "member1");
        Assert.That(result, Is.Zero);

        result = this._redis.Geo.GeoAdd(key, 13.361389, 39.115556, "member1");
        Assert.That(result, Is.Zero);

        result = this._redis.Geo.GeoAdd(key, 13.361389, 39.115556, "member1", true);
        Assert.That(result, Is.Zero);

        result = this._redis.Geo.GeoAdd(key, 13.361389, 37.115556, "member1", when: RedisGeospatial.GeoExist.XX);
        Assert.That(result, Is.Zero);

        result = this._redis.Geo.GeoAdd(key, 13.361389, 37.115556, "member1", ch: true, when: RedisGeospatial.GeoExist.XX);
        Assert.That(result, Is.Zero);

        result = this._redis.Geo.GeoAdd(key, 13.361389, 38.115556, "member1", ch: true, when: RedisGeospatial.GeoExist.XX);
        Assert.That(result, Is.EqualTo(1));

        result = this._redis.Geo.GeoAdd(key, new GeoMember(13.361389, 38.115556, "member2"), ch: true, when: RedisGeospatial.GeoExist.NX);
        Assert.That(result, Is.EqualTo(1));

        result = this._redis.Geo.GeoAdd(key, [new GeoMember(14.361389, 38.115556, "member3"), new GeoMember(15.361389, 38.115556, "member4")]);
        Assert.That(result, Is.EqualTo(2));

        result = this._redis.Geo.GeoAdd(key, [new GeoMember(14.361389, 38.115556, "member3"), new GeoMember(15.361389, 38.115556, "member4")]);
        Assert.That(result, Is.Zero);

        result = this._redis.Geo.GeoAdd(key, [new GeoMember(15.361389, 38.115556, "member3"), new GeoMember(15.361389, 39.115556, "member4")], true);
        Assert.That(result, Is.EqualTo(2));

        result = this._redis.Geo.GeoAdd(key, [new GeoMember(16.361389, 38.115556, "member3"), new GeoMember(15.361389, 39.115556, "member4")], true);
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void GeoDist()
    {
        const string key = "Geo:{GeoDist}:Key";

        var result = this._redis.Geo.GeoDist(key, "Palermo", "Catania");
        Assert.IsNull(result);

        var add = this._redis.Geo.GeoAdd(key, [new GeoMember(13.361389, 38.115556, "Palermo"), new GeoMember(15.087269, 37.502669, "Catania")]);
        Assert.That(add, Is.EqualTo(2));

        result = this._redis.Geo.GeoDist(key, "Palermo", "Catania");
        Assert.That(result, Is.EqualTo(166274.1516));

        result = this._redis.Geo.GeoDist(key, "Palermo", "Catania", GeoDistUnit.M);
        Assert.That(result, Is.EqualTo(166274.1516));

        result = this._redis.Geo.GeoDist(key, "Palermo", "Catania", GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(166.2742));

        result = this._redis.Geo.GeoDist(key, "Palermo", "Catania", GeoDistUnit.MI);
        Assert.That(result, Is.EqualTo(103.3182));

        result = this._redis.Geo.GeoDist(key, "Palermo", "Non");
        Assert.IsNull(result);
    }

    [Test]
    public void GeoHash()
    {
        const string key = "Geo:{GeoHash}:Key";

        var result = this._redis.Geo.GeoAdd(key, [new GeoMember(14.361389, 38.115556, "Palermo"), new GeoMember(15.361389, 38.115556, "Catania")]);
        Assert.That(result, Is.EqualTo(2));

        var hash = this._redis.Geo.GeoHash(key, "Palermo");
        Assert.IsNotNull(hash);
        Assert.IsNotEmpty(hash);

        hash = this._redis.Geo.GeoHash(key, "Palermo_non");
        Assert.IsNull(hash);

        var hashArray = this._redis.Geo.GeoHash(key, ["Palermo", "Catania"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(hashArray);
            Assert.That(hashArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(hashArray[0]);
            Assert.IsNotEmpty(hashArray[1]);
        }

        hashArray = this._redis.Geo.GeoHash(key, ["Palermo", string.Empty, "Catania"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(hashArray);
            Assert.That(hashArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(hashArray[0]);
            Assert.IsNull(hashArray[1]);
            Assert.IsNotEmpty(hashArray[2]);
        }
    }

    [Test]
    public void GeoPos()
    {
        const string key = "Geo:{GeoPos}:Key";

        var result = this._redis.Geo.GeoAdd(key, [new GeoMember(14.361389, 38.115556, "Palermo"), new GeoMember(15.361389, 38.115556, "Catania")]);
        Assert.That(result, Is.EqualTo(2));

        var pos = this._redis.Geo.GeoPos(key, "Palermo");
        Assert.NotNull(pos);
        Assert.IsTrue(pos.HasValue);

        pos = this._redis.Geo.GeoPos(key, "Palermo_none");
        Assert.Null(pos);
        Assert.IsFalse(pos.HasValue);

        pos = this._redis.Geo.GeoPos("none_geo_pos", "Palermo");
        Assert.Null(pos);
        Assert.IsFalse(pos.HasValue);

        var posArray = this._redis.Geo.GeoPos(key, ["Palermo", "ss", "Catania"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(posArray);
            Assert.That(posArray, Has.Length.EqualTo(3));
            Assert.IsTrue(posArray[0].HasValue);
            Assert.IsFalse(posArray[1].HasValue);
            Assert.IsTrue(posArray[2].HasValue);
        }

        posArray = this._redis.Geo.GeoPos("none_geo_pos", ["Palermo", "ss", "Catania"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(posArray);
            Assert.That(posArray, Has.Length.EqualTo(3));
            Assert.IsFalse(posArray[0].HasValue);
            Assert.IsFalse(posArray[1].HasValue);
            Assert.IsFalse(posArray[2].HasValue);
        }
    }

    [Test]
    public void GeoRadius()
    {
        const string key = "Geo:{GeoRadius}:Key";
        const string storeKey = "Geo:{GeoRadius}:storeKey";
        const string storeDistKey = "Geo:{GeoRadius}:storeDistKey";

        var count = this._redis.Geo.GeoAdd(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania")]);
        Assert.That(count, Is.EqualTo(2));

        var stringArray = this._redis.Geo.GeoRadius(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = this._redis.Geo.GeoRadius(key, (14.461389, 38.115556), 1000, GeoDistUnit.KM, 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(1));
            Assert.IsNotEmpty(stringArray![0]);
        }

        stringArray = this._redis.Geo.GeoRadius(key, 14.461389, 38.115556, 1000, GeoDistUnit.KM, 1, true, GeoSort.Desc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(1));
            Assert.IsNotEmpty(stringArray![0]);
        }

        stringArray = this._redis.Geo.GeoRadius(key, 14.461389, 38.115556, 1, GeoDistUnit.M, 1, true, GeoSort.Desc);
        Assert.IsNull(stringArray);

        var radiusArray = this._redis.Geo.GeoRadius(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, true, true, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadius(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: true, withDist: false, withHash: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadius(key, 14.461389, 38.115556, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadius(key, (14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: false);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadius(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true, 10, true, GeoSort.Asc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadius(key, new(14.461389, 38.115556), 1, GeoDistUnit.M, withCoord: true, withDist: false, withHash: true, 20, true, GeoSort.Asc);
        Assert.IsNull(radiusArray);

        var zcount = this._redis.Geo.GeoRadiusStore(key, storeKey, false, (14.461389, 38.115556), 1000, GeoDistUnit.KM);
        Assert.That(zcount, Is.EqualTo(2));

        zcount = this._redis.Geo.GeoRadiusStore(key, storeKey, false, new(14.461389, 38.115556), (1000, GeoDistUnit.KM), 20, false, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(2));

        zcount = this._redis.Geo.GeoRadiusStore(key, storeKey, false, 14.461389, 38.115556, 1, GeoDistUnit.M, 20, false, GeoSort.Asc);
        Assert.That(zcount, Is.Zero);

        zcount = this._redis.Geo.GeoRadiusStore(key, storeDistKey, true, (14.461389, 38.115556), 1000, GeoDistUnit.KM);
        Assert.That(zcount, Is.EqualTo(2));

        zcount = this._redis.Geo.GeoRadiusStore(key, storeDistKey, true, 14.461389, 38.115556, 1000, GeoDistUnit.KM, 10);
        Assert.That(zcount, Is.EqualTo(2));

        zcount = this._redis.Geo.GeoRadiusStore(key, storeDistKey, true, (14.461389, 38.115556), (1000, GeoDistUnit.KM), 10, true, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(2));

        zcount = this._redis.Geo.GeoRadiusStore(key, storeDistKey, true, new(14.461389, 38.115556), 1, GeoDistUnit.M, 10, true, GeoSort.Asc);
        Assert.That(zcount, Is.Zero);
    }

    [Test]
    public void GeoRadius_Ro()
    {
        const string key = "Geo:{GeoRadius_Ro}:Key";

        var count = this._redis.Geo.GeoAdd(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania")]);
        Assert.That(count, Is.EqualTo(2));

        var stringArray = this._redis.Geo.GeoRadius_Ro(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = this._redis.Geo.GeoRadius_Ro(key, (14.461389, 38.115556), 1000, GeoDistUnit.KM, 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(1));
            Assert.IsNotEmpty(stringArray![0]);
        }

        stringArray = this._redis.Geo.GeoRadius_Ro(key, 14.461389, 38.115556, 1000, GeoDistUnit.KM, 1, true, GeoSort.Desc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(1));
            Assert.IsNotEmpty(stringArray![0]);
        }

        stringArray = this._redis.Geo.GeoRadius_Ro(key, 14.461389, 38.115556, 1, GeoDistUnit.M, 1, true, GeoSort.Desc);
        Assert.IsNull(stringArray);

        var radiusArray = this._redis.Geo.GeoRadius_Ro(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, true, true, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadius_Ro(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: true, withDist: false, withHash: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadius_Ro(key, 14.461389, 38.115556, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadius_Ro(key, (14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: false);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadius_Ro(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true, 10, true, GeoSort.Asc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadius_Ro(key, new(14.461389, 38.115556), 1, GeoDistUnit.M, withCoord: true, withDist: false, withHash: true, 20, true, GeoSort.Asc);
        Assert.IsNull(radiusArray);

        radiusArray = this._redis.Geo.GeoRadius_Ro("none_key_GeoRadius_Ro", new(14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true, 10, true, GeoSort.Asc);
        Assert.IsNull(radiusArray);
    }

    [Test]
    public void GeoRadiusByMember()
    {
        const string key = "Geo:{GeoRadiusByMember}:Key";
        const string storeKey = "Geo:{GeoRadiusByMember}:storeKey";
        const string storeDistKey = "Geo:{GeoRadiusByMember}:storeDistKey";
        const string centerName = "Center";

        var count = this._redis.Geo.GeoAdd(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania"), new(14.461389, 38.115556, centerName)]);
        Assert.That(count, Is.EqualTo(3));

        var stringArray = this._redis.Geo.GeoRadiusByMember(key, centerName, 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = this._redis.Geo.GeoRadiusByMember(key, centerName, 1000, GeoDistUnit.KM, 10, true, GeoSort.Desc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        var radiusArray = this._redis.Geo.GeoRadiusByMember(key, centerName, 1000, GeoDistUnit.KM, true, true, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadiusByMember(key, centerName, 1000, GeoDistUnit.KM, withCoord: true, withDist: false, withHash: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadiusByMember(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadiusByMember(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: false);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadiusByMember(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true, 10, true, GeoSort.Asc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        var zcount = this._redis.Geo.GeoRadiusByMemberStore(key, storeKey, false, centerName, (GeoDistUnit.KM, 1000));
        Assert.That(zcount, Is.EqualTo(3));

        zcount = this._redis.Geo.GeoRadiusByMemberStore(key, storeKey, false, centerName, 1000, GeoDistUnit.KM, 20, false, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(3));

        zcount = this._redis.Geo.GeoRadiusByMemberStore(key, storeKey, false, centerName, 1, GeoDistUnit.M, 20, false, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(1));

        zcount = this._redis.Geo.GeoRadiusByMemberStore(key, storeDistKey, true, centerName, 1000, GeoDistUnit.KM);
        Assert.That(zcount, Is.EqualTo(3));

        zcount = this._redis.Geo.GeoRadiusByMemberStore(key, storeDistKey, true, centerName, (GeoDistUnit.KM, 1000), 10);
        Assert.That(zcount, Is.EqualTo(3));

        zcount = this._redis.Geo.GeoRadiusByMemberStore(key, storeDistKey, true, centerName, 1000, GeoDistUnit.KM, 10, true, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(3));

        zcount = this._redis.Geo.GeoRadiusByMemberStore(key, storeDistKey, true, centerName, (1, GeoDistUnit.M), 10, true, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(1));
    }

    [Test]
    public void GeoRadiusByMember_Ro()
    {
        const string key = "Geo:{GeoRadiusByMember_Ro}:Key";
        const string centerName = "Center";

        var count = this._redis.Geo.GeoAdd(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania"), new(14.461389, 38.115556, centerName)]);
        Assert.That(count, Is.EqualTo(3));

        var stringArray = this._redis.Geo.GeoRadiusByMember_Ro(key, centerName, 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = this._redis.Geo.GeoRadiusByMember_Ro(key, centerName, 1000, GeoDistUnit.KM, 10, true, GeoSort.Desc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        var radiusArray = this._redis.Geo.GeoRadiusByMember_Ro(key, centerName, 1000, GeoDistUnit.KM, true, true, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadiusByMember_Ro(key, centerName, 1000, GeoDistUnit.KM, withCoord: true, withDist: false, withHash: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadiusByMember_Ro(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadiusByMember_Ro(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: false);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoRadiusByMember_Ro(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true, 10, true, GeoSort.Asc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNull(radiusArray![1].Dist);
        }
    }

    [Test]
    public void GeoSearch()
    {
        const string key = "Geo:{GeoSearch}:Key";
        const string centerName = "Center";

        var count = this._redis.Geo.GeoAdd(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania"), new(14.461389, 38.115556, centerName)]);
        Assert.That(count, Is.EqualTo(3));

        var stringArray = this._redis.Geo.GeoSearch(key, centerName, 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
            Assert.IsNotEmpty(stringArray![2]);
        }

        stringArray = this._redis.Geo.GeoSearch(key, centerName, (1000, GeoDistUnit.KM));
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
            Assert.IsNotEmpty(stringArray![2]);
        }

        stringArray = this._redis.Geo.GeoSearch(key, centerName, (1000, GeoDistUnit.KM), GeoSort.None, 2, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = this._redis.Geo.GeoSearch(key, centerName, (1000, 1000, GeoDistUnit.KM));
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
            Assert.IsNotEmpty(stringArray![2]);
        }

        stringArray = this._redis.Geo.GeoSearch(key, (14.461389, 38.115556), 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
            Assert.IsNotEmpty(stringArray![2]);
        }

        stringArray = this._redis.Geo.GeoSearch(key, (14.461389, 38.115556), 1000, 1200, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
            Assert.IsNotEmpty(stringArray![2]);
        }

        stringArray = this._redis.Geo.GeoSearch(key, (14.461389, 38.115556), (1000, GeoDistUnit.KM), GeoSort.Desc, 2, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = this._redis.Geo.GeoSearch(key, (14.461389, 38.115556), (1000, 1000, GeoDistUnit.KM), GeoSort.Desc, 2, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = this._redis.Geo.GeoSearch(key, 14.461389, 38.115556, (1000, GeoDistUnit.KM), GeoSort.Desc, 2, false);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = this._redis.Geo.GeoSearch(key, 14.461389, 38.115556, 1000, 1000, GeoDistUnit.KM, GeoSort.Desc, 2, false);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = this._redis.Geo.GeoSearch(key, 14.461389, 38.115556, 1000, GeoDistUnit.KM, GeoSort.Desc, 1, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(1));
            Assert.IsNotEmpty(stringArray![0]);
        }

        var radiusArray = this._redis.Geo.GeoSearch(key, centerName, 1000, GeoDistUnit.KM, true, true, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoSearch(key, centerName, (1000, GeoDistUnit.KM), true, true, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoSearch(key, 14.461389, 38.115556, (1000, GeoDistUnit.KM), true, true, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoSearch(key, (14.461389, 38.115556), (1000, GeoDistUnit.KM), true, true, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoSearch(key, 14.461389, 38.115556, (1000, 1000, GeoDistUnit.KM), true, true, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNotNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNotNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoSearch(key, 14.461389, 38.115556, (1000, 1000, GeoDistUnit.KM), true, true, false);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(3));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoSearch(key, 14.461389, 38.115556, (1000, 1000, GeoDistUnit.KM), true, true, false, GeoSort.Desc, 2, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }

        radiusArray = this._redis.Geo.GeoSearch(key, centerName, (1000, 1000, GeoDistUnit.KM), true, true, false, GeoSort.Desc, 2, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(radiusArray);
            Assert.That(radiusArray, Has.Length.EqualTo(2));
            Assert.IsNotNull(radiusArray![0]);
            Assert.IsNotNull(radiusArray![0].Coord);
            Assert.IsNull(radiusArray![0].Hash);
            Assert.IsNotNull(radiusArray![0].Dist);

            Assert.IsNotNull(radiusArray![1]);
            Assert.IsNotNull(radiusArray![1].Coord);
            Assert.IsNull(radiusArray![1].Hash);
            Assert.IsNotNull(radiusArray![1].Dist);
        }
    }

    [Test]
    public void GeoSearchStore()
    {
        const string key = "Geo:{GeoSearchStore}:Key";
        const string destkey = "Geo:{GeoSearchStore}:DestKey";
        const string centerName = "Center";

        var count = this._redis.Geo.GeoAdd(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania"), new(14.461389, 38.115556, centerName)]);
        Assert.That(count, Is.EqualTo(3));

        var result = this._redis.Geo.GeoSearchStore(destkey, key, 14.461389, 38.115556, 1000, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, (14.461389, 38.115556), 1000, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, 14.461389, 38.115556, (1000, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, (14.461389, 38.115556), (1000, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, (14.461389, 38.115556), (1000, GeoDistUnit.MI), GeoSort.Desc, 2, true);
        Assert.That(result, Is.EqualTo(2));

        result = this._redis.Geo.GeoSearchStore(destkey, key, (14.461389, 38.115556), (1000, GeoDistUnit.MI), GeoSort.Desc, 2, true, true);
        Assert.That(result, Is.EqualTo(2));

        result = this._redis.Geo.GeoSearchStore(destkey, key, (14.461389, 38.115556), (1000, GeoDistUnit.MI), GeoSort.Desc, 2);
        Assert.That(result, Is.EqualTo(2));

        result = this._redis.Geo.GeoSearchStore(destkey, key, centerName, 1000, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, centerName, (1000, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, centerName, (1000, GeoDistUnit.MI), GeoSort.Asc, 1, true);
        Assert.That(result, Is.EqualTo(1));

        result = this._redis.Geo.GeoSearchStore(destkey, key, centerName, (1000, GeoDistUnit.MI), GeoSort.Asc, 1);
        Assert.That(result, Is.EqualTo(1));

        result = this._redis.Geo.GeoSearchStore(destkey, key, 14.461389, 38.115556, 1000, 1000, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, (14.461389, 38.115556), 1000, 1000, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, 14.461389, 38.115556, (1000, 1001, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, (14.461389, 38.115556), (1000, 2000, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, (14.461389, 38.115556), (1000, 3000.12, GeoDistUnit.MI), GeoSort.Desc, 2, true);
        Assert.That(result, Is.EqualTo(2));

        result = this._redis.Geo.GeoSearchStore(destkey, key, (14.461389, 38.115556), 1000, 2026.123, GeoDistUnit.MI, GeoSort.Desc, 2);
        Assert.That(result, Is.EqualTo(2));

        result = this._redis.Geo.GeoSearchStore(destkey, key, centerName, 1000, 1001.123456, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, centerName, (1000, 6543.21, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = this._redis.Geo.GeoSearchStore(destkey, key, centerName, (1000, 1000, GeoDistUnit.MI), GeoSort.Asc, 1, true);
        Assert.That(result, Is.EqualTo(1));

        result = this._redis.Geo.GeoSearchStore(destkey, key, centerName, (1000, 2025.78901, GeoDistUnit.MI), GeoSort.Asc, 1);
        Assert.That(result, Is.EqualTo(1));

        result = this._redis.Geo.GeoSearchStore(destkey, key, centerName, 1000, 1001.123456, GeoDistUnit.KM, GeoSort.Asc, 30, true, true);
        Assert.That(result, Is.EqualTo(3));
    }
}
