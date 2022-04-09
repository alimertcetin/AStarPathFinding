namespace AStarPathFinding
{
    public class Node
    {
        public int index => (int)gridPos.x + (int)gridPos.z;
        public Vec3 gridPos { get; private set; }
        public Vec3 worldPosition { get; private set; }
        public Vec3 forward { get; private set; }

        public float gCost { get; private set; }
        public float hCost { get; private set; }
        public float fCost => gCost + hCost;

        public Node parent { get; private set; }
        public bool isObstacle;
        public bool dontIncludeToPath;
        public bool ignoreable;
        public float radius;

        public Node(Vec3 worldPosition, Vec3 gridPos, float radius)
        {
            this.worldPosition = worldPosition;
            this.gridPos = gridPos;
            this.radius = radius;
        }

        public void SetCost(float gCost, float hCost)
        {
            this.gCost = gCost;
            this.hCost = hCost;
        }

        public void SetWorldPosition(Vec3 newPos)
        {
            this.worldPosition = newPos;
            this.gridPos = new Vec3(gridPos.x, newPos.y, gridPos.z);
        }

        public void SetParent(Node parentNode)
        {
            parent = parentNode;
            this.forward = (parent.worldPosition - this.worldPosition).normalized;
        }

    }

}