using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// Game States codes used in events
/// </summary>
public enum GameState {
	Waiting = 0,
	Starting = 1,
	Playing = 2,
	Ending = 3
}

/// <summary>
/// NetworkingScene's Network Manager script handling multiplayer gameplay events
/// </summary>
public class EventManager : MonoBehaviourPunCallbacks, IOnEventCallback {

	#region variables

	// List of players in current room
	public List<ProfileData> playerInfo = new List<ProfileData>();
	// Index of the local player inside the playerInfo list
	public int myIndex;

	// Game state and required kills for the win
	public int requiredKills = 15;
	public GameState state { get; private set; }

	// Game time settings
	// Round time in seconds
	double gameTime = 300.0;
	// Time of the game start received from the server
	double startGameTime;
	// UI object
	GameObject timeText; 

	public UIController ui;
	public GameObject gameOverText; // PlayerCamera/MainCamera/Canvas/GameOver

	#endregion

	#region enums

	public enum EventCodes {
		NewPlayer,
		RemovePlayer,
		UpdatePlayer,
		ChangeStat,
		SetupTime
	}

	public enum StatCodes {
		Actor,
		Kills,
		Deaths
	}

	#endregion

	#region gameplayFunctions

	void Start() {
		// default game state. if the state is different, this player will be notified by the master client after he records new player
		state = GameState.Waiting;
		ValidateConnection();

		// notifies the master client about joining the game
		SendNewPlayer(Launcher.myProfile);
		gameObject.GetComponent<SpawnerManager>().Spawn();

		// time setup
		timeText = ui.transform.Find("HUD/Time").gameObject;
	}

	void Update() {
		// checks for the full room
		StateCheck();
	}

	// Function called when player leaves the room after clicking Quit button in pause menu
	public override void OnLeftRoom() {
		base.OnLeftRoom();
		SceneManager.LoadScene("RoomList");
	}

	// Functions setting this script as the callback target of the custom Events sent via Photon
	private void OnEnable() {
		PhotonNetwork.AddCallbackTarget(this);
	}

	private void OnDisable() {
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Called at the start, moves the player back to the room list if he is not connected to the network
	// Gives effect only in the editor, as in normal gameplay there is no way of reaching Networking Scene 
	// before the RoomList Scene in which the connection takes place
	void ValidateConnection() {
		if (PhotonNetwork.IsConnected) return;
		SceneManager.LoadScene("RoomList");
	}

	// Function executing corresponding methods depending on the game state
	void StateCheck() {
		switch(state) {
			case GameState.Waiting:
				CheckPlayers();
				break;
			case GameState.Starting:
				StartGame();
				break;
			case GameState.Playing:
				UpdateTime();
				break;
			case GameState.Ending:
				EndGame();
				break;
		}
	}

	// Function checking if the room is full, and if yes, clearing the scores and sending the events for game start
	// only Master Client executes interior of this function
	void CheckPlayers() {
		if(PhotonNetwork.IsMasterClient) {
			if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount) {
				foreach (ProfileData p in playerInfo) {
					p.kills = 0;
					p.deaths = 0;
				}
				startGameTime = PhotonNetwork.Time;
				SendSetupTime(startGameTime);
				SendUpdatePlayer((int)GameState.Starting, playerInfo);
			}
		}
	}

	// Function that spawns the player at the start of the round;
	void StartGame() {
		ui.RefreshStats();
		state = GameState.Playing;
		gameObject.GetComponent<SpawnerManager>().Spawn();
	}

	// Function that shows the game over screen
	void EndGame() {
		state = GameState.Ending;
		// if we are the master client, we close the room
		if(PhotonNetwork.IsMasterClient) {
			PhotonNetwork.DestroyAll();
			PhotonNetwork.CurrentRoom.IsVisible = false;
			PhotonNetwork.CurrentRoom.IsOpen = false;
		}

		gameOverText.gameObject.SetActive(true);
		ui.leaderboard.SetActive(true);

		StartCoroutine(End(5f));
	}

	// Function that deals with the round time
	// Prints formated data on the screen and calls EndGame function if the time runs out
	void UpdateTime() {
		double currentTime = gameTime + startGameTime - PhotonNetwork.Time;
		if (currentTime <= 0) {
			EndGame();
		}
		// otherwise countdown
		string timestr;
		double minutes = Mathf.Floor((float)currentTime / 60);
		double seconds = Mathf.RoundToInt((float)currentTime % 60);
		timestr = $"{minutes}:{(seconds < 10 ? "0" + seconds.ToString() : seconds.ToString())}";
		timeText.GetComponent<Text>().text = timestr;
	}

