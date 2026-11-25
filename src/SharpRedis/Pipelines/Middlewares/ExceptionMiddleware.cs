using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Exceptions;
using SharpRedis.Extensions;
using SharpRedis.Network;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Pipelines.Middlewares;

internal sealed class ExceptionMiddleware : IMiddleware
{
    public const string NAME = "Exception";
    private bool _disposedValue;

    public string Name => ExceptionMiddleware.NAME;

    internal ExceptionMiddleware()
    {
    }

    RedisResult IMiddleware.Invoke(ICommand command, BaseConnection connection, RedisDelegate next, CancellationToken cancellationToken)
    {
        try
        {
            var result = next.Invoke(command, connection, cancellationToken);
            return ExceptionMiddleware.IfException(connection, ref result, command, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            return ExceptionMiddleware.CanceledException(connection, command, ex, cancellationToken);
        }
        catch (TimeoutException ex)
        {
            return ExceptionMiddleware.TimeoutException(connection, command, ex);
        }
        catch (Exception ex)
        {
            return ExceptionMiddleware.Exception(connection, command, ex);
        }
    }

    async Task<RedisResult> IMiddleware.InvokeAsync(ICommand command, BaseConnection connection, AsyncRedisDelegate next, CancellationToken cancellationToken)
    {
        try
        {
            var result = await next.Invoke(command, connection, cancellationToken).ConfigureAwait(false);
            return ExceptionMiddleware.IfException(connection, ref result, command, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            return ExceptionMiddleware.CanceledException(connection, command, ex, cancellationToken);
        }
        catch (TimeoutException ex)
        {
            return ExceptionMiddleware.TimeoutException(connection, command, ex);
        }
        catch (Exception ex)
        {
            return ExceptionMiddleware.Exception(connection, command, ex);
        }
    }

    private static RedisResult IfException(BaseConnection connection, ref readonly RedisResult result, ICommand command, CancellationToken cancellationToken)
    {
        if (result.Type is not RespType.BulkErrors and not RespType.SimpleErrors and not RespType.Exception)
        {
            return result;
        }

        // A successful SHUTDOWN closes its connection without sending a RESP reply.
        if (result.RawValue is System.IO.EndOfStreamException && !command.IsBatch
            && (command.Mode & CommandMode.Shutdown) != 0)
            return RedisResult.Empty;

        if (result.RawValue is OperationCanceledException cancelex)
        {
            return ExceptionMiddleware.CanceledException(connection, command, cancelex, cancellationToken);
        }
        else if (result.RawValue is TimeoutException timeoutex)
        {
            return ExceptionMiddleware.TimeoutException(connection, command, timeoutex);
        }
        else
        {
            string commandString = command.GetCommandString(connection.Encoding);
            var ex = new RedisErrorException(
                command: commandString,
                msg: result.RawValue.ToString() ?? Config.UNKNOWN_ERROR_MSG,
                errorType: (RedisErrorException.RedisErrorType)(sbyte)result.Type,
                clientId: connection.ClientId,
                connectionName: connection.ConnectionName,
                host: connection.Host,
                port: connection.Port,
                databaseIndex: command.DataBaseIndex
            );
            return new(result.Type, ex, connection.Encoding);
        }
    }

    private static RedisResult TimeoutException(BaseConnection connection, ICommand command, Exception ex)
    {
        string commandString = command.GetCommandString(connection.Encoding);
        var resultEx = new RedisTimeoutException(
            innerException: ex,
            command: commandString,
            msg: $"Command [{commandString}] execution timed out.",
            clientId: connection.ClientId,
            connectionName: connection.ConnectionName,
            host: connection.Host,
            port: connection.Port,
            databaseIndex: command.DataBaseIndex
        );
        return new(RespType.Exception, resultEx, connection.Encoding);
    }

    private static RedisResult CanceledException(BaseConnection connection, ICommand command, Exception ex, CancellationToken cancellationToken)
    {
        string commandString = command.GetCommandString(connection.Encoding);
        var resultEx = new RedisCanceledException(
            message: $"The command [{commandString}] operation was canceled.",
            innerException: ex,
            command: commandString,
            clientId: connection.ClientId,
            connectionName: connection.ConnectionName,
            host: connection.Host,
            port: connection.Port,
            databaseIndex: command.DataBaseIndex,
            token: cancellationToken
        );
        return new(RespType.Exception, resultEx, connection.Encoding);
    }

    private static RedisResult Exception(BaseConnection connection, ICommand command, Exception ex)
    {
        string commandString = command.GetCommandString(connection.Encoding);
        var resultEx = new RedisException(
            innerException: ex,
            command: commandString,
            msg: $"An exception occurred while executing the command [{commandString}], Msg: {ex.Message}",
            clientId: connection.ClientId,
            connectionName: connection.ConnectionName,
            host: connection.Host,
            port: connection.Port,
            databaseIndex: command.DataBaseIndex
        );
        return new(RespType.Exception, resultEx, connection.Encoding);
    }

    #region Dispose
    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            this._disposedValue = true;
            if (disposing)
            {
            }
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
