using UnityEngine;
using System.Collections;

public class HealSphereGun : NetworkedComponent
{
	[SerializeField] private Transform _firePoint;
	[SerializeField] private float _cooldownTime = 20;
	private float _timeLeftInCooldown = 0;
	
	public void Fire()
	{
		if (_timeLeftInCooldown <= 0)
		{
			object[] instationData = new object[] { _firePoint.forward };
			PhotonNetwork.Instantiate("Effects/Medigun/HealingSphere", _firePoint.position, Quaternion.Euler(0, 0, 0), 0, instationData);
			_timeLeftInCooldown = _cooldownTime;
		}
	}
	
	private void Update()
	{
		if (_timeLeftInCooldown > 0)
		{
			_timeLeftInCooldown -= Time.deltaTime;
		}
	}
	
	public void OnGUI()
	{
		if(IsLocal)
		{
			GUI.skin = _guiSkin;
			int w = Screen.width;
			int h = Screen.height;
			
			if (_timeLeftInCooldown <= 0)
			{
				GUI.color = Color.white;
				GUI.Label(new Rect(0f, h * 0.85f, w * 0.25f, h * 0.15f), "Heal Ready");
			}
			else
			{
				GUI.color = Color.red;
				GUI.Label(new Rect(0f, h * 0.85f, w * 0.25f, h * 0.15f), "Recharging: " + (int) _timeLeftInCooldown);
			}
			
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
