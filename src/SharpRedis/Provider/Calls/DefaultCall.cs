#if !LOW_NET
using System.Threading.Tasks;
using System.Threading;
#endif
using SharpRedis.Models;
using SharpRedis.Provider.Standard;
using System;
using SharpRedis.Network.Standard;

namespace SharpRedis.Provider.Calls
{
    internal sealed class DefaultCall : BaseCall
    {
        internal sealed override bool SubUsable => true;

        internal sealed override string CallMode => "Default mode";

        internal DefaultCall(IConnectionPool connectionPool)
            : base(connectionPool)
        {
        }

        #region Sync
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        sealed internal override object? Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        sealed internal override object Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            var connection = base.ConnectionPool.GetMasterConnection(cancellationToken);
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
        sealed async internal override Task<object?> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        sealed async internal override Task<object> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            var connection = await base.ConnectionPool.GetMasterConnectionAsync(cancellationToken).ConfigureAwait(false);
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
                base.ConnectionPool.ReturnMasterConnection(connection);
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal sealed override Task<object?[]?> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        async internal sealed override Task<object[]> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
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
