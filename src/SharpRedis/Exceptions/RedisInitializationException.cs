#if NET8_0
#pragma warning disable IDE0290
#endif
using System;

namespace SharpRedis
{
    public sealed class RedisInitializationException : Exception
    {
        public RedisInitializationException(string msg)
            : base(msg)
        {
        }
    }
}
