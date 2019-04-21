using UnityEngine;
using UnityEngine.EventSystems;
public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    private const int markZ = -1;
    private GameManager gameManager;
    [SerializeField] private GameObject playerCheckpointPrefab;
    [SerializeField] private GameObject particlePrefab;

    private Vector3 world = new Vector3();
    private void Awake()
    {
        playerCheckpointPrefab = Resources.Load<GameObject>("Prefabs/blackDotPrefab");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        particlePrefab = Resources.Load<GameObject>("Prefabs/Smoke");
    }

    public void OnPointerClick(PointerEventData data)
    {
        world = Input.mousePosition;
        world = Camera.main.ScreenToWorldPoint(world);
        world.z = markZ - 0.01f;
        GameObject particleInstance = GameObject.Instantiate(particlePrefab, world, Quaternion.identity);
        world.z = markZ;

        if (!gameManager.playerCheckpoint)
        {
            GameObject spriteInstance = GameObject.Instantiate(playerCheckpointPrefab, world, Quaternion.identity);
            gameManager.playerCheckpoint = spriteInstance;
        }

        else
        {
            gameManager.playerCheckpoint.transform.position = world;
        }


        gameManager.playerCheckpointUpdated = true;
    }
}