using UnityEngine;
using System.Collections;

public class GermSpawner : MonoBehaviour
{
    public GameObject germPrefab;

    public int maxGerms = 10;
    public float spawnInterval = 2f;

    public float spawnRadius = 3f;
    public float spawnHeight = 1.0f;

    private int currentGerms = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentGerms < maxGerms)
            {
                SpawnGerm();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnGerm()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            Random.Range(0f, spawnHeight),
            Random.Range(-spawnRadius, spawnRadius)
        );

        Vector3 spawnPos = transform.position + randomOffset;

        GameObject germ = Instantiate(germPrefab, spawnPos, Quaternion.identity);

        currentGerms++;
    }
}