using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private class Node
    {
        public Vector3Int position;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
        public Node parent;

        public Node(Vector3Int pos)
        {
            position = pos;
        }
    }

    public List<Vector3Int> FindPath(Vector3Int startPos, Vector3Int targetPos, GridManager gridManager)
    {
        if (!gridManager.IsTileWalkable(targetPos))
        {
            return null;
        }

        Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();
        List<Node> openSet = new List<Node>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        Node startNode = new Node(startPos);
        allNodes[startPos] = startNode;
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || 
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode.position);

            if (currentNode.position == targetPos)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Vector3Int neighborPos in gridManager.GetNeighbors(currentNode.position))
            {
                if (closedSet.Contains(neighborPos))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode.position, neighborPos);
                
                Node neighborNode;
                if (!allNodes.ContainsKey(neighborPos))
                {
                    neighborNode = new Node(neighborPos);
                    allNodes[neighborPos] = neighborNode;
                }
                else
                {
                    neighborNode = allNodes[neighborPos];
                }

                if (newMovementCostToNeighbor < neighborNode.gCost || !openSet.Contains(neighborNode))
                {
                    neighborNode.gCost = newMovementCostToNeighbor;
                    neighborNode.hCost = GetDistance(neighborPos, targetPos);
                    neighborNode.parent = currentNode;

                    if (!openSet.Contains(neighborNode))
                    {
                        openSet.Add(neighborNode);
                    }
                }
            }
        }

        return null;
    }

    List<Vector3Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(Vector3Int posA, Vector3Int posB)
    {
        int distX = Mathf.Abs(posA.x - posB.x);
        int distY = Mathf.Abs(posA.y - posB.y);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }
}
