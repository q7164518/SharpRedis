#if NET8_0
#pragma warning disable IDE0290
#endif
using System;

namespace SharpRedis
{
    public sealed class RedisException : Exception
    {
        public RedisException(string msg)
            : base(msg)
        {
        }
    }
}
