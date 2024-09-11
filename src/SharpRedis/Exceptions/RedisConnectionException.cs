#if NET8_0
#pragma warning disable IDE0290
#endif
using System;

namespace SharpRedis
{
    public sealed class RedisConnectionException : Exception
    {
        public RedisConnectionException(string msg)
            : base(msg)
        {
        }
    }
}
