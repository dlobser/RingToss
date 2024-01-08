using UnityEngine;
using UnityEngine.Events;

namespace Quilt
{
    [CreateAssetMenu(fileName = "CollisionBehaviorSettings", menuName = "Game/CollisionBehaviorSettings")]
    public class CollisionBehaviorSettings : ScriptableObject
    {
        public Globals.CustomTag customTag = Globals.CustomTag.Item;
        public bool addToScore = false;
        public int scoreValue = 10;
        public bool destroyOnHit;
        public bool destroyColliderOnHit;
        public bool destroyObjectsOnhit;
        public GameObject[] objectsToDestroyOnHit;
        public bool endGame;
        public bool playSound;
        public AudioClip collisionSound; // Direct reference to the audio clip
        public Vector2 audioPitchRange = new Vector2(.7f, 1);
        public Vector2 audioVolumeRange = new Vector2(.7f, 1);
        public float audioVelocityMultiplier = 1;
        public bool playEffect;
        public GameObject effectPrefab; // Direct reference to the effect prefab
        public Vector2 effectIntensityRange = new Vector2(.7f, 1);
        public Vector2 effectScaleRange = new Vector2(.7f, 1);
        public Vector2 effectDurationRange = new Vector2(.7f, 1);
        public UnityEvent onTriggerEnterAction;

    }
}