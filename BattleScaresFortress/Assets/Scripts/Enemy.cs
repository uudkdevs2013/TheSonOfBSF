using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	
	[SerializeField] PhotonView _photonView;
	
	public float Health { get; private set; }
	
	private void Awake()
	{
		Health = 10;
	}
	
	public void ApplyDamage(float amountOfDamage)
	{
		print("enemy apply damage");
		_photonView.RPC("rpcApplyDamage", PhotonTargets.All, amountOfDamage);
	}
	
	[RPC]
	private void rpcApplyDamage(float amountOfDamage)
	{
		print("rpc enemy apply damage");
		Health -= amountOfDamage;
		if (Health < 0)
		{
			Die();
		}
	}
	
	public void Die()
	{
		if (_photonView.isMine)
		{
			PhotonNetwork.Destroy(gameObject);
		}
	}
	
}
