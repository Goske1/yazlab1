using UnityEngine;

public class TPS_CameraController : MonoBehaviour
{
    public Transform target; // Karakterimizin Transform'u (Inspector'dan sürükleyeceğiz)
    public float sensitivity = 3.0f; // Fare hassasiyeti

    // Dikey eksende kameranın ne kadar dönebileceğini kısıtlayalım (ters dönmesin)
    public float minYAngle = -89.9f; // Yukarı bakış limiti (mouse Y ters olduğu için)
    public float maxYAngle = 89.9f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    void Start()
    {
        // Fare imlecini oyun ekranına kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Fare girdilerini al
        currentX += Input.GetAxis("Mouse X") * sensitivity;
        currentY -= Input.GetAxis("Mouse Y") * sensitivity; // Y ekseni ters çalışır genelde

        // Y eksenindeki açıyı kısıtla
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
    }

    void LateUpdate()
    {
        // Kamera hareketlerini her zaman LateUpdate içinde yaparız.
        if (target != null)
        {
            // 1. Bu nesnenin (tps_camera) pozisyonunu, karakterin pozisyonuna eşitle
            // (Karakterin ayakları yerine biraz üstünü, örn. göğsünü hedef alabiliriz)
            transform.position = target.position + Vector3.up * 1.10f; 

            // 2. Bu nesnenin (tps_camera) rotasyonunu, fare girdisine göre ayarla
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            transform.rotation = rotation;
        }
    }
}