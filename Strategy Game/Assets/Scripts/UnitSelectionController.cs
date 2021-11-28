using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionController : MonoBehaviour
{

    LineRenderer lineRenderer;


    //Visuals

    private Vector2 firstPositionRectangle;
    private Vector2 secondPositionRectangle;

    //Logic

    private Vector2 firstPositionSelection;
    private Vector2 secondPositionSelection;

    // Start is called before the first frame update
    void Start()
    {

        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 5;

    }

    // Update is called once per frame
    void Update()
    {

        GetMousePositionsForRectangle();

        GetMousePositionsForSelection();

        DrawRectangle(firstPositionRectangle, secondPositionRectangle);

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
