using UnityEngine;
using System.Collections;
using Photon;

public enum MatchMakingStatus
{
	InLobby, SearchingForRooms, CreatingRoom, InRoom, NotConnected, Connecting
}

public class MatchMakingController : Photon.MonoBehaviour
{
	
	[SerializeField] private string _playerName;
	[SerializeField] PhotonView _pv;
	
	public static MatchMakingController Instance { get; private set; }
	
	private MatchMakingStatus _currentStatus;
	public MatchMakingStatus CurrentStatus
	{
		get
		{
			return _currentStatus;
		}
	}
	
	private void Awake()
	{
		Application.runInBackground = true;
		PhotonNetwork.player.name = _playerName;
		Instance = this;
		_currentStatus = MatchMakingStatus.NotConnected;
	}
	
//	private void Start()
//	{
//		PhotonNetwork.ConnectUsingSettings("v1.0");
		
//		PhotonNetwork.Connect("192.168.1.143", 8088, "v1.0");
//		PhotonNetwork.Connect("192.168.1.143", 4530, "v1.0");
//		PhotonNetwork.Connect("192.168.1.143", 843, "v1.0");
//		PhotonNetwork.Connect("192.168.1.143", 5050, "78a89ebc-d11b-4604-8d6d-0598726bb883", "v1.0");
//		PhotonNetwork.Connect("192.168.1.143", 5057, "78a89ebc-d11b-4604-8d6d-0598726bb883", "v1.0"); // udp  connects, create room fails
//		PhotonNetwork.Connect("192.168.1.143", 5055, "78a89ebc-d11b-4604-8d6d-0598726bb883", "v1.0"); // tcp  connects, creates rooms, starts games
//	}
	
	private void OnGUI()
	{
		GUI.Label(new Rect(10, Screen.height - 45, 1000, 20), "Photon Status: " + PhotonNetwork.connectionState.ToString());
//		GUI.Label(new Rect(10, Screen.height - 45, 1000, 20), "Photon Status, detailed: " + PhotonNetwork.connectionStateDetailed.ToString());
		GUI.Label(new Rect(10, Screen.height - 25, 1000, 20), "Matchmaking Status: " + _currentStatus);
//		print("Photon Status: " + PhotonNetwork.connectionState.ToString());
//		print("Photon Status, detailed: " + PhotonNetwork.connectionStateDetailed.ToString());
		
//		if (_currentStatus == MatchMakingStatus.InRoom)
//		{
//			GUI.Label(new Rect(Screen.width - 400, 20, 200, 20), "My name: " + PhotonNetwork.player.name + " " + PhotonNetwork.isMasterClient);
//			for (int i = 0; i < PhotonNetwork.otherPlayers.Length; ++i)
//			{
//				GUI.Label(new Rect(Screen.width - 400, i * 30 + 50, 200, 20), PhotonNetwork.otherPlayers[i].name);
//			}
//		}
	}
	
	public void ConnectToRemoteServers()
	{
		PhotonNetwork.ConnectUsingSettings("v1.0");
		_currentStatus = MatchMakingStatus.Connecting;
	}
	
	public void ConnectToCustomServer(string ipAddress, int port)
	{
		PhotonNetwork.Connect(ipAddress, port, "78a89ebc-d11b-4604-8d6d-0598726bb883", "v1.0");
		_currentStatus = MatchMakingStatus.Connecting;
	}
	
	public void CreateRoom(string roomName)
	{
		PhotonNetwork.CreateRoom(roomName);
		_currentStatus = MatchMakingStatus.CreatingRoom;
	}
	
	public void JoinRoom(string roomName)
	{
		PhotonNetwork.JoinRoom(roomName);
	}
	
	public void StartMatch(string level)
	{
		_pv.RPC("rpcStartMatch", PhotonTargets.All, level);
	}
	
	[RPC]
	private void rpcStartMatch(string level)
	{
		PhotonNetwork.isMessageQueueRunning = false;
		Application.LoadLevel(level);
	}
	
	// these are in the enum: PhotonNetworkingMessage
	#region Photon Network Callbacks
	
	private void OnConnectedToPhoton()
	{
		Debug.Log("Connected to photon with ping: " + PhotonNetwork.GetPing());
	}
	
	private void OnDisconnectedFromPhoton()
	{
		Debug.Log("Disconnected from photon");
		_currentStatus = MatchMakingStatus.NotConnected;
	}
	
	private void OnJoinedLobby()
	{
		_currentStatus = MatchMakingStatus.InLobby;
	}
	
	private void OnLeftLobby()
	{
	}
	
	private void OnCreatedRoom()
	{
	}
	
	private void OnJoinedRoom()
	{
		_currentStatus = MatchMakingStatus.InRoom;
	}
	
	private void OnLeftRoom()
	{
	}
	
	private void OnPhotonRandomJoinFailed()
	{
	}
	
	private void OnPhotonCreateRoomFailed()
	{
		Debug.Log("create room failed");
	}
	
	private void OnPhotonJoinRoomFailed()
	{
	}
	
	private void OnPhotonPlayerConnected()
	{
	}
	
	private void OnPhotonPlayerDisconnected()
	{
	}
	
	// these are the messages I don't listen for
	/*
    OnMasterClientSwitched
    OnConnectionFail,
    OnFailedToConnectToPhoton,
    OnReceivedRoomListUpdate,
    OnConnectedToMaster,
    OnPhotonSerializeView,
    OnPhotonInstantiate,
    OnPhotonMaxCccuReached,
	OnPhotonCustomRoomPropertiesChanged,
	OnPhotonPlayerPropertiesChanged,
    OnUpdatedFriendList,
    OnCustomAuthenticationFailed,
	*/
	
	#endregion
	
}
