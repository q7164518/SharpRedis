using System;

namespace SharpRedis.Exceptions;

/// <summary>
/// Represents that the command execution has timed out. 
/// <para>This exception does not indicate that the Redis service failed to execute the command; it merely means that SharpRedis will no longer wait for the Redis server to return the result.</para>
/// <para>表示命令执行超时异常. 此异常不一定代表Redis服务未成功执行命令, 只表示SharpRedis不再等待Redis服务返回的结果</para>
/// </summary>
public sealed class RedisTimeoutException : RedisException
{
    internal RedisTimeoutException(
        Exception innerException,
        string command,
        string msg,
        long clientId,
        string connectionName,
        string host,
        int port,
        int databaseIndex
    ) : base(
        innerException: innerException,
        command: command,
        msg: msg,
        clientId: clientId,
        connectionName: connectionName,
        host: host,
        port: port,
        databaseIndex: databaseIndex
    )
    {
    }

    public override string ToString()
    {
        return $"Command [{this.Command}] execution timed out, {base.Message}. Host: {this.Host}, Port: {this.Port}";
    }
}
