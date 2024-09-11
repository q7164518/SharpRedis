#if !LOW_NET
using System.Threading.Tasks;
using System.Threading;
#endif
using SharpRedis.Models;
using SharpRedis.Network.Pool;
using SharpRedis.Provider.Standard;
using System;
using SharpRedis.Network.Standard;
using System.Text;

namespace SharpRedis.Provider.Calls
{
    internal sealed class MasterSlaveCall : BaseCall
    {
        internal MasterSlaveCall(IConnectionPool connectionPool)
            : base(connectionPool)
        {
        }

        internal sealed override bool SubUsable => true;

        internal sealed override string CallMode => "Master slave mode";

        #region Sync
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal override object? Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        internal override object Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            throw new NotImplementedException();
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal override object?[]? Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        internal override object[] Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Async
#if !LOW_NET
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal override Task<object?> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        internal override Task<object> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            throw new NotImplementedException();
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal override Task<object?[]?> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        internal override Task<object[]> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            throw new NotImplementedException();
        }
#endif
        #endregion
    }
}
