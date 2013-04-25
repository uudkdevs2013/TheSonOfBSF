using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
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
		
		GameObject localPlayer;
		if (PhotonNetwork.isMasterClient)
		{
			print("level loader master client");
			localPlayer = PhotonNetwork.Instantiate("FirstPersonController", new Vector3(1006, 50, 1042), Quaternion.Euler(0, 0, 0), 0);
		}
		else
		{
			print("level loader not master client");
			localPlayer = PhotonNetwork.Instantiate("FirstPersonController", new Vector3(1010, 50, 1050), Quaternion.Euler(0, 0, 0), 0);
		}
		localPlayer.GetComponent<CharacterController>().enabled = true;
		localPlayer.GetComponent<MouseLook>().enabled = true;
		localPlayer.GetComponent<FPSInputController>().enabled = true;
		localPlayer.GetComponent<CharacterMotor>().enabled = true;
		localPlayer.GetComponentInChildren<Camera>().enabled = true;
		
		var cameraChild = localPlayer.transform.FindChild("Main Camera");
		cameraChild.GetComponent<MouseLook>().enabled = true;
		cameraChild.GetComponent<AudioListener>().enabled = true;
		
//		if (PhotonNetwork.isMasterClient)
//		{
//			yield return new WaitForSeconds(5);
//			var drone = PhotonNetwork.Instantiate("Drone", new Vector3(800, 100, 800), Quaternion.Euler(0, 0, 0), 0);
//			drone.GetComponent<Hover>().enabled = true;
//			var crawler = PhotonNetwork.Instantiate("Crawler", new Vector3(1017, 30, 1054), Quaternion.Euler(0, 0, 0), 0);
//			var groundEnemy = PhotonNetwork.Instantiate("GroundEnemy", new Vector3(1022, 30, 1054), Quaternion.Euler(0, 0, 0), 0);
//		}
	}
}
