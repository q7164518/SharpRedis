namespace SharpRedis.Exceptions;

/// <summary>
/// Represents that an error occurred when the Redis server was executing a command.
/// <para>表示Redis服务端执行命令出现了错误</para>
/// </summary>
public sealed class RedisErrorException : RedisException
{
    /// <summary>
    /// Gets the error type of Redis
    /// <para>获得Redis错误类型</para>
    /// </summary>
    public RedisErrorType ErrorType { get; }

    internal RedisErrorException(
        string command,
        string msg,
        RedisErrorType errorType,
        long clientId,
        string connectionName,
        string host,
        int port,
        int databaseIndex
    ) : base(
        innerException: null,
        command: command,
        msg: msg,
        clientId: clientId,
        connectionName: connectionName,
        host: host,
        port: port,
        databaseIndex: databaseIndex
    )
    {
        this.ErrorType = errorType;
    }

    public override string ToString()
    {
        return $"Command: [{this.Command}], {base.Message}. Host: {this.Host}, Port: {this.Port}";
    }

    /// <summary>
    /// The error types returned by Redis
    /// <para>Redis返回的错误类型</para>
    /// </summary>
    public enum RedisErrorType : sbyte
    {
        /// <summary>
        /// Bulk errors
        /// </summary>
        BulkErrors = 33,

        /// <summary>
        /// Simple Errors
        /// </summary>
        SimpleErrors = 45,
    }
}
