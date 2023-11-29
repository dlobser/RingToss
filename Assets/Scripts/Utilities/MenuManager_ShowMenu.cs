using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager_ShowMenu : MonoBehaviour
{
    public string menuName;
        // Function to call ShowMenu on MenuManager with a given menu name
    public void TriggerShowMenu()
    {
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager != null)
        {
            Debug.Log("Show Menu: " + menuName);
            menuManager.ShowMenu(menuName);
        }
        else
        {
            Debug.LogError("MenuManager not found in the scene.");
        }
    }

}