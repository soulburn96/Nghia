using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class ParticleSoundEffect : MonoBehaviour
{
    [SerializeField] private InteractableInputs input;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startClip;
    [SerializeField] private AudioClip loopClip;
    [SerializeField] private AudioClip stopClip;


    IEnumerator PlayStartAndLoopSound()
    {
        audioSource.clip = startClip;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.loop = true;
        audioSource.clip = loopClip;
        audioSource.Play();
    }


    public void PlaySound()
    {        
       
        StartCoroutine(PlayStartAndLoopSound());
    }
    public void StopSound()
    {
        StopAllCoroutines();
        audioSource.loop = false;
        audioSource.clip = stopClip;
        audioSource.Play();
    }
}
