using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    public void DoReset(){
        GameManager.Instance.RandomizeSeed();
        GameManager.Instance.levelGenerator.GenerateLevel();
    }
}
