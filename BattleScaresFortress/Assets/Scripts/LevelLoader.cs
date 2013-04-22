using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	private void Start()
	{
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
			localPlayer = PhotonNetwork.Instantiate("FirstPersonController", new Vector3(0, 50, 0), Quaternion.Euler(0, 0, 0), 0);
		}
		else
		{
			localPlayer = PhotonNetwork.Instantiate("FirstPersonController", new Vector3(200, 50, 0), Quaternion.Euler(0, 0, 0), 0);
		}
		localPlayer.GetComponent<CharacterController>().enabled = true;
		localPlayer.GetComponent<MouseLook>().enabled = true;
		localPlayer.GetComponent<FPSInputController>().enabled = true;
		localPlayer.GetComponent<CharacterMotor>().enabled = true;
		localPlayer.GetComponentInChildren<Camera>().enabled = true;
		
		var cameraChild = localPlayer.transform.FindChild("Main Camera");
		cameraChild.GetComponent<MouseLook>().enabled = true;
		cameraChild.GetComponent<AudioListener>().enabled = true;
	}
}
