using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Network;
using SharpRedis.Network.Pool;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Pipelines.Middlewares.SelectConnection;

internal sealed class MasterSlaveSelectConnectionMiddleware : IMiddleware
{
    public const string NAME = "MasterSlaveSelectConnection";
    private RedisContext _context;
    private readonly bool _masterOnly;
    private bool _disposedValue;

    public string Name => MasterSlaveSelectConnectionMiddleware.NAME;

    internal MasterSlaveSelectConnectionMiddleware(RedisContext context)
    {
        this._context = context;
        this._masterOnly = context.RedisOptions is SentinelConnectionOptions { EnableReadWriteSeparation: false };
    }

    RedisResult IMiddleware.Invoke(ICommand command, BaseConnection connection, RedisDelegate next, CancellationToken cancellationToken)
    {
        BaseConnection execConnection = this.GetConnection(command, connection);
        try
        {
            return next.Invoke(command, execConnection, cancellationToken);
        }
        finally
        {
            if (execConnection is TempConnection temp)
            {
                ((IReadWriteConnectionPool)this._context.ConnectionPool).ReleaseTempConnection(temp);
            }
        }
    }

    async Task<RedisResult> IMiddleware.InvokeAsync(ICommand command, BaseConnection connection, AsyncRedisDelegate next, CancellationToken cancellationToken)
    {
        BaseConnection execConnection = this.GetConnection(command, connection);
        try
        {
            return await next.Invoke(command, execConnection, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (execConnection is TempConnection temp)
            {
                ((IReadWriteConnectionPool)this._context.ConnectionPool).ReleaseTempConnection(temp);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private BaseConnection GetConnection(ICommand command, BaseConnection connection)
    {
        // Explicit connections belong to transactions or other connection-bound operations.
        if (connection is not null)
        {
            // A switched database can outlive a Sentinel topology. Use a fresh master
            // connection for its next command; never replay an in-flight transaction.
            if (!connection.Available && this._context.ConnectionPool is SentinelConnectionPool sentinel
                && (command.Mode & CommandMode.Transaction) == 0)
                return sentinel.GetTempConnection(command.DataBaseIndex);
            return connection;
        }
        var pool = (IReadWriteConnectionPool)this._context.ConnectionPool;
        if (command.IsBatch)
        {
            //Batch command execution, using the master database
            return pool.GetMasterConnection();
        }

        if ((command.Mode & (CommandMode.Sub | CommandMode.PSub)) != 0)
            return pool.GetSubConnection();

        if (this._masterOnly)
        {
            return (command.Mode & CommandMode.WithBlocking) != 0
                ? pool.GetTempConnection(command.DataBaseIndex)
                : pool.GetMasterConnection();
        }

        bool readOnly = MasterSlaveSelectConnectionMiddleware.IsReadOnly(command.Mode);
        bool cacheable = pool.ShouldCache(command);

        var flags = command.Flags;
        bool forceMaster = (flags & CommandFlags.ForceMaster) == CommandFlags.ForceMaster;
        bool forceSlave = (flags & CommandFlags.ForceSlave) == CommandFlags.ForceSlave;

        if (forceMaster && forceSlave) throw new ArgumentException("ForceMaster and ForceSlave cannot be combined in one command or batch.");
        if (forceSlave && !readOnly) throw new ArgumentException("ForceSlave requires a read-only command or batch.");

        bool slave = !forceMaster && readOnly && (forceSlave || !cacheable);
        if ((command.Mode & CommandMode.WithBlocking) == CommandMode.WithBlocking)
        {
            return pool.GetTempConnection(command.DataBaseIndex);
        }
        if (slave)
        {
            command.RemoveMode(CommandMode.WithClientCaching);
            return pool.GetSlaveConnection(forceSlave);
        }
        return pool.GetMasterConnection();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsReadOnly(CommandMode mode)
        => (mode & CommandMode.WithRead) == CommandMode.WithRead;

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
        GC.SuppressFinalize(this);
    }
    #endregion
}
