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
