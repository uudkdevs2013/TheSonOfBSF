using UnityEngine;
using System.Collections;

public class Grenadier : PlayerController
{
	[SerializeField] private Shotgun _shotgun;
	
	protected override void Start()
	{
		base.Start();
	}
	
	protected override void Update()
	{
		base.Update();
		
		if(IsLocal)
		{
			if(Input.GetMouseButton(1))
			{
				
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
}