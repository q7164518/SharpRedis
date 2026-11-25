namespace SharpRedis;

/// <summary>
/// Some commands can specify synchronous and asynchronous operations.
/// <para>某些命令可指定同步和异步操作</para>
/// </summary>
public enum AsyncSync : byte
{
    /// <summary>
    /// No specification
    /// <para>不指定</para>
    /// </summary>
    None = 0,

    /// <summary>
    /// asynchronously
    /// <para>异步的</para>
    /// </summary>
    Async = 1,

    /// <summary>
    /// synchronously
    /// <para>同步的</para>
    /// </summary>
    Sync = 2,
}