	// Function that checks for the win condition among the players in the player list
	void CheckScores() {
		bool detectWin = false;
		foreach(ProfileData player in playerInfo) {
			if(player.kills >= requiredKills) {
				detectWin = true;
				break;
			}
		}

		if(detectWin) {
			// Only the master client should send the event
			if(PhotonNetwork.IsMasterClient && state != GameState.Ending) {
				SendUpdatePlayer((int)GameState.Ending, playerInfo);
			}
		}
	}

	// Coroutine that waits in the Game Over screen after which causes all players to leave the room
	IEnumerator End(float time) {
		yield return new WaitForSeconds(time);
		PhotonNetwork.LeaveRoom();
	}

	#endregion

	#region events

	// IOnEventCallback interface function called whenever new event is recorded
	// Based on the EventCode, calls corresponding 'Receive' function
	public void OnEvent(EventData photonEvent) {
		// event codes above 200 are reserved by Photon
		if (photonEvent.Code >= 200) return;

		EventCodes eventCode = (EventCodes)photonEvent.Code;
		object[] eventData = (object[])photonEvent.CustomData;

		switch(eventCode) {

			case EventCodes.NewPlayer:
				ReceiveNewPlayer(eventData);
				break;

			case EventCodes.RemovePlayer:
				ReceiveRemovePlayer(eventData);
				break;

			case EventCodes.UpdatePlayer:
				ReceiveUpdatePlayer(eventData);
				break;

			case EventCodes.ChangeStat:
				ReceiveChangeStat(eventData);
				break;

			case EventCodes.SetupTime:
				ReceiveSetupTime(eventData);
				break;
		}
	}

	// Event sent by player joining the room to the master client
	// Data sent is array representation of his ProfileData
	public void SendNewPlayer(ProfileData data) {
		object[] package = new object[4];

		package[0] = data.username;
		package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
		package[2] = (short) data.kills;
		package[3] = (short) data.deaths;

		PhotonNetwork.RaiseEvent(
			(byte)EventCodes.NewPlayer,
			package,
			new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
			new SendOptions { Reliability = true } );
	}

	// Event received by master client ONLY when new player joins the room
	// Data received is array representation of joining player's ProfileData class
	public void ReceiveNewPlayer(object[] data) {
		ProfileData newPlayer = new ProfileData(
			(string)data[0],
			(int)data[1],
			(short)data[2],
			(short)data[3]);

		playerInfo.Add(newPlayer);
		// sends the information to rest of the players
		SendUpdatePlayer((int)state, playerInfo);
	}

	// Event sent by the player who leaves the room, targeted to the master client
	// Data sent is his username and his actor id
	public void SendRemovePlayer(ProfileData data) {
		object[] package = new object[2];

		package[0] = data.username;
		package[1] = PhotonNetwork.LocalPlayer.ActorNumber;

		PhotonNetwork.RaiseEvent(
			(byte)EventCodes.RemovePlayer,
			package,
			new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
			new SendOptions { Reliability = true });
		
		// if we are not master client, we can leave the room
		if (!PhotonNetwork.IsMasterClient) {
			PhotonNetwork.LeaveRoom();
		}
		// if we were the master client, we need to receive the event first, 
		// let other players know that we are leaving and only after that leave the room
	}

	// Event received by master client ONLY when any player leaves the game room
	// Data received is:
	//		username of the player who left
	//		id of the player who left
	public void ReceiveRemovePlayer(object[] data) {
		string username = (string)data[0];
		int actorID = (int)data[1];
		int indexToRemove = 0;

		foreach(ProfileData player in playerInfo) {
			if(player.username == username && player.actorID == actorID) {
				break;
			}
			indexToRemove++;
		}

		playerInfo.RemoveAt(indexToRemove);
		// sends the information to rest of the players
		SendUpdatePlayer((int)state, playerInfo);

		// if leaving player is us, that means we are the master client
		if (actorID == PhotonNetwork.LocalPlayer.ActorNumber) {
			// checks if we were the last one in the room. Closes the room if yes
			if(playerInfo.Count == 0) {
				PhotonNetwork.CurrentRoom.IsVisible = false;
				PhotonNetwork.CurrentRoom.IsOpen = false;
			}
			// proceeds to leave the room. Master client status is automatically passed to another player
			PhotonNetwork.LeaveRoom();
		}
	}

