using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// Class controlling UI elements during gameplay.
/// </summary>
public class UIController : MonoBehaviour {

	#region variables
	// References to various UI elements this class is controlling

	public EventManager eventManager;

	public GameObject leaderboard;
	public GameObject leaderboardEntry;
	public Transform leaderboardContent;

	public GameObject playerStats;
	public Text kills;
	public Text deaths;

	public Text pingCounter;

	GameObject localPlayer;
	
	#endregion

	private void Start() {
		RefreshStats();
		RefreshLeaderboard();
		StartCoroutine(PingUpdater());
	}

	void Update() {
		if(!PausemenuScript.GameIsPaused && eventManager.state != GameState.Ending) {
			if (Input.GetKeyDown(KeyCode.Tab)) {
				if(!PausemenuScript.GameIsPaused) {
					if (leaderboard.activeSelf) {
						leaderboard.SetActive(false);
					}
					else {
						RefreshLeaderboard();
						leaderboard.SetActive(true);
					}
				}
			}
		}
		RefreshStats();
		RotatePlayerNames();
	}

	void RotatePlayerNames() {
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject p in players) {
			if(p.layer == 9 /* LOCAL PLAYER */) {
				localPlayer = p;
			}
		}
		foreach (GameObject p in players) {
			if (p.layer == 9 /* LOCAL PLAYER */) continue;
			if(localPlayer != null) {
				RectTransform t = p.transform.Find("Text (TMP)").GetComponent<RectTransform>();
				t.LookAt(localPlayer.transform.position);
				t.Rotate(-t.localRotation.x, t.localRotation.y + 180, -t.localRotation.z, Space.Self);
			}
		}
	}

	// On screen K/D counter
	public void RefreshStats() {	// count > 0, myindex >= 0
		if (eventManager.playerInfo.Count > eventManager.myIndex) {
			kills.text = $"{eventManager.playerInfo[eventManager.myIndex].kills} kills";
			deaths.text = $"{eventManager.playerInfo[eventManager.myIndex].deaths} deaths";
		}
		else {
			kills.text = "0 kills";
			deaths.text = "0 deaths";
		}
	}

	public void RefreshLeaderboard() {
		// clear data
		foreach (Transform t in leaderboardContent) Destroy(t.gameObject);

		// setup new data
		List<ProfileData> tempList = SortList(eventManager.playerInfo);
		int i = 0;
		foreach (ProfileData player in eventManager.playerInfo) {
			i++;
			GameObject entry = Instantiate(leaderboardEntry, leaderboardContent) as GameObject;
			entry.transform.Find("Position/Text").GetComponent<Text>().text = i.ToString();
			entry.transform.Find("Name/Text").GetComponent<Text>().text = player.username;
			entry.transform.Find("Kills/Text").GetComponent<Text>().text = $"{player.kills}";
			entry.transform.Find("Deaths/Text").GetComponent<Text>().text = $"{player.deaths}";
		}
	}

	List<ProfileData> SortList(List<ProfileData> o) {
		// s - sorted, o - original, tmp - temporary value, p - player
		List<ProfileData> s = new List<ProfileData>();
		for (int i = 0; i < o.Count; i++) {
			for (int j = i; j < o.Count; j++) {
				if (o[i].kills < o[j].kills) {
					ProfileData tmp = new ProfileData(o[i]);
					o[i] = o[j];
					o[j] = tmp;
				}
			}
		}
		foreach (ProfileData p in o) {
			s.Add(p);
		}
		return s;
	}

	IEnumerator PingUpdater() {
		yield return new WaitForSeconds(1f);
		UpdatePingCounter();
		StartCoroutine(PingUpdater());
	}

	void UpdatePingCounter() {
		int ping = PhotonNetwork.GetPing();
		pingCounter.text = $"Ping: {ping}ms";
	}
}
