using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    public float cloudZ = -9;
    public Vector2 cloudDirection = new Vector2(0.5f, 0.5f);
    public int cloudsAmount = 15;
    public GameObject cloudPrefab;
    public GameObject cloudMask;
    public float cloudSpeed = 2.0f;
    public List<Sprite> clouds = new List<Sprite>();
    public List<GameObject> cloudPool = new List<GameObject>();
    // Start is called before the first frame update
    private void Awake()
    {
        cloudPrefab = Resources.Load("Prefabs/cloudPrefab") as GameObject;
        cloudMask = GameObject.Find("cloudMask");
        for (int i = 0; i < cloudsAmount; ++i)
        {
            var bounds = cloudMask.GetComponent<BoxCollider2D>().bounds;
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            var cloud = GameObject.Instantiate(cloudPrefab, new Vector3(randomX, randomY, cloudZ), Quaternion.identity);
            Sprite randomSprite = clouds[Random.Range(0, clouds.Count)];
            cloud.GetComponent<SpriteRenderer>().sprite = randomSprite;
            cloud.GetComponent<CloudScript>().cloudController = this;
            cloud.GetComponent<CloudScript>().cloudDirection = cloudDirection;
            cloud.GetComponent<CloudScript>().cloudSpeed = Random.Range(cloudSpeed - 1.0f, cloudSpeed + 1.0f); ;
            cloud.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = randomSprite;
            cloudPool.Add(cloud);
        }
    }

    public void SetSunDir(Vector3 vec)
    {
        cloudPool[0].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sharedMaterial.SetVector("_SunDir", vec);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void replaceCloud(CloudScript script)
    {

    }
}
