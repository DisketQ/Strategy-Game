using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolSpawner : MonoBehaviour
{

    [SerializeField] private List<Pool> poolList = new List<Pool>(); //List of serializable pools

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    [SerializeField] private Vector3 deadZonePosition; //Dead zone to move the non active objects

    [SerializeField] private Transform garbageCollector; //Object to put spawned objects

    private void Awake()
    {

        for (int i = 0; i < poolList.Count; i++)
        {
            AddPoolToDictionary(poolList[i]); //Move pools from serializable pool to dictionary
        }

    }

    public void AddPoolToDictionary(Pool _pool)
    {

        poolDictionary.Add(_pool.tag, new Queue<GameObject>()); //Move it to the dictionary

        PoolSpawn(_pool); //Spawn the pool items

    }

    public void AddPoolToList(Pool givenPool)
    {

        poolList.Add(givenPool);

    }

    public void PoolSpawn(Pool givenPool)
    {

        for (int i = 0; i < givenPool.size; i++)
        {

            GameObject instantiatedGameObject = Instantiate(givenPool.prefab); //Instantiate the object

            instantiatedGameObject.transform.SetParent(garbageCollector); //Set parent

            poolDictionary[givenPool.tag].Enqueue(instantiatedGameObject); //Add it to the dictionary

            instantiatedGameObject.SetActive(false); //Deactivate object


            //First Spawn Behaviour

            IPoolable poolable = instantiatedGameObject.GetComponent<IPoolable>();

            if (poolable != null) 
            {


                poolable.FirstSpawnBehaviour(); //Execute the first spawn behaviour of IPoolable

              
            }

     

        }

    }
    public GameObject spawnObject(string poolTag, Vector3 position , Quaternion rotation)
    {

        if (poolDictionary[poolTag].Peek().activeSelf) 
        {

            //Not enough objects in pool 
            //Spawn more here

            return null;
        
        }

        GameObject spawnedObject = poolDictionary[poolTag].Dequeue();  //Get one item from the queue

        spawnedObject.transform.position = position; //Set the position

        spawnedObject.transform.localRotation = rotation; //Rotation

        poolDictionary[poolTag].Enqueue(spawnedObject); //And put it back to queue


        //Activate if it is inactive

        if (!spawnedObject.activeSelf)
            spawnedObject.SetActive(true);

        return spawnedObject;  //Returned the spawned gameobject

    }



    public void DespawnObject(GameObject givenObject)
    {

        //IPoolable behaviour

        IPoolable givenPoolable = givenObject.GetComponent<IPoolable>();

       

        //

        givenObject.SetActive(false);

        givenObject.transform.position = deadZonePosition;


    }



}

public interface IPoolable
{

    void FirstSpawnBehaviour();
    void SpawnBehaviour();
    void DespawnBehaviour();

    GameObject GetGameObject();


}

[System.Serializable]
public class Pool
{
    
    public string tag;
    public int size;
    public GameObject prefab;

    public Pool(string _tag, GameObject _prefab, int _size)
    {
        size = _size;
        tag = _tag;
        prefab = _prefab;
    }


}
