using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeSoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource motorSource;
     
    public AudioSource fxPlayer;
    public AudioClip startBikeClip, motorClip, bikeCrash;
    void Start()
    {
        if(motorSource && motorClip)
        {
            motorSource.clip = motorClip;
            motorSource.Play();
            motorSource.volume = 0;
        }

    }
    public void updateMotorVolume(float newVolume)
    {
        motorSource.volume = newVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void StartBike()
    {
        if (fxPlayer && startBikeClip)
        {
            fxPlayer.PlayOneShot(startBikeClip);
        }
    }
}
