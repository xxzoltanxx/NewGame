using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageScript : MonoBehaviour
{
    private WorldAIDirector director = null;
    private AffinityBar affinityBar = null;
    private TransformedSector sector;
    public string name = "";
    public float patrol;
    public float affinity = 0;
    public int boundSoldiers = 0;
    public bool needSupply = false;

    public List<GameObject> receivingPatrols = new List<GameObject>();
    public List<GameObject> sentPatrols = new List<GameObject>();
    // Start is called before the first frame update
    void Awake()
    {
        affinityBar = transform.gameObject.GetComponentInChildren<AffinityBar>();
        director = GameObject.Find("GameManager").GetComponent<WorldAIDirector>();
    }

    public void initFresh(TransformedSector sector, WorldAIDirector director, string name, int soldiers, bool needsSup)
    {
        this.director = director;
        this.sector = sector;
        setBoundSoldiers(soldiers);
        director.addToDispatchers(gameObject);
        director.addToReceivers(gameObject);
        needSupply = needsSup;
        setName(name);
    }

    public void setName(string _name)
    {
        name = _name;
        affinityBar.setName(name);
    }
    public void setAffinity(float _affinity)
    {
        _affinity = Mathf.Clamp01(_affinity);
        this.affinity = _affinity;
        affinityBar.setAffinityPercentage(affinity);
    }
    public void setBoundSoldiers(int soldiers)
    {
        boundSoldiers = soldiers;
        affinityBar.setBoundSoldiers(soldiers);
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void addToReceiving(GameObject obj)
    {
        receivingPatrols.Add(obj);
    }

    public void AddToSent(GameObject obj)
    {
        sentPatrols.Add(obj);
    }

    public void RemoveFromReceiving(GameObject obj)
    {
        receivingPatrols.Remove(obj);
        StartCoroutine("scheduleForReceiversAfterDelay");
    }

    public void RemoveFromSent(GameObject obj)
    {
        sentPatrols.Remove(obj);
        StartCoroutine("scheduleForDispatchersAfterDelay");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (receivingPatrols.Contains(collision.gameObject))
        {
            needSupply = !needSupply;
            collision.gameObject.GetComponent<Patrollable>().boundVillage.GetComponent<VillageScript>().RemoveFromSent(collision.gameObject);

            RemoveFromReceiving(collision.gameObject);
            collision.gameObject.GetComponent<FovFadeable>().Dissapear();
        }
    }

    IEnumerator scheduleForDispatchersAfterDelay()
    {
        yield return new WaitForSeconds(2.0f + Random.Range(-0.5f, 0.5f));
        director.addToDispatchers(gameObject);
    }

    IEnumerator scheduleForReceiversAfterDelay()
    {
        yield return new WaitForSeconds(2.0f + Random.Range(-0.5f, 0.5f));
        director.addToReceivers(gameObject);
    }
}
