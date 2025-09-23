using UnityEngine;
using System.Collections.Generic;

public class PlatformSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Prefabs & Pooling")]
    public List<GameObject> platformPrefabs;
    public int poolPerPrefab = 6;

    [Header("Spawn tuning")]
    public float gapMin = 0.6f;
    public float gapMax = 2.2f;
    public float minY = -1f;
    public float maxY = 3f;
    public float maxRise = 2f;   // max kenaikan tinggi antar platform
    public float maxDrop = 3f;   // max penurunan antar platform
    public float despawnDistance = 12f; // jarak di belakang player untuk despawn

    [Header("Auto-calc from player (optional)")]
    public float playerSpeedX = 4f;
    public float jumpInitialVelocity = 5f; // isi sesuai player
    public bool autoCalcGapFromJump = true;
    public float safetyFactor = 0.9f;

    // internal
    private List<Queue<GameObject>> pools;
    private List<GameObject> activePlatforms = new List<GameObject>();
    private float lastPlatformRightX = 0f;
    private float lastPlatformY = 0f;

    void Start()
    {
        if (platformPrefabs == null || platformPrefabs.Count == 0) { enabled = false; Debug.LogError("No platform prefabs!"); return; }

        // buat pool
        pools = new List<Queue<GameObject>>();
        for (int i = 0; i < platformPrefabs.Count; i++)
        {
            var q = new Queue<GameObject>();
            for (int j = 0; j < poolPerPrefab; j++)
            {
                var go = Instantiate(platformPrefabs[i], Vector3.one * 9999f, Quaternion.identity);
                var p = go.GetComponent<Platform>();
                if (p != null) p.prefabIndex = i;
                go.SetActive(false);
                q.Enqueue(go);
            }
            pools.Add(q);
        }

        // spawn awal
        SpawnInitialChain();
    }

    void Update()
    {
        // spawn lebih banyak kalau player mendekat
        float spawnAheadKey = player.position.x + playerSpeedX * 3f;
        if (spawnAheadKey > lastPlatformRightX)
            SpawnPlatform();

        // cek platform yang sudah jauh di belakang -> return to pool
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            var p = activePlatforms[i];
            float w = GetPlatformWidth(p);
            float rightEdge = p.transform.position.x + w / 2f;
            if (rightEdge < player.position.x - despawnDistance)
            {
                ReturnToPool(p);
                activePlatforms.RemoveAt(i);
            }
        }
    }

    void SpawnInitialChain()
    {
        // pertama platform di x = 0
        var first = GetFromPool(0);
        float w = GetPlatformWidth(first);
        first.transform.position = new Vector3(w / 2f, 0f, 0f);
        first.SetActive(true);
        activePlatforms.Add(first);
        lastPlatformRightX = first.transform.position.x + w / 2f;
        lastPlatformY = first.transform.position.y;

        // beberapa platform awal
        for (int i = 0; i < 4; i++) SpawnPlatform();
    }

    void SpawnPlatform()
{
    int idx = Random.Range(0, platformPrefabs.Count);
    var go = GetFromPool(idx);
    float width = GetPlatformWidth(go);

    float gap = 0f;

    // random pilih jenis gap
    float roll = Random.value; // antara 0 - 1
    if (roll < 0.3f)
    {
        // 30% chance: platform rapat/menempel
        gap = Random.Range(0f, 0.5f);
    }
    else if (roll < 0.8f)
    {
        // 50% chance: normal gap
        gap = Random.Range(gapMin, gapMax);
    }
    else
    {
        // 20% chance: jauh tapi masih bisa dilompati
        float maxGap = autoCalcGapFromJump 
            ? ComputeMaxHorizontalDistance(playerSpeedX, jumpInitialVelocity) * safetyFactor 
            : gapMax * 1.5f;

        gap = Random.Range(gapMax, Mathf.Max(gapMax + 1f, maxGap));
    }

    float spawnCenterX = lastPlatformRightX + gap + width / 2f;

    // ketinggian: harus dalam batas maxRise / maxDrop relatif ke platform sebelumnya
    float minAllowedY = Mathf.Max(minY, lastPlatformY - maxDrop);
    float maxAllowedY = Mathf.Min(maxY, lastPlatformY + maxRise);
    float y = Random.Range(minAllowedY, maxAllowedY);

    go.transform.position = new Vector3(spawnCenterX, y, 0f);
    go.SetActive(true);
    activePlatforms.Add(go);

    lastPlatformRightX = spawnCenterX + width / 2f;
    lastPlatformY = y;
}


    GameObject GetFromPool(int prefabIndex)
    {
        if (pools[prefabIndex].Count > 0)
            return pools[prefabIndex].Dequeue();
        else
        {
            var g = Instantiate(platformPrefabs[prefabIndex]);
            var p = g.GetComponent<Platform>();
            if (p != null) p.prefabIndex = prefabIndex;
            return g;
        }
    }

    void ReturnToPool(GameObject platform)
    {
        var p = platform.GetComponent<Platform>();
        int idx = p != null ? p.prefabIndex : 0;
        platform.SetActive(false);

        // reset local state jika perlu (animator, velocity, child objects)
        platform.transform.SetParent(transform); // keep hierarchy tidy
        pools[idx].Enqueue(platform);
    }

    float GetPlatformWidth(GameObject go)
    {
        var p = go.GetComponent<Platform>();
        if (p != null) return p.GetWidth();
        var r = go.GetComponentInChildren<Renderer>();
        if (r != null) return r.bounds.size.x;
        var c = go.GetComponent<Collider>();
        if (c != null) return c.bounds.size.x;
        var c2 = go.GetComponent<Collider2D>();
        if (c2 != null) return c2.bounds.size.x;
        return 1f;
    }

    // helper physics
    public float ComputeMaxJumpHeight(float jumpVelocity)
    {
        float g = Mathf.Abs(Physics.gravity.y);
        return (jumpVelocity * jumpVelocity) / (2f * g);
    }
    public float ComputeAirTime(float jumpVelocity)
    {
        float g = Mathf.Abs(Physics.gravity.y);
        return 2f * jumpVelocity / g;
    }
    public float ComputeMaxHorizontalDistance(float speedX, float jumpVelocity)
    {
        return speedX * ComputeAirTime(jumpVelocity);
    }
}
