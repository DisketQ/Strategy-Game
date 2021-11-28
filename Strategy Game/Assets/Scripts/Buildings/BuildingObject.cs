using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Building" ,menuName ="RTSItems/Building")]
public class BuildingObject : ScriptableObject
{
    public int sizeX;
    public int sizeY;
    public string prefabPoolString;
}
