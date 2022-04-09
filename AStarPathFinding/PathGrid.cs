using System;
using System.Collections.Generic;

namespace AStarPathFinding
{
    public class PathGrid
    {
        public Node[,] nodes { get; private set; }
        public float nodeDiameter => nodeRadius * 2;

        public int GridSizeX
        {
            get => gridSizeX;
            set
            {
                gridSizeX = value;
                CreateGrid();
            }
        }
        public int GridSizeZ
        {
            get => gridSizeZ;
            set
            {
                gridSizeZ = value;
                CreateGrid();
            }
        }
        public Vec3 GridCenter
        {
            get => gridCenter;
            set
            {
                gridCenter = value;
                CreateGrid();
            }
        }

        public float NodeRadius
        {
            get => nodeRadius;
            set
            {
                nodeRadius = value;
                CreateGrid();
            }
        }

        int gridSizeX;
        int gridSizeZ;
        Vec3 gridCenter;
        float nodeRadius;

        public PathGrid(float gridCenterX, float gridCenterY, float gridCenterZ, 
            float gridSizeX, float gridSizeZ, 
            float nodeRadius)
        {
            this.gridCenter = new Vec3(gridCenterX, gridCenterY, gridCenterZ);
            this.gridSizeX = (int)gridSizeX;
            this.gridSizeZ = (int)gridSizeZ;
            this.nodeRadius = nodeRadius;

            CreateGrid();
        }

        public void CreateGrid()
        {
            nodes = new Node[gridSizeX, gridSizeZ];
            Vec3 bottomLeft = gridCenter - Vec3.right * gridSizeX / 2 - Vec3.forward * gridSizeZ / 2;
            for (int z = 0; z < gridSizeZ; z++)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    Vec3 worldPoint = bottomLeft + Vec3.right * (x * nodeDiameter + nodeRadius) + Vec3.forward * (z * nodeDiameter + nodeRadius);
                    nodes[x, z] = new Node(worldPoint, new Vec3(x, worldPoint.y, z), nodeRadius);
                }
            }
        }

        public void UpdateNode(Vec3 previousPos, Vec3 nextPos)
        {
            Node node = NodeFromWorldPosition(previousPos);
            node.SetWorldPosition(nextPos);
        }

        public void UpdateNode(Node node, Vec3 nextPos)
        {
            node.SetWorldPosition(nextPos);
        }

        public Node NodeFromWorldPosition(Vec3 currentPos)
        {
            float xPoint = ((currentPos.x + gridSizeX / 2) / gridSizeX);
            float zPoint = ((currentPos.z + gridSizeZ / 2) / gridSizeZ);

            if (xPoint > 1)
            {
                xPoint = 1;
            }
            else if(xPoint < 0)
            {
                xPoint = 0;
            }

            if (zPoint > 1)
            {
                zPoint = 1;
            }
            else if(zPoint < 0)
            {
                zPoint = 0;
            }

            int x = (int)Math.Round((gridSizeX - 1) * xPoint);
            int z = (int)Math.Round((gridSizeZ - 1) * zPoint);
            return nodes[x, z];
        }

        public List<Node> GetNeighborNodes(Node current)
        {
            List<Node> neighbors = new List<Node>();
            Node right = GetNeighbor(current, Vec3.right);
            Node left = GetNeighbor(current, Vec3.left);
            Node forward = GetNeighbor(current, Vec3.forward);
            Node back = GetNeighbor(current, Vec3.back);

            if (right != null) neighbors.Add(right);
            if (left != null) neighbors.Add(left);
            if (forward != null) neighbors.Add(forward);
            if (back != null) neighbors.Add(back);

            return neighbors;
        }

        public Node GetNeighbor(Node current, Vec3 direction)
        {
            int xCheck = (int)current.gridPos.x + (int)direction.x;
            int zCheck = (int)current.gridPos.z + (int)direction.z;

            if (xCheck >= 0 && xCheck < gridSizeX && zCheck >= 0 && zCheck < gridSizeZ)
            {
                return nodes[xCheck, zCheck];
            }
            return null;
        }

        public Node GetClosest(List<Node> nodes, Vec3 currentPosition)
        {
            List<Node> newNodeList = new List<Node>(nodes);
            SortByDistance(newNodeList);
            
            Node current = newNodeList[0];
            float diff = Vec3.Distance(currentPosition, current.worldPosition);
            for (int i = 0; i < newNodeList.Count; i++)
            {
                Node newNode = newNodeList[i];
                float newDiff = Vec3.Distance(newNode.worldPosition, currentPosition);
                if (newDiff < diff)
                {
                    diff = newDiff;
                    current = newNode;
                }
            }

            return current;
        }

        public Node GetClosestNext(List<Node> nodes, Vec3 currentPosition)
        {
            Node closestNode = GetClosest(nodes, currentPosition);
            int indexOfNode = nodes.IndexOf(closestNode);
            if (indexOfNode + 1 < nodes.Count)
            {
                return nodes[indexOfNode + 1];
            }
            else
            {
                return closestNode;
            }
        }

        public void SortByDistance(List<Node> arr)
        {
            float distance = float.MaxValue;
            for (var i = 0; i < arr.Count; i++)
            {
                var current = i;
                for (var j = i + 1; j < arr.Count; j++)
                {
                    if (Vec3.Distance(arr[current].worldPosition, arr[j].worldPosition) < distance)
                    {
                        distance = Vec3.Distance(arr[current].worldPosition, arr[j].worldPosition);
                        current = j;
                    }
                }
                if (current != i)
                {
                    var lowerValue = arr[current];
                    arr[current] = arr[i];
                    arr[i] = lowerValue;
                }
            }
        }

        public void SortByDistance(List<Vec3> arr)
        {
            float distance = float.MaxValue;
            for (var i = 0; i < arr.Count; i++)
            {
                var current = i;
                for (var j = i + 1; j < arr.Count; j++)
                {
                    if (Vec3.Distance(arr[current], arr[j]) < distance)
                    {
                        distance = Vec3.Distance(arr[current], arr[j]);
                        current = j;
                    }
                }
                if (current != i)
                {
                    var lowerValue = arr[current];
                    arr[current] = arr[i];
                    arr[i] = lowerValue;
                }
            }
        }


    }
}
