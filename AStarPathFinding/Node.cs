using UnityEngine;

namespace AStarPathFinding
{
    public class Node
    {
        public int index => (int)gridX + (int)gridZ;
        public float gridX { get; private set; }
        public float gridY { get; private set; }
        public float gridZ { get; private set; }
        public Vector3 worldPosition { get; private set; }
        public Vector3 forward { get; private set; }

        public float gCost { get; private set; }
        public float hCost { get; private set; }
        public float fCost => gCost + hCost;

        public Node parent { get; private set; }
        public bool isObstacle;
        public bool dontIncludeToPath;
        public bool isModified;
        public bool isInsideIgnoredLayer;
        public float radius;

        public Node(bool isObstacle, bool isInsideIgnoredLayer, Vector3 worldPosition, Vector3 gridPos, float radius)
        {
            this.isObstacle = isObstacle;
            this.isInsideIgnoredLayer = isInsideIgnoredLayer;
            this.worldPosition = worldPosition;
            this.gridX = gridPos.x;
            this.gridY = gridPos.y;
            this.gridZ = gridPos.z;
            this.radius = radius;
        }

        public void SetCost(float gCost, float hCost)
        {
            this.gCost = gCost;
            this.hCost = hCost;
        }

        public void SetWorldPosition(Vector3 newPos)
        {
            this.worldPosition = newPos;
            this.gridY = newPos.y;
        }

        public void SetParent(Node parentNode)
        {
            parent = parentNode;
            this.forward = (parent.worldPosition - this.worldPosition).normalized;
        }



    }

}
