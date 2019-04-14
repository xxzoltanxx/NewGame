using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class FOV : MonoBehaviour
{
    public Material material;
    public const float terrainStep = 0.01f;
    private GameWorld gameWorld;
    private Entity boundEntity;
    public float perlinScale = 0.5f;
    Vector3[] oldPerimeterPoints;
    public float speed = 0.5f;
    // Start is called before the first frame update
    private void Awake()
    {
        boundEntity = transform.parent.GetComponent<Entity>();
        gameWorld = GameObject.Find("GameWorld").GetComponent<GameWorld>();

    }
    void Start()
    {
        GetComponent<MeshRenderer>().material = material;
        Mesh proceduralMesh = new Mesh();
        int[] triangles = new int[90 * 3];
        for (int i = 0; i < 89; ++i)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = i + 1;
        }
        triangles[3 * 89] = 0;
        triangles[3 * 89 + 1] = 1;
        triangles[3 * 89 + 2] = 90;
        Vector2[] uvs = new Vector2[92];
        for (int i = 1; i < 92; ++i)
        {
            if (i % 2 == 0)
                uvs[i] = new Vector2(0, 1);
            else
                uvs[i] = new Vector2(1, 1);
        }
        Vector2 startAngleDir = new Vector2(0.01f, 0);
        int j = 1;
        Vector3[] fowPointsPerimeter = getFOWBoundsPolygon();
        oldPerimeterPoints = fowPointsPerimeter;
        proceduralMesh.vertices = fowPointsPerimeter;
        proceduralMesh.triangles = triangles;
        proceduralMesh.uv = uvs;
        Vector3[] norms = new Vector3[fowPointsPerimeter.Length];
        for (int i = 0; i < fowPointsPerimeter.Length; ++i)
        {
            norms[i] = fowPointsPerimeter[i].normalized;
        }
        proceduralMesh.normals = norms;
        GetComponent<MeshFilter>().mesh = proceduralMesh;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] fowPointsPerimeter = getFOWBoundsPolygon();
        Vector3[] holder = new Vector3[fowPointsPerimeter.Length];
        for (int i = 0; i < fowPointsPerimeter.Length; ++i)
        {
            holder[i] = Vector3.Lerp(oldPerimeterPoints[i], fowPointsPerimeter[i], Time.deltaTime);
        }
        float distance = holder[1].magnitude;
        GetComponent<CircleCollider2D>().radius = distance * 2;
        oldPerimeterPoints = new Vector3[holder.Length];
        holder.CopyTo(oldPerimeterPoints, 0);

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        float timex = Time.time * speed + 0.1365143f;

        for (var i = 1; i < holder.Length; i++)
        {
            var vertex = holder[i];

            float dist = (Mathf.PerlinNoise(timex + vertex.x, timex + vertex.y) - 0.5f) * perlinScale;
            vertex.x += (vertex.x + mesh.normals[i].x * dist);
            vertex.y += (vertex.y + mesh.normals[i].y * dist);

            holder[i] = vertex;
        }

        mesh.vertices = holder;
        mesh.RecalculateBounds();

    }

    private Vector3[] getFOWBoundsPolygon()
    {
        Vector3[] points = new Vector3[92];
        Vector2 currentPosition = transform.position;
        points[0] = transform.InverseTransformPoint(currentPosition);
        points[0].z = 0;
        Vector2 startAngleDir = new Vector2(0.01f, 0);
        int i = 1;
        GameWorld.Terrain currentTerrain = gameWorld.getTerrainAtPoint(currentPosition);
        if (currentTerrain == GameWorld.Terrain.Forest)
        {
            boundEntity.hidden = true;
        }
        else
        {
            boundEntity.hidden = false;
        }
        float distance = boundEntity.fovDistance[currentTerrain];
        for (float angle = 0; angle <= 360; angle += 4)
        {
            Vector2 angleDir = startAngleDir.Rotate(angle);
            //GameWorld.Terrain currentTerrain = gameWorld.getTerrainAtPoint(currentPosition + angleDir);
            Vector2 angleDirIncreased = new Vector2(distance * Mathf.Cos(Mathf.Deg2Rad * angle), distance * Mathf.Sin(Mathf.Deg2Rad * angle));
            Vector2 endPosition = currentPosition + angleDirIncreased;
            //for (float dist = 0; dist < distance - 0.01f; dist = dist + terrainStep)
            /*{
                //Vector2 angleDirIncreased = angleDir + new Vector2(dist * Mathf.Cos(Mathf.Deg2Rad * angle), dist * Mathf.Sin( Mathf.Deg2Rad * angle));
                //endPosition = currentPosition + angleDirIncreased;
                GameWorld.Terrain newTerrain = gameWorld.getTerrainAtPoint(endPosition);
                if (newTerrain != currentTerrain)
                {
                    //currentTerrain = newTerrain;
                    //distance = boundEntity.fovDistance[newTerrain];
                    distance = boundEntity.fovDistance[currentTerrain];
                }
            }*/

            endPosition = transform.InverseTransformPoint(endPosition);
            points[i] = endPosition;
            ++i;
        }
        return points;
    }
}

public static class Vector2Extension
{

    public static Vector2 Rotate(this Vector2 vu, float degrees)
    {
        Vector2 v = new Vector2();
        float sin = Mathf.Sin(degrees);
        float cos = Mathf.Cos(degrees);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static bool FuzzyEquals(this Vector2 vu, Vector2 other, float diff)
    {
        if ((Vector2.Distance(vu,other) < diff))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}