using UnityEngine;

public class TPS_CameraController : MonoBehaviour
{
    [Header("Ana Ayarlar")]
    public Transform target; // Karakterimizin Transform'u
    public Camera mainCamera; // Asıl kamera objenizi buraya sürükleyin

    [Header("Hassasiyet Ayarları")]
    public float normalSensitivity = 3.0f; // Normal fare hassasiyeti
    public float aimSensitivity = 1.5f;    // Nişan alırkenki fare hassasiyeti

    [Header("Açı Limitleri")]
    public float minYAngle = -89.9f;
    public float maxYAngle = 89.9f;

    [Header("Zoom (Nişan Alma) Ayarları")]
    public float normalFOV = 60f;     // Kameranın normal görüş alanı
    public float zoomedFOV = 30f;     // Nişan alırkenki görüş alanı
    public float zoomSpeed = 10f;     // Zoom geçiş hızı

    [Header("Karakter Dönüş")]
    public float characterRotationSpeed = 10f; // Nişan alırken karakterin kameraya dönüş hızı

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    void Start()
    {
        // Fare imlecini oyun ekranına kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Eğer mainCamera atanmışsa, normalFOV değerini kameranın mevcut FOV'u yap
        if (mainCamera != null)
        {
            normalFOV = mainCamera.fieldOfView;
        }
        else
        {
            Debug.LogError("Lütfen 'Main Camera' referansını Inspector'dan atayın!");
        }
    }

    void Update()
    {
        // Nişan alınıp alınmadığını kontrol et
        bool isAiming = Input.GetMouseButton(1); // 1 = Sağ Mouse Tuşu

        // Duruma göre hassasiyeti belirle
        float currentSensitivity = isAiming ? aimSensitivity : normalSensitivity;

        // Fare girdilerini al
        currentX += Input.GetAxis("Mouse X") * currentSensitivity;
        currentY -= Input.GetAxis("Mouse Y") * currentSensitivity;

        // Y eksenindeki açıyı kısıtla
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
    }

    void LateUpdate()
    {
        if (target == null || mainCamera == null) return;

        // --- 1. Kamera Pozisyonu ve Rotasyonu (Sizin kodunuz) ---
        // Bu nesnenin (tps_camera_pivot) pozisyonunu, karakterin pozisyonuna eşitle
        transform.position = target.position + Vector3.up * 1.5f;

        // Bu nesnenin (tps_camera_pivot) rotasyonunu, fare girdisine göre ayarla
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.rotation = rotation;

        // --- 2. Zoom (FOV) Ayarı (YENİ EKLENDİ) ---
        bool isAiming = Input.GetMouseButton(1); // Sağ Mouse Tuşu
        
        if (isAiming)
        {
            // Sağ tıka basılı tutuluyorsa: Yumuşakça ZoomedFOV'a geç
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomedFOV, Time.deltaTime * zoomSpeed);
<<<<<<< Updated upstream
        }
        else
        {
            // Basılı tutulmuyorsa: Yumuşakça NormalFOV'a geri dön
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);
        }

        // --- 3. Karakter Rotasyonu (YENİ EKLENDİ) ---
        // Eğer nişan alıyorsak, karakterimiz de kameranın baktığı yöne (sadece Y ekseninde) dönsün
        if (isAiming)
        {
            // Kameranın Y açısını al
            Quaternion targetRotation = Quaternion.Euler(0, currentX, 0);

            // Karakterin rotasyonunu yumuşak bir geçişle (Slerp) kameranın Y rotasyonuna eşitle
            target.rotation = Quaternion.Slerp(target.rotation, targetRotation, Time.deltaTime * characterRotationSpeed);
=======
>>>>>>> Stashed changes
        }
        else
        {
            // Basılı tutulmuyorsa: Yumuşakça NormalFOV'a geri dön
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);
        }

        // --- 3. Karakter Rotasyonu (YENİ EKLENDİ) ---
        // Eğer nişan alıyorsak, karakterimiz de kameranın baktığı yöne (sadece Y ekseninde) dönsün
        if (isAiming)
        {
            // Kameranın Y açısını al
            Quaternion targetRotation = Quaternion.Euler(0, currentX, 0);

            // Karakterin rotasyonunu yumuşak bir geçişle (Slerp) kameranın Y rotasyonuna eşitle
            target.rotation = Quaternion.Slerp(target.rotation, targetRotation, Time.deltaTime * characterRotationSpeed);
        }
    }
}