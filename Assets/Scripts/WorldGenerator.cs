using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Sector
{
    public Sector(int x, int y, Rect rec)
    {
        xVillage = x;
        yVillage = y;
        sectorRect = rec;
    }
    public int xVillage, yVillage;
    public Rect sectorRect;
};

public class WorldGenerator : MonoBehaviour
{
    public float forestThreshold = 0.2f;
    public float minRand = -3;
    public float maxRand = 3;
    public float waterMinMargin = -3;
    public float waterMaxMargin = -2;
    public float grassMinMargin = 1;
    public float grassMaxMargin = 3;
    public float mountainMaxMargin = 4;
    public float humidityMin = -1.0f;
    public float humidityMax = 1.0f;
    public int width = 513;
    public int height = 513;
    public int numDiamondSquares = 8;
    public float[,] world;
    public float[,] humidityMap;
    public float deepWaterNum = 0.3f;
    public float mountainNum = 0.3f;
    public float grassLandsNum = 0.4f;
    public int featureSizeForest = 4;
    public float roughness = 0.1f;
    public float humidityRoughness = 0.1f;
    public int waterAreaTreshold = 9;
    public int mountainAreaTreshold = 9;
    public int forestAreaTreshold = 9;
    public int bufferArea = 3;

    //Just use this outgoing
    public List<Sector> sectors = new List<Sector>();
    public WorldTextureAtlas.Tiles[,] tileMap;


