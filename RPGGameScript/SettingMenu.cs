using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SettingMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider volumeSlider;
    public AudioSource audioSrc;


    public void OnEnable()
    {
        volumeSlider.onValueChanged.AddListener(delegate { SetVolume(); });
    }
    public void SetVolume()
    {
        audioSrc.volume = volumeSlider.value; 
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
