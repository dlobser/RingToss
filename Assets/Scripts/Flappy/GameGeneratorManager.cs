using UnityEngine;

namespace Quilt{
    public class GameGeneratorManager : MonoBehaviour
    {
        // Singleton instance
        public static GameGeneratorManager Instance { get; private set; }

        private GameGenerator currentGameGenerator;

        public GameGenerator[] gameGenerators;

        public Transform rootParent;

        void Awake()
        {
            // Singleton pattern to ensure only one instance exists
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Start(){
            StartGame();
        }

        // Method to start a game with a specific generator
        public void StartGame()
        {
            if (gameGenerators == null || gameGenerators.Length == 0)
            {
                Debug.LogError("GameGenerator array is not set up correctly.");
                return;
            }

            if (currentGameGenerator != null)
            {
                StopGame();
            }

            currentGameGenerator = Instantiate(gameGenerators[0], Globals.GetRootParent());
            currentGameGenerator.InitializeGame();
            currentGameGenerator.StartGame();
        }

        // Method to stop the current game
        public void StopGame()
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