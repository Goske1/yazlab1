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
        else if (!other.CompareTag("Enemy"))
        {
            // Düşmana çarpmadıysa, duvara vs çarptığında da yok olsun
            Destroy(gameObject);
        }
    }
}
