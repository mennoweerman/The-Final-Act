using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 1000f;
    public float currentHealth;

    [Header("Phase Thresholds (in %)")]
    public float phase2Threshold = 0.7f; // bij 70% HP naar CardChaos
    public float phase3Threshold = 0.4f; // bij 40% HP naar FollowTheBall

    [Header("UI Elements")]
    public Slider healthSlider;
    public Image healthBarFill;

    public UnityEvent onBossDeath;

    private BossPhaseManager phaseManager;
    private bool phase2Started;
    private bool phase3Started;
    private bool isStunned = false;

    void Start()
    {
        currentHealth = maxHealth;
        phaseManager = GetComponent<BossPhaseManager>();
        
        // Als healthSlider niet is ingesteld, probeer het te vinden
        if (healthSlider == null)
        {
            GameObject sliderObject = GameObject.Find("BossHealthSlider");
            if (sliderObject != null)
            {
                healthSlider = sliderObject.GetComponent<Slider>();
            }
        }
        
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        if (BossPhaseManager.Instance != null)
        {
            BossPhase current = BossPhaseManager.Instance.currentPhase;

            if (current == BossPhase.FollowTheBall)
            {
                if (!isStunned) return;
            }
        }
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        CheckPhaseTransitions();
        UpdateHealthUI();

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
            phaseManager.StartPhase(BossPhase.CardChaos);
        }

        if (!phase3Started && hpPercent <= phase3Threshold)
        {
            phase3Started = true;
            phaseManager.StartPhase(BossPhase.FollowTheBall);
        }
    }

    void Die()
    {
        Destroy(gameObject, 2f);
    }

    public void SetStunned(bool stunned, float duration = 0f)
    {
        isStunned = stunned;
        if (stunned && duration > 0f)
        {
            StartCoroutine(StunTimer(duration));
        }
    }
    
    private System.Collections.IEnumerator StunTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.minValue = 0;
            healthSlider.value = currentHealth;
        }
    }
}
