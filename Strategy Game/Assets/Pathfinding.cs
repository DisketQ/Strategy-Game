using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using System.Diagnostics;
using System.Threading.Tasks;
public class Pathfinding : MonoBehaviour
{
    GridSystem.Grid GridReference; //Grid class reference
    public Vector2 StartPosition; //Starting position to pathfind from
    public Vector2 TargetPosition; //Starting position to pathfind to

    //REMOVAL


    private void Awake()
    {
        GridReference = GetComponent<GridSystem.Grid>();//Get a reference to the game manager
    }

    private void Start()
    {
            
    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            StartPosition = worldPos;

        }


        if (Input.GetKeyDown(KeyCode.Mouse1))
        {

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            TargetPosition = worldPos;

            for (int i = 0; i < GridReference.ThreadSize; i++)
            {

               FindPath(StartPosition, TargetPosition, i);//Find a path to the goal

            }
            

        }

        if (Input.GetKey(KeyCode.Space))
        {

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GridReference.WorldPositionToCell(new Vector2(worldPos.x, worldPos.y)).terrainIndex = 1;

        }

    }

    public async void FindPath(Vector3 _startPos, Vector3 _targetPos, int _threadIndex)
    {



        Cell startCell = GridReference.WorldPositionToCell(_startPos); //Gets the closest cell to the starting position
        Cell targetCell = GridReference.WorldPositionToCell(_targetPos); //Gets the closest cell to the target position

        Heap<Cell> openList = new Heap<Cell>(GridReference.MaxSize, _threadIndex); //Heap List of cells for the open list
        HashSet<Cell> closedList = new HashSet<Cell>(); //Hashset of cells for the closed list

        openList.Add(startCell);//Add the starting Cell to the open list to begin the program


        List<Cell> pathList = await Task.Run(() => 
        {

            pathList = new List<Cell>(); 

            while (openList.Count > 0)//Whilst there is something in the open list
            {
                Cell currentCell = openList.RemoveFirst(); //Remove the first item in the heap then rearrange the rest

                closedList.Add(currentCell);//And add it to the closed list

                if (currentCell == targetCell)//If the current Cell is the same as the target Cell
                {
                    pathList = GetFinalPath(startCell, targetCell, _threadIndex);//Calculate the final path

                    UnityEngine.Debug.Log(pathList.Count);

                    return pathList;

                }

                foreach (Cell neighborCell in GridReference.GetNeighboringCells(currentCell))//Loop through each neighbor of the current Cell
                {


                    if (neighborCell.terrainIndex == 1 || closedList.Contains(neighborCell))//If the neighbor is a wall or has already been checked
                    {

                        continue; //Skip it

                    }
                    int MoveCost = currentCell.gCost[_threadIndex] + GridMath.GetManhattenDistance(currentCell, neighborCell);//Get the F cost of that neighbor

                    if (MoveCost < neighborCell.gCost[_threadIndex] || !openList.Contains(neighborCell))//If the f cost is greater than the g cost or it is not in the open list
                    {
                        neighborCell.gCost[_threadIndex] = MoveCost;//Set the g cost to the f cost
                        neighborCell.hCost[_threadIndex] = GridMath.GetManhattenDistance(neighborCell, targetCell);//Set the h cost
                        neighborCell.previousCellOnPath[_threadIndex] = currentCell;//Set the parent of the Cell for retracing steps

                        if (!openList.Contains(neighborCell))//If the neighbor is not in the openlist
                        {
                            openList.Add(neighborCell); //Add it to the list
                        }
                        {
                            openList.UpdateItem(neighborCell); //Update the item if it is in the list
                        }
                    }
                }

            }

            return pathList;

        });


        //TEST


            



    }



    List<Cell> GetFinalPath(Cell startingCell, Cell endCell, int _threadIndex)
    {
        List<Cell> _finalPath = new List<Cell>();//List to hold the path sequentially 
        Cell currentCell = endCell;//Cell to store the current Cell being checked

        while (currentCell != startingCell)//While loop to work through each Cell going through the parents to the beginning of the path
        {

            _finalPath.Add(currentCell);//Add that Cell to the final path
            currentCell = currentCell.previousCellOnPath[_threadIndex];//Move onto its parent Cell
        }

        _finalPath.Reverse();//Reverse the path to get the correct order

        return _finalPath;//Set the final path

    }


}


public class PathfindingCell : IHeapItem<PathfindingCell>
{

    //Pathfinding is using arrays to work simultaneously

    public int[] gCost; //Distance between start of the path and this cell
    public int[] hCost; //Distance between end of the path and this cell
    public int[] fCost; // gCost + hCost

    public PathfindingCell[] previousCellOnPath; //Used to record previous path in pathfinding

    public int[] HeapIndex { get; set; } //Index in heap array

    public PathfindingCell(int _gridX, int _gridY, int threadSize)
    {

        gCost = new int[threadSize];
        hCost = new int[threadSize];
        fCost = new int[threadSize];
        previousCellOnPath = new PathfindingCell[threadSize];
        HeapIndex = new int[threadSize];
    }

    public int CompareTo(PathfindingCell otherCell, int _threadIndex)
    {
        int compare = GetFCost(_threadIndex).CompareTo(otherCell.GetFCost(_threadIndex)); //First compare fCosts 

        if (compare == 0) //Then if they are equal
        {
            compare = hCost[_threadIndex].CompareTo(otherCell.hCost[_threadIndex]); //Compare the hCosts
        }
        return -compare;


    }

    public int GetFCost(int threadIndex) //Calculate the F cost on the selected thread
    {

        return gCost[threadIndex] + hCost[threadIndex];

    }

}

