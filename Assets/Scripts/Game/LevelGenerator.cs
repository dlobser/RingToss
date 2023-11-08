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
    }

    [HideInInspector]
    public GameObject root;
    public GameObject rootParent;

    public virtual GameObject GenerateLevel()
    {
        root = new GameObject("Root");
        root.transform.SetParent(rootParent.transform);
        return root;
    }

    public virtual void Destroy(){
    
    }

    public virtual void SetRandomPhysics()
    {
        float energy = Random.Range(.5f, 5f);
        GlobalSettings.Physics.ballSpeed = Random.Range(1, 3) * energy;
        GlobalSettings.Physics.ballGravity = energy * .5f;
        GlobalSettings.Physics.platformBounce = Random.Range(.2f,.7f);
        GlobalSettings.Physics.ballSize =  Random.Range(.4f,.8f);
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
