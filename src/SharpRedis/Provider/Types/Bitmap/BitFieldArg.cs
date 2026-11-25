using System;

namespace SharpRedis;

public sealed partial class RedisBitmap
{
    /// <summary>
    /// BitField Arg
    /// <para>位域参数</para>
    /// </summary>
    /// <example>
    /// <code>
    /// BitFieldArg.Get().IEncoding(8).Offset(1); // GET i8 1
    /// BitFieldArg.Get("u8", "1"); // GET i8 1
    /// 
    /// BitFieldArg.Get().IEncoding(8).Offset(1, true); // GET i8 #1
    /// BitFieldArg.Get("i8", "#1"); // GET i8 #1
    /// 
    /// BitFieldArg.Get().UEncoding(8).Offset(1, true); // GET u8 #1
    /// BitFieldArg.Get("u8", "#1"); // GET u8 #1
    /// 
    /// 
    /// BitFieldArg.Set().IEncoding(8).Offset(2).Value(1); // SET i8 2 1
    /// BitFieldArg.Set("i8", "2", 1); // SET i8 2 1
    /// 
    /// BitFieldArg.Set().IEncoding(8).Offset(2, true).Value(1); // SET i8 #2 1
    /// BitFieldArg.Set("i8", "#2", 1); // SET i8 #2 1
    /// 
    /// BitFieldArg.Set().UEncoding(8).Offset(2, true).Value(1); // SET u8 #2 1
    /// BitFieldArg.Set("u8", "#2", 1); // SET u8 #2 1
    /// 
    /// 
    /// BitFieldArg.Overflow().Wrap(); // OVERFLOW WRAP
    /// BitFieldArg.Overflow("WRAP"); // OVERFLOW WRAP
    /// 
    /// BitFieldArg.Overflow().Sat(); // OVERFLOW SAT
    /// BitFieldArg.Overflow("SAT"); // OVERFLOW SAT
    /// 
    /// BitFieldArg.Overflow().Fail(); // OVERFLOW FAIL
    /// BitFieldArg.Overflow("FAIL"); // OVERFLOW FAIL
    /// 
    /// 
    /// BitFieldArg.IncrBy().IEncoding(4).Offset(5).Increment(10); // INCRBY i4 5 10
    /// BitFieldArg.IncrBy("i4", "5", 10); // INCRBY i4 5 10
    /// 
    /// BitFieldArg.IncrBy().IEncoding(4).Offset(5, true).Increment(10); // INCRBY i4 #5 10
    /// BitFieldArg.IncrBy("i4", "#5", 10); // INCRBY i4 #5 10
    /// 
    /// BitFieldArg.IncrBy().UEncoding(4).Offset(5, true).Increment(-10); // INCRBY u4 #5 -10
    /// BitFieldArg.IncrBy("u4", "#5", -10); // INCRBY u4 #5 -10
    /// </code>
    /// </example>
    public sealed class BitFieldArg : IBitFieldArg,
        IBitFieldArgEncoding<IBitFieldArgOffset<BitFieldArg>>, IBitFieldArgOffset<BitFieldArg>,   //GET
        IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgValue>>, IBitFieldArgOffset<IBitFieldArgValue>, IBitFieldArgValue,  //SET
        IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgIncrement>>, IBitFieldArgOffset<IBitFieldArgIncrement>, IBitFieldArgIncrement,  //Increment
        IBitFieldArgOverflow    //OVERFLOW
    {
        private readonly string[] _args;
        private int _currentIndex = 0;
        private readonly BitFieldOperation _type;

        BitFieldOperation IBitFieldArg.Operation => this._type;

        private BitFieldArg(int argsSize, string argName, BitFieldOperation type)
        {
            this._args = new string[argsSize];
            this.Write(argName);
            this._type = type;
        }

        #region Creates
        /// <summary>
        /// Create a GET arg
        /// <para>创建一个GET参数</para>
        /// </summary>
        /// <returns></returns>
        public static IBitFieldArgEncoding<IBitFieldArgOffset<BitFieldArg>> Get()
        {
            return new BitFieldArg(3, "GET", BitFieldOperation.Get);
        }

        /// <summary>
        /// Create a GET arg
        /// <para>创建一个GET参数</para>
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static BitFieldArg Get(string encoding, string offset)
        {
            return (BitFieldArg.Get() as BitFieldArg)!
                .Write(encoding)
                .Write(offset);
        }

        /// <summary>
        /// Create a SET arg
        /// <para>创建一个SET参数</para>
        /// </summary>
        /// <returns></returns>
        public static IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgValue>> Set()
        {
            return new BitFieldArg(4, "SET", BitFieldOperation.Set);
        }

        /// <summary>
        /// Create a SET arg
        /// <para>创建一个SET参数</para>
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BitFieldArg Set(string encoding, string offset, long value)
        {
            return (BitFieldArg.Set() as BitFieldArg)!
                .Write(encoding)
                .Write(offset)
                .Write(value.ToString());
        }

        /// <summary>
        /// Create a OVERFLOW arg
        /// <para>创建一个OVERFLOW参数</para>
        /// </summary>
        /// <returns></returns>
        public static IBitFieldArgOverflow Overflow()
        {
            return new BitFieldArg(2, "OVERFLOW", BitFieldOperation.Overflow);
        }

        /// <summary>
        /// Create a OVERFLOW arg
        /// <para>创建一个OVERFLOW参数</para>
        /// </summary>
        /// <param name="way">WRAP | SAT | FAIL</param>
        /// <returns></returns>
        public static BitFieldArg Overflow(string way)
        {
            return (BitFieldArg.Overflow() as BitFieldArg)!
                .Write(way);
        }

        /// <summary>
        /// Create a INCRBY arg
        /// <para>创建一个INCRBY参数</para>
        /// </summary>
        /// <returns></returns>
        public static IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgIncrement>> IncrBy()
        {
            return new BitFieldArg(4, "INCRBY", BitFieldOperation.IncrBy);
        }

        /// <summary>
        /// Create a INCRBY arg
        /// <para>创建一个INCRBY参数</para>
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="offset"></param>
        /// <param name="increment">increment
        /// <para>要自增的数值</para>
        /// </param>
        /// <returns></returns>
        public static BitFieldArg IncrBy(string encoding, string offset, long increment)
        {
            return (BitFieldArg.IncrBy() as BitFieldArg)!
                .Write(encoding)
                .Write(offset)
                .Write(increment.ToString());
        }
        #endregion

        #region Get
        IBitFieldArgOffset<BitFieldArg> IBitFieldArgEncoding<IBitFieldArgOffset<BitFieldArg>>.IEncoding(byte bits)
        {
            return this.SignedEncoding(ref bits);
        }

        IBitFieldArgOffset<BitFieldArg> IBitFieldArgEncoding<IBitFieldArgOffset<BitFieldArg>>.UEncoding(byte bits)
        {
            return this.UnsignedEncoding(ref bits);
        }

        BitFieldArg IBitFieldArgOffset<BitFieldArg>.Offset(uint offset, bool isMultiplied)
        {
            return isMultiplied ? this.Write($"#{offset}") : this.Write(offset.ToString());
        }
        #endregion

        #region Set
        IBitFieldArgOffset<IBitFieldArgValue> IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgValue>>.IEncoding(byte bits)
        {
            return this.SignedEncoding(ref bits);
        }

        IBitFieldArgOffset<IBitFieldArgValue> IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgValue>>.UEncoding(byte bits)
        {
            return this.UnsignedEncoding(ref bits);
        }

        IBitFieldArgValue IBitFieldArgOffset<IBitFieldArgValue>.Offset(uint offset, bool isMultiplied)
        {
            return isMultiplied ? this.Write($"#{offset}") : this.Write(offset.ToString());
        }

        BitFieldArg IBitFieldArgValue.Value(long value)
        {
            return this.Write(value.ToString());
        }
        #endregion

        #region Overflow
        BitFieldArg IBitFieldArgOverflow.Wrap()
        {
            this._args[this._currentIndex++] = "WRAP";
            return this;
        }

        BitFieldArg IBitFieldArgOverflow.Sat()
        {
            this._args[this._currentIndex++] = "SAT";
            return this;
        }

        BitFieldArg IBitFieldArgOverflow.Fail()
        {
            this._args[this._currentIndex++] = "FAIL";
            return this;
        }
        #endregion

        #region Increment
        IBitFieldArgOffset<IBitFieldArgIncrement> IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgIncrement>>.IEncoding(byte bits)
        {
            return this.SignedEncoding(ref bits);
        }

        IBitFieldArgOffset<IBitFieldArgIncrement> IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgIncrement>>.UEncoding(byte bits)
        {
            return this.UnsignedEncoding(ref bits);
        }

        IBitFieldArgIncrement IBitFieldArgOffset<IBitFieldArgIncrement>.Offset(uint offset, bool isMultiplied)
        {
            return isMultiplied ? this.Write($"#{offset}") : this.Write(offset.ToString());
        }

        BitFieldArg IBitFieldArgIncrement.Increment(long increment)
        {
            return this.Write(increment.ToString());
        }
        #endregion

        string[] IBitFieldArg.Convert() => this._args;

        private BitFieldArg Write(string arg)
        {
            this._args[this._currentIndex++] = arg;
            return this;
        }

        private BitFieldArg SignedEncoding(ref readonly byte bits)
        {
            if (bits <= 0) throw new OverflowException("At least 1 bit is required.");
            if (bits >= 65) throw new OverflowException("The supported encodings are up to 64 bits for signed integers, and up to 63 bits for unsigned integers. This limitation with unsigned integers is due to the fact that currently the Redis protocol is unable to return 64 bit unsigned integers as replies.");
            this.Write($"i{bits}");
            return this;
        }

        private BitFieldArg UnsignedEncoding(ref readonly byte bits)
        {
            if (bits <= 0) throw new OverflowException("At least 1 bit is required.");
            if (bits >= 64) throw new OverflowException("The supported encodings are up to 64 bits for signed integers, and up to 63 bits for unsigned integers. This limitation with unsigned integers is due to the fact that currently the Redis protocol is unable to return 64 bit unsigned integers as replies.");
            this.Write($"u{bits}");
            return this;
        }

#if MODERN_NET
        public override string? ToString()
#else
        public override string ToString()
#endif
        {
            return string.Join(" ", this._args);
        }
    }
}
