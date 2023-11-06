using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    [HideInInspector]
    public GameObject root;

    public virtual GameObject GenerateLevel()
    {
        root = new GameObject("Root"); 
        return root;
    }

    public virtual void Destroy(){

    }
}
