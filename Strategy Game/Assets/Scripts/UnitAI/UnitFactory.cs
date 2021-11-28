using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    private PoolSpawner poolingManager;

    public List<UnitObject> unitList = new List<UnitObject>();

    void Start()
    {

        poolingManager = GameObject.FindGameObjectWithTag("PoolingManager").GetComponent<PoolSpawner>();
        
    }

    public GameObject SpawnUnitWithIndex(int index,Vector2 _position) //Spawn the item with given index
    {

      GameObject spawnedUnit =  poolingManager.spawnObject(unitList[index].spawnString, _position, Quaternion.identity);


        return spawnedUnit;
    
    }


  
}
