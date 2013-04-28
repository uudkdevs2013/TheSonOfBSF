using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	
	[SerializeField] private PhotonView _photonView;
	[SerializeField] private Camera _camera;
	[SerializeField] GameObject _weapon;
	
	public PhotonView photonView
	{
		get
		{
			return _photonView;
		}
	}
	
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
	
	private void Update()
	{
		if (_photonView.isMine)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (_weapon.GetComponent<Weapon>().Fire()) {
					RaycastHit hit;
					Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
					if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
					{
						print("raycast hit: " + hit.collider.name);
						var enemy = hit.collider.gameObject.GetComponent<Enemy>();
						if (enemy == null && hit.transform.parent != null)
						{
							enemy = hit.transform.parent.GetComponent<Enemy>();
						}
						if (enemy != null)
						{
							enemy.ApplyDamage(3);
						}
					}
				}
			}
		}
	}
	
	
	private static LinkedList<Player> _allPlayers;
	
	public static IEnumerable<Player> GetAllPlayers()
	{
		return (IEnumerable<Player>)_allPlayers;
	}
	
}
