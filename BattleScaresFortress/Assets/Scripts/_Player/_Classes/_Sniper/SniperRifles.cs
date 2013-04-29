using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SniperRifles : NetworkedComponent
{
	[SerializeField] private float _range = 200;
	[SerializeField] private float _damage = 15;
	[SerializeField] private float _shotCooldown = 1f;
	[SerializeField] private float _reloadTime = 3f;
	[SerializeField] private int _clipSize = 6;
	[SerializeField] private int _maxAmmo = 42;
	
	[SerializeField] private AudioClip _fireSound;
	[SerializeField] private AudioClip _emptySound;
	[SerializeField] private AudioClip _reloadSound;
	[SerializeField] private GUISkin _guiSkin;
	
	private int _ammoInClip;
	private int _ammo;
	private float _cooldown;
	private bool _reloading;
	
	[SerializeField] private Transform[] _firePoints;
	[SerializeField] private GameObject _trail;
	private LinkedList<Vector3> _fireTargets = new LinkedList<Vector3>();
	
	void Start()
	{
		_ammoInClip = _clipSize;
		_ammo = _maxAmmo;
	}
	
	void Update()
	{
		_cooldown -= Time.deltaTime;
	}
	
	public void RefillAmmo()
	{
		_ammo = _maxAmmo;
		if(_ammoInClip == 0 && _ammo > 0)
			Reload();
	}
	
	public void Reload()
	{
		if(!_reloading && _ammoInClip < _ammo && _ammo > 0)
		{
			_reloading = true;
			_ammo += _ammoInClip;
			_ammoInClip = 0;
			audio.PlayOneShot(_reloadSound);
			StartCoroutine(ReloadDelay());
		}
	}
	
	private IEnumerator ReloadDelay()
	{
		yield return new WaitForSeconds(_reloadTime);
		_reloading = false;
		_ammoInClip = Mathf.Min(_clipSize, _ammo);
		_ammo -= _ammoInClip;
	}
	
	public void Fire()
	{
		if(_ammoInClip > 0 && _cooldown <= 0.0f)
		{
			Vector3 hit = transform.position + (_range * transform.forward);
			foreach(Transform source in _firePoints)
			{
				RaycastHit rH;
				if(Physics.Raycast(new Ray(source.position, source.forward), out rH, _range))
				{
					// Handle damage
					var enemy = rH.collider.gameObject.GetComponent<Enemy>();
					if (enemy == null && rH.collider.transform.parent != null)
					{
						enemy = rH.collider.transform.parent.GetComponent<Enemy>();
					}
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
			audio.PlayOneShot(_fireSound);
			
			_cooldown = _shotCooldown;
			_ammoInClip--;
		}
		else
		{
			audio.PlayOneShot(_emptySound);
			if(_ammoInClip == 0 && _ammo > 0)
				Reload();
		}
		if(_ammoInClip == 0 && _ammo > 0)
			Reload();
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
			GUI.Label(new Rect(w * 0.75f, h * 0.85f, w * 0.25f, h * 0.15f), _ammoInClip + " / " + _ammo);
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
			audio.PlayOneShot(_fireSound);
		}
	}
}