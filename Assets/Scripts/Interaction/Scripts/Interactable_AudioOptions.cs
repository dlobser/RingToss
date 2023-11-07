using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Interactable_AudioOptions", menuName = "ScriptableObjects/Interactable_AudioOptionsScriptableObject", order = 1)]

public class Interactable_AudioOptions : ScriptableObject
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
