using UnityEngine;
using UnityEngine.Events;

namespace Quilt{
    public class Item : MonoBehaviour
    {
        public Globals.CustomTag customTag = Globals.CustomTag.Item;
        public int scoreValue = 10;
        public bool destroyOnHit;
        public AudioClip collisionSound; // Audio clip to be played on collision
        public Vector2 audioPitchRange = new Vector2(.7f, 1); // Range of possible pitches for the audio clip
        public Vector2 audioVolumeRange = new Vector2(.7f, 1); // Range of possible volumes for the audio clip
        public Platform platform;
        public UnityEvent onTriggerEnterAction;

        private void Start()
        {

        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollide();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnCollide();
        }

        private void OnCollide(){
            Globals.GlobalSettings.Managers.audioManager.PlayOneShotAtLocation(collisionSound,
            this.transform.position,
            Random.Range(audioPitchRange.x,audioPitchRange.y), 
            Random.Range(audioVolumeRange.x,audioVolumeRange.y));
            onTriggerEnterAction.Invoke();
            Hit();
        }

        public virtual void Hit()
        {
            if (platform != null)
            {
                platform.OnItemHit();
            }
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}