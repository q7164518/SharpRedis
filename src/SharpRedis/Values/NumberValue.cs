#pragma warning disable IDE0130
#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
#if !LOW_NET
using System.Numerics;
#endif
using System;

namespace SharpRedis
{
    public sealed class NumberValue : IComparable, IConvertible
    {
        private static readonly NumberValue _null = new NumberValue(null, 0);

        internal static NumberValue Null => _null;

        /// <summary>
        /// 40: ( Big numbers
        /// <para>44: , double</para>
        /// <para>58: : int long</para>
        /// <para>0: NULL</para>
        /// <para>-1: unknown</para>
        /// </summary>
        internal int NumberType => this._numberType;

        private readonly bool _hasValue;
        private readonly int _numberType;

        public bool HasValue => this._hasValue;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private readonly string? _numberString;

        internal NumberValue(string? numberString, int numberType)
#else
        private readonly string _numberString;

        internal NumberValue(string numberString, int numberType)
#endif
        {
            this._numberType = numberType;
            if (!string.IsNullOrEmpty(numberString))
            {
                this._numberString = numberString;
                this._hasValue = true;
            }
        }

        #region CompareTo
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public int CompareTo(object? value)
#else
        public int CompareTo(object value)
#endif
        {
            if (value is null) return 1;
            if (value is int @int)
            {
                if (this < @int) return -1;
                if (this > @int) return 1;
                return 0;
            }
            else if (value is uint @uint)
            {
                if (this < @uint) return -1;
                if (this > @uint) return 1;
                return 0;
            }
            else if (value is short @short)
            {
                if (this < @short) return -1;
                if (this > @short) return 1;
                return 0;
            }
            else if (value is ushort @ushort)
            {
                if (this < @ushort) return -1;
                if (this > @ushort) return 1;
                return 0;
            }
            else if (value is long @long)
            {
                if (this < @long) return -1;
                if (this > @long) return 1;
                return 0;
            }
            else if (value is ulong @ulong)
            {
                if (this < @ulong) return -1;
                if (this > @ulong) return 1;
                return 0;
            }
            else if (value is float @float)
            {
                if (this < @float) return -1;
                if (this > @float) return 1;
                return 0;
            }
            else if (value is double @double)
            {
                if (this < @double) return -1;
                if (this > @double) return 1;
                return 0;
            }
            else if (value is decimal @decimal)
            {
                if (this < @decimal) return -1;
                if (this > @decimal) return 1;
                return 0;
            }
#if !LOW_NET
            else if (value is BigInteger bigInteger)
            {
                if (this < bigInteger) return -1;
                if (this > bigInteger) return 1;
                return 0;
            }
#endif
#if NET7_0_OR_GREATER
            else if (value is Int128 int128)
            {
                if (this < int128) return -1;
                if (this > int128) return 1;
                return 0;
            }
            else if (value is UInt128 uint128)
            {
                if (this < uint128) return -1;
                if (this > uint128) return 1;
                return 0;
            }
#endif
            throw new ArgumentException("Not a valid numeric value");
        }

        public int CompareTo(short value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

        public int CompareTo(ushort value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

        public int CompareTo(int value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

        public int CompareTo(uint value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

        public int CompareTo(long value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

        public int CompareTo(ulong value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

        public int CompareTo(float value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

        public int CompareTo(double value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

        public int CompareTo(decimal value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

#if !LOW_NET
        public int CompareTo(BigInteger value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }
#endif

#if NET7_0_OR_GREATER
        public int CompareTo(Int128 value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

        public int CompareTo(UInt128 value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }
#endif
        #endregion

        #region Convert methods
        public short ToInt16()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            return short.Parse(this._numberString);
        }

        public ushort ToUInt16()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            return ushort.Parse(this._numberString);
        }

        public int ToInt32()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            return int.Parse(this._numberString);
        }

        public uint ToUInt32()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            return uint.Parse(this._numberString);
        }

        public long ToInt64()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            return long.Parse(this._numberString);
        }

        public ulong ToUInt64()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            return ulong.Parse(this._numberString);
        }

        public double ToDouble()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            if (this._numberString.Equals("inf", StringComparison.OrdinalIgnoreCase))
            {
                return double.PositiveInfinity;
            }
            if (this._numberString.Equals("-inf", StringComparison.OrdinalIgnoreCase))
            {
                return double.NegativeInfinity;
            }
            if (this._numberString.Equals("nan", StringComparison.OrdinalIgnoreCase))
            {
                return double.NaN;
            }
            return double.Parse(this._numberString);
        }

        public float ToSingle()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            if (this._numberString.Equals("inf", StringComparison.OrdinalIgnoreCase))
            {
                return float.PositiveInfinity;
            }
            if (this._numberString.Equals("-inf", StringComparison.OrdinalIgnoreCase))
            {
                return float.NegativeInfinity;
            }
            if (this._numberString.Equals("nan", StringComparison.OrdinalIgnoreCase))
            {
                return float.NaN;
            }
            return float.Parse(this._numberString);
        }

        public decimal ToDecimal()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            return decimal.Parse(this._numberString);
        }

#if !LOW_NET
        public BigInteger ToBigInteger()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            return BigInteger.Parse(this._numberString);
        }
#endif

#if NET7_0_OR_GREATER
        public Int128 ToInt128()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            return Int128.Parse(this._numberString);
        }

