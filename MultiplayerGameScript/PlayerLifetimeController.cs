using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifetimeController : MonoBehaviour, IPunObservable {

	// character statistics data
	PlayerStat stats;
	// spawn data
	SpawnerManager spawner;
	Transform initialSpawnPoint;
    void Start() {
		spawner = GameObject.Find("Network Manager").GetComponent<SpawnerManager>();
		initialSpawnPoint = spawner.pickInitialSpawnPoint();
		stats = gameObject.GetComponent<PlayerStat>();
		if (gameObject.GetComponent<PhotonView>().isMine) {
			gameObject.layer = 9; // local player layer
		}
	}

	void LateUpdate() {
		if (Input.GetKey(KeyCode.R)) {
			spawner.Spawn();
		}
	}
	
	[PunRPC]
	void receiveDamage(int damage, int playerHitId) {
		if(playerHitId == gameObject.GetComponent<PhotonView>().ownerId) {
			stats.currentHealth -= damage;
			if (stats.currentHealth <= 0) {
				Debug.Log("dead");
				spawner.Spawn();
				//above line should send another message to spawn only hit player, not all players.
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo messageInfo) {
		if(stream.isWriting) {
			stream.SendNext(stats.currentHealth);
		}
		else {
			this.stats.currentHealth = (int)stream.ReceiveNext();
		}
	}
}
