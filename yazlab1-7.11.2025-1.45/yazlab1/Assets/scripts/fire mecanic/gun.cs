using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject projectilePrefab;     
    public Transform firePoint;             
    public float shootForce = 20f;        
    public Camera tpsCamera;               
    public float maxShootDistance = 100f;   
    public LayerMask aimLayerMask;          

    [Header("Otomatik Ateş Ayarları")] 
    public float fireRate = 10f; 
    private float nextFireTime = 0f; 

    void Update()
    {

        if (Input.GetMouseButton(0))
        {
   
            if (Time.time >= nextFireTime)
            {
        
                nextFireTime = Time.time + 1f / fireRate;

             
                Shoot();
            }
        }
    }

    void Shoot()
    {
      
        Ray ray = tpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        Vector3 targetPoint;

        
        if (Physics.Raycast(ray, out RaycastHit hit, maxShootDistance, aimLayerMask))
        {
            targetPoint = hit.point; 
        }
        else
        {
            targetPoint = ray.GetPoint(maxShootDistance); 
        }

       
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
        Debug.Log($"Projectile spawned: {projectile.name} at {firePoint.position}");
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        
        rb.AddForce(direction * shootForce, ForceMode.Impulse); 

        Debug.Log("Fired toward " + targetPoint);
    }
}