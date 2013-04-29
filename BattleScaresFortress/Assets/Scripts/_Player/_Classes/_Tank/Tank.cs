using UnityEngine;
using System.Collections;

public class Tank : PlayerController
{
	[SerializeField] private Minigun _minigun;
	[SerializeField] private RocketLauncher _rocketLauncher;
	
	protected override void Start()
	{
		base.Start();
		Debug.Log("Tank is local: " + IsLocal);
	}
	
	protected override void Update()
	{
		base.Update();
		
		if(IsLocal)
		{
			float delta = Time.deltaTime;
			_minigun.UpdateFiring(delta, Input.GetMouseButton(0));
			if(Input.GetMouseButtonDown(1))
			{
				_rocketLauncher.Fire();
			}
		}
	}
}