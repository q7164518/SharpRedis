using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Network;
using SharpRedis.Network.Pool;
using SharpRedis.Pipelines;
using SharpRedis.Provider.Standard;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Provider.Client;

internal sealed class RedisPipeliningClient : BaseRedis, IRedisPipeliningClient
{
    private CommandBatch _commandBatch;
    private readonly int _dataBase;
    private int _clusterSlot = -1;

    public sealed override int DataBase => this._dataBase;

    internal RedisPipeliningClient(RedisContext context, int dataBase) : base(context)
    {
        this._dataBase = dataBase;
        this.InitCommandBatch();
    }

#if MODERN_NET
    [System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(_commandBatch))]
#endif
    private void InitCommandBatch()
    {
        this._clusterSlot = -1;
        this._commandBatch = new();
        _ = this._commandBatch | this._dataBase;
    }

#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    IRedisPipeliningClient.ExecutePipelining(CommandFlags flags, CancellationToken cancellationToken)
    {
        return this.Run(flags, cancellationToken);
    }

#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    IRedisPipeliningClient.ExecutePipeliningAsync(CommandFlags flags, CancellationToken cancellationToken)
    {
        return this.RunAsync(flags, cancellationToken);
    }

#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    IRedisPipeliningClient.InvokePipelining(CommandFlags flags, CancellationToken cancellationToken)
    {
        return this.Run(flags, cancellationToken);
    }

#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    IRedisPipeliningClient.InvokePipeliningAsync(CommandFlags flags, CancellationToken cancellationToken)
    {
        return this.RunAsync(flags, cancellationToken);
    }

#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    IRedisPipeliningClient.RunPipelining(CommandFlags flags, CancellationToken cancellationToken)
    {
        return this.Run(flags, cancellationToken);
    }

#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    IRedisPipeliningClient.RunPipeliningAsync(CommandFlags flags, CancellationToken cancellationToken)
    {
        return this.RunAsync(flags, cancellationToken);
    }

#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    IRedisPipeliningClient.EndPipelining(CommandFlags flags, CancellationToken cancellationToken)
    {
        return this.Run(flags, cancellationToken);
    }

#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    IRedisPipeliningClient.EndPipeliningAsync(CommandFlags flags, CancellationToken cancellationToken)
    {
        return this.RunAsync(flags, cancellationToken);
    }

    internal sealed override RedisResult Invoke(CommandPacket command, CancellationToken cancellationToken)
    {
        try
        {
            int slot = this._clusterSlot;
            if (base._context.ConnectionPool is ClusterConnectionPool pool) pool.CheckKeys(command, ref slot);
            this.AddCommand(command);
            this._clusterSlot = slot;
        }
        catch
        {
            command.Dispose();
            throw;
        }
        return RedisResult._pipelineOrTranEmpty;
    }

    internal sealed override Task<RedisResult> InvokeAsync(CommandPacket command, CancellationToken cancellationToken)
    {
        try
        {
            int slot = this._clusterSlot;
            if (base._context.ConnectionPool is ClusterConnectionPool pool) pool.CheckKeys(command, ref slot);
            this.AddCommand(command);
            this._clusterSlot = slot;
        }
        catch
        {
            command.Dispose();
            throw;
        }
        return Task.FromResult(RedisResult._pipelineOrTranEmpty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddCommand(CommandPacket command)
    {
        if ((command.Mode & CommandMode.WithBlocking) == CommandMode.WithBlocking)
        {
            throw new NotSupportedException("Commands containing blocking operations cannot be executed in the pipeline.");
        }
        if ((command.Mode & CommandMode.WithoutResult) == CommandMode.WithoutResult)
        {
            throw new NotSupportedException("Commands without return values cannot be executed in the pipeline.");
        }
        _ = command | this.DataBase;
        this._commandBatch.AddCommand(command);
    }

#if MODERN_NET
    internal RedisResult[]? Run(CommandFlags flags, CancellationToken cancellationToken)
#else
    internal RedisResult[] Run(CommandFlags flags, CancellationToken cancellationToken)
#endif
    {
        if (this._commandBatch is null or { ResultCount: <= 0 }) return null;
        try
        {
            var result = base._context.Invoke(this._commandBatch | flags | CommandMode.WithPipelining, cancellationToken);
            BaseRedis.CheckException(ref result);
            return result;
        }
        finally
        {
            this.InitCommandBatch();
        }
    }

#if MODERN_NET
    internal async Task<RedisResult[]?> RunAsync(CommandFlags flags, CancellationToken cancellationToken)
#else
    internal async Task<RedisResult[]> RunAsync(CommandFlags flags, CancellationToken cancellationToken)
#endif
    {
        if (this._commandBatch is null or { ResultCount: <= 0 }) return null;
        try
        {
            var result = await base._context.InvokeAsync(this._commandBatch | flags | CommandMode.WithPipelining, cancellationToken).ConfigureAwait(false);
            BaseRedis.CheckException(ref result);
            return result;
        }
        finally
        {
            this.InitCommandBatch();
        }
    }

#if MODERN_NET
    internal RedisResult[]? Run(BaseConnection connection, CommandFlags flags, CancellationToken cancellationToken)
#else
    internal RedisResult[] Run(BaseConnection connection, CommandFlags flags, CancellationToken cancellationToken)
#endif
    {
        if (this._commandBatch is null or { ResultCount: <= 0 }) return null;
        try
        {
            var result = base._context.Invoke(this._commandBatch | flags | CommandMode.WithPipelining, connection, cancellationToken);
            BaseRedis.CheckException(ref result);
            return result;
        }
        finally
        {
            this.InitCommandBatch();
        }
    }

#if MODERN_NET
    internal async Task<RedisResult[]?> RunAsync(BaseConnection connection, CommandFlags flags, CancellationToken cancellationToken)
#else
    internal async Task<RedisResult[]> RunAsync(BaseConnection connection, CommandFlags flags, CancellationToken cancellationToken)
#endif
    {
        if (this._commandBatch is null or { ResultCount: <= 0 }) return null;
        try
        {
            var result = await base._context.InvokeAsync(this._commandBatch | flags | CommandMode.WithPipelining, connection, cancellationToken).ConfigureAwait(false);
            BaseRedis.CheckException(ref result);
            return result;
        }
        finally
        {
            this.InitCommandBatch();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!base._disposedValue)
        {
            base._disposedValue = true;
            if (disposing)
            {
                this._commandBatch?.Dispose();
                base.Dispose(disposing);
            }

            base._context = null!;
            this._commandBatch = null!;
        }
    }
}
