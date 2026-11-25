using SharpRedis.Entities.Command;
using SharpRedis.Extensions;
using SharpRedis.Network;
using SharpRedis.Provider.Standard;
using System;
using System.Threading;

namespace SharpRedis.Provider.Types;

public abstract class RedisTypeBase : IDisposable
{
    private bool _disposedValue;
    private BaseRedis _redis;

    protected private RedisTypeBase(BaseRedis redis)
    {
        this._redis = redis;
    }

    protected private virtual BaseConnection GetCurrentMasterConnection()
    {
        return this._redis.GetCurrentMasterConnection();
    }

    protected private SharpTask InvokeAsync(CommandPacket command, CancellationToken cancellationToken)
        => new(this._redis.InvokeAsync(command, cancellationToken));

    protected private RedisResult Invoke(CommandPacket command, CancellationToken cancellationToken)
        => this._redis.Invoke(command, cancellationToken);

    protected private SharpTask InvokeAsync(CommandPacket command, BaseConnection connection, CancellationToken cancellationToken)
        => new(this._redis.InvokeAsync(command, connection, cancellationToken));

    protected private RedisResult Invoke(CommandPacket command, BaseConnection connection, CancellationToken cancellationToken)
        => this._redis.Invoke(command, connection, cancellationToken);

    protected private SharpTask InvokeAsync(CommandSubscribe command, CancellationToken cancellationToken)
        => new(this._redis.InvokeAsync(command, cancellationToken));

    protected private RedisResult Invoke(CommandSubscribe command, CancellationToken cancellationToken)
        => this._redis.Invoke(command, cancellationToken);

    #region Dispose
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            this._disposedValue = true;
            if (disposing)
            {
            }
            this._redis = null!;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
