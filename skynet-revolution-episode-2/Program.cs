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
        var inputLine = Console.ReadLine();
        Console.Error.WriteLine(inputLine);
        inputs = inputLine.Split(' ');
        int nodesNum = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        nodes = new int[nodesNum, nodesNum];
        gateways = new int[nodesNum];

        int linkesNum = int.Parse(inputs[1]); // the number of links
        int exitsNum = int.Parse(inputs[2]); // the number of exit gateways
        for (int i = 0; i < linkesNum; i++)
        {
            inputLine = Console.ReadLine();
            Console.Error.WriteLine(inputLine);
            inputs = inputLine.Split(' ');
            int N1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            int N2 = int.Parse(inputs[1]);
            nodes[N1, N2] = 1;
            nodes[N2, N1] = 1;
        }

        for (int i = 0; i < exitsNum; i++)
        {
            inputLine = Console.ReadLine();
            Console.Error.WriteLine(inputLine);
            int EI = int.Parse(inputLine); // the index of a gateway node
            gateways[EI] = 1;
        }

        // game loop
        //List<ClosestNodeWithDist> removedNodes = new List<ClosestNodeWithDist>();
        
        while (true)
        {
            var closestNodes = new List<ClosestNodeWithDist>();
            inputLine = Console.ReadLine();
            Console.Error.WriteLine(inputLine);
            int skynetIndex = int.Parse(inputLine); // The index of the node on which the Skynet agent is positioned this turn

            ClosestNodeWithDist closestNode = new ClosestNodeWithDist { dist = int.MaxValue };

            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                if (gateways[i] != 0)
                {
                    var newclosestNodeWithDist = GetClosestNodeWithDist(skynetIndex, i);
                    closestNodes.Add(newclosestNodeWithDist);

                    if (newclosestNodeWithDist.dist < closestNode.dist)
                    {
                        closestNode = newclosestNodeWithDist;
                        //removedNodes.Add(new ClosestNodeWithDist { y = closestNode.y, x = closestNode.x, dist = closestNode.dist });
                    }
                }
            }

            closestNodes = closestNodes.OrderBy(x => x.dist).ToList();

            if (closestNodes[0].dist == 1)
            {
                nodes[closestNode.x, closestNode.y] = 0;
                nodes[closestNode.y, closestNode.x] = 0;
                Console.WriteLine($"{closestNode.x} {closestNode.y}");
                continue;
            }
            else
            {
                var breaked = false;
                for (int i = 0; i < closestNodes.Count - 1; i++)
                {
                    if (closestNodes[i].x == closestNodes[i + 1].x)
                    {
                        nodes[closestNodes[i].x, closestNodes[i].y] = 0;
                        nodes[closestNodes[i].y, closestNodes[i].x] = 0;
                        Console.WriteLine($"{closestNodes[i].x} {closestNodes[i].y}");
                        breaked = true;
                        break;
                    }
                }
                if (breaked)
                {
                    continue;
                }
            }

            Console.Error.WriteLine("Dist: " + closestNode.dist);

            nodes[closestNode.x, closestNode.y] = 0;
            nodes[closestNode.y, closestNode.x] = 0;
            Console.WriteLine($"{closestNode.x} {closestNode.y}");
        }
    }

    private static ClosestNodeWithDist GetClosestNodeWithDist(int startNode, int exitNode)
    {
        Queue<int> queue = new Queue<int>();
        int[] distToNode = Enumerable.Range(0, nodes.GetLength(0)).Select(x => int.MaxValue).ToArray();
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