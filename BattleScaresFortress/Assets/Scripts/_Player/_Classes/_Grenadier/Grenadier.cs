using UnityEngine;
using System.Collections;

public class Grenadier : PlayerController
{
	[SerializeField] private Shotgun _shotgun;
	[SerializeField] private MineLauncher _mineLauncher;
	
	protected override void Start()
	{
		base.Start();
		Debug.Log("Grenadier is local: " + IsLocal);
	}
	
	protected override void Update()
	{
		base.Update();
		
		if(IsLocal)
		{
			if(Input.GetMouseButtonDown(1))
			{
				_mineLauncher.Fire();
			}
			
			if(Input.GetKeyDown(KeyCode.Tab))
			{
				_mineLauncher.TriggerAllMines();
			}
			
			if(Input.GetMouseButtonDown(0))
			{
				_shotgun.Fire();
			}
			
			if(Input.GetKeyDown(KeyCode.R))
			{
				_shotgun.Reload();
			}
		}
	}
	
	public override void PerformAmmoRefill()
	{
		_shotgun.RefillAmmo();
		_mineLauncher.RefillAmmo();
	}
}