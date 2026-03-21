using UnityEngine;
using System.Collections;
using Meta.XR.MRUtilityKit;

public class GermSpawnerMR : MonoBehaviour
{
    public GameObject germPrefab;

    public int maxGerms = 10;
    public float spawnInterval = 2f;

    public float spawnRange = 2.5f; // how far from center

    private int currentGerms = 0;
    private MRUKRoom room;

    void Start()
    {
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        // Wait for MRUK to load room
        yield return new WaitUntil(() => MRUK.Instance != null && MRUK.Instance.GetCurrentRoom() != null);

        room = MRUK.Instance.GetCurrentRoom();

        Debug.Log("MRUK Ready → Spawning on floor");

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentGerms < maxGerms)
            {
                SpawnOnFloor();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOnFloor()
    {
        if (room == null) return;

        MRUKAnchor floor = room.GetFloorAnchor();

        if (floor == null)
        {
            Debug.Log("No floor found!");
            return;
        }

        Vector3 center = floor.GetAnchorCenter();

        float randomX = Random.Range(-spawnRange, spawnRange);
        float randomZ = Random.Range(-spawnRange, spawnRange);

        Vector3 spawnPos = center + new Vector3(randomX, 0.05f, randomZ);

        GameObject germ = Instantiate(germPrefab, spawnPos, Quaternion.identity);

        // Track death
       /* EnemyHealth health = germ.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.onDeath += OnGermDeath;
        }*/

        currentGerms++;
    }

    void OnGermDeath()
    {
        currentGerms--;
    }
}