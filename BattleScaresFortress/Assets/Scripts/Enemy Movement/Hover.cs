using UnityEngine;
using System.Collections.Generic;

public class Hover : MonoBehaviour
{
	
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
	private float distanteToPlayer;									// Current distance to player
	private float velocity;											// Current following velocity
	
	// Target
	[SerializeField]
	private GameObject target = null;
	
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
			followPlayer();
		}
	}
	
	private void FindTarget()
	{
		Player closestPlayer = null;
		float closestDistance = float.MaxValue;
		foreach (var player in Player.GetAllPlayers())
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
	
	// Maintain desired height
	private void maintainHeight()
	{
		float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);
		float targetHeight = target.transform.position.y;
		if (terrainHeight < targetHeight)
		{
			hoverHeight = transform.position.y - targetHeight;
		} else
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
			//velocity = distanteToPlayer + minDistance;
			transform.position += transform.forward * velocity * Time.deltaTime;
		}

	}
}
