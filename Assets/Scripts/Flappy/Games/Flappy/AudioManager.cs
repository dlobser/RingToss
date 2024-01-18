using System.Collections.Generic;
using MM.Msg;
using UnityEngine;

namespace Quilt.Flappy
{
    public class AudioManager : Quilt.AudioManager
    {
        // Add references to audio clips
        public AudioClip itemCollectedClip;
        public AudioClip wallHitClip;

        public AudioSource musicAudioA;
        public AudioSource musicAudioB;

        public string levelStartSoundLocation;
        AudioClip levelStartSound;

        List<AudioSource> fxAudioSources = new List<AudioSource>();
        GameObject audioPool;
        GameObject activeAudio;

        void Start()
        {
            GameObject audioManager = new GameObject("AudioManager");
            audioManager.transform.parent = Globals.GetManagersRoot();
            audioPool = new GameObject("AudioPool");
            audioPool.transform.parent = audioManager.transform;
            audioPool.SetActive(false);
            activeAudio = new GameObject("ActiveAudio");
            activeAudio.transform.parent = audioManager.transform;

            for (int i = 0; i < 20; i++)
            {
                GameObject audio = new GameObject("Audio");
                audio.transform.parent = audioPool.transform;
                audio.AddComponent<AudioSource>();
                audio.GetComponent<AudioSource>().playOnAwake = false;
                fxAudioSources.Add(audio.GetComponent<AudioSource>());
            }

            levelStartSound = Resources.Load<AudioClip>(levelStartSoundLocation);
        }

        void Update()
        {
            foreach (AudioSource audio in fxAudioSources)
            {
                if (audio.gameObject.activeInHierarchy)
                {
                    if (!audio.isPlaying)
                    {
                        audio.transform.parent = audioPool.transform;
                    }
                }
            }
        }

        private void OnEnable()
        {
            GameManager.Instance.eventManager.OnLevelStart += PlayLevelStartSound;
            GameManager.Instance.eventManager.OnLevelEnd += PlayLevelEndSound;
        }

        private void OnDisable()
        {
            GameManager.Instance.eventManager.OnLevelStart -= PlayLevelStartSound;
            GameManager.Instance.eventManager.OnLevelEnd -= PlayLevelEndSound;
        }

        private void PlayLevelStartSound()
        {
            levelStartSound = Resources.Load<AudioClip>(levelStartSoundLocation);
            Debug.Log("Playing level start sound: " + levelStartSound);
            PlayOneShot(levelStartSound);
        }

        private void PlayLevelEndSound()
        {

        }

        public void PlayOneShot(AudioClip clip){
            if (audioPool != null && clip!=null)
            {
                if (audioPool.transform.childCount > 0)
                {
                    
                    AudioSource effect = audioPool.transform.GetChild(0).GetComponent<AudioSource>();
                    effect.transform.parent = activeAudio.transform;
                    effect.clip = clip;
                    effect.Play();
                    Debug.Log("Playing sound: " + clip + "" + effect.transform.parent);
                }
            }
        }

        public override void PlayOneShotAtLocation(AudioClip clip, Vector3 location, float volume = 1f, float pitch = 1f)
        {
            if (audioPool != null)
            {
                if (audioPool.transform.childCount > 0)
                {
                    AudioSource effect = audioPool.transform.GetChild(0).GetComponent<AudioSource>();
                    effect.transform.parent = activeAudio.transform;
                    effect.volume = volume;
                    effect.pitch = pitch;
                    effect.transform.position = location;
                    effect.clip = clip;
                    effect.Play();
                    Debug.Log("Playing sound at location: " + effect);
                }
            }
        }
    }
}