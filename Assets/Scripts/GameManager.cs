using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public CameraFollow cameraFollow;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public GameObject playerCheckpoint;
    public bool playerCheckpointUpdated = false;
    public GameObject unitCheckpoint;
    public GameObject player;

    private Vector3 cameraFollowPosition;
    //NEEDS TO BE INITIALIZED
    public Func<Vector3, bool> cameraBoundsFuncUp;
    public Func<Vector3, bool> cameraBoundsFuncLeft;
    public Func<Vector3, bool> cameraBoundsFuncRight;
    public Func<Vector3, bool> cameraBoundsFuncDown;
    public Func<float> maxWidth;

    public float zoom = 8.0f;

    public int timeMultiplier = 1;
    private void Awake()
    {
    }
    private void Start()
    {
        player = GameObject.Find("player");
        GetComponent<WorldPlayerActionHandler>().lazyInit(player.GetComponent<Entity>());

        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.Setup(() => cameraFollowPosition, () => zoom);
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    private void Update()
    {
        HandleManualMovement();
        HandleZoom(Time.deltaTime);
    }
    void HandleManualMovement()
    {
        float moveAmount = 15.0f;
        float edgeSize = 10.0f;
        Vector3 cameraPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
        if (Input.mousePosition.x > Screen.width - edgeSize && cameraBoundsFuncRight(cameraPos))
        {
            cameraFollowPosition.x += moveAmount * Time.deltaTime;
        }
        if (Input.mousePosition.x < edgeSize && cameraBoundsFuncLeft(cameraPos))
        {
            cameraFollowPosition.x -= moveAmount * Time.deltaTime;
        }
        if (Input.mousePosition.y > Screen.height - edgeSize && cameraBoundsFuncUp(cameraPos))
        {
            cameraFollowPosition.y += moveAmount * Time.deltaTime;
        }
        if (Input.mousePosition.y < edgeSize && cameraBoundsFuncDown(cameraPos))
        {
            cameraFollowPosition.y -= moveAmount * Time.deltaTime;
        }
    }

    private void HandleZoom(float dt)
    {
        float zoomChangeAmount = 100.0f;
        if (Input.mouseScrollDelta.y > 0)
        {
            zoom -= zoomChangeAmount * Time.deltaTime;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            zoom += zoomChangeAmount * Time.deltaTime;
        }
      float maxW = maxWidth();
      zoom = Mathf.Clamp(zoom, 7.0f, maxWidth());
    }
}
