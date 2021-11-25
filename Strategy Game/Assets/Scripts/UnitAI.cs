using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public abstract class UnitBaseBehaviour 
{

    public abstract void StartBehaviour();

    public abstract void UpdateBehaviour();

    public abstract void FixedUpdateBehaviour();


}
