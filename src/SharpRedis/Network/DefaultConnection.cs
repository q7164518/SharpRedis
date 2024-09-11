using SharpRedis.Network.Standard;
using System.Text;

namespace SharpRedis.Network
{
    internal sealed class DefaultConnection : BaseConnection
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        internal DefaultConnection(Encoding encoding, ConnectionOptions connectionOptions, byte[]? _prefix)
#else
        internal DefaultConnection(Encoding encoding, ConnectionOptions connectionOptions, byte[] _prefix)
#endif
            : base(encoding, connectionOptions, ConnectionType.Master, _prefix)
        {
        }

        ~DefaultConnection()
        {
            base.Dispose(true);
        }
    }
}
