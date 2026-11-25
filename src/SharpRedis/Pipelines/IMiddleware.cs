using SharpRedis.Entities.Command;
using SharpRedis.Network;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Pipelines;

internal interface IMiddleware : IDisposable
{
    string Name { get; }

    RedisResult Invoke(ICommand command, BaseConnection connection, RedisDelegate next, CancellationToken cancellationToken);

    Task<RedisResult> InvokeAsync(ICommand command, BaseConnection connection, AsyncRedisDelegate next, CancellationToken cancellationToken);
}
