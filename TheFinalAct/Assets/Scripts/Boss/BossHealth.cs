using UnityEngine;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 1000f;
    public float currentHealth;

    [Header("Phase Thresholds (in %)")]
    public float phase2Threshold = 0.7f; // bij 70% HP naar CardChaos
    public float phase3Threshold = 0.4f; // bij 40% HP naar FollowTheBall

    public UnityEvent onBossDeath;

    private BossPhaseManager phaseManager;
    private bool phase2Started;
    private bool phase3Started;

    void Start()
    {
        currentHealth = maxHealth;
        phaseManager = GetComponent<BossPhaseManager>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"💥 Boss damage: {amount} (HP: {currentHealth}/{maxHealth})");

        CheckPhaseTransitions();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void CheckPhaseTransitions()
    {
        float hpPercent = currentHealth / maxHealth;

        if (!phase2Started && hpPercent <= phase2Threshold)
        {
            phase2Started = true;
            phaseManager.StartPhase(BossPhaseManager.BossPhase.CardChaos);
        }

        if (!phase3Started && hpPercent <= phase3Threshold)
        {
            phase3Started = true;
            phaseManager.StartPhase(BossPhaseManager.BossPhase.FollowTheBall);
        }
    }

    void Die()
    {
        Debug.Log("☠️ Boss verslagen!");
        onBossDeath?.Invoke();
        // Je kunt hier een animatie starten, loot spawnen, arena unlocken etc.
        Destroy(gameObject, 2f);
    }
}
