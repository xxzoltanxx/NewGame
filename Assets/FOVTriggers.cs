using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVTriggers : MonoBehaviour
{
    Collider2D collision = null;
    bool isEnemyInsideFOV = false;
    private bool oneIterTrigger = false;
    private GameWorld gameWorld = null;
    private GameManager gameManager = null;
    private Material copyMaterial = null;
    // Start is called before the first frame update
    private void Awake()
    {
        gameWorld = GameObject.Find("GameWorld").GetComponent<GameWorld>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void Start()
    {
        Material material = GetComponent<MeshRenderer>().sharedMaterial;
        copyMaterial = new Material(material);
        GetComponent<MeshRenderer>().material = copyMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = gameManager.player.transform.position;
        float playerDistance = gameManager.player.GetComponent<Entity>().viewingDistance;
        copyMaterial.SetVector("_PlayerPosition", playerPos);
        copyMaterial.SetFloat("_Distance", playerDistance);
        transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * 15.0f);
        if (collision && collision.gameObject.GetComponent<Entity>().hiddenInPlainSight == true)
        {
            oneIterTrigger = false;
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = null;
        }
        else if (isEnemyInsideFOV && (!transform.parent.gameObject.GetComponent<Entity>().hidden && !collision.gameObject.GetComponent<Entity>().hidden) && gameWorld.noForestPastThis(transform.parent.gameObject.transform.position,collision.gameObject.transform.position))
        {
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = collision;
            transform.parent.gameObject.GetComponent<Patrollable>().lastSeenEnemyPosition = collision.gameObject.transform.position;
            transform.parent.gameObject.GetComponent<Patrollable>().didintCheckLastPosition = true;
            oneIterTrigger = true;
        }
        else if (isEnemyInsideFOV && (!transform.parent.gameObject.GetComponent<Entity>().hidden && collision.gameObject.GetComponent<Entity>().hidden) && !collision.gameObject.GetComponent<Entity>().beingScanned)
        {
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = null;
            if (oneIterTrigger)
            {
                transform.parent.gameObject.GetComponent<Patrollable>().lastSeenEnemyPosition = collision.gameObject.transform.position;
                transform.parent.gameObject.GetComponent<Patrollable>().didintCheckLastPosition = true;
                oneIterTrigger = false;
            }
        }
        else if (isEnemyInsideFOV && (transform.parent.gameObject.GetComponent<Entity>().hidden && collision.gameObject.GetComponent<Entity>().hidden) && gameWorld.noGreenPastThis(transform.parent.gameObject.transform.position, collision.gameObject.transform.position)) //and no green between
        {
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = collision;
            transform.parent.gameObject.GetComponent<Patrollable>().lastSeenEnemyPosition = collision.gameObject.transform.position;
            transform.parent.gameObject.GetComponent<Patrollable>().didintCheckLastPosition = true;
            oneIterTrigger = true;
        }
        else if (isEnemyInsideFOV && (transform.parent.gameObject.GetComponent<Entity>().hidden && !collision.gameObject.GetComponent<Entity>().hidden) && gameWorld.noForestPastThisForest(transform.parent.gameObject.transform.position, collision.gameObject.transform.position)) //and no trees past this trees
        {
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = collision;
            transform.parent.gameObject.GetComponent<Patrollable>().lastSeenEnemyPosition = collision.gameObject.transform.position;
            transform.parent.gameObject.GetComponent<Patrollable>().didintCheckLastPosition = true;
            oneIterTrigger = true;
        }
        else if (isEnemyInsideFOV && collision.gameObject.GetComponent<Entity>().beingScanned)
        {
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = collision;
            transform.parent.gameObject.GetComponent<Patrollable>().lastSeenEnemyPosition = collision.gameObject.transform.position;
            transform.parent.gameObject.GetComponent<Patrollable>().didintCheckLastPosition = true;
            oneIterTrigger = true;
        }
        else if (isEnemyInsideFOV)
        {
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = null;
            if (oneIterTrigger)
            {
                transform.parent.gameObject.GetComponent<Patrollable>().lastSeenEnemyPosition = collision.gameObject.transform.position;
                transform.parent.gameObject.GetComponent<Patrollable>().didintCheckLastPosition = true;
                oneIterTrigger = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Entity>())
        {
            if (collision.gameObject.GetComponent<Entity>().isPlayer)
            {
                isEnemyInsideFOV = true;
                this.collision = collision;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Entity>())
        {
            if (collision.gameObject.GetComponent<Entity>().isPlayer)
            {
                isEnemyInsideFOV = false;
                this.collision = null;
                transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = null;
            }
        }
    }
}
