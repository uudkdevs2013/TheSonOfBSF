using UnityEngine;
using System.Collections;

public abstract class NetworkedComponent : MonoBehaviour
{
	public bool IsLocal { get; set; }
	public abstract bool NeedsSend();
	public abstract void SendData(PhotonStream stream, PhotonMessageInfo info);
	public abstract void ReadData(PhotonStream stream, PhotonMessageInfo info);
}