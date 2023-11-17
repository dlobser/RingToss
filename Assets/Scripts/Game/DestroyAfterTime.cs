using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float time = 1.0f;
    public void DestroyAfterTimeFunction(float time)
    {
        Destroy(this.gameObject, time);
    }
    void Start()
    {
        DestroyAfterTimeFunction(time);
    }

}
