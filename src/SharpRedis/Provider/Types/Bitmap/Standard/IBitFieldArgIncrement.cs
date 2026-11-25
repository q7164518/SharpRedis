namespace SharpRedis;

public sealed partial class RedisBitmap
{
    public interface IBitFieldArgIncrement
    {
        /// <summary>
        /// Write number increment
        /// <para>写入自增值</para>
        /// </summary>
        /// <param name="increment">increment
        /// <para>要自增的数值</para>
        /// </param>
        /// <returns></returns>
        BitFieldArg Increment(long increment);
    }
}
