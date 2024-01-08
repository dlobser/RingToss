using UnityEngine;
using System.Collections;
namespace Quilt
{
    public class GameGeneratorManager : MonoBehaviour
    {
        // Singleton instance
        // public static GameGeneratorManager Instance { get; private set; }

        private GameGenerator currentGameGenerator;

        public GameGenerator[] gameGenerators;

        public Transform rootParent;
        string rootParentName = "Root Parent";

        // void Awake()
        // {
        //     // Singleton pattern to ensure only one instance exists
        //     if (Instance == null)
        //     {
        //         Instance = this;
        //         // DontDestroyOnLoad(gameObject);
        //     }
        //     else
        //     {
        //         Destroy(gameObject);
        //     }
        // }

        public void Start()
        {
            Globals.GlobalSettings.LevelGlobals.rootParent = rootParent;
            BuildGame();
        }

        // Method to start a game with a specific generator
        public void BuildGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        IEnumerator StartGameCoroutine()
        {
            if (gameGenerators == null || gameGenerators.Length == 0)
            {
                Debug.LogError("GameGenerator array is not set up correctly.");
                yield break;
            }

            DestroyGame();

            // Debug.Log("current game generator: " + currentGameGenerator);

            // if (currentGameGenerator != null)
            // {
            //     Destroy(currentGameGenerator);
            // }

            // // yield return null;

            // if (rootParent.transform.childCount > 0)
            // {
            //     foreach (Transform child in rootParent.transform)
            //     {
            //         Destroy(child.gameObject);
            //     }
            // }

            // yield return null;
            Debug.Log("GGG: " + currentGameGenerator);

            currentGameGenerator = Instantiate(gameGenerators[0], Globals.GetRootParent());
            yield return null;
            currentGameGenerator.InitializeGame();
            // yield return null;
            currentGameGenerator.StartGame();
        }

        // Method to stop the current game
        public void DestroyGame()
        {
            if (currentGameGenerator != null)
            {
                currentGameGenerator.StopGame();
                Destroy(currentGameGenerator.gameObject);
                currentGameGenerator = null;
            }
        }
    }
}