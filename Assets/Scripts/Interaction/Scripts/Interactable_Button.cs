using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;
namespace ON.interaction
{
    [System.Serializable]
    public class AnimationOptions
    {
        public bool turnAnimationOff;
        public bool turnAnimationOn;

    }
    [System.Serializable]
    public class ResetOptions
    {
        public AnimationOptions triggerAnimationOptions;
        public AnimationOptions enableAnimationOptions;
        public bool resetCountersOnTrigger;
        public bool resetCountersOnEnable;
        public bool resetUIOnTrigger;
    }
    [System.Serializable]
    public class UIAudio
    {
        public AudioSource audioSource;
        public AudioClip Enter;
        public float enterVolume = 1;
        public AudioClip Exit;
        public float exitVolume = 1;
        public AudioClip Trigger;
        public float triggerVolume = 1;
        public AudioSource hoverAudioSource;
        public float hoverVolume = 1;
        public AudioSource clickedAudioSource;
        public float clickedVolume = 1;
    }
    public class Interactable_Button : Interactable
    {

        //public InteractableOptions interactableOptions;

        public float clickCounter { get; set; }
        public float clickTime = 1;

        public float inactiveCounter { get; set; }
        public float inactiveTime = 1;

        //[Header("Health Settings")] add some headers

        public GameObject[] UIElements;
        public GameObject[] hoverElements;
        public float[] hoverAlphas;
        SpriteRenderer spriteRenderer;
        MeshRenderer meshRenderer;
        TMPro.TextMeshPro tmPro;
        Vector4 mix = Vector4.zero;

        // public Interactable_AudioOptions uiAudioSO;
        public UIAudio uiAudio;
        public ResetOptions resetOptions; //better name

        public bool setActive;
        public bool setInactive;

        bool update = true;

        void Awake()
        {
            SetMaterialValues();
            //options.triggerOnHoverEnd = interactableOptions.triggerOnHoverEnd;

        }

        public override void HandleStart()
        {
            base.HandleStart();
            if (UIElements.Length == 0)
            {
                UIElements = new GameObject[1];
                UIElements[0] = this.gameObject;
            }
            //if (spriteRenderer == null)
            //    spriteRenderer = UIElement.GetComponent<SpriteRenderer>();
            //if(meshRenderer == null)
                //meshRenderer = UIElement.GetComponent<MeshRenderer>();
        }

        public void DisableAnimation()
        {
            SetMaterialValues();
            update = false;
            clickCounter = 0;
            hoverCounter = 0;
            ping = false;
            prevPing = false;
            state = State.Waiting;
        }

        public void EnableAnimation()
        {
            update = true;
            clickCounter = 0;
            hoverCounter = 0;
            ping = false;
            prevPing = false;
            state = State.Waiting;
            SetMaterialValues();
        }

        public override void HandleUpdate()
        {
            base.HandleUpdate();
            SetMaterialValues();
            if (setInactive)
            {
                base.SetInactive();
            }
            if (setActive)
            {
                setActive = false;
            }
            UpdateHoverAudio();
            UpdateClickedAudio();
            if (debug)
                print(this.gameObject.name + " animating: " + update);
        }

        public override void HandleInactive()
        {
            base.HandleInactive();
            if (setInactive)
            {
                setInactive = false;
            }
            if (setActive)
            {
                base.SetActive();
            }
            if (inactiveCounter < inactiveTime)
            {
                inactiveCounter += Time.deltaTime;
            }
            SetMaterialValues();
            UpdateHoverAudio();
            UpdateClickedAudio();
        }

        private void OnDisable()
        {
            ping = false;
            prevPing = false;
        }

        public override void HandleClicked()
        {
            if (debug)
                Debug.Log(this.gameObject.name + " clicked");
            interactionEvents.clickEvent.Invoke();
            if (uiAudio.audioSource != null)
            {
                if (clickCounter <= 0 && !triggered && (!uiAudio.audioSource.isPlaying || uiAudio.audioSource.clip != uiAudio.Trigger))
                {
                    PlayAudio(uiAudio.Trigger, false, uiAudio.triggerVolume);

                }
            }
            if (clickCounter < clickTime)
            {
                clickCounter += Time.deltaTime;
            }
            else if (!options.handleTriggerOnStartOfClick && clickCounter >= clickTime && options.triggerOnClick)
            {
                if (!triggered)
                    HandleTrigger();
            }
            if (options.handleTriggerOnStartOfClick && options.triggerOnClick)
            {
                if (!triggered)
                    HandleTrigger();
            }
            if (options.stopHoveringOnTrigger)
            {
                if (hoverCounter > 0)
                    hoverCounter -= Time.deltaTime;
                if (hoverCounter < 0)
                    hoverCounter = 0;
            }

        }

