#if !LOW_NET
using System.Threading;
using System.Threading.Tasks;
#endif
using System;
using System.Text;


namespace SharpRedis.Network.Standard
{
    internal interface IConnectionPool : IDisposable
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        byte[]? KeyPrefix { get; }
#else
        byte[] KeyPrefix { get; }
#endif

        Encoding Encoding { get; }

        ConnectionOptions MasterConnectionOptions { get; }

        #region Methods
        DefaultConnection GetMasterConnection(CancellationToken cancellationToken);

        DefaultConnection GetSlaveConnection(CancellationToken cancellationToken);
#if !LOW_NET
        Task<DefaultConnection> GetMasterConnectionAsync(CancellationToken cancellationToken);

        Task<DefaultConnection> GetSlaveConnectionAsync(CancellationToken cancellationToken);
#endif

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        DefaultConnection? CreateMasterConnection();

        SubConnection? CreateSubConnection();
#else
        DefaultConnection CreateMasterConnection();

        SubConnection CreateSubConnection();
#endif

        void ReturnMasterConnection(DefaultConnection connection);

        void ReturnSlaveConnection(DefaultConnection connection);

        IConnection[] GetAllMasterConnections();

        SubConnection GetSubConnection();

        SubConnection[] GetAllSubConnections();

        IConnection[] GetAllConnections();

        void SetSetClientSideCaching(ClientSideCachingStandard clientSideCaching);
        #endregion

        #region Idle methods
        void IdleWithHeartbeat();

        void IdleMasterConnections(int delayMilliseconds);

        void IdleSubConnections();

        void IdleSlaveConnections(int delayMilliseconds);
        #endregion
    }
}