	// Event sent by master client when he records new player in the game
	// Data sent are game state and the list of players in array representation
	public void SendUpdatePlayer(int gameState, List<ProfileData> playersList) {
		object[] package = new object[playersList.Count + 1];

		package[0] = gameState;
		for(int i = 0; i < playersList.Count; i++) {
			object[] singleEntry = new object[4];
			singleEntry[0] = playersList[i].username;
			singleEntry[1] = playersList[i].actorID;
			singleEntry[2] = playersList[i].kills;
			singleEntry[3] = playersList[i].deaths;

			package[i + 1] = singleEntry;
		}

		PhotonNetwork.RaiseEvent(
			(byte)EventCodes.UpdatePlayer,
			package,
			new RaiseEventOptions { Receivers = ReceiverGroup.All },
			new SendOptions { Reliability = true } );
	}

	// Event received by all players when new player joins the game, and the master client sends his info around
	// Data received is a list where:
	//		first element is the game state
	//		following elements are an array representation of player's list
	public void ReceiveUpdatePlayer(object[] data) {
		playerInfo = new List<ProfileData>();

		state = (GameState)data[0];
		for(int i = 1; i < data.Length; i++) {
			object[] singleEntry = (object[])data[i];
			ProfileData player = new ProfileData(
				(string)singleEntry[0],
				(int)singleEntry[1],
				(short)singleEntry[2],
				(short)singleEntry[3]);

			playerInfo.Add(player);
			// setting up own id
			if(PhotonNetwork.LocalPlayer.ActorNumber == player.actorID) {
				myIndex = i - 1;
			}
		}
		// checks for the full room
		StateCheck();
	}

	// Event sent by a player when they score a kill/die
	// Data sent consists of id of the player scoring, what he scores and by what amount
	public void SendChangeStat(int actor, byte stat, byte amount) {
		object[] package = new object[] { actor, stat, amount };

		PhotonNetwork.RaiseEvent(
			(byte)EventCodes.ChangeStat,
			package,
			new RaiseEventOptions { Receivers = ReceiverGroup.All},
			new SendOptions { Reliability= true } );
	}

	// Event received by all players that update player's stats - kills and deaths
	// Data received is:
	//		Id of the player received kill/death
	//		Stat wheter it is kill or death
	//		Amount how much the stat should be increased (always 1 in current gamemode)
	public void ReceiveChangeStat(object[] data) {
		int actor = (int)data[0];
		byte stat = (byte)data[1];
		byte amount = (byte)data[2];

		// looking for the received player id in the list and updating his scores
		for(int i = 0; i < playerInfo.Count; i++) {
			if(playerInfo[i].actorID == actor) {

				switch (stat) {

					case (byte)StatCodes.Kills:
						playerInfo[i].kills += amount;
						Debug.Log($"Player {playerInfo[i].username}: kills = {playerInfo[i].kills}");
						break;

					case (byte)StatCodes.Deaths:
						playerInfo[i].deaths += amount;
						Debug.Log($"Player {playerInfo[i].username}: deaths = {playerInfo[i].deaths}");
						break;
				}

				// if it was the local player, update the UI elements
				if (i == myIndex) ui.RefreshStats();
				// if during receiving the event leaderboard was opened, refresh the leaderboard
				if (ui.leaderboard.activeSelf) ui.RefreshLeaderboard();

				break;
			}
		}
		// Check if someone has won
		CheckScores();
	}

	// Event sent at round start to sync the round time
	// Data sent is the time at which the game starts
	public void SendSetupTime(double startTime) {
		object[] package = new object[] { startTime };

		PhotonNetwork.RaiseEvent(
			(byte)EventCodes.SetupTime,
			package,
			new RaiseEventOptions { Receivers = ReceiverGroup.All },
			new SendOptions { Reliability = true});
	}

	// Event received when room is full and the round is starting
	// Data received is the server time at which the game started
	public void ReceiveSetupTime(object[] data) {
		startGameTime = (double)data[0];
	}

	#endregion
}