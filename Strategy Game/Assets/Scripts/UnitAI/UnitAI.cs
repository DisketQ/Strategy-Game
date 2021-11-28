using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour,IDynamicUnit,IPoolable
{
    public List<Cell> path { get; set; } //Path t follow

    //References

    private GridSystem.Grid gridReference;
    public PathRequestingManager pathRequestManager { get; private set; }

    //States

    public SoldierUnitBaseState currentState { get; private set; } //Current state of the Unit
    public SoldierUnitIdleState idleState { get; private set; } //Idling state
    public SoldierUnitWalkState walkState { get; private set; } //Walking state

    //Serialize

   [SerializeField] private int AttackRange; 
   [SerializeField] private float WalkSpeed;
   [SerializeField] private float MaxHP;

    //Only gets

    public int attackRange { get; private set; }
    public float walkSpeed { get; private set; }
    public float maxHP { get; private set; }

    public Cell currentCell { get; private set; }

    private void Awake()
    {
        //Move from serializables to private sets

        attackRange = AttackRange;
        walkSpeed = WalkSpeed;
        maxHP = MaxHP;

        //Instance states

        idleState = new SoldierUnitIdleState();
        walkState = new SoldierUnitWalkState();

        //Assign the start state

        currentState = idleState;

        //References

        if (gridReference == null) 
            gridReference = FindObjectOfType<GridSystem.Grid>().GetComponent<GridSystem.Grid>();

        if (pathRequestManager == null)
            pathRequestManager = FindObjectOfType<PathRequestingManager>();
    }
    private void Start()
    {


        SpawnBehaviour();

    }

    private void Update()
    {

        currentState.UpdateBehaviour(this); //Call the update behaviour every frame

    }

    public void GoToPosition(Vector2 targetPosition) 
    {

        pathRequestManager.TryPath(this, transform.position, targetPosition); //Request for the path

    }

    public void SwitchState(SoldierUnitBaseState _state) //Switch to another state
    {

        currentState = _state; //Change the state

        _state.StartBehaviour(this); //And execute start behaviour of the new state
    
    }


    public void GetPath(List<Cell> _path) //Get the path and move to walking state
    {

        Debug.Log("IS THERE?");
        if (_path == null) //Return if there is no path
            return;

        path = _path; //Assign the path

        if(currentState != walkState) 
        {

            SwitchState(walkState); //Switch to the walking state

        }

        Debug.Log("THERE IS");

    }

    public void ChangeCurrentCell(Cell newCell) 
    {

        if(currentCell != null)
            currentCell.RemoveDynamicUnit(this); //Remove this object from cell's dynamicUnit list

        currentCell = newCell; //Change the current cell

        currentCell.PlaceDynamicUnit(this); //Place the this dynamic unit to new cell

    }

    //IPoolable

    public void FirstSpawnBehaviour()
    {
        
    }

    public void SpawnBehaviour()
    {

        ChangeCurrentCell(gridReference.WorldPositionToCell(transform.position)); //Set current cell for the first time
      
    }

    public void DespawnBehaviour()
    {
        
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}

public abstract class SoldierUnitBaseState
{

    public abstract void StartBehaviour(UnitAI _manager);

    public abstract void UpdateBehaviour(UnitAI _manager);

    public abstract void FixedUpdateBehaviour(UnitAI _manager);


}

public class SoldierUnitIdleState : SoldierUnitBaseState
{
    public override void StartBehaviour(UnitAI _manager)
    {
       
    }

    public override void FixedUpdateBehaviour(UnitAI _manager)
    {
        
    }

    public override void UpdateBehaviour(UnitAI _manager)
    {
        
    }
}

public class SoldierUnitWalkState : SoldierUnitBaseState
{
    public override void StartBehaviour(UnitAI _manager)
    {
        
    }

    public override void FixedUpdateBehaviour(UnitAI _manager)
    {
       
    }

    public override void UpdateBehaviour(UnitAI _manager)
    {
        TryWalkingThePath(_manager);

        CheckForEnemy();
    }

    private bool TryWalkingThePath(UnitAI _manager) //Returns false if there is no path left to walk
    {
        if (_manager.path.Count <= 0) //If there is no path
        {

            _manager.SwitchState(_manager.idleState); //Return to idle state

            return false; //And return false
        }

        Cell cellToWalk = _manager.path[0]; //Take the first path

        //Check if path is still available

        if (cellToWalk.terrainIndex == 1) //If path is blocked
        {

            _manager.SwitchState(_manager.idleState); //Return the idle state

            if(_manager.path[_manager.path.Count - 1].terrainIndex != 1)
            _manager.pathRequestManager.TryPath(_manager, _manager.transform.position, cellToWalk.worldPosition);  //Request a path again

            return false;
        
        }

        Vector2 walkingDirection = cellToWalk.worldPosition - (Vector2)_manager.transform.position; //Take the direction to Cell from Unit
    

        if (walkingDirection.magnitude < 0.01f * GridSystem.Grid.cellDiameter) //If is close enough to the cell
        {
                _manager.ChangeCurrentCell(_manager.path[0]); //Change the current cell

                _manager.path.RemoveAt(0); //Remove the reached path node 
        }

        WalkToThePosition(_manager.transform, cellToWalk.worldPosition, _manager.walkSpeed); //Walk to the position

        return true; //Return true if there is still a path

    }

    private void WalkToThePosition(Transform _transform,Vector2 _positionToWalk,float _walkingSpeed)
    {

        _transform.position += ((Vector3)_positionToWalk - _transform.position).normalized * _walkingSpeed * Time.deltaTime; //Walk through the direction


    }

    private void CheckForEnemy() 
    { 
    
    
    }

}
