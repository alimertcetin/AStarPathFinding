using System;
using System.Collections.Generic;

namespace AStarPathFinding
{
    public class PathFinder
    {
        public List<Node> finalPath;

        public Vec3 extraCost;

        PathGrid grid;
        public PathGrid pathGrid => grid;

        public PathFinder(PathGrid grid)
        {
            this.grid = grid;
        }

        public PathFinder(PathGrid grid, Vec3 extraCost)
        {
            this.extraCost = extraCost;
            this.grid = grid;
            if (this.extraCost.sqrMagnitude == Single.PositiveInfinity)
            {
                throw new InvalidOperationException("Extra cost of node returned infinity. this is not allowed");
            }
        }

        public Vec3 GetClosestPosition(Vec3 currentPosition)
        {
            if (finalPath == null) return currentPosition;

            return grid.NodeFromWorldPosition(grid.GetClosest(finalPath, currentPosition).worldPosition).worldPosition;
        }

        public Vec3 GetNextPos(Vec3 currentPosition)
        {
            if (finalPath == null || finalPath.Count == 0) return currentPosition;

            Node nodeA = grid.GetClosestNext(finalPath, currentPosition);

            return nodeA.worldPosition == currentPosition ? currentPosition : nodeA.worldPosition;
        }

        public void FindPath(Vec3 startPos, Vec3 targetPos)
        {
            Node startNode = grid.NodeFromWorldPosition(startPos);
            Node targetNode = grid.NodeFromWorldPosition(targetPos);

            List<Node> openList = new List<Node>();
            HashSet<Node> closedList = new HashSet<Node>();

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList[0];

                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].fCost <= currentNode.fCost && openList[i].hCost < currentNode.hCost)
                    {
                        currentNode = openList[i];
                    }
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == targetNode)
                {
                    finalPath = CreatePath(startNode, targetNode);
                }

                List<Node> neighborNodes = grid.GetNeighborNodes(currentNode);
                for (int i = 0; i < neighborNodes.Count; i++)
                {
                    Node neighborNode = neighborNodes[i];
                    if (neighborNode.isObstacle || neighborNode.dontIncludeToPath || closedList.Contains(neighborNode))
                    {
                        continue;
                    }

                    GetManhattenDistance(currentNode, neighborNode, targetPos);

                    bool isInTheList = openList.Contains(neighborNode);
                    if (currentNode.fCost < neighborNode.fCost || !isInTheList)
                    {
                        GetManhattenDistance(neighborNode, targetNode, targetPos);
                        neighborNode.SetParent(currentNode);

                        if (!isInTheList)
                        {
                            openList.Add(neighborNode);
                        }
                    }

                }

            }

        }

        List<Node> CreatePath(Node startNode, Node targetNode)
        {
            List<Node> finalPath = new List<Node>();
            Node currentNode = targetNode;
            while (currentNode != startNode)
            {
                finalPath.Add(currentNode);
                currentNode = currentNode.parent;
            }

            finalPath.Reverse();
            return finalPath;
        }

        void GetManhattenDistance(Node nodeA, Node nodeB, Vec3 targetPos)
        {
            var distVec = nodeA.worldPosition - nodeB.worldPosition;

            distVec += extraCost;

            float gCost = (nodeA.worldPosition.x - targetPos.x) + 
                (nodeA.worldPosition.y - targetPos.y) + 
                (nodeA.worldPosition.z - targetPos.z);

            float hCost = nodeB.hCost + distVec.sqrMagnitude;
            nodeA.SetCost(gCost, hCost);

        }
    }

}
