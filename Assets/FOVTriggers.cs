using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVTriggers : MonoBehaviour
{
    Collider2D collision = null;
    bool isEnemyInsideFOV = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnemyInsideFOV && (!transform.parent.gameObject.GetComponent<Entity>().hidden && !collision.gameObject.GetComponent<Entity>().hidden))
        {
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = collision;
            transform.parent.gameObject.GetComponent<Patrollable>().lastSeenEnemyPosition = collision.gameObject.transform.position;
            transform.parent.gameObject.GetComponent<Patrollable>().didintCheckLastPosition = true;
        }
        else if (isEnemyInsideFOV && (!transform.parent.gameObject.GetComponent<Entity>().hidden && collision.gameObject.GetComponent<Entity>().hidden))
        {
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = null;
        }
        else if (isEnemyInsideFOV && (transform.parent.gameObject.GetComponent<Entity>().hidden && collision.gameObject.GetComponent<Entity>().hidden))
        {
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = collision;
            transform.parent.gameObject.GetComponent<Patrollable>().lastSeenEnemyPosition = collision.gameObject.transform.position;
            transform.parent.gameObject.GetComponent<Patrollable>().didintCheckLastPosition = true;
        }
        else if (isEnemyInsideFOV)
        {
            transform.parent.gameObject.GetComponent<Patrollable>().enterTrigger = null;
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
