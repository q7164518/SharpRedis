#pragma warning disable IDE0130
#if !NET30
using System.Linq;
#endif
#if !LOW_NET
using System.Threading.Tasks;
using System.Threading;
#endif
using SharpRedis.Commands;
using SharpRedis.Extensions;
using SharpRedis.Provider.Standard;
using System;

namespace SharpRedis
{
    /// <summary>
    /// Redis connection operation class
    /// <para>Redis连接操作类</para>
    /// </summary>
    public sealed class RedisConnection : BaseType
    {
        internal RedisConnection(BaseCall call)
            : base(call)
        {
        }

        #region Sync
        /// <summary>
        /// Returns message.
        /// <para>Available since:1.0.0</para>
        /// <para>返回一个message</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="message">Message
        /// <para>要原样返回的消息</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>message</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? Echo(string message, CancellationToken cancellationToken = default)
#else
        public string Echo(string message, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(ConnectionCommands.Echo(message), cancellationToken);
        }

        /// <summary>
        /// The command just returns the ID of the current connection.
        /// <para>Because connection pooling is used, the returned ids may not be for the same connection</para>
        /// <para>Available since:5.0.0</para>
        /// <para>获得当前连接的ID</para>
        /// <para>因为使用了连接池, 获得的ID可能不是同一个连接的</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public long ClientId(CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ConnectionCommands.ClientId(), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command enables the tracking feature of the Redis server, that is used for server assisted client side caching.
        /// <para>Call is not recommended, SharpRedis will handle it automatically when you turn on local cache</para>
        /// <para>Available since:6.0.0</para>
        /// <para>启用或停止Redis服务器的跟踪, 给客户端缓存提供支持</para>
        /// <para>不建议调用, 在你开启了本地缓存支持的时候, SharpRedis会自动处理</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="onOff">Track on or off<para>跟踪是否开启</para></param>
        /// <param name="clientId">Send invalidation messages to the connection with the specified ID. The connection must exist.
        /// <para>指定发送缓存过期消息的连接ID</para>
        /// </param>
        /// <param name="prefixes">For broadcasting, register a given key prefix, so that notifications will be provided only for keys starting with this string. This option can be given multiple times to register multiple prefixes.
        /// <para>广播模式下, 要追踪的Key前缀, 可以有多个</para>
        /// </param>
        /// <param name="bcast">Enable tracking in broadcasting mode.
        /// <para>是否启用广播模式</para>
        /// </param>
        /// <param name="optin">When broadcasting is NOT active, normally don't track keys in read only commands, unless they are called immediately after a CLIENT CACHING yes command.
        /// <para>当广播不活动时，通常不跟踪只读命令中的键，除非它们在 CLIENT CACHING YES 命令之后立即被调用</para>
        /// </param>
        /// <param name="optout">When broadcasting is NOT active, normally track keys in read only commands, unless they are called immediately after a CLIENT CACHING no command.
        /// <para>当广播不活动时，通常跟踪只读命令中的键，除非它们在 CLIENT CACHING NO 命令之后立即被调用</para>
        /// </param>
        /// <param name="noloop">Don't send notifications about keys modified by this connection itself.
        /// <para>不要发送关于此连接本身修改的键的通知</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public bool ClientTracking(bool onOff, long clientId, string[]? prefixes = null, bool bcast = false, bool optin = false, bool optout = false, bool noloop = false, CancellationToken cancellationToken = default)
#else
        public bool ClientTracking(bool onOff, long clientId, string[] prefixes = null, bool bcast = false, bool optin = false, bool optout = false, bool noloop = false, CancellationToken cancellationToken = default)
#endif
        {
            var allMasters = base._call.ConnectionPool.GetAllMasterConnections();
            if (allMasters is null || allMasters.Length is 0) return false;
            var command = ConnectionCommands.ClientTracking(onOff, clientId, prefixes, bcast, optin, optout, noloop);
            if (bcast)
            {
#if NET30
                bool anyTracking = false;
                for (uint i = 0; i < allMasters.Length; i++)
                {
                    if (allMasters[i].Tracking)
                    {
                        anyTracking = true;
                        break;
                    }
                }
                if (!anyTracking)
#else
                if (!allMasters.Any(f => f.Tracking))
#endif
                {
                    var connection = allMasters[0];
                    var trackingResult = connection.ExecuteCommand(command, cancellationToken);
                    if (ConvertExtensions.To<string>(trackingResult, ResultType.String, base._call.Encoding) != "OK")
                    {
                        return false;
                    }
                }
                else return true;

                for (int i = 0; i < allMasters.Length; i++)
                {
                    allMasters[i].Tracking = true;
                    allMasters[i].RedirectConnectionId = clientId;
                }
                return true;
            }

            var result = true;
            foreach (var connection in allMasters)
            {
                if (connection.Tracking) continue;
                var trackingResult = connection.ExecuteCommand(command, cancellationToken);
                if (ConvertExtensions.To<string>(trackingResult, ResultType.String, base._call.Encoding) == "OK")
                {
                    connection.Tracking = true;
                    connection.RedirectConnectionId = clientId;
                    continue;
                }
                result = false;
                break;
            }
            if (result) return true;

            var closeTrackingCommand = ConnectionCommands.ClientTracking(false, clientId, prefixes, bcast, optin, optout, noloop);
            foreach (var connection in allMasters)
            {
                if (connection.Tracking)
                {
                    var trackingResult = connection.ExecuteCommand(closeTrackingCommand, cancellationToken);
                    if (ConvertExtensions.To<string>(trackingResult, ResultType.String, base._call.Encoding) == "OK")
                    {
                        connection.Tracking = false;
                        connection.RedirectConnectionId = -1;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This command returns the client ID we are redirecting our tracking notifications to.
        /// <para>Available since:6.0.0</para>
        /// <para>获得客户端缓存跟踪通知订阅的客户端连接ID</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>
        /// <para>0 when not redirecting notifications to any client.</para>
        /// <para>-1 if client tracking is not enabled.</para>
        /// <para>The ID of the client to which notification are being redirected.</para>
        /// <para>无订阅返回0, 未开启客户端缓存返回-1. 其它则为订阅的连接ID</para>
        /// </returns>
        public long ClientGetRedir(CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ConnectionCommands.ClientGetRedir(), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// The CLIENT GETNAME returns the name of the current connection as set by CLIENT SETNAME.
        /// <para>Since every new connection starts without an associated name, if no name was assigned a null bulk reply is returned.</para>
        /// <para>Because connection pools are used, the connection names in all connection pools are returned</para>
        /// <para>Available since:2.6.9</para>
        /// <para>返回连接名称. 因为使用了连接池, 所以会返回所有连接池中的连接名称</para>
        /// <para>支持此命令的Redis版本, 2.6.9+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Names</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? ClientGetName(CancellationToken cancellationToken = default)
#else
        public string ClientGetName(CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(ConnectionCommands.ClientGetName(), cancellationToken);
        }
        #endregion

        #region Async
#if !LOW_NET
        /// <summary>
        /// Returns message.
        /// <para>Available since:1.0.0</para>
        /// <para>返回一个message</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="message">Message
        /// <para>要原样返回的消息</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>message</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string?> EchoAsync(string message, CancellationToken cancellationToken = default)
#else
        public Task<string> EchoAsync(string message, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStringAsync(ConnectionCommands.Echo(message), cancellationToken);
        }

        /// <summary>
        /// The command just returns the ID of the current connection.
        /// <para>Because connection pooling is used, the returned ids may not be for the same connection</para>
        /// <para>Available since:5.0.0</para>
        /// <para>获得当前连接的ID</para>
        /// <para>因为使用了连接池, 获得的ID可能不是同一个连接的</para>
        /// <para>支持此命令的Redis版本, 5.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<long> ClientIdAsync(CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(ConnectionCommands.ClientId(), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command enables the tracking feature of the Redis server, that is used for server assisted client side caching.
        /// <para>Call is not recommended, SharpRedis will handle it automatically when you turn on local cache</para>
        /// <para>Available since:6.0.0</para>
        /// <para>启用或停止Redis服务器的跟踪, 给客户端缓存提供支持</para>
        /// <para>不建议调用, 在你开启了本地缓存支持的时候, SharpRedis会自动处理</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="onOff">Track on or off<para>跟踪是否开启</para></param>
        /// <param name="clientId">Send invalidation messages to the connection with the specified ID. The connection must exist.
        /// <para>指定发送缓存过期消息的连接ID</para>
        /// </param>
        /// <param name="prefixes">For broadcasting, register a given key prefix, so that notifications will be provided only for keys starting with this string. This option can be given multiple times to register multiple prefixes.
        /// <para>广播模式下, 要追踪的Key前缀, 可以有多个</para>
        /// </param>
        /// <param name="bcast">Enable tracking in broadcasting mode.
        /// <para>是否启用广播模式</para>
        /// </param>
        /// <param name="optin">When broadcasting is NOT active, normally don't track keys in read only commands, unless they are called immediately after a CLIENT CACHING yes command.
        /// <para>当广播不活动时，通常不跟踪只读命令中的键，除非它们在 CLIENT CACHING YES 命令之后立即被调用</para>
        /// </param>
        /// <param name="optout">When broadcasting is NOT active, normally track keys in read only commands, unless they are called immediately after a CLIENT CACHING no command.
        /// <para>当广播不活动时，通常跟踪只读命令中的键，除非它们在 CLIENT CACHING NO 命令之后立即被调用</para>
        /// </param>
        /// <param name="noloop">Don't send notifications about keys modified by this connection itself.
        /// <para>不要发送关于此连接本身修改的键的通知</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public async Task<bool> ClientTrackingAsync(bool onOff, int clientId, string[]? prefixes = null, bool bcast = false, bool optin = false, bool optout = false, bool noloop = false, CancellationToken cancellationToken = default)
#else
        public async Task<bool> ClientTrackingAsync(bool onOff, int clientId, string[] prefixes = null, bool bcast = false, bool optin = false, bool optout = false, bool noloop = false, CancellationToken cancellationToken = default)
#endif
        {
            var allMasters = base._call.ConnectionPool.GetAllMasterConnections();
            if (allMasters is null || allMasters.Length is 0) return false;
            var command = ConnectionCommands.ClientTracking(onOff, clientId, prefixes, bcast, optin, optout, noloop);
            if (bcast)
            {
                if (!allMasters.Any(f => f.Tracking))
                {
                    var connection = allMasters[0];
                    var trackingResult = await connection.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false);
                    if (ConvertExtensions.To<string>(trackingResult, ResultType.String, base._call.Encoding) != "OK")
                    {
                        return false;
                    }
                }
                else return true;

                for (int i = 0; i < allMasters.Length; i++)
                {
                    allMasters[i].Tracking = true;
                    allMasters[i].RedirectConnectionId = clientId;
                }
                return true;
            }

            var result = true;
            foreach (var connection in allMasters)
            {
                if (connection.Tracking) continue;
                var trackingResult = await connection.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false);
                if (ConvertExtensions.To<string>(trackingResult, ResultType.String, base._call.Encoding) == "OK")
                {
                    connection.Tracking = true;
                    connection.RedirectConnectionId = clientId;
                    continue;
                }
                result = false;
                break;
            }
            if (result) return true;

            var closeTrackingCommand = ConnectionCommands.ClientTracking(false, clientId, prefixes, bcast, optin, optout, noloop);
            foreach (var connection in allMasters)
            {
                if (connection.Tracking)
                {
                    var trackingResult = await connection.ExecuteCommandAsync(closeTrackingCommand, cancellationToken).ConfigureAwait(false);
                    if (ConvertExtensions.To<string>(trackingResult, ResultType.String, base._call.Encoding) == "OK")
                    {
                        connection.Tracking = false;
                        connection.RedirectConnectionId = -1;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This command returns the client ID we are redirecting our tracking notifications to.
        /// <para>Available since:6.0.0</para>
        /// <para>获得客户端缓存跟踪通知订阅的客户端连接ID</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>
        /// <para>0 when not redirecting notifications to any client.</para>
        /// <para>-1 if client tracking is not enabled.</para>
        /// <para>The ID of the client to which notification are being redirected.</para>
        /// <para>无订阅返回0, 未开启客户端缓存返回-1. 其它则为订阅的连接ID</para>
        /// </returns>
        public Task<long> ClientGetRedirAsync(CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(ConnectionCommands.ClientGetRedir(), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// The CLIENT GETNAME returns the name of the current connection as set by CLIENT SETNAME.
        /// <para>Since every new connection starts without an associated name, if no name was assigned a null bulk reply is returned.</para>
        /// <para>Because connection pools are used, the connection names in all connection pools are returned</para>
        /// <para>Available since:2.6.9</para>
        /// <para>返回连接名称. 因为使用了连接池, 所以会返回所有连接池中的连接名称</para>
        /// <para>支持此命令的Redis版本, 2.6.9+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Names</returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string?> ClientGetNameAsync(CancellationToken cancellationToken = default)
#else
        public Task<string> ClientGetNameAsync(CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStringAsync(ConnectionCommands.ClientGetName(), cancellationToken);
        }
#endif
        #endregion

        [Obsolete("The [AUTH] command does not need to be invoked, and authentication is done automatically internally.", true)]
        public void Auth() => throw new NotImplementedException();

        [Obsolete("The [HELLO] command does not need to be invoked, and authentication is done automatically internally.", true)]
        public void Hello() => throw new NotImplementedException();
    }
}
