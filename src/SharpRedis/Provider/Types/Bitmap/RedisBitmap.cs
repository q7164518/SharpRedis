#if !LOW_NET
using System.Threading;
#else
#pragma warning disable IDE0130
#endif
using SharpRedis.Commands;
using SharpRedis.Provider.Standard;

namespace SharpRedis
{
    /// <summary>
    /// Redis Bitmap
    /// <para>Redis Bitmap类型操作类</para>
    /// </summary>
    public sealed partial class RedisBitmap : BaseType
    {
        internal RedisBitmap(BaseCall call) : base(call)
        {
        }

        /// <summary>
        /// Sets or clears the bit at offset in the string value stored at key
        /// <para>Available since: 2.2.0</para>
        /// <para>给一个指定Key的第offset位赋值</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="offset">offset
        /// <para>偏移位</para>
        /// </param>
        /// <param name="value">value
        /// <para>值, 只能是true或false</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The original bit value stored at offset
        /// <para>偏移处存储的原始位值</para>
        /// </returns>
        public bool SetBit(string key, uint offset, bool value, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(BitmapCommands.SetBit(key, offset, value), "1", cancellationToken);
        }

        /// <summary>
        /// Sets or clears the bit at offset in the string value stored at key
        /// <para>Available since: 2.2.0</para>
        /// <para>给一个指定Key的第offset位赋值</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="offset">offset
        /// <para>偏移位</para>
        /// </param>
        /// <param name="value">value
        /// <para>值, 只能是true或false</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The original bit value stored at offset
        /// <para>偏移处存储的原始位值</para>
        /// </returns>
        public bool SetBit(string key, long offset, bool value, CancellationToken cancellationToken = default)
        {
            if (offset < 0) throw new RedisException($"The offset passed in is {offset}, offset cannot be less than 0");
            if (offset > uint.MaxValue) throw new RedisException($"The offset passed in is {offset}, the maximum value of offset is {uint.MaxValue}");
            return this.SetBit(key, (uint)offset, value, cancellationToken);
        }

        /// <summary>
        /// Returns the bit value at offset in the string value stored at key
        /// <para>Available since: 2.2.0</para>
        /// <para>根据偏移位获取值</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="offset">offset
        /// <para>偏移位</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The bit value stored at offset
        /// <para>存储在此偏移位的值</para>
        /// </returns>
        public bool GetBit(string key, uint offset, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(BitmapCommands.GetBit(key, offset), "1", cancellationToken);
        }

        /// <summary>
        /// Returns the bit value at offset in the string value stored at key
        /// <para>Available since: 2.2.0</para>
        /// <para>根据偏移位获取值</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="offset">offset
        /// <para>偏移位</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The bit value stored at offset
        /// <para>存储在此偏移位的值</para>
        /// </returns>
        public bool GetBit(string key, long offset, CancellationToken cancellationToken = default)
        {
            if (offset < 0) throw new RedisException($"The offset passed in is {offset}, offset cannot be less than 0");
            if (offset > uint.MaxValue) throw new RedisException($"The offset passed in is {offset}, the maximum value of offset is {uint.MaxValue}");
            return this.GetBit(key, (uint)offset, cancellationToken);
        }

