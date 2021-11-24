
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace GridSystem
{

    public class Grid : MonoBehaviour
    {

        [Header("Grid Settings")]
        public int gridSizeX;
        public int gridSizeY;
        [Tooltip("Edge size of each individual grid cell")]
        public float cellDiameter = 1;
        [Tooltip("Bottom left corner of the grid on world positions")]
        [SerializeField] private Vector2 bottomLeftCorner;
        [Tooltip("Grid will take the world center as it's center if you check this option")]
        public bool isCentered;
        public int ThreadSize;

        //Privates

        public int MaxSize
        {
            get
            {
                return gridSizeX * gridSizeY;
            }
        }

        private Cell[,] gridArray;

        private void Awake()
        {
            if (isCentered)
                bottomLeftCorner = new Vector2(-gridSizeX / 2 * cellDiameter, -gridSizeY / 2 * cellDiameter);

            CreateGrid(); //Create the grid with the total size and cell diameter

        }

        private void CreateGrid()
        {

            gridArray = new Cell[gridSizeX, gridSizeY]; //New array from grid sizes

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    gridArray[x, y] = new Cell(x, y, ThreadSize, 0);
                }
            }

        }

        public Cell WorldPositionToCell(Vector2 _worldPosition)
        {

            Vector2 gridIndex = new Vector2(0, 0);

            _worldPosition = WorldToGridMatrix(_worldPosition); //Transform the world matrix to grid matrix by moving zero to bottom left of the grid

            gridIndex.x = GridMath.DivideAndFloorToDivisor(_worldPosition.x, cellDiameter); //Divide with cellDiameter and floor to cellDiameter to get the grid index
            gridIndex.y = GridMath.DivideAndFloorToDivisor(_worldPosition.y, cellDiameter); //Divide with cellDiameter and floor to cellDiameter to get the grid index  

            gridIndex.x = Mathf.Clamp(gridIndex.x, 0, gridSizeX - 1); //Clamp the index between 0 and grid size
            gridIndex.y = Mathf.Clamp(gridIndex.y, 0, gridSizeX - 1); //Clamp the index between 0 and grid size

            return gridArray[Mathf.RoundToInt(gridIndex.x), Mathf.RoundToInt(gridIndex.y)];
        }

        public Vector2 WorldToGridMatrix(Vector2 _worldPosition) //Removes the world offset from given position 
        {

            Vector2 worldPositionWithoutOffset = _worldPosition - bottomLeftCorner;
            return worldPositionWithoutOffset;

        }
        public List<Cell> GetNeighboringCells(Cell neighborCell)
        {

            List<Cell> neighborList = new List<Cell>();//Make a new list of all available neighbors.

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    //if we are on the node tha was passed in, skip this iteration.
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    int checkX = neighborCell.gridX + x;
                    int checkY = neighborCell.gridY + y;

                    //Make sure the node is within the grid.
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighborList.Add(gridArray[checkX, checkY]); //Adds to the neighbours list.
                    }

                }
            }

            return neighborList;//Return the neighbors list.
        }



    }

    public class Cell : IHeapItem<Cell>
    {

        public int terrainIndex; //Like 0 = Grass , 1 = Wall , 2 = Mud
        public int gridX { get; private set; } //X Index on grid
        public int gridY { get; private set; } //Y Index on grid

        //Pathfinding is using arrays to work simultaneously

        public int[] gCost; //Distance between start of the path and this cell
        public int[] hCost; //Distance between end of the path and this cell
        public int[] fCost; // gCost + hCost

        public Cell[] previousCellOnPath; //Used to record previous path in pathfinding

        public int[] HeapIndex { get; set; } //Index in heap array


        public Cell(int _gridX, int _gridY, int threadSize, int _terrainIndex = 0)
        {
            gridX = _gridX;
            gridY = _gridY;
            terrainIndex = _terrainIndex;

            gCost = new int[threadSize];
            hCost = new int[threadSize];
            fCost = new int[threadSize];
            previousCellOnPath = new Cell[threadSize];
            HeapIndex = new int[threadSize];
        }

        public int CompareTo(Cell otherCell, int _threadIndex)
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

    public class GridMath
    {
        public static float DivideAndFloorToDivisor(float dividend, float divisor)
        {

            float remainder = dividend % divisor; //Get the remainder

            dividend -= remainder; //Remove it from the number to floor it

            return dividend / divisor;

        }

        public static int GetManhattenDistance(Cell a_CellA, Cell a_CellB)
        {
            int ix = Mathf.Abs(a_CellA.gridX - a_CellB.gridX); //x1-x2
            int iy = Mathf.Abs(a_CellA.gridY - a_CellB.gridY); //y1-y2

            return ix + iy; //Return the sum
        }

    }

}















