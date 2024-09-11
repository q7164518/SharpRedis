#pragma warning disable IDE0130
using System;

namespace SharpRedis
{
    public readonly struct BooleanValue
    {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private readonly string? _boolString;

        internal BooleanValue(string? boolString)
#else
        private readonly string _boolString;

        internal BooleanValue(string boolString)
#endif
        {
            this._boolString = boolString;
        }

        public bool ToBoolean()
        {
            if (this._boolString is null) return false;
            if (this._boolString.Equals("t", StringComparison.OrdinalIgnoreCase)) return true;
            if (this._boolString.Equals("f", StringComparison.OrdinalIgnoreCase)) return false;
            throw new FormatException($"\"{this._boolString}\" is not a valid Boolean value");
        }

        public override string ToString()
        {
            return this.ToBoolean().ToString();
        }

        public static implicit operator bool(BooleanValue value) => value.ToBoolean();
    }
}
