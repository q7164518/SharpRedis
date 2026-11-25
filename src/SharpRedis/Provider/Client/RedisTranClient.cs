using SharpRedis.Commands;
using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Exceptions;
using SharpRedis.Extensions;
using SharpRedis.Network;
using SharpRedis.Network.Pool;
using SharpRedis.Pipelines;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Provider.Client;

internal sealed class RedisTranClient : BaseRedis, IRedisTranClient
{
    private const string _disposedException = "The transaction has been released and cannot continue";
    private const string _noOpenExceptionMsg = "The transaction is not started, you need to run the [MULTI] command to start the transaction.";

    private bool _multi = false;
    private BaseConnection _tranConnection;
    private readonly int _database;
    private List<RedisResult> _queueResult = [with(8)];

    public sealed override int DataBase => this._database;

    internal RedisTranClient(RedisContext context)
        : base(context)
    {
        this._database = base._context.RedisOptions.Database;
        try
        {
            this._tranConnection = base._context.ConnectionPool.CreateMasterConnection();
        }
        catch
        {
            this.Dispose();
            throw;
        }
    }

    internal RedisTranClient(RedisContext context, int database)
        : this(context)
    {
        if (database == base._context.RedisOptions.Database)
        {
            return;
        }
        this._database = database;
        try
        {
            if (!this._tranConnection.SelectDatabase(this._database, base._context.RedisOptions.CommandTimeout))
            {
                this.Dispose();
                throw new RedisInitializationException("Transaction creation failed, Failed to switch the database");
            }
        }
        catch
        {
            this.Dispose();
            throw;
        }
    }

    bool IRedisTranClient.Discard(CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        if (!this._multi) return true;
        bool boolResult = this.Run(TransactionCommands.Discard(), flags, cancellationToken);
        if (boolResult)
        {
            this._multi = false;
            this._queueResult.Clear();
        }
        return boolResult;
    }

    async Task<bool> IRedisTranClient.DiscardAsync(CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        if (!this._multi) return true;
        bool boolResult = await this.RunAsync(TransactionCommands.Discard(), flags, cancellationToken).ConfigureAwait(false);
        if (boolResult)
        {
            this._multi = false;
            this._queueResult.Clear();
        }
        return boolResult;
    }

    bool IRedisTranClient.Multi(CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        if (this._multi) return true;
        bool boolResult = this.Run(TransactionCommands.Multi(), flags, cancellationToken);
        if (boolResult) this._multi = true;
        return boolResult;
    }

    async Task<bool> IRedisTranClient.MultiAsync(CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        if (this._multi) return true;
        bool boolResult = await this.RunAsync(TransactionCommands.Multi(), flags, cancellationToken).ConfigureAwait(false);
        if (boolResult) this._multi = true;
        return boolResult;
    }

    IRedisTranClient.TranResult IRedisTranClient.Exec(CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        try
        {
            var tranResult = new IRedisTranClient.TranResult(this.Run(TransactionCommands.Exec(), flags, cancellationToken), [.. this._queueResult]);
            return tranResult;
        }
        catch
        {
            // EXEC may not have been sent, or its outcome may be unknown.
            // Retire the transaction instead of reusing a connection with uncertain state.
            this.Dispose();
            throw;
        }
        finally
        {
            this._queueResult?.Clear();
            this._multi = false;
        }
    }

    async Task<IRedisTranClient.TranResult> IRedisTranClient.ExecAsync(CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        try
        {
            var result = await this.RunAsync(TransactionCommands.Exec(), flags, cancellationToken).ConfigureAwait(false);
            var tranResult = new IRedisTranClient.TranResult(result, [.. this._queueResult]);
            return tranResult;
        }
        catch
        {
            // EXEC may not have been sent, or its outcome may be unknown.
            // Retire the transaction instead of reusing a connection with uncertain state.
            this.Dispose();
            throw;
        }
        finally
        {
            this._queueResult?.Clear();
            this._multi = false;
        }
    }

    bool IRedisTranClient.Watch(SharpValue key, CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        return this.Run(TransactionCommands.Watch(key), flags, cancellationToken);
    }

