using System;
using System.IO;

namespace Catalyst
{
    public static class Debug
    {
        private static StreamWriter _logStream;

        static Debug()
        {
            _logStream = File.CreateText( $"log-{DateTime.Now:s}.txt");
        }
        
        public static void Log(string text)
        {
#if DEBUG
            Console.WriteLine(text);
#endif
            _logStream.WriteLine($"[{DateTime.Now:s}] {text}");
            Flush();
        }

        public static void Flush() => _logStream.Flush();
    }
}