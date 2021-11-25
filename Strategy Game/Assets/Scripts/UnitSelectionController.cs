using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionController : MonoBehaviour
{

    LineRenderer lineRenderer;

    private Vector2 firstPosition;
    private Vector2 secondPosition;

    // Start is called before the first frame update
    void Start()
    {

        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 5;

    }

    // Update is called once per frame
    void Update()
    {

        GetMousePositions();

        DrawRectangle(firstPosition,secondPosition);

    }

    private void GetMousePositions() 
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            firstPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }

        if (Input.GetKey(KeyCode.Mouse0))
        {

            secondPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {

            firstPosition = new Vector2(0, -100);

            secondPosition = new Vector2(0, -100);

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
