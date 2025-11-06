using UnityEngine;
using TMPro;
using System.Collections;

public class LevelUpUI : MonoBehaviour
{
    public TextMeshProUGUI levelUpText;

    public void ShowLevelUp()
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowText());
    }
    void Start()
    {
    gameObject.SetActive(false);
    }

    private IEnumerator ShowText()
    {
        levelUpText.alpha = 1;
        yield return new WaitForSeconds(1.2f);
        levelUpText.alpha = 0;
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
