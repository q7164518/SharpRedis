using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Pipelines.Middlewares.ClientCaching;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SharpRedis.Provider.Standard;

/// <summary>
/// Client-side caching standard
/// <para>Implementing this abstract class enables the implementation of custom client-side caching logic</para>
/// <para>客户端本地缓存标准抽象类</para>
/// <para>实现此抽象类, 可实现自定义客户端缓存逻辑</para>
/// </summary>
public abstract class ClientCachingBase : IDisposable
{
    private bool _disposedValue;
    private ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> _redisKeyMapperCacheKey = new();

    internal long RedirectConnectionId { get; set; }

    /// <summary>
    /// Specify the Redis Client-side caching mode, The Broadcasting mode is recommended
    /// <para>指定Redis客户端缓存模式, 推荐使用广播模式</para>
    /// </summary>
    protected internal abstract ClientCachingMode Mode { get; }

    /// <summary>
    /// For broadcasting mode, register a given key prefix, so that notifications will be provided only for keys starting with this string. This option can be given multiple times to register multiple prefixes.
    /// <para>If broadcasting is enabled without this option, Redis will send notifications for every key.</para>
    /// <para>广播模式下, 要跟踪缓存的Key前缀集合, 可以设置多个前缀. 只有Key是这其中之一开头才有可能被客户端缓存</para>
    /// <para>如果不设置, 所有的Key都有可能被客户端缓存</para>
    /// </summary>
#if MODERN_NET
    protected internal abstract string[]? KeyPrefixes { get; }
#else
    protected internal abstract string[] KeyPrefixes { get; }
#endif

    /// <summary>
    /// By default, it checks whether a Key should be tracked and cached on the client side.
    /// <para>默认模式下, 检测一个Key是否应该被跟踪并缓存到客户端</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Returning true indicates that it should be tracked, otherwise it will not be tracked.
    /// <para>返回true表示应该被跟踪缓存, 否则不跟踪缓存</para>
    /// </returns>
    protected internal abstract bool KeyMatching(string key);

    /// <summary>
    /// According to the executed command, store the cache locally
    /// <para>根据执行的命令, 设置缓存到本地</para>
    /// </summary>
    /// <param name="cacheKey">The executed read-only Redis commands, used as the keys for caching
    /// <para>当前执行的Redis命令, 用作缓存的Key</para>
    /// </param>
    /// <param name="value">The return value of the command
    /// <para>命令的返回值</para>
    /// </param>
    protected internal abstract void Set(string cacheKey, in RedisResult value);

    /// <summary>
    /// Depending on the command to be executed, attempt to retrieve the cache from the local system.
    /// <para>根据要执行的命令, 尝试从本地获取缓存</para>
    /// </summary>
    /// <param name="cacheKey">The executed read-only Redis commands, used as the keys for caching
    /// <para>当前执行的Redis命令, 用作缓存的Key</para>
    /// </param>
    /// <param name="value">The value associated with the given key.
    /// <para>获取到的缓存值</para>
    /// </param>
    /// <returns>Whether the operation is successful or not. If it returns true, SharpRedis will not send the command to Redis for execution.
    /// <para>是否获取成功, 如果返回true, SharpRedis将立即返回结果给调用者, 且不会将当前命令发送到Redis服务执行</para>
    /// </returns>
#if MODERN_NET
    protected internal abstract bool TryGet(string cacheKey, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out RedisResult? value);
#else
    protected internal abstract bool TryGet(string cacheKey, out RedisResult? value);
#endif

    /// <summary>
    /// Remove the cache according to the command. When the Redis service notifies that the cache has become invalid, this method will be executed.
    /// <para>根据命令移除缓存. 当Redis服务通知缓存失效时, 会执行该方法</para>
    /// </summary>
    /// <param name="cacheKey">command</param>
    protected internal abstract void Remove(string cacheKey);

    /// <summary>
    /// Clear all caches
    /// <para>清除所有缓存</para>
    /// </summary>
    protected internal abstract void Clear();

    #region Internal methods
    // Routing and cache lookup must use the same full-key matching rules.
    internal bool MatchesKeys(ICommand command, CacheKeyInvoker keyInvoker)
    {
        bool hasKey = false;
        foreach (var item in command.Items)
        {
            if (item.Category != CommandItemCategory.Key) continue;
            hasKey = true;
            var key = keyInvoker.FullKey(in item);
            if (this.Mode == ClientCachingMode.Default)
            {
                if (!this.KeyMatching(key)) return false;
            }
            else if (this.KeyPrefixes is { Length: > 0 } prefixes)
            {
                bool matched = false;
                foreach (var prefix in prefixes)
                {
                    if (key.StartsWith(prefix, StringComparison.Ordinal))
                    {
                        matched = true;
                        break;
                    }
                }
                if (!matched) return false;
            }
        }
        return hasKey;
    }

