using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.interaction
{

    public class ResetInteractable_ButtonCounters : MonoBehaviour
    {
        public bool turnAnimationOff;
        public bool turnAnimationOn;
        public GameObject target;


        public void Reset()
        {
            if (target == null)
                target = this.gameObject;
            target.GetComponent<Interactable_Button>().hoverCounter = 0;
            target.GetComponent<Interactable_Button>().clickCounter = 0;
            target.GetComponent<Interactable_Button>().inactiveCounter = 0;
            if(turnAnimationOff)
                target.GetComponent<Interactable_Button>().DisableAnimation();
            if (turnAnimationOn)
                target.GetComponent<Interactable_Button>().EnableAnimation();
        }
    }
}