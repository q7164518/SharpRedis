using SharpRedis;
using static SharpRedis.RedisGeospatial;

namespace NET10.Geospatial;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class GeospatialAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    [Test]
    public async Task GeoAddAsync()
    {
        const string key = "Geo:{GeoAddAsync}:Key";

        var result = await this._redis.Geo.GeoAddAsync(key, 13.361389, 38.115556, "member1");
        Assert.That(result, Is.EqualTo(1));

        result = await this._redis.Geo.GeoAddAsync(key, 13.361389, 38.115556, "member1");
        Assert.That(result, Is.Zero);

        result = await this._redis.Geo.GeoAddAsync(key, 13.361389, 39.115556, "member1");
        Assert.That(result, Is.Zero);

        result = this._redis.Geo.GeoAdd(key, 13.361389, 39.115556, "member1", true);
        Assert.That(result, Is.Zero);

        result = await this._redis.Geo.GeoAddAsync(key, 13.361389, 37.115556, "member1", when: RedisGeospatial.GeoExist.XX);
        Assert.That(result, Is.Zero);

        result = await this._redis.Geo.GeoAddAsync(key, 13.361389, 37.115556, "member1", ch: true, when: RedisGeospatial.GeoExist.XX);
        Assert.That(result, Is.Zero);

        result = await this._redis.Geo.GeoAddAsync(key, 13.361389, 38.115556, "member1", ch: true, when: RedisGeospatial.GeoExist.XX);
        Assert.That(result, Is.EqualTo(1));

        result = await this._redis.Geo.GeoAddAsync(key, new GeoMember(13.361389, 38.115556, "member2"), ch: true, when: RedisGeospatial.GeoExist.NX);
        Assert.That(result, Is.EqualTo(1));

        result = await this._redis.Geo.GeoAddAsync(key, [new GeoMember(14.361389, 38.115556, "member3"), new GeoMember(15.361389, 38.115556, "member4")]);
        Assert.That(result, Is.EqualTo(2));

        result = await this._redis.Geo.GeoAddAsync(key, [new GeoMember(14.361389, 38.115556, "member3"), new GeoMember(15.361389, 38.115556, "member4")]);
        Assert.That(result, Is.Zero);

        result = await this._redis.Geo.GeoAddAsync(key, [new GeoMember(15.361389, 38.115556, "member3"), new GeoMember(15.361389, 39.115556, "member4")], true);
        Assert.That(result, Is.EqualTo(2));

        result = await this._redis.Geo.GeoAddAsync(key, [new GeoMember(16.361389, 38.115556, "member3"), new GeoMember(15.361389, 39.115556, "member4")], true);
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task GeoDistAsync()
    {
        const string key = "Geo:{GeoDistAsync}:Key";

        var result = await this._redis.Geo.GeoDistAsync(key, "Palermo", "Catania");
        Assert.IsNull(result);

        var add = await this._redis.Geo.GeoAddAsync(key, [new GeoMember(13.361389, 38.115556, "Palermo"), new GeoMember(15.087269, 37.502669, "Catania")]);
        Assert.That(add, Is.EqualTo(2));

        result = await this._redis.Geo.GeoDistAsync(key, "Palermo", "Catania");
        Assert.That(result, Is.EqualTo(166274.1516));

        result = await this._redis.Geo.GeoDistAsync(key, "Palermo", "Catania", GeoDistUnit.M);
        Assert.That(result, Is.EqualTo(166274.1516));

        result = await this._redis.Geo.GeoDistAsync(key, "Palermo", "Catania", GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(166.2742));

        result = await this._redis.Geo.GeoDistAsync(key, "Palermo", "Catania", GeoDistUnit.MI);
        Assert.That(result, Is.EqualTo(103.3182));

        result = await this._redis.Geo.GeoDistAsync(key, "Palermo", "Non");
        Assert.IsNull(result);
    }

    [Test]
    public async Task GeoHashAsync()
    {
        const string key = "Geo:{GeoHashAsync}:Key";

        var result = await this._redis.Geo.GeoAddAsync(key, [new GeoMember(14.361389, 38.115556, "Palermo"), new GeoMember(15.361389, 38.115556, "Catania")]);
        Assert.That(result, Is.EqualTo(2));

        var hash = await this._redis.Geo.GeoHashAsync(key, "Palermo");
        Assert.IsNotNull(hash);
        Assert.IsNotEmpty(hash);

        hash = await this._redis.Geo.GeoHashAsync(key, "Palermo_non");
        Assert.IsNull(hash);

        var hashArray = await this._redis.Geo.GeoHashAsync(key, ["Palermo", "Catania"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(hashArray);
            Assert.That(hashArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(hashArray[0]);
            Assert.IsNotEmpty(hashArray[1]);
        }

        hashArray = await this._redis.Geo.GeoHashAsync(key, ["Palermo", string.Empty, "Catania"]);
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
    public async Task GeoPosAsync()
    {
        const string key = "Geo:{GeoPosAsync}:Key";

        var result = await this._redis.Geo.GeoAddAsync(key, [new GeoMember(14.361389, 38.115556, "Palermo"), new GeoMember(15.361389, 38.115556, "Catania")]);
        Assert.That(result, Is.EqualTo(2));

        var pos = this._redis.Geo.GeoPos(key, "Palermo");
        Assert.NotNull(pos);
        Assert.IsTrue(pos.HasValue);

        pos = await this._redis.Geo.GeoPosAsync(key, "Palermo_none");
        Assert.Null(pos);
        Assert.IsFalse(pos.HasValue);

        pos = await this._redis.Geo.GeoPosAsync("none_geo_pos", "Palermo");
        Assert.Null(pos);
        Assert.IsFalse(pos.HasValue);

        var posArray = await this._redis.Geo.GeoPosAsync(key, ["Palermo", "ss", "Catania"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(posArray);
            Assert.That(posArray, Has.Length.EqualTo(3));
            Assert.IsTrue(posArray[0].HasValue);
            Assert.IsFalse(posArray[1].HasValue);
            Assert.IsTrue(posArray[2].HasValue);
        }

        posArray = await this._redis.Geo.GeoPosAsync("none_geo_pos", ["Palermo", "ss", "Catania"]);
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
    public async Task GeoRadiusAsync()
    {
        const string key = "Geo:{GeoRadiusAsync}:Key";
        const string storeKey = "Geo:{GeoRadiusAsync}:storeKey";
        const string storeDistKey = "Geo:{GeoRadiusAsync}:storeDistKey";

        var count = await this._redis.Geo.GeoAddAsync(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania")]);
        Assert.That(count, Is.EqualTo(2));

        var stringArray = await this._redis.Geo.GeoRadiusAsync(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = await this._redis.Geo.GeoRadiusAsync(key, (14.461389, 38.115556), 1000, GeoDistUnit.KM, 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(1));
            Assert.IsNotEmpty(stringArray![0]);
        }

        stringArray = await this._redis.Geo.GeoRadiusAsync(key, 14.461389, 38.115556, 1000, GeoDistUnit.KM, 1, true, GeoSort.Desc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(1));
            Assert.IsNotEmpty(stringArray![0]);
        }

        stringArray = await this._redis.Geo.GeoRadiusAsync(key, 14.461389, 38.115556, 1, GeoDistUnit.M, 1, true, GeoSort.Desc);
        Assert.IsNull(stringArray);

        var radiusArray = await this._redis.Geo.GeoRadiusAsync(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, true, true, true);
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

        radiusArray = await this._redis.Geo.GeoRadiusAsync(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: true, withDist: false, withHash: true);
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

        radiusArray = await this._redis.Geo.GeoRadiusAsync(key, 14.461389, 38.115556, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true);
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

        radiusArray = await this._redis.Geo.GeoRadiusAsync(key, (14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: false);
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

        radiusArray = await this._redis.Geo.GeoRadiusAsync(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true, 10, true, GeoSort.Asc);
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

        radiusArray = await this._redis.Geo.GeoRadiusAsync(key, new(14.461389, 38.115556), 1, GeoDistUnit.M, withCoord: true, withDist: false, withHash: true, 20, true, GeoSort.Asc);
        Assert.IsNull(radiusArray);

        var zcount = await this._redis.Geo.GeoRadiusStoreAsync(key, storeKey, false, (14.461389, 38.115556), 1000, GeoDistUnit.KM);
        Assert.That(zcount, Is.EqualTo(2));

        zcount = await this._redis.Geo.GeoRadiusStoreAsync(key, storeKey, false, new(14.461389, 38.115556), (1000, GeoDistUnit.KM), 20, false, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(2));

        zcount = await this._redis.Geo.GeoRadiusStoreAsync(key, storeKey, false, 14.461389, 38.115556, 1, GeoDistUnit.M, 20, false, GeoSort.Asc);
        Assert.That(zcount, Is.Zero);

        zcount = await this._redis.Geo.GeoRadiusStoreAsync(key, storeDistKey, true, (14.461389, 38.115556), 1000, GeoDistUnit.KM);
        Assert.That(zcount, Is.EqualTo(2));

        zcount = await this._redis.Geo.GeoRadiusStoreAsync(key, storeDistKey, true, 14.461389, 38.115556, new(1000, GeoDistUnit.KM), 10);
        Assert.That(zcount, Is.EqualTo(2));

        zcount = await this._redis.Geo.GeoRadiusStoreAsync(key, storeDistKey, true, (14.461389, 38.115556), (1000, GeoDistUnit.KM), 10, true, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(2));

        zcount = await this._redis.Geo.GeoRadiusStoreAsync(key, storeDistKey, true, new(14.461389, 38.115556), 1, GeoDistUnit.M, 10, true, GeoSort.Asc);
        Assert.That(zcount, Is.Zero);
    }

    [Test]
    public async Task GeoRadius_RoAsync()
    {
        const string key = "Geo:{GeoRadius_RoAsync}:Key";

        var count = await this._redis.Geo.GeoAddAsync(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania")]);
        Assert.That(count, Is.EqualTo(2));

        var stringArray = await this._redis.Geo.GeoRadius_RoAsync(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = await this._redis.Geo.GeoRadius_RoAsync(key, (14.461389, 38.115556), 1000, GeoDistUnit.KM, 1);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(1));
            Assert.IsNotEmpty(stringArray![0]);
        }

        stringArray = await this._redis.Geo.GeoRadius_RoAsync(key, 14.461389, 38.115556, 1000, GeoDistUnit.KM, 1, true, GeoSort.Desc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(1));
            Assert.IsNotEmpty(stringArray![0]);
        }

        stringArray = await this._redis.Geo.GeoRadius_RoAsync(key, 14.461389, 38.115556, 1, GeoDistUnit.M, 1, true, GeoSort.Desc);
        Assert.IsNull(stringArray);

        var radiusArray = await this._redis.Geo.GeoRadius_RoAsync(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, true, true, true);
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

        radiusArray = await this._redis.Geo.GeoRadius_RoAsync(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: true, withDist: false, withHash: true);
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

        radiusArray = await this._redis.Geo.GeoRadius_RoAsync(key, 14.461389, 38.115556, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true);
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

        radiusArray = await this._redis.Geo.GeoRadius_RoAsync(key, (14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: false);
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

        radiusArray = await this._redis.Geo.GeoRadius_RoAsync(key, new(14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true, 10, true, GeoSort.Asc);
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

        radiusArray = await this._redis.Geo.GeoRadius_RoAsync(key, new(14.461389, 38.115556), 1, GeoDistUnit.M, withCoord: true, withDist: false, withHash: true, 20, true, GeoSort.Asc);
        Assert.IsNull(radiusArray);

        radiusArray = await this._redis.Geo.GeoRadius_RoAsync("none_key_GeoRadius_Ro", new(14.461389, 38.115556), 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true, 10, true, GeoSort.Asc);
        Assert.IsNull(radiusArray);
    }

    [Test]
    public async Task GeoRadiusByMemberAsync()
    {
        const string key = "Geo:{GeoRadiusByMemberAsync}:Key";
        const string storeKey = "Geo:{GeoRadiusByMemberAsync}:storeKey";
        const string storeDistKey = "Geo:{GeoRadiusByMemberAsync}:storeDistKey";
        const string centerName = "Center";

        var count = await this._redis.Geo.GeoAddAsync(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania"), new(14.461389, 38.115556, centerName)]);
        Assert.That(count, Is.EqualTo(3));

        var stringArray = await this._redis.Geo.GeoRadiusByMemberAsync(key, centerName, 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = await this._redis.Geo.GeoRadiusByMemberAsync(key, centerName, 1000, GeoDistUnit.KM, 10, true, GeoSort.Desc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        var radiusArray = await this._redis.Geo.GeoRadiusByMemberAsync(key, centerName, 1000, GeoDistUnit.KM, true, true, true);
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

        radiusArray = await this._redis.Geo.GeoRadiusByMemberAsync(key, centerName, 1000, GeoDistUnit.KM, withCoord: true, withDist: false, withHash: true);
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

        radiusArray = await this._redis.Geo.GeoRadiusByMemberAsync(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true);
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

        radiusArray = await this._redis.Geo.GeoRadiusByMemberAsync(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: false);
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

        radiusArray = await this._redis.Geo.GeoRadiusByMemberAsync(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true, 10, true, GeoSort.Asc);
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

        var zcount = await this._redis.Geo.GeoRadiusByMemberStoreAsync(key, storeKey, false, centerName, 1000, GeoDistUnit.KM);
        Assert.That(zcount, Is.EqualTo(3));

        zcount = await this._redis.Geo.GeoRadiusByMemberStoreAsync(key, storeKey, false, centerName, 1000, GeoDistUnit.KM, 20, false, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(3));

        zcount = await this._redis.Geo.GeoRadiusByMemberStoreAsync(key, storeKey, false, centerName, (1, GeoDistUnit.M), 20, false, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(1));

        zcount = await this._redis.Geo.GeoRadiusByMemberStoreAsync(key, storeDistKey, true, centerName, 1000, GeoDistUnit.KM);
        Assert.That(zcount, Is.EqualTo(3));

        zcount = await this._redis.Geo.GeoRadiusByMemberStoreAsync(key, storeDistKey, true, centerName, (1000, GeoDistUnit.KM), 10);
        Assert.That(zcount, Is.EqualTo(3));

        zcount = await this._redis.Geo.GeoRadiusByMemberStoreAsync(key, storeDistKey, true, centerName, new(1000, GeoDistUnit.KM), 10, true, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(3));

        zcount = await this._redis.Geo.GeoRadiusByMemberStoreAsync(key, storeDistKey, true, centerName, 1, GeoDistUnit.M, 10, true, GeoSort.Asc);
        Assert.That(zcount, Is.EqualTo(1));
    }

    [Test]
    public async Task GeoRadiusByMember_RoAsync()
    {
        const string key = "Geo:{GeoRadiusByMember_RoAsync}:Key";
        const string centerName = "Center";

        var count = await this._redis.Geo.GeoAddAsync(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania"), new(14.461389, 38.115556, centerName)]);
        Assert.That(count, Is.EqualTo(3));

        var stringArray = await this._redis.Geo.GeoRadiusByMember_RoAsync(key, centerName, 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = await this._redis.Geo.GeoRadiusByMember_RoAsync(key, centerName, 1000, GeoDistUnit.KM, 10, true, GeoSort.Desc);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        var radiusArray = await this._redis.Geo.GeoRadiusByMember_RoAsync(key, centerName, 1000, GeoDistUnit.KM, true, true, true);
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

        radiusArray = await this._redis.Geo.GeoRadiusByMember_RoAsync(key, centerName, 1000, GeoDistUnit.KM, withCoord: true, withDist: false, withHash: true);
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

        radiusArray = await this._redis.Geo.GeoRadiusByMember_RoAsync(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true);
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

        radiusArray = await this._redis.Geo.GeoRadiusByMember_RoAsync(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: false);
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

        radiusArray = await this._redis.Geo.GeoRadiusByMember_RoAsync(key, centerName, 1000, GeoDistUnit.KM, withCoord: false, withDist: false, withHash: true, 10, true, GeoSort.Asc);
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
    public async Task GeoSearchAsync()
    {
        const string key = "Geo:{GeoSearchAsync}:Key";
        const string centerName = "Center";

        var count = this._redis.Geo.GeoAdd(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania"), new(14.461389, 38.115556, centerName)]);
        Assert.That(count, Is.EqualTo(3));

        var stringArray = await this._redis.Geo.GeoSearchAsync(key, centerName, 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
            Assert.IsNotEmpty(stringArray![2]);
        }

        stringArray = await this._redis.Geo.GeoSearchAsync(key, centerName, (1000, GeoDistUnit.KM));
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
            Assert.IsNotEmpty(stringArray![2]);
        }

        stringArray = await this._redis.Geo.GeoSearchAsync(key, centerName, (1000, GeoDistUnit.KM), GeoSort.None, 2, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = await this._redis.Geo.GeoSearchAsync(key, centerName, (1000, 1000, GeoDistUnit.KM));
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
            Assert.IsNotEmpty(stringArray![2]);
        }

        stringArray = await this._redis.Geo.GeoSearchAsync(key, (14.461389, 38.115556), 1000, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
            Assert.IsNotEmpty(stringArray![2]);
        }

        stringArray = await this._redis.Geo.GeoSearchAsync(key, (14.461389, 38.115556), 1000, 1200, GeoDistUnit.KM);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(3));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
            Assert.IsNotEmpty(stringArray![2]);
        }

        stringArray = await this._redis.Geo.GeoSearchAsync(key, (14.461389, 38.115556), (1000, GeoDistUnit.KM), GeoSort.Desc, 2, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = await this._redis.Geo.GeoSearchAsync(key, (14.461389, 38.115556), (1000, 1000, GeoDistUnit.KM), GeoSort.Desc, 2, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = await this._redis.Geo.GeoSearchAsync(key, 14.461389, 38.115556, (1000, GeoDistUnit.KM), GeoSort.Desc, 2, false);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = await this._redis.Geo.GeoSearchAsync(key, 14.461389, 38.115556, 1000, 1000, GeoDistUnit.KM, GeoSort.Desc, 2, false);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(2));
            Assert.IsNotEmpty(stringArray![0]);
            Assert.IsNotEmpty(stringArray![1]);
        }

        stringArray = await this._redis.Geo.GeoSearchAsync(key, 14.461389, 38.115556, 1000, GeoDistUnit.KM, GeoSort.Desc, 1, true);
        using (Assert.EnterMultipleScope())
        {
            Assert.IsNotNull(stringArray);
            Assert.That(stringArray, Has.Length.EqualTo(1));
            Assert.IsNotEmpty(stringArray![0]);
        }

        var radiusArray = await this._redis.Geo.GeoSearchAsync(key, centerName, 1000, GeoDistUnit.KM, true, true, true);
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

        radiusArray = await this._redis.Geo.GeoSearchAsync(key, centerName, (1000, GeoDistUnit.KM), true, true, true);
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

        radiusArray = await this._redis.Geo.GeoSearchAsync(key, 14.461389, 38.115556, (1000, GeoDistUnit.KM), true, true, true);
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

        radiusArray = await this._redis.Geo.GeoSearchAsync(key, (14.461389, 38.115556), (1000, GeoDistUnit.KM), true, true, true);
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

        radiusArray = await this._redis.Geo.GeoSearchAsync(key, 14.461389, 38.115556, (1000, 1000, GeoDistUnit.KM), true, true, true);
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

        radiusArray = await this._redis.Geo.GeoSearchAsync(key, 14.461389, 38.115556, (1000, 1000, GeoDistUnit.KM), true, true, false);
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

        radiusArray = await this._redis.Geo.GeoSearchAsync(key, 14.461389, 38.115556, (1000, 1000, GeoDistUnit.KM), true, true, false, GeoSort.Desc, 2, true);
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

        radiusArray = await this._redis.Geo.GeoSearchAsync(key, centerName, (1000, 1000, GeoDistUnit.KM), true, true, false, GeoSort.Desc, 2, true);
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
    public async Task GeoSearchStoreAsync()
    {
        const string key = "Geo:{GeoSearchStoreAsync}:Key";
        const string destkey = "Geo:{GeoSearchStoreAsync}:DestKey";
        const string centerName = "Center";

        var count = await this._redis.Geo.GeoAddAsync(key, [new(14.361389, 38.115556, "Palermo"), new(15.361389, 38.115556, "Catania"), new(14.461389, 38.115556, centerName)]);
        Assert.That(count, Is.EqualTo(3));

        var result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, 14.461389, 38.115556, 1000, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, (14.461389, 38.115556), 1000, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, 14.461389, 38.115556, (1000, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, (14.461389, 38.115556), (1000, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, (14.461389, 38.115556), (1000, GeoDistUnit.MI), GeoSort.Desc, 2, true);
        Assert.That(result, Is.EqualTo(2));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, (14.461389, 38.115556), (1000, GeoDistUnit.MI), GeoSort.Desc, 2, true, true);
        Assert.That(result, Is.EqualTo(2));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, (14.461389, 38.115556), (1000, GeoDistUnit.MI), GeoSort.Desc, 2);
        Assert.That(result, Is.EqualTo(2));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, centerName, 1000, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, centerName, (1000, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, centerName, (1000, GeoDistUnit.MI), GeoSort.Asc, 1, true);
        Assert.That(result, Is.EqualTo(1));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, centerName, (1000, GeoDistUnit.MI), GeoSort.Asc, 1);
        Assert.That(result, Is.EqualTo(1));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, 14.461389, 38.115556, 1000, 1000, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, (14.461389, 38.115556), 1000, 1000, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, 14.461389, 38.115556, (1000, 1001, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, (14.461389, 38.115556), (1000, 2000, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, (14.461389, 38.115556), (1000, 3000.12, GeoDistUnit.MI), GeoSort.Desc, 2, true);
        Assert.That(result, Is.EqualTo(2));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, (14.461389, 38.115556), 1000, 2026.123, GeoDistUnit.MI, GeoSort.Desc, 2);
        Assert.That(result, Is.EqualTo(2));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, centerName, 1000, 1001.123456, GeoDistUnit.KM);
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, centerName, (1000, 6543.21, GeoDistUnit.MI));
        Assert.That(result, Is.EqualTo(3));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, centerName, (1000, 1000, GeoDistUnit.MI), GeoSort.Asc, 1, true);
        Assert.That(result, Is.EqualTo(1));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, centerName, (1000, 2025.78901, GeoDistUnit.MI), GeoSort.Asc, 1);
        Assert.That(result, Is.EqualTo(1));

        result = await this._redis.Geo.GeoSearchStoreAsync(destkey, key, centerName, 1000, 1001.123456, GeoDistUnit.KM, GeoSort.Asc, 30, true, true);
        Assert.That(result, Is.EqualTo(3));
    }
}
