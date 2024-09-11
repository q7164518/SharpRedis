#pragma warning disable IDE0130
using System;

namespace SharpRedis
{
    public readonly struct ClientSideCacheKey : IEquatable<ClientSideCacheKey>
    {
        private readonly string[] _keys;
        private readonly long _keyHash;

        internal ClientSideCacheKey(string[] keys, long keyHash)
        {
            this._keys = keys;
            this._keyHash = keyHash;
        }

        internal bool ContainsKey(string key)
        {
            for (uint i = 0; i < this._keys.Length; i++)
            {
                if (this._keys[i] == key) return true;
            }
            return false;
        }

        internal string[] GetKeys()
        {
            return this._keys;
        }

        public override int GetHashCode()
        {
            return this._keyHash.GetHashCode();
        }

        public bool Equals(ClientSideCacheKey other)
        {
            return other._keyHash == this._keyHash;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public override bool Equals(object? obj)
#else
        public override bool Equals(object obj)
#endif
        {
            if (obj == null) return false;
            if (obj is ClientSideCacheKey item)
            {
                return item.Equals(this);
            }
            return false;
        }

        public override string ToString()
        {
            return this._keyHash.ToString();
        }

        public static bool operator ==(ClientSideCacheKey left, ClientSideCacheKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ClientSideCacheKey left, ClientSideCacheKey right)
        {
            return !left.Equals(right);
        }
    }
}
