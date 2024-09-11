#pragma warning disable IDE0130

namespace SharpRedis
{
    /// <summary>
    /// BitField Arg
    /// </summary>
    public sealed class BitFieldArg : IBitFieldArg, IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArg>>, IBitFieldArgOffset<IBitFieldArg>,   //GET
        IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgValue>>, IBitFieldArgOffset<IBitFieldArgValue>, IBitFieldArgValue,  //SET
        IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgIncrement>>, IBitFieldArgOffset<IBitFieldArgIncrement>, IBitFieldArgIncrement,  //Increment
        IBitFieldArgOverflow    //OVERFLOW
    {
        private readonly string[] _args;
        private int _currentIndex = 0;
        private readonly BitFieldArgType _type;

        BitFieldArgType IBitFieldArg.ArgType => this._type;

        private BitFieldArg(int argsSize, string argName, BitFieldArgType type)
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
        public static IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArg>> CreateGet()
        {
            return new BitFieldArg(3, "GET", BitFieldArgType.Get);
        }

        /// <summary>
        /// Create a SET arg
        /// <para>创建一个SET参数</para>
        /// </summary>
        /// <returns></returns>
        public static IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgValue>> CreateSet()
        {
            return new BitFieldArg(4, "SET", BitFieldArgType.Set);
        }

        /// <summary>
        /// Create a OVERFLOW arg
        /// <para>创建一个OVERFLOW参数</para>
        /// </summary>
        /// <returns></returns>
        public static IBitFieldArgOverflow CreateOverflow()
        {
            return new BitFieldArg(2, "OVERFLOW", BitFieldArgType.Overflow);
        }

        /// <summary>
        /// Create a INCRBY arg
        /// <para>创建一个INCRBY参数</para>
        /// </summary>
        /// <returns></returns>
        public static IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgIncrement>> CreateIncrement()
        {
            return new BitFieldArg(4, "INCRBY", BitFieldArgType.Increment);
        }
        #endregion

        #region Get
        IBitFieldArgOffset<IBitFieldArg> IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArg>>.Signed(uint u)
        {
            if (u <= 0) throw new RedisException("Minimum support 1");
            if (u >= 65) throw new RedisException("Maximum support 64");
            this.Write($"i{u}");
            return this;
        }

        IBitFieldArgOffset<IBitFieldArg> IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArg>>.Unsigned(uint u)
        {
            if (u <= 0) throw new RedisException("Minimum support 1");
            if (u >= 64) throw new RedisException("Maximum support 63");
            this.Write($"u{u}");
            return this;
        }

        IBitFieldArg IBitFieldArgOffset<IBitFieldArg>.Offset(uint offset)
        {
            this.Write(offset.ToString());
            return this;
        }

        IBitFieldArg IBitFieldArgOffset<IBitFieldArg>.MultipliedOffset(uint offset)
        {
            this.Write($"#{offset}");
            return this;
        }
        #endregion

        #region Set
        IBitFieldArgOffset<IBitFieldArgValue> IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgValue>>.Signed(uint u)
        {
            if (u <= 0) throw new RedisException("Minimum support 1");
            if (u >= 65) throw new RedisException("Maximum support 64");
            this.Write($"i{u}");
            return this;
        }

        IBitFieldArgOffset<IBitFieldArgValue> IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgValue>>.Unsigned(uint u)
        {
            if (u <= 0) throw new RedisException("Minimum support 1");
            if (u >= 64) throw new RedisException("Maximum support 63");
            this.Write($"u{u}");
            return this;
        }

        IBitFieldArgValue IBitFieldArgOffset<IBitFieldArgValue>.Offset(uint offset)
        {
            this.Write(offset.ToString());
            return this;
        }

        IBitFieldArgValue IBitFieldArgOffset<IBitFieldArgValue>.MultipliedOffset(uint offset)
        {
            this.Write($"#{offset}");
            return this;
        }

        IBitFieldArg IBitFieldArgValue.Value(long value)
        {
            this.Write(value.ToString());
            return this;
        }
        #endregion

        #region Overflow
        IBitFieldArg IBitFieldArgOverflow.Wrap()
        {
            this._args[this._currentIndex++] = "WRAP";
            return this;
        }

        IBitFieldArg IBitFieldArgOverflow.Sat()
        {
            this._args[this._currentIndex++] = "SAT";
            return this;
        }

        IBitFieldArg IBitFieldArgOverflow.Fail()
        {
            this._args[this._currentIndex++] = "FAIL";
            return this;
        }
        #endregion

        #region Increment
        IBitFieldArgOffset<IBitFieldArgIncrement> IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgIncrement>>.Signed(uint u)
        {
            if (u <= 0) throw new RedisException("Minimum support 1");
            if (u >= 65) throw new RedisException("Maximum support 64");
            this.Write($"i{u}");
            return this;
        }

        IBitFieldArgOffset<IBitFieldArgIncrement> IBitFieldArgEncoding<IBitFieldArgOffset<IBitFieldArgIncrement>>.Unsigned(uint u)
        {
            if (u <= 0) throw new RedisException("Minimum support 1");
            if (u >= 64) throw new RedisException("Maximum support 63");
            this.Write($"u{u}");
            return this;
        }

        IBitFieldArgIncrement IBitFieldArgOffset<IBitFieldArgIncrement>.Offset(uint offset)
        {
            this.Write(offset.ToString());
            return this;
        }

        IBitFieldArgIncrement IBitFieldArgOffset<IBitFieldArgIncrement>.MultipliedOffset(uint offset)
        {
            this.Write($"#{offset}");
            return this;
        }

        IBitFieldArg IBitFieldArgIncrement.Increment(long increment)
        {
            this.Write(increment.ToString());
            return this;
        }
        #endregion

        string[] IBitFieldArg.Convert() => this._args;

        private void Write(string arg)
        {
            this._args[this._currentIndex++] = arg;
        }
    }
}
