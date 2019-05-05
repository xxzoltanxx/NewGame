using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public Vector3 worldPosition;
    public GameWorld.Terrain weight;
    public int gridX;
    public int gridY;
    public bool battling = false;

    public int gCost;
    public int hCost;
    public int FCost { get { return gCost + hCost;  } }
    public HashSet<GameObject> entitiesCurrentlyOnTile = new HashSet<GameObject>();
    public PathNode parent;
    public PathNode(GameWorld.Terrain _weight, Vector3 _worldPos, int _X, int _Y)
    {
        weight = _weight;
        worldPosition = _worldPos;
        gridX = _X;
        gridY = _Y;
    }
}
