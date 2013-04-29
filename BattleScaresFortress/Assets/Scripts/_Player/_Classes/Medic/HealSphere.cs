using UnityEngine;
using System.Collections.Generic;

public class HealSphere : MonoBehaviour
{
	
	[SerializeField] private GameObject _innerSphere;
	[SerializeField] private GameObject _outerSphere;
	[SerializeField] private PhotonView _photonView;
	[SerializeField] private float _timeBeforeHeal = 0.5f;
	[SerializeField] private float _amountToHeal = 3;
	[SerializeField] private float _timeToLive = 15;
	
	[SerializeField] private float _moveSpeed = 1;
	private Vector3 _fireDirection;
	private bool _isFiring = true;
	
	
	
	private Dictionary<PlayerController, float> _playersBeingHealed;
	
	private bool _isGrowing = true;
	
	private void Awake()
	{
		if (_photonView.isMine)
		{
			_playersBeingHealed = new Dictionary<PlayerController, float>();
		}
	}
	
	private void Start()
	{
		_fireDirection = (Vector3)(_photonView.instantiationData[0]);
		_fireDirection.Normalize();
	}
	
	private void Update()
	{
		if (_isFiring)
		{
			if (ShouldStopMoving())
			{
				_isFiring = false;
				if (_photonView.isMine)
				{
					_photonView.RPC("rpcStopMoving", PhotonTargets.Others, transform.position);
				}
			}
			else
			{
				transform.position += _fireDirection * _moveSpeed * Time.deltaTime;
			}
		}
		else if (_isGrowing)
		{
			float scale = Mathf.Lerp(transform.localScale.x, 1, Time.deltaTime * 3);
			if (scale > 0.999)
			{
				scale = 1;
				_isGrowing = false;
			}
			transform.localScale = new Vector3(scale, scale, scale);
		}
		else
		{
			if (_photonView.isMine)
			{
				FindPlayersInRange();
				TryToHealPlayers();
				_timeToLive -= Time.deltaTime;
				if (_timeToLive < 0)
				{
					PhotonNetwork.Destroy(gameObject);
				}
			}
		}
		
		_innerSphere.transform.rotation *= Quaternion.Euler(0, 0.5f, 0);
		_outerSphere.transform.rotation *= Quaternion.Euler(0, -0.5f, 0);
	}
	
	private bool ShouldStopMoving()
	{
		foreach (var collider in Physics.OverlapSphere(transform.position, 1))
		{
//			if (!isEnemy && !isPlayer)
			return true;
		}
		return false;
	}
	
	[RPC]
	private void rpcStopMoving(Vector3 finalPosition)
	{
		_isFiring = false;
		transform.position = finalPosition;
	}
	
	private void FindPlayersInRange()
	{
		HashSet<PlayerController> playersInRange = new HashSet<PlayerController>();
		foreach (var player in PlayerController.GetAllPlayers())
		{
			if (Vector3.Distance(transform.position, player.transform.position) <= 18)
			{
				playersInRange.Add(player);
			}
		}
		// add players that have entered
		foreach (var player in playersInRange)
		{
			if (!_playersBeingHealed.ContainsKey(player))
			{
				_playersBeingHealed.Add(player, 0f);
			}
		}
		// remove players that have left
		PlayerController[] keys = new PlayerController[_playersBeingHealed.Keys.Count];
		_playersBeingHealed.Keys.CopyTo(keys, 0);
		foreach (var player in keys)
		{
			if (!playersInRange.Contains(player))
			{
				_playersBeingHealed.Remove(player);
			}
		}
	}
	
	private void TryToHealPlayers()
	{
		PlayerController[] keys = new PlayerController[_playersBeingHealed.Keys.Count];
		_playersBeingHealed.Keys.CopyTo(keys, 0);
		foreach (var player in keys)
		{
			float playerTime = _playersBeingHealed[player];
			playerTime += Time.deltaTime;
			if (playerTime > _timeBeforeHeal)
			{
				player.ApplyHeal(_amountToHeal);
				playerTime -= _timeBeforeHeal;
			}
			_playersBeingHealed[player] = playerTime;
		}
	}
	
}
