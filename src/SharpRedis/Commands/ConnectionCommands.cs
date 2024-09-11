using SharpRedis.Models;

namespace SharpRedis.Commands
{
    internal static class ConnectionCommands
    {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket Auth(string? user, string? password)
#else
        internal static CommandPacket Auth(string user, string password)
#endif
        {
            return new CommandPacket("AUTH", CommandMode.Connection)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArg(!string.IsNullOrEmpty(user), user!)
                .WriteArg(!string.IsNullOrEmpty(password), password!);
#else
                .WriteArg(!string.IsNullOrEmpty(user), user)
                .WriteArg(!string.IsNullOrEmpty(password), password);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket Hello(RespVersion protover, string? user, string? password, string? clientname)
#else
        internal static CommandPacket Hello(RespVersion protover, string user, string password, string clientname)
#endif
        {
            var command = new CommandPacket("HELLO", CommandMode.Connection)
                .WriteArg((int)protover)
                .WriteArg(!string.IsNullOrEmpty(user) || !string.IsNullOrEmpty(password), "AUTH")
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteValue(!string.IsNullOrEmpty(user), user!)
#else
                .WriteValue(!string.IsNullOrEmpty(user), user)
#endif
                .WriteValue(string.IsNullOrEmpty(user), "default")
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteValue(!string.IsNullOrEmpty(password), password!);
#else
                .WriteValue(!string.IsNullOrEmpty(password), password);
#endif
            if (!string.IsNullOrEmpty(clientname))
            {
                command.WriteArg("SETNAME")
                    .WriteValue(clientname);
            }
            return command;
        }

        internal static CommandPacket Echo<TValue>(TValue message) where TValue : class
        {
            return new CommandPacket("ECHO", CommandMode.Connection)
                .WriteValue(message);
        }

        internal static CommandPacket Select(uint index)
        {
            return new CommandPacket("SELECT", CommandMode.Connection)
                .WriteArg(index);
        }

        internal static CommandPacket ClientSetName(string clientname)
        {
            return new CommandPacket("CLIENT", CommandMode.Connection)
                .WriteArg("SETNAME")
                .WriteValue(clientname);
        }

        internal static CommandPacket ClientGetName()
            => new CommandPacket("CLIENT", CommandMode.Connection).WriteArg("GETNAME");

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket Ping(string? message)
#else
        internal static CommandPacket Ping(string message)
#endif
        {
            return new CommandPacket("PING", CommandMode.Connection | CommandMode.WithoutActiveTime)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteValue(!string.IsNullOrEmpty(message), message!);
#else
                .WriteValue(!string.IsNullOrEmpty(message), message);
#endif
        }

        internal static CommandPacket ClientId() => new CommandPacket("CLIENT", CommandMode.Connection).WriteArg("ID");

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket ClientTracking(bool onOff, long clientId, string[]? prefixes = null, bool bcast = false, bool optin = false, bool optout = false, bool noloop = false)
#else
        internal static CommandPacket ClientTracking(bool onOff, long clientId, string[] prefixes = null, bool bcast = false, bool optin = false, bool optout = false, bool noloop = false)
#endif
        {
            var command = new CommandPacket("CLIENT", CommandMode.Connection)
                .WriteArg("TRACKING")
                .WriteArg(onOff, "ON")
                .WriteArg(!onOff, "OFF")
                .WriteArg("REDIRECT", clientId);

            if (prefixes?.Length > 0)
            {
                for (uint i = 0; i < prefixes.Length; i++)
                {
                    command.WriteArg("PREFIX", prefixes[i]);
                }
            }
            command.WriteArg(bcast, "BCAST")
                .WriteArg(optin, "OPTIN")
                .WriteArg(optout, "OPTOUT")
                .WriteArg(noloop, "NOLOOP");
            return command;
        }

        internal static CommandPacket ClientCaching(bool yn)
        {
            return new CommandPacket("CLIENT", CommandMode.Connection)
                .WriteArg("CACHING")
                .WriteValue(yn, "YES")
                .WriteValue(!yn, "NO");
        }

        internal static CommandPacket ClientGetRedir()
            => new CommandPacket("CLIENT", CommandMode.Connection).WriteArg("GETREDIR");
    }
}
