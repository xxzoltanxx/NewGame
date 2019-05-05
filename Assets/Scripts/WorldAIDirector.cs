using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAIDirector : MonoBehaviour
{
    public float ScannerZ = -1.8f;
    public GameObject patrolPrefab;
    public GameObject radarPrefab;
    public GameWorld gameWorld;
    public List<GameObject> villagesReadyToDispatch = new List<GameObject>();
    public List<GameObject> villagesReadyToReceive = new List<GameObject>();
    public List<GameObject> scannerPool = new List<GameObject>();
    public PatrolSprites patrolSprites;

    public float villagePatrolModifier = 0.5f;
    public float villageSupplyRunModifier = 0.33f;
    public float hunterModifier = 0.8f;
    // Start is called before the first frame update
    private void Awake()
    {
        patrolSprites = GameObject.Find("GameManager").GetComponent<PatrolSprites>();
        gameWorld = GameObject.Find("GameWorld").GetComponent<GameWorld>();
    }

    public void lazyInit(GameObject patrolPrefab, GameObject radarPrefab)
    {
        this.patrolPrefab = patrolPrefab;
        this.radarPrefab = radarPrefab;
    }

    public void DispatchHunters(Vector2 pos)
    {
        TransformedSector sector = gameWorld.sectorFromWorldPos(pos);
        var village = sector.village;
        var patrol = GameObject.Instantiate(patrolPrefab, village.transform.position, Quaternion.identity);
        patrol.GetComponent<Patrollable>().lazyInit(pos, village, village);
        patrol.GetComponent<Entity>().SetSoldiers((int)Mathf.Ceil(hunterModifier * village.GetComponent<VillageScript>().boundSoldiers));
        patrol.GetComponent<SpriteRenderer>().sprite = patrolSprites.patrolSprite;
        patrol.GetComponent<Patrollable>().isHunting = true;
        patrol.GetComponent<Patrollable>().isEscaping = true;
        village.GetComponent<VillageScript>().addToReceiving(patrol);
        patrol.GetComponent<Patrollable>().reset(pos);
        patrol.GetComponent<Animator>().SetTrigger("hunt");
    }

    public void addToDispatchers(GameObject obj)
    {
        foreach (var village in villagesReadyToReceive)
        {
            if (village != obj)
            {
                villagesReadyToReceive.Remove(village);
                var patrol = GameObject.Instantiate(patrolPrefab, obj.transform.position, Quaternion.identity);
                patrol.GetComponent<Patrollable>().lazyInit(village.transform.position, obj, village);
                if (village.GetComponent<VillageScript>().needSupply)
                {
                    patrol.GetComponent<Entity>().SetSoldiers((int)Mathf.Ceil(villageSupplyRunModifier * obj.GetComponent<VillageScript>().boundSoldiers));
                    patrol.GetComponent<SpriteRenderer>().sprite = patrolSprites.patrolSprite;
                }
                else
                {
                    patrol.GetComponent<Entity>().SetSoldiers((int)Mathf.Ceil(villagePatrolModifier * obj.GetComponent<VillageScript>().boundSoldiers));
                    patrol.GetComponent<SpriteRenderer>().sprite = patrolSprites.patrolSprite;
                }
                village.GetComponent<VillageScript>().addToReceiving(patrol);
                obj.GetComponent<VillageScript>().AddToSent(patrol);
                return;
            }
        }
        villagesReadyToDispatch.Add(obj);
    }

    public GameObject ScanSector(float distance, Vector2 middle, float duration, Entity boundEntity)
    {
        GameObject scanner = null;
        foreach (var scanr in scannerPool)
        {
            if (scanr.GetComponent<scannerScript>().isFinished())
            {
                scanner = scanr;
            }
        }
        if (scanner == null)
        {
            scanner = GameObject.Instantiate(radarPrefab);
            scannerPool.Add(scanner);
        }
        scanner.transform.localScale = new Vector3(distance, distance, 0);
        //+0.02f because the mesh is fucked up and is shifted (fuck blender)
        scanner.transform.position = new Vector3(middle.x, middle.y + 0.02f, ScannerZ);
        scanner.GetComponent<scannerScript>().Init(distance, middle, duration, boundEntity);
        return scanner;
    }
    public void addToReceivers(GameObject obj)
    {
        foreach (var village in villagesReadyToDispatch)
        {
            if (village != obj)
            {
                villagesReadyToDispatch.Remove(village);
                var patrol = GameObject.Instantiate(patrolPrefab, village.transform.position, Quaternion.identity);
                patrol.GetComponent<Patrollable>().lazyInit(obj.transform.position, village, obj);
                if (obj.GetComponent<VillageScript>().needSupply)
                {
                    patrol.GetComponent<Entity>().SetSoldiers((int)Mathf.Ceil(villageSupplyRunModifier * village.GetComponent<VillageScript>().boundSoldiers));
                    patrol.GetComponent<SpriteRenderer>().sprite = patrolSprites.patrolSprite;
                }
                else
                {
                    patrol.GetComponent<Entity>().SetSoldiers((int)Mathf.Ceil(villagePatrolModifier * village.GetComponent<VillageScript>().boundSoldiers));
                    patrol.GetComponent<SpriteRenderer>().sprite = patrolSprites.patrolSprite;
                }

                village.GetComponent<VillageScript>().AddToSent(patrol);
                obj.GetComponent<VillageScript>().addToReceiving(patrol);
                return;
            }
        }
        villagesReadyToReceive.Add(obj);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
