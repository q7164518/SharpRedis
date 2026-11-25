using SharpRedis.Entities.Command;
using SharpRedis.Network;
using SharpRedis.Provider.Standard;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Pipelines.Middlewares;

internal static class CommandDisposeMiddleware
{
    internal static RedisResult Invoke(ICommand command, BaseConnection connection, RedisDelegate next, CancellationToken cancellationToken)
    {
        try
        {
            return next.Invoke(command, connection, cancellationToken);
        }
        finally
        {
            command.Dispose();
        }
    }

    internal static async Task<RedisResult> InvokeAsync(ICommand command, BaseConnection connection, AsyncRedisDelegate next, CancellationToken cancellationToken, bool checkException = false)
    {
        try
        {
            var result = await next.Invoke(command, connection, cancellationToken).ConfigureAwait(false);
            if (checkException) BaseRedis.CheckException(ref result);
            return result;
        }
        finally
        {
            command.Dispose();
        }
    }
}
