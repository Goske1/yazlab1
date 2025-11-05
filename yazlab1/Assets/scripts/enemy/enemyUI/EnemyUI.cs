using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    // Bu referansı artık Spawner vermeyecek, kendi bulacak
    private EnemyStats enemyStats; 
    
    public Image healthFill;
    public TextMeshProUGUI levelText;
    // public Vector3 offset = new Vector3(0, 2.5f, 0); // Buna gerek kalmadı
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        
        // Yeni eklenen satır:
        // Anlamı: "Ben (UI), kimin çocuğuyum? (Enemy). Onun üzerindeki EnemyStats script'ini bul."
        enemyStats = GetComponentInParent<EnemyStats>(); 
    }

    void Update()
    {
        if (enemyStats == null) return;

        // Level yazısı (Bu satır aynı)
        levelText.text = $"Lv. {enemyStats.level}";

        // Health bar güncelle (Bunu yeni değişkenlere göre düzelttik)
        // Canı (0-1 arasında) oranla
        healthFill.fillAmount = (float)enemyStats.currentHealth / (float)enemyStats.maxHealth;

        // Takip etme kodlarını SİLİYORUZ. 
        // Çünkü şapka zaten kafanın üstünde, takip etmesine gerek yok.
        // transform.position = enemyStats.transform.position + offset;
    }

    // Pozisyonu sildik ama rotasyona (dönme) ihtiyacımız var.
    // Bu kod, UI'ın her zaman kameraya bakmasını sağlar (yassı görünmez)
    void LateUpdate()
    {
         if (cam != null)
         {
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                             cam.transform.rotation * Vector3.up);
         }
    }
}