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

    private MenuManager menuManager;
    private GameObject playerController;

    public virtual void GenerateLevel()
    {

        if (levelSettings.gameManager.root != null)
            Destroy(levelSettings.gameManager.root);

        if (root != null)
        {
            Destroy(root);
        }

        root = new GameObject("Root");
        root.transform.SetParent(levelSettings.gameManager.rootParent.transform);

        menuManager = Instantiate(levelSettings.menuManager, root.transform);
        playerController = Instantiate(levelSettings.playerController, root.transform);

        // if (levelSettings.gameManager == null)
        //     levelSettings.gameManager = FindObjectOfType<GameManager>();
        // if (levelSettings.menuManagerName.Length > 0)
        //     levelSettings.gameManager.ShowMenu(levelSettings.menuManagerName);
        levelSettings.gameManager.root = root;
        // return root;
    }

    public virtual void OnGenerateLevelComplete()
    {
        levelSettings.scoreKeeper.OnLevelStart();
        menuManager.ShowMenu("Title");
    }

    public virtual void Destroy()
    {

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
