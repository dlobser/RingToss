using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : MonoBehaviour
{
    public GameObject root;

    public virtual void Activate()
    {
        root.SetActive(true);
    }


    public virtual void Deactivate()
    {
        root.SetActive(false);
    }
}
