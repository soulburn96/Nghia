using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpellHitDestroyer : MonoBehaviour {

	private void Start() {
		Invoke("DestroyThis", gameObject.GetComponentInChildren<ParticleSystem>().main.duration);
	}

	void DestroyThis() {
		Destroy(gameObject);
	}
}
