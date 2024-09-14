#if !LOW_NET
using System.Threading.Tasks;
using System.Threading;
#endif
using SharpRedis.Models;
using SharpRedis.Provider.Standard;
using System;
using SharpRedis.Network.Standard;
using SharpRedis.Network;

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
        internal sealed override object? Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        internal sealed override object Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            DefaultConnection connection;
            if ((command.Mode & CommandMode.Read) == CommandMode.Read)
            {
                connection = base.ConnectionPool.GetSlaveConnection(cancellationToken);
            }
            else
            {
                connection = base.ConnectionPool.GetMasterConnection(cancellationToken);
            }
            try
            {
                return connection.ExecuteCommand(command, cancellationToken);
            }
            catch
            {
                throw;
            }
            finally
            {
                if ((command.Mode & CommandMode.Read) == CommandMode.Read)
                    base.ConnectionPool.ReturnSlaveConnection(connection);
                else
                    base.ConnectionPool.ReturnMasterConnection(connection);
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override object?[]? Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        internal sealed override object[] Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            var connection = base.ConnectionPool.GetMasterConnection(cancellationToken);
            try
            {
                var result = connection.ExecuteCommands(commands, cancellationToken);
                if (result is Exception ex) throw ex;
                if (result is object[] array)
                {
                    if (array.Length != commands.Length) throw new RedisException("The number of pipe return values is inconsistent");
                    return array;
                }
                throw new RedisException("Not a valid pipe return value");
            }
            catch
            {
                throw;
            }
            finally
            {
                base.ConnectionPool.ReturnMasterConnection(connection);
            }
        }
        #endregion

        #region Async
#if !LOW_NET
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async sealed internal override Task<object?> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        async sealed internal override Task<object> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            DefaultConnection connection;
            if ((command.Mode & CommandMode.Read) == CommandMode.Read)
            {
                connection = await base.ConnectionPool.GetSlaveConnectionAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                connection = await base.ConnectionPool.GetMasterConnectionAsync(cancellationToken).ConfigureAwait(false);
            }
            try
            {
                return await connection.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
            finally
            {
                if ((command.Mode & CommandMode.Read) == CommandMode.Read)
                    base.ConnectionPool.ReturnSlaveConnection(connection);
                else
                    base.ConnectionPool.ReturnMasterConnection(connection);
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async sealed internal override Task<object?[]?> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        async sealed internal override Task<object[]> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            var connection = await base.ConnectionPool.GetMasterConnectionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var result = await connection.ExecuteCommandsAsync(commands, cancellationToken).ConfigureAwait(false);
                if (result is Exception ex) throw ex;
                if (result is object[] array)
                {
                    if (array.Length != commands.Length) throw new RedisException("The number of pipe return values is inconsistent");
                    return array;
                }
                throw new RedisException("Not a valid pipe return value");
            }
            catch
            {
                throw;
            }
            finally
            {
                base.ConnectionPool.ReturnMasterConnection(connection);
            }
        }
#endif
        #endregion
    }
}
