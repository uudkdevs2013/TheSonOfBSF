using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	
	private void Start()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.Instantiate("First Person Controller", new Vector3(-100, 100, 0), Quaternion.Euler(0, 0, 0), 0);
			for (int i = 0; i < PhotonNetwork.otherPlayers.Length; ++i)
			{
				PhotonNetwork.Instantiate("First Person Controller", new Vector3(i * 100, 100, 0), Quaternion.Euler(0, 0, 0), 0);
			}
		}
	}
	
}
