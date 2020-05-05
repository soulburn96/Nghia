using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///	Class responsible for managing sounds in maze called "swooshes". 
/// </summary> 

// These sounds are necessary for blind people to know which way they can turn while standing on intersection of maze.
// Script is placed on SwooshTrigger prefab in		/Assets/Prefabs/Audio
// the role of Swoosh Triggers is to spawn Swooshes prefab from	/Assets/Prefabs/Audio  , which will play the sounds in constant order dependent on player's rotation:
//		1. front swoosh, 2. right and left swooshes at once, 3. back swoosh
//		this class contains methods responsible for choosing the order of swooshes, correctly placing audiosources and playing them while triggered.

public class Swooshes : MonoBehaviour
{
	public GameObject swooshesPrefab;
	GameObject swooshesInstance;
	AudioSource sourceF;
	AudioSource sourceR;
	AudioSource sourceL;
	AudioSource sourceB;

	bool[] booleanTableOfWalls = new bool[4]; //in order: 0:front, 1:right, 2:left, 3:back
	AudioSource[] sources;

	BoxCollider boxCollider;
	Rigidbody rigid_body;
	Collider playerCollider;
	float interval = 0.2f;


	void Start()
	{
		boxCollider = gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;	//adding box collider to object.
		transform.position = transform.parent.position;	// displacing the SwooshTrigger prefab
		boxCollider.isTrigger = true;
		boxCollider.size = new Vector3(0.5f, 1.0f, 0.5f);

		rigid_body = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
		rigid_body.useGravity = false;
	}

	private void OnTriggerEnter(Collider other)	// starts when player enters the trigger
	{
		if (other.gameObject.name == "Player")
		{
			playerCollider = other;

			CheckOrderOfPlayingSwooshes();
			ControlAudio();
		}
	}

	void ControlAudio()
	{
		StartCoroutine(SwooshCoroutine());
	}

	IEnumerator SwooshCoroutine()	//plays swooshes in order
	{
		sourceF.Play();
		yield return new WaitForSeconds(interval);
		sourceR.Play();
		sourceL.Play();
		yield return new WaitForSeconds(interval);
		sourceB.Play();
		yield return new WaitForSeconds(interval);
	}


	void CheckOrderOfPlayingSwooshes()	//HELPER FOR BLIND: checks direction of your player and based on it chooses the order of playing swooshes.
	{
		Vector3[] directions = { Vector3.forward, Vector3.right, Vector3.left, Vector3.back };	// get global direction vectors
		Quaternion playerRotation = playerCollider.gameObject.transform.rotation;	// get player direction
		int i;
		for (i = 0; i < 4; i++) booleanTableOfWalls[i] = false;	// set all booleans in table to false
		i = 0;
		foreach (Vector3 direction in directions)
		{
			Vector3 directionAfterRotation = playerRotation * direction;	// multiplication of quaternion and vector returns direction vector of player.
																			// we want to throw raycast in calculated direction to check if there is a wall. 
			if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), directionAfterRotation, 1.0f))	// todo: add layer mask so it won't collide with anything else than walls
			{
				booleanTableOfWalls[i] = true; // true means there is a wall.
			}
			i++;
		}

		swooshesInstance = Instantiate(swooshesPrefab, gameObject.transform.position, playerRotation, gameObject.transform);
		sourceF = swooshesInstance.transform.Find("Swoosh Source Front").GetComponent<AudioSource>();
		sourceR = swooshesInstance.transform.Find("Swoosh Source Right").GetComponent<AudioSource>();
		sourceL = swooshesInstance.transform.Find("Swoosh Source Left").GetComponent<AudioSource>();
		sourceB = swooshesInstance.transform.Find("Swoosh Source Back").GetComponent<AudioSource>();

		sources = new AudioSource[] { sourceF, sourceR, sourceL, sourceB };
		ModifySounds();
	}

	void ModifySounds() // mutes all swoosh sound sources in directions where there are walls; if you have wall in front of you, the source in front of you will be muted.
	{
		for (int i = 0; i < 4; i++)
		{
			if (booleanTableOfWalls[i] == true)
				sources[i].volume = 0.00f;
		}
	}
}
