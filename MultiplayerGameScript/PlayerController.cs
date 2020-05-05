using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Class handling player's controls and elemental's lifetime
/// </summary>
public class PlayerController : MonoBehaviour {

	#region variables
	
	/// <summary>
	/// Network Manager Object's event manager.
	/// </summary>
	EventManager eventManager;

	/// <summary>
	/// Network Manager Object's spawn manager.
	/// </summary>
	SpawnerManager spawner;

	// player direction definitions
	Vector3 nextPos;                    // tile direction base on player position
	Vector3 destination;                // player's next position on the map
	public Vector3 currentDirection;    // current player's direction 

	/// <summary>
	/// Spell objects dictionary. Key: spell name. Value: spell prefab from Resources/Spells set in the inspector.
	/// </summary>
	Dictionary<string, GameObject> spells;

	// Spells prefabs
	public GameObject spellFire;
	public GameObject spellWater;
	public GameObject spellLighting;
	public GameObject spellEarth;

	/// <summary>
	/// This elemental's stats object set in the inspector.
	/// </summary>
	public PlayerStat playerStat;

	/// <summary>
	/// This elemental's animator component;
	/// </summary>
	Animator anim;

	// Values used in player's handlingEd
	bool spellReady = true;         // boolean indicating that it is possible to cast spell
	bool canAttack = true;          // boolean to check if player can attack
	bool canRotate = true;          // boolean to check if player can rotate
	public bool rotating = false;   // boolean to check if player is rotating at this moment
	public bool isDead = false;     // boolean to check if player is dead
	public float stepDuration;      // value calulated from playerStats speed based on duration of each step taken
	public Coroutine playerMovement;// coroutine that handles movement
	public Coroutine spellMovement; // coroutine that handle spellMovement

	#endregion

	void Start() {
		// Default values setup
		nextPos = Vector3.forward;
		currentDirection = transform.rotation.eulerAngles;
		destination = transform.position;
		anim = GetComponent<Animator>();
		eventManager = GameObject.Find("Network Manager").GetComponent<EventManager>();
		spawner = GameObject.Find("Network Manager").GetComponent<SpawnerManager>();
		stepDuration = playerStat.speed;

		// setup local player layering
		if (gameObject.GetPhotonView().IsMine) {
			gameObject.layer = 9; // local player layer
		}

		// spells Dictionary setup
		spells = new Dictionary<string, GameObject>(4);
		spells.Add(spellFire.name, spellFire);
		spells.Add(spellWater.name, spellWater);
		spells.Add(spellLighting.name, spellLighting);
		spells.Add(spellEarth.name, spellEarth);
	}

	void Update() {
		// If object does not belong to me, quit execution
		if (!gameObject.GetPhotonView().IsMine) return;

		// If game is paused, do nothing with the player
		if (!PausemenuScript.GameIsPaused) {
			ControlMovement();
			CastSpell();
		}
	}

