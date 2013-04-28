using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SniperRifles : NetworkedComponent
{
	[SerializeField] private float _range = 200;
	[SerializeField] private float _damage = 15;
	[SerializeField] private Transform[] _firePoints;
	[SerializeField] private AudioSource _fireSound;
	[SerializeField] private GameObject _trail;
	private LinkedList<Vector3> _fireTargets = new LinkedList<Vector3>();
	
	public void Fire()
	{
		Vector3 hit = transform.position + (_range * transform.forward);
		foreach(Transform source in _firePoints)
		{
			RaycastHit rH;
			if(Physics.Raycast(new Ray(source.position, source.forward), out rH, _range))
			{
				// Handle damage
				print("raycast hit");
				var enemy = rH.collider.gameObject.GetComponent<Enemy>();
				print("enemy: " + enemy);
				if (enemy != null)
				{
					enemy.ApplyDamage(_damage);
				}
				
				hit = rH.point;
				FireEffect(source, rH.point);
			}
			else
			{
				FireEffect(source, source.position + (_range * source.forward));
			}
		}
		_fireTargets.AddLast(hit);
		_fireSound.Play();
	}
	
	public void FireEffect(Transform source, Vector3 target)
	{
		GameObject instance = (GameObject) GameObject.Instantiate(_trail);
		BulletTrail trail = instance.GetComponent<BulletTrail>();
		if(trail != null)
		{
			trail.Position(source.position, target);
		}
	}
	
	
	public override bool NeedsSend()
	{
		return _fireTargets.Count > 0;
	}
	
	public override void SendData(PhotonStream stream, PhotonMessageInfo info)
	{
		int numTargets = _fireTargets.Count;
		stream.SendNext(numTargets);
		for(int i = 0; i < numTargets; i++)
		{
			stream.SendNext(_fireTargets.First.Value);
			_fireTargets.RemoveFirst();
		}
	}
	
	public override void ReadData(PhotonStream stream, PhotonMessageInfo info)
	{
		int numTargets = (int) stream.ReceiveNext();
		for(int i = 0; i < numTargets; i++)
		{
			Vector3 target = (Vector3) stream.ReceiveNext();
			foreach(Transform source in _firePoints)
			{
				FireEffect(source, target);
			}
			_fireSound.Play();
		}
	}
}