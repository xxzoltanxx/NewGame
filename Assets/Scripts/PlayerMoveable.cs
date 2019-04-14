using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Entity))]
public class PlayerMoveable : MonoBehaviour
{
    public const float distanceToEnablePathfinding = 3.0f;
    public const float lineZ = -2.0f;
    public GameObject line;
    private GameManager gameManager;
    private Entity boundEntity;
    private GameWorld world;
    private PathGrid grid;
    private List<Vector2> pathNodes;
    private int currentPathNodeIndex = 0;
    // Start is called before the first frame update
    void Awake()
    {
        line = GameObject.Find("Path");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        world = GameObject.Find("GameWorld").GetComponent<GameWorld>();
        grid = GameObject.Find("GameWorld").GetComponent<PathGrid>();
        boundEntity = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.playerCheckpoint && gameManager.playerCheckpointUpdated)
        {
            line.GetComponent<CurveLineRenderer>().ClearVertices();
            if (pathNodes != null)
                pathNodes.Clear();
            Vector2 sameZ = gameManager.playerCheckpoint.transform.position;
            Vector2 sameZPlayer = transform.position;
            if (Vector2.Distance(sameZ, sameZPlayer) > distanceToEnablePathfinding)
            {
                pathNodes = grid.FindPath(sameZPlayer, sameZ, boundEntity);
                pathNodes[pathNodes.Count - 1] = sameZ;
                currentPathNodeIndex = 0;
                gameManager.playerCheckpointUpdated = false;
            }
            else if (!sameZ.FuzzyEquals(sameZPlayer, 0.01f))
            {
                Vector3 direction = gameManager.playerCheckpoint.transform.position - transform.position;
                direction.z = 0;
                direction.Normalize();

                GameWorld.Terrain currentTerrain = grid.NodeFromWorldPoint(transform.position).weight;
                float movementModifier = boundEntity.terrainModifiers[currentTerrain];

                Vector3 newPosition = transform.position + direction * movementModifier * gameManager.timeMultiplier * Time.deltaTime;
                transform.position = newPosition;
            }
            else
            {
                GameObject.Destroy(gameManager.playerCheckpoint);
            }
        }
        else if (gameManager.playerCheckpoint && !gameManager.playerCheckpointUpdated)
        {
            Vector3 checkpointPosition = pathNodes[currentPathNodeIndex];
            Vector2 sameZEnd = gameManager.playerCheckpoint.transform.position;
            Vector2 sameZCheckpoint = checkpointPosition;
            Vector2 sameZPlayer = transform.position;
            List<Vector3> linePosition = new List<Vector3>();
            linePosition.Add(new Vector3(sameZPlayer.x, sameZPlayer.y, lineZ));
            line.GetComponent<CurveLineRenderer>().ClearVertices();
            for (int i = currentPathNodeIndex; i < pathNodes.Count; ++i)
            {
                linePosition.Add(new Vector3(pathNodes[i].x, pathNodes[i].y, lineZ));
            }
            line.GetComponent<CurveLineRenderer>().SetVertices(linePosition);
            if (!sameZCheckpoint.FuzzyEquals(sameZPlayer, 0.025f))
            {
                Vector2 direction2D = sameZCheckpoint - sameZPlayer;
                Vector3 direction = direction2D;
                direction.Normalize();

                GameWorld.Terrain currentTerrain = grid.NodeFromWorldPoint(transform.position).weight;
                float movementModifier = boundEntity.terrainModifiers[currentTerrain] * boundEntity.pace * boundEntity.speed; ;

                Vector3 newPosition = transform.position + direction * movementModifier * gameManager.timeMultiplier * Time.deltaTime;
                transform.position = newPosition;
            }
            else
            {
                transform.position = new Vector3(sameZCheckpoint.x, sameZCheckpoint.y, transform.position.z);
                if (currentPathNodeIndex + 1 <= pathNodes.Count - 1)
                {
                    ++currentPathNodeIndex;
                }
                else
                {
                    gameManager.playerCheckpointUpdated = true;
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (pathNodes.Count != 0)
        {
            foreach (Vector2 shit in pathNodes)
            {
                Gizmos.DrawCube(shit, new Vector2(0.2f, 0.2f));
            }
        }
    }
}
