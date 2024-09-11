using Microsoft.Extensions.Caching.Memory;
using SharpRedis;
using System.Diagnostics.CodeAnalysis;

namespace Example_NET8;

sealed public class LocalCache : ClientSideCachingStandard
{
    private readonly MemoryCache _cache;

    public LocalCache()
    {
        this._cache = new MemoryCache(new MemoryCacheOptions { });
    }

    public override ClientSideCachingMode Mode => ClientSideCachingMode.Default;

    public override string[]? KeyPatterns => ["localcache_test*"];

    public override string[]? WithoutKeyPatterns => ["nocache*"];

    public override string[]? KeyPrefixes => ["localcache_test:"];

    protected override bool Clear()
    {
        this._cache.Clear();
        return true;
    }

    protected override bool Delete(in ClientSideCacheKey key)
    {
        this._cache.Remove(key);
        return true;
    }

    protected override bool Set(in ClientSideCacheKey key, object value)
    {
        this._cache.Set(key, value);
        return true;
    }

    protected override bool TryGet(in ClientSideCacheKey key, [NotNullWhen(true)] out object? value)
    {
        return this._cache.TryGetValue(key, out value);
    }
}
