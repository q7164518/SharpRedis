#if LOW_NET || NET40 || NET45
#pragma warning disable IDE0130
namespace SharpRedis
{
    internal static class Array
    {
        public static T[] Empty<T>()
        {
            return EmptyArray<T>.Value;
        }
    }

    internal static class EmptyArray<T>
    {
        public static readonly T[] Value = new T[0];
    }
}
#endif
