using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 20;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Projectile touched: " + other.name);

        // Oyuncuya hasar
        if (other.CompareTag("Player"))
        {
            playerstats player = other.GetComponent<playerstats>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("Player took damage!");
            }

            Destroy(gameObject);
        }

        // Düşmana hasar
        else if (other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Enemy took damage!");
            }

            Destroy(gameObject);
        }

        // Duvarda vs çarptığında da yok olsun
        else
        {
            Destroy(gameObject);
        }
    }
}
