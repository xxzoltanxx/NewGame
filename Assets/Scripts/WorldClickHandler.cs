using UnityEngine;
using UnityEngine.EventSystems;
public class WorldClickHandler : MonoBehaviour, IPointerClickHandler
{
    private const int markZ = -1;
    private GameManager gameManager;
    [SerializeField] private GameObject playerCheckpointPrefab;
    [SerializeField] private GameObject particlePrefab;
    private WorldPlayerActionHandler handler;

    private void Awake()
    {
        playerCheckpointPrefab = Resources.Load<GameObject>("Prefabs/blackDotPrefab");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        particlePrefab = Resources.Load<GameObject>("Prefabs/Smoke");
        handler = gameManager.gameObject.GetComponent<WorldPlayerActionHandler>();
    }

    public void OnPointerClick(PointerEventData data)
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector3 world = Camera.main.ScreenToWorldPoint(mousePosition);
        if (data.button == PointerEventData.InputButton.Left)
        {
            if (gameManager.playerCheckpoint)
            {
                GameObject.Destroy(gameManager.playerCheckpoint);
            }

            world.z = markZ - 0.01f;
            GameObject particleInstance = GameObject.Instantiate(particlePrefab, world, Quaternion.identity);
            world.z = markZ;
            GameObject spriteInstance = GameObject.Instantiate(playerCheckpointPrefab, world, Quaternion.identity);

            gameManager.playerCheckpoint = spriteInstance;
            gameManager.playerCheckpointUpdated = true;
        }
        else if (data.button == PointerEventData.InputButton.Right)
        {
            handler.OpenMenuWorld(Camera.main.ScreenToWorldPoint(mousePosition));
        }
    }
}