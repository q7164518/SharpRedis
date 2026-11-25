using System.Threading.Tasks;

namespace SharpRedis;

/// <summary>
/// Receive subscription event
/// <para>收到订阅消息异步事件</para>
/// </summary>
/// <param name="channel">Subscribed channel
/// <para>订阅的频道名称</para>
/// </param>
/// <param name="data">Data received
/// <para>收到的数据</para>
/// </param>
public delegate Task OnReceive(string channel, RedisResult data);

/// <summary>
/// Receive subscription event
/// <para>收到订阅消息异步事件</para>
/// </summary>
/// <param name="pattern">Subscribes the client to the given patterns.</param>
/// <param name="channel">Subscribed channel
/// <para>订阅的频道名称</para>
/// </param>
/// <param name="data">
/// Data received
/// <para>收到的数据</para>
/// </param>
/// <returns></returns>
public delegate Task POnReceive(string pattern, string channel, RedisResult data);
