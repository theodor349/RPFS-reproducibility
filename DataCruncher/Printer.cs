using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataCruncher
{
    internal static class Printer
    {
        private static ConsoleColor HeaderColor = ConsoleColor.Green;
        private static ConsoleColor HighlightColor = ConsoleColor.Yellow;
        private static bool shouldPrint = true;

        public static void Start()
        {
            shouldPrint = true;
        }

        public static void Pause()
        {
            shouldPrint = false;
        }

        public static void PrintHeader(string message)
        {
            if (!shouldPrint)
                return;

            Console.ForegroundColor = HeaderColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void PrintLine(string message)
        {
            if (!shouldPrint)
                return;
            Console.WriteLine(message);
        }

        public static void PrintLine(string composite, object value)
        {
            PrintLine(composite, new object[] { value });
        }

        public static void PrintLine(string composite, object value1, object value2)
        {
            PrintLine(composite, new object[] { value1, value2 });
        }

        public static void PrintLine(string composite, object value1, object value2, object value3)
        {
            PrintLine(composite, new object[] { value1, value2, value3 });
        }

        public static void PrintLine(string composite, object value1, object value2, object value3, object value4)
        {
            PrintLine(composite, new object[] { value1, value2, value3, value4 });
        }

        public static void PrintLine(string composite, object[] values)
        {
            if (!shouldPrint)
                return;
            var splits = Regex.Split(composite, "{}");
            for (int i = 0; i < splits.Length - 1; i++)
            {
                Console.Write(splits[i]);
                Console.ForegroundColor = HighlightColor;
                Console.Write(values[i]);
                Console.ResetColor();
            }
            Console.WriteLine(splits.Last());
        }

        internal static void PrintSpace()
        {
            if (!shouldPrint)
                return;
            Console.WriteLine();
        }
    }
}
