using UnityEngine;

public class playerstats : MonoBehaviour
{
    [Header("Level Up FX")]
    public AudioSource levelUpSound;
    public LevelUpUI levelUpUI;


    [Header("Player Health stats")]
    [SerializeField] public int health = 100;
    [SerializeField] public int currentHealth;
    public HealthBar healthBar;
    public CameraShake camShake;

    [Header("XP & Level Stats")]
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public XpBar xpBar; 

    void Start()
    {
        currentHealth = health;
        healthBar.SetMaxHealth(health);


        if (xpBar != null)
        {
            xpBar.SetMaxXP(xpToNextLevel);
            xpBar.SetXP(currentXP);
        }
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        if (camShake != null)
            StartCoroutine(camShake.Shake(0.15f, 0.2f));
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"Player gained {amount} XP! (Total: {currentXP}/{xpToNextLevel})");

        if (xpBar != null)
            xpBar.SetXP(currentXP);
        else
        Debug.LogWarning("xpBar referansı BOŞ!");


        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.25f); 

        Debug.Log($"Level Up! Player is now level {level}!");

        // XP barı sıfırla
        if (xpBar != null)
        {
            xpBar.SetMaxXP(xpToNextLevel);
            xpBar.SetXP(currentXP);
        }
        if (levelUpSound != null)
        levelUpSound.Play();
        
        if (levelUpUI != null)
        levelUpUI.ShowLevelUp();


    }

    void Die()
    {
        Debug.Log("Player Died!");
    }
}
