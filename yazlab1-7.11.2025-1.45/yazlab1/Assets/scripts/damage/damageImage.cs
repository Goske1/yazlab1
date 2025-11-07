using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

public class DamageImage : MonoBehaviour
{
    public Image damageImage; 

    public void StartFlash()
    {
        StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {

        damageImage.color = new Color(1, 0, 0, 0.3f);
        yield return new WaitForSeconds(0.1f);
        damageImage.color = Color.clear;
    }
}
