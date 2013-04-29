using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour
{
	private bool IsLocal { get; set; }
	
	[SerializeField] private PhotonView _photonView;
	
	[SerializeField] private float _rotationSpeed;
	[SerializeField] private GameObject _effect;
	[SerializeField] private float _checkInterval = 0.35f;
	[SerializeField] private float _pickupRadius = 1;
	private bool _used;
	private float _checkCounter;
	
	public MineLauncher ParentGun { get; set; }
	
	void Start()
	{
		IsLocal = _photonView == null || _photonView.isMine;
	}
	
	void Update()
	{
		transform.localEulerAngles += new Vector3(0, _rotationSpeed * Time.deltaTime, 0);
		
		if(IsLocal && !_used)
		{
			_checkCounter -= Time.deltaTime;
			if(_checkCounter <= 0.0f)
			{
				_checkCounter = _checkInterval;
				
				foreach(PlayerController player in PlayerController.GetAllPlayers())
					if((player.transform.position - transform.position).magnitude <= _pickupRadius)
					{
						PhotonNetwork.Destroy(_photonView);
						_used = true;
						player.RefillAmmo();
					}
			}
		}
	}
	
	void OnDestroy()
	{
		GameObject.Instantiate(_effect, transform.position, transform.rotation);
	}
	
	protected void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			
		}
		else
		{
			
		}
	}
}