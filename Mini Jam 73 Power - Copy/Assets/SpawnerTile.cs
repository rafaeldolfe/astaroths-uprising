using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SpawnerTile : Tile, ILifecycleTile
{
    private MapManager mm;
    public GameObject spawnerType;
    public void TileAwake(Vector3Int position, Tilemap tilemap)
    {

    }
    public void TileStart(Vector3Int position, Tilemap tilemap)
    {
        List<Type> depTypes = ProgramUtils.GetMonoBehavioursOnType(this.GetType());
        List<MonoBehaviour> deps = new List<MonoBehaviour>
            {
                (mm = FindObjectOfType(typeof(MapManager)) as MapManager),
            };
        if (deps.Contains(null))
        {
            throw ProgramUtils.DependencyException(deps, depTypes);
        }

        if (spawnerType.name.Contains("Medium"))
        {
            int i = 0;
        }
        mm.SpawnSpawner(position, spawnerType);
    }
}