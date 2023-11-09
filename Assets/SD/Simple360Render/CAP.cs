using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CAP : MonoBehaviour
{
    public bool doit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(doit){
            byte[] b =  I360Render.Capture(  );
            File.WriteAllBytes("Assets/BOB.png",b);
            doit = false;
        }
    }
}
