using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using UnityEngine.EventSystems;
public class BuildingSelector : MonoBehaviour
{

    [SerializeField] List<GameObject> buildingUIList = new List<GameObject>();
    private IStaticUnit currentStaticUnit;
    private int currentUIIndex;

    //References
    private GridSystem.Grid gridReference;
    private UnitFactory unitSpawner;
    private void Awake()
    {
        gridReference = FindObjectOfType<GridSystem.Grid>();
        unitSpawner = FindObjectOfType<UnitFactory>();

    }

    // Update is called once per frame
    void Update()
    {

        ChangeSelectedUnitOnClick();

        ClearUIOnClick();

        
    }

    private void ClearUIOnClick() 
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            buildingUIList[currentUIIndex].SetActive(false);

        }

    }
    private void ChangeSelectedUnitOnClick() 
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            Vector2 gridIndex = gridReference.WorldPositionToGridArray(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            IStaticUnit selectedStaticUnit = gridReference.GridPositionToCell(Mathf.RoundToInt(gridIndex.x), Mathf.RoundToInt(gridIndex.y)).staticUnit;

            if (selectedStaticUnit != null)
            {

                currentStaticUnit = selectedStaticUnit;

                //UI

                ChangeUI(selectedStaticUnit.GetUIIndex());

            }

        }
  

    }

    private void ChangeUI(int indexUI) 
    {
        if (currentUIIndex >= 0)
            buildingUIList[currentUIIndex].SetActive(false); //Disable the current UI

        currentUIIndex = indexUI; //Change the current UI

        buildingUIList[indexUI].SetActive(true); //Activate the new UI

    }

    public void StartSetUniquePointOnCurrentStaticUnit() //Coroutine starter
    {
        StopCoroutine("SetUniquePointOnCurrentStaticUnit");
        
        StartCoroutine(SetUniquePointOnCurrentStaticUnit());

    }
    public IEnumerator SetUniquePointOnCurrentStaticUnit() 
    {
        

        yield return new WaitUntil(() => { return Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Escape); });

        if (!EventSystem.current.IsPointerOverGameObject()) 
        {

            currentStaticUnit.SetUniquePoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Debug.Log("ENDED");

        }

 
    
    }

    public void SpawnUnitFromCurrentBuilding(int unitIndex) //Spawn the item with given index on the building
    {

       GameObject spawnedUnit = unitSpawner.SpawnUnitWithIndex(unitIndex, currentStaticUnit.GetSpawnPoint()); //Spawn the unit

        spawnedUnit.GetComponent<UnitAI>().GoToPosition(currentStaticUnit.GetUniquePoint()); //Move to spawn target
    
    }
}
