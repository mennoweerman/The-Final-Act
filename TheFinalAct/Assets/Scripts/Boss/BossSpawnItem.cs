using UnityEngine;
using UnityEngine.UI;

public class BossSpawnItem : MonoBehaviour
{
    [Header("Boss Settings")]
    public GameObject bossObject;
    public Slider BossHealthSlider;
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

            BossHealthSlider.gameObject.SetActive(true);
            bossObject.gameObject.SetActive(true);
            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

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
            
            // Koppel de health slider aan de boss
            BossHealth bossHealth = bossObject.GetComponent<BossHealth>();
            if (bossHealth != null && BossHealthSlider != null)
            {
                bossHealth.healthSlider = BossHealthSlider;
            }
        }
        
        if (bossPhaseManager != null)
        {
            bossPhaseManager.StartPhase(BossPhase.VanishingTricks);
        }
        
        bossSpawned = true;
        
        // Verberg/vernietig de spawn item
        gameObject.SetActive(false);
    }
}
