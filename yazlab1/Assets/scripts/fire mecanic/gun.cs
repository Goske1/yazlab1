using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject projectilePrefab;   // Mermi prefab’ı
    public Transform firePoint;           // Merminin çıkış noktası
    public float shootForce = 20f;        // Merminin kuvveti
    public Camera tpsCamera;              // TPS kameranı buraya bağla
    public float maxShootDistance = 100f; // Mermi yönü için raycast menzili

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // --- Kamera merkezinden bir Ray çıkar ---
        Ray ray = tpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Ekran ortası
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, maxShootDistance))
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
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(direction * shootForce, ForceMode.Impulse);

        Debug.Log("Fired toward " + targetPoint);
    }
}
