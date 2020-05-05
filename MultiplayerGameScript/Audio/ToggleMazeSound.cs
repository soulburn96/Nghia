using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TEST PURPOSES. handles the switch which you can see in Options menu.
/// </summary>
public class ToggleMazeSound : MonoBehaviour
{
	public AudioSource mazeSource;

	public void ToggleSound(bool toggleValue)
	{
		mazeSource.enabled = toggleValue;
	}
}
