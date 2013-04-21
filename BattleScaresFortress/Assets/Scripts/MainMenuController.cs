using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
	
	private void OnGUI()
	{
		GUI.Label(new Rect(20, 20, 200, 30), "Battle Scares Fortress");
		if (MatchMakingController.Instance.CurrentStatus == MatchMakingStatus.InRoom)
		{
			if (PhotonNetwork.isMasterClient)
			{
				if (GUI.Button(new Rect(40, 80, 100, 30), "Start Match"))
				{
					RPCManager.Instance.StartMatch();
				}
			}
		}
	}
	
}
