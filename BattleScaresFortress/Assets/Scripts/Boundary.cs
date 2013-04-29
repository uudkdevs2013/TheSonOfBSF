using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour {

	void OnTriggerExit (Collider obj) {
		GameObject o = obj.gameObject;
		if (o.GetComponent<AstarAI>())
			o.SendMessage("SetInPath", false);
		
	}
	
	void OnTriggerEnter (Collider obj) {
		GameObject o = obj.gameObject;
		if (o.GetComponent<AstarAI>())
			o.SendMessage("SetInPath", true);
	}
}
