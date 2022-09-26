using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] Customer[] customerPrefabs;
    [SerializeField] Transform exitTrans;
    [SerializeField] int customersToSpawn;
    [SerializeField] Vector2 timeBetweenSpawns = new Vector2(10, 25);

    float timeToSpawnNext;

    // Start is called before the first frame update
    void Start()
    {
        timeToSpawnNext = Time.time + 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeToSpawnNext)
            SpawnCustomer();
    }

    void SpawnCustomer()
    {
        Customer spawnedCustomer = Instantiate(customerPrefabs[Random.Range(0, customerPrefabs.Length)], transform.position, Quaternion.identity);
        spawnedCustomer.exitTrans = exitTrans;

        customersToSpawn--;
        CustomerManager.instance.addCustomer();
        timeToSpawnNext = Time.time + Random.Range(timeBetweenSpawns.x, timeBetweenSpawns.y);
    }
}
