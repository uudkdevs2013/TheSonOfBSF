using UnityEngine;
using System.Collections;

public class Crawler : MonoBehaviour {
	
	// Player following-related
	[SerializeField]
	private float maxVelocity;										// Max following velocity
	[SerializeField]
	private float minDistance;										// Minimun distance to keep from player
	private float distanteToPlayer;									// Current distance to player
	private float velocity;			
	
	// Target
	[SerializeField]
	private GameObject target = null;
	
	[SerializeField] private CrawlerGun _gun;
	[SerializeField] float firingDistance = 25.0f;
	[SerializeField] float fireInterval;
	float fireCounter;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (target == null)
		{
			FindTarget();
		}
		else
		{
			followPlayer();
		}
	}
	
	private void FindTarget()
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
		target = closestPlayer.gameObject;
	}
	
	// Making it follow the target
	private void followPlayer()
	{
		transform.LookAt(target.transform); 
		distanteToPlayer = Vector3.Distance(transform.position, target.transform.position);
		velocity = distanteToPlayer - minDistance;
		if (velocity > maxVelocity)
		{
			velocity = maxVelocity;
		}
		if (distanteToPlayer > minDistance)
		{
			transform.position += transform.forward * velocity * Time.deltaTime;
		} else if (distanteToPlayer < minDistance)
		{
			velocity = distanteToPlayer + minDistance;
			transform.position -= transform.forward * (1 * velocity) * Time.deltaTime;
		}

	}
}
