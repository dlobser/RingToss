using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt{
    public class InteractionBehavior_BouncePlayer : InteractionBehavior
    {
        public GameObject player;

        public override void HandleInteraction()
        {
            if (player != null)
            {
                Vector2 tapPosition = Input.mousePosition;
                float relativeX = tapPosition.x / Screen.width; // Normalized position (0 to 1)

                // Assuming you want to keep the vertical velocity constant
                float verticalVelocity = 1.5f;

                // Modify horizontal velocity based on tap position
                // Example: multiply by 5 and then scale with relativeX, which ranges from 0 to 1
                float horizontalVelocity = Mathf.Clamp( relativeX ,.25f,.75f); 

                player.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalVelocity, verticalVelocity) * 5;
            }
        }
    }
}