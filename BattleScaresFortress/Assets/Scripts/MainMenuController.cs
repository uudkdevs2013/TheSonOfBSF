using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
	
	private string _roomName = "";
	
	private string _ipAddress = "192.168.1.143";
	private string _port = "5055";
	private string _playerName = "player";
	
	private void OnGUI()
	{
		if (MatchMakingController.Instance.CurrentStatus == MatchMakingStatus.NotConnected)
		{
			int w = (int)(Screen.width * 0.25f);
			int h = (int)(Screen.height * 0.25f + 100);
			
			// connect to Photon Cloud servers in Europe
			if (GUI.Button(new Rect(w - 50, h - 15, 200, 30), "Connect to Photon Cloud"))
			{
				MatchMakingController.Instance.ConnectToRemoteServers(_playerName);
			}
			
			// connect to custom server
			GUI.Label(new Rect(w - 50, h + 70, 70, 20), "IP Address:");
			_ipAddress = GUI.TextField(new Rect(w + 30, h + 70, 110, 20), _ipAddress, 15);
			GUI.Label(new Rect(w - 8, h + 100, 28, 20), "Port:");
			_port = GUI.TextField(new Rect(w + 30, h + 100, 50, 20), _port, 5);
			if (GUI.Button(new Rect(w - 50, h + 130, 190, 30), "Connect to custom server"))
			{
				int port;
				if (System.Int32.TryParse(_port, out port))
				{
					MatchMakingController.Instance.ConnectToCustomServer(_ipAddress, port, _playerName);
				}
			}
			
			// player name
			GUI.Label(new Rect(w + 300, h - 15, 80, 20), "Player Name:");
			_playerName = GUI.TextField(new Rect(w + 390, h - 15, 100, 30), _playerName, 15);
		}
		else if (MatchMakingController.Instance.CurrentStatus == MatchMakingStatus.InLobby)
		{
			int w = (int)(Screen.width * 0.25f);
			int h = (int)(Screen.height * 0.25f + 100);
			
			_roomName = GUI.TextField(new Rect(w - 50, h - 15, 100, 30), _roomName, 10);
			if (GUI.Button(new Rect(w + 60, h - 15, 100, 30), "Create Room"))
			{
				if (_roomName == "")
				{
					Debug.LogError("Room name must not be blank");
				}
				MatchMakingController.Instance.CreateRoom(_roomName);
			}
			
			GUI.Label(new Rect(Screen.width - 400, 20, 200, 20), "Select a room to join");
			var roomList = PhotonNetwork.GetRoomList();
			for (int i = 0, row = 0; i < roomList.Length; ++i)
			{
				if (roomList[i].open)
				{
					if (GUI.Button(new Rect(Screen.width - 400, row * 30 + 50, 200, 20), roomList[i].name))
					{
						MatchMakingController.Instance.JoinRoom(roomList[i].name);
					}
					++row;
				}
			}
		}
		else if (MatchMakingController.Instance.CurrentStatus == MatchMakingStatus.InRoom)
		{
			// show the start room buttons
			if (PhotonNetwork.isMasterClient)
			{
				int w = (int)(Screen.width * 0.25f);
				int h = (int)(Screen.height * 0.25f + 100);
				
				if (GUI.Button(new Rect(w - 50, h - 15, 100, 30), "Start Match"))
				{
					MatchMakingController.Instance.StartMatch("NetworkTestScene");
				}
				
				if (GUI.Button(new Rect(w - 50, h + 25, 150, 30), "Test ScriptsTestScene"))
				{
					MatchMakingController.Instance.StartMatch("ScriptsTestScene");
				}
			}
			
			// show the other players
			GUI.Label(new Rect(Screen.width - 400, 20, 200, 20), "Room: " + PhotonNetwork.room.name);
			GUI.Label(new Rect(Screen.width - 400, 60, 200, 20), PhotonNetwork.player.name);
			for (int i = 0; i < PhotonNetwork.otherPlayers.Length; ++i)
			{
				GUI.Label(new Rect(Screen.width - 400, i * 30 + 90, 200, 20), PhotonNetwork.otherPlayers[i].name);
			}
		}
	}
}
