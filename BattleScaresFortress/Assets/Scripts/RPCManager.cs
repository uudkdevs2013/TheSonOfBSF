using UnityEngine;
using System.Collections.Generic;

public class RPCManager : MonoBehaviour
{
	public static RPCManager Instance { get; private set; }
	
	[SerializeField] PhotonView _pv;
	
	private Dictionary<string, bool> _levelsLoaded;
	public bool AllLevelsAreLoaded { get; private set; }
	
	private void Awake()
	{
		Instance = this;
		_levelsLoaded = new Dictionary<string, bool>();
		foreach (var player in PhotonNetwork.playerList)
		{
			_levelsLoaded.Add(player.name, false);
		}
		AllLevelsAreLoaded = false;
	}
	
	private void OnDestroy()
	{
		Instance = null;
	}
	
	private void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 200, 20), "Current Ping: " + PhotonNetwork.GetPing());
	}
	
	public void MyLevelIsLoaded()
	{
		_pv.RPC("rpcOtherLevelLoaded", PhotonTargets.All, PhotonNetwork.player.name);
	}
	
	[RPC]
	private void rpcOtherLevelLoaded(string playerName)
	{
		_levelsLoaded[playerName] = true;
		bool allAreLoaded = true;
		foreach (bool isLoaded in _levelsLoaded.Values)
		{
			allAreLoaded = allAreLoaded && isLoaded;
		}
		AllLevelsAreLoaded = allAreLoaded;
	}
	
	
	private void OnDisconnectedFromPhoton()
	{
		Debug.LogError("disconnected from photon");
	}
	
}
