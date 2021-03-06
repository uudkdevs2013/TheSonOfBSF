using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
	
	[SerializeField] PhotonView _photonView;
	[SerializeField] private float _health;
	public float Health 
	{
		get
		{
			return _health;
		}
	}
	
	private void Awake()
	{
		if (_allEnemies == null)
		{
			_allEnemies = new List<Enemy>();
		}
		_allEnemies.Add(this);
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
		_health -= amountOfDamage;
		if (_health < 0)
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
	
	private void OnDestroy()
	{
		_allEnemies.Remove(this);
	}
	
	
	private static List<Enemy> _allEnemies = null;
	
	public static IEnumerable<Enemy> AllEnemies()
	{
		return (IEnumerable<Enemy>)_allEnemies;
	}
	
	public static int NumberOfEnemies()
	{
		return _allEnemies.Count;
	}
	
}
