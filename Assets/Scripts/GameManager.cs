using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public Sprite playerFlag;
    public Sprite enemyFlag;

    public NotificationHandler notificationHandler = null;
    public BattleState currentBattleState = null;
    public bool cameraLocked = false;
    public float cameraZoomMin = 7.0f;
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

    public bool canCheckpoint = true;
    public float zoom = 8.0f;

    public int timeMultiplier = 1;

    Vector3 cameraPos = new Vector3();
    private void Awake()
    {
        notificationHandler = GameObject.Find("NotificationDock").GetComponent<NotificationHandler>();
    }

    public void postBattleActions()
    {
        cameraLocked = false;
        timeMultiplier = 1;
        canCheckpoint = true;
        Vector3 pos = currentBattleState.winningEntity.gameObject.transform.position;
        if (currentBattleState.winningEntity.isPlayer)
        {
            notificationHandler.AddBattleWonNotification(playerFlag);
        }
        else
        {
            notificationHandler.AddBattleLostNotification(enemyFlag);
        }
    }
    private void Start()
    {
        player = GameObject.Find("player");
        GetComponent<WorldPlayerActionHandler>().lazyInit(player.GetComponent<Entity>());

        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.Setup(() => cameraFollowPosition, () => zoom);
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    public void zoomToPosition(Vector2 position)
    {
        cameraFollowPosition = position;
        zoom = cameraZoomMin;
    }

    public void openMenuLockActions(Vector2 position)
    {
        zoomToPosition(position);
        cameraLocked = true;
        timeMultiplier = 0;
        canCheckpoint = false;
    }
    private void Update()
    {
        if (!cameraLocked)
        {
            HandleManualMovement();
            HandleZoom(Time.deltaTime);
        }
    }
    void HandleManualMovement()
    {
        float moveAmount = 15.0f;
        float edgeSize = 10.0f;
        cameraPos.x = Camera.main.transform.position.x;
        cameraPos.y = Camera.main.transform.position.y;
        cameraPos.z = 0;
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
      zoom = Mathf.Clamp(zoom, cameraZoomMin, maxWidth());
    }
}

public class BattleState
{
    public Entity playerEntity;
    public Entity enemyEntity;
    public Entity winningEntity;
    public GameManager gamemanager;
    public BattleState(Entity playerEntity, Entity enemyEntity, GameManager manager)
    {
        this.playerEntity = playerEntity;
        this.enemyEntity = enemyEntity;
        gamemanager = manager;
    }
    public void ResolveBattle()
    {
        float winChance = (float)playerEntity.soldierAmount / (playerEntity.soldierAmount + enemyEntity.soldierAmount);
        float roll = UnityEngine.Random.Range(0, 1.0f);
        if (roll >= winChance)
        {
            enemyEntity.SetSoldiers(enemyEntity.soldierAmount - 2);
            enemyEntity.gameObject.GetComponent<Animator>().SetTrigger("flee");
            winningEntity = playerEntity;
        }
        else
        {
            playerEntity.SetSoldiers(playerEntity.soldierAmount - 2);
            enemyEntity.gameObject.GetComponent<Patrollable>().SetToAfterWin();
            winningEntity = enemyEntity;
        }
        gamemanager.postBattleActions();
    }
}