        public UInt128 ToUInt128()
        {
            if (this._numberString is null) throw new FormatException("Cannot convert NULL to a number");
            return UInt128.Parse(this._numberString);
        }
#endif

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        bool IConvertible.ToBoolean(IFormatProvider? provider)
#else
        bool IConvertible.ToBoolean(IFormatProvider provider)
#endif
        {
            if (string.IsNullOrEmpty(this._numberString)) return false;
            return this._numberString != "0";
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        byte IConvertible.ToByte(IFormatProvider? provider)
#else
        byte IConvertible.ToByte(IFormatProvider provider)
#endif
        {
            throw new NotSupportedException();
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        char IConvertible.ToChar(IFormatProvider? provider)
#else
        char IConvertible.ToChar(IFormatProvider provider)
#endif
        {
            throw new NotSupportedException();
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        DateTime IConvertible.ToDateTime(IFormatProvider? provider)
#else
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
#endif
        {
            throw new NotSupportedException();
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        decimal IConvertible.ToDecimal(IFormatProvider? provider)
#else
        decimal IConvertible.ToDecimal(IFormatProvider provider)
#endif
        {
            return this;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        double IConvertible.ToDouble(IFormatProvider? provider)
#else
        double IConvertible.ToDouble(IFormatProvider provider)
#endif
        {
            return this;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        short IConvertible.ToInt16(IFormatProvider? provider)
#else
        short IConvertible.ToInt16(IFormatProvider provider)
#endif
        {
            return this;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        int IConvertible.ToInt32(IFormatProvider? provider)
#else
        int IConvertible.ToInt32(IFormatProvider provider)
#endif
        {
            return this;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        long IConvertible.ToInt64(IFormatProvider? provider)
#else
        long IConvertible.ToInt64(IFormatProvider provider)
#endif
        {
            return this;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        sbyte IConvertible.ToSByte(IFormatProvider? provider)
#else
        sbyte IConvertible.ToSByte(IFormatProvider provider)
#endif
        {
            throw new NotSupportedException();
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        float IConvertible.ToSingle(IFormatProvider? provider)
#else
        float IConvertible.ToSingle(IFormatProvider provider)
#endif
        {
            return this;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        string IConvertible.ToString(IFormatProvider? provider)
#else
        string IConvertible.ToString(IFormatProvider provider)
#endif
        {
            if (string.IsNullOrEmpty(this._numberString)) throw new FormatException("Cannot convert NULL to string");
            return this._numberString.ToString(provider);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
#else
        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
#endif
        {
            if (string.IsNullOrEmpty(this._numberString)) throw new FormatException("Cannot convert NULL to object");
            return (this._numberString as IConvertible).ToType(conversionType, provider);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        ushort IConvertible.ToUInt16(IFormatProvider? provider)
#else
        ushort IConvertible.ToUInt16(IFormatProvider provider)
#endif
        {
            return this;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        uint IConvertible.ToUInt32(IFormatProvider? provider)
#else
        uint IConvertible.ToUInt32(IFormatProvider provider)
#endif
        {
            return this;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        ulong IConvertible.ToUInt64(IFormatProvider? provider)
#else
        ulong IConvertible.ToUInt64(IFormatProvider provider)
#endif
        {
            return this;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public override string? ToString()
#else
        public override string ToString()
#endif
        {
            return this._numberString;
        }

        public static implicit operator short(NumberValue nv)
        {
            return nv.ToInt16();
        }

        public static implicit operator short?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToInt16();
        }

        public static implicit operator ushort(NumberValue nv)
        {
            return nv.ToUInt16();
        }

        public static implicit operator ushort?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToUInt16();
        }

        public static implicit operator int(NumberValue nv)
        {
            return nv.ToInt32();
        }

        public static implicit operator int?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToInt32();
        }

        public static implicit operator uint(NumberValue nv)
        {
            return nv.ToUInt32();
        }

        public static implicit operator uint?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToUInt32();
        }

        public static implicit operator long(NumberValue nv)
        {
            return nv.ToInt64();
        }

        public static implicit operator long?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToInt64();
        }

        public static implicit operator ulong(NumberValue nv)
        {
            return nv.ToUInt64();
        }

        public static implicit operator ulong?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToUInt64();
        }

        public static implicit operator float(NumberValue nv)
        {
            return nv.ToSingle();
        }

        public static implicit operator float?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToSingle();
        }

        public static implicit operator double(NumberValue nv)
        {
            return nv.ToDouble();
        }

        public static implicit operator double?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToDouble();
        }

        public static implicit operator decimal(NumberValue nv)
        {
            return nv.ToDecimal();
        }

        public static implicit operator decimal?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToDecimal();
        }

#if !LOW_NET
        public static implicit operator BigInteger(NumberValue nv)
        {
            return nv.ToBigInteger();
        }

        public static implicit operator BigInteger?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToBigInteger();
        }
#endif

#if NET7_0_OR_GREATER
        public static implicit operator Int128(NumberValue nv)
        {
            return nv.ToInt128();
        }

        public static implicit operator Int128?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToInt128();
        }

        public static implicit operator UInt128(NumberValue nv)
        {
            return nv.ToUInt128();
        }

        public static implicit operator UInt128?(NumberValue nv)
        {
            if (nv is null) return null;
            return nv.ToUInt128();
        }
#endif
        #endregion

        public override int GetHashCode()
        {
            if (this._numberString is null) return base.GetHashCode();
            return this._numberString.GetHashCode();
        }

        #region Equals methods
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public override bool Equals(object? obj)
#else
        public override bool Equals(object obj)
#endif
        {
            if (obj is null) return false;
            if (obj is short @short)
            {
                return this.Equals(@short);
            }
            else if (obj is ushort @ushort)
            {
                return this.Equals(@ushort);
            }
            else if (obj is int @int)
            {
                return this.Equals(@int);
            }
            else if (obj is uint @uint)
            {
                return this.Equals(@uint);
            }
            else if (obj is long @long)
            {
                return this.Equals(@long);
            }
            else if (obj is ulong @ulong)
            {
                return this.Equals(@ulong);
            }
            else if (obj is float @float)
            {
                return this.Equals(@float);
            }
            else if (obj is double @double)
            {
                return this.Equals(@double);
            }
            else if (obj is decimal @decimal)
            {
                return this.Equals(@decimal);
            }
#if !LOW_NET
            else if (obj is BigInteger bigInteger)
            {
                return this.Equals(bigInteger);
            }
#endif
#if NET7_0_OR_GREATER
            else if (obj is Int128 int128)
            {
                return this.Equals(int128);
            }
            else if (obj is UInt128 uint128)
            {
                return this.Equals(uint128);
            }
#endif
            else if (obj is string str)
            {
                return this._numberString == str;
            }
            return false;
        }

        public bool Equals(short other)
        {
            return this.ToInt16() == other;
        }

        public bool Equals(ushort other)
        {
            return this.ToUInt16() == other;
        }

        public bool Equals(int other)
        {
            return this.ToInt32() == other;
        }

        public bool Equals(uint other)
        {
            return this.ToUInt32() == other;
        }

        public bool Equals(long other)
        {
            return this.ToInt64() == other;
        }

        public bool Equals(ulong other)
        {
            return this.ToUInt64() == other;
        }

        public bool Equals(float other)
        {
            return this.ToSingle() == other;
        }

        public bool Equals(double other)
        {
            return this.ToDouble() == other;
        }

        public bool Equals(decimal other)
        {
            return this.ToDecimal() == other;
        }

        public bool Equals(short? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToInt16() == other;
        }

        public bool Equals(ushort? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToUInt16() == other;
        }

        public bool Equals(int? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToInt32() == other;
        }

        public bool Equals(uint? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToUInt32() == other;
        }

        public bool Equals(long? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToInt64() == other;
        }

        public bool Equals(ulong? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToUInt64() == other;
        }

        public bool Equals(float? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToSingle() == other;
        }

        public bool Equals(double? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToDouble() == other;
        }

        public bool Equals(decimal? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToDecimal() == other;
        }

#if !LOW_NET
        public bool Equals(BigInteger other)
        {
            return this.ToBigInteger() == other;
        }

        public bool Equals(BigInteger? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToBigInteger() == other;
        }
#endif

#if NET7_0_OR_GREATER
        public bool Equals(Int128 other)
        {
            return this.ToInt128() == other;
        }

        public bool Equals(UInt128 other)
        {
            return this.ToUInt128() == other;
        }

        public bool Equals(Int128? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToInt128() == other;
        }

        public bool Equals(UInt128? other)
        {
            if (!this._hasValue && !other.HasValue) return true;
            if (!other.HasValue || !this.HasValue) return false;
            return this.ToUInt128() == other;
        }
#endif

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public bool Equals(string? other)
#else
        public bool Equals(string other)
#endif
        {
            if (other == null) return false;
            return this._numberString == other;
        }
        #endregion

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static bool operator ==(NumberValue? left, NumberValue? right)
#else
        public static bool operator ==(NumberValue left, NumberValue right)
#endif
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left._numberString == right._numberString;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static bool operator !=(NumberValue? left, NumberValue? right)
#else
        public static bool operator !=(NumberValue left, NumberValue right)
#endif
        {
            return !(left == right);
        }
    }
}
