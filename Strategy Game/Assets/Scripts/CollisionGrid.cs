using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;

public class CollisionGrid : MonoBehaviour
{
    private GridSystem.Grid gridReference;
    private void Awake()
    {
      gridReference = GameObject.FindGameObjectWithTag("GridObject").GetComponent<GridSystem.Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

public class CollisionCell
{

    UnitAI[] dynamicUnitArray;

    public CollisionCell() 
    {

        dynamicUnitArray = new UnitAI[2];

    }


}




