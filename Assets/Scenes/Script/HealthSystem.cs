using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthSlider;

    [Header("Effects")]
    public HitEffect hitEffect;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    private void Update()
    {
        // 🔥 Shortcut test: tekan P untuk mati
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("TEST: Player killed manually with P");
            currentHealth = 0;
            UpdateUI();

            if (hitEffect != null)
                hitEffect.PlayHitEffect();

            Die();
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateUI();

        if (hitEffect != null)
            hitEffect.PlayHitEffect();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateUI()
    {
        if (healthSlider != null)
            healthSlider.value = (float)currentHealth / maxHealth;
    }

    void Die()
    {
        GameOverUI.instance.ShowGameOver();
    }
}
