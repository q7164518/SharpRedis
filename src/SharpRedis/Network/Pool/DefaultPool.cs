using SharpRedis.Network.Standard;

namespace SharpRedis.Network.Pool
{
    internal sealed class DefaultPool : BaseConnectionPool
    {
        internal DefaultPool(ConnectionOptions masterConnectionOptions)
            : base(masterConnectionOptions)
        {
        }

        ~DefaultPool()
        {
            base.Dispose(true);
        }
    }
}
