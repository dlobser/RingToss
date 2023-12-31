using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [System.Serializable]
    public class LevelSettings
    {
        public GameScoreKeeper scoreKeeper;
        public GameObject playerController;
        public GameManager gameManager;
        public MenuManager menuManager;
        // public string menuManagerName;
    }

    [HideInInspector]
    public GameObject root;
    // public GameObject rootParent;
    public LevelSettings levelSettings;

    [HideInInspector]
    public MenuManager menuManager;
    private GameObject playerController;

    public virtual void GenerateLevel()
    {

        // if (levelSettings.gameManager.root != null)
        //     Destroy(levelSettings.gameManager.root);

        if(GameManager.Instance.rootParent!=null){
            if (GameManager.Instance.rootParent.transform.childCount > 0)
            {
                List<GameObject> children = new List<GameObject>();

                for (int i = 0; i < GameManager.Instance.rootParent.transform.childCount; i++)
                {
                    children.Add(GameManager.Instance.rootParent.transform.GetChild(i).gameObject);
                }

                foreach (GameObject child in children)
                {
                    Destroy(child);
                }
            }
        }

        root = new GameObject("Root");
        root.transform.SetParent(levelSettings.gameManager.rootParent.transform);
        menuManager = Instantiate(levelSettings.menuManager, root.transform);
        GameObject menuRoot = FindObjectOfType<GameParent>().gameObject;
        GameManager.Instance.root = menuRoot;
        root = menuRoot;
        playerController = Instantiate(levelSettings.playerController, GameManager.Instance.root.transform);
        levelSettings.gameManager.root = root;

    }

    public virtual void OnGenerateLevelComplete()
    {
        levelSettings.scoreKeeper.OnLevelStart();
        menuManager.ShowMenu("Title");
    }

    public virtual void Destroy()
    {

    }

    public virtual GameObject GeneratePlatformPositions()
    {
        return null;
    }

    public virtual void SetRandomPhysics()
    {
        float energy = Random.Range(.5f, 5f);
        GlobalSettings.Physics.ballSpeed = Random.Range(1, 3) * energy;
        GlobalSettings.Physics.ballGravity = energy * .5f;
        GlobalSettings.Physics.platformBounce = Random.Range(.2f, .7f);
        GlobalSettings.Physics.ballSize = Random.Range(.4f, .8f);
    }

    public virtual void SetRandomStyle()
    {
        GlobalSettings.ImageIndeces.Style = int.Parse(ImageLoader.Instance.styleNum);
        GlobalSettings.ImageIndeces.BG = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Background");
        GlobalSettings.ImageIndeces.Platform = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Platform");
        GlobalSettings.ImageIndeces.Title = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Title");
        GlobalSettings.ImageIndeces.Projectile = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Projectile");
    }

}
