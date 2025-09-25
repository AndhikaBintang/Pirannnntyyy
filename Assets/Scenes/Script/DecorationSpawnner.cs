using System.Collections.Generic;
using UnityEngine;

public class DecorationSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public PlatformSpawner platformSpawner; // referensi ke PlatformSpawner

    [Header("Prefabs & Pooling")]
    public List<GameObject> decorationPrefabs;
    public int poolPerPrefab = 6;

    [Header("Spawn Settings")]
    public float minYOffset = 0.5f; // jarak minimum di atas platform
    public float maxYOffset = 2f;   // jarak maksimum di atas platform
    public float minZOffset = -2f;  // variasi ke belakang
    public float maxZOffset = 2f;   // variasi ke depan
    public float chanceToSpawn = 0.7f; // peluang spawn dekorasi per platform
    public float despawnDistance = 20f;

    // internal
    private List<Queue<GameObject>> pools;
    private List<GameObject> activeObjects = new List<GameObject>();

    void Start()
    {
        if (decorationPrefabs == null || decorationPrefabs.Count == 0)
        {
            enabled = false;
            Debug.LogError("No decoration prefabs!");
            return;
        }

        // pool
        pools = new List<Queue<GameObject>>();
        for (int i = 0; i < decorationPrefabs.Count; i++)
        {
            var q = new Queue<GameObject>();
            for (int j = 0; j < poolPerPrefab; j++)
            {
                var go = Instantiate(decorationPrefabs[i], Vector3.one * 9999f, Quaternion.identity);
                go.SetActive(false);
                q.Enqueue(go);
            }
            pools.Add(q);
        }

        // subscribe ke event platform spawn
        if (platformSpawner != null)
        {
            platformSpawner.OnPlatformSpawned += TrySpawnDecoration;
        }
    }

    void Update()
    {
        // despawn object lama
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            var obj = activeObjects[i];
            if (obj.transform.position.x < player.position.x - despawnDistance)
            {
                ReturnToPool(obj);
                activeObjects.RemoveAt(i);
            }
        }
    }

    void TrySpawnDecoration(GameObject platform)
    {
        if (Random.value > chanceToSpawn) return;

        int idx = Random.Range(0, decorationPrefabs.Count);
        var go = GetFromPool(idx);

        // ambil posisi platform
        Vector3 basePos = platform.transform.position;
        float yOffset = Random.Range(minYOffset, maxYOffset);
        float zOffset = Random.Range(minZOffset, maxZOffset);

        Vector3 spawnPos = new Vector3(basePos.x, basePos.y + yOffset, zOffset);

        go.transform.position = spawnPos;
        go.SetActive(true);
        activeObjects.Add(go);
    }

    GameObject GetFromPool(int prefabIndex)
    {
        if (pools[prefabIndex].Count > 0)
            return pools[prefabIndex].Dequeue();
        else
            return Instantiate(decorationPrefabs[prefabIndex]);
    }

    void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        int idx = decorationPrefabs.IndexOf(obj);
        if (idx >= 0 && idx < pools.Count)
            pools[idx].Enqueue(obj);
    }
}
