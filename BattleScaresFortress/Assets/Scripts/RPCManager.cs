using UnityEngine;
using System.Collections.Generic;

public class RPCManager : MonoBehaviour
{
	public static RPCManager Instance { get; private set; }
	
	[SerializeField] PhotonView _pv;
	
	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(this);
	}
	
	public void StartMatch()
	{
		_pv.RPC("rpcStartMatch", PhotonTargets.All);
	}
	
	[RPC] private void rpcStartMatch()
	{
		_levelsLoaded = new Dictionary<string, bool>();
		foreach (var player in PhotonNetwork.playerList)
		{
			_levelsLoaded.Add(player.name, false);
		}
		AllLevelsAreLoaded = false;
		
		Application.LoadLevel("Map1");
	}
	
	
	private Dictionary<string, bool> _levelsLoaded;
	public bool AllLevelsAreLoaded { get; private set; }
	
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
	
}
