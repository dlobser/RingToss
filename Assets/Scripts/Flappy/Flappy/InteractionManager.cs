using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt.Flappy{
    public class InteractionManager : Quilt.InteractionManager
    {
        public GameObject player {get;set;}
        bool gameOver = false;

        private void Update()
        {
            if(!gameOver){
                if (Input.GetMouseButtonDown(0))
                {
                    OnMouseDown();
                }
            }
        }

        void OnMouseDown(){
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(.5f,1.5f) * 5;
        }
    }   
}