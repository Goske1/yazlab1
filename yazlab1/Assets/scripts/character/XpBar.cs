using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{
    [Header("Inspector: bağlayın")]
    public Slider slider;             // Slider bileşeni (XPBar GameObject'in Slider component'i)
    public Image fillImage;           // Fill Area -> Fill objesinin Image component'i (opsiyonel)

    void Awake()
    {
        // Eğer inspector'da atanmamışsa, kendi üzerinde ara
        if (slider == null)
            slider = GetComponent<Slider>();

        // Fill image otomatik bulunur (isteğe bağlı)
        if (fillImage == null && transform.childCount > 0)
        {
            Transform fillArea = transform.Find("Fill Area/Fill");
            if (fillArea != null)
                fillImage = fillArea.GetComponent<Image>();
        }
    }

    // playerstats.Start() çağırdığı için SetMaxXP isimleri korunmuştur
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

        // Güvence: değeri sınırlandır
        slider.value = Mathf.Clamp(xp, (float)slider.minValue, (float)slider.maxValue);
        UpdateFillVisual();
        Debug.Log($"XpBar updated: {slider.value}/{slider.maxValue}");
    }

    void UpdateFillVisual()
    {
        if (fillImage != null && slider != null)
        {
            // normalizedValue = 0..1 arası oran
            fillImage.fillAmount = slider.normalizedValue;
        }
    }
}
