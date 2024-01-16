using UnityEngine;
using System.Collections;
namespace Quilt
{
    public class GameGeneratorManager : MonoBehaviour
    {

        private GameGenerator currentGameGenerator;

        public GameGenerator[] gameGenerators;

        private Transform rootParent;

        public Transform RootParent
        {
            get
            {
                return rootParent;
            }
        }

        string rootParentName = "Root Parent";

        public void Start()
        {
            if(rootParent == null)
            {
                rootParent = new GameObject(rootParentName).transform;
                rootParent.transform.parent = this.transform;
            }
            Globals.GlobalSettings.LevelGlobals.rootParent = rootParent;
            BuildGame();
        }

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

            yield return DestroyGameCoroutine();
            currentGameGenerator = Instantiate(gameGenerators[0], Globals.GetRootParent());
            currentGameGenerator.InitializeGame();
            currentGameGenerator.StartGame();
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space)){
                BuildGame();
            }
        }

        public void DestroyGame()
        {
            StartCoroutine(DestroyGameCoroutine());
        }

        IEnumerator DestroyGameCoroutine(){
            if (currentGameGenerator != null)
            {
                currentGameGenerator.StopGame();
                Destroy(currentGameGenerator.gameObject);
            }
            yield return null;
        }
    }
}