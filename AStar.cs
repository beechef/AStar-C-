using System.Collections.Generic;
using UnityEngine;

namespace Algorithm
{
    public static class AStar
    {
        private static readonly Vector2Int[] Directions = new[]
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
            new Vector2Int(1, -1),
        };

        public static List<Vector2Int> Run(bool[,] matrix, Vector2Int startPos, Vector2Int endPos)
        {
            List<Node> openNodes = new List<Node>()
            {
                new Node()
                {
                    Position = startPos,
                    G = 0f,
                    H = 0f,
                    Parent = null
                }
            };
            List<Node> closeNodes = new List<Node>();

            Node endNode = null;

            while (openNodes.Count != 0)
            {
                int minIndex = GetMinNodeIndex(openNodes);
                var minNode = openNodes[minIndex];
                openNodes.RemoveAt(minIndex);

                var availableNodes = GetAllAvailableNodes(matrix, minNode, endPos);

                foreach (var availableNode in availableNodes)
                {
                    var isInOpen = false;
                    if (availableNode.Position == endPos)
                    {
                        endNode = availableNode;
                        break;
                    }

                    foreach (var openNode in openNodes)
                    {
                        if (openNode.Position == availableNode.Position && openNode.G > availableNode.G)
                        {
                            openNode.G = availableNode.G;
                            openNode.H = availableNode.H;
                            openNode.Parent = availableNode.Parent;
                            isInOpen = true;
                            break;
                        }
                    }

                    foreach (var closeNode in closeNodes)
                    {
                        if (closeNode.Position == availableNode.Position && closeNode.G > availableNode.G)
                        {
                            closeNodes.Remove(closeNode);
                        }
                    }

                    if (!isInOpen)
                    {
                        openNodes.Add(availableNode);
                    }
                }

                if (endNode != null) break;
            }

            return GetPath(endNode);
        }

        private static int GetMinNodeIndex(List<Node> nodes)
        {
            float minF = float.MaxValue;
            int minIndex = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (node.F >= minF) continue;

                minF = node.F;
                minIndex = i;
            }

            return minIndex;
        }

        private static List<Node> GetAllAvailableNodes(bool[,] matrix, Node node, Vector2Int targetPos)
        {
            List<Node> availableNodes = new List<Node>();

            foreach (var direction in Directions)
            {
                var pos = node.Position + direction;

                if (pos.x < 0 || pos.x >= matrix.Length) continue;
                if (pos.y < 0 || pos.y >= matrix.Length) continue;
                
                if (!matrix[pos.x, pos.y]) continue;

                availableNodes.Add(new Node()
                {
                    Position = pos,
                    H = Vector2Int.Distance(pos, targetPos),
                    G = node.G + Vector2Int.Distance(node.Position, pos),
                    Parent = node
                });
            }

            return availableNodes;
        }

        private static List<Vector2Int> GetPath(Node endNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            while (endNode != null)
            {
                path.Add(endNode.Position);
                endNode = endNode.Parent;
            }

            return path;
        }
    }

    internal class Node
    {
        public Vector2Int Position;
        public float H;

        public float G;
        public float F => G + H;

        public Node Parent;
    }
}