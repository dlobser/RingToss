using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ON.interaction
{
    [System.Serializable]
    public class interactionEvents
    {
        public UnityEvent triggerEvent;
        public UnityEvent hoverEvent;
        public UnityEvent waitingEvent;
        public UnityEvent enterEvent;
        public UnityEvent exitEvent;
        public UnityEvent clickEvent;
    }
    [System.Serializable]
    public class Options
    {
        public bool triggerOnClick = false;
        public bool triggerOnHoverEnd = false;
        public bool blockEnterEventIfClicked = true;
        public bool triggerOnce = false;
        public bool handleTriggerOnStartOfClick = false;
        public bool dontDeselect = false;
        public bool stopHoveringOnTrigger = false;
        public bool noTriggerIfClickOnEnter = false;
        public float clickThreshold = 1;
        public float hoverTime = 1;
    }
    public class Interactable : MonoBehaviour //get rid of this inheritance
    {

        public bool ping { get; set;}
        public bool prevPing { get; set; }

        public enum State { Enter, Hover, Exit, Waiting, Clicked, Trigger, Inactive };
        protected State state;
        protected State prevState;

        public RaycastInteraction gaze { get; set; }

        public bool debug;

        //public bool triggerOnClick = false;
        //public bool triggerOnHoverEnd = false;
        //public bool blockEnterEventIfClicked = true;
        //public bool triggerOnce = false;
        public Options options;
        public interactionEvents interactionEvents;
        public bool triggered { get; set; }

        public float clicked { get; set; }

        public string type;
        public float hoverCounter { get; set; }


    	private void Start()
    	{
            HandleStart();
    	}

        private void OnEnable()
        {
            ping = false;
            prevPing = false;
        }

        void Update()
        {

            if (state != State.Inactive)
            {
                HandleUpdate();
            }
            else
            {
                HandleInactive();
            }

        }

        public virtual void HandleStart(){
            state = new State();
        }

        public virtual void HandleUpdate(){

            if (!ping && !prevPing)
            {
                state = State.Waiting;
            }
            else if (ping && !prevPing)
            {
                state = State.Enter;
            }
            else if (ping && prevPing)
            {
                state = (clicked>=options.clickThreshold) ? State.Clicked : State.Hover;
                if (state == State.Hover && prevState == State.Clicked)
                    if(!options.triggerOnce)
                        triggered = false;
            }
            else if (!ping && prevPing)
            {
                state = State.Exit;
            }

            if (state == State.Enter)
                HandleEnter();
            if (state == State.Hover)
                HandleHover();
            if (state == State.Exit)
                HandleExit();
            if (state == State.Waiting)
                HandleWaiting();
            if (state == State.Trigger)
                HandleTrigger();
            if (state == State.Clicked)
                HandleClicked();

            if (debug)
                print("pinging: " + ping);

            prevPing = ping;
            prevState = state;
            ping = false;

           
        }

        public virtual void Ping(RaycastInteraction raycaster, float click, string whatType){
            gaze = raycaster;
            if(whatType.Equals(type)){
                ping = true;
                clicked = click;
            }
        }

        public virtual void SetInactive()
        {
            state = State.Inactive;
        }

        public virtual void SetActive()
        {
            state = State.Waiting;
        }

        public virtual void HandleInactive()
        {
            if (debug)
                Debug.Log(this.gameObject.name + " inactive");
        }

        public virtual void HandleEnter(){
            if (debug)
                Debug.Log(this.gameObject.name + " enter");
            if (options.noTriggerIfClickOnEnter && clicked >= options.clickThreshold)
                triggered = true;
            else
                triggered = false;
            if(clicked < options.clickThreshold)
                interactionEvents.enterEvent.Invoke();
            else if (!options.blockEnterEventIfClicked)
            {
                interactionEvents.enterEvent.Invoke();
            }

        }

        public virtual void HandleHover(){
            if (debug)
                Debug.Log(this.gameObject.name + " hover");
            if (hoverCounter < options.hoverTime)
                hoverCounter += Time.deltaTime;
            else if (options.triggerOnHoverEnd)
            {
                if (!triggered)
                    HandleTrigger();
            }
            interactionEvents.hoverEvent.Invoke();
        }

        public virtual void HandleExit(){
            if (debug)
                Debug.Log(this.gameObject.name + " exit");
            triggered = false;
            interactionEvents.exitEvent.Invoke();
        }

        public virtual void HandleClicked()
        {
            if (debug)
                Debug.Log(this.gameObject.name + " clicked");
            interactionEvents.clickEvent.Invoke();
            if (options.triggerOnClick)
            {
                if(!triggered)
                    HandleTrigger();
            }
           
        }

        public virtual void HandleWaiting(){
            if (hoverCounter > 0)
                hoverCounter -= Time.deltaTime;
            else if (hoverCounter < 0)
            {
                hoverCounter = 0;
            }
            if (debug)
                Debug.Log(this.gameObject.name + " is waiting");
            interactionEvents.waitingEvent.Invoke();
        }

        public virtual void HandleTrigger()
        {
            if (debug)
                Debug.Log(this.gameObject.name + " trigger");
            interactionEvents.triggerEvent.Invoke();
            triggered = true;
            //base.Click();
        }
    }
}