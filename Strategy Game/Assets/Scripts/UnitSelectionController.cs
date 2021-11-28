using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using UnityEngine.EventSystems;
using TMPro;

public class UnitSelectionController : MonoBehaviour
{

    LineRenderer lineRenderer;
    GridSystem.Grid gridReference;

    //Visuals

    private Vector2 firstPositionRectangle;
    private Vector2 secondPositionRectangle;

    public TextMeshProUGUI unitCountText;
    //Logic

    private Vector2 firstPositionSelection;
    private Vector2 secondPositionSelection;

    private List<IDynamicUnit> selectedUnitsList = new List<IDynamicUnit>();

    // Start is called before the first frame update
    void Start()
    {

        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 5;

        gridReference = FindObjectOfType<GridSystem.Grid>();

    }

    // Update is called once per frame
    void Update()
    {

        //Visuals

        GetMousePositionsForRectangle();

        DrawRectangle(firstPositionRectangle, secondPositionRectangle);

        //Logic

        GetMousePositionsForSelection();

        CollectDynamicUnits();

        MoveUnitsOnClick();


    }

    private void MoveUnitsOnClick() 
    {

        if (Input.GetKeyUp(KeyCode.Mouse1) && !EventSystem.current.IsPointerOverGameObject()) 
        {

            foreach (var unit in selectedUnitsList)
            {

                unit.GoToPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            }
        
        }
    
    }
  
    private void CollectDynamicUnits() 
    {

        if(Input.GetKeyUp(KeyCode.Mouse0))
        selectedUnitsList = ExtractUnitsFromCellList(GetEveryCellBetweenGridPositions(firstPositionSelection, secondPositionSelection));

    }
    private List<IDynamicUnit> ExtractUnitsFromCellList(List<Cell> cellList) 
    {

        List<IDynamicUnit> dynamicUnitList = new List<IDynamicUnit>();

        foreach (var cell in cellList)
        {

            if(cell.dynamicUnitList.Count > 0) 
            {

                foreach (var _dynamicUnit in cell.dynamicUnitList)
                {
                    dynamicUnitList.Add(_dynamicUnit);
                }

            }

        }

        unitCountText.text = dynamicUnitList.Count + " units selected"; //Change Text

        return dynamicUnitList;
    
    }
    private List<Cell> GetEveryCellBetweenGridPositions(Vector2 firstPos,Vector2 secondPos) 
    {

        List<Cell> finalList = new List<Cell>();

        Cell firstCell = gridReference.WorldPositionToCell(firstPos);
        Cell secondCell = gridReference.WorldPositionToCell(secondPos);

        int firstCellGridX = firstCell.gridX;
        int firstCellGridY = firstCell.gridY;
        int secondCellGridX = secondCell.gridX;
        int secondCellGridY = secondCell.gridY;


        int lowerGridX = 0;
        int lowerGridY = 0;
        int higherGridX = 0;
        int higherGridY = 0;

        if (firstCellGridX > secondCellGridX) 
        {
            higherGridX = Mathf.RoundToInt(firstCellGridX);
            lowerGridX = Mathf.RoundToInt(secondCellGridX);
        }
        else 
        {
            higherGridX = Mathf.RoundToInt(secondCellGridX);
            lowerGridX = Mathf.RoundToInt(firstCellGridX);
        }

        if (firstCellGridY > secondCellGridY)
        {
            higherGridY = Mathf.RoundToInt(firstCellGridY);
            lowerGridY = Mathf.RoundToInt(secondCellGridY);
        }
        else
        {
            higherGridY = Mathf.RoundToInt(secondCellGridY);
            lowerGridY = Mathf.RoundToInt(firstCellGridY);
        }

        int sizeX = higherGridX - lowerGridX;
        int sizeY = higherGridY - lowerGridY;

        for (int x = 0; x < sizeX + 1; x++)
        {
            for (int y = 0; y < sizeY + 1; y++)
            {
                finalList.Add(gridReference.GridPositionToCell(lowerGridX + x, lowerGridY + y));
            }
        }

        return finalList;
    }
    private void GetMousePositionsForSelection()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            firstPositionSelection = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }


        if (Input.GetKeyUp(KeyCode.Mouse0))
        {


            secondPositionSelection = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }

    }
    private void GetMousePositionsForRectangle() 
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            firstPositionRectangle = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }

        if (Input.GetKey(KeyCode.Mouse0))
        {

            secondPositionRectangle = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {

            firstPositionRectangle = new Vector2(0, -1000);

            secondPositionRectangle = new Vector2(0, -1000);

        }

    }

    private void DrawRectangle(Vector2 _firstPos,Vector2 _secondPos) 
    {

        Vector3[] Vertices = new Vector3[5];

        Vertices[0] = new Vector3(_firstPos.x , _firstPos.y,-1);
        Vertices[1] = new Vector3(_secondPos.x, _firstPos.y,-1);
        Vertices[2] = new Vector3(_secondPos.x, _secondPos.y,-1);
        Vertices[3] = new Vector3(_firstPos.x, _secondPos.y,-1);
        Vertices[4] = new Vector3(_firstPos.x, _firstPos.y,-1);


        lineRenderer.SetPositions(Vertices);

    }
}
