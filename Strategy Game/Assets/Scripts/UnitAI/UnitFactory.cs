using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;

public class UnitFactory : MonoBehaviour
{
    private PoolSpawner poolingManager;

    private GridSystem.Grid gridReference;

    private Pathfinding pathfindingReference;

    public List<UnitObject> unitList = new List<UnitObject>();

    void Start()
    {

        poolingManager =  FindObjectOfType<PoolSpawner>();

        gridReference = FindObjectOfType<GridSystem.Grid>();

        pathfindingReference = FindObjectOfType<Pathfinding>();
        
    }

    public GameObject SpawnUnitWithIndex(int index,Vector2 _position) //Spawn the item with given index
    {

        Cell spawnCell = gridReference.WorldPositionToCell(_position);

        if (spawnCell.SimpleCollisionCheck()) 
        {
          spawnCell =  pathfindingReference.LookForClosestAvailableCell(spawnCell);
        }
        
        GameObject spawnedUnit =  poolingManager.spawnObject(unitList[index].spawnString, spawnCell.worldPosition, Quaternion.identity);

        return spawnedUnit;
    
    }


  
}
