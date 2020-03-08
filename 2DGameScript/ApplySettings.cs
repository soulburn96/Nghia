using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplySettings : MonoBehaviour
{
    // Start is called before the first frame update
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
