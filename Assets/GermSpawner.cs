using UnityEngine;
using System.Collections;
using Meta.XR.MRUtilityKit;

public class GermSpawnerMR : MonoBehaviour
{
    public GameObject germPrefab;

    public int maxGerms = 10;
    public float spawnInterval = 2f;

    [Header("Spawn Area")]
    public float spawnRange = 2.5f;
    public float floorOffsetY = 0.05f;

    [Header("Player Safety")]
    public Transform playerCam;
    public float playerSafeRadius = 1.0f;

    private int currentGerms = 0;
    private MRUKRoom room;

    private bool canSpawn = false;

    void Start()
    {
        if (playerCam == null && Camera.main != null)
        {
            playerCam = Camera.main.transform;
        }

        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        yield return new WaitUntil(() => MRUK.Instance != null && MRUK.Instance.GetCurrentRoom() != null);

        room = MRUK.Instance.GetCurrentRoom();

        Debug.Log("MRUK Ready → Germ spawner initialized");
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (canSpawn && currentGerms < maxGerms)
            {
                SpawnOnFloor();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void StartSpawning()
    {
        canSpawn = true;
        Debug.Log("Germ spawning started!");
    }

    public void StopSpawning()
    {
        canSpawn = false;
        Debug.Log("Germ spawning stopped!");
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
        Vector3 spawnPos = center;

        bool foundValidPosition = false;

        for (int i = 0; i < 20; i++)
        {
            float randomX = Random.Range(-spawnRange, spawnRange);
            float randomZ = Random.Range(-spawnRange, spawnRange);

            Vector3 testPos = center + new Vector3(randomX, floorOffsetY, randomZ);

            if (playerCam != null)
            {
                Vector3 playerFlat = new Vector3(playerCam.position.x, 0f, playerCam.position.z);
                Vector3 spawnFlat = new Vector3(testPos.x, 0f, testPos.z);

                float distanceToPlayer = Vector3.Distance(playerFlat, spawnFlat);

                if (distanceToPlayer < playerSafeRadius)
                {
                    continue;
                }
            }

            spawnPos = testPos;
            foundValidPosition = true;
            break;
        }

        if (!foundValidPosition)
        {
            Debug.Log("Could not find a valid spawn position away from player.");
            return;
        }

        Instantiate(germPrefab, spawnPos, Quaternion.identity);
        currentGerms++;
    }

    void OnGermDeath()
    {
        currentGerms--;
    }
}