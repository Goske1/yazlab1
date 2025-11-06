using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject projectilePrefab;     // Mermi prefab’ı
    public Transform firePoint;             // Merminin çıkış noktası
    public float shootForce = 20f;          // Merminin kuvveti
    public Camera tpsCamera;                // TPS kameranı buraya bağla
    public float maxShootDistance = 100f;   // Mermi yönü için raycast menzili
    public LayerMask aimLayerMask;          // Inspector'dan ayarladığımız maske

    void Update()
    {
        // Eski Input Manager'da "Fire1" varsayılan olarak Mouse0 ve Left Ctrl'e bağlıdır.
        // Dash LeftControl ile tetiklendiğinden, ateşi sadece mouse sol tıklamasına sabitliyoruz.
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
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
        // --- YAZIM HATASI (Impilse) DÜZELTİLDİ ---
        rb.AddForce(direction * shootForce, ForceMode.Impulse); 

        // --- YAZIM HATASI (...) DÜZELTİLDİ ---
        Debug.Log("Fired toward " + targetPoint);
    }
}