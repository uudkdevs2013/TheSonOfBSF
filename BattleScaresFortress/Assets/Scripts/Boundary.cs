using UnityEngine;
using System.Collections;

// Boundary used to switch between AI an simple follow behavior.
// Once the Crawler enters the Fortress area, it will enable IA (OnTriggerIn)
// Once it exits, it will enable simple follow (OnTriggerExit)
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
