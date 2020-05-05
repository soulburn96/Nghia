using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Handles playing footsteps locally as well as on the network.
/// </summary>
// This script is attached to every character  /Assets/Resources/*Player
// Footstep sound sources are attached to characters' armatures

public class Footsteps : MonoBehaviour
{
	public bool toggleOwnFootsteps = true;	//TESTING PURPOSES
    public AudioSource rightFoot;
	public AudioSource leftFoot;
	public AudioClip[] footsteps = new AudioClip[4];
	public AudioClip idleSound;
	public GameObject idleSoundSourcePrefab;
	public float timeToPlayIdleSounds = 5.0f;
	
	private float ownFootstepsVolume = 0.03f;
    private int rightPreviousFootstepClip = 0;
    private int leftPreviousFootstepClip = 0;
    private int rightCurrentFootstepClip = 0;
    private int leftCurrentFootstepClip = 0;
    private bool areStepsPlayed = false;
    private Coroutine lastFootstepRoutine;
    private System.Random rnd = new System.Random();
    private PlayerController playerController;
	private PhotonView photon;
	private float stepDuration;


    void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        stepDuration = playerController.stepDuration;
		photon = gameObject.GetComponent<PhotonView>();
	}

    void Update()
    {
        ControlAudio();
    }



    void ControlAudio()	// controls all the audio in this class. 
    {
		if (playerController.playerMovement != null && areStepsPlayed == false)		// if player moves but steps are not played yet
        {
            areStepsPlayed = true;
			if (toggleOwnFootsteps)	//TESTING PURPOSES: you can turn your footsteps on/off in game (options menu).
				PlayFootstepsFunction(true);
			photon.RPC("PlayFootstepsFunction", RpcTarget.Others, false);	//play your footsteps on enemies' clients
		}
		else if (playerController.playerMovement == null && areStepsPlayed == true)	//if player stopped and footsteps are still being played
        {
            areStepsPlayed = false;
			if(toggleOwnFootsteps) //TESTING PURPOSES:
				StopFootstepsFunction();
			photon.RPC("StopFootstepsFunction", RpcTarget.Others);	//enemies stop hearing your footsteps
		}
	}



    IEnumerator PlayFootsteps(bool isMine)		// coroutine playing footstep sounds interchangeably
    {
        RandomizeFootstepClip(isMine, rightFoot, ref rightCurrentFootstepClip, ref rightPreviousFootstepClip);

		if (isMine) //RESONANCEAUDIO GLITCH: turning off/on "spatialize" and "enabled" is performed to fix issues with ResonanceAudio. Do it only for your player, so there won't be so many audio vibrations during debugging
		{
			rightFoot.spatialize = true;
			rightFoot.enabled = true;
		}
		rightFoot.PlayOneShot(rightFoot.clip);
		yield return new WaitForSeconds(stepDuration);

		if (isMine)
		{
			rightFoot.enabled = false;
			rightFoot.spatialize = false;
		}
		RandomizeFootstepClip(isMine, leftFoot, ref leftCurrentFootstepClip, ref leftPreviousFootstepClip);

		if (isMine)
		{
			leftFoot.spatialize = true;
			leftFoot.enabled = true;
		}
		leftFoot.PlayOneShot(leftFoot.clip);
		yield return new WaitForSeconds(stepDuration);

		if (isMine)
		{
			leftFoot.enabled = false;
			leftFoot.spatialize = false;
		}

		lastFootstepRoutine = StartCoroutine(PlayFootsteps(isMine));
    }



    void RandomizeFootstepClip(bool isMine, AudioSource audioSource, ref int currentFootstepClip, ref int previousFootstepClip)	//setting some random values for audioclip to make it sound more random. IsMine parameter used for manipulating your characater's volume
    {
        currentFootstepClip = rnd.Next(0, 4);
        if (currentFootstepClip == previousFootstepClip) //making sure the footsteps don't repeat
        {
            currentFootstepClip++;
            currentFootstepClip %= 4;
        }

        audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        audioSource.volume = UnityEngine.Random.Range(0.8f, 1.0f);

		if (isMine)
		{
			audioSource.volume = ownFootstepsVolume;
		}
        audioSource.clip = footsteps[currentFootstepClip];
        previousFootstepClip = currentFootstepClip;
    }

	[PunRPC]
	void PlayFootstepsFunction(bool isMine)	//RPC, but is called also locally. calls coroutine responsible for repeating footsteps.
	{
		lastFootstepRoutine = StartCoroutine(PlayFootsteps(isMine));
	}

	[PunRPC]
	void StopFootstepsFunction()	//RPC, also called locally. stops the last PlayFootsteps coroutine
	{
		StopCoroutine(lastFootstepRoutine);
	}
}