    bool IRedisTranClient.Watch(SharpValueSpan keys, CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        return this.Run(TransactionCommands.Watch(keys), flags, cancellationToken);
    }

    Task<bool> IRedisTranClient.WatchAsync(SharpValue key, CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        return this.RunAsync(TransactionCommands.Watch(key), flags, cancellationToken).AsBool();
    }

    Task<bool> IRedisTranClient.WatchAsync(SharpValueSpan keys, CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        return this.RunAsync(TransactionCommands.Watch(keys), flags, cancellationToken).AsBool();
    }

    bool IRedisTranClient.Unwatch(CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        return this.Run(TransactionCommands.Unwatch(), flags, cancellationToken);
    }

    Task<bool> IRedisTranClient.UnwatchAsync(CommandFlags flags, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        return this.RunAsync(TransactionCommands.Unwatch(), flags, cancellationToken).AsBool();
    }

    internal sealed override RedisResult Invoke(CommandPacket command, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        if (!this._multi) throw new NotSupportedException(RedisTranClient._noOpenExceptionMsg);
        _ = command | CommandMode.Transaction;
        var result = this.Execute(command | this._database, cancellationToken);
        if ((command.Mode & ~CommandMode.DoNotReuseConnection) == CommandMode.Transaction)
        {
            BaseRedis.CheckException(ref result);
            return result;
        }
        this._queueResult.Add(result);
        return RedisResult._pipelineOrTranEmpty;
    }