        /// <summary>
        /// Return the position of the first bit set to 1 or 0 in a string
        /// <para>Available since: 2.8.7</para>
        /// <para>返回指定Key中第一个0或1的位置(offset). bit参数指定查找0还是1</para>
        /// <para>支持此命令的Redis版本, 2.8.7+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="bit">Bit value, 1 is true, 0 is false
        /// <para>查找的值, false表示0, true表示1</para>
        /// </param>
        /// <param name="start">Index of the start. It can contain negative values in order to index bytes starting from the end of the string
        /// <para>开始查找的位置, 可以不传. 也可以为负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="end">Index of the end. It can contain negative values in order to index bytes starting from the end of the string
        /// <para>结束查找的位置, 可以不传. 也可以为负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The position of the first bit set to 1 or 0 according to the request
        /// <para>-1. In case the bit argument is 1 and the string is empty or composed of just zero bytes</para>
        /// <para>对应第一个0或1的位置</para>
        /// <para>如果Key是空的, 或Key不存在, 或者bit为1, 但Key里面没有1, 都返回-1</para>
        /// </returns>
        public long BitPos(string key, bool bit, long? start = null, long? end = null, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(BitmapCommands.BitPos(key, bit, start, end), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Return the position of the first bit set to 1 or 0 in a string
        /// <para>Available since: 7.0.0</para>
        /// <para>返回指定Key中第一个0或1的位置(offset). bit参数指定查找0还是1</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="bit">Bit value, 1 is true, 0 is false
        /// <para>查找的值, false表示0, true表示1</para>
        /// </param>
        /// <param name="start">Index of the start. It can contain negative values in order to index bytes starting from the end of the string
        /// <para>开始查找的位置, 可以不传. 也可以为负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="end">Index of the end. It can contain negative values in order to index bytes starting from the end of the string
        /// <para>结束查找的位置, 可以不传. 也可以为负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="bb">Specifies whether to search by byte or bit
        /// <para>When BIT is specified, -1 is the last bit, -2 is the penultimate, and so forth</para>
        /// <para>When BYTE is specified, -1 is the last byte, -2 is the penultimate, and so forth</para>
        /// <para>指定按字节还是按比特位查找</para>
        /// <para>当指定为BIT时, -1是倒数第一位, -2是倒数第二位</para>
        /// <para>当指定为BYTE时, -1是倒数第一个字节, -2是倒数第二字节</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The position of the first bit set to 1 or 0 according to the request
        /// <para>-1. In case the bit argument is 1 and the string is empty or composed of just zero bytes</para>
        /// <para>对应第一个0或1的位置</para>
        /// <para>如果Key是空的, 或Key不存在, 或者bit为1, 但Key里面没有1, 都返回-1</para>
        /// </returns>
        public long BitPos(string key, bool bit, ByteBit bb, long? start = null, long? end = null, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(BitmapCommands.BitPos(key, bit, start, end, bb), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Perform a bitwise operation between multiple keys (containing string values) and store the result in the destination key.
        /// <para>Available since: 2.6.0</para>
        /// <para>在多个Key(包含字符串值)之间执行按位操作，并将结果存储在目标Key中</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="bitOperation">The BITOP command supports four bitwise operations: AND, OR, XOR and NOT
        /// <para>BITOP命令支持AND、OR、XOR和NOT四种按位操作</para>
        /// </param>
        /// <param name="destkey">The destination key
        /// <para>保存结果的目标Key</para>
        /// </param>
        /// <param name="keys">Keys
        /// <para>参与计算的Key数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The size of the string stored in the destination key is equal to the size of the longest input string
        /// <para>保存在目标Key的字符串长度</para>
        /// </returns>
        public long BitOp(BitOperation bitOperation, string destkey, string[] keys, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(BitmapCommands.BitOp(bitOperation, destkey, keys), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Count the number of set bits (population counting) in a string
        /// <para>Available since: 2.6.0</para>
        /// <para>统计指定Key中的位数, 可指定开始和结束位置</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="start">Index of the start. It can contain negative values in order to index bytes starting from the end of the string
        /// <para>开始查找的位置, 可以不传. 也可以为负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="end">Index of the end. It can contain negative values in order to index bytes starting from the end of the string
        /// <para>结束查找的位置, 可以不传. 也可以为负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of set bits in the string. Non-existent keys are treated as empty strings and will return zero
        /// <para>字符串中设置的位数, 不存在的Key或空字符串返回0</para>
        /// </returns>
        public long BitCount(string key, long? start = null, long? end = null, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(BitmapCommands.BitCount(key, start, end), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Count the number of set bits (population counting) in a string
        /// <para>Available since: 7.0.0</para>
        /// <para>统计指定Key中的位数, 可指定开始和结束位置</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="bb">Offset mode
        /// <para>When BIT is specified, -1 is the last bit, -2 is the penultimate, and so forth</para>
        /// <para>When BYTE is specified, -1 is the last byte, -2 is the penultimate, and so forth</para>
        /// <para>Offset mode</para>
        /// <para>当指定为BIT时, -1是倒数第一位, -2是倒数第二位</para>
        /// <para>当指定为BYTE时, -1是倒数第一个字节, -2是倒数第二字节</para>
        /// </param>
        /// <param name="start">Index of the start. It can contain negative values in order to index bytes starting from the end of the string
        /// <para>开始查找的位置, 可以不传. 也可以为负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="end">Index of the end. It can contain negative values in order to index bytes starting from the end of the string
        /// <para>结束查找的位置, 可以不传. 也可以为负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of set bits in the string. Non-existent keys are treated as empty strings and will return zero
        /// <para>字符串中设置的位数, 不存在的Key或空字符串返回0</para>
        /// </returns>
        public long BitCount(string key, ByteBit bb, long? start = null, long? end = null, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(BitmapCommands.BitCount(key, start, end, bb), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// The command treats a Redis string as an array of bits, and is capable of addressing specific integer fields of varying bit widths and arbitrary non (necessary) aligned offset
        /// <para>Available since: 3.2.0</para>
        /// <para>该命令将Redis字符串视为位数组，并且能够寻址不同位宽度的特定整数字段和任意非(必要)对齐偏移量</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="arg">BITFIELD arg. The creation method is as follows:
        /// <para>BITFIELD命令参数, 创建方式如下</para>
        /// <para>GET u6 1 -> BitFieldArg.CreateGet().Unsigned(6).Offset(1)</para>
        /// <para>SET i6 #1 1 -> BitFieldArg.CreateSet().Signed(6).MultipliedOffset(1).Value(1)</para>
        /// <para>OVERFLOW WRAP -> BitFieldArg.CreateOverflow().Wrap()</para>
        /// <para>INCRBY u2 #1 1 -> BitFieldArg.CreateIncrement().Unsigned(2).MultipliedOffset(1).Increment(1)</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Each entry being the corresponding result of the sub-command given at the same position
        /// <para>Return null if OVERFLOW FAIL is given and an overflow or underflow is detected</para>
        /// <para>每个条目是同一位置给出的子命令的相应结果</para>
        /// <para>如果给定OVERFLOW FAIL并且检测到溢出或下溢，则返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public long?[]? BitField(string key, IBitFieldArg arg, CancellationToken cancellationToken = default)
#else
        public long?[] BitField(string key, IBitFieldArg arg, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return this.BitField(key, [arg] , cancellationToken);
#else
            return this.BitField(key, new IBitFieldArg[] { arg }, cancellationToken);
#endif
        }

        /// <summary>
        /// The command treats a Redis string as an array of bits, and is capable of addressing specific integer fields of varying bit widths and arbitrary non (necessary) aligned offset
        /// <para>Available since: 3.2.0</para>
        /// <para>该命令将Redis字符串视为位数组，并且能够寻址不同位宽度的特定整数字段和任意非(必要)对齐偏移量</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="args">BITFIELD args array. The creation method is as follows:
        /// <para>BITFIELD命令参数数组, 创建方式如下</para>
        /// <para>GET u6 1 -> BitFieldArg.CreateGet().Unsigned(6).Offset(1)</para>
        /// <para>SET i6 #1 1 -> BitFieldArg.CreateSet().Signed(6).MultipliedOffset(1).Value(1)</para>
        /// <para>OVERFLOW WRAP -> BitFieldArg.CreateOverflow().Wrap()</para>
        /// <para>INCRBY u2 #1 1 -> BitFieldArg.CreateIncrement().Unsigned(2).MultipliedOffset(1).Increment(1)</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Each entry being the corresponding result of the sub-command given at the same position
        /// <para>Return null if OVERFLOW FAIL is given and an overflow or underflow is detected</para>
        /// <para>每个条目是同一位置给出的子命令的相应结果</para>
        /// <para>如果给定OVERFLOW FAIL并且检测到溢出或下溢，则返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public long?[]? BitField(string key, IBitFieldArg[] args, CancellationToken cancellationToken = default)
#else
        public long?[] BitField(string key, IBitFieldArg[] args, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallNullableStructArray<long>(BitmapCommands.BitField(key, args), ResultType.Array | ResultType.Int64 | ResultType.Nullable, cancellationToken);
        }

        /// <summary>
        /// Read-only variant of the BITFIELD command. It is like the original BITFIELD but only accepts GET subcommand and can safely be used in read-only replicas
        /// <para>Available since: 6.0.0</para>
        /// <para>BITFIELD命令的只读变体。它类似于原始的 BITFIELD 但仅接受 GET 子命令，并且可以安全地在只读副本中使用</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="gets">GET args array
        /// <para>GET参数数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Each entry being the corresponding result of the sub-command given at the same position
        /// <para>每个条目是在同一位置给出的子命令的相应结果</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public long[]? BitField_Ro(string key, IBitFieldArg[] gets, CancellationToken cancellationToken = default)
#else
        public long[] BitField_Ro(string key, IBitFieldArg[] gets, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<long>(BitmapCommands.BitField_Ro(key, gets), ResultType.Array | ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Read-only variant of the BITFIELD command. It is like the original BITFIELD but only accepts GET subcommand and can safely be used in read-only replicas
        /// <para>Available since: 6.0.0</para>
        /// <para>BITFIELD命令的只读变体。它类似于原始的 BITFIELD 但仅接受 GET 子命令，并且可以安全地在只读副本中使用</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="get">GET arg
        /// <para>GET参数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Each entry being the corresponding result of the sub-command given at the same position
        /// <para>每个条目是在同一位置给出的子命令的相应结果</para>
        /// </returns>
        public long BitField_Ro(string key, IBitFieldArg get, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            return base._call.CallNumber<long>(BitmapCommands.BitField_Ro(key, [get]), ResultType.Int64, cancellationToken);
#else
            return base._call.CallNumber<long>(BitmapCommands.BitField_Ro(key, new IBitFieldArg[] { get }), ResultType.Int64, cancellationToken);
#endif
        }
    }
}