        public override void HandleTrigger()
        {
            base.HandleTrigger();
            //IMPLEMENT - play trigger audio on trigger
            // if (uiAudio.audioSource != null)
            // {
            //     if (clickCounter <= 0 && !triggered && (!uiAudio.audioSource.isPlaying || uiAudio.audioSource.clip != uiAudio.Trigger))
            //     {
            //         PlayAudio(uiAudio.Trigger, false, uiAudio.triggerVolume);

            //     }
            // }
            if(resetOptions.resetCountersOnTrigger)
                Reset(resetOptions.triggerAnimationOptions.turnAnimationOn, resetOptions.triggerAnimationOptions.turnAnimationOff);
        }

        public override void HandleWaiting()
        {
            base.HandleWaiting();
            SubClickCounter();
        }

        public override void HandleHover()
        {
            if (debug)
                Debug.Log(this.gameObject.name + " hover" + " triggered: " + triggered + " TimesUp: " + (hoverCounter<options.hoverTime));
            if (hoverCounter < options.hoverTime){
                if(!(triggered && options.stopHoveringOnTrigger))
                    hoverCounter += Time.deltaTime;
            }
            else if (options.triggerOnHoverEnd)
            {
                if (!triggered){
                    HandleTrigger();
                    if(debug)print("Should Trigger");
                }
            }
            if (triggered && options.stopHoveringOnTrigger)
            {
                if (hoverCounter > 0)
                    hoverCounter -= Time.deltaTime;
                if (hoverCounter < 0)
                    hoverCounter = 0;
            }

            interactionEvents.hoverEvent.Invoke();
            SubClickCounter();
        }

        public override void HandleEnter()
        {
            base.HandleEnter();
            PlayAudio(uiAudio.Enter, false, uiAudio.enterVolume);

        }

        public override void HandleExit()
        {
            base.HandleExit();
            PlayAudio(uiAudio.Exit, false, uiAudio.exitVolume);
        }

        void SubClickCounter()
        {
            if (clickCounter > 0 && !options.dontDeselect)
                clickCounter -= Time.deltaTime;
            else if (clickCounter < 0)
            {
                clickCounter = 0;
            }
            if (inactiveCounter > 0)
                inactiveCounter -= Time.deltaTime;

            else if (inactiveCounter < 0)
            {
                inactiveCounter = 0;
            }
        }

