using UnityEngine;

namespace Quilt
{
    [CreateAssetMenu(fileName = "ManagerReferences", menuName = "Game/ManagerReferences")]
    public class ManagerReferences : ScriptableObject
    {
        public GameObject gameManagerPrefab;
        public GameObject menuManagerPrefab;
        public GameObject scoreManagerPrefab;
        public GameObject interactionManagerPrefab;
        public GameObject fxManagerPrefab;
        public GameObject audioManagerPrefab;
        public GameObject eventManagerPrefab;
        public CreationSettings creationSettings;
    }
}