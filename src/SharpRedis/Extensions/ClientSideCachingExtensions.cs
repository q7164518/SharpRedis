using SharpRedis.Commands;
using SharpRedis.Models;
using System;

namespace SharpRedis.Extensions
{
    internal static class ClientSideCachingExtensions
    {
        internal const string _invalidate_channel_name = "__redis__:invalidate";

        internal const string _clientSideCachingPipePlaceholder = "OK";

        internal const CommandMode _cacheCommandMode = CommandMode.Read | CommandMode.WithClientSideCache;

        internal static void UseClientSideCaching(Redis redis, ClientSideCachingStandard clientSideCaching)
        {
            if (clientSideCaching.Mode is ClientSideCachingMode.Default)
            {
                ClientSideCachingExtensions.UseClientSideCachingByDefault(redis, clientSideCaching);
            }
            else if (clientSideCaching.Mode is ClientSideCachingMode.Broadcasting)
            {
                ClientSideCachingExtensions.UseClientSideCachingByBroadcasting(redis, clientSideCaching);
            }
            else
            {
                redis.Dispose();
                throw new RedisException("Unrecognized client cache mode");
            }
        }

        private static void UseClientSideCachingByDefault(Redis redis, ClientSideCachingStandard clientSideCaching)
        {
            bool track;
            try
            {
                var clientId = redis.PubSub.SubscribeReturnClientID(ClientSideCachingExtensions._invalidate_channel_name, clientSideCaching.ClientSideCacheInvalidate);
                if ((clientSideCaching.KeyPatterns is null || clientSideCaching.KeyPatterns.Length is 0)
                    && (clientSideCaching.WithoutKeyPatterns is null || clientSideCaching.WithoutKeyPatterns.Length is 0))
                {
                    track = redis.Connection.ClientTracking(onOff: true, clientId: clientId, noloop: true);
                }
                else if (clientSideCaching.KeyPatterns?.Length > 0)
                {
                    track = redis.Connection.ClientTracking(onOff: true, clientId: clientId, optin: true, noloop: true);
                }
                else
                {
                    track = redis.Connection.ClientTracking(onOff: true, clientId: clientId, optout: true, noloop: true);
                }
                if (track)
                {
                    clientSideCaching.RedirectConnectionId = clientId;
                }
            }
            catch (Exception ex)
            {
                redis.Dispose();
                throw new RedisException($"Failed to start tracing, Msg: {ex.Message}");
            }
            if (!track)
            {
                redis.Dispose();
                throw new RedisException("Failed to start tracing");
            }
        }

        private static void UseClientSideCachingByBroadcasting(Redis redis, ClientSideCachingStandard clientSideCaching)
        {
            bool track;
            try
            {
                var clientId = redis.PubSub.SubscribeReturnClientID(ClientSideCachingExtensions._invalidate_channel_name, clientSideCaching.ClientSideCacheInvalidate);
                if (clientSideCaching.KeyPrefixes is null || clientSideCaching.KeyPrefixes.Length is 0)
                {
                    track = redis.Connection.ClientTracking(onOff: true, clientId: clientId, bcast: true, noloop: true);
                }
                else
                {
                    track = redis.Connection.ClientTracking(onOff: true, clientId: clientId, bcast: true, noloop: true, prefixes: clientSideCaching.KeyPrefixes);
                }
                if (track)
                {
                    clientSideCaching.RedirectConnectionId = clientId;
                }
            }
            catch (Exception ex)
            {
                redis.Dispose();
                throw new RedisException($"Failed to start tracing, Msg: {ex.Message}");
            }
            if (!track)
            {
                redis.Dispose();
                throw new RedisException("Failed to start tracing");
            }
        }

        internal static CommandPacket GetTrackingCommand(ClientSideCachingStandard clientSideCaching)
        {
            if (clientSideCaching.Mode is ClientSideCachingMode.Default)
            {
                if ((clientSideCaching.KeyPatterns is null || clientSideCaching.KeyPatterns.Length is 0)
                    && (clientSideCaching.WithoutKeyPatterns is null || clientSideCaching.WithoutKeyPatterns.Length is 0))
                {
                    return ConnectionCommands.ClientTracking(onOff: true, clientId: clientSideCaching.RedirectConnectionId, noloop: true);
                }
                else if (clientSideCaching.KeyPatterns != null)
                {
                    return ConnectionCommands.ClientTracking(onOff: true, clientId: clientSideCaching.RedirectConnectionId, optin: true, noloop: true);
                }
                else
                {
                    return ConnectionCommands.ClientTracking(onOff: true, clientId: clientSideCaching.RedirectConnectionId, optout: true, noloop: true);
                }
            }
            else
            {
                if (clientSideCaching.KeyPrefixes is null)
                {
                    return ConnectionCommands.ClientTracking(onOff: true, clientId: clientSideCaching.RedirectConnectionId, bcast: true, noloop: true);
                }
                else
                {
                    return ConnectionCommands.ClientTracking(onOff: true, clientId: clientSideCaching.RedirectConnectionId, bcast: true, noloop: true, prefixes: clientSideCaching.KeyPrefixes);
                }
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static bool MatcheBroadcasting(ClientSideCachingStandard clientSideCaching, CommandPacket command, string? keyPrefix)
#else
        internal static bool MatcheBroadcasting(ClientSideCachingStandard clientSideCaching, CommandPacket command, string keyPrefix)
#endif
        {
            if (clientSideCaching.KeyPrefixes is null || clientSideCaching.KeyPrefixes.Length is 0)
            {
                return true;
            }

            var keys = command.GetKeys(keyPrefix);
            for (uint keyIndex = 0; keyIndex < keys.Length; keyIndex++)
            {
                for (uint prefixeIndex = 0; prefixeIndex < clientSideCaching.KeyPrefixes.Length; prefixeIndex++)
                {
                    if (!keys[keyIndex].StartsWith(clientSideCaching.KeyPrefixes[prefixeIndex]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static ClientSideCachingDefaultMatchType MatcheDefault(ClientSideCachingStandard clientSideCaching, CommandPacket command, string? keyPrefix)
#else
        internal static ClientSideCachingDefaultMatchType MatcheDefault(ClientSideCachingStandard clientSideCaching, CommandPacket command, string keyPrefix)
#endif
        {
            if ((clientSideCaching.KeyPatterns is null || clientSideCaching.KeyPatterns.Length is 0)
                && (clientSideCaching.WithoutKeyPatterns is null || clientSideCaching.WithoutKeyPatterns.Length is 0))
            {
                //all keys
                return ClientSideCachingDefaultMatchType.Include;
            }

            var keys = command.GetKeys(keyPrefix);

            if (clientSideCaching.KeyPatterns?.Length > 0)
            {
                for (uint keyIndex = 0; keyIndex < keys.Length; keyIndex++)
                {
                    if (!StringExtensions.Matches(keys[keyIndex], clientSideCaching.KeyPatterns))
                        return ClientSideCachingDefaultMatchType.Unmatch;
                }

                return ClientSideCachingDefaultMatchType.Include;
            }

            if (clientSideCaching.WithoutKeyPatterns?.Length > 0)
            {
                for (uint keyIndex = 0; keyIndex < keys.Length; keyIndex++)
                {
                    if (StringExtensions.Matches(keys[keyIndex], clientSideCaching.WithoutKeyPatterns))
                    {
                        return ClientSideCachingDefaultMatchType.Exclude;
                    }
                }
            }

            return ClientSideCachingDefaultMatchType.Include | ClientSideCachingDefaultMatchType.Unmatch;
        }
    }
}
