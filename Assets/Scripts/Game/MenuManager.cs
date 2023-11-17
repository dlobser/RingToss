using UnityEngine;
using System;

public class MenuManager : MonoBehaviour
{
    // Holds a reference to all the possible menus
    [System.Serializable]
    public struct GameMenu
    {
        public string name;
        public GameScreen menuObject;
    }

    // Array of all the menus
    public GameMenu[] menus;

    // Function to activate a specific menu by name
    public void ShowMenu(string menuName)
    {
        foreach (var menu in menus)
        {
            if (menu.name == menuName)
            {
                menu.menuObject.Activate();
            }
            else
            {
                menu.menuObject.Deactivate();
            }
        }
        print("Show Menu: " + menuName);
    }

    public void SetSprite(string name, Sprite sprite)
    {
        //de nada
    }

    public void ModifyMenu(string menuName, Action<GameMenu> operation)
    {
        foreach (var menu in menus)
        {
            if (menu.name == menuName)
            {
                operation?.Invoke(menu);
            }
        }
    }


    // Call this to deactivate all menus
    public void HideAllMenus()
    {
        foreach (var menu in menus)
        {
            menu.menuObject.Activate();
        }
    }
}
