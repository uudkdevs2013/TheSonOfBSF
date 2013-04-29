using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minigun : NetworkedComponent
{
	[SerializeField] private float _range = 50;
	[SerializeField] private float _damage = 3;
	[SerializeField] private float _inaccuracy = 0.25f;
	[SerializeField] private float _fireSpeedSlow = 0.7f;
	[SerializeField] private float _fireSpeedFast = 0.1f;
	[SerializeField] private float _fireSpeedRate = 0.3f;
	[SerializeField] private int _maxAmmo = 350;
	
	[SerializeField] private AudioClip _fireSound;
	[SerializeField] private AudioClip _emptySound;
	[SerializeField] private GUISkin _guiSkin;
	
	[SerializeField] private Transform _firePoint;
	[SerializeField] private GameObject _trail;
	
	private int _ammo;
	private float _fireSpeed;
	private float _fireCounter;
	private LinkedList<Vector3> _fireTargets = new LinkedList<Vector3>();
	
	void Start()
	{
		_ammo = _maxAmmo;
	}
	
	public void UpdateFiring(float delta, bool fire)
	{
		if(fire)
		{
			if(_fireCounter <= 0.0f)
			{
				Fire();
				_fireCounter += 1f;
			}
		}
		_fireCounter -= (delta / _fireSpeed);
		if(_fireCounter < 0.0f)
			_fireCounter = 0.0f;
		
		if(fire)
			_fireSpeed -= _fireSpeedRate * delta;
		else
			_fireSpeed += _fireSpeedRate * delta;
		_fireSpeed = Mathf.Clamp(_fireSpeed, _fireSpeedFast, _fireSpeedSlow);
	}
	
	public void Fire()
	{
		if(_ammo > 0)
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
			audio.PlayOneShot(_fireSound);
			_ammo--;
		}
		else
		{
			audio.PlayOneShot(_emptySound);
		}
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
	
	void OnGUI()
	{
		if(IsLocal)
		{
			GUI.skin = _guiSkin;
			int w = Screen.width;
			int h = Screen.height;
			GUI.Label(new Rect(w * 0.75f, h * 0.85f, w * 0.25f, h * 0.15f), "" + _ammo);
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
			audio.PlayOneShot(_fireSound);
		}
	}
}