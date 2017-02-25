using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Solution
{
    static void Main(string[] args)
    {
        string MESSAGE = Console.ReadLine();

        byte[] bytes = Encoding.ASCII.GetBytes(MESSAGE);
        var S = string.Join("", bytes.Select(b =>
        {
            var s = Convert.ToString(b, 2);
            var missingZeros = 7 - s.Length;
            
            s = (missingZeros==0)? s : (new int[missingZeros]).Select(x => "0").Aggregate((x, y) => x + y) + s;
            return s;
        }));

        var res = "";
        char cPrev = '2';
        foreach (var c in S)
        {
            if (c == '0' && c != cPrev)
                res += " 00 0";
            else if (c == '0' && c == cPrev)
                res += "0";
            else if (c == '1' && c != cPrev)
                res += " 0 0";
            else if (c == '1' && c == cPrev)
                res += "0";
            cPrev = c;
        }

        Console.WriteLine(res.Trim());
    }
}