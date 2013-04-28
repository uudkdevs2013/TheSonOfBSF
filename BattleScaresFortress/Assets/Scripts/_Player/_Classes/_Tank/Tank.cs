using UnityEngine;
using System.Collections;

public class Tank : PlayerController
{
	[SerializeField] private Minigun _minigun;
	
	protected override void Start()
	{
		base.Start();
	}
	
	protected override void Update()
	{
		base.Update();
		
		if(IsLocal)
		{
			float delta = Time.deltaTime;
			_minigun.UpdateFiring(delta, Input.GetMouseButton(0));
		}
	}
}