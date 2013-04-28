using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minigun : NetworkedComponent
{
	[SerializeField] private float _range = 50;
	[SerializeField] private float _damage = 3;
	[SerializeField] private float _inaccuracy = 0.25f;
	[SerializeField] private float _fireInterval = 0.3f;
	
	[SerializeField] private Transform _firePoint;
	[SerializeField] private AudioSource _fireSound;
	[SerializeField] private GameObject _trail;
	
	private float _fireCounter;
	private LinkedList<Vector3> _fireTargets = new LinkedList<Vector3>();
	
	public void UpdateFiring(float delta, bool fire)
	{
		if(fire)
		{
			if(_fireCounter <= 0.0f)
			{
				Fire();
				_fireCounter += _fireInterval;
			}
		}
		_fireCounter -= delta;
		if(_fireCounter < 0.0f)
			_fireCounter = 0.0f;
	}
	
	public void Fire()
	{
		Vector3 direction = GetInaccuracy(_firePoint, _inaccuracy);
		Vector3 hit = _firePoint.position + (_range * direction);
		RaycastHit rH;
		if(Physics.Raycast(new Ray(_firePoint.position, direction), out rH, _range))
		{
			// Handle damage
			
			hit = rH.point;
		}
		FireEffect(_firePoint, hit);
		_fireTargets.AddLast(hit);
		_fireSound.Play();
	}
	
	private Vector3 GetInaccuracy(Transform source, float amount)
	{
		Vector3 result = source.forward;
		result += (Random.Range(-1.0f, 1.0f) * amount) * source.up;
		result += (Random.Range(-1.0f, 1.0f) * amount) * source.right;
		return result.normalized;
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
			FireEffect(_firePoint, target);
			_fireSound.Play();
		}
	}
}