#nullable enable
using SharpRedis.Entities.Command;
using SharpRedis.Pipelines.Middlewares.ClientCaching;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace SharpRedis.Provider.Standard;

internal abstract class DefaultClientCaching(ClientCachingBase.ClientCachingMode mode, string[]? keyPrefixes, Func<string, bool>? matching) : ClientCachingBase
{
    protected internal override ClientCachingMode Mode => mode;

    protected internal override string[]? KeyPrefixes => keyPrefixes;

    protected internal override bool KeyMatching(string key) => matching?.Invoke(key) ?? false;

    internal static DefaultClientCaching Create(ClientCachingMode mode, string[]? keyPrefixes, Func<string, bool>? matching, TimeSpan? expiration)
    {
        if (!expiration.HasValue) return new Permanent(mode, keyPrefixes, matching);
        if (expiration.Value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(expiration));
        return new Expiring(mode, keyPrefixes, matching, expiration.Value);
    }

    private sealed class Permanent(ClientCachingMode mode, string[]? keyPrefixes, Func<string, bool>? matching)
        : DefaultClientCaching(mode, keyPrefixes, matching)
    {
        private readonly ConcurrentDictionary<string, RedisResult> _caches = new();

        protected internal override bool TryGet(string command,
#if MODERN_NET
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            out RedisResult? value)
        {
            if (this._caches.TryGetValue(command, out var cached))
            {
                value = cached;
                return true;
            }
            value = null;
            return false;
        }

        protected internal override void Set(string command, in RedisResult value) => this._caches[command] = value;

        protected internal override void Remove(string command) => this._caches.TryRemove(command, out _);

        protected internal override void Clear() => this._caches.Clear();
    }

    private sealed class Expiring : DefaultClientCaching
    {
        private readonly ConcurrentDictionary<string, (RedisResult Value, long WrittenAt)> _caches = new();
        private readonly Stopwatch _clock = Stopwatch.StartNew();
        private readonly long _lifetime;
        private readonly Timer _timer;
        private bool _disposed;

        internal Expiring(ClientCachingMode mode, string[]? keyPrefixes, Func<string, bool>? matching, TimeSpan expiration)
            : base(mode, keyPrefixes, matching)
        {
            this._lifetime = expiration.Ticks;
            int interval = (int)Math.Min(60000, Math.Max(1000, expiration.TotalMilliseconds));
            // Unused connection options can be collected even before a client owns them.
            this._timer = new Timer(static state =>
            {
                if (((WeakReference<Expiring>)state!).TryGetTarget(out var cache)) cache.Sweep();
            }, new WeakReference<Expiring>(this), interval, interval);
        }

        protected internal override bool TryGet(string command,
#if MODERN_NET
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            out RedisResult? value)
        {
            if (this._caches.TryGetValue(command, out var entry) && this._clock.Elapsed.Ticks - entry.WrittenAt < this._lifetime)
            {
                value = entry.Value;
                return true;
            }
            value = null;
            return false;
        }

        internal override void SetCache(ICommand command, string cacheKey, in RedisResult value, CacheKeyInvoker cacheKeyInvoker)
        {
            // Publish the reverse mappings and the value together with respect to pruning.
            lock (this._caches)
            {
                if (!this._disposed) base.SetCache(command, cacheKey, in value, cacheKeyInvoker);
            }
        }

        protected internal override void Set(string command, in RedisResult value)
            => this._caches[command] = (value, this._clock.Elapsed.Ticks);

        protected internal override void Remove(string command) => this._caches.TryRemove(command, out _);

        protected internal override void Clear() => this._caches.Clear();

        private void Sweep()
        {
            lock (this._caches)
            {
                if (this._disposed) return;
                long now = this._clock.Elapsed.Ticks;
                foreach (var entry in this._caches)
                {
                    if (now - entry.Value.WrittenAt >= this._lifetime) this.Remove(entry.Key);
                }
                this.PruneMappings(this._caches.ContainsKey);
            }
        }

        protected override void Dispose(bool disposing)
        {
            lock (this._caches)
            {
                if (this._disposed) return;
                this._disposed = true;
                this._timer.Dispose();
                base.Dispose(disposing);
            }
        }
    }
}
