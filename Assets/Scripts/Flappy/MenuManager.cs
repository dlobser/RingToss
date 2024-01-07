using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Quilt
{

    public class MenuManager : MonoBehaviour
    {
        [System.Serializable]
        public struct GameMenu
        {
            public string name;
            public GameScreen menuObject;
        }

        public int currentMenu;
        public float fadeSpeed;
        public GameMenu[] menus;
        public GameObject menuRoot;
        public GameObject gameRoot;

        private Coroutine currentFadeCoroutine;

        public void ShowMenu(string menuName)
        {
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
                menus[currentMenu].menuObject.Deactivate(); // Immediately deactivate current menu
            }

            currentFadeCoroutine = StartCoroutine(Fade(menus[currentMenu].menuObject, true));

            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].name == menuName)
                {
                    currentMenu = i;
                    currentFadeCoroutine = StartCoroutine(Fade(menus[i].menuObject, false));
                    break;
                }
            }
        }

        IEnumerator Fade(GameScreen menuObject, bool down = true)
        {
            float counter = 0;
            if (!down) menuObject.Activate();

            while (counter < fadeSpeed)
            {
                counter += Time.deltaTime;
                menuObject.Fade(down ? 1 - (counter / fadeSpeed) : (counter / fadeSpeed));
                yield return null;
            }

            if (down) menuObject.Deactivate();
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
                menu.menuObject.Deactivate();
            }
        }
    }


}
