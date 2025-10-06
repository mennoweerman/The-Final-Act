using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Invincibility Settings")]
    public float invincibilityTime = 1f;
    private bool isInvincible;
    private bool isDead;

    [Header("UI Elements")]
    public Slider healthSlider;
    public Image healthBarFill;

    [Header("Events")]
    public UnityEvent onPlayerDeath;

    private Rigidbody rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        UpdateHealthUI();
    }

    // 🔥 Standaard damage zonder knockback
    public void TakeDamage(float amount)
    {
        TakeDamage(amount, Vector3.zero, 0f);
    }

    // 💥 Damage mét knockback
    public void TakeDamage(float amount, Vector3 knockbackDir, float knockbackForce)
    {
        if (isDead || isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        UpdateHealthUI();

        if (rb != null && knockbackForce > 0)
        {
            rb.AddForce(knockbackDir.normalized * knockbackForce, ForceMode.Impulse);
        }

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    void Die()
    {
        isDead = true;

        onPlayerDeath?.Invoke();

        GetComponent<PlayerController>().enabled = false;
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.minValue = 0;
            healthSlider.value = currentHealth;
        }

        if (healthBarFill != null)
            {
                float healthPercentage = currentHealth / maxHealth;

                if (healthPercentage > 0.6f)
                    healthBarFill.color = Color.green;
                else if (healthPercentage > 0.3f)
                    healthBarFill.color = Color.yellow;
                else
                    healthBarFill.color = Color.red;
            }
    }
}