    public int villagesNum = 10;
    // Start is called before the first frame update
    private void Awake()
    {
    }
    List<float> constructCornerValues()
    {
        List<float> cornerValues = new List<float>();
        int numOfCornerValues = (numDiamondSquares + 1) * (numDiamondSquares + 1);
        float numOfDeepWater = numOfCornerValues * deepWaterNum;
        float numOfMountain = numOfCornerValues * mountainNum;
        float numOfGrassLands = numOfCornerValues * grassLandsNum;

        float leftOver = (numOfDeepWater - Mathf.Floor(numOfDeepWater)) + (numOfMountain - Mathf.Floor(numOfMountain)) + (numOfGrassLands - Mathf.Floor(numOfGrassLands));
        int remaining = Mathf.CeilToInt(leftOver) * numOfCornerValues;

        for (int i = 0; i < (int)numOfDeepWater; ++i)
        {
            cornerValues.Add(waterMinMargin);//Random.Range(minRand, waterMaxMargin));
        }
        for (int i = 0; i < (int)numOfMountain; ++i)
        {
            cornerValues.Add(mountainMaxMargin);// Random.Range(grassMaxMargin, maxRand));
        }
        for (int i = 0; i < (int)numOfGrassLands; ++i)
        {
            cornerValues.Add(Random.Range(grassMinMargin, grassMaxMargin));
        }

        for (int i = 0; i < (int) remaining; ++i)
        {
            cornerValues.Add(Random.Range(grassMinMargin, grassMaxMargin));
        }
        cornerValues.Shuffle();
        return cornerValues;
    }
    public void Construct()
    {
        List<float> cornerValues = constructCornerValues();
        world = new float[width, height];
        humidityMap = new float[width, height];
        tileMap = new WorldTextureAtlas.Tiles[width, height];
        int stepSize = (width - 1) / numDiamondSquares;
        int initialWidth = stepSize + 1;
        int initialHeight = stepSize + 1;

        humidityMap[0,0] = Random.Range(humidityMin, humidityMax);
        humidityMap[0, height - 1] = Random.Range(humidityMin, humidityMax);
        humidityMap[width - 1, 0] = Random.Range(humidityMin, humidityMax);
        humidityMap[width - 1, height - 1] = Random.Range(humidityMin, humidityMax);

        int humidityStepSize = width - 1;
        DiamondSquare(humidityMap, humidityMin, humidityMax, humidityStepSize, ((width - 1) / 2), ((height - 1)) / 2, width, height, humidityRoughness, true);
        for (int i = 0; i < numDiamondSquares; ++i)
            for (int j = 0; j < numDiamondSquares; ++j)
            {
                int x = stepSize / 2 + i * stepSize;
                int y = stepSize / 2 + j * stepSize;
                if (i == 0)
                {
                    world[x - stepSize / 2, y - stepSize / 2] = cornerValues[0];
                    cornerValues.RemoveAt(0);
                    world[x - stepSize / 2, y + stepSize / 2] = cornerValues[0];
                    cornerValues.RemoveAt(0);
                }
                if (j == 0)
                {
                    world[x + stepSize / 2, y - stepSize / 2] = cornerValues[0];
                    cornerValues.RemoveAt(0);
                }
                world[x + stepSize / 2, y + stepSize / 2] = cornerValues[0];
                cornerValues.RemoveAt(0);
            }
        for (int i = 0; i < numDiamondSquares; ++i)
            for (int j = 0; j < numDiamondSquares; ++j)
            {
                int x = stepSize / 2 + i * stepSize;
                int y = stepSize / 2 + j * stepSize;
                DiamondSquare(world, minRand, maxRand, stepSize, x, y, initialWidth, initialHeight, roughness, true);
            }


        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                if (world[i, j] < waterMaxMargin)
                {
                    tileMap[i, j] = WorldTextureAtlas.Tiles.WaterNone;
                }
                else if (world[i, j] < grassMaxMargin && humidityMap[i, j] < forestThreshold)
                {
                    tileMap[i, j] = WorldTextureAtlas.Tiles.GrassBasic;
                }
                else if (world[i, j] < grassMaxMargin && humidityMap[i, j] > forestThreshold)
                {
                    tileMap[i, j] = WorldTextureAtlas.Tiles.Tree;
                }
                else if (world[i, j] > grassMaxMargin)
                {
                    tileMap[i, j] = WorldTextureAtlas.Tiles.Mountain;
                }
            }

        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                if (tileMap[i,j] == WorldTextureAtlas.Tiles.WaterNone)
                {
                    List<Vector2> tiles = new List<Vector2>();
                    sumArea(i, j, tiles, WorldTextureAtlas.Tiles.WaterNone);
                    if (tiles.Count < waterAreaTreshold)
                    {
                        foreach (Vector2 tile in tiles)
                        {
                            tileMap[(int)tile.x, (int)tile.y] = WorldTextureAtlas.Tiles.GrassBasic;
                        }
                    }
                }
            }
        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                if (tileMap[i, j] == WorldTextureAtlas.Tiles.Mountain)
                {
                    List<Vector2> tiles = new List<Vector2>();
                    sumArea(i, j, tiles, WorldTextureAtlas.Tiles.Mountain);
                    if (tiles.Count < mountainAreaTreshold)
                    {
                        foreach (Vector2 tile in tiles)
                        {
                            tileMap[(int)tile.x, (int)tile.y] = WorldTextureAtlas.Tiles.GrassBasic;
                        }
                    }
                }
            }

        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                if (tileMap[i, j] == WorldTextureAtlas.Tiles.Tree)
                {
                    List<Vector2> tiles = new List<Vector2>();
                    sumArea(i, j, tiles, WorldTextureAtlas.Tiles.Tree);
                    if (tiles.Count < forestAreaTreshold)
                    {
                        foreach (Vector2 tile in tiles)
                        {
                            tileMap[(int)tile.x, (int)tile.y] = WorldTextureAtlas.Tiles.GrassBasic;
                        }
                    }
                }
            }
        for (int i = 0; i < 5; ++i)
        {
            cullLandSuroundedByWater();
            cullWaterSurroundedByGrass();
        }

        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                if (tileMap[i,j] == WorldTextureAtlas.Tiles.GrassBasic && humidityMap[i, j] > forestThreshold)
                {
                    tileMap[i, j] = WorldTextureAtlas.Tiles.Tree;
                }
            }

        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                if (tileMap[i, j] == WorldTextureAtlas.Tiles.Tree)
                {
                    List<Vector2> tiles = new List<Vector2>();
                    sumArea(i, j, tiles, WorldTextureAtlas.Tiles.Tree);
                    if (tiles.Count < forestAreaTreshold)
                    {
                        foreach (Vector2 tile in tiles)
                        {
                            tileMap[(int)tile.x, (int)tile.y] = WorldTextureAtlas.Tiles.GrassBasic;
                        }
                    }
                }
            }
        AddVillages();
        UpdateWaterSprites();
    }

    public void cullWaterSurroundedByGrass()
    {
        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                //L - 1
                //R - 2
                //U - 4
                //D - 8
                if (!isNotWater(tileMap[i, j]))
                {
                    int sides = 0;
                    if (i > 0)
                    {
                        if (isNotWater(tileMap[i - 1, j]))
                            ++sides;
                    }
                    if (i < width - 1)
                        if (isNotWater(tileMap[i + 1, j]))
                            ++sides;
                    if (j > 0)
                        if (isNotWater(tileMap[i, j - 1]))
                            ++sides;
                    if (j < height - 1)
                        if (isNotWater(tileMap[i, j + 1]))
                            ++sides;

                    if (sides >= 3)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.GrassBasic;
                }
            }
    }
    public void cullLandSuroundedByWater()
    {
        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                //L - 1
                //R - 2
                //U - 4
                //D - 8
                if (isNotWater(tileMap[i, j]))
                {
                    int sides = 0;
                    if (i > 0)
                    {
                        if (!isNotWater(tileMap[i - 1, j]))
                            ++sides;
                    }
                    if (i < width - 1)
                        if (!isNotWater(tileMap[i + 1, j]))
                            ++sides;
                    if (j > 0)
                        if (!isNotWater(tileMap[i, j - 1]))
                            ++sides;
                    if (j < height - 1)
                        if (!isNotWater(tileMap[i, j + 1]))
                            ++sides;

                    if (sides >= 3)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterNone;
                }
            }
    }
    public void UpdateWaterSprites()
    {
        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                //L - 1
                //R - 2
                //U - 4
                //D - 8
                if (!isNotWater(tileMap[i, j]))
                {
                    int flag = 0;
                    if (i > 0)
                    {
                        if (isNotWater(tileMap[i - 1, j]))
                            flag |= 1;
                    }
                    if (i < width - 1)
                        if (isNotWater(tileMap[i + 1, j]))
                            flag |= 2;
                    if (j > 0)
                        if (isNotWater(tileMap[i, j - 1]))
                            flag |= 8;
                    if (j < height - 1)
                        if (isNotWater(tileMap[i, j + 1]))
                            flag |= 4;

                    if (flag == 1)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterL;
                    else if (flag == 2)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterR;
                    else if (flag == 4)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterU;
                    else if (flag == 8)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterD;
                    else if (flag == 3)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterLR;
                    else if (flag == 5)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterLU;
                    else if (flag == 6)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterRU;
                    else if (flag == 7)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterLRU;
                    else if (flag == 9)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterDL;
                    else if (flag == 10)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterRD;
                    else if (flag == 11)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterLRD;
                    else if (flag == 12)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterDU;
                    else if (flag == 13)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterLUD;
                    else if (flag == 14)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.WaterRUD;
                    else if (flag == 15)
                        tileMap[i, j] = WorldTextureAtlas.Tiles.GrassBasic;
                }
            }
    }
    private void AddVillages()
    {
        int villageArea = (width - 1) * (height - 1) / villagesNum;
        int len = (int)Mathf.Sqrt(villageArea);
        for (int i = bufferArea; i < width - len; i += len)
            for (int j = bufferArea; j < height - len; j += len)
            {
                int x = i;
                int y = j;
                Rect rect = new Rect(new Vector2(i, j), new Vector2(len, len));
                float distance = findDistanceFromWater(x, y, rect);
                for (int g = i; g < i + len; ++g)
                    for (int w = j; w < j + len; ++w)
                    {
                        float iterDistance = findDistanceFromWater(g, w, rect);
                        if (iterDistance >= distance)
                        {
                            distance = iterDistance;
                            x = g;
                            y = w;
                        }
                    }
                //Change it later
                //Or just change the texture
                tileMap[x, y] = WorldTextureAtlas.Tiles.Village;
                sectors.Add(new Sector(x, y, rect));
            }
    }

    private float findDistanceFromWater(int x, int y, Rect rect)
    {
        float distance = 20;
        for (int i = (int)rect.x - (int)rect.width; i < (int)rect.x + (int) rect.width + 10; ++i)
            for (int j = (int)rect.y - (int)rect.height; j < (int)rect.y + (int) rect.height + 10; ++j)
            {
                if (i < 0 || i > width - 1 || j < 0 || j > height - 1) continue;
                if ((!WorldGenerator.isNotWater(tileMap[i,j]) || tileMap[i,j] == WorldTextureAtlas.Tiles.Village) && Vector2.Distance(new Vector2(i,j), new Vector2(x,y)) < distance)
                {
                    distance = Vector2.Distance(new Vector2(i, j), new Vector2(x, y));
                }
            }
        return distance;
    }
    private void sumArea(int x, int y, List<Vector2> tiles, WorldTextureAtlas.Tiles tile)
    {
        tiles.Add(new Vector2(x, y));
        if (x > 0 && tileMap[x - 1, y] == tile && !tiles.Contains(new Vector2(x - 1, y)))
        {
            sumArea(x - 1, y,tiles, tile);
        }
        if (x < width-1 && tileMap[x + 1, y] == tile && !tiles.Contains(new Vector2(x + 1, y)))
        {
            sumArea(x + 1, y, tiles, tile);
        }
        if (y > 0 && tileMap[x, y - 1] == tile && !tiles.Contains(new Vector2(x, y - 1)))
        {
            sumArea(x, y - 1,tiles, tile);
        }
        if (y < height - 1 && tileMap[x, y + 1] == tile && !tiles.Contains(new Vector2(x, y + 1)))
        {
            sumArea(x, y + 1, tiles, tile);
        }
    }
    public static bool isNotWater(WorldTextureAtlas.Tiles tile)
    {
        if (tile == WorldTextureAtlas.Tiles.GrassBasic || tile == WorldTextureAtlas.Tiles.Tree || tile == WorldTextureAtlas.Tiles.Mountain || tile == WorldTextureAtlas.Tiles.Road || tile == WorldTextureAtlas.Tiles.Village)
            return true;
        return false;
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void DiamondSquare(float[,] world, float minRand, float maxRand, int stepSize, int x, int y, int initialWidth, int initialHeight, float roughness, bool increaseMin = true)
    {
        while (stepSize > 1)
        {
            int g = 0;
            for (int i = x - (initialWidth - 1)/2 + stepSize / 2; i < x + (initialWidth - 1)/2; i += stepSize / 2)
                for (int j = y - (initialHeight - 1)/2 + stepSize / 2; j < y + (initialHeight - 1)/2; j += stepSize / 2)
                    DiamondStep(stepSize, i, j, world, minRand, maxRand);
            for (int i = x - (initialWidth - 1) / 2 + stepSize / 2; i < x + (initialWidth - 1) / 2; i += stepSize / 2)
                for (int j = y - (initialHeight - 1) / 2 + stepSize / 2; j < y + (initialHeight - 1) / 2; j += stepSize / 2)
                    SquareStep(stepSize, i, j, world, minRand, maxRand);
            stepSize /= 2;
            if (increaseMin)
            {
                minRand = minRand * Mathf.Pow(2, -roughness);
            }
            maxRand = maxRand * Mathf.Pow(2, -roughness);
        }
    }

    void DiamondStep(int stepSize, int x, int y, float[,] world, float minRand, float maxRand)
    {
        if (stepSize == 1) return;
        float LD = world[x - stepSize / 2, y - stepSize / 2];
        float LU = world[x - stepSize / 2, y + stepSize / 2];
        float RD = world[x + stepSize / 2, y - stepSize / 2];
        float RU = world[x + stepSize / 2, y + stepSize / 2];

        float result = (LD + LU + RD + RU) / 4.0f + Random.Range(minRand, maxRand);
        world[x, y] = result;
    }

    void SquareStep(int stepSize, int x, int y, float[,] world, float minRand, float maxRand)
    {
        int keepx = x;
        int keepy = y;
        if (stepSize == 0) return;
        for (x = x - stepSize / 2; x <= keepx + stepSize / 2; x += stepSize)
        {
            y = keepy;
            {
                int c = 0;
                float sum = 0;
                if (x > 0)
                {
                    sum += world[x - stepSize / 2, y];
                    ++c;
                }
                if (y > 0)
                {
                    sum += world[x, y - stepSize / 2];
                    ++c;
                }
                if (y < height - 1)
                {
                    sum += world[x, y + stepSize / 2];
                    ++c;
                }
                if (x < width - 1)
                {
                    sum += world[x + stepSize / 2, y];
                    ++c;
                }
                float average = sum / c + Random.Range(minRand, maxRand);
                world[x, y] = average;
            }
        }
        for (y = y - stepSize / 2; y <= keepy + stepSize / 2; y += stepSize)
        {
            x = keepx;
            {
                int c = 0;
                float sum = 0;
                if (x > 0)
                {
                    sum += world[x - stepSize / 2, y];
                    ++c;
                }
                if (y > 0)
                {
                    sum += world[x, y - stepSize / 2];
                    ++c;
                }
                if (y < height - 1)
                {
                    sum += world[x, y + stepSize / 2];
                    ++c;
                }
                if (x < width - 1)
                {
                    sum += world[x + stepSize / 2, y];
                    ++c;
                }
                float average = sum / c + Random.Range(minRand, maxRand);
                world[x, y] = average;
            }
        }
    }
}

static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0,n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}