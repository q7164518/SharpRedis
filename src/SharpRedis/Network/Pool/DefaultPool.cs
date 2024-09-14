#if !LOW_NET
using System.Threading.Tasks;
using System.Threading;
#endif
using SharpRedis.Network.Standard;

namespace SharpRedis.Network.Pool
{
    internal sealed class DefaultPool : BaseConnectionPool
    {
        internal DefaultPool(ConnectionOptions masterConnectionOptions)
            : base(masterConnectionOptions)
        {
            base._idleThread.Start();
        }

        public sealed override void ReturnSlaveConnection(DefaultConnection connection)
        {
            throw new System.NotSupportedException();
        }

        public sealed override DefaultConnection GetSlaveConnection(CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }

#if !LOW_NET
        public sealed override Task<DefaultConnection> GetSlaveConnectionAsync(CancellationToken cancellationToken)
        {
            throw new System.NotSupportedException();
        }
#endif

        ~DefaultPool()
        {
            base.Dispose(true);
        }
    }
}
