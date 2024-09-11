#pragma warning disable IDE0130
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#nullable enable
#endif
#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
#if !LOW_NET
using System.Threading.Tasks;
#endif
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SharpRedis
{
    /// <summary>
    /// Client-side caching standard
    /// <para>SharpRedis itself is not responsible for the local cache logic</para>
    /// <para>If you need to use Redis client-side caching, implement this abstract class to provide local caching</para>
    /// <para>客户端本地缓存标准抽象类</para>
    /// <para>SharpRedis本身不负责存储本地缓存</para>
    /// <para>如果你需要使用Redis的客户端缓存请根据自身情况实现该抽象类, 以提供本地缓存功能</para>
    /// </summary>
    public abstract class ClientSideCachingStandard : IDisposable
    {
        internal long RedirectConnectionId { get; set; } = -1;
        private ConcurrentDictionary<ClientSideCacheKey, bool> _redisKeys = new ConcurrentDictionary<ClientSideCacheKey, bool>();
        private ConcurrentDictionary<string, Dictionary<ClientSideCacheKey, bool>> _keyMapperCacheKeys = new ConcurrentDictionary<string, Dictionary<ClientSideCacheKey, bool>>();
        private bool _disposedValue;

        /// <summary>
        /// Redis client cache mode
        /// <para><see href="https://redis.io/docs/manual/client-side-caching/">Click Me to view the official Redis documentation</see></para>
        /// <para>The Broadcasting mode is recommended</para>
        /// <para>Redis客户端缓存模式</para>
        /// <para><see href="https://redis.io/docs/manual/client-side-caching/">详细信息点我查看Redis官方文档</see></para>
        /// <para>推荐使用广播模式</para>
        /// </summary>
        public abstract ClientSideCachingMode Mode { get; }

        /// <summary>
        /// For default mode, the Key pattern to be tracking by the client. If this parameter is not set, all keys will be tracking. Supported glob-style patterns:
        /// <para>h?llo tracking to hello, hallo and hxllo</para>
        /// <para>h*llo tracking to hllo and heeeello</para>
        /// <para>h[ae]llo tracking to hello and hallo, but not hillo</para>
        /// <para>默认模式下, 要跟踪缓存Key的匹配模式, *表示一个或多个任意字符, ?表示任意一个字符, [abc]表示abc其中一个, 如果不设置, 所有的Key都会被跟踪缓存</para>
        /// <para>如h?llo会缓存hello, hallo, hxllo.</para>
        /// <para>如h*llo会缓存hello, hallo, hxxxxxxxxxxxxxxxxllo.</para>
        /// <para>如h[abc]llo会缓存hallo, hbllo, hcllo, 其它的不缓存.</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public abstract string[]? KeyPatterns { get; }
#else
        public abstract string[] KeyPatterns { get; }
#endif

        /// <summary>
        /// For default mode, the client excludes the Key mode of the tracking. If this parameter is not set, all keys will be tracking. Supported glob-style patterns:
        /// <para>h?llo excludes tracking to hello, hallo and hxllo</para>
        /// <para>h*llo excludes tracking to hllo and heeeello</para>
        /// <para>h[ae]llo excludes tracking to hello and hallo</para>
        /// <para>The priority is lower than KeyPattern</para>
        /// <para>默认模式下, 不踪缓存Key的匹配模式, *表示一个或多个任意字符, ?表示任意一个字符, [abc]表示abc其中一个, 如果不设置, 所有的Key都会被跟踪缓存</para>
        /// <para>如h?llo不缓存hello, hallo, hxllo. 其它的都缓存</para>
        /// <para>如h*llo不缓存hello, hallo, hxxxxxxxxxxxxxxxxllo. 其它的都缓存</para>
        /// <para>如h[abc]llo不缓存hallo, hbllo, hcllo, 其它的都缓存.</para>
        /// <para>优先级低于KeyPattern</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public abstract string[]? WithoutKeyPatterns { get; }
#else
        public abstract string[] WithoutKeyPatterns { get; }
#endif

        /// <summary>
        /// For broadcasting mode, register a given key prefix, so that notifications will be provided only for keys starting with this string. This option can be given multiple times to register multiple prefixes.
        /// <para>If broadcasting is enabled without this option, Redis will send notifications for every key.</para>
        /// <para>广播模式下, 要跟踪缓存的Key前缀集合, 可以设置多个前缀. 只有Key是这其中之一开头才有可能被客户端缓存</para>
        /// <para>如果不设置, 所有的Key都有可能被客户端缓存</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public abstract string[]? KeyPrefixes { get; }
#else
        public abstract string[] KeyPrefixes { get; }
#endif

        internal void WholeSet(in ClientSideCacheKey key, object value)
        {
            if (!this._redisKeys.ContainsKey(key) && this._redisKeys.TryAdd(key, true))
            {
                if (!this.Set(in key, value))
                {
                    this._redisKeys.TryRemove(key, out _);
                }
                else
                {
                    foreach (var redisKey in key.GetKeys())
                    {
                        if (this._keyMapperCacheKeys.TryGetValue(redisKey, out var mapperCacheKeys))
                        {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                            mapperCacheKeys.TryAdd(key, true);
#else
                            if (!mapperCacheKeys.ContainsKey(key))
                            {
                                mapperCacheKeys[key] = true;
                            }
#endif
                        }
                        else
                        {
                            this._keyMapperCacheKeys.TryAdd(redisKey, new Dictionary<ClientSideCacheKey, bool>
                            {
                                { key, true }
                            });
                        }
                    }
                }
            }
        }

        internal void WholeDelete(string key)
        {
            if (this._keyMapperCacheKeys.TryRemove(key, out var cacheKeys))
            {
                foreach (var cacheKey in cacheKeys.Keys)
                {
                    this._redisKeys.TryRemove(cacheKey, out _);
                    this.Delete(in cacheKey);
                }
            }
        }

        internal void WholeClear()
        {
            this.Clear();
            this._redisKeys.Clear();
            this._keyMapperCacheKeys.Clear();
        }

#if LOW_NET
        internal void ClientSideCacheInvalidate(string channel, object data)
#else
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal Task ClientSideCacheInvalidate(string channel, object? data)
#else
        internal Task ClientSideCacheInvalidate(string channel, object data)
#endif
#endif
        {
            if (data is null)
            {
                this.WholeClear();
            }
            else if (data is object[] keys && keys.Length > 0)
            {
                foreach (var key in keys)
                {
                    if (key is string sKey)
                    {
                        this.WholeDelete(sKey);
                    }
                }
            }
            else if (data is string key)
            {
                this.WholeDelete(key);
            }
#if !LOW_NET
#if NET46_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            return Task.CompletedTask;
#elif NET45
            return Task.Delay(0);
#elif NET40
            return TaskEx.Delay(0);
#endif
#endif
        }

        /// <summary>
        /// Clear all cache keys
        /// <para>清除所有缓存的Key</para>
        /// </summary>
        /// <returns></returns>
        protected internal abstract bool Clear();

        /// <summary>
        /// Set a cache
        /// <para>设置一个缓存</para>
        /// </summary>
        /// <param name="key">Key<para>缓存的Key</para></param>
        /// <param name="value">Value<para>缓存的值</para></param>
        /// <returns>Whether the cache is set successfully
        /// <para>是否设置成功</para>
        /// </returns>
        protected internal abstract bool Set(in ClientSideCacheKey key, object value);

        /// <summary>
        /// Try to get the value of a cache key
        /// <para>根据Key尝试获取缓存值</para>
        /// </summary>
        /// <param name="key">Key<para>缓存的Key</para></param>
        /// <param name="value">Value<para>缓存的值</para></param>
        /// <returns>Whether to obtain success</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        protected internal abstract bool TryGet(in ClientSideCacheKey key, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out object? value);
#else
        protected internal abstract bool TryGet(in ClientSideCacheKey key, out object value);
#endif

        /// <summary>
        /// Delete a cache
        /// <para>根据Key删除一个缓存</para>
        /// </summary>
        /// <param name="key">Cache Key to delete<para>要删除的缓存Key</para></param>
        /// <returns></returns>
        protected internal abstract bool Delete(in ClientSideCacheKey key);

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS8625
#endif
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                this._disposedValue = true;
                if (disposing)
                {
                    this.WholeClear();
                }

                this._redisKeys = null;
                this._keyMapperCacheKeys = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~ClientSideCachingStandard()
        {
            this.Dispose(disposing: true);
        }
    }
}
