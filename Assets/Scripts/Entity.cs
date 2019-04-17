using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Soldier[] roster;
    public float pace = 1;
    public float speed = 1;
    public bool hidden = false;
    public Dictionary<GameWorld.Terrain, float> terrainModifiers;
    public Dictionary<GameWorld.Terrain, float> fovDistance;
    public Dictionary<GameWorld.Terrain, int> pathfindingWeights;
    GameWorld.Terrain currentTile;
    public float ambushValue;
    private void Awake()
    {
        fovDistance = new Dictionary<GameWorld.Terrain, float>();
        fovDistance[GameWorld.Terrain.Jungle] = 2.0f;
        fovDistance[GameWorld.Terrain.Water] = 1.5f;
        fovDistance[GameWorld.Terrain.Road] = 3.0f;
        fovDistance[GameWorld.Terrain.Village] = 1.0f;
        fovDistance[GameWorld.Terrain.Forest] = 1.5f;
        fovDistance[GameWorld.Terrain.Mountain] = 3.5f;

        terrainModifiers = new Dictionary<GameWorld.Terrain, float>();
        terrainModifiers[GameWorld.Terrain.Jungle] = 0.8f;
        terrainModifiers[GameWorld.Terrain.Water] = 0.2f;
        terrainModifiers[GameWorld.Terrain.Road] = 1;
        terrainModifiers[GameWorld.Terrain.Village] = 0.8f;
        terrainModifiers[GameWorld.Terrain.Forest] = 0.5f;
        terrainModifiers[GameWorld.Terrain.Mountain] = 0.3f;

        pathfindingWeights = new Dictionary<GameWorld.Terrain,int>();
        pathfindingWeights[GameWorld.Terrain.Jungle] = 20;
        pathfindingWeights[GameWorld.Terrain.Water] = 400;
        pathfindingWeights[GameWorld.Terrain.Road] = 1;
        pathfindingWeights[GameWorld.Terrain.Village] = 80;
        pathfindingWeights[GameWorld.Terrain.Forest] = 40;
        pathfindingWeights[GameWorld.Terrain.Mountain] = 60;
    }
}
