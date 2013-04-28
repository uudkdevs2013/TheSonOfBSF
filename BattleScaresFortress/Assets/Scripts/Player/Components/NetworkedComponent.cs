using UnityEngine;
using System.Collections;

public abstract class NetworkedComponent : MonoBehaviour
{
	public abstract bool NeedsSend();
	public abstract void SendData(PhotonStream stream, PhotonMessageInfo info);
	public abstract void ReadData(PhotonStream stream, PhotonMessageInfo info);
}