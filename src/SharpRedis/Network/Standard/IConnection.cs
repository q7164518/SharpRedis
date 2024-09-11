#if !LOW_NET
using System.Threading.Tasks;
using System.Threading;
#endif
using SharpRedis.Models;
using System;
using System.Text;

namespace SharpRedis.Network.Standard
{
    internal interface IConnection : IDisposable
    {
        #region Properties
        bool Connected { get; }

        ConnectionOptions ConnectionOptions { get; }

        ConnectionType Type { get; }

        long ConnectionId { get; }

        string ConnectionName { get; }

        DateTime LastActiveTime { get; }

        ushort CurrentDataBaseIndex { get; }

        bool Tracking { get; set; }

        long RedirectConnectionId { get; set; }

        Encoding Encoding { get; }
        #endregion

        #region Methods
        void ResetBuffer();

        bool Connect();

        bool SwitchDatabaseIndex(ushort databaseIndex);

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        object? ExecuteCommand(CommandPacket command, CancellationToken cancellationToken);

        object? ExecuteCommands(CommandPacket[] commands, CancellationToken cancellationToken);

        Task<object?> ExecuteCommandAsync(CommandPacket command, CancellationToken cancellationToken);

        Task<object?> ExecuteCommandsAsync(CommandPacket[] commands, CancellationToken cancellationToken);
#else
        object ExecuteCommand(CommandPacket command, CancellationToken cancellationToken);

        object ExecuteCommands(CommandPacket[] commands, CancellationToken cancellationToken);

#if !LOW_NET
        Task<object> ExecuteCommandAsync(CommandPacket command, CancellationToken cancellationToken);

        Task<object> ExecuteCommandsAsync(CommandPacket[] commands, CancellationToken cancellationToken);
#endif
#endif
        #endregion
    }
}
