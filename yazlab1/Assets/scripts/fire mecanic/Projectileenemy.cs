using UnityEngine;

// Bu script, SADECE düşmanların ateşlediği mermilerde kullanılacak
public class ProjectileEnemy : MonoBehaviour
{
    // Düşman mermisinin hasarını buradan ayarlayabilirsiniz
    public int damage = 10; 
    public float lifetime = 5f;

    void Start()
    {
        // Mermi 5 saniye sonra kendini yok etsin
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // SADECE "Player" etiketli bir objeye çarparsa hasar ver
        if (collision.gameObject.CompareTag("Player"))
        {
            // Oyuncunun playerstats script'ini bul
            playerstats player = collision.gameObject.GetComponent<playerstats>();
            if (player != null)
            {
                // Oyuncuya hasar ver
                player.TakeDamage(damage);
                Debug.Log("Player took damage from ENEMY!");
            }
        }

        // DİKKAT:
        // Düşmana hasar verme (else if... "Enemy") bloğu buradan KASTEN kaldırıldı.
        // Bu sayede mermi başka bir düşmana çarpsa bile ona hasar VERMEZ.

        // Mermi neye çarparsa çarpsın (Oyuncu, başka bir Düşman, Duvar fark etmez)
        // kendini yok etsin ki ortalıkta dolaşmasın.
        Destroy(gameObject);
    }
}