using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int totalWaves = 5;
    public int enemiesPerWave = 4;          // Her dalgada sabit kalan sayı
    public float spawnInterval = 10f;
    public Vector3 size = new Vector3(20, 0, 20);
    public float minDistanceFromPlayer = 5f;
    public float maxDistanceFromSpawner = 15f;

    public Transform player;

    void Start()
    {
        StartCoroutine(SpawnWithDelay());
    }

    IEnumerator SpawnWithDelay()
    {
        for (int wave = 0; wave < totalWaves; wave++)
        {
            SpawnEnemies(enemiesPerWave);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemies(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos;
            int safetyCheck = 0;

            do
            {
                spawnPos = transform.position + new Vector3(
                    Random.Range(-size.x / 2, size.x / 2),
                    0,
                    Random.Range(-size.z / 2, size.z / 2)
                );

                safetyCheck++;
                if (safetyCheck > 25) break;

            } while (Vector3.Distance(spawnPos, player.position) < minDistanceFromPlayer ||
                     Vector3.Distance(spawnPos, transform.position) > maxDistanceFromSpawner);

            GameObject e = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            // dusman cani burada ayarlanabilir
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
