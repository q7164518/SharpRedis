using SharpRedis.Enums;
using System;
using System.Runtime.InteropServices;

namespace SharpRedis;

/// <summary>
/// Expiry Options. Implicit conversions can be performed using the five types: long, int, TimeSpan, DateTime, and DateTimeOffset.
/// <para>有效期结构. 可以使用long, int, TimeSpan, DateTime, DateTimeOffset以上这五个类型进行隐式转换</para>
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct SharpExpiry : IEquatable<SharpExpiry>
{
    [FieldOffset(0)]
    private readonly ExpirationValueKind _valueKind;

    [FieldOffset(8)]
    private readonly ulong _number;

    [FieldOffset(8)]
    private readonly DateTimeOffset _time;

    [FieldOffset(8)]
    private readonly TimeSpan _span;

    internal readonly ExpirationValueKind ValueKind => this._valueKind;

    /// <summary>
    /// Obtain valid time in seconds
    /// <para>获得有效时间以秒为单位</para>
    /// </summary>
    public ulong GetSeconds =>
        this._valueKind switch
        {
            ExpirationValueKind.TimeSpan => (ulong)this._span.TotalSeconds,
            ExpirationValueKind.DateTime => (ulong)(this._time - DateTimeOffset.UtcNow).TotalSeconds,
            ExpirationValueKind.Number => this._number,
            _ => throw new InvalidOperationException(),
        };

    /// <summary>
    /// Obtain valid time in milliseconds
    /// <para>获得有效时间以毫秒为单位</para>
    /// </summary>
    public ulong GetMilliseconds =>
        this._valueKind switch
        {
            ExpirationValueKind.TimeSpan => (ulong)this._span.TotalMilliseconds,
            ExpirationValueKind.DateTime => (ulong)(this._time - DateTimeOffset.UtcNow).TotalMilliseconds,
            ExpirationValueKind.Number => this._number,
            _ => throw new InvalidOperationException(),
        };

    /// <summary>
    /// Obtain the timestamp of the absolute expiration time in seconds
    /// <para>获得绝对到期时间的时间戳以秒为单位</para>
    /// </summary>
    public ulong GetUnixTimeSeconds =>
        this._valueKind switch
        {
            ExpirationValueKind.TimeSpan => (ulong)DateTimeOffset.UtcNow.AddMilliseconds(this._span.TotalMilliseconds).ToUnixTimeSeconds(),
            ExpirationValueKind.DateTime => (ulong)this._time.ToUnixTimeSeconds(),
            ExpirationValueKind.Number => this._number,
            _ => throw new InvalidOperationException(),
        };

    /// <summary>
    /// Obtain the timestamp of the absolute expiration time in milliseconds
    /// <para>获得绝对到期时间的时间戳以毫秒为单位</para>
    /// </summary>
    public ulong GetUnixTimeMilliseconds =>
        this._valueKind switch
        {
            ExpirationValueKind.TimeSpan => (ulong)DateTimeOffset.UtcNow.AddMilliseconds(this._span.TotalMilliseconds).ToUnixTimeMilliseconds(),
            ExpirationValueKind.DateTime => (ulong)this._time.ToUnixTimeMilliseconds(),
            ExpirationValueKind.Number => this._number,
            _ => throw new InvalidOperationException(),
        };

    /// <summary>
    /// Created based on TimeSpan
    /// <para>根据TimeSpan创建</para>
    /// </summary>
    /// <param name="expiry">expiry</param>
    public SharpExpiry(TimeSpan expiry)
    {
        this._span = expiry;
        this._valueKind = ExpirationValueKind.TimeSpan;
    }

    /// <summary>
    /// Created based on DateTimeOffset or DateTime
    /// <para>根据DateTimeOffset或DateTime创建</para>
    /// </summary>
    /// <param name="expiry">expiry</param>
    public SharpExpiry(DateTimeOffset expiry)
    {
        this._time = expiry.ToUniversalTime();
        this._valueKind = ExpirationValueKind.DateTime;
    }

    /// <summary>
    /// Created based on long or int
    /// <para>根据long或int创建</para>
    /// </summary>
    /// <param name="expiry">It can accept the number of seconds, milliseconds and timestamp.
    /// <para>可以传入秒数, 毫秒数和时间戳</para>
    /// </param>
    public SharpExpiry(long expiry) : this((ulong)expiry)
    { }

    /// <summary>
    /// Created based on long or int
    /// <para>根据long或int创建</para>
    /// </summary>
    /// <param name="expiry">It can accept the number of seconds, milliseconds and timestamp.
    /// <para>可以传入秒数, 毫秒数和时间戳</para>
    /// </param>
    public SharpExpiry(ulong expiry)
    {
        this._number = expiry;
        this._valueKind = ExpirationValueKind.Number;
    }

#if MODERN_NET
    public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
#else
    public override bool Equals(object obj)
#endif
    {
        if (obj is null) return false;
        if (obj is not SharpExpiry se) return false;
        return se == this;
    }

    public override int GetHashCode()
    {
        return this._valueKind switch
        {
            ExpirationValueKind.TimeSpan => this._span.GetHashCode(),
            ExpirationValueKind.DateTime => this._time.GetHashCode(),
            ExpirationValueKind.Number => this._number.GetHashCode(),
            _ => -1,
        };
    }

    public bool Equals(SharpExpiry other)
    {
        if (other._valueKind != this._valueKind) return false;
        return this._valueKind switch
        {
            ExpirationValueKind.TimeSpan => this._span == other._span,
            ExpirationValueKind.DateTime => this._time == other._time,
            ExpirationValueKind.Number => this._number == other._number,
            _ => false,
        };
    }

    public static implicit operator SharpExpiry(TimeSpan expiry)
        => new(expiry);

    public static implicit operator SharpExpiry(DateTimeOffset expiry)
        => new(expiry);

    public static implicit operator SharpExpiry(DateTime expiry)
        => new(expiry);

    public static implicit operator SharpExpiry(int expiry)
        => new(expiry);

    public static implicit operator SharpExpiry(uint expiry)
        => new(expiry);

    public static implicit operator SharpExpiry(long expiry)
        => new(expiry);

    public static implicit operator SharpExpiry(ulong expiry)
        => new(expiry);

    public static bool operator ==(SharpExpiry left, SharpExpiry right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SharpExpiry left, SharpExpiry right)
    {
        return !(left == right);
    }
}
