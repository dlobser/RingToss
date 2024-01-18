using JetBrains.Annotations;
using UnityEngine;

namespace Quilt
{
    public class CollisionBehavior_PlaySound : CollisionBehavior
    {
        public string collisionSoundPath; // Path to the AudioClip in the Resources folder
        private AudioClip collisionSound;
        public Vector2 audioVolumeRange = new Vector2(0.5f, 1f);
        public float audioVelocityMultiplier = 0.5f;

        public void Init(string path = "")
        {
            if(path.Length>0)
                collisionSoundPath = path;
            
            collisionSound = Resources.Load<AudioClip>(collisionSoundPath);
            if (collisionSound == null)
            {
                Debug.LogWarning("Audio clip not found at path: " + collisionSoundPath);
            }
        }

        public override void HandleCollision(GameObject collisionObject = null)
        {

            if (collisionSound == null)
            {
                return; // No sound to play
            }

            float velocity = 0;
            if (collisionObject.GetComponent<Rigidbody2D>() != null)
                velocity = collisionObject.GetComponent<Rigidbody2D>().velocity.magnitude / 10;

            Globals.GetAudioManager().PlayOneShotAtLocation(
                collisionSound,
                colliderPosition,
                Random.Range(audioVolumeRange.x, audioVolumeRange.y) + 
                Mathf.LerpUnclamped(0f, velocity, audioVelocityMultiplier));
        }
    }
}
