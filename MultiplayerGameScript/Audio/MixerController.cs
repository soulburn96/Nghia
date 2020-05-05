using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Handles changing master volume in the game.
/// </summary>
// The script is placed on the    Player Camera / Main Camera / Canvas / Options / volume / slider

public class MixerController : MonoBehaviour
{
	public AudioMixer mixer;

	public void ChangeAmbientVolume(float sliderValue)	// it turns out that RESONANCE AUDIO doesn't support multiple mixers in UNITY. I am afraid it will be necessary to write 
		// some mixer component or operate on single AudioSources. Thus, this mixer's volume cannot be changed.
	{
		mixer.SetFloat("volumeAmbient", Mathf.Log10(sliderValue) * 20);	// this allows to change volume with slider in logarithmic way, not linear.
	}

	public void ChangeMasterVolume(float sliderValue)
	{
		mixer.SetFloat("volumeMaster", Mathf.Log10(sliderValue) * 20);  // this allows to change volume with slider in logarithmic way, not linear.
	}
}
