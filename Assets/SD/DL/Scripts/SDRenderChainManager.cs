using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDRenderChainManager : MonoBehaviour
{
    public SDRenderChainLink[] links;
    public Request request;
    // Start is called before the first frame update
    void Start()
    {
        request = FindObjectOfType<Request>();
        StartCoroutine(Run(0));
    }

    IEnumerator Run(int which)
    {
        // links[which].RunUnityFunction();
        yield return null;
    }
}
