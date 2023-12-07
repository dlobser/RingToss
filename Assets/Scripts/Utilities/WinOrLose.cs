using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinOrLose : MonoBehaviour
{
    public UnityEvent OnAnimationComplete;
    public float animationTime;
    float counter;
    public GameObject sprite;
    SpriteRenderer thisSprite;
    Color spriteColor;
    bool animating = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(animating){
            if(thisSprite==null){
                GameObject g = Instantiate(sprite, GameManager.Instance.rootParent.transform);
                thisSprite = g.GetComponent<SpriteRenderer>();
                thisSprite.sprite = ImageLoader.Instance.GetSpriteWithIndex("Platform", GlobalSettings.ImageIndeces.Platform);
                spriteColor = thisSprite.color;
                thisSprite.color = new Color(spriteColor.r,spriteColor.g,spriteColor.b,0);
            }
            counter+=Time.deltaTime;
            if(counter<1){
                thisSprite.color = new Color(spriteColor.r,spriteColor.g,spriteColor.b,counter);
            }
            if(counter>animationTime){
                OnAnimationComplete?.Invoke();
                animating = false;
            }
        }
    }

    public void StartAnimating(){
        counter = 0;

        Invoke("Announce",1);
        animating = true;
    }

    public void Announce(){
        GameScoreKeeperLimitedProjectiles scoreKeeper = (GameScoreKeeperLimitedProjectiles)GameManager.Instance.gameScoreKeeper;

        if(scoreKeeper.items!=0)
            GameManager.Instance.SendAnnouncement("LOSE!");
        else
            GameManager.Instance.SendAnnouncement("WIN!");
    }
}
