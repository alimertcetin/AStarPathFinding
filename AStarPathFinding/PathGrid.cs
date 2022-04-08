using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
        public Vector3 GridCenter
        {
            get => gridCenter;
            set
            {
                gridCenter = value;
                CreateGrid();
            }
        }
        public LayerMask LayerToAvoid
        {
            get => layerToAvoid;
            set
            {
                layerToAvoid = value;
                CreateGrid();
            }
        }
        public LayerMask LayerToIgnore
        {
            get => layerToIgnore;
            set
            {
                layerToIgnore = value;
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

        public List<Node> ObstacleNodes
        {
            get => obstacleNodes;
        }
        public List<Node> IgnoredLayerNodes
        {
            get => ignoredLayerNodes;
        }

        int gridSizeX;
        int gridSizeZ;
        Vector3 gridCenter;
        LayerMask layerToAvoid;
        LayerMask layerToIgnore;
        float nodeRadius;
        List<Node> obstacleNodes = new List<Node>();
        List<Node> ignoredLayerNodes = new List<Node>();

        public PathGrid(Vector3 gridCenterPos, float gridWorldSizeX, float gridWorldSizeZ, LayerMask obstacleLayer, LayerMask ignoreLayer,
            float nodeRadius, bool makeNeighborObstacles)
        {
            this.gridSizeX = (int)gridWorldSizeX;
            this.gridSizeZ = (int)gridWorldSizeZ;
            this.gridCenter = gridCenterPos;
            this.layerToAvoid = obstacleLayer;
            this.layerToIgnore = ignoreLayer;
            this.nodeRadius = nodeRadius;

            CreateGrid();

            if (makeNeighborObstacles)
            {
                MakeNeighborObstacle();
            }
        }

        void CreateGrid()
        {
            obstacleNodes.Clear();
            nodes = new Node[gridSizeX, gridSizeZ];
            Vector3 bottomLeft = gridCenter - Vector3.right * gridSizeX / 2 - Vector3.forward * gridSizeZ / 2;
            for (int z = 0; z < gridSizeZ; z++)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                    bool isInsideIgnoredLayer = Physics.CheckSphere(worldPoint, nodeRadius, layerToIgnore);
                    bool isObstacle = Physics.CheckSphere(worldPoint, nodeRadius, layerToAvoid);
                    Node newNode = new Node(isObstacle, isInsideIgnoredLayer,
                        worldPoint, new Vector3(x, worldPoint.y, z), nodeRadius);
                    nodes[x, z] = newNode;

                    if (isObstacle) obstacleNodes.Add(newNode);
                    if (isInsideIgnoredLayer) ignoredLayerNodes.Add(newNode);
                }
            }
        }

        void MakeNeighborObstacle()
        {
            for (int i = 0; i < obstacleNodes.Count; i++)
            {
                List<Node> neighbors = GetNeighborNodes(obstacleNodes[i]);
                for (int j = 0; j < neighbors.Count; j++)
                {
                    neighbors[j].isObstacle = true;
                }
            }
        }

        public void UpdateNode(Vector3 previousPos, Vector3 nextPos)
        {
            Node node = NodeFromWorldPosition(previousPos);
            node.SetWorldPosition(nextPos);
        }

        public void UpdateNode(Node node, Vector3 nextPos)
        {
            node.SetWorldPosition(nextPos);
        }

        public Node NodeFromWorldPosition(Vector3 startPos)
        {
            float xPoint = ((startPos.x + gridSizeX / 2) / gridSizeX);
            float zPoint = ((startPos.z + gridSizeZ / 2) / gridSizeZ);

            xPoint = Mathf.Clamp01(xPoint);
            zPoint = Mathf.Clamp01(zPoint);

            int x = Mathf.RoundToInt((gridSizeX - 1) * xPoint);
            int z = Mathf.RoundToInt((gridSizeZ - 1) * zPoint);
            return nodes[x, z];
        }

        public List<Node> GetNeighborNodes(Node current)
        {
            List<Node> neighbors = new List<Node>();
            Node right = GetNeighbor(current, Vector3.right);
            Node left = GetNeighbor(current, Vector3.left);
            Node up = GetNeighbor(current, Vector3.up);
            Node down = GetNeighbor(current, Vector3.down);

            if (right != null) neighbors.Add(right);
            if (left != null) neighbors.Add(left);
            if (up != null) neighbors.Add(up);
            if (down != null) neighbors.Add(down);

            return neighbors;
        }

        public Node GetNeighbor(Node current, Vector3 direction)
        {
            int xCheck = (int)current.gridX + (int)direction.x;
            int zCheck = (int)current.gridZ + (int)direction.y;

            if (xCheck >= 0 && xCheck < gridSizeX && zCheck >= 0 && zCheck < gridSizeZ)
            {
                return nodes[xCheck, zCheck];
            }
            return null;
        }

        public Node GetClosest(List<Node> nodes, Vector3 currentPosition)
        {
            List<Node> newNodeList = new List<Node>(nodes.Count);
            for (int i = 0; i < nodes.Count; i++)
            {
                newNodeList.Add(nodes[i]);
            }
            SortByDistance(newNodeList);
            Node current = newNodeList[0];
            float diff = Vector3.Distance(currentPosition, current.worldPosition);
            for (int i = 0; i < newNodeList.Count; i++)
            {
                Node newNode = newNodeList[i];
                float newDiff = Vector3.Distance(newNode.worldPosition, currentPosition);
                if (newDiff < diff)
                {
                    diff = newDiff;
                    current = newNode;
                }
            }

            return current;
        }

        public Node GetClosestNext(List<Node> nodes, Vector3 currentPosition)
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
                    if (Vector3.Distance(arr[current].worldPosition, arr[j].worldPosition) < distance)
                    {
                        distance = Vector3.Distance(arr[current].worldPosition, arr[j].worldPosition);
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

        //public void SortByDistance(List<Vector3> arr)
        //{
        //    float distance = float.MaxValue;
        //    for (var i = 0; i < arr.Count; i++)
        //    {
        //        var current = i;
        //        for (var j = i + 1; j < arr.Count; j++)
        //        {
        //            if (Vector3.Distance(arr[current], arr[j]) < distance)
        //            {
        //                distance = Vector3.Distance(arr[current], arr[j]);
        //                current = j;
        //            }
        //        }
        //        if (current != i)
        //        {
        //            var lowerValue = arr[current];
        //            arr[current] = arr[i];
        //            arr[i] = lowerValue;
        //        }
        //    }
        //}


    }
}
