#pragma warning disable IDE0130

namespace SharpRedis
{
    public interface IBitFieldArgValue
    {
        /// <summary>
        /// Write number value
        /// <para>写入数值</para>
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        IBitFieldArg Value(long value);
    }
}
