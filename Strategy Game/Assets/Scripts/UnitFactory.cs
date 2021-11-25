using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    PoolSpawner poolingManager;

    private Vector2 spawnPoint;

    public string UnitSpawnKey;

    void Start()
    {

        poolingManager = GameObject.FindGameObjectWithTag("PoolingManager").GetComponent<PoolSpawner>();
        
    }

    public void SpawnSoldier() 
    {

        poolingManager.spawnObject(UnitSpawnKey, spawnPoint, Quaternion.identity, true);
    
    }
    public void SetSpawnPoint(Vector2 _spawnPoint) 
    {

        spawnPoint = _spawnPoint;
    
    }

  
}
