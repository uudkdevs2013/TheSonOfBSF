using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
	private void OnGUI()
	{
		if (MatchMakingController.Instance.CurrentStatus == MatchMakingStatus.InRoom)
		{
			if (PhotonNetwork.isMasterClient)
			{
				int w = (int) (Screen.width * 0.25f);
				int h = (int) (Screen.height * 0.25f + 100);
				
				if (GUI.Button(new Rect(w - 50, h - 15, 100, 30), "Start Match"))
				{
					MatchMakingController.Instance.StartMatch();
				}
			}
		}
	}
}
