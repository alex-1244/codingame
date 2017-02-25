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
        var a = Console.ReadLine();
        Console.Error.WriteLine(a);
        inputs = a.Split(' ');
        int N = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        nodes = new int[N, N];
        gateways = new int[N];

        int L = int.Parse(inputs[1]); // the number of links
        int E = int.Parse(inputs[2]); // the number of exit gateways
        for (int i = 0; i < L; i++)
        {
            a = Console.ReadLine();
            Console.Error.WriteLine(a);
            inputs = a.Split(' ');
            int N1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            int N2 = int.Parse(inputs[1]);
            nodes[N1, N2] = 1;
            nodes[N2, N1] = 1;
        }
        for (int i = 0; i < E; i++)
        {
            a = Console.ReadLine();
            Console.Error.WriteLine(a);
            int EI = int.Parse(a); // the index of a gateway node
            gateways[EI] = 1;
        }

        // game loop
        List<ClosestNodeWithDist> removedNodes = new List<ClosestNodeWithDist>();
        while (true)
        {
            a = Console.ReadLine();
            Console.Error.WriteLine(a);
            int SI = int.Parse(a); // The index of the node on which the Skynet agent is positioned this turn

            ClosestNodeWithDist closestNode = new ClosestNodeWithDist { dist = int.MaxValue };
            
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                if (gateways[i] != 0)
                {
                    var newclosestNodeWithDist = GetClosestNodeWithDist(SI, i);
                    if (newclosestNodeWithDist.dist < closestNode.dist)
                    {
                        closestNode = newclosestNodeWithDist;
                        removedNodes.Add(new ClosestNodeWithDist { y = closestNode.y, x = closestNode.x, dist = closestNode.dist });
                    }
                }
            }
            nodes[closestNode.x, closestNode.y] = 0;
            nodes[closestNode.y, closestNode.x] = 0;
            Console.WriteLine($"{closestNode.x} {closestNode.y}");
        }
    }

    private static ClosestNodeWithDist GetClosestNodeWithDist(int startNode, int exitNode)
    {
        Queue<int> queue = new Queue<int>();
        int[] distToNode = new int[nodes.GetLength(0)].Select(x => int.MaxValue).ToArray();
        distToNode[startNode] = 0;

        int[] parentNodes = new int[nodes.GetLength(0)];

        int currNode = startNode;
        queue.Enqueue(currNode);
        while (currNode != exitNode)
        {
            if (queue.Count > 0)
                currNode = queue.Dequeue();
            else
                return new ClosestNodeWithDist { dist = int.MaxValue };
            for (int j = 0; j < nodes.GetLength(0); j++)
            {
                if (nodes[currNode, j] == 1)
                {
                    if (distToNode[j] > distToNode[currNode] + 1)
                    {
                        queue.Enqueue(j);
                        distToNode[j] = distToNode[currNode] + 1;
                        parentNodes[j] = currNode;
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