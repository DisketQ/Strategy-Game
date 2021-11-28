using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceHub : MonoBehaviour
{

    // singleton static reference
    private static ReferenceHub _Instance;
    public static ReferenceHub Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<ReferenceHub>();
                if (_Instance == null)
                    Debug.LogError("There is no ReferenceHub in the scene!");
            }
            return _Instance;
        }
    }


    [SerializeField]
    private Canvas _mainCanvas;
    public Canvas MainCanvas { get { return _mainCanvas; } }

}