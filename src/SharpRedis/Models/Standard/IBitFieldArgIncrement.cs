#pragma warning disable IDE0130

namespace SharpRedis
{
    public interface IBitFieldArgIncrement
    {
        /// <summary>
        /// Write number increment
        /// <para>写入自增值</para>
        /// </summary>
        /// <param name="increment">increment</param>
        /// <returns></returns>
        IBitFieldArg Increment(long increment);
    }
}
