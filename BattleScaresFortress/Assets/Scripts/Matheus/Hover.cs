using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {
	
	// Hover-related 
	[SerializeField]
	float desiredHeight;									// The desired height the object should hover
	float hoverHeight;										// The current hover height
	[SerializeField]
	float hoverVelocity;									// The hover velocity 
	
	// Player following-related
	[SerializeField]
	float maxVelocity;										// Max following velocity
	[SerializeField]
	float minDistance;										// Minimun distance to keep from player
	float distanteToPlayer;									// Current distance to player
	float velocity;											// Current following velocity
	
	// Target
	[SerializeField]
	GameObject target;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		maintainHeight();
		followPlayer();	
	}
	
	// Maintain desired height
	void maintainHeight () {		
		float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);
		float targetHeight = target.transform.position.y;
		if (terrainHeight < targetHeight)
			hoverHeight = transform.position.y -  targetHeight;
		else
			hoverHeight = transform.position.y - terrainHeight;
		
		if (hoverHeight != desiredHeight)
		{
			Vector3 newPos = transform.position;	
			newPos.y += (desiredHeight - hoverHeight) * hoverVelocity;
			transform.position = newPos;
		}
	}
	
	// Making it follow the target
	void followPlayer () {
		transform.LookAt(target.transform); 
		distanteToPlayer = Vector3.Distance(transform.position, target.transform.position);
		velocity = distanteToPlayer - minDistance;
		if (velocity > maxVelocity)
			velocity = maxVelocity;
		if (distanteToPlayer > minDistance) {
			transform.position += transform.forward * velocity * Time.deltaTime;
		}
		else if (distanteToPlayer < minDistance) {
			velocity = distanteToPlayer + minDistance;
			transform.position -= transform.forward * (1 * velocity) * Time.deltaTime;
		}

	}
}
