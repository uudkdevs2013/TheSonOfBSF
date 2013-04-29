using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MineLauncher : NetworkedComponent
{
	[SerializeField] private string _mine;
	[SerializeField] private float _shotCooldown = 1f;
	[SerializeField] private float _startVelocity = 20f;
	[SerializeField] private int _maxAmmo = 20;
	[SerializeField] private int _maxActiveMines = 5;
	
	[SerializeField] private AudioClip _fireSound;
	[SerializeField] private AudioClip _emptySound;
	[SerializeField] private GUISkin _guiSkin;
		
	[SerializeField] private Transform _firePoint;
	
	private int _ammo;
	private float _cooldown;
	private LinkedList<Mine> _mines = new LinkedList<Mine>();
	
	void Start()
	{
		_ammo = _maxAmmo;
	}
	
	void Update()
	{
		_cooldown -= Time.deltaTime;
	}
	
	public void RefillAmmo()
	{
		_ammo = _maxAmmo;
	}
	
	public void Fire()
	{
		if(_ammo > 0 && _cooldown <= 0.0f)
		{
			GameObject mineObj = (GameObject) PhotonNetwork.Instantiate(_mine, _firePoint.position, _firePoint.rotation, 0);
			mineObj.rigidbody.velocity = _firePoint.forward * _startVelocity;
			Mine mine = mineObj.GetComponent<Mine>();
			if(mine != null)
			{
				mine.ParentGun = this;
				_mines.AddLast(mine);
			}
			
			while(_mines.Count > _maxActiveMines)
			{
				_mines.First.Value.Explode();
			}
			
			audio.PlayOneShot(_fireSound);
			_cooldown = _shotCooldown;
			_ammo--;
		}
		else
		{
			audio.PlayOneShot(_emptySound);
		}
	}
	
	public void TriggerAllMines()
	{
		while(_mines.Count > 0)
		{
			_mines.First.Value.Explode();
		}
	}
	
	public void RemoveMine(Mine mine)
	{
		_mines.Remove(mine);
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
			
			GUI.Label(new Rect(0f, h * 0.85f, w * 0.25f, h * 0.15f), _ammo + " -- " + _mines.Count + " active");
			
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