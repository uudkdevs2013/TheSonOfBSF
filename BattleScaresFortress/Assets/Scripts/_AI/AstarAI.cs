using UnityEngine;
using System.Collections;
using Pathfinding;

public class AstarAI : MonoBehaviour
{

	//The point to move to
	public GameObject target;
	private Seeker seeker;
	private CharacterController controller;
 
	//The calculated path
	public Path path;
    
	//The AI's speed per second
	public float speed = 100;
	
	// Player following-related
	[SerializeField]
	private float maxVelocity;										// Max following velocity
	[SerializeField]
	private float minDistance;										// Minimun distance to keep from player
	private float distanceToPlayer;									// Current distance to player
    
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;
	public float repeatTime = 0.5f;
	public bool inPathArea;
 
	public void Start()
	{
		seeker = GetComponent<Seeker>();
		controller = GetComponent<CharacterController>();
		FindTarget();
		StartCoroutine(GetNewPath());   
	}
    
	public void OnPathComplete(Path p)
	{
		//Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		if (!p.error)
		{
			path = p;
			//Reset the waypoint counter
			currentWaypoint = 1;
		}
	}
 
	public void Update()
	{
		if (target == null)
		{
			FindTarget();
			return;
		}
		if (inPathArea)
		{
	        
			if (currentWaypoint >= path.vectorPath.Count)
			{
				//Debug.Log ("End Of Path Reached");
				return;
			}
	        
			//Direction to the next waypoint
			Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
			dir *= speed * Time.fixedDeltaTime;
			controller.SimpleMove(dir);
			transform.LookAt(target.transform); 
			//Check if we are close enough to the next waypoint
			//If we are, proceed to follow the next waypoint
			if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
			{
				currentWaypoint++;
				return;
			}
		}
	}
	
	private void GetPath()
	{
		if (target != null)
		{
			seeker.StartPath(transform.position, target.transform.position, OnPathComplete);
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
		if (closestPlayer != null)
		{
			target = closestPlayer.gameObject;
		}
	}
	
	IEnumerator GetNewPath()
	{
		while (true)
		{
			GetPath();
			yield return new WaitForSeconds(repeatTime);
		}
	}
	
	public void SetInPath(bool v)
	{
		inPathArea = v;
	}
}
