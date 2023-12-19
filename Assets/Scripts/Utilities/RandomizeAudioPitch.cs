using UnityEngine;

public class RandomizeAudioPitch : MonoBehaviour
{
    public AudioSource audioSource;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
        }
        else
        {
            Debug.LogWarning("AudioSource component not found on the GameObject");
        }
    }
}
