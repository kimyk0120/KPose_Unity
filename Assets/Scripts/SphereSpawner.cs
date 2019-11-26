using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour
{
    public GameObject[] spPrefab;
    
    public float spawnRateMin = 3f;
    public float spawnRateMax = 8f;
    public Transform target;
    private float spawnRate;
    private float timeAfterSpawn;
    
    void Start()
    {
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
    }

    // Update is called once per frame
    void Update()
    {
        timeAfterSpawn += Time.deltaTime;
        if (timeAfterSpawn >= spawnRate)
        {
            timeAfterSpawn = 0f;
            int a = Random.Range(0, 3);
            GameObject sp = Instantiate(spPrefab[a], 
                new Vector3(Random.Range(-1f,1f), 4, Random.Range(-.3f,.3f)) , transform.rotation);
            sp.transform.LookAt(target);
            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }
    }
}
