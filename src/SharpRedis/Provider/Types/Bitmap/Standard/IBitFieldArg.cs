namespace SharpRedis;

public sealed partial class RedisBitmap
{
    /// <summary>
    /// BitField args
    /// </summary>
    internal interface IBitFieldArg
    {
        /// <summary>
        /// Get operation type
        /// <para>获得参数操作类型</para>
        /// </summary>
        BitFieldOperation Operation { get; }

        /// <summary>
        /// Convert to args
        /// </summary>
        /// <returns></returns>
        string[] Convert();
    }
}
