using UnityEngine;

namespace Quilt
{
    public class GameGenerator : MonoBehaviour
    {
        public GameObject managersGameObject;
        // public GameObject _managersGameObject {get;set;}
        public CreationSettings creationSettings;

        public GameObject root { get; private set; }
        public GameObject managersRoot { get; private set; }
        public GameObject menuRoot { get; private set; }

        public virtual void InitializeGame()
        {
            SetUpRootObjects();
            AssignManagersFromGameObject();
        }

        public virtual void StartGame() { 
            GameManager.Instance.StartGame();
        }

        // public virtual void StopGame()
        // {
        //     // if (root != null)
        //     // {
        //     //     Destroy(root);
        //     // }
        // }

        protected void SetUpRootObjects()
        {
            if (root != null)
                Destroy(root);
            
            Resources.UnloadUnusedAssets();

            root = new GameObject("Root");
            root.transform.SetParent(Globals.GlobalSettings.LevelGlobals.rootParent);
            Globals.GlobalSettings.LevelGlobals.root = root.transform;

            managersRoot = new GameObject("Managers Root");
            managersRoot.transform.SetParent(root.transform);
            Globals.GlobalSettings.LevelGlobals.managersRoot = managersRoot.transform;

            menuRoot = new GameObject("Menu Root");
            menuRoot.transform.SetParent(root.transform);
            Globals.GlobalSettings.LevelGlobals.menuRoot = menuRoot.transform;
        }

        void Update()
        {
            // Debug.Log(_managersGameObject.name + " instantiated.");

        }

        protected void AssignManagersFromGameObject()
        {
            if (managersGameObject == null)
            {
                Debug.LogError("Managers GameObject not assigned.");
                return;
            }

            // if (_managersGameObject != null)
            //     Destroy(_managersGameObject);

            // _managersGameObject = Instantiate(managersGameObject, managersRoot.transform);

            Globals.GlobalSettings.Managers.eventManager = managersGameObject.GetComponent<EventManager>();
            Globals.GlobalSettings.Managers.gameManager = managersGameObject.GetComponent<GameManager>();
            Globals.GlobalSettings.Managers.menuManager = managersGameObject.GetComponent<MenuManager>();
            Globals.GlobalSettings.LevelGlobals.gameRoot = Globals.GetMenuManager().gameRoot.transform;
            Globals.GetMenuManager().menuRoot.transform.SetParent(Globals.GlobalSettings.LevelGlobals.menuRoot);
            Globals.GlobalSettings.Managers.scoreManager = managersGameObject.GetComponent<ScoreManager>();
            Globals.GlobalSettings.Managers.interactionManager = managersGameObject.GetComponent<InteractionManager>();
            Globals.GlobalSettings.Managers.fxManager = managersGameObject.GetComponent<FXManager>();
            Globals.GlobalSettings.Managers.audioManager = managersGameObject.GetComponent<AudioManager>();
            Globals.GlobalSettings.Managers.uiManager = managersGameObject.GetComponent<UIManager>();

            // Add other managers as needed
        }
    }
}
