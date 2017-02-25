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
    static int[,] nodes;
    static int[] gateways;

    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int N = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        nodes = new int[N, N];
        gateways = new int[N];

        int L = int.Parse(inputs[1]); // the number of links
        int E = int.Parse(inputs[2]); // the number of exit gateways
        for (int i = 0; i < L; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int N1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            int N2 = int.Parse(inputs[1]);
            nodes[N1, N2] = 1;
        }
        for (int i = 0; i < E; i++)
        {
            int EI = int.Parse(Console.ReadLine()); // the index of a gateway node
            gateways[EI] = 1;
        }

        // game loop
        while (true)
        {
            int SI = int.Parse(Console.ReadLine()); // The index of the node on which the Skynet agent is positioned this turn

            var closestNodeWithDist = gateways.Where(x => x == 1).Select(x => GetClosestNodeWithDist(SI, x));
            var closestNode = closestNodeWithDist.First(x => x.dist == closestNodeWithDist.Min(y => y.dist));

            Console.WriteLine($"{closestNode.x} {closestNode.y}");
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");


            // Example: 0 1 are the indices of the nodes you wish to sever the link between
            //Console.WriteLine("0 1");
        }
    }

    private static ClosestNodeWithDist GetClosestNodeWithDist(int startNode, int exitNode)
    {
        Queue<int> queue = new Queue<int>();
        int[] distToNode = new int[nodes.GetLength(0)];
        int[] parentNodes = new int[nodes.GetLength(0)];

        int currNode = startNode;
        queue.Enqueue(currNode);
        while (currNode != exitNode)
        {
            currNode = queue.Dequeue();
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(0); j++)
                {
                    if (nodes[i, j] == 1)
                    {
                        queue.Enqueue(j);
                        if (distToNode[j] > distToNode[i] + 1)
                        {
                            distToNode[j] = distToNode[i] + 1;
                            parentNodes[j] = i;
                        }
                    }
                }
            }
        }
        return new ClosestNodeWithDist { y = exitNode, x = parentNodes[exitNode], dist = distToNode[exitNode] };
    }

    public class ClosestNodeWithDist
    {
        public int x;
        public int y;
        public int dist;
    }
}