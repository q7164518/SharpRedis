using SharpRedis.Provider.Standard;
using System;

namespace SharpRedis.Modules;

/// <summary>
/// Redis extension module provides classes.
/// <para><b>ℹ️By using the corresponding extension module, you need to enable the corresponding module or use Redis version 8.0 or above. Otherwise, the operation will result in an exception.</b></para>
/// <para>Redis扩展模块提供类</para>
/// <para><b>ℹ️使用对应的扩展模块, 需要按照启用对应的模块, 或者使用Redis 8.0及以上版本, 否则操作都会引发异常</b></para>
/// </summary>
public sealed class RedisModules : IDisposable
{
    private bool _disposedValue;
    private RedisBloom _bloom;
    private RedisCuckoo _cuckoo;
    private RedisCountMinSketch _sketch;
    private RedisSearch _search;

    /// <summary>
    /// Gets the operation property of the Bloom filter.
    /// <para><b>ℹ️Note: The RedisBloom extension module is required, or a version of Redis 8.0 or above must be used. Otherwise, any command operation under this attribute will result in an exception.</b></para>
    /// <para>获得布隆过滤器操作属性</para>
    /// <para><b>ℹ️提示: 需要有RedisBloom扩展模块, 或者Redis8.0以上版本, 否则该属性下的命令操作都会引发异常</b></para>
    /// </summary>
    public RedisBloom Bloom => this._disposedValue ? throw new ObjectDisposedException(nameof(RedisModules)) : this._bloom;

    /// <summary>
    /// Gets the operation property of the Cuckoo filter.
    /// <para><b>ℹ️Note: The RedisBloom extension module is required, or a version of Redis 8.0 or above must be used. Otherwise, any command operation under this attribute will result in an exception.</b></para>
    /// <para>获得布谷鸟过滤器操作属性</para>
    /// <para><b>ℹ️提示: 需要有RedisBloom扩展模块, 或者Redis8.0以上版本, 否则该属性下的命令操作都会引发异常</b></para>
    /// </summary>
    public RedisCuckoo Cuckoo => this._disposedValue ? throw new ObjectDisposedException(nameof(RedisModules)) : this._cuckoo;

    /// <summary>
    /// Gets the operation property of the Count-min sketch.
    /// <para><b>ℹ️Note: The RedisBloom 2.0.0 extension module is required, or a version of Redis 8.0 or above must be used. Otherwise, any command operation under this attribute will result in an exception.</b></para>
    /// <para>获得Count-min sketch操作属性</para>
    /// <para><b>ℹ️提示: 需要有RedisBloom 2.0.0+ 扩展模块, 或者Redis8.0以上版本, 否则该属性下的命令操作都会引发异常</b></para>
    /// </summary>
    public RedisCountMinSketch Sketch => this._disposedValue ? throw new ObjectDisposedException(nameof(RedisModules)) : this._sketch;

    /// <summary>
    /// Gets the operation property of the RedisSearch.
    /// <para><b>ℹ️Note: The RedisSearch 1.0.0 extension module is required, or a version of Redis 8.0 or above must be used. Otherwise, any command operation under this attribute will result in an exception.</b></para>
    /// <para>获得RedisSearch操作属性</para>
    /// <para><b>ℹ️提示: 需要有RedisSearch 1.0.0+ 扩展模块, 或者Redis8.0以上版本, 否则该属性下的命令操作都会引发异常</b></para>
    /// </summary>
    public RedisSearch Search => this._disposedValue ? throw new ObjectDisposedException(nameof(RedisModules)) : this._search;

    internal RedisModules(BaseRedis redis)
    {
        this._bloom = new(redis);
        this._cuckoo = new(redis);
        this._sketch = new(redis);
        this._search = new(redis);
    }

    #region Dispose
    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            this._disposedValue = true;
            if (disposing)
            {
                this._bloom?.Dispose();
                this._cuckoo?.Dispose();
                this._sketch?.Dispose();
                this._search?.Dispose();
            }
            this._bloom = null!;
            this._cuckoo = null!;
            this._sketch = null!;
            this._search = null!;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
