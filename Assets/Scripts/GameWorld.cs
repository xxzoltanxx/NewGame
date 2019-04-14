﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    private WorldGenerator generator;
    private WorldMesh mesh;
    public enum Terrain
    {
        Road,
        Village,
        Water,
        Jungle,
        Mountain,
        Forest
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Terrain getTerrainAtPoint(Vector2 position)
    {
        generator = GetComponent<WorldGenerator>();
        mesh = GetComponent<WorldMesh>();
        Vector2 LL = new Vector2(transform.position.x - mesh.totalSize.x / 2.0f,transform.position.y -mesh.totalSize.y / 2.0f);
        Vector2 gridSpace = position - LL;
        Vector2 normalized = new Vector2(gridSpace.x / mesh.totalSize.x, gridSpace.y / mesh.totalSize.y);
        Vector2 tiles = new Vector2(normalized.x * generator.width, normalized.y * generator.height);
        WorldTextureAtlas.Tiles tile = generator.tileMap[(int)tiles.x,(int) tiles.y];

        if (tile == WorldTextureAtlas.Tiles.GrassBasic)
        {
            return Terrain.Jungle;
        }
        else if ((int) tile >= 1 && (int) tile <= 16)
        {
            return Terrain.Water;
        }
        else if ((int) tile == 18)
        {
            return Terrain.Forest;
        }
        else if ((int) tile == 17)
        {
            return Terrain.Mountain;
        }
        else if ((int) tile == 19)
        {
            return Terrain.Village;
        }
        else if ((int) tile == 20)
        {
            return Terrain.Road;
        }
        else
        {
            return Terrain.Jungle;
        }
    }
}
