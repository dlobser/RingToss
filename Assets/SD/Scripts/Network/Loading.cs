using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour
{
    int counter = 0;

    void Update()
    {
        if (++counter % 15 == 0) transform.Rotate(Vector3.forward, 15);
    }
}
