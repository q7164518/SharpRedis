using System;
using System.Runtime.CompilerServices;

namespace SharpRedis.Extensions;

internal static class SharpConsole
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteError(string message)
    {
        Console.WriteLine($"\e[37;41m{message}\e[0m");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteWarning(string message)
    {
        Console.WriteLine($"\e[31;43m{message}\e[0m");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteSucceed(string message)
    {
        Console.WriteLine($"\e[30;42m{message}\e[0m");
    }
}
