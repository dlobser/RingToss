using UnityEngine;
using UnityEngine.Events;

namespace Quilt
{
    public class CollisionBehavior_PlayFX : MonoBehaviour
    {
        public string effectPrefabName;
        GameObject effectPrefab;
        public Vector2 effectIntensityRange;
        public Vector2 effectScaleRange;
        public Vector2 effectDurationRange;
        public Vector3 colliderPosition;

        public virtual void HandleCollision(GameObject collisionObject = null)
        {
            effectPrefab = Resources.Load<GameObject>(effectPrefabName);
            if(effectPrefab!=null)
                GameManager.Instance.fxManager.PlayEffectAtLocation(
                    effectPrefab,
                    colliderPosition,
                    Random.Range(effectIntensityRange.x, effectIntensityRange.y),
                    Random.Range(effectScaleRange.x, effectScaleRange.y),
                    Random.Range(effectDurationRange.x, effectDurationRange.y));
            else{
                Debug.Log("Effect Prefab not found" + effectPrefabName);
            }
        }
    }
}

