using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVTriggers : MonoBehaviour
{
    Collider2D collision = null;
    bool isEnemyInsideFOV = false;
    private bool oneIterTrigger = false;
    private GameWorld gameWorld = null;
    // Start is called before the first frame update
    private void Awake()
    {
        gameWorld = GameObject.Find("GameWorld").GetComponent<GameWorld>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * 15.0f);
        if (isEnemyInsideFOV && (!transform.parent.gameObject.GetComponent<Entity>().hidden && !collision.gameObject.GetComponent<Entity>().hidden) && gameWorld.noForestPastThis(transform.parent.gameObject.transform.position,collision.gameObject.transform.position))
        {
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = collision;
            transform.parent.gameObject.GetComponent<Patrollable>().lastSeenEnemyPosition = collision.gameObject.transform.position;
            transform.parent.gameObject.GetComponent<Patrollable>().didintCheckLastPosition = true;
            oneIterTrigger = true;
        }
        else if (isEnemyInsideFOV && (!transform.parent.gameObject.GetComponent<Entity>().hidden && collision.gameObject.GetComponent<Entity>().hidden))
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
