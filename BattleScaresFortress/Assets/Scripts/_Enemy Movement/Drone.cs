using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Drone : MonoBehaviour
{
	
	[SerializeField] PhotonView _photonView;
	
	// Hover-related 
	[SerializeField]
	private float desiredHeight;									// The desired height the object should hover
	private float hoverHeight;										// The current hover height
	[SerializeField]
	private float hoverVelocity;									// The hover velocity 
	
	// Player following-related
	[SerializeField]
	private float maxVelocity;										// Max following velocity
	[SerializeField]
	private float minDistance;										// Minimun distance to keep from player
	private float distanceToPlayer;									// Current distance to player
	private float velocity;											// Current following velocity
	
	[SerializeField] float firingDistance;
	float fireCounter;
	
	// Strafe
	[SerializeField] float strafeSpeed = 5.0f;
	[SerializeField] float strafeFrequency = 3.0f;
	Vector3 strafeDirection;
	bool strafeRight = false;
	
	[SerializeField] private DroneGun _gun;
	
	
	// Target
	[SerializeField]
	private GameObject target = null;
	
	void Start () {
		StartCoroutine(GetNewStrafe());
		firingDistance = _gun.Range;
		StartCoroutine(FindNewTarget());
	}
	
	
	// Update is called once per frame
	private void Update()
	{
		if (target == null)
		{
			FindTarget();
		}
		else
		{
			maintainHeight();
			Strafe();
			followPlayer();
			TryFire();
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
				target = closestPlayer.gameObject;
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
				target = player.gameObject;
				break;
			}
		}
	}
	
	// Maintain desired height
	private void maintainHeight()
	{
		float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);
		float targetHeight = target.transform.position.y;
		if (terrainHeight < targetHeight)
		{
			hoverHeight = transform.position.y - targetHeight;
		}
		else
		{
			hoverHeight = transform.position.y - terrainHeight;
		}
		
		if (hoverHeight != desiredHeight)
		{
			Vector3 newPos = transform.position;	
			newPos.y += (desiredHeight - hoverHeight) * hoverVelocity;
			transform.position = newPos;
		}
	}
	
	void Strafe () {
		if (strafeRight)
			strafeDirection = transform.right;
		else
			strafeDirection = - transform.right;
		transform.position += strafeDirection * strafeSpeed * Time.deltaTime;
	}
	
	// Making it follow the target
	private void followPlayer()
	{
		transform.LookAt(target.transform); 
		distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
		velocity = distanceToPlayer - minDistance;
		if (velocity > maxVelocity)
		{
			velocity = maxVelocity;
		}

		transform.position += transform.forward * velocity * Time.deltaTime;

	}
	
	void TryFire()
	{
		if (distanceToPlayer <= firingDistance)
		{
			float delta = Time.deltaTime;
			_gun.UpdateFiring(delta, true);
		}
	}
	
	IEnumerator GetNewStrafe () {
		while (true) {
			int r = Random.Range(0,2);
			if (r == 0)
				ChangeDirection();
			yield return new WaitForSeconds(strafeFrequency);
		}
	}
	
	void ChangeDirection () {
		strafeRight = !strafeRight;
	}
	
	IEnumerator FindNewTarget () {
		while (true) {
			FindTarget();
			yield return new WaitForSeconds(10f);
		}
	}
}
