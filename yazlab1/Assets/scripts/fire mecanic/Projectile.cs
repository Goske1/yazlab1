using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 20;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // --- ESKİ OnTRiggerEnter YERİNE BU FONKSİYONU KULLAN ---
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Projectile collided with: " + collision.gameObject.name);

        // Oyuncuya hasar
        if (collision.gameObject.CompareTag("Player"))
        {
            playerstats player = collision.gameObject.GetComponent<playerstats>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("Player took damage!");
            }
            Destroy(gameObject);
        }

        // Düşmana hasar
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemy = collision.gameObject.GetComponent<EnemyStats>();
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