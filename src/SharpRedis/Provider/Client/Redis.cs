using SharpRedis.Pipelines;
using SharpRedis.Provider.Standard;

namespace SharpRedis;

/// <summary>
/// Redis client
/// <para>Redis客户端</para>
/// </summary>
public sealed partial class Redis : BaseRedis, IRedisPipelining, IRedisSwitch, IRedisTran
{
    internal Redis(RedisContext context) : base(context) { }

    IRedisPipeliningClient IRedisPipelining.CreatePipeliningRedis()
    {
        return new Provider.Client.RedisPipeliningClient(base._context, base.DataBase);
    }

    IRedisSwitchClient IRedisSwitch.CreateSwitchRedis(int database)
    {
        if (this.RedisMode is RedisServerMode.Cluster) throw new System.NotSupportedException("The cluster mode does not support switching databases.");
        return new Provider.Client.RedisSwitchClient(base._context, database);
    }

    IRedisTranClient IRedisTran.CreateTranRedis()
    {
        return new Provider.Client.RedisTranClient(base._context);
    }

    protected override void Dispose(bool disposing)
    {
        if (!base._disposedValue)
        {
            base._disposedValue = true;
            if (disposing)
            {
                base._context.Dispose();
                base.Dispose(disposing);
            }

            base._context = null!;
        }
    }
}
