using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	private CharacterMotor _motor;
	
	[SerializeField] private PhotonView _photonView;
	[SerializeField] private Transform _leftGun;
	[SerializeField] private Transform _rightGun;
	[SerializeField] private Transform _direction;
	
	[SerializeField] private Camera _camera;
	[SerializeField] private GameObject _body;
	
	private float _aimX;
	private float _aimY;
	private float _aimDistance = 200;
	private Vector3 _movement;
	private bool _jump;
	private float _minAimY = -60;
	private float _maxAimY = 60;
	private float _aimSensitivityX = 15;
	private float _aimSensitivityY = 15;
	
	void Start()
	{
		_aimX = transform.localEulerAngles.y;
		_aimY = -_direction.localEulerAngles.x;
		_motor = GetComponent<CharacterMotor>();
		_camera.enabled = (_photonView == null || _photonView.isMine);
		_body.SetActive(_photonView != null && !_photonView.isMine);
	}
	
	void Update()
	{
		if(_photonView == null || _photonView.isMine)
		{
			UpdateControls();
		}
		UpdateAiming();
		UpdateMovement();
	}
	
	private void UpdateControls()
	{
		// Movement
		_movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if (_movement != Vector3.zero)
		{
			float magnitude = _movement.magnitude;
			_movement = _movement / magnitude;
			magnitude = Mathf.Min(1, magnitude);
			magnitude = magnitude * magnitude;
			_movement = _movement * magnitude;
		}
		_jump = Input.GetButton("Jump");
		
		//Aiming
		_aimX += (Input.GetAxis("Mouse X") * _aimSensitivityX);
		_aimY += Input.GetAxis("Mouse Y") * _aimSensitivityY;
		_aimY = Mathf.Clamp (_aimY, _minAimY, _maxAimY);
		
		RaycastHit rH;
		if(Physics.Raycast(new Ray(_direction.transform.position, _direction.transform.forward), out rH, 200))
		{
			_aimDistance = rH.distance;
		}
		else
			_aimDistance = 200;
	}
	
	private void UpdateAiming()
	{
		transform.localEulerAngles = new Vector3(0, _aimX, 0);
		_direction.localEulerAngles = new Vector3(-_aimY, 0, 0);
		Vector3 target = _direction.position + (_direction.forward * _aimDistance);
		_rightGun.LookAt(target);
		_leftGun.LookAt(target);
	}
	
	private void UpdateMovement()
	{
		_motor.InputMoveDirection = transform.rotation * _movement;
		_motor.InputJump = _jump;
	}
}