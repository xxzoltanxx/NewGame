using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CheckpointAble : MonoBehaviour
{
    private const int markZ = -1;
    private GameManager gameManager;
    [SerializeField] private GameObject playerCheckpointPrefab;
    [SerializeField] private GameObject particlePrefab;

    //Optimization variables
    private Vector3 world = new Vector3();
    // Start is called before the first frame update
    private void Awake()
    {
        playerCheckpointPrefab = Resources.Load<GameObject>("Prefabs/blackDotPrefab");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        particlePrefab = Resources.Load<GameObject>("Prefabs/Smoke");
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
