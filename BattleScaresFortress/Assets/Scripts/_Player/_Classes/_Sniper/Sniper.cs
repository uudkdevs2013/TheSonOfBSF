using UnityEngine;
using System.Collections;

public class Sniper : PlayerController
{
	[SerializeField] private Camera _camera;
	
	[SerializeField] private float _focusFoV;
	private float _baseFoV;
	
	[SerializeField] private Vector2 _focusSensitivity;
	private Vector2 _baseSensitivity;
	
	[SerializeField] private SniperRifles _rifles;
	
	protected override void Start()
	{
		base.Start();
		Debug.Log("Sniper is local: " + IsLocal);
		_baseFoV = _camera.fov;
		_baseSensitivity = _aimSensitivity;
	}
	
	protected override void Update()
	{
		base.Update();
		
		if(IsLocal)
		{
			if(Input.GetMouseButton(1))
			{
				_camera.fov = _focusFoV;
				_aimSensitivity = _focusSensitivity;
			}
			else
			{
				_camera.fov = _baseFoV;
				_aimSensitivity = _baseSensitivity;
			}
			
			if(Input.GetMouseButtonDown(0))
			{
				_rifles.Fire();
			}
			
			if(Input.GetKeyDown(KeyCode.R))
			{
				_rifles.Reload();
			}
		}
	}
}
