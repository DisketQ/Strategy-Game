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

    public void RequestPath()
    {

       

    }

    

}

public class PathCallSocket //Class for calling and sending paths to receivers
{

    private bool socketOn; //Is this socket currently working?

    private IPathReceiver pathReceiver; //Receiver of the called path
    
    public void SwitchSocket(bool switchBool) //Change the socket bool
    {

        socketOn = switchBool;
    
    }



    public void SendPathToReceiver(List<Cell> _path)
    {

        pathReceiver.GetPath(_path); //Give the path to receiver

        pathReceiver = null; //Remove the receiver

        SwitchSocket(false); //Make the socket free

    }


}

public interface IPathReceiver 
{

    List<Cell> path { get; set; }

    void GetPath(List<Cell> _path);

}
