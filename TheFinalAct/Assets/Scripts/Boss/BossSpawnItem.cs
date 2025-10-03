using UnityEngine;

public class BossSpawnItem : MonoBehaviour
{
    [Header("Boss Settings")]
    public GameObject bossObject;
    private BossPhaseManager bossPhaseManager;
    public Transform bossSpawnPosition;
    
    [Header("Interaction")]
    public BoxCollider interactionArea;
    public KeyCode interactionKey = KeyCode.E;
    
    private bool playerInRange = false;
    private bool bossSpawned = false;

    void Start()
    {
        if (interactionArea == null)
            interactionArea = GetComponent<BoxCollider>();
        
        interactionArea.isTrigger = true;
        
        if (bossObject != null)
        {
            bossObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && !bossSpawned && Input.GetKeyDown(interactionKey))
        {
            SpawnBoss();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Press E to start boss fight!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void SpawnBoss()
    {
        if (bossObject != null)
        {
            bossObject.SetActive(true);
            bossPhaseManager = bossObject.GetComponent<BossPhaseManager>();
            
            if (bossSpawnPosition != null)
            {
                bossObject.transform.position = bossSpawnPosition.position;
                bossObject.transform.rotation = bossSpawnPosition.rotation;
            }
            
            bossObject.transform.localScale = new Vector3(5f, 5f, 5f);
        }
        
        if (bossPhaseManager != null)
        {
            bossPhaseManager.StartPhase(BossPhaseManager.BossPhase.VanishingTricks);
        }
        
        bossSpawned = true;
        Debug.Log("Boss fight started! First phase: VanishingTricks");
    }
}
