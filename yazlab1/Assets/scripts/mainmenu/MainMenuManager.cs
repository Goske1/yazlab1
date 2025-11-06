using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için BU SATIR ÇOK ÖNEMLİ!

public class MainMenuManager : MonoBehaviour
{
    // Bu fonksiyonu "Oyunu Başlat" butonu için kullanacağız
    public void StartGame()
    {
        // "GameScene" yazan yere kendi oyun sahnenizin ADINI YAZMALISINIZ
        // Adın büyük/küçük harf duyarlı ve tam olarak doğru olması gerekir
        SceneManager.LoadScene("Goske1 - test");
    }

    // Bu fonksiyonu "Çıkış" butonu için kullanacağız
    public void QuitGame()
    {
        // Bu komut Unity Editör'de çalışmaz, sadece build (exe) aldığınızda çalışır
        Debug.Log("Oyundan çıkılıyor..."); // Editörde çalıştığını anlamak için log
        Application.Quit();
    }
}