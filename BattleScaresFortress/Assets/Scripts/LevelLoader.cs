using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	
	[SerializeField] private Camera _loaderCamera;
	[SerializeField] private GUITexture _crossHairs;
	
	public static LevelLoader Instance { get; private set; }
	private bool _isRespawningPlayer = false;
	private bool _playerIsDead = true;
	
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
	
	public static void ShowSpectatorCameras()
	{
		Instance._loaderCamera.enabled = true;
		Instance._crossHairs.enabled = false;
		Instance._playerIsDead = true;
	}
	
	public static void RespawnPlayer()
	{
		if (Instance._playerIsDead)
		{
			Instance._playerIsDead = false;
			Instance._isRespawningPlayer = true;
		}
	}
	
	private void OnGUI()
	{
		if (_isRespawningPlayer)
		{
			GUI.Label(new Rect(50, 50, 200, 20), "Select Your Class:");
			if (GUI.Button(new Rect(100, 100, 200, 30), "Sniper"))
			{
				_isRespawningPlayer = false;
				_loaderCamera.enabled = false;
				_crossHairs.enabled = true;
				SpawnSniper(new Vector3(1006, 50, 1042));
			}
			if (GUI.Button(new Rect(100, 140, 200, 30), "Tank"))
			{
				_isRespawningPlayer = false;
				_loaderCamera.enabled = false;
				_crossHairs.enabled = true;
				SpawnTank(new Vector3(1006, 50, 1042));
			}
			if (GUI.Button(new Rect(100, 180, 200, 30), "Medic"))
			{
				_isRespawningPlayer = false;
				_loaderCamera.enabled = false;
				_crossHairs.enabled = true;
				SpawnMedic(new Vector3(1006, 50, 1042));
			}
			if (GUI.Button(new Rect(100, 220, 200, 30), "Grenadier"))
			{
				_isRespawningPlayer = false;
				_loaderCamera.enabled = false;
				_crossHairs.enabled = true;
				SpawnGrenadier(new Vector3(1006, 50, 1042));
			}
		}
	}
	
}
