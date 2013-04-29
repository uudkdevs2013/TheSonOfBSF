using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour
{
	
	[SerializeField] private Vector2 _centerPoint;
	[SerializeField] private float _radius;
	
	private void Update()
	{
		if (Enemy.AllEnemies() != null)
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
	
}
