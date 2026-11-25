namespace SharpRedis;

/// <summary>
/// Expiry processing method marking enums
/// <para>有效时间处理方式枚举</para>
/// </summary>
public enum ExpirationFlags : byte
{
    /// <summary>
    /// Seconds, Only use the TimeSpan type or numeric types
    /// <para>秒</para>
    /// </summary>
    EX = 0,

    /// <summary>
    /// Milliseconds
    /// <para>毫秒</para>
    /// </summary>
    PX,

    /// <summary>
    /// unix-time-seconds
    /// <para>时间戳, 秒级</para>
    /// </summary>
    EXAT,

    /// <summary>
    /// Unix-time-milliseconds
    /// <para>时间戳, 毫秒级</para>
    /// </summary>
    PXAT,

    /// <summary>
    /// Retain the TTL associated with the fields.
    /// <para>If this value is used, the expiry parameter of the setting will become invalid.</para>
    /// <para>保留有效期, 如果使用此值, 设置的有效期参数将无效</para>
    /// </summary>
    KEEPTTL,

    /// <summary>
    /// Remove the time to live associated, Set to never expire
    /// <para>If this value is used, the expiry parameter of the setting will become invalid.</para>
    /// <para>去掉过期时间, 设为永不过期, 如果使用此值, 设置的有效期参数将无效</para>
    /// </summary>
    PERSIST,
}
