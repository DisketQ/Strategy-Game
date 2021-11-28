using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using UnityEngine.EventSystems;
public class BuildingSpawner : MonoBehaviour
{

    [SerializeField] List<BuildingObject> buildingObjects;

    //List of current building area

    List<Cell> buildingAreaCells = new List<Cell>();

    //References

    private BuildingObject currentBuilding;
    private CollisionGrid collisionGridReference;
    private GridSystem.Grid gridReference;
    private PoolSpawner poolingManager;

    //Indicators

    private List<GameObject> activeGreenIndicators = new List<GameObject>();
    private List<GameObject> activeRedIndicators = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        if(collisionGridReference == null)
        collisionGridReference = FindObjectOfType<CollisionGrid>();

        if (poolingManager == null)
            poolingManager = FindObjectOfType<PoolSpawner>();

        if (gridReference == null)
            gridReference = FindObjectOfType<GridSystem.Grid>();


    }

    // Update is called once per frame
    void Update()
    {

        if (currentBuilding != null)
        {

            SaveBuildingCells();

            VisualizePlaceableness();

        }
     
        PlaceABuildingOnClick(currentBuilding, Camera.main.ScreenToWorldPoint(Input.mousePosition)); 

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {

            CleanCurrentBuilding();

        }   

    }

    public void ChangeCurrentBuilding(BuildingObject _building) 
    {

        CleanCurrentBuilding();

        currentBuilding = _building;

        GetIndicators();

    }


    public void CleanCurrentBuilding()
    {

        currentBuilding = null;

        foreach (var item in activeGreenIndicators)
        {
            poolingManager.DespawnObject(item);
        }

        foreach (var item in activeRedIndicators)
        {
            poolingManager.DespawnObject(item);
        }

        activeGreenIndicators.Clear();
        activeRedIndicators.Clear();
        buildingAreaCells.Clear();


    }

    private void GetIndicators() 
    {

        for (int i = 0; i < currentBuilding.sizeX * currentBuilding.sizeY; i++)
        {

            activeGreenIndicators.Add(poolingManager.spawnObject("greencell", Vector3.zero, Quaternion.identity));
            activeRedIndicators.Add(poolingManager.spawnObject("redcell", Vector3.zero, Quaternion.identity));

        }
    
    }
    public void PlaceABuildingOnClick(BuildingObject _building,Vector2 worldPosition) 
    {

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (currentBuilding == null) //Return if there are no current buildings
                return;

            //Error if you cant place

            if (EventSystem.current.IsPointerOverGameObject()) //Return if mouse is hovering on the UI
                return;

            if (buildingAreaCells.Count < currentBuilding.sizeX * currentBuilding.sizeY) //If the cell list is smaller than the building size
            {

                //Throw an error here
                return;

            }

            //Put the object

            Cell cellToPlace = gridReference.WorldPositionToCell(worldPosition);

            Vector2 spawnPosition = cellToPlace.worldPosition - new Vector2(GridSystem.Grid.cellDiameter / 2, GridSystem.Grid.cellDiameter / 2); //Remove cellDiameter / 2 to get the corner of the cell

            GameObject spawnedBuilding = poolingManager.spawnObject(_building.prefabPoolString, spawnPosition, Quaternion.identity); //Spawn object

            IStaticUnit staticUnit = spawnedBuilding.GetComponent<IStaticUnit>(); //Get the static unit interface

            staticUnit.SetSpawnPoint(worldPosition + Vector2.down ); //Set the spawn position for units 

            staticUnit.SetUniquePoint(worldPosition + Vector2.down ); //Set the spawn position for units 

            foreach (var cell in buildingAreaCells) //Place walls and set static units
            {
                cell.PlaceStaticUnit(staticUnit); //Set the static unit in cell
            }


            CleanCurrentBuilding();

        }

    }

    public void VisualizePlaceableness() 
    {


        if (currentBuilding == null)
            return;



        for (int x = 0,i = 0; x < currentBuilding.sizeX; x++)
        {

            for (int y = 0; y < currentBuilding.sizeY; y++)
            {
          
                Cell firstCell = gridReference.WorldPositionToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                if(firstCell.gridX + x >= gridReference.GridSizeX || firstCell.gridY + y >= gridReference.GridSizeY) 
                {

                    activeRedIndicators[i].transform.position = firstCell.worldPosition + new Vector2(GridSystem.Grid.cellDiameter * x, GridSystem.Grid.cellDiameter * y);

                    activeGreenIndicators[i].transform.position = new Vector3(0, 0, -1000);

                }
                else 
                {

                    Cell chosenCell = gridReference.GridPositionToCell(firstCell.gridX + x, firstCell.gridY + y);

                    if (chosenCell.terrainIndex != 1)
                    {

                        activeGreenIndicators[i].transform.position = chosenCell.worldPosition;

                        activeRedIndicators[i].transform.position = new Vector3(0, 0, -1000);

                    }
                    else
                    {

                        activeRedIndicators[i].transform.position = chosenCell.worldPosition;

                        activeGreenIndicators[i].transform.position = new Vector3(0, 0, -1000);

                    }

                }
                i++;
            }
       
        }

    }

    private void SaveBuildingCells() 
    {

        buildingAreaCells = GetBuildingCellsWithMousePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

    
    }
    List<Cell> GetBuildingCellsWithMousePosition(Vector2 _worldPosition)
    {

        List<Cell> finalCellList = new List<Cell>();

        Cell firstCell = gridReference.WorldPositionToCell(_worldPosition);

        for (int y = 0; y < currentBuilding.sizeY; y++)
        {
            for (int x = 0; x < currentBuilding.sizeX; x++)
            {


                if(firstCell.gridX + x < gridReference.GridSizeX && firstCell.gridY + y < gridReference.GridSizeY) 
                {
                    Cell _cell = gridReference.GridPositionToCell(firstCell.gridX + x, firstCell.gridY + y);

                    if(_cell.terrainIndex != 1) 
                    {

                        finalCellList.Add(_cell);

                    }

                }


            }
        }

        return finalCellList;
    
    }

    public void SelectBuilding(int index) 
    {

        ChangeCurrentBuilding(buildingObjects[index]);

    
    }
    

}
