using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;

public class SettingMenu : MonoBehaviour
{
    // Start is called before the first frame update
     public Resolution[] resolutions;
    public Toggle fullScreenToggle;
    public Dropdown resolutionDropdown;
    public Resolution currentResolution;
    
    
    public Slider volumeSlider;

    public AudioSource audioSrc;
    
    public bool fullScreen;
    public int resolutionIndex;
    public bool isApply = false;
    public float currentVolume =1f;
    public bool currentFullScreen;

    public void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("SliderVolumeLevel", audioSrc.volume);
        audioSrc.volume = PlayerPrefs.GetFloat("SliderVolumeLevel", audioSrc.volume);
        audioSrc.volume = volumeSlider.value;

    }
    public void OnEnable()
    {


        currentResolution = Screen.currentResolution;
              
        currentFullScreen = fullScreenToggle.isOn;
        fullScreenToggle.onValueChanged.AddListener(delegate { OnFullScreenChange(); });
        
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChange(); });
        
        
    }
    public void OnResolutionChange()
    {
        string selectedResolution = resolutionDropdown.options[resolutionDropdown.value].text;
        string[] value = selectedResolution.Split('x');
        
        Screen.SetResolution(int.Parse(value[0]), int.Parse(value[1]), Screen.fullScreen);
        
    }
    
    public void OnVolumeChange()
    {
        audioSrc.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("SliderVolumeLevel", volumeSlider.value);
        audioSrc.volume = PlayerPrefs.GetFloat("SliderVolumeLevel", audioSrc.volume);
    }
    public void OnFullScreenChange()
    {
        Screen.fullScreen = fullScreenToggle.isOn;
        fullScreen = fullScreenToggle.isOn;
    }
    public void OnApplyButtonClick()
    {
        if (FindObjectOfType<PauseScript>() != null) {
            FindObjectOfType<PauseScript>().Resume();
        }
        
        isApply = true;
        currentResolution = Screen.currentResolution;
        currentVolume = volumeSlider.value;
        currentFullScreen = fullScreenToggle.isOn;
        Debug.Log("volume:" + currentVolume);
    }
    public void OnCancelButtonClick()
    {

        if (FindObjectOfType<PauseScript>() != null)
        {
            FindObjectOfType<PauseScript>().Resume();
        }
        isApply = false;
        Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);        
        audioSrc.volume = currentVolume;
        volumeSlider.value = currentVolume;
        Screen.fullScreen = currentFullScreen;
        fullScreenToggle.isOn = currentFullScreen;
    }
}
