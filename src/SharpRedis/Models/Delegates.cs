#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#nullable enable
#endif
#if !LOW_NET
using System.Threading.Tasks;
#endif

namespace SharpRedis
{
    #region Sync NET3.0 NET3.5
#if LOW_NET
    /// <summary>
    /// Receive subscription event
    /// <para>收到订阅消息事件</para>
    /// </summary>
    /// <param name="channel">Subscribed channel
    /// <para>订阅的通道名称</para>
    /// </param>
    /// <param name="data">Data received
    /// <para>收到的数据</para>
    /// </param>
    public delegate void OnReceive(string channel, object data);

    /// <summary>
    /// Receive subscription event
    /// <para>收到订阅消息事件</para>
    /// </summary>
    /// <param name="pattern">Subscribes the client to the given patterns
    /// <para>订阅的模式</para>
    /// </param>
    /// <param name="channel">Subscribed channel
    /// <para>订阅的通道名称</para>
    /// </param>
    /// <param name="data">Data received
    /// <para>收到的数据</para>
    /// </param>
    public delegate void POnReceive(string pattern, string channel, object data);
#endif
    #endregion

    #region Async
#if !LOW_NET
    /// <summary>
    /// Receive subscription event
    /// <para>收到订阅消息异步事件</para>
    /// </summary>
    /// <param name="channel">Subscribed channel
    /// <para>订阅的通道名称</para>
    /// </param>
    /// <param name="data">Data received
    /// <para>收到的数据</para>
    /// </param>
    public delegate Task OnReceive(string channel, object data);

    /// <summary>
    /// Receive subscription event
    /// <para>收到订阅消息异步事件</para>
    /// </summary>
    /// <param name="pattern">Subscribes the client to the given patterns.</param>
    /// <param name="channel">Subscribed channel
    /// <para>订阅的通道名称</para>
    /// </param>
    /// <param name="data">
    /// Data received
    /// <para>收到的数据</para>
    /// </param>
    /// <returns></returns>
    public delegate Task POnReceive(string pattern, string channel, object data);
#endif
    #endregion
}
