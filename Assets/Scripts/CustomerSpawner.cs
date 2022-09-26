using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] Customer[] customerPrefabs;
    [SerializeField] Transform exitTrans;
    [SerializeField] int customersToSpawn;
    [SerializeField] Vector2 timeBetweenSpawns;

    const float TIME_TO_NEXT_SUBWAVE = 48;

    int waveNum;

    float timeToSpawnNext;

    // Start is called before the first frame update
    void Start()
    {
        timeToSpawnNext = Time.time + 2;
    }

    // Update is called once per frame
    void Update()
    {
        int newWaveNum = getWave();
        if (waveNum != newWaveNum)
        {
            waveNum = newWaveNum;
            AudioManager.playTheme(waveNum, false);
        }

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

    int getWave()
    {
        int val = Mathf.Clamp((int)Mathf.Floor((Time.time + AudioManager.instance.FADE_TIME / 2) / (TIME_TO_NEXT_SUBWAVE * 3)), 0, 3);

        if (val > 2)
            val = 2;

        return val;
    }

    int getSubwave()
    {        
        float timeInWave = Time.time % (TIME_TO_NEXT_SUBWAVE * 3);
        return (int)Mathf.Floor(timeInWave / TIME_TO_NEXT_SUBWAVE);
    }

    float getSubwaveMult()
    {
        const float SUBWAVE_MULT = .8f;

        float subwaveMult = Mathf.Pow(SUBWAVE_MULT, getWave() + getSubwave());

        //Debug.LogWarning("Time: " + Time.time + "  wave: " + getWave() + "   subwave: " + getSubwave() + "     subwaveMult: " + subwaveMult);

        return subwaveMult;
    }
}