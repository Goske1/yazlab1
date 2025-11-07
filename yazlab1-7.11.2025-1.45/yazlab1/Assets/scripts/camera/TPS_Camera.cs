using UnityEngine;

public class TPS_CameraController : MonoBehaviour
{
    [Header("Ana Ayarlar")]
    public Transform target;     
    public Camera mainCamera; 

    [Header("Hassasiyet Ayarları")]
    public float normalSensitivity = 3.0f; 
    public float aimSensitivity = 1.5f;   

    [Header("Açı Limitleri")]
    public float minYAngle = -89.9f;
    public float maxYAngle = 89.9f;

    [Header("Zoom (Nişan Alma) Ayarları")]
    public float normalFOV = 60f;     
    public float zoomedFOV = 30f;     
    public float zoomSpeed = 10f;     

    [Header("Karakter Dönüş")]
    public float characterRotationSpeed = 10f; 

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
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
        
        bool isAiming = Input.GetMouseButton(1); 

        
        float currentSensitivity = isAiming ? aimSensitivity : normalSensitivity;

        
        currentX += Input.GetAxis("Mouse X") * currentSensitivity;
        currentY -= Input.GetAxis("Mouse Y") * currentSensitivity;

        
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
    }

    void LateUpdate()
    {
        if (target == null || mainCamera == null) return;

       
        transform.position = target.position + Vector3.up * 1.5f;

        
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.rotation = rotation;

        
        bool isAiming = Input.GetMouseButton(1); // Sağ Mouse Tuşu
        
        if (isAiming)
        {
            
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomedFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);
        }

        
        if (isAiming)
        {
            
            Quaternion targetRotation = Quaternion.Euler(0, currentX, 0);

            
            target.rotation = Quaternion.Slerp(target.rotation, targetRotation, Time.deltaTime * characterRotationSpeed);
        }
    }
}