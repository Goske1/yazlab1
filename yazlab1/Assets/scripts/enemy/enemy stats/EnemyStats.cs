using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int enemyhealth = 100;
    public int level = 1;
    public int baseXP = 50;
    private playerstats player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<playerstats>();

        // --- Güvenli seviye aralığı: player level ±1 ---
        int minLevel = Mathf.Max(1, player.level - 1);
        int maxLevel = player.level + 2; // Random.Range'de üst limit hariçtir
        int randomLevel = Random.Range(minLevel, maxLevel);

        level = randomLevel;

        // Level'e göre sağlık ve XP ölçeklemesi
        enemyhealth += (level - 1) * 20;
        baseXP += (level - 1) * 10;

        Debug.Log($"{gameObject.name} spawned with level {level} and {enemyhealth} HP");
    }

    public void TakeDamage(int damage)
    {
        enemyhealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Remaining health: " + enemyhealth);

        if (enemyhealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        int xpToGive = baseXP + (level * 10);
        Debug.Log(gameObject.name + " died! Giving XP: " + xpToGive);

        if (player != null)
        {
            player.GainXP(xpToGive);
        }

        Destroy(gameObject);
    }
}
