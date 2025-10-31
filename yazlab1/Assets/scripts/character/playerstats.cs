using UnityEngine;


public class playerstats : MonoBehaviour
{
    [Header("Player Health stats")]
    [SerializeField] public int health = 100;
    [SerializeField]public int currentHealth;
    public HealthBar healthBar;
    public CameraShake camShake;

    
    void Start()
    {
        currentHealth = health;
        healthBar.SetMaxHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
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
        // Player vurulunca çağır:
        if (camShake != null)
        StartCoroutine(camShake.Shake(0.15f, 0.2f));

    }
    void Die()
    {
        Debug.Log("Player Died!");
    }
    private void OnTriggerEnter(Collider other)
    {
        // Merminin tag'ını "Projectile" olarak ayarladığını varsayıyoruz
        if (other.CompareTag("Projectile"))
        {
            // Mermi scriptinden damage değerini al
            Projectile projectile = other.GetComponent<Projectile>();

            if (projectile != null)
            {
                TakeDamage(projectile.damage);
            }

            // Mermiyi yok et (isteğe bağlı)
            Destroy(other.gameObject);
        }
    }
    
}
