namespace SharpRedis;

public sealed partial class RedisBitmap
{
    public interface IBitFieldArgOffset<T>
    {
        /// <summary>
        /// Set offset
        /// <para>设置偏移量</para>
        /// </summary>
        /// <param name="offset">offset</param>
        /// <param name="isMultiplied">If the offset is prefixed with a # character, the specified offset is multiplied by the integer encoding's width
        /// <para>偏移是否要乘以编码宽度, 默认按位偏移</para>
        /// </param>
        /// <returns></returns>
        T Offset(uint offset, bool isMultiplied = false);
    }
}
