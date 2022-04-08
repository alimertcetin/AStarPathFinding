using System.Collections.Generic;

namespace AStarPathFinding
{
    public class PathFinder
    {
        public Transform pathFollower
        {
            get => startPoint;
            set
            {
                startPoint = value;
                UpdatePath();
            }
        }

        public Transform target
        {
            get => endPoint;
            set
            {
                endPoint = value;
                UpdatePath();
            }
        }

        public List<Node> finalPath { get; private set; }

        public Vector3 extraCost;

        PathGrid grid;
        public PathGrid pathGrid
        {
            get
            {
                return grid;
            }

            private set
            {
                grid = value;
                UpdatePath();
            }
        }

        Transform startPoint;
        Transform endPoint;

        public PathFinder(Transform pathFollower, Transform target, PathGrid grid)
        {
            this.startPoint = pathFollower;
            this.endPoint = target;
            this.grid = grid;
        }

        public PathFinder(Transform pathFollower, Transform target, PathGrid grid, Vector3 extraCost)
        {
            this.startPoint = pathFollower;
            this.endPoint = target;
            this.extraCost = extraCost;
            this.grid = grid;

#if UNITY_EDITOR
            Debug.Assert(this.extraCost.sqrMagnitude != Mathf.Infinity, "Extra cost of node returned infinity. this is not allowed");
            if (this.extraCost.sqrMagnitude == Mathf.Infinity) this.extraCost = Vector3.zero; // to not break GetManhattenDistance() calculation
#endif
        }

        public void UpdatePath()
        {
            FindPath(pathFollower.position, target.position);
        }

        public Vector3 GetClosestPosition(Vector3 currentPosition)
        {
            if (finalPath == null) return currentPosition;

            return grid.NodeFromWorldPosition(grid.GetClosest(finalPath, currentPosition).worldPosition).worldPosition;
        }

        public Vector3 GetNextPos(Vector3 currentPosition)
        {
            if (finalPath == null || finalPath.Count == 0) return currentPosition;

            Node nodeA = grid.GetClosestNext(finalPath, currentPosition);

            return nodeA.worldPosition == currentPosition ? currentPosition : nodeA.worldPosition;
        }

        void FindPath(Vector3 startPos, Vector3 targetPos)
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

        void GetManhattenDistance(Node nodeA, Node nodeB, Vector3 targetPos)
        {
            var distanceX = (nodeA.gridX - nodeB.gridX);
            var distanceY = (nodeA.gridY - nodeB.gridY);
            var distanceZ = (nodeA.gridZ - nodeB.gridZ);

            var distVec = new Vector3(distanceX, distanceY, distanceZ);
            distVec += extraCost;

            float gCost = (nodeA.gridX - targetPos.x) + (nodeA.gridY - targetPos.y) + (nodeA.gridZ - targetPos.z);
            float hCost = nodeB.hCost + distVec.sqrMagnitude;
            nodeA.SetCost(gCost, hCost);

        }
    }

}