    internal sealed override async Task<RedisResult> InvokeAsync(CommandPacket command, CancellationToken cancellationToken)
    {
        if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTranClient), RedisTranClient._disposedException);
        if (!this._multi) throw new NotSupportedException(RedisTranClient._noOpenExceptionMsg);
        _ = command | CommandMode.Transaction;
        var result = await this.ExecuteAsync(command | this._database, cancellationToken).ConfigureAwait(false);
        if ((command.Mode & ~CommandMode.DoNotReuseConnection) == CommandMode.Transaction)
        {
            BaseRedis.CheckException(ref result);
            return result;
        }
        this._queueResult.Add(result);
        return RedisResult._pipelineOrTranEmpty;
    }

    private RedisResult Run(CommandPacket command, CommandFlags flags, CancellationToken cancellationToken)
    {
        var result = this.Execute(command | flags | this._database, cancellationToken);
        BaseRedis.CheckException(ref result);
        return result;
    }

    private async Task<RedisResult> RunAsync(CommandPacket command, CommandFlags flags, CancellationToken cancellationToken)
    {
        var result = await this.ExecuteAsync(command | flags | this._database, cancellationToken).ConfigureAwait(false);
        BaseRedis.CheckException(ref result);
        return result;
    }

    private RedisResult Execute(CommandPacket command, CancellationToken cancellationToken)
    {
        if (this._tranConnection is ClusterConnection holder)
            return this.ExecuteCluster(command, holder, cancellationToken);
        return base._context.Invoke(command, this._tranConnection, cancellationToken);
    }

    private Task<RedisResult> ExecuteAsync(CommandPacket command, CancellationToken cancellationToken)
    {
        if (this._tranConnection is ClusterConnection holder)
            return this.ExecuteClusterAsync(command, holder, cancellationToken);
        return base._context.InvokeAsync(command, this._tranConnection, cancellationToken);
    }

    /// <summary>
    /// Cluster transactions own preparation and command disposal. Their bound pipeline
    /// skips connection selection and cannot redirect/replay a transaction command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="holder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private RedisResult ExecuteCluster(CommandPacket command, ClusterConnection holder, CancellationToken cancellationToken)
    {
        using (command)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var pool = (ClusterConnectionPool)base._context.ConnectionPool;
                int slot = pool.GetSlot(command);
                if (RedisTranClient.PrepareTransaction(pool, command, holder, slot, out var target))
                    return new RedisResult(RespType.SimpleString, "OK", pool.Options.Encoding);
                var result = pool.TransactionExecutor.Invoke(command, target, cancellationToken);
                RedisTranClient.CompleteTransaction(pool, command, holder, result);
                return result;
            }
            catch { holder.Dispose(); throw; }
        }
    }

    private async Task<RedisResult> ExecuteClusterAsync(CommandPacket command, ClusterConnection holder, CancellationToken cancellationToken)
    {
        using (command)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var pool = (ClusterConnectionPool)base._context.ConnectionPool;
                int slot = pool.GetSlot(command);
                if (RedisTranClient.PrepareTransaction(pool, command, holder, slot, out var target))
                    return new RedisResult(RespType.SimpleString, "OK", pool.Options.Encoding);
                var result = await pool.AsyncTransactionExecutor.Invoke(command, target, cancellationToken).ConfigureAwait(false);
                RedisTranClient.CompleteTransaction(pool, command, holder, result);
                return result;
            }
            catch
            {
                holder.Dispose();
                throw;
            }
        }
    }

    private static bool PrepareTransaction(ClusterConnectionPool pool, CommandPacket command, ClusterConnection holder, int slot, out BaseConnection connection)
    {
        if (!holder.Available) throw new RedisInitializationException("The Cluster transaction connection is no longer available.");
        string name = command.Items[0].ToString(pool.Options.Encoding)!;
        if (slot >= 0)
        {
            if (holder.Slot >= 0 && slot != holder.Slot)
            {
                holder.Dispose();
                throw new ArgumentException("CROSSSLOT: all transaction keys must use the same slot.");
            }
            if (holder.Slot < 0)
            {
                var node = pool.Node(slot);
                if (!ReferenceEquals(node, holder.Node))
                {
                    if (holder.MultiSent)
                    {
                        holder.Dispose();
                        throw new ArgumentException("A keyless transaction command already bound the transaction to another node.");
                    }
                    holder.Target = (ClusterConnection)node.CreateMasterConnection();
                }
                holder.Slot = slot;
            }
        }
        connection = holder.Target ?? holder;
        if (name.Equals("MULTI", StringComparison.OrdinalIgnoreCase))
        {
            holder.MultiPending = true;
            return true; // Delay MULTI until the first key establishes the slot.
        }
        if (name.Equals("DISCARD", StringComparison.OrdinalIgnoreCase) && !holder.MultiSent)
        {
            // MULTI was deferred, but an earlier WATCH may already exist on this socket.
            connection.Unwatch(pool.Options.CommandTimeout);
            RedisTranClient.ResetTransaction(holder);
            return true;
        }
        if (holder.MultiPending && !holder.MultiSent)
        {
            connection.Multi(pool.Options.CommandTimeout);
            holder.MultiSent = true;
        }
        return false;
    }

    private static void ResetTransaction(ClusterConnection holder)
    {
        holder.MultiPending = false; holder.MultiSent = false; holder.Slot = -1;
        holder.Target?.Dispose(); holder.Target = null!;
    }

    private static void CompleteTransaction(ClusterConnectionPool pool, CommandPacket command, ClusterConnection holder, RedisResult result)
    {
        if (result.Type is RespType.Exception or RespType.SimpleErrors or RespType.BulkErrors)
        {
            holder.Dispose();
            if (result.RawValue is Exception ex) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
            throw new RedisInitializationException("Cluster transaction failed: " + result.RawValue);
        }
        string name = command.Items[0].ToString(pool.Options.Encoding)!;
        if (name.Equals("EXEC", StringComparison.OrdinalIgnoreCase) || name.Equals("DISCARD", StringComparison.OrdinalIgnoreCase)
            || (!holder.MultiSent && name.Equals("UNWATCH", StringComparison.OrdinalIgnoreCase))) RedisTranClient.ResetTransaction(holder);
    }

    protected override void Dispose(bool disposing)
    {
        if (!base._disposedValue)
        {
            base._disposedValue = true;
            if (disposing)
            {
                base.Dispose(disposing);
                this._tranConnection?.Dispose();
                this._queueResult?.Clear();
            }

            base._context = null!;
            this._tranConnection = null!;
            this._queueResult = null!;
        }
    }
}
