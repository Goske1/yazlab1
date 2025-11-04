using UnityEngine;

public class EnemyLevelSystem : MonoBehaviour
{
    [Header("Level Stats")]
    public int level = 1;
    public float healthGrowth = 1.1f;   // Her level'da can %10 artsın
    public float rewardGrowth = 1.05f;  // Her level'da XP ödülü %5 artsın

    [Header("Base Stats")]
    public int baseHealth = 100;        // Seviye 1 can
    public int baseDamage = 10;         // Seviye 1 hasar
    public int baseXPReward = 25;       // Seviye 1 XP ödülü

    [Header("Runtime")]
    public int currentHealth;
    public int currentXPReward;

    void Start()
    {
        // Seviye başına çarpan uygula
        int scaledHealth = Mathf.RoundToInt(baseHealth * Mathf.Pow(healthGrowth, level - 1));
        currentHealth = scaledHealth;

        currentXPReward = Mathf.RoundToInt(baseXPReward * Mathf.Pow(rewardGrowth, level - 1));

        // HealthBar varsa canı ayarla
        HealthBar bar = GetComponent<HealthBar>();
        if (bar != null)
        {
            bar.SetMaxHealth(currentHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        HealthBar bar = GetComponent<HealthBar>();
        if (bar != null)
            bar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // XP kazandır
        PlayerXP playerXP = FindObjectOfType<PlayerXP>();
        if (playerXP != null)
            playerXP.AddXP(currentXPReward); 

        Destroy(gameObject);
    }
}
