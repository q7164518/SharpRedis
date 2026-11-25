using SharpRedis.Entities;
using SharpRedis.Network.Pool;
using System;

namespace SharpRedis.Network;

internal sealed class StandaloneConnection : BaseConnection
{
    public StandaloneConnection(SharpConnectionOptions connectionOptions, CommandDispatcher commandDispatcher, int connectTimeout = 0)
        : base(connectionOptions, commandDispatcher, connectTimeout)
    {
        base.BeginReceive(null);
    }

    internal sealed override BaseConnection Copy(IConnectionPool pool)
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(base._disposedValue || base._connectionOptions is null, this);
#else
        if (base._disposedValue || base._connectionOptions is null)
        {
            throw new ObjectDisposedException(nameof(StandaloneConnection), "The current connection object has been released and cannot be copied.");
        }
#endif

        return pool.CreateMasterConnection();
    }

    protected private sealed override void Dispose(bool disposing)
    {
        if (!base._disposedValue)
        {
            base._disposedValue = true;
            base.Dispose(disposing);
            if (disposing)
            {
            }
        }
    }
}
