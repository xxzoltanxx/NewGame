using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformedSector
{
    public Rect sector;
}

[RequireComponent(typeof(WorldGenerator))]
[RequireComponent(typeof(WorldTextureAtlas))]
public class WorldMesh : MonoBehaviour
{
    public Material material;
    public float baseMeshZLevel = 0;
    public float villageZLevel = -1.5f;
    public Vector2 tileSize = new Vector2(5.0f, 5.0f);
    private WorldGenerator generator;
    private WorldTextureAtlas textureAtlas;
    public GameObject villagePrefab;
    private WorldAIDirector worldAI;
    public Vector2 totalSize;
    public GameObject linePrefab;
    public int distanceToScanVillages = 10;
    private PathGrid pathGrid;
    public GameManager gameManager;
    public ParametersDDOL parameters;
    public int roadZ = -1;
    public int initialSoldiersPerVillage = 10;

    public List<TransformedSector> sectors = new List<TransformedSector>();
    // Start is called before the first frame update
    void Awake()
    {
        worldAI = GameObject.Find("GameManager").GetComponent<WorldAIDirector>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        parameters = gameManager.gameObject.GetComponent<ParametersDDOL>();
        generator = GetComponent<WorldGenerator>();
        totalSize = new Vector2(tileSize.x * generator.width, tileSize.y * generator.height);
        GetComponent<BoxCollider2D>().size = new Vector2(totalSize.x, totalSize.y);
        textureAtlas = GetComponent<WorldTextureAtlas>();
        pathGrid = GetComponent<PathGrid>();

        generator.Construct();
        textureAtlas.Construct();
        pathGrid.CreateGrid();
        worldAI.lazyInit(Resources.Load("Prefabs/enemy") as GameObject);
    }
    private void Start()
    {
        //TEST CODE
        AddRoads();
        AddVillageSprites();
        Mesh mesh = constructBaseMesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", textureAtlas.packedTexture);

        gameManager.cameraBoundsFuncUp = (Vector3 point) =>
        {
            Bounds bounds = new Bounds(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(totalSize.x, totalSize.y));
            if (bounds.max.y > point.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        };
        gameManager.cameraBoundsFuncLeft = (Vector3 point) =>
        {
            Bounds bounds = new Bounds(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(totalSize.x, totalSize.y));
            if (bounds.min.x < point.x)
            {
                return true;
            }
            else
            {
                return false;
            }
        };
        gameManager.cameraBoundsFuncRight = (Vector3 point) =>
        {
            Bounds bounds = new Bounds(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(totalSize.x, totalSize.y));
            if (bounds.max.x > point.x)
            {
                return true;
            }
            else
            {
                return false;
            }
        };
        gameManager.cameraBoundsFuncDown = (Vector3 point) =>
        {
            Bounds bounds = new Bounds(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(totalSize.x, totalSize.y));
            if (bounds.min.y < point.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        };
        gameManager.maxWidth = () => ((float)Screen.height / Screen.width) * totalSize.x * 0.5f;
    }

    private void AddVillageSprites()
    {
        var villages = parameters.parameters.villages;
        
        villages.Shuffle();
        int counter = 0;
        foreach (Sector sector in generator.sectors)
        {
            Vector3 worldPos = worldPosFromNode(sector.xVillage, sector.yVillage);
            worldPos.z = villageZLevel;
            GameObject village = GameObject.Instantiate(villagePrefab, worldPos, Quaternion.identity, transform);
            Rect transformedSector = sector.sectorRect;
            var LD = worldPosFromNode((int)transformedSector.min.x,(int) transformedSector.min.y);
            var size = new Vector2(transformedSector.size.x * tileSize.x, transformedSector.size.y * tileSize.y);
            transformedSector.Set(LD.x, LD.y, size.x, size.y);

            TransformedSector completeTransformed = new TransformedSector();
            completeTransformed.sector = transformedSector;
            village.GetComponent<VillageScript>().initFresh(completeTransformed, worldAI, villages[counter].name, initialSoldiersPerVillage);

            sectors.Add(completeTransformed);

            ++counter;
            if (counter >= villages.Count - 1)
            {
               counter = 0;
            }

        }
}
    private void AddRoads()
    {
        //Bound entity contains pathfinding weights for roads;
        HashSet<Vector2> connectedVillages = new HashSet<Vector2>();
        var tileMap = generator.tileMap;
        var width = generator.width;
        var height = generator.height;
        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
            {
                if (tileMap[i, j] == WorldTextureAtlas.Tiles.Village)
                {
                    List<Vector2> neighbouringVillages = scanNeighbouringVillages(i, j);
                    foreach (var villageID in neighbouringVillages)
                    {
                        if (connectedVillages.Contains(villageID)) continue;
                        List<Vector2> path = pathGrid.FindPathTiles(new Vector2(i, j), villageID, GetComponent<Entity>());
                        GameObject newRoad = GameObject.Instantiate(linePrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
                        newRoad.GetComponent<CurveLineRenderer>().ClearVertices();
                        connectedVillages.Add(villageID);
                        foreach (var roadID in path)
                        {
                            if (connectedVillages.Contains(roadID)) continue;
                            if (tileMap[(int)roadID.x, (int)roadID.y] == WorldTextureAtlas.Tiles.Village)
                            {
                                break;
                            }
                            else
                            {
                                tileMap[(int)roadID.x, (int)roadID.y] = WorldTextureAtlas.Tiles.Road;
                                Vector2 worldPos = worldPosFromNode((int)roadID.x, (int)roadID.y);
                                newRoad.GetComponent<CurveLineRenderer>().AddVertex(new Vector3(worldPos.x, worldPos.y, roadZ));
                            }

                        }

                    }
                }
            }
        pathGrid.CreateGrid();
        generator.UpdateWaterSprites();
    }

    private List<Vector2> scanNeighbouringVillages(int x, int y)
    {
        var tileMap = generator.tileMap;
        var width = generator.width;
        var height = generator.height;
        List<Vector2> neighbouringVillages = new List<Vector2>();
        for (int i = x - distanceToScanVillages; i < x + distanceToScanVillages; ++i)
        {
            for (int j = y - distanceToScanVillages; j < y + distanceToScanVillages; ++j)
            {
                if (i == x && y == j) continue;
                if (i < 0 || i > width - 1 || j < 0 || j > height - 1) continue;
                if (tileMap[i, j] == WorldTextureAtlas.Tiles.Village)
                {
                    neighbouringVillages.Add(new Vector2(i, j));
                }
            }
        }
        return neighbouringVillages;
    }

    public Vector2 worldPosFromNode(int x, int y)
    {
        float xOs = transform.position.x - generator.width * tileSize.x / 2.0f + x * tileSize.x + tileSize.x / 2.0f;
        float yOs = transform.position.y - generator.height * tileSize.y / 2.0f + y * tileSize.y + tileSize.y / 2.0f;
        return new Vector2(xOs, yOs);
    }
    // Update is called once per frame
    public Mesh constructBaseMesh()
    {
        Mesh mesh = new Mesh();
        List<int> triangles = new List<int>();
        List<Vector3> vertex = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        int width = generator.width;
        int height = generator.height;
        int jcounter = 0;
        int xcounter = 0;
        Vector2 LD = new Vector2(transform.position.x -tileSize.x * (float)width / 2.0f,transform.position.y -tileSize.y * (float)height / 2.0f);
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                Vector3 vertex1 = new Vector3(LD.x + i * tileSize.x,LD.y + j * tileSize.y, baseMeshZLevel);
                Vector3 vertex2 = new Vector3(LD.x + i * tileSize.x + tileSize.x,LD.y + j * tileSize.y, baseMeshZLevel);
                Vector3 vertex3 = new Vector3(LD.x + i * tileSize.x + tileSize.x,LD.y + j * tileSize.y + tileSize.y, baseMeshZLevel);
                Vector3 vertex4 = new Vector3(LD.x + i * tileSize.x,LD.y + j * tileSize.y + tileSize.y, baseMeshZLevel);

                int triStart = 4 * xcounter + 4 * jcounter;

                int firstTri = triStart + 1;
                int secondTri = triStart + 0;
                int thirdTri = triStart + 2;

                int firstTri2 = triStart + 3;
                int secondTri2 = triStart + 2;
                int thirdTri2 = triStart + 0;

                vertex.Add(vertex1);
                vertex.Add(vertex2);
                vertex.Add(vertex3);
                vertex.Add(vertex4);
                triangles.Add(firstTri);
                triangles.Add(secondTri);
                triangles.Add(thirdTri);

                triangles.Add(firstTri2);
                triangles.Add(secondTri2);
                triangles.Add(thirdTri2);

                WorldTextureAtlas.Tiles tile = generator.tileMap[i,j];

                List<Vector2> uvLocal = getUvs(tile);
                foreach (Vector2 vec in uvLocal)
                {
                    uv.Add(vec);
                }
                if (j != height - 1)
                 ++jcounter;
            }
            ++xcounter;
        }

        Vector3[] normals = new Vector3[vertex.Count];
        mesh.vertices = vertex.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals;
        mesh.uv = uv.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    private List<Vector2> getUvs(WorldTextureAtlas.Tiles tile)
    {
        if (tile == WorldTextureAtlas.Tiles.Mountain)
        {
            int y = 1 + 1;
        }
        Rect rect = textureAtlas.tileMapRects[tile];

        List<Vector2> uv = new List<Vector2>();

        Vector2 uv1 = new Vector2(rect.min.x + 0.0015f, rect.min.y + 0.0015f);
        Vector2 uv2 = new Vector2(rect.max.x - 0.0015f, rect.min.y + 0.0015f);
        Vector2 uv3 = new Vector2(rect.max.x - 0.0015f, rect.max.y - 0.0015f);
        Vector2 uv4 = new Vector2(rect.min.x + 0.0015f, rect.max.y - 0.0015f);

        uv.Add(uv1);
        uv.Add(uv2);
        uv.Add(uv3);
        uv.Add(uv4);

        return uv;
    }

    private void OnDrawGizmos()
    {
        if (sectors != null)
        {
            foreach (var sector in sectors)
            {
                Gizmos.DrawWireCube(sector.sector.center, sector.sector.size);
            }
        }
    }
}