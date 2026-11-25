using SharpRedis.Entities.Command;

namespace SharpRedis.Network.Pool;

internal interface IReadWriteConnectionPool : IConnectionPool
{
    BaseConnection GetSlaveConnection(bool requireSlave);

    bool ShouldCache(ICommand command);

    void ReleaseTempConnection(TempConnection connection);
}
