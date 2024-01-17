using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt{
    public class Manager : MonoBehaviour
    {
        public GameManager gameManager;
        public virtual void Init(){
            gameManager = FindObjectOfType<GameManager>();
        }
    }
}