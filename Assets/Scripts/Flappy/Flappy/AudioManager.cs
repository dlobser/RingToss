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
            EventManager.Instance.ItemCollected += PlayItemCollectedSound;
            EventManager.Instance.Collision += PlayCollisionEffect;
        }

        private void OnDisable()
        {
            EventManager.Instance.ItemCollected -= PlayItemCollectedSound;
            EventManager.Instance.Collision -= PlayCollisionEffect;
        }

        private void PlayItemCollectedSound()
        {
            // Play item collected sound
            AudioSource.PlayClipAtPoint(itemCollectedClip, transform.position);
        }

        private void PlayCollisionEffect(CollisionEventArgs e)
        {
            // Play wall hit sound

            AudioSource.PlayClipAtPoint(wallHitClip, transform.position);
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