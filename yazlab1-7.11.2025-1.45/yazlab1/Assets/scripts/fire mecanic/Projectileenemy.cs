using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
   
    public int damage = 10; 
    public float lifetime = 5f;

    void Start()
    {
        
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            
            playerstats player = collision.gameObject.GetComponent<playerstats>();
            if (player != null)
            {
                
                player.TakeDamage(damage);
                Debug.Log("Player took damage from ENEMY!");
            }
        }

        
        Destroy(gameObject);
    }
}