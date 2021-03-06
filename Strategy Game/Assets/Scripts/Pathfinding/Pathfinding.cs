using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
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



    public List<Cell> FindPath(Vector3 _startPos, Vector3 _targetPos, int _threadIndex)
    {

        Cell startCell = GridReference.WorldPositionToCell(_startPos); //Gets the closest cell to the starting position
        Cell targetCell = GridReference.WorldPositionToCell(_targetPos); //Gets the closest cell to the target position

        if(targetCell.terrainIndex == 1) 
        {
            targetCell = LookForClosestAvailableCell(targetCell);
        }

        Heap<Cell> openList = new Heap<Cell>(GridReference.MaxSize, _threadIndex); //Heap List of cells for the open list
        HashSet<Cell> closedList = new HashSet<Cell>(); //Hashset of cells for the closed list

        openList.Add(startCell);//Add the starting Cell to the open list to begin the program

        while (openList.Count > 0)//Whilst there is something in the open list
        {
            Cell currentCell = openList.RemoveFirst(); //Remove the first item in the heap then rearrange the rest

            closedList.Add(currentCell);//And add it to the closed list

            if (currentCell == targetCell)//If the current Cell is the same as the target Cell
            {
                return GetFinalPath(startCell, targetCell, _threadIndex);//Calculate the final path

            }

            foreach (Cell neighborCell in GridReference.GetNeighboringCells(currentCell))//Loop through each neighbor of the current Cell
            {


                if (neighborCell.StaticCollisionCheck() != null || closedList.Contains(neighborCell)) //If the neighbor has a static object or has already been checked
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
        return null;
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

    public Cell LookForClosestAvailableCell(Cell startCell)
    {

        System.Random random = new System.Random();

        List<Cell> openList = new List<Cell>(); //Heap List of cells for the open list
        HashSet<Cell> closedList = new HashSet<Cell>(); //Hashset of cells for the closed list

        openList.Add(startCell);//Add the starting Cell to the open list to begin the program

        while (openList.Count > 0)//While there is something in the open list
        {
            Cell currentCell = openList[random.Next(0,openList.Count)]; //Find a random cell next to current cell

            openList.RemoveAt(0);

            closedList.Add(currentCell);//And add it to the closed list

            if (currentCell.terrainIndex != 1)//If the current Cell is available, then return it
            {
                return currentCell;

            }

            foreach (Cell neighborCell in GridReference.GetNeighboringCells(currentCell))//Loop through each neighbor of the current Cell
            {

                if (!openList.Contains(neighborCell))//If the neighbor is not in the openlist
                {
                    openList.Add(neighborCell); //Add it to the list
                }

            }

        }

        return null;

    }

}


