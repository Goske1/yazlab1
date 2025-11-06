using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject projectilePrefab;     // Mermi prefab’ı
    public Transform firePoint;             // Merminin çıkış noktası
    public float shootForce = 20f;          // Merminin kuvveti
    public Camera tpsCamera;                // TPS kameranı buraya bağla
    public float maxShootDistance = 100f;   // Mermi yönü için raycast menzili
    public LayerMask aimLayerMask;          // Inspector'dan ayarladığımız maske

    [Header("Otomatik Ateş Ayarları")] // <-- YENİ BAŞLIK
    public float fireRate = 10f; // Saniyede kaç mermi atacağı
    private float nextFireTime = 0f; // Bir sonraki ateş zamanını tutan değişken

    void Update()
    {
        // 'GetMouseButtonDown' (bir kez basma) yerine 'GetMouseButton' (basılı tutma) olarak değiştirildi.
        // --- DEĞİŞTİ ---
        if (Input.GetMouseButton(0))
        {
            // Ateş etmek için "fire rate" süresinin geçip geçmediğini kontrol et
            // Time.time, oyun başladığından beri geçen toplam süredir.
            // --- YENİ KONTROL EKLENDİ ---
            if (Time.time >= nextFireTime)
            {
                // Bir sonraki ateş zamanını ayarla
                // Örnek: fireRate 10 ise, 1 / 10 = 0.1 saniye sonra tekrar ateş edebilir.
                nextFireTime = Time.time + 1f / fireRate;

                // Ateş et
                Shoot();
            }
        }
    }

    void Shoot()
    {
        // --- Kamera merkezinden bir Ray çıkar ---
        Ray ray = tpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Ekran ortası
        Vector3 targetPoint;

        // --- Raycast'i yaparken artık LayerMask'i kullanıyoruz ---
        if (Physics.Raycast(ray, out RaycastHit hit, maxShootDistance, aimLayerMask))
        {
            targetPoint = hit.point; // Çarptığı nokta
        }
        else
        {
            targetPoint = ray.GetPoint(maxShootDistance); // Hiçbir şeye çarpmadıysa ileriye ateş et
        }

        // --- FirePoint'ten hedef noktaya doğru yön belirle ---
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        // --- Mermiyi oluştur ve yönlendir ---
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
        Debug.Log($"Projectile spawned: {projectile.name} at {firePoint.position}");
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // Mermiye kuvvet uygula
        rb.AddForce(direction * shootForce, ForceMode.Impulse); 

        Debug.Log("Fired toward " + targetPoint);
    }
}