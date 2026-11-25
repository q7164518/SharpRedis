using SharpRedis;

namespace NET10.ClientCaching;

internal class CacheExpirationTests
{
    [TestCase("all")]
    [TestCase("prefix")]
    [TestCase("matching")]
    public async Task CachedValueExpiresAndCanBeRefilled(string mode)
    {
        var connection = Environment.GetEnvironmentVariable("SHARPREDIS_CACHE_TEST_CONNECTION");
        if (string.IsNullOrEmpty(connection)) Assert.Ignore("Set SHARPREDIS_CACHE_TEST_CONNECTION to a Redis 6+ instance.");
        var key = "cache-expiration:" + Guid.NewGuid().ToString("N");
        var ttl = TimeSpan.FromMilliseconds(300);
        using var writer = Redis.UseStandalone(connection!);
        writer.String.Set(key, "value", 60);
        try
        {
            using var reader = Redis.UseStandalone(connection!, options =>
            {
                switch (mode)
                {
                    case "prefix": options.UseClientCaching([key], ttl); break;
                    case "matching": options.UseClientCaching(k => k == key, ttl); break;
                }
            });
            var first = reader.String.Get(key, CommandFlags.ReturnBytes).RawValue;
            Assert.That(first, Is.TypeOf<byte[]>());
            Assert.That((await reader.String.GetAsync(key, CommandFlags.ReturnBytes)).RawValue, Is.SameAs(first));
            await Task.Delay(ttl + TimeSpan.FromMilliseconds(50));
            var refreshed = (await reader.String.GetAsync(key, CommandFlags.ReturnBytes)).RawValue;
            Assert.That(refreshed, Is.EqualTo(first));
            Assert.That(refreshed, Is.Not.SameAs(first));
            Assert.That(reader.String.Get(key, CommandFlags.ReturnBytes).RawValue, Is.SameAs(refreshed));
        }
        finally
        {
            writer.Key.Del(key);
        }
    }
}
