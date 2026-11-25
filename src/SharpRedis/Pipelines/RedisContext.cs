using SharpRedis.Entities.Command;
using SharpRedis.Exceptions;
using SharpRedis.Network;
using SharpRedis.Network.Pool;
using SharpRedis.Pipelines.Middlewares;
using SharpRedis.Pipelines.Middlewares.ClientCaching;
using SharpRedis.Pipelines.Middlewares.SelectConnection;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Pipelines;

internal sealed class RedisContext : IDisposable
{
    private static readonly string[] _middlewareSequence =
    [
        StandaloneSelectConnectionMiddleware.NAME,
        MasterSlaveSelectConnectionMiddleware.NAME,
        ClusterSelectConnectionMiddleware.NAME,
        ExceptionMiddleware.NAME,
        ClientCachingDefaultMiddleware.NAME,
        ClientCachingBroadcastMiddleware.NAME,
        ExecuteCommandMiddleware.NAME,
    ];
    private RedisDelegate _syncPacketDelegate = null!;
    private AsyncRedisDelegate _asyncPacketDelegate = null!;

    private IConnectionPool _connectionPool = null!;
    private Dictionary<string, IMiddleware> _middlewares = null!;
    private bool _disposedValue;

    internal IConnectionPool ConnectionPool => this._connectionPool;

    internal BasalConnectionOptions RedisOptions { get; set; } = null!;

    internal RedisServerMode RedisMode { get; private set; }

    internal RedisContext(RedisServerMode redisMode)
    {
        this.RedisMode = redisMode;
    }

#if MODERN_NET
    internal void Build(Dictionary<string, IMiddleware> middlewares, ClientCachingBase? clientCaching)
#else
    internal void Build(Dictionary<string, IMiddleware> middlewares, ClientCachingBase clientCaching)
#endif
    {
        this._middlewares = middlewares;
        if (this.RedisOptions is StandaloneConnectionOptions connectionOptions)
        {
            this._connectionPool = new StandaloneConnectionPool(connectionOptions, clientCaching);
        }
        else if (this.RedisOptions is MasterSlaveConnectionOptions masterSlaveOptions)
        {
            this._connectionPool = new MasterSlaveConnectionPool(masterSlaveOptions, clientCaching);
        }
        else if (this.RedisOptions is SentinelConnectionOptions sentinelOptions)
        {
            this._connectionPool = new SentinelConnectionPool(sentinelOptions, clientCaching);
        }
        else if (this.RedisOptions is ClusterConnectionOptions clusterOptions)
        {
            this._connectionPool = new ClusterConnectionPool(clusterOptions, clientCaching);
        }
        else
        {
            throw new RedisInitializationException("Unsupported Redis server mode");
        }

        using PipelineBuilder pipelineBuilder = new();
        foreach (var name in RedisContext._middlewareSequence)
        {
            if (middlewares.TryGetValue(name, out var clientCachingMiddleware))
            {
                _ = pipelineBuilder.Use(clientCachingMiddleware);
            }
        }
        (this._asyncPacketDelegate, this._syncPacketDelegate) = pipelineBuilder.Build();
        if (this._connectionPool is ClusterConnectionPool clusterPool)
        {
            // Build once for Cluster transactions: RedisTranClient selects the bound
            // connection and owns command disposal, including deferred MULTI commands.
            using PipelineBuilder transactionPipeline = new();
            foreach (var name in RedisContext._middlewareSequence)
            {
                if (name is ClusterSelectConnectionMiddleware.NAME) continue;
                if (middlewares.TryGetValue(name, out var middleware)) transactionPipeline.Use(middleware);
            }
            (clusterPool.AsyncTransactionExecutor, clusterPool.TransactionExecutor) = transactionPipeline.Build();
        }
    }

    internal Task<RedisResult> InvokeAsync(ICommand command, CancellationToken cancellationToken)
        => CommandDisposeMiddleware.InvokeAsync(command, null!, this._asyncPacketDelegate, cancellationToken);

    internal RedisResult Invoke(ICommand command, CancellationToken cancellationToken)
        => CommandDisposeMiddleware.Invoke(command, null!, this._syncPacketDelegate, cancellationToken);

    internal Task<RedisResult> InvokeAsync(ICommand command, BaseConnection connection, CancellationToken cancellationToken)
        => CommandDisposeMiddleware.InvokeAsync(command, connection, this._asyncPacketDelegate, cancellationToken);

    internal RedisResult Invoke(ICommand command, BaseConnection connection, CancellationToken cancellationToken)
        => CommandDisposeMiddleware.Invoke(command, connection, this._syncPacketDelegate, cancellationToken);

    internal Task<RedisResult> InvokeCheckedAsync(ICommand command, BaseConnection connection, CancellationToken cancellationToken)
        => CommandDisposeMiddleware.InvokeAsync(command, connection, this._asyncPacketDelegate, cancellationToken, checkException: true);

    #region Dispose
    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            this._disposedValue = true;
            if (disposing)
            {
                this._connectionPool?.Dispose();
                if (this._middlewares != null)
                {
                    foreach (var item in this._middlewares)
                        item.Value.Dispose();
                }
                this._middlewares?.Clear();
            }
            this._asyncPacketDelegate = null!;
            this._syncPacketDelegate = null!;
            this._connectionPool = null!;
            this._middlewares = null!;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
