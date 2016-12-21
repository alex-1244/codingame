using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCII_Art
{
    class Solution
    {
        const string pattern = "ABCDEFGHIJKLMNOPQRSTUVWXYZ?";
        static int L;
        static int H;
        static string T;

        static void Main(string[] args)
        {
            L = int.Parse(Console.ReadLine());
            H = int.Parse(Console.ReadLine());
            T = Console.ReadLine();
            List<string> rows = new List<string>();
            List<string> outRows = new List<string>();

            for (int i = 0; i < H; i++)
            {
                Console.WriteLine(GetLine(Console.ReadLine()));
            }

        }

        static string GetLine(string asciiLine)
        {
            string res = "";
            foreach (var c in T)
            {
                var index = pattern.IndexOf(Char.ToUpper(c));
                if (index == -1)
                    index = pattern.Length - 1;
                var charLine = asciiLine.Substring(index * L, L);
                res += charLine;
            }

            return res;
        }
    }
}
