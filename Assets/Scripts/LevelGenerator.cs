using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    [HideInInspector]
    public GameObject root;
    public GameObject rootParent;

    public virtual GameObject GenerateLevel()
    {
        root = new GameObject("Root");
        root.transform.SetParent(rootParent.transform);
        return root;
    }

    public virtual void Destroy(){
    
    }
}
