using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XpBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider slider;                 // XP bar slider
    public Image fillImage;               // Fill kısmı (opsiyonel)
    public TextMeshProUGUI levelText;     // “Lv. 1” yazısı

    void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        if (fillImage == null)
        {
            Transform fillArea = transform.Find("Fill Area/Fill");
            if (fillArea != null)
                fillImage = fillArea.GetComponent<Image>();
        }
    }

    public void SetMaxXP(int maxXp)
    {
        if (slider == null) return;
        slider.maxValue = maxXp;
        slider.value = 0;
        UpdateFillVisual();
    }

    public void SetXP(int xp)
    {
        if (slider == null) return;
        slider.value = Mathf.Clamp(xp, slider.minValue, slider.maxValue);
        UpdateFillVisual();
    }

    public void SetLevel(int level)
    {
        if (levelText != null)
            levelText.text = $"Lv. {level}";
    }

    void UpdateFillVisual()
    {
        if (fillImage != null && slider != null)
            fillImage.fillAmount = slider.normalizedValue;
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
