using UnityEngine;
using System.Collections;

public class RPCManager : MonoBehaviour
{
	
	public static RPCManager Instance { get; private set; }
	
	[SerializeField] PhotonView _pv;
	
	private void Awake()
	{
		Instance = this;
	}
	
	public void StartMatch()
	{
		_pv.RPC("rpcStartMatch", PhotonTargets.All);
	}
	
	[RPC]
	private void rpcStartMatch()
	{
		Application.LoadLevel("NetworkTestScene");
	}
	
}
