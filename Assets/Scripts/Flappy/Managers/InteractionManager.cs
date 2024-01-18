using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt{
    public class InteractionManager : Manager
    {
        public List<InteractionBehavior> interactionBehaviors = new List<InteractionBehavior>();
        public void RegisterAction(InteractionBehavior behavior){
            interactionBehaviors.Add(behavior);
        }
        public virtual void Interact(){
            foreach(InteractionBehavior behavior in interactionBehaviors){
                behavior.HandleInteraction();
            }
        }
    }
}