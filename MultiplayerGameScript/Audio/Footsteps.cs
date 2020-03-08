using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{

    public AudioSource rightFoot;
    public AudioSource leftFoot;
    private int rightPreviousFootstepClip = 0;
    private int leftPreviousFootstepClip = 0;
    private int rightCurrentFootstepClip = 0;
    private int leftCurrentFootstepClip = 0;
    public AudioClip[] footsteps = new AudioClip[4];
    bool areStepsPlayed = false;
    Coroutine lastFootstepRoutine;
    System.Random rnd = new System.Random();
    private PlayerController playerController;

    private float stepDuration;


    // Start is called before the first frame update
    void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        stepDuration = playerController.stepDuration;
    }

    // Update is called once per frame
    void Update()
    {
        ControlAudio();
    }


    IEnumerator PlayFootsteps()
    {
        RandomizeFootstepClip(rightFoot, ref rightCurrentFootstepClip, ref rightPreviousFootstepClip);
        rightFoot.Play();
        yield return new WaitForSeconds(stepDuration);
        RandomizeFootstepClip(leftFoot, ref leftCurrentFootstepClip, ref leftPreviousFootstepClip);
        leftFoot.Play();
        yield return new WaitForSeconds(stepDuration);
        lastFootstepRoutine = StartCoroutine(PlayFootsteps());
    }

    void ControlAudio()
    {
        if (playerController.playerMovement != null && areStepsPlayed == false)
        {
            areStepsPlayed = true;
            lastFootstepRoutine = StartCoroutine(PlayFootsteps());
        }
        else if (playerController.playerMovement == null && areStepsPlayed == true)
        {
            areStepsPlayed = false;
            StopCoroutine(lastFootstepRoutine);
        }
    }

    // todo: pass by reference !!!
    void RandomizeFootstepClip(AudioSource audioSource, ref int currentFootstepClip, ref int previousFootstepClip)
    {
        currentFootstepClip = rnd.Next(0, 3);
        if (currentFootstepClip == previousFootstepClip)
        {
            currentFootstepClip++;
            currentFootstepClip %= 4;
        }

        //Debug.Log(currentFootstepClip + " " + previousFootstepClip);

        audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        audioSource.volume = UnityEngine.Random.Range(0.8f, 1.0f);
        audioSource.clip = footsteps[currentFootstepClip];
        previousFootstepClip = currentFootstepClip;
    }
}
