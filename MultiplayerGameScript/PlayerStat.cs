using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

/// <summary>
/// Class representing elemental's statistics, attached to the elemental prefab.
/// </summary>
public class PlayerStat : MonoBehaviour {

	#region variables

	// Graphical text objects of UI elements
	public Text hpIndicator;                // PlayerCamera/MainCamera/Canvas/HUD/HpIndicator
	public Text usernameText;               // PlayerCamera/MainCamera/Canvas/HUD/Username
	public TextMeshPro playerUsernameText;  // On top of player prefab in Resources

	/// <summary>
	/// Copy of players Profile Data from RoomList/Launcher script's static field.
	/// </summary>
	public ProfileData playerProfile;

	// Intialized in player prefabs elementals stats
	public string elementName;
	public int currentHealth;
	public int maxHp;
	public int hpRegen;
	public float speed;
	public int spellCooldown;
	public string strongerElement;
	public string weakerElement;

	#endregion

	// Set default values
	void Awake() {
		currentHealth = maxHp;
		hpIndicator = GameObject.Find("HpIndicator").GetComponent<Text>();
		usernameText = GameObject.Find("Username").GetComponent<Text>();
		if (gameObject.GetPhotonView().IsMine) {
			usernameText.text = Launcher.myProfile.username;
			gameObject.GetComponent<PhotonView>().RPC("SyncProfile", RpcTarget.AllBuffered,
				Launcher.myProfile.username, Launcher.myProfile.actorID, Launcher.myProfile.kills, Launcher.myProfile.deaths);
		}
	}

	// Updates current health points UI element
	void Update() {
		if (gameObject.GetPhotonView().IsMine) {
			hpIndicator.text = currentHealth.ToString();
		}
	}

	//Sync player profile in network
	[PunRPC]
	private void SyncProfile(string username, int actorID, short kills, short deaths) {
		playerProfile = new ProfileData(username, actorID, kills, deaths);
		playerUsernameText.text = playerProfile.username;
	}

	// Regenerates player HP
	public void regenHealth() {
		if (currentHealth < maxHp) {
			currentHealth += hpRegen;
		}
	}
}
