using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static float PlayerProgress;

    private GlobalEventManager gem;
    public Tilemap entities;
    public Tilemap obstacles;
    public Tilemap ground;

    public PlayerScript playerScript;

    private List<SpawnerScript> spawners = new List<SpawnerScript>();

    void Awake()
    {
        List<Type> depTypes = ProgramUtils.GetMonoBehavioursOnType(this.GetType());
        List<MonoBehaviour> deps = new List<MonoBehaviour>
            {
                (gem = FindObjectOfType(typeof(GlobalEventManager)) as GlobalEventManager),
            };
        if (deps.Contains(null))
        {
            throw ProgramUtils.DependencyException(deps, depTypes);
        }
        playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
    }
    private void Update()
    {
        PlayerProgress += (playerScript.power * 0.10f) * Time.deltaTime;
    }
    internal void SpawnSpawner(Vector3Int position, GameObject spawner)
    {
        GameObject spawnerClone = Instantiate(spawner, new Vector3(position.x + Constants.TILE_OFFSET_SPAWNING, position.y + Constants.TILE_OFFSET_SPAWNING), Quaternion.identity);
        spawners.Add(spawnerClone.GetComponent<SpawnerScript>());
    }
}

public class Constants
{
    public static float TILE_OFFSET_SPAWNING = 0.5f;

    public static float PROJECTILE_DRAIN_TIME = 3.0f;
    internal static float PROJECTILE_DAMAGE_POWER_RATIO = 1f;
    internal static float PROJECTILE_ABILITY_COOLDOWN = 3.0f;
    internal static float AOE_DAMAGE_POWER_RATIO = 0.5f;
    internal static float AOE_DURATION = 2.0f;
}