using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 200;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Projectile collided with: " + collision.gameObject.name +
                  " | Tag: " + collision.gameObject.tag +
                  " | IsTrigger: " + collision.collider.isTrigger);

        
        if (collision.gameObject.CompareTag("Player"))
        {
            playerstats player = collision.gameObject.GetComponent<playerstats>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("Player took damage!");
            }
        }

        
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemy = collision.gameObject.GetComponentInParent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Enemy took damage!");
            }
        }

        
        Destroy(gameObject);
    }
}
