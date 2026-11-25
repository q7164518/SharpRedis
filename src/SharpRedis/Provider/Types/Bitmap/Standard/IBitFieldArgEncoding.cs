namespace SharpRedis;

public sealed partial class RedisBitmap
{
    public interface IBitFieldArgEncoding<T>
    {
        /// <summary>
        ///  Use unsigned integer encoding
        /// <para>使用无符号整数编码</para>
        /// </summary>
        /// <param name="bits">The number of occupied bits, ranging from 1 to 64
        /// <para>占用 bit 数量, 范围1 ~ 63</para>
        /// </param>
        /// <returns></returns>
        T UEncoding(byte bits);

        /// <summary>
        /// Use signed integer encoding
        /// <para>使用有符号整数编码</para>
        /// </summary>
        /// <param name="bits">The number of occupied bits, ranging from 1 to 64
        /// <para>占用 bit 数量, 范围1 ~ 64</para>
        /// </param>
        /// <returns></returns>
        T IEncoding(byte bits);
    }
}