        void SetMaterialValues()
        {
            if (update)
            {
                for (int i = 0; i < UIElements.Length; i++)
                {
                    if (UIElements[i].GetComponent<TMPro.TextMeshPro>() != null)
                    {
                        tmPro = UIElements[i].GetComponent<TMPro.TextMeshPro>();
                        mix.Set(hoverCounter / options.hoverTime, clickCounter / clickTime, inactiveCounter / inactiveTime, 0);
                        Color c = tmPro.color;
                        tmPro.overrideColorTags = true;
                        c.a = mix.x;
                        tmPro.color = c;
                    }
                    else if (UIElements[i].GetComponent<MeshRenderer>() != null)       
                    {
                        meshRenderer = UIElements[i].GetComponent<MeshRenderer>();
                        mix.Set(hoverCounter / options.hoverTime, clickCounter / clickTime, inactiveCounter / inactiveTime, 0);
                        meshRenderer.material.SetVector("_Mix", mix);
                    }
                    else if (UIElements[i].GetComponent<SpriteRenderer>() != null)
                    {
                        spriteRenderer = UIElements[i].GetComponent<SpriteRenderer>();
                        mix.Set(hoverCounter / options.hoverTime, clickCounter / clickTime, inactiveCounter / inactiveTime, 0);
                        spriteRenderer.material.SetVector("_Mix", mix);
                    }
                }
                if (hoverElements.Length > 0 && hoverAlphas.Length == 0) 
                {
                    print("madehoverl");
                    hoverAlphas = new float[hoverElements.Length];
                    for (int i = 0; i < hoverElements.Length; i++)
                    {
                        if (hoverElements[i].GetComponent<TMPro.TextMeshPro>() != null)
                        {
                            hoverAlphas[i] = hoverElements[i].GetComponent<TMPro.TextMeshPro>().color.a;
    
                        }
                        else if (hoverElements[i].GetComponent<MeshRenderer>() != null)
                        {
                            hoverAlphas[i] = hoverElements[i].GetComponent<MeshRenderer>().material.color.a;
                        }
                        else if (hoverElements[i].GetComponent<SpriteRenderer>() != null)
                        {
                            hoverAlphas[i] = hoverElements[i].GetComponent<SpriteRenderer>().color.a;
                        }
                    }
                }

                for (int i = 0; i < hoverElements.Length; i++)
                {
                    if (hoverElements[i].GetComponent<TMPro.TextMeshPro>() != null)
                    {
                        tmPro = hoverElements[i].GetComponent<TMPro.TextMeshPro>();
                        mix.Set(hoverCounter / options.hoverTime, clickCounter / clickTime, inactiveCounter / inactiveTime, 0);
                        Color c = tmPro.color;
                        tmPro.overrideColorTags = true;
                        c.a = mix.x * hoverAlphas[i];
                        tmPro.color = c;
                    }
                    else if (hoverElements[i].GetComponent<MeshRenderer>() != null )
                    {
                        meshRenderer = hoverElements[i].GetComponent<MeshRenderer>();
                        mix.Set(hoverCounter / options.hoverTime, 0, 0, 0);
                        Color c = meshRenderer.material.color;
                        c.a = mix.x * hoverAlphas[i];
                        meshRenderer.material.color = c;
                    }
                    else if (hoverElements[i].GetComponent<SpriteRenderer>() != null)
                    {
                        spriteRenderer = hoverElements[i].GetComponent<SpriteRenderer>();
                        mix.Set(hoverCounter / options.hoverTime, 0, 0, 0);
                        Color c = spriteRenderer.color;
                        c.a = mix.x * hoverAlphas[i];
                        spriteRenderer.color = c;
                    }
                }
            }
        }

        void PlayAudio(AudioClip clip, bool loop, float volume)
        {
            if (update)
            {
                if (uiAudio.audioSource != null)
                {
                    if (clip != null)
                    {
                        uiAudio.audioSource.clip = clip;
                        uiAudio.audioSource.loop = loop;
                        uiAudio.audioSource.Play();
                        uiAudio.audioSource.volume = volume;
                    }
                }
            }
        }

        void UpdateHoverAudio()
        {
            if (update && uiAudio.hoverAudioSource != null)
            {
                //print(uiAudio.hoverAudioSource.isPlaying);
                uiAudio.hoverAudioSource.volume = (hoverCounter / options.hoverTime) * uiAudio.hoverVolume;
            }

        }

        void UpdateClickedAudio()
        {
            if (update && uiAudio.clickedAudioSource != null)
            {
                uiAudio.clickedAudioSource.volume = (clickCounter / clickTime) * uiAudio.clickedVolume;
            }
        }

        void OnEnable()
        {
            if (resetOptions.resetCountersOnEnable)
                Reset(resetOptions.enableAnimationOptions.turnAnimationOn, resetOptions.enableAnimationOptions.turnAnimationOff);
        }

        public void Reset(bool turnOn,bool turnOff)
        {

            GameObject target = this.gameObject;
            target.GetComponent<Interactable_Button>().hoverCounter = 0;
            target.GetComponent<Interactable_Button>().clickCounter = 0;
            target.GetComponent<Interactable_Button>().inactiveCounter = 0;
            if (turnOff)
                target.GetComponent<Interactable_Button>().DisableAnimation();
            if (turnOn)
                target.GetComponent<Interactable_Button>().EnableAnimation();
            if (resetOptions.resetUIOnTrigger)
                SetMaterialValues();


        }
    }
}