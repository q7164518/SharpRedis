#if LOW_NET
#pragma warning disable IDE0130
namespace System.Collections.StructuralComparisons
{
    internal static class StructuralEqualityComparer
    {
        static internal bool Equals(byte[] array1, byte[] array2)
        {
            if (array1 == array2)  return true;

            if (array1 == null || array2 == null || array1.Length != array2.Length)
                return false;

            for (uint i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }
    }
}
#endif
