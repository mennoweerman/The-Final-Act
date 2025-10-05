using UnityEngine;

public class CardChaos : MonoBehaviour
{
    public bool isActive;
    public GameObject cardPrefab;
    public Transform shootingPoint;
    public float shootInterval = 0.5f;
    
    [Header("Vanishing Settings")]
    public GameObject bossModel;
    public float vanishInterval = 4f;
    public VanishingTricks vanishingTricks;
    
    private float teleportMinX;
    private float teleportMaxX;
    
    private float shootTimer;
    private float vanishTimer;

    void Start()
    {
        teleportMinX = vanishingTricks.teleportMinX;
        teleportMaxX = vanishingTricks.teleportMaxX;
    }

    public void StartPhase()
    {
        isActive = true;
        shootTimer = 0f;
        vanishTimer = 0f;
    }

    void Update()
    {
        if (!isActive) return;

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            ShootCardPattern();
            shootTimer = 0f;
        }
        
        vanishTimer += Time.deltaTime;
        if (vanishTimer >= vanishInterval)
        {
            TeleportBoss();
            vanishTimer = 0f;
        }
    }

    void ShootCardPattern()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 directionToPlayer = (player.transform.position - shootingPoint.position).normalized;
        int patternType = Random.Range(0, 9);
        
        int cardCount = 12;
        for (int i = 0; i < cardCount; i++)
        {
            Vector3 dir = directionToPlayer;
            
            switch (patternType)
            {
                case 0:
                    break;
                    
                case 1:
                    float smallSpread = Random.Range(-5f, 5f);
                    dir = Quaternion.Euler(0, smallSpread, 0) * directionToPlayer;
                    break;
                    
                case 2:
                    float mediumSpread = Random.Range(-15f, 15f);
                    dir = Quaternion.Euler(0, mediumSpread, 0) * directionToPlayer;
                    break;
                    
                case 3:
                    float largeSpread = Random.Range(-30f, 30f);
                    dir = Quaternion.Euler(0, largeSpread, 0) * directionToPlayer;
                    break;
                    
                case 4:
                    float fanAngle = (i - cardCount/2f) * 5f;
                    dir = Quaternion.Euler(0, fanAngle, 0) * directionToPlayer;
                    break;
                    
                case 5:
                    float zigzagAngle = (i % 2 == 0) ? -10f : 10f;
                    dir = Quaternion.Euler(0, zigzagAngle, 0) * directionToPlayer;
                    break;
                    
                case 6:
                    float spiralOffset = i * 15f + (Time.time * 30f);
                    dir = Quaternion.Euler(0, spiralOffset, 0) * directionToPlayer;
                    break;
                    
                case 7:
                    float doubleSpread = (i < cardCount/2) ? -20f : 20f;
                    dir = Quaternion.Euler(0, doubleSpread, 0) * directionToPlayer;
                    break;
                    
                case 8:
                    float circleAngle = i * (360f / cardCount);
                    Vector3 circleDir = Quaternion.Euler(0, circleAngle, 0) * Vector3.forward;
                    dir = Vector3.Lerp(circleDir, directionToPlayer, 0.7f).normalized;
                    break;
            }

            if (dir.magnitude < 0.1f)
            {
                dir = directionToPlayer;
            }
            
            Vector3 spawnPos = shootingPoint.position + (dir.normalized * 3f);
            GameObject card = Instantiate(cardPrefab, spawnPos, Quaternion.LookRotation(dir));
            
            Rigidbody cardRb = card.GetComponent<Rigidbody>();
            if (cardRb != null)
            {
                cardRb.linearVelocity = dir.normalized * 8f;
            }
            
            BulletAttack bulletScript = card.GetComponent<BulletAttack>();
            if (bulletScript != null)
            {
                bulletScript.lifeTime = 10f;
                bulletScript.destroyOnHit = false;
            }
            
            card.tag = "BossBullet";
            
            Collider cardCollider = card.GetComponent<Collider>();
            if (cardCollider != null)
            {
                cardCollider.enabled = false;
                StartCoroutine(EnableColliderAfterDelay(cardCollider, 0.5f));
            }
        }
    }

    System.Collections.IEnumerator EnableColliderAfterDelay(Collider cardCollider, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (cardCollider != null)
        {
            cardCollider.enabled = true;
        }
    }

    System.Collections.IEnumerator IgnoreBossCollision(GameObject card, float delay)
    {
        if (card == null) yield break;
        
        Collider cardCollider = card.GetComponent<Collider>();
        Collider bossCollider = GetComponent<Collider>();
        
        if (cardCollider != null && bossCollider != null)
        {
            Physics.IgnoreCollision(cardCollider, bossCollider, true);
            yield return new WaitForSeconds(delay);
            
            if (card != null && cardCollider != null && bossCollider != null)
            {
                Physics.IgnoreCollision(cardCollider, bossCollider, false);
            }
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }
    }

    void TeleportBoss()
    {
        if (bossModel == null) return;
        
        Vector3 randomPos = new Vector3(Random.Range(teleportMinX, teleportMaxX), bossModel.transform.position.y, 0);
        bossModel.transform.position = randomPos;
        
        LookAtPlayer();
    }
    
    void LookAtPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && bossModel != null)
        {
            Vector3 direction = (player.transform.position - bossModel.transform.position).normalized;
            direction.y = 0;
            
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                bossModel.transform.rotation = lookRotation;
            }
        }
    }

    public void EndPhase()
    {
        isActive = false;
    }
}

