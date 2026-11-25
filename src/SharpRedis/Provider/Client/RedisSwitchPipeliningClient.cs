using SharpRedis.Entities.Command;
using SharpRedis.Network;
using SharpRedis.Pipelines;
using SharpRedis.Provider.Standard;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Provider.Client;

internal sealed class RedisSwitchPipeliningClient : BaseRedis, IRedisPipeliningClient
{
    private BaseConnection _redisConnection;
    private RedisPipeliningClient _redis;

    public sealed override int DataBase => this._redis.DataBase;

    internal RedisSwitchPipeliningClient(
        RedisPipeliningClient redis,
        RedisContext context,
        BaseConnection redisConnection
    )
        : base(context)
    {
        this._redis = redis;
        this._redisConnection = redisConnection;
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
        var result = this._redis.Invoke(command, cancellationToken);
        return result;
    }

    internal sealed override Task<RedisResult> InvokeAsync(CommandPacket command, CancellationToken cancellationToken)
    {
        var result = this._redis.InvokeAsync(command, cancellationToken);
        return result;
    }

#if MODERN_NET
    private RedisResult[]? Run(CommandFlags flags, CancellationToken cancellationToken)
#else
    private RedisResult[] Run(CommandFlags flags, CancellationToken cancellationToken)
#endif
    {
        return this._redis.Run(this._redisConnection, flags, cancellationToken);
    }

#if MODERN_NET
    private Task<RedisResult[]?> RunAsync(CommandFlags flags, CancellationToken cancellationToken)
#else
    private Task<RedisResult[]> RunAsync(CommandFlags flags, CancellationToken cancellationToken)
#endif
    {
        return this._redis.RunAsync(this._redisConnection, flags, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        if (!base._disposedValue)
        {
            base._disposedValue = true;
            if (disposing)
            {
                base.Dispose(disposing);
                this._redis.Dispose();
            }

            base._context = null!;
            this._redis = null!;
            this._redisConnection = null!;
        }
    }
}
