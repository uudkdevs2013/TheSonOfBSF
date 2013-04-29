using UnityEngine;
using System.Collections;

public class Medic : PlayerController
{
	[SerializeField] private Pistol _pistol;
	
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
			if(Input.GetMouseButton(1))
			{
				_pistol.Fire();
			}
			
			if(Input.GetMouseButtonDown(0))
			{
				
			}
			
			if(Input.GetKeyDown(KeyCode.R))
			{
				_pistol.Reload();
			}
		}
	}
}