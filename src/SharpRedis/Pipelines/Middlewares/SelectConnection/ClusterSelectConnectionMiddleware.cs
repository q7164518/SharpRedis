using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Network;
using SharpRedis.Network.Pool;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Pipelines.Middlewares.SelectConnection;

internal sealed class ClusterSelectConnectionMiddleware : IMiddleware
{
    public const string NAME = "ClusterSelectConnection";
    private readonly RedisContext _context;

    public string Name => NAME;

    internal ClusterSelectConnectionMiddleware(RedisContext context) => this._context = context;

    private ClusterConnectionPool Pool => (ClusterConnectionPool)this._context.ConnectionPool;

    public void Dispose() { }

    private BaseConnection Select(ICommand command, int slot)
    {
        var pool = this.Pool;
        if (command.IsBatch) return pool.Node(slot).GetMasterConnection();
        if ((command.Mode & (CommandMode.Sub | CommandMode.PSub)) != 0) return pool.GetSubConnection();
        var node = pool.Node(slot);
        if ((command.Mode & CommandMode.WithBlocking) != 0) return node.GetTempConnection(0);
        if ((command.Mode & CommandMode.DoNotReuseConnection) != 0) return node.CreateMasterConnection();
        if (pool.Options.EnableReadWriteSeparation)
        {
            bool master = (command.Flags & CommandFlags.ForceMaster) != 0;
            bool replica = (command.Flags & CommandFlags.ForceSlave) != 0;
            bool read = (command.Mode & CommandMode.WithRead) != 0;
            if (master && replica) throw new ArgumentException("ForceMaster and ForceSlave cannot be combined.");
            if (replica && !read) throw new ArgumentException("ForceSlave requires a read-only command.");
            if (!master && read && (replica || !pool.ShouldCache(command)))
            {
                var selected = node.GetSlaveConnection(replica);
                if (selected is ClusterConnection { IsReplica: true }) command.RemoveMode(CommandMode.WithClientCaching);
                return selected;
            }
        }
        return node.GetMasterConnection();
    }

    private static bool Redirect(RedisResult result, string sourceHost, int expectedSlot, out string host, out int port, out bool asking)
    {
        host = string.Empty;
        port = 0;
        asking = false;
        if (result.Type is not (RespType.SimpleErrors or RespType.BulkErrors)) return false;

        string message = result.RawValue is Exception error ? error.Message : result.RawValue.ToString() ?? string.Empty;
        var parts = message.Split(' ');
        if (parts.Length != 3 || (parts[0] != "MOVED" && parts[0] != "ASK")) return false;
        if (!int.TryParse(parts[1], out int slot) || slot < 0 || slot >= 16384 || (expectedSlot >= 0 && slot != expectedSlot)) return false;
        int colon = parts[2].LastIndexOf(':');
#if !NETSTANDARD2_0
        if (colon < 0 || !int.TryParse(parts[2].AsSpan(colon + 1), out port) || port <= 0 || port > 65535) return false;
        host = parts[2][..colon].Trim('[', ']');
#else
        if (colon < 0 || !int.TryParse(parts[2].Substring(colon + 1), out port) || port <= 0 || port > 65535) return false;
        host = parts[2].Substring(0, colon).Trim('[', ']');
#endif
        if (host.Length == 0) host = sourceHost;
        asking = parts[0] == "ASK";
        return true;
    }

    private void Release(BaseConnection connection)
    {
        if (connection is ClusterConnection { Dedicated: true } dedicated) this.Pool.Release(dedicated);
    }

    RedisResult IMiddleware.Invoke(ICommand command, BaseConnection connection, RedisDelegate next, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        int slot = this.Pool.GetSlot(command);
        var selected = this.Select(command, slot);
        try
        {
            var result = next.Invoke(command, selected, cancellationToken);
            if (command.IsBatch && result.RawValue is RedisResult[] results)
            {
                // Retry only individual commands explicitly rejected by Redis; never replay successful writes.
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i].Type is RespType.SimpleErrors or RespType.BulkErrors)
                    {
                        results[i] = this.Retry(command.BatchCommands[i], results[i], selected.Host, slot, next, cancellationToken);
                    }
                }
                return result;
            }
            return result.Type is RespType.SimpleErrors or RespType.BulkErrors
                ? this.Retry(command, result, selected.Host, slot, next, cancellationToken)
                : result;
        }
        finally
        {
            this.Release(selected);
        }
    }

    private RedisResult Retry(ICommand command, RedisResult result, string sourceHost, int slot, RedisDelegate next, CancellationToken cancellationToken)
    {
        for (int attempt = 0;
            attempt < this.Pool.Options.MaxRedirects
                && ClusterSelectConnectionMiddleware.Redirect(result, sourceHost, slot, out var host, out var port, out var asking);
            attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var connection = this.Pool.Redirect(host, port, !asking);
            try
            {
                if (asking) connection.Asking(this.Pool.Options.CommandTimeout);
                result = next.Invoke(command, connection, cancellationToken);
                sourceHost = host;
            }
            finally
            {
                this.Pool.Release(connection);
            }
        }
        return result;
    }

    async Task<RedisResult> IMiddleware.InvokeAsync(ICommand command, BaseConnection connection, AsyncRedisDelegate next, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        int slot = this.Pool.GetSlot(command);
        var selected = this.Select(command, slot);
        try
        {
            var result = await next.Invoke(command, selected, cancellationToken).ConfigureAwait(false);
            if (command.IsBatch && result.RawValue is RedisResult[] results)
            {
                // Retry only individual commands explicitly rejected by Redis; never replay successful writes.
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i].Type is RespType.SimpleErrors or RespType.BulkErrors)
                    {
                        results[i] = await this.RetryAsync(command.BatchCommands[i], results[i], selected.Host, slot, next, cancellationToken).ConfigureAwait(false);
                    }
                }
                return result;
            }
            if (result.Type is RespType.SimpleErrors or RespType.BulkErrors)
                return await this.RetryAsync(command, result, selected.Host, slot, next, cancellationToken).ConfigureAwait(false);
            return result;
        }
        finally
        {
            this.Release(selected);
        }
    }

    private async Task<RedisResult> RetryAsync(ICommand command, RedisResult result, string sourceHost, int slot, AsyncRedisDelegate next, CancellationToken cancellationToken)
    {
        for (int attempt = 0;
            attempt < this.Pool.Options.MaxRedirects
                && Redirect(result, sourceHost, slot, out var host, out var port, out var asking);
            attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var connection = this.Pool.Redirect(host, port, !asking);
            try
            {
                if (asking) connection.Asking(this.Pool.Options.CommandTimeout);
                result = await next.Invoke(command, connection, cancellationToken).ConfigureAwait(false);
                sourceHost = host;
            }
            finally
            {
                this.Pool.Release(connection);
            }
        }
        return result;
    }
}
