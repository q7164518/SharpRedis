#pragma warning disable IDE0130

namespace SharpRedis
{
    public interface IBitFieldArgEncoding<T>
    {
        /// <summary>
        /// Writes an unsigned integer
        /// <para>写入无符号整数</para>
        /// </summary>
        /// <param name="u">number</param>
        /// <returns></returns>
        T Unsigned(uint u);

        /// <summary>
        /// Writes an signed integer
        /// <para>写入有符号整数</para>
        /// </summary>
        /// <param name="u">number</param>
        /// <returns></returns>
        T Signed(uint u);
    }
}
