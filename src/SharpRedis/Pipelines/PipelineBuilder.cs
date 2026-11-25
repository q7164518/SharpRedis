using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpRedis.Pipelines;

internal sealed class PipelineBuilder : IDisposable
{
    private List<IMiddleware> _middlewares = [];
    private bool _disposedValue;

    internal TMiddleware Use<TMiddleware>(TMiddleware middleware)
        where TMiddleware : IMiddleware
    {
        this._middlewares.Add(middleware);
        return middleware;
    }

    internal (AsyncRedisDelegate, RedisDelegate) Build()
    {
        AsyncRedisDelegate asyncPipeline = static (_, _, _) => Task.FromResult(RedisResult.Empty);

        RedisDelegate syncPipeline = static (_, _, _) => RedisResult.Empty;

        for (int i = this._middlewares.Count - 1; i >= 0; i--)
        {
            var middleware = this._middlewares[i];

            var nextAsync = asyncPipeline;
            var nextSync = syncPipeline;

            asyncPipeline = (command, connection, cancellationToken)
                => middleware.InvokeAsync(
                    command,
                    connection,
                    nextAsync,
                    cancellationToken);

            syncPipeline = (command, connection, cancellationToken)
                => middleware.Invoke(
                    command,
                    connection,
                    nextSync,
                    cancellationToken);
        }

        return (asyncPipeline, syncPipeline);
    }

    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            this._disposedValue = true;
            if (disposing)
            {
                this._middlewares.Clear();
            }
            this._middlewares = null!;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
