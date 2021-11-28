
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{

    public class Grid : MonoBehaviour
    {

        [Header("Grid Settings")]
        [SerializeField] private int gridSizeX;
        [SerializeField] private int gridSizeY;
        [Tooltip("Edge size of each individual grid cell")]
        [SerializeField] private float CellDiameter = 1;

        [Tooltip("Bottom left corner of the grid on world positions")]
        [SerializeField] private Vector2 bottomLeftCorner;

        [Tooltip("Grid will take the world center as it's center if you check this option")]
        [SerializeField] private bool isCentered = false;

        [Tooltip("How many seperate threads can pathfinding operations work?")]
        [SerializeField] private int ThreadSize = 1;

        //Private Sets

        public static int threadSize { get; private set; }
        public static float cellDiameter { get; private set; }

        //Only get
        public int MaxSize { get { return gridSizeX * gridSizeY; } }

        public int GridSizeX { get { return gridSizeX; } }
        public int GridSizeY { get { return gridSizeY; } }

        private Cell[,] gridArray;

        private void Awake()
        {
            //Give value from serializables to private sets
            threadSize = ThreadSize;
            cellDiameter = CellDiameter;

            //Calculate the bottom left corner if the grid is centered
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
                    gridArray[x, y] = new Cell(x, y, threadSize, GridToWorldPosition(x, y), 0);
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

        public Vector2 WorldPositionToGridArray(Vector2 _worldPosition)
        {

            Vector2 gridIndex = new Vector2(0, 0);

            _worldPosition = WorldToGridMatrix(_worldPosition); //Transform the world matrix to grid matrix by moving zero to bottom left of the grid

            gridIndex.x = GridMath.DivideAndFloorToDivisor(_worldPosition.x, cellDiameter); //Divide with cellDiameter and floor to cellDiameter to get the grid index
            gridIndex.y = GridMath.DivideAndFloorToDivisor(_worldPosition.y, cellDiameter); //Divide with cellDiameter and floor to cellDiameter to get the grid index  

            gridIndex.x = Mathf.Clamp(gridIndex.x, 0, gridSizeX - 1); //Clamp the index between 0 and grid size
            gridIndex.y = Mathf.Clamp(gridIndex.y, 0, gridSizeX - 1); //Clamp the index between 0 and grid size

            return gridIndex;
        }

        public Cell GridPositionToCell(int gridX, int gridY) //Removes the world offset from given position 
        {

            gridX  = Mathf.Clamp(gridX, 0, gridSizeX - 1); //Clamp the index between 0 and grid size
            gridY = Mathf.Clamp(gridY, 0, gridSizeY - 1); //Clamp the index between 0 and grid size

            return gridArray[gridX, gridY];

        }

        public Vector2 GridToWorldPosition(int _gridIndexX,int _gridIndexY)
        {

            Vector2 gridIndex = new Vector2(_gridIndexX, _gridIndexY); //Take the grid index

            Vector2 worldPosition = new Vector2((gridIndex.x * cellDiameter) + (cellDiameter / 2), (gridIndex.y * cellDiameter) + (cellDiameter / 2));  // Multiply with radius

            worldPosition = GridToWorldMatrix(worldPosition); //Convert to world matrix

            return worldPosition;
        }

        public Vector2 WorldToGridMatrix(Vector2 _worldPosition) //Removes the world offset from given position 
        {

            Vector2 worldPositionWithoutOffset = _worldPosition - bottomLeftCorner;
            return worldPositionWithoutOffset;

        }

        public Vector2 GridToWorldMatrix(Vector2 _gridPosition) //Removes the world offset from given position 
        {

            Vector2 worldPositionWithOffset = _gridPosition + bottomLeftCorner;
            return worldPositionWithOffset;

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


        //Collisions

        public IStaticUnit StaticCollisionCheckOnPosition(Vector2 _worldPosition)
        {

            Vector2 gridIndex = WorldPositionToGridArray(_worldPosition);

            Cell chosenCell = gridArray[Mathf.FloorToInt(gridIndex.x), Mathf.FloorToInt(gridIndex.y)];

            return chosenCell.staticUnit;


        }

        public IDynamicUnit DynamicCollisionCheckOnPosition(Vector2 _worldPosition)
        {
            Vector2 gridIndex = WorldPositionToGridArray(_worldPosition);

            Cell chosenCell = gridArray[Mathf.FloorToInt(gridIndex.x), Mathf.FloorToInt(gridIndex.y)];

            return chosenCell.dynamicUnitList[0];
        }

        public bool SimpleCollisionCheckOnPosition(Vector2 _worldPosition)
        {
            Vector2 gridIndex = WorldPositionToGridArray(_worldPosition);

            Cell _cell = gridArray[Mathf.FloorToInt(gridIndex.x), Mathf.FloorToInt(gridIndex.y)];

            if (_cell.dynamicUnitList.Count > 0 || _cell.staticUnit != null)
                return true;

            return false;

        }


      

        

    }

    public class Cell : IHeapItem<Cell>
    {

        public int terrainIndex; //Like 0 = Grass , 1 = Wall , 2 = Mud
        public int defaultTerrainIndex { get; private set; }
        public int gridX { get; private set; } //X Index on grid
        public int gridY { get; private set; } //Y Index on grid

        public Vector2 worldPosition { get; private set; } //World position of the cell
      

        //Pathfinding is using arrays to work simultaneously

        public int[] gCost; //Distance between start of the path and this cell
        public int[] hCost; //Distance between end of the path and this cell

        public Cell[] previousCellOnPath; //Used to record previous path in pathfinding

        public int[] HeapIndex { get; set; } //Index in heap array

        //Collisions

        public List<IDynamicUnit> dynamicUnitList;
        public IStaticUnit staticUnit;


        public Cell(int _gridX, int _gridY, int threadSize,Vector2 _worldPosition, int _terrainIndex = 0)
        {
            gridX = _gridX;
            gridY = _gridY;
            terrainIndex = _terrainIndex;
            defaultTerrainIndex = _terrainIndex;
            worldPosition = _worldPosition;

            //Pathfinding informations

            gCost = new int[threadSize];
            hCost = new int[threadSize];
            previousCellOnPath = new Cell[threadSize];
            HeapIndex = new int[threadSize];

            //Collision

            dynamicUnitList = new List<IDynamicUnit>();
            staticUnit = null;
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

        public void UpdateCollision() //Update the collision with SimpleCollisionCheck
        {

            if (SimpleCollisionCheck()) 
            {

                terrainIndex = 1;

            }
            else 
            {

                terrainIndex = defaultTerrainIndex;

            }

        }

        private bool SimpleCollisionCheck() //Update the collision by checking dynamic and static units
        {
            if (dynamicUnitList.Count > 0 || staticUnit != null)
                return true;

            return false;

        }

        public void PlaceDynamicUnit(IDynamicUnit _dynamicUnit) //Add dynamic unit to list
        {

            dynamicUnitList.Add(_dynamicUnit);

            UpdateCollision();

        }

        public void RemoveDynamicUnit(IDynamicUnit _dynamicUnit) //Remove dynamic unit from list
        {

            if (!dynamicUnitList.Contains(_dynamicUnit)) //If it doesnt contain the item, then return
                return;

            dynamicUnitList.Remove(_dynamicUnit);

            UpdateCollision();

        }

        public void PlaceStaticUnit(IStaticUnit _staticUnit) 
        {

           staticUnit = _staticUnit;

            UpdateCollision();

        }
        public void RemoveStaticUnit() 
        {

            staticUnit = null;

            UpdateCollision();

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

    public interface IStaticUnit
    {
        //Unique point is for spawn destination at barracks
        //And can be used for special effects on other buildings
        
        void SetUniquePoint(Vector2 _point);
        Vector2 GetUniquePoint();

        void SetSpawnPoint(Vector2 _point);
        Vector2 GetSpawnPoint();
        int GetUIIndex();
        void DamageUnit(float _damageAmount);



    }

    public interface IDynamicUnit : IPathReceiver
    {



    }

}















