using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Class that spawns the players in the map. Is attached to Network Manager Object.
/// </summary>
public class SpawnerManager : MonoBehaviour {

	#region variables

	public PlayerCamera Camera;
	public GameObject[] cornerSpawnPoints;	// IntialSpawnPoints in NetworkingScene. Spawn point in corner of the map
	public GameObject player;				// reference to the player object
	public GameObject elementMenu;			// ElementChoosingMenu in NetworkingScene
	public GameObject HUD;					// PlayerCamera/MainCamera/Canvas/HUD
	public string element;					// spawning element name
	public int playerId { get; private set; }	// network ID of the player later changed to spawn corner position

	#endregion

	public void Spawn() {
		// from this variable spawn point is picked, it is always evaluated to 1, 2, 3 or 4, depending on actual network ID
		playerId = ((PhotonNetwork.LocalPlayer.ActorNumber - 1) % 4) + 1;
		
		if (player != null) {
			PhotonNetwork.Destroy(player);
		}
		HUD.SetActive(false);
		elementMenu.SetActive(true);
	}

	public void SetupElementName(string elementName) {
		Debug.Log(elementName);
		element = elementName;
		HUD.SetActive(true);
		CreatePlayerObject();
	}

	public void CreatePlayerObject() {
		Transform spawnPoint = pickInitialSpawnPoint();
		player = PhotonNetwork.Instantiate(element + "Player", spawnPoint.position, spawnPoint.rotation, 0);
		player.name = "Player";
		player.layer = 8; // players layer
		Camera.Target = player;
	}

	public Transform pickInitialSpawnPoint() {
		return cornerSpawnPoints[playerId].transform;
	}	
}