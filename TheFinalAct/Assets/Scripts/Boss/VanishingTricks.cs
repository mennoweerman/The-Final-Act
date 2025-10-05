using UnityEngine;

public class VanishingTricks : MonoBehaviour
{
    public bool isActive;
    public GameObject bossModel;
    public GameObject[] illusions;
    public float teleportInterval = 3f;
    public float teleportMinX = -8f;
    public float teleportMaxX = 8f;
    
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform shootingPoint;
    public float shootInterval = 1f;
    
    private float teleportTimer;
    private float shootTimer;

    public void StartPhase()
    {
        isActive = true;
        teleportTimer = 0f;
        shootTimer = 0f;
        SpawnIllusions();
    }

    void Update()
    {
        if (!isActive) return;

        // Teleport timer
        teleportTimer += Time.deltaTime;
        if (teleportTimer >= teleportInterval)
        {
            TeleportBoss();
            teleportTimer = 0f;
        }
        
        // Shooting timer
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            ShootAtPlayer();
            shootTimer = 0f;
        }
    }

    void SpawnIllusions()
    {
        if (illusions == null || illusions.Length == 0) 
        {
            Debug.LogWarning("Geen illusions ingesteld in VanishingTricks!");
            return;
        }
        
        System.Collections.Generic.List<Vector3> usedPositions = new System.Collections.Generic.List<Vector3>();
        usedPositions.Add(bossModel.transform.position);
        
        Debug.Log($"Spawning {illusions.Length} illusions...");
        
        for (int i = 0; i < illusions.Length; i++)
        {
            GameObject illusion = illusions[i];
            if (illusion != null)
            {
                // Zet Y-waarde naar 5 voor alle illusions
                float yPosition = 5f;
                Vector3 illusionPos = GetAvailablePositionForIllusion(usedPositions, yPosition);
                illusion.transform.position = illusionPos;
                illusion.SetActive(true);
                usedPositions.Add(illusionPos);
                
                Debug.Log($"Illusion {i} spawned at {illusionPos}");
            }
            else
            {
                Debug.LogWarning($"Illusion {i} is null!");
            }
        }
        
        Debug.Log($"Total active illusions: {System.Array.FindAll(illusions, illusion => illusion != null && illusion.activeInHierarchy).Length}");
    }

    void TeleportBoss()
    {
        TeleportAllObjects();
        LookAtPlayer();
    }
    
    void TeleportAllObjects()
    {
        System.Collections.Generic.List<Vector3> usedPositions = new System.Collections.Generic.List<Vector3>();
        
        // Teleport boss eerst
        Vector3 bossPos = GetAvailablePosition(usedPositions);
        bossModel.transform.position = bossPos;
        usedPositions.Add(bossPos);
        
        // Teleport alle illusions naar nieuwe posities met Y=5
        foreach (var illusion in illusions)
        {
            if (illusion != null && illusion.activeInHierarchy)
            {
                Vector3 illusionPos = GetAvailablePositionForIllusion(usedPositions, 5f);
                illusion.transform.position = illusionPos;
                usedPositions.Add(illusionPos);
            }
        }
    }
    
    Vector3 GetAvailablePosition(System.Collections.Generic.List<Vector3> usedPositions)
    {
        Vector3 newPos;
        int attempts = 0;
        
        do 
        {
            newPos = new Vector3(Random.Range(teleportMinX, teleportMaxX), bossModel.transform.position.y, 0);
            attempts++;
        } 
        while (IsPositionTooClose(newPos, usedPositions) && attempts < 50);
        
        if (attempts >= 50)
        {
            Debug.LogWarning($"Could not find good position for boss after 50 attempts. Using position: {newPos}");
        }
        
        return newPos;
    }
    
    Vector3 GetAvailablePositionForIllusion(System.Collections.Generic.List<Vector3> usedPositions, float yPosition)
    {
        Vector3 newPos;
        int attempts = 0;
        
        do 
        {
            newPos = new Vector3(Random.Range(teleportMinX, teleportMaxX), yPosition, 0);
            attempts++;
        } 
        while (IsPositionTooClose(newPos, usedPositions) && attempts < 50);
        
        if (attempts >= 50)
        {
            Debug.LogWarning($"Could not find good position for illusion after 50 attempts. Using position: {newPos}");
        }
        
        return newPos;
    }
    
    bool IsPositionTooClose(Vector3 newPos, System.Collections.Generic.List<Vector3> usedPositions)
    {
        float minimumDistance = 5f;
        
        foreach (Vector3 usedPos in usedPositions)
        {
            if (Vector3.Distance(newPos, usedPos) < minimumDistance)
            {
                return true;
            }
        }
        
        return false;
    }
    
    void LookAtPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 direction = (player.transform.position - bossModel.transform.position).normalized;
            direction.y = 0; // Keep boss upright, only rotate on Y-axis
            
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                bossModel.transform.rotation = lookRotation;
            }
        }
    }
    
    void ShootAtPlayer()
    {
        if (bulletPrefab == null || shootingPoint == null)
        {
            Debug.LogWarning("❌ BulletPrefab of ShootingPoint niet ingesteld voor VanishingTricks!");
            return;
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        
        // Bereken richting naar speler
        Vector3 direction = (player.transform.position - shootingPoint.position).normalized;
        
        // Spawn bullet richting speler
        Vector3 spawnPos = shootingPoint.position + (direction * 2f);
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(direction));
        
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = direction * 10f;
        }
        
        // Zorg dat bullet boss niet raakt
        BulletAttack bulletScript = bullet.GetComponent<BulletAttack>();
        if (bulletScript != null)
        {
            bulletScript.lifeTime = 8f;
            bulletScript.destroyOnHit = true;
            // Tag de bullet als boss bullet
            bullet.tag = "BossBullet";
        }
    }

    public void EndPhase()
    {
        isActive = false;
        foreach (var illusion in illusions)
            illusion.SetActive(false);
    }
}
