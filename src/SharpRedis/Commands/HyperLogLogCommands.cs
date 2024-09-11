using SharpRedis.Models;

namespace SharpRedis.Commands
{
    internal static class HyperLogLogCommands
    {
        internal static CommandPacket PFAdd<TElement>(string key, params TElement[] elements) where TElement : class
        {
            return new CommandPacket("PFADD", CommandMode.Write)
                .WriteKey(key)
                .WriteValues(elements);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket PFCount(string key, string[]? keys)
#else
        internal static CommandPacket PFCount(string key, string[] keys)
#endif
        {
            return new CommandPacket("PFCOUNT", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteKeys(keys?.Length > 0, keys!);
#else
                .WriteKeys(keys?.Length > 0, keys);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal static CommandPacket PFMerge(string destkey, string? sourceKey, string[]? sourcekeys)
#else
        internal static CommandPacket PFMerge(string destkey, string sourceKey, string[] sourcekeys)
#endif
        {
            return new CommandPacket("PFMERGE", CommandMode.Write)
                .WriteKey(destkey)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteKey(!string.IsNullOrEmpty(sourceKey), sourceKey!)
                .WriteKeys(sourcekeys?.Length > 0, sourcekeys!);
#else
                .WriteKey(!string.IsNullOrEmpty(sourceKey), sourceKey)
                .WriteKeys(sourcekeys?.Length > 0, sourcekeys);
#endif
        }
    }
}
