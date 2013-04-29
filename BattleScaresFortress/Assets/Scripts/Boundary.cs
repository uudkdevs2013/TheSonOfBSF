using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour
{

//	void OnTriggerExit(Collider obj)
//	{
//		GameObject o = obj.gameObject;
//		if (o.GetComponent<AstarAI>())
//		{
//			o.SendMessage("SetInPath", false);
//		}
//		
//	}
//	
//	void OnTriggerEnter(Collider obj)
//	{
//		GameObject o = obj.gameObject;
//		if (o.GetComponent<AstarAI>())
//		{
//			o.SendMessage("SetInPath", true);
//		}
//	}
	
	[SerializeField] private Vector2 _centerPoint;
	[SerializeField] private float _radius;
	
	private void Update()
	{
		foreach (var enemy in Enemy.AllEnemies())
		{
			var crawler = enemy.GetComponent<Crawler>();
			if (crawler != null)
			{
				var crawlerPosition = new Vector2(crawler.transform.position.x, crawler.transform.position.z);
				crawler.gameObject.SendMessage("SetInPath", Vector2.Distance(_centerPoint, crawlerPosition) <= _radius);
			}
		}
	}
	
}
