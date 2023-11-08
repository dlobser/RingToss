using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerManager : MonoBehaviour
{
    public GameObject[] controllers;
    public static PlayerControllerManager Instance;

    public void SetActiveController(GameObject controller){
        foreach(GameObject g in controllers){
            g.SetActive(false);
        }
        controller.SetActive(true);
        print(controller.name + " CONTROLLER");
    }

    private void Awake()
    {
        Instance = this;
    }

}
