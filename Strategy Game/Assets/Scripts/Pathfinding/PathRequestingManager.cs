﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
public class PathRequestingManager : MonoBehaviour
{

    //References

    private Pathfinding pathFindingReference;
    private GridSystem.Grid gridReference;

    //Privates
    private PathCallSocket[] pathCallSocketArray;
    private List<PathCallBox> waitingPathCalls = new List<PathCallBox>();


    private void Awake()
    {

        if(pathFindingReference == null)
        pathFindingReference = FindObjectOfType<Pathfinding>();

        if (gridReference == null)
        gridReference = FindObjectOfType<GridSystem.Grid>();

    }

    private void Start()
    {
        pathCallSocketArray = new PathCallSocket[GridSystem.Grid.threadSize];

        for (int i = 0; i < pathCallSocketArray.Length; i++)
        {
            pathCallSocketArray[i] = new PathCallSocket(i);
        }
    }

<<<<<<< HEAD
=======
 
    public void TryPath(IPathReceiver _pathReceiver, Vector3 _startPos, Vector3 _targetPos) //Check if the thread is busy in this method
    {
>>>>>>> parent of c556a24 (Fixed Thread)



<<<<<<< HEAD
    public void TryPathThreading(IPathReceiver _pathReceiver, Vector3 _startPos, Vector3 _targetPos) //Check if the thread is busy in this method
=======
        }
        
    
    }

    public Thread StartThePathThread(IPathReceiver _pathReceiver, Vector3 _startPos, Vector3 _targetPos, Thread _thread)
    {
        var t = new Thread(() => RequestPath(_pathReceiver, _startPos, _targetPos, _thread));
        t.Start();
        return t;
    }
    public void RequestPath(IPathReceiver _pathReceiver,Vector3 _startPos, Vector3 _targetPos,Thread _thread) //Ask for a if one of the sockets is free
>>>>>>> parent of c556a24 (Fixed Thread)
    {


        PathCallSocket freeSocket = null;

        for (int i = 0; i < pathCallSocketArray.Length; i++) //Check if the sockets are available
        {
            if (!pathCallSocketArray[i].socketOn) //If socket is not occupied
                freeSocket = pathCallSocketArray[i]; //Take the socket
        }

        if (freeSocket != null) //If thread is not working
        {

            StartThePathThread(_pathReceiver, _startPos, _targetPos,freeSocket); //Start the thread

        }
        else 
        {

            waitingPathCalls.Add(new PathCallBox(_pathReceiver, _startPos, _targetPos)); //Else add to the queue

        }
        
    
    }

    public Thread StartThePathThread(IPathReceiver _pathReceiver, Vector3 _startPos, Vector3 _targetPos,PathCallSocket _socket)
    {
        Thread t = new Thread(() => _socket.CallForPath(pathFindingReference, _pathReceiver, _startPos, _targetPos, waitingPathCalls));
        t.Start();
        return t;
    }

    public void CancelRequest(IPathReceiver _pathReceiver, List<PathCallBox> _waitingCallList)
    {

     

        for (int i = 0; i < pathCallSocketArray.Length; i++) //Look if there is one pathreceiver working in sockets same as the given
        {

            if(pathCallSocketArray[i].pathReceiver == _pathReceiver) //If found
            {
                pathCallSocketArray[i].workingThread.Abort(); //Stop it
                return;
            }

        }

        for (int i = 0; i < waitingPathCalls.Count; i++) //Otherwise search in the queue
        {
            if (waitingPathCalls[i].pathReceiver == _pathReceiver) //If there is one
            {
                waitingPathCalls.RemoveAt(i); //Kill the queue
                return;
            }

        }


    }

   

}

public class PathCallSocket //Class for calling and sending paths to receivers
{

    public bool socketOn { get; private set; } //Is this socket currently working?
    public int threadIndex { get; private set; } //Thread index of socket
    public IPathReceiver pathReceiver { get; private set; } //Receiver of the called path
    public Thread workingThread;
    
    public PathCallSocket(int _threadIndex) 
    {
        threadIndex = _threadIndex;
    }

    public void CallForPath(Pathfinding _pathFinding, IPathReceiver _pathReceiver,Vector3 _startPos, Vector3 _targetPos, List<PathCallBox> _waitingCallList) 
    {

        pathReceiver = _pathReceiver; //Assign the receiver

<<<<<<< HEAD
        workingThread = Thread.CurrentThread; //Change the current thread
=======
        workingThread = _workingThread; //Change the thread data
>>>>>>> parent of c556a24 (Fixed Thread)

        SwitchSocket(true); //Socket is on

        List<Cell> _path = _pathFinding.FindPath(_startPos, _targetPos, threadIndex); //Start searching

        SendPathToReceiver(_path); //Send the path to receiver when search is done

        if(_waitingCallList.Count > 0) //If there is a call waiting in queue
        {


            PathCallBox _callBox = _waitingCallList[0]; //Take the item in the queue

            _waitingCallList.RemoveAt(0); //Remove it

            CallForPath(_pathFinding, _callBox.pathReceiver, _callBox.startPosition, _callBox.targetPosition, _waitingCallList); //and run the method again

            UnityEngine.Debug.Log("Girildi");

        }




        SwitchSocket(false); //Else close the socket



    }


   
    public void SendPathToReceiver(List<Cell> _path)
    {

        pathReceiver.GetPath(_path); //Give the path to receiver

    


    }

    public void SwitchSocket(bool switchBool) //Change the socket bool
    {
        socketOn = switchBool;
    }


}

public class PathCallBox //To store path receivers out of sockets
{

    public PathCallBox(IPathReceiver _pathReceiver,Vector3 _startPos,Vector3 _targetPos) 
    {
        pathReceiver = _pathReceiver;
        startPosition = _startPos;
        targetPosition = _targetPos;
    }

    public Vector3 startPosition;
    public Vector3 targetPosition;
    public IPathReceiver pathReceiver;
}

public interface IPathReceiver 
{

    List<Cell> path { get; set; }

    void GetPath(List<Cell> _path);

}

