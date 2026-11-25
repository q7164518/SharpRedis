using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Network;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Pipelines.Middlewares.SelectConnection;

internal sealed class StandaloneSelectConnectionMiddleware : IMiddleware
{
    public const string NAME = "StandaloneSelectConnection";

    private RedisContext _context;
    private bool _disposedValue;
    private const CommandMode _exceptiveCommandMode =
        CommandMode.WithBlocking
        | CommandMode.Sub
        | CommandMode.PSub;
    public string Name => StandaloneSelectConnectionMiddleware.NAME;

    internal StandaloneSelectConnectionMiddleware(RedisContext context)
    {
        this._context = context;
    }

    RedisResult IMiddleware.Invoke(ICommand command, BaseConnection connection, RedisDelegate next, CancellationToken cancellationToken)
    {
        BaseConnection execConnection = this.GetConnection(command, connection);
        if (connection is null && execConnection is TempConnection temporary)
        {
            bool reusable = false;
            try
            {
                bool canReuse = StandaloneSelectConnectionMiddleware.CanReuseConnection(command);
                var result = next.Invoke(command, temporary, cancellationToken);
                reusable = canReuse && StandaloneSelectConnectionMiddleware.IsSuccessful(result);
                return result;
            }
            finally
            {
                temporary.Return(reusable);
            }
        }
        return next.Invoke(command, execConnection, cancellationToken);
    }

    Task<RedisResult> IMiddleware.InvokeAsync(ICommand command, BaseConnection connection, AsyncRedisDelegate next, CancellationToken cancellationToken)
    {
        BaseConnection execConnection = this.GetConnection(command, connection);
        if (connection is null && execConnection is TempConnection temporary)
        {
            return InvokeTemporaryAsync(command, temporary, next, cancellationToken);
        }
        return next.Invoke(command, execConnection, cancellationToken);
    }

    private static async Task<RedisResult> InvokeTemporaryAsync(
        ICommand command, TempConnection connection, AsyncRedisDelegate next, CancellationToken cancellationToken)
    {
        bool reusable = false;
        try
        {
            bool canReuse = StandaloneSelectConnectionMiddleware.CanReuseConnection(command);
            var result = await next.Invoke(command, connection, cancellationToken).ConfigureAwait(false);
            reusable = canReuse && StandaloneSelectConnectionMiddleware.IsSuccessful(result);
            return result;
        }
        finally
        {
            connection.Return(reusable);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanReuseConnection(ICommand command)
        => (command.Mode & CommandMode.DoNotReuseConnection) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSuccessful(RedisResult result)
        => result.Type is not (RespType.Exception or RespType.SimpleErrors or RespType.BulkErrors);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private BaseConnection GetConnection(ICommand command, BaseConnection connection)
    {
        if (connection is null)
        {
            var exceptiveCommandMode = command.Mode & StandaloneSelectConnectionMiddleware._exceptiveCommandMode;
            return exceptiveCommandMode switch
            {
                CommandMode.WithBlocking => this._context.ConnectionPool.GetTempConnection(command.DataBaseIndex),
                CommandMode.Sub or CommandMode.PSub => this._context.ConnectionPool.GetSubConnection(),
                _ => this._context.ConnectionPool.GetMasterConnection(),
            };
        }
        return connection;
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
            this._context = null!;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }
    #endregion
}
