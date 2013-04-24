using UnityEngine;
using System.Collections;

public class NetworkMotionInterpolator : MonoBehaviour
{
	
	[SerializeField] private PhotonView _photonView;
	
	private Vector3 _oldPosition;
	private Vector3 _velocity;
	
	private void Start()
	{
		_oldPosition = transform.position;
		_velocity = Vector3.zero;
	}
	
	private void Update()
	{
		if (_photonView.isMine)
		{
			_velocity = Vector3.Normalize(transform.position - _oldPosition);
			_oldPosition = transform.position;
		}
		else
		{
			transform.position += _velocity * Time.deltaTime;
		}
	}
	
	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		print("on serialize view");
		if (stream.isWriting)
		{
			// we own this player; send data
			stream.SendNext(transform.position);
			stream.SendNext(_velocity);
		}
		else
		{
			// networked player; receive data
			transform.position = (Vector3)stream.ReceiveNext();
			_velocity = (Vector3)stream.ReceiveNext();
		}
	}
	
}
