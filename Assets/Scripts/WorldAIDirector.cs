using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAIDirector : MonoBehaviour
{
    public GameObject patrolPrefab;
    public List<GameObject> villagesReadyToDispatch = new List<GameObject>();
    public List<GameObject> villagesReadyToReceive = new List<GameObject>();
    // Start is called before the first frame update
    private void Awake()
    {
    }

    private void lazyInit(GameObject patrolPrefab)
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
