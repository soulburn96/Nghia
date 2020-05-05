using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PausemenuScript : MonoBehaviour {

	public static bool GameIsPaused = false;
	public GameObject pauseMenuUI;
	public GameObject leaderboard;
	public GameObject options;

	private void Start() {
		GameIsPaused = false;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (GameIsPaused) {
				Resume();
			}
			else {
				Pause();
			}
		}
	}

	public void Resume() {
		pauseMenuUI.SetActive(false);
		leaderboard.SetActive(false);
		options.SetActive(false);
		GameIsPaused = false;
	}

	public void Pause() {
		pauseMenuUI.SetActive(true);
		leaderboard.SetActive(false);
		options.SetActive(false);
		GameIsPaused = true;
	}

	public void LoadMenu() {
		GameObject.Find("Network Manager").GetComponent<EventManager>().SendRemovePlayer(Launcher.myProfile);
	}

}
