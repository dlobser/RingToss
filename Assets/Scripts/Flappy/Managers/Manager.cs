using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt{
    public class Manager : MonoBehaviour
    {
        public GameManager gameManager {get;set;}

        public virtual void Init(GameManager manager){
            gameManager = manager;
        }

        public virtual void Init(){
            gameManager = FindObjectOfType<GameManager>();
        }
    }
}