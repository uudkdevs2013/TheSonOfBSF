using UnityEngine;


// Require a character controller to be attached to the same game object
[RequireComponent (typeof(CharacterMotor))]
[AddComponentMenu ("Character/FPC Input Controller")]
public class FPSInputController : MonoBehaviour
{
	private CharacterMotor motor;
	
	[SerializeField] private PhotonView _photonView;
	private Vector3 _directionVector;
	private bool _jumpIsPressed;
	
	private void Awake()
	{
		motor = GetComponent<CharacterMotor>();
		_directionVector = Vector3.zero;
		_jumpIsPressed = false;
	}
	
	private void Update()
	{
		if (_photonView.isMine)
		{
			// Get the input vector from kayboard or analog stick
			_directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			
			if (_directionVector != Vector3.zero)
			{
				// Get the length of the directon vector and then normalize it
				// Dividing by the length is cheaper than normalizing when we already have the length anyway
				var directionLength = _directionVector.magnitude;
				_directionVector = _directionVector / directionLength;
				
				// Make sure the length is no bigger than 1
				directionLength = Mathf.Min(1, directionLength);
				
				// Make the input vector more sensitive towards the extremes and less sensitive in the middle
				// This makes it easier to control slow speeds when using analog sticks
				directionLength = directionLength * directionLength;
				
				// Multiply the normalized direction vector by the modified length
				_directionVector = _directionVector * directionLength;
			}
			
			_jumpIsPressed = Input.GetButton("Jump");
		}
		
		// Apply the direction to the CharacterMotor
		motor.InputMoveDirection = transform.rotation * _directionVector;
		motor.InputJump = _jumpIsPressed;
	}
	
	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// we own this player; send data
			
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(_directionVector);
			stream.SendNext(_jumpIsPressed);
		}
		else
		{
			// networked player; receive data
			transform.position = (Vector3)stream.ReceiveNext();
			transform.rotation = (Quaternion)stream.ReceiveNext();
			_directionVector = (Vector3)stream.ReceiveNext();
			_jumpIsPressed = (bool)stream.ReceiveNext();
		}
	}
	
}
