using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageScript : MonoBehaviour
{
    private AffinityBar affinityBar = null;
    public string name = "";
    public float affinity = 0;
    public int boundSoldiers = 0;
    public int boundSupplyWagons = 0;
    // Start is called before the first frame update
    void Awake()
    {
        affinityBar = transform.gameObject.GetComponentInChildren<AffinityBar>();
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
    public void setBoundSupplyWagons(int wagons)
    {
        boundSupplyWagons = wagons;
        affinityBar.setBoundSupplyWagons(wagons);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
