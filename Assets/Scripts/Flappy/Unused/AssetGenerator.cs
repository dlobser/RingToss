using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt
{
    [ExecuteAlways]
    public class AssetGenerator : MonoBehaviour
    {
        public GameObject root;
        public string name;
        public bool rebuild = false;

        public virtual void GenerateAsset()
        {
            if (root != null)
            {
                DestroyImmediate(root);
            }
            root = new GameObject(name);
            root.transform.localPosition = Vector3.zero;
            root.transform.localScale = Vector3.one;
            root.transform.localRotation = Quaternion.identity;
        }

        void Update()
        {
            if (rebuild)
            {
                GenerateAsset();
                rebuild = false;
            }
        }
    }
}