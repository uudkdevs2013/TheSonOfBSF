using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	
//	[SerializeField] private Camera _loaderCamera;
	[SerializeField] private GUITexture _crossHairs;
	
	public static LevelLoader Instance { get; private set; }
	private bool _isRespawningPlayer = false;
	private float _timeUntilRespawn = 10f;
	
	private void Awake()
	{
		Instance = this;
	}
	
	private void Start()
	{
		PhotonNetwork.isMessageQueueRunning = true;
		RPCManager.Instance.MyLevelIsLoaded();
		StartCoroutine(WaitAndSpawnPlayer());
	}
	
	private IEnumerator WaitAndSpawnPlayer()
	{
		while (!RPCManager.Instance.AllLevelsAreLoaded)
		{
			yield return null;
		}
		
		RespawnPlayer();
		
//		if (PhotonNetwork.isMasterClient)
//		{
//			SpawnSniper(new Vector3(1006, 50, 1042));
//		}
//		else
//		{
//			SpawnSniper(new Vector3(1010, 50, 1050));
//		}
	}
	
	private void Update()
	{
		if (_isRespawningPlayer)
		{
			_timeUntilRespawn -= Time.deltaTime;
		}
	}
	
	private GameObject SpawnSniper(Vector3 position)
	{
		return PhotonNetwork.Instantiate("Players/Sniper", position, Quaternion.Euler(0, 0, 0), 0);
	}
	
	private GameObject SpawnTank(Vector3 position)
	{
		return PhotonNetwork.Instantiate("Players/Tank", position, Quaternion.Euler(0, 0, 0), 0);
	}
	
	private GameObject SpawnGrenadier(Vector3 position)
	{
		return PhotonNetwork.Instantiate("Players/Grenadier", position, Quaternion.Euler(0, 0, 0), 0);
	}
	
	private GameObject SpawnMedic(Vector3 position)
	{
		return PhotonNetwork.Instantiate("Players/Medic", position, Quaternion.Euler(0, 0, 0), 0);
	}
	
	public static void RespawnPlayer()
	{
		Instance.RespawnLocalPlayer();
	}
	
	private void RespawnLocalPlayer()
	{
		SpawnSniper(new Vector3(1006, 50, 1042));
//		print("respawning player");
//		_isRespawningPlayer = true;
////		_loaderCamera.enabled = true;
//		_crossHairs.enabled = false;
//		_timeUntilRespawn = 5f;
	}
	
	private void OnGUI()
	{
		if (_isRespawningPlayer)
		{
			if (_timeUntilRespawn > 0)
			{
				int timeLeft = (int)_timeUntilRespawn;
				GUI.Label(new Rect(50, 50, 200, 20), "Time until respawn: " + timeLeft);
			}
			else
			{
				GUI.Label(new Rect(50, 50, 200, 20), "Select Your Class:");
				if (GUI.Button(new Rect(100, 100, 200, 30), "Sniper"))
				{
					_isRespawningPlayer = false;
//					_loaderCamera.enabled = false;
					_crossHairs.enabled = true;
					SpawnSniper(new Vector3(1006, 50, 1042));
				}
				if (GUI.Button(new Rect(100, 140, 200, 30), "Tank"))
				{
					_isRespawningPlayer = false;
//					_loaderCamera.enabled = false;
					_crossHairs.enabled = true;
					SpawnTank(new Vector3(1006, 50, 1042));
				}
				if (GUI.Button(new Rect(100, 180, 200, 30), "Grenadier"))
				{
					_isRespawningPlayer = false;
//					_loaderCamera.enabled = false;
					_crossHairs.enabled = true;
					SpawnGrenadier(new Vector3(1006, 50, 1042));
				}
			}
		}
	}
	
}
