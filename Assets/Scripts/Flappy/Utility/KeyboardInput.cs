using System.Collections;
using System.Collections.Generic;
using Quilt;
using UnityEngine;

namespace Quilt{
public class KeyboardInput : MonoBehaviour
{
    public string collisionSoundPath; // Path to the AudioClip in the Resources folder
    private AudioClip collisionSound;
    public Vector2 audioVolumeRange = new Vector2(0.5f, 1f);

    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        Init(collisionSoundPath);
    }

    public void Init(string path = "")
    {
        if(path.Length>0)
            collisionSoundPath = path;
        
        collisionSound = Resources.Load<AudioClip>(collisionSoundPath);
        if (collisionSound == null)
        {
            Debug.LogWarning("Audio clip not found at path: " + collisionSoundPath);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyUp(KeyCode.D))
            // FindObjectOfType<GameGeneratorManager>().DestroyGame();
        if (Input.GetKeyUp(KeyCode.S))
            FindObjectOfType<GameGeneratorManager>().BuildGame();
        if(Input.GetKeyDown(KeyCode.A)){
            if (collisionSound == null)
            {
                return; // No sound to play
            }


            GameManager.Instance.audioManager.PlayOneShotAtLocation(
                collisionSound,
                Vector3.zero,
                Random.Range(audioVolumeRange.x, audioVolumeRange.y));
        }
        if(Input.GetKeyDown(KeyCode.B)){
            audioSource.Play();
        }


    }
}
}