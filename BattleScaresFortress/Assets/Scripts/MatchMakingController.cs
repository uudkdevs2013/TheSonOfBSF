using UnityEngine;
using System.Collections;
using Photon;

public enum MatchMakingStatus
{
	InLobby, SearchingForRooms, CreatingRoom, InRoom, NotConnected, Connecting
}

public class MatchMakingController : Photon.MonoBehaviour
{
	
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
		Instance = this;
		_currentStatus = MatchMakingStatus.NotConnected;
	}
	
	private void Start()
	{
		PhotonNetwork.sendRate = 40;
		PhotonNetwork.sendRateOnSerialize = 30;
	}
	
	private void OnGUI()
	{
		GUI.Label(new Rect(10, Screen.height - 45, 1000, 20), "Photon Status: " + PhotonNetwork.connectionState.ToString());
		GUI.Label(new Rect(10, Screen.height - 25, 1000, 20), "Matchmaking Status: " + _currentStatus);
	}
	
	public void ConnectToRemoteServers(string playerName)
	{
		PhotonNetwork.player.name = playerName;
		
		PhotonNetwork.ConnectUsingSettings("v1.0");
		_currentStatus = MatchMakingStatus.Connecting;
	}
	
	public void ConnectToCustomServer(string ipAddress, int port, string playerName)
	{
		PhotonNetwork.player.name = playerName;
		
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
		PhotonNetwork.room.open = false;
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
