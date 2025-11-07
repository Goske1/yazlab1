using UnityEngine;

public class EnemyLevelSystem : MonoBehaviour
{
    [Header("Level Stats")]
    public int level = 1;
    public float healthGrowth = 1.1f;  
    public float rewardGrowth = 1.05f; 

    [Header("Base Stats")]
    public int baseHealth = 100;        
    public int baseDamage = 10;     
    public int baseXPReward = 25;      

    [Header("Runtime")]
    public int currentHealth;
    public int currentXPReward;

    void Start()
    {
     
        int scaledHealth = Mathf.RoundToInt(baseHealth * Mathf.Pow(healthGrowth, level - 1));
        currentHealth = scaledHealth;

        currentXPReward = Mathf.RoundToInt(baseXPReward * Mathf.Pow(rewardGrowth, level - 1));

     
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

        PlayerXP playerXP = FindObjectOfType<PlayerXP>();
        if (playerXP != null)
            playerXP.AddXP(currentXPReward); 

        Destroy(gameObject);
    }
}
