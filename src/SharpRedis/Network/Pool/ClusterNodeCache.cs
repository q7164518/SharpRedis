using SharpRedis.Provider.Standard;

namespace SharpRedis.Network.Pool;

// Each master has its own REDIRECT client id; cache contents and invalidations are shared.
internal sealed class ClusterNodeCache : ClientCachingBase
{
    private readonly ClientCachingBase _shared;
    internal ClusterNodeCache(ClientCachingBase shared) => this._shared = shared;
    protected internal override ClientCachingMode Mode => this._shared.Mode;
#if MODERN_NET
    protected internal override string[]? KeyPrefixes => this._shared.KeyPrefixes;
#else
    protected internal override string[] KeyPrefixes => this._shared.KeyPrefixes;
#endif
    protected internal override bool KeyMatching(string key) => this._shared.KeyMatching(key);
    protected internal override void Set(string key, in RedisResult value) => this._shared.Set(key, in value);
    protected internal override bool TryGet(string key,
#if MODERN_NET
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
        out RedisResult? value) => this._shared.TryGet(key, out value);
    protected internal override void Remove(string key) => this._shared.Remove(key);
    protected internal override void Clear() => this._shared.ClearCache();
}
