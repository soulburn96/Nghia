using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SpawnerManager : MonoBehaviour {
	public PlayerCamera Camera;
	public GameObject[] cornerSpawnPoints;
	public GameObject player;
	public GameObject elementMenu;
	public string element;
	public int playerId { get; private set; }

	void Start() { }

	void OnJoinedRoom() {
		playerId = PhotonNetwork.player.ID;
		Spawn();
	}

	public void Spawn() {
		if(player != null) {
			PhotonNetwork.Destroy(player);
		}
		elementMenu.SetActive(true);
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