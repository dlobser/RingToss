using MM.Msg;
using UnityEngine;

namespace Quilt
{
    public abstract class GameGenerator : MonoBehaviour
    {
        // Paths to prefabs in the Resources folder
        public string gameManagerPrefabPath;
        public string menuManagerPrefabPath;
        public string scoreManagerPrefabPath;
        public string interactionManagerPrefabPath;
        public string fxManagerPrefabPath;
        public string audioManagerPrefabPath;
        // public string transitionManagerPrefabPath;
        public string eventManagerPrefabPath;
        public string creationSettingsPath;

        // Instances of the prefabs
        protected GameObject gameManager;
        protected GameObject menuManager;
        protected GameObject scoreManager;
        protected GameObject interactionManager;
        protected GameObject fxManager;
        protected GameObject audioManager;
        // protected GameObject transitionManager;
        protected GameObject eventManager;
        protected CreationSettings creationSettings;

        public GameObject root { get; set; }
        public GameObject managersRoot { get; set; }
        public GameObject menuRoot { get; set; }

        public virtual void InitializeGame()
        {
            if (root != null)
            {
                Destroy(root);
            }
            if (root == null)
            {
                root = new GameObject("Root");
                root.transform.SetParent(GameGeneratorManager.Instance.rootParent);
                Globals.GlobalSettings.LevelGlobals.root = root.transform;

                managersRoot = new GameObject("Managers Root");
                managersRoot.transform.SetParent(root.transform);
                Globals.GlobalSettings.LevelGlobals.managersRoot = managersRoot.transform;

                menuRoot = new GameObject("Menu Root");
                menuRoot.transform.SetParent(root.transform);
                Globals.GlobalSettings.LevelGlobals.menuRoot = menuRoot.transform;
            }
            creationSettings = Resources.Load<CreationSettings>(creationSettingsPath);
            if (creationSettings == null)
            {
                Debug.LogError($"CreationSettings not found at path: {creationSettingsPath}");
            }

            InstantiatePrefabs();

            Globals.GlobalSettings.LevelGlobals.gameRoot = Globals.GetMenuManager().gameRoot.transform;
        }

        public virtual void StartGame() { /* ... implementation ... */ }

        public virtual void StopGame()
        {
            if (root != null)
            {
                Destroy(root);
            }
        }

        protected virtual void InstantiatePrefabs()
        {
            GameObject eventManagerObj = InstantiatePrefab(eventManagerPrefabPath, managersRoot.transform);
            Globals.GlobalSettings.Managers.eventManager = eventManagerObj.GetComponent<EventManager>();
            print(Globals.GlobalSettings.Managers.eventManager);

            GameObject gameManagerObj = InstantiatePrefab(gameManagerPrefabPath, managersRoot.transform);
            Globals.GlobalSettings.Managers.gameManager = gameManagerObj.GetComponent<GameManager>();

            GameObject menuManagerObj = InstantiatePrefab(menuManagerPrefabPath, managersRoot.transform);
            Globals.GlobalSettings.Managers.menuManager = menuManagerObj.GetComponent<MenuManager>();

            GameObject scoreManagerObj = InstantiatePrefab(scoreManagerPrefabPath, managersRoot.transform);
            Globals.GlobalSettings.Managers.scoreManager = scoreManagerObj.GetComponent<ScoreManager>();

            GameObject interactionManagerObj = InstantiatePrefab(interactionManagerPrefabPath, managersRoot.transform);
            Globals.GlobalSettings.Managers.interactionManager = interactionManagerObj.GetComponent<InteractionManager>();

            GameObject fxManagerObj = InstantiatePrefab(fxManagerPrefabPath, managersRoot.transform);
            Globals.GlobalSettings.Managers.fxManager = fxManagerObj.GetComponent<FXManager>();

            GameObject audioManagerObj = InstantiatePrefab(audioManagerPrefabPath, managersRoot.transform);
            Globals.GlobalSettings.Managers.audioManager = audioManagerObj.GetComponent<AudioManager>();

        }

        private GameObject InstantiatePrefab(string path, Transform parent)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab != null)
            {
                return Instantiate(prefab, parent);
            }
            else
            {
                Debug.LogError($"Prefab not found at path: {path}");
                return null;
            }
        }
    }
}
