using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public float regenPerSecond = 1f; // saniyede 1 can yenileme
    private float regenTimer;

    [Header("UI References")]
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    private bool isDead = false;

    void Start()
    {
        SetMaxHealth(maxHealth);
    }

    void Update()
    {

        if (!isDead && currentHealth < maxHealth)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= 1f) // her 1 saniyede bir
            {
                Heal(Mathf.RoundToInt(regenPerSecond));
                regenTimer = 0f;
            }
        }
    }
    public void SetHealth(int health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }


    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        currentHealth = health;

        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(1f);
    }


    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    // Level up olduğunda
    public void OnLevelUp(int newMaxHealth, float regenBonus)
    {
        SetMaxHealth(newMaxHealth);
        regenPerSecond += regenBonus; 
        Debug.Log($"Level up! Yeni max health: {newMaxHealth}, regen: {regenPerSecond}/s");
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Oyuncu öldü!");

    }
}
