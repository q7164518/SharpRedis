#if !NET6_0_OR_GREATER
using SharpRedis.Extensions;
#endif
using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Network;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Pipelines.Middlewares;

internal sealed class ExecuteCommandMiddleware : IMiddleware
{
    public const string NAME = "ExecuteCommand";
    private Encoding _encoding;
    private readonly int _millisecondsTimeout;
    private bool _disposedValue;

    public string Name => ExecuteCommandMiddleware.NAME;

    internal ExecuteCommandMiddleware(RedisContext context)
    {
        this._encoding = context.RedisOptions.Encoding;
        this._millisecondsTimeout = context.RedisOptions.CommandTimeout;
    }

    public RedisResult Invoke(ICommand command, BaseConnection connection, RedisDelegate next, CancellationToken cancellationToken)
    {
        int timeout = this.GetTimeout(command);
        command.CancellationToken = cancellationToken;
        command.TimeoutAt = timeout == Timeout.Infinite ? DateTime.MaxValue : DateTime.UtcNow.AddMilliseconds(timeout);
        TaskCompletionSource<RedisResult> execution = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, execution);
        try
        {
            if (execution.Task.Wait(timeout, cancellationToken))
            {
                return execution.Task.GetAwaiter().GetResult();
            }
            return new(RespType.Exception, new TimeoutException(), this._encoding);
        }
        catch (OperationCanceledException ex)
        {
            return new(RespType.Exception, ex, this._encoding);
        }
    }

    public Task<RedisResult> InvokeAsync(ICommand command, BaseConnection connection, AsyncRedisDelegate next, CancellationToken cancellationToken)
    {
        int timeout = this.GetTimeout(command);
        command.CancellationToken = cancellationToken;
        command.TimeoutAt = timeout == Timeout.Infinite ? DateTime.MaxValue : DateTime.UtcNow.AddMilliseconds(timeout);
        TaskCompletionSource<RedisResult> execution = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, execution);
        if (timeout == Timeout.Infinite && !cancellationToken.CanBeCanceled) return execution.Task;
        return execution.Task.WaitAsync(TimeSpan.FromMilliseconds(timeout), cancellationToken);
    }

    private int GetTimeout(ICommand command)
        => (command.Mode & CommandMode.WithBlocking) == CommandMode.WithBlocking
            ? Timeout.Infinite
            : this._millisecondsTimeout;

    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            this._disposedValue = true;
            if (disposing)
            {
            }
            this._encoding = null!;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
