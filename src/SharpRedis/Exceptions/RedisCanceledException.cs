using System;
using System.Threading;

namespace SharpRedis.Exceptions;

/// <summary>
/// An exception that is triggered when a Redis operation is cancelled
/// <para>一个Redis操作被取消时引发的异常</para>
/// </summary>
public sealed class RedisCanceledException : OperationCanceledException
{
    /// <summary>
    /// Gets the command that caused an execution error. Does not include command parameters
    /// <para>获得执行错误的命令。不包含命令参数</para>
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// Gets the connection ID for executing the command
    /// <para>获得执行命令的连接ID</para>
    /// </summary>
    public long ClientId { get; }

    /// <summary>
    /// Gets the connection name for executing the command
    /// <para>获得执行命令的连接名称</para>
    /// </summary>
    public string ConnectionName { get; }

    /// <summary>
    /// Gets the Host address of the Redis service that has executed the command
    /// <para>获得执行命令的Redis服务的Host地址</para>
    /// </summary>
    public string Host { get; }

    /// <summary>
    /// Gets the Host port of the Redis service that has executed the command
    /// <para>获得执行命令的Redis服务的Host端口</para>
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Gets the database index for executing the command
    /// <para>获得执行命令的数据库</para>
    /// </summary>
    public int DataBaseInedx { get; }

    internal RedisCanceledException(
#if MODERN_NET
        string? message, Exception? innerException,
#else
        string message, Exception innerException,
#endif
        string command,
        long clientId,
        string connectionName,
        string host,
        int port,
        int databaseIndex,
        CancellationToken token)
        : base(message, innerException, token)
    {
        this.Command = command;
        this.ClientId = clientId;
        this.ConnectionName = connectionName;
        this.Host = host;
        this.Port = port;
        this.DataBaseInedx = databaseIndex;
    }
}