	// 'W' key, when player is on normal state (not attacking nor dead nor rotating), moves player 1 tile unit forward
	// 'A', 'S', 'D' rotate player, respectively, 90 - 180 - 270 degrees;
	void ControlMovement() {
		if (Input.GetKey(KeyCode.W)) {
			if (anim.GetBool("Attacking") || isDead) { // check for the state of the player
				return;
			}
			else { // if able to move then handle movement
				if (playerMovement == null && !rotating) {
					playerMovement = StartCoroutine(Move(newPosition(currentDirection)));
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.S)) {
			if (canRotate && !rotating) {
				currentDirection.y += 180;
				if (currentDirection.y >= 360) {
					currentDirection.y -= 360;
				}
				StartCoroutine(Rotate(QuaternionAngle(), 0.3f));
			}
		}
		if (Input.GetKeyDown(KeyCode.D)) {
			if (canRotate && !rotating) {
				currentDirection.y += 90;
				if (currentDirection.y >= 360) {
					currentDirection.y -= 360;
				}
				StartCoroutine(Rotate(QuaternionAngle(), 0.3f));
			}
		}
		if (Input.GetKeyDown(KeyCode.A)) {
			if (canRotate && !rotating) {
				currentDirection.y += 270;
				if (currentDirection.y >= 360) {
					currentDirection.y -= 360;
				}
				StartCoroutine(Rotate(QuaternionAngle(), 0.3f));
			}
		}
	}

	// Return quaternion from player rotation angle
	Quaternion QuaternionAngle() {
		float radians = currentDirection.y * Mathf.Deg2Rad;
		Vector3 normalized = currentDirection.normalized * Mathf.Sin(radians / 2);
		return new Quaternion(normalized.x, normalized.y, normalized.z, Mathf.Cos(radians / 2));

	}

	// Coroutine that handles movement to given direction
	IEnumerator Move(Vector3 nextTile) {
		if (CanMove()) {	
			// set animation to walking
			anim.SetInteger("Condition", 1);

			// each step regens player's HP
			playerStat.regenHealth();

			// setup position for movement
			Vector3 startPosition = transform.position;
			Vector3 destinationPosition = transform.position + nextTile;
			float elapsedTime = 0.0f;

			// while moving can't attack or rotate
			canAttack = false;
			canRotate = false;

			// lerping movement overtime
			while (elapsedTime < 1.0f) {
				transform.position = Vector3.Lerp(startPosition, destinationPosition, elapsedTime);
				elapsedTime += Time.deltaTime / stepDuration;
				yield return new WaitForEndOfFrame();
			}

			// set values to default
			transform.position = destinationPosition;
			CancelAnimation();
			canAttack = true;
			canRotate = true;
			playerMovement = null;
		}
	}

	// Based on the given rotation returns the Vector3 corresponging to the next tile
	public Vector3 newPosition(Vector3 currentDirection) {
		if (currentDirection.y == 0) {
			return Vector3.forward;
		}
		if (currentDirection.y == 90) {
			return Vector3.right;
		}
		if (currentDirection.y == 270) {
			return Vector3.left;
		}
		if (currentDirection.y == 180) {
			return Vector3.back;
		}
		return Vector3.zero;
	}

	// Handles spells casting
	void CastSpell() {
		if (canAttack && spellReady && !isDead & !rotating) {
			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				// Send RPC of spell casting
				this.gameObject.GetComponent<PhotonView>().RPC("CastSpellRPC", RpcTarget.All, spellFire.name,
					transform.position + spellOffset(currentDirection), transform.rotation, this.gameObject.GetPhotonView().Owner.ActorNumber);
				// Handle casting Animation and cooldown for spell
				spellMovement = StartCoroutine(CorountineCastSpell(playerStat.spellCooldown));
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow)) {
				this.gameObject.GetComponent<PhotonView>().RPC("CastSpellRPC", RpcTarget.All, spellWater.name,
					transform.position + spellOffset(currentDirection), transform.rotation, this.gameObject.GetPhotonView().Owner.ActorNumber);
				spellMovement = StartCoroutine(CorountineCastSpell(playerStat.spellCooldown));
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow)) {
				this.gameObject.GetComponent<PhotonView>().RPC("CastSpellRPC", RpcTarget.All, spellLighting.name,
					transform.position + spellOffset(currentDirection), transform.rotation, this.gameObject.GetPhotonView().Owner.ActorNumber);
				spellMovement = StartCoroutine(CorountineCastSpell(playerStat.spellCooldown));
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				this.gameObject.GetComponent<PhotonView>().RPC("CastSpellRPC", RpcTarget.All, spellEarth.name,
					transform.position + spellOffset(currentDirection), transform.rotation, this.gameObject.GetPhotonView().Owner.ActorNumber);
				spellMovement = StartCoroutine(CorountineCastSpell(playerStat.spellCooldown));
			}
		}
	}

	// Spell cast position for instantite based on player
	Vector3 spellOffset(Vector3 currentDirection) {
		return new Vector3(newPosition(currentDirection).x, newPosition(currentDirection).y + 0.7f, newPosition(currentDirection).z);
	}

	// Set animation to default value
	void CancelAnimation() {
		anim.SetBool("Attacking", false);
		anim.SetInteger("Condition", 0);
	}

	// Corouting handling attack animation
	IEnumerator CorountineCastSpell(int cooldownTime) {
		spellReady = false;
		anim.SetInteger("Condition", 3);
		anim.SetBool("Attacking", true);
		yield return new WaitForSeconds(1.2f);
		CancelAnimation();
		yield return new WaitForSeconds(cooldownTime);
		spellMovement = null;
		spellReady = true;
	}

	// Check if any obstacle in the next tile
	public bool CanMove() {
		Ray myRay = new Ray(transform.position + new Vector3(0, 0.25f, 0), transform.forward);
		RaycastHit hit;
		Debug.DrawRay(myRay.origin, myRay.direction, Color.red);

		if (Physics.Raycast(myRay, out hit, 1f)) {
			if (hit.collider.tag == "Wall") {
				return false;
			}
			if (hit.collider.tag == "Player") {
				return false;
			}
		}
		return true;
	}

	// Player receives damage and sends event in case he died
	public void ReceiveDamage(int damage, int casterId) {
		playerStat.currentHealth -= damage;
		if (playerStat.currentHealth <= 0) {
			eventManager.SendChangeStat(casterId, (byte)EventManager.StatCodes.Kills, 1);
			eventManager.SendChangeStat(gameObject.GetPhotonView().Owner.ActorNumber, (byte)EventManager.StatCodes.Deaths, 1);
			playerStat.currentHealth = 0;
			Die();
			StartCoroutine(SpawnCoroutine());
		}
	}

	// Corouting that handles rotation
	IEnumerator Rotate(Quaternion targetRotation, float duration) {
		if (rotating) {
			yield break;
		}
		rotating = true;
		anim.SetInteger("Condition", 1);    //RESONANCEAUDIO GLITCH: fix for resonance audio bug
		float elapsedTime = 0f;
		while (elapsedTime < duration) {
			elapsedTime += Time.deltaTime;
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsedTime / duration);
			yield return null;
		}
		CancelAnimation();  //RESONANCEAUDIO GLITCH: fix for resonance audio bug
		transform.position = transform.position + new Vector3(0f, 0f, 0f);      //RESONANCEAUDIO GLITCH: fix for resonance audio bug
		rotating = false;

	}

	// Coroutine after player getting kill
	IEnumerator SpawnCoroutine() {
		yield return new WaitForSeconds(2);
		spawner.Spawn();
	}

	// Change player state to die and animate it
	public void Die() {
		anim.SetInteger("Condition", 4);
		isDead = true;
	}
	
	// Spell Cast Remote Procedure Call
	[PunRPC]
	void CastSpellRPC(string spellName, Vector3 startPoint, Quaternion goToRotation, int casterID) {
		GameObject spell = Instantiate(spells[spellName], startPoint, goToRotation);
		spell.GetComponent<SpellMovement>().casterId = casterID;
	}
}
