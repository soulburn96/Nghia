using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Class controling Menu option in RoomList game Scene
/// </summary>
public class MultiplayerMenu : MonoBehaviourPunCallbacks {

	#region references
	// References to various game objects that this class controls

	public Launcher launcher;
	public InputField usernameField;

	public GameObject mainView;
	public GameObject roomListView;
	public GameObject createRoomView;

	public GameObject roomButton;
	public GameObject joinRoomButton;

	public GameObject createRoomName;
	public GameObject roomMaxPlayers;

	List<RoomInfo> roomList;
	Transform selectedRoom;
	string selectedRoomName;
	
	#endregion

	private void Awake() {
		if(Launcher.myProfile.username != null) {
			usernameField.text = Launcher.myProfile.username;
		}

		roomListView.SetActive(false);
		joinRoomButton.GetComponent<Button>().interactable = false;
	}

	#region buttons

	public void JoinRandom() {
		launcher.Join();
	}

	public void RoomList() {
		mainView.SetActive(false);
		roomListView.SetActive(true);
	}

	public void CreateRoom() {
		createRoomView.SetActive(true);
		roomListView.SetActive(false);
	}

	public void BackFromRoomList() {
		mainView.SetActive(true);
		roomListView.SetActive(false);
	}

	public void BackFromMain() {
		SceneManager.LoadScene("Tutorial");
	}

	public void BackFromCreateRoom() {
		createRoomView.SetActive(false);
		roomListView.SetActive(true);
	}

	#endregion

	#region roomList

	void ClearRoomList() {
		Transform content = roomListView.transform.Find("Anchor/Scroll View/Viewport/Content");
		foreach (Transform t in content) Destroy(t.gameObject);
	}

	// Photon Callback from lobby's updating room list.
	public override void OnRoomListUpdate(List<RoomInfo> roomList) {
		this.roomList = roomList;
		ClearRoomList();

		Transform content = roomListView.transform.Find("Anchor/Scroll View/Viewport/Content");
		foreach(RoomInfo r in this.roomList) {
			GameObject newRoomButton = Instantiate(roomButton, content) as GameObject;
			newRoomButton.transform.Find("Name").GetComponent<Text>().text = r.Name;
			newRoomButton.transform.Find("Players").GetComponent<Text>().text = r.PlayerCount + " / " + r.MaxPlayers;

			newRoomButton.GetComponent<Button>().onClick.AddListener(delegate { SelectRoom(newRoomButton.transform); });
		}

		base.OnRoomListUpdate(roomList);
	}

	// Highlight of the button during deselection doesnt work properly
	void SelectRoom(Transform room) {
		if (selectedRoom != null) {
			selectedRoom.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 125);
		}
		selectedRoom = room;
		selectedRoom.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 200);
		selectedRoomName = room.Find("Name").GetComponent<Text>().text;

		joinRoomButton.GetComponent<Button>().interactable = true;
		joinRoomButton.GetComponent<Button>().onClick.RemoveAllListeners();
		joinRoomButton.GetComponent<Button>().onClick.AddListener(() => launcher.JoinRoom(selectedRoomName));
	}

	#endregion
}
