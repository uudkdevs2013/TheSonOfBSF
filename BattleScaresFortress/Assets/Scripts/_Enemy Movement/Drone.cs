using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Drone : MonoBehaviour
{
	
	[SerializeField] PhotonView _photonView;
	
	// Hover-related 
	[SerializeField] private float _desiredHeight;					// The desired height the object should hover
	[SerializeField] private float _hoverVelocity;					// The hover velocity 
	private float _hoverHeight;										// The current hover height
	
	// Player following-related
	[SerializeField] private float _maxVelocity;					// Max following velocity
	[SerializeField] private float _minDistance;					// Minimun distance to keep from player
	private float _distanceToPlayer;								// Current distance to player
	private float _velocity;										// Current following velocity
	
	[SerializeField] private float _firingDistance;					// Minimum distance to fire
	private float _fireTimer;										// Timer used to firing rate
	
	// Strafe
	[SerializeField] private float _strafeSpeed = 5.0f;				// Sideways speed
	[SerializeField] private float _strafeFrequency = 3.0f;			// How often the strafe randomly changes
	private Vector3 _strafeDirection;								// The current strafe direction
	private bool _strafeRight = false;								// It is strafing right (false is left)
	
	// The Drone's gun
	[SerializeField] private DroneGun _gun;
		
	// Target
	[SerializeField] private GameObject _target = null;
	
	private void Start()
	{
		if (_photonView.isMine)
		{
			StartCoroutine(GetNewStrafe());
			_firingDistance = _gun.Range;
			StartCoroutine(FindNewTarget());
		}
		_gun.IsLocal = (_photonView == null || _photonView.isMine);
	}
	/*
	protected void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(transform.position);
			if(_gun.NeedsSend())
			{
				stream.SendNext(true);
				_gun.SendData(stream, info);
			}
			else
			{
				stream.SendNext(false);
			}
		}
		else
		{
			// networked player; receive data
			transform.position = (Vector3)stream.ReceiveNext();
			if((bool) stream.ReceiveNext())
			{
				_gun.ReadData(stream, info);
			}
		}
	}*/
	
	// Update is called once per frame
	private void Update()
	{
		if (_photonView.isMine)
		{
			if (_target == null)
			{
				FindTarget();
			}
			else
			{
				MaintainHeight();
				Strafe();
				FollowPlayer();
				TryFire();
			}
		}
		else
		{
			if (_target != null)
			{
				MaintainHeight();
				Strafe();
				FollowPlayer();
				TryFire();
			}
		}
	}
	
	private void FindTarget()
	{
		if (_photonView.isMine)
		{
			PlayerController closestPlayer = null;
			float closestDistance = float.MaxValue;
			foreach (var player in PlayerController.GetAllPlayers())
			{
				float distance = Vector3.Distance(player.transform.position, transform.position);
				if (distance < closestDistance)
				{
					closestPlayer = player;
					closestDistance = distance;
				}
			}
			if (closestPlayer != null)
			{
				_photonView.RPC("rpcSetTarget", PhotonTargets.Others, closestPlayer.photonView.owner.name);
				_target = closestPlayer.gameObject;
			}
		}
	}
	
	[RPC]
	private void rpcSetTarget(string targetName)
	{
		foreach (var player in PlayerController.GetAllPlayers())
		{
			if (player.photonView.owner.name == targetName)
			{
				_target = player.gameObject;
				break;
			}
		}
	}
	
	// Maintain desired height
	private void MaintainHeight()
	{
		float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);
		float targetHeight = _target.transform.position.y;
		if (terrainHeight < targetHeight)
		{
			_hoverHeight = transform.position.y - targetHeight;
		} else
		{
			_hoverHeight = transform.position.y - terrainHeight;
		}
		
		if (_hoverHeight != _desiredHeight)
		{
			Vector3 newPos = transform.position;	
			newPos.y += (_desiredHeight - _hoverHeight) * _hoverVelocity;
			transform.position = newPos;
		}
	}
	
	private void Strafe()
	{
		if (_strafeRight)
		{
			_strafeDirection = transform.right;
		}
		else
		{
			_strafeDirection = - transform.right;
		}
		transform.position += _strafeDirection * _strafeSpeed * Time.deltaTime;
	}
	
	// Making it follow the target
	private void FollowPlayer()
	{
		transform.LookAt(_target.transform); 
		_distanceToPlayer = Vector3.Distance(transform.position, _target.transform.position);
		_velocity = _distanceToPlayer - _minDistance;
		if (_velocity > _maxVelocity)
		{
			_velocity = _maxVelocity;
		}

		transform.position += transform.forward * _velocity * Time.deltaTime;

	}
	
	private void TryFire()
	{
		if (_distanceToPlayer <= _firingDistance)
		{
			float delta = Time.deltaTime;
			_gun.UpdateFiring(delta, true);
		}
	}
	
	private IEnumerator GetNewStrafe()
	{
		while (true)
		{
			int r = Random.Range(0, 2);
			if (r == 0)
			{
				ChangeDirection();
			}
			float randomWait = Random.value - 0.5f;
			yield return new WaitForSeconds(_strafeFrequency + randomWait);
		}
	}
	
	private void ChangeDirection()
	{
		_photonView.RPC("rpcChangeDirection", PhotonTargets.All);
	}
	
	[RPC]
	private void rpcChangeDirection()
	{
		_strafeRight = !_strafeRight;
	}
	
	private IEnumerator FindNewTarget()
	{
		while (true)
		{
			FindTarget();
			yield return new WaitForSeconds(10f);
		}
	}
}
