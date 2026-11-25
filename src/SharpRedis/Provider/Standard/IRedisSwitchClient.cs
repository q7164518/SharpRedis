namespace SharpRedis.Provider.Standard;

/// <summary>
/// After changing the data index, the operation interface
/// <para>切换数据库后的操作接口</para>
/// </summary>
public interface IRedisSwitchClient : IRedis, IRedisPipelining
{
}
