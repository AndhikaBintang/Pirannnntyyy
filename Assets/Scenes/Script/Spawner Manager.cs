using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public List<GameObject> fallingObjects;
    public Transform spawnPoint;
    public float rangeX, rangeY;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawner());    
    }

    private IEnumerator spawner()
    {
        while (true)
        {
            Vector3 spawnPos = spawnPoint.position;
            float x=Random.Range(-rangeX, rangeX);
            float y=Random.Range(-rangeY, rangeY);
            float mass = Random.Range(1, 20);
            spawnPos = new Vector3((spawnPos.x + x), spawnPos.y, (spawnPos.z + y));
            int index = (int)Random.Range(0, fallingObjects.Count);
            GameObject temp = GameObject.Instantiate(fallingObjects[index], spawnPos, Quaternion.identity);
            temp.AddComponent<Rigidbody>();
            temp.GetComponent<Rigidbody>().mass = mass;
            temp.AddComponent<ObjectFalling>();
            float time = Random.Range(2, 3);
            yield return new WaitForSeconds(time);

        }
    }
}
