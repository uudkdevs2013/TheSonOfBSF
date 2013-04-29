using UnityEngine;
using System.Collections;

public class Crawler : MonoBehaviour {
	
	[SerializeField] PhotonView _photonView;
	
	// Player following-related
	[SerializeField]
	private float maxVelocity;										// Max following velocity
	[SerializeField]
	private float minDistance;										// Minimun distance to keep from player
	private float distanceToPlayer;									// Current distance to player
	private float velocity;			
	
	// Hover-related 
	[SerializeField]
	private float desiredHeight;									// The desired height the object should hover
	private float hoverHeight;										// The current hover height
	[SerializeField]
	private float hoverVelocity;									// The hover velocity 
	
	// Target
	[SerializeField]
	private GameObject target = null;
	
	[SerializeField] float firingDistance;
	float fireCounter;
	[SerializeField] float fireInterval;
	
	[SerializeField] private CrawlerGun _gun;
	
	bool isPathing;
	
	// Use this for initialization
	void Start () {
		firingDistance = _gun.Range;
		StartCoroutine(FindNewTarget());
	}
	
	// Update is called once per frame
	void Update () {
		if (target == null)
		{
			FindTarget();
		}
		else {
			isPathing = GetComponent<AstarAI>().inPathArea;
			if (!isPathing) {
				maintainHeight();
				followPlayer();
			}
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
	
	
	void TryFire()
	{
		distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
		if (distanceToPlayer <= firingDistance)
		{
			float delta = Time.deltaTime;
			_gun.UpdateFiring(delta, true);
		}
	}
	

	
	IEnumerator FindNewTarget () {
		while (true) {
			FindTarget();
			yield return new WaitForSeconds(10f);
		}
	}
	
		// Maintain desired height
	private void maintainHeight()
	{
		float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);

		hoverHeight = transform.position.y - terrainHeight;
		
		if (hoverHeight != desiredHeight)
		{
			Vector3 newPos = transform.position;	
			newPos.y += (desiredHeight - hoverHeight) * hoverVelocity;
			transform.position = newPos;
		}
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
	
}
