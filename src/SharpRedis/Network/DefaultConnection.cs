using SharpRedis.Network.Standard;
using System.Text;

namespace SharpRedis.Network
{
    internal sealed class DefaultConnection : BaseConnection
    {
        private readonly int _slaveIndex;

        internal int SlaveIndex => this._slaveIndex;

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        internal DefaultConnection(Encoding encoding, ConnectionOptions connectionOptions, byte[]? _prefix, int slaveIndex = -1)
#else
        internal DefaultConnection(Encoding encoding, ConnectionOptions connectionOptions, byte[] _prefix, int slaveIndex = -1)
#endif
            : base(encoding, connectionOptions, ConnectionType.Master, _prefix)
        {
            this._slaveIndex = slaveIndex;
        }

        ~DefaultConnection()
        {
            base.Dispose(true);
        }
    }
}
