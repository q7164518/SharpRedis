using System;

namespace SharpRedis.Extensions
{
    internal static class SharpConsole
    {
        private static readonly object _lock = new object();

        internal static void WriteError(string errorMessage)
        {
            lock (SharpConsole._lock)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Error: ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}. {errorMessage}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        internal static void WriteInfo(string message)
        {
            lock (SharpConsole._lock)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Info: ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}. {message}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        internal static void WriteWarning(string warningMessage)
        {
            lock (SharpConsole._lock)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("Warning: ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}. {warningMessage}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
