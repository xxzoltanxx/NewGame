using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAIDirector : MonoBehaviour
{
    public GameObject patrolPrefab;
    public List<GameObject> villagesReadyToDispatch = new List<GameObject>();
    public List<GameObject> villagesReadyToReceive = new List<GameObject>();

    public float villagePatrolModifier = 0.5f;
    public float villageSupplyRunModifier = 0.33f;
    // Start is called before the first frame update
    private void Awake()
    {
    }

    public void lazyInit(GameObject patrolPrefab)
    {
        this.patrolPrefab = patrolPrefab;
    }
    public void addToDispatchers(GameObject obj)
    {
        foreach (var village in villagesReadyToReceive)
        {
            if (village != obj)
            {
                villagesReadyToReceive.Remove(village);
                var patrol = GameObject.Instantiate(patrolPrefab, obj.transform.position, Quaternion.identity);
                patrol.GetComponent<Patrollable>().lazyInit(village.transform.position, obj);
                if (village.GetComponent<VillageScript>().needSupply)
                {
                    patrol.GetComponent<Entity>().SetSoldiers((int)Mathf.Ceil(villageSupplyRunModifier * obj.GetComponent<VillageScript>().boundSoldiers));
                }
                else
                {
                    patrol.GetComponent<Entity>().SetSoldiers((int)Mathf.Ceil(villagePatrolModifier * obj.GetComponent<VillageScript>().boundSoldiers));
                }
                village.GetComponent<VillageScript>().addToReceiving(patrol);
                obj.GetComponent<VillageScript>().AddToSent(patrol);
                return;
            }
        }
        villagesReadyToDispatch.Add(obj);
    }

    public void addToReceivers(GameObject obj)
    {
        foreach (var village in villagesReadyToDispatch)
        {
            if (village != obj)
            {
                villagesReadyToDispatch.Remove(village);
                var patrol = GameObject.Instantiate(patrolPrefab, village.transform.position, Quaternion.identity);
                patrol.GetComponent<Patrollable>().lazyInit(obj.transform.position, village);
                if (obj.GetComponent<VillageScript>().needSupply)
                {
                    patrol.GetComponent<Entity>().SetSoldiers((int)Mathf.Ceil(villageSupplyRunModifier * village.GetComponent<VillageScript>().boundSoldiers));
                }
                else
                {
                    patrol.GetComponent<Entity>().SetSoldiers((int)Mathf.Ceil(villagePatrolModifier * village.GetComponent<VillageScript>().boundSoldiers));
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
