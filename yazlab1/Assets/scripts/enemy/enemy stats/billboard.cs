using UnityEngine;

public class billboard : MonoBehaviour
{
    public Transform cam;
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
