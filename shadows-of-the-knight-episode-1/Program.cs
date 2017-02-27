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
class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        var a = Console.ReadLine();
        Console.Error.WriteLine(a);
        inputs = a.Split(' ');
        int W = int.Parse(inputs[0]); // width of the building.
        int H = int.Parse(inputs[1]); // height of the building.
        a = Console.ReadLine();
        Console.Error.WriteLine(a);
        int N = int.Parse(a); // maximum number of turns before game over.
        a = Console.ReadLine();
        Console.Error.WriteLine(a);
        inputs = a.Split(' ');
        int X0 = int.Parse(inputs[0]);
        int Y0 = int.Parse(inputs[1]);

        int maxJumpHigh = -1, maxJumpWidth = -1, currX = X0, currY = Y0;

        while (true)
        {
            //TODO: add history 
            //if direction down define the rang (x[n]-x[n-1])/2 or x[n]/2  OR  x[n] + (h - x[n]) / 2

            int x = 0, y = 0;
            string bombDir = Console.ReadLine(); // the direction of the bombs from batman's current location (U, UR, R, DR, D, DL, L or UL)
            Console.Error.WriteLine(bombDir);
            Console.Error.WriteLine(maxJumpHigh);
            Console.Error.WriteLine(maxJumpWidth);

            if (bombDir.IndexOf("U") > -1 || bombDir.IndexOf("D") > -1)
            {
                if (bombDir.IndexOf("U") > -1)
                {
                    x = currX;
                    y = maxJumpHigh == -1 ? (currY) / 2 : currY - maxJumpHigh;
                    maxJumpHigh = maxJumpHigh == -1 ? ((currY + 2) / 4) : (maxJumpHigh + 1) / 2;
                    //maxJumpHigh = maxJumpHigh / 2;
                    maxJumpHigh = maxJumpHigh <= 0 ? 1 : maxJumpHigh;
                }
                else if (bombDir.IndexOf("D") > -1)
                {
                    x = currX;
                    y = maxJumpHigh == -1 ? (currY + ((H - currY) / 2)) : currY + maxJumpHigh;
                    maxJumpHigh = maxJumpHigh == -1 ? ((H - currY + 2) / 4) : (maxJumpHigh + 1) / 2;
                    //maxJumpHigh = maxJumpHigh / 2;
                    maxJumpHigh = maxJumpHigh <= 0 ? 1 : maxJumpHigh;
                }
                if (y < 0)
                {
                    Console.Error.WriteLine("y<0");
                    y = 0;
                }
                if (y >= H)
                {
                    y = H - 1;
                    Console.Error.WriteLine("y>H");
                }
                currY = y;
            }
            if (bombDir.IndexOf("L") > -1 || bombDir.IndexOf("R") > -1)
            {
                if (bombDir.IndexOf("L") > -1)
                {
                    x = maxJumpWidth == -1 ? (currX) / 2 : currX - maxJumpWidth;
                    y = currY;
                    maxJumpWidth = maxJumpWidth == -1 ? (currX + 2) / 4 : (maxJumpWidth + 1) / 2;
                    //maxJumpWidth = maxJumpWidth / 2;
                    maxJumpWidth = maxJumpWidth == 0 ? 1 : maxJumpWidth;
                }
                else if (bombDir.IndexOf("R") > -1)
                {
                    x = maxJumpWidth == -1 ? (currX + ((W - currX) / 2)) : currX + maxJumpWidth;
                    y = currY;
                    maxJumpWidth = maxJumpWidth == -1 ? ((W - currX + 2) / 4) : (maxJumpWidth + 1) / 2;
                    //maxJumpWidth = maxJumpWidth / 2;
                    maxJumpWidth = maxJumpWidth == 0 ? 1 : maxJumpWidth;
                }
                if (x < 0)
                {
                    Console.Error.WriteLine("x<0");
                    x = 0;
                }
                if (x >= W)
                {
                    x = W - 1;
                    Console.Error.WriteLine("x>W");
                }
                currX = x;
            }

            Console.WriteLine($"{currX} {currY}");
        }
        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");


        // the location of the next window Batman should jump to.
        //Console.WriteLine("0 0");
    }
}