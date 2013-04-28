using UnityEngine;
using System.Collections;

public class Rotater : MonoBehaviour
{
	[SerializeField] private float _rotationSpeed = 10f;
	
	void Update()
	{
		Vector3 rotation = transform.localEulerAngles;
		rotation.y += _rotationSpeed * Time.deltaTime;
		transform.localEulerAngles = rotation;
	}
}
