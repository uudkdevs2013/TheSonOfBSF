using UnityEngine;
using System.Collections;

public class DeathSpawn : MonoBehaviour
{
	[SerializeField] private GameObject _toSpawn;
	
	void OnDestroy()
	{
		GameObject.Instantiate(_toSpawn, transform.position, transform.rotation);
	}
}