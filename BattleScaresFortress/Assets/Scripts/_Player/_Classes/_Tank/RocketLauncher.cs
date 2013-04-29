using UnityEngine;
using System.Collections;

public class RocketLauncher : NetworkedComponent
{
	[SerializeField] private string _rocket;
	[SerializeField] private float _shotCooldown = 0.5f;
	[SerializeField] private float _startVelocity = 20f;
	[SerializeField] private int _maxAmmo = 48;
	
	[SerializeField] private AudioClip _fireSound;
	[SerializeField] private AudioClip _emptySound;
	[SerializeField] private GUISkin _guiSkin;
	
	private int _ammo;
	private float _cooldown;
	
	[SerializeField] private Transform _firePoint;
	
	void Start()
	{
		_ammo = _maxAmmo;
	}
	
	void Update()
	{
		_cooldown -= Time.deltaTime;
	}
	
	public void Fire()
	{
		if(_ammo > 0 && _cooldown <= 0.0f)
		{
			GameObject rocket = (GameObject) PhotonNetwork.Instantiate(_rocket, _firePoint.position, _firePoint.rotation, 0);
			rocket.rigidbody.velocity = _firePoint.forward * _startVelocity;
			
			audio.PlayOneShot(_fireSound);
			_cooldown = _shotCooldown;
			_ammo--;
		}
		else
		{
			audio.PlayOneShot(_emptySound);
		}
	}
	
	void OnGUI()
	{
		if(IsLocal)
		{
			GUI.skin = _guiSkin;
			int w = Screen.width;
			int h = Screen.height;
			
			if(_ammo > 0 && _cooldown <= 0.0f)
				GUI.color = Color.white;
			else
				GUI.color = Color.red;
			
			GUI.Label(new Rect(0f, h * 0.85f, w * 0.25f, h * 0.15f), "" + _ammo);
			
			GUI.color = Color.white;
		}
	}
	
	public override bool NeedsSend()
	{
		return false;
	}
	
	public override void SendData(PhotonStream stream, PhotonMessageInfo info)
	{
		
	}
	
	public override void ReadData(PhotonStream stream, PhotonMessageInfo info)
	{
		
	}
}