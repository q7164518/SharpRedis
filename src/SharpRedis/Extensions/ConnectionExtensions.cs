#if NET8_0_OR_GREATER
#pragma warning disable IDE0300
#endif
using SharpRedis.Commands;
using SharpRedis.Network.Standard;
using System;

namespace SharpRedis.Extensions
{
    internal static class ConnectionExtensions
    {
        private static readonly string[] _separator = new string[] { "\r\n" };

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static bool Auth(IConnection connection, string? user, string? password)
#else
        internal static bool Auth(IConnection connection, string user, string password)
#endif
        {
            if (string.IsNullOrEmpty(password)) return true;
            var result = connection.ExecuteCommand(ConnectionCommands.Auth(user, password), default);
            if (result is Exception ex) throw ex;
            if (result is byte[] bytes)
            {
                if ((bytes[0] == 79 || bytes[0] == 111)
                    && (bytes[1] == 75 || bytes[1] == 107)) return true;
            }
            return false;
        }

        internal static string GetRedisVersion(IConnection connection)
        {
            var infoResult = connection.ExecuteCommand(ServerCommands.Info("server", "|", "grep", "redis_version"), default);
            if (infoResult is string serverInfo)
            {
                if (string.IsNullOrEmpty(serverInfo)) return string.Empty;
                var infoArray = serverInfo.Split(ConnectionExtensions._separator, StringSplitOptions.None);
                for (uint i = 0; i < infoArray.Length; i++)
                {
                    if (infoArray[i].StartsWith("redis_version:", StringComparison.OrdinalIgnoreCase))
                    {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                        return infoArray[i][14..];
#else
                        return infoArray[i].Substring(14);
#endif
                    }
                }
            }
            return string.Empty;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static bool Hello(IConnection connection, RespVersion respVersion, string? user, string? password)
#else
        internal static bool Hello(IConnection connection, RespVersion respVersion, string user, string password)
#endif
        {
            var helloResult = connection.ExecuteCommand(ConnectionCommands.Hello(respVersion, user, password, connection.ConnectionName), default);
            if (helloResult is Exception ex) throw ex;
            if (helloResult != null && helloResult != DBNull.Value)
            {
                return true;
            }
            return false;
        }

        internal static bool SetClientName(IConnection connection)
        {
            var setNameResult = connection.ExecuteCommand(ConnectionCommands.ClientSetName(connection.ConnectionName), default);
            if (setNameResult is byte[] bytes)
            {
                if ((bytes[0] == 79 || bytes[0] == 111)
                    && (bytes[1] == 75 || bytes[1] == 107)) return true;
            }
            return false;
        }

        internal static long ClientId(IConnection connection)
        {
            return ConvertExtensions.To<long>(connection.ExecuteCommand(ConnectionCommands.ClientId(), default), ResultType.Int64, connection.Encoding);
        }

        /// <summary>
        /// Idle ping
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        internal static bool Ping(IConnection connection)
        {
            try
            {
                var pingMsg = $"SharpRedis_{Guid.NewGuid():N}_Ping";
                var pingResult = connection.ExecuteCommand(ConnectionCommands.Ping(pingMsg), default);
                var pintResultString = ConvertExtensions.To<string>(pingResult, ResultType.String, connection.Encoding);
                if (pingMsg.Equals(pintResultString, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                SharpConsole.WriteError($"Idle Exception, message: {ex.Message}");
                return false;
            }
        }

        internal static bool Select(IConnection connection, ushort index)
        {
            var selectResult = connection.ExecuteCommand(ConnectionCommands.Select(index), default);
            if (selectResult is byte[] bytes)
            {
                if ((bytes[0] == 79 || bytes[0] == 111)
                    && (bytes[1] == 75 || bytes[1] == 107)) return true;
            }
            return false;
        }
    }
}
