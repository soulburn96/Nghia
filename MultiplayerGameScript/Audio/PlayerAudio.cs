using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Class handling player interaction sounds, e.g. idle sounds, receiving damage, etc.
/// </summary>

// Player Audio class is responsible for managing many minor sounds that announce some interaction with player, e.g.: 
//		- HELPER FOR BLIND: hitting a wall, increasing enemy's volume when player can see them,
//		- receiving damage
//		- IDLESOUND: idle sounds ( they are currently placed in Footsteps.cs file, because of reliability issues: read comment over "PlayIdleSounds()" function in Footsteps.cs)
//	This script is attached to every character  /Assets/Resources/*Player
// 
//	RESONANCEAUDIO GLITCH: Another role of this class is to fix problems caused by ResonanceAudio: https://github.com/resonance-audio/resonance-audio-unity-sdk/issues/54


public class PlayerAudio : MonoBehaviour
{
	PlayerController playerController;
	public AudioSource playerMouth;
	public AudioSource debugSource;
	bool isDebugSoundPlayed;

	public AudioClip wallHitSound;
	public AudioClip deathSound;

	public AudioClip ownHitSound0;	// your sound when you receive 0 damage
	public AudioClip ownHitSound;
	public AudioClip enemyHitSound0;    // enemy sound when he receives 0 damage
	public AudioClip enemyHitSound;

	public AudioClip idleSound;
	public GameObject idleSoundSourcePrefab;


	private GameObject idleSourceInstance;
	private Coroutine debugCoroutine;
	private PhotonView photon;
	private GameObject previouslyHitPlayer;
	private bool hasAudioChanged = false;


	void Start()
	{
		playerController = transform.GetComponent<PlayerController>();
		isDebugSoundPlayed = false;
		
		photon = gameObject.GetComponent<PhotonView>();
		if (!photon.IsMine)	//play idle sound of every enemy on the map. called on client side, not on the network.
		{
			PlayIdleSound();
		}
	}

	void Update()
	{

		if (gameObject.GetPhotonView().IsMine)
		{
			if (!playerController.CanMove() && !playerMouth.isPlaying && (Input.GetKey(KeyCode.W)))
			{
				StartCoroutine(PlayWallHit());
			}
			if (!playerController.rotating && !isDebugSoundPlayed)  //RESONANCEAUDIO GLITCH: fixes ResonanceAudio
			{
				isDebugSoundPlayed = true;
				debugCoroutine = StartCoroutine(PlayRotate());
			}
		}

		CheckIfEnemyCanBeShot();
		
	}

	IEnumerator PlayWallHit()	//HELPER FOR BLIND: plays sound when player hits a wall
	{
		playerMouth.spatialize = true;
		playerMouth.enabled = true;
		playerMouth.PlayOneShot(wallHitSound);
		yield return new WaitForSeconds(1.0f);
		playerMouth.enabled = false;
		playerMouth.spatialize = false;
	}

	IEnumerator PlayRotate()    //RESONANCEAUDIO GLITCH: audio fix. Fixes problems with ResonanceAudio. What it does actually is playing muted sound placed on top of player's head. Thanks to this solution, position of the player is updated for ResonanceAudio Listener.
	{
		debugSource.spatialize = true;
		debugSource.enabled = true;
		debugSource.Play();
		while (debugSource.isPlaying)
			yield return new WaitForEndOfFrame();
		debugSource.enabled = false;
		debugSource.spatialize = false;

		isDebugSoundPlayed = false;
	}

	void PlayDeathAudio()	// called by an event in death animation. 
	//NOTE: there might be sometimes problems that at the time when player is killed during movement animation, the death animation will not played. audio won't be played as well. I am editing this code quite late, so animation guy does not work on the game anymore.
	{
		playerMouth.PlayOneShot(deathSound);
	}

	public void PlayHitSoundRPC(int damage, int playerHitId)	//handles playing sounds of player being hit. 
	{
		PhotonView photon = gameObject.GetComponent<PhotonView>();
		PlayerStat stats = gameObject.GetComponent<PlayerStat>();
		if (!((stats.currentHealth - damage) <= 0))
		{
			photon.RPC("PlayHitSound", RpcTarget.Others, damage, playerHitId);
			if (damage == 0)
			{
				playerMouth.PlayOneShot(ownHitSound0);
			}
			else
			{
				playerMouth.PlayOneShot(ownHitSound);
			}
		}
	}

