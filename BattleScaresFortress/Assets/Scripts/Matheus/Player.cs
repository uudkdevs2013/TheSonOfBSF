using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	
	Weapon weapon;
	
	private void Awake()
	{
		if (_allPlayers == null)
		{
			_allPlayers = new LinkedList<Player>();
		}
		_allPlayers.AddLast(this);
	}
	
	private void OnDestroy()
	{
		_allPlayers.Remove(this);
	}
	
	
	private static LinkedList<Player> _allPlayers;
	
	public static IEnumerable<Player> GetAllPlayers()
	{
		return (IEnumerable<Player>)_allPlayers;
	}
	
}
