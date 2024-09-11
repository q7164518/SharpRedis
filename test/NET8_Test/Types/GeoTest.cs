using SharpRedis;
using System.Text;

namespace NET8_Test.Types;

public class GeoTest
{
    [Theory, ClassData(typeof(RedisProvider))]
    public void GeoAdd(Redis redis)
    {
        const string key = "geo_geoadd";

        _ = redis.Key.Del(key);

        var count = redis.Geospatial.GeoAdd(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", new CoordinateValue(13.361389D, 38.115556D) },
            { "Catania", new CoordinateValue(15.087269D, 37.502669D) },
        });
        Assert.Equal(2, count);

        var dist = redis.Geospatial.GeoDist(key, "Palermo", "Catania");
        Assert.Equal(166274.1516D, dist);

        dist = redis.Geospatial.GeoDist(key, "Palermo", "Catania", DistanceUnit.km);
        Assert.Equal(166.2742D, dist);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GeoAddAsync(Redis redis)
    {
        const string key = "geo_geoadd_async";

        _ = await redis.Key.DelAsync(key);

        var count = await redis.Geospatial.GeoAddAsync(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", new CoordinateValue(13.361389D, 38.115556D) },
            { "Catania", new CoordinateValue(15.087269D, 37.502669D) },
        });
        Assert.Equal(2, count);

        var dist = await redis.Geospatial.GeoDistAsync(key, "Palermo", "Catania");
        Assert.Equal(166274.1516D, dist);

