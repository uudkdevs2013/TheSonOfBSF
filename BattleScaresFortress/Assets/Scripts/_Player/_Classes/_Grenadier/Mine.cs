using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour
{
	private bool IsLocal { get; set; }
	
	[SerializeField] private PhotonView _photonView;
	[SerializeField] private GameObject _explosion;
	[SerializeField] private float _damage = 10;
	[SerializeField] private float _explosionRadius = 5;
	[SerializeField] private float _checkInterval = 0.35f;
	[SerializeField] private float _checkRadius = 3;
	private bool _activated;
	private bool _exploded;
	private float _checkCounter;
	
	public MineLauncher ParentGun { get; set; }
	
	void Start()
	{
		IsLocal = _photonView == null || _photonView.isMine;
	}
	
	void Update()
	{
		if(IsLocal && _activated && !_exploded)
		{
			_checkCounter -= Time.deltaTime;
			if(_checkCounter <= 0.0f)
			{
				_checkCounter = _checkInterval;
				
				if(HasEnemyNearby())
				{
					Explode();
				}
			}
		}
	}
	
	public void Explode()
	{
		if(IsLocal && !_exploded)
		{
			_exploded = true;
			
			RaycastHit[] hits = Physics.SphereCastAll(transform.position, _explosionRadius, Vector3.up, _explosionRadius);
			Debug.Log("Mine hit " + hits.Length + " targets!");
			foreach(RaycastHit rH in hits)
			{
				var enemy = rH.collider.gameObject.GetComponent<Enemy>();
				if (enemy == null && rH.collider.transform.parent != null)
				{
					enemy = rH.collider.transform.parent.GetComponent<Enemy>();
				}
				if (enemy != null)
				{
					enemy.ApplyDamage(_damage);
				}
			}
			
			if(ParentGun != null)
				ParentGun.RemoveMine(this);
			
			PhotonNetwork.Destroy(_photonView);
		}
	}
	
	public bool HasEnemyNearby()
	{
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, _checkRadius, Vector3.up, _checkRadius);
		foreach(RaycastHit rH in hits)
		{
			var enemy = rH.collider.gameObject.GetComponent<Enemy>();
			if (enemy == null && rH.collider.transform.parent != null)
			{
				enemy = rH.collider.transform.parent.GetComponent<Enemy>();
			}
			if (enemy != null)
			{
				return true;
			}
		}
		return false;
	}
	
	public void OnCollisionEnter(Collision collision)
	{
		if(IsLocal && !_activated && collision.collider.gameObject.GetComponent<PlayerController>() == null)
		{
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			_activated = true;
		}
	}
	
	void OnDestroy()
	{
		GameObject.Instantiate(_explosion, transform.position, transform.rotation);
	}
}