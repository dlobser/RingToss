using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    public bool randomizeSeed = false;
    public void DoReset(){
        if(randomizeSeed)
            GameManager.Instance.RandomizeSeed();
        GameManager.Instance.levelGenerator.GenerateLevel();
    }
}
