using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;
    public int playerXP = 100;
    public float xpGrowthRate = 1.3f;

    public int baseHealth = 100;
    public float healthGrowth = 1.1f; // her levelde %10 artış
    public float regenGrowth = 0.5f;  // her levelde saniyede +0.5 can

    private HealthBar healthBar;

    void Start()
    {
        healthBar = FindObjectOfType<HealthBar>();
        healthBar.SetMaxHealth(baseHealth);
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= playerXP)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentXP -=  playerXP;
        level++;
        playerXP = Mathf.RoundToInt(playerXP * xpGrowthRate);

        int newMaxHealth = Mathf.RoundToInt(baseHealth * Mathf.Pow(healthGrowth, level - 1));
        float regenBonus = regenGrowth;

        // HealthBar’a haber ver
        healthBar.OnLevelUp(newMaxHealth, regenBonus);

        Debug.Log($"🎉 Level {level}! Yeni max health: {newMaxHealth}");
    }
}
