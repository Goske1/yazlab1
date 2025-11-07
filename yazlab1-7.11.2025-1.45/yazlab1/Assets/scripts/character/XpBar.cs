using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{
    [Header("Inspector: bağlayın")]
    public Slider slider;        
    public Image fillImage;         

    void Awake()
    {
        
        if (slider == null)
            slider = GetComponent<Slider>();

       
        if (fillImage == null && transform.childCount > 0)
        {
            Transform fillArea = transform.Find("Fill Area/Fill");
            if (fillArea != null)
                fillImage = fillArea.GetComponent<Image>();
        }
    }

  
    public void SetMaxXP(int maxXp)
    {
        if (slider == null)
        {
            Debug.LogWarning("XpBar.SetMaxXP: slider referansı yok!");
            return;
        }

        slider.maxValue = maxXp;
        slider.value = 0;
        UpdateFillVisual();
        Debug.Log($"XpBar: max set to {maxXp}");
    }

    public void SetXP(int xp)
    {
        if (slider == null)
        {
            Debug.LogWarning("XpBar.SetXP: slider referansı yok!");
            return;
        }

   
        slider.value = Mathf.Clamp(xp, (float)slider.minValue, (float)slider.maxValue);
        UpdateFillVisual();
        Debug.Log($"XpBar updated: {slider.value}/{slider.maxValue}");
    }

    void UpdateFillVisual()
    {
        if (fillImage != null && slider != null)
        {
            fillImage.fillAmount = slider.normalizedValue;
        }
    }
}
