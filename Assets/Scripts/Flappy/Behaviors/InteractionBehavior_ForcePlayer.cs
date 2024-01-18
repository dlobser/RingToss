using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt {
    public class InteractionBehavior_ForcePlayer : InteractionBehavior {
        public GameObject player;
        public Vector2 maxVelocity = new Vector2(1.5f, 4f); // Set your desired maximum velocities for x and y here
        private Coroutine forceCoroutine;

        public override void HandleInteraction() {
            if (player != null) {
                Vector2 tapPosition = Input.mousePosition;
                float relativeY = tapPosition.y / Screen.height; // Normalized position (0 to 1)

                float horizontalVelocity = 1f;
                float verticalVelocity = (relativeY-.5f)*5;//relativeY > .5f ? 1 : -1;
                Vector2 force = new Vector2(horizontalVelocity, verticalVelocity) * 2;

                if (forceCoroutine != null) {
                    StopCoroutine(forceCoroutine);
                }
                forceCoroutine = StartCoroutine(ApplyForceOverTime(force, 1f));
            }
        }

        private IEnumerator ApplyForceOverTime(Vector2 force, float duration) {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            float time = 0;

            while (time < duration) {
                float diminishingMultiplier = 1 - (time / duration); // Decreases from 1 to 0 over the duration
                Vector2 appliedForce = (force * Time.deltaTime * diminishingMultiplier) / duration;
                Vector2 newVelocity = rb.velocity + appliedForce;
                
                newVelocity.x = Mathf.Clamp(newVelocity.x, -maxVelocity.x, maxVelocity.x);
                newVelocity.y = Mathf.Clamp(newVelocity.y, -maxVelocity.y, maxVelocity.y);

                rb.velocity = newVelocity; // Applying the clamped velocity
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}
