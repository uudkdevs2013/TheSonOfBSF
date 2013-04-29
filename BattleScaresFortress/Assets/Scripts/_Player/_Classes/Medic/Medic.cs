using UnityEngine;
using System.Collections;

public class Medic : PlayerController
{
	[SerializeField] private Pistol _pistol;
	[SerializeField] private HealSphereGun _healGun;
	
	protected override void Start()
	{
		base.Start();
		Debug.Log("Medic is local: " + IsLocal);
	}
	
	protected override void Update()
	{
		base.Update();
		
		if(IsLocal)
		{
			if(Input.GetMouseButtonDown(1))
			{
				_pistol.Fire();
			}
			
			if(Input.GetMouseButtonDown(0))
			{
				_healGun.Fire();
			}
			
			if(Input.GetKeyDown(KeyCode.R))
			{
				_pistol.Reload();
			}
		}
	}
	
	public override void PerformAmmoRefill()
	{
		_pistol.RefillAmmo();
	}
}