using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour
{
	private bool IsLocal { get; set; }
	
	[SerializeField] private PhotonView _photonView;
	[SerializeField] private GameObject _explosion;
	[SerializeField] private float _damage = 20;
	[SerializeField] private float _maxRange = 80;
	[SerializeField] private float _explosionRadius = 8;
	private Vector3 _prevLoc;
	private bool _exploded;
	
	void Start()
	{
		IsLocal = _photonView == null || _photonView.isMine;
		_prevLoc = transform.position;
	}
	
	void Update()
	{
		_maxRange -= (_prevLoc - transform.position).magnitude;
		if(IsLocal && _maxRange <= 0.0f)
		{
			Explode();
		}
	}
	
	public void OnCollisionEnter(Collision collision)
	{
		if(IsLocal)
		{
			Explode();
		}
	}
	
	public void Explode()
	{
		if(!_exploded)
		{
			RaycastHit[] hits = Physics.SphereCastAll(transform.position, _explosionRadius, Vector3.up, _explosionRadius);
			Debug.Log("Rocket hit " + hits.Length + " targets!");
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
			PhotonNetwork.Destroy(_photonView);
			_exploded = true;
		}
	}
	
	void OnDestroy()
	{
		GameObject.Instantiate(_explosion, transform.position, transform.rotation);
	}
}