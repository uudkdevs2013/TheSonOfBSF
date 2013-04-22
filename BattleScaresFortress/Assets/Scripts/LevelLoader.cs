using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	private void Start()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.Instantiate("FirstPersonController", new Vector3(1024, 100, 1024), Quaternion.Euler(0, 0, 0), 0);
			for (int i = 0; i < PhotonNetwork.otherPlayers.Length; ++i)
			{
				PhotonNetwork.Instantiate("FirstPersonController", new Vector3(i * 100 + 1024, 100, 1024), Quaternion.Euler(0, 0, 0), 0);
			}
		}
	}
}