using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TEST PURPOSES. handles the switch which you can see in Options menu.
/// </summary>

public class ToggleOwnFootsteps : MonoBehaviour
{
	GameObject player;

	private void Start()
	{
		player = GameObject.Find("Player");
		Debug.Log(player.name);
	}

	public void TogglePlayerFootsteps(bool toggleValue)
	{
		player.GetComponent<Footsteps>().toggleOwnFootsteps = toggleValue;
	}
}
