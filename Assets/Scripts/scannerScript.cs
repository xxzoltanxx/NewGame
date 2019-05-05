using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scannerScript : MonoBehaviour
{
    Vector2 middle;
    float duration;
    float timer = 0;
    float expandEndTime = 0;
    float crunchStartTime = 0;
    private Material material;
    public List<GameObject> scannedEntities = new List<GameObject>();
    public Entity enemyEntityScanned = null;
    public Vector3 lastEnemyPosition = new Vector3(0, 0, 0);
    public Entity boundEntity = null;
    float dist = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init(float distance, Vector2 middle, float duration, Entity boundEntity)
    {

        material = GetComponent<MeshRenderer>().material;
        timer = 0;
        this.middle = middle;
        this.duration = duration;
        this.boundEntity = boundEntity;
        boundEntity.boundScanner = this;
        material.SetVector("_ScanPosition", new Vector4(middle.x, middle.y, 0, 0));
        material.SetFloat("_ScanDistance", 0);

        dist = distance;
        expandEndTime = duration * 0.1f;
        crunchStartTime = duration * 0.9f;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isFinished())
        {
            if (enemyEntityScanned)
            {
                lastEnemyPosition = enemyEntityScanned.transform.position;
            }
            timer += Time.deltaTime;
            if (timer > duration)
            {
                transform.localScale = new Vector3(0, 0, 0);
            }
            if (timer < expandEndTime)
            {
                float ss = Mathf.SmoothStep(0, dist, timer / expandEndTime);
                material.SetFloat("_ScanDistance", ss);
            }
            else if (timer > crunchStartTime && timer < duration)
            {
                float ss = dist - Mathf.SmoothStep(0, dist, (timer - crunchStartTime) / (duration - crunchStartTime));
                material.SetFloat("_ScanDistance", ss);
            }
        }
    }
    public bool isFinished()
    {
        return timer >= duration;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.gameObject.GetComponent<Entity>();
        if (entity != null)
        {
            if (entity.isPlayer)
            {
                enemyEntityScanned = entity;
            }
            scannedEntities.Add(collision.gameObject);
            entity.beingScanned = true;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {

    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        var entity = collision.gameObject.GetComponent<Entity>();
        if (entity != null)
        {
            if (entity.isPlayer)
            {
                enemyEntityScanned = null;
            }
            scannedEntities.Remove(collision.gameObject);
            entity.beingScanned = false;
        }
    }

    public void LateUpdate()
    {
        if (isFinished())
        {
            foreach(GameObject obj in scannedEntities)
            {
                obj.GetComponent<Entity>().beingScanned = false;
            }
            scannedEntities.Clear();
            enemyEntityScanned = null;
        }
    }
}
