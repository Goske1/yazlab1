using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; 
    // enemyUIPrefab satırını sildik, artık gerek yok.

    void SpawnEnemy(Vector3 position)
    {
        // Sadece düşmanı yarat. Şapkası (UI) zaten üstünde gelecek.
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        
        // UI yaratma ve bağlama satırlarını SİLİYORUZ:
        // GameObject ui = Instantiate(enemyUIPrefab);
        // EnemyUI uiComp = ui.GetComponent<EnemyUI>();
        // uiComp.enemyStats = enemy.GetComponent<EnemyStats>();
    }
}