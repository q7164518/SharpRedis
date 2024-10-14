#pragma warning disable IDE0130

namespace SharpRedis
{
    public interface IBitFieldArgOffset<T>
    {
        /// <summary>
        /// Set offset
        /// <para>设置偏移量</para>
        /// </summary>
        /// <param name="offset">offset</param>
        /// <returns></returns>
        T Offset(uint offset);

        /// <summary>
        /// if the offset is prefixed with a # character, the specified offset is multiplied by the integer encoding's width
        /// <para>指定的偏移量将乘以整数编码的宽度</para>
        /// </summary>
        /// <param name="offset">offset</param>
        /// <returns></returns>
        T MultipliedOffset(uint offset);
    }
}
