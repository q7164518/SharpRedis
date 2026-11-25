namespace SharpRedis.Provider.Standard;

/// <summary>
/// Indicates that this client supports pipes
/// <para>表示当前客户端支持管道功能</para>
/// </summary>
public interface IRedisPipelining
{
    IRedisPipeliningClient CreatePipeliningRedis();
}
