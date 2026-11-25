using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis Bitmap
/// <para>Bitmap类型操作类(位图)</para>
/// <para>If you are new to bitmaps, see <see href="https://redis.io/docs/develop/data-types/strings/bitmaps">Introduction to Redis bitmaps</see></para>
/// <para>如果你不太懂bitmaps类型, 请<see href="https://redis.io/docs/develop/data-types/strings/bitmaps">查看bitmaps文档</see></para>
/// </summary>
public sealed partial class RedisBitmap : RedisTypeBase
{
    #region enums
    /// <summary>
    /// Bitmap unit enum
    /// <para>Available since: 7.0.0</para>
    /// <para>位图单位枚举</para>
    /// <para>支持此参数的Redis版本, 7.0.0+</para>
    /// </summary>
    public enum BitUnit : byte
    {
        /// <summary>
        /// No specification
        /// <para>不指定</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Using bytes as units
        /// <para>Available since: 7.0.0</para>
        /// <para>使用字节作为单位</para>
        /// <para>支持此参数的Redis版本, 7.0.0+</para>
        /// </summary>
        Byte = 1,

        /// <summary>
        /// Using bits as units
        /// <para>Available since: 7.0.0</para>
        /// <para>使用位作为单位</para>
        /// <para>支持此参数的Redis版本, 7.0.0+</para>
        /// </summary>
        Bit = 2,
    }

    /// <summary>
    /// Enum of <b>BITOP</b> command operation methods
    /// <para><b>BITOP</b>命令操作方式枚举</para>
    /// </summary>
    public enum BitOperation : byte
    {
        /// <summary>
        /// A bit in destkey is set only if it is set in all source bitmaps.
        /// <para>所有源Key按位与</para>
        /// </summary>
        And = 0,

        /// <summary>
        /// A bit in destkey is set only if it is set in at least one source bitmap.
        /// <para>所有源Key按位或</para>
        /// </summary>
        Or = 1,

        /// <summary>
        /// Mostly used with two source bitmaps, a bit in destkey is set only if its value differs between the two source bitmaps.
        /// <para>所有源Key按位异或</para>
        /// </summary>
        Xor = 2,

        /// <summary>
        /// NOT is a unary operator and only supports a single source bitmap; set the bit to the inverse of its value in the source bitmap.
        /// <para>源Key按位取反. 此模式只支持传入一个源Key</para>
        /// </summary>
        Not = 3,

        /// <summary>
        /// <b>BITOP DIFF destkey X [Y1 Y2 ...]</b> A bit in destkey is set if it is set in X, but not in any of Y1, Y2, ...
        /// <para>Available since: 8.2.0</para>
        /// <para>计算位差集, 第一个源 key 中为 1，而其它源 key 中全部为 0 的 bit</para>
        /// <para>支持此命令的Redis版本, 8.2.0+</para>
        /// </summary>
        Diff = 4,

        /// <summary>
        /// <b>BITOP DIFF1 destkey X [Y1 Y2 ...]</b> A bit in destkey is set if it is set in one or more of Y1, Y2, ..., but not in X.
        /// <para>Available since: 8.2.0</para>
        /// <para>位差集取反. 第一个源 key 位是 0，但是其它源 key 位存在 1 的</para>
        /// <para>支持此命令的Redis版本, 8.2.0+</para>
        /// </summary>
        Diff1 = 5,

        /// <summary>
        /// <b>BITOP ANDOR destkey X [Y1 Y2 ...]</b> A bit in destkey is set if it is set in X and also in one or more of Y1, Y2, ....
        /// <para>Available since: 8.2.0</para>
        /// <para>所有源Key先按位与再按位或</para>
        /// <para>支持此命令的Redis版本, 8.2.0+</para>
        /// </summary>
        AndOr = 6,

        /// <summary>
        /// <b>BITOP ONE destkey X1 [X2 X3 ...]</b> A bit in destkey is set if it is set in exactly one of X1, X2, ....
        /// <para>Available since: 8.2.0</para>
        /// <para>所有源Key中, 有且只有其中一个Key的位为1的</para>
        /// <para>支持此命令的Redis版本, 8.2.0+</para>
        /// </summary>
        One = 7,
    }

    internal enum BitFieldOperation : byte
    {
        Get = 0,

        Set = 1,

        Overflow = 2,

        IncrBy = 3,
    }
    #endregion

    internal RedisBitmap(BaseRedis redis) : base(redis)
    { }

