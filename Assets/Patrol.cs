using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : StateMachineBehaviour
{
    private RadiableNPC npcRadiableComp;
    private GameWorld gameWorld;
    private Patrollable patrollable;
    private Vector2 patrolPoint;
    private List<Vector2> pathNodes;
    private int currentPathNodeIndex = 0;
    private Entity boundEntity;
    Vector2 currentCheckpoint = new Vector2();
    private PathGrid grid;
    private GameManager gameManager;
    private bool usingPathfinding = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameWorld = GameObject.Find("GameWorld").GetComponent<GameWorld>();
        patrollable = animator.gameObject.GetComponent<Patrollable>();
        boundEntity = animator.gameObject.GetComponent<Entity>();
        grid = GameObject.Find("GameWorld").GetComponent<PathGrid>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!usingPathfinding)
        {
            Vector2 sameZcheckpoint = currentCheckpoint;
            Vector2 sameZ = animator.gameObject.transform.position;
            if (Vector2.Distance(sameZ, sameZcheckpoint) > patrollable.distanceToEnablePathfinding)
            {
                pathNodes = grid.FindPath(sameZcheckpoint, sameZ, boundEntity);
                pathNodes[pathNodes.Count - 1] = sameZ;
                currentPathNodeIndex = 0;
                usingPathfinding = true;
            }
            else if (!sameZ.FuzzyEquals(sameZcheckpoint, 0.01f))
            {
                Vector3 direction = sameZcheckpoint - sameZ;
                direction.z = 0;
                direction.Normalize();

                GameWorld.Terrain currentTerrain = grid.NodeFromWorldPoint(animator.gameObject.transform.position).weight;
                if (currentTerrain == GameWorld.Terrain.Forest)
                {
                    animator.gameObject.GetComponent<Entity>().hidden = true;
                }
                else
                {
                    animator.gameObject.GetComponent<Entity>().hidden = false;
                }
                float movementModifier = boundEntity.terrainModifiers[currentTerrain] * boundEntity.pace * boundEntity.speed;

                Vector3 newPosition = animator.gameObject.transform.position + direction * movementModifier * gameManager.timeMultiplier * Time.deltaTime;
                animator.gameObject.transform.position = newPosition;
            }
            else
            {
                getNewCheckpoint(animator);
            }
        }
        else if (usingPathfinding)
        {
            Vector3 checkpointPosition = pathNodes[currentPathNodeIndex];
            Vector2 sameZEnd = currentCheckpoint;
            Vector2 sameZCheckpoint = checkpointPosition;
            Vector2 sameZPlayer = animator.gameObject.transform.position;
            if (!sameZCheckpoint.FuzzyEquals(sameZPlayer, 0.025f))
            {
                Vector2 direction2D = sameZCheckpoint - sameZPlayer;
                Vector3 direction = direction2D;
                direction.Normalize();

                GameWorld.Terrain currentTerrain = grid.NodeFromWorldPoint(animator.gameObject.transform.position).weight;
                if (currentTerrain == GameWorld.Terrain.Forest)
                {
                    animator.gameObject.GetComponent<Entity>().hidden = true;
                }
                else
                {
                    animator.gameObject.GetComponent<Entity>().hidden = false;
                }
                float movementModifier = boundEntity.terrainModifiers[currentTerrain] * boundEntity.pace * boundEntity.speed;

                Vector3 newPosition = animator.gameObject.transform.position + direction * movementModifier * gameManager.timeMultiplier * Time.deltaTime;
                animator.gameObject.transform.position = newPosition;
            }
            else
            {
                animator.gameObject.transform.position = new Vector3(sameZCheckpoint.x, sameZCheckpoint.y, animator.gameObject.transform.position.z);
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
    }

    private void getNewCheckpoint(Animator animator)
    {
        Bounds bounds = gameWorld.GetComponent<MeshRenderer>().bounds;
        Vector3 LD = bounds.min;
        float x = patrollable.areaOfInterest.x + Random.Range(-patrollable.patrolRadius, patrollable.patrolRadius);
        float y = patrollable.areaOfInterest.y + Random.Range(-patrollable.patrolRadius, patrollable.patrolRadius);
        LD = new Vector3(Mathf.Clamp(x, bounds.min.x, bounds.max.x), Mathf.Clamp(y, bounds.min.y, bounds.max.y), animator.gameObject.transform.position.z);
        currentCheckpoint = LD;
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
