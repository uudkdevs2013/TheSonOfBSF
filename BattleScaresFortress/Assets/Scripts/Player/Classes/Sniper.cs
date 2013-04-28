using UnityEngine;
using System.Collections;

public class Sniper : PlayerController
{
	[SerializeField] private Camera _camera;
	
	[SerializeField] private float _focusFoV;
	private float _baseFoV;
	
	[SerializeField] private Vector2 _focusSensitivity;
	private Vector2 _baseSensitivity;
	
	public void Start()
	{
		base.Start();
		_baseFoV = _camera.fov;
		_baseSensitivity = _aimSensitivity;
	}
	
	public void Update()
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
		}
	}
}
