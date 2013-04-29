using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
	[SerializeField] private ParticleEmitter[] _emitters;
	[SerializeField] private Light _light;
	[SerializeField] private float _duration;
	[SerializeField] private float _amount;
	private float _brightness;
	private float _radius;
	private float _counter;
	
	void Start()
	{
		foreach(ParticleEmitter emitter in _emitters)
		{
			emitter.Emit();
		}
		_brightness = _light.intensity;
		_radius = _light.range;
		_counter = 1f;
	}
	
	void Update()
	{
		_counter -= Time.deltaTime / _duration;
		if(_counter <= 0.0f)
		{
			Destroy(gameObject);
		}
		else
		{
			_light.intensity = Mathf.Lerp(0f, _brightness, _counter);
			_light.range = Mathf.Lerp(0f, _radius, _counter);
		}
	}
}
