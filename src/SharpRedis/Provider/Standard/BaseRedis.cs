using SharpRedis.Entities.Command;
using SharpRedis.Modules;
using SharpRedis.Network;
using SharpRedis.Pipelines;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Provider.Standard;

public abstract class BaseRedis : IRedis
{
    private protected bool _disposedValue;
    private protected RedisContext _context;
    private protected RedisConnection _connection;
    private protected RedisString _string;
    private protected RedisServer _server;
    private protected RedisKeyspace _keyspace;
    private protected RedisHash _hash;
    private protected RedisList _list;
    private protected RedisSet _set;
    private protected RedisSortedSet _sortedSet;
    private protected RedisPubSub _pubsub;
    private protected RedisStream _stream;
    private protected RedisHyperLogLog _hyperLogLog;
    private protected RedisBitmap _bitmap;
    private protected RedisGeospatial _geo;
    private protected RedisScript _script;
    private protected RedisFunction _func;
    private protected RedisCluster _cluster;
    private protected RedisModules _modules;
    private protected RedisArray _array;

    public RedisServerMode RedisMode => this._context.RedisMode;

    public virtual int DataBase => this._context.RedisOptions.Database;

    public virtual RedisConnection Connection => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._connection;

    public virtual RedisString String => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._string;

    public virtual RedisServer Server => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._server;

    public virtual RedisKeyspace Key => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._keyspace;

    public virtual RedisHash Hash => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._hash;

    public virtual RedisList List => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._list;

    public virtual RedisPubSub PubSub => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._pubsub;

    public virtual RedisSet Set => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._set;

    public virtual RedisSortedSet SortedSet => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._sortedSet;

    public virtual RedisStream Stream => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._stream;

    public virtual RedisHyperLogLog HyperLogLog => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._hyperLogLog;

    public virtual RedisBitmap Bitmap => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._bitmap;

    public virtual RedisGeospatial Geo => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._geo;

    public virtual RedisScript Script => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._script;

    public virtual RedisFunction Func => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._func;

    public virtual RedisCluster Cluster => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._cluster;

    public virtual RedisModules Modules => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._modules;

    public virtual RedisArray Array => this._disposedValue ? throw new ObjectDisposedException(nameof(Redis)) : this._array;

    private protected BaseRedis(RedisContext context)
    {
        this._context = context;
        this._connection = new(this);
        this._string = new(this);
        this._server = new(this);
        this._keyspace = new(this);
        this._hash = new(this);
        this._list = new(this);
        this._set = new(this);
        this._sortedSet = new(this);
        this._pubsub = new(this);
        this._stream = new(this);
        this._hyperLogLog = new(this);
        this._bitmap = new(this);
        this._geo = new(this);
        this._script = new(this);
        this._func = new(this);
        this._cluster = new(this);
        this._modules = new(this);
        this._array = new(this);
    }

    internal virtual RedisResult Invoke(CommandPacket command, CancellationToken cancellationToken)
    {
        var result = this._context.Invoke(command | this.DataBase, cancellationToken);
        BaseRedis.CheckException(ref result);
        return result;
    }

    internal virtual Task<RedisResult> InvokeAsync(CommandPacket command, CancellationToken cancellationToken)
        => this._context.InvokeCheckedAsync(command | this.DataBase, null!, cancellationToken);

    internal virtual RedisResult Invoke(CommandPacket command, BaseConnection connection, CancellationToken cancellationToken)
    {
        var result = this._context.Invoke(command | this.DataBase, connection, cancellationToken);
        BaseRedis.CheckException(ref result);
        return result;
    }

    internal virtual Task<RedisResult> InvokeAsync(CommandPacket command, BaseConnection connection, CancellationToken cancellationToken)
        => this._context.InvokeCheckedAsync(command | this.DataBase, connection, cancellationToken);

    internal virtual RedisResult Invoke(CommandSubscribe command, CancellationToken cancellationToken)
    {
        var result = this._context.Invoke(command | this.DataBase, cancellationToken);
        BaseRedis.CheckException(ref result);
        return result;
    }

    internal virtual Task<RedisResult> InvokeAsync(CommandSubscribe command, CancellationToken cancellationToken)
        => this._context.InvokeCheckedAsync(command | this.DataBase, null!, cancellationToken);

    internal virtual BaseConnection GetCurrentMasterConnection()
    {
        return this._context.ConnectionPool.GetMasterConnection();
    }

    public RedisResult Call(SharpValueSpan command, SharpValueSpan keys = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return this.Invoke(Commands.CommandExtensions.CallCommand(command, keys) | flags | CommandFlags.ForceMaster, cancellationToken);
    }

    public Task<RedisResult> CallAsync(SharpValueSpan command, SharpValueSpan keys = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return this.InvokeAsync(Commands.CommandExtensions.CallCommand(command, keys) | flags | CommandFlags.ForceMaster, cancellationToken);
    }

    public override string ToString()
    {
        return $"SharpRedis, Mode: {this._context.RedisMode}, DataBase: {this.DataBase}";
    }

    #region Dispose
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._connection?.Dispose();
            this._string?.Dispose();
            this._server?.Dispose();
            this._keyspace?.Dispose();
            this._hash?.Dispose();
            this._list?.Dispose();
            this._set?.Dispose();
            this._sortedSet?.Dispose();
            this._stream?.Dispose();
            this._hyperLogLog?.Dispose();
            this._bitmap?.Dispose();
            this._geo?.Dispose();
            this._script?.Dispose();
            this._func?.Dispose();
            this._cluster?.Dispose();
            this._modules?.Dispose();
            this._array?.Dispose();
        }

        this._connection = null!;
        this._string = null!;
        this._server = null!;
        this._keyspace = null!;
        this._hash = null!;
        this._list = null!;
        this._set = null!;
        this._sortedSet = null!;
        this._stream = null!;
        this._hyperLogLog = null!;
        this._bitmap = null!;
        this._geo = null!;
        this._script = null!;
        this._func = null!;
        this._cluster = null!;
        this._modules = null!;
        this._array = null!;
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal static void CheckException(ref readonly RedisResult result)
    {
        if (result.RawValue is Exception ex)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
        }
    }
}
