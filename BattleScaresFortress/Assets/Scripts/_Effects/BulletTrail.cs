using UnityEngine;
using System.Collections;

public class BulletTrail : MonoBehaviour
{
	[SerializeField] private LineRenderer _line;
	[SerializeField] private Color _startColor;
	[SerializeField] private Color _endColor;
	[SerializeField] private float _duration;
	private float _counter = 1f;
	
	public void Position(Vector3 start, Vector3 end)
	{
		_line.SetPosition(0, start);
		_line.SetPosition(1, end);
		_counter = 1f;
	}
	
	void Update()
	{
		_counter -= Time.deltaTime / _duration;
		if(_counter <= 0.0f)
		{
			GameObject.Destroy(gameObject);
			GameObject.Destroy(this);
		}
		else
		{
			Color color = Color.Lerp(_endColor, _startColor, _counter);
			_line.SetColors(color, color);
		}
	}
}