using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedToVolume : MonoBehaviour
{
    public float speedMultiplier = 0.1f;
    AudioSource audio;
    float counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (audio == null)
            audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (counter < 1)
            counter += Time.deltaTime;
        SetVolume();
    }

    //function to get the speed of the player and set the volume of the audio listener to that speed
    public void SetVolume()
    {
        //get the speed of the player
        float speed = GetComponent<Rigidbody2D>().velocity.magnitude * speedMultiplier * counter;
        //set the volume of the audio listener to the speed of the player
        audio.volume = Mathf.Min(speed, 1.0f);
        audio.pitch = speed;
    }
}
