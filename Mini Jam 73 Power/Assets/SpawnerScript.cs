using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public float spawnRadius;
    public GameObject unitToSpawn;
    private Coroutine spawningCoroutine;
    private bool spawning = false;
    public float progressThreshold;
    private void Start()
    {
        spawning = true;
        spawningCoroutine = StartCoroutine(SpawnPeriodically());
    }
    private IEnumerator SpawnPeriodically()
    {
        while (spawning)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(20,30));
            if (MapManager.PlayerProgress > progressThreshold)
            {
                SpawnEnemy();
            }
        }
    }
    private void SpawnEnemy()
    {
        Vector3 position = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
        Vector3 newPosition = transform.position + position * UnityEngine.Random.Range(spawnRadius * 0.7f, spawnRadius);
        Instantiate(unitToSpawn, newPosition, Quaternion.identity);
    }
}
