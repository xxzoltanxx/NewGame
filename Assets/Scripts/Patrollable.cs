using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrollable : MonoBehaviour
{
    public enum PatrolStatus
    {
        Ongoing = 0,
        Finished = 1
    }
    public GameObject boundVillage = null;
    public GameObject destinationVillage = null;
    private GameWorld gameWorld;
    private List<Vector2> pathNodes;
    private int currentPathNodeIndex = 0;
    private Entity boundEntity;
    public bool isSupply = false;
    public Collider2D enterTrigger = null;
    public Vector2 lastSeenEnemyPosition = new Vector2(0, 0);
    public bool didintCheckLastPosition = true;

    public Vector2 currentCheckpoint = new Vector2();

    private PathGrid grid;
    private GameManager gameManager;
    private bool usingPathfinding = false;
    public float distanceToEnablePathfinding = 5.0f;
    // Start is called before the first frame update
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameWorld = GameObject.Find("GameWorld").GetComponent<GameWorld>();
        boundEntity = GetComponent<Entity>();
        grid = GameObject.Find("GameWorld").GetComponent<PathGrid>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void lazyInit(Vector2 checkpoint, GameObject boundVillage, GameObject destinationVillage)
    {
        usingPathfinding = false;
        currentCheckpoint = checkpoint;
        this.boundVillage = boundVillage;
        this.destinationVillage = destinationVillage;
    }

    public void resetToDestinationVillage()
    {
        reset(destinationVillage.transform.position);
    }

    public void SetToNormal()
    {
        boundEntity.pace = 1.0f;
    }
    public void SetToScout()
    {
        transform.GetChild(3).gameObject.GetComponent<Animator>().SetTrigger("questionMark");
        boundEntity.pace = 0.5f;
    }

    public void RunAway(Vector2 enemyPosition)
    {
        Vector2 currentPosition = transform.position;
        Vector2 dir = currentPosition - enemyPosition;
        dir.Normalize();
        var newPosition = grid.getNearestRunableTileInDirection(currentPosition, dir);
        reset(newPosition);
    }

    public void reset(Vector2 checkpoint)
    {
        usingPathfinding = false;
        currentCheckpoint = checkpoint;
    }

    public PatrolStatus TickMovement(float dt)
    {
        if (!usingPathfinding)
        {
            Vector2 sameZcheckpoint = currentCheckpoint;
            Vector2 sameZ = transform.position;
            if (Vector2.Distance(sameZ, sameZcheckpoint) > distanceToEnablePathfinding)
            {
                pathNodes = grid.FindPath(sameZ, sameZcheckpoint, boundEntity);
                pathNodes[pathNodes.Count - 1] = sameZcheckpoint;
                currentPathNodeIndex = 0;
                usingPathfinding = true;
                return PatrolStatus.Ongoing;
            }
            else if (!sameZ.FuzzyEquals(sameZcheckpoint, 0.01f))
            {
                Vector3 direction = sameZcheckpoint - sameZ;
                direction.z = 0;
                direction.Normalize();

                GameWorld.Terrain currentTerrain = grid.NodeFromWorldPoint(transform.position).weight;
                if (currentTerrain == GameWorld.Terrain.Forest)
                {
                    GetComponent<Entity>().hidden = true;
                }
                else
                {
                    GetComponent<Entity>().hidden = false;
                }
                float movementModifier = boundEntity.terrainModifiers[currentTerrain] * boundEntity.pace * boundEntity.speed;

                Vector3 newPosition = transform.position + direction * movementModifier * gameManager.timeMultiplier * Time.deltaTime;
                transform.position = newPosition;
                return PatrolStatus.Ongoing;
            }
            else
            {
                //REACHED THE DESTINATION
                return PatrolStatus.Finished;
            }
        }
        else if (usingPathfinding)
        {
            Vector3 checkpointPosition = pathNodes[currentPathNodeIndex];
            Vector2 sameZEnd = currentCheckpoint;
            Vector2 sameZCheckpoint = checkpointPosition;
            Vector2 sameZPlayer = transform.position;
            if (!sameZCheckpoint.FuzzyEquals(sameZPlayer, 0.025f))
            {
                Vector2 direction2D = sameZCheckpoint - sameZPlayer;
                Vector3 direction = direction2D;
                direction.Normalize();

                GameWorld.Terrain currentTerrain = grid.NodeFromWorldPoint(transform.position).weight;
                if (currentTerrain == GameWorld.Terrain.Forest)
                {
                    GetComponent<Entity>().hidden = true;
                }
                else
                {
                    GetComponent<Entity>().hidden = false;
                }
                float movementModifier = boundEntity.terrainModifiers[currentTerrain] * boundEntity.pace * boundEntity.speed;

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
                    usingPathfinding = false;
                }
            }
        }
        return PatrolStatus.Ongoing;
    }
}
