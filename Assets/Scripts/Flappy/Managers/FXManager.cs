using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt
{
    public class FXManager : Manager
    {
        public virtual void PlayEffectAtLocation(GameObject effect, Vector3 location,
        float intensity = 1f, float scale = 1f, float duration = 1f)
        {
            GameObject newEffect = Instantiate(effect, location, Quaternion.identity, Globals.GetGameRoot());
            newEffect.transform.localScale = new Vector3(scale, scale, scale);
            Destroy(newEffect, duration);
        }
    }
}
