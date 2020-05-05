using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Class handling spell movement and collisions
/// </summary>
public class SpellMovement : MonoBehaviour {

	#region variables

	// flight speed
	public float speed;

	// spell element name
	public string spellElement;

	// spell damage default
	int damage = 50;

	// Networking id (photonID) of the caster of this spell
	public int casterId;

	// Spell Hit particle effects from Resources/Spells
	public GameObject spellHitParticles;

	// Limiters offset used in emergency destroy
	float LIMITERS_POSITION = 0.7f;

	// internal collision counter
	int counter = 0;

	#endregion

	// Moves the spell forward
	void Update() {
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
		// destroy if off map (above or below) - Might be redundant after fixing spell instantiation
		if (transform.position.y > LIMITERS_POSITION + 1 || transform.position.y < LIMITERS_POSITION - 1) {
			Destroy(gameObject);
		}
	}

	// collision callback
	private void OnCollisionEnter(Collision collision) {
		if (counter == 0) {
			counter++;
			//if spell collide with player, wall or limiter
			if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Limiter")) {
				// if collide with local player, deal damage and destroy, otherwise just destroy
				if (collision.collider.gameObject == GameObject.Find("Player")) {
					PlayerStat hitStat = collision.collider.GetComponent<PlayerStat>();
					collision.collider.GetComponent<PlayerAudio>().PlayHitSoundRPC(DamageCalculate(hitStat), collision.collider.gameObject.GetPhotonView().Owner.ActorNumber);
					collision.collider.GetComponent<PlayerController>().ReceiveDamage(DamageCalculate(hitStat), casterId);
				}
				GameObject spellhit = Instantiate(spellHitParticles, gameObject.transform.position, gameObject.transform.rotation);
				Destroy(gameObject);
			}
		}
	}

	// caculate damage base on collided player
	int DamageCalculate(PlayerStat playerStat) {
		if (spellElement == playerStat.strongerElement) {
			return damage * 2;
		}
		else if (spellElement == playerStat.weakerElement) {
			return 0;
		}
		else {
			return damage;
		}
	}

}
