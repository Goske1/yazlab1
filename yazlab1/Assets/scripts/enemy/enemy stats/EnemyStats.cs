using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    // "enemyhealth" yerine bu ikisini kullanmak daha nettir:
    public int currentHealth; // Mevcut canı
    public int maxHealth;     // Maksimum alabileceği can

    public int level = 1;
    public int baseXP = 50;
    private playerstats player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<playerstats>();

        int minLevel = Mathf.Max(1, player.level - 1);
        int maxLevel = player.level + 2; 
        level = Random.Range(minLevel, maxLevel);

        // Canı ayarla
        maxHealth = 100 + (level - 1) * 20; // Formülünüz
        currentHealth = maxHealth; // Başlangıçta canı fulle

        baseXP += (level - 1) * 10;
<<<<<<< Updated upstream
        
=======

>>>>>>> Stashed changes
        // Debug.Log'u güncelledik
        Debug.Log($"{gameObject.name} spawned with level {level} and {maxHealth} HP");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Remaining: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
<<<<<<< Updated upstream
       
=======

>>>>>>> Stashed changes
        player.GainXP(baseXP + (level * 10));
        Destroy(gameObject); 
    }
}