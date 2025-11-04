using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int enemyhealth = 100;

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
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject);
    }
}
