using UnityEngine;

[RequireComponent(typeof(BossHealth))]
[RequireComponent(typeof(BossPhaseManager))]
public class BossController : MonoBehaviour
{
    private BossHealth health;
    private BossPhaseManager phaseManager;
    private Transform player;

    [Header("Combat Settings")]
    public float contactDamage = 20f;
    public float attackRange = 5f;

    void Start()
    {
        health = GetComponent<BossHealth>();
        phaseManager = GetComponent<BossPhaseManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 dir = player.position - transform.position;
        if (Vector2.Distance(transform.position, player.position) < attackRange)
        {
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
            other.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(20f, knockbackDir, 8f);
        }
    }
}