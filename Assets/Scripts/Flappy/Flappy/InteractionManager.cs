using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt.Flappy
{
    public class InteractionManager : Quilt.InteractionManager
    {
        public GameObject player;
        bool gameOver = false;

        private void Update()
        {
            // if (!gameOver)
            // {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                Debug.Log("Interacting");

                moose();
            }
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Interacting");

                moose();
            }
            // }
            if (player == null)
            {
                Debug.Log("Where's the player");
            }

        }

        void moose()
        {
            Debug.Log("Tap: " + player);
            if (player != null)
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(.5f, 1.5f) * 5;
        }
    }
}