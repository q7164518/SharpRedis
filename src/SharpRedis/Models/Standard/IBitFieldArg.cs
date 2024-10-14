#pragma warning disable IDE0130

namespace SharpRedis
{
    /// <summary>
    /// BitField args
    /// </summary>
    public interface IBitFieldArg
    {
        /// <summary>
        /// Get arg type
        /// <para>获得参数类型</para>
        /// </summary>
        BitFieldArgType ArgType { get; }

        /// <summary>
        /// Convert to args
        /// </summary>
        /// <returns></returns>
        string[] Convert();
    }
}
