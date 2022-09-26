using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] Customer[] customerPrefabs;
    [SerializeField] Transform exitTrans;
    [SerializeField] int customersToSpawn;
    [SerializeField] Vector2 timeBetweenSpawns;

    const float TIME_TO_NEXT_SUBWAVE = 1f;

    int subwaveNum;

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
        timeToSpawnNext = Time.time + Random.Range(timeBetweenSpawns.x, timeBetweenSpawns.y) * getSubwaveMult();
    }

    float getSubwaveMult()
    {
        float timeInWave = Time.time % (TIME_TO_NEXT_SUBWAVE * 3);
        float subwaveMult;
        int prevSubwaveNum = subwaveNum;
        int newSubwaveNum = (int)Mathf.Floor(timeInWave / TIME_TO_NEXT_SUBWAVE);

        if (timeInWave > TIME_TO_NEXT_SUBWAVE * 2)
        {
            subwaveMult = .64f;
            newSubwaveNum = 2;
        }
        else if (timeInWave > TIME_TO_NEXT_SUBWAVE)
        {
            subwaveMult = 8f;
        }
        else// if (timeInWave > 0)
        {
            subwaveMult = 1;
        }

        return subwaveMult;
    }
}