using UnityEngine;

public class playerstats : MonoBehaviour
{
    [Header("Level Up FX")]
    public AudioSource levelUpSound;
    public LevelUpUI levelUpUI;
    public GameObject levelUpVFX; // Buraya level atlama efektinizi (prefab) sürükleyeceksiniz
    private Animator anim;


    [Header("Player Health stats")]
    [SerializeField] public int health = 100;
    [SerializeField] public int currentHealth;
    public HealthBar healthBar;
    public CameraShake camShake;

    [Header("XP & Level Stats")]
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public XpBar xpBar; // yeni ekleyeceğimiz bar

    void Start()
    {
        currentHealth = health;
        healthBar.SetMaxHealth(health);
        anim = GetComponentInChildren<Animator>();

        // XP bar başlangıcı
        if (xpBar != null)
        {
            xpBar.SetMaxXP(xpToNextLevel);
            xpBar.SetXP(currentXP);
        }
        if (xpBar != null)
        xpBar.SetLevel(level);
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
        Debug.LogWarning("⚠️ xpBar referansı BOŞ!");

        // Seviye atlama kontrolü
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

            Debug.Log($"🎉 Level Up! Player is now level {level}!");

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
            if (xpBar != null)
                xpBar.SetLevel(level);

            // --- YENİ EKLENEN KISIM (GÖRSEL EFEKT) ---
            
            // 1. Parçacık Efektini (VFX) Başlat
            if (levelUpVFX != null)
            {
                // Efekti, karakterin bulunduğu pozisyonda oluştur
                // transform.position + new Vector3(0, 1, 0) diyerek biraz üstünde de başlatabilirsiniz
                Instantiate(levelUpVFX, transform.position, Quaternion.identity); 
            }

            // 2. Karakter Animasyonunu Tetikle (Yöntem 2)
            if (anim != null)
            {
                anim.SetTrigger("LevelUpTrigger"); // Animator'deki trigger'ın adıyla aynı olmalı
            }
        }
    void Die()
    {
        Debug.Log("Player Died!");
    }
}
