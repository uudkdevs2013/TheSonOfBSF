using UnityEngine;
using System.Collections;

public class Shotgun : NetworkedComponent
{
	[SerializeField] private float _range = 15;
	[SerializeField] private float _damage = 2;
	[SerializeField] private float _inaccuracy = 0.25f;
	[SerializeField] private float _shotCooldown = 0.5f;
	[SerializeField] private float _reloadTime = 2f;
	[SerializeField] private int _numBullets = 12;
	[SerializeField] private int _clipSize = 6;
	[SerializeField] private int _maxAmmo = 30;
	
	[SerializeField] private AudioClip _fireSound;
	[SerializeField] private AudioClip _emptySound;
	[SerializeField] private AudioClip _reloadSound;
	[SerializeField] private GUISkin _guiSkin;
	
	private int _ammoInClip;
	private int _ammo;
	private float _cooldown;
	private bool _reloading;
	
	[SerializeField] private Transform _firePoint;
	[SerializeField] private GameObject _trail;
	private int _numShotsFired;
	
	void Start()
	{
		_ammoInClip = _clipSize;
		_ammo = _maxAmmo;
	}
	
	void Update()
	{
		_cooldown -= Time.deltaTime;
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
			for(int i = 0; i < _numBullets; i++)
			{
				Vector3 dir = GetInaccuracy(_firePoint, _inaccuracy);
				RaycastHit rH;
				if(Physics.Raycast(new Ray(_firePoint.position, dir), out rH, _range))
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
				}
			}
			
			FireEffect(_firePoint);
			_numShotsFired++;
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
	}
	
	private Vector3 GetInaccuracy(Transform source, float amount)
	{
		Vector3 result = source.forward;
		result += (Random.Range(-1.0f, 1.0f) * amount) * source.up;
		result += (Random.Range(-1.0f, 1.0f) * amount) * source.right;
		return result.normalized;
	}
	
	public void FireEffect(Transform source)
	{
		for(int i = 0; i < _numBullets; i++)
		{
			Vector3 dir = GetInaccuracy(source, _inaccuracy);
			GameObject instance = (GameObject) GameObject.Instantiate(_trail);
			BulletTrail trail = instance.GetComponent<BulletTrail>();
			if(trail != null)
			{
				trail.Position(source.position, source.position + (dir * _range));
			}
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
		return _numShotsFired > 0;
	}
	
	public override void SendData(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(_numShotsFired);
	}
	
	public override void ReadData(PhotonStream stream, PhotonMessageInfo info)
	{
		int numTargets = (int) stream.ReceiveNext();
		for(int i = 0; i < numTargets; i++)
		{
			FireEffect(_firePoint);
			audio.PlayOneShot(_fireSound);
		}
	}
}