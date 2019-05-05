using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    public bool beingScanned = false;
    public float viewingDistance = 0.0f;
    public Abilities abilities;
    public Text soldierAmountText;
    public GameObject dissapearPrefab;
    public List<Soldier> roster;
    //Replace this later with roster
    public int soldierAmount = 3;
    public float pace = 1;
    public float speed = 1;
    public bool hidden = false;
    public bool isPlayer = false;
    public bool hiddenInPlainSight = false;
    public bool reappear = false;
    public Dictionary<GameWorld.Terrain, float> terrainModifiers;
    public Dictionary<GameWorld.Terrain, float> fovDistance;
    public Dictionary<GameWorld.Terrain, int> pathfindingWeights;
    public scannerScript boundScanner = null;
    public float nightFOVModifier = 0.7f;
    GameWorld.Terrain currentTile;
    public float ambushValue;

    public void Dissapear()
    {
        reappear = false;
        var gj = GameObject.Instantiate(dissapearPrefab, transform, false );
        gj.transform.localPosition = new Vector3(gj.transform.localPosition.x, gj.transform.localPosition.y, -1);
        Color color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0.05f);
    }

    public void Reappear()
    {
        reappear = true;
    }

    private void Update()
    {
        if (reappear)
        {
            Color color = GetComponent<SpriteRenderer>().color;
            float a = color.a;
            a = Mathf.Clamp(a + Time.deltaTime, 0, 1.0f);
            GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, a);
            if (a == 1.0f)
            {
                reappear = false;
            }
        }
    }
    private void Awake()
    {
        abilities = GetComponent<Abilities>();
        if (gameObject.name != "GameWorld")
            soldierAmountText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
        fovDistance = new Dictionary<GameWorld.Terrain, float>();
        fovDistance[GameWorld.Terrain.Jungle] = 2.0f;
        fovDistance[GameWorld.Terrain.Water] = 1.5f;
        fovDistance[GameWorld.Terrain.Road] = 2.0f;
        fovDistance[GameWorld.Terrain.Village] = 1.0f;
        fovDistance[GameWorld.Terrain.Forest] = 0.8f;
        fovDistance[GameWorld.Terrain.Mountain] = 3.5f;

        terrainModifiers = new Dictionary<GameWorld.Terrain, float>();
        terrainModifiers[GameWorld.Terrain.Jungle] = 0.6f;
        terrainModifiers[GameWorld.Terrain.Water] = 0.2f;
        terrainModifiers[GameWorld.Terrain.Road] = 0.7f;
        terrainModifiers[GameWorld.Terrain.Village] = 0.4f;
        terrainModifiers[GameWorld.Terrain.Forest] = 0.4f;
        terrainModifiers[GameWorld.Terrain.Mountain] = 0.3f;

        pathfindingWeights = new Dictionary<GameWorld.Terrain,int>();
        pathfindingWeights[GameWorld.Terrain.Jungle] = 20;
        pathfindingWeights[GameWorld.Terrain.Water] = 400;
        pathfindingWeights[GameWorld.Terrain.Road] = 1;
        pathfindingWeights[GameWorld.Terrain.Village] = 80;
        pathfindingWeights[GameWorld.Terrain.Forest] = 40;
        pathfindingWeights[GameWorld.Terrain.Mountain] = 60;
    }

    public void SetSoldiers(int s)
    {
        soldierAmount = s;
        soldierAmountText.text = s.ToString();
    }
    public void LazyInit(List<Soldier> soldiers)
    {
        roster = soldiers;
        soldierAmountText.text = roster.Count.ToString();
    }
}
