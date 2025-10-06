using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletAttack : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 20f;
    public float lifeTime = 5f;
    private float damage = 20f;
    public float playerDamage = 10f;
    public float bossDamage = 100f;
    public float knockbackForce = 5f;

    [Tooltip("True = vernietig jezelf na eerste hit")]
    public bool destroyOnHit = true;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDamage(collision.collider);
    }

    private void TryDamage(Collider target)
    {
        var ballHitDetector = target.GetComponent<BallHitDetector>();
        if (ballHitDetector != null)
        {
            return;
        }
        
        // Check of target een health script heeft
        var playerHealth = target.GetComponent<PlayerHealth>();
        var bossHealth = target.GetComponent<BossHealth>();

        if (playerHealth != null)
        {
            // Bereken knockback richting
            damage = playerDamage;
            Vector3 knockbackDir = (target.transform.position - transform.position).normalized;
            playerHealth.TakeDamage(damage, knockbackDir, knockbackForce);
        }
        else if (bossHealth != null)
        {
            // Boss bullets mogen boss niet raken
            if (gameObject.CompareTag("BossBullet"))
            {
                return; // Stop hier, raak boss niet
            }
            
            // Alleen player bullets raken de boss
            damage = bossDamage;
            bossHealth.TakeDamage(damage);
        }

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
