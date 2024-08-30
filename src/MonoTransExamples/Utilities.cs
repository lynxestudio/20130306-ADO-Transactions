using System;

namespace MonoTransExamples
{
    internal static class Utilities
    {
        internal static void SetTitle(string title)
        {
            Console.WriteLine("=====================================================");
            Console.WriteLine("\t" + title);
            Console.WriteLine("=====================================================");
        }

        internal static string Scanf(string message)
        {
            Console.Write("\t[ " + message + " ]\t");
            return Console.ReadLine();
        }

        internal static void PrintLabel(string text, string value)
        {
            Console.WriteLine("\t" + text + " [ " + value + " ] ");
        }

        internal static void PrintMessage(string message)
        {
            Console.WriteLine();
            Console.WriteLine("\t+--------------------------------------+");
            Console.WriteLine("\t|" + message);
            Console.WriteLine("\t+--------------------------------------+");
            Console.WriteLine();
        }

        internal static void DisplayMenu(string[] collection)
        {
            int i = 0;
            Console.WriteLine("\n");
            foreach (var item in collection)
            {
                i++;
                Console.WriteLine("\t" + i + ") " + item);
            }
            Console.WriteLine("\t0) Exit");
            Console.WriteLine();
        }

        internal static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("\tPress [Enter] to continue...");
            Console.ReadLine();
        }
    }
}
