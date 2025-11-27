using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [Header("Falling Object Prefabs")]
    public List<GameObject> fallingObjects;

    [Header("Spawn Settings")]
    public float rangeX = 5f;
    public float rangeZ = 5f;

    [Header("Spawn Rate Settings")]
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 2f;
    public float difficultyRamp = 0.97f;

    [Header("Burst Spawn")]
    public int minBurst = 1;
    public int maxBurst = 4;

    [Header("Performance Settings")]
    public int maxObjectsInScene = 120;
    public List<GameObject> activeObjects = new List<GameObject>();

    [Header("Follow Player Settings")]
    public Transform player;                      // <-- assign di Inspector
    public Vector3 followOffset = new Vector3(0, 10f, 0);
    [Range(1f, 20f)] public float followSmooth = 5f;

    // internal center used for spawning (follows player)
    private Vector3 spawnCenter;

    void Start()
    {
        // safety checks
        if (player == null)
            Debug.LogWarning("[SpawnerManager] Player not assigned! Spawner won't follow.");

        // initialize spawn center at current transform position
        spawnCenter = transform.position;

        StartCoroutine(SpawnerLoop());
    }

    void Update()
    {
        // follow player smoothly if assigned
        if (player != null)
        {
            Vector3 target = player.position + followOffset;
            spawnCenter = Vector3.Lerp(spawnCenter, target, followSmooth * Time.deltaTime);

            // also move the spawner GameObject (optional, helpful for debug gizmos)
            transform.position = spawnCenter;
        }
    }

    private IEnumerator SpawnerLoop()
    {
        while (true)
        {
            if (activeObjects.Count < maxObjectsInScene)
            {
                int burst = Random.Range(minBurst, maxBurst + 1);
                for (int i = 0; i < burst; i++)
                {
                    SpawnOneObject();
                }
            }

            // difficulty ramp (slowly reduce spawn times)
            minSpawnTime *= difficultyRamp;
            maxSpawnTime *= difficultyRamp;
            minSpawnTime = Mathf.Clamp(minSpawnTime, 0.1f, 5f);
            maxSpawnTime = Mathf.Clamp(maxSpawnTime, 0.2f, 6f);

            float wait = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(wait);
        }
    }

    private void SpawnOneObject()
    {
        if (fallingObjects == null || fallingObjects.Count == 0) return;

        float rx = Random.Range(-rangeX, rangeX);
        float rz = Random.Range(-rangeZ, rangeZ);

        Vector3 pos = new Vector3(
            spawnCenter.x + rx,
            spawnCenter.y,           // spawn at same Y as spawner center
            spawnCenter.z + rz
        );

        int index = Random.Range(0, fallingObjects.Count);
        GameObject obj = Instantiate(fallingObjects[index], pos, Quaternion.identity);

        Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.mass = Random.Range(1, 60);

        // add ObjectFalling and set its spawner ref if exists
        ObjectFalling fall = obj.AddComponent<ObjectFalling>();
        var sm = GetComponent<SpawnerManager>();
        if (sm != null) fall.spawner = sm;

        activeObjects.Add(obj);
    }

    public void RemoveObject(GameObject go)
    {
        if (activeObjects.Contains(go))
            activeObjects.Remove(go);
    }

    // debug: draw spawn area in editor
    void OnDrawGizmosSelected()
    {
        Vector3 center = (player != null) ? player.position + followOffset : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, new Vector3(rangeX * 2f, 0.1f, rangeZ * 2f));
    }
}
