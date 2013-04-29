using UnityEngine;
using System.Collections;

public class Crawler : MonoBehaviour
{
	
	[SerializeField]
	PhotonView _photonView;
	
	// Player following-related
	[SerializeField] private float _maxVelocity;						// Max following velocity
	[SerializeField] private float _minDistance;						// Minimun distance to keep from player
	private float _distanceToPlayer;									// Current distance to player
	private float _velocity;			
	
	// Hover-related 
	[SerializeField] private float _desiredHeight;						// The desired height the object should hover
	private float _hoverHeight;											// The current hover height
	[SerializeField] private float _hoverVelocity;						// The hover velocity 
	
	// Target
	[SerializeField] private GameObject _target = null;					// The Crawler current target
	[SerializeField] private float _firingDistance;						// Firing distance. Is set to the guns's distance
	[SerializeField] private float _fireInterval;						// Fire rate
	[SerializeField] private CrawlerGun _gun;			
	private float _fireTimer;											// Timer for fire rate							
	private bool _isPathing;
	
	// Use this for initialization
	private void Start()
	{
		_firingDistance = _gun.Range;
		if (_photonView.isMine)
		{
			StartCoroutine(FindNewTarget());
		}
	}
	
	// Update is called once per frame
	private void Update()
	{
		if (_photonView.isMine && _target == null)
		{
			FindTarget();
		}
		
		if (_target != null)
		{
			_isPathing = GetComponent<AstarAI>().inPathArea;
			if (!_isPathing)
			{
				maintainHeight();
				followPlayer();
			}
			if (_photonView.isMine)
			{
				TryFire();
			}
		}
		
		if (_photonView.isMine && transform.position.y < -100)
		{
			PhotonNetwork.Destroy(gameObject);
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
	
	private void TryFire()
	{
		if (_photonView.isMine)
		{
			_distanceToPlayer = Vector3.Distance(transform.position, _target.transform.position);
			if (_distanceToPlayer <= _firingDistance)
			{
				float delta = Time.deltaTime;
				_gun.UpdateFiring(delta, true);
			}
		}
	}
	
	private IEnumerator FindNewTarget()
	{
		while (true)
		{
			FindTarget();
			yield return new WaitForSeconds(10f);
		}
	}
	
	// Maintain desired height
	private void maintainHeight()
	{
		float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);

		_hoverHeight = transform.position.y - terrainHeight;
		
		if (_hoverHeight != _desiredHeight)
		{
			Vector3 newPos = transform.position;	
			newPos.y += (_desiredHeight - _hoverHeight) * _hoverVelocity;
			transform.position = newPos;
		}
	}
	
	// Making it follow the target
	private void followPlayer()
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
	
}
