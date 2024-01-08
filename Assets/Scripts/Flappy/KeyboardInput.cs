using System.Collections;
using System.Collections.Generic;
using Quilt;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.D))
            FindObjectOfType<GameGeneratorManager>().DestroyGame();
        if (Input.GetKeyUp(KeyCode.S))
            FindObjectOfType<GameGeneratorManager>().BuildGame();


    }
}
