// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace ON.interaction{

//     public class RaycastInteraction : MonoBehaviour {
        
//         [Tooltip("Used to link specific raycasters with interactables")]
//         public string type;

//         public Vector3 hitPosition { get; set; }
//         public Vector3 hitNormal{ get; set; }
//         public GameObject hitObject{ get; set; }

//         public bool useMouse;

//     	public delegate void MouseHasHit();
//     	public static event MouseHasHit mouseHasHit;
//         [Tooltip("Turn this off to only raycast when a button is clicked")]
//         public bool alwaysActive = true;
//         [Tooltip("Drag in a button component, or leave this slot empty")]
//         public Button button;
//         float click;
//         int layerMask;
//         [Tooltip("Raycast to layers, or leave blank for all")]
//         public string[] layers;

//     	void Start() {
//             if (button == null)
//                 alwaysActive = true;
//             if(layers.Length>0)
//                 layerMask = LayerMask.GetMask(layers);
//         }


//         void Update() {

//             if (alwaysActive || button != null && button.click > .5f)
//             {
//                 RaycastHit hit = new RaycastHit();
//                 bool didHit;

//                 if (useMouse)
//                 {
//                     if (layers.Length > 0)
//                         didHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1e6f, layerMask);
//                     else
//                         didHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
//                 }
//                 else
//                 {
//                     if (layers.Length > 0)
//                         didHit = Physics.Raycast(new Ray(this.transform.position, this.transform.forward), out hit, 1e6f, layerMask);
//                     else
//                         didHit = Physics.Raycast(new Ray(this.transform.position, this.transform.forward), out hit);
//                 }
           
//                 if (didHit)
//                 {
//                     hitPosition = hit.point;
//                     hitNormal = hit.normal;
//                     hitObject = hit.collider.gameObject;

//                     click = 0;
//                     if (button != null)
//                         click = button.click;
                        

//                     if (hit.transform.gameObject.GetComponent<Interactable>() != null)
//                     {
//                         Interactable[] interactables = hit.transform.gameObject.GetComponents<Interactable>();
//                         foreach (Interactable t in interactables)
//                         {
//                             t.Ping(this, click, type);
//                         }
//                     }
//                 }
//                 else
//                 {
//                     hitPosition = Vector3.zero;
//                     hitNormal = Vector3.zero;
//                     hitObject = null;
//                 }
//             }

//     	}
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.interaction{

    public class RaycastInteraction : MonoBehaviour {
        
        [Tooltip("Used to link specific raycasters with interactables")]
        public string type;

        public Vector3 hitPosition { get; set; }
        public Vector3 hitNormal{ get; set; }
        public GameObject hitObject{ get; set; }

        public bool useMouse;

    	public delegate void MouseHasHit();
    	public static event MouseHasHit mouseHasHit;
        [Tooltip("Turn this off to only raycast when a button is clicked")]
        public bool alwaysActive = true;
        [Tooltip("Drag in a button component, or leave this slot empty")]
        public Button button;
        float click;
        int layerMask;
        [Tooltip("Raycast to layers, or leave blank for all")]
        public string[] layers;

    	void Start() {
            if (button == null)
                alwaysActive = true;
            if(layers.Length>0)
                layerMask = LayerMask.GetMask(layers);
        }


       void Update() {
            if (alwaysActive || (button != null && button.click > .5f)) {
                // Perform 3D Raycast
                RaycastHit hit3D;
                bool didHit3D = false;

                // Perform 2D Raycast
                RaycastHit2D hit2D = default;
                bool didHit2D = false;

                Vector3 raycastDirection = useMouse ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : transform.forward;
                Vector2 raycastDirection2D = new Vector2(raycastDirection.x, raycastDirection.y);

                if (useMouse) {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (layers.Length > 0) {
                        didHit3D = Physics.Raycast(ray, out hit3D, Mathf.Infinity, layerMask);
                        hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);
                    } else {
                        didHit3D = Physics.Raycast(ray, out hit3D);
                        hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    }
                } else {
                    Vector3 rayOrigin = transform.position;
                    Ray ray = new Ray(rayOrigin, transform.forward);
                    if (layers.Length > 0) {
                        didHit3D = Physics.Raycast(ray, out hit3D, Mathf.Infinity, layerMask);
                        hit2D = Physics2D.Raycast(rayOrigin, raycastDirection2D, Mathf.Infinity, layerMask);
                    } else {
                        didHit3D = Physics.Raycast(ray, out hit3D);
                        hit2D = Physics2D.Raycast(rayOrigin, raycastDirection2D);
                    }
                }

                if (didHit3D || didHit2D)
                {
                    bool h3d = true;
                    if(didHit2D) h3d = false;
                    hitPosition = h3d ? hit3D.point : hit2D.point;
                    hitNormal = h3d ? hit3D.normal : hit2D.normal;
                    hitObject = h3d ? hit3D.collider.gameObject : hit2D.collider.gameObject;

                    click = 0;
                    if (button != null)
                        click = button.click;
                        

                    if (hitObject.transform.gameObject.GetComponent<Interactable>() != null)
                    {
                        Interactable[] interactables = hitObject.transform.gameObject.GetComponents<Interactable>();
                        foreach (Interactable t in interactables)
                        {
                            t.Ping(this, click, type);
                        }
                    }
                }
                else
                {
                    hitPosition = Vector3.zero;
                    hitNormal = Vector3.zero;
                    hitObject = null;
                }
            }
    	}
    }
}