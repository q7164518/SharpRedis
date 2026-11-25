using SharpRedis.Provider.Standard;
using System;

namespace SharpRedis.Network.Pool;

internal interface IConnectionPool : IDisposable
{
#if MODERN_NET
    ClientCachingBase? ClientCaching { get; }
#else
    ClientCachingBase ClientCaching { get; }
#endif

    BaseConnection CreateMasterConnection();

    BaseConnection CreateSlaveConnection();

    BaseConnection CreateSubConnection();

    BaseConnection GetMasterConnection();

    BaseConnection GetSlaveConnection();

    BaseConnection GetSubConnection();

    BaseConnection GetTempConnection(int database);

    bool IsCacheReady(BaseConnection connection, long redirectId);

    void RetryTracking();
}
