using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	private CharacterMotor _motor;
	
	[SerializeField] private PhotonView _photonView;
	[SerializeField] private Transform _leftGun;
	[SerializeField] private Transform _rightGun;
	[SerializeField] private Transform _direction;
	[SerializeField] private NetworkedComponent[] _components;
	
	[SerializeField] private GameObject[] _disableIfLocal;
	[SerializeField] private GameObject[] _disableIfNetworked;
	
	public bool IsLocal { get; set; }
	
	private float _aimDistance = 200;
	private Vector3 _movement;
	private bool _jump;
	private float _minAimY = -60;
	private float _maxAimY = 60;
	
	protected Vector2 _aim = new Vector2(0, 0);
	protected Vector2 _aimSensitivity = new Vector2(15, 15);
	
	public virtual void Start()
	{
		_aim.x = transform.localEulerAngles.y;
		_aim.y = -_direction.localEulerAngles.x;
		_motor = GetComponent<CharacterMotor>();
		
		IsLocal = _photonView == null || _photonView.isMine;
		if(IsLocal)
		{
			foreach(GameObject obj in _disableIfLocal)
				obj.SetActive(false);
		}
		else
		{
			foreach(GameObject obj in _disableIfNetworked)
				obj.SetActive(false);
		}
	}
	
	public virtual void Update()
	{
		if(IsLocal)
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
		_aim.x += (Input.GetAxis("Mouse X") * _aimSensitivity.x);
		_aim.y += Input.GetAxis("Mouse Y") * _aimSensitivity.y;
		_aim.y = Mathf.Clamp (_aim.y, _minAimY, _maxAimY);
		
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
		transform.localEulerAngles = new Vector3(0, _aim.x, 0);
		_direction.localEulerAngles = new Vector3(-_aim.y, 0, 0);
		Vector3 target = _direction.position + (_direction.forward * _aimDistance);
		_rightGun.LookAt(target);
		_leftGun.LookAt(target);
	}
	
	private void UpdateMovement()
	{
		_motor.InputMoveDirection = transform.rotation * _movement;
		_motor.InputJump = _jump;
	}
	
	protected void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// we own this player; send data
			stream.SendNext(transform.position);
			stream.SendNext(_aim);
			stream.SendNext(_movement);
			stream.SendNext(_jump);
			foreach(NetworkedComponent component in _components)
			{
				if(component.NeedsSend())
				{
					stream.SendNext(true);
					component.SendData(stream, info);
				}
				else
				{
					stream.SendNext(false);
				}
			}
		}
		else
		{
			// networked player; receive data
			transform.position = (Vector3)stream.ReceiveNext();
			_aim = (Vector2)stream.ReceiveNext();
			_movement = (Vector3)stream.ReceiveNext();
			_jump = (bool)stream.ReceiveNext();
			
			foreach(NetworkedComponent component in _components)
			{
				if((bool) stream.ReceiveNext())
				{
					component.ReadData(stream, info);
				}
			}
		}
	}
}