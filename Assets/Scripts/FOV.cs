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
    public float speed = 0.5f;
    public Material worldEffectMaterial;
    // Start is called before the first frame update

    //Optimization
    Vector3[] oldPerimeterPoints = new Vector3[92];
    Vector3[] pointsFOWBOUNDS = new Vector3[92];
    Vector3[] holder = new Vector3[92];
    Vector3[] norms = new Vector3[92];
    private void Awake()
    {
        boundEntity = transform.parent.GetComponent<Entity>();
        gameWorld = GameObject.Find("GameWorld").GetComponent<GameWorld>();
        worldEffectMaterial = GameObject.Find("Quad").GetComponent<MeshRenderer>().material;
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
        Vector3[] fowPointsPerimeter = getFOWBoundsPolygon(gameWorld.isNight);
        fowPointsPerimeter.CopyTo(oldPerimeterPoints, 0);
        proceduralMesh.vertices = fowPointsPerimeter;
        proceduralMesh.triangles = triangles;
        proceduralMesh.uv = uvs;
        for (int i = 0; i < fowPointsPerimeter.Length; ++i)
        {
            norms[i] = fowPointsPerimeter[i].normalized;
        }
        proceduralMesh.normals = norms;
        GetComponent<MeshFilter>().mesh = proceduralMesh;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3[] fowPointsPerimeter = getFOWBoundsPolygon(gameWorld.isNight);
        for (int i = 0; i < fowPointsPerimeter.Length; ++i)
        {
            holder[i] = Vector3.Lerp(oldPerimeterPoints[i], fowPointsPerimeter[i], Time.deltaTime);
        }
        float distance = holder[1].magnitude;
        GetComponent<CircleCollider2D>().radius = distance * 2;
        holder.CopyTo(oldPerimeterPoints, 0);

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        float timex = Time.time * speed + 0.1365143f;

       for (int i = 1; i < holder.Length; i++)
        {

            float dist = (Mathf.PerlinNoise(timex + holder[i].x, timex + holder[i].y) - 0.5f) * perlinScale;
            holder[i].x += (holder[i].x + norms[i].x * dist);
            holder[i].y += (holder[i].y + norms[i].y * dist);
        }

       mesh.vertices = holder;
       mesh.RecalculateBounds();
        worldEffectMaterial.SetFloat("_Distance", distance * 8.0f);
       worldEffectMaterial.SetVector("_Position", transform.position);
    }

    private Vector3[] getFOWBoundsPolygon(bool isNight)
    {
        Vector2 currentPosition = transform.position;
        pointsFOWBOUNDS[0] = transform.InverseTransformPoint(currentPosition);
        pointsFOWBOUNDS[0].z = 0;
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
        if (isNight)
            distance *= boundEntity.nightFOVModifier;
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
            pointsFOWBOUNDS[i] = endPosition;
            ++i;
        }
        return pointsFOWBOUNDS;
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