	[PunRPC]
	void PlayHitSound(int damage, int playerHitId) // called from "PlayHitSoundRPC" as literal (actually works so-so, i think it can be better to rewrite it so it will be performed on client side rather than waiting to receive RPC)
	{
		if (damage == 0)
		{
			playerMouth.PlayOneShot(enemyHitSound0);
		}
		else
		{
			playerMouth.PlayOneShot(enemyHitSound);
		}
	}

	void PlayIdleSound() //IDLESOUND
	{
		idleSourceInstance = Instantiate(idleSoundSourcePrefab, gameObject.transform);
		AudioSource source = idleSourceInstance.GetComponent<AudioSource>();
		source.clip = idleSound;
		source.Play();
	}

	void CheckIfEnemyCanBeShot()    //HELPER FOR BLIND: If enemy is standing in your sight (or on your sides/back), their Idle Loop sound will be played louder. May need adjustments,
		//e.g. how to manage if player see more than one player at once? 
		//For now it only increases volume of the first player it will spot in order: front, right, left, back.
	{
		Ray shotRayForward = new Ray(transform.position + new Vector3(0, 0.5f, 0), transform.forward);
		Ray shotRayLeft = new Ray(transform.position + new Vector3(0, 0.5f, 0), -transform.right);
		Ray shotRayRight = new Ray(transform.position + new Vector3(0, 0.5f, 0), transform.right);
		Ray shotRayBack = new Ray(transform.position + new Vector3(0, 0.5f, 0), -transform.forward);
		Ray[] rays = new Ray[4] { shotRayForward, shotRayRight, shotRayLeft, shotRayBack };
		RaycastHit hit;
		int dontHitSwooshTriggersLayerMask = (1 << 11) | (1 << 10) | (1 << 9); 
		dontHitSwooshTriggersLayerMask = ~dontHitSwooshTriggersLayerMask;   //don't hit swoosh triggers, spells and local player

		//Debug.DrawRay(shotRay.origin, shotRay.direction, Color.red);
		for (int i = 0; i < 4; i++)	//search for player in every direction
		{
			if (Physics.Raycast(rays[i], out hit, Mathf.Infinity, dontHitSwooshTriggersLayerMask))  //For now it only increases volume of the first player it will spot in order: front, right, left, back.
			{
				if (hit.collider.tag == "Player")	//if found a player
				{
					if (hit.collider.gameObject != previouslyHitPlayer)	//if this player is someone else than player hit by raycast in previous frame
					{
						if (hasAudioChanged == false)	// if this player's audio has not been changed yet 
						{
							//Debug.Log("setting minDistance!!!");
							GameObject soundObject = hit.collider.transform.Find("IdleSound SoundSource(Clone)").gameObject;
							AudioSource source = soundObject.GetComponent<AudioSource>();
							source.rolloffMode = AudioRolloffMode.Custom;
							hasAudioChanged = true;
							previouslyHitPlayer = hit.collider.gameObject;
						}
						else // if this player is already loud
						{
							detachPreviouslyHitPlayer();
						}
					}
					else // if this player is the same player as the one hit in previous frame
					{
						Debug.Log("it's the same player!!!");
						return;
					}
					break;
				}
				else    //if raycast hits wall or something else
				{
					detachPreviouslyHitPlayer();
				}
			}
		}
	}

	void detachPreviouslyHitPlayer()	// if other player steps between you and enemy whose Idle Loop is loud now, the sound of previous enemy will be set to previous state before new enemy becomes loud.
	{
		if (previouslyHitPlayer != null)
		{
			//Debug.Log("REsetting previous player's minDistance!!!");
			previouslyHitPlayer.transform.Find("IdleSound SoundSource(Clone)").gameObject.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Logarithmic;
		}
		previouslyHitPlayer = null;
		hasAudioChanged = false;
	}
}
