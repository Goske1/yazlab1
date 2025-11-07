using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuManager : MonoBehaviour
{

    public void StartGame()
    {

        SceneManager.LoadScene("Goske1 - test");
    }


    public void QuitGame()
    {

        Debug.Log("Oyundan çıkılıyor..."); 
        Application.Quit();
    }
}