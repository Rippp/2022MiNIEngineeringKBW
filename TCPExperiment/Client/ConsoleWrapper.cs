using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class ConsoleWrapper
    {
        public static void WriteError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error] {errorMessage}");
            Console.ResetColor();
        }

        public static void WriteInfo(string infoMessage)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"[Info] {infoMessage}");
            Console.ResetColor();
        }
    }
}
