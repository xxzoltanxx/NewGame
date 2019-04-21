using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NPCFOV : MonoBehaviour
{
    private GameWorld gameWorld;
    private Entity boundEntity;
    private GameObject childSpriteCircle;
    float distancePreviousFrame = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        boundEntity = GetComponent<Entity>();
        gameWorld = GameObject.Find("GameWorld").GetComponent<GameWorld>();
        childSpriteCircle = transform.GetChild(0).gameObject;
    }
    void Start()
    {
        float distance = getFOWBoundsDistance();
        childSpriteCircle.transform.localScale = new Vector2(distance * 2, distance * 2);
        distancePreviousFrame = distance;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = getFOWBoundsDistance();
        distance = Mathf.Lerp(distancePreviousFrame, distance, Time.deltaTime);
        childSpriteCircle.transform.localScale = new Vector2(distance * 2, distance * 2);
        distancePreviousFrame = distance;
    }

    private float getFOWBoundsDistance()
    {
        GameWorld.Terrain currentTerrain = gameWorld.getTerrainAtPoint(transform.position);
        float distance = boundEntity.fovDistance[currentTerrain];
        return distance;
    }
}
