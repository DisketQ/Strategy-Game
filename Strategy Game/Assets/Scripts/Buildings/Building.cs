using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;

public class Building : MonoBehaviour, IStaticUnit
{

    [SerializeField] int UIindex;
    float currentHealth;

    private Vector2 spawnTarget;
    private Vector2 spawnPoint;



    public void DamageUnit(float _damageAmount)
    {
        currentHealth -= _damageAmount;
    }

    public Vector2 GetSpawnPoint()
    {
        return spawnPoint;
    }

    public void SetSpawnPoint(Vector2 _point)
    {
        spawnPoint = _point;
    }
    public int GetUIIndex()
    {
        return UIindex;
    }

    public Vector2 GetUniquePoint()
    {
        return spawnTarget;
    }


    public void SetUniquePoint(Vector2 _point)
    {
        spawnTarget = _point;
    }
}
