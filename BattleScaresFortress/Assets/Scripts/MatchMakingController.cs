using UnityEngine;
using System.Collections;
using Photon;

public enum MatchMakingStatus
{
	InLobby, SearchingForRooms, CreatingRoom, InRoom
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
	}
	
	private void Start()
	{
		PhotonNetwork.ConnectUsingSettings("v1.0");
	}
	
	private void OnGUI()
	{
		GUI.Label(new Rect(10, Screen.height - 45, 1000, 20), "Photon Status: " + PhotonNetwork.connectionState.ToString());
		GUI.Label(new Rect(10, Screen.height - 25, 1000, 20), "Matchmaking Status: " + _currentStatus);
		
		if (_currentStatus == MatchMakingStatus.InRoom)
		{
			GUI.Label(new Rect(Screen.width - 400, 20, 200, 20), "My name: " + PhotonNetwork.player.name + " " + PhotonNetwork.isMasterClient);
			for (int i = 0; i < PhotonNetwork.otherPlayers.Length; ++i)
			{
				GUI.Label(new Rect(Screen.width - 400, i * 30 + 50, 200, 20), PhotonNetwork.otherPlayers[i].name);
			}
		}
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
	}
	
	private void OnJoinedLobby()
	{
		_currentStatus = MatchMakingStatus.InLobby;
		PhotonNetwork.JoinRandomRoom();
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
		Debug.Log("Random join failed");
		PhotonNetwork.CreateRoom(Time.time.ToString());
		_currentStatus = MatchMakingStatus.CreatingRoom;
	}
	
	private void OnPhotonCreateRoomFailed()
	{
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
