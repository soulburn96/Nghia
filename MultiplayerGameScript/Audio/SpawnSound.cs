using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the SpawnSound soundsource when a character(player) spawns.
/// </summary>
// Attached to every character.

public class SpawnSound : MonoBehaviour
{
	public GameObject sourcePrefab;	// prefab of SpawnSound soundsource
	GameObject instance;

	void Start()
    {
		instance = Instantiate(sourcePrefab, gameObject.transform);
	}


    void Update()
    {
		if (instance != null)
		{
			if (instance.GetComponent<AudioSource>().isPlaying == false)	// destroy soundsource after playing spawn sound
			{
				Destroy(instance);
			}
		}
    }
}