    /// <summary>
    /// Sets or clears the bit at offset in the string value stored at key.
    /// <para>Available since: 2.2.0</para>
    /// <para>设置指定位置的位值</para>
    /// <para>支持此命令的Redis版本, 2.2.0+</para>
    /// </summary>
    /// <param name="key">Bitmap key<para>位图的Key</para></param>
    /// <param name="offset">Offset<para>位置(偏移位 | 下标)</para></param>
    /// <param name="value">Value<para>值</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The original bit value stored at offset.
    /// <para>指定位置(偏移 | 下标)上的原始位值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SetBit">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SetBit">命令文档</see></para>
    /// </remarks>
    public bool SetBit(SharpValue key, uint offset, bool value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(BitmapCommands.SetBit(key, offset, value) | flags, cancellationToken);

    /// <summary>
    /// Gets the bit value at offset in the string value stored at key.
    /// <para>Available since: 2.2.0</para>
    /// <para>获得位图中, 指定位置(偏移 | 下标)上的位值</para>
    /// <para>支持此命令的Redis版本, 2.2.0+</para>
    /// </summary>
    /// <param name="key">Bitmap key<para>位图的Key</para></param>
    /// <param name="offset">Offset<para>位置(偏移位 | 下标)</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>0 --> false, 1 --> true</returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GetBit">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GetBit">命令文档</see></para>
    /// </remarks>
    public bool GetBit(SharpValue key, uint offset, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(BitmapCommands.GetBit(key, offset) | flags, cancellationToken);

    /// <summary>
    /// Count the number of set bits (population counting) in a string.
    /// <para>Available since: 2.6.0</para>
    /// <para>统计位图中, 位值为1的数量</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key">Bitmap key<para>位图的Key</para></param>
    /// <param name="start">Start can contain negative values in order to index bytes starting from the end of the string, where -1 is the last byte, -2 is the penultimate, and so forth.
    /// <para>起始位置, 可以为负数, -1表示最后一个字节, -2表示倒数第二个字节, 以此类推</para></param>
    /// <param name="end">End can contain negative values in order to index bytes starting from the end of the string, where -1 is the last byte, -2 is the penultimate, and so forth.
    /// <para>结束位置, 可以为负数, -1表示最后一个字节, -2表示倒数第二个字节, 以此类推</para>
    /// </param>
    /// <param name="unit">By default, the additional arguments start and end specify a byte index. We can use an additional argument BIT to specify a bit index. So 0 is the first bit, 1 is the second bit, and so forth. For negative values, -1 is the last bit, -2 is the penultimate, and so forth.
    /// <para><b>Available since: 7.0.0</b></para>
    /// <para>指定start和end索引的单位, 默认为按字节. 可以使用BIT参数指定按位索引</para>
    /// <para><b>支持此参数的Redis版本, 7.0.0+</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of bits set to 1.
    /// <para>位值为1的数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BitCount">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BitCount">命令文档</see></para>
    /// </remarks>
    public RedisNumber BitCount(SharpValue key, long? start = null, long? end = null, BitUnit unit = BitUnit.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(BitmapCommands.BitCount(key, start, end, unit) | flags, cancellationToken);

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Count the number of set bits (population counting) in a string.
    /// <para>Available since: 2.6.0</para>
    /// <para>统计位图中, 位值为1的数量</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key">Bitmap key<para>位图的Key</para></param>
    /// <param name="range">Start and end can contain negative values in order to index bytes starting from the end of the string, where -1 is the last byte, -2 is the penultimate, and so forth.
    /// <para>起始和结束位置, 可以为负数, -1表示最后一个字节, -2表示倒数第二个字节, 以此类推</para>
    /// <code>
    /// range = new Range(0, 10); // index 0 to index 10
    /// range = ..^1; // from the start to the last byte
    /// range = 1..5; // index 1 to index 5
    /// </code>
    /// </param>
    /// <param name="unit">By default, the additional arguments start and end specify a byte index. We can use an additional argument BIT to specify a bit index. So 0 is the first bit, 1 is the second bit, and so forth. For negative values, -1 is the last bit, -2 is the penultimate, and so forth.
    /// <para><b>Available since: 7.0.0</b></para>
    /// <para>指定start和end索引的单位, 默认为按字节. 可以使用BIT参数指定按位索引</para>
    /// <para><b>支持此参数的Redis版本, 7.0.0+</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of bits set to 1.
    /// <para>位值为1的数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BitCount">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BitCount">命令文档</see></para>
    /// </remarks>
    public RedisNumber BitCount(SharpValue key, global::System.Range range, BitUnit unit = BitUnit.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(BitmapCommands.BitCount(key, range, unit) | flags, cancellationToken);
#endif

    /// <summary>
    /// The command treats a Redis string as an array of bits, and is capable of addressing specific integer fields of varying bit widths and arbitrary non (necessary) aligned offset.
    /// <para>Available since: 3.2.0</para>
    /// <para>该命令将Redis字符串视为位数组，并且能够寻址不同位宽度的特定整数字段和任意非(必要)对齐偏移量</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="args">BITFIELD args array. The creation method is as follows:
    /// <para>BITFIELD子命令参数数组, 创建方式如下</para>
    /// <para>
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
    /// </para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Each entry being the corresponding result of the sub-command given at the same position
    /// <para>Return null if OVERFLOW FAIL is given and an overflow or underflow is detected</para>
    /// <para>跟子命令顺序对应的结果数组</para>
    /// <para>如果给定OVERFLOW FAIL并且检测到溢出或下溢，则返回null</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BitField">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BitField">命令文档</see></para>
    /// </remarks>
    public RedisNumber[] BitField(SharpValue key, ReadOnlySpan<BitFieldArg> args, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(BitmapCommands.BitField(key, args) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Read-only variant of the <b>BITFIELD</b> command. It is like the original BITFIELD but only accepts <i><b>GET</b></i> subcommand and can safely be used in read-only replicas.
    /// <para>Available since: 6.0.0</para>
    /// <para><b>BITFIELD</b>只读版本, 只支持<i><b>GET</b></i>参数</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="arg">BITFIELD_RO arg. The creation method is as follows:
    /// <para>BITFIELD_RO子命令参数, 创建方式如下</para>
    /// <para>
    /// <code>
    /// BitFieldArg.Get().IEncoding(8).Offset(1); // GET i8 1
    /// BitFieldArg.Get("u8", "1"); // GET i8 1
    /// 
    /// BitFieldArg.Get().IEncoding(8).Offset(1, true); // GET i8 #1
    /// BitFieldArg.Get("i8", "#1"); // GET i8 #1
    /// 
    /// BitFieldArg.Get().UEncoding(8).Offset(1, true); // GET u8 #1
    /// BitFieldArg.Get("u8", "#1"); // GET u8 #1
    /// </code>
    /// </para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>sub-command result
    /// <para>子命令的返回值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BitField_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BitField_Ro">命令文档</see></para>
    /// </remarks>
    public RedisNumber BitField_Ro(SharpValue key, BitFieldArg arg, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(BitmapCommands.BitField_Ro(key, arg) | flags, cancellationToken);
    }

    /// <summary>
    /// Read-only variant of the <b>BITFIELD</b> command. It is like the original BITFIELD but only accepts <i><b>GET</b></i> subcommand and can safely be used in read-only replicas.
    /// <para>Available since: 6.0.0</para>
    /// <para><b>BITFIELD</b>只读版本, 只支持<i><b>GET</b></i>参数</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="args">BITFIELD_RO args array. The creation method is as follows:
    /// <para>BITFIELD_RO子命令参数数组, 创建方式如下</para>
    /// <para>
    /// <code>
    /// BitFieldArg.Get().IEncoding(8).Offset(1); // GET i8 1
    /// BitFieldArg.Get("u8", "1"); // GET i8 1
    /// 
    /// BitFieldArg.Get().IEncoding(8).Offset(1, true); // GET i8 #1
    /// BitFieldArg.Get("i8", "#1"); // GET i8 #1
    /// 
    /// BitFieldArg.Get().UEncoding(8).Offset(1, true); // GET u8 #1
    /// BitFieldArg.Get("u8", "#1"); // GET u8 #1
    /// </code>
    /// </para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>sub-command result
    /// <para>子命令的返回值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BitField_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BitField_Ro">命令文档</see></para>
    /// </remarks>
    public RedisNumber[] BitField_Ro(SharpValue key, ReadOnlySpan<BitFieldArg> args, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(BitmapCommands.BitField_Ro(key, args) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Perform a bitwise operation between multiple keys (containing string values) and store the result in the destination key.
    /// <para>Available since: 2.6.0</para>
    /// <para>对多个源Key进行位运算, 并将结果存入目标Key中</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="op">The <b>BITOP</b> command supports eight bitwise operations: AND, OR, XOR, NOT, DIFF, DIFF1, ANDOR, and ONE.
    /// <para>位运算的方式, 支持AND, OR, XOR, NOT, DIFF, DIFF1, ANDOR, 和 ONE</para>
    /// </param>
    /// <param name="destkey">The key to store the result in.
    /// <para>保存结果的目标Key</para>
    /// </param>
    /// <param name="keys">One or more source keys.
    /// <para>一个或多个的源Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The size of the string stored in the destination key is equal to the size of the longest input string.
    /// <para>存入目标Key中的结果长度. 以字符串形式存在的长度</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BitOp">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BitOp">命令文档</see></para>
    /// </remarks>
    public long BitOp(BitOperation op, SharpValue destkey, SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(BitmapCommands.BitOp(op, destkey, keys) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the position of the first bit set to 1 or 0 in a string.
    /// <para>Available since: 2.8.7</para>
    /// <para>返回Key中第一个值为 1 或 0 的位的下标</para>
    /// <para>支持此命令的Redis版本, 2.8.7+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="bit">Bit value, 1 is true, 0 is false
    /// <para>查找的位值, false表示0, true表示1</para>
    /// </param>
    /// <param name="start">Index of the start. It can contain negative values in order to index bytes starting from the end of the string
    /// <para>开始查找的下标, 可以不传. 也可以为负数, -1表示倒数第一个, 以此类推</para>
    /// </param>
    /// <param name="end">Index of the end. It can contain negative values in order to index bytes starting from the end of the string
    /// <para>结束查找的下标, 可以不传. 也可以为负数, -1表示倒数第一个, 以此类推</para>
    /// </param>
    /// <param name="unit">Interpret start and end as byte offsets (BYTE, the default) or bit offsets (BIT).
    /// <para>将下标解释为按位偏移还是按字节偏移</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The position of the first bit set to 1 or 0 according to the request
    /// <para>-1. In case the bit argument is 1 and the string is empty or composed of just zero bytes</para>
    /// <para>对应第一个0或1的位置</para>
    /// <para>如果Key是空的, 或Key不存在, 或者bit为1, 但Key里面没有1, 都返回-1</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BitPos">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BitPos">命令文档</see></para>
    /// </remarks>
    public long BitPos(SharpValue key, bool bit, long? start = null, long? end = null, BitUnit unit = BitUnit.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(BitmapCommands.BitPos(key, bit, start, end, unit) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns set-bit offsets within a bitmap range in ascending order. Use ZEROS to return clear-bit offsets instead.
    /// <para>Available since: 8.12.0</para>
    /// <para>获取 Bitmap 指定范围内，所有值为 1 或 0 的 Bit 位置（偏移量）</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The key that holds a string or native bitmap.
    /// <para>位图的Key</para>
    /// </param>
    /// <param name="start">The first byte or bit offset in the range, inclusive. The default unit is BYTE. Negative indexes count from the end of the value
    /// <para>偏移开始位置. 单位默认为BYTE. 负数表示从末尾开始</para>
    /// </param>
    /// <param name="end">The last byte or bit offset in the range, inclusive. Negative indexes count from the end: -1 selects the final byte, or the final bit with BIT. Use 0 -1 to cover the whole value.
    /// <para>偏移结束位置, 负数表示从末尾开始</para>
    /// </param>
    /// <param name="unit">Interpret start and end as byte offsets (BYTE, the default) or bit offsets (BIT).
    /// <para>将偏移下标解释为按位偏移还是按字节偏移</para>
    /// </param>
    /// <param name="limit">Return the lowest matching offsets in the range. With BIT, the reply contains at most count offsets. With BYTE, the command also returns the remaining matching offsets in the byte containing the countth match. The reply can therefore contain up to count + 7 offsets.
    /// <para>返回范围内的最小匹配偏移量。使用 BIT 时，回复最多包含 count 偏移量。使用 BYTE 时，该命令还会返回包含 count 个匹配项的字节中剩余的匹配偏移量。</para>
    /// </param>
    /// <param name="zeros">Return clear-bit offsets instead of set-bit offsets within the range.
    /// <para>返回范围内的清除位偏移量，而不是置位位偏移量</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    public
#if MODERN_NET
    long[]?
#else
    long[]
#endif
    BitRangeIntArray(
        SharpValue key,
        long start,
        long end,
        RedisBitmap.BitUnit unit,
        long? limit,
        bool zeros,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(BitmapCommands.BitRangeIntArray(
            key: key,
            start: start,
            end: end,
            unit: unit,
            limit: limit,
            zeros: zeros) | flags, cancellationToken);
    }
}
