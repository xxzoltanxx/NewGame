using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour
{
    public float cameraEndCloudTrans = 5.0f;
    public float cameraStartCloudTrans = 7.0f;
    public Vector2 cloudDirection = new Vector2(0.5f, 0.5f);
    public CloudController cloudController;
    public float cloudSpeed = 2.0f;
    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float orthographicSize = Camera.main.orthographicSize;
        float alphaDiff = (Mathf.Clamp01((orthographicSize - cameraStartCloudTrans)/ cameraEndCloudTrans)) / 2.0f;
        Color col = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(col.r, col.g, col.b, alphaDiff);

        transform.position = new Vector3(transform.position.x + Time.deltaTime * cloudSpeed * cloudDirection.x, transform.position.y + Time.deltaTime * cloudSpeed * cloudDirection.y, transform.position.z);
        var bounds = GetComponent<SpriteRenderer>().bounds;
        bounds.center = new Vector3(bounds.center.x, bounds.center.y, cloudController.cloudMask.GetComponent<BoxCollider2D>().bounds.center.z);
        bool doesIntersect = cloudController.cloudMask.GetComponent<BoxCollider2D>().bounds.Intersects(bounds);
        Vector2 referencePoint = new Vector2(0, 0);
        if (cloudDirection.x > 0)
        {
            referencePoint.x = cloudController.cloudMask.GetComponent<BoxCollider2D>().bounds.max.x;
        }
        else
        {
            referencePoint.x = cloudController.cloudMask.GetComponent<BoxCollider2D>().bounds.min.x;
        }
        if (cloudDirection.y > 0)
        {
            referencePoint.y = cloudController.cloudMask.GetComponent<BoxCollider2D>().bounds.max.y;
        }
        else
        {
            referencePoint.y = cloudController.cloudMask.GetComponent<BoxCollider2D>().bounds.min.y;
        }
        Vector2 direction = transform.position - new Vector3(referencePoint.x, referencePoint.y, transform.position.z);
        if (!doesIntersect && Vector2.Angle(direction, cloudDirection) < 90)
        {
            transform.position = new Vector3(transform.position.x - cloudDirection.x * cloudController.cloudMask.GetComponent<BoxCollider2D>().bounds.size.x * 2.5f, transform.position.y - cloudDirection.y * cloudController.cloudMask.GetComponent<BoxCollider2D>().bounds.size.y * 2.5f, transform.position.z);
        }
    }
}
