namespace SharpRedis;

public sealed partial class RedisBitmap
{
    public interface IBitFieldArgValue
    {
        /// <summary>
        /// Write number value
        /// <para>写入数值</para>
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        BitFieldArg Value(long value);
    }
}
