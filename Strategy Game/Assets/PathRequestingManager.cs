using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
public class PathRequestingManager : MonoBehaviour
{

    //References

    private Pathfinding pathFindingReference;
    private GridSystem.Grid gridReference;

    //Privates
    private PathCallSocket[] pathCallSockets;

    private void Awake()
    {

        if(pathFindingReference == null)
        pathFindingReference = FindObjectOfType<Pathfinding>();

        if (gridReference == null)
        gridReference = FindObjectOfType<GridSystem.Grid>();

    }

   

}

public class PathCallSocket
{

    private bool socketOn;


    
    public void SwitchSocket() 
    { 
    
    
    }

}

public interface IPathReceiver 
{ 


}
