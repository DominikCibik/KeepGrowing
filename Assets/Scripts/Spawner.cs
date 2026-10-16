using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] foodPrefabs;
    public GameObject[] laxativePrefabs;
    public int maxFoodCount = 10;
    public int initialFoodCountMin = 3;
    public int initialFoodCountMax = 5;
    public int maxLaxativeCount = 2;
    public float spawnInterval = 1f;
    public Vector2 spawnArea;

    private int currentFoodCount = 0;
    private int currentLaxativeCount = 0;

    public PlayerController player;

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        int initialFoodCount = Random.Range(initialFoodCountMin, initialFoodCountMax + 1);
        for (int i = 0; i < initialFoodCount; i++)
        {
            SpawnFood();
        }

        StartCoroutine(SpawnFoodGradually(initialFoodCount));
        SpawnLaxatives();
    }

    IEnumerator SpawnFoodGradually(int alreadySpawned)
    {
        while (currentFoodCount < maxFoodCount)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnFood();
        }
    }

    void SpawnFood()
    {
        if (currentFoodCount >= maxFoodCount || foodPrefabs == null || foodPrefabs.Length == 0)
            return;

        GameObject prefab = foodPrefabs[Random.Range(0, foodPrefabs.Length)];
        Vector2 position = new Vector2(
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y)
        );

        Instantiate(prefab, position, Quaternion.identity);
        currentFoodCount++;

        if (player != null)
        {
            player.AddToTotalFoodCount();
        }
    }

    void SpawnLaxatives()
    {
        for (int i = 0; i < maxLaxativeCount; i++)
        {
            GameObject prefab = laxativePrefabs[Random.Range(0, laxativePrefabs.Length)];
            Vector2 position = new Vector2(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y, spawnArea.y)
            );

            Instantiate(prefab, position, Quaternion.identity);
            currentLaxativeCount++;
        }
    }
}




