using SharpRedis.Entities;
using SharpRedis.Network.Pool;
using System;
using System.Diagnostics;
using System.Threading;

namespace SharpRedis.Network;

internal sealed class TempConnection : BaseConnection
{
    private const long _borrowed = 0;
    private const long _retired = -1;
    private long _idleSince;

    internal int Database { get; }
    internal bool IsRetired => Volatile.Read(ref this._idleSince) == _retired;

    internal TempConnection(SharpConnectionOptions connectionOptions, CommandDispatcher commandDispatcher, int database, int connectTimeout = 0)
        : base(connectionOptions, commandDispatcher, connectTimeout)
    {
        this.Database = database;
        base.BeginReceive(null);
    }

    internal bool TryBorrow()
    {
        var idleSince = Volatile.Read(ref this._idleSince);
        return idleSince > 0 && this.Available
            && Interlocked.CompareExchange(ref this._idleSince, _borrowed, idleSince) == idleSince;
    }

    internal void Return(bool reusable)
    {
        if (reusable && this.Available)
        {
            _ = Interlocked.CompareExchange(ref this._idleSince, Stopwatch.GetTimestamp(), _borrowed);
        }
        else
        {
            this.Dispose();
        }
    }

    internal void RetireIfIdle(long now, double idleTicks)
    {
        var idleSince = Volatile.Read(ref this._idleSince);
        if (idleSince > 0 && (!this.Available || now - idleSince >= idleTicks)
            && Interlocked.CompareExchange(ref this._idleSince, _retired, idleSince) == idleSince)
        {
            this.Dispose();
        }
    }

    internal sealed override BaseConnection Copy(IConnectionPool pool)
    {
        throw new NotImplementedException("Temporary connection does not support copying.");
    }

    protected private sealed override void Dispose(bool disposing)
    {
        _ = Interlocked.Exchange(ref this._idleSince, _retired);
        if (!base._disposedValue)
        {
            base._disposedValue = true;
            base.Dispose(disposing);
        }
    }
}
