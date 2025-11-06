using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI bileşenlerini kullanmak için gerekli!

public class DamageImage : MonoBehaviour
{
    public Image damageImage; // UI Image referansı

    public void StartFlash()
    {
        StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {
        // Ekranı kısa süre kırmızıya boyar
        damageImage.color = new Color(1, 0, 0, 0.3f);
        yield return new WaitForSeconds(0.1f);
        damageImage.color = Color.clear;
    }
}
