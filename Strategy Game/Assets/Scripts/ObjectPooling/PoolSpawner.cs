using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolSpawner : MonoBehaviour
{

    public List<Pool> poolList = new List<Pool>();

    private Dictionary<string, Queue<IPoolable>> poolDictionary = new Dictionary<string, Queue<IPoolable>>();

    public Vector3 deadZonePosition;

    public Transform garbageCollector;

    private void Awake()
    {

       

        for (int i = 0; i < poolList.Count; i++)
        {
            AddPoolToDictionary(poolList[i].tag, poolList[i].prefab, poolList[i].size);
        }

        Debug.Log(poolDictionary.Count);

    }

    public void AddPoolToDictionary(string _tag, GameObject _prefab, int _size)
    {

        Pool createdPool = new Pool(_tag, _prefab, _size);

        poolDictionary.Add(createdPool.tag, new Queue<IPoolable>());

        PoolSpawn(createdPool);

    }

    public void AddPoolToList(Pool givenPool)
    {

        poolList.Add(givenPool);

    }

    public void PoolSpawn(Pool givenPool)
    {

        for (int i = 0; i < givenPool.size; i++)
        {

            IPoolable instantiatedPoolable = Instantiate(givenPool.prefab).GetComponent<IPoolable>();

            instantiatedPoolable.GetGameObject().transform.SetParent(garbageCollector);

            poolDictionary[givenPool.tag].Enqueue(instantiatedPoolable);

            //First Spawn Behaviour

            instantiatedPoolable.isInPool = true; //Now is in pool

            instantiatedPoolable.FirstSpawnBehaviour();

            instantiatedPoolable.GetGameObject().SetActive(false);

        }

    }
    public GameObject spawnObject(string poolTag, Vector3 position, Quaternion rotation, bool executeSpawnBehaviour)
    {

        if (!poolDictionary[poolTag].Peek().isInPool) 
        {

            //Not enough objects in pool 
            //Spawn more here

            return null;
        
        }

        IPoolable spawnedPoolable = poolDictionary[poolTag].Dequeue(); 

        spawnedPoolable.GetGameObject().transform.position = position;

        spawnedPoolable.GetGameObject().transform.localRotation = rotation;

        poolDictionary[poolTag].Enqueue(spawnedPoolable);

        spawnedPoolable.isInPool = false; //Not in pool anymore

        //Activate if it is inactive

        if (!spawnedPoolable.GetGameObject().activeSelf)
            spawnedPoolable.GetGameObject().SetActive(true);

        //IPoolable behaviour

        if(executeSpawnBehaviour)
        spawnedPoolable.SpawnBehaviour();


        //Returned the spawned gameobject
        return spawnedPoolable.GetGameObject();


    }



    public void DespawnObject(GameObject givenObject)
    {

        //IPoolable behaviour

        IPoolable givenPoolable = givenObject.GetComponent<IPoolable>();

        if (givenPoolable != null) 
        {

            givenPoolable.isInPool = true; //Now is in pool

            givenPoolable.DespawnBehaviour();

        }
       

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

    bool isInPool { get; set; }

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
