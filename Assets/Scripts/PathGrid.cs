using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGrid : MonoBehaviour
{
    [SerializeField] private GameWorld world;
    private WorldGenerator generator;
    private WorldMesh mesh;
    private float nodeRadius;
    PathNode[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    // Start is called before the first frame update

    private void Awake()
    {
    }
    void Start()
    {
    }
    public void CreateGrid()
    {
        mesh = world.GetComponent<WorldMesh>();
        generator = world.GetComponent<WorldGenerator>();
        nodeDiameter = mesh.tileSize.x;
        nodeRadius = nodeDiameter / 2.0f;
        gridSizeX = generator.width;
        gridSizeY = generator.height;
        grid = new PathNode[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * mesh.totalSize.x / 2 - Vector3.up * mesh.totalSize.y / 2;
        for (int x = 0; x < gridSizeX; ++x)
            for (int y = 0; y < gridSizeY; ++y)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                GameWorld.Terrain terrain = world.getTerrainAtPoint(worldPoint);
                grid[x, y] = new PathNode(terrain, worldPoint, x, y);
            }
    }
    // Update is called once per frame
    void Update()
    {

    }

    public PathNode NodeFromWorldPoint(Vector3 point)
    {
        float percentX = ((float)point.x + transform.position.x + mesh.totalSize.x / 2.0f) / mesh.totalSize.x;
        float percentY = ((float)point.y + transform.position.y + mesh.totalSize.y / 2) / mesh.totalSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = (int)((gridSizeX * percentX));
        int y = (int)((gridSizeY * percentY));
        return grid[x, y];
    }

    public List<Vector2> FindPathTiles(Vector2 start, Vector2 end, Entity entity)
    {
        PathNode startNode = grid[(int)start.x, (int)start.y];
        PathNode endNode = grid[(int)end.x, (int)end.y];

        List<PathNode> openSet = new List<PathNode>();
        HashSet<PathNode> closedSet = new HashSet<PathNode>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            PathNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; ++i)
            {
                if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePathTiles(startNode, endNode);
            }

            foreach (PathNode node in GetNeighbours(currentNode))
            {
                if (closedSet.Contains(node))
                    continue;
                int newMovementCostToNeighbour = currentNode.gCost + getDistance(currentNode, node) + entity.pathfindingWeights[node.weight];
                if (newMovementCostToNeighbour < node.gCost || !openSet.Contains(node))
                {
                    node.gCost = newMovementCostToNeighbour;
                    node.hCost = getDistance(node, endNode);
                    node.parent = currentNode;

                    if (!openSet.Contains(node))
                    {
                        openSet.Add(node);
                    }
                }
            }


        }
        return null;
    }

    public List<Vector2> FindPath(Vector3 startPos, Vector3 targetPos, Entity entity)
    {
        PathNode startNode = NodeFromWorldPoint(startPos);
        PathNode endNode = NodeFromWorldPoint(targetPos);

        List<PathNode> openSet = new List<PathNode>();
        HashSet<PathNode> closedSet = new HashSet<PathNode>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            PathNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; ++i)
            {
                if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (PathNode node in GetNeighbours(currentNode))
            {
                if (closedSet.Contains(node))
                    continue;
                int newMovementCostToNeighbour = currentNode.gCost + getDistance(currentNode, node) + entity.pathfindingWeights[node.weight];
                if (newMovementCostToNeighbour < node.gCost || !openSet.Contains(node))
                {
                    node.gCost = newMovementCostToNeighbour;
                    node.hCost = getDistance(node, endNode);
                    node.parent = currentNode;

                    if (!openSet.Contains(node))
                    {
                        openSet.Add(node);
                    }
                }
            }


        }
        return null;
    }

    List<Vector2> RetracePathTiles(PathNode start, PathNode end)
    {
        List<Vector2> path = new List<Vector2>();
        PathNode currentNode = end;
        while (currentNode != start)
        {
            path.Add(new Vector2(currentNode.gridX, currentNode.gridY));
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
    List<Vector2> RetracePath(PathNode start, PathNode end)
    {
        List<Vector2> path = new List<Vector2>();
        PathNode currentNode = end;
        while (currentNode != start)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
    int getDistance(PathNode nodeA, PathNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
    public List<PathNode> GetNeighbours(PathNode node)
    {
        List<PathNode> neighbours = new List<PathNode>();
        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                if (x == 0 && y == 0)
                    continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public Vector2 getNearestRunableTileInDirection(Vector2 position, Vector2 direction)
    {
        if (direction.x < 0.5f && direction.x > 0)
            direction.x = 0;
        else if (direction.x >= 0.5f)
            direction.x = 1.0f;
        else if (direction.x <= -0.5f)
            direction.x = -1.0f;
        else
            direction.x = 0;
        if (direction.y < 0.5f && direction.y > 0)
            direction.y = 0;
        else if (direction.y >= 0.5f)
            direction.y = 1.0f;
        else if (direction.y <= -0.5f)
            direction.y = -1.0f;
        else
            direction.y = 0;

        direction *= nodeDiameter;
        Vector2 point = position + direction;
        float percentX = ((float)point.x + transform.position.x + mesh.totalSize.x / 2.0f) / mesh.totalSize.x;
        float percentY = ((float)point.y + transform.position.y + mesh.totalSize.y / 2) / mesh.totalSize.y;
        if (percentX >= 1.0f || percentX <= 0)
        {
            direction.x *= -1;
        }
        if (percentY >= 1.0f || percentY <= 0)
        {
            direction.y *= -1;
        }
        PathNode node = NodeFromWorldPoint(position + direction);
        return node.worldPosition;
        
    }
}
