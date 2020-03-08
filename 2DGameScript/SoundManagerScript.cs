using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{

    public AudioSource audioSource;

    void Start()
    {
        audioSource.volume = PlayerPrefs.GetFloat("SliderVolumeLevel", 5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