        dist = await redis.Geospatial.GeoDistAsync(key, "Palermo", "Catania", DistanceUnit.km);
        Assert.Equal(166.2742D, dist);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GeoDist(Redis redis)
    {
        const string key = "geo_geodist";

        _ = redis.Key.Del(key);

        var count = redis.Geospatial.GeoAdd(key, "Palermo", 13.361389D, 38.115556D);
        Assert.Equal(1, count);

        count = redis.Geospatial.GeoAdd(key, Encoding.UTF8.GetBytes("Catania"), 15.087269, 37.502669D);
        Assert.Equal(1, count);

        var dist = redis.Geospatial.GeoDist(key, "Palermo", "Catania");
        Assert.Equal(166274.1516D, dist);

        dist = redis.Geospatial.GeoDist(key, "Palermo", "Catania", DistanceUnit.km);
        Assert.Equal(166.2742D, dist);

        dist = redis.Geospatial.GeoDist(key, "Palermo_none", "Catania_none");
        Assert.False(dist.HasValue);

        dist = redis.Geospatial.GeoDist(key, "Palermo", "Catania_none");
        Assert.False(dist.HasValue);

        dist = redis.Geospatial.GeoDist(key, Encoding.UTF8.GetBytes("Palermo"), Encoding.UTF8.GetBytes("Catania"));
        Assert.Equal(166274.1516D, dist);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GeoDistAsync(Redis redis)
    {
        const string key = "geo_geodist_async";

        _ = await redis.Key.DelAsync(key);

        var count = await redis.Geospatial.GeoAddAsync(key, "Palermo", 13.361389D, 38.115556d);
        Assert.Equal(1, count);

        count = await redis.Geospatial.GeoAddAsync(key, Encoding.UTF8.GetBytes("Catania"), 15.087269d, 37.502669d);
        Assert.Equal(1, count);

        var dist = await redis.Geospatial.GeoDistAsync(key, "Palermo", "Catania");
        Assert.Equal(166274.1516d, dist);

        dist = await redis.Geospatial.GeoDistAsync(key, "Palermo", "Catania", DistanceUnit.km);
        Assert.Equal(166.2742d, dist);

        dist = await redis.Geospatial.GeoDistAsync(key, "Palermo_none", "Catania_none");
        Assert.False(dist.HasValue);

        dist = await redis.Geospatial.GeoDistAsync(key, "Palermo", "Catania_none");
        Assert.False(dist.HasValue);

        dist = await redis.Geospatial.GeoDistAsync(key, Encoding.UTF8.GetBytes("Palermo"), Encoding.UTF8.GetBytes("Catania"));
        Assert.Equal(166274.1516d, dist);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GeoHash(Redis redis)
    {
        const string key = "geo_geohash";

        _ = redis.Key.Del(key);

        var count = redis.Geospatial.GeoAdd(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", new CoordinateValue(13.361389D, 38.115556D) },
            { "Catania", new CoordinateValue(15.087269D, 37.502669D) },
        });
        Assert.Equal(2, count);

        var hash = redis.Geospatial.GeoHash(key, "Palermo");
        Assert.Equal("sqc8b49rny0", hash);

        hash = redis.Geospatial.GeoHash(key, "Palermo_none");
        Assert.Null(hash);

        var hashArray = redis.Geospatial.GeoHash(key, ["Palermo", "Catania"]);
        Assert.NotNull(hashArray);
        Assert.Equal(2, hashArray.Length);
        Assert.Equal<string?[]>(["sqc8b49rny0", "sqdtr74hyu0"], hashArray);

        hashArray = redis.Geospatial.GeoHash(key, ["Palermo", "Catania_none"]);
        Assert.NotNull(hashArray);
        Assert.Equal(2, hashArray.Length);
        Assert.Equal<string?[]>(["sqc8b49rny0", null], hashArray);

        hashArray = redis.Geospatial.GeoHash(key, ["Palermo_none", "Catania_none"]);
        Assert.NotNull(hashArray);
        Assert.Equal(2, hashArray.Length);
        Assert.Equal<string?[]>([null, null], hashArray);

        hashArray = redis.Geospatial.GeoHash("none_key____kkkkk", ["Palermo", "Catania"]);
        Assert.NotNull(hashArray);
        Assert.Equal(2, hashArray.Length);
        Assert.Equal<string?[]>([null, null], hashArray);

        hashArray = redis.Geospatial.GeoHash(key, [Encoding.UTF8.GetBytes("Palermo"), Encoding.UTF8.GetBytes("Catania")]);
        Assert.NotNull(hashArray);
        Assert.Equal(2, hashArray.Length);
        Assert.Equal<string?[]>(["sqc8b49rny0", "sqdtr74hyu0"], hashArray);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GeoHashAsync(Redis redis)
    {
        const string key = "geo_geohash_async";

        _ = await redis.Key.DelAsync(key);

        var count = await redis.Geospatial.GeoAddAsync(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", new CoordinateValue(13.361389D, 38.115556d) },
            { "Catania", new CoordinateValue(15.087269D, 37.502669d) },
        });
        Assert.Equal(2, count);

        var hash = await redis.Geospatial.GeoHashAsync(key, "Palermo");
        Assert.Equal("sqc8b49rny0", hash);

        hash = await redis.Geospatial.GeoHashAsync(key, "Palermo_none");
        Assert.Null(hash);

        var hashArray = await redis.Geospatial.GeoHashAsync(key, ["Palermo", "Catania"]);
        Assert.NotNull(hashArray);
        Assert.Equal(2, hashArray.Length);
        Assert.Equal<string?[]>(["sqc8b49rny0", "sqdtr74hyu0"], hashArray);

        hashArray = await redis.Geospatial.GeoHashAsync(key, ["Palermo", "Catania_none"]);
        Assert.NotNull(hashArray);
        Assert.Equal(2, hashArray.Length);
        Assert.Equal<string?[]>(["sqc8b49rny0", null], hashArray);

        hashArray = await redis.Geospatial.GeoHashAsync(key, ["Palermo_none", "Catania_none"]);
        Assert.NotNull(hashArray);
        Assert.Equal(2, hashArray.Length);
        Assert.Equal<string?[]>([null, null], hashArray);

        hashArray = await redis.Geospatial.GeoHashAsync("none_key____kkkkk", ["Palermo", "Catania"]);
        Assert.NotNull(hashArray);
        Assert.Equal(2, hashArray.Length);
        Assert.Equal<string?[]>([null, null], hashArray);

        hashArray = await redis.Geospatial.GeoHashAsync(key, [Encoding.UTF8.GetBytes("Palermo"), Encoding.UTF8.GetBytes("Catania")]);
        Assert.NotNull(hashArray);
        Assert.Equal(2, hashArray.Length);
        Assert.Equal<string?[]>(["sqc8b49rny0", "sqdtr74hyu0"], hashArray);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GeoPos(Redis redis)
    {
        const string key = "geo_geopos";

        _ = redis.Key.Del(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var count = redis.Geospatial.GeoAdd(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
        });
        Assert.Equal(2, count);

        var pos = redis.Geospatial.GeoPos(key, "Palermo");
        Assert.True(pos.HasValue);
        Assert.Equal(13.36138933897018433, pos.Value.Longitude);
        Assert.Equal(38.11555639549629859, pos.Value.Latitude);

        pos = redis.Geospatial.GeoPos(key, Encoding.UTF8.GetBytes("Catania"));
        Assert.True(pos.HasValue);
        Assert.Equal(15.08726745843887329, pos.Value.Longitude);
        Assert.Equal(37.50266842333162032, pos.Value.Latitude);

        pos = redis.Geospatial.GeoPos(key, "Palermo_none");
        Assert.False(pos.HasValue);

        pos = redis.Geospatial.GeoPos("nonono_Key_keykkkkk", "Palermo_none");
        Assert.False(pos.HasValue);

        var posArray = redis.Geospatial.GeoPos(key, ["Palermo", "Catania", "nono", "Palermo"]);
        Assert.NotNull(posArray);
        Assert.Equal(4, posArray.Length);
        Assert.True(posArray[0].HasValue);
        Assert.Equal(13.36138933897018433, posArray[0]!.Value.Longitude);
        Assert.Equal(38.11555639549629859, posArray[0]!.Value.Latitude);
        Assert.True(posArray[1].HasValue);
        Assert.Equal(15.08726745843887329, posArray[1]!.Value.Longitude);
        Assert.Equal(37.50266842333162032, posArray[1]!.Value.Latitude);
        Assert.False(posArray[2].HasValue);
        Assert.True(posArray[3].HasValue);
        Assert.Equal(13.36138933897018433, posArray[3]!.Value.Longitude);
        Assert.Equal(38.11555639549629859, posArray[3]!.Value.Latitude);

        posArray = redis.Geospatial.GeoPos(key, [Encoding.UTF8.GetBytes("Palermo"), Encoding.UTF8.GetBytes("Catania")]);
        Assert.NotNull(posArray);
        Assert.Equal(2, posArray.Length);
        Assert.True(posArray[0].HasValue);
        Assert.Equal(13.36138933897018433, posArray[0]!.Value.Longitude);
        Assert.Equal(38.11555639549629859, posArray[0]!.Value.Latitude);
        Assert.True(posArray[1].HasValue);
        Assert.Equal(15.08726745843887329, posArray[1]!.Value.Longitude);
        Assert.Equal(37.50266842333162032, posArray[1]!.Value.Latitude);

        posArray = redis.Geospatial.GeoPos(key, ["Palermo_no", "Catania_no", "nono", "Palermo_no"]);
        Assert.NotNull(posArray);
        Assert.Equal(4, posArray.Length);
        Assert.False(posArray[0].HasValue);
        Assert.False(posArray[1].HasValue);
        Assert.False(posArray[2].HasValue);
        Assert.False(posArray[3].HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GeoPosAsync(Redis redis)
    {
        const string key = "geo_geopos_async";

        _ = await redis.Key.DelAsync(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var count = await redis.Geospatial.GeoAddAsync(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
        });
        Assert.Equal(2, count);

        var pos = await redis.Geospatial.GeoPosAsync(key, "Palermo");
        Assert.True(pos.HasValue);
        Assert.Equal(13.36138933897018433, pos.Value.Longitude);
        Assert.Equal(38.11555639549629859, pos.Value.Latitude);

        pos = await redis.Geospatial.GeoPosAsync(key, Encoding.UTF8.GetBytes("Catania"));
        Assert.True(pos.HasValue);
        Assert.Equal(15.08726745843887329, pos.Value.Longitude);
        Assert.Equal(37.50266842333162032, pos.Value.Latitude);

        pos = await redis.Geospatial.GeoPosAsync(key, "Palermo_none");
        Assert.False(pos.HasValue);

        pos = await redis.Geospatial.GeoPosAsync("nonono_Key_keykkkkk", "Palermo_none");
        Assert.False(pos.HasValue);

        var posArray = await redis.Geospatial.GeoPosAsync(key, ["Palermo", "Catania", "nono", "Palermo"]);
        Assert.NotNull(posArray);
        Assert.Equal(4, posArray.Length);
        Assert.True(posArray[0].HasValue);
        Assert.Equal(13.36138933897018433, posArray[0]!.Value.Longitude);
        Assert.Equal(38.11555639549629859, posArray[0]!.Value.Latitude);
        Assert.True(posArray[1].HasValue);
        Assert.Equal(15.08726745843887329, posArray[1]!.Value.Longitude);
        Assert.Equal(37.50266842333162032, posArray[1]!.Value.Latitude);
        Assert.False(posArray[2].HasValue);
        Assert.True(posArray[3].HasValue);
        Assert.Equal(13.36138933897018433, posArray[3]!.Value.Longitude);
        Assert.Equal(38.11555639549629859, posArray[3]!.Value.Latitude);

        posArray = await redis.Geospatial.GeoPosAsync(key, [Encoding.UTF8.GetBytes("Palermo"), Encoding.UTF8.GetBytes("Catania")]);
        Assert.NotNull(posArray);
        Assert.Equal(2, posArray.Length);
        Assert.True(posArray[0].HasValue);
        Assert.Equal(13.36138933897018433, posArray[0]!.Value.Longitude);
        Assert.Equal(38.11555639549629859, posArray[0]!.Value.Latitude);
        Assert.True(posArray[1].HasValue);
        Assert.Equal(15.08726745843887329, posArray[1]!.Value.Longitude);
        Assert.Equal(37.50266842333162032, posArray[1]!.Value.Latitude);

        posArray = await redis.Geospatial.GeoPosAsync(key, ["Palermo_no", "Catania_no", "nono", "Palermo_no"]);
        Assert.NotNull(posArray);
        Assert.Equal(4, posArray.Length);
        Assert.False(posArray[0].HasValue);
        Assert.False(posArray[1].HasValue);
        Assert.False(posArray[2].HasValue);
        Assert.False(posArray[3].HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GeoRadius(Redis redis)
    {
        const string key = "geo_georadius";
        const string storeKey = "geo_georadius_store";

        _ = redis.Key.Del(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var count = redis.Geospatial.GeoAdd(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
        });
        Assert.Equal(2, count);

        var result = redis.Geospatial.GeoRadius(key, 16.11, 33.45, 100000, DistanceUnit.km);
        Assert.NotNull(result);
        Assert.Equal(2, result.Length);

        result = redis.Geospatial.GeoRadius(key, 16.11, 33.45, 100000, DistanceUnit.km, 1);
        Assert.NotNull(result);
        Assert.Single(result);

        result = redis.Geospatial.GeoRadius(key, 16.11, 33.45, 1000, DistanceUnit.m);
        Assert.Null(result);

        var resultBytes = redis.Geospatial.GeoRadiusBytes(key, 16.11, 33.45, 100000, DistanceUnit.km);
        Assert.NotNull(resultBytes);
        Assert.Equal(2, resultBytes.Length);

        resultBytes = redis.Geospatial.GeoRadiusBytes(key, 16.11, 33.45, 1000, DistanceUnit.m);
        Assert.Null(resultBytes);

        var dist = redis.Geospatial.GeoRadius(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Catania", dist[0].Member);
        Assert.False(dist[0].Coordinate.HasValue);
        Assert.False(dist[1].Coordinate.HasValue);
        Assert.False(dist[0].Dist.HasValue);
        Assert.False(dist[1].Dist.HasValue);
        Assert.False(dist[0].Hash.HasValue);
        Assert.False(dist[1].Hash.HasValue);

        dist = redis.Geospatial.GeoRadius(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);

        dist = redis.Geospatial.GeoRadius(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Catania", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);


        dist = redis.Geospatial.GeoRadius(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);

        dist = redis.Geospatial.GeoRadius(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);

        dist = redis.Geospatial.GeoRadius(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);
        Assert.True(dist[0].Hash.HasValue);
        Assert.True(dist[1].Hash.HasValue);

        dist = redis.Geospatial.GeoRadius(key, 16.11, 33.45, 100, DistanceUnit.m, true, true, true);
        Assert.Null(dist);

        var distBytes = redis.Geospatial.GeoRadiusBytes(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(2, distBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("Catania"), distBytes[0].Member);
        Assert.True(distBytes[0].Coordinate.HasValue);
        Assert.True(distBytes[1].Coordinate.HasValue);

        var countResult = redis.Geospatial.GeoRadiusStore(storeKey, key, 16.11, 33.45, 10000, DistanceUnit.km);
        Assert.Equal(2, countResult);

        countResult = redis.Geospatial.GeoRadiusStore(storeKey, key, 16.11, 33.45, 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = redis.Geospatial.GeoRadiusStore(storeKey, key, 16.11, 33.45, 100, DistanceUnit.m, 1);
        Assert.Equal(0, countResult);

        countResult = redis.Geospatial.GeoRadiusStoreDist(storeKey, key, 16.11, 33.45, 10000, DistanceUnit.km);
        Assert.Equal(2, countResult);

        countResult = redis.Geospatial.GeoRadiusStoreDist(storeKey, key, 16.11, 33.45, 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = redis.Geospatial.GeoRadiusStoreDist(storeKey, key, 16.11, 33.45, 100, DistanceUnit.m, 1);
        Assert.Equal(0, countResult);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GeoRadiusAsync(Redis redis)
    {
        const string key = "geo_georadius_async";
        const string storeKey = "geo_georadius_store_async";

        _ = await redis.Key.DelAsync(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var count = await redis.Geospatial.GeoAddAsync(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
        });
        Assert.Equal(2, count);

        var result = await redis.Geospatial.GeoRadiusAsync(key, 16.11, 33.45, 100000, DistanceUnit.km);
        Assert.NotNull(result);
        Assert.Equal(2, result.Length);

        result = await redis.Geospatial.GeoRadiusAsync(key, 16.11, 33.45, 100000, DistanceUnit.km, 1);
        Assert.NotNull(result);
        Assert.Single(result);

        result = await redis.Geospatial.GeoRadiusAsync(key, 16.11, 33.45, 1000, DistanceUnit.m);
        Assert.Null(result);

        var resultBytes = await redis.Geospatial.GeoRadiusBytesAsync(key, 16.11, 33.45, 100000, DistanceUnit.km);
        Assert.NotNull(resultBytes);
        Assert.Equal(2, resultBytes.Length);

        resultBytes = await redis.Geospatial.GeoRadiusBytesAsync(key, 16.11, 33.45, 1000, DistanceUnit.m);
        Assert.Null(resultBytes);

        var dist = await redis.Geospatial.GeoRadiusAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Catania", dist[0].Member);
        Assert.False(dist[0].Coordinate.HasValue);
        Assert.False(dist[1].Coordinate.HasValue);
        Assert.False(dist[0].Dist.HasValue);
        Assert.False(dist[1].Dist.HasValue);
        Assert.False(dist[0].Hash.HasValue);
        Assert.False(dist[1].Hash.HasValue);

        dist = await redis.Geospatial.GeoRadiusAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);

        dist = await redis.Geospatial.GeoRadiusAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Catania", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);


        dist = await redis.Geospatial.GeoRadiusAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);

        dist = await redis.Geospatial.GeoRadiusAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);

        dist = await redis.Geospatial.GeoRadiusAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);
        Assert.True(dist[0].Hash.HasValue);
        Assert.True(dist[1].Hash.HasValue);

        dist = await redis.Geospatial.GeoRadiusAsync(key, 16.11, 33.45, 100, DistanceUnit.m, true, true, true);
        Assert.Null(dist);

        var distBytes = await redis.Geospatial.GeoRadiusBytesAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(2, distBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("Catania"), distBytes[0].Member);
        Assert.True(distBytes[0].Coordinate.HasValue);
        Assert.True(distBytes[1].Coordinate.HasValue);

        var countResult = await redis.Geospatial.GeoRadiusStoreAsync(storeKey, key, 16.11, 33.45, 10000, DistanceUnit.km);
        Assert.Equal(2, countResult);

        countResult = await redis.Geospatial.GeoRadiusStoreAsync(storeKey, key, 16.11, 33.45, 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = await redis.Geospatial.GeoRadiusStoreAsync(storeKey, key, 16.11, 33.45, 100, DistanceUnit.m, 1);
        Assert.Equal(0, countResult);

        countResult = await redis.Geospatial.GeoRadiusStoreDistAsync(storeKey, key, 16.11, 33.45, 10000, DistanceUnit.km);
        Assert.Equal(2, countResult);

        countResult = await redis.Geospatial.GeoRadiusStoreDistAsync(storeKey, key, 16.11, 33.45, 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = await redis.Geospatial.GeoRadiusStoreDistAsync(storeKey, key, 16.11, 33.45, 100, DistanceUnit.m, 1);
        Assert.Equal(0, countResult);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GeoRadius_Ro(Redis redis)
    {
        const string key = "geo_georadius_ro";

        _ = redis.Key.Del(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var count = redis.Geospatial.GeoAdd(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
        });
        Assert.Equal(2, count);

        var result = redis.Geospatial.GeoRadius_Ro(key, 16.11, 33.45, 100000, DistanceUnit.km);
        Assert.NotNull(result);
        Assert.Equal(2, result.Length);

        result = redis.Geospatial.GeoRadius_Ro(key, 16.11, 33.45, 100000, DistanceUnit.km, 1);
        Assert.NotNull(result);
        Assert.Single(result);

        result = redis.Geospatial.GeoRadius_Ro(key, 16.11, 33.45, 1000, DistanceUnit.m);
        Assert.Null(result);

        var resultBytes = redis.Geospatial.GeoRadius_RoBytes(key, 16.11, 33.45, 100000, DistanceUnit.km);
        Assert.NotNull(resultBytes);
        Assert.Equal(2, resultBytes.Length);

        resultBytes = redis.Geospatial.GeoRadius_RoBytes(key, 16.11, 33.45, 1000, DistanceUnit.m);
        Assert.Null(resultBytes);

        var dist = redis.Geospatial.GeoRadius_Ro(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Catania", dist[0].Member);
        Assert.False(dist[0].Coordinate.HasValue);
        Assert.False(dist[1].Coordinate.HasValue);
        Assert.False(dist[0].Dist.HasValue);
        Assert.False(dist[1].Dist.HasValue);
        Assert.False(dist[0].Hash.HasValue);
        Assert.False(dist[1].Hash.HasValue);

        dist = redis.Geospatial.GeoRadius_Ro(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);

        dist = redis.Geospatial.GeoRadius_Ro(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Catania", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);


        dist = redis.Geospatial.GeoRadius_Ro(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);

        dist = redis.Geospatial.GeoRadius_Ro(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);

        dist = redis.Geospatial.GeoRadius_Ro(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);
        Assert.True(dist[0].Hash.HasValue);
        Assert.True(dist[1].Hash.HasValue);

        dist = redis.Geospatial.GeoRadius_Ro(key, 16.11, 33.45, 100, DistanceUnit.m, true, true, true);
        Assert.Null(dist);

        var distBytes = redis.Geospatial.GeoRadius_RoBytes(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(2, distBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("Catania"), distBytes[0].Member);
        Assert.True(distBytes[0].Coordinate.HasValue);
        Assert.True(distBytes[1].Coordinate.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GeoRadius_RoAsync(Redis redis)
    {
        const string key = "geo_georadius_ro_async";

        _ = await redis.Key.DelAsync(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var count = await redis.Geospatial.GeoAddAsync(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
        });
        Assert.Equal(2, count);

        var result = await redis.Geospatial.GeoRadius_RoAsync(key, 16.11, 33.45, 100000, DistanceUnit.km);
        Assert.NotNull(result);
        Assert.Equal(2, result.Length);

        result = await redis.Geospatial.GeoRadius_RoAsync(key, 16.11, 33.45, 100000, DistanceUnit.km, 1);
        Assert.NotNull(result);
        Assert.Single(result);

        result = await redis.Geospatial.GeoRadius_RoAsync(key, 16.11, 33.45, 1000, DistanceUnit.m);
        Assert.Null(result);

        var resultBytes = await redis.Geospatial.GeoRadius_RoBytesAsync(key, 16.11, 33.45, 100000, DistanceUnit.km);
        Assert.NotNull(resultBytes);
        Assert.Equal(2, resultBytes.Length);

        resultBytes = await redis.Geospatial.GeoRadius_RoBytesAsync(key, 16.11, 33.45, 1000, DistanceUnit.m);
        Assert.Null(resultBytes);

        var dist = await redis.Geospatial.GeoRadius_RoAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Catania", dist[0].Member);
        Assert.False(dist[0].Coordinate.HasValue);
        Assert.False(dist[1].Coordinate.HasValue);
        Assert.False(dist[0].Dist.HasValue);
        Assert.False(dist[1].Dist.HasValue);
        Assert.False(dist[0].Hash.HasValue);
        Assert.False(dist[1].Hash.HasValue);

        dist = await redis.Geospatial.GeoRadius_RoAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);

        dist = await redis.Geospatial.GeoRadius_RoAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Catania", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);


        dist = await redis.Geospatial.GeoRadius_RoAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);

        dist = await redis.Geospatial.GeoRadius_RoAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, false);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);

        dist = await redis.Geospatial.GeoRadius_RoAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal(2, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);
        Assert.True(dist[0].Hash.HasValue);
        Assert.True(dist[1].Hash.HasValue);

        dist = await redis.Geospatial.GeoRadius_RoAsync(key, 16.11, 33.45, 100, DistanceUnit.m, true, true, true);
        Assert.Null(dist);

        var distBytes = await redis.Geospatial.GeoRadius_RoBytesAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(2, distBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("Catania"), distBytes[0].Member);
        Assert.True(distBytes[0].Coordinate.HasValue);
        Assert.True(distBytes[1].Coordinate.HasValue);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GeoRadiusByMember(Redis redis)
    {
        const string key = "geo_georadiusbymember";
        const string storeKey = "geo_georadiusbymember_store";

        _ = redis.Key.Del(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var where = new CoordinateValue(16.11D, 33.45D);
        var count = redis.Geospatial.GeoAdd(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
            { "Where", where },
        });
        Assert.Equal(3, count);

        var result = redis.Geospatial.GeoRadiusByMember(key, "Where", 100000, DistanceUnit.km);
        Assert.NotNull(result);
        Assert.Equal(3, result.Length);

        result = redis.Geospatial.GeoRadiusByMember(key, "Where", 100000, DistanceUnit.km, 1);
        Assert.NotNull(result);
        Assert.Single(result);

        result = redis.Geospatial.GeoRadiusByMember(key, Encoding.UTF8.GetBytes("Where"), 1000, DistanceUnit.m);
        Assert.Equal<string[]>(result, ["Where"]);

        var resultBytes = redis.Geospatial.GeoRadiusByMemberBytes(key, "Where", 100000, DistanceUnit.km);
        Assert.NotNull(resultBytes);
        Assert.Equal(3, resultBytes.Length);

        resultBytes = redis.Geospatial.GeoRadiusByMemberBytes(key, "Where", 1000, DistanceUnit.m);
        Assert.Equal(resultBytes, [Encoding.UTF8.GetBytes("Where")]);

        var dist = redis.Geospatial.GeoRadiusByMember(key, "Where", 10000, DistanceUnit.km, false, false, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Where", dist[0].Member);
        Assert.False(dist[0].Coordinate.HasValue);
        Assert.False(dist[1].Coordinate.HasValue);
        Assert.False(dist[0].Dist.HasValue);
        Assert.False(dist[1].Dist.HasValue);
        Assert.False(dist[0].Hash.HasValue);
        Assert.False(dist[1].Hash.HasValue);

        dist = redis.Geospatial.GeoRadiusByMember(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);

        dist = redis.Geospatial.GeoRadiusByMember(key, "Where", 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Where", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);


        dist = redis.Geospatial.GeoRadiusByMember(key, "Where", 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);

        dist = redis.Geospatial.GeoRadiusByMember(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, true, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);

        dist = redis.Geospatial.GeoRadiusByMember(key, "Where", 10000, DistanceUnit.km, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);
        Assert.True(dist[0].Hash.HasValue);
        Assert.True(dist[1].Hash.HasValue);

        dist = redis.Geospatial.GeoRadiusByMember(key, "Where", 100, DistanceUnit.m, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal("Where", dist[0].Member);

        var distBytes = redis.Geospatial.GeoRadiusByMemberBytes(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(3, distBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
        Assert.True(distBytes[0].Coordinate.HasValue);
        Assert.True(distBytes[1].Coordinate.HasValue);
        Assert.True(distBytes[2].Coordinate.HasValue);

        distBytes = redis.Geospatial.GeoRadiusByMemberBytes(key, "Where", 100, DistanceUnit.m, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);

        var countResult = redis.Geospatial.GeoRadiusByMemberStore(storeKey, key, "Where", 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = redis.Geospatial.GeoRadiusByMemberStore(storeKey, key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = redis.Geospatial.GeoRadiusByMemberStore(storeKey, key, "Where", 100, DistanceUnit.m, 1);
        Assert.Equal(1, countResult);

        countResult = redis.Geospatial.GeoRadiusByMemberStoreDist(storeKey, key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = redis.Geospatial.GeoRadiusByMemberStoreDist(storeKey, key, "Where", 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = redis.Geospatial.GeoRadiusByMemberStoreDist(storeKey, key, "Where", 100, DistanceUnit.m, 1);
        Assert.Equal(1, countResult);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GeoRadiusByMemberAsync(Redis redis)
    {
        const string key = "geo_georadiusbymember_async";
        const string storeKey = "geo_georadiusbymember_store_async";

        _ = await redis.Key.DelAsync(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var where = new CoordinateValue(16.11D, 33.45D);
        var count = await redis.Geospatial.GeoAddAsync(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
            { "Where", where },
        });
        Assert.Equal(3, count);

        var result = await redis.Geospatial.GeoRadiusByMemberAsync(key, "Where", 100000, DistanceUnit.km);
        Assert.NotNull(result);
        Assert.Equal(3, result.Length);

        result = await redis.Geospatial.GeoRadiusByMemberAsync(key, "Where", 100000, DistanceUnit.km, 1);
        Assert.NotNull(result);
        Assert.Single(result);

        result = await redis.Geospatial.GeoRadiusByMemberAsync(key, Encoding.UTF8.GetBytes("Where"), 1000, DistanceUnit.m);
        Assert.Equal<string[]>(result, ["Where"]);

        var resultBytes = await redis.Geospatial.GeoRadiusByMemberBytesAsync(key, "Where", 100000, DistanceUnit.km);
        Assert.NotNull(resultBytes);
        Assert.Equal(3, resultBytes.Length);

        resultBytes = await redis.Geospatial.GeoRadiusByMemberBytesAsync(key, "Where", 1000, DistanceUnit.m);
        Assert.Equal(resultBytes, [Encoding.UTF8.GetBytes("Where")]);

        var dist = await redis.Geospatial.GeoRadiusByMemberAsync(key, "Where", 10000, DistanceUnit.km, false, false, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Where", dist[0].Member);
        Assert.False(dist[0].Coordinate.HasValue);
        Assert.False(dist[1].Coordinate.HasValue);
        Assert.False(dist[0].Dist.HasValue);
        Assert.False(dist[1].Dist.HasValue);
        Assert.False(dist[0].Hash.HasValue);
        Assert.False(dist[1].Hash.HasValue);

        dist = await redis.Geospatial.GeoRadiusByMemberAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);

        dist = await redis.Geospatial.GeoRadiusByMemberAsync(key, "Where", 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Where", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);


        dist = await redis.Geospatial.GeoRadiusByMemberAsync(key, "Where", 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);

        dist = await redis.Geospatial.GeoRadiusByMemberAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, true, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);

        dist = await redis.Geospatial.GeoRadiusByMemberAsync(key, "Where", 10000, DistanceUnit.km, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);
        Assert.True(dist[0].Hash.HasValue);
        Assert.True(dist[1].Hash.HasValue);

        dist = await redis.Geospatial.GeoRadiusByMemberAsync(key, "Where", 100, DistanceUnit.m, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal("Where", dist[0].Member);

        var distBytes = await redis.Geospatial.GeoRadiusByMemberBytesAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(3, distBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
        Assert.True(distBytes[0].Coordinate.HasValue);
        Assert.True(distBytes[1].Coordinate.HasValue);
        Assert.True(distBytes[2].Coordinate.HasValue);

        distBytes = await redis.Geospatial.GeoRadiusByMemberBytesAsync(key, "Where", 100, DistanceUnit.m, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);

        var countResult = await redis.Geospatial.GeoRadiusByMemberStoreAsync(storeKey, key, "Where", 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = await redis.Geospatial.GeoRadiusByMemberStoreAsync(storeKey, key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = await redis.Geospatial.GeoRadiusByMemberStoreAsync(storeKey, key, "Where", 100, DistanceUnit.m, 1);
        Assert.Equal(1, countResult);

        countResult = await redis.Geospatial.GeoRadiusByMemberStoreDistAsync(storeKey, key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = await redis.Geospatial.GeoRadiusByMemberStoreDistAsync(storeKey, key, "Where", 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = await redis.Geospatial.GeoRadiusByMemberStoreDistAsync(storeKey, key, "Where", 100, DistanceUnit.m, 1);
        Assert.Equal(1, countResult);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GeoRadiusByMember_Ro(Redis redis)
    {
        const string key = "geo_georadiusbymember_ro";

        _ = redis.Key.Del(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var where = new CoordinateValue(16.11D, 33.45D);
        var count = redis.Geospatial.GeoAdd(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
            { "Where", where },
        });
        Assert.Equal(3, count);

        var result = redis.Geospatial.GeoRadiusByMember_Ro(key, "Where", 100000, DistanceUnit.km);
        Assert.NotNull(result);
        Assert.Equal(3, result.Length);

        result = redis.Geospatial.GeoRadiusByMember_Ro(key, "Where", 100000, DistanceUnit.km, 1);
        Assert.NotNull(result);
        Assert.Single(result);

        result = redis.Geospatial.GeoRadiusByMember_Ro(key, Encoding.UTF8.GetBytes("Where"), 1000, DistanceUnit.m);
        Assert.Equal<string[]>(result, ["Where"]);

        var resultBytes = redis.Geospatial.GeoRadiusByMember_RoBytes(key, "Where", 100000, DistanceUnit.km);
        Assert.NotNull(resultBytes);
        Assert.Equal(3, resultBytes.Length);

        resultBytes = redis.Geospatial.GeoRadiusByMember_RoBytes(key, "Where", 1000, DistanceUnit.m);
        Assert.Equal(resultBytes, [Encoding.UTF8.GetBytes("Where")]);

        var dist = redis.Geospatial.GeoRadiusByMember_Ro(key, "Where", 10000, DistanceUnit.km, false, false, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Where", dist[0].Member);
        Assert.False(dist[0].Coordinate.HasValue);
        Assert.False(dist[1].Coordinate.HasValue);
        Assert.False(dist[0].Dist.HasValue);
        Assert.False(dist[1].Dist.HasValue);
        Assert.False(dist[0].Hash.HasValue);
        Assert.False(dist[1].Hash.HasValue);

        dist = redis.Geospatial.GeoRadiusByMember_Ro(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);

        dist = redis.Geospatial.GeoRadiusByMember_Ro(key, "Where", 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Where", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);


        dist = redis.Geospatial.GeoRadiusByMember_Ro(key, "Where", 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);

        dist = redis.Geospatial.GeoRadiusByMember_Ro(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, true, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);

        dist = redis.Geospatial.GeoRadiusByMember_Ro(key, "Where", 10000, DistanceUnit.km, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);
        Assert.True(dist[0].Hash.HasValue);
        Assert.True(dist[1].Hash.HasValue);

        dist = redis.Geospatial.GeoRadiusByMember_Ro(key, "Where", 100, DistanceUnit.m, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal("Where", dist[0].Member);

        var distBytes = redis.Geospatial.GeoRadiusByMember_RoBytes(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(3, distBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
        Assert.True(distBytes[0].Coordinate.HasValue);
        Assert.True(distBytes[1].Coordinate.HasValue);
        Assert.True(distBytes[2].Coordinate.HasValue);

        distBytes = redis.Geospatial.GeoRadiusByMember_RoBytes(key, "Where", 100, DistanceUnit.m, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GeoRadiusByMember_RoAsync(Redis redis)
    {
        const string key = "geo_georadiusbymember_ro_async";

        _ = await redis.Key.DelAsync(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var where = new CoordinateValue(16.11D, 33.45D);
        var count = await redis.Geospatial.GeoAddAsync(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
            { "Where", where },
        });
        Assert.Equal(3, count);

        var result = await redis.Geospatial.GeoRadiusByMember_RoAsync(key, "Where", 100000, DistanceUnit.km);
        Assert.NotNull(result);
        Assert.Equal(3, result.Length);

        result = await redis.Geospatial.GeoRadiusByMember_RoAsync(key, "Where", 100000, DistanceUnit.km, 1);
        Assert.NotNull(result);
        Assert.Single(result);

        result = await redis.Geospatial.GeoRadiusByMember_RoAsync(key, Encoding.UTF8.GetBytes("Where"), 1000, DistanceUnit.m);
        Assert.Equal<string[]>(result, ["Where"]);

        var resultBytes = await redis.Geospatial.GeoRadiusByMember_RoBytesAsync(key, "Where", 100000, DistanceUnit.km);
        Assert.NotNull(resultBytes);
        Assert.Equal(3, resultBytes.Length);

        resultBytes = await redis.Geospatial.GeoRadiusByMember_RoBytesAsync(key, "Where", 1000, DistanceUnit.m);
        Assert.Equal(resultBytes, [Encoding.UTF8.GetBytes("Where")]);

        var dist = await redis.Geospatial.GeoRadiusByMember_RoAsync(key, "Where", 10000, DistanceUnit.km, false, false, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Where", dist[0].Member);
        Assert.False(dist[0].Coordinate.HasValue);
        Assert.False(dist[1].Coordinate.HasValue);
        Assert.False(dist[0].Dist.HasValue);
        Assert.False(dist[1].Dist.HasValue);
        Assert.False(dist[0].Hash.HasValue);
        Assert.False(dist[1].Hash.HasValue);

        dist = await redis.Geospatial.GeoRadiusByMember_RoAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);

        dist = await redis.Geospatial.GeoRadiusByMember_RoAsync(key, "Where", 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Where", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);


        dist = await redis.Geospatial.GeoRadiusByMember_RoAsync(key, "Where", 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.Equal("Palermo", dist[0].Member);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);

        dist = await redis.Geospatial.GeoRadiusByMember_RoAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, true, false);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);

        dist = await redis.Geospatial.GeoRadiusByMember_RoAsync(key, "Where", 10000, DistanceUnit.km, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal(3, dist.Length);
        Assert.True(dist[0].Coordinate.HasValue);
        Assert.True(dist[1].Coordinate.HasValue);
        Assert.True(dist[0].Dist.HasValue);
        Assert.True(dist[1].Dist.HasValue);
        Assert.True(dist[0].Hash.HasValue);
        Assert.True(dist[1].Hash.HasValue);

        dist = await redis.Geospatial.GeoRadiusByMember_RoAsync(key, "Where", 100, DistanceUnit.m, true, true, true);
        Assert.NotNull(dist);
        Assert.Equal("Where", dist[0].Member);

        var distBytes = await redis.Geospatial.GeoRadiusByMember_RoBytesAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(3, distBytes.Length);
        Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
        Assert.True(distBytes[0].Coordinate.HasValue);
        Assert.True(distBytes[1].Coordinate.HasValue);
        Assert.True(distBytes[2].Coordinate.HasValue);

        distBytes = await redis.Geospatial.GeoRadiusByMember_RoBytesAsync(key, "Where", 100, DistanceUnit.m, true, false, false);
        Assert.NotNull(distBytes);
        Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GeoSearch(Redis redis)
    {
        const string key = "geo_geosearch";

        _ = redis.Key.Del(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var count = redis.Geospatial.GeoAdd(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
        });
        Assert.Equal(2, count);

        {
            var result = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 100000, DistanceUnit.km);
            Assert.NotNull(result);
            Assert.Equal(2, result.Length);

            result = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 10000, 10000, DistanceUnit.km);
            Assert.NotNull(result);
            Assert.Equal(2, result.Length);

            result = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 100000, DistanceUnit.km, 1);
            Assert.NotNull(result);
            Assert.Single(result);

            result = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 100000, 100000, DistanceUnit.km, 1);
            Assert.NotNull(result);
            Assert.Single(result);

            result = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 1000, DistanceUnit.m);
            Assert.Null(result);

            result = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 1000, 1000, DistanceUnit.m);
            Assert.Null(result);

            var resultBytes = redis.Geospatial.GeoSearchBytes(key, 16.11, 33.45, 100000, DistanceUnit.km);
            Assert.NotNull(resultBytes);
            Assert.Equal(2, resultBytes.Length);

            resultBytes = redis.Geospatial.GeoSearchBytes(key, 16.11, 33.45, 100000, 100000, DistanceUnit.km);
            Assert.NotNull(resultBytes);
            Assert.Equal(2, resultBytes.Length);

            resultBytes = redis.Geospatial.GeoSearchBytes(key, 16.11, 33.45, 1000, DistanceUnit.m);
            Assert.Null(resultBytes);

            resultBytes = redis.Geospatial.GeoSearchBytes(key, 16.11, 33.45, 1000, 1000, DistanceUnit.m);
            Assert.Null(resultBytes);

            var dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Catania", dist[0].Member);
            Assert.False(dist[0].Coordinate.HasValue);
            Assert.False(dist[1].Coordinate.HasValue);
            Assert.False(dist[0].Dist.HasValue);
            Assert.False(dist[1].Dist.HasValue);
            Assert.False(dist[0].Hash.HasValue);
            Assert.False(dist[1].Hash.HasValue);

            dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 10000, 10000, DistanceUnit.km, false, false, false);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Catania", dist[0].Member);
            Assert.False(dist[0].Coordinate.HasValue);
            Assert.False(dist[1].Coordinate.HasValue);
            Assert.False(dist[0].Dist.HasValue);
            Assert.False(dist[1].Dist.HasValue);
            Assert.False(dist[0].Hash.HasValue);
            Assert.False(dist[1].Hash.HasValue);

            dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);

            dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 10000, 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);

            dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Catania", dist[0].Member);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);

            dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 10000, 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Catania", dist[0].Member);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);

            dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);

            dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, false);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);

            dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, true);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);
            Assert.True(dist[0].Hash.HasValue);
            Assert.True(dist[1].Hash.HasValue);

            dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 10000, 10000, DistanceUnit.km, true, true, true);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);
            Assert.True(dist[0].Hash.HasValue);
            Assert.True(dist[1].Hash.HasValue);

            dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 100, DistanceUnit.m, true, true, true);
            Assert.Null(dist);

            dist = redis.Geospatial.GeoSearch(key, 16.11, 33.45, 100, 100, DistanceUnit.m, true, true, true);
            Assert.Null(dist);

            var distBytes = redis.Geospatial.GeoSearchBytes(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(distBytes);
            Assert.Equal(2, distBytes.Length);
            Assert.Equal(Encoding.UTF8.GetBytes("Catania"), distBytes[0].Member);
            Assert.True(distBytes[0].Coordinate.HasValue);
            Assert.True(distBytes[1].Coordinate.HasValue);
        }


        _ = redis.Key.Del(key);

        var where = new CoordinateValue(16.11D, 33.45D);
        count = redis.Geospatial.GeoAdd(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
            { "Where", where },
        });
        Assert.Equal(3, count);

        {

            var result = redis.Geospatial.GeoSearch(key, "Where", 100000, DistanceUnit.km);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);

            result = redis.Geospatial.GeoSearch(key, "Where", 100000, 100000, DistanceUnit.km);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);

            result = redis.Geospatial.GeoSearch(key, "Where", 100000, DistanceUnit.km, 1);
            Assert.NotNull(result);
            Assert.Single(result);

            result = redis.Geospatial.GeoSearch(key, "Where", 100000, 100000, DistanceUnit.km, 1);
            Assert.NotNull(result);
            Assert.Single(result);

            result = redis.Geospatial.GeoSearch(key, Encoding.UTF8.GetBytes("Where"), 1000, DistanceUnit.m);
            Assert.Equal<string[]>(result, ["Where"]);

            result = redis.Geospatial.GeoSearch(key, Encoding.UTF8.GetBytes("Where"), 1000, 1000, DistanceUnit.m);
            Assert.Equal<string[]>(result, ["Where"]);

            var resultBytes = redis.Geospatial.GeoSearchBytes(key, "Where", 100000, DistanceUnit.km);
            Assert.NotNull(resultBytes);
            Assert.Equal(3, resultBytes.Length);

            resultBytes = redis.Geospatial.GeoSearchBytes(key, "Where", 100000, 100000, DistanceUnit.km);
            Assert.NotNull(resultBytes);
            Assert.Equal(3, resultBytes.Length);

            resultBytes = redis.Geospatial.GeoSearchBytes(key, "Where", 1000, DistanceUnit.m);
            Assert.Equal(resultBytes, [Encoding.UTF8.GetBytes("Where")]);

            resultBytes = redis.Geospatial.GeoSearchBytes(key, "Where", 1000, 1000, DistanceUnit.m);
            Assert.Equal(resultBytes, [Encoding.UTF8.GetBytes("Where")]);

            var dist = redis.Geospatial.GeoSearch(key, "Where", 10000, DistanceUnit.km, false, false, false);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Where", dist[0].Member);
            Assert.False(dist[0].Coordinate.HasValue);
            Assert.False(dist[1].Coordinate.HasValue);
            Assert.False(dist[0].Dist.HasValue);
            Assert.False(dist[1].Dist.HasValue);
            Assert.False(dist[0].Hash.HasValue);
            Assert.False(dist[1].Hash.HasValue);

            dist = redis.Geospatial.GeoSearch(key, "Where", 10000, 10000, DistanceUnit.km, false, false, false);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Where", dist[0].Member);
            Assert.False(dist[0].Coordinate.HasValue);
            Assert.False(dist[1].Coordinate.HasValue);
            Assert.False(dist[0].Dist.HasValue);
            Assert.False(dist[1].Dist.HasValue);
            Assert.False(dist[0].Hash.HasValue);
            Assert.False(dist[1].Hash.HasValue);

            dist = redis.Geospatial.GeoSearch(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);

            dist = redis.Geospatial.GeoSearch(key, Encoding.UTF8.GetBytes("Where"), 10000, 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);

            dist = redis.Geospatial.GeoSearch(key, "Where", 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Where", dist[0].Member);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);


            dist = redis.Geospatial.GeoSearch(key, "Where", 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);

            dist = redis.Geospatial.GeoSearch(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, true, false);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);

            dist = redis.Geospatial.GeoSearch(key, Encoding.UTF8.GetBytes("Where"), 10000, 10000, DistanceUnit.km, true, true, false);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);

            dist = redis.Geospatial.GeoSearch(key, "Where", 10000, DistanceUnit.km, true, true, true);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);
            Assert.True(dist[0].Hash.HasValue);
            Assert.True(dist[1].Hash.HasValue);

            dist = redis.Geospatial.GeoSearch(key, "Where", 100, DistanceUnit.m, true, true, true);
            Assert.NotNull(dist);
            Assert.Equal("Where", dist[0].Member);

            var distBytes = redis.Geospatial.GeoSearchBytes(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(distBytes);
            Assert.Equal(3, distBytes.Length);
            Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
            Assert.True(distBytes[0].Coordinate.HasValue);
            Assert.True(distBytes[1].Coordinate.HasValue);
            Assert.True(distBytes[2].Coordinate.HasValue);

            distBytes = redis.Geospatial.GeoSearchBytes(key, Encoding.UTF8.GetBytes("Where"), 10000, 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(distBytes);
            Assert.Equal(3, distBytes.Length);
            Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
            Assert.True(distBytes[0].Coordinate.HasValue);
            Assert.True(distBytes[1].Coordinate.HasValue);
            Assert.True(distBytes[2].Coordinate.HasValue);

            distBytes = redis.Geospatial.GeoSearchBytes(key, "Where", 100, DistanceUnit.m, true, false, false);
            Assert.NotNull(distBytes);
            Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
        }
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GeoSearchAsync(Redis redis)
    {
        const string key = "geo_geosearch_async";

        _ = await redis.Key.DelAsync(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var count = await redis.Geospatial.GeoAddAsync(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
        });
        Assert.Equal(2, count);

        {
            var result = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 100000, DistanceUnit.km);
            Assert.NotNull(result);
            Assert.Equal(2, result.Length);

            result = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 10000, 10000, DistanceUnit.km);
            Assert.NotNull(result);
            Assert.Equal(2, result.Length);

            result = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 100000, DistanceUnit.km, 1);
            Assert.NotNull(result);
            Assert.Single(result);

            result = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 100000, 100000, DistanceUnit.km, 1);
            Assert.NotNull(result);
            Assert.Single(result);

            result = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 1000, DistanceUnit.m);
            Assert.Null(result);

            result = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 1000, 1000, DistanceUnit.m);
            Assert.Null(result);

            var resultBytes = await redis.Geospatial.GeoSearchBytesAsync(key, 16.11, 33.45, 100000, DistanceUnit.km);
            Assert.NotNull(resultBytes);
            Assert.Equal(2, resultBytes.Length);

            resultBytes = await redis.Geospatial.GeoSearchBytesAsync(key, 16.11, 33.45, 100000, 100000, DistanceUnit.km);
            Assert.NotNull(resultBytes);
            Assert.Equal(2, resultBytes.Length);

            resultBytes = await redis.Geospatial.GeoSearchBytesAsync(key, 16.11, 33.45, 1000, DistanceUnit.m);
            Assert.Null(resultBytes);

            resultBytes = await redis.Geospatial.GeoSearchBytesAsync(key, 16.11, 33.45, 1000, 1000, DistanceUnit.m);
            Assert.Null(resultBytes);

            var dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Catania", dist[0].Member);
            Assert.False(dist[0].Coordinate.HasValue);
            Assert.False(dist[1].Coordinate.HasValue);
            Assert.False(dist[0].Dist.HasValue);
            Assert.False(dist[1].Dist.HasValue);
            Assert.False(dist[0].Hash.HasValue);
            Assert.False(dist[1].Hash.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 10000, 10000, DistanceUnit.km, false, false, false);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Catania", dist[0].Member);
            Assert.False(dist[0].Coordinate.HasValue);
            Assert.False(dist[1].Coordinate.HasValue);
            Assert.False(dist[0].Dist.HasValue);
            Assert.False(dist[1].Dist.HasValue);
            Assert.False(dist[0].Hash.HasValue);
            Assert.False(dist[1].Hash.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);

            dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 10000, 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);

            dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Catania", dist[0].Member);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 10000, 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Catania", dist[0].Member);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, false);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, true, true);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);
            Assert.True(dist[0].Hash.HasValue);
            Assert.True(dist[1].Hash.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 10000, 10000, DistanceUnit.km, true, true, true);
            Assert.NotNull(dist);
            Assert.Equal(2, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);
            Assert.True(dist[0].Hash.HasValue);
            Assert.True(dist[1].Hash.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 100, DistanceUnit.m, true, true, true);
            Assert.Null(dist);

            dist = await redis.Geospatial.GeoSearchAsync(key, 16.11, 33.45, 100, 100, DistanceUnit.m, true, true, true);
            Assert.Null(dist);

            var distBytes = await redis.Geospatial.GeoSearchBytesAsync(key, 16.11, 33.45, 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(distBytes);
            Assert.Equal(2, distBytes.Length);
            Assert.Equal(Encoding.UTF8.GetBytes("Catania"), distBytes[0].Member);
            Assert.True(distBytes[0].Coordinate.HasValue);
            Assert.True(distBytes[1].Coordinate.HasValue);
        }


        _ = await redis.Key.DelAsync(key);

        var where = new CoordinateValue(16.11D, 33.45D);
        count = await redis.Geospatial.GeoAddAsync(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
            { "Where", where },
        });
        Assert.Equal(3, count);

        {

            var result = await redis.Geospatial.GeoSearchAsync(key, "Where", 100000, DistanceUnit.km);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);

            result = await redis.Geospatial.GeoSearchAsync(key, "Where", 100000, 100000, DistanceUnit.km);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);

            result = await redis.Geospatial.GeoSearchAsync(key, "Where", 100000, DistanceUnit.km, 1);
            Assert.NotNull(result);
            Assert.Single(result);

            result = await redis.Geospatial.GeoSearchAsync(key, "Where", 100000, 100000, DistanceUnit.km, 1);
            Assert.NotNull(result);
            Assert.Single(result);

            result = await redis.Geospatial.GeoSearchAsync(key, Encoding.UTF8.GetBytes("Where"), 1000, DistanceUnit.m);
            Assert.Equal<string[]>(result, ["Where"]);

            result = await redis.Geospatial.GeoSearchAsync(key, Encoding.UTF8.GetBytes("Where"), 1000, 1000, DistanceUnit.m);
            Assert.Equal<string[]>(result, ["Where"]);

            var resultBytes = await redis.Geospatial.GeoSearchBytesAsync(key, "Where", 100000, DistanceUnit.km);
            Assert.NotNull(resultBytes);
            Assert.Equal(3, resultBytes.Length);

            resultBytes = await redis.Geospatial.GeoSearchBytesAsync(key, "Where", 100000, 100000, DistanceUnit.km);
            Assert.NotNull(resultBytes);
            Assert.Equal(3, resultBytes.Length);

            resultBytes = await redis.Geospatial.GeoSearchBytesAsync(key, "Where", 1000, DistanceUnit.m);
            Assert.Equal(resultBytes, [Encoding.UTF8.GetBytes("Where")]);

            resultBytes = await redis.Geospatial.GeoSearchBytesAsync(key, "Where", 1000, 1000, DistanceUnit.m);
            Assert.Equal(resultBytes, [Encoding.UTF8.GetBytes("Where")]);

            var dist = await redis.Geospatial.GeoSearchAsync(key, "Where", 10000, DistanceUnit.km, false, false, false);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Where", dist[0].Member);
            Assert.False(dist[0].Coordinate.HasValue);
            Assert.False(dist[1].Coordinate.HasValue);
            Assert.False(dist[0].Dist.HasValue);
            Assert.False(dist[1].Dist.HasValue);
            Assert.False(dist[0].Hash.HasValue);
            Assert.False(dist[1].Hash.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, "Where", 10000, 10000, DistanceUnit.km, false, false, false);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Where", dist[0].Member);
            Assert.False(dist[0].Coordinate.HasValue);
            Assert.False(dist[1].Coordinate.HasValue);
            Assert.False(dist[0].Dist.HasValue);
            Assert.False(dist[1].Dist.HasValue);
            Assert.False(dist[0].Hash.HasValue);
            Assert.False(dist[1].Hash.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);

            dist = await redis.Geospatial.GeoSearchAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, 10000, DistanceUnit.km, false, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);

            dist = await redis.Geospatial.GeoSearchAsync(key, "Where", 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Where", dist[0].Member);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);


            dist = await redis.Geospatial.GeoSearchAsync(key, "Where", 10000, DistanceUnit.km, true, false, false, sortord: OrderType.Desc);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.Equal("Palermo", dist[0].Member);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, true, false);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, 10000, DistanceUnit.km, true, true, false);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, "Where", 10000, DistanceUnit.km, true, true, true);
            Assert.NotNull(dist);
            Assert.Equal(3, dist.Length);
            Assert.True(dist[0].Coordinate.HasValue);
            Assert.True(dist[1].Coordinate.HasValue);
            Assert.True(dist[0].Dist.HasValue);
            Assert.True(dist[1].Dist.HasValue);
            Assert.True(dist[0].Hash.HasValue);
            Assert.True(dist[1].Hash.HasValue);

            dist = await redis.Geospatial.GeoSearchAsync(key, "Where", 100, DistanceUnit.m, true, true, true);
            Assert.NotNull(dist);
            Assert.Equal("Where", dist[0].Member);

            var distBytes = await redis.Geospatial.GeoSearchBytesAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(distBytes);
            Assert.Equal(3, distBytes.Length);
            Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
            Assert.True(distBytes[0].Coordinate.HasValue);
            Assert.True(distBytes[1].Coordinate.HasValue);
            Assert.True(distBytes[2].Coordinate.HasValue);

            distBytes = await redis.Geospatial.GeoSearchBytesAsync(key, Encoding.UTF8.GetBytes("Where"), 10000, 10000, DistanceUnit.km, true, false, false);
            Assert.NotNull(distBytes);
            Assert.Equal(3, distBytes.Length);
            Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
            Assert.True(distBytes[0].Coordinate.HasValue);
            Assert.True(distBytes[1].Coordinate.HasValue);
            Assert.True(distBytes[2].Coordinate.HasValue);

            distBytes = await redis.Geospatial.GeoSearchBytesAsync(key, "Where", 100, DistanceUnit.m, true, false, false);
            Assert.NotNull(distBytes);
            Assert.Equal(Encoding.UTF8.GetBytes("Where"), distBytes[0].Member);
        }
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void GeoSearchStore(Redis redis)
    {
        const string key = "geo_geosearchstore";
        const string storeKey = "geo_geosearchstore_store";

        _ = redis.Key.Del(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var where = new CoordinateValue(16.11D, 33.45D);
        var count = redis.Geospatial.GeoAdd(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
            { "Where", where },
        });
        Assert.Equal(3, count);

        var countResult = redis.Geospatial.GeoSearchStore(storeKey, key, "Where", 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = redis.Geospatial.GeoSearchStore(storeKey, key, "Where", 10000, 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = redis.Geospatial.GeoSearchStore(storeKey, key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = redis.Geospatial.GeoSearchStore(storeKey, key, "Where", 100, DistanceUnit.m, 1);
        Assert.Equal(1, countResult);

        countResult = redis.Geospatial.GeoSearchStore(storeKey, key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = redis.Geospatial.GeoSearchStore(storeKey, key, Encoding.UTF8.GetBytes("Where"), 10000, 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = redis.Geospatial.GeoSearchStore(storeKey, key, "Where", 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = redis.Geospatial.GeoSearchStore(storeKey, key, "Where", 100, DistanceUnit.m, 1);
        Assert.Equal(1, countResult);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task GeoSearchStoreAsync(Redis redis)
    {
        const string key = "geo_geosearchstore_async";
        const string storeKey = "geo_geosearchstore_store_async";

        _ = await redis.Key.DelAsync(key);

        var palermo = new CoordinateValue(13.361389D, 38.115556D);
        var catania = new CoordinateValue(15.087269D, 37.502669D);
        var where = new CoordinateValue(16.11D, 33.45D);
        var count = await redis.Geospatial.GeoAddAsync(key, new Dictionary<string, CoordinateValue>
        {
            { "Palermo", palermo },
            { "Catania", catania },
            { "Where", where },
        });
        Assert.Equal(3, count);

        var countResult = await redis.Geospatial.GeoSearchStoreAsync(storeKey, key, "Where", 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = await redis.Geospatial.GeoSearchStoreAsync(storeKey, key, "Where", 10000, 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = await redis.Geospatial.GeoSearchStoreAsync(storeKey, key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = await redis.Geospatial.GeoSearchStoreAsync(storeKey, key, "Where", 100, DistanceUnit.m, 1);
        Assert.Equal(1, countResult);

        countResult = await redis.Geospatial.GeoSearchStoreAsync(storeKey, key, Encoding.UTF8.GetBytes("Where"), 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = await redis.Geospatial.GeoSearchStoreAsync(storeKey, key, Encoding.UTF8.GetBytes("Where"), 10000, 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = await redis.Geospatial.GeoSearchStoreAsync(storeKey, key, "Where".AsSpan(), 10000, 10000, DistanceUnit.km);
        Assert.Equal(3, countResult);

        countResult = await redis.Geospatial.GeoSearchStoreAsync(storeKey, key, "Where", 10000, DistanceUnit.km, 1);
        Assert.Equal(1, countResult);

        countResult = await redis.Geospatial.GeoSearchStoreAsync(storeKey, key, "Where", 100, DistanceUnit.m, 1);
        Assert.Equal(1, countResult);
    }
}