    internal virtual void SetCache(ICommand command, string cacheKey, in RedisResult value, CacheKeyInvoker cacheKeyInvoker)
    {
        foreach (var item in command.Items)
        {
            if (item.Category is CommandItemCategory.Key)
            {
                var redisKey = cacheKeyInvoker.FullKey(in item);
                var mapper = this._redisKeyMapperCacheKey.GetOrAdd(redisKey, static _ => new ConcurrentDictionary<string, bool>());
                _ = mapper.GetOrAdd(cacheKey, true);
            }
        }

        this.Set(cacheKey, in value);
    }

    // The expiring cache serializes this pass with SetCache so a new mapping cannot be pruned mid-fill.
    internal void PruneMappings(Func<string, bool> contains)
    {
        foreach (var mapping in this._redisKeyMapperCacheKey)
        {
            foreach (var command in mapping.Value)
            {
                if (!contains(command.Key)) mapping.Value.TryRemove(command.Key, out _);
            }
            if (mapping.Value.IsEmpty) this._redisKeyMapperCacheKey.TryRemove(mapping.Key, out _);
        }
    }

    internal void RemoveCache(string redisKey)
    {
        if (this._redisKeyMapperCacheKey.TryRemove(redisKey, out var mapper))
        {
            foreach (var command in mapper.Keys)
            {
                this.Remove(command);
            }
            mapper.Clear();
        }
    }

    internal void ClearCache()
    {
        this.Clear();
        foreach (var item in this._redisKeyMapperCacheKey)
        {
            item.Value.Clear();
        }
        this._redisKeyMapperCacheKey.Clear();
    }

    internal Task ClientCacheInvalidate(string channel, RedisResult data)
    {
        if(data.RawValue is DBNull)
        {
            // clear all cache
            this.ClearCache();
            return Task.CompletedTask;
        }
        if (data.RawValue is not RedisResult[] redisKeys) return Task.CompletedTask;

#if MODERN_NET
        foreach (string? redisKey in redisKeys)
#else
        foreach (string redisKey in redisKeys)
#endif
        {
            if (redisKey is not null)
                this.RemoveCache(redisKey);
        }

        return Task.CompletedTask;
    }
    #endregion

    #region Dispose
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            this._disposedValue = true;
            if (disposing)
            {
                this.Clear();
                if (this._redisKeyMapperCacheKey != null)
                {
                    foreach (var item in this._redisKeyMapperCacheKey)
                    {
                        item.Value.Clear();
                    }
                }
                this._redisKeyMapperCacheKey?.Clear();
            }

            this._redisKeyMapperCacheKey = null!;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    /// <summary>
    /// Redis client cache mode
    /// <para><see href="https://redis.io/docs/latest/develop/reference/client-side-caching">Click Me to view the official Redis documentation</see></para>
    /// <para>The Broadcasting mode is recommended</para>
    /// <para>Redis客户端缓存模式</para>
    /// <para><see href="https://redis.io/docs/latest/develop/reference/client-side-caching">详细信息点我查看Redis官方文档</see></para>
    /// <para>推荐使用广播模式</para>
    /// </summary>
    public enum ClientCachingMode : byte
    {
        /// <summary>
        /// In the default mode, the server remembers what keys a given client accessed, and sends invalidation messages when the same keys are modified.
        /// <para>This costs memory in the server side, but sends invalidation messages only for the set of keys that the client might have in memory.</para>
        /// <para><see href="https://redis.io/docs/latest/develop/reference/client-side-caching/#what-tracking-tracks">Click Me to view the official Redis documentation</see></para>
        /// <para>默认模式, 服务端记住给定客户端访问的密钥，并在修改相同密钥时发送无效消息</para>
        /// <para>这会消耗服务器端的内存，但只会为客户端内存中可能存在的Key发送无效消息。</para>
        /// <see href="https://redis.io/docs/latest/develop/reference/client-side-caching/#what-tracking-tracks">详细信息点我查看Redis官方文档</see>
        /// </summary>
        Default,

        /// <summary>
        /// Broadcasting mode
        /// <para><see href="https://redis.io/docs/latest/develop/reference/client-side-caching/#broadcasting-mode">Click Me to view the official Redis documentation</see></para>
        /// <para>广播模式</para>
        /// <para><see href="https://redis.io/docs/latest/develop/reference/client-side-caching/#broadcasting-mode">详细信息点我查看Redis官方文档</see></para>
        /// </summary>
        Broadcast,
    }
}
