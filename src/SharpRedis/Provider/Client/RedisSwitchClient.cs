using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Exceptions;
using SharpRedis.Network;
using SharpRedis.Pipelines;
using SharpRedis.Provider.Standard;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Provider.Client;

/// <summary>
/// Temporarily switch to the Redis operation class under the specified Redis database index
/// <para>临时切换Redis数据库的Redis操作类</para>
/// </summary>
internal sealed class RedisSwitchClient : BaseRedis, IRedisSwitchClient, IRedisTran
{
#if MODERN_NET
    private BaseConnection? _redisConnection;
#else
    private BaseConnection _redisConnection;
#endif
    private readonly int _database;

    public sealed override int DataBase => this._database;

    internal RedisSwitchClient(RedisContext context, int database)
        : base(context)
    {
        if (context.RedisMode == RedisServerMode.Cluster && database != 0)
            throw new NotSupportedException("Redis Cluster only supports database 0.");
        if (database == base._context.RedisOptions.Database)
        {
            this._database = database;
            return;
        }
        this._database = database;
        this._redisConnection = this.CreateConnection();
    }

    internal override RedisResult Invoke(CommandPacket command, CancellationToken cancellationToken)
    {
        BaseConnection connection;
        if ((command.Mode & CommandMode.WithBlocking) == CommandMode.WithBlocking)
        {
            connection = null!;
        }
        else
        {
            connection = this._redisConnection!;
        }
        var result = base._context.Invoke(command | this.DataBase, connection, cancellationToken);
        BaseRedis.CheckException(ref result);
        return result;
    }

    internal override async Task<RedisResult> InvokeAsync(CommandPacket command, CancellationToken cancellationToken)
    {
        BaseConnection connection;
        if ((command.Mode & CommandMode.WithBlocking) == CommandMode.WithBlocking)
        {
            connection = null!;
        }
        else
        {
            connection = this._redisConnection!;
        }
        var result = await base._context.InvokeAsync(command | this.DataBase, connection, cancellationToken).ConfigureAwait(false);
        BaseRedis.CheckException(ref result);
        return result;
    }

    internal override BaseConnection GetCurrentMasterConnection()
    {
        if (this._redisConnection is not null)
        {
            return this._redisConnection;
        }
        return base.GetCurrentMasterConnection();
    }

    private BaseConnection CreateConnection()
    {
        var connection = base._context.ConnectionPool.CreateMasterConnection();
        try
        {
            if (connection.SelectDatabase(this._database, base._context.RedisOptions.CommandTimeout))
            {
                return connection;
            }
            base.Dispose();
            throw new RedisInitializationException("Failed to switch the database");
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!base._disposedValue)
        {
            base._disposedValue = true;
            if (disposing)
            {
                base.Dispose(disposing);
                this._redisConnection?.Dispose();
            }

            base._context = null!;
            this._redisConnection = null;
        }
    }

    IRedisPipeliningClient IRedisPipelining.CreatePipeliningRedis()
    {
#if NET8_0_OR_GREATER
        ObjectDisposedException.ThrowIf(base._disposedValue, this);
#else
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisSwitchClient));
#endif
        var redis = new RedisPipeliningClient(base._context, this.DataBase);
        return new RedisSwitchPipeliningClient(
            redis: redis,
            context: base._context,
            redisConnection: this._redisConnection!);
    }

    IRedisTranClient IRedisTran.CreateTranRedis()
    {
#if NET8_0_OR_GREATER
        ObjectDisposedException.ThrowIf(base._disposedValue, this);
#else
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisSwitchClient));
#endif
        return new RedisTranClient(base._context, this.DataBase);
    }
}
